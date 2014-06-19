using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Implementation
{
    internal class GenreCollection :IDisposable
    {

        private StrongCacheCollection<string, Genre> _AllGenres = new StrongCacheCollection<string, Genre>((g => g.NormalizedName), (s => s.Normalized()));
        private UISafeObservableCollection<IGenre> _Genres = new UISafeObservableCollection<IGenre>();
        private MusicSessionImpl _Session;


        internal MusicSessionImpl Session
        {
            get { return _Session; }
            set { _Session = value; }
        }

        internal GenreCollection(MusicSessionImpl msi)
        {
            _Session = msi;
        }

        internal Genre Find(string Name)
        {
             return _AllGenres.Find(Name);
        }

        internal Genre FindOrCreate(string Name, Func<string,Genre> Cons)
        {
             Tuple<Genre,bool> res = _AllGenres.FindOrCreateValue(Name,Cons);
             if (res.Item2 == false)
                _Genres.Add(res.Item1);
             
             return res.Item1;
        }

        internal void Register(Genre genre)
        {
            _AllGenres.Register(genre);
            _Genres.Add(genre);
            genre.Session = Session;
        }

        //private static Dictionary<Tuple<Genre, Genre>, int> _Dic = new Dictionary<Tuple<Genre, Genre>, int>();
        //internal int Compare(Genre one, Genre two)
        //{
        //    if (one==two)
        //        return 0;

        //    Tuple<Genre, Genre> cg = new Tuple<Genre, Genre>(one, two);


        //    var par = _Dic.FindOrCreate(cg, c =>  one.ComputeCompare(two) );
        //    if (par.Item1==false)
        //    {
        //        _Dic.Add(new Tuple<Genre, Genre>(two, one), -par.Item2);
        //    }

        //    return par.Item2;
        //}

        private static Dictionary<SimpleCouple<Genre>, int> _Dic = new Dictionary<SimpleCouple<Genre>, int>();
        internal int Compare(Genre one, Genre two)
        {
            if (one == two)
                return 0;

            if ((one.IsEmpty) || (two.IsEmpty))
                return int.MaxValue;

            return _Dic.FindOrCreateEntity(new SimpleCouple<Genre>(one, two), c => one.ComputeCompare(two));
        }
                  
 

        internal ObservableCollection<IGenre> Genres
        {
            get
            {
                return _Genres;
            }
        }


        public void Dispose()
        {
            _AllGenres.Dispose();
        }
    }
}
