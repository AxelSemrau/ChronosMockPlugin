﻿using System;
using System.ServiceModel;
using AxelSemrau.Chronos.Plugin;
using MockPlugin.RemoteAccessTester;
// ReSharper disable LocalizableElement

namespace MockPlugin.SampleList
{
    /// <summary>
    /// Example for communication with external programs.
    /// </summary>
    /// <remarks>
    /// This example uses WCF to show how you can trigger actions within the plugin from external programs.
    /// See RemoteAccessTester.MessageTesterForm for the client side.
    /// Please note that WCF will be discontinued by Microsoft, the server components will not be part of .Net Core in the future.
    /// So, while this is a working example for current Chronos releases, you should maybe not base a new project on this
    /// method of interprocess communication, but switch to something else like gRPC or an ASP.net WebAPI.
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
            return System.Windows.Forms.MessageBox.Show(
                       $"Plugin does something on remote request: {someParameter}\nSucceeded?", 
                       "Remote request to plugin", 
                       System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
        }
        private static ServiceHost mHost;
        private static void HostFaultedHandler(object sender, EventArgs e)
        {
            if (Helpers.Gui != null)
            {
                System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,"Remote plugin service failed.");
            }
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

        public static void StopService()
        {
            try
            {
                if (mHost?.State == CommunicationState.Opened)
                {
                    mHost?.Close();
                }
            }
            catch 
            {
                // nothing
            }
        }
    }
}
