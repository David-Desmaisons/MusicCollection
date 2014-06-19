using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using MusicCollection.Properties;
using MusicCollection.Fundation;

namespace MusicCollection.SettingsManagement
{
    internal class DeleteManagerImpl : IDeleteManager
    {
        private bool? _DeleteRemovedFile;
        internal DeleteManagerImpl(IMaturityUserSettings ibh)
        {
            switch (ibh.DeleteRemovedFile)
            {
                case BasicBehaviour.AskEndUser:
                    _DeleteRemovedFile = null;
                    break;

                case BasicBehaviour.No:
                    _DeleteRemovedFile = false;
                    break;

                case BasicBehaviour.Yes:
                    _DeleteRemovedFile = true;
                    break;

            }
        }

        public bool? DeleteFileOnDeleteAlbum
        {
            get { return _DeleteRemovedFile; }
        }
    }
}
