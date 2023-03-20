using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace LV_Inspection_System.GUI
{
    public partial class Frm_Trackbar : Form
    {
        public int t_idx = 0;
        private bool t_check = false;
        public double t_Precision = 1.0f;
        private int m_total = 0;

        public Frm_Trackbar()
        {
            InitializeComponent();
        }

        private void colorSlider_Scroll(object sender, ScrollEventArgs e)
        {
            if (t_check)
            {
                return;
            }
            Thread.Sleep(30);
            t_check = true;
            if (LVApp.Instance().m_Config.ROI_Cam_Num == 0)
            {
                //if (LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[t_idx].Cells[0].Value.ToString().Contains("두께(mm)") || LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[t_idx].Cells[0].Value.ToString().Contains("Thickness of"))
                //{
                //    float t_v0 = float.Parse(LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[t_idx - 1].Cells[1].Value.ToString());
                //    float t_v1 = t_Precision != 1 ? float.Parse((t_Precision * (double)colorSlider.Value).ToString("0.00")) : colorSlider.Value;
                //    if (t_v1 > t_v0)
                //    {
                //        colorSlider.Value = t_v0 / (float)t_Precision;
                //        colorSlider.Refresh();
                //        return;
                //    }
                //}

                LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[t_idx][1] = t_Precision != 1 ? double.Parse((t_Precision * (double)colorSlider.Value).ToString("0.00")) : colorSlider.Value;
                LVApp.Instance().m_mainform.ctr_ROI1.SubDB_to_MainDB();
                LVApp.Instance().m_Config.Set_Parameters();
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
                
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.ctr_ROI1.button_INSPECTION_Click(sender, e);
                }
            }
            else if (LVApp.Instance().m_Config.ROI_Cam_Num == 1)
            {
                LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[t_idx][1] = t_Precision != 1 ? double.Parse((t_Precision * (double)colorSlider.Value).ToString("0.00")) : colorSlider.Value;
                LVApp.Instance().m_mainform.ctr_ROI2.SubDB_to_MainDB();
                LVApp.Instance().m_Config.Set_Parameters();
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
                //if (t_idx >= 1 && t_idx <= 4)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI2.pictureBox_Image.Refresh();
                //}
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.ctr_ROI2.button_INSPECTION_Click(sender, e);
                }
            }
            else if (LVApp.Instance().m_Config.ROI_Cam_Num == 2)
            {
                LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[t_idx][1] = t_Precision != 1 ? double.Parse((t_Precision * (double)colorSlider.Value).ToString("0.00")) : colorSlider.Value;
                LVApp.Instance().m_mainform.ctr_ROI3.SubDB_to_MainDB();
                LVApp.Instance().m_Config.Set_Parameters();
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.ctr_ROI3.button_INSPECTION_Click(sender, e);
                }
            }
            else if (LVApp.Instance().m_Config.ROI_Cam_Num == 3)
            {
                LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[t_idx][1] = t_Precision != 1 ? double.Parse((t_Precision * (double)colorSlider.Value).ToString("0.00")) : colorSlider.Value;
                LVApp.Instance().m_mainform.ctr_ROI4.SubDB_to_MainDB();
                LVApp.Instance().m_Config.Set_Parameters();
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.ctr_ROI4.button_INSPECTION_Click(sender, e);
                }
            }
            t_check = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_total >= 10) // check for closing
            {
                timer1.Stop();
                Close();
                return;
            }
            m_total++;
        }
    }
    class FloatTrackBar : TrackBar
    {
        private float precision = 0.01f;

        public float Precision
        {
            get { return precision; }
            set
            {
                precision = value;
                // todo: update the 5 properties below
            }
        }
        public new float LargeChange
        { get { return base.LargeChange * precision; } set { base.LargeChange = (int)(value / precision); } }
        public new float Maximum
        { get { return base.Maximum * precision; } set { base.Maximum = (int)(value / precision); } }
        public new float Minimum
        { get { return base.Minimum * precision; } set { base.Minimum = (int)(value / precision); } }
        public new float SmallChange
        { get { return base.SmallChange * precision; } set { base.SmallChange = (int)(value / precision); } }
        public new float Value
        { get { return base.Value * precision; } set { base.Value = (int)(value / precision); } }
    }
}
