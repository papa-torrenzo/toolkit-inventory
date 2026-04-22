namespace SABI
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;

    public static class ScriptGenerator_Context_ProjectWindow
    {
        [MenuItem("Assets/ScriptsTemplets/Context_ProjectWindow")]
        static void CreateScript()
        {
            string pathToNewFile = EditorUtility.SaveFilePanel(
                "Context_ProjectWindow",
                ScriptTemplet.GetCurrentPath(),
                "Context_ProjectWindow.cs",
                "cs"
            );

            if (!string.IsNullOrEmpty(pathToNewFile))
            {
                FileInfo fileInfo = new FileInfo(pathToNewFile);
                string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

                string text =
                    "#if UNITY_EDITOR \n"
                    + @"


using SABI;
using UnityEditor;
using UnityEngine;

public static class #Context_ProjectWindow#
{
    [UnityEditor.MenuItem(""Assets/#Context_ProjectWindow#"")]
    static void Generate#Context_ProjectWindow#()
    {
        string currentPath = ScriptTemplet.GetCurrentPath();
        Debug.Log($""#Context_ProjectWindow# on {currentPath} "");
        AssetDatabase.Refresh();
    }
}
"
                    + "#endif";
                text = text.Replace("#Context_ProjectWindow#", nameOfScript);
                File.WriteAllText(pathToNewFile, text);
                AssetDatabase.Refresh();
            }
        }
    }
#endif
}
