using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.ToolBox;

namespace MusicCollection.Infra.File
{
    public class FileSystem : IFileTools
    {
        public string CreateNewAvailableName(string targetdir, string TargetName, string Ext, bool Rename = true)
        {
            return FileInternalToolBox.CreateNewAvailableName(targetdir, TargetName, Ext, Rename); 
        }


        public string MusicFolder
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); }
        }

        public string DocumentFolder
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
        }


        public string KeysFileExtesion
        {
            get { return ".mck"; }
        }


        public string GetFileFilter(string iExtension, string iDescription)
        {
            return string.Format("{1} (*{0})|*{0}", iExtension, iDescription);
        }


        public bool FileExists(string iPath)
        {
            return System.IO.File.Exists(iPath);
        }

        public bool DirectoryExists(string iPath)
        {
            return System.IO.Directory.Exists(iPath);
        }


        
    }
}
