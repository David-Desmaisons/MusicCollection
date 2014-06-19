using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
//using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Diagnostics;

//using MusicCollection.AmazonAws;
using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using MusicCollection.Implementation;
using System.Threading;


namespace MusicCollection.WebServices.Amazon
{
    [WebServicesInfoProvider(WebProvider.Amazon)]
    internal class AmazonFinder : WebFinderAdapter, IInternerInformationProvider
    {
        private AWSECommerceServicePortTypeClient _AmazonClient;
        private IWebUserSettings _IWebServicesManager;

        public AmazonFinder(IWebUserSettings iwsm)
        {
            _IWebServicesManager = iwsm;

            if (_IWebServicesManager.AmazonActivated == false)
                return;

            try
            {
                _AmazonClient = new AWSECommerceServicePortTypeClient(_IWebServicesManager);
                //_AmazonClient = new AWSECommerceServicePortTypeClient();
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Amazon Web Services problem");
                Trace.WriteLine(ex.ToString());
            }
        }

        private Items[] LookForAsin(string iAsin)
        {
            ItemLookupRequest request = new ItemLookupRequest(); 
            request.ResponseGroup = new string[] { "Medium", "Tracks" };
            request.ItemId = new string[] {iAsin};
            request.IdType = ItemLookupRequestIdType.ASIN;
            //request.IdTypeSpecified = true;
            request.Condition = Condition.All;
            request.IncludeReviewsSummary = "False";
            //request.SearchIndex = "All";
       
            //myItemLookup.Validate = "False";
            //myItemLookup.XMLEscaping = "Single";

            ItemLookup myItemLookup = new ItemLookup();
            myItemLookup.Request = new ItemLookupRequest [] {request};
            //myItemLookup.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            //myItemLookup.AssociateTag = ConfigurationManager.AppSettings["secretKey"];

            myItemLookup.AWSAccessKeyId = _IWebServicesManager.AmazonaccessKeyId;
            myItemLookup.AssociateTag = _IWebServicesManager.AmazonsecretKey;
      
            ItemLookupResponse response = _AmazonClient.ItemLookup(myItemLookup);
            return response.Items;
        }

        private Items[] LookForArtistAndName(string iArtist, string iName)
        {
            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = "Music";
            request.ResponseGroup = new string[] { "Medium", "Tracks" };
            request.Sort = "relevancerank";
            request.Title = iName;
            request.Artist = iArtist;
         
            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            //itemSearch.AWSAccessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            itemSearch.AWSAccessKeyId = _IWebServicesManager.AmazonaccessKeyId;
            itemSearch.AssociateTag = string.Empty;

            // send the ItemSearch request
            ItemSearchResponse response = _AmazonClient.ItemSearch(itemSearch);
            return response.Items;
        }
      

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellationToken)
        {
            if (_AmazonClient == null)
                yield break;

            if ((query == null) || (query.Type != QueryType.FromAlbumInfo))
                yield break;

            AlbumDescriptorQuery adq = query as AlbumDescriptorQuery;
            if (adq == null)
                yield break;

            IAlbumDescriptor lookingFor = adq.AlbumDescriptor;
            if (lookingFor == null)
                yield break;

            Items[] result = null;

            try
            {
                result = (string.IsNullOrEmpty(lookingFor.IDs.Asin)) ?
                    LookForArtistAndName(lookingFor.Artist, lookingFor.Name) :
                    LookForAsin(lookingFor.IDs.Asin);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                OnWebExeption(ex);
                yield break;
            }

            if (( result == null)  ||  (result.Length==0) ) 
                yield break;

            Item[] Items = result[0].Item;

            if ((Items == null) || (Items.Length == 0))
                yield break;

            //int Sizemax = (query.MaxResult == -1) ? Items.Length : Math.Min(Items.Length, query.MaxResult);
            int Sizemax = Items.Length;
            int FoundNumber=0;

            bool NeedCovers = query.NeedCoverArt;

            for (int i=0;i< Sizemax;i++)
            {
                if (iCancellationToken.IsCancellationRequested)
                    yield break;

                if (FoundNumber == query.MaxResult)
                    break;

                AlbumDescriptor candidat = AlbumDescriptor.FromAmazonItem(Items[i], NeedCovers,iCancellationToken);
                if (candidat != null)
                {
                    FoundNumber++;
                    yield return new Match<AlbumDescriptor>(candidat, MatchPrecision.Suspition);

                    if (FoundNumber == query.MaxResult)
                        break;
                }
            }
       }

        public void Dispose()
        {
            IDisposable amd = _AmazonClient;
            amd.Dispose();
        }
    }
}
