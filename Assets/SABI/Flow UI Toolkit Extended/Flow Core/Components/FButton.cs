using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FButton : Div
    {
        public FButton(string message, Action onClick, Action<VisualElement> visualUpdate = null)
        {
            Button btn = new Button() { text = message }
                .OnClick(onClick)
                .BorderRadius(15)
                .Height(30);
            visualUpdate?.Invoke(btn);
            this.Insert(btn);
        }
    }
}
