namespace SABI
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;

    public static class ScriptGenerator_Context_Hierarchy
    {
        [UnityEditor.MenuItem("Assets/ScriptsTemplets/Context_Hierarchy")]
        static void CreateScript()
        {
            string pathToNewFile = EditorUtility.SaveFilePanel(
                "Context_Hierarchy",
                ScriptTemplet.GetCurrentPath(),
                "Context_Hierarchy.cs",
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

public static class #Context_Hierarchy#
{
    [UnityEditor.MenuItem(""GameObject/#Context_Hierarchy#"")]
    static void Generate#Context_Hierarchy#()
    {
        GameObject newGameObject = new GameObject(""#Context_Hierarchy#"");
        Undo.RegisterCreatedObjectUndo(newGameObject, ""Create #Context_Hierarchy#"");
        Debug.Log($""#Context_Hierarchy#"");
        EditorApplication.RepaintHierarchyWindow();
    }
}"
                    + "#endif";
                text = text.Replace("#Context_Hierarchy#", nameOfScript);
                File.WriteAllText(pathToNewFile, text);
                AssetDatabase.Refresh();
            }
        }
    }
#endif
}
