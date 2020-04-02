using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.Columns;
using MockPlugin.Properties;

namespace MockPlugin.SampleListColumns
{
    /// <summary>
    /// Example column type that allows to pick a date and time from a graphical editor.
    /// </summary>
    /// <remarks>
    /// You can check with the coffee 
    /// </remarks>
    // ReSharper disable once UnusedType.Global
    public class DateColumn : IColumnTypeDefinition
    {
        // Do NOT localize this
        public string InternalName => "MockPlugin_Date";
        public string VisibleName => LocalizeMockPlugin.DateColumn_VisibleName_Date;
        public IEnumerable<object> ComboboxItems => null;
        public Type ValueType => typeof(MyDate);

        /// <summary>
        /// Provide some column header menus for increasing the date in typically used steps.
        /// </summary>
        public IEnumerable<IColumnMenu> ColumnHeaderMenu
        {
            get
            {
                yield return new ColumnHeaderMenu(() => LocalizeMockPlugin.DateColumn_ColumnHeaderMenu_Fill_down, cells => IncreaseBy(cells, dt => dt));
                yield return new ColumnHeaderMenu(() => LocalizeMockPlugin.DateColumn_ColumnHeaderMenu_Daily, cells => IncreaseBy(cells, dt => dt.Add(TimeSpan.FromDays(1))));
                yield return new ColumnHeaderMenu(() => LocalizeMockPlugin.DateColumn_ColumnHeaderMenu_Weekly, cells => IncreaseBy(cells,dt => dt.Add(TimeSpan.FromDays(7))));
                yield return new ColumnHeaderMenu(() => LocalizeMockPlugin.DateColumn_ColumnHeaderMenu_Monthly, cells => IncreaseBy(cells, dt => dt.AddMonths(1)));
            }
        }

        private void IncreaseBy(IReadOnlyList<ICellAccessor> cells, Func<DateTime,DateTime> adder)
        {
            if (cells.Count >= 2)
            {
                var currVal = (MyDate) cells[0].Value;
                foreach (var someCell in cells.Skip(1))
                {
                    currVal = new MyDate(adder(currVal.DateTime));
                    someCell.Value = currVal;
                }
            }
        }
    }

    /// <summary>
    /// Simple wrapper around DateTime that makes sure to always use the invariant culture.
    /// </summary>
    [Editor(typeof(MyDateEditor),typeof(UITypeEditor))]
    [TypeConverter(typeof(MyDateConverter))]
    public class MyDate
    {
        public MyDate(DateTime dt)
        {
            DateTime = dt;
        }

        public DateTime DateTime { get; }
        public override string ToString()
        {
            return DateTime.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Just like the default converter, but always use the invariant culture and wraps/unwraps the "MyDate".
    /// </summary>
    public class MyDateConverter : TypeConverter
    {
        private readonly TypeConverter mConv = TypeDescriptor.GetConverter(typeof(DateTime));
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is MyDate mdt)
            {
                return mConv.ConvertTo(context, CultureInfo.InvariantCulture, mdt.DateTime, destinationType);
            }
            return mConv.ConvertTo(context, CultureInfo.InvariantCulture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // TypeConverter throws a NotSupportedException if the type is already correct, don't call ConvertFrom then!
            if (value is DateTime dt)
            {
                return new MyDate(dt);
            }
            // ReSharper disable once PossibleNullReferenceException
            return new MyDate((DateTime) mConv.ConvertFrom(context, CultureInfo.InvariantCulture, value));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return mConv.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return mConv.CanConvertTo(context, destinationType);
        }
    }

    /// <summary>
    /// Just unwrap and edit the inner DateTime
    /// </summary>
    public class MyDateEditor : DateTimeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is MyDate mdt)
            {
                // ReSharper disable once PossibleNullReferenceException
                return new MyDate((DateTime)base.EditValue(context, provider, mdt.DateTime));
            }
            return base.EditValue(context, provider, value);
        }
    }
}
