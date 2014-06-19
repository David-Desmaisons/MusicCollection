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
    internal class TrackMap: ClassMap<Track>
    {
        internal TrackMap()
        {
            Not.LazyLoad();
            Table("Tracks");
            Id(Reveal.Member<Track>("ID"));

            Map(x => x.Interface.Name);
            Map(Reveal.Member<Track>("_Duration")).Column("Duration").Not.Nullable();
            Map(x => x.TrackNumber);
            Map(Reveal.Member<Track>("_Rating")).Column("Rating");
            Map(Reveal.Member<Track>("_PlayCount")).Column("PlayCount");
            Map(Reveal.Member<Track>("_DateAdded")).Column("DateAdded");
            Map(Reveal.Member<Track>("_LastPlayed")).Column("LastPlay");
            Map(Reveal.Member<Track>("_MD5")).Column("Hashkey");
            //Map(Reveal.Member<Track>("_ISRC")).Column("ISRC");
            Map(Reveal.Member<Track>("_Skipped")).Column("SkipCount");
            Map(Reveal.Member<Track>("_DiscNumber")).Column("DiscNumber");
             
            Map(x => x.Path).Unique();

            References<Album>(Reveal.Member<Track>("_Album"))
                //.ForeignKey("DiscID")
                .Column("DiscID");

            //NhibernateSession.AddMaturation<Track>((t, s) => s.Register(t));

        }
    }
}
