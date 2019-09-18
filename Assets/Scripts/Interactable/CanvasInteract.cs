using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasInteract : Interactable {
    public Image panel;

    [Header("This UI canvas is clicked. Indicate what to do next.")]
    public UnityEvent clicked;

    [Header("This UI canvas is clicked with 2 touch inputs. Indicate what to do next.")]
    public UnityEvent doubleclicked;

    public GameObject touchDebug;

    public int touches;

    public float timePassed;

    /// <summary>
    /// Percentage of darkness added to the original color when the canvas is hovered.
    /// </summary>
    private static readonly float dark = 0.4f;
    private static readonly Color darkColor = new Color(dark, dark, dark, 0f);

    public override void OnTouchDown() {
        if (panel != null && Input.touchCount == 1) {
            panel.color -= darkColor;
            touches = 1;
            Debug.Log("Touches: " + touches);
        }

        /*if (panel != null && Input.touchCount >= 2) {
            panel.color -= darkColor;
            touches = 2;
            Debug.Log("Touches: " + touches);
        }
        //touchDebug.SetActive(true);*/
    }

    public override void OnTouchUp() {
        if (panel != null && touches == 1) {
            panel.color += darkColor;
            clicked.Invoke();
        }

        /*else if(panel != null && touches == 2)
        {
            panel.color += darkColor * 2;
            doubleclicked.Invoke();
        }*/

        //touchDebug.SetActive(false);
        touches = 0;
    }
}
