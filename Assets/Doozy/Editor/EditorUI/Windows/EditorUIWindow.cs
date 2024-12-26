// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.ScriptableObjects.Fonts;
using Doozy.Editor.EditorUI.ScriptableObjects.Layouts;
using Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations;
using Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets;
using Doozy.Editor.EditorUI.ScriptableObjects.Styles;
using Doozy.Editor.EditorUI.ScriptableObjects.Textures;
using Doozy.Editor.EditorUI.Windows.Internal;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Windows
{
    public class EditorUIWindow : FluidWindow<EditorUIWindow>
    {
        public const string k_WindowTitle = "EditorUI";
        public const string k_WindowMenuPath = "Tools/Doozy/Refresh/";

        public static void Open() => InternalOpenWindow(k_WindowTitle);

        [MenuItem("Tools/Doozy/Refresh/EditorUI/Refresh All", priority = -450)]
        public static void Refresh()
        {
            if (EditorUtility.DisplayDialog
                (
                    $"Refresh the all the {k_WindowTitle} databases?",
                    "This will regenerate all the databases with the latest registered items, from the source files." +
                    "\n\n" +
                    "Takes a few minutes, depending on the number of source files and your computer's performance." +
                    "\n\n" +
                    "This operation cannot be undone!",
                    "Yes",
                    "No"
                )
               )
            {
                EditorDataColorDatabase.instance.RefreshDatabase();
                EditorDataFontDatabase.instance.RefreshDatabase();
                EditorDataLayoutDatabase.instance.RefreshDatabase();
                EditorDataMicroAnimationDatabase.instance.RefreshDatabase();
                EditorDataSelectableColorDatabase.instance.RefreshDatabase();
                EditorDataSpriteSheetDatabase.instance.RefreshDatabase();
                EditorDataStyleDatabase.instance.RefreshDatabase();
                EditorDataTextureDatabase.instance.RefreshDatabase();
            }
        }

        protected override void CreateGUI()
        {
            //REMOVED    
        }
    }
}
