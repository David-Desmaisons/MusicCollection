using MusicCollection.FileConverter;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation
{
    internal interface IInternalMusicSession : IMusicSession
    {
        AlbumCollection Albums { get; }

        TrackCollection Tracks { get; }

        ArtistCollection Artists { get; }

        GenreCollection Genres { get; }

        IMusicConverter MusicConverter { get; }

        MusicFolderHelper Folders { get; }

        Nullable<bool> DBCleanOnOpen { get; }

        ISessionFactory GetNHibernateFactory();

        USBDriveListener DriverListener { get; }

        ImportTransaction GetNewSessionContext(AlbumMaturity iDefaultMaturity = AlbumMaturity.Discover);

        void AddFileTobeRemovedLater(string File, bool Reversible);

        IDisposable GetSessionLock();

    }
}
