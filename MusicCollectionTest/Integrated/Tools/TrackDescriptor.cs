//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using MusicCollection.DataExchange;
//using MusicCollection.Fundation;


//namespace MusicCollectionTest.Integrated.Tools
//{
//    internal class TrackDescriptor:ITrackMetaDataDescriptor
//    {
//        public IAlbumDescriptor AlbumDescriptor
//        {
//            get {return Album; }
//        }

//        internal AlbumDescriptor Album {get;set;}

//        public string Artist {get;set;}
        
//        public string Name {get;set;}
       
//        public ISRC ISRC {get;set;}
       
//        public uint TrackNumber {get;set;}

//        public TimeSpan Duration { get; set; }
//    }

//    internal class AlbumDescriptor : IAlbumDescriptor
//    {
//        public string Artist { get; set; }

//        public string Genre { get; set; }

//        public IDiscIDs IDs { get; set; }

//        public uint TracksNumber { get; set; }

//        public string Name { get; set; }

//        public int Year { get; set; }
//    }



    
//}
