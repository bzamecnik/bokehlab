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
        this.drawingPanel = new System.Windows.Forms.Panel();
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.rayDirectionYNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.rayDirectionXNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.rayOriginYNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.rayOriginXNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.label7 = new System.Windows.Forms.Label();
        this.sphereRadiusNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.label5 = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.rayDirectionYNumericUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.rayDirectionXNumericUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.rayOriginYNumericUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.rayOriginXNumericUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.sphereRadiusNumericUpDown)).BeginInit();
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
        this.groupBox1.Controls.Add(this.rayDirectionYNumericUpDown);
        this.groupBox1.Controls.Add(this.rayDirectionXNumericUpDown);
        this.groupBox1.Controls.Add(this.rayOriginYNumericUpDown);
        this.groupBox1.Controls.Add(this.rayOriginXNumericUpDown);
        this.groupBox1.Controls.Add(this.label7);
        this.groupBox1.Controls.Add(this.sphereRadiusNumericUpDown);
        this.groupBox1.Controls.Add(this.label5);
        this.groupBox1.Controls.Add(this.label6);
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
        // rayDirectionYNumericUpDown
        // 
        this.rayDirectionYNumericUpDown.DecimalPlaces = 1;
        this.rayDirectionYNumericUpDown.Location = new System.Drawing.Point(76, 172);
        this.rayDirectionYNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
        this.rayDirectionYNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
        this.rayDirectionYNumericUpDown.Name = "rayDirectionYNumericUpDown";
        this.rayDirectionYNumericUpDown.Size = new System.Drawing.Size(110, 20);
        this.rayDirectionYNumericUpDown.TabIndex = 2;
        this.rayDirectionYNumericUpDown.ValueChanged += new System.EventHandler(this.rayDirectionYNumericUpDown_ValueChanged);
        // 
        // rayDirectionXNumericUpDown
        // 
        this.rayDirectionXNumericUpDown.DecimalPlaces = 1;
        this.rayDirectionXNumericUpDown.Location = new System.Drawing.Point(76, 146);
        this.rayDirectionXNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
        this.rayDirectionXNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
        this.rayDirectionXNumericUpDown.Name = "rayDirectionXNumericUpDown";
        this.rayDirectionXNumericUpDown.Size = new System.Drawing.Size(110, 20);
        this.rayDirectionXNumericUpDown.TabIndex = 2;
        this.rayDirectionXNumericUpDown.ValueChanged += new System.EventHandler(this.rayDirectionXNumericUpDown_ValueChanged);
        // 
        // rayOriginYNumericUpDown
        // 
        this.rayOriginYNumericUpDown.DecimalPlaces = 1;
        this.rayOriginYNumericUpDown.Location = new System.Drawing.Point(76, 107);
        this.rayOriginYNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
        this.rayOriginYNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
        this.rayOriginYNumericUpDown.Name = "rayOriginYNumericUpDown";
        this.rayOriginYNumericUpDown.Size = new System.Drawing.Size(110, 20);
        this.rayOriginYNumericUpDown.TabIndex = 2;
        this.rayOriginYNumericUpDown.ValueChanged += new System.EventHandler(this.rayOriginYNumericUpDown_ValueChanged);
        // 
        // rayOriginXNumericUpDown
        // 
        this.rayOriginXNumericUpDown.DecimalPlaces = 1;
        this.rayOriginXNumericUpDown.Location = new System.Drawing.Point(76, 81);
        this.rayOriginXNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
        this.rayOriginXNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
        this.rayOriginXNumericUpDown.Name = "rayOriginXNumericUpDown";
        this.rayOriginXNumericUpDown.Size = new System.Drawing.Size(110, 20);
        this.rayOriginXNumericUpDown.TabIndex = 2;
        this.rayOriginXNumericUpDown.ValueChanged += new System.EventHandler(this.rayOriginXNumericUpDown_ValueChanged);
        // 
        // label7
        // 
        this.label7.AutoSize = true;
        this.label7.Location = new System.Drawing.Point(8, 174);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(62, 13);
        this.label7.TabIndex = 1;
        this.label7.Text = "Direction Y:";
        // 
        // sphereRadiusNumericUpDown
        // 
        this.sphereRadiusNumericUpDown.DecimalPlaces = 1;
        this.sphereRadiusNumericUpDown.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
        this.sphereRadiusNumericUpDown.Location = new System.Drawing.Point(76, 41);
        this.sphereRadiusNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
        this.sphereRadiusNumericUpDown.Name = "sphereRadiusNumericUpDown";
        this.sphereRadiusNumericUpDown.Size = new System.Drawing.Size(110, 20);
        this.sphereRadiusNumericUpDown.TabIndex = 2;
        this.sphereRadiusNumericUpDown.ValueChanged += new System.EventHandler(this.sphereRadiusNumericUpDown_ValueChanged);
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(8, 148);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(62, 13);
        this.label5.TabIndex = 1;
        this.label5.Text = "Direction X:";
        // 
        // label6
        // 
        this.label6.AutoSize = true;
        this.label6.Location = new System.Drawing.Point(8, 109);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(47, 13);
        this.label6.TabIndex = 1;
        this.label6.Text = "Origin Y:";
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(8, 88);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(47, 13);
        this.label4.TabIndex = 1;
        this.label4.Text = "Origin X:";
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
        this.label3.Location = new System.Drawing.Point(8, 63);
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
        ((System.ComponentModel.ISupportInitialize)(this.rayDirectionYNumericUpDown)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.rayDirectionXNumericUpDown)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.rayOriginYNumericUpDown)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.rayOriginXNumericUpDown)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.sphereRadiusNumericUpDown)).EndInit();
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
    private System.Windows.Forms.NumericUpDown rayDirectionXNumericUpDown;
    private System.Windows.Forms.NumericUpDown rayOriginXNumericUpDown;
    private System.Windows.Forms.NumericUpDown sphereRadiusNumericUpDown;
    private System.Windows.Forms.NumericUpDown rayOriginYNumericUpDown;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown rayDirectionYNumericUpDown;
    private System.Windows.Forms.Label label7;

  }
}
