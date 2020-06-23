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

namespace IFramework.GUITool.RectDesign
{
    class GUINodeInspectorView : ILayoutGUIDrawer
    {
        private Dictionary<Type, GUINodeEditor> dic = new Dictionary<Type, GUINodeEditor>();
        private GUINodeEditor Pan;
        private Vector2 scroll2;

        public GUINodeInspectorView()
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

            GUINodeSelection.onNodeChange += (ele) =>
            {
                if (ele != null)
                {
                    Pan = dic[ele.GetType()];
                    Pan.element = ele;
                }
            };
        }
        public void OnGUI(Rect rect)
        {
            if (Pan == null) return;
            this.BeginArea(rect)
                    .BeginScrollView(ref scroll2)
                        .Pan(Pan.OnInspectorGUI)
                    .LayoutEndScrollView()
                .EndArea();
        }
    }
}
