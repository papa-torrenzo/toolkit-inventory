using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FPopup : Div
    {
        public FPopup(VisualElement child = null, float size = 50)
        {
            Div bg = new FAbsoluteSpace().BGColor(Color.black.WithA(0.2f)).Expand();
            Div popup = new Div()
                // .Size(200, 100)
                .MaxWidth(400)
                .BGColor(Color.black)
                .BorderRadius()
                .Insert(
                    new FColumn(
                        new FText("Delete ?"),
                        new FText(
                            "Are you sure do you want to delete the file cat.jpeg ?"
                        ).TextWrap(),
                        new FRow(new FButton("Yes", null), new FButton("No", null))
                            .AlignSelf(Align.Center)
                            .BGColor()
                    )
                );

            bg.Insert(popup);
            this.Insert(bg);
        }
    }
}
