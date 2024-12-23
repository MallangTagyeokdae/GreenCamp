// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
namespace Doozy.Runtime.Nody.Nodes.Internal
{
    /// <summary>
    /// Base class for global nodes.
    /// <para/> A global node is a node that is always active.
    /// </summary>
    [Serializable]
    public abstract class GlobalNode : FlowNode
    {
        protected GlobalNode() : base(NodeType.Global) {}

        public override void OnExit() =>
            NodeState = NodeState.Running;
    }
}
