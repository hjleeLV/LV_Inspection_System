using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeoTabControlClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void neoTabWindow1_RendererUpdated(object sender, EventArgs e)
        {
            this.BackColor = neoTabWindow1.BackColor;
        }

        private void neoTabWindow1_RendererChanged(object sender, EventArgs e)
        {
            this.BackColor = neoTabWindow1.BackColor;
            string typeName = neoTabWindow1.Renderer.GetType().Name;
            if (typeName.StartsWith("CCleanerRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("AvastRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("NeoTabStripRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("MarginGrayRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("VS2008LikeRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("VS2010LikeRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("VS2012LikeRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.White;
            }
            else if (typeName.StartsWith("WebSliderRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.Transparent;
            }
            else if (typeName.Equals("TelerikRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.Transparent;
            }
            else if (typeName.Equals("DefaultRenderer"))
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = Color.Transparent;
            }
            else
            {
                foreach (NeoTabControlLibrary.NeoTabPage tp in neoTabWindow1.Controls)
                    tp.BackColor = neoTabWindow1.BackColor;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            neoTabWindow1.ShowAddInRendererManager();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            neoTabWindow1.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("MYNETRendererVS2");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (neoTabWindow1.IsTooltipEnabled)
            {
                toolStripButton3.Text = "Tooltip: OFF";
                toolStripButton3.Image = NeoTabControlClient.Properties.Resources.red0001;
            }
            else
            {
                toolStripButton3.Text = "Tooltip: ON";
                toolStripButton3.Image = NeoTabControlClient.Properties.Resources.green0001;
            }
            neoTabWindow1.IsTooltipEnabled = !neoTabWindow1.IsTooltipEnabled;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            neoTabWindow1.ShowTabManager();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            NeoTabControlLibrary.NeoTabPage[] items ={
                new NeoTabControlLibrary.NeoTabPage(){ Text = "Mario Andretti" },
                new NeoTabControlLibrary.NeoTabPage(){ Text = "Graham Hill" },
                new NeoTabControlLibrary.NeoTabPage(){ Text = "Emerson Fittipaldi" },
                new NeoTabControlLibrary.NeoTabPage(){ Text = "Michael Schumacher" }};
            NeoTabControlLibrary.NeoTabWindow tw = neoTabWindow1.Clone() as NeoTabControlLibrary.NeoTabWindow;
            tw.Controls.AddRange(items);
            tw.Dock = DockStyle.Fill;
            Form frm = new Form();
            frm.ShowIcon = false;
            frm.ShowInTaskbar = false;
            frm.Text = "Formula-1 Racers";
            frm.Controls.Add(tw);
            frm.Show();
        }

        private void toolStripDropDownButton1_ButtonClick(object sender, EventArgs e)
        {
            if (neoTabWindow1.AllowDrop)
            {
                toolStripDropDownButton1.Text = "Drag && Drop: Disabled";
            }
            else
            {
                toolStripDropDownButton1.Text = "Drag && Drop: Enabled";
            }
            neoTabWindow1.AllowDrop = !neoTabWindow1.AllowDrop;
        }

        private void pageEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isChecked = pageEffectToolStripMenuItem.Checked;
            if (!isChecked)
            {
                neoTabWindow1.DraggingStyle = NeoTabControlLibrary.NeoTabWindow.DragDropStyle.PageEffect;
                pageEffectToolStripMenuItem.Checked = true;
                tabPageItemEffectToolStripMenuItem.Checked = false;
            }
        }

        private void tabPageItemEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isChecked = tabPageItemEffectToolStripMenuItem.Checked;
            if (!isChecked)
            {
                neoTabWindow1.DraggingStyle = NeoTabControlLibrary.NeoTabWindow.DragDropStyle.TabPageItemEffect;
                tabPageItemEffectToolStripMenuItem.Checked = true;
                pageEffectToolStripMenuItem.Checked = false;
            }
        }

        private void neoTabWindow1_DropDownButtonClicked(object sender, NeoTabControlLibrary.DropDownButtonClickedEventArgs e)
        {
            for (int i = 0; i < e.ContextMenu.Items.Count; i++)
            {
                ToolStripItem item = e.ContextMenu.Items[i];
                switch (i)
                {
                    case 0:
                        item.Image = NeoTabControlClient.Properties.Resources.Close;
                        break;
                    case 1:
                        item.Image = NeoTabControlClient.Properties.Resources.settings_16;
                        break;
                    case 2:
                        break;
                    default:
                        if (item.Enabled)
                            item.Image = NeoTabControlClient.Properties.Resources.InsertTabControlHS;
                        else
                            item.Image = NeoTabControlClient.Properties.Resources.Locked;
                        break;
                }
            }
        }
    }
}