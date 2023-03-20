namespace LV_Inspection_System.GUI
{
    partial class Form_BigImage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BigImage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageControl1 = new ImageControlBox.ImageControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.panel1.Controls.Add(this.imageControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(994, 750);
            this.panel1.TabIndex = 0;
            // 
            // imageControl1
            // 
            this.imageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageControl1.DrawRectMode = false;
            this.imageControl1.Image = null;
            this.imageControl1.initialimage = null;
            this.imageControl1.Location = new System.Drawing.Point(0, 0);
            this.imageControl1.Name = "imageControl1";
            this.imageControl1.Origin = new System.Drawing.Point(0, 0);
            this.imageControl1.PanButton = System.Windows.Forms.MouseButtons.Left;
            this.imageControl1.PanMode = true;
            this.imageControl1.ScrollbarsVisible = true;
            this.imageControl1.Size = new System.Drawing.Size(994, 750);
            this.imageControl1.StretchImageToFit = false;
            this.imageControl1.TabIndex = 0;
            this.imageControl1.ZoomFactor = 1D;
            this.imageControl1.ZoomOnMouseWheel = true;
            // 
            // Form_BigImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 750);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_BigImage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form_BigImage_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public ImageControlBox.ImageControl imageControl1;
    }
}