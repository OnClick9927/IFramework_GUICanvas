/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-10-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUITool.LayoutDesign
{
    static class GUINodeEditors
    {
        public static List<Type> editorTypes = typeof(GUINodeEditor).GetSubTypesInAssemblys().ToList();
    }
    abstract class DesignWindowItem: ILayoutGUIDrawer
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
        public delegate void OnElementChange(GUINode node);
        public static event OnElementChange onNodeChange;
        private static GUINode _node;
        public static GUINode node
        {
            get { return _node; }
            set
            {
                _node = value;

                if (EditorWindow.focusedWindow != null)
                    EditorWindow.focusedWindow.Repaint();

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
                for (int i = 0; i < ele.children.Count; i++)
                {
                    EleGUI(ele.children[i] as GUINode);
                }
            });

        }

    }
    class GUICanvasHierarchyTree: DesignWindowItem
    {
        private class Trunk
        {
            public const float LineHeight = 17;
            private bool _fold = true;
            private GUINode _element;
            private GUICanvasHierarchyTree _tree;
            private List<Trunk> _children;
            private RenameLabelDrawer _nameLabel;

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
            public Trunk parent;

            public Trunk(Trunk parent, GUINode element)
            {
                _nameLabel = new RenameLabelDrawer();
                _nameLabel.value = element.name;
                _nameLabel.onEndEdit += (str) =>
                {
                    element.name = str.Trim();
                };

                this._element = element;
                this.parent = parent;
                _children = new List<Trunk>();
                if (element.children.Count > 0)
                {
                    for (int i = 0; i < element.children.Count; i++)
                    {
                        _children.Add(new Trunk(this, element.children[i] as GUINode));
                    }
                }
            }

            public void OnTreeChange()
            {
                List<Trunk> tep = new List<Trunk>();
                tep.AddRange(_children);
                _children.Clear();
                for (int i = 0; i < _element.children.Count; i++)
                {
                    var trunk = tep.Find((t) => { return t._element == _element.children[i]; });
                    if (trunk == null)
                    {
                        _children.Add(new Trunk(this, _element.children[i] as GUINode));
                    }
                    else
                    {
                        tep.Remove(trunk);
                        _children.Add(trunk);
                    }
                }
                tep.ForEach((t) => { t._nameLabel.Dispose(); });
                _children.ForEach((t) => { t.OnTreeChange(); });
            }



            public float Height { get; private set; }
            public float CalcHeight()
            {
                if (!_fold || _children.Count == 0)
                {
                    Height = LineHeight;
                }
                else
                {
                    Height = LineHeight;
                    for (int i = 0; i < _children.Count; i++)
                    {
                        Height += _children[i].CalcHeight();
                    }
                }
                return Height;
            }
            public void OGUI(Rect rect, Event e)
            {
                bool active = _element.active;
                if (active)
                {
                    GUINode ele = _element;
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
                if (GUINodeSelection.node == this._element && e.type == EventType.Repaint)
                    new GUIStyle("SelectionRect").Draw(selfRect, false, false, false, false);
                selfRect.xMin += 20 * _element.depth;
                Rect childrenRect = rs[1];
                childrenRect.xMin += 20 * _element.depth;

                if (_children.Count > 0)
                {
                    var rss = selfRect.VerticalSplit(12);
                    _fold = EditorGUI.Foldout(rss[0], _fold, "", false);
                    _nameLabel.OnGUI(rss[1]);
                    //GUI.Label(rss[1], element.name);
                    if (tree.handleEve)
                    {
                        Eve(rss[1], e);
                    }
                    if (!_fold) return;
                    float y = 0;
                    for (int i = 0; i < _children.Count; i++)
                    {
                        Rect r = new Rect(rect.x, childrenRect.y + y, rect.width, _children[i].Height);
                        y += _children[i].Height;
                        _children[i].OGUI(r, e);
                    }
                }
                else
                {
                    _nameLabel.OnGUI(selfRect);

                    if (tree.handleEve)
                    {
                        Eve(selfRect, e);
                    }
                }
            }
            private void Eve(Rect r, Event e)
            {
                MouseDragEve(r, e);
                if (r.Contains(e.mousePosition) /*&& e.type == EventType.MouseDown */&& e.clickCount == 1 && e.button == 0) GUINodeSelection.node = this._element;
                if (r.Contains(e.mousePosition) && GUINodeSelection.node == this._element)
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
            private void MouseDragEve(Rect r, Event e)
            {
                if (tree._isDragOtherWindow)
                {
                    GUINodeSelection.dragNode = null;
                }
                bool CouldPutdown = r.Contains(e.mousePosition) && GUINodeSelection.dragNode != null && GUINodeSelection.dragNode != this._element;
                GUINode tmp = this._element;
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
                    GUI.Box(r, "", this._element.GetType().IsSubclassOf(typeof(ParentGUINode)) && _fold ? "SelectionRect" : "PR Insertion");
                if (CouldPutdown && e.type == EventType.MouseUp)
                {
                    if (this._element.GetType().IsSubclassOf(typeof(ParentGUINode)) && _fold)
                    {
                        (this._element as ParentGUINode).Node(GUINodeSelection.dragNode);
                    }
                    else
                    {
                        (this._element.parent as ParentGUINode).Node(GUINodeSelection.dragNode);
                        GUINodeSelection.dragNode.siblingIndex = _element.siblingIndex + 1;
                    }
                    GUINodeSelection.dragNode = null;
                }
                else if (GUINodeSelection.node == this._element)
                {
                    if (e.type == EventType.MouseDrag)
                    {
                        GUINodeSelection.dragNode = GUINodeSelection.node;
                    }
                    else if (e.type == EventType.MouseUp)
                    {
                        GUINodeSelection.dragNode = null;
                    }
                }


            }
            private void OnMenu(GenericMenu menu)
            {
                var types = GUINodes.nodeTypes
                .FindAll((type) => { return !type.IsAbstract && type.IsDefined(typeof(GUINodeAttribute), false); });
                types.ForEach((type) =>
                {
                    string createPath = (type.GetCustomAttributes(typeof(GUINodeAttribute), false).First() as GUINodeAttribute).path;
                    menu.AddItem(new GUIContent("Create/" + createPath), false, () => { OnCeateElement(type); });
                });
                if (GUINodeSelection.copyNode == null)
                    menu.AddDisabledItem(new GUIContent("Paste"));
                else
                    menu.AddItem(new GUIContent("Paste"), false, OnCtrlV);
                menu.AddItem(new GUIContent("Reset"), false, _element.Reset);
                if (!(_element is GUICanvas))
                {
                    menu.AddItem(new GUIContent("Copy"), false, OnCtrlC);
                    menu.AddItem(new GUIContent("Duplicate"), false, OnCtrlD);
                    menu.AddItem(new GUIContent("Delete"), false, OnDelete);
                    if (_element.siblingIndex == 0)
                        menu.AddDisabledItem(new GUIContent("MoveUp"));
                    else
                        menu.AddItem(new GUIContent("MoveUp"), false, OnMoveUp);
                    if (_element.siblingIndex == _element.parent.children.Count - 1)
                        menu.AddDisabledItem(new GUIContent("MoveDown"));
                    else
                        menu.AddItem(new GUIContent("MoveDown"), false, OnMoveDown);
                }
            }


            private void OnCeateElement(Type type)
            {
                GUINode copy = Activator.CreateInstance(type) as GUINode;
                if (!this._element.GetType().IsSubclassOf(typeof(ParentGUINode)))
                    (this._element.parent as ParentGUINode).Node(copy);
                else
                    (this._element as ParentGUINode).Node(copy);
            }
            private void OnMoveUp()
            {
                int tmp = _element.siblingIndex;
                if (tmp != 0)
                {
                    _element.siblingIndex = tmp - 1;
                }
            }
            private void OnMoveDown()
            {
                int tmp = _element.siblingIndex;
                if (tmp != _element.parent.children.Count - 1)
                {
                    _element.siblingIndex = tmp + 1;
                }
            }
            protected virtual void OnCtrlC()
            {
                if (this._element.GetType() != typeof(GUICanvas))
                {
                    GUINodeSelection.copyNode = this._element;
                }
            }
            protected virtual void OnCtrlV()
            {
                if (GUINodeSelection.copyNode == null) return;
                GUINode copy = Activator.CreateInstance(GUINodeSelection.copyNode.GetType(), GUINodeSelection.copyNode) as GUINode;
                if (this._element.GetType() != typeof(GUICanvas))
                    (this._element.parent as ParentGUINode).Node(copy);
                else
                    (this._element as ParentGUINode).Node(copy);
                GUINodeSelection.copyNode = null;
            }
            protected virtual void OnCtrlD()
            {
                if (this._element.GetType() == typeof(GUICanvas)) return;
                GUINode copy = Activator.CreateInstance(this._element.GetType(), this._element) as GUINode;
                (this._element.parent as ParentGUINode).Node(copy);
                GUINodeSelection.copyNode = null;
            }
            protected virtual void OnDelete()
            {
                if (this._element.GetType() == typeof(GUICanvas)) return;
                _element.Destoty();
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
                _canvas.OnCanvasTreeChange += _root.OnTreeChange;
            }
        }

        public override void OnGUI(Rect rect)
        {
            if (_root == null) return;
            Event e = Event.current;
            if (!rect.Contains(Event.current.mousePosition))
                GUINodeSelection.dragNode = null;
            _root.CalcHeight();
            var rs = rect.HorizontalSplit(_root.Height);

            HandleDragOther(rs[0]);
            _scroll = GUI.BeginScrollView(rect, _scroll, rs[0]);
            _root.OGUI(rs[0], e);
            GUI.EndScrollView();
            if (handleEve)
            {
                EmptyEve(e, rs[1]);
            }
        }
        private void EmptyEve(Event e, Rect r)
        {
            //r.DrawOutLine(12, Color.red);
            if (r.height > 0 && r.Contains(e.mousePosition) && e.type == EventType.MouseUp)
            {
                //dragTrunk = null;
                GUINodeSelection.node = null;
                GUINodeSelection.dragNode = null;
            }
        }
    }
    class GUINodeInspectorView : DesignWindowItem
    {
        private Dictionary<Type, GUINodeEditor> _dic = new Dictionary<Type, GUINodeEditor>();
        private GUINodeEditor _pan;
        private Vector2 _scroll;

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
                        _dic.Add(type, Activator.CreateInstance(des) as GUINodeEditor);
                        break;

                    }
                }
            }

            GUINodeSelection.onNodeChange += (ele) =>
            {
                if (ele != null)
                {
                    _pan = _dic[ele.GetType()];
                    _pan.node = ele;
                }
            };
        }
        public override void OnGUI(Rect rect)
        {
            if (_pan == null) return;
            this.BeginArea(rect)
                    .BeginScrollView(ref _scroll)
                        .Pan(_pan.OnInspectorGUI)
                    .LayoutEndScrollView()
                .EndArea();
        }
    }

    [EditorWindowCache]
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
                                if (GUI.Button(r1, new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Clear XmlPath")))
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

            this.titleContent = new GUIContent("LayoutCanvas");
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
