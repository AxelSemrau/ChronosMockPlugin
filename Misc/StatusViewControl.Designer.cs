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
            this.SimpleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SimpleLabel
            // 
            this.SimpleLabel.AutoSize = true;
            this.SimpleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SimpleLabel.Location = new System.Drawing.Point(0, 0);
            this.SimpleLabel.Name = "SimpleLabel";
            this.SimpleLabel.Padding = new System.Windows.Forms.Padding(3);
            this.SimpleLabel.Size = new System.Drawing.Size(102, 32);
            this.SimpleLabel.TabIndex = 0;
            this.SimpleLabel.Text = "Here\'s some status\r\ntext for our device";
            // 
            // StatusViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.SimpleLabel);
            this.Name = "StatusViewControl";
            this.Size = new System.Drawing.Size(105, 32);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SimpleLabel;
    }
}
