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

    public partial class AlbumLikehoodPresenter : AlbumPresenterBase
        //AlbumPresenterUserControl
    {
        public static readonly DependencyProperty IsNavigatingProperty = DependencyProperty.Register(
                "IsNavigating", typeof(bool), typeof(AlbumLikehoodPresenter), new PropertyMetadata(IsNavigatingChangedCallback));

       public bool IsNavigating
       {
           get { return (bool)GetValue(IsNavigatingProperty); }
           set { SetValue(IsNavigatingProperty, value); }
       }

       static private void IsNavigatingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           AlbumLikehoodPresenter alp = d as AlbumLikehoodPresenter;
           alp.IsNavigatingChanged((bool)e.NewValue);
       }

       private void IsNavigatingChanged(bool navigationvalue)
       {
           if (navigationvalue)
               InitChanges();
           else
               DoAfterRenderUpdate();
       }

        public AlbumLikehoodPresenter()
        {
            InitializeComponent();
        }

        //public override ListBox MyDisc
        //{
        //    get { return ListDisc; }
        //}

        private void Mute(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }


        private ITransitioner _Trans;
    
        private void DoAfterRenderUpdate()
        {
            Action ac = () =>
                {
                    if (_Trans != null)
                    {
                        _Trans.Dispose();
                        _Trans = null;
                    }
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, ac);          
        }

        private void InitChanges()
        {
            if (_Trans != null)
                return;

            _Trans = Transtionner.GetTransitionner();
        }

    }
}
