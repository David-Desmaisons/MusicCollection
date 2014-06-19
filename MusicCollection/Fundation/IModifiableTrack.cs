using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IModifiableTrack : INotifyPropertyChanged 
    {
        IModifiableAlbum Father
        {
            get;
        }

        bool IsModified
        {
            get;
        }

        string Artist
        {
            set;
            get;
        }

        string Name
        {
            set;
            get;
        }

        uint TrackNumber
        {
            set;
            get;
        }

        uint DiscNumber
        {
            set;
            get;
        }

        

        uint Rating
        {
            set;
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        ObjectState State
        {
            get;
        }

        string Path
        {
            get;
        }

        void Delete();

    }
}
