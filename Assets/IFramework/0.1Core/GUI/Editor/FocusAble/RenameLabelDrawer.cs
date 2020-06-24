/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;

namespace IFramework.GUITool
{
    [Serializable]
	public class RenameLabelDrawer:FocusAbleGUIDrawer,IRectGUIDrawer
	{
        public RenameLabelDrawer() : base() { }
        public override void Dispose()
        {
            base.Dispose();
            onValueChange = null;
            onEndEdit = null;
        }
        private int _clickCount = 0;
        private System.DateTime _lastTime;

        public event Action<string> onValueChange;
        public event Action<string> onEndEdit;
        public string value;

        public bool clicked { get { return _clickCount > 0; } }
        public bool editing { get { return focused && _clickCount == 2; } }

        protected override void OnLostFous()
        {
            _clickCount = 0;
        }
        protected override void OnFocusOther()
        {
            _clickCount = 0;
        }

        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            Event e = Event.current;
            if (editing)
            {
                GUI.SetNextControlName(focusID);
                string tmp = value;
                this.TextField(position, ref tmp);
                if (value != tmp)
                {
                    value = tmp;
                    if (onValueChange != null)
                        onValueChange(value);
                }
            }
            else
            {
                this.Label(position, value);
            }

            if (position.Contains(e.mousePosition))
            {
                if (!focused)
                {
                    if (e.type == EventType.MouseDown && e.clickCount == 1)
                    {
                        if (_clickCount < 2)
                        {
                            if (_clickCount==0)
                            {
                                _clickCount++;
                                _lastTime = System.DateTime.Now;
                            }
                            else if(_clickCount==1)
                            {
                                if ((System.DateTime.Now-_lastTime).TotalSeconds>0.2)
                                {
                                    _clickCount++;
                                }
                            }
                        }
                        if (_clickCount==2)
                        {
                            GUIFocusControl.Focus(this);
                            if (e.type != EventType.Repaint && e.type != EventType.Layout)
                                Event.current.Use();
                        }
                    }
                    if (e.keyCode == KeyCode.F2)
                    {
                        _clickCount = 2;
                        GUIFocusControl.Focus(this);
                        if (e.type != EventType.Repaint && e.type != EventType.Layout)
                            Event.current.Use();
                    }
                }
            }
            else
            {
                if (e.type== EventType.MouseDrag && _clickCount>0)
                {
                    _clickCount = 0;
                    GUIFocusControl.Diffuse(this);
                    if (onEndEdit != null) onEndEdit(value);
                }
                if (e.type == EventType.MouseDown && e.clickCount == 1)
                {
                    _clickCount = 0;
                    GUIFocusControl.Diffuse(this);
                    if (onEndEdit != null) onEndEdit(value);
                }
            }
            if(e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape || e.character == '\n' || e.button==1)
            {
                GUIFocusControl.Diffuse(this);
                if (e.type != EventType.Repaint && e.type != EventType.Layout)
                    Event.current.Use();
                if (onEndEdit != null) onEndEdit(value);
            }
        }
        public override void Focus()
        {
            base.Focus();
            _clickCount = 2;
        }
        public override void Difuse()
        {
            base.Difuse();
            _clickCount = 0;
        }
    }
}
