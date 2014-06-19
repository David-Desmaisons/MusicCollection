using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SevenZip;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;

namespace MusicCollection.FileImporter
{
    internal class RarImporterHelper : IImportHelper
    {
        private string _DP;
        private string _Album;
        private string _Artist;
        private DirectoryHelper _RootDir;
        private int? _Year = null;
        private int _RootLength = 0;
        private int _MaxLengthWithoutRoot;
        private int _MaxLengthFlat;
        private int _MaxLengthBasic;

        internal RarImporterHelper(SevenZipExtractor Sex)
        {
            //_Sex = Sex;
            string FileName = Sex.FileName;
            _DP = Path.GetFileName(FileName);

            StringAlbumParser sa = null;
            _RootDir = Sex.GetRootDir();

            if (_RootDir != null)
            {
                sa = StringAlbumParser.FromDirectoryHelper(_RootDir);
                _RootLength = _RootDir.Path.Length;
            }

            if ((sa == null) || (sa.FounSomething == false))
                sa = StringAlbumParser.FromRarZipName(FileName);

            _Album = sa.AlbumName;
            _Artist = sa.AlbumAuthor;
            _Year = sa.AlbumYear;

            _MaxLengthWithoutRoot = (from path in Sex.SafeArchiveFileNames() let npath = ConvertFileName(path) let len = (npath == null) ? 0 : npath.Length select len).Max();
            _MaxLengthFlat = (from path in Sex.ArchiveFileData let npath = Path.GetFileName(path.SafePath()) where (!path.IsDirectory) select npath.Length).Max();
            _MaxLengthBasic = (from path in Sex.SafeArchiveFileNames() select path.Length).Max();

            if (_MaxLengthFlat > _MaxLengthWithoutRoot)
                throw new Exception("Algo Error");

            if (_MaxLengthWithoutRoot > _MaxLengthBasic)
                throw new Exception("Algo Error");
        }

        public override string ToString()
        {
            return string.Format("{3}:{0}-{1},{2}", _Album, _Artist, _Year, _DP);
        }


        string IImportHelper.DisplayName { get { return _DP; } }
        string IDiscInfoCue.AlbumNameClue { get { return _Album; } }
        string IDiscInfoCue.AlbumArtistClue { get { return _Artist; } }
      

        internal int MaxLengthWithoutRoot
        {
            get
            {
                return _MaxLengthWithoutRoot;
            }
        }

        internal int MaxLengthFlat
        {
            get
            {
                return _MaxLengthFlat;
            }
        }

        internal int MaxLengthBasic
        {
            get
            {
                return _MaxLengthBasic;
            }
        }

        internal bool RootContainsSubFolder()
        {
            if (_RootDir == null)
                return true;

            return _RootDir.ContainsSubFolder;
        }
        

        internal string ConvertFileName(string Name)
        {
            //if (!FileInternalToolBox.HasValidExtension(Name))
            //    return null;

            string nname = Name.FormatExistingRelativeDirectoryName();

            if (_RootDir == null)
                return nname;
                //return Path.GetFileName(nname).RemoveInvalidCharacters();

            return _RootDir.ConvertFileName(nname);
        }

        int? IImportHelper.Year { get { return _Year; } }
    }
}
