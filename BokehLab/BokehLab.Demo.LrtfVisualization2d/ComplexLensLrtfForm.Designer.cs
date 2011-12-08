namespace BokehLab.Demo.LrtfVisualization2d
{
    partial class ComplexLensLrtfForm
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
            this.posThetaPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.sampleCountNumeric = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.variableParameterComboBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.directionPhiNumeric = new System.Windows.Forms.NumericUpDown();
            this.positionPhiNumeric = new System.Windows.Forms.NumericUpDown();
            this.directionThetaNumeric = new System.Windows.Forms.NumericUpDown();
            this.positionThetaNumeric = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dirPhiPanel = new System.Windows.Forms.Panel();
            this.dirThetaPanel = new System.Windows.Forms.Panel();
            this.posPhiPanel = new System.Windows.Forms.Panel();
            this.lensComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionPhiNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionPhiNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionThetaNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionThetaNumeric)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // posThetaPanel
            // 
            this.posThetaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.posThetaPanel.Location = new System.Drawing.Point(5, 5);
            this.posThetaPanel.Name = "posThetaPanel";
            this.posThetaPanel.Size = new System.Drawing.Size(274, 202);
            this.posThetaPanel.TabIndex = 0;
            this.posThetaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.posThetaPanel_Paint);
            this.posThetaPanel.Resize += new System.EventHandler(this.DrawingPanel_Resize);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lensComboBox);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.sampleCountNumeric);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.variableParameterComboBox);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.directionPhiNumeric);
            this.groupBox1.Controls.Add(this.positionPhiNumeric);
            this.groupBox1.Controls.Add(this.directionThetaNumeric);
            this.groupBox1.Controls.Add(this.positionThetaNumeric);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 415);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(82, 370);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(64, 13);
            this.label16.TabIndex = 17;
            this.label16.Text = "direction phi";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 370);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(74, 13);
            this.label15.TabIndex = 17;
            this.label15.Text = "direction theta";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(82, 347);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(60, 13);
            this.label14.TabIndex = 17;
            this.label14.Text = "position phi";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 347);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 17;
            this.label13.Text = "position theta";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 325);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Outgoing ray plots:";
            // 
            // sampleCountNumeric
            // 
            this.sampleCountNumeric.Location = new System.Drawing.Point(111, 268);
            this.sampleCountNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.sampleCountNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sampleCountNumeric.Name = "sampleCountNumeric";
            this.sampleCountNumeric.Size = new System.Drawing.Size(54, 20);
            this.sampleCountNumeric.TabIndex = 6;
            this.sampleCountNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sampleCountNumeric.ValueChanged += new System.EventHandler(this.sampleCountNumeric_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 270);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Number of samples:";
            // 
            // variableParameterComboBox
            // 
            this.variableParameterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.variableParameterComboBox.FormattingEnabled = true;
            this.variableParameterComboBox.Location = new System.Drawing.Point(5, 241);
            this.variableParameterComboBox.Name = "variableParameterComboBox";
            this.variableParameterComboBox.Size = new System.Drawing.Size(121, 21);
            this.variableParameterComboBox.TabIndex = 5;
            this.variableParameterComboBox.SelectedIndexChanged += new System.EventHandler(this.variableParameterComboBox_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, 295);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Recompute!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // directionPhiNumeric
            // 
            this.directionPhiNumeric.DecimalPlaces = 3;
            this.directionPhiNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            this.directionPhiNumeric.Location = new System.Drawing.Point(50, 202);
            this.directionPhiNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.directionPhiNumeric.Name = "directionPhiNumeric";
            this.directionPhiNumeric.Size = new System.Drawing.Size(54, 20);
            this.directionPhiNumeric.TabIndex = 4;
            this.directionPhiNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.directionPhiNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // positionPhiNumeric
            // 
            this.positionPhiNumeric.DecimalPlaces = 3;
            this.positionPhiNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            this.positionPhiNumeric.Location = new System.Drawing.Point(49, 133);
            this.positionPhiNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.positionPhiNumeric.Name = "positionPhiNumeric";
            this.positionPhiNumeric.Size = new System.Drawing.Size(54, 20);
            this.positionPhiNumeric.TabIndex = 2;
            this.positionPhiNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.positionPhiNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // directionThetaNumeric
            // 
            this.directionThetaNumeric.DecimalPlaces = 3;
            this.directionThetaNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            this.directionThetaNumeric.Location = new System.Drawing.Point(50, 176);
            this.directionThetaNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.directionThetaNumeric.Name = "directionThetaNumeric";
            this.directionThetaNumeric.Size = new System.Drawing.Size(54, 20);
            this.directionThetaNumeric.TabIndex = 3;
            this.directionThetaNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.directionThetaNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // positionThetaNumeric
            // 
            this.positionThetaNumeric.DecimalPlaces = 3;
            this.positionThetaNumeric.Increment = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            this.positionThetaNumeric.Location = new System.Drawing.Point(49, 107);
            this.positionThetaNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.positionThetaNumeric.Name = "positionThetaNumeric";
            this.positionThetaNumeric.Size = new System.Drawing.Size(54, 20);
            this.positionThetaNumeric.TabIndex = 1;
            this.positionThetaNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.positionThetaNumeric.ValueChanged += new System.EventHandler(this.SceneControlsValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 204);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Phi:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Phi:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 225);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Variable parameter:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "LRTF parameters:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 178);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Theta:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 109);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Theta:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Direction:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Position:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(7, 43);
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
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(780, 421);
            this.splitContainer1.SplitterDistance = 211;
            this.splitContainer1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dirPhiPanel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dirThetaPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.posPhiPanel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.posThetaPanel, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(-1, -1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(566, 422);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // dirPhiPanel
            // 
            this.dirPhiPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dirPhiPanel.Location = new System.Drawing.Point(287, 215);
            this.dirPhiPanel.Name = "dirPhiPanel";
            this.dirPhiPanel.Size = new System.Drawing.Size(274, 202);
            this.dirPhiPanel.TabIndex = 3;
            this.dirPhiPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.dirPhiPanel_Paint);
            this.dirPhiPanel.Resize += new System.EventHandler(this.DrawingPanel_Resize);
            // 
            // dirThetaPanel
            // 
            this.dirThetaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dirThetaPanel.Location = new System.Drawing.Point(5, 215);
            this.dirThetaPanel.Name = "dirThetaPanel";
            this.dirThetaPanel.Size = new System.Drawing.Size(274, 202);
            this.dirThetaPanel.TabIndex = 2;
            this.dirThetaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.dirThetaPanel_Paint);
            this.dirThetaPanel.Resize += new System.EventHandler(this.DrawingPanel_Resize);
            // 
            // posPhiPanel
            // 
            this.posPhiPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.posPhiPanel.Location = new System.Drawing.Point(287, 5);
            this.posPhiPanel.Name = "posPhiPanel";
            this.posPhiPanel.Size = new System.Drawing.Size(274, 202);
            this.posPhiPanel.TabIndex = 1;
            this.posPhiPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.posPhiPanel_Paint);
            this.posPhiPanel.Resize += new System.EventHandler(this.DrawingPanel_Resize);
            // 
            // lensComboBox
            // 
            this.lensComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lensComboBox.FormattingEnabled = true;
            this.lensComboBox.Location = new System.Drawing.Point(51, 17);
            this.lensComboBox.Name = "lensComboBox";
            this.lensComboBox.Size = new System.Drawing.Size(121, 21);
            this.lensComboBox.TabIndex = 8;
            this.lensComboBox.SelectedIndexChanged += new System.EventHandler(this.lensComboBox_SelectedIndexChanged);
            // 
            // ComplexLensLrtfForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 446);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(620, 200);
            this.Name = "ComplexLensLrtfForm";
            this.Text = "BokehLab - Lens Ray Transfer Function of a complex lens";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionPhiNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionPhiNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionThetaNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionThetaNumeric)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel posThetaPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NumericUpDown positionThetaNumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox variableParameterComboBox;
        private System.Windows.Forms.NumericUpDown directionPhiNumeric;
        private System.Windows.Forms.NumericUpDown positionPhiNumeric;
        private System.Windows.Forms.NumericUpDown directionThetaNumeric;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown sampleCountNumeric;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel dirPhiPanel;
        private System.Windows.Forms.Panel dirThetaPanel;
        private System.Windows.Forms.Panel posPhiPanel;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox lensComboBox;

    }
}
