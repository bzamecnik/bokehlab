namespace BokehLab.RayTracing.Test.GUI
{
    partial class SphereIntersectionForm
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
            this.sphereCenterZNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayOriginZNumeric = new System.Windows.Forms.NumericUpDown();
            this.sphereCenterYNumeric = new System.Windows.Forms.NumericUpDown();
            this.sphereRadiusNumeric = new System.Windows.Forms.NumericUpDown();
            this.sphereCenterXNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayOriginYNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayDirectionPhiNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rayOriginXNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sphereCenterZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphereCenterYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphereRadiusNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphereCenterXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginXNumeric)).BeginInit();
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
            this.groupBox1.Controls.Add(this.sphereCenterZNumeric);
            this.groupBox1.Controls.Add(this.rayOriginZNumeric);
            this.groupBox1.Controls.Add(this.sphereCenterYNumeric);
            this.groupBox1.Controls.Add(this.sphereRadiusNumeric);
            this.groupBox1.Controls.Add(this.sphereCenterXNumeric);
            this.groupBox1.Controls.Add(this.rayOriginYNumeric);
            this.groupBox1.Controls.Add(this.rayDirectionPhiNumeric);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rayOriginXNumeric);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(598, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 422);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // sphereCenterZNumeric
            // 
            this.sphereCenterZNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sphereCenterZNumeric.Location = new System.Drawing.Point(122, 84);
            this.sphereCenterZNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.sphereCenterZNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.sphereCenterZNumeric.Name = "sphereCenterZNumeric";
            this.sphereCenterZNumeric.Size = new System.Drawing.Size(49, 20);
            this.sphereCenterZNumeric.TabIndex = 4;
            this.sphereCenterZNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // rayOriginZNumeric
            // 
            this.rayOriginZNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.rayOriginZNumeric.Location = new System.Drawing.Point(122, 148);
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
            // sphereCenterYNumeric
            // 
            this.sphereCenterYNumeric.Enabled = false;
            this.sphereCenterYNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sphereCenterYNumeric.Location = new System.Drawing.Point(71, 84);
            this.sphereCenterYNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.sphereCenterYNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.sphereCenterYNumeric.Name = "sphereCenterYNumeric";
            this.sphereCenterYNumeric.Size = new System.Drawing.Size(45, 20);
            this.sphereCenterYNumeric.TabIndex = 3;
            this.sphereCenterYNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // sphereRadiusNumeric
            // 
            this.sphereRadiusNumeric.DecimalPlaces = 1;
            this.sphereRadiusNumeric.Enabled = false;
            this.sphereRadiusNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.sphereRadiusNumeric.Location = new System.Drawing.Point(82, 41);
            this.sphereRadiusNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.sphereRadiusNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.sphereRadiusNumeric.Name = "sphereRadiusNumeric";
            this.sphereRadiusNumeric.Size = new System.Drawing.Size(89, 20);
            this.sphereRadiusNumeric.TabIndex = 1;
            // 
            // sphereCenterXNumeric
            // 
            this.sphereCenterXNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sphereCenterXNumeric.Location = new System.Drawing.Point(11, 84);
            this.sphereCenterXNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.sphereCenterXNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.sphereCenterXNumeric.Name = "sphereCenterXNumeric";
            this.sphereCenterXNumeric.Size = new System.Drawing.Size(54, 20);
            this.sphereCenterXNumeric.TabIndex = 2;
            this.sphereCenterXNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // rayOriginYNumeric
            // 
            this.rayOriginYNumeric.Enabled = false;
            this.rayOriginYNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.rayOriginYNumeric.Location = new System.Drawing.Point(71, 148);
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
            5,
            0,
            0,
            131072});
            this.rayDirectionPhiNumeric.Location = new System.Drawing.Point(11, 190);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Center (X, Y, Z):";
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 174);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Direction (phi):";
            // 
            // rayOriginXNumeric
            // 
            this.rayOriginXNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.rayOriginXNumeric.Location = new System.Drawing.Point(11, 148);
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
            this.label10.Location = new System.Drawing.Point(8, 132);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Origin (X, Y, Z):";
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
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sphere:";
            // 
            // SphereIntersectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 446);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.drawingPanel);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(620, 200);
            this.Name = "SphereIntersectionForm";
            this.Text = "Ray-sphere intersection";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sphereCenterZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphereCenterYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphereRadiusNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sphereCenterXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayOriginXNumeric)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel drawingPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown sphereRadiusNumeric;
        private System.Windows.Forms.NumericUpDown rayOriginZNumeric;
        private System.Windows.Forms.NumericUpDown rayOriginYNumeric;
        private System.Windows.Forms.NumericUpDown rayOriginXNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown sphereCenterZNumeric;
        private System.Windows.Forms.NumericUpDown sphereCenterYNumeric;
        private System.Windows.Forms.NumericUpDown sphereCenterXNumeric;
        private System.Windows.Forms.NumericUpDown rayDirectionPhiNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;

    }
}
