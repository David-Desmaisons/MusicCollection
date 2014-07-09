using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Threading;

using MusicCollection.DataExchange;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Web;
using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollection.Implementation;


namespace MusicCollection.WebServices.Discogs2
{
    [WebServicesInfoProvider(WebProvider.Discogs)]
    internal class Discogs2Finder : WebFinderAdapter, IInternetServiceListener, IInternerInformationProvider, IHttpContextFurnisher
    {
        private string _UA;
        private int? _TOut;
        private bool _Activated;
        private OAuthManager _AuthManager;

        public Discogs2Finder(IWebUserSettings iwsm)
        {
            _Activated = iwsm.DiscogsActivated;
            _UA = @"Discogs2Net https://sourceforge.net/projects/discog";
            _TOut = iwsm.DiscogsTimeOut * 1000;
            _AuthManager = new DiscogsAuthentificationProvider(iwsm).GetDiscogsOAuthManager();
        }

        private class InternetServiceListener : WebFinderAdapterBase, IInternetServiceListener
        {
            private List<InternetFailedArgs> _Failures;
            internal List<InternetFailedArgs> Failures
            {
                get
                {
                    if (_Failures == null)
                        _Failures = new List<InternetFailedArgs>();

                    return _Failures;
                }
            }

            internal bool IsOK
            {
                get { return (_Failures == null); }
            }


            protected override void OnEvent(InternetFailedArgs ifa)
            {
                Failures.Add(ifa);
            }
        }

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellationToken)
        {
            if (_Activated == false)
                yield break;

            if ( (query==null) || (query.Type!=QueryType.FromAlbumInfo))
                yield break;

            IAlbumDescriptor lookingFor = query.AlbumDescriptor;
            if (lookingFor == null)
                yield break;

            Discogs2HttpRequestCreator src = new Discogs2HttpRequestCreator(_UA, _TOut);

            HttpJsonInterpretor hji = new HttpJsonInterpretor(src.GetSearchRequest(lookingFor));

            dynamic myres = hji.GetObjectResponse(this);
            if (myres == null)
                yield break; 
            
            int Sizemax = myres.results.Count;
            if (Sizemax == 0)
                yield break;
          
            int FoundItem = 0;

            InternetServiceListener isl = new InternetServiceListener();

            bool needcovers = (query.NeedCoverArt) && (_AuthManager!=null);

            if (iCancellationToken.IsCancellationRequested)
                yield break;

            for (int i = 0; i < Sizemax; i++)
            {
               Thread.Sleep(1000);

                if (iCancellationToken.IsCancellationRequested)
                    yield break;

                hji = new HttpJsonInterpretor(src.FromUrl(myres.results[i].resource_url));
                dynamic albumresponse = hji.GetObjectResponse(isl);
                if (albumresponse == null)
                   continue;

                AlbumDescriptor res = AlbumDescriptor.FromDiscogs(albumresponse, needcovers, _AuthManager, Context, iCancellationToken);

                if (res != null)
                {
                    FoundItem++;
                    yield return new Match<AlbumDescriptor>(res, MatchPrecision.Suspition);

                    if (FoundItem == query.MaxResult)
                        break;
                }

                if (iCancellationToken.IsCancellationRequested)
                    yield break;

            }

            if (!isl.IsOK)
            {
                FireEvent(InternetFailedArgs.PartialResult(isl.Failures));
            }
        }

        public void Dispose()
        {
        }

        public HttpContext Context
        {
            get { return new HttpContext(_UA, null); }
        }
    }
}
