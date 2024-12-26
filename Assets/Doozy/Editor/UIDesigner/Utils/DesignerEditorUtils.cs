// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIDesigner;
using Doozy.Runtime.UIDesigner.Utils;
using UnityEditor;
using UnityEngine;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;
using Space = Doozy.Runtime.UIDesigner.Space;
// ReSharper disable CoVariantArrayConversion

namespace Doozy.Editor.UIDesigner.Utils
{
    public static class DesignerEditorUtils
    {
        private static HashSet<RectTransform> selectedRectTransforms { get; } = new HashSet<RectTransform>();
        public static HashSet<RectTransform> selected
        {
            get
            {
                selectedRectTransforms.Clear();
                foreach (Object o in Selection.objects)
                {
                    if (o == null) continue;
                    if (o is GameObject go)
                        selectedRectTransforms.Add(go.GetComponent<RectTransform>());
                }
                selectedRectTransforms.Remove(null);
                return selectedRectTransforms;
            }
        }

        #region Align

        public static void Align(AlignTo alignTo, Align align, AlignMode alignMode, bool updateAnchors, RectTransform keyObject = null)
        {
            RectTransform[] targets = selected.ToArray();
            RecordUndo(targets, $"{nameof(Align)} {ObjectNames.NicifyVariableName(align.ToString())}");
            DesignerUtils.Align(alignTo, align, alignMode, updateAnchors, keyObject, targets);
        }

        public static void Distribute(AlignTo alignTo, Distribute distribute, RectTransform keyObject = null, float spacing = 0f)
        {
            RectTransform[] targets = selected.ToArray();
            RecordUndo(targets, $"{nameof(Distribute)} {ObjectNames.NicifyVariableName(distribute.ToString())}");
            DesignerUtils.Distribute(alignTo, distribute, keyObject, spacing, targets);
        }

        public static void HorizontalDistributeSpacing(AlignTo alignTo, RectTransform keyObject, float spacing)
        {
            RectTransform[] targets = selected.ToArray();
            RecordUndo(targets, $"{ObjectNames.NicifyVariableName(nameof(HorizontalDistributeSpacing))}");
            DesignerUtils.DistributeHorizontalWithSpacing(alignTo, keyObject, spacing, targets);
        }

        public static void VerticalDistributeSpacing(AlignTo alignTo, RectTransform keyObject, float spacing)
        {
            RectTransform[] targets = selected.ToArray();
            RecordUndo(targets, $"{ObjectNames.NicifyVariableName(nameof(VerticalDistributeSpacing))}");
            DesignerUtils.DistributeVerticalWithSpacing(alignTo, keyObject, spacing, targets);
        }

        #endregion

        #region Rotation

        public static void StartRotation()
        {
            RectTransform[] targets = selected.ToArray();
            if (targets.Length == 0) return;
            RecordUndo(targets, "Rotate");
            DesignerUtils.StartRotationChange(targets);
        }

        public static void EndRotation()
        {
            DesignerUtils.StopRotationChange();
            SetDirty(selected);
        }

        public static void UpdateRotationXY(Space space, float x, float y, bool relativeChange)
        {
            DesignerUtils.UpdateRotationXY(space, x, y, relativeChange);
            SetDirty(selected);
        }

        public static void UpdateRotationXZ(Space space, float x, float z, bool relativeChange)
        {
            DesignerUtils.UpdateRotationXZ(space, x, z, relativeChange);
            SetDirty(selected);
        }

        public static void UpdateRotationYZ(Space space, float y, float z, bool relativeChange)
        {
            DesignerUtils.UpdateRotationYZ(space, y, z, relativeChange);
            SetDirty(selected);
        }

        public static void UpdateRotation(Space space, float x, float y, float z, bool relativeChange)
        {
            DesignerUtils.UpdateRotation(space, x, y, z, relativeChange);
            SetDirty(selected);
        }

        public static void UpdateRotation(Space space, Axis axis, float value, bool relativeChange)
        {
            DesignerUtils.UpdateRotation(space, axis, value, relativeChange);
            SetDirty(selected);
        }

        #endregion

        #region Scale

        public static void StartScale()
        {
            RectTransform[] targets = selected.ToArray();
            if (targets.Length == 0) return;
            RecordUndo(targets, "Scale");
            DesignerUtils.StartScaleChange(targets);
        }

        public static void EndScale()
        {
            DesignerUtils.StopScaleChange();
            SetDirty(selected);
        }

        public static void UpdateScaleX(float x, bool relativeChange)
        {
            DesignerUtils.UpdateScaleX(x, relativeChange);
            SetDirty(selected);
        }

        public static void UpdateScaleY(float y, bool relativeChange)
        {
            DesignerUtils.UpdateScaleY(y, relativeChange);
            SetDirty(selected);
        }

        public static void UpdateScaleXY(float x, float y, bool relativeChange)
        {
            DesignerUtils.UpdateScaleXY(x, y, relativeChange);
            SetDirty(selected);
        }

        #endregion

        #region Size
        
        public static void StartSize()
        {
            RectTransform[] targets = selected.ToArray();
            if (targets.Length == 0) return;
            RecordUndo(targets, "Size");
            DesignerUtils.StartSizeChange(targets);
        }
        
        public static void EndSize()
        {
            DesignerUtils.StopSizeChange();
            SetDirty(selected);
        }
        
        public static void UpdateSizeX(float x, bool relativeChange)
        {
            DesignerUtils.UpdateSizeX(x, relativeChange);
            SetDirty(selected);
        }
        
        public static void UpdateSizeY(float y, bool relativeChange)
        {
            DesignerUtils.UpdateSizeY(y, relativeChange);
            SetDirty(selected);
        }
        
        public static void UpdateSizeXY(float x, float y, bool relativeChange)
        {
            DesignerUtils.UpdateSizeXY(x, y, relativeChange);
            SetDirty(selected);
        }

        #endregion

        private static void SetDirty(IEnumerable<RectTransform> targets)
        {
            foreach (RectTransform target in targets)
                EditorUtility.SetDirty(target);
        }

        private static void RecordUndo(RectTransform[] targets, string message)
        {
            switch (targets.Length)
            {
                case 0:
                    return;
                case 1:
                    Undo.RecordObject(targets[0], message);
                    EditorUtility.SetDirty(targets[0]);
                    return;
                default:
                {
                    Undo.RecordObjects(targets, message);
                    foreach (RectTransform target in targets)
                        EditorUtility.SetDirty(target);
                    break;
                }
            }
        }
    }
}
