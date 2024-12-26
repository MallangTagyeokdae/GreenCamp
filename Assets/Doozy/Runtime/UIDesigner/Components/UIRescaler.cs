// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.UIDesigner.Components
{
    /// <summary>
    /// Changes a RectTransform's localScale to match a target size from a given reference size
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class UIRescaler : UnityEngine.EventSystems.UIBehaviour
    {
        private RectTransform m_RectTransform;
        /// <summary> The RectTransform attached to the GameObject </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public RectTransform rectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();

        [SerializeField] private Vector2 ReferenceSize;
        /// <summary>
        /// The reference size of this RectTransform.
        /// This is the size that the RectTransform will have when the localScale is set to (1,1,1).
        /// </summary>
        public Vector2 referenceSize
        {
            get => ReferenceSize;
            set => ReferenceSize = value;
        }

        [SerializeField] private Vector2 TargetSize;
        /// <summary>
        /// The target size of this RectTransform.
        /// This is the value that is used to determine the localScale of the RectTransform.
        /// </summary>
        public Vector2 targetSize
        {
            get => TargetSize;
            set => TargetSize = value;
        }

        [SerializeField] private bool ContinuousUpdate;
        /// <summary> If TRUE, the RectTransform localScale will be updated every frame in the LateUpdate method </summary>
        public bool continuousUpdate
        {
            get => ContinuousUpdate;
            set => ContinuousUpdate = value;
        }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            ReferenceSize = rectTransform.rect.size;
            TargetSize = ReferenceSize;

            UpdateScale();
        }

        protected override void OnValidate()
        {
            if (IsActive())
            {
                UpdateScale();
            }

            base.OnValidate();
        }
        #endif //UNITY_EDITOR

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateScale();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateScale();
        }

        private void LateUpdate()
        {
            if (!ContinuousUpdate) return;
            UpdateScale();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Updates the scale of the RectTransform based on the reference size and the target size
        /// </summary>
        public void UpdateScale()
        {
            Vector2 scale = rectTransform.localScale;
            //fix for the case when the reference size is 0
            if (ReferenceSize.x <= 0) ReferenceSize.x = 1;
            if (ReferenceSize.y <= 0) ReferenceSize.y = 1;
            //fix for the case when the target size is 0
            if (TargetSize.x < 0) TargetSize.x = 0;
            if (TargetSize.y < 0) TargetSize.y = 0;
            //calculate the scale based on the reference size and the target size
            scale.x = TargetSize.x / ReferenceSize.x;
            scale.y = TargetSize.y / ReferenceSize.y;
            //NaN check
            if (float.IsNaN(scale.x)) scale.x = 1;
            if (float.IsNaN(scale.y)) scale.y = 1;
            //less than 0 check
            if (scale.x < 0) scale.x = 0;
            if (scale.y < 0) scale.y = 0;
            //update the RectTransform scale
            rectTransform.localScale = scale;
        }
    }
}
