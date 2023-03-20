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
using AForge.Imaging.IPPrototyper;
using AForge.Imaging.Formats;
using System.Collections;

namespace IPSST_Inspection_System.GUI
{
    public partial class Frm_Main : XCoolForm.XCoolForm
    {
        public ImPro_Library_Clr.ClassClr m_ImProClr_Class = new ImPro_Library_Clr.ClassClr(); // C++ source CLR 연결;
        //public ImPro_Library_Clr.ClassClr m_ImProClr_Class0 = new ImPro_Library_Clr.ClassClr(); // C++ source CLR 연결;
        //public ImPro_Library_Clr.ClassClr m_ImProClr_Class1 = new ImPro_Library_Clr.ClassClr(); // C++ source CLR 연결;
        //public ImPro_Library_Clr.ClassClr m_ImProClr_Class2 = new ImPro_Library_Clr.ClassClr(); // C++ source CLR 연결;
        public bool Force_close = false;
        private bool t_setting_view_mode = false;
        private bool t_cam_setting_view_mode = false;
        private DongleKey t_DongleKey = new DongleKey();

        Thread[] threads = new Thread[4];
        Thread[] Probe_threads = new Thread[4];
        Thread[] Viewthreads = new Thread[4];
        Thread[] Missedthreads = new Thread[4];


        Thread ImageSavethread = null;
        public bool m_ImageSavethread_Check = false;

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
        public int m_Missed_Job_Mode0 = 0;
        public int m_Missed_Job_Mode1 = 0;
        public int m_Missed_Job_Mode2 = 0;
        public int m_Missed_Job_Mode3 = 0;

        private Bitmap[] Capture_Image0 = new Bitmap[4];
        private Bitmap[] Capture_Image1 = new Bitmap[4];
        private Bitmap[] Capture_Image2 = new Bitmap[4];
        private Bitmap[] Capture_Image3 = new Bitmap[4];
        private Bitmap[] Result_Image0 = new Bitmap[4];
        private Bitmap[] Result_Image1 = new Bitmap[4];
        private Bitmap[] Result_Image2 = new Bitmap[4];
        private Bitmap[] Result_Image3 = new Bitmap[4];
        private Bitmap[] Missed_Image0 = new Bitmap[4];
        private Bitmap[] Missed_Image1 = new Bitmap[4];
        private Bitmap[] Missed_Image2 = new Bitmap[4];
        private Bitmap[] Missed_Image3 = new Bitmap[4];

        public Queue[] Capture_framebuffer = new Queue[4];
        public Queue[] Result_framebuffer = new Queue[4];

        public int[] Capture_Count = new int[4];
        public int[] Missed_Count = new int[4];

        public bool m_Start_Check = false;

        public bool m_Last_Start_Loading = false;

        public Frm_Main()
        {
            InitializeComponent();
            IPSSTApp.Instance().m_mainform = this;

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
                    neoTabWindow_MAIN.TabPages[0].Text = "메인화면";
                    neoTabWindow_MAIN.TabPages[1].Text = "검사설정";
                    neoTabWindow_MAIN.TabPages[2].Text = "장비설정";
                    neoTabWindow_MAIN.TabPages[3].Text = "로그";
                    neoTabWindow_MAIN.TabPages[4].Text = "수동검사";
                    neoTabWindow_MAIN.TabPages[5].Text = "모델";
                    neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                    neoTabWindow_MAIN.TabPages[7].Text = "도움말";
                    neoTabWindow_INSP_SETTING.TabPages[0].Text = "판정 설정";
                    neoTabWindow_INSP_SETTING.TabPages[1].Text = "변수 설정";
                    neoTabWindow_INSP_SETTING.TabPages[2].Text = "관리자 설정";
                    neoTabWindow_EQUIP_SETTING.TabPages[0].Text = "카메라 설정";
                    neoTabWindow_EQUIP_SETTING.TabPages[1].Text = "통신 설정";
                    neoTabWindow_LOG.TabPages[0].Text = "검사 결과값";
                    neoTabWindow_LOG.TabPages[1].Text = "시스템 로그";
                    neoTabWindow_LOG.TabPages[3].Text = "NG 로그";
                    neoTabWindow_LOG.TabPages[4].Text = "로그 설정";
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

                    neoTabWindow_MAIN.TabPages[0].Text = "Main";
                    neoTabWindow_MAIN.TabPages[1].Text = "Parameters";
                    neoTabWindow_MAIN.TabPages[2].Text = "System";
                    neoTabWindow_MAIN.TabPages[3].Text = "Log";
                    neoTabWindow_MAIN.TabPages[4].Text = "Test";
                    neoTabWindow_MAIN.TabPages[5].Text = "Model";
                    neoTabWindow_MAIN.TabPages[6].Text = "Login";
                    neoTabWindow_MAIN.TabPages[7].Text = "Help";
                    neoTabWindow_INSP_SETTING.TabPages[0].Text = "Judgement";
                    neoTabWindow_INSP_SETTING.TabPages[1].Text = "ROI parameter";
                    neoTabWindow_INSP_SETTING.TabPages[2].Text = "Admin setting";
                    neoTabWindow_EQUIP_SETTING.TabPages[0].Text = "Camera setting";
                    neoTabWindow_EQUIP_SETTING.TabPages[1].Text = "PLC setting";
                    neoTabWindow_LOG.TabPages[0].Text = "Result data";
                    neoTabWindow_LOG.TabPages[1].Text = "system log";
                    neoTabWindow_LOG.TabPages[3].Text = "NG log";
                    neoTabWindow_LOG.TabPages[4].Text = "Log setting";
                }
                m_Language = value;
            }
        }

        private void Frm_Main_Load(object sender, EventArgs e)
        {
            //neoTabWindow_MAIN.Enabled = false;
            //t_DongleKey.Check_License();
            Program_Start_Check();                                              // 프로그램 시작시 중복 체크
            DebugLogger.Instance().LoggerStatusEvent += new LoggerStatusHandler(LoggerStatusEvent);
            DebugLogger.Instance().LogRecord("Program Start / Ready.");

            AutoClosingMessageBox.Show("Program is loading.", "LOAD", 3000);

            if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                add_Log("프로그램 시작 및 준비가 되었습니다.");
            }
            else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                add_Log("Program Start / Ready.");
            }

            IPSSTApp.Instance().m_Config.m_SetLanguage = Properties.Settings.Default.Language;
            //IPSSTApp.Instance().m_Config.ds_DATA_4 = new DataSet();
            IPSSTApp.Instance().m_Config.ds_LOG = new DataSet();
            GUI_Initialize();                                                   // UI 화면 정보 및 디자인 갱신
            IPSSTApp.Instance().m_Config.Initial_DataBase();                    // DataBase 초기화
            ctr_Model1.read_model_list();                                       // 모델 리스트 불러오기

            Thread.Sleep(100);
            IPSSTApp.Instance().m_Config.Load_Judge_Data();                     // 저장 변수 불러오기
            //IPSSTApp.Instance().m_mainform.ctr_Log1.Refresh_Log_Data();
            //IPSSTApp.Instance().m_Config.Initialize_Data_Log(0);
            //IPSSTApp.Instance().m_Config.Initialize_Data_Log(1);
            //IPSSTApp.Instance().m_Config.Initialize_Data_Log(2);
            //IPSSTApp.Instance().m_Config.Initialize_Data_Log(3);

            Thread.Sleep(100);
            Camera_Initialize();                                                // 카메라 초기화
            Thread.Sleep(100);

            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
            {
                ctr_ROI4.Cam_Num = 3;
                ctr_ROI4.ROI_Idx = 0;
                IPSSTApp.Instance().m_Config.ROI_Cam_Num = 3;
                ctr_ROI4.Initialize_ROI();
                //ctr_ROI4.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                ctr_ROI4.button_LOAD_Click(sender, e);
                ctr_Log1.checkBox_CAM3.Visible = true;
                ctr_ROI4.load_check = false;
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = -1;
                ctr_ROI4.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_Config.Realtime_Running_Check[3] = false;
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
            {
                ctr_ROI3.Cam_Num = 2;
                ctr_ROI3.ROI_Idx = 0;
                IPSSTApp.Instance().m_Config.ROI_Cam_Num = 2;
                ctr_ROI3.Initialize_ROI();
                //ctr_ROI3.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                ctr_ROI3.button_LOAD_Click(sender, e);
                ctr_Log1.checkBox_CAM2.Visible = true;
                ctr_ROI3.load_check = false;
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = -1;
                ctr_ROI3.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_Config.Realtime_Running_Check[2] = false;
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
            {
                ctr_ROI2.Cam_Num = 1;
                ctr_ROI2.ROI_Idx = 0;
                IPSSTApp.Instance().m_Config.ROI_Cam_Num = 1;
                ctr_ROI2.Initialize_ROI();
                //ctr_ROI2.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                ctr_ROI2.button_LOAD_Click(sender, e);
                ctr_Log1.checkBox_CAM1.Visible = true;
                ctr_ROI2.load_check = false;
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = -1;
                ctr_ROI2.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_Config.Realtime_Running_Check[1] = false;
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
            {
                ctr_ROI1.Cam_Num = 0;
                ctr_ROI1.ROI_Idx = 0;
                IPSSTApp.Instance().m_Config.ROI_Cam_Num = 0;
                ctr_ROI1.Initialize_ROI();
                //ctr_ROI1.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                ctr_ROI1.button_LOAD_Click(sender, e);
                ctr_Log1.checkBox_CAM0.Visible = true;
                ctr_ROI1.load_check = false;
                IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = -1;
                ctr_ROI1.listBox1.SelectedIndex = 0;
                IPSSTApp.Instance().m_Config.Realtime_Running_Check[0] = false;
            }

            //IPSSTApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);
            //ctr_Admin_Param1.button_LOAD_Click(sender, e);

            IPSSTApp.Instance().m_Config.ROI_Cam_Num = 0;
            if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                add_Log("검사를 시작하시려면 우상단 [검사 대기] 버튼을 눌러주세요!");
            }
            else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                add_Log("If you want to start inspection, click START button.");
            }
            IPSSTApp.Instance().t_QuickMenu.Top = 7;
            IPSSTApp.Instance().t_QuickMenu.Left = 1280 - 271 - 126;
            IPSSTApp.Instance().t_QuickMenu.Show();

            System.Drawing.Point location = ctr_ROI1.PointToScreen(System.Drawing.Point.Empty);
            IPSSTApp.Instance().m_help.Location = location;
            IPSSTApp.Instance().m_help.webBrowser1.Navigate(IPSSTApp.Instance().excute_path + "\\Help\\Index.html");

            IPSSTApp.Instance().m_Ctr_Mysql.DB_connect();
            IPSSTApp.Instance().m_Ctr_Mysql.DB_Create();
            IPSSTApp.Instance().m_Config.ds_STATUS.Tables[2].Rows[0][1] = IPSSTApp.Instance().m_Config.m_Model_Name;
            IPSSTApp.Instance().m_Ctr_Mysql.DB_Operating(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[2], "Information");
            IPSSTApp.Instance().m_Ctr_Mysql.DB_Operating(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0], "ProgramCheck");
            Thread.Sleep(1000);

            ctr_Model1.cmdLoad_Click(sender, e);
            frmSplash splash = new frmSplash();                                 // 로딩화면
            splash.Show();

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
                Result_Image0[i] = new Bitmap(640, 480);
                Capture_Image0[i] = new Bitmap(640, 480);
                Result_Image1[i] = new Bitmap(640, 480);
                Capture_Image1[i] = new Bitmap(640, 480);
                Result_Image2[i] = new Bitmap(640, 480);
                Capture_Image2[i] = new Bitmap(640, 480);
                Result_Image3[i] = new Bitmap(640, 480);
                Capture_Image3[i] = new Bitmap(640, 480);
                Capture_framebuffer[i] = new Queue();
                Result_framebuffer[i] = new Queue();
            }

            ctr_Log1.Refresh_Log_Data();
            IPSSTApp.Instance().m_Config.ds_LOG.Tables.Clear();
            IPSSTApp.Instance().m_Config.ds_LOG.Clear();
            for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
            {
                IPSSTApp.Instance().m_Config.m_Error_Flag[i] = -1;
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] = 0;
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] = 0;
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] = 0;
                IPSSTApp.Instance().m_Config.Initialize_Data_Log(i);
                //IPSSTApp.Instance().m_Config.m_Log_Data_Cnt[i] = 0;
                IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[i] = 0;
            }

            IPSSTApp.Instance().m_Config.t_Create_Save_Folders_oldtime = DateTime.Now;


            bool t_space_check = true;
            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
            {
                t_space_check = Check_HD_available(IPSSTApp.Instance().excute_path);
            }
            else
            {
                t_space_check = Check_HD_available(IPSSTApp.Instance().m_Config.m_Log_Save_Folder);
            }
            if (t_space_check)
            {
                IPSSTApp.Instance().SAVE_IMAGE_List[0].Clear();
                IPSSTApp.Instance().SAVE_IMAGE_List[1].Clear();
                IPSSTApp.Instance().SAVE_IMAGE_List[2].Clear();
                IPSSTApp.Instance().SAVE_IMAGE_List[3].Clear();
                ImageSavethread = new Thread(ImageSavethread_Proc);
                m_ImageSavethread_Check = true;
                ImageSavethread.Start();
            }

            Inspection_Thread_Start();
            Viewthreads[0] = new Thread(ResultProc0);
            Viewthreads[1] = new Thread(ResultProc1);
            Viewthreads[2] = new Thread(ResultProc2);
            Viewthreads[3] = new Thread(ResultProc3);
            m_Result_Job_Mode0 = 0;
            m_Result_Job_Mode1 = 0;
            m_Result_Job_Mode2 = 0;
            m_Result_Job_Mode3 = 0;
            Viewthreads[0].Start();
            Viewthreads[1].Start();
            Viewthreads[2].Start();
            Viewthreads[3].Start();
            splitContainer11.SplitterDistance = splitContainer16.SplitterDistance = splitContainer19.SplitterDistance = splitContainer21.SplitterDistance = 360;
        }

        public void ImageSavethread_Proc()
        {
            while (m_ImageSavethread_Check)
            {
                Thread.Sleep(1);
                if (IPSSTApp.Instance().SAVE_IMAGE_List[0].Count > 0)
                {
                    Save_Image_List(0);
                }
                if (IPSSTApp.Instance().SAVE_IMAGE_List[1].Count > 0)
                {
                    Save_Image_List(1);
                }
                if (IPSSTApp.Instance().SAVE_IMAGE_List[2].Count > 0)
                {
                    Save_Image_List(2);
                }
                if (IPSSTApp.Instance().SAVE_IMAGE_List[3].Count > 0)
                {
                    Save_Image_List(3);
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
                    string fn = "";
                    DateTime dt = DateTime.Now;
                    // MM_DD_YYYY_HH_MM_SS.LOG
                    fn += dt.Year.ToString("0000");
                    fn += "_" + dt.Month.ToString("00");
                    fn += "_" + dt.Day.ToString("00") + "";

                    //if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                    //{
                    //    DirectoryInfo dir = new DirectoryInfo(IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[0]._Cam_num.ToString());
                    //    // 폴더가 존재하지 않으면
                    //    if (dir.Exists == false)
                    //    {
                    //        return;
                    //    }
                    //}
                    //else
                    //{
                    //    DirectoryInfo dir = new DirectoryInfo(IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[0]._Cam_num.ToString());
                    //    // 폴더가 존재하지 않으면
                    //    if (dir.Exists == false)
                    //    {
                    //        // 새로 생성합니다.
                    //        return;
                    //    }
                    //}

                    if (IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._OK_NG_Flag && (IPSSTApp.Instance().m_Config.m_Cam_Log_Method == 0 || IPSSTApp.Instance().m_Config.m_Cam_Log_Method == 2)) // OK 저장
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 0)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 1)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 2)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 3)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\OK\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                    else if (!IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._OK_NG_Flag && (IPSSTApp.Instance().m_Config.m_Cam_Log_Method == 1 || IPSSTApp.Instance().m_Config.m_Cam_Log_Method == 2)) // NG 저장
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 0)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".bmp";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 1)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".jpg";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 2)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Log_Format == 3)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                string filename = IPSSTApp.Instance().excute_path + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                string filename = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + fn + "\\CAM" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "\\NG\\#" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Cam_num.ToString() + "_" + IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Filename + ".png";
                                IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num][0]._Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                    //Thread.Sleep(5);
                    IPSSTApp.Instance().SAVE_IMAGE_List[Cam_num].RemoveAt(0);
                }
                //if (IPSSTApp.Instance().SAVE_IMAGE_List.Count > 4)
                //{
                //    IPSSTApp.Instance().SAVE_IMAGE_List.RemoveAt(0);
                //}
                //this.Invoke(_dt); 
            }
            catch
            {
                add_Log("Image save error! 저장중지");
                m_ImageSavethread_Check = false;
                Thread.Sleep(10);
				if (ImageSavethread != null)
				{
					ImageSavethread.Abort();
					ImageSavethread = null;
				}

                IPSSTApp.Instance().SAVE_IMAGE_List[0].Clear();
                IPSSTApp.Instance().SAVE_IMAGE_List[1].Clear();
                IPSSTApp.Instance().SAVE_IMAGE_List[2].Clear();
                IPSSTApp.Instance().SAVE_IMAGE_List[3].Clear();
                bool t_space_check = true;
                if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                {
                    t_space_check = Check_HD_available(IPSSTApp.Instance().excute_path);
                }
                else
                {
                    t_space_check = Check_HD_available(IPSSTApp.Instance().m_Config.m_Log_Save_Folder);
                }
                if (!t_space_check)
                {
                    add_Log("이미지 저장공간 부족!");
                }
                else
                {
                    IPSSTApp.Instance().m_Config.Create_Save_Folders();
                }
                //Thread.Sleep(10);
                //ImageSavethread = null;
                //ImageSavethread = new Thread(ImageSavethread_Proc);
                //m_ImageSavethread_Check = true;
                //ImageSavethread.Start();
            }
        }

        List<Thread> Over_threads = new List<Thread>();

        public static void CPUKill(object cpuUsage)
        {
            System.Threading.Tasks.Parallel.For(0, 1, new Action<int>((int i) =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    if (watch.ElapsedMilliseconds > (int)cpuUsage)
                    {
                        Thread.Sleep(100 - (int)cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            }));

        }

        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Force_close)
            {
                e.Cancel = false;
            }
            else
            {
                if (MessageBox.Show("Do you want to exit?", " Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                        //foreach (var t in Over_threads)
                        //{
                        //    t.Abort();
                        //}

                        //IPSSTApp.Instance().t_QuickMenu.Hide();
                        Force_close = true;
                        //IPSSTApp.Instance().t_QuickMenu.Close();

                        DebugLogger.Instance().LogRecord("Closing the program!");

                        //Properties.Settings.Default.Split_dist = ctr_Display_1.splitContainer1.SplitterDistance;
                        //Properties.Settings.Default.Save();

                        Inspection_Thread_Stop();

                        IPSSTApp.Instance().m_Config.CSV_Logfile_Terminate();

                        for (int i = 0; i < 4; i++)
                        {
                            m_Threads_Check[i] = false;
                            //threads[i].Abort();
                            if (Viewthreads[i] != null)
                            {
                                Viewthreads[i].Abort();
                            }
                            //Probe_threads[i].Abort();
                        }
                        if (m_ImageSavethread_Check)
                        {
                            m_ImageSavethread_Check = false;
                            if (ImageSavethread != null)
                            {
                                ImageSavethread.Abort();
                            }
                        }

                        if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {
                            button_INSPECTION_Click(sender, e);
                        }
                        if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                        {
                            ctr_Camera_Setting1.toolStripButtonDisconnect_Click(sender, e);
                        }
                        if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                        {
                            ctr_Camera_Setting2.toolStripButtonDisconnect_Click(sender, e);
                        }
                        if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                        {
                            ctr_Camera_Setting3.toolStripButtonDisconnect_Click(sender, e);
                        }
                        if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                        {
                            ctr_Camera_Setting4.toolStripButtonDisconnect_Click(sender, e);
                        }



                        //ctr_Camera_Setting5.toolStripButtonDisconnect_Click(sender, e);
                        //ctr_Camera_Setting6.toolStripButtonDisconnect_Click(sender, e);
                        //ctr_Camera_Setting7.toolStripButtonDisconnect_Click(sender, e);
                        //ctr_Camera_Setting8.toolStripButtonDisconnect_Click(sender, e);

                        Properties.Settings.Default.Save();

                        if (IPSSTApp.Instance().m_Config.m_Model_Name != "")
                        {
                            //ctr_Model1.cmdSave_Click(sender, e);
                            //if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                            //{
                            //    ctr_Camera_Setting1.toolStripButton_SAVE_Click(sender, e);
                            //    IPSSTApp.Instance().m_Config.ROI_Cam_Num = 0;
                            //    ctr_ROI1.button_SAVE_Click(sender, e);
                            //}
                            //if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                            //{
                            //    ctr_Camera_Setting2.toolStripButton_SAVE_Click(sender, e);
                            //    IPSSTApp.Instance().m_Config.ROI_Cam_Num = 1;
                            //    ctr_ROI2.button_SAVE_Click(sender, e);
                            //}
                            //if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                            //{
                            //    ctr_Camera_Setting3.toolStripButton_SAVE_Click(sender, e);
                            //    IPSSTApp.Instance().m_Config.ROI_Cam_Num = 2;
                            //    ctr_ROI3.button_SAVE_Click(sender, e);
                            //}
                            //if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                            //{
                            //    ctr_Camera_Setting4.toolStripButton_SAVE_Click(sender, e);
                            //    IPSSTApp.Instance().m_Config.ROI_Cam_Num = 3;
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

                        IPSSTApp.Instance().m_Ctr_Mysql.DB_disconnect();

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
                if (arrayProgram[i].ProcessName.Equals("Part Appearance Inspector"))
                {
                    cnt++;
                }
            }
            if (cnt > 1)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("1개 이상의 동일한 프로그램이 실행 중입니다.!");
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("Program is running!");
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
                BeginInvoke(new MethodInvoker(delegate() { LoggerStatusEvent(o, status, message); }));
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
            ctr_LogView1.txtLog.Text += message + "\r\n";
            ctr_LogView1.txtLog.SelectionStart = ctr_LogView1.txtLog.Text.Length;
            ctr_LogView1.txtLog.ScrollToCaret();
            ctr_LogView1.txtLog.Refresh();
        }

        private void GUI_Initialize()
        {
            digitalClockCtrl1.SetDigitalColor = SriClocks.DigitalColor.GreenColor;
            this.MenuIcon = IPSST_Inspection_System.Properties.Resources.IPSST.GetThumbnailImage(24, 14, null, IntPtr.Zero);
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(8));

            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(250, ctr_Model1.ctr_History1.SW_Version));// + IPSSTApp.Instance().m_Ctr_Model_Setting.ctr_History1.SW_Version));
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(100, "Ready"));
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(this.Width - 388, "IPSAUTO Co., Ltd. / 422, 2nd Building, 62, Seongseogongdan-ro 11-gil, Dalseo-gu, Daegu, Korea / +82)53-262-2242 [FAX]+82)53-289-0211 / i999@naver.com      "));
            this.StatusBar.EllipticalGlow = false;
            this.StatusBar.BarItems[1].ItemTextAlign = StringAlignment.Center;
            this.StatusBar.BarItems[2].ItemTextAlign = StringAlignment.Center;
            this.StatusBar.BarItems[3].ItemTextAlign = StringAlignment.Far;
            this.StatusBar.BarItems[1].BarItemFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Italic);
            this.StatusBar.BarItems[2].BarItemFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Regular);
            this.StatusBar.BarItems[3].BarItemFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Regular);
            this.TitleBar.TitleBarCaption = "Vision Inspection System";
            this.TitleBar.TitleBarCaptionFont = new System.Drawing.Font("Malgun Gothic", 8.75F, System.Drawing.FontStyle.Bold);

            // 메인 디자인 Theme 로드를 위한 클래스
            XmlThemeLoader xtl = new XmlThemeLoader();
            xtl.ThemeForm = this;
            xtl.ApplyTheme(Path.Combine(Environment.CurrentDirectory, @"Theme.xml"));

            // 상단 아이콘

            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(IPSST_Inspection_System.Properties.Resources._1.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen1"));
            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(IPSST_Inspection_System.Properties.Resources._2.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen2"));
            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(IPSST_Inspection_System.Properties.Resources._3.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen3"));
            //this.IconHolder.HolderButtons.Add(new XCoolForm.XTitleBarIconHolder.XHolderButton(IPSST_Inspection_System.Properties.Resources._4.GetThumbnailImage(20, 20, null, IntPtr.Zero), "Screen4"));

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
                IPSSTApp.Instance().m_Config.m_Error_Flag[i] = -1;
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] = 0;
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] = 0;
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] = 0;
                if (i < 4)
                {
                    IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[i] = 0;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Capture_Count[i] = 0;
            }
            ctr_PLC1.send_Message[0] = new List<string>();
            ctr_PLC1.send_Message[1] = new List<string>();
            ctr_PLC1.send_Message[2] = new List<string>();
            ctr_PLC1.send_Message[3] = new List<string>();
            DebugLogger.Instance().LogRecord("GUI Initialized.");
        }

        private void Frm_Main_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && m_Start_Check)
            {
                IPSSTApp.Instance().t_QuickMenu.Hide();
            }
            else
            {
                if (m_Start_Check)
                {
                    IPSSTApp.Instance().t_QuickMenu.Show();
                }
            }
            if (this.StatusBar.BarItems.Count == 0)
            {
                return;
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 1)
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
            else if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 2)
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
            else if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 3)
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
            else if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 4)
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
            this.StatusBar.BarItems[3].ItemWidth = this.Width - 388;
            //IPSSTApp.Instance().m_mainform.ctr_Camera_Setting2.Force_USE_CheckedChanged(sender, e);
            //IPSSTApp.Instance().m_mainform.ctr_Camera_Setting3.Force_USE_CheckedChanged(sender, e);
            //IPSSTApp.Instance().m_mainform.ctr_Camera_Setting4.Force_USE_CheckedChanged(sender, e);
            if (this.WindowState == FormWindowState.Minimized && m_Start_Check)
            {
                return;
            }
            //IPSSTApp.Instance().m_mainform.ctr_Display_1.Update_Display();
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
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    ctr_Camera_Setting1.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam1.Camera_Name;
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[0] == 1)
                    {
                        IPSSTApp.Instance().m_mainform.ctrCam1.Camera_Line_Mode = true;
                    }
                    ctr_Camera_Setting1.Connect_imageProvider();
                    ctr_Camera_Setting1.toolStripButtonConnect_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    ctr_Camera_Setting2.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam2.Camera_Name;
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[1] == 1)
                    {
                        IPSSTApp.Instance().m_mainform.ctrCam2.Camera_Line_Mode = true;
                    }
                    ctr_Camera_Setting2.Connect_imageProvider();
                    ctr_Camera_Setting2.toolStripButtonConnect_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    ctr_Camera_Setting3.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam3.Camera_Name;
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[2] == 1)
                    {
                        IPSSTApp.Instance().m_mainform.ctrCam3.Camera_Line_Mode = true;
                    }
                    ctr_Camera_Setting3.Connect_imageProvider();
                    ctr_Camera_Setting3.toolStripButtonConnect_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    ctr_Camera_Setting4.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam4.Camera_Name;
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[3] == 1)
                    {
                        IPSSTApp.Instance().m_mainform.ctrCam4.Camera_Line_Mode = true;
                    }
                    ctr_Camera_Setting4.Connect_imageProvider();
                    ctr_Camera_Setting4.toolStripButtonConnect_Click(sender, e);
                }
                //ctr_Camera_Setting5.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam5.Camera_Name;
                //ctr_Camera_Setting5.Connect_imageProvider();
                //ctr_Camera_Setting5.toolStripButtonConnect_Click(sender, e);
                //ctr_Camera_Setting6.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam6.Camera_Name;
                //ctr_Camera_Setting6.Connect_imageProvider();
                //ctr_Camera_Setting6.toolStripButtonConnect_Click(sender, e);
                //ctr_Camera_Setting7.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam7.Camera_Name;
                //ctr_Camera_Setting7.Connect_imageProvider();
                //ctr_Camera_Setting7.toolStripButtonConnect_Click(sender, e);
                //ctr_Camera_Setting8.m_SetCameraName = IPSSTApp.Instance().m_mainform.ctrCam8.Camera_Name;
                //ctr_Camera_Setting8.Connect_imageProvider();
                //ctr_Camera_Setting8.toolStripButtonConnect_Click(sender, e);
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
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
                if (richTextBox_LOG.InvokeRequired)
                {
                    richTextBox_LOG.Invoke((MethodInvoker)delegate
                    {
                        if (richTextBox_LOG.TextLength > 2000)
                        {
                            richTextBox_LOG.ResetText();
                        }
                        string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + str + "\n" + richTextBox_LOG.Text;
                        richTextBox_LOG.Text = display_str;
                    });
                }
                else
                {
                    if (richTextBox_LOG.TextLength > 2000)
                    {
                        richTextBox_LOG.ResetText();
                    }
                    string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + str + "\n" + richTextBox_LOG.Text;
                    richTextBox_LOG.Text = display_str;
                }
            }
            catch
            {
            }
        }

        public Byte[] BmpToArray0(Bitmap value)
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
        public Byte[] BmpToArray1(Bitmap value)
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
        public Byte[] BmpToArray2(Bitmap value)
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
        public Byte[] BmpToArray3(Bitmap value)
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
        public Byte[] BmpToArray4(Bitmap value)
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
            catch
            {
            }
            return new Bitmap(640, 480);
        }

        private void ctrCam1_GrabComplete(object sender, EventArgs e)
        {
            int Cam_Num = 0;
            ctr_Camera_Setting1.Grab_Num++;
            IPSSTApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + Utility.CalculateFrameRate0().ToString();
            if (m_Job_Mode0 == 0)// && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            {
                //m_Job_Mode1 = 2;
                if (ctrCam1.m_bitmap != null)
                {
                    //Capture_Count[Cam_Num]++;
                    //if (Capture_Count[Cam_Num] >= 1)
                    //{
                    //    Capture_Count[Cam_Num] = 0;
                    //}
                    //else
                    //{
                    //}
                    Capture_Image0[Capture_Count[Cam_Num]] = (Bitmap)ctrCam1.m_bitmap.Clone();
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                    {
                        ctrCam2.m_bitmap = (Bitmap)ctrCam1.m_bitmap.Clone();
                        ctrCam2_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                    {
                        ctrCam3.m_bitmap = (Bitmap)ctrCam1.m_bitmap.Clone();
                        ctrCam3_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                    {
                        ctrCam4.m_bitmap = (Bitmap)ctrCam1.m_bitmap.Clone();
                        ctrCam4_GrabComplete(sender, e);
                    }
                    m_Job_Mode0 = 1;

                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
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
                    if (ctr_PLC1.m_threads_Check)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                            }
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                        }
                        //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                        //{
                        //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                        //}
                    }
                    add_Log("CAM0 Grab Error!");
                }
            }
            else
            {
                if (ctr_PLC1.m_threads_Check)
                {
                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                        }
                    }
                    else
                    {
                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                    }
                    //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                    //{
                    //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                    //}
                }
                //if (ctrCam1.m_bitmap != null)
                //{
                //    if (Missed_Count[Cam_Num] >= 9)
                //    {
                //        Missed_Count[Cam_Num] = 0;
                //    }
                //    else
                //    {
                //        Missed_Count[Cam_Num]++;
                //    }
                //    Missed_Image0[Missed_Count[Cam_Num]] = (Bitmap)ctrCam1.m_bitmap.Clone();
                //    m_Missed_Job_Mode0 = 1;
                //}
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                add_Log("CAM0 Miss!");
            }
        }

        private void ctrCam2_GrabComplete(object sender, EventArgs e)
        {
            int Cam_Num = 1;
            ctr_Camera_Setting2.Grab_Num++;
            IPSSTApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + Utility.CalculateFrameRate1().ToString();
            if (m_Job_Mode1 == 0)// && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            {
                //m_Job_Mode2 = 2;
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
                    Capture_Image1[Capture_Count[Cam_Num]] = (Bitmap)ctrCam2.m_bitmap.Clone();
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                    {
                        ctrCam1.m_bitmap = (Bitmap)ctrCam2.m_bitmap.Clone();
                        ctrCam1_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                    {
                        ctrCam3.m_bitmap = (Bitmap)ctrCam2.m_bitmap.Clone();
                        ctrCam3_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                    {
                        ctrCam4.m_bitmap = (Bitmap)ctrCam2.m_bitmap.Clone();
                        ctrCam4_GrabComplete(sender, e);
                    }
                    m_Job_Mode1 = 1;

                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
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
                    if (ctr_PLC1.m_threads_Check)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                            }
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                        }
                        //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                        //{
                        //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                        //}
                    }
                    add_Log("CAM1 Grab Error!");
                }
            }
            else
            {
                if (ctr_PLC1.m_threads_Check)
                {
                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                        }
                    }
                    else
                    {
                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                    }
                    //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                    //{
                    //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                    //}

                }
                //if (ctrCam2.m_bitmap != null)
                //{
                //    if (Missed_Count[Cam_Num] >= 9)
                //    {
                //        Missed_Count[Cam_Num] = 0;
                //    }
                //    else
                //    {
                //        Missed_Count[Cam_Num]++;
                //    }
                //    Missed_Image1[Missed_Count[Cam_Num]] = (Bitmap)ctrCam2.m_bitmap.Clone();
                //    m_Missed_Job_Mode1 = 1;
                //}
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                add_Log("CAM1 Miss!");
            }
        }

        private void ctrCam3_GrabComplete(object sender, EventArgs e)
        {
            int Cam_Num = 2;
            ctr_Camera_Setting3.Grab_Num++;
            IPSSTApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + Utility.CalculateFrameRate2().ToString();
            if (m_Job_Mode2 == 0)// && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            {
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
                    Capture_Image2[Capture_Count[Cam_Num]] = (Bitmap)ctrCam3.m_bitmap.Clone();
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                    {
                        ctrCam1.m_bitmap = (Bitmap)ctrCam3.m_bitmap.Clone();
                        ctrCam1_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                    {
                        ctrCam2.m_bitmap = (Bitmap)ctrCam3.m_bitmap.Clone();
                        ctrCam2_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[3] != 3 && !ctr_Camera_Setting4.Force_USE.Checked)
                    {
                        ctrCam4.m_bitmap = (Bitmap)ctrCam3.m_bitmap.Clone();
                        ctrCam4_GrabComplete(sender, e);
                    }
                    m_Job_Mode2 = 1;

                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[3] == 3 && !ctr_Camera_Setting4.Force_USE.Checked)
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
                    if (ctr_PLC1.m_threads_Check)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                            }
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                        }
                        //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                        //{
                        //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                        //}
                    }
                    add_Log("CAM2 Grab Error!");
                }
            }
            else
            {
                if (ctr_PLC1.m_threads_Check)
                {
                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                        }
                    }
                    else
                    {
                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                    }
                    //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                    //{
                    //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                    //}
                }
                //if (ctrCam3.m_bitmap != null)
                //{
                //    if (Missed_Count[Cam_Num] >= 9)
                //    {
                //        Missed_Count[Cam_Num] = 0;
                //    }
                //    else
                //    {
                //        Missed_Count[Cam_Num]++;
                //    }
                //    Missed_Image2[Missed_Count[Cam_Num]] = (Bitmap)ctrCam3.m_bitmap.Clone();
                //    m_Missed_Job_Mode2 = 1;
                //}
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                add_Log("CAM2 Miss!");
            }

        }


        //private int Cam3_Missed = 0;
        private void ctrCam4_GrabComplete(object sender, EventArgs e)
        {
            int Cam_Num = 3;
            ctr_Camera_Setting4.Grab_Num++;
            IPSSTApp.Instance().m_Config.m_FPS[Cam_Num] = "FPS : " + Utility.CalculateFrameRate3().ToString();
            if (m_Job_Mode3 == 0)// && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            {
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
                    Capture_Image3[Capture_Count[Cam_Num]] = (Bitmap)ctrCam4.m_bitmap.Clone();
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[0] != 3 && !ctr_Camera_Setting1.Force_USE.Checked)
                    {
                        ctrCam1.m_bitmap = (Bitmap)ctrCam4.m_bitmap.Clone();
                        ctrCam1_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[1] != 3 && !ctr_Camera_Setting2.Force_USE.Checked)
                    {
                        ctrCam2.m_bitmap = (Bitmap)ctrCam4.m_bitmap.Clone();
                        ctrCam2_GrabComplete(sender, e);
                    }
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[2] != 3 && !ctr_Camera_Setting3.Force_USE.Checked)
                    {
                        ctrCam3.m_bitmap = (Bitmap)ctrCam4.m_bitmap.Clone();
                        ctrCam3_GrabComplete(sender, e);
                    }
                    m_Job_Mode3 = 1;

                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[0] == 3 && !ctr_Camera_Setting1.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[1] == 3 && !ctr_Camera_Setting2.Force_USE.Checked)
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
                    if (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == Cam_Num && IPSSTApp.Instance().m_Config.m_Cam_Kind[2] == 3 && !ctr_Camera_Setting3.Force_USE.Checked)
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
                    if (ctr_PLC1.m_threads_Check)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                                IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                            }
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                        }
                        //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                        //{
                        //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                        //}
                    }
                    add_Log("CAM3 Grab Error!");
                }
            }
            else
            {
                if (ctr_PLC1.m_threads_Check)
                {
                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                    {
                        if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                        }
                        else
                        {
                            ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + "10");
                            IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                        }
                    }
                    else
                    {
                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + "10");
                    }
                    //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                    //{
                    //    add_Log("CAM" + Cam_Num.ToString() + ":10");
                    //}
                }
                //if (ctrCam3.m_bitmap != null)
                //{
                //    if (Missed_Count[Cam_Num] >= 9)
                //    {
                //        Missed_Count[Cam_Num] = 0;
                //    }
                //    else
                //    {
                //        Missed_Count[Cam_Num]++;
                //    }
                //    Missed_Image2[Missed_Count[Cam_Num]] = (Bitmap)ctrCam3.m_bitmap.Clone();
                //    m_Missed_Job_Mode2 = 1;
                //}
                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 3]++;
                add_Log("CAM3 Miss!");
            }

        }

        //private int Cam4_Missed = 0;
        private void ctrCam5_GrabComplete(object sender, EventArgs e)
        {
            //    //m_Job_Mode4 = 1;
            //    int m_Selected_Cam_Num = 4;
            //    int Judge = 40;
            //    if (m_Job_Mode4 == 0 && ctrCam5.m_bitmap != null)
            //    {
            //        Capture_Count[4]++;
            //        if (Capture_Count[4] > 4)
            //        {
            //            Capture_Count[4] = 0;
            //        }
            //        Capture_Image4[Capture_Count[4]] = (Bitmap)ctrCam5.m_bitmap.Clone();
            //        m_Job_Mode4 = 1;
            //    }
            //    else
            //    {
            //        Capture_Count[4]++;
            //        //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            //        //{
            //        //    Cam4_Missed++;
            //        //    string filename = IPSSTApp.Instance().excute_path + "\\Save\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "Cam4_Missed_" + Cam4_Missed.ToString() + ".bmp";
            //        //    ctrCam5.m_bitmap.Save(filename, ImageFormat.Bmp);
            //        //}
            //        if (this.InvokeRequired)
            //        {
            //            this.Invoke((MethodInvoker)delegate
            //            {
            //                ctr_PLC1.send_Message.Add("DW510" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //            });
            //        }
            //        else
            //        {
            //            ctr_PLC1.send_Message.Add("DW510" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //        }
            //        add_Log("CAM4 미처리 발생!");
            //    }
        }

        private void ctrCam6_GrabComplete(object sender, EventArgs e)
        {
            int CamNum = 5;
            if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                }
            }
            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipY);
                }
            }
            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY);
                }
            }
            else
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    //ctrCam6.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam6.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
            }
            if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                pictureBox_CAM5.Image = null;
                pictureBox_CAM5.Image = ctrCam6.m_bitmap;
                ctr_Camera_Setting6.Grab_Num++;
                pictureBox_CAM5.Refresh();

                // ctr_Display_1.pictureBox_5.Image = ctrCam6.m_bitmap;
            }
            else
            {
                //ctr_Display_1.pictureBox_5.Image = ctrCam6.m_bitmap;
            }
            //ctr_Display_1.Update_Main_Image(CamNum);
            bool t_Judge = true;

            IPSSTApp.Instance().m_Config.Result_Image_Save(5, (Bitmap)ctrCam6.m_bitmap.Clone(), t_Judge);
        }

        private void ctrCam7_GrabComplete(object sender, EventArgs e)
        {
            int CamNum = 6;
            if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                }
            }
            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipY);
                }
            }
            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY);
                }
            }
            else
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    //ctrCam7.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam7.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
            }
            if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                pictureBox_CAM6.Image = null;
                pictureBox_CAM6.Image = ctrCam7.m_bitmap;
                ctr_Camera_Setting7.Grab_Num++;
                pictureBox_CAM6.Refresh();

                //ctr_Display_1.pictureBox_6.Image = ctrCam7.m_bitmap;
            }
            else
            {
                //ctr_Display_1.pictureBox_6.Image = ctrCam7.m_bitmap;
            }
            //ctr_Display_1.Update_Main_Image(CamNum);
            bool t_Judge = true;

            IPSSTApp.Instance().m_Config.Result_Image_Save(6, (Bitmap)ctrCam7.m_bitmap.Clone(), t_Judge);
        }

        private void ctrCam8_GrabComplete(object sender, EventArgs e)
        {
            int CamNum = 7;
            if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                }
            }
            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipY);
                }
            }
            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipXY);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY);
                }
            }
            else
            {
                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
                {
                    //ctrCam8.m_bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
                {
                    ctrCam8.m_bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
            }
            if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                pictureBox_CAM7.Image = null;
                pictureBox_CAM7.Image = ctrCam8.m_bitmap;
                ctr_Camera_Setting8.Grab_Num++;
                pictureBox_CAM7.Refresh();

                //ctr_Display_1.pictureBox_7.Image = ctrCam8.m_bitmap;
            }
            else
            {
                //ctr_Display_1.pictureBox_7.Image = ctrCam8.m_bitmap;
            }
            //ctr_Display_1.Update_Main_Image(CamNum);
            bool t_Judge = true;

            IPSSTApp.Instance().m_Config.Result_Image_Save(7, (Bitmap)ctrCam8.m_bitmap.Clone(), t_Judge);
        }

        private void pictureBox_IPSST_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ips-auto.com");
        }

        private void dataGridView_AUTO_STATUS_SizeChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in IPSSTApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows)
            {
                row.Height = (IPSSTApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Height - IPSSTApp.Instance().m_mainform.dataGridView_AUTO_STATUS.ColumnHeadersHeight) / IPSSTApp.Instance().m_mainform.dataGridView_AUTO_STATUS.Rows.Count;
            }
        }

        private void dataGridView_AUTO_COUNT_SizeChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in IPSSTApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows)
            {
                row.Height = (IPSSTApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Height - IPSSTApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersHeight) / IPSSTApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows.Count;
            }
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
            IPSSTApp.Instance().m_Config.neoTabWindow_MAIN_idx = e.TabPageIndex;
            if (e.TabPageIndex != 1)
            {
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
            }
            if (e.TabPageIndex == 7)
            {
                neoTabWindow_MAIN.SelectedIndex = t_cur_MAIN_SelectedIndex;
                if (IPSSTApp.Instance().m_help.m_hide_check || IPSSTApp.Instance().m_help.WindowState == FormWindowState.Minimized)
                {
                    IPSSTApp.Instance().m_help.Show();
                    IPSSTApp.Instance().m_help.m_hide_check = false;
                    if (IPSSTApp.Instance().m_help.WindowState == FormWindowState.Minimized)
                    {
                        IPSSTApp.Instance().m_help.WindowState = FormWindowState.Normal;
                    }
                }
                else if (!IPSSTApp.Instance().m_help.m_hide_check)
                {
                    IPSSTApp.Instance().m_help.Hide();
                    IPSSTApp.Instance().m_help.m_hide_check = true;
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
                if (IPSSTApp.Instance().m_Config.m_Administrator_Password_Flag)
                {
                    IPSSTApp.Instance().m_Config.m_Administrator_Password_Flag = false;
                    //IPSSTApp.Instance().m_parameters.m_Administrator_GH_Password_Flag = false;
                    //ctr_PLC_Data1.button_Offset.Visible = false;
                    //ctr_PLC_Data1.neoTab_Camera_SelectedIndex = 5;
                    //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon;
                    //neoTabWindow_MAIN.TabPages[1].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[2].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[3].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[4].Enabled = false;
                    //neoTabWindow_MAIN.TabPages[5].Enabled = false;
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        neoTabWindow_MAIN.TabPages[6].Text = "Login";
                    }

                    //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
                    //IPSSTApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = false;
                    //IPSSTApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = false;
                }
                else
                {
                    Frm_Password m_Frm_Password = new Frm_Password();
                    m_Frm_Password.ShowDialog();

                    if (!IPSSTApp.Instance().m_Config.m_Administrator_Password_Flag)
                    {
                        //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon;
                        //neoTabWindow_MAIN.TabPages[1].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[2].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[3].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[4].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[5].Enabled = false;
                        if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                        }
                        else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Login";
                        }
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = false;
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = false;
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
                        if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그아웃";
                        }
                        else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Logout";
                        }
                        neoTabWindow_MAIN.TabPages[6].Width = t_size;
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Logoff");
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = true;
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = true;
                    }

                    //if (IPSSTApp.Instance().m_Config.m_Administrator_GH_Password_Flag)
                    //{
                    //    //ctr_PLC_Data1.button_Offset.Visible = true;
                    //}
                    //else
                    //{
                    //    //ctr_PLC_Data1.button_Offset.Visible = false;
                    //}
                }
                neoTabWindow_MAIN.SelectedIndex = 0;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
            }
            else if (e.TabPageIndex > 0 && e.TabPageIndex < 6 && e.TabPageIndex != 3 && e.TabPageIndex != 4)
            {
                if (!IPSSTApp.Instance().m_Config.m_Administrator_Password_Flag)
                {
                    //MessageBox.Show("관리자모드가 아닙니다. 관리자 로그인하세요!.");
                    Frm_Password m_Frm_Password = new Frm_Password();
                    m_Frm_Password.ShowDialog();
                    if (!IPSSTApp.Instance().m_Config.m_Administrator_Password_Flag)
                    {
                        neoTabWindow_MAIN.SelectedIndex = 0;
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Administrator_Password_Flag)
                    {
                        //button_KEY.Image = global::LCD_Align_SW.Properties.Resources.Key_icon;
                        //neoTabWindow_MAIN.TabPages[1].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[2].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[3].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[4].Enabled = false;
                        //neoTabWindow_MAIN.TabPages[5].Enabled = false;
                        if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그인";
                        }
                        else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Login";
                        }
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Login");
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = false;
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = false;
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
                        if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            neoTabWindow_MAIN.TabPages[6].Text = "로그아웃";
                        }
                        else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            neoTabWindow_MAIN.TabPages[6].Text = "Logout";
                        }
                        neoTabWindow_MAIN.TabPages[6].Width = t_size;
                        //button_KEY_ToolTip.SetToolTip(this.button_KEY, "관리자모드 Logoff");
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE2.Enabled = true;
                        //IPSSTApp.Instance().m_Ctr_Auto.button_MODE3.Enabled = true;
                    }
                }
            }
            if (e.TabPageIndex == 0)
            {
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
                //neoTabWindow_INSP_SETTING_CAM.TabPages[0].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[1].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[2].Visible = false;
                //neoTabWindow_INSP_SETTING_CAM.TabPages[3].Visible = false;
                //neoTabWindow2_LOG.TabPages[0].Visible = false;
                //neoTabWindow2_LOG.TabPages[1].Visible = false;
                //neoTabWindow2_LOG.TabPages[2].Visible = false;
                //neoTabWindow2_LOG.TabPages[3].Visible = false;
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    Form f = Application.OpenForms[i];
                    if (f.GetType() == typeof(Frm_Trackbar))
                    {
                        f.Close();
                    }
                }
            }
            else if (e.TabPageIndex == 1)
            {
                //IPSSTApp.Instance().m_Config.Load_Judge_Data();
                //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (neoTabWindow_INSP_SETTING.SelectedIndex == 0)
                    {
                        t_setting_view_mode = true;
                        t_cam_setting_view_mode = false;
                        //if (ctr_Log1.checkBox_Display.Checked)
                        //{
                        //    IPSSTApp.Instance().m_Config.Realtime_View_Check = false;
                        //}
                    }
                    else
                    {
                        t_setting_view_mode = false;
                        t_cam_setting_view_mode = false;
                        //if (ctr_Log1.checkBox_Display.Checked)
                        //{
                        //    IPSSTApp.Instance().m_Config.Realtime_View_Check = true;
                        //}
                    }
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

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (!m_Last_Start_Loading)
                        {
                            IPSSTApp.Instance().m_Config.Load_Judge_Data();
                            m_Last_Start_Loading = true;
                        }
                    }
                }
            }
            else if (e.TabPageIndex == 2)
            {
                //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                if (neoTabWindow_EQUIP_SETTING.SelectedIndex == 0)
                {
                    t_cam_setting_view_mode = true;
                    t_setting_view_mode = false;
                    //if (ctr_Log1.checkBox_Display.Checked)
                    //{
                    //    IPSSTApp.Instance().m_Config.Realtime_View_Check = true;
                    //}
                }
                else
                {
                    t_cam_setting_view_mode = false;
                    t_setting_view_mode = false;
                    //if (ctr_Log1.checkBox_Display.Checked)
                    //{
                    //    IPSSTApp.Instance().m_Config.Realtime_View_Check = true;
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
            if (e.TabPageIndex != 1 && e.TabPageIndex != 2)
            {
                t_cam_setting_view_mode = false;
                t_setting_view_mode = false;
                //if (ctr_Log1.checkBox_Display.Checked)
                //{
                //    IPSSTApp.Instance().m_Config.Realtime_View_Check = true;
                //}
            }

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
                //if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 1)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = false;
                //    ctr_Manual1.radioButton_CAM2.Visible = false;
                //    ctr_Manual1.radioButton_CAM3.Visible = false;
                //}
                //else if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 2)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = true;
                //    ctr_Manual1.radioButton_CAM2.Visible = false;
                //    ctr_Manual1.radioButton_CAM3.Visible = false;
                //}
                //else if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 3)
                //{
                //    ctr_Manual1.radioButton_CAM0.Visible = true;
                //    ctr_Manual1.radioButton_CAM1.Visible = true;
                //    ctr_Manual1.radioButton_CAM2.Visible = true;
                //    ctr_Manual1.radioButton_CAM3.Visible = false;
                //}
                //else if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num == 4)
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
                string msg = "";
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    msg = "Data 초기화 하시겠습니까?";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    msg = "Do you want to reset data?";
                }

                if (!Force_close)
                {
                    if (MessageBox.Show(msg, " RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                for (int i = 0; i < 41; i++)
                {
                    IPSSTApp.Instance().m_Config.m_GraphData0[i].name = "";
                    IPSSTApp.Instance().m_Config.m_GraphData0[i].use = false;
                    IPSSTApp.Instance().m_Config.m_GraphData0[i].list.Clear();
                    IPSSTApp.Instance().m_Config.m_GraphData1[i].name = "";
                    IPSSTApp.Instance().m_Config.m_GraphData1[i].use = false;
                    IPSSTApp.Instance().m_Config.m_GraphData1[i].list.Clear();
                    IPSSTApp.Instance().m_Config.m_GraphData2[i].name = "";
                    IPSSTApp.Instance().m_Config.m_GraphData2[i].use = false;
                    IPSSTApp.Instance().m_Config.m_GraphData2[i].list.Clear();
                    IPSSTApp.Instance().m_Config.m_GraphData3[i].name = "";
                    IPSSTApp.Instance().m_Config.m_GraphData3[i].use = false;
                    IPSSTApp.Instance().m_Config.m_GraphData3[i].list.Clear();
                }

                ctr_Log1.Refresh_Log_Data();
                IPSSTApp.Instance().m_Config.ds_LOG.Tables.Clear();
                IPSSTApp.Instance().m_Config.ds_LOG.Clear();
                //ctr_PLC1.PLC_Thread_Stop();
                for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                {
                    IPSSTApp.Instance().m_Config.m_Error_Flag[i] = -1;
                    IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] = 0;
                    IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] = 0;
                    IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] = 0;
                    IPSSTApp.Instance().m_Config.Initialize_Data_Log(i);
                    //IPSSTApp.Instance().m_Config.m_Log_Data_Cnt[i] = 0;
                    IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[i] = 0;
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

                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = 0;
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = 0;
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = 0;
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = 0;
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

                    //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = 0;
                    //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = 0;
                    //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = 0;
                    //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = 0;
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

                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows.Count; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][8] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][9] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][10] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][11] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][12] = 0;
                    }
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows.Count; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][8] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][9] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][10] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][11] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][12] = 0;
                    }
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows.Count; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][8] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][9] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][10] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][11] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][12] = 0;
                    }
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows.Count; i++)
                    {
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][8] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][9] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][10] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][11] = 0;
                        IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][12] = 0;
                    }
                }
                //for (int i = 0; i < IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows.Count; i++)
                //{
                //    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][8] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][9] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][10] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][11] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][12] = 0;
                //}
                //for (int i = 0; i < IPSSTApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows.Count; i++)
                //{
                //    IPSSTApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][8] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][9] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][10] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][11] = 0;
                //    IPSSTApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[i][12] = 0;
                //}

                //ctr_PLC1.PLC_Thread_Start();

                ctr_PLC1.PLC_L_WRITE("LX1" + ctr_PLC1.m_Pingpong_Num.ToString("0") + "14", 1d); // RESET
                this.Refresh();
            }
            catch
            {

            }
        }

        public void button_INSPECTION_Click(object sender, EventArgs e)
        {
            //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[0, 0] += 100;
            //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[0, 1] += 30;
            //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[1, 0] += 20;
            //IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[1, 1] += 129; 

            if (button_INSPECTION.Text == "검사 대기" || button_INSPECTION.Text == "START")
            {
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    Form f = Application.OpenForms[i];
                    if (f.GetType() == typeof(Frm_Trackbar))
                    {
                        f.Close();
                    }
                }
                //add_Log("Step_01");
                //if (neoTabWindow_INSP_SETTING.SelectedIndex != 0)
                //{
                //    neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                //    neoTabWindow_MAIN.SelectedIndex = 0;
                //}
                //if (!t_DongleKey.Check_License())
                //{
                //    return;
                //}
                timer_Refresh_Amount.Interval = 100;
                IPSSTApp.Instance().m_Config.Load_Judge_Data();
                IPSSTApp.Instance().m_Config.Set_Parameters();
                m_Last_Start_Loading = true;
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
                //if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                //{
                //    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                //}

                ctr_Log1.Refresh_Log_Data();
                IPSSTApp.Instance().m_Config.ds_LOG.Tables.Clear();
                IPSSTApp.Instance().m_Config.ds_LOG.Clear();
                //ctr_PLC1.PLC_Thread_Stop();
                for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                {
                    if (!IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                    {
                        IPSSTApp.Instance().m_Config.Initialize_Data_Log(i);
                        IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[i] = false;
                        IPSSTApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                    }
                }
                //add_Log("Step_02");

                //IPSSTApp.Instance().m_Config.ROI_Cam_Num = 0;
                //IPSSTApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
                //IPSSTApp.Instance().m_mainform.ctr_Camera_Setting1.button_TRIGGER_DELAY_CHANGE_Click(sender, e);

                //int t_num = (int)ctr_PLC1.PLC_D_READ("DW5056", 2);//검사 총갯수
                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = t_num;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[2] = t_num;
                //Thread.Sleep(20);
                //t_num = (int)ctr_PLC1.PLC_D_READ("DW5046", 2);//OK수
                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = t_num;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[0] = t_num;
                //Thread.Sleep(20);
                //t_num = (int)ctr_PLC1.PLC_D_READ("DW5048", 2);//NG수
                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = t_num;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[1] = t_num;
                //Thread.Sleep(20);
                //t_num = (int)ctr_PLC1.PLC_D_READ("DW5054", 2);//미처리수
                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = t_num;
                //IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[3] = t_num;
                //int total = Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1].ToString());
                //double OK_ratio = 0;
                //if (total > 0)
                //{
                //    OK_ratio = ((double)t_num / (double)total) * 100d;
                //}
                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[4][1] = OK_ratio.ToString("00.00");

                ctr_Display_1.pictureBox_0.BackgroundImage = null;
                ctr_Display_1.pictureBox_1.BackgroundImage = null;
                ctr_Display_1.pictureBox_2.BackgroundImage = null;
                ctr_Display_1.pictureBox_3.BackgroundImage = null;
                //ctr_Display_1.pictureBox_Main.BackgroundImage = null;
                pictureBox_Setting_0.BackgroundImage = null;
                pictureBox_Setting_1.BackgroundImage = null;
                pictureBox_Setting_2.BackgroundImage = null;
                pictureBox_Setting_3.BackgroundImage = null;
                IPSSTApp.Instance().m_Config.Set_Parameters();
                // IPSSTApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);

                //IPSSTApp.Instance().m_Config.Create_Save_Folders();
                //add_Log("Step_03");

                IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode = true;

                //IPSSTApp.Instance().m_Config.Initialize_Data_Log();

                if (!ctr_Camera_Setting1.Force_USE.Checked)
                {
                    ctr_Camera_Setting1.checkBox1.Checked = true;
                }
                if (!ctr_Camera_Setting2.Force_USE.Checked)
                {
                    ctr_Camera_Setting2.checkBox1.Checked = true;
                }
                if (!ctr_Camera_Setting3.Force_USE.Checked)
                {
                    ctr_Camera_Setting3.checkBox1.Checked = true;
                }
                if (!ctr_Camera_Setting4.Force_USE.Checked)
                {
                    ctr_Camera_Setting4.checkBox1.Checked = true;
                }
                //if (!ctr_Camera_Setting5.Force_USE.Checked)
                //{
                //    ctr_Camera_Setting5.checkBox1.Checked = true;
                //}

                ctr_Camera_Setting1.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                ctr_Camera_Setting2.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                ctr_Camera_Setting3.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                ctr_Camera_Setting4.button_TRIGGER_DELAY_CHANGE_Click(sender, e); Thread.Sleep(30);
                //add_Log("Step_04");
                if (!m_ImageSavethread_Check)
                {
                    bool t_space_check = true;
                    if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                    {
                        t_space_check = Check_HD_available(IPSSTApp.Instance().excute_path);
                    }
                    else
                    {
                        t_space_check = Check_HD_available(IPSSTApp.Instance().m_Config.m_Log_Save_Folder);
                    }
                    if (t_space_check)
                    {
                        IPSSTApp.Instance().SAVE_IMAGE_List[0].Clear();
                        IPSSTApp.Instance().SAVE_IMAGE_List[1].Clear();
                        IPSSTApp.Instance().SAVE_IMAGE_List[2].Clear();
                        IPSSTApp.Instance().SAVE_IMAGE_List[3].Clear();
                        IPSSTApp.Instance().m_Config.Create_Save_Folders();
                        if (ImageSavethread != null)
                        {
                            ImageSavethread = null;
                        }
                        ImageSavethread = new Thread(ImageSavethread_Proc);
                        m_ImageSavethread_Check = true;
                        ImageSavethread.Start();
                        add_Log("Image save thread restart!");
                    }
                }


                Inspection_Thread_Start();

                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
                }
                if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                {
                    IPSSTApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
                }

                Thread.Sleep(100);
                if (!ctr_Camera_Setting1.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == 0))
                {
                    ctr_Camera_Setting1.toolStripButtonContinuousShot_Click(sender, e);
                }
                if (!ctr_Camera_Setting2.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == 1))
                {
                    ctr_Camera_Setting2.toolStripButtonContinuousShot_Click(sender, e);
                }
                if (!ctr_Camera_Setting3.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == 2))
                {
                    ctr_Camera_Setting3.toolStripButtonContinuousShot_Click(sender, e);
                }
                if (!ctr_Camera_Setting4.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == 3))
                {
                    ctr_Camera_Setting4.toolStripButtonContinuousShot_Click(sender, e);
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

                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(IPSSTApp.Instance().m_Config.Alg_TextView, IPSSTApp.Instance().m_Config.Alg_Debugging);

                ctr_PLC1.PLC_L_WRITE("LX1" + ctr_PLC1.m_Pingpong_Num.ToString("0") + "10", 1); // 검사시작
                ctr_PLC1.PLC_L_WRITE("LX1" + ctr_PLC1.m_Pingpong_Num.ToString("0") + "11", 0); // 검사중지
                richTextBox_LOG.ResetText();
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    button_INSPECTION.Text = "검 사 중...";
                    add_Log("검사를 시작하였습니다.");
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    button_INSPECTION.Text = "RUNNING...";
                    add_Log("Inspection started.");
                }
                button_INSPECTION.BackgroundImage = IPSST_Inspection_System.Properties.Resources.Button_BG2;
            }
            else
            {
                string msg = "";
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    msg = "검사를 정지 하시겠습니까?";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    msg = "Do you want to stop?";
                }

                if (!Force_close && !IPSSTApp.Instance().m_Config.m_Check_Server_Operation)
                {
                    if (MessageBox.Show(msg, " STOP", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                //Thread.Sleep(100);
                ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
                ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
                ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
                ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
                IPSSTApp.Instance().m_Config.CSV_Logfile_Terminate();
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    button_INSPECTION.Text = "검사 대기";
                    add_Log("검사를 정지하였습니다.");
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    button_INSPECTION.Text = "START";
                    add_Log("Inspection stopped.");
                }
                button_INSPECTION.BackgroundImage = IPSST_Inspection_System.Properties.Resources.Button_BG;

                ctr_PLC1.PLC_L_WRITE("LX1" + ctr_PLC1.m_Pingpong_Num.ToString("0") + "10", 0); // 검사시작
                ctr_PLC1.PLC_L_WRITE("LX1" + ctr_PLC1.m_Pingpong_Num.ToString("0") + "11", 1); // 검사중지


                IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode = false;

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
                //ctr_Display_1.pictureBox_0.BackgroundImage = global::IPSST_Inspection_System.Properties.Resources.Display;
                //ctr_Display_1.pictureBox_1.BackgroundImage = global::IPSST_Inspection_System.Properties.Resources.Display;
                //ctr_Display_1.pictureBox_2.BackgroundImage = global::IPSST_Inspection_System.Properties.Resources.Display;
                //ctr_Display_1.pictureBox_Main.BackgroundImage = global::IPSST_Inspection_System.Properties.Resources.Display;
                //IPSSTApp.Instance().m_Config.Stop_Save_Log();
            }
        }

        private int t_refresh_count = 0;
        private void timer_Refresh_Amount_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    double t_val_cnt = 0; double t_min_cnt = 9999999999999; double t_max_cnt = 0;
                    int t_min_idx = -1;
                    for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    {
                        t_val_cnt = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                        //if (double.TryParse(IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"].ToString(), out t_val_cnt))
                        {
                            if (t_val_cnt > 0 && t_val_cnt <= t_min_cnt)
                            {
                                t_min_cnt = t_val_cnt;
                                t_min_idx = i;
                            }
                            if (t_val_cnt > 0 && t_val_cnt >= t_max_cnt)
                            {
                                t_max_cnt = t_val_cnt;
                            }
                        }
                    }
                    if (t_min_idx > -1 && Math.Abs(t_max_cnt - t_min_cnt) > 10)
                    {
                        if (t_min_idx == 0)
                        {
                            ctr_Camera_Setting1.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting1.toolStripButtonContinuousShot_Click(null, null);
                        }
                        else if (t_min_idx == 1)
                        {
                            ctr_Camera_Setting2.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting2.toolStripButtonContinuousShot_Click(null, null);
                        }
                        else if (t_min_idx == 2)
                        {
                            ctr_Camera_Setting3.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting3.toolStripButtonContinuousShot_Click(null, null);
                        }
                        else if (t_min_idx == 3)
                        {
                            ctr_Camera_Setting4.toolStripButtonStop_Click(null, null);
                            Thread.Sleep(100);
                            ctr_Camera_Setting4.toolStripButtonContinuousShot_Click(null, null);
                        }
                        IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[t_min_idx, 1] += Math.Abs(t_max_cnt - t_min_cnt);
                    }
                }

                //if (this.InvokeRequired)
                //{
                //    this.Invoke((MethodInvoker)delegate
                //    {
                        //for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                        {
                            int i = 0;
                            if (!IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                                }
                                else
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                                }
                            }
                            i = 1;
                            if (!IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                                }
                                else
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                                }
                            }
                            i = 2;
                            if (!IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                                }
                                else
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                                }
                            }
                            i = 3;
                            if (!IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                                if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                                }
                                else
                                {
                                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                                }
                            }
                        }
                //    });
                //}
                //else
                //{
                //    for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                //    {
                //        if (i == 0 && m_Job_Mode0 == 0)
                //        {
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //        if (i == 1 && m_Job_Mode1 == 0)
                //        {
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //        if (i == 2 && m_Job_Mode2 == 0)
                //        {
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //        if (i == 3 && m_Job_Mode3 == 0)
                //        {
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                //            IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                //            if ((IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1]) > 0)
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = (100 * IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] / (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1])).ToString("0.0");
                //            }
                //            else
                //            {
                //                IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["YIELD"] = 0;
                //            }
                //        }
                //    }
                //}

                if (IPSSTApp.Instance().m_Ctr_Mysql.m_Connected_flag && IPSSTApp.Instance().m_Ctr_Mysql.conn != null)
                {
                    if (t_refresh_count == 0)
                    {
                        IPSSTApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[1], "Status");
                    }
                    else if (t_refresh_count == 1 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                    {
                        IPSSTApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1], "CAM0_setting_bt");
                    }
                    else if (t_refresh_count == 2 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                    {
                        IPSSTApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1], "CAM1_setting_bt");
                    }
                    else if (t_refresh_count == 3 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                    {
                        IPSSTApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1], "CAM2_setting_bt");
                    }
                    else if (t_refresh_count == 4 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                    {
                        IPSSTApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1], "CAM3_setting_bt");
                    }
                }


                bool t_space_check = true;
                if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder == "")
                {
                    t_space_check = Check_HD_available(IPSSTApp.Instance().excute_path);
                }
                else
                {
                    t_space_check = Check_HD_available(IPSSTApp.Instance().m_Config.m_Log_Save_Folder);
                }

                if (!m_ImageSavethread_Check)
                {
                    if (t_space_check)
                    {
                        IPSSTApp.Instance().SAVE_IMAGE_List[0].Clear();
                        IPSSTApp.Instance().SAVE_IMAGE_List[1].Clear();
                        IPSSTApp.Instance().SAVE_IMAGE_List[2].Clear();
                        IPSSTApp.Instance().SAVE_IMAGE_List[3].Clear();
                        IPSSTApp.Instance().m_Config.Create_Save_Folders();
                        if (ImageSavethread != null)
                        {
                            ImageSavethread = null;
                        }
                        ImageSavethread = new Thread(ImageSavethread_Proc);
                        m_ImageSavethread_Check = true;
                        ImageSavethread.Start();
                        add_Log("Image save thread restart!");
                    }
                }

                if (t_space_check)
                {
                    String m_Log_folder = IPSSTApp.Instance().excute_path + "\\Data\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    if (IPSSTApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                    {
                        m_Log_folder = IPSSTApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                    }

                    DirectoryInfo dir = new DirectoryInfo(m_Log_folder);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        IPSSTApp.Instance().m_Config.Create_Save_Folders();
                        for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                        {
                            if (!IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                            {
                                IPSSTApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                            }
                        }
                    }
                }

                //if (t_refresh_count == 1 || t_refresh_count == 4)
                //{
                //    GC.Collect();
                //}
                if (t_refresh_count == 0 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                {
                    IPSSTApp.Instance().m_mainform.ctr_DataGrid1.Min_Max_Update(t_refresh_count);
                }
                else if (t_refresh_count == 1 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                {
                    IPSSTApp.Instance().m_mainform.ctr_DataGrid2.Min_Max_Update(t_refresh_count);
                }
                else if (t_refresh_count == 2 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                {
                    IPSSTApp.Instance().m_mainform.ctr_DataGrid3.Min_Max_Update(t_refresh_count);
                }
                else if (t_refresh_count == 3 && !IPSSTApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[t_refresh_count])
                {
                    IPSSTApp.Instance().m_mainform.ctr_DataGrid4.Min_Max_Update(t_refresh_count);
                }

                t_refresh_count++;
                t_refresh_count %= 5;

                //int t_day = Math.Abs(DateTime.Now.DayOfYear - IPSSTApp.Instance().m_Config.t_Create_Save_Folders_oldtime.DayOfYear);
                //if (t_day >= 1)
                //{
                //    IPSSTApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
                //    IPSSTApp.Instance().m_Config.t_Create_Save_Folders_oldtime = DateTime.Now;
                //}
                if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    ctrCam1.m_bitmap = (Bitmap)ctr_ROI1.pictureBox_Image.Image.Clone();
                    ctrCam1_GrabComplete(sender, e);
                    ctrCam2.m_bitmap = (Bitmap)ctr_ROI2.pictureBox_Image.Image.Clone();
                    ctrCam2_GrabComplete(sender, e);
                    ctrCam3.m_bitmap = (Bitmap)ctr_ROI3.pictureBox_Image.Image.Clone();
                    ctrCam3_GrabComplete(sender, e);
                    ctrCam4.m_bitmap = (Bitmap)ctr_ROI4.pictureBox_Image.Image.Clone();
                    ctrCam4_GrabComplete(sender, e);
                }
                return;
                //table0.Rows.Add("전체", 0);//0
                //table0.Rows.Add("-양품", 0);//1
                //table0.Rows.Add("-불량", 0);//2
                //table0.Rows.Add("-미처리", 0);//3
                //table0.Rows.Add("수율(%)", 0);//4

                //if (IPSSTApp.Instance().m_mainform.m_Job_Mode[0] == 0)
                //{
                //    IPSSTApp.Instance().m_mainform.m_Job_Mode[0] = 1;
                //}
                //else
                //{
                //    add_Log("CAM0 미처리");
                //}
                //if (IPSSTApp.Instance().m_mainform.m_Job_Mode[1] == 0)
                //{
                //    IPSSTApp.Instance().m_mainform.m_Job_Mode[1] = 1;
                //}
                //else
                //{
                //    add_Log("CAM1 미처리");
                //}
                //if (IPSSTApp.Instance().m_mainform.m_Job_Mode[2] == 0)
                //{
                //    IPSSTApp.Instance().m_mainform.m_Job_Mode[2] = 1;
                //}
                //else
                //{
                //    add_Log("CAM2 미처리");
                //}
                ////for (int i = 0; i < 12; i++)
                ////{
                //    if (IPSSTApp.Instance().m_mainform.m_Job_Mode[3] == 0)
                //    {
                //        IPSSTApp.Instance().m_mainform.m_Job_Mode[3] = 1;
                //    }
                //    else
                //    {
                //        add_Log("CAM3 미처리");
                //    }
                //    if (IPSSTApp.Instance().m_mainform.m_Job_Mode[4] == 0)
                //    {
                //        IPSSTApp.Instance().m_mainform.m_Job_Mode[4] = 1;
                //    }
                //    else
                //    {
                //        add_Log("CAM4 미처리");
                //    }
                //    //    Thread.Sleep(2000 / 12);
                //////}
                //IPSSTApp.Instance().m_Config.Add_Log_Data(0, "");
                //return;
                double t_ng_max = 0; double t_ng_max_idx = -1;
                double t_ok_min = 999999999; int t_ok_min_idx = -1;
                double t_miss_max = 0; int t_miss_max_idx = -1;
                for (int i = 0; i < IPSSTApp.Instance().m_Config.m_Cam_Total_Num; i++)
                {
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1] >= t_ng_max)
                    {
                        t_ng_max = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                        t_ng_max_idx = i;
                    }
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] <= t_ok_min)
                    {
                        t_ok_min = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                        t_ok_min_idx = i;
                    }
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 2] >= t_miss_max)
                    {
                        t_miss_max = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 2];
                        t_miss_max_idx = i;
                    }
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0];
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[i]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[i, 1];
                    //ctr_PLC1.send_Message.Add("DW510" + i.ToString() + "_" + "40");
                }

                if (t_ng_max_idx == t_ok_min_idx)
                {
                    //return;
                    //Thread.Sleep(20);
                    //int t_num = (int)PLC_D_READ("DW5056", 2);//검사 총갯수
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[2] != t_ng_max + t_ok_min)
                    {
                        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = t_ng_max + t_ok_min;
                        IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[2] = t_ng_max + t_ok_min;
                    }
                    //Thread.Sleep(20);
                    //t_num = (int)PLC_D_READ("DW5046", 2);//OK수
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[0] != t_ok_min)
                    {
                        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = t_ok_min;
                        IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[0] = t_ok_min;
                    }
                    //Thread.Sleep(20);
                    //t_num = (int)PLC_D_READ("DW5048", 2);//NG수
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[1] != t_ng_max)
                    {
                        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = t_ng_max;
                        IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[1] = t_ng_max;
                    }
                    //Thread.Sleep(20);
                    //t_num = (int)PLC_D_READ("DW5054", 2);//미처리수
                    if (IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[3] != t_miss_max)
                    {
                        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = t_miss_max;
                        IPSSTApp.Instance().m_Config.m_OK_NG_Total_Miss_Cnt[3] = t_ng_max;
                    }
                    double total = Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1].ToString());
                    double OK_ratio = 0;
                    if (total > 0)
                    {
                        OK_ratio = (double.Parse(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1].ToString()) / total) * 100d;
                    }
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[4][1] = OK_ratio.ToString("00.00");
                }


                return;

                //if (!IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[0]
                //    && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[1]
                //    && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[2]
                //    && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[3]
                //    && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[4])
                ////&& !IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                //{
                //    if (ctr_PLC1.m_Cam_Trigger_Num < 3)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 3;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(94);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5056", 2);//검사 총갯수
                //        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] = t_num;
                //    }
                //    else if (ctr_PLC1.m_Cam_Trigger_Num == 3)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 4;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(95);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5046", 2);//OK수
                //        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[1][1] = t_num;

                //        if (IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1] == null)
                //        {
                //            ctr_PLC1.m_Trigger_Check = false;
                //            return;
                //        }
                //        int total = Convert.ToInt32(IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[0][1].ToString());
                //        double OK_ratio = 0;
                //        if (total > 0)
                //        {
                //            OK_ratio = ((double)t_num / (double)total) * 100d;
                //        }
                //        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[4][1] = OK_ratio.ToString("00.00");
                //    }
                //    else if (ctr_PLC1.m_Cam_Trigger_Num == 4)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 5;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(96);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5048", 2);//NG수
                //        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[2][1] = t_num;
                //    }
                //    else if (ctr_PLC1.m_Cam_Trigger_Num == 5)
                //    {
                //        ctr_PLC1.m_Cam_Trigger_Num = 2;
                //        ctr_PLC1.m_Trigger_Check = true;
                //        //ctr_PLC1.SerialTx(94);
                //        int t_num = (int)ctr_PLC1.PLC_D_READ("DW5054", 2);//미처리수
                //        IPSSTApp.Instance().m_Config.ds_STATUS.Tables[0].Rows[3][1] = t_num;
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
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[0]["TOTAL"] = t_num;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[0]["OK"] = t_OK;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[0]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 7)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 8;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5012", 2);//Cam1 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5014", 2);//Cam1 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5016", 2);//Cam1 NG
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[1]["TOTAL"] = t_num;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[1]["OK"] = t_OK;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[1]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 8)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 9;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5024", 2);//Cam2 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5026", 2);//Cam2 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5028", 2);//Cam2 NG
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2]["TOTAL"] = t_num;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2]["OK"] = t_OK;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[2]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 9)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 10;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5036", 2);//Cam3 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5038", 2);//Cam3 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5040", 2);//Cam3 NG
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3]["TOTAL"] = t_num;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3]["OK"] = t_OK;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[3]["NG"] = t_NG;
                //}
                //else if (ctr_PLC1.m_Cam_Trigger_Num == 10)
                //{
                //    ctr_PLC1.m_Cam_Trigger_Num = 2;
                //    ctr_PLC1.m_Trigger_Check = true;
                //    //ctr_PLC1.SerialTx(94);
                //    double t_num = ctr_PLC1.PLC_D_READ("DW5060", 2);//Cam4 트리거
                //    double t_OK = ctr_PLC1.PLC_D_READ("DW5062", 2);//Cam4 OK
                //    double t_NG = ctr_PLC1.PLC_D_READ("DW5064", 2);//Cam4 NG
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[4]["TOTAL"] = t_num;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[4]["OK"] = t_OK;
                //    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[4]["NG"] = t_NG;
                //}
                //ctr_PLC1.m_Trigger_Check = false;
            }
            catch (System.Exception ex)
            {

            }
        }

        private void Camera_Connection_Check()
        {
            bool check = true;
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 1 && !ctr_Camera_Setting1.Force_USE.Checked)
            {
                if (!IPSSTApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[0] == 0))
                {
                    check = false;
                }
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 2 && !ctr_Camera_Setting2.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[1] == 1))
            {
                if (!IPSSTApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                {
                    check = false;
                }
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 3 && !ctr_Camera_Setting3.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[2] == 2))
            {
                if (!IPSSTApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                {
                    check = false;
                }
            }
            if (IPSSTApp.Instance().m_Config.m_Cam_Total_Num >= 4 && !ctr_Camera_Setting4.Force_USE.Checked && (IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == -1 || IPSSTApp.Instance().m_Config.m_Interlock_Cam[3] == 3))
            {
                if (!IPSSTApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                {
                    check = false;
                }
            }
            //if (!IPSSTApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
            //{
            //    check = false;
            //}
            //if (!IPSSTApp.Instance().m_mainform.ctrCam5.m_imageProvider.IsOpen)
            //{
            //    check = false;
            //}

            if (check)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["값"] = "정상 연결";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"] = "Conn.";
                }

                // button_INSPECTION_Click(null, null);
            }
            else
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["값"] = "에러";
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[1]["Value"] = "Error";
                }
            }
        }

        private void neoTabWindow_EQUIP_SETTING_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
            {
                ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
            }
            if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
            {
                ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
            }
            if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
            {
                ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
            }
            if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
            {
                ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
            }

            //if (e.TabPageIndex == 0)
            //{
            //    if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            if (m_Threads_Check[0])
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                if (threads[i] != null)
                {
                    threads[i].Abort();
                    threads[i] = null;
                }
                //Viewthreads[i] = null;
                if (Probe_threads[i] != null)
                {
                    Probe_threads[i].Abort();
                    Probe_threads[i] = null;
                }

                m_Threads_Check[i] = true;
                if (ctr_Manual1.Run_SW[i] == null)
                {
                    ctr_Manual1.Run_SW[i] = new Stopwatch();
                    ctr_Manual1.Run_SW[i].Reset();
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
            m_Missed_Job_Mode0 = 0;
            m_Missed_Job_Mode1 = 0;
            m_Missed_Job_Mode2 = 0;
            m_Missed_Job_Mode3 = 0;
            //m_Job_Mode4 = 0;
            threads[0] = new Thread(ThreadProc0);
            threads[1] = new Thread(ThreadProc1);
            threads[2] = new Thread(ThreadProc2);
            threads[3] = new Thread(ThreadProc3);
            Probe_threads[0] = new Thread(ProbeThreadProc0);
            Probe_threads[1] = new Thread(ProbeThreadProc1);
            Probe_threads[2] = new Thread(ProbeThreadProc2);
            Probe_threads[3] = new Thread(ProbeThreadProc3);
            //Viewthreads[0] = new Thread(f);
            //Viewthreads[1] = new Thread(ResultProc1);
            //Viewthreads[2] = new Thread(ResultProc2);
            //Viewthreads[3] = new Thread(ResultProc3);
            //Missedthreads[0] = new Thread(MissedThreadProc0);
            //Missedthreads[1] = new Thread(MissedThreadProc1);
            //Missedthreads[2] = new Thread(MissedThreadProc2);
            //threads[3] = new Thread(ThreadProc3);
            //threads[4] = new Thread(ThreadProc4);
            //threads[0].Priority = ThreadPriority.Highest;
            //threads[1].Priority = ThreadPriority.Highest;
            //threads[2].Priority = ThreadPriority.Highest;


            if (!ctr_Camera_Setting1.Force_USE.Checked)
            {
                //Viewthreads[0].Start();
                if (IPSSTApp.Instance().m_Config.m_Cam_Kind[0] == 3)
                {
                    Probe_threads[0].Start();
                }
                else
                {
                    threads[0].Start();
                }
            }
            if (!ctr_Camera_Setting2.Force_USE.Checked)
            {
                //Viewthreads[1].Start();
                if (IPSSTApp.Instance().m_Config.m_Cam_Kind[1] == 3)
                {
                    Probe_threads[1].Start();
                }
                else
                {
                    threads[1].Start();
                }
            }
            if (!ctr_Camera_Setting3.Force_USE.Checked)
            {
                //Viewthreads[2].Start();
                if (IPSSTApp.Instance().m_Config.m_Cam_Kind[2] == 3)
                {
                    Probe_threads[2].Start();
                }
                else
                {
                    threads[2].Start();
                }
            }
            if (!ctr_Camera_Setting4.Force_USE.Checked)
            {
                //Viewthreads[3].Start();
                if (IPSSTApp.Instance().m_Config.m_Cam_Kind[3] == 3)
                {
                    Probe_threads[3].Start();
                }
                else
                {
                    threads[3].Start();
                }
            }
        }

        public void Inspection_Thread_Stop()
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (m_Threads_Check[i])
                    {
                        m_Threads_Check[i] = false;
                        if (threads[i] != null)
                        {
                            threads[i].Abort();
                        }
                        //Viewthreads[i].Abort();
                        if (Probe_threads[i] != null)
                        {
                            Probe_threads[i].Abort();
                        }
                    }
                    //threads[i].Join();
                    //threads[i] = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public void ThreadProc0()
        {
            int Cam_Num = 0;
            bool t_onecycle_check = false;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Job_Mode0 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Job_Mode0 == 1)
                {
                    if (t_onecycle_check)
                    {
                        m_Job_Mode0 = 0;
                        t_onecycle_check = false;
                        continue;
                    }
                    t_onecycle_check = true;
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();
                    if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                    }
                    // 영상처리 파트
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image0[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (m_Result_Job_Mode0 == 0)
                        {
                            m_Result_Job_Mode0 = 1;
                        }
                    }
                    else // 검사하면 아래로
                    {
                        //IPSSTApp.Instance().m_Config.Set_Parameters();
                        if (Capture_Image0[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                            {
                                byte[] arr = BmpToArray0(Capture_Image0[Capture_Count[Cam_Num]]);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, Capture_Image0[Capture_Count[Cam_Num]].Width, Capture_Image0[Capture_Count[Cam_Num]].Height, 3, Cam_Num);
                            }
                            else
                            {
                                Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image0[Capture_Count[Cam_Num]]);
                                byte[] arr = BmpToArray0(grayImage);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                grayImage.Dispose();
                            }
                        }
                        else
                        {
                            byte[] arr = BmpToArray0(Capture_Image0[Capture_Count[Cam_Num]]);
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, Capture_Image0[Capture_Count[Cam_Num]].Width, Capture_Image0[Capture_Count[Cam_Num]].Height, 1, Cam_Num);
                        }

                        ctr_Manual1.Run_Inspection(Cam_Num);

                        int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                        bool t_Judge = true;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                        }
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                        byte[] Dst_Img = null;
                        int width = 0, height = 0, ch = 0;

                        if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                        {
                            Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap0(Dst_Img, width, height, ch);
                            if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                            {
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)Result_Image0[Capture_Count[Cam_Num]].Clone();
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                            }
                            if (m_Result_Job_Mode0 == 0)
                            {
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode0 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode0 = 1;
                                    }
                                }
                                else
                                {
                                    m_Result_Job_Mode0 = 1;
                                }
                            }
                        }

                        //if (!t_Judge && ctr_PLC1.m_threads_Check)
                        //if (ctr_PLC1.m_threads_Check && Judge != 40)
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }
                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}
                        }
                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image0[Capture_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                    }


                    //if (!IPSSTApp.Instance().m_Config.PLC_Once_Tx_USE && ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                    //{
                    //    Thread.Sleep(50 - (int)ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds);
                    //}
                    ctr_Manual1.Run_SW[Cam_Num].Stop();
                    IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                    IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                    m_Job_Mode0 = 0;
                    t_onecycle_check = false;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }


        public void ResultProc0()
        {
            int Cam_Num = 0;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Result_Job_Mode0 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Result_Job_Mode0 == 1)
                {
                    if (IPSSTApp.Instance().m_Config.Realtime_View_Check)
                    {
                        Bitmap bitmapNew;
                        if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {// 검사중이 아닐때
                            bitmapNew = Capture_Image0[Capture_Count[Cam_Num]].Clone() as Bitmap;
                            //IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, bitmapNew.Clone() as Bitmap, true);
                        }
                        else
                        {// 검사중일때
                            bitmapNew = Result_Image0[Capture_Count[Cam_Num]].Clone() as Bitmap;
                        }

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
                        else if (!t_setting_view_mode && !t_cam_setting_view_mode)
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
                    }
                    m_Result_Job_Mode0 = 0;
                }
                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void MissedThreadProc0()
        {
            int Cam_Num = 0;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Missed_Job_Mode0 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Missed_Job_Mode0 == 1 && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    // 영상처리 파트
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image0[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {

                    }
                    else // 검사하면 아래로
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_MissedInspection_Check[Cam_Num] = true;
                        bool t_Judge = true;
                        int Judge = 10;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                        }
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }
                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}

                        }
                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Missed_Image1[Missed_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                    }

                    IPSSTApp.Instance().m_Config.m_Cam_MissedInspection_Check[Cam_Num] = false;
                    m_Missed_Job_Mode0 = 0;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public void ThreadProc1()
        {
            int Cam_Num = 1;
            bool t_onecycle_check = false;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Job_Mode1 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Job_Mode1 == 1)
                {
                    if (t_onecycle_check)
                    {
                        m_Job_Mode1 = 0;
                        t_onecycle_check = false;
                        continue;
                    }
                    t_onecycle_check = true;
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();
                    if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                    }
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image1[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (m_Result_Job_Mode1 == 0)
                        {
                            m_Result_Job_Mode1 = 1;
                        }
                    }
                    else // 검사하면 아래로
                    {
                        //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        if (Capture_Image1[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                            {
                                byte[] arr = BmpToArray1(Capture_Image1[Capture_Count[Cam_Num]]);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, Capture_Image1[Capture_Count[Cam_Num]].Width, Capture_Image1[Capture_Count[Cam_Num]].Height, 3, Cam_Num);
                            }
                            else
                            {
                                Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image1[Capture_Count[Cam_Num]]);
                                byte[] arr = BmpToArray1(grayImage);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                grayImage.Dispose();
                            }
                        }
                        else
                        {
                            byte[] arr = BmpToArray1(Capture_Image1[Capture_Count[Cam_Num]]);
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, Capture_Image1[Capture_Count[Cam_Num]].Width, Capture_Image1[Capture_Count[Cam_Num]].Height, 1, Cam_Num);
                        }

                        ctr_Manual1.Run_Inspection(Cam_Num);

                        int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                        bool t_Judge = true;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                        }
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                        byte[] Dst_Img = null;
                        int width = 0, height = 0, ch = 0;

                        if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image1(out Dst_Img, out width, out height, out ch, Cam_Num))
                        {
                            Result_Image1[Capture_Count[Cam_Num]] = ConvertBitmap1(Dst_Img, width, height, ch);
                            if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                            {
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)Result_Image1[Capture_Count[Cam_Num]].Clone();
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                            }
                            if (m_Result_Job_Mode1 == 0)
                            {
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode1 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode1 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode1 = 1;
                                }
                            }
                        }

                        //if (!t_Judge && ctr_PLC1.m_threads_Check)
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }
                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}

                        }
                        //}
                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image1[Capture_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);

                    }


                    //if (!IPSSTApp.Instance().m_Config.PLC_Once_Tx_USE && ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                    //{
                    //    Thread.Sleep(50 - (int)ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds);
                    //}
                    ctr_Manual1.Run_SW[Cam_Num].Stop();
                    IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                    IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                    m_Job_Mode1 = 0;
                    t_onecycle_check = false;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void MissedThreadProc1()
        {
            int Cam_Num = 1;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Missed_Job_Mode1 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Missed_Job_Mode1 == 1 && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    // 영상처리 파트
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image1[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {

                    }
                    else // 검사하면 아래로
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_MissedInspection_Check[Cam_Num] = true;
                        bool t_Judge = true;
                        int Judge = 10;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                        }
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }
                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}

                        }
                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Missed_Image1[Missed_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                    }

                    IPSSTApp.Instance().m_Config.m_Cam_MissedInspection_Check[Cam_Num] = false;
                    m_Missed_Job_Mode1 = 0;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void ResultProc1()
        {
            int Cam_Num = 1;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Result_Job_Mode1 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Result_Job_Mode1 == 1)
                {
                    if (IPSSTApp.Instance().m_Config.Realtime_View_Check)
                    {
                        Bitmap bitmapNew;
                        if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {// 검사중이 아닐때
                            bitmapNew = Capture_Image1[Capture_Count[Cam_Num]].Clone() as Bitmap;
                            //IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, bitmapNew.Clone() as Bitmap, true);

                        }
                        else
                        {// 검사중일때
                            bitmapNew = Result_Image1[Capture_Count[Cam_Num]].Clone() as Bitmap;
                        }

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
                        else if (!t_setting_view_mode && !t_cam_setting_view_mode)
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
                    }
                    m_Result_Job_Mode1 = 0;
                }
                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public void ThreadProc2()
        {
            int Cam_Num = 2;
            bool t_onecycle_check = false;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Job_Mode2 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Job_Mode2 == 1)
                {
                    if (t_onecycle_check)
                    {
                        m_Job_Mode2 = 0;
                        t_onecycle_check = false;
                        continue;
                    }
                    t_onecycle_check = true;
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();
                    if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                    }
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image2[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (m_Result_Job_Mode2 == 0)
                        {
                            m_Result_Job_Mode2 = 1;
                        }
                    }
                    else // 검사하면 아래로
                    {
                        //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        if (Capture_Image2[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                            {
                                byte[] arr = BmpToArray2(Capture_Image2[Capture_Count[Cam_Num]]);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, Capture_Image2[Capture_Count[Cam_Num]].Width, Capture_Image2[Capture_Count[Cam_Num]].Height, 3, Cam_Num);
                            }
                            else
                            {
                                Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image2[Capture_Count[Cam_Num]]);
                                byte[] arr = BmpToArray2(grayImage);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                grayImage.Dispose();
                            }
                        }
                        else
                        {
                            byte[] arr = BmpToArray2(Capture_Image2[Capture_Count[Cam_Num]]);
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, Capture_Image2[Capture_Count[Cam_Num]].Width, Capture_Image2[Capture_Count[Cam_Num]].Height, 1, Cam_Num);
                        }

                        ctr_Manual1.Run_Inspection(Cam_Num);

                        int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                        bool t_Judge = true;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                        }
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                        byte[] Dst_Img = null;
                        int width = 0, height = 0, ch = 0;

                        if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image2(out Dst_Img, out width, out height, out ch, Cam_Num))
                        {
                            Result_Image2[Capture_Count[Cam_Num]] = ConvertBitmap2(Dst_Img, width, height, ch);
                            if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                            {
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)Result_Image2[Capture_Count[Cam_Num]].Clone();
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                            }
                            if (m_Result_Job_Mode2 == 0)
                            {
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode2 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode2 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode2 = 1;
                                }
                            }
                        }


                        //if (!t_Judge && ctr_PLC1.m_threads_Check)
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }
                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}

                        }                        //}

                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image2[Capture_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                    }


                    //if (!IPSSTApp.Instance().m_Config.PLC_Once_Tx_USE && ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                    //{
                    //    Thread.Sleep(50 - (int)ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds);
                    //}
                    ctr_Manual1.Run_SW[Cam_Num].Stop();
                    IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                    IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                    m_Job_Mode2 = 0;
                    t_onecycle_check = false;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void MissedThreadProc2()
        {
            int Cam_Num = 2;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Missed_Job_Mode2 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Missed_Job_Mode2 == 1 && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
                {
                    // 영상처리 파트
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Missed_Image2[Missed_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {

                    }
                    else // 검사하면 아래로
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_MissedInspection_Check[Cam_Num] = true;
                        bool t_Judge = true;
                        int Judge = 10;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                        }
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }
                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}

                        }

                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Missed_Image2[Missed_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                    }

                    IPSSTApp.Instance().m_Config.m_Cam_MissedInspection_Check[Cam_Num] = false;
                    m_Missed_Job_Mode2 = 0;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void ResultProc2()
        {
            int Cam_Num = 2;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Result_Job_Mode2 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Result_Job_Mode2 == 1)
                {
                    if (IPSSTApp.Instance().m_Config.Realtime_View_Check)
                    {
                        Bitmap bitmapNew;
                        if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {// 검사중이 아닐때
                            bitmapNew = Capture_Image2[Capture_Count[Cam_Num]].Clone() as Bitmap;
                            //IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, bitmapNew.Clone() as Bitmap, true);
                        }
                        else
                        {// 검사중일때
                            bitmapNew = Result_Image2[Capture_Count[Cam_Num]].Clone() as Bitmap;
                        }

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
                        else if (!t_setting_view_mode && !t_cam_setting_view_mode)
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
                    }
                    m_Result_Job_Mode2 = 0;
                }
                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public void ThreadProc3()
        {
            int Cam_Num = 3;
            bool t_onecycle_check = false;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Job_Mode3 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Job_Mode3 == 1)
                {
                    if (t_onecycle_check)
                    {
                        m_Job_Mode3 = 0;
                        t_onecycle_check = false;
                        continue;
                    }
                    t_onecycle_check = true;
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();
                    if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                    }
                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == false)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[Cam_Num, 1] == true)
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 0)
                        {
                            //Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 1)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 2)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[Cam_Num] == 3)
                        {
                            Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        if (m_Result_Job_Mode3 == 0)
                        {
                            m_Result_Job_Mode3 = 1;
                        }
                    }
                    else // 검사하면 아래로
                    {
                        //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        if (Capture_Image3[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            if (IPSSTApp.Instance().m_Config.m_Cam_Kind[Cam_Num] == 2)
                            {
                                byte[] arr = BmpToArray3(Capture_Image3[Capture_Count[Cam_Num]]);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, Capture_Image3[Capture_Count[Cam_Num]].Width, Capture_Image3[Capture_Count[Cam_Num]].Height, 3, Cam_Num);
                            }
                            else
                            {
                                Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image3[Capture_Count[Cam_Num]]);
                                byte[] arr = BmpToArray3(grayImage);
                                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                                grayImage.Dispose();
                            }
                        }
                        else
                        {
                            byte[] arr = BmpToArray3(Capture_Image3[Capture_Count[Cam_Num]]);
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, Capture_Image3[Capture_Count[Cam_Num]].Width, Capture_Image3[Capture_Count[Cam_Num]].Height, 1, Cam_Num);
                        }

                        ctr_Manual1.Run_Inspection(Cam_Num);

                        int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                        bool t_Judge = true;
                        if (Judge != 40)
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                            t_Judge = false;
                        }
                        else
                        {
                            IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                        }
                        //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                        byte[] Dst_Img = null;
                        int width = 0, height = 0, ch = 0;

                        if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image3(out Dst_Img, out width, out height, out ch, Cam_Num))
                        {
                            Result_Image3[Capture_Count[Cam_Num]] = ConvertBitmap3(Dst_Img, width, height, ch);
                            if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                            {
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = (Bitmap)Result_Image3[Capture_Count[Cam_Num]].Clone();
                                IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                            }
                            if (m_Result_Job_Mode3 == 0)
                            {
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode3 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode3 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode3 = 1;
                                }
                            }
                        }


                        //if (!t_Judge && ctr_PLC1.m_threads_Check)
                        if (ctr_PLC1.m_threads_Check)
                        {
                            if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                    IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                }
                            }
                            else
                            {
                                ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                            }

                            //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                            //{
                            //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                            //}
                        }                        //}

                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone(), t_Judge);
                        IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, filename);
                    }

                    //if (!IPSSTApp.Instance().m_Config.PLC_Once_Tx_USE && ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds < 50)
                    //{
                    //    Thread.Sleep(50 - (int)ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds);
                    //}
                    ctr_Manual1.Run_SW[Cam_Num].Stop();
                    IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                    IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
                    m_Job_Mode3 = 0;
                    t_onecycle_check = false;
                }

                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
            }
        }

        public void ResultProc3()
        {
            int Cam_Num = 3;
            while (m_Threads_Check[Cam_Num])
            {
                if (m_Result_Job_Mode3 == 0)
                {
                    Thread.Sleep(1);
                }
                else if (m_Result_Job_Mode3 == 1)
                {
                    if (IPSSTApp.Instance().m_Config.Realtime_View_Check)
                    {
                        Bitmap bitmapNew;
                        if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                        {// 검사중이 아닐때
                            bitmapNew = Capture_Image3[Capture_Count[Cam_Num]].Clone() as Bitmap;
                            //IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, bitmapNew.Clone() as Bitmap, true);

                        }
                        else
                        {// 검사중일때
                            bitmapNew = Result_Image3[Capture_Count[Cam_Num]].Clone() as Bitmap;
                        }

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
                        else if (!t_setting_view_mode && !t_cam_setting_view_mode)
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
                    }
                    m_Result_Job_Mode3 = 0;
                }
                if (!m_Threads_Check[Cam_Num])
                {
                    break;
                }
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
        //            //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
        //            //{

        //            //    if (Cam3_Missed_SW.ElapsedMilliseconds > 100 && Cam3_Missed > 0 && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
        //            //    {
        //            //        string filename = IPSSTApp.Instance().excute_path + "\\Save\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "Cam3_Missed_" + Cam3_Missed.ToString() + ".bmp";
        //            //        Capture_Image3[Capture_Count[Cam_Num]] = new Bitmap(filename);
        //            //        File.Delete(filename);
        //            //        Cam3_Missed--;
        //            //        m_Job_Mode3 = 1;
        //            //    }
        //            //    else if (Cam3_Missed_SW.ElapsedMilliseconds > 100 && Cam3_Missed > 0 && IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
        //            //    {
        //            //        Cam3_Missed_SW.Reset();
        //            //        Cam3_Missed_SW.Start();
        //            //    }
        //            //}
        //        }
        //        else if (m_Job_Mode3 == 1)
        //        {
        //            //if (ctrCam4.m_bitmap != null && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
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
        //            ctr_Manual1.Run_SW[Cam_Num].Reset();
        //            ctr_Manual1.Run_SW[Cam_Num].Start();
        //            int CamNum = Cam_Num;


        //            if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
        //            {
        //                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
        //                }
        //            }
        //            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
        //            {
        //                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
        //                }
        //            }
        //            else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
        //            {
        //                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
        //                }
        //            }
        //            else
        //            {
        //                if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
        //                {
        //                    //Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
        //                }
        //                else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
        //                {
        //                    Capture_Image3[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
        //                }
        //            }

        //            if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
        //                String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(CamNum, Capture_Image3[Capture_Count[Cam_Num]], true);
        //            }
        //            else // 검사하면 아래로
        //            {
        //                ctr_Camera_Setting4.Grab_Num++;
        //                //if (IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum])
        //                //{
        //                //    // 검사 실패
        //                //    return;
        //                //}
        //                IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = true;
        //                if (Capture_Image3[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
        //                {
        //                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image3[Capture_Count[Cam_Num]]);
        //                    byte[] arr = BmpToArray3(grayImage);
        //                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, CamNum);
        //                    grayImage.Dispose();
        //                }
        //                else
        //                {
        //                    //byte[] arr = BmpToArray3((Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone());
        //                    byte[] arr = BmpToArray3(Capture_Image3[Capture_Count[Cam_Num]]);
        //                    IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, Capture_Image3[Capture_Count[Cam_Num]].Width, Capture_Image3[Capture_Count[Cam_Num]].Height, 1, CamNum);
        //                }

        //                bool t_Judge = true;
        //                //IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(CamNum);
        //                //string[] strParameter = IPSSTApp.Instance().m_mainform.m_ImProClr_Class.RUN_Algorithm(m_Selected_Cam_Num).Split('_');
        //                ctr_Manual1.Run_Inspection(CamNum);
        //                int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(CamNum);
        //                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].BeginLoadData();
        //                if (Judge != 40)
        //                {
        //                    IPSSTApp.Instance().m_Config.m_Error_Flag[CamNum] = 1;
        //                    IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1]++;
        //                    // IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
        //                    t_Judge = false;
        //                }
        //                else
        //                {
        //                    IPSSTApp.Instance().m_Config.m_Error_Flag[CamNum] = 0;
        //                    IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0]++;
        //                    //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0];
        //                }
        //                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
        //                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].EndLoadData();
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
        //                if (IPSSTApp.Instance().m_Config.Realtime_View_Check)
        //                {

        //                    byte[] Dst_Img = null;
        //                    int width = 0, height = 0, ch = 0;

        //                    if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image(out Dst_Img, out width, out height, out ch, CamNum))
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
        //                    String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(CamNum, (Bitmap)Capture_Image3[Capture_Count[Cam_Num]].Clone(), t_Judge);
        //                    IPSSTApp.Instance().m_Config.Add_Log_Data(CamNum, filename);
        //                }
        //            }
        //            IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = false;

        //            m_Job_Mode3 = 0;
        //            ctr_Manual1.Run_SW[Cam_Num].Stop();
        //            IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";
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
            //                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            //                    //{

            //                    //    if (Cam4_Missed_SW.ElapsedMilliseconds > 100 && Cam4_Missed > 0 && !IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            //                    //    {
            //                    //        string filename = IPSSTApp.Instance().excute_path + "\\Save\\" + IPSSTApp.Instance().m_Config.m_Model_Name + "\\" + "Cam4_Missed_" + Cam4_Missed.ToString() + ".bmp";
            //                    //        Capture_Image4[Capture_Count[Cam_Num]] = new Bitmap(filename);
            //                    //        File.Delete(filename);
            //                    //        Cam4_Missed--;
            //                    //        m_Job_Mode4 = 1;
            //                    //    }
            //                    //    else if (Cam4_Missed_SW.ElapsedMilliseconds > 100 && Cam4_Missed > 0 && IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num])
            //                    //    {
            //                    //        Cam4_Missed_SW.Reset();
            //                    //        Cam4_Missed_SW.Start();
            //                    //    }
            //                    //}
            //                }
            //                else if (m_Job_Mode4 == 1)
            //                {
            //                    //Cam4_Missed_SW.Stop();
            //                    ctr_Manual1.Run_SW[Cam_Num].Reset();
            //                    ctr_Manual1.Run_SW[Cam_Num].Start();
            //                    int CamNum = Cam_Num;

            //                    if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == false)
            //                    {
            //                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipX);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipX);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipX);
            //                        }
            //                    }
            //                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == false && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            //                    {
            //                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipY);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipY);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipY);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipY);
            //                        }
            //                    }
            //                    else if (IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 0] == true && IPSSTApp.Instance().m_Config.m_Cam_Filp[CamNum, 1] == true)
            //                    {
            //                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipXY);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipXY);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipXY);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipXY);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 0)
            //                        {
            //                            //Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.RotateNoneFlipX);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 1)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate90FlipNone);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 2)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate180FlipNone);
            //                        }
            //                        else if (IPSSTApp.Instance().m_Config.m_Cam_Rot[CamNum] == 3)
            //                        {
            //                            Capture_Image4[Capture_Count[Cam_Num]].RotateFlip(RotateFlipType.Rotate270FlipNone);
            //                        }
            //                    }
            //                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            //                        String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(CamNum, Capture_Image4[Capture_Count[Cam_Num]], true);
            //                    }
            //                    else // 검사하면 아래로
            //                    {
            //                        ctr_Camera_Setting5.Grab_Num++;
            //                        //if (IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum])
            //                        //{
            //                        //    // 검사 실패
            //                        //    return;
            //                        //}
            //                        IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = true;
            //                        if (Capture_Image4[Capture_Count[Cam_Num]].PixelFormat == PixelFormat.Format24bppRgb)
            //                        {
            //                            Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(Capture_Image4[Capture_Count[Cam_Num]]);
            //                            byte[] arr = BmpToArray4(grayImage);
            //                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_4(arr, grayImage.Width, grayImage.Height, 1, CamNum);
            //                            grayImage.Dispose();
            //                        }
            //                        else
            //                        {
            //                            byte[] arr = BmpToArray4((Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone());
            //                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_4(arr, Capture_Image4[Capture_Count[Cam_Num]].Width, Capture_Image4[Capture_Count[Cam_Num]].Height, 1, CamNum);
            //                        }

            //                        bool t_Judge = true;
            //                        //IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(CamNum);
            //                        //string[] strParameter = IPSSTApp.Instance().m_mainform.m_ImProClr_Class.RUN_Algorithm(m_Selected_Cam_Num).Split('_');
            //                        ctr_Manual1.Run_Inspection(CamNum);
            //                        int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(CamNum);
            //                        if (Judge != 40)
            //                        {
            //                            IPSSTApp.Instance().m_Config.m_Error_Flag[CamNum] = 1;
            //                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1]++;
            //                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
            //                            t_Judge = false;
            //                        }
            //                        else
            //                        {
            //                            IPSSTApp.Instance().m_Config.m_Error_Flag[CamNum] = 0;
            //                            IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0]++;
            //                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0];
            //                        }
            //                       // IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[CamNum]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[CamNum, 1];
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

            //                        //if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image(out Dst_Img, out width, out height, out ch, CamNum))
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
            //                            String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(CamNum, (Bitmap)Capture_Image4[Capture_Count[Cam_Num]].Clone(), t_Judge);
            //                            IPSSTApp.Instance().m_Config.Add_Log_Data(CamNum, filename);
            //                        }
            //                    }
            //                    IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[CamNum] = false;


            //                    m_Job_Mode4 = 0;
            //                    ctr_Manual1.Run_SW[Cam_Num].Stop();
            //                    IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change0));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                //    if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                //    {
                //        cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change0));
                //        cm.MenuItems.Add("==============");
                //    }
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }

                dataGridView_Setting_Value_0.ContextMenu = cm;
                dataGridView_Setting_Value_0.ContextMenu.Show(dataGridView_Setting_Value_0, e.Location);
                dataGridView_Setting_Value_0.ContextMenu = null;
            }
        }
        private void dataGridView_Setting_Value_Change0(object sender, EventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            IPSSTApp.Instance().m_Config.Save_Judge_Data();
        }

        private void dataGridView_Setting_Value_Load(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_Config.Load_Judge_Data();
        }

        private void dataGridView_Setting_Value_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change1));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change1));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change2));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change2));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("값 변경", new EventHandler(dataGridView_Setting_Value_Change3));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    //{
                    //    cm.MenuItems.Add("Change Value", new EventHandler(dataGridView_Setting_Value_Change3));
                    //    cm.MenuItems.Add("==============");
                    //}
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
                }
                dataGridView_Setting_Value_4.ContextMenu = cm;
                dataGridView_Setting_Value_4.ContextMenu.Show(dataGridView_Setting_Value_4, e.Location);
                dataGridView_Setting_Value_4.ContextMenu = null;
            }

        }

        private void neoTabWindow_ALG_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            if (neoTabWindow_MAIN.SelectedIndex == 1 && neoTabWindow_INSP_SETTING.SelectedIndex == 1 && IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("자동검사 중입니다. 정지 후 설정하세요!");
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("Running inspection. After stop, setup please!");
                }
                return;
            }
            //int old_idx = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num];
            IPSSTApp.Instance().m_Config.ROI_Cam_Num = neoTabWindow_ALG.SelectedIndex;
            if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 0)
            {
                if (ctr_ROI1.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI1.listBox1.SelectedIndex = 0;
                }
                //IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI1.listBox1.SelectedIndex;
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
                if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }

                //ctr_ROI1.pictureBox_Image.Refresh();
                //ctr_ROI1.button_SAVE_Click(sender, e);
            }
            else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 1)
            {
                if (ctr_ROI2.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI2.listBox1.SelectedIndex = 0;
                }
                //IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI2.listBox1.SelectedIndex;
                //ctr_ROI2.listBox1.SelectedIndex = 0;
                // ctr_ROI2.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num];
                ctr_ROI2.listBox1_SelectedIndexChanged(sender, e);
                //ctr_ROI2.listBox1.SelectedIndex = old_idx;
                ctr_ROI2.Referesh_Select_Menu(false);
                ctr_ROI2.button_LOAD_Click(sender, e);
                ctr_ROI2.load_check = false;
                ctr_ROI2.Referesh_Select_Menu(true);
                ctr_ROI2.Fit_Size();
                ctr_ROI2.dataGridView1.Rows[0].Height = 0;
                if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                //ctr_ROI2.pictureBox_Image.Refresh();
                //ctr_ROI2.button_SAVE_Click(sender, e);
            }
            else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 2)
            {
                if (ctr_ROI3.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI3.listBox1.SelectedIndex = 0;
                }
                //IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI3.listBox1.SelectedIndex;
                ////ctr_ROI3.listBox1.SelectedIndex = 0;
                //ctr_ROI3.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num];
                ctr_ROI3.listBox1_SelectedIndexChanged(sender, e);
                ctr_ROI3.Referesh_Select_Menu(false);
                ctr_ROI3.button_LOAD_Click(sender, e);
                ctr_ROI3.load_check = false;
                ctr_ROI3.Referesh_Select_Menu(true);
                ctr_ROI3.Fit_Size();
                ctr_ROI3.dataGridView1.Rows[0].Height = 0;
                if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI4.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI4.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                //ctr_ROI3.pictureBox_Image.Refresh();
                //ctr_ROI3.button_SAVE_Click(sender, e);
            }
            else if (IPSSTApp.Instance().m_Config.ROI_Cam_Num == 3)
            {
                if (ctr_ROI4.listBox1.SelectedIndex < 0)
                {
                    ctr_ROI4.listBox1.SelectedIndex = 0;
                }
                //IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num] = ctr_ROI4.listBox1.SelectedIndex;
                ////ctr_ROI4.listBox1.SelectedIndex = 0;
                //ctr_ROI4.listBox1.SelectedIndex = IPSSTApp.Instance().m_Config.ROI_Selected_IDX[IPSSTApp.Instance().m_Config.ROI_Cam_Num];
                ctr_ROI4.listBox1_SelectedIndexChanged(sender, e);
                ctr_ROI4.Referesh_Select_Menu(false);
                ctr_ROI4.button_LOAD_Click(sender, e);
                ctr_ROI4.load_check = false;
                ctr_ROI4.Referesh_Select_Menu(true);
                ctr_ROI4.Fit_Size();
                ctr_ROI4.dataGridView1.Rows[0].Height = 0;
                if (ctr_ROI1.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI1.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI2.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI2.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }
                if (ctr_ROI3.ctr_ROI_Guide1.t_realtime_check)
                {
                    ctr_ROI3.ctr_ROI_Guide1.button_Realtime_Click(sender, e);
                }

                //ctr_ROI4.pictureBox_Image.Refresh();
                //ctr_ROI4.button_SAVE_Click(sender, e);
            }

            try
            {
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    Form f = Application.OpenForms[i];
                    if (f.GetType() == typeof(Frm_Trackbar))
                    {
                        f.Close();
                    }
                }
            }
            catch
            {
            }
        }

        private void neoTabWindow_ALG_MouseClick(object sender, MouseEventArgs e)
        {
            IPSSTApp.Instance().m_Config.ROI_Cam_Num = neoTabWindow_ALG.SelectedIndex;
        }

        public int t_RowIndex = 0;
        private int t_ColIndex = 0;
        bool t_CellBeginEdit_check = false;
        private void dataGridView_Setting_Value_0_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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

                if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                IPSSTApp.Instance().m_Config.neoTabWindow_INSP_SETTING_idx = e.TabPageIndex;
                if (neoTabWindow_MAIN.SelectedIndex == 1 && neoTabWindow_INSP_SETTING.SelectedIndex == 1 && IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        MessageBox.Show("자동검사 중입니다. 정지 후 설정하세요!");
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        MessageBox.Show("Running inspection. After stop, setup please!");
                    }
                    //neoTabWindow_MAIN.SelectedIndex = 0;
                    neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                }


                //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                if (neoTabWindow_INSP_SETTING.SelectedIndex == 0 && IPSSTApp.Instance().m_mainform.m_Start_Check)
                {
                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                        IPSSTApp.Instance().m_Config.Save_Judge_Data();
                        Thread.Sleep(100);
                        IPSSTApp.Instance().m_Config.Load_Judge_Data();
                        Thread.Sleep(100);
                    }
                }
                if (neoTabWindow_INSP_SETTING.SelectedIndex == 1)
                {
                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                    //IPSSTApp.Instance().m_Config.Save_Judge_Data();
                    //IPSSTApp.Instance().m_Config.Load_Judge_Data();
                }
                else
                {
                    //if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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

                if (neoTabWindow_INSP_SETTING.SelectedIndex == 2)
                {
                    if (!IPSSTApp.Instance().m_Config.m_Administrator_Super_Password_Flag)
                    {
                        Frm_Password m_Frm_Password = new Frm_Password();
                        m_Frm_Password.ShowDialog();

                        if (!IPSSTApp.Instance().m_Config.m_Administrator_Super_Password_Flag)
                        {
                            if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                AutoClosingMessageBox.Show("고급 관리자로 로그인 하세요!", "Caution", 2000);
                            }
                            else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                            {//영어
                                AutoClosingMessageBox.Show("Please login for advenced admin!", "Caution", 2000);
                            }
                            neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                        }
                    }
                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave0));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave0));
                    }

                    pictureBox_Setting_0.ContextMenu = cm;
                    pictureBox_Setting_0.ContextMenu.Show(pictureBox_Setting_0, e.Location);
                    pictureBox_Setting_0.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    }
                    pictureBox_Setting_0.ContextMenu = cm;
                    pictureBox_Setting_0.ContextMenu.Show(pictureBox_Setting_0, e.Location);
                    pictureBox_Setting_0.ContextMenu = null;
                }
            }
        }

        private void PictureBoxRealtimeview(object sender, EventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.Realtime_View_Check)
            {
                IPSSTApp.Instance().m_Config.Realtime_View_Check = false;
                IPSSTApp.Instance().m_mainform.ctr_Log1.checkBox_Display.Checked = false;
            }
            else
            {
                IPSSTApp.Instance().m_Config.Realtime_View_Check = true;
                IPSSTApp.Instance().m_mainform.ctr_Log1.checkBox_Display.Checked = true;
            }
            IPSSTApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);
        }

        private void PictureBoxResultview(object sender, EventArgs e)
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
            IPSSTApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);
            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(IPSSTApp.Instance().m_Config.Alg_TextView, IPSSTApp.Instance().m_Config.Alg_Debugging);
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave1));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave1));
                    }

                    pictureBox_Setting_1.ContextMenu = cm;
                    pictureBox_Setting_1.ContextMenu.Show(pictureBox_Setting_1, e.Location);
                    pictureBox_Setting_1.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave1));
                    }

                    pictureBox_Setting_1.ContextMenu = cm;
                    pictureBox_Setting_1.ContextMenu.Show(pictureBox_Setting_1, e.Location);
                    pictureBox_Setting_1.ContextMenu = null;
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveCAM0));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveCAM0));
                    }

                    pictureBox_CAM0.ContextMenu = cm;
                    pictureBox_CAM0.ContextMenu.Show(pictureBox_CAM0, e.Location);
                    pictureBox_CAM0.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    }

                    pictureBox_CAM0.ContextMenu = cm;
                    pictureBox_CAM0.ContextMenu.Show(pictureBox_CAM0, e.Location);
                    pictureBox_CAM0.ContextMenu = null;
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveCAM1));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveCAM1));
                    }
                    pictureBox_CAM1.ContextMenu = cm;
                    pictureBox_CAM1.ContextMenu.Show(pictureBox_CAM1, e.Location);
                    pictureBox_CAM1.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    }
                    pictureBox_CAM1.ContextMenu = cm;
                    pictureBox_CAM1.ContextMenu.Show(pictureBox_CAM1, e.Location);
                    pictureBox_CAM1.ContextMenu = null;
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
            //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave2));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup view", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave2));
                    }

                    pictureBox_Setting_2.ContextMenu = cm;
                    pictureBox_Setting_2.ContextMenu.Show(pictureBox_Setting_2, e.Location);
                    pictureBox_Setting_2.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    }

                    pictureBox_Setting_2.ContextMenu = cm;
                    pictureBox_Setting_2.ContextMenu.Show(pictureBox_Setting_2, e.Location);
                    pictureBox_Setting_2.ContextMenu = null;
                }
            }
        }

        private void button_TEST_INSPECTION0_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE0_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = IPSSTApp.Instance().m_Config.ROI_Cam_Num;
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = 0;
            //ctr_ROI1.button_SAVE_Click(sender, e);
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void button_TEST_INSPECTION1_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE1_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = IPSSTApp.Instance().m_Config.ROI_Cam_Num;
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = 1;
            //ctr_ROI2.button_SAVE_Click(sender, e);
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void button_TEST_INSPECTION2_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE2_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = IPSSTApp.Instance().m_Config.ROI_Cam_Num;
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = 2;
            //ctr_ROI3.button_SAVE_Click(sender, e);
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void neoTabWindow_INSP_SETTING_CAM_SelectedIndexChanged(object sender, NeoTabControlLibrary.SelectedIndexChangedEventArgs e)
        {
            try
            {
                IPSSTApp.Instance().m_Config.neoTabWindow_INSP_SETTING_CAM_idx = e.TabPageIndex;
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
                IPSSTApp.Instance().m_Config.neoTabWindow_LOG_idx = e.TabPageIndex;
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
                IPSSTApp.Instance().m_Config.neoTabWindow2_LOG_idx = e.TabPageIndex;
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
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    cm.MenuItems.Add("설정값 갱신 및 저장", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("설정값 불러오기", new EventHandler(dataGridView_Setting_Value_Load));
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    cm.MenuItems.Add("Update and Save", new EventHandler(dataGridView_Setting_Value_Update));
                    cm.MenuItems.Add("Load", new EventHandler(dataGridView_Setting_Value_Load));
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
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSave3));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultview));
                        cm.MenuItems.Add("Popup view", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave3));
                    }

                    pictureBox_Setting_3.ContextMenu = cm;
                    pictureBox_Setting_3.ContextMenu.Show(pictureBox_Setting_3, e.Location);
                    pictureBox_Setting_3.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeview));
                    }
                    else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeview));
                    }

                    pictureBox_Setting_3.ContextMenu = cm;
                    pictureBox_Setting_3.ContextMenu.Show(pictureBox_Setting_3, e.Location);
                    pictureBox_Setting_3.ContextMenu = null;
                }
            }
        }

        private void dataGridView_Setting_Value_3_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
                if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
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
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Manual_Inspection_Click(sender, e);
        }

        private void button_SAVE3_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_Config.Save_Judge_Data();
            //int t_cam_num = IPSSTApp.Instance().m_Config.ROI_Cam_Num;
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = 3;
            //ctr_ROI4.button_SAVE_Click(sender, e);
            //IPSSTApp.Instance().m_Config.ROI_Cam_Num = t_cam_num;
        }

        private void Frm_Main_Move(object sender, EventArgs e)
        {
            IPSSTApp.Instance().t_QuickMenu.Top = this.Top + 7; ;
            IPSSTApp.Instance().t_QuickMenu.Left = this.Left + 1280 - 271 - 126;
        }

        private void button_Open0_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        private void button_Open1_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        private void button_Open2_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
        }

        private void button_Open3_Click(object sender, EventArgs e)
        {
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.button_Image_Load_Click(sender, e);
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
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        //IPSSTApp.Instance().m_Config.Set_Parameters();
                        //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            // MessageBox.Show("ProbeThread Start");
                            double t_v = 0;
                            //double t_v = ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //MessageBox.Show("ProbeThread PLC READ");

                            IPSSTApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[0][3] = t_v;
                            //MessageBox.Show("ProbeThread DATA_0 WRITE");

                            int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            //MessageBox.Show("ProbeThread JUDGE");
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode0 == 0)
                            {
                                Result_Image0[Capture_Count[Cam_Num]] = null;
                                Result_Image0[Capture_Count[Cam_Num]] = new Bitmap(640, 480);
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;

                                //if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode0 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode0 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode0 = 1;
                                }
                            }
                            //MessageBox.Show("ProbeThread NG LOG");
                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (ctr_PLC1.m_threads_Check)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                                {
                                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                    }
                                    else
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                    }
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                }
                                //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                                //{
                                //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                                //}
                            }
                            //MessageBox.Show("ProbeThread SEND Message");
                            // String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image0[Capture_Count[Cam_Num]].Clone(), t_Judge);
                            IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");
                            //MessageBox.Show("ProbeThread ADD LOG");
                            ctr_Manual1.Run_SW[Cam_Num].Stop();
                            IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
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
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            //IPSSTApp.Instance().m_Config.Set_Parameters();
                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            double t_v = 0;
                            //ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            IPSSTApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[0][3] = t_v;

                            int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode1 == 0)
                            {
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;
                                Result_Image1[Capture_Count[Cam_Num]] = null;
                                Result_Image1[Capture_Count[Cam_Num]] = new Bitmap(640, 480);

                                //if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode1 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode1 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode1 = 1;
                                }
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (ctr_PLC1.m_threads_Check)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                                {
                                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                    }
                                    else
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                    }
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                }
                                //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                                //{
                                //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                                //}
                            }
                            // String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image0[Capture_Count[Cam_Num]].Clone(), t_Judge);
                            IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");

                            ctr_Manual1.Run_SW[Cam_Num].Stop();
                            IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
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
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            //IPSSTApp.Instance().m_Config.Set_Parameters();
                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            double t_v = 0;
                            //ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            IPSSTApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[0][3] = t_v;

                            int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode2 == 0)
                            {
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;
                                Result_Image2[Capture_Count[Cam_Num]] = null;
                                Result_Image2[Capture_Count[Cam_Num]] = new Bitmap(640, 480);

                                //if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode2 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode2 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode2 = 1;
                                }
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (ctr_PLC1.m_threads_Check)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                                {
                                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                    }
                                    else
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                    }
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                }
                                //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                                //{
                                //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                                //}
                            }
                            // String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image0[Capture_Count[Cam_Num]].Clone(), t_Judge);
                            IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");
                            ctr_Manual1.Run_SW[Cam_Num].Stop();
                            IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
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
                    ctr_Manual1.Run_SW[Cam_Num].Reset();
                    ctr_Manual1.Run_SW[Cam_Num].Start();

                    if (!IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {
                    }
                    else // 검사하면 아래로
                    {
                        Thread.Sleep(150);
                        if (ctr_PLC1.send_Message[0].Count == 0 && ctr_PLC1.send_Message[1].Count == 0 && ctr_PLC1.send_Message[2].Count == 0 && ctr_PLC1.send_Message[3].Count == 0 && !ctr_PLC1.m_D_Write_check)
                        {
                            //IPSSTApp.Instance().m_Config.Set_Parameters();
                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = true;
                            double t_v = 0;
                            //ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            //Thread.Sleep(ctr_PLC1.t_Tx_Interval);
                            t_v = ctr_PLC1.PLC_D_READ("DW53" + IPSSTApp.Instance().m_Config.PLC_Station_Num.ToString("0") + (Cam_Num * 2).ToString(), 2) / 10000; // Prove#1 위치
                            IPSSTApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[0][3] = t_v;

                            int Judge = IPSSTApp.Instance().m_Config.Judge_DataSet(Cam_Num);
                            bool t_Judge = true;
                            if (Judge != 40)
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 1;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["NG"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];
                                t_Judge = false;
                            }
                            else
                            {
                                IPSSTApp.Instance().m_Config.m_Error_Flag[Cam_Num] = 0;
                                IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0]++;
                                //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["OK"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0];
                            }
                            //IPSSTApp.Instance().m_Config.ds_STATUS.Tables["AUTO COUNT"].Rows[Cam_Num]["TOTAL"] = IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 0] + IPSSTApp.Instance().m_Config.m_OK_NG_Cnt[Cam_Num, 1];

                            if (m_Result_Job_Mode3 == 0)
                            {
                                //byte[] Dst_Img = null;
                                //int width = 0, height = 0, ch = 0;
                                Result_Image3[Capture_Count[Cam_Num]] = null;
                                Result_Image3[Capture_Count[Cam_Num]] = new Bitmap(640, 480);

                                //if (IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, Cam_Num))
                                {
                                    //Result_Image0[Capture_Count[Cam_Num]] = ConvertBitmap(Dst_Img, width, height, ch);
                                    if (IPSSTApp.Instance().m_Config.NG_Log_Use && Judge != 40)
                                    {
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Time[Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[Cam_Num] = null;
                                        IPSSTApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(Cam_Num);
                                    }
                                }
                                if (IPSSTApp.Instance().m_Config.Diplay_Only_NG)
                                {
                                    if (Judge != 40)
                                    {
                                        m_Result_Job_Mode3 = 1;
                                    }
                                    if (t_cam_setting_view_mode)
                                    {
                                        m_Result_Job_Mode3 = 1;
                                    }

                                }
                                else
                                {
                                    m_Result_Job_Mode3 = 1;
                                }
                            }

                            //if (!t_Judge && ctr_PLC1.m_threads_Check)
                            //if (ctr_PLC1.m_threads_Check && Judge != 40)
                            if (ctr_PLC1.m_threads_Check)
                            {
                                if (IPSSTApp.Instance().m_Config.PLC_Pingpong_USE)
                                {
                                    if (IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num])
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = false;
                                    }
                                    else
                                    {
                                        ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "1" + Cam_Num.ToString() + "_" + Judge.ToString());
                                        IPSSTApp.Instance().m_Config.PLC_Pingpong_Check[Cam_Num] = true;
                                    }
                                }
                                else
                                {
                                    ctr_PLC1.send_Message[Cam_Num].Add("DW5" + (ctr_PLC1.m_Pingpong_Num + 1).ToString("0") + "0" + Cam_Num.ToString() + "_" + Judge.ToString());
                                }
                                //if (IPSSTApp.Instance().m_Config.PLC_Judge_view)
                                //{
                                //    add_Log("CAM" + Cam_Num.ToString() + ":" + Judge.ToString());
                                //}
                            }
                            // String filename = IPSSTApp.Instance().m_Config.Result_Image_Save(Cam_Num, (Bitmap)Capture_Image0[Capture_Count[Cam_Num]].Clone(), t_Judge);
                            IPSSTApp.Instance().m_Config.Add_Log_Data(Cam_Num, "");
                            ctr_Manual1.Run_SW[Cam_Num].Stop();
                            IPSSTApp.Instance().m_Config.m_TT[Cam_Num] = "T/T : " + ctr_Manual1.Run_SW[Cam_Num].ElapsedMilliseconds.ToString() + "ms";

                            //IPSSTApp.Instance().m_Config.m_Cam_Inspection_Check[Cam_Num] = false;
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
            //dataGridView_Setting_Value_0.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = IPSSTApp.Instance().m_Config.ds_DATA_0;

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
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(0,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView_Setting_Value_1.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = IPSSTApp.Instance().m_Config.ds_DATA_1;

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
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(1,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView_Setting_Value_2.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = IPSSTApp.Instance().m_Config.ds_DATA_2;

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
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(2,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView_Setting_Value_3.ReadOnly = true;
            if (t_ColIndex == 2)
            {
                DataSet DS = IPSSTApp.Instance().m_Config.ds_DATA_3;

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
                IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_CAM_Offset(3,
                    CAM_Offset1, CAM_Offset2, CAM_Offset3, CAM_Offset4, CAM_Offset5, CAM_Offset6, CAM_Offset7, CAM_Offset8, CAM_Offset9, CAM_Offset10
                    , CAM_Offset11, CAM_Offset12, CAM_Offset13, CAM_Offset14, CAM_Offset15, CAM_Offset16, CAM_Offset17, CAM_Offset18, CAM_Offset19, CAM_Offset20
                    , CAM_Offset21, CAM_Offset22, CAM_Offset23, CAM_Offset24, CAM_Offset25, CAM_Offset26, CAM_Offset27, CAM_Offset28, CAM_Offset29, CAM_Offset30
                    , CAM_Offset31, CAM_Offset32, CAM_Offset33, CAM_Offset34, CAM_Offset35, CAM_Offset36, CAM_Offset37, CAM_Offset38, CAM_Offset39, CAM_Offset40
                    );
            }
        }

        private void dataGridView_Setting_Value_0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
            }
        }

        private void dataGridView_Setting_Value_1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
            }
        }

        private void dataGridView_Setting_Value_2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
            }
        }

        private void dataGridView_Setting_Value_3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                AutoClosingMessageBox.Show("검사중 값 변경 불가! 검사 정지 후 가능 ", "Notice", 2000);
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
                long HDPercentageUsed = 100 - (100 * a.AvailableFreeSpace / a.TotalSize);
                if (HDPercentageUsed > 99)
                {
                    ret = false;
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
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void pictureBox_Setting_0_DragDrop(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                }
                return;
            }

            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;

            foreach (string pic in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                if (pictureBox_Setting_0.Image != null)
                {
                    pictureBox_Setting_0.Image = null;
                }
                pictureBox_Setting_0.BackgroundImage = null;
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
                ctr_Manual1.propertyGrid1.SelectedObject = imageInfo;
                ctr_Manual1.propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[ctr_Manual1.m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = ctr_Manual1.BmpToArray(t_Image);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = ctr_Manual1.BmpToArray(grayImage);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = ctr_Manual1.BmpToArray(t_Image);
                    if (ctr_Manual1.m_Selected_Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
                if (ctr_Manual1.pictureBox_Manual.Image != null)
                {
                    ctr_Manual1.button_Manual_Inspection_Click(sender, e);
                }
            }
        }

        private void pictureBox_Setting_1_DragEnter(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void pictureBox_Setting_1_DragDrop(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                }
                return;
            }

            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;

            foreach (string pic in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                if (pictureBox_Setting_1.Image != null)
                {
                    pictureBox_Setting_1.Image = null;
                }
                pictureBox_Setting_1.BackgroundImage = null;
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
                ctr_Manual1.propertyGrid1.SelectedObject = imageInfo;
                ctr_Manual1.propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[ctr_Manual1.m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = ctr_Manual1.BmpToArray(t_Image);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = ctr_Manual1.BmpToArray(grayImage);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = ctr_Manual1.BmpToArray(t_Image);
                    if (ctr_Manual1.m_Selected_Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
                if (ctr_Manual1.pictureBox_Manual.Image != null)
                {
                    ctr_Manual1.button_Manual_Inspection_Click(sender, e);
                }
            }
        }

        private void pictureBox_Setting_2_DragEnter(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void pictureBox_Setting_2_DragDrop(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                }
                return;
            }

            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = false;

            foreach (string pic in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                if (pictureBox_Setting_2.Image != null)
                {
                    pictureBox_Setting_2.Image = null;
                }
                pictureBox_Setting_2.BackgroundImage = null;
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
                ctr_Manual1.propertyGrid1.SelectedObject = imageInfo;
                ctr_Manual1.propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[ctr_Manual1.m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = ctr_Manual1.BmpToArray(t_Image);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = ctr_Manual1.BmpToArray(grayImage);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = ctr_Manual1.BmpToArray(t_Image);
                    if (ctr_Manual1.m_Selected_Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
                if (ctr_Manual1.pictureBox_Manual.Image != null)
                {
                    ctr_Manual1.button_Manual_Inspection_Click(sender, e);
                }
            }
        }

        private void pictureBox_Setting_3_DragEnter(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void pictureBox_Setting_3_DragDrop(object sender, DragEventArgs e)
        {
            if (IPSSTApp.Instance().m_Config.m_Check_Inspection_Mode && IPSSTApp.Instance().m_mainform.m_Start_Check && !IPSSTApp.Instance().m_mainform.Force_close)
            {
                if (IPSSTApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                }
                else if (IPSSTApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                }
                return;
            }

            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = false;
            IPSSTApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;

            foreach (string pic in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                if (pictureBox_Setting_3.Image != null)
                {
                    pictureBox_Setting_3.Image = null;
                }
                pictureBox_Setting_3.BackgroundImage = null;
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
                ctr_Manual1.propertyGrid1.SelectedObject = imageInfo;
                ctr_Manual1.propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24)
                {
                    if (IPSSTApp.Instance().m_Config.m_Cam_Kind[ctr_Manual1.m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = ctr_Manual1.BmpToArray(t_Image);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, ctr_Manual1.m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = ctr_Manual1.BmpToArray(grayImage);

                        if (ctr_Manual1.m_Selected_Cam_Num == 0)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }
                        else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                        {
                            IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                }
                else
                {
                    byte[] arr = ctr_Manual1.BmpToArray(t_Image);
                    if (ctr_Manual1.m_Selected_Cam_Num == 0)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 1)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 2)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                    else if (ctr_Manual1.m_Selected_Cam_Num == 3)
                    {
                        IPSSTApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, ctr_Manual1.m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
                if (ctr_Manual1.pictureBox_Manual.Image != null)
                {
                    ctr_Manual1.button_Manual_Inspection_Click(sender, e);
                }
            }
        }
    }
}
