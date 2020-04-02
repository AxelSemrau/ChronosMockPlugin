namespace MockPlugin.Misc
{
    partial class StatusViewControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SimpleLabel = new System.Windows.Forms.Label();
            this.mUpdateStatusTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // SimpleLabel
            // 
            this.SimpleLabel.AutoSize = true;
            this.SimpleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SimpleLabel.Location = new System.Drawing.Point(0, 0);
            this.SimpleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SimpleLabel.Name = "SimpleLabel";
            this.SimpleLabel.Padding = new System.Windows.Forms.Padding(4);
            this.SimpleLabel.Size = new System.Drawing.Size(137, 42);
            this.SimpleLabel.TabIndex = 0;
            this.SimpleLabel.Text = "Here\'s some status\r\ntext for our device";
            // 
            // mUpdateStatusTimer
            // 
            this.mUpdateStatusTimer.Enabled = true;
            this.mUpdateStatusTimer.Interval = 1000;
            this.mUpdateStatusTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // StatusViewControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.SimpleLabel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StatusViewControl";
            this.Size = new System.Drawing.Size(137, 42);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SimpleLabel;
        private System.Windows.Forms.Timer mUpdateStatusTimer;
    }
}
