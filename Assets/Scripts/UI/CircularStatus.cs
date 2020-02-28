using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Circular slide bar with a high bar.
/// </summary>
public class CircularStatus : SlideBarPointer {
    public Color[] colors;
    /// <summary>
    /// Intervals for different colors (in increasing order).
    /// interval: 0 -- i[0] -- i[1] -- i[2] -- ...
    /// color:     c[0] -- c[1] -- c[2] -- ...
    /// Please be sure that the last interval value is 100.
    /// </summary>
    public int[] intervals;

    private Image circle;
 
    private void Awake() {
        circle = GetComponent<Image>();
    }

    public override void SetProgress(int progress) {
        this.progress = progress;
        circle.fillAmount = progress / 100f;

        for (int i = 0; i < intervals.Length; i++) {
            if ((i == 0 || progress > intervals[i - 1])
                && progress <= intervals[i]) {
                circle.color = colors[i];
                break;
            }
        }
    }
}
