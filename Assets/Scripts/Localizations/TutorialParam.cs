public class TutorialParam {
    /// <summary>
    /// Title for the tutorial page.
    /// </summary>
    public LocalizedGroup Title { get; }
    /// <summary>
    /// Text for the tutorial page.
    /// </summary>
    public LocalizedGroup Contents { get; }

    public TutorialParam(string title, LocalizedParam[] titleArg, string contents, LocalizedParam[] contentArg) {
        Title = new LocalizedGroup(title, titleArg);
        Contents = new LocalizedGroup(contents, contentArg);
    }

    public TutorialParam(LocalizedGroup title, LocalizedGroup contents) {
        Title = title ?? new LocalizedGroup("");
        Contents = contents ?? new LocalizedGroup("");
    }

    /// <summary>
    /// If no parameter is needed, use this to shorten code.
    /// </summary>
    /// <param name="title">Title id.</param>
    /// <param name="contents">Contents id.</param>
    public TutorialParam(string title, string contents) {
        Title = new LocalizedGroup(title);
        Contents = new LocalizedGroup(contents);
    }
}