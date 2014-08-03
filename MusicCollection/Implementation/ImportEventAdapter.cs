using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Event;

namespace MusicCollection.Implementation
{
    abstract internal class SimpleImportEventAdapter : IImporterEvent
    {
        private event EventHandler<ImportExportErrorEventArgs> _Error;
        private event EventHandler<ProgessEventArgs> _Progress;

        public event EventHandler<ImportExportErrorEventArgs> Error
        {
            add { _Error += value; }
            remove { _Error -= value; }
        }

        public event EventHandler<ProgessEventArgs> Progress
        {
            add { _Progress += value; }
            remove { _Progress -= value; }
        }

        protected bool ListeningError
        { get { return (_Error != null); } }

        protected void OnError(ImportExportErrorEventArgs Error)
        {
            if (_Error != null)
            {
                _Error(this, Error);
            }
        }

        protected void OnProgress(ProgessEventArgs Where)
        {
            if (_Progress != null)
            {
                _Progress(this, Where);
            }
        }
    }

}
