using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SevenZip;


namespace MusicCollection.ToolBox
{

    internal class DirectoryHelper
    {
        private string _Path;
        private List<string> _Dirs = new List<string>();

        internal bool ContainsSubFolder
        {
            get;
            set;
        }

        internal DirectoryHelper(string iPath)
        {
            _Path = iPath;
            ContainsSubFolder = false;

            string CurrPath = _Path;
            string Father = System.IO.Path.GetDirectoryName(_Path);

            while (!string.IsNullOrEmpty(Father))
            {
                _Dirs.Insert(0, System.IO.Path.GetFileName(CurrPath));
                CurrPath = Father;
                Father = System.IO.Path.GetDirectoryName(Father);
            }

            _Dirs.Insert(0, CurrPath);


        }

        internal string ConvertFileName(string iName)
        {
            //string Name = iName.RemoveInvalidCharacters();

            if (string.IsNullOrEmpty(iName))
                return iName;

            int RootLength = Path.Length;

            if (iName.Length < RootLength)
            {
                if (!Path.StartsWith(iName))
                    throw new Exception("Algo Error");

                return null;
            }

            if (iName.Length == RootLength)
            {
                if (iName != Path)
                    throw new Exception("Algo Error");

                return null;
            }

            if (!iName.StartsWith(Path))
                throw new Exception("Algo Error");

            return iName.Substring(RootLength + 1).FormatExistingRelativeDirectoryName();//tirons le \
        }

        private DirectoryHelper(List<string> Source,int Rank)
        {
            if (Rank > Source.Count)
                throw new Exception("Algo Error");

            for (int i = 0; i < Rank; i++)
            {
                _Dirs.Add(Source[i]);
            }

            _Path = System.IO.Path.Combine(_Dirs.ToArray());
        }

        internal string Path
        {
            get
            {
                return _Path;
            }
        }

        internal string this[int Rank]
        {
            get
            {
                return _Dirs[Rank];
            }
        }

        internal int Count
        {
            get
            {
                return _Dirs.Count;
            }
        }

        internal DirectoryHelper GetCommunRoot(DirectoryHelper Help2)
        {
            if (Help2==null)
                return null;

            int CommunCount = 0;

            int To = Math.Min(Count,Help2.Count);

            for (int i=0; i < To; i++)
            {
                if (this[i] != Help2[i])
                    break;

                CommunCount++;
            }

            return ( CommunCount == 0) ? null : new DirectoryHelper(_Dirs,CommunCount);
        }


    }




    static internal class SevenZipExtractorExtender
    {
        static internal IEnumerable<string> SafeArchiveFileNames(this SevenZipExtractor sze)
        {
            return from afn in sze.ArchiveFileNames select afn.FormatExistingRelativeDirectoryName();
        }

        static internal string SafePath(this ArchiveFileInfo afi)
        {
            return afi.FileName.FormatExistingRelativeDirectoryName();
        }

        static internal DirectoryHelper GetRootDir(this SevenZipExtractor sze)
        {
            DirectoryHelper RootName = null;

            foreach (ArchiveFileInfo path in sze.ArchiveFileData)
            {
  
                if (path.IsDirectory)
                    continue;

                string CurRoot = Path.GetDirectoryName(path.SafePath());

                if (string.IsNullOrEmpty(CurRoot))
                    return null;

                if (RootName == null)
                {
                    RootName = new DirectoryHelper(CurRoot);
                    continue;
                }

                if (RootName.Path == CurRoot)
                    continue;

                //Ici c'est la partie compliquee on va chercher le plus grand root commun au deux directory
                RootName = RootName.GetCommunRoot(new DirectoryHelper(CurRoot));

                if (RootName == null)
                    return null;

                RootName.ContainsSubFolder = true;
            }

            return RootName;

        }


    }
}
