using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Implementation.Session;
using MusicCollection.ToolBox;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.Nhibernate.Session;
using MusicCollection.FileConverter;
using MusicCollection.MusicPlayer;

namespace MusicCollectionTest.Integrated.Session_Accessor
{
    internal class DummySessionBuilder : ISessionBuilder
    {
        private IInfraDependencies _IInfraDependencies;
        public DummySessionBuilder(MusicFolderHelper iPath, ManualSettings settings)
        {
            Folders = iPath;
            SettingFactory = settings;
            _IInfraDependencies = new CurrentInfraTools(new WindowsMusicFactory());
        }

        public MusicFolderHelper Folders
        {
            get;
            private set;
        }

        public IntPtr MainWindowsHandle
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            }
        }

        private string InitSQLiteData()
        {
            try
            {
                string mypath = Path.Combine(Folders.Root, "MusicCollection.db");
                if (File.Exists(mypath))
                    return mypath;

                //recuperons le fichier embedded
                using (Stream str = Assembly.GetAssembly(typeof(Album)).GetManifestResourceStream("MusicCollection.Data.SQLiteData.MusicCollection.db"))
                {
                    using (FileStream fs = new FileStream(mypath, FileMode.CreateNew, FileAccess.Write))
                    {
                        str.CopyTo(fs);
                    }
                }

                return mypath;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw e;
            }
        }

        private IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return SQLiteConfiguration.Standard.UsingFile(InitSQLiteData()).MaxFetchDepth(2);
        }

        Configuration ISessionBuilder.GetNhibernateConfiguration(Func<IPersistenceConfigurer, Configuration> ConfigBuilder)
        {
            return Fluently.Configure()
                .Database(GetPersistenceConfigurer())
                .Mappings(m =>
                {
                    //foreach (Tuple<MappingClassAttribute, Type> T in AttributeExtender.GetMarkedTypeInTheAssembly<MappingClassAttribute>()) m.FluentMappings.Add(T.Item2);
                    typeof(Album).Assembly.GetMarkedType<MappingClassAttribute>().Apply(t => m.FluentMappings.Add(t.Item2));
                    m.FluentMappings.Conventions.Add(new DummyBufferConvention());
                }).BuildConfiguration();
        }

        public IMusicSettings SettingFactory
        {
            get;
            private set;
        }

        public string OnSessionClose
        {
            get { return String.Empty; }
        }

        public bool? DBCleanOnOpen
        {
            get { return true; }
        }


        public IInfraDependencies InfraTools
        {
            get { return _IInfraDependencies; }
            set { _IInfraDependencies = value; }
        }

        internal IMusicConverter MusicConverter {get;set;}
        public Func<IMusicConverter> MusicConverterBuilder
        {
            get
            {
                return () =>
                    {
                        if (MusicConverter == null)
                            MusicConverter = BassMusicConverter.GetBassConverter(SettingFactory.ConverterUserSettings).Result;

                        return MusicConverter;
                    };
            }
        }
    }
}
