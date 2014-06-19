using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using NUnit.Framework;

using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox
{
    


    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class LambdaHelperTester
    {

        //private LambdaInspector<MyObject,string> _LH1;
        //private LambdaInspector<MyObject, string> _LH2;
        //private LambdaInspector<MyObject, string> _LH22;
        //private LambdaInspector<MyObject, int> _LH3;
        //private LambdaInspector<MyObject, int> _LH4;
        //private LambdaInspector<MyObject, string> _LH5;
        //private LambdaInspector<MyObject, string> _LH6;
        //private LambdaInspector<MyObject, int> _LH7;
        //private LambdaInspector<MyObject, string> _LH8;
        //private LambdaInspector<MyObject, string> _LH9;
        //private LambdaInspector<MyObject, string> _LH10;
        //private LambdaInspector<MyObject, string> _LH11;
        //private LambdaInspector<MyObject, string> _LH12;
        //private LambdaInspector<MyObject, string>  _LH13;
        //private LambdaInspector<MyObject, string> _LH14;
        //private LambdaInspector<MyObject,int> _LH15;
        //private LambdaInspector<MyObject,bool> _LH16;
        //private LambdaInspector<MyObject, int> _LH17;
        //private LambdaInspector<MyObject, bool> _LH18;
        //private LambdaInspector<MyObject, bool> _LH19;
        //private LambdaInspector<MyObject, string> _LH20;



        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;
        private MyObject _Quatre;
        private MyObject _Cinq;

        private class dc
        {
            private Expression<Action> _ac;
            internal dc(Expression<Action> ac)
            {
                _ac = ac;
            }

        }

        private void list(object sender, ObjectModifiedArgs arg)
        {
        }

        [SetUp]
        public void setUP()
        {
            IObjectAttribute io = _Un;

            _Un = new MyObject("a", 0);
            _Deux = new MyObject("b", 1);
            _Trois = new MyObject("c", 2);
            _Quatre = new MyObject("d", 3);
            _Cinq = new MyObject("e", 4);   
            
            //int yu = 23;

            //_LH20 = new LambdaInspector<MyObject, string>(o=> (o.Friend==null) ? string.Empty : o.Friend.Name );

            //_LH19 = new LambdaInspector<MyObject, bool>(o => o.Friend.Name.Length == _Cinq.Value);

            //_LH18 = new LambdaInspector<MyObject, bool>(o => o.Friend == _Cinq.Friend);


            //_LH15 = new LambdaInspector<MyObject, int>(o => o.Name.Length * yu + o.Value * 5 + o.Value * o.Value);

            //_LH17 = new LambdaInspector<MyObject, int>(o => o.Name.Length * (yu + o.Name.Length));


            //_LH1 = new LambdaInspector<MyObject, string>(o => o.Name);
            //_LH2 = new LambdaInspector<MyObject, string>(o => "Toolbox");
            //_LH22 = new LambdaInspector<MyObject, string>(o => string.Format("Toolbox{0}", o));
            //_LH3 = new LambdaInspector<MyObject, int>(o => o.Value);
            //_LH4 = new LambdaInspector<MyObject, int>(o => 33 * o.Value + 15);
            //_LH5 = new LambdaInspector<MyObject, string>(o => string.Format("Toolbox{0}", o.Name));

            //_LH6 = new LambdaInspector<MyObject, string>(o => Toto());


            //_LH7 = new LambdaInspector<MyObject, int>(o => yu * o.Value);
            //_LH8 = new LambdaInspector<MyObject, string>(o => Name);
            //_LH9 = new LambdaInspector<MyObject, string>(o => _Cinq.Name);
            //_LH10 = new LambdaInspector<MyObject, string>(o => o.ToString());
            //_LH11 = new LambdaInspector<MyObject, string>(o => string.Format("{1}Toolbox{0}", o.Name, o.Value));
            //_LH12 = new LambdaInspector<MyObject, string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, _Cinq.Name));

            //MyObject m2 = new MyObject("Toolbox",33);
            //_LH13 = new LambdaInspector<MyObject, string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, m2.Name));

            //_LH14 = new LambdaInspector<MyObject, string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, Name));

            //_LH16 = new LambdaInspector<MyObject, bool>(o => (o.Value == (o.Name.Length + Diff)));

        }

        private int Diff
        {
            get
            {
                return 5;
            }
        }

        private string Toto()
        {
            return "33";
        }

        private string Name
        {
            get { return "33sss"; }
        }

        //private List<ObjectAttributeWatched<MyObject, T>> FromParameter<T>(IEnumerable<ObjectAttributeWatched<MyObject, T>> ori)
        //{
        //    return  (from el in ori where el.DefiningExpression.NodeType == ExpressionType.Parameter select el).ToList();

        //}

        [Test]
        public void test00()
        {
            Expression<Func<MyObject,int>> ef = o=>24;
            IFunction<MyObject,int> ifu = ef.CompileToObservable();
            Assert.That(ifu.IsDynamic, Is.False);
            Assert.That(ifu.IsParameterDynamic, Is.False);

            Expression<Func<MyObject, int>> ef2 = o => _Un.Value;
            ifu = ef2.CompileToObservable();
            Assert.That(ifu.IsDynamic, Is.True);
            Assert.That(ifu.IsParameterDynamic, Is.False);
        }

        [Test]
        public void test31()
        {
            using (LambdaInspectorTestor<int> lib4 = new LambdaInspectorTestor<int>(o => o.Value*2 - _Un.Value, _Un))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo(0));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Un.Value = 1;
                Assert.That(lib4.CachedValue, Is.EqualTo(1));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(1));
                //Assert.That(lib4.AVEC, Is.EqualTo(1));
                Assert.That(lib4.EVC, Is.EqualTo(1));


                _Un.Value = 3;
                Assert.That(lib4.CachedValue, Is.EqualTo(3));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(2));
                Assert.That(lib4.EVC, Is.EqualTo(2));
            }
        }

        [Test]
        public void test13()
        {
            MyObject m2 = new MyObject("rou", 33);
            using (LambdaInspectorTestor<string> lib4 = new LambdaInspectorTestor<string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, m2.Name), _Un))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo("a Toolbox 0 Reference rou"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Un.Name = "rou";
                Assert.That(lib4.CachedValue, Is.EqualTo("rou Toolbox 0 Reference rou"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(1));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(1));

                _Un.Value = 2;
                Assert.That(lib4.CachedValue, Is.EqualTo("rou Toolbox 2 Reference rou"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(2));

                m2.Value = 2;
                Assert.That(lib4.CachedValue, Is.EqualTo("rou Toolbox 2 Reference rou"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(2));

                m2.Name = "a";
                Assert.That(lib4.CachedValue, Is.EqualTo("rou Toolbox 2 Reference a"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(1));
                Assert.That(lib4.EVC, Is.EqualTo(3));
            }
        
        }


          [Test]
        public void test4()
        {
            using (LambdaInspectorTestor<int> lib4 = new LambdaInspectorTestor<int>(o => o.Value * (1 + o.Value), _Un))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo(0));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Un.Value = 1;
                Assert.That(lib4.CachedValue, Is.EqualTo(2));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(1));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(1));

                _Un.Value = 2;
                Assert.That(lib4.CachedValue, Is.EqualTo(6));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(2));

                _Un.Value = 3;
                Assert.That(lib4.CachedValue, Is.EqualTo(12));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(3));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(3));

                _Un.Name = "c 2 1";
                Assert.That(lib4.CachedValue, Is.EqualTo(12));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(3));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(3));
            }
    
          }

          [Test]
          public void test21()
          {
              _Un.Friend = _Deux;
              _Deux.Friend = _Trois;
              _Trois.Friend = _Quatre;
              _Quatre.Friend = _Cinq;
              _Cinq.Friend = _Un;

              using (LambdaInspectorTestor<string> lib4 = new LambdaInspectorTestor<string>(o => o.Friend.Friend.Name, _Un))
              {
                  Assert.That(lib4.CachedValue, Is.EqualTo("c"));
                  Assert.That(lib4.Check, Is.True);
                  ////Assert.That(lib4.EVC, Is.EqualTo(0));
                  ////Assert.That(lib4.AVEC, Is.EqualTo(0));
                  Assert.That(lib4.EVC, Is.EqualTo(0));

                 _Deux.Friend=_Cinq;

                  Assert.That(lib4.CachedValue, Is.EqualTo("e"));
                  Assert.That(lib4.Check, Is.True);
                  //Assert.That(lib4.EVC, Is.EqualTo(1));
                  //Assert.That(lib4.AVEC, Is.EqualTo(0));
                  Assert.That(lib4.EVC, Is.EqualTo(1));

                  _Cinq.Name = "cab";

                  Assert.That(lib4.CachedValue, Is.EqualTo("cab"));
                  Assert.That(lib4.Check, Is.True);
                  //Assert.That(lib4.EVC, Is.EqualTo(2));
                  //Assert.That(lib4.AVEC, Is.EqualTo(0));
                  Assert.That(lib4.EVC, Is.EqualTo(2));

                  _Un.Friend = _Cinq;

                  Assert.That(lib4.CachedValue, Is.EqualTo("a"));
                  Assert.That(lib4.Check, Is.True);
                  //Assert.That(lib4.EVC, Is.EqualTo(3));
                  //Assert.That(lib4.AVEC, Is.EqualTo(0));
                  Assert.That(lib4.EVC, Is.EqualTo(3));

                  _Deux.Name = "azzzz";
                  Assert.That(lib4.CachedValue, Is.EqualTo("a"));
                  Assert.That(lib4.Check, Is.True);
                  //Assert.That(lib4.EVC, Is.EqualTo(3));
                  //Assert.That(lib4.AVEC, Is.EqualTo(0));
                  Assert.That(lib4.EVC, Is.EqualTo(3));

                  _Deux.Friend = _Trois;
                  Assert.That(lib4.CachedValue, Is.EqualTo("a"));
                  Assert.That(lib4.Check, Is.True);
                  //Assert.That(lib4.EVC, Is.EqualTo(3));
                  //Assert.That(lib4.AVEC, Is.EqualTo(0));
                  Assert.That(lib4.EVC, Is.EqualTo(3));

                  lib4.Disconnect();

              }

          }
                 

        [Test]
        public void test11()
        {
            using (LambdaInspectorTestor<string> lib4 = new LambdaInspectorTestor<string>(o => string.Format("{0} {1} {2}", o.Name, o.Value, o.Name.Length), _Trois))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo("c 2 1"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Trois.Friend = _Un;
                Assert.That(lib4.CachedValue, Is.EqualTo("c 2 1"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Trois.Value = 2;
                Assert.That(lib4.CachedValue, Is.EqualTo("c 2 1"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Trois.Value = 4;
                Assert.That(lib4.CachedValue, Is.EqualTo("c 4 1"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(1));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(1));

                _Trois.Value = 5;
                Assert.That(lib4.CachedValue, Is.EqualTo("c 5 1"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(2));

                _Trois.Name = "cce";
                Assert.That(lib4.CachedValue, Is.EqualTo("cce 5 3"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(3));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(3));

                _Trois.Name = "cce";
                Assert.That(lib4.CachedValue, Is.EqualTo("cce 5 3"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(3));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(3));


            }
            
        }
    

        [Test]
        public void test1()
        {
            using (LambdaInspectorTestor<string> lib4 = new LambdaInspectorTestor<string>(o => o.Name, _Trois))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo("c"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Un.Name = string.Empty;
                Assert.That(lib4.CachedValue, Is.EqualTo("c"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                 Assert.That(lib4.EVC, Is.EqualTo(0));

                _Trois.Name = "cc";
                Assert.That(lib4.CachedValue, Is.EqualTo("cc"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(1));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                 Assert.That(lib4.EVC, Is.EqualTo(1));

                _Trois.Name = "c";
                Assert.That(lib4.CachedValue, Is.EqualTo("c"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                 Assert.That(lib4.EVC, Is.EqualTo(2));

                _Trois.Name = "ceee";
                Assert.That(lib4.CachedValue, Is.EqualTo("ceee"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(3));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                 Assert.That(lib4.EVC, Is.EqualTo(3));
            }


        }

         //_LH1 = new LambdaInspector<MyObject, string>(o => o.Name);

          [Test]
        public void test9()
        {
            using (LambdaInspectorTestor<string> lib4 = new LambdaInspectorTestor<string>(o => _Cinq.Name, _Trois))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo("e"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Un.Name = "ee";
                Assert.That(lib4.CachedValue, Is.EqualTo("e"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Un.Value = 5;
                Assert.That(lib4.CachedValue, Is.EqualTo("e"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Cinq.Name = "eddde";
                Assert.That(lib4.CachedValue, Is.EqualTo("eddde"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(1));
                Assert.That(lib4.EVC, Is.EqualTo(1));
            }

       
        }

          [Test]
          public void discotest18()
          {

              using (LambdaInspectorTestor<bool> lib2 = new LambdaInspectorTestor<bool>(o => o.Friend == _Cinq.Friend, _Un))
              {
                  Assert.That(lib2.CachedValue, Is.True);
                  Assert.That(lib2.Check, Is.True);

                  _Cinq.Friend = null;
                  Assert.That(lib2.CachedValue, Is.True);
                  Assert.That(lib2.Check, Is.True);
                  Assert.That(lib2.EVC, Is.EqualTo(0));

                  _Cinq.Friend = _Cinq;
                  Assert.That(lib2.CachedValue, Is.False);
                  Assert.That(lib2.Check, Is.True);
                  //Assert.That(lib2.EVC, Is.EqualTo(0));
                  //Assert.That(lib2.AVEC, Is.EqualTo(1));
                  Assert.That(lib2.EVC, Is.EqualTo(1));

                  lib2.Disconnect();

                  Assert.That(lib2.CachedValue, Is.False);
                  //Assert.That(lib2.EVC, Is.EqualTo(0));
                  //Assert.That(lib2.AVEC, Is.EqualTo(1));
                  Assert.That(lib2.EVC, Is.EqualTo(1));

                  _Cinq.Friend = _Un.Friend;

                  Assert.That(lib2.CachedValue, Is.False);
                  Assert.That(lib2.RealValue, Is.True);
                  //Assert.That(lib2.AVEC, Is.EqualTo(2));
                  //Assert.That(lib2.EVC, Is.EqualTo(0));
                  Assert.That(lib2.EVC, Is.EqualTo(1));

                  _Un.Friend = _Cinq;

                  Assert.That(lib2.CachedValue, Is.False);
                  Assert.That(lib2.RealValue, Is.False);
                  //Assert.That(lib2.AVEC, Is.EqualTo(2));
                  //Assert.That(lib2.EVC, Is.EqualTo(0));
                  Assert.That(lib2.EVC, Is.EqualTo(1));

                  _Cinq.Friend = _Cinq;

                  Assert.That(lib2.CachedValue, Is.False);
                  Assert.That(lib2.RealValue, Is.True);
                  //Assert.That(lib2.AVEC, Is.EqualTo(3));
                  //Assert.That(lib2.EVC, Is.EqualTo(0));
                  Assert.That(lib2.EVC, Is.EqualTo(1));


              }
          }

        [Test]
        public void test18()
        {
            //Assert.That(_LH18.IsValid, Is.True);
            //DynamicFunction<MyObject, bool> BigRes = _LH18.ObjectAttributes;
            //List<ObjectAttributeWatched<MyObject, bool>> list = _LH18.ObjectAttributes.ObjectWatchers.ToList();
            //Assert.That(list.Count, Is.EqualTo(2));

            //var res = FromParameter(_LH18.ObjectAttributes.ObjectWatchers);
            //ObjectAttributeWatched<MyObject, bool> parametrised = res[0];
            //Assert.That(res.Count, Is.EqualTo(1));
            //List<AttributeListenerHelper<MyObject, bool>> attributes = res[0].Attributes.ToList();
            //Assert.That(attributes.Count, Is.EqualTo(1));
            //Assert.That(attributes[0].Attribute.Name, Is.EqualTo("Friend"));

            //AttributeListenerHelper<MyObject, bool> mytoto = res[0].AttributebyName("Friend");
            //Assert.That(mytoto, Is.Not.Null);
            //Assert.That(mytoto, Is.EqualTo(attributes[0]));
            ////Assert.That(mytoto.ApplyOnAttribute(_Un, null), Is.False);
            ////Assert.That(mytoto.ApplyOnAttribute(_Deux, null), Is.False);
            ////Assert.That(mytoto.ApplyOnAttribute(_Deux, _Deux), Is.False);
            ////Assert.That(mytoto.ApplyOnAttribute(_Deux, _Un), Is.False);
            ////Assert.That(mytoto.ApplyOnAttribute(null, null), Is.True);
            ////Assert.That(mytoto.ApplyOnAttribute(null, _Deux), Is.True);
            ////Assert.That(mytoto.ApplyOnAttribute(null, _Un), Is.True);
            //Assert.That(parametrised.IsParameterDependant, Is.True);
            //Assert.That(parametrised is ParameterAttributeWatch<MyObject, bool>, Is.True);

            //List<ConstantAttributeWatch<MyObject, bool>> ca1 = BigRes.ConstantObjectWatchers.ToList();
            //List<ParameterAttributeWatch<MyObject, bool>> ca2 = BigRes.ParameterObjectWatchers.ToList();

            //Assert.That(ca1.Count, Is.EqualTo(1));
            //Assert.That(ca2.Count, Is.EqualTo(1));

            //ConstantAttributeWatch<MyObject, bool> ca = ca1[0];
            //attributes = ca.Attributes.ToList();
            //Assert.That(attributes.Count, Is.EqualTo(1));
            //AttributeListenerHelper<MyObject, bool> myonly = attributes[0];
            //Assert.That(myonly, Is.Not.Null);
            //Assert.That(mytoto.Attribute.Name, Is.EqualTo("Friend"));
            //Assert.That(ca.AttributebyName("Friend"), Is.EqualTo(myonly));
            //Assert.That(ca.AttributebyName("Frddiend"), Is.Null);


            //bool ress = _LH18.ObjectAttributes.Evaluate(_Un);
            //Assert.That(ress, Is.True);
            _Un.Value = 21112;
            _Un.Friend = _Cinq;

            //ress = _LH18.ObjectAttributes.Evaluate(_Un);
            //Assert.That(ress, Is.False);

            using (LambdaInspectorTestor<bool> lib = new LambdaInspectorTestor<bool>(o => o.Friend == _Cinq.Friend, _Un))
            {

                Assert.That(lib.CachedValue, Is.False);
                Assert.That(lib.Check, Is.True);

                _Un.Friend = _Deux;

                Assert.That(lib.CachedValue, Is.False);
                Assert.That(lib.Check, Is.True);
                Assert.That(lib.EVC, Is.EqualTo(0));

                _Un.Friend = _Trois;

                Assert.That(lib.CachedValue, Is.False);
                Assert.That(lib.Check, Is.True);
                Assert.That(lib.EVC, Is.EqualTo(0));

                _Cinq.Friend = _Un;

                Assert.That(lib.CachedValue, Is.False);
                Assert.That(lib.Check, Is.True);
                //Assert.That(lib.EVC, Is.EqualTo(0));
                //Assert.That(lib.AVEC, Is.EqualTo(1));
                Assert.That(lib.EVC, Is.EqualTo(0));


                _Un.Friend = _Un;

                Assert.That(lib.CachedValue, Is.True);
                Assert.That(lib.Check, Is.True);
                //Assert.That(lib.EVC, Is.EqualTo(1));
                //Assert.That(lib.AVEC, Is.EqualTo(1));
                Assert.That(lib.EVC, Is.EqualTo(1));

                _Un.Value = 333;

                Assert.That(lib.CachedValue, Is.True);
                Assert.That(lib.Check, Is.True);
                //Assert.That(lib.EVC, Is.EqualTo(1));
                //Assert.That(lib.AVEC, Is.EqualTo(1));
                Assert.That(lib.EVC, Is.EqualTo(1));

                _Cinq.Value = 333333;

                Assert.That(lib.CachedValue, Is.True);
                Assert.That(lib.Check, Is.True);
                //Assert.That(lib.EVC, Is.EqualTo(1));
                //Assert.That(lib.AVEC, Is.EqualTo(1));
                Assert.That(lib.EVC, Is.EqualTo(1));
            }


            using (LambdaInspectorTestor<bool> lib2 = new LambdaInspectorTestor<bool>(o => o.Friend == _Cinq.Friend, _Cinq))
            {
                Assert.That(lib2.CachedValue, Is.True);
                Assert.That(lib2.Check, Is.True);

                _Cinq.Friend = null;
                Assert.That(lib2.CachedValue, Is.True);
                Assert.That(lib2.Check, Is.True);
                Assert.That(lib2.EVC, Is.EqualTo(0));

                _Cinq.Friend = _Cinq;
                Assert.That(lib2.CachedValue, Is.True);
                Assert.That(lib2.Check, Is.True);
                Assert.That(lib2.EVC, Is.EqualTo(0));
            }

          
        }

        [Test]
        public void test20()
        {
            using (LambdaInspectorTestor<string> lib4 = new LambdaInspectorTestor<string>(o => (o.Friend == null) ? string.Empty : o.Friend.Name, _Trois))
            {
                Assert.That(lib4.CachedValue, Is.EqualTo(string.Empty));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Quatre = new MyObject("d", 3);
                _Trois.Name = "cddd";
                Assert.That(lib4.CachedValue, Is.EqualTo(string.Empty));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(0));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(0));

                _Trois.Friend = _Quatre;
                Assert.That(lib4.CachedValue, Is.EqualTo("d"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(1));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(1));

                _Quatre.Name = "foooo";
                Assert.That(lib4.CachedValue, Is.EqualTo("foooo"));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(2));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(2));

                _Trois.Friend = _Un;
                Assert.That(lib4.CachedValue, Is.EqualTo(_Un.Name));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(3));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(3));

                _Trois.Friend = _Cinq;
                Assert.That(lib4.CachedValue, Is.EqualTo(_Cinq.Name));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(4));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(4));

                _Un.Name = "tatatattattatattata";
                Assert.That(lib4.CachedValue, Is.EqualTo(_Cinq.Name));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(4));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(4));

                _Trois.Friend = null;
                Assert.That(lib4.CachedValue, Is.EqualTo(string.Empty));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(5));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(5));

                _Cinq.Name = "tatatattattaddtattata";
                Assert.That(lib4.CachedValue, Is.EqualTo(string.Empty));
                Assert.That(lib4.Check, Is.True);
                //Assert.That(lib4.EVC, Is.EqualTo(5));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(5));

                lib4.Disconnect();


                _Trois.Friend = _Cinq;
                Assert.That(lib4.CachedValue, Is.EqualTo(string.Empty));
                Assert.That(lib4.RealValue, Is.EqualTo("tatatattattaddtattata"));
                //Assert.That(lib4.EVC, Is.EqualTo(5));
                //Assert.That(lib4.AVEC, Is.EqualTo(0));
                Assert.That(lib4.EVC, Is.EqualTo(5));


            }
        }

        [Test]
        public void test15()
        {
            int yu = 3;
            _Un.Name = string.Empty;
            _Un.Value = 0;

            using (LambdaInspectorTestor<int> lib3 = new LambdaInspectorTestor<int>(o => o.Name.Length * yu + o.Value * 5 + o.Value * o.Value, _Un))
            {
                Assert.That(lib3.CachedValue, Is.EqualTo(0));
                Assert.That(lib3.Check, Is.True);

                _Un.Friend = _Deux;
                Assert.That(lib3.CachedValue, Is.EqualTo(0));
                Assert.That(lib3.Check, Is.True);
                //Assert.That(lib3.EVC, Is.EqualTo(0));
                //Assert.That(lib3.AVEC, Is.EqualTo(0));
                Assert.That(lib3.EVC, Is.EqualTo(0));

                _Un.Name = "a";
                Assert.That(lib3.CachedValue, Is.EqualTo(3));
                Assert.That(lib3.Check, Is.True);
                //Assert.That(lib3.EVC, Is.EqualTo(1));
                //Assert.That(lib3.AVEC, Is.EqualTo(0));
                Assert.That(lib3.EVC, Is.EqualTo(1));

                _Un.Name = "b";
                Assert.That(lib3.CachedValue, Is.EqualTo(3));
                Assert.That(lib3.Check, Is.True);
                //Assert.That(lib3.EVC, Is.EqualTo(1));
                //Assert.That(lib3.AVEC, Is.EqualTo(0));
                Assert.That(lib3.EVC, Is.EqualTo(1));

                _Un.Value = 1;
                Assert.That(lib3.CachedValue, Is.EqualTo(9));
                Assert.That(lib3.Check, Is.True);
                //Assert.That(lib3.EVC, Is.EqualTo(2));
                //Assert.That(lib3.AVEC, Is.EqualTo(0));
                Assert.That(lib3.EVC, Is.EqualTo(2));

                _Un.Name = "";
                Assert.That(lib3.CachedValue, Is.EqualTo(6));
                Assert.That(lib3.Check, Is.True);
                //Assert.That(lib3.EVC, Is.EqualTo(3));
                //Assert.That(lib3.AVEC, Is.EqualTo(0));
                Assert.That(lib3.EVC, Is.EqualTo(3));

                _Un.Value = 0;
                Assert.That(lib3.CachedValue, Is.EqualTo(0));
                Assert.That(lib3.Check, Is.True);
                //Assert.That(lib3.EVC, Is.EqualTo(4));
                //Assert.That(lib3.AVEC, Is.EqualTo(0));
                Assert.That(lib3.EVC, Is.EqualTo(4));

                _Trois.Name = "c";
                _Trois.Value = 2;
                _Trois.Friend = null;
            }
          
  
        }

        [Test]
        public void basictest()
        {
            object toto = 99;

           // List<toto.GetType()> totot = new List<toto.GetType()>();

            //Type d1 = typeof(List<>);
            //Type[] typeArgs = { typeof(string) };
            //Type makeme = d1.MakeGenericType(typeArgs);
            //object o = Activator.CreateInstance(makeme);
            //List<string> itsMe = o as List<string>;
            //Console.WriteLine((itsMe == null) ? "Failed" : "Succeeded");

            //Console.WriteLine(_LH1);
            //Console.WriteLine(_LH2);
            //Console.WriteLine(_LH22);
            //Console.WriteLine(_LH3);
            //Console.WriteLine(_LH4);
            //Console.WriteLine(_LH5);
            //Console.WriteLine(_LH6);
            //Console.WriteLine(_LH7);
            //Console.WriteLine(_LH8);
            //Console.WriteLine(_LH9);
            //Console.WriteLine(_LH10);
            //Console.WriteLine(_LH11);
            //Console.WriteLine(_LH12);
            //Console.WriteLine(_LH13);
            //Console.WriteLine(_LH14);
            //Console.WriteLine(_LH15);
            //Console.WriteLine(_LH16);
            //Console.WriteLine(_LH17);
            //Console.WriteLine(_LH18);

           //_LH17 = new Inspector<MyObject, int>(o => o.Name.Length * (yu + o.Name.Length));


            //_LH1 = new Inspector<MyObject, string>(o => o.Name);
            //_LH2 = new Inspector<MyObject, string>(o => "Toolbox");
            //_LH22 = new Inspector<MyObject, string>(o => string.Format("Toolbox{0}", o));
            //_LH3 = new Inspector<MyObject, int>(o => o.Value);
            //_LH4 = new Inspector<MyObject, int>(o => 33 * o.Value + 15);
            //_LH5 = new Inspector<MyObject, string>(o => string.Format("Toolbox{0}", o.Name));

            //_LH6 = new Inspector<MyObject, string>(o => Toto());


            //_LH7 = new Inspector<MyObject, int>(o => yu * o.Value);
            //_LH8 = new Inspector<MyObject, string>(o => Name);
            //_LH9 = new Inspector<MyObject, string>(o => _Cinq.Name);
            //_LH10 = new Inspector<MyObject, string>(o => o.ToString());
            //_LH11 = new Inspector<MyObject, string>(o => string.Format("{1}Toolbox{0}", o.Name, o.Value));
            //_LH12 = new Inspector<MyObject, string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, _Cinq.Name));

            //MyObject m2 = new MyObject("Toolbox", 33);
            //_LH13 = new Inspector<MyObject, string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, m2.Name));

            //_LH14 = new Inspector<MyObject, string>(o => string.Format("{0} Toolbox {1} Reference {2}", o.Name, o.Value, Name));



            //_LH16 = new Inspector<MyObject, bool>(o => (o.Value == (o.Name.Length + Diff)));



        }
    }
}
