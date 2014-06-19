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
using MusicCollection.ToolBox.FunctionListener;
using System.ComponentModel;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class FunctionResultListenerFactoryTestor
    {
        private interface CLFInterface:IDisposable,INotifyPropertyChanged
        {
            MyObject Item { get; set; }

            bool IsMyneOurs { get; }
        }

        private class CLF0 : NotifyCompleteListenerObject, CLFInterface
        {
            static private readonly MyObject _Ref;

            static public MyObject RefObject
            {
                get { return _Ref; }
            }

            private MyObject _Item;
            public MyObject Item
            {
                get { return _Item; }
                set
                {
                    this.Set(ref _Item,value);
                }
            }

            public bool IsMyneOurs
            {
                get { return this.Get<CLF0,bool>(() => (t) => (t.Item == RefObject.Friend));}
            }

            static CLF0()
            {
                _Ref = new MyObject("aa", 1);
                _Ref.Friend = _Ref;
            }
        }

        //private class CLF : NotifyCompleteListenerObject, CLFInterface
        //{
        //    static private readonly IResultListenerFactory<CLF, bool> _IsMyOurs;
        //    static private readonly MyObject _Ref;

        //    static public  MyObject RefObject
        //    {
        //        get { return _Ref; }
        //    }

        //    private MyObject _Item;
        //    public MyObject Item
        //    {
        //        get { return _Item; }
        //        set
        //        {
        //            if (object.ReferenceEquals(_Item, value))
        //                return;

        //            var old = _Item;
        //            _Item = value;
        //            this.PropertyHasChanged("Item", old, _Item);
        //        }
        //    }

        //    public bool IsMyneOurs
        //    {
        //        get { return this.GetValue(_IsMyOurs); }
        //    }

        //    static public FunctionResultListenerCompleteFactory<CLF, bool> FunctionResultListenerCompleteFactory
        //    {
        //        get { return _IsMyOurs as FunctionResultListenerCompleteFactory<CLF, bool>; }
        //    }


        //    //static CLF()
        //    //{
        //    //    _Ref = new MyObject("aa", 1);
        //    //    _Ref.Friend = _Ref;
        //    //    _IsMyOurs = ListenerFunctionBuilder.Register<CLF, bool>((t) => (t.Item == RefObject.Friend), "IsMyneOurs");
        //    //}
        //}



        private CLFInterface _CLF;
        private List<PropertyChangedEventArgs> _Event;

        [SetUp]
        public void SetUp()
        {
            _CLF = new CLF0();
            _Event = new List<PropertyChangedEventArgs>();
            CLF0.RefObject.Friend = CLF0.RefObject;
        }

        [TearDown]
        public void TearDown()
        {
            _CLF.Dispose();
            _Event = null;
        }


        [Test]
        public void Test_Basic()
        {
            //CLF0.FunctionResultListenerCompleteFactory.Should().NotBeNull();
            _CLF.Item.Should().BeNull();
            CLF0.RefObject.Should().NotBeNull();
            _CLF.IsMyneOurs.Should().BeFalse();
        }

        [Test]
        public void Test_Simple()
        {
            _CLF.MonitorEvents();
            _CLF.IsMyneOurs.Should().BeFalse();
            _CLF.Item = CLF0.RefObject;
            _CLF.ShouldRaisePropertyChangeFor(c => c.IsMyneOurs);
            _CLF.IsMyneOurs.Should().BeTrue();
        }

        [Test]
        public void Test_Simple_2()
        {
            _CLF.MonitorEvents();
            _CLF.IsMyneOurs.Should().BeFalse();
            CLF0.RefObject.Friend=null;
            _CLF.ShouldRaisePropertyChangeFor(c => c.IsMyneOurs);
            _CLF.IsMyneOurs.Should().BeTrue();
        }

        [Test]
        public void Test_Simple_NoChangesAfterDispose()
        {
            _CLF.IsMyneOurs.Should().BeFalse();
            _CLF.MonitorEvents();
            _CLF.Dispose();
            CLF0.RefObject.Friend = null;
            _CLF.ShouldNotRaisePropertyChangeFor(c => c.IsMyneOurs);
            _CLF.IsMyneOurs.Should().BeTrue();
            CLF0.RefObject.Friend = CLF0.RefObject;
            _CLF.IsMyneOurs.Should().BeFalse();
        }

        [Test]
        public void Test_Simple_NoChangesAfterDispose_Nolistener()
        {
            _CLF.IsMyneOurs.Should().BeFalse();
            _CLF.Dispose();
            CLF0.RefObject.Friend = null;
            _CLF.IsMyneOurs.Should().BeTrue();
        }

        [Test]
        public void Test_Simple_NoChangesAfterDispose_DynamicRegister()
        {
            _CLF.IsMyneOurs.Should().BeFalse();
            _CLF.PropertyChanged += _CLFL_PropertyChanged;
            _CLF.Item = CLF0.RefObject;
            _Event.Count.Should().Be(2);
            _Event.Select(e=>e.PropertyName).Should().Contain("IsMyneOurs");
            _CLF.IsMyneOurs.Should().BeTrue();
            _CLF.PropertyChanged -= _CLFL_PropertyChanged;

            _CLF.Item = null;
            _CLF.IsMyneOurs.Should().BeFalse();
        }

        private void _CLFL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _Event.Add(e);
        }

    }
}
