namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_Model
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
            this.label6 = new System.Windows.Forms.Label();
            this.cmbModels = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lstModels = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Restore = new System.Windows.Forms.Button();
            this.cmdNew = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.button_BACKUP = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.comboBox_Language = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ctr_History1 = new LV_Inspection_System.GUI.Control.Ctr_History();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Title = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 17);
            this.label6.TabIndex = 67;
            this.label6.Text = "모델 :";
            // 
            // cmbModels
            // 
            this.cmbModels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbModels.FormattingEnabled = true;
            this.cmbModels.Location = new System.Drawing.Point(14, 74);
            this.cmbModels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbModels.Name = "cmbModels";
            this.cmbModels.Size = new System.Drawing.Size(242, 25);
            this.cmbModels.TabIndex = 66;
            this.cmbModels.SelectedIndexChanged += new System.EventHandler(this.cmbModels_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.lstModels);
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Controls.Add(this.button_Refresh);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(3, 107);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(381, 536);
            this.groupBox4.TabIndex = 65;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "저장된 모델";
            // 
            // lstModels
            // 
            this.lstModels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstModels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lstModels.ForeColor = System.Drawing.Color.White;
            this.lstModels.FormattingEnabled = true;
            this.lstModels.ItemHeight = 17;
            this.lstModels.Location = new System.Drawing.Point(11, 57);
            this.lstModels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstModels.Name = "lstModels";
            this.lstModels.Size = new System.Drawing.Size(356, 361);
            this.lstModels.TabIndex = 54;
            this.lstModels.SelectedIndexChanged += new System.EventHandler(this.lstModels_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button_Restore);
            this.groupBox1.Controls.Add(this.cmdNew);
            this.groupBox1.Controls.Add(this.cmdDelete);
            this.groupBox1.Controls.Add(this.button_BACKUP);
            this.groupBox1.Controls.Add(this.cmdSave);
            this.groupBox1.Controls.Add(this.cmdLoad);
            this.groupBox1.Location = new System.Drawing.Point(11, 426);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 103);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            // 
            // button_Restore
            // 
            this.button_Restore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Restore.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Restore.ForeColor = System.Drawing.Color.White;
            this.button_Restore.Location = new System.Drawing.Point(110, 57);
            this.button_Restore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Restore.Name = "button_Restore";
            this.button_Restore.Size = new System.Drawing.Size(97, 40);
            this.button_Restore.TabIndex = 63;
            this.button_Restore.Text = "복원";
            this.button_Restore.UseVisualStyleBackColor = true;
            this.button_Restore.Click += new System.EventHandler(this.button_Restore_Click);
            // 
            // cmdNew
            // 
            this.cmdNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdNew.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.cmdNew.ForeColor = System.Drawing.Color.White;
            this.cmdNew.Location = new System.Drawing.Point(6, 15);
            this.cmdNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdNew.Name = "cmdNew";
            this.cmdNew.Size = new System.Drawing.Size(97, 40);
            this.cmdNew.TabIndex = 55;
            this.cmdNew.Text = "모델 생성";
            this.cmdNew.UseVisualStyleBackColor = true;
            this.cmdNew.Click += new System.EventHandler(this.cmdNew_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDelete.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.cmdDelete.ForeColor = System.Drawing.Color.White;
            this.cmdDelete.Location = new System.Drawing.Point(110, 15);
            this.cmdDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(97, 40);
            this.cmdDelete.TabIndex = 56;
            this.cmdDelete.Text = "모델 삭제";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // button_BACKUP
            // 
            this.button_BACKUP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_BACKUP.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_BACKUP.ForeColor = System.Drawing.Color.White;
            this.button_BACKUP.Location = new System.Drawing.Point(6, 57);
            this.button_BACKUP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_BACKUP.Name = "button_BACKUP";
            this.button_BACKUP.Size = new System.Drawing.Size(97, 40);
            this.button_BACKUP.TabIndex = 62;
            this.button_BACKUP.Text = "백업";
            this.button_BACKUP.UseVisualStyleBackColor = true;
            this.button_BACKUP.Click += new System.EventHandler(this.button_BACKUP_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSave.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.cmdSave.ForeColor = System.Drawing.Color.White;
            this.cmdSave.Location = new System.Drawing.Point(214, 15);
            this.cmdSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(135, 40);
            this.cmdSave.TabIndex = 60;
            this.cmdSave.Text = "모델 저장";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdLoad
            // 
            this.cmdLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoad.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.cmdLoad.ForeColor = System.Drawing.Color.White;
            this.cmdLoad.Location = new System.Drawing.Point(214, 57);
            this.cmdLoad.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(135, 40);
            this.cmdLoad.TabIndex = 61;
            this.cmdLoad.Text = "불러오기";
            this.cmdLoad.UseVisualStyleBackColor = true;
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // button_Refresh
            // 
            this.button_Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Refresh.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Refresh.ForeColor = System.Drawing.Color.White;
            this.button_Refresh.Location = new System.Drawing.Point(298, 26);
            this.button_Refresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(70, 30);
            this.button_Refresh.TabIndex = 63;
            this.button_Refresh.Text = "Refresh";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 17);
            this.label7.TabIndex = 59;
            this.label7.Text = "모델 리스트 :";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox_Title);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox_Language);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.cmbModels);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(764, 643);
            this.splitContainer1.SplitterDistance = 393;
            this.splitContainer1.TabIndex = 69;
            this.splitContainer1.TabStop = false;
            // 
            // comboBox_Language
            // 
            this.comboBox_Language.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Language.FormattingEnabled = true;
            this.comboBox_Language.Items.AddRange(new object[] {
            "KOREAN",
            "ENGLISH",
            "CHINESE"});
            this.comboBox_Language.Location = new System.Drawing.Point(262, 74);
            this.comboBox_Language.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_Language.Name = "comboBox_Language";
            this.comboBox_Language.Size = new System.Drawing.Size(122, 25);
            this.comboBox_Language.TabIndex = 69;
            this.comboBox_Language.Text = "KOREAN";
            this.comboBox_Language.SelectedIndexChanged += new System.EventHandler(this.comboBox_Language_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(268, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 68;
            this.label1.Text = "언어 :";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ctr_History1);
            this.splitContainer2.Panel1MinSize = 1;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Size = new System.Drawing.Size(367, 643);
            this.splitContainer2.SplitterDistance = 300;
            this.splitContainer2.TabIndex = 1;
            this.splitContainer2.TabStop = false;
            this.splitContainer2.SizeChanged += new System.EventHandler(this.splitContainer2_SizeChanged);
            // 
            // ctr_History1
            // 
            this.ctr_History1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctr_History1.Location = new System.Drawing.Point(0, 0);
            this.ctr_History1.m_SetLanguage = -1;
            this.ctr_History1.Name = "ctr_History1";
            this.ctr_History1.Size = new System.Drawing.Size(300, 643);
            this.ctr_History1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(63, 643);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 70;
            this.label2.Text = "Title :";
            // 
            // textBox_Title
            // 
            this.textBox_Title.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold);
            this.textBox_Title.Location = new System.Drawing.Point(61, 12);
            this.textBox_Title.Name = "textBox_Title";
            this.textBox_Title.Size = new System.Drawing.Size(323, 25);
            this.textBox_Title.TabIndex = 71;
            this.textBox_Title.Text = "Cosmetic Check Machine #3-2";
            this.textBox_Title.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_Title_KeyUp);
            // 
            // Ctr_Model
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Ctr_Model";
            this.Size = new System.Drawing.Size(764, 643);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox cmbModels;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.ListBox lstModels;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.Button cmdNew;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.Button cmdSave;
        public Ctr_History ctr_History1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ComboBox comboBox_Language;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_BACKUP;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Restore;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox textBox_Title;
    }
}
