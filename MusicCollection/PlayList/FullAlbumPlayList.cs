using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.PlayList
{
    internal class FullAlbumPlayList : PlayListBase, IAlbumPlayList, IDisposable
    {
        private UISafeObservableCollectionUnic<IAlbum> _Albums = new UISafeObservableCollectionUnic<IAlbum>();

        internal FullAlbumPlayList(string Name)
            : base(Name)
        {
            _ObservedAlbums = _Albums.LiveWhere(a => a.IsAlive);
            _ReadOnlyObservableTracks = _ObservedAlbums.LiveSelectMany(al => al.Tracks);
            ReadOnlyTracks = _ReadOnlyObservableTracks;
        }

        public override void Dispose()
        {
            _ObservedAlbums.Dispose();
            _ReadOnlyObservableTracks.Dispose();
            base.Dispose();
        }

        public new IAlbum CurrentAlbumItem
        {
            get { return base.CurrentAlbumItem; }
            set
            {
                if (!(Albums.Contains(value)))
                    return;

                var old = CurrentAlbumItem;
                if (old == value)
                    return;

                CurrentTrack = value.Tracks[0];
            }
        }

        public void AddAlbum(IAlbum al)
        {
            _Albums.Add(al);
        }

        public void RemoveAlbum(IAlbum al)
        {
            _Albums.Remove(al);
        }

        //public int AlbumIndex(IAlbum al)
        //{
        //    return _Albums.IndexOf(al);
        //}

        private IExtendedObservableCollection<IAlbum> _ObservedAlbums;
        public IObservableCollection<IAlbum> Albums
        {
            get { return _ObservedAlbums; }
        }

        private IExtendedObservableCollection<ITrack> _ReadOnlyObservableTracks;
   
    }
}
