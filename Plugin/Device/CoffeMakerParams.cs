using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace MockPlugin.Device
{
    /// <summary>
    /// We have a fancy coffee machine that can regulate the warmer temperature for the pot and has lamps for ambient light.
    /// </summary>
    /// <remarks>
    /// You must override the Equals operator to allow Chronos to check if two method's settings are compatible.
    /// Any better ideas? Your suggestions are welcome.
    /// </remarks>
    [Serializable]
    [Editor(typeof(CoffeeMakerParamsEditor),typeof(UITypeEditor))]
    public class CoffeMakerParams
    {
        public bool AmbientLight { get; set; }
        public int? WarmerTemperature { get; set; }

        public override string ToString()
        {
            return $"Ambient light {(AmbientLight ? "on" : "off")}, warmer temperature (°C) {(WarmerTemperature?.ToString() ?? "(off)")}";
        }

        #region Overrides of Object

        public override bool Equals(object obj)
        {
            return obj is CoffeMakerParams other && other.AmbientLight == AmbientLight &&
                   other.WarmerTemperature == WarmerTemperature;
        }

        /// <summary>
        /// Since all parameters are represented in the ToString representation, we can use its hash code instead of coming up with a different calculation method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion

        public int CompareTo(object obj)
        {
            if (obj is CoffeMakerParams other && other.AmbientLight == AmbientLight && other.WarmerTemperature == WarmerTemperature)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public CoffeMakerParams Clone()
        {
            return new CoffeMakerParams() {AmbientLight = AmbientLight, WarmerTemperature = WarmerTemperature};
        }
    }
}