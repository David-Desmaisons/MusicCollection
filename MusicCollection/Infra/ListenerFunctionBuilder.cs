//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;

//using MusicCollection.ToolBox.FunctionListener;

//namespace MusicCollection.Infra
//{
//    public class ListenerFunctionBuilder
//    {
//        public static IResultListenerFactory<Tor, TDes> BuildFunctionListenerFactory<Tor, TDes>(Expression<Func<Tor, TDes>> expression, string iAttributename) where Tor : class, IObjectBuildAttributeListener
//        {
//            IResultListenerCompleteFactory<Tor, TDes> res = FactoryListenerBuilder<Tor, TDes>.FunctionResultListenerFactory(expression, iAttributename);
//            res.RegisterOnValue = true;
//            return res;
//        }

//        //public static IResultListenerFactory<Tor, TDes> BuildFunctionListenerFactory<Tor, TDes>(Expression<Func<Tor, TDes>> expression, Action<Tor, ObjectAttributeChangedArgs<TDes>> Listener) where Tor : class
//        //{
//        //    IResultListenerCompleteFactory<Tor, TDes> res = FactoryGenericListenerBuilder<Tor, TDes>.FunctionResultGenericListenerFactory(expression, Listener);
//        //    res.RegisterOnValue = true;
//        //    return res;
//        //}

//        //static public IResultListenerFactory<Tor, TDes> Register<Tor, TDes>(Expression<Func<Tor, TDes>> expression, string iAttributename) where Tor : NotifyCompleteListenerObject
//        //{
//        //    return GlobalFunctionListener.Register<Tor, TDes>(expression, iAttributename);
//        //}
//    }
//}
