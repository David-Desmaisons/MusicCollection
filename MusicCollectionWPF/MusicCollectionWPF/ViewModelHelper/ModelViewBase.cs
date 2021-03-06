﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;

using MusicCollection.Infra;
using MusicCollectionWPF.Infra;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModelHelper
{
    abstract public class ViewModelBase : NotifyCompleteListenerObject, IDispatcher
    {
        
        protected ViewModelBase Father { get; set; }

        private IWindow _IWindow;
        internal IWindow Window
        {
            get { if (_IWindow != null) return _IWindow; if (Father != null) return Father.Window; return null; }
            set { _IWindow=value;}
        }

        internal virtual bool CanClose()
        {
            return true;
        }

        public Dispatcher GetDispatcher()
        {
            return Application.Current.Dispatcher;
        }

        public virtual Task InitAsync()
        {
            return Task.FromResult<object>(null);
        }

        protected async Task RunAsync(Action iaction)
        {
           await this.GetDispatcher().ExecuteAsync(() => iaction() );
        }
    }
}
