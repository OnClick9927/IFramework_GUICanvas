/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.271
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
    public class MenuTree:GUIDrawer
    {
        public class MenuTrunk
        {
            public string name;
            public float height = 25;
            protected bool isOn;

            public virtual int depth { get { return parent.depth + 1; } }
            public MenuTrunk parent;
            public MenuTree tree;
            protected List<MenuTrunk> childs { get { return tree.GetChild(this); } }
            public string path
            {
                get
                {
                    if (parent == null)
                        return name;
                    return parent.path + "/" + name;
                }
            }
            public float totalHeight
            {
                get
                {
                    float tmp = height;
                    if (isOn)
                    {
                        childs.ForEach((node) => {
                            tmp += node.totalHeight;
                        });
                    }
                    return tmp;

                }
            }

            public virtual void OnGUI(Rect rect)
            {
                if (childs == null || childs.Count == 0 || !isOn)
                {
                    SelfGUI(rect);
                }
                else
                {
                    var rs = rect.HorizontalSplit(height);
                    SelfGUI(rs[0]);

                    DrawChild(rs[1], 0);
                }
            }
            private void SelfGUI(Rect rect)
            {
                GUI.Box(rect, "", "RL Background");
                if (tree._current == this) GUI.Box(rect, "", "SelectionRect");


                var r = rect.Zoom(AnchorType.MiddleRight, new Vector2(-depth * 10, 0));
                if (childs == null || childs.Count == 0)
                    GUI.Label(r, name);
                else
                    isOn = EditorGUI.Foldout(r, isOn, name, false);

                if (rect.Zoom(AnchorType.MiddleRight, new Vector2(-50, 0)).Contains(Event.current.mousePosition) && Event.current.clickCount == 1)
                {
                    if (tree._current != this)
                    {
                        tree._current = this;
                    }
                }
            }
            protected void DrawChild(Rect rect, int index)
            {
                if (index >= childs.Count) return;
                var rs = rect.HorizontalSplit(childs[index].totalHeight);
                childs[index].OnGUI(rs[0]);
                DrawChild(rs[1], ++index);
            }
        }
        public class MenuRoot : MenuTrunk
        {
            public override int depth { get { return 0; } }
            public MenuRoot() { isOn = true; height = 0; }
            public override void OnGUI(Rect rect)
            {
                if (childs != null && childs.Count > 0)
                {
                    DrawChild(rect, 0);
                }
            }
        }
        private MenuRoot _root;
        private List<MenuTrunk> _nodes;
        private MenuTrunk __current;
        private Vector2 _scroll;
        private MenuTrunk _current
        {
            get { return __current; }
            set
            {
                if (__current != value)
                {
                    __current = value;
                    if (__current != null && onCurrentChange != null)
                    {

                        onCurrentChange(__current.path.Substring(_root.path.Length + 1));
                    }
                }
            }
        }

        public event Action<string> onCurrentChange;
        public float height { get { return _root.totalHeight; } }
        public MenuTree()
        {
            _root = new MenuRoot();
            _root.name = "root";
            _root.tree = this;
            _nodes = new List<MenuTrunk>();
            _nodes.Add(_root);
        }


        private List<MenuTrunk> GetChild(MenuTrunk trunk)
        {
            return _nodes.FindAll((_node) => { return _node.parent == trunk; });
        }
        private MenuTrunk CreateTrunk(string content, MenuTrunk parent)
        {
            MenuTrunk leaf = new MenuTrunk();
            leaf.name = content;
            leaf.tree = this;
            leaf.parent = parent;
            _nodes.Add(leaf);
            return leaf;
        }


        public void ReadTree(List<string> paths)
        {
            paths.Sort();
            for (int i = 0; i < paths.Count; i++)
            {
                string path = _root.path + "/" + paths[i];
                if (ContainsNode(path)) continue;
                var items = path.Split('/');
                for (int j = 1; j < items.Length; j++)
                {
                    string tmpPath = ToString(items, j);
                    CreateNode(tmpPath, items[j]);
                }
            }

        }
        private string ToString(string[] strs, int count)
        {
            string tmp = "";
            for (int i = 0; i < count; i++)
            {
                tmp += "/" + strs[i];
            }
            return tmp.Substring(1);
        }
        private void CreateNode(string path, string content)
        {
            if (ContainsNode(path + "/" + content)) return;
            var trunk = Find(path);
            var t = CreateTrunk(content, trunk);


        }
        private bool ContainsNode(string path)
        {
            return Find(path) != null;
        }
        private MenuTrunk Find(string path)
        {
            return _nodes.Find((_node) => { return path == _node.path; });
        }
        private MenuTrunk FindByName(string name)
        {
            return _nodes.Find((_node) => { return name == _node.name; });
        }




        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            Event e = Event.current;
            var rs = position.HorizontalSplit(_root.totalHeight);
            _scroll = GUI.BeginScrollView(position, _scroll, rs[0]);
            _root.OnGUI(rs[0]);
            EmptyEve(e, rs[1]);
            GUI.EndScrollView();
        }
        private void EmptyEve(Event e, Rect r)
        {
          //  r.DrawOutLine(2, Color.red);
            if (r.height > 0 && r.Contains(e.mousePosition) && e.type == EventType.MouseUp)
            {
                _current = null;
            }
        }

        public override void Dispose()
        {
           
        }
    }

}
