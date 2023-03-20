namespace NeoTabControlClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.tabPageItemEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.neoTabWindow1 = new NeoTabControlLibrary.NeoTabWindow();
            this.neoTabPage1 = new NeoTabControlLibrary.NeoTabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.neoTabPage2 = new NeoTabControlLibrary.NeoTabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.label2 = new System.Windows.Forms.Label();
            this.neoTabPage3 = new NeoTabControlLibrary.NeoTabPage();
            this.webBrowser2 = new System.Windows.Forms.WebBrowser();
            this.label3 = new System.Windows.Forms.Label();
            this.neoTabPage4 = new NeoTabControlLibrary.NeoTabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.neoTabPage5 = new NeoTabControlLibrary.NeoTabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neoTabWindow1)).BeginInit();
            this.neoTabWindow1.SuspendLayout();
            this.neoTabPage1.SuspendLayout();
            this.neoTabPage2.SuspendLayout();
            this.neoTabPage3.SuspendLayout();
            this.neoTabPage4.SuspendLayout();
            this.neoTabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton4,
            this.toolStripDropDownButton1,
            this.toolStripButton5,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(784, 46);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::NeoTabControlClient.Properties.Resources._077_AddFile_24x24_72;
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(133, 43);
            this.toolStripButton1.Text = "Show Add-in Manager";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::NeoTabControlClient.Properties.Resources._037_Colorize_24x24_72;
            this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(144, 43);
            this.toolStripButton2.Text = "Load a Custom Renderer";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Image = global::NeoTabControlClient.Properties.Resources.settings;
            this.toolStripButton4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(81, 43);
            this.toolStripButton4.Text = "Tab Manager";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabPageItemEffectToolStripMenuItem,
            this.pageEffectToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::NeoTabControlClient.Properties.Resources.PreviousPageHL;
            this.toolStripDropDownButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(143, 43);
            this.toolStripDropDownButton1.Text = "Drag && Drop: Enabled";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripDropDownButton1.ButtonClick += new System.EventHandler(this.toolStripDropDownButton1_ButtonClick);
            // 
            // tabPageItemEffectToolStripMenuItem
            // 
            this.tabPageItemEffectToolStripMenuItem.Checked = true;
            this.tabPageItemEffectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tabPageItemEffectToolStripMenuItem.Name = "tabPageItemEffectToolStripMenuItem";
            this.tabPageItemEffectToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.tabPageItemEffectToolStripMenuItem.Text = "TabPageItemEffect";
            this.tabPageItemEffectToolStripMenuItem.Click += new System.EventHandler(this.tabPageItemEffectToolStripMenuItem_Click);
            // 
            // pageEffectToolStripMenuItem
            // 
            this.pageEffectToolStripMenuItem.Name = "pageEffectToolStripMenuItem";
            this.pageEffectToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.pageEffectToolStripMenuItem.Text = "PageEffect";
            this.pageEffectToolStripMenuItem.Click += new System.EventHandler(this.pageEffectToolStripMenuItem_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Image = global::NeoTabControlClient.Properties.Resources.Paperclip;
            this.toolStripButton5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(108, 43);
            this.toolStripButton5.Text = "Get Control Clone";
            this.toolStripButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = global::NeoTabControlClient.Properties.Resources.red0001;
            this.toolStripButton3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(73, 43);
            this.toolStripButton3.Text = "Tooltip: ON";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // neoTabWindow1
            // 
            this.neoTabWindow1.AllowDrop = true;
            this.neoTabWindow1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.neoTabWindow1.Controls.Add(this.neoTabPage1);
            this.neoTabWindow1.Controls.Add(this.neoTabPage2);
            this.neoTabWindow1.Controls.Add(this.neoTabPage3);
            this.neoTabWindow1.Controls.Add(this.neoTabPage4);
            this.neoTabWindow1.Controls.Add(this.neoTabPage5);
            this.neoTabWindow1.Location = new System.Drawing.Point(17, 50);
            this.neoTabWindow1.Name = "neoTabWindow1";
            this.neoTabWindow1.RendererName = "NeoTabStripRenderer";
            this.neoTabWindow1.SelectedIndex = 0;
            this.neoTabWindow1.Size = new System.Drawing.Size(750, 215);
            this.neoTabWindow1.TabIndex = 1;
            this.neoTabWindow1.TooltipRenderer.BarBackgroundColorEnd = System.Drawing.Color.Black;
            this.neoTabWindow1.TooltipRenderer.BarBackgroundColorStart = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.neoTabWindow1.TooltipRenderer.BarBorderColor = System.Drawing.Color.DarkGray;
            this.neoTabWindow1.TooltipRenderer.BarProgressColorEnd = System.Drawing.Color.Yellow;
            this.neoTabWindow1.TooltipRenderer.BarProgressColorStart = System.Drawing.Color.Gold;
            this.neoTabWindow1.TooltipRenderer.LightBackgroundColor = System.Drawing.Color.DimGray;
            this.neoTabWindow1.RendererUpdated += new System.EventHandler(this.neoTabWindow1_RendererUpdated);
            this.neoTabWindow1.RendererChanged += new System.EventHandler(this.neoTabWindow1_RendererChanged);
            this.neoTabWindow1.DropDownButtonClicked += new System.EventHandler<NeoTabControlLibrary.DropDownButtonClickedEventArgs>(this.neoTabWindow1_DropDownButtonClicked);
            // 
            // neoTabPage1
            // 
            this.neoTabPage1.BackColor = System.Drawing.Color.Transparent;
            this.neoTabPage1.Controls.Add(this.label1);
            this.neoTabPage1.Name = "neoTabPage1";
            this.neoTabPage1.Text = "Solution &Explorer";
            this.neoTabPage1.ToolTipText = "Solution Explorer provides you with an organized view of your projects and their " +
    "files as well as ready access to the commands that pertain to them.";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(742, 189);
            this.label1.TabIndex = 0;
            this.label1.Text = "Solution Explorer";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // neoTabPage2
            // 
            this.neoTabPage2.BackColor = System.Drawing.Color.Transparent;
            this.neoTabPage2.Controls.Add(this.webBrowser1);
            this.neoTabPage2.Controls.Add(this.label2);
            this.neoTabPage2.IsCloseable = false;
            this.neoTabPage2.Name = "neoTabPage2";
            this.neoTabPage2.Text = "Pro&perties";
            this.neoTabPage2.ToolTipText = "The Properties window displays different types of editing fields, depending on th" +
    "e needs of a particular property.";
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(23, 18);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(742, 189);
            this.webBrowser1.TabIndex = 2;
            this.webBrowser1.Url = new System.Uri("http://msdn.microsoft.com", System.UriKind.Absolute);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(742, 189);
            this.label2.TabIndex = 1;
            this.label2.Text = "Properties";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // neoTabPage3
            // 
            this.neoTabPage3.BackColor = System.Drawing.Color.Transparent;
            this.neoTabPage3.Controls.Add(this.webBrowser2);
            this.neoTabPage3.Controls.Add(this.label3);
            this.neoTabPage3.Name = "neoTabPage3";
            this.neoTabPage3.Text = "&Toolbox";
            this.neoTabPage3.ToolTipText = "The Toolbox displays icons for items that you can add to projects.";
            // 
            // webBrowser2
            // 
            this.webBrowser2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser2.Location = new System.Drawing.Point(0, 0);
            this.webBrowser2.MinimumSize = new System.Drawing.Size(23, 18);
            this.webBrowser2.Name = "webBrowser2";
            this.webBrowser2.Size = new System.Drawing.Size(742, 189);
            this.webBrowser2.TabIndex = 2;
            this.webBrowser2.Url = new System.Uri("http://www.facebook.com", System.UriKind.Absolute);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(742, 189);
            this.label3.TabIndex = 1;
            this.label3.Text = "Toolbox";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // neoTabPage4
            // 
            this.neoTabPage4.BackColor = System.Drawing.Color.Transparent;
            this.neoTabPage4.Controls.Add(this.label4);
            this.neoTabPage4.IsSelectable = false;
            this.neoTabPage4.Name = "neoTabPage4";
            this.neoTabPage4.Text = "Error &List";
            this.neoTabPage4.ToolTipText = "The Error List helps you speed application development.";
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(742, 189);
            this.label4.TabIndex = 1;
            this.label4.Text = "Error List";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // neoTabPage5
            // 
            this.neoTabPage5.BackColor = System.Drawing.Color.Transparent;
            this.neoTabPage5.Controls.Add(this.label5);
            this.neoTabPage5.Name = "neoTabPage5";
            this.neoTabPage5.Text = "&Output";
            this.neoTabPage5.ToolTipText = "This window can display status messages for various features in the integrated de" +
    "velopment environment (IDE).";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(742, 189);
            this.label5.TabIndex = 1;
            this.label5.Text = "Output";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 275);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.neoTabWindow1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(464, 188);
            this.Name = "Form1";
            this.Text = "NeoTabControl Test Page";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neoTabWindow1)).EndInit();
            this.neoTabWindow1.ResumeLayout(false);
            this.neoTabPage1.ResumeLayout(false);
            this.neoTabPage2.ResumeLayout(false);
            this.neoTabPage3.ResumeLayout(false);
            this.neoTabPage4.ResumeLayout(false);
            this.neoTabPage5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NeoTabControlLibrary.NeoTabWindow neoTabWindow1;
        private NeoTabControlLibrary.NeoTabPage neoTabPage1;
        private System.Windows.Forms.Label label1;
        private NeoTabControlLibrary.NeoTabPage neoTabPage2;
        private NeoTabControlLibrary.NeoTabPage neoTabPage3;
        private NeoTabControlLibrary.NeoTabPage neoTabPage4;
        private NeoTabControlLibrary.NeoTabPage neoTabPage5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.WebBrowser webBrowser2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSplitButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem tabPageItemEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pageEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
    }
}

