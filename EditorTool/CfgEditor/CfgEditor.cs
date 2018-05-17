using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace CfgFileCreateEditor
{
    /// <summary>
    /// 配置表字符集类型
    /// </summary>
    public enum ECharSetType
    {
        UTF8,
        Default,
        ASCII
    }

    public class CfgEditor : EditorWindow
    {
        private string csvFileName;
        private ECharSetType charSetType = ECharSetType.UTF8;

        [MenuItem("Assets/CSV/Create CSVFile", false, 1)]
        static void CreateCSVTable()
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            if (path != null && !path.Equals(""))
            {
                Rect createCSVFileCreateWindowRect = new Rect(700, 500, 260, 100);
                CfgEditor cfgEditor = (CfgEditor)CfgEditor.GetWindowWithRect(typeof(CfgEditor), createCSVFileCreateWindowRect, true, "Create CSV File");
                cfgEditor.Show();
            }
            else
            {
                Debug.LogError("请在Project面板中选择文件夹或文件夹");
            }
        }

        [MenuItem("Assets/CSV/CSVToText/UTF8", false, 2)]
        static void CSVToTextUTF8()
        {
            CSVToTextMethod(Encoding.UTF8);
        }

        [MenuItem("Assets/CSV/CSVToText/Default", false, 2)]
        static void CSVToTextDefault()
        {
            CSVToTextMethod(Encoding.Default);
        }

        [MenuItem("Assets/CSV/CSVToText/ASCII", false, 2)]
        static void CSVToTextASCII()
        {
            CSVToTextMethod(Encoding.ASCII);
        }

        private static void CSVToTextMethod(Encoding encoding)
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            if (path != null && !path.Equals(""))
            {
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.FullName.IndexOf(".csv") > 1)
                    {
                        string[] content = File.ReadAllLines(fileInfo.FullName, Encoding.Default);
                        File.Delete(fileInfo.FullName);
                        File.WriteAllLines(fileInfo.FullName.Replace(".csv", ".txt"), content, encoding);
                        Debug.Log("文件后缀修改完成，请刷新Project面板");
                        return;
                    }
                    else
                    {
                        Debug.LogError("你选择的文件不是CSV文件");
                    }
                }
                if (Directory.Exists(path))
                {
                    DirectoryInfo dirctoryInfo = new DirectoryInfo(path);
                    foreach (FileInfo csvFileInfo in dirctoryInfo.GetFiles("*.csv", SearchOption.AllDirectories))
                    {
                        string[] content = File.ReadAllLines(csvFileInfo.FullName, Encoding.Default);
                        File.Delete(csvFileInfo.FullName);
                        File.WriteAllLines(csvFileInfo.FullName.Replace(".csv", ".txt"), content, encoding);
                    }
                    Debug.Log("文件后缀修改完成，请刷新Project面板");
                    return;
                }
            }
            else
            {
                Debug.LogError("请在Project面板中选择文件夹");
            }
        }

        [MenuItem("Assets/CSV/TextToCSV", false, 2)]
        static void TextToCSV()
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            if (path != null && !path.Equals(""))
            {
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.FullName.IndexOf(".txt") > 1)
                    {
                        File.Move(path, path.Replace(".txt", ".csv"));
                        Debug.Log("CSV转换TXT完成，请刷新Project面板");
                        return;
                    }
                    else
                    {
                        Debug.LogError("你选择的文件不是TXT文件");
                    }
                }
                if (Directory.Exists(path))
                {
                    DirectoryInfo dirctoryInfo = new DirectoryInfo(path);
                    foreach (FileInfo csvFileInfo in dirctoryInfo.GetFiles("*.txt", SearchOption.AllDirectories))
                    {
                        string allPath = csvFileInfo.FullName;
                        File.Move(allPath, allPath.Replace(".txt", ".csv"));
                    }
                    Debug.Log("TXT转换CSV完成，请刷新Project面板");
                    return;
                }
            }
            else
            {
                Debug.LogError("请在Project面板中选择文件夹");
            }
        }

        void OnGUI()
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            if (path != null && !path.Equals(""))
            {
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    path = fileInfo.DirectoryName;
                }
                GUILayout.Label("CSV表格文件名称");
                csvFileName = EditorGUILayout.TextField(csvFileName);
                charSetType = (ECharSetType)EditorGUILayout.EnumPopup("CSV表格文件字符集类型", charSetType);
                GUILayout.Label("");
                if (GUILayout.Button("创建CSV文件"))
                {
                    if (csvFileName == null || csvFileName.Equals(""))
                    {
                        Debug.LogError("新建CSV表格文件名称不能为空");
                    }
                    else
                    {
                        path = string.Format(@"{0}\{1}.csv", path, csvFileName);
                        if (File.Exists(path))
                        {
                            Debug.LogError(string.Format("配置表文件{0}已经存在", csvFileName));
                        }
                        else
                        {
                            string[] content = new string[1];
                            File.WriteAllLines(path, content, GetEncoding(charSetType));
                            Debug.Log(string.Format("{0}文件创建成功，请刷新Project面板", csvFileName));
                            this.Close();
                        }
                    }
                }
            }
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

        private void OnLostFocus()
        {
            this.Close();
        }
    }
}
