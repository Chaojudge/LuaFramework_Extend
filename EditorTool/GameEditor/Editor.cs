using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameEditor
{
    /// <summary>
    /// Unity编辑器
    /// </summary>
    public class Editor
    {
        /// <summary>
        /// 启动调试模式
        /// </summary>
        [MenuItem("Tool/GameDebug/true")]
        static void SetGameDebugTrue()
        {
            PlayerPrefs.SetInt("GameDebug", 1);
            GameDebug.Log.isDebug = true;
            GameDebug.Log.Print("启动调试模式");
        }

        /// <summary>
        /// 关闭调试模式
        /// </summary>
        [MenuItem("Tool/GameDebug/false")]
        static void SetGameDebugFalse()
        {
            PlayerPrefs.SetInt("GameDebug", 0);
            GameDebug.Log.isDebug = false;
            UnityEngine.Debug.Log("提示：关闭调试模式");
        }

        /// <summary>
        /// 清除所有缓存的键值对
        /// </summary>
        [MenuItem("Tool/ClearAllPlayerPrefs")]
        static void ClearAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// 将所有.png转换成prefab
        /// </summary>
        [MenuItem("Assets/Tool/UIResourcesMaker")]
        static void UIResourcesMake()
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            Debug.Log(path.Replace("Assets/", ""));
            Debug.Log(Application.dataPath.Replace("/", @"\"));
            if (path != null && !path.Equals(""))
            {
                if (Directory.Exists(path))
                {
                    string spriteDir = Application.dataPath + "/Resources/Sprite";
                    if (!Directory.Exists(spriteDir))
                    {
                        Directory.CreateDirectory(spriteDir);
                    }
                    DirectoryInfo rootDirInfo = new DirectoryInfo(path);
                    foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
                    {
                        foreach (FileInfo pngInfo in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
                        {
                            string allPath = pngInfo.FullName;
                            string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                            GameObject go = new GameObject(sprite.name);
                            go.AddComponent<SpriteRenderer>().sprite = sprite;
                            allPath = spriteDir + "/" + dirInfo.Name + "/" + sprite.name + ".prefab";
                            if (!Directory.Exists(spriteDir + "/" + dirInfo.Name))
                            {
                                Directory.CreateDirectory(spriteDir + "/" + dirInfo.Name);
                            }
                            string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
                            PrefabUtility.CreatePrefab(prefabPath, go);
                            GameObject.DestroyImmediate(go, true);
                        }
                    }
                    Debug.Log("生成完成");
                }
                else
                {
                    Debug.Log("请存放有.png图片选择文件夹");
                    return;
                }
            }
            else
            {
                Debug.Log("请在Project面板中选择存放有.png图片选择文件夹");
            }
        }
    }
}
