using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.Implementation;

namespace MusicCollection.Fundation
{
    public class OptionChooser<T> : NotifySimpleAdapter
    {
        internal OptionChooser(IEnumerable<T> iOptions)
        {

            Options = iOptions.ToList();
            Standard = (Options.Count == 1) ? Options[0] : default(T);
            
            _C = Standard;
        }

        public List<T> Options
        {
            get;
            private set;
        }

        protected virtual T Analyse(T inv)
        {
            return inv;
        }

        public T Standard
        {
            get;
            private set;
        }

        public bool Changed
        {
            get
            {
                if (object.ReferenceEquals(Standard, Choosed))
                    return false;

                if (Standard == null)
                    return true;

                return !Standard.Equals(Choosed);
            }
        }


        private T _C;
        public T Choosed
        {
            get { return _C; }
            set
            {
                T iv = Analyse(value);

                if (object.ReferenceEquals(iv, _C))
                    return;

                if ((_C != null) && _C.Equals(iv))
                    return;

                _C = iv;
                PropertyHasChanged("Choosed");
            }
        }

        public override string ToString()
        {
            return string.Format("Choosed:{0}, Options:{1}",Choosed,string.Join(",",Options));
        }
    }

    public class OptionChooserArtist : OptionChooser<string>
    {
        private IInternalMusicSession _IMS;

        internal OptionChooserArtist(IEnumerable<string> iOptions, IInternalMusicSession iMS)
            : base(iOptions)
        {
            _IMS = iMS;
        }

        protected override string  Analyse(string inv)
        {
 	        return Artist.AuthorName( Artist.GetArtistFromName(inv, _IMS));
        }

        internal IList<Artist> Artists
        {
            get { if (Choosed == null) return null; return Artist.GetArtistFromName(Choosed, _IMS).ToList(); }
        }
    }
}
