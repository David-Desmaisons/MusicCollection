using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Implementation.Session;
using MusicCollection.SettingsManagement;
using MusicCollection.Fundation;

namespace MusicCollectionTest.Integrated.Session_Accessor
{
    public class SessionKeys
    {
        private ISessionBuilder _SessionBuilder;
        internal ISessionBuilder Builder
        {
            get { return _SessionBuilder; }
        }

        private ManualSettings _Settings;
        internal ManualSettings Settings
        {
            get { return _Settings; }
        }

        internal SessionKeys(string iOut, Nullable<bool> OpenClean, bool UseBassMusicConverter)
        {
            _Out = iOut;
            _Settings = new ManualSettings();

            if (UseBassMusicConverter)
            {
                var conv = new StandardConverterSettings();
                var convus = new ManualConverterSettings();
                convus.BassPassword = conv.BassPassword;
                convus.BassUser = conv.BassUser;
                _Settings.ConverterUserSettings = convus;
            }
            _SessionBuilder = GetFrom(OpenClean, _Settings, iOut);
        }


        internal SessionKeys(string iOut, Nullable<bool> OpenClean)
        {
            _Out = iOut;
            ////_In = iIn;
            _Settings = new ManualSettings();
            //_SessionBuilder = SessionBuilder.GetSessionBuilder(SessionBuilder.DBtype.SQLite, OpenClean, _Settings, iOut);
            _SessionBuilder = GetFrom(OpenClean, _Settings, iOut);

        }

        internal SessionKeys(string iOut)
        {
            _Out = iOut;
            ////_In = iIn;
            _Settings = new ManualSettings();
            //_SessionBuilder = SessionBuilder.GetSessionBuilder(SessionBuilder.DBtype.SQLite, _Settings, iOut);
            _SessionBuilder = GetFrom(_Settings, iOut);
        }

        internal virtual ISessionBuilder GetFrom(bool? OpenClean, ManualSettings ms, string o)
        {
            return SessionBuilder.GetSessionBuilder(SessionBuilder.DBtype.SQLite, OpenClean, ms, o);
        }

        internal virtual ISessionBuilder GetFrom(ManualSettings ms, string o)
        {
            return SessionBuilder.GetSessionBuilder(SessionBuilder.DBtype.SQLite, ms, o);
        }



        private readonly string _Out;


        internal string OutPath
        {
            get { return _Out; }
        }

    }

    public class DummySessionKeys : SessionKeys
    {
        internal DummySessionKeys(string iOut, Nullable<bool> OpenClean)
            : base(iOut, OpenClean)
        {
        }

        internal override ISessionBuilder GetFrom(bool? OpenClean, ManualSettings ms, string o)
        {
            return new DummySessionBuilder(new MusicFolderHelper(o), ms);
        }

        internal override ISessionBuilder GetFrom(ManualSettings ms, string o)
        {
            return new DummySessionBuilder(new MusicFolderHelper(o), ms);
        }
    }


}
