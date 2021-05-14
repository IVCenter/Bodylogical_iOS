using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// A slider with a low and high bar. 0-50 is considered normal, 50-75 is
/// considered warning, and 75-100 is considered danger.
/// </summary>
public class CircularSlideBar : SlideBar {
    [SerializeField] private Image normalBar, warningBar, dangerBar;
    [SerializeField] private Color normalColor, warningColor, dangerColor;
    
    public override void SetProgress(int progress) {
        this.progress = progress;
        
        if (progress <= 50) {
            normalBar.fillAmount = progress / 100f;
            warningBar.fillAmount = 0.01f;
            dangerBar.fillAmount = 0.01f;
            
            if (progress > 45) {
                normalBar.color = ((50 - progress) * normalColor - (45 - progress) * warningColor) / 5;
            }  else {
                normalBar.color = normalColor;
            }
            
        } else if (progress <= 75) {
            normalBar.fillAmount = 0.5f;
            warningBar.fillAmount = (progress - 50) / 100f;
            dangerBar.fillAmount = 0.01f;

            if (progress > 70) {
                warningBar.color = ((75 - progress) * warningColor - (70 - progress) * dangerColor) / 5;
            } else {
                warningBar.color = warningColor;
            }
            
            normalBar.color = warningBar.color;
            
        } else {
            normalBar.fillAmount = 0.5f;
            warningBar.fillAmount = 0.25f;
            dangerBar.fillAmount = (progress - 75) / 100f;

            normalBar.color = dangerColor;
            warningBar.color = dangerColor;
        }
    }
}