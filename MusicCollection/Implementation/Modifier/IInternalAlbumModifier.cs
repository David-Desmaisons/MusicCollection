using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TagLib;

using MusicCollection.Fundation;

namespace MusicCollection.Implementation.Modifier
{
    internal interface IInternalAlbumModifier : IModifiableAlbum
    {
        bool IsAlbumOnlyModified {get;}

        bool AuthorDirty { get; }

        bool NeedToUpdateFile { get; }
        
        string NewName {get;}

        string NewGenre{get;}

        string ArtistName {get;}

        int? NewYear { get; }

        IDiscIDs CDIDs { get; }

        bool IsImageDirty{get;}

        IPicture[] PictureTobeStored{get;}

        IImportContext Context { get; }

        void Remove(TrackModifier TM);
                      
    }
}
