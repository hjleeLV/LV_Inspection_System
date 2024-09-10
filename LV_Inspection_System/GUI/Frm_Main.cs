using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;   //호환되지 않은 Dll을 사용할때
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
//using AForge.Imaging.IPPrototyper;
using AForge.Imaging.Formats;
using System.Collections;
using PylonC.NETSupportLibrary;
using OfficeOpenXml;
using LV_Inspection_System.UTIL;
using OpenCvSharp;
using System.Threading.Tasks;

namespace LV_Inspection_System.GUI
{
    public partial class Frm_Main : XCoolForm.XCoolForm
    {
        public bool Simulation_mode = false;

        public ImPro_Library_Clr.ClassClr m_ImProClr_Class = new ImPro_Library_Clr.ClassClr(); // C++ source CLR 연결;

        public bool Force_close = false;
        public bool t_setting_view_mode = false;
        public bool t_cam_setting_view_mode = false;
        private bool t_cam_ROI_view_mode = false;


        private DongleKey t_DongleKey = new DongleKey();

        public Stopwatch[] Run_SW = new Stopwatch[4];

        Thread[] threads = new Thread[4];
        Thread[] Probe_threads = new Thread[4];
        Thread[] Viewthreads = new Thread[4];
        //private System.Windows.Forms.Timer[] timer_Cam = new System.Windows.Forms.Timer[4];


        Thread ImageSavethread = null;
        public bool m_ImageSavethread_Check = false;

        Thread Monitoringthread = null;
        public bool m_Monitoringthread_Check = false;


        private bool[] m_ViewThreads_Check = new bool[4];
        private bool[] m_Threads_Check = new bool[4];
        public int m_Job_Mode0 = 0;
        public int m_Job_Mode1 = 0;
        public int m_Job_Mode2 = 0;
        public int m_Job_Mode3 = 0;
        public int m_Probe_Job_Mode0 = 0;
        public int m_Probe_Job_Mode1 = 0;
        public int m_Probe_Job_Mode2 = 0;
        public int m_Probe_Job_Mode3 = 0;
        public int m_Result_Job_Mode0 = 0;
        public int m_Result_Job_Mode1 = 0;
        public int m_Result_Job_Mode2 = 0;
        public int m_Result_Job_Mode3 = 0;

        //public Queue[] Capture_framebuffer = new Queue[4];
        //public Queue[] Result_framebuffer = new Queue[4];

        public List<Bitmap>[] Capture_framebuffer = new List<Bitmap>[4];
        public List<Bitmap>[] Result_framebuffer = new List<Bitmap>[4];

        public int[] Capture_Count = new int[4];

        public bool m_Start_Check = false;
        public bool m_Start_Button_Check = false;

        public bool m_Last_Start_Loading = false;

        public Frm_Main()
        {
            InitializeComponent();

            button_INSPECTION.Enabled = false;

            LVApp.Instance().m_mainform = this;

            pictureBox_Setting_0.AllowDrop = true;
            pictureBox_Setting_1.AllowDrop = true;
            pictureBox_Setting_2.AllowDrop = true;
            pictureBox_Setting_3.AllowDrop = true;
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    button_INSPECTION.Text = "검사 대기";
                    button_RESET.Text = "리셋[Reset]";
                    button_TEST_INSPECTION0.Text = "시험검사";
                    button_SAVE0.Text = "저장";
                    button_TEST_INSPECTION1.Text = "시험검사";
                    button_SAVE1.Text = "저장";
                    button_TEST_INSPECTION2.Text = "시험검사";
                    button_SAVE2.Text = "저장";
                    button_TEST_INSPECTION3.Text = "시험검사";
                    button_SAVE3.Text = "저장";
                    button_Open0.Text = "이미지 열기";
                    button_Open1.Text = "이미지 열기";
                    button_Open2.Text = "이미지 열기";
                    button_Open3.Text = "이미지 열기";
                    neoTabWindow_MAIN.TabPages[0].Text = "메인화면";
                    neoTabWindow_MAIN.TabPages[1].Text = "검사설정";
                    neoTabWindow_MAIN.TabPages[2].Text = "스펙설정";
                    neoTabWindow_MAIN.TabPages[3].Text = "장비설정";
                    neoTabWindow_MAIN.TabPages[4].Text = "로그";
                    neoTabWindow_MAIN.TabPages[5].Text = "모델";
                    neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                    //neoTabWindow_MAIN.TabPages[7].Text = "도움말";
                    //neoTabWindow_INSP_SETTING.TabPages[0].Text = "판정 설정";
                    neoTabWindow_INSP_SETTING.TabPages[0].Text = "변수 설정";
                    neoTabWindow_INSP_SETTING.TabPages[1].Text = "관리자 설정";
                    neoTabWindow_EQUIP_SETTING.TabPages[0].Text = "카메라 설정";
                    neoTabWindow_EQUIP_SETTING.TabPages[1].Text = "통신 설정";
                    neoTabWindow_LOG.TabPages[0].Text = "검사 결과값";
                    neoTabWindow_LOG.TabPages[2].Text = "NG 로그";
                    neoTabWindow_LOG.TabPages[1].Text = "Graph";
                    neoTabWindow_LOG.TabPages[3].Text = "로그 설정";
                    neoTabWindow_LOG.TabPages[4].Text = "공정능력";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    button_INSPECTION.Text = "START";
                    button_RESET.Text = "Reset";
                    button_TEST_INSPECTION0.Text = "Test";
                    button_SAVE0.Text = "Save";
                    button_TEST_INSPECTION1.Text = "Test";
                    button_SAVE1.Text = "Save";
                    button_TEST_INSPECTION2.Text = "Test";
                    button_SAVE2.Text = "Save";
                    button_TEST_INSPECTION3.Text = "Test";
                    button_SAVE3.Text = "Save";
                    button_Open0.Text = "Image Open";
                    button_Open1.Text = "Image Open";
                    button_Open2.Text = "Image Open";
                    button_Open3.Text = "Image Open";

                    neoTabWindow_MAIN.TabPages[0].Text = "Main";
                    neoTabWindow_MAIN.TabPages[1].Text = "Parameters";
                    neoTabWindow_MAIN.TabPages[2].Text = "Spec";
                    neoTabWindow_MAIN.TabPages[3].Text = "System";
                    neoTabWindow_MAIN.TabPages[4].Text = "Log";
                    neoTabWindow_MAIN.TabPages[5].Text = "Model";
                    neoTabWindow_MAIN.TabPages[6].Text = "Login";
                    //neoTabWindow_MAIN.TabPages[7].Text = "Help";
                    //neoTabWindow_INSP_SETTING.TabPages[0].Text = "Judgement";
                    neoTabWindow_INSP_SETTING.TabPages[0].Text = "ROI parameter";
                    neoTabWindow_INSP_SETTING.TabPages[1].Text = "Admin setting";
                    neoTabWindow_EQUIP_SETTING.TabPages[0].Text = "Camera setting";
                    neoTabWindow_EQUIP_SETTING.TabPages[1].Text = "PLC setting";
                    neoTabWindow_LOG.TabPages[0].Text = "Result data";
                    neoTabWindow_LOG.TabPages[1].Text = "NG log";
                    neoTabWindow_LOG.TabPages[2].Text = "Graph";
                    neoTabWindow_LOG.TabPages[3].Text = "Log setting";
                    neoTabWindow_LOG.TabPages[4].Text = "Report";
                }
                else if (value == 2 && m_Language != value)
                {// 중국어
                    button_INSPECTION.Text = "开始";
                    button_RESET.Text = "重 置";
                    button_TEST_INSPECTION0.Text = "测试";
                    button_SAVE0.Text = "救";
                    button_TEST_INSPECTION1.Text = "测试";
                    button_SAVE1.Text = "救";
                    button_TEST_INSPECTION2.Text = "测试";
                    button_SAVE2.Text = "救";
                    button_TEST_INSPECTION3.Text = "测试";
                    button_SAVE3.Text = "救";
                    button_Open0.Text = "图像打开";
                    button_Open1.Text = "图像打开";
                    button_Open2.Text = "图像打开";
                    button_Open3.Text = "图像打开";

                    neoTabWindow_MAIN.TabPages[0].Text = "主要";
                    neoTabWindow_MAIN.TabPages[1].Text = "参数";
                    neoTabWindow_MAIN.TabPages[2].Text = "规范";
                    neoTabWindow_MAIN.TabPages[3].Text = "系统";
                    neoTabWindow_MAIN.TabPages[4].Text = "日志";
                    neoTabWindow_MAIN.TabPages[5].Text = "模型";
                    neoTabWindow_MAIN.TabPages[6].Text = "登录";
                    //neoTabWindow_MAIN.TabPages[7].Text = "帮助";
                    //neoTabWindow_INSP_SETTING.TabPages[0].Text = "Judgement";
                    neoTabWindow_INSP_SETTING.TabPages[0].Text = "ROI 参数";
                    neoTabWindow_INSP_SETTING.TabPages[1].Text = "管理设置";
                    neoTabWindow_EQUIP_SETTING.TabPages[0].Text = "CAMERA 设置";
                    neoTabWindow_EQUIP_SETTING.TabPages[1].Text = "PLC 设置";

                    neoTabWindow_LOG.TabPages[0].Text = "结果数据";
                    neoTabWindow_LOG.TabPages[1].Text = "NG 日志";
                    neoTabWindow_LOG.TabPages[2].Text = "产量图";
                    neoTabWindow_LOG.TabPages[3].Text = "日志设置";
                    neoTabWindow_LOG.TabPages[4].Text = "报告";
                }

                m_Language = value;
            }
        }

        private void Frm_Main_Load(object sender, EventArgs e)
        {
            //neoTabWindow_MAIN.Enabled = false;
            //t_DongleKey.Check_License();
            try
            {
                Program_Start_Check();
                // 프로그램 시작시 중복 체크
                LVApp.Instance().m_Config.m_SetLanguage = Properties.Settings.Default.Language;

                frmSplash splash = new frmSplash();                                 // 로딩화면
                splash.Show();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    add_Log("프로그램 로딩 중입니다..");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Program is loading.", "LOAD", 3000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("程序正在加载.", "LOAD", 3000);
                }

                DebugLogger.Instance().LoggerStatusEvent += new LoggerStatusHandler(LoggerStatusEvent);
                DebugLogger.Instance().LogRecord("Program Start / Ready.");


                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    add_Log("프로그램 시작 및 준비가 되었습니다.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    add_Log("Program Start / Ready.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    add_Log("程序已启动/就绪.");
                }

                LVApp.Instance().m_Config.m_SetLanguage = Properties.Settings.Default.Language;
                //LVApp.Instance().m_Config.ds_DATA_4 = new DataSet();
                LVApp.Instance().m_Config.ds_LOG = new DataSet();

                GUI_Initialize();                                                   // UI 화면 정보 및 디자인 갱신

                LVApp.Instance().m_Config.Initial_DataBase();                    // DataBase 초기화

                Camera_Initialize();

                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    ctr_ROI4.Cam_Num = 3;
                    ctr_ROI4.ROI_Idx = 0;
                    LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                    ctr_ROI4.Initialize_ROI();
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    ctr_ROI3.Cam_Num = 2;
                    ctr_ROI3.ROI_Idx = 0;
                    LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                    ctr_ROI3.Initialize_ROI();
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    ctr_ROI2.Cam_Num = 1;
                    ctr_ROI2.ROI_Idx = 0;
                    LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                    ctr_ROI2.Initialize_ROI();
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    ctr_ROI1.Cam_Num = 0;
                    ctr_ROI1.ROI_Idx = 0;
                    LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                    ctr_ROI1.Initialize_ROI();
                }

                ctr_Model1.read_model_list();                                       // 모델 리스트 불러오기

                Thread.Sleep(100);
                LVApp.Instance().m_Config.Load_Judge_Data();                     // 저장 변수 불러오기
                                                                                 //LVApp.Instance().m_mainform.ctr_Log1.Refresh_Log_Data();
                                                                                 //LVApp.Instance().m_Config.Initialize_Data_Log(0);
                                                                                 //LVApp.Instance().m_Config.Initialize_Data_Log(1);
                                                                                 //LVApp.Instance().m_Config.Initialize_Data_Log(2);
                                                                                 //LVApp.Instance().m_Config.Initialize_Data_Log(3);

                //LVApp.Instance().m_mainform.Top = 0;
                //LVApp.Instance().m_mainform.Left = 0;
                //LVApp.Instance().m_mainform.Width = 1280;
                //LVApp.Instance().m_mainform.Height = 1024;
                LVApp.Instance().m_mainform.WindowState = FormWindowState.Maximized;

                LVApp.Instance().m_mainform.ctr_Admin_Param1.button_LOAD_Click(sender, e);

                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                    //ctr_ROI4.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Inspection_Delay[3] = 0;
                    LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                    ctr_ROI4.button_LOAD_Click(sender, e);
                    ctr_ROI4.Fit_Size();
                    ctr_Log1.checkBox_CAM3.Visible = true;
                    ctr_ROI4.load_check = false;
                    LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = -1;
                    ctr_ROI4.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Realtime_Running_Check[3] = false;
                    //LVApp.Instance().m_Config.Set_ROI(LVApp.Instance().m_Config.ROI_Cam_Num);
                    //ctr_ROI4.button_INSPECTION_Click(sender, e);
                    //ctr_ROI4.dataGridView1.Visible = false;
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                    //ctr_ROI3.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Inspection_Delay[2] = 0;
                    LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                    ctr_ROI3.button_LOAD_Click(sender, e);
                    ctr_ROI3.Fit_Size();
                    ctr_Log1.checkBox_CAM2.Visible = true;
                    ctr_ROI3.load_check = false;
                    LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = -1;
                    ctr_ROI3.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Realtime_Running_Check[2] = false;
                    //LVApp.Instance().m_Config.Set_ROI(LVApp.Instance().m_Config.ROI_Cam_Num);
                    //ctr_ROI3.button_INSPECTION_Click(sender, e);
                    //ctr_ROI3.dataGridView1.Visible = false;
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                    //ctr_ROI2.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Inspection_Delay[1] = 0;
                    LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                    ctr_ROI2.button_LOAD_Click(sender, e);
                    ctr_ROI2.Fit_Size();
                    ctr_Log1.checkBox_CAM1.Visible = true;
                    ctr_ROI2.load_check = false;
                    LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = -1;
                    ctr_ROI2.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Realtime_Running_Check[1] = false;
                    //LVApp.Instance().m_Config.Set_ROI(LVApp.Instance().m_Config.ROI_Cam_Num);
                    //ctr_ROI2.button_INSPECTION_Click(sender, e);
                    //ctr_ROI2.dataGridView1.Visible = false;
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                    //ctr_ROI1.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Inspection_Delay[0] = 0;
                    LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                    ctr_ROI1.button_LOAD_Click(sender, e);
                    ctr_ROI1.Fit_Size();
                    ctr_Log1.checkBox_CAM0.Visible = true;
                    ctr_ROI1.load_check = false;
                    LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = -1;
                    ctr_ROI1.listBox1.SelectedIndex = 0;
                    LVApp.Instance().m_Config.Realtime_Running_Check[0] = false;
                    //LVApp.Instance().m_Config.Set_ROI(LVApp.Instance().m_Config.ROI_Cam_Num);
                    //ctr_ROI1.button_INSPECTION_Click(sender, e);
                    //ctr_ROI1.dataGridView1.Visible = false;
                }

                //LVApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);
                //ctr_Admin_Param1.button_LOAD_Click(sender, e);

                LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    add_Log("검사를 시작하시려면 우상단 [검사 대기] 버튼을 눌러주세요!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    add_Log("If you want to start inspection, click START button.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    add_Log("如果要开始检查，请单击 [开始] 按钮.");
                }
                //LVApp.Instance().t_QuickMenu.Top = 7;
                //LVApp.Instance().t_QuickMenu.Left = 1280 - 271 - 126;
                //LVApp.Instance().t_QuickMenu.Show();

                System.Drawing.Point location = ctr_ROI1.PointToScreen(System.Drawing.Point.Empty);
                LVApp.Instance().m_help.Location = location;
                LVApp.Instance().m_help.webBrowser1.Navigate(LVApp.Instance().excute_path + "\\Help\\Index.html");

                //LVApp.Instance().m_Ctr_Mysql.DB_connect();
                //LVApp.Instance().m_Ctr_Mysql.DB_Create();
                LVApp.Instance().m_Config.ds_STATUS.Tables[2].Rows[0][1] = LVApp.Instance().m_Config.m_Model_Name;
                //LVApp.Instance().m_Ctr_Mysql.DB_Operating(LVApp.Instance().m_Config.ds_STATUS.Tables[2], "Information");
                //LVApp.Instance().m_Ctr_Mysql.DB_Operating(LVApp.Instance().m_Config.ds_STATUS.Tables[0], "ProgramCheck");
                //Thread.Sleep(1000);

                ctr_Model1.cmdLoad_Click(sender, e);

                ctr_Display_1.Update_Display();
                m_Start_Check = true;

                //int percentage = 20;
                //for (int i = 0; i < Environment.ProcessorCount; i++)
                //{
                //    Thread t = new Thread(new ParameterizedThreadStart(CPUKill));
                //    t.Start(percentage);
                //    Over_threads.Add(t);
                //}
                for (int i = 0; i < 4; i++)
                {
                    //Result_Image0[i] = new Bitmap(640, 480);
                    //Capture_Image0[i] = new Bitmap(640, 480);
                    //Result_Image1[i] = new Bitmap(640, 480);
                    //Capture_Image1[i] = new Bitmap(640, 480);
                    //Result_Image2[i] = new Bitmap(640, 480);
                    //Capture_Image2[i] = new Bitmap(640, 480);
                    //Result_Image3[i] = new Bitmap(640, 480);
                    //Capture_Image3[i] = new Bitmap(640, 480);
                    //Capture_framebuffer[i] = new Queue();
                    //Result_framebuffer[i] = new Queue();
                    Capture_framebuffer[i] = new List<Bitmap>();
                    Result_framebuffer[i] = new List<Bitmap>();
                    LVApp.Instance().m_Config.Tx_Idx[i] = 0;
                    //timer_Cam[i] = new System.Windows.Forms.Timer();
                }

                ctr_Log1.Refresh_Log_Data();
                LVApp.Instance().m_Config.ds_LOG.Tables.Clear();
                LVApp.Instance().m_Config.ds_LOG.Clear();
                for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                {
                    LVApp.Instance().m_Config.m_Error_Flag[i] = -1;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3] = 0;
                    LVApp.Instance().m_Config.Initialize_Data_Log(i);
                    //LVApp.Instance().m_Config.m_Log_Data_Cnt[i] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[i] = 0;
                }

                LVApp.Instance().m_Config.t_Create_Save_Folders_oldtime = DateTime.Now;

                textBox_LOT.Text = Properties.Settings.Default.Lot_No;

                bool t_space_check = true;
                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                {
                    t_space_check = Check_HD_available(LVApp.Instance().excute_path);
                }
                else
                {
                    t_space_check = Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                }
                if (t_space_check)
                {
                    LVApp.Instance().SAVE_IMAGE_List[0].Clear();
                    LVApp.Instance().SAVE_IMAGE_List[1].Clear();
                    LVApp.Instance().SAVE_IMAGE_List[2].Clear();
                    LVApp.Instance().SAVE_IMAGE_List[3].Clear();
                    ImageSavethread = new Thread(ImageSavethread_Proc);
                    m_ImageSavethread_Check = true;
                    ImageSavethread.IsBackground = true;
                    ImageSavethread.Start();
                }

                timer_Refresh_Amount.Stop();
                Monitoringthread = new Thread(Monitoringthread_Proc);
                m_Monitoringthread_Check = true;
                Monitoringthread.IsBackground = true;
                Monitoringthread.Start();

                Inspection_Thread_Start();

                m_ViewThreads_Check[0] = m_ViewThreads_Check[1] = m_ViewThreads_Check[2] = m_ViewThreads_Check[3] = true;
                Viewthreads[0] = new Thread(ResultProc0); Viewthreads[0].IsBackground = true;
                Viewthreads[1] = new Thread(ResultProc1); Viewthreads[1].IsBackground = true;
                Viewthreads[2] = new Thread(ResultProc2); Viewthreads[2].IsBackground = true;
                Viewthreads[3] = new Thread(ResultProc3); Viewthreads[3].IsBackground = true;
                m_Result_Job_Mode0 = m_Result_Job_Mode1 = m_Result_Job_Mode2 = m_Result_Job_Mode3 = 0;
                Viewthreads[0].Start();
                Viewthreads[1].Start();
                Viewthreads[2].Start();
                Viewthreads[3].Start();
                LVApp.Instance().m_DIO.CAM0_Trigger_Completed += new System.EventHandler(MIL_CAM0_Grab);
                LVApp.Instance().m_DIO.CAM1_Trigger_Completed += new System.EventHandler(MIL_CAM1_Grab);
                LVApp.Instance().m_DIO.CAM2_Trigger_Completed += new System.EventHandler(MIL_CAM2_Grab);
                LVApp.Instance().m_DIO.CAM3_Trigger_Completed += new System.EventHandler(MIL_CAM3_Grab);
                //Auto_Start_when_Start();
                //LVApp.Instance().m_Config.Load_Judge_Data();
                //Thread.Sleep(500);
                //ctr_PLC1.btnOpen_Click(sender, e);
                //LVApp.Instance().m_Config.Set_Parameters();
                richTextBox_LOG.ResetText();
                splitContainer11.SplitterDistance = splitContainer16.SplitterDistance = splitContainer19.SplitterDistance = splitContainer21.SplitterDistance = 650;
            }
            catch
            { }
        }

        private void MIL_CAM0_Grab(object sender, EventArgs e)
        {
            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
            {
                LVApp.Instance().m_MIL.CAM0_Mil_Grab();
            }
        }
        private void MIL_CAM1_Grab(object sender, EventArgs e)
        {
            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
            {
                LVApp.Instance().m_MIL.CAM1_Mil_Grab();
            }
        }
        private void MIL_CAM2_Grab(object sender, EventArgs e)
        {
            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
            {
                LVApp.Instance().m_MIL.CAM2_Mil_Grab();
            }
        }
        private void MIL_CAM3_Grab(object sender, EventArgs e)
        {
            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
            {
                LVApp.Instance().m_MIL.CAM3_Mil_Grab();
            }
        }


        private int t_Auto_Start_CNT = 0;
        public bool t_Auto_Start_Check = false;
        public void Auto_Start_when_Start()
        {
            Frm_Main_SizeChanged(null, null);
            Camera_Connection_Check();
            if (LVApp.Instance().m_Config.m_Model_Name.Length <= 0)
            {
                button_INSPECTION.Enabled = true;
                t_Auto_Start_Check = true;
                m_Start_Button_Check = true;
                add_Log("No model!");
                return;
            }
            m_Start_Button_Check = true;
            ctr_PLC1.button_LOAD_Click(null, null);
            ctr_PLC1.btnOpen_Click(null, null);
            if (!Simulation_mode)
            {
                if (!t_DongleKey.Check_License())
                {
                    add_Log("No licence dongle key!");
                    button_INSPECTION.Enabled = true;
                    return;
                }


                bool t_CAM_Check = false;
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    if (LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["값"].ToString() == "정상 연결")
                    {
                        t_CAM_Check = true;
                    }
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    if (LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"].ToString() == "Conn.")
                    {
                        t_CAM_Check = true;
                    }
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    if (LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"].ToString() == "Conn.")
                    {
                        t_CAM_Check = true;
                    }
                }

                if (!Camera_Connectio_check_flag)
                {
                    t_CAM_Check = false;
                }
                if (!t_CAM_Check)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {
                        add_Log("카메라 연결을 점검하세요!");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {
                        add_Log("Check Camera connection!");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        add_Log("检查摄像机连接!");
                    }
                    button_INSPECTION.Enabled = true;
                    t_Auto_Start_Check = true;
                    m_Start_Button_Check = true;
                    return;
                }
            }

            LVApp.Instance().m_Config.Load_Judge_Data();

            this.Refresh();

            if (!Simulation_mode)
            {
                bool t_PLC_Check = false;
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    if (LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"].ToString() == "정상 연결")
                    {
                        t_PLC_Check = true;
                    }
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    if (LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"].ToString() == "Conn.")
                    {
                        t_PLC_Check = true;
                    }
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    if (LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"].ToString() == "Conn.")
                    {
                        t_PLC_Check = true;
                    }
                }

                if (!ctr_PLC1.btnClose.Enabled)
                {
                    t_PLC_Check = false;
                }

                if (!t_PLC_Check)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {
                        add_Log("PLC 연결을 점검하세요!");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {
                        add_Log("Check PLC connection!");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        add_Log("检查 PLC 连接!");
                    }
                    button_INSPECTION.Enabled = true;
                    t_Auto_Start_Check = true;
                    m_Start_Button_Check = true;
                    return;
                }
            }

            //button_INSPECTION_Click(null, null);
            button_INSPECTION.Enabled = true;
            t_Auto_Start_Check = true;
        }

        private int t_camera_refresh_count = 0;
        public void Monitoringthread_Proc()
        {
            try
            {
                while (m_Monitoringthread_Check)
                {
                    if (t_cam_ROI_view_mode)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    //DebugLogger.Instance().LogRecord("TEST");

                    if (t_Auto_Start_CNT == 14)
                    {
                        t_Auto_Start_CNT = -1;
                        //if (LVApp.Instance().m_Config.m_Model_Name.Length > 0 && Properties.Settings.Default.Last_Model_Name.Length > 0)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    Auto_Start_when_Start();
                                });
                            }
                            else
                            {
                                Auto_Start_when_Start();
                            }
                        }
                    }
                    else
                    {
                        if (t_Auto_Start_CNT >= 0)
                        {
                            t_Auto_Start_CNT++;
                        }
                    }

                    //CurrencyManager currencyManager0 = (CurrencyManager)dataGridView_AUTO_COUNT.BindingContext[dataGridView_AUTO_COUNT.DataSource];
                    // currencyManager0.SuspendBinding();

                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && !Simulation_mode)
                    //{
                    //    double t_val_cnt = 0; double t_min_cnt = 9999999999999; double t_max_cnt = 0;
                    //    int t_min_idx = -1;
                    //    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    //    {
                    //        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //        {
                    //            t_val_cnt = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //            //if (double.TryParse(LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"].ToString(), out t_val_cnt))
                    //            {
                    //                if (t_val_cnt >= 0 && t_val_cnt <= t_min_cnt)
                    //                {
                    //                    t_min_cnt = t_val_cnt;
                    //                    t_min_idx = i;
                    //                }
                    //                if (t_val_cnt >= 0 && t_val_cnt >= t_max_cnt)
                    //                {
                    //                    t_max_cnt = t_val_cnt;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    if (t_min_idx > -1 && Math.Abs(t_max_cnt - t_min_cnt) >= LVApp.Instance().m_Config.CAM_Refresh_CNT && LVApp.Instance().m_Config.CAM_Refresh_CNT > 0)
                    //    {
                    //        if (t_min_idx == 0 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                    //        {
                    //            ctr_Camera_Setting1.toolStripButtonInitialize_Click(null, null);
                    //            //Thread.Sleep(100);
                    //            ctr_Camera_Setting1.toolStripButtonContinuousShot_Click(null, null);
                    //        }
                    //        else if (t_min_idx == 1 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                    //        {
                    //            ctr_Camera_Setting2.toolStripButtonInitialize_Click(null, null);
                    //            //Thread.Sleep(100);
                    //            ctr_Camera_Setting2.toolStripButtonContinuousShot_Click(null, null);
                    //        }
                    //        else if (t_min_idx == 2 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                    //        {
                    //            ctr_Camera_Setting3.toolStripButtonInitialize_Click(null, null);
                    //            //Thread.Sleep(100);
                    //            ctr_Camera_Setting3.toolStripButtonContinuousShot_Click(null, null);
                    //        }
                    //        else if (t_min_idx == 3 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                    //        {
                    //            ctr_Camera_Setting4.toolStripButtonInitialize_Click(null, null);
                    //            //Thread.Sleep(100);
                    //            ctr_Camera_Setting4.toolStripButtonContinuousShot_Click(null, null);
                    //        }
                    //        LVApp.Instance().m_Config.m_OK_NG_Cnt[t_min_idx, 1] += Math.Abs(t_max_cnt - t_min_cnt);
                    //    }
                    //for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    //{
                    //    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //    {


                    //    }
                    //}

                    //}

                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        t_camera_refresh_count++;

                        if (t_camera_refresh_count > 20)
                        {
                            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1]
                                    && LVApp.Instance().m_mainform.ctr_Camera_Setting1.sliderWidth.slider.Enabled)
                                {
                                    LVApp.Instance().m_mainform.add_Log("CAM0 Restart!");
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonStop_Click(null, null);
                                    Thread.Sleep(100);
                                    ctr_PLC1.send_Message[0].Clear();
                                    ctr_PLC1.send_Idx[0].Clear();
                                    ctr_PLC1.send_Message_Time[0].Clear();
                                    ctr_PLC1.send_Idx_Time[0].Clear();
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonContinuousShot_Click(null, null);
                                }
                            }
                            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2]
                                    && LVApp.Instance().m_mainform.ctr_Camera_Setting2.sliderWidth.slider.Enabled)
                                {
                                    LVApp.Instance().m_mainform.add_Log("CAM1 Restart!");
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonStop_Click(null, null);
                                    Thread.Sleep(100);
                                    ctr_PLC1.send_Message[1].Clear();
                                    ctr_PLC1.send_Idx[1].Clear();
                                    ctr_PLC1.send_Message_Time[1].Clear();
                                    ctr_PLC1.send_Idx_Time[1].Clear();
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonContinuousShot_Click(null, null);
                                }
                            }
                            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3]
                                    && LVApp.Instance().m_mainform.ctr_Camera_Setting3.sliderWidth.slider.Enabled)
                                {
                                    LVApp.Instance().m_mainform.add_Log("CAM2 Restart!");
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonStop_Click(null, null);
                                    Thread.Sleep(100);
                                    ctr_PLC1.send_Message[2].Clear();
                                    ctr_PLC1.send_Idx[2].Clear();
                                    ctr_PLC1.send_Message_Time[2].Clear();
                                    ctr_PLC1.send_Idx_Time[2].Clear();
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonContinuousShot_Click(null, null);
                                }
                            }
                            if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4]
                                    && LVApp.Instance().m_mainform.ctr_Camera_Setting4.sliderWidth.slider.Enabled)
                                {
                                    LVApp.Instance().m_mainform.add_Log("CAM3 Restart!");
                                    LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonStop_Click(null, null);
                                    Thread.Sleep(100);
                                    ctr_PLC1.send_Message[3].Clear();
                                    ctr_PLC1.send_Idx[3].Clear();
                                    ctr_PLC1.send_Message_Time[3].Clear();
                                    ctr_PLC1.send_Idx_Time[3].Clear();

                                    LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonContinuousShot_Click(null, null);
                                }
                            }

                            t_camera_refresh_count = 16;
                        }
                    }
                    else
                    {
                        t_camera_refresh_count = 0;
                    }

                    //if (this.InvokeRequired)
                    //{
                    //    this.Invoke((MethodInvoker)delegate
                    //    {
                    //for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i][2] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i][1] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 1][2] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3];
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][1] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3];
                            if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][2] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] /*+ LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3]*/)).ToString("0.0");
                            }
                            else
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][2] = 0;
                            }
                            if (LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows.Count > 0)
                            {
                                LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i][2];//검사수량
                                LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i][1];//양품수량
                                LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][1];//불량수량
                                LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][2];//수율
                            }
                        }

                        if (!m_ViewThreads_Check[i] && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i] && Viewthreads[i] != null)
                        {
                            if (Viewthreads[i].IsAlive)
                            {
                                Viewthreads[i].Abort();
                                Viewthreads[i] = null;
                            }
                            if (i == 0)
                            {
                                m_Result_Job_Mode0 = 0;
                                Viewthreads[i] = new Thread(ResultProc0);
                            }
                            if (i == 1)
                            {
                                m_Result_Job_Mode1 = 0;
                                Viewthreads[i] = new Thread(ResultProc1);
                            }
                            if (i == 2)
                            {
                                m_Result_Job_Mode2 = 0;
                                Viewthreads[i] = new Thread(ResultProc2);
                            }
                            if (i == 3)
                            {
                                m_Result_Job_Mode3 = 0;
                                Viewthreads[i] = new Thread(ResultProc3);
                            }
                            m_ViewThreads_Check[i] = true;
                            Viewthreads[i].IsBackground = true;
                            Viewthreads[i].Start();
                            add_Log("CAM" + i.ToString() + " View Thread Restart");
                            if (Result_framebuffer[i].Count > 0)
                            {
                                Result_framebuffer[i].Clear();
                            }
                        }

                        if (!m_Threads_Check[i] && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i] && LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (threads[i].IsAlive)
                            {
                                threads[i].Abort();
                                threads[i] = null;
                            }
                            if (i == 0)
                            {
                                m_Job_Mode0 = 0;
                                threads[i] = new Thread(ThreadProc0);
                            }
                            if (i == 1)
                            {
                                m_Job_Mode1 = 0;
                                threads[i] = new Thread(ThreadProc1);
                            }
                            if (i == 2)
                            {
                                m_Job_Mode2 = 0;
                                threads[i] = new Thread(ThreadProc2);
                            }
                            if (i == 3)
                            {
                                m_Job_Mode3 = 0;
                                threads[i] = new Thread(ThreadProc3);
                            }
                            m_Threads_Check[i] = true;
                            threads[i].IsBackground = true;
                            threads[i].Start();

                            add_Log("CAM" + i.ToString() + " Inspection Thread Restart");
                            if (Capture_framebuffer[i].Count > 0)
                            {
                                Capture_framebuffer[i].Clear();
                            }
                        }
                    }

                    if (!m_ImageSavethread_Check)
                    {
                        Thread.Sleep(10);
                        if (ImageSavethread != null)
                        {
                            if (ImageSavethread.IsAlive)
                            {
                                ImageSavethread.Abort();
                                ImageSavethread = null;
                            }
                        }

                        LVApp.Instance().SAVE_IMAGE_List[0].Clear();
                        LVApp.Instance().SAVE_IMAGE_List[1].Clear();
                        LVApp.Instance().SAVE_IMAGE_List[2].Clear();
                        LVApp.Instance().SAVE_IMAGE_List[3].Clear();
                        bool t_space_check = true;
                        if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                        {
                            t_space_check = Check_HD_available(LVApp.Instance().excute_path);
                        }
                        else
                        {
                            t_space_check = Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                        }
                        if (!t_space_check)
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {
                                add_Log("이미지 저장공간 부족!");
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                            {
                                add_Log("Not enough space to save!");
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                            {
                                add_Log("图片存储空间不足!");
                            }
                        }
                        else
                        {
                            LVApp.Instance().m_Config.Create_Save_Folders();
                            //Thread.Sleep(10);
                            ImageSavethread = null;
                            ImageSavethread = new Thread(ImageSavethread_Proc);
                            m_ImageSavethread_Check = true;
                            ImageSavethread.IsBackground = true;
                            ImageSavethread.Start();
                        }
                    }
                    //{
                    //    int i = 0;
                    //    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //        }
                    //        else
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //        }
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //    }
                    //    i = 1;
                    //    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //        }
                    //        else
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //        }
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //    }
                    //    i = 2;
                    //    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //        }
                    //        else
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //        }
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //    }
                    //    i = 3;
                    //    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //        if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //        }
                    //        else
                    //        {
                    //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //        }
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //        LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //    }
                    //}
                    //currencyManager0.ResumeBinding();

                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        bool t_space_check = true;
                        if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                        {
                            t_space_check = Check_HD_available(LVApp.Instance().excute_path);
                        }
                        else
                        {
                            t_space_check = Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                        }

                        if (!m_ImageSavethread_Check)
                        {
                            if (t_space_check)
                            {
                                LVApp.Instance().SAVE_IMAGE_List[0].Clear();
                                LVApp.Instance().SAVE_IMAGE_List[1].Clear();
                                LVApp.Instance().SAVE_IMAGE_List[2].Clear();
                                LVApp.Instance().SAVE_IMAGE_List[3].Clear();
                                LVApp.Instance().m_Config.Create_Save_Folders();
                                if (ImageSavethread != null)
                                {
                                    ImageSavethread = null;
                                }
                                ImageSavethread = new Thread(ImageSavethread_Proc);
                                m_ImageSavethread_Check = true;
                                ImageSavethread.IsBackground = true;
                                ImageSavethread.Start();
                                add_Log("Image save thread restart!");
                            }
                        }

                        if (t_space_check)
                        {
                            String m_Log_folder = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                            if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                            {
                                m_Log_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                            }

                            DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                            // 폴더가 존재하지 않으면
                            if (dir.Exists == false)
                            {
                                LVApp.Instance().m_Config.Create_Save_Folders();
                                t_csv_init_check = 2;
                            }

                            if (t_csv_init_check == 2)
                            {
                                for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                                {
                                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                    {

                                        String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + i.ToString() + ".csv"; //파일경로
                                        if (LVApp.Instance().m_Config.m_Log_Save_Folder != "")
                                        {
                                            m_Log_File_Name = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + i.ToString() + ".csv"; //파일경로
                                        }

                                        FileInfo templateFile = new FileInfo(m_Log_File_Name);

                                        if (!templateFile.Exists)
                                        {
                                            //LVApp.Instance().m_Config.t_try_cnt = 0;

                                            //if (this.InvokeRequired)
                                            //{
                                            //    this.Invoke((MethodInvoker)delegate
                                            //    {
                                            //        LVApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                                            //    });
                                            //}
                                            //else
                                            //{
                                            LVApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                                            //}
                                        }
                                    }
                                }
                                //if (this.InvokeRequired)
                                //{
                                //    this.Invoke((MethodInvoker)delegate
                                //    {
                                //        LVApp.Instance().m_Config.CSV_Logfile_Initialize(4);
                                //    });
                                //}
                                //else
                                //{
                                LVApp.Instance().m_Config.CSV_Logfile_Initialize(4);
                                //}
                            }
                            t_csv_init_check--;
                            if (t_csv_init_check < 0)
                            {
                                t_csv_init_check = 0;
                            }
                        }
                    }
                    //if (t_refresh_count == 1 || t_refresh_count == 4)
                    //{
                    //    //GC.Collect();
                    //}
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (t_refresh_count == 0 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                        {
                            LVApp.Instance().m_mainform.ctr_DataGrid1.Min_Max_Update(t_refresh_count);
                        }
                        else if (t_refresh_count == 1 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                        {
                            LVApp.Instance().m_mainform.ctr_DataGrid2.Min_Max_Update(t_refresh_count);
                        }
                        else if (t_refresh_count == 2 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                        {
                            LVApp.Instance().m_mainform.ctr_DataGrid3.Min_Max_Update(t_refresh_count);
                        }
                        else if (t_refresh_count == 3 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                        {
                            LVApp.Instance().m_mainform.ctr_DataGrid4.Min_Max_Update(t_refresh_count);
                        }
                    }
                    t_refresh_count++;
                    t_refresh_count %= 5;

                    if (t_pooling_idx == 2)
                    {
                        ctr_PLC1.poolingforconnection();
                        t_pooling_idx = 0;
                    }
                    else
                    {
                        t_pooling_idx++;
                    }


                    //int t_day = Math.Abs(DateTime.Now.DayOfYear - LVApp.Instance().m_Config.t_Create_Save_Folders_oldtime.DayOfYear);
                    //if (t_day >= 1)
                    //{
                    //    LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
                    //    LVApp.Instance().m_Config.t_Create_Save_Folders_oldtime = DateTime.Now;
                    //}
                    if (Simulation_mode)
                    {
                        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            //LVApp.Instance().m_Config.Initialize_Data_Log();

                            if (!ctr_Camera_Setting1.Force_USE.Checked && ctr_ROI1.pictureBox_Image.Image != null)
                            {
                                if (ctr_PLC1.m_Protocal == (int)Control.Ctr_PLC.PROTOCAL.IPSBoard)
                                {
                                    lock (ctr_PLC1.send_Idx_Time[0])
                                    {
                                        ctr_PLC1.send_Idx_Time[0].Add(DateTime.Now);
                                    }
                                    lock (ctr_PLC1.send_Idx[0])
                                    {
                                        ctr_PLC1.send_Idx[0].Add((uint)ctr_Camera_Setting1.Grab_Num);
                                    }
                                }
                                Bitmap t_Image = null;
                                if (ctr_ROI1.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb
                                    || ctr_ROI1.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppPArgb)
                                {
                                    t_Image = ConvertTo24(GetCopyOf0((Bitmap)ctr_ROI1.pictureBox_Image.Image));
                                }
                                else
                                {
                                    t_Image = GetCopyOf0((Bitmap)ctr_ROI1.pictureBox_Image.Image);
                                }
                                //lock (ctrCam1.m_bitmap)
                                {
                                    ctrCam1.m_bitmap = t_Image;
                                }
                                ctrCam1_GrabComplete(null, null);
                            }
                            if (!ctr_Camera_Setting2.Force_USE.Checked && ctr_ROI2.pictureBox_Image.Image != null)
                            {
                                if (ctr_PLC1.m_Protocal == (int)Control.Ctr_PLC.PROTOCAL.IPSBoard)
                                {
                                    lock (ctr_PLC1.send_Idx_Time[1])
                                    {
                                        ctr_PLC1.send_Idx_Time[1].Add(DateTime.Now);
                                    }
                                    lock (ctr_PLC1.send_Idx[1])
                                    {
                                        ctr_PLC1.send_Idx[1].Add((uint)ctr_Camera_Setting2.Grab_Num);
                                    }
                                }
                                Bitmap t_Image = null;
                                if (ctr_ROI2.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb
                                    || ctr_ROI2.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppPArgb)
                                {
                                    t_Image = ConvertTo24(GetCopyOf1((Bitmap)ctr_ROI2.pictureBox_Image.Image));
                                }
                                else
                                {
                                    t_Image = GetCopyOf1((Bitmap)ctr_ROI2.pictureBox_Image.Image);
                                }
                                //lock (ctrCam2.m_bitmap)
                                {
                                    ctrCam2.m_bitmap = t_Image;
                                }
                                ctrCam2_GrabComplete(null, null);
                            }
                            if (!ctr_Camera_Setting3.Force_USE.Checked && ctr_ROI3.pictureBox_Image.Image != null)
                            {
                                if (ctr_PLC1.m_Protocal == (int)Control.Ctr_PLC.PROTOCAL.IPSBoard)
                                {
                                    lock (ctr_PLC1.send_Idx_Time[2])
                                    {
                                        ctr_PLC1.send_Idx_Time[2].Add(DateTime.Now);
                                    }
                                    lock (ctr_PLC1.send_Idx[2])
                                    {
                                        ctr_PLC1.send_Idx[2].Add((uint)ctr_Camera_Setting3.Grab_Num);
                                    }
                                }
                                Bitmap t_Image = null;
                                if (ctr_ROI3.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb
                                    || ctr_ROI3.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppPArgb)
                                {
                                    t_Image = ConvertTo24(GetCopyOf2((Bitmap)ctr_ROI3.pictureBox_Image.Image));
                                }
                                else
                                {
                                    t_Image = GetCopyOf2((Bitmap)ctr_ROI3.pictureBox_Image.Image);
                                }
                                //lock (ctrCam3.m_bitmap)
                                {
                                    ctrCam3.m_bitmap = t_Image;
                                }
                                ctrCam3_GrabComplete(null, null);
                            }
                            if (!ctr_Camera_Setting4.Force_USE.Checked && ctr_ROI4.pictureBox_Image.Image != null)
                            {
                                if (ctr_PLC1.m_Protocal == (int)Control.Ctr_PLC.PROTOCAL.IPSBoard)
                                {
                                    lock (ctr_PLC1.send_Idx_Time[3])
                                    {
                                        ctr_PLC1.send_Idx_Time[3].Add(DateTime.Now);
                                    }
                                    lock (ctr_PLC1.send_Idx[3])
                                    {
                                        ctr_PLC1.send_Idx[3].Add((uint)ctr_Camera_Setting4.Grab_Num);
                                    }
                                }
                                Bitmap t_Image = null;
                                if (ctr_ROI4.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb
                                    || ctr_ROI4.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppPArgb)
                                {
                                    t_Image = ConvertTo24(GetCopyOf3((Bitmap)ctr_ROI4.pictureBox_Image.Image));
                                }
                                else
                                {
                                    t_Image = GetCopyOf3((Bitmap)ctr_ROI4.pictureBox_Image.Image);
                                }
                                //lock (ctrCam4.m_bitmap)
                                {
                                    ctrCam4.m_bitmap = t_Image;
                                }
                                ctrCam4_GrabComplete(null, null);
                            }
                        }
                    }

                    if (!m_Monitoringthread_Check)
                    {
                        break;
                    }
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && Simulation_mode)
                    {
                        Thread.Sleep(1000 / 2);
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                    GC.Collect();
                }
            }
            catch (System.Exception ex)
            {
                add_Log(ex.ToString());
            }
        }

        public void ImageSavethread_Proc()
        {
            while (m_ImageSavethread_Check)
            {
                int t_sum = 0;
                for (int s = 0; s < 4; s++)
                {
                    int t_cnt = LVApp.Instance().SAVE_IMAGE_List[s].Count;
                    if (t_cnt > 0)
                    {
                        for (int i = 0; i < t_cnt; i++)
                        {
                            lock (LVApp.Instance().SAVE_IMAGE_List[s])
                            {
                                Save_Image_List(s);
                                LVApp.Instance().SAVE_IMAGE_List[s].RemoveAt(0);
                            }
                            Thread.Sleep(1);
                        }
                    }
                    t_sum += t_cnt;
                }
                if (t_sum == 0)
                {
                    Thread.Sleep(1);
                }
                if (!m_ImageSavethread_Check)
                {
                    break;
                }
            }
        }

        delegate void MyDelegate();      //델리게이트 선언(크로스 쓰레드 해결하기 위한 용도)
        private void Save_Image_List(int Cam_num)
        {
            try
            {
                //MyDelegate _dt = delegate()
                {
                    //string fn = "";
                    DateTime dt = DateTime.Now;
                    // MM_DD_YYYY_HH_MM_SS.LOG
                    string fn = dt.Year.ToString("0000") + "_" + dt.Month.ToString("00") + "_" + dt.Day.ToString("00") + "";
                    //fn += "_" + dt.Month.ToString("00");
                    //fn += "_" + dt.Day.ToString("00") + "";

                    //if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                    //{
                    //    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[0]._Cam_num.ToString());
                    //    // 폴더가 존재하지 않으면
                    //    if (dir.Exists == false)
                    //    {
                    //        return;
                    //    }
                    //}
                    //else
                    //{
                    //    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[0]._Cam_num.ToString());
                    //    // 폴더가 존재하지 않으면
                    //    if (dir.Exists == false)
                    //    {
                    //        // 새로 생성합니다.
                    //        return;
                    //    }
                    //}
                    //lock (LVApp.Instance().SAVE_IMAGE_List[Cam_num])
                    {
                        if (LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._OK_NG_NONE_Flag == 0 && (LVApp.Instance().m_Config.m_Cam_Log_Method == 0 || LVApp.Instance().m_Config.m_Cam_Log_Method >= 2)) // OK 저장
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Format == 0)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 1)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 2)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 3)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }
                        else if (LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._OK_NG_NONE_Flag == 1 && (LVApp.Instance().m_Config.m_Cam_Log_Method == 1 || LVApp.Instance().m_Config.m_Cam_Log_Method == 2)) // NG 저장
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Format == 0)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 1)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 2)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 3)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }
                        else if (LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._OK_NG_NONE_Flag == 2 && (LVApp.Instance().m_Config.m_Cam_Log_Method == 1 || LVApp.Instance().m_Config.m_Cam_Log_Method == 2)) // No Object 저장
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Format == 0)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 1)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 2)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Log_Format == 3)
                            {
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                                {
                                    string filename = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                else
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 != "")
                                {
                                    string filename = LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NO Object\\#" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                    LVApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }
                        //Thread.Sleep(5);
                    }
                }
                //if (LVApp.Instance().SAVE_IMAGE_List.Count > 4)
                //{
                //    LVApp.Instance().SAVE_IMAGE_List.RemoveAt(0);
                //}
                //this.Invoke(_dt);
            }
            catch
            {
                add_Log("Image save error! 저장중지");
                m_ImageSavethread_Check = false;
                //Thread.Sleep(10);
                //if (ImageSavethread != null && ImageSavethread.IsAlive)
                //{
                //    ImageSavethread.Abort();
                //    ImageSavethread = null;
                //}

                //LVApp.Instance().SAVE_IMAGE_List[0].Clear();
                //LVApp.Instance().SAVE_IMAGE_List[1].Clear();
                //LVApp.Instance().SAVE_IMAGE_List[2].Clear();
                //LVApp.Instance().SAVE_IMAGE_List[3].Clear();
                //bool t_space_check = true;
                //if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                //{
                //    t_space_check = Check_HD_available(LVApp.Instance().excute_path);
                //}
                //else
                //{
                //    t_space_check = Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                //}
                //if (!t_space_check)
                //{
                //    add_Log("이미지 저장공간 부족!");
                //    return;
                //}
                //else
                //{
                //    LVApp.Instance().m_Config.Create_Save_Folders();
                //}
                ////Thread.Sleep(10);
                //ImageSavethread = null;
                //ImageSavethread = new Thread(ImageSavethread_Proc);
                //m_ImageSavethread_Check = true;
                //ImageSavethread.IsBackground = true;
                //ImageSavethread.Start();
            }
        }

        //List<Thread> Over_threads = new List<Thread>();

        //public static void CPUKill(object cpuUsage)
        //{
        //    System.Threading.Tasks.Parallel.For(0, 1, new Action<int>((int i) =>
        //    {
        //        Stopwatch watch = new Stopwatch();
        //        watch.Start();
        //        while (true)
        //        {
        //            if (watch.ElapsedMilliseconds > (int)cpuUsage)
        //            {
        //                Thread.Sleep(100 - (int)cpuUsage);
        //                watch.Reset();
        //                watch.Start();
        //            }
        //        }
        //    }));

        //}

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rectangle rectangle);

        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Force_close)
            {
                e.Cancel = false;
            }
            else
            {
                //if (LVApp.Instance().m_MIL.CAM0_Grabbing || LVApp.Instance().m_MIL.CAM1_Grabbing || LVApp.Instance().m_MIL.CAM2_Grabbing || LVApp.Instance().m_MIL.CAM3_Grabbing)
                //{
                //    e.Cancel = true;
                //    return;
                //}

                if (MessageBox.Show("Do you want to exit?", " Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (m_Language == 0)
                            {
                                AutoClosingMessageBox.Show("검사중 프로그램을 종료 할 수 없습니다.", "Warning", 2000);
                            }
                            else
                            {
                                AutoClosingMessageBox.Show("Can't exit the program on inspection.", "Warning", 2000);
                            }
                            e.Cancel = true;
                            return;
                        }

                        #region LHJ - 240804 - 디버깅용
                        // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                        // 알고리즘을 다운 시키는 이미지가 있는지 확인
                        // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                        System.DateTime dateTime = DateTime.Now;

                        DirectoryInfo lastImagePath = new DirectoryInfo($"{LVApp.Instance().excute_path}\\Logs\\{LVApp.Instance().m_Config.m_Model_Name}\\{dateTime:yyyy}\\{dateTime:yyyy-MM}\\{dateTime:yyyy-MM-dd}\\{dateTime:yyyy-MM-dd-HH-mm-ss-ff}");
                        if (!lastImagePath.Exists)
                        {
                            lastImagePath.Create();
                        }

                        for (int i = 0; i < LV_Config._lastImageCount; ++i)
                        {
                            LVApp.Instance().m_Config._lastImage_Cam0[i]?.Save($"{lastImagePath}\\Cam0_{i}.bmp");
                            LVApp.Instance().m_Config._lastImage_Cam1[i]?.Save($"{lastImagePath}\\Cam1_{i}.bmp");
                            LVApp.Instance().m_Config._lastImage_Cam2[i]?.Save($"{lastImagePath}\\Cam2_{i}.bmp");
                            LVApp.Instance().m_Config._lastImage_Cam3[i]?.Save($"{lastImagePath}\\Cam3_{i}.bmp");
                        }
                        #endregion

                        m_Monitoringthread_Check = false;
                        Thread.Sleep(500);
                        if (Monitoringthread.IsAlive)
                        {
                            Monitoringthread.Abort();
                            Monitoringthread = null;
                        }

                        //foreach (var t in Over_threads)
                        //{
                        //    t.Abort();
                        //}
                        Force_close = true;

                        if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                        {
                            ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
                            ctr_Camera_Setting1.toolStripButtonDisconnect_Click(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                        {
                            ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
                            ctr_Camera_Setting2.toolStripButtonDisconnect_Click(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                        {
                            ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
                            ctr_Camera_Setting3.toolStripButtonDisconnect_Click(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                        {
                            ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
                            ctr_Camera_Setting4.toolStripButtonDisconnect_Click(sender, e);
                        }
                        Thread.Sleep(200);
                        //LVApp.Instance().t_QuickMenu.Hide();
                        //LVApp.Instance().t_QuickMenu.Close();

                        DebugLogger.Instance().LogRecord("Closing the program!");

                        //Properties.Settings.Default.Split_dist = ctr_Display_1.splitContainer1.SplitterDistance;
                        //Properties.Settings.Default.Save();

                        Inspection_Thread_Stop();

                        LVApp.Instance().m_Config.CSV_Logfile_Terminate();

                        for (int i = 0; i < 4; i++)
                        {
                            m_ViewThreads_Check[i] = false;
                            //threads[i].Abort();
                            Thread.Sleep(100);
                            if (Viewthreads[i].IsAlive)
                            {
                                Viewthreads[i].Abort();
                            }
                            //Probe_threads[i].Abort();
                        }
                        if (m_ImageSavethread_Check)
                        {
                            m_ImageSavethread_Check = false;
                            Thread.Sleep(100);
                            if (ImageSavethread.IsAlive)
                            {
                                ImageSavethread.Abort();
                            }
                        }

                        //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        //{
                        //    button_INSPECTION_Click(sender, e);
                        //}




                        //ctr_Camera_Setting5.toolStripButtonDisconnect_Click(sender, e);
                        //ctr_Camera_Setting6.toolStripButtonDisconnect_Click(sender, e);
                        //ctr_Camera_Setting7.toolStripButtonDisconnect_Click(sender, e);
                        //ctr_Camera_Setting8.toolStripButtonDisconnect_Click(sender, e);


                        Process[] arrayProgram = Process.GetProcesses();
                        for (int i = 0; i < arrayProgram.Length; i++)
                        {
                            if (arrayProgram[i].ProcessName.Equals("Data_Display"))
                            {
                                Rectangle t_R = new Rectangle();
                                GetWindowRect(arrayProgram[i].MainWindowHandle, ref t_R);
                                System.Drawing.Point t_P = new System.Drawing.Point(t_R.Left, t_R.Top);
                                Properties.Settings.Default.DP_Location = t_P;
                                arrayProgram[i].Kill();
                            }
                        }

                        Properties.Settings.Default.Save();

                        if (LVApp.Instance().m_Config.m_Model_Name != "")
                        {
                            //ctr_Model1.cmdSave_Click(sender, e);
                            //if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                            //{
                            //    ctr_Camera_Setting1.toolStripButton_SAVE_Click(sender, e);
                            //    LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                            //    ctr_ROI1.button_SAVE_Click(sender, e);
                            //}
                            //if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                            //{
                            //    ctr_Camera_Setting2.toolStripButton_SAVE_Click(sender, e);
                            //    LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                            //    ctr_ROI2.button_SAVE_Click(sender, e);
                            //}
                            //if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                            //{
                            //    ctr_Camera_Setting3.toolStripButton_SAVE_Click(sender, e);
                            //    LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                            //    ctr_ROI3.button_SAVE_Click(sender, e);
                            //}
                            //if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                            //{
                            //    ctr_Camera_Setting4.toolStripButton_SAVE_Click(sender, e);
                            //    LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                            //    ctr_ROI4.button_SAVE_Click(sender, e);
                            //}
                        }

                        //ctr_Camera_Setting5.toolStripButton_SAVE_Click(sender, e);
                        //ctr_Camera_Setting6.toolStripButton_SAVE_Click(sender, e);
                        //ctr_Camera_Setting7.toolStripButton_SAVE_Click(sender, e);
                        //ctr_Camera_Setting8.toolStripButton_SAVE_Click(sender, e);

                        ctr_PLC1.btnClose_Click(sender, e);

                        //foreach (Form form in Application.OpenForms)
                        //{
                        //    if (form.GetType() == typeof(Frm_Trackbar))
                        //    {
                        //        form.Close();
                        //    }
                        //}

                        //LVApp.Instance().m_AI_Pro.Terminate_Env();
                        //LVApp.Instance().m_Ctr_Mysql.DB_disconnect();

                        e.Cancel = false;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }

        private bool Program_Start_Check()
        {
            Process[] arrayProgram = Process.GetProcesses();
            int cnt = 0;
            for (int i = 0; i < arrayProgram.Length; i++)
            {
                if (arrayProgram[i].ProcessName.Equals("LVInspector"))
                {
                    cnt++;
                }
            }
            if (cnt > 1)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("1개 이상의 동일한 프로그램이 실행 중입니다.!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("Program is running!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    MessageBox.Show("程序正在运行!");
                }

                //Process[] ps = Process.GetProcessesByName("PCB Align SW");
                //if (ps.Length != 0)
                //{
                //    ps[0].Kill();
                //}
                Force_close = true;
                this.Close();
                return false;
            }
            return true;
        }

        void LoggerStatusEvent(Logger o, eLogStatus status, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate () { LoggerStatusEvent(o, status, message); }));
            }
            else
            {
                switch (status)
                {
                    case eLogStatus.eLogWroteRecord:
                        AddtoLog(message);
                        break;
                }
            }
        }

        private void AddtoLog(string message)
        {
            //ctr_LogView1.txtLog.Text += message + "\r\n";
            //ctr_LogView1.txtLog.SelectionStart = ctr_LogView1.txtLog.Text.Length;
            //ctr_LogView1.txtLog.ScrollToCaret();
            //ctr_LogView1.txtLog.Refresh();

            try
            {
                //DebugLogger.Instance().LogRecord(str);
                if (ctr_LogView1.txtLog.InvokeRequired)
                {
                    ctr_LogView1.txtLog.Invoke((MethodInvoker)delegate
                    {
                        if (ctr_LogView1.txtLog.Lines.Length > 60)
                        {
                            int totalCharacters = ctr_LogView1.txtLog.Text.Trim().Length;
                            int totalLines = ctr_LogView1.txtLog.Lines.Length;
                            string lastLine = ctr_LogView1.txtLog.Lines[totalLines - 1] + "\n";
                            string copyOfLastLine = ctr_LogView1.txtLog.Lines[totalLines - 1];
                            if (totalLines > 1)
                            {
                                string newstring = ctr_LogView1.txtLog.Text.Substring(0, totalCharacters - lastLine.Length);
                                ctr_LogView1.txtLog.Text = newstring;
                            }
                            else
                            {
                                ctr_LogView1.txtLog.Text = "";
                            }                        //richTextBox_Main.ResetText();
                        }
                        string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message + "\r\n" + ctr_LogView1.txtLog.Text;
                        ctr_LogView1.txtLog.Text = display_str;
                    });
                }
                else
                {
                    if (ctr_LogView1.txtLog.Lines.Length > 60)
                    {
                        int totalCharacters = ctr_LogView1.txtLog.Text.Trim().Length;
                        int totalLines = ctr_LogView1.txtLog.Lines.Length;
                        string lastLine = ctr_LogView1.txtLog.Lines[totalLines - 1] + "\n";
                        string copyOfLastLine = ctr_LogView1.txtLog.Lines[totalLines - 1];
                        if (totalLines > 1)
                        {
                            string newstring = ctr_LogView1.txtLog.Text.Substring(0, totalCharacters - lastLine.Length);
                            ctr_LogView1.txtLog.Text = newstring;
                        }
                        else
                        {
                            ctr_LogView1.txtLog.Text = "";
                        }                        //richTextBox_Main.ResetText();
                    }
                    string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message + "\r\n" + ctr_LogView1.txtLog.Text;
                    ctr_LogView1.txtLog.Text = display_str;
                }
            }
            catch
            {
            }
        }

        private void GUI_Initialize()
        {
            digitalClockCtrl1.SetDigitalColor = SriClocks.DigitalColor.GreenColor;
            //this.MenuIcon = LV_Inspection_System.Properties.Resources.LV.GetThumbnailImage(24, 14, null, IntPtr.Zero);
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(8));

            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(250, ctr_Model1.ctr_History1.SW_Version));// + LVApp.Instance().m_Ctr_Model_Setting.ctr_History1.SW_Version));
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(100, "Ready"));
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(this.Width - 388, "(주)러닝비전 Co., Ltd. / 대구광역시 달서구 성서공단로 11길 62 대구R&D융합센터 732호, 427713 / 010-9212-7088 / kjlee@learningvision.co.kr      "));
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(this.Width - 388, "LearningVision Co., Ltd. / No.732, 62, Seongseogongdan-ro 11-gil, Dalseo-gu, Daegu, Republic of Korea, 427713 / 010-9212-7088 / kjlee@learningvision.co.kr      "));
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(this.Width - 388, "LearningVision Co., Ltd. / No.732, 62, Seongseogongdan-ro 11-gil, Dalseo-gu, Daegu, Republic of Korea, 427713 / 010-9212-7088 / kjlee@learningvision.co.kr      "));
            }
            this.StatusBar.EllipticalGlow = false;
            this.StatusBar.BarItems[1].ItemTextAlign = StringAlignment.Center;
            this.StatusBar.BarItems[2].ItemTextAlign = StringAlignment.Center;
            this.StatusBar.BarItems[3].ItemTextAlign = StringAlignment.Far;
            this.StatusBar.BarItems[1].BarItemFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Italic);
            this.StatusBar.BarItems[2].BarItemFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Regular);
            this.StatusBar.BarItems[3].BarItemFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Regular);
            this.TitleBar.TitleBarCaption = "LV Inspection System";
            this.TitleBar.TitleBarCaptionFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Bold);

            // 메인 디자인 Theme 로드를 위한 클래스
            XmlThemeLoader xtl = new XmlThemeLoader();
            xtl.ThemeForm = this;
            xtl.ApplyTheme(Path.Combine(Environment.CurrentDirectory, @"Theme.xml"));

            // 상단 아이콘

            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(LV_Inspection_System.Properties.Resources._1.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen1"));
            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(LV_Inspection_System.Properties.Resources._2.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen2"));
            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(LV_Inspection_System.Properties.Resources._3.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen3"));
            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(LV_Inspection_System.Properties.Resources._4.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen4"));

            //this.XCoolFormHolderButtonClick += new XCoolFormHolderButtonClickHandler(HolderButtonClick);

            //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
            neoTabWindow_MAIN.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("CCleanerRenderer");
            //neoTabWindow_MAIN.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("AvastRendererVS2");
            neoTabWindow_INSP_SETTING.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("MenuBarRenderer");
            neoTabWindow_ALG.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("AvastRendererVS3");
            neoTabWindow_INSP_SETTING_CAM.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("AvastRendererVS3");
            neoTabWindow_EQUIP_SETTING.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("MenuBarRenderer");
            neoTabWindow_LOG.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("MenuBarRenderer");
            neoTabWindow1.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("AvastRendererVS3");
            neoTabWindow2_LOG.Renderer = NeoTabControlLibrary.AddInRendererManager.LoadRenderer("AvastRendererVS3");

            neoTabWindow_INSP_SETTING.SelectedIndex = 0;
            neoTabWindow_ALG.SelectedIndex = 0;
            splitContainer_AUTO_main.SplitterDistance = 750;
            if (Properties.Settings.Default.Split_dist >= 0)
            {
                ctr_Display_1.splitContainer1.SplitterDistance = Properties.Settings.Default.Split_dist;
                Properties.Settings.Default.Save();
            }
            this.Refresh();

            for (int i = 0; i < 10; i++)
            {
                LVApp.Instance().m_Config.m_Error_Flag[i] = -1;
                LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] = 0;
                LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] = 0;
                LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] = 0;
                LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3] = 0;
                if (i < 4)
                {
                    LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[i] = 0;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Capture_Count[i] = 0;
                ctr_PLC1.send_Message[i] = new List<string>();
                ctr_PLC1.send_Message_Time[i] = new List<DateTime>();
                ctr_PLC1.send_Idx[i] = new List<uint>();
                ctr_PLC1.send_Idx_Time[i] = new List<DateTime>();
            }

            if (Properties.Settings.Default.Title.Length > 0)
            {
                label_Title.Text = Properties.Settings.Default.Title;
                ctr_Model1.textBox_Title.Text = Properties.Settings.Default.Title;
                button_Main_View.Visible = false;
            }
            else
            {
                button_Main_View.Visible = true;
                label_Title.Text = ctr_Model1.textBox_Title.Text = String.Empty;
            }
            DebugLogger.Instance().LogRecord("GUI Initialized.");
        }

        private void Frm_Main_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                //if (this.WindowState == FormWindowState.Minimized && m_Start_Check)
                //{
                //    LVApp.Instance().t_QuickMenu.Hide();
                //}
                //else
                //{
                //    if (m_Start_Check)
                //    {
                //        LVApp.Instance().t_QuickMenu.Show();
                //    }
                //}
                if (this.StatusBar.BarItems.Count == 0)
                {
                    return;
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num == 1)
                {
                    if (neoTabWindow_INSP_SETTING_CAM.TabPages.Count > 1)
                    {
                        neoTabWindow_INSP_SETTING_CAM.TabPages[7].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[6].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[5].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[4].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[3].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[2].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[1].Dispose();
                    }
                    if (neoTabWindow_ALG.TabPages.Count > 1)
                    {
                        neoTabWindow_ALG.TabPages[7].Dispose();
                        neoTabWindow_ALG.TabPages[6].Dispose();
                        neoTabWindow_ALG.TabPages[5].Dispose();
                        neoTabWindow_ALG.TabPages[4].Dispose();
                        neoTabWindow_ALG.TabPages[3].Dispose();
                        neoTabWindow_ALG.TabPages[2].Dispose();
                        neoTabWindow_ALG.TabPages[1].Dispose();
                    }

                    if (neoTabWindow1.TabPages.Count > 1)
                    {
                        neoTabWindow1.TabPages[7].Dispose();
                        neoTabWindow1.TabPages[6].Dispose();
                        neoTabWindow1.TabPages[5].Dispose();
                        neoTabWindow1.TabPages[4].Dispose();
                        neoTabWindow1.TabPages[3].Dispose();
                        neoTabWindow1.TabPages[2].Dispose();
                        neoTabWindow1.TabPages[1].Dispose();
                    }
                    if (neoTabWindow2_LOG.TabPages.Count > 1)
                    {
                        neoTabWindow2_LOG.TabPages[7].Dispose();
                        neoTabWindow2_LOG.TabPages[6].Dispose();
                        neoTabWindow2_LOG.TabPages[5].Dispose();
                        neoTabWindow2_LOG.TabPages[4].Dispose();
                        neoTabWindow2_LOG.TabPages[3].Dispose();
                        neoTabWindow2_LOG.TabPages[2].Dispose();
                        neoTabWindow2_LOG.TabPages[1].Dispose();
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 2)
                {
                    if (neoTabWindow_INSP_SETTING_CAM.TabPages.Count > 2)
                    {
                        neoTabWindow_INSP_SETTING_CAM.TabPages[7].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[6].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[5].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[4].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[3].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[2].Dispose();
                        //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Dispose();
                    }
                    if (neoTabWindow_ALG.TabPages.Count > 2)
                    {
                        neoTabWindow_ALG.TabPages[7].Dispose();
                        neoTabWindow_ALG.TabPages[6].Dispose();
                        neoTabWindow_ALG.TabPages[5].Dispose();
                        neoTabWindow_ALG.TabPages[4].Dispose();
                        neoTabWindow_ALG.TabPages[3].Dispose();
                        neoTabWindow_ALG.TabPages[2].Dispose();
                        //neoTabWindow_ALG.TabPages[1].Dispose();
                    }

                    if (neoTabWindow1.TabPages.Count > 2)
                    {
                        neoTabWindow1.TabPages[7].Dispose();
                        neoTabWindow1.TabPages[6].Dispose();
                        neoTabWindow1.TabPages[5].Dispose();
                        neoTabWindow1.TabPages[4].Dispose();
                        neoTabWindow1.TabPages[3].Dispose();
                        neoTabWindow1.TabPages[2].Dispose();
                        //neoTabWindow1.TabPages[1].Dispose();
                    }
                    if (neoTabWindow2_LOG.TabPages.Count > 2)
                    {
                        neoTabWindow2_LOG.TabPages[7].Dispose();
                        neoTabWindow2_LOG.TabPages[6].Dispose();
                        neoTabWindow2_LOG.TabPages[5].Dispose();
                        neoTabWindow2_LOG.TabPages[4].Dispose();
                        neoTabWindow2_LOG.TabPages[3].Dispose();
                        neoTabWindow2_LOG.TabPages[2].Dispose();
                        //neoTabWindow2_LOG.TabPages[1].Dispose();
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 3)
                {
                    if (neoTabWindow_INSP_SETTING_CAM.TabPages.Count > 3)
                    {
                        neoTabWindow_INSP_SETTING_CAM.TabPages[7].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[6].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[5].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[4].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[3].Dispose();
                        //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Dispose();
                        //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Dispose();
                    }
                    if (neoTabWindow_ALG.TabPages.Count > 3)
                    {
                        neoTabWindow_ALG.TabPages[7].Dispose();
                        neoTabWindow_ALG.TabPages[6].Dispose();
                        neoTabWindow_ALG.TabPages[5].Dispose();
                        neoTabWindow_ALG.TabPages[4].Dispose();
                        neoTabWindow_ALG.TabPages[3].Dispose();
                        //neoTabWindow_ALG.TabPages[2].Dispose();
                        //neoTabWindow_ALG.TabPages[1].Dispose();
                    }

                    if (neoTabWindow1.TabPages.Count > 3)
                    {
                        neoTabWindow1.TabPages[7].Dispose();
                        neoTabWindow1.TabPages[6].Dispose();
                        neoTabWindow1.TabPages[5].Dispose();
                        neoTabWindow1.TabPages[4].Dispose();
                        neoTabWindow1.TabPages[3].Dispose();
                        //neoTabWindow1.TabPages[2].Dispose();
                        //neoTabWindow1.TabPages[1].Dispose();
                    }
                    if (neoTabWindow2_LOG.TabPages.Count > 3)
                    {
                        neoTabWindow2_LOG.TabPages[7].Dispose();
                        neoTabWindow2_LOG.TabPages[6].Dispose();
                        neoTabWindow2_LOG.TabPages[5].Dispose();
                        neoTabWindow2_LOG.TabPages[4].Dispose();
                        neoTabWindow2_LOG.TabPages[3].Dispose();
                        //neoTabWindow2_LOG.TabPages[2].Dispose();
                        //neoTabWindow2_LOG.TabPages[1].Dispose();
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 4)
                {
                    if (neoTabWindow_INSP_SETTING_CAM.TabPages.Count > 4)
                    {
                        neoTabWindow_INSP_SETTING_CAM.TabPages[7].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[6].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[5].Dispose();
                        neoTabWindow_INSP_SETTING_CAM.TabPages[4].Dispose();
                        //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Dispose();
                        //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Dispose();
                        //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Dispose();
                    }
                    if (neoTabWindow_ALG.TabPages.Count > 4)
                    {
                        neoTabWindow_ALG.TabPages[7].Dispose();
                        neoTabWindow_ALG.TabPages[6].Dispose();
                        neoTabWindow_ALG.TabPages[5].Dispose();
                        neoTabWindow_ALG.TabPages[4].Dispose();
                        //neoTabWindow_ALG.TabPages[3].Dispose();
                        //neoTabWindow_ALG.TabPages[2].Dispose();
                        //neoTabWindow_ALG.TabPages[1].Dispose();
                    }

                    if (neoTabWindow1.TabPages.Count > 4)
                    {
                        neoTabWindow1.TabPages[7].Dispose();
                        neoTabWindow1.TabPages[6].Dispose();
                        neoTabWindow1.TabPages[5].Dispose();
                        neoTabWindow1.TabPages[4].Dispose();
                        //neoTabWindow1.TabPages[3].Dispose();
                        //neoTabWindow1.TabPages[2].Dispose();
                        //neoTabWindow1.TabPages[1].Dispose();
                    }
                    if (neoTabWindow2_LOG.TabPages.Count > 4)
                    {
                        neoTabWindow2_LOG.TabPages[7].Dispose();
                        neoTabWindow2_LOG.TabPages[6].Dispose();
                        neoTabWindow2_LOG.TabPages[5].Dispose();
                        neoTabWindow2_LOG.TabPages[4].Dispose();
                        //neoTabWindow2_LOG.TabPages[3].Dispose();
                        //neoTabWindow2_LOG.TabPages[2].Dispose();
                        //neoTabWindow2_LOG.TabPages[1].Dispose();
                    }
                }
                neoTabWindow_MAIN.TabPages[7].Dispose();
                this.StatusBar.BarItems[3].ItemWidth = this.Width - 388;
                //LVApp.Instance().m_mainform.ctr_Camera_Setting2.Force_USE_CheckedChanged(sender, e);
                //LVApp.Instance().m_mainform.ctr_Camera_Setting3.Force_USE_CheckedChanged(sender, e);
                //LVApp.Instance().m_mainform.ctr_Camera_Setting4.Force_USE_CheckedChanged(sender, e);
                if (this.WindowState == FormWindowState.Minimized && m_Start_Check)
                {
                    return;
                }
                //LVApp.Instance().m_mainform.ctr_Display_1.Update_Display();

                splitContainer11.SplitterDistance = splitContainer16.SplitterDistance = splitContainer19.SplitterDistance = splitContainer21.SplitterDistance = 650;
            }
            catch
            { }
        }


        private void HolderButtonClick(XCoolForm.XCoolForm.XCoolFormHolderButtonClickArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:

                    break;
                case 3:

                    break;
            }
        }

        public void Camera_Initialize()
        {
            try
            {
                object sender = null; EventArgs e = null;
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[0] == 6)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                        ctr_Camera_Setting1.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam1.Camera_Name;
                        ctr_Camera_Setting1.toolStripButtonConnect_Click(sender, e);
                    }
                    else
                    {
                        if (!LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen && (LVApp.Instance().m_Config.m_Interlock_Cam[0] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[0] == 0))
                        {
                            LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                            ctr_Camera_Setting1.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam1.Camera_Name;
                            if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 1)
                            {
                                LVApp.Instance().m_mainform.ctrCam1.Camera_Line_Mode = true;
                            }
                            ctr_Camera_Setting1.Connect_imageProvider();
                            ctr_Camera_Setting1.toolStripButtonConnect_Click(sender, e);
                        }
                    }
                }

                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[1] == 6)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                        ctr_Camera_Setting2.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam2.Camera_Name;
                        ctr_Camera_Setting2.toolStripButtonConnect_Click(sender, e);
                    }
                    else
                    {
                        if (!LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen && (LVApp.Instance().m_Config.m_Interlock_Cam[1] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[1] == 1))
                        {
                            LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                            ctr_Camera_Setting2.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam2.Camera_Name;
                            if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 1)
                            {
                                LVApp.Instance().m_mainform.ctrCam2.Camera_Line_Mode = true;
                            }
                            ctr_Camera_Setting2.Connect_imageProvider();
                            ctr_Camera_Setting2.toolStripButtonConnect_Click(sender, e);
                        }
                    }
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[2] == 6)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                        ctr_Camera_Setting3.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam3.Camera_Name;
                        ctr_Camera_Setting3.toolStripButtonConnect_Click(sender, e);
                    }
                    else
                    {
                        if (!LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen && (LVApp.Instance().m_Config.m_Interlock_Cam[2] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[2] == 2))
                        {
                            LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                            ctr_Camera_Setting3.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam3.Camera_Name;
                            if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 1)
                            {
                                LVApp.Instance().m_mainform.ctrCam3.Camera_Line_Mode = true;
                            }
                            ctr_Camera_Setting3.Connect_imageProvider();
                            ctr_Camera_Setting3.toolStripButtonConnect_Click(sender, e);
                        }
                    }
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[3] == 6)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                        ctr_Camera_Setting4.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam4.Camera_Name;
                        ctr_Camera_Setting4.toolStripButtonConnect_Click(sender, e);
                    }
                    else
                    {
                        if (!LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen && (LVApp.Instance().m_Config.m_Interlock_Cam[3] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[3] == 3))
                        {
                            LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                            ctr_Camera_Setting4.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam4.Camera_Name;
                            if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 1)
                            {
                                LVApp.Instance().m_mainform.ctrCam4.Camera_Line_Mode = true;
                            }
                            ctr_Camera_Setting4.Connect_imageProvider();
                            ctr_Camera_Setting4.toolStripButtonConnect_Click(sender, e);
                        }
                    }
                }
                //ctr_Camera_Setting5.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam5.Camera_Name;
                //ctr_Camera_Setting5.Connect_imageProvider();
                //ctr_Camera_Setting5.toolStripButtonConnect_Click(sender, e);
                //ctr_Camera_Setting6.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam6.Camera_Name;
                //ctr_Camera_Setting6.Connect_imageProvider();
                //ctr_Camera_Setting6.toolStripButtonConnect_Click(sender, e);
                //ctr_Camera_Setting7.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam7.Camera_Name;
                //ctr_Camera_Setting7.Connect_imageProvider();
                //ctr_Camera_Setting7.toolStripButtonConnect_Click(sender, e);
                //ctr_Camera_Setting8.m_SetCameraName = LVApp.Instance().m_mainform.ctrCam8.Camera_Name;
                //ctr_Camera_Setting8.Connect_imageProvider();
                //ctr_Camera_Setting8.toolStripButtonConnect_Click(sender, e);
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1 && (LVApp.Instance().m_Config.m_Interlock_Cam[0] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[0] == 0))
                {
                    ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2 && (LVApp.Instance().m_Config.m_Interlock_Cam[1] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[1] == 1))
                {
                    ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3 && (LVApp.Instance().m_Config.m_Interlock_Cam[2] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[2] == 2))
                {
                    ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4 && (LVApp.Instance().m_Config.m_Interlock_Cam[3] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[3] == 3))
                {
                    ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                }
                //ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                //ctr_Camera_Setting5.toolStripButton_LOAD_Click(sender, e);
                //ctr_Camera_Setting6.toolStripButton_LOAD_Click(sender, e);
                //ctr_Camera_Setting7.toolStripButton_LOAD_Click(sender, e);
                //ctr_Camera_Setting8.toolStripButton_LOAD_Click(sender, e);
                DebugLogger.Instance().LogRecord("Camera initialized!");
                Camera_Connection_Check();
            }
            catch (System.Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void add_Log(string str)
        {
            try
            {
                DebugLogger.Instance().LogRecord(str);
                if (richTextBox_LOG.InvokeRequired)
                {
                    richTextBox_LOG.Invoke((MethodInvoker)delegate
                    {
                        if (richTextBox_LOG.Lines.Length > 50)
                        {
                            int totalCharacters = richTextBox_LOG.Text.Trim().Length;
                            int totalLines = richTextBox_LOG.Lines.Length;
                            string lastLine = richTextBox_LOG.Lines[totalLines - 1] + "\n";
                            string copyOfLastLine = richTextBox_LOG.Lines[totalLines - 1];
                            if (totalLines > 1)
                            {
                                string newstring = richTextBox_LOG.Text.Substring(0, totalCharacters - lastLine.Length);
                                richTextBox_LOG.Text = newstring;
                            }
                            else
                            {
                                richTextBox_LOG.Text = "";
                            }                        //richTextBox_Main.ResetText();
                        }
                        string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + str + "\r\n" + richTextBox_LOG.Text;
                        richTextBox_LOG.Text = display_str;
                    });
                }
                else
                {
                    if (richTextBox_LOG.Lines.Length > 50)
                    {
                        int totalCharacters = richTextBox_LOG.Text.Trim().Length;
                        int totalLines = richTextBox_LOG.Lines.Length;
                        string lastLine = richTextBox_LOG.Lines[totalLines - 1] + "\n";
                        string copyOfLastLine = richTextBox_LOG.Lines[totalLines - 1];
                        if (totalLines > 1)
                        {
                            string newstring = richTextBox_LOG.Text.Substring(0, totalCharacters - lastLine.Length);
                            richTextBox_LOG.Text = newstring;
                        }
                        else
                        {
                            richTextBox_LOG.Text = "";
                        }                        //richTextBox_Main.ResetText();
                    }
                    string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + str + "\r\n" + richTextBox_LOG.Text;
                    richTextBox_LOG.Text = display_str;
                }
            }
            catch
            {
            }
        }

        public static Byte[] BmpToArray0(Bitmap value)
        {

            try
            {
                BitmapData data = value.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.ReadOnly, value.PixelFormat);
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
        public static Byte[] BmpToArray1(Bitmap value)
        {

            try
            {
                BitmapData data = value.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.ReadOnly, value.PixelFormat);
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
        public static Byte[] BmpToArray2(Bitmap value)
        {

            try
            {
                BitmapData data = value.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.ReadOnly, value.PixelFormat);
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
        public static Byte[] BmpToArray3(Bitmap value)
        {

            try
            {
                BitmapData data = value.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.ReadOnly, value.PixelFormat);
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
        //public Byte[] BmpToArray4(Bitmap value)
        //{
        //    BitmapData data = value.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.ReadOnly, value.PixelFormat);

        //    try
        //    {
        //        IntPtr ptr = data.Scan0;
        //        int bytes = Math.Abs(data.Stride) * value.Height;
        //        byte[] rgbValues = new byte[bytes];
        //        Marshal.Copy(ptr, rgbValues, 0, bytes);

        //        return rgbValues;
        //    }
        //    finally
        //    {
        //        value.UnlockBits(data);
        //    }
        //}


        public Bitmap ConvertBitmap0(byte[] frame, int width, int height, int ch)
        {
            try
            {
                if (frame == null || frame.Length == 0)
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

                   = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

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
            catch
            {
            }
            return new Bitmap(640, 480);
        }

        public Bitmap ConvertBitmap1(byte[] frame, int width, int height, int ch)
        {
            try
            {
                if (frame == null || frame.Length == 0)
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

                   = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

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
            catch
            {
            }
            return new Bitmap(640, 480);
        }

        public Bitmap ConvertBitmap2(byte[] frame, int width, int height, int ch)
        {
            try
            {
                if (frame == null || frame.Length == 0)
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

                   = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

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
            catch
            {
            }
            return new Bitmap(640, 480);
        }

        public Bitmap ConvertBitmap3(byte[] frame, int width, int height, int ch)
        {
            try
            {
                if (frame == null || frame.Length == 0)
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

                   = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

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
            catch
            {
            }
            return new Bitmap(640, 480);
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory")]
        static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        private Bitmap GetCopyOf0(Bitmap bmp, bool CopyPalette = true)
        {
            Bitmap bmpDest = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);

            if (!KernellDllCopyBitmap(bmp, bmpDest, CopyPalette))
                bmpDest = null;

            return bmpDest;
        }

        private Bitmap GetCopyOf1(Bitmap bmp, bool CopyPalette = true)
        {
            Bitmap bmpDest = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);

            if (!KernellDllCopyBitmap(bmp, bmpDest, CopyPalette))
                bmpDest = null;

            return bmpDest;
        }

        private Bitmap GetCopyOf2(Bitmap bmp, bool CopyPalette = true)
        {
            Bitmap bmpDest = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);

            if (!KernellDllCopyBitmap(bmp, bmpDest, CopyPalette))
                bmpDest = null;

            return bmpDest;
        }

        private Bitmap GetCopyOf3(Bitmap bmp, bool CopyPalette = true)
        {
            Bitmap bmpDest = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);

            if (!KernellDllCopyBitmap(bmp, bmpDest, CopyPalette))
                bmpDest = null;

            return bmpDest;
        }


        private bool KernellDllCopyBitmap(Bitmap bmpSrc, Bitmap bmpDest, bool CopyPalette = false)
        {
            bool copyOk = false;
            copyOk = CheckCompatibility(bmpSrc, bmpDest);
            if (copyOk)
            {
                BitmapData bmpDataSrc;
                BitmapData bmpDataDest;

                //Lock Bitmap to get BitmapData
                bmpDataSrc = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), ImageLockMode.ReadOnly, bmpSrc.PixelFormat);
                bmpDataDest = bmpDest.LockBits(new Rectangle(0, 0, bmpDest.Width, bmpDest.Height), ImageLockMode.WriteOnly, bmpDest.PixelFormat);
                int lenght = bmpDataSrc.Stride * bmpDataSrc.Height;

                CopyMemory(bmpDataDest.Scan0, bmpDataSrc.Scan0, (uint)lenght);

                bmpSrc.UnlockBits(bmpDataSrc);
                bmpDest.UnlockBits(bmpDataDest);

                if (CopyPalette && bmpSrc.Palette.Entries.Length > 0)
                    bmpDest.Palette = bmpSrc.Palette;
            }
            return copyOk;
        }

        private bool CheckCompatibility(Bitmap bmp1, Bitmap bmp2)
        {
            return ((bmp1.Width == bmp2.Width) && (bmp1.Height == bmp2.Height) && (bmp1.PixelFormat == bmp2.PixelFormat));
        }

        int t_Cam_CNT = 0;
        public void Add_PLC_Tx_Message(int Cam_Num, int t_v)
        {
            if (ctr_PLC1.m_threads_Check)
            {
                if (t_v == -10)
                {
                    //LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    t_v = 10;
                }
                //if (t_v == -1)
                //{
                //    return;
                //}
                if (LVApp.Instance().m_Config.PLC_Pingpong_USE)
                {
                    //lock (ctr_PLC1.send_Message[Cam_Num])
                    {
                        //if (ctr_PLC1.send_Message[Cam_Num].Count > 10)
                        //{
                        //    ctr_PLC1.send_Message[Cam_Num].Clear();
                        //}
                        int t_idx = LVApp.Instance().m_Config.Tx_Idx[Cam_Num] % LVApp.Instance().m_Config.Tx_Room_Num;
                        string t_msg = "";
                        if (ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                        {
                            //int t_Idx_cnt = ctr_PLC1.send_Idx[Cam_Num].Count;
                            //if (t_Idx_cnt == 0)
                            //{
                            //    return;
                            //}

                            t_msg = "EW000" + Cam_Num.ToString("0") + "_" + (t_v / 10.0).ToString("00") + "_0";
                            lock (ctr_PLC1.send_Message_Time[Cam_Num])
                            {
                                ctr_PLC1.send_Message_Time[Cam_Num].Add(DateTime.Now);
                            }
                        }
                        else
                        {
                            t_msg = "DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + t_idx.ToString("0") + Cam_Num.ToString() + "_" + t_v.ToString("00") + "_0";
                        }

                        lock (ctr_PLC1.send_Message[Cam_Num])
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add(t_msg);
                        }


                        LVApp.Instance().m_Config.Tx_Idx[Cam_Num]++;

                        //int t_max_v = 0;
                        //for (int i = 0; i < t_Cam_CNT; i++)
                        //{
                        //    if (t_max_v <= LVApp.Instance().m_Config.Tx_Idx[i])
                        //    {
                        //        t_max_v = LVApp.Instance().m_Config.Tx_Idx[i];
                        //    }
                        //}

                        //if (t_max_v != LVApp.Instance().m_Config.Tx_Idx[Cam_Num])
                        //{
                        //    LVApp.Instance().m_Config.Tx_Idx[Cam_Num] = t_max_v;
                        //}

                        //if (LVApp.Instance().m_Config.Tx_Idx[Cam_Num] > int.MaxValue - LVApp.Instance().m_Config.Tx_Room_Num * 2)
                        //{
                        //    if (LVApp.Instance().m_Config.Tx_Idx[Cam_Num] % LVApp.Instance().m_Config.Tx_Room_Num == 0)
                        //    {
                        //        LVApp.Instance().m_Config.Tx_Idx[Cam_Num] = 0;
                        //    }
                        //}
                    }
                }
                else
                {
                    string t_msg = "";
                    if (ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.LVDIO)
                    {
                        if (LVApp.Instance().m_DIO.m_DoportData.Count > 0)
                        {
                            if (Cam_Num == 0)
                            {
                                if (t_v == 40)
                                { // OK일 때
                                    LVApp.Instance().m_DIO.m_DoportData[0].Set(2, true);
                                    LVApp.Instance().m_DIO.m_DoportData[0].Set(3, false);
                                }
                                else
                                {
                                    if (t_v == -1)
                                    { // No Object일 때
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(2, false);
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(3, false);
                                    }
                                    else
                                    { // NG일 때
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(2, false);
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(3, true);
                                    }
                                }
                            }
                            if (Cam_Num == 1)
                            {
                                if (t_v == 40)
                                { // OK일 때
                                    LVApp.Instance().m_DIO.m_DoportData[0].Set(4, true);
                                    LVApp.Instance().m_DIO.m_DoportData[0].Set(5, false);
                                }
                                else
                                {
                                    if (t_v == -1)
                                    { // No Object일 때
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(4, false);
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(5, false);
                                    }
                                    else
                                    { // NG일 때
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(4, false);
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(5, true);
                                    }
                                }
                            }
                            if (Cam_Num == 2)
                            {
                                if (t_v == 40)
                                { // OK일 때
                                    LVApp.Instance().m_DIO.m_DoportData[0].Set(6, true);
                                    LVApp.Instance().m_DIO.m_DoportData[0].Set(7, false);
                                }
                                else
                                {
                                    if (t_v == -1)
                                    { // No Object일 때
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(6, false);
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(7, false);
                                    }
                                    else
                                    { // NG일 때
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(6, false);
                                        LVApp.Instance().m_DIO.m_DoportData[0].Set(7, true);
                                    }
                                }
                            }
                            LVApp.Instance().m_DIO.Do_Job_Mode = 1;
                        }
                        if (LVApp.Instance().m_DIO.m_DoportData.Count > 1)
                        {
                            if (Cam_Num == 3)
                            {
                                if (t_v == 40)
                                { // OK일 때
                                    LVApp.Instance().m_DIO.m_DoportData[1].Set(0, true);
                                    LVApp.Instance().m_DIO.m_DoportData[1].Set(1, false);
                                }
                                else
                                {
                                    if (t_v == -1)
                                    { // No Object일 때
                                        LVApp.Instance().m_DIO.m_DoportData[1].Set(0, false);
                                        LVApp.Instance().m_DIO.m_DoportData[1].Set(1, false);
                                    }
                                    else
                                    { // NG일 때
                                        LVApp.Instance().m_DIO.m_DoportData[1].Set(0, false);
                                        LVApp.Instance().m_DIO.m_DoportData[1].Set(1, true);
                                    }
                                }
                            }
                        }

                        //t_msg = "EW000" + Cam_Num.ToString("0") + "_" + (t_v / 10.0).ToString("00") + "_0";
                        ////int t_Idx_cnt = ctr_PLC1.send_Idx[Cam_Num].Count;
                        ////if (t_Idx_cnt == 0)
                        ////{
                        ////    return;
                        ////}
                        //lock (ctr_PLC1.send_Message_Time[Cam_Num])
                        //{
                        //    ctr_PLC1.send_Message_Time[Cam_Num].Add(DateTime.Now);
                        //}
                    }
                    else
                    {
                        t_msg = "DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + t_v.ToString("00") + "_0";
                    }
                    lock (ctr_PLC1.send_Message[Cam_Num])
                    {
                        ctr_PLC1.send_Message[Cam_Num].Add(t_msg);
                    }
                }
            }
        }

        private Stopwatch[] Interval_SW = new Stopwatch[4];
        private readonly int[] _interval = { 200, 200, 200, 200 };
        private bool[] _is_NewFrame = { false, false, false, false };
        private int[] _mergeImageCount = { 0, 0, 0, 0 };
        public void ctrCam1_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 0;
                ctr_Camera_Setting1.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(4);

                #region LHJ - 240806 - 이미지 프레임 Merge 기능이 추가되면서, 아래 로그가 너무 많이 발생 함
                // Merge 기능을 사용하지 않을 때만 로그를 추가하고, Merge 기능을 사용 중일 때는 Merge 후에 로그를 쓰도록 변경 함
                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                {
                    DebugLogger.Instance().LogRecord($"AREA CAM0 Grab: {ctr_Camera_Setting1.Grab_Num}");
                }
                else
                {
                    if (Interval_SW[Cam_Num] == null)
                    {
                        Interval_SW[Cam_Num] = new Stopwatch();
                    }
                }
                #endregion
                if (m_Job_Mode0 == 0)// && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    //GC.Collect();
                    //m_Job_Mode1 = 2;
                    if (ctrCam1.m_bitmap != null)
                    {
                        Bitmap NewImg = null;
                        //lock (ctrCam1.m_bitmap)
                        {
                            ctrCam1.t_check_grab = true;
                            NewImg = ctrCam1.m_bitmap.Clone() as Bitmap;
                            ctrCam1.t_check_grab = false;
                            if (NewImg == null)
                            {
                                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                                {
                                    Add_PLC_Tx_Message(Cam_Num, 10);
                                    add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                    return;
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                            {
                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == -1)
                                {
                                    // 한 제품에 대해 Merge는 완료, 알고리즘은 동작 중일 때

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (Interval_SW[Cam_Num].ElapsedMilliseconds > _interval[Cam_Num] || Interval_SW[Cam_Num].ElapsedMilliseconds <= 0)
                                {
                                    // Interval이 길거나 0이면 (첫 제품 또는) 다음 제품으로 간주
                                    _is_NewFrame[Cam_Num] = true;
                                    ++_mergeImageCount[Cam_Num];
                                    DebugLogger.Instance().LogRecord($"AREA CAM{Cam_Num} Start Merge Grab: {_mergeImageCount[Cam_Num]} Previous Merge Count: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");

                                    if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] != 0)
                                    {
                                        // Interval이 긴데도 Image_Merge Index가 남아 있으면,이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                        DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous Remain: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");
                                        LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                    }
                                }
                                else if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == 0)
                                {
                                    // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                    // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                    // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 예외

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] >= 0 && LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    Capture_framebuffer[Cam_Num].Add(NewImg);
                                    ++LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num];
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = -1;

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            else
                            {
                                if (Capture_framebuffer[Cam_Num].Count > 0)
                                {
                                    Capture_framebuffer[Cam_Num].Clear();
                                }
                                Capture_framebuffer[Cam_Num].Add(NewImg);

                                #region LHJ - 240804 - 디버깅용
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam0[((uint)ctr_Camera_Setting1.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                                #endregion
                            }
                        }

                        //{
                        //    Capture_framebuffer[Cam_Num].Add((Bitmap)ctrCam1.m_bitmap.Clone());
                        //}
                        //Capture_Image0[Capture_Count[Cam_Num]] = (Bitmap)ctrCam1.m_bitmap.Clone();
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            ctrCam2.m_bitmap = NewImg;
                            //ctrCam2.m_bitmap = (Bitmap)ctrCam1.m_bitmap.Clone();
                            ctrCam2_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            ctrCam3.m_bitmap = NewImg;
                            //ctrCam3.m_bitmap = (Bitmap)ctrCam1.m_bitmap.Clone();
                            ctrCam3_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            ctrCam4.m_bitmap = NewImg;
                            //ctrCam4.m_bitmap = (Bitmap)ctrCam1.m_bitmap.Clone();
                            ctrCam4_GrabComplete(sender, e);
                        }
                        m_Job_Mode0 = 1;
                        ctr_PLC1.MC_Rx_Request[Cam_Num] = true;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            //Capture_Count[1]++;
                            //if (Capture_Count[1] >= 3)
                            //{
                            //    Capture_Count[1] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode1 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            //Capture_Count[2]++;
                            //if (Capture_Count[2] >= 3)
                            //{
                            //    Capture_Count[2] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode2 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            //Capture_Count[3]++;
                            //if (Capture_Count[3] >= 3)
                            //{
                            //    Capture_Count[3] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode3 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM0 Grab Error!");
                    }
                }
                else
                {
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    #region LHJ - 240806 - 이미지 Merge 기능을 사용할 때는 Cam Miss 로그를 남기지 않고, Merge 후 Cam Miss 개수를 남길 수 있도록 준비
                    // 사유 : Image Merge 기능은 고속획득 기반으로 + 이미지 누락을 감안하고 동작
                    // 기존 코드 - Start
                    //DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    // 기존 코드 - End

                    // 신규 코드 - Start
                    if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                    {
                        // LHJ - 240808 Interval 위주로 제품을 구분
                        Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                    }
                    else
                    {
                        DebugLogger.Instance().LogRecord($"CAM{Cam_Num} Miss!");
                    }
                    // 신규 코드 - End
                    #endregion

                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }
                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                    //Thread.Sleep(ctr_PLC1.m_DELAYCAMMISS);
                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode0 = 0;
                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_0.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log0 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log0[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log0[j + 1] = "";// LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();
                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log0[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log0[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc0();
                        //lock (LVApp.Instance().m_Config.CSVLog[4])
                        //{
                        //    LVApp.Instance().m_Config.LogThreadProc_Total();
                        //}
                    }

                }
                //GC.Collect();
            }
            catch (Exception ex)
            {
                ctrCam1.t_check_grab = false;
                DebugLogger.Instance().LogRecord($"Cam0 GrabComplete Error: {ex.StackTrace}");
            }
        }

        public void ctrCam2_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 1;
                ctr_Camera_Setting2.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(5);

                #region LHJ - 240806 - 이미지 프레임 Merge 기능이 추가되면서, 아래 로그가 너무 많이 발생 함
                // Merge 기능을 사용하지 않을 때만 로그를 추가하고, Merge 기능을 사용 중일 때는 Merge 후에 로그를 쓰도록 변경 함
                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                {
                    DebugLogger.Instance().LogRecord($"AREA CAM1 Grab: {ctr_Camera_Setting2.Grab_Num}");
                }
                else
                {
                    if (Interval_SW[Cam_Num] == null)
                    {
                        Interval_SW[Cam_Num] = new Stopwatch();
                    }
                }
                #endregion

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    Bitmap img = (Bitmap)ctrCam2.m_bitmap.Clone();
                //    LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                //}

                if (m_Job_Mode1 == 0)// && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    //GC.Collect();
                    if (ctrCam2.m_bitmap != null)
                    {
                        //Capture_Count[Cam_Num]++;
                        //if (Capture_Count[Cam_Num] >= 1)
                        //{
                        //    Capture_Count[Cam_Num] = 0;
                        //}
                        //else
                        //{
                        //}
                        //Run_SW[Cam_Num].Reset();
                        //Run_SW[Cam_Num].Start();
                        //Capture_framebuffer[Cam_Num].Enqueue((Bitmap)ctrCam2.m_bitmap.Clone());
                        //lock (Capture_framebuffer[Cam_Num])
                        Bitmap NewImg = null;
                        //lock (ctrCam2.m_bitmap)
                        {
                            ctrCam2.t_check_grab = true;
                            //NewImg = GetCopyOf(ctrCam2.m_bitmap);//.Clone() as Bitmap;
                            //NewImg = GetCopyOf1(ctrCam2.m_bitmap);//.Clone() as Bitmap;
                            NewImg = ctrCam2.m_bitmap.Clone() as Bitmap;
                            ctrCam2.t_check_grab = false;
                            if (NewImg == null)
                            {
                                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                                {
                                    Add_PLC_Tx_Message(Cam_Num, 10);
                                    add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                    return;
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                            {
                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == -1)
                                {
                                    // 한 제품에 대해 Merge는 완료, 알고리즘은 동작 중일 때

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (Interval_SW[Cam_Num].ElapsedMilliseconds > _interval[Cam_Num] || Interval_SW[Cam_Num].ElapsedMilliseconds <= 0)
                                {
                                    // Interval이 길거나 0이면 (첫 제품 또는) 다음 제품으로 간주
                                    _is_NewFrame[Cam_Num] = true;
                                    ++_mergeImageCount[Cam_Num];
                                    DebugLogger.Instance().LogRecord($"AREA CAM{Cam_Num} Start Merge Grab: {_mergeImageCount[Cam_Num]} Previous Merge Count: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");

                                    if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] != 0)
                                    {
                                        // Interval이 긴데도 Image_Merge Index가 남아 있으면,이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                        DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous Remain: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");
                                        LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                    }
                                }
                                else if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == 0)
                                {
                                    // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                    // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                    // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 예외

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] >= 0 && LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    Capture_framebuffer[Cam_Num].Add(NewImg);
                                    ++LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num];
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = -1;

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            else
                            {
                                if (Capture_framebuffer[Cam_Num].Count > 0)
                                {
                                    Capture_framebuffer[Cam_Num].Clear();
                                }
                                Capture_framebuffer[Cam_Num].Add(NewImg);

                                #region LHJ - 240804 - 디버깅용
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam1[((uint)ctr_Camera_Setting2.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                                #endregion
                            }
                        }

                        //{
                        //    Capture_framebuffer[Cam_Num].Add((Bitmap)ctrCam2.m_bitmap.Clone());
                        //}
                        //Capture_Image1[Capture_Count[Cam_Num]] = (Bitmap)ctrCam2.m_bitmap.Clone();
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            ctrCam1.m_bitmap = NewImg;
                            //ctrCam1.m_bitmap = (Bitmap)ctrCam2.m_bitmap.Clone();
                            ctrCam1_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            ctrCam3.m_bitmap = NewImg;
                            //ctrCam3.m_bitmap = (Bitmap)ctrCam2.m_bitmap.Clone();
                            ctrCam3_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            ctrCam4.m_bitmap = NewImg;
                            //ctrCam4.m_bitmap = (Bitmap)ctrCam2.m_bitmap.Clone();
                            ctrCam4_GrabComplete(sender, e);
                        }
                        m_Job_Mode1 = 1;
                        ctr_PLC1.MC_Rx_Request[Cam_Num] = true;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            //Capture_Count[0]++;
                            //if (Capture_Count[0] >= 3)
                            //{
                            //    Capture_Count[0] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode0 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            //Capture_Count[2]++;
                            //if (Capture_Count[2] >= 3)
                            //{
                            //    Capture_Count[2] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode2 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            //Capture_Count[3]++;
                            //if (Capture_Count[3] >= 3)
                            //{
                            //    Capture_Count[3] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode3 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM1 Grab Error!");
                    }
                }
                else
                {

                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    #region LHJ - 240806 - 이미지 Merge 기능을 사용할 때는 Cam Miss 로그를 남기지 않고, Merge 후 Cam Miss 개수를 남길 수 있도록 준비
                    // 사유 : Image Merge 기능은 고속획득 기반으로 + 이미지 누락을 감안하고 동작
                    // 기존 코드 - Start
                    //DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    // 기존 코드 - End

                    // 신규 코드 - Start
                    if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                    {
                        // LHJ - 240808 Interval 위주로 제품을 구분
                        Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                    }
                    else
                    {
                        DebugLogger.Instance().LogRecord($"CAM{Cam_Num} Miss!");
                    }
                    // 신규 코드 - End
                    #endregion

                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }
                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);

                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                    //Thread.Sleep(ctr_PLC1.m_DELAYCAMMISS);
                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode1 = 0;

                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_1.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log1 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log1[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log1[j + 1] = LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();

                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log1[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else if (j > 1)
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log1[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc1();
                        //lock (LVApp.Instance().m_Config.CSVLog[4])
                        //{
                        //    LVApp.Instance().m_Config.LogThreadProc_Total();
                        //}
                    }

                }
                //GC.Collect();
            }
            catch (Exception ex)
            {
                ctrCam2.t_check_grab = false;
                DebugLogger.Instance().LogRecord($"Cam1 GrabComplete Error: {ex.StackTrace}");
            }
        }

        public void ctrCam3_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 2;
                ctr_Camera_Setting3.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(6);

                #region LHJ - 240806 - 이미지 프레임 Merge 기능이 추가되면서, 아래 로그가 너무 많이 발생 함
                // Merge 기능을 사용하지 않을 때만 로그를 추가하고, Merge 기능을 사용 중일 때는 Merge 후에 로그를 쓰도록 변경 함
                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                {
                    DebugLogger.Instance().LogRecord($"AREA CAM2 Grab: {ctr_Camera_Setting3.Grab_Num}");
                }
                else
                {
                    if (Interval_SW[Cam_Num] == null)
                    {
                        Interval_SW[Cam_Num] = new Stopwatch();
                    }
                }
                #endregion

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    Bitmap img = (Bitmap)ctrCam3.m_bitmap.Clone();
                //    LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                //}

                if (m_Job_Mode2 == 0)// && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    //GC.Collect();
                    //m_Job_Mode2 = 2;
                    if (ctrCam3.m_bitmap != null)
                    {
                        //Capture_Count[Cam_Num]++;
                        //if (Capture_Count[Cam_Num] >= 3)
                        //{
                        //    Capture_Count[Cam_Num] = 0;
                        //}
                        //else
                        //{
                        //}
                        //Run_SW[Cam_Num].Reset();
                        //Run_SW[Cam_Num].Start();
                        //Capture_framebuffer[Cam_Num].Enqueue((Bitmap)ctrCam3.m_bitmap.Clone());
                        //lock (Capture_framebuffer[Cam_Num])
                        //{
                        Bitmap NewImg = null;
                        //lock (ctrCam3.m_bitmap)
                        {
                            ctrCam3.t_check_grab = true;
                            //NewImg = GetCopyOf(ctrCam2.m_bitmap);//.Clone() as Bitmap;
                            //NewImg = GetCopyOf1(ctrCam2.m_bitmap);//.Clone() as Bitmap;
                            NewImg = ctrCam3.m_bitmap.Clone() as Bitmap;
                            ctrCam3.t_check_grab = false;
                            if (NewImg == null)
                            {
                                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                                {
                                    Add_PLC_Tx_Message(Cam_Num, 10);
                                    add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                    return;
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                            {
                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == -1)
                                {
                                    // 한 제품에 대해 Merge는 완료, 알고리즘은 동작 중일 때

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (Interval_SW[Cam_Num].ElapsedMilliseconds > _interval[Cam_Num] || Interval_SW[Cam_Num].ElapsedMilliseconds <= 0)
                                {
                                    // Interval이 길거나 0이면 (첫 제품 또는) 다음 제품으로 간주
                                    _is_NewFrame[Cam_Num] = true;
                                    ++_mergeImageCount[Cam_Num];
                                    DebugLogger.Instance().LogRecord($"AREA CAM{Cam_Num} Start Merge Grab: {_mergeImageCount[Cam_Num]} Previous Merge Count: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");

                                    if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] != 0)
                                    {
                                        // Interval이 긴데도 Image_Merge Index가 남아 있으면,이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                        DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");
                                        LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                    }
                                }
                                else if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == 0)
                                {
                                    // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                    // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                    // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 예외

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] >= 0 && LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    Capture_framebuffer[Cam_Num].Add(NewImg);
                                    ++LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num];
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = -1;

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            else
                            {
                                if (Capture_framebuffer[Cam_Num].Count > 0)
                                {
                                    Capture_framebuffer[Cam_Num].Clear();
                                }
                                Capture_framebuffer[Cam_Num].Add(NewImg);

                                #region LHJ - 240804 - 디버깅용
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam2[((uint)ctr_Camera_Setting3.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                                #endregion
                            }
                        }

                        //{
                        //    Capture_framebuffer[Cam_Num].Add((Bitmap)ctrCam3.m_bitmap.Clone());
                        //}
                        //Capture_Image2[Capture_Count[Cam_Num]] = (Bitmap)ctrCam3.m_bitmap.Clone();
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            ctrCam1.m_bitmap = NewImg;
                            //ctrCam1.m_bitmap = (Bitmap)ctrCam3.m_bitmap.Clone();
                            ctrCam1_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            ctrCam2.m_bitmap = NewImg;
                            //ctrCam2.m_bitmap = (Bitmap)ctrCam3.m_bitmap.Clone();
                            ctrCam2_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            ctrCam4.m_bitmap = NewImg;
                            //ctrCam4.m_bitmap = (Bitmap)ctrCam3.m_bitmap.Clone();
                            ctrCam4_GrabComplete(sender, e);
                        }
                        m_Job_Mode2 = 1;
                        ctr_PLC1.MC_Rx_Request[Cam_Num] = true;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            //Capture_Count[0]++;
                            //if (Capture_Count[0] >= 3)
                            //{
                            //    Capture_Count[0] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode0 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            //Capture_Count[1]++;
                            //if (Capture_Count[1] >= 3)
                            //{
                            //    Capture_Count[1] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode1 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            //Capture_Count[3]++;
                            //if (Capture_Count[3] >= 3)
                            //{
                            //    Capture_Count[3] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode3 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM2 Grab Error!");
                    }
                }
                else
                {
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    #region LHJ - 240806 - 이미지 Merge 기능을 사용할 때는 Cam Miss 로그를 남기지 않고, Merge 후 Cam Miss 개수를 남길 수 있도록 준비
                    // 사유 : Image Merge 기능은 고속획득 기반으로 + 이미지 누락을 감안하고 동작
                    // 기존 코드 - Start
                    //DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    // 기존 코드 - End

                    // 신규 코드 - Start
                    if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                    {
                        // LHJ - 240808 Interval 위주로 제품을 구분
                        Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                    }
                    else
                    {
                        DebugLogger.Instance().LogRecord($"CAM{Cam_Num} Miss!");
                    }
                    // 신규 코드 - End
                    #endregion

                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }

                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);

                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                    //Thread.Sleep(ctr_PLC1.m_DELAYCAMMISS);
                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode2 = 0;

                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_2.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log2 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log2[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log2[j + 1] = LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();

                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log2[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else if (j > 1)
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log2[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc2();
                        //lock (LVApp.Instance().m_Config.CSVLog[4])
                        //{
                        //    LVApp.Instance().m_Config.LogThreadProc_Total();
                        //}

                    }
                }

                //GC.Collect();
            }
            catch (Exception ex)
            {
                ctrCam3.t_check_grab = false;
                DebugLogger.Instance().LogRecord($"Cam2 GrabComplete Error: {ex.StackTrace}");
            }
        }

        public void ctrCam4_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 3;
                ctr_Camera_Setting4.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(7);

                #region LHJ - 240806 - 이미지 프레임 Merge 기능이 추가되면서, 아래 로그가 너무 많이 발생 함
                // Merge 기능을 사용하지 않을 때만 로그를 추가하고, Merge 기능을 사용 중일 때는 Merge 후에 로그를 쓰도록 변경 함
                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                {
                    DebugLogger.Instance().LogRecord($"AREA CAM3 Grab: {ctr_Camera_Setting4.Grab_Num}");
                }
                else
                {
                    if (Interval_SW[Cam_Num] == null)
                    {
                        Interval_SW[Cam_Num] = new Stopwatch();
                    }
                }
                #endregion

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    Bitmap img = (Bitmap)ctrCam4.m_bitmap.Clone();
                //    LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                //}

                if (m_Job_Mode3 == 0)// && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    //GC.Collect();
                    //m_Job_Mode2 = 2;
                    if (ctrCam4.m_bitmap != null)
                    {
                        //Capture_Count[Cam_Num]++;
                        //if (Capture_Count[Cam_Num] >= 3)
                        //{
                        //    Capture_Count[Cam_Num] = 0;
                        //}
                        //else
                        //{
                        //}
                        //Run_SW[Cam_Num].Reset();
                        //Run_SW[Cam_Num].Start();
                        //Capture_framebuffer[Cam_Num].Enqueue((Bitmap)ctrCam4.m_bitmap.Clone());
                        //lock (Capture_framebuffer[Cam_Num])

                        Bitmap NewImg = null;
                        //lock (ctrCam4.m_bitmap)
                        {
                            ctrCam4.t_check_grab = true;
                            //NewImg = GetCopyOf(ctrCam2.m_bitmap);//.Clone() as Bitmap;
                            //NewImg = GetCopyOf1(ctrCam2.m_bitmap);//.Clone() as Bitmap;
                            NewImg = ctrCam4.m_bitmap.Clone() as Bitmap;
                            ctrCam4.t_check_grab = false;
                            if (NewImg == null)
                            {
                                if (!LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                                {
                                    Add_PLC_Tx_Message(Cam_Num, 10);
                                    add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                    return;
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                            {
                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == -1)
                                {
                                    // 한 제품에 대해 Merge는 완료, 알고리즘은 동작 중일 때

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (Interval_SW[Cam_Num].ElapsedMilliseconds > _interval[Cam_Num] || Interval_SW[Cam_Num].ElapsedMilliseconds <= 0)
                                {
                                    // Interval이 길거나 0이면 (첫 제품 또는) 다음 제품으로 간주
                                    _is_NewFrame[Cam_Num] = true;
                                    ++_mergeImageCount[Cam_Num];
                                    DebugLogger.Instance().LogRecord($"AREA CAM{Cam_Num} Start Merge Grab: {_mergeImageCount[Cam_Num]} Previous Merge Count: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");

                                    if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] != 0)
                                    {
                                        // Interval이 긴데도 Image_Merge Index가 남아 있으면,이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                        DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous: {LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num]}");
                                        LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                    }
                                }
                                else if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == 0)
                                {
                                    // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                    // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                    // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 예외

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] >= 0 && LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    Capture_framebuffer[Cam_Num].Add(NewImg);
                                    ++LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num];
                                }

                                if (LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                                {
                                    LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = -1;

                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                }
                                else
                                {
                                    // LHJ - 240808 Interval 위주로 제품을 구분
                                    Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                                    return;
                                }
                            }
                            else
                            {
                                if (Capture_framebuffer[Cam_Num].Count > 0)
                                {
                                    Capture_framebuffer[Cam_Num].Clear();
                                }
                                Capture_framebuffer[Cam_Num].Add(NewImg);

                                #region LHJ - 240804 - 디버깅용
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam3[((uint)ctr_Camera_Setting4.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                                #endregion
                            }
                        }
                        //{
                        //    Capture_framebuffer[Cam_Num].Add((Bitmap)ctrCam4.m_bitmap.Clone());
                        //}
                        //Capture_Image3[Capture_Count[Cam_Num]] = (Bitmap)ctrCam4.m_bitmap.Clone();
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            ctrCam1.m_bitmap = NewImg;
                            //ctrCam1.m_bitmap = (Bitmap)ctrCam4.m_bitmap.Clone();
                            ctrCam1_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            ctrCam2.m_bitmap = NewImg;
                            //ctrCam2.m_bitmap = (Bitmap)ctrCam4.m_bitmap.Clone();
                            ctrCam2_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            ctrCam3.m_bitmap = NewImg;
                            //ctrCam3.m_bitmap = (Bitmap)ctrCam4.m_bitmap.Clone();
                            ctrCam3_GrabComplete(sender, e);
                        }
                        m_Job_Mode3 = 1;
                        ctr_PLC1.MC_Rx_Request[Cam_Num] = true;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            //Capture_Count[0]++;
                            //if (Capture_Count[0] >= 3)
                            //{
                            //    Capture_Count[0] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode0 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            //Capture_Count[1]++;
                            //if (Capture_Count[1] >= 3)
                            //{
                            //    Capture_Count[1] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode1 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            //Capture_Count[2]++;
                            //if (Capture_Count[2] >= 3)
                            //{
                            //    Capture_Count[2] = 0;
                            //}
                            //else
                            //{
                            //}
                            m_Probe_Job_Mode2 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM3 Grab Error!");
                    }
                }
                else
                {
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    #region LHJ - 240806 - 이미지 Merge 기능을 사용하지 않을 때만 Cam Miss 체크를 함
                    // 사유 : Image Merge 기능은 고속획득 기반으로 + 이미지 누락을 감안하고 동작
                    // 기존 코드 - Start
                    //DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    // 기존 코드 - End

                    // 신규 코드 - Start
                    if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                    {
                        // LHJ - 240808 Interval 위주로 제품을 구분
                        Interval_SW[Cam_Num].Reset(); Interval_SW[Cam_Num].Start();
                    }
                    else
                    {
                        DebugLogger.Instance().LogRecord($"CAM{Cam_Num} MIss!");
                    }

                    // 신규 코드 - End
                    #endregion

                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }

                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode3 = 0;

                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_3.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log3 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log3[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log3[j + 1] = LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();

                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log3[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else if (j > 1)
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log3[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        //lock (LVApp.Instance().m_Config.CSVLog[4])
                        //{
                        //    LVApp.Instance().m_Config.LogThreadProc_Total();
                        //}
                    }
                }

                //GC.Collect();
            }
            catch (Exception ex)
            {
                ctrCam4.t_check_grab = false;
                DebugLogger.Instance().LogRecord($"Cam3 GrabComplete Error: {ex.StackTrace}");
            }
        }

        public void MILCam1_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 0;
                ctr_Camera_Setting1.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(4);

                DebugLogger.Instance().LogRecord("MIL CAM0 Grab: " + ctr_Camera_Setting1.Grab_Num.ToString());

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    if (LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex] != null)
                //    {
                //        Bitmap NewImg = null;
                //        {
                //            NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex]);
                //            LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, NewImg, 0);
                //        }
                //    }
                //}

                if (m_Job_Mode0 == 0)
                {
                    if (LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex] != null)
                    {
                        Bitmap NewImg = null;
                        {
                            //NewImg = GetCopyOf0(LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex]);
                            NewImg = LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex].Clone() as Bitmap;
                            if (NewImg == null)
                            {
                                Add_PLC_Tx_Message(Cam_Num, 10);
                                add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                return;
                            }
                            if (Capture_framebuffer[Cam_Num].Count > 0)
                            {
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                            Capture_framebuffer[Cam_Num].Add(NewImg);
                        }

                        #region LHJ - 240804 - 디버깅용
                        // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                        // 알고리즘을 다운 시키는 이미지가 있는지 확인
                        // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                        LVApp.Instance().m_Config._lastImage_Cam0[((uint)ctr_Camera_Setting1.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                        #endregion

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex] = NewImg;
                            MILCam2_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex] = NewImg;
                            MILCam3_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex] = NewImg;
                            MILCam4_GrabComplete(sender, e);
                        }
                        m_Job_Mode0 = 1;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode1 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode2 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode3 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM0 Grab Error!");
                    }
                }
                else
                {
                    DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }
                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;

                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode0 = 0;
                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_0.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log0 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log0[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log0[j + 1] = "";// LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();
                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log0[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log0[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc0();
                    }

                }
                //GC.Collect();
            }
            catch
            {
                //ctrCam1.t_check_grab = false;
            }
        }

        public void MILCam2_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 1;
                ctr_Camera_Setting2.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(5);

                DebugLogger.Instance().LogRecord("MIL CAM1 Grab: " + ctr_Camera_Setting2.Grab_Num.ToString());

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    if (LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex] != null)
                //    {
                //        Bitmap NewImg = null;
                //        {
                //            NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex]);
                //            LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, NewImg, 0);
                //        }
                //    }
                //}

                if (m_Job_Mode1 == 0)
                {
                    if (LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex] != null)
                    {
                        Bitmap NewImg = null;
                        {
                            //NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex]);
                            NewImg = LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex].Clone() as Bitmap;
                            if (NewImg == null)
                            {
                                Add_PLC_Tx_Message(Cam_Num, 10);
                                add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                return;
                            }
                            if (Capture_framebuffer[Cam_Num].Count > 0)
                            {
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                            Capture_framebuffer[Cam_Num].Add(NewImg);
                        }

                        #region LHJ - 240804 - 디버깅용
                        // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                        // 알고리즘을 다운 시키는 이미지가 있는지 확인
                        // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                        LVApp.Instance().m_Config._lastImage_Cam1[((uint)ctr_Camera_Setting2.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                        #endregion

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex] = NewImg;
                            MILCam1_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex] = NewImg;
                            MILCam3_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex] = NewImg;
                            MILCam4_GrabComplete(sender, e);
                        }
                        m_Job_Mode1 = 1;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode0 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode2 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode3 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM1 Grab Error!");
                    }
                }
                else
                {
                    DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }
                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;

                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode1 = 0;
                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_1.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log1 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log1[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log1[j + 1] = "";// LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();
                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log1[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log1[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc1();
                    }

                }
                //GC.Collect();
            }
            catch
            {
                //ctrCam2.t_check_grab = false;
            }
        }

        public void MILCam3_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 2;
                ctr_Camera_Setting3.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(6);

                DebugLogger.Instance().LogRecord("MIL CAM2 Grab: " + ctr_Camera_Setting3.Grab_Num.ToString());

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    if (LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex] != null)
                //    {
                //        Bitmap NewImg = null;
                //        {
                //            NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex]);
                //            LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, NewImg, 0);
                //        }
                //    }
                //}

                if (m_Job_Mode2 == 0)
                {
                    if (LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex] != null)
                    {
                        Bitmap NewImg = null;
                        {
                            //NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex]);
                            NewImg = LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex].Clone() as Bitmap;
                            if (NewImg == null)
                            {
                                Add_PLC_Tx_Message(Cam_Num, 10);
                                add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                return;
                            }
                            if (Capture_framebuffer[Cam_Num].Count > 0)
                            {
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                            Capture_framebuffer[Cam_Num].Add(NewImg);
                        }

                        #region LHJ - 240804 - 디버깅용
                        // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                        // 알고리즘을 다운 시키는 이미지가 있는지 확인
                        // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                        LVApp.Instance().m_Config._lastImage_Cam2[((uint)ctr_Camera_Setting3.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                        #endregion

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex] = NewImg;
                            MILCam1_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex] = NewImg;
                            MILCam2_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex] = NewImg;
                            MILCam4_GrabComplete(sender, e);
                        }
                        m_Job_Mode2 = 1;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode0 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode1 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode3 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM2 Grab Error!");
                    }
                }
                else
                {
                    DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }
                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;

                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode2 = 0;
                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_2.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log2 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log2[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log2[j + 1] = "";// LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();
                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log2[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log2[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc2();
                    }

                }
                //GC.Collect();
            }
            catch
            {
                //ctrCam2.t_check_grab = false;
            }
        }

        public void MILCam4_GrabComplete(object sender, EventArgs e)
        {
            try
            {
                int Cam_Num = 3;
                ctr_Camera_Setting4.Grab_Num++;
                LVApp.Instance().t_Util.CalculateFrameRate(7);

                DebugLogger.Instance().LogRecord("MIL CAM3 Grab: " + ctr_Camera_Setting4.Grab_Num.ToString());

                //if (LVApp.Instance().m_Config.m_Cam_Log_Method == 4)
                //{
                //    if (LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex] != null)
                //    {
                //        Bitmap NewImg = null;
                //        {
                //            NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex]);
                //            LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, NewImg, 0);
                //        }
                //    }
                //}

                if (m_Job_Mode3 == 0)
                {
                    if (LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex] != null)
                    {
                        Bitmap NewImg = null;
                        {
                            //NewImg = GetCopyOf1(LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex]);
                            NewImg = LVApp.Instance().m_MIL.CAM3_MilGrabBMPList[LVApp.Instance().m_MIL.CAM3_MilGrabBufferIndex].Clone() as Bitmap;
                            if (NewImg == null)
                            {
                                Add_PLC_Tx_Message(Cam_Num, 10);
                                add_Log("CAM" + Cam_Num.ToString() + " Grab Error!");
                                return;
                            }
                            if (Capture_framebuffer[Cam_Num].Count > 0)
                            {
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                            Capture_framebuffer[Cam_Num].Add(NewImg);
                        }

                        #region LHJ - 240804 - 디버깅용
                        // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                        // 알고리즘을 다운 시키는 이미지가 있는지 확인
                        // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                        LVApp.Instance().m_Config._lastImage_Cam3[((uint)ctr_Camera_Setting4.Grab_Num) % LV_Config._lastImageCount] = NewImg.Clone() as Bitmap;
                        #endregion

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM0_MilGrabBMPList[LVApp.Instance().m_MIL.CAM0_MilGrabBufferIndex] = NewImg;
                            MILCam1_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM1_MilGrabBMPList[LVApp.Instance().m_MIL.CAM1_MilGrabBufferIndex] = NewImg;
                            MILCam2_GrabComplete(sender, e);
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            LVApp.Instance().m_MIL.CAM2_MilGrabBMPList[LVApp.Instance().m_MIL.CAM2_MilGrabBufferIndex] = NewImg;
                            MILCam3_GrabComplete(sender, e);
                        }
                        m_Job_Mode3 = 1;

                        if (LVApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode0 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode1 = 1;
                        }
                        if (LVApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                        {
                            m_Probe_Job_Mode2 = 1;
                        }
                    }
                    else
                    {
                        Add_PLC_Tx_Message(Cam_Num, 10);
                        add_Log("CAM3 Grab Error!");
                    }
                }
                else
                {
                    DebugLogger.Instance().LogRecord("CAM" + Cam_Num.ToString() + " Miss!");
                    //add_Log("CAM" + Cam_Num.ToString() + " Miss!");
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        return;
                    }
                    LVApp.Instance().t_Util.Delay(ctr_PLC1.m_DELAYCAMMISS);
                    Add_PLC_Tx_Message(Cam_Num, -10);
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;

                    if (Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Capture_framebuffer[Cam_Num].Clear();
                    }
                    m_Job_Mode3 = 0;
                    if (LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                    {
                        LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num]++;
                        for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows.Count; i++)
                        {
                            if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i][2].ToString() == "" || LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i][0].ToString().Contains("alse"))
                            {
                                continue;
                            }

                            for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = (LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] + 1).ToString("000000000");
                                }

                                if (LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[j].ToString().Contains(LVApp.Instance().m_Config.ds_DATA_3.Tables[Cam_Num].Rows[i][2].ToString()))
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][3] != null)
                                    {
                                        LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j] = "";
                                    }
                                }
                            }
                        }
                        LVApp.Instance().m_Config.t_str_log3 = new string[LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count + 1];
                        LVApp.Instance().m_Config.t_str_log3[0] = LVApp.Instance().m_Config.m_lot_str; //LVApp.Instance().m_Config.t_str_log_Total[0] = LVApp.Instance().m_Config.m_lot_str;
                        int t_t_idx = 0;
                        for (int j = 0; j < LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count; j++)
                        {
                            LVApp.Instance().m_Config.t_str_log3[j + 1] = "";// LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[(int)(LVApp.Instance().m_Config.m_Log_Data_Cnt[Cam_Num] % (double)LVApp.Instance().m_Config.m_Log_Save_Num)][j].ToString();
                            if (j == 1)
                            {
                                if (!LVApp.Instance().m_Config.t_str_log3[j + 1].Contains("OK"))
                                {
                                    LVApp.Instance().m_Config.t_Result_log_Total[Cam_Num] = false;
                                }
                            }
                            else
                            {
                                LVApp.Instance().m_Config.t_str_log_Total[LVApp.Instance().m_Config.t_int_log_Total[Cam_Num] + t_t_idx] = LVApp.Instance().m_Config.t_str_log3[j + 1];
                                t_t_idx++;
                            }
                        }
                        LVApp.Instance().m_Config.t_bool_log_Total[Cam_Num] = false;
                        LVApp.Instance().m_Config.LogThreadProc3();
                    }

                }
                //GC.Collect();
            }
            catch
            {
                //ctrCam2.t_check_grab = false;
            }
        }

        private void pictureBox_LV_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.learningvision.co.kr");
        }

        private void dataGridView_AUTO_STATUS_SizeChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows)
            {
                row.Height = (LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Height - LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeight) / LVApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows.Count;
            }
        }

        private void dataGridView_AUTO_COUNT_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                int t_cnt = 0;
                foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows)
                {
                    if (row.Visible)
                    {
                        t_cnt++;
                    }
                }
                if (t_cnt > 0)
                {
                    foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows)
                    {
                        row.Height = (LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Height - LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersHeight) / LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows.Count;
                    }
                }
            }
            catch
            { }
        }

        //int pictureBox_Setting_0_Height = 320;
        private void splitContainer11_SizeChanged(object sender, EventArgs e)
        {
            //splitContainer11.SplitterDistance = 815;//splitContainer11.Width - (80 + pictureBox_Setting_0_Height * 4 / 3);
            //splitContainer11.SplitterDistance = splitContainer16.SplitterDistance = splitContainer19.SplitterDistance = splitContainer21.SplitterDistance = 815;
            //splitContainer19.SplitterDistance = splitContainer11.SplitterDistance;
            //splitContainer21.SplitterDistance = splitContainer11.SplitterDistance;
            //splitContainer23.SplitterDistance = splitContainer11.SplitterDistance;
            //splitContainer25.SplitterDistance = splitContainer11.SplitterDistance;
            //splitContainer27.SplitterDistance = splitContainer11.SplitterDistance;
            //splitContainer29.SplitterDistance = splitContainer11.SplitterDistance;
            //splitContainer33.SplitterDistance = splitContainer32.SplitterDistance = splitContainer31.SplitterDistance = 75;
            //splitContainer33.IsSplitterFixed = splitContainer32.IsSplitterFixed = splitContainer31.IsSplitterFixed = true;
        }

        int t_cur_MAIN_SelectedIndex = 0;
        private void neoTabWindow_MAIN_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            LVApp.Instance().m_Config.neoTabWindow_MAIN_idx = e.TabPageIndex;
            if (e.TabPageIndex != 1)
            {
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
            }
            if (e.TabPageIndex == 7)
            {
                neoTabWindow_MAIN.SelectedIndex = t_cur_MAIN_SelectedIndex;
                if (LVApp.Instance().m_help.m_hide_check || LVApp.Instance().m_help.WindowState == FormWindowState.Minimized)
                {
                    LVApp.Instance().m_help.Show();
                    LVApp.Instance().m_help.m_hide_check = false;
                    if (LVApp.Instance().m_help.WindowState == FormWindowState.Minimized)
                    {
                        LVApp.Instance().m_help.WindowState = FormWindowState.Normal;
                    }
                }
                else if (!LVApp.Instance().m_help.m_hide_check)
                {
                    LVApp.Instance().m_help.Hide();
                    LVApp.Instance().m_help.m_hide_check = true;
                }
            }
            else
            {
                if (e.TabPageIndex != 6)
                {
                    t_cur_MAIN_SelectedIndex = neoTabWindow_MAIN.SelectedIndex;
                }
            }
            if (e.TabPageIndex == 6)
            {
                neoTabWindow_MAIN.SelectedIndex = 0;
                if (LVApp.Instance().m_Config.m_Administrator_Password_Flag)
                {
                    LVApp.Instance().m_Config.m_Administrator_Password_Flag = false;
                    //LVApp.Instance().m_parameters.m_Administrator_GH_Password_Flag = false;
                    //ctr_PLC_Data1.button_Offset.Visible = false;
                    //ctr_PLC_Data1.neoTab_Camera_SelectedIndex = 5;
                    //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon;
                    //neoTabWindow_MAIN.TabPages[1].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[2].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[3].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[4].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[5].Enabled = false;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        neoTabWindow_MAIN.TabPages[6].Text = "Login";
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        neoTabWindow_MAIN.TabPages[6].Text = "登录";
                    }

                    //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
                    //LVApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = false;
                    //LVApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = false;
                }
                else
                {
                    Frm_Password m_Frm_Password = new Frm_Password();
                    m_Frm_Password.ShowDialog();

                    if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
                    {
                        //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon;
                        //neoTabWindow_MAIN.TabPages[1].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[2].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[3].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[4].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[5].Enabled = false;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Login";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            neoTabWindow_MAIN.TabPages[6].Text = "登录";
                        }
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
                        //LVApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = false;
                        //LVApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = false;
                    }
                    else
                    {
                        //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon2;
                        //neoTabWindow_MAIN.TabPages[1].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[2].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[3].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[4].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[5].Enabled = true;
                        int t_size = neoTabWindow_MAIN.TabPages[6].Width;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그아웃";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Logout";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            neoTabWindow_MAIN.TabPages[6].Text = "注销";
                        }
                        neoTabWindow_MAIN.TabPages[6].Width = t_size;
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Logoff");
                        //LVApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = true;
                        //LVApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = true;
                    }

                    //if (LVApp.Instance().m_Config.m_Administrator_GH_Password_Flag)
                    //{
                    //    //ctr_PLC_Data1.button_Offset.Visible = true;
                    //}
                    //else
                    //{
                    //    //ctr_PLC_Data1.button_Offset.Visible = false;
                    //}
                }
                neoTabWindow_MAIN.SelectedIndex = 0;
                LVApp.Instance().m_Config.neoTabWindow_MAIN_idx = 0;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
            }
            else if (e.TabPageIndex > 0 && e.TabPageIndex < 6 && e.TabPageIndex != 5 && e.TabPageIndex != 4)
            {
                if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
                {
                    //MessageBox.Show("관리자모드가 아닙니다. 관리자 로그인하세요!.");
                    Frm_Password m_Frm_Password = new Frm_Password();
                    m_Frm_Password.ShowDialog();
                    if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
                    {
                        t_setting_view_mode = false;
                        t_cam_setting_view_mode = false;
                        t_cam_ROI_view_mode = false;

                        neoTabWindow_MAIN.SelectedIndex = 0;
                        LVApp.Instance().m_Config.neoTabWindow_MAIN_idx = 0;
                    }

                    if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
                    {
                        //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon;
                        //neoTabWindow_MAIN.TabPages[1].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[2].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[3].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[4].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[5].Enabled = false;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Login";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            neoTabWindow_MAIN.TabPages[6].Text = "登录";
                        }
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
                        //LVApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = false;
                        //LVApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = false;
                        return;
                    }
                    else
                    {
                        //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon2;
                        //neoTabWindow_MAIN.TabPages[1].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[2].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[3].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[4].Enabled = true;
                        //neoTabWindow_MAIN.TabPages[5].Enabled = true;
                        int t_size = neoTabWindow_MAIN.TabPages[6].Width;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그아웃";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Logout";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            neoTabWindow_MAIN.TabPages[6].Text = "注销";
                        }
                        neoTabWindow_MAIN.TabPages[6].Width = t_size;
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Logoff");
                        //LVApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = true;
                        //LVApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = true;
                    }
                }
            }
            t_setting_view_mode = false;
            t_cam_setting_view_mode = false;
            t_cam_ROI_view_mode = false;
            if (e.TabPageIndex == 0)
            {
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
                //for (int i = 0; i < Application.OpenForms.Count; i++)
                //{
                //    Form f = Application.OpenForms[i];
                //    if (f.GetType() == typeof(Frm_Trackbar))
                //    {
                //        f.Close();
                //    }
                //}
                //this.Refresh();
            }
            else if (e.TabPageIndex == 1)
            {
                t_cam_ROI_view_mode = true;
                if (LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag)
                {
                    ctr_ROI1.m_advenced_param_visible = true; ctr_ROI1.Referesh_Select_Menu(true);
                    ctr_ROI2.m_advenced_param_visible = true; ctr_ROI2.Referesh_Select_Menu(true);
                    ctr_ROI3.m_advenced_param_visible = true; ctr_ROI3.Referesh_Select_Menu(true);
                    ctr_ROI4.m_advenced_param_visible = true; ctr_ROI4.Referesh_Select_Menu(true);
                }
                else
                {
                    ctr_ROI1.m_advenced_param_visible = false; ctr_ROI1.Referesh_Select_Menu(true);
                    ctr_ROI2.m_advenced_param_visible = false; ctr_ROI2.Referesh_Select_Menu(true);
                    ctr_ROI3.m_advenced_param_visible = false; ctr_ROI3.Referesh_Select_Menu(true);
                    ctr_ROI4.m_advenced_param_visible = false; ctr_ROI4.Referesh_Select_Menu(true);
                }
            }
            else if (e.TabPageIndex == 2)
            {
                //LVApp.Instance().m_Config.Load_Judge_Data();
                //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    //if (neoTabWindow_INSP_SETTING.SelectedIndex == 0)
                    //{
                    t_setting_view_mode = true;
                    t_cam_setting_view_mode = false;
                    //if (ctr_Log1.checkBox_Display.Checked)
                    //{
                    //    LVApp.Instance().m_Config.Realtime_View_Check = false;
                    //}
                    //}
                    //else
                    //{
                    //    t_setting_view_mode = false;
                    //    t_cam_setting_view_mode = false;
                    //    //if (ctr_Log1.checkBox_Display.Checked)
                    //    //{
                    //    //    LVApp.Instance().m_Config.Realtime_View_Check = true;
                    //    //}
                    //}

                    //neoTabWindow_EQUIP_SETTING.SelectedIndex = 2;
                    //neoTabWindow_ALG.SelectedIndex = 0;
                    //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                    //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                    //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                    //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                    //if (neoTabWindow_INSP_SETTING_CAM.SelectedIndex >= 0 && neoTabWindow_INSP_SETTING.SelectedIndex == 0)
                    //{
                    //    neoTabWindow_INSP_SETTING_CAM.TabPages[neoTabWindow_INSP_SETTING_CAM.SelectedIndex].Visible = true;
                    //}
                    //neoTabWindow2_LOG.TabPages[0].Visible = false;
                    //neoTabWindow2_LOG.TabPages[1].Visible = false;
                    //neoTabWindow2_LOG.TabPages[2].Visible = false;
                    //neoTabWindow2_LOG.TabPages[3].Visible = false;

                    //splitContainer11.SplitterDistance = splitContainer16.SplitterDistance = splitContainer19.SplitterDistance = splitContainer21.SplitterDistance = 700;

                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (!m_Last_Start_Loading)
                        {
                            LVApp.Instance().m_Config.Load_Judge_Data();
                            m_Last_Start_Loading = true;
                        }
                    }

                    if (LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag)
                    {
                        ctr_ROI1.m_advenced_param_visible = true; ctr_ROI1.Referesh_Select_Menu(true);
                        ctr_ROI2.m_advenced_param_visible = true; ctr_ROI2.Referesh_Select_Menu(true);
                        ctr_ROI3.m_advenced_param_visible = true; ctr_ROI3.Referesh_Select_Menu(true);
                        ctr_ROI4.m_advenced_param_visible = true; ctr_ROI4.Referesh_Select_Menu(true);
                    }
                    else
                    {
                        ctr_ROI1.m_advenced_param_visible = false; ctr_ROI1.Referesh_Select_Menu(true);
                        ctr_ROI2.m_advenced_param_visible = false; ctr_ROI2.Referesh_Select_Menu(true);
                        ctr_ROI3.m_advenced_param_visible = false; ctr_ROI3.Referesh_Select_Menu(true);
                        ctr_ROI4.m_advenced_param_visible = false; ctr_ROI4.Referesh_Select_Menu(true);
                    }
                }
            }
            else if (e.TabPageIndex == 3)
            {
                //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                if (neoTabWindow_EQUIP_SETTING.SelectedIndex == 0)
                {
                    t_cam_setting_view_mode = true;
                    t_setting_view_mode = false;
                    //if (ctr_Log1.checkBox_Display.Checked)
                    //{
                    //    LVApp.Instance().m_Config.Realtime_View_Check = true;
                    //}
                }
                else
                {
                    t_cam_setting_view_mode = false;
                    t_setting_view_mode = false;
                    //if (ctr_Log1.checkBox_Display.Checked)
                    //{
                    //    LVApp.Instance().m_Config.Realtime_View_Check = true;
                    //}
                }
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
            }
            //if (e.TabPageIndex != 1 && e.TabPageIndex != 2)
            //{
            //    t_cam_setting_view_mode = false;
            //    t_setting_view_mode = false;
            //    //if (ctr_Log1.checkBox_Display.Checked)
            //    //{
            //    //    LVApp.Instance().m_Config.Realtime_View_Check = true;
            //    //}
            //}

            if (e.TabPageIndex == 3)
            {
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
                //if (neoTabWindow2_LOG.SelectedIndex >= 0 && neoTabWindow_LOG.SelectedIndex == 0)
                //{
                //    neoTabWindow2_LOG.TabPages[neoTabWindow2_LOG.SelectedIndex].Visible = true;
                //}
            }

            if (e.TabPageIndex == 4)
            {
                //if (LVApp.Instance().m_Config.m_Cam_Total_Num == 1)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = false;
                //    ctr_Manual1.radioButton_CAM2.Visible = false;
                //    ctr_Manual1.radioButton_CAM3.Visible = false;
                //}
                //else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 2)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = true;
                //    ctr_Manual1.radioButton_CAM2.Visible = false;
                //    ctr_Manual1.radioButton_CAM3.Visible = false;
                //}
                //else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 3)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = true;
                //    ctr_Manual1.radioButton_CAM2.Visible = true;
                //    ctr_Manual1.radioButton_CAM3.Visible = false;
                //}
                //else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 4)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = true;
                //    ctr_Manual1.radioButton_CAM2.Visible = true;
                //    ctr_Manual1.radioButton_CAM3.Visible = true;
                //}
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
            }
        }

        public void button_RESET_Click(object sender, EventArgs e)
        {
            try
            {
                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    return;
                }
                string msg = "";
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    msg = "Data 초기화 하시겠습니까?";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    msg = "Do you want to reset data?";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    msg = "是否要重置数据?";
                }

                if (!Force_close)
                {
                    if (MessageBox.Show(msg, " RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                m_Monitoringthread_Check = false;
                Thread.Sleep(500);
                if (Monitoringthread.IsAlive)
                {
                    Monitoringthread.Abort();
                    Monitoringthread = null;
                }

                for (int i = 0; i < 4; i++)
                {
                    LVApp.Instance().m_Config.m_Error_Flag[i] = -1;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3] = 0;
                    LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[i] = 0;
                    LVApp.Instance().m_Config.Tx_Idx[i] = 0;
                    LVApp.Instance().m_Config.t_bool_log_Total[i] = true;
                }
                if (ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                {

                }
                else
                {
                    //if (t_v == 0)
                    {
                        ctr_PLC1.PLC_L_WRITE("LX1014", 1d); // RESET
                        Thread.Sleep(50);
                        //}
                        //else if (t_v == 1)
                        //{
                        ctr_PLC1.PLC_L_WRITE("LX1114", 1d); // RESET
                    }
                }

                for (int i = 0; i < 41; i++)
                {
                    LVApp.Instance().m_Config.m_GraphData0[i].name = "";
                    LVApp.Instance().m_Config.m_GraphData0[i].use = false;
                    LVApp.Instance().m_Config.m_GraphData0[i].list.Clear();
                    LVApp.Instance().m_Config.m_GraphData1[i].name = "";
                    LVApp.Instance().m_Config.m_GraphData1[i].use = false;
                    LVApp.Instance().m_Config.m_GraphData1[i].list.Clear();
                    LVApp.Instance().m_Config.m_GraphData2[i].name = "";
                    LVApp.Instance().m_Config.m_GraphData2[i].use = false;
                    LVApp.Instance().m_Config.m_GraphData2[i].list.Clear();
                    LVApp.Instance().m_Config.m_GraphData3[i].name = "";
                    LVApp.Instance().m_Config.m_GraphData3[i].use = false;
                    LVApp.Instance().m_Config.m_GraphData3[i].list.Clear();
                }

                ctr_Log1.Refresh_Log_Data();
                LVApp.Instance().m_Config.ds_LOG.Tables.Clear();
                LVApp.Instance().m_Config.ds_LOG.Clear();
                //ctr_PLC1.PLC_Thread_Stop();
                for (int i = 0; i < 4; i++)
                {
                    LVApp.Instance().m_Config.Initialize_Data_Log(i);
                }

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        ctr_Display_1.pictureBox_0.Image = null;
                        ctr_Display_1.pictureBox_1.Image = null;
                        ctr_Display_1.pictureBox_2.Image = null;
                        ctr_Display_1.pictureBox_3.Image = null;
                        //ctr_Display_1.pictureBox_4.Image = null;
                        //ctr_Display_1.pictureBox_Main.Image = null;
                        ctr_Display_1.Refresh();
                        dataGridView_AUTO_COUNT.Refresh();

                        //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = 0;
                        //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = 0;
                        //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = 0;
                        //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = 0;
                    });
                }
                else
                {
                    ctr_Display_1.pictureBox_0.Image = null;
                    ctr_Display_1.pictureBox_1.Image = null;
                    ctr_Display_1.pictureBox_2.Image = null;
                    ctr_Display_1.pictureBox_3.Image = null;
                    //ctr_Display_1.pictureBox_4.Image = null;
                    //ctr_Display_1.pictureBox_Main.Image = null;
                    ctr_Display_1.Refresh();
                    dataGridView_AUTO_COUNT.Refresh();

                    //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = 0;
                    //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = 0;
                    //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = 0;
                    //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = 0;
                }

                if (richTextBox_LOG.InvokeRequired)
                {
                    richTextBox_LOG.Invoke((MethodInvoker)delegate
                    {
                        richTextBox_LOG.ResetText();
                    });
                }
                else
                {
                    richTextBox_LOG.ResetText();
                }

                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows.Count; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][8] = 0;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][9] = 0;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][10] = 0;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][11] = 0;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][12] = 0;
                    }
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows.Count; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][8] = 0;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][9] = 0;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][10] = 0;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][11] = 0;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][12] = 0;
                    }
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows.Count; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][8] = 0;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][9] = 0;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][10] = 0;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][11] = 0;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][12] = 0;
                    }
                }
                if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows.Count; i++)
                    {
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][8] = 0;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][9] = 0;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][10] = 0;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][11] = 0;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][12] = 0;
                    }
                }
                //for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows.Count; i++)
                //{
                //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][8] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][9] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][10] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][11] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][12] = 0;
                //}
                //for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows.Count; i++)
                //{
                //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][8] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][9] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][10] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][11] = 0;
                //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][12] = 0;
                //}

                //ctr_PLC1.PLC_Thread_Start();

                #region 240814 - LHJ - 로그에 사용하는 영상 그랩 횟수 초기화
                ctr_Camera_Setting1.Grab_Num = 0D;
                ctr_Camera_Setting2.Grab_Num = 0D;
                ctr_Camera_Setting3.Grab_Num = 0D;
                ctr_Camera_Setting4.Grab_Num = 0D;

                for (int Cam_Num = 0; Cam_Num < 4; ++Cam_Num)
                {
                    if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                    {
                        _mergeImageCount[Cam_Num] = 0;
                    }
                }
                #endregion

                this.Refresh();

                Monitoringthread = new Thread(Monitoringthread_Proc);
                m_Monitoringthread_Check = true;
                Monitoringthread.IsBackground = true;
                Monitoringthread.Start();
            }
            catch
            {
                //if (!m_Monitoringthread_Check)
                //{
                //    Monitoringthread = new Thread(Monitoringthread_Proc);
                //    m_Monitoringthread_Check = true;
                //    Monitoringthread.IsBackground = true;
                //    Monitoringthread.Start();
                //}

            }
        }

        public void button_INSPECTION_Click(object sender, EventArgs e)
        {
            //if (!Camera_Connectio_check_flag)
            //{
            //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            //    {
            //        add_Log("카메라 연결을 점검하세요!");
            //        MessageBox.Show("카메라 연결을 점검하세요!");
            //    }
            //    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            //    {
            //        add_Log("Check Camera connection!");
            //        MessageBox.Show("Check Camera connection!");
            //    }
            //    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            //    {//중국어
            //        add_Log("检查摄像机连接!");
            //        MessageBox.Show("检查摄像机连接!");
            //    }
            //    return;
            //}

            //LVApp.Instance().m_Config.m_OK_NG_Cnt[0, 0] += 100;
            //LVApp.Instance().m_Config.m_OK_NG_Cnt[0, 1] += 30;
            //LVApp.Instance().m_Config.m_OK_NG_Cnt[1, 0] += 20;
            //LVApp.Instance().m_Config.m_OK_NG_Cnt[1, 1] += 129;
            if (!m_Monitoringthread_Check)
            {
                if (Monitoringthread.IsAlive)
                {
                    Monitoringthread.Abort();
                    Monitoringthread = null;
                }
                Monitoringthread = new Thread(Monitoringthread_Proc);
                m_Monitoringthread_Check = true;
                Monitoringthread.IsBackground = true;
                Monitoringthread.Start();
            }

            if (button_INSPECTION.Text == "검사 대기" || button_INSPECTION.Text == "START" || button_INSPECTION.Text == "开始")
            {
                t_cam_ROI_view_mode = false;
                neoTabWindow_MAIN.SelectedIndex = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[j])
                    {
                        t_Cam_CNT++;
                    }
                }
                button_RESET.Enabled = false;
                Properties.Settings.Default.Lot_No = textBox_LOT.Text;
                LVApp.Instance().m_Config.m_lot_str = textBox_LOT.Text;

                //for (int i = 0; i < Application.OpenForms.Count; i++)
                //{
                //    Form f = Application.OpenForms[i];
                //    if (f.GetType() == typeof(Frm_Trackbar))
                //    {
                //        f.Close();
                //    }
                //}
                //add_Log("Step_01");
                if (neoTabWindow_INSP_SETTING.SelectedIndex != 0)
                {
                    neoTabWindow_MAIN.SelectedIndex = 1;
                    neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                    neoTabWindow_MAIN.SelectedIndex = 0;
                }

                if (!Simulation_mode)
                {
                    if (!t_DongleKey.Check_License())
                    {

                        return;
                    }
                    //timer_Refresh_Amount.Interval = 500;
                }
                else
                {
                    //timer_Refresh_Amount.Interval = 1000 / 2;
                }
                //timer_Refresh_Amount.Interval = 100;
                //LVApp.Instance().m_Config.Load_Judge_Data();
                //LVApp.Instance().m_Config.Set_Parameters();
                m_Last_Start_Loading = true;

                ctr_Log1.Refresh_Log_Data();
                LVApp.Instance().m_Config.ds_LOG.Tables.Clear();
                LVApp.Instance().m_Config.ds_LOG.Clear();
                //ctr_PLC1.PLC_Thread_Stop();
                for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                {
                    Capture_framebuffer[i].Clear();
                    Result_framebuffer[i].Clear();
                    LVApp.Instance().m_Config.Tx_Idx[i] = 0;
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    {
                        LVApp.Instance().m_Config.Initialize_Data_Log(i);
                        LVApp.Instance().m_Config.m_Cam_Inspection_Check[i] = false;
                        LVApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                    }
                    if (ctr_PLC1.send_Message[i].Count > 0)
                    {
                        ctr_PLC1.send_Message[i].Clear();
                    }
                    if (ctr_PLC1.send_Idx[i].Count > 0)
                    {
                        ctr_PLC1.send_Idx[i].Clear();
                    }
                    if (ctr_PLC1.send_Message_Time[i].Count > 0)
                    {
                        ctr_PLC1.send_Message_Time[i].Clear();
                    }
                    if (ctr_PLC1.send_Idx_Time[i].Count > 0)
                    {
                        ctr_PLC1.send_Idx_Time[i].Clear();
                    }

                }
                LVApp.Instance().m_Config.CSV_Logfile_Initialize(4);
                //add_Log("Step_02");

                //LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                //LVApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
                //LVApp.Instance().m_mainform.ctr_Camera_Setting1.button_TRIGGER_DELAY_CHANGE_Click(sender, e);

                //int t_num = (int)ctr_PLC1.PLC_D_READ("DW5056", 2);//검사 총갯수
                //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = t_num;
                //LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[2] = t_num;
                //Thread.Sleep(20);
                //t_num = (int)ctr_PLC1.PLC_D_READ("DW5046", 2);//OK수
                //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = t_num;
                //LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[0] = t_num;
                //Thread.Sleep(20);
                //t_num = (int)ctr_PLC1.PLC_D_READ("DW5048", 2);//NG수
                //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = t_num;
                //LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[1] = t_num;
                //Thread.Sleep(20);
                //t_num = (int)ctr_PLC1.PLC_D_READ("DW5054", 2);//미처리수
                //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = t_num;
                //LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[3] = t_num;
                //int total = Convert.ToInt32(LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1].ToString());
                //double OK_ratio = 0;
                //if (total > 0)
                //{
                //    OK_ratio = ((double)t_num / (double)total) * 100d;
                //}
                //LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[4][1] = OK_ratio.ToString("00.00");

                ctr_Display_1.pictureBox_0.BackgroundImage = null;
                ctr_Display_1.pictureBox_1.BackgroundImage = null;
                ctr_Display_1.pictureBox_2.BackgroundImage = null;
                ctr_Display_1.pictureBox_3.BackgroundImage = null;
                //ctr_Display_1.pictureBox_Main.BackgroundImage = null;
                pictureBox_Setting_0.BackgroundImage = null;
                pictureBox_Setting_1.BackgroundImage = null;
                pictureBox_Setting_2.BackgroundImage = null;
                pictureBox_Setting_3.BackgroundImage = null;
                LVApp.Instance().m_Config.Set_Parameters();
                // LVApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);

                //LVApp.Instance().m_Config.Create_Save_Folders();
                //add_Log("Step_03");

                LVApp.Instance().m_Config.m_Check_Inspection_Mode = true;

                //LVApp.Instance().m_Config.Initialize_Data_Log();
                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1] && LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
                }
                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2] && LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
                }
                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3] && LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
                }
                if (LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4] && LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
                }
                LVApp.Instance().m_DIO.DI_worker_Initialized[0] = false;

                if (!ctr_Camera_Setting1.Force_USE.Checked)
                {
                    ctr_Camera_Setting1.checkBox1.Checked = true;
                    ctr_ROI1.button_LOAD_Click(sender, e);
                    if (ctr_PLC1.m_Protocal != (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                    {
                        ctr_Camera_Setting1.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                    }
                }
                if (!ctr_Camera_Setting2.Force_USE.Checked)
                {
                    ctr_Camera_Setting2.checkBox1.Checked = true;
                    ctr_ROI2.button_LOAD_Click(sender, e);
                    if (ctr_PLC1.m_Protocal != (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                    {
                        ctr_Camera_Setting2.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                    }
                }
                if (!ctr_Camera_Setting3.Force_USE.Checked)
                {
                    ctr_Camera_Setting3.checkBox1.Checked = true;
                    ctr_ROI3.button_LOAD_Click(sender, e);
                    if (ctr_PLC1.m_Protocal != (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                    {
                        ctr_Camera_Setting3.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                    }
                }
                if (!ctr_Camera_Setting4.Force_USE.Checked)
                {
                    ctr_Camera_Setting4.checkBox1.Checked = true;
                    ctr_ROI4.button_LOAD_Click(sender, e);
                    if (ctr_PLC1.m_Protocal != (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                    {
                        ctr_Camera_Setting4.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                    }
                }
                //if (!ctr_Camera_Setting5.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting5.checkBox1.Checked = true;
                //}

                ctr_PLC1.button_Send_Save_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Tx_Table_Enable(false);
                //add_Log("Step_04");
                if (!m_ImageSavethread_Check)
                {
                    bool t_space_check = true;
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                    {
                        t_space_check = Check_HD_available(LVApp.Instance().excute_path);
                    }
                    else
                    {
                        t_space_check = Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                    }
                    if (t_space_check)
                    {
                        LVApp.Instance().SAVE_IMAGE_List[0].Clear();
                        LVApp.Instance().SAVE_IMAGE_List[1].Clear();
                        LVApp.Instance().SAVE_IMAGE_List[2].Clear();
                        LVApp.Instance().SAVE_IMAGE_List[3].Clear();
                        LVApp.Instance().m_Config.Create_Save_Folders();
                        m_ImageSavethread_Check = false;
                        if (ImageSavethread != null && ImageSavethread.IsAlive)
                        {
                            ImageSavethread.Abort();
                            ImageSavethread = null;

                            ImageSavethread = new Thread(ImageSavethread_Proc);
                            m_ImageSavethread_Check = true;
                            ImageSavethread.IsBackground = true;
                            ImageSavethread.Start();
                            add_Log("Image save thread restart!");
                        }
                    }
                }


                Inspection_Thread_Start();



                Thread.Sleep(100);
                if (!Simulation_mode)
                {
                    if (!ctr_Camera_Setting1.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[0] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[0] == 0))
                    {
                        ctr_Camera_Setting1.toolStripButtonContinuousShot_Click(sender, e);
                    }
                    if (!ctr_Camera_Setting2.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[1] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[1] == 1))
                    {
                        ctr_Camera_Setting2.toolStripButtonContinuousShot_Click(sender, e);
                    }
                    if (!ctr_Camera_Setting3.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[2] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[2] == 2))
                    {
                        ctr_Camera_Setting3.toolStripButtonContinuousShot_Click(sender, e);
                    }
                    if (!ctr_Camera_Setting4.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[3] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[3] == 3))
                    {
                        ctr_Camera_Setting4.toolStripButtonContinuousShot_Click(sender, e);
                    }
                }
                //if (!ctr_Camera_Setting5.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting5.toolStripButtonContinuousShot_Click(sender, e);
                //}
                //ctr_ROI1.Enable_control(false);
                //ctr_ROI2.Enable_control(false);
                //ctr_ROI3.Enable_control(false);
                //ctr_ROI4.Enable_control(false);
                //add_Log("Step_05");

                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);

                ctr_Display_1.pictureBox_0.BackgroundImage = null;
                ctr_Display_1.pictureBox_1.BackgroundImage = null;
                ctr_Display_1.pictureBox_2.BackgroundImage = null;
                ctr_Display_1.pictureBox_3.BackgroundImage = null;
                //ctr_Display_1.pictureBox_Main.BackgroundImage = null;
                pictureBox_Setting_0.BackgroundImage = null;
                pictureBox_Setting_1.BackgroundImage = null;
                pictureBox_Setting_2.BackgroundImage = null;
                pictureBox_Setting_3.BackgroundImage = null;

                richTextBox_LOG.ResetText();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    button_INSPECTION.Text = "검 사 중...";
                    add_Log("검사를 시작하였습니다.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    button_INSPECTION.Text = "RUNNING...";
                    add_Log("Inspection started.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    button_INSPECTION.Text = "运行...";
                    add_Log("检查开始.");
                }
                button_INSPECTION.BackgroundImage = LV_Inspection_System.Properties.Resources.Button_BG2;

                //if (Simulation_mode)
                //{
                //    for (int i = 0; i < 4; i++)
                //    {
                //        timer_Cam[i].Interval = 1000/10;
                //        timer_Cam[i].Enabled = true;
                //    }
                //}

                int t_v = LVApp.Instance().m_mainform.ctr_PLC1.m_Pingpong_Num;

                if (ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.LVDIO)
                {
                    if (LVApp.Instance().m_DIO.m_DoportData.Count > 0)
                    {
                        LVApp.Instance().m_DIO.m_DoportData[0].Set(0, true);
                        LVApp.Instance().m_DIO.Do_Job_Mode = 1;
                    }
                }
                else
                {
                    if (t_v == 0)
                    {
                        ctr_PLC1.PLC_L_WRITE("LX1010", 1); // 검사시작
                        ctr_PLC1.PLC_L_WRITE("LX1011", 0); // 검사중지
                    }
                    else if (t_v == 1)
                    {
                        ctr_PLC1.PLC_L_WRITE("LX1110", 1); // 검사시작
                        ctr_PLC1.PLC_L_WRITE("LX1111", 0); // 검사중지
                    }
                }
            }
            else
            {
                string msg = "";
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    msg = "검사를 정지 하시겠습니까?";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    msg = "Do you want to stop?";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    msg = "是否要停止?";
                }

                if (!Force_close && !LVApp.Instance().m_Config.m_Check_Server_Operation)
                {
                    if (MessageBox.Show(msg, " STOP", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                button_RESET.Enabled = true;

                //if (Simulation_mode)
                //{
                //    for (int i = 0; i < 4; i++)
                //    {
                //        timer_Cam[i].Enabled = false;
                //    }
                //}

                //Thread.Sleep(100);
                ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
                ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
                ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
                ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
                ctrCam1.t_check_grab = false;
                ctrCam2.t_check_grab = false;
                ctrCam3.t_check_grab = false;
                ctrCam4.t_check_grab = false;

                if (!ctr_Camera_Setting1.Force_USE.Checked)
                {
                    ctr_Camera_Setting1.checkBox1.Checked = false;
                    //ctr_ROI1.button_LOAD_Click(sender, e);
                }
                if (!ctr_Camera_Setting2.Force_USE.Checked)
                {
                    ctr_Camera_Setting2.checkBox1.Checked = false;
                    //ctr_ROI2.button_LOAD_Click(sender, e);
                }
                if (!ctr_Camera_Setting3.Force_USE.Checked)
                {
                    ctr_Camera_Setting3.checkBox1.Checked = false;
                    //ctr_ROI3.button_LOAD_Click(sender, e);
                }
                if (!ctr_Camera_Setting4.Force_USE.Checked)
                {
                    ctr_Camera_Setting4.checkBox1.Checked = false;
                    //ctr_ROI4.button_LOAD_Click(sender, e);
                }

                LVApp.Instance().m_Config.CSV_Logfile_Terminate();
                //Thread.Sleep(500);
                ////Inspection_Thread_Stop();

                //if (!ctr_Camera_Setting1.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting1.checkBox1.Checked = false;
                //}
                //if (!ctr_Camera_Setting2.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting2.checkBox1.Checked = false;
                //}
                //if (!ctr_Camera_Setting3.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting3.checkBox1.Checked = false;
                //}
                //if (!ctr_Camera_Setting4.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting4.checkBox1.Checked = false;
                //}

                //ctr_Camera_Setting5.toolStripButtonStop_Click(sender, e);


                //ctr_PLC1.PLC_L_WRITE("LX1010", 0); // 검사시작
                //ctr_PLC1.PLC_L_WRITE("LX1011", 1); // 검사중지
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    button_INSPECTION.Text = "검사 대기";
                    add_Log("검사를 정지하였습니다.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    button_INSPECTION.Text = "START";
                    add_Log("Inspection stopped.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    button_INSPECTION.Text = "开始";
                    add_Log("检查已停止.");
                }
                button_INSPECTION.BackgroundImage = LV_Inspection_System.Properties.Resources.Button_BG;

                int t_v = LVApp.Instance().m_mainform.ctr_PLC1.m_Pingpong_Num;

                if (ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.LVDIO)
                {
                    if (LVApp.Instance().m_DIO.m_DoportData.Count > 0)
                    {
                        LVApp.Instance().m_DIO.m_DoportData[0].Set(0, false);
                        LVApp.Instance().m_DIO.Do_Job_Mode = 1;
                    }
                    //ctr_PLC1.PLC_D_WRITE("EW0004", 1, 0);
                }
                else
                {
                    if (t_v == 0)
                    {
                        ctr_PLC1.PLC_L_WRITE("LX1010", 1); // 검사시작
                        ctr_PLC1.PLC_L_WRITE("LX1011", 0); // 검사중지
                    }
                    else if (t_v == 1)
                    {
                        ctr_PLC1.PLC_L_WRITE("LX1110", 1); // 검사시작
                        ctr_PLC1.PLC_L_WRITE("LX1111", 0); // 검사중지
                    }
                }
                LVApp.Instance().m_mainform.ctr_PLC1.MC_Rx_Tx_Table_Enable(true);

                LVApp.Instance().m_Config.m_Check_Inspection_Mode = false;

                pictureBox_Setting_0.AllowDrop = true;
                pictureBox_Setting_1.AllowDrop = true;
                pictureBox_Setting_2.AllowDrop = true;
                pictureBox_Setting_3.AllowDrop = true;

                //ctr_ROI1.Enable_control(true);
                //ctr_ROI2.Enable_control(true);
                //ctr_ROI3.Enable_control(true);
                //ctr_ROI4.Enable_control(true);

                //ctr_Display_1.pictureBox_0.Image = null;
                //ctr_Display_1.pictureBox_1.Image = null;
                //ctr_Display_1.pictureBox_2.Image = null;
                //ctr_Display_1.pictureBox_Main.Image = null;
                //ctr_Display_1.pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                //ctr_Display_1.pictureBox_1.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                //ctr_Display_1.pictureBox_2.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                //ctr_Display_1.pictureBox_Main.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                //LVApp.Instance().m_Config.Stop_Save_Log();
            }
        }

        private int t_refresh_count = 0;
        private int t_pooling_idx = 0;
        private int t_csv_init_check = 0;
        private void timer_Refresh_Amount_Tick(object sender, EventArgs e)
        {
            //return;
            try
            {
                //if (m_Job_Mode0 == 0 && m_Job_Mode1 == 0 && m_Job_Mode2 == 0 && m_Job_Mode3 == 0)
                //{
                //    GC.Collect();
                //}
                if (t_Auto_Start_CNT == 17)
                {
                    t_Auto_Start_CNT = -1;
                    //if (LVApp.Instance().m_Config.m_Model_Name.Length > 0 && Properties.Settings.Default.Last_Model_Name.Length > 0)
                    {
                        Auto_Start_when_Start();
                    }
                }
                else
                {
                    if (t_Auto_Start_CNT >= 0)
                    {
                        t_Auto_Start_CNT++;
                    }
                }

                //CurrencyManager currencyManager0 = (CurrencyManager)dataGridView_AUTO_COUNT.BindingContext[dataGridView_AUTO_COUNT.DataSource];
                // currencyManager0.SuspendBinding();

                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && !Simulation_mode)
                {
                    double t_val_cnt = 0; double t_min_cnt = 9999999999999; double t_max_cnt = 0;
                    int t_min_idx = -1;
                    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            t_val_cnt = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                            //if (double.TryParse(LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"].ToString(), out t_val_cnt))
                            {
                                if (t_val_cnt >= 0 && t_val_cnt <= t_min_cnt)
                                {
                                    t_min_cnt = t_val_cnt;
                                    t_min_idx = i;
                                }
                                if (t_val_cnt >= 0 && t_val_cnt >= t_max_cnt)
                                {
                                    t_max_cnt = t_val_cnt;
                                }
                            }
                        }
                    }
                    if (t_min_idx > -1 && Math.Abs(t_max_cnt - t_min_cnt) >= LVApp.Instance().m_Config.CAM_Refresh_CNT && LVApp.Instance().m_Config.CAM_Refresh_CNT > 0)
                    {
                        if (t_min_idx == 0 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                        {
                            ctr_Camera_Setting1.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting1.toolStripButtonContinuousShot_Click(null, null);
                        }
                        else if (t_min_idx == 1 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                        {
                            ctr_Camera_Setting2.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting2.toolStripButtonContinuousShot_Click(null, null);
                        }
                        else if (t_min_idx == 2 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                        {
                            ctr_Camera_Setting3.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting3.toolStripButtonContinuousShot_Click(null, null);
                        }
                        else if (t_min_idx == 3 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_min_idx])
                        {
                            ctr_Camera_Setting4.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting4.toolStripButtonContinuousShot_Click(null, null);
                        }
                        LVApp.Instance().m_Config.m_OK_NG_Cnt[t_min_idx, 1] += Math.Abs(t_max_cnt - t_min_cnt);
                    }
                }

                //if (this.InvokeRequired)
                //{
                //    this.Invoke((MethodInvoker)delegate
                //    {
                //
                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i][2] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i][1] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 1][1] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 3];
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][1] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                            if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][2] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                            }
                            else
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3 * i + 2][2] = 0;
                            }
                            LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2 * i][2];//검사수량
                            LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2 * i][1];//양품수량
                            LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2 * i + 1][1];//불량수량
                            LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2 * i + 1][2];//수율
                        }
                    }
                    //i = 1;
                    //if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //{
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //    if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //    }
                    //    else
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //    }
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //}
                    //i = 2;
                    //if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //{
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //    if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //    }
                    //    else
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //    }
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //}
                    //i = 3;
                    //if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    //{
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //    if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                    //    }
                    //    else
                    //    {
                    //        LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                    //    }
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][0] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"];//검사수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][1] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"];//양품수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][2] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"];//불량수량
                    //    LVApp.Instance().m_Config.ds_YIELD.Tables[i + 4].Rows[0][3] = LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"];//수율
                    //}
                }
                //currencyManager0.ResumeBinding();
                //    });
                //}
                //else
                //{
                //    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                //    {
                //        if (i == 0 && m_Job_Mode0 == 0)
                //        {
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //        if (i == 1 && m_Job_Mode1 == 0)
                //        {
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //        if (i == 2 && m_Job_Mode2 == 0)
                //        {
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //        if (i == 3 && m_Job_Mode3 == 0)
                //        {
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //    }
                //}


                // Option needs
                //if (LVApp.Instance().m_Ctr_Mysql.m_Connected_flag && LVApp.Instance().m_Ctr_Mysql.conn != null)
                //{
                //    if (t_refresh_count == 0)
                //    {
                //        LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_STATUS.Tables[1], "Status");
                //    }
                //    else if (t_refresh_count == 1 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                //    {
                //        LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_0.Tables[1], "CAM0_setting_bt");
                //    }
                //    else if (t_refresh_count == 2 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                //    {
                //        LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_1.Tables[1], "CAM1_setting_bt");
                //    }
                //    else if (t_refresh_count == 3 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                //    {
                //        LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_2.Tables[1], "CAM2_setting_bt");
                //    }
                //    else if (t_refresh_count == 4 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                //    {
                //        LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_DATA_3.Tables[1], "CAM3_setting_bt");
                //    }
                //}

                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    bool t_space_check = true;
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                    {
                        t_space_check = Check_HD_available(LVApp.Instance().excute_path);
                    }
                    else
                    {
                        t_space_check = Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                    }

                    if (!m_ImageSavethread_Check)
                    {
                        if (t_space_check)
                        {
                            LVApp.Instance().SAVE_IMAGE_List[0].Clear();
                            LVApp.Instance().SAVE_IMAGE_List[1].Clear();
                            LVApp.Instance().SAVE_IMAGE_List[2].Clear();
                            LVApp.Instance().SAVE_IMAGE_List[3].Clear();
                            LVApp.Instance().m_Config.Create_Save_Folders();
                            if (ImageSavethread != null)
                            {
                                ImageSavethread = null;
                            }
                            ImageSavethread = new Thread(ImageSavethread_Proc);
                            m_ImageSavethread_Check = true;
                            ImageSavethread.IsBackground = true;
                            ImageSavethread.Start();
                            add_Log("Image save thread restart!");
                        }
                    }

                    if (t_space_check)
                    {
                        String m_Log_folder = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                        if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                        {
                            m_Log_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                        }

                        DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            LVApp.Instance().m_Config.Create_Save_Folders();
                            t_csv_init_check = 2;
                        }

                        if (t_csv_init_check == 2)
                        {
                            for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                            {
                                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                                {
                                    String m_Log_File_Name = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + i.ToString() + ".csv"; //파일경로
                                    if (LVApp.Instance().m_Config.m_Log_Save_Folder != "")
                                    {
                                        m_Log_File_Name = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\" + "CAM" + i.ToString() + ".csv"; //파일경로
                                    }

                                    FileInfo templateFile = new FileInfo(m_Log_File_Name);

                                    if (!templateFile.Exists)
                                    {
                                        //LVApp.Instance().m_Config.t_try_cnt = 0;

                                        //if (this.InvokeRequired)
                                        //{
                                        //    this.Invoke((MethodInvoker)delegate
                                        //    {
                                        //        LVApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                                        //    });
                                        //}
                                        //else
                                        //{
                                        LVApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                                        //}
                                    }
                                }
                            }
                            //if (this.InvokeRequired)
                            //{
                            //    this.Invoke((MethodInvoker)delegate
                            //    {
                            //        LVApp.Instance().m_Config.CSV_Logfile_Initialize(4);
                            //    });
                            //}
                            //else
                            //{
                            LVApp.Instance().m_Config.CSV_Logfile_Initialize(4);
                            //}
                        }
                        t_csv_init_check--;
                        if (t_csv_init_check < 0)
                        {
                            t_csv_init_check = 0;
                        }
                    }
                }
                //if (t_refresh_count == 1 || t_refresh_count == 4)
                //{
                //    //GC.Collect();
                //}
                //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (t_refresh_count == 0 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                    {
                        LVApp.Instance().m_mainform.ctr_DataGrid1.Min_Max_Update(t_refresh_count);
                    }
                    else if (t_refresh_count == 1 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                    {
                        LVApp.Instance().m_mainform.ctr_DataGrid2.Min_Max_Update(t_refresh_count);
                    }
                    else if (t_refresh_count == 2 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                    {
                        LVApp.Instance().m_mainform.ctr_DataGrid3.Min_Max_Update(t_refresh_count);
                    }
                    else if (t_refresh_count == 3 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                    {
                        LVApp.Instance().m_mainform.ctr_DataGrid4.Min_Max_Update(t_refresh_count);
                    }
                }
                t_refresh_count++;
                t_refresh_count %= 5;

                if (t_pooling_idx == 2)
                {
                    ctr_PLC1.poolingforconnection();
                    t_pooling_idx = 0;
                }
                else
                {
                    t_pooling_idx++;
                }


                //int t_day = Math.Abs(DateTime.Now.DayOfYear - LVApp.Instance().m_Config.t_Create_Save_Folders_oldtime.DayOfYear);
                //if (t_day >= 1)
                //{
                //    LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
                //    LVApp.Instance().m_Config.t_Create_Save_Folders_oldtime = DateTime.Now;
                //}
                if (Simulation_mode)
                {
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        //LVApp.Instance().m_Config.Initialize_Data_Log();
                        if (!ctr_Camera_Setting1.Force_USE.Checked && ctr_ROI1.pictureBox_Image.Image != null)
                        {
                            Bitmap t_Image = null;
                            if (ctr_ROI1.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb)
                            {
                                t_Image = ConvertTo24((Bitmap)ctr_ROI1.pictureBox_Image.Image.Clone());
                            }
                            else
                            {
                                t_Image = (Bitmap)ctr_ROI1.pictureBox_Image.Image.Clone();
                            }
                            ctrCam1.m_bitmap = t_Image;
                            ctrCam1_GrabComplete(sender, e);
                        }
                        if (!ctr_Camera_Setting2.Force_USE.Checked && ctr_ROI2.pictureBox_Image.Image != null)
                        {
                            Bitmap t_Image = null;
                            if (ctr_ROI2.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb)
                            {
                                t_Image = ConvertTo24((Bitmap)ctr_ROI2.pictureBox_Image.Image.Clone());
                            }
                            else
                            {
                                t_Image = (Bitmap)ctr_ROI2.pictureBox_Image.Image.Clone();
                            }
                            ctrCam2.m_bitmap = t_Image;
                            ctrCam2_GrabComplete(sender, e);
                        }
                        if (!ctr_Camera_Setting3.Force_USE.Checked && ctr_ROI3.pictureBox_Image.Image != null)
                        {
                            Bitmap t_Image = null;
                            if (ctr_ROI3.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb)
                            {
                                t_Image = ConvertTo24((Bitmap)ctr_ROI3.pictureBox_Image.Image.Clone());
                            }
                            else
                            {
                                t_Image = (Bitmap)ctr_ROI3.pictureBox_Image.Image.Clone();
                            }
                            ctrCam3.m_bitmap = t_Image;
                            ctrCam3_GrabComplete(sender, e);
                        }
                        if (!ctr_Camera_Setting4.Force_USE.Checked && ctr_ROI4.pictureBox_Image.Image != null)
                        {
                            Bitmap t_Image = null;
                            if (ctr_ROI4.pictureBox_Image.Image.PixelFormat == PixelFormat.Format32bppRgb)
                            {
                                t_Image = ConvertTo24((Bitmap)ctr_ROI4.pictureBox_Image.Image.Clone());
                            }
                            else
                            {
                                t_Image = (Bitmap)ctr_ROI4.pictureBox_Image.Image.Clone();
                            }
                            ctrCam4.m_bitmap = t_Image;
                            ctrCam4_GrabComplete(sender, e);
                        }
                    }
                }
                ////GC.Collect();
                return;
                //table0.Rows.Add("전체", 0);//0
                //table0.Rows.Add("-양품", 0);//1
                //table0.Rows.Add("-불량", 0);//2
                //table0.Rows.Add("-미처리", 0);//3
                //table0.Rows.Add("수율(%)", 0);//4

                //if (LVApp.Instance().m_mainform.m_Job_Mode[0] == 0)
                //{
                //    LVApp.Instance().m_mainform.m_Job_Mode[0] = 1;
                //}
                //else
                //{
                //    add_Log("CAM0 미처리");
                //}
                //if (LVApp.Instance().m_mainform.m_Job_Mode[1] == 0)
                //{
                //    LVApp.Instance().m_mainform.m_Job_Mode[1] = 1;
                //}
                //else
                //{
                //    add_Log("CAM1 미처리");
                //}
                //if (LVApp.Instance().m_mainform.m_Job_Mode[2] == 0)
                //{
                //    LVApp.Instance().m_mainform.m_Job_Mode[2] = 1;
                //}
                //else
                //{
                //    add_Log("CAM2 미처리");
                //}
                ////for (int i = 0; i < 12; i++)
                ////{
                //    if (LVApp.Instance().m_mainform.m_Job_Mode[3] == 0)
                //    {
                //        LVApp.Instance().m_mainform.m_Job_Mode[3] = 1;
                //    }
                //    else
                //    {
                //        add_Log("CAM3 미처리");
                //    }
                //    if (LVApp.Instance().m_mainform.m_Job_Mode[4] == 0)
                //    {
                //        LVApp.Instance().m_mainform.m_Job_Mode[4] = 1;
                //    }
                //    else
                //    {
                //        add_Log("CAM4 미처리");
                //    }
                //    //    Thread.Sleep(2000 / 12);
                //////}
                //LVApp.Instance().m_Config.Add_Log_Data(0, "");
                //return;
                //double t_ng_max = 0; double t_ng_max_idx = -1;
                //double t_ok_min = 999999999; int t_ok_min_idx = -1;
                //double t_miss_max = 0; int t_miss_max_idx = -1;
                //for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                //{
                //    if (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] >= t_ng_max)
                //    {
                //        t_ng_max = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //        t_ng_max_idx = i;
                //    }
                //    if (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] <= t_ok_min)
                //    {
                //        t_ok_min = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //        t_ok_min_idx = i;
                //    }
                //    if (LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] >= t_miss_max)
                //    {
                //        t_miss_max = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 2];
                //        t_miss_max_idx = i;
                //    }
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //    //ctr_PLC1.send_Message.Add("DW510" + i.ToString() + "_" + "40");
                //}

                //if (t_ng_max_idx == t_ok_min_idx)
                //{
                //    //return;
                //    //Thread.Sleep(20);
                //    //int t_num = (int)PLC_D_READ("DW5056", 2);//검사 총갯수
                //    if (LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[2] != t_ng_max + t_ok_min)
                //    {
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = t_ng_max + t_ok_min;
                //        LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[2] = t_ng_max + t_ok_min;
                //    }
                //    //Thread.Sleep(20);
                //    //t_num = (int)PLC_D_READ("DW5046", 2);//OK수
                //    if (LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[0] != t_ok_min)
                //    {
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = t_ok_min;
                //        LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[0] = t_ok_min;
                //    }
                //    //Thread.Sleep(20);
                //    //t_num = (int)PLC_D_READ("DW5048", 2);//NG수
                //    if (LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[1] != t_ng_max)
                //    {
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = t_ng_max;
                //        LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[1] = t_ng_max;
                //    }
                //    //Thread.Sleep(20);
                //    //t_num = (int)PLC_D_READ("DW5054", 2);//미처리수
                //    if (LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[3] != t_miss_max)
                //    {
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = t_miss_max;
                //        LVApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[3] = t_ng_max;
                //    }
                //    double total = Convert.ToInt32(LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1].ToString());
                //    double OK_ratio = 0;
                //    if (total > 0)
                //    {
                //        OK_ratio = (double.Parse(LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1].ToString()) / total) * 100d;
                //    }
                //    LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[4][1] = OK_ratio.ToString("00.00");
                //}


                //return;

                //if (!LVApp.Instance().m_Config.m_Cam_Inspection_Check[0]
                //    && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[1]
                //    && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[2]
                //    && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[3]
                //    && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[4])
                ////&& !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                //{
                //    if (ctr_PLC1.m_Cam_Trigger_Num < 3)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 3;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(94);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5056", 2);//검사 총갯수
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = t_num;
                //    }
                //    else if (ctr_PLC1.m_Cam_Trigger_Num == 3)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 4;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(95);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5046", 2);//OK수
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = t_num;

                //        if (LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] == null)
                //        {
                //            ctr_PLC1.m_Trigger_Check = false;
                //            return;
                //        }
                //        int total = Convert.ToInt32(LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1].ToString());
                //        double OK_ratio = 0;
                //        if (total > 0)
                //        {
                //            OK_ratio = ((double)t_num / (double)total) * 100d;
                //        }
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[4][1] = OK_ratio.ToString("00.00");
                //    }
                //    else if (ctr_PLC1.m_Cam_Trigger_Num == 4)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 5;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(96);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5048", 2);//NG수
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = t_num;
                //    }
                //    else if (ctr_PLC1.m_Cam_Trigger_Num == 5)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 2;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(94);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5054", 2);//미처리수
                //        LVApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = t_num;
                //    }
                //    ctr_PLC1.m_Trigger_Check = false;
                //}

                //else if (ctr_PLC1.m_Cam_Trigger_Num == 6)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 7;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5000", 2);//Cam0 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5002", 2);//Cam0 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5004", 2);//Cam0 NG
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[0]["TOTAL"] = t_num;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[0]["OK"] = t_OK;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[0]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 7)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 8;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5012", 2);//Cam1 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5014", 2);//Cam1 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5016", 2);//Cam1 NG
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[1]["TOTAL"] = t_num;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[1]["OK"] = t_OK;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[1]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 8)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 9;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5024", 2);//Cam2 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5026", 2);//Cam2 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5028", 2);//Cam2 NG
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2]["TOTAL"] = t_num;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2]["OK"] = t_OK;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 9)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 10;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5036", 2);//Cam3 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5038", 2);//Cam3 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5040", 2);//Cam3 NG
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3]["TOTAL"] = t_num;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3]["OK"] = t_OK;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 10)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 2;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5060", 2);//Cam4 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5062", 2);//Cam4 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5064", 2);//Cam4 NG
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[4]["TOTAL"] = t_num;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[4]["OK"] = t_OK;
                //    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[4]["NG"] = t_NG;
                //}
                //ctr_PLC1.m_Trigger_Check = false;
            }
            catch
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

        bool Camera_Connectio_check_flag = true;
        public void Camera_Connection_Check()
        {
            Camera_Connectio_check_flag = true;

            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1 && !ctr_Camera_Setting1.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[0] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[0] == 0))
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 4)
                {
                    if (!LVApp.Instance().m_MIL.CAM0_Initialized)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[0] == 6)
                {
                    if (!LVApp.Instance().m_GenICam.CAM[0].Connection)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else
                {
                    if (!LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2 && !ctr_Camera_Setting2.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[1] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[1] == 1))
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 4)
                {
                    if (!LVApp.Instance().m_MIL.CAM1_Initialized)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[1] == 6)
                {
                    if (!LVApp.Instance().m_GenICam.CAM[1].Connection)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else
                {

                    if (!LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3 && !ctr_Camera_Setting3.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[2] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[2] == 2))
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 4)
                {
                    if (!LVApp.Instance().m_MIL.CAM2_Initialized)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[2] == 6)
                {
                    if (!LVApp.Instance().m_GenICam.CAM[2].Connection)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else
                {
                    if (!LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4 && !ctr_Camera_Setting4.Force_USE.Checked && (LVApp.Instance().m_Config.m_Interlock_Cam[3] == -1 || LVApp.Instance().m_Config.m_Interlock_Cam[3] == 3))
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 4)
                {
                    if (!LVApp.Instance().m_MIL.CAM3_Initialized)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[3] == 6)
                {
                    if (!LVApp.Instance().m_GenICam.CAM[3].Connection)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
                else
                {
                    if (!LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        Camera_Connectio_check_flag = false;
                    }
                }
            }
            //if (!LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
            //{
            //    check = false;
            //}
            //if (!LVApp.Instance().m_mainform.ctrCam5.m_imageProvider.IsOpen)
            //{
            //    check = false;
            //}

            if (Camera_Connectio_check_flag)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["값"] = "정상 연결";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"] = "Conn.";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"] = "Conn.";
                }

                // button_INSPECTION_Click(null, null);
            }
            else
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["값"] = "에러";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"] = "Error";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"] = "Error";
                }
            }
        }

        private void neoTabWindow_EQUIP_SETTING_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            if (neoTabWindow_EQUIP_SETTING.SelectedIndex == 0)
            {
                t_cam_setting_view_mode = true;
                t_setting_view_mode = false;
                //if (ctr_Log1.checkBox_Display.Checked)
                //{
                //    LVApp.Instance().m_Config.Realtime_View_Check = true;
                //}
            }
            else
            {
                t_cam_setting_view_mode = false;
                t_setting_view_mode = false;
                //if (ctr_Log1.checkBox_Display.Checked)
                //{
                //    LVApp.Instance().m_Config.Realtime_View_Check = true;
                //
            }

            //if (e.TabPageIndex == 0)
            //{
            //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //    {
            //        t_cam_setting_view_mode = true;
            //        t_setting_view_mode = false;
            //        //neoTabWindow_EQUIP_SETTING.SelectedIndex = 2;
            //        //MessageBox.Show("검사중에는 카메라 설정을 할 수 없습니다.");
            //    }
            //}
            //else
            //{
            //    t_cam_setting_view_mode = false;
            //}
        }

        public void Inspection_Thread_Start()
        {
            DebugLogger.Instance().LogRecord("Inspection Thread Start");
            if (m_Threads_Check[0])
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                if (threads[i] != null && threads[i].IsAlive)
                {
                    threads[i].Abort();
                    threads[i] = null;
                }
                //Viewthreads[i] = null;
                if (Probe_threads[i] != null && Probe_threads[i].IsAlive)
                {
                    Probe_threads[i].Abort();
                    Probe_threads[i] = null;
                }

                m_Threads_Check[i] = true;
                if (Run_SW[i] == null)
                {
                    Run_SW[i] = new Stopwatch();
                    Run_SW[i].Reset();
                }
            }
            m_Job_Mode0 = 0; m_Result_Job_Mode0 = 0;
            m_Job_Mode1 = 0; m_Result_Job_Mode1 = 0;
            m_Job_Mode2 = 0; m_Result_Job_Mode2 = 0;
            m_Job_Mode3 = 0; m_Result_Job_Mode3 = 0;
            m_Probe_Job_Mode0 = 0;
            m_Probe_Job_Mode1 = 0;
            m_Probe_Job_Mode2 = 0;
            m_Probe_Job_Mode3 = 0;
            threads[0] = new Thread(ThreadProc0); threads[0].IsBackground = true;
            threads[1] = new Thread(ThreadProc1); threads[1].IsBackground = true;
            threads[2] = new Thread(ThreadProc2); threads[2].IsBackground = true;
            threads[3] = new Thread(ThreadProc3); threads[3].IsBackground = true;
            //threads[0].Priority = ThreadPriority.Highest;
            //threads[1].Priority = ThreadPriority.Highest;
            //threads[2].Priority = ThreadPriority.Highest;
            //threads[3].Priority = ThreadPriority.Highest;
            Probe_threads[0] = new Thread(ProbeThreadProc0); Probe_threads[0].IsBackground = true;
            Probe_threads[1] = new Thread(ProbeThreadProc1); Probe_threads[1].IsBackground = true;
            Probe_threads[2] = new Thread(ProbeThreadProc2); Probe_threads[2].IsBackground = true;
            Probe_threads[3] = new Thread(ProbeThreadProc3); Probe_threads[3].IsBackground = true;

            //timer_Cam[0].Tick += new System.EventHandler(timer_Cam0_Simulation_Tick);
            //timer_Cam[1].Tick += new System.EventHandler(timer_Cam1_Simulation_Tick);
            //timer_Cam[2].Tick += new System.EventHandler(timer_Cam2_Simulation_Tick);
            //timer_Cam[3].Tick += new System.EventHandler(timer_Cam3_Simulation_Tick);

            if (!ctr_Camera_Setting1.Force_USE.Checked)
            {
                //Viewthreads[0].Start();
                if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 3)
                {
                    Probe_threads[0].Start();
                }
                else
                {
                    threads[0].Start();
                    if (LVApp.Instance().m_Config.Image_Merge_Check[0])
                    {
                        DebugLogger.Instance().LogRecord("Try 0");
                        //ImageMergeThread[0].Start();
                    }
                }
            }
            if (!ctr_Camera_Setting2.Force_USE.Checked)
            {
                //Viewthreads[1].Start();
                if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 3)
                {
                    Probe_threads[1].Start();
                }
                else
                {
                    threads[1].Start();
                    if (LVApp.Instance().m_Config.Image_Merge_Check[1])
                    {
                        DebugLogger.Instance().LogRecord("Try 1");
                        //ImageMergeThread[1].Start();
                    }
                }
            }
            if (!ctr_Camera_Setting3.Force_USE.Checked)
            {
                //Viewthreads[2].Start();
                if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 3)
                {
                    Probe_threads[2].Start();
                }
                else
                {
                    threads[2].Start();
                    if (LVApp.Instance().m_Config.Image_Merge_Check[2])
                    {
                        DebugLogger.Instance().LogRecord("Try 2");
                        //ImageMergeThread[2].Start();
                    }
                }
            }
            if (!ctr_Camera_Setting4.Force_USE.Checked)
            {
                //Viewthreads[3].Start();
                if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 3)
                {
                    DebugLogger.Instance().LogRecord("Try 3");
                    Probe_threads[3].Start();
                }
                else
                {
                    threads[3].Start();
                    if (LVApp.Instance().m_Config.Image_Merge_Check[3])
                    {
                        //ImageMergeThread[3].Start();
                    }
                }
            }
        }

        public void Inspection_Thread_Stop()
        {
            DebugLogger.Instance().LogRecord("Inspection Thread Stop");
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (m_Threads_Check[i])
                    {
                        m_Threads_Check[i] = false;
                        Thread.Sleep(100);
                        if (threads[i].IsAlive)
                        {
                            threads[i].Abort();
                        }
                        //Viewthreads[i].Abort();
                        if (Probe_threads[i].IsAlive)
                        {
                            Probe_threads[i].Abort();
                        }
                    }
                    //threads[i].Abort();
                    //threads[i] = null;
                }
                //timer_Cam[0].Enabled = false;
                //timer_Cam[1].Enabled = false;
                //timer_Cam[2].Enabled = false;
                //timer_Cam[3].Enabled = false;
                //timer_Cam[0].Tick -= new System.EventHandler(timer_Cam0_Simulation_Tick);
                //timer_Cam[1].Tick -= new System.EventHandler(timer_Cam1_Simulation_Tick);
                //timer_Cam[2].Tick -= new System.EventHandler(timer_Cam2_Simulation_Tick);
                //timer_Cam[3].Tick -= new System.EventHandler(timer_Cam3_Simulation_Tick);
            }
            catch
            {
            }
        }

        public Bitmap cropAtRect(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private int[] _mergeProcessCount = { 0, 0, 0, 0 };

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public void ThreadProc0()
        {
            int Cam_Num = 0;
            try
            {
                //bool t_onecycle_check = false;
                while (m_Threads_Check[Cam_Num])
                {
                    if (m_Job_Mode0 == 0)
                    {
                        Thread.Sleep(1);
                    }
                    else if (m_Job_Mode0 == 1 && Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Bitmap Capture_frame = null;
                        //DebugLogger.Instance().LogRecord($"mergeProcessCount : {_mergeProcessCount[Cam_Num]}");
                        if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                        {
                            Bitmap Capture_frame_For_Merge = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                            Capture_framebuffer[Cam_Num].RemoveAt(0);

                            if (_is_NewFrame[Cam_Num])
                            {
                                _is_NewFrame[Cam_Num] = false;
                                LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num] = new Bitmap(Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height * LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num]);
                                if (_mergeProcessCount[Cam_Num] != 0)
                                {
                                    // Interval 이 긴데도 Image_Merge Index가 남아 있으면, 이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                    //DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous: {_mergeProcessCount[Cam_Num]}");
                                    _mergeProcessCount[Cam_Num] = 0;
                                }
                            }
                            else if (_mergeProcessCount[Cam_Num] == 0)
                            {
                                // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 리턴
                                return;
                            }

                            if (_mergeProcessCount[Cam_Num] >= 0 && _mergeProcessCount[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                Rectangle bounds = new Rectangle(0, Capture_frame_For_Merge.Height * _mergeProcessCount[Cam_Num], Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height);
                                using (Graphics g = Graphics.FromImage(LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num]))
                                {
                                    g.DrawImage(Capture_frame_For_Merge, bounds, 0, 0, Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height, GraphicsUnit.Pixel);
                                }
                                ++_mergeProcessCount[Cam_Num];
                            }

                            if (_mergeProcessCount[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                                LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                                Run_SW[Cam_Num].Reset();
                                Run_SW[Cam_Num].Start();

                                Capture_framebuffer[Cam_Num].Clear();
                                _mergeProcessCount[Cam_Num] = 0;

                                Capture_frame = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num].Clone());

                                #region LHJ - 240804 - 디버깅용, 240806(Merge 여부에 따라 Count 를 다르게 계산)
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam0[_mergeImageCount[Cam_Num] % LV_Config._lastImageCount] = (Bitmap)Capture_frame.Clone();
                                #endregion
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                            LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                            //if (t_onecycle_check)
                            //{
                            //    m_Job_Mode0 = 0;
                            //    t_onecycle_check = false;
                            //    continue;
                            //}
                            //t_onecycle_check = true;
                            Run_SW[Cam_Num].Reset();
                            Run_SW[Cam_Num].Start();

                            //lock (Capture_framebuffer[Cam_Num])
                            {
                                Capture_frame = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                        }

                        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        }
                        if (!Simulation_mode || !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            // 영상처리 파트
                            if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipX);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipY);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipXY);
                                }
                            }
                            else
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    //Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                            }
                        }

                        if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (m_Result_Job_Mode0 == 0)
                            {
                                //lock (Result_framebuffer[Cam_Num])
                                {
                                    LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                    Result_framebuffer[Cam_Num].Add((Bitmap)Capture_frame.Clone());
                                    //m_Result_Job_Mode0 = 1;
                                }
                                //Capture_frame.Dispose();
                            }
                        }
                        else // 검사하면 아래로
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                            }

                            //LVApp.Instance().m_Config.Set_Parameters();
                            if (Capture_frame.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2 || LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 6)
                                {
                                    byte[] arr = BmpToArray0((Bitmap)Capture_frame.Clone());
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, Capture_frame.Width, Capture_frame.Height, 3, Cam_Num);
                                }
                                else
                                {
                                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)Capture_frame.Clone());
                                    byte[] arr = BmpToArray0(grayImage);
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                    grayImage.Dispose();
                                }
                            }
                            else
                            {
                                byte[] arr = BmpToArray0((Bitmap)Capture_frame.Clone());
                                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, Capture_frame.Width, Capture_frame.Height, 1, Cam_Num);
                            }

                            ctr_Manual1.Run_Inspection(Cam_Num, ref Capture_frame);

                            LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                            //int Judge = 40;
                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            // Judge :10, 20, 30:NG, 40:OK. -1: NOOBJ

                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            byte[] Dst_Img = null;
                            int width = 0, height = 0, ch = 0;

                            if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                            {
                                Bitmap t_bmp = ConvertBitmap0(Dst_Img, width, height, ch);

                                if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40 && m_Result_Job_Mode0 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_Result_Job_Mode0 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                        }
                                    }
                                }

                                //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap0(Dst_Img, width, height, ch);
                                if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                {
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)t_bmp.Clone();
                                    LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                }

                                if (LVApp.Instance().m_Config.SSF_Image_Save && t_Judge == false)
                                {
                                    LVApp.Instance().m_Config.SSF_Result_Image_Save(Cam_Num, (Bitmap)t_bmp.Clone(), 1);
                                }

                                t_bmp.Dispose();
                                //if (m_Result_Job_Mode0 == 0)
                                //{
                                //    if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //    {
                                //        if (Judge != 40)
                                //        {
                                //            m_Result_Job_Mode0 = 1;
                                //        }
                                //        if (t_cam_setting_view_mode)
                                //        {
                                //            m_Result_Job_Mode0 = 1;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        m_Result_Job_Mode0 = 1;
                                //    }
                                //}
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }

                            if (Run_SW[Cam_Num].ElapsedMilliseconds < ctr_PLC1.m_MinProcessingTime && ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                            {
                                Thread.Sleep(ctr_PLC1.m_MinProcessingTime - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                            }
                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            String filename = string.Empty;
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                if (t_Judge)
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK";
                                }
                                else
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NG";
                                }

                                //filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, Capture_frame, false);
                            }
                            else
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                if (t_Judge)
                                {
                                    filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                                }
                                else
                                {
                                    if (Judge == -1)
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 2);
                                    }
                                    else
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 1);
                                    }
                                }
                            }

                            int t_wait_cnt = 0;
                            while (!ctr_PLC1.MC_Rx_Value_Updated[Cam_Num])
                            {
                                Thread.Sleep(1);
                                t_wait_cnt++;
                                if (t_wait_cnt >= ctr_PLC1.m_MinProcessingTime)
                                {
                                    break;
                                }
                            }
                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                        }

                        Run_SW[Cam_Num].Stop();
                        LVApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + LVApp.Instance().t_Util.m_FPS[Cam_Num].ToString("0.0") + "/" + LVApp.Instance().t_Util.m_FPS[Cam_Num + 4].ToString("0.0");
                        LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                        //lock (Capture_framebuffer[Cam_Num])
                        //if (Capture_framebuffer[Cam_Num].Count > 0)
                        //{
                        //    Capture_framebuffer[Cam_Num].Clear();
                        //}
                        Capture_frame.Dispose();
                        LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                        //t_onecycle_check = false;
                        m_Job_Mode0 = 0;
                        DebugLogger.Instance().LogRecord($"CAM0 PROC End - {LVApp.Instance().m_Config.m_TT[Cam_Num]}");
                    }

                    if (!m_Threads_Check[Cam_Num])
                    {
                        break;
                    }
                }
            }
            catch
            {
                m_Threads_Check[Cam_Num] = false;
                //if (threads[Cam_Num] != null)
                //{
                //    threads[Cam_Num].Abort();
                //    threads[Cam_Num] = null;
                //}
                //m_Job_Mode0 = 0;
                //threads[Cam_Num] = new Thread(ThreadProc0);
                //threads[Cam_Num].IsBackground = true;
                //threads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " Inspection Thread Restart");
                //if (Capture_framebuffer[Cam_Num].Count > 0)
                //{
                //    Capture_framebuffer[Cam_Num].Clear();
                //}
            }
        }


        public void ResultProc0()
        {
            int Cam_Num = 0;
            try
            {
                while (m_ViewThreads_Check[Cam_Num])
                {
                    if (!m_ViewThreads_Check[Cam_Num])
                    {
                        break;
                    }
                    if (m_Result_Job_Mode0 == 0)
                    {
                        if (Result_framebuffer[Cam_Num].Count > 0)
                        {
                            m_Result_Job_Mode0 = 1;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else if (m_Result_Job_Mode0 == 1)
                    {
                        if (LVApp.Instance().m_Config.Realtime_View_Check)
                        {
                            //if (t_Main_View != 0 && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            //{
                            //    //lock (Result_framebuffer[Cam_Num])
                            //    {
                            //        if (Result_framebuffer[Cam_Num].Count > 0)
                            //        {
                            //            Result_framebuffer[Cam_Num].Clear();
                            //        }
                            //    }
                            //    Thread.Sleep(5);
                            //    m_Result_Job_Mode0 = 0;
                            //    continue;
                            //}

                            Bitmap bitmapNew = null;
                            //lock (Result_framebuffer[Cam_Num])
                            {
                                bitmapNew = Result_framebuffer[Cam_Num][0];
                            }

                            if (t_cam_ROI_view_mode)
                            {
                                //if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                                //{
                                //    if (ctr_ROI1.pictureBox_Image.InvokeRequired)
                                //    {
                                //        ctr_ROI1.pictureBox_Image.Invoke((MethodInvoker)delegate
                                //        {
                                //            ctr_ROI1.pictureBox_Image.Image = bitmapNew;
                                //        });
                                //    }
                                //    else
                                //    {
                                //        ctr_ROI1.pictureBox_Image.Image = bitmapNew;
                                //    }
                                //}
                            }
                            else
                            {
                                if (t_setting_view_mode && !t_cam_setting_view_mode)
                                {// 판정 설정 뷰
                                    if (pictureBox_Setting_0.InvokeRequired)
                                    {
                                        pictureBox_Setting_0.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_Setting_0.Image as Bitmap;
                                            pictureBox_Setting_0.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                            //pictureBox_Setting_0_Height = pictureBox_Setting_0.Height;
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_Setting_0.Image as Bitmap;
                                        pictureBox_Setting_0.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                        //pictureBox_Setting_0_Height = pictureBox_Setting_0.Height;
                                    }
                                }
                                else if (!t_setting_view_mode && t_cam_setting_view_mode)
                                {// 카메라 설정 뷰
                                    if (pictureBox_CAM0.InvokeRequired)
                                    {
                                        pictureBox_CAM0.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_CAM0.Image as Bitmap;
                                            pictureBox_CAM0.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_CAM0.Image as Bitmap;
                                        pictureBox_CAM0.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                                {// 메인 뷰
                                    if (ctr_Display_1.pictureBox_0.InvokeRequired)
                                    {
                                        ctr_Display_1.pictureBox_0.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = ctr_Display_1.pictureBox_0.Image as Bitmap;
                                            ctr_Display_1.pictureBox_0.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = ctr_Display_1.pictureBox_0.Image as Bitmap;
                                        ctr_Display_1.pictureBox_0.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                //bitmapNew.Dispose();
                            }
                        }
                        else
                        {
                            if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            {// 메인 뷰
                                if (ctr_Display_1.pictureBox_0.InvokeRequired)
                                {
                                    ctr_Display_1.pictureBox_0.Invoke((MethodInvoker)delegate
                                    {
                                        if (ctr_Display_1.pictureBox_0.Image != null)
                                        {
                                            ctr_Display_1.pictureBox_0.Image = null;
                                        }
                                        ctr_Display_1.pictureBox_0.Refresh();
                                    });
                                }
                                else
                                {
                                    if (ctr_Display_1.pictureBox_0.Image != null)
                                    {
                                        ctr_Display_1.pictureBox_0.Image = null;
                                    }
                                    ctr_Display_1.pictureBox_0.Refresh();
                                }
                            }
                        }
                        //lock (Result_framebuffer[Cam_Num])
                        {
                            if (Result_framebuffer[Cam_Num].Count > 0)
                            {
                                Result_framebuffer[Cam_Num].Clear();
                            }
                        }
                        //if (LVApp.Instance().t_Util.m_FPS[Cam_Num + 4] > 4.9)
                        //{
                        //    Thread.Sleep(1000 / 4);
                        //}
                        //else
                        //{
                        //    Thread.Sleep(10);
                        //}
                        m_Result_Job_Mode0 = 0;
                    }
                }
            }
            catch
            {
                m_ViewThreads_Check[Cam_Num] = false;
            }
        }


        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public void ThreadProc1()
        {
            int Cam_Num = 1;
            //bool t_onecycle_check = false;
            try
            {
                while (m_Threads_Check[Cam_Num])
                {
                    if (m_Job_Mode1 == 0)
                    {
                        Thread.Sleep(1);
                    }
                    else if (m_Job_Mode1 == 1 && Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Bitmap Capture_frame = null;
                        //DebugLogger.Instance().LogRecord($"mergeProcessCount : {_mergeProcessCount[Cam_Num]}");
                        if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                        {
                            Bitmap Capture_frame_For_Merge = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                            Capture_framebuffer[Cam_Num].RemoveAt(0);

                            if (_is_NewFrame[Cam_Num])
                            {
                                _is_NewFrame[Cam_Num] = false;
                                LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num] = new Bitmap(Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height * LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num]);
                                if (_mergeProcessCount[Cam_Num] != 0)
                                {
                                    // Interval 이 긴데도 Image_Merge Index가 남아 있으면, 이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                    //DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous: {_mergeProcessCount[Cam_Num]}");
                                    _mergeProcessCount[Cam_Num] = 0;
                                }
                            }
                            else if (_mergeProcessCount[Cam_Num] == 0)
                            {
                                // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 리턴
                                return;
                            }

                            if (_mergeProcessCount[Cam_Num] >= 0 && _mergeProcessCount[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                Rectangle bounds = new Rectangle(0, Capture_frame_For_Merge.Height * _mergeProcessCount[Cam_Num], Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height);
                                using (Graphics g = Graphics.FromImage(LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num]))
                                {
                                    g.DrawImage(Capture_frame_For_Merge, bounds, 0, 0, Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height, GraphicsUnit.Pixel);
                                }
                                ++_mergeProcessCount[Cam_Num];
                            }

                            if (_mergeProcessCount[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                                LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                                Run_SW[Cam_Num].Reset();
                                Run_SW[Cam_Num].Start();

                                Capture_framebuffer[Cam_Num].Clear();
                                _mergeProcessCount[Cam_Num] = 0;

                                Capture_frame = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num].Clone());

                                #region LHJ - 240804 - 디버깅용, 240806(Merge 여부에 따라 Count 를 다르게 계산)
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam1[_mergeImageCount[Cam_Num] % LV_Config._lastImageCount] = (Bitmap)Capture_frame.Clone();
                                #endregion
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                            LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                            //if (t_onecycle_check)
                            //{
                            //    m_Job_Mode0 = 0;
                            //    t_onecycle_check = false;
                            //    continue;
                            //}
                            //t_onecycle_check = true;
                            Run_SW[Cam_Num].Reset();
                            Run_SW[Cam_Num].Start();

                            //lock (Capture_framebuffer[Cam_Num])
                            {
                                Capture_frame = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                        }

                        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        }
                        if (!Simulation_mode || !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipX);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipY);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipXY);
                                }
                            }
                            else
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    //Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                            }
                        }

                        if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (m_Result_Job_Mode1 == 0)
                            {
                                LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                //lock (Result_framebuffer[Cam_Num])
                                {
                                    Result_framebuffer[Cam_Num].Add((Bitmap)Capture_frame.Clone());
                                }
                                //m_Result_Job_Mode1 = 1;
                            }
                            //Capture_frame.Dispose();
                        }
                        else // 검사하면 아래로
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                // Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                            }

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            if (Capture_frame.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2 || LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 6)
                                {
                                    byte[] arr = BmpToArray1((Bitmap)Capture_frame.Clone());
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, Capture_frame.Width, Capture_frame.Height, 3, Cam_Num);
                                }
                                else
                                {
                                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)Capture_frame.Clone());
                                    byte[] arr = BmpToArray1(grayImage);
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                    grayImage.Dispose();
                                }
                            }
                            else
                            {
                                byte[] arr = BmpToArray1((Bitmap)Capture_frame.Clone());
                                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, Capture_frame.Width, Capture_frame.Height, 1, Cam_Num);
                            }

                            ctr_Manual1.Run_Inspection(Cam_Num, ref Capture_frame);
                            LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            byte[] Dst_Img = null;
                            int width = 0, height = 0, ch = 0;

                            if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image1(out Dst_Img, out width, out height, out ch, Cam_Num))
                            {
                                Bitmap t_bmp = ConvertBitmap1(Dst_Img, width, height, ch);
                                if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40 && m_Result_Job_Mode1 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_Result_Job_Mode1 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                        }
                                    }
                                }

                                if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                {
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)t_bmp.Clone();
                                    LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                }

                                if (LVApp.Instance().m_Config.SSF_Image_Save && t_Judge == false)
                                {
                                    LVApp.Instance().m_Config.SSF_Result_Image_Save(Cam_Num, (Bitmap)t_bmp.Clone(), 1);
                                }

                                t_bmp.Dispose();
                                //if (m_Result_Job_Mode1 == 0)
                                //{
                                //    if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //    {
                                //        if (Judge != 40)
                                //        {
                                //            m_Result_Job_Mode1 = 1;
                                //        }
                                //        if (t_cam_setting_view_mode)
                                //        {
                                //            m_Result_Job_Mode1 = 1;
                                //        }

                                //    }
                                //    else
                                //    {
                                //        m_Result_Job_Mode1 = 1;
                                //    }
                                //}
                            }
                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }
                            if (Run_SW[Cam_Num].ElapsedMilliseconds < ctr_PLC1.m_MinProcessingTime && ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                            {
                                Thread.Sleep(ctr_PLC1.m_MinProcessingTime - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                            }
                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            String filename = string.Empty;
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                if (t_Judge)
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK";
                                }
                                else
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NG";
                                }
                                //filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, Capture_frame, false);
                            }
                            else
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                if (t_Judge)
                                {
                                    filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                                }
                                else
                                {
                                    if (Judge == -1)
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 2);
                                    }
                                    else
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 1);
                                    }
                                }
                            }

                            int t_wait_cnt = 0;
                            while (!ctr_PLC1.MC_Rx_Value_Updated[Cam_Num])
                            {
                                Thread.Sleep(1);
                                t_wait_cnt++;
                                if (t_wait_cnt >= ctr_PLC1.m_MinProcessingTime)
                                {
                                    break;
                                }
                            }

                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                        }


                        //if (Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                        //{
                        //    Thread.Sleep(50 - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                        //}
                        Run_SW[Cam_Num].Stop();
                        LVApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + LVApp.Instance().t_Util.m_FPS[Cam_Num].ToString("0.0") + "/" + LVApp.Instance().t_Util.m_FPS[Cam_Num + 4].ToString("0.0");
                        LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                        //lock (Capture_framebuffer[Cam_Num])
                        //if (Capture_framebuffer[Cam_Num].Count > 0)
                        //{
                        //    Capture_framebuffer[Cam_Num].Clear();
                        //}
                        Capture_frame.Dispose();

                        LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                        //t_onecycle_check = false;
                        m_Job_Mode1 = 0;
                        DebugLogger.Instance().LogRecord($"CAM1 PROC End - {LVApp.Instance().m_Config.m_TT[Cam_Num]}");
                    }

                    if (!m_Threads_Check[Cam_Num])
                    {
                        break;
                    }
                }
            }
            catch
            {
                m_Threads_Check[Cam_Num] = false;
                //if (threads[Cam_Num] != null)
                //{
                //    threads[Cam_Num].Abort();
                //    threads[Cam_Num] = null;
                //}
                //m_Job_Mode1 = 0;
                //threads[Cam_Num] = new Thread(ThreadProc1);
                //threads[Cam_Num].IsBackground = true;
                //threads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " Inspection Thread Restart");
                //if (Capture_framebuffer[Cam_Num].Count > 0)
                //{
                //    Capture_framebuffer[Cam_Num].Clear();
                //}
            }
        }


        public void ResultProc1()
        {
            int Cam_Num = 1;
            try
            {
                while (m_ViewThreads_Check[Cam_Num])
                {
                    if (!m_ViewThreads_Check[Cam_Num])
                    {
                        break;
                    }
                    if (m_Result_Job_Mode1 == 0)
                    {
                        if (Result_framebuffer[Cam_Num].Count > 0)
                        {
                            m_Result_Job_Mode1 = 1;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else if (m_Result_Job_Mode1 == 1)
                    {
                        if (LVApp.Instance().m_Config.Realtime_View_Check)
                        {
                            //if (t_Main_View != 0 && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            //{
                            //    //lock (Result_framebuffer[Cam_Num])
                            //    {
                            //        if (Result_framebuffer[Cam_Num].Count > 0)
                            //        {
                            //            Result_framebuffer[Cam_Num].Clear();
                            //        }
                            //    }
                            //    Thread.Sleep(5);
                            //    m_Result_Job_Mode1 = 0;
                            //    continue;
                            //}

                            Bitmap bitmapNew = null;
                            //lock (Result_framebuffer[Cam_Num])
                            {
                                bitmapNew = Result_framebuffer[Cam_Num][0];
                            }
                            if (t_cam_ROI_view_mode)
                            {
                                //if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                                //{
                                //    if (ctr_ROI2.pictureBox_Image.InvokeRequired)
                                //    {
                                //        ctr_ROI2.pictureBox_Image.Invoke((MethodInvoker)delegate
                                //        {
                                //            ctr_ROI2.pictureBox_Image.Image = bitmapNew;
                                //        });
                                //    }
                                //    else
                                //    {
                                //        ctr_ROI2.pictureBox_Image.Image = bitmapNew;
                                //    }
                                //}
                            }
                            else
                            {
                                if (t_setting_view_mode && !t_cam_setting_view_mode)
                                {// 판정 설정 뷰
                                    if (pictureBox_Setting_1.InvokeRequired)
                                    {
                                        pictureBox_Setting_1.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_Setting_1.Image as Bitmap;
                                            pictureBox_Setting_1.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_Setting_1.Image as Bitmap;
                                        pictureBox_Setting_1.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && t_cam_setting_view_mode)
                                {// 카메라 설정 뷰
                                    if (pictureBox_CAM1.InvokeRequired)
                                    {
                                        pictureBox_CAM1.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_CAM1.Image as Bitmap;
                                            pictureBox_CAM1.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_CAM1.Image as Bitmap;
                                        pictureBox_CAM1.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                                {// 메인 뷰
                                    if (ctr_Display_1.pictureBox_1.InvokeRequired)
                                    {
                                        ctr_Display_1.pictureBox_1.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = ctr_Display_1.pictureBox_1.Image as Bitmap;
                                            ctr_Display_1.pictureBox_1.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = ctr_Display_1.pictureBox_1.Image as Bitmap;
                                        ctr_Display_1.pictureBox_1.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                //bitmapNew.Dispose();
                            }
                        }
                        else
                        {
                            if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            {// 메인 뷰
                                if (ctr_Display_1.pictureBox_1.InvokeRequired)
                                {
                                    ctr_Display_1.pictureBox_1.Invoke((MethodInvoker)delegate
                                    {
                                        if (ctr_Display_1.pictureBox_1.Image != null)
                                        {
                                            ctr_Display_1.pictureBox_1.Image = null;
                                        }
                                        ctr_Display_1.pictureBox_1.Refresh();
                                    });
                                }
                                else
                                {
                                    if (ctr_Display_1.pictureBox_1.Image != null)
                                    {
                                        ctr_Display_1.pictureBox_1.Image = null;
                                    }
                                    ctr_Display_1.pictureBox_1.Refresh();
                                }
                            }
                        }
                        //lock (Result_framebuffer[Cam_Num])
                        {
                            if (Result_framebuffer[Cam_Num].Count > 0)
                            {
                                Result_framebuffer[Cam_Num].Clear();
                            }
                        }
                        //if (LVApp.Instance().t_Util.m_FPS[Cam_Num + 4] > 4.9)
                        //{
                        //    Thread.Sleep(1000 / 4);
                        //}
                        //else
                        //{
                        //    Thread.Sleep(10);
                        //}
                        m_Result_Job_Mode1 = 0;
                    }
                }
            }
            catch
            {
                m_ViewThreads_Check[Cam_Num] = false;
                //if (Viewthreads[Cam_Num] != null)
                //{
                //    Viewthreads[Cam_Num].Abort();
                //    Viewthreads[Cam_Num] = null;
                //}
                //m_Result_Job_Mode1 = 0;
                //Viewthreads[Cam_Num] = new Thread(ResultProc1);
                //Viewthreads[Cam_Num].IsBackground = true;
                //Viewthreads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " View Thread Restart");
                //if (Result_framebuffer[Cam_Num].Count > 0)
                //{
                //    Result_framebuffer[Cam_Num].Clear();
                //}
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public void ThreadProc2()
        {
            int Cam_Num = 2;
            //bool t_onecycle_check = false;
            try
            {
                while (m_Threads_Check[Cam_Num])
                {
                    if (m_Job_Mode2 == 0)
                    {
                        Thread.Sleep(1);
                    }
                    else if (m_Job_Mode2 == 1 && Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Bitmap Capture_frame = null;
                        //DebugLogger.Instance().LogRecord($"mergeProcessCount : {_mergeProcessCount[Cam_Num]}");
                        if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                        {
                            Bitmap Capture_frame_For_Merge = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                            Capture_framebuffer[Cam_Num].RemoveAt(0);

                            if (_is_NewFrame[Cam_Num])
                            {
                                _is_NewFrame[Cam_Num] = false;
                                LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num] = new Bitmap(Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height * LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num]);
                                if (_mergeProcessCount[Cam_Num] != 0)
                                {
                                    // Interval 이 긴데도 Image_Merge Index가 남아 있으면, 이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                    //DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous: {_mergeProcessCount[Cam_Num]}");
                                    _mergeProcessCount[Cam_Num] = 0;
                                }
                            }
                            else if (_mergeProcessCount[Cam_Num] == 0)
                            {
                                // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 리턴
                                return;
                            }

                            if (_mergeProcessCount[Cam_Num] >= 0 && _mergeProcessCount[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                Rectangle bounds = new Rectangle(0, Capture_frame_For_Merge.Height * _mergeProcessCount[Cam_Num], Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height);
                                using (Graphics g = Graphics.FromImage(LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num]))
                                {
                                    g.DrawImage(Capture_frame_For_Merge, bounds, 0, 0, Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height, GraphicsUnit.Pixel);
                                }
                                ++_mergeProcessCount[Cam_Num];
                            }

                            if (_mergeProcessCount[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                                LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                                Run_SW[Cam_Num].Reset();
                                Run_SW[Cam_Num].Start();

                                Capture_framebuffer[Cam_Num].Clear();
                                _mergeProcessCount[Cam_Num] = 0;

                                Capture_frame = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num].Clone());

                                #region LHJ - 240804 - 디버깅용, 240806(Merge 여부에 따라 Count 를 다르게 계산)
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam2[_mergeImageCount[Cam_Num] % LV_Config._lastImageCount] = (Bitmap)Capture_frame.Clone();
                                #endregion
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                            LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                            //if (t_onecycle_check)
                            //{
                            //    m_Job_Mode0 = 0;
                            //    t_onecycle_check = false;
                            //    continue;
                            //}
                            //t_onecycle_check = true;
                            Run_SW[Cam_Num].Reset();
                            Run_SW[Cam_Num].Start();

                            //lock (Capture_framebuffer[Cam_Num])
                            {
                                Capture_frame = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                        }

                        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        }
                        if (!Simulation_mode || !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipX);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipY);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipXY);
                                }
                            }
                            else
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    //Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                            }
                        }

                        if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (m_Result_Job_Mode2 == 0)
                            {
                                LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                //lock (Result_framebuffer[Cam_Num])
                                {
                                    Result_framebuffer[Cam_Num].Add((Bitmap)Capture_frame.Clone());
                                }
                                //m_Result_Job_Mode2 = 1;
                            }
                            //Capture_frame.Dispose();
                        }
                        else // 검사하면 아래로
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                            }

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            if (Capture_frame.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2 || LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 6)
                                {
                                    byte[] arr = BmpToArray2((Bitmap)Capture_frame.Clone());
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, Capture_frame.Width, Capture_frame.Height, 3, Cam_Num);
                                }
                                else
                                {
                                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)Capture_frame.Clone());
                                    byte[] arr = BmpToArray2(grayImage);
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                    grayImage.Dispose();
                                }
                            }
                            else
                            {
                                byte[] arr = BmpToArray2((Bitmap)Capture_frame.Clone());
                                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, Capture_frame.Width, Capture_frame.Height, 1, Cam_Num);
                            }

                            ctr_Manual1.Run_Inspection(Cam_Num, ref Capture_frame);
                            LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                            byte[] Dst_Img = null;
                            int width = 0, height = 0, ch = 0;

                            if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image2(out Dst_Img, out width, out height, out ch, Cam_Num))
                            {
                                Bitmap t_bmp = ConvertBitmap2(Dst_Img, width, height, ch);
                                if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40 && m_Result_Job_Mode2 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_Result_Job_Mode2 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                        }
                                    }
                                }
                                if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                {
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)t_bmp.Clone();
                                    LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                }

                                if (LVApp.Instance().m_Config.SSF_Image_Save && t_Judge == false)
                                {
                                    LVApp.Instance().m_Config.SSF_Result_Image_Save(Cam_Num, (Bitmap)t_bmp.Clone(), 1);
                                }

                                t_bmp.Dispose();
                                //if (m_Result_Job_Mode2 == 0)
                                //{
                                //    if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //    {
                                //        if (Judge != 40)
                                //        {
                                //            m_Result_Job_Mode2 = 1;
                                //        }
                                //        if (t_cam_setting_view_mode)
                                //        {
                                //            m_Result_Job_Mode2 = 1;
                                //        }

                                //    }
                                //    else
                                //    {
                                //        m_Result_Job_Mode2 = 1;
                                //    }
                                //}
                            }

                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }
                            if (Run_SW[Cam_Num].ElapsedMilliseconds < ctr_PLC1.m_MinProcessingTime && ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                            {
                                Thread.Sleep(ctr_PLC1.m_MinProcessingTime - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                            }
                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            String filename = string.Empty;
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                if (t_Judge)
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK";
                                }
                                else
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NG";
                                }
                                //filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, Capture_frame, false);
                            }
                            else
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                if (t_Judge)
                                {
                                    filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                                }
                                else
                                {
                                    if (Judge == -1)
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 2);
                                    }
                                    else
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 1);
                                    }
                                }
                            }

                            int t_wait_cnt = 0;
                            while (!ctr_PLC1.MC_Rx_Value_Updated[Cam_Num])
                            {
                                Thread.Sleep(1);
                                t_wait_cnt++;
                                if (t_wait_cnt >= ctr_PLC1.m_MinProcessingTime)
                                {
                                    break;
                                }
                            }

                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                        }


                        //if (Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                        //{
                        //    Thread.Sleep(50 - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                        //}
                        Run_SW[Cam_Num].Stop();
                        LVApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + LVApp.Instance().t_Util.m_FPS[Cam_Num].ToString("0.0") + "/" + LVApp.Instance().t_Util.m_FPS[Cam_Num + 4].ToString("0.0");
                        LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                        //lock (Capture_framebuffer[Cam_Num])
                        //if (Capture_framebuffer[Cam_Num].Count > 0)
                        //{
                        //    Capture_framebuffer[Cam_Num].Clear();
                        //}
                        Capture_frame.Dispose();
                        LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                        //t_onecycle_check = false;
                        m_Job_Mode2 = 0;
                        DebugLogger.Instance().LogRecord($"CAM2 PROC End - {LVApp.Instance().m_Config.m_TT[Cam_Num]}");
                    }

                    if (!m_Threads_Check[Cam_Num])
                    {
                        break;
                    }
                }
            }
            catch
            {
                m_Threads_Check[Cam_Num] = false;
                //if (threads[Cam_Num] != null)
                //{
                //    threads[Cam_Num].Abort();
                //    threads[Cam_Num] = null;
                //}
                //m_Job_Mode2 = 0;
                //threads[Cam_Num] = new Thread(ThreadProc2);
                //threads[Cam_Num].IsBackground = true;
                //threads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " Inspection Thread Restart");
                //if (Capture_framebuffer[Cam_Num].Count > 0)
                //{
                //    Capture_framebuffer[Cam_Num].Clear();
                //}
            }

        }


        public void ResultProc2()
        {
            int Cam_Num = 2;
            try
            {
                while (m_ViewThreads_Check[Cam_Num])
                {
                    if (!m_ViewThreads_Check[Cam_Num])
                    {
                        break;
                    }
                    if (m_Result_Job_Mode2 == 0)
                    {
                        if (Result_framebuffer[Cam_Num].Count > 0)
                        {
                            m_Result_Job_Mode2 = 1;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else if (m_Result_Job_Mode2 == 1)
                    {
                        if (LVApp.Instance().m_Config.Realtime_View_Check)
                        {
                            //if (t_Main_View != 0 && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            //{
                            //    //lock (Result_framebuffer[Cam_Num])
                            //    {
                            //        if (Result_framebuffer[Cam_Num].Count > 0)
                            //        {
                            //            Result_framebuffer[Cam_Num].Clear();
                            //        }
                            //    }
                            //    Thread.Sleep(5);
                            //    m_Result_Job_Mode2 = 0;
                            //    continue;
                            //}
                            Bitmap bitmapNew = null;
                            //lock (Result_framebuffer[Cam_Num])
                            {
                                bitmapNew = Result_framebuffer[Cam_Num][0];
                            }
                            if (t_cam_ROI_view_mode)
                            {
                                //if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                                //{
                                //    if (ctr_ROI3.pictureBox_Image.InvokeRequired)
                                //    {
                                //        ctr_ROI3.pictureBox_Image.Invoke((MethodInvoker)delegate
                                //        {
                                //            ctr_ROI3.pictureBox_Image.Image = bitmapNew;
                                //        });
                                //    }
                                //    else
                                //    {
                                //        ctr_ROI3.pictureBox_Image.Image = bitmapNew;
                                //    }
                                //}
                            }
                            else
                            {
                                if (t_setting_view_mode && !t_cam_setting_view_mode)
                                {// 판정 설정 뷰
                                    if (pictureBox_Setting_2.InvokeRequired)
                                    {
                                        pictureBox_Setting_2.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_Setting_2.Image as Bitmap;
                                            pictureBox_Setting_2.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_Setting_2.Image as Bitmap;
                                        pictureBox_Setting_2.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && t_cam_setting_view_mode)
                                {// 카메라 설정 뷰
                                    if (pictureBox_CAM2.InvokeRequired)
                                    {
                                        pictureBox_CAM2.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_CAM2.Image as Bitmap;
                                            pictureBox_CAM2.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_CAM2.Image as Bitmap;
                                        pictureBox_CAM2.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                                {// 메인 뷰
                                    if (ctr_Display_1.pictureBox_2.InvokeRequired)
                                    {
                                        ctr_Display_1.pictureBox_2.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = ctr_Display_1.pictureBox_2.Image as Bitmap;
                                            ctr_Display_1.pictureBox_2.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = ctr_Display_1.pictureBox_2.Image as Bitmap;
                                        ctr_Display_1.pictureBox_2.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                //bitmapNew.Dispose();
                            }
                        }
                        else
                        {
                            if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            {// 메인 뷰
                                if (ctr_Display_1.pictureBox_2.InvokeRequired)
                                {
                                    ctr_Display_1.pictureBox_2.Invoke((MethodInvoker)delegate
                                    {
                                        if (ctr_Display_1.pictureBox_2.Image != null)
                                        {
                                            ctr_Display_1.pictureBox_2.Image = null;
                                        }
                                        ctr_Display_1.pictureBox_2.Refresh();
                                    });
                                }
                                else
                                {
                                    if (ctr_Display_1.pictureBox_2.Image != null)
                                    {
                                        ctr_Display_1.pictureBox_2.Image = null;
                                    }
                                    ctr_Display_1.pictureBox_2.Refresh();
                                }
                            }
                        }
                        //lock (Result_framebuffer[Cam_Num])
                        {
                            if (Result_framebuffer[Cam_Num].Count > 0)
                            {
                                Result_framebuffer[Cam_Num].Clear();
                            }
                        }
                        //if (LVApp.Instance().t_Util.m_FPS[Cam_Num + 4] > 4.9)
                        //{
                        //    Thread.Sleep(1000 / 4);
                        //}
                        //else
                        //{
                        //    Thread.Sleep(10);
                        //}
                        m_Result_Job_Mode2 = 0;
                    }
                }
            }
            catch
            {
                m_ViewThreads_Check[Cam_Num] = false;
                //if (Viewthreads[Cam_Num] != null)
                //{
                //    Viewthreads[Cam_Num].Abort();
                //    Viewthreads[Cam_Num] = null;
                //}
                //m_Result_Job_Mode2 = 0;
                //Viewthreads[Cam_Num] = new Thread(ResultProc2);
                //Viewthreads[Cam_Num].IsBackground = true;
                //Viewthreads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " View Thread Restart");
                //if (Result_framebuffer[Cam_Num].Count > 0)
                //{
                //    Result_framebuffer[Cam_Num].Clear();
                //}
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public void ThreadProc3()
        {
            int Cam_Num = 3;
            //bool t_onecycle_check = false;
            try
            {
                while (m_Threads_Check[Cam_Num])
                {
                    if (m_Job_Mode3 == 0)
                    {
                        Thread.Sleep(1);
                    }
                    else if (m_Job_Mode3 == 1 && Capture_framebuffer[Cam_Num].Count > 0)
                    {
                        Bitmap Capture_frame = null;
                        //DebugLogger.Instance().LogRecord($"mergeProcessCount : {_mergeProcessCount[Cam_Num]}");
                        if (LVApp.Instance().m_Config.Image_Merge_Check[Cam_Num])
                        {
                            Bitmap Capture_frame_For_Merge = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                            Capture_framebuffer[Cam_Num].RemoveAt(0);

                            if (_is_NewFrame[Cam_Num])
                            {
                                _is_NewFrame[Cam_Num] = false;
                                LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num] = new Bitmap(Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height * LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num]);
                                if (_mergeProcessCount[Cam_Num] != 0)
                                {
                                    // Interval 이 긴데도 Image_Merge Index가 남아 있으면, 이전 제품 이미지가 완전히 Merge 되지 않았다는 의미
                                    //DebugLogger.Instance().LogRecord($"Cam{Cam_Num} Miss! - Previous: {_mergeProcessCount[Cam_Num]}");
                                    _mergeProcessCount[Cam_Num] = 0;
                                }
                            }
                            else if (_mergeProcessCount[Cam_Num] == 0)
                            {
                                // 연속 그랩 중인데도, 알고리즘 처리 후 첫 이미지 인 경우
                                // 연속 그랩 중 & (이전 이미지에 대해) 알고리즘 처리 완료
                                // 이전 제품에 대해 Merge를 다 한 후(알고리즘 동작 완료 플래그<0>)에서도 연속 그랩 중인 경우 리턴
                                return;
                            }

                            if (_mergeProcessCount[Cam_Num] >= 0 && _mergeProcessCount[Cam_Num] < LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                Rectangle bounds = new Rectangle(0, Capture_frame_For_Merge.Height * _mergeProcessCount[Cam_Num], Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height);
                                using (Graphics g = Graphics.FromImage(LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num]))
                                {
                                    g.DrawImage(Capture_frame_For_Merge, bounds, 0, 0, Capture_frame_For_Merge.Width, Capture_frame_For_Merge.Height, GraphicsUnit.Pixel);
                                }
                                ++_mergeProcessCount[Cam_Num];
                            }

                            if (_mergeProcessCount[Cam_Num] == LVApp.Instance().m_Config.Image_Merge_Number[Cam_Num])
                            {
                                DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                                LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                                Run_SW[Cam_Num].Reset();
                                Run_SW[Cam_Num].Start();

                                Capture_framebuffer[Cam_Num].Clear();
                                _mergeProcessCount[Cam_Num] = 0;

                                Capture_frame = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)LVApp.Instance().m_Config.Image_Merge_BMP[Cam_Num].Clone());

                                #region LHJ - 240804 - 디버깅용, 240806(Merge 여부에 따라 Count 를 다르게 계산)
                                // 검사 중 SW가 정지되는 현상을 디버깅 하기 위함
                                // 알고리즘을 다운 시키는 이미지가 있는지 확인
                                // 검사 종료 시, 카메라마다 마지막 이미지, 그 이전 이미지(총 두 이미지)를 저장함
                                LVApp.Instance().m_Config._lastImage_Cam3[_mergeImageCount[Cam_Num] % LV_Config._lastImageCount] = (Bitmap)Capture_frame.Clone();
                                #endregion
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            DebugLogger.Instance().LogRecord($"CAM{Cam_Num} PROC Start");

                            LVApp.Instance().t_Util.CalculateFrameRate(Cam_Num);
                            //if (t_onecycle_check)
                            //{
                            //    m_Job_Mode0 = 0;
                            //    t_onecycle_check = false;
                            //    continue;
                            //}
                            //t_onecycle_check = true;
                            Run_SW[Cam_Num].Reset();
                            Run_SW[Cam_Num].Start();

                            //lock (Capture_framebuffer[Cam_Num])
                            {
                                Capture_frame = Capture_framebuffer[Cam_Num][0].Clone() as Bitmap;
                                Capture_framebuffer[Cam_Num].Clear();
                            }
                        }
                        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        }
                        if (!Simulation_mode || !LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipX);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipY);
                                }
                            }
                            else if (LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipXY);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipXY);
                                }
                            }
                            else
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                                {
                                    //Capture_frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else if (LVApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                                {
                                    Capture_frame.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                            }
                        }

                        if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            if (m_Result_Job_Mode3 == 0)
                            {
                                LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;
                                //lock (Result_framebuffer[Cam_Num])
                                {
                                    Result_framebuffer[Cam_Num].Add((Bitmap)Capture_frame.Clone());
                                }
                                //m_Result_Job_Mode3 = 1;
                            }
                            //Capture_frame.Dispose();
                        }
                        else // 검사하면 아래로
                        {
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                            }

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            if (Capture_frame.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                if (LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2 || LVApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 6)
                                {
                                    byte[] arr = BmpToArray3((Bitmap)Capture_frame.Clone());
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, Capture_frame.Width, Capture_frame.Height, 3, Cam_Num);
                                }
                                else
                                {
                                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply((Bitmap)Capture_frame.Clone());
                                    byte[] arr = BmpToArray3(grayImage);
                                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                    grayImage.Dispose();
                                }
                            }
                            else
                            {
                                byte[] arr = BmpToArray3((Bitmap)Capture_frame.Clone());
                                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, Capture_frame.Width, Capture_frame.Height, 1, Cam_Num);
                            }

                            ctr_Manual1.Run_Inspection(Cam_Num, ref Capture_frame);
                            LVApp.Instance().m_Config.Image_Merge_Idx[Cam_Num] = 0;

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                            byte[] Dst_Img = null;
                            int width = 0, height = 0, ch = 0;

                            if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image3(out Dst_Img, out width, out height, out ch, Cam_Num))
                            {
                                Bitmap t_bmp = ConvertBitmap3(Dst_Img, width, height, ch);
                                if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40 && m_Result_Job_Mode3 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_Result_Job_Mode3 == 0)
                                    {
                                        //lock (Result_framebuffer[Cam_Num])
                                        {
                                            //Result_framebuffer[Cam_Num].Add(t_bmp);
                                            Result_framebuffer[Cam_Num].Add((Bitmap)t_bmp.Clone());
                                        }
                                    }
                                }

                                if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                {
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                    LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)t_bmp.Clone();
                                    LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                }

                                if (LVApp.Instance().m_Config.SSF_Image_Save && t_Judge == false)
                                {
                                    LVApp.Instance().m_Config.SSF_Result_Image_Save(Cam_Num, (Bitmap)t_bmp.Clone(), 1);
                                }

                                t_bmp.Dispose();
                                //if (m_Result_Job_Mode3 == 0)
                                //{
                                //    if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //    {
                                //        if (Judge != 40)
                                //        {
                                //            m_Result_Job_Mode3 = 1;
                                //        }
                                //        if (t_cam_setting_view_mode)
                                //        {
                                //            m_Result_Job_Mode3 = 1;
                                //        }

                                //    }
                                //    else
                                //    {
                                //        m_Result_Job_Mode3 = 1;
                                //    }
                                //}
                            }

                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }

                            if (Run_SW[Cam_Num].ElapsedMilliseconds < ctr_PLC1.m_MinProcessingTime && ctr_PLC1.m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                            {
                                Thread.Sleep(ctr_PLC1.m_MinProcessingTime - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                            }

                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            String filename = string.Empty;
                            if (LVApp.Instance().m_Config.m_Cam_Log_Method == 3)
                            {
                                if (t_Judge)
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_OK";
                                }
                                else
                                {
                                    filename = DateTime.Now.ToString("yyyyMMdd HH_mm_ss_fff") + "_NG";
                                }

                                //filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, Capture_frame, false);
                            }
                            else
                            {
                                Bitmap img = (Bitmap)Capture_frame.Clone();
                                //Bitmap NewImg = new Bitmap(img);
                                //img.Dispose();
                                if (t_Judge)
                                {
                                    filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 0);
                                }
                                else
                                {
                                    if (Judge == -1)
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 2);
                                    }
                                    else
                                    {
                                        filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, img, 1);
                                    }
                                }
                            }

                            int t_wait_cnt = 0;
                            while (!ctr_PLC1.MC_Rx_Value_Updated[Cam_Num])
                            {
                                Thread.Sleep(1);
                                t_wait_cnt++;
                                if (t_wait_cnt >= ctr_PLC1.m_MinProcessingTime)
                                {
                                    break;
                                }
                            }

                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                        }

                        //if (Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                        //{
                        //    Thread.Sleep(50 - (int)Run_SW[Cam_Num].ElapsedMilliseconds);
                        //}
                        Run_SW[Cam_Num].Stop();

                        LVApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + LVApp.Instance().t_Util.m_FPS[Cam_Num].ToString("0.0") + "/" + LVApp.Instance().t_Util.m_FPS[Cam_Num + 4].ToString("0.0");
                        LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                        //lock (Capture_framebuffer[Cam_Num])
                        //if (Capture_framebuffer[Cam_Num].Count > 0)
                        //{
                        //    Capture_framebuffer[Cam_Num].Clear();
                        //}
                        Capture_frame.Dispose();
                        LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                        //t_onecycle_check = false;
                        m_Job_Mode3 = 0;
                        DebugLogger.Instance().LogRecord($"CAM3 PROC End - {LVApp.Instance().m_Config.m_TT[Cam_Num]}");
                    }

                    if (!m_Threads_Check[Cam_Num])
                    {
                        break;
                    }
                }
            }
            catch
            {
                m_Threads_Check[Cam_Num] = false;
                //if (threads[Cam_Num] != null)
                //{
                //    threads[Cam_Num].Abort();
                //    threads[Cam_Num] = null;
                //}
                //m_Job_Mode3 = 0;
                //threads[Cam_Num] = new Thread(ThreadProc3);
                //threads[Cam_Num].IsBackground = true;
                //threads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " Inspection Thread Restart");
                //if (Capture_framebuffer[Cam_Num].Count > 0)
                //{
                //    Capture_framebuffer[Cam_Num].Clear();
                //}
            }

        }

        public void ResultProc3()
        {
            int Cam_Num = 3;
            try
            {
                while (m_ViewThreads_Check[Cam_Num])
                {
                    if (!m_ViewThreads_Check[Cam_Num])
                    {
                        break;
                    }

                    if (m_Result_Job_Mode3 == 0)
                    {
                        if (Result_framebuffer[Cam_Num].Count > 0)
                        {
                            m_Result_Job_Mode3 = 1;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else if (m_Result_Job_Mode3 == 1)
                    {
                        if (LVApp.Instance().m_Config.Realtime_View_Check)
                        {
                            //if (t_Main_View != 0 && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            //{
                            //    //lock (Result_framebuffer[Cam_Num])
                            //    {
                            //        if (Result_framebuffer[Cam_Num].Count > 0)
                            //        {
                            //            Result_framebuffer[Cam_Num].Clear();
                            //        }
                            //    }
                            //    Thread.Sleep(5);
                            //    m_Result_Job_Mode3 = 0;
                            //    continue;
                            //}
                            Bitmap bitmapNew = null;
                            //lock (Result_framebuffer[Cam_Num])
                            {
                                bitmapNew = Result_framebuffer[Cam_Num][0];
                            }
                            if (t_cam_ROI_view_mode)
                            {
                                //if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                                //{
                                //    if (ctr_ROI4.pictureBox_Image.InvokeRequired)
                                //    {
                                //        ctr_ROI4.pictureBox_Image.Invoke((MethodInvoker)delegate
                                //        {
                                //            ctr_ROI4.pictureBox_Image.Image = bitmapNew;
                                //        });
                                //    }
                                //    else
                                //    {
                                //        ctr_ROI4.pictureBox_Image.Image = bitmapNew;
                                //    }
                                //}
                            }
                            else
                            {
                                if (t_setting_view_mode && !t_cam_setting_view_mode)
                                {// 판정 설정 뷰
                                    if (pictureBox_Setting_3.InvokeRequired)
                                    {
                                        pictureBox_Setting_3.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_Setting_3.Image as Bitmap;
                                            pictureBox_Setting_3.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_Setting_3.Image as Bitmap;
                                        pictureBox_Setting_3.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && t_cam_setting_view_mode)
                                {// 카메라 설정 뷰
                                    if (pictureBox_CAM3.InvokeRequired)
                                    {
                                        pictureBox_CAM3.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = pictureBox_CAM3.Image as Bitmap;
                                            pictureBox_CAM3.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = pictureBox_CAM3.Image as Bitmap;
                                        pictureBox_CAM3.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                else if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                                {// 메인 뷰
                                    if (ctr_Display_1.pictureBox_3.InvokeRequired)
                                    {
                                        ctr_Display_1.pictureBox_3.Invoke((MethodInvoker)delegate
                                        {
                                            //Bitmap bitmapOld = ctr_Display_1.pictureBox_3.Image as Bitmap;
                                            ctr_Display_1.pictureBox_3.Image = bitmapNew;
                                            //if (bitmapOld != null)
                                            //{
                                            //    bitmapOld.Dispose();
                                            //}
                                        });
                                    }
                                    else
                                    {
                                        //Bitmap bitmapOld = ctr_Display_1.pictureBox_3.Image as Bitmap;
                                        ctr_Display_1.pictureBox_3.Image = bitmapNew;
                                        //if (bitmapOld != null)
                                        //{
                                        //    bitmapOld.Dispose();
                                        //}
                                    }
                                }
                                //bitmapNew.Dispose();
                            }
                        }
                        else
                        {
                            if (!t_setting_view_mode && !t_cam_setting_view_mode && LVApp.Instance().m_Config.neoTabWindow_MAIN_idx == 0)
                            {// 메인 뷰
                                if (ctr_Display_1.pictureBox_3.InvokeRequired)
                                {
                                    ctr_Display_1.pictureBox_3.Invoke((MethodInvoker)delegate
                                    {
                                        if (ctr_Display_1.pictureBox_3.Image != null)
                                        {
                                            ctr_Display_1.pictureBox_3.Image = null;
                                        }
                                        ctr_Display_1.pictureBox_3.Refresh();
                                    });
                                }
                                else
                                {
                                    if (ctr_Display_1.pictureBox_3.Image != null)
                                    {
                                        ctr_Display_1.pictureBox_3.Image = null;
                                    }
                                    ctr_Display_1.pictureBox_3.Refresh();
                                }
                            }
                        }
                        //lock (Result_framebuffer[Cam_Num])
                        {
                            if (Result_framebuffer[Cam_Num].Count > 0)
                            {
                                Result_framebuffer[Cam_Num].Clear();
                            }
                        }
                        //if (LVApp.Instance().t_Util.m_FPS[Cam_Num + 4] > 4.9)
                        //{
                        //    Thread.Sleep(1000 / 4);
                        //}
                        //else
                        //{
                        //    Thread.Sleep(10);
                        //}
                        m_Result_Job_Mode3 = 0;
                    }
                }
            }
            catch
            {
                m_ViewThreads_Check[Cam_Num] = false;
                //if (Viewthreads[Cam_Num] != null)
                //{
                //    Viewthreads[Cam_Num].Abort();
                //    Viewthreads[Cam_Num] = null;
                //}
                //m_Result_Job_Mode3 = 0;
                //Viewthreads[Cam_Num] = new Thread(ResultProc3);
                //Viewthreads[Cam_Num].IsBackground = true;
                //Viewthreads[Cam_Num].Start();
                //add_Log("CAM" + Cam_Num.ToString() + " View Thread Restart");
                //if (Result_framebuffer[Cam_Num].Count > 0)
                //{
                //    Result_framebuffer[Cam_Num].Clear();
                //}
            }
        }
        //private Stopwatch Cam3_Missed_SW = new Stopwatch();
        //public void ThreadProc3()
        //{
        //    int Cam_Num = 3;
        //    while (m_Threads_Check[Cam_Num])
        //    {
        //        if (m_Job_Mode3 == 0)
        //        {
        //            Thread.Sleep(10);
        //            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
        //            //{

        //            //    if (Cam3_Missed_SW.ElapsedMilliseconds > 100 && Cam3_Missed > 0 && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
        //            //    {
        //            //        string filename = LVApp.Instance().excute_path + "\\Save\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "Cam3_Missed_" + Cam3_Missed.ToString() + ".bmp";
        //            //        Capture_Image3[Capture_Count[Cam_Num]] = new Bitmap(filename);
        //            //        File.Delete(filename);
        //            //        Cam3_Missed--;
        //            //        m_Job_Mode3 = 1;
        //            //    }
        //            //    else if (Cam3_Missed_SW.ElapsedMilliseconds > 100 && Cam3_Missed > 0 && LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
        //            //    {
        //            //        Cam3_Missed_SW.Reset();
        //            //        Cam3_Missed_SW.Start();
        //            //    }
        //            //}
        //        }
        //        else if (m_Job_Mode3 == 1)
        //        {
        //            //if (ctrCam4.m_bitmap != null && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
        //            //{
        //            //    Capture_Count[Cam_Num]++;
        //            //    if (Capture_Count[Cam_Num] > 4)
        //            //    {
        //            //        Capture_Count[Cam_Num] = 0;
        //            //    }
        //            //    Capture_Image3[Capture_Count[Cam_Num]] = (Bitmap)ctrCam4.m_bitmap.Clone();
        //            //}
        //            //else
        //            //{
        //            //    Thread.Sleep(3);
        //            //    m_Job_Mode3 = 0;
        //            //    continue;
        //            //}
        //            Run_SW[Cam_Num].Reset();
        //            Run_SW[Cam_Num].Start();
        //            int CamNum = Cam_Num;


        //            if (LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
        //            {
        //                if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
        //                }
        //            }
        //            else if (LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
        //            {
        //                if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
        //                }
        //            }
        //            else if (LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
        //            {
        //                if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
        //                }
        //            }
        //            else
        //            {
        //                if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    //Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
        //                }
        //                else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
        //                }
        //            }

        //            if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
        //            {
        //                //lock (this)
        //                {
        //                    ctr_Camera_Setting4.Grab_Num++;
        //                    if (pictureBox_Setting_3.InvokeRequired)
        //                    {
        //                        pictureBox_Setting_3.Invoke((MethodInvoker)delegate
        //                        {
        //                            if (t_setting_view_mode)
        //                            {
        //                                pictureBox_Setting_3.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                                pictureBox_Setting_3.Refresh();
        //                            }
        //                        });
        //                    }
        //                    else
        //                    {
        //                        if (t_setting_view_mode)
        //                        {
        //                            pictureBox_Setting_3.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                            pictureBox_Setting_3.Refresh();
        //                        }
        //                    }

        //                    if (pictureBox_CAM3.InvokeRequired)
        //                    {
        //                        pictureBox_CAM3.Invoke((MethodInvoker)delegate
        //                        {
        //                            if (t_cam_setting_view_mode)
        //                            {
        //                                pictureBox_CAM3.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                                pictureBox_CAM3.Refresh();
        //                            }
        //                        });
        //                    }
        //                    else
        //                    {
        //                        if (t_cam_setting_view_mode)
        //                        {
        //                            pictureBox_CAM3.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                            pictureBox_CAM3.Refresh();
        //                        }
        //                    }


        //                    if (ctr_Display_1.m_Selected_PictureBox == Cam_Num)
        //                    {
        //                        if (ctr_Display_1.pictureBox_Main.InvokeRequired)
        //                        {
        //                            ctr_Display_1.pictureBox_Main.Invoke((MethodInvoker)delegate
        //                            {
        //                                ctr_Display_1.pictureBox_Main.Image = null;
        //                                ctr_Display_1.pictureBox_Main.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                                ctr_Display_1.pictureBox_Main.Refresh();
        //                            });
        //                        }
        //                        else
        //                        {
        //                            ctr_Display_1.pictureBox_Main.Image = null;
        //                            ctr_Display_1.pictureBox_Main.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                            ctr_Display_1.pictureBox_Main.Refresh();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (ctr_Display_1.pictureBox_3.InvokeRequired)
        //                        {
        //                            ctr_Display_1.pictureBox_3.Invoke((MethodInvoker)delegate
        //                            {
        //                                ctr_Display_1.pictureBox_3.Image = null;
        //                                ctr_Display_1.pictureBox_3.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                                ctr_Display_1.pictureBox_3.Refresh();
        //                            });
        //                        }
        //                        else
        //                        {
        //                            ctr_Display_1.pictureBox_3.Image = null;
        //                            ctr_Display_1.pictureBox_3.Image = (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone();
        //                            ctr_Display_1.pictureBox_3.Refresh();
        //                        }
        //                    }
        //                }
        //                String filename = LVApp.Instance().m_Config.Result_Image_Save(CamNum, Capture_Image3[Capture_Count[Cam_Num]], true);
        //            }
        //            else // 검사하면 아래로
        //            {
        //                ctr_Camera_Setting4.Grab_Num++;
        //                //if (LVApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum])
        //                //{
        //                //    // 검사 실패
        //                //    return;
        //                //}
        //                LVApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = true;
        //                if (Capture_Image3[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
        //                {
        //                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image3[Capture_Count[Cam_Num]]);
        //                    byte[] arr = BmpToArray3(grayImage);
        //                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, CamNum);
        //                    grayImage.Dispose();
        //                }
        //                else
        //                {
        //                    //byte[] arr = BmpToArray3((Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone());
        //                    byte[] arr = BmpToArray3(Capture_Image3[Capture_Count[Cam_Num]]);
        //                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, Capture_Image3[Capture_Count[Cam_Num]].Width, Capture_Image3[Capture_Count[Cam_Num]].Height, 1, CamNum);
        //                }

        //                bool t_Judge = true;
        //                //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(CamNum);
        //                //string[] strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_Algorithm(m_Selected_Cam_Num).Split('_');
        //                ctr_Manual1.Run_Inspection(CamNum);
        //                int Judge = LVApp.Instance().m_Config.Judge_DataSet(CamNum);
        //                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].BeginLoadData();
        //                if (Judge != 40)
        //                {
        //                    LVApp.Instance().m_Config.m_Error_Flag[CamNum] = 1;
        //                    LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1]++;
        //                    // LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
        //                    t_Judge = false;
        //                }
        //                else
        //                {
        //                    LVApp.Instance().m_Config.m_Error_Flag[CamNum] = 0;
        //                    LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0]++;
        //                    //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0];
        //                }
        //                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
        //                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].EndLoadData();
        //                //if (this.InvokeRequired)
        //                //{
        //                //    this.Invoke((MethodInvoker)delegate
        //                //    {
        //                //        ctr_PLC1.send_Message.Add("DW510" + CamNum.ToString() + "_" + Judge.ToString());
        //                //    });
        //                //}
        //                //else
        //                //{
        //                Thread.Sleep(10);
        //                ctr_PLC1.send_Message.Add("DW510" + CamNum.ToString() + "_" + Judge.ToString());
        //                //}
        //                if (LVApp.Instance().m_Config.Realtime_View_Check)
        //                {

        //                    byte[] Dst_Img = null;
        //                    int width = 0, height = 0, ch = 0;

        //                    if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image(out Dst_Img, out width, out height, out ch, CamNum))
        //                    {
        //                        Bitmap t_Bitmap = ConvertBitmap(Dst_Img, width, height, ch);

        //                        if (pictureBox_Setting_3.InvokeRequired)
        //                        {
        //                            pictureBox_Setting_3.Invoke((MethodInvoker)delegate
        //                            {
        //                                if (t_setting_view_mode)
        //                                {
        //                                    pictureBox_Setting_3.Image = null;
        //                                    pictureBox_Setting_3.Image = (Bitmap)t_Bitmap.Clone();
        //                                    pictureBox_Setting_3.Refresh();
        //                                }
        //                            });
        //                        }
        //                        else
        //                        {
        //                            if (t_setting_view_mode)
        //                            {
        //                                pictureBox_Setting_3.Image = null;
        //                                pictureBox_Setting_3.Image = (Bitmap)t_Bitmap.Clone();
        //                                pictureBox_Setting_3.Refresh();
        //                            }
        //                        }

        //                        if (pictureBox_CAM3.InvokeRequired)
        //                        {
        //                            pictureBox_CAM3.Invoke((MethodInvoker)delegate
        //                            {
        //                                if (t_cam_setting_view_mode)
        //                                {
        //                                    pictureBox_CAM3.Image = null;
        //                                    pictureBox_CAM3.Image = (Bitmap)t_Bitmap.Clone();
        //                                    pictureBox_CAM3.Refresh();
        //                                }
        //                            });
        //                        }
        //                        else
        //                        {
        //                            if (t_cam_setting_view_mode)
        //                            {
        //                                pictureBox_CAM3.Image = null;
        //                                pictureBox_CAM3.Image = (Bitmap)t_Bitmap.Clone();
        //                                pictureBox_CAM3.Refresh();
        //                            }
        //                        }


        //                        if (ctr_Display_1.m_Selected_PictureBox == Cam_Num)
        //                        {
        //                            if (ctr_Display_1.pictureBox_Main.InvokeRequired)
        //                            {
        //                                ctr_Display_1.pictureBox_Main.Invoke((MethodInvoker)delegate
        //                                {
        //                                    ctr_Display_1.pictureBox_Main.Image = null;
        //                                    ctr_Display_1.pictureBox_Main.Image = (Bitmap)t_Bitmap.Clone();
        //                                    ctr_Display_1.pictureBox_Main.Refresh();
        //                                });
        //                            }
        //                            else
        //                            {
        //                                ctr_Display_1.pictureBox_Main.Image = null;
        //                                ctr_Display_1.pictureBox_Main.Image = (Bitmap)t_Bitmap.Clone();
        //                                ctr_Display_1.pictureBox_Main.Refresh();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (ctr_Display_1.pictureBox_3.InvokeRequired)
        //                            {
        //                                ctr_Display_1.pictureBox_3.Invoke((MethodInvoker)delegate
        //                                {
        //                                    ctr_Display_1.pictureBox_3.Image = null;
        //                                    ctr_Display_1.pictureBox_3.Image = (Bitmap)t_Bitmap.Clone();
        //                                    ctr_Display_1.pictureBox_3.Refresh();
        //                                });
        //                            }
        //                            else
        //                            {
        //                                ctr_Display_1.pictureBox_3.Image = null;
        //                                ctr_Display_1.pictureBox_3.Image = (Bitmap)t_Bitmap.Clone();
        //                                ctr_Display_1.pictureBox_3.Refresh();
        //                            }
        //                        }
        //                        t_Bitmap.Dispose();
        //                    }
        //                }

        //                if (Capture_Image3[Capture_Count[Cam_Num]] != null)
        //                {
        //                    String filename = LVApp.Instance().m_Config.Result_Image_Save(CamNum, (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone(), t_Judge);
        //                    LVApp.Instance().m_Config.Add_Log_Data(CamNum, filename);
        //                }
        //            }
        //            LVApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = false;

        //            m_Job_Mode3 = 0;
        //            Run_SW[Cam_Num].Stop();
        //            LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";
        //            //Cam3_Missed_SW.Reset();
        //            //Cam3_Missed_SW.Start();
        //        }

        //        if (!m_Threads_Check[Cam_Num])
        //        {
        //            break;
        //        }
        //    }
        //}

        //private Stopwatch Cam4_Missed_SW = new Stopwatch();
        public void ThreadProc4()
        {
            //            int Cam_Num = 4;
            //            while (m_Threads_Check[Cam_Num])
            //            {
            //                if (m_Job_Mode4 == 0)
            //                {
            //                    Thread.Sleep(1);
            //                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //                    //{

            //                    //    if (Cam4_Missed_SW.ElapsedMilliseconds > 100 && Cam4_Missed > 0 && !LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            //                    //    {
            //                    //        string filename = LVApp.Instance().excute_path + "\\Save\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "Cam4_Missed_" + Cam4_Missed.ToString() + ".bmp";
            //                    //        Capture_Image4[Capture_Count[Cam_Num]] = new Bitmap(filename);
            //                    //        File.Delete(filename);
            //                    //        Cam4_Missed--;
            //                    //        m_Job_Mode4 = 1;
            //                    //    }
            //                    //    else if (Cam4_Missed_SW.ElapsedMilliseconds > 100 && Cam4_Missed > 0 && LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            //                    //    {
            //                    //        Cam4_Missed_SW.Reset();
            //                    //        Cam4_Missed_SW.Start();
            //                    //    }
            //                    //}
            //                }
            //                else if (m_Job_Mode4 == 1)
            //                {
            //                    //Cam4_Missed_SW.Stop();
            //                    Run_SW[Cam_Num].Reset();
            //                    Run_SW[Cam_Num].Start();
            //                    int CamNum = Cam_Num;

            //                    if (LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
            //                    {
            //                        if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
            //                        }
            //                    }
            //                    else if (LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            //                    {
            //                        if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
            //                        }
            //                    }
            //                    else if (LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            //                    {
            //                        if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            //Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
            //                        }
            //                        else if (LVApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
            //                        }
            //                    }
            //                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //                    {
            //                        lock (this)
            //                        {
            //                            ctr_Camera_Setting5.Grab_Num++;

            //                            if (pictureBox_Setting_4.InvokeRequired)
            //                            {
            //                                pictureBox_Setting_4.Invoke((MethodInvoker)delegate
            //                                {
            //                                    if (t_setting_view_mode)
            //                                    {
            //                                        pictureBox_Setting_4.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                        pictureBox_Setting_4.Refresh();
            //                                    }
            //                                });
            //                            }
            //                            else
            //                            {
            //                                if (t_setting_view_mode)
            //                                {
            //                                    pictureBox_Setting_4.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                    pictureBox_Setting_4.Refresh();
            //                                }
            //                            }

            //                            if (pictureBox_CAM4.InvokeRequired)
            //                            {
            //                                pictureBox_CAM4.Invoke((MethodInvoker)delegate
            //                                {
            //                                    if (t_cam_setting_view_mode)
            //                                    {
            //                                        pictureBox_CAM4.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                        pictureBox_CAM4.Refresh();
            //                                    }
            //                                });
            //                            }
            //                            else
            //                            {
            //                                if (t_cam_setting_view_mode)
            //                                {
            //                                    pictureBox_CAM4.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                    pictureBox_CAM4.Refresh();
            //                                }
            //                            }

            //                            if (ctr_Display_1.m_Selected_PictureBox == Cam_Num)
            //                            {
            //                                if (ctr_Display_1.pictureBox_Main.InvokeRequired)
            //                                {
            //                                    ctr_Display_1.pictureBox_Main.Invoke((MethodInvoker)delegate
            //                                    {
            //                                        ctr_Display_1.pictureBox_Main.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                        ctr_Display_1.pictureBox_Main.Refresh();
            //                                    });
            //                                }
            //                                else
            //                                {
            //                                    ctr_Display_1.pictureBox_Main.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                    ctr_Display_1.pictureBox_Main.Refresh();
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (ctr_Display_1.pictureBox_4.InvokeRequired)
            //                                {
            //                                    ctr_Display_1.pictureBox_4.Invoke((MethodInvoker)delegate
            //                                    {
            //                                        ctr_Display_1.pictureBox_4.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                        ctr_Display_1.pictureBox_4.Refresh();
            //                                    });
            //                                }
            //                                else
            //                                {
            //                                    ctr_Display_1.pictureBox_4.Image = (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone();
            //                                    ctr_Display_1.pictureBox_4.Refresh();
            //                                }
            //                            }
            //                        }
            //                        String filename = LVApp.Instance().m_Config.Result_Image_Save(CamNum, Capture_Image4[Capture_Count[Cam_Num]], true);
            //                    }
            //                    else // 검사하면 아래로
            //                    {
            //                        ctr_Camera_Setting5.Grab_Num++;
            //                        //if (LVApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum])
            //                        //{
            //                        //    // 검사 실패
            //                        //    return;
            //                        //}
            //                        LVApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = true;
            //                        if (Capture_Image4[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
            //                        {
            //                            Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image4[Capture_Count[Cam_Num]]);
            //                            byte[] arr = BmpToArray4(grayImage);
            //                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_4(arr, grayImage.Width, grayImage.Height, 1, CamNum);
            //                            grayImage.Dispose();
            //                        }
            //                        else
            //                        {
            //                            byte[] arr = BmpToArray4((Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone());
            //                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_4(arr, Capture_Image4[Capture_Count[Cam_Num]].Width, Capture_Image4[Capture_Count[Cam_Num]].Height, 1, CamNum);
            //                        }

            //                        bool t_Judge = true;
            //                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(CamNum);
            //                        //string[] strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_Algorithm(m_Selected_Cam_Num).Split('_');
            //                        ctr_Manual1.Run_Inspection(CamNum);
            //                        int Judge = LVApp.Instance().m_Config.Judge_DataSet(CamNum);
            //                        if (Judge != 40)
            //                        {
            //                            LVApp.Instance().m_Config.m_Error_Flag[CamNum] = 1;
            //                            LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1]++;
            //                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
            //                            t_Judge = false;
            //                        }
            //                        else
            //                        {
            //                            LVApp.Instance().m_Config.m_Error_Flag[CamNum] = 0;
            //                            LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0]++;
            //                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0];
            //                        }
            //                       // LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
            //                        //if (this.InvokeRequired)
            //                        //{
            //                        //    this.Invoke((MethodInvoker)delegate
            //                        //    {
            //                        //        ctr_PLC1.send_Message.Add("DW510" + CamNum.ToString() + "_" + Judge.ToString());
            //                        //    });
            //                        //}
            //                        //else
            //                        //{
            //                            ctr_PLC1.send_Message.Add("DW510" + CamNum.ToString() + "_" + Judge.ToString());
            //                        //}

            //                        //byte[] Dst_Img = null;
            //                        //int width = 0, height = 0, ch = 0;

            //                        //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image(out Dst_Img, out width, out height, out ch, CamNum))
            //                        //{
            //                        //    Bitmap t_Bitmap = ConvertBitmap(Dst_Img, width, height, ch);

            //                        //    //if (pictureBox_Setting_4.InvokeRequired)
            //                        //    //{
            //                        //    //    pictureBox_Setting_4.Invoke((MethodInvoker)delegate
            //                        //    //    {
            //                        //    //        if (t_setting_view_mode)
            //                        //    //        {
            //                        //    //            pictureBox_Setting_4.Image = (Bitmap)t_Bitmap.Clone();
            //                        //    //            pictureBox_Setting_4.Refresh();
            //                        //    //        }
            //                        //    //    });
            //                        //    //}
            //                        //    //else
            //                        //    //{
            //                        //    //    if (t_setting_view_mode)
            //                        //    //    {
            //                        //    //        pictureBox_Setting_4.Image = (Bitmap)t_Bitmap.Clone();
            //                        //    //        pictureBox_Setting_4.Refresh();
            //                        //    //    }
            //                        //    //}

            //                        //    //if (pictureBox_CAM4.InvokeRequired)
            //                        //    //{
            //                        //    //    pictureBox_CAM4.Invoke((MethodInvoker)delegate
            //                        //    //    {
            //                        //    //        if (t_cam_setting_view_mode)
            //                        //    //        {
            //                        //    //            pictureBox_CAM4.Image = (Bitmap)t_Bitmap.Clone();
            //                        //    //            pictureBox_CAM4.Refresh();
            //                        //    //        }
            //                        //    //    });
            //                        //    //}
            //                        //    //else
            //                        //    //{
            //                        //    //    if (t_cam_setting_view_mode)
            //                        //    //    {
            //                        //    //        pictureBox_CAM4.Image = (Bitmap)t_Bitmap.Clone();
            //                        //    //        pictureBox_CAM4.Refresh();
            //                        //    //    }
            //                        //    //}

            //                        //    if (ctr_Display_1.m_Selected_PictureBox == Cam_Num)
            //                        //    {
            //                        //        if (ctr_Display_1.pictureBox_Main.InvokeRequired)
            //                        //        {
            //                        //            ctr_Display_1.pictureBox_Main.Invoke((MethodInvoker)delegate
            //                        //            {
            //                        //                ctr_Display_1.pictureBox_Main.Image = (Bitmap)t_Bitmap.Clone();
            //                        //                ctr_Display_1.pictureBox_Main.Refresh();
            //                        //            });
            //                        //        }
            //                        //        else
            //                        //        {
            //                        //            ctr_Display_1.pictureBox_Main.Image = (Bitmap)t_Bitmap.Clone();
            //                        //            ctr_Display_1.pictureBox_Main.Refresh();
            //                        //        }
            //                        //    }
            //                        //    else
            //                        //    {
            //                        //        if (ctr_Display_1.pictureBox_4.InvokeRequired)
            //                        //        {
            //                        //            ctr_Display_1.pictureBox_4.Invoke((MethodInvoker)delegate
            //                        //            {
            //                        //                ctr_Display_1.pictureBox_4.Image = (Bitmap)t_Bitmap.Clone();
            //                        //                ctr_Display_1.pictureBox_4.Refresh();
            //                        //            });
            //                        //        }
            //                        //        else
            //                        //        {
            //                        //            ctr_Display_1.pictureBox_4.Image = (Bitmap)t_Bitmap.Clone();
            //                        //            ctr_Display_1.pictureBox_4.Refresh();
            //                        //        }
            //                        //    }

            //                        //    t_Bitmap.Dispose();
            //                        //}
            //                        if (Capture_Image4[Capture_Count[Cam_Num]] != null)
            //                        {
            //                            String filename = LVApp.Instance().m_Config.Result_Image_Save(CamNum, (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone(), t_Judge);
            //                            LVApp.Instance().m_Config.Add_Log_Data(CamNum, filename);
            //                        }
            //                    }
            //                    LVApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = false;


            //                    m_Job_Mode4 = 0;
            //                    Run_SW[Cam_Num].Stop();
            //                    LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";
            //                    //Cam4_Missed_SW.Reset();
            //                    //Cam4_Missed_SW.Start();
            //}

            //                if (!m_Threads_Check[Cam_Num])
            //                {
            //                    break;
            //                }
            //            }
        }

        private void dataGridView_AUTO_STATUS_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_AUTO_COUNT_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_0_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_4_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_5_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_6_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_7_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_Setting_Value_0_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change0));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //    {
                    //        cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change0));
                    //        cm.MenuItems.Add("==============");
                    //    }
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //    {
                    //        cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change0));
                    //        cm.MenuItems.Add("==============");
                    //    }
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }

                dataGridView_Setting_Value_0.ContextMenu = cm;
                dataGridView_Setting_Value_0.ContextMenu.Show(dataGridView_Setting_Value_0, e.Location);
                dataGridView_Setting_Value_0.ContextMenu = null;
            }
        }
        private void dataGridView_Setting_Value_Change0(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (t_CellBeginEdit_check)
                {
                    return;
                }
                t_CellBeginEdit_check = true;
                //CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_0.DataSource];
                //currencyManager00.SuspendBinding();

                if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7)
                {
                    System.Drawing.Point t_Location = dataGridView_Setting_Value_0.PointToScreen(
                       dataGridView_Setting_Value_0.GetCellDisplayRectangle(t_ColIndex, t_RowIndex, false).Location);

                    Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
                    t_Frm_Textbox.m_Cam_Num = 0;
                    t_Frm_Textbox.m_row = t_RowIndex;
                    t_Frm_Textbox.m_col = t_ColIndex;
                    t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
                    t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
                    t_Frm_Textbox.Width = dataGridView_Setting_Value_0.Columns[t_ColIndex].Width;
                    t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_0.Columns[t_ColIndex].Width;
                    //t_Frm_Textbox.textBox1.Width;
                    t_Frm_Textbox.Height = dataGridView_Setting_Value_0.Rows[t_RowIndex].Height;
                    t_Frm_Textbox.ShowDialog();
                }
                //currencyManager00.ResumeBinding();
                t_CellBeginEdit_check = false;
            }
            dataGridView_Setting_Value_0_CellEndEdit(sender, null);
        }

        private void dataGridView_Setting_Value_Change1(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (t_CellBeginEdit_check)
                {
                    return;
                }
                t_CellBeginEdit_check = true;
                //CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_0.DataSource];
                //currencyManager00.SuspendBinding();

                if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7)
                {
                    System.Drawing.Point t_Location = dataGridView_Setting_Value_1.PointToScreen(
                       dataGridView_Setting_Value_1.GetCellDisplayRectangle(t_ColIndex, t_RowIndex, false).Location);

                    Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
                    t_Frm_Textbox.m_Cam_Num = 1;
                    t_Frm_Textbox.m_row = t_RowIndex;
                    t_Frm_Textbox.m_col = t_ColIndex;
                    t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
                    t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
                    t_Frm_Textbox.Width = dataGridView_Setting_Value_1.Columns[t_ColIndex].Width;
                    t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_1.Columns[t_ColIndex].Width;
                    //t_Frm_Textbox.textBox1.Width;
                    t_Frm_Textbox.Height = dataGridView_Setting_Value_1.Rows[t_RowIndex].Height;
                    t_Frm_Textbox.ShowDialog();
                }
                //currencyManager00.ResumeBinding();
                t_CellBeginEdit_check = false;
            }
            dataGridView_Setting_Value_1_CellEndEdit(sender, null);
        }

        private void dataGridView_Setting_Value_Change2(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (t_CellBeginEdit_check)
                {
                    return;
                }
                t_CellBeginEdit_check = true;
                //CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_0.DataSource];
                //currencyManager00.SuspendBinding();

                if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7)
                {
                    System.Drawing.Point t_Location = dataGridView_Setting_Value_2.PointToScreen(
                       dataGridView_Setting_Value_2.GetCellDisplayRectangle(t_ColIndex, t_RowIndex, false).Location);

                    Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
                    t_Frm_Textbox.m_Cam_Num = 2;
                    t_Frm_Textbox.m_row = t_RowIndex;
                    t_Frm_Textbox.m_col = t_ColIndex;
                    t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
                    t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
                    t_Frm_Textbox.Width = dataGridView_Setting_Value_2.Columns[t_ColIndex].Width;
                    t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_2.Columns[t_ColIndex].Width;
                    //t_Frm_Textbox.textBox1.Width;
                    t_Frm_Textbox.Height = dataGridView_Setting_Value_2.Rows[t_RowIndex].Height;
                    t_Frm_Textbox.ShowDialog();
                }
                //currencyManager00.ResumeBinding();
                t_CellBeginEdit_check = false;
            }
            dataGridView_Setting_Value_2_CellEndEdit(sender, null);
        }

        private void dataGridView_Setting_Value_Change3(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (t_CellBeginEdit_check)
                {
                    return;
                }
                t_CellBeginEdit_check = true;
                //CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_0.DataSource];
                //currencyManager00.SuspendBinding();

                if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7)
                {
                    System.Drawing.Point t_Location = dataGridView_Setting_Value_3.PointToScreen(
                       dataGridView_Setting_Value_3.GetCellDisplayRectangle(t_ColIndex, t_RowIndex, false).Location);

                    Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
                    t_Frm_Textbox.m_Cam_Num = 3;
                    t_Frm_Textbox.m_row = t_RowIndex;
                    t_Frm_Textbox.m_col = t_ColIndex;
                    t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
                    t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
                    t_Frm_Textbox.Width = dataGridView_Setting_Value_3.Columns[t_ColIndex].Width;
                    t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_3.Columns[t_ColIndex].Width;
                    //t_Frm_Textbox.textBox1.Width;
                    t_Frm_Textbox.Height = dataGridView_Setting_Value_3.Rows[t_RowIndex].Height;
                    t_Frm_Textbox.ShowDialog();
                }
                //currencyManager00.ResumeBinding();
                t_CellBeginEdit_check = false;
            }
            dataGridView_Setting_Value_3_CellEndEdit(sender, null);
        }

        private void dataGridView_Setting_Value_Update(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Save_Judge_Data();
        }

        private void dataGridView_Setting_Value_Load(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Load_Judge_Data();
        }

        private void dataGridView_Setting_Value_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change1));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change1));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //    {
                    //        cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change0));
                    //        cm.MenuItems.Add("==============");
                    //    }
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_Value_1.ContextMenu = cm;
                dataGridView_Setting_Value_1.ContextMenu.Show(dataGridView_Setting_Value_1, e.Location);
                dataGridView_Setting_Value_1.ContextMenu = null;
            }
        }

        private void dataGridView_Setting_Value_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change2));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change2));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //    {
                    //        cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change0));
                    //        cm.MenuItems.Add("==============");
                    //    }
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_Value_2.ContextMenu = cm;
                dataGridView_Setting_Value_2.ContextMenu.Show(dataGridView_Setting_Value_2, e.Location);
                dataGridView_Setting_Value_2.ContextMenu = null;
            }

        }

        private void dataGridView_Setting_Value_3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change3));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change3));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //    {
                    //        cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change0));
                    //        cm.MenuItems.Add("==============");
                    //    }
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_Value_3.ContextMenu = cm;
                dataGridView_Setting_Value_3.ContextMenu.Show(dataGridView_Setting_Value_3, e.Location);
                dataGridView_Setting_Value_3.ContextMenu = null;
            }

        }

        private void dataGridView_Setting_Value_4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_Value_4.ContextMenu = cm;
                dataGridView_Setting_Value_4.ContextMenu.Show(dataGridView_Setting_Value_4, e.Location);
                dataGridView_Setting_Value_4.ContextMenu = null;
            }

        }

        private void neoTabWindow_ALG_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            if (neoTabWindow_MAIN.SelectedIndex == 0 && neoTabWindow_INSP_SETTING.SelectedIndex == 0 && LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("자동검사 중입니다. 정지 후 설정하세요!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("Running inspection. After stop, setup please!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    MessageBox.Show("正在运行检查。停止后，请设置!");
                }

                return;
            }
            //int old_idx = LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num];
            LVApp.Instance().m_Config.ROI_Cam_Num = neoTabWindow_ALG.SelectedIndex;
            if (LVApp.Instance().m_Config.ROI_Cam_Num == 0)
            {
                if (ctr_ROI1.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI1.listBox1.SelectedIndex = 0;
                }
                //LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI1.listBox1.SelectedIndex;
                //ctr_ROI1.listBox1.SelectedIndex = 0;
                ctr_ROI1.listBox1_SelectedIndexChanged(sender, e);
                //ctr_ROI1.listBox1.SelectedIndex = old_idx;
                ctr_ROI1.Referesh_Select_Menu(false);
                ctr_ROI1.button_LOAD_Click(sender, e);
                ctr_ROI1.load_check = false;
                ctr_ROI1.Referesh_Select_Menu(true);
                ctr_ROI1.Fit_Size();
                //ctr_ROI1.dataGridView1.ClearSelection();
                //ctr_ROI1.dataGridView1.Rows[5].Selected = true;
                //ctr_ROI1.dataGridView1.Rows[0].Height = 0;
                //if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}

                //ctr_ROI1.pictureBox_Image.Refresh();
                //ctr_ROI1.button_SAVE_Click(sender, e);
            }
            else if (LVApp.Instance().m_Config.ROI_Cam_Num == 1)
            {
                if (ctr_ROI2.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI2.listBox1.SelectedIndex = 0;
                }
                //LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI2.listBox1.SelectedIndex;
                //ctr_ROI2.listBox1.SelectedIndex = 0;
                // ctr_ROI2.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num];
                ctr_ROI2.listBox1_SelectedIndexChanged(sender, e);
                //ctr_ROI2.listBox1.SelectedIndex = old_idx;
                ctr_ROI2.Referesh_Select_Menu(false);
                ctr_ROI2.button_LOAD_Click(sender, e);
                ctr_ROI2.load_check = false;
                ctr_ROI2.Referesh_Select_Menu(true);
                ctr_ROI2.Fit_Size();
                ctr_ROI2.dataGridView1.Rows[0].Height = 0;
                //if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //ctr_ROI2.pictureBox_Image.Refresh();
                //ctr_ROI2.button_SAVE_Click(sender, e);
            }
            else if (LVApp.Instance().m_Config.ROI_Cam_Num == 2)
            {
                if (ctr_ROI3.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI3.listBox1.SelectedIndex = 0;
                }
                //LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI3.listBox1.SelectedIndex;
                ////ctr_ROI3.listBox1.SelectedIndex = 0;
                //ctr_ROI3.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num];
                ctr_ROI3.listBox1_SelectedIndexChanged(sender, e);
                ctr_ROI3.Referesh_Select_Menu(false);
                ctr_ROI3.button_LOAD_Click(sender, e);
                ctr_ROI3.load_check = false;
                ctr_ROI3.Referesh_Select_Menu(true);
                ctr_ROI3.Fit_Size();
                ctr_ROI3.dataGridView1.Rows[0].Height = 0;
                //if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //ctr_ROI3.pictureBox_Image.Refresh();
                //ctr_ROI3.button_SAVE_Click(sender, e);
            }
            else if (LVApp.Instance().m_Config.ROI_Cam_Num == 3)
            {
                if (ctr_ROI4.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI4.listBox1.SelectedIndex = 0;
                }
                //LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI4.listBox1.SelectedIndex;
                ////ctr_ROI4.listBox1.SelectedIndex = 0;
                //ctr_ROI4.listBox1.SelectedIndex = LVApp.Instance().m_Config.ROI_Selected_IDX[LVApp.Instance().m_Config.ROI_Cam_Num];
                ctr_ROI4.listBox1_SelectedIndexChanged(sender, e);
                ctr_ROI4.Referesh_Select_Menu(false);
                ctr_ROI4.button_LOAD_Click(sender, e);
                ctr_ROI4.load_check = false;
                ctr_ROI4.Referesh_Select_Menu(true);
                ctr_ROI4.Fit_Size();
                ctr_ROI4.dataGridView1.Rows[0].Height = 0;
                //if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}
                //if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}

                //ctr_ROI4.pictureBox_Image.Refresh();
                //ctr_ROI4.button_SAVE_Click(sender, e);
            }

            try
            {
                //for (int i = 0; i < Application.OpenForms.Count; i++)
                //{
                //    Form f = Application.OpenForms[i];
                //    if (f.GetType() == typeof(Frm_Trackbar))
                //    {
                //        f.Close();
                //    }
                //}
            }
            catch
            {
            }
        }

        private void neoTabWindow_ALG_MouseClick(object sender, MouseEventArgs e)
        {
            LVApp.Instance().m_Config.ROI_Cam_Num = neoTabWindow_ALG.SelectedIndex;
        }

        public int t_RowIndex = 0;
        private int t_ColIndex = 0;
        bool t_CellBeginEdit_check = false;
        private void dataGridView_Setting_Value_0_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //{
            //    //e.Cancel = true;
            //    return;
            //    if (t_CellBeginEdit_check)
            //    {
            //        return;
            //    }
            //    t_CellBeginEdit_check = true;
            //    //CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_0.DataSource];
            //    //currencyManager00.SuspendBinding();

            //    if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
            //    {
            //        System.Drawing.Point t_Location = dataGridView_Setting_Value_0.PointToScreen(
            //           dataGridView_Setting_Value_0.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location);

            //        Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
            //        t_Frm_Textbox.m_Cam_Num = 0;
            //        t_Frm_Textbox.m_row = e.RowIndex;
            //        t_Frm_Textbox.m_col = e.ColumnIndex;
            //        t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
            //        t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
            //        t_Frm_Textbox.Width = dataGridView_Setting_Value_0.Columns[e.ColumnIndex].Width;
            //        t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_0.Columns[e.ColumnIndex].Width;
            //        //t_Frm_Textbox.textBox1.Width;
            //        t_Frm_Textbox.Height = dataGridView_Setting_Value_0.Rows[e.RowIndex].Height;
            //        t_Frm_Textbox.ShowDialog();
            //    }
            //    //currencyManager00.ResumeBinding();
            //    t_CellBeginEdit_check = false;
            //}
        }

        bool t_CellClick_check = false;
        private void dataGridView_Setting_Value_0_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (t_CellClick_check)
                {
                    return;
                }

                t_CellClick_check = true;

                if (e.ColumnIndex < 1 || e.RowIndex < 0)
                {
                    t_CellClick_check = false;
                    return;
                }

                t_ColIndex = e.ColumnIndex;

                if (t_RowIndex != e.RowIndex)
                {
                    dataGridView_Setting_0.ClearSelection();
                    dataGridView_Setting_0.Rows[e.RowIndex].Selected = true;
                }
                t_RowIndex = e.RowIndex;

                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    //dataGridView_Setting_Value_0.ReadOnly = false;
                    dataGridView_Setting_Value_0.BeginEdit(true);
                }

                t_CellClick_check = false;
            }
            catch
            {
                t_CellClick_check = false;
            }
        }

        private void dataGridView_Setting_0_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex < 0)
            {
                return;
            }
            foreach (DataGridViewRow row in dataGridView_Setting_Value_0.Rows)
            {
                row.Selected = false;
            }
            dataGridView_Setting_Value_0.Rows[rowIndex].Selected = true;
        }

        private void dataGridView_Setting_1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex < 0)
            {
                return;
            }
            foreach (DataGridViewRow row in dataGridView_Setting_Value_1.Rows)
            {
                row.Selected = false;
            }
            dataGridView_Setting_Value_1.Rows[rowIndex].Selected = true;
        }

        private void dataGridView_Setting_Value_1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (t_CellClick_check)
                {
                    return;
                }

                t_CellClick_check = true;

                if (e.ColumnIndex < 1 || e.RowIndex < 0)
                {
                    t_CellClick_check = false;
                    return;
                }

                t_ColIndex = e.ColumnIndex;

                if (t_RowIndex != e.RowIndex)
                {
                    dataGridView_Setting_1.ClearSelection();
                    dataGridView_Setting_1.Rows[e.RowIndex].Selected = true;

                }
                t_RowIndex = e.RowIndex;
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    //dataGridView_Setting_Value_1.ReadOnly = false;
                    dataGridView_Setting_Value_1.BeginEdit(true);
                }

                t_CellClick_check = false;
            }
            catch
            {
                t_CellClick_check = false;
            }
        }

        private void dataGridView_Setting_Value_1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //{
            //    if (t_CellBeginEdit_check)
            //    {
            //        return;
            //    }
            //    t_CellBeginEdit_check = true;

            //    CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_1.DataSource];
            //    currencyManager00.SuspendBinding();

            //    //return;
            //    if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7 || t_ColIndex == 8)
            //    {
            //        System.Drawing.Point t_Location = dataGridView_Setting_Value_1.PointToScreen(
            //           dataGridView_Setting_Value_1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location);

            //        Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
            //        t_Frm_Textbox.m_Cam_Num = 1;
            //        t_Frm_Textbox.m_row = t_RowIndex;
            //        t_Frm_Textbox.m_col = t_ColIndex;
            //        t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
            //        t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
            //        t_Frm_Textbox.Width = dataGridView_Setting_Value_1.Columns[t_ColIndex].Width;
            //        t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_1.Columns[t_ColIndex].Width;
            //        //t_Frm_Textbox.textBox1.Width;
            //        t_Frm_Textbox.Height = dataGridView_Setting_Value_1.Rows[t_RowIndex].Height;
            //        t_Frm_Textbox.ShowDialog();
            //    }

            //    currencyManager00.ResumeBinding();
            //    t_CellBeginEdit_check = false;
            //}
        }

        private void neoTabWindow_INSP_SETTING_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            try
            {
                LVApp.Instance().m_Config.neoTabWindow_INSP_SETTING_idx = e.TabPageIndex;
                if (neoTabWindow_MAIN.SelectedIndex == 0 && neoTabWindow_INSP_SETTING.SelectedIndex == 0 && LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        MessageBox.Show("자동검사 중입니다. 정지 후 설정하세요!");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        MessageBox.Show("Running inspection. After stop, setup please!");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        MessageBox.Show("正在运行检查。停止后，请设置!");
                    }
                    //neoTabWindow_MAIN.SelectedIndex = 0;
                    neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                }


                //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                //{
                //    if (neoTabWindow_MAIN.SelectedIndex == 1 && neoTabWindow_INSP_SETTING.SelectedIndex == 0)
                //    {
                //        neoTabWindow_MAIN.SelectedIndex = 0;
                //        if (m_Language == 0)
                //        {
                //            AutoClosingMessageBox.Show("검사중 설정 불가!", "Notice", 2000);
                //        }
                //        else
                //        {
                //            AutoClosingMessageBox.Show("Can't setup during running!", "Notice", 2000);
                //        }
                //    }
                //}
                if (neoTabWindow_INSP_SETTING.SelectedIndex == 0 && LVApp.Instance().m_mainform.m_Start_Check)
                {
                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        LVApp.Instance().m_Config.Save_Judge_Data();
                        Thread.Sleep(10);
                        LVApp.Instance().m_Config.Load_Judge_Data();
                        //Thread.Sleep(10);
                    }
                }
                if (neoTabWindow_INSP_SETTING.SelectedIndex == 0)
                {
                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        int t_idx = 0;
                        foreach (DataGridViewRow row in ctr_ROI1.dataGridView1.Rows)
                        {
                            if (t_idx == 0)
                            {
                                row.Height = 0;
                            }
                            else
                            {
                                row.Height = (ctr_ROI1.dataGridView1.Height - ctr_ROI1.dataGridView1.ColumnHeadersHeight) / (ctr_ROI1.dataGridView1.Rows.Count - 5);
                            }
                            t_idx++;
                        }
                        t_idx = 0;
                        foreach (DataGridViewRow row in ctr_ROI2.dataGridView1.Rows)
                        {
                            if (t_idx == 0)
                            {
                                row.Height = 0;
                            }
                            else
                            {
                                row.Height = (ctr_ROI2.dataGridView1.Height - ctr_ROI2.dataGridView1.ColumnHeadersHeight) / (ctr_ROI2.dataGridView1.Rows.Count - 5);
                            }
                            t_idx++;
                        }
                        t_idx = 0;
                        foreach (DataGridViewRow row in ctr_ROI3.dataGridView1.Rows)
                        {
                            if (t_idx == 0)
                            {
                                row.Height = 0;
                            }
                            else
                            {
                                row.Height = (ctr_ROI3.dataGridView1.Height - ctr_ROI3.dataGridView1.ColumnHeadersHeight) / (ctr_ROI3.dataGridView1.Rows.Count - 5);
                            }
                            t_idx++;
                        }
                        t_idx = 0;
                        foreach (DataGridViewRow row in ctr_ROI4.dataGridView1.Rows)
                        {
                            if (t_idx == 0)
                            {
                                row.Height = 0;
                            }
                            else
                            {
                                row.Height = (ctr_ROI4.dataGridView1.Height - ctr_ROI4.dataGridView1.ColumnHeadersHeight) / (ctr_ROI4.dataGridView1.Rows.Count - 5);
                            }
                            t_idx++;
                        }
                    }
                    //LVApp.Instance().m_Config.Save_Judge_Data();
                    //LVApp.Instance().m_Config.Load_Judge_Data();
                }
                else
                {
                    //if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                    //    neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                    //    neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                    //    neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                    //    if (neoTabWindow_INSP_SETTING_CAM.SelectedIndex >= 0)
                    //    {
                    //        neoTabWindow_INSP_SETTING_CAM.TabPages[neoTabWindow_INSP_SETTING_CAM.SelectedIndex].Visible = true;
                    //    }
                    //}
                }

                if (neoTabWindow_INSP_SETTING.SelectedIndex == 1)
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
                                AutoClosingMessageBox.Show("请登录管理员!", "Caution", 2000);
                            }
                            neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                        }
                    }
                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        ctr_Admin_Param1.Get_Item_From_ROI();
                    }
                }
            }
            catch
            {
            }
        }

        private void dataGridView_Setting_0_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_0.ContextMenu = cm;
                dataGridView_Setting_0.ContextMenu.Show(dataGridView_Setting_0, e.Location);
                dataGridView_Setting_0.ContextMenu = null;
            }
        }

        private void dataGridView_Setting_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_1.ContextMenu = cm;
                dataGridView_Setting_1.ContextMenu.Show(dataGridView_Setting_1, e.Location);
                dataGridView_Setting_1.ContextMenu = null;
            }

        }

        private void pictureBox_Setting_0_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_Setting_0.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave0));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave0));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave0));
                    }

                    pictureBox_Setting_0.ContextMenu = cm;
                    pictureBox_Setting_0.ContextMenu.Show(pictureBox_Setting_0, e.Location);
                    pictureBox_Setting_0.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //pictureBox_Setting_0.ContextMenu = cm;
                    //pictureBox_Setting_0.ContextMenu.Show(pictureBox_Setting_0, e.Location);
                    //pictureBox_Setting_0.ContextMenu = null;
                }
            }
        }

        private void PictureBoxRealtimeview(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.Realtime_View_Check)
            {
                LVApp.Instance().m_Config.Realtime_View_Check = false;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_Display.Checked = false;
            }
            else
            {
                LVApp.Instance().m_Config.Realtime_View_Check = true;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_Display.Checked = true;
            }
            LVApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);
        }

        private void PictureBoxResultview(object sender, EventArgs e)
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
            LVApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);
            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);
        }

        private void PictureBoxPopupView0(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_Setting_0.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSave0(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_Setting_0.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp, 0);
                }
            }
        }

        private void PictureBoxPopupView1(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_Setting_1.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSave1(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_Setting_1.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp, 1);
                }
            }
        }

        private void PictureBoxSave2(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_Setting_2.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp, 2);
                }
            }
        }

        private void PictureBoxSave3(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_Setting_3.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp, 2);
                }
            }
        }

        private void PictureBoxPopupView2(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_Setting_2.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxPopupView3(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_Setting_3.Image.Clone();
            View_form.Show();
        }

        public void Image_SaveFileDialog(System.Drawing.Image bmp, int m_Selected_PictureBox)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = LVApp.Instance().excute_path;
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
            SaveFileDialog1.FileName = "Save_" + m_Selected_PictureBox.ToString() + ".bmp";
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

        private void pictureBox_Setting_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_Setting_1.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave1));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                     //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                     // cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave1));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave1));
                    }

                    pictureBox_Setting_1.ContextMenu = cm;
                    pictureBox_Setting_1.ContextMenu.Show(pictureBox_Setting_1, e.Location);
                    pictureBox_Setting_1.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave1));
                    //}

                    //pictureBox_Setting_1.ContextMenu = cm;
                    //pictureBox_Setting_1.ContextMenu.Show(pictureBox_Setting_1, e.Location);
                    //pictureBox_Setting_1.ContextMenu = null;
                }
            }
        }

        private void PictureBoxSaveCAM0(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_CAM0.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp, 0);
                }
            }
        }

        private void PictureBoxSaveCAM1(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_CAM1.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp, 1);
                }
            }
        }


        private void pictureBox_CAM0_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_CAM0.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveCAM0));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveCAM0));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSaveCAM0));
                    }

                    pictureBox_CAM0.ContextMenu = cm;
                    pictureBox_CAM0.ContextMenu.Show(pictureBox_CAM0, e.Location);
                    pictureBox_CAM0.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    //}

                    //pictureBox_CAM0.ContextMenu = cm;
                    //pictureBox_CAM0.ContextMenu.Show(pictureBox_CAM0, e.Location);
                    //pictureBox_CAM0.ContextMenu = null;
                }
            }
        }

        private void pictureBox_CAM1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_CAM1.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveCAM1));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveCAM1));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSaveCAM1));
                    }
                    pictureBox_CAM1.ContextMenu = cm;
                    pictureBox_CAM1.ContextMenu.Show(pictureBox_CAM1, e.Location);
                    pictureBox_CAM1.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //pictureBox_CAM1.ContextMenu = cm;
                    //pictureBox_CAM1.ContextMenu.Show(pictureBox_CAM1, e.Location);
                    //pictureBox_CAM1.ContextMenu = null;
                }
            }
        }

        private void dataGridView_Setting_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }

                dataGridView_Setting_2.ContextMenu = cm;
                dataGridView_Setting_2.ContextMenu.Show(dataGridView_Setting_2, e.Location);
                dataGridView_Setting_2.ContextMenu = null;
            }
        }

        private void dataGridView_Setting_2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex < 0)
            {
                return;
            }
            foreach (DataGridViewRow row in dataGridView_Setting_Value_2.Rows)
            {
                row.Selected = false;
            }
            dataGridView_Setting_Value_2.Rows[rowIndex].Selected = true;
        }

        private void dataGridView_Setting_Value_2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //{
            //    return;
            //    if (t_CellBeginEdit_check)
            //    {
            //        return;
            //    }
            //    t_CellBeginEdit_check = true;

            //    CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_2.DataSource];
            //    currencyManager00.SuspendBinding();

            //    //return;
            //    if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7 || t_ColIndex == 8)
            //    {
            //        System.Drawing.Point t_Location = dataGridView_Setting_Value_2.PointToScreen(
            //           dataGridView_Setting_Value_2.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location);

            //        Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
            //        t_Frm_Textbox.m_Cam_Num = 2;
            //        t_Frm_Textbox.m_row = t_RowIndex;
            //        t_Frm_Textbox.m_col = t_ColIndex;
            //        t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
            //        t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
            //        t_Frm_Textbox.Width = dataGridView_Setting_Value_2.Columns[t_ColIndex].Width;
            //        t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_2.Columns[t_ColIndex].Width;
            //        //t_Frm_Textbox.textBox1.Width;
            //        t_Frm_Textbox.Height = dataGridView_Setting_Value_2.Rows[t_RowIndex].Height;
            //        t_Frm_Textbox.ShowDialog();
            //    }

            //    currencyManager00.ResumeBinding();
            //    t_CellBeginEdit_check = false;
            //}
        }

        private void dataGridView_Setting_Value_2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (t_CellClick_check)
                {
                    return;
                }

                t_CellClick_check = true;

                if (e.ColumnIndex < 1 || e.RowIndex < 0)
                {
                    t_CellClick_check = false;
                    return;
                }

                t_ColIndex = e.ColumnIndex;

                if (t_RowIndex != e.RowIndex)
                {
                    dataGridView_Setting_2.ClearSelection();
                    dataGridView_Setting_2.Rows[e.RowIndex].Selected = true;

                }
                t_RowIndex = e.RowIndex;
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    //dataGridView_Setting_Value_2.ReadOnly = false;
                    dataGridView_Setting_Value_2.BeginEdit(true);
                }

                t_CellClick_check = false;
            }
            catch
            {
                t_CellClick_check = false;
            }
        }

        private void pictureBox_Setting_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_Setting_2.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave2));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup view", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave2));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("弹出视图", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave2));
                    }


                    pictureBox_Setting_2.ContextMenu = cm;
                    pictureBox_Setting_2.ContextMenu.Show(pictureBox_Setting_2, e.Location);
                    pictureBox_Setting_2.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    //}

                    //pictureBox_Setting_2.ContextMenu = cm;
                    //pictureBox_Setting_2.ContextMenu.Show(pictureBox_Setting_2, e.Location);
                    //pictureBox_Setting_2.ContextMenu = null;
                }
            }
        }

        private void button_TEST_INSPECTION0_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.m_Selected_Cam_Num = 0;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE0_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = LVApp.Instance().m_Config.ROI_Cam_Num;
            //LVApp.Instance().m_Config.ROI_Cam_Num = 0;
            //ctr_ROI1.button_SAVE_Click(sender, e);
            //LVApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void button_TEST_INSPECTION1_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.m_Selected_Cam_Num = 1;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE1_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = LVApp.Instance().m_Config.ROI_Cam_Num;
            //LVApp.Instance().m_Config.ROI_Cam_Num = 1;
            //ctr_ROI2.button_SAVE_Click(sender, e);
            //LVApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void button_TEST_INSPECTION2_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.m_Selected_Cam_Num = 2;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE2_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = LVApp.Instance().m_Config.ROI_Cam_Num;
            //LVApp.Instance().m_Config.ROI_Cam_Num = 2;
            //ctr_ROI3.button_SAVE_Click(sender, e);
            //LVApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void neoTabWindow_INSP_SETTING_CAM_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            try
            {
                LVApp.Instance().m_Config.neoTabWindow_INSP_SETTING_CAM_idx = e.TabPageIndex;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //if (neoTabWindow_INSP_SETTING_CAM.SelectedIndex >= 0 && neoTabWindow_INSP_SETTING.SelectedIndex == 0)
                //{
                //    neoTabWindow_INSP_SETTING_CAM.TabPages[neoTabWindow_INSP_SETTING_CAM.SelectedIndex].Visible = true;
                //}
            }
            catch
            {
            }
        }

        private void neoTabWindow_LOG_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            try
            {
                LVApp.Instance().m_Config.neoTabWindow_LOG_idx = e.TabPageIndex;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
                //if (neoTabWindow2_LOG.SelectedIndex >= 0 && neoTabWindow_LOG.SelectedIndex == 0)
                //{
                //    neoTabWindow2_LOG.TabPages[neoTabWindow2_LOG.SelectedIndex].Visible = true;
                //}

                if (neoTabWindow_MAIN.SelectedIndex == 3 && neoTabWindow_LOG.SelectedIndex == 2)
                {
                    ctr_Graph1.CreateGraphAll();
                    if (!ctr_Graph1.timer_Update.Enabled)
                    {
                        ctr_Graph1.timer_Update.Interval = 1000;//CreateGraphAll();
                        ctr_Graph1.timer_Update.Start();
                    }
                }
                else
                {
                    ctr_Graph1.timer_Update.Stop();
                }
            }
            catch
            {
            }
        }

        private void neoTabWindow2_LOG_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            try
            {
                LVApp.Instance().m_Config.neoTabWindow2_LOG_idx = e.TabPageIndex;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
                //if (neoTabWindow2_LOG.SelectedIndex >= 0 && neoTabWindow_LOG.SelectedIndex == 0)
                //{
                //    neoTabWindow2_LOG.TabPages[neoTabWindow2_LOG.SelectedIndex].Visible = true;
                //}
            }
            catch
            {
            }
        }

        private void dataGridView_Setting_3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex < 0)
            {
                return;
            }
            foreach (DataGridViewRow row in dataGridView_Setting_Value_3.Rows)
            {
                row.Selected = false;
            }
            dataGridView_Setting_Value_3.Rows[rowIndex].Selected = true;
        }

        private void dataGridView_Setting_3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    cm.MenuItems.Add("更新和保存", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("负荷", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_3.ContextMenu = cm;
                dataGridView_Setting_3.ContextMenu.Show(dataGridView_Setting_3, e.Location);
                dataGridView_Setting_3.ContextMenu = null;
            }
        }

        private void pictureBox_Setting_3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_Setting_3.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave3));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup view", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave3));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("弹出视图", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave3));
                    }

                    pictureBox_Setting_3.ContextMenu = cm;
                    pictureBox_Setting_3.ContextMenu.Show(pictureBox_Setting_3, e.Location);
                    pictureBox_Setting_3.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    //}

                    //pictureBox_Setting_3.ContextMenu = cm;
                    //pictureBox_Setting_3.ContextMenu.Show(pictureBox_Setting_3, e.Location);
                    //pictureBox_Setting_3.ContextMenu = null;
                }
            }
        }

        private void dataGridView_Setting_Value_3_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            //{
            //    return;
            //    if (t_CellBeginEdit_check)
            //    {
            //        return;
            //    }
            //    t_CellBeginEdit_check = true;

            //    CurrencyManager currencyManager00 = (CurrencyManager)BindingContext[dataGridView_Setting_Value_3.DataSource];
            //    currencyManager00.SuspendBinding();

            //    //return;
            //    if (t_ColIndex == 2 || t_ColIndex == 5 || t_ColIndex == 6 || t_ColIndex == 7 || t_ColIndex == 8)
            //    {
            //        System.Drawing.Point t_Location = dataGridView_Setting_Value_3.PointToScreen(
            //           dataGridView_Setting_Value_3.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location);

            //        Frm_Textbox t_Frm_Textbox = new Frm_Textbox();
            //        t_Frm_Textbox.m_Cam_Num = 3;
            //        t_Frm_Textbox.m_row = t_RowIndex;
            //        t_Frm_Textbox.m_col = t_ColIndex;
            //        t_Frm_Textbox.Left = t_Location.X;//Cursor.Position.X;
            //        t_Frm_Textbox.Top = t_Location.Y;// Cursor.Position.Y;
            //        t_Frm_Textbox.Width = dataGridView_Setting_Value_3.Columns[t_ColIndex].Width;
            //        t_Frm_Textbox.textBox1.Width = dataGridView_Setting_Value_3.Columns[t_ColIndex].Width;
            //        //t_Frm_Textbox.textBox1.Width;
            //        t_Frm_Textbox.Height = dataGridView_Setting_Value_3.Rows[t_RowIndex].Height;
            //        t_Frm_Textbox.ShowDialog();
            //    }

            //    currencyManager00.ResumeBinding();
            //    t_CellBeginEdit_check = false;
            //}
        }

        private void dataGridView_Setting_Value_3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (t_CellClick_check)
                {
                    return;
                }

                t_CellClick_check = true;

                if (e.ColumnIndex < 1 || e.RowIndex < 0)
                {
                    t_CellClick_check = false;
                    return;
                }

                t_ColIndex = e.ColumnIndex;

                if (t_RowIndex != e.RowIndex)
                {
                    dataGridView_Setting_3.ClearSelection();
                    dataGridView_Setting_3.Rows[e.RowIndex].Selected = true;

                }
                t_RowIndex = e.RowIndex;
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    //dataGridView_Setting_Value_3.ReadOnly = false;
                    dataGridView_Setting_Value_3.BeginEdit(true);
                }

                t_CellClick_check = false;
            }
            catch
            {
                t_CellClick_check = false;
            }
        }

        private void button_TEST_INSPECTION3_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.m_Selected_Cam_Num = 3;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE3_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = LVApp.Instance().m_Config.ROI_Cam_Num;
            //LVApp.Instance().m_Config.ROI_Cam_Num = 3;
            //ctr_ROI4.button_SAVE_Click(sender, e);
            //LVApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void Frm_Main_Move(object sender, EventArgs e)
        {
            //LVApp.Instance().t_QuickMenu.Top = this.Top + 7; ;
            //LVApp.Instance().t_QuickMenu.Left = this.Left + 1280 - 271 - 126;
        }

        private void button_Open0_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        private void button_Open1_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        private void button_Open2_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        private void button_Open3_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
            LVApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        public void ProbeThreadProc0()
        {
            int Cam_Num = 0;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Probe_Job_Mode0 == 0)
                {
                    Thread.Sleep(1);
                    //add_Log("ProbeThread Run");
                }
                else if (m_Probe_Job_Mode0 == 1)
                {
                    Run_SW[Cam_Num].Reset();
                    Run_SW[Cam_Num].Start();

                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        //LVApp.Instance().m_Config.Set_Parameters();
                        //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            // MessageBox.Show("ProbeThread Start");
                            double t_v = 0;
                            //double t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //MessageBox.Show("ProbeThread PLC READ");

                            LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[0][3] = t_v;
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] = t_v.ToString();
                            //MessageBox.Show("ProbeThread DATA_0 WRITE");

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            //MessageBox.Show("ProbeThread JUDGE");
                            //bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                //t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode0 == 0)
                            {
                                Result_framebuffer[Cam_Num].Add(new Bitmap(640, 480));
                                //Result_Image0[Capture_Count[Cam_Num]] = null;
                                //Result_Image0[Capture_Count[Cam_Num]] = new Bitmap(640, 480);
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;

                                //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                //if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //{
                                //    if (Judge != 40)
                                //    {
                                //        m_Result_Job_Mode0 = 1;
                                //    }
                                //    if (t_cam_setting_view_mode)
                                //    {
                                //        m_Result_Job_Mode0 = 1;
                                //    }

                                //}
                                //else
                                //{
                                //    m_Result_Job_Mode0 = 1;
                                //}
                            }
                            //MessageBox.Show("ProbeThread NG LOG");
                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }

                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            //MessageBox.Show("ProbeThread SEND Message");
                            // String filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_frame.Clone(), t_Judge);
                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");
                            //MessageBox.Show("ProbeThread ADD LOG");
                            Run_SW[Cam_Num].Stop();
                            LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                            m_Probe_Job_Mode0 = 0;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void ProbeThreadProc1()
        {
            int Cam_Num = 1;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Probe_Job_Mode1 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Probe_Job_Mode1 == 1)
                {
                    Run_SW[Cam_Num].Reset();
                    Run_SW[Cam_Num].Start();

                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            //LVApp.Instance().m_Config.Set_Parameters();
                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            double t_v = 0;
                            //ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[0][3] = t_v;
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] = t_v.ToString();

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            //bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                //t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode1 == 0)
                            {
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;
                                Result_framebuffer[Cam_Num].Add(new Bitmap(640, 480));
                                //Result_Image1[Capture_Count[Cam_Num]] = null;
                                //Result_Image1[Capture_Count[Cam_Num]] = new Bitmap(640, 480);

                                //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                //if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //{
                                //    if (Judge != 40)
                                //    {
                                //        m_Result_Job_Mode1 = 1;
                                //    }
                                //    if (t_cam_setting_view_mode)
                                //    {
                                //        m_Result_Job_Mode1 = 1;
                                //    }

                                //}
                                //else
                                //{
                                //    m_Result_Job_Mode1 = 1;
                                //}
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }

                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            // String filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_frame.Clone(), t_Judge);
                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");

                            Run_SW[Cam_Num].Stop();
                            LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                            m_Probe_Job_Mode1 = 0;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void ProbeThreadProc2()
        {
            int Cam_Num = 2;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Probe_Job_Mode2 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Probe_Job_Mode2 == 1)
                {
                    Run_SW[Cam_Num].Reset();
                    Run_SW[Cam_Num].Start();

                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            //LVApp.Instance().m_Config.Set_Parameters();
                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            double t_v = 0;
                            //ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[0][3] = t_v;
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] = t_v.ToString();

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            //bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                //t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode2 == 0)
                            {
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;
                                Result_framebuffer[Cam_Num].Add(new Bitmap(640, 480));

                                //Result_Image2[Capture_Count[Cam_Num]] = null;
                                //Result_Image2[Capture_Count[Cam_Num]] = new Bitmap(640, 480);

                                //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                //if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //{
                                //    if (Judge != 40)
                                //    {
                                //        m_Result_Job_Mode2 = 1;
                                //    }
                                //    if (t_cam_setting_view_mode)
                                //    {
                                //        m_Result_Job_Mode2 = 1;
                                //    }

                                //}
                                //else
                                //{
                                //    m_Result_Job_Mode2 = 1;
                                //}
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }

                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            // String filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_frame.Clone(), t_Judge);
                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");
                            Run_SW[Cam_Num].Stop();
                            LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                            m_Probe_Job_Mode2 = 0;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void ProbeThreadProc3()
        {
            int Cam_Num = 3;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Probe_Job_Mode3 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Probe_Job_Mode3 == 1)
                {
                    Run_SW[Cam_Num].Reset();
                    Run_SW[Cam_Num].Start();

                    if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            //LVApp.Instance().m_Config.Set_Parameters();
                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            double t_v = 0;
                            //ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] = "";
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 1000; // Prove#1 위치
                            if (t_v < 0)
                            {
                                Thread.Sleep(20);
                                t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 1000; // Prove#1 위치
                            }
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[0][3] = t_v;
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] += t_v.ToString();
                            Thread.Sleep(20);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + ((Cam_Num + 1) * 2).ToString(), 2) / 1000; // Prove#1 위치
                            if (t_v < 0)
                            {
                                Thread.Sleep(20);
                                t_v = ctr_PLC1.PLC_D_READ("DW53" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + ((Cam_Num + 1) * 2).ToString(), 2) / 1000; // Prove#1 위치
                            }
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[1][3] = t_v;
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] += ", " + t_v.ToString();
                            Thread.Sleep(20);
                            t_v = ctr_PLC1.PLC_D_READ("DW54" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + "0", 2) / 1000; // Prove#1 위치
                            if (t_v < 0)
                            {
                                Thread.Sleep(20);
                                t_v = ctr_PLC1.PLC_D_READ("DW54" + LVApp.Instance().m_Config.PLC_Station_Num.ToString("0") + "0", 2) / 1000; // Prove#1 위치
                            }

                            LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[2][3] = t_v;
                            LVApp.Instance().m_Config.m_Probe[Cam_Num] += ", " + t_v.ToString();

                            int Judge = LVApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            //bool t_Judge = true;
                            if (Judge != 40)
                            {
                                if (Judge == -1)
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 2;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                                }
                                else
                                {
                                    LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                    LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                }

                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                //t_Judge = false;
                            }
                            else
                            {
                                LVApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + LVApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode3 == 0)
                            {
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;
                                Result_framebuffer[Cam_Num].Add(new Bitmap(640, 480));

                                //Result_Image3[Capture_Count[Cam_Num]] = null;
                                //Result_Image3[Capture_Count[Cam_Num]] = new Bitmap(640, 480);

                                //if (LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (LVApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                //if (LVApp.Instance().m_Config.Diplay_Only_NG)
                                //{
                                //    if (Judge != 40)
                                //    {
                                //        m_Result_Job_Mode3 = 1;
                                //    }
                                //    if (t_cam_setting_view_mode)
                                //    {
                                //        m_Result_Job_Mode3 = 1;
                                //    }

                                //}
                                //else
                                //{
                                //    m_Result_Job_Mode3 = 1;
                                //}
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (LVApp.Instance().m_Config.Inspection_Delay[Cam_Num] > 0)
                            {
                                Thread.Sleep(LVApp.Instance().m_Config.Inspection_Delay[Cam_Num]);
                            }

                            Add_PLC_Tx_Message(Cam_Num, Judge);
                            // String filename = LVApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_frame.Clone(), t_Judge);
                            LVApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");
                            Run_SW[Cam_Num].Stop();
                            LVApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //LVApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                            m_Probe_Job_Mode3 = 0;
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        private void dataGridView_Setting_Value_0_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
            {
                string str = dataGridView_Setting_Value_0.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                double t_v = 0;
                if (double.TryParse(str, out t_v))
                {
                    dataGridView_Setting_Value_0.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = t_v;
                }
                else
                {
                    dataGridView_Setting_Value_0.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("변수값이 숫자가 아닙니다. 다시 입력하세요!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Not numeric value!, Please type again!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        AutoClosingMessageBox.Show("不是数字值！，请再次键入!", "Notice", 3000);
                    }
                    return;
                }
            }

            //dataGridView_Setting_Value_0.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = LVApp.Instance().m_Config.ds_DATA_0;

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
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(0,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
            {
                string str = dataGridView_Setting_Value_1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                double t_v = 0;
                if (double.TryParse(str, out t_v))
                {
                    dataGridView_Setting_Value_1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = t_v;
                }
                else
                {
                    dataGridView_Setting_Value_1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("변수값이 숫자가 아닙니다. 다시 입력하세요!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Not numeric value!, Please type again!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        AutoClosingMessageBox.Show("不是数字值！，请再次键入!", "Notice", 3000);
                    }
                    return;
                }
            }
            //dataGridView_Setting_Value_1.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = LVApp.Instance().m_Config.ds_DATA_1;

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
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(1,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
            {
                string str = dataGridView_Setting_Value_2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                double t_v = 0;
                if (double.TryParse(str, out t_v))
                {
                    dataGridView_Setting_Value_2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = t_v;
                }
                else
                {
                    dataGridView_Setting_Value_2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("변수값이 숫자가 아닙니다. 다시 입력하세요!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Not numeric value!, Please type again!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        AutoClosingMessageBox.Show("不是数字值！，请再次键入!", "Notice", 3000);
                    }
                    return;
                }
            }
            //dataGridView_Setting_Value_2.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = LVApp.Instance().m_Config.ds_DATA_2;

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
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(2,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
            {
                string str = dataGridView_Setting_Value_3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                double t_v = 0;
                if (double.TryParse(str, out t_v))
                {
                    dataGridView_Setting_Value_3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = t_v;
                }
                else
                {
                    dataGridView_Setting_Value_3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("변수값이 숫자가 아닙니다. 다시 입력하세요!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Not numeric value!, Please type again!", "Notice", 3000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        AutoClosingMessageBox.Show("不是数字值！，请再次键入!", "Notice", 3000);
                    }
                    return;
                }
            }
            //dataGridView_Setting_Value_3.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = LVApp.Instance().m_Config.ds_DATA_3;

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
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(3,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change during inspection!", "Notice", 3000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("检查期间无法更改!", "Notice", 3000);
                }
            }
        }

        private void dataGridView_Setting_Value_1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change during inspection!", "Notice", 3000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("检查期间无法更改!", "Notice", 3000);
                }
            }
        }

        private void dataGridView_Setting_Value_2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change during inspection!", "Notice", 3000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("检查期间无法更改!", "Notice", 3000);
                }
            }
        }

        private void dataGridView_Setting_Value_3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change during inspection!", "Notice", 3000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("检查期间无法更改!", "Notice", 3000);
                }
            }
        }

        // Display information about the selected drive.
        public bool Check_HD_available(String folder)
        {
            bool ret = true;
            try
            {
                System.IO.DriveInfo drive = new System.IO.DriveInfo(folder);
                System.IO.DriveInfo a = new System.IO.DriveInfo(drive.Name);

                DirectoryInfo root_dir = new DirectoryInfo(drive.Name);
                if (root_dir.Exists == false)
                {
                    ret = false;
                    return ret;
                }

                long HDPercentageUsed = 100 - (100 * a.AvailableFreeSpace / a.TotalSize);
                if (HDPercentageUsed > 99)
                {
                    ret = false;
                    return ret;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        private void pictureBox_Setting_0_DragEnter(object sender, DragEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
            //foreach (Form form in Application.OpenForms)
            //{
            //    if (form.GetType() == typeof(Frm_DragImageControl))
            //    {
            //        form.Close();
            //    }
            //}
        }

        bool DragDrop_flag = false;
        private void pictureBox_Setting_0_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
                if (pictureBox_Setting_0.Image != null)
                {
                    pictureBox_Setting_0.Image = null;
                }
                pictureBox_Setting_0.BackgroundImage = null;

                Frm_DragImageControl t_Frm_DragImageControl = new Frm_DragImageControl();
                t_Frm_DragImageControl.t_Cam_Num = 0;
                t_Frm_DragImageControl.t_Image_List = ((string[])e.Data.GetData(DataFormats.FileDrop));
                t_Frm_DragImageControl.Update_Image_List();
                System.Drawing.Point loc = pictureBox_Setting_0.PointToScreen(System.Drawing.Point.Empty);
                t_Frm_DragImageControl.Width = pictureBox_Setting_0.Width + 4;
                t_Frm_DragImageControl.Location = new System.Drawing.Point(loc.X + pictureBox_Setting_0.Width - t_Frm_DragImageControl.Width + 2, loc.Y + pictureBox_Setting_0.Height + 1);
                t_Frm_DragImageControl.Show();
            }
            catch
            {
            }
        }

        public void Drag_Image_Inspection(string pic, object sender, EventArgs e)
        {
            try
            {
                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        AutoClosingMessageBox.Show("在线检查期间无法进行手动检查!", "Notice", 2000);
                    }
                    return;
                }
                if (DragDrop_flag)
                {
                    return;
                }

                DragDrop_flag = true;
                // Load bitmap
                ImageInfo imageInfo = null;
                Bitmap t_Image = ImageDecoder.DecodeFromFile(pic, out imageInfo);
                if (t_Image.PixelFormat == PixelFormat.Format32bppRgb)
                {
                    Bitmap tt_Image = ImageDecoder.DecodeFromFile(pic, out imageInfo);
                    t_Image = null;
                    t_Image = ctr_Manual1.ConvertTo24((Bitmap)tt_Image.Clone());
                    tt_Image.Dispose();
                }
                ctr_Manual1.pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();
                //ctr_Manual1.propertyGrid1.SelectedObject = imageInfo;
                //ctr_Manual1.propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24 || imageInfo.BitsPerPixel == 32)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[ctr_Manual1.m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = ctr_Manual1.BmpToArray(t_Image);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = ctr_Manual1.BmpToArray(grayImage);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = ctr_Manual1.BmpToArray(t_Image);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    if (ctr_Manual1.m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
                //if (ctr_Manual1.pictureBox_Manual.Image != null)
                {
                    ctr_Manual1.button_Manual_Inspection_Click(sender, e);
                }
                DragDrop_flag = false;
                //GC.Collect();
            }
            catch
            {
                DragDrop_flag = false;
            }
        }

        private void pictureBox_Setting_1_DragEnter(object sender, DragEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
            //foreach (Form form in Application.OpenForms)
            //{
            //    if (form.GetType() == typeof(Frm_DragImageControl))
            //    {
            //        form.Close();
            //    }
            //}
        }

        private void pictureBox_Setting_1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
                if (pictureBox_Setting_1.Image != null)
                {
                    pictureBox_Setting_1.Image = null;
                }
                pictureBox_Setting_1.BackgroundImage = null;

                Frm_DragImageControl t_Frm_DragImageControl = new Frm_DragImageControl();
                t_Frm_DragImageControl.t_Cam_Num = 1;
                t_Frm_DragImageControl.t_Image_List = ((string[])e.Data.GetData(DataFormats.FileDrop));
                t_Frm_DragImageControl.Update_Image_List();
                t_Frm_DragImageControl.Width = pictureBox_Setting_1.Width + 4;
                System.Drawing.Point loc = pictureBox_Setting_1.PointToScreen(System.Drawing.Point.Empty);
                t_Frm_DragImageControl.Location = new System.Drawing.Point(loc.X + pictureBox_Setting_1.Width - t_Frm_DragImageControl.Width + 2, loc.Y + pictureBox_Setting_1.Height + 1);
                t_Frm_DragImageControl.Show();
            }
            catch
            {
            }
        }

        private void pictureBox_Setting_2_DragEnter(object sender, DragEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
            //foreach (Form form in Application.OpenForms)
            //{
            //    if (form.GetType() == typeof(Frm_DragImageControl))
            //    {
            //        form.Close();
            //    }
            //}
        }

        private void pictureBox_Setting_2_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
                if (pictureBox_Setting_2.Image != null)
                {
                    pictureBox_Setting_2.Image = null;
                }
                pictureBox_Setting_2.BackgroundImage = null;

                Frm_DragImageControl t_Frm_DragImageControl = new Frm_DragImageControl();
                t_Frm_DragImageControl.t_Cam_Num = 2;
                t_Frm_DragImageControl.t_Image_List = ((string[])e.Data.GetData(DataFormats.FileDrop));
                t_Frm_DragImageControl.Update_Image_List();
                t_Frm_DragImageControl.Width = pictureBox_Setting_2.Width + 4;
                System.Drawing.Point loc = pictureBox_Setting_2.PointToScreen(System.Drawing.Point.Empty);
                t_Frm_DragImageControl.Location = new System.Drawing.Point(loc.X + pictureBox_Setting_2.Width - t_Frm_DragImageControl.Width + 2, loc.Y + pictureBox_Setting_2.Height + 1);
                t_Frm_DragImageControl.Show();
            }
            catch
            {
            }
        }

        private void pictureBox_Setting_3_DragEnter(object sender, DragEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
            //foreach (Form form in Application.OpenForms)
            //{
            //    if (form.GetType() == typeof(Frm_DragImageControl))
            //    {
            //        form.Close();
            //    }
            //}
        }

        private void pictureBox_Setting_3_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
                if (pictureBox_Setting_3.Image != null)
                {
                    pictureBox_Setting_3.Image = null;
                }
                pictureBox_Setting_3.BackgroundImage = null;

                Frm_DragImageControl t_Frm_DragImageControl = new Frm_DragImageControl();
                t_Frm_DragImageControl.t_Cam_Num = 3;
                t_Frm_DragImageControl.t_Image_List = ((string[])e.Data.GetData(DataFormats.FileDrop));
                t_Frm_DragImageControl.Update_Image_List();
                t_Frm_DragImageControl.Width = pictureBox_Setting_3.Width + 4;
                System.Drawing.Point loc = pictureBox_Setting_3.PointToScreen(System.Drawing.Point.Empty);
                t_Frm_DragImageControl.Location = new System.Drawing.Point(loc.X + pictureBox_Setting_3.Width - t_Frm_DragImageControl.Width + 2, loc.Y + pictureBox_Setting_3.Height + 1);
                t_Frm_DragImageControl.Show();
            }
            catch
            {
            }
        }

        private void textBox_LOT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Properties.Settings.Default.Lot_No = textBox_LOT.Text;
                LVApp.Instance().m_Config.m_lot_str = textBox_LOT.Text;
            }
        }

        private void neoTabWindow_MAIN_SelectedIndexChanging(object sender, NeoTabControlLibrary.SelectedIndexChangingEventArgs e)
        {
            if (!m_Start_Button_Check)
            {
                if (m_Language == 0)
                {
                    add_Log("프로그램 로딩중입니다!");
                }
                else
                {
                    add_Log("Wait for loading!");
                }
                e.Cancel = true;
                return;
            }
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (e.TabPageIndex == 1)
                {
                    if (m_Language == 0)
                    {
                        add_Log("검사중에는 검사설정을 할 수 없습니다!");
                    }
                    else
                    {
                        add_Log("Can't setup during inspection!");
                    }
                    e.Cancel = true;
                    return;
                }
            }
            if (LVApp.Instance().m_Config.Disable_Menu && LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (m_Language == 0)
                {
                    add_Log("검사중 이동 금지 옵션이 체크되어 있습니다!");
                }
                else
                {
                    add_Log("Now moving prevent option checked!");
                }
                e.Cancel = true;
                return;
            }

        }

        public int t_Main_View = 0;
        private void button_Main_View_Click(object sender, EventArgs e)
        {
            if (t_Main_View == 0)
            {
                splitContainer_AUTO_main.Panel1.Controls.Clear();
                splitContainer_AUTO_main.Panel1.Controls.Add(ctr_Yield1);
                ctr_Yield1.Dock = DockStyle.Fill;
                ctr_Yield1.Update_UI();
                t_Main_View = 1;
            }
            else if (t_Main_View == 1)
            {
                splitContainer_AUTO_main.Panel1.Controls.Clear();
                splitContainer_AUTO_main.Panel1.Controls.Add(ctr_Display_1);
                ctr_Display_1.Dock = DockStyle.Fill;
                ctr_Display_1.Update_Display();
                t_Main_View = 0;
            }
        }

        private void dataGridView_AUTO_STATUS_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView_AUTO_STATUS.ClearSelection();
        }

        private void dataGridView_AUTO_COUNT_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView_AUTO_COUNT.ClearSelection();
        }

        Bitmap t_Cam0_Simul; bool t_Cam0_Simul_check = false;
        private void timer_Cam0_Simulation_Tick(object sender, EventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (!ctr_Camera_Setting1.Force_USE.Checked)
                {
                    if (!t_Cam0_Simul_check)
                    {
                        t_Cam0_Simul = (Bitmap)ctr_ROI1.pictureBox_Image.Image.Clone();
                        t_Cam0_Simul_check = true;
                    }
                    ctrCam1.m_bitmap = t_Cam0_Simul;
                    ctrCam1_GrabComplete(sender, e);
                }
                else
                {
                    //timer_Cam[0].Enabled = false;
                }
            }
        }

        Bitmap t_Cam1_Simul; bool t_Cam1_Simul_check = false;
        private void timer_Cam1_Simulation_Tick(object sender, EventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (!ctr_Camera_Setting2.Force_USE.Checked)
                {
                    if (!t_Cam1_Simul_check)
                    {
                        t_Cam1_Simul = (Bitmap)ctr_ROI2.pictureBox_Image.Image.Clone();
                        t_Cam1_Simul_check = true;
                    }
                    ctrCam2.m_bitmap = t_Cam1_Simul;
                    ctrCam2_GrabComplete(sender, e);
                }
                else
                {
                    //timer_Cam[1].Enabled = false;
                }
            }
        }

        Bitmap t_Cam2_Simul; bool t_Cam2_Simul_check = false;
        private void timer_Cam2_Simulation_Tick(object sender, EventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (!ctr_Camera_Setting3.Force_USE.Checked)
                {
                    if (!t_Cam2_Simul_check)
                    {
                        t_Cam2_Simul = (Bitmap)ctr_ROI3.pictureBox_Image.Image.Clone();
                        t_Cam2_Simul_check = true;
                    }
                    ctrCam3.m_bitmap = t_Cam2_Simul;
                    ctrCam3_GrabComplete(sender, e);
                }
                else
                {
                    //timer_Cam[2].Enabled = false;
                }
            }
        }

        Bitmap t_Cam3_Simul; bool t_Cam3_Simul_check = false;
        private void timer_Cam3_Simulation_Tick(object sender, EventArgs e)
        {
            //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (!ctr_Camera_Setting4.Force_USE.Checked)
                {
                    if (!t_Cam3_Simul_check)
                    {
                        t_Cam3_Simul = (Bitmap)ctr_ROI4.pictureBox_Image.Image.Clone();
                        t_Cam3_Simul_check = true;
                    }
                    ctrCam4.m_bitmap = t_Cam3_Simul;
                    ctrCam4_GrabComplete(sender, e);
                }
                else
                {
                    //timer_Cam[3].Enabled = false;
                }
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private void button_STATUS_Click(object sender, EventArgs e)
        {
            Process[] arrayProgram = Process.GetProcesses();
            int cnt = 0;
            for (int i = 0; i < arrayProgram.Length; i++)
            {
                if (arrayProgram[i].ProcessName.Equals("Data_Display"))
                {
                    SetForegroundWindow(arrayProgram[i].MainWindowHandle);
                    cnt++;
                }
            }
            if (cnt > 0)
            {
                for (int i = 0; i < arrayProgram.Length; i++)
                {
                    if (arrayProgram[i].ProcessName.Equals("Data_Display"))
                    {
                        arrayProgram[i].Kill();
                    }
                }
            }
            if (cnt == 0)
            {
                Process flash = new Process();
                flash.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

                flash.StartInfo.FileName = LVApp.Instance().excute_path + "\\Data_Display.exe";
                flash.Start();
                Thread.Sleep(500);
                System.Drawing.Point locationOnForm = splitContainer17.Panel1.FindForm().PointToClient(splitContainer17.Panel1.Parent.PointToScreen(splitContainer17.Panel1.Location));
                //MoveWindow(flash.MainWindowHandle, Properties.Settings.Default.DP_Location.X, Properties.Settings.Default.DP_Location.Y, 215, 159, true);
                MoveWindow(flash.MainWindowHandle, locationOnForm.X, locationOnForm.Y, 180, splitContainer17.Panel1.Height, true);

                MoveWindow(flash.MainWindowHandle, locationOnForm.X, locationOnForm.Y, 180, splitContainer17.Panel1.Height, true);
            }
        }
    }
}
