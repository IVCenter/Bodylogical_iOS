﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriusVisualizer : MonoBehaviour {
    public Color goodColor, intermediateColor, badColor;
    public Image heartIndicator, liverIndicator, kidneyIndicator;

    private int heartScore, liverScore, kidneyScore;
    private enum Status { good, intermediate, bad };

    void Start() {
        heartScore = Random.Range(0, 10);
        liverScore = Random.Range(0, 10);
        kidneyScore = Random.Range(0, 10);
    }

    public bool Visualize() {
        bool diff = false;

        int addition = GenerateAddition();
        if (GetStatus(heartScore) != GetStatus(heartScore + addition)) {
            diff = true;
        }
        heartScore += addition;
        heartIndicator.color = SetColor(heartScore);

        addition = GenerateAddition();
        if (GetStatus(kidneyScore) != GetStatus(kidneyScore + addition)) {
            diff = true;
        }
        kidneyScore += addition;
        kidneyIndicator.color = SetColor(kidneyScore);

        addition = GenerateAddition();
        if (GetStatus(liverScore) != GetStatus(liverScore + addition)) {
            diff = true;
        }
        liverScore += addition;
        liverIndicator.color = SetColor(liverScore);

        return diff;
    }

    #region TESTING PURPOSES ONLY
    /// <summary>
    /// TODO: to be removed by real data
    /// </summary>
    private int GenerateAddition() {
        return Random.Range(-1, 2);
    }

    private Color SetColor(int score) {
        if (score < 4) {
            return badColor;
        } 

        if (score < 7) {
            return intermediateColor;
        }

        return goodColor;
    }

    private Status GetStatus(int score) {
        if (score < 4) {
            return Status.bad;
        }

        if (score < 7) {
            return Status.intermediate;
        }

        return Status.good;
    }
    #endregion
}