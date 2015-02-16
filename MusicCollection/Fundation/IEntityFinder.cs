using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IEntityFinder<T> : IDisposable where T : class
    {
        IEnumerable<T> Search(string searh);

        IEnumerable<T> SearchOrdered(string searh);

        IEnumerable<T> FindSimilarMatches(string search, byte Distance = 2);

        IEnumerable<T> FindExactMatches(string search);

        IEnumerable<SimpleCouple<T>> FindPotentialMisname(byte Distance=2);

        int MinimunLengthForSearch { get; }

        event EventHandler OnUpdate;
    }

}
