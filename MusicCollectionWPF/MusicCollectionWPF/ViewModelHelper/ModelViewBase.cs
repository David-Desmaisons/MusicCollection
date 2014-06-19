using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;

using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModelHelper
{
    abstract public class ViewModelBase : NotifyCompleteListenerObject, IDispatcher
    {
        private Lazy<List<IDisposable>> _Disposables = new Lazy<List<IDisposable>>();

        protected T Register<T>(T idependency) where T : IDisposable
        {
            _Disposables.Value.Add(idependency);
            return idependency;
        }

        protected ViewModelBase Father
        {
            get;
            set;
        }

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

        public override void Dispose()
        {
            base.Dispose();
            if (_Disposables.IsValueCreated)
            {
                _Disposables.Value.Apply(t => t.Dispose());
                _Disposables.Value.Clear();
            }
        }
    }
}
