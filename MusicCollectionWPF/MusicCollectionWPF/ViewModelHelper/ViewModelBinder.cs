using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModelHelper
{
    public class ViewModelBinder : IViewModelBinder
    {
        //From ModelViewBase to IWindow
        private IDictionary<Type, Type> _Mapper = new Dictionary<Type, Type>();

        public ViewModelBinder(Assembly iassembly)
        {
            var markedwindows = iassembly.GetMarkedType<ViewModelBindingAttribute>();
            markedwindows.Apply(t => _Mapper.Add(t.Item1.ViewModelBaseType, t.Item2));
            CheckCoherence();
        }

        private void CheckCoherence()
        {
            foreach (var kp in _Mapper)
            {
                if (!kp.Key.GetBaseTypes().Any(t=> t == typeof(ViewModelBase)))
                    throw new ArgumentException("ViewModelBinding  ViewModelBaseType should inherits from ViewModelBase");

                if (!kp.Value.GetInterfaces().Cast<Type>().Contains(typeof(IWindow)))
                    throw new ArgumentException("an element with a ViewModelBinding attribute should be an IWindow");
            }
        }

        private Type SolveType(Type iType)
        {
            Type result = null;
            foreach (Type InType in iType.GetBaseTypes())
            {
                if (_Mapper.TryGetValue(InType, out result))
                    return result;
            }
            return null;
        }

        public IWindow Solve(ViewModelBase ivm)
        {
            if (ivm == null)
                throw new ArgumentNullException();

            Type result = SolveType(ivm.GetType());

            if (result == null)
                return null;
         
            return Activator.CreateInstance(result) as IWindow;
        }
    }
}
