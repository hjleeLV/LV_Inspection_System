namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_PLC
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_PLC));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button_View = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_Tab_Enable = new System.Windows.Forms.CheckBox();
            this.checkBox_SIMULATION = new System.Windows.Forms.CheckBox();
            this.checkBox_JView = new System.Windows.Forms.CheckBox();
            this.comboBox_Protocal = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBox_SlaveID = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.button_LOG_CLEAR = new System.Windows.Forms.Button();
            this.txt1 = new System.Windows.Forms.TextBox();
            this.richTextBox_LOG = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_MC = new System.Windows.Forms.CheckBox();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.cbReceiveFormat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbStopBits = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbParity = new System.Windows.Forms.ComboBox();
            this.cbBaudrate = new System.Windows.Forms.ComboBox();
            this.cbPortName = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbDataBits = new System.Windows.Forms.ComboBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.cbSendFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_SERVER_PORT = new System.Windows.Forms.TextBox();
            this.textBox_SERVER_IP = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_L_READ = new System.Windows.Forms.Button();
            this.button_L_WRITE = new System.Windows.Forms.Button();
            this.textBox_L_DATA = new System.Windows.Forms.TextBox();
            this.textBox_L_DEVICE = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txt_Data = new System.Windows.Forms.TextBox();
            this.button_SAVE = new System.Windows.Forms.Button();
            this.groupBox_D = new System.Windows.Forms.GroupBox();
            this.textBox_D_SIZE = new System.Windows.Forms.TextBox();
            this.textBox_D_DATA = new System.Windows.Forms.TextBox();
            this.button_D_READ = new System.Windows.Forms.Button();
            this.button_D_WRITE = new System.Windows.Forms.Button();
            this.textBox_D_DEVICE = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.button_LOAD = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt2 = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.panel_MC = new System.Windows.Forms.Panel();
            this.dataGridView_MC_Tx = new System.Windows.Forms.DataGridView();
            this.numericUpDown_MC_Tx = new System.Windows.Forms.NumericUpDown();
            this.button_MC_Tx_Apply = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.checkBox_MC_Tx_Use = new System.Windows.Forms.CheckBox();
            this.numericUpDown_MC_Rx = new System.Windows.Forms.NumericUpDown();
            this.dataGridView_MC_Rx = new System.Windows.Forms.DataGridView();
            this.button_MC_Rx_Apply = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.checkBox_MC_Rx_Use = new System.Windows.Forms.CheckBox();
            this.textBox_MinTime = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.textBox_RESETDURATION = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox_DELAYCAMMISS = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.checkBox_MERGETX = new System.Windows.Forms.CheckBox();
            this.textBox_ROOMNUM = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox_CAMREFCNT = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBox_NOOBJECT = new System.Windows.Forms.ComboBox();
            this.Button_Send_Apply = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.textBox_Delay3 = new System.Windows.Forms.TextBox();
            this.textBox_TxInterval = new System.Windows.Forms.TextBox();
            this.textBox_CheckDelay = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.textBox_Delay2 = new System.Windows.Forms.TextBox();
            this.checkBox_AllOnceTx = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.checkBox_PINGPONG = new System.Windows.Forms.CheckBox();
            this.textBox_Delay1 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox_Delay0 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.button_MCRx_Test = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox_D.SuspendLayout();
            this.panel_MC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_MC_Tx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MC_Tx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MC_Rx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_MC_Rx)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.ReadBufferSize = 2048;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button_View);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.button_LOG_CLEAR);
            this.splitContainer1.Panel1.Controls.Add(this.txt1);
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox_LOG);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.txt_Data);
            this.splitContainer1.Panel1.Controls.Add(this.button_SAVE);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox_D);
            this.splitContainer1.Panel1.Controls.Add(this.button_LOAD);
            this.splitContainer1.Panel1.Controls.Add(this.label10);
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.txt2);
            this.splitContainer1.Panel1.Controls.Add(this.btnSend);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel_MC);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_MinTime);
            this.splitContainer1.Panel2.Controls.Add(this.label26);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_RESETDURATION);
            this.splitContainer1.Panel2.Controls.Add(this.label25);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_DELAYCAMMISS);
            this.splitContainer1.Panel2.Controls.Add(this.label24);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_MERGETX);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_ROOMNUM);
            this.splitContainer1.Panel2.Controls.Add(this.label23);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_CAMREFCNT);
            this.splitContainer1.Panel2.Controls.Add(this.label22);
            this.splitContainer1.Panel2.Controls.Add(this.comboBox_NOOBJECT);
            this.splitContainer1.Panel2.Controls.Add(this.Button_Send_Apply);
            this.splitContainer1.Panel2.Controls.Add(this.label36);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_Delay3);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_TxInterval);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_CheckDelay);
            this.splitContainer1.Panel2.Controls.Add(this.label17);
            this.splitContainer1.Panel2.Controls.Add(this.label21);
            this.splitContainer1.Panel2.Controls.Add(this.label35);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_Delay2);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_AllOnceTx);
            this.splitContainer1.Panel2.Controls.Add(this.label20);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_PINGPONG);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_Delay1);
            this.splitContainer1.Panel2.Controls.Add(this.label19);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_Delay0);
            this.splitContainer1.Panel2.Controls.Add(this.label18);
            this.splitContainer1.Size = new System.Drawing.Size(1410, 676);
            this.splitContainer1.SplitterDistance = 337;
            this.splitContainer1.TabIndex = 38;
            this.splitContainer1.TabStop = false;
            // 
            // button_View
            // 
            this.button_View.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_View.Location = new System.Drawing.Point(667, 260);
            this.button_View.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_View.Name = "button_View";
            this.button_View.Size = new System.Drawing.Size(49, 20);
            this.button_View.TabIndex = 67;
            this.button_View.Text = "뷰";
            this.button_View.UseVisualStyleBackColor = true;
            this.button_View.Click += new System.EventHandler(this.button_View_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox_Tab_Enable);
            this.groupBox3.Controls.Add(this.checkBox_SIMULATION);
            this.groupBox3.Controls.Add(this.checkBox_JView);
            this.groupBox3.Controls.Add(this.comboBox_Protocal);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.comboBox_SlaveID);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Location = new System.Drawing.Point(776, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 289);
            this.groupBox3.TabIndex = 68;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "프로토콜 설정";
            // 
            // checkBox_Tab_Enable
            // 
            this.checkBox_Tab_Enable.AutoSize = true;
            this.checkBox_Tab_Enable.Location = new System.Drawing.Point(40, 246);
            this.checkBox_Tab_Enable.Name = "checkBox_Tab_Enable";
            this.checkBox_Tab_Enable.Size = new System.Drawing.Size(160, 19);
            this.checkBox_Tab_Enable.TabIndex = 73;
            this.checkBox_Tab_Enable.Text = "Disable Menu when start";
            this.checkBox_Tab_Enable.UseVisualStyleBackColor = true;
            this.checkBox_Tab_Enable.CheckedChanged += new System.EventHandler(this.checkBox_Tab_Enable_CheckedChanged);
            // 
            // checkBox_SIMULATION
            // 
            this.checkBox_SIMULATION.AutoSize = true;
            this.checkBox_SIMULATION.Location = new System.Drawing.Point(40, 218);
            this.checkBox_SIMULATION.Name = "checkBox_SIMULATION";
            this.checkBox_SIMULATION.Size = new System.Drawing.Size(119, 19);
            this.checkBox_SIMULATION.TabIndex = 72;
            this.checkBox_SIMULATION.Text = "Simulation Mode";
            this.checkBox_SIMULATION.UseVisualStyleBackColor = true;
            this.checkBox_SIMULATION.CheckedChanged += new System.EventHandler(this.checkBox_SIMULATION_CheckedChanged);
            // 
            // checkBox_JView
            // 
            this.checkBox_JView.AutoSize = true;
            this.checkBox_JView.Location = new System.Drawing.Point(40, 191);
            this.checkBox_JView.Name = "checkBox_JView";
            this.checkBox_JView.Size = new System.Drawing.Size(106, 19);
            this.checkBox_JView.TabIndex = 71;
            this.checkBox_JView.Text = "판정 신호 보기";
            this.checkBox_JView.UseVisualStyleBackColor = true;
            this.checkBox_JView.CheckedChanged += new System.EventHandler(this.checkBox_JView_CheckedChanged);
            // 
            // comboBox_Protocal
            // 
            this.comboBox_Protocal.FormattingEnabled = true;
            this.comboBox_Protocal.Items.AddRange(new object[] {
            "XGT RS232",
            "Modbus TCP",
            "Modbus RTU",
            "LV DIO"});
            this.comboBox_Protocal.Location = new System.Drawing.Point(89, 24);
            this.comboBox_Protocal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_Protocal.Name = "comboBox_Protocal";
            this.comboBox_Protocal.Size = new System.Drawing.Size(108, 23);
            this.comboBox_Protocal.TabIndex = 23;
            this.comboBox_Protocal.Text = "XGT RS232";
            this.comboBox_Protocal.SelectedIndexChanged += new System.EventHandler(this.comboBox_Protocal_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 28);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 15);
            this.label16.TabIndex = 24;
            this.label16.Text = "프로토콜";
            // 
            // comboBox_SlaveID
            // 
            this.comboBox_SlaveID.FormattingEnabled = true;
            this.comboBox_SlaveID.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09"});
            this.comboBox_SlaveID.Location = new System.Drawing.Point(89, 55);
            this.comboBox_SlaveID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_SlaveID.Name = "comboBox_SlaveID";
            this.comboBox_SlaveID.Size = new System.Drawing.Size(108, 23);
            this.comboBox_SlaveID.TabIndex = 21;
            this.comboBox_SlaveID.Text = "00";
            this.comboBox_SlaveID.SelectedIndexChanged += new System.EventHandler(this.comboBox_SlaveID_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 59);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(31, 15);
            this.label15.TabIndex = 22;
            this.label15.Text = "국번";
            // 
            // button_LOG_CLEAR
            // 
            this.button_LOG_CLEAR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_LOG_CLEAR.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_LOG_CLEAR.Location = new System.Drawing.Point(1352, 9);
            this.button_LOG_CLEAR.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LOG_CLEAR.Name = "button_LOG_CLEAR";
            this.button_LOG_CLEAR.Size = new System.Drawing.Size(49, 20);
            this.button_LOG_CLEAR.TabIndex = 69;
            this.button_LOG_CLEAR.Text = "클리어";
            this.button_LOG_CLEAR.UseVisualStyleBackColor = true;
            this.button_LOG_CLEAR.Click += new System.EventHandler(this.button_LOG_CLEAR_Click);
            // 
            // txt1
            // 
            this.txt1.Location = new System.Drawing.Point(227, 282);
            this.txt1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt1.Multiline = true;
            this.txt1.Name = "txt1";
            this.txt1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt1.Size = new System.Drawing.Size(544, 50);
            this.txt1.TabIndex = 25;
            // 
            // richTextBox_LOG
            // 
            this.richTextBox_LOG.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_LOG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox_LOG.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBox_LOG.Location = new System.Drawing.Point(995, 7);
            this.richTextBox_LOG.Name = "richTextBox_LOG";
            this.richTextBox_LOG.Size = new System.Drawing.Size(409, 325);
            this.richTextBox_LOG.TabIndex = 78;
            this.richTextBox_LOG.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_MC);
            this.groupBox2.Controls.Add(this.label38);
            this.groupBox2.Controls.Add(this.label37);
            this.groupBox2.Controls.Add(this.cbReceiveFormat);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbStopBits);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cbParity);
            this.groupBox2.Controls.Add(this.cbBaudrate);
            this.groupBox2.Controls.Add(this.cbPortName);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cbDataBits);
            this.groupBox2.Controls.Add(this.btnOpen);
            this.groupBox2.Controls.Add(this.cbSendFormat);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btnClose);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBox_SERVER_PORT);
            this.groupBox2.Controls.Add(this.textBox_SERVER_IP);
            this.groupBox2.Location = new System.Drawing.Point(8, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 326);
            this.groupBox2.TabIndex = 67;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "통신 설정";
            // 
            // checkBox_MC
            // 
            this.checkBox_MC.AutoSize = true;
            this.checkBox_MC.Location = new System.Drawing.Point(19, 254);
            this.checkBox_MC.Name = "checkBox_MC";
            this.checkBox_MC.Size = new System.Drawing.Size(152, 19);
            this.checkBox_MC.TabIndex = 72;
            this.checkBox_MC.Text = "Data Send through MC";
            this.checkBox_MC.UseVisualStyleBackColor = true;
            this.checkBox_MC.CheckedChanged += new System.EventHandler(this.checkBox_MC_CheckedChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(16, 89);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(64, 15);
            this.label38.TabIndex = 36;
            this.label38.Text = "서버 PORT";
            this.label38.Visible = false;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(16, 28);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(45, 15);
            this.label37.TabIndex = 35;
            this.label37.Text = "서버 IP";
            this.label37.Visible = false;
            // 
            // cbReceiveFormat
            // 
            this.cbReceiveFormat.FormattingEnabled = true;
            this.cbReceiveFormat.Items.AddRange(new object[] {
            "ASCII",
            "HEX"});
            this.cbReceiveFormat.Location = new System.Drawing.Point(89, 180);
            this.cbReceiveFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbReceiveFormat.Name = "cbReceiveFormat";
            this.cbReceiveFormat.Size = new System.Drawing.Size(108, 23);
            this.cbReceiveFormat.TabIndex = 31;
            this.cbReceiveFormat.Text = "ASCII";
            this.cbReceiveFormat.SelectedIndexChanged += new System.EventHandler(this.cbReceiveFormat_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 15);
            this.label4.TabIndex = 24;
            this.label4.Text = "포트 이름";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 15);
            this.label3.TabIndex = 34;
            this.label3.Text = "송신 포맷";
            // 
            // cbStopBits
            // 
            this.cbStopBits.FormattingEnabled = true;
            this.cbStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.cbStopBits.Location = new System.Drawing.Point(89, 118);
            this.cbStopBits.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbStopBits.Name = "cbStopBits";
            this.cbStopBits.Size = new System.Drawing.Size(108, 23);
            this.cbStopBits.TabIndex = 23;
            this.cbStopBits.Text = "1";
            this.cbStopBits.SelectedIndexChanged += new System.EventHandler(this.cbStopBits_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 15);
            this.label7.TabIndex = 28;
            this.label7.Text = "스탑 비트";
            // 
            // cbParity
            // 
            this.cbParity.FormattingEnabled = true;
            this.cbParity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even"});
            this.cbParity.Location = new System.Drawing.Point(89, 149);
            this.cbParity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbParity.Name = "cbParity";
            this.cbParity.Size = new System.Drawing.Size(108, 23);
            this.cbParity.TabIndex = 22;
            this.cbParity.Text = "None";
            // 
            // cbBaudrate
            // 
            this.cbBaudrate.FormattingEnabled = true;
            this.cbBaudrate.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cbBaudrate.Location = new System.Drawing.Point(89, 55);
            this.cbBaudrate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbBaudrate.Name = "cbBaudrate";
            this.cbBaudrate.Size = new System.Drawing.Size(108, 23);
            this.cbBaudrate.TabIndex = 20;
            this.cbBaudrate.Text = "115200";
            this.cbBaudrate.SelectedIndexChanged += new System.EventHandler(this.cbBaudrate_SelectedIndexChanged);
            // 
            // cbPortName
            // 
            this.cbPortName.FormattingEnabled = true;
            this.cbPortName.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.cbPortName.Location = new System.Drawing.Point(89, 24);
            this.cbPortName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbPortName.Name = "cbPortName";
            this.cbPortName.Size = new System.Drawing.Size(108, 23);
            this.cbPortName.TabIndex = 19;
            this.cbPortName.Text = "COM1";
            this.cbPortName.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 15);
            this.label8.TabIndex = 26;
            this.label8.Text = "페러티";
            // 
            // cbDataBits
            // 
            this.cbDataBits.FormattingEnabled = true;
            this.cbDataBits.Items.AddRange(new object[] {
            "7",
            "8"});
            this.cbDataBits.Location = new System.Drawing.Point(89, 86);
            this.cbDataBits.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbDataBits.Name = "cbDataBits";
            this.cbDataBits.Size = new System.Drawing.Size(108, 23);
            this.cbDataBits.TabIndex = 21;
            this.cbDataBits.Text = "8";
            this.cbDataBits.SelectedIndexChanged += new System.EventHandler(this.cbDataBits_SelectedIndexChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.btnOpen.ForeColor = System.Drawing.Color.White;
            this.btnOpen.Location = new System.Drawing.Point(10, 286);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(94, 30);
            this.btnOpen.TabIndex = 29;
            this.btnOpen.Text = "연결";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // cbSendFormat
            // 
            this.cbSendFormat.FormattingEnabled = true;
            this.cbSendFormat.Items.AddRange(new object[] {
            "ASCII",
            "HEX"});
            this.cbSendFormat.Location = new System.Drawing.Point(89, 210);
            this.cbSendFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbSendFormat.Name = "cbSendFormat";
            this.cbSendFormat.Size = new System.Drawing.Size(108, 23);
            this.cbSendFormat.TabIndex = 32;
            this.cbSendFormat.Text = "ASCII";
            this.cbSendFormat.SelectedIndexChanged += new System.EventHandler(this.cbSendFormat_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 15);
            this.label6.TabIndex = 27;
            this.label6.Text = "데이터 비트";
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.Enabled = false;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(109, 286);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 30);
            this.btnClose.TabIndex = 30;
            this.btnClose.Text = "접속 해제";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 15);
            this.label5.TabIndex = 25;
            this.label5.Text = "보드속도";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 33;
            this.label2.Text = "수신 포맷";
            // 
            // textBox_SERVER_PORT
            // 
            this.textBox_SERVER_PORT.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_SERVER_PORT.Location = new System.Drawing.Point(18, 118);
            this.textBox_SERVER_PORT.Name = "textBox_SERVER_PORT";
            this.textBox_SERVER_PORT.Size = new System.Drawing.Size(179, 23);
            this.textBox_SERVER_PORT.TabIndex = 61;
            this.textBox_SERVER_PORT.Text = "502";
            this.textBox_SERVER_PORT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_SERVER_PORT.Visible = false;
            // 
            // textBox_SERVER_IP
            // 
            this.textBox_SERVER_IP.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_SERVER_IP.Location = new System.Drawing.Point(18, 55);
            this.textBox_SERVER_IP.Name = "textBox_SERVER_IP";
            this.textBox_SERVER_IP.Size = new System.Drawing.Size(179, 23);
            this.textBox_SERVER_IP.TabIndex = 60;
            this.textBox_SERVER_IP.Text = "192.168.0.10";
            this.textBox_SERVER_IP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_SERVER_IP.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_L_READ);
            this.groupBox1.Controls.Add(this.button_L_WRITE);
            this.groupBox1.Controls.Add(this.textBox_L_DATA);
            this.groupBox1.Controls.Add(this.textBox_L_DEVICE);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(503, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 160);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "L or M memory";
            // 
            // button_L_READ
            // 
            this.button_L_READ.ForeColor = System.Drawing.Color.White;
            this.button_L_READ.Image = ((System.Drawing.Image)(resources.GetObject("button_L_READ.Image")));
            this.button_L_READ.Location = new System.Drawing.Point(11, 114);
            this.button_L_READ.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_L_READ.Name = "button_L_READ";
            this.button_L_READ.Size = new System.Drawing.Size(98, 30);
            this.button_L_READ.TabIndex = 62;
            this.button_L_READ.Text = "읽기";
            this.button_L_READ.Click += new System.EventHandler(this.button_L_READ_Click);
            // 
            // button_L_WRITE
            // 
            this.button_L_WRITE.ForeColor = System.Drawing.Color.White;
            this.button_L_WRITE.Image = ((System.Drawing.Image)(resources.GetObject("button_L_WRITE.Image")));
            this.button_L_WRITE.Location = new System.Drawing.Point(124, 114);
            this.button_L_WRITE.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_L_WRITE.Name = "button_L_WRITE";
            this.button_L_WRITE.Size = new System.Drawing.Size(98, 30);
            this.button_L_WRITE.TabIndex = 63;
            this.button_L_WRITE.Text = "쓰기";
            this.button_L_WRITE.Click += new System.EventHandler(this.button_L_WRITE_Click);
            // 
            // textBox_L_DATA
            // 
            this.textBox_L_DATA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_L_DATA.Location = new System.Drawing.Point(76, 49);
            this.textBox_L_DATA.Name = "textBox_L_DATA";
            this.textBox_L_DATA.Size = new System.Drawing.Size(66, 23);
            this.textBox_L_DATA.TabIndex = 61;
            this.textBox_L_DATA.Text = "1";
            this.textBox_L_DATA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_L_DEVICE
            // 
            this.textBox_L_DEVICE.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_L_DEVICE.Location = new System.Drawing.Point(76, 17);
            this.textBox_L_DEVICE.Name = "textBox_L_DEVICE";
            this.textBox_L_DEVICE.Size = new System.Drawing.Size(66, 23);
            this.textBox_L_DEVICE.TabIndex = 60;
            this.textBox_L_DEVICE.Text = "LX1001";
            this.textBox_L_DEVICE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(31, 15);
            this.label13.TabIndex = 54;
            this.label13.Text = "주소";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 15);
            this.label14.TabIndex = 56;
            this.label14.Text = "데이터 값";
            // 
            // txt_Data
            // 
            this.txt_Data.AcceptsReturn = true;
            this.txt_Data.Location = new System.Drawing.Point(227, 170);
            this.txt_Data.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txt_Data.Multiline = true;
            this.txt_Data.Name = "txt_Data";
            this.txt_Data.ReadOnly = true;
            this.txt_Data.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Data.Size = new System.Drawing.Size(544, 39);
            this.txt_Data.TabIndex = 66;
            this.txt_Data.TabStop = false;
            // 
            // button_SAVE
            // 
            this.button_SAVE.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SAVE.BackgroundImage")));
            this.button_SAVE.ForeColor = System.Drawing.Color.White;
            this.button_SAVE.Location = new System.Drawing.Point(877, 302);
            this.button_SAVE.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_SAVE.Name = "button_SAVE";
            this.button_SAVE.Size = new System.Drawing.Size(112, 30);
            this.button_SAVE.TabIndex = 36;
            this.button_SAVE.Text = "적용 및 저장";
            this.button_SAVE.UseVisualStyleBackColor = true;
            this.button_SAVE.Click += new System.EventHandler(this.button_SAVE_Click);
            // 
            // groupBox_D
            // 
            this.groupBox_D.Controls.Add(this.textBox_D_SIZE);
            this.groupBox_D.Controls.Add(this.textBox_D_DATA);
            this.groupBox_D.Controls.Add(this.button_D_READ);
            this.groupBox_D.Controls.Add(this.button_D_WRITE);
            this.groupBox_D.Controls.Add(this.textBox_D_DEVICE);
            this.groupBox_D.Controls.Add(this.label9);
            this.groupBox_D.Controls.Add(this.label11);
            this.groupBox_D.Controls.Add(this.label12);
            this.groupBox_D.Location = new System.Drawing.Point(227, 6);
            this.groupBox_D.Name = "groupBox_D";
            this.groupBox_D.Size = new System.Drawing.Size(267, 161);
            this.groupBox_D.TabIndex = 64;
            this.groupBox_D.TabStop = false;
            this.groupBox_D.Text = "D memory";
            // 
            // textBox_D_SIZE
            // 
            this.textBox_D_SIZE.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_D_SIZE.Location = new System.Drawing.Point(90, 51);
            this.textBox_D_SIZE.Name = "textBox_D_SIZE";
            this.textBox_D_SIZE.Size = new System.Drawing.Size(30, 25);
            this.textBox_D_SIZE.TabIndex = 58;
            this.textBox_D_SIZE.Text = "2";
            this.textBox_D_SIZE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_D_DATA
            // 
            this.textBox_D_DATA.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_D_DATA.Location = new System.Drawing.Point(90, 84);
            this.textBox_D_DATA.Name = "textBox_D_DATA";
            this.textBox_D_DATA.Size = new System.Drawing.Size(66, 25);
            this.textBox_D_DATA.TabIndex = 59;
            this.textBox_D_DATA.Text = "12345678";
            this.textBox_D_DATA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button_D_READ
            // 
            this.button_D_READ.ForeColor = System.Drawing.Color.White;
            this.button_D_READ.Image = ((System.Drawing.Image)(resources.GetObject("button_D_READ.Image")));
            this.button_D_READ.Location = new System.Drawing.Point(8, 115);
            this.button_D_READ.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_D_READ.Name = "button_D_READ";
            this.button_D_READ.Size = new System.Drawing.Size(98, 30);
            this.button_D_READ.TabIndex = 52;
            this.button_D_READ.Text = "읽기";
            this.button_D_READ.Click += new System.EventHandler(this.button_D_READ_Click);
            // 
            // button_D_WRITE
            // 
            this.button_D_WRITE.ForeColor = System.Drawing.Color.White;
            this.button_D_WRITE.Image = ((System.Drawing.Image)(resources.GetObject("button_D_WRITE.Image")));
            this.button_D_WRITE.Location = new System.Drawing.Point(126, 115);
            this.button_D_WRITE.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_D_WRITE.Name = "button_D_WRITE";
            this.button_D_WRITE.Size = new System.Drawing.Size(98, 30);
            this.button_D_WRITE.TabIndex = 53;
            this.button_D_WRITE.Text = "쓰기";
            this.button_D_WRITE.Click += new System.EventHandler(this.button_D_WRITE_Click);
            // 
            // textBox_D_DEVICE
            // 
            this.textBox_D_DEVICE.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_D_DEVICE.Location = new System.Drawing.Point(90, 18);
            this.textBox_D_DEVICE.Name = "textBox_D_DEVICE";
            this.textBox_D_DEVICE.Size = new System.Drawing.Size(66, 23);
            this.textBox_D_DEVICE.TabIndex = 57;
            this.textBox_D_DEVICE.Text = "DW5000";
            this.textBox_D_DEVICE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 15);
            this.label9.TabIndex = 54;
            this.label9.Text = "주소";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 87);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 15);
            this.label11.TabIndex = 56;
            this.label11.Text = "데이터 값";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 15);
            this.label12.TabIndex = 55;
            this.label12.Text = "데이터 크기";
            // 
            // button_LOAD
            // 
            this.button_LOAD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_LOAD.BackgroundImage")));
            this.button_LOAD.ForeColor = System.Drawing.Color.White;
            this.button_LOAD.Location = new System.Drawing.Point(777, 302);
            this.button_LOAD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LOAD.Name = "button_LOAD";
            this.button_LOAD.Size = new System.Drawing.Size(94, 30);
            this.button_LOAD.TabIndex = 35;
            this.button_LOAD.Text = "불러오기";
            this.button_LOAD.UseVisualStyleBackColor = true;
            this.button_LOAD.Click += new System.EventHandler(this.button_LOAD_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(223, 213);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 15);
            this.label10.TabIndex = 28;
            this.label10.Text = "[Tx Data]";
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClear.Location = new System.Drawing.Point(722, 259);
            this.btnClear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(49, 20);
            this.btnClear.TabIndex = 27;
            this.btnClear.Text = "클리어";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(223, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 26;
            this.label1.Text = "[Rx Data]";
            // 
            // txt2
            // 
            this.txt2.Location = new System.Drawing.Point(227, 233);
            this.txt2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt2.Name = "txt2";
            this.txt2.Size = new System.Drawing.Size(544, 23);
            this.txt2.TabIndex = 24;
            // 
            // btnSend
            // 
            this.btnSend.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSend.Location = new System.Drawing.Point(722, 211);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(49, 20);
            this.btnSend.TabIndex = 23;
            this.btnSend.Text = "송신";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // panel_MC
            // 
            this.panel_MC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_MC.Controls.Add(this.button_MCRx_Test);
            this.panel_MC.Controls.Add(this.dataGridView_MC_Tx);
            this.panel_MC.Controls.Add(this.numericUpDown_MC_Tx);
            this.panel_MC.Controls.Add(this.button_MC_Tx_Apply);
            this.panel_MC.Controls.Add(this.label28);
            this.panel_MC.Controls.Add(this.checkBox_MC_Tx_Use);
            this.panel_MC.Controls.Add(this.numericUpDown_MC_Rx);
            this.panel_MC.Controls.Add(this.dataGridView_MC_Rx);
            this.panel_MC.Controls.Add(this.button_MC_Rx_Apply);
            this.panel_MC.Controls.Add(this.label27);
            this.panel_MC.Controls.Add(this.checkBox_MC_Rx_Use);
            this.panel_MC.Location = new System.Drawing.Point(8, 37);
            this.panel_MC.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.panel_MC.Name = "panel_MC";
            this.panel_MC.Size = new System.Drawing.Size(1402, 295);
            this.panel_MC.TabIndex = 81;
            // 
            // dataGridView_MC_Tx
            // 
            this.dataGridView_MC_Tx.AllowUserToAddRows = false;
            this.dataGridView_MC_Tx.AllowUserToResizeColumns = false;
            this.dataGridView_MC_Tx.AllowUserToResizeRows = false;
            this.dataGridView_MC_Tx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_MC_Tx.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_MC_Tx.Location = new System.Drawing.Point(523, 35);
            this.dataGridView_MC_Tx.Name = "dataGridView_MC_Tx";
            this.dataGridView_MC_Tx.RowHeadersWidth = 21;
            this.dataGridView_MC_Tx.RowTemplate.Height = 23;
            this.dataGridView_MC_Tx.Size = new System.Drawing.Size(876, 257);
            this.dataGridView_MC_Tx.TabIndex = 82;
            // 
            // numericUpDown_MC_Tx
            // 
            this.numericUpDown_MC_Tx.Location = new System.Drawing.Point(716, 6);
            this.numericUpDown_MC_Tx.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown_MC_Tx.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_MC_Tx.Name = "numericUpDown_MC_Tx";
            this.numericUpDown_MC_Tx.Size = new System.Drawing.Size(59, 23);
            this.numericUpDown_MC_Tx.TabIndex = 81;
            this.numericUpDown_MC_Tx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_MC_Tx.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_MC_Tx.ValueChanged += new System.EventHandler(this.numericUpDown_MC_Tx_ValueChanged);
            // 
            // button_MC_Tx_Apply
            // 
            this.button_MC_Tx_Apply.ForeColor = System.Drawing.Color.White;
            this.button_MC_Tx_Apply.Image = ((System.Drawing.Image)(resources.GetObject("button_MC_Tx_Apply.Image")));
            this.button_MC_Tx_Apply.Location = new System.Drawing.Point(781, 2);
            this.button_MC_Tx_Apply.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_MC_Tx_Apply.Name = "button_MC_Tx_Apply";
            this.button_MC_Tx_Apply.Size = new System.Drawing.Size(86, 30);
            this.button_MC_Tx_Apply.TabIndex = 80;
            this.button_MC_Tx_Apply.Text = "적용";
            this.button_MC_Tx_Apply.Click += new System.EventHandler(this.button_MC_Tx_Apply_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(644, 9);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(65, 15);
            this.label28.TabIndex = 79;
            this.label28.Text = "Address 수";
            // 
            // checkBox_MC_Tx_Use
            // 
            this.checkBox_MC_Tx_Use.AutoSize = true;
            this.checkBox_MC_Tx_Use.Location = new System.Drawing.Point(523, 8);
            this.checkBox_MC_Tx_Use.Name = "checkBox_MC_Tx_Use";
            this.checkBox_MC_Tx_Use.Size = new System.Drawing.Size(114, 19);
            this.checkBox_MC_Tx_Use.TabIndex = 78;
            this.checkBox_MC_Tx_Use.Text = "송신(비전→PLC)";
            this.checkBox_MC_Tx_Use.UseVisualStyleBackColor = true;
            this.checkBox_MC_Tx_Use.CheckedChanged += new System.EventHandler(this.checkBox_MC_Tx_Use_CheckedChanged);
            // 
            // numericUpDown_MC_Rx
            // 
            this.numericUpDown_MC_Rx.Location = new System.Drawing.Point(199, 6);
            this.numericUpDown_MC_Rx.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown_MC_Rx.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_MC_Rx.Name = "numericUpDown_MC_Rx";
            this.numericUpDown_MC_Rx.Size = new System.Drawing.Size(59, 23);
            this.numericUpDown_MC_Rx.TabIndex = 77;
            this.numericUpDown_MC_Rx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_MC_Rx.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_MC_Rx.ValueChanged += new System.EventHandler(this.numericUpDown_MC_Rx_ValueChanged);
            // 
            // dataGridView_MC_Rx
            // 
            this.dataGridView_MC_Rx.AllowUserToAddRows = false;
            this.dataGridView_MC_Rx.AllowUserToResizeColumns = false;
            this.dataGridView_MC_Rx.AllowUserToResizeRows = false;
            this.dataGridView_MC_Rx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView_MC_Rx.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_MC_Rx.Location = new System.Drawing.Point(6, 35);
            this.dataGridView_MC_Rx.Name = "dataGridView_MC_Rx";
            this.dataGridView_MC_Rx.RowHeadersWidth = 21;
            this.dataGridView_MC_Rx.RowTemplate.Height = 23;
            this.dataGridView_MC_Rx.Size = new System.Drawing.Size(511, 257);
            this.dataGridView_MC_Rx.TabIndex = 76;
            // 
            // button_MC_Rx_Apply
            // 
            this.button_MC_Rx_Apply.ForeColor = System.Drawing.Color.White;
            this.button_MC_Rx_Apply.Image = ((System.Drawing.Image)(resources.GetObject("button_MC_Rx_Apply.Image")));
            this.button_MC_Rx_Apply.Location = new System.Drawing.Point(264, 2);
            this.button_MC_Rx_Apply.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_MC_Rx_Apply.Name = "button_MC_Rx_Apply";
            this.button_MC_Rx_Apply.Size = new System.Drawing.Size(86, 30);
            this.button_MC_Rx_Apply.TabIndex = 75;
            this.button_MC_Rx_Apply.Text = "적용";
            this.button_MC_Rx_Apply.Click += new System.EventHandler(this.button_MC_Rx_Apply_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(127, 9);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(65, 15);
            this.label27.TabIndex = 74;
            this.label27.Text = "Address 수";
            // 
            // checkBox_MC_Rx_Use
            // 
            this.checkBox_MC_Rx_Use.AutoSize = true;
            this.checkBox_MC_Rx_Use.Location = new System.Drawing.Point(6, 8);
            this.checkBox_MC_Rx_Use.Name = "checkBox_MC_Rx_Use";
            this.checkBox_MC_Rx_Use.Size = new System.Drawing.Size(114, 19);
            this.checkBox_MC_Rx_Use.TabIndex = 73;
            this.checkBox_MC_Rx_Use.Text = "수신(PLC→비전)";
            this.checkBox_MC_Rx_Use.UseVisualStyleBackColor = true;
            this.checkBox_MC_Rx_Use.CheckedChanged += new System.EventHandler(this.checkBox_MC_Rx_Use_CheckedChanged);
            // 
            // textBox_MinTime
            // 
            this.textBox_MinTime.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_MinTime.Location = new System.Drawing.Point(854, 7);
            this.textBox_MinTime.Name = "textBox_MinTime";
            this.textBox_MinTime.Size = new System.Drawing.Size(87, 23);
            this.textBox_MinTime.TabIndex = 80;
            this.textBox_MinTime.Text = "30";
            this.textBox_MinTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_MinTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_MinTime_KeyUp);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(743, 11);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(103, 15);
            this.label26.TabIndex = 79;
            this.label26.Text = "최소처리시간(ms)";
            // 
            // textBox_RESETDURATION
            // 
            this.textBox_RESETDURATION.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_RESETDURATION.Location = new System.Drawing.Point(478, 82);
            this.textBox_RESETDURATION.Name = "textBox_RESETDURATION";
            this.textBox_RESETDURATION.Size = new System.Drawing.Size(48, 25);
            this.textBox_RESETDURATION.TabIndex = 77;
            this.textBox_RESETDURATION.Text = "2";
            this.textBox_RESETDURATION.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(300, 86);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(152, 15);
            this.label25.TabIndex = 76;
            this.label25.Text = "Tx가 없을때 리셋 시간(sec)";
            // 
            // textBox_DELAYCAMMISS
            // 
            this.textBox_DELAYCAMMISS.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_DELAYCAMMISS.Location = new System.Drawing.Point(478, 7);
            this.textBox_DELAYCAMMISS.Name = "textBox_DELAYCAMMISS";
            this.textBox_DELAYCAMMISS.Size = new System.Drawing.Size(48, 25);
            this.textBox_DELAYCAMMISS.TabIndex = 75;
            this.textBox_DELAYCAMMISS.Text = "50";
            this.textBox_DELAYCAMMISS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DELAYCAMMISS.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_DELAYCAMMISS_KeyUp);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(300, 11);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(159, 15);
            this.label24.TabIndex = 74;
            this.label24.Text = "카메라 미처리일때 지연시간";
            // 
            // checkBox_MERGETX
            // 
            this.checkBox_MERGETX.AutoSize = true;
            this.checkBox_MERGETX.Location = new System.Drawing.Point(794, 51);
            this.checkBox_MERGETX.Name = "checkBox_MERGETX";
            this.checkBox_MERGETX.Size = new System.Drawing.Size(83, 19);
            this.checkBox_MERGETX.TabIndex = 73;
            this.checkBox_MERGETX.Text = "Merged Tx";
            this.checkBox_MERGETX.UseVisualStyleBackColor = true;
            // 
            // textBox_ROOMNUM
            // 
            this.textBox_ROOMNUM.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_ROOMNUM.Location = new System.Drawing.Point(722, 48);
            this.textBox_ROOMNUM.Name = "textBox_ROOMNUM";
            this.textBox_ROOMNUM.Size = new System.Drawing.Size(49, 23);
            this.textBox_ROOMNUM.TabIndex = 72;
            this.textBox_ROOMNUM.Text = "2";
            this.textBox_ROOMNUM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_ROOMNUM.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_ROOMNUM_KeyUp);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(640, 52);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(73, 15);
            this.label23.TabIndex = 71;
            this.label23.Text = "Room Num.";
            // 
            // textBox_CAMREFCNT
            // 
            this.textBox_CAMREFCNT.Enabled = false;
            this.textBox_CAMREFCNT.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_CAMREFCNT.Location = new System.Drawing.Point(478, 8);
            this.textBox_CAMREFCNT.Name = "textBox_CAMREFCNT";
            this.textBox_CAMREFCNT.Size = new System.Drawing.Size(48, 25);
            this.textBox_CAMREFCNT.TabIndex = 69;
            this.textBox_CAMREFCNT.Text = "10";
            this.textBox_CAMREFCNT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Enabled = false;
            this.label22.Location = new System.Drawing.Point(300, 12);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(123, 15);
            this.label22.TabIndex = 68;
            this.label22.Text = "카메라 재시작 카운트";
            // 
            // comboBox_NOOBJECT
            // 
            this.comboBox_NOOBJECT.FormattingEnabled = true;
            this.comboBox_NOOBJECT.Items.AddRange(new object[] {
            "CLASS1",
            "CLASS2",
            "CLASS3",
            "NONE",
            "OK"});
            this.comboBox_NOOBJECT.Location = new System.Drawing.Point(118, 7);
            this.comboBox_NOOBJECT.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_NOOBJECT.Name = "comboBox_NOOBJECT";
            this.comboBox_NOOBJECT.Size = new System.Drawing.Size(87, 23);
            this.comboBox_NOOBJECT.TabIndex = 69;
            this.comboBox_NOOBJECT.Text = "CLASS1";
            this.comboBox_NOOBJECT.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Button_Send_Apply
            // 
            this.Button_Send_Apply.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_Send_Apply.BackgroundImage")));
            this.Button_Send_Apply.ForeColor = System.Drawing.Color.White;
            this.Button_Send_Apply.Location = new System.Drawing.Point(190, 8);
            this.Button_Send_Apply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button_Send_Apply.Name = "Button_Send_Apply";
            this.Button_Send_Apply.Size = new System.Drawing.Size(94, 118);
            this.Button_Send_Apply.TabIndex = 67;
            this.Button_Send_Apply.Text = "적용 및 저장";
            this.Button_Send_Apply.UseVisualStyleBackColor = true;
            this.Button_Send_Apply.Click += new System.EventHandler(this.button_Send_Save_Click);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(23, 11);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(87, 15);
            this.label36.TabIndex = 70;
            this.label36.Text = "대상 없음 판정";
            this.label36.Click += new System.EventHandler(this.label36_Click);
            // 
            // textBox_Delay3
            // 
            this.textBox_Delay3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Delay3.Location = new System.Drawing.Point(133, 107);
            this.textBox_Delay3.Name = "textBox_Delay3";
            this.textBox_Delay3.Size = new System.Drawing.Size(48, 25);
            this.textBox_Delay3.TabIndex = 66;
            this.textBox_Delay3.Text = "0";
            this.textBox_Delay3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_TxInterval
            // 
            this.textBox_TxInterval.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_TxInterval.Location = new System.Drawing.Point(638, 7);
            this.textBox_TxInterval.Name = "textBox_TxInterval";
            this.textBox_TxInterval.Size = new System.Drawing.Size(87, 23);
            this.textBox_TxInterval.TabIndex = 64;
            this.textBox_TxInterval.Text = "15";
            this.textBox_TxInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_TxInterval.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_TxInterval_KeyUp);
            // 
            // textBox_CheckDelay
            // 
            this.textBox_CheckDelay.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_CheckDelay.Location = new System.Drawing.Point(791, 81);
            this.textBox_CheckDelay.Name = "textBox_CheckDelay";
            this.textBox_CheckDelay.Size = new System.Drawing.Size(65, 23);
            this.textBox_CheckDelay.TabIndex = 68;
            this.textBox_CheckDelay.Text = "100";
            this.textBox_CheckDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(549, 11);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(79, 15);
            this.label17.TabIndex = 63;
            this.label17.Text = "전송간격(ms)";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 111);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(116, 15);
            this.label21.TabIndex = 65;
            this.label21.Text = "CAM3 Trigger Delay";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(675, 84);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(111, 15);
            this.label35.TabIndex = 67;
            this.label35.Text = "판정 임계 시간(ms)";
            // 
            // textBox_Delay2
            // 
            this.textBox_Delay2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Delay2.Location = new System.Drawing.Point(133, 76);
            this.textBox_Delay2.Name = "textBox_Delay2";
            this.textBox_Delay2.Size = new System.Drawing.Size(48, 25);
            this.textBox_Delay2.TabIndex = 64;
            this.textBox_Delay2.Text = "0";
            this.textBox_Delay2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBox_AllOnceTx
            // 
            this.checkBox_AllOnceTx.AutoSize = true;
            this.checkBox_AllOnceTx.Location = new System.Drawing.Point(552, 83);
            this.checkBox_AllOnceTx.Name = "checkBox_AllOnceTx";
            this.checkBox_AllOnceTx.Size = new System.Drawing.Size(118, 19);
            this.checkBox_AllOnceTx.TabIndex = 66;
            this.checkBox_AllOnceTx.Text = "모아서 통합 판정";
            this.checkBox_AllOnceTx.UseVisualStyleBackColor = true;
            this.checkBox_AllOnceTx.CheckedChanged += new System.EventHandler(this.checkBox_AllOnceTx_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(11, 80);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(116, 15);
            this.label20.TabIndex = 63;
            this.label20.Text = "CAM2 Trigger Delay";
            // 
            // checkBox_PINGPONG
            // 
            this.checkBox_PINGPONG.AutoSize = true;
            this.checkBox_PINGPONG.Location = new System.Drawing.Point(552, 51);
            this.checkBox_PINGPONG.Name = "checkBox_PINGPONG";
            this.checkBox_PINGPONG.Size = new System.Drawing.Size(78, 19);
            this.checkBox_PINGPONG.TabIndex = 65;
            this.checkBox_PINGPONG.Text = "교차 전송";
            this.checkBox_PINGPONG.UseVisualStyleBackColor = true;
            this.checkBox_PINGPONG.CheckedChanged += new System.EventHandler(this.checkBox_PINGPONG_CheckedChanged);
            // 
            // textBox_Delay1
            // 
            this.textBox_Delay1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Delay1.Location = new System.Drawing.Point(133, 45);
            this.textBox_Delay1.Name = "textBox_Delay1";
            this.textBox_Delay1.Size = new System.Drawing.Size(48, 25);
            this.textBox_Delay1.TabIndex = 62;
            this.textBox_Delay1.Text = "0";
            this.textBox_Delay1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(11, 49);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(116, 15);
            this.label19.TabIndex = 61;
            this.label19.Text = "CAM1 Trigger Delay";
            // 
            // textBox_Delay0
            // 
            this.textBox_Delay0.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Delay0.Location = new System.Drawing.Point(133, 8);
            this.textBox_Delay0.Name = "textBox_Delay0";
            this.textBox_Delay0.Size = new System.Drawing.Size(48, 25);
            this.textBox_Delay0.TabIndex = 60;
            this.textBox_Delay0.Text = "0";
            this.textBox_Delay0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 12);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(116, 15);
            this.label18.TabIndex = 59;
            this.label18.Text = "CAM0 Trigger Delay";
            // 
            // button_MCRx_Test
            // 
            this.button_MCRx_Test.Location = new System.Drawing.Point(356, 6);
            this.button_MCRx_Test.Name = "button_MCRx_Test";
            this.button_MCRx_Test.Size = new System.Drawing.Size(75, 23);
            this.button_MCRx_Test.TabIndex = 83;
            this.button_MCRx_Test.Text = "Test";
            this.button_MCRx_Test.UseVisualStyleBackColor = true;
            this.button_MCRx_Test.Click += new System.EventHandler(this.button_MCRx_Test_Click);
            // 
            // Ctr_PLC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Ctr_PLC";
            this.Size = new System.Drawing.Size(1410, 676);
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.Ctr_PLC_ControlRemoved);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox_D.ResumeLayout(false);
            this.groupBox_D.PerformLayout();
            this.panel_MC.ResumeLayout(false);
            this.panel_MC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_MC_Tx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MC_Tx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MC_Rx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_MC_Rx)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbStopBits;
        private System.Windows.Forms.ComboBox cbSendFormat;
        private System.Windows.Forms.Button button_SAVE;
        private System.Windows.Forms.ComboBox cbParity;
        private System.Windows.Forms.Button button_LOAD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbReceiveFormat;
        private System.Windows.Forms.ComboBox cbPortName;
        private System.Windows.Forms.ComboBox cbDataBits;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ComboBox cbBaudrate;
        private System.Windows.Forms.TextBox txt1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txt2;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txt_Data;
        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.Button button_L_READ;
        internal System.Windows.Forms.Button button_L_WRITE;
        private System.Windows.Forms.TextBox textBox_L_DATA;
        private System.Windows.Forms.TextBox textBox_L_DEVICE;
        internal System.Windows.Forms.Label label13;
        internal System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox_D;
        private System.Windows.Forms.TextBox textBox_D_SIZE;
        private System.Windows.Forms.TextBox textBox_D_DATA;
        internal System.Windows.Forms.Button button_D_READ;
        internal System.Windows.Forms.Button button_D_WRITE;
        private System.Windows.Forms.TextBox textBox_D_DEVICE;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Label label11;
        internal System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox_Protocal;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBox_SlaveID;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button_View;
        private System.Windows.Forms.TextBox textBox_TxInterval;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox checkBox_PINGPONG;
        private System.Windows.Forms.TextBox textBox_CheckDelay;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.CheckBox checkBox_AllOnceTx;
        private System.Windows.Forms.ComboBox comboBox_NOOBJECT;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.CheckBox checkBox_JView;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox textBox_SERVER_IP;
        private System.Windows.Forms.TextBox textBox_SERVER_PORT;
        private System.Windows.Forms.CheckBox checkBox_SIMULATION;
        private System.Windows.Forms.Button Button_Send_Apply;
        private System.Windows.Forms.TextBox textBox_Delay3;
        internal System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox_Delay2;
        internal System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBox_Delay1;
        internal System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBox_Delay0;
        internal System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBox_CAMREFCNT;
        internal System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox checkBox_MERGETX;
        private System.Windows.Forms.TextBox textBox_ROOMNUM;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox checkBox_Tab_Enable;
        private System.Windows.Forms.TextBox textBox_RESETDURATION;
        internal System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox textBox_DELAYCAMMISS;
        internal System.Windows.Forms.Label label24;
        private System.Windows.Forms.RichTextBox richTextBox_LOG;
        private System.Windows.Forms.Button button_LOG_CLEAR;
        private System.Windows.Forms.TextBox textBox_MinTime;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.CheckBox checkBox_MC;
        private System.Windows.Forms.Panel panel_MC;
        private System.Windows.Forms.CheckBox checkBox_MC_Rx_Use;
        internal System.Windows.Forms.Button button_MC_Rx_Apply;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown numericUpDown_MC_Rx;
        private System.Windows.Forms.DataGridView dataGridView_MC_Rx;
        private System.Windows.Forms.DataGridView dataGridView_MC_Tx;
        private System.Windows.Forms.NumericUpDown numericUpDown_MC_Tx;
        internal System.Windows.Forms.Button button_MC_Tx_Apply;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.CheckBox checkBox_MC_Tx_Use;
        private System.Windows.Forms.Button button_MCRx_Test;
    }
}
