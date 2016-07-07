using System;
using System.Drawing;
using System.Windows.Forms;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Device
{
    /// <summary>
    /// Example implementation for a WinForms toolbox for our Mock Device.
    /// </summary>
    /// <remarks>
    /// Just passes a message to the device which will show it in a MessageBox and
    /// allows to connect/disconnect.
    /// </remarks>
    public partial class MockDeviceToolbox : UserControl, IToolbox<MockDevice>, IUsableDuringRun<MockDevice>
    {
        public MockDeviceToolbox()
        {
            InitializeComponent();
        }

        private MockDevice mDev;

        public MockDevice Device
        {
            get
            {
                return mDev;
            }
            set
            {
                mDev = value;
                if (mDev != null)
                {
                    // Handle connect/disconnect using data binding
                    checkBox1.DataBindings.Add(new Binding("Checked",
                                                   mDev,
                                                   "IsConnected",
                                                   false,
                                                   DataSourceUpdateMode.OnPropertyChanged));
                }
            }
        }

        private void btnShowMessage_Click(object sender, EventArgs e)
        {
            mDev.ShowTheMessage(textBox1.Text);
        }

        public Icon NavBarIcon
        {
            get
            {
                return Properties.Resources.Mock;
            }
        }

        public bool SequenceRunning
        {
            set { lblRunning.Text = (value ? "Sequence" : "No sequence") + " running"; }
        }
    }
}