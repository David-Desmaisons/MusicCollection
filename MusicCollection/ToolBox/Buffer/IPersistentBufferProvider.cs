using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Buffer
{
    internal interface IPersistentBufferProvider : IBufferProvider
    {
        bool Save(string FileName);

        bool IsPersistent { get; set; }
    }
}
