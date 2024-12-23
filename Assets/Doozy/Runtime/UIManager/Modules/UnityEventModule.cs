// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Modules
{
    /// <summary> Mody module used to trigger a UnityEvent </summary>
    [AddComponentMenu("Mody/UnityEvent Module")]
    public class UnityEventModule : ModyModule
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Mody/UnityEvent Module", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UnityEventModule>("UnityEvent Module", false, true);
        }
        #endif
        
        /// <summary> Default module name </summary>
        public const string k_DefaultModuleName = "UnityEvent";

        /// <summary> Target UnityEvent </summary>
        public UnityEvent Event = new UnityEvent();

        /// <summary> Simple action that triggers the UnityEvent </summary>
        public SimpleModyAction InvokeEvent;

        /// <summary> Construct a UnityEvent Module with the default name </summary>
        public UnityEventModule() : this(k_DefaultModuleName) {}

        /// <summary> Construct a UnityEvent Module with the given name </summary>
        /// <param name="moduleName"> Module name </param>
        public UnityEventModule(string moduleName) : base(moduleName.IsNullOrEmpty() ? k_DefaultModuleName : moduleName) {}

        /// <summary> Initialize the actions </summary>
        protected override void SetupActions()
        {
            this.AddAction(InvokeEvent ??= new SimpleModyAction(this, nameof(InvokeEvent), ExecuteInvokeEvent));
        }

        /// <summary> Execute Invoke on the UnityEvent </summary>
        public void ExecuteInvokeEvent()
        {
            Event?.Invoke();
        }
    }
}
