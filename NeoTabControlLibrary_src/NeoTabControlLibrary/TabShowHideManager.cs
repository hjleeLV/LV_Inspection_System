using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoTabControlLibrary
{
    public class TabShowHideManager : Form
    {
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Managed Resources
                foreach (Control control in this.Controls)
                    control.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabList = new System.Windows.Forms.CheckedListBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabList
            // 
            this.tabList.FormatString = "T";
            this.tabList.FormattingEnabled = true;
            this.tabList.Location = new System.Drawing.Point(6, 31);
            this.tabList.Name = "tabList";
            this.tabList.Size = new System.Drawing.Size(318, 109);
            this.tabList.Sorted = true;
            this.tabList.TabIndex = 0;
            this.tabList.SelectedIndexChanged += new System.EventHandler(this.tabList_SelectedIndexChanged);
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(170, 146);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(249, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(6, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 100);
            this.panel1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(318, 64);
            this.label2.TabIndex = 6;
            this.label2.Text = "desc...";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(318, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "Number of visits:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.Location = new System.Drawing.Point(6, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 25);
            this.button1.TabIndex = 6;
            this.button1.Text = "-";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Details";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.tabList);
            this.panel2.Controls.Add(this.btnApply);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Location = new System.Drawing.Point(6, 108);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(330, 175);
            this.panel2.TabIndex = 8;
            // 
            // TabShowHideManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 287);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TabShowHideManager";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Show / Hide Tab Manager";
            this.Load += new System.EventHandler(this.TabShowHideManager_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox tabList;
        private System.Windows.Forms.Button btnApply;
        private Panel panel1;
        private Label label2;
        private Label label1;
        private Button button1;
        private Label label3;
        private Panel panel2;
        private System.Windows.Forms.Button btnCancel;

        #region Constructor
        
        public TabShowHideManager()
        {
            InitializeComponent();
        }

        #endregion
                
        #region Destructor

        ~TabShowHideManager()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        public NeoTabPage[] SelectedItems
        {
            get
            {
                NeoTabPage[] items =
                    new NeoTabPage[tabList.CheckedItems.Count];
                tabList.CheckedItems.CopyTo(items, 0);
                return items;
            }
        }

        public List<NeoTabPage> UnSelectedItems
        {
            get
            {
                List<NeoTabPage> items =
                    new List<NeoTabPage>();
                for (int i = 0; i < tabList.Items.Count; i++)
                {
                    if (!tabList.GetItemChecked(i))
                        items.Add(tabList.Items[i] as NeoTabPage);
                }
                return items;
            }
        }

        #endregion

        #region Helper Methods - Event Handlers

        private void tabList_SelectedIndexChanged(object sender, EventArgs e)
        {
            NeoTabPage tp = tabList.SelectedItem as NeoTabPage;
            if (tp != null)
            {
                label1.Text = String.Format("Number of visits: {0}",
                    tp.ProgressUsedValue);
                label2.Text = tp.ToolTipText;
            }
        }

        private void TabShowHideManager_Load(object sender, EventArgs e)
        {
            if (tabList.Items.Count > 0)
                tabList.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (panel1.Visible)
            {
                panel1.Visible = false;
                button1.Text = "+";
                this.Height -= panel1.Height;
                panel2.Top -= panel1.Height;
            }
            else
            {
                panel1.Visible = true;
                button1.Text = "-";
                this.Height += panel1.Height;
                panel2.Top += panel1.Height;
            }
        }

        #endregion

        #region General Methods

        public void AddNewItem(NeoTabPage tp, bool isChecked)
        {
            if (tp != null)
                tabList.Items.Add(tp, isChecked);
        }

        #endregion
    }
}