using System;
using System.Collections.Generic;
using AxelSemrau.Chronos.Plugin.Columns;

/*!
 * \brief The classes in this namespace demonstrate how to define your own sample list column types.
 */
namespace MockPlugin.SampleListColumns
{
    /// <summary>
    /// Just an example for freshly updated combobox entries.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class CoffeeTypeColumn : IColumnTypeDefinition
    {
        public string InternalName { get; } = "MockPlugin_CoffeeType";
        public string VisibleName { get; } = "Coffee Type";

        /// <summary>
        /// Last entry is based on current time to show when it was created.
        /// </summary>
        public IEnumerable<object> ComboboxItems
        {
            get
            {
                yield return "Americano";
                yield return "Espresso";
                yield return "Cortado";
                yield return $"Hyped coffee of the second ({DateTime.Now:yyyy-MM-dd HH:mm:ss})";
            }
        }

        public Type ValueType { get; } = typeof(string);
        public IEnumerable<IColumnMenu> ColumnHeaderMenu { get; } = null;
    }
}
