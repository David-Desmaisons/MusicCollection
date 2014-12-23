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
using System.Windows.Threading;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Infra;
using System.Threading.Tasks;
using MusicCollection.Infra.Collection;
using MusicCollectionWPF.ViewModel.Element;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for AlbumLikehoodPresenter.xaml
    /// </summary>
    /// 

    //#region disc comparer


    ////internal class ComparerTo : IAlbumBasicSorter, ICompleteComparer<IAlbum>
    ////{

    ////    private IMusicSession _Session;
    ////    private IDictionary<IAlbum, int> _Dic;

    ////    internal ComparerTo(IMusicSession ims, IAlbum init)
    ////    {
    ////        _Session = ims;
    ////        _Dic = new Dictionary<IAlbum, int>(ims.AllAlbums.Count);
    ////        Album = init;
    ////    }

    ////    private void Bufferize()
    ////    {
    ////        _Artist = _All.Artists.ToHashSet();
    ////        _ArtitsCount = _Artist.Count;
    ////        _Genre = _All.MainGenre;

    ////        //bufferize results
    ////        _Session.AllGenres.Apply(g => g.Compare(_Genre));
    ////        _RatioArtits = (int)Math.Floor((double)35000 / _ArtitsCount);

    ////        var als = _Session.AllAlbums.ToList();

    ////        Parallel.ForEach(als,
    ////            () => new Dictionary<IAlbum, int>(),
    ////            (el, lc, localdic) => { localdic.Add(el, RawCompare(el)); return localdic; },
    ////            (ldic) => { lock (_Dic) _Dic.Import(ldic); });
    ////    }

    ////    private HashSet<IArtist> _Artist;
    ////    private int _ArtitsCount = 0;
    ////    private IGenre _Genre;
    ////    private int _RatioArtits = 0;

    ////    private int RawCompare(IAlbum T1)
    ////    {
    ////        int res = 0;

    ////        if (Object.ReferenceEquals(T1, _All))
    ////            return res;

    ////        res = 100000;

    ////        if ((T1 == null) || (_All == null))
    ////            return res;

    ////        IGenre g = T1.MainGenre;

    ////        if ((g != null) && (_Genre != null))
    ////        {
    ////            int vg = g.Compare(_Genre);
    ////            res -= 1900 - Math.Min(vg, 19) * 100;
    ////        }

    ////        if ((T1.Year != 0) && (_All.Year != 0))
    ////        {
    ////            int Dist = Math.Min(2000, (Math.Abs(T1.Year - _All.Year)));

    ////            res -= 10000 - 5 * Dist;
    ////        }

    ////        if (_ArtitsCount == 0)
    ////            return res;

    ////        IList<IArtist> arts = T1.Artists;
    ////        int c = arts.Count;
    ////        if (c == 0)
    ////        {
    ////            return res;
    ////        }

    ////        arts.Apply((a) => { if (_Artist.Contains(a)) res -= (int)Math.Floor((double)35000 / c); });

    ////        _Artist.Apply((a) => { if (arts.Contains(a)) res -= this._RatioArtits; });

    ////        return res;
    ////    }

    ////    private IAlbum _All;
    ////    internal IAlbum Album
    ////    {
    ////        get { return _All; }
    ////        private set
    ////        {
    ////            if (_All != null)
    ////                throw new Exception("likehodd error");

    ////            if (value == null)
    ////                return;

    ////            _All = value;
    ////            Bufferize();
    ////        }
    ////    }

    ////    internal bool ResetAlbum(IAlbum al)
    ////    {
    ////        _Dic.Remove(al);
    ////        _Dic.Add(al, RawCompare(al));

    ////        return al == Album;
    ////    }

    ////    private static ComparerTo PrivateCreateComparer(IMusicSession ims, IAlbum init)
    ////    {
    ////        return new ComparerTo(ims, init);
    ////    }

    ////    internal static Task<ComparerTo> CreateComparerAnsyc(IMusicSession ims, IAlbum init)
    ////    {
    ////        return Task.Run(() => PrivateCreateComparer(ims, init));
    ////    }

    ////    public int Compare(IAlbum xx, IAlbum yy)
    ////    {

    ////        if (object.ReferenceEquals(xx, yy))
    ////            return 0;

    ////        int res = Value(xx) - Value(yy);
    ////        return res;
    ////    }

    ////    private int Value(IAlbum al)
    ////    {
    ////        return Math.Abs(_Dic.FindOrCreateEntity(al, c => RawCompare(c)));
    ////    }

    ////    public int Compare(object x, object y)
    ////    {
    ////        IAlbum xx = x as IAlbum;
    ////        IAlbum yy = y as IAlbum;

    ////        return Compare(xx, yy);
    ////    }

    ////    public IComparer Sorter
    ////    {
    ////        get { return this; }
    ////    }

    ////    ICompleteComparer<IAlbum> IAlbumBasicSorter.Sorter
    ////    {
    ////        get { return this; }
    ////    }

    ////    event EventHandler IAlbumBasicSorter.OnChanged
    ////    { add { } remove { } }

    ////}

    //#endregion

    public partial class AlbumLikehoodPresenter 
        //: AlbumPresenterUserControl
    {
        //public override IAlbumBasicSorter Sorter
        //{
        //    get
        //    {
        //        return base.Sorter;
        //    }
        //    set
        //    { }
        //}

        //private bool _Loading = true;
        //private IMusicSession _Session;
        //internal IMusicSession Session
        //{
        //    private set
        //    {
        //        if (value == null)
        //            return;

        //        _Session = value;

        //        IAlbum MyAlb = Albums.MaxBy(value.AlbumSorter.Sorter);

        //        ImprovedComparer = new ComparerTo(value, MyAlb);

        //        CenterAlbum = MyAlb;
        //        _Loading = false;
        //    }

        //    get
        //    {
        //        return _Session;
        //    }
        //}

        private bool _Loading = true;
        //private IMusicSession _Session;
        //internal IMusicSession Session
        //{
        //    private set
        //    {
        //        if (value == null)
        //            return;

        //        _Session = value;

        //        //AlbumDistanceComparerFactory adcf = new AlbumDistanceComparerFactory(value);
        //        //_OrderByAfinity = new AfinityCollection<IAlbum>(this.Albums, al => adcf.GetComparer(al), 50);
        //        //AffinityOrderedCollection = _OrderByAfinity.Collection;

        //        //CenterAlbum = Albums.MaxBy(value.AlbumSorter.Sorter);
        //        //_Loading = false;
        //    }

        //    get
        //    {
        //        return _Session;
        //    }
        //}


        //public override IEnumerable<IAlbum> SelectedAlbums
        //{
        //    get { return base.SelectedAlbums; }
        //    set { base.SelectedAlbums = value; CenterAlbum = value.FirstOrDefault() ?? VisibleAlbums.FirstOrDefault(); }
        //}

        //private ComparerTo _Comparer;
        //private ComparerTo ImprovedComparer
        //{
        //    get { return _Comparer; }
        //    set
        //    {
        //        _Comparer = value;
        //        base.Sorter = value;
        //    }
        //}

       // public static readonly DependencyProperty CenterAlbumProperty = DependencyProperty.Register(
       //"CenterAlbum", typeof(IAlbum), typeof(AlbumLikehoodPresenter), new PropertyMetadata(null, CenterAlbumChangedCallback));

       // public IAlbum CenterAlbum
       // {
       //     get { return (IAlbum)GetValue(CenterAlbumProperty); }
       //     set { SetValue(CenterAlbumProperty, value); }
       // }

        //public static readonly DependencyProperty AffinityOrderedCollectionProperty = DependencyProperty.Register("AffinityOrderedCollection",
        //        typeof(IExtendedObservableCollection<IAlbum>), typeof(AlbumLikehoodPresenter));

        //public IExtendedObservableCollection<IAlbum> AffinityOrderedCollection
        //{
        //    get { return (IExtendedObservableCollection<IAlbum>)GetValue(AffinityOrderedCollectionProperty); }
        //    set { SetValue(AffinityOrderedCollectionProperty, value); }
        //}

        

        //oldstatic private async void CenterAlbumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    AlbumLikehoodPresenter a = d as AlbumLikehoodPresenter;

        //    if (object.ReferenceEquals(e.NewValue, e.OldValue))
        //        return;

        //    if (a.ImprovedComparer != null)
        //    {
        //        if (object.ReferenceEquals(a.ImprovedComparer.Album, e.NewValue))
        //            return;

        //        a.InitChanges();
        //        a.SetComparerTo(await ComparerTo.CreateComparerAnsyc(a.Session, e.NewValue as IAlbum));
        //    }
        //}old

        //new

        //private AfinityCollection<IAlbum> _OrderByAfinity;

        //public IExtendedObservableCollection<IAlbum> AffinityOrderedCollection
        //{
        //    get { return _OrderByAfinity.Collection; }
        //}

     

        //static private async void CenterAlbumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    AlbumLikehoodPresenter a = d as AlbumLikehoodPresenter;

        //    if (object.ReferenceEquals(e.NewValue, e.OldValue))
        //        return;

        //    IAlbum al = e.NewValue as IAlbum;

        //    if (object.ReferenceEquals(a._OrderByAfinity.Reference, al))
        //        return;

        //    a.InitChanges();

        //    await a.SetCenteredAlbumAsync(al);

        //    a._Loading = false;
        //} //new

        //oldprivate void SetComparerTo(ComparerTo value)
        //{
        //    ImprovedComparer = value;
        //}old

        //private Task SetCenteredAlbumAsync(IAlbum ialbum)
        //{
        //    return _OrderByAfinity.ComputeAsync(ialbum);
        //}

        public AlbumLikehoodPresenter()
        {
            InitializeComponent();
            //this.DataContextChanged += OnDataContextChanged;
        }


        //private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Session = (e.NewValue as IMusicSession);
        //}


        //private void ChangeArtist_Click(object sender, RoutedEventArgs e)
        //{

        //    if (VisibleAlbums == null)
        //        return;

        //    if (CenterAlbum == null)
        //        return;

        //    IAlbum ia = VisibleAlbums.Where(a => a.Genre == CenterAlbum.Genre).Where(a => !CenterAlbum.Artists.Any(b => a.Artists.Contains(b))).RandomizedItem();
        //    if (ia != null)
        //        CenterAlbum = ia;

        //}

        //private void ChangeGenre_Click(object sender, RoutedEventArgs e)
        //{
        //    if (VisibleAlbums == null)
        //        return;

        //    if (CenterAlbum == null)
        //        return;

        //    IAlbum ia = VisibleAlbums.Where(a => a.Genre != CenterAlbum.Genre).RandomizedItem();
        //    if (ia != null)
        //        CenterAlbum = ia;

        //}

        //private void ChangeAlbum_Click(object sender, RoutedEventArgs e)
        //{
        //    if (VisibleAlbums == null)
        //        return;

        //    if (CenterAlbum == null)
        //        return;

        //    IAlbum ia = VisibleAlbums.Where(a => a != CenterAlbum).Where(a => CenterAlbum.Artists.Any(b => a.Artists.Contains(b))).RandomizedItem();
        //    if (ia != null)
        //        CenterAlbum = ia;
        //    else ChangeArtist_Click(sender, e);


        //}

        //private void Random_Click(object sender, RoutedEventArgs e)
        //{
        //    if (VisibleAlbums == null)
        //        return;

        //    if (CenterAlbum == null)
        //        return;

        //    IAlbum ia = VisibleAlbums.RandomizedItem();
        //    if (ia != null)
        //        CenterAlbum = ia;
        //}

        public override ListBox MyDisc
        {
            get { return ListDisc; }
        }


        private void Mute(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Toro(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;

            e.Handled = true;

            if (_Trans != null)
                return;

            //if (e.ClickCount > 1)
            //{
            //    object selected = ListDisc.ItemContainerGenerator.ItemFromContainer(lbi);
            //    if (object.ReferenceEquals(selected, CenterAlbum))
            //        return;

            //    InitChanges();
            //    CenterAlbum = selected as IAlbum;
            //}
            //else
            //{
                ListBoxItem_PreviewMouseLeftButtonDown(sender, e);
            //}
        }

        //private IList<IAlbum> _Editeds;

        //protected override void EditEntity(IEnumerable<IAlbum> objs)
        //{
        //    base.EditEntity(objs);
            //_Editeds = objs.ToList();
        //}

        //public override void CancelEdit()
        //{
        //    base.CancelEdit();
        //    //_Editeds = null;
        //}

        private void ReDraw()
        {
            InitChanges();
            //LCW.Refresh();
        }

        //public override void EndEdit()
        //{
        //    //bool bc = false;old
        //    //_Editeds.Apply(a => { if (ImprovedComparer.ResetAlbum(a)) bc = true; });

        //    base.EndEdit();
        //    //_Editeds = null;
        //    //if (bc)
        //    //    ReDraw();old
        //}

        private BeautifulSmartPanel _BSP;
        private void BeautifulSmartPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _BSP = sender as BeautifulSmartPanel;
            _BSP.OnDraw += _BSP_OnDraw;
        }

        private ITransitioner _Trans;
        private void _BSP_OnDraw(object sender, EventArgs ea)
        {
            Action ac = AfterRenderUpdate;

            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, ac);
        }

        private void AfterRenderUpdate()
        {
            EventHandler eh = null;
            eh = delegate
            {
                CompositionTarget.Rendering -= eh;
                DoAfterRenderUpdate();
            };

            CompositionTarget.Rendering += eh;
        }

        private void DoAfterRenderUpdate()
        {
            if (_Trans != null)
            {
                _Trans.Dispose();
                _Trans = null;
            }
        }

        private void InitChanges()
        {
            if ((_Trans != null) || (_Loading))
                return;

            _Trans = Transtionner.GetTransitionner();
        }

        //protected override void BeforeChangeSorter()
        //{
        //    InitChanges();
        //}


    }
}
