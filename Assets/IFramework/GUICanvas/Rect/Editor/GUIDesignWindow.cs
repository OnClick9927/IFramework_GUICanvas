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
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUITool.RectDesign
{
    static class GUINodeEditors
    {
        public static List<Type> editorTypes = typeof(GUINodeEditor).GetSubTypesInAssemblys().ToList();
    }
    abstract class DesignWindowItem : ILayoutGUIDrawer
    {
        protected bool _isDragOtherWindow { get; private set; }
        public abstract void OnGUI(Rect rect);

        protected void HandleDragOther(Rect rect)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDrag)
            {
                if (!rect.Contains(e.mousePosition))
                {
                    _isDragOtherWindow = true;
                }
            }
            if (e.type == EventType.MouseUp)
            {
                _isDragOtherWindow = false;
            }
        }
    }
    class GUINodeSelection
    {
        public delegate void OnNodeChange(GUINode node);
        public static event OnNodeChange onNodeChange;
        private static GUINode _node;
        public static GUINode node
        {
            get { return _node; }
            set
            {
                _node = value;

                if (UnityEditor.EditorWindow.focusedWindow != null)
                    UnityEditor.EditorWindow.focusedWindow.Repaint();
                if (Event.current != null && Event.current.type != EventType.Layout)
                    Event.current.Use();
                if (onNodeChange != null)
                    onNodeChange(_node);
            }
        }

        public static GUINode copyNode { get; set; }
        public static GUINode dragNode { get; set; }
    }
    [Serializable]
    class GUINodeSceneView : DesignWindowItem
    {
        private class CanvasSetting : PopupWindowContent, ILayoutGUIDrawer
        {
            public GUINodeSceneView window;
            public override void OnGUI(Rect rect)
            {
                if (window == null) return;
                this.RectField("Canvas Rect", ref window.canvas.canvasRect)
                    .FloatField("zoomDelta", ref window._zoomDelta)
                    .FloatField("minZoom", ref window._minZoom)
                    .FloatField("maxZoom", ref window._maxZoom)
                    .Pan(() => {
                        window.ZoomScale = EditorGUILayout.FloatField("ZoomScale", window.ZoomScale);
                    })
                    .FloatField("panSpeed", ref window._panSpeed)
                    .Vector2Field("panOffset", ref window._panOffset)
                                            .FlexibleSpace()

                    .BeginHorizontal()
                        .FlexibleSpace()
                        .Button(() => {
                            window._zoomDelta = 0.01f;
                            window._minZoom = 1f;
                            window._maxZoom = 8f;
                            window._panSpeed = 1.2f;
                            window.ZoomScale = 1f;
                            window._panOffset = Vector2.zero;
                        }, "Reset")
                    .EndHorizontal();
            }
        }

        private enum EditType
        {
            Design = 0, Result = 1
        }

        public GUICanvas canvas;
        private EditType _editType;
        private const float ToolBarHeight = 20;
        private ToolBarTree _toolbar = new ToolBarTree();
        private Dictionary<Type, GUINodeEditor> _dic = new Dictionary<Type, GUINodeEditor>();

        public GUINodeSceneView()
        {
            GUIScaleUtility.CheckInit();

            var designs = GUINodeEditors.editorTypes.FindAll((t) => { return t.IsDefined(typeof(CustomGUINodeAttribute), false); });
            var eles = GUINodes.nodeTypes;
            foreach (var type in eles)
            {
                var typeTree = type.GetTypeTree();
                for (int i = 0; i < typeTree.Count; i++)
                {
                    Type des = designs.Find((t) => {
                        return (t.GetCustomAttributes(typeof(CustomGUINodeAttribute), false).First() as CustomGUINodeAttribute).EditType == typeTree[i];
                    });
                    if (des != null)
                    {
                        _dic.Add(type, Activator.CreateInstance(des) as GUINodeEditor);
                        break;

                    }
                }
            }

            _toolbar
            .Delegate((r) =>
            {
                _editType = (EditType)GUILayout.Toolbar((int)_editType, System.Enum.GetNames(typeof(EditType)));
            }, 0)
            .FlexibleSpace()
            .Button(new GUIContent("ReCeter"), (rec) =>
            {
                _panOffset = -canvas.canvasRect.center;
            }, 50)
            .Button(new GUIContent("Setting"), (Rect rect) => {
                PopupWindow.Show(rect, new CanvasSetting() { window = this });
            }, 50);
        }


        private float _zoomDelta = 0.01f;
        private float _minZoom = 1f;
        private float _maxZoom = 8f;
        private float _panSpeed = 1.2f;
        private Vector2 _zoomAdjustment;
        private Vector2 _panOffset = Vector2.zero;
        private Vector2 _zoom = Vector2.one;

        private float ZoomScale
        {
            get { return _zoom.x; }
            set
            {
                float z = Mathf.Clamp(value, _minZoom, _maxZoom);
                _zoom.Set(z, z);
            }
        }
        private Vector2 GraphToScreenSpace(Vector2 graphPos)
        {
            return graphPos + _zoomAdjustment + _panOffset;
        }
        private void Pan(Vector2 delta)
        {
            _panOffset += delta * ZoomScale * _panSpeed;
        }
        private void Zoom(float zoomDirection)
        {
            float scale = (zoomDirection < 0f) ? (1f - _zoomDelta) : (1f + _zoomDelta);
            _zoom *= scale;
            float cap = Mathf.Clamp(_zoom.x, _minZoom, _maxZoom);
            _zoom.Set(cap, cap);
        }
        private void DrawAxes(Rect rect, Color color, float width = 2f)
        {
            Vector2 down = Vector2.up * rect.height * ZoomScale;
            Vector2 right = Vector2.right * rect.width * ZoomScale;
            Vector2 up = -down;
            Vector2 left = -right;
            up = GraphToScreenSpace(up);
            down = GraphToScreenSpace(down);
            right = GraphToScreenSpace(right);
            left = GraphToScreenSpace(left);
            DrawLine(right, left, color, width);
            DrawLine(up, down, color, width);
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color, float width = 2f)
        {
            var handleColor = Handles.color;
            Handles.color = color;
            Handles.DrawAAPolyLine(width, start, end);
            Handles.color = handleColor;
        }

        public override void OnGUI(Rect rect)
        {
            var rs = rect.HorizontalSplit(ToolBarHeight, 1);
            Rect graphRect = new Rect(new Vector2(5, ToolBarHeight * 1.2f), rs[1].size);
            var center = graphRect.size / 2f;
            _zoomAdjustment = GUIScaleUtility.BeginScale(ref graphRect, center, ZoomScale, false);
            graphRect.position = GraphToScreenSpace(canvas.position.position);
            DrawAxes(graphRect, Color.grey, 10);
            GUI.BeginClip(graphRect);
            switch (_editType)
            {
                case EditType.Design:
                    EleGUI(canvas);
                    break;
                case EditType.Result:
                    canvas.OnGUI();
                    break;
            }
            GUI.EndClip();
            GUIScaleUtility.EndScale();

            Event e = Event.current;
            HandleDragOther(rs[1]);
            if (e.type == EventType.ScrollWheel && rs[1].Contains(e.mousePosition))
            {
                Zoom(e.delta.y);
            }
            if (e.type == EventType.MouseDrag && !_isDragOtherWindow)
            {
                Pan(e.delta);
            }

            _toolbar.OnGUI(rs[0].Zoom(AnchorType.LowerCenter, new Vector2(8, 2)).MoveUp(2));
        }
        public void EleGUI(GUINode ele)
        {
            GUINodeEditor des = _dic[ele.GetType()];
            des.node = ele;
            des.OnSceneGUI(() => {
                for (int i = 0; i < ele.Children.Count; i++)
                {
                    EleGUI(ele.Children[i] as GUINode);
                }
            });

        }

    }

    class GUICanvasHierarchyTree: DesignWindowItem
    {
        private class Trunk
        {
            private bool foldOn = true;
            private const float GapHeight = 2;
            private const float SingleLineHeight = 17;
            private const float LineHeight = SingleLineHeight + GapHeight /** 2*/;


            public float height { get; private set; }
            private GUICanvasHierarchyTree _tree;
            private GUINode _node;
            private List<Trunk> _children;
            private RenameLabelDrawer _nameLabel;
            private int _siblingIndex { get { return _node.siblingIndex; } }

            public Trunk parent;
            public GUICanvasHierarchyTree tree
            {
                get
                {
                    if (parent != null)
                        return parent.tree;
                    return _tree;
                }
                set
                {
                    _tree = value;
                }
            }
            public Trunk(Trunk parent, GUINode node)
            {
                this._node = node;
                this.parent = parent;
                _nameLabel = new RenameLabelDrawer();
                _nameLabel.value = node.name;
                _nameLabel.onEndEdit += (str) => { node.name = str.Trim(); };

                _children = new List<Trunk>();
                if (node.Children.Count > 0)
                    for (int i = 0; i < node.Children.Count; i++)
                        _children.Add(new Trunk(this, node.Children[i] as GUINode));
            }

            public void OnTreeChange()
            {
                List<Trunk> tmp = new List<Trunk>();
                tmp.AddRange(_children);
                _children.Clear();
                for (int i = 0; i < _node.Children.Count; i++)
                {
                    var trunk = tmp.Find((t) => { return t._node == _node.Children[i]; });
                    if (trunk == null)
                        _children.Add(new Trunk(this, _node.Children[i] as GUINode));
                    else
                    {
                        tmp.Remove(trunk);
                        _children.Add(trunk);
                    }
                }
                tmp.ForEach((t) => { t._nameLabel.Dispose(); });
                _children.ForEach((t) => { t.OnTreeChange(); });
            }



            public float CalcHeight()
            {
                if (!foldOn || _children.Count == 0)
                {
                    height = LineHeight;
                }
                else
                {
                    height = LineHeight;
                    for (int i = 0; i < _children.Count; i++)
                    {
                        height += _children[i].CalcHeight();
                    }
                }
                return height;
            }

            public void OnGUI(Rect rect, Event e)
            {
                bool active = _node.active;
                if (active)
                {
                    GUINode ele = _node;
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
                if (GUINodeSelection.node == this._node && e.type == EventType.Repaint)
                    new GUIStyle("SelectionRect").Draw(selfRect, false, false, false, false);
                selfRect.xMin += 20 * _node.depth;
                Rect childrenRect = rs[1];
                childrenRect.xMin += 20 * _node.depth;

                //Rect topR = new Rect(selfRect.position, new Vector2(selfRect.width, gapHeight));
                Rect sf = new Rect(selfRect.position, new Vector2(selfRect.width, SingleLineHeight));
                Rect butR = new Rect(new Vector2(selfRect.x, selfRect.yMin + SingleLineHeight), new Vector2(selfRect.width, GapHeight));

                if (_children.Count > 0)
                {
                    var rss = sf.VerticalSplit(12);
                    foldOn = EditorGUI.Foldout(rss[0], foldOn, "", false);
                    _nameLabel.OnGUI(rss[1]);
                    //GUI.Label(rss[1], element.name);
                    if (tree.handleEve)
                    {
                        Eve(selfRect, rss[1],/*topR,*/butR, e);
                    }
                    if (!foldOn) return;
                    float y = 0;
                    for (int i = 0; i < _children.Count; i++)
                    {
                        Rect r = new Rect(rect.x, childrenRect.y + y, rect.width, _children[i].height);
                        y += _children[i].height;
                        _children[i].OnGUI(r, e);
                    }
                }
                else
                {
                    _nameLabel.OnGUI(sf);

                    //GUI.Label(sf, element.name);
                    if (tree.handleEve)
                    {
                        Eve(selfRect, sf,/*topR,*/butR, e);
                    }
                }
                GUI.enabled = true;
            }
            private void Eve(Rect r, Rect sf,/*Rect tr,*/Rect br, Event e)
            {
                MouseDragEve(r, sf,/*tr,*/ br, e);
                if (r.Contains(e.mousePosition) /*&& e.type == EventType.MouseDown */&& e.clickCount == 1 && e.button == 0) GUINodeSelection.node = this._node;
                if (r.Contains(e.mousePosition) && GUINodeSelection.node == this._node)
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
                if (tree._isDragOtherWindow)
                {
                    GUINodeSelection.dragNode = null;
                }
                bool CouldPutdown = r.Contains(e.mousePosition) && GUINodeSelection.dragNode != null && GUINodeSelection.dragNode != this._node;
                GUINode tmp = this._node;
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
                        if (!(_node is GUICanvas))
                            GUI.Box(sf, "", "PR Insertion");
                    }
                }
                if (CouldPutdown && e.type == EventType.MouseUp)
                {
                    if (sf.Contains(e.mousePosition))
                    {
                        this._node.Node(GUINodeSelection.dragNode);

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
                        if (!(_node is GUICanvas))
                        {
                            (this._node.parent as GUINode).Node(GUINodeSelection.dragNode);
                            GUINodeSelection.dragNode.siblingIndex = _node.siblingIndex + 1;
                        }

                    }

                    GUINodeSelection.dragNode = null;
                }
                else if (GUINodeSelection.node == this._node)
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
                    menu.AddItem(new GUIContent("Create/" + createPath), false, () => { OnCeateNode(type); });
                });
                if (GUINodeSelection.copyNode == null)
                    menu.AddDisabledItem(new GUIContent("Paste"));
                else
                    menu.AddItem(new GUIContent("Paste"), false, OnCtrlV);
                menu.AddItem(new GUIContent("Reset"), false, _node.Reset);
                if (!(_node is GUICanvas))
                {

                    menu.AddItem(new GUIContent("Copy"), false, OnCtrlC);
                    menu.AddItem(new GUIContent("Duplicate"), false, OnCtrlD);
                    menu.AddItem(new GUIContent("Delete"), false, OnDelete);
                    if (_siblingIndex == 0)
                        menu.AddDisabledItem(new GUIContent("MoveUp"));
                    else
                        menu.AddItem(new GUIContent("MoveUp"), false, OnMoveUp);
                    if (_siblingIndex == _node.parent.Children.Count - 1)
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
                    _node.LoadXmlPrefab(str);
                }
            }
            private void OnSavePrefab()
            {
                string str = EditorUtility.OpenFilePanel("Save", Application.dataPath, "xml");
                if (System.IO.File.Exists(str))
                {
                    _node.SaveXmlPrefab(str);
                }
            }

            private void OnCeateNode(Type type)
            {
                GUINode copy = Activator.CreateInstance(type) as GUINode;
                this._node.Node(copy);
                //copy.parent = this.element;
                //tree.OnTreeChange();
            }
            private void OnMoveUp()
            {
                int tmp = _node.siblingIndex;
                if (tmp != 0)
                    _node.siblingIndex = tmp - 1;
            }
            private void OnMoveDown()
            {
                int tmp = _node.siblingIndex;
                if (tmp != _node.parent.Children.Count - 1)
                    _node.siblingIndex = tmp + 1;
            }
            protected virtual void OnCtrlC()
            {
                if (this._node.GetType() != typeof(GUICanvas))
                {
                    GUINodeSelection.copyNode = this._node;
                }
            }
            protected virtual void OnCtrlV()
            {
                if (GUINodeSelection.copyNode == null) return;
                GUINode copy = Activator.CreateInstance(GUINodeSelection.copyNode.GetType(), GUINodeSelection.copyNode) as GUINode;
                if (this._node.GetType() != typeof(GUICanvas))
                    (this._node.parent as GUINode).Node(copy);
                else
                    (this._node as GUINode).Node(copy);
                GUINodeSelection.copyNode = null;
            }
            protected virtual void OnCtrlD()
            {
                if (this._node.GetType() == typeof(GUICanvas)) return;
                GUINode copy = Activator.CreateInstance(this._node.GetType(), this._node) as GUINode;
                (this._node.parent as GUINode).Node(copy);
            }
            protected virtual void OnDelete()
            {
                if (this._node.GetType() == typeof(GUICanvas)) return;
                _node.Destoty();
                GUINodeSelection.node = null;
            }
        }


        private GUICanvas _canvas;
        private Trunk _root;
        private Vector2 _scroll;
        public bool handleEve = true;
        public GUICanvas canvas
        {
            get { return _canvas; }
            set
            {
                _canvas = value;
                _root = new Trunk(null, _canvas);
                _root.tree = this;
                canvas.OnCanvasTreeChange += _root.OnTreeChange;
            }
        }

        public override void OnGUI(Rect rect)
        {
            if (_root == null) return;
            Event e = Event.current;
            if (!rect.Contains(Event.current.mousePosition))
                GUINodeSelection.dragNode = null;
            _root.CalcHeight();
            var rs = rect.HorizontalSplit(_root.height);
            _scroll = GUI.BeginScrollView(rect, _scroll, rs[0]);
            _root.OnGUI(rs[0], e);
            GUI.EndScrollView();
            if (handleEve)
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
    class GUINodeInspectorView : DesignWindowItem
    {
        private Dictionary<Type, GUINodeEditor> dic = new Dictionary<Type, GUINodeEditor>();
        private GUINodeEditor Pan;
        private Vector2 scroll2;

        public GUINodeInspectorView()
        {
            var designs = GUINodeEditors.editorTypes.FindAll((t) => { return t.IsDefined(typeof(CustomGUINodeAttribute), false); });
            var eles = GUINodes.nodeTypes;
            foreach (var type in eles)
            {
                var typeTree = type.GetTypeTree();
                for (int i = 0; i < typeTree.Count; i++)
                {
                    Type des = designs.Find((t) => {
                        return (t.GetCustomAttributes(typeof(CustomGUINodeAttribute), false).First() as CustomGUINodeAttribute).EditType == typeTree[i];
                    });
                    if (des != null)
                    {
                        dic.Add(type, Activator.CreateInstance(des) as GUINodeEditor);
                        break;

                    }
                }
            }

            GUINodeSelection.onNodeChange += (ele) =>
            {
                if (ele != null)
                {
                    Pan = dic[ele.GetType()];
                    Pan.node = ele;
                }
            };
        }
        public override void OnGUI(Rect rect)
        {
            if (Pan == null) return;
            this.BeginArea(rect)
                    .BeginScrollView(ref scroll2)
                        .Pan(Pan.OnInspectorGUI)
                    .LayoutEndScrollView()
                .EndArea();
        }
    }
    partial class GUIDesignWindow
    {
        private const string SceneViewName = "Canvas";
        private const string InspectorViewName = "Design";
        private const string HierarchyViewName = "Tree";
        private const float ToolBarHeight = 22;
        private Rect _localPosition { get { return new Rect(Vector2.zero, position.size); } }

        [SerializeField] private GUICanvasHierarchyTree _hierarchy = new GUICanvasHierarchyTree();
        [SerializeField] private GUINodeInspectorView _inspector = new GUINodeInspectorView();
        [SerializeField] private GUINodeSceneView _scene = new GUINodeSceneView();
        private string _tmpSubWinLayout;
        private SubWinTree _subWin;
        private ToolBarTree _toolbar;

        [SerializeField]
        private string _xmlPath;
        [SerializeField]
        private string _tmpDesign;
        private GUICanvas _canvas;
    }
    [EditorWindowCache]
    partial class GUIDesignWindow : EditorWindow, ILayoutGUIDrawer
    {
        private void SubwinInit()
        {
            _subWin = new SubWinTree();
            _subWin.repaintEve += Repaint;
            _subWin.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };
            if (string.IsNullOrEmpty(_tmpSubWinLayout))
            {
                for (int i = 1; i <= 3; i++)
                {
                    string userdata = i == 1 ? InspectorViewName : i == 2 ? SceneViewName : HierarchyViewName;
                    SubWinTree.TreeLeaf L = _subWin.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    _subWin.DockLeaf(L, SubWinTree.DockType.Left);
                }
            }
            else
            {
                _subWin.DeSerialize(_tmpSubWinLayout);

            }
            _subWin[InspectorViewName].titleContent = new GUIContent(InspectorViewName);
            _subWin[InspectorViewName].paintDelegate += DesignGUI;

            _subWin[SceneViewName].titleContent = new GUIContent(SceneViewName);
            _subWin[SceneViewName].paintDelegate += CanvasGUI;

            _subWin[HierarchyViewName].titleContent = new GUIContent(HierarchyViewName);
            _subWin[HierarchyViewName].paintDelegate += TreeGUI;


            _toolbar = new ToolBarTree();
            _toolbar.DropDownButton(new GUIContent("Views"), (rect) => {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < _subWin.allLeafCount; i++)
                {
                    SubWinTree.TreeLeaf leaf = _subWin.allLeafs[i];
                    menu.AddItem(leaf.titleContent, !_subWin.closedLeafs.Contains(leaf), () => {
                        if (_subWin.closedLeafs.Contains(leaf))
                            _subWin.DockLeaf(leaf, SubWinTree.DockType.Left);
                        else
                            _subWin.CloseLeaf(leaf);
                    });
                }
                menu.DropDown(rect);
                Event.current.Use();

            }, 50)
                        .Toggle(new GUIContent("Title"), (bo) => { _subWin.isShowTitle = bo; }, _subWin.isShowTitle, 50)
                        .Toggle(new GUIContent("Lock"), (bo) => { _subWin.isLocked = bo; }, _subWin.isLocked, 50)
                        .FlexibleSpace()
                        .Delegate((r) =>
                        {
                            if (string.IsNullOrEmpty(_xmlPath))
                            {
                                GUI.Label(r, new GUIContent("Please Drag Xml File Here", "XmlPath:  " + _xmlPath));
                                r.DrawOutLine(2, Color.red);
                            }
                            else 
                            {
                                Rect r1 = r;
                                Rect r2 = r;
                                r2.xMax -= ToolBarHeight;
                                r1.xMin = r.xMax - ToolBarHeight;
                                GUI.Label(r2, new GUIContent(_xmlPath, "XmlPath:  " + _xmlPath));
                                r2.DrawOutLine(2, Color.black);
                                if (GUI.Button(r1, new GUIContent(EditorGUIUtility.IconContent("Refresh").image,"Clear XmlPath")))
                                {
                                    _xmlPath = string.Empty;
                                }
                            }
                                

                            Event e = Event.current;
                            if (!r.Contains(e.mousePosition)) return;

                            if (!string.IsNullOrEmpty(_xmlPath) && e.clickCount == 2)
                            {
                                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_xmlPath);
                            }
                            var info = DragAndDropUtil.Drag(e, r);
                            if (info.paths.Length == 1 && info.enterArera && info.compelete && info.paths[0].EndsWith(".xml") && System.IO.File.Exists(info.paths[0]))
                                _xmlPath = info.paths[0];

                        }, 200)
                        .Button(new GUIContent("Save"), (rect) =>
                        {
                            if (string.IsNullOrEmpty(_xmlPath) || !System.IO.File.Exists(_xmlPath))
                            {
                                ShowNotification(new GUIContent("Please Set Legal XmlPath"));
                                return;
                            }
                            XmlDocument doc = new XmlDocument();
                            doc.AppendChild(_canvas.Serialize(doc));
                            doc.Save(_xmlPath);
                            AssetDatabase.Refresh();
                        }, 50)
                        .Button(new GUIContent("Load"), (rect) =>
                        {
                            if (string.IsNullOrEmpty(_xmlPath) || !System.IO.File.Exists(_xmlPath))
                            {
                                ShowNotification(new GUIContent("Please Set Legal XmlPath"));
                                return;
                            }
                            XmlDocument doc = new XmlDocument();
                            doc.Load(_xmlPath);
                            _canvas.DeSerialize(doc.DocumentElement);
                            _hierarchy.canvas = _canvas;
                            _scene.canvas = _canvas;
                        }, 50);


        }
      
        private void TreeGUI(Rect rect)
        {
            _hierarchy.OnGUI(rect);
        }
        private void DesignGUI(Rect rect)
        {
            _inspector.OnGUI(rect);
        }
        private void CanvasGUI(Rect rect)
        {
            _scene.OnGUI(rect);
        }

        private void GUICanvasEditorViewInit()
        {
            if (string.IsNullOrEmpty(_tmpDesign))
            {
                _canvas = new GUICanvas() { canvasRect = new Rect(0, 0, 500, 500) }
                         .Node(new Button() { name = "Button", text = "123" });
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_tmpDesign);
                _canvas = new GUICanvas();
                _canvas.DeSerialize(doc.FirstChild as XmlElement);
            }
            _hierarchy.canvas = _canvas;
            _scene.canvas = _canvas;
            _subWin.onDragWindow += () => { _hierarchy.handleEve = false; };
            _subWin.onResizeWindow += () => { _hierarchy.handleEve = false; };
            _subWin.onEndDragWindow += () => { _hierarchy.handleEve = true; };
            _subWin.onEndResizeWindow += () => { _hierarchy.handleEve = true; };

        }
        private void SaveTmpInfo()
        {
            _tmpSubWinLayout = _subWin.Serialize();
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(_canvas.Serialize(doc));
            _tmpDesign = doc.InnerXml;
        }




        private void OnEnable()
        {
            SubwinInit();
            GUICanvasEditorViewInit();

            GUIScaleUtility.CheckInit();
            this.titleContent = new GUIContent("RectCanvas");
        }
        private void OnDisable()
        {
            SaveTmpInfo();
        }
        private void OnGUI()
        {
            var rs = _localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            _toolbar.OnGUI(rs[0]);
            _subWin.OnGUI(rs[1]);
            this.minSize = _subWin.minSize + new Vector2(0, ToolBarHeight);
            if (UnityEditor.SceneView.lastActiveSceneView != null) UnityEditor.SceneView.lastActiveSceneView.sceneViewState.Toggle(true);
            Repaint();
        }
    }
}
