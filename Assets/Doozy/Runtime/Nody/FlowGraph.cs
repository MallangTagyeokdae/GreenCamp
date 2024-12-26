// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Nody.Nodes.System;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Doozy.Runtime.Nody
{
    /// <summary> Scriptable object class used a container for nodes </summary>
    [CreateAssetMenu(menuName = "Doozy/Flow Graph", fileName = "Flow Graph", order = -1000)]
    public class FlowGraph : ScriptableObject
    {
        /// <summary> Pointer to the UIManagerInputSettings instance </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
        /// <summary> True is Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;
        /// <summary> Default player index value (used for global user) </summary>
        public static int defaultPlayerIndex => inputSettings.defaultPlayerIndex;

        [SerializeField]
        public Vector3 EditorPosition = Vector3.zero;
        /// <summary> Used by the editor to keep track of the graph position inside the Nody window </summary>
        internal Vector3 editorPosition
        {
            get => EditorPosition;
            set => EditorPosition = value;
        }

        [SerializeField]
        public Vector3 EditorScale = Vector3.one;
        /// <summary> Used by the editor to keep track of the graph scale (zoom) inside the Nody window </summary>
        internal Vector3 editorScale
        {
            get => EditorScale;
            set => EditorScale = value;
        }

        [SerializeField] private string Id;
        /// <summary> Flow Graph Id </summary>
        public string id
        {
            get => Id;
            set => Id = value;
        }

        [SerializeField] private string GraphName;
        /// <summary> Name of the graph </summary>
        public string graphName
        {
            get => GraphName;
            set => GraphName = value;
        }

        [SerializeField] private string GraphDescription;
        /// <summary> Description for the graph </summary>
        public string graphDescription
        {
            get => GraphDescription;
            set => GraphDescription = value;
        }

        [SerializeField] private bool IsSubGraph;
        /// <summary> Flag used to mark the graph as a sub-graph </summary>
        public bool isSubGraph
        {
            get => IsSubGraph;
            set => IsSubGraph = value;
        }

        [SerializeField] protected GraphState GraphState = GraphState.Idle;
        /// <summary> Current graph state </summary>
        public GraphState graphState
        {
            get => GraphState;
            set
            {
                GraphState = value;
                OnStateChanged?.Invoke(value);
            }
        }

        /// <summary> Triggered every time the graph changes its state </summary>
        public GraphStateEvent OnStateChanged = new GraphStateEvent();

        [SerializeField] private List<FlowNode> Nodes;
        /// <summary> All the nodes in the graph </summary>
        public List<FlowNode> nodes
        {
            get => Nodes;
            private set => Nodes = value;
        }

        /// <summary> All the global nodes in the graph </summary>
        public IEnumerable<FlowNode> globalNodes =>
            Nodes.Where(node => node.nodeType == NodeType.Global);

        [SerializeField] private FlowNode RootNode;
        /// <summary>
        /// The first node that becomes active.
        /// <para/> If this is a graph it will be a Start Node
        /// <para/> Id this is a sub-graph it will be an Enter Node </summary>
        public FlowNode rootNode
        {
            get => RootNode;
            set => RootNode = value;
        }

        [SerializeField] private FlowNode ActiveNode;
        /// <summary> The node that is currently active </summary>
        public FlowNode activeNode
        {
            get => ActiveNode;
            private set => ActiveNode = value;
        }

        /// <summary> The node that was previously active </summary>
        public FlowNode previousActiveNode { get; private set; }

        /// <summary> The port that lead to the previously active node </summary>
        public FlowPort previousActivePort { get; private set; }

        /// <summary> The sub-graph that is currently active (can be null) </summary>
        public FlowGraph activeSubGraph { get; private set; }

        /// <summary> The parent graph that contains this graph (if this is a sub-graph) (can be null) </summary>
        public FlowGraph parentGraph { get; private set; }

        /// <summary> All the input ports in the graph </summary>
        public List<FlowPort> inputPorts
        {
            get
            {
                var list = new List<FlowPort>();
                foreach (FlowNode node in Nodes)
                    list.AddRange(node.inputPorts);
                return list;
            }
        }

        /// <summary> All the output ports in the graph </summary>
        public List<FlowPort> outputPorts
        {
            get
            {
                var list = new List<FlowPort>();
                foreach (FlowNode node in Nodes)
                    list.AddRange(node.outputPorts);
                return list;
            }
        }

        /// <summary> All the ports in the graph (input and output) </summary>
        public List<FlowPort> ports
        {
            get
            {
                var list = new List<FlowPort>();
                foreach (FlowNode node in Nodes)
                {
                    list.AddRange(node.inputPorts);
                    list.AddRange(node.outputPorts);
                }
                return list;
            }
        }

        /// <summary> Current controller for the graph </summary>
        public FlowController controller { get; internal set; }

        /// <summary> Construct a new FlowGraph </summary>
        public FlowGraph()
        {
            Id = Guid.NewGuid().ToString();
            GraphName = ObjectNames.NicifyVariableName(nameof(FlowGraph));
            Nodes = new List<FlowNode>();
        }

        /// <summary> Reset the graph editor settings used inside the Nody window </summary>
        internal void ResetEditorSettings()
        {
            EditorPosition = Vector3.zero;
            EditorScale = Vector3.one;
        }

        /// <summary> Reset the graph </summary>
        public void ResetGraph()
        {
            ClearHistory();
            previousActiveNode = null;
            activeNode = null;
            nodes.ForEach(n => n.ResetNode());
            CleanGraph();
            graphState = GraphState.Idle;
        }

        private void CleanGraph()
        {
            nodes.ForEach(n =>
            {
                foreach (FlowPort inputPort in n.inputPorts)
                    foreach (string otherPortId in inputPort.connections.ToList()
                                 .Where(otherPortId => GetPortById(otherPortId) == null))
                        inputPort.connections.Remove(otherPortId);

                foreach (FlowPort outputPort in n.outputPorts)
                    foreach (string otherPortId in outputPort.connections.ToList()
                                 .Where(otherPortId => GetPortById(otherPortId) == null))
                        outputPort.connections.Remove(otherPortId);
            });
        }

        /// <summary> Activate the given node </summary>
        /// <param name="node"> Target node (next active node) </param>
        /// <param name="fromPort"> Port that activated this node (can be null) </param>
        public void SetActiveNode(FlowNode node, FlowPort fromPort = null)
        {
            if (node == null) return;
            if (activeNode != null)
                activeNode.OnExit();

            history.Push(new GraphHistory(previousActiveNode, previousActivePort, activeNode));
            previousActiveNode = activeNode;
            previousActivePort = fromPort;
            activeNode = node;

            activeNode.OnEnter();
            activeNode.Ping(FlowDirection.Forward);
            fromPort?.Ping(FlowDirection.Forward);
        }

        /// <summary> Go back to the previous node (if possible) by activating the node in the graph </summary>
        public void GoBack() =>
            GoBack(defaultPlayerIndex);

        /// <summary> Go back to the previous node (if possible) by activating the node in the graph </summary>
        /// <param name="playerIndex"> Player index </param>
        public void GoBack(int playerIndex)
        {
            if (history.Count == 0) return; //cannot go back

            //node type check -> block going back to special nodes
            switch (previousActiveNode)
            {
                case StartNode _:
                case EnterNode _:
                    return;
            }

            if
            (
                multiplayerMode &&                                    //check if multiplayer mode is enabled
                playerIndex != defaultPlayerIndex &&                  //check if default player index -> ignore player index
                controller.hasMultiplayerInfo &&                      //check if the controller is bound to a player index
                playerIndex != controller.multiplayerInfo.playerIndex //check player index
            )
                return;

            tempNodesSet.Clear();
            tempPortsSet.Clear();

            tempPortsSet.Add(previousActivePort);

            if (history.All(item => item.activeNode.passthrough))
                return; //cannot go back as there is no non-passthrough node in history

            GraphHistory peek = history.Peek();

            //multiple entries of the same node in history and passthrough check
            if (peek.activeNode == activeNode | peek.activeNode.passthrough) //we may have 1 or more nodes that are passthrough
            {
                while (history.Count > 0) //iterate through history
                {
                    peek = history.Peek();
                    if (peek.activeNode == activeNode) //we found the node we are currently clear the pings for the visual need to dig deeper
                    {
                        tempNodesSet.Clear(); //don't ping nodes 
                        tempPortsSet.Clear(); //don't ping ports
                        history.Pop();
                        continue;
                    }

                    if (peek.activeNode.passthrough)               //check if the node is passthrough 
                    {                                              //
                        tempNodesSet.Add(peek.activeNode);         //save node to be able to ping it (visuals matter)
                        tempPortsSet.Add(peek.previousActivePort); //save port to be able to ping it (visuals matter)
                        history.Pop();                             //pop history and go to the next entry
                        continue;
                    }

                    break; //found node that is NOT passthrough -> prepare to activate it
                }
            }

            if (history.Count == 0) return; //cannot go back

            if (activeNode != null)
                activeNode.OnExit();

            //ping nodes
            tempNodesSet.Remove(null); //remove null
            foreach (FlowNode flowNode in tempNodesSet)
                flowNode.Ping(FlowDirection.Back);

            //ping ports
            tempPortsSet.Remove(null); //remove null
            foreach (FlowPort flowPort in tempPortsSet)
                flowPort.Ping(FlowDirection.Back);

            previousActiveNode = history.Peek().previousActiveNode;
            previousActivePort = history.Peek().previousActivePort;
            activeNode = history.Peek().activeNode;
            history.Pop();

            activeNode.OnEnter();

            tempNodesSet.Clear();
            tempPortsSet.Clear();
        }

        /// <summary> Activate the first node with the given node name </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public void SetActiveNodeByNodeName(string nodeName) =>
            SetActiveNode(GetNodeByName(nodeName));

        /// <summary> Activate the node with the given node id </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public void SetActiveNodeByNodeId(string nodeId) =>
            SetActiveNode(GetNodeById(nodeId));

        /// <summary> Start the graph </summary>
        public void Start()
        {
            ResetGraph();
            UpdateNodes();
            StartGlobalNodes();
            SetActiveNode(RootNode);
            graphState = GraphState.Running;
        }

        /// <summary> Resume the graph </summary>
        public void Resume()
        {
            if (graphState != GraphState.Paused) //check if the graph is paused
                return;                          //graph is not paused -> do nothing
            graphState = GraphState.Running;     //set graph state to running
            StartGlobalNodes();                  //start global nodes
            if (activeNode != null)              //check if there is an active node
                activeNode.OnEnter();            //activate the active node
        }

        /// <summary> Pause the graph </summary>
        public void Pause()
        {
            if (graphState == GraphState.Paused) //check if the graph is paused
                return;                          //graph is paused -> do nothing
            graphState = GraphState.Paused;      //set graph state to paused
            StopGlobalNodes();                   //stop global nodes
            if (activeNode != null)              //check if there is an active node
                activeNode.OnExit();             //deactivate the active node
        }

        /// <summary> Toggle the graph state between running and paused </summary>
        /// <param name="paused"> If true, the graph will be paused, otherwise it will be resumed </param>
        public void SetPaused(bool paused)
        {
            if (paused)
            {
                Pause();
                return;
            }

            Resume();
        }

        /// <summary> Stop the graph </summary>
        public void Stop()
        {
            StopGlobalNodes();            //stop global nodes
            if (activeNode != null)       //check if there is an active node
                activeNode.OnExit();      //deactivate the active node
            activeNode = null;            //set active node to null
            graphState = GraphState.Idle; //set graph state to idle
        }

        /// <summary> Start all the global nodes inside the graph </summary>
        public void StartGlobalNodes()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < nodes.Count; i++) //iterate through nodes
            {
                FlowNode node = nodes[i];                       //get node
                if (node.nodeType != NodeType.Global) continue; //check if the node is global
                node.Start();                                   //start the node
            }

            if (activeSubGraph != null)            //check if there is an active sub graph
                activeSubGraph.StartGlobalNodes(); //start global nodes in the active sub graph
        }

        /// <summary> Stop all the global nodes inside the graph </summary>
        public virtual void StopGlobalNodes()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < nodes.Count; i++) //iterate through nodes
            {
                FlowNode node = nodes[i];                       //get node
                if (node.nodeType != NodeType.Global) continue; //check if the node is global
                node.Stop();                                    //stop the node
            }

            if (activeSubGraph != null)           //check if there is an active sub graph
                activeSubGraph.StopGlobalNodes(); //stop global nodes in the active sub graph
        }

        /// <summary> FixedUpdate is called every fixed framerate frame, if this flow has been loaded by a controller </summary>
        public void FixedUpdate()
        {
            if (graphState != GraphState.Running) //check if the graph is running
                return;                           //graph is not running -> return

            if (activeNode != null && activeNode.runFixedUpdate) //check if the active node is not null and if it should run fixed update
                activeNode.FixedUpdate();                        //run fixed update

            if (activeSubGraph != null)       //check if the active sub graph is not null
                activeSubGraph.FixedUpdate(); //run fixed update

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < nodes.Count; i++) //iterate through nodes
            {
                FlowNode node = nodes[i];                       //get node
                if (node.nodeType != NodeType.Global) continue; //check if node is global
                if (!node.runFixedUpdate) continue;             //check if node should run fixed update
                node.FixedUpdate();                             //run fixed update
            }
        }

        /// <summary> LateUpdate is called every frame, after all Update functions have been called and if this flow has been loaded by a controller </summary>
        public void LateUpdate()
        {
            if (graphState != GraphState.Running) //check if the graph is running
                return;                           //graph is not running -> return

            if (activeNode != null && activeNode.runLateUpdate) //check if the active node is running
                activeNode.LateUpdate();                        //run the active node

            if (activeSubGraph != null)      //check if the active subgraph is running
                activeSubGraph.LateUpdate(); //run the active subgraph

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < nodes.Count; i++) //iterate through all nodes
            {
                FlowNode node = nodes[i];                       //get the current node
                if (node.nodeType != NodeType.Global) continue; //check if the node is global
                if (node.runLateUpdate == false) continue;      //check if the node is running
                if (node.nodeState == NodeState.Idle) continue; //check if the node is idle
                node.LateUpdate();                              //run the node
            }
        }

        /// <summary> Update is called every frame, if this flow has been loaded by a controller </summary>
        public void Update()
        {
            if (graphState != GraphState.Running) //check if the graph is running
                return;                           //graph is not running -> return

            if (activeNode != null && activeNode.runUpdate) //check if the active node is running
                activeNode.Update();                        //run the active node

            if (activeSubGraph != null)  //check if the active subgraph is running
                activeSubGraph.Update(); //run the active subgraph


            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < nodes.Count; i++) //iterate through all nodes
            {
                FlowNode node = nodes[i];                       //get the current node
                if (node.nodeType != NodeType.Global) continue; //check if the node is global
                if (node.runUpdate == false) continue;          //check if the node is running
                if (node.nodeState == NodeState.Idle) continue; //check if the node is idle
                node.Update();                                  //run the node
            }
        }

        /// <summary> Refresh all the references for the graph's nodes </summary>
        public void UpdateNodes()
        {
            Nodes.RemoveNulls();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < nodes.Count; i++) //iterate through all nodes
                nodes[i].SetFlowGraph(this);      //set the flow graph for the node
        }

        /// <summary> Create a clone of this graph </summary>
        /// <returns> The cloned graph </returns>
        public FlowGraph Clone()
        {
            FlowGraph flowClone = Instantiate(this);
            flowClone.RootNode = RootNode.Clone().SetFlowGraph(flowClone);
            flowClone.nodes = nodes.ConvertAll(n => n.Clone());
            flowClone.UpdateNodes();
            return flowClone;
        }

        /// <summary> True if the graph contains the given node </summary>
        /// <param name="node"> Node to search for </param>
        public bool ContainsNode(FlowNode node) =>
            node != null && nodes.Contains(node);

        /// <summary> True if the graph contains a node with the given id </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public bool ContainsNodeById(string nodeId) =>
            nodes.Any(node => node.nodeId.Equals(nodeId));

        /// <summary> True if the graph contains a node with the given node name </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public bool ContainsNodeByName(string nodeName) =>
            nodes.Any(node => node.nodeName.Equals(nodeName));

        /// <summary>
        /// Get the StartNode if this is not a sub graph.
        /// If not found, this method returns null
        /// </summary>
        public StartNode GetStartNode() =>
            (StartNode)nodes.FirstOrDefault(n => n is StartNode);

        /// <summary>
        /// Get the EnterNode if this is a sub graph.
        /// If not found, this method returns null
        /// </summary>
        public EnterNode GetEnterNode() =>
            (EnterNode)nodes.FirstOrDefault(n => n is EnterNode);

        /// <summary>
        /// Get the ExitNode if this is a sub graph.
        /// If not found, this method returns null
        /// </summary>
        public ExitNode GetExitNode() =>
            (ExitNode)nodes.FirstOrDefault(n => n is ExitNode);

        /// <summary>
        /// Get the first node with the given node name.
        /// If one is not found, this method returns null
        /// </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public FlowNode GetNodeByName(string nodeName) =>
            nodes.FirstOrDefault(node => node.nodeName.Equals(nodeName));

        /// <summary>
        /// Get the node with the given node id.
        /// If one is not found, this method returns null
        /// </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public FlowNode GetNodeById(string nodeId) =>
            nodes.FirstOrDefault(node => node.nodeId.Equals(nodeId));

        /// <summary> Get all the nodes of the given type </summary>
        /// <typeparam name="T"> Type of node </typeparam>
        public List<T> GetNodeByType<T>() where T : FlowNode =>
            (List<T>)nodes.Where(node => node is T);

        /// <summary>
        /// Get the port with the given port id.
        /// If one is not found, this method returns null
        /// </summary>
        /// <param name="portId"> Port id to search for </param>
        public FlowPort GetPortById(string portId) =>
            nodes.Select(node => node.GetPortFromId(portId)).FirstOrDefault(port => port != null);

        #region Graph History and Cats

        private Stack<GraphHistory> history { get; set; }
        private HashSet<FlowNode> tempNodesSet { get; set; }
        private HashSet<FlowPort> tempPortsSet { get; set; }

        //    |\__/,|   (`\
        //  _.|o o  |_   ) )
        //-(((---(((--------
        /// <summary>
        /// Clear graph history and remove the possibility of being able to go back to previously active nodes
        /// </summary>
        public void ClearHistory()
        {
            history ??= new Stack<GraphHistory>();
            tempNodesSet ??= new HashSet<FlowNode>();
            tempPortsSet ??= new HashSet<FlowPort>();

            history.Clear();
        }

        //                /)
        //       /\___/\ ((
        //       \`@_@'/  ))
        //       {_:Y:.}_//
        //-------{_}^-'{_}---------
        private struct GraphHistory
        {
            public FlowNode previousActiveNode { get; set; }
            public FlowPort previousActivePort { get; set; }
            public FlowNode activeNode { get; set; }

            public GraphHistory(FlowNode previousActiveNode, FlowPort previousActivePort, FlowNode activeNode)
            {
                this.previousActiveNode = previousActiveNode;
                this.previousActivePort = previousActivePort;
                this.activeNode = activeNode;
            }
        }

        #endregion
    }
}
