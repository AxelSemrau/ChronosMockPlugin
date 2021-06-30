using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MockPlugin.Consumables
{
    /// <summary>
    /// Viel model for coffee machine related consumables.
    /// </summary>
    /// <remarks>
    /// This is a bit more complicated than strictly necessary. The idea is to show that you don't have to follow the predefined model of
    /// consumables pools/puddles strictly. In this case the consumabels get re-arranged and ordered by device by the view model.
    /// </remarks>
    public class CoffeeConsumableViewModel
    {
        private readonly CoffeeConsumableManager mManager;

        /// <summary>
        /// For the XAML Designer
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public CoffeeConsumableViewModel() : this(new CoffeeConsumableManager())
        {

        }

        public CoffeeConsumableViewModel(CoffeeConsumableManager value)
        {
            mManager = value;
            if (value?.Pools is INotifyCollectionChanged incs)
            {
                incs.CollectionChanged += RebuildLists;
            }
            RebuildLists(null, null);
        }

        private void RebuildLists(object sender, NotifyCollectionChangedEventArgs e)
        {
            Ingredients.Clear();
            var allIngredients = mManager.Pools.OfType<CoffeeIngredient>().ToArray();
            foreach (var dev in allIngredients.Select(somePool => somePool.ForDevice).Distinct())
            {
                var newBundle = new IngredientsForDevice() {DeviceName = dev.Name};
                foreach (var someIng in allIngredients.Where(someIng => someIng.ForDevice == dev))
                {
                    newBundle.Ingredients.Add(someIng);
                }
                Ingredients.Add(newBundle);
            }
        }

        public ObservableCollection<IngredientsForDevice> Ingredients { get; } = new ObservableCollection<IngredientsForDevice>();
    }

    public class IngredientsForDevice
    {
        public string DeviceName { get; set; }
        public ObservableCollection<CoffeeIngredient> Ingredients { get; } = new ObservableCollection<CoffeeIngredient>();
    }
}