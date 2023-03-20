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
    public partial class Frm_QuickMenu : Form
    {
        public Frm_QuickMenu()
        {
            InitializeComponent();
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    button2.Text = "검사설정";
                    button1.Text = "스펙설정";
                    button3.Text = "모델";
                    button4.Text = "로그";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    button2.Text = "ROI";
                    button1.Text = "SPEC";
                    button3.Text = "MODEL";
                    button4.Text = "LOG";
                }
                m_Language = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }
            //if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
            //{
            //    LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 6;
            //    return;
            //}
            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 2;
           // LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING.SelectedIndex = 0;
            //neoTabWindow_EQUIP_SETTING
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }
            //if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
            //{
            //    LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 6;
            //    return;
            //}
            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 1;
            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }
            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }
            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 4;
        }

        private void Frm_QuickMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (LVApp.Instance().m_mainform.Force_close)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    this.WindowState = FormWindowState.Minimized;
            //    e.Cancel = true;
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }

            //if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
            //{
            //    LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 6;
            //    return;
            //}
            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 3;
            LVApp.Instance().m_mainform.neoTabWindow_EQUIP_SETTING.SelectedIndex = 0;
        }
    }
}
