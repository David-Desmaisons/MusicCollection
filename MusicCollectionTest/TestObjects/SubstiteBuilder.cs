using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using MusicCollection.Fundation;
using FluentAssertions;
using FluentAssertions.Events;
using System.Collections.Specialized;
using System.Linq.Expressions;
using MusicCollection.Implementation;

namespace MusicCollectionTest.TestObjects
{
    public static class SubstiteBuilder
    {
        //public static IAlbum ForAlbum(int iTrackNumber=0)
        //{
        //    CompleteObservableCollectionImpl<ITrack> coci = new CompleteObservableCollectionImpl<ITrack>();
        //    IAlbum res = Substitute.For<IAlbum>();
        //    res.IsAlive.Returns(true);
        //    res.Tracks.Returns(coci);

        //    for (int i = 0; i < iTrackNumber; i++)
        //    {
        //        IInternalTrack subres = ForTrack(res);
        //    }
        //    return res;
        //}

        internal static IInternalTrack ForTrack(IAlbum Father)
        {
            IInternalTrack res = Substitute.For<IInternalTrack>();
            Father.Tracks.Add(res);
            res.Album.Returns(Father);
            res.IsAlive.Returns(true);
            return res;
        }

        public static IAlbum ForAlbum(int iTrackNumber = 0, string iName = null, 
            string iArtist = null, string iGenre = null)
        {
            CompleteObservableCollectionImpl<ITrack> coci = new CompleteObservableCollectionImpl<ITrack>();
            IAlbum res = Substitute.For<IAlbum>();
            res.IsAlive.Returns(true);
            res.Tracks.Returns(coci);
            res.Name.Returns(iName);

            for (int i = 0; i < iTrackNumber; i++)
            {
                IInternalTrack subres = ForTrack(res);
            }
            return res;
        }

      
    }
}
