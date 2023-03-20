namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_Display_2
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
            this.pictureBox_1 = new System.Windows.Forms.PictureBox();
            this.pictureBox_2 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.pictureBox_0 = new System.Windows.Forms.PictureBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pictureBox_3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_1
            // 
            this.pictureBox_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.pictureBox_1.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_1.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_1.Name = "pictureBox_1";
            this.pictureBox_1.Size = new System.Drawing.Size(402, 254);
            this.pictureBox_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_1.TabIndex = 1;
            this.pictureBox_1.TabStop = false;
            this.pictureBox_1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_1_Paint);
            this.pictureBox_1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_1_MouseClick);
            // 
            // pictureBox_2
            // 
            this.pictureBox_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.pictureBox_2.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_2.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_2.Name = "pictureBox_2";
            this.pictureBox_2.Size = new System.Drawing.Size(466, 246);
            this.pictureBox_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_2.TabIndex = 2;
            this.pictureBox_2.TabStop = false;
            this.pictureBox_2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_2_Paint);
            this.pictureBox_2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_2_MouseClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(870, 502);
            this.splitContainer1.SplitterDistance = 466;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.pictureBox_0);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pictureBox_2);
            this.splitContainer3.Size = new System.Drawing.Size(466, 502);
            this.splitContainer3.SplitterDistance = 254;
            this.splitContainer3.SplitterWidth = 2;
            this.splitContainer3.TabIndex = 0;
            // 
            // pictureBox_0
            // 
            this.pictureBox_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_0.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_0.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.pictureBox_0.Name = "pictureBox_0";
            this.pictureBox_0.Size = new System.Drawing.Size(466, 254);
            this.pictureBox_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_0.TabIndex = 1;
            this.pictureBox_0.TabStop = false;
            this.pictureBox_0.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_0_Paint);
            this.pictureBox_0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_0_MouseClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pictureBox_1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_3);
            this.splitContainer2.Panel2MinSize = 0;
            this.splitContainer2.Size = new System.Drawing.Size(402, 502);
            this.splitContainer2.SplitterDistance = 254;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 0;
            // 
            // pictureBox_3
            // 
            this.pictureBox_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.pictureBox_3.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_3.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_3.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.pictureBox_3.Name = "pictureBox_3";
            this.pictureBox_3.Size = new System.Drawing.Size(402, 246);
            this.pictureBox_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_3.TabIndex = 3;
            this.pictureBox_3.TabStop = false;
            this.pictureBox_3.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_3_Paint);
            this.pictureBox_3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_3_MouseClick);
            // 
            // Ctr_Display_2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Ctr_Display_2";
            this.Size = new System.Drawing.Size(870, 502);
            this.SizeChanged += new System.EventHandler(this.Ctr_Display_2_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_2)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_0)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox_1;
        public System.Windows.Forms.PictureBox pictureBox_2;
        public System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.SplitContainer splitContainer3;
        public System.Windows.Forms.PictureBox pictureBox_0;
        public System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.PictureBox pictureBox_3;
    }
}
