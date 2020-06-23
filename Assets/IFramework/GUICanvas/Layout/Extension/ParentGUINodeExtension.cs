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
    public static class ParentGUINodeExtension
    {
        public static T Node<T>(this T self, GUINode element) where T : ParentGUINode
        {
            element.parent = self;
            GUICanvas canvas = element.root as GUICanvas;
            if (canvas != null)
                canvas.TreeChange();
            return self;
        }
    }

}
