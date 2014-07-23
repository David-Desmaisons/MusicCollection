using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Un4seen.Bass.AddOn.Cd;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.DataExchange
{
   

    internal class CDInfoHandler : ICDInfoHandler
    {
        private int _CDNumber;
        private DiscIDs _ID;

        internal CDInfoHandler(int CDNumber)
        {
            _CDNumber = CDNumber;
            _ID = DiscIDs.FromBassInfo(_CDNumber);
            Tocs = BassCd.BASS_CD_GetTOC(_CDNumber, BASSCDTOCMode.BASS_CD_TOC_LBA).tracks.Select(t => t.lba).ToList();
        }

        internal int CDPlayer
        {
            get { return _CDNumber; }
        }

        public bool IsReady
        {
            get { return BassCd.BASS_CD_IsReady(CDPlayer); }
        }

        public int TrackNumbers
        {
            get
            {
                return BassCd.BASS_CD_GetTracks(_CDNumber);
            }
        }

        public TimeSpan Duration(int traknumber)
        {
            return TimeSpan.FromSeconds(BassCd.BASS_CD_GetTrackLengthSeconds(_CDNumber, traknumber));
        }

        public string Driver
        {
            get
            {
                return string.Format("{0}:", BassCd.BASS_CD_GetInfo(_CDNumber).DriveLetter);
            }
        }


        public IDiscIDs IDs
        {
            get { return _ID; }
        }

        public List<int> Tocs { get; private set; }
    }
}