using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Changes the image of the play/pause button.
/// </summary>
public class PlayPauseButton : MonoBehaviour {
    public Sprite play, pause;

    private Image image;

    void Awake() {
        image = GetComponent<Image>();
    }

    public void ChangeImage(bool on) {
        if (image != null) {
            if (on) {
                image.sprite = pause;
            } else {
                image.sprite = play;
            }
        }
    }
}
