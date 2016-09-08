using System.Windows.Forms;
using MockPlugin.Device;

namespace MockPlugin.Misc
{
    public partial class StatusViewControl : UserControl, AxelSemrau.Chronos.Plugin.IStatusView<MockDevice>
    {
        private MockDevice mDev;

        public StatusViewControl()
        {
            InitializeComponent();
        }

        public MockDevice Device
        {
            get
            {
                return mDev;
            }
            set
            {
                mDev = value;
                if (value != null)
                {
                    SimpleLabel.Text += " " + value.Name;
                }
            }
        }
    }
}