using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WPSync.Shared;

namespace MusicCollection.WindowsPhone
{
    public class WindowsDictionaryDecorator : IObserver<KeyValuePair<string, object>>
    {
        private Lazy<Dictionary> _Dic = new Lazy<Dictionary>();
        public Dictionary Dictionary
        {
            get { return _Dic.Value; }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Value == null)
                return;

            Dictionary.SetObjectForKey(value.Value,value.Key);
        }
    }
}
