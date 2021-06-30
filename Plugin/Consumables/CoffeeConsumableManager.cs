using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.Consumables;
using MockPlugin.Device;

namespace MockPlugin.Consumables
{
    /// <summary>
    /// Keeps track of all consumables that are associated to our mock coffee machine.
    /// </summary>
    /// <remarks>
    /// A real implementation would of course include some warning or error levels.
    /// </remarks>
    public class CoffeeConsumableManager : IManageConsumables, IDirectDeviceAccess
    {
        private IEnumerable<IDevice> mConfiguredDevices;
        private readonly ObservableCollection<IConsumablePool> mPools = new ObservableCollection<IConsumablePool>();
        public ReadOnlyObservableCollection<IConsumablePool> Pools { get; }

        /// <summary>
        /// Will be cloned for schedule validation.
        /// </summary>
        /// <returns></returns>
        public IManageConsumables Clone()
        {
            return new CoffeeConsumableManager(this);
        }

        private CoffeeConsumableManager(CoffeeConsumableManager copyFrom) : this()
        {
            mKnownCoffeeMakers = new List<MockDevice>(copyFrom.mKnownCoffeeMakers);
            mPerDeviceConsumables.AddRange(copyFrom.mPerDeviceConsumables.Select(item => item.Clone(this)));
            PoolsChanged();
        }

        // This is called by Chronos.
        // ReSharper disable once MemberCanBePrivate.Global
        public CoffeeConsumableManager()
        {
            Pools = new ReadOnlyObservableCollection<IConsumablePool>(mPools);
            // looks like we are running in the designer?
            if (Helpers.Debug == null)
            {
                ConfiguredDevices = new[] {new MockDevice() {Name = "Fake 1"}, new MockDevice() {Name = "Fake 2"}};
            }
        }

        public IEnumerable<IDevice> ConfiguredDevices
        {
            set
            {
                mConfiguredDevices = value;
                UpdateDeviceConsumables();
            }
        }

        private readonly List<MockDevice> mKnownCoffeeMakers = new List<MockDevice>();
        private readonly List<MockConsumablesForCoffeeMakerDevice> mPerDeviceConsumables = new List<MockConsumablesForCoffeeMakerDevice>();

        private void UpdateDeviceConsumables()
        {
            var currentDevs = mConfiguredDevices.OfType<MockDevice>().ToList();
            var removedDevs = mKnownCoffeeMakers.Except(currentDevs).ToList();
            mPerDeviceConsumables.RemoveAll(item => removedDevs.Contains(item.ForDevice));
            var newDevs = currentDevs.Except(mKnownCoffeeMakers).ToList();
            mPerDeviceConsumables.AddRange(newDevs.Select(someDev => new MockConsumablesForCoffeeMakerDevice(someDev,this)));
            mKnownCoffeeMakers.Clear();
            mKnownCoffeeMakers.AddRange(currentDevs);
            if (newDevs.Any() || removedDevs.Any())
            {
                PoolsChanged();
            }
        }

        private void PoolsChanged()
        {
            mPools.Clear();
            foreach (var somePool in mPerDeviceConsumables.SelectMany(item => item.Pools))
            {
                mPools.Add(somePool);
            }
        }

        public static string GetLocationIdentifier(MockDevice dev, string ingredientName)
        {
            return $"{dev.Name}:{ingredientName}";
        }
    }
}
