using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox
{
    internal class SimpleProgress<T> : IProgress<T>
    {
        private Action<T> _Action;
        internal SimpleProgress(Action<T> iAction)
        {
            _Action = iAction;
        }
        public void Report(T value)
        {
            _Action(value);
        }
    }
}
