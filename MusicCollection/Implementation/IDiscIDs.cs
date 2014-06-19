using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Implementation
{
    public interface IDiscIDs
    {
        string Asin { get; }
        //string ISRC { get; }
        string MusicBrainz_HashID { get; }
        string MusicBrainz_ID { get; }
        string CDDB { get; }
    }
}
