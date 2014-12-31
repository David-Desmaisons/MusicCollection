using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Threading;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.Windows;


namespace MusicCollectionWPF.Infra
{
    public class CustomWindow : Window, IWindow
    {
         private BlurEffect _BlurEffect;
         private CancellationTokenSource _CTS;

         private CancellationTokenSource ResetCancellationTokenSource()
         {
             if (_CTS != null)
             {
                 _CTS.Cancel();
             }
             return _CTS = new CancellationTokenSource();
         }
    
        public CustomWindow(): base()
        {
            Opacity = GetOriginalOpacity();
            this.Loaded += CustomWindow_Loaded; 
            _BlurEffect = new BlurEffect() { Radius = 0 };
            this.Effect = _BlurEffect;
        }

        static private IViewModelBinder _ViewModelBinder;
        static CustomWindow()
        {
            _ViewModelBinder = new ViewModelBinder(typeof(CustomWindow).Assembly);
        }

        #region Display

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
           typeof(CornerRadius), typeof(CustomWindow), new FrameworkPropertyMetadata(new CornerRadius()));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected virtual void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                e.Handled = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button minimizeButton = (Button)GetTemplateChild("Minimize");
            Button closeButton = (Button)GetTemplateChild("CloseButton");
            closeButton.Click += OnCloseButtonClick;
            minimizeButton.Click += OnMinimizeClick;
            //(Label)
            UIElement titlebar = GetTemplateChild("TitleBarPart") as UIElement;

            titlebar.PreviewMouseDown += OnTitleBarMouseDown;
        }

        #endregion

        #region IWindow

        private ViewModelBase _ModelView;
        public ViewModelBase ModelView
        {
            get { return _ModelView; }
            set
            {
                _ModelView = value;
                DataContext = _ModelView;
                _ModelView.Window = this;
            }
        }

        void IWindow.Close()
        {
            Close();
        }

        public virtual IWindow CreateFromViewModel(ViewModelBase iModelViewBase)
        {
            IWindow res = _ViewModelBinder.Solve(iModelViewBase);
            if (res == null)
                return null;

            res.LogicOwner = this;
            res.CenterScreenLocation = true;
            res.ModelView = iModelViewBase;
            return res;
        }

        public string ChooseFile(string iTitle, string Extension)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = Extension;
            openFileDialog.Title = iTitle;

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return null;
        }

        public IEnumerable<string> ChooseFiles(string iTitle, string Extension, string InitialDirectory = null)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = true;
            openFileDialog.Filter = Extension;
            openFileDialog.Title = iTitle;
            if (InitialDirectory != null)
                openFileDialog.InitialDirectory = InitialDirectory;

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileNames;

            return Enumerable.Empty<string>();
        }

        public bool ShowConfirmationMessage(string iMessage, string iTitle)
        {
            CustoMessageBox cmb = new CustoMessageBox(iMessage, iTitle, true);
            cmb.Owner=this;
            return cmb.ShowDialog() == true;
        }

        public void ShowMessage(string iMessage, string iTitle, bool iBlocking)
        {
           Show( new CustoMessageBox(iMessage, iTitle, false),iBlocking);
        }

        public void ShowMessage(string iMessage, string iTitle, string iAdditionalInfo, bool iBlocking)
        {
            Show(new CustoMessageBox(iMessage, iTitle, false, iAdditionalInfo),iBlocking);             
        }

        private void Show(Window cmb, bool iBlocking)
        {
            if (iBlocking)
            {
                cmb.Owner = this;
                cmb.ShowDialog();
            }
            else
                cmb.Show();
        }

        private bool _IsLogicalyClosing = false;
        private Nullable<bool> _DialogResult = null;

        protected bool IsLogicalyClosing { get {return _IsLogicalyClosing;}}

        protected override async void OnClosing(CancelEventArgs e)
        {
            if (_IsLogicalyClosing)
            {
                if (_DialogResult != this.DialogResult)
                {
                    this.DialogResult = _DialogResult;
                }
                base.OnClosing(e);
                return;
            }

            if ((_ModelView == null) || (_ModelView.CanClose()))
            {
                base.OnClosing(e);
                if (e.Cancel == false)
                {
                    _IsLogicalyClosing = true;
                    e.Cancel = true;
                    _DialogResult = this.DialogResult;
                    await ClosingAnimation();
                    Close();
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

      
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.GetVisualChild<IDisposable>().Apply(t => t.Dispose());

            if (_ModelView != null)
            {
                _ModelView.Dispose();
                _ModelView = null;
            }
        }

        #endregion

        #region Animation

        protected virtual double GetOriginalOpacity()
        {
            return 0;
        }

        protected virtual void OnLoaded()
        {
            this.SmoothSet(FrameworkElement.OpacityProperty, 1, TimeSpan.FromSeconds(0.2));
        }

        protected virtual Task ClosingAnimation()
        {
            return this.SmoothSetAsync(FrameworkElement.OpacityProperty, 0, TimeSpan.FromSeconds(0.2));
        }

        private async void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoaded();

            if (ModelView != null)
            {
                await ModelView.InitAsync();
            }
        }

        #endregion

        IWindow IWindow.LogicOwner
        {
            get { return Owner as IWindow; }
            set { Owner = value as Window; }
        }


        public bool CenterScreenLocation
        {
            get
            {
                return WindowStartupLocation == System.Windows.WindowStartupLocation.CenterScreen;
            }
            set
            {
                if (value)
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }
        }

        bool? IWindow.ShowDialog(bool AddEffect)
        {
            if ((AddEffect) && (this._BlurEffect.Radius==0))
            {
                this.Loaded += iwindow_Loaded;
                this.Closing += iwindow_Closing;
            }
            return this.ShowDialog();
        }

        void IWindow.Show()
        {
            this.Show();
        }

        private async void iwindow_Loaded(object sender, RoutedEventArgs e)
        {
            CustomWindow iwindow = sender as CustomWindow;
            iwindow.Loaded -= iwindow_Loaded;
            CustomWindow Father = iwindow.Owner as CustomWindow;
            if (Father == null)
                return;

            await Father._BlurEffect.SafeSmoothSet(BlurEffect.RadiusProperty, Father, 5, TimeSpan.FromSeconds(0.3), ResetCancellationTokenSource());
        }

        private async void iwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CustomWindow iwindow = sender as CustomWindow;
            iwindow.Closing -= iwindow_Closing;
            CustomWindow Father = iwindow.Owner as CustomWindow;
            if (Father == null)
                return;

            await Father._BlurEffect.SafeSmoothSet(BlurEffect.RadiusProperty, Father, 0, TimeSpan.FromSeconds(0.3), ResetCancellationTokenSource());
        }


       
    }
}
