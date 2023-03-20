namespace LV_Inspection_System.GUI
{
    partial class Frm_Trackbar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Trackbar));
            this.colorSlider = new ColorSliderJCD.ColorSliderJCD();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // colorSlider
            // 
            this.colorSlider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.colorSlider.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.colorSlider.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.colorSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSlider.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.colorSlider.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.colorSlider.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.colorSlider.ForeColor = System.Drawing.Color.White;
            this.colorSlider.LargeChange = ((uint)(1u));
            this.colorSlider.Location = new System.Drawing.Point(0, 0);
            this.colorSlider.Name = "colorSlider";
            this.colorSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.colorSlider.ScaleDivisions = 10;
            this.colorSlider.ScaleSubDivisions = 5;
            this.colorSlider.ShowDivisionsText = true;
            this.colorSlider.ShowSmallScale = false;
            this.colorSlider.Size = new System.Drawing.Size(48, 200);
            this.colorSlider.SmallChange = ((uint)(1u));
            this.colorSlider.TabIndex = 32;
            this.colorSlider.Text = "colorSlider1";
            this.colorSlider.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSlider.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSlider.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.colorSlider.ThumbSize = new System.Drawing.Size(16, 16);
            this.colorSlider.TickAdd = 0F;
            this.colorSlider.TickColor = System.Drawing.Color.White;
            this.colorSlider.TickDivide = 0F;
            this.colorSlider.Value = 30F;
            this.colorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider_Scroll);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Frm_Trackbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(48, 200);
            this.Controls.Add(this.colorSlider);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(48, 200);
            this.MinimumSize = new System.Drawing.Size(48, 200);
            this.Name = "Frm_Trackbar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.ResumeLayout(false);

        }

        #endregion

        public ColorSliderJCD.ColorSliderJCD colorSlider;
        private System.Windows.Forms.Timer timer1;


    }
}