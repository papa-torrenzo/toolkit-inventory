using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FToggle : Div
    {
        public FToggle(string label, bool defaultValue, Action<bool> onValueChanged = null)
        {
            Toggle toggle = new Toggle(label) { value = defaultValue };

            if (onValueChanged != null)
            {
                toggle.RegisterValueChangedCallback(e => onValueChanged?.Invoke(e.newValue));
            }

            this.Insert(toggle);
        }
    }
}
