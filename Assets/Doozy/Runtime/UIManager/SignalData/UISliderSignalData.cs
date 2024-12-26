// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public struct UISliderSignalData
    {
        public string sliderCategory { get; private set; }
        public string sliderName { get; private set; }
        public SliderState sliderState { get; private set; }
        public UISlider slider { get; private set; }

        public UISliderSignalData(string sliderCategory, string sliderName, SliderState sliderState, UISlider slider = null)
        {
            this.sliderCategory = sliderCategory;
            this.sliderName = sliderName;
            this.sliderState = sliderState;
            this.slider = slider;
        }

        public override string ToString()
        {
            return $"({ObjectNames.NicifyVariableName(sliderState.ToString())}) {sliderCategory} / {sliderName}";
        }
    }
}
