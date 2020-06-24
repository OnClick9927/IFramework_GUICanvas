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
    [GUINode("Toggle/ImageToggle")]
    public class ImageToggle : ImageNode
    {
        public bool value;
        public Action<bool> onValueChange { get; set; }
        public override GUIStyle style
        {
            get
            {
                if (_style == null)
                    _style = new GUIStyle(GUI.skin.toggle);
                return _style;
            }
            set { _style = new GUIStyle(value); }
        }

        public ImageToggle() : base() { }
        public ImageToggle(ImageToggle other) : base(other)
        {
            value = other.value;
        }
        public override void Reset()
        {
            base.Reset();
            value = false;
        }
        protected override void OnGUI_Self()
        {
            bool tmp = DrawGUI();
            position = GUILayoutUtility.GetLastRect();
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }
        }

      

        protected virtual bool DrawGUI()
        {
            return GUILayout.Toggle(value, image, style, CalcGUILayOutOptions());
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "value", value);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "value", ref value);

        }
    }
}
