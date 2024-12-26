// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
namespace Doozy.Runtime.Common.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject target) where T : MonoBehaviour
        {
            T component = target.GetComponent<T>();
            if (component == null) target.AddComponent<T>();
            return component;
        }
        
        public static bool HasComponent<T>(this GameObject target) where T : MonoBehaviour =>
            target.GetComponent<T>() != null;
        
        
    }
}
