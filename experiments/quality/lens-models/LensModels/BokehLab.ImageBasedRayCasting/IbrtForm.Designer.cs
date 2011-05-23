namespace BokehLab.ImageBasedRayCasting
{
    partial class IbrtForm
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
            this.tonemapOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.specificOutputSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.lensFocalLengthNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.lensApertureNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.outputSizeYNumeric = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.outputSizeXNumeric = new System.Windows.Forms.NumericUpDown();
            this.sampleCountNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.layerZNumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.senzorShiftZNumeric = new System.Windows.Forms.NumericUpDown();
            this.senzorShiftYNumeric = new System.Windows.Forms.NumericUpDown();
            this.senzorTiltZNumeric = new System.Windows.Forms.NumericUpDown();
            this.senzorTiltYNumeric = new System.Windows.Forms.NumericUpDown();
            this.senzorShiftXNumeric = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.senzorTiltXNumeric = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedTimeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openLayerFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveRenderedFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.renderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.senzorWidthNumeric = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lensFocalLengthNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lensApertureNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputSizeYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputSizeXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layerZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorShiftZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorShiftYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorTiltZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorTiltYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorShiftXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorTiltXNumeric)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.senzorWidthNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(199, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(460, 378);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.senzorWidthNumeric);
            this.groupBox1.Controls.Add(this.tonemapOutputCheckBox);
            this.groupBox1.Controls.Add(this.specificOutputSizeCheckBox);
            this.groupBox1.Controls.Add(this.lensFocalLengthNumeric);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lensApertureNumeric);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.outputSizeYNumeric);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.outputSizeXNumeric);
            this.groupBox1.Controls.Add(this.sampleCountNumeric);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.layerZNumeric);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.senzorShiftZNumeric);
            this.groupBox1.Controls.Add(this.senzorShiftYNumeric);
            this.groupBox1.Controls.Add(this.senzorTiltZNumeric);
            this.groupBox1.Controls.Add(this.senzorTiltYNumeric);
            this.groupBox1.Controls.Add(this.senzorShiftXNumeric);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.senzorTiltXNumeric);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(12, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 378);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // tonemapOutputCheckBox
            // 
            this.tonemapOutputCheckBox.AutoSize = true;
            this.tonemapOutputCheckBox.Location = new System.Drawing.Point(9, 297);
            this.tonemapOutputCheckBox.Name = "tonemapOutputCheckBox";
            this.tonemapOutputCheckBox.Size = new System.Drawing.Size(141, 17);
            this.tonemapOutputCheckBox.TabIndex = 14;
            this.tonemapOutputCheckBox.Text = "Tonemap output image?";
            this.tonemapOutputCheckBox.UseVisualStyleBackColor = true;
            this.tonemapOutputCheckBox.CheckedChanged += new System.EventHandler(this.tonemapOutputCheckBox_CheckedChanged);
            // 
            // specificOutputSizeCheckBox
            // 
            this.specificOutputSizeCheckBox.AutoSize = true;
            this.specificOutputSizeCheckBox.Location = new System.Drawing.Point(9, 248);
            this.specificOutputSizeCheckBox.Name = "specificOutputSizeCheckBox";
            this.specificOutputSizeCheckBox.Size = new System.Drawing.Size(121, 17);
            this.specificOutputSizeCheckBox.TabIndex = 11;
            this.specificOutputSizeCheckBox.Text = "Specify output size?";
            this.specificOutputSizeCheckBox.UseVisualStyleBackColor = true;
            this.specificOutputSizeCheckBox.CheckedChanged += new System.EventHandler(this.specificOutputSizeCheckBox_CheckedChanged);
            // 
            // lensFocalLengthNumeric
            // 
            this.lensFocalLengthNumeric.DecimalPlaces = 3;
            this.lensFocalLengthNumeric.Location = new System.Drawing.Point(109, 71);
            this.lensFocalLengthNumeric.Name = "lensFocalLengthNumeric";
            this.lensFocalLengthNumeric.Size = new System.Drawing.Size(60, 20);
            this.lensFocalLengthNumeric.TabIndex = 3;
            this.lensFocalLengthNumeric.ValueChanged += new System.EventHandler(this.lensFocalLengthNumeric_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Lens focal length:";
            // 
            // lensApertureNumeric
            // 
            this.lensApertureNumeric.DecimalPlaces = 2;
            this.lensApertureNumeric.Location = new System.Drawing.Point(109, 45);
            this.lensApertureNumeric.Name = "lensApertureNumeric";
            this.lensApertureNumeric.Size = new System.Drawing.Size(60, 20);
            this.lensApertureNumeric.TabIndex = 2;
            this.lensApertureNumeric.ValueChanged += new System.EventHandler(this.lensApertureNumeric_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Lens aperture:";
            // 
            // outputSizeYNumeric
            // 
            this.outputSizeYNumeric.Enabled = false;
            this.outputSizeYNumeric.Location = new System.Drawing.Point(106, 271);
            this.outputSizeYNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.outputSizeYNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.outputSizeYNumeric.Name = "outputSizeYNumeric";
            this.outputSizeYNumeric.Size = new System.Drawing.Size(69, 20);
            this.outputSizeYNumeric.TabIndex = 13;
            this.outputSizeYNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(88, 273);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "x";
            // 
            // outputSizeXNumeric
            // 
            this.outputSizeXNumeric.Enabled = false;
            this.outputSizeXNumeric.Location = new System.Drawing.Point(9, 271);
            this.outputSizeXNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.outputSizeXNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.outputSizeXNumeric.Name = "outputSizeXNumeric";
            this.outputSizeXNumeric.Size = new System.Drawing.Size(73, 20);
            this.outputSizeXNumeric.TabIndex = 12;
            this.outputSizeXNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // sampleCountNumeric
            // 
            this.sampleCountNumeric.Location = new System.Drawing.Point(109, 19);
            this.sampleCountNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.sampleCountNumeric.Name = "sampleCountNumeric";
            this.sampleCountNumeric.Size = new System.Drawing.Size(60, 20);
            this.sampleCountNumeric.TabIndex = 1;
            this.sampleCountNumeric.ValueChanged += new System.EventHandler(this.sampleCountNumeric_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Sample count:";
            // 
            // layerZNumeric
            // 
            this.layerZNumeric.DecimalPlaces = 3;
            this.layerZNumeric.Location = new System.Drawing.Point(109, 97);
            this.layerZNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.layerZNumeric.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.layerZNumeric.Name = "layerZNumeric";
            this.layerZNumeric.Size = new System.Drawing.Size(60, 20);
            this.layerZNumeric.TabIndex = 4;
            this.layerZNumeric.ValueChanged += new System.EventHandler(this.layerZNumeric_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Layer position (Z):";
            // 
            // senzorShiftZNumeric
            // 
            this.senzorShiftZNumeric.DecimalPlaces = 2;
            this.senzorShiftZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.senzorShiftZNumeric.Location = new System.Drawing.Point(119, 178);
            this.senzorShiftZNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.senzorShiftZNumeric.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.senzorShiftZNumeric.Name = "senzorShiftZNumeric";
            this.senzorShiftZNumeric.Size = new System.Drawing.Size(50, 20);
            this.senzorShiftZNumeric.TabIndex = 10;
            this.senzorShiftZNumeric.ValueChanged += new System.EventHandler(this.senzorShiftZNumeric_ValueChanged);
            // 
            // senzorShiftYNumeric
            // 
            this.senzorShiftYNumeric.DecimalPlaces = 2;
            this.senzorShiftYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.senzorShiftYNumeric.Location = new System.Drawing.Point(64, 178);
            this.senzorShiftYNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.senzorShiftYNumeric.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.senzorShiftYNumeric.Name = "senzorShiftYNumeric";
            this.senzorShiftYNumeric.Size = new System.Drawing.Size(50, 20);
            this.senzorShiftYNumeric.TabIndex = 9;
            this.senzorShiftYNumeric.ValueChanged += new System.EventHandler(this.senzorShiftYNumeric_ValueChanged);
            // 
            // senzorTiltZNumeric
            // 
            this.senzorTiltZNumeric.DecimalPlaces = 2;
            this.senzorTiltZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.senzorTiltZNumeric.Location = new System.Drawing.Point(119, 139);
            this.senzorTiltZNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.senzorTiltZNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.senzorTiltZNumeric.Name = "senzorTiltZNumeric";
            this.senzorTiltZNumeric.Size = new System.Drawing.Size(50, 20);
            this.senzorTiltZNumeric.TabIndex = 7;
            this.senzorTiltZNumeric.ValueChanged += new System.EventHandler(this.senzorTiltZNumeric_ValueChanged);
            // 
            // senzorTiltYNumeric
            // 
            this.senzorTiltYNumeric.DecimalPlaces = 2;
            this.senzorTiltYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.senzorTiltYNumeric.Location = new System.Drawing.Point(63, 139);
            this.senzorTiltYNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.senzorTiltYNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.senzorTiltYNumeric.Name = "senzorTiltYNumeric";
            this.senzorTiltYNumeric.Size = new System.Drawing.Size(50, 20);
            this.senzorTiltYNumeric.TabIndex = 6;
            this.senzorTiltYNumeric.ValueChanged += new System.EventHandler(this.senzorTiltYNumeric_ValueChanged);
            // 
            // senzorShiftXNumeric
            // 
            this.senzorShiftXNumeric.DecimalPlaces = 2;
            this.senzorShiftXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.senzorShiftXNumeric.Location = new System.Drawing.Point(9, 178);
            this.senzorShiftXNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.senzorShiftXNumeric.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.senzorShiftXNumeric.Name = "senzorShiftXNumeric";
            this.senzorShiftXNumeric.Size = new System.Drawing.Size(50, 20);
            this.senzorShiftXNumeric.TabIndex = 8;
            this.senzorShiftXNumeric.ValueChanged += new System.EventHandler(this.senzorShiftXNumeric_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 162);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Senzor shift (X, Y, Z):";
            // 
            // senzorTiltXNumeric
            // 
            this.senzorTiltXNumeric.DecimalPlaces = 2;
            this.senzorTiltXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.senzorTiltXNumeric.Location = new System.Drawing.Point(9, 139);
            this.senzorTiltXNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.senzorTiltXNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.senzorTiltXNumeric.Name = "senzorTiltXNumeric";
            this.senzorTiltXNumeric.Size = new System.Drawing.Size(50, 20);
            this.senzorTiltXNumeric.TabIndex = 5;
            this.senzorTiltXNumeric.ValueChanged += new System.EventHandler(this.senzorTiltXNumeric_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Senzor tilt (X, Y, Z):";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.elapsedTimeToolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 412);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(671, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(77, 17);
            this.toolStripStatusLabel1.Text = "Elapsed time:";
            // 
            // elapsedTimeToolStripStatusLabel
            // 
            this.elapsedTimeToolStripStatusLabel.Name = "elapsedTimeToolStripStatusLabel";
            this.elapsedTimeToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // openLayerFileDialog
            // 
            this.openLayerFileDialog.Filter = "PNG Files|*.png|All files|*.*";
            // 
            // saveRenderedFileDialog
            // 
            this.saveRenderedFileDialog.Filter = "PNG Files|*.png|All files|*.*";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(671, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(73, 20);
            this.toolStripMenuItem2.Text = "Rendering";
            // 
            // renderToolStripMenuItem
            // 
            this.renderToolStripMenuItem.Name = "renderToolStripMenuItem";
            this.renderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.renderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renderToolStripMenuItem.Text = "&Render";
            this.renderToolStripMenuItem.Click += new System.EventHandler(this.renderToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 206);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Senzor width:";
            // 
            // senzorWidthNumeric
            // 
            this.senzorWidthNumeric.DecimalPlaces = 2;
            this.senzorWidthNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.senzorWidthNumeric.Location = new System.Drawing.Point(106, 204);
            this.senzorWidthNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.senzorWidthNumeric.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.senzorWidthNumeric.Name = "senzorWidthNumeric";
            this.senzorWidthNumeric.Size = new System.Drawing.Size(63, 20);
            this.senzorWidthNumeric.TabIndex = 15;
            this.senzorWidthNumeric.ValueChanged += new System.EventHandler(this.senzorWidthNumeric_ValueChanged);
            // 
            // IbrtForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 434);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IbrtForm";
            this.Text = "BokehLab - Image-based ray tracing";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lensFocalLengthNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lensApertureNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputSizeYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputSizeXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layerZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorShiftZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorShiftYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorTiltZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorTiltYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorShiftXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.senzorTiltXNumeric)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.senzorWidthNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown layerZNumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel elapsedTimeToolStripStatusLabel;
        private System.Windows.Forms.NumericUpDown sampleCountNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown lensApertureNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openLayerFileDialog;
        private System.Windows.Forms.SaveFileDialog saveRenderedFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem renderToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown lensFocalLengthNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox specificOutputSizeCheckBox;
        private System.Windows.Forms.NumericUpDown outputSizeYNumeric;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown outputSizeXNumeric;
        private System.Windows.Forms.CheckBox tonemapOutputCheckBox;
        private System.Windows.Forms.NumericUpDown senzorTiltXNumeric;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown senzorTiltZNumeric;
        private System.Windows.Forms.NumericUpDown senzorTiltYNumeric;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown senzorShiftZNumeric;
        private System.Windows.Forms.NumericUpDown senzorShiftYNumeric;
        private System.Windows.Forms.NumericUpDown senzorShiftXNumeric;
        private System.Windows.Forms.NumericUpDown senzorWidthNumeric;
        private System.Windows.Forms.Label label1;
    }
}

