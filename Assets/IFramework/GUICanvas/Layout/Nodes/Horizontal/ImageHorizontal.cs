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
    [GUINode("Horizontal/ImageHorizontal")]
    public class ImageHorizontal : ParentImageNode
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        public ImageHorizontal() : base() { }
        public ImageHorizontal(ImageHorizontal other) : base(other) { }

        protected override void OnGUI_Self()
        {

            GUILayout.BeginHorizontal(image, imageStyle, CalcGUILayOutOptions());
            OnGUI_Children();
            GUILayout.EndHorizontal();
        }

    }
}
