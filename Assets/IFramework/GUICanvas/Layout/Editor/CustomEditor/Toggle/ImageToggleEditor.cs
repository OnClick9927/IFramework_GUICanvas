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
    [CustomGUINodeAttribute(typeof(ImageToggle))]
    public class ImageToggleEditor : ImageNodeEditor
    {
        private ImageToggle toggle { get { return node as ImageToggle; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Toggle", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.Toggle("Value", ref toggle.value);
        }
        public override void OnSceneGUI(Action child)
        {
            if (!toggle.active) return;
            BeginGUI();
            toggle.value = GUILayout.Toggle(toggle.value, toggle.image, toggle.style, CalcGUILayOutOptions());
            toggle.position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
