using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;   //호환되지 않은 Dll을 사용할때
using OfficeOpenXml;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net.Sockets;
using MCProtocol;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Net;
using System.Security.Cryptography;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_PLC : UserControl
    {
        public enum PROTOCAL { XGTRS232, ModbbusTCP, ModbusRTU, LVDIO, XGTTCP, IPSBoard };
        delegate void MyDelegate();      //델리게이트 선언(크로스 쓰레드 해결하기 위한 용도)
        public bool m_Trigger_Check = false;
        public int m_Cam_Trigger_Num = 0;

        bool SendForamt = true;          // true : ASCII   false : HEX
        bool ReceiveFormat = true;       // true : ASCII   false : HEX
        bool m_check_sending = false;

        public bool m_D_Write_check = false;

        Thread[] threads = new Thread[2];
        public bool m_threads_Check = false;

        Thread MC_Rx_threads;
        public bool MC_Rx_threads_Check = false;
        Thread MC_Tx_threads;
        public bool MC_Tx_threads_Check = false;


        public static char sSTX() { return Convert.ToChar(0x02); }
        public static char sETX() { return Convert.ToChar(0x03); }
        public static char sEOT() { return Convert.ToChar(0x04); }
        public static char sENQ() { return Convert.ToChar(0x05); }
        public static char sACK() { return Convert.ToChar(0x06); }
        public static char sNAK() { return Convert.ToChar(0x15); }
        public static char sCR() { return Convert.ToChar(13); }
        public static char sLF() { return Convert.ToChar(10); }
        public static string sCRLF() { return "\r\n"; }
        //int port;

        public string m_SlaveID;
        public int m_Pingpong_Num = 0;
        public int m_Protocal;
        //private Stopwatch send_sw = new Stopwatch();

        private EasyModbus.ModbusClient modbusClient;
        private TcpClient TCP_Client;
        private NetworkStream TCP_Client_Stream;
        //private StreamReader TCP_Client_SR = null;
        //private StreamWriter TCP_Client_SW = null;

        //private int writeBufferSize = 1024;
        //private int readBufferSize = 1024;


        public List<string>[] send_Message = new List<string>[4];
        public List<uint>[] send_Idx = new List<uint>[4];
        public List<DateTime>[] send_Message_Time = new List<DateTime>[4];
        public List<DateTime>[] send_Idx_Time = new List<DateTime>[4];
        //public Queue[] send_Message = new Queue[4];

        public int m_DELAYCAMMISS = 50;
        public float m_RESETDURATION = 2.0f;
        public int m_MinProcessingTime = 50;

        MCProtocol.Mitsubishi.McProtocolTcp McProtocolApp = new Mitsubishi.McProtocolTcp();


        public Ctr_PLC()
        {
            InitializeComponent();
            modbusClient = new EasyModbus.ModbusClient();
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    groupBox2.Text = "통신 설정";
                    label4.Text = "포트 이름";
                    label5.Text = "보드속도";
                    label6.Text = "데이터 비트";
                    label7.Text = "스탑 비트";
                    label8.Text = "페러티";
                    label2.Text = "수신 포맷";
                    label3.Text = "송신 포맷";
                    btnOpen.Text = "연결";
                    btnClose.Text = "접속 해제";
                    label9.Text = "주소";
                    label12.Text = "데이터 크기";
                    label11.Text = "데이터 값";
                    button_D_READ.Text = "읽기";
                    button_D_WRITE.Text = "쓰기";
                    label13.Text = "주소";
                    label14.Text = "데이터 값";
                    button_L_READ.Text = "읽기";
                    button_L_WRITE.Text = "쓰기";
                    groupBox3.Text = "프로토콜 설정";
                    label16.Text = "프로토콜";
                    label15.Text = "국번";
                    label17.Text = "전송간격(ms)";
                    checkBox_PINGPONG.Text = "교차 전송";
                    checkBox_AllOnceTx.Text = "모아서 통합 판정";
                    label35.Text = "판정 임계 시간(ms)";
                    button_LOAD.Text = "불러오기";
                    button_SAVE.Text = "적용 및 저장";
                    label36.Text = "대상 없음 판정";
                    checkBox_JView.Text = "판정 신호 보기";
                    label37.Text = "서버 IP";
                    label38.Text = "서버 PORT";
                    checkBox_SIMULATION.Text = "시물레이션 모드";
                    checkBox_Tab_Enable.Text = "검사중 메뉴 사용안함";
                    btnSend.Text = "송신";
                    button_View.Text = "뷰";
                    btnClear.Text = "클리어";
                    Button_Send_Apply.Text = "적용 및 저장";
                    button_LOG_CLEAR.Text = "클리어";
                    label26.Text = "최소처리시간(ms)";
                    label22.Text = "카메라 재시작 카운트";
                    label24.Text = "카메라 미처리일때 지연시간";
                    label25.Text = "Tx가 없을때 리셋 시간(sec)";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    groupBox2.Text = "Communication";
                    label4.Text = "Port Name";
                    label5.Text = "Baudrate";
                    label6.Text = "Data Bit";
                    label7.Text = "Stop Bit";
                    label8.Text = "Parity";
                    label2.Text = "Rx Format";
                    label3.Text = "Tx Format";
                    btnOpen.Text = "Connect";
                    btnClose.Text = "Disconnect";
                    label9.Text = "Device";
                    label12.Text = "Data Size";
                    label11.Text = "Data Value";
                    button_D_READ.Text = "Read";
                    button_D_WRITE.Text = "Write";
                    label13.Text = "Device";
                    label14.Text = "Data Value";
                    button_L_READ.Text = "Read";
                    button_L_WRITE.Text = "Write";
                    groupBox3.Text = "Protocol";
                    label16.Text = "Method";
                    label15.Text = "Station No.";
                    label17.Text = "Tx Interval(ms)";
                    checkBox_PINGPONG.Text = "Tx in turns";
                    checkBox_AllOnceTx.Text = "Once Tx after inspection";
                    label35.Text = "Judgement time(ms)";
                    button_LOAD.Text = "Load";
                    button_SAVE.Text = "Apply and Save";
                    label36.Text = "No object Tx";
                    checkBox_JView.Text = "Judge signal view";
                    label37.Text = "Server IP";
                    label38.Text = "Server PORT";
                    checkBox_SIMULATION.Text = "Simulation Mode";
                    checkBox_Tab_Enable.Text = "Disable Menu when start";
                    btnSend.Text = "Send";
                    button_View.Text = "View";
                    btnClear.Text = "Clear";
                    Button_Send_Apply.Text = "Apply & Save";
                    button_LOG_CLEAR.Text = "Clear";
                    label26.Text = "Min. Processing Time(ms)";
                    label22.Text = "Camera Refresh Count";
                    label24.Text = "Delay when CAM missed(ms)";
                    label25.Text = "Reset duration when no Tx(sec)";
                }
                else if (value == 2 && m_Language != value)
                {// 중국어
                    groupBox2.Text = "通信";
                    label4.Text = "Port Name";
                    label5.Text = "Baudrate";
                    label6.Text = "Data Bit";
                    label7.Text = "Stop Bit";
                    label8.Text = "Parity";
                    label2.Text = "Rx Format";
                    label3.Text = "Tx Format";
                    btnOpen.Text = "连接";
                    btnClose.Text = "断开";
                    label9.Text = "地址";
                    label12.Text = "Data 长度";
                    label11.Text = "Data 价值";
                    button_D_READ.Text = "读";
                    button_D_WRITE.Text = "写";
                    label13.Text = "地址";
                    label14.Text = "Data 价值";
                    button_L_READ.Text = "读";
                    button_L_WRITE.Text = "写";
                    groupBox3.Text = "Protocol";
                    label16.Text = "方法";
                    label15.Text = "Station No.";
                    label17.Text = "Tx 区间(ms)";
                    checkBox_PINGPONG.Text = "Tx in turns";
                    checkBox_AllOnceTx.Text = "Once Tx after inspection";
                    label35.Text = "判断 time(ms)";
                    button_LOAD.Text = "负荷";
                    button_SAVE.Text = "应用和保存";
                    label36.Text = "无对象 Tx";
                    checkBox_JView.Text = "法官信号视图";
                    label37.Text = "Server IP";
                    label38.Text = "Server PORT";
                    checkBox_SIMULATION.Text = "仿真 Mode";
                    checkBox_Tab_Enable.Text = "启动时禁用菜单";
                    btnSend.Text = "发送";
                    button_View.Text = "视图";
                    btnClear.Text = "清楚";
                    Button_Send_Apply.Text = "应用和保存";
                    button_LOG_CLEAR.Text = "清楚";
                    label26.Text = "最短处理时间(ms)";
                    //label22.Text = "Camera Refresh Count";
                    label24.Text = "相机未错过延迟(ms)";
                    //label25.Text = "Reset duration when no Tx(sec)";
                }
                m_Language = value;
            }
        }

        private void Ctr_PLC_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232)
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    serialPort1.Close();
                }
            }
        }


        string str = string.Empty;

        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    return;
                }
                if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 3 ||
                    LVApp.Instance().m_Config.m_Cam_Kind[1] == 3 ||
                    LVApp.Instance().m_Config.m_Cam_Kind[2] == 3 ||
                    LVApp.Instance().m_Config.m_Cam_Kind[3] == 3 || t_PLC_D_READ_check || t_PLC_L_READ_check)
                {
                    return;
                }
                byte[] array = new byte[512];
                int temp;
                temp = serialPort1.Read(array, 0, 512);

                //아스키 값으로 받기
                if (ReceiveFormat)
                {
                    MyDelegate dt = delegate ()
                    {
                        txt1.Text = "Receive Data : ";
                        for (int i = 0; i < temp; i++)
                        {
                            txt1.Text += (char)array[i];
                        }
                    };
                    this.Invoke(dt);

                }
                //HEX 값으로 받기
                else
                {
                    str = "";
                    for (int i = 0; i < temp; i++)
                    {
                        str += string.Format("{0:x2} ", array[i]);
                    }

                    //헥사로 바꿔서 출력
                    MyDelegate dt = delegate ()
                    {
                        txt1.Text = "Receive Data : ";
                        txt1.Text += str;
                    };
                    str = "";
                    this.Invoke(dt);
                }


                //bool m_Next = false;
                //string strInData = string.Empty;
                //do
                //{
                //    str = strInData;
                //    string msg = serialPort1.ReadExisting();
                //    strInData += msg;

                //    if (msg.Length == 0)
                //    {
                //        m_Next = true;
                //    }
                //    Thread.Sleep(1);
                //} while (!m_Next);

                ////아스키 형태로 보내라
                //if (SendForamt)
                //{
                //    MyDelegate dt = delegate()
                //    {
                //        txt1.Text = "Receive Data : " + str;
                //    };
                //    this.Invoke(dt);
                //}
                ////HEX 형태로 보내라
                //else
                //{
                //    string str1 = string.Empty;
                //    byte[] arr = Encoding.ASCII.GetBytes((str).ToCharArray());
                //    foreach (byte b in arr)
                //    {
                //        str1 += string.Format("{0:x2}", b);
                //    }
                //    MyDelegate dt = delegate()
                //    {
                //        txt1.Text = "Receive Data : " + str1;
                //    };
                //    this.Invoke(dt);
                //}

            }
            catch
            {
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    if (serialPort1.IsOpen)
                    {
                        //아스키 형태로 보내라
                        if (SendForamt)
                        {
                            serialPort1.Write(txt2.Text);
                        }
                        //HEX 형태로 보내라
                        else
                        {
                            string str = string.Empty;
                            byte[] arr = Encoding.ASCII.GetBytes((txt2.Text).ToCharArray());
                            foreach (byte b in arr)
                            {
                                str += string.Format("{0:x2}", b);
                            }
                            serialPort1.Write(str);
                        }

                        Thread.Sleep(10);
                        bool m_Next = false;
                        string strInData = string.Empty;
                        do
                        {
                            str = strInData;
                            string msg = serialPort1.ReadExisting();
                            strInData += msg;

                            if (msg.Length == 0)
                            {
                                m_Next = true;
                            }
                            Thread.Sleep(1);
                        } while (!m_Next);

                        //아스키 형태로 보내라
                        if (SendForamt)
                        {
                            MyDelegate dt = delegate ()
                            {
                                txt1.Text = "Receive Data : " + str;
                            };
                            this.Invoke(dt);
                        }
                        //HEX 형태로 보내라
                        else
                        {
                            string str1 = string.Empty;
                            byte[] arr = Encoding.ASCII.GetBytes((str).ToCharArray());
                            foreach (byte b in arr)
                            {
                                str1 += string.Format("{0:x2}", b);
                            }
                            MyDelegate dt = delegate ()
                            {
                                txt1.Text = "Receive Data : " + str1;
                            };
                            this.Invoke(dt);
                            str1 = string.Empty;
                        }

                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            LVApp.Instance().m_mainform.add_Log("포트가 연결되지 않았습니다.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            LVApp.Instance().m_mainform.add_Log("Use after connection of PLC.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_mainform.add_Log("端口未连接.");
                        }
                    }
                }
                if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (TCP_Client.Connected)
                    {
                        //아스키 형태로 보내라
                        if (SendForamt)
                        {
                            TCP_Send_String(txt2.Text);
                        }
                        //HEX 형태로 보내라
                        else
                        {
                            string str = string.Empty;
                            byte[] arr = Encoding.ASCII.GetBytes((txt2.Text).ToCharArray());
                            foreach (byte b in arr)
                            {
                                str += string.Format("{0:x2}", b);
                            }
                            TCP_Send_String(str);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            LVApp.Instance().m_mainform.add_Log("포트가 연결되지 않았습니다.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            LVApp.Instance().m_mainform.add_Log("Use after connection of PLC.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_mainform.add_Log("端口未连接.");
                        }
                    }
                }
            }
            catch
            { }
        }

        public bool TCP_Send_String(string m_str)
        {
            try
            {
                if (TCP_Client.Connected)
                {
                    byte[] byte_send = Encoding.UTF8.GetBytes(m_str);
                    //byte [] byte_send = StringToByteArray(m_str);
                    TCP_Client_Stream.Write(byte_send, 0, byte_send.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void cb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                serialPort1.PortName = cbPortName.Text;
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                modbusClient.SerialPort = cbPortName.Text;
            }
        }

        public void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                textBox_Delay0.Enabled =
                textBox_Delay1.Enabled =
                textBox_Delay2.Enabled =
                textBox_Delay3.Enabled =
                Button_Send_Apply.Enabled =
                comboBox_SlaveID.Enabled =
                checkBox_PINGPONG.Enabled = true;

                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    button_LOAD_Click(sender, e);
                    cb1_SelectedIndexChanged(sender, e);
                    cbBaudrate_SelectedIndexChanged(sender, e);
                    cbDataBits_SelectedIndexChanged(sender, e);
                    cbStopBits_SelectedIndexChanged(sender, e);
                    cbParity_SelectedIndexChanged(sender, e);
                    cbReceiveFormat_SelectedIndexChanged(sender, e);
                    cbSendFormat_SelectedIndexChanged(sender, e);
                    comboBox_Protocal_SelectedIndexChanged(sender, e);
                    comboBox_SlaveID_SelectedIndexChanged(sender, e);

                    serialPort1.Open();
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    btnClose.Enabled = true;
                    btnOpen.Enabled = false;
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    if (modbusClient.Connected)
                    {
                        modbusClient.Disconnect();
                    }
                    button_LOAD_Click(sender, e);
                    modbusClient.IPAddress = textBox_SERVER_IP.Text;
                    modbusClient.Port = int.Parse(textBox_SERVER_PORT.Text);
                    modbusClient.SerialPort = null;
                    //modbusClient.receiveDataChanged += new EasyModbus.ModbusClient.ReceiveDataChanged(UpdateReceiveData);
                    //modbusClient.sendDataChanged += new EasyModbus.ModbusClient.SendDataChanged(UpdateSendData);
                    //modbusClient.connectedChanged += new EasyModbus.ModbusClient.ConnectedChanged(UpdateConnectedChanged);

                    modbusClient.Connect();
                    btnClose.Enabled = true;
                    btnOpen.Enabled = false;
                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    if (modbusClient.Connected)
                    {
                        modbusClient.Disconnect();
                    }
                    button_LOAD_Click(sender, e);
                    cb1_SelectedIndexChanged(sender, e);
                    cbBaudrate_SelectedIndexChanged(sender, e);
                    cbDataBits_SelectedIndexChanged(sender, e);
                    cbStopBits_SelectedIndexChanged(sender, e);
                    cbParity_SelectedIndexChanged(sender, e);
                    cbReceiveFormat_SelectedIndexChanged(sender, e);
                    cbSendFormat_SelectedIndexChanged(sender, e);
                    comboBox_Protocal_SelectedIndexChanged(sender, e);
                    comboBox_SlaveID_SelectedIndexChanged(sender, e);

                    modbusClient.Connect();
                    btnClose.Enabled = true;
                    btnOpen.Enabled = false;
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (TCP_Client != null)
                    {
                        if (TCP_Client.Connected)
                        {
                            TCP_Client.Close();
                        }
                        TCP_Client = null;
                    }
                    button_LOAD_Click(sender, e);

                    TCP_Client = new TcpClient(textBox_SERVER_IP.Text, Int32.Parse(textBox_SERVER_PORT.Text));
                    TCP_Client.ReceiveTimeout = 9999999;
                    TCP_Client.SendTimeout = 10000;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    TCP_Client_Stream = TCP_Client.GetStream();

                    btnClose.Enabled = true;
                    btnOpen.Enabled = false;

                    textBox_Delay0.Enabled =
                    textBox_Delay1.Enabled =
                    textBox_Delay2.Enabled =
                    textBox_Delay3.Enabled =
                    Button_Send_Apply.Enabled =
                    comboBox_SlaveID.Enabled =
                    checkBox_PINGPONG.Enabled = false;
                }
                else if (m_Protocal == (int)PROTOCAL.LVDIO)
                {
                    LVApp.Instance().m_DIO.Initialize();
                    m_threads_Check = true;
                    btnClose.Enabled = true;
                    btnOpen.Enabled = false;
                }

                //if (m_Protocal == (int)PROTOCAL.IPSBoard)
                //{
                //    LVApp.Instance().m_mainform.neoTabWindow_EQUIP_SETTING.TabPages[2].Enabled = true;
                //}
                //else
                //{
                //    LVApp.Instance().m_mainform.neoTabWindow_EQUIP_SETTING.TabPages[2].Enabled = false;
                //}

                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "실패";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Fail";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Fail";
                }

                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    PLC_L_WRITE("LX1" + m_Pingpong_Num.ToString("0") + "13", 1); // PLC와 연결확인
                    Thread.Sleep(100);
                    PLC_L_WRITE("LX1" + m_Pingpong_Num.ToString("0") + "13", 1); // PLC와 연결확인
                    Thread.Sleep(100);

                    // MessageBox.Show(PLC_L_READ("LX1013").ToString());
                    if (PLC_L_READ("LX1" + m_Pingpong_Num.ToString("0") + "13") == 1)
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "정상 연결";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        //LVApp.Instance().m_mainform.timer_Refresh_Amount.Start();
                        if (!m_threads_Check)
                        {
                            PLC_Thread_Start();
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                        if (PLC_L_READ("LX1" + m_Pingpong_Num.ToString("0") + "13") == 1)
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "정상 연결";
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                            {//중국어
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                            }
                            //LVApp.Instance().m_mainform.timer_Refresh_Amount.Start();
                            if (!m_threads_Check)
                            {
                                PLC_Thread_Start();
                            }
                        }
                        else
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "에러";
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                            {
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                            }
                            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                            {//중국어
                                LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                            }
                        }
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP || m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    if (modbusClient.Connected)
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "정상 연결";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        //LVApp.Instance().m_mainform.timer_Refresh_Amount.Start();
                        if (!m_threads_Check)
                        {
                            PLC_Thread_Start();
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "에러";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                        }
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (TCP_Client.Connected)
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "정상 연결";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        //LVApp.Instance().m_mainform.timer_Refresh_Amount.Start();
                        if (!m_threads_Check)
                        {
                            PLC_Thread_Start();
                        }
                        //PLC_D_WRITE("EW0006", 1, 1);
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "에러";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                        }
                        //PLC_D_WRITE("EW0006", 1, 0);
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.LVDIO)
                {
                    if (LVApp.Instance().m_DIO.m_Initialized)
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "정상 연결";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Conn.";
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "에러";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                        }
                    }

                    if (!MC_Rx_threads_Check)
                    {
                        MCRx_Thread_Start();
                    }
                    if (!MC_Tx_threads_Check)
                    {
                        MCTx_Thread_Start();
                    }
                }
                button_Send_Save_Click(sender, e);
                for (int i = 0; i < 4; i++)
                {
                    LVApp.Instance().m_Config.Tx_Idx[i] = 0;
                }
                //timer_SEND.Start();
                //send_sw.Reset();
                //send_sw.Start();

                if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    LVApp.Instance().m_mainform.button_STATUS.Visible = true;
                }
                else
                {
                    LVApp.Instance().m_mainform.button_STATUS.Visible = false;
                }


                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && m_Protocal == (int)LV_Inspection_System.GUI.Control.Ctr_PLC.PROTOCAL.IPSBoard)
                {

                }
            }

            catch
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("통신 포트를 열 수 없습니다.");
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "에러";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("Can't open the port.");
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("无法打开端口.");
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Error";
                }
            }

        }

        private string TCP_Custom_RX_MSG = string.Empty;
        //void TCP_Client_MessageReceived(byte[] data)
        //{
        //    var message = e.Message as ScsTextMessage; //Server only accepts text messages
        //    if (message == null)
        //    {
        //        return;
        //    }

        //    TCP_Custom_RX_MSG += message.Text;

        //        ThreadPool.QueueUserWorkItem(delegate
        //        {
        //            Parsing_Rx_for_IPSBOARD();
        //        });

        //    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
        //    {
        //        return;
        //    }

        //    MyDelegate dt = delegate()
        //    {
        //        txt1.Text = message.Text;
        //    };
        //    this.Invoke(dt);
        //}

        private void Parsing_Rx_for_IPSBOARD()
        {
            do
            {
                if (TCP_Custom_RX_MSG.Length >= 3)
                {
                    if (TCP_Custom_RX_MSG.Substring(0, 1) == sSTX().ToString())
                    {
                        //OSVApp.Instance().m_AppConfig.m_Current_Area = TCP_Custom_RX_MSG.Substring(1, 1);
                        //if (OSVApp.Instance().m_AppConfig.m_Current_Area != "A"
                        //    && OSVApp.Instance().m_AppConfig.m_Current_Area != "B"
                        //    && OSVApp.Instance().m_AppConfig.m_Current_Area != "C"
                        //    && OSVApp.Instance().m_AppConfig.m_Current_Area != "D"
                        //    && OSVApp.Instance().m_AppConfig.m_Current_Area != "E"
                        //    && OSVApp.Instance().m_AppConfig.m_Current_Area != "F"
                        //    )
                        //{
                        //    OSVApp.Instance().m_AppConfig.m_Current_Area = "A";
                        //}
                        string t_TCP_Custom_RX_MSG = TCP_Custom_RX_MSG;
                        TCP_Custom_RX_MSG = t_TCP_Custom_RX_MSG.Substring(3);
                    }
                    else
                    {
                        string t_TCP_Custom_RX_MSG = TCP_Custom_RX_MSG;
                        TCP_Custom_RX_MSG = t_TCP_Custom_RX_MSG.Substring(1, t_TCP_Custom_RX_MSG.Length - 1);
                    }
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        public void poolingforconnection()
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (t_Tx_Check > 1000)
                    {
                        //IPS_BOARD.D5D5_Message t_M = LVApp.Instance().m_mainform.ipS_BOARD1.D5D5_P2B;
                        //t_M.RequestNum = 0;
                        //LVApp.Instance().m_mainform.ipS_BOARD1.D5D5_P2B = t_M;
                        //LVApp.Instance().m_mainform.ctr_PLC1.IPSBOARD_MESSAGE_TX(0xD5D5, 0);

                        //LV_Inspection_System.GUI.Control.IPS_BOARD.D0D0_Message t_M = LVApp.Instance().m_mainform.ipS_BOARD1.D0D0_P2B;
                        //if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                        //{
                        //    t_M.Start_Stop = 1;
                        //}
                        //else
                        //{
                        //    t_M.Start_Stop = 0;
                        //}
                        //LVApp.Instance().m_mainform.ipS_BOARD1.D0D0_P2B = t_M;
                        //IPSBOARD_MESSAGE_TX(0xD0D0, 0);
                        t_Tx_Check = 0;
                    }
                    //if (!TCP_Client.Connected)
                    //{
                    //    if (btnClose.InvokeRequired)
                    //    {
                    //        btnClose.Invoke((MethodInvoker)delegate
                    //        {
                    //            if (btnClose.Enabled)
                    //            {
                    //                btnClose_Click(null, null);
                    //            }
                    //        });
                    //    }
                    //    else
                    //    {
                    //        if (btnClose.Enabled)
                    //        {
                    //            btnClose_Click(null, null);
                    //        }
                    //    }

                    //    if (btnOpen.InvokeRequired)
                    //    {
                    //        btnOpen.Invoke((MethodInvoker)delegate
                    //        {
                    //            if (btnOpen.Enabled)
                    //            {
                    //                btnOpen_Click(null, null);
                    //            }
                    //        });
                    //    }
                    //    else
                    //    {
                    //        if (btnOpen.Enabled)
                    //        {
                    //            btnOpen_Click(null, null);
                    //        }
                    //    }
                    //}
                }
                return;
            }

            if (m_Protocal == (int)PROTOCAL.LVDIO)
            {
                return;
            }

            if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            PLC_D_WRITE("DW5" + (m_Pingpong_Num + 1).ToString("0") + "04", 1, 40);
        }

        bool btnClose_Click_check = false;
        public void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                btnClose_Click_check = true;
                if (m_Protocal != (int)PROTOCAL.IPSBoard)
                {
                    PLC_L_WRITE("LX1" + m_Pingpong_Num.ToString("0") + "13", 0);
                }
                //LVApp.Instance().m_mainform.timer_Refresh_Amount.Stop();
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    serialPort1.Close();
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    if (modbusClient.Connected)
                    {
                        modbusClient.Disconnect();
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    if (modbusClient.Connected)
                    {
                        modbusClient.Disconnect();
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                    {

                    }

                    if (TCP_Client != null)
                    {
                        PLC_Thread_Stop();
                        Thread.Sleep(100);
                        if (TCP_Client.Connected)
                        {
                            TCP_Client.Close();
                        }
                        TCP_Client = null;
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.LVDIO)
                {
                    LVApp.Instance().m_DIO.Release();
                    m_threads_Check = false;

                    if (MC_Rx_threads_Check)
                    {
                        MCRx_Thread_Stop();
                    }
                    if (MC_Tx_threads_Check)
                    {
                        MCTx_Thread_Stop();
                    }
                }

                btnClose.Enabled = false;
                btnOpen.Enabled = true;
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["값"] = "끊김";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Disconn.";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_Config.ds_STATUS.Tables["AUTO STATUS"].Rows[2]["Value"] = "Disconn.";
                }

                //timer_SEND.Stop();
                if (m_threads_Check)
                {
                    PLC_Thread_Stop();
                }
                btnClose_Click_check = false;
            }
            catch
            {
                btnClose_Click_check = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txt1.Text = "";
        }

        private void cbReceiveFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbReceiveFormat.Text == "ASCII")
            {
                ReceiveFormat = true;
            }
            else
            {
                ReceiveFormat = false;
            }

        }

        private void cbSendFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSendFormat.Text == "ASCII")
            {
                SendForamt = true;
            }
            else
            {
                SendForamt = false;
            }
        }

        private void cbBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232)
            {
                serialPort1.BaudRate = int.Parse(cbBaudrate.Text);
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                modbusClient.Baudrate = int.Parse(cbBaudrate.Text);
            }
        }

        private void cbDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232)
            {
                serialPort1.DataBits = int.Parse(cbDataBits.Text);
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                //NONE
            }
        }

        private void cbStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                switch (cbStopBits.SelectedIndex)
                {
                    case 0:
                        serialPort1.StopBits = StopBits.One;
                        break;
                    case 1:
                        serialPort1.StopBits = StopBits.OnePointFive;
                        break;
                    case 2:
                        serialPort1.StopBits = StopBits.Two;
                        break;
                    default:
                        serialPort1.StopBits = StopBits.One;
                        break;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                switch (cbStopBits.SelectedIndex)
                {
                    case 0:
                        modbusClient.StopBits = StopBits.One;
                        break;
                    case 1:
                        modbusClient.StopBits = StopBits.OnePointFive;
                        break;
                    case 2:
                        modbusClient.StopBits = StopBits.Two;
                        break;
                    default:
                        modbusClient.StopBits = StopBits.One;
                        break;
                }
            }
        }

        private void cbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                switch (cbParity.SelectedIndex)
                {
                    case 0:
                        serialPort1.Parity = Parity.None;
                        break;
                    case 1:
                        serialPort1.Parity = Parity.Odd;
                        break;
                    case 2:
                        serialPort1.Parity = Parity.Even;
                        break;
                    default:
                        serialPort1.Parity = Parity.None;
                        break;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                switch (cbParity.SelectedIndex)
                {
                    case 0:
                        modbusClient.Parity = Parity.None;
                        break;
                    case 1:
                        modbusClient.Parity = Parity.Odd;
                        break;
                    case 2:
                        modbusClient.Parity = Parity.Even;
                        break;
                    default:
                        modbusClient.Parity = Parity.None;
                        break;
                }
            }
        }

        public void Form_RS232_Load(object sender, EventArgs e)
        {

        }

        public void button_LOAD_Click(object sender, EventArgs e)
        {
            //LVApp.Instance().m_Config.m_Model_Name = LVApp.Instance().m_mainform.textBox_MODEL_NAME.Text;
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
                    return;
                }
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[3];

                    cbPortName.SelectedIndex = cbPortName.FindStringExact(worksheet.Cells[2, 2].Value.ToString());
                    cbBaudrate.SelectedIndex = cbBaudrate.FindStringExact(worksheet.Cells[3, 2].Value.ToString());
                    cbDataBits.SelectedIndex = cbDataBits.FindStringExact(worksheet.Cells[4, 2].Value.ToString());
                    cbStopBits.SelectedIndex = cbStopBits.FindStringExact(worksheet.Cells[5, 2].Value.ToString());
                    cbParity.SelectedIndex = cbParity.FindStringExact(worksheet.Cells[6, 2].Value.ToString());
                    cbReceiveFormat.SelectedIndex = cbReceiveFormat.FindStringExact(worksheet.Cells[7, 2].Value.ToString());
                    cbSendFormat.SelectedIndex = cbSendFormat.FindStringExact(worksheet.Cells[8, 2].Value.ToString());
                    if (worksheet.Cells[9, 2].Value != null && worksheet.Cells[10, 2].Value != null)
                    {
                        comboBox_SlaveID.SelectedIndex = comboBox_SlaveID.FindStringExact(worksheet.Cells[9, 2].Value.ToString());
                        comboBox_Protocal.SelectedIndex = comboBox_Protocal.FindStringExact(worksheet.Cells[10, 2].Value.ToString());
                    }
                    else
                    {
                        comboBox_SlaveID.SelectedIndex = 0;// comboBox_SlaveID.FindStringExact(worksheet.Cells[9, 2].Value.ToString());
                        comboBox_Protocal.SelectedIndex = 0;//comboBox_Protocal.FindStringExact(worksheet.Cells[10, 2].Value.ToString());
                    }

                    if (worksheet.Cells[10, 3].Value != null)
                    {
                        textBox_TxInterval.Text = worksheet.Cells[10, 3].Value.ToString();
                    }
                    else
                    {
                        textBox_TxInterval.Text = "15";
                    }
                    int.TryParse(textBox_TxInterval.Text, out t_Tx_Interval);
                    //if (m_Protocal == (int)PROTOCAL.LVDIO)
                    {
                        LVApp.Instance().m_DIO.m_Trigger_Interval = t_Tx_Interval;
                    }

                    if (worksheet.Cells[10, 4].Value != null)
                    {
                        LVApp.Instance().m_Config.PLC_Pingpong_USE = worksheet.Cells[10, 4].Value.ToString() == "0" ? false : true;
                        checkBox_PINGPONG.Checked = LVApp.Instance().m_Config.PLC_Pingpong_USE;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.PLC_Pingpong_USE = false;
                        checkBox_PINGPONG.Checked = false;
                    }

                    if (worksheet.Cells[10, 5].Value != null)
                    {
                        LVApp.Instance().m_Config.PLC_Once_Tx_USE = worksheet.Cells[10, 5].Value.ToString() == "0" ? false : true;
                        checkBox_AllOnceTx.Checked = LVApp.Instance().m_Config.PLC_Once_Tx_USE;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.PLC_Once_Tx_USE = false;
                        checkBox_AllOnceTx.Checked = false;
                    }

                    if (worksheet.Cells[10, 6].Value != null)
                    {
                        textBox_CheckDelay.Text = worksheet.Cells[10, 6].Value.ToString();
                    }
                    else
                    {
                        textBox_CheckDelay.Text = "100";
                    }
                    int.TryParse(textBox_CheckDelay.Text, out t_Once_Delay);

                    if (worksheet.Cells[10, 7].Value != null)
                    {
                        comboBox_NOOBJECT.SelectedIndex = int.Parse(worksheet.Cells[10, 7].Value.ToString());
                    }
                    else
                    {
                        comboBox_NOOBJECT.SelectedIndex = 0;
                    }
                    LVApp.Instance().m_Config.m_Judge_Priority = comboBox_NOOBJECT.SelectedIndex;
                    //if (worksheet.Cells[11, 2].Value != null)
                    //{
                    //    LVApp.Instance().m_Config.m_SetLanguage = 0;
                    //    LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex = 0;
                    //}
                    //else
                    //{
                    //    LVApp.Instance().m_Config.m_SetLanguage = int.Parse(worksheet.Cells[10, 7].Value.ToString());
                    //    LVApp.Instance().m_mainform.ctr_Model1.comboBox_Language.SelectedIndex = LVApp.Instance().m_Config.m_SetLanguage;
                    //}

                    if (worksheet.Cells[13, 2].Value != null)
                    {
                        textBox_SERVER_IP.Text = worksheet.Cells[13, 2].Value.ToString();
                    }
                    else
                    {
                        textBox_SERVER_IP.Text = "192.168.0.10";
                    }

                    if (worksheet.Cells[13, 3].Value != null)
                    {
                        textBox_SERVER_PORT.Text = worksheet.Cells[13, 3].Value.ToString();
                    }
                    else
                    {
                        textBox_SERVER_PORT.Text = "502";
                    }

                    if (worksheet.Cells[13, 4].Value != null)
                    {
                        LVApp.Instance().m_mainform.Simulation_mode = worksheet.Cells[13, 4].Value.ToString() == "0" ? false : true;
                        checkBox_SIMULATION.Checked = LVApp.Instance().m_mainform.Simulation_mode;
                    }
                    else
                    {
                        LVApp.Instance().m_mainform.Simulation_mode = false;
                        checkBox_SIMULATION.Checked = false;
                    }


                    if (worksheet.Cells[14, 2].Value != null)
                    {
                        textBox_Delay0.Text = worksheet.Cells[14, 2].Value.ToString();
                    }
                    else
                    {
                        textBox_Delay0.Text = "0";
                    }
                    int.TryParse(textBox_Delay0.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[0]);

                    if (worksheet.Cells[14, 3].Value != null)
                    {
                        textBox_Delay1.Text = worksheet.Cells[14, 3].Value.ToString();
                    }
                    else
                    {
                        textBox_Delay1.Text = "0";
                    }
                    int.TryParse(textBox_Delay1.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[1]);

                    if (worksheet.Cells[14, 4].Value != null)
                    {
                        textBox_Delay2.Text = worksheet.Cells[14, 4].Value.ToString();
                    }
                    else
                    {
                        textBox_Delay2.Text = "0";
                    }
                    int.TryParse(textBox_Delay2.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[2]);

                    if (worksheet.Cells[14, 5].Value != null)
                    {
                        textBox_Delay3.Text = worksheet.Cells[14, 5].Value.ToString();
                    }
                    else
                    {
                        textBox_Delay3.Text = "0";
                    }
                    int.TryParse(textBox_Delay3.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[3]);
                    button_Send_Save_Click(sender, e);



                    if (worksheet.Cells[14, 6].Value != null)
                    {
                        textBox_CAMREFCNT.Text = worksheet.Cells[14, 6].Value.ToString();
                    }
                    else
                    {
                        textBox_CAMREFCNT.Text = "10";
                    }
                    int t_v = 10;
                    int.TryParse(textBox_CAMREFCNT.Text, out t_v);
                    if (t_v >= 0)
                    {
                        LVApp.Instance().m_Config.CAM_Refresh_CNT = t_v;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.CAM_Refresh_CNT = 10;
                    }

                    if (worksheet.Cells[14, 7].Value != null)
                    {
                        textBox_ROOMNUM.Text = worksheet.Cells[14, 7].Value.ToString();
                    }
                    else
                    {
                        textBox_ROOMNUM.Text = "2";
                    }
                    t_v = 2;
                    int.TryParse(textBox_ROOMNUM.Text, out t_v);
                    LVApp.Instance().m_Config.Tx_Room_Num = t_v;

                    if (worksheet.Cells[14, 8].Value != null)
                    {
                        LVApp.Instance().m_Config.Tx_Merge = worksheet.Cells[14, 8].Value.ToString() == "0" ? false : true;
                        checkBox_MERGETX.Checked = LVApp.Instance().m_Config.Tx_Merge;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Tx_Merge = false;
                        checkBox_MERGETX.Checked = false;
                    }
                    if (worksheet.Cells[14, 9].Value != null)
                    {
                        LVApp.Instance().m_Config.Disable_Menu = worksheet.Cells[14, 9].Value.ToString() == "0" ? false : true;
                        checkBox_Tab_Enable.Checked = LVApp.Instance().m_Config.Disable_Menu;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Disable_Menu = false;
                        checkBox_Tab_Enable.Checked = false;
                    }


                    if (worksheet.Cells[15, 2].Value != null)
                    {
                        textBox_DELAYCAMMISS.Text = worksheet.Cells[15, 2].Value.ToString();
                    }
                    else
                    {
                        textBox_DELAYCAMMISS.Text = "50";
                    }

                    if (worksheet.Cells[15, 3].Value != null)
                    {
                        textBox_RESETDURATION.Text = worksheet.Cells[15, 3].Value.ToString();
                    }
                    else
                    {
                        textBox_RESETDURATION.Text = "2";
                    }

                    int.TryParse(textBox_DELAYCAMMISS.Text, out m_DELAYCAMMISS);
                    float.TryParse(textBox_RESETDURATION.Text, out m_RESETDURATION);

                    if (worksheet.Cells[15, 4].Value != null)
                    {
                        textBox_MinTime.Text = worksheet.Cells[15, 4].Value.ToString();
                    }
                    else
                    {
                        textBox_MinTime.Text = "50";
                    }
                    int.TryParse(textBox_MinTime.Text, out m_MinProcessingTime);

                    if (worksheet.Cells[15, 5].Value != null)
                    {
                        checkBox_MC.Checked = Use_CAM1_CAM2_ROI2_MC_Tx = worksheet.Cells[15, 5].Value.ToString() == "0" ? false : true;
                    }
                    else
                    {
                        Use_CAM1_CAM2_ROI2_MC_Tx = checkBox_MC.Checked = false;
                    }


                    // 2024.08.24 by CD
                    // START
                    if (worksheet.Cells[15, 6].Value != null)
                    {
                        checkBox_MC_Rx_Use.Checked = Use_MC_Rx = worksheet.Cells[15, 6].Value.ToString() == "0" ? false : true;
                    }
                    else
                    {
                        Use_MC_Rx = checkBox_MC_Rx_Use.Checked = false;
                    }
                    if (worksheet.Cells[15, 7].Value != null)
                    {
                        checkBox_MC_Tx_Use.Checked = Use_MC_Tx = worksheet.Cells[15, 7].Value.ToString() == "0" ? false : true;
                    }
                    else
                    {
                        Use_MC_Tx = checkBox_MC_Tx_Use.Checked = false;
                    }
                    if (worksheet.Cells[15, 8].Value != null)
                    {
                        decimal d_v = 1;
                        decimal.TryParse(worksheet.Cells[15, 8].Value.ToString(), out d_v);
                        numericUpDown_MC_Rx.Value = d_v;
                        MC_Rx_Row_CNT = (int)d_v;
                    }
                    else
                    {
                        numericUpDown_MC_Rx.Value = 0x01;
                        MC_Rx_Row_CNT = 1;
                    }
                    if (worksheet.Cells[15, 9].Value != null)
                    {
                        decimal d_v = 1;
                        decimal.TryParse(worksheet.Cells[15, 9].Value.ToString(), out d_v);
                        numericUpDown_MC_Tx.Value = d_v;
                        MC_Tx_Row_CNT = (int)d_v;
                    }
                    else
                    {
                        numericUpDown_MC_Tx.Value = 0x01;
                        MC_Tx_Row_CNT = 1;
                    }

                    string MC_Rx_filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "MC_Rx_Table.csv";
                    if (System.IO.File.Exists(MC_Rx_filename))
                    {
                        DT_MC_Rx.Clear();
                        DT_MC_Rx.OpenCSVFile(MC_Rx_filename);
                        button_MC_Rx_Apply_Click(sender, e);
                    }
                    DT_MC_Rx.WriteToCsvFile(MC_Rx_filename);
                    string MC_Tx_filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "MC_Tx_Table.csv";
                    if (System.IO.File.Exists(MC_Tx_filename))
                    {
                        DT_MC_Tx.Clear();
                        DT_MC_Tx.OpenCSVFile(MC_Tx_filename);
                        button_MC_Tx_Apply_Click(sender, e);
                    }
                    // END
                }
            }
            finally
            {
                checkBox_PINGPONG.Visible =
                 checkBox_PINGPONG.Enabled =
                label23.Visible =
                textBox_ROOMNUM.Visible =
                checkBox_MERGETX.Visible =
                checkBox_AllOnceTx.Visible =
                label35.Visible =
                textBox_CheckDelay.Visible =
                label25.Visible =
                textBox_RESETDURATION.Visible =
                groupBox_D.Enabled =
                groupBox1.Enabled =
                comboBox_SlaveID.Enabled =
                                label18.Visible =
                    textBox_Delay0.Visible = textBox_Delay0.Enabled =
                    label19.Visible =
                    textBox_Delay1.Visible = textBox_Delay1.Enabled =
                    label20.Visible =
                    textBox_Delay2.Visible = textBox_Delay2.Enabled =
                    label21.Visible =
                    textBox_Delay3.Visible = textBox_Delay3.Enabled =
                    Button_Send_Apply.Visible =
                    Button_Send_Apply.Enabled =
                true;
                if (m_Protocal == (int)PROTOCAL.LVDIO)
                {
                    LVApp.Instance().m_Config.PLC_Pingpong_USE = false;
                    comboBox_Protocal_SelectedIndexChanged(sender, e);
                }

                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("PLC 불러오기 완료.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("PLC Load Completed!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("PLC 设置已加载.");
                }
            }
        }

        public void button_SAVE_Click(object sender, EventArgs e)
        {
            //LVApp.Instance().m_Config.m_Model_Name = LVApp.Instance().m_mainform.textBox_MODEL_NAME.Text;
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
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[3];

                    worksheet.Cells[2, 2].Value = cbPortName.Items[cbPortName.SelectedIndex].ToString();
                    worksheet.Cells[3, 2].Value = cbBaudrate.Items[cbBaudrate.SelectedIndex].ToString();
                    worksheet.Cells[4, 2].Value = cbDataBits.Items[cbDataBits.SelectedIndex].ToString();
                    worksheet.Cells[5, 2].Value = cbStopBits.Items[cbStopBits.SelectedIndex].ToString();
                    worksheet.Cells[6, 2].Value = cbParity.Items[cbParity.SelectedIndex].ToString();
                    worksheet.Cells[7, 2].Value = cbReceiveFormat.Items[cbReceiveFormat.SelectedIndex].ToString();
                    worksheet.Cells[8, 2].Value = cbSendFormat.Items[cbSendFormat.SelectedIndex].ToString();
                    worksheet.Cells[9, 2].Value = comboBox_SlaveID.Items[comboBox_SlaveID.SelectedIndex].ToString();
                    worksheet.Cells[10, 2].Value = comboBox_Protocal.Items[comboBox_Protocal.SelectedIndex < 0 ? 0 : comboBox_Protocal.SelectedIndex].ToString();
                    worksheet.Cells[10, 3].Value = textBox_TxInterval.Text;
                    worksheet.Cells[10, 4].Value = checkBox_PINGPONG.Checked == false ? 0 : 1;
                    worksheet.Cells[10, 5].Value = checkBox_AllOnceTx.Checked == false ? 0 : 1;
                    worksheet.Cells[10, 6].Value = textBox_CheckDelay.Text;
                    worksheet.Cells[10, 7].Value = comboBox_NOOBJECT.SelectedIndex;
                    worksheet.Cells[11, 2].Value = LVApp.Instance().m_Config.m_SetLanguage;
                    worksheet.Cells[13, 2].Value = textBox_SERVER_IP.Text;
                    worksheet.Cells[13, 3].Value = textBox_SERVER_PORT.Text;
                    worksheet.Cells[13, 4].Value = checkBox_SIMULATION.Checked == false ? 0 : 1;

                    worksheet.Cells[14, 2].Value = textBox_Delay0.Text;
                    worksheet.Cells[14, 3].Value = textBox_Delay1.Text;
                    worksheet.Cells[14, 4].Value = textBox_Delay2.Text;
                    worksheet.Cells[14, 5].Value = textBox_Delay3.Text;
                    worksheet.Cells[14, 6].Value = textBox_CAMREFCNT.Text;

                    worksheet.Cells[14, 7].Value = textBox_ROOMNUM.Text;
                    worksheet.Cells[14, 8].Value = checkBox_MERGETX.Checked == false ? 0 : 1;
                    worksheet.Cells[14, 9].Value = checkBox_Tab_Enable.Checked == false ? 0 : 1;

                    worksheet.Cells[15, 2].Value = textBox_DELAYCAMMISS.Text;
                    worksheet.Cells[15, 3].Value = textBox_RESETDURATION.Text;
                    worksheet.Cells[15, 4].Value = textBox_MinTime.Text;

                    worksheet.Cells[15, 5].Value = checkBox_MC.Checked == false ? 0 : 1;

                    // 2024.08.24 by CD
                    // START
                    worksheet.Cells[15, 6].Value = checkBox_MC_Rx_Use.Checked == false ? 0 : 1;
                    worksheet.Cells[15, 7].Value = checkBox_MC_Tx_Use.Checked == false ? 0 : 1;
                    worksheet.Cells[15, 8].Value = ((int)numericUpDown_MC_Rx.Value).ToString();
                    worksheet.Cells[15, 9].Value = ((int)numericUpDown_MC_Tx.Value).ToString();
                    string MC_Rx_filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "MC_Rx_Table.csv";
                    if (System.IO.File.Exists(MC_Rx_filename))
                    {
                        System.IO.File.Delete(MC_Rx_filename);
                    }
                    DT_MC_Rx.WriteToCsvFile(MC_Rx_filename);
                    string MC_Tx_filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + "MC_Tx_Table.csv";
                    if (System.IO.File.Exists(MC_Tx_filename))
                    {
                        System.IO.File.Delete(MC_Tx_filename);
                    }
                    DT_MC_Tx.WriteToCsvFile(MC_Tx_filename);
                    // END


                    int.TryParse(textBox_DELAYCAMMISS.Text, out m_DELAYCAMMISS);
                    float.TryParse(textBox_RESETDURATION.Text, out m_RESETDURATION);

                    int.TryParse(textBox_ROOMNUM.Text, out LVApp.Instance().m_Config.Tx_Room_Num);
                    LVApp.Instance().m_Config.Tx_Merge = checkBox_MERGETX.Checked;

                    int.TryParse(textBox_TxInterval.Text, out t_Tx_Interval);
                    int.TryParse(textBox_CheckDelay.Text, out t_Once_Delay);
                    LVApp.Instance().m_Config.m_Judge_Priority = comboBox_NOOBJECT.SelectedIndex;

                    int t_v = 10;
                    int.TryParse(textBox_CAMREFCNT.Text, out t_v);
                    if (t_v >= 0)
                    {
                        LVApp.Instance().m_Config.CAM_Refresh_CNT = t_v;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.CAM_Refresh_CNT = 10;
                    }

                    int.TryParse(textBox_MinTime.Text, out m_MinProcessingTime);

                    package.Save();

                    if (m_Protocal == (int)PROTOCAL.LVDIO)
                    {
                        LVApp.Instance().m_Config.PLC_Pingpong_USE = false;
                        comboBox_Protocal_SelectedIndexChanged(sender, e);
                    }

                }
                button_Send_Save_Click(sender, e);
            }
            finally
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("PLC 저장 에러.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("PLC Saving Error!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("PLC 保存错误!");
                }
            }
        }

        private void txt2_Enter(object sender, EventArgs e)
        {
            btnSend_Click(sender, e);
        }

        private void txt2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
            }
        }

        public void SerialTx(int OkNg)
        {
            //label9.Text = cam.ToString() + OkNg.ToString();

            if (!serialPort1.IsOpen)
            {
                //send_Message.Clear();
                return;
            }

            if (!m_check_sending)
            {
                if (OkNg >= 51 && OkNg <= 59)
                {
                    //Thread.Sleep(50);
                    m_check_sending = true;
                    serialPort1.Write(sENQ() + "00" + OkNg.ToString("00") + sETX());
                    m_check_sending = false;
                    //LVApp.Instance().m_mainform.add_Log("캠1판정 : " + sENQ() + "00" + OkNg.ToString("00") + sETX());
                }
                else if (OkNg >= 41 && OkNg <= 49)
                {
                    m_check_sending = true;
                    serialPort1.Write(sSTX() + "00" + OkNg.ToString("00") + sETX());
                    m_check_sending = false;
                    //LVApp.Instance().m_mainform.add_Log("캠0판정 : " + sSTX() + "00" + OkNg.ToString("00") + sETX());
                }
                Thread.Sleep(5);
                if (!m_check_sending)
                {
                    if (m_Cam_Trigger_Num < 3)
                    {
                        m_Cam_Trigger_Num = 3;
                        m_Trigger_Check = true;
                        serialPort1.Write(sSTX() + "0094" + sETX());
                    }
                    else if (m_Cam_Trigger_Num == 3)
                    {
                        m_Cam_Trigger_Num = 4;
                        m_Trigger_Check = true;
                        serialPort1.Write(sSTX() + "0095" + sETX());
                    }
                    else if (m_Cam_Trigger_Num == 4)
                    {
                        m_Cam_Trigger_Num = 5;
                        m_Trigger_Check = true;
                        serialPort1.Write(sSTX() + "0096" + sETX());
                    }
                    else if (m_Cam_Trigger_Num == 5)
                    {
                        m_Cam_Trigger_Num = 3;
                        m_Trigger_Check = true;
                        serialPort1.Write(sSTX() + "0094" + sETX());
                    }
                }
            }
            else
            {
                Thread.Sleep(5);
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1);
                    if (!m_check_sending)
                    {
                        if (OkNg >= 51 && OkNg <= 59)
                        {
                            //Thread.Sleep(50);
                            m_check_sending = true;
                            serialPort1.Write(sENQ() + "00" + OkNg.ToString("00") + sETX());
                            m_check_sending = false;
                            break;
                            //LVApp.Instance().m_mainform.add_Log("캠1판정 : " + sENQ() + "00" + OkNg.ToString("00") + sETX());
                        }
                        else if (OkNg >= 41 && OkNg <= 49)
                        {
                            m_check_sending = true;
                            serialPort1.Write(sSTX() + "00" + OkNg.ToString("00") + sETX());
                            m_check_sending = false;
                            break;
                            //LVApp.Instance().m_mainform.add_Log("캠0판정 : " + sSTX() + "00" + OkNg.ToString("00") + sETX());
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                return;
            }
            int ii, jj;
            ii = e.RowIndex;
            jj = e.ColumnIndex;
            //txt2.Text = "+" + e.RowIndex.ToString() + e.ColumnIndex.ToString();
            txt2.Text = "S" + e.RowIndex.ToString() + "1P";
            serialPort1.Write(txt2.Text);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void PLC_Thread_Start()
        {
            m_threads_Check = true;
            //timer_Judge.Start();
            //return;
            for (int i = 0; i < 4; i++)
            {
                send_Message[i].Clear();
                send_Idx[i].Clear();
                send_Message_Time[i].Clear();
                send_Idx_Time[i].Clear();
            }
            // send_Message[2].Clear();
            if (m_Protocal != (int)PROTOCAL.IPSBoard)
            {
                threads[0] = new Thread(ThreadProc0); threads[0].IsBackground = true;
                //threads[0].Priority = ThreadPriority.Normal;
                threads[0].Start();
            }
        }

        public void MCRx_Thread_Start()
        {
            MC_Rx_threads_Check = true;
            MC_Rx_threads = new Thread(ThreadProcMCRx);
            MC_Rx_threads.Start();
        }
        public void MCTx_Thread_Start()
        {
            MC_Tx_threads_Check = true;
            MC_Tx_threads = new Thread(ThreadProcMCTx);
            MC_Tx_threads.Start();
        }

        public void PLC_Thread_Stop()
        {
            try
            {
                m_threads_Check = false;
                //timer_Judge.Stop();
                //return;
                //threads[0].Abort();

                if (m_Protocal != (int)PROTOCAL.IPSBoard)
                {
                    if (threads[0] != null && threads[0].IsAlive)
                    {
                        threads[0].Interrupt();
                        threads[0].Abort();
                        threads[0] = null;
                    }
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {

                }
            }
            catch
            { }
        }

        public void MCRx_Thread_Stop()
        {
            MC_Rx_threads_Check = false;
            Thread.Sleep(100);
            if (MC_Rx_threads != null && MC_Rx_threads.IsAlive)
            {
                MC_Rx_threads.Interrupt();
                MC_Rx_threads.Abort();
                MC_Rx_threads = null;
            }
        }
        public void MCTx_Thread_Stop()
        {
            MC_Tx_threads_Check = false;
            Thread.Sleep(100);
            if (MC_Tx_threads != null && MC_Tx_threads.IsAlive)
            {
                MC_Tx_threads.Interrupt();
                MC_Tx_threads.Abort();
                MC_Tx_threads = null;
            }
        }

        /// PLC로 부터 수신된 데이타를 가지고 온다. ///
        string strRemainData = string.Empty;
        private string DataRead()
        {
            bool m_Next = false;
            string strInData = string.Empty;
            string strRetValue = string.Empty;
            int t_end_idx = -1;
            int t_start_idx = -1;

            DateTime start = DateTime.Now;
            //strInData += strRemainData;
            do
            {
                strRemainData = strInData;
                string msg = serialPort1.ReadExisting();
                strInData += msg;

                if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    // 여기 구현해 넣어야 함.
                }
                else
                {
                    if (strRemainData.Length == strInData.Length)
                    {
                        if (strInData.Length > 12)
                        {
                            t_end_idx = strInData.LastIndexOf(sETX());
                            t_start_idx = strInData.LastIndexOf(sACK());
                            if (t_end_idx > 0 && t_start_idx >= 0 && t_start_idx < t_end_idx)
                            {
                                strInData = strInData.Substring(t_start_idx, t_end_idx - t_start_idx + 1);
                                //txt_Data.Text += strInData;//Test용

                                if (strInData.Length == 13)
                                {
                                    strRetValue = strInData.Substring(10, 2); //실제Data
                                    m_Next = true;
                                }
                                else if (strInData.Length == 15)
                                {
                                    strRetValue = strInData.Substring(10, 4); //실제Data
                                    m_Next = true;
                                }
                                else if (strInData.Length > 15)//00RSS01010000
                                {
                                    strRetValue = strInData.Substring(14, 4) + strInData.Substring(10, 4); //실제Data
                                    m_Next = true;
                                }
                            }
                        }
                    }
                }
                ////TODO : 데이타에 종료문자가 있으면...
                //if (strInData.IndexOf(sETX()) > 0 && strInData.IndexOf(sACK()) == 0)
                //{
                //    //strInData = strInData.Substring(strInData.IndexOf(sACK()), strInData.Length - strInData.IndexOf(sACK()));
                //    //TODO 데이타 처음에 정상 응답이 있으면
                //    //if (strInData[0] == sACK())
                //    //{
                //    //TODO 들어오는 데이타를 분석..[ETX(1)+국번(2)+비트읽기(3)+블륵수(2)]
                //    //txt_Data.Text += strInData ;//Test용

                //    if (strInData.Length == 13)
                //    {
                //        strRetValue = strInData.Substring(10, 2); //실제Data
                //    }
                //    else if (strInData.Length == 15)
                //    {
                //        strRetValue = strInData.Substring(10, 4); //실제Data
                //    }
                //    else if (strInData.Length > 15)//00RSS01010000
                //    {
                //        strRetValue = strInData.Substring(14, 4) + strInData.Substring(10, 4); //실제Data
                //    }
                //    else
                //    {
                //        strRetValue = "1";
                //    }
                //    m_Next = true;
                //    //}
                //    //else
                //    //{
                //    //    string t_str = strRetValue.
                //    //}
                //}

                //    //TODO: 데이타에 비정상 응답이 들어오면..

                //else if (strInData.IndexOf(sETX()) > 0 && strInData.IndexOf(sNAK()) == 0)
                //{
                //    //txt1.Text = "NAK";
                //    strRetValue = "-1";
                //    m_Next = true;
                //}
                //else
                //{
                //    strRemainData += strRetValue;
                //}
                //}

                //DOTO : 응답이 없으면 0.5초간은 로프를둘면서 기다란다.

                TimeSpan ts = DateTime.Now.Subtract(start);

                if (ts.Milliseconds > 100)
                {
                    //txt1.Text = "TimeOut";
                    strRetValue = "-3";
                    m_Next = true;
                }
                Thread.Sleep(1);
            } while (!m_Next);

            return strRetValue;
        }

        private void Response_read()
        {
            bool m_Next = false;
            string strInData = string.Empty;
            int t_old_length = 0; int t_new_length = 0;
            DateTime start = DateTime.Now;
            do
            {
                t_old_length = strInData.Length;
                string msg = serialPort1.ReadExisting();
                strInData += msg;
                t_new_length = strInData.Length;

                if (t_old_length == t_new_length && t_old_length > 0)
                {
                    m_Next = true;
                }

                TimeSpan ts = DateTime.Now.Subtract(start);

                if (ts.Milliseconds > 50)
                {
                    m_Next = true;
                }
            } while (!m_Next);
        }

        private bool t_PLC_D_READ_check = false;
        /// PLC에 m_device를 읽어 오라고 명령한다. ///
        public double PLC_D_READ(string m_device, int m_size)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }
                    t_PLC_D_READ_check = true;
                    while (serialPort1.WriteBufferSize == 0)
                    {
                        //Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    }
                    //DateTime t1 = DateTime.Now;
                    string strOutputData = sENQ() + m_SlaveID + "RSB" + (m_device.Length + 1).ToString("00") + "%" + m_device + m_size.ToString("00") + sEOT(); ;
                    serialPort1.Write(strOutputData);

                    do
                    {
                        //Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    } while (serialPort1.WriteBufferSize == 0);

                    string indata = DataRead();
                    t_PLC_D_READ_check = false;
                    //txt_Data.Text += textBox_D_DEVICE.Text + " : " + indata + " 읽기완료 \r\n";
                    if (indata == null || indata == "")
                    {
                        return -3;
                    }
                    return Convert.ToDouble(indata);
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    t_PLC_D_READ_check = true;
                    double t_out = -3;
                    if (m_size == 1)
                    {
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] serverResponse = modbusClient.ReadHoldingRegisters(nTmp, m_size);
                        t_out = (double)serverResponse[0];
                    }
                    else if (m_size == 2)
                    {
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] serverResponse = modbusClient.ReadHoldingRegisters(nTmp, m_size);
                        t_out = double.Parse(serverResponse[1].ToString("0000") + serverResponse[0].ToString("0000"));
                    }
                    t_PLC_D_READ_check = false;

                    return t_out;
                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    t_PLC_D_READ_check = true;
                    double t_out = -3;
                    if (m_size == 1)
                    {
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] serverResponse = modbusClient.ReadHoldingRegisters(nTmp, m_size);
                        t_out = (double)serverResponse[0];
                    }
                    else if (m_size == 2)
                    {
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] serverResponse = modbusClient.ReadHoldingRegisters(nTmp, m_size);
                        t_out = double.Parse(serverResponse[1].ToString("0000") + serverResponse[0].ToString("0000"));
                    }
                    t_PLC_D_READ_check = false;

                    return t_out;
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }
                    t_PLC_D_READ_check = true;
                    while (serialPort1.WriteBufferSize == 0)
                    {
                        //Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    }
                    //DateTime t1 = DateTime.Now;
                    string strOutputData = m_device + m_size.ToString("00");
                    serialPort1.Write(strOutputData);

                    do
                    {
                        //Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    } while (serialPort1.WriteBufferSize == 0);

                    string indata = DataRead();
                    t_PLC_D_READ_check = false;
                    //txt_Data.Text += textBox_D_DEVICE.Text + " : " + indata + " 읽기완료 \r\n";
                    if (indata == null || indata == "")
                    {
                        return -3;
                    }
                    return Convert.ToDouble(indata);
                }

            }
            catch
            { }
            return -1;
            //TimeSpan span = DateTime.Now.Subtract(t1);
            //txt1.Text = span.Milliseconds.ToString();
        }
        private bool t_PLC_L_READ_check = false;
        /// PLC에 m_device를 읽어 오라고 명령한다. ///
        public double PLC_L_READ(string m_device)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }
                    while (serialPort1.WriteBufferSize == 0)
                    {
                        Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    }

                    if (m_D_Write_check)
                    {
                        return -1;
                    }

                    t_PLC_L_READ_check = true;
                    //DateTime t1 = DateTime.Now;
                    string strOutputData = sENQ() + m_SlaveID + "RSS" + "01" + (m_device.Length + 1).ToString("00") + "%" + m_device + sEOT();
                    serialPort1.Write(strOutputData);

                    do
                    {
                        //데이타를 전부 PLC로 전송 하기 위함..
                    } while (serialPort1.WriteBufferSize == 0);

                    string indata = DataRead();
                    t_PLC_L_READ_check = false;
                    //txt_Data.Text += textBox_D_DEVICE.Text + " : " + indata + " 읽기완료 \r\n";
                    if (indata == null || indata == "")
                    {
                        return -3;
                    }
                    return Convert.ToDouble(indata);
                    //TimeSpan span = DateTime.Now.Subtract(t1);
                    //txt1.Text = span.Milliseconds.ToString();
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    if (!modbusClient.Connected)
                    {
                        return -1;
                    }
                    t_PLC_L_READ_check = true;
                    string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);

                    bool[] serverResponse = modbusClient.ReadCoils(nTmp, 1);
                    t_PLC_L_READ_check = false;
                    if (serverResponse.Length == 0)
                    {
                        return -1;
                    }
                    return Convert.ToDouble(serverResponse[0] == false ? 0d : 1d);
                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    if (!modbusClient.Connected)
                    {
                        return -1;
                    }
                    t_PLC_L_READ_check = true;
                    string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);

                    bool[] serverResponse = modbusClient.ReadCoils(nTmp, 1);
                    t_PLC_L_READ_check = false;
                    if (serverResponse.Length == 0)
                    {
                        return -1;
                    }
                    return Convert.ToDouble(serverResponse[0] == false ? 0d : 1d);
                }
            }
            catch
            { }
            return 0;
        }

        /// PLC에 m_device를 읽어 오라고 명령한다. ///
        public double PLC_D_WRITE(string m_device, int m_size, double m_data)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }
                    m_D_Write_check = true;
                    while (serialPort1.WriteBufferSize == 0)
                    {
                        Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    }

                    //DateTime t1 = DateTime.Now;
                    if (m_size == 1)
                    {
                        //Thread.Sleep(1);
                        string strOutputData = sENQ() + m_SlaveID + "WSS" + m_size.ToString("00") + (m_device.Length + 1).ToString("00") + "%" + m_device + m_data.ToString("0000") + sEOT();
                        serialPort1.Write(strOutputData);
                        //Thread.Sleep(1);
                    }
                    else if (m_size == 2)
                    {
                        string strOutputData = sENQ() + m_SlaveID + "WSB" + (m_device.Length + 1).ToString("00") + "%" + m_device + m_size.ToString("00") + m_data.ToString("00000000").Substring(4, 4) + m_data.ToString("00000000").Substring(0, 4) + sEOT();
                        serialPort1.Write(strOutputData);
                    }

                    m_D_Write_check = false;
                    do
                    {
                        //데이타를 전부 PLC로 전송 하기 위함..
                    } while (serialPort1.WriteBufferSize == 0);

                    Response_read();
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    m_D_Write_check = true;
                    if (m_size == 1)
                    {
                        int registerToSend = int.Parse(m_data.ToString("0000"));
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        modbusClient.WriteSingleRegister(nTmp, registerToSend);
                    }
                    else if (m_size == 2)
                    {
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] registersToSend = new int[m_size];
                        for (int i = 0; i < m_size; i++)
                        {
                            registersToSend[i] = int.Parse(m_data.ToString("00000000").Substring((1 - i) * 4, 4));
                        }
                        modbusClient.WriteMultipleRegisters(nTmp, registersToSend);
                    }
                    m_D_Write_check = false;
                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    m_D_Write_check = true;
                    if (m_size == 1)
                    {
                        int registerToSend = int.Parse(m_data.ToString("0000"));
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        modbusClient.WriteSingleRegister(nTmp, registerToSend);
                    }
                    else if (m_size == 2)
                    {
                        string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] registersToSend = new int[m_size];
                        for (int i = 0; i < m_size; i++)
                        {
                            registersToSend[i] = int.Parse(m_data.ToString("00000000").Substring((1 - i) * 4, 4));
                        }
                        modbusClient.WriteMultipleRegisters(nTmp, registersToSend);
                    }
                    m_D_Write_check = false;
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }
                    m_D_Write_check = true;
                    while (serialPort1.WriteBufferSize == 0)
                    {
                        Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    }

                    //DateTime t1 = DateTime.Now;
                    if (m_size == 1)
                    {
                        //Thread.Sleep(1);
                        string strOutputData = m_device + m_data.ToString("X2");
                        serialPort1.Write(strOutputData);
                        //Thread.Sleep(1);
                    }
                    else if (m_size == 2)
                    {
                        string strOutputData = m_device + m_data.ToString("X4");
                        serialPort1.Write(strOutputData);
                    }

                    m_D_Write_check = false;
                    do
                    {
                        //데이타를 전부 PLC로 전송 하기 위함..
                    } while (serialPort1.WriteBufferSize == 0);

                    Response_read();
                }

                //Response_read();
            }
            catch
            {
                m_D_Write_check = false;
            }

            return 1;

            //string indata = DataRead();
            //m_D_Write_check = false;
            //return Convert.ToDouble(indata);
        }

        /// PLC에 m_device를 읽어 오라고 명령한다. ///
        public double PLC_D_BLOCK_WRITE(string[] m_device, int m_size, double[] m_data)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }

                    m_D_Write_check = true;
                    //while (serialPort1.WriteBufferSize == 0)
                    //{
                    //    Thread.Sleep(10);
                    //    //데이타를 전부 PLC로 전송 하기 위함..
                    //}

                    //DateTime t1 = DateTime.Now;
                    if (m_size == 1)
                    {
                        //Thread.Sleep(1);
                        string strOutputData = sENQ() + m_SlaveID + "WSS" + m_size.ToString("00") + (m_device[0].Length + 1).ToString("00") + "%" + m_device[0] + m_data[0].ToString("0000") + sEOT();
                        //LVApp.Instance().m_mainform.add_Log(strOutputData);
                        serialPort1.Write(strOutputData);
                        //Thread.Sleep(1);
                    }
                    else if (m_size == 2)
                    {
                        string strOutputData = sENQ() + m_SlaveID + "WSS" + m_size.ToString("00") + (m_device[0].Length + 1).ToString("00") + "%" + m_device[0] + m_data[0].ToString("0000") + (m_device[1].Length + 1).ToString("00") + "%" + m_device[1] + m_data[1].ToString("0000") + sEOT();
                        //LVApp.Instance().m_mainform.add_Log(strOutputData);
                        serialPort1.Write(strOutputData);
                    }
                    else if (m_size == 3)
                    {
                        string strOutputData = sENQ() + m_SlaveID + "WSS" + m_size.ToString("00") + (m_device[0].Length + 1).ToString("00") + "%" + m_device[0] + m_data[0].ToString("0000") + (m_device[1].Length + 1).ToString("00") + "%" + m_device[1] + m_data[1].ToString("0000") + (m_device[2].Length + 1).ToString("00") + "%" + m_device[2] + m_data[2].ToString("0000") + sEOT();
                        //LVApp.Instance().m_mainform.add_Log(strOutputData);
                        serialPort1.Write(strOutputData);
                    }
                    else if (m_size == 4)
                    {
                        string strOutputData = sENQ() + m_SlaveID + "WSS" + m_size.ToString("00") + (m_device[0].Length + 1).ToString("00") + "%" + m_device[0] + m_data[0].ToString("0000") + (m_device[1].Length + 1).ToString("00") + "%" + m_device[1] + m_data[1].ToString("0000") + (m_device[2].Length + 1).ToString("00") + "%" + m_device[2] + m_data[2].ToString("0000") + (m_device[3].Length + 1).ToString("00") + "%" + m_device[3] + m_data[3].ToString("0000") + sEOT();
                        //LVApp.Instance().m_mainform.add_Log(strOutputData);
                        serialPort1.Write(strOutputData);
                    }

                    m_D_Write_check = false;
                    do
                    {
                        //데이타를 전부 PLC로 전송 하기 위함..
                    } while (serialPort1.WriteBufferSize == 0);
                    //Response_read();

                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    if (!modbusClient.Connected)
                    {
                        return -1;
                    }

                    m_D_Write_check = true;

                    if (LVApp.Instance().m_Config.Tx_Merge)
                    {
                        string strTmp = Regex.Replace(m_device[0], @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] t_sv = new int[m_data.Length];
                        for (int i = 0; i < m_data.Length; i++)
                        {
                            t_sv[i] = (int)m_data[i];
                        }
                        //if (this.InvokeRequired)
                        //{
                        //    this.Invoke((MethodInvoker)delegate
                        //    {
                        //        modbusClient.WriteMultipleRegisters(nTmp, t_sv);
                        //    });
                        //}
                        //else
                        //{
                        modbusClient.WriteMultipleRegisters(nTmp, t_sv);
                        //}
                    }
                    else
                    {
                        for (int i = 0; i < m_size; i++)
                        {
                            int registerToSend = int.Parse(m_data[i].ToString("0000"));
                            string strTmp = Regex.Replace(m_device[i], @"\D", ""); int nTmp = int.Parse(strTmp);
                            //if (this.InvokeRequired)
                            //{
                            //    this.Invoke((MethodInvoker)delegate
                            //{
                            modbusClient.WriteSingleRegister(nTmp, registerToSend);
                            //    });
                            //}
                            //else
                            //{
                            //    modbusClient.WriteSingleRegister(nTmp, registerToSend);
                            //}
                            Thread.Sleep(t_Tx_Interval);
                        }
                    }

                    m_D_Write_check = false;

                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    if (!modbusClient.Connected)
                    {
                        return -1;
                    }
                    m_D_Write_check = true;

                    if (LVApp.Instance().m_Config.Tx_Merge)
                    {
                        string strTmp = Regex.Replace(m_device[0], @"\D", ""); int nTmp = int.Parse(strTmp);
                        int[] t_sv = new int[m_data.Length];
                        for (int i = 0; i < m_data.Length; i++)
                        {
                            t_sv[i] = (int)m_data[i];
                        }
                        //if (this.InvokeRequired)
                        //{
                        //    this.Invoke((MethodInvoker)delegate
                        //    {
                        //        modbusClient.WriteMultipleRegisters(nTmp, t_sv);
                        //    });
                        //}
                        //else
                        //{
                        modbusClient.WriteMultipleRegisters(nTmp, t_sv);
                        //}
                    }
                    else
                    {
                        for (int i = 0; i < m_size; i++)
                        {
                            int registerToSend = int.Parse(m_data[i].ToString("0000"));
                            string strTmp = Regex.Replace(m_device[i], @"\D", ""); int nTmp = int.Parse(strTmp);
                            //if (this.InvokeRequired)
                            //{
                            //    this.Invoke((MethodInvoker)delegate
                            //    {
                            //        modbusClient.WriteSingleRegister(nTmp, registerToSend);
                            //    });
                            //}
                            //else
                            //{
                            modbusClient.WriteSingleRegister(nTmp, registerToSend);
                            //}
                            Thread.Sleep(t_Tx_Interval);
                        }
                    }

                    //int[] registersToSend = new int[m_size];

                    //for (int i = 0; i < m_size; i++)
                    //{

                    //    registersToSend[i] = (int)(m_data[i]);
                    //}

                    //string strTmp = Regex.Replace(m_device[0], @"\D", ""); int nTmp = int.Parse(strTmp);
                    //modbusClient.WriteMultipleRegisters(nTmp, registersToSend);
                    m_D_Write_check = false;
                }
                else if (m_Protocal == (int)PROTOCAL.IPSBoard)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }

                    m_D_Write_check = true;

                    for (int i = 0; i < m_size; i++)
                    {
                        string strOutputData = m_device[i] + m_data[i].ToString("00");
                        //LVApp.Instance().m_mainform.add_Log(strOutputData);
                        serialPort1.Write(strOutputData);
                        do
                        {
                            Thread.Sleep(1);
                            //데이타를 전부 PLC로 전송 하기 위함..
                        } while (serialPort1.WriteBufferSize == 0);
                        bool m_Next = false;
                        int t_time = 0;
                        string strInData = string.Empty;
                        do
                        {
                            str = strInData;
                            string msg = serialPort1.ReadExisting();
                            strInData += msg;

                            if (msg.Length == 0 && str.Length >= 1)
                            {
                                m_Next = true;
                            }
                            Thread.Sleep(1);
                            t_time++;

                            if (t_time > 30)
                            {
                                m_Next = true;
                            }
                        } while (!m_Next);
                    }

                    m_D_Write_check = false;
                    //Response_read();
                }
            }
            catch
            {
                m_D_Write_check = false;
            }
            //DataRead();
            //serialPort1.ReadExisting();
            return 1;

            //string indata = DataRead();
            //m_D_Write_check = false;
            //return Convert.ToDouble(indata);
        }

        /// PLC에 m_device를 읽어 오라고 명령한다. ///
        public double PLC_L_WRITE(string m_device, double m_data)
        {
            try
            {
                if (m_Protocal == (int)PROTOCAL.XGTRS232)
                {
                    if (!serialPort1.IsOpen)
                    {
                        return -1;
                    }
                    while (serialPort1.WriteBufferSize == 0)
                    {
                        Thread.Sleep(1);
                        //데이타를 전부 PLC로 전송 하기 위함..
                    }
                    lock (this)
                    {
                        //DateTime t1 = DateTime.Now;
                        string strOutputData = sENQ() + m_SlaveID + "WSS" + "01" + (m_device.Length + 1).ToString("00") + "%" + m_device + m_data.ToString("00") + sEOT();
                        serialPort1.Write(strOutputData);

                        do
                        {
                            //데이타를 전부 PLC로 전송 하기 위함..
                        } while (serialPort1.WriteBufferSize == 0);

                        //string indata = DataRead();
                        //txt_Data.Text += textBox_D_DEVICE.Text + " : " + indata + " 읽기완료 \r\n";
                        //if (indata == null || indata == "")
                        //{
                        //    return -3d;
                        //}
                        Response_read();
                        return 1d;//Convert.ToDouble(indata);
                    }
                    //TimeSpan span = DateTime.Now.Subtract(t1);
                    //txt1.Text = span.Milliseconds.ToString();
                }
                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                {
                    if (!modbusClient.Connected)
                    {
                        return -1;
                    }

                    bool coilsToSend = false;

                    coilsToSend = m_data == 0d ? false : true;
                    string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);

                    modbusClient.WriteSingleCoil(nTmp, coilsToSend);

                }
                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                {
                    if (!modbusClient.Connected)
                    {
                        return -1;
                    }

                    bool coilsToSend = false;

                    coilsToSend = m_data == 0d ? false : true;
                    string strTmp = Regex.Replace(m_device, @"\D", ""); int nTmp = int.Parse(strTmp);

                    modbusClient.WriteSingleCoil(nTmp, coilsToSend);
                }
            }
            catch
            { }
            return 1d;
        }


        private void button_D_READ_Click(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                if (!serialPort1.IsOpen)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            txt_Data.ResetText();
            double m_data = PLC_D_READ(textBox_D_DEVICE.Text, Convert.ToInt32(textBox_D_SIZE.Text));
            txt_Data.Text += textBox_D_DEVICE.Text + " : " + m_data.ToString() + " Recieved \r\n";
        }

        private void button_D_WRITE_Click(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                if (!serialPort1.IsOpen)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            txt_Data.ResetText();
            double m_data = PLC_D_WRITE(textBox_D_DEVICE.Text, Convert.ToInt32(textBox_D_SIZE.Text), Convert.ToDouble(textBox_D_DATA.Text));
            if (m_data == 1)
            {
                txt_Data.Text += textBox_D_DEVICE.Text + " : Send \r\n";
            }
            else
            {
                txt_Data.Text += textBox_D_DEVICE.Text + " : Send fail \r\n";
            }
        }

        private void button_L_READ_Click(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                if (!serialPort1.IsOpen)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            txt_Data.ResetText();
            double m_data = PLC_L_READ(textBox_L_DEVICE.Text);
            txt_Data.Text += textBox_L_DEVICE.Text + " : " + m_data.ToString() + " Recieved \r\n";
        }

        private void button_L_WRITE_Click(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
            {
                if (!serialPort1.IsOpen)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                if (!modbusClient.Connected)
                {
                    return;
                }
            }
            txt_Data.ResetText();
            double m_data = PLC_L_WRITE(textBox_L_DEVICE.Text, Convert.ToDouble(textBox_L_DATA.Text));
            if (m_data == 1)
            {
                txt_Data.Text += textBox_L_DEVICE.Text + " : Send \r\n";
            }
            else
            {
                txt_Data.Text += textBox_L_DEVICE.Text + " : Send fail \r\n";
            }
        }

        private void comboBox_SlaveID_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_SlaveID = comboBox_SlaveID.Items[comboBox_SlaveID.SelectedIndex].ToString();
            int.TryParse(m_SlaveID, out m_Pingpong_Num); m_Pingpong_Num %= 2;
            int.TryParse(m_SlaveID, out LVApp.Instance().m_Config.PLC_Station_Num);
            if (m_Protocal == (int)PROTOCAL.ModbusRTU)
            {
                modbusClient.UnitIdentifier = byte.Parse(LVApp.Instance().m_Config.PLC_Station_Num.ToString());
            }
        }

        private void comboBox_Protocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_Protocal = comboBox_Protocal.SelectedIndex;
            if (m_Protocal < 0)
            {
                return;
            }
            checkBox_PINGPONG.Visible =
             checkBox_PINGPONG.Enabled =
            label23.Visible =
            textBox_ROOMNUM.Visible =
            checkBox_MERGETX.Visible =
            checkBox_AllOnceTx.Visible =
            label35.Visible =
            textBox_CheckDelay.Visible =
            label25.Visible =
            textBox_RESETDURATION.Visible =
            groupBox_D.Enabled =
            groupBox1.Enabled =
            comboBox_SlaveID.Enabled =
                            label18.Visible =
                textBox_Delay0.Visible = textBox_Delay0.Enabled =
                label19.Visible =
                textBox_Delay1.Visible = textBox_Delay1.Enabled =
                label20.Visible =
                textBox_Delay2.Visible = textBox_Delay2.Enabled =
                label21.Visible =
                textBox_Delay3.Visible = textBox_Delay3.Enabled =
                Button_Send_Apply.Visible =
                Button_Send_Apply.Enabled =
                label15.Visible =
                comboBox_SlaveID.Visible =
                label22.Visible =
                textBox_CAMREFCNT.Visible =
            true;
            if (m_Protocal == (int)PROTOCAL.ModbbusTCP || m_Protocal == (int)PROTOCAL.XGTTCP)
            {
                label37.Visible = true;
                label38.Visible = true;
                textBox_SERVER_IP.Visible = true;
                textBox_SERVER_PORT.Visible = true;

                label2.Visible = false; cbReceiveFormat.Visible = false;
                label3.Visible = false; cbSendFormat.Visible = false;
                label4.Visible = false; cbPortName.Visible = false;
                label5.Visible = false; cbBaudrate.Visible = false;
                label6.Visible = false; cbDataBits.Visible = false;
                label7.Visible = false; cbStopBits.Visible = false;
                label8.Visible = false; cbParity.Visible = false;
            }
            else if (m_Protocal == (int)PROTOCAL.LVDIO)
            {
                //label17.Text = "최소처리시간(ms)";
                label37.Visible = true;
                label38.Visible = true;
                textBox_SERVER_IP.Visible = true; //textBox_SERVER_IP.Text = "192.168.0.251";
                textBox_SERVER_PORT.Visible = true;// textBox_SERVER_PORT.Text = "8082";

                label2.Visible = false; cbReceiveFormat.Visible = false;
                label3.Visible = false; cbSendFormat.Visible = false;
                label4.Visible = false; cbPortName.Visible = false;
                label5.Visible = false; cbBaudrate.Visible = false;
                label6.Visible = false; cbDataBits.Visible = false;
                label7.Visible = false; cbStopBits.Visible = false;
                label8.Visible = false; cbParity.Visible = false;

                if (m_threads_Check)
                {
                    btnOpen.Enabled = false;
                    btnClose.Enabled = true;
                }
                else
                {
                    btnOpen.Enabled = true;
                    btnClose.Enabled = false;
                }

                label15.Visible =
                comboBox_SlaveID.Visible =
                checkBox_PINGPONG.Visible =
                 checkBox_PINGPONG.Enabled =
                label23.Visible =
                textBox_ROOMNUM.Visible =
                checkBox_MERGETX.Visible =
                checkBox_AllOnceTx.Visible =
                label35.Visible =
                textBox_CheckDelay.Visible =
                label25.Visible =
                textBox_RESETDURATION.Visible =
                groupBox_D.Enabled =
                groupBox1.Enabled =
                comboBox_SlaveID.Enabled =
                                label18.Visible =
                    textBox_Delay0.Visible = textBox_Delay0.Enabled =
                    label19.Visible =
                    textBox_Delay1.Visible = textBox_Delay1.Enabled =
                    label20.Visible =
                    textBox_Delay2.Visible = textBox_Delay2.Enabled =
                    label21.Visible =
                    textBox_Delay3.Visible = textBox_Delay3.Enabled =
                    Button_Send_Apply.Visible =
                    Button_Send_Apply.Enabled =
                label22.Visible =
                textBox_CAMREFCNT.Visible =
                false;
            }
            else
            {
                label37.Visible = false;
                label38.Visible = false;
                textBox_SERVER_IP.Visible = false;
                textBox_SERVER_PORT.Visible = false;

                label2.Visible = true; cbReceiveFormat.Visible = true;
                label3.Visible = true; cbSendFormat.Visible = true;
                label4.Visible = true; cbPortName.Visible = true;
                label5.Visible = true; cbBaudrate.Visible = true;
                label6.Visible = true; cbDataBits.Visible = true;
                label7.Visible = true; cbStopBits.Visible = true;
                label8.Visible = true; cbParity.Visible = true;
            }
            //if (m_Protocal == (int)PROTOCAL.IPSBoard)
            //{
            //    LVApp.Instance().m_mainform.neoTabWindow_EQUIP_SETTING.TabPages[2].Enabled = true;
            //}
            //else
            //{
            //  LVApp.Instance().m_mainform.neoTabWindow_EQUIP_SETTING.TabPages[2].Enabled = false;
            //}
        }

        private void button_View_Click(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.XGTRS232)
            {
                if (!serialPort1.IsOpen)
                {
                    return;
                }
                if (button_View.Text == "View")
                {
                    button_View.Text = "Stop";
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                }
                else
                {
                    button_View.Text = "View";
                    serialPort1.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                }
            }
        }

        public int t_Tx_Interval = 15;
        private void textBox_TxInterval_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(textBox_TxInterval.Text, out t_Tx_Interval);
                if (m_Protocal == (int)PROTOCAL.LVDIO)
                {
                    LVApp.Instance().m_DIO.m_Trigger_Interval = t_Tx_Interval;
                }
            }
        }

        private void checkBox_PINGPONG_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_PINGPONG.Checked)
            {
                checkBox_AllOnceTx.Checked = false;
                LVApp.Instance().m_Config.PLC_Once_Tx_USE = checkBox_AllOnceTx.Checked;
            }
            LVApp.Instance().m_Config.PLC_Pingpong_USE = checkBox_PINGPONG.Checked;
        }

        private void checkBox_AllOnceTx_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_AllOnceTx.Checked)
            {
                checkBox_PINGPONG.Checked = false;
                LVApp.Instance().m_Config.PLC_Pingpong_USE = checkBox_PINGPONG.Checked;
            }
            LVApp.Instance().m_Config.PLC_Once_Tx_USE = checkBox_AllOnceTx.Checked;
            if (LVApp.Instance().m_Config.PLC_Once_Tx_USE)
            {
                LVApp.Instance().m_mainform.ctrCam1.m_Camera_Interval = 1;
                LVApp.Instance().m_mainform.ctrCam2.m_Camera_Interval = 1;
                LVApp.Instance().m_mainform.ctrCam3.m_Camera_Interval = 1;
                LVApp.Instance().m_mainform.ctrCam4.m_Camera_Interval = 1;
            }
            else
            {
                LVApp.Instance().m_mainform.ctrCam1.m_Camera_Interval = 30;
                LVApp.Instance().m_mainform.ctrCam2.m_Camera_Interval = 30;
                LVApp.Instance().m_mainform.ctrCam3.m_Camera_Interval = 30;
                LVApp.Instance().m_mainform.ctrCam4.m_Camera_Interval = 30;
            }
        }

        private int t_Once_Delay = 100;
        private void ThreadProc0()
        {
            int t_time_cnt = 0;
            int t_cnt = 0;
            int t_send_num = 0;
            int t_send_num_M = 0;
            int i = 0; int j = 0;
            const int cam_num = 4;
            string[] t_send_Device_M = new string[cam_num];
            double[] t_send_Data_M = new double[cam_num];

            string[] t_send_Device = new string[cam_num];
            double[] t_send_Data = new double[cam_num];
            Stopwatch[] t_SW = new Stopwatch[5];
            t_SW[0] = new Stopwatch(); t_SW[1] = new Stopwatch(); t_SW[2] = new Stopwatch(); t_SW[3] = new Stopwatch(); t_SW[4] = new Stopwatch();
            int[] t_mcnt = new int[4];
            t_mcnt[0] = t_mcnt[1] = t_mcnt[2] = t_mcnt[3] = 0;
            bool[] t_all_true = new bool[4];
            t_all_true[0] = t_all_true[1] = t_all_true[2] = t_all_true[3] = true;
            double t_Max_NG = 10; int t_Cam_CNT = -1; //int t_Message_CNT = 0;
            string log_str_M = ""; bool t_RESETDURATION_check = false;

            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                LVApp.Instance().m_mainform.add_Log("PLC 쓰레드 시작.");
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                LVApp.Instance().m_mainform.add_Log("PLC Thread Start.");
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                LVApp.Instance().m_mainform.add_Log("PLC 线程启动.");
            }

            try
            {
                while (m_threads_Check)
                {
                    if (!m_threads_Check)
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            LVApp.Instance().m_mainform.add_Log("PLC 쓰레드 종료.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            LVApp.Instance().m_mainform.add_Log("PLC Thread terminated.");
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            LVApp.Instance().m_mainform.add_Log("PLC 线程终止.");
                        }
                        break;
                    }
                    if (!LVApp.Instance().m_Config.PLC_Once_Tx_USE)
                    {// 검사하고 바로 보내기

                        if (LVApp.Instance().m_Config.Tx_Merge)
                        {// 카메라 수만큼 메세지가 들어오면 합쳐서 보냄

                            if (t_SW[4].ElapsedMilliseconds > (int)(m_RESETDURATION * 1000) && t_RESETDURATION_check == false)
                            {
                                for (int k = 0; k < LVApp.Instance().m_Config.m_Cam_Total_Num; k++)
                                {
                                    LVApp.Instance().m_Config.Tx_Idx[k] = 0;
                                    send_Message[k].Clear();
                                    //send_Idx[k].Clear();
                                }
                                LVApp.Instance().m_mainform.add_Log("Reset Tx address!");
                                t_RESETDURATION_check = true;
                            }

                            if (t_Cam_CNT == -1)
                            {
                                t_Cam_CNT = 0;
                                for (j = 0; j < 4; j++)
                                {
                                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[j])
                                    {
                                        t_Cam_CNT++;
                                    }
                                    t_send_Device_M[j] = "";
                                    t_send_Data_M[j] = 0;
                                    t_send_Device[j] = "";
                                    t_send_Data[j] = 0;
                                }
                            } // 초기화 완료


                            bool t_send_check = false;
                            t_send_num = 0;
                            for (j = 0; j < 4; j++)
                            {
                                t_send_Data[j] = -1;
                                t_cnt = send_Message[j].Count;
                                if (t_cnt > 0)
                                {
                                    string[] cp_msg;
                                    lock (send_Message[j])
                                    {
                                        cp_msg = send_Message[j][0].Split('_');
                                        send_Message[j].RemoveAt(0);
                                    }

                                    if (cp_msg.Length == 2)
                                    {// 일반 메세지
                                        t_send_Device[j] = cp_msg[0];
                                        t_send_Data[j] = Convert.ToDouble(cp_msg[1]);
                                        t_send_num++;
                                        t_send_check = true;
                                    }
                                    else if (cp_msg.Length == 3)
                                    {//결과 메세지
                                        if (t_send_Device_M[j].Length == 0)
                                        {
                                            t_send_Device_M[j] = cp_msg[0];
                                            t_send_Data_M[j] = Convert.ToDouble(cp_msg[1]);
                                            t_send_num_M++;
                                            log_str_M += "C" + j.ToString() + "[" + t_send_Device_M[j] + "_" + t_send_Data_M[j].ToString() + "] ";
                                        }
                                    }
                                }
                            }

                            if (t_send_num_M == t_Cam_CNT)
                            {
                                PLC_D_BLOCK_WRITE(t_send_Device_M, t_send_num_M, t_send_Data_M);
                                if (LVApp.Instance().m_Config.PLC_Judge_view)
                                {
                                    LVApp.Instance().m_mainform.add_Log(log_str_M);
                                }
                                for (j = 0; j < 4; j++)
                                {
                                    t_send_Device_M[j] = "";
                                    t_send_Data_M[j] = 0;
                                }
                                log_str_M = "";
                                t_send_num_M = 0;
                                t_RESETDURATION_check = false;
                                t_SW[4].Reset(); t_SW[4].Start();
                                Thread.Sleep(t_Tx_Interval);
                            }
                            else if (t_send_check)
                            {
                                string[] tt_send_Device = new string[t_send_num];
                                double[] tt_send_Data = new double[t_send_num];
                                string log_str = "";
                                t_send_num = 0;
                                for (j = 0; j < cam_num; j++)
                                {
                                    if (t_send_Data[j] >= 0)
                                    {
                                        tt_send_Device[t_send_num] = t_send_Device[j];
                                        tt_send_Data[t_send_num] = t_send_Data[j];
                                        t_send_num++;
                                        log_str += "C" + j.ToString() + "[" + t_send_Device[j] + "_" + t_send_Data[j].ToString() + "] ";
                                    }
                                }
                                PLC_D_BLOCK_WRITE(tt_send_Device, t_send_num, tt_send_Data);
                                if (LVApp.Instance().m_Config.PLC_Judge_view)
                                {
                                    LVApp.Instance().m_mainform.add_Log(log_str);
                                }
                                Thread.Sleep(t_Tx_Interval);
                            }
                        }
                        else
                        {// 메세지가 들어오면 되는대로 보냄
                            t_send_num = 0;
                            //CAM0,1,2번 판정
                            for (i = 0; i < cam_num; i++)
                            {
                                t_send_Data[i] = -1;
                                t_cnt = send_Message[i].Count;
                                if (t_cnt > 3)
                                {
                                    t_Tx_Interval--;
                                    if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                                    {
                                        if (t_Tx_Interval < 30)
                                        {
                                            t_Tx_Interval = 30;
                                        }
                                    }
                                    else
                                    {
                                        if (t_Tx_Interval < 20)
                                        {
                                            t_Tx_Interval = 20;
                                        }
                                    }
                                    if (textBox_TxInterval.InvokeRequired)
                                    {
                                        textBox_TxInterval.Invoke((MethodInvoker)delegate
                                        {
                                            textBox_TxInterval.Text = t_Tx_Interval.ToString();
                                        });
                                    }
                                    else
                                    {
                                        textBox_TxInterval.Text = t_Tx_Interval.ToString();
                                    }
                                }
                                if (t_cnt > 0)
                                {
                                    //if (LVApp.Instance().m_Config.PLC_Judge_view)
                                    //{
                                    //    LVApp.Instance().m_mainform.add_Log("C:" + i.ToString() + "M:" + t_cnt);
                                    //}
                                    //string t_m = (string)send_Message[i].Dequeue();
                                    //if (t_m != null)
                                    //{
                                    //    string[] t_send_Message = t_m.Split('_');
                                    lock (send_Message[i])
                                    {
                                        if (send_Message[i][0] != null)
                                        {
                                            string[] t_send_Message = send_Message[i][0].Split('_');

                                            if (t_send_Message.Length >= 2)
                                            {
                                                t_send_Device[i] = t_send_Message[0];
                                                t_send_Data[i] = Convert.ToDouble(t_send_Message[1]);
                                                t_send_num++;
                                            }
                                            //if (i < 3)
                                            //{
                                            //    send_Message[i].Clear();
                                            //}
                                            //else
                                            //{
                                            send_Message[i].RemoveAt(0);
                                            //}
                                        }
                                        //else if (t_m == null)
                                        //{
                                        //    //send_Message[i].RemoveAt(0);
                                        //}
                                        else if (send_Message[i][0] == null)
                                        {
                                            send_Message[i].RemoveAt(0);
                                        }
                                    }
                                }
                                //if (send_Message[i].Count > 2)
                                //{
                                //    send_Message[i].Clear();
                                //}
                            }

                            if (t_send_num > 0)
                            {
                                string[] tt_send_Device = new string[t_send_num];
                                double[] tt_send_Data = new double[t_send_num];
                                string log_str = "";
                                t_send_num = 0;
                                for (j = 0; j < cam_num; j++)
                                {
                                    if (t_send_Data[j] >= 0)
                                    {
                                        tt_send_Device[t_send_num] = t_send_Device[j];
                                        tt_send_Data[t_send_num] = t_send_Data[j];
                                        t_send_num++;
                                        log_str += "C" + j.ToString() + "[" + t_send_Device[j] + "_" + t_send_Data[j].ToString() + "] ";
                                    }
                                    //else
                                    //{
                                    //    log_str += "C" + j.ToString() + "[No] ";
                                    //}
                                }
                                if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
                                {
                                    bool t_write_check = false;
                                    while (serialPort1.WriteBufferSize == 0)
                                    {
                                        Thread.Sleep(1);
                                        t_write_check = true;
                                        //데이타를 전부 PLC로 전송 하기 위함..
                                    }
                                    if (t_write_check)
                                    {
                                        Thread.Sleep(11);
                                    }
                                }
                                else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                                {

                                }
                                else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                                {

                                }

                                PLC_D_BLOCK_WRITE(tt_send_Device, t_send_num, tt_send_Data);
                                if (LVApp.Instance().m_Config.PLC_Judge_view)
                                {
                                    LVApp.Instance().m_mainform.add_Log(log_str);
                                }
                                Thread.Sleep(t_Tx_Interval);
                                t_time_cnt = 0;
                            }
                            else
                            {
                                t_time_cnt = t_Tx_Interval;
                            }
                        }
                    }
                    else
                    {// 검사하고 일정 delay후 값이 더이상 안들어오면 보내기
                        Thread.Sleep(t_Tx_Interval);

                        for (i = 0; i < cam_num; i++)
                        {
                            t_send_Data[i] = -1;
                            t_cnt = send_Message[i].Count;
                            if (t_cnt > t_mcnt[i])
                            {
                                t_mcnt[i] = t_cnt;
                                t_SW[i].Reset(); t_SW[i].Start();
                            }
                            else
                            {
                                if (t_SW[i].ElapsedMilliseconds >= t_Once_Delay && t_mcnt[i] > 0)
                                {
                                    t_SW[i].Stop();
                                    lock (send_Message[i])
                                    {
                                        t_mcnt[i] = send_Message[i].Count;
                                        for (j = 0; j < t_mcnt[i]; j++)
                                        {
                                            if (send_Message[i][j] != null)
                                            {
                                                string[] t_send_Message = send_Message[i][j].Split('_');
                                                if (t_send_Message.Length >= 2)
                                                {

                                                    //string t_m = (string)send_Message[i].Dequeue();
                                                    //if (t_m != null)
                                                    //{
                                                    //    string[] t_send_Message = t_m.Split('_');
                                                    //    if (t_send_Message.Length == 2)
                                                    //{
                                                    if (j == 0)
                                                    {
                                                        t_send_Device[i] = t_send_Message[0];
                                                    }
                                                    t_send_Data[i] = Convert.ToDouble(t_send_Message[1]);
                                                    if (t_send_Data[i] != 40)
                                                    {
                                                        if (t_Max_NG < t_send_Data[i])
                                                        {
                                                            t_Max_NG = t_send_Data[i];
                                                        }
                                                        t_all_true[i] = false;
                                                        //break;
                                                    }
                                                }
                                            }
                                        }
                                        send_Message[i].Clear();
                                        //send_Idx[i].Clear();
                                    }

                                    if (t_all_true[i])
                                    {
                                        t_send_Data[i] = 40;
                                    }
                                    else
                                    {
                                        t_send_Data[i] = t_Max_NG;
                                    }
                                    t_send_num++;
                                    t_mcnt[i] = 0;
                                    t_all_true[i] = true;
                                    t_Max_NG = 10;
                                }
                            }
                        }

                        if (t_send_num > 0)
                        {
                            string[] tt_send_Device = new string[t_send_num];
                            double[] tt_send_Data = new double[t_send_num];
                            string log_str = "";
                            t_send_num = 0;
                            for (j = 0; j < cam_num; j++)
                            {
                                if (t_send_Data[j] >= 0)
                                {
                                    tt_send_Device[t_send_num] = t_send_Device[j];
                                    tt_send_Data[t_send_num] = t_send_Data[j];
                                    t_send_num++;
                                    log_str += "C" + j.ToString() + "[" + t_send_Device[j] + "_" + t_send_Data[j].ToString() + "] ";
                                }
                                //else
                                //{
                                //    log_str += "C" + j.ToString() + "[No] ";
                                //}
                            }
                            if (m_Protocal == (int)PROTOCAL.XGTRS232 || m_Protocal == (int)PROTOCAL.IPSBoard)
                            {
                                bool t_write_check = false;
                                while (serialPort1.WriteBufferSize == 0)
                                {
                                    Thread.Sleep(1);
                                    t_write_check = true;
                                    //데이타를 전부 PLC로 전송 하기 위함..
                                }
                                if (t_write_check)
                                {
                                    Thread.Sleep(11);
                                }
                            }
                            else if (m_Protocal == (int)PROTOCAL.ModbbusTCP)
                            {

                            }
                            else if (m_Protocal == (int)PROTOCAL.ModbusRTU)
                            {

                            }
                            PLC_D_BLOCK_WRITE(tt_send_Device, t_send_num, tt_send_Data);
                            if (LVApp.Instance().m_Config.PLC_Judge_view)
                            {
                                LVApp.Instance().m_mainform.add_Log(log_str);
                            }
                        }
                        t_send_num = 0;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LVApp.Instance().m_mainform.add_Log(ex.ToString());
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("PLC 쓰레드 에러로 종료.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("PLC Thread terminated with error!");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("PLC 线程因错误而终止.");
                }

                m_threads_Check = false;
                PLC_Thread_Start();
            }
        }


        public static string Hex2Ascii(string hexString)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hexString.Length; i += 2)
            {
                string hs = hexString.Substring(i, 2);
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }

            return sb.ToString();
        }

        public int t_Tx_Check = 0;
        private void IPSBOARD_Tx_ThreadProc()
        {
            try
            {
                int t_Cam_CNT = -1; int j = 0;
                int t_cnt = 0; int t_Idx_cnt = 0;
                int t_time_cnt = 0; int t_time_Idx_cnt = 0;
                uint Idx_cnt = 0;
                const int cam_num = 4;
                string[] t_send_Device_M = new string[cam_num];
                double[] t_send_Data_M = new double[cam_num];

                //string[] t_send_Device = new string[cam_num];
                //double[] t_send_Data = new double[cam_num];
                string log_str_M = string.Empty;
                bool t_exist_check = false;
                bool[] t_first_check = new bool[cam_num];

                int Cam_last_idx = 0;
                if (t_Cam_CNT == -1)
                {
                    t_Cam_CNT = 0;
                    for (j = 0; j < 4; j++)
                    {
                        t_first_check[j] = false;
                        if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[j])
                        {
                            t_Cam_CNT++;
                            Cam_last_idx = j;
                        }
                        t_send_Device_M[j] = "";
                        t_send_Data_M[j] = 0;
                        //t_send_Device[j] = "";
                        //t_send_Data[j] = 0;
                    }
                } // 초기화 완료

                byte[] byte_send = new byte[27];
                byte_send[0] = 0xC1;
                byte_send[1] = 0xC1;
                double t_th_time = 1000;
                TimeSpan deltaTime;
                //int t_last_idx = 0;
                while (m_threads_Check)
                {
                    if (!m_threads_Check)
                    {
                        break;
                    }

                    t_exist_check = false;
                    int t_send_num = 0;
                    string sendmessage = string.Empty;
                    string sendmessage2 = string.Empty;
                    for (j = 0; j <= Cam_last_idx; j++)
                    {
                        if (LVApp.Instance().m_Config.PLC_Judge_view)
                        {
                            t_cnt = send_Message[j].Count;
                            t_Idx_cnt = send_Idx[j].Count;
                            t_time_cnt = send_Message_Time[j].Count;
                            t_time_Idx_cnt = send_Idx_Time[j].Count;
                            sendmessage2 += j.ToString("") + "[" + t_cnt.ToString() + "," + t_time_cnt.ToString() + "/" + t_Idx_cnt.ToString() + "," + t_time_Idx_cnt.ToString() + "]  ";
                        }
                        t_cnt = send_Message[j].Count;
                        t_Idx_cnt = send_Idx[j].Count;
                        if (t_cnt == 0 && t_Idx_cnt == 0)
                        {
                            continue;
                        }

                        t_cnt = send_Message[j].Count;
                        t_time_cnt = send_Message_Time[j].Count;
                        t_Idx_cnt = send_Idx[j].Count;
                        t_time_Idx_cnt = send_Idx_Time[j].Count;

                        if (t_cnt > t_Idx_cnt)
                        {
                            for (int s = 0; s < t_cnt - t_Idx_cnt; s++)
                            {
                                lock (send_Message[j])
                                {
                                    send_Message[j].RemoveAt(0);
                                }
                                lock (send_Message_Time[j])
                                {
                                    send_Message_Time[j].RemoveAt(0);
                                }
                            }
                        }

                        t_time_cnt = send_Message_Time[j].Count;
                        t_time_Idx_cnt = send_Idx_Time[j].Count;
                        if (t_cnt > 0 && t_time_cnt > 0)
                        {
                            for (int kk = 0; kk < t_cnt; kk++)
                            {
                                // 시간으로 거르기
                                // 1. send message 처리시간과 현재 시간 비교
                                deltaTime = send_Message_Time[j][0] - DateTime.Now;
                                //if (LVApp.Instance().t_Util.m_FPS[j + 4] >= 1.5)
                                //{
                                t_th_time = 2.5f * ((double)1000 / (double)LVApp.Instance().t_Util.m_FPS[j + 4]);
                                //}
                                //t_th_time = 200;
                                if (deltaTime.TotalMilliseconds > t_th_time)
                                {// 현재 결과가 오래 된거면 제거
                                    lock (send_Message[j])
                                    {
                                        send_Message[j].RemoveAt(0);
                                    }
                                    lock (send_Message_Time[j])
                                    {
                                        send_Message_Time[j].RemoveAt(0);
                                    }
                                }
                            }
                        }

                        //if (t_Idx_cnt > 0 && t_time_Idx_cnt > 0)
                        //{
                        //    deltaTime = send_Idx_Time[j][0] - DateTime.Now;
                        //    //if (LVApp.Instance().t_Util.m_FPS[j + 4] >= 1.5)
                        //    {
                        //        t_th_time = 1.1f * ((double)1000 / (double)LVApp.Instance().t_Util.m_FPS[j + 4]);
                        //    }

                        //    if (deltaTime.TotalMilliseconds > t_th_time)
                        //    {// 받은 index가 오래 된거면 제거
                        //        lock (send_Idx[j])
                        //        {
                        //            send_Idx[j].RemoveAt(0);
                        //        }
                        //        lock (send_Idx_Time[j])
                        //        {
                        //            if (t_time_cnt > 0)
                        //            {
                        //                send_Idx_Time[j].RemoveAt(0);
                        //            }
                        //        }
                        //    }
                        //}



                        if (t_time_cnt > 0 && t_time_Idx_cnt > 0)
                        {
                            t_th_time = 2.5f * ((double)1000 / (double)LVApp.Instance().t_Util.m_FPS[j + 4]);
                            for (int kk = 0; kk < t_time_cnt; kk++)
                            {
                                deltaTime = send_Message_Time[j][0] - send_Idx_Time[j][0];
                                if (deltaTime.TotalMilliseconds > t_th_time)
                                {// 받은 제품 번호가 너무 오래전거면 제거
                                    lock (send_Idx[j])
                                    {
                                        if (send_Idx[j].Count > 0)
                                        {
                                            send_Idx[j].RemoveAt(0);
                                        }
                                    }
                                    lock (send_Idx_Time[j])
                                    {
                                        if (send_Idx_Time[j].Count > 0)
                                        {
                                            send_Idx_Time[j].RemoveAt(0);
                                        }
                                    }
                                }
                            }
                            t_Idx_cnt = send_Idx[j].Count;
                            t_time_Idx_cnt = send_Idx_Time[j].Count;
                        }

                        if (t_cnt > 0 && t_Idx_cnt > 0)
                        {
                            //t_exist_check = false;
                            string[] cp_msg = null;
                            //if (send_Message[j].Count > 0)
                            {
                                cp_msg = send_Message[j][0].Split('_');
                                lock (send_Message[j])
                                {
                                    send_Message[j].RemoveAt(0);
                                }
                            }
                            //if (send_Message_Time[j].Count > 0)
                            {
                                lock (send_Message_Time[j])
                                {
                                    send_Message_Time[j].RemoveAt(0);
                                }
                            }

                            //if (send_Idx[j].Count > 0)
                            {
                                Idx_cnt = send_Idx[j][0];
                                lock (send_Idx[j])
                                {
                                    send_Idx[j].RemoveAt(0);
                                }
                            }
                            //if (send_Idx_Time[j].Count > 0)
                            {
                                lock (send_Idx_Time[j])
                                {
                                    send_Idx_Time[j].RemoveAt(0);
                                }
                            }

                            if (cp_msg == null)
                            {// 일반 메세지
                                continue;
                                //t_send_Device[j] = cp_msg[0];
                                //t_send_Data[j] = Convert.ToDouble(cp_msg[1]);
                            }
                            //if (cp_msg.Length == 3)
                            {//결과 메세지
                                //if (t_send_Device_M[j].Length == 0 && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[j])
                                {
                                    //t_send_Device_M[j] = cp_msg[0];
                                    t_send_Data_M[j] = Convert.ToDouble(cp_msg[1]);

                                    if (j == 0)
                                    {
                                        byte_send[3 + t_send_num * 6] = 0x00;
                                    }
                                    else if (j == 1)
                                    {
                                        byte_send[3 + t_send_num * 6] = 0x01;
                                    }
                                    else if (j == 2)
                                    {
                                        byte_send[3 + t_send_num * 6] = 0x02;
                                    }
                                    else if (j == 3)
                                    {
                                        byte_send[3 + t_send_num * 6] = 0x03;
                                    }
                                    byte[] result = BitConverter.GetBytes(Idx_cnt);
                                    byte_send[4 + t_send_num * 6] = result[0];
                                    byte_send[5 + t_send_num * 6] = result[1];
                                    byte_send[6 + t_send_num * 6] = result[2];
                                    byte_send[7 + t_send_num * 6] = result[3];
                                    byte_send[8 + t_send_num * 6] = 0x00;
                                    if (t_send_Data_M[j] == 10 || t_send_Data_M[j] == 1)
                                    {
                                        byte_send[8 + t_send_num * 6] = 0x01;
                                    }
                                    else if (t_send_Data_M[j] == 20 || t_send_Data_M[j] == 2)
                                    {
                                        byte_send[8 + t_send_num * 6] = 0x02;
                                    }
                                    else if (t_send_Data_M[j] == 30 || t_send_Data_M[j] == 3)
                                    {
                                        byte_send[8 + t_send_num * 6] = 0x03;
                                    }
                                    else if (t_send_Data_M[j] == 40 || t_send_Data_M[j] == 4)
                                    {
                                        byte_send[8 + t_send_num * 6] = 0x04;
                                    }
                                    else
                                    {
                                        byte_send[8 + t_send_num * 6] = 0x01;
                                    }
                                    t_send_num++;
                                    //LVApp.Instance().m_mainform.ipS_BOARD1.C1C1_P2B[j] = t_P;
                                    //IPSBOARD_MESSAGE_TX(0xC1C1, j);

                                    if (LVApp.Instance().m_Config.PLC_Judge_view)
                                    {
                                        //if (j == 0)
                                        {

                                            //sendmessage += LVApp.Instance().m_mainform.ipS_BOARD1.C1C1_P2B[j].Message_Start.ToString("x2");;
                                            sendmessage += "CAM" + j.ToString("");
                                            sendmessage += " " + Idx_cnt.ToString("");
                                            sendmessage += " " + t_send_Data_M[j].ToString("") + "  ";
                                            //DebugLogger.Instance().LogRecord("Tx : " + sendmessage);
                                        }
                                    }
                                }
                                t_send_Device_M[j] = "";
                            }
                        }
                    }

                    if (t_send_num > 0 && m_threads_Check)
                    {
                        byte_send[2] = (byte)t_send_num;
                        if (TCP_Client_Stream.CanWrite)
                        {
                            TCP_Client_Stream.Write(byte_send, 0, 3 + t_send_num * 6);
                            if (LVApp.Instance().m_Config.PLC_Judge_view)
                            {
                                add_Log("Tx " + sendmessage);
                                add_Log("Judge, Time / Product, Time : " + sendmessage2);
                            }
                        }
                        else
                        {
                            Thread.Sleep(1);
                            if (TCP_Client_Stream.CanWrite)
                            {
                                TCP_Client_Stream.Write(byte_send, 0, 3 + t_send_num * 6);
                                if (LVApp.Instance().m_Config.PLC_Judge_view)
                                {
                                    add_Log("Tx " + sendmessage);
                                    add_Log("Judge, Time / Product, Time : " + sendmessage2);
                                }
                            }
                            else
                            {
                                add_Log("Tx Error " + sendmessage);
                            }
                            //do
                            //{
                            //    TCP_Client_Stream.Write(byte_send, 0, 3 + t_send_num * 6);
                            //}
                            //while (TCP_Client_Stream.CanWrite);
                        }
                        t_exist_check = true;
                    }

                    if (t_exist_check)
                    {
                        Thread.Sleep(t_Tx_Interval);
                        t_Tx_Check = 0;
                    }
                    else
                    {
                        Thread.Sleep(1);
                        t_Tx_Check++;
                        if (LVApp.Instance().m_Config.PLC_Judge_view)
                        {
                            if (t_Tx_Check == 1000)
                            {
                                send_Idx[0].Clear();
                                send_Idx_Time[0].Clear();
                                send_Idx[1].Clear();
                                send_Idx_Time[1].Clear();
                                send_Idx[2].Clear();
                                send_Idx_Time[2].Clear();
                                send_Idx[3].Clear();
                                send_Idx_Time[3].Clear();

                                add_Log("Judge, Time / Product, Time : " + sendmessage2);
                                //t_Tx_Check = 0;
                            }
                        }
                        if (t_Tx_Check == 10000)
                        {
                            t_Tx_Check = 0;
                        }
                    }
                }
            }
            catch
            {
                if (btnClose_Click_check)
                {
                    return;
                }

                if (TCP_Client != null)
                {
                    if (TCP_Client.Connected)
                    {
                        PLC_Thread_Stop();
                        PLC_Thread_Start();
                        add_Log("Tx Thread Restart");
                    }
                }
            }
            return;
        }


        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.m_Judge_Priority = comboBox_NOOBJECT.SelectedIndex;
        }

        private void checkBox_JView_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.PLC_Judge_view = checkBox_JView.Checked;
        }

        private void checkBox_SIMULATION_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.Simulation_mode = checkBox_SIMULATION.Checked;
        }

        public void button_Send_Save_Click(object sender, EventArgs e)
        {
            if (m_Protocal == (int)PROTOCAL.IPSBoard)
            {

            }
            else
            {
                int.TryParse(textBox_Delay0.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[0]);
                int.TryParse(textBox_Delay1.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[1]);
                int.TryParse(textBox_Delay2.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[2]);
                int.TryParse(textBox_Delay3.Text, out LVApp.Instance().m_Config.m_Cam_Trigger_Delay[3]);

                send_Message[0].Add("DW5090" + "_" + LVApp.Instance().m_Config.m_Cam_Trigger_Delay[0].ToString());
                send_Message[1].Add("DW5091" + "_" + LVApp.Instance().m_Config.m_Cam_Trigger_Delay[1].ToString());
                send_Message[2].Add("DW5092" + "_" + LVApp.Instance().m_Config.m_Cam_Trigger_Delay[2].ToString());
                send_Message[3].Add("DW5093" + "_" + LVApp.Instance().m_Config.m_Cam_Trigger_Delay[3].ToString());
            }
        }

        private void checkBox_MERGETX_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Tx_Merge = checkBox_MERGETX.Checked;
        }

        private void textBox_ROOMNUM_KeyUp(object sender, KeyEventArgs e)
        {
            int t_v = 0;
            int.TryParse(textBox_ROOMNUM.Text, out t_v);
            if (t_v <= 0)
            {
                textBox_ROOMNUM.Text = "1";
            }
            if (t_v >= 10)
            {
                textBox_ROOMNUM.Text = "9";
            }
        }

        private void checkBox_Tab_Enable_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Disable_Menu = checkBox_Tab_Enable.Checked;
        }

        private bool IPSBOARD_COMMUNICATION_check = false;
        public string IPSBOARD_COMMUNICATION(string sendmessage)
        {
            while (IPSBOARD_COMMUNICATION_check)
            {
                Thread.Sleep(1);
            };

            IPSBOARD_COMMUNICATION_check = true;
            string str_out = "";
            try
            {
                if (serialPort1.IsOpen)
                {
                    if (m_Protocal == (int)PROTOCAL.IPSBoard)
                    {
                        serialPort1.Encoding = Encoding.GetEncoding("Windows-1252");
                        int t_time = 0;

                        serialPort1.Write(sendmessage);
                        do
                        {
                            Thread.Sleep(1);
                            //데이타를 전부 PLC로 전송 하기 위함..
                        } while (serialPort1.WriteBufferSize == 0);
                        bool m_Next = false;
                        string strInData = string.Empty;
                        do
                        {
                            str = strInData;
                            string msg = serialPort1.ReadExisting();
                            strInData += msg;

                            if (msg.Length == 0 && str.Length >= 1)
                            {
                                m_Next = true;
                            }
                            Thread.Sleep(1);
                            t_time++;

                            if (t_time > 30)
                            {
                                m_Next = true;
                            }
                        } while (!m_Next);

                        str_out = str;
                    }
                }
                else
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        LVApp.Instance().m_mainform.add_Log("포트가 연결되지 않았습니다.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        LVApp.Instance().m_mainform.add_Log("Use after connection of PLC.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        LVApp.Instance().m_mainform.add_Log("PLC 连接后使用.");
                    }
                }
            }
            catch
            {
                IPSBOARD_COMMUNICATION_check = false;
            }
            IPSBOARD_COMMUNICATION_check = false;
            return str_out;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        uint t_log_cnt = 0;
        public void add_Log(string str)
        {
            try
            {
                t_log_cnt++;
                if (richTextBox_LOG.InvokeRequired)
                {
                    richTextBox_LOG.Invoke((MethodInvoker)delegate
                    {
                        if (richTextBox_LOG.Lines.Length > 100)
                        {
                            richTextBox_LOG.Lines = richTextBox_LOG.Lines.Take(richTextBox_LOG.Lines.Length - 3).ToArray();
                        }
                        string display_str = "[" + t_log_cnt.ToString("0000000000") + "]" + " (" + DateTime.Now.ToString("HH:mm:ss.fff") + ") " + str + "\n" + richTextBox_LOG.Text;
                        richTextBox_LOG.Text = display_str;
                    });
                }
                else
                {
                    if (richTextBox_LOG.Lines.Length > 100)
                    {
                        richTextBox_LOG.Lines = richTextBox_LOG.Lines.Take(richTextBox_LOG.Lines.Length - 3).ToArray();
                    }
                    string display_str = "[" + t_log_cnt.ToString("0000000000") + "]" + " (" + DateTime.Now.ToString("HH:mm:ss.fff") + ") " + str + "\n" + richTextBox_LOG.Text;
                    richTextBox_LOG.Text = display_str;
                }
            }
            catch
            {
            }
        }

        private void button_LOG_CLEAR_Click(object sender, EventArgs e)
        {
            richTextBox_LOG.ResetText();
        }

        private void textBox_DELAYCAMMISS_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(textBox_DELAYCAMMISS.Text, out m_DELAYCAMMISS);
            }
        }

        private void textBox_MinTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(textBox_MinTime.Text, out m_MinProcessingTime);
            }
        }

        public bool Use_CAM1_CAM2_ROI2_MC_Tx = false;
        public double CAM2_ROI2_Value;
        public double CAM2_ROI2_Max;
        public double CAM2_ROI2_Min;
        public double CAM1_ROI2_Value;
        public double CAM1_ROI2_Max;
        public double CAM1_ROI2_Min;
        public bool CAM2_Updated = false;
        public bool CAM1_Updated = false;

        private bool Send_Data_MC_check = false;
        public void Send_Data_MC()
        {
            if (LVApp.Instance().m_mainform.Simulation_mode)
            {
                CAM2_Updated = CAM1_Updated = false;
                return;
            }
            try
            {
                if (Send_Data_MC_check || !Use_CAM1_CAM2_ROI2_MC_Tx)
                {
                    return;
                }
                if (CAM2_Updated && CAM1_Updated)
                {
                    Send_Data_MC_check = true;
                    CAM2_Updated = CAM1_Updated = false;

                    if (!McProtocolApp.Connected)
                    {
                        McProtocolApp.HostName = textBox_SERVER_IP.Text;
                        McProtocolApp.PortNumber = int.Parse(textBox_SERVER_PORT.Text);
                        McProtocolApp.CommandFrame = Mitsubishi.McFrame.MC3E;
                        McProtocolApp.Open();
                    }
                    if (McProtocolApp.Connected)
                    {
                        //using (McProtocolApp.Open())
                        int m_size = 13;
                        int[] nData = new int[m_size];
                        int t_idx = 0;
                        string nAddress = "D4500";

                        int[] t_nData = Convert_Data(CAM1_ROI2_Value);
                        nData[t_idx * 2] = t_nData[0]; nData[t_idx * 2 + 1] = t_nData[1]; t_idx++;

                        t_nData = Convert_Data(CAM1_ROI2_Max);
                        nData[t_idx * 2] = t_nData[0]; nData[t_idx * 2 + 1] = t_nData[1]; t_idx++;

                        t_nData = Convert_Data(CAM1_ROI2_Min);
                        nData[t_idx * 2] = t_nData[0]; nData[t_idx * 2 + 1] = t_nData[1]; t_idx++;

                        t_nData = Convert_Data(CAM2_ROI2_Value);
                        nData[t_idx * 2] = t_nData[0]; nData[t_idx * 2 + 1] = t_nData[1]; t_idx++;

                        t_nData = Convert_Data(CAM2_ROI2_Max);
                        nData[t_idx * 2] = t_nData[0]; nData[t_idx * 2 + 1] = t_nData[1]; t_idx++;

                        t_nData = Convert_Data(CAM2_ROI2_Min);
                        nData[t_idx * 2] = t_nData[0]; nData[t_idx * 2 + 1] = t_nData[1]; t_idx++;

                        nData[t_idx * 2] = 1;

                        McProtocolApp.WriteDeviceBlock(nAddress, m_size, nData);
                        add_Log("Data Tx complited. D4500 [CAM1:" + ((int)CAM1_ROI2_Value).ToString() + "," + ((int)CAM1_ROI2_Max).ToString() + "," + ((int)CAM1_ROI2_Min).ToString() + ", CAM2: " + ((int)CAM2_ROI2_Value).ToString() + ", " + ((int)CAM2_ROI2_Max).ToString() + ", " + ((int)CAM2_ROI2_Min).ToString() + "]");
                    }
                    //Task<int> McTask = McProtocolApp.Open();

                    Send_Data_MC_check = false;
                }
            }
            catch
            {

            }
        }

        private int[] Convert_Data(double t_v)
        {
            int m_data = (int)(t_v);
            int t_sign = 1;
            if (m_data < 0) t_sign = -1;
            string t_str = Convert.ToString((int)m_data * t_sign, 2);
            string a_str = "";
            int m_add_length = 32 - t_str.Length;
            if (m_add_length > 0)
            {
                for (int j = 0; j < m_add_length; j++)
                {
                    a_str += "0";
                }
                a_str += t_str;
            }
            else
            {
                a_str = t_str;
            }
            int[] t_nData = new int[2];
            for (int j = 0; j < 2; j++)
            {
                string s = a_str.Substring(j * 16, 16);
                if (t_sign == -1)
                {
                    t_nData[2 - j - 1] = (65536 - Convert.ToInt16(s, 2));
                }
                else
                {
                    t_nData[2 - j - 1] = (Convert.ToInt16(s, 2));
                }
            }
            return t_nData;
        }

        private void checkBox_MC_CheckedChanged(object sender, EventArgs e)
        {
            Use_CAM1_CAM2_ROI2_MC_Tx = checkBox_MC.Checked;
        }

        // 2024.08.24 by CD
        // START
        private bool Use_MC_Rx = false;
        private bool Use_MC_Tx = false;
        private int MC_Rx_Row_CNT = 1;
        private int MC_Tx_Row_CNT = 1;
        public DataTable DT_MC_Rx = new DataTable("MC_Rx");
        public DataTable DT_MC_Tx = new DataTable("MC_Tx");

        public bool[] CAM_Value_Updated = new bool[4] { false, false, false, false };
        public double[] CAM0_Value = new double[40];
        public double[] CAM1_Value = new double[40];
        public double[] CAM2_Value = new double[40];
        public double[] CAM3_Value = new double[40];

        public int[] MC_Rx_CAM_Num = new int[4];
        public bool[] MC_Rx_Request = new bool[4] { false, false, false, false };
        public bool[] MC_Rx_Value_Updated = new bool[4] { false, false, false, false };
        public int[] MC_Rx_Value = new int[4] { -1, -1, -1, -1 };

        private void checkBox_MC_Rx_Use_CheckedChanged(object sender, EventArgs e)
        {
            Use_MC_Rx = checkBox_MC_Rx_Use.Checked;
        }

        private void checkBox_MC_Tx_Use_CheckedChanged(object sender, EventArgs e)
        {
            Use_MC_Tx = checkBox_MC_Tx_Use.Checked;
        }

        private void numericUpDown_MC_Rx_ValueChanged(object sender, EventArgs e)
        {
            MC_Rx_Row_CNT = (int)numericUpDown_MC_Rx.Value;
        }

        private void numericUpDown_MC_Tx_ValueChanged(object sender, EventArgs e)
        {
            MC_Tx_Row_CNT = (int)numericUpDown_MC_Tx.Value;
        }

        private void button_MC_Rx_Apply_Click(object sender, EventArgs e)
        {
            int Row_CNT = DT_MC_Rx.Rows.Count;
            if (DT_MC_Rx.Columns.Count == 0)
            {
                DT_MC_Rx.Columns.Add("Address");
                DT_MC_Rx.Columns.Add("카메라");
                DT_MC_Rx.Columns.Add("값");
                DT_MC_Rx.Columns.Add("비고");
            }

            if (Row_CNT < MC_Rx_Row_CNT)
            {
                for (int i = 0; i < MC_Rx_Row_CNT - Row_CNT; i++)
                {
                    DT_MC_Rx.Rows.Add("D", "CAM0", "", "셀ID(QR코드)");
                }
            }

            if (Row_CNT > MC_Rx_Row_CNT)
            {
                for (int i = Row_CNT - 1; i >= MC_Rx_Row_CNT; i--)
                {
                    DT_MC_Rx.Rows.RemoveAt(i);
                }
            }

            dataGridView_MC_Rx.DataSource = DT_MC_Rx;
            dataGridView_MC_Rx.AllowUserToAddRows = false;
            dataGridView_MC_Rx.AllowUserToDeleteRows = false;
            dataGridView_MC_Rx.AllowUserToResizeColumns = false;
            dataGridView_MC_Rx.AllowUserToResizeRows = false;
            dataGridView_MC_Rx.RowHeadersWidth = 5;
            dataGridView_MC_Rx.ColumnHeadersHeight = 26;
            dataGridView_MC_Rx.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView_MC_Rx.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView_MC_Rx.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            dataGridView_MC_Rx.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_MC_Rx.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridView_MC_Rx.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView_MC_Rx.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridView_MC_Rx.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn column in dataGridView_MC_Rx.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView_MC_Rx.ClearSelection();

            foreach (DataGridViewRow row in dataGridView_MC_Rx.Rows)
            {
                string t_str = row.Cells[1].Value.ToString();
                row.Cells[1].Value = null;
                DataGridViewComboBoxCell dgvCmbCell = new DataGridViewComboBoxCell(); dgvCmbCell.FlatStyle = FlatStyle.Flat;
                dgvCmbCell.Items.Add("ALL");
                dgvCmbCell.Items.Add("CAM0");
                dgvCmbCell.Items.Add("CAM1");
                dgvCmbCell.Items.Add("CAM2");
                dgvCmbCell.Items.Add("CAM3");

                row.Cells[1] = dgvCmbCell;
                bool t_exist = false;
                foreach (var t_item in dgvCmbCell.Items)
                {
                    if (t_str == t_item.ToString())
                    {
                        t_exist = true;
                    }
                }
                if (t_exist)
                {
                    row.Cells[1].Value = t_str;
                }
                else
                {
                    row.Cells[1].Value = "ALL";
                }
            }
        }

        private void button_MC_Tx_Apply_Click(object sender, EventArgs e)
        {
            int Row_CNT = DT_MC_Tx.Rows.Count;
            if (DT_MC_Tx.Columns.Count == 0)
            {
                DT_MC_Tx.Columns.Add("Address");
                DT_MC_Tx.Columns.Add("Type");
                DT_MC_Tx.Columns.Add("카메라");
                DT_MC_Tx.Columns.Add("ROI");
                DT_MC_Tx.Columns.Add("SCALE");
                DT_MC_Tx.Columns.Add("값");
                DT_MC_Tx.Columns.Add("비고");
            }

            if (Row_CNT < MC_Tx_Row_CNT)
            {
                for (int i = 0; i < MC_Tx_Row_CNT - Row_CNT; i++)
                {
                    DT_MC_Tx.Rows.Add("D", "DWORD", "CAM0", "01", "1", "", "");
                }
            }

            if (Row_CNT > MC_Tx_Row_CNT)
            {
                for (int i = Row_CNT - 1; i >= MC_Tx_Row_CNT; i--)
                {
                    DT_MC_Tx.Rows.RemoveAt(i);
                }
            }

            dataGridView_MC_Tx.DataSource = DT_MC_Tx;
            dataGridView_MC_Tx.AllowUserToAddRows = false;
            dataGridView_MC_Tx.AllowUserToDeleteRows = false;
            dataGridView_MC_Tx.AllowUserToResizeColumns = false;
            dataGridView_MC_Tx.AllowUserToResizeRows = false;
            dataGridView_MC_Tx.RowHeadersWidth = 5;
            dataGridView_MC_Tx.ColumnHeadersHeight = 26;
            dataGridView_MC_Tx.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView_MC_Tx.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView_MC_Tx.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            dataGridView_MC_Tx.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_MC_Tx.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridView_MC_Tx.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView_MC_Tx.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridView_MC_Tx.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn column in dataGridView_MC_Tx.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView_MC_Tx.ClearSelection();

            foreach (DataGridViewRow row in dataGridView_MC_Tx.Rows)
            {
                string t_str = row.Cells[1].Value.ToString();
                row.Cells[1].Value = null;
                DataGridViewComboBoxCell dgvCmbCell0 = new DataGridViewComboBoxCell(); dgvCmbCell0.FlatStyle = FlatStyle.Flat;
                dgvCmbCell0.Items.Add("WORD");
                dgvCmbCell0.Items.Add("DWORD");

                row.Cells[1] = dgvCmbCell0;
                bool t_exist = false;
                foreach (var t_item in dgvCmbCell0.Items)
                {
                    if (t_str == t_item.ToString())
                    {
                        t_exist = true;
                    }
                }
                if (t_exist)
                {
                    row.Cells[1].Value = t_str;
                }
                else
                {
                    row.Cells[1].Value = "DWORD";
                }

                t_str = row.Cells[2].Value.ToString();
                row.Cells[2].Value = null;
                DataGridViewComboBoxCell dgvCmbCell1 = new DataGridViewComboBoxCell(); dgvCmbCell1.FlatStyle = FlatStyle.Flat;
                dgvCmbCell1.Items.Add("CAM0");
                dgvCmbCell1.Items.Add("CAM1");
                dgvCmbCell1.Items.Add("CAM2");
                dgvCmbCell1.Items.Add("CAM3");
                dgvCmbCell1.Items.Add("사용안함");

                row.Cells[2] = dgvCmbCell1;
                t_exist = false;
                foreach (var t_item in dgvCmbCell1.Items)
                {
                    if (t_str == t_item.ToString())
                    {
                        t_exist = true;
                    }
                }
                if (t_exist)
                {
                    row.Cells[2].Value = t_str;
                }
                else
                {
                    row.Cells[2].Value = "사용안함";
                }

                t_str = row.Cells[3].Value.ToString();
                row.Cells[3].Value = null;
                DataGridViewComboBoxCell dgvCmbCell2 = new DataGridViewComboBoxCell(); dgvCmbCell2.FlatStyle = FlatStyle.Flat;
                dgvCmbCell2.Items.Add("고정값");
                for (int i = 1; i <= 40; i++)
                {
                    dgvCmbCell2.Items.Add(i.ToString("00"));
                }

                row.Cells[3] = dgvCmbCell2;
                t_exist = false;
                foreach (var t_item in dgvCmbCell2.Items)
                {
                    if (t_str == t_item.ToString())
                    {
                        t_exist = true;
                    }
                }
                if (t_exist)
                {
                    row.Cells[3].Value = t_str;
                }
                else
                {
                    row.Cells[3].Value = "고정값";
                }
            }
        }

        #region 모든 카메라에서 검사가 끝났을 때 한번에 데이터 전송. 240827- 각 카메라마다 각각 전송하도록 변경. ThreadProcMCTx 참고
        bool Data_MC_Tx_check = false;
        public void Data_MC_Tx()
        {
            try
            {
                if (!Use_MC_Tx)
                {
                    for (int cam = 0; cam < 4; cam++)
                    { // Update flag false로 바꿈
                        CAM_Value_Updated[cam] = false;
                    }
                    Data_MC_Tx_check = false;
                    return;
                }

                if (Data_MC_Tx_check)
                {
                    return;
                }

                bool Data_Full_Check = true;

                for (int cam = 0; cam < 4; cam++)
                {
                    if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[cam])
                    { // 카메라 사용안하면 무조건 update 되었다고 생각
                        CAM_Value_Updated[cam] = true;
                    }
                    else
                    {// 카메라 사용
                        if (!CAM_Value_Updated[cam])
                        { // 사용하는 카메라의 값이 update 안되어 있다면...
                            Data_Full_Check = false;
                        }
                    }
                }

                if (Data_Full_Check)
                { // 모든 카메라 값이 update 되었을때
                    if (!McProtocolApp.Connected)
                    { //MCProtocol 접속 안되어 있으면 접속하기
                        McProtocolApp.HostName = textBox_SERVER_IP.Text;
                        McProtocolApp.PortNumber = int.Parse(textBox_SERVER_PORT.Text);
                        McProtocolApp.CommandFrame = Mitsubishi.McFrame.MC3E;
                        McProtocolApp.Open();
                    }

                    if (McProtocolApp.Connected)
                    { // 접속 되면
                        Data_MC_Tx_check = true;
                        for (int cam = 0; cam < 4; cam++)
                        { // Update flag false로 바꿈
                            CAM_Value_Updated[cam] = false;
                        }

                        // 보내야 할 data 수 계산
                        int _row_cnt = DT_MC_Tx.Rows.Count;
                        List<Tuple<string, int, int>> Tx_list = new List<Tuple<string, int, int>>();//주소, Type, Value
                        for (int idx = 0; idx < _row_cnt; idx++)
                        {
                            var _row = DT_MC_Tx.Rows[idx];
                            if (_row[2].ToString() == "사용안함")
                            {

                            }
                            else if (_row[2].ToString() == "CAM0")
                            {
                                int t_ROI = -1;
                                int.TryParse(_row[3].ToString(), out t_ROI);
                                double t_Scale = 1;
                                double.TryParse(_row[4].ToString(), out t_Scale);

                                if (t_ROI <= 0)
                                {// 고정값

                                }
                                else
                                {// ROI값
                                    _row[5] = (int)(CAM0_Value[t_ROI - 1] * t_Scale);
                                }

                                int t_Type = 2;
                                if (_row[1].ToString() == "WORD")
                                {
                                    t_Type = 1;
                                }
                                // 주소와 값을 list에 넣기
                                int t_Value = 0;
                                int.TryParse(_row[5].ToString(), out t_Value);
                                Tx_list.Add(new Tuple<string, int, int>(_row[0].ToString(), t_Type, t_Value));
                            }
                            else if (_row[2].ToString() == "CAM1")
                            {
                                int t_ROI = -1;
                                int.TryParse(_row[3].ToString(), out t_ROI);
                                double t_Scale = 1;
                                double.TryParse(_row[4].ToString(), out t_Scale);

                                if (t_ROI <= 0)
                                {// 고정값

                                }
                                else
                                {// ROI값
                                    _row[5] = (int)(CAM1_Value[t_ROI - 1] * t_Scale);
                                }

                                int t_Type = 2;
                                if (_row[1].ToString() == "WORD")
                                {
                                    t_Type = 1;
                                }
                                // 주소와 값을 list에 넣기
                                int t_Value = 0;
                                int.TryParse(_row[5].ToString(), out t_Value);
                                Tx_list.Add(new Tuple<string, int, int>(_row[0].ToString(), t_Type, t_Value));
                            }
                            else if (_row[2].ToString() == "CAM2")
                            {
                                int t_ROI = -1;
                                int.TryParse(_row[3].ToString(), out t_ROI);
                                double t_Scale = 1;
                                double.TryParse(_row[4].ToString(), out t_Scale);

                                if (t_ROI <= 0)
                                {// 고정값

                                }
                                else
                                {// ROI값
                                    _row[5] = (int)(CAM2_Value[t_ROI - 1] * t_Scale);
                                }

                                int t_Type = 2;
                                if (_row[1].ToString() == "WORD")
                                {
                                    t_Type = 1;
                                }
                                // 주소와 값을 list에 넣기
                                int t_Value = 0;
                                int.TryParse(_row[5].ToString(), out t_Value);
                                Tx_list.Add(new Tuple<string, int, int>(_row[0].ToString(), t_Type, t_Value));
                            }
                            else if (_row[2].ToString() == "CAM3")
                            {
                                int t_ROI = -1;
                                int.TryParse(_row[3].ToString(), out t_ROI);
                                double t_Scale = 1;
                                double.TryParse(_row[4].ToString(), out t_Scale);

                                if (t_ROI <= 0)
                                {// 고정값

                                }
                                else
                                {// ROI값
                                    _row[5] = (int)(CAM3_Value[t_ROI - 1] * t_Scale);
                                }

                                int t_Type = 2;
                                if (_row[1].ToString() == "WORD")
                                {
                                    t_Type = 1;
                                }
                                // 주소와 값을 list에 넣기
                                int t_Value = 0;
                                int.TryParse(_row[5].ToString(), out t_Value);
                                Tx_list.Add(new Tuple<string, int, int>(_row[0].ToString(), t_Type, t_Value));
                            }
                        }

                        string _log = "MC Tx:";
                        foreach (var _data in Tx_list)
                        {
                            if (_data.Item2 == 1)
                            {
                                McProtocolApp.SetDevice(_data.Item1, _data.Item3);
                            }
                            else
                            {
                                int[] nData = new int[2];
                                int[] t_nData = Convert_Data(_data.Item3);
                                nData[0] = t_nData[0]; nData[1] = t_nData[1];
                                McProtocolApp.WriteDeviceBlock(_data.Item1, 2, nData);
                            }
                            _log += " [" + _data.Item1 + ":" + _data.Item3.ToString() + "]";
                        }
                        if (LVApp.Instance().m_Config.PLC_Judge_view)
                        {
                            add_Log(_log);
                        }
                    }
                    Data_MC_Tx_check = false;
                }
            }
            catch
            {

            }
        }
        #endregion

        //bool Data_MC_Rx_check = false;
        //public void Data_MC_Rx()
        //{
        //    try
        //    {
        //        if (Data_MC_Rx_check)
        //        {
        //            return;
        //        }

        //        Data_MC_Rx_check = true;

        //        if (!Use_MC_Rx)
        //        {
        //            for (int cam = 0; cam < 4; cam++)
        //            { // Update flag false로 바꿈
        //                MC_Rx_Value_Updated[cam] = false;
        //            }
        //            Data_MC_Rx_check = false;
        //            return;
        //        }

        //        for (int cam = 0; cam < 4; cam++)
        //        {
        //            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[cam])
        //            { // 카메라 사용안하면 무조건 update 되었다고 생각
        //                MC_Rx_Value_Updated[cam] = true;
        //            }
        //        }

        //        if (!McProtocolApp.Connected)
        //        { //MCProtocol 접속 안되어 있으면 접속하기
        //            McProtocolApp.HostName = textBox_SERVER_IP.Text;
        //            McProtocolApp.PortNumber = int.Parse(textBox_SERVER_PORT.Text);
        //            McProtocolApp.CommandFrame = Mitsubishi.McFrame.MC3E;
        //            McProtocolApp.Open();
        //        }

        //        if (McProtocolApp.Connected)
        //        { // 접속 되면
        //            if (!McProtocolApp.Connected)
        //            { //MCProtocol 접속 안되어 있으면 접속하기
        //                McProtocolApp.HostName = textBox_SERVER_IP.Text;
        //                McProtocolApp.PortNumber = int.Parse(textBox_SERVER_PORT.Text);
        //                McProtocolApp.CommandFrame = Mitsubishi.McFrame.MC3E;
        //                McProtocolApp.Open();
        //            }

        //            if (McProtocolApp.Connected)
        //            { // 접속 되면
        //                // 받아야 할 data 수 계산
        //                string _log = "MC Rx:";

        //                int _row_cnt = DT_MC_Tx.Rows.Count;
        //                for (int idx = 0; idx < _row_cnt; idx++)
        //                {
        //                    var _row = DT_MC_Rx.Rows[idx];
        //                    if (_row[1].ToString() == "사용안함")
        //                    {

        //                    }
        //                    else
        //                    {
        //                        Task<int> _v = McProtocolApp.GetDevice(DT_MC_Rx.Rows[idx][0].ToString());
        //                        int t_v = _v.Result;
        //                        _row[2] = t_v;
        //                        _log += " [" + _row[0].ToString() + ":" + t_v.ToString() + "]";

        //                        if (_row[1].ToString() == "CAM0")
        //                        {
        //                            MC_Rx_Value[0] = t_v;
        //                            MC_Rx_Value_Updated[0] = true;
        //                        }
        //                        else if (_row[1].ToString() == "CAM1")
        //                        {
        //                            MC_Rx_Value[1] = t_v;
        //                            MC_Rx_Value_Updated[1] = true;
        //                        }
        //                        else if (_row[1].ToString() == "CAM2")
        //                        {
        //                            MC_Rx_Value[2] = t_v;
        //                            MC_Rx_Value_Updated[2] = true;
        //                        }
        //                        else if (_row[1].ToString() == "CAM3")
        //                        {
        //                            MC_Rx_Value[3] = t_v;
        //                            MC_Rx_Value_Updated[3] = true;
        //                        }
        //                    }
        //                }

        //                add_Log(_log);
        //            }
        //            Data_MC_Rx_check = false;
        //        }
        //    }
        //    catch
        //    {
        //        Data_MC_Rx_check = false;
        //    }
        //}

        object _isMCRead_Lock = new object();
        void ThreadProcMCRx()
        {
            try
            {
                int _cam = 0;
                while (MC_Rx_threads_Check)
                {
                    if (!MC_Rx_threads_Check)
                    {
                        break;
                    }
                    if (!Use_MC_Rx)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (!McProtocolApp.Connected)
                    { //MCProtocol 접속 안되어 있으면 접속하기
                        McProtocolApp.HostName = textBox_SERVER_IP.Text;
                        McProtocolApp.PortNumber = int.Parse(textBox_SERVER_PORT.Text);
                        McProtocolApp.CommandFrame = Mitsubishi.McFrame.MC3E;
                        McProtocolApp.Open();
                        continue;
                    }

                    // 받는게 ALL 인지 체크
                    bool _All_check = false;
                    int _row_cnt = DT_MC_Rx.Rows.Count;
                    for (int idx = 0; idx < _row_cnt; idx++)
                    {
                        var _row = DT_MC_Rx.Rows[idx];
                        if (_row[1].ToString() == "ALL")
                        {
                            _All_check = true;
                        }
                    }

                    if (_All_check)
                    {
                        if (MC_Rx_Request[0] || MC_Rx_Request[1] || MC_Rx_Request[2] || MC_Rx_Request[3])
                        {
                            string _log = "MC Rx:";
                            for (int idx = 0; idx < _row_cnt; idx++)
                            {
                                var _row = DT_MC_Rx.Rows[idx];
                                if (_row[1].ToString() == "ALL")
                                {
                                    string _address = DT_MC_Rx.Rows[idx][0].ToString();
                                    int[] arrDeviceValue = new int[1];
                                    lock (_isMCRead_Lock)
                                    {
                                        Task<byte[]> McTask = McProtocolApp.ReadDeviceBlock(_address, 1, arrDeviceValue);
                                        McTask.Wait();
                                    }
                                    int t_v = arrDeviceValue[0];

                                    if (t_v >= 0)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            MC_Rx_Value[i] = t_v;
                                            MC_Rx_Value_Updated[i] = true;
                                            MC_Rx_Request[i] = false;
                                        }
                                        _row[2] = t_v;
                                        _log += " [" + _row[0].ToString() + ":" + t_v.ToString() + "]";

                                        if (LVApp.Instance().m_Config.PLC_Judge_view)
                                        {
                                            add_Log(_log);
                                        }

                                        if (dataGridView_MC_Rx.InvokeRequired)
                                        {
                                            dataGridView_MC_Rx.Invoke((MethodInvoker)delegate
                                            {
                                                dataGridView_MC_Rx.Refresh();
                                            });
                                        }
                                        else
                                        {
                                            dataGridView_MC_Rx.Refresh();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    { // ALL 없으면
                        if (MC_Rx_Request[_cam])// && !MC_Rx_Value_Updated[_cam])
                        {
                            string _log = "MC Rx:";
                            for (int idx = 0; idx < _row_cnt; idx++)
                            {
                                var _row = DT_MC_Rx.Rows[idx];
                                if (_row[1].ToString() == "CAM" + _cam.ToString())
                                {
                                    string _address = DT_MC_Rx.Rows[idx][0].ToString();
                                    int[] arrDeviceValue = new int[1];
                                    lock (_isMCRead_Lock)
                                    {
                                        Task<byte[]> McTask = McProtocolApp.ReadDeviceBlock(_address, 1, arrDeviceValue);
                                        McTask.Wait();
                                    }
                                    int t_v = arrDeviceValue[0];
                                    if (_row[1].ToString() == "CAM" + _cam.ToString() && t_v >= 0)
                                    {
                                        MC_Rx_Value[_cam] = t_v;
                                        MC_Rx_Value_Updated[_cam] = true;
                                        _row[2] = t_v;
                                        _log += " [" + _row[0].ToString() + ":" + t_v.ToString() + "]";
                                    }

                                    if (dataGridView_MC_Rx.InvokeRequired)
                                    {
                                        dataGridView_MC_Rx.Invoke((MethodInvoker)delegate
                                        {
                                            dataGridView_MC_Rx.Refresh();
                                        });
                                    }
                                    else
                                    {
                                        dataGridView_MC_Rx.Refresh();
                                    }
                                }
                            }
                            if (MC_Rx_Value_Updated[_cam])
                            {
                                MC_Rx_Request[_cam] = false;
                                if (LVApp.Instance().m_Config.PLC_Judge_view)
                                {
                                    add_Log(_log);
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                        _cam++;
                        if (_cam == 4)
                        {
                            _cam = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord($"MCRx Error: {ex.StackTrace}");
                MC_Rx_threads_Check = false;
            }
        }

        void ThreadProcMCTx()
        {
            try
            {
                int _cam = 0;
                while (MC_Tx_threads_Check)
                {
                    if (!MC_Tx_threads_Check)
                    {
                        break;
                    }
                    if (!Use_MC_Tx)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    if (CAM_Value_Updated[_cam])
                    {
                        if (!McProtocolApp.Connected)
                        { //MCProtocol 접속 안되어 있으면 접속하기
                            McProtocolApp.HostName = textBox_SERVER_IP.Text;
                            McProtocolApp.PortNumber = int.Parse(textBox_SERVER_PORT.Text);
                            McProtocolApp.CommandFrame = Mitsubishi.McFrame.MC3E;
                            McProtocolApp.Open();
                        }

                        if (McProtocolApp.Connected)
                        { // 접속 되면
                            CAM_Value_Updated[_cam] = false;
                            // 보내야 할 data 수 계산
                            int _row_cnt = DT_MC_Tx.Rows.Count;
                            List<Tuple<string, int, int>> Tx_list = new List<Tuple<string, int, int>>();//주소, Type, Value
                            for (int idx = 0; idx < _row_cnt; idx++)
                            {
                                var _row = DT_MC_Tx.Rows[idx];
                                if (_row[2].ToString() == "CAM" + _cam.ToString())
                                {
                                    int t_ROI = -1;
                                    int.TryParse(_row[3].ToString(), out t_ROI);
                                    double t_Scale = 1;
                                    double.TryParse(_row[4].ToString(), out t_Scale);

                                    if (t_ROI <= 0)
                                    {// 고정값

                                    }
                                    else
                                    {// ROI값
                                        if (_cam == 0)
                                        {
                                            _row[5] = (int)(CAM0_Value[t_ROI - 1] * t_Scale);
                                        }
                                        if (_cam == 1)
                                        {
                                            _row[5] = (int)(CAM1_Value[t_ROI - 1] * t_Scale);
                                        }
                                        if (_cam == 2)
                                        {
                                            _row[5] = (int)(CAM2_Value[t_ROI - 1] * t_Scale);
                                        }
                                        if (_cam == 3)
                                        {
                                            _row[5] = (int)(CAM3_Value[t_ROI - 1] * t_Scale);
                                        }
                                    }

                                    int t_Type = 2;
                                    if (_row[1].ToString() == "WORD")
                                    {
                                        t_Type = 1;
                                    }
                                    // 주소와 값을 list에 넣기
                                    int t_Value = 0;
                                    int.TryParse(_row[5].ToString(), out t_Value);
                                    Tx_list.Add(new Tuple<string, int, int>(_row[0].ToString(), t_Type, t_Value));
                                }
                            }

                            string _log = "MC Tx:";
                            foreach (var _data in Tx_list)
                            {
                                if (_data.Item2 == 1)
                                {
                                    lock (_isMCRead_Lock)
                                    {
                                        McProtocolApp.SetDevice(_data.Item1, _data.Item3);
                                    }
                                }
                                else
                                {
                                    int[] nData = new int[2];
                                    int[] t_nData = Convert_Data(_data.Item3);
                                    nData[0] = t_nData[0]; nData[1] = t_nData[1];
                                    lock (_isMCRead_Lock)
                                    {
                                        McProtocolApp.WriteDeviceBlock(_data.Item1, 2, nData);
                                    }
                                }
                                _log += " [" + _data.Item1 + ":" + _data.Item3.ToString() + "]";
                            }
                            if (LVApp.Instance().m_Config.PLC_Judge_view)
                            {
                                add_Log(_log);
                            }
                        }
                    }
                    _cam++;
                    _cam %= 4;
                }
            }
            catch(Exception ex)
            {
                MC_Tx_threads_Check = false;
                DebugLogger.Instance().LogRecord($"MCTx Error: {ex.StackTrace}");
            }
        }

        private void button_MCRx_Test_Click(object sender, EventArgs e)
        {
            MC_Rx_Request[0] = MC_Rx_Request[1] = MC_Rx_Request[2] = MC_Rx_Request[3] = true;
        }

        // 끝 2024.08.27 by CD
    }
}
