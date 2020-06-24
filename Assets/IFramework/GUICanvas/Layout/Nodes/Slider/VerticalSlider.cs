/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [GUINode("Slider/Vertical")]
    public class VerticalSlider : SliderNode
    {
        public override GUIStyle slider
        {
            get
            {
                if (_slider == null)
                {
                    _slider = new GUIStyle(GUI.skin.verticalSlider);
                }
                return _slider;
            }
            set { _slider = new GUIStyle(value); }
        }
        public override GUIStyle thumb
        {
            get
            {
                if (_thumb == null)
                {
                    _thumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                }
                return _thumb;
            }
            set { _thumb = new GUIStyle(value); }
        }

        public VerticalSlider() : base() { }
        public VerticalSlider(SliderNode other) : base(other) { }

        protected override float DrawGUI()
        {
            return GUILayout.VerticalSlider(value, startValue, endValue, slider, thumb, CalcGUILayOutOptions());
        }
    }
}
