using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using System.Collections.ObjectModel;

namespace MusicCollection.WebServices
{
    internal class WebResult : IWebResult
    {
        //internal WebResult(IEnumerable<WebMatch<AlbumDescriptor>> found)
        //{
        //    //Found = new ObservableCollection<WebMatch<IFullAlbumDescriptor>>();
        //    //if (found == null)
        //    //    return;

        //    foreach (WebMatch<AlbumDescriptor> rf in found)
        //    {
        //        Found.Add(new WebMatch<IFullAlbumDescriptor>(rf.FindItem, rf.Precision, rf.WebProvider));
        //    }

        //}

        internal WebResult()
        {
        }

        private IList<WebMatch<IFullAlbumDescriptor>> _Found = new UISafeObservableCollection<WebMatch<IFullAlbumDescriptor>>();
        public IList<WebMatch<IFullAlbumDescriptor>> Found
        {
            get { return _Found; }
            //private set;
        }

        //public List<WebMatch<AlbumDescriptor>> RawFound
        //{
        //    get;
        //    private set;
        //}


    }
}
