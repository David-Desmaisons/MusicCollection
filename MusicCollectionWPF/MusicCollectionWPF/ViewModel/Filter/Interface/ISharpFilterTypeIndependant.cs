using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;


namespace MusicCollectionWPF.ViewModel
{
    //[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    //internal class FilterAttribute : Attribute
    //{
    //    internal string AttributeName
    //    {
    //        set;
    //        get;
    //    }

    //    internal FilterAttribute(string Name)
    //    {
    //        AttributeName = Name;
    //    }


    //    internal FilterAttribute()
    //    {
    //    }
    //}

    public enum FilterType
    {
        //[Filter("All")]
        All,

        //[Filter("Author")]
        Artist,

        //[Filter("Name")]
        Name,

        //[Filter("Track")]
        Track,

        //[Filter("Genre")]
        Genre,

        //[Filter("Year")]
        Year,

        //[Filter("Maturity")]
        Maturity
    };

    public interface ISharpFilterTypeIndependant
    {
        bool IsFiltering { get; }

        string FilterName { get; }

        FilterType Type { get; }

        void Reset();
    }
}
