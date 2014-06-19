using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.Implementation;
using MusicCollection.FileConverter;


namespace MusicCollection.FileImporter
{
    internal class FolderInspector
    {
        private IEnumerable<IImporter> _Importers = null;
        private DirectoryInfo _RootDir;
        private int _SkipDir = 0;
        private IEventListener _IEL;
        private IInternalMusicSession _IMusicConverter;

        internal FolderInspector(IInternalMusicSession iMusicConverter, DirectoryInfo DI, IEventListener iel)
        {
            _IMusicConverter = iMusicConverter;
            _RootDir = DI;
            _IEL = iel;
            string DirName = Path.GetDirectoryName(DI.FullName);
            _SkipDir = (DirName == null) ? DI.FullName.Length : DirName.Length;
            _SkipDir++;
        }

        private void Compute()
        {
            List<ICollector> validated = new List<ICollector>();
            _Importers = from c in validated.AddCollection(InternalCompute(_RootDir, validated)) where c.Importer != null select c.Importer;
        }

        private List<ICollector> InternalCompute(DirectoryInfo DI, List<ICollector> validated)
        {  
   
            List<ICollector> RemainingCollector = new List<ICollector>();

            string StringHelper = _SkipDir>DI.FullName.Length? string.Empty : DI.FullName.Substring(_SkipDir);
     
            //get collector of the current folder     
            foreach (ICollector Co in new NonRecursiveFolderInspector(_IMusicConverter, from di in DI.GetFiles() let fname = di.FullName select di.FullName, new FolderImporterHelper(StringHelper), _IEL).Collectors)
            {
                if (Co != null)
                {
                    //if sealed I can add to add to the finallist               
                    if (Co.IsSealed)
                        validated.Add(Co);
                    else
                        RemainingCollector.Add(Co);
                }
            }

            //trying to merge remaining collectors and collector from subdirectories
            var SubCollectors = (from di in DI.GetDirectories() from o in InternalCompute(di, validated) select o).ToList();

            if (RemainingCollector.Count == 0)
                return SubCollectors;

            if (SubCollectors.Count == 0)
                return RemainingCollector;

            bool Mergeable = true;
            bool SelfMergeable = SubCollectors.Count>1;


            int Size = SubCollectors.Count;
            for (int i = 0; i < Size;i++)
            {
                ICollector col = SubCollectors[i];
                if (!col.IsMergeable(RemainingCollector[0]))
                    Mergeable = false;

                for (int j=i+1;j<Size;j++)
                {
                    ICollector col2 = SubCollectors[j];

                    if (!col.IsMergeable(col2))
                        SelfMergeable = false;
                }
            }

            if (SelfMergeable)
            {
                ICollector res = SubCollectors[0];
                for (int i=1;i< SubCollectors.Count;i++)
                    res = res.Merge(SubCollectors[i]);
                SubCollectors.Clear();
                SubCollectors.Add(res);
            }

            if (!Mergeable)
            {
                validated.AddRange(SubCollectors);
                return RemainingCollector;
            }

            validated.AddRange(from a in SubCollectors from b in RemainingCollector select a.Merge(b));
            return new List<ICollector>();
        }

        internal IEnumerable<IImporter> Collectors
        {
            get
            {
                if (_Importers==null)
                    Compute();

                return _Importers;
            }
        }
    }
}
