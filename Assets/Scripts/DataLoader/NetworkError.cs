/// <summary>
/// A utility class to handle network errors while making server requests.
/// One cannot do "yield return" within a try/catch loop. There are some ways around it:
/// - Use callbacks for error. However that callback cannot be another coroutine;
/// - Wrap around the Coroutine class;
/// - Use a utility class that is passed in as an argument.
/// </summary>
public class NetworkError {
    public bool success = true;
    public string message;
}