using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SliderManager : MonoBehaviour {
  public List<SlideBar> sliders;
  public Text status;
  public List<string> statusTexts;

  public void SetSlider(int index, float number) {
    sliders[index].SetProgress(GetPercentage(number));
  }

  public void SetAll(float number) {
    int percentage = GetPercentage(number);
    foreach (SlideBar slider in sliders) {
      slider.SetProgress(percentage);
    }
  }

  public abstract int GetPercentage(float number);
}
