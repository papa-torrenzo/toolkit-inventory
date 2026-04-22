using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FHorizontalButtonGroup : Div
    {
        public FHorizontalButtonGroup(Dictionary<string, Action> values)
        {
            var buttonGroup = new Div().Row();

            foreach (var item in values)
            {
                int index = values.ToList().IndexOf(item);
                new FButton(
                    item.Key,
                    item.Value,
                    (e) =>
                    {
                        if (values.Count < 2)
                            return;
                        float margineToSet = 1;
                        if (index == 0)
                            e.BorderRadiusRight(0).Expand().MarginRight(margineToSet);
                        else if (index == values.Count - 1)
                            e.BorderRadiusLeft(0).Expand().MarginLeft(margineToSet);
                        else
                            e.BorderRadius(0).Expand().MarginLeftRight(margineToSet);
                    }
                ).SetParent(buttonGroup);
            }

            this.Insert(buttonGroup);
        }
    }
}
