using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal interface IInternalAlbum : IAlbum
    {
        void Visit(IAlbumVisitor Visitor);
    }
}
