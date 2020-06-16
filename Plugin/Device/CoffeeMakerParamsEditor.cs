using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MockPlugin.Device
{
    internal class CoffeeMakerParamsEditor : UITypeEditor
    {
        #region Overrides of UITypeEditor

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var myPars = (value as CoffeMakerParams)?.Clone();
            var editorService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            var theForm = new CoffeeMakerParamsUI() {Parameters = myPars};
            if (editorService?.ShowDialog(theForm) == DialogResult.OK)
            {
                return theForm.Parameters;
            }
            // if the dialog was cancelled, return the old parameter set.
            return value;
        }

        #endregion
    }
}