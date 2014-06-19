using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IArtist : IMusicObject
    {
        string Name 
        { 
            get; 
        }

        //string NormalizedName
        //{
        //    get;
        //}

        IObservableCollection<IAlbum> Albums
        {
            get;
        }
    }
}
