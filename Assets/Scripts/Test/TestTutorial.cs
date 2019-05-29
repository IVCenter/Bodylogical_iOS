using UnityEngine;

public class TestTutorial : MonoBehaviour {
    private void Start() {

        TutorialParam[] groups = {
            new TutorialParam("Legends.PriusWelcome", "Tutorials.LCIntroText1"),
            new TutorialParam("Legends.PriusWelcome", null, "Instructions.PathSwitch",
                new LocalizedParam[] {
                    new LocalizedParam("General.Lang-en_US", true)
                }
            )
        };
        TutorialManager.Instance.ShowTutorial(groups);

        groups = new TutorialParam[] {
            new TutorialParam("Legends.PriusWelcome", "Legends.PriusComparison"),
            new TutorialParam("Legends.PriusWelcome", null, "Instructions.PathSwitch",
                new LocalizedParam[] {
                    new LocalizedParam("General.Lang-ja_JP", true)
                }
            )
        };
        TutorialManager.Instance.ShowTutorial(groups);

    }
}
