using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using TagLib;

using MusicCollection.FileImporter;
using MusicCollection.TaglibExtender;
using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal class PathTrackDescriptor : ITrackDescriptor, IDisposable, IAlbumDescriptor
    {
        private string _Path;
        private TagLib.File _File;
        //private bool _FileExist = false;
        private StringTrackParser _TrackParser;
        private IImportHelper _NH;
        private bool _IsInit = false;
        private string _MD5 = null;

        internal PathTrackDescriptor(string iPath, IImportHelper NameHelper)
        {
            _Path = iPath;

            _NH = NameHelper;
        }

        string IAlbumDescriptor.Artist
        {
            get
            {
                Init();

                string FromFile = _File.Tag.FirstAlbumArtist;
                if (string.IsNullOrEmpty(FromFile))
                    FromFile = _File.Tag.FirstPerformer;

                if (string.IsNullOrEmpty(FromFile))
                {
                    FromFile = _NH.AlbumArtistClue;
                }

                return FromFile;
            }
        }

        IAlbumDescriptor ITrackDescriptor.AlbumDescriptor { get { return this; } }

        string IAlbumDescriptor.Genre
        {
            get
            {
                Init();

                return _File.Tag.FirstGenre;
            }
        }

        DiscIDs IAlbumDescriptor.IDs
        {
            get
            {
                Init();
                return DiscIDs.FromFile(_File);
            }
        }

        uint IAlbumDescriptor.TracksNumber
        {
            get
            {
                Init();

                return _File.Tag.TrackCount;
            }
        }

        string IAlbumDescriptor.Name
        {
            get
            {
                Init();

                string FromFile = _File.Tag.Album;

                if (string.IsNullOrEmpty(FromFile))
                {
                    FromFile = _NH.AlbumNameClue;
                }

                return FromFile;
            }
        }

        int IAlbumDescriptor.Year
        {
            
            get
            {
                Init();

                int FromFile = (int)_File.Tag.Year;

                if (FromFile == 0)
                {
                    FromFile = _NH.Year ?? 0;
                }

                return FromFile;
            }
        }

        private void Init()
        {
            if (_IsInit)
                return;

            if (!System.IO.File.Exists(_Path))
                throw new Exception("Not Handled");

            _File = TagLib.File.Create(_Path);
            _TrackParser = new StringTrackParser(_Path);

            _IsInit = true;
        }

        private ISRC _ISRC = null;
        private bool _ISRCIsInit = false;

        public ISRC ISRC
        {
            get
            {
                if (_ISRCIsInit)
                    return _ISRC;

                Init();
                _ISRC = _File.ISRC();
                _ISRCIsInit = true;

                return _ISRC;
            }
        }

        public string MD5
        {
            get
            {
                if (_MD5 != null)
                    return _MD5;

                Init();
                _MD5 = _File.MD5();

                return _MD5;
            }
        }

        public Stream MusicStream()
        {
            return _File.RawMusicStream();
        }




        public string Artist
        {
            get
            {
                Init();
                string FromFile = _File.Tag.FirstPerformer;


                return FromFile;
            }
        }

        public string Name
        {
            get
            {
                Init();

                string FromFile = _File.Tag.Title;
                if (string.IsNullOrEmpty(FromFile))
                {
                    FromFile = _TrackParser.TrackName;
                }

                return FromFile;
            }
        }

        public string Path
        {
            get
            {
                return _Path;
            }
        }

       



        public uint TrackNumber
        {
            get
            {
                Init();

                uint FromFile = _File.Tag.Track;
                if (FromFile == 0)
                    FromFile = _TrackParser.TrackNumber ?? 0;

                return FromFile;
            }
        }

      

       

        public TimeSpan Duration
        {
            get
            {
                Init();

                return _File.Properties.Duration;
            }
        }


        public void Dispose()
        {
            if (_File != null)
            {
                _File.Dispose();
                _File = null;
            }
        }


    }
}
