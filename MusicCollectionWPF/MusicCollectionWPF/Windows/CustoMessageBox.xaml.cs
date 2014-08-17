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

using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [ViewModelBinding(typeof(InfoViewModel))]
    public partial class CustoMessageBox : CustomWindow
    {

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
                IsOK = false,
                Window = this
            };

            DataContext = ivm;

            InitializeComponent();
        }

    }
}
