using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    //public class Couple<T> : Tuple<T, T>, IComparable<Couple<T>> where T : IComparable
    ////<T>
    //{
    //    public Couple(T first, T second)
    //        : base(Max(first, second), Min(first, second))
    //    {
    //    }


    //    private static T Max(T f, T s)
    //    {
    //        return (f.CompareTo(s) >= 0) ? f : s;
    //    }

    //    private static T Min(T f, T s)
    //    {
    //        return (f.CompareTo(s) < 0) ? f : s;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        Couple<T> o = obj as Couple<T>;
    //        if (o == null)
    //            return false;

    //        return ((object.Equals(Item1, o.Item1) && object.Equals(Item2, o.Item2)));
    //        //||                       (object.Equals(Item2, o.Item1) && object.Equals(Item1, o.Item2)));

    //    }

    //    public override int GetHashCode()
    //    {
    //        return ((Item1 == null) ? 0 : Item1.GetHashCode()) ^ ((Item2 == null) ? 0 : Item2.GetHashCode());
    //    }

    //    public override string ToString()
    //    {
    //        return string.Format("Couple {0}:{1}", Item1, Item2);
    //    }

    //    public int CompareTo(object other)
    //    {
    //        Couple<T> ct = other as Couple<T>;
    //        IComparable<Couple<T>> @this = this;
    //        return @this.CompareTo(ct);
    //    }

    //    int IComparable<Couple<T>>.CompareTo(Couple<T> other)
    //    {
    //        IComparable ct = this;
    //        return ct.CompareTo(other);
    //    }
    //}

    public class SimpleCouple<T> : Tuple<T, T>
    {
        public SimpleCouple(T first, T second)
            : base(first, second)
        {
        }


        public override bool Equals(object obj)
        {
            SimpleCouple<T> o = obj as SimpleCouple<T>;
            if (o == null)
                return false;

            return ((object.Equals(Item1, o.Item1) && object.Equals(Item2, o.Item2)))
                || (object.Equals(Item2, o.Item1) && object.Equals(Item1, o.Item2));

        }

        public override int GetHashCode()
        {
            return ((Item1 == null) ? 0 : Item1.GetHashCode()) + ((Item2 == null) ? 0 : Item2.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("SimpleCouple <{0}:{1}>", Item1, Item2);
        }
    }
}
