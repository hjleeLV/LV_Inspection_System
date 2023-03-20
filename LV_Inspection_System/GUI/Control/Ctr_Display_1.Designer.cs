namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_Display_1
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_Display_1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox_0 = new System.Windows.Forms.PictureBox();
            this.pictureBox_1 = new System.Windows.Forms.PictureBox();
            this.pictureBox_2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_3 = new System.Windows.Forms.PictureBox();
            this.pictureBox_4 = new System.Windows.Forms.PictureBox();
            this.pictureBox_5 = new System.Windows.Forms.PictureBox();
            this.pictureBox_6 = new System.Windows.Forms.PictureBox();
            this.pictureBox_7 = new System.Windows.Forms.PictureBox();
            this.pictureBox_Main = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Main)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox_Main);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(870, 502);
            this.splitContainer1.SplitterDistance = 82;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_0);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_1);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_2);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_3);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_4);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_5);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_6);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox_7);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(870, 82);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // pictureBox_0
            // 
            this.pictureBox_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_0.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_0.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.pictureBox_0.Name = "pictureBox_0";
            this.pictureBox_0.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_0.TabIndex = 0;
            this.pictureBox_0.TabStop = false;
            this.pictureBox_0.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_0_Paint);
            this.pictureBox_0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_0_MouseClick);
            // 
            // pictureBox_1
            // 
            this.pictureBox_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_1.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_1.Location = new System.Drawing.Point(119, 0);
            this.pictureBox_1.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_1.Name = "pictureBox_1";
            this.pictureBox_1.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_1.TabIndex = 1;
            this.pictureBox_1.TabStop = false;
            this.pictureBox_1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_1_Paint);
            this.pictureBox_1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_1_MouseClick);
            // 
            // pictureBox_2
            // 
            this.pictureBox_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_2.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_2.Location = new System.Drawing.Point(238, 0);
            this.pictureBox_2.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_2.Name = "pictureBox_2";
            this.pictureBox_2.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_2.TabIndex = 2;
            this.pictureBox_2.TabStop = false;
            this.pictureBox_2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_2_Paint);
            this.pictureBox_2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_2_MouseClick);
            // 
            // pictureBox_3
            // 
            this.pictureBox_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_3.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_3.Location = new System.Drawing.Point(357, 0);
            this.pictureBox_3.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_3.Name = "pictureBox_3";
            this.pictureBox_3.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_3.TabIndex = 3;
            this.pictureBox_3.TabStop = false;
            this.pictureBox_3.Visible = false;
            this.pictureBox_3.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_3_Paint);
            this.pictureBox_3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_3_MouseClick);
            // 
            // pictureBox_4
            // 
            this.pictureBox_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_4.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_4.Location = new System.Drawing.Point(476, 0);
            this.pictureBox_4.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_4.Name = "pictureBox_4";
            this.pictureBox_4.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_4.TabIndex = 4;
            this.pictureBox_4.TabStop = false;
            this.pictureBox_4.Visible = false;
            this.pictureBox_4.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_4_Paint);
            this.pictureBox_4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_4_MouseClick);
            // 
            // pictureBox_5
            // 
            this.pictureBox_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_5.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_5.Location = new System.Drawing.Point(595, 0);
            this.pictureBox_5.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_5.Name = "pictureBox_5";
            this.pictureBox_5.Size = new System.Drawing.Size(123, 80);
            this.pictureBox_5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_5.TabIndex = 5;
            this.pictureBox_5.TabStop = false;
            this.pictureBox_5.Visible = false;
            this.pictureBox_5.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_5_Paint);
            this.pictureBox_5.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_5_MouseClick);
            // 
            // pictureBox_6
            // 
            this.pictureBox_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_6.BackgroundImage")));
            this.pictureBox_6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_6.Location = new System.Drawing.Point(720, 0);
            this.pictureBox_6.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.pictureBox_6.Name = "pictureBox_6";
            this.pictureBox_6.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_6.TabIndex = 6;
            this.pictureBox_6.TabStop = false;
            this.pictureBox_6.Visible = false;
            this.pictureBox_6.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_6_Paint);
            this.pictureBox_6.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_6_MouseClick);
            // 
            // pictureBox_7
            // 
            this.pictureBox_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_7.BackgroundImage")));
            this.pictureBox_7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_7.Location = new System.Drawing.Point(0, 80);
            this.pictureBox_7.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_7.Name = "pictureBox_7";
            this.pictureBox_7.Size = new System.Drawing.Size(107, 80);
            this.pictureBox_7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_7.TabIndex = 7;
            this.pictureBox_7.TabStop = false;
            this.pictureBox_7.Visible = false;
            this.pictureBox_7.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_7_Paint);
            this.pictureBox_7.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_7_MouseClick);
            // 
            // pictureBox_Main
            // 
            this.pictureBox_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_Main.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_Main.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_Main.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Main.Name = "pictureBox_Main";
            this.pictureBox_Main.Size = new System.Drawing.Size(870, 419);
            this.pictureBox_Main.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Main.TabIndex = 1;
            this.pictureBox_Main.TabStop = false;
            this.pictureBox_Main.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Main_Paint);
            this.pictureBox_Main.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Main_MouseClick);
            // 
            // Ctr_Display_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Ctr_Display_1";
            this.Size = new System.Drawing.Size(870, 502);
            this.SizeChanged += new System.EventHandler(this.Ctr_Display_1_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Main)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.PictureBox pictureBox_0;
        public System.Windows.Forms.PictureBox pictureBox_1;
        public System.Windows.Forms.PictureBox pictureBox_2;
        public System.Windows.Forms.PictureBox pictureBox_3;
        public System.Windows.Forms.PictureBox pictureBox_4;
        public System.Windows.Forms.PictureBox pictureBox_5;
        public System.Windows.Forms.PictureBox pictureBox_6;
        public System.Windows.Forms.PictureBox pictureBox_7;
        public System.Windows.Forms.PictureBox pictureBox_Main;
    }
}
