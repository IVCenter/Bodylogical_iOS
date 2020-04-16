using System.Collections;
using UnityEngine;

public class TestTutorial : MonoBehaviour {
    private bool hide;

    private void Start() {
        StartCoroutine(SetHide());
        TutorialParam param = new TutorialParam("Legends.PriusWelcome", "Tutorials.LCIntroText1");
        TutorialManager.Instance.ShowTutorial(param, TutorialManager.Instance.tutorialParent, () => hide);
    }

    private IEnumerator SetHide() {
        yield return new WaitForSeconds(10);
        hide = true;
    }
}
