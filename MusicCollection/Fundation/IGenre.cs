using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IGenre : IMusicObject
    {
        string Name { get; }
        string FullName { get; }
        string NormalizedName { get; }
        IGenre Father { get; }
        IObservableCollection<IGenre> SubGenres { get; }
        IGenre AddSubGenre(string iName);
        IObservableCollection<IAlbum> Albums { get; }
        int Compare(IGenre other);
    }
}
