/*********************************************************************************
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
    [GUINode("Space")]
    public class Space : GUINode
    {
        public float pixels;

        public Space() : base() { }
        public Space(Space other) : base(other)
        {
            pixels = other.pixels;
        }
        public override void Reset()
        {
            base.Reset();
            pixels = 0;
        }
        protected override void OnGUI_Self()
        {
            GUILayout.Space(pixels);
            position = GUILayoutUtility.GetLastRect();
        }
       
        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "pixels", pixels);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "pixels", ref pixels);
        }
    }
}
