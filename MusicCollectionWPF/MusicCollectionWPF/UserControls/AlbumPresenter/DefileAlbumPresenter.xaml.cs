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
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Infra;


namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for DefileAlbumPresenter.xaml
    /// </summary>
    public partial class DefileAlbumPresenter : AlbumPresenterUserControl
    // UserControl, INotifyPropertyChanged,IDisposable
    {

        public double Translate
        {
            get { return -ItemHeight / 2; }
        }



        public DefileAlbumPresenter()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            this.MyDisc.SelectionChanged += new SelectionChangedEventHandler(MyDisc_SelectionChanged);
        }

        private IMusicSession Session
        {
            set
            {
                if (value == null)
                    return;
                Sorter = value.AlbumSorter;
            }
        }


        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Session = (e.NewValue as IMusicSession);
        }

        public override ListBox MyDisc
        {
            get { return ListDisc; }
        }

        private void ListDisc_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }


        //private IEnumerable<ListBoxItem> GetPrecedent(ListBoxItem li)
        //{
        //    int res = this.ListDisc.ItemContainerGenerator.IndexFromContainer(li);

        //    for (int i = 0; i < res; i++)
        //    {
        //        ListBoxItem lbi = this.ListDisc.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
        //        if (lbi != null)
        //        {
        //            yield return lbi;
        //        }
        //    }
        //}

        //private double GetTranslateLI(ListBoxItem lbi)
        //{
        //    TranslateTransform ts = lbi.RenderTransform as TranslateTransform;
        //    return ts.X;
        //}

        //private void SetTranslateLI(ListBoxItem lbi, double dist)
        //{      
        //    TranslateTransform ts = lbi.RenderTransform as TranslateTransform;
        //    try
        //    {
        //        ts.X = dist;
        //    }
        //    catch
        //    {
        //       Console.WriteLine("Echec translation {0} Has Animation {1} ",lbi,ts.HasAnimatedProperties);
        //    }
        // }

        //private Storyboard Build(List<ListBoxItem> liss, double Distance, TimeSpan dur, TimeSpan ts)
        //{
        //    Storyboard sb = new Storyboard();

        //    foreach (ListBoxItem lis in liss)
        //    {
        //        DoubleAnimation db = new DoubleAnimation();
        //        db.To = Distance;
        //        db.Duration = dur;
        //        db.BeginTime = ts;

        //        Storyboard.SetTarget(db, lis);
        //        Storyboard.SetTargetProperty(db, new PropertyPath("RenderTransform.X"));
        //        sb.Children.Add(db);
        //    }

        //    sb.Completed += (o, e) => { var Dist = liss.Select(l => new Tuple<ListBoxItem, double>(l, this.GetTranslateLI(l))); sb.Remove(); Dist.Apply(li => SetTranslateLI(li.Item1, li.Item2)); };
        //    return sb;
        //}

        //private Storyboard _EnterSB;

        //private void StopAnimation(ListBoxItem lbi)
        //{
        //    TranslateTransform tt = lbi.RenderTransform as TranslateTransform;
        //    bool res = tt.HasAnimatedProperties;
        //    Console.WriteLine("Has animation {0}",res);
        //    try
        //    {
        //        tt.BeginAnimation(TranslateTransform.XProperty, null);
        //    }
        //    catch
        //    {
        //        Console.WriteLine("Echec Stop animation {0}", lbi);
        //    }
        //}

        //private void StopAnimationAndSetOffsets()
        //{
        //    ListBoxItem lbi = GetListBoxItemMouseOver();
        //    _MySmartPanel.Children.Cast<ListBoxItem>().Apply(StopAnimation);
        //    double dist = -(_MySmartPanel.ItemHeight * (_MySmartPanel.ItemByWidth - 1)) / _MySmartPanel.ItemByWidth;
        //    GetPrecedent(lbi).ToList().Apply(l => this.SetTranslateLI(l, dist));
        //    SetTranslateLI(lbi, 0);
        //}

        private void OnEnter(object sender, EventArgs ea)
        {
            _MySmartPanel.OnEnter(sender, ea);
            //if (_MySmartPanel.ItemByWidth == 1)
            //    return;

            //ListBoxItem li = sender as ListBoxItem;

            //if (li.IsSelected)
            //    return;

            //var res = GetPrecedent(li).ToList();

            //double dist = -(_MySmartPanel.ItemHeight * (_MySmartPanel.ItemByWidth - 1)) / _MySmartPanel.ItemByWidth;

            //_EnterSB = Build(res, dist, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));
            //_EnterSB.Completed += (o, e) => { _EnterSB = null; };
            //_EnterSB.Begin();

        }

        private void OnLeave(object sender, EventArgs ea)
        {
            _MySmartPanel.OnLeave(sender, ea);
        //    if (_MySmartPanel.ItemByWidth == 1)
        //        return;

        //    ListBoxItem li = sender as ListBoxItem;

        //    if (li.IsSelected)
        //        return;

        //    FrameworkElement fel = VisualTreeHelper.GetChild(li, 0) as FrameworkElement;

        //    while ((fel != null) && (fel.ContextMenu == null))
        //    {
        //        fel = VisualTreeHelper.GetChild(fel, 0) as FrameworkElement;
        //    }


        //    if (fel != null)
        //    {
        //        if (fel.ContextMenu.IsOpen)
        //            return;
        //    }


        //    var res = GetPrecedent(li).ToList();

        //    Storyboard sb = Build(res, 0, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
        //    sb.Begin();
        }

        //private void OnSelect(ListBoxItem lbi)
        //{
        //    if (lbi == null)
        //        return;

        //    GetPrecedent(lbi).Apply(li => this.SetTranslateLI(li, 0));
        //}

        //private void OnUnSelect(ListBoxItem lbi)
        //{
        //    if (lbi == null)
        //        return;

        //    if (lbi.IsMouseOver)
        //    {
        //        double dist = -(_MySmartPanel.ItemHeight * (_MySmartPanel.ItemByWidth - 1)) / _MySmartPanel.ItemByWidth;
        //        GetPrecedent(lbi).Apply(li => this.SetTranslateLI(li, dist));
        //    }
        //}


        private SmartPanel2 _MySmartPanel;
        private void SmartPanel2_Loaded(object sender, RoutedEventArgs e)
        {
            _MySmartPanel = sender as SmartPanel2;
            //_MySmartPanel.OnListBoxItemCreated += OnListBoxCreated;
        }

        protected override void ApplyChanges(ListBoxItem lbi, Changes ichanges)
        {
            MyDisc.SelectionChanged -= MyDisc_SelectionChanged;
            base.ApplyChanges(lbi, ichanges);
            MyDisc.SelectionChanged += MyDisc_SelectionChanged;

            _MySmartPanel.ApplyChanges(lbi, ichanges);

            //if (_EnterSB == null)
            //{
            //    SynchronizeApplyChanges(lbi, ichanges);
            //}
            //else
            //{
            //    Console.WriteLine("No Wait end animation");
            //    _EnterSB.Stop();
            //    _EnterSB.Remove();
            //    _EnterSB = null;
            //    StopAnimationAndSetOffsets();
            //    SynchronizeApplyChanges(lbi, ichanges);
            //}
        }

     

        void MyDisc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _MySmartPanel.InvalidateMeasure();
        }

       



    }
}
