﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.Library.Backend.Strings {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class WEBDAV {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal WEBDAV() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.Library.Backend.Strings.WEBDAV", typeof(WEBDAV).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supports connections to a WEBDAV enabled web server, using the HTTP protocol. Allowed formats are &quot;webdav://hostname/folder&quot; or &quot;webdav://username:password@hostname/folder&quot;..
        /// </summary>
        internal static string Description {
            get {
                return ResourceManager.GetString("Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The password used to connect to the server. This may also be supplied as the environment variable &quot;AUTH_PASSWORD&quot;..
        /// </summary>
        internal static string DescriptionAuthPasswordLong {
            get {
                return ResourceManager.GetString("DescriptionAuthPasswordLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supplies the password used to connect to the server.
        /// </summary>
        internal static string DescriptionAuthPasswordShort {
            get {
                return ResourceManager.GetString("DescriptionAuthPasswordShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The username used to connect to the server. This may also be supplied as the environment variable &quot;AUTH_USERNAME&quot;..
        /// </summary>
        internal static string DescriptionAuthUsernameLong {
            get {
                return ResourceManager.GetString("DescriptionAuthUsernameLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supplies the username used to connect to the server.
        /// </summary>
        internal static string DescriptionAuthUsernameShort {
            get {
                return ResourceManager.GetString("DescriptionAuthUsernameShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To aid in debugging issues, it is possible to set a path to a file that will be overwritten with the PROPFIND response.
        /// </summary>
        internal static string DescriptionDebugPropfindLong {
            get {
                return ResourceManager.GetString("DescriptionDebugPropfindLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dump the PROPFIND response.
        /// </summary>
        internal static string DescriptionDebugPropfindShort {
            get {
                return ResourceManager.GetString("DescriptionDebugPropfindShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Using the HTTP Digest authentication method allows the user to authenticate with the server, without sending the password in clear. However, a man-in-the-middle attack is easy, because the HTTP protocol specifies a fallback to Basic authentication, which will make the client send the password to the attacker. Using this flag, the client does not accept this, and always uses Digest authentication or fails to connect..
        /// </summary>
        internal static string DescriptionForceDigestLong {
            get {
                return ResourceManager.GetString("DescriptionForceDigestLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Force the use of the HTTP Digest authentication method.
        /// </summary>
        internal static string DescriptionForceDigestShort {
            get {
                return ResourceManager.GetString("DescriptionForceDigestShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If the server and client both supports integrated authentication, this option enables that authentication method. This is likely only available with windows servers and clients..
        /// </summary>
        internal static string DescriptionIntegratedAuthenticationLong {
            get {
                return ResourceManager.GetString("DescriptionIntegratedAuthenticationLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use windows integrated authentication to connect to the server.
        /// </summary>
        internal static string DescriptionIntegratedAuthenticationShort {
            get {
                return ResourceManager.GetString("DescriptionIntegratedAuthenticationShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use this flag to communicate using Secure Socket Layer (SSL) over http (https)..
        /// </summary>
        internal static string DescriptionUseSSLLong {
            get {
                return ResourceManager.GetString("DescriptionUseSSLLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Instructs Duplicati to use an SSL (https) connection.
        /// </summary>
        internal static string DescriptionUseSSLShort {
            get {
                return ResourceManager.GetString("DescriptionUseSSLShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WebDAV.
        /// </summary>
        internal static string DisplayName {
            get {
                return ResourceManager.GetString("DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The folder {0} was not found, message: {1}.
        /// </summary>
        internal static string MissingFolderError {
            get {
                return ResourceManager.GetString("MissingFolderError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When listing the folder {0} the file {1} was listed, but the server now reports that the file is not found.
        ///This can be because the file is deleted or unavailable, but it can also be because the file extension {2} is blocked by the web server. IIS blocks unknown extensions by default.
        ///Error message: {3}.
        /// </summary>
        internal static string SeenThenNotFoundError {
            get {
                return ResourceManager.GetString("SeenThenNotFoundError", resourceCulture);
            }
        }
    }
}
