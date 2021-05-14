using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls one or more slide bar pointers and sets the status text.
/// </summary>
public abstract class SlideBarManager : MonoBehaviour {
    public SlideBar[] slideBars;
    [SerializeField] private Text status;

    public List<float> values = new List<float>();

    private class StatusInfo {
        public Color color;
        public string key;
    }

    private static readonly Dictionary<NumberStatus, StatusInfo> Info = new Dictionary<NumberStatus, StatusInfo> {
        {NumberStatus.Normal, new StatusInfo {color = Color.white, key = "General.StatusGood"}},
        {NumberStatus.Warning, new StatusInfo {color = Color.red, key = "General.StatusModerate"}},
        {NumberStatus.Danger, new StatusInfo {color = Color.blue, key = "General.StatusBad"}}
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
            status.GetComponent<LocalizedText>().SetText(Info[healthStatus].key);
            status.color = Info[healthStatus].color;
        }
    }

    protected abstract int GetPercentage(int index, float number);

    /// <summary>
    /// Get the the status of the specific/all slide bar.
    /// </summary>
    /// <param name="index">When set to -1, get a comprehensive status that
    /// combines the statuses of all slide bars. </param>
    /// <returns></returns>
    protected virtual NumberStatus GetStatus(int index = -1) {
        return NumberStatus.Normal;
    }
}