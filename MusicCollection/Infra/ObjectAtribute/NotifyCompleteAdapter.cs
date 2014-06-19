using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Event;
using System.Runtime.CompilerServices;

namespace MusicCollection.Infra
{
    public class NotifyCompleteAdapterNoCache : INotifyPropertyChanged, IObjectAttribute
    {
        private PropertyChangedEventHandlerUISafeEvent _PropertyChanged;
        private PropertyChangedEventHandlerUISafeEvent PropertyChangedEvent
        {
            get
            {
                if (_PropertyChanged == null)
                {
                    _PropertyChanged = new PropertyChangedEventHandlerUISafeEvent(this);
                }
                return _PropertyChanged;
            }
        }

        protected NotifyCompleteAdapterNoCache()
        {          
        }

        //protected void OnPropertyHasChanged<Tat>(Tat iold, Tat iNew, [CallerMemberName] string PropertyName = null)
        //{
        //    PropertyHasChanged(PropertyName, iold, iNew);
        //}

        protected void PropertyHasChanged<Tat>(string PropertyName, Tat iold, Tat iNew)
        {
            if (object.Equals(iold, iNew))
                return;

            EventHandler<ObjectModifiedArgs> o = _ObjectChanged;
            if (o != null)
            {
                //var c = Register(PropertyName, iold, iNew);             
                //o(this, c);
                //UnRegister(c);

                o(this, new ObjectAttributeChangedArgs<Tat>(this, PropertyName, iold, iNew));
            }

            //OnChanged(PropertyName, iold);
            PropertyChangedEvent.Fire(PropertyName, true);
        }

        //protected virtual void OnChanged(string iPropertyName,object old)
        //{
        //}

        protected void PropertyHasChangedUIOnly(string PropertyName)
        {
            PropertyChangedEvent.Fire(PropertyName, true);
        }

        protected virtual void OnObserved()
        {
        }

        protected virtual void OnEndObserved()
        {
        }

        protected bool IsObjectObserved
        {
            get { return (_ObjectChanged != null); }
        }

        protected bool IsPropertyObserved
        {
            get { return ((_PropertyChanged != null) && _PropertyChanged.IsObserved); }
        }

        protected bool ObjectAttributeListenerAreAll(IEnumerable<EventHandler<ObjectModifiedArgs>> tocomp)
        {
            if (_ObjectChanged == null)
                return ((tocomp == null) || (!tocomp.Any()));

            return _ObjectChanged.GetInvocationList().Cast<EventHandler<ObjectModifiedArgs>>().SequenceComparNoOrder(tocomp);
        }

        private event EventHandler<ObjectModifiedArgs> _ObjectChanged;
        
        
        public event EventHandler<ObjectModifiedArgs> ObjectChanged
        {
            add
            {
                OnObserved();
                _ObjectChanged += value;
            }
            remove
            {
                _ObjectChanged -= value;
                OnEndObserved();
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                OnObserved();
                PropertyChangedEvent.Event += value;
            }
            remove
            {
                PropertyChangedEvent.Event -= value;
                OnEndObserved();
            }
        }

        protected bool Set<Tat>(ref Tat current, Tat iNew, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(current, iNew))
                return false;

            Tat old = current;
            current = iNew;
            PropertyHasChanged(propertyName, old, iNew);
            return true;
        }

        //public virtual ObjectModifiedArgs this[string iAttributeName]
        //{
        //    get
        //    {
        //        if (_Changings == null)
        //            return null;

        //        ObjectModifiedArgs res = null;

        //        if (_Changings.TryGetValue(iAttributeName, out res))
        //            return res;

        //        return null;
        //    }
        //}

        //private PolyMorphDictionaryGeneric<string, ObjectModifiedArgs> _Changings = null;

        //protected ObjectAttributeChangedArgs<Tat> Register<Tat>(string PropertyName, Tat Old, Tat New)
        //{
        //    //ObjectAttributeChangedArgs<Tat> eventargs = new ObjectAttributeChangedArgs<Tat>(this, PropertyName, Old, New);
        //    ////if (_Changings == null)
        //    ////{
        //    ////    _Changings = new PolyMorphDictionary<string, ObjectModifiedArgs>();
        //    ////}
        //    ////_Changings.Add(eventargs.AttributeName, eventargs);

        //    //return eventargs;

        //   return new ObjectAttributeChangedArgs<Tat>(this, PropertyName, Old, New);
        //}

        //protected void UnRegister(ObjectModifiedArgs argument)
        //{
        //    //_Changings.Remove(argument.AttributeName);
        //}
    }

    public class NotifyCompleteAdapterWithCache : NotifyCompleteAdapterNoCache, IObjectBuildAttributeListener
    {
        protected NotifyCompleteAdapterWithCache()
            : base()
        {
        }

        public void AttributeChanged<T>(string AttributeName, T oldv, T newv)
        {
            PropertyHasChanged(AttributeName, oldv, newv);
        }
    }
}
