using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class ExtensionTestor
    {

        MyObject[] _Objects = { new MyObject("Ashkenazi",1),new MyObject("Walker",1),new MyObject("Scratches",0),new MyObject("Dusty",19) } ;


        [SetUp]
        public void SU()
        {
            _Objects[0].MyFriends.Add(_Objects[0]);
            _Objects[1].MyFriends.Add(_Objects[0]); _Objects[1].MyFriends.Add(_Objects[1]);
            _Objects[2].MyFriends.Add(_Objects[0]); _Objects[2].MyFriends.Add(_Objects[3]);
            _Objects[3].MyFriends.Add(_Objects[1]);
        }
                     
        //[Test]
        //public void SingleKey()
        //{

        //    Expression<Func<MyObject, IList<MyObject>>> ac = (p) => p.MyFriends;
        //    Expression<Func<MyObject, MyObject, Tuple<MyObject, MyObject>>> acc = (a, b) => new Tuple<MyObject, MyObject>(a, b);


        //    var titi = ac.TreatLiveCollectionOutput<MyObject, MyObject, Tuple<MyObject, MyObject>>(acc);

        //    var toto = titi.Compile();

        //    var res = _Objects.SelectMany(toto);
        //    Console.WriteLine(string.Join(",", res));


        //    IFunction<MyObject, IList<Tuple<MyObject, MyObject>>> func = titi.CompileToObservable();

        //    Assert.That(func.IsDynamic, Is.True);
        //    Assert.That(func.IsParameterDynamic, Is.True);
        //    Assert.That(func.IsConstantDynamic, Is.False);
           
        //}
    }
}
