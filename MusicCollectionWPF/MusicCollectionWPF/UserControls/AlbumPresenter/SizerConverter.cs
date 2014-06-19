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
using System.Collections;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{

    public class DefileSizerConverter : IValueConverter
    {
        private DefileSizerConverter()
        {
        }

        static DefileSizerConverter()
        {
            Converter = new DefileSizerConverter();
        }

        static public DefileSizerConverter Converter
        {
            get;
            private set;
        }

        public static int Convert(int sv)
        {
            return Math.Max(0, Math.Min(4, sv)) + 1;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int sv = (int)value;
            return Convert(sv);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("ohoho");
        }
    }

    public class ClassicSizerConverter : IMultiValueConverter
    {
        private ClassicSizerConverter()
        {
        }


        static ClassicSizerConverter()
        {
            Converter = new ClassicSizerConverter();
        }

        static public IMultiValueConverter Converter
        {
            get;
            private set;
        }


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double dou = (double)values[0];
                int sv = (int)values[1];

                double inrage = Math.Max(0, Math.Min(4, sv));
                double intm = inrage / 3;
                double res = dou / (1 + intm);

                return res;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error in ClassicSizerConverter " + ex.ToString());
                return 0;

            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        //public class LikeHoodSizerConverter : IValueConverter
        //{
        //    private LikeHoodSizerConverter()
        //    {
        //    }


        //    static LikeHoodSizerConverter()
        //    {
        //        Converter = new LikeHoodSizerConverter();
        //    }

        //    static public IValueConverter Converter
        //    {
        //        get;
        //        private set;
        //    }

        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        int sv = (int)value;

        //        return Math.Max(0, Math.Min(4, sv)) + 9;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new Exception("ohoho");
        //    }
        //}
    }
}
