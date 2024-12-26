// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Triggers.Internal;

namespace Doozy.Runtime.UIManager.Triggers
{
    public class UISliderValueTrigger : BaseValueTrigger<UISlider>
    {
        protected override float value => Target.value;
    }
}
