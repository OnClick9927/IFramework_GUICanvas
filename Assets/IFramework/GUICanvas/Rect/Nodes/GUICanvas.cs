/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-11-04
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    public class GUICanvas : GUINode
    {
        public event Action OnCanvasTreeChange;
        public void TreeChange()
        {
            if (OnCanvasTreeChange != null)
                OnCanvasTreeChange();
        }
        public Rect canvasRect;
        public override Rect position { get { return new Rect(Vector2.zero, canvasRect.size); } }


        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            OnGUI_Self();
            EndGUI();
        }
        protected override void OnGUI_Self()
        {
            GUI.BeginClip(canvasRect);
            OnGUI_Children();
            GUI.EndClip();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root= base.Serialize(doc);
            SerializeField(root, "canvasRect", canvasRect);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "canvasRect", ref canvasRect);
        }
    }
}
