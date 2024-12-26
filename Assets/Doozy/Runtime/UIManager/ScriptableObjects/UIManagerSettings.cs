// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.ScriptableObjects;

namespace Doozy.Runtime.UIManager.ScriptableObjects
{
    public class UIManagerSettings : SingletonRuntimeScriptableObject<UIManagerSettings>
    {
        /// <summary> Global debug flag for the UIManager system </summary>
        public bool DebugMode;
    }
}