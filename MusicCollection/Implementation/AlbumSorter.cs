using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;


using MusicCollection.Fundation;
//using MusicCollection.Properties;
using MusicCollection.ToolBox;
using MusicCollection.Implementation;
using MusicCollection.RandomExtender;


namespace MusicCollection.Implementation
{
    internal class AlbumSorter : IAlbumSorter
    {
        private abstract class Comparator : ICompleteComparer<IAlbum>
        {
            private bool _Asc;

            internal bool Ascendant
            {
                get { return _Asc; }
                set { _Asc = value; }
            }

            protected Comparator(bool asc)
            {
                _Asc = asc;
            }

            protected abstract int BasicCompare(IAlbum x, IAlbum y);

            public int Compare(IAlbum x, IAlbum y)
            {
                return _Asc ? BasicCompare(x, y) : -BasicCompare(x, y);
            }

            public int Compare(object x, object y)
            {
                return _Asc ? BasicCompare(x as Album, y as Album) : -BasicCompare(x as Album, y as Album);
            }


            static internal Comparator FromType(AlbumFieldType type, bool asc)
            {
                switch (type)
                {
                    case AlbumFieldType.AlbumYear:
                        return new AlbumYearComparator(asc);

                    case AlbumFieldType.Genre:
                        return new GenreComparator(asc);

                    case AlbumFieldType.Artist:
                        return new ArtistComparator(asc);

                    case AlbumFieldType.ImportDate:
                        return new ImportDateComparator(asc);

                    case AlbumFieldType.Random:
                        return new RandomComparator(asc);

                    case AlbumFieldType.Name:
                        return new NameComparator(asc);
                }

                return null;
            }
        }

        private class AlbumYearComparator : Comparator
        {
            internal AlbumYearComparator(bool a)
                : base(a)
            {
            }

            protected override int BasicCompare(IAlbum x, IAlbum y)
            {
                return x.Year - y.Year;
            }
        }

        private class GenreComparator : Comparator
        {
            internal GenreComparator(bool a)
                : base(a)
            {
            }

            protected override int BasicCompare(IAlbum x, IAlbum y)
            {
                string xgenre = x.Genre;
                string ygenre = y.Genre;

                if (xgenre != null)
                    return xgenre.CompareTo(ygenre);

                if (ygenre == null)
                    return 0;

                return -ygenre.CompareTo(xgenre);
            }
        }

        private class ArtistComparator : Comparator
        {
            internal ArtistComparator(bool a)
                : base(a)
            {
            }

            protected override int BasicCompare(IAlbum x, IAlbum y)
            {
                return x.Author.CompareTo(y.Author);
            }
        }

        private class ImportDateComparator : Comparator
        {
            internal ImportDateComparator(bool a)
                : base(a)
            {
            }

            protected override int BasicCompare(IAlbum x, IAlbum y)
            {
                return x.DateAdded.CompareTo(y.DateAdded);
            }
        }

        private class NameComparator : Comparator
        {
            internal NameComparator(bool a)
                : base(a)
            {
            }

            protected override int BasicCompare(IAlbum x, IAlbum y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        private class RandomComparator : Comparator
        {
            internal RandomComparator(bool a)
                : base(a)
            {
            }

            protected override int BasicCompare(IAlbum x, IAlbum y)
            {
                return x.RandomValue() - y.RandomValue();
            }
        }


        private MusicSessionImpl _MSI;
        private Comparator _CAP;
        private IAparencyUserSettings _IAparencyUserSettings;

        internal AlbumSorter(MusicSessionImpl msi)
        {
            _MSI = msi;
            _IAparencyUserSettings = msi.Setting.AparencyUserSettings;

            //if (Settings.Default.AlbumSortering == AlbumFieldType.Random)
            //    AlbumRandomExtender.Resetvalues(_MSI);

            if (_IAparencyUserSettings.AlbumSortering == AlbumFieldType.Random)
                AlbumRandomExtender.Resetvalues(_MSI);

            _CAP = Comparator.FromType(FilterOn, Ascendant);
        }

        public bool Ascendant
        {
            get { return _IAparencyUserSettings.AlbumSorterPolarity; }
            set
            {
                _IAparencyUserSettings.AlbumSorterPolarity = value;

                _CAP.Ascendant = value;

                OnSpecChange();
            }
        }

        public ICompleteComparer<IAlbum> Sorter
        {
            get
            {
                return _CAP;
            }
        }

        public AlbumFieldType FilterOn
        {
            get { return _IAparencyUserSettings.AlbumSortering; }
            set
            {
                _IAparencyUserSettings.AlbumSortering = value;
                if (value == AlbumFieldType.Random)
                    AlbumRandomExtender.Resetvalues(_MSI);

                _CAP = Comparator.FromType(value, Ascendant);

                OnSpecChange();
            }
        }

        private void OnSpecChange()
        {
            if (OnChanged == null)
                return;

            OnChanged(this, new EventArgs());
        }

        public event EventHandler OnChanged;


        public Func<IAlbum, object> KeySelector
        {
            get { throw new NotImplementedException(); }
        }
    }
}

namespace MusicCollection.RandomExtender
{
    static internal class AlbumRandomExtender
    {
        static private Dictionary<IAlbum, int> _Values = new Dictionary<IAlbum, int>();
        static private int _Max;
        static private Random _Random;

        static internal void Resetvalues(IInternalMusicSession ses)
        {
            _Values = new Dictionary<IAlbum, int>();
            _Max = Math.Min(100000, ses.Albums.Count * 10);
            _Random = new Random();
        }

        static internal int RandomValue(this IAlbum al)
        {
            int res = 0;

            if (_Values.TryGetValue(al, out res))
            {
                return res;
            }

            res = _Random.Next(0, _Max);
            _Values.Add(al, res);

            return res;
        }
    }


}
