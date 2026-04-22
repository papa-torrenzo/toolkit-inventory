using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FDropDownList : Div
    {
        public FDropDownList(string label, Dictionary<string, Action<string>> onValueChanged)
        {
            PopupField<string> popupField = new(label, onValueChanged.Keys.ToList(), 0, null, null);
            popupField.RegisterValueChangedCallback(e =>
            {
                onValueChanged[e.newValue]?.Invoke(e.newValue);
            });
            this.Insert(popupField);
        }
    }
}
