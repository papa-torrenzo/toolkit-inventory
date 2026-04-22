using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FSideView : Div
    {
        public FSideView(
            string label,
            Dictionary<string, Func<string, Div>> data,
            float leftPanelWidth = 200,
            float topBarHeight = 50
        )
        {
            Color colorA = new(0, 0, 0, 1);
            Color colorB = new(0.2f, 0.2f, 0.2f, 1);
            this.Expand().BGColor(colorA);
            Div topView = new Div()
                .FixedHeight(topBarHeight)
                .ExpandedWidth()
                .BGColor(colorB)
                .BorderRadiusRight()
                .MarginTop()
                .MarginRight();

            FScrollable leftView = new FScrollable(showScrollBar: false)
                .FixedWidth(leftPanelWidth)
                .ExpandedHeight()
                .BGColor(colorB)
                .Margin()
                .MarginLeft(0)
                .Padding()
                .BorderRadiusRight();

            Div inside = new();

            Div bodyView = new Div(inside)
                .ExpandedHeight()
                .ExpandedWidth()
                .MarginTopBottom()
                .MarginRight()
                // .BGColor(colorB)
                .BorderRadius();

            Button GetButton(string title, Func<string, Div> onClick)
            {
                return new Button(() =>
                {
                    bodyView.Clear();
                    inside = onClick?.Invoke(title);
                    bodyView.Add(inside);
                })
                {
                    text = title,
                }
                    .Height(50)
                    .BorderRadius()
                    .MarginBottom();
            }

            this.Insert(new FColumn(topView, new FRow(leftView, bodyView).Expand()).Expand());

            Dictionary<string, Action> bValues = new Dictionary<string, Action>();
            // data.Keys.ToList().ForEach(item => bValues.Add(item, data[item]));

            // FlowPreBuild.GenerateButtonGroup(  )

            data.Keys.ToList()
                .ForEach(item =>
                {
                    leftView.Insert(GetButton(item, data[item]));
                });
        }
    }
}
