/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine;
using IFramework.GUITool;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EditorWindowCacheAttribute : Attribute
    {
        public string searchName { get; private set; }
        public EditorWindowCacheAttribute() { }
        public EditorWindowCacheAttribute(string searchName)
        {
            this.searchName = searchName;
        }

    }
    class EditorWindowUtil
    {
        public class EditorWindowInfo
        {
            public string searchName;
            public Type type;
            public EditorWindowInfo(Type type, string SearchName = "")
            {
                this.type = type;
                if (string.IsNullOrEmpty(SearchName))
                    this.searchName = type.Name;
                else
                    this.searchName = SearchName;
            }
            public EditorWindow[] FindAll()
            {
                if (type == null)
                    return new EditorWindow[0];
                return (EditorWindow[])(Resources.FindObjectsOfTypeAll(type));
            }
            public EditorWindow Find()
            {
                foreach (EditorWindow window in FindAll())
                {
                    //window.Focus();
                    return window;
                }
                return null;
            }
            public EditorWindow FindOrCreate()
            {
                EditorWindow window = Find();
                if (window != null) return window;
                if (type == null) return null;
                //if (menuPath != null && menuPath.Length != 0)
                //    try
                //    {
                //        EditorApplication.ExecuteMenuItem(menuPath);
                //    }
                //    catch (Exception) { }
                window = Create();
                return window;
            }
            public EditorWindow Create()
            {
                EditorWindow window = EditorWindow.GetWindow(type);
                //window.Focus();
                return window;
            }
            public Rect position
            {
                get
                {
                    EditorWindow window = Find();
                    if (window == null)
                        return new Rect(0, 0, 0, 0);
                    return window.position;
                }
                set
                {
                    EditorWindow window = FindOrCreate();
                    if (window != null)
                        window.position = value;
                }
            }
            public bool isOpen
            {
                get
                {
                    return FindAll().Length != 0;
                }
                set
                {
                    if (value)
                        FindOrCreate();
                    else
                        foreach (EditorWindow window in FindAll())
                            window.Close();
                }
            }
        }


        [InitializeOnLoadMethod]
        private static void FreshInfoDic()
        {
            Windows.Clear();
            foreach (var item in typeof(EditorWindow).GetSubTypesInAssemblys())
            {
                if (!item.IsDefined(typeof(EditorWindowCacheAttribute), false)) continue;
                object[] attrs = item.GetCustomAttributes(false);
                for (int i = 0; i < attrs.Length; i++)
                {
                    if (attrs[i] is EditorWindowCacheAttribute)
                    {
                        EditorWindowCacheAttribute attr = attrs[i] as EditorWindowCacheAttribute;
                        Windows.Add(new EditorWindowInfo(item, string.IsNullOrEmpty(attr.searchName) ? item.FullName : attr.searchName));
                    }
                }
            }
            AddDefautEditorWindows();

        }
        private static void AddDefautEditorWindows()
        {
            System.Reflection.Assembly assembly = typeof(EditorWindow).Assembly;
            typeof(EditorWindow).GetSubTypesInAssemblys().ForEach((type) =>
            {
                if (type.Namespace != null && type.Namespace.Contains("UnityEditor") && !type.IsAbstract)
                {
                    Windows.Add(new EditorWindowInfo(type, type.Name));
                }

            });


        }

        public static List<EditorWindowInfo> Windows = new List<EditorWindowInfo>();

        public static bool Contains(string name)
        {
            return FindInfo(name) != null;
        }
        public static EditorWindowInfo FindInfo(string name)
        {
            EditorWindowInfo Info = Windows.Find((info) => { return info.searchName == name; });
            if (Info == null)
            {
                FreshInfoDic();
                Info = Windows.Find((info) => { return info.searchName == name; });
                if (Info == null) return null;
                return Info;
            }
            return Info;
        }
        public static EditorWindow Find(string name)
        {
            EditorWindowInfo info = FindInfo(name);
            return info == null ? null : info.Find();
        }
        public static EditorWindow[] FindAll(string name)
        {
            EditorWindowInfo info = FindInfo(name);
            return info == null ? null : info.FindAll();
        }
        public static EditorWindow FindOrCreate(string name)
        {
            EditorWindowInfo info = FindInfo(name);
            return info == null ? null : info.FindOrCreate();
        }
        public static EditorWindow Create(string name)
        {
            EditorWindowInfo info = FindInfo(name);
            return info == null ? null : info.Create();
        }
        public static bool IsActive(string name)
        {
            EditorWindowInfo info = FindInfo(name);
            return info == null ? false : info.isOpen;
        }
        public static void SetActive(string name, bool active)
        {
            EditorWindowInfo info = FindInfo(name);
            if (info != null)
            {
                info.isOpen = active;
            }
        }
        public static Rect Postion(string name)
        {
            EditorWindowInfo info = FindInfo(name);
            return info == null ? Rect.zero : info.position;
        }
        public static void SetPostion(string name, Rect position)
        {
            EditorWindowInfo info = FindInfo(name);
            if (info != null)
            {
                info.position = position;
            }
        }

    }
    class EditorWindowCollection : EditorWindow, IRectGUIDrawer, ILayoutGUIDrawer
    {
        [MenuItem("IFramework/EditorWindowCollection")]
        static void ShowWindow()
        {
            GetWindow<EditorWindowCollection>();

        }
        private void OnEnable()
        {
            sear = new SearchFieldDrawer() { value = "", };
            sear.onValueChange += (str) => { search = str; };
        }
        static string search = "";
        SearchFieldDrawer sear;
        TableViewCalculator table = new TableViewCalculator();
        private const string Name = "Name";
        private const string Find = "Find";
        private const string Create = "Create";
        private const string FindOrCreate = "FindOrCreate";
        private const string CloseBtn = "Colse";
        private const string SearchStr = "SearchStr";

        private const string TitleStyle = "IN BigTitle";
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";
        private const float LineHeight = 20f;
        private Vector2 scroll;

        private ListViewCalculator.ColumnSetting[] setting = new ListViewCalculator.ColumnSetting[]
        {
                new ListViewCalculator.ColumnSetting()
                {
                    name=Name,
                    width=400
                },
                new ListViewCalculator.ColumnSetting()
                {
                    name=Find,
                        width=60
                },
                new ListViewCalculator.ColumnSetting()
                {
                    name=Create,
                        width=60
                },
                new ListViewCalculator.ColumnSetting()
                {
                    name=FindOrCreate,
                        width=100,
                        offsetX=-8
                },
                new ListViewCalculator.ColumnSetting()
                {
                    name=CloseBtn,
                        width=50,
                },
                new ListViewCalculator.ColumnSetting()
                {
                    name=SearchStr,
                        width=50
                }
        };
        private void OnGUI()
        {
            var ws = EditorWindowUtil.Windows.FindAll((w) => { return w.searchName.ToLower().Contains(search); }).ToArray();
            table.Calc(new Rect(Vector2.zero, position.size), new Vector2(0, LineHeight), scroll, LineHeight, ws.Length, setting);

            this.LabelField(table.titleRow.position, "", new GUIStyle(TitleStyle))
                .LabelField(table.titleRow[Create].localPostion, Create)
                .LabelField(table.titleRow[Name].localPostion, Name)
                .LabelField(table.titleRow[Find].localPostion, Find)
                .LabelField(table.titleRow[FindOrCreate].localPostion, FindOrCreate)
                .LabelField(table.titleRow[CloseBtn].localPostion, CloseBtn)
                .Pan(() => {
                    sear.OnGUI(table.titleRow[SearchStr].localPostion);
                })
                .DrawScrollView(() =>
                {
                    for (int i = table.firstVisibleRow; i < table.lastVisibleRow + 1; i++)
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                            style.Draw(table.rows[i].position, false, false, false, false);
                        }
                        Texture tx = null;
                        if (EditorWindowUtil.Windows[i].type.Namespace.Contains("UnityEditor"))
                            tx = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
                        string windowName = ws[i].searchName;
                        this.Label(table.rows[i][Name].position, new GUIContent(windowName, tx))
                            .Button(() =>
                            {
                                var w = EditorWindowUtil.Find(windowName);
                                if (w != null)
                                {
                                    w.Focus();
                                }
                            }, table.rows[i][Find].position, Find)
                            .Button(() =>
                            {
                                var w = EditorWindowUtil.Create(windowName);
                                if (w != null)
                                {
                                    w.Focus();
                                }
                            }, table.rows[i][Create].position, Create)
                            .Button(() =>
                            {
                                var w = EditorWindowUtil.FindOrCreate(windowName);
                                if (w != null)
                                {
                                    w.Focus();
                                }
                            }, table.rows[i][FindOrCreate].position, FindOrCreate)
                            .Button(() =>
                            {
                                EditorWindowUtil.FindAll(windowName).ToList().ForEach((w) =>
                                {
                                    w.Close();
                                });
                            }, table.rows[i][CloseBtn].position, CloseBtn);
                    }
                }, table.view, ref scroll, table.content, false, false);


            Handles.color = Color.black;
            for (int i = 0; i < table.titleRow.columns.Count; i++)
            {
                var item = table.titleRow.columns[i];

                if (i != 0)
                    Handles.DrawAAPolyLine(1, new Vector3(item.position.x,
                                                            item.position.y,
                                                            0),
                                              new Vector3(item.position.x,
                                                            item.position.y + item.position.height - 2,
                                                            0));
            }
            table.position.DrawOutLine(2, Color.black);
            Handles.color = Color.white;
        }

    }
}
