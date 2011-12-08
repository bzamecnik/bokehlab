namespace DDA
{
    partial class Dda3dForm
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
            this.xPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.zPictureBox = new System.Windows.Forms.PictureBox();
            this.yPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.xPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // xPictureBox
            // 
            this.xPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xPictureBox.Location = new System.Drawing.Point(3, 3);
            this.xPictureBox.Name = "xPictureBox";
            this.xPictureBox.Size = new System.Drawing.Size(346, 237);
            this.xPictureBox.TabIndex = 0;
            this.xPictureBox.TabStop = false;
            this.xPictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.zPictureBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.yPictureBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.xPictureBox, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(704, 486);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // zPictureBox
            // 
            this.zPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zPictureBox.Location = new System.Drawing.Point(3, 246);
            this.zPictureBox.Name = "zPictureBox";
            this.zPictureBox.Size = new System.Drawing.Size(346, 237);
            this.zPictureBox.TabIndex = 2;
            this.zPictureBox.TabStop = false;
            // 
            // yPictureBox
            // 
            this.yPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.yPictureBox.Location = new System.Drawing.Point(355, 3);
            this.yPictureBox.Name = "yPictureBox";
            this.yPictureBox.Size = new System.Drawing.Size(346, 237);
            this.yPictureBox.TabIndex = 1;
            this.yPictureBox.TabStop = false;
            // 
            // Dda3dForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 510);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Dda3dForm";
            this.Text = "DDA line rasterization";
            ((System.ComponentModel.ISupportInitialize)(this.xPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox xPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox zPictureBox;
        private System.Windows.Forms.PictureBox yPictureBox;
    }
}

