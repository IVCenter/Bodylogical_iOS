using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiverDisplay : OrganDisplay {
    public GameObject liver;
    public Texture normalMap;

    void Awake() {
        foreach (Transform obj in liver.transform) {
            Material mat = obj.GetComponent<MeshRenderer>().material;
            mat.EnableKeyword("_NORMALMAP");
            mat.EnableKeyword("_PARALLAXMAP");
        }
    }

    public override void DisplayOrgan(int score, HealthStatus status) {
        if (gameObject.activeInHierarchy) {
            liver.SetActive(true);
            switch (PriusManager.Instance.ShowStatus) {
                case PriusShowStatus.Character:
                    if (status != HealthStatus.Good) {
                        // healthScore ranges from 0 - 60 when not in Good status.
                        // Converts it to 0-1 scale, and then lerp.
                        DisplayHeightMap(Mathf.Lerp(0.005f, 0.08f, score / 60.0f));

                    } else {
                        HideHeightMap();
                    }
                    break;
                case PriusShowStatus.Bad:
                    DisplayHeightMap(0.08f);
                    break;
                case PriusShowStatus.Intermediate:
                    DisplayHeightMap(0.005f);
                    break;
                case PriusShowStatus.Good:
                    HideHeightMap();
                    break;
            }
        }
    }

    private void DisplayHeightMap(float value) {
        foreach (Transform obj in liver.transform) {
            Material mat = obj.GetComponent<MeshRenderer>().material;
            mat.SetTexture("_BumpMap", normalMap);
            mat.SetFloat("_Parallax", value);
        }
    }

    private void HideHeightMap() {
        foreach (Transform obj in liver.transform) {
            obj.GetComponent<MeshRenderer>().material.SetTexture("_BumpMap", null);
        }
    }
}
