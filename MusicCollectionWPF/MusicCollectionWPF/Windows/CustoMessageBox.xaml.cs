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
using System.Windows.Shapes;

using MusicCollectionWPF.Infra;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [ViewModelBinding(typeof(InfoViewModel))]
    public partial class CustoMessageBox : CustomWindow
    {
        //private ConfirmationNeededEventArgs _IEEA;

        //public CustoMessageBox(ImportExportErrorEventArgs IEEA)
        //{
        //    InitializeComponent();
        //    this.Title = IEEA.WindowName;
        //    this.textBlock1.Text = IEEA.What;

        //    if (IEEA.Who==null)
        //        this.textBlock2.Visibility = Visibility.Collapsed;
        //    else
        //        this.textBlock2.Text = IEEA.Who;

        //    _IEEA = IEEA as ConfirmationNeededEventArgs;

        //    if (_IEEA==null)
        //    {
        //        this.button1.Visibility = Visibility.Collapsed;
        //        this.button2.Visibility = Visibility.Collapsed;
        //    }

        //    this.ResizeMode = ResizeMode.CanResize;
        //}

        //public CustoMessageBox(ProgessEventArgs pea):this(pea.Operation,pea.Operation,false,pea.Entity)
        //{
        //}

        public CustoMessageBox()
        {
            InitializeComponent();
        }

        public CustoMessageBox(string iMessage, string iTitle, bool iConfirmationnedded, string iMessageAdditional=null)
        {
            InfoViewModel ivm = new InfoViewModel()
            {
                Message = iMessage,
                Title = iTitle,
                ConfirmationNeeded = iConfirmationnedded,
                MessageAdditional = iMessageAdditional,
                IsOK = false
            };

            DataContext = ivm;

            InitializeComponent();
            //this.Title = iTitle;
            //this.textBlock1.Text = Message;

            //if (who == null)
            //    this.textBlock2.Visibility = Visibility.Collapsed;
            //else
            //    this.textBlock2.Text = who;

            //_IEEA = null;

            //if (!Confirmationnedded)
            //{
            //    this.button1.Visibility = Visibility.Collapsed;
            //    this.button2.Visibility = Visibility.Collapsed;
            //}
        }

        //private void OK(object sender, RoutedEventArgs e)
        //{
        //    this.DialogResult = true;
        //    if (_IEEA != null)
        //        _IEEA.Continue = true;
        //}

      
    }
}
