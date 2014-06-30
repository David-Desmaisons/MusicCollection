using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Drawing;

using MusicCollection.Infra;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IModifiableAlbum : INotifyPropertyChanged , IDisposable
    {
        //event EventHandler<ImportExportErrorEventArgs> Error;
        //event EventHandler<EventArgs> EndEdit;

        IList<IArtist> Artists {get;}


        string MainDirectory { get; }

        string Name  {get;set;}

        string Genre{get;set;}
       
        int Year {get;set;}

        IAlbumPicture FrontImage { get; }

        ObservableCollection<IAlbumPicture> Images { get; }

        ObservableCollection<IModifiableTrack> Tracks { get; }

        //bool? Commit(bool Sync);
        bool Commit(IProgress<ImportExportErrorEventArgs> progress=null);

        Task<bool> CommitAsync(IProgress<ImportExportErrorEventArgs> progress);

        void CancelChanges();

        IAlbum OriginalAlbum { get; }

        IMusicSession Session { get; }

        string CreateSearchGoogleSearchString();
 
        IAlbumPicture AddAlbumPicture(string FileName, int Index);

        IAlbumPicture AddAlbumPicture(BitmapSource BMS, int Index);

        IAlbumPicture SplitImage(int Index);

        IAlbumPicture GetAlbumPictureFromUri(string uri, int Index,IHttpContextFurnisher Context=null);

        IAlbumPicture RotateImage(int Index, bool angle);

        IAlbumDescriptor GetAlbumDescriptor();

        void MergeFromMetaData(IFullAlbumDescriptor iad, IMergeStrategy Strategy);

        Task MergeFromMetaDataAsync(IFullAlbumDescriptor iad, IMergeStrategy Strategy);

        void ReinitImages();
     
       
    }
}
