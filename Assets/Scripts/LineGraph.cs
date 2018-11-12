using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://stackoverflow.com/questions/38672831/
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Transform))]
public class LineGraph : MonoBehaviour {
  /// <summary>
  /// FOR TESTING ONLY
  /// </summary>
  public List<int> values;

  private float graphWidth, graphHeight;
  private LineRenderer lineRenderer;

  void Start() {
    graphWidth = GetComponent<RectTransform>().rect.width;
    graphHeight = GetComponent<RectTransform>().rect.height;
    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.useWorldSpace = false;
    Draw();
  }

  /// <summary>
  /// TODO: Take List as a parameter
  /// </summary>
  public void Draw() {
    int size = values.Count;
    lineRenderer.positionCount = size;

    float xInterval = graphWidth / size;

    // scale from largest to graphHeight
    int max = int.MinValue, min = int.MaxValue;
    foreach (int num in values) {
      if (num < min) {
        min = num;
      }
      if (num > max) {
        max = num;
      }
    }

    float yInterval = graphHeight * (1 - min) / (max - min);

    for (int i = 0; i < size; i++) {
      float y = values[i] * yInterval;
      float x = i * xInterval;

      lineRenderer.SetPosition(i, new Vector3(x - graphWidth / 2, y - graphHeight / 2, 0));
    }
  }
}
