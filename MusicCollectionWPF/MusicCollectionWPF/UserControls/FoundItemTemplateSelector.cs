using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.UserControls
{
    class FoundItemTemplateSelector : DataTemplateSelector
    {
        private DataTemplate GetTemplate(FrameworkElement element, string Name)
        {
            return element.FindResource(Name) as DataTemplate;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element == null || item == null)
                return null;

            if (item is IArtist)
            {
                return GetTemplate(element, "ArtistTemplate");
            }

            if (item is IAlbum)
            {
                return GetTemplate(element, "AlbumTemplate");
            }

            if (item is ITrack)
            {
                return GetTemplate(element, "TrackTemplate");
            }

            return null;
        }
    }
}
