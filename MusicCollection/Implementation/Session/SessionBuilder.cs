using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.FileConverter;
using MusicCollection.MusicPlayer;


//DEM Changes TR

namespace MusicCollection.Implementation.Session
{
    static internal class SessionBuilder
    {
        internal enum DBtype
        {
            MySQL,
            SQLite
        }

        [Serializable]
        private class StandardBuilder : ISessionBuilder, ISerializable
        {
            internal StandardBuilder(DBtype type, MusicFolderHelper iPath, ManualSettings ism)
                : this(type, iPath, ism, true)
            {
            }

            private DBtype _Type;
            private bool _FromSetting = false;
            private Configuration _Cfg;
            private bool _NeedToSerialize = true;


            internal StandardBuilder(DBtype type, MusicFolderHelper iPath, ManualSettings ism, Nullable<bool> CDBOOpen)
            {
                _Type = type;
                _Folders = iPath;
                _FromSetting = (ism == null);
                _ISM = ism ?? SettingsBuilder.FromUserSetting();
                DBCleanOnOpen = CDBOOpen;
                _NeedToSerialize = true;
            }

            private StandardBuilder(SerializationInfo info, StreamingContext context)
            {
                _Type = (DBtype)info.GetValue("Type", typeof(DBtype));
                _FromSetting = info.GetBoolean("FromSetting");
                if (_FromSetting)
                {
                    _ISM = SettingsBuilder.FromUserSetting();
                }
                else
                {
                    _ISM = (IMusicSettings)info.GetValue("Session Factory", typeof(ManualSettings));
                }

                _Cfg = (Configuration)info.GetValue("Configuration", typeof(Configuration));
                DBCleanOnOpen = (bool?)info.GetValue("DBCleanOnOpen", typeof(bool?));
                _NeedToSerialize = false;
            }


            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (_Cfg == null)
                    throw new Exception("Need a configuration to serialiaze");

                info.AddValue("Type", _Type);
                info.AddValue("FromSetting", _FromSetting);
                if (!_FromSetting)
                {
                    info.AddValue("Session Factory", _ISM);
                }
                info.AddValue("Configuration", _Cfg);
                info.AddValue("DBCleanOnOpen", DBCleanOnOpen);
            }

            private IMusicSettings _ISM;
            public IMusicSettings SettingFactory
            {
                get { return _ISM; }
            }

            private MusicFolderHelper _Folders;
            public MusicFolderHelper Folders
            {
                get { return _Folders; }
                internal set { _Folders = value; }
            }

            internal void Check()
            {
                if (this._Type == DBtype.SQLite)
                {
                    InitSQLiteData();
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
                    using (Stream str = Assembly.GetAssembly(typeof(StandardBuilder)).GetManifestResourceStream("MusicCollection.Data.SQLiteData.MusicCollection.db"))
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
                    throw;
                }
            }


            private IPersistenceConfigurer GetPersistenceConfigurer()
            {
                switch (_Type)
                {
                    case DBtype.MySQL:
                        return MySQLConfiguration.Standard.ConnectionString(c => c.Is("Server=localhost;Database=Musictest;User=root;Password=BBCasa;charset=utf8")).MaxFetchDepth(2);

                    case DBtype.SQLite:
                        return SQLiteConfiguration.Standard.UsingFile(InitSQLiteData()).MaxFetchDepth(2);
                }

                throw new Exception();
            }

            public string OnSessionClose
            {
                get
                {
                    switch (_Type)
                    {
                        case DBtype.MySQL:
                            return null;

                        case DBtype.SQLite:
                            return "VACUUM;";
                    }

                    throw new Exception();
                }
            }

            public Nullable<bool> DBCleanOnOpen
            {
                get;
                private set;
            }

            private void Save()
            {
                string ffile = Path.Combine(this._Folders.Root, FileName);

                IFormatter formatter = new BinaryFormatter();

                using (FileStream s = File.Create(ffile))
                {
                    formatter.Serialize(s, this);
                }
            }

            public Configuration GetNhibernateConfiguration(Func<IPersistenceConfigurer, NHibernate.Cfg.Configuration> ConfigBuilder)
            {
                if (_Cfg == null)
                {
                    _Cfg = ConfigBuilder(GetPersistenceConfigurer());

                    if (_NeedToSerialize)
                    {
                        Save();
                    }

                }

                return _Cfg;
            }

            private BassMusicConverter _BassMusicConverter;
            private BassMusicConverter GetBassMusicConverter()
            {
                if (_BassMusicConverter != null)
                    return _BassMusicConverter;

                _BassMusicConverter = BassMusicConverter.GetBassConverter(SettingFactory.ConverterUserSettings).Result;
                return _BassMusicConverter;
            }

            public IInfraDependencies InfraTools
            {
                get { return new CurrentInfraTools(GetBassMusicConverter()); }
            }


            public Func<IMusicConverter> MusicConverterBuilder
            {
                get
                {
                    return () => GetBassMusicConverter();
                }
            }

            //public IInfraDependencies InfraTools
            //{
            //    get { return new CurrentInfraTools(new WindowsMusicFactory()); }
            //}


            //public Func<IMusicConverter> MusicConverterBuilder
            //{
            //    get
            //    {
            //        return () => BassMusicConverter.GetBassConverter(SettingFactory.ConverterUserSettings).Result;
            //    }
            //}
        }

        private const string FileName = "Cfg.bin";

        static private StandardBuilder GetFromFile(MusicFolderHelper mfh)
        {
            try
            {
                using (TimeTracer.TimeTrack("Load Nhibernate Confi file"))
                {
                    string ffile = Path.Combine(mfh.Root, FileName);
                    if (!File.Exists(ffile))
                        return null;

                    IFormatter formatter = new BinaryFormatter();

                    using (FileStream s = File.OpenRead(ffile))
                    {
                        StandardBuilder cfg = (StandardBuilder)formatter.Deserialize(s);
                        return cfg;
                    }
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Problem while Reading Nhibernate cfg {0}", e));
                return null;
            }
        }

        static private ISessionBuilder CreateSessionBuilder(MusicFolderHelper mfh, Func<ISessionBuilder> Create)
        {
            StandardBuilder Fromstream = GetFromFile(mfh);
            if (Fromstream != null)
            {
                Fromstream.Folders = mfh;
                Fromstream.Check();
                return Fromstream;
            }

            return Create();
        }


        static internal ISessionBuilder GetSessionBuilder(DBtype iType, Nullable<bool> CDBOOpen, ManualSettings ism = null, string RootPath = null)
        {
            MusicFolderHelper mfh = new MusicFolderHelper(RootPath);
            return CreateSessionBuilder(mfh, () => new StandardBuilder(iType, mfh, ism, CDBOOpen));
        }

        static internal ISessionBuilder GetSessionBuilder(DBtype iType, ManualSettings ism = null, string RootPath = null)
        {
            MusicFolderHelper mfh = new MusicFolderHelper(RootPath);
            return CreateSessionBuilder(mfh, () => new StandardBuilder(iType, mfh, ism));
        }

        static internal ISessionBuilder FromSettings()
        {
            MusicFolderHelper mfh = new MusicFolderHelper();
            return CreateSessionBuilder(mfh, () => new StandardBuilder(DBtype.SQLite, mfh, null));
        }

    }
}
