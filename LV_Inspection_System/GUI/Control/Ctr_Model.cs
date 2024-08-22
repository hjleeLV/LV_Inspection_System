using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using LV_Inspection_System.GUI;
using OfficeOpenXml;
using System.Threading;
using Microsoft.VisualBasic;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Model : UserControl
    {
        public bool m_checkNew = false;

        public Ctr_Model()
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
                    label6.Text = "모델 :";
                    label1.Text = "언어 :";
                    groupBox4.Text = "저장된 모델";
                    label7.Text = "모델 리스트 :";
                    cmdNew.Text = "모델 생성";
                    cmdSave.Text = "모델 저장";
                    cmdLoad.Text = "불러오기";
                    cmdDelete.Text = "모델 삭제";
                    button_BACKUP.Text = "백업";
                    button_Restore.Text = "복원";

                    comboBox_Language.Items.Clear();
                    comboBox_Language.Items.Add("KOREAN");
                    comboBox_Language.Items.Add("ENGLISH");
                    comboBox_Language.Items.Add("CHINESE");
                    comboBox_Language.Text = "KOREAN";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    label6.Text = "Model :";
                    label1.Text = "Lenguage :";
                    groupBox4.Text = "Saved Model";
                    label7.Text = "Model List :";
                    cmdNew.Text = "Create";
                    cmdSave.Text = "Save";
                    cmdLoad.Text = "Load";
                    cmdDelete.Text = "Delete";
                    button_BACKUP.Text = "Backup";
                    button_Restore.Text = "Restore";
                    comboBox_Language.Items.Clear();
                    comboBox_Language.Items.Add("KOREAN");
                    comboBox_Language.Items.Add("ENGLISH");
                    comboBox_Language.Items.Add("CHINESE");
                    comboBox_Language.Text = "ENGLISH";
                }
                else if (value == 2 && m_Language != value)
                {// 영어
                    label6.Text = "模型 :";
                    label1.Text = "伦古奇 :";
                    groupBox4.Text = "已保存的模型";
                    label7.Text = "型号列表 :";
                    cmdNew.Text = "创建";
                    cmdSave.Text = "救";
                    cmdLoad.Text = "负荷";
                    cmdDelete.Text = "删除";
                    button_BACKUP.Text = "备份";
                    button_Restore.Text = "恢复";
                    comboBox_Language.Items.Clear();
                    comboBox_Language.Items.Add("KOREAN");
                    comboBox_Language.Items.Add("ENGLISH");
                    comboBox_Language.Items.Add("CHINESE");
                    comboBox_Language.Text = "CHINESE";
                }
                ctr_History1.m_SetLanguage = value;
                m_Language = value;
            }
        }

        public void cmdNew_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 모델 생성을 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't create model during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法创建模型！", "Notice", 2000);
                }
                return;
            }

            if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("관리자 로그인후 모델 생성을 할 수 있습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Please login!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("请登录！", "Notice", 2000);
                }
                return;
            }
            // prompt for a new name
            Frm_Model_Name frm = new Frm_Model_Name();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (frm.ModelName == "")
                {
                    m_checkNew = false;
                    return;
                }

                string pre_model_name = LVApp.Instance().m_Config.m_Model_Name;
                //save it
                string shortname = frm.ModelName;
                bool CreateMethod = frm.CreateMethod;

                LVApp.Instance().m_Config.m_Model_Name = shortname;
                Properties.Settings.Default.Last_Model_Name = shortname;
                LVApp.Instance().m_mainform.TitleBar.TitleBarCaption = "Vision Inspection System" + " [Current Model : " + Properties.Settings.Default.Last_Model_Name.ToString() + "]";
                LVApp.Instance().m_mainform.Refresh();

                if (cmbModels.FindStringExact(shortname) >= 0) // 기존 모델일때
                {
                    DebugLogger.Instance().LogWarning("There is same model.");
                    cmbModels.SelectedIndex = cmbModels.FindStringExact(shortname);
                    return;
                } else
                {
                    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                    dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + shortname);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    if (pre_model_name != "")
                    {
                        string sourcePath = LVApp.Instance().excute_path + "\\Models\\" + pre_model_name;
                        if (!CreateMethod) // 신규 생성일때
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                sourcePath = LVApp.Instance().excute_path + "\\Add-ins\\Basic_Model";
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                            {//영어
                                sourcePath = LVApp.Instance().excute_path + "\\Add-ins\\Basic_Model_En";
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                            {//중국어
                                sourcePath = LVApp.Instance().excute_path + "\\Add-ins\\Basic_Model_Ch";
                            }
                        }
                        string targetPath = LVApp.Instance().excute_path + "\\Models\\" + shortname;
                        if (System.IO.Directory.Exists(sourcePath))
                        {
                            copyDirectory(sourcePath, targetPath);
                        }
                        sourcePath = LVApp.Instance().excute_path + "\\Models\\" + shortname + "\\" + pre_model_name + ".xlsx";
                        if (!CreateMethod) // 신규 생성일때
                        {
                            sourcePath = LVApp.Instance().excute_path + "\\Models\\" + shortname + "\\Basic_Model.xlsx";
                        }
                        targetPath = LVApp.Instance().excute_path + "\\Models\\" + shortname + "\\" + shortname + ".xlsx";
                            
                        if (File.Exists(sourcePath))
                        {
                            System.IO.File.Move(sourcePath, targetPath);
                        } else
                        {
                            LVApp.Instance().m_Config.Exel_basic_Setting_Create();
                        }
                    } else
                    {
                        string sourcePath = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            sourcePath = LVApp.Instance().excute_path + "\\Add-ins\\Basic_Model";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            sourcePath = LVApp.Instance().excute_path + "\\Add-ins\\Basic_Model_En";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            sourcePath = LVApp.Instance().excute_path + "\\Add-ins\\Basic_Model_Ch";
                        }
                        string targetPath = LVApp.Instance().excute_path + "\\Models\\" + shortname;

                        dir = new DirectoryInfo(sourcePath);
                        if (dir.Exists)
                        {
                            copyDirectory(sourcePath, targetPath);
                            sourcePath = LVApp.Instance().excute_path + "\\Models\\" + shortname + "\\Basic_Model.xlsx";
                            targetPath = LVApp.Instance().excute_path + "\\Models\\" + shortname + "\\" + shortname + ".xlsx";

                            if (File.Exists(sourcePath))
                            {
                                System.IO.File.Move(sourcePath, targetPath);
                            }
                        }
                        else
                        {
                            LVApp.Instance().m_Config.Exel_basic_Setting_Create();
                        }
                    }

                    read_model_list();
                }

                //if (!CreateMethod) // 신규 생성일때
                {
                    cmdSave_Click(sender, e);
                    LVApp.Instance().m_Ctr_Mysql.DB_Create();
                }
                //cmdSave_Click(sender, e);
                //cmdLoad_Click(sender, e);
            }
            else
            {
                m_checkNew = false;
                return;
            }
            for (int i = 0; i < cmbModels.Items.Count; i++)
            {
                if (cmbModels.Items[i].ToString() == frm.ModelName)
                {
                    cmbModels.SelectedIndex = i;
                }
            }
            m_checkNew = true;
            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
            LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                AutoClosingMessageBox.Show("생성 완료!", "Notice", 1000);
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                AutoClosingMessageBox.Show("Created!", "Notice", 1000);
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                AutoClosingMessageBox.Show("创建!", "Notice", 1000);
            }
        }

        public void copyDirectory(string strSource, string strDestination)
        {
            try
            {
                if (!Directory.Exists(strDestination))
                {
                    Directory.CreateDirectory(strDestination);
                }
                else
                {
                    DirectoryInfo tempDirInfo = new DirectoryInfo(strDestination);
                    foreach (DirectoryInfo di in tempDirInfo.GetDirectories())
                    {
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                            {
                                fi.Attributes = FileAttributes.Normal;
                            }
                        }
                    }
                    tempDirInfo.Delete(true);
                }

                //DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name);
                //// 폴더가 존재하지 않으면
                //if (dir.Exists == false)
                //{
                //    // 새로 생성합니다.
                //    dir.Create();
                //}

                DirectoryInfo dir = new DirectoryInfo(strDestination);
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                DirectoryInfo dirInfo = new DirectoryInfo(strSource);
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo tempfile in files)
                {
                    tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
                }

                DirectoryInfo[] directories = dirInfo.GetDirectories();
                foreach (DirectoryInfo tempdir in directories)
                {
                    copyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
                }
            }
            catch
            {

            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 모델 삭제를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't delete model during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法删除模型！", "Notice", 2000);
                }
                return;
            }

            if (!LVApp.Instance().m_Config.m_Administrator_Password_Flag)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("관리자 로그인후 모델 삭제를 할 수 있습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Please login!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("请登录！", "Notice", 2000);
                }
                return;
            }
            try
            {
                string shortname = lstModels.SelectedItem.ToString();
                if (shortname.ToLower().Contains("default"))
                {
                    MessageBox.Show("Cannot delete this profile!");
                }
                else
                {
                    string msg = string.Empty;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        msg = "[" + shortname + "]을 삭제 하시겠습니까?";
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        msg = "Do you want to delete the [" + shortname + "]?";
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        msg = "是否要删除 [" + shortname + "]?";
                    }
                    if (MessageBox.Show(msg, " Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        LVApp.Instance().m_Ctr_Mysql.DB_Drop();

                        DirectoryInfo tempDirInfo = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + shortname);
                        
                        if (tempDirInfo.Exists == true)
                        {
                            foreach (DirectoryInfo di in tempDirInfo.GetDirectories())
                            {
                                foreach (FileInfo fi in di.GetFiles())
                                {
                                    if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                                    {
                                        fi.Attributes = FileAttributes.Normal;
                                    }
                                }
                            }
                            LVApp.Instance().m_Config.m_Model_Name = "";
                            Properties.Settings.Default.Last_Model_Name = "";
                            tempDirInfo.Delete(true);
                        }
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            MessageBox.Show("[" + shortname + "]" + " 모델이 삭제 되었습니다.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            MessageBox.Show("[" + shortname + "]" + " deleted.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            msg = "删除 [" + shortname + "]";
                        }

                    }
                    read_model_list();
                }
                LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        public void read_model_list()
        {
            try
            {
                if (!LVApp.Instance().m_mainform.m_Start_Check)
                {
                    lstModels.SelectedIndex = -1;
                }
                cmbModels.ResetText();
                cmbModels.Items.Clear();
                lstModels.Items.Clear();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(LVApp.Instance().excute_path + "\\Models");

                if (!di.Exists)
                {
                    return;
                }
                foreach (System.IO.DirectoryInfo f in di.GetDirectories())
                {
                    cmbModels.Items.Add(f.Name);
                    lstModels.Items.Add(f.Name);
                    //MessageBox.Show(f.Name);
                }
                if (cmbModels.Items.Count > 0)
                {
                    if (Properties.Settings.Default.Last_Model_Name != "")
                    {
                        LVApp.Instance().m_Config.m_Model_Name = Properties.Settings.Default.Last_Model_Name;
                        int t_num = cmbModels.FindStringExact(Properties.Settings.Default.Last_Model_Name);
                        //cmbModels.SelectedIndex = t_num;
                        if (cmbModels.SelectedIndex != t_num)
                        {
                            lstModels.SelectedIndex = t_num;
                        }
                        if (t_num < 0)
                        {
                            LVApp.Instance().m_Config.m_Model_Name = "";
                        }
                    }
                    else
                    {
                        lstModels.SelectedIndex = 0;
                    }
                }
                else
                {
                    LVApp.Instance().m_Config.m_Model_Name = "";
                    Properties.Settings.Default.Last_Model_Name = "";
                }
                LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
                DebugLogger.Instance().LogRecord("Model loaded!");
                LVApp.Instance().m_mainform.TitleBar.TitleBarCaption = "Vision Inspection System" + " [Current Model : " + Properties.Settings.Default.Last_Model_Name.ToString() + "]";
            }
            catch (System.Exception ex)
            {
                DebugLogger.Instance().LogError(ex);	
            }
        }

        private void cmbModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 모델 변경를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change model during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法更改模型!", "Notice", 2000);
                }
                cmbModels.SelectedIndex = lstModels.SelectedIndex;
                return;
            }

            if (LVApp.Instance().m_Config.m_Model_Name != cmbModels.Items[cmbModels.SelectedIndex].ToString())
            {
                if (LVApp.Instance().m_mainform.m_Start_Check)
                {
                    //if (LVApp.Instance().m_Config.m_Model_Name != cmbModels.Items[cmbModels.SelectedIndex].ToString())
                    {
                        string msg = string.Empty;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            msg = "모델을 변경 하시겠습니까?";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            msg = "Do you change the model?";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            msg = "您是否更改了模型?";
                        }
                        if (MessageBox.Show(msg, " Model change", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            LVApp.Instance().m_mainform.Inspection_Thread_Stop();
                            LVApp.Instance().m_Config.m_Model_Name = cmbModels.Items[cmbModels.SelectedIndex].ToString();
                            LVApp.Instance().m_Ctr_Mysql.DB_Create();
                            bool t_space_check = true;
                            if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                            {
                                t_space_check = LVApp.Instance().m_mainform.Check_HD_available(LVApp.Instance().excute_path);
                            }
                            else
                            {
                                t_space_check = LVApp.Instance().m_mainform.Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                            }
                            if (t_space_check)
                            {
                                LVApp.Instance().m_Config.Create_Save_Folders();
                            }
                            Properties.Settings.Default.Last_Model_Name = LVApp.Instance().m_Config.m_Model_Name;
                            Properties.Settings.Default.Save();
                            LVApp.Instance().m_mainform.TitleBar.TitleBarCaption = "Vision Inspection System" + " [Current Model : " + Properties.Settings.Default.Last_Model_Name.ToString() + "]";
                            LVApp.Instance().m_mainform.Refresh();
                            if (lstModels.SelectedIndex != cmbModels.SelectedIndex)
                            {
                                lstModels.SelectedIndex = cmbModels.SelectedIndex;
                                read_model_list();
                            }


                            //LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
                            // 여기에 로드할 것들 넣을것
                            LVApp.Instance().m_mainform.ctr_PLC1.button_LOAD_Click(sender, e);
                            Thread.Sleep(100);
                            LVApp.Instance().m_mainform.ctr_Log1.button_LOGLOAD_Click(sender, e);
                            Thread.Sleep(100);
                            //LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
                            //LVApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);
                            LVApp.Instance().m_Config.Load_Judge_Data();                     // 저장 변수 불러오기    
                            Thread.Sleep(100);
                            //LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
                            //LVApp.Instance().m_mainform.Camera_Initialize();
                            //Thread.Sleep(100);
                            //LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;

                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
                                //LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonDisconnect_Click(sender, e);
                            }
                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
                                //LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonDisconnect_Click(sender, e);
                            }
                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
                                //LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonDisconnect_Click(sender, e);
                            }
                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
                                //LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonDisconnect_Click(sender, e);
                            }

                            LVApp.Instance().m_mainform.Camera_Initialize();
                            Thread.Sleep(100);
                            LVApp.Instance().m_mainform.ctr_Admin_Param1.button_LOAD_Click(sender, e);
                            for (int i = 1; i < 4; i++)
                            {
                                for (int kk = 1; kk < 41; kk++)
                                {
                                    LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[i, kk - 1] = false;
                                }
                            }

                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                            {
                                LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                                LVApp.Instance().m_mainform.ctr_ROI4.button_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_ROI4.load_check = false;
                                LVApp.Instance().m_mainform.ctr_ROI4.Fit_Size();
                                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                                //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = 0;
                            }
                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                            {
                                LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                                LVApp.Instance().m_mainform.ctr_ROI3.button_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_ROI3.load_check = false;
                                LVApp.Instance().m_mainform.ctr_ROI3.Fit_Size();
                                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                                //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = 0;
                            }
                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                            {
                                LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                                LVApp.Instance().m_mainform.ctr_ROI2.button_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_ROI2.load_check = false;
                                LVApp.Instance().m_mainform.ctr_ROI2.Fit_Size();
                                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                                //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = 0;
                            }
                            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                            {
                                LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                                LVApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                                LVApp.Instance().m_mainform.ctr_ROI1.load_check = false;
                                LVApp.Instance().m_mainform.ctr_ROI1.Fit_Size();
                                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                                //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 0;
                            }

                            LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
                        }
                        else
                        { // 아니요 선택
                            cmbModels.SelectedIndex = cmbModels.FindStringExact(LVApp.Instance().m_Config.m_Model_Name);
                            lstModels.SelectedIndex = cmbModels.SelectedIndex;
                        }
                        LVApp.Instance().m_Config.ds_STATUS.Tables[2].Rows[0][1] = LVApp.Instance().m_Config.m_Model_Name;
                        LVApp.Instance().m_Ctr_Mysql.DB_Table2MySQL(LVApp.Instance().m_Config.ds_STATUS.Tables[2], "Operating");
                        //LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
                        //LVApp.Instance().m_mainform.Inspection_Thread_Stop();
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            AutoClosingMessageBox.Show("모델 불러옴", "LOAD", 2000);
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            AutoClosingMessageBox.Show("Model loaded.", "LOAD", 2000);
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            AutoClosingMessageBox.Show("已加载模型.", "LOAD", 2000);
                        }
                        
                        LVApp.Instance().m_mainform.neoTabWindow_MAIN.SelectedIndex = 5;
                        LVApp.Instance().m_mainform.Inspection_Thread_Start();
                        //LVApp.Instance().m_mainform.ctr_Display_1.Update_Display();
                    }
                }
                else
                {//  시작할때 로딩
                    LVApp.Instance().m_Config.m_Model_Name = cmbModels.Items[cmbModels.SelectedIndex].ToString();
                    bool t_space_check = true;
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                    {
                        t_space_check = LVApp.Instance().m_mainform.Check_HD_available(LVApp.Instance().excute_path);
                    }
                    else
                    {
                        t_space_check = LVApp.Instance().m_mainform.Check_HD_available(LVApp.Instance().m_Config.m_Log_Save_Folder);
                    }
                    if (t_space_check)
                    {
                        LVApp.Instance().m_Config.Create_Save_Folders();
                    }
                    Properties.Settings.Default.Last_Model_Name = LVApp.Instance().m_Config.m_Model_Name;
                    LVApp.Instance().m_mainform.TitleBar.TitleBarCaption = "Vision Inspection System" + " [Current Model : " + Properties.Settings.Default.Last_Model_Name.ToString() + "]";
                    LVApp.Instance().m_mainform.Refresh();
                    if (lstModels.SelectedIndex != cmbModels.SelectedIndex)
                    {
                        lstModels.SelectedIndex = cmbModels.SelectedIndex;
                        read_model_list();
                    }

                    // 여기에 로드할 것들 넣을것
                    LVApp.Instance().m_mainform.Camera_Initialize();
                    Thread.Sleep(100);
                    LVApp.Instance().m_mainform.ctr_Log1.button_LOGLOAD_Click(sender, e);
                    Thread.Sleep(100);
                    //LVApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);
                    LVApp.Instance().m_Config.Load_Judge_Data();                     // 저장 변수 불러오기    
                    Thread.Sleep(100);


                    LVApp.Instance().m_mainform.ctr_Admin_Param1.button_LOAD_Click(sender, e);
                    for (int i = 1; i < 4; i++)
                    {
                        for (int kk = 1; kk < 41; kk++)
                        {
                            LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[i, kk - 1] = false;
                        }
                    }

                    //LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING.SelectedIndex = 0;
                    if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
                    {
                        LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                        LVApp.Instance().m_mainform.ctr_ROI4.button_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_ROI4.load_check = false;
                        LVApp.Instance().m_mainform.ctr_ROI4.Fit_Size();
                        //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                        //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = 0;
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
                    {
                        LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                        LVApp.Instance().m_mainform.ctr_ROI3.button_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_ROI3.load_check = false;
                        LVApp.Instance().m_mainform.ctr_ROI3.Fit_Size();
                        //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                        //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = 0;
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
                    {
                        LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                        LVApp.Instance().m_mainform.ctr_ROI2.button_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_ROI2.load_check = false;
                        LVApp.Instance().m_mainform.ctr_ROI2.Fit_Size();
                        //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                        //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = 0;
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
                    {
                        LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                        LVApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                        LVApp.Instance().m_mainform.ctr_ROI1.load_check = false;
                        LVApp.Instance().m_mainform.ctr_ROI1.Fit_Size();
                        //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                        //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 0;
                    }
                    LVApp.Instance().m_mainform.ctr_PLC1.button_LOAD_Click(sender, e);
                    LVApp.Instance().m_mainform.ctr_PLC1.btnOpen_Click(sender, e);
                }
                LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
                LVApp.Instance().m_Config.ds_STATUS.Tables[2].Rows[0][1] = LVApp.Instance().m_Config.m_Model_Name;
                LVApp.Instance().m_Ctr_Mysql.DB_Operating(LVApp.Instance().m_Config.ds_STATUS.Tables[2], "Information");
            }
        }

        private void lstModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 모델 변경를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change model during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法更改模型!", "Notice", 2000);
                }
                lstModels.SelectedIndex = cmbModels.SelectedIndex;
                return;
            }

            if (!LVApp.Instance().m_mainform.m_Start_Check)
            {
                cmbModels.SelectedIndex = -1;
            }
            if (cmbModels.SelectedIndex != lstModels.SelectedIndex && cmbModels.Items.Count > 0 && lstModels.Items.Count > 0)
            {
                cmbModels.SelectedIndex = lstModels.SelectedIndex;
            }
        }

        public void cmdSave_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 모델 저장을 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't save model during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法更改模型!", "Notice", 2000);
                }
                return;
            }

            Properties.Settings.Default.Title = textBox_Title.Text;
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.Title.Length > 0)
            {
                LVApp.Instance().m_mainform.label_Title.Text = Properties.Settings.Default.Title;
                textBox_Title.Text = Properties.Settings.Default.Title;
                LVApp.Instance().m_mainform.button_Main_View.Visible = false;
            }
            else
            {
                LVApp.Instance().m_mainform.button_Main_View.Visible = true;
                LVApp.Instance().m_mainform.label_Title.Text = textBox_Title.Text = String.Empty;
            }

            LVApp.Instance().m_Config.Save_Judge_Data();
            //LVApp.Instance().m_mainform.ctr_Parameters1.button_PARASAVE_Click(sender, e);
            LVApp.Instance().m_mainform.ctr_PLC1.button_SAVE_Click(sender, e);
            LVApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);

            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                LVApp.Instance().m_mainform.ctr_ROI4.button_SAVE_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_SAVE_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                LVApp.Instance().m_mainform.ctr_ROI3.button_SAVE_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_SAVE_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                LVApp.Instance().m_mainform.ctr_ROI2.button_SAVE_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_SAVE_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                LVApp.Instance().m_mainform.ctr_ROI1.button_SAVE_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_SAVE_Click(sender, e);
            }
            LVApp.Instance().m_mainform.ctr_Admin_Param1.button_SAVE_Click(sender, e);
            //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            //{//한국어
            //    AutoClosingMessageBox.Show("모델 백업이 [" + targetPath + "]로 완료 되었습니다.", "Notice", 2000);
            //}
            //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            //{//영어
            //    AutoClosingMessageBox.Show("Completed model backup at [" + targetPath + "]", "Notice", 2000);
            //}
            if (sender.GetType().Name == "Button")
            {
                
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("저장 완료.", "SAVE", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Save completed.", "SAVE", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("保存完成.", "SAVE", 2000);
                }
            }
        }

        public void cmdLoad_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 모델 불러오기를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't load model during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法更改模型.", "SAVE", 2000);
                }
                return;
            }

            LVApp.Instance().m_Config.Load_Judge_Data();
            //LVApp.Instance().m_mainform.ctr_Parameters1.button_PARALOAD_Click(sender, e);
            LVApp.Instance().m_mainform.ctr_PLC1.button_LOAD_Click(sender, e);
            LVApp.Instance().m_mainform.ctr_Log1.button_LOGLOAD_Click(sender, e);
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonStop_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonStop_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonStop_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonStop_Click(sender, e);
            }
            LVApp.Instance().m_mainform.ctr_Admin_Param1.button_LOAD_Click(sender, e);
            for (int i = 1; i < 4; i++)
            {
                for (int kk = 1; kk < 41; kk++)
                {
                    LVApp.Instance().m_AI_Pro.Flag_Model_Loaded[i, kk - 1] = false;
                }
            }

            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 4)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 3;
                LVApp.Instance().m_mainform.ctr_ROI4.button_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButton_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_ROI4.load_check = false;
                LVApp.Instance().m_mainform.ctr_ROI4.Fit_Size();
                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                //LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = 0;
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 3)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 2;
                LVApp.Instance().m_mainform.ctr_ROI3.button_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButton_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_ROI3.load_check = false;
                LVApp.Instance().m_mainform.ctr_ROI3.Fit_Size();
                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                //LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = 0;
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 2)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 1;
                LVApp.Instance().m_mainform.ctr_ROI2.button_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButton_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_ROI2.load_check = false;
                LVApp.Instance().m_mainform.ctr_ROI2.Fit_Size();
                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                //LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = 0;
            }
            if (LVApp.Instance().m_Config.m_Cam_Total_Num >= 1)
            {
                LVApp.Instance().m_Config.ROI_Cam_Num = 0;
                LVApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButton_LOAD_Click(sender, e);
                LVApp.Instance().m_mainform.ctr_ROI1.load_check = false;
                LVApp.Instance().m_mainform.ctr_ROI1.Fit_Size();
                //LVApp.Instance().m_Config.ROI_Selected_IDX = -1;
                //LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 0;
            }
            //LVApp.Instance().m_mainform.ctr_Admin_Param1.button_LOAD_Click(sender, e);
            //LVApp.Instance().m_Config.t_Create_Save_Folders_Enable = true;
            if (sender.GetType().Name == "Button")
            {
                //LVApp.Instance().m_mainform.Inspection_Thread_Stop();
                
                //LVApp.Instance().m_mainform.Inspection_Thread_Start();
                //LVApp.Instance().m_mainform.ctr_Display_1.Update_Display();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("불러오기 완료.", "LOAD", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Load Completed!", "LOAD", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("导入完成.", "LOAD", 2000);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("http://www.learningvision.co.kr");
        }

        private void comboBox_Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_mainform.m_Start_Check && comboBox_Language.SelectedIndex != LVApp.Instance().m_Config.m_SetLanguage)
            {
                string input = Interaction.InputBox("Input Password to change the language", "Password", "");
                //if (Interaction.InputBox("Input Password to change the language", "Password", "") != Properties.Settings.Default.PASSWD)
                if (input != "7748" && input != "9542")
                {
                    comboBox_Language.SelectedIndex = LVApp.Instance().m_Config.m_SetLanguage;
                    MessageBox.Show("Password Error");
                    return;
                }
            }

            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 언어 변경을 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't change language during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法更改模型.", "SAVE", 2000);
                }
                return;
            }

            if (comboBox_Language.SelectedIndex > -1)
            {
                LVApp.Instance().m_Config.m_SetLanguage = comboBox_Language.SelectedIndex;
                Properties.Settings.Default.Language = LVApp.Instance().m_Config.m_SetLanguage;

                LVApp.Instance().m_mainform.ctr_DataGrid1.Min_Max_Update(0);
                LVApp.Instance().m_mainform.ctr_DataGrid2.Min_Max_Update(1);
                LVApp.Instance().m_mainform.ctr_DataGrid3.Min_Max_Update(2);
                LVApp.Instance().m_mainform.ctr_DataGrid4.Min_Max_Update(3);
            }
        }

        private void button_BACKUP_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "모델을 백업 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "Do you want to backup?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "是否要备份?";
            }

            if (MessageBox.Show(msg, " BACKUP", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");

                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }
                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name);
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }
                dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param");
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    // 새로 생성합니다.
                    dir.Create();
                }

                string sourcePath = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name;
                string targetPath = LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name;

                DirectoryInfo tempDirInfo = new DirectoryInfo(targetPath);

                if (System.IO.Directory.Exists(targetPath))
                {
                    foreach (DirectoryInfo di in tempDirInfo.GetDirectories())
                    {
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                            {
                                fi.Attributes = FileAttributes.Normal;
                            }
                        }
                    }
                    tempDirInfo.Delete(true);
                }

                if (System.IO.Directory.Exists(sourcePath))
                {
                    LVApp.Instance().m_mainform.ctr_Model1.copyDirectory(sourcePath, targetPath);
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("모델 백업이 [" + targetPath + "]로 완료 되었습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Completed model backup at [" + targetPath + "]", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("已完成的模型备份，在 [" + targetPath + "]", "Notice", 2000);
                }
            }
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            read_model_list();
        }

        private void button_Restore_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "모델을 복원 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "Do you want to restore?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "是否要还原?";
            }

            if (MessageBox.Show(msg, " RESTORE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");

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

                string sourcePath = LVApp.Instance().excute_path + "\\Models_Backup\\" + LVApp.Instance().m_Config.m_Model_Name;
                string targetPath = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name;
                if (System.IO.Directory.Exists(sourcePath))
                {
                    LVApp.Instance().m_mainform.ctr_Model1.copyDirectory(sourcePath, targetPath);
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("모델 복원이 [" + targetPath + "]로 완료 되었습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Completed model restoring at [" + targetPath + "]", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("已完成模型还原，在 [" + targetPath + "]", "Notice", 2000);
                }
                cmdLoad_Click(sender, e);
            }
        }

        private void splitContainer2_SizeChanged(object sender, EventArgs e)
        {

            splitContainer2.SplitterDistance = 1;
        }

        private void textBox_Title_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Properties.Settings.Default.Title = textBox_Title.Text;
                Properties.Settings.Default.Save();

                if (Properties.Settings.Default.Title.Length > 0)
                {
                    LVApp.Instance().m_mainform.label_Title.Text = Properties.Settings.Default.Title;
                    textBox_Title.Text = Properties.Settings.Default.Title;
                    LVApp.Instance().m_mainform.button_Main_View.Visible = false;
                }
                else
                {
                    LVApp.Instance().m_mainform.button_Main_View.Visible = true;
                    LVApp.Instance().m_mainform.label_Title.Text = textBox_Title.Text = String.Empty;
                }
            }
        }
    }
}
