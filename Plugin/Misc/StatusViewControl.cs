using System;
using System.Windows.Forms;
using MockPlugin.Device;
using MockPlugin.Properties;

/*!
 \brief Contains things that do not fit into the other categories.
 */
namespace MockPlugin.Misc
{
    /// <summary>
    /// This control is used to show the status for our MockDevice.
    /// </summary>
    public partial class StatusViewControl : UserControl, AxelSemrau.Chronos.Plugin.IStatusView<MockDevice>
    {
        private MockDevice mDev;

        public StatusViewControl()
        {
            InitializeComponent();
        }

        public MockDevice Device
        {
            get => mDev;
            set => mDev = value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SimpleLabel.Text = string.Format(LocalizeMockPlugin.StatusViewControl_timer1_Tick_,DateTime.Now.ToString("HH:mm:ss"),mDev?.Name);
        }
    }
}