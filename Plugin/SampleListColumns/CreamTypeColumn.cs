using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.Columns;
using MockPlugin.Properties;
using MockPlugin.Tasks;

namespace MockPlugin.SampleListColumns
{
    /// <summary>
    /// Simple case: Enum based column. Possible values are given in the StandardItems, custom editor is not used.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class CreamTypeColumn : IColumnTypeDefinition
    {
        public string InternalName => "MockPlugin_Cream";
        public string VisibleName => "Cream Type";

        public IEnumerable<object> ComboboxItems =>
            Enum.GetValues(typeof(BrewFrappuccino.CreamType)).OfType<object>();
        public Type ValueType => typeof(BrewFrappuccino.CreamType);

        public IEnumerable<IColumnMenu> ColumnHeaderMenu
        {
            get
            {
                yield return new ColumnHeaderMenu(() => LocalizeMockPlugin.CreamTypeColumn_ColumnHeaderMenu_Fill_down, FillDown);
                yield return new ColumnHeaderMenu(() => LocalizeMockPlugin.CreamTypeColumn_ColumnHeaderMenu_Cycle_through, CycleThrough);
            }
        }

        private void CycleThrough(IReadOnlyList<ICellAccessor> cells)
        {
            if (cells.Count >= 2)
            {
                var allValues = Enum.GetValues(typeof(BrewFrappuccino.CreamType)).Cast<BrewFrappuccino.CreamType>().ToArray();
                var currVal = (BrewFrappuccino.CreamType) cells[0].Value;
                foreach (var someCell in cells.Skip(1))
                {
                    var validItems = someCell.Column.StandardItems;
                    var roundtrips = 0;
                    do
                    {
                        if (currVal == allValues.Last())
                        {
                            currVal = allValues.First();
                            roundtrips += 1;
                        }
                        else
                        {
                            currVal += 1;
                        }
                    } while (roundtrips < 2 &&  (validItems?.Count ?? 0) > 0 && !validItems.Contains(currVal.ToString()));
                    someCell.Value = currVal;
                }
            }
        }

        /// <summary>
        /// Fill all given cells with the value of the top row.
        /// </summary>
        /// <param name="cells"></param>
        private void FillDown(IReadOnlyList<ICellAccessor> cells)
        {
            if (cells.Count >= 2)
            {
                var topVal = cells[0].Value;
                foreach (var someCell in cells.Skip(1))
                {
                    someCell.Value = topVal;
                }
            }
        }
    }

    /// <summary>
    /// Just provide a TypeConverter, and Chronos is happy.
    /// </summary>
    public class CreamTypeConverter : EnumConverter
    {
        /// <summary>
        ///  Luckily we can inherit everything necessary.
        /// </summary>
        public CreamTypeConverter() : base(typeof(BrewFrappuccino.CreamType))
        {
        }

        /// <summary>
        /// The actual conversion work string->object is done here.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>The base class implementation is good enough for us.</remarks>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
    }

}
