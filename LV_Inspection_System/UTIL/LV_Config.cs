using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Threading;
using ZedGraph;
using System.Diagnostics;
using OpenCvSharp;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LV_Inspection_System.UTIL
{
    class LV_Config
    {
        public string m_Model_Name = string.Empty;                                    // 현재 모델 이름
        public int m_model_num = 0;                                         // 모델 번호
        public int m_Cam_Total_Num = 4;                                     // 카메라 총 수
        public double[,] m_Cam_Resolution = new double[10, 2];              // 카메라 가로,세로 해상도
        public bool[] m_Cam_Continuous_Mode = new bool[10];                 // 카메라 실시간 중인지 체크
        public bool[] m_Cam_Trigger_Check = new bool[10];                   // 카메라 트리거 모드인지 체크
        public bool[] m_Cam_Inspection_Check = new bool[10];                // 카메라 검사중인지 체크
        public bool[] m_Cam_MissedInspection_Check = new bool[10];                // 카메라 검사중인지 체크
        public bool m_Administrator_Password_Flag = true;                   // 관리자 권한 체크
        public bool m_Administrator_Super_Password_Flag = true;            // 슈퍼관리자 권한 체크
        public int m_Judge_Priority = 0;                                    // 판정 우선순위 결정
        public int m_Display_Control_Num = 2;
        public bool[,] m_ROI_ALG_Check = new bool[20, 4];                   // 카메라의 알고리즘 선택
        // 0 : 1<2<3
        // 1 : 1<3<2
        // 2 : 2<1<3
        // 3 : 2<3<1
        // 4 : 3<1<2
        // 5 : 3<2<1
        public bool[] m_Cam_Log_Use_Check = new bool[10];                   // 카메라 이미지 저장 유무
        public int m_Cam_Log_Method = 1;                                    // 카메라 이미지 저장 방법 0:OK만, 1:NG만, 2:모두
        public int m_Cam_Log_Date = 30;                                     // 카메라 이미지 저장 일수(Day)
        public int m_Cam_Log_Format = 1;                                    // 카메라 이미지 저장 포멧 0:bmp, 1:jpg, 2:png
        public bool m_Data_Log_Use_Check = true;                            // Data 저장 유무
        public int m_Data_Log_Date = 30;                                    // Data 저장 일수(Day)
        public int m_Log_Save_Num = 30;                                   // 검사 결과 저장 개수
        public string m_Log_Save_Folder = "";                               // 검사 결과 저장 폴더
        public string m_Log_Save_Folder2 = "";                               // 검사 결과 저장 폴더
        public string m_Data_Save_Folder = "";                               // 검사 결과 저장 폴더
        private DataTable[] destinationTable = new DataTable[5];

        public int[] m_Error_Flag = new int[10];                            // 검사 결과가 OK(0) 인지 NG(1)인지 무(-1)인지?
        public double[] m_Log_Data_Cnt = new double[10];                          // 검사 결과 로그 카운트
        public string[] m_TT = new string[10];                              // T/T
        public string[] m_FPS = new string[10];                             // FPS
        public string[] m_Probe = new string[10];                           // Probe
        public bool[,] m_Cam_Filp = new bool[10, 2];                        // 카메라 Filp(좌우, 상하 순)
        public int[] m_Cam_Rot = new int[10];                               // 카메라 회전 각도
        public int[] m_Cam_Kind = new int[10];                              // 카메라 종류(0:Area, 1:Line)
        public double[,] m_OK_NG_Cnt = new double[10, 4];                         // OK, NG 카운트
        public double[] m_OK_NG_Total_Miss_Cnt = new double[4];                   // OK, NG, Total, Miss 카운트
        public int m_PC_No = 0;
        public int[] m_Camera_Position = new int[4];                        // 각 카메라의 위치, 0이면 상/하부, 1이면 사이드
        public int[] nTableType = new int[4];
        public bool[] ctr_Camera_Setting_Force_USE = new bool[4];

        public bool m_Check_Inspection_Mode = false;                        // 자동검사 중인지 아닌지 체크
        public bool m_Check_Server_Operation = false;

        public DataSet ds_STATUS = null;//new DataSet();
        public DataSet ds_DATA_0 = null;//new DataSet();
        public DataSet ds_DATA_1 = null;//new DataSet();
        public DataSet ds_DATA_2 = null;//new DataSet();
        public DataSet ds_DATA_3 = null;//new DataSet();
        public DataSet ds_DATA_4 = null;//new DataSet();
        public DataSet ds_DATA_5 = null;//new DataSet();
        public DataSet ds_DATA_6 = null;//new DataSet();
        public DataSet ds_DATA_7 = null;//new DataSet();
        public DataSet ds_LOG = null;//new DataSet();
        public DataSet ds_YIELD = null;//new DataSet();

        public bool Realtime_View_Check = false;
        public bool Alg_Debugging = false;
        public bool Alg_TextView = false;
        public bool Diplay_Only_NG = false;
        public bool AI_Image_Save = true;
        public bool SSF_Image_Save = true;
        //const int NumOfThread = 4;
        //Thread[] threads = new Thread[NumOfThread];

        private int Total_Log_Save_Threads_JOB = 0;
        Thread Total_Log_Save_Threads;

        public UserRect[] Cam0_rect = new UserRect[41];
        public UserRect[] Cam1_rect = new UserRect[41];
        public UserRect[] Cam2_rect = new UserRect[41];
        public UserRect[] Cam3_rect = new UserRect[41];
        public int[] ROI_Selected_IDX = new int[4];
        public int ROI_Cam_Num = 0;
        //public string[] Disp_Error_List = new string[4];
        //public string[] Disp_OK_List = new string[4];
        //public int[] Disp_Error_List_CNT = new int[4];
        //public int[] Disp_OK_List_CNT = new int[4];

        public string[] Disp_OKNG_List = new string[4];
        public int[] Disp_OKNG_List_CNT = new int[4];

        public bool PLC_Pingpong_USE = false;
        public bool[] PLC_Pingpong_Check = new bool[4];
        public bool PLC_Once_Tx_USE = false;
        public int PLC_Station_Num = 0;

        public bool PLC_Judge_view = false;

        public bool[,] m_Bending_Check = new bool[4,40];
        public double[] m_Bending_value = new double[40]; // 휨 검사 합산 변수
        public int[] m_Bending_count = new int[40]; // 휨 검사 합산 할 개수
        public int[] m_Bending_count_tmp = new int[40]; // 휨 검사 합산 할 개수 Judge함수 내 사용

        public int[] m_Interlock_Cam = new int[4]; // 카메라 연동

        public bool[] Realtime_Running_Check = new bool[4];

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public UserRect[] AE_rect = new UserRect[4];

        public int neoTabWindow_MAIN_idx = 0;
        public int neoTabWindow_INSP_SETTING_idx = 0;
        public int neoTabWindow_INSP_SETTING_CAM_idx = 0;
        public int neoTabWindow_LOG_idx = 0;
        public int neoTabWindow2_LOG_idx = 0;

        public double NG_Log_Memory = 100;
        public double NG_Log_Max_CNT = 10;

        public int CAM_Refresh_CNT = 10;

        public int[] Inspection_Delay = new int[4];

        public int[] Tx_Idx = new int[4];
        public int Tx_Room_Num = 2;
        public bool Tx_Merge = false;
        public bool Disable_Menu = false;

        public bool Jog_Mode = false;

        public string m_lot_str = "";

        public bool m_Auto_Mode = false;


        public bool[] Image_Merge_Check = new bool[4] { false, false, false, false } ;
        public int[] Image_Merge_Number = new int[4] { 70, 70, 70, 70 };
        public int[] Image_Merge_Idx = new int[4] { 0, 0, 0, 0 };
        public Bitmap[] Image_Merge_BMP = new Bitmap[4];

        public struct GraphData
        {
            public string name;
            public bool use;
            public PointPairList list;
        }

        public GraphData[] m_GraphData0 = new GraphData[41];
        public GraphData[] m_GraphData1 = new GraphData[41];
        public GraphData[] m_GraphData2 = new GraphData[41];
        public GraphData[] m_GraphData3 = new GraphData[41];

        public Stopwatch[] Graph_SW = new Stopwatch[4];
        public int m_Graph_Update_sec = 5;

        public int CP_table_CamNum = 0;
        public int CP_table_Idx = 0;
        public bool CP_table_check = false;
        public bool NG_Log_Use = true;

        public CSVWriter[] CSVLog = new CSVWriter[5];
        public CSVWriter[] CSVDataLog = new CSVWriter[5];

        public int[] m_Cam_Trigger_Delay = new int[4];

        public struct AIParam
        {
            public bool USE;
            public int ROI_IDX;
            public Bitmap Image;
            public Rectangle ROI;
            public double Matching_Rate;
            public string Result;
            public long T_T;
            public double Result_MR;
            public int Result_IDX;
            public string Result_Label;
            public int ROI_Location;
            public Bitmap Mask;
            public Bitmap Cut_Mask;
        }

        public List<AIParam>[] m_AIParam = new List<AIParam>[4];
        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (m_Language == value)
                {
                    return;
                }
                m_Language = value;
                if (m_Language == 0 && LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex != 0)
                {
                    LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex = 0;
                }
                else if (m_Language == 1 && LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex != 1)
                {
                    LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex = 1;
                }
                else if (m_Language == 2 && LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex != 2)
                {
                    LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex = 2;
                }
                // 여기에 언어 관련 내용 넣기
                Initial_DataBase();
                LVApp.Instance().m_mainform.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_PLC1.m_SetLanguage = m_Language;
                int t_ROI_Cam_Num = LVApp.Instance().m_Config.ROI_Cam_Num;
                LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                LVApp.Instance().m_mainform.ctr_ROI1.m_SetLanguage = m_Language;
                LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                LVApp.Instance().m_mainform.ctr_ROI2.m_SetLanguage = m_Language;
                LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                LVApp.Instance().m_mainform.ctr_ROI3.m_SetLanguage = m_Language;
                LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                LVApp.Instance().m_mainform.ctr_ROI4.m_SetLanguage = m_Language;
                LVApp.Instance().m_Config.ROI_Cam_Num = t_ROI_Cam_Num;
                LVApp.Instance().m_mainform.ctr_Model1.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Manual1.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Log1.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.m_SetLanguage = m_Language;
                LVApp.Instance().m_mainform.ctr_Model1.ctr_History1.m_SetLanguage = m_Language;
                //LVApp.Instance().t_QuickMenu.m_SetLanguage = m_Language;
            }
        }


        public void Initial_DataBase()
        {
            if (ds_STATUS != null)
            {
                ds_STATUS = null;
            }
            ds_STATUS = new DataSet();

            if (Total_Log_Save_Threads == null)
            {
                t_bool_log_Total[0] = true;
                t_bool_log_Total[1] = true;
                t_bool_log_Total[2] = true;
                t_bool_log_Total[3] = true;

                m_AIParam[0] = new List<AIParam>();
                m_AIParam[1] = new List<AIParam>();
                m_AIParam[2] = new List<AIParam>();
                m_AIParam[3] = new List<AIParam>();

                Total_Log_Save_Threads = new Thread(LogThreadProc_Total);
                Total_Log_Save_Threads.IsBackground = true;
                Total_Log_Save_Threads.Start();
            }

            for (int i = 0; i < 10; i++)
            {
                m_Cam_Continuous_Mode[i] = false;
                m_Cam_Inspection_Check[i] = false;
                if (i < 4)
                {
                    m_Camera_Position[i] = 0;
                    LVApp.Instance().m_Config.m_Interlock_Cam[i] = -1;
                    ROI_Selected_IDX[i] = 0;
                }
            }
            for (int i = 0; i < 20; i++)
            {
                m_ROI_ALG_Check[i, 0] = true;
                m_ROI_ALG_Check[i, 1] = true;
                m_ROI_ALG_Check[i, 2] = true;
                m_ROI_ALG_Check[i, 3] = true;
            }
            //==========================================================================================================//
            // 메인에 AUTO STATUS 초기화
            DataTable table0 = new DataTable("AUTO STATUS");
            if (m_SetLanguage == 0)
            {//한국어
                table0.Columns.Add("구분");//1
                table0.Columns.Add("값");//2
                table0.Rows.Add("조명", "정상");//7
                table0.Rows.Add("카메라", "연결대기"); //8
                table0.Rows.Add("통신", "연결대기");//9
            }
            else if (m_SetLanguage == 1)
            {//영어
                table0.Columns.Add("Item");//1
                table0.Columns.Add("Value");//2
                table0.Rows.Add("Light", "OK");//7
                table0.Rows.Add("Camera", "Disconn."); //8
                table0.Rows.Add("PLC", "Disconn.");//9
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table0.Columns.Add("Item");//1
                table0.Columns.Add("Value");//2
                table0.Rows.Add("Light", "OK");//7
                table0.Rows.Add("Camera", "Disconn."); //8
                table0.Rows.Add("PLC", "Disconn.");//9
            }
            ds_STATUS.Tables.Add(table0);
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.DataSource = ds_STATUS.Tables[0];
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToAddRows = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToDeleteRows = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToResizeColumns = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToResizeRows = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowHeadersWidth = 5;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowTemplate.Height = 43;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].Width = 80;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows[0].Cells[0].Style.BackColor = Color.LightGreen;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows[1].Cells[0].Style.BackColor = Color.LightGreen;
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows[2].Cells[0].Style.BackColor = Color.LightGreen;

            foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows)
            {
                row.Height = (LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Height - LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeight) / LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows.Count;
            }
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ReadOnly = true;

            foreach (DataGridViewColumn column in LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ClearSelection();
            // 메인에 AUTO STATUS 초기화 끝
            //==========================================================================================================//

            //==========================================================================================================//
            // 메인에 AUTO COUNT 초기화
            DataTable table1 = new DataTable("AUTO COUNT");
            if (m_SetLanguage == 0)
            {//한국어
                table1.Columns.Add("구분");
            }
            else if (m_SetLanguage == 1)
            {//영어
                table1.Columns.Add("Item");//1
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table1.Columns.Add("Item");//1
            }

            table1.Columns.Add("OK//TOTAL");//1
            table1.Columns.Add("NG/NO/YIELD");//1

            //table1.Columns.Add("OK", Type.GetType("System.Int32"));
            //table1.Columns.Add("NG", Type.GetType("System.Int32"));
            //table1.Columns.Add("TOTAL", Type.GetType("System.Int32"));
            //table1.Columns.Add("YIELD");
            if (m_Cam_Total_Num >= 1)
            {
                table1.Rows.Add("CAM", 0, 0);
                table1.Rows.Add("#0", 0, 0);
                table1.Rows.Add("#0", 0, 0);
            }
            if (m_Cam_Total_Num >= 2)
            {
                table1.Rows.Add("CAM", 0, 0);
                table1.Rows.Add("#1", 0, 0);
                table1.Rows.Add("#1", 0, 0);
            }
            if (m_Cam_Total_Num >= 3)
            {
                table1.Rows.Add("CAM", 0, 0);
                table1.Rows.Add("#2", 0, 0);
                table1.Rows.Add("#2", 0, 0);
            }
            if (m_Cam_Total_Num >= 4)
            {
                table1.Rows.Add("CAM", 0, 0);
                table1.Rows.Add("#3", 0, 0);
                table1.Rows.Add("#3", 0, 0);
            }
            ds_STATUS.Tables.Add(table1);

            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.DataSource = ds_STATUS.Tables[1];
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.AllowUserToAddRows = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.AllowUserToDeleteRows = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.AllowUserToResizeColumns = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.AllowUserToResizeRows = false;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.RowHeadersWidth = 5;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.RowTemplate.Height = 43;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Font = new System.Drawing.Font("맑은 고딕", 8.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[0].Width = 35;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[0].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[1].DefaultCellStyle.BackColor = Color.LightBlue;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[2].DefaultCellStyle.BackColor = Color.LightPink;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns[3].DefaultCellStyle.BackColor = Color.LightGray;
            for (int t_idx = 0; t_idx < 4; t_idx++)
            {
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3].Cells[0].Style.BackColor = Color.LightGoldenrodYellow;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3].Cells[1].Style.BackColor = Color.LightSkyBlue;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3].Cells[2].Style.BackColor = Color.LightPink;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3 + 1].Cells[0].Style.BackColor = Color.LightGoldenrodYellow;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3 + 1].Cells[1].Style.BackColor = Color.Gray;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3 + 1].Cells[2].Style.BackColor = Color.LightGray;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3 + 2].Cells[0].Style.BackColor = Color.LightGoldenrodYellow;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3 + 2].Cells[1].Style.BackColor = Color.WhiteSmoke;
                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[t_idx * 3 + 2].Cells[2].Style.BackColor = Color.LightYellow;
            }

            foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows)
            {
                row.Height = (LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Height - LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersHeight) / LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows.Count;
            }
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ReadOnly = true;

            foreach (DataGridViewColumn column in LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ClearSelection();

            // DB에 현 상태 기록을 위한 DB 초기화
            DataTable table2 = new DataTable("DB");

            table2.Columns.Add("Item");//1
            table2.Columns.Add("Value");
            table2.Rows.Add("Current model", "");
            table2.Rows.Add("Server ip", "");
            table2.Rows.Add("Server port", "");
            ds_STATUS.Tables.Add(table2);

            // 메인에 AUTO COUNT 초기화 끝
            //==========================================================================================================//
            if (m_Cam_Total_Num >= 1)
            {
                Initialize_Setting_0();
            }
            if (m_Cam_Total_Num >= 2)
            {
                Initialize_Setting_1();
            }
            if (m_Cam_Total_Num >= 3)
            {
                Initialize_Setting_2();
            }
            if (m_Cam_Total_Num >= 4)
            {
                Initialize_Setting_3();
            }
            //Initialize_Setting_4();
            //Initialize_Setting_5();
            //Initialize_Setting_6();
            //Initialize_Setting_7();
            //Initialize_Data_Log();

            for (int i = 0; i < 41; i++)
            {
                m_GraphData0[i].name = "";
                m_GraphData0[i].use = false;
                m_GraphData0[i].list = new PointPairList();
                m_GraphData1[i].name = "";
                m_GraphData1[i].use = false;
                m_GraphData1[i].list = new PointPairList();
                m_GraphData2[i].name = "";
                m_GraphData2[i].use = false;
                m_GraphData2[i].list = new PointPairList();
                m_GraphData3[i].name = "";
                m_GraphData3[i].use = false;
                m_GraphData3[i].list = new PointPairList();
            }
            Graph_SW[0] = new Stopwatch(); Graph_SW[0].Start();
            Graph_SW[1] = new Stopwatch(); Graph_SW[1].Start();
            Graph_SW[2] = new Stopwatch(); Graph_SW[2].Start();
            Graph_SW[3] = new Stopwatch(); Graph_SW[3].Start();


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (ds_YIELD != null)
            {
                ds_YIELD = null;
            }
            ds_YIELD = new DataSet();
            //==========================================================================================================//
            // 메인에 ds_YIELD 초기화
            for (int i = 0; i < 4; i++)
            {
                DataTable table_yield = new DataTable("C" + i.ToString() + " YIELD LOG");
                if (m_SetLanguage == 0)
                {//한국어
                    table_yield.Columns.Add("항목명");//1
                    table_yield.Columns.Add("불량수");//2
                    table_yield.Columns.Add("수율");//1
                    table_yield.Columns.Add("양품평균");//2
                }
                else if (m_SetLanguage == 1)
                {//영어
                    table_yield.Columns.Add("Item");//1
                    table_yield.Columns.Add("NG");//2
                    table_yield.Columns.Add("Yield");//1
                    table_yield.Columns.Add("OK Avg.");//2
                }
                else if (m_SetLanguage == 2)
                {//중국어
                    table_yield.Columns.Add("Item");//1
                    table_yield.Columns.Add("NG");//2
                    table_yield.Columns.Add("产量");//1
                    table_yield.Columns.Add("OK Avg.");//2
                }
                ds_YIELD.Tables.Add(table_yield);
                LVApp.Instance().m_ctr_yield[i].dataGridView1.DataSource = ds_YIELD.Tables[i];
                LVApp.Instance().m_ctr_yield[i].dataGridView1.AllowUserToAddRows = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.AllowUserToDeleteRows = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.AllowUserToResizeColumns = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.AllowUserToResizeRows = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.RowHeadersWidth = 5;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.ColumnHeadersHeight = 26;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.RowTemplate.Height = 43;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                LVApp.Instance().m_ctr_yield[i].dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                LVApp.Instance().m_ctr_yield[i].dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //LVApp.Instance().m_ctr_yield[i].dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //LVApp.Instance().m_ctr_yield[i].dataGridView1.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                LVApp.Instance().m_ctr_yield[i].dataGridView1.Columns[0].Width = 150;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.ReadOnly = true;
                LVApp.Instance().m_ctr_yield[i].dataGridView1.ClearSelection();

            }
            for (int i = 0; i < 4; i++)
            {
                DataTable table_yield = new DataTable("C" + i.ToString() + " STATUS LOG");
                if (m_SetLanguage == 0)
                {//한국어
                    table_yield.Columns.Add("검사수량");//1
                    table_yield.Columns.Add("양품수량");//2
                    table_yield.Columns.Add("불량수량");//1
                    table_yield.Columns.Add("양품평균");//2
                }
                else if (m_SetLanguage == 1)
                {//영어
                    table_yield.Columns.Add("Total");//1
                    table_yield.Columns.Add("OK");//2
                    table_yield.Columns.Add("NG");//1
                    table_yield.Columns.Add("Yield");//2
                }
                else if (m_SetLanguage == 2)
                {//중국어
                    table_yield.Columns.Add("Total");//1
                    table_yield.Columns.Add("OK");//2
                    table_yield.Columns.Add("NG");//1
                    table_yield.Columns.Add("产量");//2
                }
                ds_YIELD.Tables.Add(table_yield);
                LVApp.Instance().m_ctr_yield[i].dataGridView2.DataSource = ds_YIELD.Tables[4+i];
                LVApp.Instance().m_ctr_yield[i].dataGridView2.AllowUserToAddRows = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.AllowUserToDeleteRows = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.AllowUserToResizeColumns = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.AllowUserToResizeRows = false;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.RowHeadersWidth = 5;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.ColumnHeadersHeight = 26;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.RowTemplate.Height = 43;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                LVApp.Instance().m_ctr_yield[i].dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                LVApp.Instance().m_ctr_yield[i].dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //LVApp.Instance().m_ctr_yield[i].dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //LVApp.Instance().m_ctr_yield[i].dataGridView2.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                //LVApp.Instance().m_ctr_yield[i].dataGridView2.Columns[0].Width = 150;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.ReadOnly = true;
                LVApp.Instance().m_ctr_yield[i].dataGridView2.ClearSelection();
            }


            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToAddRows = false;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToDeleteRows = false;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToResizeColumns = false;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AllowUserToResizeRows = false;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowHeadersWidth = 5;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeight = 26;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowTemplate.Height = 43;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].Width = 80;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[0].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows[0].Cells[0].Style.BackColor = Color.LightGreen;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows[1].Cells[0].Style.BackColor = Color.LightGreen;
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows[2].Cells[0].Style.BackColor = Color.LightGreen;

            //foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows)
            //{
            //    row.Height = (LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Height - LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeight) / LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows.Count;
            //}
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ReadOnly = true;

            //foreach (DataGridViewColumn column in LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Columns)
            //{
            //    column.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
            //LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ClearSelection();

        }

        void Initialize_Setting_0()
        {
            if (ds_DATA_0 != null)
            {
                ds_DATA_0 = null;
            }
            ds_DATA_0 = new DataSet();
            //==========================================================================================================//
            // 카메라 0 설정 Talbe 초기화
            DataTable table_setting_0 = new DataTable("Setting 0");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_0.Columns.Add(new DataColumn("사용", typeof(bool)));
                table_setting_0.Columns.Add("알고리즘");
                table_setting_0.Columns.Add("측정 아이템");

                DataColumn cmbComboBox2 = new DataColumn("판정 우선순위");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C0:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 측정 영역 설정", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C0:" + i.ToString("00"), "ROI#" + i.ToString("00") + " Measurement Area Setting", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C0:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 测量区域设置", "CLASS 1");
                }
            }

            ds_DATA_0.Tables.Add(table_setting_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_0.DataSource = ds_DATA_0.Tables[0];

            DataGridViewComboBoxColumn comboboxColumn2 = new DataGridViewComboBoxColumn();
            comboboxColumn2.Items.Add("CLASS 1");
            comboboxColumn2.Items.Add("CLASS 2");
            comboboxColumn2.Items.Add("CLASS 3");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn2.DataPropertyName = "판정 우선순위";
                comboboxColumn2.Name = "판정 우선순위";
                LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns.Remove("판정 우선순위");
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns.Remove("Judge Priority");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns.Remove("Judge Priority");
            }
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns.Insert(3, comboboxColumn2);


            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;


            LVApp.Instance().m_mainform.dataGridView_Setting_0.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[0].Width = 15;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[1].Width = 70;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[3].Width = 110;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[1].Visible = false;
            // 카메라 0 설정 Talbe 초기화 끝
            //==========================================================================================================//

            //==========================================================================================================//
            // 카메라 0 설정값 Talbe 초기화
            DataTable table_setting_Value_0 = new DataTable("Setting Value 0");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_Value_0.Columns.Add("알고리즘");

                DataColumn cmbComboBox3 = new DataColumn("측정방법");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("오프셋");
                table_setting_Value_0.Columns.Add("측정값");

                DataColumn cmbComboBox1 = new DataColumn("판정방법");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("기준값");
                table_setting_Value_0.Columns.Add("최솟값");
                table_setting_Value_0.Columns.Add("최댓값");
                table_setting_Value_0.Columns.Add("양품평균");
                table_setting_Value_0.Columns.Add("불량평균");
                table_setting_Value_0.Columns.Add("양품수");
                table_setting_Value_0.Columns.Add("불량수");
                table_setting_Value_0.Columns.Add("수율(%)");
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }

            for (int i = 1; i <= 40; i++)
            {
                table_setting_Value_0.Rows.Add("C0:" + i.ToString("00"), "From ALG.", 0, 0, "Range", 0, 0, 0, 0, 0, 0, 0, 0);
            }

            ds_DATA_0.Tables.Add(table_setting_Value_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DataSource = ds_DATA_0.Tables[1];

            DataGridViewComboBoxColumn comboboxColumn1 = new DataGridViewComboBoxColumn();
            comboboxColumn1.Items.Add("MIN");
            comboboxColumn1.Items.Add("AVERAGE");
            comboboxColumn1.Items.Add("MAX");
            comboboxColumn1.Items.Add("RANGE(MAX-MIN)");
            comboboxColumn1.Items.Add("From ALG.");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn1.DataPropertyName = "측정방법";
                comboboxColumn1.Name = "측정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Remove("측정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "판정방법";
                comboboxColumn.Name = "판정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Remove("판정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns.Insert(4, comboboxColumn);
            }

            for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ColumnCount; i++)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[0].Width = 50;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[1].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[9].Visible = false;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[1].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[3].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[8].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[9].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[10].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[11].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[12].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[3].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[8].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[9].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[12].DefaultCellStyle.BackColor = Color.Gainsboro;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[2].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[5].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[6].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[7].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[10].DefaultCellStyle.BackColor = Color.LightBlue;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[11].DefaultCellStyle.BackColor = Color.LavenderBlush;

            DataTable table_ROI_0_1 = new DataTable("ROI 0_1");
            if (m_SetLanguage == 0)
            {//한국어
                table_ROI_0_1.Columns.Add("구분");
                table_ROI_0_1.Columns.Add("값");

                table_ROI_0_1.Rows.Add("ROI00", "O_100_100_100_100_v1 이하_50_200_중심 기준_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_0_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_검사 영역 결과 사용_0_255_가로 길이_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_0_1.Rows.Add("Ratio", "1_1");
                ds_DATA_0.Tables.Add(table_ROI_0_1);

                DataTable table_ROI_0_2 = new DataTable("ROI 0_2");
                table_ROI_0_2.Columns.Add("구분");
                table_ROI_0_2.Columns.Add("값");
                table_ROI_0_2.Rows.Add("사용", "X"); //0
                table_ROI_0_2.Rows.Add("좌상(X)", "100");//1
                table_ROI_0_2.Rows.Add("좌상(Y)", "100");//2
                table_ROI_0_2.Rows.Add("가로(W)", "100");//3
                table_ROI_0_2.Rows.Add("높이(H)", "100");//4

                table_ROI_0_2.Rows.Add("임계화 방법", "v1 이하");//5
                table_ROI_0_2.Rows.Add("하위 임계값(v1,Gray)", "50");//6
                table_ROI_0_2.Rows.Add("상위 임계값(v2,Gray)", "200");//7
                table_ROI_0_2.Rows.Add("측정 방법", "가로 길이");//8
                table_ROI_0_2.Rows.Add("측정값 출력 방법", "평균값");//9
                table_ROI_0_2.Rows.Add("하위 측정범위(p1,%) ", "20");//10
                table_ROI_0_2.Rows.Add("상위 측정범위(p2,%)", "80");//11
                table_ROI_0_2.Rows.Add("예비변수", "0");//12
                table_ROI_0_2.Rows.Add("예비변수", "0");//13
                table_ROI_0_2.Rows.Add("예비변수", "0");//14
                table_ROI_0_2.Rows.Add("예비변수", "0");//15
                table_ROI_0_2.Rows.Add("예비변수", "0");//16
                table_ROI_0_2.Rows.Add("예비변수", "0");//17
                table_ROI_0_2.Rows.Add("예비변수", "0");//18
                table_ROI_0_2.Rows.Add("예비변수", "0");//19
                table_ROI_0_2.Rows.Add("예비변수", "0");//20
                table_ROI_0_2.Rows.Add("예비변수", "0");//21
                table_ROI_0_2.Rows.Add("예비변수", "0");//22
                table_ROI_0_2.Rows.Add("예비변수", "0");//23
                table_ROI_0_2.Rows.Add("예비변수", "0");//24
                table_ROI_0_2.Rows.Add("예비변수", "0");//25

                ds_DATA_0.Tables.Add(table_ROI_0_2);
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_ROI_0_1.Columns.Add("Item");
                table_ROI_0_1.Columns.Add("Value");

                table_ROI_0_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_0_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_0_1.Rows.Add("Ratio", "1_1");

                ds_DATA_0.Tables.Add(table_ROI_0_1);

                DataTable table_ROI_0_2 = new DataTable("ROI 0_2");
                table_ROI_0_2.Columns.Add("Item");
                table_ROI_0_2.Columns.Add("Value");
                table_ROI_0_2.Rows.Add("Use", "X"); //0
                table_ROI_0_2.Rows.Add("LT(X)", "100");//1
                table_ROI_0_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_0_2.Rows.Add("Width(W)", "100");//3
                table_ROI_0_2.Rows.Add("Height(H)", "100");//4

                table_ROI_0_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_0_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_0_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_0_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_0_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_0_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_0_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_0_2.Rows.Add("Preliminary", "0");//12
                table_ROI_0_2.Rows.Add("Preliminary", "0");//13
                table_ROI_0_2.Rows.Add("Preliminary", "0");//14
                table_ROI_0_2.Rows.Add("Preliminary", "0");//15
                table_ROI_0_2.Rows.Add("Preliminary", "0");//16
                table_ROI_0_2.Rows.Add("Preliminary", "0");//17
                table_ROI_0_2.Rows.Add("Preliminary", "0");//18
                table_ROI_0_2.Rows.Add("Preliminary", "0");//19
                table_ROI_0_2.Rows.Add("Preliminary", "0");//20
                table_ROI_0_2.Rows.Add("Preliminary", "0");//21
                table_ROI_0_2.Rows.Add("Preliminary", "0");//22
                table_ROI_0_2.Rows.Add("Preliminary", "0");//23
                table_ROI_0_2.Rows.Add("Preliminary", "0");//24
                table_ROI_0_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_0.Tables.Add(table_ROI_0_2);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_ROI_0_1.Columns.Add("Item");
                table_ROI_0_1.Columns.Add("Value");

                table_ROI_0_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_0_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_0_1.Rows.Add("Ratio", "1_1");

                ds_DATA_0.Tables.Add(table_ROI_0_1);

                DataTable table_ROI_0_2 = new DataTable("ROI 0_2");
                table_ROI_0_2.Columns.Add("Item");
                table_ROI_0_2.Columns.Add("Value");
                table_ROI_0_2.Rows.Add("Use", "X"); //0
                table_ROI_0_2.Rows.Add("LT(X)", "100");//1
                table_ROI_0_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_0_2.Rows.Add("Width(W)", "100");//3
                table_ROI_0_2.Rows.Add("Height(H)", "100");//4

                table_ROI_0_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_0_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_0_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_0_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_0_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_0_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_0_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_0_2.Rows.Add("Preliminary", "0");//12
                table_ROI_0_2.Rows.Add("Preliminary", "0");//13
                table_ROI_0_2.Rows.Add("Preliminary", "0");//14
                table_ROI_0_2.Rows.Add("Preliminary", "0");//15
                table_ROI_0_2.Rows.Add("Preliminary", "0");//16
                table_ROI_0_2.Rows.Add("Preliminary", "0");//17
                table_ROI_0_2.Rows.Add("Preliminary", "0");//18
                table_ROI_0_2.Rows.Add("Preliminary", "0");//19
                table_ROI_0_2.Rows.Add("Preliminary", "0");//20
                table_ROI_0_2.Rows.Add("Preliminary", "0");//21
                table_ROI_0_2.Rows.Add("Preliminary", "0");//22
                table_ROI_0_2.Rows.Add("Preliminary", "0");//23
                table_ROI_0_2.Rows.Add("Preliminary", "0");//24
                table_ROI_0_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_0.Tables.Add(table_ROI_0_2);
            }

            // 카메라 0 설정값 Talbe 초기화 끝
            //==========================================================================================================//
        }

        void Initialize_Setting_1()
        {
            if (ds_DATA_1 != null)
            {
                ds_DATA_1 = null;
            }
            ds_DATA_1 = new DataSet();
            //==========================================================================================================//
            // 카메라 0 설정 Talbe 초기화
            DataTable table_setting_0 = new DataTable("Setting 1");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_0.Columns.Add(new DataColumn("사용", typeof(bool)));
                table_setting_0.Columns.Add("알고리즘");
                table_setting_0.Columns.Add("측정 아이템");

                DataColumn cmbComboBox2 = new DataColumn("판정 우선순위");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C1:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 측정 영역 설정", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C1:" + i.ToString("00"), "ROI#" + i.ToString("00") + " Measurement Area Setting", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C1:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 测量区域设置", "CLASS 1");
                }
            }

            ds_DATA_1.Tables.Add(table_setting_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_1.DataSource = ds_DATA_1.Tables[0];

            DataGridViewComboBoxColumn comboboxColumn2 = new DataGridViewComboBoxColumn();
            comboboxColumn2.Items.Add("CLASS 1");
            comboboxColumn2.Items.Add("CLASS 2");
            comboboxColumn2.Items.Add("CLASS 3");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn2.DataPropertyName = "판정 우선순위";
                comboboxColumn2.Name = "판정 우선순위";
                LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns.Remove("판정 우선순위");
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns.Remove("Judge Priority");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns.Remove("Judge Priority");
            }
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns.Insert(3, comboboxColumn2);


            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;


            LVApp.Instance().m_mainform.dataGridView_Setting_1.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[0].Width = 15;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[1].Width = 70;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[3].Width = 110;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[1].Visible = false;
            // 카메라 0 설정 Talbe 초기화 끝
            //==========================================================================================================//

            //==========================================================================================================//
            // 카메라 0 설정값 Talbe 초기화
            DataTable table_setting_Value_0 = new DataTable("Setting Value 1");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_Value_0.Columns.Add("알고리즘");

                DataColumn cmbComboBox3 = new DataColumn("측정방법");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("오프셋");
                table_setting_Value_0.Columns.Add("측정값");

                DataColumn cmbComboBox1 = new DataColumn("판정방법");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("기준값");
                table_setting_Value_0.Columns.Add("최솟값");
                table_setting_Value_0.Columns.Add("최댓값");
                table_setting_Value_0.Columns.Add("양품평균");
                table_setting_Value_0.Columns.Add("불량평균");
                table_setting_Value_0.Columns.Add("양품수");
                table_setting_Value_0.Columns.Add("불량수");
                table_setting_Value_0.Columns.Add("수율(%)");
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }


            for (int i = 1; i <= 40; i++)
            {
                table_setting_Value_0.Rows.Add("C1:" + i.ToString("00"), "From ALG.", 0, 0, "Range", 0, 0, 0, 0, 0, 0, 0, 0);
            }
            ds_DATA_1.Tables.Add(table_setting_Value_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DataSource = ds_DATA_1.Tables[1];

            DataGridViewComboBoxColumn comboboxColumn1 = new DataGridViewComboBoxColumn();
            comboboxColumn1.Items.Add("MIN");
            comboboxColumn1.Items.Add("AVERAGE");
            comboboxColumn1.Items.Add("MAX");
            comboboxColumn1.Items.Add("RANGE(MAX-MIN)");
            comboboxColumn1.Items.Add("From ALG.");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn1.DataPropertyName = "측정방법";
                comboboxColumn1.Name = "측정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Remove("측정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "판정방법";
                comboboxColumn.Name = "판정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Remove("판정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns.Insert(4, comboboxColumn);
            }

            for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ColumnCount; i++)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[0].Width = 50;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[1].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[9].Visible = false;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[1].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[3].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[8].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[9].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[10].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[11].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[12].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[3].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[8].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[9].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[12].DefaultCellStyle.BackColor = Color.Gainsboro;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[2].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[5].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[6].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[7].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[10].DefaultCellStyle.BackColor = Color.LightBlue;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[11].DefaultCellStyle.BackColor = Color.LavenderBlush;

            DataTable table_ROI_1_1 = new DataTable("ROI 1_1");
            if (m_SetLanguage == 0)
            {//한국어
                table_ROI_1_1.Columns.Add("구분");
                table_ROI_1_1.Columns.Add("값");
                table_ROI_1_1.Rows.Add("ROI00", "O_100_100_100_100_v1 이하_50_200_중심 기준_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_1_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_검사 영역 결과 사용_0_255_가로 길이_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_1_1.Rows.Add("Ratio", "1_1");

                ds_DATA_1.Tables.Add(table_ROI_1_1);

                DataTable table_ROI_1_2 = new DataTable("ROI 1_2");
                table_ROI_1_2.Columns.Add("구분");
                table_ROI_1_2.Columns.Add("값");
                table_ROI_1_2.Rows.Add("사용", "X"); //0
                table_ROI_1_2.Rows.Add("좌상(X)", "100");//1
                table_ROI_1_2.Rows.Add("좌상(Y)", "100");//2
                table_ROI_1_2.Rows.Add("가로(W)", "100");//3
                table_ROI_1_2.Rows.Add("높이(H)", "100");//4

                table_ROI_1_2.Rows.Add("임계화 방법", "v1 이하");//5
                table_ROI_1_2.Rows.Add("하위 임계값(v1,Gray)", "50");//6
                table_ROI_1_2.Rows.Add("상위 임계값(v2,Gray)", "200");//7
                table_ROI_1_2.Rows.Add("측정 방법", "가로 길이");//8
                table_ROI_1_2.Rows.Add("측정값 출력 방법", "평균값");//9
                table_ROI_1_2.Rows.Add("하위 측정범위(p1,%) ", "20");//10
                table_ROI_1_2.Rows.Add("상위 측정범위(p2,%)", "80");//11
                table_ROI_1_2.Rows.Add("예비변수", "0");//12
                table_ROI_1_2.Rows.Add("예비변수", "0");//13
                table_ROI_1_2.Rows.Add("예비변수", "0");//14
                table_ROI_1_2.Rows.Add("예비변수", "0");//15
                table_ROI_1_2.Rows.Add("예비변수", "0");//16
                table_ROI_1_2.Rows.Add("예비변수", "0");//17
                table_ROI_1_2.Rows.Add("예비변수", "0");//18
                table_ROI_1_2.Rows.Add("예비변수", "0");//19
                table_ROI_1_2.Rows.Add("예비변수", "0");//20
                table_ROI_1_2.Rows.Add("예비변수", "0");//21
                table_ROI_1_2.Rows.Add("예비변수", "0");//22
                table_ROI_1_2.Rows.Add("예비변수", "0");//23
                table_ROI_1_2.Rows.Add("예비변수", "0");//24
                table_ROI_1_2.Rows.Add("예비변수", "0");//25

                ds_DATA_1.Tables.Add(table_ROI_1_2);
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_ROI_1_1.Columns.Add("Item");
                table_ROI_1_1.Columns.Add("Value");
                table_ROI_1_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_1_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_1_1.Rows.Add("Ratio", "1_1");

                ds_DATA_1.Tables.Add(table_ROI_1_1);

                DataTable table_ROI_1_2 = new DataTable("ROI 1_2");
                table_ROI_1_2.Columns.Add("Item");
                table_ROI_1_2.Columns.Add("Value");
                table_ROI_1_2.Rows.Add("Use", "X"); //0
                table_ROI_1_2.Rows.Add("LT(X)", "100");//1
                table_ROI_1_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_1_2.Rows.Add("Width(W)", "100");//3
                table_ROI_1_2.Rows.Add("Height(H)", "100");//4

                table_ROI_1_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_1_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_1_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_1_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_1_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_1_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_1_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_1_2.Rows.Add("Preliminary", "0");//12
                table_ROI_1_2.Rows.Add("Preliminary", "0");//13
                table_ROI_1_2.Rows.Add("Preliminary", "0");//14
                table_ROI_1_2.Rows.Add("Preliminary", "0");//15
                table_ROI_1_2.Rows.Add("Preliminary", "0");//16
                table_ROI_1_2.Rows.Add("Preliminary", "0");//17
                table_ROI_1_2.Rows.Add("Preliminary", "0");//18
                table_ROI_1_2.Rows.Add("Preliminary", "0");//19
                table_ROI_1_2.Rows.Add("Preliminary", "0");//20
                table_ROI_1_2.Rows.Add("Preliminary", "0");//21
                table_ROI_1_2.Rows.Add("Preliminary", "0");//22
                table_ROI_1_2.Rows.Add("Preliminary", "0");//23
                table_ROI_1_2.Rows.Add("Preliminary", "0");//24
                table_ROI_1_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_1.Tables.Add(table_ROI_1_2);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_ROI_1_1.Columns.Add("Item");
                table_ROI_1_1.Columns.Add("Value");
                table_ROI_1_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_1_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_1_1.Rows.Add("Ratio", "1_1");

                ds_DATA_1.Tables.Add(table_ROI_1_1);

                DataTable table_ROI_1_2 = new DataTable("ROI 1_2");
                table_ROI_1_2.Columns.Add("Item");
                table_ROI_1_2.Columns.Add("Value");
                table_ROI_1_2.Rows.Add("Use", "X"); //0
                table_ROI_1_2.Rows.Add("LT(X)", "100");//1
                table_ROI_1_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_1_2.Rows.Add("Width(W)", "100");//3
                table_ROI_1_2.Rows.Add("Height(H)", "100");//4

                table_ROI_1_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_1_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_1_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_1_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_1_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_1_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_1_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_1_2.Rows.Add("Preliminary", "0");//12
                table_ROI_1_2.Rows.Add("Preliminary", "0");//13
                table_ROI_1_2.Rows.Add("Preliminary", "0");//14
                table_ROI_1_2.Rows.Add("Preliminary", "0");//15
                table_ROI_1_2.Rows.Add("Preliminary", "0");//16
                table_ROI_1_2.Rows.Add("Preliminary", "0");//17
                table_ROI_1_2.Rows.Add("Preliminary", "0");//18
                table_ROI_1_2.Rows.Add("Preliminary", "0");//19
                table_ROI_1_2.Rows.Add("Preliminary", "0");//20
                table_ROI_1_2.Rows.Add("Preliminary", "0");//21
                table_ROI_1_2.Rows.Add("Preliminary", "0");//22
                table_ROI_1_2.Rows.Add("Preliminary", "0");//23
                table_ROI_1_2.Rows.Add("Preliminary", "0");//24
                table_ROI_1_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_1.Tables.Add(table_ROI_1_2);
            }
            // 카메라 1 설정값 Talbe 초기화 끝
            //==========================================================================================================//
        }

        void Initialize_Setting_2()
        {
            if (ds_DATA_2 != null)
            {
                ds_DATA_2 = null;
            }
            ds_DATA_2 = new DataSet();
            //==========================================================================================================//
            // 카메라 0 설정 Talbe 초기화
            DataTable table_setting_0 = new DataTable("Setting 2");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_0.Columns.Add(new DataColumn("사용", typeof(bool)));
                table_setting_0.Columns.Add("알고리즘");
                table_setting_0.Columns.Add("측정 아이템");

                DataColumn cmbComboBox2 = new DataColumn("판정 우선순위");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C2:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 측정 영역 설정", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C2:" + i.ToString("00"), "ROI#" + i.ToString("00") + " Measurement Area Setting", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C2:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 测量区域设置", "CLASS 1");
                }
            }

            ds_DATA_2.Tables.Add(table_setting_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_2.DataSource = ds_DATA_2.Tables[0];

            DataGridViewComboBoxColumn comboboxColumn2 = new DataGridViewComboBoxColumn();
            comboboxColumn2.Items.Add("CLASS 1");
            comboboxColumn2.Items.Add("CLASS 2");
            comboboxColumn2.Items.Add("CLASS 3");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn2.DataPropertyName = "판정 우선순위";
                comboboxColumn2.Name = "판정 우선순위";
                LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns.Remove("판정 우선순위");
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns.Remove("Judge Priority");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns.Remove("Judge Priority");
            }
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns.Insert(3, comboboxColumn2);


            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;


            LVApp.Instance().m_mainform.dataGridView_Setting_2.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[0].Width = 15;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[1].Width = 70;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[3].Width = 110;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[1].Visible = false;
            // 카메라 0 설정 Talbe 초기화 끝
            //==========================================================================================================//

            //==========================================================================================================//
            // 카메라 0 설정값 Talbe 초기화
            DataTable table_setting_Value_0 = new DataTable("Setting Value 2");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_Value_0.Columns.Add("알고리즘");

                DataColumn cmbComboBox3 = new DataColumn("측정방법");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("오프셋");
                table_setting_Value_0.Columns.Add("측정값");

                DataColumn cmbComboBox1 = new DataColumn("판정방법");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("기준값");
                table_setting_Value_0.Columns.Add("최솟값");
                table_setting_Value_0.Columns.Add("최댓값");
                table_setting_Value_0.Columns.Add("양품평균");
                table_setting_Value_0.Columns.Add("불량평균");
                table_setting_Value_0.Columns.Add("양품수");
                table_setting_Value_0.Columns.Add("불량수");
                table_setting_Value_0.Columns.Add("수율(%)");
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }

            for (int i = 1; i <= 40; i++)
            {
                table_setting_Value_0.Rows.Add("C2:" + i.ToString("00"), "From ALG.", 0, 0, "Range", 0, 0, 0, 0, 0, 0, 0, 0);
            }

            ds_DATA_2.Tables.Add(table_setting_Value_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DataSource = ds_DATA_2.Tables[1];

            DataGridViewComboBoxColumn comboboxColumn1 = new DataGridViewComboBoxColumn();
            comboboxColumn1.Items.Add("MIN");
            comboboxColumn1.Items.Add("AVERAGE");
            comboboxColumn1.Items.Add("MAX");
            comboboxColumn1.Items.Add("RANGE(MAX-MIN)");
            comboboxColumn1.Items.Add("From ALG.");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn1.DataPropertyName = "측정방법";
                comboboxColumn1.Name = "측정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Remove("측정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "판정방법";
                comboboxColumn.Name = "판정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Remove("판정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns.Insert(4, comboboxColumn);
            }

            for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ColumnCount; i++)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[0].Width = 50;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[1].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[9].Visible = false;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[1].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[3].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[8].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[9].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[10].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[11].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[12].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[3].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[8].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[9].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[12].DefaultCellStyle.BackColor = Color.Gainsboro;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[2].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[5].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[6].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[7].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[10].DefaultCellStyle.BackColor = Color.LightBlue;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[11].DefaultCellStyle.BackColor = Color.LavenderBlush;

            DataTable table_ROI_2_1 = new DataTable("ROI 2_1");
            if (m_SetLanguage == 0)
            {//한국어
                table_ROI_2_1.Columns.Add("구분");
                table_ROI_2_1.Columns.Add("값");
                table_ROI_2_1.Rows.Add("ROI00", "O_100_100_100_100_v1 이하_50_200_중심 기준_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_2_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_검사 영역 결과 사용_0_255_가로 길이_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_2_1.Rows.Add("Ratio", "1_1");

                ds_DATA_2.Tables.Add(table_ROI_2_1);

                DataTable table_ROI_2_2 = new DataTable("ROI 2_2");
                table_ROI_2_2.Columns.Add("구분");
                table_ROI_2_2.Columns.Add("값");
                table_ROI_2_2.Rows.Add("사용", "X"); //0
                table_ROI_2_2.Rows.Add("좌상(X)", "100");//1
                table_ROI_2_2.Rows.Add("좌상(Y)", "100");//2
                table_ROI_2_2.Rows.Add("가로(W)", "100");//3
                table_ROI_2_2.Rows.Add("높이(H)", "100");//4

                table_ROI_2_2.Rows.Add("임계화 방법", "v1 이하");//5
                table_ROI_2_2.Rows.Add("하위 임계값(v1,Gray)", "50");//6
                table_ROI_2_2.Rows.Add("상위 임계값(v2,Gray)", "200");//7
                table_ROI_2_2.Rows.Add("측정 방법", "가로 길이");//8
                table_ROI_2_2.Rows.Add("측정값 출력 방법", "평균값");//9
                table_ROI_2_2.Rows.Add("하위 측정범위(p1,%) ", "20");//10
                table_ROI_2_2.Rows.Add("상위 측정범위(p2,%)", "80");//11
                table_ROI_2_2.Rows.Add("예비변수", "0");//12
                table_ROI_2_2.Rows.Add("예비변수", "0");//13
                table_ROI_2_2.Rows.Add("예비변수", "0");//14
                table_ROI_2_2.Rows.Add("예비변수", "0");//15
                table_ROI_2_2.Rows.Add("예비변수", "0");//16
                table_ROI_2_2.Rows.Add("예비변수", "0");//17
                table_ROI_2_2.Rows.Add("예비변수", "0");//18
                table_ROI_2_2.Rows.Add("예비변수", "0");//19
                table_ROI_2_2.Rows.Add("예비변수", "0");//20
                table_ROI_2_2.Rows.Add("예비변수", "0");//21
                table_ROI_2_2.Rows.Add("예비변수", "0");//22
                table_ROI_2_2.Rows.Add("예비변수", "0");//23
                table_ROI_2_2.Rows.Add("예비변수", "0");//24
                table_ROI_2_2.Rows.Add("예비변수", "0");//25

                ds_DATA_2.Tables.Add(table_ROI_2_2);
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_ROI_2_1.Columns.Add("Item");
                table_ROI_2_1.Columns.Add("Value");
                table_ROI_2_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_2_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_2_1.Rows.Add("Ratio", "1_1");
                ds_DATA_2.Tables.Add(table_ROI_2_1);

                DataTable table_ROI_2_2 = new DataTable("ROI 2_2");
                table_ROI_2_2.Columns.Add("Item");
                table_ROI_2_2.Columns.Add("Value");
                table_ROI_2_2.Rows.Add("Use", "X"); //0
                table_ROI_2_2.Rows.Add("LT(X)", "100");//1
                table_ROI_2_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_2_2.Rows.Add("Width(W)", "100");//3
                table_ROI_2_2.Rows.Add("Height(H)", "100");//4

                table_ROI_2_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_2_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_2_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_2_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_2_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_2_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_2_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_2_2.Rows.Add("Preliminary", "0");//12
                table_ROI_2_2.Rows.Add("Preliminary", "0");//13
                table_ROI_2_2.Rows.Add("Preliminary", "0");//14
                table_ROI_2_2.Rows.Add("Preliminary", "0");//15
                table_ROI_2_2.Rows.Add("Preliminary", "0");//16
                table_ROI_2_2.Rows.Add("Preliminary", "0");//17
                table_ROI_2_2.Rows.Add("Preliminary", "0");//18
                table_ROI_2_2.Rows.Add("Preliminary", "0");//19
                table_ROI_2_2.Rows.Add("Preliminary", "0");//20
                table_ROI_2_2.Rows.Add("Preliminary", "0");//21
                table_ROI_2_2.Rows.Add("Preliminary", "0");//22
                table_ROI_2_2.Rows.Add("Preliminary", "0");//23
                table_ROI_2_2.Rows.Add("Preliminary", "0");//24
                table_ROI_2_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_2.Tables.Add(table_ROI_2_2);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_ROI_2_1.Columns.Add("Item");
                table_ROI_2_1.Columns.Add("Value");
                table_ROI_2_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_2_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_2_1.Rows.Add("Ratio", "1_1");
                ds_DATA_2.Tables.Add(table_ROI_2_1);

                DataTable table_ROI_2_2 = new DataTable("ROI 2_2");
                table_ROI_2_2.Columns.Add("Item");
                table_ROI_2_2.Columns.Add("Value");
                table_ROI_2_2.Rows.Add("Use", "X"); //0
                table_ROI_2_2.Rows.Add("LT(X)", "100");//1
                table_ROI_2_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_2_2.Rows.Add("Width(W)", "100");//3
                table_ROI_2_2.Rows.Add("Height(H)", "100");//4

                table_ROI_2_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_2_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_2_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_2_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_2_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_2_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_2_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_2_2.Rows.Add("Preliminary", "0");//12
                table_ROI_2_2.Rows.Add("Preliminary", "0");//13
                table_ROI_2_2.Rows.Add("Preliminary", "0");//14
                table_ROI_2_2.Rows.Add("Preliminary", "0");//15
                table_ROI_2_2.Rows.Add("Preliminary", "0");//16
                table_ROI_2_2.Rows.Add("Preliminary", "0");//17
                table_ROI_2_2.Rows.Add("Preliminary", "0");//18
                table_ROI_2_2.Rows.Add("Preliminary", "0");//19
                table_ROI_2_2.Rows.Add("Preliminary", "0");//20
                table_ROI_2_2.Rows.Add("Preliminary", "0");//21
                table_ROI_2_2.Rows.Add("Preliminary", "0");//22
                table_ROI_2_2.Rows.Add("Preliminary", "0");//23
                table_ROI_2_2.Rows.Add("Preliminary", "0");//24
                table_ROI_2_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_2.Tables.Add(table_ROI_2_2);
            }
            // 카메라 2 설정값 Talbe 초기화 끝
            //==========================================================================================================//
        }

        void Initialize_Setting_3()
        {
            if (ds_DATA_3 != null)
            {
                ds_DATA_3 = null;
            }
            ds_DATA_3 = new DataSet();
            //==========================================================================================================//
            // 카메라 0 설정 Talbe 초기화
            DataTable table_setting_0 = new DataTable("Setting 3");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_0.Columns.Add(new DataColumn("사용", typeof(bool)));
                table_setting_0.Columns.Add("알고리즘");
                table_setting_0.Columns.Add("측정 아이템");

                DataColumn cmbComboBox2 = new DataColumn("판정 우선순위");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C3:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 측정 영역 설정", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C3:" + i.ToString("00"), "ROI#" + i.ToString("00") + " Measurement Area Setting", "CLASS 1");
                }
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_0.Columns.Add(new DataColumn("Use", typeof(bool)));
                table_setting_0.Columns.Add("Algorithm");
                table_setting_0.Columns.Add("Measurement Item");

                DataColumn cmbComboBox2 = new DataColumn("Judge Priority");
                cmbComboBox2.DataType = System.Type.GetType("System.String");
                table_setting_0.Columns.Add(cmbComboBox2);

                for (int i = 1; i <= 40; i++)
                {
                    table_setting_0.Rows.Add(false, "C3:" + i.ToString("00"), "ROI#" + i.ToString("00") + " 测量区域设置", "CLASS 1");
                }
            }

            ds_DATA_3.Tables.Add(table_setting_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_3.DataSource = ds_DATA_3.Tables[0];

            DataGridViewComboBoxColumn comboboxColumn2 = new DataGridViewComboBoxColumn();
            comboboxColumn2.Items.Add("CLASS 1");
            comboboxColumn2.Items.Add("CLASS 2");
            comboboxColumn2.Items.Add("CLASS 3");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn2.DataPropertyName = "판정 우선순위";
                comboboxColumn2.Name = "판정 우선순위";
                LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns.Remove("판정 우선순위");
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns.Remove("Judge Priority");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn2.DataPropertyName = "Judge Priority";
                comboboxColumn2.Name = "Judge Priority";
                LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns.Remove("Judge Priority");
            }
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns.Insert(3, comboboxColumn2);


            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;


            LVApp.Instance().m_mainform.dataGridView_Setting_3.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[0].Width = 15;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[1].Width = 70;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[3].Width = 110;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[1].Visible = false;
            // 카메라 0 설정 Talbe 초기화 끝
            //==========================================================================================================//

            //==========================================================================================================//
            // 카메라 0 설정값 Talbe 초기화
            DataTable table_setting_Value_0 = new DataTable("Setting Value 2");
            if (m_SetLanguage == 0)
            {//한국어
                table_setting_Value_0.Columns.Add("알고리즘");

                DataColumn cmbComboBox3 = new DataColumn("측정방법");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("오프셋");
                table_setting_Value_0.Columns.Add("측정값");

                DataColumn cmbComboBox1 = new DataColumn("판정방법");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("기준값");
                table_setting_Value_0.Columns.Add("최솟값");
                table_setting_Value_0.Columns.Add("최댓값");
                table_setting_Value_0.Columns.Add("양품평균");
                table_setting_Value_0.Columns.Add("불량평균");
                table_setting_Value_0.Columns.Add("양품수");
                table_setting_Value_0.Columns.Add("불량수");
                table_setting_Value_0.Columns.Add("수율(%)");
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_setting_Value_0.Columns.Add("Algorithm");

                DataColumn cmbComboBox3 = new DataColumn("M.Method");
                cmbComboBox3.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox3);

                table_setting_Value_0.Columns.Add("Offset");
                table_setting_Value_0.Columns.Add("Result");

                DataColumn cmbComboBox1 = new DataColumn("J.Method");
                cmbComboBox1.DataType = System.Type.GetType("System.String");
                table_setting_Value_0.Columns.Add(cmbComboBox1);

                table_setting_Value_0.Columns.Add("Ref.");
                table_setting_Value_0.Columns.Add("Min.");
                table_setting_Value_0.Columns.Add("Max.");
                table_setting_Value_0.Columns.Add("OK Avg.");
                table_setting_Value_0.Columns.Add("NG Avg.");
                table_setting_Value_0.Columns.Add("OK Cnt");
                table_setting_Value_0.Columns.Add("NG Cnt");
                table_setting_Value_0.Columns.Add("Yield(%)");
            }

            for (int i = 1; i <= 40; i++)
            {
                table_setting_Value_0.Rows.Add("C3:" + i.ToString("00"), "From ALG.", 0, 0, "Range", 0, 0, 0, 0, 0, 0, 0, 0);
            }

            ds_DATA_3.Tables.Add(table_setting_Value_0);

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DataSource = ds_DATA_3.Tables[1];

            DataGridViewComboBoxColumn comboboxColumn1 = new DataGridViewComboBoxColumn();
            comboboxColumn1.Items.Add("MIN");
            comboboxColumn1.Items.Add("AVERAGE");
            comboboxColumn1.Items.Add("MAX");
            comboboxColumn1.Items.Add("RANGE(MAX-MIN)");
            comboboxColumn1.Items.Add("From ALG.");
            if (m_SetLanguage == 0)
            {//한국어
                comboboxColumn1.DataPropertyName = "측정방법";
                comboboxColumn1.Name = "측정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Remove("측정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "판정방법";
                comboboxColumn.Name = "판정방법";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Remove("판정방법");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 1)
            {//영어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Insert(4, comboboxColumn);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                comboboxColumn1.DataPropertyName = "M.Method";
                comboboxColumn1.Name = "M.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Remove("M.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Insert(1, comboboxColumn1);

                DataGridViewComboBoxColumn comboboxColumn = new DataGridViewComboBoxColumn();
                comboboxColumn.Items.Add("Min.");
                comboboxColumn.Items.Add("Max.");
                comboboxColumn.Items.Add("Range");
                comboboxColumn.Items.Add("Rev-Range");
                comboboxColumn.DataPropertyName = "J.Method";
                comboboxColumn.Name = "J.Method";
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Remove("J.Method");
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns.Insert(4, comboboxColumn);
            }

            for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ColumnCount; i++)
            {
                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.RowHeadersWidth = 24;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ColumnHeadersHeight = 26;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ScrollBars = ScrollBars.Both;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[0].Width = 50;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[0].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[1].Visible = false;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[9].Visible = false;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[1].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[3].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[8].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[9].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[10].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[11].ReadOnly = true;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[12].ReadOnly = true;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[3].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[8].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[9].DefaultCellStyle.BackColor = Color.Gainsboro;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[12].DefaultCellStyle.BackColor = Color.Gainsboro;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[2].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[5].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[6].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[7].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;

            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[10].DefaultCellStyle.BackColor = Color.LightBlue;
            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[11].DefaultCellStyle.BackColor = Color.LavenderBlush;

            DataTable table_ROI_3_1 = new DataTable("ROI 3_1");
            if (m_SetLanguage == 0)
            {//한국어
                table_ROI_3_1.Columns.Add("구분");
                table_ROI_3_1.Columns.Add("값");
                table_ROI_3_1.Rows.Add("ROI00", "O_100_100_100_100_v1 이하_50_200_중심 기준_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_3_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_검사 영역 결과 사용_0_255_가로 길이_평균값_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_3_1.Rows.Add("Ratio", "1_1");

                ds_DATA_3.Tables.Add(table_ROI_3_1);

                DataTable table_ROI_3_2 = new DataTable("ROI 3_2");
                table_ROI_3_2.Columns.Add("구분");
                table_ROI_3_2.Columns.Add("값");
                table_ROI_3_2.Rows.Add("사용", "X"); //0
                table_ROI_3_2.Rows.Add("좌상(X)", "100");//1
                table_ROI_3_2.Rows.Add("좌상(Y)", "100");//2
                table_ROI_3_2.Rows.Add("가로(W)", "100");//3
                table_ROI_3_2.Rows.Add("높이(H)", "100");//4

                table_ROI_3_2.Rows.Add("임계화 방법", "v1 이하");//5
                table_ROI_3_2.Rows.Add("하위 임계값(v1,Gray)", "50");//6
                table_ROI_3_2.Rows.Add("상위 임계값(v2,Gray)", "200");//7
                table_ROI_3_2.Rows.Add("측정 방법", "가로 길이");//8
                table_ROI_3_2.Rows.Add("측정값 출력 방법", "평균값");//9
                table_ROI_3_2.Rows.Add("하위 측정범위(p1,%) ", "20");//10
                table_ROI_3_2.Rows.Add("상위 측정범위(p2,%)", "80");//11
                table_ROI_3_2.Rows.Add("예비변수", "0");//12
                table_ROI_3_2.Rows.Add("예비변수", "0");//13
                table_ROI_3_2.Rows.Add("예비변수", "0");//14
                table_ROI_3_2.Rows.Add("예비변수", "0");//15
                table_ROI_3_2.Rows.Add("예비변수", "0");//16
                table_ROI_3_2.Rows.Add("예비변수", "0");//17
                table_ROI_3_2.Rows.Add("예비변수", "0");//18
                table_ROI_3_2.Rows.Add("예비변수", "0");//19
                table_ROI_3_2.Rows.Add("예비변수", "0");//20
                table_ROI_3_2.Rows.Add("예비변수", "0");//21
                table_ROI_3_2.Rows.Add("예비변수", "0");//22
                table_ROI_3_2.Rows.Add("예비변수", "0");//23
                table_ROI_3_2.Rows.Add("예비변수", "0");//24
                table_ROI_3_2.Rows.Add("예비변수", "0");//25

                ds_DATA_3.Tables.Add(table_ROI_3_2);
            }
            else if (m_SetLanguage == 1)
            {//영어
                table_ROI_3_1.Columns.Add("Item");
                table_ROI_3_1.Columns.Add("Value");

                table_ROI_3_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_3_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_3_1.Rows.Add("Ratio", "1_1");

                ds_DATA_3.Tables.Add(table_ROI_3_1);

                DataTable table_ROI_3_2 = new DataTable("ROI 3_2");
                table_ROI_3_2.Columns.Add("Item");
                table_ROI_3_2.Columns.Add("Value");
                table_ROI_3_2.Rows.Add("Use", "X"); //0
                table_ROI_3_2.Rows.Add("LT(X)", "100");//1
                table_ROI_3_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_3_2.Rows.Add("Width(W)", "100");//3
                table_ROI_3_2.Rows.Add("Height(H)", "100");//4

                table_ROI_3_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_3_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_3_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_3_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_3_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_3_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_3_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_3_2.Rows.Add("Preliminary", "0");//12
                table_ROI_3_2.Rows.Add("Preliminary", "0");//13
                table_ROI_3_2.Rows.Add("Preliminary", "0");//14
                table_ROI_3_2.Rows.Add("Preliminary", "0");//15
                table_ROI_3_2.Rows.Add("Preliminary", "0");//16
                table_ROI_3_2.Rows.Add("Preliminary", "0");//17
                table_ROI_3_2.Rows.Add("Preliminary", "0");//18
                table_ROI_3_2.Rows.Add("Preliminary", "0");//19
                table_ROI_3_2.Rows.Add("Preliminary", "0");//20
                table_ROI_3_2.Rows.Add("Preliminary", "0");//21
                table_ROI_3_2.Rows.Add("Preliminary", "0");//22
                table_ROI_3_2.Rows.Add("Preliminary", "0");//23
                table_ROI_3_2.Rows.Add("Preliminary", "0");//24
                table_ROI_3_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_3.Tables.Add(table_ROI_3_2);
            }
            else if (m_SetLanguage == 2)
            {//중국어
                table_ROI_3_1.Columns.Add("Item");
                table_ROI_3_1.Columns.Add("Value");

                table_ROI_3_1.Rows.Add("ROI00", "O_100_100_100_100_v1 less than_50_200_Center_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                for (int i = 1; i <= 40; i++)
                {
                    table_ROI_3_1.Rows.Add("ROI" + i.ToString("00"), "X_100_100_100_100_Insp. area result use_0_255_Hor. length_AVG_0_100_0_0_0_0_0_0_0_0_0_0_0_0_0_0");
                }
                table_ROI_3_1.Rows.Add("Ratio", "1_1");

                ds_DATA_3.Tables.Add(table_ROI_3_1);

                DataTable table_ROI_3_2 = new DataTable("ROI 3_2");
                table_ROI_3_2.Columns.Add("Item");
                table_ROI_3_2.Columns.Add("Value");
                table_ROI_3_2.Rows.Add("Use", "X"); //0
                table_ROI_3_2.Rows.Add("LT(X)", "100");//1
                table_ROI_3_2.Rows.Add("LT(Y)", "100");//2
                table_ROI_3_2.Rows.Add("Width(W)", "100");//3
                table_ROI_3_2.Rows.Add("Height(H)", "100");//4

                table_ROI_3_2.Rows.Add("Threshold", "v1 less than");//5
                table_ROI_3_2.Rows.Add("Low Value(v1,Gray)", "50");//6
                table_ROI_3_2.Rows.Add("High Value(v2,Gray)", "200");//7
                table_ROI_3_2.Rows.Add("M. Method", "Hor. length");//8
                table_ROI_3_2.Rows.Add("Cal. Method", "AVG");//9
                table_ROI_3_2.Rows.Add("M. Low bound(p1,%) ", "0");//10
                table_ROI_3_2.Rows.Add("M. High bound(p2,%)", "100");//11
                table_ROI_3_2.Rows.Add("Preliminary", "0");//12
                table_ROI_3_2.Rows.Add("Preliminary", "0");//13
                table_ROI_3_2.Rows.Add("Preliminary", "0");//14
                table_ROI_3_2.Rows.Add("Preliminary", "0");//15
                table_ROI_3_2.Rows.Add("Preliminary", "0");//16
                table_ROI_3_2.Rows.Add("Preliminary", "0");//17
                table_ROI_3_2.Rows.Add("Preliminary", "0");//18
                table_ROI_3_2.Rows.Add("Preliminary", "0");//19
                table_ROI_3_2.Rows.Add("Preliminary", "0");//20
                table_ROI_3_2.Rows.Add("Preliminary", "0");//21
                table_ROI_3_2.Rows.Add("Preliminary", "0");//22
                table_ROI_3_2.Rows.Add("Preliminary", "0");//23
                table_ROI_3_2.Rows.Add("Preliminary", "0");//24
                table_ROI_3_2.Rows.Add("Preliminary", "0");//25

                ds_DATA_3.Tables.Add(table_ROI_3_2);
            }
            // 카메라 3 설정값 Talbe 초기화 끝
            //==========================================================================================================//
        }

        //public int t_try_cnt = 0;
        public void CSV_Logfile_Initialize(int Cam_num)
        {
            try
            {
                if (Cam_num < 4)
                {
                    if (!m_Data_Log_Use_Check || LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[Cam_num])
                    {
                        return;
                    }

                    String m_Log_folder = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                    {
                        m_Log_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    }

                    DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        return;
                    }
                    if (CSVLog[Cam_num] == null)
                    {
                        String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                        if (m_Log_Save_Folder != "")
                        {
                            m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                        }
                        bool t_HeadersWritten = false;
                        if (File.Exists(m_Log_File_Name))
                        {
                            t_HeadersWritten = true;
                        }
                        CSVLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                        CSVLog[Cam_num].HeadersWritten = t_HeadersWritten;

                        if (ds_LOG.Tables[Cam_num].Columns.Count > 0)
                        {
                            string[] header = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                            header[0] = "셀ID";
                            for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                            {
                                header[j + 1] = ds_LOG.Tables[Cam_num].Columns[j].ColumnName;
                            }
                            CSVLog[Cam_num].WriteHeader(header);
                        }
                    }
                    else
                    {
                        if (!CSVLog[Cam_num].IsOpen)
                        {
                            CSVLog[Cam_num].Close();
                            String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                            if (m_Log_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            if (ds_LOG.Tables[Cam_num].Columns.Count > 0)
                            {
                                string[] header = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                header[0] = "셀ID";
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    header[j + 1] = ds_LOG.Tables[Cam_num].Columns[j].ColumnName;
                                }
                                CSVLog[Cam_num].WriteHeader(header);
                            }
                        }
                        else
                        {
                            CSVLog[Cam_num].Close();
                            CSVLog[Cam_num] = null;
                            String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                            if (m_Log_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            if (ds_LOG.Tables[Cam_num].Columns.Count > 0)
                            {
                                string[] header = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                header[0] = "셀ID";
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    header[j + 1] = ds_LOG.Tables[Cam_num].Columns[j].ColumnName;
                                }
                                CSVLog[Cam_num].WriteHeader(header);
                            }
                        }
                    }
                }
                else if (Cam_num == 4)
                {
                    if (!m_Data_Log_Use_Check)
                    {
                        return;
                    }

                    t_bool_log_Total[0] = true;
                    t_bool_log_Total[1] = true;
                    t_bool_log_Total[2] = true;
                    t_bool_log_Total[3] = true;

                    t_Result_log_Total[0] = true;
                    t_Result_log_Total[1] = true;
                    t_Result_log_Total[2] = true;
                    t_Result_log_Total[3] = true;

                    String m_Log_folder = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                    {
                        m_Log_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    }

                    DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        return;
                    }
                    if (CSVLog[Cam_num] == null)
                    {
                        String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                        if (m_Log_Save_Folder != "")
                        {
                            m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                        }
                        bool t_HeadersWritten = false;
                        if (File.Exists(m_Log_File_Name))
                        {
                            t_HeadersWritten = true;
                        }
                        CSVLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                        CSVLog[Cam_num].HeadersWritten = t_HeadersWritten;
                        int t_cnt = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                t_int_log_Total[i] = t_cnt + 2;
                                t_cnt += ds_LOG.Tables[i].Columns.Count-1;
                                if (i > 0)
                                {
                                    t_cnt -= 1;
                                }
                            }
                        }

                        if (t_cnt > 0)
                        {
                            string[] header = new string[t_cnt + 2];
                            t_str_log_Total = new string[t_cnt + 2];
                            header[0] = "Result";
                            header[1] = "셀ID";
                            t_cnt = 2;
                            for (int i = 0; i < 4; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                                    {
                                        if (j != 1)
                                        {
                                            if (i == 0)
                                            {
                                                header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                t_cnt++;
                                            }
                                            else if (i > 0 && j > 1)
                                            {
                                                header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                t_cnt++;
                                            }
                                        }
                                    }
                                }
                            }
                            CSVLog[Cam_num].WriteHeader(header);
                        }
                    }
                    else
                    {
                        if (!CSVLog[Cam_num].IsOpen)
                        {
                            CSVLog[Cam_num].Close();
                            String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                            if (m_Log_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            int t_cnt = 0;

                            for (int i = 0; i < 4; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    t_int_log_Total[i] = t_cnt + 2;
                                    t_cnt += ds_LOG.Tables[i].Columns.Count-1;
                                    if (i > 0)
                                    {
                                        t_cnt -= 1;
                                    }
                                }
                            }

                            if (t_cnt > 0)
                            {
                                string[] header = new string[t_cnt + 2];
                                t_str_log_Total = new string[t_cnt + 2];
                                header[0] = "Result";
                                header[1] = "셀ID";
                                t_cnt = 2;
                                for (int i = 0; i < 4; i++)
                                {
                                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                    {
                                        for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                                        {
                                            if (j != 1)
                                            {
                                                if (i == 0)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                                else if (i > 0 && j > 1)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                            }
                                        }
                                    }
                                }
                                CSVLog[Cam_num].WriteHeader(header);
                            }
                        }
                        else
                        {
                            CSVLog[Cam_num].Close();
                            CSVLog[Cam_num] = null;
                            String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                            if (m_Log_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            int t_cnt = 0;

                            for (int i = 0; i < 4; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    t_int_log_Total[i] = t_cnt + 2;
                                    t_cnt += ds_LOG.Tables[i].Columns.Count-1;
                                    if (i > 0)
                                    {
                                        t_cnt -= 1;
                                    }
                                }
                            }

                            if (t_cnt > 0)
                            {
                                string[] header = new string[t_cnt + 2];
                                t_str_log_Total = new string[t_cnt + 2];
                                header[0] = "Result";
                                header[1] = "셀ID";
                                t_cnt = 2;
                                for (int i = 0; i < 4; i++)
                                {
                                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                    {
                                        for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                                        {
                                            if (j != 1)
                                            {
                                                if (i == 0)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                                else if (i > 0 && j > 1)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                            }
                                        }
                                    }
                                }
                                CSVLog[Cam_num].WriteHeader(header);
                            }
                        }
                    }
                    Spec_Log_Save();
                }
                CSV_DataLogfile_Initialize(Cam_num);
            }
            catch
            {
                //if (t_try_cnt < 2)
                //{
                //    Thread.Sleep(50);
                //    CSV_Logfile_Initialize(Cam_num);
                //    t_try_cnt++;
                //}
            }
        }


        public void CSV_DataLogfile_Initialize(int Cam_num)
        {
            try
            {
                if (Cam_num < 4)
                {
                    if (!m_Data_Log_Use_Check || LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[Cam_num])
                    {
                        return;
                    }

                    String m_Log_folder = "";
                    if (LVApp.Instance().m_Config.m_Data_Save_Folder.Length > 1)
                    {
                        m_Log_folder = LVApp.Instance().m_Config.m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    }
                    else
                    {
                        return;
                    }

                    DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        return;
                    }
                    if (CSVDataLog[Cam_num] == null)
                    {
                        String m_Log_File_Name = "";
                        if (m_Data_Save_Folder != "")
                        {
                            m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                        }
                        bool t_HeadersWritten = false;
                        if (File.Exists(m_Log_File_Name))
                        {
                            t_HeadersWritten = true;
                        }
                        CSVDataLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                        CSVDataLog[Cam_num].HeadersWritten = t_HeadersWritten;

                        if (ds_LOG.Tables[Cam_num].Columns.Count > 0)
                        {
                            string[] header = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                            header[0] = "셀ID";
                            for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                            {
                                header[j + 1] = ds_LOG.Tables[Cam_num].Columns[j].ColumnName;
                            }
                            CSVDataLog[Cam_num].WriteHeader(header);
                        }
                    }
                    else
                    {
                        if (!CSVDataLog[Cam_num].IsOpen)
                        {
                            CSVDataLog[Cam_num].Close();
                            String m_Log_File_Name = "";
                            if (m_Data_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVDataLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVDataLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            if (ds_LOG.Tables[Cam_num].Columns.Count > 0)
                            {
                                string[] header = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                header[0] = "셀ID";
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    header[j + 1] = ds_LOG.Tables[Cam_num].Columns[j].ColumnName;
                                }
                                CSVDataLog[Cam_num].WriteHeader(header);
                            }
                        }
                        else
                        {
                            CSVDataLog[Cam_num].Close();
                            CSVDataLog[Cam_num] = null;
                            String m_Log_File_Name = "";
                            if (m_Data_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + Cam_num.ToString() + ".csv"; //파일경로
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVDataLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVDataLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            if (ds_LOG.Tables[Cam_num].Columns.Count > 0)
                            {
                                string[] header = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                header[0] = "셀ID";
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    header[j + 1] = ds_LOG.Tables[Cam_num].Columns[j].ColumnName;
                                }
                                CSVDataLog[Cam_num].WriteHeader(header);
                            }
                        }
                    }
                }
                else if (Cam_num == 4)
                {
                    if (!m_Data_Log_Use_Check)
                    {
                        return;
                    }

                    String m_Log_folder = "";
                    if (LVApp.Instance().m_Config.m_Data_Save_Folder.Length > 1)
                    {
                        m_Log_folder = LVApp.Instance().m_Config.m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    }
                    else
                    {
                        return;
                    }

                    DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        return;
                    }
                    if (CSVDataLog[Cam_num] == null)
                    {
                        String m_Log_File_Name = "";
                        if (m_Data_Save_Folder != "")
                        {
                            m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                        }
                        bool t_HeadersWritten = false;
                        if (File.Exists(m_Log_File_Name))
                        {
                            t_HeadersWritten = true;
                        }
                        CSVDataLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                        CSVDataLog[Cam_num].HeadersWritten = t_HeadersWritten;
                        int t_cnt = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                t_int_log_Total[i] = t_cnt + 2;
                                t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                                if (i > 0)
                                {
                                    t_cnt -= 1;
                                }
                            }
                        }

                        if (t_cnt > 0)
                        {
                            string[] header = new string[t_cnt + 2];
                            t_str_log_Total = new string[t_cnt + 2];
                            header[0] = "Result";
                            header[1] = "셀ID";
                            t_cnt = 2;
                            for (int i = 0; i < 4; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                                    {
                                        if (j != 1)
                                        {
                                            if (i == 0)
                                            {
                                                header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                t_cnt++;
                                            }
                                            else if (i > 0 && j > 1)
                                            {
                                                header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                t_cnt++;
                                            }
                                        }
                                    }
                                }
                            }
                            CSVDataLog[Cam_num].WriteHeader(header);
                        }
                    }
                    else
                    {
                        if (!CSVDataLog[Cam_num].IsOpen)
                        {
                            CSVDataLog[Cam_num].Close();
                            String m_Log_File_Name = "";
                            if (m_Data_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                            }
                            else
                            {
                                return;
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVDataLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVDataLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            int t_cnt = 0;

                            for (int i = 0; i < 4; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    t_int_log_Total[i] = t_cnt + 2;
                                    t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                                    if (i > 0)
                                    {
                                        t_cnt -= 1;
                                    }
                                }
                            }

                            if (t_cnt > 0)
                            {
                                string[] header = new string[t_cnt + 2];
                                t_str_log_Total = new string[t_cnt + 2];
                                header[0] = "Result";
                                header[1] = "셀ID";
                                t_cnt = 2;
                                for (int i = 0; i < 4; i++)
                                {
                                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                    {
                                        for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                                        {
                                            if (j != 1)
                                            {
                                                if (i == 0)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                                else if (i > 0 && j > 1)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                            }
                                        }
                                    }
                                }
                                CSVDataLog[Cam_num].WriteHeader(header);
                            }
                        }
                        else
                        {
                            CSVDataLog[Cam_num].Close();
                            CSVDataLog[Cam_num] = null;
                            String m_Log_File_Name = "";
                            if (m_Data_Save_Folder != "")
                            {
                                m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Total.csv"; //파일경로
                            }
                            else
                            {
                                return;
                            }
                            bool t_HeadersWritten = false;
                            if (File.Exists(m_Log_File_Name))
                            {
                                t_HeadersWritten = true;
                            }
                            CSVDataLog[Cam_num] = new CSVWriter(m_Log_File_Name);
                            CSVDataLog[Cam_num].HeadersWritten = t_HeadersWritten;

                            int t_cnt = 0;

                            for (int i = 0; i < 4; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    t_int_log_Total[i] = t_cnt + 2;
                                    t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                                    if (i > 0)
                                    {
                                        t_cnt -= 1;
                                    }
                                }
                            }

                            if (t_cnt > 0)
                            {
                                string[] header = new string[t_cnt + 2];
                                t_str_log_Total = new string[t_cnt + 2];
                                header[0] = "Result";
                                header[1] = "셀ID";
                                t_cnt = 2;
                                for (int i = 0; i < 4; i++)
                                {
                                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                    {
                                        for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                                        {
                                            if (j != 1)
                                            {
                                                if (i == 0)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                                else if (i > 0 && j > 1)
                                                {
                                                    header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;
                                                    t_cnt++;
                                                }
                                            }
                                        }
                                    }
                                }
                                CSVDataLog[Cam_num].WriteHeader(header);
                            }
                        }
                    }
                    Spec_DataLog_Save();
                }
            }
            catch
            {
                //if (t_try_cnt < 2)
                //{
                //    Thread.Sleep(50);
                //    CSV_Logfile_Initialize(Cam_num);
                //    t_try_cnt++;
                //}
            }
        }


        public void CSV_Logfile_Terminate()
        {
            if (CSVLog[0] != null)
            {
                if (CSVLog[0].IsOpen)
                {
                    CSVLog[0].Close();
                }
            }
            if (CSVLog[1] != null)
            {
                if (CSVLog[1].IsOpen)
                {
                    CSVLog[1].Close();
                }
            }
            if (CSVLog[2] != null)
            {
                if (CSVLog[2].IsOpen)
                {
                    CSVLog[2].Close();
                }
            }
            if (CSVLog[3] != null)
            {
                if (CSVLog[3].IsOpen)
                {
                    CSVLog[3].Close();
                }
            }
            if (CSVLog[4] != null)
            {
                if (CSVLog[4].IsOpen)
                {
                    CSVLog[4].Close();
                }
            }
            Spec_Log_Save();
            CSV_DataLogfile_Terminate();
        }


        public void CSV_DataLogfile_Terminate()
        {
            if (CSVDataLog[0] != null)
            {
                if (CSVDataLog[0].IsOpen)
                {
                    CSVDataLog[0].Close();
                }
            }
            if (CSVDataLog[1] != null)
            {
                if (CSVDataLog[1].IsOpen)
                {
                    CSVDataLog[1].Close();
                }
            }
            if (CSVDataLog[2] != null)
            {
                if (CSVDataLog[2].IsOpen)
                {
                    CSVDataLog[2].Close();
                }
            }
            if (CSVDataLog[3] != null)
            {
                if (CSVDataLog[3].IsOpen)
                {
                    CSVDataLog[3].Close();
                }
            }
            if (CSVDataLog[4] != null)
            {
                if (CSVDataLog[4].IsOpen)
                {
                    CSVDataLog[4].Close();
                }
            }
            Spec_DataLog_Save();
        }


        public void Spec_Log_Save()
        {
            try
            {
                String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Spec.csv"; //파일경로
                if (m_Log_Save_Folder != "")
                {
                    m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Spec.csv"; //파일경로
                    DirectoryInfo root_dir = new DirectoryInfo(m_Log_Save_Folder.Substring(0, 3));
                    if (root_dir.Exists == false)
                    {
                        return;
                    }
                }

                if (File.Exists(m_Log_File_Name))
                {
                    File.Delete(m_Log_File_Name);
                }

                CSVWriter t_csv_spec = new CSVWriter(m_Log_File_Name);

                int t_cnt = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    {
                        t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                        if (i > 0)
                        {
                            t_cnt -= 1;
                        }
                    }
                }

                if (t_cnt > 0)
                {
                    string[] header = new string[t_cnt];
                    string[] spec_line = new string[t_cnt];
                    header[0] = "ITEM";
                    t_cnt = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                            {
                                if (j != 1)
                                {
                                    if (i == 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                    else if (i > 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                }
                            }
                        }
                    }
                    t_csv_spec.WriteHeader(header);

                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "상한규격";
                    }
                    else
                    {
                        spec_line[0] = "USL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);
                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "하한규격";
                    }
                    else
                    {
                        spec_line[0] = "LSL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);

                }
                t_csv_spec.Close();
            }
            catch
            { }
        }

        public void Spec_DataLog_Save()
        {
            try
            {
                String m_Log_File_Name = "";
                if (m_Data_Save_Folder != "")
                {
                    m_Log_File_Name = m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Spec.csv"; //파일경로
                }
                else
                {
                    return;
                }

                if (File.Exists(m_Log_File_Name))
                {
                    File.Delete(m_Log_File_Name);
                }

                CSVWriter t_csv_spec = new CSVWriter(m_Log_File_Name);

                int t_cnt = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    {
                        t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                        if (i > 0)
                        {
                            t_cnt -= 1;
                        }
                    }
                }

                if (t_cnt > 0)
                {
                    string[] header = new string[t_cnt];
                    string[] spec_line = new string[t_cnt];
                    header[0] = "ITEM";
                    t_cnt = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                            {
                                if (j != 1)
                                {
                                    if (i == 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                    else if (i > 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                }
                            }
                        }
                    }
                    t_csv_spec.WriteHeader(header);

                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "상한규격";
                    }
                    else
                    {
                        spec_line[0] = "USL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);
                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "하한규격";
                    }
                    else
                    {
                        spec_line[0] = "LSL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);

                }
                t_csv_spec.Close();
            }
            catch
            { }
        }

        public void Spec_Log_Save(String m_Log_File_Name)
        {
            try
            {
                if (File.Exists(m_Log_File_Name))
                {
                    File.Delete(m_Log_File_Name);
                }

                CSVWriter t_csv_spec = new CSVWriter(m_Log_File_Name);

                int t_cnt = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    {
                        t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                        if (i > 0)
                        {
                            t_cnt -= 1;
                        }
                    }
                }

                if (t_cnt > 0)
                {
                    string[] header = new string[t_cnt];
                    string[] spec_line = new string[t_cnt];
                    header[0] = "ITEM";
                    t_cnt = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                            {
                                if (j != 1)
                                {
                                    if (i == 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                    else if (i > 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                }
                            }
                        }
                    }
                    t_csv_spec.WriteHeader(header);

                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "상한규격";
                    }
                    else
                    {
                        spec_line[0] = "USL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);
                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "하한규격";
                    }
                    else
                    {
                        spec_line[0] = "LSL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);

                }

                t_csv_spec.Close();
            }
            catch
            { }
        }

        public void Spec_DataLog_Save(String m_Log_File_Name)
        {
            try
            {
                if (File.Exists(m_Log_File_Name))
                {
                    File.Delete(m_Log_File_Name);
                }

                CSVWriter t_csv_spec = new CSVWriter(m_Log_File_Name);

                int t_cnt = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    {
                        t_cnt += ds_LOG.Tables[i].Columns.Count - 1;
                        if (i > 0)
                        {
                            t_cnt -= 1;
                        }
                    }
                }

                if (t_cnt > 0)
                {
                    string[] header = new string[t_cnt];
                    string[] spec_line = new string[t_cnt];
                    header[0] = "ITEM";
                    t_cnt = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            for (int j = 0; j < ds_LOG.Tables[i].Columns.Count; j++)
                            {
                                if (j != 1)
                                {
                                    if (i == 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                    else if (i > 0 && j > 1)
                                    {
                                        header[t_cnt] = ds_LOG.Tables[i].Columns[j].ColumnName;// ds_LOG.Tables[i].Columns[j].ColumnName.Substring(6, ds_LOG.Tables[i].Columns[j].ColumnName.Length - 6);
                                        t_cnt++;
                                    }
                                }
                            }
                        }
                    }
                    t_csv_spec.WriteHeader(header);

                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "상한규격";
                    }
                    else
                    {
                        spec_line[0] = "USL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][7].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);
                    if (m_SetLanguage == 0)
                    {//한국어
                        spec_line[0] = "하한규격";
                    }
                    else
                    {
                        spec_line[0] = "LSL";
                    }
                    for (int i = 1; i < header.Length; i++)
                    {
                        int t_spec_idx = -1;
                        if (header[i].Contains("C0:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_0.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C1:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_1.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C2:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_2.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                        else if (header[i].Contains("C3:"))
                        {
                            int.TryParse(header[i].Substring(3, 2), out t_spec_idx);
                            t_spec_idx--;
                            if (t_spec_idx >= 0)
                            {
                                spec_line[i] = ds_DATA_3.Tables[1].Rows[t_spec_idx][6].ToString();
                            }
                            else
                            {
                                spec_line[i] = "0";
                            }
                        }
                    }
                    t_csv_spec.WriteRecord(spec_line);

                }

                t_csv_spec.Close();
            }
            catch
            { }
        }

        public void Initialize_Data_Log(int Cam_num)
        {
            try
            {
                //if (ds_LOG.Tables.Count != 0)
                //{
                //    ds_LOG.Tables[Cam_num].Reset();
                //}

                if (Cam_num == 0)
                {
                    m_Log_Data_Cnt[0] = 0;
                    DataTable table_data_0 = new DataTable("LOG_0");
                    table_data_0.Columns.Add("No.");
                    bool t_check_cnt = false;
                    for (int i = 0; i < ds_DATA_0.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_0.Tables[0].Rows[i][2].ToString() == "")
                        {
                            continue;
                        }
                        t_check_cnt = true;
                    }
                    if (t_check_cnt)
                    {
                        table_data_0.Columns.Add("C0:Time");
                    }

                    for (int i = 0; i < ds_DATA_0.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_0.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_0.Tables[0].Rows[i][0].ToString().Contains("alse"))
                        {
                            //MessageBox.Show(ds_DATA_0.Tables[0].Rows[i][0].ToString());
                            //continue;
                        }
                        else
                        {
                            //MessageBox.Show(ds_DATA_0.Tables[0].Rows[i][0].ToString());
                            table_data_0.Columns.Add(ds_DATA_0.Tables[0].Rows[i][1].ToString() + "_" + ds_DATA_0.Tables[0].Rows[i][2].ToString());
                        }
                    }
                    if (ds_LOG.Tables.Count == 0)
                    {
                        ds_LOG.Tables.Add(table_data_0);
                    }

                    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.DataSource = ds_LOG.Tables[0];

                    for (int i = 0; i < m_Log_Save_Num; i++)
                    {
                        ds_LOG.Tables[0].Rows.Add();
                        ds_LOG.Tables[0].Rows[i][0] = (i + 1).ToString("000000000");
                        for (int j = 1; j < ds_LOG.Tables[0].Columns.Count; j++)
                        {
                            ds_LOG.Tables[0].Rows[i][j] = "";
                        }
                    }
                    LVApp.Instance().m_mainform.ctr_Yield1.Update_UI();
                }
                else if (Cam_num == 1)
                {
                    m_Log_Data_Cnt[1] = 0;
                    DataTable table_data_1 = new DataTable("LOG_1");

                    table_data_1.Columns.Add("No.");
                    bool t_check_cnt = false;
                    for (int i = 0; i < ds_DATA_1.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_1.Tables[0].Rows[i][2].ToString() == "")
                        {
                            continue;
                        }
                        t_check_cnt = true;
                    }
                    if (t_check_cnt)
                    {
                        table_data_1.Columns.Add("C1:Time");
                    }

                    for (int i = 0; i < ds_DATA_1.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_1.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_1.Tables[0].Rows[i][0].ToString().Contains("alse"))
                        {
                            continue;
                        }
                        table_data_1.Columns.Add(ds_DATA_1.Tables[0].Rows[i][1].ToString() + "_" + ds_DATA_1.Tables[0].Rows[i][2].ToString());
                    }
                    if (ds_LOG.Tables.Count == 1)
                    {
                        ds_LOG.Tables.Add(table_data_1);
                    }
                    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.DataSource = ds_LOG.Tables[1];

                    for (int i = 0; i < m_Log_Save_Num; i++)
                    {
                        ds_LOG.Tables[1].Rows.Add();
                        ds_LOG.Tables[1].Rows[i][0] = (i + 1).ToString("000000000");
                        for (int j = 1; j < ds_LOG.Tables[1].Columns.Count; j++)
                        {
                            ds_LOG.Tables[1].Rows[i][j] = "";
                        }
                    }
                }
                else if (Cam_num == 2)
                {
                    m_Log_Data_Cnt[2] = 0;
                    DataTable table_data_2 = new DataTable("LOG_2");

                    table_data_2.Columns.Add("No.");
                    bool t_check_cnt = false;
                    for (int i = 0; i < ds_DATA_2.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_2.Tables[0].Rows[i][2].ToString() == "")
                        {
                            continue;
                        }
                        t_check_cnt = true;
                    }
                    if (t_check_cnt)
                    {
                        table_data_2.Columns.Add("C2:Time");
                    }

                    for (int i = 0; i < ds_DATA_2.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_2.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_2.Tables[0].Rows[i][0].ToString().Contains("alse"))
                        {
                            continue;
                        }
                        table_data_2.Columns.Add(ds_DATA_2.Tables[0].Rows[i][1].ToString() + "_" + ds_DATA_2.Tables[0].Rows[i][2].ToString());
                    }
                    if (ds_LOG.Tables.Count == 2)
                    {
                        ds_LOG.Tables.Add(table_data_2);
                    }
                    LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.DataSource = ds_LOG.Tables[2];

                    for (int i = 0; i < m_Log_Save_Num; i++)
                    {
                        ds_LOG.Tables[2].Rows.Add();
                        ds_LOG.Tables[2].Rows[i][0] = (i + 1).ToString("000000000");
                        for (int j = 1; j < ds_LOG.Tables[2].Columns.Count; j++)
                        {
                            ds_LOG.Tables[2].Rows[i][j] = "";
                        }
                    }
                }
                else if (Cam_num == 3)
                {
                    m_Log_Data_Cnt[3] = 0;
                    DataTable table_data_3 = new DataTable("LOG_3");

                    table_data_3.Columns.Add("No.");
                    bool t_check_cnt = false;
                    for (int i = 0; i < ds_DATA_3.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_3.Tables[0].Rows[i][2].ToString() == "")
                        {
                            continue;
                        }
                        t_check_cnt = true;
                    }
                    if (t_check_cnt)
                    {
                        table_data_3.Columns.Add("C3:Time");
                    }

                    for (int i = 0; i < ds_DATA_3.Tables[0].Rows.Count; i++)
                    {
                        if (ds_DATA_3.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_3.Tables[0].Rows[i][0].ToString().Contains("alse"))
                        {
                            continue;
                        }
                        table_data_3.Columns.Add(ds_DATA_3.Tables[0].Rows[i][1].ToString() + "_" + ds_DATA_3.Tables[0].Rows[i][2].ToString());
                    }

                    if (ds_LOG.Tables.Count == 3)
                    {
                        ds_LOG.Tables.Add(table_data_3);
                    }
                    LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.DataSource = ds_LOG.Tables[3];

                    for (int i = 0; i < m_Log_Save_Num; i++)
                    {
                        ds_LOG.Tables[3].Rows.Add();
                        ds_LOG.Tables[3].Rows[i][0] = (i + 1).ToString("000000000");
                        for (int j = 1; j < ds_LOG.Tables[3].Columns.Count; j++)
                        {
                            ds_LOG.Tables[3].Rows[i][j] = "";
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void Add_Log_Data(int Cam_num, string t_filename)
        {
            //return;
            try
            {
                //if (m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num == (double)m_Log_Save_Num - 1 && m_Log_Data_Cnt[Cam_num] > 0)
                //{
                //    if (m_Data_Log_Use_Check)
                //    {
                //        if (Cam_num == 0)
                //        {
                //            //threads[Cam_num].Abort();
                //            threads[Cam_num] = null;
                //            threads[Cam_num] = new Thread(ThreadProc0);
                //            //threads[Cam_num].Priority = ThreadPriority.Normal;
                //            threads[Cam_num].Start();
                //        }
                //        else if (Cam_num == 1)
                //        {
                //            //threads[Cam_num].Abort();
                //            threads[Cam_num] = null;
                //            threads[Cam_num] = new Thread(ThreadProc1);
                //            //threads[Cam_num].Priority = ThreadPriority.Normal;
                //            threads[Cam_num].Start();
                //        }
                //        else if (Cam_num == 2)
                //        {
                //            //threads[Cam_num].Abort();
                //            threads[Cam_num] = null;
                //            threads[Cam_num] = new Thread(ThreadProc2);
                //            //threads[Cam_num].Priority = ThreadPriority.Normal;
                //            threads[Cam_num].Start();
                //        }
                //        else if (Cam_num == 3)
                //        {
                //            //threads[Cam_num].Abort();
                //            threads[Cam_num] = null;
                //            threads[Cam_num] = new Thread(ThreadProc3);
                //            //threads[Cam_num].Priority = ThreadPriority.Normal;
                //            threads[Cam_num].Start();
                //        }
                //    }
                //}

                //lock (this)
                {
                    if (t_filename == "")
                    {
                        t_filename = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                    }
                    switch (Cam_num)
                    {
                        case 0:
                            //if (m_Log_Data_Cnt[Cam_num] % ds_DATA_0.Tables[0].Rows.Count < m_Log_Save_Num)
                            {
                                //if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0 && m_Log_Data_Cnt[Cam_num] > 0)
                                //{
                                //    for (int i = 0; i < ds_LOG.Tables[0].Rows.Count; i++)
                                //    {
                                //        for (int j = 0; j < ds_LOG.Tables[0].Columns.Count; j++)
                                //        {
                                //            ds_LOG.Tables[0].Rows[i][j] = "";
                                //        }
                                //    }
                                //}

                                CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.DataSource];
                                currencyManager0.SuspendBinding();

                                for (int i = 0; i < ds_DATA_0.Tables[0].Rows.Count; i++)
                                {
                                    if (ds_DATA_0.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_0.Tables[0].Rows[i][0].ToString().Contains("alse"))
                                    {
                                        continue;
                                    }

                                    for (int j = 0; j < ds_LOG.Tables[0].Columns.Count; j++)
                                    {
                                        if (j == 0)
                                        {
                                            ds_LOG.Tables[0].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = (m_Log_Data_Cnt[Cam_num] + 1).ToString("000000000");
                                        }
                                        if (ds_LOG.Tables[0].Columns[j].ToString().Contains("Time"))
                                        {
                                            ds_LOG.Tables[0].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = t_filename;
                                        }
                                        if (ds_LOG.Tables[0].Columns[j].ToString().Contains(ds_DATA_0.Tables[0].Rows[i][2].ToString()))
                                        {
                                            if (ds_DATA_0.Tables[1].Rows[i][3] != null)
                                            {
                                                ds_LOG.Tables[0].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = ds_DATA_0.Tables[1].Rows[i][3];
                                            }
                                        }
                                    }
                                }

                                if (m_Data_Log_Use_Check)
                                {
                                    t_str_log0 = new string[ds_LOG.Tables[Cam_num].Columns.Count+1];
                                    t_str_log0[0] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];//m_lot_str;//t_str_log_Total[0] = m_lot_str;
                                    LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[Cam_num] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];
                                    LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[Cam_num] = false;
                                    LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num] = string.Empty;
                                    int t_t_idx = 0;
                                    for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                    {
                                        t_str_log0[j+1] = ds_LOG.Tables[Cam_num].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j].ToString();

                                        if (j == 1)
                                        {
                                            if (!t_str_log0[j + 1].Contains("OK"))
                                            {
                                                t_Result_log_Total[Cam_num] = false;
                                            }
                                        }
                                        else
                                        {
                                            t_str_log_Total[t_int_log_Total[Cam_num] + t_t_idx] = t_str_log0[j + 1];
                                            t_t_idx++;
                                        }
                                    }
                                    t_bool_log_Total[Cam_num] = false;
                                    LogThreadProc0();
                                    //lock (CSVLog[4])
                                    //{
                                    //    LogThreadProc_Total();
                                    //}

                                    //threads[Cam_num] = null;
                                    //threads[Cam_num] = new Thread(LogThreadProc0);
                                    //threads[Cam_num].Start();
                                }
                                currencyManager0.ResumeBinding();

                                if (LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 3 || LVApp.Instance().m_Config.neoTabWindow_LOG_idx == 0 && LVApp.Instance().m_Config.neoTabWindow2_LOG_idx == Cam_num)
                                {
                                    if (LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.InvokeRequired)
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Invoke((MethodInvoker)delegate
                                        {
                                            //LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.ClearSelection();
                                            if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                            {
                                                LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                                LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                            }
                                            else
                                            {
                                                LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                                LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                            }
                                            //if (ds_LOG.Tables[0].Rows.Count > 30)
                                            //{
                                            //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                            //    {
                                            //        LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[0].Cells[0];
                                            //    }
                                            //    else
                                            //    {
                                            //        LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[0].Cells[0];
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.ClearSelection();
                                        if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        else
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        //if (ds_LOG.Tables[0].Rows.Count > 30)
                                        //{
                                        //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[0].Cells[0];
                                        //    }
                                        //    else
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Rows[0].Cells[0];
                                        //}
                                    }
                                }
                                //LVApp.Instance().m_mainform.ctr_DataGrid1.Min_Max_Update(Cam_num);
                            }
                            break;

                        case 1:
                            //if (m_Log_Data_Cnt[Cam_num] % ds_DATA_0.Tables[0].Rows.Count < m_Log_Save_Num)
                            //{
                            //if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0 && m_Log_Data_Cnt[Cam_num] > 0)
                            //{
                            //    for (int i = 0; i < ds_LOG.Tables[1].Rows.Count; i++)
                            //    {
                            //        for (int j = 0; j < ds_LOG.Tables[1].Columns.Count; j++)
                            //        {
                            //            ds_LOG.Tables[1].Rows[i][j] = "";
                            //        }
                            //    }
                            //}
                            CurrencyManager currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.DataSource];
                            currencyManager1.SuspendBinding();

                            for (int i = 0; i < ds_DATA_1.Tables[0].Rows.Count; i++)
                            {
                                if (ds_DATA_1.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_1.Tables[0].Rows[i][0].ToString().Contains("alse"))
                                {
                                    continue;
                                }

                                for (int j = 0; j < ds_LOG.Tables[1].Columns.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        ds_LOG.Tables[1].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = (m_Log_Data_Cnt[Cam_num] + 1).ToString("000000000");
                                    }
                                    if (ds_LOG.Tables[1].Columns[j].ToString().Contains("Time"))
                                    {
                                        ds_LOG.Tables[1].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = t_filename;
                                    }
                                    if (ds_LOG.Tables[1].Columns[j].ToString().Contains(ds_DATA_1.Tables[0].Rows[i][2].ToString()))
                                    {
                                        if (ds_DATA_1.Tables[1].Rows[i][3] != null)
                                        {
                                            ds_LOG.Tables[1].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = ds_DATA_1.Tables[1].Rows[i][3];
                                        }
                                    }
                                }
                            }
                            if (m_Data_Log_Use_Check)
                            {
                                t_str_log1 = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                t_str_log1[0] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];//m_lot_str;//t_str_log_Total[0] = m_lot_str;
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[Cam_num] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[Cam_num] = false;
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num] = string.Empty;
                                int t_t_idx = 0;
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    t_str_log1[j + 1] = ds_LOG.Tables[Cam_num].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j].ToString();

                                    if (j == 1)
                                    {
                                        if (!t_str_log1[j + 1].Contains("OK"))
                                        {
                                            t_Result_log_Total[Cam_num] = false;
                                        }
                                    }
                                    else if (j > 1)
                                    {
                                        t_str_log_Total[t_int_log_Total[Cam_num] + t_t_idx] = t_str_log1[j + 1];
                                        t_t_idx++;
                                    }
                                }
                                t_bool_log_Total[Cam_num] = false;
                                LogThreadProc1();
                                //lock (CSVLog[4])
                                //{
                                //    LogThreadProc_Total();
                                //}

                                //threads[Cam_num] = null;
                                //threads[Cam_num] = new Thread(LogThreadProc1);
                                //threads[Cam_num].Start();
                            }
                            currencyManager1.ResumeBinding();

                            if (LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 3 || LVApp.Instance().m_Config.neoTabWindow_LOG_idx == 0 && LVApp.Instance().m_Config.neoTabWindow2_LOG_idx == Cam_num)
                            {
                                if (LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.InvokeRequired)
                                {
                                    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Invoke((MethodInvoker)delegate
                                    {
                                        //LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.ClearSelection();
                                        if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        else
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        //if (ds_LOG.Tables[1].Rows.Count > 30)
                                        //{
                                        //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[0].Cells[0];
                                        //    }
                                        //    else
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[0].Cells[0];
                                        //}

                                    });
                                }
                                else
                                {
                                    //LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.ClearSelection();
                                    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                    }
                                    else
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                    }
                                    //if (ds_LOG.Tables[1].Rows.Count > 30)
                                    //{
                                    //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                    //    {
                                    //        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[0].Cells[0];
                                    //    }
                                    //    else
                                    //    {
                                    //        LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Rows[0].Cells[0];
                                    //}

                                }
                                //LVApp.Instance().m_mainform.ctr_DataGrid2.Min_Max_Update(Cam_num);
                            }

                            break;

                        case 2:
                            //if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0 && m_Log_Data_Cnt[Cam_num] > 0)
                            //{
                            //    for (int i = 0; i < ds_LOG.Tables[2].Rows.Count; i++)
                            //    {
                            //        for (int j = 0; j < ds_LOG.Tables[2].Columns.Count; j++)
                            //        {
                            //            ds_LOG.Tables[2].Rows[i][j] = "";
                            //        }
                            //    }
                            //}
                            CurrencyManager currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.DataSource];
                            currencyManager2.SuspendBinding();

                            for (int i = 0; i < ds_DATA_2.Tables[0].Rows.Count; i++)
                            {
                                if (ds_DATA_2.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_2.Tables[0].Rows[i][0].ToString().Contains("alse"))
                                {
                                    continue;
                                }

                                for (int j = 0; j < ds_LOG.Tables[2].Columns.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        ds_LOG.Tables[2].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = (m_Log_Data_Cnt[Cam_num] + 1).ToString("000000000");
                                    }
                                    if (ds_LOG.Tables[2].Columns[j].ToString().Contains("Time"))
                                    {
                                        ds_LOG.Tables[2].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = t_filename;
                                    }
                                    if (ds_LOG.Tables[2].Columns[j].ToString().Contains(ds_DATA_2.Tables[0].Rows[i][2].ToString()))
                                    {
                                        if (ds_DATA_2.Tables[1].Rows[i][3] != null)
                                        {
                                            ds_LOG.Tables[2].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = ds_DATA_2.Tables[1].Rows[i][3];
                                        }
                                    }
                                }
                            }
                            if (m_Data_Log_Use_Check)
                            {
                                t_str_log2 = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                t_str_log2[0] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];//m_lot_str;//t_str_log_Total[0] = m_lot_str;
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[Cam_num] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[Cam_num] = false;
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num] = string.Empty;
                                int t_t_idx = 0;
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    t_str_log2[j + 1] = ds_LOG.Tables[Cam_num].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j].ToString();

                                    if (j == 1)
                                    {
                                        if (!t_str_log2[j + 1].Contains("OK"))
                                        {
                                            t_Result_log_Total[Cam_num] = false;
                                        }
                                    }
                                    else if (j > 1)
                                    {
                                        t_str_log_Total[t_int_log_Total[Cam_num] + t_t_idx] = t_str_log2[j + 1];
                                        t_t_idx++;
                                    }
                                }
                                t_bool_log_Total[Cam_num] = false;
                                LogThreadProc2();
                                //lock (CSVLog[4])
                                //{
                                //    LogThreadProc_Total();
                                //}
                                //threads[Cam_num] = null;
                                //threads[Cam_num] = new Thread(LogThreadProc2);
                                //threads[Cam_num].Start();
                            }
                            currencyManager2.ResumeBinding();

                            if (LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 3 || LVApp.Instance().m_Config.neoTabWindow_LOG_idx == 0 && LVApp.Instance().m_Config.neoTabWindow2_LOG_idx == Cam_num)
                            {
                                if (LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.InvokeRequired)
                                {
                                    LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Invoke((MethodInvoker)delegate
                                    {
                                        //LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.ClearSelection();
                                        if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        else
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        //if (ds_LOG.Tables[2].Rows.Count > 30)
                                        //{
                                        //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[0].Cells[0];
                                        //    }
                                        //    else
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[0].Cells[0];
                                        //}

                                    });
                                }
                                else
                                {
                                    //LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.ClearSelection();
                                    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                    }
                                    else
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                    }
                                    //if (ds_LOG.Tables[2].Rows.Count > 30)
                                    //{
                                    //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                    //    {
                                    //        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[0].Cells[0];
                                    //    }
                                    //    else
                                    //    {
                                    //        LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.Rows[0].Cells[0];
                                    //}

                                }
                                //LVApp.Instance().m_mainform.ctr_DataGrid3.Min_Max_Update(Cam_num);
                            }

                            break;
                        case 3:
                            //if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0 && m_Log_Data_Cnt[Cam_num] > 0)
                            //    {
                            //        for (int i = 0; i < ds_LOG.Tables[3].Rows.Count; i++)
                            //        {
                            //            for (int j = 0; j < ds_LOG.Tables[3].Columns.Count; j++)
                            //            {
                            //                ds_LOG.Tables[3].Rows[i][j] = "";
                            //            }
                            //        }
                            //    }
                            CurrencyManager currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.DataSource];
                            currencyManager3.SuspendBinding();

                            for (int i = 0; i < ds_DATA_3.Tables[0].Rows.Count; i++)
                            {
                                if (ds_DATA_3.Tables[0].Rows[i][2].ToString() == "" || ds_DATA_3.Tables[0].Rows[i][0].ToString().Contains("alse"))
                                {
                                    continue;
                                }

                                for (int j = 0; j < ds_LOG.Tables[3].Columns.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        ds_LOG.Tables[3].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = (m_Log_Data_Cnt[Cam_num] + 1).ToString("000000000");
                                    }
                                    if (ds_LOG.Tables[3].Columns[j].ToString().Contains("Time"))
                                    {
                                        ds_LOG.Tables[3].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = t_filename;
                                    }
                                    if (ds_LOG.Tables[3].Columns[j].ToString().Contains(ds_DATA_3.Tables[0].Rows[i][2].ToString()))
                                    {
                                        if (ds_DATA_3.Tables[1].Rows[i][3] != null)
                                        {
                                            ds_LOG.Tables[3].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j] = ds_DATA_3.Tables[1].Rows[i][3];
                                        }
                                    }
                                }
                            }
                            if (m_Data_Log_Use_Check)
                            {
                                t_str_log3 = new string[ds_LOG.Tables[Cam_num].Columns.Count + 1];
                                t_str_log3[0] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];//m_lot_str;//t_str_log_Total[0] = m_lot_str;
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[Cam_num] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num];
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[Cam_num] = false;
                                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value[Cam_num] = string.Empty;
                                int t_t_idx = 0;
                                for (int j = 0; j < ds_LOG.Tables[Cam_num].Columns.Count; j++)
                                {
                                    t_str_log3[j + 1] = ds_LOG.Tables[Cam_num].Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)][j].ToString();

                                    if (j == 1)
                                    {
                                        if (!t_str_log3[j + 1].Contains("OK"))
                                        {
                                            t_Result_log_Total[Cam_num] = false;
                                        }
                                    }
                                    else if (j > 1)
                                    {
                                        t_str_log_Total[t_int_log_Total[Cam_num] + t_t_idx] = t_str_log3[j + 1];
                                        t_t_idx++;
                                    }
                                }
                                t_bool_log_Total[Cam_num] = false;
                                LogThreadProc3();
                                //lock (CSVLog[4])
                                //{
                                //    LogThreadProc_Total();
                                //}
                                //threads[Cam_num] = null;
                                //threads[Cam_num] = new Thread(LogThreadProc3);
                                //threads[Cam_num].Start();

                                //CSVLog[Cam_num].WriteRecord(header3);
                            }
                            currencyManager3.ResumeBinding();
                            if (LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 3 || LVApp.Instance().m_Config.neoTabWindow_LOG_idx == 0 && LVApp.Instance().m_Config.neoTabWindow2_LOG_idx == Cam_num)
                            {

                                if (LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.InvokeRequired)
                                {
                                    LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Invoke((MethodInvoker)delegate
                                    {
                                        //LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.ClearSelection();
                                        if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        else
                                        {
                                            LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                            LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                        }
                                        //if (ds_LOG.Tables[3].Rows.Count > 30)
                                        //{
                                        //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[0].Cells[0];
                                        //    }
                                        //    else
                                        //    {
                                        //        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[0].Cells[0];
                                        //}

                                    });
                                }
                                else
                                {
                                    //LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.ClearSelection();
                                    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) == 0)
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[-1 + m_Log_Save_Num].Selected = false;
                                        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                    }
                                    else
                                    {
                                        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[-1 + (int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = false;
                                        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[(int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)].Selected = true;
                                    }
                                    //if (ds_LOG.Tables[3].Rows.Count > 30)
                                    //{
                                    //    if ((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num) < 30)
                                    //    {
                                    //        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[0].Cells[0];
                                    //    }
                                    //    else
                                    //    {
                                    //        LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[((int)(m_Log_Data_Cnt[Cam_num] % (double)m_Log_Save_Num)) - 30].Cells[0];
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.FirstDisplayedCell = LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.Rows[0].Cells[0];
                                    //}

                                }
                                //LVApp.Instance().m_mainform.ctr_DataGrid4.Min_Max_Update(Cam_num);
                            }

                            break;

                        // 4~7까지 추가해야 함.
                    }
                }
                if (m_Log_Data_Cnt[Cam_num] == 999999999)
                {
                    m_Log_Data_Cnt[Cam_num] = 1;
                }
                else
                {
                    m_Log_Data_Cnt[Cam_num]++;
                }
            }
            catch //(System.Exception ex)
            {

            }
        }


        public void Stop_Save_Log()
        {
            if (m_Data_Log_Use_Check)
            {
                //destinationTable = ds_LOG.Tables[0].Clone();
                //for (int i = 0; i < 5; i++)
                //{
                //    m_Log_Data_Cnt[Cam_num] = 0;
                //}
                //threads[0] = null;
                //threads[0] = new Thread(ThreadProc0);
                //threads[0].Priority = ThreadPriority.Normal;
                //threads[0].Start();
                //threads[1] = null;
                //threads[1] = new Thread(ThreadProc1);
                //threads[1].Priority = ThreadPriority.Normal;
                //threads[1].Start();
                //threads[2] = null;
                //threads[2] = new Thread(ThreadProc2);
                //threads[2].Priority = ThreadPriority.Normal;
                //threads[2].Start();
                //threads[3] = null;
                //threads[3] = new Thread(ThreadProc3);
                //threads[3].Priority = ThreadPriority.Normal;
                //threads[3].Start();
                //threads[4] = null;
                //threads[4] = new Thread(ThreadProc0);
                //threads[4].Priority = ThreadPriority.Normal;
                //threads[4].Start();
            }
        }
        //threads[0] = null;
        //threads[0] = new Thread(LogThreadProc0);
        //threads[0].Start();

        public string[] t_str_log0;
        public void LogThreadProc0()
        {
            try
            {
                CSVLog[0].WriteRecord(t_str_log0);
                CSVDataLog[0].WriteRecord(t_str_log0);
            }
            catch
            { }
        }
        public string[] t_str_log1;
        public void LogThreadProc1()
        {
            try
            {
                CSVLog[1].WriteRecord(t_str_log1);
                CSVDataLog[1].WriteRecord(t_str_log1);
            }
            catch
            { }
        }
        public string[] t_str_log2;
        public void LogThreadProc2()
        {
            try
            {
                CSVLog[2].WriteRecord(t_str_log2);
                CSVDataLog[2].WriteRecord(t_str_log2);
            }
            catch
            { }
        }
        public string[] t_str_log3;
        public void LogThreadProc3()
        {
            try
            {
                CSVLog[3].WriteRecord(t_str_log3);
                CSVDataLog[3].WriteRecord(t_str_log3);
            }
            catch
            { }
        }
        public string[] t_str_log_Total;
        public bool[] t_bool_log_Total = new bool[4];
        public int[] t_int_log_Total = new int[4];
        public bool[] t_Result_log_Total = new bool[4];
        public void LogThreadProc_Total()
        {
            try
            {
                while (true)
                {
                    if (Total_Log_Save_Threads_JOB == 0)
                    {
                        Thread.Sleep(1);

                        if (t_bool_log_Total[0] == LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0]
                         && t_bool_log_Total[1] == LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1]
                         && t_bool_log_Total[2] == LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2]
                         && t_bool_log_Total[3] == LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                        {
                            Total_Log_Save_Threads_JOB = 1;
                        }
                    }
                    else if (Total_Log_Save_Threads_JOB == 1)
                    {

                        if (!t_Result_log_Total[0] || !t_Result_log_Total[1] || !t_Result_log_Total[2] || !t_Result_log_Total[3])
                        {
                            t_str_log_Total[0] = "NG";
                        }
                        else
                        {
                            t_str_log_Total[0] = "OK";
                        }

                        t_str_log_Total[1] = "";
                        
                        if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[0].Length > 0)
                        {
                            t_str_log_Total[1] = LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[0];
                        }
                        if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[1].Length > 0 && LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[0] != LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[1])
                        {
                            if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[1].Length > 0)
                            {
                                t_str_log_Total[1] += "_" + LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[1];
                            }
                            else
                            {
                                t_str_log_Total[1] += LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[1];
                            }
                        }
                        if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[2].Length > 0 && LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[1] != LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[2])
                        {
                            if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[2].Length > 0)
                            {
                                t_str_log_Total[1] += "_" + LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[2];
                            }
                            else
                            {
                                t_str_log_Total[1] += LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[2];
                            }
                        }
                        if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[3].Length > 0 && LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[2] != LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[3])
                        {
                            if (LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[3].Length > 0)
                            {
                                t_str_log_Total[1] += "_" + LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[3];
                            }
                            else
                            {
                                t_str_log_Total[1] += LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_STR_Value_for_LOG[3];
                            }
                        }

                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[0] =
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[1] =
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[2] =
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value_Updated[3] = false;
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value[0] =
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value[1] =
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value[2] =
                        //LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Value[3] = -1;

                        t_bool_log_Total[0] = true;
                        t_bool_log_Total[1] = true;
                        t_bool_log_Total[2] = true;
                        t_bool_log_Total[3] = true;
                        t_Result_log_Total[0] = true;
                        t_Result_log_Total[1] = true;
                        t_Result_log_Total[2] = true;
                        t_Result_log_Total[3] = true;
                        if (CSVLog[4] != null)
                        {
                            CSVLog[4].WriteRecord(t_str_log_Total);
                        }
                        if (m_Data_Save_Folder != "" && CSVDataLog[4] != null)
                        {
                            CSVDataLog[4].WriteRecord(t_str_log_Total);
                        }
                        Total_Log_Save_Threads_JOB = 0;
                    }
                }
            }
            catch
            {
                Total_Log_Save_Threads.Abort();
                Total_Log_Save_Threads = new Thread(LogThreadProc_Total);
                Total_Log_Save_Threads.IsBackground = true;
                Total_Log_Save_Threads.Start();
            }
        }


        private bool[] t_Log_Save_Flag = new bool[5];
        private void ThreadProc0()
        {
            //int Cam_num = 0;
            try
            {
                //lock (this)
                {
                    t_Log_Save_Flag[0] = true;

                    CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.DataSource];
                    currencyManager0.SuspendBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[0] = new DataTable();
                    //        destinationTable[0] = ds_LOG.Tables[0].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    destinationTable[0] = new DataTable();
                    destinationTable[0] = ds_LOG.Tables[0].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //Initialize_Data_Log();
                    // }
                    currencyManager0.ResumeBinding();

                    //destinationTable.WriteToCsvFile(m_Log_File_Name);

                    String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM0\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    if (m_Log_Save_Folder != "")
                    {
                        m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM0\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    }

                    FileInfo templateFile = new FileInfo(m_Log_File_Name.Substring(0, m_Log_File_Name.Length - 3) + "xlsx");

                    using (ExcelPackage pck = new ExcelPackage(templateFile))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Data");
                        ws.Cells["A1"].LoadFromDataTable(destinationTable[0], true, TableStyles.Medium8);
                        //ws.Cells["B1:C5"].Style.Numberformat.Format = "#,##0";
                        var rowCnt = ws.Dimension.End.Row;
                        var colCnt = ws.Dimension.End.Column;

                        if (rowCnt < 2 || colCnt < 1)
                        {
                            destinationTable[0].Dispose();
                            destinationTable[0] = new DataTable();
                            t_Log_Save_Flag[0] = false;
                            return;
                        }

                        for (int j = 1; j <= colCnt; j++)
                        {
                            if (ws.Cells[1, j].Value != null)
                            {
                                if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                {
                                    ws.Column(j).Width = 25;
                                }
                            }
                        }

                        for (int i = 2; i <= rowCnt; i++)
                        {
                            for (int j = 1; j <= colCnt; j++)
                            {
                                if (ws.Cells[i, j].Value != null)
                                {
                                    if (ws.Cells[i, j].Value.ToString() == "a0")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (ws.Cells[i, j].Value.ToString() != "")
                                        {
                                            if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                            {
                                                ws.Cells[i, j].Value = ws.Cells[i, j].Value;
                                            }
                                            else
                                            {
                                                ws.Cells[i, j].Value = Convert.ToDouble(ws.Cells[i, j].Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        pck.Save();
                    }

                    destinationTable[0].Dispose();
                    destinationTable[0] = new DataTable();
                    t_Log_Save_Flag[0] = false;
                }
            }
            catch
            {
            }
        }

        private void ThreadProc1()
        {
            //int Cam_num = 1;
            try
            {
                //lock (this)
                {
                    t_Log_Save_Flag[1] = true;

                    CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.DataSource];
                    currencyManager0.SuspendBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[0] = new DataTable();
                    //        destinationTable[0] = ds_LOG.Tables[0].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    destinationTable[1] = new DataTable();
                    destinationTable[1] = ds_LOG.Tables[1].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //Initialize_Data_Log();
                    // }
                    currencyManager0.ResumeBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[1] = new DataTable();
                    //        destinationTable[1] = ds_LOG.Tables[1].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    //    destinationTable[1] = new DataTable();
                    //    destinationTable[1] = ds_LOG.Tables[1].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //    //Initialize_Data_Log();
                    //}

                    //destinationTable.WriteToCsvFile(m_Log_File_Name);

                    String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM1\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    if (m_Log_Save_Folder != "")
                    {
                        m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM1\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    }

                    FileInfo templateFile = new FileInfo(m_Log_File_Name.Substring(0, m_Log_File_Name.Length - 3) + "xlsx");

                    using (ExcelPackage pck = new ExcelPackage(templateFile))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Data");
                        ws.Cells["A1"].LoadFromDataTable(destinationTable[1], true, TableStyles.Medium8);
                        //ws.Cells["B1:C5"].Style.Numberformat.Format = "#,##0";
                        var rowCnt = ws.Dimension.End.Row;
                        var colCnt = ws.Dimension.End.Column;

                        if (rowCnt < 2 || colCnt < 1)
                        {
                            destinationTable[1].Dispose();
                            destinationTable[1] = new DataTable();
                            t_Log_Save_Flag[1] = false;
                            return;
                        }

                        for (int j = 1; j <= colCnt; j++)
                        {
                            if (ws.Cells[1, j].Value != null)
                            {
                                if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                {
                                    ws.Column(j).Width = 25;
                                }
                            }
                        }

                        for (int i = 2; i <= rowCnt; i++)
                        {
                            for (int j = 1; j <= colCnt; j++)
                            {
                                if (ws.Cells[i, j].Value != null)
                                {
                                    if (ws.Cells[i, j].Value.ToString() == "a0")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (ws.Cells[i, j].Value.ToString() != "")
                                        {
                                            if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                            {
                                                ws.Cells[i, j].Value = ws.Cells[i, j].Value;
                                            }
                                            else
                                            {
                                                ws.Cells[i, j].Value = Convert.ToDouble(ws.Cells[i, j].Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        pck.Save();
                    }

                    destinationTable[1].Dispose();
                    destinationTable[1] = new DataTable();
                    t_Log_Save_Flag[1] = false;
                }
            }
            catch
            {
            }
        }

        private void ThreadProc2()
        {
            // int Cam_num = 2;
            try
            {
                //lock (this)
                {
                    t_Log_Save_Flag[2] = true;

                    CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid3.dataGridView.DataSource];
                    currencyManager0.SuspendBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[0] = new DataTable();
                    //        destinationTable[0] = ds_LOG.Tables[0].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    destinationTable[2] = new DataTable();
                    destinationTable[2] = ds_LOG.Tables[2].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //Initialize_Data_Log();
                    // }
                    currencyManager0.ResumeBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[2] = new DataTable();
                    //        destinationTable[2] = ds_LOG.Tables[2].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    //    destinationTable[2] = new DataTable();
                    //    destinationTable[2] = ds_LOG.Tables[2].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //    //Initialize_Data_Log();
                    //}

                    //destinationTable.WriteToCsvFile(m_Log_File_Name);

                    String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM2\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    if (m_Log_Save_Folder != "")
                    {
                        m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM2\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    }

                    FileInfo templateFile = new FileInfo(m_Log_File_Name.Substring(0, m_Log_File_Name.Length - 3) + "xlsx");

                    using (ExcelPackage pck = new ExcelPackage(templateFile))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Data");
                        ws.Cells["A1"].LoadFromDataTable(destinationTable[2], true, TableStyles.Medium8);
                        //ws.Cells["B1:C5"].Style.Numberformat.Format = "#,##0";
                        var rowCnt = ws.Dimension.End.Row;
                        var colCnt = ws.Dimension.End.Column;

                        if (rowCnt < 2 || colCnt < 1)
                        {
                            destinationTable[2].Dispose();
                            destinationTable[2] = new DataTable();
                            t_Log_Save_Flag[2] = false;
                            return;
                        }

                        for (int j = 1; j <= colCnt; j++)
                        {
                            if (ws.Cells[1, j].Value != null)
                            {
                                if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                {
                                    ws.Column(j).Width = 25;
                                }
                            }
                        }

                        for (int i = 2; i <= rowCnt; i++)
                        {
                            for (int j = 1; j <= colCnt; j++)
                            {
                                if (ws.Cells[i, j].Value != null)
                                {
                                    if (ws.Cells[i, j].Value.ToString() == "a0")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (ws.Cells[i, j].Value.ToString() != "")
                                        {
                                            if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                            {
                                                ws.Cells[i, j].Value = ws.Cells[i, j].Value;
                                            }
                                            else
                                            {
                                                ws.Cells[i, j].Value = Convert.ToDouble(ws.Cells[i, j].Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        pck.Save();
                    }

                    destinationTable[2].Dispose();
                    destinationTable[2] = new DataTable();
                    t_Log_Save_Flag[2] = false;
                }
            }
            catch
            {

            }
        }

        private void ThreadProc3()
        {
            int Cam_num = 3;
            try
            {
                //lock (this)
                {
                    t_Log_Save_Flag[3] = true;

                    CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.BindingContext[LVApp.Instance().m_mainform.ctr_DataGrid4.dataGridView.DataSource];
                    currencyManager0.SuspendBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[0] = new DataTable();
                    //        destinationTable[0] = ds_LOG.Tables[0].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    destinationTable[3] = new DataTable();
                    destinationTable[3] = ds_LOG.Tables[3].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //Initialize_Data_Log();
                    // }
                    currencyManager0.ResumeBinding();

                    //if (LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.InvokeRequired)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_DataGrid1.dataGridView.Invoke((MethodInvoker)delegate
                    //    {
                    //        destinationTable[Cam_num] = new DataTable();
                    //        destinationTable[Cam_num] = ds_LOG.Tables[Cam_num].Copy();// CopyDataTable(ds_LOG.Tables[Cam_num].Clone(), m_Log_Save_Num);
                    //        //Initialize_Data_Log();
                    //    });
                    //}
                    //else
                    //{
                    //    destinationTable[Cam_num] = new DataTable();
                    //    destinationTable[Cam_num] = ds_LOG.Tables[Cam_num].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //    //Initialize_Data_Log();
                    //}

                    //destinationTable.WriteToCsvFile(m_Log_File_Name);

                    String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM3\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    if (m_Log_Save_Folder != "")
                    {
                        m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM3\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                    }

                    FileInfo templateFile = new FileInfo(m_Log_File_Name.Substring(0, m_Log_File_Name.Length - 3) + "xlsx");

                    using (ExcelPackage pck = new ExcelPackage(templateFile))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Data");
                        ws.Cells["A1"].LoadFromDataTable(destinationTable[Cam_num], true, TableStyles.Medium8);
                        //ws.Cells["B1:C5"].Style.Numberformat.Format = "#,##0";
                        var rowCnt = ws.Dimension.End.Row;
                        var colCnt = ws.Dimension.End.Column;

                        if (rowCnt < 2 || colCnt < 1)
                        {
                            destinationTable[Cam_num].Dispose();
                            destinationTable[Cam_num] = new DataTable();
                            t_Log_Save_Flag[Cam_num] = false;
                            return;
                        }

                        for (int j = 1; j <= colCnt; j++)
                        {
                            if (ws.Cells[1, j].Value != null)
                            {
                                if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                {
                                    ws.Column(j).Width = 25;
                                }
                            }
                        }

                        for (int i = 2; i <= rowCnt; i++)
                        {
                            for (int j = 1; j <= colCnt; j++)
                            {
                                if (ws.Cells[i, j].Value != null)
                                {
                                    if (ws.Cells[i, j].Value.ToString() == "a0")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (ws.Cells[i, j].Value.ToString() != "")
                                        {
                                            if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                            {
                                                ws.Cells[i, j].Value = ws.Cells[i, j].Value;
                                            }
                                            else
                                            {
                                                ws.Cells[i, j].Value = Convert.ToDouble(ws.Cells[i, j].Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        pck.Save();
                    }

                    destinationTable[Cam_num].Dispose();
                    destinationTable[Cam_num] = new DataTable();
                    t_Log_Save_Flag[Cam_num] = false;
                }
            }
            catch
            {
            }
        }

        private void ThreadProc4()
        {
            int Cam_num = 4;
            lock (this)
            {
                t_Log_Save_Flag[Cam_num] = true;
                DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd"));
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM4");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                if (LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.InvokeRequired)
                {
                    LVApp.Instance().m_mainform.ctr_DataGrid2.dataGridView.Invoke((MethodInvoker)delegate
                    {
                        destinationTable[Cam_num] = new DataTable();
                        destinationTable[Cam_num] = ds_LOG.Tables[Cam_num].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                        //Initialize_Data_Log();
                    });
                }
                else
                {
                    destinationTable[Cam_num] = new DataTable();
                    destinationTable[Cam_num] = ds_LOG.Tables[Cam_num].Copy();// CopyDataTable(ds_LOG.Tables[0].Clone(), m_Log_Save_Num);
                    //Initialize_Data_Log();
                }

                //destinationTable.WriteToCsvFile(m_Log_File_Name);

                String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM4\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                if (m_Log_Save_Folder != "")
                {
                    m_Log_File_Name = m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM4\\" + DateTime.Now.ToString("HHmmss_fff") + ".csv"; //파일경로
                }

                FileInfo templateFile = new FileInfo(m_Log_File_Name.Substring(0, m_Log_File_Name.Length - 3) + "xlsx");

                using (ExcelPackage pck = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Data");
                    ws.Cells["A1"].LoadFromDataTable(destinationTable[Cam_num], true, TableStyles.Medium8);
                    //ws.Cells["B1:C5"].Style.Numberformat.Format = "#,##0";
                    var rowCnt = ws.Dimension.End.Row;
                    var colCnt = ws.Dimension.End.Column;

                    if (rowCnt < 2 || colCnt < 1)
                    {
                        destinationTable[Cam_num].Dispose();
                        destinationTable[Cam_num] = new DataTable();
                        t_Log_Save_Flag[Cam_num] = false;
                        return;
                    }

                    for (int j = 1; j <= colCnt; j++)
                    {
                        if (ws.Cells[1, j].Value != null)
                        {
                            if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                            {
                                ws.Column(j).Width = 25;
                            }
                        }
                    }

                    for (int i = 2; i <= rowCnt; i++)
                    {
                        for (int j = 1; j <= colCnt; j++)
                        {
                            if (ws.Cells[i, j].Value != null)
                            {
                                if (ws.Cells[i, j].Value.ToString() == "a0")
                                {
                                    break;
                                }
                                else
                                {
                                    if (ws.Cells[i, j].Value.ToString() != "")
                                    {
                                        if (ws.Cells[1, j].Value.ToString().Contains("Time"))
                                        {
                                            ws.Cells[i, j].Value = ws.Cells[i, j].Value;
                                        }
                                        else
                                        {
                                            ws.Cells[i, j].Value = Convert.ToDouble(ws.Cells[i, j].Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    pck.Save();
                }

                destinationTable[Cam_num].Dispose();
                destinationTable[Cam_num] = new DataTable();
                t_Log_Save_Flag[Cam_num] = false;
            }
        }

        public void Exel_basic_Setting_Create()
        {
            FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + m_Model_Name + "\\" + m_Model_Name + ".xlsx");
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // Add a worksheet to the empty workbook
                ExcelWorksheet[] worksheet = new ExcelWorksheet[6];
                worksheet[0] = package.Workbook.Worksheets.Add("System");
                worksheet[1] = package.Workbook.Worksheets.Add("Insp. Param.");
                worksheet[2] = package.Workbook.Worksheets.Add("Communication");
                worksheet[3] = package.Workbook.Worksheets.Add("Camera");
                worksheet[4] = package.Workbook.Worksheets.Add("ETC");
                worksheet[5] = package.Workbook.Worksheets.Add("UI Data");

                for (int i = 0; i < 6; i++)
                {
                    worksheet[i].Cells[1, 1].Value = "Item";
                    worksheet[i].Cells[1, 2].Value = "Value";
                    worksheet[i].Cells[1, 3].Value = "Remark";
                    worksheet[i].DefaultColWidth = 35;
                }
                worksheet[0].Cells[2, 1].Value = "CAM0 이미지 저장";
                worksheet[0].Cells[2, 2].Value = "1";
                worksheet[0].Cells[3, 1].Value = "CAM1 이미지 저장";
                worksheet[0].Cells[3, 2].Value = "1";
                worksheet[0].Cells[4, 1].Value = "CAM2 이미지 저장";
                worksheet[0].Cells[4, 2].Value = "1";
                worksheet[0].Cells[5, 1].Value = "저장방법";
                worksheet[0].Cells[5, 2].Value = "Only NG";
                worksheet[0].Cells[6, 1].Value = "저장기간(일)";
                worksheet[0].Cells[6, 2].Value = "30";
                worksheet[0].Cells[7, 1].Value = "저장포멧";
                worksheet[0].Cells[7, 2].Value = "jpg";
                worksheet[0].Cells[8, 1].Value = "CAM0 해상도";
                worksheet[0].Cells[8, 2].Value = "1000";
                worksheet[0].Cells[9, 1].Value = "CAM1 해상도";
                worksheet[0].Cells[9, 2].Value = "1000";
                worksheet[0].Cells[10, 1].Value = "CAM2 해상도";
                worksheet[0].Cells[10, 2].Value = "1000";
                worksheet[0].Cells[11, 1].Value = "CAM0 Use유무";
                worksheet[0].Cells[11, 2].Value = "1";
                worksheet[0].Cells[12, 1].Value = "CAM1 Use유무";
                worksheet[0].Cells[12, 2].Value = "1";
                worksheet[0].Cells[13, 1].Value = "CAM2 Use유무";
                worksheet[0].Cells[13, 2].Value = "0";
                worksheet[0].Cells[14, 1].Value = "로그 Use유무";
                worksheet[0].Cells[14, 2].Value = "1";
                worksheet[0].Cells[15, 1].Value = "로그 저장기간(일)";
                worksheet[0].Cells[15, 2].Value = "365";

                worksheet[0].Cells[2, 2].Value = "0";
                worksheet[0].Cells[3, 2].Value = "0";
                worksheet[0].Cells[4, 2].Value = "0";
                worksheet[0].Cells[5, 2].Value = "0";
                worksheet[0].Cells[6, 2].Value = "0";
                worksheet[0].Cells[7, 2].Value = "0";
                worksheet[0].Cells[8, 2].Value = "0";
                worksheet[0].Cells[9, 2].Value = "0";
                worksheet[0].Cells[10, 2].Value = "Only NG";
                worksheet[0].Cells[11, 2].Value = "30";
                worksheet[0].Cells[12, 2].Value = "jpg";

                worksheet[0].Cells[13, 2].Value = "0";
                worksheet[0].Cells[14, 2].Value = "30";
                worksheet[0].Cells[15, 2].Value = "50";
                worksheet[0].Cells[16, 2].Value = "";

                worksheet[0].Cells[17, 2].Value = "1";


                worksheet[1].Cells[1, 1].Value = "Use유무";
                worksheet[1].Cells[1, 2].Value = "이니셜";
                worksheet[1].Cells[1, 3].Value = "이름";
                worksheet[1].Cells[1, 4].Value = "측정방법";
                worksheet[1].Cells[1, 5].Value = "보정값";
                worksheet[1].Cells[1, 6].Value = "하한값";
                worksheet[1].Cells[1, 7].Value = "기준값";
                worksheet[1].Cells[1, 8].Value = "상한값";
                worksheet[1].Cells[1, 9].Value = "CLASS";

                int t_num = 2;

                //if (dataGridView2.RowCount == 0)
                //{
                //    Add_Measurement_Item();
                //}

                //for (int i = 0; i < dataGridView2.RowCount; i++)
                //{
                //    worksheet[1].Cells[t_num, 1].Value = "1";
                //    worksheet[1].Cells[t_num, 2].Value = dataGridView2.Rows[i].Cells[1].Value.ToString();//"H0";
                //    worksheet[1].Cells[t_num, 3].Value = dataGridView2.Rows[i].Cells[2].Value.ToString();//"외경(mm)";
                //    worksheet[1].Cells[t_num, 4].Value = dataGridView2.Rows[i].Cells[3].Value.ToString();//"Average";
                //    worksheet[1].Cells[t_num, 5].Value = dataGridView2.Rows[i].Cells[4].Value.ToString();//"0.000";
                //    worksheet[1].Cells[t_num, 6].Value = dataGridView2.Rows[i].Cells[6].Value.ToString();//"0.000";
                //    worksheet[1].Cells[t_num, 7].Value = dataGridView2.Rows[i].Cells[7].Value.ToString();//"0.000";
                //    worksheet[1].Cells[t_num, 8].Value = dataGridView2.Rows[i].Cells[8].Value.ToString();//"0.000";
                //    worksheet[1].Cells[t_num, 9].Value = dataGridView2.Rows[i].Cells[13].Value.ToString();//"0.000";
                //    t_num++;
                //}

                worksheet[2].Cells[2, 1].Value = "Port Name";
                worksheet[2].Cells[2, 2].Value = "COM1";
                worksheet[2].Cells[3, 1].Value = "Baudrate";
                worksheet[2].Cells[3, 2].Value = "115200";
                worksheet[2].Cells[4, 1].Value = "Data Bits";
                worksheet[2].Cells[4, 2].Value = "8";
                worksheet[2].Cells[5, 1].Value = "Stop Bits";
                worksheet[2].Cells[5, 2].Value = "1";
                worksheet[2].Cells[6, 1].Value = "Parity";
                worksheet[2].Cells[6, 2].Value = "None";
                worksheet[2].Cells[7, 1].Value = "Receive Format";
                worksheet[2].Cells[7, 2].Value = "ASCII";
                worksheet[2].Cells[8, 1].Value = "Send Format";
                worksheet[2].Cells[8, 2].Value = "ASCII";


                for (int i = 0; i < 8; i++)
                {
                    worksheet[3].Cells[2 + 10 * i, 1].Value = "Camera Name";
                    worksheet[3].Cells[2 + 10 * i, 2].Value = "CAM" + i.ToString();
                    worksheet[3].Cells[3 + 10 * i, 1].Value = "Gain";
                    worksheet[3].Cells[3 + 10 * i, 2].Value = "1";
                    worksheet[3].Cells[4 + 10 * i, 1].Value = "Exposure Time";
                    worksheet[3].Cells[4 + 10 * i, 2].Value = "500";
                    worksheet[3].Cells[5 + 10 * i, 1].Value = "Width";
                    worksheet[3].Cells[5 + 10 * i, 2].Value = "1600";
                    worksheet[3].Cells[6 + 10 * i, 1].Value = "Height";
                    worksheet[3].Cells[6 + 10 * i, 2].Value = "1200";
                    worksheet[3].Cells[7 + 10 * i, 1].Value = "OffsetX";
                    worksheet[3].Cells[7 + 10 * i, 2].Value = "0";
                    worksheet[3].Cells[8 + 10 * i, 1].Value = "OffsetY";
                    worksheet[3].Cells[8 + 10 * i, 2].Value = "0";
                    worksheet[3].Cells[9 + 10 * i, 1].Value = "Res";
                    worksheet[3].Cells[9 + 10 * i, 2].Value = "1.000000";
                    worksheet[3].Cells[9 + 10 * i, 2].Value = "1.000000";
                    worksheet[3].Cells[10 + 10 * i, 1].Value = "Res";
                    worksheet[3].Cells[10 + 10 * i, 2].Value = "0";
                    worksheet[3].Cells[10 + 10 * i, 2].Value = "0";
                    worksheet[3].Cells[11 + 10 * i, 2].Value = "0";
                    worksheet[3].Cells[11 + 10 * i, 3].Value = "0";
                    worksheet[3].Cells[11 + 10 * i, 4].Value = "0";
                }

                worksheet[5].Cells[1, 1].Value = "알고리즘";
                worksheet[5].Cells[1, 2].Value = "찾는방법";
                worksheet[5].Cells[1, 3].Value = "L_Thres";
                worksheet[5].Cells[1, 4].Value = "H_Thres";
                worksheet[5].Cells[1, 5].Value = "Margin";

                t_num = 2;
                worksheet[5].Cells[t_num, 1].Value = "H0";
                worksheet[5].Cells[t_num, 2].Value = "2";
                worksheet[5].Cells[t_num, 3].Value = "50";
                worksheet[5].Cells[t_num, 4].Value = "150";
                worksheet[5].Cells[t_num, 5].Value = "10";
                t_num++;
                worksheet[5].Cells[t_num, 1].Value = "S0";
                worksheet[5].Cells[t_num, 2].Value = "1";
                worksheet[5].Cells[t_num, 3].Value = "50";
                worksheet[5].Cells[t_num, 4].Value = "150";
                worksheet[5].Cells[t_num, 5].Value = "10";
                t_num++;
                worksheet[5].Cells[t_num, 1].Value = "T0";
                worksheet[5].Cells[t_num, 2].Value = "1";
                worksheet[5].Cells[t_num, 3].Value = "50";
                worksheet[5].Cells[t_num, 4].Value = "150";
                worksheet[5].Cells[t_num, 5].Value = "10";


                // Setting some document properties
                package.Workbook.Properties.Title = "Setting Data";
                package.Workbook.Properties.Author = "LV";
                // set some extended property values
                package.Workbook.Properties.Company = "아이피에스시스텍";
                package.Save();
            }
        }

        delegate void MyDelegate();      //델리게이트 선언(크로스 쓰레드 해결하기 위한 용도)
        public bool t_Create_Save_Folders_Enable = false;
        public DateTime t_Create_Save_Folders_oldtime = new DateTime();
        public void Create_Save_Folders()
        {
            //if (!t_Create_Save_Folders_Enable)
            //{
            //    return;
            //}
            try
            {
                MyDelegate _dt = delegate()
                {

                    if (m_Model_Name == "")
                    {
                        m_Model_Name = "NoModel";
                        DebugLogger.Instance().LogRecord("Register the model.");
                        return;
                    }
                    string fn = "";
                    DateTime dt = DateTime.Now;
                    // MM_DD_YYYY_HH_MM_SS.LOG
                    fn += dt.Year.ToString("0000");
                    fn += "_" + dt.Month.ToString("00");
                    fn += "_" + dt.Day.ToString("00") + "";

                    if (m_Log_Save_Folder == "")
                    {
                        DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images");
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data");
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + m_Model_Name);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + m_Model_Name + "\\" + fn);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        for (int i = 0; i < m_Cam_Total_Num; i++)
                        {
                            int cam_num = i;
                            if (i == 0)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 1)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 2)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 3)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            //dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + m_Model_Name + "\\" + fn + "\\CAM" + i.ToString());
                            //// 폴더가 존재하지 않으면
                            //if (dir.Exists == false)
                            //{
                            //    // 새로 생성합니다.
                            //    dir.Create();
                            //}
                            if (m_Cam_Log_Use_Check[i] || LVApp.Instance().m_Config.SSF_Image_Save)
                            {
                                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name);
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn);
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString());
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\OK");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\NG");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\No Object");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                            }
                        }
                    }
                    else
                    {
                        DirectoryInfo dir = new DirectoryInfo(m_Log_Save_Folder);
                        if (dir.Exists == false)
                        {
                            return;
                        }
                        dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images");
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data");
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data\\" + m_Model_Name);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data\\" + m_Model_Name + "\\" + fn);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }

                        for (int i = 0; i < m_Cam_Total_Num; i++)
                        {
                            int cam_num = i;
                            if (i == 0)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 1)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 2)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 3)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            //dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data\\" + m_Model_Name + "\\" + fn + "\\CAM" + i.ToString());
                            //// 폴더가 존재하지 않으면
                            //if (dir.Exists == false)
                            //{
                            //    // 새로 생성합니다.
                            //    dir.Create();
                            //}

                            if (m_Cam_Log_Use_Check[i] || LVApp.Instance().m_Config.SSF_Image_Save)
                            {
                                //dir = new DirectoryInfo(m_Log_Save_Folder + "\\" + m_Model_Name);
                                //// 폴더가 존재하지 않으면
                                //if (dir.Exists == false)
                                //{
                                //    // 새로 생성합니다.
                                //    dir.Create();
                                //}
                                dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name);
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn);
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString());
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\OK");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" +  (cam_num).ToString() + "\\NG");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\No Object");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                            }
                        }
                    }

                    if (m_Log_Save_Folder2 != "")
                    {
                        DirectoryInfo dir = new DirectoryInfo(m_Log_Save_Folder2);
                        if (dir.Exists == false)
                        {
                            return;
                        }
                        dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }

                        for (int i = 0; i < m_Cam_Total_Num; i++)
                        {
                            int cam_num = i;
                            if (i == 0)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 1)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 2)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 3)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }

                            if (m_Cam_Log_Use_Check[i] || LVApp.Instance().m_Config.SSF_Image_Save)
                            {
                                //dir = new DirectoryInfo(m_Log_Save_Folder + "\\" + m_Model_Name);
                                //// 폴더가 존재하지 않으면
                                //if (dir.Exists == false)
                                //{
                                //    // 새로 생성합니다.
                                //    dir.Create();
                                //}
                                dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name);
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn);
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString());
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\OK");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\NG");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                                dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\No Object");
                                // 폴더가 존재하지 않으면
                                if (dir.Exists == false)
                                {
                                    // 새로 생성합니다.
                                    dir.Create();
                                }
                            }
                        }
                    }

                    if (m_Data_Save_Folder.Length > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(m_Data_Save_Folder);
                        if (dir.Exists == false)
                        {
                            return;
                        }
                        dir = new DirectoryInfo(m_Data_Save_Folder + "\\Data");
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(m_Data_Save_Folder + "\\Data\\" + m_Model_Name);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        dir = new DirectoryInfo(m_Data_Save_Folder + "\\Data\\" + m_Model_Name + "\\" + fn);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                    }
                    //t_Create_Save_Folders_Enable = false;
                    //DebugLogger.Instance().LogRecord("Folder created.");
                };
                LVApp.Instance().m_mainform.Invoke(_dt);
            }
            catch
            {
                //t_Create_Save_Folders_Enable = false;
                LVApp.Instance().m_mainform.add_Log("Folder create error!");
            }
        }

        public void Create_Save_Folders2()
        {
            try
            {
                if (m_Model_Name == "")
                {
                    m_Model_Name = "NoModel";
                    DebugLogger.Instance().LogRecord("Register the model.");
                    return;
                }
                string fn = "";
                DateTime dt = DateTime.Now;
                // MM_DD_YYYY_HH_MM_SS.LOG
                fn += dt.Year.ToString("0000");
                fn += "_" + dt.Month.ToString("00");
                fn += "_" + dt.Day.ToString("00") + "";

                if (m_Log_Save_Folder == "")
                {
                    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + m_Model_Name);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + m_Model_Name + "\\" + fn);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    for (int i = 0; i < m_Cam_Total_Num; i++)
                    {
                        if (m_Cam_Log_Use_Check[i] || LVApp.Instance().m_Config.SSF_Image_Save)
                        {
                            int cam_num = i;
                            if (i == 0)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 1)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 2)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 3)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString());
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\OK");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\NG");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\No Object");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                        }
                    }
                }
                else
                {
                    DirectoryInfo root_dir = new DirectoryInfo(m_Log_Save_Folder.Substring(0,3));
                    if (root_dir.Exists == false)
                    {
                        return;
                    }

                    DirectoryInfo dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images");

                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data\\" + m_Model_Name);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(m_Log_Save_Folder + "\\Data\\" + m_Model_Name + "\\" + fn);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    for (int i = 0; i < m_Cam_Total_Num; i++)
                    {
                        if (m_Cam_Log_Use_Check[i] || LVApp.Instance().m_Config.SSF_Image_Save)
                        {
                            int cam_num = i;
                            if (i == 0)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 1)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 2)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            else if (i == 3)
                            {
                                if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out cam_num))
                                {
                                }
                            }
                            //dir = new DirectoryInfo(m_Log_Save_Folder + "\\" + m_Model_Name);
                            //// 폴더가 존재하지 않으면
                            //if (dir.Exists == false)
                            //{
                            //    // 새로 생성합니다.
                            //    dir.Create();
                            //}
                            dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString());
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\OK");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\NG");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\No Object");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                        }
                    }
                }

                if (m_Log_Save_Folder2 != "")
                {
                    DirectoryInfo dir = new DirectoryInfo(m_Log_Save_Folder2);
                    if (dir.Exists == false)
                    {
                        return;
                    }
                    dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    for (int i = 0; i < m_Cam_Total_Num; i++)
                    {
                        int cam_num = i;
                        if (i == 0)
                        {
                            if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out cam_num))
                            {
                            }
                        }
                        else if (i == 1)
                        {
                            if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out cam_num))
                            {
                            }
                        }
                        else if (i == 2)
                        {
                            if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out cam_num))
                            {
                            }
                        }
                        else if (i == 3)
                        {
                            if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out cam_num))
                            {
                            }
                        }

                        if (m_Cam_Log_Use_Check[i] || LVApp.Instance().m_Config.SSF_Image_Save)
                        {
                            //dir = new DirectoryInfo(m_Log_Save_Folder + "\\" + m_Model_Name);
                            //// 폴더가 존재하지 않으면
                            //if (dir.Exists == false)
                            //{
                            //    // 새로 생성합니다.
                            //    dir.Create();
                            //}
                            dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString());
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\OK");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\NG");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                            dir = new DirectoryInfo(m_Log_Save_Folder2 + "\\" + m_Model_Name + "\\" + fn + "\\CAM" + (cam_num).ToString() + "\\No Object");
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                // 새로 생성합니다.
                                dir.Create();
                            }
                        }
                    }
                }

                if (m_Data_Save_Folder.Length > 0)
                {
                    DirectoryInfo dir = new DirectoryInfo(m_Data_Save_Folder);
                    if (dir.Exists == false)
                    {
                        return;
                    }
                    dir = new DirectoryInfo(m_Data_Save_Folder + "\\Data");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(m_Data_Save_Folder + "\\Data\\" + m_Model_Name);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(m_Data_Save_Folder + "\\Data\\" + m_Model_Name + "\\" + fn);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                }
                //DebugLogger.Instance().LogRecord("Folder created.");
            }
            catch
            {
            }
        }

        public int Judge_DataSet(int Cam_num)
        {
            int m_Ret = 40;
            try
            {
                DataSet DS = null;
                Disp_OKNG_List[Cam_num] = "";
                Disp_OKNG_List_CNT[Cam_num] = 0;
                //Disp_Error_List[Cam_num] = Disp_OK_List[Cam_num] = "";
                //Disp_Error_List_CNT[Cam_num] = Disp_OK_List_CNT[Cam_num] = 0;
                //CurrencyManager currencyManager0 = null;
                if (Cam_num == 0)
                {
                    //currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DataSource];
                    //currencyManager0.SuspendBinding();
                    //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                    DS = ds_DATA_0;
                }
                else if (Cam_num == 1)
                {
                    //currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DataSource];
                   // currencyManager0.SuspendBinding();
                    //LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ClearSelection();
                    DS = ds_DATA_1;
                }
                else if (Cam_num == 2)
                {
                    //currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DataSource];
                    //currencyManager0.SuspendBinding();
                    //LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ClearSelection();
                    DS = ds_DATA_2;
                }
                else if (Cam_num == 3)
                {
                    //currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DataSource];
                    //currencyManager0.SuspendBinding();
                    //LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ClearSelection();
                    DS = ds_DATA_3;
                }

                // DataSet 구성
                // DS.Tables[0] : Use유무[0], 알고리즘이름[1], 항목명[2], 클래스[3]
                // DS.Tables[1] : 알고리즘 이름[0], 측정방법[1], 보정값[2], 측정값[3], 판정방법[4]
                //                기준값[5], 하한값[6], 상한값[7], 양품평균[8], 불량평균[9]
                //                양품수[10], 불량수[11], 수율[12]
                bool[] Check_Class = new bool[4]; Check_Class[0] = true; Check_Class[1] = true; Check_Class[2] = true; Check_Class[3] = true;
                bool[] Judgement_List = new bool[DS.Tables[0].Rows.Count];
                bool ok_ng = true;
                int t_yield_cnt = 0;

                LVApp.Instance().m_mainform.ctr_NGLog1.t_Value[Cam_num] = "";
                for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                {
                    if (DS.Tables[0].Rows[i][0].ToString() == "True")
                    {
                        string m_judge_method = DS.Tables[1].Rows[i][4].ToString();
                        string m_class = DS.Tables[0].Rows[i][3].ToString();

                        double m_val = -1;
                        double m_min = 0;
                        double m_max = 0;
                        double OK_Avg = 0;
                        double NG_Avg = 0;
                        double OK_Cnt = 0;
                        double NG_Cnt = 0;
                        double.TryParse(DS.Tables[1].Rows[i][3].ToString(), out m_val);

                        if (DS.Tables[1].Rows[i][3] != null)
                        {
                            if (m_Bending_Check[Cam_num, i] && (DS.Tables[0].Rows[i][2].ToString().Contains("합산") || DS.Tables[0].Rows[i][2].ToString().ToUpper().Contains("SUM")))
                            {
                                m_Bending_count_tmp[i]++;
                                if (m_Bending_count[i] == m_Bending_count_tmp[i])
                                {
                                    m_val += m_Bending_value[i];
                                    DS.Tables[1].Rows[i][3] = m_val;
                                    m_Bending_value[i] = 0;
                                    m_Bending_count_tmp[i] = 0;
                                }
                                else
                                {
                                    Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + m_val.ToString() + "_OK" + "\r\n";
                                    Disp_OKNG_List_CNT[Cam_num]++;
                                    m_Bending_value[i] += m_val;
                                    Judgement_List[i] = true;
                                    if (neoTabWindow_MAIN_idx == 2 && neoTabWindow_INSP_SETTING_CAM_idx == Cam_num)
                                    {
                                        if (m_val > -1)
                                        {
                                            if (Cam_num == 0)
                                            {
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                            }
                                            else if (Cam_num == 1)
                                            {
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                            }
                                            else if (Cam_num == 2)
                                            {
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                            }
                                            else if (Cam_num == 3)
                                            {
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                                LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                        if (DS.Tables[1].Rows[i][6] != null)
                        {
                            m_min = Convert.ToDouble(DS.Tables[1].Rows[i][6]);
                        }
                        else
                        {
                            DS.Tables[1].Rows[i][6] = 0;
                        }
                        if (DS.Tables[1].Rows[i][7] != null)
                        {
                            m_max = Convert.ToDouble(DS.Tables[1].Rows[i][7]);
                        }
                        else
                        {
                            DS.Tables[1].Rows[i][7] = 0;
                        }
                        if (DS.Tables[1].Rows[i][8] != null)
                        {
                            OK_Avg = Convert.ToDouble(DS.Tables[1].Rows[i][8]);
                        }
                        if (DS.Tables[1].Rows[i][9] != null)
                        {
                            NG_Avg = Convert.ToDouble(DS.Tables[1].Rows[i][9]);
                        }
                        if (DS.Tables[1].Rows[i][10] != null)
                        {
                            OK_Cnt = Convert.ToDouble(DS.Tables[1].Rows[i][10]);
                        }
                        if (DS.Tables[1].Rows[i][11] != null)
                        {
                            NG_Cnt = Convert.ToDouble(DS.Tables[1].Rows[i][11]);
                        }
                        if (m_class == "CLASS 1")
                        {
                            Check_Class[0] = false;
                        }
                        else if (m_class == "CLASS 2")
                        {
                            Check_Class[1] = false;
                        }
                        else if (m_class == "CLASS 3")
                        {
                            Check_Class[2] = false;
                        }

                        if (Cam_num == 0)
                        {
                            LVApp.Instance().m_mainform.ctr_PLC1.CAM0_Value[i] = m_val;
                        }
                        else if (Cam_num == 1)
                        {
                            LVApp.Instance().m_mainform.ctr_PLC1.CAM1_Value[i] = m_val;
                        }
                        else if (Cam_num == 2)
                        {
                            LVApp.Instance().m_mainform.ctr_PLC1.CAM2_Value[i] = m_val;
                        }
                        else if (Cam_num == 3)
                        {
                            LVApp.Instance().m_mainform.ctr_PLC1.CAM3_Value[i] = m_val;
                        }

                        if (i == 1)
                        {
                            if (Cam_num == 1 && !LVApp.Instance().m_mainform.ctr_PLC1.CAM1_Updated)
                            {
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM1_ROI2_Value = m_val * 1000;
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM1_ROI2_Min = m_min * 1000;
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM1_ROI2_Max = m_max * 1000;
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM1_Updated = true;
                                LVApp.Instance().m_mainform.ctr_PLC1.Send_Data_MC();
                            }
                            if (Cam_num == 2 && !LVApp.Instance().m_mainform.ctr_PLC1.CAM2_Updated)
                            {
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM2_ROI2_Value = m_val * 1000;
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM2_ROI2_Min = m_min * 1000;
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM2_ROI2_Max = m_max * 1000;
                                LVApp.Instance().m_mainform.ctr_PLC1.CAM2_Updated = true;
                                LVApp.Instance().m_mainform.ctr_PLC1.Send_Data_MC();
                            }
                        }
                        if (m_judge_method == "Min.")
                        {
                            if (m_val >= m_min && m_val >= -1)
                            {
                                Judgement_List[i] = true;
                            }
                            else
                            {
                                Judgement_List[i] = false;
                            }
                        }
                        else if (m_judge_method == "Max.")
                        {
                            if (m_val <= m_max && m_val >= -1)
                            {
                                Judgement_List[i] = true;
                            }
                            else
                            {
                                Judgement_List[i] = false;
                            }
                        }
                        else if (m_judge_method == "Range")
                        {
                            if (m_val >= m_min && m_val <= m_max && m_val >= -1)
                            {
                                Judgement_List[i] = true;
                            }
                            else
                            {
                                Judgement_List[i] = false;
                            }
                        }
                        else if (m_judge_method == "Rev-Range")
                        {
                            if ((m_val <= m_min || m_val >= m_max) && m_val >= -1)
                            {
                                Judgement_List[i] = true;
                            }
                            else
                            {
                                Judgement_List[i] = false;
                            }
                        }

                        //if (m_val == -1)
                        //{
                        //    if (LVApp.Instance().m_Config.m_Judge_Priority == 3)
                        //    {
                        //        return -1;
                        //    }
                        //    Judgement_List[i] = false;
                        //    Check_Class[m_Judge_Priority] = false;
                        //}
                        if (m_val <= -1)
                        {
                            if (LVApp.Instance().m_Config.m_Judge_Priority <= 2)
                            {
                                return 10;
                            }
                            else if (LVApp.Instance().m_Config.m_Judge_Priority == 3)
                            {
                                return -1;
                            }
                            else if(LVApp.Instance().m_Config.m_Judge_Priority == 4)
                            {
                                return 40;
                            }
                            Judgement_List[i] = false;
                            Check_Class[m_Judge_Priority] = false;
                        }

                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Value[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + "^" + m_val.ToString() + "^" + Judgement_List[i].ToString() + "!";

                        if (!Judgement_List[i]) //불량이면
                        {
                            if (m_val >= -1)
                            {
                                if (NG_Cnt == 0)
                                {
                                    if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                    {
                                        NG_Avg += m_val;
                                    }
                                    NG_Cnt++;
                                }
                                else
                                {
                                    if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                    {
                                        NG_Avg = NG_Avg * NG_Cnt;
                                        NG_Avg += m_val;
                                    }
                                    NG_Cnt++;
                                }

                                if (m_class == "CLASS 1")
                                {
                                    Check_Class[0] = false;
                                }
                                else if (m_class == "CLASS 2")
                                {
                                    Check_Class[1] = false;
                                }
                                else if (m_class == "CLASS 3")
                                {
                                    Check_Class[2] = false;
                                }

                                if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                {
                                    DS.Tables[1].Rows[i][9] = Math.Round(NG_Avg / NG_Cnt, 4, MidpointRounding.AwayFromZero);
                                }
                                DS.Tables[1].Rows[i][11] = NG_Cnt;
                                //Disp_Error_List[Cam_num] += "R" + (i + 1).ToString("00") + ":" + m_val.ToString() + "\r\n";

                                if (DS.Tables[0].Rows[i][2].ToString().Length == 1)
                                {
                                    Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + m_val.ToString() + "_NG" + "\r\n";
                                }
                                else if (DS.Tables[0].Rows[i][2].ToString().Length > 1)
                                {
                                    if (DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                    {
                                        int t_AI_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                                        if (t_AI_CNT > 0)
                                        {
                                            for (int k = 0; k < t_AI_CNT; k++)
                                            {
                                                if (LVApp.Instance().m_Config.m_AIParam[Cam_num][k].ROI_IDX == i)
                                                {
                                                    Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result_Label + "_NG (" + LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result + ")" + "\r\n";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + m_val.ToString() + "_NG" + "\r\n";
                                    }
                                }
                                else
                                {
                                    Disp_OKNG_List[Cam_num] += "No name: " + m_val.ToString() + "_NG" + "\r\n";
                                }
                                Disp_OKNG_List_CNT[Cam_num]++;
                                //Disp_Error_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + m_val.ToString() + "\r\n";
                                //Disp_Error_List_CNT[Cam_num]++;
                                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                                {
                                    if (DS.Tables[0].Rows[i][2].ToString().Contains("조명") && ds_STATUS.Tables["AUTO STATUS"].Rows[0]["값"].ToString().Contains("정상"))
                                    {
                                        ds_STATUS.Tables["AUTO STATUS"].Rows[0]["값"] = "Error";
                                        //AutoClosingMessageBox.Show(Cam_num.ToString() + "번 조명을 확인하세요!", "ERROR", 700);
                                        LVApp.Instance().m_mainform.add_Log(Cam_num.ToString() + "번 조명을 확인하세요!");
                                    }
                                }
                                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                                {
                                    if (DS.Tables[0].Rows[i][2].ToString().Contains("Light") && ds_STATUS.Tables["AUTO STATUS"].Rows[0]["Value"].ToString().Contains("OK"))
                                    {
                                        ds_STATUS.Tables["AUTO STATUS"].Rows[0]["Value"] = "Error";
                                        //AutoClosingMessageBox.Show(Cam_num.ToString() + "번 조명을 확인하세요!", "ERROR", 700);
                                        LVApp.Instance().m_mainform.add_Log(Cam_num.ToString() + "'s light error. Check!");
                                    }
                                }
                            }
                            ok_ng = false;

                            if (neoTabWindow_MAIN_idx == 2 && neoTabWindow_INSP_SETTING_CAM_idx == Cam_num)
                            {
                                if (m_val >= -1)
                                {
                                    if (Cam_num == 0)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.ForeColor = Color.White;
                                    }
                                    else if (Cam_num == 1)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.ForeColor = Color.White;
                                    }
                                    else if (Cam_num == 2)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.ForeColor = Color.White;
                                    }
                                    else if (Cam_num == 3)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.BackColor = Color.Red;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.ForeColor = Color.White;
                                    }
                                }
                                else
                                {
                                    if (Cam_num == 0)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                    else if (Cam_num == 1)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                    else if (Cam_num == 2)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                    else if (Cam_num == 3)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_val >= -1)
                            {
                                if (OK_Cnt == 0)
                                {
                                    if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                    {
                                        OK_Avg = m_val;
                                    }
                                    OK_Cnt++;
                                }
                                else
                                {
                                    if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                    {
                                        OK_Avg = OK_Avg * OK_Cnt;
                                        OK_Avg += m_val;
                                    }
                                    OK_Cnt++;
                                }
                                if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                {
                                    DS.Tables[1].Rows[i][8] = Math.Round(OK_Avg / OK_Cnt, 4, MidpointRounding.AwayFromZero);
                                }
                                DS.Tables[1].Rows[i][10] = OK_Cnt;
                                //Disp_OK_List[Cam_num] += "R" + (i + 1).ToString("00") + ":" + m_val.ToString() + "\r\n";
                                if (DS.Tables[0].Rows[i][2].ToString().Length == 1)
                                {
                                    Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + m_val.ToString() + "_OK" + "\r\n";
                                }
                                else if (DS.Tables[0].Rows[i][2].ToString().Length > 1)
                                {
                                    if (DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                                    {
                                        int t_AI_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                                        if (t_AI_CNT > 0)
                                        {
                                            for (int k = 0; k < t_AI_CNT; k++)
                                            {
                                                if (LVApp.Instance().m_Config.m_AIParam[Cam_num][k].ROI_IDX == i)
                                                {
                                                    Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result_Label + "_OK (" + LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result + ")" + "\r\n";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Disp_OKNG_List[Cam_num] += DS.Tables[0].Rows[i][2].ToString() + ": " + m_val.ToString() + "_OK" + "\r\n";
                                    }
                                }
                                else
                                {
                                    Disp_OKNG_List[Cam_num] += "No name: " + m_val.ToString() + "_OK" + "\r\n";
                                }

                                Disp_OKNG_List_CNT[Cam_num]++;

                                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                                {
                                    if (DS.Tables[0].Rows[i][2].ToString().Contains("조명") && ds_STATUS.Tables["AUTO STATUS"].Rows[0]["값"].ToString().Contains("Error"))
                                    {
                                        ds_STATUS.Tables["AUTO STATUS"].Rows[0]["값"] = "정상";
                                    }
                                }
                                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                                {
                                    if (DS.Tables[0].Rows[i][2].ToString().Contains("Light") && ds_STATUS.Tables["AUTO STATUS"].Rows[0]["Value"].ToString().Contains("Error"))
                                    {
                                        ds_STATUS.Tables["AUTO STATUS"].Rows[0]["Value"] = "OK";
                                    }
                                }
                            }
                            if (neoTabWindow_MAIN_idx == 2 && neoTabWindow_INSP_SETTING_CAM_idx == Cam_num)
                            {
                                if (m_val >= -1)
                                {
                                    //if (Cam_num == 0)
                                    //{
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    //}
                                    //else if (Cam_num == 1)
                                    //{
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    //}
                                    //else if (Cam_num == 2)
                                    //{
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    //}
                                    //else if (Cam_num == 3)
                                    //{
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.BackColor = Color.Blue;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.ForeColor = Color.White;
                                    //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    //}
                                //}
                                //else
                                //{
                                    if (Cam_num == 0)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                    else if (Cam_num == 1)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                    else if (Cam_num == 2)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                    else if (Cam_num == 3)
                                    {
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.BackColor = Color.LightGray;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.BackColor = Color.LightBlue;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.BackColor = Color.LavenderBlush;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[10].Style.ForeColor = Color.Black;
                                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    }
                                }
                                //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[3].Style.BackColor = Color.Blue;
                            }
                        }

                        if ((OK_Cnt + NG_Cnt) != 0)
                        {
                            DS.Tables[1].Rows[i][12] = Math.Round(100 * OK_Cnt / (OK_Cnt + NG_Cnt), 1, MidpointRounding.AwayFromZero);
                            LVApp.Instance().m_Config.ds_YIELD.Tables[Cam_num].Rows[t_yield_cnt][1] = NG_Cnt;//불량수
                            LVApp.Instance().m_Config.ds_YIELD.Tables[Cam_num].Rows[t_yield_cnt][2] = DS.Tables[1].Rows[i][12];//수율
                            if (!DS.Tables[0].Rows[i][2].ToString().Substring(0, 2).ToUpper().Contains("AI"))
                            {
                                LVApp.Instance().m_Config.ds_YIELD.Tables[Cam_num].Rows[t_yield_cnt][3] = DS.Tables[1].Rows[i][8];//양품평균
                            }
                        }
                        t_yield_cnt++;
                    }
                    else
                    {
                        if (Cam_num == 0)
                        {
                            m_GraphData0[i].use = false;
                        }
                        else if (Cam_num == 1)
                        {
                            m_GraphData1[i].use = false;
                        }
                        else if (Cam_num == 2)
                        {
                            m_GraphData2[i].use = false;
                        }
                        else if (Cam_num == 3)
                        {
                            m_GraphData3[i].use = false;
                        }
                        //ok_ng = true;
                    }

                    if (Graph_SW[Cam_num].ElapsedMilliseconds > 1000 * m_Graph_Update_sec)
                    {
                        LVApp.Instance().m_mainform.ctr_Graph1.t_update_check = true;
                        if (DS.Tables[0].Rows[i][0].ToString() == "True")
                        {
                            if (Cam_num == 0)
                            {
                                m_GraphData0[i].name = DS.Tables[0].Rows[i][2].ToString();
                                m_GraphData0[i].use = true;
                                m_GraphData0[i].list.Add((double)new XDate(DateTime.Now), double.Parse(DS.Tables[1].Rows[i][12].ToString()));
                                if (m_GraphData0[i].list.Count > 10000)
                                {
                                    m_GraphData0[i].list.RemoveAt(0);
                                }
                            }
                            else if (Cam_num == 1)
                            {
                                m_GraphData1[i].name = DS.Tables[0].Rows[i][2].ToString();
                                m_GraphData1[i].use = true;
                                m_GraphData1[i].list.Add((double)new XDate(DateTime.Now), double.Parse(DS.Tables[1].Rows[i][12].ToString()));
                                if (m_GraphData1[i].list.Count > 10000)
                                {
                                    m_GraphData1[i].list.RemoveAt(0);
                                }
                            }
                            else if (Cam_num == 2)
                            {
                                m_GraphData2[i].name = DS.Tables[0].Rows[i][2].ToString();
                                m_GraphData2[i].use = true;
                                m_GraphData2[i].list.Add((double)new XDate(DateTime.Now), double.Parse(DS.Tables[1].Rows[i][12].ToString()));
                                if (m_GraphData2[i].list.Count > 10000)
                                {
                                    m_GraphData2[i].list.RemoveAt(0);
                                }

                            }
                            else if (Cam_num == 3)
                            {
                                m_GraphData3[i].name = DS.Tables[0].Rows[i][2].ToString();
                                m_GraphData3[i].use = true;
                                m_GraphData3[i].list.Add((double)new XDate(DateTime.Now), double.Parse(DS.Tables[1].Rows[i][12].ToString()));
                                if (m_GraphData3[i].list.Count > 10000)
                                {
                                    m_GraphData3[i].list.RemoveAt(0);
                                }

                            }
                        }
                        if (i >= DS.Tables[0].Rows.Count - 1)
                        {
                            Graph_SW[Cam_num].Reset();
                            Graph_SW[Cam_num].Start();
                        }
                        LVApp.Instance().m_mainform.ctr_Graph1.t_update_check = false;
                    }
                }

                LVApp.Instance().m_mainform.ctr_PLC1.CAM_Value_Updated[Cam_num] = true;
                //LVApp.Instance().m_mainform.ctr_PLC1.Data_MC_Tx();

                if (!ok_ng)
                {
                    if (!Check_Class[2])
                    {
                        m_Ret = 30;
                    }
                    else if (!Check_Class[1] && Check_Class[2])
                    {
                        m_Ret = 20;
                    }
                    else if (!Check_Class[0] && Check_Class[1] && Check_Class[2])
                    {
                        m_Ret = 10;
                    }
                }
                else
                {
                    m_Ret = 40;
                    //m_Ret = 8 + (Cam_num * 10 + 40);
                }
                //currencyManager0.ResumeBinding();
                //if (m_Data_Log_Use_Check)
                //{
                //    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Logs");
                //    // 폴더가 존재하지 않으면
                //    if (dir.Exists == false)
                //    {
                //        // 새로 생성합니다.
                //        dir.Create();
                //    }

                //    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                //    // 폴더가 존재하지 않으면
                //    if (dir.Exists == false)
                //    {
                //        // 새로 생성합니다.
                //        dir.Create();
                //    }

                //    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd"));
                //    // 폴더가 존재하지 않으면
                //    if (dir.Exists == false)
                //    {
                //        // 새로 생성합니다.
                //        dir.Create();
                //    }
                //    if (m_Ret == 40)
                //    {
                //        string log_file = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM" + Cam_num.ToString("0") + "_" + DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK.csv";
                //        DS.Tables[1].WriteToCsvFile(log_file);
                //    }
                //    else
                //    {
                //        string log_file = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\CAM" + Cam_num.ToString("0") + "_" + DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NG.csv";
                //        DS.Tables[1].WriteToCsvFile(log_file);
                //    }
                //}
            }
            catch
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("[Error] CAM" + Cam_num.ToString() + "의 판정 설정의 입력값을 확인하세요!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("[Error] CAM" + Cam_num.ToString() + " Please check judgement data!");
                }
                return 10;
            }

            return m_Ret;
        }

        bool t_Save_Judge_Data_flag = false;
        public void Save_Judge_Data()
        {
            if (t_Save_Judge_Data_flag)
            {
                return;
            }
            t_Save_Judge_Data_flag = true;
            if (m_Model_Name == "")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                t_Save_Judge_Data_flag = false;
                return;
            }

            try
            {
                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + m_Model_Name + "\\" + m_Model_Name + ".xlsx");
                if (!newFile.Exists)
                {
                    t_Save_Judge_Data_flag = false;
                    return;
                }
                CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DataSource];
                currencyManager0.SuspendBinding();
                CurrencyManager currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DataSource];
                currencyManager1.SuspendBinding();
                CurrencyManager currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DataSource];
                currencyManager2.SuspendBinding();
                CurrencyManager currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DataSource];
                currencyManager3.SuspendBinding();

                for (int i = 0; i < 40; i++)
                {
                    m_Bending_Check[0,i] = m_Bending_Check[1,i] = m_Bending_Check[2,i] = m_Bending_Check[3,i] = false;
                    m_Bending_count[i] = 0;
                    m_Bending_count_tmp[i] = 0;
                }
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[2];
                    //dataGridView_Setting_0
                    //dataGridView_Setting_Value_0
                    // 1번 알고리즘 관련 데이터 저장
                    int T_Cnt = 0;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_0.RowCount; i++)
                    {
                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[0];
                        worksheet.Cells[i + 2, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                        if (i <= 0)
                        {
                            worksheet.Cells[i + 2, 1].Value = "1";
                            LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[0].Value = true;
                        }
                        worksheet.Cells[i + 2, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[1].Value.ToString();
                        worksheet.Cells[i + 2, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[2].Value.ToString();
                        if (LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[2].Value.ToString().Contains("합산") || LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[2].Value.ToString().ToUpper().Contains("SUM"))
                        {
                            m_Bending_count[i]++;
                            m_Bending_Check[0,i] = true;
                        }
                        if (i < LVApp.Instance().m_mainform.dataGridView_Setting_0.RowCount)
                        {
                            if (LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Items[i + 1].ToString() != LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[2].Value.ToString())
                            {
                                LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Items[i + 1] = LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[2].Value.ToString();
                            }
                        }
                        DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[3];
                        if (comboCell.Value == null)
                        {
                            worksheet.Cells[i + 2, 4].Value = "CLASS 1";
                        }
                        else
                        {
                            worksheet.Cells[i + 2, 4].Value = comboCell.Value.ToString();
                        }
                        T_Cnt++;
                    }
                    int start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.RowCount; i++)
                    {
                        worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[0].Value.ToString();
                        DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[1];
                        worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                        DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[4];
                        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                        T_Cnt++;
                    }

                    // 2번 알고리즘 관련 데이터 저장
                    start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_1.RowCount; i++)
                    {
                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[0];
                        worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                        if (i <= 0)
                        {
                            worksheet.Cells[i + 2 + start_num, 1].Value = "1";
                            LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[0].Value = true;
                        }
                        worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[1].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[2].Value.ToString();
                        if (LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[2].Value.ToString().Contains("합산") || LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[2].Value.ToString().ToUpper().Contains("SUM"))
                        {
                            m_Bending_count[i]++;
                            m_Bending_Check[1, i] = true;
                        }
                        if (i < LVApp.Instance().m_mainform.dataGridView_Setting_1.RowCount)
                        {
                            if (LVApp.Instance().m_mainform.ctr_ROI2.listBox1.Items[i + 1].ToString() != LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[2].Value.ToString())
                            {
                                LVApp.Instance().m_mainform.ctr_ROI2.listBox1.Items[i + 1] = LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[2].Value.ToString();
                            }
                        }
                        DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[3];
                        if (comboCell.Value == null)
                        {
                            worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                        }
                        else
                        {
                            worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                        }
                        T_Cnt++;
                    }
                    start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.RowCount; i++)
                    {
                        worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[0].Value.ToString();
                        DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[1];
                        worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[2].Value.ToString();
                        DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[4];
                        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[5].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[6].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Cells[7].Value.ToString();
                        T_Cnt++;
                    }
                    // 3번 알고리즘 관련 데이터 저장
                    start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_2.RowCount; i++)
                    {
                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[0];
                        worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                        if (i <= 0)
                        {
                            worksheet.Cells[i + 2 + start_num, 1].Value = "1";
                            LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[0].Value = true;
                        }
                        worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[1].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[2].Value.ToString();
                        if (LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[2].Value.ToString().Contains("합산") || LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[2].Value.ToString().ToUpper().Contains("SUM"))
                        {
                            m_Bending_count[i]++;
                            m_Bending_Check[2, i] = true;
                        }
                        if (i < LVApp.Instance().m_mainform.dataGridView_Setting_2.RowCount)
                        {
                            if (LVApp.Instance().m_mainform.ctr_ROI3.listBox1.Items[i + 1].ToString() != LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[2].Value.ToString())
                            {
                                LVApp.Instance().m_mainform.ctr_ROI3.listBox1.Items[i + 1] = LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[2].Value.ToString();
                            }
                        }
                        DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Cells[3];
                        if (comboCell.Value == null)
                        {
                            worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                        }
                        else
                        {
                            worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                        }
                        T_Cnt++;
                    }
                    start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.RowCount; i++)
                    {
                        worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[0].Value.ToString();
                        DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[1];
                        worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[2].Value.ToString();
                        DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[4];
                        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[5].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[6].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Cells[7].Value.ToString();
                        T_Cnt++;
                    }

                    // 4번 알고리즘 관련 데이터 저장
                    start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_3.RowCount; i++)
                    {
                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[0];
                        worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                        if (i <= 0)
                        {
                            worksheet.Cells[i + 2 + start_num, 1].Value = "1";
                            LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[0].Value = true;
                        }
                        worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[1].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[2].Value.ToString();
                        if (LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[2].Value.ToString().Contains("합산") || LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[2].Value.ToString().ToUpper().Contains("SUM"))
                        {
                            m_Bending_count[i]++;
                            m_Bending_Check[3, i] = true;
                        }
                        if (i < LVApp.Instance().m_mainform.dataGridView_Setting_3.RowCount)
                        {
                            if (LVApp.Instance().m_mainform.ctr_ROI4.listBox1.Items[i + 1].ToString() != LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[2].Value.ToString())
                            {
                                LVApp.Instance().m_mainform.ctr_ROI4.listBox1.Items[i + 1] = LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[2].Value.ToString();
                            }
                        }
                        DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Cells[3];
                        if (comboCell.Value == null)
                        {
                            worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                        }
                        else
                        {
                            worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                        }
                        T_Cnt++;
                    }
                    start_num = T_Cnt;
                    for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.RowCount; i++)
                    {
                        worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[0].Value.ToString();
                        DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[1];
                        worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[2].Value.ToString();
                        DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[4];
                        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[5].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[6].Value.ToString();
                        worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Cells[7].Value.ToString();
                        T_Cnt++;
                    }

                    //// 5번 알고리즘 관련 데이터 저장
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_4.RowCount; i++)
                    //{
                    //    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_4.Rows[i].Cells[0];
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_4.Rows[i].Cells[1].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_4.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_4.Rows[i].Cells[3];
                    //    if (comboCell.Value == null)
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                    //    }
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.RowCount; i++)
                    //{
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[0].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[1];
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[4];
                    //    worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[5].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[6].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}

                    //// 6번 알고리즘 관련 데이터 저장
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_5.RowCount; i++)
                    //{
                    //    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_5.Rows[i].Cells[0];
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_5.Rows[i].Cells[1].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_5.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_5.Rows[i].Cells[3];
                    //    if (comboCell.Value == null)
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                    //    }
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.RowCount; i++)
                    //{
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[0].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[1];
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[4];
                    //    worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[5].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[6].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}

                    //// 7번 알고리즘 관련 데이터 저장
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_6.RowCount; i++)
                    //{
                    //    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_6.Rows[i].Cells[0];
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_6.Rows[i].Cells[1].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_6.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_6.Rows[i].Cells[3];
                    //    if (comboCell.Value == null)
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                    //    }
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.RowCount; i++)
                    //{
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[0].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[1];
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[4];
                    //    worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[5].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[6].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}

                    //// 8번 알고리즘 관련 데이터 저장
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_7.RowCount; i++)
                    //{
                    //    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_7.Rows[i].Cells[0];
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = Convert.ToBoolean(chk.Value) == true ? "1" : "0";
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = LVApp.Instance().m_mainform.dataGridView_Setting_7.Rows[i].Cells[1].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_7.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_7.Rows[i].Cells[3];
                    //    if (comboCell.Value == null)
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = "CLASS 1";
                    //    }
                    //    else
                    //    {
                    //        worksheet.Cells[i + 2 + start_num, 4].Value = comboCell.Value.ToString();
                    //    }
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.RowCount; i++)
                    //{
                    //    worksheet.Cells[i + 2 + start_num, 1].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[0].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell1 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[1];
                    //    worksheet.Cells[i + 2 + start_num, 2].Value = comboCell1.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 3].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[2].Value.ToString();
                    //    DataGridViewComboBoxCell comboCell2 = (DataGridViewComboBoxCell)LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[4];
                    //    worksheet.Cells[i + 2 + start_num, 4].Value = comboCell2.Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 5].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[5].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 6].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[6].Value.ToString();
                    //    worksheet.Cells[i + 2 + start_num, 7].Value = LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //worksheet.Cells[100, 2].Value = m_Judge_Priority.ToString();

                    //worksheet.Cells[110, 2].Value = textBox_PARAMETER_0.Text;
                    //worksheet.Cells[111, 2].Value = textBox_PARAMETER_1.Text;
                    //worksheet.Cells[112, 2].Value = textBox_PARAMETER_2.Text;
                    //worksheet.Cells[113, 2].Value = textBox_PARAMETER_3.Text;
                    //worksheet.Cells[114, 2].Value = textBox_PARAMETER_4.Text;
                    //worksheet.Cells[115, 2].Value = textBox_PARAMETER_5.Text;
                    //worksheet.Cells[116, 2].Value = textBox_PARAMETER_6.Text;
                    //worksheet.Cells[117, 2].Value = textBox_PARAMETER_7.Text;
                    //worksheet.Cells[118, 2].Value = textBox_PARAMETER_8.Text;
                    //worksheet.Cells[119, 2].Value = textBox_PARAMETER_9.Text;
                    //worksheet.Cells[120, 2].Value = textBox_PARAMETER_10.Text;
                    //worksheet.Cells[121, 2].Value = textBox_PARAMETER_11.Text;
                    //worksheet.Cells[122, 2].Value = textBox_PARAMETER_12.Text;
                    //worksheet.Cells[123, 2].Value = textBox_PARAMETER_13.Text;
                    //worksheet.Cells[124, 2].Value = textBox_PARAMETER_14.Text;
                    //worksheet.Cells[125, 2].Value = textBox_PARAMETER_15.Text;
                    //worksheet.Cells[126, 2].Value = textBox_PARAMETER_16.Text;
                    //worksheet.Cells[127, 2].Value = textBox_PARAMETER_17.Text;
                    //worksheet.Cells[128, 2].Value = textBox_PARAMETER_18.Text;
                    //worksheet.Cells[129, 2].Value = textBox_PARAMETER_19.Text;
                    //worksheet.Cells[130, 2].Value = textBox_PARAMETER_20.Text;
                    //worksheet.Cells[131, 2].Value = checkBox_VIEW.Checked == true ? "1" : "0";
                    //worksheet.Cells[132, 2].Value = textBox_PARAMETER_22.Text;
                    //worksheet.Cells[133, 2].Value = textBox_PARAMETER_23.Text;
                    //worksheet.Cells[134, 2].Value = textBox_PARAMETER_24.Text;
                    //worksheet.Cells[135, 2].Value = textBox_PARAMETER_25.Text;
                    //worksheet.Cells[136, 2].Value = textBox_PARAMETER_26.Text;
                    //worksheet.Cells[137, 2].Value = textBox_PARAMETER_27.Text;
                    package.Save();
                }

                currencyManager0.ResumeBinding();
                currencyManager1.ResumeBinding();
                currencyManager2.ResumeBinding();
                currencyManager3.ResumeBinding();
                if (LVApp.Instance().m_mainform.Force_close)
                {
                    //LVApp.Instance().m_mainform.ctr_Parameters1.Set_Parameters();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        LVApp.Instance().m_mainform.add_Log("판정 설정 불러오기 완료.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        LVApp.Instance().m_mainform.add_Log("Judgement Setting Loaded.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        LVApp.Instance().m_mainform.add_Log("完成导入判定设置.");
                    }
                    //DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup");
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

                    //string sourcePath = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name;
                    //string targetPath = LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name;
                    //if (System.IO.Directory.Exists(sourcePath))
                    //{
                    //    LVApp.Instance().m_mainform.ctr_Model1.copyDirectory(sourcePath, targetPath);
                    //}
                }
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_STATUS.Tables[1], "Status");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_0.Tables[0], "CAM0_setting_tp");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_1.Tables[0], "CAM1_setting_tp");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_2.Tables[0], "CAM2_setting_tp");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_3.Tables[0], "CAM3_setting_tp");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_0.Tables[1], "CAM0_setting_bt");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_1.Tables[1], "CAM1_setting_bt");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_2.Tables[1], "CAM2_setting_bt");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_3.Tables[1], "CAM3_setting_bt");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_0.Tables[2], "CAM0_ROI");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_1.Tables[2], "CAM1_ROI");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_2.Tables[2], "CAM2_ROI");
                LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_3.Tables[2], "CAM3_ROI");
                t_Save_Judge_Data_flag = false;
            }
            finally
            {
                t_Save_Judge_Data_flag = false;
            }
        }

        public void Load_Judge_Data()
        {
            if (m_Model_Name == "")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                return;
            }

            try
            {
                for (int i = 0; i < 40; i++)
                {
                    m_Bending_Check[0,i] = m_Bending_Check[1,i] = m_Bending_Check[2,i] = m_Bending_Check[3,i] = false;
                    m_Bending_count[i] = 0;
                    m_Bending_count_tmp[i] = 0;
                }
                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + m_Model_Name + "\\" + m_Model_Name + ".xlsx");
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[2];

                    // 1번 알고리즘 로드
                    int T_Cnt = 0; int start_num = 0;
                    if (m_Cam_Total_Num >= 1)
                    {
                        LVApp.Instance().m_mainform.dataGridView_Setting_0.ClearSelection();
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                        //LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[0].Selected = true;
                        //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[0].Selected = true;
                        CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_0.DataSource];
                        currencyManager0.SuspendBinding();
                        //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Enabled = true;
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_0.RowCount; i++)
                        {
                            ds_DATA_0.Tables[0].Rows[i][0] = worksheet.Cells[2 + i, 1].Value.ToString() == "0" ? false : true;
                            //LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Visible = true;
                            //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Visible = true;
                            //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                            //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                            if (i >= 0)
                            {
                                if (worksheet.Cells[2 + i, 1].Value.ToString() == "0")
                                {
                                    if (LVApp.Instance().m_mainform.m_Start_Check)
                                    {
                                        //if (i >= 1)
                                        //{
                                            LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Unchecked);
                                            LVApp.Instance().m_Config.Cam0_rect[i].mUse = false;
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                        //    LVApp.Instance().m_Config.Cam0_rect[i].mUse = true;
                                        //}

                                        //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Unchecked);
                                    }
                                }
                                else
                                {
                                    LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                    LVApp.Instance().m_Config.Cam0_rect[i].mUse = true;
                                    //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                }
                            }
                            //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 0;
                            ds_DATA_0.Tables[0].Rows[i][1] = worksheet.Cells[2 + i, 2].Value.ToString();
                            ds_DATA_0.Tables[0].Rows[i][2] = worksheet.Cells[2 + i, 3].Value.ToString();

                            if (ds_DATA_0.Tables[0].Rows[i][2].ToString().Contains("합산") || ds_DATA_0.Tables[0].Rows[i][2].ToString().ToLower().Contains("sum"))
                            {
                                m_Bending_count[i]++;
                                m_Bending_Check[0,i] = true;
                            }
                            if (i < LVApp.Instance().m_mainform.dataGridView_Setting_0.RowCount)
                            {
                                if (LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Items[i + 1].ToString() != worksheet.Cells[2 + i, 3].Value.ToString())
                                {
                                    LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Items[i + 1] = worksheet.Cells[2 + i, 3].Value.ToString();
                                }
                            }
                            ds_DATA_0.Tables[0].Rows[i][3] = worksheet.Cells[2 + i, 4].Value.ToString();
                            T_Cnt++;
                        }
                        //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Refresh();
                        //LVApp.Instance().m_mainform.dataGridView_Setting_0.Refresh();
                        //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Refresh();
                        start_num = T_Cnt;
                        currencyManager0.ResumeBinding();
                        currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DataSource];
                        currencyManager0.SuspendBinding();
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.RowCount; i++)
                        {
                            ds_DATA_0.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                            ds_DATA_0.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                            ds_DATA_0.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                            ds_DATA_0.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                            ds_DATA_0.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                            ds_DATA_0.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                            ds_DATA_0.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                            T_Cnt++;
                        }
                        currencyManager0.ResumeBinding();
                        //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 0;
                        //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.Refresh();
                    }
                    if (m_Cam_Total_Num >= 2)
                    {
                        LVApp.Instance().m_mainform.dataGridView_Setting_1.ClearSelection();
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.ClearSelection();
                        //LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[0].Selected = true;
                        //LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[0].Selected = true;
                        CurrencyManager currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_1.DataSource];
                        currencyManager1.SuspendBinding();
                        // 2번 알고리즘 로드
                        start_num = T_Cnt;
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_1.RowCount; i++)
                        {
                            ds_DATA_1.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                            if (i >= 0)
                            {
                                if (worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0")
                                {
                                    if (LVApp.Instance().m_mainform.m_Start_Check)
                                    {
                                        //if (i >= 1)
                                        //{
                                            LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SetItemCheckState(i + 1, CheckState.Unchecked);
                                            LVApp.Instance().m_Config.Cam1_rect[i].mUse = false;
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                        //    LVApp.Instance().m_Config.Cam1_rect[i].mUse = true;
                                        //}

                                    }
                                }
                                else
                                {
                                    LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                    LVApp.Instance().m_Config.Cam1_rect[i].mUse = true;
                                }
                            }

                            ds_DATA_1.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                            ds_DATA_1.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                            if (ds_DATA_1.Tables[0].Rows[i][2].ToString().Contains("합산") || ds_DATA_1.Tables[0].Rows[i][2].ToString().ToLower().Contains("sum"))
                            {
                                m_Bending_count[i]++;
                                m_Bending_Check[1, i] = true;
                            }
                            if (i < LVApp.Instance().m_mainform.dataGridView_Setting_1.RowCount)
                            {
                                if (LVApp.Instance().m_mainform.ctr_ROI2.listBox1.Items[i + 1].ToString() != worksheet.Cells[2 + i + start_num, 3].Value.ToString())
                                {
                                    LVApp.Instance().m_mainform.ctr_ROI2.listBox1.Items[i + 1] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                                }
                            }
                            ds_DATA_1.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                            T_Cnt++;
                        }
                        start_num = T_Cnt;
                        currencyManager1.ResumeBinding();
                        currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DataSource];
                        currencyManager1.SuspendBinding();
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.RowCount; i++)
                        {
                            ds_DATA_1.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                            ds_DATA_1.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                            ds_DATA_1.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                            ds_DATA_1.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                            ds_DATA_1.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                            ds_DATA_1.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                            ds_DATA_1.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                            T_Cnt++;
                        }
                        currencyManager1.ResumeBinding();
                        //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = 0;
                        //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.Refresh();
                    }
                    if (m_Cam_Total_Num >= 3)
                    {
                        LVApp.Instance().m_mainform.dataGridView_Setting_2.ClearSelection();
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.ClearSelection();
                        //LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[0].Selected = true;
                        //LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[0].Selected = true;
                        // 3번 알고리즘 로드
                        start_num = T_Cnt;
                        CurrencyManager currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_2.DataSource];
                        currencyManager2.SuspendBinding();
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_2.RowCount; i++)
                        {
                            ds_DATA_2.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                            if (i >= 0)
                            {
                                if (worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0")
                                {
                                    if (LVApp.Instance().m_mainform.m_Start_Check)
                                    {
                                        //if (i >= 1)
                                        //{
                                            LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SetItemCheckState(i + 1, CheckState.Unchecked);
                                            LVApp.Instance().m_Config.Cam2_rect[i].mUse = false;
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                        //    LVApp.Instance().m_Config.Cam2_rect[i].mUse = true;
                                        //}

                                    }
                                }
                                else
                                {
                                    LVApp.Instance().m_mainform.dataGridView_Setting_2.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                    LVApp.Instance().m_Config.Cam2_rect[i].mUse = true;
                                }
                            }

                            ds_DATA_2.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                            ds_DATA_2.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                            if (ds_DATA_2.Tables[0].Rows[i][2].ToString().Contains("합산") || ds_DATA_2.Tables[0].Rows[i][2].ToString().ToLower().Contains("sum"))
                            {
                                m_Bending_count[i]++;
                                m_Bending_Check[2, i] = true;
                            }
                            if (i < LVApp.Instance().m_mainform.dataGridView_Setting_2.RowCount)
                            {
                                if (LVApp.Instance().m_mainform.ctr_ROI3.listBox1.Items[i + 1].ToString() != worksheet.Cells[2 + i + start_num, 3].Value.ToString())
                                {
                                    LVApp.Instance().m_mainform.ctr_ROI3.listBox1.Items[i + 1] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                                }
                            }
                            ds_DATA_2.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                            T_Cnt++;
                        }
                        currencyManager2.ResumeBinding();
                        currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DataSource];
                        currencyManager2.SuspendBinding();
                        start_num = T_Cnt;
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.RowCount; i++)
                        {
                            ds_DATA_2.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                            ds_DATA_2.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                            ds_DATA_2.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                            ds_DATA_2.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                            ds_DATA_2.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                            ds_DATA_2.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                            ds_DATA_2.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                            T_Cnt++;
                        }
                        currencyManager2.ResumeBinding();
                        //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = 0;
                        //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.Refresh();
                    }
                    if (m_Cam_Total_Num >= 4)
                    {
                        LVApp.Instance().m_mainform.dataGridView_Setting_3.ClearSelection();
                        LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.ClearSelection();
                        //LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[0].Selected = true;
                        //LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[0].Selected = true;
                        CurrencyManager currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_3.DataSource];
                        currencyManager3.SuspendBinding();
                        // 3번 알고리즘 로드
                        start_num = T_Cnt;
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_3.RowCount; i++)
                        {
                            ds_DATA_3.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                            if (i >= 0)
                            {
                                if (worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0")
                                {
                                    if (LVApp.Instance().m_mainform.m_Start_Check)
                                    {
                                        //if (i >= 1)
                                        //{
                                            LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Visible = false;
                                            LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SetItemCheckState(i + 1, CheckState.Unchecked);
                                            LVApp.Instance().m_Config.Cam3_rect[i].mUse = false;
                                        //}
                                        //else
                                        //{
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Visible = true;
                                        //    LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                        //    LVApp.Instance().m_Config.Cam3_rect[i].mUse = true;
                                        //}
                                    }
                                }
                                else
                                {
                                    LVApp.Instance().m_mainform.dataGridView_Setting_3.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Rows[i].Visible = true;
                                    LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SetItemCheckState(i + 1, CheckState.Checked);
                                    LVApp.Instance().m_Config.Cam3_rect[i].mUse = true;
                                }
                            }

                            ds_DATA_3.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                            ds_DATA_3.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                            if (ds_DATA_3.Tables[0].Rows[i][2].ToString().Contains("합산") || ds_DATA_3.Tables[0].Rows[i][2].ToString().ToLower().Contains("sum"))
                            {
                                m_Bending_count[i]++;
                                m_Bending_Check[3, i] = true;
                            }
                            if (i < LVApp.Instance().m_mainform.dataGridView_Setting_3.RowCount)
                            {
                                if (LVApp.Instance().m_mainform.ctr_ROI4.listBox1.Items[i + 1].ToString() != worksheet.Cells[2 + i + start_num, 3].Value.ToString())
                                {
                                    LVApp.Instance().m_mainform.ctr_ROI4.listBox1.Items[i + 1] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                                }
                            }
                            ds_DATA_3.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                            T_Cnt++;
                        }
                        currencyManager3.ResumeBinding();
                        currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DataSource];
                        currencyManager3.SuspendBinding();
                        start_num = T_Cnt;
                        for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.RowCount; i++)
                        {
                            ds_DATA_3.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                            ds_DATA_3.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                            ds_DATA_3.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                            ds_DATA_3.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                            ds_DATA_3.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                            ds_DATA_3.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                            ds_DATA_3.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                            T_Cnt++;
                        }
                        currencyManager3.ResumeBinding();
                        //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = 0;
                        //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.Refresh();
                    }
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Columns[0].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Columns[0].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.Columns[0].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.Columns[0].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_0.Columns[1].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_1.Columns[1].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_2.Columns[1].ReadOnly = true;
                    LVApp.Instance().m_mainform.dataGridView_Setting_3.Columns[1].ReadOnly = true;
                    //// 4번 알고리즘 로드
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_3.RowCount; i++)
                    //{
                    //    ds_DATA_3.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                    //    ds_DATA_3.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                    //    ds_DATA_3.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                    //    ds_DATA_3.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.RowCount; i++)
                    //{
                    //    ds_DATA_3.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                    //    ds_DATA_3.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                    //    ds_DATA_3.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                    //    ds_DATA_3.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                    //    ds_DATA_3.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                    //    ds_DATA_3.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                    //    ds_DATA_3.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //// 5번 알고리즘 로드
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_4.RowCount; i++)
                    //{
                    //    ds_DATA_4.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                    //    ds_DATA_4.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                    //    ds_DATA_4.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                    //    ds_DATA_4.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_4.RowCount; i++)
                    //{
                    //    ds_DATA_4.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                    //    ds_DATA_4.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                    //    ds_DATA_4.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                    //    ds_DATA_4.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                    //    ds_DATA_4.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                    //    ds_DATA_4.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                    //    ds_DATA_4.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //// 6번 알고리즘 로드
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_5.RowCount; i++)
                    //{
                    //    ds_DATA_5.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                    //    ds_DATA_5.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                    //    ds_DATA_5.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                    //    ds_DATA_5.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_5.RowCount; i++)
                    //{
                    //    ds_DATA_5.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                    //    ds_DATA_5.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                    //    ds_DATA_5.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                    //    ds_DATA_5.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                    //    ds_DATA_5.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                    //    ds_DATA_5.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                    //    ds_DATA_5.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //// 7번 알고리즘 로드
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_6.RowCount; i++)
                    //{
                    //    ds_DATA_6.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                    //    ds_DATA_6.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                    //    ds_DATA_6.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                    //    ds_DATA_6.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_6.RowCount; i++)
                    //{
                    //    ds_DATA_6.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                    //    ds_DATA_6.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                    //    ds_DATA_6.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                    //    ds_DATA_6.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                    //    ds_DATA_6.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                    //    ds_DATA_6.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                    //    ds_DATA_6.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //// 8번 알고리즘 로드
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_7.RowCount; i++)
                    //{
                    //    ds_DATA_7.Tables[0].Rows[i][0] = worksheet.Cells[2 + i + start_num, 1].Value.ToString() == "0" ? false : true;
                    //    ds_DATA_7.Tables[0].Rows[i][1] = worksheet.Cells[2 + i + start_num, 2].Value.ToString();
                    //    ds_DATA_7.Tables[0].Rows[i][2] = worksheet.Cells[2 + i + start_num, 3].Value.ToString();
                    //    ds_DATA_7.Tables[0].Rows[i][3] = worksheet.Cells[2 + i + start_num, 4].Value.ToString();
                    //    T_Cnt++;
                    //}
                    //start_num = T_Cnt;
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_Value_7.RowCount; i++)
                    //{
                    //    ds_DATA_7.Tables[1].Rows[i][0] = worksheet.Cells[i + 2 + start_num, 1].Value;
                    //    ds_DATA_7.Tables[1].Rows[i][1] = worksheet.Cells[i + 2 + start_num, 2].Value;
                    //    ds_DATA_7.Tables[1].Rows[i][2] = worksheet.Cells[i + 2 + start_num, 3].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[2].Value.ToString();
                    //    ds_DATA_7.Tables[1].Rows[i][4] = worksheet.Cells[i + 2 + start_num, 4].Value;// = comboCell2.Value.ToString();
                    //    ds_DATA_7.Tables[1].Rows[i][5] = worksheet.Cells[i + 2 + start_num, 5].Value;//= LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[5].Value.ToString();
                    //    ds_DATA_7.Tables[1].Rows[i][6] = worksheet.Cells[i + 2 + start_num, 6].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[6].Value.ToString();
                    //    ds_DATA_7.Tables[1].Rows[i][7] = worksheet.Cells[i + 2 + start_num, 7].Value;// = LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].Cells[7].Value.ToString();
                    //    T_Cnt++;
                    //}

                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_0.RowCount; i++)
                    //{
                    //    if (LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].Cells[2].Value.ToString().Contains("예비"))
                    //    {
                    //        LVApp.Instance().m_mainform.dataGridView_Setting_0.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    //        LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    //    }
                    //}
                    //for (int i = 0; i < LVApp.Instance().m_mainform.dataGridView_Setting_1.RowCount; i++)
                    //{
                    //    if (LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].Cells[2].Value.ToString().Contains("예비"))
                    //    {
                    //        LVApp.Instance().m_mainform.dataGridView_Setting_1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    //        LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    //    }
                    //}

                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        LVApp.Instance().m_mainform.add_Log("판정 설정 불러오기 완료.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        LVApp.Instance().m_mainform.add_Log("Judgement Setting Loaded.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        LVApp.Instance().m_mainform.add_Log("完成导入判定设置.");
                    }
                    //LVApp.Instance().m_mainform.ctr_Parameters1.Set_Parameters();
                }
            }
            catch //(System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                //LVApp.Instance().m_mainform.add_Log("검사 설정 로드 에러!");
            }
        }

        public String Result_Image_Save(int Cam_num, Bitmap Img, int OK_NG_NONE_Flag)
        {
            string ofilename = "";
            try
            {
                if (OK_NG_NONE_Flag == 0)
                {
                    ofilename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK";
                }
                else if (OK_NG_NONE_Flag == 1)
                {
                    ofilename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NG";
                }
                else if (OK_NG_NONE_Flag == 2)
                {
                    ofilename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NO OBJECT";
                }
                if (OK_NG_NONE_Flag == 0 && LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                {
                    ofilename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NONE";
                }
                else if (OK_NG_NONE_Flag != 0 && LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                {
                    return ofilename;
                }
                if (m_Cam_Log_Use_Check[Cam_num] && LVApp.Instance().m_mainform.m_ImageSavethread_Check)
                {
                    LVApp.Save_Images t_Save_Images = new LVApp.Save_Images();
                    if (Cam_num == 0)
                    {
                        int t_cam_num = 0;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    else if (Cam_num == 1)
                    {
                        int t_cam_num = 1;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    else if (Cam_num == 2)
                    {
                        int t_cam_num = 2;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    else if (Cam_num == 3)
                    {
                        int t_cam_num = 3;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    t_Save_Images._Filename = ofilename;
                    t_Save_Images._Image = Img;
                    t_Save_Images._OK_NG_NONE_Flag = OK_NG_NONE_Flag;

                    //lock (LVApp.Instance().SAVE_IMAGE_List[Cam_num])
                    {
                        LVApp.Instance().SAVE_IMAGE_List[Cam_num].Add(t_Save_Images);
                    }
                }
                /*
                Create_Save_Folders();

                if (m_Cam_Log_Use_Check[Cam_num])
                {
                    string fn = "";
                    DateTime dt = DateTime.Now;
                    // MM_DD_YYYY_HH_MM_SS.LOG
                    fn += dt.Year.ToString("0000");
                    fn += "_" + dt.Month.ToString("00");
                    fn += "_" + dt.Day.ToString("00") + "";

                    if (OK_NG_Flag && (m_Cam_Log_Method == 0 || m_Cam_Log_Method == 2)) // OK 저장
                    {
                        if (m_Cam_Log_Format == 0)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".bmp";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".bmp";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }
                        else if (m_Cam_Log_Format == 1)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".jpg";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".jpg";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else if (m_Cam_Log_Format == 2)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        else if (m_Cam_Log_Format == 3)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\OK\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                    else if (!OK_NG_Flag && (m_Cam_Log_Method == 1 || m_Cam_Log_Method == 2)) // NG 저장
                    {
                        if (m_Cam_Log_Format == 0)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".bmp";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".bmp";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }
                        else if (m_Cam_Log_Format == 1)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".jpg";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".jpg";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else if (m_Cam_Log_Format == 2)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        else if (m_Cam_Log_Format == 3)
                        {
                            if (m_Log_Save_Folder == "")
                            {
                                string filename = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + fn + "\\CAM" + Cam_num.ToString() + "\\NG\\#" + Cam_num.ToString() + "_" + ofilename + ".png";
                                Img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                }
                 */
            }
            catch// (System.Exception ex)
            {
                //Create_Save_Folders();
                //MessageBox.Show("이미지가 없습니다. 이미지를 다시 로딩하세요!");
            }
            return ofilename;
        }


        public String SSF_Result_Image_Save(int Cam_num, Bitmap Img, int OK_NG_NONE_Flag)
        {
            string ofilename = "";
            try
            {
                if (OK_NG_NONE_Flag == 0)
                {
                    ofilename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK";
                }
                else
                {
                    ofilename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_Result";
                }

                if (m_Cam_Log_Use_Check[Cam_num] && LVApp.Instance().m_mainform.m_ImageSavethread_Check)
                {
                    LVApp.Save_Images t_Save_Images = new LVApp.Save_Images();
                    if (Cam_num == 0)
                    {
                        int t_cam_num = 0;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    else if (Cam_num == 1)
                    {
                        int t_cam_num = 1;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    else if (Cam_num == 2)
                    {
                        int t_cam_num = 2;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    else if (Cam_num == 3)
                    {
                        int t_cam_num = 3;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            t_Save_Images._Cam_num = t_cam_num;
                        }
                        else
                        {
                            t_Save_Images._Cam_num = Cam_num;
                        }
                    }
                    t_Save_Images._Filename = ofilename;
                    t_Save_Images._Image = Img;
                    t_Save_Images._OK_NG_NONE_Flag = OK_NG_NONE_Flag;

                    //lock (LVApp.Instance().SAVE_IMAGE_List[Cam_num])
                    {
                        LVApp.Instance().SAVE_IMAGE_List[Cam_num].Add(t_Save_Images);
                    }
                }
            }
            catch// (System.Exception ex)
            {
                //Create_Save_Folders();
                //MessageBox.Show("이미지가 없습니다. 이미지를 다시 로딩하세요!");
            }
            return ofilename;
        }
        public void Set_Parameters()
        {
            try
            {
                //LVApp.Instance().m_Config.m_Alg_Type = Properties.Settings.Default.PC_No;
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(Alg_TextView, Alg_Debugging);

                String m_AI_folder = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                {
                    m_AI_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                }
                if (AI_Image_Save)
                {
                    DirectoryInfo dir = new DirectoryInfo(m_AI_folder + "\\AI");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting1.Force_USE.Checked)
                    {
                        int t_cam_num = 0;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + t_cam_num.ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM0");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM0\\OK");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM0\\NG");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                    }
                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting2.Force_USE.Checked)
                    {
                        int t_cam_num = 1;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + t_cam_num.ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM1");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM1\\OK");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM1\\NG");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                    }
                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting3.Force_USE.Checked)
                    {
                        int t_cam_num = 2;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + t_cam_num.ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM2");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM2\\OK");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM2\\NG");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                    }
                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting4.Force_USE.Checked)
                    {
                        int t_cam_num = 3;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + t_cam_num.ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM3");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM3\\OK");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                        //dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM3\\NG");
                        //// 폴더가 존재하지 않으면
                        //if (dir.Exists == false)
                        //{
                        //    // 새로 생성합니다.
                        //    dir.Create();
                        //}
                    }
                }

                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_ModelName(m_Model_Name, m_AI_folder + "\\AI", false);

                String m_SSF_folder = LVApp.Instance().excute_path + "\\Images\\" + m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                {
                    m_SSF_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                }
                for (int i = 0; i < 4; i++)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_SSFSAVEFOLDER(m_SSF_folder, SSF_Image_Save, LVApp.Instance().m_Config.m_Cam_Log_Format, i);
                }

                LVApp.Instance().m_mainform.ctr_Yield1.Update_UI();
                for (int nCam = 0; nCam < 4; nCam++)
                {
                    m_AIParam[nCam].Clear();
                    DataSet DS = null;
                    if (nCam == 0)
                    {
                        DS = ds_DATA_0;
                    }
                    else if (nCam == 1)
                    {
                        DS = ds_DATA_1;
                    }
                    else if (nCam == 2)
                    {
                        DS = ds_DATA_2;
                    }
                    else if (nCam == 3)
                    {
                        DS = ds_DATA_3;
                    }

                    // Offset 입력
                    double CAM_Offset1 = Convert.ToDouble(DS.Tables[1].Rows[0][2]);
                    double CAM_Offset2 = Convert.ToDouble(DS.Tables[1].Rows[1][2]);
                    double CAM_Offset3 = Convert.ToDouble(DS.Tables[1].Rows[2][2]);
                    double CAM_Offset4 = Convert.ToDouble(DS.Tables[1].Rows[3][2]);
                    double CAM_Offset5 = Convert.ToDouble(DS.Tables[1].Rows[4][2]);
                    double CAM_Offset6 = Convert.ToDouble(DS.Tables[1].Rows[5][2]);
                    double CAM_Offset7 = Convert.ToDouble(DS.Tables[1].Rows[6][2]);
                    double CAM_Offset8 = Convert.ToDouble(DS.Tables[1].Rows[7][2]);
                    double CAM_Offset9 = Convert.ToDouble(DS.Tables[1].Rows[8][2]);
                    double CAM_Offset10 = Convert.ToDouble(DS.Tables[1].Rows[9][2]);
                    double CAM_Offset11 = Convert.ToDouble(DS.Tables[1].Rows[10][2]);
                    double CAM_Offset12 = Convert.ToDouble(DS.Tables[1].Rows[11][2]);
                    double CAM_Offset13 = Convert.ToDouble(DS.Tables[1].Rows[12][2]);
                    double CAM_Offset14 = Convert.ToDouble(DS.Tables[1].Rows[13][2]);
                    double CAM_Offset15 = Convert.ToDouble(DS.Tables[1].Rows[14][2]);
                    double CAM_Offset16 = Convert.ToDouble(DS.Tables[1].Rows[15][2]);
                    double CAM_Offset17 = Convert.ToDouble(DS.Tables[1].Rows[16][2]);
                    double CAM_Offset18 = Convert.ToDouble(DS.Tables[1].Rows[17][2]);
                    double CAM_Offset19 = Convert.ToDouble(DS.Tables[1].Rows[18][2]);
                    double CAM_Offset20 = Convert.ToDouble(DS.Tables[1].Rows[19][2]);
                    double CAM_Offset21 = Convert.ToDouble(DS.Tables[1].Rows[20][2]);
                    double CAM_Offset22 = Convert.ToDouble(DS.Tables[1].Rows[21][2]);
                    double CAM_Offset23 = Convert.ToDouble(DS.Tables[1].Rows[22][2]);
                    double CAM_Offset24 = Convert.ToDouble(DS.Tables[1].Rows[23][2]);
                    double CAM_Offset25 = Convert.ToDouble(DS.Tables[1].Rows[24][2]);
                    double CAM_Offset26 = Convert.ToDouble(DS.Tables[1].Rows[25][2]);
                    double CAM_Offset27 = Convert.ToDouble(DS.Tables[1].Rows[26][2]);
                    double CAM_Offset28 = Convert.ToDouble(DS.Tables[1].Rows[27][2]);
                    double CAM_Offset29 = Convert.ToDouble(DS.Tables[1].Rows[28][2]);
                    double CAM_Offset30 = Convert.ToDouble(DS.Tables[1].Rows[29][2]);
                    double CAM_Offset31 = Convert.ToDouble(DS.Tables[1].Rows[30][2]);
                    double CAM_Offset32 = Convert.ToDouble(DS.Tables[1].Rows[31][2]);
                    double CAM_Offset33 = Convert.ToDouble(DS.Tables[1].Rows[32][2]);
                    double CAM_Offset34 = Convert.ToDouble(DS.Tables[1].Rows[33][2]);
                    double CAM_Offset35 = Convert.ToDouble(DS.Tables[1].Rows[34][2]);
                    double CAM_Offset36 = Convert.ToDouble(DS.Tables[1].Rows[35][2]);
                    double CAM_Offset37 = Convert.ToDouble(DS.Tables[1].Rows[36][2]);
                    double CAM_Offset38 = Convert.ToDouble(DS.Tables[1].Rows[37][2]);
                    double CAM_Offset39 = Convert.ToDouble(DS.Tables[1].Rows[38][2]);
                    double CAM_Offset40 = Convert.ToDouble(DS.Tables[1].Rows[39][2]);
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(nCam,
                        CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                        , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                        , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                        , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                        );

                    // ROI 변수 입력
                    for (int j = 0; j < 41; j++)
                    {
                        string str_ROI = DS.Tables[2].Rows[j][1].ToString();
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_ROI_Parameters(str_ROI, j, nCam, DS.Tables[2].Rows[41][1].ToString());
                    }

                    // 검사 변수 입력
                    for (int t_idx = 0; t_idx < 41; t_idx++)
                    {
                        bool AI_Param_Check = false;
                        if (DS.Tables[2].Rows.Count < 1)
                        {
                            break;
                        }
                        string[] str = DS.Tables[2].Rows[t_idx][1].ToString().Split('_');

                        if (str.Length < 10)
                        {
                            break;
                        }
                        double CAM_P5 = 0;

                        if (str[5] == "v1 이하" || str[5] == "v1 less than")
                        {
                            CAM_P5 = 0;
                        }
                        else if (str[5] == "v2 이상" || str[5] == "v2 more than")
                        {
                            CAM_P5 = 1;
                        }
                        else if (str[5] == "v1~v2 사이" || str[5] == "v1~v2")
                        {
                            CAM_P5 = 2;
                        }
                        else if (str[5] == "v1이하v2이상" || str[5] == "less v1 more v2")
                        {
                            CAM_P5 = 3;
                        }
                        else if (str[5] == "자동이하" || str[5] == "Auto less than")
                        {
                            CAM_P5 = 4;
                        }
                        else if (str[5] == "자동이상" || str[5] == "Auto more than")
                        {
                            CAM_P5 = 5;
                        }
                        else if (str[5] == "에지" || str[5] == "Edge")
                        {
                            CAM_P5 = 6;
                        }
                        else if (str[5] == "검사 영역 결과 사용" || str[5] == "모델 사용" || str[5] == "Insp. area result use" || str[5] == "Model find")
                        {
                            CAM_P5 = 7;
                        }
                        else if (str[5] == "평균기준 차이" || str[5] == "Diff. from AVG")
                        {
                            CAM_P5 = 8;
                        }
                        else if (str[5] == "비교v1이하v2이상" || str[5] == "Compare less v1 more v2")
                        {
                            CAM_P5 = 9;
                        }
                        else if (str[5] == "Color CH1 v1~v2 사이" || str[5] == "Color CH1 v1~v2")
                        {
                            CAM_P5 = 10;
                        }
                        else if (str[5] == "Color CH2 v1~v2 사이" || str[5] == "Color CH2 v1~v2")
                        {
                            CAM_P5 = 11;
                        }




                        double CAM_P6 = 0; double.TryParse(str[6], out CAM_P6);
                        double CAM_P7 = 0; double.TryParse(str[7], out CAM_P7);
                        double CAM_P8 = 0;
                        if (m_Camera_Position[nCam] == 0) // 상부, 하부일때
                        {
                            if (str[8] == "가로 길이" || str[8] == "좌측 끝 기준" || str[8] == "Hor. length" || str[8] == "Left end")
                            {
                                CAM_P8 = 0;
                            }
                            else if (str[8] == "세로 길이" || str[8] == "우측 끝 기준" || str[8] == "Ver. length" || str[8] == "Right end")
                            {
                                CAM_P8 = 1;
                            }
                            else if (str[8] == "십자 치수" || str[8] == "좌상 기준" || str[8] == "Dim. of cross" || str[8] == "Left top")
                            {
                                CAM_P8 = 2;
                            }
                            else if (str[8] == "직경" || str[8] == "좌하 기준" || str[8] == "Diameter" || str[8] == "Left bottom")
                            {
                                CAM_P8 = 3;
                            }
                            else if (str[8] == "사각 영역의 밝기" || str[8] == "우상 기준" || str[8] == "Brightness of rectangle ROI" || str[8] == "Right top")
                            {
                                CAM_P8 = 4;
                            }
                            else if (str[8] == "원형 영역의 밝기" || str[8] == "우하 기준" || str[8] == "Brightness of circle ROI" || str[8] == "Right bottom")
                            {
                                CAM_P8 = 5;
                            }
                            else if (str[8] == "사각 영역의 BLOB" || str[8] == "중심 기준" || str[8] == "BLOB in rectangle ROI" || str[8] == "Center")
                            {
                                CAM_P8 = 6;
                            }
                            else if (str[8] == "사각 ROI의 BLOB 개수(Count)" || str[8] == "상부 중심 기준" || str[8] == "BLOB count in rect ROI(Count)" || str[8] == "Top Center")
                            {
                                CAM_P8 = 7;
                            }
                            else if (str[8] == "원형 영역의 BLOB" || str[8] == "상부 꼭지점 기준" || str[8] == "BLOB in circle ROI")
                            {
                                CAM_P8 = 8;
                            }
                            else if (str[8] == "원형 영역의 BLOB 갯수" || str[8] == "BLOB count in circle ROI")
                            {
                                CAM_P8 = 9;
                            }
                            else if (str[8] == "진원도(%)" || str[8] == "Circularity(%)")
                            {
                                CAM_P8 = 10;
                            }
                            else if (str[8] == "나사산 피치" || str[8] == "Pitch of thread")
                            {
                                CAM_P8 = 11;
                            }
                            else if (str[8] == "두 영역 중심간 거리" || str[8] == "Distance between two area")
                            {
                                CAM_P8 = 12;
                            }
                            else if (str[8] == "나사산 크기" || str[8] == "Size of thread")
                            {
                                CAM_P8 = 13;
                            }
                            else if (str[8] == "원형 영역의 색상 BLOB" || str[8] == "Color BLOB in circle ROI")
                            {
                                CAM_P8 = 14;
                            }
                            else if (str[8] == "볼록 BLOB 차이" || str[8] == "Convex BLOB Difference")
                            {
                                CAM_P8 = 15;
                            }
                            else if (str[8] == "내외경 중심 차이" || str[8] == "Center difference between Inner and outter circle")
                            {
                                CAM_P8 = 16;
                            }
                            else if (str[8] == "일치율(%)" || str[8] == "Match rate(%)")
                            {
                                CAM_P8 = 17;
                            }
                            else if (str[8] == "면취 측정" || str[8] == "Bevelling Measurement")
                            {
                                CAM_P8 = 18;
                            }
                            else if (str[8] == "AI 검사" || str[8] == "AI Inspection")
                            {
                                CAM_P8 = 19;
                                AI_Param_Check = true;
                            }
                            else if (str[8] == "SSF")
                            {
                                CAM_P8 = 20;
                            }
                        }
                        else // 측면일때
                        {
                            if (str[8] == "가로 길이" || str[8] == "좌측 끝 기준" || str[8] == "Hor. length" || str[8] == "Left end")
                            {
                                CAM_P8 = 0;
                            }
                            else if (str[8] == "세로 길이" || str[8] == "우측 끝 기준" || str[8] == "Ver. length" || str[8] == "Right end")
                            {
                                CAM_P8 = 1;
                            }
                            else if (str[8] == "머리 나사부 동심도" || str[8] == "좌상 기준" || str[8] == "Concentricity" || str[8] == "Left top")
                            {
                                CAM_P8 = 2;
                            }
                            else if (str[8] == "하부 V 각도" || str[8] == "좌하 기준" || str[8] == "V Angle of bottom" || str[8] == "Left bottom")
                            {
                                CAM_P8 = 3;
                            }
                            else if (str[8] == "나사산 피치" || str[8] == "우상 기준" || str[8] == "Pitch of thread" || str[8] == "Right top")
                            {
                                CAM_P8 = 4;
                            }
                            else if (str[8] == "나사산 크기" || str[8] == "우하 기준" || str[8] == "Size of thread" || str[8] == "Right bottom")
                            {
                                CAM_P8 = 5;
                            }
                            else if (str[8] == "리드각(1)" || str[8] == "중심 기준" || str[8] == "Lead angle of thread(1)" || str[8] == "Center")
                            {
                                CAM_P8 = 6;
                            }
                            else if (str[8] == "리드각(0.5)" || str[8] == "상부 중심 기준" || str[8] == "Lead angle of thread(0.5)" || str[8] == "Top Center")
                            {
                                CAM_P8 = 7;
                            }
                            else if (str[8] == "몸통 두께" || str[8] == "상부 꼭지점 기준" || str[8] == "Thickness of body(mm)")
                            {
                                CAM_P8 = 8;
                            }
                            else if (str[8] == "몸통 휨" || str[8] == "Bending of body(mm)")
                            {
                                CAM_P8 = 9;
                            }
                            else if (str[8] == "사각 영역의 밝기" || str[8] == "Brightness of rectangle ROI")
                            {
                                CAM_P8 = 10;
                            }
                            else if (str[8] == "사각 영역의 BLOB" || str[8] == "BLOB in rectangle ROI")
                            {
                                CAM_P8 = 11;
                            }
                            else if (str[8] == "하부 형상" || str[8] == "Shape of bottom")
                            {
                                CAM_P8 = 12;
                            }
                            else if (str[8] == "두 영역 중심간 거리" || str[8] == "Distance between two area")
                            {
                                CAM_P8 = 13;
                            }
                            else if (str[8] == "볼록 BLOB 차이" || str[8] == "Convex BLOB Difference")
                            {
                                CAM_P8 = 14;
                            }
                            else if (str[8] == "일치율(%)" || str[8] == "Match rate(%)")
                            {
                                CAM_P8 = 15;
                            }
                            else if (str[8] == "면취 측정" || str[8] == "Bevelling Measurement")
                            {
                                CAM_P8 = 16;
                            }
                            else if (str[8] == "AI 검사" || str[8] == "AI Inspection")
                            {
                                CAM_P8 = 17;
                                AI_Param_Check = true;
                            }
                            else if (str[8] == "SSF")
                            {
                                CAM_P8 = 18;
                            }

                        }


                        double CAM_P9 = 0;
                        if (str[9] == "최솟값" || str[9] == "MIN")
                        {
                            CAM_P9 = 0;
                        }
                        else if (str[9] == "최댓값" || str[9] == "MAX")
                        {
                            CAM_P9 = 1;
                        }
                        else if (str[9] == "최댓값-최솟값" || str[9] == "MAX-MIN")
                        {
                            CAM_P9 = 2;
                        }
                        else if (str[9] == "평균값" || str[9] == "AVG")
                        {
                            CAM_P9 = 3;
                        }
                        else if (str[9] == "총합" || str[9] == "TOTAL")
                        {
                            CAM_P9 = 4;
                        }

                        double CAM_P10 = 0; double.TryParse(str[10], out CAM_P10);
                        double CAM_P11 = 0; double.TryParse(str[11], out CAM_P11);
                        double CAM_P12 = 0; double.TryParse(str[12], out CAM_P12);
                        double CAM_P13 = 0; double.TryParse(str[13], out CAM_P13);
                        double CAM_P14 = 0; double.TryParse(str[14], out CAM_P14);
                        double CAM_P15 = 0; double.TryParse(str[15], out CAM_P15);
                        double CAM_P16 = 0; double.TryParse(str[16], out CAM_P16);
                        double CAM_P17 = 0; double.TryParse(str[17], out CAM_P17);
                        double CAM_P18 = 0; double.TryParse(str[18], out CAM_P18);
                        double CAM_P19 = 0; double.TryParse(str[19], out CAM_P19);
                        double CAM_P20 = 0; double.TryParse(str[20], out CAM_P20);
                        double CAM_P21 = 0; double.TryParse(str[21], out CAM_P21);
                        double CAM_P22 = 0; double.TryParse(str[22], out CAM_P22);
                        double CAM_P23 = 0; double.TryParse(str[23], out CAM_P23);
                        double CAM_P24 = 0; double.TryParse(str[24], out CAM_P24);
                        double CAM_P25 = 0; double.TryParse(str[25], out CAM_P25);

                        if (m_Camera_Position[nCam] == 0 && CAM_P8 == 20) // 상부, 하부일때
                        {
                            if (CAM_P24 >= 3 && CAM_P24 <= 5)
                            { // AI 검사
                                string t_Model_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + nCam.ToString() + "_ROI" + t_idx.ToString("00") + "_Model.pb";
                                if (File.Exists(t_Model_Path))
                                {
                                    string Model_Path = ".\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + nCam.ToString() + "_ROI" + t_idx.ToString("00") + "_Model.pb";
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_AI_Model(nCam, t_idx, Model_Path);
                                }
                                //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_AI_Model_Loaded(nCam, t_idx))
                                //{
                                //    if (nCam == 0)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //    else if (nCam == 1)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI2.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //    else if (nCam == 2)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI3.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //    else if (nCam == 3)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI4.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //}
                                //else
                                //{
                                //    if (nCam == 0)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //    else if (nCam == 1)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI2.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //    else if (nCam == 2)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI3.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //    else if (nCam == 3)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI4.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //}
                            }
                        }
                        else if (m_Camera_Position[nCam] != 0 && CAM_P8 == 18) // 측면일때
                        {
                            if (CAM_P24 >= 3 && CAM_P24 <= 5)
                            { // AI 검사
                                string t_Model_Path = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + nCam.ToString() + "_ROI" + t_idx.ToString("00") + "_Model.pb";
                                if (File.Exists(t_Model_Path))
                                {
                                    string Model_Path = ".\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\AI_Model\\" + "CAM" + nCam.ToString() + "_ROI" + t_idx.ToString("00") + "_Model.pb";
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_AI_Model(nCam, t_idx, Model_Path);
                                }
                                //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_AI_Model_Loaded(nCam, t_idx))
                                //{
                                //    if (nCam == 0)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //    else if (nCam == 1)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI2.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //    else if (nCam == 2)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI3.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //    else if (nCam == 3)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI4.dataGridView1.Rows[25].Cells[1].Value = "1";
                                //    }
                                //}
                                //else
                                //{
                                //    if (nCam == 0)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //    else if (nCam == 1)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI2.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //    else if (nCam == 2)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI3.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //    else if (nCam == 3)
                                //    {
                                //        LVApp.Instance().m_mainform.ctr_ROI4.dataGridView1.Rows[25].Cells[1].Value = "0";
                                //    }
                                //}
                            }
                        }

                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Parameters(t_idx, nCam,
                            nTableType[nCam], m_Camera_Position[nCam], CAM_P5, CAM_P6, CAM_P7, CAM_P8, CAM_P9, CAM_P10,
                            CAM_P11, CAM_P12, CAM_P13, CAM_P14, CAM_P15, CAM_P16, CAM_P17, CAM_P18, CAM_P19, CAM_P20, CAM_P21, CAM_P22, CAM_P23, CAM_P24, CAM_P25);

                        if (AI_Param_Check)
                        {
                            string [] AI_ROI = DS.Tables[2].Rows[t_idx][1].ToString().Split('_');
                            string [] AI_Ratio = DS.Tables[2].Rows[41][1].ToString().Split('_');
                            AIParam t_AIParam = new AIParam();
                            String m_Mask_File = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM" + nCam.ToString("0") + "_ROI" + t_idx.ToString("00") + "_Mask.bmp";
                            if (File.Exists(m_Mask_File))
                            {
                                using (Mat t_src = Cv2.ImRead(m_Mask_File, ImreadModes.Grayscale))
                                {
                                    using (Bitmap t_bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(t_src))
                                    {
                                        t_AIParam.Mask = t_bmp.Clone() as Bitmap;
                                    }
                                }
                            }
                            else
                            {
                                t_AIParam.Mask = null;
                            }
                            if (AI_ROI[0] == "O")
                            {
                                t_AIParam.USE = true;
                            }
                            else
                            {
                                t_AIParam.USE = false;
                            }
                            t_AIParam.ROI_IDX = t_idx - 1;
                            t_AIParam.Matching_Rate = CAM_P12;
                            t_AIParam.ROI.X = (int)(Convert.ToDouble(AI_ROI[1]) * Convert.ToDouble(AI_Ratio[0]));
                            t_AIParam.ROI.Y = (int)(Convert.ToDouble(AI_ROI[2]) * Convert.ToDouble(AI_Ratio[1]));
                            t_AIParam.ROI.Width = (int)(Convert.ToDouble(AI_ROI[3]) * Convert.ToDouble(AI_Ratio[0]));
                            t_AIParam.ROI.Height = (int)(Convert.ToDouble(AI_ROI[4]) * Convert.ToDouble(AI_Ratio[1]));
                            t_AIParam.ROI_Location = (int)CAM_P14;
                            m_AIParam[nCam].Add(t_AIParam);
                        }
                    }

                    String m_Mask_Image = string.Empty;
                    for (int i = 0; i < 41; i++)
                    {
                        m_Mask_Image = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "CAM" + nCam.ToString("0") + "_ROI" + i.ToString("00") + "_Mask.bmp";
                        if (File.Exists(m_Mask_Image))
                        {
                            using (Mat t_src = Cv2.ImRead(m_Mask_Image, ImreadModes.Grayscale))
                            {
                                using (Bitmap t_bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(t_src))
                                {
                                    byte[] arr = BmpToArray(t_bmp);
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Mask_Image(arr, t_src.Width, t_src.Height, 1, nCam, i);
                                }
                            }
                        }
                        else
                        {
                            byte[] arr = new byte[1];
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Mask_Image(arr, 0, 0, 1, nCam, i);
                        }
                    }
                }
            }
            catch
            {
                //MessageBox.Show("The value is mistyped or wrong!" + e.ToString());
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

        public void Set_ROI(int cam_num)
        {
            try
            {
                if (cam_num == 0)
                {
                    for (int j = 0; j < 41; j++)
                    {
                        string str_ROI = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[j][1].ToString();
                        // MessageBox.Show(str_ROI);
                        //MessageBox.Show(LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[21][1].ToString());
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_ROI_Parameters(str_ROI, j, 0, LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[41][1].ToString());
                    }
                }
                if (cam_num == 1)
                {
                    for (int j = 0; j < 41; j++)
                    {
                        string str_ROI = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[j][1].ToString();
                        // MessageBox.Show(str_ROI);
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_ROI_Parameters(str_ROI, j, 1, LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[41][1].ToString());
                    }
                }
                if (cam_num == 2)
                {
                    for (int j = 0; j < 41; j++)
                    {
                        string str_ROI = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[j][1].ToString();
                        // MessageBox.Show(str_ROI);
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_ROI_Parameters(str_ROI, j, 2, LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[41][1].ToString());
                    }
                }
                if (cam_num == 3)
                {
                    for (int j = 0; j < 41; j++)
                    {
                        string str_ROI = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[j][1].ToString();
                        // MessageBox.Show(str_ROI);
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_ROI_Parameters(str_ROI, j, 3, LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[41][1].ToString());
                    }
                }
            }
            catch
            {
            }
        }

        #region LHJ - 240804 디버깅용
        // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
        // 알고리즘을 다운 시키는 이미지가 있는지 확인
        // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 _lastImageCount개 이미지)를 저장함
        public const int _lastImageCount = 2;
        public Bitmap[] _lastImage_Cam0 = new Bitmap[_lastImageCount];
        public Bitmap[] _lastImage_Cam1 = new Bitmap[_lastImageCount];
        public Bitmap[] _lastImage_Cam2 = new Bitmap[_lastImageCount];
        public Bitmap[] _lastImage_Cam3 = new Bitmap[_lastImageCount];
        #endregion
    }
}
