using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace MusicCollection.ToolBox
{
    internal static class FileInternalToolBox
    {
        private static readonly Regex _RarParser;

        static FileInternalToolBox()
        {
            _RarParser = new Regex(@"^(\w.*?)(\.part\d)?\.\w{2,3}$");
        }


        static internal string CreateNewAvailableName(string targetdir, string TargetName,string Ext, bool Rename=true)
        {
            if (targetdir.Length >= 248)
                throw new ArgumentException();
                  
            int max = 255 - targetdir.Length -1 - Ext.Length;  
            string Tentative =  Path.Combine(targetdir, TargetName.ToMaxLength(max))+Ext; 
            string CrueName = Path.Combine(targetdir, TargetName.ToMaxLength(Math.Max(0,max-3)));

            int i = 0;
            while (File.Exists(Tentative))
            {
                if ((!Rename) && (i == 0)  )
                    return null;

                Tentative = string.Format("{0}({1}){2}", CrueName, ++i, Ext);
            }

            return Tentative;
        }

        //static internal string CreateNewAvailableName(string targetdir, string TargetName, bool Rename = true)
        //{
        //    return CreateNewAvailableName(targetdir, Path.GetFileNameWithoutExtension(TargetName), Path.GetExtension(TargetName), Rename);
        //}

        static internal string CreateNewAvailableName(string TargetName, bool Rename = true)
        {
            return CreateNewAvailableName(Path.GetDirectoryName(TargetName), Path.GetFileNameWithoutExtension(TargetName), Path.GetExtension(TargetName), Rename);
        }

        static internal string RawRarName(string FileName)
        {

            string res = Path.GetFileName(FileName);
            if (string.IsNullOrEmpty(res))
                return null;


            return _RarParser.Match(res).Groups[1].Value;
        }

        static internal string CreateFolder(string Root, string FileDirectory)
        {
            string PathDirecorty = Path.Combine(Root, FileDirectory);

            string reference = PathDirecorty;
            int i = 1;

            while (Directory.Exists(PathDirecorty))
            {
                PathDirecorty = string.Format("{0}({1})", reference, i++);
            }

            Directory.CreateDirectory(PathDirecorty);

            return PathDirecorty;
        }


        //static internal string GetDirectoryName(string path)
        //{
        //    string Ext = Path.GetExtension(path);
        //    bool HasExtension = !(string.IsNullOrEmpty(Ext)) && Ext.Length == 4;
        //    return HasExtension ? Path.GetDirectoryName(path) : path;
        //}

        static internal string GetApplicationDirectoryName()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Music Collection");

            DirectoryInfo dif = new DirectoryInfo(path);
            if (!dif.Exists)
                dif.Create();
            return path;
        }

        static private bool HasValidExtension(string path)
        {
            string Ext = Path.GetExtension(path);
            return !(string.IsNullOrEmpty(Ext)) && Ext.Length >= 4;
        }

        static internal string GetFileName(string path)
        {
            return HasValidExtension(path) ? Path.GetFileName(path) : null;
        }

        static internal long AvailableFreeSpace(string iDrive)
        {
            try
            {
                foreach (DriveInfo d in DriveInfo.GetDrives())
                {
                    if (d.Name == iDrive)
                    {
                        return d.AvailableFreeSpace;
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                //AvailableFreeSpace may throw exception
                Console.WriteLine("Problem computing disk size:{0} exception {1}",iDrive,e);
                return 0;
            }

        }

        static internal IEnumerable<char> GetAvailableCDDriver()
        {
            return from d in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.CDRom) select d.Name[0];
        }

    }

   
}
