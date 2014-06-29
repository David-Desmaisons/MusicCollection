using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Implementation
{
    internal class Genre : IGenre, IComparable, ISessionPersistentObject 
        //, IComparable<IGenre>
    {

        private string _Name;
        private Genre _Father;
        private IList<Genre> _Genres;
        private IInternalMusicSession _Session;

        internal IInternalMusicSession Session
        {
            get { return _Session; }
            set { _Session = value; }
        }

   
        private int ID { get; set; }

        internal Genre() { }

        private Genre(string iName, IInternalMusicSession msi)
        {
            _Name = iName;
            _Father = null;
            _Session = msi;
         }

        private Genre(string iName, Genre iFather, IInternalMusicSession msi)
        {
            _Name = iName;
            _Father = iFather;
            msi.Genres.Register(this);
         }

        private class GenreFactory: IMusicGenreFactory
        {
            private IInternalMusicSession _Session;

             internal GenreFactory(IInternalMusicSession Session)
            {
                _Session = Session;
            }

             IGenre IMusicGenreFactory.Create(string GenreName)
             {
                 return Genre.GetGenre(GenreName,_Session,true);
             }

             IGenre IMusicGenreFactory.Get(string GenreName)
             {
                 return Genre.GetGenre(GenreName, _Session, false);
             }

             IGenre IMusicGenreFactory.CreateDummy()
             {
                 return Genre.CreateDummy(_Session);
             }
        }

        static internal IMusicGenreFactory GetFactory(IInternalMusicSession Session )
        {
            return new GenreFactory(Session);
        }

        private class Eplucheur
        {
            private string _Name;
            private string _CurrentName;
            private int _Index = 0;
            private int _Length = 0;

            internal Eplucheur(string Name)
            {
                _Name = Name.NormalizeSpace();
                _CurrentName = _Name;
                _Length = _CurrentName.Length;
            }

            internal IEnumerable<string> Depile()
            {
                _Index = _Name.Length;
                yield return _CurrentName;

                _Index = _CurrentName.LastIndexOf(@"/");

                while (_Index != -1)
                {
                    _CurrentName = _CurrentName.Remove(_Index);
                    yield return _CurrentName;

                    _Index = _CurrentName.LastIndexOf(@"/");
                }

                yield break;

            }

            internal IEnumerable<string> Rempile()
            {
                if (_Index==_Length)
                    yield break;

                _Index = _Index + 1;

                int newindex = _Name.IndexOf(@"/", _Index);

                while (newindex != -1)
                {
                    int OldIndex = _Index;
                    _Index = newindex+1;
                    yield return _Name.Substring(OldIndex, newindex - OldIndex);
                    newindex = _Name.IndexOf(@"/", _Index);
                }

                int Old = _Index;
                _Index = _Length;

                yield return _Name.Substring(Old, _Index - Old);
                yield break;

            }
        }

        static private Genre _Dummy;
        static internal Genre CreateDummy(IInternalMusicSession Session)
        {
            if (_Dummy == null)
            {
                _Dummy = new Genre(string.Empty, Session);
            }

            return _Dummy;
        }



        static internal Genre GetGenre(string name, IInternalMusicSession Session, bool iCreateIfNecessary)
        {
            if (name == null)
                return null;

            if (!iCreateIfNecessary)
                return Session.Genres.Find(name);


            Eplucheur epl = new Eplucheur(name);
            Genre MySelfOrFather = null;

            foreach(string st in epl.Depile())
            {
                MySelfOrFather = Session.Genres.Find(st);
                if (MySelfOrFather != null)
                    break;
            }

            foreach(string st in epl.Rempile())
            {
                if (MySelfOrFather == null)
                {
                    MySelfOrFather = new Genre(st, null, Session);
                }
                else
                    MySelfOrFather =  MySelfOrFather.PrivateAddSubGenre(st);
            }

            return MySelfOrFather;
        }

        public override string ToString()
        {
            return FullName;
        }

        public string Name { get { return _Name; } }

        public string FullName
        {
            get 
            {
                if (Father == null)
                    return Name;

                return Father.FullName + "/" + Name;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (object.ReferenceEquals(this, _Dummy));
            }
        }


        private string _NormalizedName;
        public string NormalizedName
        {
            get
            {
                if (_NormalizedName == null)
                    _NormalizedName = FullName.Normalized();

                return _NormalizedName;
            }
        }

        public IGenre Father { get { return _Father; } }

        private ModelToUISafeCollectionHandler<Genre, IGenre> _GenresHandler;
        private ModelToUISafeCollectionHandler<Genre, IGenre> GenresHandler
        {
            get
            {
                if (_GenresHandler == null)
                {
                    if (_Genres == null)
                        _Genres = new List<Genre>();

                    _GenresHandler = new ModelToUISafeCollectionHandler<Genre, IGenre>(_Genres);
                }
                return _GenresHandler;
            }
        }

        public IObservableCollection<IGenre> SubGenres 
        {
            get 
            {
                return GenresHandler.ReadOnlyUICollection; 
            } 
        }

        private Genre PrivateAddSubGenre(string iName)
        {
            string newname = iName.TitleLike();

            if (_Genres == null)
                _Genres = new  List<Genre>();
            else
            {
                var res = (from g in _Genres where g.Name == newname select g).ToList();
                if (res.Count > 1)
                    throw new Exception("Algo error");
                if (res.Count == 1)
                    return res[0];
            }

            Genre novo = new Genre(newname, this, _Session);
            GenresHandler.ModelCollection.Add(novo);
            //_Genres.Add(novo);
            return novo;
        }

        public IGenre AddSubGenre(string iName)
        {
            if (iName.Contains("/"))
                return null;

            return PrivateAddSubGenre(iName);
        }

        private IEnumerable<IGenre> Fathers
        {
            get
            {
                yield return this;

                IGenre current = this;
                IGenre father = null;

                while ((father = current.Father) != null)
                {
                    yield return father;
                    current = father;
                }
            }
        }

        public int Compare(IGenre other)
        {
            Genre realo = other as Genre;
            if (realo == null)
                return int.MaxValue;

            return _Session.Genres.Compare(this, realo);
        }

        public int ComputeCompare(Genre other)
        {
            Genre realo = other as Genre;

            if (realo == null)
                return int.MaxValue;

            if (realo == this)
                return 0;

            var Yours = realo.Fathers;

            IGenre commun = Fathers.Intersect(Yours).FirstOrDefault();

            if (commun == null)
                return int.MaxValue;

            return (Fathers.Index(commun) + Yours.Index(commun));
        }

        int IComparable.CompareTo(object obj)
        {
            Genre ge = obj as Genre;

            if (obj==null)
                return 1;

            return FullName.CompareTo(ge.FullName);
        }

        #region ISessionPersistentObject

        IImportContext ISessionPersistentObject.Context
        {
            get;
            set;
        }



        int ISessionPersistentObject.ID
        {
            get { return ID; }
        }

        void ISessionPersistentObject.UnRegisterFromSession(IImportContext session)
        {
        }

        void ISessionPersistentObject.Publish()
        {
        }

        void ISessionPersistentObject.Register(IImportContext iic)
        {
            iic.Session.Genres.Register(this);
        }

        void ISessionPersistentObject.OnLoad(IImportContext iic)
        {
            iic.Session.Genres.Register(this);
        }

        #endregion

        ObjectState IObjectState.UpdatedState
        {
            get { return ObjectState.Available; }
        }

        bool IObjectState.IsAlive
        {
            get { return true; }
        }

        ObjectState IObjectState.State
        {
            get { return ObjectState.Available; }
        }

        ObjectState IObjectStateCycle.InternalState
        {
            get { return ObjectState.Available; }
        }

        #region album management

        private IObservableCollection<IAlbum> _Albums = new WrappedObservableCollection<IAlbum>();
        private IObservableCollection<IAlbum> _ReadonlyAlbums;

        public IObservableCollection<IAlbum> Albums
        {
            get
            {
                if (_ReadonlyAlbums == null)
                {
                    _ReadonlyAlbums = _Albums.LiveReadOnly();
                }
                return _ReadonlyAlbums;
            }
        }

        internal void AttachAlbum(IAlbum ialbum)
        {
            _Albums.Add(ialbum);
        }

        internal void DetachAlbum(IAlbum ialbum)
        {
            _Albums.Remove(ialbum);
        }

        #endregion

        #region IObjectStateChange

        public event EventHandler<ObjectStateChangeArgs> ObjectStateChanges
        { add { } remove { } }

        public void SetInternalState(ObjectState value, IImportContext iic)
        {
        }

        public event EventHandler<ObjectModifiedArgs> ObjectChanged
        { add { } remove { } }

        public void HasBeenUpdated()
        {
        }

        #endregion
    }
}
