using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Un4seen.Bass.AddOn.Cd;

namespace MusicCollection.FileConverter
{
    static internal class CDHelper
    {
        static internal bool  OpenCDDoor()
        {
            return BassCd.BASS_CD_Door(0,BASSCDDoor.BASS_CD_DOOR_OPEN);
        }

        static internal bool CanOpenCDDoor(int CDnumber)
        {
            return BassCd.BASS_CD_GetInfo(CDnumber).canopen;
        }

        static internal bool  IsCDAudio (int CDnumber)
        {
            return (BassCd.BASS_CD_GetTrackLength(CDnumber, 0) != -1);
        }

    }

    internal class BassCDHelper : ICDHelper
    {
        public bool OpenCDDoor()
        {
            return BassCd.BASS_CD_Door(0, BASSCDDoor.BASS_CD_DOOR_OPEN);
        }

        public bool CanOpenCDDoor(int CDnumber)
        {
            return BassCd.BASS_CD_GetInfo(CDnumber).canopen;
        }

        public bool IsCDAudio(int CDnumber)
        {
            return (BassCd.BASS_CD_GetTrackLength(CDnumber, 0) != -1);
        }
    }
}
