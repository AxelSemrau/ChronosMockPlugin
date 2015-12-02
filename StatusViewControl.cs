using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MockPlugin
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