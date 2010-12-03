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
        this.rayDirectionPhiNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.label5 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumericUpDown)).BeginInit();
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
        this.groupBox1.Controls.Add(this.rayDirectionPhiNumericUpDown);
        this.groupBox1.Controls.Add(this.label5);
        this.groupBox1.Controls.Add(this.label3);
        this.groupBox1.Location = new System.Drawing.Point(598, 44);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new System.Drawing.Size(200, 390);
        this.groupBox1.TabIndex = 1;
        this.groupBox1.TabStop = false;
        this.groupBox1.Text = "Controls";
        // 
        // rayDirectionPhiNumericUpDown
        // 
        this.rayDirectionPhiNumericUpDown.DecimalPlaces = 2;
        this.rayDirectionPhiNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
        this.rayDirectionPhiNumericUpDown.Location = new System.Drawing.Point(96, 55);
        this.rayDirectionPhiNumericUpDown.Maximum = new decimal(new int[] {
            628318,
            0,
            0,
            327680});
        this.rayDirectionPhiNumericUpDown.Name = "rayDirectionPhiNumericUpDown";
        this.rayDirectionPhiNumericUpDown.Size = new System.Drawing.Size(89, 20);
        this.rayDirectionPhiNumericUpDown.TabIndex = 2;
        this.rayDirectionPhiNumericUpDown.Value = new decimal(new int[] {
            157,
            0,
            0,
            131072});
        this.rayDirectionPhiNumericUpDown.ValueChanged += new System.EventHandler(this.rayDirectionPhiNumericUpDown_ValueChanged);
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(6, 57);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(70, 13);
        this.label5.TabIndex = 1;
        this.label5.Text = "Direction Phi:";
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this.label3.Location = new System.Drawing.Point(6, 30);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(33, 13);
        this.label3.TabIndex = 1;
        this.label3.Text = "Ray:";
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this.label1.Location = new System.Drawing.Point(598, 13);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(109, 13);
        this.label1.TabIndex = 2;
        this.label1.Text = "Refraction of light";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(805, 446);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.drawingPanel);
        this.KeyPreview = true;
        this.MinimumSize = new System.Drawing.Size(620, 200);
        this.Name = "Form1";
        this.Text = "Lens";
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.rayDirectionPhiNumericUpDown)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel drawingPanel;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown rayDirectionPhiNumericUpDown;
    private System.Windows.Forms.Label label1;

  }
}
