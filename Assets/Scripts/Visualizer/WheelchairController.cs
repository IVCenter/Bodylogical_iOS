using System.Collections.Generic;
using UnityEngine;

public class WheelchairController : MonoBehaviour {
    private List<Renderer> renderers;

    private static readonly int alphaScale = Shader.PropertyToID("_AlphaScale");

    public float Alpha {
        set {
            if (renderers == null) {
                Initialize();
            }
            
            foreach (Renderer r in renderers) {
                r.material.SetFloat(alphaScale, value);
            }
        }
    }

    private void Start() {
        Initialize();
    }
    
    private void Initialize() {
        renderers = transform.SearchAllWithType<Renderer>();
    }
}