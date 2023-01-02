using System.Windows;
using System.Windows.Controls;
using AxelSemrau.Chronos.Plugin;
using MockPlugin.Device;

namespace MockPlugin.Misc
{
    /// <summary>
    /// Interaction logic for LargeDeviceStatusView.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class LargeDeviceStatusView : UserControl, ILargeStatusView<MockDevice>
    {
        public LargeDeviceStatusView()
        {
            InitializeComponent();
            DataContext = new FakeStatusViewModel();
        }

        public MockDevice Device { get; set; }

        public object Title => $"Large status view for device {(Device?.Name ?? "N/A")}";

        public Visibility Visible { get; }
    }

    public class FakeStatusViewModel
    {
        // ReSharper disable once UnusedMember.Global
        public string FakeStatus { get; set; } = "Just some fake status";
    }
}
