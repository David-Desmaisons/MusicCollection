using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal interface IAlbumVisitor
    {
        Album Album { set; }

        void VisitImage(AlbumImage ai);

        void VisitTrack(Track tr);

        void EndAlbum();

        bool End();
    }

  
}
