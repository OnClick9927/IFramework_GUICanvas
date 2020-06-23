/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.GUITool.RectDesign
{
    [CustomGUINode(typeof(EmptyGUINode))]

    public class EmptyNodeEditor:GUINodeEditor
	{
        public override void OnSceneGUI(Action children)
        {
            if (element.active)
            {
                BeginGUI();
                if (children!=null)
                {
                    children();
                }
                EndGUI();
            }
        }
    }
}
