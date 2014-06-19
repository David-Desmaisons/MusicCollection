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

using MusicCollectionWPF.UserControls;
using MusicCollectionWPF.Infra;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [ViewModelBinding(typeof(Exporter))]
    public partial class ExportHelperWindow : CustomWindow
    {
        public ExportHelperWindow()
        {
            InitializeComponent();
            //exportHelperControl1.OK += (o, e) => { this.DialogResult = true; Close(); };
            //exportHelperControl1.KO += (o, e) => { this.DialogResult = false; Close(); };
        }

        //public Exporter Exporter
        //{
        //    set { DataContext = value; exportHelperControl1.Exporter = value; }
        //}

    }
}
