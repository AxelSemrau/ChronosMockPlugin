using System.Collections.Generic;
using System.Linq;
using AxelSemrau.Chronos.Plugin.Consumables;
using Ctc.Palplus.Integration.Driver.Entities;
using MockPlugin.Device;

namespace MockPlugin.Consumables
{
    /// <summary>
    /// All consumables for one instance of our coffee machine.
    /// </summary>
    internal class MockConsumablesForCoffeeMakerDevice
    {
        public MockDevice ForDevice { get; }

        public MockConsumablesForCoffeeMakerDevice(MockDevice forDevice, CoffeeConsumableManager parentManager)
        {
            ForDevice = forDevice;
            Pools = new List<CoffeeIngredient>{new Coffee(ForDevice, parentManager), new Milk(ForDevice, parentManager), new VeganCream(ForDevice, parentManager)};
        }

        private MockConsumablesForCoffeeMakerDevice(MockConsumablesForCoffeeMakerDevice copyFrom,
            CoffeeConsumableManager parentManager)
        {
            ForDevice = copyFrom.ForDevice;
            Pools = new List<CoffeeIngredient>(copyFrom.Pools.Select(somePool => somePool.Clone(parentManager)));
        }

        internal class VeganCream : CoffeeIngredient
        {
            public const string Name = "Soy Milk";
            public VeganCream(MockDevice forDevice, CoffeeConsumableManager parentManager) : base(Name,forDevice,parentManager)
            {
                mPuddles.Add(new CoffeeIngredientContainer(this,Name, "Soy Milk Package",new Quantity(500,Units.MilliLiter), new Quantity(450,Units.MilliLiter)));
            }
        }

        internal class Milk : CoffeeIngredient
        {
            public const string Name = "Milk";
            public Milk(MockDevice forDevice, CoffeeConsumableManager parentManager) : base(Name, forDevice, parentManager)
            {
                mPuddles.Add(new CoffeeIngredientContainer(this, Name, "Milk Bottle", new Quantity(1, Units.Liter), new Quantity(975, Units.MilliLiter)));
            }
        }

        internal class Coffee : CoffeeIngredient, IConsumablePool
        {
            public const string Name = "Coffee";
            public Coffee(MockDevice forDevice, CoffeeConsumableManager parentManager) : base(Name, forDevice,parentManager)
            {
                mPuddles.Add(new CoffeeIngredientContainer(this,Name, "Coffee Container", new Quantity(250,Units.Gram), new Quantity(225,Units.Gram)));
            }
        }


        public IReadOnlyCollection<CoffeeIngredient> Pools { get; }

        public MockConsumablesForCoffeeMakerDevice Clone(CoffeeConsumableManager forManager)
        {
            return new MockConsumablesForCoffeeMakerDevice(this, forManager);
        }
    }
}
