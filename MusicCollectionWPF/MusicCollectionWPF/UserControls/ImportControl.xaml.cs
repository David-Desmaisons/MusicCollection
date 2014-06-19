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
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.Windows;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for ImportControl.xaml
    /// </summary>
    public partial class ImportControl : UserControl, ISessionAcessor
    {
       // private string _CurrentDirectory = null;
        private IMusicSession _Session;
        private bool _Importing;

        private event EventHandler<AlbumEventArgs> _AddToPlay;
        private event RoutedEventHandler _Edit;


        //public event RoutedEventHandler Play
        //{
        //    add
        //    {
        //        _Play += value;
        //    }
        //    remove
        //    {
        //        _Play -= value;
        //    }
        //}

        public event RoutedEventHandler Edit
        {
            add
            {
                _Edit += value;
            }
            remove
            {
                _Edit -= value;
            }
        }

        internal event EventHandler<AlbumEventArgs> AlbumEvent
        {
            add
            {
                _AddToPlay += value;
            }
            remove
            {
                _AddToPlay -= value;
            }
        }


        IMusicSession ISessionAcessor.Session { get { return _Session; } set { _Session = value; } }

        public ImportControl()
        {
            InitializeComponent();
            this.DataContext = null;
        }

        private void CD_Click(object sender, RoutedEventArgs e)
        {
            IMusicImporter IM = _Session.GetImporterFactory().GetCDImporter();
            RunImporter(IM);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //if (_CurrentDirectory == null)
            //    _CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            using (System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog())
            {
                FBD.Description = "Select a folder";
                FBD.RootFolder = Environment.SpecialFolder.Desktop;
                FBD.SelectedPath = _Session.Setting.PathFolder;
                if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RunImporter(_Session.GetImporterFactory().GetFileService(FBD.SelectedPath));

                    _Session.Setting.PathFolder = FBD.SelectedPath;
                }
            }
        }

        private void Album_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            IAlbum Selected = ListDisc.SelectedItem as IAlbum;
            if ((Selected != null))
            {
                Console.WriteLine("SelectionChanged {0}", ListDisc.SelectedIndex);
            }
        }

        private void RunImporter(IMusicImporter IMu)
        {
            this.DataContext = IMu.ImportedAlbums;

            IMu.Progress += ((o, ev) => ProgressImport(ev));
            IMu.Error += ((o, ev) => ImportError(ev));
            IMu.Load(false);
        }

        private void ImportError(ImportExportErrorEventArgs Ev)
        {
            WindowFactory.GetWindowFromImporterror(Ev).ShowDialog();
        }

        private void ProgressImport(ProgessEventArgs ieea)
        {
            if (!ieea.ImportEnded)
            {
                _Importing = true;
                statustext.Text = ieea.ToString();
            }
            else
            {
                _Importing = false;
                statustext.Text = null;
            }
        }


        private void XML_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Files | " + "*.xml;*.mcc";
            openFileDialog.InitialDirectory = _Session.Setting.PathRar;
            openFileDialog.Title = "Select XML file to be imported";


            if (openFileDialog.ShowDialog() == true)
            {
                _Session.Setting.PathRar = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                RunImporter(_Session.GetImporterFactory().GetXMLImporter(openFileDialog.FileNames));
            }
        }

        private void Rar_Click(object sender, RoutedEventArgs e)
        {
            //if (_CurrentDirectory == null)
            //    _CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Rar/Zip Files | " + FileServices.GetRarFilesSelectString();
            openFileDialog.InitialDirectory = _Session.Setting.PathRar;
            openFileDialog.Title = "Select rar/zip file(s) to be imported";


            if (openFileDialog.ShowDialog() == true)
            {
                _Session.Setting.PathRar = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                RunImporter(_Session.GetImporterFactory().GetMultiRarImporter(openFileDialog.FileNames));
            }
        }

        private void Itunes_Click(object sender, RoutedEventArgs e)
        {
            ToogleAdaptor2 cna = new ToogleAdaptor2(_Session.Setting.ImportBrokenItunesTrack);
            ToogleContineCancelWindow tccgw = new ToogleContineCancelWindow("Confirm to import itunes collection", "Import broken tracks", cna);

            if (tccgw.ShowDialog() == false)
                return;

            RunImporter(_Session.GetImporterFactory().GetITunesService(cna.ResultValue));
        }


        private void OnEdit(object o, RoutedEventArgs e)
        {
            IAlbum Selected = ListDisc.SelectedItem as IAlbum;
            IModifiableAlbum IMA = Selected.GetModifiableAlbum();

            if ((IMA != null) && (_Edit != null))
            {
                _Edit(IMA, e);
            }
        }

        private void OnEvent(AlbumEventAction action, IAlbum alls)
        {
            if ((alls == null) || (_AddToPlay == null))
                return;

            _AddToPlay(this, new AlbumEventArgs(alls, action));

        }

        private void OnPlay(object o, RoutedEventArgs e)
        {         
            IAlbum al = ListDisc.SelectedItem as IAlbum;

            OnEvent(AlbumEventAction.Play,al);
        }

        internal void CanClose(System.ComponentModel.CancelEventArgs e)
        {
            if(_Importing)
            {
                CustoMessageBox cmb = new CustoMessageBox("Music Collection is importing Music", "Are you sure to quite Music Collection?", true, null);
                e.Cancel = (cmb.ShowDialog() != true);
            }
        }
    }
}
