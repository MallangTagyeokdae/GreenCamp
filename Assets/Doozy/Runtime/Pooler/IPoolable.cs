// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Pooler
{
    /// <summary> Interface for an object that can be pooled </summary>
    public interface IPoolable : IDisposable
    {
        /// <summary> Check if is in the pool or not </summary>
        bool inPool { get; set; }
        /// <summary> Reset to default values or settings </summary>
        void Reset();
        /// <summary> Return to the pool </summary>
        /// <param name="debug"> Print relevant messages to the console </param>
        void Recycle(bool debug = false);
    }
}
