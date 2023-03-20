using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automation.BDaq;

namespace LV_Inspection_System.UTIL
{
    public class Advantech_DIO
    {
        private Automation.BDaq.InstantDoCtrl inst_Do = new Automation.BDaq.InstantDoCtrl();
        private Automation.BDaq.InstantDiCtrl inst_Di = new Automation.BDaq.InstantDiCtrl();
        public bool m_Initialized = false;

        public int[] m_DoportNum = new int[4];
        public List<BitArray> m_DoportData = new List<BitArray>();
        public int[] m_DiportNum = new int[4];
        public List<BitArray> m_DiportData = new List<BitArray>();
        public int[] CAM_Di_Port = new int[4]; // 카메라 0~4 트리거 인식할 포트 1~4
        public int[] CAM_Di_bit = new int[4];  // 카메라 0~4 트리커 인식할 비트 0~7
        public int m_Trigger_Interval = 100;

        Thread Do_thread = null;
        Thread Di_thread = null;
        public bool Do_thread_flag = false;
        public bool Di_thread_flag = false;
        public int Do_Job_Mode = 0;
        BackgroundWorker[] Grab_worker = new BackgroundWorker[4];
        BackgroundWorker[] DI_worker = new BackgroundWorker[4];
        public bool[] DI_worker_Initialized = new bool[4];

        public event EventHandler CAM0_Trigger_Completed;
        public event EventHandler CAM1_Trigger_Completed;
        public event EventHandler CAM2_Trigger_Completed;
        public event EventHandler CAM3_Trigger_Completed;
        private EventArgs CAM0_Trigger_Event = new EventArgs();
        private EventArgs CAM1_Trigger_Event = new EventArgs();
        private EventArgs CAM2_Trigger_Event = new EventArgs();
        private EventArgs CAM3_Trigger_Event = new EventArgs();

        public void Initialize()
        {
            if (m_Initialized)
            {
                return;
            }
            m_Initialized = true;
            try
            {
                var Di_devicce_list = inst_Di.SupportedDevices;
                var Do_devicce_list = inst_Do.SupportedDevices;
                inst_Do.SelectedDevice = new DeviceInformation(1);
                if (!inst_Do.Initialized)
                {
                    m_Initialized = false;
                    MessageBox.Show("No device be selected or device open failed!", "DO");
                    return;
                }
                else
                {
                    InitializeDoPortState();
                    if (!Do_thread_flag)
                    {
                        Do_thread_flag = true;
                        Do_thread = new Thread(Do_Process);
                        Do_thread.Priority = ThreadPriority.Highest;
                        Do_thread.IsBackground = true;
                        Do_thread.Start();
                    }
                }
                inst_Di.SelectedDevice = new DeviceInformation(1);
                if (!inst_Di.Initialized)
                {
                    MessageBox.Show("No device be selected or device open failed!", "DI");
                    return;
                }
                else
                {
                    // 인터럽트 안됨
                    //inst_Di.Interrupt += new System.EventHandler<Automation.BDaq.DiSnapEventArgs>(inst_Di_Interrupt);
                    //Start_Di_Monitoring();

                    // 쓰레드로 구현
                    InitializeDiPortState();
                    if (!Di_thread_flag)
                    {
                        Di_thread_flag = true;
                        Di_thread = new Thread(Di_Process);
                        Di_thread.IsBackground = true;
                        Di_thread.Start();
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (Grab_worker[i] == null)
                        {
                            Grab_worker[i] = new BackgroundWorker();
                        }
                        if (DI_worker[i] == null)
                        {
                            DI_worker[i] = new BackgroundWorker();
                            DI_worker_Initialized[i] = false;
                        }
                    }
                    Grab_worker[0].DoWork += new DoWorkEventHandler(CAM0_Grab);
                    Grab_worker[1].DoWork += new DoWorkEventHandler(CAM1_Grab);
                    Grab_worker[2].DoWork += new DoWorkEventHandler(CAM2_Grab);
                    Grab_worker[3].DoWork += new DoWorkEventHandler(CAM3_Grab);

                    DI_worker[0].DoWork += new DoWorkEventHandler(Camera_Auto_Manual);
                }
            }
            catch
            {
                Console.Write("Error on initial stage");
            }
        }


        public void Release()
        {
            if (!m_Initialized)
            {
                return;
            }
            m_Initialized = false;
            if (Di_thread_flag)
            {
                Di_thread_flag = false;
                Di_thread.Abort();
            }
            Grab_worker[0].DoWork -= new DoWorkEventHandler(CAM0_Grab);
            Grab_worker[1].DoWork -= new DoWorkEventHandler(CAM1_Grab);
            Grab_worker[2].DoWork -= new DoWorkEventHandler(CAM2_Grab);
            Grab_worker[3].DoWork -= new DoWorkEventHandler(CAM3_Grab);
            DI_worker[0].DoWork -= new DoWorkEventHandler(Camera_Auto_Manual);
            for (int i = 0; i < 4; i++)
            {
                if (Grab_worker[i] != null)
                {
                    Grab_worker[i] = null;
                }
                if (DI_worker[i] != null)
                {
                    DI_worker[i] = null;
                }
            }

        }

        Stopwatch[] SW_Do_Port0 = new Stopwatch[8];
        Stopwatch[] SW_Do_Port1 = new Stopwatch[8];
        public void Do_Process()
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    SW_Do_Port0[i] = new Stopwatch();
                    SW_Do_Port0[i].Stop();
                    SW_Do_Port1[i] = new Stopwatch();
                    SW_Do_Port1[i].Stop();
                }
                while (Do_thread_flag)
                {
                    if (!Do_thread_flag)
                    {
                        break;
                    }

                    if (Do_Job_Mode == 0)
                    {
                        Thread.Sleep(1);

                        for (int i = 0; i < 8; i++)
                        {
                            if (i == 1)
                            {
                                if (SW_Do_Port0[i].ElapsedMilliseconds <= 0)
                                {
                                    SW_Do_Port0[i].Start();
                                    if (m_DoportData[0].Get(i))
                                    {
                                        m_DoportData[0].Set(i, false);
                                        //LVApp.Instance().m_mainform.add_Log(i.ToString() + "CH:OFF, " + SW_Do_Port0[i].ElapsedMilliseconds.ToString());
                                    }
                                    else
                                    {
                                        m_DoportData[0].Set(i, true);
                                        //LVApp.Instance().m_mainform.add_Log(i.ToString() + "CH:ON, " + SW_Do_Port0[i].ElapsedMilliseconds.ToString());
                                    }
                                    Do_Job_Mode = 1;
                                    //m_DoportData[0].Set(3, true);
                                }
                                if (SW_Do_Port0[i].ElapsedMilliseconds > 999)
                                {
                                    SW_Do_Port0[i].Stop();
                                    //LVApp.Instance().m_mainform.add_Log(i.ToString() + "CH:" + SW_Do_Port0[i].ElapsedMilliseconds.ToString());
                                    SW_Do_Port0[i].Reset();
                                    Do_Job_Mode = 1;
                                }
                            }

                            if (i > 1)
                            {
                                //카메라0번 결과 OK, NG
                                //카메라1번 결과 OK, NG
                                //카메라2번 결과 OK, NG
                                if (m_DoportData[0].Get(i) && SW_Do_Port0[i].ElapsedMilliseconds <= 0)
                                {
                                    //LVApp.Instance().m_mainform.add_Log(i.ToString() + "CH:ON," + SW_Do_Port0[i].ElapsedMilliseconds.ToString());
                                    SW_Do_Port0[i].Start();
                                }
                                if (SW_Do_Port0[i].ElapsedMilliseconds > m_Trigger_Interval - 1)
                                {
                                    SW_Do_Port0[i].Stop();
                                    //LVApp.Instance().m_mainform.add_Log(i.ToString() + "CH:OFF," + SW_Do_Port0[i].ElapsedMilliseconds.ToString());
                                    SW_Do_Port0[i].Reset();
                                    m_DoportData[0].Set(i, false);
                                    Do_Job_Mode = 1;
                                }
                            }

                            if (i >= 0 && i < 2)
                            {
                                //카메라3번 결과 OK, NG
                                if (m_DoportData[1].Get(i) && SW_Do_Port1[i].ElapsedMilliseconds <= 0)
                                {
                                    SW_Do_Port1[i].Start();
                                }
                                if (SW_Do_Port1[i].ElapsedMilliseconds > m_Trigger_Interval - 1)
                                {
                                    SW_Do_Port1[i].Stop();
                                    SW_Do_Port1[i].Reset();
                                    m_DoportData[1].Set(i, false);
                                    Do_Job_Mode = 1;
                                }
                            }
                        }
                    }
                    else if (Do_Job_Mode == 1)
                    {
                        DO_Write(0);
                        DO_Write(1);
                        Do_Job_Mode = 0;
                    }
                }
            }
            catch
            { }
        }

        public void Di_Process()
        {
            //int[] t_CNT = new int[4]; t_CNT[0] = t_CNT[1] = t_CNT[2] = t_CNT[3] = 0;
            try
            {
                while (Di_thread_flag)
                {
                    if (!Di_thread_flag)
                    {
                        break;
                    }
                    Thread.Sleep(1);

                    for (int port = 0; port < 2; port++)
                    {
                        byte t_Byte = ConvertToByte(m_DiportData[port]); // 직전 Di
                        byte portData;
                        inst_Di.Read(m_DiportNum[port], out portData);
                        var bytes = new byte[] { portData };
                        BitArray t_bit = new BitArray(bytes);            // 현재 Di

                        if (t_Byte.Equals(portData))
                        {
                            continue;
                        }
                        if (portData > 0x00)
                        {
                            if (CAM_Di_Port[0] == port)
                            {
                                if (!m_DiportData[CAM_Di_Port[0]].Get(CAM_Di_bit[0]) && t_bit.Get(CAM_Di_bit[0]) && CAM0_Trigger_Completed != null)
                                {
                                    Grab_worker[0].RunWorkerAsync();
                                    //CAM0_Trigger_Completed(this, CAM0_Trigger_Event);
                                    //t_CNT[0]++;
                                    //LVApp.Instance().m_mainform.add_Log("CAM0 DI " + t_CNT[0].ToString());
                                }
                            }
                            if (CAM_Di_Port[1] == port)
                            {
                                if (!m_DiportData[CAM_Di_Port[1]].Get(CAM_Di_bit[1]) && t_bit.Get(CAM_Di_bit[1]) && CAM1_Trigger_Completed != null)
                                {
                                    Grab_worker[1].RunWorkerAsync();
                                    //CAM1_Trigger_Completed(this, CAM1_Trigger_Event);
                                    //t_CNT[1]++;
                                    //LVApp.Instance().m_mainform.add_Log("CAM1 DI " + t_CNT[1].ToString());
                                }
                            }
                            if (CAM_Di_Port[2] == port)
                            {
                                if (!m_DiportData[CAM_Di_Port[2]].Get(CAM_Di_bit[2]) && t_bit.Get(CAM_Di_bit[2]) && CAM2_Trigger_Completed != null)
                                {
                                    Grab_worker[2].RunWorkerAsync();
                                    //CAM2_Trigger_Completed(this, CAM2_Trigger_Event);
                                    //t_CNT[2]++;
                                    //LVApp.Instance().m_mainform.add_Log("CAM2 DI " + t_CNT[2].ToString());
                                }
                            }
                            if (CAM_Di_Port[3] == port)
                            {
                                if (!m_DiportData[CAM_Di_Port[3]].Get(CAM_Di_bit[3]) && t_bit.Get(CAM_Di_bit[3]) && CAM3_Trigger_Completed != null)
                                {
                                    Grab_worker[3].RunWorkerAsync();
                                    //CAM3_Trigger_Completed(this, CAM3_Trigger_Event);
                                    //t_CNT[3]++;
                                    //LVApp.Instance().m_mainform.add_Log("CAM3 DI");
                                }
                            }
                            if (port == 0)
                            {
                                if (!DI_worker_Initialized[0])
                                {
                                    if (t_bit.Get(7))
                                    {// High
                                        LVApp.Instance().m_Config.m_Auto_Mode = true;
                                        DI_worker[0].RunWorkerAsync();
                                    }
                                    else if (!t_bit.Get(7))
                                    {// Low
                                        LVApp.Instance().m_Config.m_Auto_Mode = false;
                                        DI_worker[0].RunWorkerAsync();
                                    }
                                    DI_worker_Initialized[0] = true;
                                }
                                else
                                {
                                    if (!m_DiportData[0].Get(7) && t_bit.Get(7))
                                    {// Low -> High
                                        LVApp.Instance().m_Config.m_Auto_Mode = true;
                                        DI_worker[0].RunWorkerAsync();
                                    }
                                    else if (m_DiportData[0].Get(7) && !t_bit.Get(7))
                                    {// Low -> High
                                        LVApp.Instance().m_Config.m_Auto_Mode = false;
                                        DI_worker[0].RunWorkerAsync();
                                    }
                                }
                            }
                            //LVApp.Instance().m_mainform.add_Log(t_CNT[0].ToString()
                            //     + " " + t_CNT[1].ToString() + " " + t_CNT[2].ToString() + " " + t_CNT[3].ToString());

                            m_DiportData[port] = t_bit;
                        }
                    }
                }
            }
            catch
            { }
        }

        private void CAM0_Grab(object sender, DoWorkEventArgs e)
        {
            CAM0_Trigger_Completed(this, CAM0_Trigger_Event);
        }
        private void CAM1_Grab(object sender, DoWorkEventArgs e)
        {
            CAM1_Trigger_Completed(this, CAM1_Trigger_Event);
        }
        private void CAM2_Grab(object sender, DoWorkEventArgs e)
        {
            CAM2_Trigger_Completed(this, CAM2_Trigger_Event);
        }
        private void CAM3_Grab(object sender, DoWorkEventArgs e)
        {
            CAM3_Trigger_Completed(this, CAM3_Trigger_Event);
        }

        private void Camera_Auto_Manual(object sender, DoWorkEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 4 && LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[0])
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.ctr_MIL_LINK1.button_SEND_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 4 && LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[1])
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.ctr_MIL_LINK1.button_SEND_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 4 && LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[2])
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.ctr_MIL_LINK1.button_SEND_Click(sender, e);
            }
            if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 4 && LVApp.Instance().m_MIL.m_Auto_Manual_Mode_Use[3])
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.ctr_MIL_LINK1.button_SEND_Click(sender, e);
            }
        }

        public void Stop_Thread()
        {
            if (Do_thread_flag)
            {
                Do_thread_flag = false;
                Do_thread.Abort();
            }
            if (Di_thread_flag)
            {
                Di_thread_flag = false;
                Di_thread.Abort();
            }
        }

        private void InitializeDoPortState()
        {
            byte portData = 0;
            ErrorCode err = ErrorCode.Success;
            for (int i = 0; (i + ConstVal.StartPort) < inst_Do.Features.PortCount && i < ConstVal.PortCountShow; ++i)
            {
                err = inst_Do.Read(i + ConstVal.StartPort, out portData);
                if (err != ErrorCode.Success)
                {
                    HandleError(err);
                    return;
                }

                m_DoportNum[i] = i + ConstVal.StartPort;
                var bytes = new byte[] { portData };
                m_DoportData.Add(new BitArray(bytes));
            }
        }

        private void InitializeDiPortState()
        {
            byte portData = 0;
            ErrorCode err = ErrorCode.Success;
            for (int i = 0; (i + ConstVal.StartPort) < inst_Di.Features.PortCount && i < ConstVal.PortCountShow; ++i)
            {
                err = inst_Di.Read(i + ConstVal.StartPort, out portData);
                if (err != ErrorCode.Success)
                {
                    HandleError(err);
                    return;
                }

                m_DiportNum[i] = i + ConstVal.StartPort;
                var bytes = new byte[] { portData };
                m_DiportData.Add(new BitArray(bytes));
            }

            CAM_Di_Port[0] = m_DiportNum[0];
            CAM_Di_bit[0] = 0;
            CAM_Di_Port[1] = m_DiportNum[0];
            CAM_Di_bit[1] = 1;
            CAM_Di_Port[2] = m_DiportNum[0];
            CAM_Di_bit[2] = 2;
            CAM_Di_Port[3] = m_DiportNum[0];
            CAM_Di_bit[3] = 3;
        }

        private void DO_Write(int port_num)
        {
            //Stopwatch SW = new Stopwatch();
            //SW.Start();
            //DO 0, 1, 2, 3
            //DI 0, 1, 2, 3
            ErrorCode err = ErrorCode.Success;
            byte t_Byte = ConvertToByte(m_DoportData[port_num]);
            err = inst_Do.Write(m_DoportNum[port_num], t_Byte);
            //string str = SW.ElapsedMilliseconds.ToString();
            //SW.Stop();
            if (err != ErrorCode.Success)
            {
                HandleError(err);
            }
        }


        public void Start_Di_Monitoring()
        {
            ErrorCode err = ErrorCode.Success;
            err = inst_Di.SnapStart();
            if (err != ErrorCode.Success)
            {
                HandleError(err);
                return;
            }
        }

        public void Stop_Di_Monitoring()
        {
            ErrorCode err = ErrorCode.Success;
            err = inst_Di.SnapStop();
            //inst_Di.Interrupt -= new System.EventHandler<Automation.BDaq.DiSnapEventArgs>(inst_Di_Interrupt);
        }

        private void inst_Di_Interrupt(object sender, DiSnapEventArgs e)
        {
            try
            {
                //Invoke(new UpdateListview(UpdateListviewMethod), new object[] { e.SrcNum, e.PortData });
                // 여기에 파싱할것
                Console.Write("SrcNum = " + e.SrcNum.ToString() + "  PortData = " + e.PortData.ToString());
            }
            catch (System.Exception) { }
        }


        private void HandleError(ErrorCode err)
        {
            if ((err >= ErrorCode.ErrorHandleNotValid) && (err != ErrorCode.Success))
            {
                MessageBox.Show("Sorry ! Some errors happened, the error code is: " + err.ToString());
            }
        }

        byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }

    public static class ConstVal
    {
        public const int StartPort = 0;
        public const int PortCountShow = 4;
    }
}
