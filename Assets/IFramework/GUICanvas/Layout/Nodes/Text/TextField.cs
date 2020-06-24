﻿/*********************************************************************************
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
    [GUINode("Text/TextField")]
    public class TextField : TextNode
    {
        public Action<string> onValueChange { get; set; }

        public override GUIStyle style
        {
            get
            {
                if (_style == null)
                    _style = new GUIStyle(GUI.skin.textField);
                return _style;
            }
            set { _style = new GUIStyle(value); }
        }
        public TextField() : base() { }
        public TextField(TextNode other) : base(other) { }
        protected override void OnGUI_Self()
        {
            base.OnGUI_Self();
            string tmp = GUILayout.TextField(text, style, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            if (tmp != text)
            {
                text = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }

        }
      
    }
}
