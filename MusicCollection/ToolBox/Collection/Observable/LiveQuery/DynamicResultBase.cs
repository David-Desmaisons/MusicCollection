using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal abstract class DynamicResultBase<TSource,TDest> : NotifyCompleteAdapterNoCache, ILiveResult<TDest> 
    {
        protected IList<TSource> _Source;
        protected DynamicResultBase(IList<TSource> iSource)
        {
            _Source = iSource;
        }

        private TDest _Value;
        public TDest Value
        {
            //protected set { if (object.Equals(_Value, value)) return; var old = _Value; _Value = value; PropertyHasChanged("Value", old, _Value); }
            protected set { Set(ref _Value, value); }
            get { return _Value; }
        }

        public abstract void Dispose();

    }
}
