using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using SevenZip;

using MusicCollection.SettingsManagement;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using System.Threading;


namespace MusicCollection.FileImporter
{
    internal class MccDecompactor : IMccDescompactor
    {
        private SevenZipExtractor _Sex;
        private bool _Success = true;
        private IImportContext _ICC;

        internal MccDecompactor(SevenZipExtractor sex, IImportContext iic)
        {
            _Sex = sex;
            _ICC = iic;
        }

        private Dictionary<string, string> _Rerooter;
        public IDictionary<string, string> Rerooter
        {
            get { return _Rerooter; }
        }

        private void AddAssociationIfNeeded(string OldName, string NewName)
        {
            string ofn = Path.GetFileName(OldName);
            string nfn = Path.GetFileName(NewName);

            if (ofn == nfn)
                return;

            if (_Rerooter == null)
                _Rerooter = new Dictionary<string, string>();

            _Rerooter.Add(@".\" + OldName, @".\" + Path.Combine(Path.GetDirectoryName(OldName), nfn));
        }

        private void ExtractCallBack(ExtractFileCallbackArgs efc, IEventListener iel, CancellationToken iCancellationToken)
        {
            switch (efc.Reason)
            {
                case ExtractFileCallbackReason.Start:

                    string nd = null;
                    bool root = false;

                    if (efc.ArchiveFileInfo.FileName == MusicExporter.XMLName)
                    {
                        nd = _ICC.Folders.Temp;
                        root = true;
                    }
                    else
                    {
                        nd = _ICC.Folders.File;
                    }

                    efc.ExtractToFile = FileInternalToolBox.CreateNewAvailableName(Path.Combine(nd, efc.ArchiveFileInfo.FileName));
                    if (root)
                        _RX = efc.ExtractToFile;
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(efc.ExtractToFile));
                    }

                    if ((!efc.ArchiveFileInfo.IsDirectory) && !(root))
                    {
                        iel.Report(new ExtractProgessEventArgs(string.Format("{0} from {1}", Path.GetFileName(efc.ArchiveFileInfo.SafePath()), _Sex.FileName)));
                        AddAssociationIfNeeded(efc.ArchiveFileInfo.FileName, efc.ExtractToFile);
                    }

                    DescompactedFiles.Add(efc.ExtractToFile);
                    break;

                case ExtractFileCallbackReason.Failure:
                    iel.OnFactorisableError<UnableToExtractFileFromRar>(string.Format("{0} from {1} : {2}", Path.GetFileName(efc.ArchiveFileInfo.FileName), _Sex.FileName, efc.Exception));
                    efc.Exception = null;
                    efc.ExtractToFile = null;
                    _Success = false;
                    break;
            }

            if (iCancellationToken.IsCancellationRequested)
            {
                _Success = false;
                efc.CancelExtraction = true;
            }
        }


        public bool Extract(IEventListener Listener, CancellationToken iCancellationToken)
        {
            if (_Sex.IsSolid == true)
                return false;

            _Sex.ExtractFiles(n => ExtractCallBack(n, Listener, iCancellationToken));

            return _Success;
        }


        private List<string> _DescompactedFiles = new List<string>();
        public List<string> DescompactedFiles
        {
            get { return _DescompactedFiles; }
        }

        private string _RX;
        public string RootXML
        {
            get { return _RX; }
        }

        public void Dispose()
        {
            _Sex.Dispose();
        }
    }


    internal abstract class ZipabstractDescompactor : IRarDescompactor
    {
        protected const int MAX_PATH = 260;

        protected RarImporterHelper _RarContext;
        protected bool _IsValid = false;
        protected string _Root;
        protected RarManagerImpl _Father;

        protected SevenZipExtractor _Sex;
        protected bool _ContainsMusic = false;
        protected bool _Deploy = true;
        private bool _ContainsCompressedMusic;


        protected ZipabstractDescompactor(RarManagerImpl Father, SevenZipExtractor sex)
        {
            _RarContext = new RarImporterHelper(sex);
            _Father = Father;
            _Sex = sex;

            _ContainsCompressedMusic = (from p in _Sex.ArchiveFileData
                                        where (!p.IsDirectory) && (FileServices.GetFileType(p.SafePath()) == FileType.MusicToConv)
                                        select p).Any();

            _ContainsMusic = _ContainsCompressedMusic ? true : (from p in _Sex.ArchiveFileData
                                                                where (!p.IsDirectory) && (FileServices.GetFileType(p.SafePath()) == FileType.Music)
                                                                select p).Any();

        }

        private string Root
        {
            get { return _Root; }
        }

        private bool ContainsCompressedMusic
        {
            get { return _ContainsCompressedMusic; }
        }

        private bool ContainsMusic
        {
            get { return _ContainsMusic; }
        }

        public IImportHelper RarContext
        {
            get { return _RarContext; }
        }

        private bool CanBeImported
        {
            get { return _IsValid; }
        }

        public List<String> ArchiveNames
        {
            get { return (from n in _Sex.VolumeFileNames select n).Distinct().ToList(); }
        }

        public IImportHelper Helper
        {
            get { return _RarContext; }
        }

        protected abstract bool PrivateDecompactor(IEventListener Listener, CancellationToken iCancellationToken);


        public bool Extract(IEventListener Listener, CancellationToken iCancellationToken)
        {
            if (!CanBeImported)
            {
                Listener.Report(new FileTooLongArgs((_RarContext as IImportHelper).DisplayName));
                return false;
            }

            if (!ContainsMusic)
            {
                Listener.Report(new NoMusicImportErrorEventArgs((_RarContext as IImportHelper).DisplayName));
                return false;
            }

            //check available space
            SpaceChecker sc = new SpaceChecker(Root, _Sex.UnpackedSize * (ContainsCompressedMusic ? 2 : 1));

            if (!sc.OK)
            {
                Listener.Report(new NotEnougthSpace(sc.ToString()));
                return false;
            }

            return PrivateDecompactor(Listener, iCancellationToken);
        }

        private List<string> _Files = null;
        public List<string> DescompactedFiles
        {
            set { _Files = value; }
        }

        protected List<string> Files
        {
            get { return _Files; }
        }

        public void Dispose()
        {
            _Sex.Dispose();
        }

        internal static IRarDescompactor GetDescompactor(bool Local, RarManagerImpl Father, SevenZipExtractor sex)
        {
            if (sex.IsSolid)
                return new ZipSolidDescompactor(Father, sex, Local);


            if (Local)
                return new ZipLocalDescompactor(Father, sex);

            return new ZipDescompactor(Father, sex);
        }

        internal abstract class ZipNotSolidabstractDescompactor : ZipabstractDescompactor
        {
            private bool _DOK = true;

            protected ZipNotSolidabstractDescompactor(RarManagerImpl Father, SevenZipExtractor sex)
                : base(Father, sex)
            {
            }

            protected override bool PrivateDecompactor(IEventListener Listener, CancellationToken iCancellationToken)
            {
                _Sex.ExtractFiles(n => ExtractCallBack(n, Listener, iCancellationToken));
                return _DOK;
            }

            private string GetFileName(ArchiveFileInfo afi)
            {
                if ((!_Deploy) && (afi.IsDirectory))
                    return null;

                string path = afi.SafePath();

                string TRP = TargetRelativePath(path, _Deploy);

                if (TRP == null)
                    return null;

                if (afi.IsDirectory)
                {
                    if (_Deploy)
                        Directory.CreateDirectory(Path.Combine(_Root, TRP));

                    return null;
                }

                string resultDir = Path.Combine(_Root, Path.GetDirectoryName(TRP) ?? string.Empty );

                if (_Deploy)
                {
                    Directory.CreateDirectory(resultDir);
                }

                return FileInternalToolBox.CreateNewAvailableName(resultDir, Path.GetFileNameWithoutExtension(TRP), Path.GetExtension(TRP));
            }

            abstract protected string TargetRelativePath(string FileNameAsInRar, bool Deploy);

            private void ExtractCallBack(ExtractFileCallbackArgs efc, IEventListener iel, CancellationToken iCancellationToken)
            {
                switch (efc.Reason)
                {
                    case ExtractFileCallbackReason.Start:

                        string Dest = GetFileName(efc.ArchiveFileInfo);

                        if (Dest == null)
                        {
                            efc.ExtractToFile = null;
                            break;
                        }

                        efc.ExtractToFile = Dest;

                        if (!efc.ArchiveFileInfo.IsDirectory)
                            iel.Report(new ExtractProgessEventArgs(string.Format("{0} from {1}", Path.GetFileName(efc.ArchiveFileInfo.SafePath()), RarContext.DisplayName)));

                        Files.Add(Dest);
                        break;

                    case ExtractFileCallbackReason.Failure:
                        iel.OnFactorisableError<UnableToExtractFileFromRar>(string.Format("{0} from {1} : {2}", Path.GetFileName(efc.ArchiveFileInfo.SafePath()), RarContext.DisplayName, efc.Exception));
                        efc.Exception = null;
                        efc.ExtractToFile = null;
                        _DOK = false;
                        break;
                }

                if (iCancellationToken.IsCancellationRequested)
                    efc.CancelExtraction = true;
            }
        }

        private class ZipSolidDescompactor : ZipabstractDescompactor, IRarDescompactor
        {
            internal ZipSolidDescompactor(RarManagerImpl Father, SevenZipExtractor sex, bool Local)
                : base(Father, sex)
            {
                _Root = Local ? Path.Combine(Path.GetDirectoryName(sex.FileName), FileInternalToolBox.RawRarName(sex.FileName)) : _Father.ComputeName(_RarContext);
                int rootl = _Root.Length;
                _IsValid = true;
                _Deploy = (rootl + _RarContext.MaxLengthBasic) < MAX_PATH && (_RarContext.RootContainsSubFolder());
            }

            protected override bool PrivateDecompactor(IEventListener Listener,CancellationToken ict)
            {
                bool res = false;
                try
                {
                    if (!_Deploy)
                        _Sex.PreserveDirectoryStructure = false;

                    _Sex.ExtractArchive(_Root);
 
                    Files.AddCollection( _Sex.ArchiveFileData.Where(afd=>!afd.IsDirectory).Select(
                                            filedata => {
                                                string path =_Deploy ? Path.Combine(_Root, filedata.FileName) : Path.Combine(_Root, Path.GetFileName(filedata.FileName));
                                                return path.Replace(@" \", @"\");
                                            }) );

                    res = Files.Count > 0;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Descompacting error " + e.ToString());
                }

                return res;
            }
        }

        private class ZipLocalDescompactor : ZipNotSolidabstractDescompactor, IRarDescompactor
        {

            internal ZipLocalDescompactor(RarManagerImpl Father, SevenZipExtractor sex)
                : base(Father, sex)
            {
                _Root = Path.Combine(Path.GetDirectoryName(sex.FileName), FileInternalToolBox.RawRarName(sex.FileName));
                int rootl = _Root.Length;
                _IsValid = true;
                _Deploy = (rootl + _RarContext.MaxLengthBasic) < MAX_PATH;
            }


            protected override string TargetRelativePath(string FileNameAsInRar, bool Deploy)
            {
                return Deploy ? FileNameAsInRar : FileInternalToolBox.GetFileName(FileNameAsInRar);
            }
        }


        private class ZipDescompactor : ZipNotSolidabstractDescompactor, IRarDescompactor
        {

            internal ZipDescompactor(RarManagerImpl Father, SevenZipExtractor sex)
                : base(Father, sex)
            {
                _Root = _Father.ComputeName(_RarContext);

                int rootl = _Root.Length;
                _IsValid = true;
                _Deploy = (rootl + _RarContext.MaxLengthWithoutRoot) < MAX_PATH;
            }


            protected override string TargetRelativePath(string FileNameAsInRar, bool Deploy)
            {
                return Deploy ? _RarContext.ConvertFileName(FileNameAsInRar) : FileInternalToolBox.GetFileName(FileNameAsInRar);
            }
        }
    }

}
