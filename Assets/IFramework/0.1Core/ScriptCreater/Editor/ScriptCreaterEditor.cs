/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IFramework
{
    [CustomEditor(typeof(ScriptCreater))]
    public class ScriptCreaterEditor : Editor,ILayoutGUIDrawer
    {
        private static string originScriptPath;
        private static ScriptCreater _scriptCreater;
        private void OnEnable()
        {
            originScriptPath = EditorEnv.corePath.CombinePath(@"ScriptCreater/Editor/FormatScript.txt");
            _scriptCreater = this.target as ScriptCreater;
        }
      

        public override void OnInspectorGUI()
        {
            if (_scriptCreater == null) _scriptCreater = this.target as ScriptCreater;
            _scriptCreater = this.target as ScriptCreater;

            this.Space(10)
                .ETextField("Script Name", ref _scriptCreater.ScriptName)
                .Pan(() => {
                    if (!_scriptCreater.ScriptName.IsLegalFieldName())
                        _scriptCreater.ScriptName = _scriptCreater.name.Replace(" ", "").Replace("(", "").Replace(")", "");
                })
                .Label("Description")
                .TextArea(ref _scriptCreater.description, GUILayout.Height(40))
                .Space(10)
                .DrawHorizontal(() => {
                    GUILayout.Label(new GUIContent("Create Path:", "Drag Floder To Box"));
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.DrawOutLine(2, Color.black);
                    EditorGUI.LabelField(rect, _scriptCreater.CreatePath);
                    if (!rect.Contains(Event.current.mousePosition)) return;
                    var info = DragAndDropUtil.Drag(Event.current, rect);
                    if (info.paths.Length > 0 && info.compelete && info.enterArera && info.paths[0].IsDirectory())
                        _scriptCreater.CreatePath = info.paths[0];
                })
                .Space(10)
                .Toggle("Create Prefab", ref _scriptCreater.isCreatePrefab)
                .Pan(()=> {
                    if (_scriptCreater.isCreatePrefab)
                    {
                        this.ETextField("Prefab Name", ref _scriptCreater.prefabName);
                        this.DrawHorizontal(() =>
                        {
                            GUILayout.Label(new GUIContent("Prefab Path:", "Drag Floder To Box"));
                            Rect rect = EditorGUILayout.GetControlRect();
                            rect.DrawOutLine(2, Color.black);
                            EditorGUI.LabelField(rect, _scriptCreater.prefabDirectory);
                            if (!rect.Contains(Event.current.mousePosition)) return;
                            var info = DragAndDropUtil.Drag(Event.current, rect);
                            if (info.paths.Length > 0 && info.compelete && info.enterArera && info.paths[0].IsDirectory())
                                _scriptCreater.prefabDirectory = info.paths[0];
                        });
                    }
                })
                .Space(10)
                .DrawHorizontal(() => {

                    if (GUILayout.Button("Build", GUILayout.Height(25)))
                    {
                        if (BuildCheck())
                        {
                            Selection.objects = new Object[] { AssetDatabase.LoadAssetAtPath<Object>(_scriptCreater.CreatePath) };
                            CopyAsset.CopyNewAsset(_scriptCreater.ScriptName.Append(".cs"), originScriptPath);
                        }
                    }
                    if (GUILayout.Button("Remove", GUILayout.Height(25)))
                    {
                        _scriptCreater.GetComponentsInChildren<ScriptMark>(true).ToList().ForEach((sm) => {
                            DestroyImmediate(sm);
                        });
                        DestroyImmediate(_scriptCreater);
                    }
                });
            serializedObject.Update();

        }
        private bool BuildCheck()
        {
            if (EditorApplication.isCompiling) return false;
            _scriptCreater.scriptMarks = _scriptCreater.GetComponentsInChildren<ScriptMark>(true);
            for (int i = 0; i < _scriptCreater.scriptMarks.Length; i++)
            {
                var _sm = _scriptCreater.scriptMarks[i];
                if (_sm.fieldName == _scriptCreater.ScriptName)
                {
                    EditorUtility.DisplayDialog("Err", "Field Name Should be diferent With ScriptName", "ok");
                    return false;
                }
                var sameFields= _scriptCreater.scriptMarks.ToList().FindAll((__sm) => { return _sm.fieldName == __sm.fieldName; });
                if (sameFields.Count>1)
                {
                    EditorUtility.DisplayDialog("Err", "Can't Exist Same Name Field", "ok");
                    return false;
                }
                
            }

            CreateOriginIfNull();
            if (!Directory.Exists(_scriptCreater.CreatePath))
            {
                EditorUtility.DisplayDialog("Err", "Directory Not Exist ", "ok");
                return false;
            }
            if (_scriptCreater.isCreatePrefab)
            {
                if (!Directory.Exists(_scriptCreater.prefabDirectory))
                {
                    EditorUtility.DisplayDialog("Err", "Prefab Directory Not Exist ", "ok");
                    return false;
                }
                if (File.Exists(_scriptCreater.prefabPath))
                {
                    AssetDatabase.DeleteAsset(_scriptCreater.prefabPath);
                    AssetDatabase.Refresh();
                }
            }
            return true;
        }
        private void CreateOriginIfNull()
        {
            if (File.Exists(originScriptPath)) return;
            using (FileStream fs = new FileStream(originScriptPath, FileMode.Create, FileAccess.Write))
            {

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Lock(0, fs.Length);
                    sw.WriteLine("/*********************************************************************************");
                    sw.WriteLine(" *Author:         #SCAuthor#");
                    sw.WriteLine(" *Version:        #SCVERSION#");
                    sw.WriteLine(" *UnityVersion:   #SCUNITYVERSION#");
                    sw.WriteLine(" *Date:           #SCDATE#");
                    sw.WriteLine(" *Description:    #SCDescription#");
                    sw.WriteLine(" *History:        #SCDATE#--");
                    sw.WriteLine("*********************************************************************************/");
                    sw.WriteLine("using System.Collections;");
                    sw.WriteLine("using System.Collections.Generic;");
                    //sw.WriteLine("using UnityEngine;");
                    sw.WriteLine("using IFramework;");
                    sw.WriteLine("#SCUsing#");
                    sw.WriteLine("namespace #SCNameSpace#");
                    sw.WriteLine("{");
                    sw.WriteLine("\tpublic class #SCSCRIPTNAME# : MonoBehaviour");
                    sw.WriteLine("\t{");
                    sw.WriteLine("#SCField#");
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    fs.Unlock(0, fs.Length);
                    sw.Flush();
                    fs.Flush();
                }
            }
            AssetDatabase.Refresh();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void SetFields()
        {
            if (!EditorPrefs.GetBool(IsCreateKey, false)) return;
            if (!EditorPrefs.HasKey(ScriptNameKey) || !EditorPrefs.HasKey(GameobjKey))
            {
                if (EditorPrefs.HasKey(ScriptNameKey))
                    EditorPrefs.DeleteKey(ScriptNameKey);
                if (EditorPrefs.HasKey(GameobjKey))
                    EditorPrefs.DeleteKey(GameobjKey);
                EditorPrefs.SetBool(IsCreateKey, false);
                EditorUtility.ClearProgressBar();
                return;
            }

            Assembly defaultAssembly = AppDomain.CurrentDomain.GetAssemblies()
                            .First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            Type type = defaultAssembly.GetType(ProjectConfig.NameSpace + "." + EditorPrefs.GetString(ScriptNameKey));
            GameObject gameObj = GameObject.Find(EditorPrefs.GetString(GameobjKey));

            if (gameObj == null || type == null || gameObj.GetComponent<ScriptCreater>() == null)
            {
                if (EditorPrefs.HasKey(ScriptNameKey))
                    EditorPrefs.DeleteKey(ScriptNameKey);
                if (EditorPrefs.HasKey(GameobjKey))
                    EditorPrefs.DeleteKey(GameobjKey);
                EditorPrefs.SetBool(IsCreateKey, false);
                EditorUtility.ClearProgressBar();
                return;
            }
            EditorUtility.DisplayProgressBar("Build Script  " + type.Name, "Don't do anything", 0.7f);
            _scriptCreater = gameObj.GetComponent<ScriptCreater>();

            ScriptMark[] scriptMarks = _scriptCreater.scriptMarks;
            Component component = gameObj.GetComponent(type);
            if (component == null) component = gameObj.AddComponent(type);
            SerializedObject serialiedScript = new SerializedObject(component);

            foreach (var _sm in scriptMarks)
            {
                serialiedScript.FindProperty(_sm.fieldName).objectReferenceValue = _sm.GetComponent(_sm.fieldType);
            }
            serialiedScript.ApplyModifiedPropertiesWithoutUndo();
            //serialiedScript.Update();
            if (_scriptCreater.isCreatePrefab)
            {
                EditorUtil.CreatePrefab(gameObj, _scriptCreater.prefabPath);
            }



            EditorPrefs.SetBool(IsCreateKey, false);
            EditorPrefs.DeleteKey(ScriptNameKey);
            EditorPrefs.DeleteKey(GameobjKey);
            EditorUtility.ClearProgressBar();
        }
        const string ScriptNameKey = "ScriptCreaterEditorSpName";
        const string GameobjKey = "ScriptCreaterEditorGo";
        const string IsCreateKey = "ScriptCreaterEditor";
        private class CreateScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            private static void OnWillCreateAsset(string metaPath)
            {
                if (_scriptCreater == null) return;
                string filePath = metaPath.Replace(".meta", "");
                if (!filePath.EndsWith(".cs")) return;
                string realPath = filePath.ToAbsPath();
                string txt = File.ReadAllText(realPath);
                if (!txt.Contains("#SCAuthor#")) return;
                string spName = Path.GetFileNameWithoutExtension(filePath);
                EditorUtility.DisplayProgressBar("Build Script  " + spName, "Don't do anything", 0.1f);
                EditorPrefs.SetString(ScriptNameKey, spName);
                EditorPrefs.SetString(GameobjKey, _scriptCreater.name);
                EditorPrefs.SetBool(IsCreateKey, true);

                txt = txt.Replace("#SCAuthor#", ProjectConfig.UserName)
                         .Replace("#SCVERSION#", ProjectConfig.Version)
                         .Replace("#SCUNITYVERSION#", Application.unityVersion)
                         .Replace("#SCDATE#", DateTime.Now.ToString("yyyy-MM-dd"))
                         .Replace("#SCNameSpace#", ProjectConfig.NameSpace)
                         .Replace("#SCSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                         .Replace("#SCDescription#", ReplaceDescription());
                txt = txt.Replace("#SCField#", ReplaceField(txt));
                txt = txt.Replace("#SCUsing#", ReplaceNamespace(txt));

                EditorUtility.DisplayProgressBar("Build Script  " + spName, "Don't do anything", 0.2f);

                File.WriteAllText(realPath, txt, Encoding.UTF8);
                EditorUtility.DisplayProgressBar("Build Script  " + spName, "Don't do anything", 0.6f);

                AssetDatabase.Refresh();

                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                if (asset != null)
                {
                    EditorUtility.SetDirty(asset);
                }
                AssetDatabase.ImportAsset(filePath);
                AssetDatabase.SaveAssets();
            }
            private static string ReplaceDescription()
            {
                string res = string.IsNullOrEmpty(_scriptCreater.description) ? ProjectConfig.Description : _scriptCreater.description;
                if (!res.Contains("\n"))
                    return res;
                else
                {
                    string s = string.Empty;
                    var strs = res.Split('\n');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string str = strs[i];
                        if (i == 0)
                        {
                            s = s.Append(str);
                            if (strs.Length > 1)
                                s = s.Append("\n");
                        }
                        else
                        {
                            s = s.Append("                  " + str);
                            if (i < strs.Length - 1)
                                s = s.Append("\n");
                        }
                    }

                    return s;
                }

            }
            private static string ReplaceNamespace(string txt)
            {
                string res = string.Empty;
                List<string> NameSpaces = new List<string>();
                for (int i = 0; i < _scriptCreater.scriptMarks.Length; i++)
                {
                    var sm = _scriptCreater.scriptMarks[i];
                    var comp = sm.GetComponent(sm.fieldType);
                    NameSpaces.Add(comp.GetType().Namespace);
                }
                NameSpaces = NameSpaces.Distinct().ToList();
                NameSpaces.ForEach((ns) => {
                    string tmp= "using ".Append(ns);
                    if (!txt.Contains(tmp))
                        res = res.Append(tmp).Append(";\n");

                });

                if ((txt.Contains("[SerializeField]")|| txt.Contains("MonoBehaviour")) && !res.Contains("using UnityEngine;"))
                {
                    res = res.Append("using UnityEngine;\n");
                }
                return res;
            }
            private static string ReplaceField(string txt)
            {
                string result = string.Empty;
                _scriptCreater.scriptMarks.ForEach((sm) => {
                    if (!string.IsNullOrEmpty(sm.description))
                    {
                        if (sm.description.Contains("\n"))
                        {
                            sm.description.Split('\n').ToList().ForEach((str) =>
                            {
                                result = result.Append("\t\t//" + str + "\n");
                            });
                        }
                        else
                            result = result.Append("\t\t//" + sm.description + "\n");

                    }
                    var fieldTypeStrs = sm.fieldType.Split('.');
                    var fieldType = fieldTypeStrs.Last();
                    if (sm.isPublic)
                        result = result.Append("\t\tpublic " + fieldType + " " + sm.fieldName + ";\n");
                    else
                        result = result.Append("\t\t[SerializeField] private " + fieldType + " " + sm.fieldName + ";\n");

                });
                return result;
            }
        }
    }
}

