using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

using NHibernate;
using NHibernate.Cfg;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;

using MusicCollection.Nhibernate.Blob;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollection.Nhibernate.Session
{
    internal static class DBFactoryBuilder
    {
        internal static ISessionFactory GetFactory(IPersistenceConfigurer persistentConf)
        {
            return GetConfiguration(persistentConf).BuildSessionFactory();
        }

        public static Configuration GetConfiguration(IPersistenceConfigurer persistentConf)
        {
            return Fluently.Configure()
                .Database(persistentConf)
                .Mappings(m =>
                {
                    //foreach (Tuple<MappingClassAttribute, Type> T in AttributeExtender.GetMarkedTypeInTheAssembly<MappingClassAttribute>()) m.FluentMappings.Add(T.Item2);
                    AttributeExtender.GetMarkedTypeInTheAssembly<MappingClassAttribute>().Apply( t=>  m.FluentMappings.Add(t.Item2));
  
                    m.FluentMappings.Conventions.Add(new BufferConvention());
                }).BuildConfiguration();
        }
    }
}
