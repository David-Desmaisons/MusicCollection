using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class SelectionableControlFactory
    {
        public static ISelectionableControl Get(object o)
        {
            var lb = o as ListBox;
            if (lb != null)
                return new ListBoxSelectionableControl(lb);

            var dt = o as DataGrid;
            if (dt != null)
                return new DataGridSelectionableControl(dt);

            return null;
        }
    }
}
