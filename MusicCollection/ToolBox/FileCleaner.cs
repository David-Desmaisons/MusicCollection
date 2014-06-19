using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.Implementation;



namespace MusicCollection.ToolBox
{
    //internal class FileEventArgs : EventArgs
    //{
    //    internal string FileName { get; private set; }

    //    internal FileEventArgs(string name)
    //    {
    //        FileName = name;
    //    }
    //}


    internal class FileCleaner
    {
        private IEnumerable<string> _Directory;
        private HashSet<string> _Files;
        private Func<string, bool> _CanBeRemoved;
        private Func<DirectoryInfo, bool> _CanDirBeRemoved;
        private IDictionary<string, bool> _IsRemovable = new Dictionary<string, bool>();

        private IDictionary<string, DirectoryInfo> _FilesDirectory = new Dictionary<string, DirectoryInfo>();
   
        //internal event EventHandler<FileEventArgs> OnFileNotRemove;


        //static internal FileCleaner FromTracks(IEnumerable<Track> Tracks, Func<string, bool> IsAlsoRemovable, Func<DirectoryInfo, bool> fdib, bool rev, MusicFolderHelper mfh)
        //{
        //    return new FileCleaner(from t in Tracks where t.IsBroken == false select t.Path, null, IsAlsoRemovable, fdib, rev,mfh);
        //}

        //static internal FileCleaner FromAlbums(IEnumerable<Album> al, Func<string, bool> IsAlsoRemovable, Func<DirectoryInfo, bool> fdib, bool rev, MusicFolderHelper mfh)
        //{
        //    return FromTracks(from ial in al from t in ial.CrudeTracks select t, IsAlsoRemovable, fdib,rev,mfh);
        //}

        //static internal FileCleaner FromFiles(IEnumerable<string> files, Func<string, bool> IsAlsoRemovable, bool reversible, MusicFolderHelper mfh)
        //{
        //    return new FileCleaner(files, null, IsAlsoRemovable, null, reversible, mfh);
        //}

        private MusicFolderHelper _MFH;

        internal FileCleaner(IEnumerable<string> files, IEnumerable<string> Directory, Func<string, bool> IsAlsoRemovable, Func<DirectoryInfo, bool> IsDirRemovable, bool reversible, MusicFolderHelper mfh)
        {
            _Files = new HashSet<string>(from f in ((files == null) ? Enumerable.Empty<string>() : files) select f.ToLower());
            _CanBeRemoved = IsAlsoRemovable;
            _CanDirBeRemoved = IsDirRemovable;
            _Directory = Directory ?? Enumerable.Empty<string>();
            _MFH = mfh;
        }

        private bool CanFileBeRemoved(string FN)
        {
            if (_CanBeRemoved == null)
                return false;

            return _CanBeRemoved(FN);
        }

        private bool CanDirBeRemoved(DirectoryInfo DN)
        {
            if (_CanDirBeRemoved == null)
                return false;

            return _CanDirBeRemoved(DN);
        }


        

        //static internal FileCleaner FromDirectory(IEnumerable<string> Directory, Func<DirectoryInfo, bool> IsAlsoRemovable, bool reversible)
        //{
        //    return new FileCleaner(Enumerable.Empty<string>(), Directory,null, null,reversible);
        //}

        internal IEnumerable<string> Paths
        {
            get
            {
                return _Files;
            }
        }

        internal void Remove()
        {
            var FatherDirTable = _Files.ToDictionary((p)=>p,(p) => Path.GetDirectoryName(p));
            HashSet<DirectoryInfo> ToRemove = new HashSet<DirectoryInfo>();


            var Direcres = (from ds in (FatherDirTable.Values.Concat(_Directory??Enumerable.Empty<string>())).Distinct() let Dir = new DirectoryInfo(ds) where IsDirRemovable(Dir) select Dir).ToList(); 
            //tolist pour forcer le calcul de IsDirRemovable

            foreach (string file in _Files)
            {
                if (!IsDirRemovable(FatherDirTable[file]))
                {
                    try
                    {
                        FileExtender.RevervibleFileDelete(file, true);
                        //File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                        //if (OnFileNotRemove != null)
                        //    OnFileNotRemove(this, new FileEventArgs(file));
                    }
                }
            }



            foreach (DirectoryInfo dirtor in Direcres)
            {
                dirtor.Refresh();

                if (!dirtor.Exists)
                    continue;

                DirectoryInfo ToRemov = dirtor;
                DirectoryInfo Father = dirtor.Parent;

                if (IsRemovable(Father) == null)
                {
                    FileSystemInfo[] fis = Father.GetFileSystemInfos();
                    while ((fis != null) && (fis.Length == 1) && (!_MFH.IsSystemFolder(Father.FullName)))
                    {
                        ToRemov = Father;
                        Father = ToRemov.Parent;
                        fis = Father.GetFileSystemInfos();
                    }
                }

                try
                {
                     //ToRemov.Delete(true);
                     ToRemov.RevervibleDelete(true);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                } 
              
            }


        }

        private bool IsDirRemovable(string di)
        {
            return _IsRemovable[di];
        }

        private bool? IsRemovable(DirectoryInfo di)
        {
            bool res = false;
            if (_IsRemovable.TryGetValue(di.FullName, out res))
                return res;

            return null;
        }

        private void Register(DirectoryInfo di, bool removable)
        {
            _IsRemovable.Add(di.FullName, removable);
        }

        private bool IsDirRemovable(DirectoryInfo di)
        {
            bool? res = IsRemovable(di);
            if (res != null)
                return (bool)res;

            if (_MFH.IsSystemFolder(di.FullName))
            {
                Register(di, false);
                return false;
            }

            if (CanDirBeRemoved(di))
            {
                Register(di, true);
                return true;
            }

            foreach (DirectoryInfo Di in di.GetDirectories())
            {
                if (!IsDirRemovable(Di))
                {
                    Register(di, false);
                    return false;
                }
            }

            foreach (FileInfo Fi in di.GetFiles())
            {
                string fn = Fi.FullName.ToLower();
                if (!_Files.Contains(fn) && !CanFileBeRemoved(fn))
                {
                    Register(di, false);
                    return false;
                }
            }

            Register(di, true);
            return true;
        }


    }
}
