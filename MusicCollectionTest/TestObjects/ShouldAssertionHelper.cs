using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using System.Collections;
using FluentAssertions.Events;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace MusicCollectionTest.TestObjects
{
    public static class ShouldAssertionHelper
    {
        public static void ShoulBeACoherentList<T>(this IList<T> @this)
        {
            @this.Count().Should().Be(@this.Count);
            foreach (T el in @this)
            {
                @this.Contains(el).Should().BeTrue();
                @this.Should().Contain(el);
                int index = @this.IndexOf(el);
                index.Should().BeGreaterOrEqualTo(0);
                @this[index].Should().Be(el);
            }

            if (@this.Count == 0)
                return;

            T[] s1 = new T[@this.Count];
            @this.CopyTo(s1, 0);
            s1.Should().Equal(@this);

            IEnumerable nong = @this;
            IEnumerable<T> g = @this;
            g.Should().Equal(nong);
        }

        public static void ShoulBeACoherentNonGenericList(this IList @this)
        {
            @this.Should().NotBeNull();
            @this.Cast<object>().Count().Should().Be(@this.Count);
            foreach (object el in @this)
            {
                @this.Contains(el).Should().BeTrue();
                @this.Should().Contain( new object[] {el});
                int index = @this.IndexOf(el);
                index.Should().BeGreaterOrEqualTo(0);
                @this[index].Should().Be(el);
            }

            if (@this.Count == 0)
                return;

            object[] s1 = new object[@this.Count];
            @this.CopyTo(s1, 0);
            s1.Should().Equal(@this);
        }

        public static void ShouldBeReadOnlyCollection(this IList list)
        {
            Action ad = () => list.Add(new MyObject("a", 1));
            ad.ShouldThrow<Exception>();

            Action rem = () => list.Remove(new MyObject("a", 1));
            rem.ShouldThrow<Exception>();

            Action rem2 = () => list.RemoveAt(0);
            rem2.ShouldThrow<Exception>();

            Action ins = () => list.Insert(0, new MyObject("a", 1));
            ins.ShouldThrow<Exception>();


            Action set = () => list[0] = new MyObject("a", 1);
            set.ShouldThrow<Exception>();

            Action clr = () => list.Clear();
            clr.ShouldThrow<Exception>();

            list.IsReadOnly.Should().BeTrue();

        }

        public static void ShouldBeReadOnlyCollection_Generic<T>(this IList<T> list)
        {
            Action ad = () => list.Add(default(T));
            ad.ShouldThrow<Exception>();

            Action rem = () => list.Remove(default(T));
            rem.ShouldThrow<Exception>();

            Action rem2 = () => list.RemoveAt(0);
            rem2.ShouldThrow<Exception>();

            Action ins = () => list.Insert(0, default(T));
            ins.ShouldThrow<Exception>();

            Action set = () => list[0] = default(T);
            set.ShouldThrow<Exception>();

            Action clr = () => list.Clear();
            clr.ShouldThrow<Exception>();

            list.IsReadOnly.Should().BeTrue();

        }

        public static void ShouldHaveSameElements<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            @this.Count().Should().Be(other.Count());

            if (@this.Any() == false)
            {
                other.Should().BeEmpty();
                return;
            }

            @this.Should().IntersectWith(other);
            other.Should().IntersectWith(@this);
        }

        public static void ShouldBeSameAs(this object @this, object source)
        {
            object.ReferenceEquals(@this, source).Should().BeTrue();
        }

        public static IEventRecorder ShouldRaiseCollectionEvent(this INotifyCollectionChanged @EventRaiser, Expression<Func<NotifyCollectionChangedEventArgs, bool>> predicate)
        {
            return @EventRaiser.ShouldRaise("CollectionChanged").WithArgs<NotifyCollectionChangedEventArgs>(predicate);
        }

    }
}
