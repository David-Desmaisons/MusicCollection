using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public interface ISelectionableControl
    {
        IList SelectedItems { get; }

        event SelectionChangedEventHandler SelectionChanged;
    }
}
