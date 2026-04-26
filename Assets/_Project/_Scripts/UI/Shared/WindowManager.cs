using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace toolkitinventory {
public class WindowManager : MonoBehaviour {

    [SerializeField] private UIDocument uidoc;
    [SerializeField] private VisualTreeAsset gameWindowTemplate;

    private VisualElement container;
    private Dictionary<string, GameWindow> windows = new();
    
    private void Awake() {
        var root = uidoc.rootVisualElement;
        
        container = new VisualElement();
        container.name = "window-container";
        container.style.position = Position.Absolute;
        container.style.width = Length.Percent(100);
        container.style.height = Length.Percent(100);
        container.pickingMode = PickingMode.Ignore;
        
        root.Add(container);
    }

    public GameWindow CreateWindow(string id, string title, Vector2 defaultPosition) {
        float x = PlayerPrefs.GetFloat($"window_{id}_x", defaultPosition.x);
        float y = PlayerPrefs.GetFloat($"window_{id}_y", defaultPosition.y);
        
        var window = new GameWindow(gameWindowTemplate, container, title);
        window.SetPosition(x, y);

        windows[id] = window;
        return window;
    }

    public void ToggleWindow(string id) {
        if (windows.TryGetValue(id, out var window)) {
            if (window.isVisible) {
                SaveWindowPosition(id, window);
                window.Hide();
            }
            else {
                window.Show();
            }
        }
    }

    public void CloseAllWindows() {
        foreach (var winks in windows) {
            if (winks.Value.isVisible) {
                SaveWindowPosition(winks.Key, winks.Value);
                winks.Value.Hide();
            }
        }
    }

    private void SaveWindowPosition(string id, GameWindow window) {
        Vector2 pos = window.GetPosition();
        
        PlayerPrefs.SetFloat($"window_{id}_x", pos.x);
        PlayerPrefs.SetFloat($"window_{id}_y", pos.y);
    }

    private void SaveAllPositions() {
        foreach (var winks in windows) {
            if (winks.Value.isVisible) SaveWindowPosition(winks.Key, winks.Value);
        }
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() {
        SaveAllPositions();
    }
}
}