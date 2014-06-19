using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.Implementation;

using MusicCollection.Nhibernate;
using MusicCollection.Nhibernate.Mapping;
using MusicCollection.Nhibernate.Session;
using MusicCollectionTest.Integrated.Session_Accessor;

namespace MusicCollectionTest.NHibernate
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Nhibernate")]
    internal class GenericClasses
    {
        private MyObject _My;
        private SessionKeys _SK;

        public void SetUp()
        {
            _My = new MyObject("Unitary",1);
            _SK = new SessionKeys(Path.GetFullPath(@"..\..\TestFolders\OutFolder"),false);
        }


        public void Test1()
        {
            ISessionFactory isf = _SK.Builder.GetNhibernateConfiguration(  DBFactoryBuilder.GetConfiguration).BuildSessionFactory();

            using (global::NHibernate.ISession sess = isf.OpenSession())
            {
            }
        }
    }
}
