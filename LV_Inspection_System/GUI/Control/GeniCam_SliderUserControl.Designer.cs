namespace LV_Inspection_System.GUI.Control
{
    partial class GeniCam_SliderUserControl
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
            this.labelCurrentValue = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelMax = new System.Windows.Forms.Label();
            this.labelMin = new System.Windows.Forms.Label();
            this.slider = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCurrentValue
            // 
            this.labelCurrentValue.Location = new System.Drawing.Point(9, 25);
            this.labelCurrentValue.Name = "labelCurrentValue";
            this.labelCurrentValue.Size = new System.Drawing.Size(67, 21);
            this.labelCurrentValue.TabIndex = 7;
            this.labelCurrentValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.labelCurrentValue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.labelCurrentValue_KeyUp);
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(6, 5);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(149, 12);
            this.labelName.TabIndex = 4;
            this.labelName.Text = "ValueName";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMax
            // 
            this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMax.AutoSize = true;
            this.labelMax.Location = new System.Drawing.Point(232, 30);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(30, 12);
            this.labelMax.TabIndex = 5;
            this.labelMax.Text = "Max";
            // 
            // labelMin
            // 
            this.labelMin.AutoSize = true;
            this.labelMin.Location = new System.Drawing.Point(82, 30);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(26, 12);
            this.labelMin.TabIndex = 6;
            this.labelMin.Text = "Min";
            this.labelMin.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // slider
            // 
            this.slider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.slider.Location = new System.Drawing.Point(99, 15);
            this.slider.Name = "slider";
            this.slider.Size = new System.Drawing.Size(141, 45);
            this.slider.TabIndex = 3;
            this.slider.TickFrequency = 100;
            this.slider.Scroll += new System.EventHandler(this.slider_Scroll);
            // 
            // GeniCam_SliderUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelCurrentValue);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.labelMax);
            this.Controls.Add(this.labelMin);
            this.Controls.Add(this.slider);
            this.Name = "GeniCam_SliderUserControl";
            this.Size = new System.Drawing.Size(262, 46);
            ((System.ComponentModel.ISupportInitialize)(this.slider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox labelCurrentValue;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.Label labelMin;
        public System.Windows.Forms.TrackBar slider;
    }
}
