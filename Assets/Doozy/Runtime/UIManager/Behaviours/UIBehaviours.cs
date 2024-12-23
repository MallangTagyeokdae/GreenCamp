// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public class UIBehaviours
    {
        [SerializeField] private List<UIBehaviour> Behaviours;
        /// <summary> List of UIBehaviour </summary>
        public List<UIBehaviour> behaviours => Behaviours;

        [SerializeField] private GameObject SignalSource;
        /// <summary> Signal source </summary>
        public GameObject signalSource => SignalSource;

        [SerializeField] private UISelectable Selectable;
        /// <summary> UISelectable target </summary>
        public UISelectable selectable => Selectable;

        /// <summary> Construct a new UIBehaviours with a null signal source </summary>
        public UIBehaviours() : this(null) {}

        /// <summary> Construct a new UIBehaviours with the given signal source </summary>
        public UIBehaviours(GameObject signalSource)
        {
            SignalSource = signalSource;
            Behaviours = new List<UIBehaviour>();
        }

        /// <summary> Connect all behaviours </summary>
        public UIBehaviours Connect()
        {
            if (signalSource == null) return this;
            Behaviours.ForEach(ConnectBehaviour);
            return this;
        }

        /// <summary> Disconnect all behaviours </summary>
        public UIBehaviours Disconnect()
        {
            Behaviours.ForEach(DisconnectBehaviour);
            return this;
        }

        /// <summary> Connect behaviour </summary>
        /// <param name="behaviour"> Target behaviour </param>
        private void ConnectBehaviour(UIBehaviour behaviour)
        {
            if (behaviour == null) return;
            behaviour.Disconnect();
            behaviour
                .SetSelectable(selectable)
                .SetSignalSource(signalSource)
                .Connect();
        }

        /// <summary> Disconnect behaviour </summary>
        /// <param name="behaviour"> Target Behaviour </param>
        private void DisconnectBehaviour(UIBehaviour behaviour) =>
            behaviour?.Disconnect();

        /// <summary>
        /// Add the given behaviour and get a reference to it (automatically connects)
        /// If the behaviour already exists, the reference to it will get automatically returned. 
        /// </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public UIBehaviour AddBehaviour(UIBehaviour.Name behaviourName)
        {
            if (HasBehaviour(behaviourName))
                return GetBehaviour(behaviourName);

            UIBehaviour newBehaviour =
                new UIBehaviour(behaviourName, signalSource)
                    .SetSelectable(selectable);

            Behaviours.Add(newBehaviour);

            if (Application.isPlaying)
                ConnectBehaviour(newBehaviour);

            var temp = (from UIBehaviour.Name name in Enum.GetValues(typeof(UIBehaviour.Name)) select GetBehaviour(name) into b where b != null select b).ToList();
            Behaviours.Clear();
            Behaviours.AddRange(temp);

            return newBehaviour;
        }

        /// <summary> Remove the given behaviour (automatically disconnects) </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public void RemoveBehaviour(UIBehaviour.Name behaviourName)
        {
            UIBehaviour behaviour = GetBehaviour(behaviourName);
            if (behaviour == null) return;
            DisconnectBehaviour(behaviour);
            Behaviours.Remove(behaviour);
        }

        /// <summary> Check if the given behaviour has been added (exists) </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public bool HasBehaviour(UIBehaviour.Name behaviourName) =>
            Behaviours.Any(b => b.behaviourName == behaviourName);

        /// <summary>
        /// Get the behaviour with the given name.
        /// Returns null if the behaviour has not been added (does not exist)
        /// </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public UIBehaviour GetBehaviour(UIBehaviour.Name behaviourName) =>
            Behaviours.FirstOrDefault(b => b.behaviourName == behaviourName);

        /// <summary>
        /// Set a new signal source for all behaviours
        /// </summary>
        /// <param name="target"> Signal source </param>
        public UIBehaviours SetSignalSource(GameObject target)
        {
            SignalSource = target;
            foreach (UIBehaviour behaviour in Behaviours)
                behaviour.SetSignalSource(target);
            return this;
        }

        /// <summary>
        /// Set target selectable for all behaviours
        /// </summary>
        /// <param name="uiSelectable"> Target selectable </param>
        /// <returns></returns>
        public UIBehaviours SetSelectable(UISelectable uiSelectable)
        {
            Selectable = uiSelectable;
            foreach (UIBehaviour behaviour in behaviours)
                behaviour.SetSelectable(selectable);
            return this;
        }

        /// <summary>
        /// Clear the target selectable for all behaviours
        /// </summary>
        public UIBehaviours ClearSelectable() =>
            SetSelectable(null);
    }
}
