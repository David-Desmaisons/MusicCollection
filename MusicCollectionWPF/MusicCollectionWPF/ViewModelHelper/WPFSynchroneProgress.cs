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
        protected Dispatcher _UIDispatcher;
        public WPFSynchroneProgress(Action<T> action)
        {
            _Action = action;
            if (App.Current!=null)
                _UIDispatcher = App.Current.Dispatcher;
        }
        void IProgress<T>.Report(T value)
        {
            Action ac = () => _Action(value);
            if (_UIDispatcher != null)
                _UIDispatcher.Invoke(ac);
            else
                ac();
        }
    }

    public class WPFSynchroneProgress<T1, T2> :  WPFSynchroneProgress<T1>, IProgress<T2>
    {
        private Action<T2> _Action2;
        public WPFSynchroneProgress(Action<T1> action1, Action<T2> action2): base(action1)
        {
            _Action2 = action2;
        }
       
        void IProgress<T2>.Report(T2 value)
        {
            Action ac = () => _Action2(value);
            if (_UIDispatcher != null)
                _UIDispatcher.Invoke(ac);
            else
                ac();
        }
    }
}
