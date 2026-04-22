using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FTextField : Div
    {
        public FTextField(string label, string defaultValue, Action<string> onValueChanged = null)
        {
            TextField textField = new TextField(label) { value = defaultValue };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(e => onValueChanged?.Invoke(e.newValue));
            }

            this.Insert(textField);
        }
    }
}
