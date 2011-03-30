namespace PSFDiffVisualizer
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.psfRadiusNumeric = new System.Windows.Forms.NumericUpDown();
            this.antiAliasCheckBox = new System.Windows.Forms.CheckBox();
            this.diffXcheckBox = new System.Windows.Forms.CheckBox();
            this.psfPictureBox = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.scaleFactorNumeric = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.psfRadiusNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.psfPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleFactorNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PSF radius:";
            // 
            // psfRadiusNumeric
            // 
            this.psfRadiusNumeric.Location = new System.Drawing.Point(80, 13);
            this.psfRadiusNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.psfRadiusNumeric.Name = "psfRadiusNumeric";
            this.psfRadiusNumeric.Size = new System.Drawing.Size(63, 20);
            this.psfRadiusNumeric.TabIndex = 1;
            this.psfRadiusNumeric.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.psfRadiusNumeric.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // antiAliasCheckBox
            // 
            this.antiAliasCheckBox.AutoSize = true;
            this.antiAliasCheckBox.Location = new System.Drawing.Point(149, 14);
            this.antiAliasCheckBox.Name = "antiAliasCheckBox";
            this.antiAliasCheckBox.Size = new System.Drawing.Size(68, 17);
            this.antiAliasCheckBox.TabIndex = 4;
            this.antiAliasCheckBox.Text = "Anti-alias";
            this.antiAliasCheckBox.UseVisualStyleBackColor = true;
            this.antiAliasCheckBox.CheckedChanged += new System.EventHandler(this.antiAliasCheckBox_CheckedChanged);
            // 
            // diffXcheckBox
            // 
            this.diffXcheckBox.AutoSize = true;
            this.diffXcheckBox.Location = new System.Drawing.Point(224, 14);
            this.diffXcheckBox.Name = "diffXcheckBox";
            this.diffXcheckBox.Size = new System.Drawing.Size(113, 17);
            this.diffXcheckBox.TabIndex = 5;
            this.diffXcheckBox.Text = "Show X difference";
            this.diffXcheckBox.UseVisualStyleBackColor = true;
            this.diffXcheckBox.CheckedChanged += new System.EventHandler(this.diffXcheckBox_CheckedChanged);
            // 
            // psfPictureBox
            // 
            this.psfPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.psfPictureBox.BackColor = System.Drawing.Color.Black;
            this.psfPictureBox.Location = new System.Drawing.Point(9, 36);
            this.psfPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.psfPictureBox.Name = "psfPictureBox";
            this.psfPictureBox.Size = new System.Drawing.Size(526, 347);
            this.psfPictureBox.TabIndex = 6;
            this.psfPictureBox.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(343, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Scale factor (2^N):";
            // 
            // scaleFactorNumeric
            // 
            this.scaleFactorNumeric.Location = new System.Drawing.Point(445, 13);
            this.scaleFactorNumeric.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.scaleFactorNumeric.Name = "scaleFactorNumeric";
            this.scaleFactorNumeric.Size = new System.Drawing.Size(63, 20);
            this.scaleFactorNumeric.TabIndex = 1;
            this.scaleFactorNumeric.ValueChanged += new System.EventHandler(this.scaleFactorNumeric_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 392);
            this.Controls.Add(this.psfPictureBox);
            this.Controls.Add(this.diffXcheckBox);
            this.Controls.Add(this.antiAliasCheckBox);
            this.Controls.Add(this.scaleFactorNumeric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.psfRadiusNumeric);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "PSF Generator";
            ((System.ComponentModel.ISupportInitialize)(this.psfRadiusNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.psfPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleFactorNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown psfRadiusNumeric;
        private System.Windows.Forms.CheckBox antiAliasCheckBox;
        private System.Windows.Forms.CheckBox diffXcheckBox;
        private System.Windows.Forms.PictureBox psfPictureBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown scaleFactorNumeric;
    }
}

