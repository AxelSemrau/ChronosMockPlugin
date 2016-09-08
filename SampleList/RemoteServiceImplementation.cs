using System;
using System.ServiceModel;
using MockPlugin.RemoteAccessTester;

namespace MockPlugin.SampleList
{
    /// <summary>
    /// Example for communication with external programs.
    /// </summary>
    /// <remarks>
    /// This example uses WCF to show how you can trigger actions within the plugin from external programs.
    /// See RemoteAccessTester.MessageTesterForm for the client side.
    /// </remarks>
    public class RemotePluginService : IMockPlugin
    {
        private System.Windows.Forms.Form mMainWindow;
        /// <summary>
        /// Just uses a messagebox to show the external request.
        /// </summary>
        /// <param name="someParameter"></param>
        /// <returns></returns>
        public bool DoSomething(string someParameter)
        {
            if (mMainWindow == null)
            {
                mMainWindow = System.Windows.Forms.Control.FromHandle(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle) as System.Windows.Forms.Form;
            }
            // Just a few manipulations of the main window
            if (mMainWindow != null)
            {
                someParameter = someParameter.ToLowerInvariant();
                switch(someParameter)
                {
                    case "hide":
                        mMainWindow.Hide();
                        break;
                    case "show":
                        mMainWindow.Show();
                        break;
                    case "min":
                        mMainWindow.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                        break;
                    case "max":
                        mMainWindow.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                        break;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Could not get main window");
            }
            return System.Windows.Forms.MessageBox.Show(String.Format("Plugin does something on remote request: {0}\nSucceeded?", someParameter), 
                       "Remote request to plugin", 
                       System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
        }
        private static ServiceHost mHost;
        private static void HostFaultedHandler(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Remote plugin service failed.");
        }
        /// <summary>
        /// Starts a background service listening for external requests.
        /// </summary>
        internal static void StartService()
        {
            mHost = new ServiceHost(typeof(RemotePluginService));
            mHost.AddServiceEndpoint(typeof(IMockPlugin), new NetTcpBinding(), EndpointDef.Endpoint);
            mHost.Faulted += HostFaultedHandler;
            mHost.Open();
        }
    }
}
