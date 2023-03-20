namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_Manual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_Manual));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox_Manual = new System.Windows.Forms.PictureBox();
            this.button_SnapShot = new System.Windows.Forms.Button();
            this.button_Manual_Inspection = new System.Windows.Forms.Button();
            this.button_Image_Load = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.radioButton_CAM4 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM5 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM6 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM7 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM3 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM1 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM0 = new System.Windows.Forms.RadioButton();
            this.radioButton_CAM2 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Manual)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox_Manual);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.splitContainer1.Panel2.Controls.Add(this.button_SnapShot);
            this.splitContainer1.Panel2.Controls.Add(this.button_Manual_Inspection);
            this.splitContainer1.Panel2.Controls.Add(this.button_Image_Load);
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM4);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM5);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM6);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM7);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM3);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM1);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM0);
            this.splitContainer1.Panel2.Controls.Add(this.radioButton_CAM2);
            this.splitContainer1.Size = new System.Drawing.Size(759, 479);
            this.splitContainer1.SplitterDistance = 601;
            this.splitContainer1.TabIndex = 0;
            // 
            // pictureBox_Manual
            // 
            this.pictureBox_Manual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.pictureBox_Manual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_Manual.Image = global::LV_Inspection_System.Properties.Resources.Display;
            this.pictureBox_Manual.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Manual.Name = "pictureBox_Manual";
            this.pictureBox_Manual.Size = new System.Drawing.Size(601, 479);
            this.pictureBox_Manual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Manual.TabIndex = 0;
            this.pictureBox_Manual.TabStop = false;
            this.pictureBox_Manual.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_Manual_DragDrop);
            this.pictureBox_Manual.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_Manual_DragEnter);
            this.pictureBox_Manual.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Manual_Paint);
            this.pictureBox_Manual.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Manual_MouseClick);
            // 
            // button_SnapShot
            // 
            this.button_SnapShot.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SnapShot.BackgroundImage")));
            this.button_SnapShot.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_SnapShot.ForeColor = System.Drawing.Color.White;
            this.button_SnapShot.Location = new System.Drawing.Point(9, 85);
            this.button_SnapShot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_SnapShot.Name = "button_SnapShot";
            this.button_SnapShot.Size = new System.Drawing.Size(137, 41);
            this.button_SnapShot.TabIndex = 17;
            this.button_SnapShot.Text = "카메라 촬영";
            this.button_SnapShot.UseVisualStyleBackColor = true;
            this.button_SnapShot.Click += new System.EventHandler(this.button_SnapShot_Click);
            // 
            // button_Manual_Inspection
            // 
            this.button_Manual_Inspection.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Manual_Inspection.BackgroundImage")));
            this.button_Manual_Inspection.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Manual_Inspection.ForeColor = System.Drawing.Color.White;
            this.button_Manual_Inspection.Location = new System.Drawing.Point(9, 180);
            this.button_Manual_Inspection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Manual_Inspection.Name = "button_Manual_Inspection";
            this.button_Manual_Inspection.Size = new System.Drawing.Size(137, 41);
            this.button_Manual_Inspection.TabIndex = 13;
            this.button_Manual_Inspection.Text = "시험 검사";
            this.button_Manual_Inspection.UseVisualStyleBackColor = true;
            this.button_Manual_Inspection.Click += new System.EventHandler(this.button_Manual_Inspection_Click);
            // 
            // button_Image_Load
            // 
            this.button_Image_Load.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Image_Load.BackgroundImage")));
            this.button_Image_Load.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Image_Load.ForeColor = System.Drawing.Color.White;
            this.button_Image_Load.Location = new System.Drawing.Point(9, 133);
            this.button_Image_Load.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Image_Load.Name = "button_Image_Load";
            this.button_Image_Load.Size = new System.Drawing.Size(137, 41);
            this.button_Image_Load.TabIndex = 12;
            this.button_Image_Load.Text = "이미지 열기";
            this.button_Image_Load.UseVisualStyleBackColor = true;
            this.button_Image_Load.Click += new System.EventHandler(this.button_Image_Load_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 228);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(148, 248);
            this.propertyGrid1.TabIndex = 1;
            // 
            // radioButton_CAM4
            // 
            this.radioButton_CAM4.AutoSize = true;
            this.radioButton_CAM4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM4.Location = new System.Drawing.Point(51, 95);
            this.radioButton_CAM4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM4.Name = "radioButton_CAM4";
            this.radioButton_CAM4.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM4.TabIndex = 22;
            this.radioButton_CAM4.Text = "CAM4";
            this.radioButton_CAM4.UseVisualStyleBackColor = true;
            this.radioButton_CAM4.Visible = false;
            this.radioButton_CAM4.CheckedChanged += new System.EventHandler(this.radioButton_CAM4_CheckedChanged);
            // 
            // radioButton_CAM5
            // 
            this.radioButton_CAM5.AutoSize = true;
            this.radioButton_CAM5.Enabled = false;
            this.radioButton_CAM5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM5.Location = new System.Drawing.Point(64, 90);
            this.radioButton_CAM5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM5.Name = "radioButton_CAM5";
            this.radioButton_CAM5.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM5.TabIndex = 20;
            this.radioButton_CAM5.Text = "CAM5";
            this.radioButton_CAM5.UseVisualStyleBackColor = true;
            this.radioButton_CAM5.Visible = false;
            this.radioButton_CAM5.CheckedChanged += new System.EventHandler(this.radioButton_CAM5_CheckedChanged);
            // 
            // radioButton_CAM6
            // 
            this.radioButton_CAM6.AutoSize = true;
            this.radioButton_CAM6.Enabled = false;
            this.radioButton_CAM6.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM6.Location = new System.Drawing.Point(64, 95);
            this.radioButton_CAM6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM6.Name = "radioButton_CAM6";
            this.radioButton_CAM6.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM6.TabIndex = 19;
            this.radioButton_CAM6.Text = "CAM6";
            this.radioButton_CAM6.UseVisualStyleBackColor = true;
            this.radioButton_CAM6.Visible = false;
            this.radioButton_CAM6.CheckedChanged += new System.EventHandler(this.radioButton_CAM6_CheckedChanged);
            // 
            // radioButton_CAM7
            // 
            this.radioButton_CAM7.AutoSize = true;
            this.radioButton_CAM7.Enabled = false;
            this.radioButton_CAM7.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM7.Location = new System.Drawing.Point(84, 87);
            this.radioButton_CAM7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM7.Name = "radioButton_CAM7";
            this.radioButton_CAM7.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM7.TabIndex = 21;
            this.radioButton_CAM7.Text = "CAM7";
            this.radioButton_CAM7.UseVisualStyleBackColor = true;
            this.radioButton_CAM7.Visible = false;
            this.radioButton_CAM7.CheckedChanged += new System.EventHandler(this.radioButton_CAM7_CheckedChanged);
            // 
            // radioButton_CAM3
            // 
            this.radioButton_CAM3.AutoSize = true;
            this.radioButton_CAM3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM3.Location = new System.Drawing.Point(84, 47);
            this.radioButton_CAM3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM3.Name = "radioButton_CAM3";
            this.radioButton_CAM3.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM3.TabIndex = 18;
            this.radioButton_CAM3.Text = "CAM3";
            this.radioButton_CAM3.UseVisualStyleBackColor = true;
            this.radioButton_CAM3.CheckedChanged += new System.EventHandler(this.radioButton_CAM3_CheckedChanged);
            // 
            // radioButton_CAM1
            // 
            this.radioButton_CAM1.AutoSize = true;
            this.radioButton_CAM1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM1.Location = new System.Drawing.Point(13, 47);
            this.radioButton_CAM1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM1.Name = "radioButton_CAM1";
            this.radioButton_CAM1.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM1.TabIndex = 15;
            this.radioButton_CAM1.Text = "CAM1";
            this.radioButton_CAM1.UseVisualStyleBackColor = true;
            this.radioButton_CAM1.CheckedChanged += new System.EventHandler(this.radioButton_CAM1_CheckedChanged);
            // 
            // radioButton_CAM0
            // 
            this.radioButton_CAM0.AutoSize = true;
            this.radioButton_CAM0.Checked = true;
            this.radioButton_CAM0.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM0.Location = new System.Drawing.Point(13, 12);
            this.radioButton_CAM0.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM0.Name = "radioButton_CAM0";
            this.radioButton_CAM0.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM0.TabIndex = 14;
            this.radioButton_CAM0.TabStop = true;
            this.radioButton_CAM0.Text = "CAM0";
            this.radioButton_CAM0.UseVisualStyleBackColor = true;
            this.radioButton_CAM0.CheckedChanged += new System.EventHandler(this.radioButton_CAM0_CheckedChanged);
            // 
            // radioButton_CAM2
            // 
            this.radioButton_CAM2.AutoSize = true;
            this.radioButton_CAM2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_CAM2.Location = new System.Drawing.Point(84, 12);
            this.radioButton_CAM2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_CAM2.Name = "radioButton_CAM2";
            this.radioButton_CAM2.Size = new System.Drawing.Size(62, 21);
            this.radioButton_CAM2.TabIndex = 16;
            this.radioButton_CAM2.Text = "CAM2";
            this.radioButton_CAM2.UseVisualStyleBackColor = true;
            this.radioButton_CAM2.CheckedChanged += new System.EventHandler(this.radioButton_CAM2_CheckedChanged);
            // 
            // Ctr_Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Ctr_Manual";
            this.Size = new System.Drawing.Size(759, 479);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Manual)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.PictureBox pictureBox_Manual;
        private System.Windows.Forms.Button button_SnapShot;
        private System.Windows.Forms.Button button_Manual_Inspection;
        public System.Windows.Forms.RadioButton radioButton_CAM1;
        private System.Windows.Forms.Button button_Image_Load;
        public System.Windows.Forms.RadioButton radioButton_CAM0;
        public System.Windows.Forms.RadioButton radioButton_CAM2;
        public System.Windows.Forms.RadioButton radioButton_CAM4;
        public System.Windows.Forms.RadioButton radioButton_CAM5;
        public System.Windows.Forms.RadioButton radioButton_CAM6;
        public System.Windows.Forms.RadioButton radioButton_CAM7;
        public System.Windows.Forms.RadioButton radioButton_CAM3;
        public System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
