/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-10-09
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;

namespace IFramework.GUITool.LayoutDesign
{
    public class GUICanvas : Area
    {
        public event Action OnCanvasTreeChange;
        public void TreeChange()
        {
            if (OnCanvasTreeChange != null)
                OnCanvasTreeChange();
        }
        public Rect canvasRect;
        public override Rect position { get { return canvasRect; } set { areaRect = value; canvasRect = value; } }
        public GUICanvas(GUICanvas other) : base(other) { }
        public GUICanvas() : base() { }
 
        protected override void OnGUI_Self()
        {
            GUILayout.BeginArea(position, image, imageStyle);
           OnGUI_Children();
            GUILayout.EndArea();

        }
     
        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
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
