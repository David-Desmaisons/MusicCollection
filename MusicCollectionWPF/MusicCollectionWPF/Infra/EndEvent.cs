using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MusicCollectionWPF.Infra
{
    class EndEvent : RoutedEventArgs
    {
        public bool OK
        {
            get;
            private set;
        }

        public EndEvent(bool iOK)
        {
            OK = iOK;
        }
    }
}
