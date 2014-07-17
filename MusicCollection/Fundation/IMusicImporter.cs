using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;

using MusicCollection.Infra;
using System.Threading.Tasks;


namespace MusicCollection.Fundation
{

    public interface IMusicImporter : IImporterEvent
    {
        void Load();

        Task LoadAsync(ThreadProperties tp = null);
    }

}
