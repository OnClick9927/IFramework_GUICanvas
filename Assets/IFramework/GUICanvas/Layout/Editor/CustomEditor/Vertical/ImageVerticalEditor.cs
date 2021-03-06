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
    [CustomGUINodeAttribute(typeof(ImageVertical))]
    public class ImageVerticalEditor : ParentImageNodeEditor
    {
        private ImageVertical ele { get { return node as ImageVertical; } }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            GUILayout.BeginVertical(ele.image, ele.style, CalcGUILayOutOptions());
            if (child != null) child();

            GUILayout.EndVertical();
            EndGUI();
        }
    }
}
