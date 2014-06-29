using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicCollectionWPF.ViewModelHelper
{
    /// <summary>
    /// Main interface to use Ioc in a MVVM pattern
    /// Provide way to create-manipulate windows from ViewModel
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Opens the window and returns only when the newly opened window is closed.
        /// </summary>   
        /// <param name="AddEffect">true if an effect should be displayed in father window</param>
        /// <returns> A Nullable<T> value of type Boolean that specifies
        /// whether the activity was accepted (true) or canceled (false). 
        /// The return value is the value of the DialogResult property before a window closes.
        /// </returns>
        bool? ShowDialog(bool AddEffect = true);


        /// <summary>
        /// Opens the window and returns immediatly.
        /// </summary>   
        void Show();

        /// <summary>
        /// Show a message to end user and wait for an answer
        /// </summary>   
        /// <param name="iMessage">Message to be displayed.</param>
        /// <param name="iTitle">The title of the window.</param>
        /// <returns> 
        /// true if end users agrees to the question
        /// </returns>
        bool ShowConfirmationMessage(string iMessage, string iTitle);

        /// <summary>
        /// Show a message to end user
        /// </summary>   
        /// <param name="iMessage">Message to be displayed.</param>
        /// <param name="iTitle">The title of the window.</param>
        /// <param name="iBlocking">true if the window is modal.</param>
        void ShowMessage(string iMessage, string iTitle,bool iBlocking);

        /// <summary>
        /// Close the window
        /// </summary>
        void Close();

        /// <summary>
        /// Create a new window based on ModelViewBase
        /// </summary>        
        /// <param name="iModelViewBase">The view model.</param>
        /// <returns>the newly created window.</returns>
        IWindow CreateFromViewModel(ViewModelBase iModelViewBase);

        /// <summary>
        /// Get or set the ModelView of the window
        /// </summary>
        ViewModelBase ModelView { get; set;  }


        /// <summary>
        /// Get or set the logical owner of the window
        /// </summary>
        IWindow LogicOwner { get; set; }

        /// <summary>
        /// Choose an existing file with potential help of the user
        /// </summary>
        /// <param name="iTitle">The title of the window.</param>
        /// <param name="Extension">Extension of the file.</param>
        /// <returns>the path of the file. Null if nothing is choosed.</returns>
        string ChooseFile(string iTitle, string Extension);

         /// <summary>
        /// Choose an existing file with potential help of the user
        /// </summary>
        /// <param name="iTitle">The title of the window.</param>
        /// <param name="Extension">Extension of the file.</param>
        /// <returns>the collections of path file. Empty if nothing is choosed.</returns>
        IEnumerable<string> ChooseFiles(string iTitle, string Extension);

        /// <summary>
        /// Get or set the show in task behaviour
        /// </summary>
        bool ShowInTaskbar {get;set;}

        /// <summary>
        /// Get or set the clocation position of the window
        /// </summary>
        bool CenterScreenLocation {get;set;}

        /// <summary>
        ///  Occurs when the window is laid out, rendered, and ready for interaction.
        /// </summary>
        event RoutedEventHandler Loaded;

        /// <summary>
        ///  Occurs directly after Close is called, and can be handled to cancel window closure.
        /// </summary>       
        event CancelEventHandler Closing;

    }
}
