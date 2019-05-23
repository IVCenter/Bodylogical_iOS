public class TutorialParam {
    /// <summary>
    /// Title for the tutorial page.
    /// </summary>
    public LocalizedGroup title;
    /// <summary>
    /// Text for the tutorial page.
    /// </summary>
    public LocalizedGroup text;
    /// <summary>
    /// Callback function to be executed *after* the tutorial page.
    /// </summary>
    public System.Action callback;

    public TutorialParam(string ti, LocalizedParam[] tiArg, string te, LocalizedParam[] teArg, System.Action f = null) {
        title = new LocalizedGroup(ti, tiArg);
        text = new LocalizedGroup(te, teArg);
        callback = f;
    }

    public TutorialParam(LocalizedGroup ti, LocalizedGroup te, System.Action f = null) {
        title = ti ?? new LocalizedGroup("");
        text = te ?? new LocalizedGroup("");
        callback = f;
    }

    /// <summary>
    /// If no parameter is needed, use this to shorten code.
    /// </summary>
    /// <param name="ti">Title id.</param>
    /// <param name="te">Text id.</param>
    /// <param name="f">callback function.</param>
    public TutorialParam(string ti, string te, System.Action f = null) {
        title = new LocalizedGroup(ti, null);
        text = new LocalizedGroup(te, null);
        callback = f;
    }
}