using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

using MusicCollection.ToolBox.Event;
using MusicCollection.Infra;
using System.Diagnostics;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    [DebuggerDisplay("Count = {Count}")]
    internal class ReadOnlySimpleWraper<T> : ReadOnlyWraper<T,T> where T : class
    {
        internal ReadOnlySimpleWraper(IList<T> source):base(source)
        {
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    internal class ReadOnlyWraper<TSource, TDes> : ReadOnlyCollection<TDes>, IExtendedObservableCollection<TDes>, IInvariant where TSource : class,TDes
    {
        private IList<TSource> _Source;
        private CollectionUISafeEvent _CollectionChanged;

        internal ReadOnlyWraper(IList<TSource> source)
        {
            _Source = source;
            _CollectionChanged = new CollectionUISafeEvent(this, () => PropertyHasChanged(string.Empty));

            INotifyCollectionChanged incc = _Source as INotifyCollectionChanged;
            if (incc != null)
                incc.CollectionChanged += OnCollectionChanged;
        }

        #region Collection

        private IEnumerable<TDes> Expected
        {
            get { return _Source.Select(o=>o); }
        }

        public bool Invariant
        {
            get { return Expected.SequenceEqual(this); }
        }

        public int Count
        {
            get { return _Source.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override int IndexOf(TDes item)
        {
            return _Source.IndexOf(item as TSource);
        }

        public override TDes this[int index]
        {
            get
            {
                return _Source[index];
            }
            set
            {
                throw new AccessViolationException();
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new AccessViolationException();
            }
        }

        public override bool Contains(TDes item)
        {
            return _Source.Contains(item as TSource);
        }

        public override IEnumerator<TDes> GetEnumerator()
        {
            return Expected.GetEnumerator();
        }

        private TSource[] Copy(int Length,int arrayIndex)
        {
            TSource[] myarr = new TSource[Length];
            _Source.CopyTo(myarr, arrayIndex);
            return myarr;
        }

        public void CopyTo(TDes[] array, int arrayIndex)
        {
            TSource[] myarr = Copy(array.Length,arrayIndex);
            myarr.CopyTo(array, 0);
        }

        public void CopyTo(Array array, int index)
        {
            TSource[] myarr = Copy(array.Length, index);
            myarr.CopyTo(array, 0);
        }

        #endregion

        #region Event

        public event NotifyCollectionChangedEventHandler CollectionChanged
        { add { _CollectionChanged.Event+=value; } remove { _CollectionChanged.Event-=value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyHasChanged(string pn)
        {
            PropertyChangedEventHandler c = PropertyChanged;
            if (c != null)
                c(this, new PropertyChangedEventArgs(pn));
        }

        private void OnCollectionChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            _CollectionChanged.CollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    return;
            }

            PropertyHasChanged("Count");
        }

        //protected IDisposable GetEventFactorizable()
        //{
        //    return _CollectionChanged.GetEventFactorizable();
        //}

        #endregion

        public void Dispose()
        {
            INotifyCollectionChanged incc = _Source as INotifyCollectionChanged;
            if (incc != null)
                incc.CollectionChanged -= OnCollectionChanged;
        }

        public virtual IDisposable GetFactorizableEvent()
        {
            return _CollectionChanged.GetEventFactorizable();
            //return null;
        }
    }
}
