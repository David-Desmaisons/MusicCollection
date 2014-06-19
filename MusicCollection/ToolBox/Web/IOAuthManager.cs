using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox.Web
{
    /// <summary>
    ///   A interface to manage OAuth interactions.  This works with
    ///   Twitter, not sure about other OAuth-enabled services.
    /// </summary>
    public interface IOAuthManager
    {
        /// <summary>
        ///   Generate a string to be used in an Authorization header in
        ///   an HTTP request.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method assembles the available oauth_ parameters that
        ///     have been set in the Dictionary in this instance, produces
        ///     the signature base (As described by the OAuth spec, RFC 5849),
        ///     signs it, then re-formats the oauth_ parameters into the
        ///     appropriate form, including the oauth_signature value, and
        ///     returns the result.
        ///   </para>
        ///   <para>
        ///     If you pass in a non-null, non-empty realm, this method will
        ///     include the realm='foo' clause in the Authorization header.
        ///   </para>
        /// </remarks>
        string GenerateAuthzHeader(string uri, string method);
    }
}
