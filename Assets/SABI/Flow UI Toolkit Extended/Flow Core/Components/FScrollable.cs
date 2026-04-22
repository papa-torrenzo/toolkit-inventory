using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.Flow
{
    public class FScrollable : Div
    {
        ScrollView scrollView;

        public VisualElement GetContentContainer() => scrollView.contentContainer;

        public FScrollable(
            List<VisualElement> elements = null,
            float spaceBetween = 0,
            float spaceAround = 0,
            bool showScrollBar = true,
            ScrollViewMode scrollViewMode = ScrollViewMode.Vertical
        )
        {
            // if (elements == null || elements.Count == 0)
            //     return;
            float minHeightAround = scrollViewMode == ScrollViewMode.Vertical ? spaceAround : 0;
            float minWidthAround = scrollViewMode == ScrollViewMode.Vertical ? 0 : spaceAround;
            float minHeightBetween = scrollViewMode == ScrollViewMode.Vertical ? spaceBetween : 0;
            float minWidthBetween = scrollViewMode == ScrollViewMode.Vertical ? 0 : spaceBetween;

            scrollView = new ScrollView(scrollViewMode);

            Debug.Log($"[SAB] ScrollView Created");

            scrollView.contentContainer.Add(
                new VisualElement().MinWidth(minWidthAround).MinHeight(minHeightAround)
            );

            if (elements != null && elements.Count > 0)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    scrollView.contentContainer.Add(elements[i]);
                    if (i < elements.Count - 1)
                        scrollView.Add(
                            new VisualElement()
                                .MinWidth(minWidthBetween)
                                .MinHeight(minHeightBetween)
                        );
                }
            }
            if (!showScrollBar)
            {
                scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            }
            scrollView.contentContainer.Add(
                new VisualElement().MinWidth(minWidthAround).MinHeight(minHeightAround)
            );
            this.Add(scrollView);
        }

        // ---------------------------------------------------------------------------------------------
        public FScrollable(params VisualElement[] elements)
            : this(elements.ToList()) { }

        public FScrollable(float spaceBetween, params VisualElement[] elements)
            : this(elements.ToList(), spaceBetween: spaceBetween) { }

        public FScrollable(float spaceBetween, float spaceAround, params VisualElement[] elements)
            : this(elements.ToList(), spaceBetween: spaceBetween, spaceAround: spaceAround) { }

        // ---------------------------------------------------------------------------------------------
        public FScrollable Insert(VisualElement element)
        {
            Debug.Log($"[SAB] scrollView Insert {scrollView != null}");
            scrollView.contentContainer.Add(element);
            return this;
        }

        public FScrollable Insert(List<VisualElement> elements)
        {
            elements.ForEach(element => scrollView.contentContainer.Add(element));
            return this;
        }

        public FScrollable ClearAll()
        {
            scrollView.contentContainer.Clear();
            return this;
        }
    }
}
