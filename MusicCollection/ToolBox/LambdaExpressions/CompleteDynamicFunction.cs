using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics;

using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;


namespace MusicCollection.ToolBox.LambdaExpressions
{
    class CompleteDynamicFunction<Tor, TDes> : ICompleteFunction<Tor, TDes> where Tor : class
    {
        static WeakDictionary<Expression<Func<Tor, TDes>>, ICompleteFunction<Tor, TDes>> _Cache
            = new WeakDictionary<Expression<Func<Tor, TDes>>, ICompleteFunction<Tor, TDes>>(new Dictionary<Expression<Func<Tor, TDes>>, WeakReference<ICompleteFunction<Tor, TDes>>>
                (ExpressionComparer.Comparer));

        private Expression<Func<Tor, TDes>> _Expression;
        private Func<Tor, IVisitIObjectAttribute, TDes> _Computor;
        private Action<IVisitIObjectAttribute> _ConstantVisitor;
        private bool _IsParameterDynamic = false;
        private bool _IsConstantDynamic = false;


        private Func<Tor, TDes> _RawComputor;

        private IDictionary<Tor, ListenedRoot<Tor, TDes>> _ListenedRoots = PolyMorphDictionaryGeneric<Tor, ListenedRoot<Tor, TDes>>.ScalableDictionary();
        private ListenedConstant<Tor, TDes> _Constants;

        internal Expression Expression
        {
            get { return _Expression; }
        }

        internal Func<Tor, IVisitIObjectAttribute, TDes> Computor
        {
            get { return _Computor; }
        }

        internal Action<IVisitIObjectAttribute> ConstantVisitor
        {
            get { return _ConstantVisitor; }
        }

        private bool _FE;
        public bool FactorizeEvent
        {
            get { return _FE; }
            set { _FE = value; }
        }

        internal static ICompleteFunction<Tor, TDes> GetCompleteDynamicFunction(Expression<Func<Tor, TDes>> iExpression)
        {
            var res = _Cache.FindOrCreate_ThreadSafe(iExpression, e => CompleteDynamicFunction<Tor, TDes>.GetFromExpression(e));
            if (res.CollectionStatus == CollectionStatus.Find)
            {
                //find
                return res.Item.Clone();
            }
            else
            {
                //create
                return res.Item;
            }
        }

        static private ICompleteFunction<Tor, TDes> GetFromExpression(Expression<Func<Tor, TDes>> iExpression)
        {
            ExpressionWatcherInterceptorBuilderParameterOnly<Tor, TDes> builder = new ExpressionWatcherInterceptorBuilderParameterOnly<Tor, TDes>(iExpression);
            Expression<Action<IVisitIObjectAttribute>> ConstantRegsitor = builder.BuildFromConstant();

            if ((builder.Changed == false) && (ConstantRegsitor == null))
            {
                return new ConstantFunctionAdaptor<Tor, TDes>(iExpression.Compile());
            }

            return new CompleteDynamicFunction<Tor, TDes>(iExpression, builder, ConstantRegsitor);
        }

        private CompleteDynamicFunction(Expression<Func<Tor, TDes>> iExpression, ExpressionWatcherInterceptorBuilderParameterOnly<Tor, TDes> builder, Expression<Action<IVisitIObjectAttribute>> ConstantRegistor)
        {
            _Expression = iExpression;
            _Computor = builder.Transformed.Compile();
            _IsParameterDynamic = builder.Changed;
            _RawComputor = iExpression.Compile();
            if (ConstantRegistor != null)
            {
                _ConstantVisitor = ConstantRegistor.Compile();
                _IsConstantDynamic = true;
                _Constants = new ListenedConstant<Tor, TDes>(this);
            }
            FactorizeEvent = false;
        }

        private CompleteDynamicFunction(CompleteDynamicFunction<Tor, TDes> iClonable)
        {
            _Expression = iClonable._Expression;
            _Computor = iClonable._Computor;
            _IsParameterDynamic = iClonable._IsParameterDynamic;
            _ConstantVisitor = iClonable._ConstantVisitor;
            _IsConstantDynamic = iClonable._IsConstantDynamic;
            _RawComputor = iClonable._RawComputor;
            if (_IsConstantDynamic)
            {
                _Constants = new ListenedConstant<Tor, TDes>(this);
            }
            FactorizeEvent = false;
        }



        public ICompleteFunction<Tor, TDes> Clone()
        {
            return new CompleteDynamicFunction<Tor, TDes>(this);
        }


        #region Display

        public override string ToString()
        {
            return string.Format("Expression: <{0}>", _Expression);
        }

        #endregion

        #region Dynamic Information

        public bool IsDynamic
        {
            get { return (_IsConstantDynamic || _IsParameterDynamic); }
        }

        public bool IsParameterDynamic
        {
            get { return _IsParameterDynamic; }
        }

        public bool IsConstantDynamic
        {
            get { return _IsConstantDynamic; }
        }

        #endregion

        public EventHandler<ObjectModifiedArgs> ListenedObjects(Tor to)
        {
            ListenedRoot<Tor, TDes> root = _ListenedRoots[to];

            if (!root.Islistening(to))
                return null;

            return root.ObjectListener;
        }

        public IEnumerable<Tor> Listened
        {
            get { return _ListenedRoots.Keys; }
        }

        public IEnumerable<Tuple<Tor, TDes>> ListenedandCachedValue
        {
            get { return _ListenedRoots.Select(lr => new Tuple<Tor, TDes>(lr.Key, lr.Value.CurrentValue)); }
        }

        public int ListenedCount
        {
            get { return _ListenedRoots.Count; }
        }

        public Func<Tor, TDes> EvaluateAndRegister
        {
            get { return (o) => { Register(o); return Evaluate(o); }; }
        }

        public Func<Tor, TDes> Evaluate
        {
            get { return _RawComputor; }
        }

        public bool IsSingleRegistered(Tor or)
        {
            ListenedRoot<Tor, TDes> res = null;
            if (_ListenedRoots.TryGetValue(or, out res))
                return (res.Count==1);

            return false;

        }


        private TDes GetCurrentOrOldValueComputer(Tor or)
        {
            ListenedRoot<Tor, TDes> res = null;
            if (_ListenedRoots.TryGetValue(or, out res))
                return res.CurrentValue;

            return _RawComputor(or);
        }

        public Func<Tor, TDes> CurrentOrOldValueComputer
        {
            get { return GetCurrentOrOldValueComputer; }
        }

        public TDes GetCached(Tor t)
        {
              return _ListenedRoots[t].CurrentValue;
        }

        public bool Register(Tor to)
        {
            ListenedRoot<Tor,TDes> li = null;
            if (_ListenedRoots.TryGetValue(to, out li))
            {
                li.Count++;
                return false;
            }

            var res = new ListenedRoot<Tor, TDes>(this, to);
            _ListenedRoots.Add(to, res);
            return true;
        }

        public TDes RegisterAndGetValue(Tor to)
        {
            var res = new ListenedRoot<Tor, TDes>(this, to);
            _ListenedRoots.Add(to, res);
            return res.CurrentValue;
        }

        public bool UnRegister(Tor to)
        {
            ListenedRoot<Tor, TDes> res = _ListenedRoots[to];

            if (res.Count == 1)
            {
                res.Unregister();
                _ListenedRoots.Remove(to);
                return true;
            }

            res.Count--;
            return false;

        //    res.Unregister();
        //    _ListenedRoots.Remove(to);
        }

        public event EventHandler<ObjectAttributeChangedArgs<TDes>> ElementChanged;

        public event EventHandler<ObjectAttributesChangedArgs<Tor, TDes>> ElementsChanged;


        internal void OnChanges(Tor or, TDes old, TDes nvalue)
        {
            EventHandler<ObjectAttributeChangedArgs<TDes>> ec = ElementChanged;
            if (ec == null)
                return;

            ec(this, new ObjectAttributeChangedArgs<TDes>(or, null, old, nvalue));
        }

        internal void ConstantChanges()
        {
            if (this.FactorizeEvent == false)
            {
                EventHandler<ObjectAttributeChangedArgs<TDes>> ec = ElementChanged;
                if (ec == null)
                    return;

                _ListenedRoots.Values.Apply(lr => lr.UpdateOnConstantChanged());
            }
            else
            {
                EventHandler<ObjectAttributesChangedArgs<Tor, TDes>> esc = ElementsChanged;
                if (esc == null)
                    return;

                ObjectAttributesChangedArgs<Tor, TDes> send = new ObjectAttributesChangedArgs<Tor, TDes>(_ListenedRoots.Count);

                _ListenedRoots.Values.Apply(lr => lr.UpdateOnConstantChanged(send, this._RawComputor));

                if (send.HasChanged)
                    esc(this, send);


            }
        }

        public void Dispose()
        {
            UnListenAll();

            if (_Constants != null)
            {
                _Constants.Unregister();
                _Constants = null;
            }
           
        }

        public void UnListenAll()
        {
            _ListenedRoots.Values.Apply(lr => lr.Unregister());
            _ListenedRoots.Clear();
        }
    }
}
