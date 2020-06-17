public class TutorialParam {
    /// <summary>
    /// Title for the tutorial page.
    /// </summary>
    public LocalizedGroup title;
    /// <summary>
    /// Text for the tutorial page.
    /// </summary>
    public LocalizedGroup text;

    public TutorialParam(string ti, LocalizedParam[] tiArg, string te, LocalizedParam[] teArg) {
        title = new LocalizedGroup(ti, tiArg);
        text = new LocalizedGroup(te, teArg);
    }

    public TutorialParam(LocalizedGroup ti, LocalizedGroup te) {
        title = ti ?? new LocalizedGroup("");
        text = te ?? new LocalizedGroup("");
    }

    /// <summary>
    /// If no parameter is needed, use this to shorten code.
    /// </summary>
    /// <param name="ti">Title id.</param>
    /// <param name="te">Text id.</param>
    /// <param name="f">callback function.</param>
    public TutorialParam(string ti, string te) {
        title = new LocalizedGroup(ti, null);
        text = new LocalizedGroup(te, null);
    }
}