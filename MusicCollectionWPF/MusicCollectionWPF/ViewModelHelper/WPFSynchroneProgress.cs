using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicCollectionWPF.ViewModelHelper
{
    public class WPFSynchroneProgress<T> : IProgress<T>
    {
        private Action<T> _Action;
        private Dispatcher _UIDispatcher;
        public WPFSynchroneProgress(Action<T> action)
        {
            _Action = action;
            if (App.Current!=null)
                _UIDispatcher = App.Current.Dispatcher;
        }
        public void Report(T value)
        {
            Action ac = () => _Action(value);
            if (_UIDispatcher != null)
                _UIDispatcher.Invoke(ac);
            else
                ac();
        }
    }
}
