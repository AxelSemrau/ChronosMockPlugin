using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

// ReSharper disable LocalizableElement

namespace MockPlugin.Device
{
    /// <summary>
    /// Just a primitive UI Type Editor to demonstrate how you can add an editor of your own for connection strings.
    /// </summary>
    public class ConnectionEditor : UITypeEditor
    {
        #region Overrides of UITypeEditor

        /// <summary>
        /// We want to show a modal dialog box.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Create your editor form, initialize it from the given value, return if the user accepted the new value.
        /// </summary>
        /// <param name="context">
        /// The context.Instance is the device. If the configuration has not been saved, this could be a disposed temporary object, but you can still get its type. This can be helpful if you want to reuse the 
        /// same editor with minor runtime modifications for different device types.
        /// </param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            var myEditor = new ConnectionStringEditor {TheConnectionEdit = {Text = value?.ToString()}};
            myEditor.Text = $"{myEditor.Text} for {context?.Instance?.GetType().Name}";
            myEditor.EditLabel.Text = $"Editor for {context?.PropertyDescriptor?.Name}";
            var res = editService?.ShowDialog(myEditor) ?? DialogResult.Cancel;
            return res == DialogResult.OK ? myEditor.TheConnectionEdit.Text : value;
        }

        #endregion
    }
}