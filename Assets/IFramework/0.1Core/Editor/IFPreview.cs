/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IFramework
{
    public class IFPreview
    {
        public PreviewRenderUtility preview { get { return _preview; } }
        private PreviewRenderUtility _preview;
        private MeshFilter[] filters;
        private Renderer[] renderers;
        private GameObject go;
        private Dictionary<Renderer, Material[]> _dic = new Dictionary<Renderer, Material[]>();
        public Color skyboxColor = Color.white;
        public bool useLight;
        public Rect rect;
        public GUIStyle previewStyle = GUIStyle.none;
        public IFPreview(GameObject go, MeshFilter[] filters, Renderer[] renderers)
        {
            this.go = GameObject.Instantiate(go);
            this.filters = filters;
            this.renderers = renderers;
            for (int i = 0; i < renderers.Length; i++)
            {
                _dic.Add(renderers[i], renderers[i].sharedMaterials);
            }
            _preview = new PreviewRenderUtility(false);
            _preview.AddSingleGO(this.go);
            GameObject.DestroyImmediate(this.go);
        }



        public Texture Render(Rect rect, Matrix4x4 matrix)
        {
            this.rect = rect;
            _preview.BeginPreview(rect, previewStyle);
            for (int i = 0; i < filters.Length; i++)
            {
                for (int j = 0; j < _dic[renderers[i]].Length; j++)
                {
                    _preview.DrawMesh(filters[i].sharedMesh, matrix, _dic[renderers[i]][j], j);

                }
            }
            if (useLight)
            {
                InternalEditorUtility.SetCustomLighting(_preview.lights, skyboxColor);
                _preview.camera.Render();
                InternalEditorUtility.RemoveCustomLighting();
            }
            else
            {
                _preview.camera.Render();
            }
            return _preview.EndPreview();
        }

        public void Cleanup()
        {
            if (_preview != null)
            {
                _preview.Cleanup();
                //UnityEngine.Object.DestroyImmediate(this.mPreviewMaterial.shader, false);
                // UnityEngine.Object.DestroyImmediate(this.mPreviewMaterial, false);
                _dic.Clear();
            }
            _preview = null;
        }


    }
}
