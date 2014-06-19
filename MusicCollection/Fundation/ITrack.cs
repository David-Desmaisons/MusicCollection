using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{

    public interface ITrack : INotifyPropertyChanged, IMusicObject, IComparable<ITrack>
    {
        IAlbum Album
        {
            get;
        }

        int ID
        {
            get;
        }

        IMusicSession Session
        {
            get;
        }

        #region Metada

        string Artist
        {
            get;
        }

        string Name
        {
            get;
        }

        uint TrackNumber
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        uint DiscNumber
        {
            get;
        }

        ISingleTrackEditor GetEditor();

    #endregion

    #region File

        string Path
        {
            get;
        }

        FileStatus FileExists
        {
            get;
        }

        string MD5HashKey
        {
            get;
        }

        //DateTime TimeStamp
        //{
        //    get;
        //}

    #endregion

    #region Collection

        DateTime? DateAdded
        {
            get;
        }

        DateTime? LastPlayed
        {
            get;
        }

        uint Rating
        {
            get;
            set;
        }

        int PlayCount
        {
            get;
        }

        int SkippedCount
        {
            get;
        }

    #endregion


    }

}


        //MD

        //string Album Name     done
        //string Artist         done
        //string Name           done
        //uint TrackNumber      done   
        //TimeSpan Duration     done
        //ISRC

        //Collection Related

        //DateTime DateAdded    done
        //Note                  done
        //#Played               done
        //lastplayed            done

        //FR

        //string Path           done
        //string MD5HashKey     done
        //bool IsBroken         done
        //TimeStamp             TODO


        
