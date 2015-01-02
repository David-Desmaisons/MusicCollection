using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IAsyncLoad
    {
        Task<bool> LoadAsync(CancellationToken iCancellationRequestToken);
    }
}
