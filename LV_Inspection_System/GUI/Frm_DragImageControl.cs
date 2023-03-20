using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LV_Inspection_System.GUI
{
    public partial class Frm_DragImageControl : Form
    {
        public int t_Cam_Num = 0;
        public string[] t_Image_List;
        public Frm_DragImageControl()
        {
            InitializeComponent();
        }

        public void Update_Image_List()
        {
            int t_cnt = t_Image_List.Length;
            if (t_cnt <= 0)
            {
                return;
            }
            listBox_Images.Items.Clear();
            for (int i = 0; i < t_cnt; i++)
            {
                listBox_Images.Items.Add(t_Image_List[i]);
            }
            int t_idx = listBox_Images.SelectedIndex;
            if (t_idx < 0)
            {
                listBox_Images.SelectedIndex = 0;
            }
            if (!timer_Close.Enabled)
            {
                timer_Close.Start();
            }
        }

        bool t_Inspection_check = false;
        private void listBox_Images_SelectedIndexChanged(object sender, EventArgs e)
        {
            int t_idx = listBox_Images.SelectedIndex;
            if (t_idx < 0)
            {
                return;
            }
            string t_name = listBox_Images.Items[t_idx].ToString();
            richTextBox_ImageName.Text = t_name;
            t_Inspection_check = true;
            LVApp.Instance().m_mainform.Drag_Image_Inspection(t_name, sender, e);
            t_Inspection_check = false;
        }

        private void pictureBox_Pre_Click(object sender, EventArgs e)
        {
            int t_idx = listBox_Images.SelectedIndex;
            t_idx--;
            if (t_idx < 0)
            {
                return;
            }
            listBox_Images.SelectedIndex = t_idx;
        }

        private void pictureBox_Stop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox_Next_Click(object sender, EventArgs e)
        {
            int t_idx = listBox_Images.SelectedIndex;
            t_idx++;
            if (t_idx < 0)
            {
                return;
            }
            if (t_idx >= listBox_Images.Items.Count)
            {
                t_idx = listBox_Images.Items.Count - 1;
            }
            listBox_Images.SelectedIndex = t_idx;

            if (t_idx == listBox_Images.Items.Count - 1 && timer_Auto.Enabled)
            {
                timer_Auto.Stop();
                timer_Auto.Enabled = false;
                checkBox_AUTO.Checked = false;
            }
        }

        private void timer_Close_Tick(object sender, EventArgs e)
        {
            if (t_Cam_Num != LVApp.Instance().m_mainform.ctr_Manual1.m_Selected_Cam_Num || LVApp.Instance().m_Config.m_Check_Inspection_Mode || LVApp.Instance().m_Config.neoTabWindow_MAIN_idx != 2)
            {
                this.Close();
            }
        }

        private void timer_Auto_Tick(object sender, EventArgs e)
        {
            if (!t_Inspection_check)
            {
                pictureBox_Next_Click(sender, e);
            }
        }

        private void checkBox_AUTO_CheckedChanged(object sender, EventArgs e)
        {
            if (!timer_Auto.Enabled && checkBox_AUTO.Checked)
            {
                timer_Auto.Enabled = true;
                timer_Auto.Start();
            }
            else
            {
                timer_Auto.Stop();
                timer_Auto.Enabled = false;
            }
        }
    }
}
