using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollection.DataExchange
{
    internal class LoadingAlbumDescriptor : AlbumDescriptor
    {
        private Func<List<AImage>> _LoadAction = null;
        public LoadingAlbumDescriptor()
        {
        }

        internal Func<List<AImage>> LoadAction
        {
            //get { return _LoadAction; }
            set { _LoadAction = value; }
        }

        protected override AlbumDescriptor BasicClone()
        {
            return new LoadingAlbumDescriptor() { _LoadAction = this._LoadAction };
        }

        public override bool HasImage()
        {
            if (base.HasImage())
                return true;

            return (_LoadAction != null);
        }

        public void Load()
        {
            if (_LoadAction != null)
            {
                RawImages = _LoadAction();
                _LoadAction = null;
            }
        }
    }

    
}
