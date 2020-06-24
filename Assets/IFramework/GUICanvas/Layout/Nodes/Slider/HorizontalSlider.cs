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
    [GUINode("Slider/Horizontal")]
    public class HorizontalSlider : SliderNode
    {
        public override GUIStyle slider
        {
            get
            {
                if (_slider == null)
                {
                    _slider = new GUIStyle(GUI.skin.horizontalSlider);
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
                    _thumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                }
                return _thumb;
            }
            set { _thumb = new GUIStyle(value); }
        }

        public HorizontalSlider() : base() { }
        public HorizontalSlider(SliderNode other) : base(other) { }

        protected override float DrawGUI()
        {
            return GUILayout.HorizontalSlider(value, startValue, endValue, slider, thumb, CalcGUILayOutOptions());
        }

       
    }
}
