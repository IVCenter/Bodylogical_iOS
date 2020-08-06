public class LocalizedGroup {
    public string Id { get; }
    public LocalizedParam[] Args { get; }

    public LocalizedGroup(string i, LocalizedParam[] a = null) {
        Id = i;
        Args = a ?? new LocalizedParam[0];
    }
}