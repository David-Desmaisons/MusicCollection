using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox.ZipTools
{
    /// <summary>
    /// Interface to compress-decompress file using zip algorithm
    /// </summary>
    public interface IZipper
    {
        /// <summary>
        /// Zipp a collection of string to a given file using a password.
        /// </summary>   
        /// <param name="iText">Collection of string to be compressed.</param>
        /// <param name="iFileName">The file path of the file to be created.</param>
        /// <param name="iPassword">Password to be used.</param>
        /// <returns> true if succees false otherwise</returns>
        Task<bool> ZippAsync(IEnumerable<string> iText, string iFileName, string iPassword);

        /// <summary>
        /// Zipp a collection of string to a given file using a password.
        /// </summary>   
        /// <param name="iText">Collection of string to be compressed.</param>
        /// <param name="iFileName">The file path of the file to be created.</param>
        /// <param name="iPassword">Password to be used.</param>
        /// <returns> true if succees false otherwise</returns>
        bool Zipp(IEnumerable<string> iText, string iFileName, string iPassword);

        /// <summary>
        /// UnZipp a file and return a collection of string corresponding to the stream.
        /// </summary>   
        /// <param name="iFileName">The input file path.</param>
        /// <param name="iPassword">Password to be used.</param>
        /// <returns> an collection of string corresponding to the file</returns>
        IEnumerable<string> UnZipp(string iFileName, string iPassword);

        /// <summary>
        /// Check the validity of a given file with a given password.
        /// </summary>   
        /// <param name="iFileName">The input file path.</param>
        /// <param name="iPassword">Password to be used.</param>
        /// <returns> true if the file can be decompresses false otherwise</returns>
        bool Check(string iFileName, string iPassword);
    }
}
