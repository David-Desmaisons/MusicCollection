using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IMultiEntityEditor : IAsyncCommiter, IDisposable 
    {
        //OptionChooserArtist AutorOption { get; }

        string Author { get; set; }

        OptionArtistChooser ArtistOption { get; }

        OptionChooser<string> NameOption { get; }

        OptionChooser<string> GenreOption { get; }

        OptionChooser<int?> YearOption { get; }

        void Cancel();
    }

    public interface ISingleTrackEditor : IMultiEntityEditor
    {
        string Artist { get; set; }

        string Name { get; set; }

        uint TrackNumber { get; set; }

        uint DiscNumber { get; set; }
    }
}
