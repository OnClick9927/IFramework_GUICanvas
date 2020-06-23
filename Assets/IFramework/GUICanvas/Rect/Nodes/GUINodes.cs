/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.GUITool.RectDesign
{
    public static class GUINodes
    {
        static public List<Type> nodeTypes = typeof(GUINode).GetSubTypesInAssemblys().ToList();
    }
}
