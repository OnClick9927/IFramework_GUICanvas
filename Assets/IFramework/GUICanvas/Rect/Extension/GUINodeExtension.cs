/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Linq;
using System.Xml;

namespace IFramework.GUITool.RectDesign
{
    public static class GUINodeExtension
    {
        public static T Node<T>(this T t, GUINode element) where T : GUINode
        {
            element.parent = t;
            GUICanvas canvas = element.root as GUICanvas;
            if (canvas != null)
                canvas.TreeChange();
            return t;
        }
        public static void SaveXmlPrefab(this GUINode e, string path)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement ele = doc.CreateElement("Element_Prefab");
            ele.SetAttribute("Type", e.GetType().Name);
            ele.AppendChild(e.Serialize(doc));
            doc.AppendChild(ele);
            doc.Save(path);
        }
        public static void LoadXmlPrefab(this GUINode e, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            if (doc.DocumentElement.Name != "Element_Prefab") return;
            string attr = doc.DocumentElement.GetAttribute("Type");
            Type type = GUINodes.nodeTypes.Find((t) => { return t.Name == attr; });
            GUINode element = Activator.CreateInstance(type) as GUINode;
            element.DeSerialize(doc.FirstChild.FirstChild as XmlElement);
            e.Node(element);
        }
    }

}
