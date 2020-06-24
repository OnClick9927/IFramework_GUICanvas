/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;
namespace IFramework.GUITool
{
    public class SearchFieldDrawer : FocusAbleGUIDrawer,IRectGUIDrawer
    {
        private class Styles
        {
           public static GUIStyle SearchTextFieldStyle = GUIStyles.Get("SearchTextField");
           public static GUIStyle SearchCancelButtonStyle = GUIStyles.Get("SearchCancelButton");
           public static GUIStyle SearchCancelButtonEmptyStyle = GUIStyles.Get("SearchCancelButtonEmpty");
        }
        public string value = "";
        public bool bestFit { get; set; }
        public override void Dispose()
        {
            base.Dispose();
            onValueChange = null;
            onEndEdit = null;
        }
        public event Action<string> onValueChange;
        public event Action<string> onEndEdit;
        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            position= position.Zoom( AnchorType.MiddleCenter,new Vector2(-4,-1)).MoveUp(1);
            GUIStyle cancelBtnStyle = string.IsNullOrEmpty(value) ?Styles. SearchCancelButtonEmptyStyle : Styles.SearchCancelButtonStyle;
            position.width -= cancelBtnStyle.fixedWidth;

            Styles.SearchTextFieldStyle.fixedHeight = position.height;
            cancelBtnStyle.fixedHeight = position.height;

            Styles.SearchTextFieldStyle.alignment = TextAnchor.MiddleLeft;
            if (bestFit)
            {
                while (Styles.SearchTextFieldStyle.lineHeight < position.height - 15)
                {
                    Styles.SearchTextFieldStyle.fontSize++;
                }
            }
           
            string tmp = value;

            GUI.SetNextControlName(focusID);
            this.TextField(position.MoveRight(1), ref tmp, Styles.SearchTextFieldStyle)
                .Button(()=> {
                    tmp = string.Empty;
                    GUI.changed = true;
                    GUIUtility.keyboardControl = 0;
                    if (onEndEdit != null) onEndEdit(value);
                }, new Rect(position.xMax,position.y,cancelBtnStyle.fixedWidth, cancelBtnStyle.fixedHeight),GUIContent.none,cancelBtnStyle);

            if (tmp!=value)
            {
                value = tmp;
                if (onValueChange!=null) onValueChange(value);
            }
            Event e = Event.current;
            if (position.Contains(e.mousePosition))
            {
                if (!focused)
                    if (e.type == EventType.MouseDown)
                    {
                        focused = true;
                        GUIFocusControl.Focus(this);
                        if (e.type != EventType.Repaint && e.type != EventType.Layout)
                            Event.current.Use();
                    }
            }
            if ((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape ||e.character=='\n'))
            {
                GUIFocusControl.Diffuse(null);
                focused = false;
                if (e.type != EventType.Repaint && e.type != EventType.Layout)
                    Event.current.Use();
                if (onEndEdit != null) onEndEdit(value);
            }
        }

    }

}
