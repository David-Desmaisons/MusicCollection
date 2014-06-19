using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IMultiEntityEditor : IImporterEvent, IDisposable
    {
        OptionChooserArtist AutorOption
        {
            get;
        }

        OptionChooser<string> NameOption
        {
            get;
        }

        OptionChooser<string> GenreOption
        {
            get;
        }

        OptionChooser<int?> YearOption
        {
            get;
        }

        IMusicSession Session
        {
            get;
        }

        void Cancel();

        void CommitChanges(bool Sync);

        event EventHandler<EventArgs> EndEdit;

    }

    public interface ISingleTrackEditor : IMultiEntityEditor
    {
        string Artist
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        uint TrackNumber
        {
            get;
            set;
        }

        uint DiscNumber
        {
            get;
            set;
        }
    }
}
