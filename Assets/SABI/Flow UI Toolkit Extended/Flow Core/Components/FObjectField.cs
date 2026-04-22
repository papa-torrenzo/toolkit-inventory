#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FObjectField<T> : Div
        where T : UnityEngine.Object
    {
        public FObjectField(
            string label,
            T defaultValue = null,
            bool allowSceneObjects = true,
            Action<T> onValueChanged = null
        )
        {
            ObjectField objectField = new ObjectField(label)
            {
                objectType = typeof(T),
                value = defaultValue,
                allowSceneObjects = allowSceneObjects,
            };

            if (onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(e =>
                {
                    onValueChanged?.Invoke(e.newValue as T);
                });
            }

            this.Insert(objectField);
        }
    }
}

#endif