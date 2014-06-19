using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.Nhibernate.Mapping;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Implementation
{
    internal class Artist : StateObjectAdapter, IArtist, ISessionPersistentObject
    {
        private static readonly Regex _VA;
        private static readonly string _VAs;


        static Artist()
        {
            _VA = new Regex(@"(?i)^v\.?a\.?$");
            _VAs = "V.a.";
        }

        internal Artist()
        { }

        int ISessionPersistentObject.ID
        { get { return ID; } }

        internal Artist(string iname)
        {
            _Name = iname ?? string.Empty;
        }

        //private string _NormalizedName;
        //public string NormalizedName
        //{
        //    get
        //    {
        //        //if (_NormalizedName == null)
        //        //    _NormalizedName = _Name==null ? null: _Name.NormalizeSpace().ToLower().WithoutAccent();

        //        //return _NormalizedName;

        //        return (_Name == null) ? null : _Name.NormalizeSpace().ToLower().WithoutAccent();
        //    }
        //}

        internal void AddAlbum(Album al,IImportContext Context)
        {
            if (AlbumHandler.ModelCollection.Contains(al))
            {
                return;
            }

            AlbumHandler.ModelCollection.Add(al);

            if (AlbumHandler.ModelCollection.Count == 1)
            {
                Context.Publish(this);
            }
        }

        private void MemoryClean(IImportContext Context)
        {
            if (this.ID != 0)
            {
                //Element deja persiste, je l'ajoute a la transaction
                Context.AddForRemove(this);
                return;
            }
            else
            { 
                //element jamais periste
                //tres probablement lie a rollback de creation
                //je clean la session de l'object sans le detruire de la DB
                (this as ISessionPersistentObject).UnRegisterFromSession(Context);
                (this as ISessionPersistentObject).SetInternalState(ObjectState.Removed, Context);
            }
        }

        internal void RemoveAlbum(Album al, IImportContext Context)
        {
            AlbumHandler.ModelCollection.Remove(al);

            if (AlbumHandler.ModelCollection.Count == 0)
            {
                MemoryClean(Context);
            }
       }

        private ModelToUISafeCollectionHandler<Album, IAlbum> _AlbumHandler;

        private ModelToUISafeCollectionHandler<Album, IAlbum> AlbumHandler
        {
            get
            {
                if (_AlbumHandler == null)
                {
                    if (_Albums == null)
                        _Albums = new List<Album>();

                    _AlbumHandler = new ModelToUISafeCollectionHandler<Album, IAlbum>(_Albums);
                }
                return _AlbumHandler;
            }
        }

        public IObservableCollection<IAlbum> Albums
        {
            get { return AlbumHandler.ReadOnlyUICollection; }
        }

        private int ID { get; set; }
        private string _Name;

        private IList<Album> _privateAlbums;
        private IList<Album> _Albums
        {
            set { _privateAlbums = value; }
            get { return _privateAlbums; }
        }

        public string Name 
        { 
            get { return _Name; } 
            internal set { _Name = value; }
        }

        public override string ToString()
        {
            return _Name;
        }


        static internal string AuthorName(IEnumerable<IArtist> Artists)
        {
            if (Artists == null)
                return String.Empty;

            return AuthorName(Artists.ToList());
        }

        static internal string AuthorName(IList<string> Artists)
        {
            if (Artists == null)
                return String.Empty;

            int C = Artists.Count;

            if (C == 0)
                return String.Empty;

            if (C == 1)
                return Artists[0];

            StringBuilder SB = new StringBuilder(Artists[0]);

            int last = C - 1;

            for (int i = 1; i < C; i++)
            {
                if (i == last)
                    SB.Append(" & ");
                else
                    SB.Append(", ");

                SB.Append(Artists[i]);
            }

            return SB.ToString();
        }

        static internal string AuthorName(IList<IArtist> Artists)
        {
            if (Artists == null)
                return String.Empty;

            return AuthorName(Artists.Select(a => a.Name).ToList());        
        }

        static internal IEnumerable<Artist> GetArtistFromName(string iName, IInternalMusicSession Session)
        {
            if (iName == null)
                yield break;

            Artist res = Session.Artists.Find(iName);

            if (res != null)
            {
                yield return res;
                yield break;
            }

            char[] separators = new char[] { ',', ';', '&'};

            foreach (string Name in iName.Split(separators,StringSplitOptions.RemoveEmptyEntries))
            {
                string name = null;

                if (_VA.IsMatch(Name))
                    name = _VAs;
                else
                    name = Name.TitleLike();

                yield return Session.Artists.FindOrCreate(name, (n => new Artist(name)));
            }

            yield break;
        }

        public IImportContext Context
        {
            get;
            set;
        }

        public void Publish()
        {
        }

        public void OnLoad(IImportContext iic)
        {
            iic.Session.Artists.Register(this);
        }

        public void Register(IImportContext iic)
        {
            iic.Session.Artists.Register(this);
        }


        #region Objectstate

        void ISessionPersistentObject.UnRegisterFromSession(IImportContext iSession)
        {
            iSession.Session.Artists.Remove(this);
        }

        protected override bool IsFileBroken(bool UpdateStatusFile)
        {
            return false;
        }

        internal override void OnRemove(IImportContext iic)
        {
            if (AlbumHandler.ModelCollection.Count != 0)
                throw new Exception("Remove not allowed-life cycle artist");

        }



        #endregion
       
    }
}
