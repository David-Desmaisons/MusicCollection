using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NUnit.Framework;
using FluentAssertions;
using MusicCollectionTest.TestObjects;

using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Implementation;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    [Ignore("Life Tester not implemented - to be re-evaluated")]
    public class LifeCycleTester
    {

        //private IImportContext _IIC=null;


        //private MyObject _Un;
        //private ObjectStateChangeArgsTestor _Ex;
        //private ObjectLocker _ol;
        //private ObjectLocker _ol2;
        //private ObjectLocker _ol3;
        //private ObjectLocker _ol4;

        [SetUp]
        public void SetUp()
        {
            //_Un = new MyObject("aa", 1);
            //_ol = new ObjectLocker(0, 2, _Un);
            //_ol2 = new ObjectLocker(1, 2, _Un);
            //_ol3 = new ObjectLocker(0, 2, _Un,true);
            //_ol4 = new ObjectLocker(1, 2, _Un);

            //_Ex = new ObjectStateChangeArgsTestor();
            //_Un.ObjectStateChanges += _Ex.Li;
           
        }

        private class ObjectStateChangeArgsTestor
        {
            internal ObjectStateChangeArgsTestor()
            {
            }

            private ObjectStateChangeArgs _Expected;
            internal ObjectStateChangeArgs Expected
            {
                get { return _Expected; }
                set { _Expected = value; Next = null; }
            }

            internal ObjectStateChangeArgs Next
            {
                get;
                set;
            }

            internal bool Empty
            {
                get
                {
                    return (Next == null) && (Expected == null);
                }
            }

            internal void Li(object sender,  ObjectStateChangeArgs os)
            {
                os.ShouldHave().AllProperties().EqualTo(Expected);
                //Assert.That(os.Equals(Expected), Is.True);
                Expected = Next;
                Next = null;
            }
        }

        //private class ObjectLocker
        //{
        //    private int _t;
        //    private int _t2;
        //    private MyObject _O;
        //    private int _c;
        //    private bool _Deleter;

        //    static private int _C = 0;

        //    internal bool ExpectedNonNullResult
        //    {
        //        get;
        //        set;
        //    }

        //    internal ObjectLocker(int t, int t2, MyObject o, bool iDeleter=false)
        //    {
        //        ExpectedNonNullResult = true;
        //        _t = t * 3000;
        //        _t2 = t2* 1500;
        //        _O = o;
        //        _c = ++_C;
        //        _Deleter = iDeleter;
        //    }

        //    internal void Run()
        //    {
        //        Console.WriteLine("Entering Locker {0}, time {1}, time {2}",_c,_t,_t2);
        //        Thread.Sleep(_t);
        //        Console.WriteLine("Entering Requesting Object Lock{0}",_c);
        //        using (LockerResult l = _O.GetAvailabilityLocker(null,false))
        //        {
        //            if (l.Diagnostic == LockerStatus.NotAvailable)
        //            {
        //                Assert.That(_O.InternalState == ObjectState.Removed);
        //                Assert.That(ExpectedNonNullResult, Is.False);
        //                return;
        //            }
        //            else
        //            {
        //                if (l.Diagnostic == LockerStatus.OK)
        //                {
        //                    Assert.That(ExpectedNonNullResult, Is.True);
        //                    Assert.That(_O.InternalState == ObjectState.UnderEdit);
        //                }
        //                else
        //                    Assert.That(false, Is.True);
        //            }

        //            Console.WriteLine("Getting Object Lock{0}", _c);
        //            Thread.Sleep(_t2);
        //            if (_Deleter)
        //                l.Remove();
        //            Thread.Sleep(_t2 );
        //        }
        //        Console.WriteLine("Lefting Object Lock{0}", _c);
        //    }
        //}

        [Test]
        public void BasicTest()
        {
            //Assert.That(_Un.InternalState == ObjectState.Available);

            //_Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);
            //_Ex.Next = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.Available);

            //Thread t2 = new Thread(() => _ol2.Run());
            //_ol.Run();

            //_Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);
            //_Ex.Next = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.Available);
            //t2.Start();

            //t2.Join();

            //Assert.That(_Un.InternalState == ObjectState.Available);
            //Assert.That(_Ex.Empty, Is.True);

        }


        [Test]
        public void BasicTest2()
        {
            //Assert.That(_Un.InternalState == ObjectState.Available);

            //_Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);
            //_Ex.Next = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.Available);

            //Thread t2 = new Thread(() => _ol2.Run()); 
            //t2.Start();
            //_ol.Run();

            //t2.Join();

            //Assert.That(_Un.InternalState == ObjectState.Available);
            //Assert.That(_Ex.Empty, Is.True);
        }

        [Test]
        public void BasicTest3Remove()
        {
            //Assert.That(_Un.InternalState == ObjectState.Available);

            //_Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);
            //_Ex.Next = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.Removed);

            //_ol2.ExpectedNonNullResult = false;
            //_ol4.ExpectedNonNullResult = false;

            //Thread t2 = new Thread(() => _ol2.Run());
            //t2.Start();
            //_ol3.Run();

            //Thread t3 = new Thread(() => _ol4.Run());
            //t3.Start();

            //t2.Join();
            //t3.Join();

            //Assert.That(_Un.InternalState == ObjectState.Removed);
            //Assert.That(_Ex.Empty, Is.True);
        }

        [Test]
        public void BasicTest5()
        {
            //Assert.That(_Un.InternalState == ObjectState.Available);

            //_Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);
            //_Ex.Next = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.Available);

            //using (LockerResult ol = _Un.GetAvailabilityLocker(_IIC,true))
            //{
            //    Assert.That(ol.Diagnostic, Is.EqualTo(LockerStatus.OK));
            //    Assert.That(_Un.InternalState == ObjectState.UnderEdit);

            //    LockerResult ol2 = _Un.GetAvailabilityLocker(_IIC,true);
            //    Assert.That(ol2.Diagnostic, Is.EqualTo(LockerStatus.AlreadyLock_InstaneousRequest));

            //    LockerResult ol3 = _Un.GetAvailabilityLocker(_IIC,false);
            //    Assert.That(ol3.Diagnostic, Is.EqualTo(LockerStatus.AlreadyLock_SameThread));
            //}
        }


        [Test]
        public void BasicTestRemove4()
        {
            //Assert.That(_Un.InternalState == ObjectState.Available);

            //_Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);


            //using (LockerResult ol = _Un.GetAvailabilityLocker(_IIC,true))
            //{
            //    Assert.That(ol.Diagnostic, Is.EqualTo(LockerStatus.OK));
            //    Assert.That(_Un.InternalState == ObjectState.UnderEdit);

            //    LockerResult ol2 = _Un.GetAvailabilityLocker(_IIC,true);
            //    Assert.That(ol2.Diagnostic, Is.EqualTo(LockerStatus.AlreadyLock_InstaneousRequest));

            //    LockerResult ol3 = _Un.GetAvailabilityLocker(_IIC,false);
            //    Assert.That(ol3.Diagnostic, Is.EqualTo(LockerStatus.AlreadyLock_SameThread));

            //    ObjectState res = _Un.Break();
            //    Assert.That(res,Is.EqualTo(ObjectState.UnderEdit));
            //    Assert.That(_Un.UpdatedState, Is.EqualTo(ObjectState.UnderEdit));

            //    _Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.FileNotAvailable);
            //}

            //Assert.That(_Un.InternalState, Is.EqualTo(ObjectState.FileNotAvailable));
            //Assert.That(_Un.UpdatedState, Is.EqualTo(ObjectState.FileNotAvailable));
        }


        [Test]
        public void BasicTestOtherRemove()
        {
            //    Assert.That(_Un.InternalState == ObjectState.Available);

            //    _Ex.Expected = new ObjectStateChangeArgs(_Un, ObjectState.Available, ObjectState.UnderEdit);
            //    _Ex.Next = new ObjectStateChangeArgs(_Un, ObjectState.UnderEdit, ObjectState.Removed);

            //    using (LockerResult ol = _Un.GetAvailabilityLocker(_IIC,true))
            //    {
            //        Assert.That(ol.Diagnostic, Is.EqualTo(LockerStatus.OK));
            //        Assert.That(_Un.InternalState == ObjectState.UnderEdit);

            //        LockerResult ol2 = _Un.GetAvailabilityLocker(_IIC,false);
            //        Assert.That(ol2.Diagnostic, Is.EqualTo(LockerStatus.AlreadyLock_SameThread));


            //        ol.Remove();
            //    }
            //}
        }
    }
}
