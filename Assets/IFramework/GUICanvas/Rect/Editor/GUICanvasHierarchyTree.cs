/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-11-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    class GUICanvasHierarchyTree
    {
        private class Trunk
        {
            private GUICanvasHierarchyTree m_tree;

            public GUICanvasHierarchyTree tree
            {
                get
                {
                    if (parent != null)
                        return parent.tree;
                    return m_tree;
                }
                set
                {
                    m_tree = value;
                }
            }
            private int siblingIndex { get { return element.siblingIndex; } }
            private GUINode element;
            public Trunk parent;
            private List<Trunk> children;
            private RenameLabelDrawer nameLabel;
            public Trunk(Trunk parent, GUINode element)
            {
                this.element = element;
                this.parent = parent;
                nameLabel = new RenameLabelDrawer();
                nameLabel.value = element.name;
                nameLabel.onEndEdit += (str) => { element.name = str.Trim(); };

                children = new List<Trunk>();
                if (element.Children.Count > 0)
                    for (int i = 0; i < element.Children.Count; i++)
                        children.Add(new Trunk(this, element.Children[i] as GUINode));
            }

            public void OnTreeChange()
            {
                List<Trunk> tmp = new List<Trunk>();
                tmp.AddRange(children);
                children.Clear();
                for (int i = 0; i < element.Children.Count; i++)
                {
                    var trunk = tmp.Find((t) => { return t.element == element.Children[i]; });
                    if (trunk == null)
                        children.Add(new Trunk(this, element.Children[i] as GUINode));
                    else
                    {
                        tmp.Remove(trunk);
                        children.Add(trunk);
                    }
                }
                tmp.ForEach((t) => { t.nameLabel.Dispose(); });
                children.ForEach((t) => { t.OnTreeChange(); });
            }

            private bool foldOn = true;
            private const float gapHeight = 2;
            private const float SingleLineHeight = 17;
            public static float LineHeight = SingleLineHeight + gapHeight /** 2*/;
            public float Height { get; private set; }
            public float CalcHeight()
            {
                if (!foldOn || children.Count == 0)
                {
                    Height = LineHeight;
                }
                else
                {
                    Height = LineHeight;
                    for (int i = 0; i < children.Count; i++)
                    {
                        Height += children[i].CalcHeight();
                    }
                }
                return Height;
            }

            public void OnCanvasTreeGUI(Rect rect, Event e)
            {
                bool active = element.active;
                if (active)
                {
                    GUINode ele = element;
                    while (ele.parent != null)
                    {
                        ele = ele.parent;
                        active = active && ele.active;
                        if (!active) break;
                    }
                }
                GUI.enabled = active;


                var rs = rect.HorizontalSplit(LineHeight);
                Rect selfRect = rs[0];
                if (GUINodeSelection.node == this.element && e.type == EventType.Repaint)
                    new GUIStyle("SelectionRect").Draw(selfRect, false, false, false, false);
                selfRect.xMin += 20 * element.depth;
                Rect childrenRect = rs[1];
                childrenRect.xMin += 20 * element.depth;

                //Rect topR = new Rect(selfRect.position, new Vector2(selfRect.width, gapHeight));
                Rect sf = new Rect(selfRect.position, new Vector2(selfRect.width, SingleLineHeight));
                Rect butR = new Rect(new Vector2(selfRect.x, selfRect.yMin + SingleLineHeight), new Vector2(selfRect.width, gapHeight));

                if (children.Count > 0)
                {
                    var rss = sf.VerticalSplit(12);
                    foldOn = EditorGUI.Foldout(rss[0], foldOn, "", false);
                    nameLabel.OnGUI(rss[1]);
                    //GUI.Label(rss[1], element.name);
                    if (tree.HandleEve)
                    {
                        Eve(selfRect, rss[1],/*topR,*/butR, e);
                    }
                    if (!foldOn) return;
                    float y = 0;
                    for (int i = 0; i < children.Count; i++)
                    {
                        Rect r = new Rect(rect.x, childrenRect.y + y, rect.width, children[i].Height);
                        y += children[i].Height;
                        children[i].OnCanvasTreeGUI(r, e);
                    }
                }
                else
                {
                    nameLabel.OnGUI(sf);

                    //GUI.Label(sf, element.name);
                    if (tree.HandleEve)
                    {
                        Eve(selfRect, sf,/*topR,*/butR, e);
                    }
                }
                GUI.enabled = true;
            }
            private void Eve(Rect r, Rect sf,/*Rect tr,*/Rect br, Event e)
            {
                MouseDragEve(r, sf,/*tr,*/ br, e);
                if (r.Contains(e.mousePosition) /*&& e.type == EventType.MouseDown */&& e.clickCount == 1 && e.button == 0) GUINodeSelection.node = this.element;
                if (r.Contains(e.mousePosition) && GUINodeSelection.node == this.element)
                {
                    if (e.type == EventType.KeyUp)
                    {
                        if (e.modifiers == EventModifiers.Control && e.keyCode == KeyCode.C)
                        {
                            OnCtrlC();
                            e.Use();
                        }
                        if (e.modifiers == EventModifiers.Control && e.keyCode == KeyCode.V && GUINodeSelection.copyNode != null)
                        {
                            OnCtrlV();
                            e.Use();
                        }
                        if (e.modifiers == EventModifiers.Control && e.keyCode == KeyCode.D)
                        {
                            OnCtrlD();
                            e.Use();
                        }
                        if (e.keyCode == KeyCode.Delete)
                        {
                            OnDelete();
                            e.Use();
                        }
                    }

                    if (e.button == 1 && e.clickCount == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        OnMenu(menu);
                        menu.ShowAsContext();
                        if (e.type != EventType.Layout)
                            e.Use();
                    }
                }

            }
            private void MouseDragEve(Rect r, Rect sf, /*Rect tr,*/ Rect br, Event e)
            {
                bool CouldPutdown = r.Contains(e.mousePosition) && GUINodeSelection.dragNode != null && GUINodeSelection.dragNode != this.element;
                GUINode tmp = this.element;
                while (tmp.parent != null)
                {
                    tmp = tmp.parent;
                    if (tmp == GUINodeSelection.dragNode)
                    {
                        CouldPutdown = false;
                        break;
                    }
                }
                if (CouldPutdown)
                {
                    if (sf.Contains(e.mousePosition))
                        GUI.Box(sf, "", "SelectionRect");
                    //else if (tr.Contains(e.mousePosition))
                    //{
                    //    if (!(element is GUICanvas))
                    //        GUI.Box(new Rect(tr.x, tr.y - SingleLineHeight, tr.width, SingleLineHeight), "", "PR Insertion");
                    //}
                    else if (br.Contains(e.mousePosition))
                    {
                        if (!(element is GUICanvas))
                            GUI.Box(sf, "", "PR Insertion");
                    }
                }
                if (CouldPutdown && e.type == EventType.MouseUp)
                {
                    if (sf.Contains(e.mousePosition))
                    {
                        this.element.Node(GUINodeSelection.dragNode);

                    }
                    //else if (tr.Contains(e.mousePosition))
                    //{
                    //    if (!(element is GUICanvas))
                    //    {
                    //        ElementSelection.dragElement.parent = this.element.parent;
                    //        element.parent.Children.Remove(ElementSelection.dragElement);
                    //        element.parent.Children.Insert(element.siblingIndex /*- 1*/, ElementSelection.dragElement);
                    //        dragTrunk.parent = this.parent;
                    //        parent.children.Remove(dragTrunk);
                    //        parent.children.Insert(element.siblingIndex /*- 1*/, dragTrunk);
                    //    }

                    //}
                    else if (br.Contains(e.mousePosition))
                    {
                        if (!(element is GUICanvas))
                        {
                            (this.element.parent as GUINode).Node(GUINodeSelection.dragNode);
                            GUINodeSelection.dragNode.siblingIndex = element.siblingIndex + 1;
                        }

                    }

                    GUINodeSelection.dragNode = null;
                }
                else if (GUINodeSelection.node == this.element)
                {
                    if (e.type == EventType.MouseDrag)
                        GUINodeSelection.dragNode = GUINodeSelection.node;
                    else if (e.type == EventType.MouseUp)
                        GUINodeSelection.dragNode = null;
                }


            }
            private void OnMenu(GenericMenu menu)
            {
                var types = GUINodes.nodeTypes
                .FindAll((type) => { return !type.IsAbstract && type.IsDefined(typeof(GUINodeAttribute), false); });
                types.ForEach((type) =>
                {
                    string createPath = (type.GetCustomAttributes(typeof(GUINodeAttribute), false).First() as GUINodeAttribute).CreatPath;
                    menu.AddItem(new GUIContent("Create/" + createPath), false, () => { OnCeateElement(type); });
                });
                if (GUINodeSelection.copyNode == null)
                    menu.AddDisabledItem(new GUIContent("Paste"));
                else
                    menu.AddItem(new GUIContent("Paste"), false, OnCtrlV);
                menu.AddItem(new GUIContent("Reset"), false, element.Reset);
                if (!(element is GUICanvas))
                {

                    menu.AddItem(new GUIContent("Copy"), false, OnCtrlC);
                    menu.AddItem(new GUIContent("Duplicate"), false, OnCtrlD);
                    menu.AddItem(new GUIContent("Delete"), false, OnDelete);
                    if (siblingIndex == 0)
                        menu.AddDisabledItem(new GUIContent("MoveUp"));
                    else
                        menu.AddItem(new GUIContent("MoveUp"), false, OnMoveUp);
                    if (siblingIndex == element.parent.Children.Count - 1)
                        menu.AddDisabledItem(new GUIContent("MoveDown"));
                    else
                        menu.AddItem(new GUIContent("MoveDown"), false, OnMoveDown);
                    menu.AddItem(new GUIContent("Save Xml prefab"), false, OnSavePrefab);
                }
                menu.AddItem(new GUIContent("Load Xml prefab"), false, OnLoadPrefab);

            }

            private void OnLoadPrefab()
            {
                string str = EditorUtility.OpenFilePanel("Load", Application.dataPath, "xml");
                if (System.IO.File.Exists(str))
                {
                    element.LoadXmlPrefab(str);
                }
            }
            private void OnSavePrefab()
            {
                string str = EditorUtility.OpenFilePanel("Save", Application.dataPath, "xml");
                if (System.IO.File.Exists(str))
                {
                    element.SaveXmlPrefab(str);
                }
            }

            private void OnCeateElement(Type type)
            {
                GUINode copy = Activator.CreateInstance(type) as GUINode;
                this.element.Node(copy);
                //copy.parent = this.element;
                //tree.OnTreeChange();
            }
            private void OnMoveUp()
            {
                int tmp = element.siblingIndex;
                if (tmp != 0)
                    element.siblingIndex = tmp - 1;
            }
            private void OnMoveDown()
            {
                int tmp = element.siblingIndex;
                if (tmp != element.parent.Children.Count - 1)
                    element.siblingIndex = tmp + 1;
            }
            protected virtual void OnCtrlC()
            {
                if (this.element.GetType() != typeof(GUICanvas))
                {
                    GUINodeSelection.copyNode = this.element;
                }
            }
            protected virtual void OnCtrlV()
            {
                if (GUINodeSelection.copyNode == null) return;
                GUINode copy = Activator.CreateInstance(GUINodeSelection.copyNode.GetType(), GUINodeSelection.copyNode) as GUINode;
                if (this.element.GetType() != typeof(GUICanvas))
                    (this.element.parent as GUINode).Node(copy);
                else
                    (this.element as GUINode).Node(copy);
                GUINodeSelection.copyNode = null;
            }
            protected virtual void OnCtrlD()
            {
                if (this.element.GetType() == typeof(GUICanvas)) return;
                GUINode copy = Activator.CreateInstance(this.element.GetType(), this.element) as GUINode;
                (this.element.parent as GUINode).Node(copy);
            }
            protected virtual void OnDelete()
            {
                if (this.element.GetType() == typeof(GUICanvas)) return;
                element.Destoty();
                GUINodeSelection.node = null;
            }
        }


        public bool HandleEve = true;
        private GUICanvas m_canvas;
        public GUICanvas canvas
        {
            get { return m_canvas; }
            set
            {
                m_canvas = value;
                root = new Trunk(null, m_canvas);
                root.tree = this;
                canvas.OnCanvasTreeChange += root.OnTreeChange;
            }
        }
        private Trunk root;

        private Vector2 scroll;
        public void OnCanvasTreeGUI(Rect rect)
        {
            if (root == null) return;
            Event e = Event.current;
            if (!rect.Contains(Event.current.mousePosition))
                GUINodeSelection.dragNode = null;
            root.CalcHeight();
            var rs = rect.HorizontalSplit(root.Height);
            scroll = GUI.BeginScrollView(rect, scroll, rs[0]);
            root.OnCanvasTreeGUI(rs[0], e);
            GUI.EndScrollView();
            if (HandleEve)
            {
                EmptyEve(e, rs[1]);
            }
        }
        private void EmptyEve(Event e, Rect r)
        {
            if (r.height > 0 && r.Contains(e.mousePosition) && e.type == EventType.MouseUp)
            {
                GUINodeSelection.node = null;
                GUINodeSelection.dragNode = null;
            }
        }

    }

}
