using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;

namespace MusicCollection.Fundation
{

    public abstract class ImportExportProgress
    {
        public string Entity { get; private set; }

        virtual public bool ImportEnded
        {
            get { return false; }
        }

        public abstract string Operation { get; }


        public override string ToString()
        {
            return string.Format("{0} : {1}", Operation, Entity);
        }

        protected ImportExportProgress(string iEntity)
        {
            Entity = iEntity;
        }
    }

    public class BeginningMove : ImportExportProgress
    {
        override public bool ImportEnded
        {
            get { return false; }
        }

        public override string Operation { get { return "Beginning To Move Files"; } }

        public BeginningMove()
            : base("")
        {
        }
    }

    public class EndImport : ImportExportProgress
    {
        override public bool ImportEnded
        {
            get { return true; }
        }

        public override string Operation { get { return "Music Importer Ended"; } }

        public EndImport()
            : base("")
        {
        }
    }

    public class EndExport : ImportExportProgress
    {
        override public bool ImportEnded
        {
            get { return true; }
        }

        public override string Operation { get { return "Music Export Ended"; } }

        public EndExport(string name)
            : base(name)
        {
        }

        public EndExport(IEnumerable<IAlbum> names)
            : base(string.Join(Environment.NewLine, names))
        {
        }
    }

    public class ImportingTrack : ImportExportProgress
    {
        public override string Operation { get { return "Importing"; } }

        public ImportingTrack(int Tracknumber)
            : base(string.Format("Track {0}", Tracknumber))
        {
        }
    }

    public class DisplayingProgress : ImportExportProgress
    {
        public override string Operation { get { return "Displaying albums"; } }

        public override string ToString()
        {
            return Operation;
        }

        public DisplayingProgress()
            : base(null)
        {
        }
    }

    public class BeginImport : ImportExportProgress
    {
        public override string Operation { get { return "Preparing Import..."; } }

        public override string ToString()
        {
            return Operation;
        }

        public BeginImport()
            : base(null)
        {
        }
    }


    #region windows
    public class ConnectingToWindowsPhone : ImportExportProgress
    {
        public override string Operation { get { return "Connecting To WindowsPhone..."; } }

        public override string ToString()
        {
            return Operation;
        }

        public ConnectingToWindowsPhone()
            : base(null)
        {
        }
    }

    public class ExportToWindowsPhone : ImportExportProgress
    {
        public override string Operation { get { return string.Format("Exporting To WindowsPhone: {0}", MusicTrack.Name); } }


        public TrackDescriptor MusicTrack
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Operation;
        }

        public ExportToWindowsPhone(TrackDescriptor tr)
            : base(null)
        {
            MusicTrack = tr;
        }
    }

    #endregion

    public class OpeningAlbums : ImportExportProgress
    {
        public override string Operation { get { return "Opening Albums..."; } }

        public override string ToString()
        {
            return Operation;
        }

        public OpeningAlbums()
            : base(null)
        {
        }
    }

    public class Finalizing : ImportExportProgress
    {
        public override string Operation { get { return "Finalizing Import"; } }

        public override string ToString()
        {
            return Operation;
        }

        public double Current { get; private set; }

        public double Total { get; private set; }

        //public double Pourcentage { get { return 100 * Current / Total; } }

        public Finalizing(double Cu, double To)
            : base(string.Empty)
            //string.Format("{0:0} %", 100 * Cu / To))
        {
            Current = Cu;
            Total = To;
        }
    }


    public class ImportProgessEventArgs : ImportExportProgress
    {
        public override string Operation { get { return "Importing"; } }

        internal ImportProgessEventArgs(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class ExtractProgessEventArgs : ImportExportProgress
    {
        public override string Operation { get { return "Extracting"; } }

        internal ExtractProgessEventArgs(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class CDImportingProgessAdditionalCoverInfoEventArgs : ImportExportProgress
    {

        public override string Operation { get { return "Looking for cover information"; } }

        internal CDImportingProgessAdditionalCoverInfoEventArgs(IAlbumDescriptor iad)
            : base(iad.Name)
        {
        }
    }

    public class CDIndentifyingProgessEventArgs : ImportExportProgress
    {
        public override string ToString()
        {
            return Operation;
        }
        public override string Operation { get { return "Identifying CD"; } }

        internal CDIndentifyingProgessEventArgs()
            : base(null)
        {
        }
    }



    public class ITunesIdentifyingProgessEventArgs : ImportExportProgress
    {
        public override string ToString()
        {
            return Operation;
        }
        public override string Operation { get { return "Scanning Itunes Collection"; } }

        internal ITunesIdentifyingProgessEventArgs()
            : base(null)
        {
        }
    }


    public class ITunesExportingProgessEventArgs : ImportExportProgress
    {
        public override string ToString()
        {
            return Operation;
        }

        public int Porcentage
        {
            get;
            private set;
        }


        public override string Operation { get { return string.Format("Identifying CD: {0}%", Porcentage); } }

        internal ITunesExportingProgessEventArgs(int p)
            : base(null)
        {
            Porcentage = p;
        }
    }



    public class CDImportingProgessEventArgs : ImportExportProgress
    {
        public override string Operation { get { return "Importing CD"; } }

        internal CDImportingProgessEventArgs(string name)
            : base(name)
        {
        }
    }

    public class ConvertProgessEventArgs : ImportExportProgress
    {
        private int _Current;
        private int _Total;

        public int Current
        {
            get { return _Current; }
        }

        public int Total
        {
            get { return _Total; }
        }

        public override string Operation { get { return string.Format("Converting {0}/{1}", _Current, _Total); } }

        internal ConvertProgessEventArgs(string iEntity, int iCurrent, int iTotal)
            : base(iEntity)
        {
            _Current = iCurrent;
            _Total = iTotal;
        }
    }


}
