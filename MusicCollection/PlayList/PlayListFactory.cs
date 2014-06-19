using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Implementation;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Observable;

using MusicCollection.Infra;

namespace MusicCollection.PlayList
{
    internal class PlayListFactory : IPlayListFactory, IDisposable
    {

        private StrongCacheCollection<string, IReadOnlyPlayList> _PlayLists = new StrongCacheCollection<string, IReadOnlyPlayList>(pl => pl.PlayListname);

        //private LifeCycleCollection<string, AbstractPlayList> _PlayLists = new LifeCycleCollection<string, AbstractPlayList>((apl)=>(apl as IReadOnlyPlayList).PlayListname);
        //private UISafeReadOnlyObservableCollection<IReadOnlyPlayList> _VisiblePlayList;

        //private MusicSessionImpl _MSI;

        internal PlayListFactory(MusicSessionImpl MSI)
        {
            //_MSI = MSI;
        }

        public IReadOnlyPlayList GetPlayList(string FactoryName)
        {
            return _PlayLists.Find(FactoryName);
        }

        public IAlbumPlayList CreateAlbumPlayList(string PlaylistName)
        {
            return _PlayLists.FindOrCreateValue(PlaylistName, n => new FullAlbumPlayList(PlaylistName)).Item1 as IAlbumPlayList;

            //IReadOnlyPlayList res = _PlayLists.Find(PlaylistName);
            //if (res != null)
            //    return res as IAlbumPlayList;

            ////IAlbumPlayList apl = new AlbumPlayList(PlaylistName);
            //IAlbumPlayList apl = new FullAlbumPlayList(PlaylistName);
            //_PlayLists.Register(apl);
            //return apl;
       }

        //public ReadOnlyObservableCollection<IReadOnlyPlayList> AllPlayList
        //{
        //    get 
        //    {
        //        if (_VisiblePlayList==null)
        //            _VisiblePlayList = new UISafeReadOnlyObservableCollection<IReadOnlyPlayList>(_PlayList);

        //        return _VisiblePlayList;
        //    }
        //}

        public void Dispose()
        {
            if (_PlayLists != null)
            {
                _PlayLists.Values.Apply(o => o.Dispose());
                _PlayLists.Dispose();
                _PlayLists = null;
            }
        }
    }
}
