namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_Camera_Setting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_Camera_Setting));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOneShot = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonContinuousShot = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInitialize = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDisconnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_SAVE = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_LOAD = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonImageSave = new System.Windows.Forms.ToolStripButton();
            this.label_Camera = new System.Windows.Forms.Label();
            this.label_Camera_Name = new System.Windows.Forms.Label();
            this.textBox_Camera_Name = new System.Windows.Forms.TextBox();
            this.label_Grab_Num = new System.Windows.Forms.Label();
            this.sliderOffsetY = new PylonC.NETSupportLibrary.SliderUserControl();
            this.sliderOffsetX = new PylonC.NETSupportLibrary.SliderUserControl();
            this.sliderHeight = new PylonC.NETSupportLibrary.SliderUserControl();
            this.sliderWidth = new PylonC.NETSupportLibrary.SliderUserControl();
            this.sliderExposureTime = new PylonC.NETSupportLibrary.SliderUserControl();
            this.sliderGain = new PylonC.NETSupportLibrary.SliderUserControl();
            this.textBox_RESOLUTION_X = new System.Windows.Forms.TextBox();
            this.label_Spatial_Resolution = new System.Windows.Forms.Label();
            this.textBox_RESOLUTION_Y = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_TB = new System.Windows.Forms.CheckBox();
            this.checkBox_LR = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_Change_Rotation = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Force_USE = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_TRIGGER_DELAY = new System.Windows.Forms.TextBox();
            this.comboBox_CAMKIND = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_CO_CAM = new System.Windows.Forms.ComboBox();
            this.comboBoxPixelFormat = new PylonC.NETSupportLibrary.EnumerationComboBoxUserControl();
            this.button_Change_COCAM = new System.Windows.Forms.Button();
            this.button_CAMKIND_Apply = new System.Windows.Forms.Button();
            this.button_TRIGGER_DELAY_CHANGE = new System.Windows.Forms.Button();
            this.button_Change_Rotation = new System.Windows.Forms.Button();
            this.button_Change_Flip = new System.Windows.Forms.Button();
            this.button_Change_Resolution = new System.Windows.Forms.Button();
            this.button_Change_Cam_Name = new System.Windows.Forms.Button();
            this.ctr_MIL_LINK1 = new LV_Inspection_System.GUI.Control.Ctr_MIL_LINK();
            this.GeniCam_sliderGain = new LV_Inspection_System.GUI.Control.GeniCam_SliderUserControl();
            this.GeniCam_sliderExposureTime = new LV_Inspection_System.GUI.Control.GeniCam_SliderUserControl();
            this.GeniCam_sliderWidth = new LV_Inspection_System.GUI.Control.GeniCam_SliderUserControl();
            this.GeniCam_sliderHeight = new LV_Inspection_System.GUI.Control.GeniCam_SliderUserControl();
            this.GeniCam_sliderOffsetX = new LV_Inspection_System.GUI.Control.GeniCam_SliderUserControl();
            this.GeniCam_sliderOffsetY = new LV_Inspection_System.GUI.Control.GeniCam_SliderUserControl();
            this.checkBox_Merge = new System.Windows.Forms.CheckBox();
            this.textBox_Merge = new System.Windows.Forms.TextBox();
            this.button_Merge_Apply = new System.Windows.Forms.Button();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(52, 359);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(125, 16);
            this.checkBox1.TabIndex = 28;
            this.checkBox1.Text = "LINE1 트리거 모드";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOneShot,
            this.toolStripButtonContinuousShot,
            this.toolStripButtonStop,
            this.toolStripButtonInitialize,
            this.toolStripButtonConnect,
            this.toolStripButtonDisconnect,
            this.toolStripButton_SAVE,
            this.toolStripButton_LOAD,
            this.toolStripButtonImageSave});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(37, 705);
            this.toolStrip.TabIndex = 29;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButtonOneShot
            // 
            this.toolStripButtonOneShot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOneShot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOneShot.Image")));
            this.toolStripButtonOneShot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOneShot.Name = "toolStripButtonOneShot";
            this.toolStripButtonOneShot.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonOneShot.Text = "One Shot";
            this.toolStripButtonOneShot.ToolTipText = "촬영";
            this.toolStripButtonOneShot.Click += new System.EventHandler(this.toolStripButtonOneShot_Click);
            // 
            // toolStripButtonContinuousShot
            // 
            this.toolStripButtonContinuousShot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonContinuousShot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonContinuousShot.Image")));
            this.toolStripButtonContinuousShot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonContinuousShot.Name = "toolStripButtonContinuousShot";
            this.toolStripButtonContinuousShot.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonContinuousShot.Text = "Continuous Shot";
            this.toolStripButtonContinuousShot.ToolTipText = "동영상";
            this.toolStripButtonContinuousShot.Click += new System.EventHandler(this.toolStripButtonContinuousShot_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonStop.Text = "Stop Grab";
            this.toolStripButtonStop.ToolTipText = "정지";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // toolStripButtonInitialize
            // 
            this.toolStripButtonInitialize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInitialize.Image = global::LV_Inspection_System.Properties.Resources.Cam_Initialize;
            this.toolStripButtonInitialize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInitialize.Name = "toolStripButtonInitialize";
            this.toolStripButtonInitialize.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonInitialize.Text = "Initialize";
            this.toolStripButtonInitialize.ToolTipText = "리셋";
            this.toolStripButtonInitialize.Click += new System.EventHandler(this.toolStripButtonInitialize_Click);
            // 
            // toolStripButtonConnect
            // 
            this.toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConnect.Image = global::LV_Inspection_System.Properties.Resources.connect_icon;
            this.toolStripButtonConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConnect.Name = "toolStripButtonConnect";
            this.toolStripButtonConnect.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonConnect.Text = "Connect";
            this.toolStripButtonConnect.ToolTipText = "연결";
            this.toolStripButtonConnect.Click += new System.EventHandler(this.toolStripButtonConnect_Click);
            // 
            // toolStripButtonDisconnect
            // 
            this.toolStripButtonDisconnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDisconnect.Image = global::LV_Inspection_System.Properties.Resources.disconnect_icon;
            this.toolStripButtonDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDisconnect.Name = "toolStripButtonDisconnect";
            this.toolStripButtonDisconnect.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonDisconnect.Text = "Disconnect";
            this.toolStripButtonDisconnect.ToolTipText = "연결해제";
            this.toolStripButtonDisconnect.Click += new System.EventHandler(this.toolStripButtonDisconnect_Click);
            // 
            // toolStripButton_SAVE
            // 
            this.toolStripButton_SAVE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_SAVE.Image = global::LV_Inspection_System.Properties.Resources.Save_icon;
            this.toolStripButton_SAVE.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_SAVE.Name = "toolStripButton_SAVE";
            this.toolStripButton_SAVE.Size = new System.Drawing.Size(34, 36);
            this.toolStripButton_SAVE.Text = "Save";
            this.toolStripButton_SAVE.ToolTipText = "저장";
            this.toolStripButton_SAVE.Click += new System.EventHandler(this.toolStripButton_SAVE_Click);
            // 
            // toolStripButton_LOAD
            // 
            this.toolStripButton_LOAD.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_LOAD.Image = global::LV_Inspection_System.Properties.Resources.Load_icon;
            this.toolStripButton_LOAD.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_LOAD.Name = "toolStripButton_LOAD";
            this.toolStripButton_LOAD.Size = new System.Drawing.Size(34, 36);
            this.toolStripButton_LOAD.Text = "Load";
            this.toolStripButton_LOAD.ToolTipText = "불러오기";
            this.toolStripButton_LOAD.Click += new System.EventHandler(this.toolStripButton_LOAD_Click);
            // 
            // toolStripButtonImageSave
            // 
            this.toolStripButtonImageSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonImageSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonImageSave.Image")));
            this.toolStripButtonImageSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonImageSave.Name = "toolStripButtonImageSave";
            this.toolStripButtonImageSave.Size = new System.Drawing.Size(34, 36);
            this.toolStripButtonImageSave.Text = "Image Save";
            this.toolStripButtonImageSave.ToolTipText = "이미지 저장";
            this.toolStripButtonImageSave.Click += new System.EventHandler(this.toolStripButtonImageSave_Click);
            // 
            // label_Camera
            // 
            this.label_Camera.AutoSize = true;
            this.label_Camera.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Camera.Location = new System.Drawing.Point(60, -1);
            this.label_Camera.Name = "label_Camera";
            this.label_Camera.Size = new System.Drawing.Size(46, 17);
            this.label_Camera.TabIndex = 30;
            this.label_Camera.Text = "label1";
            // 
            // label_Camera_Name
            // 
            this.label_Camera_Name.AutoSize = true;
            this.label_Camera_Name.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Camera_Name.Location = new System.Drawing.Point(47, 415);
            this.label_Camera_Name.Name = "label_Camera_Name";
            this.label_Camera_Name.Size = new System.Drawing.Size(86, 17);
            this.label_Camera_Name.TabIndex = 31;
            this.label_Camera_Name.Text = "카메라 이름 :";
            // 
            // textBox_Camera_Name
            // 
            this.textBox_Camera_Name.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Camera_Name.Location = new System.Drawing.Point(157, 412);
            this.textBox_Camera_Name.Name = "textBox_Camera_Name";
            this.textBox_Camera_Name.Size = new System.Drawing.Size(97, 25);
            this.textBox_Camera_Name.TabIndex = 32;
            this.textBox_Camera_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_Grab_Num
            // 
            this.label_Grab_Num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Grab_Num.AutoSize = true;
            this.label_Grab_Num.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Grab_Num.Location = new System.Drawing.Point(245, -1);
            this.label_Grab_Num.Name = "label_Grab_Num";
            this.label_Grab_Num.Size = new System.Drawing.Size(90, 17);
            this.label_Grab_Num.TabIndex = 34;
            this.label_Grab_Num.Text = "[000000000]";
            // 
            // sliderOffsetY
            // 
            this.sliderOffsetY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderOffsetY.Location = new System.Drawing.Point(41, 266);
            this.sliderOffsetY.MaximumSize = new System.Drawing.Size(1021, 46);
            this.sliderOffsetY.MinimumSize = new System.Drawing.Size(262, 46);
            this.sliderOffsetY.Name = "sliderOffsetY";
            this.sliderOffsetY.NodeName = "OffsetY";
            this.sliderOffsetY.Size = new System.Drawing.Size(290, 46);
            this.sliderOffsetY.TabIndex = 27;
            // 
            // sliderOffsetX
            // 
            this.sliderOffsetX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderOffsetX.Location = new System.Drawing.Point(41, 217);
            this.sliderOffsetX.MaximumSize = new System.Drawing.Size(1021, 46);
            this.sliderOffsetX.MinimumSize = new System.Drawing.Size(262, 46);
            this.sliderOffsetX.Name = "sliderOffsetX";
            this.sliderOffsetX.NodeName = "OffsetX";
            this.sliderOffsetX.Size = new System.Drawing.Size(290, 46);
            this.sliderOffsetX.TabIndex = 26;
            // 
            // sliderHeight
            // 
            this.sliderHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderHeight.Location = new System.Drawing.Point(41, 168);
            this.sliderHeight.MaximumSize = new System.Drawing.Size(1021, 46);
            this.sliderHeight.MinimumSize = new System.Drawing.Size(262, 46);
            this.sliderHeight.Name = "sliderHeight";
            this.sliderHeight.NodeName = "Height";
            this.sliderHeight.Size = new System.Drawing.Size(290, 46);
            this.sliderHeight.TabIndex = 25;
            // 
            // sliderWidth
            // 
            this.sliderWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderWidth.Location = new System.Drawing.Point(41, 119);
            this.sliderWidth.MaximumSize = new System.Drawing.Size(1021, 46);
            this.sliderWidth.MinimumSize = new System.Drawing.Size(262, 46);
            this.sliderWidth.Name = "sliderWidth";
            this.sliderWidth.NodeName = "Width";
            this.sliderWidth.Size = new System.Drawing.Size(290, 46);
            this.sliderWidth.TabIndex = 24;
            // 
            // sliderExposureTime
            // 
            this.sliderExposureTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderExposureTime.Location = new System.Drawing.Point(41, 70);
            this.sliderExposureTime.MaximumSize = new System.Drawing.Size(1021, 46);
            this.sliderExposureTime.MinimumSize = new System.Drawing.Size(262, 46);
            this.sliderExposureTime.Name = "sliderExposureTime";
            this.sliderExposureTime.NodeName = "ExposureTimeRaw";
            this.sliderExposureTime.Size = new System.Drawing.Size(290, 46);
            this.sliderExposureTime.TabIndex = 23;
            // 
            // sliderGain
            // 
            this.sliderGain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderGain.Location = new System.Drawing.Point(41, 21);
            this.sliderGain.MaximumSize = new System.Drawing.Size(1021, 46);
            this.sliderGain.MinimumSize = new System.Drawing.Size(262, 46);
            this.sliderGain.Name = "sliderGain";
            this.sliderGain.NodeName = "GainRaw";
            this.sliderGain.Size = new System.Drawing.Size(290, 46);
            this.sliderGain.TabIndex = 22;
            // 
            // textBox_RESOLUTION_X
            // 
            this.textBox_RESOLUTION_X.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_RESOLUTION_X.Location = new System.Drawing.Point(157, 443);
            this.textBox_RESOLUTION_X.Name = "textBox_RESOLUTION_X";
            this.textBox_RESOLUTION_X.Size = new System.Drawing.Size(97, 25);
            this.textBox_RESOLUTION_X.TabIndex = 36;
            this.textBox_RESOLUTION_X.Text = "1";
            this.textBox_RESOLUTION_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_Spatial_Resolution
            // 
            this.label_Spatial_Resolution.AutoSize = true;
            this.label_Spatial_Resolution.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Spatial_Resolution.Location = new System.Drawing.Point(47, 446);
            this.label_Spatial_Resolution.Name = "label_Spatial_Resolution";
            this.label_Spatial_Resolution.Size = new System.Drawing.Size(55, 17);
            this.label_Spatial_Resolution.TabIndex = 35;
            this.label_Spatial_Resolution.Text = "해상도 :";
            // 
            // textBox_RESOLUTION_Y
            // 
            this.textBox_RESOLUTION_Y.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_RESOLUTION_Y.Location = new System.Drawing.Point(157, 474);
            this.textBox_RESOLUTION_Y.Name = "textBox_RESOLUTION_Y";
            this.textBox_RESOLUTION_Y.Size = new System.Drawing.Size(97, 25);
            this.textBox_RESOLUTION_Y.TabIndex = 38;
            this.textBox_RESOLUTION_Y.Text = "1";
            this.textBox_RESOLUTION_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(119, 446);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 39;
            this.label1.Text = "가로";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(118, 477);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 40;
            this.label2.Text = "세로";
            // 
            // checkBox_TB
            // 
            this.checkBox_TB.AutoSize = true;
            this.checkBox_TB.Location = new System.Drawing.Point(178, 542);
            this.checkBox_TB.Name = "checkBox_TB";
            this.checkBox_TB.Size = new System.Drawing.Size(76, 16);
            this.checkBox_TB.TabIndex = 44;
            this.checkBox_TB.Text = "상하 반전";
            this.checkBox_TB.UseVisualStyleBackColor = true;
            // 
            // checkBox_LR
            // 
            this.checkBox_LR.AutoSize = true;
            this.checkBox_LR.Location = new System.Drawing.Point(98, 542);
            this.checkBox_LR.Name = "checkBox_LR";
            this.checkBox_LR.Size = new System.Drawing.Size(76, 16);
            this.checkBox_LR.TabIndex = 43;
            this.checkBox_LR.Text = "좌우 반전";
            this.checkBox_LR.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(47, 541);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 17);
            this.label3.TabIndex = 41;
            this.label3.Text = "반전 :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(47, 571);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 17);
            this.label4.TabIndex = 46;
            this.label4.Text = "이미지 회전 :";
            // 
            // comboBox_Change_Rotation
            // 
            this.comboBox_Change_Rotation.FormattingEnabled = true;
            this.comboBox_Change_Rotation.Items.AddRange(new object[] {
            "None",
            "+90",
            "+180",
            "+270"});
            this.comboBox_Change_Rotation.Location = new System.Drawing.Point(157, 571);
            this.comboBox_Change_Rotation.Name = "comboBox_Change_Rotation";
            this.comboBox_Change_Rotation.Size = new System.Drawing.Size(97, 20);
            this.comboBox_Change_Rotation.TabIndex = 47;
            this.comboBox_Change_Rotation.Text = "None";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 472);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 49;
            this.label5.Text = "(mm/Pixel)";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBox1.ForeColor = System.Drawing.Color.HotPink;
            this.richTextBox1.Location = new System.Drawing.Point(50, 658);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(271, 47);
            this.richTextBox1.TabIndex = 50;
            this.richTextBox1.Text = "";
            // 
            // Force_USE
            // 
            this.Force_USE.AutoSize = true;
            this.Force_USE.Location = new System.Drawing.Point(209, 359);
            this.Force_USE.Name = "Force_USE";
            this.Force_USE.Size = new System.Drawing.Size(112, 16);
            this.Force_USE.TabIndex = 51;
            this.Force_USE.Text = "카메라 사용안함";
            this.Force_USE.UseVisualStyleBackColor = true;
            this.Force_USE.CheckedChanged += new System.EventHandler(this.Force_USE_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(47, 508);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 17);
            this.label6.TabIndex = 52;
            this.label6.Text = "검사 지연값 :";
            // 
            // textBox_TRIGGER_DELAY
            // 
            this.textBox_TRIGGER_DELAY.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_TRIGGER_DELAY.Location = new System.Drawing.Point(157, 505);
            this.textBox_TRIGGER_DELAY.Name = "textBox_TRIGGER_DELAY";
            this.textBox_TRIGGER_DELAY.Size = new System.Drawing.Size(97, 25);
            this.textBox_TRIGGER_DELAY.TabIndex = 53;
            this.textBox_TRIGGER_DELAY.Text = "0";
            this.textBox_TRIGGER_DELAY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboBox_CAMKIND
            // 
            this.comboBox_CAMKIND.FormattingEnabled = true;
            this.comboBox_CAMKIND.Items.AddRange(new object[] {
            "Area(Basler_G)",
            "Line(Basler_G)",
            "Area(Basler_C)",
            "Probe",
            "Line link(MIL)",
            "Area(GenICam_G)",
            "Area(GenICam_C)"});
            this.comboBox_CAMKIND.Location = new System.Drawing.Point(157, 385);
            this.comboBox_CAMKIND.Name = "comboBox_CAMKIND";
            this.comboBox_CAMKIND.Size = new System.Drawing.Size(97, 20);
            this.comboBox_CAMKIND.TabIndex = 56;
            this.comboBox_CAMKIND.Text = "Area(Basler_G)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(47, 386);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 17);
            this.label7.TabIndex = 55;
            this.label7.Text = "카메라 종류 :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(47, 600);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 17);
            this.label8.TabIndex = 58;
            this.label8.Text = "카메라 연동 :";
            // 
            // comboBox_CO_CAM
            // 
            this.comboBox_CO_CAM.FormattingEnabled = true;
            this.comboBox_CO_CAM.Items.AddRange(new object[] {
            "None",
            "CAM0 or 4",
            "CAM1 or 5",
            "CAM2 or 6",
            "CAM3 or 7"});
            this.comboBox_CO_CAM.Location = new System.Drawing.Point(157, 600);
            this.comboBox_CO_CAM.Name = "comboBox_CO_CAM";
            this.comboBox_CO_CAM.Size = new System.Drawing.Size(97, 20);
            this.comboBox_CO_CAM.TabIndex = 59;
            this.comboBox_CO_CAM.Text = "None";
            this.comboBox_CO_CAM.SelectedIndexChanged += new System.EventHandler(this.comboBox_CO_CAM_SelectedIndexChanged);
            // 
            // comboBoxPixelFormat
            // 
            this.comboBoxPixelFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPixelFormat.Location = new System.Drawing.Point(50, 318);
            this.comboBoxPixelFormat.MinimumSize = new System.Drawing.Size(175, 36);
            this.comboBoxPixelFormat.Name = "comboBoxPixelFormat";
            this.comboBoxPixelFormat.NodeName = "PixelFormat";
            this.comboBoxPixelFormat.Size = new System.Drawing.Size(195, 42);
            this.comboBoxPixelFormat.TabIndex = 61;
            // 
            // button_Change_COCAM
            // 
            this.button_Change_COCAM.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Change_COCAM.ForeColor = System.Drawing.Color.White;
            this.button_Change_COCAM.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Change_COCAM.Location = new System.Drawing.Point(261, 596);
            this.button_Change_COCAM.Name = "button_Change_COCAM";
            this.button_Change_COCAM.Size = new System.Drawing.Size(60, 28);
            this.button_Change_COCAM.TabIndex = 60;
            this.button_Change_COCAM.Text = "적용";
            this.button_Change_COCAM.UseVisualStyleBackColor = true;
            this.button_Change_COCAM.Click += new System.EventHandler(this.button_Change_COCAM_Click);
            // 
            // button_CAMKIND_Apply
            // 
            this.button_CAMKIND_Apply.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_CAMKIND_Apply.ForeColor = System.Drawing.Color.White;
            this.button_CAMKIND_Apply.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_CAMKIND_Apply.Location = new System.Drawing.Point(261, 381);
            this.button_CAMKIND_Apply.Name = "button_CAMKIND_Apply";
            this.button_CAMKIND_Apply.Size = new System.Drawing.Size(60, 28);
            this.button_CAMKIND_Apply.TabIndex = 57;
            this.button_CAMKIND_Apply.Text = "적용";
            this.button_CAMKIND_Apply.UseVisualStyleBackColor = true;
            this.button_CAMKIND_Apply.Click += new System.EventHandler(this.button_CAMKIND_Apply_Click);
            // 
            // button_TRIGGER_DELAY_CHANGE
            // 
            this.button_TRIGGER_DELAY_CHANGE.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_TRIGGER_DELAY_CHANGE.ForeColor = System.Drawing.Color.White;
            this.button_TRIGGER_DELAY_CHANGE.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_TRIGGER_DELAY_CHANGE.Location = new System.Drawing.Point(261, 504);
            this.button_TRIGGER_DELAY_CHANGE.Name = "button_TRIGGER_DELAY_CHANGE";
            this.button_TRIGGER_DELAY_CHANGE.Size = new System.Drawing.Size(60, 28);
            this.button_TRIGGER_DELAY_CHANGE.TabIndex = 54;
            this.button_TRIGGER_DELAY_CHANGE.Text = "적용";
            this.button_TRIGGER_DELAY_CHANGE.UseVisualStyleBackColor = true;
            this.button_TRIGGER_DELAY_CHANGE.Click += new System.EventHandler(this.button_TRIGGER_DELAY_CHANGE_Click);
            // 
            // button_Change_Rotation
            // 
            this.button_Change_Rotation.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Change_Rotation.ForeColor = System.Drawing.Color.White;
            this.button_Change_Rotation.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Change_Rotation.Location = new System.Drawing.Point(261, 567);
            this.button_Change_Rotation.Name = "button_Change_Rotation";
            this.button_Change_Rotation.Size = new System.Drawing.Size(60, 28);
            this.button_Change_Rotation.TabIndex = 48;
            this.button_Change_Rotation.Text = "적용";
            this.button_Change_Rotation.UseVisualStyleBackColor = true;
            this.button_Change_Rotation.Click += new System.EventHandler(this.button_Change_Rotation_Click);
            // 
            // button_Change_Flip
            // 
            this.button_Change_Flip.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Change_Flip.ForeColor = System.Drawing.Color.White;
            this.button_Change_Flip.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Change_Flip.Location = new System.Drawing.Point(261, 535);
            this.button_Change_Flip.Name = "button_Change_Flip";
            this.button_Change_Flip.Size = new System.Drawing.Size(60, 28);
            this.button_Change_Flip.TabIndex = 45;
            this.button_Change_Flip.Text = "적용";
            this.button_Change_Flip.UseVisualStyleBackColor = true;
            this.button_Change_Flip.Click += new System.EventHandler(this.button_Change_Flip_Click);
            // 
            // button_Change_Resolution
            // 
            this.button_Change_Resolution.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Change_Resolution.ForeColor = System.Drawing.Color.White;
            this.button_Change_Resolution.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Change_Resolution.Location = new System.Drawing.Point(261, 443);
            this.button_Change_Resolution.Name = "button_Change_Resolution";
            this.button_Change_Resolution.Size = new System.Drawing.Size(60, 56);
            this.button_Change_Resolution.TabIndex = 37;
            this.button_Change_Resolution.Text = "적용";
            this.button_Change_Resolution.UseVisualStyleBackColor = true;
            this.button_Change_Resolution.Click += new System.EventHandler(this.button_Change_Resolution_Click);
            // 
            // button_Change_Cam_Name
            // 
            this.button_Change_Cam_Name.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Change_Cam_Name.ForeColor = System.Drawing.Color.White;
            this.button_Change_Cam_Name.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Change_Cam_Name.Location = new System.Drawing.Point(261, 411);
            this.button_Change_Cam_Name.Name = "button_Change_Cam_Name";
            this.button_Change_Cam_Name.Size = new System.Drawing.Size(60, 28);
            this.button_Change_Cam_Name.TabIndex = 33;
            this.button_Change_Cam_Name.Text = "적용";
            this.button_Change_Cam_Name.UseVisualStyleBackColor = true;
            this.button_Change_Cam_Name.Click += new System.EventHandler(this.button_Change_Cam_Name_Click);
            // 
            // ctr_MIL_LINK1
            // 
            this.ctr_MIL_LINK1.Location = new System.Drawing.Point(46, 21);
            this.ctr_MIL_LINK1.m_SetLanguage = 0;
            this.ctr_MIL_LINK1.Name = "ctr_MIL_LINK1";
            this.ctr_MIL_LINK1.Size = new System.Drawing.Size(285, 332);
            this.ctr_MIL_LINK1.TabIndex = 62;
            this.ctr_MIL_LINK1.Visible = false;
            // 
            // GeniCam_sliderGain
            // 
            this.GeniCam_sliderGain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeniCam_sliderGain.Location = new System.Drawing.Point(41, 21);
            this.GeniCam_sliderGain.MaximumSize = new System.Drawing.Size(1021, 46);
            this.GeniCam_sliderGain.MinimumSize = new System.Drawing.Size(262, 46);
            this.GeniCam_sliderGain.Name = "GeniCam_sliderGain";
            this.GeniCam_sliderGain.NodeName = "GainRaw";
            this.GeniCam_sliderGain.Size = new System.Drawing.Size(290, 46);
            this.GeniCam_sliderGain.TabIndex = 22;
            this.GeniCam_sliderGain.Visible = false;
            // 
            // GeniCam_sliderExposureTime
            // 
            this.GeniCam_sliderExposureTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeniCam_sliderExposureTime.Location = new System.Drawing.Point(41, 70);
            this.GeniCam_sliderExposureTime.MaximumSize = new System.Drawing.Size(1021, 46);
            this.GeniCam_sliderExposureTime.MinimumSize = new System.Drawing.Size(262, 46);
            this.GeniCam_sliderExposureTime.Name = "GeniCam_sliderExposureTime";
            this.GeniCam_sliderExposureTime.NodeName = "ExposureTime";
            this.GeniCam_sliderExposureTime.Size = new System.Drawing.Size(290, 46);
            this.GeniCam_sliderExposureTime.TabIndex = 23;
            this.GeniCam_sliderExposureTime.Visible = false;
            // 
            // GeniCam_sliderWidth
            // 
            this.GeniCam_sliderWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeniCam_sliderWidth.Location = new System.Drawing.Point(41, 119);
            this.GeniCam_sliderWidth.MaximumSize = new System.Drawing.Size(1021, 46);
            this.GeniCam_sliderWidth.MinimumSize = new System.Drawing.Size(262, 46);
            this.GeniCam_sliderWidth.Name = "GeniCam_sliderWidth";
            this.GeniCam_sliderWidth.NodeName = "Width";
            this.GeniCam_sliderWidth.Size = new System.Drawing.Size(290, 46);
            this.GeniCam_sliderWidth.TabIndex = 24;
            this.GeniCam_sliderWidth.Visible = false;
            // 
            // GeniCam_sliderHeight
            // 
            this.GeniCam_sliderHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeniCam_sliderHeight.Location = new System.Drawing.Point(41, 168);
            this.GeniCam_sliderHeight.MaximumSize = new System.Drawing.Size(1021, 46);
            this.GeniCam_sliderHeight.MinimumSize = new System.Drawing.Size(262, 46);
            this.GeniCam_sliderHeight.Name = "GeniCam_sliderHeight";
            this.GeniCam_sliderHeight.NodeName = "Height";
            this.GeniCam_sliderHeight.Size = new System.Drawing.Size(290, 46);
            this.GeniCam_sliderHeight.TabIndex = 25;
            this.GeniCam_sliderHeight.Visible = false;
            // 
            // GeniCam_sliderOffsetX
            // 
            this.GeniCam_sliderOffsetX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeniCam_sliderOffsetX.Location = new System.Drawing.Point(41, 217);
            this.GeniCam_sliderOffsetX.MaximumSize = new System.Drawing.Size(1021, 46);
            this.GeniCam_sliderOffsetX.MinimumSize = new System.Drawing.Size(262, 46);
            this.GeniCam_sliderOffsetX.Name = "GeniCam_sliderOffsetX";
            this.GeniCam_sliderOffsetX.NodeName = "OffsetX";
            this.GeniCam_sliderOffsetX.Size = new System.Drawing.Size(290, 46);
            this.GeniCam_sliderOffsetX.TabIndex = 26;
            this.GeniCam_sliderOffsetX.Visible = false;
            // 
            // GeniCam_sliderOffsetY
            // 
            this.GeniCam_sliderOffsetY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeniCam_sliderOffsetY.Location = new System.Drawing.Point(41, 266);
            this.GeniCam_sliderOffsetY.MaximumSize = new System.Drawing.Size(1021, 46);
            this.GeniCam_sliderOffsetY.MinimumSize = new System.Drawing.Size(262, 46);
            this.GeniCam_sliderOffsetY.Name = "GeniCam_sliderOffsetY";
            this.GeniCam_sliderOffsetY.NodeName = "OffsetY";
            this.GeniCam_sliderOffsetY.Size = new System.Drawing.Size(290, 46);
            this.GeniCam_sliderOffsetY.TabIndex = 27;
            this.GeniCam_sliderOffsetY.Visible = false;
            // 
            // checkBox_Merge
            // 
            this.checkBox_Merge.AutoSize = true;
            this.checkBox_Merge.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBox_Merge.Location = new System.Drawing.Point(50, 630);
            this.checkBox_Merge.Name = "checkBox_Merge";
            this.checkBox_Merge.Size = new System.Drawing.Size(106, 19);
            this.checkBox_Merge.TabIndex = 63;
            this.checkBox_Merge.Text = "Image Merge";
            this.checkBox_Merge.UseVisualStyleBackColor = true;
            this.checkBox_Merge.CheckedChanged += new System.EventHandler(this.checkBox_Merge_CheckedChanged);
            // 
            // textBox_Merge
            // 
            this.textBox_Merge.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Merge.Location = new System.Drawing.Point(157, 627);
            this.textBox_Merge.Name = "textBox_Merge";
            this.textBox_Merge.Size = new System.Drawing.Size(97, 25);
            this.textBox_Merge.TabIndex = 64;
            this.textBox_Merge.Text = "70";
            this.textBox_Merge.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button_Merge_Apply
            // 
            this.button_Merge_Apply.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Merge_Apply.ForeColor = System.Drawing.Color.White;
            this.button_Merge_Apply.Image = global::LV_Inspection_System.Properties.Resources.Button_BG;
            this.button_Merge_Apply.Location = new System.Drawing.Point(261, 625);
            this.button_Merge_Apply.Name = "button_Merge_Apply";
            this.button_Merge_Apply.Size = new System.Drawing.Size(60, 28);
            this.button_Merge_Apply.TabIndex = 65;
            this.button_Merge_Apply.Text = "적용";
            this.button_Merge_Apply.UseVisualStyleBackColor = true;
            this.button_Merge_Apply.Click += new System.EventHandler(this.button_Merge_Apply_Click);
            // 
            // Ctr_Camera_Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.button_Merge_Apply);
            this.Controls.Add(this.textBox_Merge);
            this.Controls.Add(this.checkBox_Merge);
            this.Controls.Add(this.comboBoxPixelFormat);
            this.Controls.Add(this.button_Change_COCAM);
            this.Controls.Add(this.comboBox_CO_CAM);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button_CAMKIND_Apply);
            this.Controls.Add(this.comboBox_CAMKIND);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button_TRIGGER_DELAY_CHANGE);
            this.Controls.Add(this.textBox_TRIGGER_DELAY);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Force_USE);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_Change_Rotation);
            this.Controls.Add(this.comboBox_Change_Rotation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_Change_Flip);
            this.Controls.Add(this.checkBox_TB);
            this.Controls.Add(this.checkBox_LR);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_RESOLUTION_Y);
            this.Controls.Add(this.button_Change_Resolution);
            this.Controls.Add(this.textBox_RESOLUTION_X);
            this.Controls.Add(this.label_Spatial_Resolution);
            this.Controls.Add(this.label_Grab_Num);
            this.Controls.Add(this.button_Change_Cam_Name);
            this.Controls.Add(this.textBox_Camera_Name);
            this.Controls.Add(this.label_Camera_Name);
            this.Controls.Add(this.label_Camera);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.sliderOffsetY);
            this.Controls.Add(this.sliderOffsetX);
            this.Controls.Add(this.sliderHeight);
            this.Controls.Add(this.sliderWidth);
            this.Controls.Add(this.sliderExposureTime);
            this.Controls.Add(this.sliderGain);
            this.Controls.Add(this.GeniCam_sliderOffsetY);
            this.Controls.Add(this.GeniCam_sliderOffsetX);
            this.Controls.Add(this.GeniCam_sliderHeight);
            this.Controls.Add(this.GeniCam_sliderWidth);
            this.Controls.Add(this.GeniCam_sliderExposureTime);
            this.Controls.Add(this.GeniCam_sliderGain);
            this.Controls.Add(this.ctr_MIL_LINK1);
            this.Name = "Ctr_Camera_Setting";
            this.Size = new System.Drawing.Size(339, 705);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox checkBox1;
        public PylonC.NETSupportLibrary.SliderUserControl sliderOffsetY;
        public PylonC.NETSupportLibrary.SliderUserControl sliderOffsetX;
        public PylonC.NETSupportLibrary.SliderUserControl sliderHeight;
        public PylonC.NETSupportLibrary.SliderUserControl sliderWidth;
        public PylonC.NETSupportLibrary.SliderUserControl sliderExposureTime;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonOneShot;
        private System.Windows.Forms.ToolStripButton toolStripButtonContinuousShot;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.ToolStripButton toolStripButtonInitialize;
        private System.Windows.Forms.Label label_Camera;
        private System.Windows.Forms.ToolStripButton toolStripButtonDisconnect;
        private System.Windows.Forms.ToolStripButton toolStripButtonConnect;
        private System.Windows.Forms.Label label_Camera_Name;
        private System.Windows.Forms.TextBox textBox_Camera_Name;
        private System.Windows.Forms.Button button_Change_Cam_Name;
        private System.Windows.Forms.Label label_Grab_Num;
        private System.Windows.Forms.Label label_Spatial_Resolution;
        private System.Windows.Forms.Button button_Change_Resolution;
        private System.Windows.Forms.ToolStripButton toolStripButton_SAVE;
        private System.Windows.Forms.ToolStripButton toolStripButton_LOAD;
        public System.Windows.Forms.TextBox textBox_RESOLUTION_Y;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_TB;
        private System.Windows.Forms.CheckBox checkBox_LR;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Change_Flip;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_Change_Rotation;
        private System.Windows.Forms.Button button_Change_Rotation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.CheckBox Force_USE;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_TRIGGER_DELAY_CHANGE;
        private System.Windows.Forms.TextBox textBox_TRIGGER_DELAY;
        public System.Windows.Forms.TextBox textBox_RESOLUTION_X;
        private System.Windows.Forms.Button button_CAMKIND_Apply;
        private System.Windows.Forms.ComboBox comboBox_CAMKIND;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripButton toolStripButtonImageSave;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_CO_CAM;
        private System.Windows.Forms.Button button_Change_COCAM;
        private PylonC.NETSupportLibrary.EnumerationComboBoxUserControl comboBoxPixelFormat;
        public Ctr_MIL_LINK ctr_MIL_LINK1;
        public PylonC.NETSupportLibrary.SliderUserControl sliderGain;

        public GeniCam_SliderUserControl GeniCam_sliderGain;
        public GeniCam_SliderUserControl GeniCam_sliderExposureTime;
        public GeniCam_SliderUserControl GeniCam_sliderOffsetY;
        public GeniCam_SliderUserControl GeniCam_sliderOffsetX;
        public GeniCam_SliderUserControl GeniCam_sliderHeight;
        public GeniCam_SliderUserControl GeniCam_sliderWidth;
        private System.Windows.Forms.CheckBox checkBox_Merge;
        private System.Windows.Forms.TextBox textBox_Merge;
        private System.Windows.Forms.Button button_Merge_Apply;
    }
}
