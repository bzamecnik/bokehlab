namespace BokehLab.RayTracing.Test.GUI
{
    partial class HeighFieldForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllLayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.layerCountLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rayEndZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.intersectionLabel = new System.Windows.Forms.Label();
            this.rayEndXYZLabel = new System.Windows.Forms.Label();
            this.rayStartXYZLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rayStartZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.heightFieldPanel = new System.Windows.Forms.Panel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartZNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(685, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem,
            this.clearAllLayersToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // addLayerToolStripMenuItem
            // 
            this.addLayerToolStripMenuItem.Name = "addLayerToolStripMenuItem";
            this.addLayerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.addLayerToolStripMenuItem.Text = "&Add height-field layer";
            this.addLayerToolStripMenuItem.Click += new System.EventHandler(this.addLayerToolStripMenuItem_Click);
            // 
            // clearAllLayersToolStripMenuItem
            // 
            this.clearAllLayersToolStripMenuItem.Name = "clearAllLayersToolStripMenuItem";
            this.clearAllLayersToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.clearAllLayersToolStripMenuItem.Text = "&Clear all layers";
            this.clearAllLayersToolStripMenuItem.Click += new System.EventHandler(this.clearAllLayersToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.layerCountLabel);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.rayEndZNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.intersectionLabel);
            this.splitContainer1.Panel1.Controls.Add(this.rayEndXYZLabel);
            this.splitContainer1.Panel1.Controls.Add(this.rayStartXYZLabel);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.rayStartZNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.heightFieldPanel);
            this.splitContainer1.Size = new System.Drawing.Size(685, 400);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 1;
            // 
            // layerCountLabel
            // 
            this.layerCountLabel.AutoSize = true;
            this.layerCountLabel.Location = new System.Drawing.Point(105, 12);
            this.layerCountLabel.Name = "layerCountLabel";
            this.layerCountLabel.Size = new System.Drawing.Size(0, 13);
            this.layerCountLabel.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Number of layers:";
            // 
            // rayEndZNumeric
            // 
            this.rayEndZNumeric.DecimalPlaces = 2;
            this.rayEndZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayEndZNumeric.Location = new System.Drawing.Point(108, 82);
            this.rayEndZNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rayEndZNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.rayEndZNumeric.Name = "rayEndZNumeric";
            this.rayEndZNumeric.Size = new System.Drawing.Size(64, 20);
            this.rayEndZNumeric.TabIndex = 1;
            this.rayEndZNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rayEndZNumeric.ValueChanged += new System.EventHandler(this.rayEndZNumeric_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Ray end Z:";
            // 
            // intersectionLabel
            // 
            this.intersectionLabel.AutoSize = true;
            this.intersectionLabel.Location = new System.Drawing.Point(105, 129);
            this.intersectionLabel.Name = "intersectionLabel";
            this.intersectionLabel.Size = new System.Drawing.Size(0, 13);
            this.intersectionLabel.TabIndex = 0;
            // 
            // rayEndXYZLabel
            // 
            this.rayEndXYZLabel.AutoSize = true;
            this.rayEndXYZLabel.Location = new System.Drawing.Point(105, 105);
            this.rayEndXYZLabel.Name = "rayEndXYZLabel";
            this.rayEndXYZLabel.Size = new System.Drawing.Size(0, 13);
            this.rayEndXYZLabel.TabIndex = 0;
            // 
            // rayStartXYZLabel
            // 
            this.rayStartXYZLabel.AutoSize = true;
            this.rayStartXYZLabel.Location = new System.Drawing.Point(105, 60);
            this.rayStartXYZLabel.Name = "rayStartXYZLabel";
            this.rayStartXYZLabel.Size = new System.Drawing.Size(0, 13);
            this.rayStartXYZLabel.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Intersection:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Ray end X, Y, Z:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ray start X, Y, Z:";
            // 
            // rayStartZNumeric
            // 
            this.rayStartZNumeric.DecimalPlaces = 2;
            this.rayStartZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayStartZNumeric.Location = new System.Drawing.Point(108, 32);
            this.rayStartZNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rayStartZNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.rayStartZNumeric.Name = "rayStartZNumeric";
            this.rayStartZNumeric.Size = new System.Drawing.Size(64, 20);
            this.rayStartZNumeric.TabIndex = 1;
            this.rayStartZNumeric.ValueChanged += new System.EventHandler(this.rayStartZNumeric_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ray start Z:";
            // 
            // heightFieldPanel
            // 
            this.heightFieldPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.heightFieldPanel.Location = new System.Drawing.Point(3, 3);
            this.heightFieldPanel.Name = "heightFieldPanel";
            this.heightFieldPanel.Size = new System.Drawing.Size(473, 392);
            this.heightFieldPanel.TabIndex = 0;
            this.heightFieldPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.heightFieldPanel_Paint);
            this.heightFieldPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.heightFieldPanel_MouseMove);
            this.heightFieldPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.heightFieldPanel_MouseDown);
            this.heightFieldPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.heightFieldPanel_MouseUp);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "PNG files|*.png|All files|*.*";
            this.openFileDialog.Multiselect = true;
            // 
            // HeighFieldForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 424);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "HeighFieldForm";
            this.Text = "BokehLab - ray height-field intersection";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rayEndZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartZNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLayerToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.NumericUpDown rayEndZNumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown rayStartZNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label rayEndXYZLabel;
        private System.Windows.Forms.Label rayStartXYZLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel heightFieldPanel;
        private System.Windows.Forms.Label intersectionLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripMenuItem clearAllLayersToolStripMenuItem;
        private System.Windows.Forms.Label layerCountLabel;
        private System.Windows.Forms.Label label6;
    }
}