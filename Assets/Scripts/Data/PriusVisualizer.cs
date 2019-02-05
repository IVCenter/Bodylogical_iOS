using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriusVisualizer : MonoBehaviour {
    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;

    public void Visualize() {
        heartIndicator.color = SetColor();
        liverIndicator.color = SetColor();
        kidneyIndicator.color = SetColor();
    }

    /// <summary>
    /// FOR TESTING PURPOSES ONLY.
    /// TODO: to be removed by real data
    /// </summary>
    private Color SetColor() {
        int point = Random.Range(0, 10);
        if (point < 4) {
            return badColor;
        } 

        if (point < 7) {
            return intermediateColor;
        } 

        return goodColor;
    }
}
