using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicCollectionWPF.Infra
{
    public static class WPFTask
    {
        public static Task ExecuteAsync(this Dispatcher @this,Action iDo)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Action Execute = ()=>
                {
                    try
                    {
                        iDo();
                        tcs.SetResult(null);
                    }
                    catch(Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                };
            @this.BeginInvoke(Execute);
            return tcs.Task;
        }
    }
}
