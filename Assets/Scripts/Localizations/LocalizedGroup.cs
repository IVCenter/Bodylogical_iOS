public class LocalizedGroup {
    public string id;
    public LocalizedParam[] args;

    public LocalizedGroup(string i, LocalizedParam[] a = null) {
        id = i;
        args = a ?? new LocalizedParam[0];
    }
}