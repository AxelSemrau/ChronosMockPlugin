using System.ComponentModel;
using AxelSemrau.Chronos.Plugin.Consumables;
using Ctc.Palplus.Integration.Driver.Entities;

namespace MockPlugin.Consumables
{
    /// <summary>
    /// One physical location where coffee machine consumables are stored.
    /// </summary>
    internal class CoffeeIngredientContainer : IConsumablePuddle
    {
        private Quantity mCurrentLevel;

        public CoffeeIngredientContainer(CoffeeIngredient forPool, string location, string containerName, Quantity maxLevel, Quantity currentLevel)
        {
            ParentPool = forPool;
            Location = CoffeeConsumableManager.GetLocationIdentifier(forPool.ForDevice, location);
            MaxLevel = new Quantity(maxLevel.Value, maxLevel.Unit);
            CurrentLevel = new Quantity(currentLevel.Value, currentLevel.Unit);
            ContainerName = containerName;
        }

        public string ContainerName { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public IConsumablePool ParentPool { get; }
        public string Location { get; }
        public Quantity MaxLevel { get; }

        public Quantity CurrentLevel
        {
            get => mCurrentLevel;
            private set
            {
                mCurrentLevel = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(CurrentLevel)));
            }
        }

        public void ModifyBy(Quantity relativeAmount)
        {
            if (relativeAmount != null)
            {
                CurrentLevel = CurrentLevel + relativeAmount;
            }
        }

        public void SetTo(Quantity absoluteAmount)
        {
            CurrentLevel = absoluteAmount;
        }

        public CoffeeIngredientContainer Clone(CoffeeIngredient forPool)
        {
            return new CoffeeIngredientContainer(forPool, Location, ContainerName, MaxLevel, CurrentLevel);
        }
    }

}
