using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MusicCollectionWPF.Infra
{
    public class MusicCollectionCommands
    {
        static MusicCollectionCommands()
        {
            Delete = new RoutedUICommand("Delete", "Delete", typeof(MusicCollectionCommands));
            Edit = new RoutedUICommand("Edit", "Edit", typeof(MusicCollectionCommands));
            Move = new RoutedUICommand("Move", "Move", typeof(MusicCollectionCommands));
            Export = new RoutedUICommand("Export", "Export", typeof(MusicCollectionCommands));

            EditTrack_RemoveTrackNumber = new RoutedUICommand("EditTrack_RemoveTrackNumber", "EditTrack_RemoveTrackNumber", typeof(MusicCollectionCommands));
            EditTrack_PrefixArtistName = new RoutedUICommand("EditTrack_PrefixArtistName", "EditTrack_PrefixArtistName", typeof(MusicCollectionCommands));
        }

        public static RoutedUICommand EditTrack_RemoveTrackNumber
        {
            get;
            private set;
        }

        public static RoutedUICommand EditTrack_PrefixArtistName
        {
            get;
            private set;
        }

        public static RoutedUICommand Delete
        {
            get;
            private set;
        }

        public static RoutedUICommand Edit
        {
            get;
            private set;
        }

        public static RoutedUICommand Move
        {
            get;
            private set;
        }

        public static RoutedUICommand Export
        {
            get;
            private set;
        }
    }
}
