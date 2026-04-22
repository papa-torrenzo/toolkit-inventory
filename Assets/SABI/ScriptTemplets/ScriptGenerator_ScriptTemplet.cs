namespace SABI
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;

    public static class ScriptGenerator_ScriptTemplet
    {
        [MenuItem("Assets/ScriptsTemplets/New Templet", false, 0)]
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
                // ---------------------------------------------------------------------------------------------
                string text =
                    @"
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    using SABI;

    public static class ScriptGenerator_#TempletName#
    {
        [MenuItem(""Assets/ScriptsTemplets/#TempletName#"")]
        static void CreateScript()
        {
            string pathToNewFile = EditorUtility.SaveFilePanel(
                ""#TempletName#"",
                ScriptTemplet.GetCurrentPath(),
                ""#TempletName#Instance.cs"",
                ""cs""
            );

            if (!string.IsNullOrEmpty(pathToNewFile))
            {
                FileInfo fileInfo = new FileInfo(pathToNewFile);
                string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

                string text =
                    @"" 
                    
public class #TempletName#Class
{

}

                    "";
                text = text.Replace(""#TempletName#"", nameOfScript);
                File.WriteAllText(pathToNewFile, text);
                AssetDatabase.Refresh();
            }
        }
    }
#endif
                    ";
                // ---------------------------------------------------------------------------------------------
                text = text.Replace("#TempletName#", nameOfScript);
                File.WriteAllText(pathToNewFile, text);
                AssetDatabase.Refresh();
            }
        }
    }
#endif
}
