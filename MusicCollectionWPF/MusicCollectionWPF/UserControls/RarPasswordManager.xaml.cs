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
using System.IO;
using System.Diagnostics;

using MusicCollection.Fundation;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for RarPasswordManager.xaml
    /// </summary>
    public partial class RarPasswordManager : UserControl
    {
        //private IRarFileManagement _IRFM;

        //public event EventHandler<EventArgs> RequestClose;

        public RarPasswordManager()
        {
            InitializeComponent();
        }

        //  public IMusicSettings Settings
        //{
        //    set 
        //    { 
        //        _IRFM = value.RarFileManagement;
        //        DataContext = _IRFM;
        //    }
        //}

        //public RarPasswordManager(IRarFileManagement irfm)
        //{
        //    InitializeComponent();

        //    DataContext = irfm;
        //    _IRFM = irfm;
        //}

        //private void DispatchRequestClose(EventArgs e)
        //{
        //    if (RequestClose != null)
        //        RequestClose(this, null);
        //}

        //private void OK(object sender, RoutedEventArgs e)
        //{
        //    _IRFM.CommitPassWordsChange();
        //    DispatchRequestClose(e);
        //}


        //private void TextFile(object sender, RoutedEventArgs e)
        //{
  
        //    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

        //    openFileDialog.Multiselect = false;
        //    openFileDialog.Filter = "Text Files | *.txt";
        //    openFileDialog.Title = "Select text file to be imported";

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        List<string> PWR = new List<string>(_IRFM.RarPasswords);

        //        try
        //        {
        //            using (StreamReader sr = new StreamReader(openFileDialog.FileName))
        //            {
        //                String line;
        //                while ((line = sr.ReadLine()) != null)
        //                {
        //                    if (!string.IsNullOrEmpty(line))
        //                        PWR.Add(line);
        //                }
        //            }

        //            _IRFM.RarPasswords = PWR.ToArray();
        //        }
        //        catch (Exception ex)
        //        {
        //            Trace.WriteLine("The file could not be read:");
        //            Trace.WriteLine(ex.Message);
        //        }
        //    }
        //}

        //private void Cancel(object sender, RoutedEventArgs e)
        //{
        //    _IRFM.CancelChanges();
        //    DispatchRequestClose(e);
        //}

        //private void ResetList(object sender, RoutedEventArgs e)
        //{
        //    _IRFM.ClearPassWords();
        //}

        
    }
}
