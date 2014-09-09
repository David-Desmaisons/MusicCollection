using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for DirectoryPathChooserControl.xaml
    /// </summary>
    public partial class DirectoryPathChooserControl : UserControl
    {
        public DirectoryPathChooserControl()
        {
            InitializeComponent();
            DirectoryPathTextBox.TextChanged += OnTextChanged;
        }

        public string DirectoryPath
        {
            get { return (string)GetValue(DirectoryPathProperty); }
            set { SetValue(DirectoryPathProperty, value); }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
        }

        public static readonly DependencyProperty DirectoryPathProperty = DependencyProperty.Register("DirectoryPath", typeof(string), typeof(DirectoryPathChooserControl));

        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog())
            {
                FBD.Description = "Select a folder";

                if (DirectoryPath != null)
                {
                    FBD.SelectedPath = DirectoryPath;
                }

                if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DirectoryPath = FBD.SelectedPath;
                }
            }

        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    DirectoryPath = Clipboard.GetText();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Copy-Paste exception {0}",ex));
            }
        }
    }
}
