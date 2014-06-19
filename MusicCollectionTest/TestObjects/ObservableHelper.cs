using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using NUnit.Framework;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollectionTest.TestObjects
{
    internal abstract class EventSendingTester<T> where T : EventArgs
    {
        private EventListener<T> _CCL;
        protected T _nccea;
        protected int _Count = 0;

        internal protected EventListener<T> Listener
        {
            get { return _CCL; }
        }

        internal protected T Event
        {
            get { return _nccea; }
        }

        protected virtual void Init()
        {
            _CCL = new EventListener<T>();
            _Count = 0;
            _nccea = null;
        }

        private class Changes : IDisposable
        {
            private EventSendingTester<T> _F;

            internal Changes(EventSendingTester<T> F)
            {
                _F = F;
            }

            public void Dispose()
            { 
                _F.DisplayInformation();
                _F.UpdateCountAndEvent();
               
            }
        }

        internal IDisposable Transaction()
        {
            return new Changes(this);
        }

        protected virtual void UpdateCountAndEvent()
        {
            _Count = _CCL.EventCount;
            _nccea = _CCL.GetDeplieEvent();
        }

        abstract protected void DisplayInformation();

        protected void AssertNonEvent()
        {
            Assert.That(_nccea, Is.Null);
            Assert.That(_Count, Is.EqualTo(0));
        }

        protected void AssertEvent(int EventNumber=1)
        {
            Assert.That(_nccea, Is.Not.Null);
            Assert.That(_Count, Is.EqualTo(EventNumber));
        }

        protected void AssertEvent(T Event)
        {
            Assert.That(_nccea, Is.EqualTo(Event));
            Assert.That(_Count, Is.EqualTo(1));
        }
    }

    internal class ModifiableObjectEventTester : EventSendingTester<ObjectModifiedArgs>
    {
        internal void AssertEvent<T>(string Name, T old, T nnew)
        {
            ObjectAttributeChangedArgs<T> evente = Event as ObjectAttributeChangedArgs<T>;
            Assert.That(evente, Is.Not.Null);

            Assert.That(evente.AttributeName, Is.EqualTo(Name));
            Assert.That(evente.Old, Is.EqualTo(old));
            Assert.That(evente.New, Is.EqualTo(nnew));
        }

        protected override void DisplayInformation()
        {
        }
    }

    internal class ObservableHelper : EventSendingTester<NotifyCollectionChangedEventArgs> 
    {
        protected void AssertAddEvent(object item,Nullable<int> Index=null)
        {
            AssertEvent();
            Assert.That(_nccea.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            Assert.That(_nccea.NewItems[0], Is.EqualTo(item));
            if (Index!=null)
                Assert.That(_nccea.NewStartingIndex, Is.EqualTo((int)Index));
        }

        protected void AssertRemoveEvent(object item, Nullable<int> Index = null)
        {
            AssertEvent();
            Assert.That(_nccea.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
            Assert.That(_nccea.OldItems[0], Is.EqualTo(item));
            if (Index != null)
                Assert.That(_nccea.OldStartingIndex, Is.EqualTo((int)Index));
        }

        protected void AssertEvent(NotifyCollectionChangedAction et, int EventNumber = 1)
        {
            Assert.That(_nccea, Is.Not.Null);
            Assert.That(_nccea.Action, Is.EqualTo(et));
            Assert.That(_Count, Is.EqualTo(EventNumber));
        }

        protected void AssertResetEvent()
        {
            AssertEvent();
            Assert.That(_nccea.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
        }

        protected void AssertMoveEvent( Nullable<int> Indexold,Nullable<int> Indexoldnew, object item=null)
        {
            AssertEvent();
            Assert.That(_nccea.Action, Is.EqualTo(NotifyCollectionChangedAction.Move));
            if (Indexold != null)
                Assert.That(_nccea.OldStartingIndex, Is.EqualTo((int)Indexold));
            if (Indexoldnew != null)
                Assert.That(_nccea.NewStartingIndex, Is.EqualTo((int)Indexoldnew));

            if (item != null)
                Assert.That(_nccea.NewItems[0], Is.EqualTo(item));
        }

        protected void AssertReplaceEvent(Nullable<int> Index = null, object olditem = null, object newitem = null)
        {
            AssertEvent();
            Assert.That(_nccea.Action, Is.EqualTo(NotifyCollectionChangedAction.Replace));

            if (Index != null)
                Assert.That(_nccea.NewStartingIndex, Is.EqualTo((int)Index));

            if (olditem != null)
                Assert.That(_nccea.OldItems[0], Is.EqualTo(olditem));

            if (newitem != null)
                Assert.That(_nccea.NewItems[0], Is.EqualTo(newitem));
        }

        private IInvariant _Invariant;
        internal void InitializeAndRegisterCollection(INotifyCollectionChanged coll)
        {
            Init();
            coll.CollectionChanged += Listener.SingleElementChangedListener;
            _Invariant = coll as IInvariant;
        }

        protected void AssertCollectionIs<T>(IEnumerable<T> Un, IEnumerable<T> Deux,bool OrderisKey=true)
        {
            if (OrderisKey)
                Assert.That(Un.SequenceEqual(Deux), Is.True);
            else
                Assert.That(Un.SequenceCompareWithoutOrder(Deux), Is.True);
        }

        protected override void UpdateCountAndEvent()
        {
            base.UpdateCountAndEvent();
            if (_Invariant != null)
            {
                Assert.That(_Invariant.Invariant, Is.True);
            }
        }

        protected override void DisplayInformation()
        {
        }
    }



    internal abstract class LiveObservableHelper<TOr, TDes> : ObservableHelper
    {
        protected ObservableCollection<TOr> _Collection = null;
        protected IInvariant _LD;

        protected IObservableCollection<TDes> _Treated = null;

        protected override void DisplayInformation()
        {
            Console.WriteLine();
            Console.WriteLine("Original Collection: {0}", string.Join(",", _Collection));
            Console.WriteLine("Distinct Collection: {0}", string.Join(",", _Treated));
            Console.WriteLine();
        } 
        
        protected override void UpdateCountAndEvent()
        {
            base.UpdateCountAndEvent();

            Console.WriteLine("Event Count {0}", this._Count);

            if (_LD != null)
                Assert.That(_LD.Invariant, Is.True);
        }

    }


    internal abstract class LiveObservableHelperResult<TOr, TDes> : EventSendingTester<ObjectModifiedArgs>
    {
        protected ObservableCollection<TOr> _Collection = null;
        protected IInvariant _LD;

        protected TDes _ResultExpected;
        protected Func<IEnumerable<TOr>, TDes> _Evaluator;
        protected ILiveResult<TDes> _Computed;

        protected override void Init()
        {
            base.Init();
            _Computed.ObjectChanged += this.Listener.SingleElementChangedListener;

        }

        protected override void DisplayInformation()
        {
            Console.WriteLine();
            Console.WriteLine("Original Collection: {0}", string.Join(",", _Collection));
            Console.WriteLine("Computed Result: {0}", ComputedValue);
            Console.WriteLine();
        }

        protected TDes ComputedValue
        {
            get { return _Computed.Value; }
        }

        protected void AssertChangeEvent(TDes oldv, TDes newv)
        {
            this.AssertEvent(new ObjectAttributeChangedArgs<TDes>(_Computed, "Value", oldv, newv));
        }

                   
        protected override void UpdateCountAndEvent()
        {
            base.UpdateCountAndEvent();

            Console.WriteLine("Event Count {0}", this._Count);

            _ResultExpected = _Evaluator(_Collection);

            Assert.That(_Computed.Value, Is.EqualTo(_ResultExpected));

            if (_LD != null)
                Assert.That(_LD.Invariant, Is.True);       
        }

    }
}
