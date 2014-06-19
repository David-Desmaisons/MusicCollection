using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollectionWPF.Infra
{
    public interface IEditListener
    {
        void EditEntity(IEnumerable<IMusicObject> al);

        void Remove(IEnumerable<IMusicObject> al);

        void CancelEdit();

        void EndEdit();
    }
}
