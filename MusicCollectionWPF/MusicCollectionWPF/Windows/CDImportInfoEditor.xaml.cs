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
    }
}
