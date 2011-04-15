namespace LightTracing
{
    partial class LightTracingForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sampleCountNumeric = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.lightIntensityNumeric = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.lensApertureNumeric = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.lensFocalLengthNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.senzorCenterZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.lightZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.lightYNumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lightXNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightIntensityNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lensApertureNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lensFocalLengthNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorCenterZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightXNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(548, 555);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sampleCountNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.lightIntensityNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.lensApertureNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.lensFocalLengthNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.senzorCenterZNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.lightZNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.lightYNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.lightXNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(755, 561);
            this.splitContainer1.SplitterDistance = 197;
            this.splitContainer1.TabIndex = 1;
            // 
            // sampleCountNumeric
            // 
            this.sampleCountNumeric.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.sampleCountNumeric.Location = new System.Drawing.Point(109, 185);
            this.sampleCountNumeric.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.sampleCountNumeric.Name = "sampleCountNumeric";
            this.sampleCountNumeric.Size = new System.Drawing.Size(85, 20);
            this.sampleCountNumeric.TabIndex = 8;
            this.sampleCountNumeric.ValueChanged += new System.EventHandler(this.sampleCountNumeric_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 187);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Number of samples:";
            // 
            // lightIntensityNumeric
            // 
            this.lightIntensityNumeric.DecimalPlaces = 3;
            this.lightIntensityNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.lightIntensityNumeric.Location = new System.Drawing.Point(83, 159);
            this.lightIntensityNumeric.Name = "lightIntensityNumeric";
            this.lightIntensityNumeric.Size = new System.Drawing.Size(111, 20);
            this.lightIntensityNumeric.TabIndex = 7;
            this.lightIntensityNumeric.ValueChanged += new System.EventHandler(this.lightIntensityNumeric_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 161);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Light intensity:";
            // 
            // lensApertureNumeric
            // 
            this.lensApertureNumeric.DecimalPlaces = 3;
            this.lensApertureNumeric.Location = new System.Drawing.Point(90, 133);
            this.lensApertureNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lensApertureNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.lensApertureNumeric.Name = "lensApertureNumeric";
            this.lensApertureNumeric.Size = new System.Drawing.Size(104, 20);
            this.lensApertureNumeric.TabIndex = 6;
            this.lensApertureNumeric.ValueChanged += new System.EventHandler(this.lensApertureNumeric_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Aperture radius:";
            // 
            // lensFocalLengthNumeric
            // 
            this.lensFocalLengthNumeric.DecimalPlaces = 3;
            this.lensFocalLengthNumeric.Location = new System.Drawing.Point(77, 107);
            this.lensFocalLengthNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lensFocalLengthNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.lensFocalLengthNumeric.Name = "lensFocalLengthNumeric";
            this.lensFocalLengthNumeric.Size = new System.Drawing.Size(117, 20);
            this.lensFocalLengthNumeric.TabIndex = 5;
            this.lensFocalLengthNumeric.ValueChanged += new System.EventHandler(this.lensFocalLengthNumeric_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Focal length:";
            // 
            // senzorCenterZNumeric
            // 
            this.senzorCenterZNumeric.DecimalPlaces = 3;
            this.senzorCenterZNumeric.Location = new System.Drawing.Point(95, 81);
            this.senzorCenterZNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.senzorCenterZNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.senzorCenterZNumeric.Name = "senzorCenterZNumeric";
            this.senzorCenterZNumeric.Size = new System.Drawing.Size(99, 20);
            this.senzorCenterZNumeric.TabIndex = 4;
            this.senzorCenterZNumeric.ValueChanged += new System.EventHandler(this.senzorCenterZNumeric_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Senzor center Z:";
            // 
            // lightZNumeric
            // 
            this.lightZNumeric.DecimalPlaces = 3;
            this.lightZNumeric.Location = new System.Drawing.Point(52, 55);
            this.lightZNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lightZNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.lightZNumeric.Name = "lightZNumeric";
            this.lightZNumeric.Size = new System.Drawing.Size(142, 20);
            this.lightZNumeric.TabIndex = 3;
            this.lightZNumeric.ValueChanged += new System.EventHandler(this.lightZNumeric_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Light Z:";
            // 
            // lightYNumeric
            // 
            this.lightYNumeric.DecimalPlaces = 3;
            this.lightYNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.lightYNumeric.Location = new System.Drawing.Point(52, 29);
            this.lightYNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lightYNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.lightYNumeric.Name = "lightYNumeric";
            this.lightYNumeric.Size = new System.Drawing.Size(142, 20);
            this.lightYNumeric.TabIndex = 2;
            this.lightYNumeric.ValueChanged += new System.EventHandler(this.lightYNumeric_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Light Y:";
            // 
            // lightXNumeric
            // 
            this.lightXNumeric.DecimalPlaces = 3;
            this.lightXNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.lightXNumeric.Location = new System.Drawing.Point(52, 3);
            this.lightXNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lightXNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.lightXNumeric.Name = "lightXNumeric";
            this.lightXNumeric.Size = new System.Drawing.Size(142, 20);
            this.lightXNumeric.TabIndex = 1;
            this.lightXNumeric.ValueChanged += new System.EventHandler(this.lightXNumeric_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Light X:";
            // 
            // LightTracingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 585);
            this.Controls.Add(this.splitContainer1);
            this.Name = "LightTracingForm";
            this.Text = "BokehLab - PSF light tracer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightIntensityNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lensApertureNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lensFocalLengthNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorCenterZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightXNumeric)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NumericUpDown lensApertureNumeric;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown lensFocalLengthNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown senzorCenterZNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown lightZNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown lightYNumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown lightXNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown lightIntensityNumeric;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown sampleCountNumeric;
        private System.Windows.Forms.Label label8;
    }
}

