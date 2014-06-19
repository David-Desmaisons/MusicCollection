using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MusicCollection.Infra
{
    public class TimeTracer : IDisposable
    {
        private Stopwatch _StopWatch;
        private TimeSpan _Init;
        private string _Name;
        private string _fn;
        private Process _Pro;

        private TimeTracer(string iName,string fn):this(iName)
        {
            _fn = fn;
        }

        private TimeTracer(string iName)
        {
            _StopWatch = new Stopwatch();

            _Name = iName;
            EllapsedTimeSpent = null;
            CPUTimeSpent = null;
            _Pro = Process.GetCurrentProcess();

            _StopWatch.Start();         
            _Init = _Pro.TotalProcessorTime;
        }

        internal Nullable<TimeSpan> EllapsedTimeSpent
        {
            get;
            private set;
        }

        internal Nullable<TimeSpan> CPUTimeSpent
        {
            get;
            private set;
        }

        static public TimeTracer TimeTrack(string Name)
        {
            return new TimeTracer(Name);
        }

        static public TimeTracer TimeTrack(string Name, string fn)
        {
            return new TimeTracer(Name,fn);
        }

        private void TraceTimeSpan(TimeSpan ts, string context)
        {
            string res = null;

            if (_fn == null)
            {
                res = string.Format("{4}: {6} {0:00}:{1:00}:{2:00}.{3:00}  Ticks #:{5}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, _Name, ts.Ticks, context);
            }
            else
            {
                res = string.Format("{4}: {6} {0:00}:{1:00}:{2:00}.{3:00}  Ticks #:{5} - {7}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, _Name, ts.Ticks, context, _fn);
            }

            Trace.WriteLine(res);
        }

        public void Dispose()
        {
            TimeSpan fin = _Pro.TotalProcessorTime;
            _StopWatch.Stop();
            TimeSpan ts = _StopWatch.Elapsed;

            TimeSpan cpu = fin - _Init;
            this.CPUTimeSpent = cpu;
        
            EllapsedTimeSpent = ts;
            // Format and display the TimeSpan value.
            TraceTimeSpan(ts,string.Empty);
            //TraceTimeSpan(cpu, "CPU");
        }
    }
}
