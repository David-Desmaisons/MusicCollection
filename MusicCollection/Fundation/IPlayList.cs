using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public class SelectionChangedargs : EventArgs
    {
        internal SelectionChangedargs() { }
    }

    public interface IReadOnlyPlayList : IObjectAttribute,INotifyPropertyChanged, IDisposable
    {
        string PlayListname { get; set; }

        void Init();

        bool Transition();

        ITrack CurrentTrack
        {
            get;
            set;
        }

        bool AutoReplay
        { set; get; }

        IObservableCollection<ITrack> ReadOnlyTracks { get; }
        event EventHandler<SelectionChangedargs> SelectionChanged;
    }

    public interface IAlbumPlayList : IReadOnlyPlayList
    {
        IAlbum CurrentAlbumItem { get; set; }

        IObservableCollection<IAlbum> Albums { get; }

        void AddAlbum(IAlbum al);

        void RemoveAlbum(IAlbum al);

   }


    

}
