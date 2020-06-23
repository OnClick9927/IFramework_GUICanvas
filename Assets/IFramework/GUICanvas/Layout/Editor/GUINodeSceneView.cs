/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    class GUINodeSceneView : ILayoutGUIDrawer
    {
        private Dictionary<Type, GUINodeEditor> dic = new Dictionary<Type, GUINodeEditor>();
        public GUICanvas canvas;
        public GUINodeSceneView()
        {
            var designs = GUINodeEditors.editorTypes.FindAll((t) => { return t.IsDefined(typeof(CustomGUINodeAttribute), false); });
            var eles = GUINodes.nodeTypes;
            foreach (var type in eles)
            {
                var typeTree = type.GetTypeTree();
                for (int i = 0; i < typeTree.Count; i++)
                {
                    Type des = designs.Find((t) => {
                        return (t.GetCustomAttributes(typeof(CustomGUINodeAttribute), false).First() as CustomGUINodeAttribute).EditType == typeTree[i];
                    });
                    if (des != null)
                    {
                        dic.Add(type, Activator.CreateInstance(des) as GUINodeEditor);
                        break;

                    }
                }
            }
        }
        public void OnGUI(Rect rect)
        {
            EleGUI(canvas);
        }
        public void EleGUI(GUINode ele)
        {
            GUINodeEditor des = dic[ele.GetType()];
            des.node = ele;
            des.OnSceneGUI(() => {
                for (int i = 0; i < ele.Children.Count; i++)
                {
                    EleGUI(ele.Children[i] as GUINode);
                }
            });

        }

    }

}
