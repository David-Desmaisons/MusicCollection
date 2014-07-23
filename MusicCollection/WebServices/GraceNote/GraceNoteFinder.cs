using MusicCollection.DataExchange;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.WebServices.GraceNote
{
    [WebServicesInfoProvider(WebProvider.MusicBrainz)]
    internal class GraceNoteFinder : WebFinderAdapter, IInternerInformationProvider
    {
        private string _ApplicationID;
        private string _DeviceID;
        private IWebUserSettings _IWebUserSettings;

        public GraceNoteFinder(IWebUserSettings iwsm)
        {
            _IWebUserSettings = iwsm;
            _ApplicationID = iwsm.GraceNoteAplicationID;
            _DeviceID = iwsm.GraceNoteDeviceID;
        }

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellation)
        {
            if (string.IsNullOrEmpty(_ApplicationID))
                return Enumerable.Empty<Match<AlbumDescriptor>>();

            if (string.IsNullOrEmpty(_DeviceID))
            {
                _DeviceID = null;
                _IWebUserSettings.GraceNoteDeviceID = _DeviceID;
                _IWebUserSettings.Save();
            }

            return Enumerable.Empty<Match<AlbumDescriptor>>();
        }

        public void Dispose()
        {
        }
    }
}
