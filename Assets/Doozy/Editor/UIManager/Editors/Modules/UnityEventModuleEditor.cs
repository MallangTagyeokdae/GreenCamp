// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody;
using Doozy.Editor.Mody.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Modules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Modules
{
    [CustomEditor(typeof(UnityEventModule), true)]
    public sealed class UnityEventModuleEditor : ModyModuleEditor<UnityEventModule>
    {
        private SerializedProperty propertyEvent { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyEvent = serializedObject.FindProperty(nameof(UnityEventModule.Event));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText(UnityEventModule.k_DefaultModuleName)
                .SetSecondaryIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Modules.UnityEventModule.html")
                .AddYouTubeButton();
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();
            
            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                var actionsDrawer =
                    new ModyActionsDrawer();

                actionsDrawer.schedule.Execute(() => actionsDrawer.Update());

                VisualElement actionsContainer =
                    new VisualElement().SetName("Actions Container");

                void AddActionToDrawer(ModyActionsDrawerItem item)
                {
                    actionsDrawer.AddItem(item);
                    actionsContainer.AddChild(item.animatedContainer);
                }

                //MODULE ACTIONS
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(UnityEventModule.InvokeEvent))));
               
                settingsAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Called when invoked", propertyEvent))
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(actionsDrawer)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(actionsContainer)
                    .Bind(serializedObject);
            });
        }
    }
}
