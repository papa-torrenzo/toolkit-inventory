using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FRow : Div
    {
        public FRow(
            List<VisualElement> elements = null,
            float spaceBetween = 0,
            float spaceAround = 0,
            bool fillSize = false
        )
        {
            if (elements == null || elements.Count == 0)
                return;
            this.Row();
            if (fillSize)
                this.Expand();
            Add(new Div().MinWidth(spaceAround));
            for (int i = 0; i < elements.Count; i++)
            {
                Add(elements[i]);
                if (i < elements.Count - 1)
                    Add(fillSize ? new FSpacer() : new Div().MinWidth(spaceBetween));
            }
            Add(new Div().MinWidth(spaceAround));
        }

        // ---------------------------------------------------------------------------------------------
        public FRow(params VisualElement[] elements)
            : this(elements.ToList()) { }

        public FRow(float spaceBetween, params VisualElement[] elements)
            : this(elements.ToList(), spaceBetween: spaceBetween) { }

        public FRow(float spaceBetween, float spaceAround, params VisualElement[] elements)
            : this(elements.ToList(), spaceBetween: spaceBetween, spaceAround: spaceAround) { }

        public FRow(bool fillSize, params VisualElement[] elements)
            : this(elements.ToList(), fillSize: fillSize) { }

        public FRow(float spaceAround, bool fillSize, params VisualElement[] elements)
            : this(elements.ToList(), spaceAround: spaceAround, fillSize: fillSize) { }
    }
}
