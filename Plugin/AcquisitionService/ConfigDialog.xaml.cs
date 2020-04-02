using System.Windows;

namespace MockPlugin.AcquisitionService
{
    /// <summary>
    /// Interaktionslogik für ConfigDialog.xaml
    /// </summary>
    public partial class ConfigDialog
    {
        public ConfigDialog()
        {
            InitializeComponent();
        }

        public string ParamText { get => txtSomeParameter.Text;
            set => txtSomeParameter.Text = value;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
