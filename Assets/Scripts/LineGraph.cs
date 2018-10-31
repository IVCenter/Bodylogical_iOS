using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://stackoverflow.com/questions/38672831/
/// TODO: get this working
/// </summary>
public class LineGraph : MonoBehaviour {

  public float graphWidth;
  public float graphHeight;
  public LineRenderer newLineRenderer;
  public List<int> decibels;
  int vertexAmount = 50;
  float xInterval;


  // Use this for initialization
  void Start() {
    graphWidth = newLineRenderer.GetComponent<RectTransform>().rect.width;
    graphHeight = newLineRenderer.GetComponent<RectTransform>().rect.height;
    newLineRenderer = GetComponentInChildren<LineRenderer>();
    newLineRenderer.positionCount = vertexAmount;

    xInterval = graphWidth / vertexAmount;

    Draw();
  }

  //Display 1 minute of data or as much as there is.
  public void Draw() {
    if (decibels.Count == 0)
      return;

    float x = 0;

    for (int i = 0; i < vertexAmount && i < decibels.Count; i++) {
      int _index = decibels.Count - i - 1;

      float y = decibels[_index] * (graphHeight / 130); //(Divide grapheight with the maximum value of decibels.
      x = i * xInterval;

      newLineRenderer.SetPosition(i, new Vector3(x - graphWidth / 2, y - graphHeight / 2, 0));
    }
  }
}
