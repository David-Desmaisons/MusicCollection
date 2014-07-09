using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModel
{
    public interface IInformationEditor
    {
        IAsyncCommiter GetCommiter();
    }
}
