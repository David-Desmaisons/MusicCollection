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

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel;


namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for ExportHelperControl.xaml
    /// </summary>
    public partial class ExportHelperControl : UserControl
    {

        public ExportHelperControl()
        {
            InitializeComponent();
        }

        //public event EventHandler<RoutedEventArgs> OK;
        //public event EventHandler<RoutedEventArgs> KO;

        //private void OnOK(RoutedEventArgs e)
        //{
        //    if (OK == null)
        //        return;

        //    OK(this, e);
        //}

        //private void OnKO(RoutedEventArgs e)
        //{
        //    if (KO == null)
        //        return;

        //    KO(this, e);
        //}

        //private void button2_Click(object sender, RoutedEventArgs e)
        //{
        //    OnKO(e);
        //}

        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    OnOK(e);
        //}

        //private void ItemSourceChanged(object sender, DataTransferEventArgs e)
        //{
        //    foreach (IAlbum al in AlbumSelector.ItemsSource)
        //        AlbumSelector.SelectedItems.Add(al);
        //}

        //private Exporter _Exporter;
        //public Exporter Exporter
        //{
        //    get { return _Exporter; }
        //    set { _Exporter = value; }
        //}

        //private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    _Exporter.SelectedAlbums = AlbumSelector.SelectedItems.Cast<IAlbum>();
        //}

    }
}
