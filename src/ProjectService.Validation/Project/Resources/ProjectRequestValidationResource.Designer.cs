﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LT.DigitalOffice.ProjectService.Validation.Project.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ProjectRequestValidationResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ProjectRequestValidationResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LT.DigitalOffice.ProjectService.Validation.Project.Resources.ProjectRequestValida" +
                            "tionResource", typeof(ProjectRequestValidationResource).Assembly);
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
        ///   Looks up a localized string similar to User cannot be added to the project twice.
        /// </summary>
        internal static string AddUserTwice {
            get {
                return ResourceManager.GetString("AddUserTwice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project customer is too long.
        /// </summary>
        internal static string CustomerLong {
            get {
                return ResourceManager.GetString("CustomerLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Department id must not be empty.
        /// </summary>
        internal static string DepartmentIdIsEmpty {
            get {
                return ResourceManager.GetString("DepartmentIdIsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EndDateUtc can&apos;t be null if project is not active.
        /// </summary>
        internal static string EndDateUtcIsNull {
            get {
                return ResourceManager.GetString("EndDateUtcIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect project status.
        /// </summary>
        internal static string IncorrectStatus {
            get {
                return ResourceManager.GetString("IncorrectStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project name already exists.
        /// </summary>
        internal static string NameExists {
            get {
                return ResourceManager.GetString("NameExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project name is too long.
        /// </summary>
        internal static string NameLong {
            get {
                return ResourceManager.GetString("NameLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name can`t be null or empty.
        /// </summary>
        internal static string NameNotNullOrEmpty {
            get {
                return ResourceManager.GetString("NameNotNullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project short description is too long.
        /// </summary>
        internal static string ShortDescriptionLong {
            get {
                return ResourceManager.GetString("ShortDescriptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project short name already exists.
        /// </summary>
        internal static string ShortNameExists {
            get {
                return ResourceManager.GetString("ShortNameExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project short name is too long.
        /// </summary>
        internal static string ShortNameLong {
            get {
                return ResourceManager.GetString("ShortNameLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User id must not be empty.
        /// </summary>
        internal static string UserIdIsEmpty {
            get {
                return ResourceManager.GetString("UserIdIsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some users does not exist.
        /// </summary>
        internal static string UsersDoNotExist {
            get {
                return ResourceManager.GetString("UsersDoNotExist", resourceCulture);
            }
        }
    }
}
