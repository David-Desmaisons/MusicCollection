using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IMultiEntityEditor : IAsyncCommiter, IDisposable 
        //IImporterEvent,
    {
        OptionChooserArtist AutorOption { get; }

        OptionChooser<string> NameOption { get; }

        OptionChooser<string> GenreOption { get; }

        OptionChooser<int?> YearOption { get; }

        IMusicSession Session { get; }

        //void Commit(IProgress<ImportExportErrorEventArgs> progress = null);

        //Task CommitAsync(IProgress<ImportExportErrorEventArgs> progress);

        void Cancel();

        //void CommitChanges(bool Sync);

        //event EventHandler<EventArgs> EndEdit;
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
