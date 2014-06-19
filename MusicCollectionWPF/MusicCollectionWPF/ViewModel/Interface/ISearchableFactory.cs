using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollectionWPF.ViewModel
{
    public interface ISearchableFactory: IDisposable
    {
        IList PossibilitiesFromClue(string clue);

        object CreateFromName(string clue);
    }
}
