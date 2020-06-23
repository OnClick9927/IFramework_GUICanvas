/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.Modules.Coroutine;
using System;

namespace IFramework
{
    class EditorEnv
    {
        private const string _relativeCorePath = "0.1Core";
        private const string _relativeCoreEditorPath = "0.1Core/Editor";
        private const string _relativeUtilPath = "UTil";
        private const string _relativeEditorPath = "UTil/Editor";
        private static string _fpath;
        public const EnvironmentType envType = EnvironmentType.Ev0;
        public static FrameworkEnvironment env { get { return Framework.env0; } }
        public static FrameworkModules moudules { get { return env.modules; } }



        public static string frameworkName { get { return Framework.FrameworkName; } }
        public static string author { get { return Framework.Author; } }
        public static string version { get { return Framework.Version; } }
        public static string description { get { return Framework.Description; } }
        public static string frameworkPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fpath))
                {
                    string[] assetPaths = AssetDatabase.GetAllAssetPaths();
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        if (assetPaths[i].Contains(_relativeCorePath))
                        {
                            string tempPath = assetPaths[i];
                            int index = tempPath.IndexOf(_relativeCorePath);
                            _fpath = tempPath.Substring(0, index);
                            break;
                        }
                    }
                }
                return _fpath;
            }
        }
        public static string editorPath { get { return frameworkPath.CombinePath(_relativeEditorPath); } }
        public static string utilPath { get { return frameworkPath.CombinePath(_relativeUtilPath); } }
        public static string corePath { get { return frameworkPath.CombinePath(_relativeCorePath); } }
        public static string coreEditorPath { get { return frameworkPath.CombinePath(_relativeCoreEditorPath); } }





        public static event EditorApplication.CallbackFunction update { add { EditorApplication.update += value; } remove { EditorApplication.update -= value; } }
        public static event EditorApplication.CallbackFunction delayCall { add { EditorApplication.delayCall += value; } remove { EditorApplication.delayCall -= value; } }
        public static event EditorApplication.CallbackFunction searchChanged { add { EditorApplication.searchChanged += value; } remove { EditorApplication.searchChanged -= value; } }
        public static event EditorApplication.CallbackFunction modifierKeysChanged { add { EditorApplication.modifierKeysChanged += value; } remove { EditorApplication.modifierKeysChanged -= value; } }
        public static event EditorApplication.SerializedPropertyCallbackFunction contextualPropertyMenu { add { EditorApplication.contextualPropertyMenu += value; } remove { EditorApplication.contextualPropertyMenu -= value; } }

        public static event Func<bool> wantsToQuit { add { EditorApplication.wantsToQuit += value; } remove { EditorApplication.wantsToQuit -= value; } }
        public static event Action quitting { add { EditorApplication.quitting += value; } remove { EditorApplication.quitting -= value; } }

        public static event Action<PlayModeStateChange> playModeStateChanged { add { EditorApplication.playModeStateChanged += value; } remove { EditorApplication.playModeStateChanged -= value; } }
        public static event Action<PauseState> pauseStateChanged { add { EditorApplication.pauseStateChanged += value; } remove { EditorApplication.pauseStateChanged -= value; } }


        public static event EditorApplication.ProjectWindowItemCallback projectWindowItemOnGUI { add { EditorApplication.projectWindowItemOnGUI += value; } remove { EditorApplication.projectWindowItemOnGUI -= value; } }
        public static event EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI { add { EditorApplication.hierarchyWindowItemOnGUI += value; } remove { EditorApplication.hierarchyWindowItemOnGUI -= value; } }
        public static event Action projectChanged { add { EditorApplication.projectChanged += value; } remove { EditorApplication.projectChanged -= value; } }
        public static event Action hierarchyChanged { add { EditorApplication.hierarchyChanged += value; } remove { EditorApplication.hierarchyChanged -= value; } }


        [InitializeOnLoadMethod]
        static void EditorEnvInit()
        {
            UnityEngine.Debug.Log("FrameworkPath   right?   " + frameworkPath);
            Framework.InitEnv("IFramework_Editor", envType).InitWithAttribute();

            quitting += Framework.env0.Dispose;
            update += Framework.env0.Update;
            Framework.env0.modules.Coroutine = Framework.env0.modules.CreateModule<CoroutineModule>();

#if UNITY_2018_1_OR_NEWER
            PlayerSettings.allowUnsafeCode = true;
#else
          string  path = Application.dataPath.CombinePath("mcs.rsp");
            string content = "-unsafe";
            if (File.Exists(path) && path.ReadText(System.Text.Encoding.Default) == content) return;
                path.WriteText(content, System.Text.Encoding.Default); 
            AssetDatabase.Refresh();
            EditorUtil.ReOpen2();
#endif
        }
    }

}
