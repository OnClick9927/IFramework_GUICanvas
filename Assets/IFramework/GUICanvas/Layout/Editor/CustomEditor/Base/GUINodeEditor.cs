/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    public class GUINodeEditor : ILayoutGUIDrawer
    {
        protected class GUIStyleDesign
        {
            public class GUIStyleStateDesign
            {
                public string name;
                public GUIStyleState state;

                public GUIStyleStateDesign(string name, GUIStyleState state)
                {
                    this.name = name;
                    this.state = state;
                    //state.scaledBackgrounds = new Texture2D[2];

                }

                private bool foldon;
                private bool foldonList;
                public void OnGUI()
                {
                    foldon = EditorGUILayout.Foldout(foldon, name, true);
                    if (!foldon) return;
                    HorizontalView(() => {
                        GUILayout.Space(20);
                        VerticalView(() => {
                            state.textColor = EditorGUILayout.ColorField("Text Color", state.textColor);
                            state.background = (Texture2D)EditorGUILayout.ObjectField("Back Ground", state.background, typeof(Texture2D), false);
                            foldonList = EditorGUILayout.Foldout(foldonList, "Scaled Backgrounds", true);

                            if (foldonList)
                            {
                                HorizontalView(() =>
                                {
                                    GUILayout.Space(20);
                                    VerticalView(() => {
                                        int size = EditorGUILayout.IntField("Size", state.scaledBackgrounds.Length);

                                        if (size != state.scaledBackgrounds.Length)
                                        {
                                            Texture2D[] txs = new Texture2D[state.scaledBackgrounds.Length];
                                            for (int i = 0; i < state.scaledBackgrounds.Length; i++)
                                            {
                                                txs[i] = txs[i];
                                            }
                                            state.scaledBackgrounds = new Texture2D[size];
                                            for (int i = 0; i < (txs.Length < size ? txs.Length : size); i++)
                                            {
                                                state.scaledBackgrounds[i] = txs[i];
                                            }
                                        }
                                        List<Texture2D> tts = new List<Texture2D>();
                                        tts = state.scaledBackgrounds.ToList();
                                        for (int i = 0; i < tts.Count; i++)
                                        {
                                            tts[i] = (Texture2D)EditorGUILayout.ObjectField("Back Ground", tts[i], typeof(Texture2D), false);
                                        }
                                        state.scaledBackgrounds = tts.ToArray();
                                    }, GUIStyle.none);
                                }, GUIStyle.none);
                            }


                        }, GUIStyle.none);
                    }, GUIStyle.none);
                }
            }
            public class RectOffsetDesign
            {
                private RectOffset offset;
                private string name;

                public RectOffsetDesign(string name, RectOffset offset)
                {
                    this.name = name;
                    this.offset = offset;
                }

                private bool foldon;
                public void OnGUI()
                {
                    foldon = EditorGUILayout.Foldout(foldon, name, true);
                    if (foldon)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();
                            {
                                offset.left = EditorGUILayout.IntField("Left", offset.left);
                                offset.right = EditorGUILayout.IntField("Right", offset.right);
                                offset.top = EditorGUILayout.IntField("Top", offset.top);
                                offset.bottom = EditorGUILayout.IntField("Bottom", offset.bottom);
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                    }

                }
            }

            private GUIStyle style;
            private string name;
            private List<GUIStyleStateDesign> states;
            private List<RectOffsetDesign> offsets;

            public GUIStyleDesign(GUIStyle style, string name)
            {
                this.name = name;
                this.style = style;
                states = new List<GUIStyleStateDesign>();
                offsets = new List<RectOffsetDesign>();
                states.Add(new GUIStyleStateDesign("normal", style.normal));
                states.Add(new GUIStyleStateDesign("onNormal", style.onNormal));
                states.Add(new GUIStyleStateDesign("hover", style.hover));
                states.Add(new GUIStyleStateDesign("onHover", style.onHover));
                states.Add(new GUIStyleStateDesign("active", style.active));
                states.Add(new GUIStyleStateDesign("onActive", style.onActive));
                states.Add(new GUIStyleStateDesign("focused", style.focused));
                states.Add(new GUIStyleStateDesign("onFocused", style.onFocused));
                offsets.Add(new RectOffsetDesign("border", style.border));
                offsets.Add(new RectOffsetDesign("margin", style.margin));
                offsets.Add(new RectOffsetDesign("padding", style.padding));
                offsets.Add(new RectOffsetDesign("overflow", style.overflow));
            }

            private bool foldon;
            public void OnGUI()
            {
                foldon = EditorGUILayout.Foldout(foldon, name, true);
                if (!foldon) return;
                HorizontalView(() => {
                    GUILayout.Space(20);
                    VerticalView(() => {
                        style.name = EditorGUILayout.TextField("Name", style.name);
                        states.ForEach((s) => { s.OnGUI(); });
                        offsets.ForEach((o) => { o.OnGUI(); });
                        style.font = EditorGUILayout.ObjectField("Font", style.font, typeof(Font), false) as Font;
                        style.fontSize = EditorGUILayout.IntField("Font Stize", style.fontSize);
                        style.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", style.fontStyle);
                        style.alignment = (TextAnchor)EditorGUILayout.EnumPopup("Alignment", style.alignment);
                        style.wordWrap = EditorGUILayout.Toggle("Word Wrap", style.wordWrap);
                        style.richText = EditorGUILayout.Toggle("Rich Text", style.richText);

                        style.clipping = (TextClipping)EditorGUILayout.EnumPopup("Over flow", style.clipping);
                        style.imagePosition = (ImagePosition)EditorGUILayout.EnumPopup("Image Position", style.imagePosition);

                        style.contentOffset = EditorGUILayout.Vector2Field("Content Offset", style.contentOffset);
                        style.fixedWidth = EditorGUILayout.FloatField("Fixed Width", style.fixedWidth);
                        style.fixedHeight = EditorGUILayout.FloatField("Fixed Height", style.fixedHeight);
                        style.stretchWidth = EditorGUILayout.Toggle("Stretch Width", style.stretchWidth);
                        style.stretchHeight = EditorGUILayout.Toggle("Stretch Height", style.stretchHeight);

                    }, GUIStyle.none);
                }, GUIStyle.none);
            }
        }

        public GUINode node;
        private bool insFold = true;

        public virtual void OnInspectorGUI()
        {
            insFold = FormatFoldGUI(insFold, "Element:" + node.GetType().Name, HeadGUI, ContentGUI);
        }
        private void ContentGUI()
        {
            this.Pan(() =>
            {
                node.name = EditorGUILayout.TextField("Name", node.name);
                using (new EditorGUI.DisabledGroupScope(true)) EditorGUILayout.RectField("Position", node.position);
                node.enable = EditorGUILayout.Toggle("Enable", node.enable);
            })
                .FloatField("Rotate Angle", ref node.rotateAngle)
                .Vector2Field("Rotate Offset", ref node.rotateOffset)
                .ColorField("Color", ref node.color)
                .ColorField("Content Color", ref node.contentColor)
                .ColorField("Background Color", ref node.backgroundColor)
                .BeginVertical("box")
                    .Label("GUILayoutOptions", EditorStyles.boldLabel)
                    .Foldout(ref node.enableSize, "Enable Size", true)
                    .Pan(() => { if (node.enableSize) this.Vector2Field("Size", ref node.size); })
                    .Foldout(ref node.enableMinSize, "Enable Min Size", true)
                    .Pan(() => { if (node.enableMinSize) this.Vector2Field("Min Size", ref node.minSize); })
                    .Foldout(ref node.enableMaxSize, "Enable Max Size", true)
                    .Pan(() => { if (node.enableMaxSize) this.Vector2Field("Max Size", ref node.maxSize); })
                    .Foldout(ref node.enableExpandHeight, "Enable Expand Height", true)
                    .Pan(() => { if (node.enableExpandHeight) this.Toggle("Expand Height", ref node.expandHeight); })
                    .Foldout(ref node.enableExpandWidth, "Enable Expand Width", true)
                    .Pan(() => { if (node.enableExpandWidth) this.Toggle("Expand Width", ref node.expandWidth); })
                .EndVertical();

        }

        private bool HeadGUI(bool bo, string title)
        {
            this.BeginHorizontal()
                    .Foldout(ref bo, title, true)
                    .Pan(() => { node.active = EditorGUILayout.Toggle(node.active, GUILayout.Width(18)); })
                    .Button(() => { node.Reset(); }, EditorGUIUtility.IconContent("d_TreeEditor.Refresh"), GUILayout.Width(25))
                .EndHorizontal();
            return bo;
        }

        private static void HorizontalView(Action draw, GUIStyle style)
        {
            if (style == GUIStyle.none)
                GUILayout.BeginHorizontal();
            else
                GUILayout.BeginHorizontal(style);
            if (draw != null) draw();
            GUILayout.EndHorizontal();
        }
        private static void VerticalView(Action draw, GUIStyle style)
        {
            if (style == GUIStyle.none)
                GUILayout.BeginVertical();
            else
                GUILayout.BeginVertical(style);
            if (draw != null) draw();
            GUILayout.EndVertical();
        }
        protected static bool FormatFoldGUI(bool fold, string title, Func<bool, string, bool> titledraw, Action draw)
        {
            VerticalView(() => {
                if (titledraw == null)
                    fold = EditorGUILayout.Foldout(fold, title, true);
                else
                    fold = titledraw(fold, title);
                HorizontalView(() => {
                    GUILayout.Space(20);
                    VerticalView(() => {
                        if (fold && draw != null)
                            draw();
                    }, GUIStyle.none);
                }, GUIStyle.none);
            }, "box");
            return fold;
        }

        public virtual void OnSceneGUI(Action child) { }
        private Color preContentColor;
        private Color preBgColor;
        private Color preColor;
        private GUISkin preSkin;
        private Matrix4x4 preMat4x4;
        private bool preEnable;
        protected void BeginGUI()
        {
            preSkin = GUI.skin;
            preContentColor = GUI.contentColor;
            preBgColor = GUI.backgroundColor;
            preColor = GUI.color;
            preMat4x4 = GUI.matrix;
            preEnable = GUI.enabled;

            GUI.color = node.color;
            GUI.backgroundColor = node.backgroundColor;
            GUI.contentColor = node.contentColor;
            Vector2 tmp = new Vector2(node.rotateOffset.x * node.position.width, node.rotateOffset.y * node.position.height);
            GUIUtility.RotateAroundPivot(node.rotateAngle, node.position.center + tmp);

            //bool bo = this.element.enable;
            //if (bo)
            //{
            //    GUINode d = this.element;
            //    while (d.parent != null)
            //    {
            //        d = d.parent;
            //        bo = d.enable && bo;
            //        if (!bo) break;
            //    }
            //}
            //GUI.enabled = bo;
            GUI.enabled = false;

        }
        protected void EndGUI()
        {
            if (GUINodeSelection.node == this.node)
                node.position.DrawOutLine(2, Color.magenta);
            GUI.backgroundColor = preBgColor;
            GUI.contentColor = preContentColor;
            GUI.skin = preSkin;
            GUI.matrix = preMat4x4;
            GUI.color = preColor;
            GUI.enabled = preEnable;
        }
        protected GUILayoutOption[] CalcGUILayOutOptions()
        {
            List<GUILayoutOption> options = new List<GUILayoutOption>();
            if (node.enableSize)
            {
                options.Add(GUILayout.Width(node.size.x));
                options.Add(GUILayout.Height(node.size.y));
            }
            if (node.enableMinSize)
            {
                options.Add(GUILayout.MinWidth(node.minSize.x));
                options.Add(GUILayout.MinHeight(node.minSize.y));
            }
            if (node.enableMaxSize)
            {
                options.Add(GUILayout.MaxWidth(node.maxSize.x));
                options.Add(GUILayout.MaxHeight(node.maxSize.y));
            }
            if (node.enableExpandHeight)
                options.Add(GUILayout.ExpandHeight(node.expandHeight));
            if (node.enableExpandWidth)
                options.Add(GUILayout.ExpandWidth(node.expandWidth));
            return options.ToArray();
        }

    }

}
