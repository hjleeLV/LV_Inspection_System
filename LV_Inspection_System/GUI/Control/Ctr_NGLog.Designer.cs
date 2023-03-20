namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_NGLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_NGLog));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox4 = new System.Windows.Forms.ListBox();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.button_ScreenSave = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.imageControl1 = new ImageControlBox.ImageControl();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button_RESET0 = new System.Windows.Forms.Button();
            this.button_RESET1 = new System.Windows.Forms.Button();
            this.button_RESET2 = new System.Windows.Forms.Button();
            this.button_RESET3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(826, 578);
            this.splitContainer1.SplitterDistance = 195;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.button_RESET3, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBox4, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.listBox3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.listBox2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_RESET0, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_RESET1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.button_RESET2, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.08425F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.72161F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(195, 578);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listBox4
            // 
            this.listBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox4.FormattingEnabled = true;
            this.listBox4.ItemHeight = 12;
            this.listBox4.Location = new System.Drawing.Point(3, 448);
            this.listBox4.Name = "listBox4";
            this.listBox4.Size = new System.Drawing.Size(189, 105);
            this.listBox4.TabIndex = 3;
            this.listBox4.Tag = "CAM3";
            this.listBox4.SelectedIndexChanged += new System.EventHandler(this.listBox4_SelectedIndexChanged);
            // 
            // listBox3
            // 
            this.listBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox3.FormattingEnabled = true;
            this.listBox3.ItemHeight = 12;
            this.listBox3.Location = new System.Drawing.Point(3, 276);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(189, 138);
            this.listBox3.TabIndex = 2;
            this.listBox3.Tag = "CAM2";
            this.listBox3.SelectedIndexChanged += new System.EventHandler(this.listBox3_SelectedIndexChanged);
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(3, 110);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(189, 128);
            this.listBox2.TabIndex = 1;
            this.listBox2.Tag = "CAM1";
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(189, 82);
            this.listBox1.TabIndex = 0;
            this.listBox1.Tag = "CAM0";
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.button_ScreenSave);
            this.splitContainer2.Panel1.Controls.Add(this.button_Save);
            this.splitContainer2.Panel1.Controls.Add(this.imageControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Size = new System.Drawing.Size(627, 578);
            this.splitContainer2.SplitterDistance = 499;
            this.splitContainer2.TabIndex = 0;
            // 
            // button_ScreenSave
            // 
            this.button_ScreenSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ScreenSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ScreenSave.Location = new System.Drawing.Point(522, 3);
            this.button_ScreenSave.Name = "button_ScreenSave";
            this.button_ScreenSave.Size = new System.Drawing.Size(88, 23);
            this.button_ScreenSave.TabIndex = 2;
            this.button_ScreenSave.Text = "Screen Save";
            this.button_ScreenSave.UseVisualStyleBackColor = true;
            this.button_ScreenSave.Click += new System.EventHandler(this.button_ScreenSave_Click);
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Save.Location = new System.Drawing.Point(440, 3);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(80, 23);
            this.button_Save.TabIndex = 1;
            this.button_Save.Text = "Image Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // imageControl1
            // 
            this.imageControl1.Cursor = System.Windows.Forms.Cursors.Hand;
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
            this.imageControl1.Size = new System.Drawing.Size(627, 499);
            this.imageControl1.StretchImageToFit = false;
            this.imageControl1.TabIndex = 0;
            this.imageControl1.ZoomFactor = 1D;
            this.imageControl1.ZoomOnMouseWheel = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(627, 75);
            this.dataGridView1.TabIndex = 0;
            // 
            // button_RESET0
            // 
            this.button_RESET0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RESET0.BackgroundImage")));
            this.button_RESET0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_RESET0.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_RESET0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_RESET0.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_RESET0.ForeColor = System.Drawing.Color.White;
            this.button_RESET0.Location = new System.Drawing.Point(3, 91);
            this.button_RESET0.Name = "button_RESET0";
            this.button_RESET0.Size = new System.Drawing.Size(189, 13);
            this.button_RESET0.TabIndex = 56;
            this.button_RESET0.Text = "Reset";
            this.button_RESET0.UseVisualStyleBackColor = true;
            this.button_RESET0.Click += new System.EventHandler(this.button_RESET0_Click);
            // 
            // button_RESET1
            // 
            this.button_RESET1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RESET1.BackgroundImage")));
            this.button_RESET1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_RESET1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_RESET1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_RESET1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_RESET1.ForeColor = System.Drawing.Color.White;
            this.button_RESET1.Location = new System.Drawing.Point(3, 244);
            this.button_RESET1.Name = "button_RESET1";
            this.button_RESET1.Size = new System.Drawing.Size(189, 26);
            this.button_RESET1.TabIndex = 57;
            this.button_RESET1.Text = "Reset";
            this.button_RESET1.UseVisualStyleBackColor = true;
            this.button_RESET1.Click += new System.EventHandler(this.button_RESET1_Click);
            // 
            // button_RESET2
            // 
            this.button_RESET2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RESET2.BackgroundImage")));
            this.button_RESET2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_RESET2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_RESET2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_RESET2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_RESET2.ForeColor = System.Drawing.Color.White;
            this.button_RESET2.Location = new System.Drawing.Point(3, 420);
            this.button_RESET2.Name = "button_RESET2";
            this.button_RESET2.Size = new System.Drawing.Size(189, 22);
            this.button_RESET2.TabIndex = 58;
            this.button_RESET2.Text = "Reset";
            this.button_RESET2.UseVisualStyleBackColor = true;
            this.button_RESET2.Click += new System.EventHandler(this.button_RESET2_Click);
            // 
            // button_RESET3
            // 
            this.button_RESET3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RESET3.BackgroundImage")));
            this.button_RESET3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_RESET3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_RESET3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_RESET3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_RESET3.ForeColor = System.Drawing.Color.White;
            this.button_RESET3.Location = new System.Drawing.Point(3, 559);
            this.button_RESET3.Name = "button_RESET3";
            this.button_RESET3.Size = new System.Drawing.Size(189, 16);
            this.button_RESET3.TabIndex = 59;
            this.button_RESET3.Text = "Reset";
            this.button_RESET3.UseVisualStyleBackColor = true;
            this.button_RESET3.Click += new System.EventHandler(this.button_RESET3_Click);
            // 
            // Ctr_NGLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Ctr_NGLog";
            this.Size = new System.Drawing.Size(826, 578);
            this.SizeChanged += new System.EventHandler(this.Ctr_NGLog_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listBox4;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ImageControlBox.ImageControl imageControl1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_ScreenSave;
        private System.Windows.Forms.Button button_RESET3;
        private System.Windows.Forms.Button button_RESET0;
        private System.Windows.Forms.Button button_RESET1;
        private System.Windows.Forms.Button button_RESET2;
    }
}
