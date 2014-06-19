using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MusicCollectionTest.TestObjects
{
    public class ConfigurationFileChanger : IDisposable
    {
        private string currentConfigFilePath;
        private string _newConfigFilePath;

        public ConfigurationFileChanger(string Mypath)
        {
            currentConfigFilePath = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();  
            _newConfigFilePath = Mypath;
            ChangeConfigFile(_newConfigFilePath);     
        }

        protected void ChangeConfigFile(string newConfigFilePath)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", newConfigFilePath);
            FieldInfo[] fiStateValues = null;

            Type initStateType = typeof(System.Configuration.ConfigurationManager).GetNestedType("InitState", BindingFlags.NonPublic);
            if (initStateType != null)
            {
                fiStateValues = initStateType.GetFields();
            }
            FieldInfo fiInit = typeof(System.Configuration.ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo fiSystem = typeof(System.Configuration.ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
            if (fiInit != null && fiSystem != null && null != fiStateValues)
            {
                fiInit.SetValue(null, fiStateValues[1].GetValue(null));
                fiSystem.SetValue(null, null);
            }
        }

        public void Dispose()
        {
            ChangeConfigFile(currentConfigFilePath);
        }
    }

}
