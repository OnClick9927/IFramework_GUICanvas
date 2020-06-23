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
    [GUINode("Empty")]
    public class EmptyGUINode : GUINode
    {
        public event Action<EmptyGUINode> pan;
        protected override void OnGUI_Self()
        {
            if (pan != null)
            {
                pan(this);
            }
        }
    }
}
