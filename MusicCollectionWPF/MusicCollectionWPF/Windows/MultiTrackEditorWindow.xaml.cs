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
using MusicCollection.Infra;

using MusicCollectionWPF.Infra;


namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for MultiTrackEditor.xaml
    /// </summary>
    public partial class MultiTrackEditorWindow : CustomWindow, IWindowEditor
    {
        private IMultiEntityEditor _MYMod;

        private void Init()
        {
            multiTrackEditor1.Finalize += Finalize;

            if (_MYMod != null)
            {
                DataContext = _MYMod;
                _MYMod.EndEdit += OnEndEdit;
                _MYMod.Error += OnError;
            }
        }

        private void OnError(object sender, ImportExportErrorEventArgs e)
        {
            if (Error != null)
                Error(sender, e);
        }

        private void OnEndEdit(object sender, EventArgs ea)
        {
            if (_MYMod != null)
            {
                _MYMod.EndEdit -= OnEndEdit;
                _MYMod.Error -= OnError;
                _MYMod.Dispose();
            }

            EventHandler<EventArgs> ah = EndEdit;

            if (ah != null)
                ah(this, ea);
        }

        public MultiTrackEditorWindow()
        {
            InitializeComponent();
            Init();
        }


        private IMusicSession _IMS;

        public MultiTrackEditorWindow(IMusicSession iims, IEnumerable<ITrack> tracks)
        {
            _IMS = tracks.First().Album.Session;
            _MYMod = iims.GetTrackEditor(tracks);
            InitializeComponent();
            Title = "Multi Tracks Editor";
            Init();
        }

        public bool IsEditing { get { return true; } }


        public MultiTrackEditorWindow(IMusicSession iims, IEnumerable<IAlbum> albums)
        {
            _IMS = albums.First().Session;
            _MYMod = iims.GetAlbumEditor(albums);
            InitializeComponent();
            Init();
        }

        private bool _needClean = true;
        private void Finalize(object sender, EndEvent ee)
        {
            _needClean = false;
            multiTrackEditor1.Finalize -= Finalize;

            if (ee.OK)
            {
                this.DialogResult = true;
                _MYMod.CommitChanges(false);
            }
            else
            {
                _MYMod.Cancel();
                _MYMod.Dispose();
            }

            multiTrackEditor1.Dispose();

            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_needClean)
            {
                multiTrackEditor1.Finalize -= Finalize;

                _MYMod.Cancel();
                _MYMod.Dispose();
                multiTrackEditor1.Dispose();

            }
            base.OnClosed(e);
        }

        public event EventHandler<EventArgs> EndEdit;

        public event EventHandler<ImportExportErrorEventArgs> Error;
    }
}
