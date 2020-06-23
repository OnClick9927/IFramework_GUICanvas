/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.266
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
    [Serializable]
    public class TabViewDrawer : GUIDrawer
    {
        [Serializable]
        private class TabNode
        {
            public float tabWidth = 50;
            public string content;
            public bool isOn;
            public bool needCloseBtn;
            public Action<Rect> pan;

            public TabNode Config(string content, float tabWidth, bool needCloseBtn, Action<Rect> pan)
            {
                this.tabWidth = tabWidth;
                this.content = content;
                this.needCloseBtn = needCloseBtn;
                this.pan = pan;
                return this;
            }

            public void OnGUI(Rect rect)
            {
                GUI.Box(rect, "");
                if (pan != null)
                {
                    pan(rect);
                }
            }
        }

        private class TabPool : ObjectPool<TabNode>
        {
            protected override TabNode CreatNew(IEventArgs arg)
            {
                return new TabNode();
            }
        }

        private TabPool _pool = new TabPool();
        [SerializeField]
        private List<TabNode> _nodes = new List<TabNode>();
        [SerializeField]
        private TabNode _current;

        public void CreateTab(string content, Action<Rect> pan, float width = 50, bool needClose = false)
        {
            var tab = _pool.Get().Config(content, width, needClose, pan);
            _nodes.Add(tab);
        }
        public void RemoveTab(string content)
        {
            var tab = _nodes.Find((node) => { return node.content == content; });
            _nodes.Remove(tab);
            if (tab != null)
            {
                _pool.Set(tab);
            }
        }


        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            var rs = position.Split(SplitType.Horizontal, 17);
            TiTle(rs[0]);
            Content(rs[1]);
        }
        private void TiTle(Rect rect)
        {
            rect.DrawOutLine(2, Color.red);
            GUI.Box(rect, "", "GameToolbar");
            float offset = rect.x;
            int xx_size = 17;
            _nodes.ForEach((index, node) =>
            {
                bool _draw = node.needCloseBtn ? offset + node.tabWidth + xx_size <= rect.xMax : offset + node.tabWidth <= rect.xMax;

                if (!_draw) return;

                var _rect = new Rect(offset, rect.y, node.tabWidth, rect.height);
                node.isOn = GUI.Toggle(_rect, node.isOn, node.content, "toolbarbutton");
                bool ison = node.isOn;
                if (ison)
                {
                    _current = node;
                    _nodes.ForEach((_node) =>
                    {
                        if (_node != node)
                        {
                            _node.isOn = false;
                        }
                    });
                }
                offset += node.tabWidth;
                if (node.needCloseBtn)
                {
                    _rect = new Rect(offset, rect.y, xx_size, rect.height);
                    if (GUI.Button(_rect, "", "WinBtnClose"))
                    {
                        RemoveTab(node.content);
                    }
                    offset += xx_size;
                }

            });
        }
        private void Content(Rect rect)
        {
            if (_current != null)
            {
                _current.OnGUI(rect);
            }
        }

        public override void Dispose()
        {
            _nodes.Clear();
            _pool.Dispose();
        }
    }
}
