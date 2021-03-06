﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.GUITool.RectDesign
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGUINodeAttribute : Attribute
    {
        public Type EditType { get; private set; }
        public CustomGUINodeAttribute(Type designTYpe)
        {
            this.EditType = designTYpe;
        }
    }
}
