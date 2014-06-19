using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

namespace MusicCollection.ToolBox
{
    internal static class FileExtender
    {
        internal static void RevervibleFileDelete(string ifile, bool Reversible)
        {
            if (Reversible)
                FileSystem.DeleteFile(ifile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else
                File.Delete(ifile);
        }

        //internal static void RevervibleDirDelete(string iDir, bool Reversible)
        //{
        //    if (Reversible)
        //        FileSystem.DeleteFile(iDir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        //    else
        //        Directory.Delete(iDir);
        //}


        internal static void RevervibleDelete( this FileInfo ifile, bool Reversible)
        {
            if (Reversible)
                FileSystem.DeleteFile(ifile.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else
                ifile.Delete();
        }

        internal static void RevervibleDelete(this DirectoryInfo iDir, bool Reversible)
        {
            if (Reversible)
                FileSystem.DeleteDirectory(iDir.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else
                iDir.Delete();
        }

        public static void Empty(this DirectoryInfo directory, bool Sync =false)
        {
            try
            {
                FileInfo[] fis = directory.GetFiles();
                foreach (FileInfo file in fis) file.Delete();

                DirectoryInfo[] dis = directory.GetDirectories();
                foreach (DirectoryInfo subDirectory in dis) subDirectory.Delete(true);

                directory.Refresh();

                if (Sync)
                {
                    while (directory.GetFileSystemInfos().Length != 0)
                    {
                        System.Threading.Thread.Sleep(200);
                        directory.Refresh();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Empty Problem {0}", e));

            }
        }

        public static bool Copy(this DirectoryInfo directory, DirectoryInfo Destination, bool overwrite = true)
        {
            if (!directory.Exists)
                return false;

            if (!Destination.Exists)
                Destination.Create();

            foreach (FileInfo fi in directory.GetFiles())
            {
                string destFile = System.IO.Path.Combine(Destination.FullName, fi.Name);
                fi.CopyTo(destFile, overwrite);
            }

            foreach (DirectoryInfo di in directory.GetDirectories())
            {
                string destdir = System.IO.Path.Combine(Destination.FullName, di.Name);
                di.Copy(destdir, overwrite);
            }

            return true;
        }

        public static bool Copy(this DirectoryInfo directory,string Destination,bool overwrite=true)
        {
           return directory.Copy(new DirectoryInfo(Destination),overwrite);
        }
    }
}
