// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Reactor
{
    /// <summary> Describes the types of references available when computing reaction's 'From' and/or 'To' values </summary>
    public enum ReferenceValue
    {
        /// <summary> Initial value (semi-relative value as it gets updated on start or on demand) </summary>
        StartValue = 0,
        /// <summary> Current value (relative value) </summary>
        CurrentValue = 1,
        /// <summary> Custom value (absolute value) </summary>
        CustomValue = 2
    }
}
