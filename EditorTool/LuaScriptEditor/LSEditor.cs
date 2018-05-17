using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LuaScriptEditor
{
    /// <summary>
    /// Lua脚本字符集类型
    /// </summary>
    public enum ECharSetType
    {
        UTF8,
        Default,
        ASCII
    }

    /// <summary>
    /// Lua脚本内容类型
    /// </summary>
    public enum EContentType
    {
        类,
        Table表
    }

    /// <summary>
    /// Lua脚本创建编辑器扩展类
    /// </summary>
    public class LSEditor : EditorWindow
    {
        private string luaFileName;
        private string luaScriptTip;
        private ECharSetType charSetType;
        private EContentType contentType;
        private bool isReturn;

        [MenuItem("Assets/Create/Lua Script", false, 82)]
        static void CreateLuaScript()
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            if (path != null && !path.Equals(""))
            {
                Rect createLuaScriptWindowRect = new Rect(700, 500, 260, 140);
                LSEditor luaScripteEditor = (LSEditor)LSEditor.GetWindowWithRect(typeof(LSEditor),
                    createLuaScriptWindowRect, true, "Create Lua Script");
                luaScripteEditor.Show();
            }
            else
            {
                Debug.Log("请在Project面板中选择文件夹");
            }
        }

        private void OnGUI()
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                path = fileInfo.DirectoryName;
            }
            GUILayout.Label("Lua文件名称");
            luaFileName = EditorGUILayout.TextField(luaFileName);
            charSetType = (ECharSetType)EditorGUILayout.EnumPopup("字符集类型", charSetType);
            contentType = (EContentType)EditorGUILayout.EnumPopup("Lua内容格式", contentType);
            isReturn = true;
            isReturn = EditorGUILayout.Toggle("是否有返回值", isReturn);
            GUILayout.Label("");
            if (GUILayout.Button("创建脚本"))
            {
                if (luaFileName == null || luaFileName.Equals(""))
                {
                    Debug.LogError("新建Lua脚本文件名称不允许为空");
                }
                else
                {
                    path = string.Format(@"{0}\{1}.lua", path, luaFileName);
                    if (File.Exists(path))
                    {
                        Debug.LogError(string.Format("{0}文件已经存在", luaFileName));
                    }
                    else
                    {
                        string[] content = new string[5];
                        switch (contentType)
                        {
                            case EContentType.类:
                                content[0] = string.Format("{0}=class(\"{1}\");", luaFileName, luaFileName);
                                break;
                            case EContentType.Table表:
                                content[0] = string.Format("{0}={{}};", luaFileName);
                                break;
                        }
                        if (isReturn)
                        {
                            content[4] = string.Format("return {0};", luaFileName);
                        }
                        File.WriteAllLines(path, content, GetEncoding(charSetType));
                        Debug.Log(string.Format("{0}.lua文件创建完成，请刷新下Project面板", luaFileName));
                        this.Close();
                    }
                }
            }
        }

        private void OnLostFocus()
        {
            this.Close();
        }

        private void PrintTip(string tip)
        {
            luaScriptTip = EditorGUILayout.TextField("!!Error:" + tip + "!!");
        }

        private Encoding GetEncoding(ECharSetType charSetType)
        {
            Encoding encoding = null;
            switch (charSetType)
            {
                case ECharSetType.ASCII:
                    encoding = Encoding.ASCII;
                    break;
                case ECharSetType.Default:
                    encoding = Encoding.Default;
                    break;
                case ECharSetType.UTF8:
                    encoding = Encoding.UTF8;
                    break;
            }
            return encoding;
        }
    }
}


