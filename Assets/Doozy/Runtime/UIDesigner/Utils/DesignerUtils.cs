// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBePrivate.Local

namespace Doozy.Runtime.UIDesigner.Utils
{
    public static class DesignerUtils
    {
        #region Align

        #region Align

        public static void Align(AlignTo alignTo, Align align, AlignMode alignMode, bool updateAnchors, RectTransform keyObject = null, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;

            switch (alignTo)
            {
                case AlignTo.RootCanvas:
                    AlignToRootCanvas(align, alignMode, updateAnchors, rectTransforms);
                    break;
                case AlignTo.Parent:
                    AlignToParent(align, alignMode, updateAnchors, rectTransforms);
                    break;
                case AlignTo.Selection:
                    AlignToSelection(align, alignMode, updateAnchors, rectTransforms);
                    break;
                case AlignTo.KeyObject:
                    AlignToKeyObject(align, alignMode, updateAnchors, keyObject, rectTransforms);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignTo), alignTo, null);
            }
        }

        public static void AlignToRootCanvas(Align align, AlignMode alignMode, bool updateAnchors, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            RectTransform root = rectTransforms[0].root.GetComponent<RectTransform>();
            Rect rootRect = root.rect;
            float xMin = rootRect.xMin;
            float xMax = rootRect.xMax;
            float yMin = rootRect.yMin;
            float yMax = rootRect.yMax;
            float xCenter = rootRect.center.x;
            float yCenter = rootRect.center.y;

            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(root);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
                AlignRectTransform(align, alignMode, updateAnchors, r, xMin, xMax, yMin, yMax, xCenter, yCenter);
            }

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void AlignToParent(Align align, AlignMode alignMode, bool updateAnchors, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            foreach (RectTransform r in rectTransforms)
            {
                Transform transformParent = r.transform.parent;
                if (transformParent == null) continue;
                RectTransform parent = transformParent.GetComponent<RectTransform>();
                if (parent == null) continue;
                Rect parentRect = parent.rect;
                float xMin = parentRect.xMin;
                float xMax = parentRect.xMax;
                float yMin = parentRect.yMin;
                float yMax = parentRect.yMax;
                float xCenter = parentRect.center.x;
                float yCenter = parentRect.center.y;
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
                AlignRectTransform(align, alignMode, updateAnchors, r, xMin, xMax, yMin, yMax, xCenter, yCenter);
            }

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void AlignToSelection(Align align, AlignMode alignMode, bool updateAnchors, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            RectTransform root = rectTransforms[0].root.GetComponent<RectTransform>();

            float xMin = float.PositiveInfinity;
            float xMax = float.NegativeInfinity;
            float yMin = float.PositiveInfinity;
            float yMax = float.NegativeInfinity;

            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(root);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
                Rect designerRect = r.DesignerRect();
                xMin = Min(xMin, r.anchoredPosition.x - designerRect.width / 2f);
                xMax = Max(xMax, r.anchoredPosition.x + designerRect.width / 2f);
                yMin = Min(yMin, r.anchoredPosition.y - designerRect.height / 2f);
                yMax = Max(yMax, r.anchoredPosition.y + designerRect.height / 2f);
            }

            float xCenter = (xMin + xMax) / 2f;
            float yCenter = (yMin + yMax) / 2f;

            foreach (RectTransform r in rectTransforms)
                AlignRectTransform(align, alignMode, updateAnchors, r, xMin, xMax, yMin, yMax, xCenter, yCenter);

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void AlignToKeyObject(Align align, AlignMode alignMode, bool updateAnchors, RectTransform keyObject, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            if (keyObject == null) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            RectTransform root = keyObject.root.GetComponent<RectTransform>();
            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(root);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
            }

            Vector2 keyPosition = keyObject.localPosition;
            Rect keyRect = keyObject.DesignerRect();
            Vector2 keyPivot = keyObject.pivot;
            float xMin = keyPosition.x - keyRect.width * (1 - keyPivot.x);
            float xMax = keyPosition.x + keyRect.width * keyPivot.x;
            float yMin = keyPosition.y - keyRect.height * (1 - keyPivot.y);
            float yMax = keyPosition.y + keyRect.height * keyPivot.y;
            float xCenter = keyPosition.x;
            float yCenter = keyPosition.y;

            rectTransforms = rectTransforms.Where(r => r != keyObject).ToArray();

            foreach (RectTransform r in rectTransforms)
                AlignRectTransform(align, alignMode, updateAnchors, r, xMin, xMax, yMin, yMax, xCenter, yCenter);

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        private static void AlignRectTransform(Align align, AlignMode alignMode, bool updateAnchors, RectTransform rectTransform, float xMin, float xMax, float yMin, float yMax, float xCenter, float yCenter)
        {
            Vector2 result;

            Vector2 p = rectTransform.pivot;
            float pX = p.x;
            float pY = p.y;

            Vector3 lp = rectTransform.localPosition;
            float x = lp.x;
            float y = lp.y;

            Rect dRect = rectTransform.DesignerRect();

            switch (align)
            {
                case UIDesigner.Align.HorizontalLeft:
                    switch (alignMode)
                    {
                        case AlignMode.Inside:
                            result = new Vector2(xMin + dRect.width * pX, y);
                            break;
                        case AlignMode.Center:
                            result = new Vector2(xMin, y);
                            break;
                        case AlignMode.Outside:
                            result = new Vector2(xMin - dRect.width * pX, y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(alignMode), alignMode, null);
                    }
                    break;
                case UIDesigner.Align.HorizontalCenter:
                    result = new Vector2(xCenter - dRect.width * 0.5f + dRect.width * pX, y);
                    break;
                case UIDesigner.Align.HorizontalRight:
                    switch (alignMode)
                    {
                        case AlignMode.Inside:
                            result = new Vector2(xMax - dRect.width * (1 - pX), y);
                            break;
                        case AlignMode.Center:
                            result = new Vector2(xMax, y);
                            break;
                        case AlignMode.Outside:
                            result = new Vector2(xMax + dRect.width * (1 - pX), y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(alignMode), alignMode, null);
                    }
                    break;
                case UIDesigner.Align.VerticalTop:
                    switch (alignMode)
                    {
                        case AlignMode.Inside:
                            result = new Vector2(x, yMax - dRect.height * (1 - pY));
                            break;
                        case AlignMode.Center:
                            result = new Vector2(x, yMax);
                            break;
                        case AlignMode.Outside:
                            result = new Vector2(x, yMax + dRect.height * (1 - pY));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(alignMode), alignMode, null);
                    }
                    break;
                case UIDesigner.Align.VerticalCenter:
                    result = new Vector2(x, yCenter - dRect.height * 0.5f + dRect.height * pY);
                    break;
                case UIDesigner.Align.VerticalBottom:
                    switch (alignMode)
                    {
                        case AlignMode.Inside:
                            result = new Vector2(x, yMin + dRect.height * pY);
                            break;
                        case AlignMode.Center:
                            result = new Vector2(x, yMin);
                            break;
                        case AlignMode.Outside:
                            result = new Vector2(x, yMin - dRect.height * pY);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(alignMode), alignMode, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(align), align, null);
            }

            if (updateAnchors)
            {
                switch (align)
                {
                    case UIDesigner.Align.HorizontalLeft:
                        rectTransform.anchorMin = new Vector2(0f, rectTransform.anchorMin.y);
                        rectTransform.anchorMax = new Vector2(0f, rectTransform.anchorMax.y);
                        break;
                    case UIDesigner.Align.HorizontalCenter:
                        rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
                        rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
                        break;
                    case UIDesigner.Align.HorizontalRight:
                        rectTransform.anchorMin = new Vector2(1f, rectTransform.anchorMin.y);
                        rectTransform.anchorMax = new Vector2(1f, rectTransform.anchorMax.y);
                        break;
                    case UIDesigner.Align.VerticalTop:
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f);
                        break;
                    case UIDesigner.Align.VerticalCenter:
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0.5f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0.5f);
                        break;
                    case UIDesigner.Align.VerticalBottom:
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(align), align, null);
                }
            }

            rectTransform.localPosition = result;
        }

        #endregion

        #region Distribute

        public static void DistributeHorizontalWithSpacing(AlignTo alignTo, RectTransform keyObject, float spacing, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            int count = rectTransforms.Length;
            if (count < 2) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
            float xMin = float.PositiveInfinity;
            float xMax = float.NegativeInfinity;
            float rectsWidth = 0;
            float curX;
            switch (alignTo)
            {
                case AlignTo.RootCanvas:
                    RectTransform root = rectTransforms[0].root.GetComponent<RectTransform>();
                    float rootWidth = root.rect.width;
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.SetParent(root);
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                        Rect dRect = r.DesignerRect();
                        rectsWidth += dRect.width;
                    }
                    spacing = (rootWidth - rectsWidth) / (count - 1);
                    curX = root.rect.xMin;
                    rectTransforms = rectTransforms.OrderBy(r => r.localPosition.x).ToArray();
                    foreach (RectTransform r in rectTransforms)
                    {
                        Rect dRect = r.DesignerRect();
                        r.localPosition = new Vector2(curX + dRect.width * r.pivot.x, r.localPosition.y);
                        curX += dRect.width + spacing;
                    }
                    break;
                case AlignTo.Parent:
                    Transform transformParent = rectTransforms[0].parent;
                    if (transformParent == null) return;
                    RectTransform parentRectTransform = transformParent.GetComponent<RectTransform>();
                    if (parentRectTransform == null) return;
                    Rect parentDesignerRect = parentRectTransform.DesignerRect();
                    float parentWidth = parentDesignerRect.width;
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                        Rect dRect = r.DesignerRect();
                        rectsWidth += dRect.width;
                    }
                    spacing = (parentWidth - rectsWidth) / (count - 1);
                    curX = parentDesignerRect.xMin;
                    rectTransforms = rectTransforms.Where(r => r.parent == transformParent).OrderBy(r => r.localPosition.x).ToArray();
                    foreach (RectTransform r in rectTransforms)
                    {
                        Rect dRect = r.DesignerRect();
                        r.localPosition = new Vector2(curX + dRect.width * r.pivot.x, r.localPosition.y);
                        curX += dRect.width + spacing;
                    }
                    break;
                case AlignTo.Selection:
                    root = rectTransforms[0].root.GetComponent<RectTransform>();
                    float selectionWidth;
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.SetParent(root);
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                        Rect dRect = r.DesignerRect();
                        Vector3 localPosition = r.localPosition;
                        xMin = Min(xMin, localPosition.x - dRect.width * r.pivot.x);
                        xMax = Max(xMax, localPosition.x + dRect.width * r.pivot.x);
                        rectsWidth += dRect.width;
                    }
                    selectionWidth = xMax - xMin;
                    spacing = (selectionWidth - rectsWidth) / (count - 1);
                    curX = xMin;
                    rectTransforms = rectTransforms.OrderBy(r => r.localPosition.x).ToArray();
                    foreach (RectTransform r in rectTransforms)
                    {
                        Rect dRect = r.DesignerRect();
                        r.localPosition = new Vector2(curX + dRect.width * r.pivot.x, r.localPosition.y);
                        curX += dRect.width + spacing;
                    }
                    break;
                case AlignTo.KeyObject:
                    if (keyObject == null) return;
                    root = keyObject.root.GetComponent<RectTransform>();
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.SetParent(root);
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                    }
                    rectTransforms = rectTransforms.Where(r => r != keyObject).ToArray();
                    RectTransform[] left = rectTransforms.Where(r => r.localPosition.x <= keyObject.localPosition.x).OrderByDescending(r => r.localPosition.x).ToArray();
                    RectTransform[] right = rectTransforms.Where(r => r.localPosition.x > keyObject.localPosition.x).OrderBy(r => r.localPosition.x).ToArray();
                    Rect keyDesignerRect = keyObject.DesignerRect();
                    if (left.Length > 0)
                    {
                        curX = keyObject.localPosition.x - keyDesignerRect.width * (1 - keyObject.pivot.x);
                        curX -= spacing;
                        foreach (RectTransform r in left)
                        {
                            Rect dRect = r.DesignerRect();
                            r.localPosition = new Vector2(curX - dRect.width * r.pivot.x, r.localPosition.y);
                            curX -= dRect.width + spacing;
                        }
                    }
                    if (right.Length > 0)
                    {
                        curX = keyObject.localPosition.x + keyDesignerRect.width * keyObject.pivot.x;
                        curX += spacing;
                        foreach (RectTransform r in right)
                        {
                            Rect dRect = r.DesignerRect();
                            r.localPosition = new Vector2(curX + dRect.width * r.pivot.x, r.localPosition.y);
                            curX += dRect.width + spacing;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignTo), alignTo, null);
            }

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void DistributeVerticalWithSpacing(AlignTo alignTo, RectTransform keyObject, float spacing, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            int count = rectTransforms.Length;
            if (count < 2) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
            float yMin = float.PositiveInfinity;
            float yMax = float.NegativeInfinity;
            float rectsHeight = 0;
            float curY;
            switch (alignTo)
            {
                case AlignTo.RootCanvas:
                    RectTransform root = rectTransforms[0].root.GetComponent<RectTransform>();
                    float rootHeight = root.rect.height;
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.SetParent(root);
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                        Rect dRect = r.DesignerRect();
                        rectsHeight += dRect.height;
                    }
                    spacing = (rootHeight - rectsHeight) / (count - 1);
                    curY = root.rect.yMin;
                    rectTransforms = rectTransforms.OrderBy(r => r.localPosition.y).ToArray();
                    foreach (RectTransform r in rectTransforms)
                    {
                        Rect dRect = r.DesignerRect();
                        r.localPosition = new Vector2(r.localPosition.x, curY + dRect.height * r.pivot.y);
                        curY += dRect.height + spacing;
                    }
                    break;
                case AlignTo.Parent:
                    Transform transformParent = rectTransforms[0].parent;
                    if (transformParent == null) return;
                    RectTransform parentRectTransform = transformParent.GetComponent<RectTransform>();
                    if (parentRectTransform == null) return;
                    Rect parentDesignerRect = parentRectTransform.DesignerRect();
                    float parentHeight = parentDesignerRect.height;
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                        Rect dRect = r.DesignerRect();
                        rectsHeight += dRect.height;
                    }
                    spacing = (parentHeight - rectsHeight) / (count - 1);
                    curY = parentDesignerRect.yMin;
                    rectTransforms = rectTransforms.Where(r => r.parent == transformParent).OrderBy(r => r.localPosition.y).ToArray();
                    foreach (RectTransform r in rectTransforms)
                    {
                        Rect dRect = r.DesignerRect();
                        r.localPosition = new Vector2(r.localPosition.x, curY + dRect.height * r.pivot.y);
                        curY += dRect.height + spacing;
                    }
                    break;
                case AlignTo.Selection:
                    root = rectTransforms[0].root.GetComponent<RectTransform>();
                    float selectionHeight;
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.SetParent(root);
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                        Rect dRect = r.DesignerRect();
                        Vector3 localPosition = r.localPosition;
                        yMin = Min(yMin, localPosition.y - dRect.height * r.pivot.y);
                        yMax = Max(yMax, localPosition.y + dRect.height * r.pivot.y);
                        rectsHeight += dRect.height;
                    }
                    selectionHeight = yMax - yMin;
                    spacing = (selectionHeight - rectsHeight) / (count - 1);
                    curY = yMin;
                    rectTransforms = rectTransforms.OrderBy(r => r.localPosition.y).ToArray();
                    foreach (RectTransform r in rectTransforms)
                    {
                        Rect dRect = r.DesignerRect();
                        r.localPosition = new Vector2(r.localPosition.x, curY + dRect.height * r.pivot.y);
                        curY += dRect.height + spacing;
                    }
                    break;
                case AlignTo.KeyObject:
                    if (keyObject == null) return;
                    root = keyObject.root.GetComponent<RectTransform>();
                    foreach (RectTransform r in rectTransforms)
                    {
                        r.SetParent(root);
                        r.ChangePivot(new Vector2(0.5f, 0.5f));
                        r.ForceUpdateRectTransforms();
                    }
                    rectTransforms = rectTransforms.Where(r => r != keyObject).ToArray();
                    RectTransform[] up = rectTransforms.Where(r => r.localPosition.x <= keyObject.localPosition.y).OrderByDescending(r => r.localPosition.y).ToArray();
                    RectTransform[] down = rectTransforms.Where(r => r.localPosition.x > keyObject.localPosition.y).OrderBy(r => r.localPosition.y).ToArray();
                    Rect keyDesignerRect = keyObject.DesignerRect();
                    if (up.Length > 0)
                    {
                        curY = keyObject.localPosition.y - keyDesignerRect.height * (1 - keyObject.pivot.y);
                        curY -= spacing;
                        foreach (RectTransform r in up)
                        {
                            Rect dRect = r.DesignerRect();
                            r.localPosition = new Vector2(r.localPosition.x, curY - dRect.height * r.pivot.y);
                            curY -= dRect.height + spacing;
                        }
                    }
                    if (down.Length > 0)
                    {
                        curY = keyObject.localPosition.y + keyDesignerRect.height * keyObject.pivot.y;
                        curY += spacing;
                        foreach (RectTransform r in down)
                        {
                            Rect dRect = r.DesignerRect();
                            r.localPosition = new Vector2(r.localPosition.x, curY + dRect.height * r.pivot.y);
                            curY += dRect.height + spacing;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignTo), alignTo, null);
            }

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void Distribute(AlignTo alignTo, Distribute distribute, RectTransform keyObject = null, float spacing = 0f, params RectTransform[] rectTransforms)
        {
            switch (alignTo)
            {
                case AlignTo.RootCanvas:
                    DistributeToRootCanvas(distribute, rectTransforms);
                    break;
                case AlignTo.Parent:
                    DistributeToParent(distribute, rectTransforms);
                    break;
                case AlignTo.Selection:
                    DistributeToSelection(distribute, rectTransforms);
                    break;
                case AlignTo.KeyObject:
                    DistributeToKeyObject(distribute, keyObject, spacing, rectTransforms);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignTo), alignTo, null);
            }
        }

        public static void DistributeToRootCanvas(Distribute distribute, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            RectTransform root = rectTransforms[0].root.GetComponent<RectTransform>();
            Rect rootRect = root.rect;
            float xMin = rootRect.xMin;
            float xMax = rootRect.xMax;
            float yMin = rootRect.yMin;
            float yMax = rootRect.yMax;

            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(root);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
            }

            Distribute(distribute, xMin, xMax, yMin, yMax, rectTransforms);

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void DistributeToParent(Distribute distribute, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            Transform transformParent = rectTransforms.First().transform.parent;
            if (transformParent == null) return;
            RectTransform parent = transformParent.GetComponent<RectTransform>();
            if (parent == null) return;
            Rect parentRect = parent.rect;
            float xMin = parentRect.xMin;
            float xMax = parentRect.xMax;
            float yMin = parentRect.yMin;
            float yMax = parentRect.yMax;

            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(transformParent);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
            }

            Distribute(distribute, xMin, xMax, yMin, yMax, rectTransforms);

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void DistributeToSelection(Distribute distribute, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            RectTransform root = rectTransforms[0].root.GetComponent<RectTransform>();

            float xMin = float.PositiveInfinity;
            float xMax = float.NegativeInfinity;
            float yMin = float.PositiveInfinity;
            float yMax = float.NegativeInfinity;

            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(root);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
                Rect designerRect = r.DesignerRect();
                Vector3 localPosition = r.localPosition;
                xMin = Min(xMin, localPosition.x - designerRect.width * (1 - r.pivot.x));
                xMax = Max(xMax, localPosition.x + designerRect.width * r.pivot.x);
                yMin = Min(yMin, localPosition.y - designerRect.height * (1 - r.pivot.y));
                yMax = Max(yMax, localPosition.y + designerRect.height * r.pivot.y);
            }

            Distribute(distribute, xMin, xMax, yMin, yMax, rectTransforms);

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        public static void DistributeToKeyObject(Distribute distribute, RectTransform keyObject, float spacing, params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return;
            IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);

            keyObject ??= rectTransforms[0];
            Vector2 anchoredPosition = keyObject.anchoredPosition;
            Rect dRect = keyObject.DesignerRect();
            Vector2 pivot = keyObject.pivot;

            float xMin = anchoredPosition.x - dRect.width * (1 - pivot.x);
            float xMax = anchoredPosition.x + dRect.width * pivot.x;
            float yMin = anchoredPosition.y - dRect.height * (1 - pivot.y);
            float yMax = anchoredPosition.y + dRect.height * pivot.y;

            RectTransform root = keyObject.root.GetComponent<RectTransform>();

            foreach (RectTransform r in rectTransforms)
            {
                r.SetParent(root);
                r.ChangePivot(new Vector2(0.5f, 0.5f));
                r.ForceUpdateRectTransforms();
            }

            Distribute(distribute, xMin, xMax, yMin, yMax, spacing, rectTransforms);

            foreach (TargetInfo info in targetInfos)
                info.Restore();
        }

        private static void Distribute(Distribute distribute, float xMin, float xMax, float yMin, float yMax, float spacing, params RectTransform[] rectTransforms)
        {
            int count = rectTransforms.Length;
            if (count < 2) return;
            switch (distribute)
            {
                case UIDesigner.Distribute.VerticalTop:
                case UIDesigner.Distribute.VerticalCenter:
                case UIDesigner.Distribute.VerticalBottom:
                    rectTransforms = rectTransforms.OrderByDescending(r => r.localPosition.y).ToArray();
                    break;
                case UIDesigner.Distribute.HorizontalLeft:
                case UIDesigner.Distribute.HorizontalCenter:
                case UIDesigner.Distribute.HorizontalRight:
                    rectTransforms = rectTransforms.OrderBy(r => r.localPosition.x).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(distribute), distribute, null);
            }

            switch (distribute)
            {
                case UIDesigner.Distribute.VerticalTop:
                    DistributeVerticalTop(spacing, yMax, rectTransforms);
                    break;
                case UIDesigner.Distribute.VerticalCenter:
                    DistributeVerticalCenter(spacing, yMax, rectTransforms);
                    break;
                case UIDesigner.Distribute.VerticalBottom:
                    DistributeVerticalBottom(spacing, yMin, rectTransforms);
                    break;
                case UIDesigner.Distribute.HorizontalLeft:
                    DistributeHorizontalLeft(spacing, xMin, rectTransforms);
                    break;
                case UIDesigner.Distribute.HorizontalCenter:
                    DistributeHorizontalCenter(spacing, xMin, rectTransforms);
                    break;
                case UIDesigner.Distribute.HorizontalRight:
                    DistributeHorizontalRight(spacing, xMax, rectTransforms);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(distribute), distribute, null);
            }
        }

        private static void DistributeVerticalTop(float spacing, float yMax, params RectTransform[] rectTransforms)
        {
            float curY = yMax;
            foreach (RectTransform r in rectTransforms)
            {
                r.localPosition = new Vector2(r.localPosition.x, curY - r.DesignerRect().height * (1 - r.pivot.y));
                curY -= spacing;
            }
        }

        private static void DistributeVerticalCenter(float spacing, float yMax, params RectTransform[] rectTransforms)
        {
            RectTransform first = rectTransforms.First();
            float firstItemTopHalfHeight = first.DesignerRect().height * (1 - first.pivot.y);
            float curY = yMax;
            foreach (RectTransform r in rectTransforms)
            {
                bool isFirst = r == first;
                if (isFirst)
                {
                    r.localPosition = new Vector2(r.localPosition.x, curY - firstItemTopHalfHeight);
                    curY -= firstItemTopHalfHeight + spacing;
                    continue;
                }

                r.localPosition = new Vector2(r.localPosition.x, curY);
                curY -= spacing;
            }
        }

        private static void DistributeVerticalBottom(float spacing, float yMin, params RectTransform[] rectTransforms)
        {
            float curY = yMin;
            for (int i = rectTransforms.Length - 1; i >= 0; i--)
            {
                RectTransform r = rectTransforms[i];
                r.localPosition = new Vector2(r.localPosition.x, curY + r.DesignerRect().height * r.pivot.y);
                curY += spacing;
            }
        }

        private static void DistributeHorizontalLeft(float spacing, float xMin, params RectTransform[] rectTransforms)
        {
            float curX = xMin;
            foreach (RectTransform r in rectTransforms)
            {
                r.localPosition = new Vector2(curX + r.DesignerRect().width * r.pivot.x, r.localPosition.y);
                curX += spacing;
            }
        }

        private static void DistributeHorizontalCenter(float spacing, float xMin, params RectTransform[] rectTransforms)
        {
            RectTransform firstItem = rectTransforms.First();
            float firstItemLeftHalfWidth = firstItem.DesignerRect().width * (1 - firstItem.pivot.x);
            float curX = xMin;
            foreach (RectTransform r in rectTransforms)
            {
                bool isFirst = r == firstItem;
                if (isFirst)
                {
                    r.localPosition = new Vector2(curX + firstItemLeftHalfWidth, r.localPosition.y);
                    curX += firstItemLeftHalfWidth + spacing;
                    continue;
                }

                r.localPosition = new Vector2(curX, r.localPosition.y);
                curX += spacing;
            }
        }

        private static void DistributeHorizontalRight(float spacing, float xMax, params RectTransform[] rectTransforms)
        {
            float curX = xMax;
            for (int i = rectTransforms.Length - 1; i >= 0; i--)
            {
                RectTransform r = rectTransforms[i];
                r.localPosition = new Vector2(curX - r.DesignerRect().width * (1 - r.pivot.x), r.localPosition.y);
                curX -= spacing;
            }
        }

        private static void Distribute(Distribute distribute, float xMin, float xMax, float yMin, float yMax, params RectTransform[] rectTransforms)
        {
            int count = rectTransforms.Length;
            if (count < 2) return;
            switch (distribute)
            {
                case UIDesigner.Distribute.VerticalTop:
                case UIDesigner.Distribute.VerticalCenter:
                case UIDesigner.Distribute.VerticalBottom:
                    rectTransforms = rectTransforms.OrderByDescending(r => r.localPosition.y).ToArray();
                    break;
                case UIDesigner.Distribute.HorizontalLeft:
                case UIDesigner.Distribute.HorizontalCenter:
                case UIDesigner.Distribute.HorizontalRight:
                    rectTransforms = rectTransforms.OrderBy(r => r.localPosition.x).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(distribute), distribute, null);
            }

            float width = xMax - xMin;
            float height = yMax - yMin;
            float spacing;

            RectTransform firstItem = rectTransforms.First();
            RectTransform lastItem = rectTransforms.Last();

            switch (distribute)
            {
                case UIDesigner.Distribute.VerticalTop:
                    float lastItemHeight = lastItem.DesignerRect().height;
                    spacing = (height - lastItemHeight) / (count - 1);
                    DistributeVerticalTop(spacing, yMax, rectTransforms);
                    break;
                case UIDesigner.Distribute.VerticalCenter:
                    float firstItemTopHalfHeight = firstItem.DesignerRect().height * (1 - firstItem.pivot.y);
                    float lastItemBottomHalfHeight = lastItem.DesignerRect().height * lastItem.pivot.y;
                    spacing = (height - firstItemTopHalfHeight - lastItemBottomHalfHeight) / (count - 1);
                    DistributeVerticalCenter(spacing, yMax, rectTransforms);
                    break;
                case UIDesigner.Distribute.VerticalBottom:
                    float firstItemHeight = firstItem.DesignerRect().height;
                    spacing = (height - firstItemHeight) / (count - 1);
                    DistributeVerticalBottom(spacing, yMin, rectTransforms);
                    break;
                case UIDesigner.Distribute.HorizontalLeft:
                    float lastItemWidth = lastItem.DesignerRect().width;
                    spacing = (width - lastItemWidth) / (count - 1);
                    DistributeHorizontalLeft(spacing, xMin, rectTransforms);
                    break;
                case UIDesigner.Distribute.HorizontalCenter:
                    float firstItemLeftHalfWidth = firstItem.DesignerRect().width * (1 - firstItem.pivot.x);
                    float lastItemRightHalfWidth = lastItem.DesignerRect().width * lastItem.pivot.x;
                    spacing = (width - firstItemLeftHalfWidth - lastItemRightHalfWidth) / (count - 1);
                    DistributeHorizontalCenter(spacing, xMin, rectTransforms);
                    break;
                case UIDesigner.Distribute.HorizontalRight:
                    float firstItemWidth = firstItem.DesignerRect().width;
                    spacing = (width - firstItemWidth) / (count - 1);
                    DistributeHorizontalRight(spacing, xMax, rectTransforms);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(distribute), distribute, null);
            }
        }

        #endregion

        private static IEnumerable<TargetInfo> GetTargetInfos(params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return null;
            var targetInfos = new TargetInfo[rectTransforms.Length];
            for (int i = 0; i < rectTransforms.Length; i++)
                targetInfos[i] = new TargetInfo(rectTransforms[i]);
            return targetInfos;
        }

        private struct TargetInfo
        {
            public RectTransform rectTransform { get; }
            public Transform savedParent { get; private set; }
            public int savedSiblingIndex { get; private set; }
            public Vector2 savedPivot { get; private set; }

            public TargetInfo(RectTransform rectTransform)
            {
                this.rectTransform = rectTransform;
                savedParent = rectTransform.parent;
                savedSiblingIndex = rectTransform.GetSiblingIndex();
                savedPivot = rectTransform.pivot;
            }

            public void Restore()
            {
                if (savedPivot != rectTransform.pivot)
                {
                    rectTransform.ChangePivot(savedPivot);
                }

                if (rectTransform.parent != savedParent)
                {
                    rectTransform.SetParent(savedParent);
                    rectTransform.SetSiblingIndex(savedSiblingIndex);
                }

                rectTransform.ForceUpdateRectTransforms();
            }
        }

        #endregion

        #region Rotation

        private static List<RotationInfo> currentRotationInfos { get; } = new List<RotationInfo>();

        public static void StartRotationChange(params RectTransform[] rectTransforms)
        {
            currentRotationInfos.Clear();
            currentRotationInfos.AddRange(GetRotationInfos(rectTransforms));
        }

        public static void StopRotationChange()
        {
            currentRotationInfos.Clear();
        }

        public static void UpdateRotationXY(Space space, float x, float y, bool relativeChange)
        {
            if (currentRotationInfos.Count == 0) return;
            foreach (RotationInfo info in currentRotationInfos)
                info.SetRotationXY(space, x, y, relativeChange);
        }

        public static void UpdateRotationXZ(Space space, float x, float z, bool relativeChange)
        {
            if (currentRotationInfos.Count == 0) return;
            foreach (RotationInfo info in currentRotationInfos)
                info.SetRotationXZ(space, x, z, relativeChange);
        }

        public static void UpdateRotationYZ(Space space, float y, float z, bool relativeChange)
        {
            if (currentRotationInfos.Count == 0) return;
            foreach (RotationInfo info in currentRotationInfos)
                info.SetRotationYZ(space, y, z, relativeChange);
        }

        public static void UpdateRotation(Space space, float x, float y, float z, bool relativeChange)
        {
            if (currentRotationInfos.Count == 0) return;
            foreach (RotationInfo info in currentRotationInfos)
                info.SetRotation(space, x, y, z, relativeChange);
        }

        public static void UpdateRotation(Space space, Axis axis, float value, bool relativeChange)
        {
            if (currentRotationInfos.Count == 0) return;
            foreach (RotationInfo info in currentRotationInfos)
                info.SetRotation(space, axis, value, relativeChange);
        }

        private static IEnumerable<RotationInfo> GetRotationInfos(params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return null;
            var infos = new RotationInfo[rectTransforms.Length];
            for (int i = 0; i < rectTransforms.Length; i++)
                infos[i] = new RotationInfo(rectTransforms[i]);
            return infos;
        }

        private readonly struct RotationInfo
        {
            /// <summary> Target RectTransform </summary>
            public RectTransform rectTransform { get; }

            /// <summary> The initial rectTransform.eulerAngles value of the rectTransform before the rotation changed </summary>
            public Vector3 worldRotation { get; }

            /// <summary> The initial rectTransform.localEulerAngles value of the rectTransform before the rotation changed </summary>
            public Vector3 localRotation { get; }

            public RotationInfo(RectTransform rectTransform)
            {
                this.rectTransform = rectTransform;
                worldRotation = rectTransform.eulerAngles;
                localRotation = rectTransform.localEulerAngles;
            }

            public void SetRotation(Space space, Axis axis, float value, bool relativeChange)
            {
                Vector3 r = GetRotation(space);
                switch (axis)
                {
                    case Axis.X:
                        r.x = relativeChange ? r.x + value : value;
                        break;
                    case Axis.Y:
                        r.y = relativeChange ? r.y + value : value;
                        break;
                    case Axis.Z:
                        r.z = relativeChange ? r.z + value : value;
                        break;
                }
                SetRotation(space, r);
            }

            public void SetRotationXY(Space space, float x, float y, bool relativeChange)
            {
                Vector3 r = GetRotation(space);
                r.x = relativeChange ? r.x + x : x;
                r.y = relativeChange ? r.y + y : y;
                SetRotation(space, r);
            }

            public void SetRotationXZ(Space space, float x, float z, bool relativeChange)
            {
                Vector3 r = GetRotation(space);
                r.x = relativeChange ? r.x + x : x;
                r.z = relativeChange ? r.z + z : z;
                SetRotation(space, r);
            }

            public void SetRotationYZ(Space space, float y, float z, bool relativeChange)
            {
                Vector3 r = GetRotation(space);
                r.y = relativeChange ? r.y + y : y;
                r.z = relativeChange ? r.z + z : z;
                SetRotation(space, r);
            }

            public void SetRotation(Space space, float x, float y, float z, bool relativeChange)
            {
                Vector3 r = GetRotation(space);
                r.x = relativeChange ? r.x + x : x;
                r.y = relativeChange ? r.y + y : y;
                r.z = relativeChange ? r.z + z : z;
                SetRotation(space, r);
            }

            private Vector3 GetRotation(Space space) =>
                space == Space.World ? worldRotation : localRotation;

            private void SetRotation(Space space, Vector3 value)
            {
                switch (space)
                {
                    case Space.World:
                        rectTransform.eulerAngles = value;
                        break;
                    case Space.Local:
                        rectTransform.localEulerAngles = value;
                        break;
                }
            }
        }

        #endregion

        #region Scale

        private static List<ScaleInfo> currentScaleInfos { get; } = new List<ScaleInfo>();
        
        public static void StartScaleChange(params RectTransform[] rectTransforms)
        {
            currentScaleInfos.Clear();
            currentScaleInfos.AddRange(GetScaleInfos(rectTransforms));
        }
        
        public static void StopScaleChange()
        {
            currentScaleInfos.Clear();
        }

        public static void UpdateScaleX(float x, bool relativeChange)
        {
            if(currentScaleInfos.Count == 0) return;
            foreach (ScaleInfo info in currentScaleInfos)
                info.SetScaleX(x, relativeChange);
        }
        
        public static void UpdateScaleY(float y, bool relativeChange)
        {
            if(currentScaleInfos.Count == 0) return;
            foreach (ScaleInfo info in currentScaleInfos)
                info.SetScaleY(y, relativeChange);
        }
        
        public static void UpdateScaleXY(float x, float y, bool relativeChange)
        {
            if(currentScaleInfos.Count == 0) return;
            foreach (ScaleInfo info in currentScaleInfos)
                info.SetScaleXY(x, y, relativeChange);
        }

        private static IEnumerable<ScaleInfo> GetScaleInfos(params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return null;
            var infos = new ScaleInfo[rectTransforms.Length];
            for (int i = 0; i < rectTransforms.Length; i++)
                infos[i] = new ScaleInfo(rectTransforms[i]);
            return infos;
        }

        private readonly struct ScaleInfo
        {
            /// <summary> Target RectTransform </summary>
            public RectTransform rectTransform { get; }

            /// <summary> The initial rectTransform.localScale value of the rectTransform before the scale changed </summary>
            public Vector3 localScale { get; }

            public ScaleInfo(RectTransform rectTransform)
            {
                this.rectTransform = rectTransform;
                localScale = rectTransform.localScale;
            }

            public void SetScaleX(float x, bool relativeChange)
            {
                Vector3 s = GetScale();
                s.x = relativeChange ? s.x + x : x;
                SetScale(s);
            }

            public void SetScaleY(float y, bool relativeChange)
            {
                Vector3 s = GetScale();
                s.y = relativeChange ? s.y + y : y;
                SetScale(s);
            }

            public void SetScaleXY(float x, float y, bool relativeChange)
            {
                Vector3 s = GetScale();
                s.x = relativeChange ? s.x + x : x;
                s.y = relativeChange ? s.y + y : y;
                SetScale(s);
            }

            private Vector3 GetScale() =>
                localScale;

            private void SetScale(Vector3 value)
            {
                rectTransform.localScale = value;
            }
        }

        #endregion

        #region Size

        private static List<SizeInfo> currentSizeInfos { get; } = new List<SizeInfo>();
        
        public static void StartSizeChange(params RectTransform[] rectTransforms)
        {
            currentSizeInfos.Clear();
            currentSizeInfos.AddRange(GetSizeInfos(rectTransforms));
        }
        
        public static void StopSizeChange()
        {
            currentSizeInfos.Clear();
        }
        
        public static void UpdateSizeX(float x, bool relativeChange)
        {
            if(currentSizeInfos.Count == 0) return;
            foreach (SizeInfo info in currentSizeInfos)
                info.SetSizeX(x, relativeChange);
        }
        
        public static void UpdateSizeY(float y, bool relativeChange)
        {
            if(currentSizeInfos.Count == 0) return;
            foreach (SizeInfo info in currentSizeInfos)
                info.SetSizeY(y, relativeChange);
        }
        
        public static void UpdateSizeXY(float x, float y, bool relativeChange)
        {
            if(currentSizeInfos.Count == 0) return;
            foreach (SizeInfo info in currentSizeInfos)
                info.SetSizeXY(x, y, relativeChange);
        }

        private static IEnumerable<SizeInfo> GetSizeInfos(params RectTransform[] rectTransforms)
        {
            if (rectTransforms == null || rectTransforms.Length == 0) return null;
            var infos = new SizeInfo[rectTransforms.Length];
            for (int i = 0; i < rectTransforms.Length; i++)
                infos[i] = new SizeInfo(rectTransforms[i]);
            return infos;
        }

        private readonly struct SizeInfo
        {
            /// <summary> Target RectTransform </summary>
            public RectTransform rectTransform { get; }
            
            /// <summary> The initial rectTransform.sizeDelta value of the rectTransform before the size changed </summary>
            public Vector2 sizeDelta { get; }
            
            public SizeInfo(RectTransform rectTransform)
            {
                this.rectTransform = rectTransform;
                sizeDelta = rectTransform.sizeDelta;
            }
            
            public void SetSizeX(float x, bool relativeChange)
            {
                Vector2 s = GetSize();
                s.x = relativeChange ? s.x + x : x;
                SetSize(s);
            }
            
            public void SetSizeY(float y, bool relativeChange)
            {
                Vector2 s = GetSize();
                s.y = relativeChange ? s.y + y : y;
                SetSize(s);
            }
            
            public void SetSizeXY(float x, float y, bool relativeChange)
            {
                Vector2 s = GetSize();
                s.x = relativeChange ? s.x + x : x;
                s.y = relativeChange ? s.y + y : y;
                SetSize(s);
            }
            
            private Vector2 GetSize() =>
                sizeDelta;
            
            private void SetSize(Vector2 value)
            {
                rectTransform.sizeDelta = value;
            }
        }

        #endregion

    }
}
