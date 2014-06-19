using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra.File
{
    /// <summary>
    /// Interface to interact with file systems
    /// </summary>
    public interface IFileTools
    {
        /// <summary>
        /// Create file name corresponding to a new file.
        /// Change name if a file with same name exists.
        /// </summary>   
        /// <param name="targetdir">target directory.</param>
        /// <param name="TargetName">target file name.</param>
        /// <param name="Ext">file extension.</param>
        /// <param name="Rename">if true api will create a new file name if not only check that file does not exist.</param>
        /// <returns> new name, null if failed in case of rename is false</returns>
        string CreateNewAvailableName(string targetdir, string TargetName, string Ext, bool Rename = true);

        /// <summary>
        /// Get the current music folder
        /// </summary>  
        /// <returns> music folder path</returns>
        string MusicFolder { get; }

        /// <summary>
        /// Get the current document folder
        /// </summary>  
        /// <returns> document folder path</returns>
        string DocumentFolder { get; }

        /// <summary>
        /// Get the current document folder
        /// </summary>  
        /// <returns> document folder path</returns>
        string KeysFileExtesion { get; }

        /// <summary>
        /// Compute file filter base on file extension and file description
        /// </summary>  
        /// <param name="iExtension">File extension.</param>
        /// <param name="iDescription">File description.</param>
        /// <returns> the corresponding filter</returns>
        string GetFileFilter(string iExtension, string iDescription);

        /// <summary>
        /// Compute if a file exist
        /// </summary>  
        /// <param name="iPath">File path.</param>
        /// <returns> true if file exists</returns>
        bool FileExists(string iPath);

        /// <summary>
        /// Compute if a Directory exist
        /// </summary>  
        /// <param name="iPath">Directory path.</param>
        /// <returns> true if Directory exists</returns>
        bool DirectoryExists(string iPath);

    }
        
        
}
