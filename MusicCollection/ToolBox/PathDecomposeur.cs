using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.ToolBox.Collection;

namespace MusicCollection.ToolBox
{
    internal class PathDecomposeur : IComparable
    {
        private static int _Count = 0;
        private int _Rank;
        private PathDecomposeur(PathDecomposeur iFather, String Name)
        {
            _Rank = _Count++;
            LocalPath = Name;
            Father = iFather;
            if (Father != null)
                Father.Register(this);
            else
                Roots.Add(Name, this);
        }

        static IDictionary<string, PathDecomposeur> _Roots;
        static private IDictionary<string, PathDecomposeur> Roots
        {
            get
            {
                if (_Roots == null)
                    _Roots = new SortedList<string, PathDecomposeur>();

                return _Roots;
            }
        }

        internal PathDecomposeur Father
        {
            get;
            set;
        }

     


        static internal PathDecomposeur FromName(string Name)
        {
            if (Name == null)
                return null;

            string[] res = Name.ToLower().Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            IDictionary<string, PathDecomposeur> Finder = null;// Roots;
            PathDecomposeur MyPD = null;

            bool newi = false;

            foreach (string r in res)
            {
                if (Finder == null)
                    Finder = Roots;
                else
                {
                    if (MyPD == null)
                        throw new Exception("Algo error");

                    Finder = MyPD.Childs;
                }

                PathDecomposeur old = MyPD;
                if (newi || !Finder.TryGetValue(r, out MyPD))
                {
                    MyPD = new PathDecomposeur(old, r);
                    newi = true;
                }

            }

            return MyPD;
        }

        private StringBuilder FullNameBuilder
        {
            get { return (Father == null) ? new StringBuilder(LocalPath) : Father.FullNameBuilder.Append('\\').Append(LocalPath); }
        }

        internal string FullName
        {
            get { return FullNameBuilder.ToString(); }
        }

        internal string LocalPath
        { get; private set; }

        public override string ToString()
        {
            return FullName;
        }


        private IDictionary<string, PathDecomposeur> _Childs;
        private IDictionary<string, PathDecomposeur> Childs
        {
            get
            {
                if (_Childs == null)
                    _Childs = new PolyMorphDictionary<string, PathDecomposeur>();

                return _Childs;
            }
        }

        private void Register(PathDecomposeur Child)
        {
            if (Child == null)
                throw new ArgumentNullException();


            Childs.Add(Child.LocalPath, Child);
        }

        public int CompareTo(object obj)
        {
            PathDecomposeur ot = obj as PathDecomposeur;
            if (ot == null)
                return 1;

            if (object.ReferenceEquals(ot,this))
                return 0;

            return _Rank.CompareTo(ot._Rank);
        }


        //internal IEnumerable<PathDecomposeur> Fathers
        //{
        //    get
        //    {
        //        if (Father != null)
        //        {
        //            yield return Father;
        //            foreach (PathDecomposeur dp in Father.Fathers)
        //            {
        //                yield return dp;
        //            }
        //        }
        //    }
        //}
        //private static Tuple<PathDecomposeur, PathDecomposeur> PointOfComparison(PathDecomposeur first, PathDecomposeur second)
        //{
        //    var FFathers = first.Fathers.ToList();
        //    FFathers.Add(null);
        //    var SFathers = second.Fathers.ToList();
        //    SFathers.Add(null);

        //    for (int j = 0; j < Math.Max(FFathers.Count, SFathers.Count); j++)
        //    {
        //        if (FFathers[j] != SFathers[j])
        //            return new Tuple<PathDecomposeur, PathDecomposeur>(FFathers[j], SFathers[j]);
        //    }
        //}

        //public int CompareTo(object obj)
        //{
        //    PathDecomposeur ot = obj as PathDecomposeur;
        //    if (ot == null)
        //        return 1;

        //    if (ot == this)
        //        return 0;

        //    Tuple<PathDecomposeur, PathDecomposeur> Pof = PointOfComparison(this, ot);

        //    if (Pof.Item1 == null)
        //        return 1;

        //    if (Pof.Item2 == null)
        //        return -1;

        //    return Pof.Item1.LocalPath.CompareTo(Pof.Item2.LocalPath);

        //    //var cachedFathers = Fathers.ToList();
        //    //var cachedFathersot = ot.Fathers.ToList();

        //    //var commun = Fathers.Intersect(cachedFathersot).FirstOrDefault();

        //    //int i = 0;
        //    //int io = 0;

        //    //if (commun == null)
        //    //{
        //    //    i = cachedFathers.IndexOf(commun) - 1;
        //    //    io = cachedFathersot.IndexOf(commun) - 1;
        //    //}
        //    //else
        //    //{
        //    //    i = cachedFathers.Count - 1;
        //    //    io = cachedFathersot.Count - 1;
        //    //}


        //    //return cachedFathers[i].LocalPath.CompareTo(cachedFathersot[io].LocalPath);
        //}
    }
}
