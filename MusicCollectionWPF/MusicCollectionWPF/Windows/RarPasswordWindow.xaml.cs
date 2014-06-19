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

using MusicCollection.Fundation;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for RarPasswordWindow.xaml
    /// </summary>
    public partial class RarPasswordWindow : CustomWindow
    {
        private CorruptedRarOrMissingPasswordArgs _Error;

        public RarPasswordWindow(CorruptedRarOrMissingPasswordArgs Ev)
        {
            _Error = Ev;
            DataContext = _Error;
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if ((Pass.Text != null) && (Pass.Text.Length > 0))
            {
                DialogResult = true;
                _Error.accept = true;
            }

            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //private void WindowP_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.SizeToContent = SizeToContent.WidthAndHeight;
        //}
    }
}
