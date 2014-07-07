using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;

using MusicCollection.FileImporter;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.FileConverter;
using MusicCollection.Itunes;
using MusicCollection.Infra;
using System.Threading.Tasks;


namespace MusicCollection.Implementation
{

    internal interface IMusicImporterFactory
    {
        IMusicImporter GetITunesService(bool ImportBrokenTunes, string Directory, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetDBImporter();

        IMusicImporter GetFileService(IInternalMusicSession iconv, string Directory, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetRarImporter(string FileName, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetXMLImporter(IEnumerable<string> FileName, bool ImportAllMetaData, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetMultiRarImporter(IEnumerable<string> FileName, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetCDImporter(AlbumMaturity iDefaultMaturity, bool iOpenCDDoorOnComplete);
    }

    internal interface IEventListener
    {
        void OnError(ImportExportErrorEventArgs Error);

        void OnProgress(ProgessEventArgs Where);

        void OnFactorisableError<T>(IEnumerable<string> message) where T : ImportExportErrorEventListItemsArgs;

        void OnFactorisableError<T>(string message) where T : ImportExportErrorEventListItemsArgs;

    }


    internal sealed class LazyLoadingMusicImporter : UIThreadSafeImportEventAdapter, IMusicImporter, IEventListener
    {

        private class MusicImporterFactory : IMusicImporterFactory
        {
            private IInternalMusicSession _Session;

            internal MusicImporterFactory(IInternalMusicSession Session)
            {
                _Session = Session;
            }

            IMusicImporter IMusicImporterFactory.GetITunesService(bool ImportBrokenTrack, string Directory, AlbumMaturity iDefaultMaturity)
            {
                return new LazyLoadingMusicImporter(_Session, (() => new IiTunesImporter(ImportBrokenTrack, Directory)), iDefaultMaturity);
            }

            IMusicImporter IMusicImporterFactory.GetDBImporter()
            {
                return new LazyLoadingMusicImporter(_Session, (() => new DataBaseImporter(_Session.DBCleanOnOpen)), AlbumMaturity.Discover);
            }

            IMusicImporter IMusicImporterFactory.GetXMLImporter(IEnumerable<string> FileNames, bool ImportAllMetaData, AlbumMaturity iDefaultMaturity)
            {
 
                return new LazyLoadingMusicImporter(_Session, ((iel) => FileNames.Select<string,IImporter>( fn =>
                    {
                        switch( FileServices.GetFileType(fn) )
                        {
                            case MusicCollection.Infra.FileType.Mcc : 
                                return  new MccImporter(fn, ImportAllMetaData);

                            case MusicCollection.Infra.FileType.XML:
                                return  new XMLImporter(fn, ImportAllMetaData);
                        }
                        return null;
                    }).Where(i=>i!=null)),
                   
                    iDefaultMaturity);            
            }

            IMusicImporter IMusicImporterFactory.GetRarImporter(string FileName, AlbumMaturity iDefaultMaturity)
            {
                return GetMultiRarImporter(FileName.SingleItemCollection<string>(), iDefaultMaturity);
            }

            IMusicImporter IMusicImporterFactory.GetCDImporter(AlbumMaturity iDefaultMaturity, bool iOpenCDDoorOnComplete)
            {
                return new LazyLoadingMusicImporter(_Session, (iel) => FileInternalToolBox.GetAvailableCDDriver().Select(d=> new CDImporter(_Session.MusicConverter, d, iOpenCDDoorOnComplete)), iDefaultMaturity);
            }

            public IMusicImporter GetMultiRarImporter(IEnumerable<string> FileName, AlbumMaturity iDefaultMaturity)
            {
                return new LazyLoadingMusicImporter(_Session, (iel) => CollectorFactory.CollectorRar(_Session,FileName).Select(col => col.Importer), iDefaultMaturity);
            }

            IMusicImporter IMusicImporterFactory.GetFileService(IInternalMusicSession iconv, string dir, AlbumMaturity iDefaultMaturity)
            {
                DirectoryInfo DI = new DirectoryInfo(dir);
                if (!DI.Exists)
                    throw new InvalidDataException(string.Format("Need a valid path {0}", dir));

                return new LazyLoadingMusicImporter(_Session, ((iel) => GetFrom(iconv,DI, iel)), iDefaultMaturity);
            }


            private IEnumerable<IImporter> GetFrom(IInternalMusicSession iconv, DirectoryInfo DI, IEventListener iel)
            {
                try
                {
                    return new FolderInspector(iconv, DI, iel).Collectors;
                }
                catch (PathTooLongException ptle)
                {
                    Trace.WriteLine(string.Format("Error during import: {0}", ptle));
                    throw ImportExportException.FromError(new PathTooLong(string.Empty)); ;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(string.Format("Error during import: {0}", e));
                    throw;
                }
            }
        }


        private bool _Done = false;
        private ImportTransaction _Transaction;
        private IEnumerable<IImporter> _Importers;
        private Func<IEventListener, IEnumerable<IImporter>> _Const;

        private LazyLoadingMusicImporter(IInternalMusicSession Session, Func<IEventListener, IEnumerable<IImporter>> Con, AlbumMaturity iDefaultMaturity)
        {
            _Transaction = Session.GetNewSessionContext(iDefaultMaturity);
            _Transaction.Error += ((o, e) => OnError(e));
            _Transaction.Progress += ((o, e) => OnProgress(e));
            _Importers = null;
            _Const = Con;
        }

        private LazyLoadingMusicImporter(IInternalMusicSession Session, Func<IImporter> Con, AlbumMaturity iDefaultMaturity)
            : this(Session, ((iel) => Con().SingleItemCollection<IImporter>()), iDefaultMaturity)
        {
        }

        void IEventListener.OnFactorisableError<T>(IEnumerable<string> message)
        {
            _Transaction.OnFactorisableError<T>(message);
        }

        void IEventListener.OnFactorisableError<T>(string message)
        {
            _Transaction.OnFactorisableError<T>(message);
        }


        void IEventListener.OnError(ImportExportErrorEventArgs Error)
        {
            OnError(Error);
        }

        void IEventListener.OnProgress(ProgessEventArgs Where)
        {
            OnProgress(Where);
        }

        static internal IMusicImporterFactory GetFactory(IInternalMusicSession Session)
        {
            return new MusicImporterFactory(Session);
        }


        private void SecureImport()
        {
            if (_Done)
                return;

            using (_Transaction.SessionLock())
            {
                try
                {
                    if (_Importers == null)
                    {
                        OnProgress(new BeginImport());
                        _Importers = _Const(this);
                        _Const = null;
                    }

                    bool donesemething = false;

                    foreach (IImporter Importer in _Importers)
                    {
                        IImporter CurrentImporter = Importer;
                        while (CurrentImporter != null)
                        {
                            donesemething = true;
                            CurrentImporter.Context = _Transaction;
                            CurrentImporter = CurrentImporter.Action(this);
                        }
                    }

                    _Transaction.FireFactorizedEvents();

                    if (!donesemething)
                        OnError(new NullMusicImportErrorEventArgs());
                    else
                        _Transaction.Commit();

                    _Done = true;

                    OnProgress(new EndImport());
                }
                catch (ImportExportException iee)
                {
                    OnError(iee.Error);
                    _Transaction.FireFactorizedEvents();
                    OnProgress(new EndImport());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    OnError(new UnknowError());
                    _Transaction.FireFactorizedEvents();
                    OnProgress(new EndImport());
                }
            }

        }

        void IMusicImporter.Load()
        {
            SecureImport();
        }

        Task IMusicImporter.LoadAsync(ThreadProperties tp)
        {
            return Task.Factory.StartNew( ()=>
                {
                    using (tp.GetChanger())
                    {
                        SecureImport();
                    }
                }, 
                TaskCreationOptions.LongRunning);
        }

    }
}

