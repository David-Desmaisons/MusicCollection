using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate;

using MusicCollection.Nhibernate.Session;
using MusicCollection.Implementation;
using MusicCollection.Fundation;

namespace MusicCollection.Nhibernate.Mapping
{
    [MappingClass()]
    internal class ArtistMap : ClassMap<Artist>
    {
        internal ArtistMap()
        {
            Not.LazyLoad();
            Table("Artists");
            Id(Reveal.Member<Artist>("ID"));
            Map(x => x.Name).Column("Name").Not.Nullable().Unique();

            HasManyToMany<Album>(Reveal.Member<Artist>("_Albums")).Table("AlbumToArtist").ParentKeyColumn("ArtistID").ChildKeyColumn("DiscId").Inverse().Not.LazyLoad();

        }
    }
}

