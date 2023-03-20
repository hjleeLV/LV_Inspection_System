namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_ROI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_ROI));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBox_AutoInspection = new System.Windows.Forms.CheckBox();
            this.button_ParamView = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.CheckedListBox();
            this.button_ROTATION_CAL = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_CAMPOSITION = new System.Windows.Forms.ComboBox();
            this.comboBox_TABLETYPE = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.button_INSPECTION = new System.Windows.Forms.Button();
            this.button_SAVE = new System.Windows.Forms.Button();
            this.button_LOAD = new System.Windows.Forms.Button();
            this.button_SNAPSHOT = new System.Windows.Forms.Button();
            this.button_OPEN = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer_RIGHT = new System.Windows.Forms.SplitContainer();
            this.splitContainer_RIGHT2 = new System.Windows.Forms.SplitContainer();
            this.pictureBox_Image = new System.Windows.Forms.PictureBox();
            this.pictureBox_RImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_RIGHT)).BeginInit();
            this.splitContainer_RIGHT.Panel1.SuspendLayout();
            this.splitContainer_RIGHT.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_RIGHT2)).BeginInit();
            this.splitContainer_RIGHT2.Panel1.SuspendLayout();
            this.splitContainer_RIGHT2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RImage)).BeginInit();
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
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.checkBox_AutoInspection);
            this.splitContainer1.Panel1.Controls.Add(this.button_ParamView);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.button_ROTATION_CAL);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox_CAMPOSITION);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox_TABLETYPE);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.radioButton2);
            this.splitContainer1.Panel1.Controls.Add(this.radioButton1);
            this.splitContainer1.Panel1.Controls.Add(this.button_INSPECTION);
            this.splitContainer1.Panel1.Controls.Add(this.button_SAVE);
            this.splitContainer1.Panel1.Controls.Add(this.button_LOAD);
            this.splitContainer1.Panel1.Controls.Add(this.button_SNAPSHOT);
            this.splitContainer1.Panel1.Controls.Add(this.button_OPEN);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(932, 496);
            this.splitContainer1.SplitterDistance = 284;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // checkBox_AutoInspection
            // 
            this.checkBox_AutoInspection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_AutoInspection.AutoSize = true;
            this.checkBox_AutoInspection.Location = new System.Drawing.Point(7, 361);
            this.checkBox_AutoInspection.Name = "checkBox_AutoInspection";
            this.checkBox_AutoInspection.Size = new System.Drawing.Size(128, 16);
            this.checkBox_AutoInspection.TabIndex = 63;
            this.checkBox_AutoInspection.Text = "ROI Click 자동시험";
            this.checkBox_AutoInspection.UseVisualStyleBackColor = true;
            this.checkBox_AutoInspection.CheckedChanged += new System.EventHandler(this.checkBox_AutoInspection_CheckedChanged);
            // 
            // button_ParamView
            // 
            this.button_ParamView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ParamView.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_ParamView.ForeColor = System.Drawing.Color.Black;
            this.button_ParamView.Location = new System.Drawing.Point(176, 356);
            this.button_ParamView.Name = "button_ParamView";
            this.button_ParamView.Size = new System.Drawing.Size(105, 26);
            this.button_ParamView.TabIndex = 62;
            this.button_ParamView.Text = "Param. View";
            this.button_ParamView.UseVisualStyleBackColor = true;
            this.button_ParamView.Visible = false;
            this.button_ParamView.Click += new System.EventHandler(this.button_ParamView_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "제품 전체 영역 설정",
            "ROI #01 측정 영역 설정",
            "ROI #02 측정 영역 설정",
            "ROI #03 측정 영역 설정",
            "ROI #04 측정 영역 설정",
            "ROI #05 측정 영역 설정",
            "ROI #06 측정 영역 설정",
            "ROI #07 측정 영역 설정",
            "ROI #08 측정 영역 설정",
            "ROI #09 측정 영역 설정",
            "ROI #10 측정 영역 설정",
            "ROI #11 측정 영역 설정",
            "ROI #12 측정 영역 설정",
            "ROI #13 측정 영역 설정",
            "ROI #14 측정 영역 설정",
            "ROI #15 측정 영역 설정",
            "ROI #16 측정 영역 설정",
            "ROI #17 측정 영역 설정",
            "ROI #18 측정 영역 설정",
            "ROI #19 측정 영역 설정",
            "ROI #20 측정 영역 설정",
            "ROI #21 측정 영역 설정",
            "ROI #22 측정 영역 설정",
            "ROI #23 측정 영역 설정",
            "ROI #24 측정 영역 설정",
            "ROI #25 측정 영역 설정",
            "ROI #26 측정 영역 설정",
            "ROI #27 측정 영역 설정",
            "ROI #28 측정 영역 설정",
            "ROI #29 측정 영역 설정",
            "ROI #30 측정 영역 설정",
            "ROI #31 측정 영역 설정",
            "ROI #32 측정 영역 설정",
            "ROI #33 측정 영역 설정",
            "ROI #34 측정 영역 설정",
            "ROI #35 측정 영역 설정",
            "ROI #36 측정 영역 설정",
            "ROI #37 측정 영역 설정",
            "ROI #38 측정 영역 설정",
            "ROI #39 측정 영역 설정",
            "ROI #40 측정 영역 설정"});
            this.listBox1.Location = new System.Drawing.Point(3, 70);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(278, 84);
            this.listBox1.TabIndex = 2;
            this.listBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listBox1_ItemCheck);
            this.listBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseClick);
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // button_ROTATION_CAL
            // 
            this.button_ROTATION_CAL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ROTATION_CAL.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ROTATION_CAL.BackgroundImage")));
            this.button_ROTATION_CAL.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_ROTATION_CAL.ForeColor = System.Drawing.Color.White;
            this.button_ROTATION_CAL.Location = new System.Drawing.Point(96, 408);
            this.button_ROTATION_CAL.Name = "button_ROTATION_CAL";
            this.button_ROTATION_CAL.Size = new System.Drawing.Size(51, 45);
            this.button_ROTATION_CAL.TabIndex = 61;
            this.button_ROTATION_CAL.Text = "보정";
            this.button_ROTATION_CAL.UseVisualStyleBackColor = true;
            this.button_ROTATION_CAL.Click += new System.EventHandler(this.button_ROTATION_CAL_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.DarkOrange;
            this.label2.Location = new System.Drawing.Point(5, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 60;
            this.label2.Text = "Position";
            // 
            // comboBox_CAMPOSITION
            // 
            this.comboBox_CAMPOSITION.FormattingEnabled = true;
            this.comboBox_CAMPOSITION.Items.AddRange(new object[] {
            "#1 상부/하부",
            "#2 사이드"});
            this.comboBox_CAMPOSITION.Location = new System.Drawing.Point(68, 28);
            this.comboBox_CAMPOSITION.Name = "comboBox_CAMPOSITION";
            this.comboBox_CAMPOSITION.Size = new System.Drawing.Size(213, 20);
            this.comboBox_CAMPOSITION.TabIndex = 59;
            this.comboBox_CAMPOSITION.Text = "#1 상부/하부";
            this.comboBox_CAMPOSITION.SelectedIndexChanged += new System.EventHandler(this.comboBox_CAMPOSITION_SelectedIndexChanged);
            // 
            // comboBox_TABLETYPE
            // 
            this.comboBox_TABLETYPE.FormattingEnabled = true;
            this.comboBox_TABLETYPE.Items.AddRange(new object[] {
            "#1 인덱스 타입",
            "#2 유리판 타입",
            "#3 라인스캔 벨트 타입",
            "#4 가이드 없음",
            "#5 ROI 기준 측정",
            "#6 고객사 전용 타입"});
            this.comboBox_TABLETYPE.Location = new System.Drawing.Point(68, 5);
            this.comboBox_TABLETYPE.Name = "comboBox_TABLETYPE";
            this.comboBox_TABLETYPE.Size = new System.Drawing.Size(213, 20);
            this.comboBox_TABLETYPE.TabIndex = 58;
            this.comboBox_TABLETYPE.Text = "#1 인덱스 타입";
            this.comboBox_TABLETYPE.SelectedIndexChanged += new System.EventHandler(this.comboBox_PC_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 12);
            this.label1.TabIndex = 57;
            this.label1.Text = "Type";
            // 
            // radioButton2
            // 
            this.radioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(143, 388);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(127, 16);
            this.radioButton2.TabIndex = 56;
            this.radioButton2.Text = "알고리즘 시험 모드";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 388);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(131, 16);
            this.radioButton1.TabIndex = 55;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "검사 영역 설정 모드";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // button_INSPECTION
            // 
            this.button_INSPECTION.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_INSPECTION.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_INSPECTION.BackgroundImage")));
            this.button_INSPECTION.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_INSPECTION.ForeColor = System.Drawing.Color.White;
            this.button_INSPECTION.Location = new System.Drawing.Point(153, 408);
            this.button_INSPECTION.Name = "button_INSPECTION";
            this.button_INSPECTION.Size = new System.Drawing.Size(128, 45);
            this.button_INSPECTION.TabIndex = 54;
            this.button_INSPECTION.Text = "검사 결과";
            this.button_INSPECTION.UseVisualStyleBackColor = true;
            this.button_INSPECTION.Click += new System.EventHandler(this.button_INSPECTION_Click);
            // 
            // button_SAVE
            // 
            this.button_SAVE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_SAVE.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SAVE.BackgroundImage")));
            this.button_SAVE.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_SAVE.ForeColor = System.Drawing.Color.White;
            this.button_SAVE.Location = new System.Drawing.Point(194, 458);
            this.button_SAVE.Name = "button_SAVE";
            this.button_SAVE.Size = new System.Drawing.Size(88, 37);
            this.button_SAVE.TabIndex = 53;
            this.button_SAVE.Text = "설정 저장";
            this.button_SAVE.UseVisualStyleBackColor = true;
            this.button_SAVE.Click += new System.EventHandler(this.button_SAVE_Click);
            // 
            // button_LOAD
            // 
            this.button_LOAD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_LOAD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_LOAD.BackgroundImage")));
            this.button_LOAD.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_LOAD.ForeColor = System.Drawing.Color.White;
            this.button_LOAD.Location = new System.Drawing.Point(96, 458);
            this.button_LOAD.Name = "button_LOAD";
            this.button_LOAD.Size = new System.Drawing.Size(93, 37);
            this.button_LOAD.TabIndex = 52;
            this.button_LOAD.Text = "설정 불러오기";
            this.button_LOAD.UseVisualStyleBackColor = true;
            this.button_LOAD.Click += new System.EventHandler(this.button_LOAD_Click);
            // 
            // button_SNAPSHOT
            // 
            this.button_SNAPSHOT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_SNAPSHOT.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SNAPSHOT.BackgroundImage")));
            this.button_SNAPSHOT.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_SNAPSHOT.ForeColor = System.Drawing.Color.White;
            this.button_SNAPSHOT.Location = new System.Drawing.Point(3, 458);
            this.button_SNAPSHOT.Name = "button_SNAPSHOT";
            this.button_SNAPSHOT.Size = new System.Drawing.Size(88, 37);
            this.button_SNAPSHOT.TabIndex = 51;
            this.button_SNAPSHOT.Text = "카메라 촬영";
            this.button_SNAPSHOT.UseVisualStyleBackColor = true;
            this.button_SNAPSHOT.Click += new System.EventHandler(this.button_SNAPSHOT_Click);
            // 
            // button_OPEN
            // 
            this.button_OPEN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_OPEN.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_OPEN.BackgroundImage")));
            this.button_OPEN.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_OPEN.ForeColor = System.Drawing.Color.White;
            this.button_OPEN.Location = new System.Drawing.Point(3, 408);
            this.button_OPEN.Name = "button_OPEN";
            this.button_OPEN.Size = new System.Drawing.Size(88, 45);
            this.button_OPEN.TabIndex = 50;
            this.button_OPEN.Text = "이미지 열기";
            this.button_OPEN.UseVisualStyleBackColor = true;
            this.button_OPEN.Click += new System.EventHandler(this.button_OPEN_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 160);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 21;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.Size = new System.Drawing.Size(278, 222);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            this.dataGridView1.SizeChanged += new System.EventHandler(this.dataGridView1_SizeChanged);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer_RIGHT);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2MinSize = 1;
            this.splitContainer2.Size = new System.Drawing.Size(647, 496);
            this.splitContainer2.SplitterDistance = 494;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 3;
            // 
            // splitContainer_RIGHT
            // 
            this.splitContainer_RIGHT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_RIGHT.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_RIGHT.Name = "splitContainer_RIGHT";
            this.splitContainer_RIGHT.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_RIGHT.Panel1
            // 
            this.splitContainer_RIGHT.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer_RIGHT.Panel1.Controls.Add(this.splitContainer_RIGHT2);
            // 
            // splitContainer_RIGHT.Panel2
            // 
            this.splitContainer_RIGHT.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer_RIGHT.Panel2MinSize = 0;
            this.splitContainer_RIGHT.Size = new System.Drawing.Size(647, 494);
            this.splitContainer_RIGHT.SplitterDistance = 453;
            this.splitContainer_RIGHT.SplitterWidth = 1;
            this.splitContainer_RIGHT.TabIndex = 2;
            // 
            // splitContainer_RIGHT2
            // 
            this.splitContainer_RIGHT2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_RIGHT2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_RIGHT2.Name = "splitContainer_RIGHT2";
            // 
            // splitContainer_RIGHT2.Panel1
            // 
            this.splitContainer_RIGHT2.Panel1.Controls.Add(this.pictureBox_Image);
            this.splitContainer_RIGHT2.Panel1.Controls.Add(this.pictureBox_RImage);
            this.splitContainer_RIGHT2.Panel2MinSize = 0;
            this.splitContainer_RIGHT2.Size = new System.Drawing.Size(647, 453);
            this.splitContainer_RIGHT2.SplitterDistance = 617;
            this.splitContainer_RIGHT2.SplitterWidth = 1;
            this.splitContainer_RIGHT2.TabIndex = 2;
            // 
            // pictureBox_Image
            // 
            this.pictureBox_Image.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox_Image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_Image.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_Image.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Image.Name = "pictureBox_Image";
            this.pictureBox_Image.Size = new System.Drawing.Size(617, 453);
            this.pictureBox_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Image.TabIndex = 0;
            this.pictureBox_Image.TabStop = false;
            this.pictureBox_Image.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Image_Paint);
            this.pictureBox_Image.DoubleClick += new System.EventHandler(this.pictureBox_Image_DoubleClick);
            this.pictureBox_Image.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Image_MouseClick);
            this.pictureBox_Image.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Image_MouseMove);
            // 
            // pictureBox_RImage
            // 
            this.pictureBox_RImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox_RImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_RImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_RImage.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_RImage.Name = "pictureBox_RImage";
            this.pictureBox_RImage.Size = new System.Drawing.Size(617, 453);
            this.pictureBox_RImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_RImage.TabIndex = 1;
            this.pictureBox_RImage.TabStop = false;
            this.pictureBox_RImage.Visible = false;
            this.pictureBox_RImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_RImage_Paint);
            this.pictureBox_RImage.DoubleClick += new System.EventHandler(this.pictureBox_RImage_DoubleClick);
            this.pictureBox_RImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_RImage_MouseClick);
            this.pictureBox_RImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_RImage_MouseMove);
            // 
            // Ctr_ROI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Ctr_ROI";
            this.Size = new System.Drawing.Size(932, 496);
            this.SizeChanged += new System.EventHandler(this.Ctr_ROI_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer_RIGHT.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_RIGHT)).EndInit();
            this.splitContainer_RIGHT.ResumeLayout(false);
            this.splitContainer_RIGHT2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_RIGHT2)).EndInit();
            this.splitContainer_RIGHT2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView1;
        public System.Windows.Forms.PictureBox pictureBox_Image;
        //private ProPictureBox pictureBox_Image;
        private System.Windows.Forms.Button button_OPEN;
        //public System.Windows.Forms.ListBox listBox1;
        public System.Windows.Forms.CheckedListBox listBox1;
        private System.Windows.Forms.Button button_SAVE;
        private System.Windows.Forms.Button button_LOAD;
        private System.Windows.Forms.Button button_SNAPSHOT;
        private System.Windows.Forms.Button button_INSPECTION;
        private System.Windows.Forms.PictureBox pictureBox_RImage;
        public System.Windows.Forms.ComboBox comboBox_TABLETYPE;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.SplitContainer splitContainer_RIGHT;
        private System.Windows.Forms.SplitContainer splitContainer_RIGHT2;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox comboBox_CAMPOSITION;
        private System.Windows.Forms.Button button_ROTATION_CAL;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button button_ParamView;
        private System.Windows.Forms.CheckBox checkBox_AutoInspection;
        public System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.RadioButton radioButton2;
        public System.Windows.Forms.RadioButton radioButton1;
    }
}
