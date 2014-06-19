using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MusicCollection.Fundation
{
    public interface IPlayListFactory
    {
        IReadOnlyPlayList GetPlayList(string FactoryName);

        IAlbumPlayList CreateAlbumPlayList(string FactoryName);
        //, bool Persitance);

        //ReadOnlyObservableCollection<IReadOnlyPlayList> AllPlayList { get; }

    }
}
