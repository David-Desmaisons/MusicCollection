using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel.Element;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra.Collection;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class CenteredAlbumViewModel : ViewModelBase
    {

        private IList<IAlbum> Albums { get; set; }

        private AfinityCollection<IAlbum> _OrderByAfinity;

        public IAlbum _CenterAlbum;
        public IAlbum CenterAlbum
        {
            get { return _CenterAlbum; }
            set
            {  
                if (object.ReferenceEquals(_OrderByAfinity.Reference, value))
                        return;
                
                Set(ref _CenterAlbum, value);
                SetCenteredAlbumAsync(value);
            }
        }

        private Task SetCenteredAlbumAsync(IAlbum ialbum)
        {
            return _OrderByAfinity.ComputeAsync(ialbum);
        }

        private IExtendedObservableCollection<IAlbum> _AffinityOrderedCollection;
        public IExtendedObservableCollection<IAlbum> AffinityOrderedCollection
        {
            get { return _AffinityOrderedCollection; }
            set { Set(ref _AffinityOrderedCollection, value); }
        }


        public CenteredAlbumViewModel(IList<IAlbum> iAlbums, IMusicSession isession)
        {
            Albums = iAlbums;

            AlbumDistanceComparerFactory adcf = new AlbumDistanceComparerFactory(isession);
            _OrderByAfinity = new AfinityCollection<IAlbum>(this.Albums, al => adcf.GetComparer(al), 50);
            AffinityOrderedCollection = _OrderByAfinity.Collection;

            CenterAlbum = Albums.MaxBy(isession.AlbumSorter.Sorter);

            ChangeArtist = RelayCommand.Instanciate(DoChangeArtist);
            ChangeGenre = RelayCommand.Instanciate(DoChangeGenre);
            Random = RelayCommand.Instanciate(DoRandom);
            ChangeAlbum = RelayCommand.Instanciate(DoChangeAlbum);
            Center = RelayCommand.Instanciate<IAlbum>(al => CenterAlbum = al);
        }


        public ICommand ChangeArtist { get; private set; }
        public ICommand ChangeGenre { get; private set; }
        public ICommand Random { get; private set; }
        public ICommand ChangeAlbum { get; private set; }

        public ICommand Center { get; private set; }

        private void DoChangeArtist()
        {
            if (CenterAlbum == null)
                return;

            IAlbum ia = Albums.Where(a => a.Genre == CenterAlbum.Genre).Where(a => !CenterAlbum.Artists.Any(b => a.Artists.Contains(b))).RandomizedItem();
            if (ia != null)
                CenterAlbum = ia;

        }

        private void DoChangeGenre()
        {

            if (CenterAlbum == null)
                return;

            IAlbum ia = Albums.Where(a => a.Genre != CenterAlbum.Genre).RandomizedItem();
            if (ia != null)
                CenterAlbum = ia;
        }

        private void DoChangeAlbum()
        {

            if (CenterAlbum == null)
                return;

            IAlbum ia = Albums.Where(a => a != CenterAlbum).Where(a => CenterAlbum.Artists.Any(b => a.Artists.Contains(b))).RandomizedItem();
            if (ia != null)
                CenterAlbum = ia;
            else
                DoChangeArtist();
        }

        private void DoRandom()
        {
            if (CenterAlbum == null)
                return;

            IAlbum ia = Albums.RandomizedItem();
            if (ia != null)
                CenterAlbum = ia;
        }
    }

}
