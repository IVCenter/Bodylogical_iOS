using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Automatically adjusts the size of the panel to fit tutorial text.
/// </summary>
public class TutorialPanel : MonoBehaviour {
    [SerializeField] private Text text;
    [SerializeField] private Transform expandableBorder;
    [SerializeField] private Transform expandablePanel;
    [SerializeField] private Transform bottomBorder;
    [SerializeField] private Transform bottomPanel;

    /// <summary>
    /// Updates the height of the tutorial panel to contain all the texts.
    /// </summary>
    public void UpdatePanel() {
        float newHeight = text.preferredHeight;
        // The tutorial text is of font size 25, and its default height is
        // 3 lines (75). A scale of one cannot fit 2 lines (50) fully, so we
        // divide by 40. We also want it to have a minimum height, 
        float panelScale = Mathf.Max(1, (newHeight - 75) / 40 + 1);
        expandablePanel.localScale = new Vector3(expandablePanel.localScale.x,
            panelScale, expandablePanel.localScale.z);

        // The border is about 1/3 of the length of the panel, but the top 1/3
        // of the panel is covered by the header part.
        float borderScale = (panelScale - 1) * 3.2f + 1f
            + (panelScale == 1 ? 0.7f : 0);
        expandableBorder.localScale = new Vector3(expandableBorder.localScale.x,
            borderScale, expandableBorder.localScale.z);

        // For every scale, shift the bottom parts down by 0.01.
        float bottomShift = (panelScale - 1) * -0.01f;
        bottomBorder.localPosition = new Vector3(bottomBorder.localPosition.x,
            bottomShift, bottomBorder.localPosition.z);
        bottomPanel.localPosition = new Vector3(bottomPanel.localPosition.x,
            bottomShift, bottomPanel.localPosition.z);
    }
}
