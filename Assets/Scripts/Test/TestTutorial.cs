using UnityEngine;

public class TestTutorial : MonoBehaviour {
    private void Start() {
        LocalizedGroup[] groups = {
            new LocalizedGroup("Legends.PriusWelcome"),
            new LocalizedGroup("Instructions.PathSwitch",
                new LocalizedParam[] {
                    new LocalizedParam("General.Lang-en_US", true)
                }
            )
        };
        TutorialManager.Instance.ShowTutorial(groups);

        groups = new LocalizedGroup[] {
            new LocalizedGroup("Legends.PriusComparison"),
            new LocalizedGroup("Instructions.PathSwitch",
                new LocalizedParam[] {
                    new LocalizedParam("General.Lang-ja_JP", true)
                }
            )
        };
        TutorialManager.Instance.ShowTutorial(groups);

    }
}
