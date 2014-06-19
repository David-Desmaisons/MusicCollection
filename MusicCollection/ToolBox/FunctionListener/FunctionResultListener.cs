using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

using MusicCollection.Infra;
using MusicCollection.ToolBox.LambdaExpressions;

namespace MusicCollection.ToolBox.FunctionListener
{
    internal class FactoryListenerBuilder<Tor, TDes>  where Tor : class,IObjectBuildAttributeListener
    {
        static internal IResultListenerFactory<Tor, TDes> FunctionResultListenerFactory(Expression<Func<Tor, TDes>> expression, string iAttributename)
        {

            if (expression == null)
                throw new ArgumentNullException();

            ICompleteFunction<Tor, TDes> func = CompleteDynamicFunction<Tor, TDes>.GetCompleteDynamicFunction(expression);

            if (func.IsConstantDynamic)
                return new FunctionResultListenerCompleteFactory<Tor, TDes>(func, iAttributename);

            return new FunctionResultListenerNoneConstantFactory<Tor, TDes>(func, iAttributename);
        }
    }

    internal class FunctionResultListenerNoneConstantFactory<Tor, TDes> : IResultListenerFactory<Tor, TDes> where Tor : class,IObjectBuildAttributeListener
    {

        protected ICompleteFunction<Tor, TDes> _Function;
        //protected Action<Tor, ObjectAttributeChangedArgs<TDes>> _Listener;
        private string _PropertyName;

        internal FunctionResultListenerNoneConstantFactory(ICompleteFunction<Tor, TDes> iFunction, string PN)
            //Action<Tor, ObjectAttributeChangedArgs<TDes>> Listener)
        {
            //_Listener = Listener;
            _PropertyName = PN;
            _Function = iFunction;
            _Function.ElementChanged += OnChanges;
            //RegisterOnValue = false;
        }


        public ICompleteFunction<Tor, TDes> Function
        {
            get { return _Function; }
        }

        public override string ToString()
        {
            return string.Format("PropertyName:<{0}> IFunction:<{1}>", _PropertyName, _Function);
        }

        protected void OnChanges(object sender, ObjectAttributeChangedArgs<TDes> oaca)
        {
            IObjectBuildAttributeListener listener = oaca.ModifiedObject as IObjectBuildAttributeListener;
            if (listener == null)
                return;

            listener.AttributeChanged(this._PropertyName, oaca.Old, oaca.New);
        }

        public IResultListener<TDes> CreateListener(Tor origine)
        {
            return new FunctionResultListener<Tor, TDes>(origine, this);
        }

        public IRawResultListener CreateRawListener(object origine)
        {
            return CreateListener((Tor)origine);
            //return new FunctionResultListener<Tor, TDes>((Tor)origine, this);
        }

        public virtual void Register(FunctionResultListener<Tor, TDes> origine)
        {
            _Function.Register(origine.Origine);
        }

        public virtual void UnRegister(FunctionResultListener<Tor, TDes> Origine)
        {
            _Function.UnRegister(Origine.Origine);
        }

        public TDes Evaluate(Tor Origine)
        {
            return _Function.Evaluate(Origine);
        }

        public TDes GetCached(Tor Origine)
        {
            return _Function.GetCached(Origine);
        }

        public virtual void Dispose()
        {
            _Function.ElementChanged -= OnChanges;
            _Function.Dispose();
        }

        //public bool RegisterOnValue
        //{
        //    get;
        //    set;
        //}
    }

    //, IResultListenerCompleteFactory<Tor, TDes> 
    internal class FunctionResultListenerCompleteFactory<Tor, TDes> : FunctionResultListenerNoneConstantFactory<Tor, TDes> where Tor : class,IObjectBuildAttributeListener
    {

        private HashSet<Tor> _List = new HashSet<Tor>();

        internal FunctionResultListenerCompleteFactory(ICompleteFunction<Tor, TDes> iFunction, string PN)
            : base(iFunction, PN)
        {
            //RegisterOnValue = false;
        }

        public override void Register(FunctionResultListener<Tor, TDes> origine)
        {
            _List.Add(origine.Origine);
            base.Register(origine);
        }

        public override void UnRegister(FunctionResultListener<Tor, TDes> Origine)
        {
            _List.Remove(Origine.Origine);
            base.UnRegister(Origine);
        }

        public override void Dispose()
        {
            _List.Apply(o => _Function.UnRegister(o));
            _List = null;
            base.Dispose();
        }
    }

    

    internal class FunctionResultListener<Tor, TDes> : IResultListener<TDes> where Tor : class,IObjectBuildAttributeListener
    {
        private FunctionResultListenerNoneConstantFactory<Tor, TDes> _Father;
        private Tor _Origine;
        private bool _Init = false;

        internal FunctionResultListener(Tor or, FunctionResultListenerNoneConstantFactory<Tor, TDes> Father)
        {
            _Father = Father;
            _Origine = or;
        }

        public override string ToString()
        {
            return string.Format("Function:<{0}> Object:<{1}> Init:<{2}>", _Father, _Origine, _Init);
        }

        public Tor Origine
        {
            get { return _Origine; }
        }

        public TDes Value
        {
            get
            {
                //if (_Father.RegisterOnValue)
                //    Register();

                if (_Init)
                    return _Father.GetCached(_Origine);
                
                return _Father.Evaluate(_Origine);
            }
        }

        public void Register()
        {
            if (_Init == false)
            {
                _Init = true;
                _Father.Register(this);
            }
        }

        public void UnRegister()
        {
            if (_Init == true)
            {
                _Init = false;
                _Father.UnRegister(this);
            }
        }

        public void Dispose()
        {
            if (_Init == true)
            {
                _Father.UnRegister(this);
                _Init = false;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public EventHandler<ObjectModifiedArgs> ListenedObject
        {
            get { return (_Init==false) ? null : _Father.Function.ListenedObjects(_Origine); }
        }
    }

}
