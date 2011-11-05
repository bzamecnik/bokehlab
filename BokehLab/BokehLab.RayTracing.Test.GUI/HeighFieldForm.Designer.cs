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
            this.intersectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recomputeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.layerCountLabel = new System.Windows.Forms.Label();
            this.epsilonForCloseDepthNumeric = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.rayEndZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.isecLayerLabel = new System.Windows.Forms.Label();
            this.intersectionLabel = new System.Windows.Forms.Label();
            this.rayEndXYZLabel = new System.Windows.Forms.Label();
            this.rayStartXYZLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rayEndYNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayStartYNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayEndXNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayStartXNumeric = new System.Windows.Forms.NumericUpDown();
            this.rayStartZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.heightFieldPanel = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.footprintTraversalPanel = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lightSourceLayerNumeric = new System.Windows.Forms.NumericUpDown();
            this.cocRadiusNumeric = new System.Windows.Forms.NumericUpDown();
            this.cocFootprintRadiusNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cocClippingPanel = new System.Windows.Forms.Panel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.epsilonForCloseDepthNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartZNumeric)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightSourceLayerNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cocRadiusNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cocFootprintRadiusNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.intersectionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(992, 24);
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
            this.addLayerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.addLayerToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.addLayerToolStripMenuItem.Text = "&Add height-field layer";
            this.addLayerToolStripMenuItem.Click += new System.EventHandler(this.addLayerToolStripMenuItem_Click);
            // 
            // clearAllLayersToolStripMenuItem
            // 
            this.clearAllLayersToolStripMenuItem.Name = "clearAllLayersToolStripMenuItem";
            this.clearAllLayersToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.clearAllLayersToolStripMenuItem.Text = "&Clear all layers";
            this.clearAllLayersToolStripMenuItem.Click += new System.EventHandler(this.clearAllLayersToolStripMenuItem_Click);
            // 
            // intersectionToolStripMenuItem
            // 
            this.intersectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recomputeToolStripMenuItem});
            this.intersectionToolStripMenuItem.Name = "intersectionToolStripMenuItem";
            this.intersectionToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.intersectionToolStripMenuItem.Text = "Intersection";
            // 
            // recomputeToolStripMenuItem
            // 
            this.recomputeToolStripMenuItem.Name = "recomputeToolStripMenuItem";
            this.recomputeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.recomputeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.recomputeToolStripMenuItem.Text = "&Recompute";
            this.recomputeToolStripMenuItem.Click += new System.EventHandler(this.recomputeToolStripMenuItem_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.epsilonForCloseDepthNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.rayEndZNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label11);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.isecLayerLabel);
            this.splitContainer1.Panel1.Controls.Add(this.intersectionLabel);
            this.splitContainer1.Panel1.Controls.Add(this.rayEndXYZLabel);
            this.splitContainer1.Panel1.Controls.Add(this.rayStartXYZLabel);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.rayEndYNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.rayStartYNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.rayEndXNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.rayStartXNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.rayStartZNumeric);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(992, 526);
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
            // epsilonForCloseDepthNumeric
            // 
            this.epsilonForCloseDepthNumeric.DecimalPlaces = 4;
            this.epsilonForCloseDepthNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.epsilonForCloseDepthNumeric.Location = new System.Drawing.Point(138, 208);
            this.epsilonForCloseDepthNumeric.Name = "epsilonForCloseDepthNumeric";
            this.epsilonForCloseDepthNumeric.Size = new System.Drawing.Size(50, 20);
            this.epsilonForCloseDepthNumeric.TabIndex = 1;
            this.epsilonForCloseDepthNumeric.ValueChanged += new System.EventHandler(this.epsilonForCloseDepthNumeric_ValueChanged);
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
            this.rayEndZNumeric.Location = new System.Drawing.Point(133, 114);
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
            this.rayEndZNumeric.Size = new System.Drawing.Size(55, 20);
            this.rayEndZNumeric.TabIndex = 6;
            this.rayEndZNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rayEndZNumeric.ValueChanged += new System.EventHandler(this.rayEndZNumeric_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 210);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(121, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Eplison for intersections:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Ray end (X, Y, Z):";
            // 
            // isecLayerLabel
            // 
            this.isecLayerLabel.AutoSize = true;
            this.isecLayerLabel.Location = new System.Drawing.Point(108, 186);
            this.isecLayerLabel.Name = "isecLayerLabel";
            this.isecLayerLabel.Size = new System.Drawing.Size(0, 13);
            this.isecLayerLabel.TabIndex = 0;
            // 
            // intersectionLabel
            // 
            this.intersectionLabel.AutoSize = true;
            this.intersectionLabel.Location = new System.Drawing.Point(105, 164);
            this.intersectionLabel.Name = "intersectionLabel";
            this.intersectionLabel.Size = new System.Drawing.Size(0, 13);
            this.intersectionLabel.TabIndex = 0;
            // 
            // rayEndXYZLabel
            // 
            this.rayEndXYZLabel.AutoSize = true;
            this.rayEndXYZLabel.Location = new System.Drawing.Point(105, 137);
            this.rayEndXYZLabel.Name = "rayEndXYZLabel";
            this.rayEndXYZLabel.Size = new System.Drawing.Size(0, 13);
            this.rayEndXYZLabel.TabIndex = 0;
            // 
            // rayStartXYZLabel
            // 
            this.rayStartXYZLabel.AutoSize = true;
            this.rayStartXYZLabel.Location = new System.Drawing.Point(108, 73);
            this.rayStartXYZLabel.Name = "rayStartXYZLabel";
            this.rayStartXYZLabel.Size = new System.Drawing.Size(0, 13);
            this.rayStartXYZLabel.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Intersection layer:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Intersection:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Ray end X, Y, Z:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ray start:";
            // 
            // rayEndYNumeric
            // 
            this.rayEndYNumeric.DecimalPlaces = 2;
            this.rayEndYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayEndYNumeric.Location = new System.Drawing.Point(72, 114);
            this.rayEndYNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.rayEndYNumeric.Name = "rayEndYNumeric";
            this.rayEndYNumeric.Size = new System.Drawing.Size(55, 20);
            this.rayEndYNumeric.TabIndex = 5;
            this.rayEndYNumeric.ValueChanged += new System.EventHandler(this.rayEndYNumeric_ValueChanged);
            // 
            // rayStartYNumeric
            // 
            this.rayStartYNumeric.DecimalPlaces = 2;
            this.rayStartYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayStartYNumeric.Location = new System.Drawing.Point(72, 50);
            this.rayStartYNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.rayStartYNumeric.Name = "rayStartYNumeric";
            this.rayStartYNumeric.Size = new System.Drawing.Size(55, 20);
            this.rayStartYNumeric.TabIndex = 2;
            this.rayStartYNumeric.ValueChanged += new System.EventHandler(this.rayStartYNumeric_ValueChanged);
            // 
            // rayEndXNumeric
            // 
            this.rayEndXNumeric.DecimalPlaces = 2;
            this.rayEndXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayEndXNumeric.Location = new System.Drawing.Point(11, 114);
            this.rayEndXNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.rayEndXNumeric.Name = "rayEndXNumeric";
            this.rayEndXNumeric.Size = new System.Drawing.Size(55, 20);
            this.rayEndXNumeric.TabIndex = 4;
            this.rayEndXNumeric.ValueChanged += new System.EventHandler(this.rayEndXNumeric_ValueChanged);
            // 
            // rayStartXNumeric
            // 
            this.rayStartXNumeric.DecimalPlaces = 2;
            this.rayStartXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayStartXNumeric.Location = new System.Drawing.Point(11, 50);
            this.rayStartXNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.rayStartXNumeric.Name = "rayStartXNumeric";
            this.rayStartXNumeric.Size = new System.Drawing.Size(55, 20);
            this.rayStartXNumeric.TabIndex = 1;
            this.rayStartXNumeric.ValueChanged += new System.EventHandler(this.rayStartXNumeric_ValueChanged);
            // 
            // rayStartZNumeric
            // 
            this.rayStartZNumeric.DecimalPlaces = 2;
            this.rayStartZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rayStartZNumeric.Location = new System.Drawing.Point(133, 50);
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
            this.rayStartZNumeric.Size = new System.Drawing.Size(55, 20);
            this.rayStartZNumeric.TabIndex = 3;
            this.rayStartZNumeric.ValueChanged += new System.EventHandler(this.rayStartZNumeric_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ray start(X,Y,Z):";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(780, 518);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.heightFieldPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(772, 492);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Height-field";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // heightFieldPanel
            // 
            this.heightFieldPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.heightFieldPanel.Location = new System.Drawing.Point(3, 3);
            this.heightFieldPanel.Name = "heightFieldPanel";
            this.heightFieldPanel.Size = new System.Drawing.Size(763, 483);
            this.heightFieldPanel.TabIndex = 0;
            this.heightFieldPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.heightFieldPanel_Paint);
            this.heightFieldPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.heightFieldPanel_MouseMove);
            this.heightFieldPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.heightFieldPanel_MouseDown);
            this.heightFieldPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.heightFieldPanel_MouseUp);
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.footprintTraversalPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(772, 492);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Line footprint traversal";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // footprintTraversalPanel
            // 
            this.footprintTraversalPanel.Location = new System.Drawing.Point(6, 6);
            this.footprintTraversalPanel.Name = "footprintTraversalPanel";
            this.footprintTraversalPanel.Size = new System.Drawing.Size(760, 483);
            this.footprintTraversalPanel.TabIndex = 0;
            this.footprintTraversalPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.footprintTraversalPanel_Paint);
            this.footprintTraversalPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.footprintTraversalPanel_MouseMove);
            this.footprintTraversalPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.footprintTraversalPanel_MouseDown);
            this.footprintTraversalPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.footprintTraversalPanel_MouseUp);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(772, 492);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "CoC clipping";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lightSourceLayerNumeric);
            this.splitContainer2.Panel1.Controls.Add(this.cocRadiusNumeric);
            this.splitContainer2.Panel1.Controls.Add(this.cocFootprintRadiusNumeric);
            this.splitContainer2.Panel1.Controls.Add(this.label10);
            this.splitContainer2.Panel1.Controls.Add(this.label9);
            this.splitContainer2.Panel1.Controls.Add(this.label8);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.Controls.Add(this.cocClippingPanel);
            this.splitContainer2.Size = new System.Drawing.Size(766, 486);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 1;
            // 
            // lightSourceLayerNumeric
            // 
            this.lightSourceLayerNumeric.Location = new System.Drawing.Point(113, 6);
            this.lightSourceLayerNumeric.Name = "lightSourceLayerNumeric";
            this.lightSourceLayerNumeric.Size = new System.Drawing.Size(59, 20);
            this.lightSourceLayerNumeric.TabIndex = 1;
            this.lightSourceLayerNumeric.ValueChanged += new System.EventHandler(this.lightSourceNumeric_ValueChanged);
            // 
            // cocRadiusNumeric
            // 
            this.cocRadiusNumeric.Location = new System.Drawing.Point(113, 58);
            this.cocRadiusNumeric.Name = "cocRadiusNumeric";
            this.cocRadiusNumeric.Size = new System.Drawing.Size(59, 20);
            this.cocRadiusNumeric.TabIndex = 1;
            this.cocRadiusNumeric.ValueChanged += new System.EventHandler(this.cocRadiusNumeric_ValueChanged);
            // 
            // cocFootprintRadiusNumeric
            // 
            this.cocFootprintRadiusNumeric.Location = new System.Drawing.Point(113, 32);
            this.cocFootprintRadiusNumeric.Name = "cocFootprintRadiusNumeric";
            this.cocFootprintRadiusNumeric.Size = new System.Drawing.Size(59, 20);
            this.cocFootprintRadiusNumeric.TabIndex = 1;
            this.cocFootprintRadiusNumeric.ValueChanged += new System.EventHandler(this.cocFootprintRadiusNumeric_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "CoC radius:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Light source layer:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "CoC footprint radius:";
            // 
            // cocClippingPanel
            // 
            this.cocClippingPanel.Location = new System.Drawing.Point(3, 3);
            this.cocClippingPanel.Name = "cocClippingPanel";
            this.cocClippingPanel.Size = new System.Drawing.Size(556, 480);
            this.cocClippingPanel.TabIndex = 0;
            this.cocClippingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.cocClippingPanel_Paint);
            this.cocClippingPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cocClippingPanel_MouseMove);
            this.cocClippingPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cocClippingPanel_MouseDown);
            this.cocClippingPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cocClippingPanel_MouseUp);
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
            this.ClientSize = new System.Drawing.Size(992, 550);
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
            ((System.ComponentModel.ISupportInitialize)(this.epsilonForCloseDepthNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayEndXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayStartZNumeric)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lightSourceLayerNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cocRadiusNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cocFootprintRadiusNumeric)).EndInit();
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown rayEndYNumeric;
        private System.Windows.Forms.NumericUpDown rayStartYNumeric;
        private System.Windows.Forms.NumericUpDown rayEndXNumeric;
        private System.Windows.Forms.NumericUpDown rayStartXNumeric;
        private System.Windows.Forms.ToolStripMenuItem intersectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recomputeToolStripMenuItem;
        private System.Windows.Forms.Panel footprintTraversalPanel;
        private System.Windows.Forms.Label isecLayerLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel cocClippingPanel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.NumericUpDown cocFootprintRadiusNumeric;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown lightSourceLayerNumeric;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown cocRadiusNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown epsilonForCloseDepthNumeric;
        private System.Windows.Forms.Label label11;
    }
}