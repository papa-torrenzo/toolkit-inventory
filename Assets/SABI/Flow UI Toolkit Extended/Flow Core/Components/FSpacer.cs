using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FSpacer : Div
    {
        public FSpacer(VisualElement child = null)
        {
            this.FlexGrow().Size(0);
            if (child != null)
                this.Add(child);
        }
    }
}
