//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;

//using MusicCollection.FileImporter;
//using MusicCollection.Fundation;
//using MusicCollection.Implementation;

//namespace MusicCollection.FileConverter
//{
//    class MusicCueSelectorImporter : IImporter
//    {
//        private List<string> _Music;
//        private List<string> _Image;
//        private List<string> _Cues;
//        private IImportHelper _ClueName;

//        MusicCueSelectorImporter(List<string> Music, List<string> Image, List<string> Cues, IImportHelper ClueName)
//        {
//            _Music = Music;
//            _Image = Image;
//            _Cues = Cues;
//            _ClueName = ClueName;
//        }

//        public IImportContext Context { set; get; }

//        public IImporter Action(IEventListener iel)
//        {
//            throw new NotImplementedException();
//        }

//        public ImportType Type
//        {
//            get { throw new NotImplementedException(); }
//        }
//    }
//}
