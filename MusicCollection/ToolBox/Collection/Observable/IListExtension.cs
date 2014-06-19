using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  System.Collections.Specialized;
using System.ComponentModel;

namespace MusicCollection.ToolBox.Collection.Observable
{
    static internal class IListExtension
    {

        
       
        static internal Nullable<int> ApplyChanges<T>(this IList<T> list, NotifyCollectionChangedEventArgs nce, int Delta=0)
        {
            if (list==null)
                return null;

            if (nce==null)
                return null;

            int IndexNew = nce.NewStartingIndex + Delta;
            int IndexOld = nce.OldStartingIndex + Delta;

            switch (nce.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    return null;

                case NotifyCollectionChangedAction.Add:                   
                    foreach (T elem in nce.NewItems)
                    {
                        list.Insert(IndexNew++, elem);
                    }
                    return nce.NewItems.Count;

                case NotifyCollectionChangedAction.Move:
                    foreach (T elem in nce.NewItems)
                    {
                        list.RemoveAt(IndexOld++);
                        list.Insert(IndexNew++, elem);
                    }
                    return 0;

                case NotifyCollectionChangedAction.Remove:
                    foreach (T elem in nce.OldItems)
                    {
                        list.RemoveAt(IndexOld++);
                    }
                    return -nce.OldItems.Count;

                case NotifyCollectionChangedAction.Replace:
                    foreach (T elem in nce.NewItems)
                    {
                        list[IndexNew++] = elem;
                    }
                    return 0;
            }

            return null;

        }
    }
}
