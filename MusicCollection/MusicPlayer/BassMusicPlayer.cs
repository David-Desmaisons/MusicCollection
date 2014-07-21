using MusicCollection.FileConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace MusicCollection.MusicPlayer
{
    internal class BassMusicPlayer : InternalPlayerAdapter, IInternalPlayer
    {
        private BassMusicConverter _BassMusicConverter;
        private int _CurrentStream;
        private TimeSpan _MaxPosition;
        public BassMusicPlayer(BassMusicConverter bmc):base(100)
        {
            _BassMusicConverter = bmc;
        }

        private void Clean()
        {
            if (_CurrentStream != 0)
            { 
                Bass.BASS_StreamFree(_CurrentStream);
                _CurrentStream = 0;
            }
        }

        private void GetStreamFromFile()
        {
            _CurrentStream = Bass.BASS_StreamCreateFile(_FileSource, 0, 0, BASSFlag.BASS_DEFAULT);         
            _MaxPosition = TimeSpan.FromSeconds(
                Bass.BASS_ChannelBytes2Seconds(_CurrentStream, Bass.BASS_ChannelGetLength(_CurrentStream)));

            if (_CurrentStream!=0)
                Listener.OnTrackLoadedPlay();
        }

        private void UpdateFile()
        {
            Clean();

            _BassMusicConverter.InitAndValidate(_FileSource);

            GetStreamFromFile();
   
            //Listener.OnTrackLoadingForPlay();

            //Listener.OnTrackLoadedPlay();
        }

        protected override void DoStop()
        {
            if (_CurrentStream!=0)
                Bass.BASS_ChannelStop(_CurrentStream);
        }

        protected override void DoPlay()
        {
            if (_CurrentStream == 0)
            {
                GetStreamFromFile();
            }
                
            Bass.BASS_ChannelPlay(_CurrentStream, false);
        }

        protected override void DoPause()
        {
            if (_CurrentStream != 0)
                Bass.BASS_ChannelPause(_CurrentStream);
        }

        protected override void DoClose()
        {
            DoStop();
            Clean();
        }

        protected override void OnTimer()
        {
            var p = Position;
            if (p >= _MaxPosition)
                Listener.OnTrackEnd();
            else
                Listener.OnTrackPlayingEvent(Position,_MaxPosition);
        }

        public IInternalPlayerListener Listener { get; set; }

        private string _FileSource;
        public string FileSource
        {
            get
            {
                return _FileSource;
            }
            set
            {
                if (_FileSource == value)
                    return;

                if (string.IsNullOrEmpty(value))
                { 
                    Stop();
                    Clean();
                    return;
                }

                _FileSource = value;

                UpdateFile();
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_CurrentStream == 0)
                    return TimeSpan.FromSeconds(0);

                return TimeSpan.FromSeconds( Bass.BASS_ChannelBytes2Seconds(_CurrentStream, 
                                                Bass.BASS_ChannelGetPosition(_CurrentStream)));
            }
            set
            {
                if (_CurrentStream == 0)
                    return ;

                var position = Bass.BASS_ChannelSeconds2Bytes(_CurrentStream, value.TotalSeconds);
                Bass.BASS_ChannelSetPosition(_CurrentStream, position);
            }
        }

        public double Volume
        {
            get
            {
                return ((double)Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM) / 10000d);
            }
            set
            {
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM, (int)(Math.Truncate(value * 10000)));
            }
        }

        public TimeSpan MaxPosition
        {
            get { return _MaxPosition; }
        }
    }
}
