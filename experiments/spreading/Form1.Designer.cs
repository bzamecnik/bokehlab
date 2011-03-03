namespace spreading
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
        this.panel1 = new System.Windows.Forms.Panel();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.buttonSave = new System.Windows.Forms.Button();
        this.buttonRecode = new System.Windows.Forms.Button();
        this.labelElapsed = new System.Windows.Forms.Label();
        this.buttonLoad = new System.Windows.Forms.Button();
        this.blurRadiusNumeric = new System.Windows.Forms.NumericUpDown();
        this.label1 = new System.Windows.Forms.Label();
        this.imageTypeComboBox = new System.Windows.Forms.ComboBox();
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.blurRadiusNumeric)).BeginInit();
        this.SuspendLayout();
        // 
        // panel1
        // 
        this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.panel1.AutoScroll = true;
        this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.panel1.Controls.Add(this.pictureBox1);
        this.panel1.Location = new System.Drawing.Point(13, 13);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(877, 410);
        this.panel1.TabIndex = 0;
        // 
        // pictureBox1
        // 
        this.pictureBox1.Location = new System.Drawing.Point(0, 0);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(680, 410);
        this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
        this.pictureBox1.TabIndex = 2;
        this.pictureBox1.TabStop = false;
        // 
        // buttonSave
        // 
        this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.buttonSave.Location = new System.Drawing.Point(782, 439);
        this.buttonSave.Name = "buttonSave";
        this.buttonSave.Size = new System.Drawing.Size(108, 23);
        this.buttonSave.TabIndex = 4;
        this.buttonSave.Text = "Save image";
        this.buttonSave.UseVisualStyleBackColor = true;
        this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
        // 
        // buttonRecode
        // 
        this.buttonRecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.buttonRecode.Location = new System.Drawing.Point(140, 439);
        this.buttonRecode.Name = "buttonRecode";
        this.buttonRecode.Size = new System.Drawing.Size(97, 23);
        this.buttonRecode.TabIndex = 2;
        this.buttonRecode.Text = "Filter";
        this.buttonRecode.UseVisualStyleBackColor = true;
        this.buttonRecode.Click += new System.EventHandler(this.buttonRecode_Click);
        // 
        // labelElapsed
        // 
        this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.labelElapsed.AutoSize = true;
        this.labelElapsed.Location = new System.Drawing.Point(435, 445);
        this.labelElapsed.Name = "labelElapsed";
        this.labelElapsed.Size = new System.Drawing.Size(48, 13);
        this.labelElapsed.TabIndex = 8;
        this.labelElapsed.Text = "Elapsed:";
        // 
        // buttonLoad
        // 
        this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.buttonLoad.Location = new System.Drawing.Point(13, 439);
        this.buttonLoad.Name = "buttonLoad";
        this.buttonLoad.Size = new System.Drawing.Size(110, 23);
        this.buttonLoad.TabIndex = 1;
        this.buttonLoad.Text = "Load image";
        this.buttonLoad.UseVisualStyleBackColor = true;
        this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
        // 
        // blurRadiusNumeric
        // 
        this.blurRadiusNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.blurRadiusNumeric.Location = new System.Drawing.Point(309, 442);
        this.blurRadiusNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
        this.blurRadiusNumeric.Name = "blurRadiusNumeric";
        this.blurRadiusNumeric.Size = new System.Drawing.Size(120, 20);
        this.blurRadiusNumeric.TabIndex = 9;
        this.blurRadiusNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
        this.blurRadiusNumeric.ValueChanged += new System.EventHandler(this.blurRadiusNumeric_ValueChanged);
        // 
        // label1
        // 
        this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(244, 445);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(59, 13);
        this.label1.TabIndex = 10;
        this.label1.Text = "Blur radius:";
        // 
        // imageTypeComboBox
        // 
        this.imageTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.imageTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.imageTypeComboBox.FormattingEnabled = true;
        this.imageTypeComboBox.Items.AddRange(new object[] {
            "original",
            "blurred"});
        this.imageTypeComboBox.Location = new System.Drawing.Point(655, 441);
        this.imageTypeComboBox.Name = "imageTypeComboBox";
        this.imageTypeComboBox.Size = new System.Drawing.Size(121, 21);
        this.imageTypeComboBox.TabIndex = 11;
        this.imageTypeComboBox.Visible = false;
        this.imageTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.imageTypeComboBox_SelectedIndexChanged);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(909, 474);
        this.Controls.Add(this.imageTypeComboBox);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.blurRadiusNumeric);
        this.Controls.Add(this.buttonLoad);
        this.Controls.Add(this.labelElapsed);
        this.Controls.Add(this.buttonRecode);
        this.Controls.Add(this.buttonSave);
        this.Controls.Add(this.panel1);
        this.MinimumSize = new System.Drawing.Size(720, 200);
        this.Name = "Form1";
        this.Text = "Fast rectangle spreading";
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.blurRadiusNumeric)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonRecode;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Button buttonLoad;
    private System.Windows.Forms.NumericUpDown blurRadiusNumeric;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox imageTypeComboBox;
  }
}

