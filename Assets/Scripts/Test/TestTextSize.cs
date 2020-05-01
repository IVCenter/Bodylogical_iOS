using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestTextSize : MonoBehaviour {
    [SerializeField] private Text text;
    [SerializeField] private TutorialPanel panel;

    private void Start() {
        StartCoroutine(Test());    
    }

    private IEnumerator Test() {
        text.text = "This is a one-liner";
        panel.UpdatePanel();
        yield return new WaitForSeconds(3);
        text.text = "This is a two-liner\nこれは2行のテキスト";
        panel.UpdatePanel();
        yield return new WaitForSeconds(3);
        text.text = "The default panel can hold 3 lines\nAfter the 4th line the panel will expand\n" +
            "You can see this right now\n";
        panel.UpdatePanel();
        yield return new WaitForSeconds(5);
        text.text = "The next example will be super long\nReal tutorials won't be that long\n" +
            "This is just to test the system's capabilities";
        panel.UpdatePanel();
        yield return new WaitForSeconds(5);
        text.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        panel.UpdatePanel();
    }
}
