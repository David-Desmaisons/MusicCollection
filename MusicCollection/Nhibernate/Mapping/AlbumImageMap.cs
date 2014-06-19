using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate;

using MusicCollection.Nhibernate.Session;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.Implementation;
using MusicCollection.Fundation;

namespace MusicCollection.Nhibernate.Mapping
{
    [MappingClass()]
    internal class AlbumImageMap:ClassMap<AlbumImage>
    {
        internal AlbumImageMap()
        {
            Not.LazyLoad();
            Table("Covers");
            Id(Reveal.Member<AlbumImage>("ID"));
            Map(Reveal.Member<AlbumImage>("_IBP")).Column("Location").Update();
            Map(Reveal.Member<AlbumImage>("_Description")).Column("Description");
            Map(Reveal.Member<AlbumImage>("_PT")).Column("Type");
            Map(x => x.Rank);
            References<Album>(Reveal.Member<AlbumImage>("Owner")).Column("DiscID");
            
         }
    }
}
