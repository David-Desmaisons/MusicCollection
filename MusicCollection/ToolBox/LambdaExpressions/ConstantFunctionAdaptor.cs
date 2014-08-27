using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;


namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal class ConstantFunctionAdaptor<Tor, TDes> : ICompleteFunction<Tor, TDes> where Tor : class
    {
        private IDictionary<Tor, int> _SourceCount =  PolyMorphDictionaryGeneric<Tor, int>.ScalableDictionary();

        internal ConstantFunctionAdaptor(Func<Tor, TDes> Const)
        {
            Evaluate = Const;
        }

        public Func<Tor, TDes> Evaluate { get; private set; }

        public Func<Tor, TDes> CurrentOrOldValueComputer
        {
            get { return Evaluate; }
        }

        public bool Register(Tor to)
        {
            int res = _SourceCount.AddOrUpdate(to, 1, (t, c) => ++c);
            return (res ==1);
        }

        public TDes RegisterAndGetValue(Tor to)
        {
            Register(to);
            return Evaluate(to);
        }

        public bool IsSingleRegistered(Tor item)
        {
            int value = -1;
            _SourceCount.TryGetValue(item, out value);
            return (value == 1);
        }

        public bool UnRegister(Tor item)
        {
            int value = -1;
            if (!_SourceCount.TryGetValue(item, out value))
            {
                Trace.WriteLine("Problem ");
                return false;
            }

            if (value > 1)
            {
                _SourceCount[item] = value - 1;
                return false;
            }
            else
            {
                _SourceCount.Remove(item);
            }

            return true;
        }

        public event EventHandler<ObjectAttributeChangedArgs<TDes>> ElementChanged
        {
            add { }
            remove { }
        }

        public event EventHandler<ObjectAttributesChangedArgs<Tor, TDes>> ElementsChanged
        {
            add { }
            remove { }
        }

        public void Dispose()
        {
        }

        public Func<Tor, TDes> EvaluateAndRegister
        {
            get { return Evaluate; }
        }

        public TDes GetCached(Tor t)
        {
            return Evaluate(t);
        }


        public bool IsDynamic
        {
            get { return false; }
        }

        public bool IsParameterDynamic
        {
            get { return false; }
        }

        public bool IsConstantDynamic
        {
            get { return false; }
        }

        public EventHandler<ObjectModifiedArgs> ListenedObjects(Tor to)
        {
            return null;
        }

        public void UnListenAll()
        {
            _SourceCount.Clear();
        }

        public IEnumerable<Tor> Listened
        {
            get { return _SourceCount.Keys; }
        }

        public IEnumerable<Tuple<Tor, TDes>> ListenedandCachedValue
        {
            get { return Listened.Select(o => new Tuple<Tor, TDes>(o, Evaluate(o))); }
        }

        public int ListenedCount
        {
            get { return _SourceCount.Count; }
        }

        public ICompleteFunction<Tor, TDes> Clone()
        {
            return new ConstantFunctionAdaptor<Tor, TDes>(Evaluate);
        }

        public bool FactorizeEvent
        {
            get { return false; }
            set { }
        }


    }
}
