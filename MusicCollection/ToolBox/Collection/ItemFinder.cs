using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.ToolBox.Collection.Observable.LiveQuery;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox.Collection
{

    public sealed class ItemFinder<T> : IEntityFinder<T>, ICollectionFunctionListener<T, string>, IDisposable where T : class
    {
        private IDictionary<string, ISimpleSet<T>> _Myres = null;
        private int _Depth = 2;
        private IFunction<T, string> _Namer = null;
        private RegistorCollectionChanged<T> _RC = null;
        private IList<T> _WholeCollection = null;

        internal ItemFinder(IList<T> tracks, Expression<Func<T, string>> namer, int Capacity = 2000)
        {
            _Namer = namer.CompileToObservable();
            _Myres = new Dictionary<string, ISimpleSet<T>>(Capacity);

            _RC = RegistrorCollectionIFunctionChanges<T, string>.GetListener(tracks, _Namer, this);
            _WholeCollection = tracks;

            foreach (T t in tracks)
            {
                foreach (string s in _Namer.GetCached(t).GetLowerWithoutAccentSubstrings(_Depth))
                {
                        _Myres.FindOrCreateEntity(s, (st) => new PolyMorphSet<T>()).Add(t);
                }
            }
        }

        private IEnumerable<T> Candidate(string sear)
        {
            IEnumerable<string> strings = sear.GetExactSubstrings(_Depth);
            IEnumerable<T> Res = null;

            foreach (string s in strings)
            {
                ISimpleSet<T> r = null;
                if (!_Myres.TryGetValue(s, out r))
                    return Enumerable.Empty<T>();

                if (Res == null)
                {
                    Res = r;
                }
                else
                {
                    Res = Res.Where(t => r.Contains(t));
                }
            }

            return Res;
        }

        private IEnumerable<T> TokenSearch(string sear)
        {
            if (string.IsNullOrEmpty(sear))
                return _WholeCollection;

            ISimpleSet<T> r = null;
            _Myres.TryGetValue(sear, out r);
            return r ?? Enumerable.Empty<T>();
        }

        public IEnumerable<T> Search(string searh)
        {
            string sear = searh.ToLowerWithoutAccent();
            if (sear.Length < _Depth)
            {
                return null;
            }
            if (sear.Length == _Depth)
            {
                return TokenSearch(sear);
            }

            return Candidate(sear).Where(t => _Namer.GetCached(t).ToLowerWithoutAccent().Contains(sear));
        }


        public IEnumerable<T> SearchOrdered(string searh)
        {

            string sear = searh.ToLowerWithoutAccent();
            if (sear.Length < _Depth)
            {
                return null;
            }

            IEnumerable<T> RawAnswer = null;
            if (sear.Length == _Depth)
            {
                RawAnswer = TokenSearch(sear);
            }
            else
            {
                RawAnswer = Candidate(sear);
            }

            return RawAnswer
                   .Select(t => new Tuple<T, int>(t, _Namer.GetCached(t).ToLowerWithoutAccent().FastIndexOf(sear)))
                   .Where(t => t.Item2 != -1)
                   .OrderBy(t => (t.Item2 == 0) ? 0 : ((_Namer.GetCached(t.Item1)[t.Item2 - 1] == ' ') ? 1 : 2))
                   .Select(t => t.Item1);
        }

        #region EventListener

        void ICollectionFunctionListener<T, string>.OnCollectionItemPropertyChanged(T item, ObjectAttributeChangedArgs<string> changes)
        {
            HashSet<string> OldSs = changes.Old.GetLowerWithoutAccentSubstrings(_Depth).ToHashSet();
            HashSet<string> NewSs = changes.New.GetLowerWithoutAccentSubstrings(_Depth).ToHashSet();

            //add real new
            NewSs.Where(ns => !OldSs.Contains(ns)).Apply((s) => _Myres.FindOrCreateEntity(s, (st) => new PolyMorphSet<T>()).Add(item));

            //remove real old
            OldSs.Where(ns => !NewSs.Contains(ns)).Apply((s) => RemoveItemAssociation(item, s));

            FireUpdate();
        }

        void ICollectionFunctionListener<T, string>.OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<T, string> Changes)
        {
        }

        bool ICollectionFunctionListener<T, string>.FactorizeEvents
        {
            get { return false; }
        }

        private void RemoveItemAssociation(T item, string s)
        {
            ISimpleSet<T> r = null;
            if (!_Myres.TryGetValue(s, out r))
                return;

            r.Remove(item);
            if (r.Count == 0)
                _Myres.Remove(s);
        }

        void ICollectionListener<T>.AddItem(T newItem, int index, bool? First)
        {
            foreach (string s in _Namer.GetCached(newItem).GetLowerWithoutAccentSubstrings(_Depth))
            {
                ISimpleSet<T> r = _Myres.FindOrCreateEntity(s, (st) => new PolyMorphSet<T>());
                r.Add(newItem);
            }

            FireUpdate();
        }

        void ICollectionListener<T>.AddItems(IEnumerable<Changed<T>> sources)
        {
            ICollectionListener<T> ICollectionListener = this;
            sources.Apply(s => ICollectionListener.AddItem(s.Source, s.Index, s.First));
        }

        bool ICollectionListener<T>.RemoveItem(T oldItem, int index, bool? Last)
        {
            foreach (string s in _Namer.GetCached(oldItem).GetLowerWithoutAccentSubstrings(_Depth))
            {
                RemoveItemAssociation(oldItem, s);
            }

            FireUpdate();

            return true;
        }

        IDisposable ICollectionListener<T>.GetFactorizable()
        {
            return null;
        }

        void ICollectionListener<T>.OnClear()
        {
            _Myres.Clear();
            _Namer.UnListenAll();
            FireUpdate();
        }

        #endregion

        public void Dispose()
        {
            _Myres.Clear();
            _Namer.Dispose();
            _RC.Dispose();
        }

        private IEnumerable<T> PrivateComputeForFuzzySearch(string search, byte Distance)
        {
            int L = search.Length;
            int LMin = L - Distance;
            int LMax = L + Distance;

            HashSet<T> Res = new HashSet<T>();

            foreach (string s in search.GetExactSubstrings(_Depth))
            {
                ISimpleSet<T> r = null;
                if (!_Myres.TryGetValue(s, out r))
                    continue;

                r.Where(it => { string str = _Namer.GetCached(it).ToLowerWithoutAccent(); int Lc = str.Length; return (Lc >= LMin) && (Lc <= LMax) && (str.GetDamerauLevenshteinDistance(search) <= Distance); }).Apply(it => Res.Add(it));
            }

            return Res;
        }


        public IEnumerable<T> PrivateFindSimilarMatches(string isearch, byte Distance = 2)
        {
            if (Distance == 0)
            {
                return FindExactMatchOnNormalized(isearch);
            }

            if (isearch.Length < _Depth)
                return Enumerable.Empty<T>();

            if (Distance > 3)
                Distance = 3;

            return PrivateComputeForFuzzySearch(isearch, Distance);
        }


        public IEnumerable<T> FindSimilarMatches(string isearch, byte Distance = 2)
        {
            return PrivateFindSimilarMatches( isearch.ToLowerWithoutAccent() );
        }

        public IEnumerable<SimpleCouple<T>> FindPotentialMisname(byte Distance = 2)
        {
            HashSet<SimpleCouple<T>> set = new HashSet<SimpleCouple<T>>();

            List<T> Duplicate = new List<T>(_WholeCollection);

            //foreach (T element in Duplicate)
            //{
            //    foreach (T similar in PrivateFindSimilarMatches(_Namer.GetCached(element), Distance).Where(s => s != element))
            //    {
            //        SimpleCouple<T> tentative = new SimpleCouple<T>(element, similar);

            //        if (set.Add(tentative))
            //            yield return tentative;
            //    }
            //}
            Parallel.ForEach(Duplicate,
                () => new List<SimpleCouple<T>>(),
                (el, lc, locallist) => { locallist.AddRange(
                    PrivateFindSimilarMatches(_Namer.GetCached(el), Distance).
                    Where(s => s != el).
                    Select(sim => new SimpleCouple<T>(el, sim))); return locallist; },
                (llist) => { lock (set) llist.Apply(co => set.Add(co)); });

            return set;
        }


        private IEnumerable<T> FindExactMatchOnNormalized(string sear)
        {
            IEnumerable<T> list = (sear.Length <= _Depth) ? TokenSearch(sear) : Candidate(sear);
            return list.Where(t => _Namer.GetCached(t).ToLowerWithoutAccent() == (sear));
        }

        public IEnumerable<T> FindExactMatches(string search)
        {
            return FindExactMatchOnNormalized(search.ToLowerWithoutAccent());
        }

        public int MinimunLengthForSearch
        {
            get { return _Depth; }
        }

        private void FireUpdate()
        {
            if (OnUpdate == null)
                return;

            OnUpdate(this, EventArgs.Empty);
        }


        public event EventHandler OnUpdate;


     
    }

}
