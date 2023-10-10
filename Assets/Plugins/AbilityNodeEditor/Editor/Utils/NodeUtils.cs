using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityNodeEditor;
using static AbilityNodeEditor.BaseNode;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AbilityNodeEditor
{
    public static class NodeUtils
    {
        public static NodeGraph CreateNewGraph(string name)
        {
            NodeGraph curGraph = ScriptableObject.CreateInstance<NodeGraph>();
            if (curGraph)
            {
                curGraph.GraphName = name;
                curGraph.InitGraph();

                AssetDatabase.CreateAsset(curGraph, "Assets/Plugins/AbilityNodeEditor/Database/" + curGraph.GraphName + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                NodeEditorWindow curWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                curWindow?.SetGraph(curGraph);
            }
            else
            {
                EditorUtility.DisplayDialog("Node Message", "Unable to create new graph. Call motleycrue6502@gmail.com", "OK");
            }

            return curGraph;

        }
        public static void LoadGraph()
        {
            NodeGraph curGraph = null;
            string grathPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + "/Plugins/AbilityNodeEditor/Database", "asset");
            int appPathLen = Application.dataPath.Length;
            int assetPathLen = "Asset/".Length;
            string finalPath = grathPath.Substring(appPathLen - assetPathLen);

            curGraph = AssetDatabase.LoadAssetAtPath<NodeGraph>(finalPath);
            if (curGraph != null)
            {
                NodeEditorWindow curWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                curWindow?.SetGraph(curGraph);
            }
            else
            {
                EditorUtility.DisplayDialog("Node Message", "Unable to load this asset. Cast error.", "OK");
            }
        }

        public static void CreateNode(NodeGraph nodeGraph, NodeType nodeType, Vector2 mousePos)
        {
            if (nodeGraph != null)
            {
                BaseNode node = null;
                switch (nodeType)
                {
                    case NodeType.AbilityNode:
                        node = ScriptableObject.CreateInstance<AbilityNode>();
                        node.NodeName = "Ability node";
                        break;

                    case NodeType.RootAbilityNode:
                        node = ScriptableObject.CreateInstance<RootAbilityNode>();
                        node.NodeName = "Ability root node";
                        break;
                }

                if (node != null) {
                    node.InitNode();
                    node.NodeRect.x = mousePos.x;
                    node.NodeRect.y = mousePos.y;
                    node.parentGraph = nodeGraph;

                    nodeGraph.AddNode(node);
                }
            }
        }


        public static void UnloadGraph()
        {
            NodeEditorWindow curWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            curWindow?.UnloadGraph();
        }

        public static void DrawLine(Vector3 pointA, Vector3 pointB)
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawLine(pointA, pointB);
            Handles.EndGUI();
        }

        public static void DrawLineBetween(NodeInput nodeInput, NodeOutput nodeOutput)
        {
            DrawLine(nodeInput.PointConnection, nodeOutput.PointConnection);
            //new Vector3(nodeOutput.Position.x + 24, nodeOutput.Position.y + 12, 0), 
            //new Vector3(nodeInput.Position.x, nodeInput.Position.y + 12, 0));
        }

        public static void DrawLineToInput(BaseNode from, NodeInput nodeInput)
        {
            //DrawLine(nodeInput.PointConnection, new Vector3(from.NodeRect.x - 24f, from.NodeRect.y + from.NodeRect.height / 3 - 12f));

            NodeUtils.DrawLine(new Vector3(nodeInput.InputNode.NodeRect.x + nodeInput.InputNode.NodeRect.width + 24f,
                                    nodeInput.InputNode.NodeRect.y + nodeInput.InputNode.NodeRect.height / 2f - 12f,
                                    0f),
                                new Vector3(from.NodeRect.x - 24f, from.NodeRect.y + from.NodeRect.height / 3 - 12f));
        }

        public static Vector3 GetNodeConnectionPosition(Vector3 nodePosition, ConnectionNodeSide nodeSide)
        {
            switch (nodeSide)
            {
                case ConnectionNodeSide.Right:
                    return new Vector3(nodePosition.x + 24, nodePosition.y + 12, 0);

                default: return Vector3.zero;
            }
            
        }

        public static void DeleteNode(NodeGraph nodeGraph, int nodeNumber) => DeleteNode(nodeGraph, nodeGraph.Nodes[nodeNumber]);
        public static void DeleteNode(NodeGraph nodeGraph, BaseNode node)
        {
            if (!nodeGraph) throw new NullReferenceException();
            if (!node) throw new NullReferenceException();
            nodeGraph.RemoveNode(node);
        }
    }

    public enum ConnectionNodeType
    {
        Input,
        Output
    }
    public enum ConnectionNodeSide
    {
        Up,
        Right,
        Down,
        Left
    }
}