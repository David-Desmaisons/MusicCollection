using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;

using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;


namespace MusicCollection.Implementation
{

    internal class TrackCollection : ROCollection<ITrack>, IFullObservableCollection<ITrack>, IInvariant
    {

        private StrongCacheCollection<PathDecomposeur, Track> _AllTracksByPath = new StrongCacheCollection<PathDecomposeur, Track>(t => t.PathHelper,null, () => new SortedDictionary<PathDecomposeur, Track>());
        //private StrongCacheCollection<string, Track> _AllTracksByISRC = new StrongCacheCollection<string, Track>(t => t.ISRCName, null, 10000);


        //private ILookup<string, Track> _AllTrackByHash   //= new LookUpImpl<string, Track>(t => t.MD5HashKey, 10000);
        private SimpleLookImpl<string, Track> _AllTrackByHash = new SimpleLookImpl<string, Track>(t => t.MD5HashKey, () => new SortedDictionary<string, List<Track>>());
        

        private MusicSessionImpl _Session;

        internal TrackCollection(MusicSessionImpl Session):base(10000)
        {
            _Session = Session;
        }

        internal void Register(Track tr)
        {
            _AllTracksByPath.Register(tr);
            _AllTrackByHash.Add(tr);
            //_AllTracksByISRC.Register(tr);
        }

        internal void RegisterPublish(Track tr)
        {
            Register(tr);
            Publish(tr);
        }

        public bool Invariant
        {
            get
            {
                int C = _AllTracksByPath.Count;
                if (C != _AllTrackByHash.Count)
                    return false;

                //if (C < _AllTracksByISRC.Count)
                //    return false;

                if (C != base.Count)
                    return false;

                return true;
            }
        }

        internal void Publish(Track tr)
        {
            base.RealAdd(tr);
        }

        internal void Remove(Track tr)
        {
            _AllTracksByPath.Remove(tr);
            _AllTrackByHash.Remove(tr);
            //_AllTracksByISRC.Remove(tr); 
            base.RealRemove(tr);
        }

        internal IEnumerable<Match<Track>> Find(ITrackDescriptor td)
        {
            PathDecomposeur pd = PathDecomposeur.FromName(td.Path);
            Track tr = _AllTracksByPath.Find(pd);

            if (tr != null)
                return new Match<Track>(tr, MatchPrecision.Exact).SingleItemCollection();

            //ISRC isrc = td.ISRC;// ISRCFormat.Format(td.ISRC);

            //if (isrc != null)
            //{
            //    tr = _AllTracksByISRC.Find(isrc.Name);
            //    if (tr != null)
            //        return new Match<Track>(tr, MatchPrecision.Exact).SingleItemCollection();
            //}

            if (SHA1KeyComputer.IsKeyDummy(td.MD5))
                return Enumerable.Empty<Match<Track>>();

            return FindByHashKey(td);
        }



        private IEnumerable<Match<Track>> FindByHashKey(ITrackDescriptor td)
        {
            return from tr in _AllTrackByHash[td.MD5] let ok = tr.CompareMusic(td.MusicStream(), true) where ok != false orderby (ok == true ? 0 : 1) select new Match<Track>(tr, ok == true ? MatchPrecision.Exact : MatchPrecision.Suspition);
        }

    }
}
