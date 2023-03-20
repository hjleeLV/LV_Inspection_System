namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_MIL_LINK
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbPortName = new System.Windows.Forms.ComboBox();
            this.button_SEND = new System.Windows.Forms.Button();
            this.button_OPEN = new System.Windows.Forms.Button();
            this.button_CLOSE = new System.Windows.Forms.Button();
            this.textBox_DCF = new System.Windows.Forms.TextBox();
            this.button_DCF = new System.Windows.Forms.Button();
            this.richTextBox_SCRIPT_Auto = new System.Windows.Forms.RichTextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBox_GRABBER = new System.Windows.Forms.ComboBox();
            this.comboBox_CAMERA = new System.Windows.Forms.ComboBox();
            this.tabControl_COMMAND = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBox_SCRIPT_Manual = new System.Windows.Forms.RichTextBox();
            this.checkBox_NOMANUAL = new System.Windows.Forms.CheckBox();
            this.comboBox_GBOARD = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.richTextBox_SCRIPT = new System.Windows.Forms.RichTextBox();
            this.tabControl_COMMAND.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
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
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14",
            "COM15",
            "COM16"});
            this.cbPortName.Location = new System.Drawing.Point(4, 60);
            this.cbPortName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbPortName.Name = "cbPortName";
            this.cbPortName.Size = new System.Drawing.Size(66, 20);
            this.cbPortName.TabIndex = 20;
            this.cbPortName.Text = "COM1";
            // 
            // button_SEND
            // 
            this.button_SEND.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SEND.Location = new System.Drawing.Point(202, 59);
            this.button_SEND.Name = "button_SEND";
            this.button_SEND.Size = new System.Drawing.Size(73, 23);
            this.button_SEND.TabIndex = 21;
            this.button_SEND.Text = "Send";
            this.button_SEND.UseVisualStyleBackColor = true;
            this.button_SEND.Click += new System.EventHandler(this.button_SEND_Click);
            // 
            // button_OPEN
            // 
            this.button_OPEN.Location = new System.Drawing.Point(76, 59);
            this.button_OPEN.Name = "button_OPEN";
            this.button_OPEN.Size = new System.Drawing.Size(57, 23);
            this.button_OPEN.TabIndex = 22;
            this.button_OPEN.Text = "Open";
            this.button_OPEN.UseVisualStyleBackColor = true;
            this.button_OPEN.Click += new System.EventHandler(this.button_OPEN_Click);
            // 
            // button_CLOSE
            // 
            this.button_CLOSE.Location = new System.Drawing.Point(139, 59);
            this.button_CLOSE.Name = "button_CLOSE";
            this.button_CLOSE.Size = new System.Drawing.Size(57, 23);
            this.button_CLOSE.TabIndex = 23;
            this.button_CLOSE.Text = "Close";
            this.button_CLOSE.UseVisualStyleBackColor = true;
            this.button_CLOSE.Click += new System.EventHandler(this.button_CLOSE_Click);
            // 
            // textBox_DCF
            // 
            this.textBox_DCF.Location = new System.Drawing.Point(126, 33);
            this.textBox_DCF.Name = "textBox_DCF";
            this.textBox_DCF.ReadOnly = true;
            this.textBox_DCF.Size = new System.Drawing.Size(94, 21);
            this.textBox_DCF.TabIndex = 24;
            // 
            // button_DCF
            // 
            this.button_DCF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_DCF.Location = new System.Drawing.Point(226, 32);
            this.button_DCF.Name = "button_DCF";
            this.button_DCF.Size = new System.Drawing.Size(48, 23);
            this.button_DCF.TabIndex = 25;
            this.button_DCF.Text = "DCF";
            this.button_DCF.UseVisualStyleBackColor = true;
            this.button_DCF.Click += new System.EventHandler(this.button_DCF_Click);
            // 
            // richTextBox_SCRIPT_Auto
            // 
            this.richTextBox_SCRIPT_Auto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_SCRIPT_Auto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_SCRIPT_Auto.Location = new System.Drawing.Point(3, 3);
            this.richTextBox_SCRIPT_Auto.Name = "richTextBox_SCRIPT_Auto";
            this.richTextBox_SCRIPT_Auto.Size = new System.Drawing.Size(257, 259);
            this.richTextBox_SCRIPT_Auto.TabIndex = 26;
            this.richTextBox_SCRIPT_Auto.Text = "";
            // 
            // comboBox_GRABBER
            // 
            this.comboBox_GRABBER.FormattingEnabled = true;
            this.comboBox_GRABBER.Items.AddRange(new object[] {
            "Grabber DEV0",
            "Grabber DEV1"});
            this.comboBox_GRABBER.Location = new System.Drawing.Point(4, 5);
            this.comboBox_GRABBER.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_GRABBER.Name = "comboBox_GRABBER";
            this.comboBox_GRABBER.Size = new System.Drawing.Size(129, 20);
            this.comboBox_GRABBER.TabIndex = 27;
            this.comboBox_GRABBER.Text = "Grabber DEV0";
            this.comboBox_GRABBER.SelectedIndexChanged += new System.EventHandler(this.comboBox_GRABBER_SelectedIndexChanged);
            // 
            // comboBox_CAMERA
            // 
            this.comboBox_CAMERA.FormattingEnabled = true;
            this.comboBox_CAMERA.Items.AddRange(new object[] {
            "Camera CH0",
            "Camera CH1"});
            this.comboBox_CAMERA.Location = new System.Drawing.Point(145, 5);
            this.comboBox_CAMERA.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_CAMERA.Name = "comboBox_CAMERA";
            this.comboBox_CAMERA.Size = new System.Drawing.Size(129, 20);
            this.comboBox_CAMERA.TabIndex = 28;
            this.comboBox_CAMERA.Text = "Camera CH0";
            this.comboBox_CAMERA.SelectedIndexChanged += new System.EventHandler(this.comboBox_CAMERA_SelectedIndexChanged);
            // 
            // tabControl_COMMAND
            // 
            this.tabControl_COMMAND.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl_COMMAND.Controls.Add(this.tabPage3);
            this.tabControl_COMMAND.Controls.Add(this.tabPage1);
            this.tabControl_COMMAND.Controls.Add(this.tabPage2);
            this.tabControl_COMMAND.Location = new System.Drawing.Point(4, 88);
            this.tabControl_COMMAND.Name = "tabControl_COMMAND";
            this.tabControl_COMMAND.SelectedIndex = 0;
            this.tabControl_COMMAND.Size = new System.Drawing.Size(271, 291);
            this.tabControl_COMMAND.TabIndex = 29;
            this.tabControl_COMMAND.SelectedIndexChanged += new System.EventHandler(this.tabControl_COMMAND_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox_SCRIPT_Auto);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(263, 265);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "AUTO Mode";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox_SCRIPT_Manual);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(263, 265);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "MANUAL Mode";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox_SCRIPT_Manual
            // 
            this.richTextBox_SCRIPT_Manual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_SCRIPT_Manual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_SCRIPT_Manual.Location = new System.Drawing.Point(3, 3);
            this.richTextBox_SCRIPT_Manual.Name = "richTextBox_SCRIPT_Manual";
            this.richTextBox_SCRIPT_Manual.Size = new System.Drawing.Size(257, 259);
            this.richTextBox_SCRIPT_Manual.TabIndex = 27;
            this.richTextBox_SCRIPT_Manual.Text = "";
            // 
            // checkBox_NOMANUAL
            // 
            this.checkBox_NOMANUAL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_NOMANUAL.AutoSize = true;
            this.checkBox_NOMANUAL.Location = new System.Drawing.Point(226, 90);
            this.checkBox_NOMANUAL.Name = "checkBox_NOMANUAL";
            this.checkBox_NOMANUAL.Size = new System.Drawing.Size(46, 16);
            this.checkBox_NOMANUAL.TabIndex = 30;
            this.checkBox_NOMANUAL.Text = "Use";
            this.checkBox_NOMANUAL.UseVisualStyleBackColor = true;
            this.checkBox_NOMANUAL.CheckedChanged += new System.EventHandler(this.checkBox_NOMANUAL_CheckedChanged);
            // 
            // comboBox_GBOARD
            // 
            this.comboBox_GBOARD.FormattingEnabled = true;
            this.comboBox_GBOARD.Items.AddRange(new object[] {
            "SOLIOS",
            "RADIENTEVCL"});
            this.comboBox_GBOARD.Location = new System.Drawing.Point(4, 33);
            this.comboBox_GBOARD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_GBOARD.Name = "comboBox_GBOARD";
            this.comboBox_GBOARD.Size = new System.Drawing.Size(116, 20);
            this.comboBox_GBOARD.TabIndex = 31;
            this.comboBox_GBOARD.Text = "RADIENTEVCL";
            this.comboBox_GBOARD.SelectedIndexChanged += new System.EventHandler(this.comboBox_GBOARD_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.richTextBox_SCRIPT);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(263, 265);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Script";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBox_SCRIPT
            // 
            this.richTextBox_SCRIPT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_SCRIPT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_SCRIPT.Location = new System.Drawing.Point(3, 3);
            this.richTextBox_SCRIPT.Name = "richTextBox_SCRIPT";
            this.richTextBox_SCRIPT.Size = new System.Drawing.Size(257, 259);
            this.richTextBox_SCRIPT.TabIndex = 28;
            this.richTextBox_SCRIPT.Text = "";
            // 
            // Ctr_MIL_LINK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBox_GBOARD);
            this.Controls.Add(this.checkBox_NOMANUAL);
            this.Controls.Add(this.tabControl_COMMAND);
            this.Controls.Add(this.comboBox_CAMERA);
            this.Controls.Add(this.comboBox_GRABBER);
            this.Controls.Add(this.button_DCF);
            this.Controls.Add(this.textBox_DCF);
            this.Controls.Add(this.button_CLOSE);
            this.Controls.Add(this.button_OPEN);
            this.Controls.Add(this.button_SEND);
            this.Controls.Add(this.cbPortName);
            this.Name = "Ctr_MIL_LINK";
            this.Size = new System.Drawing.Size(286, 382);
            this.tabControl_COMMAND.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPortName;
        private System.Windows.Forms.Button button_SEND;
        private System.Windows.Forms.Button button_OPEN;
        private System.Windows.Forms.Button button_CLOSE;
        private System.Windows.Forms.TextBox textBox_DCF;
        private System.Windows.Forms.Button button_DCF;
        private System.Windows.Forms.RichTextBox richTextBox_SCRIPT_Auto;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboBox_GRABBER;
        private System.Windows.Forms.ComboBox comboBox_CAMERA;
        private System.Windows.Forms.TabControl tabControl_COMMAND;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox richTextBox_SCRIPT_Manual;
        private System.Windows.Forms.CheckBox checkBox_NOMANUAL;
        private System.Windows.Forms.ComboBox comboBox_GBOARD;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox richTextBox_SCRIPT;
    }
}
