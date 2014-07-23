using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("RANGE")]
    public class RangeDto
    {
        public RangeDto()
        {
        }

        public RangeDto(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }

        [XmlElement("COUNT")]
        public int Count { get; set; }

        [XmlElement("END")]
        public int End { get; set; }

        [XmlElement("START")]
        public int Start { get; set; }
    }
}

