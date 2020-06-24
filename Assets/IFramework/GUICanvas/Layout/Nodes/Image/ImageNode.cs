/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    public abstract class ImageNode : GUINode
    {
        public Texture2D image;
        protected GUIStyle _style;
        public virtual GUIStyle style
        {
            get
            {
                if (_style == null)
                    _style = new GUIStyle(GUI.skin.box);
                return _style;
            }
            set { _style = new GUIStyle(value); }
        }

        protected ImageNode() : base() { }
        protected ImageNode(ImageNode other) : base(other)
        {
            image = other.image;
            _style = new GUIStyle(other._style);
        }
        public override void Reset()
        {
            base.Reset();
            image = null;
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            root.AppendChild(new GUIStyleSerializer(style, "Image Style").Serializate(doc));
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());

            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            _style = new GUIStyle();

            new GUIStyleSerializer(style, "Image Style").DeSerializate(styleE);
        }
    }

}
