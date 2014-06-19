using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{

    internal class SizeComputer : IAlbumVisitor
    {
        private long _Length = 0;

        internal SizeComputer()
        {
        }

        public Album Album
        {
            set
            { }
        }

        internal long Length
        {
            get { return _Length; }
        }

        public void VisitImage(AlbumImage ai)
        {
            if (ai!=null) //@report
                _Length += ai.SizeOnDisk;
        }

        public void VisitTrack(Track tr)
        {
            try
            {
                FileInfo fi = new FileInfo(tr.Path);

                if (!fi.Exists)
                    return;

                _Length += fi.Length;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem with fileinfo and track"+e.ToString());
            }
        }

        public void EndAlbum()
        {
        }

        public bool End()
        {
            return true;
        }
    }

    internal class SizeChecker : IAlbumVisitor
    {
        private long _Length = 0;
        private string _DN;
        private SpaceChecker _SC;

        internal SizeChecker(string DirectoryName)
        {
            _DN = DirectoryName;
        }

        public Album Album
        {
            set
            { }
        }

        internal SpaceChecker Checker
        {
            get { return _SC; }
        }

        public void VisitImage(AlbumImage ai)
        {
            _Length += ai.SizeOnDisk;
        }

        public void VisitTrack(Track tr)
        {
            try
            {
                FileInfo fi = new FileInfo(tr.Path);

                if (!fi.Exists)
                    return;

                _Length += fi.Length;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem with fileinfo and track"+e.ToString());
            }
        }

        public void EndAlbum()
        {
        }

        public bool End()
        {
            _SC = new SpaceChecker(_DN, _Length);
            return _SC.OK;
        }
    }
}

//SizeChecker sc = new SizeChecker(FileDirectory);

//                foreach (Album Al in AlbumToExport)
//                {
//                    Al.Visit(sc);
//                }

//                if (!sc.End())
//                {
//                    OnError(new NotEnougthSpace(sc.Checker.ToString()));
//                    return;
//                }