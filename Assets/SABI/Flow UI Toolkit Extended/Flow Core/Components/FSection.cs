using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FSection : Div
    {
        public FSection(string heading, VisualElement body)
        {
            var sectionGroup = new FContainer()
                .Margin(5)
                .BorderWidth(1)
                .BGColor(Color.black.WithA(0.2f))
                .BorderColor(new Color(0.75f, 0.75f, 0.75f))
                .Insert(
                    new FRow(
                        new Button(() => body.DisplayToggle()) { text = "" },
                        new FText(heading)
                    ).MarginBottom(10)
                );

            body.SetParent(sectionGroup);

            this.Insert(sectionGroup);
        }
    }
}
