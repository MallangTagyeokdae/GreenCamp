// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Ticker;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Ticker
{
    /// <summary> Registers to the Editor Ticker Service and ticks a target callback (when registered) </summary>
    public class EditorHeartbeat : Heartbeat
    {
        /// <summary> Construct an editor Heartbeat with no target callback </summary>
        public EditorHeartbeat() : base(null) {}
        
        /// <summary> Construct an editor Heartbeat with a target callback </summary>
        /// <param name="onTickCallback"> Target callback </param>
        public EditorHeartbeat(ReactionCallback onTickCallback) : base(onTickCallback) {}
        
        /// <summary> EditorApplication.timeSinceStartup </summary>
        public override double timeSinceStartup => EditorTicker.timeSinceStartup;
       
        /// <summary> Register to the Editor Ticker Service <para/> Start ticking the callback </summary>
        public override void RegisterToTickService()
        {
            base.RegisterToTickService();
            EditorTicker.service.Register(this);
        }

        /// <summary> Unregister from the Runtime Ticker Service <para/> Stop ticking the callback </summary>
        public override void UnregisterFromTickService()
        {
            base.UnregisterFromTickService();
            EditorTicker.service.Unregister(this);
        }

        /// <summary>
        /// EditorUtility needs to SetDirty on this UnityEngine.Object to perform the SceneView.RepaintAll()
        /// </summary>
        private Object targetObject { get; set; }
        
        /// <summary>
        /// Calls EditorUtility.SetDirty on the targetObject and then SceneView.RepaintAll
        /// </summary>
        private void RefreshSceneViewOnTick()
        {
            if (targetObject == null)
            {
                StopSceneViewRefresh();
                return;
            }
            EditorUtility.SetDirty(targetObject);
            SceneView.RepaintAll();
        }

        /// <summary>
        /// Triggers a SceneView.RepaintAll whenever this heartbeat ticks 
        /// </summary>
        /// <param name="target">
        /// Target UnityEngine.Object needed by the EditorUtility to SetDirty.
        /// SceneView.RepaintAll does not work otherwise.
        /// </param>
        public EditorHeartbeat StartSceneViewRefresh(Object target)
        {
            StopSceneViewRefresh();
            targetObject = target;
            if (target == null) return this;
            this.AddOnTickCallback(RefreshSceneViewOnTick);
            return this;
        }

        /// <summary>
        /// Stop SceneView.RepaintAll whenever this heartbeat ticks
        /// </summary>
        public EditorHeartbeat StopSceneViewRefresh()
        {
            targetObject = null;
            this.RemoveOnTickCallback(RefreshSceneViewOnTick);
            return this;
        }
    }
}