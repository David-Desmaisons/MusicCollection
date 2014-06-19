using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;

namespace MusicCollection.Fundation
{
    public static class ArtistHelper
    {
        public static string GetDisplayName(this IList<IArtist> Artists)
        {
            return Artist.AuthorName(Artists);
        }
    }
}
