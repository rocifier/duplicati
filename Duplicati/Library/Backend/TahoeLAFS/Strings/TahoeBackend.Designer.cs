﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.17020
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Duplicati.Library.Backend.Strings {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TahoeBackend {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TahoeBackend() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Duplicati.Library.Backend.Strings.TahoeBackend", typeof(TahoeBackend).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string Displayname {
            get {
                return ResourceManager.GetString("Displayname", resourceCulture);
            }
        }
        
        internal static string DescriptionUseSSLShort {
            get {
                return ResourceManager.GetString("DescriptionUseSSLShort", resourceCulture);
            }
        }
        
        internal static string Description {
            get {
                return ResourceManager.GetString("Description", resourceCulture);
            }
        }
        
        internal static string UnexpectedJsonFragmentType {
            get {
                return ResourceManager.GetString("UnexpectedJsonFragmentType", resourceCulture);
            }
        }
        
        internal static string UriHasQueryError {
            get {
                return ResourceManager.GetString("UriHasQueryError", resourceCulture);
            }
        }
        
        internal static string MissingFolderError {
            get {
                return ResourceManager.GetString("MissingFolderError", resourceCulture);
            }
        }
        
        internal static string UnrecognizedUriError {
            get {
                return ResourceManager.GetString("UnrecognizedUriError", resourceCulture);
            }
        }
        
        internal static string DescriptionUseSSLLong {
            get {
                return ResourceManager.GetString("DescriptionUseSSLLong", resourceCulture);
            }
        }
    }
}
