using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Implementation
{
    internal class ArtistCollection : IDisposable
    {
        //private StrongCacheCollection<string, Artist> _AllArtistsByName = new StrongCacheCollection<string, Artist>((a => a.NormalizedName), (s => s.NormalizeSpace().ToLower().WithoutAccent()));

        private IEntityFinder<IArtist> _AllArtistsByName;
        
        private ModelToUISafeCollectionHandler<Artist, IArtist> _OArtists = null;
        private List<Artist> _Artists = new List<Artist>();
        private MusicSessionImpl _Session;

        internal ArtistCollection(MusicSessionImpl Session)
        {
            _Session = Session;
            _OArtists = new ModelToUISafeCollectionHandler<Artist, IArtist>(_Artists);
            _AllArtistsByName = new ItemFinder<IArtist>(_OArtists.ModifiableUICollection, a => a.Name);
            //.NormalizeSpace().ToLower().WithoutAccent());
        }

        public IEntityFinder<IArtist> ArtistFinder
        {
            get { return _AllArtistsByName; }
        }

        private void LifeChanges(object sender, ObjectStateChangeArgs args)
        {
            if (args.NewState == ObjectState.Removed)
            {
                IObjectState ios = args.Changed;
                ios.ObjectStateChanges -= LifeChanges;
                RawCollection.Remove(ios as Artist);              
            }
        }


        internal void Register(Artist tr)
        {
            //_AllArtistsByName.Register(tr);
            BasicRegister(tr);
        }

        private void BasicRegister(Artist tr)
        {
             RawCollection.Add(tr);

            IObjectState ios = tr;
            ios.ObjectStateChanges += LifeChanges;
        }

        internal void Publish(IArtist o)
        {
            Artist tr = o as Artist;

            if (!RawCollection.Contains(tr))
                BasicRegister(tr);
        }

        private void UnPublish(IArtist o)
        {
            Artist tr = o as Artist;

            IObjectState ios = tr;
            ios.ObjectStateChanges -= LifeChanges;
            RawCollection.Remove(tr);
        }

        internal IObservableCollection<IArtist> ReadOnlyUICollection
        {
            get { return _OArtists.ReadOnlyUICollection; }
        }

        internal IImprovedList<Artist> RawCollection
        {
            get { return _OArtists.ModelCollection; }
        }

        internal void Remove(Artist tr)
        {
            //_AllArtistsByName.Remove(tr);
            UnPublish(tr);
            
        }

        internal Artist Find(string Name)
        {
            //return _AllArtistsByName.Find(Name);
            return _AllArtistsByName.FindExactMatches(Name.NormalizeSpace()).FirstOrDefault() as Artist;
        }


        internal Artist FindOrCreate(string key, Func<string, Artist> Constructor)
        {
            //Tuple<Artist, bool> res = _AllArtistsByName.FindOrCreateValue(key, Constructor);
            //return res.Item1;

            Artist res = Find(key);
            if (res != null)
            {
                return res;
            }

            res = Constructor(key);
            RawCollection.Add(res);
            return res;
        }

        public void Dispose()
        {
            _AllArtistsByName.Dispose();
        }
    }
 
}
