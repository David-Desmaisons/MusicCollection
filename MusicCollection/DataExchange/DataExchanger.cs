using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;


namespace MusicCollection.DataExchange
{
    internal class DataExchanger<Tc> where Tc : class, new()
    {
        private Tc _TD;

        static readonly private IDictionary<DataExportImportType, IDictionary<String, PropertyInfo>> _AttributebyXMLKeys =
            new SortedList<DataExportImportType, IDictionary<String, PropertyInfo>>();

        internal DataExchanger(Tc Root)
        {
            _TD = Root;
        }

        protected DataExchanger(Func<Tc> fact = null)
        {
            _TD = (fact == null) ? new Tc() : fact();
        }


        static private void Register(MusicObjectAttributeMappingAttribute itamp, PropertyInfo pi)
        {
            IDictionary<String, PropertyInfo> res = null;
            _AttributebyXMLKeys.TryGetValue(itamp.TypeofImport, out res);

            if (res == null)
            {
                res = new SortedList<String, PropertyInfo>();
                _AttributebyXMLKeys.Add(itamp.TypeofImport, res);
            }

            res.Add(itamp.AttributeName, pi);
        }

        static DataExchanger()
        {
            PropertyInfo[] PIs = typeof(Tc).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo PI in PIs)
            {
                object[] iTuA = PI.GetCustomAttributes(typeof(MusicObjectAttributeMappingAttribute), false);
                if (iTuA != null)
                {
                    foreach (MusicObjectAttributeMappingAttribute CustAttr in iTuA)
                    {
                        Register(CustAttr, PI);
                    }
                }
            }
        }

        private static PropertyInfo GetPropertyInfoFromName(DataExportImportType dit, string Name)
        {
            IDictionary<String, PropertyInfo> res = null;
            _AttributebyXMLKeys.TryGetValue(dit, out res);

            if (res == null)
                return null;

            PropertyInfo PI = null;
            res.TryGetValue(Name, out PI);
            return PI;
        }



        internal void DescribeAttribute(string Attribute, Func<string> Value, DataExportImportType dit)
        {
            PropertyInfo PI = GetPropertyInfoFromName(dit, Attribute);
            if (PI == null)
                return;

            try
            {
                TypeConverter TypeConvertor = TypeDescriptor.GetConverter(PI.PropertyType);
                PI.SetValue(_TD, TypeConvertor.ConvertFromString(Value()), null);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Problem reading data {0}", ex));
            }
        }

        internal void Describe(DataExportImportType dit, IObserver<KeyValuePair<string,object>> Description)
        {
            IDictionary<String, PropertyInfo> res = null;
            _AttributebyXMLKeys.TryGetValue(dit, out res);

            if (res == null)
            {
                Description.OnCompleted();
                return;
            }

            foreach (KeyValuePair<String, PropertyInfo> kp in res)
            {
                try
                {
                    Description.OnNext(new KeyValuePair<string, object>(kp.Key, kp.Value.GetValue(_TD, null)));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Problem reading data {0} {1}",kp, ex));
                }
            }

            Description.OnCompleted();
        }

        protected Tc Object
        {
            get { return _TD; }
        }

    }
}
