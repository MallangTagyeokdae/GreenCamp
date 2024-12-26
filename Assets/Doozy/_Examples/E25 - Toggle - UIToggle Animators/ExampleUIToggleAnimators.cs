using System.Globalization;
using TMPro;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Doozy._Examples.E25___Toggle___UIToggle_Animators
{
    public class ExampleUIToggleAnimators : MonoBehaviour
    {
        [Header("Float Animator")]
        public float FloatValue;
        public TextMeshProUGUI FloatLabel;

        [Header("Int Animator")]
        public int IntValue;
        public TextMeshProUGUI IntLabel;

        [Header("Vector2 Animator")]
        public Vector2 Vector2Value;
        public TextMeshProUGUI Vector2Label;

        [Header("Vector3 Animator")]
        public Vector3 Vector3Value;
        public TextMeshProUGUI Vector3Label;

        private bool hasFloatLabel { get; set; }
        private bool hasIntLabel { get; set; }
        private bool hasVector2Label { get; set; }
        private bool hasVector3Label { get; set; }

        private void OnEnable()
        {
            hasFloatLabel = FloatLabel != null;
            hasIntLabel = IntLabel != null;
            hasVector2Label = Vector2Label != null;
            hasVector3Label = Vector3Label != null;
        }

        private void LateUpdate()
        {
            if (hasFloatLabel) FloatLabel.text = FloatValue.ToString(CultureInfo.InvariantCulture);
            if (hasIntLabel) IntLabel.text = IntValue.ToString();
            if (hasVector2Label) Vector2Label.text = Vector2Value.ToString();
            if (hasVector3Label) Vector3Label.text = Vector3Value.ToString();
        }
    }
}
