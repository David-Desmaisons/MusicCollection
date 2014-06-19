using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Implementation
{
    internal interface IExportListener
    {
        bool Export{get;}

        void AddExportAlbum(ExportAlbum EA);

        bool Normalisename{get;}

        bool ExportImage { get; }

        void FileBroken(Track FileName);

        void UnableCopyFile(Track FileName);

        void UnableCopyImage();

        bool StopOnFileExist(Track FileName,string TargetPath);

        bool StopOnImageExist(string TargetPath);
    }
}
