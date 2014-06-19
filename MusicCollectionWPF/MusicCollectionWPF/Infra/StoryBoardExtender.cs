using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.Infra
{
    public static class StoryBoardExtender
    {

        public static Task RunAsync(this Storyboard @this)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            EventHandler handler = null;
            handler = delegate
            {
                @this.Completed -= handler;
                tcs.TrySetResult(null);
            };
            @this.Completed += handler;
            @this.Begin();

            return tcs.Task;
        }
    }
}
