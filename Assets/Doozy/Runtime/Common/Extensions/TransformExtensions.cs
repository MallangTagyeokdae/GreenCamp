// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Doozy.Runtime.Common.Extensions
{
    public static class TransformExtensions
    {
        public static void DestroyChildren(this Transform target)
        {
            for (int i = target.childCount - 1; i >= 0; i--)
                Object.Destroy(target.GetChild(i).gameObject);
        }
        
        public static void ResetTransformation(this Transform target)
        {
            target.position = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }
        
        public static Transform GetChildByName(this Transform target, string childName)
        {
            foreach (Transform child in target)
            {
                if (child.name == childName)
                {
                    return child;
                }
            }
 
            throw new KeyNotFoundException();
        }
        
        public static Transform GetFromPath(this Transform target, string path)
        {
            string[] split = path.Split('/');
            return split.Aggregate(target, (current1, childName) => current1.GetChildByName(childName));
        }
        
        public static IEnumerable<Transform> GetChildren(this Transform target)
        {
            foreach (Transform child in target)
                yield return child;
        }
        
        public static IEnumerable<Transform> Traverse(this Transform target)
        {
            yield return target;
            foreach (Transform x in target)
                foreach (Transform y in x.Traverse())
                    yield return y;
        }
        
        public static IEnumerable<Transform> Ancestors(this Transform target)
        {
            yield return target;
            if (target.parent == null)
                yield break;
            foreach (Transform x in target.parent.Ancestors())
                yield return x;
        }
    }
}
