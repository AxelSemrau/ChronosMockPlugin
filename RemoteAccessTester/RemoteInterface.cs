using System.ServiceModel;

namespace MockPlugin.RemoteAccessTester
{
    /// <summary>
    /// Very basic service, only to show that you can trigger some action from outside and
    /// get a response.
    /// </summary>
    [ServiceContract]
    public interface IMockPlugin
    {
        [OperationContract]
        bool DoSomething(string someParameter);
    }
    /// <summary>
    /// To avoid duplicating the endpoint definition in server and client
    /// </summary>
    public static class EndpointDef
    {
        public const string Endpoint = "net.tcp://localhost:12543/RemotePluginService";
    }
}
