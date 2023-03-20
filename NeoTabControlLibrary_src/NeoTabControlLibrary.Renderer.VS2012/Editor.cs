using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace NeoTabControlLibrary.Renderer.VS2012
{
    public class Editor : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(435, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customize the template settings using the property grid.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(85, 93);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 21);
            this.button3.TabIndex = 7;
            this.button3.Text = "Default Settings";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.BackColor = System.Drawing.SystemColors.Control;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 90);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(435, 334);
            this.propertyGrid1.TabIndex = 8;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 46);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(435, 44);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Padding = new System.Windows.Forms.Padding(4);
            this.toolStripButton1.Size = new System.Drawing.Size(46, 41);
            this.toolStripButton1.Text = "Apply";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Padding = new System.Windows.Forms.Padding(4);
            this.toolStripButton2.Size = new System.Drawing.Size(51, 40);
            this.toolStripButton2.Text = "Cancel";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 424);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Editor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VS2012Like Renderer Editor";
            this.Load += new System.EventHandler(this.Editor_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private PropertyGrid propertyGrid1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private Button button3;

        public Editor()
        {
            InitializeComponent();
        }
        
        #region Property

        public Settings TemplateSettings { get; set; }

        #endregion

        private void Editor_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = TemplateSettings;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = null;
            TemplateSettings = new Settings()
            {
                NeoTabPageItemsFont = new Font("Arial", 8.25f),
                BackColor = Color.FromArgb(85, 41, 41),
                TabPageItemForeColor = Color.White,
                SelectedTabPageItemForeColor = Color.White,
                DisabledTabPageItemForeColor = SystemColors.GrayText,
                MouseOverTabPageItemForeColor = Color.White,
                TabItemFirstColor = Color.FromArgb(130, 77, 77),
                TabItemSecondColor = Color.FromArgb(120, 63, 63),
                TabItemHoverFirstColor = Color.FromArgb(140, 85, 85),
                TabItemHoverSecondColor = Color.FromArgb(130, 75, 75),
                ItemObjectsDrawingMargin = 4,
                TabPageItemsBetweenSpacing = 1
            };
            propertyGrid1.SelectedObject = TemplateSettings;
        }
    }
}