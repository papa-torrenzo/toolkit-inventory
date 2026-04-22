using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FButtonGroup : Div
    {
        public FButtonGroup(Dictionary<string, Action> values)
        {
            var buttonGroup = new Div();

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
                        if (index == 0)
                            e.BorderRadiusBottom(0);
                        else if (index == values.Count - 1)
                            e.BorderRadiusTop(0);
                        else
                            e.BorderRadius(0);
                    }
                ).SetParent(buttonGroup);
            }

            this.Insert(buttonGroup);
        }
    }
}
