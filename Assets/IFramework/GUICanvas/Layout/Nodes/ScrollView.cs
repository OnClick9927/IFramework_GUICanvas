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
    [GUINode("ScrollView")]
    public class ScrollView : ParentGUINode
    {
        public bool alwaysShowHorizontal;
        public bool alwaysShowVertical;
        public Vector2 value;
        public Action<Vector2> onValueChange { get; set; }
        private GUIStyle _Hstyle;
        private GUIStyle _Vstyle;

        public GUIStyle Hstyle
        {
            get
            {
                if (_Hstyle == null)
                    _Hstyle = new GUIStyle(GUI.skin.horizontalScrollbar);
                return _Hstyle;
            }
            set { _Hstyle = new GUIStyle(value); }
        }
        public GUIStyle Vstyle
        {
            get
            {
                if (_Vstyle == null)
                    _Vstyle = new GUIStyle(GUI.skin.verticalScrollbar);
                return _Vstyle;
            }
            set { _Vstyle = new GUIStyle(value); }
        }
        private GUIStyleSerializer HstyleDrawer;
        private GUIStyleSerializer VstyleDrawer;

        public ScrollView() : base() { }
        public ScrollView(ScrollView other) : base(other)
        {
            alwaysShowHorizontal = other.alwaysShowHorizontal;
            alwaysShowVertical = other.alwaysShowVertical;
            value = other.value;
            _Hstyle = new GUIStyle(other._Hstyle);
            _Vstyle = new GUIStyle(other._Vstyle);
        }
        public override void Reset()
        {
            base.Reset();
            value = Vector2.zero;
            alwaysShowHorizontal = alwaysShowVertical = false;
        }
        protected override void OnGUI_Self()
        {
            Vector2 tmp = GUILayout.BeginScrollView(value, alwaysShowHorizontal, alwaysShowVertical, Hstyle, Vstyle, CalcGUILayOutOptions());
            OnGUI_Children();
            GUILayout.EndScrollView();
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null)
                    onValueChange(value);
            }
        }


        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "alwaysShowHorizontal", alwaysShowHorizontal);
            SerializeField(root, "alwaysShowVertical", alwaysShowVertical);
            SerializeField(root, "value", value);
            XmlElement stylesE = doc.CreateElement("Styles");
            stylesE.AppendChild(new GUIStyleSerializer(Hstyle, "Hrizontal Style").Serializate(doc));
            stylesE.AppendChild(new GUIStyleSerializer(Vstyle, "Vertical Style").Serializate(doc));
            root.AppendChild(stylesE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "alwaysShowHorizontal", ref alwaysShowHorizontal);
            DeSerializeField(root, "alwaysShowVertical", ref alwaysShowVertical);
            DeSerializeField(root, "value", ref value);
            _Hstyle = new GUIStyle();
            _Vstyle = new GUIStyle();


            XmlElement styleE = root.SelectSingleNode("Styles") as XmlElement;
            new GUIStyleSerializer(Hstyle, "Hrizontal Style").DeSerializate(styleE.FirstChild as XmlElement);
            new GUIStyleSerializer(Vstyle, "Vertical Style").DeSerializate(styleE.LastChild as XmlElement);
        }

    }
}
