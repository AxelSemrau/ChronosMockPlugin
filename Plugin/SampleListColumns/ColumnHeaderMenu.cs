using System;
using System.Collections.Generic;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.Columns;

namespace MockPlugin.SampleListColumns
{
    /// <summary>
    /// Just a general purpose column header menu.
    /// </summary>
    public class ColumnHeaderMenu : IColumnMenu
    {
        private readonly Func<string> mGetCaption;
        private readonly Action<IReadOnlyList<ICellAccessor>> mMenuAction;

        public ColumnHeaderMenu(Func<string> getCaption, Action<IReadOnlyList<ICellAccessor>> menuAction)
        {
            mGetCaption = getCaption;
            mMenuAction = menuAction;
        }

        public string Caption => mGetCaption();
        public void ProcessSelected(IReadOnlyList<ICellAccessor> affectedCells) => mMenuAction(affectedCells);
    }
}