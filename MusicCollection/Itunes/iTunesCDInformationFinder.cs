using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using iTunesLib;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.ToolBox.Event;
using MusicCollection.WebServices;

namespace MusicCollection.Itunes
{
    public class iTunesCDInformationFinder : IDiscInformationProvider
    {
        private UISafeEvent<EventArgs> _OnCompleted;
        private UISafeEvent<ImportExportErrorEventArgs> _OnError;

        public  iTunesCDInformationFinder()
        {
            _OnCompleted = new UISafeEvent<EventArgs>(this);
            _OnError = new UISafeEvent<ImportExportErrorEventArgs>(this);
        }

        public event EventHandler<EventArgs> OnCompleted
        { add { _OnCompleted.Event += value; } remove { _OnCompleted.Event -= value; } }

        public event EventHandler<ImportExportErrorEventArgs> OnError
        { add { _OnError.Event += value; } remove { _OnError.Event -= value; } }

        public WebMatch<IFullAlbumDescriptor> FoundCDInfo
        {
            get;
            private set;
        }

        public void Compute(bool Sync)
        {
            if (Sync)
            {
                ComputeCurrentCDInfo();
                OnEnd(null);
            }
            else
            {
                Action ac = ComputeCurrentCDInfo;
                AsyncCallback asc = OnEnd;
                ac.BeginInvoke(asc, null);
            }
        }

        private void ComputeCurrentCDInfo()
        {
            try
            {
                iTunesApp iTunesApp = new iTunesApp();

                IITSourceCollection _sources = iTunesApp.Sources;

                var lib = iTunesApp.Sources.Cast<IITSource>().Where(s => s.Kind == ITSourceKind.ITSourceKindAudioCD).FirstOrDefault();

                if (lib == null)
                {
                    Error(new ItunesCDNotFoundError());
                    return;
                }

                IITAudioCDPlaylist cd = lib.Playlists.get_ItemByName(lib.Name) as IITAudioCDPlaylist;

                if (cd == null)
                {
                    Error(new ItunesUnknownError());
                    return;
                }

                IFullAlbumDescriptor ad = AlbumDescriptor.FromiTunes(cd);


                FoundCDInfo = new WebMatch<IFullAlbumDescriptor>(ad, MatchPrecision.Suspition, WebProvider.iTunes);
            }
            catch (Exception e)
            {
                Trace.WriteLine("iTunes CD information introspection failed "+e.ToString());
                Error(new ITunesNotResponding());
            }
        }

        private void OnEnd(IAsyncResult iac)
        {
            _OnCompleted.Fire(EventArgs.Empty,true);
        }

        private void Error(ImportExportErrorEventArgs err )
        {
           _OnError.Fire(err,true);
        }
    }
}
