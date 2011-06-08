namespace BokehLab.RayTracing.Test.GUI
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
            this.drawingPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.label12 = new System.Windows.Forms.Label();
            this.shownParameterComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleCountNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionPhiNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionPhiNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionThetaNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionThetaNumeric)).BeginInit();
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
            this.groupBox1.Controls.Add(this.sampleCountNumeric);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.shownParameterComboBox);
            this.groupBox1.Controls.Add(this.variableParameterComboBox);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.directionPhiNumeric);
            this.groupBox1.Controls.Add(this.positionPhiNumeric);
            this.groupBox1.Controls.Add(this.directionThetaNumeric);
            this.groupBox1.Controls.Add(this.positionThetaNumeric);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label12);
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
            // sampleCountNumeric
            // 
            this.sampleCountNumeric.Location = new System.Drawing.Point(112, 298);
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
            this.label11.Location = new System.Drawing.Point(6, 300);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Number of samples:";
            // 
            // variableParameterComboBox
            // 
            this.variableParameterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.variableParameterComboBox.FormattingEnabled = true;
            this.variableParameterComboBox.Location = new System.Drawing.Point(6, 271);
            this.variableParameterComboBox.Name = "variableParameterComboBox";
            this.variableParameterComboBox.Size = new System.Drawing.Size(121, 21);
            this.variableParameterComboBox.TabIndex = 5;
            this.variableParameterComboBox.SelectedIndexChanged += new System.EventHandler(this.variableParameterComboBox_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 325);
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
            this.directionPhiNumeric.Location = new System.Drawing.Point(51, 232);
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
            this.positionPhiNumeric.Location = new System.Drawing.Point(50, 163);
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
            this.directionThetaNumeric.Location = new System.Drawing.Point(51, 206);
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
            this.positionThetaNumeric.Location = new System.Drawing.Point(50, 137);
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
            this.label9.Location = new System.Drawing.Point(7, 234);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Phi:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Phi:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 255);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Variable parameter:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "LRTF parameters:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 208);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Theta:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Theta:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Position:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Position:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(8, 73);
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
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 356);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Shown parameter:";
            // 
            // shownParameterComboBox
            // 
            this.shownParameterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.shownParameterComboBox.FormattingEnabled = true;
            this.shownParameterComboBox.Location = new System.Drawing.Point(6, 372);
            this.shownParameterComboBox.Name = "shownParameterComboBox";
            this.shownParameterComboBox.Size = new System.Drawing.Size(121, 21);
            this.shownParameterComboBox.TabIndex = 8;
            this.shownParameterComboBox.SelectedIndexChanged += new System.EventHandler(this.shownParameterComboBox_SelectedIndexChanged);
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
            this.Text = "Complex lens";
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel drawingPanel;
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
        private System.Windows.Forms.ComboBox shownParameterComboBox;
        private System.Windows.Forms.Label label12;

    }
}
