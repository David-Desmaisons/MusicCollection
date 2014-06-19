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
    internal class GenreMap : ClassMap<Genre>
    {
        internal GenreMap()
        {
            Not.LazyLoad();
            Table("Genres");
            Id(Reveal.Member<Genre>("ID"));
            Map(x => x.Name).Column("Name").Not.Nullable();
            References<Genre>(Reveal.Member<Genre>("_Father")).Column("FatherID");


            HasMany<Genre>(Reveal.Member<Genre>("_Genres")).KeyColumn("FatherID").Not.LazyLoad().Cascade.All().Inverse();

            //NhibernateSession.AddMaturation<Genre>((a, s) => s.Session.Genres.Register(a));
        }
    }
}
