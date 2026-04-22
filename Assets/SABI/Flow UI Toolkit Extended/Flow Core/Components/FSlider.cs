using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FSlider : Div
    {
        public FSlider(
            string label,
            float minValue,
            float maxValue,
            float defaultValue,
            Action<float> onValueChanged = null
        )
        {
            Slider slider = new Slider(label, minValue, maxValue) { value = defaultValue };

            if (onValueChanged != null)
            {
                slider.RegisterValueChangedCallback(e => onValueChanged?.Invoke(e.newValue));
            }

            this.Insert(slider);
        }
    }
}
