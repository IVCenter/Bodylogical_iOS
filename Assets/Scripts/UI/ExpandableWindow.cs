using System;
using System.Collections;
using UnityEngine;


public class ExpandableWindow : MonoBehaviour {
    [SerializeField] private RectTransform icon;
    [SerializeField] private Transform panel;
    [SerializeField] private float animationTime = 1;

    private Vector3 startingScale;
    private bool running;
    private bool expanded;
    
    private void Start() {
        Vector2 iconSize = icon.sizeDelta;
        Vector2 panelSize = panel.GetComponent<RectTransform>().sizeDelta;
        startingScale = new Vector3(iconSize.x / panelSize.x, iconSize.y / panelSize.y, 1);
        panel.localScale = startingScale;
    }

    public void OnClick() {
        if (!running) {
            StartCoroutine(expanded ? Collapse() : Expand());
            expanded = !expanded;
        }
    }

    private IEnumerator Expand() {
        running = true;
        
        float time = 0;
        Vector3 velocity = Vector3.zero;
        while (time < animationTime) {
            panel.localScale = Vector3.SmoothDamp(panel.localScale, Vector3.one, ref velocity,
                animationTime - time);
            time += Time.deltaTime;
            yield return null;
        }

        panel.localScale = Vector3.one;
        running = false;
    }

    private IEnumerator Collapse() {
        running = true;
        
        float time = 0;
        Vector2 velocity = Vector2.zero;
        while (time < animationTime) {
            panel.localScale = Vector2.SmoothDamp(panel.localScale, startingScale, ref velocity,
                animationTime - time);
            time += Time.deltaTime;
            yield return null;
        }

        panel.localScale = startingScale;
        running = false;
    }
}
