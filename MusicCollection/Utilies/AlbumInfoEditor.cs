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

namespace MusicCollection.Utilies
{


    internal class AlbumInfoEditor : UIThreadSafeImportEventAdapter, IDisposable, INotifyPropertyChanged, IMultiEntityEditor
    {

        #region Artists

        public OptionChooserArtist AutorOption
        {
            get;
            private set;
        }

        private IList<Artist> Artists
        {
            get { return AutorOption.Artists; }
        }

        internal string Author
        {
            get { return AutorOption.Choosed; }
            set { AutorOption.Choosed = value; }
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

        private IImportContext Context
        {
            get;
            set;
        }

        private IList<Track> Tracks
        {
            get;
            set;
        }

        private void ErrorHandling(object sender, ImportExportErrorEventArgs ieeea)
        {
            OnError(ieeea);
        }

        internal AlbumInfoEditor(IEnumerable<Track> tracks, IMusicSession iContext)
        {
            _EndEdit = new UISafeEvent<EventArgs>(this);
            Tracks = tracks.ToList();

            var ab = Tracks.Select(t => t.RawAlbum).Distinct();

            AlbumMaturity DefaultAlbumMaturity = ab.Any(a => a.Maturity == AlbumMaturity.Discover) ? AlbumMaturity.Discover : AlbumMaturity.Collection;
            Context = (iContext as IInternalMusicSession).GetNewSessionContext(DefaultAlbumMaturity);
            Context.Error += ErrorHandling;

            AutorOption = new OptionChooserArtist(ab.Select(alb => alb.Author).Distinct(), Context.Session);
            GenreOption = new OptionChooser<string>(ab.Select(alb => alb.Genre).Distinct());
            YearOption = new OptionChooser<int?>(ab.Select(alb => (int?)alb.Year).Distinct());
            NameOption = new OptionChooser<string>(ab.Select(alb => alb.Name).Distinct());
        }

        protected IList<AlbumTarget> AlbumTargets
        {
            get;
            private set;
        }

        private bool IsModifiable
        {
            get
            {
                if (AlbumTargets == null)
                    return false;

                if (AlbumTargets.All(at => at.HasChangedInAlbumName))
                    return true;

                //if ((Artists != null) || (AlbumName != null))
                //    return true;

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
        }

        public void CommitChanges(bool Sync)
        {
            if (Sync)
            {
                CommitChanges();
            }
            else
            {
                Action Ac = CommitChanges;
                Ac.BeginInvoke(null, null);
            }
        }

        private bool AlbumModified
        {
            get
            {
                return ((AutorOption.Changed) || (GenreOption.Changed) || (YearOption.Changed) || (NameOption.Changed));
            }
        }

        private void CommitChanges()
        {

            if (Context == null)
                return;


            if (!AlbumModified && !SingleTrackUpdateNeeded)
            {
                OnEdit();
                return;
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

                    OnEdit();

                    //reinjecter md dans la d

                    Tracks.Apply(t => t.SavetoDisk(Context));

                }
                else
                {
                    OnEdit();
                }



                return;
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
                OnEdit();

                //reinjecter md dans la d
                AlbumTargets.Apply(ate => { if (ate.AlbumNewAlbum != null) ate.AlbumNewAlbum.RawTracks.Apply(t => t.SavetoDisk(Context)); });
            }

            //change year and genre on 2 phase
        }

        public void Dispose()
        {
            Context.Error -= ErrorHandling;
            Context.Dispose();
        }

        #region event

        protected void PropertyHasChanged(string PropertyName)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));

        }

        public IMusicSession Session
        {
            get { return Context.Session; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private UISafeEvent<EventArgs> _EndEdit;

        public event EventHandler<EventArgs> EndEdit
        {
            add { _EndEdit.Event += value; }
            remove { _EndEdit.Event -= value; }
        }


        private void OnEdit()
        {
            _EndEdit.Fire(new EventArgs(), true);
        }

        #endregion
    }
}