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
    public partial class Frm_Textbox : Form
    {
        public int m_row = 0;
        public int m_col = 0;
        public int m_Cam_Num = 0;

        public Frm_Textbox()
        {
            InitializeComponent();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (textBox1.Text == "")
                    {
                        this.Close();
                        return;
                    }
                    double t_v = 0;
                    if (double.TryParse(textBox1.Text, out t_v))
                    {
                        if (m_Cam_Num == 0)
                        {
                            CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DataSource];
                            currencyManager0.SuspendBinding();
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[m_row][m_col] = t_v;
                            currencyManager0.ResumeBinding();
                        }
                        else if (m_Cam_Num == 1)
                        {
                            CurrencyManager currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DataSource];
                            currencyManager1.SuspendBinding();
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[m_row][m_col] = t_v;
                            currencyManager1.ResumeBinding();
                        }
                        else if (m_Cam_Num == 2)
                        {
                            CurrencyManager currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DataSource];
                            currencyManager2.SuspendBinding();
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[m_row][m_col] = t_v;
                            currencyManager2.ResumeBinding();
                        }
                        else if (m_Cam_Num == 3)
                        {
                            CurrencyManager currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DataSource];
                            currencyManager3.SuspendBinding();
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[m_row][m_col] = t_v;
                            currencyManager3.ResumeBinding();
                        }
                    }
                    this.Close();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    return;
                }
            }
            catch
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("숫자를 입력하세요!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("You have to type a number!");
                }
            }
        }
    }
}
