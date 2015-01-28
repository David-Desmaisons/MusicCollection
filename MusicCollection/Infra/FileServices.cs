using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;

using MusicCollection.ToolBox;
using System.Runtime.InteropServices;

namespace MusicCollection.Infra
{

    public enum FileType { Music = 0, MusicToConv = 1, RarCompressed = 2, CueSheet = 3, Image = 4, Txt = 5, Trash = 6, Mcc=7, XML=8, Unknown = 9 };

    public enum FileStatus
    {
        [Description("File Found")]
        FileExistsDriverFixed,

        [Description("File Found")]
        FileExistsDriverRemovable,

        [Description("File Not  Found")]
        DriverNotFound,

        [Description("File Not Found")]
        FileNotFoundDriverFoundRemovable,

        [Description("File Broken")]
        FileNotFoundDriverFoundFixed 
    
    };

    public static class FileServices
    {
        static private string[] MusicFiles = { ".mp3", ".mp2", ".wma", ".m4a", ".aac", ".mp4", ".aiff" };
        static private string[] MusicToconvert = { ".ape", ".flac", ".ogg", ".wav", ".wv" };
        static private string[] CompressedFiles = { ".rar", ".zip", ".7z", ".tar", ".lzh" };
        static private string[] MusicCUE = { ".cue" };
        static public string[] ImagesFiles = { ".jpg", ".jpeg", ".bmp", ".png", ".tiff", ".tif", ".gif" };
        static private string[] Trash = { ".db" };
        static private string[] txt = { ".txt" };
        static private string[] MusicCollectionCompressed = { ".mcc" };
        static private string[] XML = { ".xml" };

        //private const string _RAR = ".rar";
        //public static string RAR
        //{
        //    get { return _RAR; }
        //}

        private const string _WV = ".wv";
        public static string WV
        {
            get { return _WV; }
        }

        private const string _FLAC = ".flac";
        public static string FLAC
        {
            get { return _FLAC; }
        }

        private const string _MP3 = ".mp3";
        public static string MP3
        {
            get { return _MP3; }
        }

        private const string _APE = ".ape";
        public static string APE
        {
            get { return _APE; }
        }

        private const string _AIFF = ".aiff";
        public static string AIFF
        {
            get { return _AIFF; }
        }

        private const string _AAC = ".aac";
        public static string AAC
        {
            get { return _AAC; }
        }


        private const string _M4A = ".m4a";
        public static string M4A
        {
            get { return _M4A; }
        }

        private const string _MP4 = ".mp4";
        public static string MP4
        {
            get { return _MP4; }
        }



        private const string _OGG = ".ogg";
        public static string OGG
        {
            get { return _OGG; }
        }

        private const string _WAV = ".wav";
        public static string WAV
        {
            get { return _WAV; }
        }

        static private Dictionary<string, FileType> _FileExtDic = new Dictionary<string, FileType>();

        private const string _UserAgent = @"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.215 Safari/535.1";

        static public string UserAgent
        {
            get
            { return _UserAgent; }
        }

        static FileServices()
        {
            foreach (string m in MusicFiles)
            {
                _FileExtDic.Add(m, FileType.Music);
            }

            foreach (string m in MusicToconvert)
            {
                _FileExtDic.Add(m, FileType.MusicToConv);
            }

            foreach (string m in CompressedFiles)
            {
                _FileExtDic.Add(m, FileType.RarCompressed);
            }

            foreach (string m in ImagesFiles)
            {
                _FileExtDic.Add(m, FileType.Image);
            }

            foreach (string m in MusicCUE)
            {
                _FileExtDic.Add(m, FileType.CueSheet);
            }

            foreach (string m in Trash)
            {
                _FileExtDic.Add(m, FileType.Trash);
            }

            foreach (string m in txt)
            {
                _FileExtDic.Add(m, FileType.Txt);
            }

            foreach (string m in MusicCollectionCompressed)
            {
                _FileExtDic.Add(m, FileType.Mcc);
            }

            foreach (string m in XML)
            {
                _FileExtDic.Add(m, FileType.XML);
            }

        }

        public static FileType GetFileType(this string Fi)
        {
            string Ext = Path.GetExtension(Fi).ToLower();
            FileType res = FileType.Unknown;


            if (!_FileExtDic.TryGetValue(Ext, out res))
                return FileType.Unknown;

            return res;
        }

        static public bool IsBroken(FileStatus fs)
        {
            switch (fs)
            {
                case FileStatus.FileExistsDriverFixed:
                case FileStatus.FileExistsDriverRemovable:
                    return false;
            }

            return true;
        }

        static public bool PotentialRemovable(this FileStatus fs)
        {
            switch (fs)
            {
                case FileStatus.DriverNotFound:
                case FileStatus.FileExistsDriverRemovable:
                case FileStatus.FileNotFoundDriverFoundRemovable:
                    return true;
            }

            return false;
        }

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static bool PathFileExists(StringBuilder path);

        private static bool FastExists(string iName)
        {
            StringBuilder builder = new StringBuilder(iName);
            return PathFileExists(builder);
        }

        static public FileStatus GetFileStatus(string FileName)
        {
            DirectoryInfo Di = new DirectoryInfo(Path.GetDirectoryName(FileName));

            if (!Di.Root.Exists)
                return FileStatus.DriverNotFound;

            DriveInfo DI = new DriveInfo(Di.Root.Name);

            if (FastExists(FileName))
            {
                return (DI.DriveType == DriveType.Fixed) ? FileStatus.FileExistsDriverFixed : FileStatus.FileExistsDriverRemovable;
            }

            if (DI.DriveType == DriveType.Fixed)
                return FileStatus.FileNotFoundDriverFoundFixed;


            return FileStatus.FileNotFoundDriverFoundRemovable;
        }

        static public string GetRarFilesSelectString()
        {
            return string.Join(";",from com in CompressedFiles select string.Format("*{0}",com) );
        }

        static public string GetImagesFilesSelectString()
        {
            return string.Join(";", from com in ImagesFiles select string.Format("*{0}", com));
        }

        static public void OpenExplorerWithSelectedFiles(IEnumerable<string> FilePath )
        {
            ILookup<string, string> loo = (from f in FilePath let fi=new FileInfo(f) where fi.Exists select fi).ToLookup(f => f.DirectoryName,f=>f.FullName);

            foreach (IGrouping<string, string> dir in loo)
            {
                //"/select,Z:\Music\Counting Sheep\01. Sheep #1.mp3 /select,Z:\Music\Counting Sheep\02. Sheep #2.mp3")
               // string select = string.Join(" ", from p in dir select string.Format("/select,{0}", p));

                using (Process explorer = new Process())
                {
                    explorer.StartInfo.FileName = "explorer.exe";
                    explorer.StartInfo.Arguments = string.Format(@"/e, /select,""{0}""", dir.First());
                    explorer.Start();
                }

                //Process.Start("explorer.exe", string.Format(@"/e, /select,""{0}""", dir.First()));
            }
        }

        static public string GetPathRoot(string Directory)
        {
            string res = string.Empty;

            if (string.IsNullOrEmpty(Directory))
                return res;

            try
            {
                res = Path.GetPathRoot(Directory);
                if (res.Length == 2)
                    res += @"\";
            }
            catch (Exception E)
            {
                Trace.WriteLine(E);
            }

            return res.ToUpper();
        }
       
    }
}
