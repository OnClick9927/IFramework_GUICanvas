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
    [CustomGUINodeAttribute(typeof(ImageLabel))]
    public class ImageLabelEditor : ImageNodeEditor
    {
        private ImageLabel image { get { return node as ImageLabel; } }
        public override void OnSceneGUI(Action child)
        {
            if (!image.active) return;
            BeginGUI();
            GUILayout.Label(image.image, image.style, CalcGUILayOutOptions());
            image.position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
