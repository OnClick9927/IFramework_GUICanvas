/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework
{
    [OnEnvironmentInit]
    public static  class ProjectConfig
    {
        public static string NameSpace { get { return info.NameSpace; } }
        public static string UserName { get { return info.UserName; } }
        public static string Version { get { return info.Version; } }
        public static string Description { get { return info.Description; } }
        public static int lev_L { get { return info.lev_L; } }
        public static int lev_W { get { return info.lev_W; } }
        public static int lev_E { get { return info.lev_E; } }
        public static bool enable { get { return info.enable; } }
        public static bool enable_L { get { return info.enable_L; } }
        public static bool enable_W { get { return info.enable_W; } }
        public static bool enable_E { get { return info.enable_E; } }
        public const string configName = "ProjectConfig";

        private static ProjectConfigInfo __info;
        private static ProjectConfigInfo info
        {
            get
            {
                if (__info == null) LoadProjectInfo();
                return __info;
            }
        }
        private static void LoadProjectInfo()
        {
            __info = UnityEngine.Resources.Load<ProjectConfigInfo>(configName);
            if (__info==null)
            {
                Log.E("Open Project Config Window in WindowCollection");
            }
        }



        static ProjectConfig()
        {
            Log.loger = new UnityLoger();
            Log.enable_L = ProjectConfig.enable_L;
            Log.enable_W = ProjectConfig.enable_W;
            Log.enable_E = ProjectConfig.enable_E;
            Log.lev_L = ProjectConfig.lev_L;
            Log.lev_W = ProjectConfig.lev_W;
            Log.enable = ProjectConfig.enable;
            Log.lev_E = ProjectConfig.lev_E;
        }
    }
}
