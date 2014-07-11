using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for ArtistsUserControl.xaml
    /// </summary>
    public partial class ArtistsControl : UserControl, IDropTargetAdvisor, IDragSourceAdvisor
    {
        public ArtistsControl()
        {
            EnableDragAndDrop = true;
            InitializeComponent();
            this.TextInputer.ObjectChanged += new EventHandler<MusicCollection.Infra.ObjectModifiedArgs>(TextInput_ObjectChanged);
            this.TextInputer.Texter.GotFocus += new RoutedEventHandler(Texter_GotFocus);
            this.TextInputer.SizeChanged += new SizeChangedEventHandler(TextInputer_SizeChanged);
        }


        public static readonly DependencyProperty SFProperty = DependencyProperty.Register("SF", typeof(ISearchableFactory),
            typeof(ArtistsControl), new PropertyMetadata(null, SFPropertyChanged));

        public ISearchableFactory SF
        {
            get { return (ISearchableFactory)GetValue(SFProperty); }
            set { SetValue(SFProperty, value); }
        }

        private static void SFPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArtistsControl ac = d as ArtistsControl;
            ac.TextInputer.SF = e.NewValue as ISearchableFactory;
        }

        //#endregion

        void TextInputer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_SP == null)
                return;

            _SP.ScrollToHorizontalOffset(_SP.ExtentWidth);
        }

        void Texter_GotFocus(object sender, RoutedEventArgs e)
        {
            _SP.ScrollToHorizontalOffset(_SP.ExtentWidth);
        }

        public bool EnableDragAndDrop { get; set; }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(ArtistsControl), new PropertyMetadata(new ObservableCollection<object>()));
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public void Commit()
        {
            this.TextInputer.Commit();
        }

        void TextInput_ObjectChanged(object sender, MusicCollection.Infra.ObjectModifiedArgs e)
        {
            if (e.AttributeName != "SelectItem")
                return;

            if (e.NewAttributeValue == null)
                return;

            ItemsSource.Add(e.NewAttributeValue as IArtist);

            TextInputer.SelectItem = null;

            this.TextInputer.Texter.Focus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            ContentPresenter cp = bt.FindAncestor<ContentPresenter>();
            ItemsSource.Remove(cp.DataContext);
        }

        public static readonly DependencyProperty SessionProperty = DependencyProperty.Register("Session", typeof(IMusicSession), typeof(ArtistsControl));
        public IMusicSession Session
        {
            get { return (IMusicSession)GetValue(SessionProperty); }
            set { SetValue(SessionProperty, value); }
        }

        private void Panel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.TextInputer.Texter.IsKeyboardFocused)
                return;

            if ((e.Key != Key.Delete) && (e.Key != Key.Back))
                return;

            if (string.IsNullOrEmpty(this.TextInputer.Texter.Text))
            {
                int c = ItemsSource.Count;
                if (c > 0)
                {
                    ItemsSource.RemoveAt(c - 1);
                }
            }
        }

        private void Panel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == this.Border)
            {
                bool res = this.TextInputer.Texter.Focus();
                if (res)
                    e.Handled = true;
                return;
            }
        }

        private void CommitChangesifNeeded(TextBox fe)
        {
            ListBoxItem lbi = fe.FindAncestor<ListBoxItem>();

            object original = lbi.DataContext;

            string text = fe.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                ItemsSource.Remove(original);
                return;
            }

            object candidat = SF.CreateFromName(text);

            if (candidat == original)
                return;

            LB.SelectedIndex = -1;
            int Index = ItemsSource.IndexOf(original);
            if (Index == -1)
                return;

            ItemsSource[Index] = candidat;
        }

        private void Edit_LostKeyBoardFocus(object sender, RoutedEventArgs e)
        {
            CommitChangesifNeeded(sender as TextBox);
        }

        private void SplitCurrentListBoxItem(ListBoxItem lbi, TextBox tb)
        {
            string Fs = tb.Text.Remove(tb.CaretIndex);
            if (string.IsNullOrWhiteSpace(Fs))
                return;

            int startsegondstring = (tb.SelectionStart == tb.CaretIndex) ? tb.CaretIndex + tb.SelectionLength : tb.CaretIndex;

            string Ls = tb.Text.Substring(startsegondstring);
            if (string.IsNullOrWhiteSpace(Ls))
                return;

            object first = SF.CreateFromName(Fs);
            object last = SF.CreateFromName(Ls);

            int Indexc = this.ItemsSource.IndexOf(lbi.DataContext);
            ItemsSource.RemoveAt(Indexc);
            ItemsSource.Insert(Indexc, first);
            ItemsSource.Insert(Indexc + 1, last);
        }

        private void Edit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            switch (e.Key)
            {
                case Key.Enter:
                    CommitChangesifNeeded(tb);
                    break;

                case Key.Oem1:
                case Key.Oem2:
                    ListBoxItem lbi = tb.FindAncestor<ListBoxItem>();
                    SplitCurrentListBoxItem(lbi, tb);

                    break;
            }
        }

        #region IDragSourceAdvisor

        private UIElement _SI;
        UIElement IDragSourceAdvisor.SourceUI
        {
            get { return _SI;}
            set { _SI = value;}
        }

        DragDropEffects IDragSourceAdvisor.SupportedEffects
        {
            get { return DragDropEffects.Move; }
        }

        DataObject IDragSourceAdvisor.GetDataObject(UIElement draggedElt)
        {
            ListBoxItem lbi = draggedElt.FindAncestor<ListBoxItem>();
            DataObject dob = new DataObject("LBIForDD", lbi);
            dob.SetData("RawUI", draggedElt);
            dob.SetData("OriginalIndex",this.ItemsSource.IndexOf(lbi.DataContext));
            return dob;
        }

        void IDragSourceAdvisor.FinishDrag(DataObject draggedElt, DragDropEffects finalEffects, bool DropOk)
        {
        }

        bool IDragSourceAdvisor.IsDraggable(UIElement dragElt)
        {
            if (EnableDragAndDrop == false)
                return false;

            if (dragElt == this.LB)
                return false;

            ListBoxItem lbi = dragElt.FindAncestor<ListBoxItem>();
            return (lbi != null);
        }

        UIElement IDragSourceAdvisor.GetTopContainer()
        {
            return this.FindAncestor<Window>() ;
        }

        #endregion

        #region IDropTargetAdvisor

        private UIElement _TUI;
        UIElement IDropTargetAdvisor.TargetUI
        {
            get { return _TUI; }
            set { _TUI = value;}
        }

        bool IDropTargetAdvisor.ApplyMouseOffset
        {
            get { return true; }
        }

        bool IDropTargetAdvisor.IsValidDataObject(IDataObject obj)
        {
            return obj.GetDataPresent("LBIForDD");
        }

        bool IDropTargetAdvisor.OnDropCompleted(IDataObject obj, Point dropPoint, object Originalsource)
        {
            ListBoxItem lbior = obj.GetData("LBIForDD") as ListBoxItem;

            if (lbior == null)
                return false;

            UIElement tar = Originalsource as UIElement;

            ListBoxItem target = tar.FindAncestor<ListBoxItem>();

            if (target == lbior)
                return false;

            if (target != null)
            {
                object DC = lbior.DataContext;

                int or = this.ItemsSource.IndexOf(DC);

                if (or != -1)
                {
                    int dest = this.ItemsSource.IndexOf(target.DataContext);

                    object oindex = obj.GetData("OriginalIndex");
                    if (oindex != null)
                    {
                        ItemsSource.RemoveAt((int)oindex);
                    }                   
                    ItemsSource.Insert(dest, DC);
                }
            }

            return true;
        }

        UIElement IDropTargetAdvisor.GetVisualFeedback(IDataObject obj)
        {
            UIElement elt = obj.GetData("RawUI") as UIElement;
            System.Windows.Shapes.Rectangle rect = elt.CreateSnapshot();
            rect.Opacity = 0.5;
            rect.IsHitTestVisible = false;
            return rect;
        }

        UIElement IDropTargetAdvisor.GetTopContainer()
        {
            return this.LB.FindAncestor<Window>();
        }

        #endregion

        private ScrollViewer _SP;
        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _SP = sender as ScrollViewer;
        }

        private void Edit_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}

