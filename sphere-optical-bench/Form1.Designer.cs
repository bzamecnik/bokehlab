namespace SphericLensGUI
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
            this.drawingPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sphericalCapConvexCheckBox = new System.Windows.Forms.CheckBox();
            this.rayDirectionPhiNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.rayOriginPhiNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.rayOriginRadiusNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.sphericalCapApertureNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.sphericalCapRadiusNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginPhiNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginRadiusNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphericalCapApertureNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphericalCapRadiusNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // drawingPanel
            // 
            this.drawingPanel.Location = new System.Drawing.Point(12, 12);
            this.drawingPanel.Name = "drawingPanel";
            this.drawingPanel.Size = new System.Drawing.Size(580, 422);
            this.drawingPanel.TabIndex = 0;
            this.drawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawingPanel_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sphericalCapConvexCheckBox);
            this.groupBox1.Controls.Add(this.rayDirectionPhiNumericUpDown);
            this.groupBox1.Controls.Add(this.rayOriginPhiNumericUpDown);
            this.groupBox1.Controls.Add(this.rayOriginRadiusNumericUpDown);
            this.groupBox1.Controls.Add(this.sphericalCapApertureNumericUpDown);
            this.groupBox1.Controls.Add(this.sphericalCapRadiusNumericUpDown);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(598, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 422);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // sphericalCapConvexCheckBox
            // 
            this.sphericalCapConvexCheckBox.AutoSize = true;
            this.sphericalCapConvexCheckBox.Location = new System.Drawing.Point(97, 95);
            this.sphericalCapConvexCheckBox.Name = "sphericalCapConvexCheckBox";
            this.sphericalCapConvexCheckBox.Size = new System.Drawing.Size(15, 14);
            this.sphericalCapConvexCheckBox.TabIndex = 3;
            this.sphericalCapConvexCheckBox.UseVisualStyleBackColor = true;
            this.sphericalCapConvexCheckBox.CheckedChanged += new System.EventHandler(this.sphericalCapConvexCheckBox_CheckedChanged);
            // 
            // rayDirectionPhiNumericUpDown
            // 
            this.rayDirectionPhiNumericUpDown.DecimalPlaces = 2;
            this.rayDirectionPhiNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayDirectionPhiNumericUpDown.Location = new System.Drawing.Point(97, 196);
            this.rayDirectionPhiNumericUpDown.Maximum = new decimal(new int[] {
            628318,
            0,
            0,
            327680});
            this.rayDirectionPhiNumericUpDown.Name = "rayDirectionPhiNumericUpDown";
            this.rayDirectionPhiNumericUpDown.Size = new System.Drawing.Size(89, 20);
            this.rayDirectionPhiNumericUpDown.TabIndex = 2;
            this.rayDirectionPhiNumericUpDown.ValueChanged += new System.EventHandler(this.rayDirectionPhiNumericUpDown_ValueChanged);
            // 
            // rayOriginPhiNumericUpDown
            // 
            this.rayOriginPhiNumericUpDown.DecimalPlaces = 2;
            this.rayOriginPhiNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayOriginPhiNumericUpDown.Location = new System.Drawing.Point(97, 157);
            this.rayOriginPhiNumericUpDown.Maximum = new decimal(new int[] {
            628318,
            0,
            0,
            327680});
            this.rayOriginPhiNumericUpDown.Name = "rayOriginPhiNumericUpDown";
            this.rayOriginPhiNumericUpDown.Size = new System.Drawing.Size(89, 20);
            this.rayOriginPhiNumericUpDown.TabIndex = 2;
            this.rayOriginPhiNumericUpDown.ValueChanged += new System.EventHandler(this.rayOriginPhiNumericUpDown_ValueChanged);
            // 
            // rayOriginRadiusNumericUpDown
            // 
            this.rayOriginRadiusNumericUpDown.DecimalPlaces = 1;
            this.rayOriginRadiusNumericUpDown.Location = new System.Drawing.Point(97, 131);
            this.rayOriginRadiusNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rayOriginRadiusNumericUpDown.Name = "rayOriginRadiusNumericUpDown";
            this.rayOriginRadiusNumericUpDown.Size = new System.Drawing.Size(89, 20);
            this.rayOriginRadiusNumericUpDown.TabIndex = 2;
            this.rayOriginRadiusNumericUpDown.ValueChanged += new System.EventHandler(this.rayOriginRadiusNumericUpDown_ValueChanged);
            // 
            // sphericalCapApertureNumericUpDown
            // 
            this.sphericalCapApertureNumericUpDown.DecimalPlaces = 1;
            this.sphericalCapApertureNumericUpDown.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.sphericalCapApertureNumericUpDown.Location = new System.Drawing.Point(97, 67);
            this.sphericalCapApertureNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.sphericalCapApertureNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.sphericalCapApertureNumericUpDown.Name = "sphericalCapApertureNumericUpDown";
            this.sphericalCapApertureNumericUpDown.Size = new System.Drawing.Size(89, 20);
            this.sphericalCapApertureNumericUpDown.TabIndex = 2;
            this.sphericalCapApertureNumericUpDown.ValueChanged += new System.EventHandler(this.sphericalCapApertureNumericUpDown_ValueChanged);
            // 
            // sphericalCapRadiusNumericUpDown
            // 
            this.sphericalCapRadiusNumericUpDown.DecimalPlaces = 1;
            this.sphericalCapRadiusNumericUpDown.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.sphericalCapRadiusNumericUpDown.Location = new System.Drawing.Point(97, 41);
            this.sphericalCapRadiusNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.sphericalCapRadiusNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.sphericalCapRadiusNumericUpDown.Name = "sphericalCapRadiusNumericUpDown";
            this.sphericalCapRadiusNumericUpDown.Size = new System.Drawing.Size(89, 20);
            this.sphericalCapRadiusNumericUpDown.TabIndex = 2;
            this.sphericalCapRadiusNumericUpDown.ValueChanged += new System.EventHandler(this.sphericalCapRadiusNumericUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Direction Phi:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Convex:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Origin Phi:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Aperture:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Origin R:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Radius:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(8, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Ray:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "SphericalCap:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 446);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.drawingPanel);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(620, 200);
            this.Name = "Form1";
            this.Text = "Lens";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginPhiNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginRadiusNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphericalCapApertureNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphericalCapRadiusNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel drawingPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown rayDirectionPhiNumericUpDown;
        private System.Windows.Forms.NumericUpDown rayOriginRadiusNumericUpDown;
        private System.Windows.Forms.NumericUpDown sphericalCapRadiusNumericUpDown;
        private System.Windows.Forms.NumericUpDown rayOriginPhiNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox sphericalCapConvexCheckBox;
        private System.Windows.Forms.NumericUpDown sphericalCapApertureNumericUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;

    }
}
