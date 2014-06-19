using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Nhibernate.Session;

namespace MusicCollection.Utilies
{
    internal class OrphanArtistDBCleaner
    {
     //  private IImportContext _Context;
        private ISession _DBSession;

        internal OrphanArtistDBCleaner(ISession context)
        {
            _DBSession = context;
        }

        //internal OrphanArtistDBCleaner(IImportContext context)
        //{
        //    _Context = context;
        //}

        internal bool Clean()
        {
            ISQLQuery sql = _DBSession.CreateSQLQuery("delete from Artists where ID not in (select ArtistID from AlbumToArtist)");
            if (sql == null)
                return false;

            sql.ExecuteUpdate();
            return true;

            //return Clean(_DBSession);
            //.ExecuteInANewDBTransaction();
        }

        internal int GetArtistsCount()
        {
            var res = GetArtists();
            return (res != null) ? res.Count : 0;
        }

        internal IList<IArtist> GetArtists()
        {
            //IList<IArtist> res = ComputeArtists(_DBSession);
            ////Func<IDBSession, bool> MyFunc = (db) => { res = ComputeArtists(db); return true; };
            ////_Context.ExecuteInANewDBTransaction(MyFunc);
            //return res;

            ISQLQuery sql = _DBSession.CreateSQLQuery("select * from Artists where ID not in (select ArtistID from AlbumToArtist)")
               .AddEntity("item", typeof(Artist));
            if (sql == null)
                return null;

            return sql.List<Artist>().ToList<IArtist>();
        }


        //static private IList<IArtist> ComputeArtists(ISession Context)
        //{
        //    //ISQLQuery sql = Context.NHSession.CreateSQLQuery("select * from Artists where ID in (select ID from (select a.ID as ID, ata.ID as AlbumID from Artists as a left outer join AlbumToArtist as ata on a.ID = ata.ArtistID where AlbumID is null))")
        //    ISQLQuery sql = Context.CreateSQLQuery("select * from Artists where ID not in (select ArtistID from AlbumToArtist)")
        //        .AddEntity("item", typeof(Artist));
        //    if (sql == null)
        //        return null;

        //    return sql.List<Artist>().ToList<IArtist>();
        //}

        //static private bool Clean(ISession Context)
        //{
            
        //}
    }
}
