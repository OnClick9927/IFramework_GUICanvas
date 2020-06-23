/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-11-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    class GUINodeSelection
    {
        public delegate void OnNodeChange(GUINode node);
        public static event OnNodeChange onNodeChange;
        private static GUINode _node;
        public static GUINode node
        {
            get { return _node; }
            set
            {
                _node = value;

                if (UnityEditor.EditorWindow.focusedWindow != null)
                    UnityEditor.EditorWindow.focusedWindow.Repaint();
                if (Event.current != null && Event.current.type != EventType.Layout)
                    Event.current.Use();
                if (onNodeChange != null)
                    onNodeChange(_node);
            }
        }

        public static GUINode copyNode { get; set; }
        public static GUINode dragNode { get; set; }
    }
}
