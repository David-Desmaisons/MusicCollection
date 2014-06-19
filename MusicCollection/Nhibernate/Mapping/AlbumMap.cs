using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate;

using MusicCollection.Nhibernate.Session;
using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;

namespace MusicCollection.Nhibernate.Mapping
{
    [MappingClass()]
    internal class AlbumMap : ClassMap<Album>
    {
        internal AlbumMap()
        {
            Not.LazyLoad();
            Table("Discs");
            Id(Reveal.Member<Album>("ID"));
            Map(Reveal.Member<Album>("InitName")).Column("Name").Not.Nullable();
            Map(x => x.Interface.Year).Column("Year");
            Map(Reveal.Member<Album>("_Maturity")).Column("Maturity");
            Map(Reveal.Member<Album>("_DateAdded")).Column("DateAdded");
            Map(Reveal.Member<Album>("ImageCacheBuffer")).Column("FrontCoverLocation").Update();
            References<Genre>(Reveal.Member<Album>("_Genre")).Column("GenreID").Cascade.SaveUpdate();

            Map(x => x.Asin).Column("Asin");
            Map(x => x.MusicBrainzID).Column("MusicBrainzID");
            Map(x => x.CDDB).Column("CDDB");
            Map(x => x.MusicBrainzHash).Column("MusicBrainzHash");
  
            //Component(x => x.CDIDs, m =>
            //{
            //    m.Map(x => x.Asin).Column("Asin");
            //    m.Map(x => x.MusicBrainzID).Column("MusicBrainzID");
            //    m.Map(Reveal.Member<DiscIDs>("CDDB_Hash")).Column("CDDB");
            //    m.Map(Reveal.Member<DiscIDs>("MusicBrainz_Hash")).Column("MusicBrainzHash"); 
            //}
            //);

            //HasMany<Track>(Reveal.Member<Album>("_Tracks"))
            //     .KeyColumn("DiscID").Cascade.All()
            //    .Not.LazyLoad().Inverse();

            HasMany<Track>(Reveal.Member<Album>("_Tracks")).Fetch.Join()
               .KeyColumn("DiscID").Fetch.Join().Cascade.All()
              .Inverse();

            HasMany<AlbumImage>(Reveal.Member<Album>("PersistentImages")).Table("Covers").KeyColumn("DiscID").Cascade.AllDeleteOrphan().AsList(i => i.Column("Rank")).Not.LazyLoad().Inverse();

            HasManyToMany<Artist>(Reveal.Member<Album>("_Artists")).Table("AlbumToArtist").ParentKeyColumn("DiscId").ChildKeyColumn("ArtistID").AsList(i => i.Column("Rank")).Not.LazyLoad().Cascade.SaveUpdate();


            //NhibernateSession.AddMaturation<Album>((a, s) => { s.Register(a); a.AfterLoad(); });
        }
    }
}