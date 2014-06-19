using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit;
using NUnit.Framework;
using FluentAssertions;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Infra")]
    [NUnit.Framework.Category("Unitary")]
    public class ListExtensionTestor
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AddCollection_null()
        {
            IList<int> target = null;
            IList<int> other = new List<int>();
            Action wt = () => target.AddCollection(other);
            wt.ShouldThrow<Exception>();
        }

        [Test]
        public void AddCollection_addnull()
        {
            IList<int> target = new List<int>();
            IList<int> other = null;
            ((object)target.AddCollection(other)).Should().Be(target);
        }


        [Test]
        public void AddCollection_normal()
        {
            IList<int> target = new List<int>();
            target.Add(0); target.Add(1);
            IList<int> other = new List<int>();
            target.Add(2); target.Add(3);

            var res = target.AddCollection(other);
            res.Should().Equal(0, 1, 2, 3);
        }

        [Test]
        public void Randomize_Capacity_Basic_NoGood()
        {
            List<int> target = new List<int> { 0,1,2,3,4};
            target.Randomize(10).Should().BeNull();
        }

        [Test]
        public void Randomize_Capacity_Basic()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4 };
            IList<int> res = target.Randomize(5);
            res.Should().NotBeNull();
            res.Should().BeEquivalentTo(target);
            res.Should().NotEqual(target);
        }

        [Test]
        public void Randomize_Capacity_Basic_Big()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4,5,6,7,8,9,10,11,12,13,15,16,17,18 };
            IList<int> res = target.Randomize(2);
            res.Should().NotBeNull();
            res.Should().BeSubsetOf(target);
            res.Count.Should().Be(2);
        }


        [Test]
        public void Randomize_Randomize_T()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            IEnumerable<int> res = target.Randomize();
            res.Should().NotBeNull();
            res.Should().BeEquivalentTo(target);
            res.Should().NotEqual(target);
        }

        [Test]
        public void Randomize_Randomized_ItemT()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int res = target.RandomizedItem();
            target.Should().Contain(res);
        }

        [Test]
        public void Randomize_Randomized_ItemT_IEnumerable()
        {
            IEnumerable<int> target = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int res = target.RandomizedItem();
            target.Should().Contain(res);
        }

        [Test]
        public void SingleItemCollection_null()
        {
            object on = null;
            on.SingleItemCollection().Should().BeEmpty();
        }

        [Test]
        public void SingleItemCollection_standard()
        {
            int on = 1;
            on.SingleItemCollection().Should().Equal(1);
        }


        [Test]
        public void Randomize_Randomized_ItemT_Null()
        {
            List<int> target = null;
            int res = 0;
            Action ac = () => res = target.RandomizedItem();
            ac.ShouldThrow<ArgumentNullException>();
        }

         [Test]
        public void Randomize_Randomized_ItemT_Empty()
        {
            List<string> target = new List<string> ();
            string res = target.RandomizedItem();
            res.Should().BeNull();
        }


         [Test]
         public void Randomize_Randomized_ItemT_Null_IEnumerable()
         {
             IEnumerable<int> target = null;
             int res = 0;
             Action ac = () => res = target.RandomizedItem();
             ac.ShouldThrow<ArgumentNullException>();
         }


        [Test]
        public void Index()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            target.Index(21).Should().Be(-1);
            target.Index(0).Should().Be(0);
            target.Index(3).Should().Be(3);
        }

        [Test]
        public void OrberWithIndexBy_First()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var res = target.OrberWithIndexBy((i, j) => -i);
            res.Should().BeEquivalentTo(target);
            res.Reverse().Should().Equal(target);
        }

        [Test]
        public void OrberWithIndexBy_Second()
        {
            List<int> target = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var res = target.OrberWithIndexBy((i, j) => i);
            res.Should().Equal(target);
        }

        [Test]
        public void OrberWithIndexBy_Third()
        {
            List<int> target = new List<int> { 1, 13, 14, 21, 61, 19, 10, 71, 661, 6661 };
            var res = target.OrberWithIndexBy((i, j) => j);
            res.Should().Equal(target.OrderBy(i=>i));
        }

         [Test]
        public void Apply_First()
        {
            List<int> target = null;
            var res = target.Apply(i => Console.WriteLine(i));
            res.Should().BeNull();
        }

         [Test]
         public void Apply_Second()
         {
             List<int> target = new List<int> { 1, 13, 14, 21, 61, 19, 10, 71, 661, 6661 };
             List<int> expected=new List<int>();
             object res = target.Apply(i => expected.Add(i));
             res.Should().Be(target);
             expected.Should().Equal(target);
         }

         [Test]
         public void SequenceEqual_First()
         {
             List<int> target = new List<int> { 1, 13, 14, 21, 61, 19, 10, 71, 661, 6661 };
             List<int> target2 = new List<int> { 1, 13, 14, 21, 61, 19, 10, 71, 661, 6661 };
             target.SequenceEqual(target2, (i, j) => i == j).Should().BeTrue();
         }

         [Test]
         public void SequenceEqual_Second()
         {
             List<int> target = new List<int> { 1, 13, 14,21 };
             List<int> target2 = new List<int> { 1, 13, 14, 21, 61, 19, 10, 71, 661, 6661 };
             target.SequenceEqual(target2, (i, j) => i == j).Should().BeFalse();
         }

         [Test]
         public void SequenceEqual_Third()
         {
             List<int> target = new List<int> { 1, 2, 3, 4};
             List<int> target2 = new List<int> { 2, 3, 4, 5};
             target.SequenceEqual(target2, (i, j) => i == j - 1).Should().BeTrue();
             target.SequenceEqual(target2, (i, j) => i == j).Should().BeFalse();
         }

         [Test]
         public void BinarySearch_In()
         {
             IList<int> target = new List<int> { 0, 2, 4, 8 };

             target.BinarySearch(0).Should().Be(0);
             target.BinarySearch(2).Should().Be(1);
             target.BinarySearch(4).Should().Be(2);
             target.BinarySearch(8).Should().Be(3);

             target.BinarySearch(-1).Should().Be(~0);

             target.BinarySearch(1).Should().Be(~1);
             target.BinarySearch(3).Should().Be(~2);
             target.BinarySearch(5).Should().Be(~3);
             target.BinarySearch(7).Should().Be(~3);

             target.BinarySearch(9).Should().Be(~4);


         }

         [Test]
         public void InternalBinarySearch_ArgumentNullException()
         {
             IList<int> target = null;

             Action ac = () => target.BinarySearch(9);
             ac.ShouldThrow<ArgumentNullException>();
         }
        
    }
}
