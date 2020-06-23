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
    [GUINode("Text/Label")]
    public class Label : TextNode
    {
        public Label() : base() { }
        public Label(TextNode other) : base(other) { }
        protected override void OnGUI_Self()
        {
            base.OnGUI_Self();
            GUILayout.Label(new GUIContent(text, tooltip), textStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();

        }
     
    }
}
