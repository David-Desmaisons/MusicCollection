using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Collections.Specialized;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;

//DEM changes TR

namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal class ListenedElement
    {   
        internal ListenedElement(IObjectAttribute LO, PropertyInfo pi)
        {
            ListenedObject = LO;
            _Properties.Add(pi.Name,pi);
        }

        public override string ToString()
        {
            return string.Format("Object:<{0}> Attributes<{1}>",ListenedObject,string.Join(",",_Properties.Keys));
        }

        internal IObjectAttribute ListenedObject
        {
            get;
            private set;
        }

        internal bool Add(PropertyInfo pi)
        {
            return _Properties.FindOrCreate(pi.Name, s => pi).CollectionStatus == CollectionStatus.Find;
            //return _Properties.FindOrCreate(pi.Name, s => pi).Item1;
        }

        internal bool ContainsProperty(string PName)
        {
            return _Properties.ContainsKey(PName);
        }


        internal IDictionary<string, PropertyInfo> _Properties = new PolyMorphDictionary<string, PropertyInfo>();
        internal IEnumerable<PropertyInfo> Properties
        {
            get { return _Properties.Values; }
        }
    }

    //internal interface IListenedElementCollectionBuilder<Tor,TDes>: IVisitIObjectAttribute where Tor:class 
    //{
    //    ListenedElementCollection<Tor,TDes> GetCollection();
    //}

    internal interface IListenedElementCollectionBuilder : IVisitIObjectAttribute
    {
        ListenedElementCollection GetCollection();
    }


    internal class ListenedElementCollection
        //<Tor,TDes> where Tor:class
    {
        private ListenedElementCollection()
        {
        }

        private IDictionary<IObjectAttribute, ListenedElement> _ObjectAttributePropertiesListened = new PolyMorphSimpleDictionary<IObjectAttribute, ListenedElement>();
        private ISimpleSet<INotifyCollectionChanged> _CollectionListened = new PolyMorphSet<INotifyCollectionChanged>();
      

        internal IEnumerable<INotifyCollectionChanged> CollectionListened
        {
            get{return _CollectionListened;}
        }

        internal bool Contains(IObjectAttribute ioa)
        {
            return _ObjectAttributePropertiesListened.ContainsKey(ioa);
        }

        internal bool Contains(INotifyCollectionChanged ioa)
        {
            return _CollectionListened.Contains(ioa);
        }

        internal IEnumerable<ListenedElement> ObjectAttributePropertiesListened
        {
            get { return _ObjectAttributePropertiesListened.Values; }
        }

        internal IEnumerable<IObjectAttribute> ObjectAttributeProperties
        {
            get { return _ObjectAttributePropertiesListened.Keys; }
        }

        internal bool Contains(object potential)
        {
            IObjectAttribute ioa = potential as IObjectAttribute;
            if (ioa == null)
                return false;

            return Contains(ioa);
        }

        internal bool IsPertinent(ObjectModifiedArgs oma)
        {
            ListenedElement le = null;
            if (!_ObjectAttributePropertiesListened.TryGetValue(oma.ModifiedObject as IObjectAttribute, out le))
                return false;

            return le.ContainsProperty(oma.AttributeName);
        }

        private void Register(IObjectAttribute io, PropertyInfo myProperty)
        {
            var res = _ObjectAttributePropertiesListened.FindOrCreate(io, o => new ListenedElement(o, myProperty));
            if (res.CollectionStatus == CollectionStatus.Find)
            {
                res.Item.Add(myProperty);
            }
        }

        private void Register(INotifyCollectionChanged io)
        {
            _CollectionListened.Add(io);
        }

        private class ListenedElementCollectionBuilder : IListenedElementCollectionBuilder
            //<Tor, TDes>
        {
            //private ListenedElementCollection<Tor, TDes> _Result;
            private ListenedElementCollection _Result;

            internal ListenedElementCollectionBuilder()
            {    
                //_Result = new ListenedElementCollection<Tor, TDes>();
                _Result = new ListenedElementCollection();
            }

            //private ListenedElementCollection<Tor, TDes> Result
            //{
            //    get
            //    {
            //        return _Result;
            //    }
            //}

            public void Visit(IObjectAttribute io, PropertyInfo myProperty, bool Isparameter)
            {
                _Result.Register(io, myProperty);
            }

            public void Visit(INotifyCollectionChanged icc)
            {
                _Result.Register(icc);
            }

            //public ListenedElementCollection<Tor, TDes> GetCollection()
            public ListenedElementCollection GetCollection()
            {
                //var res = Result;
                //_Result = null;
                return _Result;
            }
        }

        //static private  IListenedElementCollectionBuilder<Tor, TDes> _Builder;
        //static private IListenedElementCollectionBuilder<Tor, TDes> Builder
        //{
        //    get
        //    {
        //        if (_Builder == null)
        //            _Builder = new ListenedElementCollectionBuilder();

        //        return new ListenedElementCollectionBuilder();
        //    }
        //}

        //static internal IListenedElementCollectionBuilder<Tor, TDes> GetBuilder()
        static internal IListenedElementCollectionBuilder GetBuilder()
        {
            //return Builder;
            return new ListenedElementCollectionBuilder();
        }
    }
}
