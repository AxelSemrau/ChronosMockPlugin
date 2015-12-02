using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MockPlugin
{
    /// <summary>
    /// Provides an editor for the BrewFrappuccino-Task's "Composition" information.
    /// </summary>
    public partial class FrappuccinoCompositionEditor : Form
    {
        public FrappuccinoCompositionEditor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initialize from a given composition object.
        /// </summary>
        /// <param name="theData"></param>
        public FrappuccinoCompositionEditor(BrewFrappuccino.CompositionData theData)
        {
            InitializeComponent();
            mData = theData;
        }
        private readonly BrewFrappuccino.CompositionData mData;

        /// <summary>
        /// Let the GUI reflect our data.
        /// </summary>
        private void CopyDataToGUI()
        {
            if (mData.Volume < 200)
            {
                cboSize.SelectedIndex = 0;
            }
            else if (mData.Volume < 400)
            {
                cboSize.SelectedIndex = 1;
            }
            else
            {
                cboSize.SelectedIndex = 2;
            }
            switch (mData.Cream)
            {
                case BrewFrappuccino.CreamType.LowFat:
                    rbLowFat.Checked = true;
                    break;
                case BrewFrappuccino.CreamType.Normal:
                    rbRegularCream.Checked = true;
                    break;
                case BrewFrappuccino.CreamType.Vegan:
                    rbVeganCream.Checked = true;
                    break;
                default:
                    break;

            }
            chkCaffeine.Checked = !mData.DeCaffeinated;
            if (mData.MuchIce)
            {
                rbLotsOfIce.Checked = true;
            }
            else
            {
                rbLowIce.Checked = true;
            }
        }
        /// <summary>
        /// If we set the controls' values from the constructor, sometimes they don't get updated properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrappuccinoCompositionEditor_Shown(object sender, EventArgs e)
        {
            CopyDataToGUI();
        }

        /// <summary>
        /// When the user clicked on OK, we want to push the state represented by the GUI into our composition object.
        /// </summary>
        private void CopyGUItoData()
        {
            var oldVol = mData.Volume;
            if (cboSize.SelectedIndex == 0)
            {
                mData.Volume = 150;
            }
            else if (cboSize.SelectedIndex == 1)
            {
                mData.Volume = 250;
            }
            else
            {
                mData.Volume = 450;
            }
            mData.MuchIce = rbLotsOfIce.Checked;
            mData.DeCaffeinated = !chkCaffeine.Checked;
            if (rbVeganCream.Checked)
            {
                mData.Cream = BrewFrappuccino.CreamType.Vegan;
            }
            else if (rbRegularCream.Checked)
            {
                mData.Cream = BrewFrappuccino.CreamType.Normal;
            }
            else
            {
                mData.Cream = BrewFrappuccino.CreamType.LowFat;
            }
        }
        /// <summary>
        /// Update our data object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            CopyGUItoData();
        }
    }
}
