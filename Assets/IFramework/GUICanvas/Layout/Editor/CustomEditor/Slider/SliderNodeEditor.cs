/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.GUITool.LayoutDesign
{
    [CustomGUINodeAttribute(typeof(SliderNode))]
    public class SliderNodeEditor : GUINodeEditor
    {
        private SliderNode slider { get { return node as SliderNode; } }
        private bool insFold = true;
        private GUIStyleDesign thumbDrawer;
        private GUIStyleDesign sliderDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (sliderDrawer == null)
                sliderDrawer = new GUIStyleDesign(slider.slider, "Slider Style");
            if (thumbDrawer == null)
                thumbDrawer = new GUIStyleDesign(slider.thumb, "Thumb Style");
            insFold = FormatFoldGUI(insFold, "Slider", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.FloatField("Value", ref slider.value)
                .FloatField("Start Value", ref slider.startValue)
                .FloatField("End Value", ref slider.endValue);
            sliderDrawer.OnGUI();
            thumbDrawer.OnGUI();
        }
    }
}
