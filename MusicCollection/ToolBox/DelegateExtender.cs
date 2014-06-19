//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace MusicCollection.ToolBox
//{
//    internal static class DelegateExtender
//    {
//        static internal T Convert<T>(this Delegate source) where T:class
//        {
//            if (source.GetInvocationList().Length > 1)
//                throw new ArgumentException("Cannot safely convert MulticastDelegate");

//            return Delegate.CreateDelegate(typeof(T), source.Target, source.Method) as T;
//        }
//    }
//}
