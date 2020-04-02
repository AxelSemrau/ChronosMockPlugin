namespace MockPlugin.Device
{
    partial class MockDeviceToolbox
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
            this.btnShowMessage = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblRunning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnShowMessage
            // 
            this.btnShowMessage.Location = new System.Drawing.Point(4, 30);
            this.btnShowMessage.Name = "btnShowMessage";
            this.btnShowMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnShowMessage.Size = new System.Drawing.Size(138, 23);
            this.btnShowMessage.TabIndex = 0;
            this.btnShowMessage.Text = "Show message";
            this.btnShowMessage.UseVisualStyleBackColor = true;
            this.btnShowMessage.Click += new System.EventHandler(this.btnShowMessage_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(138, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Just some text";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(4, 60);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(78, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Connected";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // lblRunning
            // 
            this.lblRunning.AutoSize = true;
            this.lblRunning.Location = new System.Drawing.Point(4, 84);
            this.lblRunning.Name = "lblRunning";
            this.lblRunning.Size = new System.Drawing.Size(76, 13);
            this.lblRunning.TabIndex = 3;
            this.lblRunning.Text = "(set at runtime)";
            // 
            // MockDeviceToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblRunning);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnShowMessage);
            this.Name = "MockDeviceToolbox";
            this.Size = new System.Drawing.Size(349, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnShowMessage;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblRunning;
    }
}
