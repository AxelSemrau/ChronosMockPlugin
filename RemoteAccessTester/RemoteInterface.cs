using System.ServiceModel;

/*!
 \brief Interaction with Chronos crossing the process barrier.
 A normal Chronos plugin is a class library loaded into Chronos.
 If you want to interact with Chronos from a different application, you can use the techniques demonstrated here
 to communicate with another process and remotely control Chronos.
 */
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
