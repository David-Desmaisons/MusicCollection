using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{
    [XmlRoot("TOC")]
    public class Toc
    {
        public Toc( params int[] args)
        {
            _Offset = new List<int>(args);
        }

        public Toc(IEnumerable<int> offsets)
        {
            _Offset = new List<int>(offsets);
        }

        public Toc()
        {
            _Offset = new List<int>();
        }

        private IList<int> _Offset;
        [XmlIgnore]
        public IList<int> ListOffSet
        {
            get { return _Offset; }
        }

        [XmlElement("OFFSETS")]
        public string OffSet
        {
            get { return string.Join(" ", ListOffSet.Select(o => o.ToString())); }
            set { }
        }
    }
}
