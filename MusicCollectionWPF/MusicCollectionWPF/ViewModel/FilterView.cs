using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel.Filter;

namespace MusicCollectionWPF.ViewModel
{

    public class FilterView : NotifyCompleteListenerObject
    {

        private IMusicFilter _FO;
        public IMusicFilter FilteringObject
        {
            get { return _FO; }
            set
            {
                if (_FO == value)
                    return;

                var old = _FO;
                _FO = value;
                
                this.PropertyHasChanged("FilteringObject", old, _FO);
                this.PropertyHasChangedUIOnly("FilteringEntity");

                if (old != null)
                    old.Dispose();

                UpdateExpression();
            }
        }

      

        private IMusicSession _IMS;
        public FilterView(IMusicSession ims)
        {
            _IMS = ims;
            _FO = new NoFilter();

              _DF = new DynamicFilter<IAlbum>(_FO.AlbumFilter);
              _DFT = new DynamicFilter<ITrack>(_FO.TrackFilter);
        }



        
        public IFunction<IAlbum, bool> FilterAlbum
        {
            get { return _DF; }
        }

        public IFunction<ITrack, bool> FilterTrack
        {
            get { return _DFT; }
        }

        private DynamicFilter<IAlbum> _DF;


        //private FilterType _FE;
        public FilterType FilteringEntity
        {
            get { return FilteringObject.Type; }
        }

        //static readonly IResultListenerFactory<FilterView, string> _FilteringDisplayFactory = ListenerFunctionBuilder.Register<FilterView, string>((t) => t.FilteringObject.DisplayName, "FilteringDisplay");     
        public string FilteringDisplay
        {
            //get { return this.GetValue(_FilteringDisplayFactory); }
            get { return Get<FilterView, string>(() => (t) => t.FilteringObject.DisplayName); }
        }

        //public AllFilter FilterAll
        //{
        //    get { return _FO as AllFilter; }
        //}

        //public void SetFilterAll(string f)
        //{
        //    AllFilter fo = FilterAll;
        //    if (fo != null)
        //    {
        //        fo.Filter = f;
        //    }
        //    else
        //        this.FilteringObject = new AllFilter(f);
        //}

        public override string ToString()
        {
            return string.Format("FilterValue:<{0}> FilterType:<{1}> ", this._FO, this.FilteringEntity);
        }

        private Expression<Func<IAlbum, bool>> FilterAlbumExpression
        {
            get { return _DF.FilterExpression; }
            set
            {
                _DF.FilterExpression = value;
             }
        }

        private DynamicFilter<ITrack> _DFT;
        private Expression<Func<ITrack, bool>> FilterTrackExpression
        {
            get { return _DFT.FilterExpression; }
            set
            {
                _DFT.FilterExpression = value;
             }
        }

  
        private void UpdateExpression()
        {
            FilterAlbumExpression = _FO.AlbumFilter;
            FilterTrackExpression = _FO.TrackFilter;

        }

        public override void Dispose()
        {
            base.Dispose();
            if (_FO != null)
                _FO.Dispose();
        }

     }
}
