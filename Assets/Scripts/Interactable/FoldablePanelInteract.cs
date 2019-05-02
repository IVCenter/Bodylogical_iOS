using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldablePanelInteract : ButtonInteract {
    public GameObject canvas;

    private bool on = false;

    public override void OnScreenTouch(Vector2 coord) {
        on = !on;
        canvas.SetActive(on);

        base.OnScreenTouch(coord);
    }
}
