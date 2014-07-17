using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IMusicSession :  IDisposable
    {
        #region Properties 

        ObservableCollection<IGenre> AllGenres { get; }

        IFullObservableCollection<IAlbum> AllAlbums { get; }
   
        IObservableCollection<IArtist> AllArtists { get; }

        IFullObservableCollection<ITrack> AllTracks { get; }

        ISessionEntityFinder EntityFinder { get; }

        IAlbumSorter AlbumSorter { get; }

        IPlayListFactory PlayListFactory { get; }

        IMusicPlayer MusicPlayer { get; }

        IMusicSettings Setting { get; }

        IMergeStrategyFactory Strategy { get; }

        bool IsUnderTransaction { get; }
  
        IMusicSplashScreenHelper SplashScreen { get; }

        IWebQueryFactory WebQueryFactory { get; }

        IntPtr MainWindow {get;}

        IInfraDependencies Dependencies{get;}

        #endregion 

        IArtist CreateArtist(string ArtistName);

        IList<IArtist> GetArtistFromName(string name);

        IMusicImporterBuilder GetImporterBuilder(MusicImportType type);

        IMusicImporter GetDBImporter();

        IMusicExporterFactory GetExporterFactory();

        IMusicGenreFactory GetGenreFactory();

        IInternetFinder GetInternetFinder(IWebQuery iWebQuer);

        IMusicRemover GetMusicRemover();

        IMultiEntityEditor GetTrackEditor(IEnumerable<ITrack> tracks);

        IMultiEntityEditor GetAlbumEditor(IEnumerable<IAlbum> albums);

        IDiscInformationProvider GetITunesCDIdentificator();

        IEmailFactory GetEmailFactory();
        
    }
}