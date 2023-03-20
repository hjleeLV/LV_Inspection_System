namespace LV_Inspection_System.GUI
{
    partial class Frm_DragImageControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_DragImageControl));
            this.pictureBox_Stop = new System.Windows.Forms.PictureBox();
            this.pictureBox_Next = new System.Windows.Forms.PictureBox();
            this.pictureBox_Pre = new System.Windows.Forms.PictureBox();
            this.listBox_Images = new System.Windows.Forms.ListBox();
            this.richTextBox_ImageName = new System.Windows.Forms.RichTextBox();
            this.timer_Close = new System.Windows.Forms.Timer(this.components);
            this.checkBox_AUTO = new System.Windows.Forms.CheckBox();
            this.timer_Auto = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Next)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Pre)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Stop
            // 
            this.pictureBox_Stop.Image = global::LV_Inspection_System.Properties.Resources.icons8_stop_64;
            this.pictureBox_Stop.Location = new System.Drawing.Point(59, 4);
            this.pictureBox_Stop.Name = "pictureBox_Stop";
            this.pictureBox_Stop.Size = new System.Drawing.Size(49, 36);
            this.pictureBox_Stop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Stop.TabIndex = 2;
            this.pictureBox_Stop.TabStop = false;
            this.pictureBox_Stop.Visible = false;
            this.pictureBox_Stop.Click += new System.EventHandler(this.pictureBox_Stop_Click);
            // 
            // pictureBox_Next
            // 
            this.pictureBox_Next.Image = global::LV_Inspection_System.Properties.Resources.right_down;
            this.pictureBox_Next.Location = new System.Drawing.Point(59, 4);
            this.pictureBox_Next.Name = "pictureBox_Next";
            this.pictureBox_Next.Size = new System.Drawing.Size(49, 36);
            this.pictureBox_Next.TabIndex = 1;
            this.pictureBox_Next.TabStop = false;
            this.pictureBox_Next.Click += new System.EventHandler(this.pictureBox_Next_Click);
            // 
            // pictureBox_Pre
            // 
            this.pictureBox_Pre.Image = global::LV_Inspection_System.Properties.Resources.left_down;
            this.pictureBox_Pre.Location = new System.Drawing.Point(2, 4);
            this.pictureBox_Pre.Name = "pictureBox_Pre";
            this.pictureBox_Pre.Size = new System.Drawing.Size(49, 36);
            this.pictureBox_Pre.TabIndex = 0;
            this.pictureBox_Pre.TabStop = false;
            this.pictureBox_Pre.Click += new System.EventHandler(this.pictureBox_Pre_Click);
            // 
            // listBox_Images
            // 
            this.listBox_Images.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_Images.FormattingEnabled = true;
            this.listBox_Images.ItemHeight = 12;
            this.listBox_Images.Location = new System.Drawing.Point(2, 44);
            this.listBox_Images.Name = "listBox_Images";
            this.listBox_Images.Size = new System.Drawing.Size(387, 148);
            this.listBox_Images.Sorted = true;
            this.listBox_Images.TabIndex = 3;
            this.listBox_Images.SelectedIndexChanged += new System.EventHandler(this.listBox_Images_SelectedIndexChanged);
            // 
            // richTextBox_ImageName
            // 
            this.richTextBox_ImageName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_ImageName.Location = new System.Drawing.Point(177, 4);
            this.richTextBox_ImageName.Name = "richTextBox_ImageName";
            this.richTextBox_ImageName.Size = new System.Drawing.Size(212, 38);
            this.richTextBox_ImageName.TabIndex = 4;
            this.richTextBox_ImageName.Text = "";
            // 
            // timer_Close
            // 
            this.timer_Close.Interval = 1000;
            this.timer_Close.Tick += new System.EventHandler(this.timer_Close_Tick);
            // 
            // checkBox_AUTO
            // 
            this.checkBox_AUTO.AutoSize = true;
            this.checkBox_AUTO.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_AUTO.Location = new System.Drawing.Point(114, 15);
            this.checkBox_AUTO.Name = "checkBox_AUTO";
            this.checkBox_AUTO.Size = new System.Drawing.Size(57, 16);
            this.checkBox_AUTO.TabIndex = 5;
            this.checkBox_AUTO.Text = "AUTO";
            this.checkBox_AUTO.UseVisualStyleBackColor = false;
            this.checkBox_AUTO.CheckedChanged += new System.EventHandler(this.checkBox_AUTO_CheckedChanged);
            // 
            // timer_Auto
            // 
            this.timer_Auto.Tick += new System.EventHandler(this.timer_Auto_Tick);
            // 
            // Frm_DragImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(391, 194);
            this.Controls.Add(this.checkBox_AUTO);
            this.Controls.Add(this.listBox_Images);
            this.Controls.Add(this.richTextBox_ImageName);
            this.Controls.Add(this.pictureBox_Next);
            this.Controls.Add(this.pictureBox_Pre);
            this.Controls.Add(this.pictureBox_Stop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_DragImageControl";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.HotPink;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Next)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Pre)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Pre;
        private System.Windows.Forms.PictureBox pictureBox_Next;
        private System.Windows.Forms.PictureBox pictureBox_Stop;
        private System.Windows.Forms.ListBox listBox_Images;
        private System.Windows.Forms.RichTextBox richTextBox_ImageName;
        private System.Windows.Forms.Timer timer_Close;
        private System.Windows.Forms.CheckBox checkBox_AUTO;
        private System.Windows.Forms.Timer timer_Auto;
    }
}