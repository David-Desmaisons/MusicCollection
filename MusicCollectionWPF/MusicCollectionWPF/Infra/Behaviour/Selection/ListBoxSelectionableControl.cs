using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class ListBoxSelectionableControl : ISelectionableControl
    {
        private  ListBox _ListBox;

        public ListBoxSelectionableControl(ListBox iListBox)
        {
            _ListBox = iListBox;
        }
        public IList SelectedItems
        {
            get { return _ListBox.SelectedItems; }
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { _ListBox.SelectionChanged += value; }
            remove { _ListBox.SelectionChanged -= value; }
        }

        public override int GetHashCode()
        {
            return _ListBox.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ListBoxSelectionableControl tar = obj as ListBoxSelectionableControl;
            if (tar == null) return false;
            return _ListBox.Equals(tar._ListBox);
        }
    }

}
