using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Implementation.Session
{
    public interface IMainWindowHwndProvider
    {
        IntPtr MainWindow
        {
            get;
        }
    }
}
