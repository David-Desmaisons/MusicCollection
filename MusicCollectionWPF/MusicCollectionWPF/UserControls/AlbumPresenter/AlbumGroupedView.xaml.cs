using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for AlbumGroupedView.xaml
    /// </summary>
    public partial class AlbumGroupedView : AlbumPresenterBase, IAlbumPresenter
    {
        public AlbumGroupedView()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }



        #region Dependency Properties


        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
      typeof(double), typeof(AlbumGroupedView), new FrameworkPropertyMetadata(200D,
          FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty SizerProperty = DependencyProperty.Register("Sizer",
            typeof(int), typeof(AlbumGroupedView), new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public int Sizer
        {
            get { return (int)GetValue(SizerProperty); }
            set { SetValue(SizerProperty, value); }
        }

        #endregion

        private IList _Groups;
        public IList Groups
        {
            get { return _Groups; }
            private set
            {
                if (_Groups == value)
                    return;

                _Groups = value;

                if (ScrollViewer!=null)
                    ScrollViewer.ScrollToHorizontalOffset(0);

                PropertyHasChanged("Groups");
            }
        }

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
                
                if (old!=null)
                    old.Dispose();  
            }
        }

     

        private void NavigatorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Item")
                return;

            ObservedCollection = GetGroups();
        }

        private class GenreGrouped : IComposedObservedCollection
        {
            private IDisposable _ToClean = null;
            private IExtendedOrderedObservableCollection<IObservableGrouping<IGenre, IAlbum>> _Coll;

            internal GenreGrouped(IList<IAlbum> albums)
            {
                var Int = albums.LiveToLookUp((al) => al.MainGenre);
                _Coll = Int.LiveOrderBy((g) => g.Key.FullName);
                _ToClean = Int;
            }

            public IList Collection
            {
                get { return _Coll; }
            }

            public void Dispose()
            {
                _Coll.Dispose();
                _ToClean.Dispose();
            }
        }

        private class ArtistGrouped : IComposedObservedCollection
        {
            private IDisposable _ToClean = null;
            private IDisposable _ToClean2 = null;
            private IExtendedOrderedObservableCollection<IObservableGrouping<IArtist, IAlbum>> _Coll;

            internal ArtistGrouped(IList<IAlbum> albums)
            {
                var Int = albums.LiveSelectManyTuple((a) => a.Artists);
                var Int2 = Int.LiveToLookUp((t) => t.Item2, (t) => t.Item1);
                _Coll = Int2.LiveOrderBy((a) => a.Key.Name);
                _ToClean = Int;
                _ToClean2 = Int2;
            }

            public IList Collection
            {
                get { return _Coll; }
            }

            public void Dispose()
            {
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
                partial = partial.LiveWhere(al => al.Artists.Any(a=>a == ArtistNavigation.Item));


            if (ArtistNavigation.IsFiltering)
            {
                return new ComposedObservedCollectionAdapter<IAlbum>(partial.LiveOrderBy((al)=>al.Year));
            }

            if (GenreNavigation.IsFiltering)
            {
                return new ArtistGrouped(partial);
            }

            throw new Exception();

        }


        private GenreNagigator _GN= new GenreNagigator();
        private GenreNagigator  GenreNavigation
        {
            get{return _GN;}
        }

        private ArtistNagigator _AN = new ArtistNagigator();
        private ArtistNagigator ArtistNavigation
        {
            get { return _AN; }
        }

        private IMusicSession _Session;
        public IMusicSession Session
        {
            set
            {
                if (value == null)
                    return;

                _Session = value;

 
                this.GenreControl.Filter = this.GenreNavigation;
                this.ArtistControl.Filter = this.ArtistNavigation;

                this.GenreNavigation.PropertyChanged += NavigatorChanged;
                this.ArtistNavigation.PropertyChanged += NavigatorChanged;

                Sorter = value.AlbumSorter;

                ObservedCollection = GetGroups();

                PropertyHasChanged("Session");

            }
            get
            {
                return _Session;
            }
        }


        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Session = (e.NewValue as IMusicSession);
        }

        public override IEnumerable<IAlbum> SelectedAlbums
        {
            get { return Enumerable.Empty<IAlbum>(); }
            set
            { }
        }


        public override ListBox MyDisc
        {
            get { return ListDisc; }
        }

        private AutoTransitionGrid GetTransitioner(Button button)
        {
            Grid father = button.Parent as Grid;

            if (father == null)
                return null;

            AutoTransitionGrid found = father.FindName("Transition") as AutoTransitionGrid;
            return found;
        }


        private ICollectionView CVSFromObject(FrameworkElement im)
        {
             return CollectionViewSource.GetDefaultView((im.DataContext as ICollectionAcessor<IAlbum>).Collection);
        }

        private void DiscImage_Up(object sender, RoutedEventArgs e)
        {

            Button im = sender as Button;
            ICollectionView cvs = CVSFromObject(im);

            if (cvs == null)
                return;

            AutoTransitionGrid found = GetTransitioner(im);
            if (found == null)
                return;

            using (found.GetTransitionner())
            {

                if (!cvs.MoveCurrentToNext())
                    cvs.MoveCurrentToFirst();
            }

        }

        private void DiscImage_MouseDown(object sender, RoutedEventArgs e)
        {
            Button im = sender as Button;
            ICollectionView cvs = CVSFromObject(im);

            if (cvs == null)
                return;

            AutoTransitionGrid found = GetTransitioner(im);
            if (found == null)
                return;

            using (found.GetTransitionner())
            {

                if (!cvs.MoveCurrentToPrevious())
                    cvs.MoveCurrentToLast();
            }
        }

        private void Genre_Click(object sender, RoutedEventArgs e)
        {
            Button mybutt = sender as Button;
            IObservableGrouping<IGenre, IAlbum> LookUp = mybutt.DataContext as IObservableGrouping<IGenre, IAlbum>;
            if (LookUp == null)
                return;

            IGenre mygenre = LookUp.Key;
            if (mygenre == null)
                return;

            this.GenreNavigation.Item = mygenre;
        }

        private void Artist_Click(object sender, RoutedEventArgs e)
        {
            Button mybutt = sender as Button;
            IObservableGrouping<IArtist, IAlbum> LookUp = mybutt.DataContext as IObservableGrouping<IArtist, IAlbum>;
            if (LookUp == null)
                return;

            IArtist mygenre = LookUp.Key;
            if (mygenre == null)
                return;

            this.ArtistNavigation.Item = mygenre;
       }

        protected override void EditEntity(IEnumerable<IAlbum> objs)
        {
        }

        public override void CancelEdit()
        {
        }

        public override void EndEdit()
        {
        }      

        private void Root_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Focus();
            IInputElement res = Keyboard.Focus(this);
        }

        private void ListDisc_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        //private void SizeContextMenu(Viewbox vb, FrameworkElement fe)
        //{
        //    fe.ContextMenu.VerticalOffset = vb.ActualWidth / 2 - fe.ContextMenu.ActualHeight / 2;
        //}
        //
        //private void DiscImage_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    FrameworkElement fe = sender as FrameworkElement;
        //    Viewbox vb = fe.FindAncestor<Viewbox>();
        //    if (fe.ContextMenu.ActualHeight != 0)
        //    {
        //        SizeContextMenu(vb, fe);
        //    }
        //    else
        //    {
        //        fe.ContextMenu.SizeChanged += (o, eb) => SizeContextMenu(vb, fe);
        //    }
        //}
    }
}
