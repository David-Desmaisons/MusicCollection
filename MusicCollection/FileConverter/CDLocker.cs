using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Un4seen.Bass;
using Un4seen.Bass.AddOn.Cd;
using Un4seen.Bass.AddOn;

namespace MusicCollection.FileConverter
{
    internal class CDLocker:IDisposable
    {
        private int _CDID;
        private bool _OK;
        private bool _IsValid;

        private CDLocker(int CDid)
        {
            _CDID = CDid;
            _OK = BassCd.BASS_CD_Door(_CDID, BASSCDDoor.BASS_CD_DOOR_LOCK);
            _IsValid = BassCd.BASS_CD_IsReady(_CDID);
        }

        public bool IsOK
        {
            get { return _OK && _IsValid; }
        }

        static public  CDLocker GetLocker(int CDID)
        {
            return new CDLocker(CDID);
        }

        public void Dispose()
        {
            if (_OK)
            {
                BassCd.BASS_CD_Door(_CDID, BASSCDDoor.BASS_CD_DOOR_UNLOCK);
            }
        }
    }
}
