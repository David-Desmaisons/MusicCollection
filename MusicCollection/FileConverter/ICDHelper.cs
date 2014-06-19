using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.FileConverter
{
    internal interface ICDHelper
    {
        bool  OpenCDDoor();

        bool CanOpenCDDoor(int CDnumber);

        bool IsCDAudio(int CDnumber);
    }
}
