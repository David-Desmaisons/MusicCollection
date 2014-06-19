using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NHibernate;
using NHibernate.Type;

using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Buffer;
using MusicCollection.Infra;

namespace MusicCollection.Nhibernate.Blob
{

    internal class ExternalBlobManagerTransaction
    {
        static private readonly Type _BLOB = typeof(IBufferProvider);

        private IImportContext _Tx;
        private List<string> _NewFiles;
        private List<string> _OldFiles;

        

        static ExternalBlobManagerTransaction()
        {
        }

        private List<string> NewFiles
        {
            get { if (_NewFiles == null) _NewFiles = new List<string>(); return _NewFiles; }
        }

        private List<string> OldFiles
        {
            get { if (_OldFiles == null) _OldFiles = new List<string>(); return _OldFiles; }
        }

        internal ExternalBlobManagerTransaction(IImportContext tx)
        {
            _Tx = tx;
        }

        internal bool UpdateBlobToDiscIfNeeded(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            bool res = false;

            var result = from t in types.Select((n, index) => new { Type = n, Index = index }) where t.Type.ReturnedClass.Equals(_BLOB) select t.Index;

            foreach (var el in result)
            {

                object o = (currentState == null) ? null : currentState[el];
                object p = (previousState == null) ? null : previousState[el];
                bool needtocleanold = (p != null);

                if (object.ReferenceEquals(o, p))
                    continue;

                if (o != null)
                {
                    res = true;
                    IPersistentBufferProvider IBP = o as IPersistentBufferProvider;

                    if (IBP.IsPersistent)
                        needtocleanold = false;
                    else
                    {
                        if (!IBP.Save( _Tx.Folders.GetPrivatePath()))
                        {
                            //rollback if save as failed
                            currentState[el] = p;
                            needtocleanold = false;
                        }
                        else
                        {
                            IBP.IsPersistent = true;
                            File.SetAttributes(IBP.PersistedPath, File.GetAttributes(IBP.PersistedPath) | (FileAttributes.Hidden | FileAttributes.Archive |
                                                                                         FileAttributes.ReadOnly));

                            NewFiles.Add(IBP.PersistedPath);
                        }
                    }


                }

                if (needtocleanold)
                {
                    IPersistentBufferProvider old = previousState[el] as IPersistentBufferProvider;
                    if (old.IsPersistent)
                        OldFiles.Add(old.PersistedPath);
                }

            }

            return res;
        }



        private void DeleteList(List<string> L)
        {

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            List<string> tor = new List<string>(L);

            foreach (string Old in tor)
            {
                bool res = _Tx.Folders.RemoveFileAndEmptyDirectory(Old, false);
                if (res == false)
                    _Tx.AddFileTobeRemovedLater(Old,false);
                
                L.Remove(Old);

           }


        }

        internal void Commit()
        {
            if (_OldFiles != null)
                DeleteList(OldFiles);
        }

        internal void RollBack()
        {
            if (_NewFiles != null)
                DeleteList(NewFiles);
        }


    }
}
