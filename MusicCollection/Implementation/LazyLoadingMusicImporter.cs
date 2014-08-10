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

    internal interface IEventListener : IImportExportProgress
    {
        void OnFactorisableError<T>(string message) where T : ImportExportErrorEventListItemsArgs;
    }


    internal sealed class LazyLoadingMusicImporter : IMusicImporter
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

                return new LazyLoadingMusicImporter(_Session, ((iel) => FileNames.Select<string, IImporter>(fn =>
                    {
                        switch (FileServices.GetFileType(fn))
                        {
                            case MusicCollection.Infra.FileType.Mcc:
                                return new MccImporter(fn, ImportAllMetaData);

                            case MusicCollection.Infra.FileType.XML:
                                return new XMLImporter(fn, ImportAllMetaData);
                        }
                        return null;
                    }).Where(i => i != null)),

                    iDefaultMaturity);
            }

            IMusicImporter IMusicImporterFactory.GetRarImporter(string FileName, AlbumMaturity iDefaultMaturity)
            {
                return GetMultiRarImporter(FileName.SingleItemCollection<string>(), iDefaultMaturity);
            }

            IMusicImporter IMusicImporterFactory.GetCDImporter(AlbumMaturity iDefaultMaturity, bool iOpenCDDoorOnComplete)
            {
                return new LazyLoadingMusicImporter(_Session, (iel) => FileInternalToolBox.GetAvailableCDDriver().Select(d => new CDImporter(_Session.MusicConverter, d, iOpenCDDoorOnComplete)), iDefaultMaturity);
            }

            public IMusicImporter GetMultiRarImporter(IEnumerable<string> FileName, AlbumMaturity iDefaultMaturity)
            {
                return new LazyLoadingMusicImporter(_Session, (iel) => CollectorFactory.CollectorRar(_Session, FileName).Select(col => col.Importer), iDefaultMaturity);
            }

            IMusicImporter IMusicImporterFactory.GetFileService(IInternalMusicSession iconv, string dir, AlbumMaturity iDefaultMaturity)
            {
                DirectoryInfo DI = new DirectoryInfo(dir);
                if (!DI.Exists)
                    throw new InvalidDataException(string.Format("Need a valid path {0}", dir));

                return new LazyLoadingMusicImporter(_Session, ((iel) => GetFrom(iconv, DI, iel)), iDefaultMaturity);
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
        private IInternalMusicSession _IInternalMusicSession;

        private LazyLoadingMusicImporter(IInternalMusicSession Session, Func<IEventListener, IEnumerable<IImporter>> Con, AlbumMaturity iDefaultMaturity)
        {
            _IInternalMusicSession = Session;
            _Transaction = Session.GetNewSessionContext(iDefaultMaturity);
            _Importers = null;
            _Const = Con;
        }

        private LazyLoadingMusicImporter(IInternalMusicSession Session, Func<IImporter> Con, AlbumMaturity iDefaultMaturity)
            : this(Session, ((iel) => Con().SingleItemCollection<IImporter>()), iDefaultMaturity)
        {
        }

        static internal IMusicImporterFactory GetFactory(IInternalMusicSession Session)
        {
            return new MusicImporterFactory(Session);
        }

        private bool IsCancelled(CancellationToken iCancellationToken)
        {
            return ((_Transaction.IsEnded) || (iCancellationToken.IsCancellationRequested));
        }

        private void SecureImport(IImportExportProgress iIImportProgress, CancellationToken? iCancelationToken)
        {
            if (_Done)
                return;

            _Transaction.Error += ((o, e) => iIImportProgress.SafeReport(e));
            _Transaction.Progress += ((o, e) => iIImportProgress.SafeReport(e));

            CancellationToken ct = (iCancelationToken != null) ? iCancelationToken.Value : CancellationToken.None;
            var listener = new IEventListenerAdaptor(iIImportProgress, _Transaction);

            bool Cancelled = false;

            using (_Transaction.SessionLock())
            {
                if (IsCancelled(ct))
                    return;

                try
                {
                    _Importers = _Const(listener);
                    iIImportProgress.SafeReport(new BeginImport());

                    bool donesemething = false;

                    foreach (IImporter Importer in _Importers)
                    {
                        if (Cancelled = IsCancelled(ct))
                            break;

                        IImporter CurrentImporter = Importer;
                        while ((CurrentImporter != null) && (!(Cancelled=IsCancelled(ct))) )
                        {
                            donesemething = true;
                            CurrentImporter.Context = _Transaction;
                            CurrentImporter = CurrentImporter.Action(listener, ct);
                        }
                    }

                    _Transaction.FireFactorizedEvents();

                    if (!donesemething)
                        iIImportProgress.SafeReport(new NullMusicImportErrorEventArgs());
                    else if (!Cancelled)
                        _Transaction.Commit();

                    _Done = true;

                    iIImportProgress.SafeReport(new EndImport());
                }
                catch (ImportExportException iee)
                {
                    iIImportProgress.SafeReport(iee.Error);
                    _Transaction.FireFactorizedEvents();
                    iIImportProgress.SafeReport(new EndImport());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    iIImportProgress.SafeReport(new UnknowError());
                    _Transaction.FireFactorizedEvents();
                    iIImportProgress.SafeReport(new EndImport());
                }
            }
        }

        void IMusicImporter.Load(IImportExportProgress iIImportProgress)
        {
            SecureImport(iIImportProgress, null);
        }

        private Task UniversalLoadAsync(IImportExportProgress iIImportProgress, CancellationToken? iCancelationToken, ThreadProperties tp)
        {
            return Task.Factory.StartNew(
               () =>
               {
                   using (tp.GetChanger())
                   {
                       SecureImport(iIImportProgress, iCancelationToken);
                   }
               },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        Task IMusicImporter.LoadAsync(IImportExportProgress iIImportProgress, CancellationToken? iCancelationToken)
        {
            return UniversalLoadAsync(iIImportProgress, iCancelationToken,null);
        }

        Task IMusicImporter.LoadAsync(ThreadProperties tp)
        {
            return UniversalLoadAsync(null, CancellationToken.None, tp);
        }

    }
}

