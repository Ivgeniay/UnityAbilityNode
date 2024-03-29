using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AbilityNodeEditor
{
    [Serializable]
    internal abstract class BaseView 
    {
        internal string ViewTitle { get; set; }
        internal Rect ViewRect;

        private string defaultViewTitle;
        protected GUISkin viewSkin;
        protected AbilityNodeGraph curGraph;

        internal BaseView(string title)
        {
            defaultViewTitle = title;
            ViewTitle = title;
            viewSkin = GetEditorSkin();
        }

        internal virtual void OnChangeGraphHandler(AbilityNodeGraph nodeGraph)
        {
            curGraph = nodeGraph;
            if (curGraph) ViewTitle = curGraph.GraphName + " " + defaultViewTitle; 
            else ViewTitle = "No Graph";
        }

        internal virtual void UpdateView(Rect editorRect, Rect precentageRect, Event e, AbilityNodeGraph nodeGraph)
        {
            if (viewSkin == null) 
            {
                GetEditorSkin();
                return;
            }

            ViewRect = new Rect(editorRect.x * precentageRect.x,
                                editorRect.y * precentageRect.y, 
                                editorRect.width * precentageRect.width,
                                editorRect.height * precentageRect.height);
        }

        internal virtual void ProcessEvents(Event e) { }
        protected GUISkin GetEditorSkin() => Resources.Load<GUISkin>("GUISkins/EditorSkins/NodeEditorSkin");
        

    }
}
