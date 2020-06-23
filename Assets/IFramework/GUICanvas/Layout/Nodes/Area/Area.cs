﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [GUINode("Area/Area")]
    public class Area : ImageArea
    {
        public string text;
        public string tooltip;

        public Area() : base() { }
        public Area(Area other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
        }
        public override void Reset()
        {
            base.Reset();
            text = tooltip = string.Empty;
        }


        protected override void OnGUI_Self()
        {

            GUILayout.BeginArea(areaRect, new GUIContent(text, image, tooltip), imageStyle);
            OnGUI_Children();
            GUILayout.EndArea();
        }
        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);
        }
    }
}
