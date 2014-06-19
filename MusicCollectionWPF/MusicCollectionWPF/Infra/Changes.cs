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
        internal List<ListBoxItem> AddedSelectedListBoxItem { get; private set; }
        internal List<ListBoxItem> RemovedSelectedListBoxItem
        {
            get
            {
                return RemovedSelected.Select(o => _LBI.ItemContainerGenerator.ContainerFromItem(o) as ListBoxItem).ToList();
            }
        }

        internal List<object> RemovedSelected { get; private set; }
        internal List<object> AddeddSelected
        {
            get
            {
                return AddedSelectedListBoxItem.Select(o => o.DataContext).ToList();
            }
        }

        private ListBox _LBI;

        internal Changes(ListBox iLBI)
        {
            _LBI = iLBI;
            AddedSelectedListBoxItem = new List<ListBoxItem>();
            RemovedSelected = new List<object>();
        }

        internal void AddSelected(ListBoxItem lbi)
        {
            AddedSelectedListBoxItem.Add(lbi);
        }

        internal void AddRemoved(ListBoxItem o)
        {
            RemovedSelected.Add(o.DataContext);
        }

        internal void AddRemoved(object o)
        {
            RemovedSelected.Add(o);
        }

        public void ApplyChanges()
        {
            AddedSelectedListBoxItem.Apply(a => a.IsSelected = true);
            RemovedSelected.Apply(r => _LBI.SelectedItems.Remove(r));
        }

    }
}
