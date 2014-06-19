using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;

using MusicCollection.Infra;

namespace MusicCollection
{
    namespace Fundation
    {
        public enum AlbumMaturity
        {
            Discover,
            Collection
        }


        public interface IAlbum : INotifyPropertyChanged, IMusicObject, IComparable
            //IComparable<IAlbum>
        {
            string Author{get;}

            string Name { get; }

            string NormalizedName { get; }

            AlbumMaturity Maturity
            {
                get;
                set;
            }

            string Genre { get; }

            string Label { get; }

            uint TracksNumber { get; }

            int EffectiveTrackNumber { get; }

            DateTime DateAdded { get; }

            int Year { get; }

            ICompleteObservableCollection<IAlbumPicture> Images { get; }

            BitmapImage CoverImage { get; }

            ICompleteObservableCollection<ITrack> Tracks { get; }

            ICompleteObservableCollection<IArtist> Artists { get; }

            //bool IsModifiable { get; }

            IGenre MainGenre { get; }

            IMusicSession Session { get; }

            IDiscIDs CDIDs { get; }

            IModifiableAlbum GetModifiableAlbum(bool resetCorruptedImage=false);

        }
    }
}
