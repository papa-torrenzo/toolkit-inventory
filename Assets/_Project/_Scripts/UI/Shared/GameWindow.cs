using UnityEngine;
using UnityEngine.UIElements;

namespace toolkitinventory {
public class GameWindow {

    private VisualElement winroot;
    private Label windowTitle;
    private VisualElement contentArea;
    private WindowDragManipulator dragManipulator;

    public VisualElement Winroot => winroot;
    public Label WindowTitle => windowTitle;
    public VisualElement ContentArea => contentArea;
    public WindowDragManipulator DragManipulator => dragManipulator;

    
    
    public bool isVisible => winroot.resolvedStyle.display == DisplayStyle.Flex;
    
    public GameWindow(VisualTreeAsset template, VisualElement parent, string title) {
        var templateContainer = template.Instantiate();
        winroot = templateContainer.Q<VisualElement>("game-window");

        for (int i = 0; i < templateContainer.styleSheets.count; i++) {
            winroot.styleSheets.Add(templateContainer.styleSheets[i]);
        }
        
        windowTitle = winroot.Q<Label>("title-label");
        contentArea = winroot.Q<VisualElement>("content-area");
        
        windowTitle.text = title;
        
        var titlebar = winroot.Q<VisualElement>("title-bar");
        dragManipulator = new WindowDragManipulator(titlebar, winroot);
        titlebar.AddManipulator(dragManipulator);
        
        winroot.RegisterCallback<PointerDownEvent>(evt => winroot.BringToFront());
        
        parent.Add(winroot);
    }



    public Vector2 GetPosition() {
        return new Vector2(winroot.resolvedStyle.left, winroot.resolvedStyle.top);
    }

    public void SetPosition(float x, float y) {
        winroot.style.left = x;
        winroot.style.top = y;
    }
    
    public void Show() {
        winroot.style.display = DisplayStyle.Flex;
        winroot.BringToFront();
    }

    public void Hide() => WindowTitle.style.display = DisplayStyle.None;
}
}