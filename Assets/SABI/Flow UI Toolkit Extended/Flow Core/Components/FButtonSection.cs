using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FButtonSection : Div
    {
        public FButtonSection(string heading, Dictionary<string, Action> values)
        {
            var buttonGroup = new FContainer()
                .Margin(5)
                .BorderWidth(1)
                .BGColor(Color.black.WithA(0.2f))
                .BorderColor(new Color(0.75f, 0.75f, 0.75f))
                .Insert(new FText(heading).MarginBottom(10));

            new FButtonGroup(values).SetParent(buttonGroup);

            this.Insert(buttonGroup);
        }
    }
}
