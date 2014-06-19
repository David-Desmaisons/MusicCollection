using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using TagLib;

using MusicCollection.TaglibExtender;
using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal abstract class TrackDescriptorAdapterAncester
    {
        private string _MD5 = null;
        private ISRC _ISRC = null;
        private bool _ISRCdone = false;

        public abstract string PathAcessor { get; }

        private TagLib.File File
        {
            get
            {
                TagLib.File file = null;
                try
                {
                    file = TagLib.File.Create(PathAcessor);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return file;
            }
        }


        public string MD5
        {
            get
            {
                if (_MD5 != null)
                    return _MD5;

                using (TagLib.File f =File)
                {
                    _MD5 = f.MD5();
                }

                return _MD5;
            }
        }

        public virtual ISRC ISRC
        {
            get
            {
                if (!_ISRCdone)
                {
                    using (TagLib.File f = File)
                    {
                        _ISRC = f.ISRC();
                    }
                    _ISRCdone = true;
                }

                return _ISRC;
            }

        }

        public Stream MusicStream()
        {
            using (TagLib.File f = File)
            {
                return f.RawMusicStream();
            }
        }


    }


    internal abstract class TrackDescriptorAdapter : TrackDescriptorAdapterAncester
    {


        public abstract string Path { get; }

        public override string PathAcessor { get { return Path; } }

    }
}
