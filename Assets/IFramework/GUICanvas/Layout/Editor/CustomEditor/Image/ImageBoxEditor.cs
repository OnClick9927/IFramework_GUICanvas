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
    [CustomGUINodeAttribute(typeof(ImageBox))]
    public class ImageBoxEditor : ImageNodeEditor
    {
        private ImageBox image { get { return node as ImageBox; } }
        public override void OnSceneGUI(Action child)
        {
            if (!image.active) return;
            BeginGUI();
            GUILayout.Box(image.image, image.style, CalcGUILayOutOptions());
            image.position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
