namespace HeartRate
{
    partial class HeartRateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelHR = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelHR
            // 
            this.labelHR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHR.Font = new System.Drawing.Font("Microsoft Sans Serif", 70F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHR.Location = new System.Drawing.Point(2, 13);
            this.labelHR.Name = "labelHR";
            this.labelHR.Size = new System.Drawing.Size(397, 116);
            this.labelHR.TabIndex = 0;
            this.labelHR.Text = "9999";
            this.labelHR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HeartRateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 138);
            this.Controls.Add(this.labelHR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "HeartRateForm";
            this.Text = "Heart rate monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HeartRateForm_FormClosing);
            this.Load += new System.EventHandler(this.HeartRateForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelHR;
    }
}

