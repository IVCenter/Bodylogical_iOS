using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Circular slide bar with a high bar.
/// </summary>
public class CircularSlideBar : SlideBarPointer {
    [SerializeField] private Image normalBar, warningBar;
    [SerializeField] private Color normalColor, warningColor;
    
    public override void SetProgress(int progress) {
        this.progress = progress;
        
        if (progress <= 75) {
            normalBar.fillAmount = progress / 100f;
            warningBar.fillAmount = 0.01f;

            if (progress > 70) {
                normalBar.color =
                    ((75 - progress) * normalColor - (70 - progress) * warningColor) / 5;
            } else {
                normalBar.color = normalColor;
            }
        } else if (progress <= 100) {
            normalBar.fillAmount = 0.75f;
            warningBar.fillAmount = (progress - 75) / 100f;

            normalBar.color = warningColor;
        }
    }
}