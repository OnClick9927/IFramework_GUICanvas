/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [CustomGUINodeAttribute(typeof(TextNode))]
    public class TextNodeEditor : GUINodeEditor
    {
        private TextNode textElement { get { return node as TextNode; } }
        private bool insFold = true;
        private GUIStyleDesign textStyleDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (textStyleDrawer == null)
                textStyleDrawer = new GUIStyleDesign(textElement.style, "Text Style");
            insFold = FormatFoldGUI(insFold, "Text", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.LabelField("Text")
                .TextArea(ref textElement.text, GUILayout.Height(50))
                .LabelField("Tooltip")
                .TextArea(ref textElement.tooltip, GUILayout.Height(50))
                .ObjectField("Font", ref textElement.font, false)
                .IntField("Font Stize", ref textElement.fontSize)
                .Toggle("Rich Text", ref textElement.richText)
                .Pan(() => {
                    textElement.overflow = (TextClipping)EditorGUILayout.EnumPopup("Over flow", textElement.overflow);
                    textElement.alignment = (TextAnchor)EditorGUILayout.EnumPopup("Alignment", textElement.alignment);
                    textElement.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", textElement.fontStyle);
                });

            textStyleDrawer.OnGUI();
        }
        public override void OnSceneGUI(Action children)
        {
            if (node.active)
            {
                textElement.style.font = textElement.font;
                textElement.style.fontStyle = textElement.fontStyle;
                textElement.style.fontSize = textElement.fontSize;
                textElement.style.alignment = textElement.alignment;
                textElement.style.clipping = textElement.overflow;
                textElement.style.richText = textElement.richText;
            }
        }
    }
}
