using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FProgressBar : Div
    {
        public FProgressBar(string title, float currentValue, float maxValue)
        {
            ProgressBar progressBar = new ProgressBar()
            {
                title = title,
                value = currentValue,
                highValue = maxValue,
            };
            this.Insert(progressBar);
        }
    }
}
