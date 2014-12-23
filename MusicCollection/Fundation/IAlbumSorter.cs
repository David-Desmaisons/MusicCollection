using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace MusicCollection.Fundation
{

    public interface ICompleteComparer<T> : IComparer, IComparer<T>
    {
    }

    public interface IAlbumBasicSorter
    {
        ICompleteComparer<IAlbum> Sorter { get; }

        event EventHandler OnChanged;
    } 

    public interface IAlbumSorter : IAlbumBasicSorter
    {
        bool Ascendant { get; set; }

        AlbumFieldType FilterOn { get; set; }

        Func<IAlbum,object> KeySelector { get; }
    }
    
   
    [Serializable]
    public enum AlbumFieldType 
    {
        [Description("Sort by release year")]
        AlbumYear,

        [Description("Sort by import date")]
        ImportDate,

        [Description("Sort by artist name")]
        Artist,

        [Description("Sort by album name")]
        Name,

         [Description("Sort by album genre")]
        Genre,

         [Description("Sort randomly")]
        Random
    
    };
}
