// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Doozy.Runtime.Common
{
    [Serializable]
    public partial struct FormattedLabel
    {
        public TMP_Text Label;
        public string Format;

        public FormattedLabel(TMP_Text label = null, string format = "")
        {
            Label = label;
            Format = format;
        }

        public void SetText(DateTime value)
        {
            if (Label == null) return;
            Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
        }

        public void SetText(TimeSpan value)
        {
            if (Label == null) return;
            Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString() : value.ToString(Format));
        }

        public void SetText(string value)
        {
            if (Label == null) return;
            Label.SetText(string.IsNullOrEmpty(Format) ? value : string.Format(Format, value));
        }

        public void SetText(int value)
        {
            if (Label == null) return;
            Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
        }

        public void SetText(float value)
        {
            if (Label == null) return;
            Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
        }

        public void SetText(double value)
        {
            if (Label == null) return;
            Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
        }
    }
}
