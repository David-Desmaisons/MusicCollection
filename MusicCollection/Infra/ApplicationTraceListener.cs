using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MusicCollection.Infra
{
    internal class ApplicationTraceListener : TraceListener
    {
        private LinkedList<Tuple<DateTime, string>> _Events = new LinkedList<Tuple<DateTime, string>>();

        public override void Write(string message)
        {
            _Events.AddLast(new Tuple<DateTime, string>(DateTime.Now,message));
        }

        public override void WriteLine(string message)
        {
            _Events.AddLast(new Tuple<DateTime, string>(DateTime.Now, message));
        }

        internal IEnumerable<Tuple<DateTime, string>> Events
        {
            get
            {
                return _Events;
            }
        }
    }
}