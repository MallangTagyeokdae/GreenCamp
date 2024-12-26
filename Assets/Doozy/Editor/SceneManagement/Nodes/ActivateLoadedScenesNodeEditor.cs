// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.SceneManagement.Nodes;
using UnityEditor;
using UnityEngine;
namespace Doozy.Editor.SceneManagement.Nodes
{
    [CustomEditor(typeof(ActivateLoadedScenesNode))]
    public class ActivateLoadedScenesNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.SceneManagement.Icons.ActivateLoadedScenesNode;

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(ActivateLoadedScenesNode)))
                .SetIcon(EditorSpriteSheets.SceneManagement.Icons.ActivateLoadedScenesNode)
                .SetAccentColor(EditorColors.SceneManagement.Component)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton()
                ;
        }
    }
}
