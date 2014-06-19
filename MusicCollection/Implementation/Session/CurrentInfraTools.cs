using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;
using MusicCollection.Infra.File;
using MusicCollection.ToolBox.ZipTools;
using MusicCollection.MusicPlayer;

namespace MusicCollection.Implementation.Session
{
    internal class CurrentInfraTools : IInfraDependencies
    {
        internal CurrentInfraTools(IMusicFactory imf)
        { 
            _Zipper = new SevenZipZipper();
            _Filer = new FileSystem();
            _MusicFactory = imf;
        }

        private IZipper _Zipper;
        public IZipper Zip
        {
            get { return _Zipper; }
        }

        private IFileTools _Filer;
        public IFileTools File
        {
            get { return _Filer; }
        }

        private IMusicFactory _MusicFactory;
        public IMusicFactory MusicFactory
        {
            get { return _MusicFactory; }
        }
    }
}
