// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Nody.Nodes.PortData
{
    /// <summary> <see cref="RandomNode"/> output port value </summary>
    [Serializable]
    public class RandomNodeOutputPortData
    {
        /// <summary> Default weight value </summary>
        public const int k_DefaultWeight = 100;
        
        /// <summary> Weight value </summary>
        public int Weight = k_DefaultWeight;
    }
}
