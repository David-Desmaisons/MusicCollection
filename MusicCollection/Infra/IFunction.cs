using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MusicCollection.Infra;

//DEM Changes TR

namespace MusicCollection.Infra
{
    public interface IFunction : IDisposable, IObjectAttribute, INotifyPropertyChanged
    {
        bool IsObserved { get; }
        
        object RawValue { get; }

        EventHandler<ObjectModifiedArgs> ObjectListener { get; }
    }

    public interface IPropertyFunction : IFunction
    {
        string PropertyName { get; }
    }

    public interface IFunction<TDes> : IFunction
    {
        TDes Value { get; }
    }

    public interface IPropertyFunction<TDes> : IPropertyFunction, IFunction<TDes>
    {
    }

    public interface ISimpleFunction<Tor, TDes> : IDisposable where Tor : class
    {
        bool FactorizeEvent { get; set; }

        Func<Tor, TDes> Evaluate { get; }

        Func<Tor, TDes> CurrentOrOldValueComputer { get; }

        TDes GetCached(Tor t);

        bool Register(Tor to);

    
        TDes RegisterAndGetValue(Tor to);  

        bool UnRegister(Tor to);

        event EventHandler<ObjectAttributesChangedArgs<Tor,TDes>> ElementsChanged;

        event EventHandler<ObjectAttributeChangedArgs<TDes>> ElementChanged;
    }


    public interface IFunction<Tor, TDes> : ISimpleFunction<Tor, TDes>, IDisposable where Tor:class
    {         
        Func<Tor, TDes> EvaluateAndRegister { get; }

        bool IsSingleRegistered(Tor t);

        void UnListenAll();

        bool IsDynamic { get; }

        bool IsParameterDynamic { get; }

        bool IsConstantDynamic { get; }

        IEnumerable<Tor> Listened
        {
            get;
        }
    }

    internal interface ICompleteFunction<Tor, TDes> : IFunction<Tor, TDes> where Tor : class
    {
        EventHandler<ObjectModifiedArgs> ListenedObjects(Tor to);

        IEnumerable<Tuple<Tor,TDes>> ListenedandCachedValue
        {
            get;
        }

        int ListenedCount
        { get; }

        ICompleteFunction<Tor, TDes> Clone();
    }

    public class ObjectAttributesChangedArgs<Tor,TDes> : EventArgs
    {
        private IDictionary<Tor, IObjectAttributeChanged<TDes>> Changed
        {
            get;
            set;
        }

        public bool HasChanged
        {
            get { return Changed.Count > 0; }
        }

        public void AddChanges(Tor or, IObjectAttributeChanged<TDes> cha)
        {
            Changed.Add(or, cha);
        }

        public IObjectAttributeChanged<TDes> GetChanges(Tor objectchanges)
        {
            IObjectAttributeChanged<TDes> res = null;
            Changed.TryGetValue(objectchanges, out res);
            return res;            
        }

        internal ObjectAttributesChangedArgs(int SizePrev)
        {
            Changed = new Dictionary<Tor, IObjectAttributeChanged<TDes>>(SizePrev);
        }
    }

    //public class GroupedValueChangedArgs<Tor, TDes> : EventArgs
    //{
    //    private Func<Tor, TDes> _RawFunc;
    //    private Func<Tor, TDes> _PFunc;
    //    internal GroupedValueChangedArgs(Tor Parameter, Func<Tor, TDes> newf, Func<Tor, TDes> oldf)
    //    {
    //        _RawFunc = newf;
    //        _PFunc = oldf;
    //        _Myobject = Parameter;
    //    }

    //    private Tor _Myobject;

    //    public ObjectAttributeChangedArgs<TDes> GetChangesFor(Tor or)
    //    {
    //        if (object.ReferenceEquals(or, _Myobject))
    //            return null;

    //        TDes oldv=_PFunc(or);
    //        TDes newv=_RawFunc(or);

    //        if (object.Equals(oldv, newv))
    //            return null;

    //        return new ObjectAttributeChangedArgs<TDes>(or, null, oldv, newv);
    //    }
    //}
}
