using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;
using NUnit.Framework;
using System.Linq.Expressions;
using MusicCollectionTest.TestObjects;
using System.Collections.ObjectModel;

using FluentAssertions;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class CollectionFunctionallyListened
    {
        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;
        private MyObject _Quatre;
        private MyObject _Cinq;
        private ObservableCollection<MyObject> _List;

        [SetUp]
        public void setUP()
        {
            _Un = new MyObject("a", 0);
            _Deux = new MyObject("b", 1);
            _Trois = new MyObject("c", 2);
            _Quatre = new MyObject("d", 3);
            _Cinq = new MyObject("e", 4);
            _List = new ObservableCollection<MyObject>();
        }

        [Test]
        public void test_0()
        {
            LambdaInspectorGenericTestor<IList<MyObject>, bool> ligt =
                new LambdaInspectorGenericTestor<IList<MyObject>, bool>( l => l.Contains(_Un), _List);

            ligt.Check.Should().BeTrue();
            ligt.CachedValue.Should().BeFalse();

            _List.Add(_Un);           
            ligt.CachedValue.Should().BeTrue(); 
            ligt.Check.Should().BeTrue();

            _List.Add(_Deux);
            ligt.CachedValue.Should().BeTrue();
            ligt.Check.Should().BeTrue();

            _List.Clear();
            ligt.CachedValue.Should().BeFalse();
            ligt.Check.Should().BeTrue();

            _List.Add(_Un);
            ligt.CachedValue.Should().BeTrue();
            ligt.Check.Should().BeTrue();

            _List.Remove(_Un);
            ligt.CachedValue.Should().BeFalse();
            ligt.Check.Should().BeTrue();
        } 
    }
}