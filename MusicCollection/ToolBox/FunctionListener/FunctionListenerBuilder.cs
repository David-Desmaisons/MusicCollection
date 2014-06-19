//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;

//using MusicCollection.ToolBox.Collection;
//using MusicCollection.Infra;

//namespace MusicCollection.ToolBox.FunctionListener
//{
//    internal class FunctionListenerBuilder
//    {
//        private Type _ConsideredType;
//        private List<IRawResultListenerFactory> _Facs = new List<IRawResultListenerFactory>();
 
//        internal Type Type
//        {
//            get { return _ConsideredType; }
//        }

//        internal IEnumerable<IRawResultListenerFactory> Factories
//        {
//            get { return _Facs; }
//        }

//        internal FunctionListenerBuilder(Type type)
//        {
//            _ConsideredType = type;
//        }

//        internal void AddFactory(IRawResultListenerFactory fac)
//        {
//            _Facs.Add(fac);
//        }
//    }
//}
