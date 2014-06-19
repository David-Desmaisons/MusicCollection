using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;

namespace MusicCollection.DataExchange
{
    internal static class IAlbumDescriptorExt
    {
        internal static string DisplayName(this IAlbumDescriptor iad)
        {
            if (iad == null)
                return null;

            return string.Format("{0} - {1}", iad.Name, iad.Artist);
        }
    }


    internal class TrackDescriptorDecorator : ITrackDescriptor
    {
        private ITrackDescriptor _TD;
        private IImportContext _Session;
        private AlbumDescriptorDecorator _Al;


        internal TrackDescriptorDecorator(ITrackDescriptor itd, IImportContext ims)
        {
            _TD = itd;
            _Session = ims;
            _Al = new AlbumDescriptorDecorator(_TD.AlbumDescriptor, ims);
        }


        internal AlbumDescriptorDecorator AlbumInfo
        {
            get
            {
                return _Al;
            }
        }

        public uint DiscNumber { get { return _TD.DiscNumber; } }

        public IImportContext ImportContext
        {
            get { return _Session; }
        }


        public IAlbumDescriptor AlbumDescriptor
        {
            get { return _Al; }
        }

        public string Artist
        {
            get { return _TD.Artist; }
        }

        public string Name
        {
            get { return _TD.Name; }
        }

        public string Path
        {
            get { return _TD.Path; }
        }

        public string MD5
        {
            get { return _TD.MD5; }
        }

        public Stream MusicStream()
        {
            return _TD.MusicStream();
        }

        //public ISRC ISRC
        //{
        //    get { return _TD.ISRC; }
        //}

        public uint TrackNumber
        {
            get { return _TD.TrackNumber; }
        }

        public TimeSpan Duration
        {
            get { return _TD.Duration; }
        }
    }

    internal class AlbumDescriptorDecorator : IAlbumDescriptor
    {
        private IAlbumDescriptor _AD;
        private IImportContext _Session;

        internal AlbumDescriptorDecorator(IAlbumDescriptor itd, IImportContext Context)
        {
            _AD = itd;
            _Session = Context;
        }

        public AlbumMaturity Maturity
        {
            get { return _Session.DefaultMaturity; }
        }

        public IAlbumDescriptor Wrapped
        {
            get { return _AD; }
        }

        public IImportContext ImportContext
        {
            get { return _Session; }
        }

        public string Artist
        {
            get { return _AD.Artist; }
        }

        public string Genre
        {
            get { return _AD.Genre; }
        }

        public Genre MainGenre
        {
            get { return _Session.GetGenreFromName(this.Genre, false); }
        }

        public IEnumerable<Artist> Artists
        {
            get { return _Session.GetArtistFromName(Artist); }
        }

        public string CorrectName
        {
            get { return _AD.Name.NormalizeSpace(); }
        }

        public IDiscIDs IDs
        {
            get { return _AD.IDs; }
        }

        public uint TracksNumber
        {
            get { return _AD.TracksNumber; }
        }

        public string Name
        {
            get { return _AD.Name; }
        }

        public int Year
        {
            get { return _AD.Year; }
        }
    }
}
