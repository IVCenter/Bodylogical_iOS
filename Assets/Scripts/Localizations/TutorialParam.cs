public class TutorialParam {
    /// <summary>
    /// Localization id of the text displayed for the tutorial.
    /// </summary>
    public string id;
    /// <summary>
    /// Arguments for the localized string.
    /// </summary>
    public LocalizedParam[] args;
    /// <summary>
    /// Callback function to be executed *after* the tutorial page.
    /// </summary>
    public System.Action callback;

    public TutorialParam(string i, LocalizedParam[] a = null, System.Action f = null) {
        id = i;
        args = a ?? (new LocalizedParam[0]);
        callback = f;
    }
}