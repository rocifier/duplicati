﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.Library.Modules.Builtin.Strings {
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
    internal class ConsolePasswordInput {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConsolePasswordInput() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.Library.Modules.Builtin.Strings.ConsolePasswordInput", typeof(ConsolePasswordInput).Assembly);
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
        ///   Looks up a localized string similar to Confirm encryption passphrase.
        /// </summary>
        internal static string ConfirmPassphrasePrompt {
            get {
                return ResourceManager.GetString("ConfirmPassphrasePrompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This module will ask the user for a encryption password on the commandline unless encryption is disabled or the password is supplied by other means.
        /// </summary>
        internal static string Description {
            get {
                return ResourceManager.GetString("Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password prompt.
        /// </summary>
        internal static string Displayname {
            get {
                return ResourceManager.GetString("Displayname", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Empty passphrases are not allowed.
        /// </summary>
        internal static string EmptyPassphraseError {
            get {
                return ResourceManager.GetString("EmptyPassphraseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter encryption passphrase.
        /// </summary>
        internal static string EnterPassphrasePrompt {
            get {
                return ResourceManager.GetString("EnterPassphrasePrompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The passphrases do not match.
        /// </summary>
        internal static string PassphraseMismatchError {
            get {
                return ResourceManager.GetString("PassphraseMismatchError", resourceCulture);
            }
        }
    }
}
