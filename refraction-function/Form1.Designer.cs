namespace refraction_function
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.outerMediumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.innerMediumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outerMediumNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.innerMediumNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 370);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.outerMediumNumericUpDown);
            this.groupBox1.Controls.Add(this.innerMediumNumericUpDown);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(40, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 370);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // outerMediumNumericUpDown
            // 
            this.outerMediumNumericUpDown.DecimalPlaces = 5;
            this.outerMediumNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.outerMediumNumericUpDown.Location = new System.Drawing.Point(85, 58);
            this.outerMediumNumericUpDown.Name = "outerMediumNumericUpDown";
            this.outerMediumNumericUpDown.Size = new System.Drawing.Size(104, 20);
            this.outerMediumNumericUpDown.TabIndex = 3;
            this.outerMediumNumericUpDown.ValueChanged += new System.EventHandler(this.outerMediumNumericUpDown_ValueChanged);
            // 
            // innerMediumNumericUpDown
            // 
            this.innerMediumNumericUpDown.DecimalPlaces = 5;
            this.innerMediumNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.innerMediumNumericUpDown.Location = new System.Drawing.Point(85, 32);
            this.innerMediumNumericUpDown.Name = "innerMediumNumericUpDown";
            this.innerMediumNumericUpDown.Size = new System.Drawing.Size(104, 20);
            this.innerMediumNumericUpDown.TabIndex = 3;
            this.innerMediumNumericUpDown.ValueChanged += new System.EventHandler(this.innerMediumNumericUpDown_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Outer medium:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Inner medium:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Indices of refraction:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 394);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outerMediumNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.innerMediumNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown outerMediumNumericUpDown;
        private System.Windows.Forms.NumericUpDown innerMediumNumericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

