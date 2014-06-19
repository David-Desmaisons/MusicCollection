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

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : CustomWindow
    {
        public ImportWindow(IMusicSession isession)
        {
            InitializeComponent();
            importControl1.Session = isession;
            importControl1.OK += (o, e) => { this.DialogResult = true; Close(); };
            importControl1.KO += (o, e) => { this.DialogResult = false; Close(); };
        }

        internal IMusicImporter Importer
        {
            get { return importControl1.Importer; }
        }

    }
}
