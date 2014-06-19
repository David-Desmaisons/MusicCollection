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

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModelHelper;


namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for CDImportInfoEditor.xaml
    /// </summary>
    [ViewModelBinding(typeof(CDAlbumDescriptorCreatorViewModel))]
    public partial class CDImportInfoEditor : CustomWindow
    {
        public CDImportInfoEditor()
        {
            InitializeComponent();
        }

     
        //private ArtistSearchableFactory _ASF;

        //public CDImportInfoEditor(CDAlbumDescriptorCreatorViewModel cadc)
        //{
        //    CDAlbumDescriptorCreator = cadc;
        //    InitializeComponent();
        //    //this.ArtistsControl1.SF = new ArtistSearchableFactory(CDAlbumDescriptorCreator.Session);
        //    //this.ArtistsControl1.SF = _ASF;
        //}

        //private CDAlbumDescriptorCreatorViewModel _CDAlbumDescriptorCreator;
        //public CDAlbumDescriptorCreatorViewModel CDAlbumDescriptorCreator
        //{
        //    get { return _CDAlbumDescriptorCreator; }
        //    set
        //    {
        //        _CDAlbumDescriptorCreator = value;
        //        DataContext = _CDAlbumDescriptorCreator;
        //        //_ASF = new ArtistSearchableFactory(_CDAlbumDescriptorCreator.Session);
        //        //if (this.ArtistsControl1!=null)
        //        //    this.ArtistsControl1.SF = new ArtistSearchableFactory(_CDAlbumDescriptorCreator.Session);
        //    }
        //}

        //private void Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    _CDAlbumDescriptorCreator.Cancel();
        //    DialogResult = false;
        //    this.Close();
        //}

        //private void OK_Click(object sender, RoutedEventArgs e)
        //{
        //    //this.ArtistsControl1.Commit();
        //    _CDAlbumDescriptorCreator.Commit();
        //    DialogResult = true;
        //    this.Close();
        //}

        //private void Internet_Click(object sender, RoutedEventArgs e)
        //{
        //    //this.ArtistsControl1.Commit();
        //    _CDAlbumDescriptorCreator.FindAdditionalInfoFromWeb();
        //}

        //private void iTunes_Click(object sender, RoutedEventArgs e)
        //{
        //    //this.ArtistsControl1.Commit();
        //    _CDAlbumDescriptorCreator.FindAdditionalInfoFromiTunes();
        //}

        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);
        //    _CDAlbumDescriptorCreator.Dispose();
        //    //_ASF.Dispose();
        //}
    }
}
