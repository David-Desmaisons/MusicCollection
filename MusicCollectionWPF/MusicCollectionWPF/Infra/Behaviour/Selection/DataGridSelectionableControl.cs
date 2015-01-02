using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class DataGridSelectionableControl : ISelectionableControl
    {
        private DataGrid _DataGrid;

        public DataGridSelectionableControl(DataGrid iDataGrid)
        {
            _DataGrid = iDataGrid;
        }
        public IList SelectedItems
        {
            get { return _DataGrid.SelectedItems; }
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { _DataGrid.SelectionChanged += value; }
            remove { _DataGrid.SelectionChanged -= value; }
        }

        public override int GetHashCode()
        {
            return _DataGrid.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DataGridSelectionableControl tar = obj as DataGridSelectionableControl;
            if (tar == null) return false;
            return _DataGrid.Equals(tar._DataGrid);
        }
    }
}
