﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.Infra;
using MusicCollection.WebServices;
using System.Collections.Specialized;
using MusicCollectionWPF.ViewModelHelper;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModel
{
    public class CDAlbumDescriptorCreatorViewModel : ViewModelBase
    {
        private AmbigueousCDInformationArgs _CADI;
        private IMusicSession _Session;
        private bool _IsCancel = false;
        private ArtistSearchableFactory _ArtistSearchableFactory;

        public CDAlbumDescriptorCreatorViewModel(AmbigueousCDInformationArgs cadi, IMusicSession session)
        {
            _CADI = cadi;
            _Session = session;

            _CDInfos.AddCollection(_CADI.CDInfos);

            _Authours = new ObservableCollection<IArtist>();
            _Authours.CollectionChanged += _Authours_CollectionChanged;

            if (CDInfos.Count == 0)
            {
                Created = Default.GetEditable();
            }
            else if (CDInfos.Count == 1)
            {
                Created = CDInfos[0].FindItem.GetEditable();
            }

            _ArtistSearchableFactory = new ArtistSearchableFactory(_Session);  
            
            GenreFactory = FactoryBuilder.Instanciate((n) => _Session.GetGenreFactory().Create(n));

            CommitCommand = RelayCommand.Instanciate(() => OnCommintCommand());

            InternetFind = RelayCommand.InstanciateAsync(() => FindAdditionalInfoFromWebAsync());

            iTunesFind = RelayCommand.InstanciateAsync(() => FindAdditionalInfoFromiTunesAsync());
        }

        private void OnCommintCommand()
        {
            Commit();
            this.Window.Close();
        }

        public IFactory GenreFactory
        {
            get;
            private set;
        }

        private void _Authours_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _CADI.Provider = WebProvider.Unknown;
        }

        private UISafeObservableCollection<WebMatch<IFullAlbumDescriptor>> _CDInfos = new UISafeObservableCollection<WebMatch<IFullAlbumDescriptor>>();
        public IList<WebMatch<IFullAlbumDescriptor>> CDInfos
        {
            get { return _CDInfos; }
        }

        public IFullAlbumDescriptor Default
        {
            get { return _CADI.Default; }
        }

        public IMusicSession Session
        {
            get { return _Session; }
        }

        public ArtistSearchableFactory ArtistSearchableFactory
        {
            get { return _ArtistSearchableFactory; }
        }

        private IFullEditableAlbumDescriptor _Created;
        private IFullEditableAlbumDescriptor Created
        {
            get { return _Created; }
            set
            {
                Set(ref _Created, value);
                OnCreatedChanged();
            }
        }

        public int Year
        {
            get { return Get<CDAlbumDescriptorCreatorViewModel, int>(() => (t) => (t.Created != null) ? t.Created.Year : 0); }
            set
            {
                if (_Created != null)
                {
                    _Created.Year = value;
                    return;
                }

                var newc = this.FullEditable();
                newc.Year = value;
                _Created = newc;
            }
        }

        public uint DiscNumber
        {
            get { return Get<CDAlbumDescriptorCreatorViewModel, uint>(() => (t) => (t.Created != null) ? t.Created.EditableTrackDescriptors[0].DiscNumber : 0); }
            set
            {
                if (_Created != null)
                {
                    _Created.EditableTrackDescriptors.Apply(tr => tr.DiscNumber = value);
                    return;
                }

                var newc = this.FullEditable();
                _Created.EditableTrackDescriptors.Apply(tr => tr.DiscNumber = value);
                _Created = newc;
            }
        }

        private IFullEditableAlbumDescriptor FullEditable()
        {
            _CADI.Provider = WebProvider.Unknown;
            return this.Default.GetEditable();
        }

        public string Name
        {
            get { return Get<CDAlbumDescriptorCreatorViewModel, string>(() => (t) => (t.Created != null) ? t.Created.Name : null); }
            set
            {
                _CADI.Provider = WebProvider.Unknown;

                if (_Created != null)
                {
                    _Created.Name = value;
                    return;
                }

                var newc = FullEditable();
                newc.Name = value;
                _Created = newc;
            }
        }

        private string _Genre;
        public string Genre
        {
            get { return _Genre; }
            set { this.Set(ref _Genre, value); }
        }

        private IList<IEditableTrackDescriptor> _Tracks;
        public IList<IEditableTrackDescriptor> Tracks
        {
            get { return _Tracks; }
            set { this.Set(ref _Tracks, value); }
        }

        private ObservableCollection<IArtist> _Authours;
        public IList<IArtist> Authours
        {
            get { return _Authours; }
        }

        private WebMatch<IFullAlbumDescriptor> _Option;
        public WebMatch<IFullAlbumDescriptor> Option
        {
            get { return _Option; }
            set
            {
                Set(ref _Option, value);
                if (_Option != null)
                {
                    Created = _Option.FindItem.GetEditable();
                    this._CADI.Provider = _Option.WebProvider;
                }
            }
        }

        public ICommand CommitCommand { get; private set; }

        public ICommand InternetFind { get; private set; }

        public ICommand iTunesFind { get; private set; }

        private IFullEditableAlbumDescriptor FromIFullAlbumDescriptor(IFullAlbumDescriptor entry)
        {
            IFullEditableAlbumDescriptor res = entry.GetEditable();
            if (res.TrackDescriptors.Count == 0)
                return res;

            uint dc = res.TrackDescriptors[0].DiscNumber;
            res.EditableTrackDescriptors.Apply(tr => tr.DiscNumber = dc);

            return res;
        }

        public bool ReadyForCommit
        {
            get { return Get<CDAlbumDescriptorCreatorViewModel, bool>(() => (t) => (t.Created != null)); }
        }

        public bool Commit()
        {
            if (_Created == null)
            {
                _CADI.Continue = false;
                return false;
            }

            _CADI.Continue = true;
            _Created.Artist = AuthorName;
            _Created.Genre = Genre;
            _Created.Year = Year;

            IWebResult wr = null;
            if (_Results.TryGetValue(new Tuple<string, string>(_Created.Artist, _Created.Name), out wr))
                _CADI.PreprocessedWebInfo = wr.Found;

            _CADI.SelectedInfo = _Created;
            return true;
        }

        public void Cancel()
        {
            _CADI.Continue = false;
            _IsCancel = true;
        }

        private void OnCreatedChanged()
        {
            _Authours.CollectionChanged -= _Authours_CollectionChanged;

            _Authours.Clear();

            if (_Created != null)
            {
                var res = _Session.GetArtistFromName(_Created.Artist);
                if (res != null)
                    _Authours.AddCollection(res);

                Tracks = _Created.EditableTrackDescriptors;
            }

            _Authours.CollectionChanged += _Authours_CollectionChanged;
        }

        private string AuthorName
        {
            get { return this._Authours.GetDisplayName(); }
        }

        private bool _ComputingInfoWeb = false;
        public bool ComputingInfoWeb
        {
            get { return _ComputingInfoWeb; }
            set { Set(ref _ComputingInfoWeb, value); }
        }

        private bool _ComputingInfoiTunes = false;
        public bool ComputingInfoiTunes
        {
            get { return _ComputingInfoiTunes; }
            set { Set(ref _ComputingInfoiTunes, value); }
        }

        private bool _iTunesComputed = false;
        public bool iTunesComputed
        {
            get { return _iTunesComputed; }
            set { Set(ref _iTunesComputed, value); }
        }

        public async Task  FindAdditionalInfoFromiTunesAsync()
        {
            if ((ComputingInfoWeb) || (iTunesComputed))
                return;

            this.ComputingInfoiTunes = true;


            var progresser = new WPFSynchroneProgress<ImportExportError>(
                (e) =>
                {
                    if (!_IsCancel)
                    {
                        this.ComputingInfoiTunes = false;
                        this.Window.ShowMessage(e.What, e.WindowName, false);
                    }
                });

            IDiscInformationProvider wb = _Session.GetITunesCDIdentificator();
        
            await wb.ComputeAsync(progresser);

            if (!_IsCancel)
            {
                this.ComputingInfoiTunes = false;

                if (wb.FoundCDInfo != null)
                {
                    _CDInfos.Add(wb.FoundCDInfo);
                    iTunesComputed = true;
                    Option = wb.FoundCDInfo;
                }
            }
        }

        
        public bool InternetInfoComputed(string Artist, string Name)
        {
            return this._Results.ContainsKey(new Tuple<string, string>(_Created.Artist, _Created.Name));
        }

        public async Task FindAdditionalInfoFromWebAsync()
        {
            if (ComputingInfoWeb)
                return;

            if (_Created == null)
                return;

            _Created.Artist = this.AuthorName;

            if (this._Results.ContainsKey(new Tuple<string, string>(_Created.Artist, _Created.Name)))
                return;

            ComputingInfoWeb = true;
            _Created.Artist = AuthorName;

            IWebQuery wb = _Session.WebQueryFactory.FromAlbumDescriptor(_Created);
            wb.NeedCoverArt = false;
            wb.MaxResult = 5;

            IInternetFinder iif = _Session.GetInternetFinder(wb);

            await iif.ComputeAsync(CancellationToken.None, null);

            if (!_IsCancel)
            {
                ComputingInfoWeb = false;
                var ad = iif.Query.AlbumDescriptor;

                //split album in case of multi disc:
                //I consider one IFullAlbumDescriptor by discnumber
                IList<WebMatch<IFullAlbumDescriptor>> deployedresult = iif.Result.Found.SelectMany
                    (item => item.FindItem.SplitOnDiscNumber(), (or, deployedelemt) =>
                        new WebMatch<IFullAlbumDescriptor>(deployedelemt, or.Precision, or.WebProvider)).ToList();

                _Results.Add(new Tuple<string, string>(ad.Artist, ad.Name), iif.Result);

                int tgc = Default.TrackDescriptors.Count;
                var newalbs = deployedresult.Where(wr => (wr.FindItem.TrackDescriptors.Count == tgc));
                var tentative = newalbs.FirstOrDefault();

                newalbs.Apply(wr => { if ((!_CDInfos.Contains(wr))) _CDInfos.Add(wr); });

                if (tentative != null)
                    Option = tentative;
            }
        }

        private IDictionary<Tuple<string, string>, IWebResult> _Results = new Dictionary<Tuple<string, string>, IWebResult>();

        public override void Dispose()
        {
            _ArtistSearchableFactory.Dispose();
            base.Dispose();
        }
    }
}
