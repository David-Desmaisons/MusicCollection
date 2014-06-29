﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MusicCollection.Fundation;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.Infra;
using System.Windows;
using System.Collections.Specialized;
using System.Diagnostics;

namespace MusicCollectionWPF.ViewModel
{
    public class AlbumEditorViewModel : ViewModelBase
    {
        private IModifiableAlbum _IModifiableAlbum;
        private IMusicSession _Session;
        public AlbumEditorViewModel(IMusicSession iMusicSession, IModifiableAlbum iModifiableAlbum)
        {
            _IModifiableAlbum = iModifiableAlbum;
            _Session = iMusicSession;

            ArtistSearchableFactory = new ArtistSearchableFactory(iMusicSession);

            GenreFactory = FactoryBuilder.Instanciate((n) => iMusicSession.GetGenreFactory().Create(n));

            Images = _IModifiableAlbum.Images;
            SelectedImages = new WrappedObservableCollection<IAlbumPicture>();
            SelectedTracks = new WrappedObservableCollection<IModifiableTrack>();

            if (Images.Count > 0)
                SelectedImage = Images[0];

            _Name = _IModifiableAlbum.Name;
            Authours = _IModifiableAlbum.Artists;
            _Year = _IModifiableAlbum.Year;
            _Genre = iMusicSession.GetGenreFactory().Get(_IModifiableAlbum.Genre);
            Genres = iMusicSession.AllGenres.LiveOrderBy(global => global.FullName);

            Tracks = new WrappedObservableCollection<IModifiableTrack>(_IModifiableAlbum.Tracks.
                OrderBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber).ThenBy(t => t.Name));

            SetFrontCover = Register(RelayCommand.Instanciate<IAlbumPicture>(SetToFront, ial => (ial != null) && Images.IndexOf(ial) > 0));
            ToLast = Register(RelayCommand.Instanciate<IAlbumPicture>(SetToLast, ial => (ial != null) && Images.IndexOf(ial) != Images.Count - 1));

            SplitImage = Register(RelayCommand.Instanciate<IAlbumPicture>(DoSplitImage, ial => ial != null));
            RotateImage = Register(RelayCommand.Instanciate<IAlbumPicture>(DoRotateImage, ial => ial != null));
            DeleteImage = Register(RelayCommand.Instanciate<IAlbumPicture>(DoDeleteImage, ial => ial != null));
            ImageFromFile = RelayCommand.Instanciate(DoImageFromFile);
            PasteImage = RelayCommand.InstanciateStatic(DoPasteImage, CanExecuteImage);

            FindFromDB = RelayCommand.Instanciate(DoFindFromInternet);
            BrowseInternet = RelayCommand.Instanciate(FindOnInternet);
        }

        private void DoImageFromFile()
        {
            var results = this.Window.ChooseFiles("Select Image", "All Image Files | " + FileServices.GetImagesFilesSelectString());

            int index = (SelectedImage == null) ? -1 : _IModifiableAlbum.Images.IndexOf(SelectedImage);

            var mynew = results.Select(filename => _IModifiableAlbum.AddAlbumPicture(filename, ++index)).ToList().FirstOrDefault();

            if (mynew != null)
                SelectedImage = mynew;
        }

        private void DoFindFromInternet()
        {
            WebAlbumSelectorViewModel wasvm = new WebAlbumSelectorViewModel(this._Session, this._IModifiableAlbum);
            Window.CreateFromViewModel(wasvm).ShowDialog();
        }

        private void FindOnInternet()
        {
            InternetFinderViewModel wasvm = new InternetFinderViewModel(this._IModifiableAlbum);
            Window.CreateFromViewModel(wasvm).Show();
        }

        private void DoSplitImage(IAlbumPicture ial)
        {
            int Index = Images.IndexOf(ial);
            if (Index == -1) return;
            SelectedImage = _IModifiableAlbum.SplitImage(Index);
        }

        private void DoRotateImage(IAlbumPicture ial)
        {
            int Index = Images.IndexOf(ial);
            if (Index == -1) return;
            SelectedImage = _IModifiableAlbum.RotateImage(Index, true);
        }

        private void DoDeleteImage(IAlbumPicture ial)
        {
            int Index = Images.IndexOf(ial);
            if (Index == -1) return;

            _IModifiableAlbum.Images.RemoveAt(Index);
           
            int count = _IModifiableAlbum.Images.Count;
            if (count>0)
            {
                int imaindex = Math.Min(Math.Max(Index--, 0), count - 1);
                SelectedImage = _IModifiableAlbum.Images[imaindex];
            }
        }

        private void SetToFront(IAlbumPicture ial)
        {
            if (ial == null)
                return;

            int Index = Images.IndexOf(ial);
            if (Index == -1)
                return;

            Images.Move(Index, 0);
        }

        private void SetToLast(IAlbumPicture ial)
        {
            if (ial == null)
                return;

            int Index = Images.IndexOf(ial);
            if (Index == -1)
                return;

            Images.Move(Index, Images.Count - 1);
        }

        private void DoPasteImage()
        {
            int c = (SelectedImage == null) ? 0 : _IModifiableAlbum.Images.IndexOf(SelectedImage);
            IAlbumPicture newapic = null;

            try
            {
                if (Clipboard.ContainsImage())
                {
                    newapic = _IModifiableAlbum.AddAlbumPicture(Clipboard.GetImage(), c);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    int Index = c;
                    Clipboard.GetFileDropList().Cast<string>().Where(f => FileServices.GetFileType(f) == FileType.Image)
                            .Apply(sn => _IModifiableAlbum.AddAlbumPicture(sn, Index++));
                    newapic = _IModifiableAlbum.Images[c];
                }

                SelectedImage = newapic;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem using clipboard: Memory issue?:" + ex.ToString());
            }
        }

        private bool CanExecuteImage()
        {
            if (Clipboard.ContainsImage())
                return true;

            if (!Clipboard.ContainsFileDropList())
                return false;

            try
            {
                StringCollection res = Clipboard.GetFileDropList();
                if (res == null)
                    return false;

                return res.Cast<string>().Any(f => FileServices.GetFileType(f) == FileType.Image);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem using clipboard:" + ex.ToString());
            }

            return false;
        }


        //private void PrefixeTrack(IModifiableTrack imt)
        //{
        //    imt.Name = string.Format("{0}-{1}", imt.Father.Artists.GetDisplayName(), imt.Name);
        //}

        //private void PreFixByArtistName(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mi = sender as MenuItem;
        //    var trcs = TrackFromContext(mi.DataContext as IModifiableTrack);

        //    if (trcs == null)
        //        return;

        //    trcs.Apply(PrefixeTrack);
        //}

        //DiscNumber

        public IList<IModifiableTrack> Tracks { get; private set; }

        public IList<IGenre> Genres { get; private set; }


        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.Set(ref _Name, value); }
        }

        private int _Year;
        public int Year
        {
            get { return _Year; }
            set { this.Set(ref _Year, value); }
        }

        private IGenre _Genre;
        public IGenre Genre
        {
            get { return _Genre; }
            set { this.Set(ref _Genre, value); }
        }

        private IAlbumPicture _SelectedImage;
        public IAlbumPicture SelectedImage
        {
            get { return _SelectedImage; }
            set { this.Set(ref _SelectedImage, value); }
        }

        public ObservableCollection<IAlbumPicture> Images { get; private set; }

        public IFactory GenreFactory { get; private set; }

        public ArtistSearchableFactory ArtistSearchableFactory { get; private set; }

        public IList<IArtist> Authours { get; private set; }

        public WrappedObservableCollection<IAlbumPicture> SelectedImages { get; private set; }

        public WrappedObservableCollection<IModifiableTrack> SelectedTracks { get; private set; }

        public bool CanMoveImage { get { return this.Get<AlbumEditorViewModel, bool>(() => (t) => t.Images.Count > 1); } }

        public ICommand FindFromDB { get; private set; }
        public ICommand OK { get; private set; }
        public ICommand BrowseInternet { get; private set; }
        public ICommand SetFrontCover { get; private set; }
        public ICommand ToLast { get; private set; }
        public ICommand ImageFromFile { get; private set; }
        public ICommand SplitImage { get; private set; }
        public ICommand RotateImage { get; private set; }
        public ICommand DeleteImage { get; private set; }
        public ICommand PasteImage { get; private set; }
        public ICommand DeleteTrack { get; private set; }
        public ICommand WindowOpenTrack { get; private set; }
        public ICommand UpdateFromFileName { get; private set; }
        public ICommand RemoveTrackNumber { get; private set; }
    }

}