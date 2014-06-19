using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.DataExchange;

//DEM changes TR

namespace MusicCollection.WebServices
{
    internal class WebQueryBase
    {
        internal WebQueryBase()
        {
        }

        //public QueryMode Mode
        //{
        //    get;
        //    set;
        //}

        public bool NeedCoverArt { get; set; }

        private int _MR = -1;
        public int MaxResult
        {
            set
            {
                if (value <= 0)
                    _MR = -1;
                else
                    _MR = value;
            }
            get { return _MR; }
        }
    }

    internal class WebQueryFactory : IWebQueryFactory
    {
        private IMusicSession _Session;

        internal WebQueryFactory(IMusicSession iSession)
            //IMusicSettings iSession)
        {
            _Session = iSession;
        }

        public IWebQuery FromAlbumDescriptor(IAlbumDescriptor iad)
        {
            return new AlbumDescriptorQuery(iad);
        }

        public IWebQuery FromCDInfo(ICDInfoHandler iad)
        {
            return new CDInfoQuery(iad);
        }
    }

    internal class AlbumDescriptorQuery : WebQueryBase, IWebQuery
    {
        internal AlbumDescriptorQuery(IAlbumDescriptor iad)
        {
            AlbumDescriptor = iad;
        }

        public IAlbumDescriptor AlbumDescriptor
        {
            get;
            private set;
        }

        public QueryType Type
        {
            get { return QueryType.FromAlbumInfo; }
        }

        public ICDInfoHandler CDInfo
        {
            get { return null; }
        }
    }

    internal class CDInfoQuery : WebQueryBase, IWebQuery
    {
        internal CDInfoQuery(ICDInfoHandler iad)
        {
            CDInfo = iad;
        }

        public ICDInfoHandler CDInfo
        {
            get;
            private set;
        }

        public IAlbumDescriptor AlbumDescriptor
        {
            get { return null; }
        }

        public QueryType Type
        {
            get { return QueryType.FromCD; }
        } 
    }
}
