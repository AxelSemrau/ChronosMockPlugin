using System;
using System.Windows.Forms;

namespace MockPlugin.Tasks
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
        private void CopyDataToGui()
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
            lblDevName.Text = mData.DevInEditor?.Name ?? "(not set)";
        }
        /// <summary>
        /// If we set the controls' values from the constructor, sometimes they don't get updated properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrappuccinoCompositionEditor_Shown(object sender, EventArgs e)
        {
            CopyDataToGui();
        }

        /// <summary>
        /// When the user clicked on OK, we want to push the state represented by the GUI into our composition object.
        /// </summary>
        private void CopyGuItoData()
        {
            switch (cboSize.SelectedIndex)
            {
                case 0:
                    mData.Volume = 150;
                    break;
                case 1:
                    mData.Volume = 250;
                    break;
                default:
                    mData.Volume = 450;
                    break;
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
            CopyGuItoData();
        }
    }
}
