using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Nhibernate.Session;
using MusicCollection.Nhibernate.Mapping;

namespace MusicCollection.Utilies
{
    internal enum DBOperation { Ignore, Update, Delete };

    internal abstract class DataBaseUpdater<T> where T : class
    {
        private Func<T, DBOperation> _OnTrack;

        internal DataBaseUpdater(Func<T, DBOperation> OnTrack)
        {
            _OnTrack = OnTrack;
        }

        private List<Tuple<DBOperation, T>> _ToDo = new List<Tuple<DBOperation, T>>();

        protected abstract IEnumerable<T> GetAllElements(IMusicSession session);

        internal int UpdateDataBase()
        {
            if (_OnTrack == null)
                throw new Exception("Must set a OnTrack");

            using (IMusicSession session = MusicSession.GetSession(null))
            {
                session.GetDBImporter().Load();
                //session.GetDBImporter().Load(true);

                // session.GetImporterFactory().GetDBImporter().Load(true);

                foreach (T tr in GetAllElements(session))
                {
                    DBOperation todo = _OnTrack(tr);
                    if (todo != DBOperation.Ignore)
                        _ToDo.Add(new Tuple<DBOperation, T>(todo, tr));
                }

                using (IDBSession session2 = DBSession.CreateorGetCurrentSession((session as IInternalMusicSession).GetNewSessionContext()))
                {

                    GenericDAO<T, int> td = new GenericDAO<T, int>(session2.NHSession);

                    using (ITransaction tran = session2.NHSession.BeginTransaction())
                    {

                        foreach (Tuple<DBOperation, T> tr in _ToDo)
                        {
                            switch (tr.Item1)
                            {
                                case DBOperation.Delete:
                                    td.MakeTransient(tr.Item2);
                                    break;

                                case DBOperation.Update:
                                    td.SaveOrUpdate(tr.Item2);
                                    break;
                            }
                        }

                        tran.Commit();
                    }
                }


            }

            return _ToDo.Count;
        }
    }
}
