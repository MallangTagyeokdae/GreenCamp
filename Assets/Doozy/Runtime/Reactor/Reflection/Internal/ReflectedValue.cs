// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Reflection.Enums;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Runtime.Reactor.Reflection.Internal
{
    [Serializable]
    public abstract class ReflectedValue
    {
        [SerializeField] protected Object Target;
        /// <summary> Value target </summary>
        public Object target => Target;

        [SerializeField] protected string FieldName = "";
        /// <summary> Target fieldName  </summary>
        public string fieldName => FieldName;

        [SerializeField] protected string PropertyName = "";
        /// <summary> Target propertyName </summary>
        public string propertyName => PropertyName;

        [SerializeField] protected ValueDetails ValueDetails = ValueDetails.None;
        /// <summary> Target value details </summary>
        public ValueDetails valueDetails => ValueDetails;

        protected FieldInfo targetField { get; set; }
        protected PropertyInfo targetProperty { get; set; }

        protected bool initialized { get; set; }
        protected HashSet<SearchItem> searchItems { get; set; }

        /// <summary> Check if it's valid and get either the target FieldInfo or the target PropertyInfo </summary>
        public abstract bool Initialize();
		public abstract bool IsValid();
        public abstract List<KeyValuePair<string, UnityAction>> GetSearchMenuItems();
        
        protected void SetTarget(Object targetObject)
        {
            ClearValueDetails();
            Target = targetObject;
        }

        protected void SetProperty(string nameOfProperty)
        {
            FieldName = string.Empty;
            PropertyName = nameOfProperty;
            ValueDetails = nameOfProperty.IsNullOrEmpty() ? ValueDetails.None : ValueDetails.IsProperty;
        }

        protected void SetField(string nameOfField)
        {
            FieldName = nameOfField;
            PropertyName = string.Empty;
            ValueDetails = nameOfField.IsNullOrEmpty() ? ValueDetails.None : ValueDetails.IsField;
        }

        protected void ClearValueDetails()
        {
            FieldName = string.Empty;
            PropertyName = string.Empty;
            ValueDetails = ValueDetails.None;
        }

        protected GameObject GetGameObject() =>
            Target switch
            {
                GameObject go => go,
                Component co  => co.gameObject,
                _             => null
            };

        protected IEnumerable<FieldInfo> GetFieldInfos<T>(IReflect targetType) =>
            FieldInfos(targetType, typeof(T));

        protected IEnumerable<PropertyInfo> GetPropertyInfos<T>(IReflect targetType) =>
            PropertyInfos(targetType, typeof(T));

        protected IEnumerable<FieldInfo> GetFieldInfos<T>(Object targetObject) =>
            GetFieldInfos<T>(targetObject.GetType());

        protected IEnumerable<PropertyInfo> GetPropertyInfos<T>(Object targetObject) =>
            GetPropertyInfos<T>(targetObject.GetType());

        protected static IEnumerable<FieldInfo> FieldInfos(IReflect targetType, Type ofType) =>
            targetType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == ofType);

        protected static IEnumerable<PropertyInfo> PropertyInfos(IReflect targetType, Type ofType) =>
            targetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == ofType && p.CanRead & p.CanWrite);

        [Serializable]
        public struct SearchItem
        {
            public Object target { get; }
            public List<string> fields { get; }
            public List<string> properties { get; }

            public UnityAction<Object> TargetSetter;
            public UnityAction<string> FieldSetter;
            public UnityAction<string> PropertySetter;

            private string typeName => target.GetType().Name;
            private string GetPath(string s) => $"{typeName}/{s}";

            public List<KeyValuePair<string, UnityAction>> GetSearchActions()
            {
                //why are you here?

                var list = new List<KeyValuePair<string, UnityAction>>();

                foreach (string f in fields)
                {
                    SearchItem tmpThis = this;
                    list.Add(new KeyValuePair<string, UnityAction>(tmpThis.GetPath(f), () =>
                    {
                        tmpThis.TargetSetter.Invoke(tmpThis.target);
                        tmpThis.FieldSetter.Invoke(f);
                    }));
                }

                foreach (string p in properties)
                {
                    SearchItem tmpThis = this;
                    list.Add(new KeyValuePair<string, UnityAction>(tmpThis.GetPath(p), () =>
                    {
                        tmpThis.TargetSetter.Invoke(tmpThis.target);
                        tmpThis.PropertySetter.Invoke(p);
                    }));
                }

                return list;
            }

            public SearchItem
            (
                Object target,
                IEnumerable<FieldInfo> fields,
                IEnumerable<PropertyInfo> properties,
                UnityAction<Object> targetSetter,
                UnityAction<string> fieldSetter,
                UnityAction<string> propertySetter
            )
            {
                //what are you searching for?
                this.target = target;
                this.fields = fields.Select(f => f.Name).ToList();
                this.properties = properties.Select(p => p.Name).ToList();
                TargetSetter = targetSetter;
                FieldSetter = fieldSetter;
                PropertySetter = propertySetter;
            }
        }
    }
}
