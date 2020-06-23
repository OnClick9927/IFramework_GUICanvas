/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.IO;
using UnityEngine;
using IFramework.GUITool;

namespace IFramework
{
    [EditorWindowCache]
    partial class ProjectConfigWindow
    {

        public static void ShowWindow()
        {
            GetWindow<ProjectConfigWindow>(false, "ProjectConfig", true);
        }
    }
    partial class ProjectConfigWindow : EditorWindow, ILayoutGUIDrawer
    {
        private ProjectConfigInfo Info;
        private void OnEnable()
        {
            string ProjectConfigInfoPath = EditorEnv.corePath.CombinePath("ProjectConfig/Resources/"+ ProjectConfig.configName + ".asset").ToRegularPath();

            if (File.Exists(ProjectConfigInfoPath))
                Info = ScriptableObj.Load<ProjectConfigInfo>(ProjectConfigInfoPath);
            else
            {
                Info = ScriptableObj.Create<ProjectConfigInfo>(ProjectConfigInfoPath);

                Info.enable_L = Log.enable_L;
                Info.enable_W = Log.enable_W;
                Info.enable_E = Log.enable_E;
                Info.lev_L = Log.lev_L;
                Info.lev_W = Log.lev_W;
                Info.enable = Log.enable;
                Info.lev_E = Log.lev_E;
            }
        }
        private void OnDisable()
        {
            ScriptableObj.Update(Info);
        }

        private void OnGUI()
        {


            this.Space()
                    .ETextField(new GUIContent("UserName","Project Author's Name"), ref Info.UserName)
                    .ETextField(new GUIContent("Version","Version of Project"), ref Info.Version)
                    .LabelField(new GUIContent("NameSpace","Script's Namespace"))
                    .TextArea(ref Info.NameSpace)
                    .Label("Description of Scripts")
                    .TextArea(ref Info.Description, GUILayout.Height(100))
                    .Space(10);
            //.FlexibleSpace();
          
            this.Label("LogSetting",new GUIStyle("IN Title"))
                .IntField("Log Lev", ref Info.lev_L)
                .IntField("Warning Lev", ref Info.lev_W)
                .IntField("Error Lev", ref Info.lev_E)
                .Toggle("Enable", ref Info.enable)
                .Pan(() =>
                {
                    GUI.enabled = Info.enable;
                    this.Toggle("Log Enable", ref Info.enable_L)
                        .Toggle("Warning Enable", ref Info.enable_W)
                        .Toggle("Error Enable", ref Info.enable_E);

                    GUI.enabled = true;
                }).FlexibleSpace();


            this.BeginHorizontal()
                        .FlexibleSpace()
                        .Button(() => { ScriptableObj.Update(Info); }, "Save")
                    .EndHorizontal();
        }


    }
}
