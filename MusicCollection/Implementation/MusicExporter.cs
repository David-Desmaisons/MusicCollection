using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

using SevenZip;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;


namespace MusicCollection.Implementation
{
    internal class MusicExporter : UIThreadSafeImportEventAdapter, IMusicCompleteFileExporter, IEventListener
    {
        //private bool _ExportImages;
        //private List<IAlbum> _AlbumToExport = new List<IAlbum>();
        private IImportContext _IIC;

        private const string _RX = "Albums.XML";
        static internal string XMLName
        {
            get { return _RX; }
        }

        private const string _ext = ".7z";
        static internal string RealExtName
        {
            get { return _ext; }
        }

        #region event

        //private UISafeEvent<ImportExportErrorEventArgs> _Error;

        //public event EventHandler<ImportExportErrorEventArgs> Error
        //{
        //    add { _Error.Event += value; }
        //    remove { _Error.Event -= value; }
        //}

        //protected void OnError(ImportExportErrorEventArgs Error)
        //{
        //    _Error.Fire(Error, true);
        //}

        #endregion

        internal MusicExporter(IInternalMusicSession MSI, MusicImportExportType mit)
        {
            _IIC = MSI.GetNewSessionContext();
            _IIC.Error += ((o, e) => OnError(e));
            CompactFiles = mit;
        }

        public MusicImportExportType CompactFiles { get; private set; }

        public void Export(bool Sync)
        {
            if (Sync)
                PrivateExportToDirectory();
            else
            {
                Action Ac = PrivateExportToDirectory;
                Ac.BeginInvoke(null, null);
            }
        }

        public string FileDirectory { set; get; }

        public IEnumerable<IAlbum> AlbumToExport
        {
            set;
            get;
        }

       

        private class FileCompactor : IAlbumVisitor
        {
            private MusicExporter _Father;
            private string _FinalDirectory;
            private string _FD;
            private Album _Album;
            private AlbumDescriptor _EA;
            private SevenZipCompressor _Compressor;
            private Dictionary<string, string> _Dic;
            private IEventListener _IEL;

            public Album Album
            {
                private get { return _Album; }
                set { _Album = value; UpdateAlbum(); }
            }


            internal bool CustoMode
            {
                get;
                private set;
            }

            private void UpdateAlbum()
            {
                _FinalDirectory = Path.Combine(Album.Interface.Author.FormatFileName(), Album.Interface.Name.FormatFileName());

                AddExportAlbum(AlbumDescriptor.DuplicateFromAlbum(Album));
            }


            private List<AlbumDescriptor> _EAs;
            private void AddExportAlbum(AlbumDescriptor EA)
            {
                if (_EAs == null)
                    _EAs = new List<AlbumDescriptor>();

                _EAs.Add(EA);
                _EA = EA;
            }

            internal FileCompactor(MusicExporter Father, string FD, bool iCustoMode, IEventListener iel)
            {
                _Father = Father;
                _FD = FD;
                _Compressor = new SevenZipCompressor();
                _Compressor.PreserveDirectoryRoot = true;
                _Compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
                _Compressor.CompressionMethod= CompressionMethod.BZip2;
                CustoMode = iCustoMode;
                _IEL = iel;

                if (CustoMode)
                    _Compressor.CustomParameters.Add("s", "off");
            }

            private string AddFileToConvert(string Or, string New)
            {
                New = Path.Combine(_FinalDirectory, New); 

                if (!File.Exists(Or))
                    return New;

                if (_Dic == null)
                    _Dic = new Dictionary<string, string>();
                else if (_Dic.ContainsKey(New))
                {
                    string basen = Path.GetFileNameWithoutExtension(New);
                    string pathn = Path.GetDirectoryName(New);
                    string extn = Path.GetExtension(New);
                    int i = 1;

                    do
                    {
                        New = Path.Combine(pathn, string.Format("{0}({1}){2}", basen, i++, extn));
                    }
                    while (_Dic.ContainsKey(New));
                }

                _Dic.Add(New, Or);
                return New;
            }

            public void VisitImage(AlbumImage ai)
            {
                string dir = Path.Combine(_Father._IIC.Folders.Temp, _FinalDirectory);
                Directory.CreateDirectory(dir);
                string res = ai.ExportTo(dir);
                if (res == null)
                    return;

                string path = AddFileToConvert(res, Path.GetFileName(res));
                _EA.AddImage(ai.Rank, @".\" + path);
            }

            public void VisitTrack(Track tr)
            {
                string path = AddFileToConvert(tr.Path, tr.NormalizedName);
                _EA.AddExportedTrack(tr, @".\" + path);

                if (tr.IsBroken)
                    _IEL.OnFactorisableError<FileBrokenCannotBeExported>(tr.Path);

            }

            public void EndAlbum()
            {
                //throw new NotImplementedException();
            }



            public bool End()
            {
                if ((_Dic == null) || (_Dic.Count == 0))
                    return true;

                AlbumDescriptorExchanger eas = new AlbumDescriptorExchanger() { Albums = _EAs };
                
                if (eas.Export(_Father._IIC.Folders.Temp, @".\") == false)
                    return false;

                _Dic.Add(XMLName, eas.CurrentPath);

                string name = CustoMode ? "Albums.mcc" : "Albums.7z";
                string path = FileInternalToolBox.CreateNewAvailableName(Path.Combine(_FD, name));

                try
                {
                    using (Stream fs = File.Create(path))
                    {
                        _Compressor.CompressFileDictionary(_Dic, fs);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem creating compressed file"+e.ToString());
                    return false;
                }

                return true;
            }
        }

        private class SimpleExporter : IAlbumVisitor
        {
            private MusicExporter _Father;
            private string _FD;
            private string _FinalDirectory;
            private bool _del1 = false;
            private bool _del2 = false;
            private DirectoryInfo _di;
            private DirectoryInfo _di2;
            private bool _Exported = false;
            private AlbumDescriptor _EA;
            private IEventListener _IEL;

            private Album _Album;
            public Album Album
            {
                private get { return _Album; }
                set { _Album = value; UpdateAlbum(); }
            }

            internal SimpleExporter(MusicExporter Father, string FD, IEventListener iel)
            {
                _Father = Father;
                _FD = FD;
                _IEL = iel;
            }

            private void UpdateAlbum()
            {
                string OutDirector = Path.Combine(_FD, Album.Author.FormatForDirectoryName());
                _Exported = false;

                _di = new DirectoryInfo(OutDirector);
                if (!_di.Exists)
                {
                    _di.Create();
                    _del1 = true;
                }

                _FinalDirectory = Path.Combine(OutDirector, Album.Interface.Name.FormatForDirectoryName());

                _di2 = new DirectoryInfo(_FinalDirectory);
                if (!_di2.Exists)
                {
                    _di2.Create();
                    _del2 = true;
                }

                AddExportAlbum(AlbumDescriptor.DuplicateFromAlbum(Album));
            }

            public void EndAlbum()
            {
                if (_Exported)
                    return;

                if (_del1)
                    _di.Delete(true);
                else if (_del2)
                    _di2.Delete(true);

            }


            private List<AlbumDescriptor> _EAs;

            private void AddExportAlbum(AlbumDescriptor EA)
            {
                if (_EAs == null)
                    _EAs = new List<AlbumDescriptor>();

                _EAs.Add(EA);
                _EA = EA;
            }

 

            public bool End()
            {
                if (_EAs == null)
                    return true;

                AlbumDescriptorExchanger eas = new AlbumDescriptorExchanger() { Albums = _EAs };
                return eas.Export(_FD);
            }


            void IAlbumVisitor.VisitImage(AlbumImage ai)
            {
                if (_Exported)
                {
                    string res = ai.ExportTo(_FinalDirectory);
                    if (res != null)
                        _EA.AddImage(ai.Rank, res);
                }
            }

            public void VisitTrack(Track tr)
            {
                if (tr.IsBroken)
                {
                    _Father._IIC.OnFactorisableError<FileBrokenCannotBeExported>(tr.ToString());
                    return;
                }

                string NPath = System.IO.Path.Combine(_FinalDirectory, tr.NormalizedName);

                if (NPath == tr.Path)
                    return;

                try
                {
                    System.IO.File.Copy(tr.Path, NPath);

                    if (_EA != null)
                        _EA.AddExportedTrack(tr, NPath);

                    _Exported = true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem moving file"+e.ToString());
                    _Father._IIC.OnFactorisableError<UnableToCopyFile>(tr.ToString());
                }
            }
        }

        private void PrivateExportToDirectory()
        {
            if (!Directory.Exists(FileDirectory))
            {
                OnError(new ExportDirectoryNotFound(FileDirectory));
                return;
            }


            using (_IIC.SessionLock())
            {
                SizeChecker sc = new SizeChecker(FileDirectory);

                foreach (IInternalAlbum Al in AlbumToExport)
                {
                    Al.Visit(sc);
                }

                if (!sc.End())
                {
                    OnError(new NotEnougthSpace(sc.Checker.ToString()));
                    return;
                }

                IAlbumVisitor exp = null;

                if (CompactFiles == MusicImportExportType.Directory)
                    exp = new SimpleExporter(this, FileDirectory, this);
                else
                    exp = new FileCompactor(this, FileDirectory, (CompactFiles == MusicImportExportType.Custo), this);

                foreach (IInternalAlbum Al in AlbumToExport)
                {
                    Al.Visit(exp);
                }

                if (!exp.End())
                {
                    OnError(new UnableToCreateFile(string.Join(Environment.NewLine, AlbumToExport)));
                    return;
                }

            }

            _IIC.FireFactorizedEvents();
            OnProgress(new EndExport(AlbumToExport));

        }

        void IEventListener.OnFactorisableError<T>(IEnumerable<string> message)
        {
            _IIC.OnFactorisableError<T>(message);
        }

        void IEventListener.OnFactorisableError<T>(string message)
        {
            _IIC.OnFactorisableError<T>(message);
        }


        void IEventListener.OnError(ImportExportErrorEventArgs Error)
        {
            OnError(Error);
        }

        void IEventListener.OnProgress(ProgessEventArgs Where)
        {
            OnProgress(Where);
        }

    }


}
