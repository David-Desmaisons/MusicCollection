using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    public class AlbumGroupViewTemplateSelector : DataTemplateSelector
    {
        public static DataTemplateSelector Selector
        {
            get;
            private set;
        }

        public AlbumGroupViewTemplateSelector()
        {
        }

        static AlbumGroupViewTemplateSelector()
        {
            Selector = new AlbumGroupViewTemplateSelector();
        }

        private DataTemplate GetTemplate(FrameworkElement element, string Name)
        {
            return element.FindResource(Name) as DataTemplate;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element == null || item == null)
                return null;

            if (item is ComposedObservedCollection<IArtist, IAlbum>)
            {
                return GetTemplate(element,"ArtistCollection");
            }

            if (item is ComposedObservedCollection<IGenre, IAlbum>)
            {
                return GetTemplate(element, "GenreCollection");
            }

            if (item is IAlbum)
            {
                return GetTemplate(element,"AlbumCollection");
            }

            return null;
        }
    }
}
