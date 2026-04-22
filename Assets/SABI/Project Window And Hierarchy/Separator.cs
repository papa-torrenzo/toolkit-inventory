#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SABI
{
    public static class Separator
    {
        [MenuItem("GameObject/──────────────── ", false, 0)]
        private static void CreateSeparator()
        {
            GameObject separator = new GameObject("──────────────────────────────────────── ");
        }
    }
}
#endif
