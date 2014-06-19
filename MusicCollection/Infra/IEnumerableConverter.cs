using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollection.Infra
{
    public static class IEnumerableConverter
    {

        static public IEnumerable<IAlbum> ConvertToAlbums(this IEnumerable<IMusicObject> objs)
        {
            return objs.Select
                (o =>
                {
                    IAlbum al = o as IAlbum;
                    if (al == null)
                    {
                        ITrack tr = o as ITrack;
                        al = (tr == null) ? null : tr.Album;
                    }
                    return al;
                }
                ).Where(o=>o!=null).Distinct();
        }

        static public IEnumerable<ITrack> ConvertToTracks(this IEnumerable<IMusicObject> objs)
        {
            if (objs == null)
                return null;

            IEnumerable<ITrack> tv = objs as IEnumerable<ITrack>;

            if (tv != null)
                return tv;

            IEnumerable<IAlbum> als = objs as IEnumerable<IAlbum>;
            if (als == null)
                return null;

            return als.SelectMany(alb => alb.Tracks);
        }

        static public IEnumerable<Tout> ConvertMusicObject<Tout>(this IEnumerable<IMusicObject> objs)
            where Tout : class
        {
            return objs.Convert<IMusicObject, Tout>();
        }

        static public IEnumerable<Tout> Convert<Tin,Tout>(this IEnumerable<Tin> objs) where Tin:class where Tout :class
        {
            if (objs==null)
                return Enumerable.Empty<Tout>();

            return objs.Select(o => o as Tout).Where(o => o != null);
        }

    }
}
