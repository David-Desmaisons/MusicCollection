using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IMusicObject : IObjectAttribute, IObjectState
    {
        string Name { get; }
    }
}
