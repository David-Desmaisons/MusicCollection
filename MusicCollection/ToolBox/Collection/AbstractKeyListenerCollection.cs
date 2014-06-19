using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading;

using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection
{
    internal abstract class AbstractKeyListenerCollection<TKey, TElement> : IDisposable where TElement : class
    {
        private readonly bool _IsUnsafeOnRename;
        private IFunction<TElement, TKey> _Function;
        private Func<TKey, TKey> _Prepro;

        protected readonly object _Locker = new object();

        protected AbstractKeyListenerCollection(Expression<Func<TElement, TKey>> iAcessor, Func<TKey, TKey> Preprocessing = null)
        {
            if (iAcessor == null)
                throw new ArgumentNullException();

            lock (_Locker)
            {
                _Prepro = Preprocessing;
                _Function = iAcessor.CompileToObservable();
                _IsUnsafeOnRename = !_Function.IsParameterDynamic;
                _Function.ElementChanged += ElementChanged;
            }
        }

        #region contract_with_Derived

        protected abstract IEnumerable<TElement> Elements { get; }

        protected abstract bool RawRegister(TElement iEl);

        protected abstract bool RawRemove(TElement iEl, TKey key);

        protected virtual void OnAdd(TElement iEL)
        {
            if (!IsUnsafeOnRename)
            {
                _Function.Register(iEL);
            }
        }

        protected virtual void OnRemove(TElement iEL)
        {
            if (!IsUnsafeOnRename)
            {
                _Function.UnRegister(iEL);
            }
        }


        #endregion

        public void Dispose()
        {
            lock (_Locker)
            {
                _Function.Dispose();
            }
        }


        protected Func<TElement, TKey> _Acessor
        {
            get { return _Function.Evaluate; }
        }

        public bool IsUnsafeOnRename
        {
            get { return _IsUnsafeOnRename; }
        }

        protected TKey ProcessKey(TKey key)
        {
            if (_Prepro == null)
                return key;

            return _Prepro.Invoke(key);
        }

        protected TKey GetKey(TElement value)
        {
            return ProcessKey(_Acessor(value));
        }

        private void UpdateElement(ObjectAttributeChangedArgs<TKey> oac)
        {
            if (oac == null)
                return;

            TElement or = oac.ModifiedObject as TElement;
            RawRegister(or);

            RawRemove(or, ProcessKey(oac.Old));
        }

        private void ElementChanged(object sender, ObjectAttributeChangedArgs<TKey> oac)
        {
            lock (_Locker)
            {
                UpdateElement(oac);
            }
        }

        //internal IDisposable GetThreadSafer()
        //{
        //    return new ThreadSafer(_Locker);
        //}

        //private class ThreadSafer : IDisposable
        //{
        //    private readonly object _Lock;
        //    private bool _LT = false;

        //    internal ThreadSafer(object ilock)
        //    {
        //        _Lock = ilock;
        //        Monitor.Enter(_Lock, ref _LT);
        //    }

        //    public void Dispose()
        //    {
        //        if (_LT)
        //        {
        //            _LT = false;
        //            Monitor.Exit(_LT);
        //        }
        //    }
        //}



    }
}
