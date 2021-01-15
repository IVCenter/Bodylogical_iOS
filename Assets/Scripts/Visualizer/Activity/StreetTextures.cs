using System.Collections;
using UnityEngine;

public class StreetTextures : PropAnimation {
    private Material material;

    private void Start() {
        material = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// Refer to https://www.youtube.com/watch?v=auVq3TSz20o for more details.
    /// Notice that texture coordinates range from 0-1, so no need for Time.time.
    /// </summary>
    public override IEnumerator Animate() {
        float moves = 0;

        while (true) {
            moves += Speed * Time.deltaTime;
            if (moves > 1) {
                moves = 0;
            }

            material.mainTextureOffset = new Vector2(0, moves);
            yield return null;
        }
    }

    public override void Toggle(bool on) {
        // Do nothing
    }
}