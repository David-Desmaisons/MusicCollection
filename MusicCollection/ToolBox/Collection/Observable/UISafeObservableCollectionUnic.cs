using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.ToolBox.Event;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable
{

    internal class UISafeObservableCollectionUnic<T> : UISafeObservableCollection<T>
    {
        public UISafeObservableCollectionUnic(IList<T> ienum) : base(ienum.Distinct())
        {           
        }

        public UISafeObservableCollectionUnic(IEnumerable<T> ienum) : base(ienum.Distinct())
        {
        }

        public UISafeObservableCollectionUnic() : base()
        {
        }

        protected override void InsertItem(int index, T item)
        {
            if (this.Contains(item))
                return;

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (this.Contains(item))
                return;

            base.SetItem(index, item);
        }
    }
    
}
