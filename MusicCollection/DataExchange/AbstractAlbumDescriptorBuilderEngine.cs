using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollection.DataExchange
{
    internal abstract class AbstractAlbumDescriptorBuilderEngine:IAlbumDescriptorBuilderEngine
    {
        protected abstract bool PrivateVisit(IAlbumDescriptorBuilder iab);
        
        public AlbumDescriptor  Visit(IAlbumDescriptorBuilder iBuilder)
        {
            bool res = PrivateVisit(iBuilder);
            if (res == false)
                return null;

            return iBuilder.Mature();
        }
    }
}
