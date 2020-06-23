/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using UnityEditor;
namespace IFramework
{
    public class DragAndDropIInfo
    {
        public bool dragging;
        public bool enterArera;
        public bool compelete;
        public Object[] objectReferences { get { return DragAndDrop.objectReferences; } }
        public string[] paths { get { return DragAndDrop.paths; } }
        public DragAndDropVisualMode visualMode { get { return DragAndDrop.visualMode; } }
        public int activeControlID { get { return DragAndDrop.activeControlID; } }
    }

    public class DragAndDropUtil
	{
        private static bool _dragging;
        private static bool _enterArera;
        private static bool _compelete;
        private static DragAndDropIInfo _info=new DragAndDropIInfo();
        public static DragAndDropIInfo Drag(Event eve ,Rect Content, DragAndDropVisualMode mode = DragAndDropVisualMode.Generic)
        {
            switch (eve.type)
            {
                case EventType.DragUpdated:
                    _dragging = true; _compelete = false;
                    _enterArera = Content.Contains(eve.mousePosition);
                    if (_enterArera)
                    {
                        DragAndDrop.visualMode = mode;
                        Event.current.Use();
                    }
                    break;
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    _enterArera = Content.Contains(eve.mousePosition);
                    _compelete = true;_dragging = false;
                    Event.current.Use();

                    break;
                case EventType.DragExited:
                    _dragging = false; _compelete = true;
                    _enterArera = Content.Contains(eve.mousePosition);
                    break;
                default:
                    _dragging = false; _compelete = false;
                    _enterArera = Content.Contains(eve.mousePosition);
                    break;
            }
            _info.compelete = _compelete;
            _info.enterArera = _enterArera;
            _info.dragging = _dragging;
            return _info;
        }
	}
}
