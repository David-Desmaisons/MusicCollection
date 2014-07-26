using MusicCollection.DataExchange;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.WebServices.GraceNote.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.WebServices.GraceNote
{
    [WebServicesInfoProvider(WebProvider.GraceNote)]
    internal class GraceNoteFinder : WebFinderAdapter, IInternerInformationProvider
    {
        private string _ApplicationID;
        private string _DeviceID;
        private IWebUserSettings _IWebUserSettings;
        private GraceNoteClient _GraceNoteClient;

        public GraceNoteFinder(IWebUserSettings iwsm)
        {
            _IWebUserSettings = iwsm;
            _ApplicationID = iwsm.GraceNoteAplicationID;
            _DeviceID = iwsm.GraceNoteDeviceID;

            _GraceNoteClient = new GraceNoteClient(_ApplicationID);
        }

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellation)
        {
            if (string.IsNullOrEmpty(_ApplicationID))
                return Enumerable.Empty<Match<AlbumDescriptor>>();

            if (string.IsNullOrEmpty(_DeviceID))
            {
                Responses rr = _GraceNoteClient.Post(GetRegisterQuery());

                if ((rr.Response == null) || (rr.Response.User == null) || (rr.Response.User.Token == null))
                {
                    Trace.WriteLine("Unable to register GraceNote on this device");
                    return Enumerable.Empty<Match<AlbumDescriptor>>();
                }

                _DeviceID = rr.Response.User.Token;
                _IWebUserSettings.GraceNoteDeviceID = _DeviceID;
                _IWebUserSettings.Save();
            }

            Queries q = BuildQueryFromRequest(query);
            Responses r = _GraceNoteClient.Post(q);

            if ((r.Response == null) || (r.Response.AlbumDto == null))
            {
                return Enumerable.Empty<Match<AlbumDescriptor>>();
            }

            IEnumerable<AlbumDto> Final = null;

            if (query.Type==QueryType.FromCD)
            {
                Queries nq = BuildQueryFromGraceNoteID(r.Response.AlbumDto[0].GracenoteId);
                Responses nr = _GraceNoteClient.Post(nq);
                if ((nr.Response == null) || (nr.Response.AlbumDto == null))
                {
                    return Enumerable.Empty<Match<AlbumDescriptor>>();
                }

                Final = nr.Response.AlbumDto;
            }
            else
            {
                Final = r.Response.AlbumDto;
            }

            return Final.Select(adto => new Match<AlbumDescriptor>(AlbumDescriptor.FromGraceNote(adto, query.NeedCoverArt, iCancellation), MatchPrecision.Suspition))
                    .Where(res => res.FindItem != null);
        }

        private Queries GetBasicQuery()
        {
            return new Queries()
            {
                Auth = new Auth(this._ApplicationID, this._DeviceID)
            };
        }

        private Queries GetRegisterQuery()
        {
            return new Queries() { Query = new Query("REGISTER", new Client(_ApplicationID)) };
        }

        private Queries BuildQueryFromGraceNoteID(string GraceNoteID)
        {
            Queries res = GetBasicQuery();

            Query smallquery = new Query()
            {
                Command = "ALBUM_FETCH",
                GracenoteId = GraceNoteID,
                Mode = "SINGLE_BEST_COVER"
            };

            smallquery.NeedFullCover();
            res.Query = smallquery;

            return res;
        }

        private Queries BuildQueryFromRequest(IWebQuery query)
        {
            Queries res = GetBasicQuery();

            Query smallquery = new Query();
            res.Query = smallquery;

            switch (query.Type)
            {
                case QueryType.FromAlbumInfo:
                    smallquery.Command = "ALBUM_SEARCH";
                    smallquery.AddSearch(Type: "ARTIST", Value: query.AlbumDescriptor.Artist)
                              .AddSearch(Type: "ALBUM_TITLE", Value: query.AlbumDescriptor.Name);
                    smallquery.RangeDto = new RangeDto(1, query.MaxResult);

                    if (query.NeedCoverArt)
                        smallquery.Mode = "SINGLE_BEST_COVER";

                    smallquery.NeedFullCover();
                    break;

                case QueryType.FromCD:
                    smallquery.Command = "ALBUM_TOC";
                    //smallquery.Mode = "SINGLE_BEST_COVER";
                    smallquery.Toc = new Toc(query.CDInfo.Tocs);
                    break;
            }

            return res;
        }

        public void Dispose()
        {
        }
    }
}
