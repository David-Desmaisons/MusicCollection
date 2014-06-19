using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    [Serializable]
    public enum CompleteFileBehaviour
    {
        [Description("Delete File")]
        Delete,

        [Description("No Action")]
        DoNothing,
    };

    [Serializable]
    public enum ConvertFileBehaviour
    {
        [Description("Music Collection Folder")]
        CopyInMananagedFolder,

        [Description("Original Folder")]
        SameFolder
    };

    [Serializable]
    public enum PartialFileBehaviour
    {
        [Description("Delete")]
        Delete,

        [Description("No Action")]
        DoNothing
    };

    [Serializable]
    public enum BasicBehaviour
    {
        [Description("Always")]
        Yes,

        [Description("Ask")]
        AskEndUser,

        [Description("Never")]
        No
    };
}
