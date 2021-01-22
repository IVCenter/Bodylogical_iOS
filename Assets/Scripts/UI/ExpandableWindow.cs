using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExpandableWindow : MonoBehaviour {
    [SerializeField] private Transform panel;
    [SerializeField] private PulseIcon icon;
    [SerializeField] private float animationTime = 1;
    
    private bool running;
    private bool expanded;

    private FloatingWindow window;

    private void Start() {
        panel.localScale = Vector3.zero;
        icon.StartPulse();

        window = GetComponent<FloatingWindow>();
        if (window != null) {
            window.enabled = false;
        }
    }

    public void OnClick() {
        if (!running) {
            StartCoroutine(expanded ? Collapse() : Expand());
            expanded = !expanded;
        }
    }

    private IEnumerator Expand() {
        icon.StopPulse();
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

        if (window != null) {
            window.enabled = true;
        }
    }

    private IEnumerator Collapse() {
        running = true;
        
        float time = 0;
        Vector3 velocity = Vector3.zero;
        while (time < animationTime) {
            panel.localScale = Vector3.SmoothDamp(panel.localScale, Vector3.zero, ref velocity,
                animationTime - time);
            
            time += Time.deltaTime;
            yield return null;
        }
        
        panel.localScale = Vector3.zero;
        
        running = false;
        icon.StartPulse();
        
        if (window != null) {
            window.enabled = false;
        }
    }
}
