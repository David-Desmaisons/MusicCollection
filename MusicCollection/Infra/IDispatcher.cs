using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace MusicCollection.Infra
{
    public interface IDispatcher
    {
        Dispatcher GetDispatcher();
    }
}
