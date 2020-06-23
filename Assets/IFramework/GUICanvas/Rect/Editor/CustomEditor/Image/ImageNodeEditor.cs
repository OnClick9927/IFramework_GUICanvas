/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.GUITool.RectDesign
{
    [CustomGUINode(typeof(ImageNode))]
    public class ImageNodeEditor : GUINodeEditor
    {
        private ImageNode image { get { return element as ImageNode; } }
        private bool insFold = true;
        private GUIStyleEditor imageStyleDrawer;



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (imageStyleDrawer == null)
                imageStyleDrawer = new GUIStyleEditor(image.imageStyle, "Image Style");
            insFold = FormatInspectorHeadGUI(insFold, "Image Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref image.image, false);
            imageStyleDrawer.OnGUI();
        }
    }
}
