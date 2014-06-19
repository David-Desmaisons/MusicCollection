using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IMusicGenreFactory
    {
        IGenre Create(string GenreName);

        IGenre CreateDummy();
    }
}
