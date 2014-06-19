using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MusicCollection.ToolBox.Collection
{
     [DebuggerDisplay("Count = {Count}")]
    internal class PolyMorphDictionary<TKey, TValue> : PolyMorphDictionaryGeneric<TKey, TValue>, IDictionary<TKey, TValue> 
         where TKey : IComparable<TKey>
    {

        public override void Add(TKey key, TValue value)
        {
            switch (Count)
            {
                case 0:
                    _Implementation = new SingleDictionary<TKey, TValue>(key, value);
                    return;

                case 1:
                    //_Implementation = new ListDictionary<TKey, TValue>(_Implementation);
                    _Implementation = new SortedList<TKey, TValue>(_Implementation);
                    break;

                case TransitionToDictionary:
                    _Implementation = new Dictionary<TKey, TValue>(_Implementation);
                    break;
            }

            _Implementation.Add(key, value);
        }

        public override bool Remove(TKey key)
        {
            switch (Count)
            {
                case 0:
                    return false;

                case 1:
                    if (_Implementation.ContainsKey(key))
                    {
                        _Implementation = null;
                        return true;
                    }

                    return false;

                case TransitionToDictionary + 1:
                    if (_Implementation.ContainsKey(key))
                        _Implementation = new SortedList<TKey, TValue>(_Implementation);
                    else
                        return false;

                    break;

            }

            return _Implementation.Remove(key);
        }
    }
}
