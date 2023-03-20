namespace LV_Inspection_System.GUI
{
    partial class Form_Masking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Masking));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button_DELETE = new System.Windows.Forms.Button();
            this.button_CLOSE = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_Size = new System.Windows.Forms.TextBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.button_SAVE = new System.Windows.Forms.Button();
            this.imageBox1 = new Cyotek.Windows.Forms.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button_DELETE);
            this.splitContainer1.Panel1.Controls.Add(this.button_CLOSE);
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.button_SAVE);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.imageBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1080, 613);
            this.splitContainer1.SplitterDistance = 169;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // button_DELETE
            // 
            this.button_DELETE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_DELETE.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_DELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_DELETE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_DELETE.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_DELETE.ForeColor = System.Drawing.Color.White;
            this.button_DELETE.Location = new System.Drawing.Point(0, 465);
            this.button_DELETE.Name = "button_DELETE";
            this.button_DELETE.Size = new System.Drawing.Size(167, 48);
            this.button_DELETE.TabIndex = 57;
            this.button_DELETE.Text = "DELETE MASK";
            this.button_DELETE.UseVisualStyleBackColor = true;
            this.button_DELETE.Click += new System.EventHandler(this.button_DELETE_Click);
            // 
            // button_CLOSE
            // 
            this.button_CLOSE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_CLOSE.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_CLOSE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_CLOSE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_CLOSE.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_CLOSE.ForeColor = System.Drawing.Color.White;
            this.button_CLOSE.Location = new System.Drawing.Point(0, 565);
            this.button_CLOSE.Name = "button_CLOSE";
            this.button_CLOSE.Size = new System.Drawing.Size(167, 48);
            this.button_CLOSE.TabIndex = 56;
            this.button_CLOSE.Text = "CLOSE";
            this.button_CLOSE.UseVisualStyleBackColor = true;
            this.button_CLOSE.Click += new System.EventHandler(this.Button_CLOSE_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Menu;
            this.richTextBox1.Location = new System.Drawing.Point(3, 275);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(164, 185);
            this.richTextBox1.TabIndex = 55;
            this.richTextBox1.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton7);
            this.groupBox3.Controls.Add(this.radioButton5);
            this.groupBox3.Controls.Add(this.radioButton6);
            this.groupBox3.Location = new System.Drawing.Point(3, 165);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(164, 103);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "View";
            // 
            // radioButton7
            // 
            this.radioButton7.AutoSize = true;
            this.radioButton7.Checked = true;
            this.radioButton7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton7.Location = new System.Drawing.Point(14, 72);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(105, 16);
            this.radioButton7.TabIndex = 2;
            this.radioButton7.TabStop = true;
            this.radioButton7.Text = "Merged Image";
            this.radioButton7.UseVisualStyleBackColor = true;
            this.radioButton7.CheckedChanged += new System.EventHandler(this.RadioButton7_CheckedChanged);
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton5.Location = new System.Drawing.Point(14, 23);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(105, 16);
            this.radioButton5.TabIndex = 0;
            this.radioButton5.Text = "Original Image";
            this.radioButton5.UseVisualStyleBackColor = true;
            this.radioButton5.CheckedChanged += new System.EventHandler(this.RadioButton5_CheckedChanged);
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton6.Location = new System.Drawing.Point(14, 47);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(93, 16);
            this.radioButton6.TabIndex = 1;
            this.radioButton6.Text = "Mask Image";
            this.radioButton6.UseVisualStyleBackColor = true;
            this.radioButton6.CheckedChanged += new System.EventHandler(this.RadioButton6_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_Size);
            this.groupBox2.Controls.Add(this.radioButton3);
            this.groupBox2.Controls.Add(this.radioButton4);
            this.groupBox2.Location = new System.Drawing.Point(3, 83);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(164, 76);
            this.groupBox2.TabIndex = 54;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Shape";
            // 
            // textBox_Size
            // 
            this.textBox_Size.Location = new System.Drawing.Point(96, 45);
            this.textBox_Size.Name = "textBox_Size";
            this.textBox_Size.Size = new System.Drawing.Size(52, 21);
            this.textBox_Size.TabIndex = 2;
            this.textBox_Size.Text = "10";
            this.textBox_Size.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_Size.TextChanged += new System.EventHandler(this.TextBox_Size_TextChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton3.Location = new System.Drawing.Point(14, 23);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(79, 16);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Rectangle";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.RadioButton3_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton4.Location = new System.Drawing.Point(14, 47);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(76, 16);
            this.radioButton4.TabIndex = 1;
            this.radioButton4.Text = "Freehand";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.RadioButton4_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 74);
            this.groupBox1.TabIndex = 53;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mode";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton1.Location = new System.Drawing.Point(14, 46);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(138, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Making Area(Green)";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioButton2.Location = new System.Drawing.Point(14, 22);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(133, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Feasible Area(Red)";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // button_SAVE
            // 
            this.button_SAVE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SAVE.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_SAVE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_SAVE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SAVE.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_SAVE.ForeColor = System.Drawing.Color.White;
            this.button_SAVE.Location = new System.Drawing.Point(0, 515);
            this.button_SAVE.Name = "button_SAVE";
            this.button_SAVE.Size = new System.Drawing.Size(167, 48);
            this.button_SAVE.TabIndex = 52;
            this.button_SAVE.Text = "SAVE MASK";
            this.button_SAVE.UseVisualStyleBackColor = true;
            this.button_SAVE.Click += new System.EventHandler(this.Button_SAVE_Click);
            // 
            // imageBox1
            // 
            this.imageBox1.AllowDoubleClick = true;
            this.imageBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox1.Location = new System.Drawing.Point(0, 0);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(907, 613);
            this.imageBox1.TabIndex = 1;
            this.imageBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageBox1_Paint);
            this.imageBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ImageBox1_MouseDoubleClick);
            this.imageBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageBox1_MouseDown);
            this.imageBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageBox1_MouseMove);
            this.imageBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageBox1_MouseUp);
            // 
            // Form_Masking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 613);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Masking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Don\'t Care Areas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Masking_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button_SAVE;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_Size;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private Cyotek.Windows.Forms.ImageBox imageBox1;
        private System.Windows.Forms.Button button_CLOSE;
        private System.Windows.Forms.Button button_DELETE;
    }
}