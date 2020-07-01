public class TutorialParam {
    /// <summary>
    /// Title for the tutorial page.
    /// </summary>
    public LocalizedGroup Title { get; }
    /// <summary>
    /// Text for the tutorial page.
    /// </summary>
    public LocalizedGroup Contents { get; }

    public TutorialParam(string ti, LocalizedParam[] tiArg, string te, LocalizedParam[] teArg) {
        Title = new LocalizedGroup(ti, tiArg);
        Contents = new LocalizedGroup(te, teArg);
    }

    public TutorialParam(LocalizedGroup ti, LocalizedGroup te) {
        Title = ti ?? new LocalizedGroup("");
        Contents = te ?? new LocalizedGroup("");
    }

    /// <summary>
    /// If no parameter is needed, use this to shorten code.
    /// </summary>
    /// <param name="ti">Title id.</param>
    /// <param name="te">Text id.</param>
    /// <param name="f">callback function.</param>
    public TutorialParam(string ti, string te) {
        Title = new LocalizedGroup(ti, null);
        Contents = new LocalizedGroup(te, null);
    }
}