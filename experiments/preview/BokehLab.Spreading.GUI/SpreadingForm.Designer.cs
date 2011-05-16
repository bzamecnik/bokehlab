namespace BokehLab.Spreading.GUI
{
  partial class SpreadingForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose ( bool disposing )
    {
      if ( disposing && (components != null) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
        this.buttonSave = new System.Windows.Forms.Button();
        this.buttonFilter = new System.Windows.Forms.Button();
        this.labelElapsed = new System.Windows.Forms.Label();
        this.loadImageButton = new System.Windows.Forms.Button();
        this.blurRadiusNumeric = new System.Windows.Forms.NumericUpDown();
        this.label1 = new System.Windows.Forms.Label();
        this.imageTypeComboBox = new System.Windows.Forms.ComboBox();
        this.loadDepthmapButton = new System.Windows.Forms.Button();
        this.imageSizeOrigbutton = new System.Windows.Forms.Button();
        this.imageSizeStretchButton = new System.Windows.Forms.Button();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.label8 = new System.Windows.Forms.Label();
        this.filterTypeComboBox = new System.Windows.Forms.ComboBox();
        this.toneMappingCheckBox = new System.Windows.Forms.CheckBox();
        this.label7 = new System.Windows.Forms.Label();
        this.apertureNumeric = new System.Windows.Forms.NumericUpDown();
        this.label6 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.focusPlaneNumeric = new System.Windows.Forms.NumericUpDown();
        this.label4 = new System.Windows.Forms.Label();
        this.imageSizeLabel = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.clearDepthmapButton = new System.Windows.Forms.Button();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        ((System.ComponentModel.ISupportInitialize)(this.blurRadiusNumeric)).BeginInit();
        this.splitContainer1.Panel1.SuspendLayout();
        this.splitContainer1.Panel2.SuspendLayout();
        this.splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.apertureNumeric)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.focusPlaneNumeric)).BeginInit();
        this.tableLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        this.SuspendLayout();
        // 
        // buttonSave
        // 
        this.buttonSave.Location = new System.Drawing.Point(1, 177);
        this.buttonSave.Name = "buttonSave";
        this.buttonSave.Size = new System.Drawing.Size(145, 23);
        this.buttonSave.TabIndex = 6;
        this.buttonSave.Text = "Save image";
        this.buttonSave.UseVisualStyleBackColor = true;
        this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
        // 
        // buttonFilter
        // 
        this.buttonFilter.Location = new System.Drawing.Point(2, 130);
        this.buttonFilter.Name = "buttonFilter";
        this.buttonFilter.Size = new System.Drawing.Size(144, 23);
        this.buttonFilter.TabIndex = 3;
        this.buttonFilter.Text = "Filter";
        this.buttonFilter.UseVisualStyleBackColor = true;
        this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
        // 
        // labelElapsed
        // 
        this.labelElapsed.AutoSize = true;
        this.labelElapsed.Location = new System.Drawing.Point(2, 156);
        this.labelElapsed.Name = "labelElapsed";
        this.labelElapsed.Size = new System.Drawing.Size(70, 13);
        this.labelElapsed.TabIndex = 0;
        this.labelElapsed.Text = "Elapsed time:";
        // 
        // loadImageButton
        // 
        this.loadImageButton.Location = new System.Drawing.Point(3, 3);
        this.loadImageButton.Name = "loadImageButton";
        this.loadImageButton.Size = new System.Drawing.Size(144, 23);
        this.loadImageButton.TabIndex = 1;
        this.loadImageButton.Text = "Load image";
        this.loadImageButton.UseVisualStyleBackColor = true;
        this.loadImageButton.Click += new System.EventHandler(this.buttonLoad_Click);
        // 
        // blurRadiusNumeric
        // 
        this.blurRadiusNumeric.Location = new System.Drawing.Point(3, 456);
        this.blurRadiusNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
        this.blurRadiusNumeric.Name = "blurRadiusNumeric";
        this.blurRadiusNumeric.Size = new System.Drawing.Size(144, 20);
        this.blurRadiusNumeric.TabIndex = 10;
        this.blurRadiusNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
        this.blurRadiusNumeric.ValueChanged += new System.EventHandler(this.blurRadiusNumeric_ValueChanged);
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(3, 440);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(104, 13);
        this.label1.TabIndex = 0;
        this.label1.Text = "Max blur PSF radius:";
        // 
        // imageTypeComboBox
        // 
        this.imageTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.imageTypeComboBox.FormattingEnabled = true;
        this.imageTypeComboBox.Items.AddRange(new object[] {
            "Original",
            "Filtered",
            "Depth map"});
        this.imageTypeComboBox.Location = new System.Drawing.Point(3, 224);
        this.imageTypeComboBox.Name = "imageTypeComboBox";
        this.imageTypeComboBox.Size = new System.Drawing.Size(144, 21);
        this.imageTypeComboBox.TabIndex = 7;
        this.imageTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.imageTypeComboBox_SelectedIndexChanged);
        // 
        // loadDepthmapButton
        // 
        this.loadDepthmapButton.Location = new System.Drawing.Point(2, 48);
        this.loadDepthmapButton.Name = "loadDepthmapButton";
        this.loadDepthmapButton.Size = new System.Drawing.Size(144, 23);
        this.loadDepthmapButton.TabIndex = 2;
        this.loadDepthmapButton.Text = "Load depth map";
        this.loadDepthmapButton.UseVisualStyleBackColor = true;
        this.loadDepthmapButton.Click += new System.EventHandler(this.loadDepthMapButton_Click);
        // 
        // imageSizeOrigbutton
        // 
        this.imageSizeOrigbutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.imageSizeOrigbutton.AutoSize = true;
        this.imageSizeOrigbutton.Location = new System.Drawing.Point(0, 0);
        this.imageSizeOrigbutton.Margin = new System.Windows.Forms.Padding(0);
        this.imageSizeOrigbutton.Name = "imageSizeOrigbutton";
        this.imageSizeOrigbutton.Size = new System.Drawing.Size(58, 23);
        this.imageSizeOrigbutton.TabIndex = 8;
        this.imageSizeOrigbutton.Text = "1:1";
        this.imageSizeOrigbutton.UseVisualStyleBackColor = true;
        this.imageSizeOrigbutton.Click += new System.EventHandler(this.imageSizeOrigButton_Click);
        // 
        // imageSizeStretchButton
        // 
        this.imageSizeStretchButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.imageSizeStretchButton.AutoSize = true;
        this.imageSizeStretchButton.Location = new System.Drawing.Point(58, 0);
        this.imageSizeStretchButton.Margin = new System.Windows.Forms.Padding(0);
        this.imageSizeStretchButton.Name = "imageSizeStretchButton";
        this.imageSizeStretchButton.Size = new System.Drawing.Size(58, 23);
        this.imageSizeStretchButton.TabIndex = 9;
        this.imageSizeStretchButton.Text = "<->";
        this.imageSizeStretchButton.UseVisualStyleBackColor = true;
        this.imageSizeStretchButton.Click += new System.EventHandler(this.imageSizeStretchButton_Click);
        // 
        // splitContainer1
        // 
        this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        this.splitContainer1.Location = new System.Drawing.Point(12, 13);
        this.splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        this.splitContainer1.Panel1.Controls.Add(this.label8);
        this.splitContainer1.Panel1.Controls.Add(this.filterTypeComboBox);
        this.splitContainer1.Panel1.Controls.Add(this.toneMappingCheckBox);
        this.splitContainer1.Panel1.Controls.Add(this.label7);
        this.splitContainer1.Panel1.Controls.Add(this.apertureNumeric);
        this.splitContainer1.Panel1.Controls.Add(this.label6);
        this.splitContainer1.Panel1.Controls.Add(this.label5);
        this.splitContainer1.Panel1.Controls.Add(this.focusPlaneNumeric);
        this.splitContainer1.Panel1.Controls.Add(this.label4);
        this.splitContainer1.Panel1.Controls.Add(this.imageSizeLabel);
        this.splitContainer1.Panel1.Controls.Add(this.label3);
        this.splitContainer1.Panel1.Controls.Add(this.label2);
        this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
        this.splitContainer1.Panel1.Controls.Add(this.loadImageButton);
        this.splitContainer1.Panel1.Controls.Add(this.buttonSave);
        this.splitContainer1.Panel1.Controls.Add(this.blurRadiusNumeric);
        this.splitContainer1.Panel1.Controls.Add(this.clearDepthmapButton);
        this.splitContainer1.Panel1.Controls.Add(this.loadDepthmapButton);
        this.splitContainer1.Panel1.Controls.Add(this.buttonFilter);
        this.splitContainer1.Panel1.Controls.Add(this.label1);
        this.splitContainer1.Panel1.Controls.Add(this.imageTypeComboBox);
        this.splitContainer1.Panel1.Controls.Add(this.labelElapsed);
        // 
        // splitContainer1.Panel2
        // 
        this.splitContainer1.Panel2.AutoScroll = true;
        this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
        this.splitContainer1.Size = new System.Drawing.Size(889, 561);
        this.splitContainer1.SplitterDistance = 150;
        this.splitContainer1.TabIndex = 15;
        // 
        // label8
        // 
        this.label8.AutoSize = true;
        this.label8.Location = new System.Drawing.Point(6, 107);
        this.label8.Name = "label8";
        this.label8.Size = new System.Drawing.Size(55, 13);
        this.label8.TabIndex = 24;
        this.label8.Text = "Filter type:";
        // 
        // filterTypeComboBox
        // 
        this.filterTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.filterTypeComboBox.FormattingEnabled = true;
        this.filterTypeComboBox.Items.AddRange(new object[] {
            "rectangle",
            "perimeter",
            "hybrid"});
        this.filterTypeComboBox.Location = new System.Drawing.Point(67, 103);
        this.filterTypeComboBox.Name = "filterTypeComboBox";
        this.filterTypeComboBox.Size = new System.Drawing.Size(79, 21);
        this.filterTypeComboBox.TabIndex = 23;
        // 
        // toneMappingCheckBox
        // 
        this.toneMappingCheckBox.AutoSize = true;
        this.toneMappingCheckBox.Location = new System.Drawing.Point(3, 280);
        this.toneMappingCheckBox.Name = "toneMappingCheckBox";
        this.toneMappingCheckBox.Size = new System.Drawing.Size(135, 17);
        this.toneMappingCheckBox.TabIndex = 22;
        this.toneMappingCheckBox.Text = "Tone mapping enabled";
        this.toneMappingCheckBox.UseVisualStyleBackColor = true;
        this.toneMappingCheckBox.CheckedChanged += new System.EventHandler(this.toneMappingCheckBox_CheckedChanged);
        // 
        // label7
        // 
        this.label7.AutoSize = true;
        this.label7.Location = new System.Drawing.Point(3, 417);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(93, 13);
        this.label7.TabIndex = 0;
        this.label7.Text = "Constant blur size:";
        // 
        // apertureNumeric
        // 
        this.apertureNumeric.Location = new System.Drawing.Point(4, 384);
        this.apertureNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
        this.apertureNumeric.Name = "apertureNumeric";
        this.apertureNumeric.Size = new System.Drawing.Size(144, 20);
        this.apertureNumeric.TabIndex = 5;
        this.apertureNumeric.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
        this.apertureNumeric.ValueChanged += new System.EventHandler(this.apertureNumeric_ValueChanged);
        // 
        // label6
        // 
        this.label6.AutoSize = true;
        this.label6.Location = new System.Drawing.Point(3, 368);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(50, 13);
        this.label6.TabIndex = 21;
        this.label6.Text = "Aperture:";
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(3, 306);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(84, 13);
        this.label5.TabIndex = 0;
        this.label5.Text = "Thin lens model:";
        // 
        // focusPlaneNumeric
        // 
        this.focusPlaneNumeric.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
        this.focusPlaneNumeric.Location = new System.Drawing.Point(4, 345);
        this.focusPlaneNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
        this.focusPlaneNumeric.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
        this.focusPlaneNumeric.Name = "focusPlaneNumeric";
        this.focusPlaneNumeric.Size = new System.Drawing.Size(144, 20);
        this.focusPlaneNumeric.TabIndex = 4;
        this.focusPlaneNumeric.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
        this.focusPlaneNumeric.ValueChanged += new System.EventHandler(this.focusPlaneNumeric_ValueChanged);
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(3, 329);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(84, 13);
        this.label4.TabIndex = 0;
        this.label4.Text = "Focus plane (Z):";
        // 
        // imageSizeLabel
        // 
        this.imageSizeLabel.AutoSize = true;
        this.imageSizeLabel.Location = new System.Drawing.Point(69, 29);
        this.imageSizeLabel.Name = "imageSizeLabel";
        this.imageSizeLabel.Size = new System.Drawing.Size(0, 13);
        this.imageSizeLabel.TabIndex = 16;
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(3, 29);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(60, 13);
        this.label3.TabIndex = 16;
        this.label3.Text = "Image size:";
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(3, 208);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(75, 13);
        this.label2.TabIndex = 0;
        this.label2.Text = "Display image:";
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.tableLayoutPanel1.AutoSize = true;
        this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tableLayoutPanel1.ColumnCount = 2;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel1.Controls.Add(this.imageSizeStretchButton, 1, 0);
        this.tableLayoutPanel1.Controls.Add(this.imageSizeOrigbutton, 0, 0);
        this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 251);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 1;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel1.Size = new System.Drawing.Size(116, 23);
        this.tableLayoutPanel1.TabIndex = 14;
        // 
        // clearDepthmapButton
        // 
        this.clearDepthmapButton.Location = new System.Drawing.Point(3, 77);
        this.clearDepthmapButton.Name = "clearDepthmapButton";
        this.clearDepthmapButton.Size = new System.Drawing.Size(144, 23);
        this.clearDepthmapButton.TabIndex = 11;
        this.clearDepthmapButton.Text = "Clear depth map";
        this.clearDepthmapButton.UseVisualStyleBackColor = true;
        this.clearDepthmapButton.Click += new System.EventHandler(this.clearDepthmapButton_Click);
        // 
        // pictureBox1
        // 
        this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox1.Location = new System.Drawing.Point(0, 0);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(735, 561);
        this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.pictureBox1.TabIndex = 2;
        this.pictureBox1.TabStop = false;
        // 
        // SpreadingForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(913, 586);
        this.Controls.Add(this.splitContainer1);
        this.MinimumSize = new System.Drawing.Size(200, 200);
        this.Name = "SpreadingForm";
        this.Text = "Fast rectangle spreading";
        this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
        ((System.ComponentModel.ISupportInitialize)(this.blurRadiusNumeric)).EndInit();
        this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel1.PerformLayout();
        this.splitContainer1.Panel2.ResumeLayout(false);
        this.splitContainer1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.apertureNumeric)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.focusPlaneNumeric)).EndInit();
        this.tableLayoutPanel1.ResumeLayout(false);
        this.tableLayoutPanel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonFilter;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Button loadImageButton;
    private System.Windows.Forms.NumericUpDown blurRadiusNumeric;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox imageTypeComboBox;
    private System.Windows.Forms.Button loadDepthmapButton;
    private System.Windows.Forms.Button imageSizeOrigbutton;
    private System.Windows.Forms.Button imageSizeStretchButton;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label imageSizeLabel;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button clearDepthmapButton;
    private System.Windows.Forms.NumericUpDown apertureNumeric;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown focusPlaneNumeric;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.CheckBox toneMappingCheckBox;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox filterTypeComboBox;
  }
}

