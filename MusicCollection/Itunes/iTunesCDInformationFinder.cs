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
using System.Threading.Tasks;

namespace MusicCollection.Itunes
{
    public class iTunesCDInformationFinder : IDiscInformationProvider
    {
    
        public  iTunesCDInformationFinder()
        {
        }

    
        public WebMatch<IFullAlbumDescriptor> FoundCDInfo
        {
            get;
            private set;
        }

        public void Compute(IProgress<ImportExportError> iIImportExportProgress)
        {
            ComputeCurrentCDInfo(iIImportExportProgress);
        }

        public System.Threading.Tasks.Task ComputeAsync(IProgress<ImportExportError> iIImportExportProgress)
        {
            return Task.Run(() => ComputeCurrentCDInfo(iIImportExportProgress));
        }

    
        private void ComputeCurrentCDInfo(IProgress<ImportExportError> iIImportExportProgress)
        {
            try
            {
                iTunesApp iTunesApp = new iTunesApp();

                IITSourceCollection _sources = iTunesApp.Sources;

                var lib = iTunesApp.Sources.Cast<IITSource>().Where(s => s.Kind == ITSourceKind.ITSourceKindAudioCD).FirstOrDefault();

                if (lib == null)
                {
                    iIImportExportProgress.SafeReport(new ItunesCDNotFoundError());
                    return;
                }

                IITAudioCDPlaylist cd = lib.Playlists.get_ItemByName(lib.Name) as IITAudioCDPlaylist;

                if (cd == null)
                {
                    iIImportExportProgress.SafeReport(new ItunesUnknownError());
                    return;
                }

                IFullAlbumDescriptor ad = AlbumDescriptor.FromiTunes(cd);

                FoundCDInfo = new WebMatch<IFullAlbumDescriptor>(ad, MatchPrecision.Suspition, WebProvider.iTunes);
            }
            catch (Exception e)
            {
                Trace.WriteLine("iTunes CD information introspection failed "+e.ToString());
                iIImportExportProgress.SafeReport(new ITunesNotResponding());
            }
        }
    }
}
