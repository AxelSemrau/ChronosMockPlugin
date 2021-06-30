using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AxelSemrau.Chronos.Plugin.Consumables;
using MockPlugin.Device;

namespace MockPlugin.Consumables
{

    /// <summary>
    /// Base class for all used mock coffee ingredients.
    /// </summary>
    public class CoffeeIngredient : IConsumablePool
    {
        private CoffeeIngredient()
        {
            Puddles = new ReadOnlyObservableCollection<IConsumablePuddle>(mPuddles);
        }

        protected CoffeeIngredient(string ingredientName, MockDevice forDevice, CoffeeConsumableManager parentManager) : this()
        {
            ForDevice = forDevice;
            ParentManager = parentManager;
            ConsumableName = ingredientName;
        }

        private CoffeeIngredient(CoffeeIngredient copyFrom, CoffeeConsumableManager forManager) : this()
        {
            ForDevice = copyFrom.ForDevice;
            ParentManager = forManager;
            ConsumableName = copyFrom.ConsumableName;
            foreach (var somePuddle in copyFrom.mPuddles.OfType<CoffeeIngredientContainer>().Select(somePuddle => somePuddle.Clone(this)))
            {
                mPuddles.Add(somePuddle);
            }
        }

        public MockDevice ForDevice { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public IManageConsumables ParentManager { get; }
        public string ConsumableName { get; }

        protected readonly ObservableCollection<IConsumablePuddle> mPuddles = new ObservableCollection<IConsumablePuddle>();
        public ReadOnlyObservableCollection<IConsumablePuddle> Puddles { get; }

        public CoffeeIngredient Clone(CoffeeConsumableManager forManager)
        {
            return new CoffeeIngredient(this, forManager);
        }

        // ReSharper disable once UnusedMember.Global
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
