using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace MusicCollection.Infra
{
    public class NotifySimpleAdapter : INotifyPropertyChanged 
    {
        protected NotifySimpleAdapter()
        {
        }
 
        protected void PropertyHasChanged(string PropertyName)
        {
           
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
