﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

using Microsoft.Practices.Unity;

using Microsoft.WPSync.Shared;
using Microsoft.WPSync.Settings;
using Microsoft.WPSync.Device;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;


namespace MusicCollection.WindowsPhone
{
    internal class WindowsPhoneExporter : UIThreadSafeImportEventAdapter, IMusicExporter
    {
        private IMusicSession _Ims;
        private IImportContext _IIC;
        private bool _IsCopying = false;
        private CancellationTokenSource _IsCancelled = new CancellationTokenSource();
        private object _CopyingLocker = new object();
        private static readonly SemaphoreSlim _sem;

        public WindowsPhoneExporter(IInternalMusicSession ims)
        {
            _Ims = ims;
            _IIC = ims.GetNewSessionContext();
            _IIC.Error += ((o, e) => OnError(e));
        }

        static WindowsPhoneExporter()
        {
            _sem = new SemaphoreSlim(1);
            Dependency.Container.RegisterType<ISettingsRepository, SettingsPhone>(Dependency.SingletonLifetime, new InjectionMember[0]);
            Dependency.Container.RegisterType<ISettingsProvider, SettingsPhone>(Dependency.SingletonLifetime, new InjectionMember[0]);
            Dependency.Container.RegisterType<IFileSystemHelper, FileSystemHelper>(Dependency.SingletonLifetime, new InjectionMember[0]);
        }

        private AutoResetEvent _AutoResetEvent = new AutoResetEvent(false);

        public void PrivateExport()
        {
            if (AlbumToExport == null)
                return;

            _sem.Wait();

            OnProgress(new ConnectingToWindowsPhone());

            IDeviceManager idm = DeviceManager.CreateInstance(new SettingsPhone());
            idm.Startup(_Ims.MainWindow);
            idm.DeviceEnumerationEnded += new EventHandler<EventArgs>(idm_DeviceEnumerationEnded);

            if (!_AutoResetEvent.WaitOne(TimeSpan.FromSeconds(40)))
            {
                bool Copy = false;

                lock (_CopyingLocker)
                    Copy = _IsCopying;

                if (Copy)
                {
                    //le transfer a commence j'attend la fin
                    _AutoResetEvent.WaitOne();
                }
                else
                {
                    _IsCancelled.Cancel();
                    //j'ai pas de reponse de la part de device manager
                    //j'arrete tout
                    Trace.WriteLine("Time out during export to windows phone");
                    OnError(new UnknowErrorWindowsPhone());
                }
            }

            //idm.Dispose();

            _IIC.FireFactorizedEvents();
            OnProgress(new EndExport(AlbumToExport));

            _sem.Release();

            _AutoResetEvent.Dispose();
        }

        void idm_DeviceEnumerationEnded(object sender, EventArgs e)
        {
            IDeviceManager idm = sender as IDeviceManager;

            idm.DeviceEnumerationEnded -= new EventHandler<EventArgs>(idm_DeviceEnumerationEnded);


            if (idm.Devices.Count == 0)
            {
                OnError(new WindowsPhoneFound());
                _AutoResetEvent.Set();
                return;
            }

            lock (_CopyingLocker)
                _IsCopying = true;

            IDevice id = idm.Devices[0];
            ITargetDevice d = id as ITargetDevice;

            foreach (TrackDescriptor tr in AlbumToExport.Select(al => AlbumDescriptor.CopyAlbum(al as Album, false)).SelectMany(a => a.RawTrackDescriptors))
            {
                if (_IsCancelled.Token.IsCancellationRequested)
                    return;

                OnProgress(new ExportToWindowsPhone(tr));
                SaveToWindowsPhone(tr, d);
            }


            if (!_IsCancelled.Token.IsCancellationRequested)
                _AutoResetEvent.Set();
        }


        public bool SaveToWindowsPhone(TrackDescriptor tr, ITargetDevice targ)
        {
            SendItemProgressDelegate progress = SendItemProgress;
            string Path = tr.Path;

            try
            {

                if (!File.Exists(Path))
                {
                    _IIC.OnFactorisableError<FileBrokenCannotBeExported>(Path);
                    return false;
                }

                WindowsDictionaryDecorator wdd = new WindowsDictionaryDecorator();
                new DataExchanger<TrackDescriptor>(tr).Describe(DataExportImportType.WindowsPhone, wdd);

                Uri file = new Uri(Path, UriKind.Absolute);

                Dictionary OutputDictionary = new Dictionary();
                targ.SendMusicFile(file, file, wdd.Dictionary, progress, OutputDictionary);

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Unexpected error during export to Windows phone {0}", e));
                _IIC.OnFactorisableError<UnableToExportFileToWindowsPhone>(Path);
                return false;
            }
        }

        private bool SendItemProgress(float percentComplete)
        {
            return true;
        }

        public void Export(bool Sync)
        {
            if (Sync)
                PrivateExport();
            else
            {
                Action Ac = () => PrivateExport();
                Ac.BeginInvoke(null, null);
            }
        }

        public IEnumerable<IAlbum> AlbumToExport
        {
            get;
            set;
        }
    }
}
