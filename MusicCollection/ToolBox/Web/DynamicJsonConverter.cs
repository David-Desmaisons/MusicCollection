using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

//from http://drewnoakes.com/ http://www.drowningintechnicaldebt.com/ShawnWeisfeld/Default.aspx

namespace MusicCollection.ToolBox.Web
{

    internal class DynamicJsonConverter : JavaScriptConverter
    {
        private DynamicJsonConverter()
        {
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        static private JavaScriptSerializer _serializer;

        static internal object DynamicDeSerialize(string text)
        {
           
                if (_serializer == null)
                {
                    _serializer = new JavaScriptSerializer();
                    _serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                }

                try
                {
                    return _serializer.Deserialize(text, typeof(object));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("Error on reading json"+e.ToString());
                    return null;
                }

        }

        #region Nested type: DynamicJsonObject

        private sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                return string.Join(",", _dictionary.Keys.
                    Select(o => { object member = null; 
                                  TryGetMember(o, out member);
                                  List<dynamic> l = member as List<dynamic>;
                                  if (l!=null) 
                                        return string.Format("{0}:{1}", o, string.Join(",",l)); 

                                  List<object> l2 = member as List<object>;
                                  if (l2!=null) 
                                        return string.Format("{0}:{1}", o, string.Join(",",l2)); 
                                              
                                    return string.Format("{0}:{1}", o, member);
                    }));
                                

            }

          

            private bool TryGetMember(string ibindername, out object result)
            {
                string bindername = ibindername;
                if (!_dictionary.TryGetValue(bindername, out result))
                {
                    bindername = ibindername.Replace('_', '-');
                    if (!_dictionary.TryGetValue(bindername, out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }
                }

                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                {
                    result = new DynamicJsonObject(dictionary);
                    return true;
                }

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    if (arrayList[0] is IDictionary<string, object>)
                        result = new List<dynamic>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
                    else
                        result = new List<object>(arrayList.Cast<object>());
                }

                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return TryGetMember(binder.Name, out result);
            }
        }

        #endregion
    }
}
