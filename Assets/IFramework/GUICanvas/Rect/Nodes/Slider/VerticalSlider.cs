﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    [GUINode("Slider/Vertical")]
    public class VerticalSlider : SliderNode
    {
        public override GUIStyle slider
        {
            get
            {
                if (m_slider == null)
                {
                    m_slider = new GUIStyle(GUI.skin.verticalSlider);
                }
                return m_slider;
            }
            set { m_slider = new GUIStyle(value); }
        }
        public override GUIStyle thumb
        {
            get
            {
                if (m_thumb == null)
                {
                    m_thumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                }
                return m_thumb;
            }
            set { m_thumb = new GUIStyle(value); }
        }

        public VerticalSlider() : base() { }
        public VerticalSlider(SliderNode other) : base(other) { }

        protected override float DrawGUI()
        {
            return GUI.VerticalSlider(position, value, startValue, endValue, slider, thumb);
        }
    }
}
