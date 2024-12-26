// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.ScriptableObjects.Internal;

namespace Doozy.Runtime.UIManager.ScriptableObjects
{
    [Serializable]
    public class UITooltipDatabase : PrefabLinkDatabase<UITooltipDatabase, UITooltipLink>
    {
        public override string defaultLinkName => UITooltip.k_DefaultTooltipName;
        public override string databaseName => nameof(UITooltip);
    }
}
