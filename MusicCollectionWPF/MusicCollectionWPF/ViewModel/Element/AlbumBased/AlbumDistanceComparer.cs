using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel.Interface;

namespace MusicCollectionWPF.ViewModel.Element
{
    
    public class AlbumDistanceComparer : IDistanceEvaluator<IAlbum>
    {
        private IMusicSession _Session;

        public AlbumDistanceComparer(IAlbum init)
        {
            _All = init;
            _Session = init.Session;

            UpdateCacheData();

            //var als = _Session.AllAlbums.ToList();
        }

        private HashSet<IArtist> _Artist;
        private int _ArtitsCount = 0;
        private IGenre _Genre;
        private int _RatioArtits = 0;

        public  int EvaluateDistance(IAlbum T1)
        {
            int res = 0;

            if (Object.ReferenceEquals(T1, _All))
                return res;

            res = 100000;

            if ((T1 == null) || (_All == null))
                return res;

            IGenre g = T1.MainGenre;

            if ((g != null) && (_Genre != null))
            {
                int vg = g.Compare(_Genre);
                res -= 1900 - Math.Min(vg, 19) * 100;
            }

            if ((T1.Year != 0) && (_All.Year != 0))
            {
                int Dist = Math.Min(2000, (Math.Abs(T1.Year - _All.Year)));
                res -= 10000 - 5 * Dist;
            }

            if (_ArtitsCount == 0)
                return res;

            IList<IArtist> arts = T1.Artists;
            int c = arts.Count;
            if (c == 0)
            {
                return res;
            }

            arts.Apply((a) => { if (_Artist.Contains(a)) res -= (int)Math.Floor((double)35000 / c); });

            _Artist.Apply((a) => { if (arts.Contains(a)) res -= this._RatioArtits; });

            return res;
        }

        private IAlbum _All;
        public  IAlbum Reference
        {
            get { return _All; }
        }


        public void UpdateCacheData()
        {
            _Artist = _All.Artists.ToHashSet();
            _ArtitsCount = _Artist.Count;
            _Genre = _All.MainGenre;

            //bufferize results
            _Session.AllGenres.Apply(g => g.Compare(_Genre));
            _RatioArtits = (int)Math.Floor((double)35000 / _ArtitsCount);
        }
    }
   
}
