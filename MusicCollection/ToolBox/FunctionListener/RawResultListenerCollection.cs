//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;

//using MusicCollection.Infra;

//using MusicCollection.ToolBox.Collection;
//using MusicCollection.ToolBox.LambdaExpressions;

//namespace MusicCollection.ToolBox.FunctionListener
//{

//    internal class RawResultListenerCollection: IDisposable
//    {
//        private object _My;
//        private bool _Registering = false;
//        private IDictionary<IRawResultListenerFactory, IRawResultListener> _Dic = new PolyMorphSimpleDictionary<IRawResultListenerFactory, IRawResultListener>();
  
//        internal RawResultListenerCollection(object Father, IEnumerable<FunctionListenerBuilder> facs):this(Father,facs.SelectMany((fac)=>fac.Factories))
//        {
//        }

//        private RawResultListenerCollection(object Father, IEnumerable<IRawResultListenerFactory> facs)
//        {
//            _My = Father;
//            Agregate(facs);
//        }

//        internal void Agregate(IEnumerable<IRawResultListenerFactory> facs)
//        {
//            facs.Apply(f => _Dic.Add(f, f.CreateRawListener(_My)));
//        }

//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        internal IEnumerable<EventHandler<ObjectModifiedArgs>> ObservedAttribute
//        {
//            get
//            {
//                return _Dic.Values.Select(o => o.ListenedObject).Where(obj=>obj!=null);
//            }
//        }

//        internal IResultListener<TDes> Find<TDes>(IRawResultListenerFactory Factory)
//        {
//            var foc = _Dic.FindOrCreate(Factory, (f) => f.CreateRawListener(_My));
//            if ((foc.CollectionStatus == CollectionStatus.Create) && (_Registering))
//            {
//                foc.Item.Register();
//            }
//            return  foc.Item as IResultListener<TDes>;
//        }


//        internal IResultListener<TDes> Get<TDes>(IRawResultListenerFactory Factory)
//        {
//            return _Dic[Factory] as IResultListener<TDes>;
//        }

//        public void Register()
//        {
//            _Registering = true;
//            _Dic.Values.Apply(f => f.Register());
//        }

//        public void UnRegister()
//        {
//            _Registering = true;
//            _Dic.Values.Apply(f => f.UnRegister());
//        }

//        public void Dispose()
//        {
//            _Dic.Values.Apply(f => f.Dispose());
//        }
//    }
//}
