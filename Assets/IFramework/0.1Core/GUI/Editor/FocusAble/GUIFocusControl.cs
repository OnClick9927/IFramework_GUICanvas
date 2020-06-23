/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
    public class GUIFocusControl : SingletonPropertyClass<GUIFocusControl>
    {
        private GUIFocusControl() { }
        private List<FocusAbleGUIDrawer> _drawers;
        private List<string> _UUIDs;
        public static string curFocusID
        {
            get
            {
                if (curFocusDrawer == null)
                    return string.Empty;
                return curFocusDrawer.focusID;
            }
        }
        public static FocusAbleGUIDrawer curFocusDrawer { get; private set; }

        public static string Subscribe(FocusAbleGUIDrawer drawer)
        {
            if (!Instance._drawers.Contains(drawer)) Instance._drawers.Add(drawer);
            string uuid = drawer.GetHashCode().ToString();
            if (Instance._UUIDs.Contains(uuid))
                uuid += "1";
            Instance._UUIDs.Add(uuid);
            return uuid;
        }
        public static void UnSubscribe(FocusAbleGUIDrawer drawer)
        {
            if (Instance._drawers.Contains(drawer))
            {
                Instance._drawers.Remove(drawer);
            }
            if (Instance._UUIDs.Contains(drawer.focusID))
            {
                Instance._UUIDs.Remove(drawer.focusID);
            }
        }

        public static bool Contans(FocusAbleGUIDrawer drawer)
        {
            return Instance._drawers.Contains(drawer);
        }

        public static void Focus(FocusAbleGUIDrawer drawer)
        {
            if (curFocusDrawer == drawer)
                GUI.FocusControl(null);
            curFocusDrawer = drawer;

            for (int i = 0; i < Instance._drawers.Count; i++)
            {
                if (Instance._drawers[i] == drawer) Instance._drawers[i].focused = true;
                else { Instance._drawers[i].focused = false; }
            }
            GUI.FocusControl(null);
            GUI.FocusControl(drawer.focusID);
        }
        public static void Diffuse(FocusAbleGUIDrawer drawer)
        {
            if (curFocusDrawer == drawer)
            {
                for (int i = 0; i < Instance._drawers.Count; i++)
                {
                    Instance._drawers[i].focused = false;
                }
                GUI.FocusControl(null);
                curFocusDrawer = null;
            }
        }


        protected override void OnSingletonInit()
        {
            _drawers = new List<FocusAbleGUIDrawer>();
            _UUIDs = new List<string>();
        }

        public override void Dispose()
        {
            _drawers.Clear();
            _UUIDs.Clear();
        }
    }

}
