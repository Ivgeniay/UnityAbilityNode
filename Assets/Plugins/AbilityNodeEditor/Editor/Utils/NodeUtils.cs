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
    internal static class NodeUtils
    {
        internal static NodeAbilityGraph CreateNewGraph(string name)
        {
            NodeAbilityGraph curGraph = ScriptableObject.CreateInstance<NodeAbilityGraph>();
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
        internal static void LoadGraph()
        {
            NodeAbilityGraph curGraph = null;
            string grathPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + "/Plugins/AbilityNodeEditor/Database", "asset");
            int appPathLen = Application.dataPath.Length;
            int assetPathLen = "Asset/".Length;
            string finalPath = grathPath.Substring(appPathLen - assetPathLen);

            curGraph = AssetDatabase.LoadAssetAtPath<NodeAbilityGraph>(finalPath);
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

        internal static void CreateNode(NodeAbilityGraph nodeGraph, NodeType nodeType, Vector2 mousePos)
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


        internal static void UnloadGraph()
        {
            NodeEditorWindow curWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            curWindow?.UnloadGraph();
        }

        internal static void DrawLine(Vector3 pointA, Vector3 pointB)
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawLine(pointA, pointB);
            Handles.EndGUI();
        }

        internal static void DrawLineBetween(NodeInput nodeInput, NodeOutput nodeOutput)
        {
            DrawLine(nodeInput.PointConnection, nodeOutput.PointConnection);
            //new Vector3(nodeOutput.Position.x + 24, nodeOutput.Position.y + 12, 0), 
            //new Vector3(nodeInput.Position.x, nodeInput.Position.y + 12, 0));
        }

        internal static void DrawLineToInput(BaseNode from, NodeInput nodeInput)
        {
            //DrawLine(nodeInput.PointConnection, new Vector3(from.NodeRect.x - 24f, from.NodeRect.y + from.NodeRect.height / 3 - 12f));

            NodeUtils.DrawLine(new Vector3(nodeInput.InputNode.NodeRect.x + nodeInput.InputNode.NodeRect.width + 24f,
                                    nodeInput.InputNode.NodeRect.y + nodeInput.InputNode.NodeRect.height / 2f - 12f,
                                    0f),
                                new Vector3(from.NodeRect.x - 24f, from.NodeRect.y + from.NodeRect.height / 3 - 12f));
        }

        internal static Vector3 GetNodeConnectionPosition(Vector3 nodePosition, ConnectionNodeSide nodeSide)
        {
            switch (nodeSide)
            {
                case ConnectionNodeSide.Right:
                    return new Vector3(nodePosition.x + 24, nodePosition.y + 12, 0);

                default: return Vector3.zero;
            }
            
        }

        internal static void DeleteNode(NodeAbilityGraph nodeGraph, int nodeNumber) => DeleteNode(nodeGraph, nodeGraph.Nodes[nodeNumber]);
        internal static void DeleteNode(NodeAbilityGraph nodeGraph, BaseNode node)
        {
            if (!nodeGraph) throw new NullReferenceException();
            if (!node) throw new NullReferenceException();
            nodeGraph.RemoveNode(node);
        }

        internal static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(viewRect.height / gridSpacing);

            Handles.BeginGUI();

            Handles.color = gridColor;
            for (int x = 0; x < widthDivs; x++)
                Handles.DrawLine(new Vector3(gridSpacing * x, 0f, 0f), new Vector3(gridSpacing * x, viewRect.height, 0f)); 
            for (int y = 0; y < widthDivs; y++)
                Handles.DrawLine(new Vector3(0, gridSpacing * y, 0f), new Vector3(viewRect.width, gridSpacing * y, 0f));
            Handles.color = Color.white;

            Handles.EndGUI();
        }
         
        internal static void DrawFloatProperty(string textLabel, ref float @float)
        {
            EditorGUILayout.BeginVertical();
            @float = EditorGUILayout.FloatField(textLabel, @float);
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        internal static void DrawBoolProperty(string textLabel, ref bool @bool, Func<bool> predicate = null)
        {

            EditorGUILayout.BeginVertical();
            var result = EditorGUILayout.Toggle(textLabel, @bool);

            if (predicate == null || predicate.Invoke()) @bool = result;
            else @bool = false;
            
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        internal static void DrawTextProperty(string textLabel, ref string text)
        {
            EditorGUILayout.BeginVertical();
            text = EditorGUILayout.TextField(textLabel, text);
            GUILayout.Space(2);
            EditorGUILayout.EndVertical(); 
        }


    }

    internal enum ConnectionNodeType
    {
        Input,
        Output
    }
    internal enum ConnectionNodeSide
    {
        Up,
        Right,
        Down,
        Left
    }
}