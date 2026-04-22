using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FExpanded : Div
    {
        public FExpanded(VisualElement child = null)
        {
            this.Expand();
            if (child != null)
                this.Add(child);
        }
    }
}
