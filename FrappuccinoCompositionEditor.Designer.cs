namespace MockPlugin
{
    partial class FrappuccinoCompositionEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrappuccinoCompositionEditor));
            this.propertyGrid3 = new System.Windows.Forms.PropertyGrid();
            this.rbLowFat = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbVeganCream = new System.Windows.Forms.RadioButton();
            this.rbRegularCream = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbLotsOfIce = new System.Windows.Forms.RadioButton();
            this.rbLowIce = new System.Windows.Forms.RadioButton();
            this.chkCaffeine = new System.Windows.Forms.CheckBox();
            this.cboSize = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid3
            // 
            resources.ApplyResources(this.propertyGrid3, "propertyGrid3");
            this.propertyGrid3.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid3.Name = "propertyGrid3";
            // 
            // rbLowFat
            // 
            resources.ApplyResources(this.rbLowFat, "rbLowFat");
            this.rbLowFat.Checked = true;
            this.rbLowFat.Name = "rbLowFat";
            this.rbLowFat.TabStop = true;
            this.rbLowFat.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.rbVeganCream);
            this.groupBox1.Controls.Add(this.rbRegularCream);
            this.groupBox1.Controls.Add(this.rbLowFat);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rbVeganCream
            // 
            resources.ApplyResources(this.rbVeganCream, "rbVeganCream");
            this.rbVeganCream.Name = "rbVeganCream";
            this.rbVeganCream.UseVisualStyleBackColor = true;
            // 
            // rbRegularCream
            // 
            resources.ApplyResources(this.rbRegularCream, "rbRegularCream");
            this.rbRegularCream.Name = "rbRegularCream";
            this.rbRegularCream.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.rbLotsOfIce);
            this.groupBox2.Controls.Add(this.rbLowIce);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // rbLotsOfIce
            // 
            resources.ApplyResources(this.rbLotsOfIce, "rbLotsOfIce");
            this.rbLotsOfIce.Name = "rbLotsOfIce";
            this.rbLotsOfIce.UseVisualStyleBackColor = true;
            // 
            // rbLowIce
            // 
            resources.ApplyResources(this.rbLowIce, "rbLowIce");
            this.rbLowIce.Name = "rbLowIce";
            this.rbLowIce.UseVisualStyleBackColor = true;
            // 
            // chkCaffeine
            // 
            resources.ApplyResources(this.chkCaffeine, "chkCaffeine");
            this.chkCaffeine.Name = "chkCaffeine";
            this.chkCaffeine.UseVisualStyleBackColor = true;
            // 
            // cboSize
            // 
            resources.ApplyResources(this.cboSize, "cboSize");
            this.cboSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSize.FormattingEnabled = true;
            this.cboSize.Items.AddRange(new object[] {
            resources.GetString("cboSize.Items"),
            resources.GetString("cboSize.Items1"),
            resources.GetString("cboSize.Items2")});
            this.cboSize.Name = "cboSize";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FrappuccinoCompositionEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboSize);
            this.Controls.Add(this.chkCaffeine);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.propertyGrid3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrappuccinoCompositionEditor";
            this.Shown += new System.EventHandler(this.FrappuccinoCompositionEditor_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid3;
        private System.Windows.Forms.RadioButton rbLowFat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbVeganCream;
        private System.Windows.Forms.RadioButton rbRegularCream;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbLotsOfIce;
        private System.Windows.Forms.RadioButton rbLowIce;
        private System.Windows.Forms.CheckBox chkCaffeine;
        private System.Windows.Forms.ComboBox cboSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

    }
}