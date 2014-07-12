using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModelHelper;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class MusicEntitiesEditorViewModel : ViewModelBase, IInformationEditor
    {
        private IMultiEntityEditor _MYMod;
        private IMusicSession _IMS;
        private bool _Continue = false;
     
        private MusicEntitiesEditorViewModel(IMusicSession session)
        {
            _IMS = session;
            ArtistSearchableFactory = new ArtistSearchableFactory(session);
            
            Genres = Register( _IMS.AllGenres.LiveOrderBy(global => global.FullName));
            Commit = RelayCommand.Instanciate(DoCommit);
            GenreFactory = FactoryBuilder.Instanciate((n) => session.GetGenreFactory().Create(n));
            YearFactory = FactoryBuilder.Instanciate((n) => { int res = 0; int.TryParse(n, out res); return res; });
        }

        public ArtistSearchableFactory ArtistSearchableFactory { get; private set; }
        public IFactory YearFactory {get;private set;}

        public IFactory GenreFactory { get; private set; }

        public MusicEntitiesEditorViewModel(IMusicSession iims, IEnumerable<ITrack> tracks):this(tracks.First().Album.Session)
        {
            _MYMod = iims.GetTrackEditor(tracks);
        }

        public MusicEntitiesEditorViewModel(IMusicSession iims, IEnumerable<IAlbum> albums):this(albums.First().Session)
        {
            _MYMod = iims.GetAlbumEditor(albums);
        }
       
        public IAsyncCommiter GetCommiter()
        {
            if (_Continue)
                return this._MYMod;

            _MYMod.Cancel();
            return null;
        }

        public OptionChooser<string> NameOption { get { return _MYMod.NameOption;} }

        public OptionChooser<string> GenreOption { get { return _MYMod.GenreOption; } }

        public OptionChooser<int?> YearOption { get { return _MYMod.YearOption; } }

        public OptionArtistChooser ArtistOption { get { return _MYMod.ArtistOption; } }

        private void DoCommit()
        {
            _Continue = true;
            if (Genre != null) GenreOption.Choosed = Genre.FullName;
            Window.Close(); 
        }

        public IList<IGenre> Genres { get; private set; }

        private IGenre _Genre=null;
        public IGenre Genre
        {
            get { return _Genre; }
            set { this.Set(ref _Genre, value); }
        }

        public ICommand Commit { get; private set; }
        
    }
   
}
