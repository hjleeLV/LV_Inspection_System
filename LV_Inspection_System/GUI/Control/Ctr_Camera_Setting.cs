using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using OfficeOpenXml;
using System.Threading;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Camera_Setting : UserControl
    {
        protected string m_Camera_Name = "CAM0";
        protected double m_Grab_Num = 0;
        protected double[] m_Resolution = new double[2];
        public Stopwatch sw = new Stopwatch();

        public Ctr_Camera_Setting()
        {
            InitializeComponent();
            //label_Camera.Text = m_Camera_Name;
            //textBox_Camera_Name.Text = m_Camera_Name;
        }
        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    checkBox1.Text = "LINE1 트리거 모드";
                    Force_USE.Text = "카메라 사용안함 :";
                    label7.Text = "카메라 종류 :";
                    button_CAMKIND_Apply.Text = "적용";
                    label_Camera_Name.Text = "카메라 이름 :";
                    button_Change_Cam_Name.Text = "적용";
                    label_Spatial_Resolution.Text = "해상도 :";
                    label1.Text = "가로";
                    label2.Text = "세로";
                    button_Change_Resolution.Text = "적용";
                    label6.Text = "트리거 지연값 :";
                    label3.Text = "반전 :";
                    checkBox_LR.Text = "좌우 반전";
                    checkBox_TB.Text = "상하 반전";
                    button_TRIGGER_DELAY_CHANGE.Text = "적용";
                    button_Change_Flip.Text = "적용";
                    label4.Text = "이미지 회전 :";
                    button_Change_Rotation.Text = "적용";
                    label8.Text = "카메라 연동 :";
                    button_Change_COCAM.Text = "적용";
                    toolStripButtonOneShot.ToolTipText = "촬영";
                    toolStripButtonContinuousShot.ToolTipText = "동영상";
                    toolStripButtonStop.ToolTipText = "정지";
                    toolStripButtonInitialize.ToolTipText = "리셋";
                    toolStripButtonConnect.ToolTipText = "연결";
                    toolStripButtonDisconnect.ToolTipText = "연결해제";
                    toolStripButton_SAVE.ToolTipText = "저장";
                    toolStripButton_LOAD.ToolTipText = "불러오기";
                    toolStripButtonImageSave.ToolTipText = "이미지 저장";

                    button_Merge_Apply.Text = "적용";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    checkBox1.Text = "LINE1 Trigger Mode";
                    Force_USE.Text = "Camera Disable :";
                    label7.Text = "Kind of Camera :";
                    button_CAMKIND_Apply.Text = "Apply";
                    label_Camera_Name.Text = "Camera Name :";
                    button_Change_Cam_Name.Text = "Apply";
                    label_Spatial_Resolution.Text = "Resolution :";
                    label1.Text = "Hor.";
                    label2.Text = "Ver.";
                    button_Change_Resolution.Text = "Apply";
                    label6.Text = "Trigger Delay :";
                    label3.Text = "Flip :";
                    checkBox_LR.Text = "Left Right";
                    checkBox_TB.Text = "Top Bottom";
                    button_TRIGGER_DELAY_CHANGE.Text = "Apply";
                    button_Change_Flip.Text = "Apply";
                    label4.Text = "Rotation :";
                    button_Change_Rotation.Text = "Apply";
                    label8.Text = "Interlock :";
                    button_Change_COCAM.Text = "Apply";
                    toolStripButtonOneShot.ToolTipText = "Snapshot";
                    toolStripButtonContinuousShot.ToolTipText = "Continous";
                    toolStripButtonStop.ToolTipText = "Stop";
                    toolStripButtonInitialize.ToolTipText = "Reset";
                    toolStripButtonConnect.ToolTipText = "Connect";
                    toolStripButtonDisconnect.ToolTipText = "disconnect";
                    toolStripButton_SAVE.ToolTipText = "Save";
                    toolStripButton_LOAD.ToolTipText = "Load";
                    toolStripButtonImageSave.ToolTipText = "Image save";
                    button_Merge_Apply.Text = "Apply";
                }
                else if (value == 2 && m_Language != value)
                {// 중국어
                    checkBox1.Text = "LINE1 触发模式";
                    Force_USE.Text = "Camera 禁用 :";
                    label7.Text = "Camera 种类 :";
                    button_CAMKIND_Apply.Text = "应用";
                    label_Camera_Name.Text = "Camera 名称 :";
                    button_Change_Cam_Name.Text = "应用";
                    label_Spatial_Resolution.Text = "空间分辨率 :";
                    label1.Text = "水平";
                    label2.Text = "垂直";
                    button_Change_Resolution.Text = "应用";
                    label6.Text = "特里格·德莱 :";
                    label3.Text = "翻转 :";
                    checkBox_LR.Text = "左右";
                    checkBox_TB.Text = "顶部底部";
                    button_TRIGGER_DELAY_CHANGE.Text = "应用";
                    button_Change_Flip.Text = "应用";
                    label4.Text = "旋转 :";
                    button_Change_Rotation.Text = "应用";
                    label8.Text = "联锁 :";
                    button_Change_COCAM.Text = "应用";
                    toolStripButtonOneShot.ToolTipText = "快照";
                    toolStripButtonContinuousShot.ToolTipText = "连续";
                    toolStripButtonStop.ToolTipText = "停止";
                    toolStripButtonInitialize.ToolTipText = "重置";
                    toolStripButtonConnect.ToolTipText = "连接";
                    toolStripButtonDisconnect.ToolTipText = "断开";
                    toolStripButton_SAVE.ToolTipText = "救";
                    toolStripButton_LOAD.ToolTipText = "负荷";
                    toolStripButtonImageSave.ToolTipText = "图像保存";
                    button_Merge_Apply.Text = "应用";
                }
                m_Language = value;
            }
        }

        [Description("Set Camera Name"), Category("Data")]
        public string m_SetCameraName
        {
            get { return m_Camera_Name; }
            set
            {
                m_Camera_Name = value;
                label_Camera.Text = m_Camera_Name + " Camera Setting";
                textBox_Camera_Name.Text = m_Camera_Name;
            }
        }

        [Description("Camera Resolution"), Category("Data")]
        public double[] m_SetResolution
        {
            get { return m_Resolution; }
            set
            {
                m_Resolution = value;
                textBox_RESOLUTION_X.Text = m_Resolution[0].ToString("0.00000000");
                textBox_RESOLUTION_Y.Text = m_Resolution[1].ToString("0.00000000");
                LVApp.Instance().m_Config.m_Cam_Resolution[Convert.ToInt32(m_Camera_Name.Substring(3, 1)) % 4, 0] = m_Resolution[0];
                LVApp.Instance().m_Config.m_Cam_Resolution[Convert.ToInt32(m_Camera_Name.Substring(3, 1)) % 4, 1] = m_Resolution[1];
            }
        }

        public double Grab_Num
        {
            get { return m_Grab_Num; }
            set
            {
                m_Grab_Num = value;
                System.Threading.ThreadPool.QueueUserWorkItem(o =>
                {
                    Update_Grab_Num();
                });
            }
        }

        private object syncLock = new object();
        bool isInCall = false;
        private void Update_Grab_Num()
        {
            lock (syncLock)
            {
                if (isInCall)
                    return;
                isInCall = true;
            }
            try
            {
                if (label_Grab_Num.InvokeRequired)
                {
                    label_Grab_Num.Invoke((MethodInvoker)delegate
                    {
                        label_Grab_Num.Text = "[" + m_Grab_Num.ToString("000000000") + "]";
                    });
                }
                else
                {
                    label_Grab_Num.Text = "[" + m_Grab_Num.ToString("000000000") + "]";
                }
                Thread.Sleep(100);
            }
            finally
            {
                lock (syncLock)
                {
                    isInCall = false;
                }
            }
        }

        public void Connect_imageProvider()
        {
            if (m_Camera_Name == "")
            {
                return;
            }
            if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
            {
                checkBox1.Checked = LVApp.Instance().m_mainform.ctrCam1.Camera_Trigger_Mode;
                sliderWidth.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
                sliderHeight.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
                sliderOffsetX.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
                sliderOffsetY.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
                sliderGain.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
                sliderExposureTime.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
                comboBoxPixelFormat.MyImageProvider = LVApp.Instance().m_mainform.ctrCam1.m_imageProvider;
            }
            else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
            {
                checkBox1.Checked = LVApp.Instance().m_mainform.ctrCam2.Camera_Trigger_Mode;
                sliderWidth.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
                sliderHeight.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
                sliderOffsetX.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
                sliderOffsetY.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
                sliderGain.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
                sliderExposureTime.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
                comboBoxPixelFormat.MyImageProvider = LVApp.Instance().m_mainform.ctrCam2.m_imageProvider;
            }
            else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
            {
                checkBox1.Checked = LVApp.Instance().m_mainform.ctrCam3.Camera_Trigger_Mode;
                sliderWidth.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
                sliderHeight.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
                sliderOffsetX.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
                sliderOffsetY.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
                sliderGain.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
                sliderExposureTime.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
                comboBoxPixelFormat.MyImageProvider = LVApp.Instance().m_mainform.ctrCam3.m_imageProvider;
            }
            else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
            {
                checkBox1.Checked = LVApp.Instance().m_mainform.ctrCam4.Camera_Trigger_Mode;
                sliderWidth.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
                sliderHeight.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
                sliderOffsetX.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
                sliderOffsetY.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
                sliderGain.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
                sliderExposureTime.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
                comboBoxPixelFormat.MyImageProvider = LVApp.Instance().m_mainform.ctrCam4.m_imageProvider;
            }
        }

        private void EnableButtons(bool canGrab, bool canStop)
        {
            //canGrab = true;
            toolStripButtonContinuousShot.Enabled = canGrab;
            toolStripButtonOneShot.Enabled = canGrab;
            toolStripButtonDisconnect.Enabled = canGrab;
            toolStripButtonConnect.Enabled = canStop;
            toolStripButtonStop.Enabled = canStop;
            checkBox1.Enabled = canGrab;
        }

        public void toolStripButtonOneShot_Click(object sender, EventArgs e)
        {
            //sw.Reset();
            //sw.Start();

            //LVApp.Instance().m_Config.Cam0_Seq = 0;
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {
                Stopwatch Cam_SW = new Stopwatch(); Cam_SW.Start();
                if (cam_num == 0)
                {
                    LVApp.Instance().m_MIL.CAM0_Mil_Grab();
                }
                else if (cam_num == 1)
                {
                    LVApp.Instance().m_MIL.CAM1_Mil_Grab();
                }
                else if (cam_num == 2)
                {
                    LVApp.Instance().m_MIL.CAM2_Mil_Grab();
                }
                else if (cam_num == 3)
                {
                    LVApp.Instance().m_MIL.CAM3_Mil_Grab();
                }
                Cam_SW.Stop();
                Add_Message("Grab Time = " + Cam_SW.ElapsedMilliseconds.ToString() + "ms");
                EnableButtons(true, false);
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            {
                LVApp.Instance().m_GenICam.OneShot(cam_num);
                GeniCam_sliderWidth.Enabled = GeniCam_sliderHeight.Enabled =
                GeniCam_sliderOffsetX.Enabled = GeniCam_sliderOffsetY.Enabled = true;
                Thread.Sleep(100);
                LVApp.Instance().m_GenICam.Stop(cam_num);
                EnableButtons(true, false);
            }
            else
            {
                if (cam_num == 0)
                {
                    //LVApp.Instance().m_Config.Cam0_Seq = 1;
                    if (LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam1.OneShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam1.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM1.On = true;
                            LVApp.Instance().m_mainform.ctrCam1.OneShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, false);
                }
                else if (cam_num == 1)
                {
                    //LVApp.Instance().m_Config.Cam2_Seq = 1;
                    if (LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam2.OneShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam2.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM2.On = true;
                            LVApp.Instance().m_mainform.ctrCam2.OneShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, false);
                }
                else if (cam_num == 2)
                {
                    //LVApp.Instance().m_Config.Cam5_Seq = 0;
                    if (LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam3.OneShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam3.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM3.On = true;
                            LVApp.Instance().m_mainform.ctrCam3.OneShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, false);
                }
                else if (cam_num == 3)
                {
                    //LVApp.Instance().m_Config.Cam5_Seq = 1;
                    if (LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam4.OneShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam4.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM4.On = true;
                            LVApp.Instance().m_mainform.ctrCam4.OneShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, false);
                }
            }
        }

        public void toolStripButtonContinuousShot_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {
                ////return;
                //Stopwatch Cam_SW = new Stopwatch(); Cam_SW.Start();
                //if (cam_num == 0)
                //{
                //    LVApp.Instance().m_MIL.CAM0_Mil_Grab();
                //    EnableButtons(LVApp.Instance().m_MIL.CAM0_Initialized, false);
                //}
                //else if (cam_num == 1)
                //{
                //    LVApp.Instance().m_MIL.CAM1_Mil_Grab();
                //    EnableButtons(LVApp.Instance().m_MIL.CAM1_Initialized, false);
                //}
                //else if (cam_num == 2)
                //{
                //    LVApp.Instance().m_MIL.CAM2_Mil_Grab();
                //    EnableButtons(LVApp.Instance().m_MIL.CAM2_Initialized, false);
                //}
                //else if (cam_num == 3)
                //{
                //    LVApp.Instance().m_MIL.CAM3_Mil_Grab();
                //    EnableButtons(LVApp.Instance().m_MIL.CAM3_Initialized, false);
                //}
                //Cam_SW.Stop();
                //Add_Message("Grab Time = " + Cam_SW.ElapsedMilliseconds.ToString() + "ms");
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            {
                LVApp.Instance().m_GenICam.Continuous(cam_num);

                toolStripButtonContinuousShot.Enabled = false;
                toolStripButtonOneShot.Enabled = false;
                toolStripButtonDisconnect.Enabled = false;
                toolStripButtonConnect.Enabled = false;
                toolStripButtonStop.Enabled = true;
                checkBox1.Enabled = false;
                GeniCam_sliderWidth.Enabled = GeniCam_sliderHeight.Enabled = false;
                GeniCam_sliderOffsetX.Enabled = GeniCam_sliderOffsetY.Enabled = true;
            }
            else
            {
                if (cam_num == 0)
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1] = true;
                    if (LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam1.ContinuousShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam1.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM1.On = true;
                            LVApp.Instance().m_mainform.ctrCam1.ContinuousShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, true);
                }
                else if (cam_num == 1)
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2] = true;
                    if (LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam2.ContinuousShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam2.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM2.On = true;
                            LVApp.Instance().m_mainform.ctrCam2.ContinuousShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, true);
                }
                else if (cam_num == 2)
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3] = true;
                    if (LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam3.ContinuousShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam3.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM3.On = true;
                            LVApp.Instance().m_mainform.ctrCam3.ContinuousShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, true);
                }
                else if (cam_num == 3)
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4] = true;
                    if (LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam4.ContinuousShot();
                    }
                    else
                    {
                        if (LVApp.Instance().m_mainform.ctrCam4.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM4.On = true;
                            LVApp.Instance().m_mainform.ctrCam4.ContinuousShot();
                        }
                    }
                    EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, true);
                }

                toolStripButtonContinuousShot.Enabled = false;
                toolStripButtonOneShot.Enabled = false;
                toolStripButtonDisconnect.Enabled = false;
                toolStripButtonConnect.Enabled = false;
                toolStripButtonStop.Enabled = true;
                checkBox1.Enabled = false;
            }
        }

        public void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {
                if (cam_num == 0)
                {

                }
                if (cam_num == 1)
                {

                }
                if (cam_num == 2)
                {

                }
                if (cam_num == 3)
                {

                }
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            {
                LVApp.Instance().m_GenICam.Stop(cam_num);
                EnableButtons(LVApp.Instance().m_GenICam.CAM[cam_num].Connection, false);
                GeniCam_sliderWidth.Enabled = GeniCam_sliderHeight.Enabled =
                GeniCam_sliderOffsetX.Enabled = GeniCam_sliderOffsetY.Enabled = true;
            }
            else
            {
                if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1] = false;
                    if (LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam1.Stop();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2] = false;
                    if (LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam2.Stop();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3] = false;
                    if (LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam3.Stop();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
                {
                    LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4] = false;
                    if (LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam4.Stop();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, false);
                    }
                }
            }
            //LVApp.Instance().m_mainform.Button_Enable_Control(true);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[0] == 6)
                {
                    LVApp.Instance().m_GenICam.CAM[0].Line1Trigger = checkBox1.Checked;
                    LVApp.Instance().m_GenICam.Trigger_Mode(0);
                }
                else
                {
                    if (LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam1.Close();
                        LVApp.Instance().m_mainform.ctrCam1.Camera_Trigger_Mode = checkBox1.Checked;
                        LVApp.Instance().m_mainform.ctrCam1.Open();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, false);
                        //if (LVApp.Instance().m_mainform.ctrCam1.Camera_Trigger_Mode)
                        //{
                        //    LVApp.Instance().m_mainform.ctrCam1.OneShot();
                        //}
                    }
                }
            }
            else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[1] == 6)
                {
                    LVApp.Instance().m_GenICam.CAM[1].Line1Trigger = checkBox1.Checked;
                    LVApp.Instance().m_GenICam.Trigger_Mode(1);
                }
                else
                {
                    if (LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam2.Close();
                        LVApp.Instance().m_mainform.ctrCam2.Camera_Trigger_Mode = checkBox1.Checked;
                        LVApp.Instance().m_mainform.ctrCam2.Open();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, false);
                        //if (LVApp.Instance().m_mainform.ctrCam2.Camera_Trigger_Mode)
                        //{
                        //    LVApp.Instance().m_mainform.ctrCam2.OneShot();
                        //}
                    }
                }
            }
            else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[2] == 6)
                {
                    LVApp.Instance().m_GenICam.CAM[2].Line1Trigger = checkBox1.Checked;
                    LVApp.Instance().m_GenICam.Trigger_Mode(2);
                }
                else
                {
                    if (LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam3.Close();
                        LVApp.Instance().m_mainform.ctrCam3.Camera_Trigger_Mode = checkBox1.Checked;
                        LVApp.Instance().m_mainform.ctrCam3.Open();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, false);
                        //if (LVApp.Instance().m_mainform.ctrCam3.Camera_Trigger_Mode)
                        //{
                        //    LVApp.Instance().m_mainform.ctrCam3.OneShot();
                        //}
                    }
                }
            }
            else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[3] == 6)
                {
                    LVApp.Instance().m_GenICam.CAM[3].Line1Trigger = checkBox1.Checked;
                    LVApp.Instance().m_GenICam.Trigger_Mode(3);
                }
                else
                {
                    if (LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_mainform.ctrCam4.Close();
                        LVApp.Instance().m_mainform.ctrCam4.Camera_Trigger_Mode = checkBox1.Checked;
                        LVApp.Instance().m_mainform.ctrCam4.Open();
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, false);
                        //if (LVApp.Instance().m_mainform.ctrCam4.Camera_Trigger_Mode)
                        //{
                        //    LVApp.Instance().m_mainform.ctrCam4.OneShot();
                        //}
                    }
                }
            }
        }

        public void toolStripButtonInitialize_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {

                if (ctr_MIL_LINK1.m_Loaded)
                {
                    LVApp.Instance().m_MIL.MIL_Release(cam_num);
                    ////EnableButtons(false, true);
                    //if (cam_num == 0)
                    //{
                    //    LVApp.Instance().m_MIL.CAM0_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam1_GrabComplete);
                    //}
                    //if (cam_num == 1)
                    //{
                    //    LVApp.Instance().m_MIL.CAM1_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam2_GrabComplete);
                    //}
                    //if (cam_num == 2)
                    //{
                    //    LVApp.Instance().m_MIL.CAM2_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam3_GrabComplete);
                    //}
                    //if (cam_num == 3)
                    //{
                    //    LVApp.Instance().m_MIL.CAM3_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam4_GrabComplete);
                    //}

                    FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + cam_num.ToString() + ".dcf");
                    if (newFile.Exists)
                    {
                        if (ctr_MIL_LINK1.m_Loaded)
                        {
                            if (cam_num == 0)
                            {
                                LVApp.Instance().m_MIL.CAM0_dcfFilePath = newFile.FullName;
                                LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                                //LVApp.Instance().m_MIL.CAM0_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam1_GrabComplete);
                                EnableButtons(LVApp.Instance().m_MIL.CAM0_Initialized, false);
                            }
                            else if (cam_num == 1)
                            {
                                LVApp.Instance().m_MIL.CAM1_dcfFilePath = newFile.FullName;
                                LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                                //LVApp.Instance().m_MIL.CAM1_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam2_GrabComplete);
                                EnableButtons(LVApp.Instance().m_MIL.CAM1_Initialized, false);
                            }
                            else if (cam_num == 2)
                            {
                                LVApp.Instance().m_MIL.CAM2_dcfFilePath = newFile.FullName;
                                LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                               // LVApp.Instance().m_MIL.CAM2_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam3_GrabComplete);
                                EnableButtons(LVApp.Instance().m_MIL.CAM2_Initialized, false);
                            }
                            else if (cam_num == 3)
                            {
                                LVApp.Instance().m_MIL.CAM3_dcfFilePath = newFile.FullName;
                                LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                               // LVApp.Instance().m_MIL.CAM3_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam4_GrabComplete);
                                EnableButtons(LVApp.Instance().m_MIL.CAM3_Initialized, false);
                            }
                        }
                    }
                }
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            { // 중국 카메라
                LVApp.Instance().m_GenICam.Close(cam_num);
                GeniCam_sliderWidth.Enabled = GeniCam_sliderHeight.Enabled =
                GeniCam_sliderOffsetX.Enabled = GeniCam_sliderOffsetY.Enabled = true;
                //LVApp.Instance().m_GenICam.Open(cam_num);
                if (LVApp.Instance().m_GenICam.CAM[cam_num].Connection)
                {
                    EnableButtons(true, false);
                }
                else
                {
                    EnableButtons(false, true);
                }
            }
            else
            {
                if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
                {
                    if (LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_Config.m_Cam_Continuous_Mode[1] = false;
                        LVApp.Instance().m_mainform.ctrCam1.Close();
                        if (LVApp.Instance().m_mainform.ctrCam1.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM1.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
                {
                    if (LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_Config.m_Cam_Continuous_Mode[2] = false;
                        LVApp.Instance().m_mainform.ctrCam2.Close();
                        if (LVApp.Instance().m_mainform.ctrCam2.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM2.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
                {
                    if (LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_Config.m_Cam_Continuous_Mode[3] = false;
                        LVApp.Instance().m_mainform.ctrCam3.Close();
                        if (LVApp.Instance().m_mainform.ctrCam3.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM3.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
                {
                    if (LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        LVApp.Instance().m_Config.m_Cam_Continuous_Mode[4] = false;
                        LVApp.Instance().m_mainform.ctrCam4.Close();
                        if (LVApp.Instance().m_mainform.ctrCam4.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM4.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, false);
                    }
                }
            }

            toolStripButtonConnect_Click(sender, e);
            Grab_Num = 0;
        }

        public void toolStripButtonDisconnect_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {
                LVApp.Instance().m_MIL.MIL_Release(cam_num);
                EnableButtons(false, true);
                //if (cam_num == 0)
                //{
                //    LVApp.Instance().m_MIL.CAM0_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam1_GrabComplete);
                //}
                //if (cam_num == 1)
                //{
                //    LVApp.Instance().m_MIL.CAM1_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam2_GrabComplete);
                //}
                //if (cam_num == 2)
                //{
                //    LVApp.Instance().m_MIL.CAM2_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam3_GrabComplete);
                //}
                //if (cam_num == 3)
                //{
                //    LVApp.Instance().m_MIL.CAM3_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam4_GrabComplete);
                //}
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            { //중국 카메라
                LVApp.Instance().m_GenICam.Close(cam_num);
                GeniCam_sliderWidth.UpdateValues();
                GeniCam_sliderHeight.UpdateValues();
                GeniCam_sliderOffsetX.UpdateValues();
                GeniCam_sliderOffsetY.UpdateValues();
                GeniCam_sliderExposureTime.UpdateValues();
                GeniCam_sliderGain.UpdateValues();
                GeniCam_sliderWidth.Enabled = GeniCam_sliderHeight.Enabled =
                GeniCam_sliderOffsetX.Enabled = GeniCam_sliderOffsetY.Enabled = true;
                EnableButtons(false, true);
            }
            else
            {
                if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
                {
                    if (LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam1.Close())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM1.On = false;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, true);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
                {
                    if (LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam2.Close())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM2.On = false;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, true);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
                {
                    if (LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam3.Close())
                        {
                            // LVApp.Instance().m_Ctr_Auto.ledBulb_CAM3.On = false;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, true);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
                {
                    if (LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam4.Close())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM4.On = false;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, true);
                    }
                }
            }
            toolStripButtonStop.Enabled = false;
            checkBox1.Enabled = true;
        }

        public void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {
                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\CAM" + cam_num.ToString() + ".dcf");
                if (newFile.Exists)
                {
                    if (ctr_MIL_LINK1.m_Loaded)
                    {
                        if (cam_num == 0)
                        {
                            LVApp.Instance().m_MIL.CAM0_dcfFilePath = newFile.FullName;
                            LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                            LVApp.Instance().m_MIL.CAM0_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam1_GrabComplete);
                            LVApp.Instance().m_MIL.CAM0_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam1_GrabComplete);
                        }
                        else if (cam_num == 1)
                        {
                            LVApp.Instance().m_MIL.CAM1_dcfFilePath = newFile.FullName;
                            LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                            LVApp.Instance().m_MIL.CAM1_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam2_GrabComplete);
                            LVApp.Instance().m_MIL.CAM1_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam2_GrabComplete);
                        }
                        else if (cam_num == 2)
                        {
                            LVApp.Instance().m_MIL.CAM2_dcfFilePath = newFile.FullName;
                            LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                            LVApp.Instance().m_MIL.CAM2_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam3_GrabComplete);
                            LVApp.Instance().m_MIL.CAM2_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam3_GrabComplete);
                        }
                        else if (cam_num == 3)
                        {
                            LVApp.Instance().m_MIL.CAM3_dcfFilePath = newFile.FullName;
                            LVApp.Instance().m_MIL.Mil_Initialize(cam_num);
                            LVApp.Instance().m_MIL.CAM3_Grab_Completed -= new System.EventHandler(LVApp.Instance().m_mainform.MILCam4_GrabComplete);
                            LVApp.Instance().m_MIL.CAM3_Grab_Completed += new System.EventHandler(LVApp.Instance().m_mainform.MILCam4_GrabComplete);
                        }
                        EnableButtons(true, false);
                    }

                }
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            { //중국 카메라
                LVApp.Instance().m_GenICam.Open(cam_num);

                GeniCam_sliderWidth.Dev = LVApp.Instance().m_GenICam.CAM[cam_num].Dev;
                GeniCam_sliderWidth.UpdateValues();
                GeniCam_sliderHeight.Dev = LVApp.Instance().m_GenICam.CAM[cam_num].Dev;
                GeniCam_sliderHeight.UpdateValues();
                GeniCam_sliderOffsetX.Dev = LVApp.Instance().m_GenICam.CAM[cam_num].Dev;
                GeniCam_sliderOffsetX.UpdateValues();
                GeniCam_sliderOffsetY.Dev = LVApp.Instance().m_GenICam.CAM[cam_num].Dev;
                GeniCam_sliderOffsetY.UpdateValues();
                GeniCam_sliderExposureTime.Dev = LVApp.Instance().m_GenICam.CAM[cam_num].Dev;
                GeniCam_sliderExposureTime.UpdateValues();
                GeniCam_sliderGain.Dev = LVApp.Instance().m_GenICam.CAM[cam_num].Dev;
                GeniCam_sliderGain.UpdateValues();
                GeniCam_sliderWidth.Enabled = GeniCam_sliderHeight.Enabled =
                GeniCam_sliderOffsetX.Enabled = GeniCam_sliderOffsetY.Enabled = true;

                if (LVApp.Instance().m_GenICam.CAM[cam_num].Connection)
                {
                    EnableButtons(true, false);
                }
                else
                {
                    EnableButtons(false, true);
                }
            }
            else
            {
                if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
                {
                    if (comboBox_CO_CAM.SelectedIndex != 0 && comboBox_CO_CAM.SelectedIndex != 1)
                    {
                        return;
                    }
                    if (!LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam1.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM1.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam1.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
                {
                    if (comboBox_CO_CAM.SelectedIndex != 0 && comboBox_CO_CAM.SelectedIndex != 2)
                    {
                        return;
                    }
                    if (!LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam2.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM2.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam2.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
                {
                    if (comboBox_CO_CAM.SelectedIndex != 0 && comboBox_CO_CAM.SelectedIndex != 3)
                    {
                        return;
                    }
                    if (!LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam3.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM3.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam3.m_imageProvider.IsOpen, false);
                    }
                }
                else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
                {
                    if (comboBox_CO_CAM.SelectedIndex != 0 && comboBox_CO_CAM.SelectedIndex != 4)
                    {
                        return;
                    }
                    if (!LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen)
                    {
                        if (LVApp.Instance().m_mainform.ctrCam4.Open())
                        {
                            //LVApp.Instance().m_Ctr_Auto.ledBulb_CAM4.On = true;
                        }
                        EnableButtons(LVApp.Instance().m_mainform.ctrCam4.m_imageProvider.IsOpen, false);
                    }
                }
            }
            LVApp.Instance().m_mainform.Camera_Connection_Check();
        }

        private void button_Change_Cam_Name_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            try
            {
                m_SetCameraName = textBox_Camera_Name.Text;
                if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
                {
                    LVApp.Instance().m_mainform.ctrCam1.Camera_Name = m_SetCameraName;
                }
                else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
                {
                    LVApp.Instance().m_mainform.ctrCam2.Camera_Name = m_SetCameraName;
                }
                else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
                {
                    LVApp.Instance().m_mainform.ctrCam3.Camera_Name = m_SetCameraName;
                }
                else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
                {
                    LVApp.Instance().m_mainform.ctrCam4.Camera_Name = m_SetCameraName;
                }

                if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
                { // 중국 카메라
                    LVApp.Instance().m_GenICam.CAM[cam_num].Cam_Name = m_SetCameraName;
                }

                //toolStripButtonOneShot_Click(sender, e);
                toolStripButtonInitialize_Click(sender, e);
                //toolStripButton_SAVE_Click(sender, e);
            }
            catch (System.Exception ex)
            {
                textBox_Camera_Name.Text = m_SetCameraName;
                if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] <= 3)
                {
                    MessageBox.Show("Type the camera name like \"CAM0\"");
                }
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void button_Change_Resolution_Click(object sender, EventArgs e)
        {
            try
            {
                double[] cam_resol = new double[2];
                cam_resol[0] = Convert.ToDouble(textBox_RESOLUTION_X.Text);
                cam_resol[1] = Convert.ToDouble(textBox_RESOLUTION_Y.Text);
                int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
                LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Resolution(cam_resol[0], cam_resol[1], cam_num);
                //LVApp.Instance().m_mainform.m_ImProClr_Class1.Set_Resolution(cam_resol[0], cam_resol[1], cam_num);
                //LVApp.Instance().m_mainform.m_ImProClr_Class2.Set_Resolution(cam_resol[0], cam_resol[1], cam_num);

                m_SetResolution = cam_resol;
                //toolStripButtonInitialize_Click(sender, e);
                toolStripButton_SAVE_Click(sender, e);
            }
            catch (System.Exception ex)
            {
                textBox_RESOLUTION_X.Text = m_SetResolution[0].ToString("0.00000000");
                textBox_RESOLUTION_Y.Text = m_SetResolution[1].ToString("0.00000000");
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void toolStripButton_SAVE_Click(object sender, EventArgs e)
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
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[4];
                    int m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1))%4;
                    ctr_MIL_LINK1.Cam_Num = m_Cam_num;
                    worksheet.Cells[2 + 10 * m_Cam_num, 2].Value = m_Camera_Name;
                    if (LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] <= 3)
                    {
                        if (sliderGain.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[3 + 10 * m_Cam_num, 2].Value = sliderGain.labelCurrentValue.Text;
                        }
                        if (sliderExposureTime.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[4 + 10 * m_Cam_num, 2].Value = sliderExposureTime.labelCurrentValue.Text;
                        }
                        if (sliderWidth.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[5 + 10 * m_Cam_num, 2].Value = sliderWidth.labelCurrentValue.Text;
                        }
                        if (sliderHeight.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[6 + 10 * m_Cam_num, 2].Value = sliderHeight.labelCurrentValue.Text;
                        }
                        if (sliderOffsetX.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[7 + 10 * m_Cam_num, 2].Value = sliderOffsetX.labelCurrentValue.Text;
                        }
                        if (sliderOffsetY.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[8 + 10 * m_Cam_num, 2].Value = sliderOffsetY.labelCurrentValue.Text;
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] == 6)
                    { //중국 카메라
                        if (GeniCam_sliderGain.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[3 + 10 * m_Cam_num, 2].Value = GeniCam_sliderGain.labelCurrentValue.Text;
                        }
                        if (GeniCam_sliderExposureTime.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[4 + 10 * m_Cam_num, 2].Value = GeniCam_sliderExposureTime.labelCurrentValue.Text;
                        }
                        if (GeniCam_sliderWidth.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[5 + 10 * m_Cam_num, 2].Value = GeniCam_sliderWidth.labelCurrentValue.Text;
                        }
                        if (GeniCam_sliderHeight.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[6 + 10 * m_Cam_num, 2].Value = GeniCam_sliderHeight.labelCurrentValue.Text;
                        }
                        if (GeniCam_sliderOffsetX.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[7 + 10 * m_Cam_num, 2].Value = GeniCam_sliderOffsetX.labelCurrentValue.Text;
                        }
                        if (GeniCam_sliderOffsetY.labelCurrentValue.Text != "")
                        {
                            worksheet.Cells[8 + 10 * m_Cam_num, 2].Value = GeniCam_sliderOffsetY.labelCurrentValue.Text;
                        }
                    }
                    worksheet.Cells[9 + 10 * m_Cam_num, 2].Value = m_Resolution[0].ToString("0.00000000");
                    worksheet.Cells[9 + 10 * m_Cam_num, 3].Value = m_Resolution[1].ToString("0.00000000");
                    worksheet.Cells[10 + 10 * m_Cam_num, 2].Value = checkBox_LR.Checked == false ? "0" : "1";
                    worksheet.Cells[10 + 10 * m_Cam_num, 3].Value = checkBox_TB.Checked == false ? "0" : "1";
                    worksheet.Cells[11 + 10 * m_Cam_num, 2].Value = comboBox_Change_Rotation.SelectedIndex;
                    worksheet.Cells[11 + 10 * m_Cam_num, 3].Value = Force_USE.Checked == false ? "0" : "1";
                    worksheet.Cells[11 + 10 * m_Cam_num, 4].Value = textBox_TRIGGER_DELAY.Text;
                    worksheet.Cells[11 + 10 * m_Cam_num, 5].Value = comboBox_CAMKIND.SelectedIndex;
                    worksheet.Cells[11 + 10 * m_Cam_num, 6].Value = LVApp.Instance().m_Config.m_Interlock_Cam[0];
                    worksheet.Cells[11 + 10 * m_Cam_num, 7].Value = LVApp.Instance().m_Config.m_Interlock_Cam[1];
                    worksheet.Cells[11 + 10 * m_Cam_num, 8].Value = LVApp.Instance().m_Config.m_Interlock_Cam[2];
                    worksheet.Cells[11 + 10 * m_Cam_num, 9].Value = LVApp.Instance().m_Config.m_Interlock_Cam[3];

                    if (comboBoxPixelFormat.comboBox.SelectedIndex >= 0)
                    {
                        worksheet.Cells[11 + 10 * m_Cam_num, 10].Value = comboBoxPixelFormat.comboBox.SelectedIndex;
                    }
                    worksheet.Cells[11 + 10 * m_Cam_num, 11].Value = checkBox_Merge.Checked == false ? "0" : "1";
                    worksheet.Cells[11 + 10 * m_Cam_num, 12].Value = textBox_Merge.Text;

                    package.Save();
                    Add_Message(m_Camera_Name + " Setting Saved.");
                    DebugLogger.Instance().LogRecord(m_Camera_Name + " saved.");
                }

                int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
                if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
                {
                    ctr_MIL_LINK1.SAVE();
                }
            }
            catch
            {

            }
        }

        public void toolStripButton_LOAD_Click(object sender, EventArgs e)
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
                DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name);
                // 폴더가 존재하지 않으면
                if (dir.Exists == false)
                {
                    return;
                }

                FileInfo newFile = new FileInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + LVApp.Instance().m_Config.m_Model_Name + ".xlsx");
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // Add a worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[4];
                    int m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1)) % 4;
                    m_Camera_Name = worksheet.Cells[2 + 10 * m_Cam_num, 2].Value.ToString();
                    m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1)) % 4;
                    ctr_MIL_LINK1.Cam_Num = m_Cam_num;
                    if (worksheet.Cells[11 + 10 * m_Cam_num, 5].Value != null)
                    {
                        comboBox_CAMKIND.SelectedIndex = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 5].Value);
                    }
                    else
                    {
                        comboBox_CAMKIND.SelectedIndex = 0;
                    }
                    button_CAMKIND_Apply_Click(sender, e);

                    if (worksheet.Cells[11 + 10 * m_Cam_num, 3].Value != null)
                    {
                        Force_USE.Checked = worksheet.Cells[11 + 10 * m_Cam_num, 3].Value.ToString() == "0" ? false : true;
                    }
                    else
                    {
                        Force_USE.Checked = false;
                    }
                    textBox_Camera_Name.Text = m_Camera_Name;
                    button_Change_Cam_Name_Click(sender, e);

                    if (LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] <= 3)
                    {
                        //if (toolStripButtonOneShot.Enabled)
                        {
                            if (sliderOffsetX.slider.Enabled)
                            {
                                sliderOffsetX.slider.Value = 0;
                                sliderOffsetX.slider_Scroll(sender, e);
                            }
                            if (sliderOffsetY.slider.Enabled)
                            {
                                sliderOffsetY.slider.Value = 0;
                                sliderOffsetY.slider_Scroll(sender, e);

                            }
                            int t_v = 0;
                            if (sliderGain.slider.Enabled)
                            {
                                sliderGain.labelCurrentValue.Text = worksheet.Cells[3 + 10 * m_Cam_num, 2].Value.ToString();
                                int.TryParse(sliderGain.labelCurrentValue.Text, out t_v);
                                if (t_v < sliderGain.slider.Minimum)
                                {
                                    t_v = sliderGain.slider.Minimum;
                                }
                                else if (t_v > sliderGain.slider.Maximum)
                                {
                                    t_v = sliderGain.slider.Maximum;
                                }
                                sliderGain.slider.Value = t_v;
                                sliderGain.slider_Scroll(sender, e);
                            }
                            if (sliderExposureTime.slider.Enabled)
                            {
                                sliderExposureTime.labelCurrentValue.Text = worksheet.Cells[4 + 10 * m_Cam_num, 2].Value.ToString();
                                t_v = 0;
                                int.TryParse(sliderExposureTime.labelCurrentValue.Text, out t_v);
                                if (t_v < sliderExposureTime.slider.Minimum)
                                {
                                    t_v = sliderExposureTime.slider.Minimum;
                                }
                                else if (t_v > sliderExposureTime.slider.Maximum)
                                {
                                    t_v = sliderExposureTime.slider.Maximum;
                                }
                                sliderExposureTime.slider.Value = t_v;
                                sliderExposureTime.slider_Scroll(sender, e);
                            }
                            if (sliderWidth.Enabled == true)
                            {
                                sliderWidth.labelCurrentValue.Text = worksheet.Cells[5 + 10 * m_Cam_num, 2].Value.ToString();
                                int.TryParse(sliderWidth.labelCurrentValue.Text, out t_v);
                                if (t_v < sliderWidth.slider.Minimum)
                                {
                                    t_v = sliderWidth.slider.Minimum;
                                }
                                else if (t_v > sliderWidth.slider.Maximum)
                                {
                                    t_v = sliderWidth.slider.Maximum;
                                }
                                sliderWidth.slider.Value = t_v;
                                sliderWidth.slider_Scroll(sender, e);

                                sliderHeight.labelCurrentValue.Text = worksheet.Cells[6 + 10 * m_Cam_num, 2].Value.ToString();
                                int.TryParse(sliderHeight.labelCurrentValue.Text, out t_v);
                                if (t_v < sliderHeight.slider.Minimum)
                                {
                                    t_v = sliderHeight.slider.Minimum;
                                }
                                else if (t_v > sliderHeight.slider.Maximum)
                                {
                                    t_v = sliderHeight.slider.Maximum;
                                }
                                sliderHeight.slider.Value = t_v;
                                sliderHeight.slider_Scroll(sender, e);
                            }

                            if (sliderOffsetX.slider.Enabled)
                            {
                                sliderOffsetX.labelCurrentValue.Text = worksheet.Cells[7 + 10 * m_Cam_num, 2].Value.ToString();
                                int.TryParse(sliderOffsetX.labelCurrentValue.Text, out t_v);
                                if (t_v < sliderOffsetX.slider.Minimum)
                                {
                                    t_v = sliderOffsetX.slider.Minimum;
                                }
                                else if (t_v > sliderOffsetX.slider.Maximum)
                                {
                                    t_v = sliderOffsetX.slider.Maximum;
                                }
                                sliderOffsetX.slider.Value = t_v;
                                sliderOffsetX.slider_Scroll(sender, e);
                            }
                            if (sliderOffsetY.slider.Enabled)
                            {
                                sliderOffsetY.labelCurrentValue.Text = worksheet.Cells[8 + 10 * m_Cam_num, 2].Value.ToString();
                                int.TryParse(sliderOffsetY.labelCurrentValue.Text, out t_v);
                                if (t_v < sliderOffsetY.slider.Minimum)
                                {
                                    t_v = sliderOffsetY.slider.Minimum;
                                }
                                else if (t_v > sliderOffsetY.slider.Maximum)
                                {
                                    t_v = sliderOffsetY.slider.Maximum;
                                }
                                sliderOffsetY.slider.Value = t_v;
                                sliderOffsetY.slider_Scroll(sender, e);
                            }
                            if (worksheet.Cells[11 + 10 * m_Cam_num, 10].Value != null)
                            {
                                int.TryParse(worksheet.Cells[11 + 10 * m_Cam_num, 10].Value.ToString(), out t_v);
                                if (comboBoxPixelFormat.comboBox.Items.Count > t_v && t_v >= 0 && comboBoxPixelFormat.comboBox.Enabled)
                                {
                                    comboBoxPixelFormat.comboBox.SelectedIndex = t_v;
                                }
                            }
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] == 6)
                    { //중국 카메라
                        int t_v = 0;
                        if (GeniCam_sliderOffsetX.slider.Enabled)
                        {
                            GeniCam_sliderOffsetX.slider.Value = 0;
                            GeniCam_sliderOffsetX.slider_Scroll(sender, e);
                        }
                        if (GeniCam_sliderOffsetY.slider.Enabled)
                        {
                            GeniCam_sliderOffsetY.slider.Value = 0;
                            GeniCam_sliderOffsetY.slider_Scroll(sender, e);
                        }
                        if (GeniCam_sliderGain.slider.Enabled)
                        {
                            GeniCam_sliderGain.labelCurrentValue.Text = worksheet.Cells[3 + 10 * m_Cam_num, 2].Value.ToString();
                            int.TryParse(GeniCam_sliderGain.labelCurrentValue.Text, out t_v);
                            if (t_v < GeniCam_sliderGain.slider.Minimum)
                            {
                                t_v = GeniCam_sliderGain.slider.Minimum;
                            }
                            else if (t_v > GeniCam_sliderGain.slider.Maximum)
                            {
                                t_v = GeniCam_sliderGain.slider.Maximum;
                            }
                            GeniCam_sliderGain.slider.Value = t_v;
                            GeniCam_sliderGain.slider_Scroll(sender, e);
                        }
                        if (GeniCam_sliderExposureTime.slider.Enabled)
                        {
                            GeniCam_sliderExposureTime.labelCurrentValue.Text = worksheet.Cells[4 + 10 * m_Cam_num, 2].Value.ToString();
                            t_v = 0;
                            int.TryParse(GeniCam_sliderExposureTime.labelCurrentValue.Text, out t_v);
                            if (t_v < GeniCam_sliderExposureTime.slider.Minimum)
                            {
                                t_v = GeniCam_sliderExposureTime.slider.Minimum;
                            }
                            else if (t_v > GeniCam_sliderExposureTime.slider.Maximum)
                            {
                                t_v = GeniCam_sliderExposureTime.slider.Maximum;
                            }
                            GeniCam_sliderExposureTime.slider.Value = t_v;
                            GeniCam_sliderExposureTime.slider_Scroll(sender, e);
                        }
                        if (GeniCam_sliderWidth.Enabled == true)
                        {
                            GeniCam_sliderWidth.labelCurrentValue.Text = worksheet.Cells[5 + 10 * m_Cam_num, 2].Value.ToString();
                            int.TryParse(GeniCam_sliderWidth.labelCurrentValue.Text, out t_v);
                            if (t_v < GeniCam_sliderWidth.slider.Minimum)
                            {
                                t_v = GeniCam_sliderWidth.slider.Minimum;
                            }
                            else if (t_v > GeniCam_sliderWidth.slider.Maximum)
                            {
                                t_v = GeniCam_sliderWidth.slider.Maximum;
                            }
                            GeniCam_sliderWidth.slider.Value = t_v;
                            GeniCam_sliderWidth.slider_Scroll(sender, e);

                            GeniCam_sliderHeight.labelCurrentValue.Text = worksheet.Cells[6 + 10 * m_Cam_num, 2].Value.ToString();
                            int.TryParse(GeniCam_sliderHeight.labelCurrentValue.Text, out t_v);
                            if (t_v < GeniCam_sliderHeight.slider.Minimum)
                            {
                                t_v = GeniCam_sliderHeight.slider.Minimum;
                            }
                            else if (t_v > GeniCam_sliderHeight.slider.Maximum)
                            {
                                t_v = GeniCam_sliderHeight.slider.Maximum;
                            }
                            GeniCam_sliderHeight.slider.Value = t_v;
                            GeniCam_sliderHeight.slider_Scroll(sender, e);
                        }

                        if (GeniCam_sliderOffsetX.slider.Enabled)
                        {
                            GeniCam_sliderOffsetX.labelCurrentValue.Text = worksheet.Cells[7 + 10 * m_Cam_num, 2].Value.ToString();
                            int.TryParse(GeniCam_sliderOffsetX.labelCurrentValue.Text, out t_v);
                            if (t_v < GeniCam_sliderOffsetX.slider.Minimum)
                            {
                                t_v = GeniCam_sliderOffsetX.slider.Minimum;
                            }
                            else if (t_v > GeniCam_sliderOffsetX.slider.Maximum)
                            {
                                t_v = GeniCam_sliderOffsetX.slider.Maximum;
                            }
                            GeniCam_sliderOffsetX.slider.Value = t_v;
                            GeniCam_sliderOffsetX.slider_Scroll(sender, e);
                        }
                        if (GeniCam_sliderOffsetY.slider.Enabled)
                        {
                            GeniCam_sliderOffsetY.labelCurrentValue.Text = worksheet.Cells[8 + 10 * m_Cam_num, 2].Value.ToString();
                            int.TryParse(GeniCam_sliderOffsetY.labelCurrentValue.Text, out t_v);
                            if (t_v < GeniCam_sliderOffsetY.slider.Minimum)
                            {
                                t_v = GeniCam_sliderOffsetY.slider.Minimum;
                            }
                            else if (t_v > GeniCam_sliderOffsetY.slider.Maximum)
                            {
                                t_v = GeniCam_sliderOffsetY.slider.Maximum;
                            }
                            GeniCam_sliderOffsetY.slider.Value = t_v;
                            GeniCam_sliderOffsetY.slider_Scroll(sender, e);
                        }
                        //if (worksheet.Cells[11 + 10 * m_Cam_num, 10].Value != null)
                        //{
                        //    int.TryParse(worksheet.Cells[11 + 10 * m_Cam_num, 10].Value.ToString(), out t_v);
                        //    if (comboBoxPixelFormat.comboBox.Items.Count > t_v && t_v >= 0 && comboBoxPixelFormat.comboBox.Enabled)
                        //    {
                        //        comboBoxPixelFormat.comboBox.SelectedIndex = t_v;
                        //    }
                        //}
                    }

                    double[] t_res = new double[2];
                    if (worksheet.Cells[9 + 10 * m_Cam_num, 3].Value == null)
                    {
                        t_res[0] = 1;
                        t_res[1] = 1;
                    } else
                    {
                        t_res[0] = Convert.ToDouble(worksheet.Cells[9 + 10 * m_Cam_num, 2].Value.ToString());
                        t_res[1] = Convert.ToDouble(worksheet.Cells[9 + 10 * m_Cam_num, 3].Value.ToString());
                    }
                    m_SetResolution = t_res;
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Resolution(t_res[0], t_res[1], m_Cam_num);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class1.Set_Resolution(t_res[0], t_res[1], m_Cam_num);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class2.Set_Resolution(t_res[0], t_res[1], m_Cam_num);

                    if (worksheet.Cells[10 + 10 * m_Cam_num, 2].Value != null && worksheet.Cells[10 + 10 * m_Cam_num, 3].Value != null)
                    {
                        checkBox_LR.Checked = worksheet.Cells[10 + 10 * m_Cam_num, 2].Value.ToString() == "0" ? false : true;
                        checkBox_TB.Checked = worksheet.Cells[10 + 10 * m_Cam_num, 3].Value.ToString() == "0" ? false : true;
                    }
                    else
                    {
                        checkBox_LR.Checked = false;
                        checkBox_TB.Checked = false;
                    }

                    if (worksheet.Cells[11 + 10 * m_Cam_num, 2].Value != null)
                    {
                        comboBox_Change_Rotation.SelectedIndex = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 2].Value);
                    }
                    else
                    {
                        comboBox_Change_Rotation.SelectedIndex = 0;
                    }

                    if (worksheet.Cells[11 + 10 * m_Cam_num, 3].Value != null)
                    {
                        Force_USE.Checked = worksheet.Cells[11 + 10 * m_Cam_num, 3].Value.ToString() == "0" ? false : true;
                    }
                    else
                    {
                        Force_USE.Checked = false;
                    }
                    LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[m_Cam_num] = Force_USE.Checked;

                    if (worksheet.Cells[11 + 10 * m_Cam_num, 4].Value != null)
                    {
                        textBox_TRIGGER_DELAY.Text = worksheet.Cells[11 + 10 * m_Cam_num, 4].Value.ToString();
                    }
                    else
                    {
                        textBox_TRIGGER_DELAY.Text = "0";
                    }
                    int.TryParse(textBox_TRIGGER_DELAY.Text, out LVApp.Instance().m_Config.Inspection_Delay[m_Cam_num]);

                    if (worksheet.Cells[11 + 10 * m_Cam_num, 5].Value != null)
                    {
                        comboBox_CAMKIND.SelectedIndex = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 5].Value);
                    }
                    else
                    {
                        comboBox_CAMKIND.SelectedIndex = 0;
                    }

                    LVApp.Instance().m_Config.m_Cam_Filp[m_Cam_num, 0] = checkBox_LR.Checked;
                    LVApp.Instance().m_Config.m_Cam_Filp[m_Cam_num, 1] = checkBox_TB.Checked;
                    LVApp.Instance().m_Config.m_Cam_Rot[m_Cam_num] = comboBox_Change_Rotation.SelectedIndex;
                    LVApp.Instance().m_Config.m_Cam_Kind[m_Cam_num] = comboBox_CAMKIND.SelectedIndex;


                    if (worksheet.Cells[11 + 10 * m_Cam_num, 6].Value != null && worksheet.Cells[11 + 10 * m_Cam_num, 9].Value != null)
                    {
                        LVApp.Instance().m_Config.m_Interlock_Cam[0] = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 6].Value);
                        LVApp.Instance().m_Config.m_Interlock_Cam[1] = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 7].Value);
                        LVApp.Instance().m_Config.m_Interlock_Cam[2] = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 8].Value);
                        LVApp.Instance().m_Config.m_Interlock_Cam[3] = Convert.ToInt32(worksheet.Cells[11 + 10 * m_Cam_num, 9].Value);
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.comboBox_CO_CAM.SelectedIndex = LVApp.Instance().m_Config.m_Interlock_Cam[0] + 1;
                        LVApp.Instance().m_mainform.ctr_Camera_Setting2.comboBox_CO_CAM.SelectedIndex = LVApp.Instance().m_Config.m_Interlock_Cam[1] + 1;
                        LVApp.Instance().m_mainform.ctr_Camera_Setting3.comboBox_CO_CAM.SelectedIndex = LVApp.Instance().m_Config.m_Interlock_Cam[2] + 1;
                        LVApp.Instance().m_mainform.ctr_Camera_Setting4.comboBox_CO_CAM.SelectedIndex = LVApp.Instance().m_Config.m_Interlock_Cam[3] + 1;
                    }
                    else
                    {
                        LVApp.Instance().m_Config.m_Interlock_Cam[0] = LVApp.Instance().m_Config.m_Interlock_Cam[1] = LVApp.Instance().m_Config.m_Interlock_Cam[2]
                            = LVApp.Instance().m_Config.m_Interlock_Cam[3] = -1;
                        LVApp.Instance().m_mainform.ctr_Camera_Setting1.comboBox_CO_CAM.SelectedIndex =
                            LVApp.Instance().m_mainform.ctr_Camera_Setting2.comboBox_CO_CAM.SelectedIndex =
                            LVApp.Instance().m_mainform.ctr_Camera_Setting3.comboBox_CO_CAM.SelectedIndex =
                            LVApp.Instance().m_mainform.ctr_Camera_Setting4.comboBox_CO_CAM.SelectedIndex = 0;
                    }

                    if (worksheet.Cells[11 + 10 * m_Cam_num, 11].Value != null && worksheet.Cells[11 + 10 * m_Cam_num, 12].Value != null)
                    {
                        checkBox_Merge.Checked = worksheet.Cells[11 + 10 * m_Cam_num, 11].Value.ToString() == "0" ? false : true;
                        textBox_Merge.Text = worksheet.Cells[11 + 10 * m_Cam_num, 12].Value.ToString();

                        LVApp.Instance().m_Config.Image_Merge_Check[m_Cam_num] = checkBox_Merge.Checked;
                        int _v = 0;
                        int.TryParse(textBox_Merge.Text, out _v);
                        if (_v > 0)
                        {
                            LVApp.Instance().m_Config.Image_Merge_Number[m_Cam_num] = _v;
                        }
                    }
                    else
                    {
                        LVApp.Instance().m_Config.Image_Merge_Check[m_Cam_num] = checkBox_Merge.Checked = false;
                        LVApp.Instance().m_Config.Image_Merge_Number[m_Cam_num] = 0; textBox_Merge.Text = "0";
                    }

                    Add_Message(m_Camera_Name + " Setting Loaded");
                    DebugLogger.Instance().LogRecord(m_Camera_Name + " Setting loaded.");
                }

                int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
                if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
                {
                    ctr_MIL_LINK1.LOAD();
                }

            }
            finally
            {
            }
        }

        private void button_Change_Flip_Click(object sender, EventArgs e)
        {
            try
            {
                int m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1))%4;
                LVApp.Instance().m_Config.m_Cam_Filp[m_Cam_num, 0] = checkBox_LR.Checked;
                LVApp.Instance().m_Config.m_Cam_Filp[m_Cam_num, 1] = checkBox_TB.Checked;
                toolStripButton_SAVE_Click(sender, e);
                Add_Message("Applied");
            }
            catch// (System.Exception ex)
            {
                int m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1))%4;
                LVApp.Instance().m_Config.m_Cam_Filp[m_Cam_num, 0] = false;
                LVApp.Instance().m_Config.m_Cam_Filp[m_Cam_num, 1] = false;
                Add_Message("Error!");
            }
        }

        private void button_Change_Rotation_Click(object sender, EventArgs e)
        {
            // 회전없음
            // 90도 회전
            // 180도 회전
            // 270도 회전
            try
            {
                int m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1))%4;
                LVApp.Instance().m_Config.m_Cam_Rot[m_Cam_num] = comboBox_Change_Rotation.SelectedIndex;
                toolStripButton_SAVE_Click(sender, e);
                Add_Message("Applied");
            }
            catch// (System.Exception ex)
            {
                Add_Message("Error!");
            }
        }

        public void Add_Message(String msg)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    if (richTextBox1.Lines.Length > 1000)
                    {
                        richTextBox1.ResetText();
                    }
                    string display_str = msg + "\n" + richTextBox1.Text;
                    richTextBox1.Text = display_str;
                });
            }
            else
            {
                if (richTextBox1.Lines.Length > 1000)
                {
                    richTextBox1.ResetText();
                }
                string display_str = msg + "\n" + richTextBox1.Text;
                richTextBox1.Text = display_str;
            }
        }

        public void button_TRIGGER_DELAY_CHANGE_Click(object sender, EventArgs e)
        {
            try
            {
                int m_Cam_num = Convert.ToInt32(m_Camera_Name.Substring(3, 1))%4;
                int.TryParse(textBox_TRIGGER_DELAY.Text, out LVApp.Instance().m_Config.Inspection_Delay[m_Cam_num]);
                //double t_delay = double.Parse(textBox_TRIGGER_DELAY.Text);
                //LVApp.Instance().m_mainform.ctr_PLC1.PLC_D_WRITE("DW5" + LVApp.Instance().m_mainform.ctr_PLC1.m_Pingpong_Num.ToString("0") + "9" + textBox_Camera_Name.Text.Substring(3, 1), 1, t_delay);
                toolStripButton_SAVE_Click(sender, e);
                Add_Message("Applied");
            }
            catch// (System.Exception ex)
            {
                Add_Message("Error!");
            }
        }

        public void Force_USE_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
                LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[cam_num] = Force_USE.Checked;
                m_SetCameraName = textBox_Camera_Name.Text;

                if (Force_USE.Checked)
                {
                    comboBox_CO_CAM.SelectedIndex = 0;
                    button_Change_COCAM_Click(sender, e);
                }
                else
                {
                    toolStripButtonStop_Click(sender, e);
                    toolStripButtonDisconnect_Click(sender, e);
                }
                //if (m_Camera_Name.Substring(3, 1) == "0")
                //{
                //    //LVApp.Instance().m_mainform.ctrCam1.Camera_Name = m_SetCameraName;
                //}
                //else if (m_Camera_Name.Substring(3, 1) == "1")
                //{
                //    if (Force_USE.Checked && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2].Checked)
                //    {
                //        MessageBox.Show("CAM2 enabled!");
                //        Force_USE.Checked = false;
                //    }
                //}
                //else if (m_Camera_Name.Substring(3, 1) == "2")
                //{
                //    if (!Force_USE.Checked && LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1].Checked)
                //    {
                //        MessageBox.Show("CAM1 dissabled!");
                //        Force_USE.Checked = true;
                //    }
                //}
                LVApp.Instance().m_mainform.ctr_Display_1.Update_Display();
            }
            catch
            { }
        }

        private void button_CAMKIND_Apply_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] != comboBox_CAMKIND.SelectedIndex)
            {
                LVApp.Instance().m_Config.m_Cam_Kind[cam_num] = comboBox_CAMKIND.SelectedIndex;
                if (cam_num == 0)
                {
                    if (comboBox_CAMKIND.SelectedIndex != 1)
                    {
                        LVApp.Instance().m_mainform.ctrCam1.Camera_Line_Mode = false;
                    }
                    else
                    {
                        LVApp.Instance().m_mainform.ctrCam1.Camera_Line_Mode = true;
                    }
                }
                else if (cam_num == 1)
                {
                    if (comboBox_CAMKIND.SelectedIndex != 1)
                    {
                        LVApp.Instance().m_mainform.ctrCam2.Camera_Line_Mode = false;
                    }
                    else
                    {
                        LVApp.Instance().m_mainform.ctrCam2.Camera_Line_Mode = true;
                    }
                }
                else if (cam_num == 2)
                {
                    if (comboBox_CAMKIND.SelectedIndex != 1)
                    {
                        LVApp.Instance().m_mainform.ctrCam3.Camera_Line_Mode = false;
                    }
                    else
                    {
                        LVApp.Instance().m_mainform.ctrCam3.Camera_Line_Mode = true;
                    }
                }
                else if (cam_num == 3)
                {
                    if (comboBox_CAMKIND.SelectedIndex != 1)
                    {
                        LVApp.Instance().m_mainform.ctrCam4.Camera_Line_Mode = false;
                    }
                    else
                    {
                        LVApp.Instance().m_mainform.ctrCam4.Camera_Line_Mode = true;
                    }
                }
                sliderWidth.Visible =
                sliderHeight.Visible =
                sliderOffsetX.Visible =
                sliderOffsetY.Visible =
                sliderGain.Visible =
                sliderExposureTime.Visible =
                comboBoxPixelFormat.Visible = true;
                GeniCam_sliderWidth.Visible =
                GeniCam_sliderHeight.Visible =
                GeniCam_sliderOffsetX.Visible =
                GeniCam_sliderOffsetY.Visible =
                GeniCam_sliderGain.Visible =
                GeniCam_sliderExposureTime.Visible = false;
                //comboBoxPixelFormat.Visible = true;
            }

            if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 4)
            {
                sliderWidth.Visible =
                sliderHeight.Visible =
                sliderOffsetX.Visible =
                sliderOffsetY.Visible =
                sliderGain.Visible =
                sliderExposureTime.Visible =
                comboBoxPixelFormat.Visible = false;

                GeniCam_sliderWidth.Visible =
                GeniCam_sliderHeight.Visible =
                GeniCam_sliderOffsetX.Visible =
                GeniCam_sliderOffsetY.Visible =
                GeniCam_sliderGain.Visible =
                GeniCam_sliderExposureTime.Visible = false;

                if (cam_num == 0)
                {
                    ctr_MIL_LINK1.Visible = true;
                    ctr_MIL_LINK1.Cam_Num = 0;
                }
                else if (cam_num == 1)
                {
                    ctr_MIL_LINK1.Visible = true;
                    ctr_MIL_LINK1.Cam_Num = 1;
                }
                else if (cam_num == 2)
                {
                    ctr_MIL_LINK1.Visible = true;
                    ctr_MIL_LINK1.Cam_Num = 2;
                }
                else if (cam_num == 3)
                {
                    ctr_MIL_LINK1.Visible = true;
                    ctr_MIL_LINK1.Cam_Num = 3;
                }
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 5 || LVApp.Instance().m_Config.m_Cam_Kind[cam_num] == 6)
            { // 중국 카메라
                ctr_MIL_LINK1.Visible = false;
                sliderWidth.Visible =
                sliderHeight.Visible =
                sliderOffsetX.Visible =
                sliderOffsetY.Visible =
                sliderGain.Visible =
                sliderExposureTime.Visible =
                comboBoxPixelFormat.Visible = false;
                GeniCam_sliderWidth.Visible =
                GeniCam_sliderHeight.Visible =
                GeniCam_sliderOffsetX.Visible =
                GeniCam_sliderOffsetY.Visible =
                GeniCam_sliderGain.Visible =
                GeniCam_sliderExposureTime.Visible = true;
            }
            else
            {
                ctr_MIL_LINK1.Visible = false;
            }
        }

        private void toolStripButtonImageSave_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = null;
                if ((m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4") && LVApp.Instance().m_mainform.pictureBox_CAM0.Image != null)
                {
                    bmp = (Bitmap)LVApp.Instance().m_mainform.pictureBox_CAM0.Image.Clone();
                }
                else if ((m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5") && LVApp.Instance().m_mainform.pictureBox_CAM1.Image != null)
                {
                    bmp = (Bitmap)LVApp.Instance().m_mainform.pictureBox_CAM1.Image.Clone();
                }
                else if ((m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6") && LVApp.Instance().m_mainform.pictureBox_CAM2.Image != null)
                {
                    bmp = (Bitmap)LVApp.Instance().m_mainform.pictureBox_CAM2.Image.Clone();
                }
                else if ((m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7") && LVApp.Instance().m_mainform.pictureBox_CAM3.Image != null)
                {
                    bmp = (Bitmap)LVApp.Instance().m_mainform.pictureBox_CAM3.Image.Clone();
                }
                if (bmp == null)
                {
                    AutoClosingMessageBox.Show("저장할 이미지가 없습니다.", "Notice", 1500);
                    return;
                }
                Bitmap t_bmp = null;
                if (bmp.PixelFormat == PixelFormat.Format32bppRgb || bmp.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    t_bmp = ((Bitmap)bmp).Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);
                }

                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                SaveFileDialog1.InitialDirectory = LVApp.Instance().excute_path;
                SaveFileDialog1.RestoreDirectory = true;
                SaveFileDialog1.Title = "이미지 저장";
                SaveFileDialog1.Filter = "All image files|*.jpg;*.bmp;*.png";
                SaveFileDialog1.FilterIndex = 2;
                SaveFileDialog1.FileName = "Save_" + m_Camera_Name.Substring(3, 1) + ".bmp";
                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "bmp"
                        || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "Bmp"
                        || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "BMP")
                    {
                        t_bmp.Save(SaveFileDialog1.FileName, ImageFormat.Bmp);
                    }
                    else if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "jpg"
                      || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "Jpg"
                      || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "JPG")
                    {
                        t_bmp.Save(SaveFileDialog1.FileName, ImageFormat.Jpeg);
                    }
                    else if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "png"
                      || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "Png"
                      || SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)) == "PNG")
                    {
                        t_bmp.Save(SaveFileDialog1.FileName, ImageFormat.Png);
                    }
                }

                t_bmp.Dispose();
                bmp.Dispose();
            }
            catch (System.Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void button_Change_COCAM_Click(object sender, EventArgs e)
        {
            if (m_Camera_Name.Substring(3, 1) == "0" || m_Camera_Name.Substring(3, 1) == "4")
            {
                LVApp.Instance().m_Config.m_Interlock_Cam[0] = comboBox_CO_CAM.SelectedIndex - 1;
            }
            else if (m_Camera_Name.Substring(3, 1) == "1" || m_Camera_Name.Substring(3, 1) == "5")
            {
                LVApp.Instance().m_Config.m_Interlock_Cam[1] = comboBox_CO_CAM.SelectedIndex - 1;
            }
            else if (m_Camera_Name.Substring(3, 1) == "2" || m_Camera_Name.Substring(3, 1) == "6")
            {
                LVApp.Instance().m_Config.m_Interlock_Cam[2] = comboBox_CO_CAM.SelectedIndex - 1;
            }
            else if (m_Camera_Name.Substring(3, 1) == "3" || m_Camera_Name.Substring(3, 1) == "7")
            {
                LVApp.Instance().m_Config.m_Interlock_Cam[3] = comboBox_CO_CAM.SelectedIndex - 1;
            }
        }

        private void comboBox_CO_CAM_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_Change_COCAM_Click(sender, e);
        }

        private void checkBox_Merge_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button_Merge_Apply_Click(object sender, EventArgs e)
        {
            int cam_num = Convert.ToInt32(textBox_Camera_Name.Text.Substring(3, 1)) % 4;
            LVApp.Instance().m_Config.Image_Merge_Check[cam_num] = checkBox_Merge.Checked;
            int _v = 0;
            int.TryParse(textBox_Merge.Text, out _v);
            if (_v > 0)
            {
                LVApp.Instance().m_Config.Image_Merge_Number[cam_num] = _v;
            }
        }
    }
}
