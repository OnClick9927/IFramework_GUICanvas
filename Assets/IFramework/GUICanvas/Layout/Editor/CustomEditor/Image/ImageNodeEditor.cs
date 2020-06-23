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
    [CustomGUINodeAttribute(typeof(ImageNode))]
    public class ImageNodeEditor : GUINodeEditor
    {
        private ImageNode image { get { return node as ImageNode; } }
        private bool insFold = true;
        private GUIStyleDesign imageStyleDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (imageStyleDrawer == null)
                imageStyleDrawer = new GUIStyleDesign(image.imageStyle, "Image Style");
            insFold = FormatFoldGUI(insFold, "Image Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref image.image, false);
            imageStyleDrawer.OnGUI();
        }
    }
}
