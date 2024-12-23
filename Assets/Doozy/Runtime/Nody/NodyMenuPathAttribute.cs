// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Runtime.Nody
{
    /// <summary> Attribute used to add nodes to the nodes menu inside the Nody window </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NodyMenuPathAttribute : Attribute
    {
        /// <summary> Node category </summary>
        public string category { get; }
        /// <summary> Node name </summary>
        public string name { get; }
        
        /// <summary> Construct a new NodyMenuPathAttribute with the given node category and node name </summary>
        /// <param name="category"> Node category </param>
        /// <param name="name"> Node name </param>
        public NodyMenuPathAttribute(string category, string name)
        {
            this.category = category;
            this.name = name;
        }
    }
}
