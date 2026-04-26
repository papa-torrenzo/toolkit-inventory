using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using SABI;

namespace toolkitinventory {
public class WindowDragManipulator : PointerManipulator {

    private Vector2 startposp;
    private Vector2 startposw;
    private bool dragging;
    private VisualElement winroot;

    private Vector2 panelsize;
    private float winwidth;
    private readonly float PANELBUFFERSIZE = 40f;
    
    private Vector2 panelbuffer => panelsize.WithSubtract(PANELBUFFERSIZE);

    private float windowbuffer => winwidth - PANELBUFFERSIZE;
    
    public WindowDragManipulator(VisualElement dragHandle, VisualElement windowRoot) {
        target = dragHandle;
        winroot = windowRoot;
    }

    private void OnPointerDown(PointerDownEvent evt) {

        if (evt.button != 0) return;
        
        startposp = evt.position;
        startposw = new Vector2(winroot.resolvedStyle.left, winroot.resolvedStyle.top);
        
        panelsize = winroot.panel.visualTree.worldBound.size;
        winwidth = winroot.resolvedStyle.width;
        
        dragging = true;
        winroot.BringToFront();
        target.CapturePointer(evt.pointerId);
        evt.StopPropagation();
    }
    
    private void OnPointerMove(PointerMoveEvent evt) {
        if (!dragging) return;
        
        Vector2 delta = (Vector2)evt.position - startposp;
        Vector2 destination = startposw + delta;
        
//        destination.x = Mathf.Clamp(destination.x, -windowbuffer, panelbuffer.x);
//        destination.y = Mathf.Clamp(destination.y, 0f, panelbuffer.y);
        
        destination.x = Mathf.Clamp(destination.x, -(winwidth - 40), panelsize.x - 40);
        destination.y = Mathf.Clamp(destination.y, 0, panelsize.y - 40);
        
        winroot.style.left = destination.x;
        winroot.style.top = destination.y;
    }

    private void OnPointerUp(PointerUpEvent evt) {
        if (!dragging) return;
        
        dragging = false;
        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }
}
}