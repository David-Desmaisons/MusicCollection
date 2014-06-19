//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Reflection;

//using MusicCollection.ToolBox.Collection;
//using MusicCollection.ToolBox.LambdaExpressions;
//using MusicCollection.Infra;

//namespace MusicCollection.ToolBox.FunctionListener
//{
//    internal class GlobalFunctionListener
//    {
//        //static readonly IDictionary<Type, FunctionListenerBuilder> _Mapping = new Dictionary<Type, FunctionListenerBuilder>();
//        //static readonly IDictionary<Type, List<FunctionListenerBuilder>> _MappingIncludingDerivation = new Dictionary<Type, List<FunctionListenerBuilder>>();


//        //static public IResultListenerFactory<Tor, TDes> Register<Tor, TDes>(Expression<Func<Tor, TDes>> expression, string iAttributename) where Tor : NotifyCompleteListenerObject
//        //{
//        //    Type t = typeof(Tor);
//        //    FunctionListenerBuilder res = _Mapping.FindOrCreateEntity(t, (to) => new FunctionListenerBuilder(to));
//        //    IResultListenerFactory<Tor, TDes> myfr = FactoryListenerBuilder<Tor, TDes>.FunctionResultListenerFactory(expression, iAttributename);
//        //    res.AddFactory(myfr);
//        //    return myfr;
//        //}

//        //private static IEnumerable<FunctionListenerBuilder> GetRelevantBaseTypes(Type first)
//        //{
//        //    return _MappingIncludingDerivation.FindOrCreateEntity(first, (f) => BuildRelevantBaseTypes(f).ToList());
//        //}

//        //private static IEnumerable<FunctionListenerBuilder> BuildRelevantBaseTypes(Type first)
//        //{
//        //    Type t = first;
            
//        //    while ( (t != typeof(NotifyCompleteListenerObject)) && (t!=typeof(object)))
//        //    {
//        //        FunctionListenerBuilder candidat = null;
//        //        if (_Mapping.TryGetValue(t, out candidat)) 
//        //            yield return candidat;
    
//        //        t = t.BaseType;
//        //    }
//        //}

//        //static internal RawResultListenerCollection GetFromInstance(object o)
//        //{
//        //    var results = _MappingIncludingDerivation.FindOrCreateEntity(o.GetType(), (f) => BuildRelevantBaseTypes(f).ToList());
//        //    return new RawResultListenerCollection(o, results);
//        //}
//    }
//}
