using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox
{

    internal static class SimpleLookImplExtender
    {
        internal static void ShoulBeCoherent<T1, T2>(this ILookup<T1, T2> @this)
        {
            @this.Count().Should().Be(@this.Count);
            foreach (IGrouping<T1, T2> g in @this)
            {
                var values = @this[g.Key];
                values.Should().Equal(g);
            }
        }

        internal static void ShoulBeSimilar<T1, T2>(this ILookup<T1, T2> @this, ILookup<T1, T2> other)
        {
            @this.Count().Should().Be(other.Count());
            @this.Count.Should().Be(other.Count);
            
            //@this.Should().BeEquivalentTo(other);

            foreach (IGrouping<T1, T2> g in @this)
            {
                var otherkeys = other[g.Key];
                otherkeys.Should().BeEquivalentTo(g);
            }

            foreach (IGrouping<T1, T2> g in other)
            {
                var otherkeys = @this[g.Key];
                otherkeys.Should().BeEquivalentTo(g);
            }
        }

        internal static void ShoulBeAddCorrectly(this SimpleLookImpl<string, MyObject> @this, MyObject iel, List<MyObject> list)
        {
            @this.ShoulBeCoherent();
            //int c = @this.Count;
            @this.Add(iel);
            //@this.Count.Should().Be(c + 1);
            @this.Contains(iel.Name).Should().BeTrue();

            IEnumerable<MyObject> res = @this[iel.Name];
            res.Should().NotBeNull();
            @this[iel.Name].Should().Contain(iel);
            list.Add(iel);

            @this.ShoulBeCoherent();
        }

        internal static void ShouldRemoveCorrectlyAsAlreadyAdded(this SimpleLookImpl<string, MyObject> @this, MyObject iel, List<MyObject> list)
        {
            @this.ShoulBeCoherent();

            @this.Contains(iel.Name).Should().BeTrue();
            IEnumerable<MyObject> res = @this[iel.Name];
            res.Should().NotBeNull();
            @this[iel.Name].Should().Contain(iel);

            //int c = @this.Count;
            @this.Remove(iel);
            //@this.Count.Should().Be(c - 1);
           

            IEnumerable<MyObject> res2 = @this[iel.Name];
            res2.Should().NotBeNull();
            res2.Should().NotContain(iel);
            list.Remove(iel);

            @this.ShoulBeCoherent();

        }

        internal static void ShoulRemoveCorrectlyAsNotAddedAlreadyAdded(this SimpleLookImpl<string, MyObject> @this, MyObject iel)
        {
            @this.ShoulBeCoherent();

            int c = @this.Count;
            @this.Remove(iel);
            @this.Count.Should().Be(c);

            IEnumerable<MyObject> res2 = @this[iel.Name];
            res2.Should().NotBeNull();
            @this[iel.Name].Should().NotContain(iel);

            @this.ShoulBeCoherent();
        }
    }

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class SimpleLookImplTestor
    {
        private void CheckExpectations(SimpleLookImpl<string,MyObject> sli,List<MyObject> objects)
        {
            ILookup<string, MyObject> expected = objects.ToLookup(o => o.Name);
            sli.ShoulBeSimilar(expected);
        }


        [Test]
        public void Test0()
        {
            SimpleLookImpl<string,MyObject> sli = 
                new SimpleLookImpl<string,MyObject>(o=>o.Name, ()=> new Dictionary<string,List<MyObject>>());

            sli.Should().BeEmpty();
            sli.Count.Should().Be(0);

            List<MyObject> objects = new List<MyObject>();

            MyObject f = new MyObject("Toto",20);
            

            sli.ShoulBeAddCorrectly(f,objects);
            CheckExpectations(sli, objects);
            sli.Count.Should().Be(1);

            MyObject f2 = new MyObject("Titi", 20);
            
            sli.ShoulBeAddCorrectly(f2,objects);
            CheckExpectations(sli,objects);
            sli.Count.Should().Be(2);

            MyObject f3 = new MyObject("Toto", 50);

            sli.ShoulBeAddCorrectly(f3,objects);
            CheckExpectations(sli, objects);
            sli.Count.Should().Be(2);


            sli.ShouldRemoveCorrectlyAsAlreadyAdded(f, objects);
            CheckExpectations(sli, objects);
            sli.Count.Should().Be(2);


            sli.ShouldRemoveCorrectlyAsAlreadyAdded(f3, objects);
            CheckExpectations(sli, objects);
            sli.Count.Should().Be(1);

            sli.ShoulBeAddCorrectly(f, objects);
            CheckExpectations(sli, objects);
            sli.Count.Should().Be(2);

            sli.ShoulBeAddCorrectly(f3, objects);
            CheckExpectations(sli, objects);
            sli.Count.Should().Be(2);


        }
    }
}
