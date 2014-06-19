//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using MusicCollection.Properties;
//using MusicCollection.Fundation;


//namespace MusicCollection.Implementation
//{
//    class MaturityManagerImpl :IMaturityManager
//    {

//        internal MaturityManagerImpl(ICollectionFileManagement icfm)
//        {
//            _Path = icfm.DirForPermanentCollection;
//            _RMC = icfm.ExportCollectionFiles;
//        }
//        //internal MaturityManagerImpl(string iPath, bool iExportCollectionFiles)
//        //{
//        //    _Path = iPath;
//        //    _RMC = iExportCollectionFiles;
//        //}


//        private bool _RMC;
//        public bool RerootOnMaturityChange
//        {
//            get { return _RMC; }
//        }

//        private string _Path;
//        public string RerootPath
//        {
//            get { return _Path; }
//        }
//    }
//}
