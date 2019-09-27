using System.Windows.Forms;

namespace MockPlugin.Device
{
    /// <summary>
    /// Simple user interfrace for the fake coffee maker's parameters.
    /// </summary>
    public partial class CoffeeMakerParamsUI : Form
    {
        /// <summary>
        /// We are using a seperate WPF part here for actually editing the parameters so that
        /// we can reuse it if, in the hopefully not-too-far future, Chronos supports data templates for this kind of task instead of UI Type Editors.
        /// </summary>
        private readonly EditMachineParams mParamsEditor;
        public CoffeeMakerParamsUI()
        {
            InitializeComponent();
            mParamsEditor = new EditMachineParams();
            mEditorElementHost.Child = mParamsEditor;
        }

        public CoffeMakerParams Parameters
        {
            get => mParamsEditor.DataContext as CoffeMakerParams;
            set => mParamsEditor.DataContext = value;
        }
    }
}
