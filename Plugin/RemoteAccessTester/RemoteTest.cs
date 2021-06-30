using System;
using System.ServiceModel;
using System.Windows.Forms;

namespace MockPlugin.RemoteAccessTester
{
    /// <summary>
    /// Shows how to communicate from an external program with a Plugin in order to trigger actions in Chronos.
    /// </summary>
    public partial class MessageTesterForm : Form
    {
        public MessageTesterForm()
        {
            InitializeComponent();
        }

        private IMockPlugin mService;

        /// <summary>
        /// Get the service when it is needed for the first time.
        /// </summary>
        private IMockPlugin Service
        {
            get
            {
                if (mService == null)
                {
                    var endpoint = new EndpointAddress(EndpointDef.Endpoint);
                    mService = ChannelFactory<IMockPlugin>.CreateChannel(new NetTcpBinding(), endpoint);
                }
                return mService;
            }
        }

        /// <summary>
        /// When enter is pressed, send our parameter to the plugin provided service and show the result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    var retval = Service.DoSomething(tbMessage.Text);
                    lvRequests.Items.Add(string.Format("Request {0}: Result {1}",tbMessage.Text, retval));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error accessing plugin: " + ex.Message);
                }
            }
        }

    }
}
