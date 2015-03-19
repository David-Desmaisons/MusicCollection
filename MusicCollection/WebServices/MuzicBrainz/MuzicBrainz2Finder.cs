using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using System.Net;
using MusicCollection.ToolBox.Web;
using System.Threading;


namespace MusicCollection.WebServices.MuzicBrainz
{
    [WebServicesInfoProvider(WebProvider.MusicBrainz)]
    internal class MuzicBrainzFinder : WebFinderAdapter, IInternerInformationProvider, IInternetServiceListener
    {
        private string _User;
        private string _Password;
        public MuzicBrainzFinder(IWebUserSettings iwsm)
        {
            _User = iwsm.MuzicBrainzUser;
            _Password = iwsm.MuzicBrainzPassword;
        }


        private IEnumerable<Match<AlbumDescriptor>> SearchFromCDQuery(ICDInfoHandler CDUnit, bool NeedCover, CancellationToken iCancellationToken)
        {
            if (CDUnit == null)
                yield break;

            if (!CDUnit.IsReady)
                yield break;

            string discid = CDUnit.IDs.MusicBrainzCDId;

            HttpJsonInterpretor hji =
                new HttpJsonInterpretor(MusicBrainzHttpCreator.ForCDIdSearch().SetValue(discid).BuildRequest(_User,_Password));

            dynamic myres = hji.GetObjectResponse(this);
            if (myres == null)
                yield break;

            foreach (AlbumDescriptor ad in MusicBrainzJsonInterpretor.FromMusicBrainzReleaseResults(myres, discid, iCancellationToken, NeedCover))
            {
                yield return new Match<AlbumDescriptor>(ad, MatchPrecision.Suspition);
            }
        }

        private Match<AlbumDescriptor> GetFromMusicBrainzId(string mzid, bool NeedCover, CancellationToken iCancellationToken, MatchPrecision iMatchPrecision = MatchPrecision.Exact)
        {
            if (mzid == null)
                return null;

            HttpJsonInterpretor jsoncon =
                  new HttpJsonInterpretor(MusicBrainzHttpCreator.ForReleaseIdSearch().SetValue(mzid).BuildRequest(_User, _Password));

            dynamic res = jsoncon.GetObjectResponse();

            if (res == null)
                return null;

            AlbumDescriptor alres = MusicBrainzJsonInterpretor.FromMusicBrainzRelease(res, NeedCover, iCancellationToken);
            if (alres == null)
                return null;

            return new Match<AlbumDescriptor>(alres, iMatchPrecision );
        }

        private IEnumerable<Match<AlbumDescriptor>> SearchFromDiscQuery(IAlbumDescriptor AlbumDescriptor, bool NeedCover, CancellationToken iCancellationToken)
        {
            string mzid = AlbumDescriptor.IDs.MusicBrainzID;
            if (!string.IsNullOrEmpty(mzid))
            {
                Match<AlbumDescriptor> res = GetFromMusicBrainzId(mzid, NeedCover, iCancellationToken);
                if (res != null)
                    yield return res;

                yield break;
            }

            HttpJsonInterpretor jsonconv =
                new HttpJsonInterpretor(MusicBrainzHttpCreator.ForReleaseSearch().SetArtist(AlbumDescriptor.Artist).SetDiscName(AlbumDescriptor.Name).BuildRequest(_User, _Password));

            dynamic dynamicres = jsonconv.GetObjectResponse();

            if (iCancellationToken.IsCancellationRequested)
                yield break;

            if  ((dynamicres == null) || (dynamicres.count == 0))
                yield break;

            List<dynamic> Releases = dynamicres.releases;
            if (Releases == null)
                yield break;

            foreach (dynamic rel in Releases)
            {  
                if (iCancellationToken.IsCancellationRequested)
                    break;

                Match<AlbumDescriptor> res = GetFromMusicBrainzId(rel.id, NeedCover, iCancellationToken, MatchPrecision.Suspition);
                if (res != null)
                    yield return res;       
            }
        }

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellationToken)
        {
            if (query == null)
                yield break;

            bool needcovers = query.NeedCoverArt;

            IEnumerable<Match<AlbumDescriptor>> rescoll = null;


            if (query.Type == QueryType.FromCD)
            {
                rescoll = SearchFromCDQuery(query.CDInfo, needcovers, iCancellationToken);
            }
            else
            {
                rescoll = SearchFromDiscQuery(query.AlbumDescriptor, needcovers, iCancellationToken);
            }

            int FoundItem = 0;

            foreach (Match<AlbumDescriptor> res in rescoll)
            {
               

                if (res != null)
                {
                    FoundItem++;
                    yield return res;

                    if (FoundItem == query.MaxResult)
                        break;
                }
          
            }
        }

        public void Dispose()
        {
        }
    }
}
