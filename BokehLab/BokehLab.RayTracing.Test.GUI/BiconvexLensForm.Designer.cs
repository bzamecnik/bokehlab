namespace BokehLab.RayTracing.Test.GUI
{
    partial class BiconvexLensForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.rayOriginZNumeric = new System.Windows.Forms.NumericUpDown();
            this.apertureRadiusNumeric = new System.Windows.Forms.NumericUpDown();
            this.curvatureRadiusNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayOriginYNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayDirectionPhiNumeric = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rayOriginXNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.apertureRadiusNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.curvatureRadiusNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginXNumeric)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // drawingPanel
            // 
            this.drawingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.drawingPanel.Location = new System.Drawing.Point(3, 3);
            this.drawingPanel.Name = "drawingPanel";
            this.drawingPanel.Size = new System.Drawing.Size(559, 415);
            this.drawingPanel.TabIndex = 0;
            this.drawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawingPanel_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.rayOriginZNumeric);
            this.groupBox1.Controls.Add(this.apertureRadiusNumeric);
            this.groupBox1.Controls.Add(this.curvatureRadiusNumeric);
            this.groupBox1.Controls.Add(this.rayOriginYNumeric);
            this.groupBox1.Controls.Add(this.rayDirectionPhiNumeric);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rayOriginXNumeric);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 415);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Recompute!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rayOriginZNumeric
            // 
            this.rayOriginZNumeric.Location = new System.Drawing.Point(122, 125);
            this.rayOriginZNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rayOriginZNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.rayOriginZNumeric.Name = "rayOriginZNumeric";
            this.rayOriginZNumeric.Size = new System.Drawing.Size(49, 20);
            this.rayOriginZNumeric.TabIndex = 7;
            this.rayOriginZNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // apertureRadiusNumeric
            // 
            this.apertureRadiusNumeric.DecimalPlaces = 1;
            this.apertureRadiusNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.apertureRadiusNumeric.Location = new System.Drawing.Point(117, 65);
            this.apertureRadiusNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.apertureRadiusNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.apertureRadiusNumeric.Name = "apertureRadiusNumeric";
            this.apertureRadiusNumeric.Size = new System.Drawing.Size(77, 20);
            this.apertureRadiusNumeric.TabIndex = 1;
            this.apertureRadiusNumeric.ValueChanged += new System.EventHandler(this.apertureRadiusNumeric_ValueChanged);
            // 
            // curvatureRadiusNumeric
            // 
            this.curvatureRadiusNumeric.DecimalPlaces = 1;
            this.curvatureRadiusNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.curvatureRadiusNumeric.Location = new System.Drawing.Point(117, 41);
            this.curvatureRadiusNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.curvatureRadiusNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.curvatureRadiusNumeric.Name = "curvatureRadiusNumeric";
            this.curvatureRadiusNumeric.Size = new System.Drawing.Size(77, 20);
            this.curvatureRadiusNumeric.TabIndex = 1;
            this.curvatureRadiusNumeric.ValueChanged += new System.EventHandler(this.curvatureRadiusNumeric_ValueChanged);
            // 
            // rayOriginYNumeric
            // 
            this.rayOriginYNumeric.Location = new System.Drawing.Point(71, 125);
            this.rayOriginYNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rayOriginYNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.rayOriginYNumeric.Name = "rayOriginYNumeric";
            this.rayOriginYNumeric.Size = new System.Drawing.Size(45, 20);
            this.rayOriginYNumeric.TabIndex = 6;
            this.rayOriginYNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // rayDirectionPhiNumeric
            // 
            this.rayDirectionPhiNumeric.DecimalPlaces = 3;
            this.rayDirectionPhiNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayDirectionPhiNumeric.Location = new System.Drawing.Point(11, 167);
            this.rayDirectionPhiNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.rayDirectionPhiNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.rayDirectionPhiNumeric.Name = "rayDirectionPhiNumeric";
            this.rayDirectionPhiNumeric.Size = new System.Drawing.Size(54, 20);
            this.rayDirectionPhiNumeric.TabIndex = 8;
            this.rayDirectionPhiNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Aperture radius:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Radius of curvature:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Direction (phi):";
            // 
            // rayOriginXNumeric
            // 
            this.rayOriginXNumeric.Location = new System.Drawing.Point(11, 125);
            this.rayOriginXNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rayOriginXNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.rayOriginXNumeric.Name = "rayOriginXNumeric";
            this.rayOriginXNumeric.Size = new System.Drawing.Size(54, 20);
            this.rayOriginXNumeric.TabIndex = 5;
            this.rayOriginXNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 109);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Origin (X, Y, Z):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(8, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Incoming ray:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Lens:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(13, 13);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.drawingPanel);
            this.splitContainer1.Size = new System.Drawing.Size(780, 421);
            this.splitContainer1.SplitterDistance = 211;
            this.splitContainer1.TabIndex = 2;
            // 
            // BiconvexLensForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 446);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(620, 200);
            this.Name = "BiconvexLensForm";
            this.Text = "Biconvex lens";
            this.Resize += new System.EventHandler(this.BiconvexLensForm_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.apertureRadiusNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.curvatureRadiusNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginXNumeric)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel drawingPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown curvatureRadiusNumeric;
        private System.Windows.Forms.NumericUpDown rayOriginZNumeric;
        private System.Windows.Forms.NumericUpDown rayOriginYNumeric;
        private System.Windows.Forms.NumericUpDown rayOriginXNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown rayDirectionPhiNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown apertureRadiusNumeric;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;

    }
}
