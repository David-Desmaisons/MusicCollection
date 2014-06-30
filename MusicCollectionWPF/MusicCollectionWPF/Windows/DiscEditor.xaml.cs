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
    public partial class DiscEditor : CustomWindow, IWindowEditor
    {
        private IModifiableAlbum _IModifiableAlbum;

        public DiscEditor(IModifiableAlbum Al, IMusicSession Session)
        {
            DataContext = Al;
           InitializeComponent();

           _IModifiableAlbum = Al;
           Editor.Session = Session;
           //Al.EndEdit += OnEndEdit;
        }

        private void OnEndEdit(object send, EventArgs ea)
        {
            EventHandler<EventArgs> ee = EndEdit;
            if (ee != null)
                ee(this, ea);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }


        public bool IsEditing { get { return true; } }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.ShouldClose += ((ok) => { this.DialogResult = ok; this.Close(); });
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

        public event EventHandler<EventArgs> EndEdit;

        public event EventHandler<ImportExportErrorEventArgs> Error
        {
            add { }
            remove { }
        }
    }
}
