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
    public abstract class TextNode : GUINode
    {
        public string text;
        public string tooltip;
        public Font font;
        public FontStyle fontStyle;
        public int fontSize;
        public TextAnchor alignment;
        public TextClipping overflow;
        public bool richText;

        protected GUIStyle _style;
        public virtual GUIStyle style
        {
            get
            {
                if (_style == null)
                    _style = new GUIStyle(GUI.skin.label);
                return _style;
            }
            set { _style = new GUIStyle(value); }
        }

        protected TextNode() : base() { }
        protected TextNode(TextNode other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
            font = other.font;
            fontStyle = other.fontStyle;
            fontSize = other.fontSize;
            alignment = other.alignment;
            overflow = other.overflow;
            richText = other.richText;
            _style = new GUIStyle(other._style);
        }
        public override void Reset()
        {
            base.Reset();
            text = string.Empty;
            tooltip = string.Empty;
            alignment = TextAnchor.MiddleLeft;
            font = null;
            fontSize = 10;
            richText = true;
            overflow = TextClipping.Clip;
            fontStyle = FontStyle.Normal;
        }
        protected override void OnGUI_Self()
        {
            style.font = font;
            style.fontStyle = fontStyle;
            style.fontSize = fontSize;
            style.alignment = alignment;
            style.clipping = overflow;
            style.richText = richText;
        }
       

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "fontStyle", fontStyle);
            SerializeField(root, "fontSize", fontSize);
            SerializeField(root, "alignment", alignment);
            SerializeField(root, "overflow", overflow);
            SerializeField(root, "richText", richText);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            root.AppendChild(new GUIStyleSerializer(style, "Text Style").Serializate(doc));
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "fontStyle", ref fontStyle);
            DeSerializeField(root, "fontSize", ref fontSize);
            DeSerializeField(root, "alignment", ref alignment);
            DeSerializeField(root, "overflow", ref overflow);
            DeSerializeField(root, "richText", ref richText);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);

            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            _style = new GUIStyle();

            new GUIStyleSerializer(style, "Text Style").DeSerializate(styleE);
        }
    }

}
