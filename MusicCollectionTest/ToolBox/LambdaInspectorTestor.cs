using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;
using NUnit.Framework;
using System.Linq.Expressions;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.ToolBox
{
    internal class LambdaInspectorTestor<T> : LambdaInspectorGenericTestor<MyObject, T>
    {
        internal LambdaInspectorTestor(Expression<Func<MyObject, T>> Trans, MyObject Tested)
            : base(Trans,Tested)
        {
        }
    }


    internal class LambdaInspectorGenericTestor<Tin,Tout> : IDisposable where Tin:class
    {
        private Func<Tin, Tout> _Trans;
        private IFunction<Tin, Tout> _MyFunction;
        private Tin _Tested;
        private Tout _CachedValue;
        private int _evc = 0;

        internal int EVC
        {
            get { return _evc; }
        }

        internal Tout CachedValue
        {
            get { return _CachedValue; }
        }

        internal LambdaInspectorGenericTestor(Expression<Func<Tin, Tout>> Trans, Tin Tested)
        {
            _Trans = Trans.Compile();
            _MyFunction = Trans.CompileToObservable();
            _Tested = Tested;
            _CachedValue = _MyFunction.EvaluateAndRegister(_Tested);
            _MyFunction.ElementChanged += ElementChanged;
        }

        internal void Disconnect()
        {
            _MyFunction.UnRegister(_Tested);
        }

        private void ElementChanged(object sender, ObjectAttributeChangedArgs<Tout> argq)
        {
            _evc++;
            Assert.That(_CachedValue, Is.EqualTo(argq.OldAttributeValue));
            _CachedValue = argq.New;
            Assert.That(Check, Is.True);
            Assert.That(object.ReferenceEquals(argq.ModifiedObject, _Tested), Is.True);
        }

        internal Tout RealValue
        {
            get
            {
                return _Trans(_Tested);
            }
        }

        internal bool Check
        {
            get { return object.Equals(_CachedValue, _Trans(_Tested)) && object.Equals(_CachedValue, _MyFunction.Evaluate(_Tested)); }
        }

        public void Dispose()
        {
            _MyFunction.ElementChanged -= ElementChanged;
            _MyFunction.Dispose();
        }
    }
}
