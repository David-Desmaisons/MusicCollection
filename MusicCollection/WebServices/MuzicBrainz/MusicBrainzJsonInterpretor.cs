using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.DataExchange;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Web;
using MusicCollection.DataExchange.Cue;
using MusicCollection.ToolBox.Buffer;
using System.Threading;

namespace MusicCollection.WebServices.MuzicBrainz
{
    public static class MusicBrainzJsonInterpretor
    {
        static public IEnumerable<AlbumDescriptor> FromMusicBrainzReleaseResults
            (dynamic iDynaminac, string iID, CancellationToken iCancelation, bool needcovers = false)
        {
            Func<dynamic, bool> Filter = m => (m.discs as List<dynamic>).Any(d => (d.id == iID));
            return PrivateFromMusicBrainzReleaseResults(iDynaminac, Filter, iCancelation, needcovers);
        }

        static public IEnumerable<AlbumDescriptor> PrivateFromMusicBrainzReleaseResults
            (dynamic iDynaminac, Func<dynamic, bool> Filter, CancellationToken iCancelation, bool needcovers = false)
        {
            List<dynamic> releases = null;

            try
            {
                releases = iDynaminac.releases;
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("MusicBrainzException {0}", e));
            }

            if (releases == null)
                yield break;

            foreach (dynamic rel in releases)
            {
                AlbumDescriptor res = FromMusicBrainzReleaseWithFilter(rel, Filter, needcovers, iCancelation);
                if (res != null)
                    yield return res;
            }
        }




        static public AlbumDescriptor FromMusicBrainzRelease(dynamic iDynaminac, bool needcovers, CancellationToken iCancelation)
        {
            Func<dynamic, bool> MediaSelector = _ => true;
            return FromMusicBrainzReleaseWithFilter(iDynaminac, MediaSelector, needcovers, iCancelation);
        }


        static private AlbumDescriptor FromMusicBrainzReleaseWithFilter
            (dynamic iDynaminac, Func<dynamic, bool> MediaSelector, bool needcovers, CancellationToken iCancelation)
        {
            try
            {
                dynamic covers = iDynaminac.cover_art_archive;
                bool Hascover = covers.count != 0;

                if ((needcovers) && (!Hascover))
                    return null;

                AlbumDescriptor res = (needcovers || !Hascover) ? new AlbumDescriptor() : new LoadingAlbumDescriptor();
                res.Name = iDynaminac.title;
                res.MusicBrainzID = iDynaminac.id;
                res.Year = ((string)(iDynaminac.date)).Substring(0, 4).TryParse();

                res.Artist = MusicCollection.Implementation.Artist.AuthorName(
                   (iDynaminac.artist_credit as List<dynamic>).Select<dynamic, string>(a => a.name).ToList());


                IEnumerable<dynamic> mymedium = (iDynaminac.media as List<dynamic>)
                                .Where(MediaSelector);

                res.RawTrackDescriptors = mymedium.SelectMany(m=>m.tracks as List<dynamic>,(m,t)=>new Tuple<dynamic,dynamic>(m,t) )
                    .Select(t => new TrackDescriptor(res, t.Item2.title, ((string)(t.Item2.number)).TryParse(), TimeSpan.FromMilliseconds((int)t.Item2.length), null, t.Item1.position)).ToList();


                res.TracksNumber = (uint)res.RawTrackDescriptors.Count;

                if (Hascover)
                {
                    if (needcovers)
                        res.RawImages = CoverartarchiveResult_From_musicbrainzid(res.MusicBrainzID, new CancellationToken());
                    else
                        (res as LoadingAlbumDescriptor).LoadAction = 
                            () => CoverartarchiveResult_From_musicbrainzid(res.MusicBrainzID, iCancelation);
                }

                return res;
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("MusicBrainzException {0}", e));
                return null;
            }
        }

        static public IEnumerable<AImage> FromCoverartarchiveResult(dynamic dob)
        {
            List<dynamic> images = null;

            try
            {
                images = dob.images;
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("MusicBrainzException {0}", e));
            }

            if (images == null)
                yield break;

            int i = 0;
            foreach (dynamic im in images)
            {
                yield return new AImage(BufferFactory.GetBufferProviderFromURI(new Uri(im.image)), i++);
            }
        }

        static public List<AImage> CoverartarchiveResult_From_musicbrainzid(string musicbrainzid, CancellationToken iCancelation)
        {
            MusicBrainzHttpCreator mbhc = MusicBrainzHttpCreator.ForCoverArtSearch().SetValue(musicbrainzid);
            dynamic readinfo = new HttpJsonInterpretor(mbhc.BuildRequest()).GetObjectResponse();
            List<AImage> res = ((IEnumerable<AImage>)FromCoverartarchiveResult(readinfo)).CancelableToList(iCancelation);
            return res;
        }
    }
}
