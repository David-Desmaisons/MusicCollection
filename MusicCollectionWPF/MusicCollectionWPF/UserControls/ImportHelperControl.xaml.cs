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

namespace MusicCollectionWPF.UserControls
{
       

    /// <summary>
    /// Interaction logic for ImportHelperControl.xaml
    /// </summary>
    public partial class ImportHelperControl : UserControl, INotifyPropertyChanged
    { 
        static private string _OptionProperty = "Option";

        public event PropertyChangedEventHandler PropertyChanged;

        private IMusicImporterBuilder _IMIB = null;

        private void PropertyHasChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));

        }

        private IMusicImporter _Importer;
        public IMusicImporter Importer
        {
            get { return _Importer; }
            private set { _Importer = value; }
        }

        public ImportHelperControl()
        {
            InitializeComponent();

            OptionChooser.DataContext = this;
            Option = MusicImportExportType.Compressed;
        }

        private void UpdateBuilder()
        {
            if (Session != null) _IMIB = Session.GetImporterBuilder(Option);
            this.DataContext = _IMIB;
        }

        private IMusicSession _Session;
        public IMusicSession Session
        {
            get { return _Session; }
            set { _Session = value; UpdateBuilder(); Option = Session.Setting.MusicImporterExporter.LastImportType; }
        }

        private MusicImportExportType _ImportType;

        public MusicImportExportType Option
        {
            get { return _ImportType; }
            set { _ImportType = value; UpdateBuilder(); PropertyHasChanged(_OptionProperty); }
        }

        private AlbumMaturity _Maturity;
        public AlbumMaturity Maturity
        {
            get { return _Maturity; }
            set { _Maturity = value; }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Importer = _IMIB.BuildImporter();

            if (_Session!=null)
            {
                _Session.Setting.MusicImporterExporter.LastImportType = Option;
                if (Option == MusicImportExportType.iTunes)
                {
                    Session.Setting.iTunesSetting.ImportBrokenTrack = ((_IMIB as IiTunesImporterBuilder).ImportBrokenTracks==true)?
                         BasicBehaviour.Yes : BasicBehaviour.No;
                }
            }

            OnOK(e);
        }

        public event EventHandler<RoutedEventArgs> OK;

        private void OnOK(RoutedEventArgs e)
        {
            if (OK == null)
                return;

            OK(this, e);
        }

        public event EventHandler<RoutedEventArgs> KO;

        private void OnKO(RoutedEventArgs e)
        {
            if (KO == null)
                return;

            KO(this, e);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            OnKO(e);
        }
    }
}
