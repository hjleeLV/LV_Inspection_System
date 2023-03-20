namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_History
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
            this.txtHistory = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtHistory
            // 
            this.txtHistory.BackColor = System.Drawing.Color.White;
            this.txtHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHistory.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtHistory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtHistory.Location = new System.Drawing.Point(0, 0);
            this.txtHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHistory.Multiline = true;
            this.txtHistory.Name = "txtHistory";
            this.txtHistory.ReadOnly = true;
            this.txtHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHistory.Size = new System.Drawing.Size(397, 427);
            this.txtHistory.TabIndex = 48;
            // 
            // Ctr_History
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtHistory);
            this.Name = "Ctr_History";
            this.Size = new System.Drawing.Size(397, 427);
            this.Load += new System.EventHandler(this.Ctr_History_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtHistory;
    }
}
