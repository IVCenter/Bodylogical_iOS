using System.Collections.Generic;

/// <summary>
/// A utility class to handle network errors while making server requests.
/// One cannot do "yield return" within a try/catch loop. There are some ways around it:
/// - Use callbacks for error. However that callback cannot be another coroutine;
/// - Wrap around the Coroutine class;
/// - Use a utility class that is passed in as an argument.
/// </summary>
public class NetworkError {
    public NetworkStatus status = NetworkStatus.Uninitiated;
    public string message;
    
    private static readonly Dictionary<NetworkStatus, string> ErrorMsg = new Dictionary<NetworkStatus, string> {
        {NetworkStatus.ServerError, "Instructions.ServerError"},
        {NetworkStatus.BadRequest, "Instructions.RequestError"},
        {NetworkStatus.Unknown, "Instructions.RequestError"}
    };

    public string MsgKey => ErrorMsg[status];
}