// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Reflection.Enums;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable NotAccessedField.Local

namespace Doozy.Runtime.Reactor.Reflection.Internal
{
    [Serializable]
    public abstract class MetaReflectedValue<T> : ReflectedValue
    {
        /// <summary> Reflected value </summary>
        public T value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public T GetValue()
        {
            if (!initialized) Initialize();
            if (!initialized) return default;
            switch (ValueDetails)
            {
                case ValueDetails.IsProperty: return (T)targetProperty.GetValue(Target);
                case ValueDetails.IsField: return (T)targetField.GetValue(Target);
                case ValueDetails.None: return default;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void SetValue(T newValue)
        {
            if (!initialized) Initialize();
            if (!initialized) return;
            switch (ValueDetails)
            {
                case ValueDetails.IsProperty:
                    targetProperty.SetValue(Target, newValue);
                    break;
                case ValueDetails.IsField:
                    targetField.SetValue(Target, newValue);
                    break;
                case ValueDetails.None:
                default:
                    break;
            }
        }

        /// <summary> Check if it's valid and get either the target FieldInfo or the target PropertyInfo </summary>
        public override bool Initialize()
        {
            initialized = false;
            targetField = null;
            targetProperty = null;
            if (!IsValid()) return false;
            initialized = true;
            return true;

        }
        
        public override bool IsValid()
        {
            if (Target == null)
                return false;

            switch (ValueDetails)
            {
                case ValueDetails.None:
                    return false;
                case ValueDetails.IsProperty:
                    if (PropertyName.IsNullOrEmpty()) return false;
                    targetProperty = GetPropertyInfos<T>(Target).FirstOrDefault(p => p.Name.Equals(PropertyName));
                    return targetProperty != null;
                case ValueDetails.IsField:
                    if (FieldName.IsNullOrEmpty()) return false;
                    targetField = GetFieldInfos<T>(Target).FirstOrDefault(f => f.Name.Equals(FieldName));
                    return targetField != null;
                default:
                    return false;
            }
        }
        
        public override List<KeyValuePair<string, UnityAction>> GetSearchMenuItems()
        {
            searchItems ??= new HashSet<SearchItem>();
            searchItems.Clear();

            var keyValuePairsList = new List<KeyValuePair<string, UnityAction>>();

            if (Target == null)
                return keyValuePairsList;

            //get the GameObject for the target UnityEvent.Object
            GameObject go = GetGameObject();

            //add the GameObject to the search items
            searchItems.Add(new SearchItem(go, GetFieldInfos<T>(go), GetPropertyInfos<T>(go), SetTarget, SetField, SetProperty));

            //add all attached components to search items
            foreach (Component co in go.GetComponents(typeof(Component)))
                searchItems.Add(new SearchItem(co, GetFieldInfos<T>(co), GetPropertyInfos<T>(co), SetTarget, SetField, SetProperty));

            //create the search list
            foreach (SearchItem item in searchItems)
                foreach (KeyValuePair<string, UnityAction> searchAction in item.GetSearchActions())
                    keyValuePairsList.Add(new KeyValuePair<string, UnityAction>(searchAction.Key, searchAction.Value));

            //42
            return keyValuePairsList;
        }
    }
}
