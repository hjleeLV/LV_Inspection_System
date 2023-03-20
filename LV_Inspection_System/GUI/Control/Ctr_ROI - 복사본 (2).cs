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
using AForge.Imaging.IPPrototyper;
using OfficeOpenXml;
using PopupControl;
using System.Diagnostics;

namespace IPSST_Inspection_System.GUI.Control
{
    public partial class Ctr_ROI : UserControl
    {
        //public int Cam_Num = 0;
        public int Cam_Num = 0;
        public int ROI_Idx = 0;
        private Rectangle rc;
        Popup popup;
        Thread ROI_thread = null;
        public int m_Job_Mode = 0;
        private bool m_range_visible_check = false;

        public Ctr_ROI()
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
                    for (int i=1;i<listBox1.Items.Count;i++)
                    {

                        if (listBox1.Items[i].ToString().Contains("Measuremet Area Setting") ||
                            listBox1.Items[i].ToString().Contains("Measuremet Setting"))
                        {
                            listBox1.Items[i] = "ROI #" + i.ToString("00") + " 측정 영역 설정";
                        }
                    }

                    radioButton1.Text = "ROI 설정 모드";
                    radioButton2.Text = "알고리즘 시험 모드";
                    button_OPEN.Text = "이미지 열기";
                    button_INSPECTION.Text = "알고리즘 시험";
                    button_SNAPSHOT.Text = "카메라 촬영";
                    button_LOAD.Text = "설정 불러오기";
                    button_SAVE.Text = "설정 저장";
                    button_ROTATION_CAL.Text = "보정";
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

                        if (listBox1.Items[i].ToString().Contains("측정 영역 설정") 
                            || listBox1.Items[i].ToString().Contains("측정값"))
                        {
                            listBox1.Items[i] = "ROI #" + i.ToString("00") + " Measuremet Area Setting";
                        }
                    }

                    radioButton1.Text = "ROI Setting Mode";
                    radioButton2.Text = "Alg. Test Mode";
                    button_OPEN.Text = "Image Open";
                    button_INSPECTION.Text = "Alg. Test";
                    button_SNAPSHOT.Text = "Snapshot";
                    button_LOAD.Text = "Load";
                    button_SAVE.Text = "Save";
                    button_ROTATION_CAL.Text = "Cal.";
                }
                //Initialize_ROI();

                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM" + Cam_Num.ToString("0") + " Insp. Area Setting";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM" + Cam_Num.ToString("0") + " Insp. Area Setting";
                }

                dataGridView1.DataSource = null;
                if (IPSSTApp.Instance().m_Config.ds_DATA_0 == null)
                {
                    return;
                }
                if (Cam_Num == 0 && IPSSTApp.Instance().m_Config.ds_DATA_0.Tables.Count > 3)
                {
                    dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3];
                }
                else if (Cam_Num == 1 && IPSSTApp.Instance().m_Config.ds_DATA_1.Tables.Count > 3)
                {
                    dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3];
                }
                else if (Cam_Num == 2 && IPSSTApp.Instance().m_Config.ds_DATA_2.Tables.Count > 3)
                {
                    dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3];
                }
                else if (Cam_Num == 3 && IPSSTApp.Instance().m_Config.ds_DATA_3.Tables.Count > 3)
                {
                    dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3];
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

                dataGridView1.Rows[1].Visible = false;
                dataGridView1.Rows[2].Visible = false;
                dataGridView1.Rows[3].Visible = false;
                dataGridView1.Rows[4].Visible = false;

                for (int i = 12; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
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
            //IPSSTApp.Instance().m_Config.AE_rect[Cam_Num] = new UserRect(new Rectangle(pictureBox_Image.Width - 100, pictureBox_Image.Height - 100, 50, 50));
            //IPSSTApp.Instance().m_Config.AE_rect[Cam_Num].SetPictureBox(pictureBox_Image);
            //IPSSTApp.Instance().m_Config.AE_rect[Cam_Num].m_ROI_Name = "AE";
            //IPSSTApp.Instance().m_Config.AE_rect[Cam_Num].mView = true;
            //IPSSTApp.Instance().m_Config.AE_rect[Cam_Num].mUse = true;
            dataGridView1.DataSource = null;
            if (Cam_Num == 0)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam0_rect.Length; i++)
                {
                    IPSSTApp.Instance().m_Config.Cam0_rect[i] = new UserRect(rc);
                    IPSSTApp.Instance().m_Config.Cam0_rect[i].SetPictureBox(pictureBox_Image);
                    IPSSTApp.Instance().m_Config.Cam0_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    IPSSTApp.Instance().m_Config.Cam0_rect[i].mView = true;
                    IPSSTApp.Instance().m_Config.Cam0_rect[i].mUse = true;
                    IPSSTApp.Instance().m_Config.Cam0_rect[i].t_idx = i;
                    IPSSTApp.Instance().m_Config.Cam0_rect[i].m_Cam_Num = 0;
                }
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM0 검사영역 셋팅";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM0 Insp. Area Setting";
                }

                dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3];
            }
            else if (Cam_Num == 1)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam1_rect.Length; i++)
                {
                    IPSSTApp.Instance().m_Config.Cam1_rect[i] = new UserRect(rc);
                    IPSSTApp.Instance().m_Config.Cam1_rect[i].SetPictureBox(pictureBox_Image);
                    IPSSTApp.Instance().m_Config.Cam1_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    IPSSTApp.Instance().m_Config.Cam1_rect[i].mView = true;
                    IPSSTApp.Instance().m_Config.Cam1_rect[i].mUse = true;
                    IPSSTApp.Instance().m_Config.Cam1_rect[i].t_idx = i;
                    IPSSTApp.Instance().m_Config.Cam1_rect[i].m_Cam_Num = 1;
                }
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM1 검사영역 셋팅";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM1 Insp. Area Setting";
                }
                dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3];
            }
            else if (Cam_Num == 2)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam2_rect.Length; i++)
                {
                    IPSSTApp.Instance().m_Config.Cam2_rect[i] = new UserRect(rc);
                    IPSSTApp.Instance().m_Config.Cam2_rect[i].SetPictureBox(pictureBox_Image);
                    IPSSTApp.Instance().m_Config.Cam2_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    IPSSTApp.Instance().m_Config.Cam2_rect[i].mView = true;
                    IPSSTApp.Instance().m_Config.Cam2_rect[i].mUse = true;
                    IPSSTApp.Instance().m_Config.Cam2_rect[i].t_idx = i;
                    IPSSTApp.Instance().m_Config.Cam2_rect[i].m_Cam_Num = 2;
                }
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM2 검사영역 셋팅";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM2 Insp. Area Setting";
                }
                dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3];
            }
            else if (Cam_Num == 3)
            {
                rc = new Rectangle(0, 0, 0, 0);
                for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam3_rect.Length; i++)
                {
                    IPSSTApp.Instance().m_Config.Cam3_rect[i] = new UserRect(rc);
                    IPSSTApp.Instance().m_Config.Cam3_rect[i].SetPictureBox(pictureBox_Image);
                    IPSSTApp.Instance().m_Config.Cam3_rect[i].m_ROI_Name = "ROI#" + (i).ToString("00");
                    IPSSTApp.Instance().m_Config.Cam3_rect[i].mView = true;
                    IPSSTApp.Instance().m_Config.Cam3_rect[i].mUse = true;
                    IPSSTApp.Instance().m_Config.Cam3_rect[i].t_idx = i;
                    IPSSTApp.Instance().m_Config.Cam3_rect[i].m_Cam_Num = 3;
                }
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    listBox1.Items[0] = "CAM3 검사영역 셋팅";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    listBox1.Items[0] = "CAM3 Insp. Area Setting";
                }
                dataGridView1.DataSource = IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3];
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

            dataGridView1.Rows[1].Visible = false;
            dataGridView1.Rows[2].Visible = false;
            dataGridView1.Rows[3].Visible = false;
            dataGridView1.Rows[4].Visible = false;

            dataGridView1.Rows[10].Visible = false;
            dataGridView1.Rows[11].Visible = false;

            for (int i = 12; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGoldenrodYellow;
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
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
                    if (Cam_Num == 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mView = IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_0.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_0.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_0.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        }
                    }
                    else if (Cam_Num == 1)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mView = IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_1.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_1.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_1.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_1.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        }
                    }
                    else if (Cam_Num == 2)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mView = IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_2.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_2.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_2.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_2.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        }
                    }
                    else if (Cam_Num == 3)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                        IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mView = IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        if (t_idx == 0)
                        {
                            IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mView = true;
                        }

                        if (t_idx - 1 > 0)
                        {
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_3.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_3.ClearSelection();
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_3.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[0].Selected = true;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_3.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                            IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        }
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 1)
                {
                    if (Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 2)
                {
                    if (Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 3)
                {
                    if (Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 4)
                {
                    if (Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                    else if (Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                    else if (Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                    else if (Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                    }
                }
                else if (e.ColumnIndex == 1 && e.RowIndex == 8)
                {
                    if (t_before_alg[Cam_Num] != dataGridView1.Rows[8].Cells[1].Value.ToString())
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
                }

                if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI1.button_INSPECTION_Click(sender, e);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    //if (t_idx >= 1 && t_idx <= 4)
                    //{
                    //    IPSSTApp.Instance().m_mainform.ctr_ROI2.pictureBox_Image.Refresh();
                    //}
                    if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI2.button_INSPECTION_Click(sender, e);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI3.button_INSPECTION_Click(sender, e);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI4.button_INSPECTION_Click(sender, e);
                    }
                }
            }
            catch
            {
            }
            pictureBox_Image.Refresh();
        }


        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //if (load_check)
                //{
                //    load_check = false;
                //    return;
                //}
                int t_idx = listBox1.SelectedIndex;
                if (t_idx < 0)
                {
                    return;
                }
                if (pictureBox_Image.Image == null)
                {
                    listBox1.SelectedIndex = 0;
                    return;
                }

                if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == t_idx)
                {
                    return;
                }

                //radioButton1.Checked = true;
                //radioButton2.Checked = false;
                //if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] != t_idx)
                {
                    //if ((IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0 && t_idx != 0)
                    //    || (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] != 0 && t_idx == 0))
                    {
                        IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_idx;
                        Referesh_Select_Menu(false);
                    }
                    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_idx;
                    MainDB_to_SubDB();
                    MainDB_to_SubDB();
                    Referesh_Select_Menu(true);
                    //button_LOAD_Click(sender, e);
                }
                if (Cam_Num == 0 && t_idx - 1 >= 0)
                {
                    if (IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                else if (Cam_Num == 1 && t_idx - 1 >= 0)
                {
                    if (IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                else if (Cam_Num == 2 && t_idx - 1 >= 0)
                {
                    if (IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "X";
                    }
                }
                else if (Cam_Num == 3 && t_idx - 1 >= 0)
                {
                    if (IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0].ToString() == "True")
                    {
                        dataGridView1.Rows[0].Cells[1].Value = "O";
                    }
                    else if (IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0].ToString() == "False")
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
                            IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][0] = false;
                        }

                        CurrencyManager currencyManager0 = (CurrencyManager)IPSSTApp.Instance().m_mainform.dataGridView_Setting_0.BindingContext[IPSSTApp.Instance().m_mainform.dataGridView_Setting_0.DataSource];
                        currencyManager0.SuspendBinding();
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_0.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse;
                        currencyManager0.ResumeBinding();
                    }
                    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[1] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[2] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[3] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
                    button_INSPECTION_Click(sender, e);
                }
                else if (Cam_Num == 1)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        CurrencyManager currencyManager1 = (CurrencyManager)IPSSTApp.Instance().m_mainform.dataGridView_Setting_1.BindingContext[IPSSTApp.Instance().m_mainform.dataGridView_Setting_1.DataSource];
                        currencyManager1.SuspendBinding();
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_1.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse;
                        currencyManager1.ResumeBinding();
                    }
                    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[0] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[2] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[3] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
                    button_INSPECTION_Click(sender, e);
                }
                else if (Cam_Num == 2)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        CurrencyManager currencyManager2 = (CurrencyManager)IPSSTApp.Instance().m_mainform.dataGridView_Setting_2.BindingContext[IPSSTApp.Instance().m_mainform.dataGridView_Setting_2.DataSource];
                        currencyManager2.SuspendBinding();
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_2.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse;
                        currencyManager2.ResumeBinding();
                    }
                    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[0] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[1] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[3] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
                    button_INSPECTION_Click(sender, e);
                }
                else if (Cam_Num == 3)
                {
                    if (t_idx - 1 > 0)
                    {
                        if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "O" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = true;
                        }
                        else if (dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" && t_idx - 1 >= 0)
                        {
                            IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[t_idx - 1][0] = false;
                        }
                        CurrencyManager currencyManager3 = (CurrencyManager)IPSSTApp.Instance().m_mainform.dataGridView_Setting_3.BindingContext[IPSSTApp.Instance().m_mainform.dataGridView_Setting_3.DataSource];
                        currencyManager3.SuspendBinding();
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_3.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        IPSSTApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[t_idx - 1].Visible = IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse;
                        currencyManager3.ResumeBinding();
                    }
                    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[0] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[1] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[2] = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num];
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(false);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.MainDB_to_SubDB();
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
                    button_INSPECTION_Click(sender, e);
                }

                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(Frm_Trackbar))
                    {
                        form.Close();
                    }
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = (dataGridView1.Height - dataGridView1.ColumnHeadersHeight) / (dataGridView1.Rows.Count - 4);
                    //row.Height = (dataGridView1.Height) / dataGridView1.Rows.Count;
                }
            }
            catch
            {

            }
        }

        private int t_selected_row_idx = 0;
        private string [] t_before_alg = new string[4];
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int t_idx = listBox1.SelectedIndex;
            IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_idx;
            if (t_idx < 0)
            {
                return;
            }
            t_selected_row_idx = e.RowIndex;
            if (e.ColumnIndex == 1 && e.RowIndex == 0)
            {
                if (Cam_Num == 0)
                {
                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam0_rect[t_idx].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 1)
                {
                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam1_rect[t_idx].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 2)
                {
                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam2_rect[t_idx].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 3)
                {
                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam3_rect[t_idx].mUse == false ? "X" : "O";
                }
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

                pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                //propertyGrid1.SelectedObject = imageInfo;
                //propertyGrid1.ExpandAllGridItems();

                //if (imageInfo.BitsPerPixel == 24)
                //{
                //    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                //    byte[] arr = BmpToArray(grayImage);
                //    WSApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                //    grayImage.Dispose();
                //}
                //else
                //{
                //    byte[] arr = BmpToArray(t_Image);
                //    WSApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                //}
                t_Image.Dispose();

                Fit_Size();

                button_INSPECTION_Click(sender, e);
            }

        }

        public void button_SNAPSHOT_Click(object sender, EventArgs e)
        {
            if (Cam_Num == 0)
            {
                if (!IPSSTApp.Instance().m_Config.m_Cam_Continuous_Mode[1])
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                }
                if (IPSSTApp.Instance().m_mainform.ctrCam1.m_bitmap == null)
                {
                    return;
                }
                if (pictureBox_Image.Image != null)
                {
                    pictureBox_Image.Image = null;
                }
                Bitmap t_Image = (Bitmap)IPSSTApp.Instance().m_mainform.ctrCam1.m_bitmap.Clone();
                // 영상처리 파트
                if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                    }
                }
                else
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
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
                if (!IPSSTApp.Instance().m_Config.m_Cam_Continuous_Mode[2])
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                }

                if (IPSSTApp.Instance().m_mainform.ctrCam2.m_bitmap == null)
                {
                    return;
                }
                if (pictureBox_Image.Image != null)
                {
                    pictureBox_Image.Image = null;
                }
                Bitmap t_Image = (Bitmap)IPSSTApp.Instance().m_mainform.ctrCam2.m_bitmap.Clone();
                // 영상처리 파트
                if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                    }
                }
                else
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
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
                if (!IPSSTApp.Instance().m_Config.m_Cam_Continuous_Mode[3])
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                }
                if (IPSSTApp.Instance().m_mainform.ctrCam3.m_bitmap == null)
                {
                    return;
                }
                if (pictureBox_Image.Image != null)
                {
                    pictureBox_Image.Image = null;
                }

                Bitmap t_Image = (Bitmap)IPSSTApp.Instance().m_mainform.ctrCam3.m_bitmap.Clone();
                // 영상처리 파트
                if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                    }
                }
                else
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
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

                if (!IPSSTApp.Instance().m_Config.m_Cam_Continuous_Mode[4])
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                }

                if (IPSSTApp.Instance().m_mainform.ctrCam4.m_bitmap == null)
                {
                    return;
                }
                Bitmap t_Image = (Bitmap)IPSSTApp.Instance().m_mainform.ctrCam4.m_bitmap.Clone();
                // 영상처리 파트
                if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                    }
                }
                else
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                    {
                        //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                    {
                        t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                }

                pictureBox_Image.Image = (System.Drawing.Image)t_Image.Clone();
                pictureBox_RImage.Image = (System.Drawing.Image)t_Image.Clone();
                t_Image.Dispose();
            }
            if (pictureBox_Image.Image != null)
            {
                Fit_Size();
                button_INSPECTION_Click(sender, e);
            }
        }

        public bool load_check = false;
        public void button_LOAD_Click(object sender, EventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Model_Name == "")
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    IPSSTApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    IPSSTApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                return;
            }

            
            try
            {
                //IPSSTApp.Instance().m_Config.Load_Judge_Data();
                if (Cam_Num == 0)
                {
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM0_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Clear();
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM0_image.bmp";
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
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM1_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Clear();
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM1_image.bmp";
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
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM2_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Clear();
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM2_image.bmp";
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
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM3_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Clear();
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].OpenCSVFile(filename);
                    }
                    string filename1 = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM3_image.bmp";
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
            }
            catch
            {

            }
            MainDB_to_SubDB();

            //load_check = true;
            //int t_num = listBox1.SelectedIndex;
            //if (t_num < 0)
            //{
            //    listBox1.SelectedIndex = 0;
            //    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = 0;
            //    Referesh_Select_Menu(false);
            //    MainDB_to_SubDB();

            //}
            //else
            //{
            //    //listBox1.SelectedIndex = -1;
            //    //listBox1.SelectedIndex = 0;
            //    IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = t_num;
            //    Referesh_Select_Menu(false);
            //    MainDB_to_SubDB();
            //    Referesh_Select_Menu(true);
            //    listBox1.SelectedIndex = t_num;
            //}
            //comboBox_TABLETYPE.SelectedIndex = Properties.Settings.Default.PC_No;
            if (IPSSTApp.Instance().m_Config.m_Model_Name == "")
            {
                return;
            }
            FileInfo newFile = new FileInfo(IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + IPSSTApp.Instance().m_Config.m_Model_Name + ".xlsx");
            if (!newFile.Exists)
            {
                return;
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // Add a worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[3];

                for (int i = 0; i < 4; i++)
                {
                    if (worksheet.Cells[11, i+1].Value != null)
                    {
                        int.TryParse(worksheet.Cells[11, i + 1].Value.ToString(), out IPSSTApp.Instance().m_Config.m_Camera_Position[i]);
                    }
                    else
                    {
                        IPSSTApp.Instance().m_Config.m_Camera_Position[i] = 0;
                    }
                    if (worksheet.Cells[12, i+1].Value != null)
                    {
                        int.TryParse(worksheet.Cells[12, i+1].Value.ToString(), out IPSSTApp.Instance().m_Config.nTableType[i]);
                    }
                    else
                    {
                        IPSSTApp.Instance().m_Config.nTableType[i] = 0;
                    }
                    if (i == 0)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI1.comboBox_CAMPOSITION.SelectedIndex = IPSSTApp.Instance().m_Config.m_Camera_Position[i];
                        IPSSTApp.Instance().m_mainform.ctr_ROI1.comboBox_TABLETYPE.SelectedIndex = IPSSTApp.Instance().m_Config.nTableType[i];
                    }
                    else if (i == 1)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI2.comboBox_CAMPOSITION.SelectedIndex = IPSSTApp.Instance().m_Config.m_Camera_Position[i];
                        IPSSTApp.Instance().m_mainform.ctr_ROI2.comboBox_TABLETYPE.SelectedIndex = IPSSTApp.Instance().m_Config.nTableType[i];
                    }
                    else if (i == 2)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI3.comboBox_CAMPOSITION.SelectedIndex = IPSSTApp.Instance().m_Config.m_Camera_Position[i];
                        IPSSTApp.Instance().m_mainform.ctr_ROI3.comboBox_TABLETYPE.SelectedIndex = IPSSTApp.Instance().m_Config.nTableType[i];
                    }
                    else if (i == 3)
                    {
                        IPSSTApp.Instance().m_mainform.ctr_ROI4.comboBox_CAMPOSITION.SelectedIndex = IPSSTApp.Instance().m_Config.m_Camera_Position[i];
                        IPSSTApp.Instance().m_mainform.ctr_ROI4.comboBox_TABLETYPE.SelectedIndex = IPSSTApp.Instance().m_Config.nTableType[i];
                    }

                }
            }


            //IPSSTApp.Instance().m_Config.Load_Judge_Data();

            // Set parameters
            IPSSTApp.Instance().m_Config.Set_Parameters();
            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1.SelectedIndex, IPSSTApp.Instance().m_Config.ROI_Cam_Num);
            button_INSPECTION_Click(sender, e);
            //AutoClosingMessageBox.Show("Loaded.", "Notice", 500);
            //Referesh_Select_Menu(true);
            if (!IPSSTApp.Instance().m_mainform.Force_close)
            {
                ctr_ROI_Guide1.add_Log("Parameter loaded!");
            }
        }

        public void button_SAVE_Click(object sender, EventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Model_Name == "")
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    IPSSTApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    IPSSTApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                return;
            }
            //if (!radioButton1.Checked && radioButton2.Checked)
            //{
            //    radioButton1.Checked = true;
            //    radioButton2.Checked = false;
            //    //button_SAVE_Click(sender, e);
            //    //return;
            //}
            DirectoryInfo dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Models");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name);
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }

            //dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Models_Backup");
            //// 폴더가 존재하지 않으면
            //if (dir.Exists == false)
            //{
            //    // 새로 생성합니다.
            //    dir.Create();
            //}
            //dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Models_Backup\\" + IPSSTApp.Instance().m_Config.m_Model_Name);
            //// 폴더가 존재하지 않으면
            //if (dir.Exists == false)
            //{
            //    // 새로 생성합니다.
            //    dir.Create();
            //}
            //dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Models_Backup\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param");
            //// 폴더가 존재하지 않으면
            //if (dir.Exists == false)
            //{
            //    // 새로 생성합니다.
            //    dir.Create();
            //}

            try
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
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM0_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].WriteToCsvFile(filename);

                    if (pictureBox_Image.Image != null)
                    {
                        filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM0_image.bmp";
                        pictureBox_Image.Image.Save(filename);
                    }
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonOneShot_Click(sender, e);
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM1_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].WriteToCsvFile(filename);

                    if (pictureBox_Image.Image != null)
                    {
                        filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM1_image.bmp";
                        pictureBox_Image.Image.Save(filename);
                    }
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonOneShot_Click(sender, e);
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM2_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].WriteToCsvFile(filename);

                    if (pictureBox_Image.Image != null)
                    {
                        filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM2_image.bmp";
                        pictureBox_Image.Image.Save(filename);
                    }
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonOneShot_Click(sender, e);
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[41][1] = x_ratio.ToString() + "_" + y_ratio.ToString();
                    string filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\" + "CAM3_ROI_data.cvs";
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].WriteToCsvFile(filename);

                    if (pictureBox_Image.Image != null)
                    {
                        filename = IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "CAM3_image.bmp";
                        pictureBox_Image.Image.Save(filename);
                    }
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonOneShot_Click(sender, e);
                }
                //Properties.Settings.Default.PC_No = comboBox_TABLETYPE.SelectedIndex;
                //Properties.Settings.Default.Save();

                //IPSSTApp.Instance().m_Config.Save_Judge_Data();

                // Set Parameters
                IPSSTApp.Instance().m_Config.Set_Parameters();
                IPSSTApp.Instance().m_Config.Save_Judge_Data();
                //if (!IPSSTApp.Instance().m_mainform.Force_close)
                //{
                //    Referesh_Select_Menu(true);
                //    //AutoClosingMessageBox.Show("Saved.", "Notice", 500);
                //}

                try
                {
                    FileInfo newFile = new FileInfo(IPSSTApp.Instance().excute_path + "\\Models\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + IPSSTApp.Instance().m_Config.m_Model_Name + ".xlsx");
                    using (ExcelPackage package = new ExcelPackage(newFile))
                    {
                        // Add a worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[3];

                        worksheet.Cells[11, 1].Value = IPSSTApp.Instance().m_Config.m_Camera_Position[0];
                        worksheet.Cells[11, 2].Value = IPSSTApp.Instance().m_Config.m_Camera_Position[1];
                        worksheet.Cells[11, 3].Value = IPSSTApp.Instance().m_Config.m_Camera_Position[2];
                        worksheet.Cells[11, 4].Value = IPSSTApp.Instance().m_Config.m_Camera_Position[3];
                        worksheet.Cells[12, 1].Value = IPSSTApp.Instance().m_Config.nTableType[0];
                        worksheet.Cells[12, 2].Value = IPSSTApp.Instance().m_Config.nTableType[1];
                        worksheet.Cells[12, 3].Value = IPSSTApp.Instance().m_Config.nTableType[2];
                        worksheet.Cells[12, 4].Value = IPSSTApp.Instance().m_Config.nTableType[3];
                        package.Save();
                    }
                }
                finally
                {
                    if (IPSSTApp.Instance().m_mainform.Force_close)
                    {
                        ctr_ROI_Guide1.add_Log("Parameter saved!");
                        //AutoClosingMessageBox.Show("Saved!", "Notice", 700);
                    }
                }

            }
            catch
            {

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
            pictureBox_Image.Visible = true;
            pictureBox_RImage.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // ALG mode
            pictureBox_Image.Visible = false;
            pictureBox_RImage.Visible = true;
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

                return rgbValues;
            }
            finally
            {
                value.UnlockBits(data);
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
                if (t_run_check)
                {
                    t_run_check = false;
                    ROI_thread.Abort();
                    return;
                }
                t_run_check = true;

                if (IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num] == null)
                {
                    IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num] = new Stopwatch();
                }
                IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].Reset();
                IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].Start();

                if (IPSSTApp.Instance().m_Config.m_Model_Name == "")
                {
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        IPSSTApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                        MessageBox.Show("모델을 등록후 사용하세요.");
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        IPSSTApp.Instance().m_mainform.add_Log("Use after registering a model.");
                        MessageBox.Show("Use after registering a model.");
                    }
                    t_run_check = false;
                    ROI_thread.Abort();
                    return;
                }

                if (listBox1_SelectedIndex < 0)
                {
                    t_run_check = false;
                    ROI_thread.Abort();
                    return;
                }
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1_SelectedIndex;

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
                    t_run_check = false;
                    ROI_thread.Abort();
                    return;
                }

                //Properties.Settings.Default.PC_No = comboBox_TABLETYPE.SelectedIndex;
                //Properties.Settings.Default.Save();

                //SubDB_to_MainDB();



                if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("[검사중...]에는 변수값만 반영됩니다. 오른쪽 영상처리는 검사를 중지후 수행하세요.", "Notice", 2000);
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Parameters downloaded but can't process the image during inspection!", "Notice", 2000);
                    }
                    t_run_check = false;
                    ROI_thread.Abort();
                    return;
                }

                Bitmap t_Image = null;
                if (pictureBox_RImage.InvokeRequired)
                {
                    if (pictureBox_Image.Image == null)
                    {
                        t_run_check = false;
                        ROI_thread.Abort();
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
                            return;
                        }
                        t_Image = (Bitmap)pictureBox_Image.Image.Clone();
                    }
                }
                else
                {
                    if (pictureBox_Image.Image == null)
                    {
                        t_run_check = false;
                        ROI_thread.Abort();
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
                            return;
                        }
                        t_Image = (Bitmap)pictureBox_Image.Image.Clone();
                    }
                }

                if (t_Image.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                    {
                        byte[] arr = BmpToArray(t_Image);
                        if (Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                        else if (Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                        else if (Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                        else if (Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = BmpToArray(grayImage);
                        if (Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        else if (Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        else if (Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        else if (Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                        }
                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = BmpToArray(t_Image);
                    if (Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                    else if (Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                    else if (Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                    else if (Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                    }
                }
                //Thread.Sleep(100);
                t_Image.Dispose();
                byte[] Dst_Img = null;
                int width = 0, height = 0, ch = 0;

                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1_SelectedIndex, Cam_Num);
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.ROI_Object_Find(out Dst_Img, out width, out height, out ch, Cam_Num);
                if (listBox1_SelectedIndex == 0)
                {
                    Thread.Sleep(100);
                    //Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(100);
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(Cam_Num);
                    IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_Inspection(Cam_Num);
                    if (Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                    else if (Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image1(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                    else if (Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image2(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                    else if (Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image3(out Dst_Img, out width, out height, out ch, Cam_Num);
                    }
                }


                if (pictureBox_RImage.InvokeRequired)
                {
                    pictureBox_RImage.Invoke((MethodInvoker)delegate
                    {
                        pictureBox_RImage.Image = ConvertBitmap(Dst_Img, width, height, ch);
                        pictureBox_RImage.Refresh();
                    });
                }
                else
                {
                    pictureBox_RImage.Image = ConvertBitmap(Dst_Img, width, height, ch);
                    pictureBox_RImage.Refresh();
                }
                //radioButton2.Checked = true;
                //}
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(Cam_Num);

                IPSSTApp.Instance().m_Config.Realtime_Running_Check[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = false;

                IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].Stop();

                if (Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log("T/T : " + (IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds-100).ToString() + "ms");
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log("T/T : " + (IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds-100).ToString() + "ms");
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log("T/T : " + (IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds-100).ToString() + "ms");
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log("T/T : " + (IPSSTApp.Instance().m_mainform.ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds - 100).ToString() + "ms");
                }

                t_run_check = false;
                //ROI_thread.Abort();
            }
            catch
            {
                t_run_check = false;
            }
        }

        public void button_INSPECTION_Click(object sender, EventArgs e)
        {
            if (t_run_check)
            {
                return;
            }
            listBox1_SelectedIndex = listBox1.SelectedIndex;
            IPSSTApp.Instance().m_Config.Set_Parameters();
            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1_SelectedIndex, Cam_Num);

            ROI_thread = null;
            ROI_thread = new Thread(Thread_INSPECTION);
            ROI_thread.Start();
            Thread.Sleep(2);
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveMain));
                        cm.MenuItems.Add("Gray 값 출력", new EventHandler(PictureBoxViewGrayR));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveMain));
                        cm.MenuItems.Add("View Gray value", new EventHandler(PictureBoxViewGrayR));
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

        public void Image_SaveFileDialog(System.Drawing.Image bmp)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = IPSSTApp.Instance().excute_path;
            SaveFileDialog1.RestoreDirectory = true;
            if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                SaveFileDialog1.Title = "이미지 저장";
            }
            else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                SaveFileDialog1.Title = "Image save";
            }

            SaveFileDialog1.Filter = "All image files|*.jpg;*.bmp;*.png";
            SaveFileDialog1.FilterIndex = 2;
            SaveFileDialog1.FileName = "Save_" + Cam_Num.ToString() + ".bmp";
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
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
            }
        }

        private void PictureBoxResultviewMain(object sender, EventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.Alg_TextView)
            {
                IPSSTApp.Instance().m_Config.Alg_TextView = false;
                IPSSTApp.Instance().m_mainform.ctr_Log1.checkBox_TextView.Checked = false;
            }
            else
            {
                IPSSTApp.Instance().m_Config.Alg_TextView = true;
                IPSSTApp.Instance().m_mainform.ctr_Log1.checkBox_TextView.Checked = true;
            }
            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(IPSSTApp.Instance().m_Config.Alg_TextView, IPSSTApp.Instance().m_Config.Alg_Debugging);
        }

        private void comboBox_PC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Cam_Num == 0)
            {
                Properties.Settings.Default.CAM0_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
                IPSSTApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM0_Alg_Type;
            }
            else if (Cam_Num == 1)
            {
                Properties.Settings.Default.CAM1_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
                IPSSTApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM1_Alg_Type;
            }
            else if (Cam_Num == 2)
            {
                Properties.Settings.Default.CAM2_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
                IPSSTApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM2_Alg_Type;
            }
            else if (Cam_Num == 3)
            {
                Properties.Settings.Default.CAM3_Alg_Type = comboBox_TABLETYPE.SelectedIndex;
                IPSSTApp.Instance().m_Config.nTableType[Cam_Num] = Properties.Settings.Default.CAM3_Alg_Type;
            }
            Referesh_Select_Menu(true);
        }

        private void comboBox_CAMPOSITION_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_CAMPOSITION.SelectedIndex >= 0)
            {
                IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] = comboBox_CAMPOSITION.SelectedIndex;
                Referesh_Select_Menu(true);
            }
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

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new System.Windows.Forms.Control[]{label, textBox, buttonOk, buttonCancel});
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
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
            //return;
            if (listBox1.SelectedIndex >= 1)
            {
                ContextMenu cm = new ContextMenu();
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("이름 변경", new EventHandler(Name_Change));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Change name", new EventHandler(Name_Change));
                }

                listBox1.ContextMenu = cm;
                listBox1.ContextMenu.Show(listBox1, e.Location);
                listBox1.ContextMenu = null;
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (listBox1.SelectedIndex >= 1)
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("이름 변경", new EventHandler(Name_Change));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Change name", new EventHandler(Name_Change));
                    }
                    listBox1.ContextMenu = cm;
                    listBox1.ContextMenu.Show(listBox1, e.Location);
                    listBox1.ContextMenu = null;
                }
            }
        }

        private void Name_Change(object sender, EventArgs e)
        {
            string value = listBox1.Items[listBox1.SelectedIndex].ToString();
            if (InputBox("New Nmae", "New ROI name:", ref value) == DialogResult.OK)
            {
                listBox1.Items[listBox1.SelectedIndex] = value;
                if (listBox1.SelectedIndex == 0)
                {
                    return;
                }
                if (Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[listBox1.SelectedIndex-1][2] = value;
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[listBox1.SelectedIndex-1][2] = value;
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[listBox1.SelectedIndex - 1][2] = value;
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[listBox1.SelectedIndex - 1][2] = value;
                }
                IPSSTApp.Instance().m_Config.Save_Judge_Data();
            }
        }

        private void Ctr_ROI_SizeChanged(object sender, EventArgs e)
        {
            Fit_Size();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = (dataGridView1.Height - dataGridView1.ColumnHeadersHeight) / (dataGridView1.Rows.Count-4);
                //row.Height = (dataGridView1.Height) / dataGridView1.Rows.Count;
            }
        }

        public void Fit_Size()
        {
            splitContainer_RIGHT2.IsSplitterFixed = false;
            splitContainer_RIGHT.IsSplitterFixed = false;
            if (pictureBox_Image.Image == null)
            {
                splitContainer_RIGHT2.SplitterDistance = splitContainer1.Panel2.Width;
                splitContainer_RIGHT.SplitterDistance = splitContainer1.Panel2.Height;
            }
            else
            {
                splitContainer_RIGHT2.SplitterDistance = splitContainer1.Panel2.Width;
                splitContainer_RIGHT.SplitterDistance = splitContainer1.Panel2.Height;

                double Rw = (double)splitContainer1.Panel2.Width / (double)pictureBox_Image.Image.Width;
                double Rh = (double)splitContainer1.Panel2.Height / (double)pictureBox_Image.Image.Height;
                if (Rw > Rh) // 가로를 줄여야 함.
                {
                    // splitContainer_RIGHT2 : 가로 설정
                    splitContainer_RIGHT2.SplitterDistance = (int)((double)splitContainer1.Panel2.Height * (double)pictureBox_Image.Image.Width / (double)pictureBox_Image.Image.Height);
                }
                else if (Rw < Rh)
                {
                    // splitContainer_RIGHT : 세로 설정
                    splitContainer_RIGHT.SplitterDistance = (int)((double)splitContainer1.Panel2.Width * (double)pictureBox_Image.Image.Height / (double)pictureBox_Image.Image.Width);
                }
            }
            splitContainer_RIGHT2.IsSplitterFixed = true;
            splitContainer_RIGHT.IsSplitterFixed = true;
        }

        private void pictureBox_Image_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Right")
            {
                if (pictureBox_Image.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("ROI 초기화", new EventHandler(PictureBoxROIInitialize));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupViewOri));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveOri));
                        cm.MenuItems.Add("Gray 값 출력", new EventHandler(PictureBoxViewGray));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Initialize ROI", new EventHandler(PictureBoxROIInitialize));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupViewOri));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveOri));
                        cm.MenuItems.Add("View Gray value", new EventHandler(PictureBoxViewGray));
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
            if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                str = "ROI를 초기화 하시겠습니까?";
            }
            else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                str = "Do you want to initialize the ROI?";
            }
            if (MessageBox.Show(str, " Initialize ROI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dataGridView1.Rows[1].Cells[1].Value = pictureBox_Image.Width / 2 - 200;
                dataGridView1.Rows[2].Cells[1].Value = pictureBox_Image.Height / 2 - 200;
                dataGridView1.Rows[3].Cells[1].Value = 400;
                dataGridView1.Rows[4].Cells[1].Value = 400;
                if (Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam0_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam1_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam2_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.X = Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.Y = Convert.ToInt32(dataGridView1.Rows[2].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.Width = Convert.ToInt32(dataGridView1.Rows[3].Cells[1].Value);
                    IPSSTApp.Instance().m_Config.Cam3_rect[listBox1.SelectedIndex].rect.Height = Convert.ToInt32(dataGridView1.Rows[4].Cells[1].Value);
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
        }


        void MainDB_to_SubDB()
        {
            try
            {
                if (listBox1.SelectedIndex < 0)
                {
                    return;
                }
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1.SelectedIndex;
                if (Cam_Num == 0)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam0_rect.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[i].mView = false;
                    }

                    string[] str = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[4][1].ToString())
                        );
                    IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 1)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam1_rect.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[i].mView = false;
                    }
                    string[] str = IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[4][1].ToString())
                        );
                    IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 2)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam2_rect.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[i].mView = false;
                    }
                    string[] str = IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[4][1].ToString())
                        );
                    IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }
                else if (Cam_Num == 3)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.Cam3_rect.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[i].mView = false;
                    }
                    string[] str = IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1].ToString().Split('_');

                    for (int i = 1; i < str.Length; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[i][1] = str[i];
                    }
                    rc = new Rectangle(Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[1][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[2][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[3][1].ToString())
                        , Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[4][1].ToString())
                        );
                    IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].rect = rc;
                    if (str[0] == "X")
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = false;
                        IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    else
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = true;
                        IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    }
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    dataGridView1.Rows[0].Cells[1].Value = IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                }

                int t_Vx = Convert.ToInt32(dataGridView1.Rows[6].Cells[1].Value);
                if (t_Vx != ctr_ROI_Guide1.trackBar_V1.Value)
                {
                    ctr_ROI_Guide1.trackBar_V1.Value = t_Vx;
                    ctr_ROI_Guide1.textBox_V1.Text = t_Vx.ToString();
                }
                t_Vx = Convert.ToInt32(dataGridView1.Rows[7].Cells[1].Value);
                if (t_Vx != ctr_ROI_Guide1.trackBar_V2.Value)
                {
                    ctr_ROI_Guide1.trackBar_V2.Value = t_Vx;
                    ctr_ROI_Guide1.textBox_V2.Text = t_Vx.ToString();
                }

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
                if (listBox1.SelectedIndex < 0)
                {
                    return;
                }
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1.SelectedIndex;

                if (Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = IPSSTApp.Instance().m_Config.Cam0_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[14][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[25][1].ToString();
                    //MessageBox.Show(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1].ToString());
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = IPSSTApp.Instance().m_Config.Cam1_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[14][1].ToString()
                                            + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[25][1].ToString();
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = IPSSTApp.Instance().m_Config.Cam2_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[14][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[25][1].ToString();
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse = dataGridView1.Rows[0].Cells[1].Value.ToString() == "X" ? false : true;
                    IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse;
                    if (IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] == 0)
                    {
                        IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mView = true;
                    }

                    string str = IPSSTApp.Instance().m_Config.Cam3_rect[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]].mUse == false ? "X" : "O";
                    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num]][1] =
                        str
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[1][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[2][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[3][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[4][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[5][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[6][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[7][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[8][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[9][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[10][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[11][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[12][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[13][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[14][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[15][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[16][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[17][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[18][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[19][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[20][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[21][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[22][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[23][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[24][1].ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[25][1].ToString();
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
            IPSSTApp.Instance().m_Config.ROI_Selected_IDX[Cam_Num] = listBox1.SelectedIndex;
            SubDB_to_MainDB();

            IPSSTApp.Instance().m_Config.Set_Parameters();

            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, listBox1.SelectedIndex, Cam_Num);

            Bitmap t_Image = (Bitmap)pictureBox_Image.Image.Clone();

            if (t_Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                byte[] arr = BmpToArray(grayImage);
                if (Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                grayImage.Dispose();
            }
            else
            {
                byte[] arr = BmpToArray(t_Image);
                if (Cam_Num == 0)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 1)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 2)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
                else if (Cam_Num == 3)
                {
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);
                }
            }
            t_Image.Dispose();
            byte[] Dst_Img = null;
            int width = 0, height = 0, ch = 0;

            if (Cam_Num == 0)
            {
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num);
            }
            else if (Cam_Num == 1)
            {
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image1(out Dst_Img, out width, out height, out ch, Cam_Num);
            }
            else if (Cam_Num == 2)
            {
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image2(out Dst_Img, out width, out height, out ch, Cam_Num);
            }
            else if (Cam_Num == 3)
            {
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image3(out Dst_Img, out width, out height, out ch, Cam_Num);
            }

            //if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class0.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
            {
                pictureBox_Image.Image = ConvertBitmap(Dst_Img, width, height, ch);
                pictureBox_Image.Refresh();
                //radioButton2.Checked = true;
            }
        }

        public void Change_Initial_Parameter(object sender, EventArgs e)
        {
            // 8번 알고리즘 선택
            string tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
            for (int i = 12; i <= 25; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
                {
                    dataGridView1.Rows[i].Cells[1].Value = "0";
                }
            }
            if (   tstr == "좌측 끝 기준" || tstr == "우측 끝 기준" || tstr == "좌상 기준" || tstr == "좌하 기준" || tstr == "우상 기준" || tstr == "우하 기준" || tstr == "중심 기준"
                || tstr == "Left end" || tstr == "Right end" || tstr == "Left top" || tstr == "Left bottom" || tstr == "Right top" || tstr == "Right bottom" || tstr == "Center"
                )
            {
                if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0) // 상부, 하부일때
                {
                    dataGridView1.Rows[12].Cells[1].Value = "1";
                    dataGridView1.Rows[13].Cells[1].Value = "0";
                    dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                    dataGridView1.Rows[15].Cells[1].Value = "0";
                    dataGridView1.Rows[16].Cells[1].Value = "0";
                    dataGridView1.Rows[17].Cells[1].Value = "1";
                }
                else
                {
                    if (comboBox_TABLETYPE.SelectedIndex == 0)
                    {// 인덱스 타입 일때
                        dataGridView1.Rows[15].Cells[1].Value = "1"; // 이름
                    }
                    else if (comboBox_TABLETYPE.SelectedIndex == 1)
                    {// 글라스 타입 일때
                        dataGridView1.Rows[12].Cells[1].Value = "1"; // 이름
                        dataGridView1.Rows[13].Cells[1].Value = "10"; // 이름
                        dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                    }
                    else if (comboBox_TABLETYPE.SelectedIndex == 2)
                    {// 벨트 타입 일때
                        dataGridView1.Rows[12].Cells[1].Value = "10"; // 이름
                        dataGridView1.Rows[13].Cells[1].Value = "10"; // 이름
                        dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                    }
                    else if (comboBox_TABLETYPE.SelectedIndex == 3)
                    {// 가이드 없을때
                        dataGridView1.Rows[13].Cells[1].Value = "1"; // 이름
                        dataGridView1.Rows[14].Cells[1].Value = "1"; // 이름
                        dataGridView1.Rows[15].Cells[1].Value = "1"; // 이름
                    }
                }
            }
            else if (tstr == "가로 길이" || tstr == "Hor. length" || tstr == " 길이" || tstr == "Ver. length")
            {
                dataGridView1.Rows[12].Cells[1].Value = "0";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "100";
            }
            else if (tstr == "십자 치수" || tstr == "Dim. of cross")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "4";
                dataGridView1.Rows[14].Cells[1].Value = "0";
            }
            else if (tstr == "직경" || tstr == "Diameter")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "100";
            }
            else if (tstr == "사각 영역의 밝기" || tstr == "Brightness of rectangle ROI")
            {
                dataGridView1.Rows[12].Cells[1].Value = "0";
            }
            else if (tstr == "원형 영역의 밝기" || tstr == "Brightness of circle ROI")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
            }
            else if (tstr == "사각 영역의 BLOB" || tstr == "BLOB in rectangle ROI")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "1";
                dataGridView1.Rows[18].Cells[1].Value = "50";
                dataGridView1.Rows[19].Cells[1].Value = "50";
                dataGridView1.Rows[20].Cells[1].Value = "0";
                dataGridView1.Rows[21].Cells[1].Value = "0";
                dataGridView1.Rows[22].Cells[1].Value = "0";
            }
            else if (tstr == "원형 영역의 BLOB" || tstr == "BLOB in circle ROI")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
                dataGridView1.Rows[14].Cells[1].Value = "0";
                dataGridView1.Rows[15].Cells[1].Value = "0";
                dataGridView1.Rows[16].Cells[1].Value = "0";
                dataGridView1.Rows[17].Cells[1].Value = "360";
                dataGridView1.Rows[18].Cells[1].Value = "1";
                dataGridView1.Rows[19].Cells[1].Value = "0";
                dataGridView1.Rows[20].Cells[1].Value = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                dataGridView1.Rows[21].Cells[1].Value = "0";
                dataGridView1.Rows[22].Cells[1].Value = "0";
                dataGridView1.Rows[23].Cells[1].Value = "100";
                dataGridView1.Rows[24].Cells[1].Value = "0";
                dataGridView1.Rows[25].Cells[1].Value = "0";
            }
            else if (tstr == "진원도(%)" || tstr == "Circularity(%)")
            {

            }
            else if (tstr == "나사산 피치" || tstr == "Pitch of thread")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
            }
            else if (tstr == "두 영역 중심간 거리" || tstr == "Distance between two area")
            {
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
                dataGridView1.Rows[12].Cells[1].Value = "1";
                dataGridView1.Rows[13].Cells[1].Value = "0";
            }
            else if (tstr == "원형 영역의 색상 BLOB" || tstr == "Color BLOB in circle ROI")
            {
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
                dataGridView1.Rows[12].Cells[1].Value = "200";
                dataGridView1.Rows[13].Cells[1].Value = "1";
                dataGridView1.Rows[14].Cells[1].Value = "1";
                dataGridView1.Rows[15].Cells[1].Value = "1";
                dataGridView1.Rows[16].Cells[1].Value = "0";
            }
            else if (tstr == "머리 나사부 동심도" || tstr == "Concentricity")
            {

            }
            else if (tstr == "하부 V 각도" || tstr == "V Angle of bottom")
            {
                dataGridView1.Rows[12].Cells[1].Value = "5";
                dataGridView1.Rows[13].Cells[1].Value = "10";
            }
            else if (tstr == "리드각(1)" || tstr == "Lead angle of thread(1)" || tstr == "리드각(0.5)" || tstr == "Lead angle of thread(0.5)")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
            }
            else if (tstr == "몸통 두께" || tstr == "Thickness of body(mm)" || tstr == "몸통 휨" || tstr == "Bending of body(mm)")
            {
                dataGridView1.Rows[12].Cells[1].Value = "1";
            }
            else if (tstr == "하부 형상" || tstr == "Shape of bottom")
            {
                dataGridView1.Rows[12].Cells[1].Value = "5";
                dataGridView1.Rows[13].Cells[1].Value = "5";
            }
        }

        public void Referesh_Select_Menu(bool text_maintain)
        {
            try
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
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
                    if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();
                    }
                    if (main_str.Contains("모델 사용"))
                    {
                        dgvCmbCell5.Items.Add("비교v1이하v2이상");
                    }

                    if (listBox1.SelectedIndex == 0)
                    {
                        //if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] != 1)
                        {
                            dgvCmbCell5.Items.Add("모델 사용");
                        }
                    }
                    else
                    {
                        if (!main_str.Contains("모델 사용"))
                        {
                            dgvCmbCell5.Items.Add("검사 영역 결과 사용");
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

                    if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0)
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
                            if (!tstr.Contains("기준"))
                            {
                                tstr = "중심 기준";
                            }
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
                            dataGridView1.Rows[16].Cells[0].Value = "각도 보정(0:안함,1:좌측,2:우측,3:상부,4:하부,5:좌우중심,6:상하중심)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 6 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                            {
                                if (comboBox_TABLETYPE.SelectedIndex != 5)
                                {
                                    dataGridView1.Rows[16].Cells[1].Value = 0;
                                }
                            }
                            // 변수 추가
                            dataGridView1.Rows[17].Cells[0].Value = "BLOB 합침 필터 크기"; // 이름

                        }
                        else
                        {
                            dgvCmbCell8.Items.Add("가로 길이");
                            dgvCmbCell8.Items.Add("세로 길이");
                            dgvCmbCell8.Items.Add("원형 영역의 밝기");//5
                            dgvCmbCell8.Items.Add("원형 영역의 BLOB");
                            //dgvCmbCell8.Items.Add("원형 영역의 BLOB 크기");
                            //dgvCmbCell8.Items.Add("원형 영역의 BLOB 갯수");//9
                            dgvCmbCell8.Items.Add("사각 영역의 밝기");
                            dgvCmbCell8.Items.Add("사각 영역의 BLOB");//6
                           // dgvCmbCell8.Items.Add("사각 ROI의 BLOB 갯수(Count)");
                            dgvCmbCell8.Items.Add("직경");//3
                            dgvCmbCell8.Items.Add("진원도(%)");
                            dgvCmbCell8.Items.Add("십자 치수");
                            dgvCmbCell8.Items.Add("두 영역 중심간 거리");
                            dgvCmbCell8.Items.Add("나사산 피치");
                            dgvCmbCell8.Items.Add("나사산 크기");
                            dgvCmbCell8.Items.Add("원형 영역의 색상 BLOB");//14
                            //dgvCmbCell8.Items.Add("볼록 BLOB 차이");//15
                            dgvCmbCell8.Items.Add("내외경 중심 차이");//16
                            if (main_str.Contains("모델 사용"))
                            {
                                dgvCmbCell8.Items.Add("일치율(%)");
                            }
                            if (tstr.Contains("원형 영역의 BLOB 크기") || tstr.Contains("원형 영역의 BLOB 갯수"))
                            {
                                tstr = "원형 영역의 BLOB";
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
                                //&& !tstr.Contains("볼록 BLOB 차이") 
                                && !tstr.Contains("내외경 중심 차이") && !tstr.Contains("일치율(%)") && tstr.Length != 0)
                            {
                                tstr = "가로 길이";
                            }
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 1)
                    { // 사이드이면 
                        if (listBox1.SelectedIndex == 0)
                        {
                            dgvCmbCell8.Items.Add("좌상 기준");
                            //if (!tstr.Contains("기준"))
                            {
                                tstr = "좌상 기준";
                            }

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
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 1)
                            {// 글라스 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                }
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 2)
                            {// 벨트 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "가이드 선 두께"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                }
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
                            dgvCmbCell8.Items.Add("가로 길이");
                            dgvCmbCell8.Items.Add("세로 길이");
                            dgvCmbCell8.Items.Add("몸통 두께");
                            dgvCmbCell8.Items.Add("몸통 휨");
                            dgvCmbCell8.Items.Add("나사산 피치");
                            dgvCmbCell8.Items.Add("나사산 크기");
                            dgvCmbCell8.Items.Add("리드각(0.5)");
                            dgvCmbCell8.Items.Add("리드각(1)");
                            dgvCmbCell8.Items.Add("사각 영역의 밝기");
                            dgvCmbCell8.Items.Add("사각 영역의 BLOB");
                            dgvCmbCell8.Items.Add("하부 V 각도");
                            dgvCmbCell8.Items.Add("머리 나사부 동심도");
                            dgvCmbCell8.Items.Add("하부 형상");
                            dgvCmbCell8.Items.Add("두 영역 중심간 거리");
                            //dgvCmbCell8.Items.Add("볼록 BLOB 차이");//14
                            if (main_str.Contains("모델 사용"))
                            {
                                dgvCmbCell8.Items.Add("일치율(%)");
                            } 
                            if (!tstr.Contains("가로 길이") && !tstr.Contains("세로 길이")
                                && !tstr.Contains("머리 나사부 동심도") && !tstr.Contains("하부 V 각도")
                                && !tstr.Contains("나사산 피치") && !tstr.Contains("나사산 크기")
                                && !tstr.Contains("리드각(1)") && !tstr.Contains("리드각(0.5)")
                                && !tstr.Contains("몸통 두께") && !tstr.Contains("몸통 휨")
                                && !tstr.Contains("사각 영역의 밝기") && !tstr.Contains("사각 영역의 BLOB")
                                && tstr.Length != 0 && !tstr.Contains("하부 형상") && !tstr.Contains("두 영역 중심간 거리")
                                //&& !tstr.Contains("볼록 BLOB 차이") 
                                && !tstr.Contains("일치율(%)") && tstr.Length != 0)
                            {
                                tstr = "가로 길이";
                            }
                        }
                    }

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
                    tstr = dataGridView1.Rows[8].Cells[1].Value.ToString();
                    if (tstr == "가로 길이" || tstr == "세로 길이")
                    {// 0번, 1번
                        dataGridView1.Rows[12].Cells[0].Value = "계산 각도(Degree)";
                        dataGridView1.Rows[13].Cells[0].Value = "측정 최소 거리(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "측정 최대 거리(mm)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100;
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
                        }
                        else
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "예비변수";
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
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "영역1 지름(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "영역1 두께(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "영역2 지름(mm)";
                        dataGridView1.Rows[15].Cells[0].Value = "영역2 두께(mm)";
                        dataGridView1.Rows[16].Cells[0].Value = "시작각(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "종료각(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 360;
                        }

                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 0 && Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                            dataGridView1.Rows[17].Cells[1].Value = 360;
                        }
                        dataGridView1.Rows[18].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[19].Cells[0].Value = "검사대상크기(하한값)";
                        dataGridView1.Rows[20].Cells[0].Value = "검사대상크기(상한값)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[21].Cells[0].Value = "경계 연결 수";
                        if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 2)
                        {
                            dataGridView1.Rows[21].Cells[1].Value = 2;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[21].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[22].Cells[0].Value = "기준옵션(0:ROI#0,1:P이하,2:P이상)";
                        if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) > 2)
                        {
                            dataGridView1.Rows[22].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) < 3)
                        {
                            dataGridView1.Rows[23].Cells[0].Value = "P임계값";
                            if (Convert.ToInt32(dataGridView1.Rows[23].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[23].Cells[1].Value.ToString()) > 255)
                            {
                                dataGridView1.Rows[23].Cells[1].Value = 0;
                            }
                        }
                        dataGridView1.Rows[24].Cells[0].Value = "계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
                        if (Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[24].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) > 8)
                        {
                            dataGridView1.Rows[24].Cells[1].Value = 8;
                        }
                        dataGridView1.Rows[25].Cells[0].Value = "옵션(0:미사용,1:볼록 BLOB 차이,2:영역1(v1이하) 영역2(v2이상) 임계화)";
                        if (Convert.ToInt32(dataGridView1.Rows[25].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[25].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[25].Cells[1].Value = 0;
                        }
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
                        dataGridView1.Rows[12].Cells[0].Value = "노이즈 제거 필터 크기";
                        dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)";
                        dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)";
                        if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[14].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[15].Cells[0].Value = "필터 사용(0:미사용,1:사용,2:컬러+미사용,4:컬러+사용)";
                        if (Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[15].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[15].Cells[1].Value = 0;
                        }

                        dataGridView1.Rows[16].Cells[0].Value = "필터 방향(0:전방향,1:가로,2:세로)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "필터 처리수(CNT)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 1;
                        }
                        dataGridView1.Rows[18].Cells[0].Value = "어두운 임계값(Gray)";
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) > 255)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 255;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[19].Cells[0].Value = "밝은 임계값(Gray)";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 255)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 255;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[20].Cells[0].Value = "전처리(Blur)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 0;
                        }
                        if (tstr == "사각 영역의 BLOB")
                        {
                            dataGridView1.Rows[21].Cells[0].Value = "계산방법(0:픽셀수,1:가로길이,2:세로길이,3:갯수,4:기준점부터 거리,5:픽셀수(가장자리 제외),6:픽셀수(가장자리))";
                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 6 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[21].Cells[1].Value = 0;
                            }

                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 1)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "가로 최소 길이";
                                dataGridView1.Rows[14].Cells[0].Value = "가로 최대 길이";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 2)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "세로 최소 길이";
                                dataGridView1.Rows[14].Cells[0].Value = "세로 최대 길이";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 3)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "검사대상크기(하한값)";
                                dataGridView1.Rows[14].Cells[0].Value = "검사대상크기(상한값)";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 4)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "BLOB 최소 거리";
                                dataGridView1.Rows[14].Cells[0].Value = "BLOB 최대 거리";
                            }
                            dataGridView1.Rows[22].Cells[0].Value = "볼록 BLOB(0:미사용,1:사용)";
                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[22].Cells[1].Value = 0;
                            }
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
                    }
                    else if (tstr == "머리 나사부 동심도") // 번
                    {
                        dataGridView1.Rows[5].Cells[1].Value = "검사 영역 결과 사용";
                    }
                    else if (tstr == "하부 V 각도") // 3번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "각도 계산 높이(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "회전 나사선 제거 필터 크기";
                    }
                    else if (tstr == "몸통 휨" || tstr == "몸통 두께") // 7, 8번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "회전 나사선 제거 필터 크기";
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
                        if (tstr.Contains("나사산 크기"))
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "계산방법(0:픽셀수,1:가로길이,2:세로길이,3:볼록BLOB)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 0;
                            }
                            //if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0)
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
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "내경(지름 mm)"; t_idx++;
                                listBox1.Items[t_idx] = "단자경(지름 mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자경(지름 mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 폭 Min(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 폭 Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 폭 Max(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 폭 Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 깊이 Min(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 깊이 Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 깊이 Max(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 깊이 Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "단자간 각도(angle)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자간 각도(angle)"; t_idx++;
                                listBox1.Items[t_idx] = "단자 슬리트간 각도(angle)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자 슬리트간 각도(angle)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트간 각도(angle)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트간 각도(angle)"; t_idx++;
                                listBox1.Items[t_idx] = "단자수(count)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자수(count)"; t_idx++;
                                listBox1.Items[t_idx] = "단자휨 갯수(count)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자휨 갯수(count)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 수(count)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 수(count)"; t_idx++;
                                listBox1.Items[t_idx] = "슬리트 불량(pixel)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "슬리트 불량(pixel)"; t_idx++;
                                listBox1.Items[t_idx] = "단자 들뜸(count)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "단자 들뜸(count)"; t_idx++;

                                //IPSSTApp.Instance().m_Config.Save_Judge_Data();
                            }
                            else if (dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && Cam_Num != 0)
                            {
                                if (IPSSTApp.Instance().m_mainform.m_Start_Check && text_maintain)
                                {
                                    AutoClosingMessageBox.Show("스기야마는 CAM0만 지원합니다. ", "WARNING", 3000);
                                }
                            }
                            else
                            {
                                if (IPSSTApp.Instance().m_mainform.m_Start_Check && text_maintain && Cam_Num == 0)
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

                    for (int i = 12; i < dataGridView1.RowCount; i++)
                    {
                        dataGridView1.Rows[i].Visible = true;
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "예비변수" || !m_range_visible_check && (
                            dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("하위 측정범위")
                            || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("상위 측정범위")
                            || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. Low bound")
                            || dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("M. High bound"))
                            )
                        {
                            dataGridView1.Rows[i].Cells[1].Value = 0;
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                            dataGridView1.Rows[i].Visible = false;
                        }
                        else if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "고객사 번호")
                        {
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.SkyBlue;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightPink;
                        }
                    }
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
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
                    if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                    {
                        main_str = IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();
                    }
                    if (main_str.Contains("Model find"))
                    {
                        dgvCmbCell5.Items.Add("Compare less v1 more v2");
                    }

                    if (listBox1.SelectedIndex == 0)
                    {
                        if (Cam_Num != 1)
                        {
                            dgvCmbCell5.Items.Add("Model find");
                        }
                    }
                    else
                    {
                        if (!main_str.Contains("Model find"))
                        {
                            dgvCmbCell5.Items.Add("Insp. area result use");
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

                    if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 0)
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
                            if (!tstr.Contains("Left") && !tstr.Contains("Right") && !tstr.Contains("Center") && tstr.Length != 0)
                            {
                                tstr = "Center";
                            }
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
                            dataGridView1.Rows[16].Cells[0].Value = "Angle cal.(0:None,1:Left,2:Right,3:Top,4:Bottom,5:LR_Center,6:TB_Center)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 6 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) <= -1)
                            {
                                if (comboBox_TABLETYPE.SelectedIndex != 5)
                                {
                                    dataGridView1.Rows[16].Cells[1].Value = 0;
                                }
                            }
                            // 변수 추가
                            dataGridView1.Rows[17].Cells[0].Value = "Filter size for BLOB merge"; // 이름
                        }
                        else
                        {
                            dgvCmbCell8.Items.Add("Hor. length");
                            dgvCmbCell8.Items.Add("Ver. length");
                            dgvCmbCell8.Items.Add("Brightness of circle ROI");//5
                            dgvCmbCell8.Items.Add("BLOB in circle ROI");
                            //dgvCmbCell8.Items.Add("BLOB size in circle ROI");
                            //dgvCmbCell8.Items.Add("BLOB count in circle ROI");//9

                            dgvCmbCell8.Items.Add("Brightness of rectangle ROI");
                            dgvCmbCell8.Items.Add("BLOB in rectangle ROI");//6
                            //dgvCmbCell8.Items.Add("BLOB count in rect ROI(Count)");
                            dgvCmbCell8.Items.Add("Diameter");//3
                            dgvCmbCell8.Items.Add("Circularity(%)");
                            dgvCmbCell8.Items.Add("Dim. of cross");
                            dgvCmbCell8.Items.Add("Distance between two area");
                            dgvCmbCell8.Items.Add("Pitch of thread");
                            dgvCmbCell8.Items.Add("Size of thread");
                            dgvCmbCell8.Items.Add("Color BLOB in circle ROI");
                            //dgvCmbCell8.Items.Add("Convex BLOB Difference");//15
                            dgvCmbCell8.Items.Add("Center difference between Inner and outter circle");//16
                            if (main_str.Contains("Model find"))
                            {
                                dgvCmbCell8.Items.Add("Match rate(%)");
                            }
                            if (tstr.Contains("BLOB size in circle ROI") || tstr.Contains("BLOB count in circle ROI"))
                            {
                                tstr = "BLOB in circle ROI";
                            }
                            if (!tstr.Contains("Hor. length") && !tstr.Contains("Ver. length")
                                && !tstr.Contains("Dim. of cross") && !tstr.Contains("Diameter")
                                && !tstr.Contains("Brightness of rectangle ROI") && !tstr.Contains("Brightness of circle ROI")
                                && !tstr.Contains("BLOB in rectangle ROI") && !tstr.Contains("BLOB count in rect ROI(Count)")
                                //&& !tstr.Contains("BLOB size in circle ROI") && !tstr.Contains("BLOB count in circle ROI")
                                && !tstr.Contains("BLOB in circle ROI")
                                && !tstr.Contains("Circularity(%)") && !tstr.Contains("Pitch of thread") && !tstr.Contains("Distance between two area")
                                && !tstr.Contains("Size of thread") && !tstr.Contains("Color BLOB in circle ROI") && !tstr.Contains("Match rate(%)")
                                //&& !tstr.Contains("Convex BLOB Difference")
                                && !tstr.Contains("Center difference between Inner and outter circle") && tstr.Length != 0)
                            {
                                tstr = "Hor. length";
                            }
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Camera_Position[Cam_Num] == 1)
                    { // 사이드이면 
                        if (listBox1.SelectedIndex == 0)
                        {
                            dgvCmbCell8.Items.Add("Left top");
                            //if (!tstr.Contains("기준"))
                            {
                                tstr = "Left top";
                            }

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
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 1)
                            {// 글라스 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                }
                            }
                            else if (comboBox_TABLETYPE.SelectedIndex == 2)
                            {// 벨트 타입 일때
                                // 변수 추가
                                dataGridView1.Rows[12].Cells[0].Value = "Thickness of guide"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB"; // 이름
                                // 변수 추가
                                dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB"; // 이름
                                if (Convert.ToInt32(dataGridView1.Rows[14].Cells[1].Value.ToString()) == 0)
                                {
                                    dataGridView1.Rows[14].Cells[1].Value = 100000;
                                }
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
                            dgvCmbCell8.Items.Add("Hor. length");
                            dgvCmbCell8.Items.Add("Ver. length");
                            dgvCmbCell8.Items.Add("Thickness of body(mm)");
                            dgvCmbCell8.Items.Add("Bending of body(mm)");
                            dgvCmbCell8.Items.Add("Pitch of thread");
                            dgvCmbCell8.Items.Add("Size of thread");
                            dgvCmbCell8.Items.Add("Lead angle of thread(0.5)");
                            dgvCmbCell8.Items.Add("Lead angle of thread(1)");
                            dgvCmbCell8.Items.Add("Brightness of rectangle ROI");
                            dgvCmbCell8.Items.Add("BLOB in rectangle ROI");
                            dgvCmbCell8.Items.Add("V Angle of bottom");
                            dgvCmbCell8.Items.Add("Concentricity");
                            dgvCmbCell8.Items.Add("Shape of bottom");
                            dgvCmbCell8.Items.Add("Distance between two area");
                            //dgvCmbCell8.Items.Add("Convex BLOB Difference");//14
                            if (main_str.Contains("Model find"))
                            {
                                dgvCmbCell8.Items.Add("Match rate(%)");
                            }
                            if (!tstr.Contains("Hor. length") && !tstr.Contains("Ver. length")
                                && !tstr.Contains("Concentricity") && !tstr.Contains("V Angle of bottom")
                                && !tstr.Contains("Pitch of thread") && !tstr.Contains("Size of thread")
                                && !tstr.Contains("Lead angle of thread(1)") && !tstr.Contains("Lead angle of thread(0.5)")
                                && !tstr.Contains("Thickness of body(mm)") && !tstr.Contains("Bending of body(mm)")
                                && !tstr.Contains("Brightness of rectangle ROI") && !tstr.Contains("BLOB in rectangle ROI")
                                && !tstr.Contains("Shape of bottom") && !tstr.Contains("Distance between two area") && !tstr.Contains("Match rate(%)")
                                //&& !tstr.Contains("Convex BLOB Difference") 
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
                    }
                    else if (tstr == "Concentricity") // 번
                    {
                        dataGridView1.Rows[5].Cells[1].Value = "Insp. area result use";
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
                        }
                        else
                        {
                            dataGridView1.Rows[13].Cells[0].Value = "Preliminary";
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
                        // 변수 추가
                        dataGridView1.Rows[12].Cells[0].Value = "Diameter of #1 circle(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "Thickness of #1 circle(mm)";
                        dataGridView1.Rows[14].Cells[0].Value = "Diameter of #2 circle(mm)";
                        dataGridView1.Rows[15].Cells[0].Value = "Thickness of #2 circle(mm)";
                        dataGridView1.Rows[16].Cells[0].Value = "Start(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[17].Cells[0].Value = "End(Angle)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) < -360 || Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) > 360)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 360;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 0 && Convert.ToInt32(dataGridView1.Rows[16].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[16].Cells[1].Value = 0;
                            dataGridView1.Rows[17].Cells[1].Value = 360;
                        }
                        dataGridView1.Rows[18].Cells[0].Value = "Filter size for noise reduction";
                        dataGridView1.Rows[19].Cells[0].Value = "Min. size of BLOB";
                        dataGridView1.Rows[20].Cells[0].Value = "Max. size of BLOB";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) == 0)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 100000;
                        }
                        dataGridView1.Rows[21].Cells[0].Value = "No. of boundary connection";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 2)
                        {
                            dataGridView1.Rows[21].Cells[1].Value = 2;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[21].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[22].Cells[0].Value = "Position(0:ROI#0,1:P Below,2:P Above)";
                        if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) > 2)
                        {
                            dataGridView1.Rows[22].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) < 3)
                        {
                            dataGridView1.Rows[23].Cells[0].Value = "P Threshold";
                            if (Convert.ToInt32(dataGridView1.Rows[23].Cells[1].Value.ToString()) < 0 || Convert.ToInt32(dataGridView1.Rows[23].Cells[1].Value.ToString()) > 255)
                            {
                                dataGridView1.Rows[23].Cells[1].Value = 0;
                            }
                        }
                        dataGridView1.Rows[24].Cells[0].Value = "Output(0:Pixel,1:Count,2:Height,3:Width,4:Axis angle,5:Angle(from center),6:Included angle,7:Pixel(except boundary),8:Pixel(boundary only))";
                        if (Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[24].Cells[1].Value = 0;
                        }
                        if (Convert.ToInt32(dataGridView1.Rows[24].Cells[1].Value.ToString()) > 8)
                        {
                            dataGridView1.Rows[24].Cells[1].Value = 8;
                        }
                        dataGridView1.Rows[25].Cells[0].Value = "Option(0:Not use,1:Convex BLOB difference,2:#1(less than v1) #2(more than v2))";
                        if (Convert.ToInt32(dataGridView1.Rows[25].Cells[1].Value.ToString()) > 2 || Convert.ToInt32(dataGridView1.Rows[25].Cells[1].Value.ToString()) <= -1)
                        {
                            dataGridView1.Rows[25].Cells[1].Value = 0;
                        }

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
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for noise reduction";
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
                        dataGridView1.Rows[17].Cells[0].Value = "Count Filter(CNT)";
                        if (Convert.ToInt32(dataGridView1.Rows[17].Cells[1].Value.ToString()) <= 0)
                        {
                            dataGridView1.Rows[17].Cells[1].Value = 1;
                        }
                        dataGridView1.Rows[18].Cells[0].Value = "Dark Thres.(Gray)";
                        if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) > 255)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 255;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[18].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[18].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[19].Cells[0].Value = "Bright Thres.(Gray)";
                        if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) > 255)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 255;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[19].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[19].Cells[1].Value = 0;
                        }
                        dataGridView1.Rows[20].Cells[0].Value = "Preprocessing(Blur)";
                        if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) > 100)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 100;
                        }
                        else if (Convert.ToInt32(dataGridView1.Rows[20].Cells[1].Value.ToString()) < 0)
                        {
                            dataGridView1.Rows[20].Cells[1].Value = 0;
                        }
                        if (tstr == "BLOB in rectangle ROI")
                        {
                            dataGridView1.Rows[21].Cells[0].Value = "Output(0:Pixel,1:Width,2:Height,3:Count,4:Position,5:Pixel(except boundary),6:Pixel(boundary only))";
                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) > 6 || Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[21].Cells[1].Value = 0;
                            }

                            if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 1)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. of Width";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. of Width";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 2)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. of Height";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. of Height";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 3)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. size of BLOB";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. size of BLOB";
                            }
                            else if (Convert.ToInt32(dataGridView1.Rows[21].Cells[1].Value.ToString()) == 4)
                            {
                                dataGridView1.Rows[13].Cells[0].Value = "Min. Distance";
                                dataGridView1.Rows[14].Cells[0].Value = "Max. Distance";
                            }
                            dataGridView1.Rows[22].Cells[0].Value = "Convex BLOB(0:Not use,1:Use)";
                            if (Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) > 1 || Convert.ToInt32(dataGridView1.Rows[22].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[22].Cells[1].Value = 0;
                            }
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
                    }
                    else if (tstr == "V Angle of bottom") // 3번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Height for cal.(mm)";
                        dataGridView1.Rows[13].Cells[0].Value = "Filter size for removing thread";
                    }
                    else if (tstr == "Bending of body(mm)" || tstr == "Thickness of body(mm)") // 7, 8번
                    {
                        dataGridView1.Rows[12].Cells[0].Value = "Filter size for removing thread";
                    }
                    else if (tstr == "Size of bottom")
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
                            dataGridView1.Rows[13].Cells[0].Value = "Method(0:Pixel,1:Width,2:Height,3:ConvexBLOB)"; // 이름
                            if (Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) > 3 || Convert.ToInt32(dataGridView1.Rows[13].Cells[1].Value.ToString()) <= -1)
                            {
                                dataGridView1.Rows[13].Cells[1].Value = 0;
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
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Inner Diameter"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Diameter";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Diameter"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Width Min(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Width Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Width Max(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Width Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Height Min(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Height Min(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Height Max(mm)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Height Max(mm)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Angle(degree)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Angle(degree)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ-Slit Angle(degree)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ-Slit Angle(degree)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Angle(degree)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Angle(degree)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Count(Number)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Count(Number)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Bending Count(Number)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Bending Count(Number)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Count(Number)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Count(Number)"; t_idx++;
                                listBox1.Items[t_idx] = "Slit Defect Size(pixel)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "Slit Defect Size(pixel)"; t_idx++;
                                listBox1.Items[t_idx] = "DJ Birdcaging(Number)";
                                IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[t_idx - 1][2] = "DJ Birdcaging(Number)"; t_idx++;

                                //IPSSTApp.Instance().m_Config.Save_Judge_Data();
                            }
                            else if (dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && Cam_Num != 0)
                            {
                                if (IPSSTApp.Instance().m_mainform.m_Start_Check && text_maintain)
                                {
                                    AutoClosingMessageBox.Show("Only CAM0 supported for SUGIYAMA ", "WARNING", 3000);
                                }
                            }
                            else
                            {
                                if (IPSSTApp.Instance().m_mainform.m_Start_Check && text_maintain && Cam_Num == 0)
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

                    for (int i = 12; i < dataGridView1.RowCount; i++)
                    {
                        dataGridView1.Rows[i].Visible = true;
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "Preliminary")
                        {
                            dataGridView1.Rows[i].Cells[1].Value = 0;
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.DimGray;
                            dataGridView1.Rows[i].Visible = false;
                        }
                        else if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "Custome No.")
                        {
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.SkyBlue;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightPink;
                        }
                    }
                }

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
        }

        private void ctr_ROI_Guide1_Load(object sender, EventArgs e)
        {

        }

        private int t_current_row_idx = 0;
        private int t_current_col_idx = 0;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                t_current_row_idx = e.RowIndex;
                t_current_col_idx = e.ColumnIndex;
                if (t_current_col_idx == 0)
                {
                    return;
                }
                if (e.RowIndex >= 1 && e.RowIndex <= 4)
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                if (e.RowIndex == 6 || e.RowIndex == 7 || e.RowIndex >= 10)
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;


                    if (e.RowIndex == 6 || e.RowIndex == 7 || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("어두운 임계값(Gray)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("밝은 임계값(Gray)")
                        || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Dark Thres.(Gray)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Bright Thres.(Gray)"))
                    {
                        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                        t_Frm_Trackbar.t_idx = e.RowIndex;
                        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                        t_Frm_Trackbar.TopLevel = true;

                        t_Frm_Trackbar.colorSlider.Minimum = 0;
                        t_Frm_Trackbar.colorSlider.Maximum = 255;
                        if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        t_Frm_Trackbar.Show();
                    }

                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("컬러 임계") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low Thres") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High Thres")
                         || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("시작각(Angle)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("종료각(Angle)")
                         || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Start(Angle)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("End(Angle)")
                        )
                    {
                        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                        t_Frm_Trackbar.t_idx = e.RowIndex;
                        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                        t_Frm_Trackbar.TopLevel = true;

                        t_Frm_Trackbar.colorSlider.Minimum = -360;
                        t_Frm_Trackbar.colorSlider.Maximum = 360;
                        if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        t_Frm_Trackbar.Show();
                    }

                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("검사대상크기") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Min. size of BLOB") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Max. size of BLOB"))
                    {
                        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                        t_Frm_Trackbar.t_idx = e.RowIndex;
                        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                        t_Frm_Trackbar.TopLevel = true;
                        if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하한") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Min"))
                        {
                            t_Frm_Trackbar.colorSlider.Minimum = 0;
                            t_Frm_Trackbar.colorSlider.Maximum = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height/100;
                        }
                        else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상한") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Max"))
                        {
                            t_Frm_Trackbar.colorSlider.Minimum = 0;
                            t_Frm_Trackbar.colorSlider.Maximum = pictureBox_Image.Image.Width * pictureBox_Image.Image.Height;
                        }

                        if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                        {
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        t_Frm_Trackbar.Show();
                    }
                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("지름(mm)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Diameter of") 
                        || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("두께(mm)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Thickness of"))
                    {
                        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                        t_Frm_Trackbar.t_idx = e.RowIndex;
                        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                        t_Frm_Trackbar.TopLevel = true;

                        double cam_resol = 1d;
                        if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                        {
                            double.TryParse(IPSSTApp.Instance().m_mainform.ctr_Camera_Setting1.textBox_RESOLUTION_X.Text, out cam_resol);
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                        {
                            double.TryParse(IPSSTApp.Instance().m_mainform.ctr_Camera_Setting2.textBox_RESOLUTION_X.Text, out cam_resol);
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                        {
                            double.TryParse(IPSSTApp.Instance().m_mainform.ctr_Camera_Setting3.textBox_RESOLUTION_X.Text, out cam_resol);
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                        {
                            double.TryParse(IPSSTApp.Instance().m_mainform.ctr_Camera_Setting4.textBox_RESOLUTION_X.Text, out cam_resol);
                        }

                        if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("지름(mm)") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Diameter of"))
                        {
                            t_Frm_Trackbar.t_Precision = cam_resol;
                            t_Frm_Trackbar.colorSlider.Minimum = 0;
                            int t_max_v = Math.Max(pictureBox_Image.Image.Width, pictureBox_Image.Image.Height);
                            t_Frm_Trackbar.colorSlider.Maximum = (int)((double)t_max_v);
                            t_Frm_Trackbar.colorSlider.ShowDivisionsText = true;
                            t_Frm_Trackbar.colorSlider.TickDivide = 1/(float)cam_resol;
                        }
                        else
                        {
                            t_Frm_Trackbar.t_Precision = cam_resol;
                            t_Frm_Trackbar.colorSlider.Minimum = 0;
                            float t_max_v = Math.Max(pictureBox_Image.Image.Width, pictureBox_Image.Image.Height); //float.Parse(dataGridView1.Rows[e.RowIndex - 1].Cells[1].Value.ToString());
                            t_Frm_Trackbar.colorSlider.Maximum = (int)t_max_v;//(int)((double)t_max_v * cam_resol/2);
                            t_Frm_Trackbar.colorSlider.ShowDivisionsText = true;
                            t_Frm_Trackbar.colorSlider.TickDivide = 1/(float)cam_resol;
                        }
                        if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                        {
                            //MessageBox.Show(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                            t_Frm_Trackbar.colorSlider.Value = (float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                        {
                            t_Frm_Trackbar.colorSlider.Value = (float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                        {
                            t_Frm_Trackbar.colorSlider.Value = (float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                        {
                            t_Frm_Trackbar.colorSlider.Value = (float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString())) / (float)cam_resol;
                        }
                        t_Frm_Trackbar.Show();
                    }

                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("측정범위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low bound") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High bound"))
                    {
                        Frm_Trackbar t_Frm_Trackbar = new Frm_Trackbar();
                        t_Frm_Trackbar.t_idx = e.RowIndex;
                        System.Drawing.Point location = dataGridView1.PointToScreen(System.Drawing.Point.Empty);
                        t_Frm_Trackbar.Location = new System.Drawing.Point(location.X + dataGridView1.Width + 2, location.Y + e.RowIndex * dataGridView1.Rows[e.RowIndex].Height - 70);
                        t_Frm_Trackbar.TopLevel = true;

                        if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = 0;
                                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                            }
                            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                                t_Frm_Trackbar.colorSlider.Maximum = 100;
                            }
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = 0;
                                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                            }
                            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                                t_Frm_Trackbar.colorSlider.Maximum = 100;
                            }
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = 0;
                                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                            }
                            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                                t_Frm_Trackbar.colorSlider.Maximum = 100;
                            }
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("하위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("Low"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = 0;
                                t_Frm_Trackbar.colorSlider.Maximum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex + 1][1].ToString()) - 1;
                            }
                            else if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("상위") || dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Contains("High"))
                            {
                                t_Frm_Trackbar.colorSlider.Minimum = int.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex - 1][1].ToString()) + 1;
                                t_Frm_Trackbar.colorSlider.Maximum = 100;
                            }
                            t_Frm_Trackbar.colorSlider.Value = float.Parse(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[e.RowIndex][1].ToString());
                        }
                        t_Frm_Trackbar.Show();
                    }
                }
            }
            catch
            {

            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        private void pictureBox_Image_DoubleClick(object sender, EventArgs e)
        {
            //Referesh_Select_Menu(true);
            //System.Drawing.Point loc = dataGridView1.PointToScreen(System.Drawing.Point.Empty);

            //uint X = (uint)loc.X + 5 + 100;
            //uint Y = (uint)loc.Y + 5;
            //SetCursorPos((int)X, (int)Y);
            //mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            //mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);

            if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
            {
                if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    pictureBox_Image.Refresh();
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, IPSSTApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex, IPSSTApp.Instance().m_Config.ROI_Cam_Num);
                    IPSSTApp.Instance().m_mainform.ctr_ROI1.button_INSPECTION_Click(sender, e);
                }
            }
            else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
            {
                if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    pictureBox_Image.Refresh();
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, IPSSTApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex, IPSSTApp.Instance().m_Config.ROI_Cam_Num);
                    IPSSTApp.Instance().m_mainform.ctr_ROI2.button_INSPECTION_Click(sender, e);
                }
            }
            else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
            {
                if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    pictureBox_Image.Refresh();
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, IPSSTApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex, IPSSTApp.Instance().m_Config.ROI_Cam_Num);
                    IPSSTApp.Instance().m_mainform.ctr_ROI3.button_INSPECTION_Click(sender, e);
                }
            }
            else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
            {
                if (!IPSSTApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.timer_Camera.Enabled && !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    pictureBox_Image.Refresh();
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.SubDB_to_MainDB();
                    IPSSTApp.Instance().m_Config.Set_Parameters();
                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(true, IPSSTApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex, IPSSTApp.Instance().m_Config.ROI_Cam_Num);
                    IPSSTApp.Instance().m_mainform.ctr_ROI4.button_INSPECTION_Click(sender, e);
                }
            }

            radioButton1.Checked = false;
            radioButton2.Checked = true;
            pictureBox_RImage.Refresh();
        }

        private void pictureBox_RImage_DoubleClick(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            pictureBox_Image.Refresh();
        }

        public void Change_initial_ROI()
        {
            if (listBox1.SelectedIndex > 0)
            {
                return;
            }
            for (int j = 1; j < listBox1.Items.Count; j++)
            {
                if ((listBox1.Items[j].ToString().Contains("ROI") &&
                    listBox1.Items[j].ToString().Contains("측정값")) || (listBox1.Items[j].ToString().Contains("ROI") &&
                    listBox1.Items[j].ToString().Contains("result")))
                {
                    if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
                    {
                        string[] str = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[j][1].ToString().Split('_');

                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[j][1] =
                        str[0]
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam0_rect[0].rect.X.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam0_rect[0].rect.Y.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam0_rect[0].rect.Width.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam0_rect[0].rect.Height.ToString()
                        + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
                        + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
                        + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
                        + "_" + str[23] + "_" + str[24] + "_" + str[25];
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
                    {
                        string[] str = IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[j][1].ToString().Split('_');

                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[j][1] =
                        str[0]
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam1_rect[0].rect.X.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam1_rect[0].rect.Y.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam1_rect[0].rect.Width.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam1_rect[0].rect.Height.ToString()
                        + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
                        + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
                        + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
                        + "_" + str[23] + "_" + str[24] + "_" + str[25];
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
                    {
                        string[] str = IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[j][1].ToString().Split('_');

                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[j][1] =
                        str[0]
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam2_rect[0].rect.X.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam2_rect[0].rect.Y.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam2_rect[0].rect.Width.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam2_rect[0].rect.Height.ToString()
                        + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
                        + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
                        + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
                        + "_" + str[23] + "_" + str[24] + "_" + str[25];
                    }
                    else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
                    {
                        string[] str = IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[j][1].ToString().Split('_');

                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[j][1] =
                        str[0]
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam3_rect[0].rect.X.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam3_rect[0].rect.Y.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam3_rect[0].rect.Width.ToString()
                        + "_" +
                        IPSSTApp.Instance().m_Config.Cam3_rect[0].rect.Height.ToString()
                        + "_" + str[5] + "_" + str[6] + "_" + str[7] + "_" + str[8] + "_" + str[9] + "_" + str[10]
                        + "_" + str[11] + "_" + str[12] + "_" + str[13] + "_" + str[14] + "_" + str[15] + "_" + str[16]
                        + "_" + str[17] + "_" + str[18] + "_" + str[19] + "_" + str[20] + "_" + str[21] + "_" + str[22]
                        + "_" + str[23] + "_" + str[24] + "_" + str[25];
                    }
                }
            }
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

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (t_current_col_idx == 1)
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        if (m_range_visible_check)
                        {
                            cm.MenuItems.Add("측정범위 설정 OFF", new EventHandler(SettingInterval));
                        }
                        else
                        {
                            cm.MenuItems.Add("측정범위 설정 ON", new EventHandler(SettingInterval));
                        }
                        cm.MenuItems.Add("변수값 초기화", new EventHandler(Change_Initial_Parameter));
                    }
                    else
                    {
                        if (m_range_visible_check)
                        {
                            cm.MenuItems.Add("Range setting OFF", new EventHandler(SettingInterval));
                        }
                        else
                        {
                            cm.MenuItems.Add("Range setting ON", new EventHandler(SettingInterval));
                        }
                        cm.MenuItems.Add("Initialize parameters", new EventHandler(Change_Initial_Parameter));
                    }

                    dataGridView1.ContextMenu = cm;
                    dataGridView1.ContextMenu.Show(dataGridView1, e.Location);
                    dataGridView1.ContextMenu = null;
                    return;
                }

                // 기존 코드
                ToolStripDropDown popup = new ToolStripDropDown();
                popup.Margin = Padding.Empty;
                popup.Padding = Padding.Empty;
                Ctr_Popup content = new Ctr_Popup();
                content.Height = 50;
                //content.t_Item = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[t_current_row_idx][0].ToString();
                content.t_Item = dataGridView1.Rows[t_current_row_idx].Cells[0].Value.ToString();
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
                //content.t_Item = IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[t_current_row_idx][0].ToString();
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
    }
}