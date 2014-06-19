using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModelHelper
{
    public interface IViewModelBinder
    {
        IWindow Solve(ViewModelBase ivm);
    }
}
