using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using MusicCollection.Infra;

namespace MusicCollectionWPF.Infra
{
    public class Changes
    {

        internal List<object> RemovedSelected { get; private set; }
        internal List<object> AddeddSelected { get; private set; }

        private ListBox _LBI;

        internal Changes(ListBox iLBI)
        {
            _LBI = iLBI;
            AddeddSelected = new List<object>();
            RemovedSelected = new List<object>();
        }

        internal ListBoxItem Convert(object o)
        {
            return _LBI.ItemContainerGenerator.ContainerFromItem(o) as ListBoxItem;
        }
      
        internal List<ListBoxItem> AddedSelectedListBoxItem
        {
            get { return AddeddSelected.Select(o => Convert(o)).ToList(); }
        }

        internal List<ListBoxItem> RemovedSelectedListBoxItem
        {
            get {  return RemovedSelected.Select(o => Convert(o)).ToList(); }
        }

        internal void AddSelected(object lbi)
        {
            AddeddSelected.Add(lbi);
        }

        internal void AddRemoved(object o)
        {
            RemovedSelected.Add(o);
        }
    }
}
