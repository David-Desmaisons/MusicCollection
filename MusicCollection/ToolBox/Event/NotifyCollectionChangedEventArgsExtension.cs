using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox.Event
{
    public static class NotifyCollectionChangedEventArgsExtension
    {
        public static IEnumerable<NotifyCollectionChangedEventArgs> Decompose(this NotifyCollectionChangedEventArgs @this)
        {
            switch(@this.Action)
            {
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    yield return @this;
                    break;

                case NotifyCollectionChangedAction.Add:
                    if (@this.NewItems.Count==1)
                    {
                        yield return @this;
                    }
                    else
                    {
                        int index = @this.NewStartingIndex;
                        foreach(var item in @this.NewItems)
                        {
                            yield return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index++);
                        }
                    }
                    break;

            }
        }
    }
}
