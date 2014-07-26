using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Utilies.Edition;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Event;
using System.Threading.Tasks;

namespace MusicCollection.Utilies
{
    internal class AlbumInfoEditor :   INotifyPropertyChanged, IMultiEntityEditor
    {
        #region Artists

        public OptionArtistChooser ArtistOption { get; private set; }

        //public OptionChooserArtist AutorOption {get;private set;}

        //private IList<Artist> Artists
        //{
        //    get { return AutorOption.Artists; }
        //}
        private IList<Artist> Artists
        {
            get { var almost = ArtistOption.Values.Cast<Artist>().ToList(); return (almost.Count != 0) ? almost : null; }
        }

        public string Author
        {
            get { return ArtistOption.StringValue; }
            set { ArtistOption.StringValue = value; }
        }

        #endregion

        #region Name

        public OptionChooser<string> NameOption
        {
            get;
            private set;
        }

        internal string AlbumName
        {
            get { return NameOption.Choosed; }
            set { NameOption.Choosed = value; }
        }

        #endregion

        #region genre

        public OptionChooser<string> GenreOption
        {
            get;
            private set;
        }

        internal string Genre
        {
            get { return GenreOption.Choosed; }
            set { GenreOption.Choosed = value; }
        }

        #endregion

        #region Year

        public OptionChooser<int?> YearOption
        {
            get;
            private set;
        }

        internal int? Year
        {
            get { return YearOption.Choosed; }
            set { YearOption.Choosed = value; }
        }

        #endregion

        private IImportContext Context { get; set; }

        private IList<Track> Tracks {get;set;}

        internal AlbumInfoEditor(IEnumerable<Track> tracks, IMusicSession iContext)
        {
            Tracks = tracks.ToList();

            var ab = Tracks.Select(t => t.RawAlbum).Distinct();

            AlbumMaturity DefaultAlbumMaturity = ab.Any(a => a.Maturity == AlbumMaturity.Discover) ? AlbumMaturity.Discover : AlbumMaturity.Collection;
            Context = (iContext as IInternalMusicSession).GetNewSessionContext(DefaultAlbumMaturity);

            //AutorOption = new OptionChooserArtist(ab.Select(alb => alb.Author).Distinct(), Context.Session);
            ArtistOption = new OptionArtistChooser(ab.Select(alb => alb.Artists), Context.Session);
            GenreOption = new OptionChooser<string>(ab.Select(alb => alb.Genre).Distinct());
            YearOption = new OptionChooser<int?>(ab.Select(alb => (int?)alb.Year).Distinct());
            NameOption = new OptionChooser<string>(ab.Select(alb => alb.Name).Distinct());
        }

        protected IList<AlbumTarget> AlbumTargets {get;private set;}

        private bool IsModifiable
        {
            get
            {
                if (AlbumTargets == null)
                    return false;

                if (AlbumTargets.All(at => at.HasChangedInAlbumName))
                    return true;

                //Get not fully selected album
                var als = AlbumTargets.SelectMany(at => at.OrderedTrack).Where(ot => !ot.Complete).Select(ot => ot.OriginAlbum);
                if (!als.Any())
                    return true;

                //si pas de changements sur albums partiel je peux continuer
                return als.All(a => (((Genre == null) || (a.Genre == Genre)) && ((Year == null) || (a.Year == (int)Year))));
            }
        }

        protected virtual void UpdateTrack(Track tbd)
        {
        }

        protected virtual bool SingleTrackUpdateNeeded
        {
            get { return false; }
        }

        public void Cancel()
        {
            Dispose();
        }

        private bool AlbumModified
        {
            get
            {
                return ((ArtistOption.Changed) || (GenreOption.Changed) || (YearOption.Changed) || (NameOption.Changed));
            }
        }

        private bool CommitChanges()
        {
            if (Context == null)
                return false;

            if (!AlbumModified && !SingleTrackUpdateNeeded)
            {
                //OnEdit();
                return false;
            }

            AlbumTargets = AlbumTarget.FromListAndTargets(Tracks, Artists, AlbumName, Genre, Year);

            if (!AlbumModified || (!IsModifiable))
            {
                if (SingleTrackUpdateNeeded)
                {
                    using (IMusicTransaction IMut = Context.CreateTransaction())
                    {
                        Tracks.Apply(t => { Context.AddForUpdate(t); UpdateTrack(t); });

                        IMut.Commit();
                    }
                    //reinjecter md dans la d

                    Tracks.Apply(t => t.SavetoDisk(Context));

                }

                return true;
            }

            using (IMusicTransaction IMut = Context.CreateTransaction())
            {

                foreach (AlbumTarget at in AlbumTargets)
                {
                    AlbumStatus ass = null;

                    Album res = at.TrivialAlbum;
                    if (res == null)
                    {
                        ass = Album.GetAvailableAlbumFromTrackDescriptor(new AlbumDescriptorDecorator(at, Context));
                    }
                    else
                    {
                        ass = new AlbumStatus(res, AlbumInfo.ValidatedByEU);
                        Context.AddForUpdate(res);
                    }

                    if (!ass.Continue)
                        continue;

                    Album nn = ass.Found;
                    at.AlbumNewAlbum = nn;


                    if (ass.Status == AlbumInfo.ValidatedByEU)
                    {
                        if (Genre != null)
                        {
                            nn.Genre = Genre;
                        }
                        if (Year != null)
                        {
                            nn.Year = (int)Year;
                        }
                    }
                    else
                    {
                        nn.DateAdded = at.OrderedTrack[0].OriginAlbum.DateAdded;
                        nn.ClearImages();
                    }

                    at.OrderedTrack.Apply(ott =>
                    {
                        if ((ott.OriginAlbum != nn))
                        {
                           //ordre important ici
                            //1
                            ott.Tracks.Apply(tr => nn.CloneTrack(tr, Context));
                            //2
                            if (ott.Complete) Context.AddForRemove(ott.OriginAlbum); else Context.AddForUpdate(ott.OriginAlbum);

                            nn.UpdateImagesFrom(ott.OriginAlbum);
                        }
                    });
                }

                //aditional change on track if needed
                Tracks.Apply(UpdateTrack);

                IMut.Commit();

                //reinjecter md dans la d
                AlbumTargets.Apply(ate => { if (ate.AlbumNewAlbum != null) ate.AlbumNewAlbum.RawTracks.Apply(t => t.SavetoDisk(Context)); });
            }
            //change year and genre on 2 phase

            return true;
        }

        public void Dispose()
        {
            if (Context!=null)
            { 
                Context.Dispose();
                Context = null;
            }

            ArtistOption.Dispose();
        }

        #region event

        protected void PropertyHasChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        public bool Commit(IProgress<ImportExportErrorEventArgs> progress = null)
        {
            EventHandler<ImportExportErrorEventArgs> CallBack = (o,e) => progress.SafeReport(e);
            Context.Error += CallBack;
            bool res = CommitChanges();
            Context.Error -= CallBack;
            Dispose();
            return res;
        }

        public Task<bool> CommitAsync(IProgress<ImportExportErrorEventArgs> progress)
        {
            return Task.Run(() => Commit(progress));
        }
    }
}