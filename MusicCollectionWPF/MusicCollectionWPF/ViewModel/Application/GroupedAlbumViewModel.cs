using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class GroupedAlbumViewModel : ViewModelBase
    {
        public GroupedAlbumViewModel(IList<IAlbum> iAlbums)
        {
            Albums = iAlbums;
            Update();

            this.GenreNavigation.PropertyChanged += NavigatorChanged;
            this.ArtistNavigation.PropertyChanged += NavigatorChanged;

            CenterArtist = RelayCommand.Instanciate<ComposedObservedCollection<IArtist, IAlbum>>(Do_Center_Artist);
            CenterGenre = RelayCommand.Instanciate<ComposedObservedCollection<IGenre, IAlbum>>(Do_Center_Genre);
        }

        private bool _IsNavigating = false;
        public bool IsNavigating
        {
            get { return _IsNavigating; }
            set { Set(ref _IsNavigating, value); }
        }

        private void Do_Center_Genre(ComposedObservedCollection<IGenre, IAlbum> LookUp)
        {
            if (LookUp == null)
                return;

            IGenre mygenre = LookUp.Key;
            if (mygenre == null)
                return;

            this.GenreNavigation.Item = mygenre;
        }

        private void Do_Center_Artist(ComposedObservedCollection<IArtist, IAlbum> LookUp)
        {
            if (LookUp == null)
                return;

            DoGoToArtist(LookUp.Key);
        }

        public void GoToArtist(IArtist iartist)
        {
            DoGoToArtist(iartist);
        }

        private void DoGoToArtist(IArtist iartist)
        {
            if (iartist == null)
                return;

            this.ArtistNavigation.Item = iartist;
        }

        private void NavigatorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Item")
                return;

            IsNavigating = true;

            Update();
        }

        private void Update()
        {
            ObservedCollection = GetGroups();
        }

        private IList<IAlbum> Albums { get;  set; }

        private IList _Groups;
        public IList Groups
        {
            get { return _Groups; }
            private set { IsNavigating = true; Set(ref _Groups, value); IsNavigating = false; }
        }

        public ICommand CenterArtist { get; private set; }

        public ICommand CenterGenre { get; private set; }

        private IComposedObservedCollection _ObservedCollection;
        private IComposedObservedCollection ObservedCollection
        {
            get { return _ObservedCollection; }
            set
            {
                if (_ObservedCollection == value)
                    return;

                var old = _ObservedCollection;

                _ObservedCollection = value;

                Groups = _ObservedCollection.Collection;

                if (old != null)
                    old.Dispose();
            }
        }

    

        private class GenreGrouped : IComposedObservedCollection
        {
            private IDisposable _ToClean = null;
             private IExtendedObservableCollection<ComposedObservedCollection<IGenre, IAlbum>> _Coll;


            internal GenreGrouped(IList<IAlbum> albums)
            {
                var Int = albums.LiveToLookUp((al) => al.MainGenre);
                _Coll = Int.LiveOrderBy((g) => g.Key.FullName).LiveSelect(el => new ComposedObservedCollection<IGenre, IAlbum>(el.Key, el.Collection));
                _ToClean = Int;
            }

            public IList Collection { get { return _Coll; } }

            public void Dispose()
            {
                _Coll.Apply(el => el.Dispose());
                _Coll.Dispose();
                _ToClean.Dispose();
            }
        }

        private class ArtistGrouped : IComposedObservedCollection
        {
            private IDisposable _ToClean = null;
            private IDisposable _ToClean2 = null;
            private IExtendedObservableCollection<ComposedObservedCollection<IArtist, IAlbum>> _Coll;

            internal ArtistGrouped(IList<IAlbum> albums)
            {
                var Int = albums.LiveSelectManyTuple((a) => a.Artists);
                var Int2 = Int.LiveToLookUp((t) => t.Item2, (t) => t.Item1);
                _Coll = Int2.LiveOrderBy((a) => a.Key.Name).LiveSelect(el => new ComposedObservedCollection<IArtist, IAlbum>(el.Key, el.Collection));
                _ToClean = Int;
                _ToClean2 = Int2;
            }

            public IList Collection { get { return _Coll; } }

            public void Dispose()
            {
                _Coll.Apply(el => el.Dispose());
                _Coll.Dispose();
                _ToClean.Dispose();
                _ToClean2.Dispose();
            }
        }


        private IComposedObservedCollection GetGroups()
        {

            if ((!this.GenreNavigation.IsFiltering) && (!this.ArtistNavigation.IsFiltering))
            {
                return new GenreGrouped(Albums);
            }

            var partial = Albums;
            if (GenreNavigation.IsFiltering)
                partial = partial.LiveWhere(al => al.MainGenre == GenreNavigation.Item);

            if (ArtistNavigation.IsFiltering)
                partial = partial.LiveWhere(al => al.Artists.Any(a => a == ArtistNavigation.Item));


            if (ArtistNavigation.IsFiltering)
            {
                return new ComposedObservedCollectionAdapter<IAlbum>(partial.LiveOrderBy((al) => al.Year));
            }

            if (GenreNavigation.IsFiltering)
            {
                return new ArtistGrouped(partial);
            }

            throw new Exception();

        }


        private GenreNagigator _GN = new GenreNagigator();
        public GenreNagigator GenreNavigation
        {
            get { return _GN; }
        }

        private ArtistNagigator _AN = new ArtistNagigator();
        public ArtistNagigator ArtistNavigation
        {
            get { return _AN; }
        }


    }
}
