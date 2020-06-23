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
    [GUINode("Image/ImageBox")]
    public class ImageBox : ImageNode
    {
        public ImageBox() : base() { }
        public ImageBox(ImageBox other) : base(other) { }
        protected override void OnGUI_Self()
        {

            GUILayout.Box(image, imageStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
        }
      
    }
}
