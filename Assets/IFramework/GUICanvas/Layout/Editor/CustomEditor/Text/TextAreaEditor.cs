/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [CustomGUINodeAttribute(typeof(TextArea))]
    public class TextAreaEditor : TextNodeEditor
    {
        private TextArea textElement { get { return node as TextArea; } }

        private GUIStyleDesign textStyleDrawer;
        public override void OnSceneGUI(Action children)
        {
            base.OnSceneGUI(children);

            if (!node.active) return;

            BeginGUI();
            textElement.text = GUILayout.TextArea(textElement.text, textElement.textStyle, CalcGUILayOutOptions());
            textElement.position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
