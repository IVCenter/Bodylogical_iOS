using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SlideBarManager : MonoBehaviour {
    public SlideBarPointer[] slideBars;
    public Text status;

    protected List<float> values = new List<float>();

    private static readonly Dictionary<NumberStatus, Color> colors = new Dictionary<NumberStatus, Color> {
        { NumberStatus.Normal, Color.white },
        { NumberStatus.Warning, Color.red },
        { NumberStatus.Danger, Color.blue },
    };

    private static readonly Dictionary<NumberStatus, string> statusKeyDictionary = new Dictionary<NumberStatus, string> {
        { NumberStatus.Normal, "General.StatusGood" },
        { NumberStatus.Warning, "General.StatusModerate" },
        { NumberStatus.Danger, "General.StatusBad" },
    };

    public void SetSlideBar(int index, float number) {
        if (values.Count > index) {
            values[index] = number;
        } else {
            values.Insert(index, number);
        }
        slideBars[index].SetProgress(GetPercentage(index, number));

        if (status != null) {
            NumberStatus healthStatus = GetStatus();
            status.GetComponent<LocalizedText>().SetText(statusKeyDictionary[healthStatus]);
            status.color = colors[healthStatus];
        }
    }

    /// <summary>
    /// Increase the slide bar as time progresses.
    /// </summary>
    public void Interpolate() {
        for (int i = 0; i < slideBars.Length; i++) {
            StartCoroutine(slideBars[i].Interpolate(GetPercentage(i, values[i])));
        }
    }

    public abstract int GetPercentage(int index, float number);

    /// <summary>
    /// Get the the status of the specific/all slide bar.
    /// </summary>
    /// <param name="index">When set to -1, get a comprehensive status that
    /// combines the statuses of all slide bars. </param>
    /// <returns></returns>
    public virtual NumberStatus GetStatus(int index = -1) {
        return NumberStatus.Normal;
    }
}
