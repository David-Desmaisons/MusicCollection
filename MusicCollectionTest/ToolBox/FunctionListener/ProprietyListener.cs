using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra;
using MusicCollection.ToolBox;

namespace MusicCollectionTest.ToolBox.FunctionListener
{
    public class ProprietyListener : NotifyCompleteAdapterWithCache, IDisposable
    {
        private Lazy<IDictionary<string, IPropertyFunction>> _MyDynamicFuntion =
            new Lazy<IDictionary<string, IPropertyFunction>>(()=> new Dictionary<string,IPropertyFunction>());

        protected T RegisterDinamic<T>(Expression<Func<T>> Function, [CallerMemberName] string propertyName = null)
        {
            CollectionResult<IPropertyFunction> rawres = _MyDynamicFuntion.Value.FindOrCreate(propertyName, (s) => Function.CompileToObservable(s));
            if ((_Observing) && (rawres.CollectionStatus == CollectionStatus.Create))
            {
                rawres.Item.ObjectChanged += Property_ObjectChanged;
            }

            IPropertyFunction<T> res = rawres.Item as IPropertyFunction<T>;
            return res.Value;
        }

        private void RegisterAll()
        {
            _Observing = true;

            if (!_MyDynamicFuntion.IsValueCreated)
                return;

            _MyDynamicFuntion.Value.Values.Apply(l => { if (!l.IsObserved) l.ObjectChanged += Property_ObjectChanged; });
        }

        private void UnRegisterAll()
        {
            _UnderRemoveEvent = true;
            _Observing = false;

            if (!_MyDynamicFuntion.IsValueCreated)
                return;

            _MyDynamicFuntion.Value.Values.Apply(l => { if (l.IsObserved) l.ObjectChanged -= Property_ObjectChanged; });
            _UnderRemoveEvent = false;
        }

        private void Property_ObjectChanged(object sender, ObjectModifiedArgs e)
        {
            IPropertyFunction ipf = sender as IPropertyFunction;
            PropertyHasChanged(ipf.PropertyName, e.OldAttributeValue, e.NewAttributeValue);
        }

        public virtual void Dispose()
        {
            if (_MyDynamicFuntion.IsValueCreated)
            {
                UnRegisterAll();
                _MyDynamicFuntion.Value.Values.Apply(l => l.Dispose());
                _MyDynamicFuntion.Value.Clear();
            }
        }

        private bool _Observing = false;

        protected override void OnObserved()
        {
            if ((!this.IsPropertyObserved) && (!this.IsObjectObserved))
            {
                RegisterAll();
            }
        }

        private bool _UnderRemoveEvent = false;

        protected override void OnEndObserved()
        {
            if ((_UnderRemoveEvent) || (this.IsPropertyObserved))
                return;

            if (!this.IsObjectObserved)
            {
                UnRegisterAll();
            }
            else
            {
                if ((_MyDynamicFuntion.IsValueCreated && 
                    this.ObjectAttributeListenerAreAll(_MyDynamicFuntion.Value.Values.Select(s=>s.ObjectListener))))  
                {
                    UnRegisterAll();
                }
            }
        }
    }
}
