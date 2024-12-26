// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody;
using Doozy.Runtime.UIManager.Triggers;
using UnityEditor;
using UnityEditor.UIElements;

namespace Doozy.Editor.UIManager.Editors.Triggers
{
    [CustomEditor(typeof(UIDeselectedTrigger))]
    public class UIDeselectedTriggerEditor : ModyTriggerEditor<UIDeselectedTrigger>
    {
        private SerializedProperty propertyOnTrigger { get; set; }
        
        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnTrigger = serializedObject.FindProperty(nameof(UIDeselectedTrigger.OnTrigger));
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetSecondaryIcon(EditorSpriteSheets.EditorUI.Icons.Selectable)
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Triggers.UIDeselectedTrigger.html")
                .AddYouTubeButton();
            
            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if(tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks()
                {
                    if (castedTarget == null)
                        return false;
                    
                    return castedTarget.OnTrigger.GetPersistentEventCount() > 0;
                }
                
                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);
                
                //subsequent indicators state update (with animation)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);

                }).Every(200);

            });
        }
        
        protected override void InitializeCallbacks()
        {
            base.InitializeCallbacks();

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Called when a Selectable is deselected", propertyOnTrigger))
                    )
                    .Bind(serializedObject);
            });
        }
    }
}
