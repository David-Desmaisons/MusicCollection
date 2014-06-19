using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.WindowsPhone
{
    public class DictionaryObservableDecorator<Tkey, TValue> : IObserver<KeyValuePair<Tkey, TValue>>
    {
        private IDictionary<Tkey, TValue> _Dictionary;
        public DictionaryObservableDecorator(IDictionary<Tkey,TValue> dictionary)
        {
            _Dictionary = dictionary;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<Tkey, TValue> value)
        {
            _Dictionary.Add(value);
        }
    }

    public static  class DictionaryObservableExtender
    {
        public static IObserver<KeyValuePair<Tkey, TValue>> ToObserver<Tkey, TValue>(this IDictionary<Tkey, TValue> idictionary)
        {
            return new DictionaryObservableDecorator<Tkey, TValue>(idictionary);
        }
    }
}
