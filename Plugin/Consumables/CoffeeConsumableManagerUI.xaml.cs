using System.Windows.Controls;
using System.Windows.Media;
using AxelSemrau.Chronos.Plugin.Consumables;

namespace MockPlugin.Consumables
{
    /// <summary>
    /// Toolbox for all coffee machine related consumables. Currently just a display of the consumable levels.
    /// </summary>
    public partial class CoffeeConsumableManagerUI : UserControl, IConsumableManagerToolbox<CoffeeConsumableManager>
    {
        public CoffeeConsumableManagerUI()
        {
            InitializeComponent();
            NavBarIcon = (ImageSource)Resources["CoffeeMachineBitmapImage"];
        }

        /// <summary>
        /// We don't want a 1:1 display of the pools and puddles, but a re-arranged model grouped by related device.
        /// </summary>
        public CoffeeConsumableManager Manager { set => DataContext = new CoffeeConsumableViewModel(value);
        }

        public string DisplayName => "Coffee Machine";
        public ImageSource NavBarIcon { get; }
    }
}
