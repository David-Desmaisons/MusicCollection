using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IAlbumListSizeChecker : INotifyPropertyChanged
    {
        bool IsPertinent { get; }

        SpaceChecker SpaceCheck
        {
            get;
        }

        IEnumerable<IAlbum> Albums
        {
            get; 
            set;
        }

        string DirectoryName
        {
            get;
            set;
        }
    }

    public class AlbumListSizeChecker : NotifyCompleteListenerObject, IAlbumListSizeChecker
    {
        //private string _SpaceCheckProperty = "SpaceCheck";
        //private string _IsPertinentProperty = "IsPertinent";

        public AlbumListSizeChecker(string Directoryname)
        {
            _DN = Directoryname;
        }

        public bool IsPertinent
        {
            get { return Get<AlbumListSizeChecker, bool>(() => (t) => Directory.Exists(t.DirectoryName)); }
        }

        private SpaceChecker _SC = null;
        public SpaceChecker SpaceCheck
        {
            get { return IsPertinent? _SC : null; }
            set { Set(ref _SC, value); }
        }

        //private void UpdateSC(string Name)
        //{          
        //    var old = SpaceCheck;
        //    _DN = Name;
        //    _SC = new SpaceChecker(_DN, _Length);
        //    PropertyHasChanged(_SpaceCheckProperty, old, SpaceCheck);
        //}

        private void UpdateSC()
        {
            SpaceCheck = IsPertinent? new SpaceChecker(_DN, _Length) : null;
        }

        private string _DN;
        public string DirectoryName
        {
            get { return _DN; }
            set 
            {
                //if (_DN == value)
                //    return;

                //var old = IsPertinent;

                if (!Set(ref _DN, value))
                    return;
             
                //UpdateSC(value);
                UpdateSC();

                //PropertyHasChanged(_IsPertinentProperty, old, IsPertinent); 
            }
        }
  
        private Dictionary<IInternalAlbum, long> _SpaceNeededForAlbum = new Dictionary<IInternalAlbum, long>();

        private long AlbumSizeOnDisc(IInternalAlbum al)
        {
            return _SpaceNeededForAlbum.FindOrCreateEntity(al, a =>
                {
                    SizeComputer sc = new SizeComputer();
                    a.Visit(sc);
                    return sc.Length;
                }
            );
        }
        //    long res = 0;
        //    if (_SpaceNeededForAlbum.TryGetValue(al, out res))
        //        return res;

        //    SizeComputer sc = new SizeComputer();

        //    al.Visit(sc);

        //    res = sc.Length;
        //    _SpaceNeededForAlbum.Add(al, res);
        //    return res;
        //}

        private long _Length;
        private void ComputeLength()
        {
            _Length = _Albums.Sum(al => AlbumSizeOnDisc(al));
            UpdateSC();
            //UpdateSC(this._DN);
        }

        private List<IInternalAlbum> _Albums;
        public IEnumerable<IAlbum> Albums
        {
            get { return _Albums; }
            set
            {
                _Albums = value.Cast<IInternalAlbum>().ToList();
                ComputeLength();
            }
        }
    }
}