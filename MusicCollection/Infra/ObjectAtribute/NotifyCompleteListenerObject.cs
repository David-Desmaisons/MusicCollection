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

        private Lazy<List<IDisposable>> _Disposables = new Lazy<List<IDisposable>>();

        protected T Register<T>(T idependency) where T : IDisposable
        {
            _Disposables.Value.Add(idependency);
            return idependency;
        }

        protected NotifyCompleteListenerObject()
        {
        }

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

                            return facres.CreateRawListener(this);
                        });

            return (res as IResultListener<TDes>).Value;
        }

        protected override void OnObserved()
        {
            if ((!this.IsPropertyObserved) && (!this.IsObjectObserved))
            {
                _ObjectDynamicAtributes.Value.Register();
            }
        }

        private bool _UnderRemoveEvent = false;

        protected override void OnEndObserved()
        {
            if ((_Disposed) || (_UnderRemoveEvent) || (this.IsPropertyObserved) || (!_ObjectDynamicAtributes.IsValueCreated) || (!_ObjectDynamicAtributes.Value.IsListening))
                return;

            if (!this.IsObjectObserved)
            {
                _ObjectDynamicAtributes.Value.UnRegister();
            }
            else
            {
                if (this.ObjectAttributeListenerAreAll(_ObjectDynamicAtributes.Value.ObservedAttribute))   
                {
                    _UnderRemoveEvent = true;
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
        //}
        //public override void Dispose()
        //{
        //    base.Dispose();
            if (_Disposables.IsValueCreated)
            {
                _Disposables.Value.Apply(t => t.Dispose());
                _Disposables.Value.Clear();
            }
        }
    }
}
