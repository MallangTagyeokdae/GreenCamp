// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using static UnityEngine.Mathf;

namespace Doozy.Runtime.UIDesigner
{
    public static class RectTransformExtensions
    {
        /// <summary> Get UIDesigner rect </summary>
        /// <param name="target"> Target RectTransform </param>
        public static Rect DesignerRect(this RectTransform target)
        {
            Rect targetRect = target.rect;
            Vector3 targetLocalEulerAngles = target.localEulerAngles;
            Vector3 tempLocalScale = target.localScale;
            Vector2 targetLocalScale = new Vector2(Abs(tempLocalScale.x), Abs(tempLocalScale.y));

            float width = targetRect.width * targetLocalScale.x;
            float height = targetRect.height * targetLocalScale.y;
            float x = targetRect.x + targetRect.width / 2f - width / 2f;
            float y = targetRect.y + targetRect.height / 2f - height / 2f;

            float rotationOffsetX = 0f;
            float rotationOffsetY = 0f;

            float newWidth;
            float newHeight;

            float angle = Abs(targetLocalEulerAngles.z % 180);
            float theta = angle * Deg2Rad;

            if (Approximately(angle, 0) || Approximately(angle, 90))
            {
                newWidth = width;
                newHeight = height;
            }
            else if (angle < 90)
            {
                newWidth = width * Cos(theta) + height * Sin(theta);
                newHeight = width * Sin(theta) + height * Cos(theta);
            }
            else
            {
                angle -= 90;
                theta = angle * Deg2Rad;
                newWidth = height * Cos(theta) + width * Sin(theta);
                newHeight = height * Sin(theta) + width * Cos(theta);
            }

            rotationOffsetX = (newWidth - width) / 2f;
            rotationOffsetY = (newHeight - height) / 2f;

            return new Rect(x + rotationOffsetX, y + rotationOffsetY, newWidth, newHeight);
        }

        /// <summary> Change the RectTransform pivot without changing the position </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="pivot"> New pivot value </param>
        public static RectTransform ChangePivot(this RectTransform target, Vector2 pivot)
        {
                   
            Vector2 sizeDelta = target.sizeDelta;
            Vector3 localScale = target.localScale;
            Vector2 deltaPivot = target.pivot - pivot;
            float deltaX = deltaPivot.x * sizeDelta.x * localScale.x;
            float deltaY = deltaPivot.y * sizeDelta.y * localScale.y;
            float rot = target.rotation.eulerAngles.z * PI / 180;
            var deltaPosition = new Vector3(Cos(rot) * deltaX - Sin(rot) * deltaY, Sin(rot) * deltaX + Cos(rot) * deltaY);
            target.pivot = pivot;
            target.localPosition -= deltaPosition;
            return target;
        }
    }
}
