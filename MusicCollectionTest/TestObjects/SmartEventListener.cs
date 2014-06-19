using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollectionTest.TestObjects
{
    internal class SmartEventListener
    {
        private EventArgs _Expected;
        private Delegate _OnEvent;
        private Nullable<bool> res = null;
        private EventArgs _Last=null;


        internal SmartEventListener()
        {
        }

        internal void SetExpectation<T>(T expected,Action<T> OnEvent=null) where T:EventArgs
        {
            _Expected = expected;
            _OnEvent = OnEvent;
            _Last = null;
            res = null;
        }

        internal void NoExpectation()
        {
            res = null;
            _Last = null;
        }

        internal void Listener(object sender, EventArgs eventa)
        {
            _Last = eventa;
            res = eventa.Equals(_Expected);
            if ((res==true) && (_OnEvent != null))
            {
                _OnEvent.DynamicInvoke(new object[] { eventa });
            }
            _Expected = null;
            _OnEvent = null;
        }

        internal bool IsOk
        {
            get { return res == true; }
        }

        internal EventArgs LastReceived
        {
            get { return _Last; }
        }

        internal bool IsWaiting
        {
            get { return res == null; }
        }



    }
}
