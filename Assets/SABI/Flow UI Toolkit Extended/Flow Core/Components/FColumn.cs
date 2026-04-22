using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FColumn : Div
    {
        public FColumn(
            List<VisualElement> elements = null,
            float spaceBetween = 0,
            float spaceAround = 0,
            bool fillSize = false
        )
        {
            if (elements == null || elements.Count == 0)
                return;
            this.Column();
            if (fillSize)
                this.Expand();
            Add(new Div().MinHeight(spaceAround));
            for (int i = 0; i < elements.Count; i++)
            {
                Add(elements[i]);
                if (i < elements.Count - 1)
                    Add(fillSize ? new FSpacer() : new Div().MinHeight(spaceBetween));
            }
            Add(new Div().MinHeight(spaceAround));
        }

        // ---------------------------------------------------------------------------------------------
        public FColumn(params VisualElement[] elements)
            : this(elements.ToList()) { }

        public FColumn(float spaceBetween, params VisualElement[] elements)
            : this(elements.ToList(), spaceBetween: spaceBetween) { }

        public FColumn(float spaceBetween, float spaceAround, params VisualElement[] elements)
            : this(elements.ToList(), spaceBetween: spaceBetween, spaceAround: spaceAround) { }

        public FColumn(bool fillSize, params VisualElement[] elements)
            : this(elements.ToList(), fillSize: fillSize) { }

        public FColumn(float spaceAround, bool fillSize, params VisualElement[] elements)
            : this(elements.ToList(), spaceAround: spaceAround, fillSize: fillSize) { }
    }
}
