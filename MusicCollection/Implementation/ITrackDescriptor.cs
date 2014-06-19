using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal interface ITrackDescriptor
    {
        IAlbumDescriptor AlbumDescriptor { get; }

        string Artist{get;}      
        string Name{get;}
        string Path { get;} 
        //string Album{get;}
        //string Album_Artist{get;}
        //string Genre{get;}

        //DiscIDs IDs { get; }

        string MD5 { get; }
        Stream MusicStream();

        ISRC ISRC { get; }
        
        uint TrackNumber{get;}
        //uint TracksNumber { get; }
        //Int32 Year{ get; }

        TimeSpan Duration { get;}
    }

    internal interface IAlbumDescriptor
    {
        string Artist{get;}

        string Genre{get;}

        DiscIDs IDs { get; }

        uint TracksNumber { get; }

         string Name{get;}

        Int32 Year{ get; }
    }
}
