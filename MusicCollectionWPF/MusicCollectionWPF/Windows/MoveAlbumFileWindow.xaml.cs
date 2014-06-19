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
using MusicCollection.Infra;
using MusicCollection.Fundation;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for MoveAlbumFileWindow.xaml
    /// </summary>
    public partial class MoveAlbumFileWindow : CustomWindow
    {
        //private IMusicSession _IMS;
        public MoveAlbumFileWindow()
        {
            InitializeComponent();
        }

        //public MoveAlbumFileWindow(IMusicSession ims,IEnumerable<IAlbum> al)
        //{
        //    InitializeComponent();
        //    _IMS = ims;
        //    albumDirectoryCopy1.AllAlbums = al;

        //    albumDirectoryCopy1.Directory = _IMS.Setting.MusicImporterExporterManagement.PathMove;
        //}

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //private void Click_OK(object sender, RoutedEventArgs e)
        //{
        //    if (!albumDirectoryCopy1.IsValid)
        //        return;

        //    _IMS.Setting.MusicImporterExporterManagement.PathMove = albumDirectoryCopy1.Directory;

        //    IMusicFileExporter imf = _IMS.GetExporterFactory().GetMover();
        //    if (imf == null)
        //        return;

        //    imf.AlbumToExport = albumDirectoryCopy1.SelectedAlbums;
        //    imf.FileDirectory = albumDirectoryCopy1.Directory;

        //    imf.Export(false);
        //    this.Close();
        //}
    }
}
