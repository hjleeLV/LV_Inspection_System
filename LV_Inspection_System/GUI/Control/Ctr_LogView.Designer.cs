﻿namespace LV_Inspection_System.GUI.Control
{
    partial class Ctr_LogView
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
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.ForeColor = System.Drawing.SystemColors.Info;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(676, 458);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // Ctr_LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtLog);
            this.Name = "Ctr_LogView";
            this.Size = new System.Drawing.Size(676, 458);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox txtLog;
    }
}
