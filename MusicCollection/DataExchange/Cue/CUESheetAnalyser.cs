using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;

namespace MusicCollection.DataExchange.Cue
{
    internal class CUESheetAnalyser : AbstractAlbumDescriptorBuilderEngine,IAlbumDescriptorBuilderEngine
    {
        private const string _Track = "TRACK";
        private const string _Comment = "REM";
        private const string _Index = "INDEX";
        private const string _File = "FILE";

        private string _Path;
        internal CUESheetAnalyser(string Path)
        {
            _Path = Path;
        }

        private static readonly Regex _Reader = new Regex(@"^(\w+)(?:\s(?:(\d{2}:\d{2}:\d{2})|(\w+)|(?:""(.*?)""))){1,2}$");


        protected override bool PrivateVisit(IAlbumDescriptorBuilder iab)
        {
            if (iab == null)
                return false;

            IAttributeObjectDescriber currentobjecr = iab;
            int filenumber = 0;

            using (StreamReader tr = new StreamReader(_Path, Encoding.Default))
            {
                string lign;
                while ((lign = tr.ReadLine()) != null)
                {
                    lign = lign.NormalizeSpace();
                    Match m = _Reader.Match(lign);
                    if (m.Success)
                    {
                        List<string> res = (from g in m.Groups.Cast<Group>().Where(ma => ma.Success).Skip(1)
                                            from c in g.Captures.Cast<Capture>()
                                            orderby c.Index
                                            select c.Value).ToList();

                        string attribute = res[0];
                        switch (attribute)
                        {
                            case _Track:
                                IAttributeObjectBuilder<ITrackDescriptor> old = currentobjecr as IAttributeObjectBuilder<ITrackDescriptor>;
                                if (old != null)
                                {
                                    old.Mature();
                                }

                                currentobjecr = iab.DescribeNewTrack();

                                if (res.Count != 3)
                                    return false;

                                currentobjecr.DescribeAttribute(attribute, () => res[1]);
                                continue;

                            case _Comment:
                                if (res.Count != 3)
                                    continue;

                                currentobjecr.DescribeAttribute(res[1], () => res[2]);
                                continue;

                            case _File:
                                filenumber++;
                                if (filenumber > 1)
                                    return false;
                                break;

                            case _Index:
                                if (res.Count != 3)
                                    return false;

                                if (!m.Groups[2].Success)
                                    continue;

                                currentobjecr.DescribeAttribute(string.Format("{0} {1}", res[0], res[1]), () => res[2]);
                                continue;
                        }

                        currentobjecr.DescribeAttribute(attribute, () => res[1]);

                    }

                }
            }

            IAttributeObjectBuilder<ITrackDescriptor> last = currentobjecr as IAttributeObjectBuilder<ITrackDescriptor>;
            if (last != null)
            {
                last.Mature();
            }

            return (filenumber == 1);
        }

        //public IFullAlbumDescriptor Visit(IAlbumDescriptorBuilder iab)
        //{
        //  bool res = PrivateVisit(iab);
        //  if (res == false)
        //      return null;

        //  return iab.Mature();
        //}
    }
}
