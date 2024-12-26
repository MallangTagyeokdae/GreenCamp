// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public struct UIStepperSignalData
    {
        public string stepperCategory { get; private set; }
        public string stepperName { get; private set; }
        public StepperState stepperState { get; private set; }
        public UIStepper stepper { get; private set; }

        public UIStepperSignalData(string stepperCategory, string stepperName, StepperState stepperState, UIStepper stepper = null)
        {
            this.stepperCategory = stepperCategory;
            this.stepperName = stepperName;
            this.stepperState = stepperState;
            this.stepper = stepper;
        }

        public override string ToString()
        {
            return $"({ObjectNames.NicifyVariableName(stepperState.ToString())}) {stepperCategory} / {stepperName}";
        }
    }
}
