using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Implementation;
using MusicCollection.FileConverter;

namespace MusicCollection.FileImporter
{
    internal class NonRecursiveFolderInspector
    {
        private IEnumerable<string> _Files;
        private List<string>[] _Music = new List<string>[10] { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
        private bool _Computed = false;
        private IImportHelper _ClueName = null;
        private IEventListener _IEL;
        private IInternalMusicSession _IMusicConverter;

        private List<string> GetList(FileType itype)
        {
            return _Music[(int)itype];
        }

        internal NonRecursiveFolderInspector(IInternalMusicSession iIMusicConverter, IEnumerable<string> Files, IImportHelper Clue, IEventListener iel)
        {
            _IMusicConverter = iIMusicConverter;
            _Files = Files;
            _ClueName = Clue;
            _IEL = iel;
        }

        void Compute()
        {
            if (_Computed)
                return;

            _Computed = true;

            foreach (string Fi in _Files)
            {
                FileType type = Fi.GetFileType();

                if (type != FileType.Unknown)
                {
                    GetList(type).Add(Fi);
                }
            }
        }


        internal IEnumerable<ICollector> Collectors
        {
            get
            {
                Compute();

                bool NoMusic = true;

                if (GetList(FileType.Music).Count > 0)
                {
                    NoMusic = false;
                    yield return CollectorFactory.CollectorForMusicToImport(GetList(FileType.Music), GetList(FileType.Image), _ClueName);
                }

                if (GetList(FileType.MusicToConv).Count > 0)
                {
                    NoMusic = false;
                    yield return CollectorFactory.CollectorForMusicToConvert(_IMusicConverter.MusicConverter,GetList(FileType.MusicToConv), GetList(FileType.Image), GetList(FileType.CueSheet), _ClueName, _IEL);
                }

                if (GetList(FileType.RarCompressed).Count > 0)
                {
                    foreach (ICollector rar in CollectorFactory.CollectorRar(_IMusicConverter,GetList(FileType.RarCompressed)))
                    {
                        yield return rar;
                    }
                }

                if (NoMusic && (GetList(FileType.Image).Count > 0))
                    yield return CollectorFactory.CollectorImages(GetList(FileType.Image));
            }
        }

        internal ImporterConverterAbstract[] Importers
        {
            get { return (from c in Collectors let Im = c.Importer where Im != null select Im).ToArray();}
        }
    }
}
