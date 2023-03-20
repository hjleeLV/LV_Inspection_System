using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LV_Inspection_System.GUI
{
    public partial class Frm_Model_Name : Form
    {
        public Frm_Model_Name()
        {
            InitializeComponent();
            m_SetLanguage = LVApp.Instance().m_Config.m_SetLanguage;
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    radioButton1.Text = "기본 모델 복제";
                    radioButton2.Text = "신규 기본 모델 생성";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    radioButton1.Text = "Clone";
                    radioButton2.Text = "New create";
                }
                else if (value == 2 && m_Language != value)
                {// 영어
                    radioButton1.Text = "克隆";
                    radioButton2.Text = "新创建";
                }
                m_Language = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        public String ModelName
        {
            get
            {
                return txtModelname.Text;
            }
        }

        public bool CreateMethod
        {
            get
            {
                bool flag = radioButton1.Checked;
                return flag;
            }
        }

        private void Frm_Model_Name_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (txtModelname.Text != "")
            {
                return;
            }
            txtModelname.Text = "";
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void Frm_Model_Name_Load(object sender, EventArgs e)
        {
            txtModelname.Text = LVApp.Instance().m_Config.m_model_num.ToString("00")+"_";
            m_SetLanguage = LVApp.Instance().m_Config.m_SetLanguage;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
            }
            else
            {
                radioButton2.Checked = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
            }
            else
            {
                radioButton1.Checked = true;
            }
        }

        private void txtModelname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                string sssnumber = txtModelname.Text;
                if (sssnumber.Length > 0)
                {
                    sssnumber.Remove(sssnumber.Length - 1);
                    txtModelname.Text = sssnumber;
                }
                return;
            }

            if ((txtModelname.Text + e.KeyChar).Length > 0)
            {
                if (!IsValidPath(txtModelname.Text + e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private bool IsValidPath(string path)
        {
            // Check if the rest of the path is valid
            string InvalidFileNameChars = new string(Path.GetInvalidPathChars());
            InvalidFileNameChars += @":/?*,'`~!@#$%^&*+=<>;{}\|" + "\"";
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(InvalidFileNameChars) + "]");
            if (containsABadCharacter.IsMatch(path))
                return false;
            if (path[path.Length - 1] == '.') return false;

            return true;
        }
    }
}
