using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour {
    public void Start() {
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null) {
            trigger = gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry touch = new EventTrigger.Entry {
            eventID = EventTriggerType.PointerDown
        };
        touch.callback.AddListener(data => { OnTouchDown(); });

        EventTrigger.Entry press = new EventTrigger.Entry {
            eventID = EventTriggerType.Drag
        };
        press.callback.AddListener(data => { OnTouchHold(); });

        EventTrigger.Entry leave = new EventTrigger.Entry {
            eventID = EventTriggerType.PointerUp
        };
        leave.callback.AddListener(data => { OnTouchUp(); });

        trigger.triggers.Add(touch);
        trigger.triggers.Add(press);
        trigger.triggers.Add(leave);
    }

    /// <summary>
    /// Invokes when the mouse/finger touches the component.
    /// </summary>
    public virtual void OnTouchDown() { }

    /// <summary>
    /// Invokes when the mouse/finger holds on the component.
    /// </summary>
    public virtual void OnTouchHold() { }

    /// <summary>
    /// Invokes when the mouse/finger leaves the component.
    /// </summary>
    public virtual void OnTouchUp() { }
}