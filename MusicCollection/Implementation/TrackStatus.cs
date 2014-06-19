using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Implementation
{
    internal interface ITrackStatusVisitor
    {
        void Visit(string Path, TrackStatus ts);
    }


    internal class TrackStatus
    {
        internal Track Found
        {
            get;
            private set;
        }

        internal AlbumStatus AlbumInfo
        {
            get;
            private set;
        }

        internal bool Exist
        {
            get;
            private set;
        }

        internal bool AbortAlbumExist
        {
            get
            {
                return (Found == null);
            }
        }

        internal bool Continue
        {
            get 
            {
                if ((Found == null) || (Exist))
                    return false;

                if (AlbumInfo == null)
                    throw new Exception("Algo error");

                return AlbumInfo.Continue;
            }
        }

        internal TrackStatus(Track tr)
        {
            Found = tr;
            AlbumInfo = null;
            Exist = true;
        }

        //internal TrackStatus()
        //{
        //    Found = null;
        //    AlbumInfo = null;
        //    Exist = false;
        //}

        internal TrackStatus(AlbumStatus Ial)
        {
            Found = null;
            AlbumInfo = Ial;
            Exist = false;
        }

        internal TrackStatus(Track tr,AlbumStatus Ial)
        {
            Found = tr;
            AlbumInfo = Ial;
            Exist = false;
        }
    }


    internal enum AlbumInfo { NewToTransaction, AlreadyImported_ExactMatch,ValidatedByEU, RefusedByEU };

    internal class AlbumStatus
    {
  
        internal Album Found
        {
            get;
            private set;
        }

        internal AlbumInfo Status
        {
            get;
            private set;
        }

        //internal bool Exist
        //{
        //    get;
        //    private set;
        //}

        internal bool Continue
        {
            get { return (Found != null); }
            //set;
        }

        public override string ToString()
        {
            return string.Format("Status:{0} Album to consider:{1} continue:{2}",Status,Found,Continue);
        }

        //internal AlbumStatus(Album Ial, bool iExist)
        //{
        //    Found = Ial;
        //    Exist = iExist;
        //    Continue = !iExist;
        //}

        internal AlbumStatus(Album Ial, AlbumInfo ai)
        //, bool iExist,
        {
            Found = Ial;
            //Exist = iExist;
            //Continue = ;

            Status = ai;

            switch (ai)
            {
                case AlbumInfo.NewToTransaction:
                case AlbumInfo.ValidatedByEU:
                    if (Found == null)
                        throw new Exception();
                    break;

                case AlbumInfo.RefusedByEU:
                    if (Found!=null)
                        throw new Exception();
                    break;
            }
        }

    }
}
