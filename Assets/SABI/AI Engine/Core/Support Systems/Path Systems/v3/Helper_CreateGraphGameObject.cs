#if UNITY_EDITOR
namespace SABI
{
    using SABI;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class Helper_CreateGraphGameObject //: MonoBehaviour
    {
        [MenuItem("GameObject/SGraph")]
        static void CreateSGraph()
        {
            GameObject gameObject = new GameObject("SGraph");
            Undo.RegisterCreatedObjectUndo(gameObject, "Created SGraph game object");
            Undo.AddComponent<SGraph>(gameObject);
            Selection.activeGameObject = gameObject;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
