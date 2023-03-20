using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LV_Inspection_System.GUI;
using LV_Inspection_System.UTIL;
using LV_Inspection_System.GUI.Control;
using System.Windows.Forms;
using System.Drawing;
//using LV_Inspection_System.GUI.Control;

namespace LV_Inspection_System
{
    class LVApp
    {
        private static LVApp m_instance = null;
        public string excute_path = Environment.CurrentDirectory; // 실행되는 폴더이름
        public LV_Config m_Config;                             // 모든 설정값 및 데이터 관련 클래스
       // public Ctr_Model ctr_Model;
        //public Frm_QuickMenu t_QuickMenu;
        public Frm_Main m_mainform;
        public Frm_Help m_help;
        public Ctr_Mysql m_Ctr_Mysql;
        public Utility t_Util;

        public Ctr_Yield_Log[] m_ctr_yield = new Ctr_Yield_Log[4];
        public AI_Processing m_AI_Pro;

        public Advantech_DIO m_DIO;
        public Mil_Library m_MIL;
        public GenICam_Library m_GenICam;

        public static LVApp Instance()
        {
            if (m_instance == null)
            {
                m_instance = new LVApp();
            }
            return m_instance;
        }

        public struct Save_Images
        {
            public int _Cam_num;
            public string _Filename;
            public Bitmap _Image;
            public int _OK_NG_NONE_Flag;
        }
        public List<LVApp.Save_Images> [] SAVE_IMAGE_List = new List<Save_Images>[4];
        
        private LVApp() 
        {
            m_Config = new LV_Config();
            //t_QuickMenu = new Frm_QuickMenu();
            m_help = new Frm_Help();
            m_Ctr_Mysql = new Ctr_Mysql();
            t_Util = new Utility();
            //ctr_Model = new Ctr_Model();
            //m_Ctr_Setting = new Ctr_System_Setting();
            //m_Ctr_Model_Setting = new Ctr_Model_Setting();
            //m_Impro = new ImageProcessing();
            //m_Ctr_Auto = new Ctr_Auto();
            //m_Ctr_Manual_Tab = new Ctr_Manual_Tab();
            //m_Ctr_Offset = new Ctr_Offset();
            SAVE_IMAGE_List[0] = new List<LVApp.Save_Images>();
            SAVE_IMAGE_List[1] = new List<LVApp.Save_Images>();
            SAVE_IMAGE_List[2] = new List<LVApp.Save_Images>();
            SAVE_IMAGE_List[3] = new List<LVApp.Save_Images>();

            m_ctr_yield[0] = new Ctr_Yield_Log(); m_ctr_yield[0].m_cam_name = "CAM0";
            m_ctr_yield[1] = new Ctr_Yield_Log(); m_ctr_yield[1].m_cam_name = "CAM1";
            m_ctr_yield[2] = new Ctr_Yield_Log(); m_ctr_yield[2].m_cam_name = "CAM2";
            m_ctr_yield[3] = new Ctr_Yield_Log(); m_ctr_yield[3].m_cam_name = "CAM3";

            m_AI_Pro = new AI_Processing();
            m_DIO = new Advantech_DIO();
            m_MIL = new Mil_Library();
            m_GenICam = new GenICam_Library();
            m_GenICam.Param_Initialize();
        }
    }
}
