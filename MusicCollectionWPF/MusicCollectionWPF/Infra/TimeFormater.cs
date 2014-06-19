using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using MusicCollection.Fundation;
using System.Collections.ObjectModel;

namespace MusicCollectionWPF.Infra
{
    public interface ISliderMultiConverter
    {
        string Convert(double Value, double Min, double Max);
    }

     class TimeFormater :ISliderMultiConverter
    {
        private TimeFormater()
        {
        }

        static TimeFormater()
        {
            Converter = new TimeFormater();
        }

        static public TimeFormater Converter
        {
            get;
            private set;
        }

        string ISliderMultiConverter.Convert(double Value, double Min, double Max)
        {
            return  Convert(Value);
        }

        public static string Convert(double value)
        {
            TimeSpan v = TimeSpan.FromMilliseconds(value);
            return string.Format("{0:00}:{1:00}", v.Hours * 60 + v.Minutes, v.Seconds);
        }
    }
}