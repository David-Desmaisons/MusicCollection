using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("TRACK")]
    public class TrackDto
    {
        [XmlElement("ARTIST")]
        public string Artist { get; set; }

        [XmlElement("ARTIST_ERA")]
        public ArtistEra[] ArtistEra { get; set; }

        [XmlElement("ARTIST_ORIGIN")]
        public ArtistOrigin[] ArtistOrigin { get; set; }

        [XmlElement("ARTIST_TYPE")]
        public ArtistType[] ArtistType { get; set; }

        [XmlElement("GENRE")]
        public Genre[] Genre { get; set; }

        [XmlElement("GN_ID")]
        public string GracenoteId { get; set; }

        [XmlElement("MOOD")]
        public Mood[] Mood { get; set; }

        [XmlElement("TRACK_NUM")]
        public int Number { get; set; }

        [XmlElement("TEMPO")]
        public Tempo[] Tempo { get; set; }

        [XmlElement("TITLE")]
        public string Title { get; set; }
    }
}

