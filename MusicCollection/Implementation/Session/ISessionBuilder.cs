using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MusicCollection.Fundation;
using MusicCollection.FileConverter;

namespace MusicCollection.Implementation.Session
{
    internal interface ISessionBuilder
    {
        MusicFolderHelper Folders { get; }

        IInfraDependencies InfraTools { get;}

        IMusicSettings SettingFactory { get; }

        string OnSessionClose { get; }

        Nullable<bool> DBCleanOnOpen { get; }

        Func<IMusicConverter> MusicConverterBuilder { get; }

        Configuration GetNhibernateConfiguration(Func<IPersistenceConfigurer, Configuration> ConfigBuilder);

    }
}
