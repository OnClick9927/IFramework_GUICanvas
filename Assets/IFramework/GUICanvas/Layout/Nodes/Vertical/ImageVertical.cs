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
    [GUINode("Vertical/ImageVertical")]
    public class ImageVertical : ParentImageNode
    {
        public override GUIStyle style
        {
            get
            {
                if (_style == null)
                    _style = new GUIStyle(GUI.skin.label);
                return _style;
            }
            set { _style = new GUIStyle(value); }
        }
        public ImageVertical() : base() { }
        public ImageVertical(ImageVertical other) : base(other) { }

        protected override void OnGUI_Self()
        {

            GUILayout.BeginVertical(image, style, CalcGUILayOutOptions());
            OnGUI_Children();
            GUILayout.EndVertical();
        }
      
    }
}
