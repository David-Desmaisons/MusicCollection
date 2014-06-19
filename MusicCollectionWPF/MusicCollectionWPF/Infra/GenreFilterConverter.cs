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
using MusicCollection.Infra;

namespace MusicCollectionWPF.Infra
{
    public class GenreFilterConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<IGenre> genres = value as IEnumerable<IGenre>;
            if (genres == null)
                return null;

            return FilterOption<IGenre>.ItemFilterChooser(genres, (g) => g.FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ArtistFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<IArtist> genres = value as IEnumerable<IArtist>;
            if (genres == null)
                return null;

            return FilterOption<IArtist>.ItemFilterChooser(genres, (g) => g.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AlbumNameFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<IAlbum> als = value as IEnumerable<IAlbum>;
            if (als == null)
                return null;

            return FilterOption<string>.ItemFilterChooser((from al in als select al.Name).Distinct(), (g) => g);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SortedYearFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<IAlbum> als = value as IEnumerable<IAlbum>;
            if (als == null)
                return null;

            return FilterOption<int>.ItemFilterChooser((from al in als select al.Year).Distinct(), (g) => g.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    


    
}
