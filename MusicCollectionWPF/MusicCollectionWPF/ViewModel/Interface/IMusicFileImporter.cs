using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModel.Interface
{
    [ServiceContract]
    public interface IMusicFileImporter
    {
        [OperationContract]
        Task ImportCompactedFileAsync(string iPath);
    }
}
