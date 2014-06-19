using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.ToolBox.FunctionListener;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using MusicCollection.ToolBox.Collection;

namespace MusicCollection.Infra
{
    public class NotifyCompleteListenerObject : NotifyCompleteAdapterWithCache, IDisposable
    {
        //private RawResultListenerCollection __AttributeCollection = null;
        //private RawResultListenerCollection AttributeCollection
        //{
        //    get
        //    {
        //        if (__AttributeCollection == null)
        //        {
        //            __AttributeCollection = GlobalFunctionListener.GetFromInstance(this);
        //        }
        //        return __AttributeCollection;
        //    }
        //}

        protected NotifyCompleteListenerObject()
        {
            //_AttributeCollection = GlobalFunctionListener.GetFromInstance(this);
        }

        //protected TDes GetValue<Tor, TDes>(IResultListenerFactory<Tor, TDes> Factory) where Tor : class
        //{
        //    //return AttributeCollection.Get<TDes>(Factory).Value;
        //    return default(TDes);
        //}

        //private static Lazy<IDictionary<string, IRawResultListenerFactory>> _MyDynamicFuntion =
        //   new Lazy<IDictionary<string, IRawResultListenerFactory>>(() => new PolyMorphSimpleDictionary<string, IRawResultListenerFactory>());
        //  protected TDes Dynamic<Tor, TDes>(Expression<Func<Tor, TDes>> expression, [CallerMemberName] string propertyName = null) where Tor : NotifyCompleteListenerObject
        //{
        //    IRawResultListener

        //    IResultListenerFactory<Tor, TDes> res = _MyDynamicFuntion.Value.FindOrCreateEntity(propertyName, (s) =>
        //        ListenerFunctionBuilder.Register(expression,s)) as IResultListenerFactory<Tor, TDes>;

        //    return AttributeCollection.Find<TDes>(res).Value;
        //}

        private static Lazy<IDictionary<string, IRawResultListenerFactory>> _ClassFactories =
           new Lazy<IDictionary<string, IRawResultListenerFactory>>(() => new Dictionary<string, IRawResultListenerFactory>());

        private Lazy<ObjectDynamicAtributes> _ObjectDynamicAtributes= 
            new Lazy<ObjectDynamicAtributes>(()=> new ObjectDynamicAtributes());


        protected TDes Get<Tor, TDes>(Func<Expression<Func<Tor, TDes>>> iexpression, [CallerMemberName] string propertyName = null) where Tor : NotifyCompleteListenerObject
        {
            IRawResultListener res = 
                _ObjectDynamicAtributes.Value.GetOrCreate<TDes>(propertyName,
                    (pn) =>
                        { 
                            string cn = string.Format("{0}.{1}", propertyName, typeof(Tor).FullName);
                            IRawResultListenerFactory facres = _ClassFactories.Value.FindOrCreate_ThreadSafe(cn,
                                    (s) => FactoryListenerBuilder<Tor, TDes>.FunctionResultListenerFactory(iexpression(), propertyName)).Item;
                                        //GlobalFunctionListener.Register<Tor, TDes>(iexpression(), propertyName));
                                        //ListenerFunctionBuilder.Register(iexpression(), propertyName));
                            return facres.CreateRawListener(this);
                        });

            return (res as IResultListener<TDes>).Value;
        }

        ////private Lazy<IDictionary<string, IRawResultListener>> _InstanceAttribute =
        ////  new Lazy<IDictionary<string, IRawResultListener>>(() => new PolyMorphDictionary<string, IRawResultListener>());

        ////protected TDes Dynamic<Tor, TDes>(Expression<Func<Tor, TDes>> expression, [CallerMemberName] string propertyName = null) where Tor : NotifyCompleteListenerObject
        ////{
        ////    IRawResultListener almostres = null;
        ////    if (_InstanceAttribute.IsValueCreated)
        ////        _InstanceAttribute.Value.TryGetValue(propertyName, out almostres);

        ////    if (almostres == null)
        ////    {
        ////        IResultListenerFactory<Tor, TDes> facres = _ClassFactories.Value.FindOrCreateEntity(propertyName, (s) =>
        ////            ListenerFunctionBuilder.Register(expression, s)) as IResultListenerFactory<Tor, TDes>;
        ////        almostres = AttributeCollection.Find<TDes>(facres);
        ////        _InstanceAttribute.Value.Add(propertyName, almostres);
        ////    }

        ////    return (almostres as IResultListener<TDes>).Value;
        ////}

        //protected TDes Dynamic<Tor, TDes>(Func<Expression<Func<Tor, TDes>>> iexpression, [CallerMemberName] string propertyName = null) where Tor : NotifyCompleteListenerObject
        //{
        //    IRawResultListener almostres = null;
        //    if (_InstanceAttribute.IsValueCreated)
        //        _InstanceAttribute.Value.TryGetValue(propertyName, out almostres);

        //    if (almostres == null)
        //    {
        //        string cn = string.Format("{0}{1}", propertyName,typeof(Tor).FullName);
        //        IResultListenerFactory<Tor, TDes> facres = _ClassFactories.Value.FindOrCreateEntity(cn, (s) =>
        //            ListenerFunctionBuilder.Register(iexpression(), propertyName)) as IResultListenerFactory<Tor, TDes>;

        //        almostres = AttributeCollection.Find<TDes>(facres);
        //        _InstanceAttribute.Value.Add(propertyName, almostres);
        //    }

        //    return (almostres as IResultListener<TDes>).Value;
        //}

       

        //public static IResultListenerFactory<Tor, TDes> BuildFunctionListenerFactory<Tor, TDes>(Expression<Func<Tor, TDes>> expression, string iAttributename) where Tor : class, IObjectBuildAttributeListener
        //{
        //    IResultListenerCompleteFactory<Tor, TDes> res = FactoryListenerBuilder<Tor, TDes>.FunctionResultListenerFactory(expression, iAttributename);
        //    res.RegisterOnValue = true;
        //    return res;
        //}

        protected override void OnObserved()
        {
            if ((!this.IsPropertyObserved) && (!this.IsObjectObserved))
            {
                _ObjectDynamicAtributes.Value.Register();
                //AttributeCollection.Register();
            }
        }

        private bool _UnderRemoveEvent = false;

        protected override void OnEndObserved()
        {
            if ((_Disposed) || (_UnderRemoveEvent) || (this.IsPropertyObserved) || (!_ObjectDynamicAtributes.IsValueCreated) || (!_ObjectDynamicAtributes.Value.IsListening))
                return;

            if (!this.IsObjectObserved)
            {
                //AttributeCollection.UnRegister();
                _ObjectDynamicAtributes.Value.UnRegister();
            }
            else
            {
                //if (this.ObjectAttributeListenerAreAll(AttributeCollection.ObservedAttribute))
                if (this.ObjectAttributeListenerAreAll(_ObjectDynamicAtributes.Value.ObservedAttribute))   
                {
                    _UnderRemoveEvent = true;
                    //AttributeCollection.UnRegister();
                    _ObjectDynamicAtributes.Value.UnRegister();
                    _UnderRemoveEvent = false;
                }
            }
        }

        private bool _Disposed = false; 

        public virtual void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            if (_ObjectDynamicAtributes.IsValueCreated)
                _ObjectDynamicAtributes.Value.Dispose();
            //if (__AttributeCollection != null)
            //{
            //    __AttributeCollection.Dispose();
            //}
        }
    }
}
