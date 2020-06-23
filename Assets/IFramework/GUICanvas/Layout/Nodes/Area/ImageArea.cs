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

    [GUINode("Area/ImageArea")]
    public class ImageArea : ParentImageNode
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }
        public Rect areaRect;
        public override Rect position { get { return areaRect; } set { areaRect = value; } }


        public ImageArea() : base() { }
        public ImageArea(ImageArea other) : base(other) { areaRect = other.areaRect; }
        public override void Reset()
        {
            base.Reset();
            areaRect = new Rect(0, 0, 100, 100);
        }



        protected override void OnGUI_Self()
        {

            GUILayout.BeginArea(areaRect, image, imageStyle);
            OnGUI_Children();
            GUILayout.EndArea();

        }



        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "areaRect", areaRect);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            Rect _areaRect = Rect.zero;
            DeSerializeField(root, "areaRect", ref _areaRect);
            areaRect = _areaRect;
        }
    }
}
