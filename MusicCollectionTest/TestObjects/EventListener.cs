using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.Infra;

namespace MusicCollectionTest.TestObjects
{
    internal class EventListener<T> where T :EventArgs
    {
        private Queue<T> _LastEvent=new Queue<T>();
        //private int _EvtNumber = 0;

        internal void SingleElementChangedListener(object sender, T e)
        {
            _LastEvent.Enqueue(e);
           // _EvtNumber++;
        }

        internal int EventCount
        {
            get { return _LastEvent.Count; }
        }

        public void Clear()
        {
            _LastEvent.Clear();
        }

        internal T GetDeplieEvent()
        {
            //get
            //{
                if (_LastEvent.Count == 0)
                    return null;
                
                
                T newev = _LastEvent.Dequeue();
               // _EvtNumber = 0;
                _LastEvent.Clear();
                //_LastEvent = null;
                return newev;
            //}

        }

        internal IEnumerable<T> GetDeplieEvents()
        {
            //get
            //{
                var res = _LastEvent.ToList();
                _LastEvent.Clear();
                return res;
            //}

        }
    }
}
