using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using TagLib;

using MusicCollection.FileImporter;
using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using MusicCollection.Implementation;
using MusicCollection.Utilies;

namespace MusicCollection.DataExchange
{
    internal class PathTrackDescriptor : ITrackDescriptor, IDisposable, IAlbumDescriptor
    {
        private string _Path;
        private TagLib.File _File;
        private StringTrackParser _TrackParser;
        private IImportHelper _NH;
        private bool _IsInit = false;
        private string _MD5 = null;
        private IImportContext _Msi = null;

        internal PathTrackDescriptor(string iPath, IImportHelper NameHelper, IImportContext msi)
        {
            _Path = iPath;

            _NH = NameHelper;
            _Msi = msi;
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

        IAlbumDescriptor ITrackMetaDataDescriptor.AlbumDescriptor { get { return this; } }

        string IAlbumDescriptor.Genre
        {
            get
            {
                Init();

                return _File.Tag.FirstGenre;
            }
        }

        IDiscIDs IAlbumDescriptor.IDs
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

        public uint DiscNumber
        {
            get 
            {
                Init();

                return _File.Tag.Disc;
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
