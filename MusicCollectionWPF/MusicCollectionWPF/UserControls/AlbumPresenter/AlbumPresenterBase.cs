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
using System.Windows.Controls.Primitives;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    public class AlbumPresenterBase : UserControl, INotifyPropertyChanged, IDisposable
    {
        public static readonly DependencyProperty AlbumsProperty = DependencyProperty.Register("Albums",
          typeof(IList<IAlbum>), typeof(AlbumPresenterBase), new PropertyMetadata(null, AlbumsPropertyChangedCallback));

        public IList<IAlbum> Albums
        {
            get { return (IList<IAlbum>)GetValue(AlbumsProperty); }
            set { SetValue(AlbumsProperty, value); }
        }

        static private void AlbumsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            AlbumPresenterBase apuc = d as AlbumPresenterBase;

            if (apuc == null)
                throw new Exception();
            apuc.CollectionAlbums = e.NewValue as IList<IAlbum>;
        }

        protected virtual IList<IAlbum> CollectionAlbums
        {
            set
            {
                _Albums = new CollectionViewSource();
                _Albums.Source = value;
                CheckSorter();
                //CheckFilter();
                PropertyHasChanged("ViewedAlbums");
            }
        }

        internal IEnumerable<IAlbum> VisibleAlbums
        {
            get { return LCW.Cast<IAlbum>(); }
        }

        private CollectionViewSource _Albums;
        public ICollectionView ViewedAlbums
        {
            get
            {
                return _Albums == null ? null : _Albums.View;
            }
        }

        protected CollectionViewSource CVS
        {
            get { return _Albums; }
        }

        public virtual IEnumerable<IAlbum> SelectedAlbums
        {
            get { return MyDisc.SelectedItems.Cast<IAlbum>(); }
            set
            {
                using (BlockReentrance())
                {
                    //using (IDisposable dfr = CVS.DeferRefresh())
                    //{
                        MyDisc.SelectedItems.Clear(); value.Apply(a => MyDisc.SelectedItems.Add(a));
                    //}
                }
            }
        }

        private bool _BlockedReentrance = false;

        protected bool BlockedReentrance
        {
            get { return _BlockedReentrance; }
        }

        private class Locker : IDisposable
        {
            private AlbumPresenterBase _Father;

            public Locker(AlbumPresenterBase F)
            {
                _Father = F;
                _Father._BlockedReentrance = true;
            }


            public void Dispose()
            {
                _Father._BlockedReentrance = false;
            }
        }

        protected IDisposable BlockReentrance()
        {
            return new Locker(this);
        }

        public virtual ListBox MyDisc
        {
            get { return null; }
        }

        protected ListCollectionView LCW
        {
            get { if (_Albums == null) return null; return _Albums.View as ListCollectionView; }
        }


        protected ScrollViewer ScrollViewer
        {
            get;
            set;
        }


        protected void OnKeyBoardEvent(object sender, KeyEventArgs e)
        {
            ScrollViewer isc = ScrollViewer;
            if (isc == null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                    isc.LineUp();
                    e.Handled = true;
                    break;

                case Key.Down:
                    isc.LineDown();
                    e.Handled = true;
                    break;

                case Key.Left:
                     isc.LineLeft();

                    break;

                case Key.Right:
                    isc.LineRight();
                    break;

                case Key.PageUp:
                     isc.PageUp();
                     e.Handled = true;
                    break;

                case Key.PageDown:
                    isc.PageDown();
                    e.Handled = true;
                    break;
            }
        }

        protected void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer = sender as ScrollViewer;
        }

        //private IAlbumCombinedFilter _AC;
        //public IAlbumCombinedFilter Filter
        //{
        //    get { return _AC; }
        //    set
        //    {
        //        if (_AC == value)
        //            return;

        //        if (_AC != null)
        //        {
        //            _AC.OnFilterChange -= OnFilterChange;
        //            //_AC.OnFilteredElementChange -= OnElementFilteredChanged;
        //        }

        //        _AC = value;
        //        if (_AC == null)
        //            return;

        //        CheckFilter();
        //        _AC.OnFilterChange += OnFilterChange;
        //        //_AC.OnFilteredElementChange += OnElementFilteredChanged;
        //    }
        //}

        private IAlbumBasicSorter _AS;
        public virtual IAlbumBasicSorter Sorter
        {
            get { return _AS; }
            set
            {
                if (_AS == value)
                    return;

                if (_AS != null)
                    _AS.OnChanged -= OnSorterChange;

                _AS = value;
                if (_AS == null)
                    return;

                CheckSorter();
                _AS.OnChanged += OnSorterChange;
            }
        }



        //protected virtual void BeforeChangeSorter()
        //{
        //}

        //protected virtual void BeforeChangeFilter()
        //{
        //}

        private void CheckSorter()
        {
            if (_AS == null)
                return;

            if (_UnderEdit)
            {
                _NeedRefreshSorter = true;
                return;
            }


            ListCollectionView lcw = LCW;
            if (lcw != null)
            {
                using (lcw.DeferRefresh())
                {
                    //BeforeChangeSorter();
                    lcw.CustomSort = _AS.Sorter;
                    //lcw.Refresh();
                }
            }

            _NeedRefreshSorter = false;

        }


        //private void CheckFilter()
        //{
        //    //if (_AC == null)
        //    //    return;

        //    if (_UnderEdit)
        //    {
        //        _NeedRefreshFilter = true;
        //        return;
        //    }

        //    //ListCollectionView lcw = LCW;
        //    //if (lcw != null)
        //    //{
        //    //    BeforeChangeFilter();
        //    //    //lcw.Filter = _AC.AlbumFilter; OldFilter
        //    //}

        //    //_NeedRefreshFilter = false;
        //}


        private void OnSorterChange(object sender, EventArgs fce)
        {
            CheckSorter();
        }

        //private void OnFilterChange(object sender, FilterChangeArgs fce)
        //{
        //    if (fce.Init)
        //        return;

        //    CheckFilter();
        //}

        public virtual void Dispose()
        {
            Sorter = null;
            //Filter = null; OldFilter
        }

        #region album Edition

        private bool _UnderEdit = false;
        //private bool _NeedRefreshFilter = false;
        private bool _NeedRefreshSorter = false;

        public void EditEntity(IEnumerable<IMusicObject> objs)
        {
            EditEntity(objs.ConvertToAlbums());
        }

        protected virtual void EditEntity(IEnumerable<IAlbum> objs)
        {

            _UnderEdit = true;

            //IEditableCollectionView iec = LCW as IEditableCollectionView;

            //objs.Apply(al => iec.EditItem(al));
        }

        public void Remove(IEnumerable<IMusicObject> al)
        {
            //IEnumerable<IAlbum> objs = al.ConvertToAlbums();
            //IEditableCollectionView iec = LCW as IEditableCollectionView;

            //objs.Apply(all => iec.Remove(all));
        }

        private void FinalizeEdit()
        {
            //(LCW as IEditableCollectionView).CommitEdit();
            _UnderEdit = false;
            //if (_NeedRefreshFilter)
            //{
            //    CheckFilter();
            //}

            if (_NeedRefreshSorter)
            {
                CheckSorter();
            }
        }

        public virtual void CancelEdit()
        {
            FinalizeEdit();
        }

        public virtual void EndEdit()
        {
            FinalizeEdit();
        }

        public IEnumerable<IObjectAttribute> GetSelectedEntities(IObjectAttribute Context)
        {
            IAlbum al = Context as IAlbum;
            if (al == null)
                return null;

            if (!SelectedAlbums.Contains(al))
                return al.SingleItemCollection().ToList();

            return SelectedAlbums.ToList();
        }

        public bool IsCommandAllowed(ICommand command)
        {
            return true;
        }
      

        #endregion


        #region Event

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyHasChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
