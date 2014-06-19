using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

using MusicCollection.ToolBox;

namespace MusicCollection.Infra
{

    public class SpaceChecker
    {

        public FileSize SizeNeeded
        {
            get;
            private set;
        }

        public FileSize SizeAvailable
        {
            get;
            private set;
        }

        public string DiskName
        {
            get;
            private set;
        }

        public bool OK
        {
            get { return ((SizeNeeded==null) || ( SizeAvailable.SizeInByte > SizeNeeded.SizeInByte)); }
        }

        private SpaceChecker(string Directory)
        { 
            DiskName = FileServices.GetPathRoot(Directory);
            SizeAvailable = FileSize.FromBytes(FileInternalToolBox.AvailableFreeSpace(DiskName));
        }

        public SpaceChecker(string Directory, IEnumerable<string> FileToExport)
            : this(Directory)
        {
            SizeNeeded = FileSize.FromBytes((from f in FileToExport select new FileInfo(f).Length).Sum());
        }

        public SpaceChecker(string Directory, long iSizeNeeded)
            : this(Directory)
        {
            SizeNeeded = FileSize.FromBytes(iSizeNeeded);
        }

        public FileSize Delta
        {
            get { return (SizeAvailable - SizeNeeded); }
        }

        public override string ToString()
        {
            string res = string.Format("Disk:{0}\nSpace Needed (in Mo): {1}\nSpace Available (in Mo):{2}", DiskName, SizeNeeded.SizeInMB, SizeAvailable.SizeInMB);

            if (OK)           
                res += string.Format("\nRemaining space(in Mo): {0}", Delta.SizeInMB);
            else
                res += string.Format("\nMissing space(in Mo): {0}", (-Delta).SizeInMB);
            return res;
        }

    }
}