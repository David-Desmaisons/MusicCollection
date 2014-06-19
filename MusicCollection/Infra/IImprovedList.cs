using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    interface IImprovedList<T>:IObservableCollection<T>
    {
        void Move(int oldindex, int newindex);

        Func<T, T, int> Comparator
        {
            set;
            get;
        }

        void Sort(bool Asc = true, Func<T, T, int> iComparator = null);
    }
}
