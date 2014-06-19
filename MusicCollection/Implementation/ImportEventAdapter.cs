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

    abstract internal class UIThreadSafeImportEventAdapter : IImporterEvent
    {
        private UISafeEvent<ImportExportErrorEventArgs> _Error;
        private UISafeEvent<ProgessEventArgs> _Progress;

        public event EventHandler<ImportExportErrorEventArgs> Error
        {
            add { _Error.Event += value; }
            remove { _Error.Event -= value; }
        }

        public event EventHandler<ProgessEventArgs> Progress
        {
            add { _Progress.Event += value; }
            remove { _Progress.Event -= value; }
        }

        protected UIThreadSafeImportEventAdapter()
        {
            _Error = new UISafeEvent<ImportExportErrorEventArgs>(this);
            _Progress = new UISafeEvent<ProgessEventArgs>(this);
        }

        protected void OnError(ImportExportErrorEventArgs Error)
        {
            _Error.Fire(Error,true);
        }

        protected void OnProgress(ProgessEventArgs Where)
        {
            _Progress.Fire(Where,false);
        }
    }
}
