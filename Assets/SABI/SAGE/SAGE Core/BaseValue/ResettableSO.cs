namespace SABI
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ResettableSO : ScriptableObject
    {
        static List<ResettableSO> allInstances = new List<ResettableSO>();

        public static void ResetAllInstances()
        {
            foreach (var item in allInstances)
                item.ResetSO();
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
#endif
            allInstances.Add(this);
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
#endif
            allInstances.Remove(this);
        }

#if UNITY_EDITOR
        private void HandlePlayModeStateChanged(PlayModeStateChange change) => ResetSO();
#endif

        protected virtual void ResetSO() { }
    }
}
