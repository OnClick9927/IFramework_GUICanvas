/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUITool
{


    public abstract class FocusAbleGUIDrawer : GUIDrawer
    {
        public string focusID { get; private set; }
        protected bool _focused;
        public bool focused
        {
            get
            {
                return _focused;
            }
            set
            {
                if (value)
                    OnFcous();
                else
                {
                    OnFocusOther();
                    if (!_focused)
                        OnLostFous();
                }
                _focused = value;
            }
        }
        public FocusAbleGUIDrawer()
        {
            focusID = GUIFocusControl.Subscribe(this);
        }
        public Rect position { get; private set; }
        protected virtual void OnFcous() { }
        protected virtual void OnFocusOther() { }
        protected virtual void OnLostFous() { }

        public override void Dispose()
        {
            GUIFocusControl.UnSubscribe(this);
        }

        public virtual void Focus()
        {
            GUIFocusControl.Focus(this);
        }

        public virtual void Difuse()
        {
            GUIFocusControl.Diffuse(this);
        }
    }

}
