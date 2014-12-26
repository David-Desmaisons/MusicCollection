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
using System.Collections;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for AlbumGroupedView.xaml
    /// </summary>
    public partial class AlbumGroupedView :UserControl
    {
        public AlbumGroupedView()
        {
            InitializeComponent();
        }

        private void Root_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Focus();
            IInputElement res = Keyboard.Focus(this);
        }

        private void ListDisc_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
