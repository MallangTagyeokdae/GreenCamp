// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Nody;
using Doozy.Runtime.Nody;
using Doozy.Runtime.SceneManagement.Nodes;
using UnityEngine;
namespace Doozy.Editor.SceneManagement.Nodes
{
    public class ActivateLoadedScenesNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(ActivateLoadedScenesNode);
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.SceneManagement.Icons.ActivateLoadedScenesNode;
        public override Color nodeAccentColor => EditorColors.SceneManagement.Component;
        public override EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.SceneManagement.Component; 

        public ActivateLoadedScenesNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
        }
    }
}
