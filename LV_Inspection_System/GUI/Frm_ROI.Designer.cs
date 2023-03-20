namespace LV_Inspection_System.GUI
{
    partial class Frm_ROI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ROI));
            this.ctr_ROI1 = new LV_Inspection_System.GUI.Control.Ctr_ROI();
            this.SuspendLayout();
            // 
            // ctr_ROI1
            // 
            this.ctr_ROI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctr_ROI1.Location = new System.Drawing.Point(0, 0);
            this.ctr_ROI1.Name = "ctr_ROI1";
            this.ctr_ROI1.Size = new System.Drawing.Size(1444, 480);
            this.ctr_ROI1.TabIndex = 0;
            this.ctr_ROI1.Load += new System.EventHandler(this.ctr_ROI1_Load);
            // 
            // Frm_ROI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 480);
            this.Controls.Add(this.ctr_ROI1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_ROI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ROI 설정";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        public Control.Ctr_ROI ctr_ROI1;
    }
}