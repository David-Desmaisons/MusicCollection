using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

using MusicCollection.Implementation;
using MusicCollection.Infra;
using MusicCollection.ToolBox;

namespace MusicCollection.DataExchange
{
    
    public class AlbumDescriptorExchanger
    {
        public List<AlbumDescriptor> Albums
        {
            get;
            set;
        }

        public string OriginalDir
        {
            get;
            set;
        }

        internal void Import(IImportContext Context)
        {
            Albums.Apply(tr => tr.Import(Context));
        }

        private string _Path;
        internal string CurrentPath
        {
            get { return _Path; }
        }

        internal bool Export(string iDir, string ForceRoot = null)
        {
            _Path = FileInternalToolBox.CreateNewAvailableName(Path.Combine(iDir, "Albums.xml"));

            OriginalDir = ForceRoot ?? iDir;

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(AlbumDescriptorExchanger));
                using (Stream s = File.Create(_Path))
                    xs.Serialize(s, this);
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Problem creating exporting file {0} : {1}" , _Path, e.ToString()));
                return false;
            }

            return true;
        }

        private void Reroot(IDictionary<string, string> dic)
        {
           // var res = from al in Albums from tr in al.RawTrackDescriptors select tr;
            foreach (TrackDescriptor tr in Albums.SelectMany(a=>a.RawTrackDescriptors))
            {
                string output = null;
                if (dic.TryGetValue(tr.Path, out output))
                {
                    tr.Path = output;
                }
            }
        }

        private void AfterImport(bool reconnect,string iPath)
        {
            if (Albums == null)
                return;

            Albums.Apply(eal => eal.AfterImport(reconnect,iPath, OriginalDir));
        }

        static internal AlbumDescriptorExchanger ImportContainer(string iPath, bool Reconnect, string ForcePath = null, IDictionary<string, string> iReroot = null)
        {
            AlbumDescriptorExchanger Als = null;

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(AlbumDescriptorExchanger));
                using (Stream s = File.OpenRead(iPath))
                    Als = xs.Deserialize(s) as AlbumDescriptorExchanger;

                if (Als == null)
                    return null;

                if (iReroot != null)
                    Als.Reroot(iReroot);

               Als.AfterImport(Reconnect,ForcePath ?? Path.GetDirectoryName(iPath));

                return Als;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem reading XML " + e.ToString());
            }

            return null;
        }

        static internal IList<AlbumDescriptor> Import(string iPath, bool Reconnect, string iFP, IDictionary<string, string> Reroot)
        {
            AlbumDescriptorExchanger Als = ImportContainer(iPath, Reconnect, iFP, Reroot);
            return Als == null ? null : Als.Albums;
        }
    }
}
