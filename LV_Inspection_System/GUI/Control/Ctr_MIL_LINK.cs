using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_MIL_LINK : UserControl
    {
        public int Cam_Num = 0;
        public bool m_Loaded = false;
        private BackgroundWorker Tx_worker = new BackgroundWorker();
        private static char sCR() { return Convert.ToChar(13); }
        private static char sLF() { return Convert.ToChar(10); }
        private static string sCRLF() { return "\r\n"; }

        public Ctr_MIL_LINK()
        {
            InitializeComponent();
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0)// && m_Language != value)
                {// 한국어
                    button_OPEN.Text = "열기";
                    button_CLOSE.Text = "닫기";
                    button_SEND.Text = "보내기";
                    tabControl_COMMAND.TabPages[1].Text = "자동모드";
                    tabControl_COMMAND.TabPages[2].Text = "수동모드";
                }
                else if (value == 1)// && m_Language != value)
                {// 영어
                    button_OPEN.Text = "Open";
                    button_CLOSE.Text = "Close";
                    button_SEND.Text = "Send";
                    tabControl_COMMAND.TabPages[1].Text = "AUTO Mode";
                    tabControl_COMMAND.TabPages[2].Text = "MANUAL Mode";
                }
                else if (value == 2)// && m_Language != value)
                {//중국어
                    button_OPEN.Text = "打开";
                    button_CLOSE.Text = "关闭";
                    button_SEND.Text = "发送";
                    tabControl_COMMAND.TabPages[1].Text = "自动模式";
                    tabControl_COMMAND.TabPages[2].Text = "手动模式";
                }
                m_Language = value;
            }
        }

        private void button_OPEN_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = cbPortName.Text;
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    serialPort1.Open();
                }
                UI_Update();
            }
            catch
            { }
        }


        private void UI_Update()
        {
            m_SetLanguage = LVApp.Instance().m_Config.m_SetLanguage;
            if (!serialPort1.IsOpen)
            {
                if (button_OPEN.InvokeRequired)
                {
                    button_OPEN.Invoke((MethodInvoker)delegate
                    {
                        button_OPEN.Enabled = true;
                    });
                }
                else
                {
                    button_OPEN.Enabled = true;
                }
                if (button_CLOSE.InvokeRequired)
                {
                    button_CLOSE.Invoke((MethodInvoker)delegate
                    {
                        button_CLOSE.Enabled = false;
                    });
                }
                else
                {
                    button_CLOSE.Enabled = false;
                }
                if (button_SEND.InvokeRequired)
                {
                    button_SEND.Invoke((MethodInvoker)delegate
                    {
                        button_SEND.Enabled = false;
                    });
                }
                else
                {
                    button_SEND.Enabled = false;
                }
            }
            else
            {
                if (button_OPEN.InvokeRequired)
                {
                    button_OPEN.Invoke((MethodInvoker)delegate
                    {
                        button_OPEN.Enabled = false;
                    });
                }
                else
                {
                    button_OPEN.Enabled = false;
                }
                if (button_CLOSE.InvokeRequired)
                {
                    button_CLOSE.Invoke((MethodInvoker)delegate
                    {
                        button_CLOSE.Enabled = true;
                    });
                }
                else
                {
                    button_CLOSE.Enabled = true;
                }
                if (button_SEND.InvokeRequired)
                {
                    button_SEND.Invoke((MethodInvoker)delegate
                    {
                        button_SEND.Enabled = true;
                    });
                }
                else
                {
                    button_SEND.Enabled = true;
                }
            }

        }

        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] array = new byte[512];
                int temp;
                temp = serialPort1.Read(array, 0, 512);
                string t_msg = "Rx : ";
                for (int i = 0; i < temp; i++)
                {
                    t_msg += (char)array[i];
                }

                if (Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting1.Add_Message(t_msg);
                }
                else if (Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting2.Add_Message(t_msg);
                }
                else if (Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting3.Add_Message(t_msg);
                }
                else if (Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.ctr_Camera_Setting4.Add_Message(t_msg);
                }
            }
            catch
            {
            }
        }
        private void button_CLOSE_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                serialPort1.Close();
            }
            UI_Update();
        }

        public void button_SEND_Click(object sender, EventArgs e)
        {
            if (Tx_worker.IsBusy || !serialPort1.IsOpen)
            {
                return;
            }
            Tx_Work();
        }

        private object syncLock = new object();
        bool isInCall = false;
        private void Tx_Work()
        {
            lock (syncLock)
            {
                if (isInCall)
                    return;
                isInCall = true;
            }
            try
            {
                Tx_worker.DoWork += new DoWorkEventHandler(Tx_Process);
                Tx_worker.RunWorkerAsync();
                Thread.Sleep(2000);
            }
            finally
            {
                lock (syncLock)
                {
                    isInCall = false;
                }
            }
        }

        private void Tx_Process(object sender, DoWorkEventArgs e)
        {
            List<string> lines = null;
            //if (LVApp.Instance().m_Config.m_Auto_Mode)
            //{
            //    if (richTextBox_SCRIPT_Auto.InvokeRequired)
            //    {
            //        richTextBox_SCRIPT_Auto.Invoke((MethodInvoker)delegate
            //        {
            //            lines = richTextBox_SCRIPT_Auto.Lines.ToList();
            //        });
            //    }
            //    else
            //    {
            //        lines = richTextBox_SCRIPT_Auto.Lines.ToList();
            //    }
            //}
            //else
            //{
            //    if (richTextBox_SCRIPT_Manual.InvokeRequired)
            //    {
            //        richTextBox_SCRIPT_Manual.Invoke((MethodInvoker)delegate
            //        {
            //            lines = richTextBox_SCRIPT_Manual.Lines.ToList();
            //        });
            //    }
            //    else
            //    {
            //        lines = richTextBox_SCRIPT_Manual.Lines.ToList();
            //    }
            //}

            if (richTextBox_SCRIPT.InvokeRequired)
            {
                richTextBox_SCRIPT.Invoke((MethodInvoker)delegate
                {
                    lines = richTextBox_SCRIPT.Lines.ToList();
                });
            }
            else
            {
                lines = richTextBox_SCRIPT.Lines.ToList();
            }

            if (lines.Count <= 0)
            {
                return;
            }

            foreach (string current in lines)
            {
                if (current.ToUpper().Contains("DELAY") && (current.Contains("=") || current.Contains(":") || current.Contains(" ")))
                {
                    string trim = current.Replace(" ", "=");
                    string[] sub_str = trim.Split('=');
                    if (sub_str.Length > 1)
                    {
                        int t_v = 0;
                        int.TryParse(sub_str[1], out t_v);
                        Thread.Sleep(Math.Abs(t_v));
                    }
                    sub_str = current.Split(':');
                    if (sub_str.Length > 1)
                    {
                        int t_v = 0;
                        int.TryParse(sub_str[1], out t_v);
                        Thread.Sleep(Math.Abs(t_v));
                    }
                }
                else
                {
                    if (current.Length > 0)
                    {
                        if (serialPort1.IsOpen)
                        {
                            serialPort1.Write(current + sCRLF());
                            string t_msg = "Tx : " + current;
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.Add_Message(t_msg);
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.Add_Message(t_msg);
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.Add_Message(t_msg);
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.Add_Message(t_msg);
                            }
                        }
                    }
                }
                Thread.Sleep(20);
            }
            Tx_worker.DoWork -= new DoWorkEventHandler(Tx_Process);
        }

        private void button_DCF_Click(object sender, EventArgs e)
        {
            // 이미지를 불러와서 Opencv 클래스로 넣음.
            OpenFileDialog openPanel = new OpenFileDialog();
            openPanel.InitialDirectory = ".\\";
            openPanel.Filter = "All DCF file|*.dcf";
            if (openPanel.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = openPanel.FileName;
                string targetPath = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + ".dcf";
                System.IO.File.Copy(sourcePath, targetPath, true);
                string filename = Path.GetFileName(targetPath);
                textBox_DCF.Text = filename;
            }
        }

        public void SAVE()
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

            try
            {
                string t_file = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + "_Config.txt";
                string t_str = "";
                StreamWriter sw = new StreamWriter(t_file, false);
                t_str = "cbPortName:" + cbPortName.Text;
                sw.WriteLine(t_str);
                t_str = "comboBox_GRABBER:" + comboBox_GRABBER.Text;
                sw.WriteLine(t_str);
                t_str = "comboBox_CAMERA:" + comboBox_CAMERA.Text;
                sw.WriteLine(t_str);
                t_str = "checkBox_NOMANUAL:" + (checkBox_NOMANUAL.Checked == true ? "1":"0");
                sw.WriteLine(t_str);
                t_str = "comboBox_GBOARD:" + comboBox_GBOARD.Text;
                sw.WriteLine(t_str);


                t_str = "";
                List<string> lines = null;
                //if (richTextBox_SCRIPT.InvokeRequired)
                //{
                //    richTextBox_SCRIPT.Invoke((MethodInvoker)delegate
                //    {
                //        lines = richTextBox_SCRIPT.Lines.ToList();
                //    });
                //}
                //else
                {
                    lines = richTextBox_SCRIPT_Auto.Lines.ToList();
                }

                foreach (string current in lines)
                {
                    t_str = current;
                    sw.WriteLine(t_str);
                }
                sw.Close();

                t_file = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + "_MConfig.txt";
                StreamWriter sw1 = new StreamWriter(t_file, false);
                List<string> lines1 = null;
                lines1 = richTextBox_SCRIPT_Manual.Lines.ToList();

                foreach (string current in lines1)
                {
                    t_str = current;
                    sw1.WriteLine(t_str);
                }
                sw1.Close();

                t_file = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + "_Script.txt";
                StreamWriter sw2 = new StreamWriter(t_file, false);
                List<string> lines2 = null;
                lines2 = richTextBox_SCRIPT.Lines.ToList();

                foreach (string current in lines2)
                {
                    t_str = current;
                    sw2.WriteLine(t_str);
                }
                sw2.Close();

                button_OPEN_Click(null, null);
                //button_SEND_Click(null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        public void LOAD()
        {
            try
            {
                m_Loaded = true;
                string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + "_Config.txt";
                if (System.IO.File.Exists(filename))
                {
                    //if (richTextBox_SCRIPT.InvokeRequired)
                    //{
                    //    richTextBox_SCRIPT.Invoke((MethodInvoker)delegate
                    //    {
                    //        richTextBox_SCRIPT.ResetText();
                    //    });
                    //}
                    //else
                    {
                        richTextBox_SCRIPT_Auto.ResetText();
                    }

                    LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[0] = LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[1] = LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[2] = LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[3] = false;
                    string[] lines = File.ReadAllLines(filename);
                    int t_idx = 0;
                    foreach (string sub_line in lines)
                    {
                        if (t_idx == 0)
                        {
                            string[] sub_str = sub_line.Split(':');
                            cbPortName.Text = sub_str[1];
                        }
                        else if (t_idx == 1)
                        {
                            string[] sub_str = sub_line.Split(':');
                            comboBox_GRABBER.Text = sub_str[1];
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_MIL.CAM0_MIL_SystemNum = comboBox_GRABBER.SelectedIndex;
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_MIL.CAM1_MIL_SystemNum = comboBox_GRABBER.SelectedIndex;
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_MIL.CAM2_MIL_SystemNum = comboBox_GRABBER.SelectedIndex;
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_MIL.CAM3_MIL_SystemNum = comboBox_GRABBER.SelectedIndex;
                            }
                        }
                        else if (t_idx == 2)
                        {
                            string[] sub_str = sub_line.Split(':');
                            comboBox_CAMERA.Text = sub_str[1];
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_MIL.CAM0_MIL_CH = comboBox_CAMERA.SelectedIndex;
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_MIL.CAM1_MIL_CH = comboBox_CAMERA.SelectedIndex;
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_MIL.CAM2_MIL_CH = comboBox_CAMERA.SelectedIndex;
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_MIL.CAM3_MIL_CH = comboBox_CAMERA.SelectedIndex;
                            }
                        }
                        else if (t_idx == 3)
                        {
                            string[] sub_str = sub_line.Split(':');

                            if (checkBox_NOMANUAL.Name == sub_str[0])
                            {
                                if (sub_str[1] == "1")
                                {
                                    checkBox_NOMANUAL.Checked = true;
                                    LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[Cam_Num] = true;
                                }
                                else
                                {
                                    checkBox_NOMANUAL.Checked = false;
                                    LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[Cam_Num] = false;
                                }
                            }
                        }
                        else if (t_idx == 4)
                        {
                            string[] sub_str = sub_line.Split(':');
                            comboBox_GBOARD.Text = sub_str[1];
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_MIL.CAM0_MIL_GBOARD = comboBox_GBOARD.SelectedIndex;
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_MIL.CAM1_MIL_GBOARD = comboBox_GBOARD.SelectedIndex;
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_MIL.CAM2_MIL_GBOARD = comboBox_GBOARD.SelectedIndex;
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_MIL.CAM3_MIL_GBOARD = comboBox_GBOARD.SelectedIndex;
                            }
                        }
                        else
                        {
                            //if (richTextBox_SCRIPT.InvokeRequired)
                            //{
                            //    richTextBox_SCRIPT.Invoke((MethodInvoker)delegate
                            //    {
                            //        string display_str = sub_line + "\n" + richTextBox_SCRIPT.Text;
                            //        richTextBox_SCRIPT.Text = display_str;   
                            //    });
                            //}
                            //else
                            {
                                if (richTextBox_SCRIPT_Auto.Text.Length == 0)
                                {
                                    string display_str = sub_line;
                                    richTextBox_SCRIPT_Auto.Text = display_str;
                                }
                                else
                                {
                                    string display_str = richTextBox_SCRIPT_Auto.Text + "\n" + sub_line;
                                    richTextBox_SCRIPT_Auto.Text = display_str;
                                }
                            }
                        }
                        t_idx++;
                    }

                    filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + "_MConfig.txt";
                    if (System.IO.File.Exists(filename))
                    {
                        richTextBox_SCRIPT_Manual.ResetText();

                        string[] lines1 = File.ReadAllLines(filename);
                        foreach (string sub_line in lines1)
                        {
                            if (richTextBox_SCRIPT_Manual.Text.Length == 0)
                            {
                                string display_str = sub_line;
                                richTextBox_SCRIPT_Manual.Text = display_str;
                            }
                            else
                            {
                                string display_str = richTextBox_SCRIPT_Manual.Text + "\n" + sub_line;
                                richTextBox_SCRIPT_Manual.Text = display_str;
                            }
                        }
                    }

                    filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + "_Script.txt";
                    if (System.IO.File.Exists(filename))
                    {
                        richTextBox_SCRIPT.ResetText();

                        string[] lines1 = File.ReadAllLines(filename);
                        foreach (string sub_line in lines1)
                        {
                            if (richTextBox_SCRIPT.Text.Length == 0)
                            {
                                string display_str = sub_line;
                                richTextBox_SCRIPT.Text = display_str;
                            }
                            else
                            {
                                string display_str = richTextBox_SCRIPT.Text + "\n" + sub_line;
                                richTextBox_SCRIPT.Text = display_str;
                            }
                        }
                    }

                    if (Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonInitialize_Click(null, null);
                    }
                    else if (Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonInitialize_Click(null, null);
                    }
                    else if (Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonInitialize_Click(null, null);
                    }
                    else if (Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonInitialize_Click(null, null);
                    }
                    button_OPEN_Click(null, null);
                    button_SEND_Click(null, null);
                }
                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + Cam_Num.ToString() + ".dcf");
                if (newFile.Exists)
                {
                    textBox_DCF.Text = "CAM" + Cam_Num.ToString() + ".dcf";
                }
                else
                {
                    textBox_DCF.Text = "No dcf file";
                }
            }
            catch
            { }
        }

        private void tabControl_COMMAND_SelectedIndexChanged(object sender, EventArgs e)
        {
            int t_Idx = tabControl_COMMAND.SelectedIndex;

            if (t_Idx < 0)
            {
                return;
            }

            if (t_Idx == 0)
            {
                LVApp.Instance().m_Config.m_Auto_Mode = true;
            }
            else if (t_Idx == 1)
            {
                LVApp.Instance().m_Config.m_Auto_Mode = false;
            }
        }

        private void checkBox_NOMANUAL_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_GBOARD_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_GRABBER_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_CAMERA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
