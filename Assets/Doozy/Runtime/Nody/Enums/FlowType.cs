// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Describes the way a <see cref="FlowController"/> uses a flow </summary>
    public enum FlowType
    {
        /// <summary>
        /// Use the flow graph as direct reference scriptable object
        /// <para/> Global graph (use the original flow graph)
        /// </summary>
        Global,
        
        /// <summary>
        /// Create a clone of the flow graph a scriptable object and use that
        /// <para/> Local graph (use a clone of the original flow graph)
        /// </summary>
        Local
    }
}
