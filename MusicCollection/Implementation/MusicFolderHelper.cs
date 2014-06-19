using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.ToolBox;

//using MusicCollection.ToolBox;
using MusicCollection.FileImporter;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{
    internal class MusicFolderHelper
    {

        private string _Root, _Cache, _File,_Temp;

        internal MusicFolderHelper(string Root = null)
        {
            _Root = Path.GetFullPath( Path.Combine((Root ?? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)), "Music Collection"));

            if (!Directory.Exists(_Root))
                Directory.CreateDirectory(_Root);

            _Temp = Path.Combine(Path.GetTempPath(),"Music Collection");
            DirectoryInfo dire = new DirectoryInfo(_Temp);
            if (!dire.Exists)
                dire.Create();
            else
                dire.Empty();

            _File = Path.Combine(_Root, "Files");
            if (!Directory.Exists(_File))
                Directory.CreateDirectory(_File);

            _Cache = Path.Combine(_Root, "Cache");
            if (!Directory.Exists(_Cache))
                Directory.CreateDirectory(_Cache);

        }

        //internal MusicFolderHelper(string Root, string cache, string file, string temp)
        //{
        //    _Root = Root;

        //    if (!Directory.Exists(_Root))
        //        Directory.CreateDirectory(_Root);

        //    _Temp = temp;
        //    DirectoryInfo dire = new DirectoryInfo(_Temp);
        //    if (!dire.Exists)
        //        dire.Create();
        //    else
        //        dire.Empty();

        //    _File = file;
        //    if (!Directory.Exists(_File))
        //        Directory.CreateDirectory(_File);

        //    _Cache = cache;
        //    if (!Directory.Exists(_Cache))
        //        Directory.CreateDirectory(_Cache);
        //}

        internal string Root
        {
            get { return _Root; }
        }

        internal string File
        {
            get { return _File; }
        }

        internal string Cache
        {
            get { return _Cache; }
        }

        internal string Temp
        {
            get 
            {
                var di = new DirectoryInfo(_Temp);
                if (!di.Exists)
                    di.Create();

                return _Temp; 
            }
        }

        //private string MusicCreateFolder(string Name)
        //{
        //    string PathDirecorty = Path.Combine(_File, Name);
        //    if (!Directory.Exists(PathDirecorty))
        //    {
        //        Directory.CreateDirectory(PathDirecorty);
        //    }
        //    return PathDirecorty;
        //}

        internal string CreateMusicalFolder(IDiscInfoCue Cue)
        {
            string Main = Cue.AlbumArtistClue.FormatForDirectoryName(35);
            string Second = Cue.AlbumNameClue.FormatForDirectoryName(35);

            string PathDirecorty = Path.Combine(_File, Main);
            if (!Directory.Exists(PathDirecorty))
            {
                Directory.CreateDirectory(PathDirecorty);
            }

            return FileInternalToolBox.CreateFolder(PathDirecorty, Second);
        }

        internal string GetPrivatePath()
        {
            string Name = Guid.NewGuid().ToString();
            return GetPrivatePath(Name);
            //int key2 = Math.Abs(Name.GetHashCode() % 999983);

            //string res = _Cache;// GetCacheDirectoty(); // Path.Combine(_Root, key2.ToString());

            //for (int i = 0; i < 3; i++)
            //{
            //    int key = (int)(key2 / (Math.Pow(100, (2 - i))) % 100);
            //    res = Path.Combine(res, key.ToString());

            //    DirectoryInfo dou = new DirectoryInfo(res);
            //    if (!dou.Exists)
            //    {
            //        dou.Create();
            //    }
            //}

            //return Path.Combine(res, Name);
        }

        internal string GetPrivatePath(string Name)
        {
            int key2 = Math.Abs(Name.GetHashCode() % 999983);

            string res = _Cache;// GetCacheDirectoty(); // Path.Combine(_Root, key2.ToString());

            for (int i = 0; i < 3; i++)
            {
                int key = (int)(key2 / (Math.Pow(100, (2 - i))) % 100);
                res = Path.Combine(res, key.ToString());

                DirectoryInfo dou = new DirectoryInfo(res);
                if (!dou.Exists)
                {
                    dou.Create();
                }
            }

            return Path.Combine(res, Name);
        }

        internal string GetMusicFolder(string OriginalFileName)
        {
            string PathDirecorty = Path.GetDirectoryName(OriginalFileName);
            if (string.IsNullOrEmpty(PathDirecorty))
                return null;

            if (PathDirecorty.StartsWith(_File))
                return PathDirecorty;

            return null;

        }

        private bool IsUnderMusicCollectionFolder(string OriginalFileName)
        {
            string p = OriginalFileName.ToLower();
            return (p.StartsWith(_File.ToLower())); 
        }

        internal bool IsSystemFolder(string dir)
        {
            return ((dir == _Cache) || (dir == _File) || (dir == _Root));
        }

        //internal bool IsUnderMusicCollectionFolder(DirectoryInfo OriginalFileName)
        //{
        //    return IsUnderMusicCollectionFolder(OriginalFileName.FullName);
        //}

        internal bool IsFileRemovable(string fname)
        {
            FileType res = fname.GetFileType();
            switch (res)
            {
                case FileType.Image:
                case FileType.CueSheet:
                case FileType.Trash:
                case FileType.Txt:
                    return true;

                case FileType.Unknown:
                    return IsUnderMusicCollectionFolder(fname);

                default:
                    return false;
            }

        }

        internal bool RemoveFileAndEmptyDirectory(string File, bool Reversible)
        {
            FileInfo fi = new FileInfo(File);
            try
            {
                fi.Attributes = FileAttributes.Normal;
                fi.RevervibleDelete(Reversible);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem cleaning vault: " + e.ToString());
                return false;
            }


            string Dir = Path.GetDirectoryName(File);
            DirectoryInfo Di = new DirectoryInfo(Dir);

            while (!IsSystemFolder(Di.FullName) && ((Di.GetFileSystemInfos() == null) || (Di.GetFileSystemInfos().Length == 0)))
            {
                try
                {
                    Di.RevervibleDelete(Reversible);
                    Di = Di.Parent;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem cleaning vault:  " + e.ToString());
                    break;
                }
            }

            return true;
        }

        internal FileCleaner GetFileCleanerFromTracks(IEnumerable<Track> Tracks, Func<string, bool> IsAlsoRemovable, Func<DirectoryInfo, bool> fdib, bool rev)
        {
            return new FileCleaner(from t in Tracks where t.IsBroken == false select t.Path, null, IsAlsoRemovable, fdib, rev, this);
        }

        internal FileCleaner GetFileCleanerFromAlbums(IEnumerable<Album> al, Func<string, bool> IsAlsoRemovable, Func<DirectoryInfo, bool> fdib, bool rev)
        {
            return GetFileCleanerFromTracks(from ial in al from t in ial.RawTracks select t, IsAlsoRemovable, fdib, rev);
        }

        internal FileCleaner GetFileCleanerFromFiles(IEnumerable<string> files, Func<string, bool> IsAlsoRemovable, bool reversible)
        {
            return new FileCleaner(files, null, IsAlsoRemovable, null, reversible, this);
        }


    }
}
