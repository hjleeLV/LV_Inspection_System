using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Formats;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
//using AForge.Imaging.IPPrototyper;
using OfficeOpenXml;
using PopupControl;
using System.Diagnostics;
using OpenCvSharp.Flann;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_ROI : UserControl
    {
        //public int Cam_Num = 0;
        public int Cam_Num = 0;
        public int ROI_Idx = 0;
        private Rectangle rc;
        //Popup popup;
        Thread ROI_thread = null;
        public int m_Job_Mode = 0;
        private bool m_range_visible_check = false;
        private bool m_roi_mode = true;
        private bool m_roi_click_auto_mode = false;
        public int m_Camera_Interval = 500;
        private Stopwatch Cam_SW = new Stopwatch();

        public bool m_advenced_param_visible = false;

        public Ctr_ROI()
        {
            InitializeComponent();
            Cam_SW.Start();
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    int t_s = comboBox_TABLETYPE.SelectedIndex;
                    if (t_s < 0)
                    {
                        t_s = 0;
                    }
                    comboBox_TABLETYPE.Items.Clear();
                    comboBox_TABLETYPE.Items.Add("#1 인덱스 타입");
                    comboBox_TABLETYPE.Items.Add("#2 유리판 타입");
                    comboBox_TABLETYPE.Items.Add("#3 라인스캔 벨트 타입");
                    comboBox_TABLETYPE.Items.Add("#4 가이드 없음");
                    comboBox_TABLETYPE.Items.Add("#5 ROI 기준 측정");
                    comboBox_TABLETYPE.Items.Add("#6 고객사 전용 타입");

                    comboBox_TABLETYPE.SelectedIndex = t_s;

                    t_s = comboBox_CAMPOSITION.SelectedIndex;
                    if (t_s < 0)
                    {
                        t_s = 0;
                    }
                    comboBox_CAMPOSITION.Items.Clear();
                    comboBox_CAMPOSITION.Items.Add("#1 상부/하부");
                    comboBox_CAMPOSITION.Items.Add("#2 사이드");
                    comboBox_CAMPOSITION.SelectedIndex = t_s;

                    listBox1.Items[0] = "제품 검사 영역 설정";
                    for (int i = 1; i < listBox1.Items.Count; i++)
                    {

                        //if (listBox1.Items[i].ToString().Contains("Measurement Area Setting") ||
                        //    listBox1.Items[i].ToString().Contains("Measurement Setting"))
                        //{
                        listBox1.Items[i] = "ROI#" + i.ToString("00") + " 측정 영역 설정";
                        //}
                    }

                    radioButton1.Text = "검사 영역 설정 모드";
                    radioButton2.Text = "알고리즘 시험 모드";
                    button_OPEN.Text = "이미지 열기";
                    button_INSPECTION.Text = "검사 결과";
                    button_SNAPSHOT.Text = "카메라 촬영";
                    button_LOAD.Text = "설정 불러오기";
                    button_SAVE.Text = "설정 저장";
                    button_ROTATION_CAL.Text = "보정";
                    checkBox_AutoInspection.Text = "ROI Click 자동시험";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    int t_s = comboBox_TABLETYPE.SelectedIndex;
                    if (t_s < 0)
                    {
                        t_s = 0;
                    }
                    comboBox_TABLETYPE.Items.Clear();
                    comboBox_TABLETYPE.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE.Items.Add("#6 Special Customer Type");
                    comboBox_TABLETYPE.SelectedIndex = t_s;

                    t_s = comboBox_CAMPOSITION.SelectedIndex;
                    if (t_s < 0)
                    {
                        t_s = 0;
                    }
                    comboBox_CAMPOSITION.Items.Clear();
                    comboBox_CAMPOSITION.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION.Items.Add("#2 Side");
                    comboBox_CAMPOSITION.SelectedIndex = t_s;

                    listBox1.Items[0] = "Inspection Area Setting";
                    for (int i = 1; i < listBox1.Items.Count; i++)
                    {

                        //if (listBox1.Items[i].ToString().Contains("측정 영역 설정")
                        //    || listBox1.Items[i].ToString().Contains("측정값"))
                        //{
                        listBox1.Items[i] = "ROI#" + i.ToString("00") + " Measurement Area Setting";
                        //}
                    }

                    radioButton1.Text = "ROI Setting Mode";
                    radioButton2.Text = "Alg. Test Mode";
                    button_OPEN.Text = "Image Open";
                    button_INSPECTION.Text = "Alg. Test";
                    button_SNAPSHOT.Text = "Snapshot";
                    button_LOAD.Text = "Load";
                    button_SAVE.Text = "Save";
                    button_ROTATION_CAL.Text = "Cal.";
                    checkBox_AutoInspection.Text = "ROI Click Auto Test";
                }
                else if (value == 2 && m_Language != value)
                {// 중국어
                    int t_s = comboBox_TABLETYPE.SelectedIndex;
                    if (t_s < 0)
                    {
                        t_s = 0;
                    }
                    comboBox_TABLETYPE.Items.Clear();
                    comboBox_TABLETYPE.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE.Items.Add("#6 Special Customer Type");
                    comboBox_TABLETYPE.SelectedIndex = t_s;

                    t_s = comboBox_CAMPOSITION.SelectedIndex;
                    if (t_s < 0)
                    {
                        t_s = 0;
                    }
                    comboBox_CAMPOSITION.Items.Clear();
                    comboBox_CAMPOSITION.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION.Items.Add("#2 Side");
                    comboBox_CAMPOSITION.SelectedIndex = t_s;

                    listBox1.Items[0] = "检查区域设置";
                    for (int i = 1; i < listBox1.Items.Count; i++)
                    {

                        //if (listBox1.Items[i].ToString().Contains("측정 영역 설정")
                        //    || listBox1.Items[i].ToString().Contains("측정값"))
                        //{
                        listBox1.Items[i] = "ROI#" + i.ToString("00") + " 测量区域设置";
                        //}
                    }

                    radioButton1.Text = "ROI 设置模式";
                    radioButton2.Text = "Alg. 测试模式";
                    button_OPEN.Text = "图像打开";
                    button_INSPECTION.Text = "Alg. 测试";
                    button_SNAPSHOT.Text = "快照";
                    button_LOAD.Text = "负荷";
                    button_SAVE.Text = "救";
                    button_ROTATION_CAL.Text = "校准";
                    checkBox_AutoInspection.Text = "ROI 自动测试";
                }
                //Initialize_ROI();

                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM" + Cam_Num.ToString("0") + " Insp. Area Setting";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM" + Cam_Num.ToString("0") + " Insp. Area Setting";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    listBox1.Items[0] = "CAM" + Cam_Num.ToString("0") + " 检查区域设置";
                }

                dataGridView1.DataSource = null;
                if (LVApp.Instance().m_Config.ds_DATA_0 == null)
                {
                    return;
                }
                if (Cam_Num == 0 && LVApp.Instance().m_Config.ds_DATA_0.Tables.Count > 3)
                {
                    dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_0.Tables[3];
                }
                else if (Cam_Num == 1 && LVApp.Instance().m_Config.ds_DATA_1.Tables.Count > 3)
                {
                    dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_1.Tables[3];
                }
                else if (Cam_Num == 2 && LVApp.Instance().m_Config.ds_DATA_2.Tables.Count > 3)
                {
                    dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_2.Tables[3];
                }
                else if (Cam_Num == 3 && LVApp.Instance().m_Config.ds_DATA_3.Tables.Count > 3)
                {
                    dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_3.Tables[3];
                }
                MainDB_to_SubDB();
                dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //listBox1.SelectedIndex = 0;

                dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.Gainsboro;
                dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                dataGridView1.Rows[1].Cells[1].Style.BackColor = Color.LightGreen;
                dataGridView1.Rows[2].Cells[1].Style.BackColor = Color.LightGreen;
                dataGridView1.Rows[3].Cells[1].Style.BackColor = Color.LightGreen;
                dataGridView1.Rows[4].Cells[1].Style.BackColor = Color.LightGreen;


                //CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                //currencyManager1.SuspendBinding();
                //dataGridView1.ClearSelection();
                //dataGridView1.Rows[5].Selected = true; 
                //dataGridView1.Rows[0].Visible = false;
                //currencyManager1.ResumeBinding(); 
                dataGridView1.ClearSelection();
                dataGridView1.Rows[5].Selected = true;
                dataGridView1.Rows[0].Height = 0;
                dataGridView1.Rows[1].Visible = false;
                dataGridView1.Rows[2].Visible = false;
                dataGridView1.Rows[3].Visible = false;
                dataGridView1.Rows[4].Visible = false;

                for (int i = 12; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "备用变量")
                    {
                        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                        dataGridView1.Rows[i].Cells[1].Value = 0;
                    }
                }
                m_Language = value;
            }
        }

        public void Initialize_ROI()
        {
            listBox1.Top = 6;
            listBox1.Height = 148;

            //LVApp.Instance().m_Config.AE_rect[Cam_Num] = new UserRect(new Rectangle(pictureBox_Image.Width - 100, pictureBox_Image.Height - 100, 50, 50));
            //LVApp.Instance().m_Config.AE_rect[Cam_Num].SetPictureBox(pictureBox_Image);
            //LVApp.Instance().m_Config.AE_rect[Cam_Num].m_ROI_Name = "AE";
            //LVApp.Instance().m_Config.AE_rect[Cam_Num].mView = true;
            //LVApp.Instance().m_Config.AE_rect[Cam_Num].mUse = true;
            dataGridView1.DataSource = null;
            if (Cam_Num == 0)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < LVApp.Instance().m_Config.Cam0_rect.Length; i++)
                {
                    LVApp.Instance().m_Config.Cam0_rect[i] = new UserRect(rc);
                    LVApp.Instance().m_Config.Cam0_rect[i].SetPictureBox(pictureBox_Image);
                    LVApp.Instance().m_Config.Cam0_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    LVApp.Instance().m_Config.Cam0_rect[i].mView = true;
                    LVApp.Instance().m_Config.Cam0_rect[i].mUse = true;
                    LVApp.Instance().m_Config.Cam0_rect[i].t_idx = i;
                    LVApp.Instance().m_Config.Cam0_rect[i].m_Cam_Num = 0;
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM0 검사영역 셋팅";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM0 Insp. Area Setting";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    listBox1.Items[0] = "CAM0 检查区域设置";
                }

                dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_0.Tables[3];
            }
            else if (Cam_Num == 1)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < LVApp.Instance().m_Config.Cam1_rect.Length; i++)
                {
                    LVApp.Instance().m_Config.Cam1_rect[i] = new UserRect(rc);
                    LVApp.Instance().m_Config.Cam1_rect[i].SetPictureBox(pictureBox_Image);
                    LVApp.Instance().m_Config.Cam1_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    LVApp.Instance().m_Config.Cam1_rect[i].mView = true;
                    LVApp.Instance().m_Config.Cam1_rect[i].mUse = true;
                    LVApp.Instance().m_Config.Cam1_rect[i].t_idx = i;
                    LVApp.Instance().m_Config.Cam1_rect[i].m_Cam_Num = 1;
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM1 검사영역 셋팅";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM1 Insp. Area Setting";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    listBox1.Items[0] = "CAM1 检查区域设置";
                }
                dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_1.Tables[3];
            }
            else if (Cam_Num == 2)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < LVApp.Instance().m_Config.Cam2_rect.Length; i++)
                {
                    LVApp.Instance().m_Config.Cam2_rect[i] = new UserRect(rc);
                    LVApp.Instance().m_Config.Cam2_rect[i].SetPictureBox(pictureBox_Image);
                    LVApp.Instance().m_Config.Cam2_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    LVApp.Instance().m_Config.Cam2_rect[i].mView = true;
                    LVApp.Instance().m_Config.Cam2_rect[i].mUse = true;
                    LVApp.Instance().m_Config.Cam2_rect[i].t_idx = i;
                    LVApp.Instance().m_Config.Cam2_rect[i].m_Cam_Num = 2;
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM2 검사영역 셋팅";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM2 Insp. Area Setting";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    listBox1.Items[0] = "CAM2 检查区域设置";
                }
                dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_2.Tables[3];
            }
            else if (Cam_Num == 3)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < LVApp.Instance().m_Config.Cam3_rect.Length; i++)
                {
                    LVApp.Instance().m_Config.Cam3_rect[i] = new UserRect(rc);
                    LVApp.Instance().m_Config.Cam3_rect[i].SetPictureBox(pictureBox_Image);
                    LVApp.Instance().m_Config.Cam3_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    LVApp.Instance().m_Config.Cam3_rect[i].mView = true;
                    LVApp.Instance().m_Config.Cam3_rect[i].mUse = true;
                    LVApp.Instance().m_Config.Cam3_rect[i].t_idx = i;
                    LVApp.Instance().m_Config.Cam3_rect[i].m_Cam_Num = 3;
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM3 검사영역 셋팅";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM3 Insp. Area Setting";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    listBox1.Items[0] = "CAM3 检查区域设置";
                }
                dataGridView1.DataSource = LVApp.Instance().m_Config.ds_DATA_3.Tables[3];
            }
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            listBox1.SelectedIndex = 0;

            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.Gainsboro;
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            dataGridView1.Rows[1].Cells[1].Style.BackColor = Color.LightGreen;
            dataGridView1.Rows[2].Cells[1].Style.BackColor = Color.LightGreen;
            dataGridView1.Rows[3].Cells[1].Style.BackColor = Color.LightGreen;
            dataGridView1.Rows[4].Cells[1].Style.BackColor = Color.LightGreen;

            //CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            //currencyManager1.SuspendBinding();
            //dataGridView1.ClearSelection();
            //dataGridView1.Rows[5].Selected = true; 
            //dataGridView1.Rows[0].Visible = false;
            //currencyManager1.ResumeBinding();

            dataGridView1.ClearSelection();
            dataGridView1.Rows[5].Selected = true;
            dataGridView1.Rows[0].Height = 0;
            dataGridView1.Rows[1].Visible = false;
            dataGridView1.Rows[2].Visible = false;
            dataGridView1.Rows[3].Visible = false;
            dataGridView1.Rows[4].Visible = false;

            dataGridView1.Rows[10].Visible = false;
            dataGridView1.Rows[11].Visible = false;

            for (int i = 12; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "备用变量" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                    dataGridView1.Rows[i].Cells[1].Value = 0;
                }
            }
            dataGridView1.Refresh();
            Referesh_Select_Menu(false);
            MainDB_to_SubDB();
            Referesh_Select_Menu(true);
            button_LOAD_Click(null, null);
        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int t_idx = listBox1.SelectedIndex;

                bool t_alg_change_check = false;
                if (t_idx < 0)
                {
                    return;
                }
                if (e.ColumnIndex == 1 && e.RowIndex == 0)
                {
                    if (t_idx <= 1 && !listBox1.GetItemChecked(t_idx))
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                        listBox1.SetItemCheckState(t_idx, CheckState.Checked);
                    }

                    if (Cam_Num == 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = true;
                            listBox1.SetItemCheckState(t_idx, CheckState.Checked);
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = false;
                            listBox1.SetItemCheckState(t_idx, CheckState.Unchecked);
                        }
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].mView = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_Config.Cam0_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            LVApp.Instance().m_mainform.dataGridView_Setting_0.ClearSelection();
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                            //LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[0].Selected = true;
                            //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[0].Selected = true;
                            LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        }
                    }
                    else if (Cam_Num == 1)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = true;
                            listBox1.SetItemCheckState(t_idx, CheckState.Checked);
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = false;
                            listBox1.SetItemCheckState(t_idx, CheckState.Unchecked);
                        }
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].mView = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_Config.Cam1_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            LVApp.Instance().m_mainform.dataGridView_Setting_1.ClearSelection();
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ClearSelection();
                            //LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[0].Selected = true;
                            //LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[0].Selected = true;
                            LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        }
                    }
                    else if (Cam_Num == 2)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = true;
                            listBox1.SetItemCheckState(t_idx, CheckState.Checked);
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = false;
                            listBox1.SetItemCheckState(t_idx, CheckState.Unchecked);
                        }
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].mView = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_Config.Cam2_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            LVApp.Instance().m_mainform.dataGridView_Setting_2.ClearSelection();
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ClearSelection();
                            //LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[0].Selected = true;
                            //LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[0].Selected = true;
                            LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        }
                    }
                    else if (Cam_Num == 3)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = true;
                            listBox1.SetItemCheckState(t_idx, CheckState.Checked);
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = false;
                            listBox1.SetItemCheckState(t_idx, CheckState.Unchecked);
                        }
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].mView = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_Config.Cam3_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            LVApp.Instance().m_mainform.dataGridView_Setting_3.ClearSelection();
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ClearSelection();
                            //LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[0].Selected = true;
                            //LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[0].Selected = true;
                            LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 1)
                {
                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 2)
                {
                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 3)
                {
                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 4)
                {
                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 8)
                {
                    if (t_before_alg[Cam_Num] != dataGridView1.Rows[8].Cells[1].Value.ToString())
                    {
                        t_alg_change_check = true;
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 5)
                {
                    if (t_before_alg0[Cam_Num] != dataGridView1.Rows[5].Cells[1].Value.ToString())
                    {
                        t_alg_change_check = true;
                    }
                }
                //else if (e.ColumnIndex == 1 && e.RowIndex == 6)
                //{
                //    int t_Vx = Convert.ToInt32(dataGridView1.Rows[6].Cells[1].Value);
                //    if (t_Vx != ctr_ROI_Guide1.trackBar_V1.Value)
                //    {
                //        ctr_ROI_Guide1.trackBar_V1.Value = t_Vx;
                //        ctr_ROI_Guide1.textBox_V1.Text = t_Vx.ToString();
                //    }
                //}
                //else if (e.ColumnIndex == 1 && e.RowIndex == 7)
                //{
                //    int t_Vx = Convert.ToInt32(dataGridView1.Rows[7].Cells[1].Value);
                //    if (t_Vx != ctr_ROI_Guide1.trackBar_V2.Value)
                //    {
                //        ctr_ROI_Guide1.trackBar_V2.Value = t_Vx;
                //        ctr_ROI_Guide1.textBox_V2.Text = t_Vx.ToString();
                //    }
                //}
                string tstr = dataGridView1.Rows[5].Cells[1].Value.ToString();
                if (tstr.Contains("모델 사용") || tstr.Contains("Model find"))
                {
                    SubDB_to_MainDB();
                }

                //if (tstr == "v1 이하" || tstr == "v1 less than")
                //{
                //    dataGridView1.Rows[6].Visible = true;
                //    dataGridView1.Rows[7].Visible = false;
                //}
                //else if (tstr == "v2 이상" || tstr == "v2 more than")
                //{
                //    dataGridView1.Rows[6].Visible = false;
                //    dataGridView1.Rows[7].Visible = true;
                //}
                //else if (tstr == "v1~v2 사이" || tstr == "v1~v2")
                //{
                //    dataGridView1.Rows[6].Visible = true;
                //    dataGridView1.Rows[7].Visible = true;
                //}
                //else if (tstr == "v1이하v2이상" || tstr == "less v1 more v2")
                //{
                //    dataGridView1.Rows[6].Visible = true;
                //    dataGridView1.Rows[7].Visible = true;
                //}
                //else if (tstr == "자동이하" || tstr == "Auto less than")
                //{
                //    dataGridView1.Rows[6].Visible = false;
                //    dataGridView1.Rows[7].Visible = false;
                //}
                //else if (tstr == "자동이상" || tstr == "Auto more than")
                //{
                //    dataGridView1.Rows[6].Visible = false;
                //    dataGridView1.Rows[7].Visible = false;
                //}
                //else if (tstr == "에지" || tstr == "Edge")
                //{
                //    dataGridView1.Rows[6].Visible = true;
                //    dataGridView1.Rows[7].Visible = true;
                //}
                //else if (tstr == "검사 영역 결과 사용" || tstr == "모델 사용" || tstr == "Insp. area result use" || tstr == "Model find")
                //{
                //    dataGridView1.Rows[6].Visible = false;
                //    dataGridView1.Rows[7].Visible = false;
                //}
                //else if (tstr == "평균기준 차이" || tstr == "Diff. from AVG")
                //{
                //    dataGridView1.Rows[6].Visible = true;
                //    dataGridView1.Rows[7].Visible = true;
                //}
                //else if (tstr == "비교v1이하v2이상" || tstr == "Compare less v1 more v2")
                //{
                //    dataGridView1.Rows[6].Visible = true;
                //    dataGridView1.Rows[7].Visible = true;
                //}

                if (!load_check)
                {
                    SubDB_to_MainDB();
                    Referesh_Select_Menu(true);
                }
                else
                {
                    load_check = false;
                }
                if (t_alg_change_check)
                {
                    Change_Initial_Parameter(sender, e);
                    return;
                }

                LVApp.Instance().m_Config.Set_Parameters();
                //if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                //{
                //    Thread.Sleep(30);
                //    Thread_INSPECTION();
                //    //LVApp.Instance().m_mainform.ctr_ROI1.button_INSPECTION_Click(sender, e);
                //}
                pictureBox_Image.Refresh();
            }
            catch
            {
            }
        }


        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (!listBox1.GetItemChecked(0))
            //{
            //    listBox1.SetItemChecked(0, true);
            //}
            //if (!listBox1.GetItemChecked(1))
            //{
            //    listBox1.SetItemChecked(1, true);
            //}
            try
            {

                //if (load_check)
                //{
                //    load_check = false;
                //    return;
                //}
                listBox1_SelectedIndex = listBox1.SelectedIndex;
                dataGridView1.ClearSelection();

                if (listBox1_SelectedIndex < 0)
                {
                    listBox1_SelectedIndex = 0;
                    return;
                }
                if (pictureBox_Image.Image == null && LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] != 3)
                {
                    listBox1.SelectedIndex = 0;
                    //listBox1_SelectedIndexChanged(sender, e);
                    return;
                }

                if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == listBox1_SelectedIndex)
                {
                    return;
                }

                int t_idx = listBox1_SelectedIndex;
                //radioButton1.Checked = true;
                //radioButton2.Checked = false;
                if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] != listBox1_SelectedIndex)
                {
                    //if ((LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0 && t_idx != 0)
                    //    || (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] != 0 && t_idx == 0))
                    LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1_SelectedIndex;

                    string main_str = "";
                    if (m_Language == 0)
                    {
                        if (Cam_Num == 0)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("모델 사용") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("검사 영역 결과 사용", "v1 이하");
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                        else if (Cam_Num == 1)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("모델 사용") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("검사 영역 결과 사용", "v1 이하");
                                LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                        else if (Cam_Num == 2)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("모델 사용") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("검사 영역 결과 사용", "v1 이하");
                                LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                        else if (Cam_Num == 3)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("모델 사용") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("검사 영역 결과 사용", "v1 이하");
                                LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                    }
                    else
                    {
                        if (Cam_Num == 0)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("Model find") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("Insp. area result use", "v1 less than");
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                        else if (Cam_Num == 1)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("Model find") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("Insp. area result use", "v1 less than");
                                LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                        else if (Cam_Num == 2)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("Model find") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("Insp. area result use", "v1 less than");
                                LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                        else if (Cam_Num == 3)
                        {
                            main_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();
                            if (main_str.Contains("Model find") && t_idx != 0)
                            {
                                string main_idx_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1].ToString();
                                main_idx_str = main_idx_str.Replace("Insp. area result use", "v1 less than");
                                LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1] = main_idx_str;
                            }
                        }
                    }
                    Referesh_Select_Menu(false);
                    //LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_idx;
                    MainDB_to_SubDB();
                    //MainDB_to_SubDB();
                    Referesh_Select_Menu(true);
                    //button_LOAD_Click(sender, e);
                }
                if (Cam_Num == 0 && t_idx - 1 >= 0)
                {
                    if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                else if (Cam_Num == 1 && t_idx - 1 >= 0)
                {
                    if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                else if (Cam_Num == 2 && t_idx - 1 >= 0)
                {
                    if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                else if (Cam_Num == 3 && t_idx - 1 >= 0)
                {
                    if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                //Referesh_Select_Menu(true);
                if (Cam_Num == 0)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = false;
                        }

                        CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_0.DataSource];
                        currencyManager0.SuspendBinding();
                        LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        currencyManager0.ResumeBinding();
                    }
                    //LVApp.Instance().m_Config.ROI_Selected_IDX[1] = LVApp.Instance().m_Config.ROI_Selected_IDX[2] = LVApp.Instance().m_Config.ROI_Selected_IDX[3] = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI2.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI3.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI4.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
                    //button_INSPECTION_Click(sender, e);
                }
                else if (Cam_Num == 1)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        CurrencyManager currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_1.DataSource];
                        currencyManager1.SuspendBinding();
                        LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        currencyManager1.ResumeBinding();
                    }
                    //LVApp.Instance().m_Config.ROI_Selected_IDX[0] = LVApp.Instance().m_Config.ROI_Selected_IDX[2] = LVApp.Instance().m_Config.ROI_Selected_IDX[3] = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI1.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI3.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI4.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
                    //button_INSPECTION_Click(sender, e);
                }
                else if (Cam_Num == 2)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        CurrencyManager currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_2.DataSource];
                        currencyManager2.SuspendBinding();
                        LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        currencyManager2.ResumeBinding();
                    }
                    //LVApp.Instance().m_Config.ROI_Selected_IDX[0] = LVApp.Instance().m_Config.ROI_Selected_IDX[1] = LVApp.Instance().m_Config.ROI_Selected_IDX[3] = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI1.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI2.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI4.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
                    //button_INSPECTION_Click(sender, e);
                }
                else if (Cam_Num == 3)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        CurrencyManager currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_3.DataSource];
                        currencyManager3.SuspendBinding();
                        LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[t_idx - 1].Visible = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        currencyManager3.ResumeBinding();
                    }
                    //LVApp.Instance().m_Config.ROI_Selected_IDX[0] = LVApp.Instance().m_Config.ROI_Selected_IDX[1] = LVApp.Instance().m_Config.ROI_Selected_IDX[2] = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI1.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI2.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
                    //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    //LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(false);
                    //LVApp.Instance().m_mainform.ctr_ROI3.MainDB_to_SubDB();
                    //LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
                    //button_INSPECTION_Click(sender, e);
                }

                if (Cam_Num == 0)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[0][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X")
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[0][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam0_rect[t_idx].mView = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].mView = true;
                    }
                }
                else if (Cam_Num == 1)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[0][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X")
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[0][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam1_rect[t_idx].mView = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].mView = true;
                    }
                }
                else if (Cam_Num == 2)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[0][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X")
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[0][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam2_rect[t_idx].mView = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].mView = true;
                    }
                }
                else if (Cam_Num == 3)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[0][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X")
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[0][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam3_rect[t_idx].mView = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].mView = true;
                    }
                }

                pictureBox_Image.Refresh();

                //for (int i = 0; i < Application.OpenForms.Count; i++)
                //{
                //    Form f = Application.OpenForms[i];
                //    if (f.GetType() == typeof(Frm_Trackbar))
                //    {
                //        f.Close();
                //    }
                //}

                t_idx = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (t_idx == 0)
                    {
                        row.Height = 0;
                    }
                    else
                    {
                        row.Height = (dataGridView1.Height - dataGridView1.ColumnHeadersHeight) / (dataGridView1.Rows.Count - 5);
                    }
                    t_idx++;
                    //row.Height = (dataGridView1.Height) / dataGridView1.Rows.Count;
                }
                CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                currencyManager00.SuspendBinding();
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Height = 0;
                dataGridView1.Rows[0].Visible = false;
                dataGridView1.Refresh();
                currencyManager00.ResumeBinding();
                if (!m_roi_mode && m_roi_click_auto_mode)
                {
                    if (!radioButton2.Checked)
                    {
                        radioButton1.Checked = false;
                        radioButton2.Checked = true;
                    }
                    //Thread.Sleep(30);
                    Thread_INSPECTION();
                }
                //for (int i = 0; i < Application.OpenForms.Count; i++)
                //{
                //    Form f = Application.OpenForms[i];
                //    if (f.GetType() == typeof(Frm_Trackbar))
                //    {
                //        f.Close();
                //    }
                //}

                t_idx = LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
            }
            catch
            {

            }
        }

        private int t_selected_row_idx = 0;
        private string[] t_before_alg0 = new string[4];
        private string[] t_before_alg = new string[4];
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int t_idx = listBox1.SelectedIndex;
            //LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_idx;
            if (t_idx < 0)
            {
                return;
            }
            t_selected_row_idx = e.RowIndex;
            if (e.ColumnIndex == 1 && e.RowIndex == 0)
            {
                if (Cam_Num == 0)
                {
                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 1)
                {
                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 2)
                {
                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 3)
                {
                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse == false ? "X" : "O";
                }
                dataGridView1.Rows[0].Visible = false;
            }
            else if (e.ColumnIndex == 1 && e.RowIndex == 5)
            {
                t_before_alg0[Cam_Num] = dataGridView1.Rows[5].Cells[1].Value.ToString();
            }
            else if (e.ColumnIndex == 1 && e.RowIndex == 8)
            {
                t_before_alg[Cam_Num] = dataGridView1.Rows[8].Cells[1].Value.ToString();
            }
            //Referesh_Select_Menu();
        }

        private void button_OPEN_Click(object sender, EventArgs e)
        {
            // 이미지를 불러와서 Opencv 클래스로 넣음.
            OpenFileDialog openPanel = new OpenFileDialog();
            openPanel.InitialDirectory = ".\\";
            openPanel.Filter = "All image files|*.jpg;*.bmp;*.png";
            if (openPanel.ShowDialog() == DialogResult.OK)
            {
                if (pictureBox_Image.Image != null)
                {
                    pictureBox_Image.Image = null;
                }
                // Load bitmap
                ImageInfo imageInfo = null;
                Bitmap t_Image = ImageDecoder.DecodeFromFile(openPanel.FileName, out imageInfo);

                if (t_Image.PixelFormat == PixelFormat.Format32bppRgb)
                {
                    Bitmap tt_Image = ImageDecoder.DecodeFromFile(openPanel.FileName, out imageInfo);
                    t_Image = null;
                    t_Image = ConvertTo24((Bitmap)tt_Image.Clone());
                    tt_Image.Dispose();
                }

                pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                //propertyGrid1.SelectedObject = imageInfo;
                //propertyGrid1.ExpandAllGridItems();

                //if (imageInfo.BitsPerPixel == 24 || imageInfo.BitsPerPixel == 32)
                //{
                //    if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                //    {
                //        byte[] arr = BmpToArray(t_Image);
                //        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                //        if (Cam_Num == 0)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                //        }
                //    }
                //    else
                //    {
                //        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                //        byte[] arr = BmpToArray(grayImage);
                //        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                //        if (Cam_Num == 0)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                //        }

                //        grayImage.Dispose();
                //    }
                //} else
                //{
                //    byte[] arr = BmpToArray(t_Image);
                //    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                //    if (Cam_Num == 0)
                //    {
                //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                //    }
                //    else if (Cam_Num == 1)
                //    {
                //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                //    }
                //    else if (Cam_Num == 2)
                //    {
                //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                //    }
                //    else if (Cam_Num == 3)
                //    {
                //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                //    }

                //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(Cam_Num);

                t_Image.Dispose();

                Fit_Size();

                //if (!m_roi_mode)
                {
                    if (m_roi_click_auto_mode)
                    {
                        if (!radioButton2.Checked)
                        {
                            radioButton1.Checked = false;
                            radioButton2.Checked = true;
                        }
                        Thread.Sleep(30);
                        Thread_INSPECTION();
                    }
                }
                //button_INSPECTION_Click(sender, e);
            }
        }

        public void button_SNAPSHOT_Click(object sender, EventArgs e)
        {
            if (Cam_SW.ElapsedMilliseconds < m_Camera_Interval)
            {
                LVApp.Instance().m_Config.Realtime_Running_Check[Cam_Num] = false;
                return;
            }
            Cam_SW.Reset();
            Cam_SW.Start();

            if (LVApp.Instance().m_mainform.ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
            {

            }
            if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 4)
            {
                if (Cam_Num == 0)
                {
                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(500);
                    }
                    if (LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex] == null)
                    {
                        LVApp.Instance().m_Config.Realtime_Running_Check[Cam_Num] = false;
                        return;
                    }
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }
                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex].Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
                else if (Cam_Num == 1)
                {
                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(500);
                    }

                    if (LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex] == null)
                    {
                        return;
                    }
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }
                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex].Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
                else if (Cam_Num == 2)
                {
                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(200);
                    }
                    if (LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex] == null)
                    {
                        return;
                    }
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }

                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex].Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
                else if (Cam_Num == 3)
                {
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }

                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(200);
                    }

                    if (LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex] == null)
                    {
                        return;
                    }
                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex].Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
            }
            else
            {
                if (Cam_Num == 0)
                {
                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(200);
                    }
                    if (LVApp.Instance().m_mainform.ctrCam1.m_bitmap == null)
                    {
                        LVApp.Instance().m_Config.Realtime_Running_Check[Cam_Num] = false;
                        return;
                    }
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }
                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam1.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
                else if (Cam_Num == 1)
                {
                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(200);
                    }

                    if (LVApp.Instance().m_mainform.ctrCam2.m_bitmap == null)
                    {
                        return;
                    }
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }
                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam2.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
                else if (Cam_Num == 2)
                {
                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(200);
                    }
                    if (LVApp.Instance().m_mainform.ctrCam3.m_bitmap == null)
                    {
                        return;
                    }
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }

                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam3.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
                else if (Cam_Num == 3)
                {
                    if (pictureBox_Image.Image != null)
                    {
                        pictureBox_Image.Image = null;
                    }

                    if (!LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4])
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonOneShot_Click(sender, e);
                        Thread.Sleep(200);
                    }

                    if (LVApp.Instance().m_mainform.ctrCam4.m_bitmap == null)
                    {
                        return;
                    }
                    Bitmap t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam4.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                    pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                    t_Image.Dispose();
                }
            }

            if (pictureBox_Image.Image != null)
            {
                Fit_Size();
                if (m_roi_click_auto_mode)
                {
                    if (!radioButton2.Checked)
                    {
                        radioButton1.Checked = false;
                        radioButton2.Checked = true;
                    }
                    Thread.Sleep(30);
                    Thread_INSPECTION();
                }
                else
                {
                    LVApp.Instance().m_Config.Realtime_Running_Check[Cam_Num] = false;
                }
                //button_INSPECTION_Click(sender, e);
            }

        }

        public bool load_check = false;
        bool t_button_LOAD_Click_flag = false;
        public void button_LOAD_Click(object sender, EventArgs e)
        {
            t_run_check = false;
            if (t_button_LOAD_Click_flag || t_button_SAVE_Click_flag)
            {
                return;
            }
            t_button_LOAD_Click_flag = true;
            if (LVApp.Instance().m_Config.m_Model_Name == "")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("注册和使用模型.");
                }
                t_button_LOAD_Click_flag = false;
                return;
            }


            try
            {
                int t_s_idx = listBox1.SelectedIndex;
                dataGridView1.ClearSelection();
                listBox1.ClearSelected();
                for (int i = 1; i < listBox1.Items.Count; i++)
                {
                    listBox1.SetItemChecked(i, false);
                }
                if (t_s_idx < 0)
                {
                    t_s_idx = 0;
                }
                //LVApp.Instance().m_Config.Load_Judge_Data();
                if (Cam_Num == 0)
                {
                    string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM0_ROI_data.csv";
                    if (System.IO.File.Exists(filename))
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Clear();
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM0_image.bmp";
                    if (System.IO.File.Exists(filename1))
                    {
                        ImageInfo imageInfo = null;
                        Bitmap t_Image = ImageDecoder.DecodeFromFile(filename1, out imageInfo);
                        pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                        pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                        t_Image.Dispose();
                    }
                    else
                    {
                        pictureBox_Image.Image = null;
                        pictureBox_RImage.Image = null;
                    }
                }
                else if (Cam_Num == 1)
                {
                    string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM1_ROI_data.csv";
                    if (System.IO.File.Exists(filename))
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Clear();
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM1_image.bmp";
                    if (System.IO.File.Exists(filename1))
                    {
                        ImageInfo imageInfo = null;
                        Bitmap t_Image = ImageDecoder.DecodeFromFile(filename1, out imageInfo);
                        pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                        pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                        t_Image.Dispose();
                    }
                    else
                    {
                        pictureBox_Image.Image = null;
                        pictureBox_RImage.Image = null;
                    }
                }
                else if (Cam_Num == 2)
                {
                    string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM2_ROI_data.csv";
                    if (System.IO.File.Exists(filename))
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Clear();
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM2_image.bmp";
                    if (System.IO.File.Exists(filename1))
                    {
                        ImageInfo imageInfo = null;
                        Bitmap t_Image = ImageDecoder.DecodeFromFile(filename1, out imageInfo);
                        pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                        pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                        t_Image.Dispose();
                    }
                    else
                    {
                        pictureBox_Image.Image = null;
                        pictureBox_RImage.Image = null;
                    }
                }
                else if (Cam_Num == 3)
                {
                    string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM3_ROI_data.csv";
                    if (System.IO.File.Exists(filename))
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Clear();
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM3_image.bmp";
                    if (System.IO.File.Exists(filename1))
                    {
                        ImageInfo imageInfo = null;
                        Bitmap t_Image = ImageDecoder.DecodeFromFile(filename1, out imageInfo);
                        pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                        pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                        t_Image.Dispose();
                    }
                    else
                    {
                        pictureBox_Image.Image = null;
                        pictureBox_RImage.Image = null;
                    }
                }

                //listBox1.SetItemChecked(1, true);
                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.dataGridView_Setting_0.ClearSelection();
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                    listBox1.SetItemChecked(0, true);
                    listBox1.SetItemChecked(1, true);
                    LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[0][0] = true;
                    for (int i = 2; i < listBox1.Items.Count; i++)
                    {
                        string[] str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[i][1].ToString().Split('_');
                        if (str[0] == "O")
                        {
                            listBox1.SetItemChecked(i, true);
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i - 1][0] = true;
                        }
                        else
                        {
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i - 1][0] =
                            LVApp.Instance().m_Config.Cam0_rect[i].mUse =
                            LVApp.Instance().m_Config.Cam0_rect[i].mView = false;
                        }
                    }
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.dataGridView_Setting_1.ClearSelection();
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ClearSelection();
                    listBox1.SetItemChecked(0, true);
                    listBox1.SetItemChecked(1, true);
                    LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[0][0] = true;
                    for (int i = 2; i < listBox1.Items.Count; i++)
                    {
                        string[] str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[i][1].ToString().Split('_');
                        if (str[0] == "O")
                        {
                            listBox1.SetItemChecked(i, true);
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i - 1][0] = true;
                        }
                        else
                        {
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i - 1][0] = 
                            LVApp.Instance().m_Config.Cam1_rect[i].mUse = 
                            LVApp.Instance().m_Config.Cam1_rect[i].mView = false;
                        }
                    }
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.dataGridView_Setting_2.ClearSelection();
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ClearSelection();
                    listBox1.SetItemChecked(0, true);
                    listBox1.SetItemChecked(1, true);
                    LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[0][0] = true;
                    for (int i = 2; i < listBox1.Items.Count; i++)
                    {
                        string[] str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[i][1].ToString().Split('_');
                        if (str[0] == "O")
                        {
                            listBox1.SetItemChecked(i, true);
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i - 1][0] = true;
                        }
                        else
                        {
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i - 1][0] =
                            LVApp.Instance().m_Config.Cam2_rect[i].mUse =
                            LVApp.Instance().m_Config.Cam2_rect[i].mView = false;
                        }
                    }
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.dataGridView_Setting_3.ClearSelection();
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ClearSelection();
                    listBox1.SetItemChecked(0, true);
                    listBox1.SetItemChecked(1, true);
                    LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[0][0] = true;
                    for (int i = 2; i < listBox1.Items.Count; i++)
                    {
                        string[] str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[i][1].ToString().Split('_');
                        if (str[0] == "O")
                        {
                            listBox1.SetItemChecked(i, true);
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i - 1][0] = true;
                        }
                        else
                        {
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i - 1][0] =
                            LVApp.Instance().m_Config.Cam3_rect[i].mUse =
                            LVApp.Instance().m_Config.Cam3_rect[i].mView = false;
                        }
                    }
                }

                listBox1.SelectedIndex = t_s_idx;
                MainDB_to_SubDB();
                Fit_Size();
                this.Refresh();

                //listBox1.SelectedIndex = 0;

                //load_check = true;
                //int t_num = listBox1.SelectedIndex;
                //if (t_num < 0)
                //{
                //    listBox1.SelectedIndex = 0;
                //    LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = 0;
                //    Referesh_Select_Menu(false);
                //    MainDB_to_SubDB();

                //}
                //else
                //{
                //    //listBox1.SelectedIndex = -1;
                //    //listBox1.SelectedIndex = 0;
                //    LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_num;
                //    Referesh_Select_Menu(false);
                //    MainDB_to_SubDB();
                //    Referesh_Select_Menu(true);
                //    listBox1.SelectedIndex = t_num;
                //}
                //comboBox_TABLETYPE.SelectedIndex = Properties.Settings.Default.PC_No;
                if (LVApp.Instance().m_Config.m_Model_Name == "")
                {
                    t_button_LOAD_Click_flag = false;
                    return;
                }

                LVApp.Instance().m_mainform.ctr_Admin_Param1.Load_Combobox();

                //FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + LVApp.Instance().m_Config.m_Model_Name + ".xlsx");
                //if (!newFile.Exists)
                //{
                //    t_button_LOAD_Click_flag = false;
                //    return;
                //}
                //using (ExcelPackage package = new ExcelPackage(newFile))
                //{
                //    // Add a worksheet to the empty workbook
                //    ExcelWorksheet worksheet = package.Workbook.Worksheets[3];
                //    for (int i = 0; i < 4; i++)
                //    {
                //        if (worksheet.Cells[11, i + 1].Value != null)
                //        {
                //            int.TryParse(worksheet.Cells[11, i + 1].Value.ToString(), out LVApp.Instance().m_Config.m_Camera_Position[i]);
                //        }
                //        else
                //        {
                //            LVApp.Instance().m_Config.m_Camera_Position[i] = 0;
                //        }
                //        if (worksheet.Cells[12, i + 1].Value != null)
                //        {
                //            int.TryParse(worksheet.Cells[12, i + 1].Value.ToString(), out LVApp.Instance().m_Config.nTableType[i]);
                //        }
                //        else
                //        {
                //            LVApp.Instance().m_Config.nTableType[i] = 0;
                //        }
                //        if (i == 0)
                //        {
                //            LVApp.Instance().m_mainform.ctr_ROI1.comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_Config.m_Camera_Position[i];
                //            LVApp.Instance().m_mainform.ctr_ROI1.comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_Config.nTableType[i];
                //        }
                //        else if (i == 1)
                //        {
                //            LVApp.Instance().m_mainform.ctr_ROI2.comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_Config.m_Camera_Position[i];
                //            LVApp.Instance().m_mainform.ctr_ROI2.comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_Config.nTableType[i];
                //        }
                //        else if (i == 2)
                //        {
                //            LVApp.Instance().m_mainform.ctr_ROI3.comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_Config.m_Camera_Position[i];
                //            LVApp.Instance().m_mainform.ctr_ROI3.comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_Config.nTableType[i];
                //        }
                //        else if (i == 3)
                //        {
                //            LVApp.Instance().m_mainform.ctr_ROI4.comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_Config.m_Camera_Position[i];
                //            LVApp.Instance().m_mainform.ctr_ROI4.comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_Config.nTableType[i];
                //        }

                //    }
                //}


                //LVApp.Instance().m_Config.Load_Judge_Data();


                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1.SelectedIndex, Cam_Num);
                }


                //LVApp.Instance().m_Config.Load_Judge_Data();


                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1.SelectedIndex, Cam_Num);
                }
                //if (!LVApp.Instance().m_mainform.Force_close)
                //{
                //    ctr_ROI_Guide1.add_Log("Parameter loaded!");
                //}

                for (int kk = 1; kk < 41; kk++)
                {
                    if (sender != null)
                    {
                        if (sender.Equals(button_LOAD))
                        {
                            LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num, kk - 1] = false;
                        }
                    }
                    AI_Model_Load(Cam_Num, kk - 1);
                }

                Thread.Sleep(30);
                LVApp.Instance().m_Config.Set_Parameters();

                Thread_INSPECTION();
                //button_INSPECTION_Click(sender, e);
                //AutoClosingMessageBox.Show("Loaded.", "Notice", 500);
                //Referesh_Select_Menu(true);
                t_button_LOAD_Click_flag = false;

                if (sender.Equals(button_LOAD))
                {
                    AutoClosingMessageBox.Show("Loaded!", "Notice", 1000);
                    //TODO
                }
            }
            catch
            {
                t_button_LOAD_Click_flag = false;
            }
        }

        bool t_button_SAVE_Click_flag = false;
        public void button_SAVE_Click(object sender, EventArgs e)
        {
            t_run_check = false;
            if (t_button_SAVE_Click_flag)
            {
                return;
            }
            t_button_SAVE_Click_flag = true;
            if (LVApp.Instance().m_Config.m_Model_Name == "")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("注册和使用模型.");
                }
                t_button_SAVE_Click_flag = false;
                return;
            }
            //if (!radioButton1.Checked && radioButton2.Checked)
            //{
            //    radioButton1.Checked = true;
            //    radioButton2.Checked = false;
            //    //button_SAVE_Click(sender, e);
            //    //return;
            //}
            DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name);
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }

            //dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup");
            //// 폴더가 존재하지 않으면
            //if (dir.Exists == false)
            //{
            //    // 새로 생성합니다.
            //    dir.Create();
            //}
            //dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name);
            //// 폴더가 존재하지 않으면
            //if (dir.Exists == false)
            //{
            //    // 새로 생성합니다.
            //    dir.Create();
            //}
            //dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param");
            //// 폴더가 존재하지 않으면
            //if (dir.Exists == false)
            //{
            //    // 새로 생성합니다.
            //    dir.Create();
            //}

            try
            {
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    float Img_x_size = pictureBox_Image.Width;
                    float Img_y_size = pictureBox_Image.Height;
                    float x_ratio = 1; float y_ratio = 1;
                    if (pictureBox_Image.Image != null)
                    {
                        Img_x_size = (float)pictureBox_Image.Image.Width;
                        Img_y_size = (float)pictureBox_Image.Image.Height;
                        x_ratio = Img_x_size / (float)pictureBox_Image.Width;
                        y_ratio = Img_y_size / (float)pictureBox_Image.Height;
                    }

                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.ctr_ROI1.SubDB_to_MainDB();
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                        string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM0_ROI_data.csv";
                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].WriteToCsvFile(filename);

                        if (pictureBox_Image.Image != null)
                        {
                            filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM0_image.bmp";
                            ((Bitmap)(pictureBox_Image.Image.Clone())).Save(filename);
                        }
                        //LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonOneShot_Click(sender, e);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.ctr_ROI2.SubDB_to_MainDB();
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                        string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM1_ROI_data.csv";
                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].WriteToCsvFile(filename);

                        if (pictureBox_Image.Image != null)
                        {
                            filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM1_image.bmp";
                            ((Bitmap)(pictureBox_Image.Image.Clone())).Save(filename);
                        }
                        //LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonOneShot_Click(sender, e);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.ctr_ROI3.SubDB_to_MainDB();
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                        string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM2_ROI_data.csv";
                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].WriteToCsvFile(filename);

                        if (pictureBox_Image.Image != null)
                        {
                            filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM2_image.bmp";
                            ((Bitmap)(pictureBox_Image.Image.Clone())).Save(filename);
                        }
                        //LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonOneShot_Click(sender, e);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.ctr_ROI4.SubDB_to_MainDB();
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                        string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM3_ROI_data.csv";
                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].WriteToCsvFile(filename);

                        if (pictureBox_Image.Image != null)
                        {
                            filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM3_image.bmp";
                            ((Bitmap)(pictureBox_Image.Image.Clone())).Save(filename);
                        }
                        //LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonOneShot_Click(sender, e);
                    }
                    //Properties.Settings.Default.PC_No = comboBox_TABLETYPE.SelectedIndex;
                    //Properties.Settings.Default.Save();

                    //LVApp.Instance().m_Config.Save_Judge_Data();

                    // Set Parameters
                    LVApp.Instance().m_Config.Set_Parameters();
                    LVApp.Instance().m_Config.Save_Judge_Data();
                    //if (!LVApp.Instance().m_mainform.Force_close)
                    //{
                    //    Referesh_Select_Menu(true);
                    //    //AutoClosingMessageBox.Show("Saved.", "Notice", 500);
                    //}

                    try
                    {
                        FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + LVApp.Instance().m_Config.m_Model_Name + ".xlsx");
                        using (ExcelPackage package = new ExcelPackage(newFile))
                        {
                            // Add a worksheet to the empty workbook
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[3];

                            worksheet.Cells[11, 1].Value = LVApp.Instance().m_Config.m_Camera_Position[0];
                            worksheet.Cells[11, 2].Value = LVApp.Instance().m_Config.m_Camera_Position[1];
                            worksheet.Cells[11, 3].Value = LVApp.Instance().m_Config.m_Camera_Position[2];
                            worksheet.Cells[11, 4].Value = LVApp.Instance().m_Config.m_Camera_Position[3];
                            worksheet.Cells[12, 1].Value = LVApp.Instance().m_Config.nTableType[0];
                            worksheet.Cells[12, 2].Value = LVApp.Instance().m_Config.nTableType[1];
                            worksheet.Cells[12, 3].Value = LVApp.Instance().m_Config.nTableType[2];
                            worksheet.Cells[12, 4].Value = LVApp.Instance().m_Config.nTableType[3];
                            package.Save();
                        }
                    }
                    finally
                    {
                        if (!LVApp.Instance().m_mainform.Force_close)
                        {
                            LVApp.Instance().m_Config.Load_Judge_Data();
                            //AutoClosingMessageBox.Show("Saved!", "Notice", 700);
                        }
                        t_button_SAVE_Click_flag = false;
                    }
                }
                else
                { //검사중일때  
                    LVApp.Instance().m_Config.Set_Parameters();
                    try
                    {
                        FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + LVApp.Instance().m_Config.m_Model_Name + ".xlsx");
                        using (ExcelPackage package = new ExcelPackage(newFile))
                        {
                            // Add a worksheet to the empty workbook
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[3];

                            worksheet.Cells[11, 1].Value = LVApp.Instance().m_Config.m_Camera_Position[0];
                            worksheet.Cells[11, 2].Value = LVApp.Instance().m_Config.m_Camera_Position[1];
                            worksheet.Cells[11, 3].Value = LVApp.Instance().m_Config.m_Camera_Position[2];
                            worksheet.Cells[11, 4].Value = LVApp.Instance().m_Config.m_Camera_Position[3];
                            worksheet.Cells[12, 1].Value = LVApp.Instance().m_Config.nTableType[0];
                            worksheet.Cells[12, 2].Value = LVApp.Instance().m_Config.nTableType[1];
                            worksheet.Cells[12, 3].Value = LVApp.Instance().m_Config.nTableType[2];
                            worksheet.Cells[12, 4].Value = LVApp.Instance().m_Config.nTableType[3];
                            package.Save();
                        }
                    }
                    finally
                    {
                        t_button_SAVE_Click_flag = false;
                    }
                    //LVApp.Instance().m_Config.Save_Judge_Data();
                }
                if (sender.Equals(button_SAVE))
                {
                    AutoClosingMessageBox.Show("Saved!", "Notice", 1000);
                    //TODO
                }
            }
            catch
            {
                t_button_SAVE_Click_flag = false;
            }
        }

        // Convert the coordinates for the image's SizeMode.
        private void ConvertCoordinates(PictureBox pic,
            out int X0, out int Y0, int x, int y)
        {
            int pic_hgt = pic.ClientSize.Height;
            int pic_wid = pic.ClientSize.Width;
            int img_hgt = pic.Image.Height;
            int img_wid = pic.Image.Width;

            X0 = x;
            Y0 = y;
            switch (pic.SizeMode)
            {
                case PictureBoxSizeMode.AutoSize:
                case PictureBoxSizeMode.Normal:
                    // These are okay. Leave them alone.
                    break;
                case PictureBoxSizeMode.CenterImage:
                    X0 = x - (pic_wid - img_wid) / 2;
                    Y0 = y - (pic_hgt - img_hgt) / 2;
                    break;
                case PictureBoxSizeMode.StretchImage:
                    X0 = (int)(img_wid * x / (float)pic_wid);
                    Y0 = (int)(img_hgt * y / (float)pic_hgt);
                    break;
                case PictureBoxSizeMode.Zoom:
                    float pic_aspect = pic_wid / (float)pic_hgt;
                    float img_aspect = img_wid / (float)img_hgt;
                    if (pic_aspect > img_aspect)
                    {
                        // The PictureBox is wider/shorter than the image.
                        Y0 = (int)(img_hgt * y / (float)pic_hgt);

                        // The image fills the height of the PictureBox.
                        // Get its width.
                        float scaled_width = img_wid * pic_hgt / img_hgt;
                        float dx = (pic_wid - scaled_width) / 2;
                        X0 = (int)((x - dx) * img_hgt / (float)pic_hgt);
                    }
                    else
                    {
                        // The PictureBox is taller/thinner than the image.
                        X0 = (int)(img_wid * x / (float)pic_wid);

                        // The image fills the height of the PictureBox.
                        // Get its height.
                        float scaled_height = img_hgt * pic_wid / img_wid;
                        float dy = (pic_hgt - scaled_height) / 2;
                        Y0 = (int)((y - dy) * img_wid / pic_wid);
                    }
                    break;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Setup mode
            if (!pictureBox_Image.Visible)
            {
                pictureBox_Image.Visible = true;
                pictureBox_RImage.Visible = false;
                m_roi_mode = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // ALG mode
            if (!pictureBox_RImage.Visible)
            {
                pictureBox_Image.Visible = false;
                pictureBox_RImage.Visible = true;
                m_roi_mode = false;
            }
        }

        public Bitmap ConvertBitmap(byte[] frame, int width, int height, int ch)
        {
            if (frame == null)
            {
                return null;
            }
            if (ch == 3)
            {
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                //BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),

                //                                ImageLockMode.WriteOnly,PixelFormat.Format24bppRgb);

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),

                                                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                System.Runtime.InteropServices.Marshal.Copy(frame, 0, data.Scan0, frame.Length);

                bmp.UnlockBits(data);

                return bmp;
            }
            else
            {
                Bitmap res = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                BitmapData data

               = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

                IntPtr ptr = data.Scan0;
                Marshal.Copy(frame, 0, ptr, width * height);
                res.UnlockBits(data);
                ColorPalette cp = res.Palette;
                for (int i = 0; i < 256; i++)


                    cp.Entries[i] = Color.FromArgb(i, i, i);


                res.Palette = cp;
                return res;
            }
        }


        public static Byte[] BmpToArray(Bitmap value)
        {
            BitmapData data = value.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.ReadOnly, value.PixelFormat);
            try
            {
                IntPtr ptr = data.Scan0;
                int bytes = Math.Abs(data.Stride) * value.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);
                value.UnlockBits(data);

                return rgbValues;
            }
            finally
            {
            }
        }

        private Bitmap ConvertTo24(Bitmap inputFileName)
        {
            Bitmap bmpIn = inputFileName;

            Bitmap converted = new Bitmap(bmpIn.Width, bmpIn.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(converted))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.DrawImageUnscaled(bmpIn, 0, 0);
            }
            return converted;
        }

        bool t_run_check = false;
        int listBox1_SelectedIndex = 0;
        public void Thread_INSPECTION()
        {
            try
            {
                t_AI_Result = string.Empty;
                if (t_run_check || LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (ROI_thread == null)
                    {
                        return;
                    }
                    ROI_thread.Abort();
                    t_run_check = false;
                    if (ROI_thread.IsAlive)
                    {
                        ROI_thread.Abort();
                    }
                    return;
                }
                t_run_check = true;

                if (LVApp.Instance().m_mainform.Run_SW[Cam_Num] == null)
                {
                    LVApp.Instance().m_mainform.Run_SW[Cam_Num] = new Stopwatch();
                }
                LVApp.Instance().m_mainform.Run_SW[Cam_Num].Reset();
                LVApp.Instance().m_mainform.Run_SW[Cam_Num].Start();

                if (LVApp.Instance().m_Config.m_Model_Name == "")
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        LVApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                        MessageBox.Show("모델을 등록후 사용하세요.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        LVApp.Instance().m_mainform.add_Log("Use after registering a model.");
                        MessageBox.Show("Use after registering a model.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        LVApp.Instance().m_mainform.add_Log("注册和使用模型.");
                        MessageBox.Show("注册和使用模型.");
                    }
                    t_run_check = false;
                    if (ROI_thread.IsAlive)
                    {
                        ROI_thread.Abort();
                    }
                    return;
                }

                if (listBox1_SelectedIndex < 0)
                {
                    t_run_check = false;
                    ROI_thread.Abort();
                    if (ROI_thread.IsAlive)
                    {
                        ROI_thread.Abort();
                    }
                    return;
                }
                //LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1_SelectedIndex;

                //if (radioButton1.Checked && !radioButton2.Checked)
                //{
                //    radioButton1.Checked = false;
                //    radioButton2.Checked = true;
                //    //button_INSPECTION_Click(sender, e);
                //    //return;
                //}

                //pictureBox_RImage.Image = null;

                if (pictureBox_Image.Image == null)
                {
                    if (ROI_thread == null)
                    { return; }
                    t_run_check = false;
                    ROI_thread.Abort();
                    if (ROI_thread.IsAlive)
                    {
                        ROI_thread.Abort();
                    }
                    return;
                }

                //Properties.Settings.Default.PC_No = comboBox_TABLETYPE.SelectedIndex;
                //Properties.Settings.Default.Save();

                //SubDB_to_MainDB();



                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("[검사중...]에는 변수값만 반영됩니다. 오른쪽 영상처리는 검사를 중지후 수행하세요.", "Notice", 2000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Parameters downloaded but can't process the image during inspection!", "Notice", 2000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        AutoClosingMessageBox.Show("参数已下载，但在检查期间无法处理图像!", "Notice", 2000);
                    }
                    t_run_check = false;
                    ROI_thread.Abort();
                    if (ROI_thread.IsAlive)
                    {
                        ROI_thread.Abort();
                    }
                    return;
                }

                Bitmap t_Image = null;
                //if (pictureBox_RImage.InvokeRequired)
                //{
                    if (pictureBox_Image.Image == null)
                    {
                        t_run_check = false;
                        ROI_thread.Abort();
                        if (ROI_thread.IsAlive)
                        {
                            ROI_thread.Abort();
                        }
                        return;
                    }
                    if (pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb)
                    {
                        t_Image = null;
                        t_Image = ConvertTo24((Bitmap)pictureBox_Image.Image.Clone());
                    }
                    else
                    {
                        if (pictureBox_Image.Image == null)
                        {
                            t_run_check = false;
                            ROI_thread.Abort();
                            if (ROI_thread.IsAlive)
                            {
                                ROI_thread.Abort();
                            }
                            return;
                        }
                        t_Image = (Bitmap)pictureBox_Image.Image.Clone();
                    }
                //}
                //else
                //{
                //    if (pictureBox_Image.Image == null)
                //    {
                //        t_run_check = false;
                //        ROI_thread.Abort();
                //        if (ROI_thread.IsAlive)
                //        {
                //            ROI_thread.Abort();
                //        }
                //        return;
                //    }
                //    if (pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb)
                //    {
                //        t_Image = null;
                //        t_Image = ConvertTo24((Bitmap)pictureBox_Image.Image.Clone());
                //    }
                //    else
                //    {
                //        if (pictureBox_Image.Image == null)
                //        {
                //            t_run_check = false;
                //            ROI_thread.Abort();
                //            if (ROI_thread.IsAlive)
                //            {
                //                ROI_thread.Abort();
                //            }
                //            return;
                //        }
                //        t_Image = (Bitmap)pictureBox_Image.Image.Clone();
                //    }
                //}

                if (t_Image.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                    {
                        byte[] arr = BmpToArray(t_Image);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);

                        if (Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                        else if (Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                        else if (Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                        else if (Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = BmpToArray(grayImage);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                        if (Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        else if (Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        else if (Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        else if (Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = BmpToArray(t_Image);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);

                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                }

                int t_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_Num].Count;
                //if (t_CNT > 0)
                //{
                //    for (int i = 0; i < t_CNT; i++)
                //    {
                //        UTIL.IPSST_Config.AIParam t_AIParam = LVApp.Instance().m_Config.m_AIParam[Cam_Num][i];
                //        t_AIParam.Image = cropAtRect(t_Image.Clone() as Bitmap, t_AIParam.ROI);
                //        LVApp.Instance().m_Config.m_AIParam[Cam_Num][i] = t_AIParam;
                //    }
                //}
                        //Thread.Sleep(100);
                byte[] Dst_Img = null;
                int width = 0, height = 0, ch = 0;

                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1_SelectedIndex, Cam_Num);
                //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(Cam_Num);
                LVApp.Instance().m_mainform.m_ImProClr_Class.ROI_Object_Find(out Dst_Img, out width, out height, out ch, Cam_Num);
                if (listBox1_SelectedIndex == 0)
                {
                    Thread.Sleep(100);
                    //Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(100);
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(Cam_Num);
                    LVApp.Instance().m_mainform.ctr_Manual1.Run_Inspection(Cam_Num, ref t_Image);
                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image1(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image2(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image3(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }

                    if (t_CNT > 0)
                    {
                        int t_idx_check = -1;
                        for (int i = 0; i < t_CNT; i++)
                        {
                            UTIL.LV_Config.AIParam t_AIParam = LVApp.Instance().m_Config.m_AIParam[Cam_Num][i];
                            if (t_AIParam.USE && t_AIParam.ROI_IDX == listBox1_SelectedIndex - 1)
                            {
                                t_idx_check = i;
                                break;
                            }
                        }

                        if (t_idx_check > -1 && listBox1.Items[listBox1_SelectedIndex].ToString().Substring(0,2).ToUpper().Contains("AI"))
                        {
                            t_AI_Result = LVApp.Instance().m_Config.m_AIParam[Cam_Num][t_idx_check].Result + " (T/T=" + LVApp.Instance().m_Config.m_AIParam[Cam_Num][t_idx_check].T_T.ToString() + "ms)";
                        }
                    }
                }
                t_Image.Dispose();

                Bitmap t_RImage = (Bitmap)ConvertBitmap(Dst_Img, width, height, 3).Clone();

                if (pictureBox_RImage.InvokeRequired)
                {
                    pictureBox_RImage.Invoke((MethodInvoker)delegate
                    {
                        pictureBox_RImage.Image = t_RImage.Clone() as Bitmap;
                        pictureBox_RImage.Refresh();
                    });
                }
                else
                {
                    pictureBox_RImage.Image = t_RImage.Clone() as Bitmap;
                    pictureBox_RImage.Refresh();
                }
                t_RImage.Dispose();
                //radioButton2.Checked = true;
                //}
                //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(Cam_Num);

                LVApp.Instance().m_Config.Realtime_Running_Check[Cam_Num] = false;

                LVApp.Instance().m_mainform.Run_SW[Cam_Num].Stop();

                t_run_check = false;
                //ROI_thread.Abort();
            }
            catch
            {
                t_run_check = false;
            }
            //GC.Collect();
        }

        public Bitmap cropAtRect(Bitmap source, Rectangle section)
        {
            try
            {
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                bmp.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    // Draw the given area (section) of the source image
                    // at location 0,0 on the empty bitmap (bmp)
                    g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                    return bmp;
                }
            }
            catch
            {
                t_run_check = false;
            }
            return null;
        }

        public void button_INSPECTION_Click(object sender, EventArgs e)
        {
            if (t_run_check || t_button_SAVE_Click_flag || t_button_LOAD_Click_flag)
            {
                return;
            }
            
            if (listBox1_SelectedIndex < 0)
            {
                return;
            }

            //button_SAVE_Click(sender, e);
            LVApp.Instance().m_Config.Set_Parameters();
            LVApp.Instance().m_Config.Set_ROI(Cam_Num);
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }

            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1_SelectedIndex, Cam_Num);

            if (ROI_thread != null)
            {
                if (ROI_thread.ThreadState == System.Threading.ThreadState.Running)
                {
                    if (ROI_thread.IsAlive)
                    {
                        ROI_thread.Abort();
                    }
                }
                ROI_thread = null;
                Thread.Sleep(50);
                //GC.Collect();
            }

            //Stopwatch t_stw = new Stopwatch();
            //t_stw.Start();
            //Bitmap t_bmp = pictureBox_Image.Image.Clone() as Bitmap;
            //if (t_bmp != null)
            //{
            //    LVApp.Instance().m_AI_Pro.AI_Recognition_Image(Cam_Num, ref t_bmp);
            //}
            //t_stw.Stop();
            //ctr_ROI_Guide1.add_Log("AI: " + t_stw.ElapsedMilliseconds.ToString() + " ms");
            //pictureBox_RImage.Image = pictureBox_Image.Image.Clone() as Bitmap;
            if (!radioButton2.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }

            ROI_thread = new Thread(Thread_INSPECTION);
            ROI_thread.IsBackground = true;
            ROI_thread.Start();
            Thread.Sleep(2);
        }


        private void AI_Model_Load(int Cam_Num, int ROI_Num)
        {
            if (LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num, ROI_Num])
            {
                return;
            }
            string Model_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + (ROI_Num+1).ToString("00") + "_Model.pb";
            string Label_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + (ROI_Num+1).ToString("00") + "_Label.txt";
            if (!System.IO.File.Exists(Label_Path))
            {
                Label_Path = string.Empty;
            }
            if (System.IO.File.Exists(Model_Path))// && System.IO.File.Exists(Label_Path))
            {
                LVApp.Instance().m_AI_Pro.Model_Load(Cam_Num, ROI_Num, Model_Path, Label_Path);
            }

            if (dataGridView1.Rows[12].Cells[0].Value.ToString().Contains("판정 기준율") || dataGridView1.Rows[12].Cells[0].Value.ToString().Contains("Matching Rate"))
            {
                if (LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num, ROI_Num])
                {
                    dataGridView1.Rows[13].Cells[1].Value = "1";
                }
                else
                {
                    dataGridView1.Rows[13].Cells[1].Value = "0";
                }
            }
            dataGridView1.Refresh();
        }

        private void pictureBox_RImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                //radioButton1.Checked = true;
                //radioButton2.Checked = false;
                //pictureBox_RImage.Refresh();
            }
            else
            {
                if (pictureBox_RImage.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveMain));
                        cm.MenuItems.Add("Gray 값 출력", new EventHandler(PictureBoxViewGrayR));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveMain));
                        cm.MenuItems.Add("View Gray value", new EventHandler(PictureBoxViewGrayR));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("结果视图", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSaveMain));
                        cm.MenuItems.Add("查看灰色值", new EventHandler(PictureBoxViewGrayR));
                    }
                    pictureBox_RImage.ContextMenu = cm;
                    pictureBox_RImage.ContextMenu.Show(pictureBox_RImage, e.Location);
                    pictureBox_RImage.ContextMenu = null;
                }
            }
        }

        private void PictureBoxPopupViewMain(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_RImage.Image.Clone();
            View_form.Show();
        }


        private void PictureBoxPopupViewOri(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_Image.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSaveMain(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_RImage.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp);
                }
            }
        }

        private void PictureBoxSaveOri(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_Image.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp);
                }
            }
        }

        string t_Save_Temp_Directory = LVApp.Instance().excute_path;
        int t_Save_CNT = 0;
        public void Image_SaveFileDialog(System.Drawing.Image bmp)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = t_Save_Temp_Directory;
            SaveFileDialog1.RestoreDirectory = true;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                SaveFileDialog1.Title = "이미지 저장";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                SaveFileDialog1.Title = "Image save";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                SaveFileDialog1.Title = "图像保存";
            }

            SaveFileDialog1.Filter = "All image files|*.jpg;*.bmp;*.png";
            SaveFileDialog1.FilterIndex = 2;
            SaveFileDialog1.FileName = "Save_C" + Cam_Num.ToString() + "_No" + t_Save_CNT.ToString("000") + ".png";
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                t_Save_CNT++;
                if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "bmp"
                    || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "Bmp"
                    || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "BMP")
                {
                    bmp.Save(SaveFileDialog1.FileName, ImageFormat.Bmp);
                }
                else if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "jpg"
                  || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "Jpg"
                  || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "JPG")
                {
                    bmp.Save(SaveFileDialog1.FileName, ImageFormat.Jpeg);
                }
                else if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "png"
                  || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "Png"
                  || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "PNG")
                {
                    bmp.Save(SaveFileDialog1.FileName, ImageFormat.Png);
                }
                t_Save_Temp_Directory = Path.GetDirectoryName(SaveFileDialog1.FileName);
            }
        }

        private void PictureBoxResultviewMain(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.Alg_TextView)
            {
                LVApp.Instance().m_Config.Alg_TextView = false;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_TextView.Checked = false;
            }
            else
            {
                LVApp.Instance().m_Config.Alg_TextView = true;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_TextView.Checked = true;
            }
            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);
        }

        private void comboBox_PC_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox_TABLETYPE.SelectedIndex >= 0)
            //{
            //    if (Cam_Num == 0)
            //    {
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE0.SelectedIndex;
            //        //Properties.Settings.Default.CAM0_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM0_Alg_Type;
            //        comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE0.SelectedIndex;
            //    }
            //    else if (Cam_Num == 1)
            //    {
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE1.SelectedIndex;

            //        //Properties.Settings.Default.CAM1_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM1_Alg_Type;
            //        comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE1.SelectedIndex;
            //    }
            //    else if (Cam_Num == 2)
            //    {
            //       // LVApp.Instance().m_Config.nTableType[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE2.SelectedIndex;

            //        //Properties.Settings.Default.CAM2_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM2_Alg_Type;
            //        comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE2.SelectedIndex;
            //    }
            //    else if (Cam_Num == 3)
            //    {
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE3.SelectedIndex;

            //        //Properties.Settings.Default.CAM3_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
            //        //LVApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM3_Alg_Type;
            //        comboBox_TABLETYPE.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_TABLETYPE3.SelectedIndex;
            //    }

            //if (comboBox_TABLETYPE.SelectedIndex == 0 && comboBox_CAMPOSITION.SelectedIndex == 1 && listBox1.SelectedIndex == 0)
            //{
            //    dataGridView1.Rows[8].Cells[1].Value = "중심 기준";
            //}
        }

        private void comboBox_CAMPOSITION_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox_CAMPOSITION.SelectedIndex >= 0)
            //{
            //    //LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] = comboBox_CAMPOSITION.SelectedIndex;
            //    if (Cam_Num == 0)
            //    {
            //        comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION0.SelectedIndex;
            //        //LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION0.SelectedIndex;
            //    }
            //    else if (Cam_Num == 1)
            //    {
            //        comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION1.SelectedIndex;
            //        //LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION1.SelectedIndex;
            //    }
            //    else if (Cam_Num == 2)
            //    {
            //        comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION2.SelectedIndex;
            //        //LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION2.SelectedIndex;
            //    }
            //    else if (Cam_Num == 3)
            //    {
            //        comboBox_CAMPOSITION.SelectedIndex = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION3.SelectedIndex;
            //        //LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] = LVApp.Instance().m_mainform.ctr_Admin_Param1.comboBox_CAMPOSITION3.SelectedIndex;
            //    }
            //    //if (comboBox_TABLETYPE.SelectedIndex == 0 && comboBox_CAMPOSITION.SelectedIndex == 1 && listBox1.SelectedIndex == 0)
            //    //{
            //    //    dataGridView1.Rows[8].Cells[1].Value = "중심 기준";
            //    //}
            //    //Referesh_Select_Menu(true);
            //}
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 107);
            form.Controls.AddRange(new System.Windows.Forms.Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.PointToClient(Cursor.Position).X > 25)
            {
                //return;
                if (listBox1.SelectedIndex >= 1)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("이름 변경", new EventHandler(Name_Change));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Change name", new EventHandler(Name_Change));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("更改名称", new EventHandler(Name_Change));
                    }

                    listBox1.ContextMenu = cm;
                    listBox1.ContextMenu.Show(listBox1, e.Location);
                    listBox1.ContextMenu = null;
                }

                int t_idx = listBox1.SelectedIndex;
                if (t_idx == 0)
                {
                    return;
                }
                if (Cam_Num == 0)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam0_rect[t_idx].mView = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[t_idx].mView = true;
                    }
                }
                else if (Cam_Num == 1)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam1_rect[t_idx].mView = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[t_idx].mView = true;
                    }
                }
                else if (Cam_Num == 2)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam2_rect[t_idx].mView = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[t_idx].mView = true;
                    }
                }
                else if (Cam_Num == 3)
                {
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = true;
                    }
                    else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = false;
                    }
                    LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam3_rect[t_idx].mView = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                    if (t_idx == 0)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[t_idx].mView = true;
                    }
                }

                pictureBox_Image.Refresh();
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (listBox1.PointToClient(Cursor.Position).X < 25)
            //{
            //    return;
            //}
            //if (e.Button.ToString() == "Left")
            //{
            //    listBox1.Refresh();
            //    pictureBox_Image.Refresh();
            //}
            //else
            //{
            //    if (listBox1.SelectedIndex >= 1)
            //    {
            //        ContextMenu cm = new ContextMenu();
            //        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            //        {//한국어
            //            cm.MenuItems.Add("이름 변경", new EventHandler(Name_Change));
            //        }
            //        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            //        {//영어
            //            cm.MenuItems.Add("Change name", new EventHandler(Name_Change));
            //        }
            //        listBox1.ContextMenu = cm;
            //        listBox1.ContextMenu.Show(listBox1, e.Location);
            //        listBox1.ContextMenu = null;
            //    }
            //}
        }

        private void Name_Change(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex <= 0)
            {
                return;
            }
            int t_sel_idx = listBox1.SelectedIndex;
            listBox1.ClearSelected();
            string value = listBox1.Items[t_sel_idx].ToString();
            string msg = string.Empty;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "신규 이름";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "New Nmae";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "新名称";
            }

            if (InputBox(msg, "New ROI name:", ref value) == DialogResult.OK)
            {
                listBox1.Items[t_sel_idx] = value;
                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_sel_idx - 1][2] = value;
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_sel_idx - 1][2] = value;
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_sel_idx - 1][2] = value;
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_sel_idx - 1][2] = value;
                }
                //listBox1.SetItemChecked(t_sel_idx, true);
                //Thread.Sleep(100);
                listBox1.SelectedIndex = 0;
                Thread.Sleep(500);
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("변경 완료", "Caution", 1000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Change completed", "Caution", 1000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("完成更改", "Caution", 1000);
                }


                //listBox1.SelectedIndex = t_sel_idx;
                //listBox1.Refresh();
                //LVApp.Instance().m_Config.Save_Judge_Data();
            }
        }

        private void Ctr_ROI_SizeChanged(object sender, EventArgs e)
        {
            Fit_Size();
            int t_idx = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (t_idx == 0)
                {
                    row.Height = 0;
                }
                else
                {
                    row.Height = (dataGridView1.Height - dataGridView1.ColumnHeadersHeight) / (dataGridView1.Rows.Count - 5);
                }
                t_idx++;
                //row.Height = (dataGridView1.Height) / dataGridView1.Rows.Count;
            }
        }

        public void Fit_Size()
        {
            try
            {
                splitContainer_RIGHT2.IsSplitterFixed = false;
                splitContainer_RIGHT.IsSplitterFixed = false;

                if (splitContainer_RIGHT2.InvokeRequired)
                {
                    splitContainer_RIGHT2.Invoke((MethodInvoker)delegate
                    {
                        splitContainer_RIGHT2.SplitterDistance = splitContainer1.Panel2.Width;
                    });
                }
                else
                {
                    splitContainer_RIGHT2.SplitterDistance = splitContainer1.Panel2.Width;
                }
                if (splitContainer_RIGHT2.InvokeRequired)
                {
                    splitContainer_RIGHT2.Invoke((MethodInvoker)delegate
                    {
                        splitContainer_RIGHT.SplitterDistance = splitContainer1.Panel2.Height;
                    });
                }
                else
                {
                    splitContainer_RIGHT.SplitterDistance = splitContainer1.Panel2.Height;
                }


                if (pictureBox_Image.Image == null)
                {

                }
                else
                {
                    Bitmap t_bmp = pictureBox_Image.Image.Clone() as Bitmap;

                    double Img_W = (double)t_bmp.Width; 
                    double Img_H = (double)t_bmp.Height;

                    t_bmp.Dispose();

                    double Rw = (double)splitContainer1.Panel2.Width / Img_W;
                    double Rh = (double)splitContainer1.Panel2.Height / Img_H;
                    //if (Rw > Rh) // 가로를 줄여야 함.
                    {
                        // splitContainer_RIGHT2 : 가로 설정
                        if (splitContainer_RIGHT2.InvokeRequired)
                        {
                            splitContainer_RIGHT2.Invoke((MethodInvoker)delegate
                            {
                                splitContainer_RIGHT2.SplitterDistance = (int)((double)splitContainer1.Panel2.Height * Img_W / Img_H);
                            });
                        }
                        else
                        {
                            splitContainer_RIGHT2.SplitterDistance = (int)((double)splitContainer1.Panel2.Height * Img_W / Img_H);
                        }
                    }
                    //else if (Rw < Rh)
                    {
                        // splitContainer_RIGHT : 세로 설정
                        if (splitContainer_RIGHT.InvokeRequired)
                        {
                            splitContainer_RIGHT.Invoke((MethodInvoker)delegate
                            {
                                splitContainer_RIGHT.SplitterDistance = (int)((double)splitContainer1.Panel2.Width * Img_H / Img_W);
                            });
                        }
                        else
                        {
                            splitContainer_RIGHT.SplitterDistance = (int)((double)splitContainer1.Panel2.Width * Img_H / Img_W);
                        }
                    }

                    if (splitContainer_RIGHT2.InvokeRequired)
                    {
                        splitContainer_RIGHT2.Invoke((MethodInvoker)delegate
                        {
                            splitContainer_RIGHT2.SplitterDistance = (int)((Img_W / Img_H) * (double)splitContainer_RIGHT2.Panel1.Height);
                        });
                    }
                    else
                    {
                        splitContainer_RIGHT2.SplitterDistance = (int)((Img_W / Img_H) * (double)splitContainer_RIGHT2.Panel1.Height);
                    }
                }
                splitContainer_RIGHT2.IsSplitterFixed = true;
                splitContainer_RIGHT.IsSplitterFixed = true;
                pictureBox_Image.Refresh();
                pictureBox_RImage.Refresh();
            }
            catch
            { }
        }

        private void pictureBox_Image_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Right")
            {
                if (pictureBox_Image.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어

                        string tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                        if (tstr == "AI 검사" || tstr == "AI Inspection")
                        {
                            cm.MenuItems.Add("ROI 이미지 저장", new EventHandler(ROI_Image_Save));
                        }

                        cm.MenuItems.Add("ROI 초기화", new EventHandler(PictureBoxROIInitialize));
                        if (!LVApp.Instance().m_Config.CP_table_check)
                        {
                            cm.MenuItems.Add("ROI 복사", new EventHandler(Copy_ROI));
                        }
                        else if (LVApp.Instance().m_Config.CP_table_check)
                        {
                            cm.MenuItems.Add("ROI 붙여넣기", new EventHandler(Paste_ROI));
                        }
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupViewOri));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveOri));
                        cm.MenuItems.Add("Gray 값 출력", new EventHandler(PictureBoxViewGray));

                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        string tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                        if (tstr == "AI 검사" || tstr == "AI Inspection")
                        {
                            cm.MenuItems.Add("ROI 이미지 저장", new EventHandler(ROI_Image_Save));
                        }

                        cm.MenuItems.Add("Initialize ROI", new EventHandler(PictureBoxROIInitialize));
                        if (!LVApp.Instance().m_Config.CP_table_check)
                        {
                            cm.MenuItems.Add("ROI Copy", new EventHandler(Copy_ROI));
                        }
                        else if (LVApp.Instance().m_Config.CP_table_check)
                        {
                            cm.MenuItems.Add("ROI Paste", new EventHandler(Paste_ROI));
                        }
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupViewOri));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveOri));
                        cm.MenuItems.Add("View Gray value", new EventHandler(PictureBoxViewGray));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        string tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                        if (tstr == "AI 검사" || tstr == "AI Inspection")
                        {
                            cm.MenuItems.Add("ROI 保存图像", new EventHandler(ROI_Image_Save));
                        }

                        cm.MenuItems.Add("ROI 初始化", new EventHandler(PictureBoxROIInitialize));
                        if (!LVApp.Instance().m_Config.CP_table_check)
                        {
                            cm.MenuItems.Add("ROI 复制", new EventHandler(Copy_ROI));
                        }
                        else if (LVApp.Instance().m_Config.CP_table_check)
                        {
                            cm.MenuItems.Add("ROI 粘贴", new EventHandler(Paste_ROI));
                        }
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupViewOri));
                        cm.MenuItems.Add("结果视图", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSaveOri));
                        cm.MenuItems.Add("查看灰色值", new EventHandler(PictureBoxViewGray));
                    }

                    pictureBox_Image.ContextMenu = cm;
                    pictureBox_Image.ContextMenu.Show(pictureBox_Image, e.Location);
                    pictureBox_Image.ContextMenu = null;
                }
                return;
            }
            else
            {
                //radioButton1.Checked = false;
                //radioButton2.Checked = true;
                //pictureBox_RImage.Refresh();
            }
        }

        private void Copy_ROI(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.CP_table_CamNum = Cam_Num;
            LVApp.Instance().m_Config.CP_table_Idx = listBox1.SelectedIndex;
            LVApp.Instance().m_Config.CP_table_check = true;
        }

        private void Paste_ROI(object sender, EventArgs e)
        {
            if (!LVApp.Instance().m_Config.CP_table_check)
            {
                return;
            }
            if (LVApp.Instance().m_Config.CP_table_Idx > 0 && listBox1.SelectedIndex > 0)
            {
                if (Cam_Num == 0)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
                else if (Cam_Num == 1)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                        //MessageBox.Show(LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1].ToString());
                        //MessageBox.Show(LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1].ToString());
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
                else if (Cam_Num == 2)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
                else if (Cam_Num == 3)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
            }
            else if (listBox1.SelectedIndex == 0 && LVApp.Instance().m_Config.CP_table_Idx == 0)
            {
                if (Cam_Num == 0)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
                else if (Cam_Num == 1)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
                else if (Cam_Num == 2)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
                else if (Cam_Num == 3)
                {
                    if (LVApp.Instance().m_Config.CP_table_CamNum == 0)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 1)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 2)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                    else if (LVApp.Instance().m_Config.CP_table_CamNum == 3)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.CP_table_Idx][1];
                    }
                }
            }
            MainDB_to_SubDB();
            dataGridView1.Refresh();
            //SubDB_to_MainDB();
            button_SAVE_Click(sender, e);
            LVApp.Instance().m_Config.CP_table_check = false;
        }

        private bool t_ViewGray = false;
        private void PictureBoxViewGray(object sender, EventArgs e)
        {
            if (t_ViewGray)
            {
                t_ViewGray = false;
            }
            else
            {
                t_ViewGray = true;
            }
        }

        private void PictureBoxROIInitialize(object sender, EventArgs e)
        {
            string str = "";
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                str = "ROI를 초기화 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                str = "Do you want to initialize the ROI?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                str = "是否要初始化 ROI?";
            }
             
            if (MessageBox.Show(str, " Initialize ROI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dataGridView1.Rows[1].Cells[1].Value = pictureBox_Image.Width / 2 - 200;
                dataGridView1.Rows[2].Cells[1].Value = pictureBox_Image.Height / 2 - 200;
                dataGridView1.Rows[3].Cells[1].Value = 400;
                dataGridView1.Rows[4].Cells[1].Value = 400;
                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    LVApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                radioButton1.Checked = true;
                radioButton2.Checked = false;
                pictureBox_Image.Refresh();
            }
        }

        private bool t_ViewGrayR = false;
        private void PictureBoxViewGrayR(object sender, EventArgs e)
        {
            if (t_ViewGrayR)
            {
                t_ViewGrayR = false;
            }
            else
            {
                t_ViewGrayR = true;
            }
        }

        private void pictureBox_Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!t_ViewGray)
            {
                return;
            }
            t_x = e.X;
            t_y = e.Y;
            if (pictureBox_Image.Image != null)
            {
                // Declare a Bitmap
                //Bitmap mybitmap;
                // Load Picturebox image to bitmap
                //mybitmap = new Bitmap(pictureBox_Image.Image);
                // In the mouse move event
                int tt_x = (int)((double)t_x * (double)pictureBox_Image.Image.Width / (double)pictureBox_Image.Width);
                int tt_y = (int)((double)t_y * (double)pictureBox_Image.Image.Height / (double)pictureBox_Image.Height);
                if (tt_x >= 0 && tt_x < pictureBox_Image.Image.Width && tt_y >= 0 && tt_y < pictureBox_Image.Image.Height)
                {
                    var pixelcolor = ((Bitmap)pictureBox_Image.Image).GetPixel(tt_x, tt_y);
                    // Displays R  / G / B Color
                    t_xyg = "[G" + pixelcolor.R.ToString("000") + "(X" + tt_x.ToString() + ",Y" + tt_y.ToString() + ")]";
                    pictureBox_Image.Refresh();
                }
            }
        }

        private string t_xyg; private int t_x = 0; private int t_y = 0;
        public string t_AI_Result = string.Empty;
        private void pictureBox_Image_Paint(object sender, PaintEventArgs e)
        {
            if (t_ViewGray)
            {
                using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(125, 125, 125, 125)))
                {
                    if (t_x + 10 + 115 > pictureBox_Image.Width)
                    {
                        e.Graphics.FillRectangle(myBrush, new Rectangle(t_x - 125, t_y - 10, 115, 21));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(myBrush, new Rectangle(t_x + 10, t_y - 10, 115, 21));
                    }
                }

                using (Font myFont = new Font("Arial", 9))
                {
                    if (t_x + 10 + 115 > pictureBox_Image.Width)
                    {
                        e.Graphics.DrawString(t_xyg, myFont, Brushes.Yellow, new System.Drawing.Point(t_x - 122, t_y - 8));
                    }
                    else
                    {
                        e.Graphics.DrawString(t_xyg, myFont, Brushes.Yellow, new System.Drawing.Point(t_x + 13, t_y - 8));
                    }
                }

                Pen pen1 = new Pen(Color.FromArgb(125, 0, 255, 0), 1);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                System.Drawing.Point VP1 = new System.Drawing.Point(t_x - 9, t_y);
                System.Drawing.Point VP2 = new System.Drawing.Point(t_x + 9, t_y);
                e.Graphics.DrawLine(pen1, VP1, VP2);
                System.Drawing.Point HP1 = new System.Drawing.Point(t_x, t_y - 9);
                System.Drawing.Point HP2 = new System.Drawing.Point(t_x, t_y + 9);
                e.Graphics.DrawLine(pen1, HP1, HP2);
                pen1.Dispose();
            }

            //if (t_AI_Result.Length > 0)
            //{
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(t_AI_Result, myFont, Brushes.Yellow, new System.Drawing.Point(10, pictureBox_Image.Height - 25));
            //    }
            //}
        }


        void MainDB_to_SubDB()
        {
            try
            {
                int t_idx = 0;
                if (listBox1.SelectedIndex < 0)
                {
                    //return;
                }
                else
                {
                    t_idx = listBox1.SelectedIndex;
                }
                LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_idx;

                if (Cam_Num == 0 && LVApp.Instance().m_Config.Cam0_rect[0] != null)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.Cam0_rect.Length; i++)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[i].mView = false;
                    }

                    string[] str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[4][1].ToString())
                        );
                    LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 1 && LVApp.Instance().m_Config.Cam1_rect[0] != null)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.Cam1_rect.Length; i++)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[i].mView = false;
                    }
                    string[] str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[4][1].ToString())
                        );
                    LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 2 && LVApp.Instance().m_Config.Cam2_rect[0] != null)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.Cam2_rect.Length; i++)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[i].mView = false;
                    }
                    string[] str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[4][1].ToString())
                        );
                    LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 3 && LVApp.Instance().m_Config.Cam3_rect[0] != null)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.Cam3_rect.Length; i++)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[i].mView = false;
                    }
                    string[] str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[4][1].ToString())
                        );
                    LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }

                //int t_Vx = Convert.ToInt32(dataGridView1.Rows[6].Cells[1].Value);
                //if (t_Vx != ctr_ROI_Guide1.trackBar_V1.Value)
                //{
                //    ctr_ROI_Guide1.trackBar_V1.Value = t_Vx;
                //    ctr_ROI_Guide1.textBox_Offset.Text = t_Vx.ToString();
                //}
                //t_Vx = Convert.ToInt32(dataGridView1.Rows[7].Cells[1].Value);
                //if (t_Vx != ctr_ROI_Guide1.trackBar_V2.Value)
                //{
                //    ctr_ROI_Guide1.trackBar_V2.Value = t_Vx;
                //    ctr_ROI_Guide1.textBox_Max.Text = t_Vx.ToString();
                //}

                dataGridView1.Refresh();
                pictureBox_Image.Refresh();
            }
            catch
            {
            }
        }

        public void SubDB_to_MainDB()
        {
            try
            {
                if (listBox1 == null)
                {
                    return;
                }
                if (listBox1.SelectedIndex < 0)
                {
                    return;
                }
                LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1.SelectedIndex;

                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = LVApp.Instance().m_Config.Cam0_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[14][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[25][1].ToString();
                    //MessageBox.Show(LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1].ToString());
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = LVApp.Instance().m_Config.Cam1_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[14][1].ToString()
                                            + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[25][1].ToString();
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = LVApp.Instance().m_Config.Cam2_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[14][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[25][1].ToString();
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = LVApp.Instance().m_Config.Cam3_rect[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[14][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[25][1].ToString();
                }
            }
            catch
            {
            }
        }


        private void button_ROTATION_CAL_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            LVApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1.SelectedIndex;
            SubDB_to_MainDB();

            LVApp.Instance().m_Config.Set_Parameters();

            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1.SelectedIndex, Cam_Num);

            Bitmap t_Image = (Bitmap)pictureBox_Image.Image.Clone();

            if (t_Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                byte[] arr = BmpToArray(grayImage);
                //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);

                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                grayImage.Dispose();
            }
            else
            {
                byte[] arr = BmpToArray(t_Image);
                //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);

                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
            }
            t_Image.Dispose();

            //if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 1 && comboBox_TABLETYPE.SelectedIndex == 0)
            //{// 인덱스 타입 일때
                //if (LVApp.Instance().m_Config.Alg_TextView)
                //{
                //    LVApp.Instance().m_Config.Alg_TextView = false;
                //    LVApp.Instance().m_Config.Set_Parameters();
                //    button_INSPECTION_Click(sender, e);
                //    //Thread.Sleep(200);
                //    LVApp.Instance().m_Config.Alg_TextView = true;
                //    LVApp.Instance().m_Config.Set_Parameters();
                //    LVApp.Instance().m_Config.Alg_TextView = true;
                //}
                //else
                //{
                //    LVApp.Instance().m_Config.Set_Parameters();
                //    button_INSPECTION_Click(sender, e);
                //}
            //}
            //Thread.Sleep(100);
            byte[] Dst_Img = null;
            int width = 0, height = 0, ch = 0;

            LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image_Gray(out Dst_Img, out width, out height, out ch, Cam_Num);

            //if (LVApp.Instance().m_mainform.m_ImProClr_Class0.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
            {
                pictureBox_Image.Image = ConvertBitmap(Dst_Img, width, height, ch);
                pictureBox_Image.Refresh();
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
        }

        public void Advenced_Parameter(object sender, EventArgs e)
        {
            if (m_advenced_param_visible)
            {
                m_advenced_param_visible = false;
                listBox1.Top = 6;
                listBox1.Height = 148;
            }
            else
            {
                if (!LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag)
                {
                    Frm_Password m_Frm_Password = new Frm_Password();
                    m_Frm_Password.ShowDialog();

                    if (!LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag)
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            AutoClosingMessageBox.Show("고급 관리자로 로그인 하세요!", "Caution", 2000);
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            AutoClosingMessageBox.Show("Please login for admin!", "Caution", 2000);
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            AutoClosingMessageBox.Show("以管理员身份登录!", "Caution", 2000);
                        }
                        return;
                    }
                }

                m_advenced_param_visible = true;
                //listBox1.Top = 54;
                //listBox1.Height = 100;
                listBox1.Top = 6;
                listBox1.Height = 148;
            }

            if (LVApp.Instance().m_mainform.ctr_Admin_Param1.checkBox_ADMINMODE.Checked != m_advenced_param_visible)
            {
                LVApp.Instance().m_mainform.ctr_Admin_Param1.checkBox_ADMINMODE.Checked = m_advenced_param_visible;
            }

            Referesh_Select_Menu(true);
        }

        public void View_Help(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Help\\Setting");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }

            string[] t_str_help = str_help.Split('(');
            if (t_str_help.Length > 0)
            {
                str_help = t_str_help[0];
            }
            FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Help\\Setting\\" + str_help + ".html");
            if (!newFile.Exists)
            {
                return;
            }
            System.Diagnostics.Process.Start(newFile.FullName);
        }

        public void Change_Initial_Parameter(object sender, EventArgs e)
        {
            string popup_str = "";
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                popup_str = "고급 변수를 초기화 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                popup_str = "Do you want to initialize the parameters?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                popup_str = "是否要初始化参数?";
            }

            if (MessageBox.Show(popup_str, " INITIALIZE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

            }
            else
            {
                return;
            }

            string tstr = "";

            if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 1)
            { // 사이드이면 
                if (listBox1.SelectedIndex == 0)
                {
                    //dgvCmbCell8.Items.Add("좌상 기준");
                    ////if (!tstr.Contains("기준"))
                    //{
                    //    tstr = "좌상 기준";
                    //}

                    //if (comboBox_TABLETYPE.SelectedIndex == 0)
                    //{// 인덱스 타입 일때
                    //}
                }
            }

            // 8번 알고리즘 선택
            tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
            for (int i = 12; i <= 25; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "备用变量" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
                {
                    dataGridView1.Rows[i].Cells[1].Value = "0";
                }
            }
            if (tstr == "좌측 끝 기준" || tstr == "우측 끝 기준" || tstr == "좌상 기준" || tstr == "좌하 기준" || tstr == "우상 기준" || tstr == "우하 기준" || tstr == "중심 기준" || tstr == "상부 중심 기준"
                || tstr == "Left end" || tstr == "Right end" || tstr == "Left top" || tstr == "Left bottom" || tstr == "Right top" || tstr == "Right bottom" || tstr == "Center"
                )
            {
                if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0) // 상부, 하부일때
                {
                    if (dataGridView1.Rows[5].Cells[1].Value.ToString() == "모델 사용" || dataGridView1.Rows[5].Cells[1].Value.ToString() == "Model find")
                    {
                        if (LVApp.Instance().m_mainform.m_ImProClr_Class.EasyFind_Check())
                        { // 유레시스
                            dataGridView1.Rows[12].Cells[1].Value = "360";
                            dataGridView1.Rows[13].Cells[1].Value = "5";
                            dataGridView1.Rows[14].Cells[1].Value = "100";
                            dataGridView1.Rows[15].Cells[1].Value = "0";
                            dataGridView1.Rows[16].Cells[1].Value = "0";
                            dataGridView1.Rows[17].Cells[1].Value = "0";
                            dataGridView1.Rows[18].Cells[1].Value = "0";
                            dataGridView1.Rows[19].Cells[1].Value = "0";
                        }
                        else
                        { // CDJung Find Tool
                            dataGridView1.Rows[12].Cells[1].Value = "2";
                            dataGridView1.Rows[13].Cells[1].Value = "0";
                            dataGridView1.Rows[14].Cells[1].Value = "0";
                            dataGridView1.Rows[15].Cells[1].Value = "1";
                            dataGridView1.Rows[16].Cells[1].Value = "0";
                            dataGridView1.Rows[17].Cells[1].Value = "0";
                            dataGridView1.Rows[18].Cells[1].Value = "0";
                            dataGridView1.Rows[19].Cells[1].Value = "0";
                        }
                    }
                    else
                    {
                        dataGridView1.Rows[12].Cells[1].Value = "1";
                        dataGridView1.Rows[13].Cells[1].Value = "0";
                        dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                        dataGridView1.Rows[15].Cells[1].Value = "0";
                        dataGridView1.Rows[16].Cells[1].Value = "0";
                        dataGridView1.Rows[17].Cells[1].Value = "1";
                        dataGridView1.Rows[18].Cells[1].Value = "0";
                        dataGridView1.Rows[19].Cells[1].Value = "0";
                        dataGridView1.Rows[20].Cells[1].Value = "0";
                    }
                }
                else
                {
                    if (comboBox_TABLETYPE.SelectedIndex == 0)
                    {// 인덱스 타입 일때
                        dataGridView1.Rows[15].Cells[1].Value = "1"; // 이름
                        dataGridView1.Rows[16].Cells[1].Value = "2"; // 이름
                        dataGridView1.Rows[17].Cells[1].Value = "0"; // 이름
                    }
                    else if (comboBox_TABLETYPE.SelectedIndex == 1)
                    {// 글라스 타입 일때
                        //dataGridView1.Rows[12].Cells[1].Value = "1"; // 이름
                        //dataGridView1.Rows[13].Cells[1].Value = "10"; // 이름
                        //dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                        dataGridView1.Rows[15].Cells[1].Value = "20"; // 이름
                        dataGridView1.Rows[16].Cells[1].Value = "10"; // 이름
                        dataGridView1.Rows[17].Cells[1].Value = "0"; // 이름
                    }
                    else if (comboBox_TABLETYPE.SelectedIndex == 2)
                    {// 벨트 타입 일때
                        dataGridView1.Rows[12].Cells[1].Value = "10"; // 이름
                        //dataGridView1.Rows[13].Cells[1].Value = "10"; // 이름
                        //dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                    }
                    else if (comboBox_TABLETYPE.SelectedIndex == 3)
                    {// 가이드 없을때
                        dataGridView1.Rows[13].Cells[1].Value = "1"; // 이름
                        dataGridView1.Rows[14].Cells[1].Value = "1"; // 이름
                        dataGridView1.Rows[15].Cells[1].Value = "1"; // 이름
                    }
                }
            }
            else if (tstr == "가로 길이" || tstr == "Hor. length" || tstr == "세로 길이" || tstr == "Ver. length")
            {
                dataGridView1.Rows[10].Cells[1].Value = "20";
                dataGridView1.Rows[11].Cells[1].Value = "80";
                dataGridView1.Rows[12].Cells[1].Value = "0";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "100";
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "0";
            }
            else if (tstr == "십자 치수" || tstr == "Dim. of cross")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "4";
                dataGridView1.Rows[14].Cells[1].Value = "0";
            }
            else if (tstr == "직경" || tstr == "Diameter")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "100";
                dataGridView1.Rows[15].Cells[1].Value = "0";
            }
            else if (tstr == "AI 검사" || tstr == "AI Inspection")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "0.55";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
                dataGridView1.Rows[15].Cells[1].Value = "0";
            }
            else if (tstr == "SSF")
            {
                dataGridView1.Rows[12].Cells[1].Value = "100";
                dataGridView1.Rows[13].Cells[1].Value = "100";
                dataGridView1.Rows[14].Cells[1].Value = "5"; // S 필터 가로
                dataGridView1.Rows[15].Cells[1].Value = "5"; // S 필터 세로
                dataGridView1.Rows[16].Cells[1].Value = "65";// B 필터 가로
                dataGridView1.Rows[17].Cells[1].Value = "65";// B 필터 세로
                dataGridView1.Rows[18].Cells[1].Value = "0";
                dataGridView1.Rows[19].Cells[1].Value = "0";
                dataGridView1.Rows[20].Cells[1].Value = "30";
                dataGridView1.Rows[21].Cells[1].Value = "30";
                dataGridView1.Rows[22].Cells[1].Value = "10";
                dataGridView1.Rows[23].Cells[1].Value = "10";
                dataGridView1.Rows[24].Cells[1].Value = "0";
            }
            else if (tstr == "사각 영역의 밝기" || tstr == "Brightness of rectangle ROI")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "0";
				dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "0";
                dataGridView1.Rows[18].Cells[1].Value = "0";
                dataGridView1.Rows[19].Cells[1].Value = "0";
            }
            else if (tstr == "원형 영역의 밝기" || tstr == "Brightness of circle ROI")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
            }
            else if (tstr == "사각 영역의 BLOB" || tstr == "BLOB in rectangle ROI")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "0";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "10";
                dataGridView1.Rows[18].Cells[1].Value = "50";
                dataGridView1.Rows[19].Cells[1].Value = "50";
                dataGridView1.Rows[20].Cells[1].Value = "1";
                dataGridView1.Rows[21].Cells[1].Value = "1";
                dataGridView1.Rows[22].Cells[1].Value = "0";
                dataGridView1.Rows[23].Cells[1].Value = "0";
                dataGridView1.Rows[24].Cells[1].Value = "0";
            }
            else if (tstr == "면취 측정" || tstr == "Bevelling Measurement")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "0";
                dataGridView1.Rows[18].Cells[1].Value = "0";
                dataGridView1.Rows[19].Cells[1].Value = "0";
                dataGridView1.Rows[20].Cells[1].Value = "0";
                dataGridView1.Rows[21].Cells[1].Value = "0";
                dataGridView1.Rows[22].Cells[1].Value = "0";
                dataGridView1.Rows[23].Cells[1].Value = "0";
                dataGridView1.Rows[24].Cells[1].Value = "0";
            }
            else if (tstr == "원형 영역의 BLOB" || tstr == "BLOB in circle ROI")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
                dataGridView1.Rows[15].Cells[1].Value = "360";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "0";
                dataGridView1.Rows[18].Cells[1].Value = "0";
                dataGridView1.Rows[19].Cells[1].Value = "0";
                dataGridView1.Rows[20].Cells[1].Value = "0";
                dataGridView1.Rows[21].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                dataGridView1.Rows[22].Cells[1].Value = "0";
                dataGridView1.Rows[23].Cells[1].Value = "100";
                dataGridView1.Rows[24].Cells[1].Value = "0";
                dataGridView1.Rows[25].Cells[1].Value = "0";
            }
            else if (tstr == "진원도(%)" || tstr == "Circularity(%)")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "100";
            }
            else if (tstr == "나사산 피치" || tstr == "Pitch of thread")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
            }
            else if (tstr == "두 영역 중심간 거리" || tstr == "Distance between two area")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "0";
                dataGridView1.Rows[13].Cells[1].Value = "100";
                dataGridView1.Rows[14].Cells[1].Value = "100";
                dataGridView1.Rows[15].Cells[1].Value = "100";
                dataGridView1.Rows[16].Cells[1].Value = "100";
                dataGridView1.Rows[17].Cells[1].Value = "300";
                dataGridView1.Rows[18].Cells[1].Value = "100";
                dataGridView1.Rows[19].Cells[1].Value = "100";
                dataGridView1.Rows[20].Cells[1].Value = "100";
            }
            else if (tstr == "나사산 크기" || tstr == "Size of thread")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "1";
                dataGridView1.Rows[14].Cells[1].Value = "0";
            }
            else if (tstr == "원형 영역의 색상 BLOB" || tstr == "Color BLOB in circle ROI")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "1";
                dataGridView1.Rows[15].Cells[1].Value = "90";
                dataGridView1.Rows[16].Cells[1].Value = "100";
                dataGridView1.Rows[17].Cells[1].Value = "0";
                dataGridView1.Rows[18].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                dataGridView1.Rows[19].Cells[1].Value = "0";
                dataGridView1.Rows[20].Cells[1].Value = "50";
                dataGridView1.Rows[21].Cells[1].Value = "360";
            }
            else if (tstr == "내외경 중심 차이" || tstr == "Center difference between Inner and outter circle")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "200";
                dataGridView1.Rows[13].Cells[1].Value = "1";
                dataGridView1.Rows[14].Cells[1].Value = "1";
                dataGridView1.Rows[15].Cells[1].Value = "1";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "1";
            }
            else if (tstr == "머리 나사부 동심도" || tstr == "Concentricity")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
            }
            else if (tstr == "하부 V 각도" || tstr == "V Angle of bottom")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "5";
                dataGridView1.Rows[13].Cells[1].Value = "10";
            }
            else if (tstr == "리드각(1)" || tstr == "Lead angle of thread(1)" || tstr == "리드각(0.5)" || tstr == "Lead angle of thread(0.5)")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
            }
            else if (tstr == "몸통 두께" || tstr == "Thickness of body(mm)" || tstr == "몸통 휨" || tstr == "Bending of body(mm)")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
            }
            else if (tstr == "하부 형상" || tstr == "Shape of bottom")
            {
                dataGridView1.Rows[10].Cells[1].Value = "0";
                dataGridView1.Rows[11].Cells[1].Value = "100";
                dataGridView1.Rows[12].Cells[1].Value = "5";
                dataGridView1.Rows[13].Cells[1].Value = "5";
            }
        }

        public void Referesh_Select_Menu(bool text_maintain)
        {
            try
            {
                if (LVApp.Instance().m_mainform.Force_close)
                {
                    return;
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    // -------------------------------------------------------------------------
                    // 0번 사용 유무
                    string tstr = dataGridView1.Rows[0].Cells[1].Value.ToString();
                    dataGridView1.Rows[0].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell = new DataGridViewComboBoxCell();
                    dgvCmbCell.Items.Add("O");
                    dgvCmbCell.Items.Add("X");
                    dataGridView1.Rows[0].Cells[1] = dgvCmbCell;
                    if (!tstr.Contains("O") && !tstr.Contains("X"))
                    {
                        tstr = "X";
                    }
                    if (tstr != "" && text_maintain)
                    {
                        dataGridView1.Rows[0].Cells[1].Value = tstr;
                    }

                    // -------------------------------------------------------------------------
                    // 1~4번 ROI 정보
                    // -------------------------------------------------------------------------
                    // 5번 임계화
                    tstr = dataGridView1.Rows[5].Cells[1].Value.ToString();
                    dataGridView1.Rows[5].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell5 = new DataGridViewComboBoxCell();

                    dgvCmbCell5.Items.Add("v1 이하");
                    dgvCmbCell5.Items.Add("v2 이상");
                    dgvCmbCell5.Items.Add("v1~v2 사이");
                    dgvCmbCell5.Items.Add("v1이하v2이상");
                    dgvCmbCell5.Items.Add("자동이하");
                    dgvCmbCell5.Items.Add("자동이상");
                    dgvCmbCell5.Items.Add("에지");
                    dgvCmbCell5.Items.Add("평균기준 차이");

                    string main_str = "";
                    if (Cam_Num == 0)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();
                    }
                    else if (Cam_Num == 1)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();
                    }
                    else if (Cam_Num == 2)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();
                    }
                    else if (Cam_Num == 3)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();
                    }
                    if (listBox1_SelectedIndex != 0)
                    {
                        dgvCmbCell5.Items.Add("비교v1이하v2이상");
                    }

                    if (listBox1.SelectedIndex == 0)
                    {
                        if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] != 1)
                        {
                            dgvCmbCell5.Items.Add("모델 사용");
                        }
                        //if (tstr == "검사 영역 결과 사용")
                        //{
                        //    tstr = "v1 이하";
                        //    dataGridView1.Rows[5].Cells[1].Value = "v1 이하";
                        //}
                    }
                    else
                    {
                        if (!main_str.Contains("모델 사용"))
                        {
                            dgvCmbCell5.Items.Add("검사 영역 결과 사용");
                            if (tstr == "비교v1이하v2이상")
                            {
                                tstr = "v1 이하";
                            }
                        }
                        else
                        {
                            //if (tstr == "검사 영역 결과 사용" || tstr == "모델 사용")
                            //{
                            //    tstr = "v1 이하";
                            //}
                        }
                    }
                    dataGridView1.Rows[5].Cells[1] = dgvCmbCell5;
                    if (!tstr.Contains("v1 이하") && !tstr.Contains("v2 이상") && !tstr.Contains("v1~v2 사이")
                         && !tstr.Contains("자동이하") && !tstr.Contains("자동이상") && !tstr.Contains("에지")
                         && !tstr.Contains("모델 사용") && !tstr.Contains("검사 영역 결과 사용") && !tstr.Contains("v1이하v2이상") && !tstr.Contains("평균기준 차이") && !tstr.Contains("비교v1이하v2이상"))
                    {
                        tstr = "v1 이하";
                    }
                    if (tstr != "" && text_maintain)
                    {
                        if (!main_str.Contains("모델 사용") && tstr.Contains("비교v1이하v2이상"))
                        {
                            dataGridView1.Rows[5].Cells[1].Value = "v1 이하";
                        }
                        else
                        {
                            dataGridView1.Rows[5].Cells[1].Value = tstr;
                        }
                    }

                    if (tstr == "v1 이하" || tstr == "v1 less than")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "v2 이상" || tstr == "v2 more than")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "v1~v2 사이" || tstr == "v1~v2")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "v1이하v2이상" || tstr == "less v1 more v2")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "자동이하" || tstr == "Auto less than")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "자동이상" || tstr == "Auto more than")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "에지" || tstr == "Edge")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "검사 영역 결과 사용" || tstr == "모델 사용" || tstr == "Insp. area result use" || tstr == "Model find")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "평균기준 차이" || tstr == "Diff. from AVG")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "비교v1이하v2이상" || tstr == "Compare less v1 more v2")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    // -------------------------------------------------------------------------
                    // 6,7번 임계화 하한, 상한값
                    // -------------------------------------------------------------------------
                    // 8번 알고리즘 선택
                    tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                    dataGridView1.Rows[8].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell8 = new DataGridViewComboBoxCell();

                    for (int i = 12; i <= 25; i++)
                    {
                        dataGridView1.Rows[i].Cells[0].Value = "예비변수";
                        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                        //dataGridView1.Rows[i].Cells[1].Value = "0";
                    }

                    if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0)
                    { // 상부 또는 하부이면 
                        if (listBox1.SelectedIndex == 0)
                        {
                            dgvCmbCell8.Items.Add("좌측 끝 기준");
                            dgvCmbCell8.Items.Add("우측 끝 기준");
                            dgvCmbCell8.Items.Add("좌상 기준");
                            dgvCmbCell8.Items.Add("좌하 기준");
                            dgvCmbCell8.Items.Add("우상 기준");
                            dgvCmbCell8.Items.Add("우하 기준");
                            dgvCmbCell8.Items.Add("중심 기준");
                            dgvCmbCell8.Items.Add("상부 중심 기준");
                            if (!tstr.Contains("기준"))
                            {
                                tstr = "중심 기준";
                            }

                            if (dataGridView1.Rows[5].Cells[1].Value.ToString() == "모델 사용")
                            {
                                tstr = "중심 기준";
                                if (LVApp.Instance().m_mainform.m_ImProClr_Class.EasyFind_Check())
                                {
                                    // 변수 추가
                                    dataGridView1.Rows[12].Cells[0].Value = "각도 Tolerance(Degree)";
                                    // 변수 추가
                                    dataGridView1.Rows[13].Cells[0].Value = "스케일 Tolerance(%)";
                                    // 변수 추가
                                    dataGridView1.Rows[14].Cells[0].Value = "검사영역 확장(Pixel)";
                                    dataGridView1.Rows[15].Cells[0].Value = "밝기 밸런스([-1 1])";
                                    dataGridView1.Rows[16].Cells[0].Value = "예비 변수";
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) >= 360)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 360;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[13].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 100)
                                    {
                                        dataGridView1.Rows[13].Cells[1].Value = 100;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[14].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) >= Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2)
                                    {
                                        dataGridView1.Rows[14].Cells[1].Value = Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2;
                                    }
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = -1;
                                    }
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 1)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 1000)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 1000;
                                    }
                                }
                                else
                                {
                                    // 변수 추가
                                    dataGridView1.Rows[12].Cells[0].Value = "축소 Scale(1,2,4)";
                                    // 변수 추가
                                    dataGridView1.Rows[13].Cells[0].Value = "상부 Margin(Pixel)";
                                    // 변수 추가
                                    dataGridView1.Rows[14].Cells[0].Value = "하부 Margin(Pixel)";
                                    dataGridView1.Rows[15].Cells[0].Value = "에지([0 1])";
                                    dataGridView1.Rows[16].Cells[0].Value = "회전 스코어(%)";
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= 1)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) >= 4)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 4;
                                    }
                                    //if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                                    //{
                                    //    dataGridView1.Rows[13].Cells[1].Value = 0;
                                    //}
                                    //if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 100)
                                    //{
                                    //    dataGridView1.Rows[13].Cells[1].Value = 100;
                                    //}
                                    //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                                    //{
                                    //    dataGridView1.Rows[14].Cells[1].Value = 0;
                                    //}
                                    //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) >= Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2)
                                    //{
                                    //    dataGridView1.Rows[14].Cells[1].Value = Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2;
                                    //}
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = -1;
                                    }
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 100)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 100;
                                    }
                                }

                                dataGridView1.Rows[17].Cells[0].Value = "회전 옵션(0:None,1:W_Ribbon,2:B_Ribbon,3:판정NG시 상하반전)";
                                if (Convert.ToDouble(dataGridView1.Rows[17].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 0;
                                }
                                if (Convert.ToDouble(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 3)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 3;
                                }
                            }
                            else
                            {
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    if (comboBox_TABLETYPE.SelectedIndex != 5)
                                    {
                                        dataGridView1.Rows[14].Cells[1].Value = 100000;
                                    }
                                }
                                dataGridView1.Rows[15].Cells[0].Value = "노이즈 제거 필터 방향(0:전방향,1:가로,2:세로)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                                {
                                    if (comboBox_TABLETYPE.SelectedIndex != 5)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = 0;
                                    }
                                }
                                dataGridView1.Rows[16].Cells[0].Value = "각도 보정(0:안함,1:좌측,2:우측,3:상부,4:하부,5:좌우중심,6:상하중심,7:원 전처리)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 7 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                                {
                                    if (comboBox_TABLETYPE.SelectedIndex != 5)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 0;
                                    }
                                }
                                // 변수 추가
                                dataGridView1.Rows[17].Cells[0].Value = "BLOB 합침 필터 크기"; // 이름
                                dataGridView1.Rows[18].Cells[0].Value = "BLOB 선택(0:가장큰,1:상,2:하,3:좌,4:우)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[18].Cells[1].Value = 0;
                                }
                                else if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) >= 4)
                                {
                                    dataGridView1.Rows[18].Cells[1].Value = 4;
                                }
                                dataGridView1.Rows[19].Cells[0].Value = "Blur 필터";
                                if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[19].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) >= 100)
                                {
                                    dataGridView1.Rows[19].Cells[1].Value = 100;
                                }

                                dataGridView1.Rows[20].Cells[0].Value = "Color 전처리(0:None,1:Use)";
                                if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[20].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) >= 1)
                                {
                                    dataGridView1.Rows[20].Cells[1].Value = 1;
                                }
                            }
                        }
                        else
                        {
                            bool t_exist_check = false; string t_exist_str = "";
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[0, Cam_Num])
                            {
                                t_exist_str = "가로 길이";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[1, Cam_Num])
                            {
                                t_exist_str = "세로 길이";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[2, Cam_Num])
                            {
                                t_exist_str = "원형 영역의 밝기";
                                dgvCmbCell8.Items.Add(t_exist_str);//5
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[3, Cam_Num])
                            {
                                t_exist_str = "원형 영역의 BLOB";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[4, Cam_Num])
                            {
                                t_exist_str = "사각 영역의 밝기";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[5, Cam_Num])
                            {
                                t_exist_str = "사각 영역의 BLOB";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            // dgvCmbCell8.Items.Add("사각 ROI의 BLOB 갯수(Count)");
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[6, Cam_Num])
                            {
                                t_exist_str = "직경";
                                dgvCmbCell8.Items.Add(t_exist_str);//3
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[7, Cam_Num])
                            {
                                t_exist_str = "진원도(%)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[8, Cam_Num])
                            {
                                t_exist_str = "십자 치수";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[9, Cam_Num])
                            {
                                t_exist_str = "두 영역 중심간 거리";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[10, Cam_Num])
                            {
                                t_exist_str = "나사산 피치";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[11, Cam_Num])
                            {
                                t_exist_str = "나사산 크기";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[12, Cam_Num])
                            {
                                t_exist_str = "원형 영역의 색상 BLOB";
                                dgvCmbCell8.Items.Add(t_exist_str);//14
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            //dgvCmbCell8.Items.Add("볼록 BLOB 차이");//15
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[13, Cam_Num])
                            {
                                t_exist_str = "내외경 중심 차이";
                                dgvCmbCell8.Items.Add(t_exist_str);//16
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[14, Cam_Num])
                            {
                                t_exist_str = "면취 측정";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[15, Cam_Num])
                            {
                                t_exist_str = "AI 검사";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[16, Cam_Num])
                            {
                                t_exist_str = "SSF";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[17, Cam_Num])
                            {
                                if (main_str.Contains("모델 사용"))
                                {
                                    t_exist_str = "일치율(%)";
                                    dgvCmbCell8.Items.Add(t_exist_str);
                                    if (tstr == t_exist_str)
                                    {
                                        t_exist_check = true;
                                    }
                                }
                                if (tstr.Contains("원형 영역의 BLOB 크기") || tstr.Contains("원형 영역의 BLOB 갯수"))
                                {
                                    tstr = "원형 영역의 BLOB";
                                    if (tstr == "원형 영역의 BLOB")
                                    {
                                        t_exist_check = true;
                                    }
                                }
                            }

                            if (!t_exist_check)
                            {
                                tstr = t_exist_str;
                            }
                            //dgvCmbCell8.Items.Add("유사도(%)");
                            if (!tstr.Contains("가로 길이") && !tstr.Contains("세로 길이")
                                && !tstr.Contains("십자 치수") && !tstr.Contains("직경")
                                && !tstr.Contains("사각 영역의 밝기") && !tstr.Contains("원형 영역의 밝기")
                                && !tstr.Contains("사각 영역의 BLOB") && !tstr.Contains("사각 ROI의 BLOB 갯수(Count)")
                                //&& !tstr.Contains("원형 영역의 BLOB 크기") && !tstr.Contains("원형 영역의 BLOB 갯수")
                                && !tstr.Contains("원형 영역의 BLOB")
                                && !tstr.Contains("진원도(%)") && !tstr.Contains("유사도(%)") && !tstr.Contains("나사산 피치")
                                && !tstr.Contains("두 영역 중심간 거리") && !tstr.Contains("나사산 크기")
                                && !tstr.Contains("원형 영역의 색상 BLOB")
                                && !tstr.Contains("면취 측정") && !tstr.Contains("AI 검사") && !tstr.Contains("SSF")
                                && !tstr.Contains("내외경 중심 차이") && !tstr.Contains("일치율(%)") && tstr.Length != 0)
                            {
                                tstr = "가로 길이";
                            }
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 1)
                    { // 사이드이면 
                        if (listBox1.SelectedIndex == 0)
                        {
                            //if (comboBox_TABLETYPE.SelectedIndex == 0)
                            //{
                            //    //dataGridView1.Rows[8].Cells[1].Value = "중심 기준";
                            //    dgvCmbCell8.Items.Add("중심 기준");
                            //}
                            //else
                            //{
                                dgvCmbCell8.Items.Add("좌측 끝 기준");
                                dgvCmbCell8.Items.Add("우측 끝 기준");
                                dgvCmbCell8.Items.Add("좌상 기준");
                                dgvCmbCell8.Items.Add("좌하 기준");
                                dgvCmbCell8.Items.Add("우상 기준");
                                dgvCmbCell8.Items.Add("우하 기준");
                                dgvCmbCell8.Items.Add("중심 기준");
                                dgvCmbCell8.Items.Add("상부 중심 기준");
                                if (!tstr.Contains("기준"))
                                {
                                    //dataGridView1.Rows[8].Cells[1].Value = "중심 기준";
                                    tstr = "중심 기준";
                                }
                            //}

                            if (comboBox_TABLETYPE.SelectedIndex == 0)
                            {// 인덱스 타입 일때
                                //dataGridView1.Rows[12].Cells[0].Value = "머리부 가로 길이(mm)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[13].Cells[0].Value = "회전 계산 TOP 위치(mm)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[14].Cells[0].Value = "회전 계산 높이(mm)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[15].Cells[0].Value = "회전 나사선 제거 필터 크기"; // 이름
                                dataGridView1.Rows[12].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[15].Cells[0].Value = "회전 나사선 제거 필터 크기"; // 이름
                                dataGridView1.Rows[16].Cells[0].Value = "회전 기준(0:상하부,1:하부,2:Index판)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[16].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 2)
                                {
                                    dataGridView1.Rows[16].Cells[1].Value = 2;
                                }
                                dataGridView1.Rows[17].Cells[0].Value = "특수 회전(0:없음,1:상부 짧게,2:상부 길게,3:상부 앏음, 4:상부 굵음)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 4)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 4;
                                }
                                //if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 0 && Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 2)
                                //{
                                //    dataGridView1.Rows[18].Cells[0].Value = "상부 임계길이(mm)"; // 이름
                                //    if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) < 0)
                                //    {
                                //        dataGridView1.Rows[18].Cells[1].Value = 0;
                                //    }
                                //}
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 1)
                            {// 글라스 타입 일때
                                // 변수 추가
                                //dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)"; // 이름
                                dataGridView1.Rows[12].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "예비변수"; // 이름
                                //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                //{
                                //    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                //}

                                dataGridView1.Rows[15].Cells[0].Value = "회전 계산 나사몸통 높이(mm)"; // 이름
                                dataGridView1.Rows[16].Cells[0].Value = "회전 나사선 제거 필터 크기"; // 이름
                                dataGridView1.Rows[17].Cells[0].Value = "회전 기준(0:없음,1:나사몸통,2:유리판)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 2)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 2;
                                }
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 2)
                            {// 벨트 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "가이드 선 두께"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "예비변수"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)"; // 이름
                                //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                //{
                                //    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                //}
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 3)
                            {// 가이드 없을때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "예비변수"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "회전 계산 TOP 위치(mm)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "회전 계산 높이(mm)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[15].Cells[0].Value = "회전 나사선 제거 필터 크기"; // 이름
                            }
                        }
                        else
                        {
                            bool t_exist_check = false; string t_exist_str = "";
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[0, Cam_Num])
                            {
                                t_exist_str = "가로 길이";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[1, Cam_Num])
                            {
                                t_exist_str = "세로 길이";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[2, Cam_Num])
                            {
                                t_exist_str = "몸통 두께";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[3, Cam_Num])
                            {
                                t_exist_str = "몸통 휨";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[4, Cam_Num])
                            {
                                t_exist_str = "나사산 피치";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[5, Cam_Num])
                            {
                                t_exist_str = "나사산 크기";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[6, Cam_Num])
                            {
                                t_exist_str = "리드각(0.5)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[7, Cam_Num])
                            {
                                t_exist_str = "리드각(1)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[8, Cam_Num])
                            {
                                t_exist_str = "사각 영역의 밝기";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[9, Cam_Num])
                            {
                                t_exist_str = "사각 영역의 BLOB";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[10, Cam_Num])
                            {
                                t_exist_str = "하부 V 각도";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[11, Cam_Num])
                            {
                                t_exist_str = "머리 나사부 동심도";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[12, Cam_Num])
                            {
                                t_exist_str = "하부 형상";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[13, Cam_Num])
                            {
                                t_exist_str = "두 영역 중심간 거리";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[14, Cam_Num])
                            {
                                t_exist_str = "면취 측정";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[15, Cam_Num])
                            {
                                t_exist_str = "AI 검사";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[16, Cam_Num])
                            {
                                t_exist_str = "SSF";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[17, Cam_Num])
                            {
                                if (main_str.Contains("모델 사용"))
                                {
                                    t_exist_str = "일치율(%)";
                                    dgvCmbCell8.Items.Add(t_exist_str);
                                    if (tstr == t_exist_str)
                                    {
                                        t_exist_check = true;
                                    }
                                }
                            }
                            if (!t_exist_check)
                            {
                                tstr = t_exist_str;
                            }
                            //dgvCmbCell8.Items.Add("볼록 BLOB 차이");//14
                            if (!tstr.Contains("가로 길이") && !tstr.Contains("세로 길이")
                                && !tstr.Contains("머리 나사부 동심도") && !tstr.Contains("하부 V 각도")
                                && !tstr.Contains("나사산 피치") && !tstr.Contains("나사산 크기")
                                && !tstr.Contains("리드각(1)") && !tstr.Contains("리드각(0.5)")
                                && !tstr.Contains("몸통 두께") && !tstr.Contains("몸통 휨")
                                && !tstr.Contains("사각 영역의 밝기") && !tstr.Contains("사각 영역의 BLOB")
                                && tstr.Length != 0 && !tstr.Contains("하부 형상") && !tstr.Contains("두 영역 중심간 거리")
                                && !tstr.Contains("면취 측정") && !tstr.Contains("AI 검사") && !tstr.Contains("SSF")
                                && !tstr.Contains("일치율(%)") && tstr.Length != 0)
                            {
                                tstr = "가로 길이";
                            }
                        }
                    }
                    string tstr2 = tstr;
                    dataGridView1.Rows[8].Cells[1] = dgvCmbCell8;
                    if (tstr != "" && text_maintain)
                    {
                        if (!main_str.Contains("모델 사용") && tstr.Contains("일치율(%)"))
                        {
                            dataGridView1.Rows[8].Cells[1].Value = "가로 길이";
                        }
                        else
                        {
                            dataGridView1.Rows[8].Cells[1].Value = tstr;
                        }
                    }
                    //else
                    //{
                    //    dataGridView1.Rows[8].Cells[1].Value = "";
                    //}
                    // -------------------------------------------------------------------------
                    // 9번 측정 방법 선택
                    tstr = dataGridView1.Rows[9].Cells[1].Value.ToString();
                    dataGridView1.Rows[9].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell9 = new DataGridViewComboBoxCell();
                    dgvCmbCell9.Items.Add("최솟값");
                    dgvCmbCell9.Items.Add("최댓값");
                    dgvCmbCell9.Items.Add("최댓값-최솟값");
                    dgvCmbCell9.Items.Add("평균값");
                    dgvCmbCell9.Items.Add("총합");
                    dataGridView1.Rows[9].Cells[1] = dgvCmbCell9;
                    if (!tstr.Contains("최솟값") && !tstr.Contains("최댓값") && !tstr.Contains("최댓값-최솟값") && !tstr.Contains("평균값") && !tstr.Contains("총합"))
                    {
                        tstr = "평균값";
                    }
                    if (tstr != "" && text_maintain)
                    {
                        dataGridView1.Rows[9].Cells[1].Value = tstr;
                    }
                    // -------------------------------------------------------------------------
                    // 10, 11번 측정 하위,상위
                    // -------------------------------------------------------------------------
                    // 12번부터 알고리즘에 따른 변수값 설정
                    tstr = tstr2;// dataGridView1.Rows[8].Cells[1].Value.ToString();
                    if (tstr == "가로 길이" || tstr == "세로 길이")
                    {// 0번, 1번
                        dataGridView1.Rows[12].Cells[0].Value = "계산 각도(Degree)";
                        dataGridView1.Rows[13].Cells[0].Value = "측정 최소 거리(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "측정 최대 거리(mm)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "노이즈 제거 필터 크기";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 100)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[16].Cells[0].Value = "에지 전처리(0:미사용,1:사용)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 1;
                        }

                        if (tstr == "가로 길이")
                        {
                            dataGridView1.Rows[17].Cells[0].Value = "측정방법(0:방향성,1:BLOB기준 양끝단,2:Line Gauge,3:ROI좌에서BLOB우,4:ROI우에서BLOB좌)";
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) >= 4)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 4;
                            }
                        }
                        else if (tstr == "세로 길이")
                        {
                            dataGridView1.Rows[17].Cells[0].Value = "측정방법(0:방향성,1:BLOB기준 양끝단,2:Line Gauge,3:BLOB 양하단,4:ROI상에서BLOB하,5:ROI하에서BLOB상)";
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) >= 5)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 5;
                            }
                        }
                    }
                    else if (tstr == "두 영역 중심간 거리")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "측정 방향(0:가로,1:세로)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[13].Cells[0].Value = "영역#1 X좌표";
                        dataGridView1.Rows[14].Cells[0].Value = "영역#1 Y좌표";
                        dataGridView1.Rows[15].Cells[0].Value = "영역#1 가로";
                        dataGridView1.Rows[16].Cells[0].Value = "영역#1 세로";
                        dataGridView1.Rows[17].Cells[0].Value = "영역#2 X좌표";
                        dataGridView1.Rows[18].Cells[0].Value = "영역#2 Y좌표";
                        dataGridView1.Rows[19].Cells[0].Value = "영역#2 가로";
                        dataGridView1.Rows[20].Cells[0].Value = "영역#2 세로";
                    }
                    else if (tstr == "십자 치수")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "측정 모양(0:십자,1:다각,2:내부최대Diameter)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "각 갯수";
                        }
                        else
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "예비변수";
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) != 2)
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "출력(0:중심에서 거리,1:끝점간 거리)";
                            if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[14].Cells[1].Value = 0;
                            }
                        }

                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) == 0 && Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[15].Cells[0].Value = "방법(0:모두,1:좌상에서 우하,2:좌하에서 우상)";
                            if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[15].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 2)
                            {
                                dataGridView1.Rows[15].Cells[1].Value = 2;
                            }
                        }
                        else
                        {
                            dataGridView1.Rows[15].Cells[0].Value = "예비변수";
                        }
                    }
                    else if (tstr == "사각 영역의 밝기")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "처리 방법(0:흑백,1:컬러)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "컬러 임계 하한값(0~360)";
                            dataGridView1.Rows[14].Cells[0].Value = "컬러 임계 상한값(0~360)";
                            dataGridView1.Rows[15].Cells[0].Value = "노이즈 제거 필터 크기";
                            dataGridView1.Rows[16].Cells[0].Value = "검사대상크기(하한값)";
                            dataGridView1.Rows[17].Cells[0].Value = "검사대상크기(상한값)";
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) == 0)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 100000;
                            }
                            dataGridView1.Rows[18].Cells[0].Value = "출력 방법(0:밝기,1:픽셀수)";
                            if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[18].Cells[1].Value = 0;
                            }
                            dataGridView1.Rows[19].Cells[0].Value = "전처리(Blur)";
                        }
                        else
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "전처리(Blur)";
                            //dataGridView1.Rows[13].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[14].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[15].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[16].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[17].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[18].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[13].Cells[1].Value = "0";
                            dataGridView1.Rows[14].Cells[1].Value = "0";
                            dataGridView1.Rows[15].Cells[1].Value = "0";
                            dataGridView1.Rows[16].Cells[1].Value = "0";
                            dataGridView1.Rows[17].Cells[1].Value = "0";
                            dataGridView1.Rows[18].Cells[1].Value = "0";
                        }
                    }
                    else if (tstr == "원형 영역의 밝기")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "영역1 지름(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "영역1 두께(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "영역2 지름(mm)";
                        dataGridView1.Rows[15].Cells[0].Value = "영역2 두께(mm)";
                        dataGridView1.Rows[16].Cells[0].Value = "옵션(0:영역1의 밝기,1:영역1과 영역2의 밝기 차이)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "예비변수";
                            dataGridView1.Rows[15].Cells[0].Value = "예비변수";
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "영역2 지름(mm)";
                            dataGridView1.Rows[15].Cells[0].Value = "영역2 두께(mm)";
                        }
                    }
                    else if (tstr == "원형 영역의 BLOB" || tstr == "원형 영역의 BLOB 갯수")
                    {
                        int tt_idx = 12;
                        // 변수 추가
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "영역1 지름(mm)"; tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "영역1 두께(mm)"; tt_idx++;
                        //dataGridView1.Rows[14].Cells[0].Value = "영역2 지름(mm)";
                        //dataGridView1.Rows[15].Cells[0].Value = "영역2 두께(mm)";
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "시작각(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "종료각(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 360;
                        }
                        tt_idx = 14;
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) == 0 && Convert.ToInt32(dataGridView1.Rows[tt_idx+1].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                            dataGridView1.Rows[tt_idx+1].Cells[1].Value = 360;
                        }
                        tt_idx = 16;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Blur 필터링";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Erode";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 100;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Dilate";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "처리 순서(0:Erode->Dilate,1:Dilate->Erode)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 1)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 1;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }

                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "크기 하한값(Pixel)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "크기 상한값(Pixel)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 100000;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "기준옵션(0:ROI#0,1:P이하,2:P이상)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 2)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx - 1].Cells[1].Value.ToString()) < 3)
                        {
                            dataGridView1.Rows[tt_idx].Cells[0].Value = "P임계값";
                            if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 255)
                            {
                                dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                            }
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "계산방법(0:픽셀수,1:갯수,2:장축,3:단축,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 8)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 8;
                        }

                        //if (Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) >= 1 && Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) <= 6)
                        //{
                        //    if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) >= 500)
                        //    {
                        //        dataGridView1.Rows[19].Cells[1].Value = 0;
                        //    }
                        //    if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) >= 500)
                        //    {
                        //        dataGridView1.Rows[20].Cells[1].Value = 500;
                        //    }
                        //}
                        tt_idx++;
                        //dataGridView1.Rows[tt_idx].Cells[0].Value = "옵션(0:미사용,1:볼록 BLOB 차이,2:영역1(v1이하) 영역2(v2이상) 임계화)";
                        //if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= -1)
                        //{
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        //}
                    }
                    else if (tstr == "원형 영역의 색상 BLOB")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "영역 반지름(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "영역 두께(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "전처리 필터 크기";
                        dataGridView1.Rows[15].Cells[0].Value = "컬러 임계 하한값(0~360)";
                        dataGridView1.Rows[16].Cells[0].Value = "컬러 임계 상한값(0~360)";
                        //dataGridView1.Rows[17].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[17].Cells[0].Value = "검사대상크기(하한값)";
                        dataGridView1.Rows[18].Cells[0].Value = "검사대상크기(상한값)";
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[19].Cells[0].Value = "출력 방법(0:밝기,1:픽셀수,2:그룹수)";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[20].Cells[0].Value = "기준옵션(0:ROI#0,1:P이하,2:P이상,3:계산각도)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) > 3)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 2)
                        {
                            dataGridView1.Rows[21].Cells[0].Value = "P임계값";
                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 255)
                            {
                                dataGridView1.Rows[21].Cells[1].Value = 0;
                            }
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) == 3)
                        {
                            dataGridView1.Rows[21].Cells[0].Value = "계산각도";
                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 360)
                            {
                                dataGridView1.Rows[21].Cells[1].Value = 360;
                            }
                        }
                    }
                    else if (tstr == "사각 영역의 BLOB" || tstr == "사각 ROI의 BLOB 갯수(Count)")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "전처리(Blur)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)";
                        dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "MP 사용(0:미사용,1:사용,2:컬러+미사용,4:컬러+사용)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[16].Cells[0].Value = "MP 방향(0:전방향,1:가로,2:세로)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "MP 처리수(CNT)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 1;
                        }

                        dataGridView1.Rows[18].Cells[0].Value = "Erode";
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) > 500)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 500;
                        }
                        dataGridView1.Rows[19].Cells[0].Value = "Dilate";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 500)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 500;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[20].Cells[0].Value = "처리 순서(0:Erode->Dilate,1:Dilate->Erode)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) > 1)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 1;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[21].Cells[0].Value = "볼록 BLOB(0:미사용,1:사용)";
                        if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[21].Cells[1].Value = 0;
                        }

                        if (tstr == "사각 영역의 BLOB")
                        {
                            dataGridView1.Rows[22].Cells[0].Value = "계산방법(0:픽셀수,1:가로길이,2:세로길이,3:갯수,4:기준점부터 거리,5:픽셀수(가장자리 제외),6:픽셀수(가장자리),7:좌우,상하 두께 차이,8:배터리팩Align,9:예외부분)";
                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[22].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) > 9)
                            {
                                dataGridView1.Rows[22].Cells[1].Value = 9;
                            }

                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) == 1)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "가로 최소 길이";
                                dataGridView1.Rows[14].Cells[0].Value = "가로 최대 길이";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) == 2)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "세로 최소 길이";
                                dataGridView1.Rows[14].Cells[0].Value = "세로 최대 길이";
                            }
                        }
                    }
                    else if (tstr == "면취 측정")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)";
                        dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "전처리(Blur)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[16].Cells[0].Value = "계산방법(0:양면 각도, 1:양면 면취 가로, 2:양면 면취 세로, , 3:양면 면취 치수)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 3)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 3;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "면취위치(0:상, 1:하, 2:좌, 3:우)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) >= 3)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 3;
                        }
                    }
                    else if (tstr == "직경")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[13].Cells[0].Value = "직경 최소 길이";
                        dataGridView1.Rows[14].Cells[0].Value = "직경 최대 길이";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "측정방법(0:직경,1:중심에서 최소 거리,2:중심에서 최대 거리,3:중심에서 최대-최소 거리,4:Distance Transform,5:직경 추정)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 5)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 5;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) == 5)
                        {
                            dataGridView1.Rows[16].Cells[0].Value = "계산방향(0:좌->우,1:우->좌,2:상->하,3:하>상)";
                            if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[16].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 3)
                            {
                                dataGridView1.Rows[16].Cells[1].Value = 3;
                            }
                        }
                    }
                    else if (tstr == "진원도(%)")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[13].Cells[0].Value = "계산 최소 직경";
                        dataGridView1.Rows[14].Cells[0].Value = "계산 최대 직경";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "측정방법(0:%,1:100-%)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 1;
                        }
                    }
                    else if (tstr == "머리 나사부 동심도") // 번
                    {
                        //dataGridView1.Rows[5].Cells[1].Value = "검사 영역 결과 사용";
                    }
                    else if (tstr == "하부 V 각도") // 3번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "각도 계산 높이(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "회전 나사선 제거 필터 크기";
                    }
                    else if (tstr == "몸통 휨" || tstr == "몸통 두께") // 7, 8번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "회전 나사선 제거 필터 크기";
                        if (tstr == "몸통 휨" && comboBox_TABLETYPE.SelectedIndex == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "계산방법(0:차이거리,1:몸통각도,2:상부각도,3:흔들림,4:상-하부각도)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 4)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 4;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) == 3)
                            {
                                dataGridView1.Rows[14].Cells[0].Value = "흔들림 옵션(0:좌우각 차이,1:좌측 각,2:우측 각)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) >= 2)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 2;
                                }
                            }
                            else
                            {
                                dataGridView1.Rows[14].Cells[0].Value = "예비변수";
                                dataGridView1.Rows[14].Cells[1].Value = 0;
                            }
                        }
                        else if (tstr == "몸통 두께" && comboBox_TABLETYPE.SelectedIndex == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "계산방법(0:두께,1:유효경,2:평행도,3:단높이)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 3)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 3;
                            }
                        }
                    }
                    else if (tstr == "하부 형상")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "스크류 제거 필터 크기"; // 이름
                        dataGridView1.Rows[13].Cells[0].Value = "형상 계산 높이(mm)"; // 이름
                    }
                    else if (tstr.Contains("리드각") || tstr.Contains("나사산"))
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "나사산 붙음 방지 필터링(Count)"; // 이름
                        dataGridView1.Rows[13].Cells[0].Value = "나사산 찾기 필터링(Count)"; // 이름
                        if (tstr.Contains("나사산 크기"))
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "계산방법(0:픽셀수,1:가로길이,2:세로길이,3:볼록BLOB)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[14].Cells[1].Value = 0;
                            }
                            //if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0)
                            //{
                            //    dataGridView1.Rows[14].Cells[0].Value = "필터 방향(0:전방향,1:가로,2:세로)"; // 이름
                            //    if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= -1)
                            //    {
                            //        dataGridView1.Rows[14].Cells[1].Value = 0;
                            //    }
                            //}
                        }
                    }
                    else if (tstr == "볼록 BLOB 차이")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)";
                        dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "계산방법(0:픽셀수,1:가로길이,2:세로길이)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                    }
                    else if (tstr == "내외경 중심 차이")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "내경 임계값";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 255)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 255;
                        }
                        dataGridView1.Rows[13].Cells[0].Value = "내경 임계방법(0:이하,1:이상)";
                        if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[13].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[14].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[15].Cells[0].Value = "BLOB 붙임 필터 크기";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[16].Cells[0].Value = "출력(0:mm,1:픽셀,2:내경(mm),3:외경(mm),4:외경-내경(mm))";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 4 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "내경 계산 거리(mm)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 0;
                        }
                    }
                    else if (tstr == "AI 검사")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Matching Rate[0,1]";
                        if (Convert.ToDouble(dataGridView1.Rows[12].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        //if (Convert.ToDouble(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 1)
                        //{
                        //    dataGridView1.Rows[12].Cells[1].Value = 1;
                        //}
                        dataGridView1.Rows[13].Cells[0].Value = "모델 불러오기";
                        if (listBox1_SelectedIndex > 0)
                        {
                            if (LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num, listBox1_SelectedIndex - 1])
                            {
                                dataGridView1.Rows[13].Cells[1].Value = "1";
                            }
                            else
                            {
                                dataGridView1.Rows[13].Cells[1].Value = "0";
                            }
                        }
                        dataGridView1.Rows[14].Cells[0].Value = "모델 학습";
                        dataGridView1.Rows[15].Cells[0].Value = "ROI 이미지 저장";
                        dataGridView1.Rows[14].Cells[1].Value = "0";
                        dataGridView1.Rows[15].Cells[1].Value = "0";
                    }
                    else if (tstr == "SSF")
                    {
                        int t_Start_Idx = 12;
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Lifting X(%)";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0.1;
                        }
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 100;
                        }
                        t_Start_Idx++;//13
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Lifting Y(%)";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0.1;
                        }
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 100;
                        }
                        t_Start_Idx++;//14
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Defect SSF X";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//15
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Defect SSF Y";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//16
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Base SSF X";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//17
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Base SSF Y";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//18
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Opened";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//19
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Closed";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//20
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "GD Dark";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//21
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "GD Bright";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//22
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Size Dark";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//23
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Size Bright";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//24
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Output(0:Count,1:Size,2:Omit,3:AI(지정 번호),4:AI(지정 번호 외),5:AI(모두),6:AI 결과 가져오기";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 6)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 6;
                        }
                        t_Start_Idx++;//25
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx-1].Cells[1].Value.ToString()) >= 3 && Convert.ToDouble(dataGridView1.Rows[t_Start_Idx - 1].Cells[1].Value.ToString()) <= 6)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "AI 클래스 번호";
                            if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                            {
                                dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                            }
                            if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 41)
                            {
                                dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 40;
                            }

                            //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_AI_Model_Loaded(Cam_Num, listBox1_SelectedIndex))
                            //{
                            //    dataGridView1.Rows[t_Start_Idx].Cells[1].Value = "1";
                            //}
                            //else
                            //{
                            //    dataGridView1.Rows[t_Start_Idx].Cells[1].Value = "0";
                            //}
                        }
                        else
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "예비변수";
                        }
                    }
                    if (comboBox_TABLETYPE.SelectedIndex == 5)
                    {
                        if (listBox1.SelectedIndex == 0)
                        {
                            for (int i = 12; i < dataGridView1.RowCount; i++)
                            {
                                dataGridView1.Rows[i].Cells[0].Value = "예비변수";
                                dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                            }
                            dataGridView1.Rows[10].Cells[1].Style.BackColor = Color.DimGray;
                            dataGridView1.Rows[11].Cells[1].Style.BackColor = Color.DimGray;

                            dataGridView1.Rows[12].Cells[0].Value = "고객사 번호";
                            if (dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && Cam_Num == 0)
                            {
                                comboBox_CAMPOSITION.SelectedIndex = 0;
                                dataGridView1.Rows[13].Cells[0].Value = "단자 최소 크기(Pixel)";
                                dataGridView1.Rows[14].Cells[0].Value = "Slit 최소 크기(Pixel)";
                                dataGridView1.Rows[15].Cells[0].Value = "단자 높이 min(Pixel)";
                                dataGridView1.Rows[16].Cells[0].Value = "단자 높이 max(Pixel)";
                                dataGridView1.Rows[17].Cells[0].Value = "단자 휨 각도 허용 오차(Degree)";
                                dataGridView1.Rows[18].Cells[0].Value = "Slit Chip 임계값(Gray,이하)";
                                dataGridView1.Rows[19].Cells[0].Value = "단자 들뜸 최소 크기(Pixel)";
                                dataGridView1.Rows[20].Cells[0].Value = "기본값 복원(1입력)";
                                if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) == 1)
                                {
                                    dataGridView1.Rows[13].Cells[1].Value = 50;
                                    dataGridView1.Rows[14].Cells[1].Value = 30;
                                    dataGridView1.Rows[15].Cells[1].Value = 50;
                                    dataGridView1.Rows[16].Cells[1].Value = 70;
                                    dataGridView1.Rows[17].Cells[1].Value = 5;
                                    dataGridView1.Rows[18].Cells[1].Value = 150;
                                    dataGridView1.Rows[19].Cells[1].Value = 10;
                                    dataGridView1.Rows[20].Cells[1].Value = 0;
                                }
                                int t_idx = 1;
                                listBox1.Items[t_idx] = "내경(지름 mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "내경(지름 mm)"; t_idx++;
                                listBox1.Items[t_idx] = "단자경(지름 mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자경(지름 mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 폭 Min(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 폭 Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 폭 Max(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 폭 Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 깊이 Min(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 깊이 Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 깊이 Max(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 깊이 Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "단자간 각도(angle)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자간 각도(angle)"; t_idx++;
                                listBox1.Items[t_idx] = "단자 슬리트간 각도(angle)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자 슬리트간 각도(angle)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트간 각도(angle)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트간 각도(angle)"; t_idx++;
                                listBox1.Items[t_idx] = "단자수(count)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자수(count)"; t_idx++;
                                listBox1.Items[t_idx] = "단자휨 갯수(count)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자휨 갯수(count)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 수(count)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 수(count)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 불량(pixel)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 불량(pixel)"; t_idx++;
                                listBox1.Items[t_idx] = "단자 들뜸(count)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자 들뜸(count)"; t_idx++;

                                //LVApp.Instance().m_Config.Save_Judge_Data();
                            }
                            else if (dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && Cam_Num != 0)
                            {
                                if (LVApp.Instance().m_mainform.m_Start_Check && text_maintain)
                                {
                                    AutoClosingMessageBox.Show("스기야마는 CAM0만 지원합니다. ", "WARNING", 3000);
                                }
                            }
                            else
                            {
                                if (LVApp.Instance().m_mainform.m_Start_Check && text_maintain && Cam_Num == 0)
                                {
                                    AutoClosingMessageBox.Show("고객 번호가 없거나 잘못 입력 하셨습니다.", "WARNING", 3000);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 10; i < dataGridView1.RowCount; i++)
                            {
                                dataGridView1.Rows[i].Cells[0].Value = "예비변수";
                                dataGridView1.Rows[i].Cells[1].Value = 0;
                                dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                            }
                        }
                    }
                    else
                    {
                        dataGridView1.Rows[10].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                        dataGridView1.Rows[11].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                    }

                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[5].Selected = true;
                    CurrencyManager currencyManager0 = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
                    currencyManager0.SuspendBinding();

                    if (m_advenced_param_visible)
                    {
                        for (int i = 10; i < dataGridView1.RowCount; i++)
                        {
                            dataGridView1.Rows[i].Visible = true;
                            dataGridView1.Rows[i].Cells[1].ReadOnly = false;

                            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수")
                            {
                                dataGridView1.Rows[i].Visible = false;
                            }
                            else
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("하위 측정범위")
                                || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("상위 측정범위")
                                || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. Low bound")
                                || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. High bound"))
                                {
                                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                                }
                                else
                                {
                                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightPink;
                                }
                            }

                            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "모델 불러오기" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Model Load" 
                                || dataGridView1.Rows[i].Cells[0].Value.ToString() == "모델 학습" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Model Train"
                                || dataGridView1.Rows[i].Cells[0].Value.ToString() == "ROI 이미지 저장" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "ROI Image Save"
                            )
                            {
                                dataGridView1.Rows[i].Cells[1].ReadOnly = true;
                            }
                        }
                        //for (int i = 12; i < dataGridView1.RowCount; i++)
                        //{
                        //    dataGridView1.Rows[i].Visible = true;
                        //    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || !m_range_visible_check && (
                        //        dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("하위 측정범위")
                        //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("상위 측정범위")
                        //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. Low bound")
                        //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. High bound"))
                        //        )
                        //    {
                        //        dataGridView1.Rows[i].Cells[1].Value = 0;
                        //        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                        //        dataGridView1.Rows[i].Visible = false;
                        //    }
                        //    else if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "고객사 번호")
                        //    {
                        //        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.SkyBlue;
                        //    }
                        //    else if (m_range_visible_check && (
                        //        dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("하위 측정범위")
                        //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("상위 측정범위")
                        //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. Low bound")
                        //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. High bound")))
                        //    {
                        //        dataGridView1.Rows[i].Visible = true;
                        //    }
                        //    else
                        //    {
                        //        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightPink;
                        //    }
                        //}
                    }
                    else
                    {
                        for (int i = 10; i < dataGridView1.RowCount; i++)
                        {
                            dataGridView1.Rows[i].Visible = false;
                        }
                    }

                    dataGridView1.Rows[0].Visible = false;
                    currencyManager0.ResumeBinding();
                    //dataGridView1.Refresh();
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1 || LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//영어, 중국어
                    // -------------------------------------------------------------------------
                    // 0번 사용 유무
                    string tstr = dataGridView1.Rows[0].Cells[1].Value.ToString();
                    dataGridView1.Rows[0].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell = new DataGridViewComboBoxCell();
                    dgvCmbCell.Items.Add("O");
                    dgvCmbCell.Items.Add("X");
                    dataGridView1.Rows[0].Cells[1] = dgvCmbCell;
                    if (!tstr.Contains("O") && !tstr.Contains("X"))
                    {
                        tstr = "X";
                    }
                    if (tstr != "" && text_maintain)
                    {
                        dataGridView1.Rows[0].Cells[1].Value = tstr;
                    }
                    // -------------------------------------------------------------------------
                    // 1~4번 ROI 정보
                    // -------------------------------------------------------------------------
                    // 5번 임계화
                    tstr = dataGridView1.Rows[5].Cells[1].Value.ToString();
                    dataGridView1.Rows[5].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell5 = new DataGridViewComboBoxCell();

                    dgvCmbCell5.Items.Add("v1 less than");
                    dgvCmbCell5.Items.Add("v2 more than");
                    dgvCmbCell5.Items.Add("v1~v2");
                    dgvCmbCell5.Items.Add("less v1 more v2");
                    dgvCmbCell5.Items.Add("Auto less than");
                    dgvCmbCell5.Items.Add("Auto more than");
                    dgvCmbCell5.Items.Add("Edge");
                    dgvCmbCell5.Items.Add("Diff. from AVG");
                    string main_str = "";
                    if (Cam_Num == 0)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();
                    }
                    else if (Cam_Num == 1)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();
                    }
                    else if (Cam_Num == 2)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();
                    }
                    else if (Cam_Num == 3)
                    {
                        main_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();
                    }
                    if (listBox1_SelectedIndex != 0)
                    {
                        dgvCmbCell5.Items.Add("Compare less v1 more v2");
                    }

                    if (listBox1.SelectedIndex == 0)
                    {
                        if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] != 1)
                        {
                            dgvCmbCell5.Items.Add("Model find");
                        }
                        //if (tstr == "Insp. area result use")
                        //{
                        //    tstr = "v1 less than";
                        //    dataGridView1.Rows[5].Cells[1].Value = "v1 less than";
                        //}
                    }
                    else
                    {
                        if (!main_str.Contains("Model find"))
                        {
                            dgvCmbCell5.Items.Add("Insp. area result use");
                            if (tstr == "Compare less v1 more v2")
                            {
                                tstr = "v1 less than";
                            }
                        }
                        else
                        {
                            //if (tstr == "Insp. area result use" || tstr == "Model find")
                            //{
                            //    tstr = "v1 less than";
                            //}
                        }
                    }

                    dataGridView1.Rows[5].Cells[1] = dgvCmbCell5;
                    if (!tstr.Contains("v1 less than") && !tstr.Contains("v2 more than") && !tstr.Contains("v1~v2")
                         && !tstr.Contains("Auto less than") && !tstr.Contains("Auto more than") && !tstr.Contains("Edge")
                         && !tstr.Contains("Model find") && !tstr.Contains("Insp. area result use") && !tstr.Contains("less v1 more v2") && !tstr.Contains("Diff. from AVG") && !tstr.Contains("Compare less v1 more v2"))
                    {
                        tstr = "v1 less than";
                    }
                    if (tstr != "" && text_maintain)
                    {
                        if (!main_str.Contains("Model find") && tstr.Contains("Compare less v1 more v2"))
                        {
                            dataGridView1.Rows[5].Cells[1].Value = "v1 less than";
                        }
                        else
                        {
                            dataGridView1.Rows[5].Cells[1].Value = tstr;
                        }
                    }
                    if (tstr == "v1 이하" || tstr == "v1 less than")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "v2 이상" || tstr == "v2 more than")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "v1~v2 사이" || tstr == "v1~v2")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "v1이하v2이상" || tstr == "less v1 more v2")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "자동이하" || tstr == "Auto less than")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "자동이상" || tstr == "Auto more than")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "에지" || tstr == "Edge")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "검사 영역 결과 사용" || tstr == "모델 사용" || tstr == "Insp. area result use" || tstr == "Model find")
                    {
                        dataGridView1.Rows[6].Visible = false;
                        dataGridView1.Rows[7].Visible = false;
                    }
                    else if (tstr == "평균기준 차이" || tstr == "Diff. from AVG")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    else if (tstr == "비교v1이하v2이상" || tstr == "Compare less v1 more v2")
                    {
                        dataGridView1.Rows[6].Visible = true;
                        dataGridView1.Rows[7].Visible = true;
                    }
                    // -------------------------------------------------------------------------
                    // 6,7번 임계화 하한, 상한값
                    // -------------------------------------------------------------------------
                    // 8번 알고리즘 선택
                    tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                    dataGridView1.Rows[8].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell8 = new DataGridViewComboBoxCell();

                    for (int i = 12; i <= 25; i++)
                    {
                        dataGridView1.Rows[i].Cells[0].Value = "Preliminary";
                        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                        //dataGridView1.Rows[i].Cells[1].Value = "0";
                    }

                    if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0)
                    { // 상부 또는 하부이면 
                        if (listBox1.SelectedIndex == 0)
                        {
                            dgvCmbCell8.Items.Add("Left end");
                            dgvCmbCell8.Items.Add("Right end");
                            dgvCmbCell8.Items.Add("Left top");
                            dgvCmbCell8.Items.Add("Left bottom");
                            dgvCmbCell8.Items.Add("Right top");
                            dgvCmbCell8.Items.Add("Right bottom");
                            dgvCmbCell8.Items.Add("Center");
                            dgvCmbCell8.Items.Add("Top Center");
                            if (!tstr.Contains("Left") && !tstr.Contains("Right") && !tstr.Contains("Center") && tstr.Length != 0)
                            {
                                tstr = "Center";
                            }

                            if (dataGridView1.Rows[5].Cells[1].Value.ToString() == "Model find")
                            {
                                tstr = "Center";
                                if (LVApp.Instance().m_mainform.m_ImProClr_Class.EasyFind_Check())
                                {
                                    // 변수 추가
                                    dataGridView1.Rows[12].Cells[0].Value = "Angle Tolerance(Degree)"; // 이름
                                                                                                       // 변수 추가
                                    dataGridView1.Rows[13].Cells[0].Value = "Scale Tolerance(%)"; // 이름
                                                                                                  // 변수 추가
                                    dataGridView1.Rows[14].Cells[0].Value = "Find extension(Pixel)"; // 이름
                                    dataGridView1.Rows[15].Cells[0].Value = "Find LightBalance([-1,1])"; // 이름

                                    dataGridView1.Rows[16].Cells[0].Value = "None";

                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) >= 360)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 360;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[13].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 100)
                                    {
                                        dataGridView1.Rows[13].Cells[1].Value = 100;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[14].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) >= Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2)
                                    {
                                        dataGridView1.Rows[14].Cells[1].Value = Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2;
                                    }
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = -1;
                                    }
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 1)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 1000)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 1000;
                                    }
                                }
                                else
                                {
                                    // 변수 추가
                                    dataGridView1.Rows[12].Cells[0].Value = "Decimation Scale(1,2,4)";
                                    // 변수 추가
                                    dataGridView1.Rows[13].Cells[0].Value = "Top Margin(Pixel)";
                                    // 변수 추가
                                    dataGridView1.Rows[14].Cells[0].Value = "Bottom Margin(Pixel)";
                                    dataGridView1.Rows[15].Cells[0].Value = "Edge([0 1])";
                                    dataGridView1.Rows[16].Cells[0].Value = "Rotation Score(%)";
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= 1)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) >= 4)
                                    {
                                        dataGridView1.Rows[12].Cells[1].Value = 4;
                                    }
                                    //if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                                    //{
                                    //    dataGridView1.Rows[13].Cells[1].Value = 0;
                                    //}
                                    //if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 100)
                                    //{
                                    //    dataGridView1.Rows[13].Cells[1].Value = 100;
                                    //}
                                    //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                                    //{
                                    //    dataGridView1.Rows[14].Cells[1].Value = 0;
                                    //}
                                    //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) >= Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2)
                                    //{
                                    //    dataGridView1.Rows[14].Cells[1].Value = Math.Min(pictureBox_Image.Image.Height, pictureBox_Image.Image.Width) / 2;
                                    //}
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = -1;
                                    }
                                    if (Convert.ToDouble(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 1)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = 1;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 0)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 0;
                                    }
                                    if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 100)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 100;
                                    }
                                }

                                dataGridView1.Rows[17].Cells[0].Value = "Rotation(0:None,1:W_Ribbon,2:B_Ribbon,3:T/B flip when NG)";
                                if (Convert.ToDouble(dataGridView1.Rows[17].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 0;
                                }
                                if (Convert.ToDouble(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 3)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 3;
                                }
                            }
                            else
                            {
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    if (comboBox_TABLETYPE.SelectedIndex != 5)
                                    {
                                        dataGridView1.Rows[14].Cells[1].Value = 100000;
                                    }
                                }
                                dataGridView1.Rows[15].Cells[0].Value = "Filter direction for noise reduction(0:All,1:Hor.,2:Ver.)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                                {
                                    if (comboBox_TABLETYPE.SelectedIndex != 5)
                                    {
                                        dataGridView1.Rows[15].Cells[1].Value = 0;
                                    }
                                }
                                dataGridView1.Rows[16].Cells[0].Value = "Angle cal.(0:None,1:Left,2:Right,3:Top,4:Bottom,5:LR_Center,6:TB_Center,7:Circle Preprocessing)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 7 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                                {
                                    if (comboBox_TABLETYPE.SelectedIndex != 5)
                                    {
                                        dataGridView1.Rows[16].Cells[1].Value = 0;
                                    }
                                }
                                // 변수 추가
                                dataGridView1.Rows[17].Cells[0].Value = "Filter size for BLOB merge"; // 이름
                                dataGridView1.Rows[18].Cells[0].Value = "BLOB Select(0:Largest,1:Top,2:Bottom,3:Left,4:Right)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[18].Cells[1].Value = 0;
                                }
                                else if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) >= 4)
                                {
                                    dataGridView1.Rows[18].Cells[1].Value = 4;
                                }
                                dataGridView1.Rows[19].Cells[0].Value = "Blur filter";
                                if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[19].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) >= 100)
                                {
                                    dataGridView1.Rows[19].Cells[1].Value = 100;
                                }

                                dataGridView1.Rows[20].Cells[0].Value = "Color preprocessing";
                                if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[20].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) >= 1)
                                {
                                    dataGridView1.Rows[20].Cells[1].Value = 1;
                                }

                            }
                        }
                        else
                        {
                            bool t_exist_check = false; string t_exist_str = "";
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[0, Cam_Num])
                            {
                                t_exist_str = "Hor. length";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[1, Cam_Num])
                            {
                                t_exist_str = "Ver. length";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[2, Cam_Num])
                            {
                                t_exist_str = "Brightness of circle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);//5
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[3, Cam_Num])
                            {
                                t_exist_str = "BLOB in circle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[4, Cam_Num])
                            {
                                t_exist_str = "Brightness of rectangle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[5, Cam_Num])
                            {
                                t_exist_str = "BLOB in rectangle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[6, Cam_Num])
                            {
                                t_exist_str = "Diameter";
                                dgvCmbCell8.Items.Add(t_exist_str);//3
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[7, Cam_Num])
                            {
                                t_exist_str = "Circularity(%)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[8, Cam_Num])
                            {
                                t_exist_str = "Dim. of cross";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[9, Cam_Num])
                            {
                                t_exist_str = "Distance between two area";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[10, Cam_Num])
                            {
                                t_exist_str = "Pitch of thread";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[11, Cam_Num])
                            {
                                t_exist_str = "Size of thread";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[12, Cam_Num])
                            {
                                t_exist_str = "Color BLOB in circle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);//14
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[13, Cam_Num])
                            {
                                t_exist_str = "Center difference between Inner and outter circle";
                                dgvCmbCell8.Items.Add(t_exist_str);//16
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[14, Cam_Num])
                            {
                                t_exist_str = "Bevelling Measurement";
                                dgvCmbCell8.Items.Add(t_exist_str);//16
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[15, Cam_Num])
                            {
                                t_exist_str = "AI Inspection";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[16, Cam_Num])
                            {
                                t_exist_str = "SSF";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[17, Cam_Num])
                            {
                                if (main_str.Contains("Model find"))
                                {
                                    t_exist_str = "Match rate(%)";
                                    dgvCmbCell8.Items.Add(t_exist_str);
                                    if (tstr == t_exist_str)
                                    {
                                        t_exist_check = true;
                                    }
                                }
                                if (tstr.Contains("BLOB size in circle ROI") || tstr.Contains("BLOB count in circle ROI"))
                                {
                                    tstr = "BLOB in circle ROI";
                                    if (tstr == "BLOB in circle ROI")
                                    {
                                        t_exist_check = true;
                                    }
                                }
                            }

                            if (!t_exist_check)
                            {
                                tstr = t_exist_str;
                            }
                            if (!tstr.Contains("Hor. length") && !tstr.Contains("Ver. length")
                                && !tstr.Contains("Dim. of cross") && !tstr.Contains("Diameter")
                                && !tstr.Contains("Brightness of rectangle ROI") && !tstr.Contains("Brightness of circle ROI")
                                && !tstr.Contains("BLOB in rectangle ROI") && !tstr.Contains("BLOB count in rect ROI(Count)")
                                //&& !tstr.Contains("BLOB size in circle ROI") && !tstr.Contains("BLOB count in circle ROI")
                                && !tstr.Contains("BLOB in circle ROI")
                                && !tstr.Contains("Circularity(%)") && !tstr.Contains("Pitch of thread") && !tstr.Contains("Distance between two area")
                                && !tstr.Contains("Size of thread") && !tstr.Contains("Color BLOB in circle ROI") && !tstr.Contains("Match rate(%)")
                                && !tstr.Contains("Bevelling Measurement") && !tstr.Contains("AI Inspection") && !tstr.Contains("SSF") 
                                && !tstr.Contains("Center difference between Inner and outter circle") && tstr.Length != 0)
                            {
                                tstr = "Hor. length";
                            }
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 1)
                    { // 사이드이면 
                        if (listBox1.SelectedIndex == 0)
                        {
                            dgvCmbCell8.Items.Add("Left end");
                            dgvCmbCell8.Items.Add("Right end");
                            dgvCmbCell8.Items.Add("Left top");
                            dgvCmbCell8.Items.Add("Left bottom");
                            dgvCmbCell8.Items.Add("Right top");
                            dgvCmbCell8.Items.Add("Right bottom");
                            dgvCmbCell8.Items.Add("Center");
                            dgvCmbCell8.Items.Add("Top Center");
                            if (!tstr.Contains("Left") && !tstr.Contains("Right") && !tstr.Contains("Center") && tstr.Length != 0)
                            {
                                tstr = "Center";
                            }

                            //dgvCmbCell8.Items.Add("Left top");
                            ////if (!tstr.Contains("기준"))
                            //{
                            //    tstr = "Left top";
                            //}

                            if (comboBox_TABLETYPE.SelectedIndex == 0)
                            {// 인덱스 타입 일때
                                //dataGridView1.Rows[12].Cells[0].Value = "Width of head(mm)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[13].Cells[0].Value = "Top y coor. for rotation(mm)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[14].Cells[0].Value = "Height for rotation(mm)"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[15].Cells[0].Value = "Filter size for removing thread"; // 이름
                                dataGridView1.Rows[12].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[15].Cells[0].Value = "Filter size for removing thread"; // 이름
                                dataGridView1.Rows[16].Cells[0].Value = "Method(0:T&B,1:Bottom,2:Index plate)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[16].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 2)
                                {
                                    dataGridView1.Rows[16].Cells[1].Value = 2;
                                }
                                dataGridView1.Rows[17].Cells[0].Value = "Roration(0:None,1:Top Short,2:Top Long,3:Top Thin,4:Top Thick)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 4)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 4;
                                }
                                //if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 0 && Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 2)
                                //{
                                //    dataGridView1.Rows[18].Cells[0].Value = "Top Height(mm)"; // 이름
                                //    if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) < 0)
                                //    {
                                //        dataGridView1.Rows[18].Cells[1].Value = 0;
                                //    }
                                //}

                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 1)
                            {// 글라스 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Preliminary"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                }
                                //// 변수 추가
                                //dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB"; // 이름
                                //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                //{
                                //    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                //}
                                dataGridView1.Rows[15].Cells[0].Value = "Body height(mm) for angle calculration"; // 이름
                                dataGridView1.Rows[16].Cells[0].Value = "Filter size for removing thread"; // 이름
                                dataGridView1.Rows[17].Cells[0].Value = "Method(0:None,1:Body,2:Glass)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) < 0)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 2)
                                {
                                    dataGridView1.Rows[17].Cells[1].Value = 2;
                                }
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 2)
                            {// 벨트 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "Thickness of guide"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Preliminary"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                }
                                //// 변수 추가
                                //dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB"; // 이름
                                //// 변수 추가
                                //dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB"; // 이름
                                //if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                //{
                                //    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                //}
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 3)
                            {// 가이드 없을때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "Preliminary"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Top y coor. for rotation(mm)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Height for rotation(mm)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[15].Cells[0].Value = "Filter size for removing thread"; // 이름
                            }
                        }
                        else
                        {
                            bool t_exist_check = false; string t_exist_str = "";
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[0, Cam_Num])
                            {
                                t_exist_str = "Hor. length";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[1, Cam_Num])
                            {
                                t_exist_str = "Ver. length";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[2, Cam_Num])
                            {
                                t_exist_str = "Thickness of body(mm)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[3, Cam_Num])
                            {
                                t_exist_str = "Bending of body(mm)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[4, Cam_Num])
                            {
                                t_exist_str = "Pitch of thread";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[5, Cam_Num])
                            {
                                t_exist_str = "Size of thread";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[6, Cam_Num])
                            {
                                t_exist_str = "Lead angle of thread(0.5)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[7, Cam_Num])
                            {
                                t_exist_str = "Lead angle of thread(1)";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[8, Cam_Num])
                            {
                                t_exist_str = "Brightness of rectangle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[9, Cam_Num])
                            {
                                t_exist_str = "BLOB in rectangle ROI";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[10, Cam_Num])
                            {
                                t_exist_str = "V Angle of bottom";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[11, Cam_Num])
                            {
                                t_exist_str = "Concentricity";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[12, Cam_Num])
                            {
                                t_exist_str = "Shape of bottom";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[13, Cam_Num])
                            {
                                t_exist_str = "Distance between two area";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }

                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[14, Cam_Num])
                            {
                                t_exist_str = "Bevelling Measurement";
                                dgvCmbCell8.Items.Add(t_exist_str);
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[15, Cam_Num])
                            {
                                t_exist_str = "AI Inspection";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[16, Cam_Num])
                            {
                                t_exist_str = "SSF";
                                dgvCmbCell8.Items.Add(t_exist_str);//6
                                if (tstr == t_exist_str)
                                {
                                    t_exist_check = true;
                                }
                            }

                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[17, Cam_Num])
                            {
                                if (main_str.Contains("Model find"))
                                {
                                    t_exist_str = "Match rate(%)";
                                    dgvCmbCell8.Items.Add(t_exist_str);
                                    if (tstr == t_exist_str)
                                    {
                                        t_exist_check = true;
                                    }
                                }
                            }

                            if (!t_exist_check)
                            {
                                tstr = t_exist_str;
                            }
                            if (!tstr.Contains("Hor. length") && !tstr.Contains("Ver. length")
                                && !tstr.Contains("Concentricity") && !tstr.Contains("V Angle of bottom")
                                && !tstr.Contains("Pitch of thread") && !tstr.Contains("Size of thread")
                                && !tstr.Contains("Lead angle of thread(1)") && !tstr.Contains("Lead angle of thread(0.5)")
                                && !tstr.Contains("Thickness of body(mm)") && !tstr.Contains("Bending of body(mm)")
                                && !tstr.Contains("Brightness of rectangle ROI") && !tstr.Contains("BLOB in rectangle ROI")
                                && !tstr.Contains("Shape of bottom") && !tstr.Contains("Distance between two area") && !tstr.Contains("Match rate(%)")
                                && !tstr.Contains("Bevelling Measurement") && !tstr.Contains("AI Inspection") && !tstr.Contains("SSF")
                                && tstr.Length != 0)
                            {
                                tstr = "Hor. length";
                            }
                        }
                    }

                    dataGridView1.Rows[8].Cells[1] = dgvCmbCell8;
                    if (tstr != "" && text_maintain)
                    {
                        if (!main_str.Contains("Model find") && tstr.Contains("Match rate(%)"))
                        {
                            dataGridView1.Rows[8].Cells[1].Value = "Hor. length";
                        }
                        else
                        {
                            dataGridView1.Rows[8].Cells[1].Value = tstr;
                        }
                    }
                    // -------------------------------------------------------------------------
                    // 9번 측정 방법 선택
                    tstr = dataGridView1.Rows[9].Cells[1].Value.ToString();
                    dataGridView1.Rows[9].Cells[1].Value = null;
                    DataGridViewComboBoxCell dgvCmbCell9 = new DataGridViewComboBoxCell();
                    dgvCmbCell9.Items.Add("MIN");
                    dgvCmbCell9.Items.Add("MAX");
                    dgvCmbCell9.Items.Add("MAX-MIN");
                    dgvCmbCell9.Items.Add("AVG");
                    dgvCmbCell9.Items.Add("TOTAL");
                    dataGridView1.Rows[9].Cells[1] = dgvCmbCell9;
                    if (!tstr.Contains("MIN") && !tstr.Contains("MAX") && !tstr.Contains("MAX-MIN") && !tstr.Contains("AVG") && !tstr.Contains("TOTAL"))
                    {
                        tstr = "AVG";
                    }
                    if (tstr != "" && text_maintain)
                    {
                        dataGridView1.Rows[9].Cells[1].Value = tstr;
                    }
                    // -------------------------------------------------------------------------
                    // 10, 11번 측정 하위,상위
                    // -------------------------------------------------------------------------
                    // 12번부터 알고리즘에 따른 변수값 설정
                    tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                    if (tstr == "Hor. length" || tstr == "Ver. length")
                    {// 0번, 1번
                        dataGridView1.Rows[12].Cells[0].Value = "Angle(Degree)";
                        dataGridView1.Rows[13].Cells[0].Value = "Limit min length(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "Limit max length(mm)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "Filter size for noise reduction";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 100)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[16].Cells[0].Value = "Edge Preprocessing(0:No,1:Use)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 1;
                        }

                        if (tstr == "Hor. length")
                        {
                            dataGridView1.Rows[17].Cells[0].Value = "Method(0:Directional,1:End of BLOBs,2:Line Gauge,3:ROI left BLOB right,4:ROI right BLOB left)";
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) >= 4)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 4;
                            }
                        }
                        else if (tstr == "Ver. length")
                        {
                            dataGridView1.Rows[17].Cells[0].Value = "Method(0:Directional,1:End of BLOBs,2:Line Gauge,3:Bottom of BLOBs,4:ROI top BLOB bottom,5:ROI bottom BLOB top)";
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) >= 5)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 5;
                            }
                        }

                    }
                    else if (tstr == "Distance between two area")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Direction(0:Hor.,1:Ver.)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[13].Cells[0].Value = "ROI#1 X";
                        dataGridView1.Rows[14].Cells[0].Value = "ROI#1 Y";
                        dataGridView1.Rows[15].Cells[0].Value = "ROI#1 W";
                        dataGridView1.Rows[16].Cells[0].Value = "ROI#1 H";
                        dataGridView1.Rows[17].Cells[0].Value = "ROI#2 X";
                        dataGridView1.Rows[18].Cells[0].Value = "ROI#2 Y";
                        dataGridView1.Rows[19].Cells[0].Value = "ROI#2 W";
                        dataGridView1.Rows[20].Cells[0].Value = "ROI#2 H";
                    }
                    else if (tstr == "Dim. of cross")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Method(0:cross,1:Polygon,2:Inner Max Diameter)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Number of angle";
                        }
                        else
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Preliminary";
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) != 2)
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "Output(0:Radius,1:Diameter)";
                            if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[14].Cells[1].Value = 0;
                            }
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) == 0 && Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[15].Cells[0].Value = "Method(0:All,1:Left Top to Right Bottom,2:Left Bottom to Right Top)";
                            if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[15].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 2)
                            {
                                dataGridView1.Rows[15].Cells[1].Value = 2;
                            }
                        }
                        else
                        {
                            dataGridView1.Rows[15].Cells[0].Value = "Preliminary";
                        }
                    }
                    else if (tstr == "Concentricity") // 번
                    {
                        //dataGridView1.Rows[5].Cells[1].Value = "Insp. area result use";
                    }
                    else if (tstr == "Brightness of rectangle ROI")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "Method(0:Gray,1:Color)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Low Thres.(0~360)";
                            dataGridView1.Rows[14].Cells[0].Value = "High Thres.(0~360)";
                            dataGridView1.Rows[15].Cells[0].Value = "Filter size for noise reduction";
                            dataGridView1.Rows[16].Cells[0].Value = "Min. size of BLOB";
                            dataGridView1.Rows[17].Cells[0].Value = "Max. size of BLOB";
                            if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) == 0)
                            {
                                dataGridView1.Rows[17].Cells[1].Value = 100000;
                            }
                            dataGridView1.Rows[18].Cells[0].Value = "Output(0:Brightness,1:Pixel CNT)";
                            if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[18].Cells[1].Value = 0;
                            }
                            dataGridView1.Rows[19].Cells[0].Value = "Blur Filter";
                        }
                        else
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Blur Filter";
                            //dataGridView1.Rows[13].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[14].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[15].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[16].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[17].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[18].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[13].Cells[1].Value = "0";
                            dataGridView1.Rows[14].Cells[1].Value = "0";
                            dataGridView1.Rows[15].Cells[1].Value = "0";
                            dataGridView1.Rows[16].Cells[1].Value = "0";
                            dataGridView1.Rows[17].Cells[1].Value = "0";
                            dataGridView1.Rows[18].Cells[1].Value = "0";
                        }
                    }
                    else if (tstr == "Brightness of circle ROI")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "Diameter of #1 circle(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "Thickness of #1 circle(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "Diameter of #2 circle(mm)";
                        dataGridView1.Rows[15].Cells[0].Value = "Thickness of #2 circle(mm)";
                        dataGridView1.Rows[16].Cells[0].Value = "Output(0:#1 brightness,1:Difference brightnesss #1,2)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "Preliminary";
                            dataGridView1.Rows[15].Cells[0].Value = "Preliminary";
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[14].Cells[0].Value = "Diameter of #2 circle(mm)";
                            dataGridView1.Rows[15].Cells[0].Value = "Thickness of #2 circle(mm)";
                        }
                    }
                    else if (tstr == "BLOB in circle ROI" || tstr == "BLOB count in circle ROI")
                    {
                        int tt_idx = 12;
                        // 변수 추가
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Diameter of #1 circle(mm)"; tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Thickness of #1 circle(mm)"; tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Start(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "End(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 360;
                        }
                        tt_idx = 14;
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) == 0 && Convert.ToInt32(dataGridView1.Rows[tt_idx + 1].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                            dataGridView1.Rows[tt_idx + 1].Cells[1].Value = 360;
                        }
                        tt_idx = 16;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Blur Filter";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Erode";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 100;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Dilate";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "MP(0:Erode->Dilate,1:Dilate->Erode)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 1)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 1;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }

                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Size Min(Pixel)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Size Max(Pixel)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 100000;
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Location(0:ROI#0,1:P Below,2:P Above)";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 2)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        tt_idx++;
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx - 1].Cells[1].Value.ToString()) < 3)
                        {
                            dataGridView1.Rows[tt_idx].Cells[0].Value = "P threshold";
                            if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 255)
                            {
                                dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                            }
                        }
                        tt_idx++;
                        dataGridView1.Rows[tt_idx].Cells[0].Value = "Output(0:Pixel,1:Count,2:Major,3:Minor,4:Axis angle,5:Angle(from center),6:Included angle,7:Pixel(except boundary),8:Pixel(boundary only))";
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 8)
                        {
                            dataGridView1.Rows[tt_idx].Cells[1].Value = 8;
                        }

                        //if (Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) >= 1 && Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) <= 6)
                        //{
                        //    if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) >= 500)
                        //    {
                        //        dataGridView1.Rows[19].Cells[1].Value = 0;
                        //    }
                        //    if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) >= 500)
                        //    {
                        //        dataGridView1.Rows[20].Cells[1].Value = 500;
                        //    }
                        //}
                        tt_idx++;
                        //dataGridView1.Rows[tt_idx].Cells[0].Value = "옵션(0:미사용,1:볼록 BLOB 차이,2:영역1(v1이하) 영역2(v2이상) 임계화)";
                        //if (Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[tt_idx].Cells[1].Value.ToString()) <= -1)
                        //{
                        dataGridView1.Rows[tt_idx].Cells[1].Value = 0;
                        //}
                    }
                    else if (tstr == "Color BLOB in circle ROI")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "Diameter of circle(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "Thickness of circle(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "Filter size for preprocessing";
                        dataGridView1.Rows[15].Cells[0].Value = "Low Thres.(0~360)";
                        dataGridView1.Rows[16].Cells[0].Value = "High Thres.(0~360)";
                        //dataGridView1.Rows[17].Cells[0].Value = "Filter size for noise reduction";
                        dataGridView1.Rows[17].Cells[0].Value = "Min. size of BLOB";
                        dataGridView1.Rows[18].Cells[0].Value = "Max. size of BLOB";
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[19].Cells[0].Value = "Output(0:Brightness,1:Pixel CNT,2:Group CNT)";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[20].Cells[0].Value = "Position(0:ROI#0,1:P Below,2:P Above,3:Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) > 3)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 2)
                        {
                            dataGridView1.Rows[21].Cells[0].Value = "P Threshold";
                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 255)
                            {
                                dataGridView1.Rows[21].Cells[1].Value = 0;
                            }
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) == 3)
                        {
                            dataGridView1.Rows[21].Cells[0].Value = "Angle";
                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 360)
                            {
                                dataGridView1.Rows[21].Cells[1].Value = 360;
                            }
                        }
                    }
                    else if (tstr == "BLOB in rectangle ROI" || tstr == "BLOB count in rect ROI(Count)")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Preprocessing(Blur)";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB";
                        dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "Usage Filter(0:No,1:Yes,2:Color+No,3:Color+Yes)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[16].Cells[0].Value = "Direc. Filter(0:All,1:Hor.,2:Ver.)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "Filter Count(CNT)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 1;
                        }
                        dataGridView1.Rows[18].Cells[0].Value = "Erode";
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) > 500)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 500;
                        }
                        dataGridView1.Rows[19].Cells[0].Value = "Dilate";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 500)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 500;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[20].Cells[0].Value = "Method(0:Erode->Dilate,1:Dilate->Erode)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) > 1)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 1;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[21].Cells[0].Value = "Convex BLOB(0:No,1:Use)";
                        if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[21].Cells[1].Value = 0;
                        }
                        if (tstr == "BLOB in rectangle ROI")
                        {
                            //"계산방법(0:픽셀수,1:가로길이,2:세로길이,3:갯수,4:기준점부터 거리,5:픽셀수(가장자리 제외),6:픽셀수(가장자리),7:좌우,상하 두께 차이,8:배터리팩Align)";
                            dataGridView1.Rows[22].Cells[0].Value = "Output(0:Pixel,1:Width,2:Height,3:Count,4:Position,5:Pixel(except boundary),6:Pixel(boundary only),7:LR_TB Thickness Diff,8:Batt.Pack Alignment,9:Omit)";
                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[22].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) > 8)
                            {
                                dataGridView1.Rows[22].Cells[1].Value = 8;
                            }

                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) == 1)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. of Width";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. of Width";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) == 2)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. of Height";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. of Height";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) == 3)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) == 4)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. Distance";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. Distance";
                            }
                        }
                    }
                    else if (tstr == "Bevelling Measurement")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction";
                        dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB";
                        dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[20].Cells[0].Value = "Preprocessing(Blur)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[16].Cells[0].Value = "Output(0:Angle of Both Sides, 1:Width of Both Sides, 2:Height of Both Sides, 3:Length of Both Sides)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 3)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 3;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "Location(0:Top, 1:Bottom, 2:Left, 3:Right)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) >= 3)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 3;
                        }
                    }
                    else if (tstr == "Diameter")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction";
                        dataGridView1.Rows[13].Cells[0].Value = "Min lenght of diameter";
                        dataGridView1.Rows[14].Cells[0].Value = "Max lenght of diameter";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
                        }

                        dataGridView1.Rows[15].Cells[0].Value = "Method(0:Diameter,1:Min. dist. from center,2:Max. dist. from center,3:Max.-Min. dist. from center,4:Distance Transform,5:Estimate Diameter)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 5)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 5;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) == 5)
                        {
                            dataGridView1.Rows[16].Cells[0].Value = "Direction(0:Left->Right,1:Right->Left,2:Top->Bottom,3:Bottom->Top)";
                            if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[16].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) >= 3)
                            {
                                dataGridView1.Rows[16].Cells[1].Value = 3;
                            }
                        }

                    }
                    else if (tstr == "Circularity(%)")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction";
                        dataGridView1.Rows[13].Cells[0].Value = "Min lenght of diameter";
                        dataGridView1.Rows[14].Cells[0].Value = "Max lenght of diameter";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "Method(0:%,1:100-%)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) >= 1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 1;
                        }
                    }
                    else if (tstr == "V Angle of bottom") // 3번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Height for cal.(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "Filter size for removing thread";
                    }
                    else if (tstr == "Bending of body(mm)" || tstr == "Thickness of body(mm)") // 7, 8번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for removing thread";
                        if (tstr == "Bending of body(mm)" && comboBox_TABLETYPE.SelectedIndex == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Output(0:Diff.Dist.,1:Body Angle,2:Top Angle,3:L-R Angle Diff.,4:Top-Bottom Angle)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 4)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 4;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) == 3)
                            {
                                dataGridView1.Rows[14].Cells[0].Value = "L-R Angle Diff.(0:Difference,1:Left angle,2:Right angle)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 0;
                                }
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) >= 2)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 2;
                                }
                            }
                            else
                            {
                                dataGridView1.Rows[14].Cells[0].Value = "Preliminary";
                                dataGridView1.Rows[14].Cells[1].Value = 0;
                            }

                        }
                        else if (tstr == "Thickness of body(mm)" && comboBox_TABLETYPE.SelectedIndex == 1)
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Output(0:Thickness,1:Effective Diameter,2:Parallelism,3:Dan-height)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= 0)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 0;
                            }
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) >= 3)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 3;
                            }
                        }
                    }
                    else if (tstr == "Shape of bottom")
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for removing thread"; // 이름
                        dataGridView1.Rows[13].Cells[0].Value = "Height for cal.(mm)"; // 이름
                    }
                    else if (tstr.Contains("thread"))
                    {
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "Preventing paste thread(Count)"; // 이름
                        if (tstr.Contains("Size of thread"))
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Find thread(Count)"; // 이름
                            dataGridView1.Rows[14].Cells[0].Value = "Method(0:Pixel,1:Width,2:Height,3:ConvexBLOB)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[14].Cells[1].Value = 0;
                            }
                        }
                    }
                    else if (tstr == "Convex BLOB Difference")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for removing thread";
                        dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB";
                        dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "Method(0:Pixel,1:Width,2:Height)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                    }
                    else if (tstr == "Center difference between Inner and outter circle")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Inner Circle Thershold";
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 255)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 255;
                        }
                        dataGridView1.Rows[13].Cells[0].Value = "Inner Circle Threshold Method(0:Below,1:Above)";
                        if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[13].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[14].Cells[0].Value = "Filter size for noise reduction";
                        dataGridView1.Rows[15].Cells[0].Value = "Filter size for merging BLOB";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[16].Cells[0].Value = "Output(0:mm,1:Pixel,2:In Diam.(mm),3:Out Diam.(mm),4:Out-In Diam.(mm))";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 4 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "Distance for inner circle(mm)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 0;
                        }
                    }
                    else if (tstr == "AI Inspection")
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Matching Rate[0,1]";
                        if (Convert.ToDouble(dataGridView1.Rows[12].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[12].Cells[1].Value = 0;
                        }
                        //if (Convert.ToDouble(dataGridView1.Rows[12].Cells[1].Value.ToString()) > 1)
                        //{
                        //    dataGridView1.Rows[12].Cells[1].Value = 1;
                        //}
                        dataGridView1.Rows[13].Cells[0].Value = "Model Load";
                        if (listBox1_SelectedIndex > 0)
                        {
                            if (LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num, listBox1_SelectedIndex - 1])
                            {
                                dataGridView1.Rows[13].Cells[1].Value = "1";
                            }
                            else
                            {
                                dataGridView1.Rows[13].Cells[1].Value = "0";
                            }
                        }
                        dataGridView1.Rows[14].Cells[0].Value = "Model Train";
                        dataGridView1.Rows[15].Cells[0].Value = "ROI Image Save";
						dataGridView1.Rows[14].Cells[1].Value = "0";
                        dataGridView1.Rows[15].Cells[1].Value = "0";
                    }
                    else if (tstr == "SSF")
                    {
                        int t_Start_Idx = 12;
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Lifting X(%)";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0.1;
                        }
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 100;
                        }
                        t_Start_Idx++;//13
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Lifting Y(%)";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0.1;
                        }
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 100;
                        }
                        t_Start_Idx++;//14
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Defect SSF X";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//15
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Defect SSF Y";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//16
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Base SSF X";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//17
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Base SSF Y";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 1)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 1;
                        }
                        t_Start_Idx++;//18
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Opened";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//19
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Closed";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//20
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "GD Dark";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//21
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "GD Bright";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//22
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Size Dark";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//23
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Size Bright";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        t_Start_Idx++;//24
                        dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Output(0:Count,1:Size,2:Omit,3:AI(Designation class),4:AI(Other than the designated class),5:AI(All),6:AI from result";
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                        }
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 6)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 6;
                        }
                        t_Start_Idx++;//25
                        if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx - 1].Cells[1].Value.ToString()) >= 3 && Convert.ToDouble(dataGridView1.Rows[t_Start_Idx - 1].Cells[1].Value.ToString()) <= 6)
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "AI Class Number";
                            if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) < 0)
                            {
                                dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 0;
                            }
                            if (Convert.ToDouble(dataGridView1.Rows[t_Start_Idx].Cells[1].Value.ToString()) > 41)
                            {
                                dataGridView1.Rows[t_Start_Idx].Cells[1].Value = 40;
                            }

                            //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_AI_Model_Loaded(Cam_Num, listBox1_SelectedIndex))
                            //{
                            //    dataGridView1.Rows[t_Start_Idx].Cells[1].Value = "1";
                            //}
                            //else
                            //{
                            //    dataGridView1.Rows[t_Start_Idx].Cells[1].Value = "0";
                            //}

                        }
                        else
                        {
                            dataGridView1.Rows[t_Start_Idx].Cells[0].Value = "Preliminary";
                        }
                    }

                    if (comboBox_TABLETYPE.SelectedIndex == 5)
                    {
                        if (listBox1.SelectedIndex == 0)
                        {
                            for (int i = 12; i < dataGridView1.RowCount; i++)
                            {
                                dataGridView1.Rows[i].Cells[0].Value = "Preliminary";
                                dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                            }
                            dataGridView1.Rows[10].Cells[1].Style.BackColor = Color.DimGray;
                            dataGridView1.Rows[11].Cells[1].Style.BackColor = Color.DimGray;

                            dataGridView1.Rows[12].Cells[0].Value = "Custome No.";
                            if (dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && Cam_Num == 0)
                            {
                                comboBox_CAMPOSITION.SelectedIndex = 0;
                                dataGridView1.Rows[13].Cells[0].Value = "DJ min size(Pixel)";
                                dataGridView1.Rows[14].Cells[0].Value = "Slit min size(Pixel)";
                                dataGridView1.Rows[15].Cells[0].Value = "DJ min height(Pixel)";
                                dataGridView1.Rows[16].Cells[0].Value = "DJ max height(Pixel)";
                                dataGridView1.Rows[17].Cells[0].Value = "Tol. of DJ bending degree(Degree)";
                                dataGridView1.Rows[18].Cells[0].Value = "Slit Chip Threshold(Gray,less than)";
                                dataGridView1.Rows[19].Cells[0].Value = "DJ birdcaging min size(Pixel)";
                                dataGridView1.Rows[20].Cells[0].Value = "Restore data(1 type)";
                                if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) == 1)
                                {
                                    dataGridView1.Rows[13].Cells[1].Value = 50;
                                    dataGridView1.Rows[14].Cells[1].Value = 30;
                                    dataGridView1.Rows[15].Cells[1].Value = 50;
                                    dataGridView1.Rows[16].Cells[1].Value = 70;
                                    dataGridView1.Rows[17].Cells[1].Value = 5;
                                    dataGridView1.Rows[18].Cells[1].Value = 150;
                                    dataGridView1.Rows[19].Cells[1].Value = 10;
                                    dataGridView1.Rows[20].Cells[1].Value = 0;
                                }
                                int t_idx = 1;
                                listBox1.Items[t_idx] = "Inner Diameter";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Inner Diameter"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Diameter";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Diameter"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Width Min(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Width Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Width Max(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Width Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Height Min(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Height Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Height Max(mm)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Height Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Angle(degree)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Angle(degree)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ-Slit Angle(degree)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ-Slit Angle(degree)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Angle(degree)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Angle(degree)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Count(Number)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Count(Number)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Bending Count(Number)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Bending Count(Number)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Count(Number)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Count(Number)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Defect Size(pixel)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Defect Size(pixel)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Birdcaging(Number)";
                                LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Birdcaging(Number)"; t_idx++;

                                //LVApp.Instance().m_Config.Save_Judge_Data();
                            }
                            else if (dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && Cam_Num != 0)
                            {
                                if (LVApp.Instance().m_mainform.m_Start_Check && text_maintain)
                                {
                                    AutoClosingMessageBox.Show("Only CAM0 supported for SUGIYAMA ", "WARNING", 3000);
                                }
                            }
                            else
                            {
                                if (LVApp.Instance().m_mainform.m_Start_Check && text_maintain && Cam_Num == 0)
                                {
                                    AutoClosingMessageBox.Show("Invalide customer number or misstyped.", "WARNING", 3000);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 10; i < dataGridView1.RowCount; i++)
                            {
                                dataGridView1.Rows[i].Cells[0].Value = "Preliminary";
                                dataGridView1.Rows[i].Cells[1].Value = 0;
                                dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                            }
                        }
                    }
                    else
                    {
                        dataGridView1.Rows[10].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                        dataGridView1.Rows[11].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                    }

                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[5].Selected = true;
                    CurrencyManager currencyManager0 = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
                    currencyManager0.SuspendBinding();

                    //for (int i = 12; i < dataGridView1.RowCount; i++)
                    //{
                    //    dataGridView1.Rows[i].Visible = true;
                    //    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary" || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. Low bound")
                    //        || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. High bound"))
                    //    {
                    //        dataGridView1.Rows[i].Cells[1].Value = 0;
                    //        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                    //        dataGridView1.Rows[i].Visible = false;
                    //    }
                    //    else if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "Custome No.")
                    //    {
                    //        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.SkyBlue;
                    //    }
                    //    else
                    //    {
                    //        dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightPink;
                    //    }
                    //}


                    if (m_advenced_param_visible)
                    {
                        for (int i = 10; i < dataGridView1.RowCount; i++)
                        {
                            dataGridView1.Rows[i].Visible = true;
                            dataGridView1.Rows[i].Cells[1].ReadOnly = false;
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
                            {
                                dataGridView1.Rows[i].Visible = false;
                            }
                            else
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("하위 측정범위")
                                || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("상위 측정범위")
                                || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. Low bound")
                                || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. High bound"))
                                {
                                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                                }
                                else
                                {
                                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightPink;
                                }
                            }
                            
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "모델 불러오기" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Model Load"
                             || dataGridView1.Rows[i].Cells[0].Value.ToString() == "모델 학습" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Model Train"
                             || dataGridView1.Rows[i].Cells[0].Value.ToString() == "ROI 이미지 저장" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "ROI Image Save"
                             )
                            {
                                dataGridView1.Rows[i].Cells[1].ReadOnly = true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 10; i < dataGridView1.RowCount; i++)
                        {
                            dataGridView1.Rows[i].Visible = false;
                        }
                    }
                    dataGridView1.Rows[0].Visible = false;
                    currencyManager0.ResumeBinding();
                }
                //if (text_maintain)
                //{
                //    string t_str = dataGridView1.Rows[5].Cells[1].Value.ToString(); //임계화 방법
                //    if (t_str == "v1 이하" || t_str == "v1 less than")
                //    {
                //        dataGridView1.Rows[6].Visible = true;
                //        dataGridView1.Rows[7].Visible = false;
                //    }
                //    else if (t_str == "v2 이상" || t_str == "v2 more than")
                //    {
                //        dataGridView1.Rows[6].Visible = false;
                //        dataGridView1.Rows[7].Visible = true;
                //    }
                //    else if (t_str == "자동이하" || t_str == "자동이상" || t_str == "Auto less than" || t_str == "Auto more than"
                //          || t_str == "검사 영역 결과 사용" || t_str == "모델 사용" || t_str == "Insp. area result use" || t_str == "Model find")
                //    {
                //        dataGridView1.Rows[6].Visible = false;
                //        dataGridView1.Rows[7].Visible = false;
                //    }
                //    else
                //    {
                //        dataGridView1.Rows[6].Visible = true;
                //        dataGridView1.Rows[7].Visible = true;
                //    }
                //}

                dataGridView1.Refresh();
                pictureBox_Image.Refresh();
                if (text_maintain)
                {
                    SubDB_to_MainDB();
                }
            }
            catch
            {
            }
        }

        private void pictureBox_RImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!t_ViewGrayR)
            {
                return;
            }
            t_x = e.X;
            t_y = e.Y;
            if (pictureBox_RImage.Image != null)
            {
                // Declare a Bitmap
                //Bitmap mybitmap;
                // Load Picturebox image to bitmap
                //mybitmap = new Bitmap(pictureBox_Image.Image);
                // In the mouse move event
                int tt_x = (int)((double)t_x * (double)pictureBox_RImage.Image.Width / (double)pictureBox_RImage.Width);
                int tt_y = (int)((double)t_y * (double)pictureBox_RImage.Image.Height / (double)pictureBox_RImage.Height);
                if (tt_x >= 0 && tt_x < pictureBox_RImage.Image.Width && tt_y >= 0 && tt_y < pictureBox_RImage.Image.Height)
                {
                    var pixelcolor = ((Bitmap)pictureBox_RImage.Image).GetPixel(tt_x, tt_y);
                    // Displays R  / G / B Color
                    t_xyg = "[G" + pixelcolor.R.ToString("000") + "(X" + tt_x.ToString() + ",Y" + tt_y.ToString() + ")]";
                    pictureBox_RImage.Refresh();
                }
            }
        }

        private void pictureBox_RImage_Paint(object sender, PaintEventArgs e)
        {
            if (t_ViewGrayR)
            {
                using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(125, 125, 125, 125)))
                {
                    if (t_x + 10 + 115 > pictureBox_RImage.Width)
                    {
                        e.Graphics.FillRectangle(myBrush, new Rectangle(t_x - 125, t_y - 10, 115, 21));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(myBrush, new Rectangle(t_x + 10, t_y - 10, 115, 21));
                    }
                }

                using (Font myFont = new Font("Arial", 9))
                {
                    if (t_x + 10 + 115 > pictureBox_RImage.Width)
                    {
                        e.Graphics.DrawString(t_xyg, myFont, Brushes.Yellow, new System.Drawing.Point(t_x - 122, t_y - 8));
                    }
                    else
                    {
                        e.Graphics.DrawString(t_xyg, myFont, Brushes.Yellow, new System.Drawing.Point(t_x + 13, t_y - 8));
                    }
                }

                Pen pen1 = new Pen(Color.FromArgb(125, 0, 255, 0), 1);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                System.Drawing.Point VP1 = new System.Drawing.Point(t_x - 9, t_y);
                System.Drawing.Point VP2 = new System.Drawing.Point(t_x + 9, t_y);
                e.Graphics.DrawLine(pen1, VP1, VP2);
                System.Drawing.Point HP1 = new System.Drawing.Point(t_x, t_y - 9);
                System.Drawing.Point HP2 = new System.Drawing.Point(t_x, t_y + 9);
                e.Graphics.DrawLine(pen1, HP1, HP2);
                pen1.Dispose();
            }

            if (t_AI_Result.Length > 0)
            {
                using (Font myFont = new Font("Arial", 10))
                {
                    e.Graphics.DrawString(t_AI_Result, myFont, Brushes.Red, new System.Drawing.Point(15, pictureBox_RImage.Height - 30));
                }
            }
        }

        private int t_current_row_idx = 0;
        private int t_current_col_idx = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //return;
                t_current_row_idx = e.RowIndex;
                t_current_col_idx = e.ColumnIndex;
                //if (t_current_col_idx == 0)
                //{
                //    return;
                //}
                //if (e.RowIndex >= 1 && e.RowIndex <= 4)
                //{
                //    radioButton1.Checked = true;
                //    radioButton2.Checked = false;
                //}
                //if (e.RowIndex == 6 || e.RowIndex == 7 || e.RowIndex >= 10)
                //{
                //    radioButton1.Checked = false;
                //    radioButton2.Checked = true;

                //    if (e.RowIndex == 6 || e.RowIndex == 7 || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("어두운 임계값(Gray)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("밝은 임계값(Gray)")
                //        || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Dark Thres.(Gray)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Bright Thres.(Gray)"))
                //    {
                //        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                //        t_Frm_Trackbar.t_idx = e.RowIndex;
                //        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                //        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                //        t_Frm_Trackbar.TopLevel = true;

                //        t_Frm_Trackbar.colorSlider.Minimum = 0;
                //        t_Frm_Trackbar.colorSlider.Maximum = 255;
                //        if (Cam_Num == 0)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        t_Frm_Trackbar.Show();
                //    }

                //    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("컬러 임계") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low Thres") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High Thres")
                //         || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("시작각(Angle)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("종료각(Angle)")
                //         || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Start(Angle)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("End(Angle)")
                //        )
                //    {
                //        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                //        t_Frm_Trackbar.t_idx = e.RowIndex;
                //        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                //        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                //        t_Frm_Trackbar.TopLevel = true;

                //        t_Frm_Trackbar.colorSlider.Minimum = -360;
                //        t_Frm_Trackbar.colorSlider.Maximum = 360;
                //        if (Cam_Num == 0)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        t_Frm_Trackbar.Show();
                //    }

                //    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("검사대상크기") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Min. size of BLOB") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Max. size of BLOB"))
                //    {
                //        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                //        t_Frm_Trackbar.t_idx = e.RowIndex;
                //        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                //        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                //        t_Frm_Trackbar.TopLevel = true;
                //        if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하한") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Min"))
                //        {
                //            t_Frm_Trackbar.colorSlider.Minimum = 0;
                //            t_Frm_Trackbar.colorSlider.Maximum = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height / 10;
                //        }
                //        else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상한") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Max"))
                //        {
                //            t_Frm_Trackbar.colorSlider.Minimum = 0;
                //            t_Frm_Trackbar.colorSlider.Maximum = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                //        }

                //        if (Cam_Num == 0)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        t_Frm_Trackbar.Show();
                //    }
                //    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("지름(mm)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Diameter of")
                //        || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("두께(mm)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Thickness of"))
                //    {
                //        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                //        t_Frm_Trackbar.t_idx = e.RowIndex;
                //        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                //        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                //        t_Frm_Trackbar.TopLevel = true;

                //        double cam_resol = 1d;
                //        if (Cam_Num == 0)
                //        {
                //            double.TryParse(LVApp.Instance().m_mainform.ctr_Camera_Setting1.textBox_RESOLUTION_X.Text, out cam_resol);
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            double.TryParse(LVApp.Instance().m_mainform.ctr_Camera_Setting2.textBox_RESOLUTION_X.Text, out cam_resol);
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            double.TryParse(LVApp.Instance().m_mainform.ctr_Camera_Setting3.textBox_RESOLUTION_X.Text, out cam_resol);
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            double.TryParse(LVApp.Instance().m_mainform.ctr_Camera_Setting4.textBox_RESOLUTION_X.Text, out cam_resol);
                //        }

                //        if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("지름(mm)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Diameter of"))
                //        {
                //            t_Frm_Trackbar.t_Precision = cam_resol;
                //            t_Frm_Trackbar.colorSlider.Minimum = 0;
                //            int t_max_v = Math.Max(pictureBox_Image.Image.Width, pictureBox_Image.Image.Height);
                //            t_Frm_Trackbar.colorSlider.Maximum = (int)((double)t_max_v);
                //            t_Frm_Trackbar.colorSlider.ShowDivisionsText = true;
                //            t_Frm_Trackbar.colorSlider.TickDivide = 1 / (float)cam_resol;
                //        }
                //        else
                //        {
                //            t_Frm_Trackbar.t_Precision = cam_resol;
                //            t_Frm_Trackbar.colorSlider.Minimum = 0;
                //            float t_max_v = Math.Max(pictureBox_Image.Image.Width, pictureBox_Image.Image.Height); //float.Parse(dataGridView1.Rows[e.RowIndex - 1].Cells[1].Value.ToString());
                //            t_Frm_Trackbar.colorSlider.Maximum = (int)t_max_v;//(int)((double)t_max_v * cam_resol/2);
                //            t_Frm_Trackbar.colorSlider.ShowDivisionsText = true;
                //            t_Frm_Trackbar.colorSlider.TickDivide = 1 / (float)cam_resol;
                //        }
                //        if (Cam_Num == 0)
                //        {
                //            //MessageBox.Show(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                //            t_Frm_Trackbar.colorSlider.Value = (float.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = (float.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = (float.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            t_Frm_Trackbar.colorSlider.Value = (float.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                //        }
                //        t_Frm_Trackbar.Show();
                //    }

                //    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("측정범위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low bound") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High bound"))
                //    {
                //        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                //        t_Frm_Trackbar.t_idx = e.RowIndex;
                //        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                //        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                //        t_Frm_Trackbar.TopLevel = true;

                //        if (Cam_Num == 0)
                //        {
                //            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = 0;
                //                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                //            }
                //            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                //                t_Frm_Trackbar.colorSlider.Maximum = 100;
                //            }
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 1)
                //        {
                //            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = 0;
                //                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                //            }
                //            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                //                t_Frm_Trackbar.colorSlider.Maximum = 100;
                //            }
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 2)
                //        {
                //            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = 0;
                //                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                //            }
                //            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                //                t_Frm_Trackbar.colorSlider.Maximum = 100;
                //            }
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        else if (Cam_Num == 3)
                //        {
                //            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = 0;
                //                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                //            }
                //            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                //            {
                //                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                //                t_Frm_Trackbar.colorSlider.Maximum = 100;
                //            }
                //            t_Frm_Trackbar.colorSlider.Value = float.Parse(LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                //        }
                //        t_Frm_Trackbar.Show();
                //    }
                //}
            }
            catch
            {

            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        private void pictureBox_Image_DoubleClick(object sender, EventArgs e)
        {
            button_INSPECTION_Click(sender, e);
            return;
            ////Referesh_Select_Menu(true);
            ////System.Drawing.Point loc = dataGridView1.PointToScreen(System.Drawing.Point.Empty);

            ////uint X = (uint)loc.X + 5 + 100;
            ////uint Y = (uint)loc.Y + 5;
            ////SetCursorPos((int)X, (int)Y);
            ////mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            ////mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);

            ////if (LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.t_realtime_check
            ////     || LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.t_realtime_check
            ////     || LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.t_realtime_check
            ////     || LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.t_realtime_check
            ////     )
            ////{
            ////    return;
            ////}

            //pictureBox_RImage.Image = pictureBox_Image.Image.Clone() as Bitmap;
            //pictureBox_RImage.Refresh();

            ////if (!m_roi_click_auto_mode)
            ////{
            ////    m_roi_click_auto_mode = checkBox_AutoInspection.Checked = true;
            ////}
           
            //if (LVApp.Instance().m_Config.ROI_Cam_Num == 0)
            //{
            //    if (//!LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && 
            //        !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //    {
            //        //pictureBox_Image.Refresh();
            //        //LVApp.Instance().m_mainform.ctr_ROI1.
            //        SubDB_to_MainDB();
            //        //LVApp.Instance().m_Config.Set_Parameters();
            //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
            //        //LVApp.Instance().m_mainform.ctr_ROI1.
            //        button_INSPECTION_Click(sender, e);
            //        //radioButton1.Checked = false;
            //        //radioButton2.Checked = true;
            //    }
            //}
            //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 1)
            //{
            //    if (//!LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.timer_Camera.Enabled && 
            //        !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //    {
            //        //pictureBox_Image.Refresh();
            //        //LVApp.Instance().m_mainform.ctr_ROI2.
            //        SubDB_to_MainDB();
            //        //LVApp.Instance().m_Config.Set_Parameters();
            //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
            //        //LVApp.Instance().m_mainform.ctr_ROI2.
            //        button_INSPECTION_Click(sender, e);
            //        //radioButton1.Checked = false;
            //        //radioButton2.Checked = true;
            //    }
            //}
            //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 2)
            //{
            //    if (//!LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.timer_Camera.Enabled && 
            //        !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //    {
            //        //pictureBox_Image.Refresh();
            //        //LVApp.Instance().m_mainform.ctr_ROI3.
            //        SubDB_to_MainDB();
            //        //LVApp.Instance().m_Config.Set_Parameters();
            //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
            //        //LVApp.Instance().m_mainform.ctr_ROI3.
            //        button_INSPECTION_Click(sender, e);
            //        //radioButton1.Checked = false;
            //        //radioButton2.Checked = true;
            //    }
            //}
            //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 3)
            //{
            //    if (//!LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.timer_Camera.Enabled && 
            //        !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //    {
            //        //pictureBox_Image.Refresh();
            //        //LVApp.Instance().m_mainform.ctr_ROI4.
            //        SubDB_to_MainDB();
            //        //LVApp.Instance().m_Config.Set_Parameters();
            //        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex, LVApp.Instance().m_Config.ROI_Cam_Num);
            //        //LVApp.Instance().m_mainform.ctr_ROI4.
            //        button_INSPECTION_Click(sender, e);
            //        //radioButton1.Checked = false;
            //        //radioButton2.Checked = true;
            //    }
            //}

            ////pictureBox_RImage.Refresh();
        }

        private void pictureBox_RImage_DoubleClick(object sender, EventArgs e)
        {
            if (!radioButton1.Checked)
            {
                if (listBox1_SelectedIndex >= 0)
                {
                    listBox1.SelectedIndex = listBox1_SelectedIndex;
                }
                radioButton1.Checked = true;
                radioButton2.Checked = false;
                pictureBox_Image.Refresh();
            }
        }

        public void Change_initial_ROI()
        {
            return;
            //if (listBox1.SelectedIndex > 0)
            //{
            //    return;
            //}
            //for (int j = 1; j < listBox1.Items.Count; j++)
            //{
            //    if ((listBox1.Items[j].ToString().Contains("ROI") &&
            //        listBox1.Items[j].ToString().Contains("측정 영역 설정")) || (listBox1.Items[j].ToString().Contains("ROI") && (
            //        listBox1.Items[j].ToString().Contains("Measurement Area Setting") || listBox1.Items[j].ToString().Contains("测量区域设置"))))
            //    {
            //        if (Cam_Num == 0)
            //        {
            //            string[] str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[j][1].ToString().Split('_');

            //            LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[j][1] =
            //            str[0]
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam0_rect[0].rect.X.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam0_rect[0].rect.Y.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam0_rect[0].rect.Width.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam0_rect[0].rect.Height.ToString()
            //            + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
            //            + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
            //            + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
            //            + "_" + str[23] + "_" + str[24] + "_" + str[25];
            //        }
            //        else if (Cam_Num == 1)
            //        {
            //            string[] str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[j][1].ToString().Split('_');

            //            LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[j][1] =
            //            str[0]
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam1_rect[0].rect.X.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam1_rect[0].rect.Y.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam1_rect[0].rect.Width.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam1_rect[0].rect.Height.ToString()
            //            + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
            //            + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
            //            + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
            //            + "_" + str[23] + "_" + str[24] + "_" + str[25];
            //        }
            //        else if (Cam_Num == 2)
            //        {
            //            string[] str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[j][1].ToString().Split('_');

            //            LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[j][1] =
            //            str[0]
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam2_rect[0].rect.X.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam2_rect[0].rect.Y.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam2_rect[0].rect.Width.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam2_rect[0].rect.Height.ToString()
            //            + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
            //            + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
            //            + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
            //            + "_" + str[23] + "_" + str[24] + "_" + str[25];
            //        }
            //        else if (Cam_Num == 3)
            //        {
            //            string[] str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[j][1].ToString().Split('_');

            //            LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[j][1] =
            //            str[0]
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam3_rect[0].rect.X.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam3_rect[0].rect.Y.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam3_rect[0].rect.Width.ToString()
            //            + "_" +
            //            LVApp.Instance().m_Config.Cam3_rect[0].rect.Height.ToString()
            //            + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
            //            + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
            //            + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
            //            + "_" + str[23] + "_" + str[24] + "_" + str[25];
            //        }
            //    }
            //}
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (t_selected_row_idx == 0 || t_selected_row_idx == 5 || t_selected_row_idx == 8 || t_selected_row_idx == 9)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dataGridView1.EndEdit();
                dataGridView1.BindingContext[dataGridView1.DataSource].EndCurrentEdit();
            }
        }

        private void SettingInterval(object sender, EventArgs e)
        {
            try
            {
                if (m_range_visible_check)
                {
                    m_range_visible_check = false;
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[12].Cells[1].Selected = true;
                    CurrencyManager currencyManager0 = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
                    currencyManager0.SuspendBinding();
                    dataGridView1.Rows[10].Visible = false;
                    dataGridView1.Rows[11].Visible = false;
                    currencyManager0.ResumeBinding();
                }
                else
                {
                    m_range_visible_check = true;
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[10].Cells[1].Selected = true;
                    dataGridView1.Rows[10].Visible = true;
                    dataGridView1.Rows[11].Visible = true;
                }
            }
            catch
            {

            }
        }

        private string str_help = "";
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (t_current_col_idx == 1)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //if (m_range_visible_check)
                        //{
                        //    cm.MenuItems.Add("측정범위 설정 OFF", new EventHandler(SettingInterval));
                        //}
                        //else
                        //{
                        //    cm.MenuItems.Add("측정범위 설정 ON", new EventHandler(SettingInterval));
                        //}
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("모델 불러오기") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("Model Load") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("AI 클래스 번호") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("AI Class Number"))
                        {
                            if (t_current_row_idx == 25)
                            {
                                cm.MenuItems.Add("모델 파일 선택", new EventHandler(Model_PBFile_Select));
                                cm.MenuItems.Add("모델 학습하기", new EventHandler(Model_Train));
                                cm.MenuItems.Add("AI 이미지 저장", new EventHandler(AI_Image_Save));
                            }
                            else
                            {
                                cm.MenuItems.Add("모델 파일 선택", new EventHandler(Model_File_Select));
                            }
                        }

                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("모델 학습") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("Model Train"))
                        {
                            cm.MenuItems.Add("모델 학습하기", new EventHandler(Model_Train));
                        }

                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("ROI 이미지 저장") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("ROI Image Save"))
                        {
                            cm.MenuItems.Add("ROI 이미지 저장", new EventHandler(ROI_Image_Save));
                        }

                        cm.MenuItems.Add("변수값 초기화", new EventHandler(Change_Initial_Parameter));
                        if (m_advenced_param_visible)
                        {
                            cm.MenuItems.Add("고급 관리자 모드 OFF", new EventHandler(Advenced_Parameter));
                        }
                        else
                        {
                            cm.MenuItems.Add("고급 관리자 모드 ON", new EventHandler(Advenced_Parameter));
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //if (m_range_visible_check)
                        //{
                        //    cm.MenuItems.Add("Range setting OFF", new EventHandler(SettingInterval));
                        //}
                        //else
                        //{
                        //    cm.MenuItems.Add("Range setting ON", new EventHandler(SettingInterval));
                        //}
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("모델 불러오기") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("Model Load") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("AI 클래스 번호") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("AI Class Number"))
                        {
                            if (t_current_row_idx == 25)
                            {
                                cm.MenuItems.Add("Select Model File", new EventHandler(Model_File_Select));
                                cm.MenuItems.Add("Model Train", new EventHandler(Model_Train));
                                cm.MenuItems.Add("AI Image Save", new EventHandler(AI_Image_Save));
                            }
                            else
                            {
                                cm.MenuItems.Add("Select Model File", new EventHandler(Model_File_Select));
                            }

                        }
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("모델 학습") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("Model Train"))
                        {
                            cm.MenuItems.Add("Model Train", new EventHandler(Model_Train));
                        }
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("ROI 이미지 저장") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("ROI Image Save"))
                        {
                            cm.MenuItems.Add("ROI Image Save", new EventHandler(ROI_Image_Save));
                        }

                        cm.MenuItems.Add("Initialize parameters", new EventHandler(Change_Initial_Parameter));
                        if (m_advenced_param_visible)
                        {
                            cm.MenuItems.Add("Admin Mode OFF", new EventHandler(Advenced_Parameter));
                        }
                        else
                        {
                            cm.MenuItems.Add("Admin Mode ON", new EventHandler(Advenced_Parameter));
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        //if (m_range_visible_check)
                        //{
                        //    cm.MenuItems.Add("Range setting OFF", new EventHandler(SettingInterval));
                        //}
                        //else
                        //{
                        //    cm.MenuItems.Add("Range setting ON", new EventHandler(SettingInterval));
                        //}
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("모델 불러오기") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("Model Load") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("AI 클래스 번호") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("AI Class Number"))
                        {
                            if (t_current_row_idx == 25)
                            {
                                cm.MenuItems.Add("选择模型文件", new EventHandler(Model_File_Select));
                                cm.MenuItems.Add("模型列车", new EventHandler(Model_Train));
                                cm.MenuItems.Add("AI 图像保存", new EventHandler(AI_Image_Save));
                            }
                            else
                            {
                                cm.MenuItems.Add("选择模型文件", new EventHandler(Model_File_Select));
                            }
                        }
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("모델 학습") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("Model Train"))
                        {
                            cm.MenuItems.Add("模型列车", new EventHandler(Model_Train));
                        }
                        if (dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("ROI 이미지 저장") ||
                            dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString().Contains("ROI Image Save"))
                        {
                            cm.MenuItems.Add("ROI 图像保存", new EventHandler(ROI_Image_Save));
                        }

                        cm.MenuItems.Add("初始化参数", new EventHandler(Change_Initial_Parameter));
                        if (m_advenced_param_visible)
                        {
                            cm.MenuItems.Add("管理模式 OFF", new EventHandler(Advenced_Parameter));
                        }
                        else
                        {
                            cm.MenuItems.Add("管理模式 ON", new EventHandler(Advenced_Parameter));
                        }
                    }

                    dataGridView1.ContextMenu = cm;
                    dataGridView1.ContextMenu.Show(dataGridView1, e.Location);
                    dataGridView1.ContextMenu = null;
                    return;
                }
                else if (t_current_col_idx == 0)
                {
                    if (t_current_row_idx >= 0)
                    {
                        str_help = dataGridView1.Rows[t_current_row_idx].Cells[t_current_col_idx].Value.ToString();
                    }                    
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("도움말", new EventHandler(View_Help));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {
                        cm.MenuItems.Add("HELP", new EventHandler(View_Help));
                    }
                    else if(LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("帮助", new EventHandler(View_Help));
                    }
                    dataGridView1.ContextMenu = cm;
                    dataGridView1.ContextMenu.Show(dataGridView1, e.Location);
                    dataGridView1.ContextMenu = null;
                }

                if (t_current_row_idx < 0)
                {
                    return;
                }
                // 기존 코드
                ToolStripDropDown popup = new ToolStripDropDown();
                popup.Margin = Padding.Empty;
                popup.Padding = Padding.Empty;
                Ctr_Popup content = new Ctr_Popup();
                content.Height = 42;
                content.Width = 200;
                content.pictureBox1.Visible = false;
                //content.t_Item = LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[t_current_row_idx][0].ToString();
                content.t_Item = dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString();
                //content.pictureBox1.Image = global::LV_Inspection_System.Properties.Resources.Display;
                content.display_update();
                ToolStripControlHost host = new ToolStripControlHost(content);
                host.Margin = Padding.Empty;
                host.Padding = Padding.Empty;
                popup.Items.Add(host);

                System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                int t_cur_idx = -1;
                for (int i = 0; i <= t_current_row_idx; i++)
                {
                    if (dataGridView1.Rows[i].Visible == true)
                    {
                        t_cur_idx++;
                    }
                }

                if (t_cur_idx == -1)
                {
                    return;
                }
                location.X = dataGridView1.Rows[t_current_row_idx].Cells[0].Size.Width + 20;
                location.Y = 15 + dataGridView1.Rows[t_cur_idx].Height * t_cur_idx;
                popup.Show(dataGridView1, location);

                //// 기존 코드
                //ToolStripDropDown popup = new ToolStripDropDown();
                //popup.Margin = Padding.Empty;
                //popup.Padding = Padding.Empty;
                //Ctr_Popup content = new Ctr_Popup();
                //content.Height = dataGridView1.Height - 15;
                //content.t_Item = LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[t_current_row_idx][0].ToString();
                //content.display_update();
                //ToolStripControlHost host = new ToolStripControlHost(content);
                //host.Margin = Padding.Empty;
                //host.Padding = Padding.Empty;
                //popup.Items.Add(host);

                //System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                //location.X = dataGridView1.Rows[t_current_row_idx].Cells[0].Size.Width + 20;
                //location.Y = 15;

                //popup.Show(dataGridView1, location);
            }
        }

        public void Enable_control(bool t_b)
        {
            button_OPEN.Enabled = t_b;
            button_ROTATION_CAL.Enabled = t_b;
            button_SNAPSHOT.Enabled = t_b;
            button_INSPECTION.Enabled = t_b;
        }

        private void Model_File_Select(object sender, EventArgs e)
        {
            OpenFileDialog openPanel = new OpenFileDialog();
            openPanel.InitialDirectory = ".\\";
            openPanel.Filter = "All model files|*.pb;";
            int t_ROI_Idx = listBox1_SelectedIndex;
            if (t_ROI_Idx < 0)
            {
                return;
            }
            if (openPanel.ShowDialog() == DialogResult.OK)
            {
                string Model_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + t_ROI_Idx.ToString("00") + "_Model.h5";
                string Model_PB_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + t_ROI_Idx.ToString("00") + "_Model.pb";
                string Label_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + t_ROI_Idx.ToString("00") + "_Label.txt";
                if (System.IO.File.Exists(Model_Path))
                {
                    System.IO.File.Delete(Model_Path);
                }
                if (System.IO.File.Exists(Model_PB_Path))
                {
                    System.IO.File.Delete(Model_PB_Path);
                }
                if (System.IO.File.Exists(Label_Path))
                {
                    System.IO.File.Delete(Label_Path);
                }

                DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }
                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name);
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }
                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                string newModelFileName = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + t_ROI_Idx.ToString("00") + "_Model.pb";
                string newLabelFileName = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + t_ROI_Idx.ToString("00") + "_Label.txt";

                string ModelfileName = openPanel.FileName;  // 파일 경로
                string LabelfileName = Path.GetDirectoryName(openPanel.FileName) + "\\labels.txt";  // 파일 경로

                string message = string.Empty;
                if (File.Exists(newModelFileName))  // 파일의 존재 유무 확인 : 파일이 존재하면
                {
                    if (m_Language == 0)
                    {
                        message = string.Format("{0} 파일이 이미 있습니다. 바꾸시겠습니까?", Path.GetFileName(newModelFileName));
                    }
                    else
                    {
                        message = string.Format("{0} exist, do you want to change?", Path.GetFileName(newModelFileName));
                    }
                    DialogResult dialogResult = MessageBox.Show(message, "알림", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (ModelfileName != newModelFileName)
                        {
                            File.Delete(newModelFileName);  // 존재하는 파일 삭제
                            File.Copy(ModelfileName, newModelFileName);
                        }
                    }
                }
                else   // 파일의 존재하지 않으면    
                {
                    if (File.Exists(ModelfileName))
                    {
                        if (ModelfileName != newModelFileName)
                        {
                            File.Copy(ModelfileName, newModelFileName);
                        }
                    }
                }

                if (File.Exists(newLabelFileName))  // 파일의 존재 유무 확인 : 파일이 존재하면
                {
                    if (LabelfileName != newLabelFileName)
                    {
                        File.Delete(newLabelFileName);  // 존재하는 파일 삭제
                        File.Copy(LabelfileName, newLabelFileName);
                    }
                }
                else
                {
                    if (File.Exists(LabelfileName))
                    {
                        if (LabelfileName != newLabelFileName)
                        {
                            File.Copy(LabelfileName, newLabelFileName);
                        }
                    }
                }

                //if (m_Language == 0)
                //{
                //    message = string.Format("지금 모델을 로딩하시겠습니까?", Path.GetFileName(newModelFileName));
                //}
                //else
                //{
                //    message = string.Format("Do you want to load the model now?", Path.GetFileName(newModelFileName));
                //}
                //DialogResult dialogResult1 = MessageBox.Show(message, "LOAD", MessageBoxButtons.YesNo);
                //if (dialogResult1 == DialogResult.Yes)
                //{
                if (listBox1_SelectedIndex > 0)
                {
                    LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num, listBox1_SelectedIndex - 1] = false;
                    AI_Model_Load(Cam_Num, listBox1_SelectedIndex - 1);
                }
                //}
            }
        }

        private void Model_PBFile_Select(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openPanel = new OpenFileDialog();
                openPanel.InitialDirectory = ".\\";
                openPanel.Filter = "All model files|*.pb;";
                if (openPanel.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    string newModelFileName = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + listBox1_SelectedIndex.ToString("00") + "_Model.pb";
                    string newLabelFileName = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + listBox1_SelectedIndex.ToString("00") + "_Label.txt";


                    string ModelfileName = openPanel.FileName;  // 파일 경로
                    string LabelfileName = Path.GetDirectoryName(openPanel.FileName) + "\\labels.txt";  // 파일 경로

                    string message = string.Empty;
                    if (File.Exists(newModelFileName))  // 파일의 존재 유무 확인 : 파일이 존재하면
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            message = string.Format("{0} 파일이 이미 있습니다. 바꾸시겠습니까?", Path.GetFileName(newModelFileName));
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            message = string.Format("{0} exist, do you want to change?", Path.GetFileName(newModelFileName));
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            message = string.Format("{0} 存在， 你想改变?", Path.GetFileName(newModelFileName));
                        }
                        DialogResult dialogResult = MessageBox.Show(message, "Notice", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (ModelfileName != newModelFileName)
                            {
                                File.Delete(newModelFileName);  // 존재하는 파일 삭제
                                File.Copy(ModelfileName, newModelFileName);
                            }
                        }
                    }
                    else   // 파일의 존재하지 않으면    
                    {
                        if (File.Exists(ModelfileName))
                        {
                            if (ModelfileName != newModelFileName)
                            {
                                File.Copy(ModelfileName, newModelFileName);
                            }
                        }
                    }

                    if (File.Exists(newLabelFileName))  // 파일의 존재 유무 확인 : 파일이 존재하면
                    {
                        if (LabelfileName != newLabelFileName)
                        {
                            File.Delete(newLabelFileName);  // 존재하는 파일 삭제
                            File.Copy(LabelfileName, newLabelFileName);
                        }
                    }
                    else
                    {
                        if (File.Exists(LabelfileName))
                        {
                            if (LabelfileName != newLabelFileName)
                            {
                                File.Copy(LabelfileName, newLabelFileName);
                            }
                        }
                    }

                    string tnewModelFileName = ".\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + Cam_Num.ToString() + "_ROI" + listBox1_SelectedIndex.ToString("00") + "_Model.pb";
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_AI_Model_Loaded(Cam_Num, listBox1_SelectedIndex);
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_AI_Model(Cam_Num, listBox1_SelectedIndex, tnewModelFileName);
                    //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_AI_Model_Loaded(Cam_Num, listBox1_SelectedIndex))
                    //{
                    //    dataGridView1.Rows[25].Cells[1].Value = "1";
                    //}
                    //else
                    //{
                    //    dataGridView1.Rows[25].Cells[1].Value = "0";
                    //}
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{
                    //    message = string.Format("지금 모델을 로딩하시겠습니까?", Path.GetFileName(newModelFileName));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{
                    //    message = string.Format("Do you want to load the model now?", Path.GetFileName(newModelFileName));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    //{//중국어
                    //    message = string.Format("是否要现在加载模型?", Path.GetFileName(newModelFileName));
                    //}
                    //DialogResult dialogResult1 = MessageBox.Show(message, "LOAD", MessageBoxButtons.YesNo);
                    //if (dialogResult1 == DialogResult.Yes)
                    //{
                    //    LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[Cam_Num] = false;
                    //    AI_Model_Load(Cam_Num);
                    //}
                }
            }
            catch
            {

            }
        }

        private void Model_Train(object sender, EventArgs e)
        {
            try
            {
                Process[] arrayProgram = Process.GetProcesses();
                for (int i = 0; i < arrayProgram.Length; i++)
                {
                    if (arrayProgram[i].ProcessName.Equals("AI_Training_Tool"))
                    {
                        arrayProgram[i].Kill();
                    }
                }

                Process.Start(LVApp.Instance().excute_path + "\\AI_Training_Tool.exe");
                //LVApp.Instance().m_AI_Pro.Train_Test(); return;

                //if (File.Exists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"))
                //{
                //    Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "https://teachablemachine.withgoogle.com/train/image");
                //}
                //else if (File.Exists(@"C:\Program Files\Google\Chrome\Application\chrome.exe"))
                //{
                //    Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "https://teachablemachine.withgoogle.com/train/image");
                //}
                //Process.Start("chrome.exe", "https://teachablemachine.withgoogle.com/train/image");
            }
            catch
            {
                AutoClosingMessageBox.Show("Please install Chrome browser!", "ERROR", 3000);
            }
        }

        private void AI_Image_Save(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\AI_Images");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.    
                    dir.Create();
                }
                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\AI_Images\\" + LVApp.Instance().m_Config.m_Model_Name);
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.    
                    dir.Create();
                }
                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\AI_Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString());
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.    
                    dir.Create();
                }


                string tnewModelFileName = ".\\AI_Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString();
                LVApp.Instance().m_mainform.m_ImProClr_Class.Save_SSF_AI_Image(tnewModelFileName, true);
                button_INSPECTION_Click(sender, e);
            }
            catch
            {

            }
        }

        private void ROI_Image_Save(object sender, EventArgs e)
        {
            try
            {
                int t_idx = listBox1.SelectedIndex;
                if (t_idx <= 0)
                {
                    return;
                }
                DataSet DS = null;
                if (Cam_Num == 0)
                {
                    DS = LVApp.Instance().m_Config.ds_DATA_0;
                }
                else if (Cam_Num == 1)
                {
                    DS = LVApp.Instance().m_Config.ds_DATA_1;
                }
                else if (Cam_Num == 2)
                {
                    DS = LVApp.Instance().m_Config.ds_DATA_2;
                }
                else if (Cam_Num == 3)
                {
                    DS = LVApp.Instance().m_Config.ds_DATA_3;
                }

                string[] AI_ROI = DS.Tables[2].Rows[t_idx][1].ToString().Split('_');
                string[] AI_Ratio = DS.Tables[2].Rows[41][1].ToString().Split('_');
                if (AI_ROI[0] == "O")
                {
                    Rectangle t_ROI = new Rectangle(0,0,0,0);
                    t_ROI.X = (int)(Convert.ToDouble(AI_ROI[1]) * Convert.ToDouble(AI_Ratio[0]));
                    t_ROI.Y = (int)(Convert.ToDouble(AI_ROI[2]) * Convert.ToDouble(AI_Ratio[1]));
                    t_ROI.Width = (int)(Convert.ToDouble(AI_ROI[3]) * Convert.ToDouble(AI_Ratio[0]));
                    t_ROI.Height = (int)(Convert.ToDouble(AI_ROI[4]) * Convert.ToDouble(AI_Ratio[1]));
                    Bitmap t_BMP = cropAtRect(pictureBox_Image.Image.Clone() as Bitmap, t_ROI);
                    if (t_BMP == null)
                    {
                        return;
                    }
                    else
                    {
                        Image_SaveFileDialog(t_BMP);
                    }
                }
            }
            catch
            {
                //AutoClosingMessageBox.Show("Please install Chrome browser!", "ERROR", 3000);
            }
        }
        //bool t_ItemCheck_run = false;
        private void listBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (t_button_LOAD_Click_flag)
            {
                return;
            }

            if (e.Index <= 1 || !LVApp.Instance().m_mainform.m_Start_Check)
            {
                e.NewValue = CheckState.Checked;
                return;
            }

            if (listBox1.PointToClient(Cursor.Position).X > 20)
            {
                e.NewValue = e.CurrentValue;
                return;
            }

            if (e.Index <= 1)
            {
                e.NewValue = CheckState.Checked;
                //return;
            }

            //int t_change_status_off_to_on = 0;
            //if (LVApp.Instance().m_mainform.m_Start_Check)
            //{
            //    if (e.CurrentValue == CheckState.Unchecked)
            //    {
            //        t_change_status_off_to_on = 1;
            //    }
            //    else if (e.CurrentValue == CheckState.Checked)
            //    {
            //        t_change_status_off_to_on = 3;
            //    }
            //    //MessageBox.Show(sender.GetType().Name);
            //    //if (listBox1.PointToClient(Cursor.Position).X > 25 && sender.GetType().Name == "CheckedListBox")
            //    //{
            //    //    e.NewValue = e.CurrentValue;
            //    //    return;
            //    //}
            //    if (e.NewValue == CheckState.Checked && t_change_status_off_to_on == 1)
            //    {
            //        t_change_status_off_to_on = 2;
            //    }
            //    else if (e.NewValue == CheckState.Unchecked && t_change_status_off_to_on == 3)
            //    {
            //        t_change_status_off_to_on = 4;
            //    }
            //}

            CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            currencyManager00.SuspendBinding();
            dataGridView1.ClearSelection();
            dataGridView1.Rows[0].Height = 0;
            dataGridView1.Rows[0].Visible = false;
            dataGridView1.Refresh();
            currencyManager00.ResumeBinding();

            if (listBox1.GetItemChecked(e.Index))
            {
                //if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X")
                {
                    dataGridView1.Rows[0].Cells[1].Value = "X";
                    //SubDB_to_MainDB();
                }
            }
            else
            {
                //if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                {
                    dataGridView1.Rows[0].Cells[1].Value = "O";
                    //SubDB_to_MainDB();
                }
            }

            if (e.Index < 1)
            {
                dataGridView1.Rows[0].Cells[1].Value = "O";
                SubDB_to_MainDB();
                return;
            }
            if (e.Index == 1)
            {
                dataGridView1.Rows[0].Cells[1].Value = "O";
            }

            if (Cam_Num == 0)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_0.ClearSelection();
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                //LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[0].Selected = false;
                //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[0].Selected = false;

                if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                {
                    LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[e.Index - 1][0] = true;
                }
                else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && e.Index - 1 >= 0)
                {
                    LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[e.Index - 1][0] = false;
                }
                LVApp.Instance().m_Config.Cam0_rect[e.Index].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                LVApp.Instance().m_Config.Cam0_rect[e.Index].mView = LVApp.Instance().m_Config.Cam0_rect[e.Index].mUse;
                if (e.Index == 0)
                {
                    LVApp.Instance().m_Config.Cam0_rect[e.Index].mView = true;
                }
            }
            else if (Cam_Num == 1)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_1.ClearSelection();
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ClearSelection();
                //LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[0].Selected = true;
                //LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[0].Selected = true;

                if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                {
                    LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[e.Index - 1][0] = true;
                }
                else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && e.Index - 1 >= 0)
                {
                    LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[e.Index - 1][0] = false;
                }
                LVApp.Instance().m_Config.Cam1_rect[e.Index].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                LVApp.Instance().m_Config.Cam1_rect[e.Index].mView = LVApp.Instance().m_Config.Cam1_rect[e.Index].mUse;
                if (e.Index == 0)
                {
                    LVApp.Instance().m_Config.Cam1_rect[e.Index].mView = true;
                }
            }
            else if (Cam_Num == 2)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_2.ClearSelection();
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ClearSelection();
                //LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[0].Selected = true;
                //LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[0].Selected = true;
                if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                {
                    LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[e.Index - 1][0] = true;
                }
                else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && e.Index - 1 >= 0)
                {
                    LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[e.Index - 1][0] = false;
                }
                LVApp.Instance().m_Config.Cam2_rect[e.Index].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                LVApp.Instance().m_Config.Cam2_rect[e.Index].mView = LVApp.Instance().m_Config.Cam2_rect[e.Index].mUse;
                if (e.Index == 0)
                {
                    LVApp.Instance().m_Config.Cam2_rect[e.Index].mView = true;
                }
            }
            else if (Cam_Num == 3)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_3.ClearSelection();
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ClearSelection();
                //LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[0].Selected = true;
                //LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[0].Selected = true;
                if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O")
                {
                    LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[e.Index - 1][0] = true;
                }
                else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && e.Index - 1 >= 0)
                {
                    LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[e.Index - 1][0] = false;
                }
                LVApp.Instance().m_Config.Cam3_rect[e.Index].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                LVApp.Instance().m_Config.Cam3_rect[e.Index].mView = LVApp.Instance().m_Config.Cam3_rect[e.Index].mUse;
                if (e.Index == 0)
                {
                    LVApp.Instance().m_Config.Cam3_rect[e.Index].mView = true;
                }
            }
            SubDB_to_MainDB();
            MainDB_to_SubDB();
            //listBox1_SelectedIndexChanged(sender, e);
            LVApp.Instance().m_Config.Set_Parameters();
            listBox1.Refresh();
            //pictureBox_Image.Refresh();
            //t_ItemCheck_run = true;
            //if (LVApp.Instance().m_mainform.m_Start_Check)
            //{
            //    if ((t_change_status_off_to_on == 2 || t_change_status_off_to_on == 4))
            //    {
            //        button_SAVE_Click(sender, e);
            //        //LVApp.Instance().m_Config.Save_Judge_Data();
            //        //Thread.Sleep(100);
            //        //LVApp.Instance().m_Config.Load_Judge_Data();
            //        //Thread.Sleep(100);
            //    }
            //}
            //t_ItemCheck_run = false;
        }

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                currencyManager00.SuspendBinding();
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Height = 0;
                dataGridView1.Rows[0].Visible = false;
                dataGridView1.Refresh();
                currencyManager00.ResumeBinding();
            }
        }

        public void button_ParamView_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Visible == false)
            {
                dataGridView1.Visible = true;
            }
            else
            {
                dataGridView1.Visible = false;
            }
        }

        private void checkBox_AutoInspection_CheckedChanged(object sender, EventArgs e)
        {
            m_roi_click_auto_mode = checkBox_AutoInspection.Checked;

            if (m_roi_click_auto_mode && !radioButton2.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }
            else if (!m_roi_click_auto_mode && !radioButton1.Checked)
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
        }
    }
}