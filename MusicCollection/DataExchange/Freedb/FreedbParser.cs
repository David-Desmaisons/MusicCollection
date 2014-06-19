using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MusicCollection.Fundation;

namespace MusicCollection.DataExchange.Freedb
{
    internal class FreedbParser : AbstractAlbumDescriptorBuilderEngine, IAlbumDescriptorBuilderEngine
    {
        private static readonly Regex _Indexed = new Regex(@"^(TTITLE)(\d{1,2})$");
        
        private const string _DTitle = "DTITLE";

        protected override bool PrivateVisit(IAlbumDescriptorBuilder iab)
        {
            if (_Path == null)
                return false;

            foreach (string tbp in _Path)
            {
                string[] splitted = tbp.Split(new char[] { '=' });
                if (splitted.Length != 2)
                    continue;

                Match m = _Indexed.Match(splitted[0]);
                if (m.Success)
                {
                    IAttributeObjectBuilder<TrackDescriptor> track = iab.DescribeNewTrack();
                    track.DescribeAttribute("TNUMBER", () => m.Groups[2].Value);
                    track.DescribeAttribute(m.Groups[1].Value, () => splitted[1]);
                    track.Mature();
                    continue;
                }

                if (splitted[0] == _DTitle)
                {
                    string[] splitted2 = splitted[1].Split(new char[] { '/' });

                    int l = splitted2.Length;

                    if ((l != 1) && (l != 2))
                        return false;

                    iab.DescribeAttribute(string.Format("{0}0",splitted[0]), () => splitted2[0].Trim());
                    iab.DescribeAttribute(string.Format("{0}1", splitted[0]), () => ((l == 2) ? splitted2[1] : splitted2[0]).Trim());
                 
                    continue;
                }
                    
                iab.DescribeAttribute(splitted[0], ()=>splitted[1]);
                
            }

            return true;
        }

        private List<string> _Path;
        internal FreedbParser(List<string> Path)
        {
            _Path = Path;
            
        }

        
    }
}
