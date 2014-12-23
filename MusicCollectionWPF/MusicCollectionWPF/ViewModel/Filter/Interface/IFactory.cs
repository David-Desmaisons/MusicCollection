using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModel
{
    public interface IFactory
    {
        object Create(string iname);
    }

    public class FactoryBuilder
    {
        private class FuncFactory : IFactory
        {
            private Func<string, object> _Fact;
            internal FuncFactory(Func<string, object> iFact)
            {
                _Fact = iFact;
            }

            public object Create(string iname)
            {
                return _Fact(iname);
            }
        }

        public static IFactory Instanciate(Func<string, object> iFunc)
        {
            return new FuncFactory(iFunc);
        }
    }
}
