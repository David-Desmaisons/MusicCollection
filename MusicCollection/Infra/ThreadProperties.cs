using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace MusicCollection.Infra
{
    public class ThreadProperties
    {
        public ProcessPriorityClass? ProcessPriorityClass
        {
            get;
            private set;
        }

        public ThreadPriority ThreadPriority
        {
            get;
            private set;
        }

        public ThreadProperties( ThreadPriority iThreadPriority,ProcessPriorityClass? iProcessPriorityClass)
        {
            ProcessPriorityClass = iProcessPriorityClass;
            ThreadPriority = iThreadPriority;
        }

        public ThreadProperties(Thread iThread)
        {
            ProcessPriorityClass = Process.GetCurrentProcess().PriorityClass;
            ThreadPriority = iThread.Priority;
        }

        static public ThreadProperties FromCurrentThread()
        {
            return new ThreadProperties(Thread.CurrentThread);
        }

        public void SetCurrentThread()
        {
            if (ProcessPriorityClass.HasValue)
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Value;
            Thread.CurrentThread.Priority = ThreadPriority;
        }

    }


    public static class ThreadPropertiesExtender
    {
        private class ThreadPropertiesChanger :IDisposable
        {
            private ThreadProperties _Original;
            private ThreadProperties _Target;
            public ThreadPropertiesChanger(ThreadProperties itarget)
            {
                _Original = ThreadProperties.FromCurrentThread();
                _Target = itarget;
                _Target.SetCurrentThread();
            }

            public void Dispose()
            {
                _Original.SetCurrentThread();
            }
        }

        public static IDisposable GetChanger(this ThreadProperties @this)
        {
            if (@this == null)
                return null;

            return new ThreadPropertiesChanger(@this);
        }
    }



}
