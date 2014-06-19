using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.Infra
{
    public static class TaskExtender
    {

        public static async Task WithTimeout(this Task task, int millisecondstime, CancellationTokenSource iCancellationTokenSource)
        {
            Task delayTask = Task.Delay(millisecondstime);
            Task firstToFinish = await Task.WhenAny(task, delayTask);
            if (firstToFinish == delayTask)
            {
                iCancellationTokenSource.Cancel();
                // The delay finished first - deal with any exception
                task.DoNotWaitSafe();
                return;
            }

            return; // If we reach here, the original task already finished
        }

        public static void DoNotWaitSafe(this Task task)
        {
            #pragma warning disable 4014
            task.ContinueWith(HandleException);
            #pragma warning restore 4014
        }

        private static void HandleException(Task task)
        {
            if (task.Exception != null)
            {
                Trace.WriteLine(task.Exception);
            }
        }


    }
}
