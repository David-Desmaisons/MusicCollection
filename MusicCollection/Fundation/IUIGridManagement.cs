using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MusicCollection.Fundation
{


    public interface IPersistGrid
    {
        //PersistentColumns ColumnsByName(string Name);

        void PersistChange(IList<DataGridColumn> cols);

        void FromPersistance(IList<DataGridColumn> cols);


    }

    public interface IUIGridManagement
    {
        //IPersistGrid GetFromName(string GriName);

        IPersistGrid Default { get; }   
    }
}
