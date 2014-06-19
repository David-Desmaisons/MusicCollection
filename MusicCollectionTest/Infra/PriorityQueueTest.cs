using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using FakeItEasy;
using NSubstitute;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollectionTest.TestObjects;
using System.ServiceModel;
using MusicCollection.Infra.Collection;
using System.Collections;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class PriorityQueueTest
    {
        //[Serializable()]
        //public class ReversePriority : IComparable
        //{
        //    private int priority;

        //    public ReversePriority(int priority)
        //    {
        //        this.priority = priority;
        //    }

        //    public int CompareTo(object obj)
        //    {
        //        ReversePriority castObj = obj as ReversePriority;
        //        if (castObj == null)
        //            throw new InvalidCastException();
        //        if (priority < castObj.priority)
        //            return 1;
        //        if (priority == castObj.priority)
        //            return 0;
        //        return -1;
        //    }
        //}

        private class CompareMyObject : IComparer<MyObject>
        {
            public int Compare(MyObject x, MyObject y)
            {
                return x.Value - y.Value;
            }
        }

        PriorityQueue<MyObject> pq;

        [SetUp]
        public void SetUp()
        {
            pq = new PriorityQueue<MyObject>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void TestComparer()
        {
            pq.ItemComparer.Should().Be(Comparer<MyObject>.Default);
        }

        [Test]
        public void TestEnqueueing()
        {
            pq.Count.Should().Be(0, "Enqueuing first item should set count to 1");

            pq.Enqueue(new MyObject("item one", 12));
            pq.Count.Should().Be(1, "Enqueuing first item should set count to 1");

            pq.Enqueue(new MyObject("item two", 5));
            pq.Count.Should().Be(2, "Enqueuing second item should set count to 2");

            pq.Enqueue(new MyObject("item three", 5));
            pq.Count.Should().Be(3, "Enqueuing third item should set count to 3");
        }


        [Test]
        public void TestPeek_Exception()
        {
            Action ac = () => pq.Peek();
            ac.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void TestPeek()
        {
            var io = new MyObject("item one", 12);
            var it = new MyObject("item two", 5);
            pq.Enqueue(io);
            pq.Enqueue(it);

            MyObject s = pq.Peek();
            s.Should().Be(it, "Peeking should retrieve second item");
            pq.Count.Should().Be(2, "Peek item should set count to 2");

            s = pq.Dequeue();
            s.Should().Be(it, "Dequeuing should retrieve second item");
            pq.Count.Should().Be(1, "Dequeuing item should set count to 1");

            MyObject s2 = pq.Peek();
            s2.Should().Be(io, "Dequeuing should retrieve first item");
            pq.Count.Should().Be(1, "Dequeuing item should set count to 0");
            
            s2 = pq.Dequeue();
            s2.Should().Be(io, "Dequeuing should retrieve first item");
            pq.Count.Should().Be(0, "Dequeuing item should set count to 0");

            Action ac = () => pq.Dequeue();
            ac.ShouldThrow<InvalidOperationException>();

        }

        [Test]
        public void TestDequeueing()
        {
            var io = new MyObject("item one", 12);
            var it = new MyObject("item two", 5);
            pq.Enqueue(io);
            pq.Enqueue(it);

            MyObject s = pq.Dequeue();
            s.Should().Be(it, "Dequeuing should retrieve second item");
            pq.Count.Should().Be(1, "Dequeuing item should set count to 1");

            MyObject s2 = pq.Dequeue();
            s2.Should().Be(io, "Dequeuing should retrieve first item");
            pq.Count.Should().Be(0, "Dequeuing item should set count to 0");

            Action ac = () => pq.Dequeue();
            ac.ShouldThrow<InvalidOperationException>();

        }

        [Test]
        public void TestGrowingQueue()
        {
            PriorityQueue<MyObject> pqlocal = new PriorityQueue<MyObject>(new CompareMyObject());
            for (int i = 0; i < 15; i++)
            {
                pqlocal.Enqueue(new MyObject("item: " + i.ToString(), i * 2));
            }

            pqlocal.Count.Should().Be(15, "Enqueued 15 items, so there should be 15 there");

            pqlocal.Enqueue(new MyObject("trigger", 15));
            pqlocal.Count.Should().Be(16, "Enqueued 15 items, so there should be 15 there");

            MyObject found = null;


            for (int i = 14; i > 7; i--)
            {
                found = pqlocal.Dequeue();
                string expectedStr = "item: " + i.ToString();
                found.Name.Should().Be(expectedStr, "Dequeueing problem");
            }

            found = pqlocal.Dequeue();
            found.Name.Should().Be("trigger", "Dequeueing problem");

            for (int i = 7; i >= 0; i--)
            {
                found = pqlocal.Dequeue();
                string expectedStr = "item: " + i.ToString();
                found.Name.Should().Be(expectedStr, "Dequeueing problem");
            }
        }

        [Test]
        public void TestGrowingQueue2()
        {
            PriorityQueue<MyObject> pqlocal = new PriorityQueue<MyObject>(new CompareMyObject(),2);
            for (int i = 0; i < 15; i++)
            {
                pqlocal.Enqueue(new MyObject("item: " + i.ToString(), i * 2));
            }

            pqlocal.Count.Should().Be(15, "Enqueued 15 items, so there should be 15 there");

            pqlocal.Enqueue(new MyObject("trigger", 15));
            pqlocal.Count.Should().Be(16, "Enqueued 15 items, so there should be 15 there");

            MyObject found = null;


            for (int i = 14; i > 7; i--)
            {
                found = pqlocal.Dequeue();
                string expectedStr = "item: " + i.ToString();
                found.Name.Should().Be(expectedStr, "Dequeueing problem");
            }

            found = pqlocal.Dequeue();
            found.Name.Should().Be("trigger", "Dequeueing problem");

            for (int i = 7; i >= 0; i--)
            {
                found = pqlocal.Dequeue();
                string expectedStr = "item: " + i.ToString();
                found.Name.Should().Be(expectedStr, "Dequeueing problem");
            }
        }

        [Test]
        public void TestWrongCapacity()
        {
            PriorityQueue<MyObject> pqlocal = null;
            Action ac = () => pqlocal = new PriorityQueue<MyObject>(null,-1);

            ac.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void TestEnumerator()
        {
            CompareMyObject cp = new CompareMyObject();
            PriorityQueue<MyObject> pqlocal = new PriorityQueue<MyObject>(cp);

            pqlocal.ItemComparer.Should().Be(cp);
            //string s;
            // use a hashtable to check contents of PQ
            HashSet<MyObject> ht = new HashSet<MyObject>();
            for (int i = 0; i < 5; i++)
            {
                MyObject ob = new MyObject("item: " + i.ToString(), i * 2);

                ht.Add(ob);
                pqlocal.Enqueue(ob);
            }

            pqlocal.Should().BeEquivalentTo(ht, "Enumerable PriorityQueue");
        }

        //[ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void TestEnumeratorWithEnqueue()
        {
            var f = new MyObject("one", 42);
            pq.Enqueue(f);
            IEnumerator ie = pq.GetEnumerator();
            ie.MoveNext();
            ie.Current.Should().Be(f);
            pq.Enqueue(new MyObject("one", 42));

            Action wf = () => ie.MoveNext();                 // should fail
            wf.ShouldThrow<InvalidOperationException>();
        }

        //[ExpectedException(typeof(InvalidOperationException))]
        //[Test]
        //public void TestEnumeratorWithDequeue()
        //{
        //    pq.Enqueue("one", 42);
        //    IEnumerator ie = pq.GetEnumerator();
        //    ie.MoveNext();
        //    pq.Dequeue();
        //    ie.MoveNext(); // should fail
        //    Assertion.Fail("The previous call to MoveNext() should fail");
        //}

        [Test]
        public void TestCopyTo()
        {
            HashSet<MyObject> ht = new HashSet<MyObject>();
            for (int i = 0; i < 5; i++)
            {
                MyObject ob = new MyObject("item: " + i.ToString(), i * 2);
                ht.Add(ob);
                pq.Enqueue(ob);
            }

            MyObject[] heArray = new MyObject[6];

            pq.CopyTo(heArray, 1);

            heArray.Skip(1).Should().BeEquivalentTo(pq);
        }

        [Test]
        public void TestPriorityType()
        {
            PriorityQueue<MyObject> pqlocal = new PriorityQueue<MyObject>(new CompareMyObject().Revert());
            MyObject io = new MyObject("item one", 12);
            MyObject it = new MyObject("item two", 5);
            pqlocal.Enqueue(io);
            pqlocal.Enqueue(it);

            MyObject s = pqlocal.Dequeue();
            s.Should().Be(it, "Dequeuing should retrieve highest priority item");
            pqlocal.Count.Should().Be(1, "Dequeuing item should set count to 1");

            s = pqlocal.Dequeue();
            s.Should().Be(io, "Dequeuing should retrieve highest priority item");
            pqlocal.Count.Should().Be(0, "Dequeuing item should set count to 0");
        }

        [Test]
        public void TestPriorityType2()
        {
            PriorityQueue<MyObject> pqlocal = new PriorityQueue<MyObject>(new CompareMyObject());
            MyObject io = new MyObject("item one", 12);
            MyObject it = new MyObject("item two", 5);
            pqlocal.Enqueue(io);
            pqlocal.Enqueue(it);

            MyObject s = pqlocal.Dequeue();
            s.Should().Be(io, "Dequeuing should retrieve highest priority item");

            pqlocal.Count.Should().Be(1, "Dequeuing item should set count to 1");

            s = pqlocal.Dequeue();
            s.Should().Be(it, "Dequeuing should retrieve highest priority item");
            pqlocal.Count.Should().Be(0, "Dequeuing item should set count to 0");
        }

        [Test]
        public void GCTesting()
        {
            PriorityQueue<MyObject> pqlocal = new PriorityQueue<MyObject>(new CompareMyObject());
            MyObject io = new MyObject("item one", 12);
            WeakReference<MyObject> mio = new WeakReference<MyObject>(io);

            MyObject res = null;
            mio.TryGetTarget(out res).Should().BeTrue();
            res.Should().Be(io);
            io = null;

            {
                pqlocal.Enqueue(io);
                res = pqlocal.Dequeue();
                res = null;
            }

            pqlocal.Count.Should().Be(0);

            

            GC.Collect();
            GC.WaitForPendingFinalizers();

            res = null;
            mio.TryGetTarget(out res).Should().BeFalse();
            res.Should().BeNull();

        }
    }

}
