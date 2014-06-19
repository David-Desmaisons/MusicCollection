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
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for DiscEditor.xaml
    /// </summary>
    public partial class DiscEditor : CustomWindow
    {

        public DiscEditor(IModifiableAlbum Al, IMusicSession Session)
        {
            DataContext = Al;
           InitializeComponent();
           (Editor as ISessionAcessor).Session = Session;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (Editor.IsWorking)
                e.Cancel = true;

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.ShouldClose += (() => this.Close());
         }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Editor.Dispose();
        }

        public void RootGrid_KeyDown(object sender, KeyEventArgs e)
        {
            Editor.RootGrid_KeyDown(sender, e);
        }

       
    }
}
