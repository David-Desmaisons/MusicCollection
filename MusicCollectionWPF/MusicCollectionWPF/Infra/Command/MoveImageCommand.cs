using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MusicCollectionWPF.Infra
{
    public class MoveImageCommand
    { 
        static MoveImageCommand()
        {
            ToFirst = new RoutedUICommand("ToFirst", "ToFirst", typeof(MoveImageCommand));
            ToLast = new RoutedUICommand("ToLast", "ToLast", typeof(MoveImageCommand));
        }

        public static RoutedUICommand ToFirst
        {
            get;
            private set;
        }

        public static RoutedUICommand ToLast
        {
            get;
            private set;
        }
    }

   
}
