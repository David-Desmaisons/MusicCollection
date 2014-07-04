using System;
using System.Collections;
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for CustomizedComboBox.xaml
    /// </summary>
    public partial class CustomizedComboBox : UserControl, IDisposable
    {
        public CustomizedComboBox()
        {
            InitializeComponent();
            Filter.Text = string.Empty;
            filteringValue = string.Empty;
            KeepEdited = false;

            Filter.TextChanged += Filter_TextChanged;
            //Filter.LostFocus += Filter_LostFocus;

            // this.Dispatcher.ShutdownStarted += CleanUp;
            //this.Unloaded += CleanUp;
        }

        public void Dispose()
        {
            ItemsSource = null;
        }

        //private void CleanUp(object sender, RoutedEventArgs e)
        //{
        //    ItemsSource = null;
        //}

        public bool FilterOnBeginOnly
        {
            get { return (bool)GetValue(FilterOnBeginOnlyProperty); }
            set { SetValue(FilterOnBeginOnlyProperty, value); }
        }

        public static readonly DependencyProperty FilterOnBeginOnlyProperty = DependencyProperty.Register("FilterOnBeginOnly", typeof(bool), typeof(CustomizedComboBox), new PropertyMetadata(false));



        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(CustomizedComboBox), new PropertyMetadata(ItemsSourcePropertyChangedCallback));


        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomizedComboBox), new PropertyMetadata(SelectedPropertyChangedCallback));


        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(CustomizedComboBox), new PropertyMetadata(double.PositiveInfinity));

        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(CustomizedComboBox));

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(CustomizedComboBox), new PropertyMetadata(string.Empty, DisplayPropertyChangedCallback));

        //BindingExpression be = ItemChoosed.GetBindingExpression


        public bool EditMode
        {
            get { return (bool)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register("EditMode", typeof(bool), typeof(CustomizedComboBox), new PropertyMetadata(false, EditModePropertyChangedCallback));


        public bool CompleteOnEnter
        {
            get { return (bool)GetValue(CompleteOnEnterProperty); }
            set { SetValue(CompleteOnEnterProperty, value); }
        }
        public static readonly DependencyProperty CompleteOnEnterProperty = DependencyProperty.Register("CompleteOnEnter", typeof(bool), typeof(CustomizedComboBox), new PropertyMetadata(true));

        public bool AutoDropList
        {
            get { return (bool)GetValue(AutoDropListProperty); }
            set { SetValue(AutoDropListProperty, value); }
        }
        public static readonly DependencyProperty AutoDropListProperty = DependencyProperty.Register("AutoDropList", typeof(bool), typeof(CustomizedComboBox), new PropertyMetadata(false));

        public Brush RealBackground
        {
            get { return (Brush)GetValue(RealBackgroundProperty); }
            set { SetValue(RealBackgroundProperty, value); }
        }
        public static readonly DependencyProperty RealBackgroundProperty = DependencyProperty.Register("RealBackground", typeof(Brush), typeof(CustomizedComboBox), new PropertyMetadata(null));

        public int MaxResultForOpen
        {
            get { return (int)GetValue(MaxResultForOpenProperty); }
            set { SetValue(MaxResultForOpenProperty, value); }
        }
        public static readonly DependencyProperty MaxResultForOpenProperty = DependencyProperty.Register("MaxResultForOpen", typeof(int), typeof(CustomizedComboBox), new PropertyMetadata(50));



        private void ResetFilter(string Update = null)
        {
            Filter.TextChanged -= Filter_TextChanged;
            Filter.Text = Update ?? string.Empty;
            Filter.TextChanged += Filter_TextChanged;
            UpdateFilter();
        }

      
        public IFactory Factory
        {
            get { return (IFactory)GetValue(FactoryProperty); }
            set { SetValue(FactoryProperty, value); }
        }
        public static readonly DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(IFactory), typeof(CustomizedComboBox));


        static private void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomizedComboBox ccb = d as CustomizedComboBox;
            ccb.ObserveSource(e.OldValue);
        }

        public event EventHandler<ObjectModifiedArgs> AttributeChanged;

        static private void SelectedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomizedComboBox ccb = d as CustomizedComboBox;

            if (ccb.FilterOnBeginOnly)
            {
                ccb.ResetFilter(ccb.DisplayValue(ccb.SelectedItem));
            }

            EventHandler<ObjectModifiedArgs> sic = ccb.AttributeChanged;
            if (sic != null)
            {
                ObjectModifiedArgs oma = new ObjectModifiedArgs(d, "SelectedItem", e.OldValue, e.NewValue);
                sic(d, oma);
            }
        }

        private static Binding GetBinding(string AddInfo=null)
        {
            Binding b = new Binding();

            string Path = "SelectedItem";

            if (!string.IsNullOrEmpty(AddInfo))
                Path += "." + AddInfo;

            b.Path = new PropertyPath(Path);
            b.ElementName = "Root";
            b.NotifyOnTargetUpdated = true;
            b.Mode = BindingMode.OneWay;
            return b;
        }

        static private void DisplayPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomizedComboBox ccb = d as CustomizedComboBox;

            string old = (string)e.OldValue;
            string nnew = (string)e.NewValue;

     

            //BindingOperations.ClearBinding(ccb.ItemChoosed, TextBox.TextProperty); 
            
            //if (string.IsNullOrEmpty(nnew))
            //{
            //     return;
            //}     
            
          //  BindingExpression be = ccb.ItemChoosed.GetBindingExpression(TextBlock.TextProperty);
            Binding b = GetBinding(nnew);


            //string olpath = b.Path.Path;
            //if (!string.IsNullOrEmpty(nnew))
            //{
            //    b.Path.Path = b.Path.Path + "." + nnew;
            //}
            //else
            //{
            //    if (olpath.EndsWith(old))
            //    {
            //        if (nnew.Length == 0)
            //        {
            //            b.Path.Path = olpath.Remove(olpath.Length - old.Length - 1);
            //        }
            //        else
            //        {
            //            b.Path.Path = olpath.Remove(olpath.Length - old.Length) + nnew;
            //        }
            //    }
            //}

            try
            {
                ccb.ItemChoosed.SetBinding(TextBlock.TextProperty, b);
            }
            catch (Exception)
            {

            }

        }

        private void OnEdit()
        {
            ResetFilter(FilterOnBeginOnly ? Filter.Text : null);
        }

        static private void EditModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomizedComboBox ccb = d as CustomizedComboBox;

            if (((bool)e.NewValue == false) && ((bool)e.OldValue == true))
            {
                ccb.OnEdit();
            }
            else if (((bool)e.NewValue == true) && ((bool)e.OldValue == false))
            {
                ccb.Options.SelectedIndex = -1;
            }

            EventHandler<ObjectModifiedArgs> sic = ccb.AttributeChanged;
            if (sic != null)
            {
                ObjectModifiedArgs oma = new ObjectModifiedArgs(d, "EditMode", e.OldValue, e.NewValue);
                sic(d, oma);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Options.SelectedItem == null)
                return;


            SelectedItem = Options.SelectedItem;
            EditMode = false;
        }

        private string Filterstring(string or)
        {
            if (string.IsNullOrEmpty(or))
                return string.Empty;

            return or.WithoutAccent().ToLower();
        }

        private string FilterstringFromObject(object item)
        {
            return Filterstring(DisplayValue(item));
        }

        private string filteringValue
        {
            set;
            get;
        }

        private string DisplayValue(object Item)
        {
            if (Item == null)
                return string.Empty;

            if (string.IsNullOrEmpty(DisplayMemberPath))
                return Item.ToString();

            PropertyInfo myPropInfo = Item.GetType().GetProperty(DisplayMemberPath);

            if (myPropInfo == null)
                return string.Empty;

            object res = myPropInfo.GetValue(Item, null);
            if (res == null)
                return string.Empty;

            return res.ToString();
        }

        private bool FilterEventHandler(object item)
        {

            if (string.IsNullOrEmpty(filteringValue))
                return true;

            if (item == null)
            {
                return false;
            }

            bool res = FilterOnBeginOnly ? FilterstringFromObject(item).StartsWith(filteringValue) : FilterstringFromObject(item).Contains(filteringValue);

            return res;
        }

        private void ObserveSource(object Oldvalue)
        {
            if (Oldvalue != null)
            {
                INotifyCollectionChanged incc = Oldvalue as INotifyCollectionChanged;
                if (incc != null)
                    incc.CollectionChanged -= NotifySourceCollectionChanged;

                CollectionViewSource.GetDefaultView(Oldvalue).Filter = null;

            }

            if (ItemsSource != null)
            {
                CollectionView().Filter = FilterEventHandler;
                INotifyCollectionChanged incc = ItemsSource as INotifyCollectionChanged;
                if (incc != null)
                    incc.CollectionChanged += NotifySourceCollectionChanged;
            }
        }

        private void NotifySourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //abonnement sur itemsource changed kaka
            if (EditMode)
                return;


            if (ItemsSource == null)
                return;

            IEnumerable<object> io = ItemsSource.Cast<object>();

            if (io.Contains(SelectedItem))
                return;

            if (io.Any())
                SelectedItem = io.First();
        }

        private ICollectionView CollectionView()
        {
            return CollectionViewSource.GetDefaultView(ItemsSource);
        }

        private void UpdateFilter()
        {
            filteringValue = Filterstring(Filter.Text);

            if (ItemsSource != null)
                CollectionView().Refresh();
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilter();
            EditMode = true;
        }

        private void Filter_LostFocus(object sender, RoutedEventArgs e)
        {

            if (KeepEdited)
            {
                KeepEdited = false;
                return;
            }

            EditMode = false;
        }

        private bool KeepEdited
        {
            get;
            set;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            EditMode = false;
        }

        private void Options_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            KeepEdited = true;
        }

        private void UpdateSelectedItem()
        {
            object res = Options.Items.Cast<object>().FirstOrDefault(o => { if (o == null) return false; return Filterstring(o.ToString()) == filteringValue; });

            if (res != null)
            {
                SelectedItem = res;
                return;
            }

            //if ((EnableNewValues) && (Factory != null))
            if (Factory != null)
            {
                SelectedItem = Factory.Create(Filter.Text);
                return;
            }

            if ((CompleteOnEnter) && (Options.Items.Count == 1))
            {
                SelectedItem = Options.Items[0];
            }

        }

        private void Filter_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:

                    UpdateSelectedItem();

                    EditMode = false;
                    e.Handled = true;
                    break;

                case Key.Escape:

                    EditMode = false;
                    e.Handled = true;
                    break;
            }

        }

        public string FilterText
        {
            set
            {
                if (Filter.Text == value)
                    return;

                EditMode = true;// !string.IsNullOrEmpty(value);             
                Filter.Text = value;
                Filter.CaretIndex = (value == null) ? 0 : value.Length;
               
            }
            get
            {
                if (EditMode)
                    return Filter.Text;

                if (SelectedItem != null)
                    return SelectedItem.ToString();

                return string.Empty;
            }
        }

        private void Filter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditMode = true;
        }

        private void Filter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditMode = true;
        }

        private void Filter_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (e.Property.Name != "Visibility")
                return;

            if ((Visibility)e.TargetObject.GetValue(e.Property) == Visibility.Visible)
            {
                //Filter.Focus();
                Keyboard.Focus(Filter);
            }
            else
            {
                if (Filter.IsKeyboardFocused)
                    Keyboard.ClearFocus();
            }
        }

        private string _ItemChoosedDisplay;
        public string ItemChoosedDisplay
        {
            get { return _ItemChoosedDisplay; }
            set 
            {
                if (_ItemChoosedDisplay == value)
                    return;

                string old = _ItemChoosedDisplay;
                _ItemChoosedDisplay = value;

                EventHandler<ObjectModifiedArgs> sic = AttributeChanged;
                if (sic != null)
                {
                    ObjectModifiedArgs oma = new ObjectModifiedArgs(this, "ItemChoosedDisplay", old, _ItemChoosedDisplay);
                    sic(this, oma);
                }
            }
        }
      
        private void ItemChoosed_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ItemChoosedDisplay = this.ItemChoosed.Text;
        }

        private void ItemChoosed_Initialized(object sender, EventArgs e)
        {
            ItemChoosed.SetBinding(TextBlock.TextProperty, CustomizedComboBox.GetBinding());
        }

    }
}