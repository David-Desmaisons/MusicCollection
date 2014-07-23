using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;


namespace MusicCollection.WebServices.GraceNote.DTO
{
    [XmlRoot("ALBUM")]
    public class AlbumDto
    {
        [XmlElement("ARTIST")]
        public string Artist { get; set; }

        [XmlElement("ARTIST_ERA")]
        public ArtistEra[] ArtistEra { get; set; }

        [XmlElement("ARTIST_ORIGIN")]
        public ArtistOrigin[] ArtistOrigin { get; set; }

        [XmlElement("ARTIST_TYPE")]
        public ArtistType[] ArtistType { get; set; }

        [XmlElement("URL")]
        public ArtworkDto[] ArtworkDto { get; set; }

        [XmlElement("GENRE")]
        public Genre[] Genre { get; set; }

        [XmlElement("GN_ID")]
        public string GracenoteId { get; set; }

        [XmlElement("PKG_LANG")]
        public string Language { get; set; }

        [XmlElement("MATCHED_TRACK_NUM")]
        public int MatchedTrackNumber { get; set; }

        [XmlElement("MOOD")]
        public Mood[] Mood { get; set; }

        [XmlAttribute("ORD")]
        public int Order { get; set; }

        [XmlElement("TEMPO")]
        public Tempo[] Tempo { get; set; }

        [XmlElement("TITLE")]
        public string Title { get; set; }

        [XmlElement("TRACK_COUNT")]
        public int TrackCount { get; set; }

        [XmlElement("TRACK")]
        public TrackDto[] TrackDto { get; set; }

        [XmlElement("DATE")]
        public int Year { get; set; }
    }
}

