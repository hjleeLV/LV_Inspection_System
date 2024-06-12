using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Log : UserControl
    {
        public Ctr_Log()
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
                    groupBox1.Text = "이미지 저장관련[Image Logging]";
                    label4.Text = "저장할 카메라 선택";
                    label11.Text = "저장방법";
                    label1.Text = "파일포맷";
                    label7.Text = "로컬 통합 저장 폴더";
                    label17.Text = "서버 이미지 저장 폴더";
                    button_Folder_setting.Text = "폴더 선택";
                    button_OPEN.Text = "폴더 열기";
                    button_Folder_setting2.Text = "폴더 선택";
                    button_OPEN2.Text = "폴더 열기";
                    button_DELETE.Text = "폴더 삭제";
                    groupBox2.Text = "데이터 저장관련[Data Logging]";
                    label14.Text = "시스템 로그 및 데이터 로그";
                    checkBox_LOGUSE.Text = "사용유무[Usase]";
                    label6.Text = "저장수";
                    label5.Text = "개/파일";
                    label16.Text = "저장기간";
                    label15.Text = "일";
                    checkBox_Display.Text = "카메라 실시간 뷰";
                    checkBox_TextView.Text = "영상처리 결과 뷰";
                    checkBox_Debugging.Text = "알고리즘 디버깅 이미지 저장";
                    checkBox_NG_Display.Text = "NG 영상만 display";
                    button_LOGFOLDEROPEN.Text = "폴더 열기";
                    button_LOGFOLDERDELETE.Text = "폴더 삭제";
                    button_LOGLOAD.Text = "불러오기";
                    button_LOGSAVE.Text = "적용 및 저장";
                    label9.Text = "그래프";
                    label8.Text = "초 마다 저장";
                    label13.Text = "로컬 Data 저장 폴더";
                    button_Data_Folder_setting.Text = "폴더 선택";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    groupBox1.Text = "Image Logging";
                    label4.Text = "Select camera for save";
                    label11.Text = "Item";
                    label1.Text = "Format";
                    label7.Text = "Local Total Folder";
                    label17.Text = "Server Image Folder";
                    button_Folder_setting.Text = "Select Folder";
                    button_OPEN.Text = "Folder Open";
                    button_Folder_setting2.Text = "Select Folder";
                    button_OPEN2.Text = "Folder Open";
                    button_DELETE.Text = "Folder Delete";
                    groupBox2.Text = "Data Logging";
                    label14.Text = "Log for System and Data";
                    checkBox_LOGUSE.Text = "Usage";
                    label6.Text = "Count";
                    label5.Text = "ea/file";
                    label16.Text = "Duration";
                    label15.Text = "Day";
                    checkBox_Display.Text = "Real-time View Mode";
                    checkBox_TextView.Text = "Result View Mode";
                    checkBox_Debugging.Text = "Alg. Debugging Mode";
                    checkBox_NG_Display.Text = "Display only NG image";
                    button_LOGFOLDEROPEN.Text = "Folder Open";
                    button_LOGFOLDERDELETE.Text = "Folder Delete";
                    button_LOGLOAD.Text = "Load";
                    button_LOGSAVE.Text = "Apply and Save";
                    label9.Text = "Graph";
                    label8.Text = "sec/update";
                    label13.Text = "Data Folder";
                    button_Data_Folder_setting.Text = "Select Folder";
                }
                else if (value == 2 && m_Language != value)
                {// 중국어
                    groupBox1.Text = "图像日志记录";
                    label4.Text = "选择要保存的摄像机";
                    label11.Text = "项目";
                    label1.Text = "格式";
                    label7.Text = "本地集成存储文件夹";
                    label17.Text = "服务器映像文件夹";
                    button_Folder_setting.Text = "选择文件夹";
                    button_OPEN.Text = "文件夹打开";
                    button_Folder_setting2.Text = "选择文件夹";
                    button_OPEN2.Text = "文件夹打开";
                    button_DELETE.Text = "文件夹删除";
                    groupBox2.Text = "数据日志记录";
                    label14.Text = "系统和数据的日志";
                    checkBox_LOGUSE.Text = "使用";
                    label6.Text = "计数";
                    label5.Text = "ea/文件";
                    label16.Text = "时间";
                    label15.Text = "一天";
                    checkBox_Display.Text = "实时查看模式";
                    checkBox_TextView.Text = "结果视图模式";
                    checkBox_Debugging.Text = "Alg. 调试模式";
                    checkBox_NG_Display.Text = "仅显示 NG 图像";
                    button_LOGFOLDEROPEN.Text = "文件夹打开";
                    button_LOGFOLDERDELETE.Text = "文件夹删除";
                    button_LOGLOAD.Text = "负荷";
                    button_LOGSAVE.Text = "应用和保存";
                    label9.Text = "产量图";
                    label8.Text = "秒/更新";
                    label13.Text = "本地数据存储文件夹";
                    button_Data_Folder_setting.Text = "选择文件夹";
                }
                m_Language = value;
            }
        }

        private void button_OPEN_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_Folder.Text.Length > 1)
                {
                    LVApp.Instance().m_Config.m_Log_Save_Folder = textBox_Folder.Text;
                    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().m_Config.m_Log_Save_Folder);
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                }
                // Check the folder exists
                if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                {
                    if (Directory.Exists(LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void button_DELETE_Click(object sender, EventArgs e)
        {
            if (textBox_Folder.Text.Length > 1)
            {
                LVApp.Instance().m_Config.m_Log_Save_Folder = textBox_Folder.Text;
            }
            if (LVApp.Instance().m_Config.m_Model_Name == "")
            {
                return;
            }
            string t_msg = string.Empty;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                t_msg = "[" + LVApp.Instance().m_Config.m_Model_Name + "]의 이미지를 삭제할까요? ";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                t_msg = "Do you want to delete image of model [" + LVApp.Instance().m_Config.m_Model_Name + "] ? ";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                t_msg = "是否要删除模型的图像 [" + LVApp.Instance().m_Config.m_Model_Name + "] ? ";
            }

            if (MessageBox.Show(t_msg, " Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                    {
                        DirectoryInfo tempDirInfo = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name);

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

                            tempDirInfo.Delete(true);
                        }
                        //tempDirInfo = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);

                        //if (tempDirInfo.Exists == true)
                        //{
                        //    foreach (DirectoryInfo di in tempDirInfo.GetDirectories())
                        //    {
                        //        foreach (FileInfo fi in di.GetFiles())
                        //        {
                        //            if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                        //            {
                        //                fi.Attributes = FileAttributes.Normal;
                        //            }
                        //        }
                        //    }

                        //    tempDirInfo.Delete(true);
                        //}
                    }
                    else
                    {
                        DirectoryInfo tempDirInfo = new DirectoryInfo(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name);

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

                            tempDirInfo.Delete(true);
                        }
                        //tempDirInfo = new DirectoryInfo(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);

                        //if (tempDirInfo.Exists == true)
                        //{
                        //    foreach (DirectoryInfo di in tempDirInfo.GetDirectories())
                        //    {
                        //        foreach (FileInfo fi in di.GetFiles())
                        //        {
                        //            if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                        //            {
                        //                fi.Attributes = FileAttributes.Normal;
                        //            }
                        //        }
                        //    }

                        //    tempDirInfo.Delete(true);
                        //}
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("삭제 완료 되었습니다.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("Deleted.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    MessageBox.Show("删除.");
                }

            }
        }

        private List<string> AllFiles;

        public void Image_delete()
        {
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
                    LVApp.Instance().m_mainform.add_Log("注册模型后使用.");
                }
                return;
            }

            AllFiles = new List<string>();
            ParsePath(LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name);

            for (int i = 0; i < AllFiles.Count; i++)
            {
                if (AllFiles[i].Substring(AllFiles[i].Length - 2, 2) == "OK" || AllFiles[i].Substring(AllFiles[i].Length - 2, 2) == "NG")
                {
                    //MessageBox.Show(AllFiles[i]);
                    DirectoryInfo source = new DirectoryInfo(AllFiles[i]);

                    // Get info of each file into the directory
                    foreach (FileInfo fi in source.GetFiles())
                    {
                        var creationTime = fi.CreationTime;

                        if (creationTime < (DateTime.Now - new TimeSpan(Convert.ToInt32(textBox_SAVE_DATE.Text.ToString()), 0, 0, 0)))
                        {
                            fi.Delete();
                        }
                    }
                }
            }
        }

        void ParsePath(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            // 폴더가 존재하지 않으면
            if (dir.Exists == true)
            {
                string[] SubDirs = Directory.GetDirectories(path);
                AllFiles.AddRange(SubDirs);
                //AllFiles.AddRange(Directory.GetFiles(path));
                foreach (string subdir in SubDirs)
                    ParsePath(subdir);
            }
        }

        private void button_LOGFOLDEROPEN_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Data_Save_Folder.Length <= 1)
            { // 서버에 저장 안할 때
                if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length <= 1)
                { // 로그 폴더가 없을 때
                    if (Directory.Exists(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
                else
                { // 로그 폴더가 있을 때
                    if (Directory.Exists(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
            }
            else
            { // 서버에 저장 할 때
                if (Directory.Exists(LVApp.Instance().m_Config.m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name))
                {
                    try
                    {
                        // Start a new process for explorer
                        // in this location
                        ProcessStartInfo l_psi = new ProcessStartInfo();
                        l_psi.FileName = "explorer";
                        l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().m_Config.m_Data_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                        l_psi.UseShellExecute = true;

                        Process l_newProcess = new Process();
                        l_newProcess.StartInfo = l_psi;
                        l_newProcess.Start();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error in 'LaunchFolderView'.", exception);
                    }
                }

                if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length <= 1)
                { // 로그 폴더가 없을 때
                    if (Directory.Exists(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
                else
                { // 로그 폴더가 있을 때
                    if (Directory.Exists(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
            }
        }

        private void button_LOGFOLDERDELETE_Click(object sender, EventArgs e)
        {

            if (LVApp.Instance().m_Config.m_Model_Name == "")
            {
                return;
            }
            if (MessageBox.Show("Do you want to delete log of model [" + LVApp.Instance().m_Config.m_Model_Name + "]?", " Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
                    {
                        DirectoryInfo tempDirInfo = new DirectoryInfo(LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);

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

                            tempDirInfo.Delete(true);
                        }
                    }
                    else
                    {
                        DirectoryInfo tempDirInfo = new DirectoryInfo(LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name);

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

                            tempDirInfo.Delete(true);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    MessageBox.Show("삭제 완료 되었습니다.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    MessageBox.Show("Deleted.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    MessageBox.Show("删除.");
                }
            }
        }

        public void button_LOGLOAD_Click(object sender, EventArgs e)
        {
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
                    LVApp.Instance().m_mainform.add_Log("注册模型后使用.");
                }
                return;
            }

            try
            {
                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + LVApp.Instance().m_Config.m_Model_Name + ".xlsx");
                if (!newFile.Exists)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        MessageBox.Show("모델을 등록후 사용하세요.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        MessageBox.Show("Use after registering a model.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        LVApp.Instance().m_mainform.add_Log("注册模型后使用.");
                    }
                    return;
                }
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    checkBox_CAM0.Checked = worksheet.Cells[2, 2].Value.ToString() == "1" ? true : false;
                    checkBox_AISAVE.Checked = worksheet.Cells[2, 3].Value.ToString() == "1" ? true : false;
                    checkBox_CAM1.Checked = worksheet.Cells[3, 2].Value.ToString() == "1" ? true : false;
                    checkBox_CAM2.Checked = worksheet.Cells[4, 2].Value.ToString() == "1" ? true : false;
                    checkBox_CAM3.Checked = worksheet.Cells[5, 2].Value.ToString() == "1" ? true : false;
                    checkBox_SSFSAVE.Checked = worksheet.Cells[6, 2].Value.ToString() == "1" ? true : false;
                    checkBox_CAM5.Checked = worksheet.Cells[7, 2].Value.ToString() == "1" ? true : false;
                    checkBox_CAM6.Checked = worksheet.Cells[8, 2].Value.ToString() == "1" ? true : false;
                    checkBox_CAM7.Checked = worksheet.Cells[9, 2].Value.ToString() == "1" ? true : false;
                    comboBox_SAVEMETHOD.SelectedIndex = comboBox_SAVEMETHOD.FindStringExact(worksheet.Cells[10, 2].Value.ToString());
                    textBox_SAVE_DATE.Text = worksheet.Cells[11, 2].Value.ToString();
                    comboBox_SAVEFORMAT.SelectedIndex = comboBox_SAVEFORMAT.FindStringExact(worksheet.Cells[12, 2].Value.ToString());
                    checkBox_LOGUSE.Checked = worksheet.Cells[13, 2].Value.ToString() == "1" ? true : false;
                    textBox_LOGDAY.Text = worksheet.Cells[14, 2].Value.ToString();
                    if (worksheet.Cells[15, 2].Value != null)
                    {
                        textBox_LOGCOUNT.Text = worksheet.Cells[15, 2].Value.ToString();
                    }
                    else
                    {
                        textBox_LOGCOUNT.Text = "1000";
                    }
                    if (worksheet.Cells[16, 2].Value != null)
                    {
                        textBox_Folder.Text = worksheet.Cells[16, 2].Value.ToString();
                        LVApp.Instance().m_Config.m_Log_Save_Folder = textBox_Folder.Text;
                    }
                    else
                    {
                        textBox_Folder.Text = "";
                    }
                    if (worksheet.Cells[22, 4].Value != null)
                    {
                        textBox_Folder2.Text = worksheet.Cells[22, 4].Value.ToString();
                        LVApp.Instance().m_Config.m_Log_Save_Folder2 = textBox_Folder2.Text;
                    }
                    else
                    {
                        textBox_Folder2.Text = "";
                    }

                    if (worksheet.Cells[16, 3].Value != null)
                    {
                        textBox_Data_Folder.Text = worksheet.Cells[16, 3].Value.ToString();
                        LVApp.Instance().m_Config.m_Data_Save_Folder = textBox_Data_Folder.Text;
                    }
                    else
                    {
                        textBox_Data_Folder.Text = "";
                    }

                    if (worksheet.Cells[17, 2].Value != null)
                    {
                        checkBox_Display.Checked = worksheet.Cells[17, 2].Value.ToString() == "1" ? true : false;
                    }
                    else
                    {
                        checkBox_Display.Checked = true;
                    }
                    if (worksheet.Cells[18, 2].Value != null)
                    {
                        checkBox_Debugging.Checked = worksheet.Cells[18, 2].Value.ToString() == "1" ? true : false;
                    }
                    else
                    {
                        checkBox_Debugging.Checked = false;
                    }
                    if (worksheet.Cells[19, 2].Value != null)
                    {
                        checkBox_TextView.Checked = worksheet.Cells[19, 2].Value.ToString() == "1" ? true : false;
                    }
                    else
                    {
                        checkBox_TextView.Checked = true;
                    }

                    if (worksheet.Cells[20, 2].Value != null)
                    {
                        textBox_GRAPH.Text = worksheet.Cells[20, 2].Value.ToString();
                    }
                    else
                    {
                        textBox_GRAPH.Text = "5";
                    }
                    int.TryParse(textBox_GRAPH.Text, out LVApp.Instance().m_Config.m_Graph_Update_sec);

                    if (worksheet.Cells[21, 2].Value != null)
                    {
                        checkBox_NG_Display.Checked = worksheet.Cells[21, 2].Value.ToString() == "1" ? true : false;
                    }
                    else
                    {
                        checkBox_NG_Display.Checked = false;
                    }
                    if (worksheet.Cells[22, 2].Value != null)
                    {
                        checkBox_NG_LOG.Checked = worksheet.Cells[22, 2].Value.ToString() == "1" ? true : false;
                    }
                    else
                    {
                        checkBox_NG_LOG.Checked = false;
                    }
                    if (worksheet.Cells[22, 3].Value != null)
                    {
                        textBox_MEMORY.Text = worksheet.Cells[22, 3].Value.ToString();
                    }
                    else
                    {
                        textBox_MEMORY.Text = "100";
                    }
                    double.TryParse(textBox_MEMORY.Text, out LVApp.Instance().m_Config.NG_Log_Memory);
                    Update_NG_Log_Max_CNT();
                    LVApp.Instance().m_Config.AI_Image_Save = checkBox_AISAVE.Checked;
                    LVApp.Instance().m_Config.SSF_Image_Save = checkBox_SSFSAVE.Checked;
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("LOG 불러오기 완료.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("LOG loaded.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("LOG 设置已加载.");
                }
                Refresh_Log_Data();
                LVApp.Instance().m_Config.Create_Save_Folders2();
                LVApp.Instance().m_Config.Alg_Debugging = false;
            }
            catch
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("LOG 불러오기 에러.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("LOG Loading Error!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("LOG 设置错误!");
                }
            }
        }

        public void Refresh_Log_Data()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LVApp.Instance().m_Config.m_Cam_Log_Use_Check[0] = checkBox_CAM0.Checked;
                    LVApp.Instance().m_Config.m_Cam_Log_Use_Check[1] = checkBox_CAM1.Checked;
                    LVApp.Instance().m_Config.m_Cam_Log_Use_Check[2] = checkBox_CAM2.Checked;
                    LVApp.Instance().m_Config.m_Cam_Log_Use_Check[3] = checkBox_CAM3.Checked;
                    //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[4] = checkBox_CAM4.Checked;
                    //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[5] = checkBox_CAM5.Checked;
                    //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[6] = checkBox_CAM6.Checked;
                    //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[7] = checkBox_CAM7.Checked;

                    if (comboBox_SAVEMETHOD.SelectedIndex < 0)
                    {
                        comboBox_SAVEMETHOD.SelectedIndex = 1;
                    }
                    if (comboBox_SAVEFORMAT.SelectedIndex < 0)
                    {
                        comboBox_SAVEFORMAT.SelectedIndex = 1;
                    }

                    LVApp.Instance().m_Config.m_Cam_Log_Method = comboBox_SAVEMETHOD.SelectedIndex;
                    LVApp.Instance().m_Config.m_Cam_Log_Format = comboBox_SAVEFORMAT.SelectedIndex;
                    LVApp.Instance().m_Config.m_Data_Log_Use_Check = checkBox_LOGUSE.Checked;

                    int m_date = Convert.ToInt32(textBox_SAVE_DATE.Text);
                    LVApp.Instance().m_Config.m_Cam_Log_Date = m_date;

                    m_date = Convert.ToInt32(textBox_LOGDAY.Text);
                    LVApp.Instance().m_Config.m_Data_Log_Date = m_date;

                    LVApp.Instance().m_Config.m_Log_Save_Num = Convert.ToInt32(textBox_LOGCOUNT.Text);
                    LVApp.Instance().m_Config.m_Log_Save_Folder = textBox_Folder.Text;

                    if (checkBox_Display.Checked)
                    {
                        LVApp.Instance().m_Config.Realtime_View_Check = true;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Realtime_View_Check = false;
                    }
                    if (checkBox_Debugging.Checked)
                    {
                        LVApp.Instance().m_Config.Alg_Debugging = true;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Alg_Debugging = false;
                    }
                    if (checkBox_TextView.Checked)
                    {
                        LVApp.Instance().m_Config.Alg_TextView = true;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Alg_TextView = false;
                    }

                    if (checkBox_NG_Display.Checked)
                    {
                        LVApp.Instance().m_Config.Diplay_Only_NG = true;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Diplay_Only_NG = false;
                    }
                    if (checkBox_NG_LOG.Checked)
                    {
                        LVApp.Instance().m_Config.NG_Log_Use = true;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.NG_Log_Use = false;
                    }
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);


                    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    {
                        LVApp.Instance().m_Config.Initialize_Data_Log(i);
                    }

                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.m_Alg_Type, LVApp.Instance().m_Config.Alg_Debugging);

                    if (LVApp.Instance().m_Config.Alg_Debugging)
                    {
                        DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\Debugging");
                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                    }
                    DebugLogger.Instance().LogRecord("Log 设置已更新!");
                });
            }
            else
            {
                LVApp.Instance().m_Config.m_Cam_Log_Use_Check[0] = checkBox_CAM0.Checked;
                LVApp.Instance().m_Config.m_Cam_Log_Use_Check[1] = checkBox_CAM1.Checked;
                LVApp.Instance().m_Config.m_Cam_Log_Use_Check[2] = checkBox_CAM2.Checked;
                LVApp.Instance().m_Config.m_Cam_Log_Use_Check[3] = checkBox_CAM3.Checked;
                //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[4] = checkBox_CAM4.Checked;
                //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[5] = checkBox_CAM5.Checked;
                //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[6] = checkBox_CAM6.Checked;
                //LVApp.Instance().m_Config.m_Cam_Log_Use_Check[7] = checkBox_CAM7.Checked;

                LVApp.Instance().m_Config.m_Cam_Log_Method = comboBox_SAVEMETHOD.SelectedIndex;
                LVApp.Instance().m_Config.m_Cam_Log_Format = comboBox_SAVEFORMAT.SelectedIndex;
                LVApp.Instance().m_Config.m_Data_Log_Use_Check = checkBox_LOGUSE.Checked;

                int m_date = Convert.ToInt32(textBox_SAVE_DATE.Text);
                LVApp.Instance().m_Config.m_Cam_Log_Date = m_date;

                m_date = Convert.ToInt32(textBox_LOGDAY.Text);
                LVApp.Instance().m_Config.m_Data_Log_Date = m_date;

                LVApp.Instance().m_Config.m_Log_Save_Num = Convert.ToInt32(textBox_LOGCOUNT.Text);
                LVApp.Instance().m_Config.m_Log_Save_Folder = textBox_Folder.Text;

                if (checkBox_Display.Checked)
                {
                    LVApp.Instance().m_Config.Realtime_View_Check = true;
                }
                else
                {
                    LVApp.Instance().m_Config.Realtime_View_Check = false;
                }
                if (checkBox_Debugging.Checked)
                {
                    LVApp.Instance().m_Config.Alg_Debugging = true;
                }
                else
                {
                    LVApp.Instance().m_Config.Alg_Debugging = false;
                }
                if (checkBox_TextView.Checked)
                {
                    LVApp.Instance().m_Config.Alg_TextView = true;
                }
                else
                {
                    LVApp.Instance().m_Config.Alg_TextView = false;
                }
                if (checkBox_NG_Display.Checked)
                {
                    LVApp.Instance().m_Config.Diplay_Only_NG = true;
                }
                else
                {
                    LVApp.Instance().m_Config.Diplay_Only_NG = false;
                }
                if (checkBox_NG_LOG.Checked)
                {
                    LVApp.Instance().m_Config.NG_Log_Use = true;
                }
                else
                {
                    LVApp.Instance().m_Config.NG_Log_Use = false;
                }
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);

                for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                {
                    LVApp.Instance().m_Config.Initialize_Data_Log(i);
                }

                //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.m_Alg_Type, LVApp.Instance().m_Config.Alg_Debugging);
                if (LVApp.Instance().m_Config.Alg_Debugging)
                {
                    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Images\\Debugging");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }
                }

                DebugLogger.Instance().LogRecord("Log 设置已更新!");
            }
        }

        public void Update_NG_Log_Max_CNT()
        {

            if (LVApp.Instance().m_Config.NG_Log_Memory > 525)
            {
                LVApp.Instance().m_Config.NG_Log_Memory = 525;
                textBox_MEMORY.Text = LVApp.Instance().m_Config.NG_Log_Memory.ToString();
            }
            int t_cam_num = 0; double t_cam0_memory = 0; double t_cam1_memory = 0; double t_cam2_memory = 0; double t_cam3_memory = 0;
            if (!LVApp.Instance().m_mainform.ctr_Camera_Setting1.Force_USE.Checked)
            {
                t_cam_num++;
                if (LVApp.Instance().m_mainform.ctr_ROI1.pictureBox_Image.Image != null)
                {
                    t_cam0_memory = (double)LVApp.Instance().m_mainform.ctr_ROI1.pictureBox_Image.Image.Width * (double)LVApp.Instance().m_mainform.ctr_ROI1.pictureBox_Image.Image.Height * 3d / 1000000d;
                }
            }
            if (!LVApp.Instance().m_mainform.ctr_Camera_Setting2.Force_USE.Checked)
            {
                t_cam_num++;
                if (LVApp.Instance().m_mainform.ctr_ROI2.pictureBox_Image.Image != null)
                {
                    t_cam1_memory = (double)LVApp.Instance().m_mainform.ctr_ROI2.pictureBox_Image.Image.Width * (double)LVApp.Instance().m_mainform.ctr_ROI2.pictureBox_Image.Image.Height * 3d / 1000000d;
                }
            }
            if (!LVApp.Instance().m_mainform.ctr_Camera_Setting3.Force_USE.Checked)
            {
                t_cam_num++;
                if (LVApp.Instance().m_mainform.ctr_ROI3.pictureBox_Image.Image != null)
                {
                    t_cam2_memory = (double)LVApp.Instance().m_mainform.ctr_ROI3.pictureBox_Image.Image.Width * (double)LVApp.Instance().m_mainform.ctr_ROI3.pictureBox_Image.Image.Height * 3d / 1000000d;
                }
            }
            if (!LVApp.Instance().m_mainform.ctr_Camera_Setting4.Force_USE.Checked)
            {
                t_cam_num++;
                if (LVApp.Instance().m_mainform.ctr_ROI4.pictureBox_Image.Image != null)
                {
                    t_cam3_memory = (double)LVApp.Instance().m_mainform.ctr_ROI4.pictureBox_Image.Image.Width * (double)LVApp.Instance().m_mainform.ctr_ROI4.pictureBox_Image.Image.Height * 3d / 1000000d;
                }
            }
            double t_total = t_cam0_memory + t_cam1_memory + t_cam2_memory + t_cam3_memory;
            if (t_total > 0)
            {
                LVApp.Instance().m_Config.NG_Log_Max_CNT = (int)((LVApp.Instance().m_Config.NG_Log_Memory) / t_total);
                if (LVApp.Instance().m_Config.NG_Log_Max_CNT < 1)
                {
                    LVApp.Instance().m_Config.NG_Log_Max_CNT = 1;
                }
            }
        }

        public void button_LOGSAVE_Click(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Alg_Debugging = false;
            Refresh_Log_Data();

            LVApp.Instance().m_Config.ds_LOG.Tables.Clear();
            LVApp.Instance().m_Config.ds_LOG.Clear();
            for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
            {
                LVApp.Instance().m_Config.Initialize_Data_Log(i);
            }

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
                    LVApp.Instance().m_mainform.add_Log("注册模型后使用.");
                }
                return;
            }

            try
            {
                LVApp.Instance().m_Config.Create_Save_Folders2();
                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + LVApp.Instance().m_Config.m_Model_Name + ".xlsx");
                if (!newFile.Exists)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        MessageBox.Show("모델을 등록후 사용하세요.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        MessageBox.Show("Use after registering a model.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        LVApp.Instance().m_mainform.add_Log("注册模型后使用.");
                    }
                    return;
                }
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    worksheet.Cells[2, 2].Value = checkBox_CAM0.Checked == true ? "1" : "0";
                    worksheet.Cells[2, 3].Value = checkBox_AISAVE.Checked == true ? "1" : "0";
                    worksheet.Cells[3, 2].Value = checkBox_CAM1.Checked == true ? "1" : "0";
                    worksheet.Cells[4, 2].Value = checkBox_CAM2.Checked == true ? "1" : "0";
                    worksheet.Cells[5, 2].Value = checkBox_CAM3.Checked == true ? "1" : "0";
                    worksheet.Cells[6, 2].Value = checkBox_SSFSAVE.Checked == true ? "1" : "0";
                    worksheet.Cells[7, 2].Value = checkBox_CAM5.Checked == true ? "1" : "0";
                    worksheet.Cells[8, 2].Value = checkBox_CAM6.Checked == true ? "1" : "0";
                    worksheet.Cells[9, 2].Value = checkBox_CAM7.Checked == true ? "1" : "0";
                    if (comboBox_SAVEMETHOD.SelectedIndex < 0)
                    {
                        comboBox_SAVEMETHOD.SelectedIndex = 1;
                    }
                    worksheet.Cells[10, 2].Value = comboBox_SAVEMETHOD.Items[comboBox_SAVEMETHOD.SelectedIndex].ToString();
                    worksheet.Cells[11, 2].Value = textBox_SAVE_DATE.Text;
                    if (comboBox_SAVEFORMAT.SelectedIndex < 0)
                    {
                        comboBox_SAVEFORMAT.SelectedIndex = 1;
                    }
                    worksheet.Cells[12, 2].Value = comboBox_SAVEFORMAT.Items[comboBox_SAVEFORMAT.SelectedIndex].ToString();

                    worksheet.Cells[13, 2].Value = checkBox_LOGUSE.Checked == true ? "1" : "0";
                    worksheet.Cells[14, 2].Value = textBox_LOGDAY.Text;
                    worksheet.Cells[15, 2].Value = textBox_LOGCOUNT.Text;
                    worksheet.Cells[16, 2].Value = textBox_Folder.Text;
                    worksheet.Cells[16, 3].Value = textBox_Data_Folder.Text;
                    worksheet.Cells[17, 2].Value = checkBox_Display.Checked == true ? "1" : "0";
                    worksheet.Cells[18, 2].Value = checkBox_Debugging.Checked == true ? "1" : "0";
                    worksheet.Cells[19, 2].Value = checkBox_TextView.Checked == true ? "1" : "0";
                    worksheet.Cells[21, 2].Value = checkBox_NG_Display.Checked == true ? "1" : "0";
                    worksheet.Cells[22, 2].Value = checkBox_NG_LOG.Checked == true ? "1" : "0";
                    worksheet.Cells[22, 3].Value = textBox_MEMORY.Text;

                    worksheet.Cells[20, 2].Value = textBox_GRAPH.Text;
                    worksheet.Cells[22, 4].Value = textBox_Folder2.Text;

                    int.TryParse(textBox_GRAPH.Text, out LVApp.Instance().m_Config.m_Graph_Update_sec);
                    package.Save();
                }
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("LOG 저장완료.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("LOG Saved.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("LOG 设置已保存.");
                }

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

                if (!LVApp.Instance().m_Config.m_Data_Log_Use_Check)
                {
                    LVApp.Instance().m_Config.CSV_Logfile_Terminate();
                    return;
                }
                else
                {
                    for (int i = 0; i < LVApp.Instance().m_Config.m_Cam_Total_Num; i++)
                    {
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[i])
                        {
                            LVApp.Instance().m_Config.CSV_Logfile_Initialize(i);
                        }
                    }
                    LVApp.Instance().m_Config.CSV_Logfile_Initialize(4);
                }

                double.TryParse(textBox_MEMORY.Text, out LVApp.Instance().m_Config.NG_Log_Memory);
                Update_NG_Log_Max_CNT();
                //button_SAVE_Click(sender, e);
            }
            catch
            {
                string t_msg = string.Empty;
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("로그 설정 에러!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("LOG Setting Error!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("日志设置错误!");
                }
            }
        }

        private void comboBox_SAVEMETHOD_SelectedIndexChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.m_Cam_Log_Method = comboBox_SAVEMETHOD.SelectedIndex;
        }

        private void textBox_SAVE_DATE_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_SAVEFORMAT_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button_Folder_setting_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();  //폴더 다이알로그 호출

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_Folder.ResetText();
                textBox_Folder.Text = folderBrowserDialog1.SelectedPath;
                LVApp.Instance().m_Config.m_Log_Save_Folder = folderBrowserDialog1.SelectedPath;
            }
        }

        private void checkBox_Display_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Display.Checked)
            {
                LVApp.Instance().m_Config.Realtime_View_Check = true;
            }
            else
            {
                LVApp.Instance().m_Config.Realtime_View_Check = false;
            }
        }

        private void checkBox_Debugging_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Debugging.Checked)
            {
                LVApp.Instance().m_Config.Alg_Debugging = true;
            }
            else
            {
                LVApp.Instance().m_Config.Alg_Debugging = false;
            }
        }

        private void checkBox_NG_Display_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_NG_Display.Checked)
            {
                LVApp.Instance().m_Config.Diplay_Only_NG = true;
            }
            else
            {
                LVApp.Instance().m_Config.Diplay_Only_NG = false;
            }
        }

        private void checkBox_NG_LOG_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_NG_LOG.Checked)
            {
                LVApp.Instance().m_Config.NG_Log_Use = true;
            }
            else
            {
                LVApp.Instance().m_Config.NG_Log_Use = false;
                LVApp.Instance().m_mainform.ctr_NGLog1.Reset_NGLog();
            }
        }

        private void button_Data_Folder_setting_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();  //폴더 다이알로그 호출

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_Data_Folder.ResetText();
                textBox_Data_Folder.Text = folderBrowserDialog1.SelectedPath;
                LVApp.Instance().m_Config.m_Data_Save_Folder = folderBrowserDialog1.SelectedPath;
            }
        }

        private void checkBox_AISAVE_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.AI_Image_Save = checkBox_AISAVE.Checked;
        }

        private void checkBox_SSFSAVE_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.SSF_Image_Save = checkBox_SSFSAVE.Checked;

        }

        private void checkBox_CAM0_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button_Folder_setting2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();  //폴더 다이알로그 호출

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_Folder2.ResetText();

                if (folderBrowserDialog1.SelectedPath.Length == 3)
                {
                    textBox_Folder2.Text = folderBrowserDialog1.SelectedPath.Substring(0,2);
                }
                else
                {
                    textBox_Folder2.Text = folderBrowserDialog1.SelectedPath;
                }
                LVApp.Instance().m_Config.m_Log_Save_Folder2 = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button_OPEN2_Click(object sender, EventArgs e)
        {
            try
            {
                //if (textBox_Folder2.Text.Length > 1)
                //{
                //    LVApp.Instance().m_Config.m_Log_Save_Folder2 = textBox_Folder2.Text;
                //    DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().m_Config.m_Log_Save_Folder2);
                //    // 폴더가 존재하지 않으면
                //    if (dir.Exists == false)
                //    {
                //        // 새로 생성합니다.
                //        dir.Create();
                //    }
                //}
                // Check the folder exists
                if (LVApp.Instance().m_Config.m_Log_Save_Folder2 == "")
                {

                }
                else
                {
                    if (Directory.Exists(LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name))
                    {
                        try
                        {
                            // Start a new process for explorer
                            // in this location
                            ProcessStartInfo l_psi = new ProcessStartInfo();
                            l_psi.FileName = "explorer";
                            l_psi.Arguments = string.Format("/root,{0}", LVApp.Instance().m_Config.m_Log_Save_Folder2 + "\\" + LVApp.Instance().m_Config.m_Model_Name);
                            l_psi.UseShellExecute = true;

                            Process l_newProcess = new Process();
                            l_newProcess.StartInfo = l_psi;
                            l_newProcess.Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Error in 'LaunchFolderView'.", exception);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
