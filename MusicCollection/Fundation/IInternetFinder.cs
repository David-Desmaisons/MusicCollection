using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using MusicCollection.Infra;
using MusicCollection.DataExchange;
using System.Threading;
using System.Threading.Tasks;


namespace MusicCollection.Fundation
{
    public enum QueryType 
    { 
        FromCD,
        FromAlbumInfo
    }


    public interface IWebQuery
    {
        bool NeedCoverArt { get; set; }

        QueryType Type { get; }

        int MaxResult { get; set; }

        IAlbumDescriptor AlbumDescriptor { get; }

        ICDInfoHandler CDInfo  { get; }    
    }

    public interface IWebQueryFactory
    {
        IWebQuery FromAlbumDescriptor(IAlbumDescriptor iad);

        IWebQuery FromCDInfo(ICDInfoHandler iad);
    }

    public interface IWebResult
    {
        IList<WebMatch<IFullAlbumDescriptor>> Found { get; }
    }

    public interface IInternetFinder
    {
        event EventHandler<InternetFinderResultEventArgs> OnResult;
        event EventHandler<InternetFailedArgs> OnInternetError;

        IWebQuery Query { get;}
  
        IWebResult Result { get; }

        bool IsValid { get; }

        void Compute(CancellationToken iCancellationToken);

        Task ComputeAsync(CancellationToken iCancellationToken);

        void Cancel();
    }

   
}
