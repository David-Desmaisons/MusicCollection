using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private IMusicPlayer _IMusicPlayer;
        public PlayerViewModel(IMusicPlayer iMusicPlayer)
        {
            _IMusicPlayer = iMusicPlayer;

            Play = RelayCommand.Instanciate(()=>_IMusicPlayer.Mode = PlayMode.Play);
            Pause = RelayCommand.Instanciate(() => _IMusicPlayer.Mode = PlayMode.Paused);
            VolumeUp = RelayCommand.Instanciate(() => _IMusicPlayer.Volume += 0.1);
            VolumeDown = RelayCommand.Instanciate(() => _IMusicPlayer.Volume -= 0.1);
            Like = RelayCommand.Instanciate(DoLike);
        }

        public double Volume
        {
            get { return _IMusicPlayer.Volume; }
            set { _IMusicPlayer.Volume = value; }
        }

        public IList<IAlbum> Albums { get; private set; }

        #region Command

        public ICommand Play { get; private set; }

        public ICommand Pause { get; private set; }

        public ICommand VolumeUp { get; private set; }

        public ICommand VolumeDown { get; private set; }

        public ICommand Like { get; private set; }


        private void DoLike()
        {
            if (_IMusicPlayer.Mode == PlayMode.Stopped)
                return;

            ITrack track = _IMusicPlayer.PlayList.CurrentTrack;
            if (track == null)
                return;

            track.Rating = 5;
        }


        #endregion


        public void StopPlay()
        {
            _IMusicPlayer.Mode = PlayMode.Paused;
        }

        public void AddAlbumAndPlay(IEnumerable<IAlbum> ialls)
        {
        }

        public void AddAlbumAndPlay(IEnumerable<ITrack> trcs)
        {
        }


    } 
}
