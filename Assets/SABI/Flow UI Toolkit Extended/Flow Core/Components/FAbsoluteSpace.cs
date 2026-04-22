
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FAbsoluteSpace : Div
    {
        public FAbsoluteSpace(VisualElement child = null, float size = 50)
        {
            this.AbsolutePosition();
        }
    }
}
