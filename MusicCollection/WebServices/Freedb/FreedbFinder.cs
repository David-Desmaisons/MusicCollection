using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.ToolBox.Web;
using MusicCollection.Implementation;
using System.Threading;

namespace MusicCollection.WebServices.Freedb
{
    [WebServicesInfoProvider(WebProvider.Freedb)]
    internal class FreedbFinder : WebFinderAdapter, IInternerInformationProvider, IInternetServiceListener
    {
        private IWebUserSettings _WSM;
        public FreedbFinder(IWebUserSettings iwsm)
        {
            _WSM = iwsm;
        }

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellationToken)
        {
            if ((query == null) || (query.Type != QueryType.FromCD))
                yield break;

            //CDInfoQuery cdiq = query as CDInfoQuery;
            //if (cdiq == null)
            //    yield break;

            ICDInfoHandler CDUnit = query.CDInfo;
            if (CDUnit == null)
                yield break;        
            
            string ID = CDUnit.IDs.CDDBQueryString;
            if (ID == null)
                yield break;

            FreedbHelper FDH = new FreedbHelper("test", "abc.company", "FreedbDemo", "1.0");
            if (_WSM != null)
                FDH.MainSiteAdress = _WSM.FreedbServer;

            List<QueryResult> queryResults = null;

            try
            {
                queryResults = FDH.Query(ID);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Unable to retrieve CD Information. " + ex.Message);
                OnWebExeption(ex);
            }

            if (queryResults == null)
                yield break;

            

            foreach (QueryResult queryResult in queryResults)
            {
                AlbumDescriptor res = FDH.ConvertToAlbumDescriptor(queryResult, this);
                if (res!=null)
                    yield return new Match<AlbumDescriptor>(res, MatchPrecision.Suspition);
            }

            yield break;
        }


        public void Dispose()
        {
        }
    }
}
