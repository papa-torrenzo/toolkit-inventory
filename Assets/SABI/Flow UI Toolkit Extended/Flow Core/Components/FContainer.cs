using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FContainer : Div
    {
        public FContainer(VisualElement child = null)
        {
            this.Container();
            if (child != null)
                this.Add(child);
        }
    }
}
