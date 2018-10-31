using UnityEngine;
using UnityEngine.UI;

public class CircularSlideBar : MonoBehaviour {

  public Image progressBar1, progressBar2;
  public int progress;

  private int time;

  // Use this for initialization
  void Start () {
    setProgress(progress);
  }
	
	// Update is called once per frame
	void Update () {
    time++;
    if (time == 10) {
      time = 0;
      increase();
    }
  }

  void increase() {
    setProgress(progress);
    if (progress < 100) {
      progress++;
    }
  }

  void setProgress(int progress) {
    this.progress = progress;
    if (progress <= 75) {
      progressBar1.fillAmount = progress / 100f;
      progressBar2.fillAmount = 0.01f;
    } else if (progress <= 100) {
      progressBar1.fillAmount = 0.75f;
      progressBar2.fillAmount = (progress - 75) / 100f;
    }
  }
}
