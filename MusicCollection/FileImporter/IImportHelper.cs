using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SevenZip;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.DataExchange;

namespace MusicCollection.FileImporter
{
    internal interface IDiscInfoCue
    {
        string AlbumNameClue { get; }
        string AlbumArtistClue { get; }
    }

    internal class DiscInfoCue : IDiscInfoCue
    {
        public string AlbumNameClue
        {
            get; private set;
        }

        public string AlbumArtistClue
        {
            get;
            private set;
        }

        internal DiscInfoCue(string Name, string Ar)
        {
            AlbumNameClue =Name;
            AlbumArtistClue = Ar;
        }
    }


    internal interface IImportHelper : IDiscInfoCue
    {
        string DisplayName { get; }

        int? Year { get; }
    }


    internal class ICDInfosHelperAdpt : IImportHelper
    {
        private IFullAlbumDescriptor _ICI;

        internal ICDInfosHelperAdpt(IFullAlbumDescriptor ICI)
        {
            _ICI = ICI;
        }

        string IImportHelper.DisplayName { get { return _ICI.Name; } }
        string IDiscInfoCue.AlbumNameClue { get { return _ICI.Name; } }
        string IDiscInfoCue.AlbumArtistClue { get { return _ICI.Artist; } }
        int? IImportHelper.Year { get { return _ICI.Year; } }

        public override string ToString()
        {
            return string.Format("{0}-{1},{2}", _ICI.Name, _ICI.Artist, _ICI.Year);
        }
    }


    internal class FolderImporterHelper : IImportHelper
    {
        private string _Folder;
        private string _DP;
        private string _Artist;
        private string _Album;
        private int? _Year = null;

        internal FolderImporterHelper(string FolderName)
        {
            _Folder = FolderName;

            if (string.IsNullOrEmpty(_Folder))
            {
                _Album = string.Empty;
                _Artist = string.Empty;
                _Year =null;
                return;
            }

            _DP = Path.GetFileName(_Folder);
            StringAlbumParser sa = StringAlbumParser.FromString(_DP);

            if (!sa.FounSomething)
            {
                DirectoryHelper RootName = new DirectoryHelper(_Folder);
                sa = StringAlbumParser.FromDirectoryHelper(RootName);
            }

            _Album = sa.AlbumName;
            _Artist = sa.AlbumAuthor;
            _Year = sa.AlbumYear;
        }


        string IImportHelper.DisplayName { get { return _DP; } }
        string IDiscInfoCue.AlbumNameClue { get { return _Album; } }
        string IDiscInfoCue.AlbumArtistClue { get { return _Artist; } }
        //t string IImportHelper.RarHash { get { return null; } }
        int? IImportHelper.Year { get { return _Year; } }

        public override string ToString()
        {
            return string.Format("{3}:{0}-{1},{2}", _Album, _Artist, _Year, _DP);
        }

    }




}
