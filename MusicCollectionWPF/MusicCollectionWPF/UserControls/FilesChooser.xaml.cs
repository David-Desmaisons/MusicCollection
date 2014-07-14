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
using System.Globalization;
using System.IO;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for FilesChooser.xaml
    /// </summary>
    public partial class FilesChooser : UserControl
    {
        public string[] FilesPath
        {
            get { return (string[])GetValue(FilesPathProperty); }
            set { SetValue(FilesPathProperty, value); }
        }

        public static readonly DependencyProperty FilesPathProperty = DependencyProperty.Register("FilesPath", typeof(string[]), typeof(FilesChooser));

        public string File
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty FileProperty = DependencyProperty.Register("File", typeof(string), typeof(FilesChooser));


        public bool Multiselection
        {
            get { return (bool)GetValue(MultiselectionProperty); }
            set { SetValue(MultiselectionProperty, value); }
        }

        public static readonly DependencyProperty MultiselectionProperty = DependencyProperty.Register("Multiselection", typeof(bool), typeof(FilesChooser));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(FilesChooser));

        public string OriginalDirectory
        {
            get { return (string)GetValue(OriginalDirectoryProperty); }
            set { SetValue(OriginalDirectoryProperty, value); }
        }

        public static readonly DependencyProperty OriginalDirectoryProperty = DependencyProperty.Register("OriginalDirectory", typeof(string), typeof(FilesChooser));

        public FilesChooser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = Multiselection;
            openFileDialog.Filter =  Filter;

            if (Directory.Exists(OriginalDirectory))
                openFileDialog.InitialDirectory = OriginalDirectory;

            openFileDialog.Title = "Select files";


            if (openFileDialog.ShowDialog() == true)
            {
                OriginalDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                FilesPath = openFileDialog.FileNames;
                File = openFileDialog.FileName;
            }
        } 
    }
 
}
