using MusicCollection.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicCollectionWPF.ViewModelHelper
{
    public class WPFDispatcher : IDispatcher
    {
        public Dispatcher GetDispatcher()
        {
            return Application.Current.Dispatcher;
        }
    }
}
