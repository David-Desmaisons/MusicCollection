using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using MusicCollection.Fundation;

namespace MusicCollection.DataExchange
{
    public interface IFileRelatedEntity
    {
        IFullAlbumDescriptor Album { set; }

        string Path { get; set; }
    }

    static internal class IFREExtender
    {
        public static string Reconnect(this IFileRelatedEntity ifre, string current, string original)
        {
            if (!ifre.Path.StartsWith(original))
                return null;

            string Rel = ifre.Path.Substring(original.Length);

            return System.IO.Path.Combine(current, Rel);
        }

        public static void UpdatePath(this IFileRelatedEntity ifre, string idir, string original)
        {
            ifre.Path = ifre.Reconnect(idir, original);
        }
    }

    public class FileRelatedEntity : IFileRelatedEntity
    {
        public FileRelatedEntity()
        {
        }

        public FileRelatedEntity(string iPath)
        {
            Path = iPath;
        }

        private IFullAlbumDescriptor _AD;
        [XmlIgnore]
        public IFullAlbumDescriptor Album
        {
            set { if (_AD == null) _AD = value; }
            get { return _AD; }
        }

        public string Path
        {
            get;
            set;
        }
    }
}
