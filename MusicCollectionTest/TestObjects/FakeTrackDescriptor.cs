using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollectionTest.TestObjects
{
    public class FakeTrackDescriptor : ITrackDescriptor
    {
        private string _P;
        public FakeTrackDescriptor(string path)
        {
            _P = path;
        }


        public string Path
        {
            get { return _P; }
        }

        public string MD5
        {
            get { throw new NotImplementedException(); }
        }

        public System.IO.Stream MusicStream()
        {
            throw new NotImplementedException();
        }

        public IAlbumDescriptor AlbumDescriptor
        {
            get { throw new NotImplementedException(); }
        }

        public string Artist
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        //public ISRC ISRC
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public uint TrackNumber
        {
            get { throw new NotImplementedException(); }
        }

        public uint DiscNumber
        {
            get { throw new NotImplementedException(); }
        }

        public TimeSpan Duration
        {
            get { throw new NotImplementedException(); }
        }
    }
}
