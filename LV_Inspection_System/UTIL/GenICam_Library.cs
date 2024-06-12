using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThridLibray;

namespace LV_Inspection_System.UTIL
{
    public class GenICam_Library
    {
        public struct Cam_Param_Struct
        {
            public ThridLibray.IDevice Dev;
            public bool Use;
            public string Cam_Name;
            public int Width;
            public int Height;
            public int X_Offset;
            public int Y_Offset;
            public string ImagePixelFormat;
            public bool Connection;
            public List<string> ImagePixelFormat_List;
            public bool SharpnessAuto;
            public double ExposureTime;
            public bool Line1Trigger;
            public double GainRaw;
        }
        public Cam_Param_Struct[] CAM = new Cam_Param_Struct[4];

        public void Param_Initialize()
        {
            for (int i = 0; i < 4; i++)
            {
                CAM[i].Cam_Name = "CAM" + i.ToString();
                CAM[i].Width = -1;
                CAM[i].Height = -1;
                CAM[i].X_Offset = 0;
                CAM[i].Y_Offset = 0;
                CAM[i].ImagePixelFormat = "Mono8";
                CAM[i].Connection = false;
                CAM[i].ExposureTime = 1000;
                CAM[i].Line1Trigger = false;
                CAM[i].GainRaw = 1.0;
                CAM[i].ImagePixelFormat_List = new List<string>();
            }
        }

        public void Trigger_Mode(int Cam_Num)
        {
            try
            {
                if (CAM[Cam_Num].Dev == null)
                {
                    return;
                }
                if (CAM[Cam_Num].Line1Trigger)
                {
                    var t_list = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerSource].GetAllValues();
                    foreach (string t_src in t_list)
                    {
                        if (t_src.ToUpper() == "LINE0")
                        {
                            CAM[Cam_Num].Dev.TriggerSet.Open("Line0");
                            break;
                        }
                        if (t_src.ToUpper() == "LINE1")
                        {
                            CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Line1);
                            break;
                        }
                        if (t_src.ToUpper() == "LINE2")
                        {
                            CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Line2);
                            break;
                        }
                    }
                    using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                    {
                        p.SetValue(TriggerModeEnum.On);
                    }
                    //CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Line1);
                }
                else
                {
                    using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                    {
                        p.SetValue(TriggerModeEnum.Off);
                    }
                    //CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Software);
                }
            }
            catch
            { }
        }

        public void Update_Parameter(int Cam_Num)
        {
            try
            {
                if (CAM[Cam_Num].Dev == null)
                {
                    return;
                }

                if (!CAM[Cam_Num].Connection)
                {
                    return;
                }

                CAM[Cam_Num].Dev.StreamGrabber.SetBufferCount(4);

                if (CAM[Cam_Num].Line1Trigger)
                {
                    var t_list = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerSource].GetAllValues();
                    foreach (string t_src in t_list)
                    {
                        if (t_src.ToUpper() == "LINE0")
                        {
                            CAM[Cam_Num].Dev.TriggerSet.Open("Line0");
                            break;
                        }
                        if (t_src.ToUpper() == "LINE1")
                        {
                            CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Line1);
                            break;
                        }
                        if (t_src.ToUpper() == "LINE2")
                        {
                            CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Line2);
                            break;
                        }
                    }
                    using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                    {
                        p.SetValue(TriggerModeEnum.On);
                    }
                }
                else
                {
                    using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                    {
                        p.SetValue(TriggerModeEnum.Off);
                    }
                    //CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Software);
                }

                //using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerSelector])
                //{
                //    p.SetValue(TriggerSelectorEnum.FrameStart);
                //}

                //CAM[Cam_Num].Dev.TriggerSet.Start();

                //// PixelFormat
                //using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[new EnumName("PixelFormat")])
                //{
                //    CAM[Cam_Num].ImagePixelFormat_List.Clear();
                //    CAM[Cam_Num].ImagePixelFormat_List = p.GetAllValues();
                //    string t_PixelFormat = p.GetValue();
                //    int t_Idx = -1;
                //    if (t_PixelFormat != CAM[Cam_Num].ImagePixelFormat)
                //    {
                //        if (CAM[Cam_Num].ImagePixelFormat_List.Count > 0)
                //        {
                //            for (int i = 0; i < CAM[Cam_Num].ImagePixelFormat_List.Count; i++)
                //            {
                //                if (CAM[Cam_Num].ImagePixelFormat_List[i] == CAM[Cam_Num].ImagePixelFormat)
                //                {
                //                    p.SetValue(CAM[Cam_Num].ImagePixelFormat);
                //                    t_Idx = i;
                //                    break;
                //                }
                //            }
                //        }
                //        if (t_Idx == -1)
                //        {
                //            p.SetValue("Mono8");
                //            CAM[Cam_Num].ImagePixelFormat = "Mono8";
                //        }
                //    }
                //}

                // set ExposureTime
                if (CAM[Cam_Num].ExposureTime < 0)
                {
                    CAM[Cam_Num].ExposureTime = 0;
                }
                if (CAM[Cam_Num].GainRaw < 0)
                {
                    CAM[Cam_Num].GainRaw = 0;
                }

                if (CAM[Cam_Num].Dev.DeviceInfo.Vendor.ToUpper() == "BASLER")
                {
                    IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[new IntegerName("ExposureTimeRaw")];
                    if (p == null)
                    {
                        p = CAM[Cam_Num].Dev.ParameterCollection[new IntegerName("ExposureTime")];
                    }

                    if (p != null)
                    {
                        if (CAM[Cam_Num].ExposureTime > p.GetMaximum())
                        {
                            CAM[Cam_Num].ExposureTime = p.GetMaximum();
                        }
                        if (CAM[Cam_Num].ExposureTime <= p.GetMinimum())
                        {
                            CAM[Cam_Num].ExposureTime = p.GetMinimum();
                        }
                        p.SetValue((long)CAM[Cam_Num].ExposureTime);
                    }

                    p = CAM[Cam_Num].Dev.ParameterCollection[new IntegerName("GainRaw")];

                    if (p != null)
                    {
                        if (CAM[Cam_Num].GainRaw > p.GetMaximum())
                        {
                            CAM[Cam_Num].GainRaw = p.GetMaximum();
                        }
                        if (CAM[Cam_Num].GainRaw <= p.GetMinimum())
                        {
                            CAM[Cam_Num].GainRaw = p.GetMinimum();
                        }
                        p.SetValue((long)CAM[Cam_Num].GainRaw);
                    }
                }
                else
                {
                    using (IFloatParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
                    {
                        //CAM[Cam_Num].ExposureTime = 1000;
                        if (CAM[Cam_Num].ExposureTime > p.GetMaximum())
                        {
                            CAM[Cam_Num].ExposureTime = p.GetMaximum();
                        }
                        if (CAM[Cam_Num].ExposureTime <= p.GetMinimum())
                        {
                            CAM[Cam_Num].ExposureTime = p.GetMinimum();
                        }
                        p.SetValue((long)CAM[Cam_Num].ExposureTime);
                    }
                    using (IFloatParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.GainRaw])
                    {
                        // CAM[Cam_Num].GainRaw = 1.0;
                        if (CAM[Cam_Num].GainRaw > p.GetMaximum())
                        {
                            CAM[Cam_Num].GainRaw = p.GetMaximum();
                        }
                        if (CAM[Cam_Num].GainRaw <= p.GetMinimum())
                        {
                            CAM[Cam_Num].GainRaw = p.GetMinimum();
                        }
                        p.SetValue((long)CAM[Cam_Num].GainRaw);
                    }
                }

                using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetX])
                {
                    p.SetValue(0);
                }
                using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetY])
                {
                    p.SetValue(0);
                }

                // set Width
                if (CAM[Cam_Num].Width >= 0)
                {
                    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageWidth])
                    {
                        CAM[Cam_Num].Width -= CAM[Cam_Num].Width % 4;
                        if (CAM[Cam_Num].Width > (int)p.GetMaximum())
                        {
                            CAM[Cam_Num].Width = (int)p.GetMaximum();
                        }
                        if (CAM[Cam_Num].Width <= (int)p.GetMinimum())
                        {
                            CAM[Cam_Num].Width = (int)p.GetMinimum();
                        }
                        p.SetValue(CAM[Cam_Num].Width);
                    }
                }
                else
                {
                    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageWidth])
                    {
                        CAM[Cam_Num].Width = (int)p.GetMaximum();
                        CAM[Cam_Num].Width -= CAM[Cam_Num].Width % 4;
                        if (CAM[Cam_Num].Width > (int)p.GetMaximum())
                        {
                            CAM[Cam_Num].Width = (int)p.GetMaximum();
                        }
                        if (CAM[Cam_Num].Width <= (int)p.GetMinimum())
                        {
                            CAM[Cam_Num].Width = (int)p.GetMinimum();
                        }
                        p.SetValue(CAM[Cam_Num].Width);
                    }
                }

                // set Height
                if (CAM[Cam_Num].Height >= 0)
                {
                    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageHeight])
                    {
                        CAM[Cam_Num].Height -= CAM[Cam_Num].Height % 4;
                        if (CAM[Cam_Num].Height > (int)p.GetMaximum())
                        {
                            CAM[Cam_Num].Height = (int)p.GetMaximum();
                        }
                        if (CAM[Cam_Num].Height <= (int)p.GetMinimum())
                        {
                            CAM[Cam_Num].Height = (int)p.GetMinimum();
                        }
                        p.SetValue(CAM[Cam_Num].Height);
                    }
                }
                else
                {
                    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageHeight])
                    {
                        CAM[Cam_Num].Height = (int)p.GetMaximum();
                        CAM[Cam_Num].Height -= CAM[Cam_Num].Height % 4;
                        if (CAM[Cam_Num].Height > (int)p.GetMaximum())
                        {
                            CAM[Cam_Num].Height = (int)p.GetMaximum();
                        }
                        if (CAM[Cam_Num].Height <= (int)p.GetMinimum())
                        {
                            CAM[Cam_Num].Height = (int)p.GetMinimum();
                        }

                        p.SetValue(CAM[Cam_Num].Height);
                    }
                }

                // set X_Offset
                if (CAM[Cam_Num].X_Offset >= 0)
                {
                    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetX])
                    {
                        if (CAM[Cam_Num].X_Offset > (int)p.GetMaximum())
                        {
                            CAM[Cam_Num].X_Offset = (int)p.GetMaximum();
                        }
                        if (CAM[Cam_Num].X_Offset <= (int)p.GetMinimum())
                        {
                            CAM[Cam_Num].X_Offset = (int)p.GetMinimum();
                        }
                        p.SetValue(CAM[Cam_Num].X_Offset);
                        //if (CAM[Cam_Num].X_Offset <= p.GetMaximum())
                        //{
                        //    p.SetValue(CAM[Cam_Num].X_Offset);
                        //}
                        //else
                        //{
                        //    CAM[Cam_Num].X_Offset = (int)p.GetMaximum();
                        //    p.SetValue(CAM[Cam_Num].X_Offset);
                        //}
                    }
                }
                else
                {
                    CAM[Cam_Num].X_Offset = 0;
                }

                // set Y_Offset
                if (CAM[Cam_Num].Y_Offset >= 0)
                {
                    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetY])
                    {
                        if (CAM[Cam_Num].Y_Offset > (int)p.GetMaximum())
                        {
                            CAM[Cam_Num].Y_Offset = (int)p.GetMaximum();
                        }
                        if (CAM[Cam_Num].Y_Offset <= (int)p.GetMinimum())
                        {
                            CAM[Cam_Num].Y_Offset = (int)p.GetMinimum();
                        }
                        p.SetValue(CAM[Cam_Num].Y_Offset);
                        //if (CAM[Cam_Num].Y_Offset <= p.GetMaximum())
                        //{
                        //    p.SetValue(CAM[Cam_Num].Y_Offset);
                        //}
                        //else
                        //{
                        //    CAM[Cam_Num].Y_Offset = (int)p.GetMaximum();
                        //    p.SetValue(CAM[Cam_Num].Y_Offset);
                        //}
                    }
                }
                else
                {
                    CAM[Cam_Num].Y_Offset = 0;
                }
            }
            catch
            { }
        }

        public void OneShot(int Cam_Num)
        {
            if (!CAM[Cam_Num].Connection)
            {
                return;
            }
            if (CAM[Cam_Num].Line1Trigger)
            {
                //CAM[Cam_Num].Dev.ExecuteSoftwareTrigger();
            }
            else
            {
                CAM[Cam_Num].Dev.TriggerSet.Open(TriggerSourceEnum.Software);
            }
            using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.AcquisitionMode])
            {
                p.SetValue(AcquisitionModeEnum.Continuous);
            }

            if (!CAM[Cam_Num].Dev.IsGrabbing)
            {
                CAM[Cam_Num].Dev.GrabUsingGrabLoopThread();
            }
            CAM[Cam_Num].Dev.StreamGrabber.Start();

            using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
            {
                p.SetValue(TriggerModeEnum.On);
            }

            if (CAM[Cam_Num].Line1Trigger)
            {
                //CAM[Cam_Num].Dev.ExecuteSoftwareTrigger();
            }
            else
            {
                CAM[Cam_Num].Dev.ExecuteSoftwareTrigger();
            }
            //Thread.Sleep(50);
            //CAM[Cam_Num].Dev.StreamGrabber.Stop();
            //if (CAM[Cam_Num].Dev.IsGrabbing)
            //{
            //    CAM[Cam_Num].Dev.ShutdownGrab();
            //}
            //if (Cam_Num == 0)
            //{
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderWidth.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderHeight.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderOffsetX.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderOffsetY.UpdateValues();
            //}
            //else if (Cam_Num == 1)
            //{
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderWidth.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderHeight.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderOffsetX.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderOffsetY.UpdateValues();
            //}
            //else if (Cam_Num == 2)
            //{
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderWidth.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderHeight.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderOffsetX.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderOffsetY.UpdateValues();
            //}
            //else if (Cam_Num == 3)
            //{
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderWidth.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderHeight.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderOffsetX.UpdateValues();
            //    LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderOffsetY.UpdateValues();
            //}
        }

        public void Continuous(int Cam_Num)
        {
            if (!CAM[Cam_Num].Connection)
            {
                return;
            }
            using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.AcquisitionMode])
            {
                p.SetValue(AcquisitionModeEnum.Continuous);
            }

            if (!CAM[Cam_Num].Dev.IsGrabbing)
            {
                CAM[Cam_Num].Dev.GrabUsingGrabLoopThread();
            }

            if (CAM[Cam_Num].Line1Trigger)
            {
                using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                {
                    p.SetValue(TriggerModeEnum.On);
                }

                using (IBooleanParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.AcquisitionFrameRateEnable])
                {
                    p.SetValue(false);
                }
            }
            else
            {
                using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                {
                    p.SetValue(TriggerModeEnum.Off);
                }
                using (IBooleanParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.AcquisitionFrameRateEnable])
                {
                    p.SetValue(true);
                }
                using (IFloatParameter p = CAM[Cam_Num].Dev.ParameterCollection[ParametrizeNameSet.AcquisitionFrameRate])
                {
                    p.SetValue(5.0);
                }
            }
            CAM[Cam_Num].Dev.StreamGrabber.Start();

            //AcquisitionModeEnum.Continuous
            //CAM[Cam_Num].Dev.GrabUsingGrabLoopThread();
        }

        public void Stop(int Cam_Num)
        {
            if (!CAM[Cam_Num].Connection || !CAM[Cam_Num].Dev.IsGrabbing)
            {
                return;
            }
            CAM[Cam_Num].Dev.StreamGrabber.Stop();
            CAM[Cam_Num].Dev.ShutdownGrab();

            if (Cam_Num == 0)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderWidth.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderHeight.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderOffsetX.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderOffsetY.UpdateValues();
            }
            else if (Cam_Num == 1)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderWidth.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderHeight.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderOffsetX.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderOffsetY.UpdateValues();
            }
            else if (Cam_Num == 2)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderWidth.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderHeight.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderOffsetX.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderOffsetY.UpdateValues();
            }
            else if (Cam_Num == 3)
            {
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderWidth.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderHeight.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderOffsetX.UpdateValues();
                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderOffsetY.UpdateValues();
            }
        }

        //private ThridLibray.IDevice m_dev;
        public bool Open(int Cam_Num)
        {
            bool t_R = true;
            if (CAM[Cam_Num].Connection)
            {
                return t_R;
            }
            try
            {
                if (CAM[Cam_Num].Dev != null)
                {
                    CAM[Cam_Num].Connection = true;
                    return true;
                }
                List<IDeviceInfo> li = Enumerator.EnumerateDevices();
                if (li.Count > 0)
                {
                    // 获取搜索到的第一个设备
                    // get the first searched device Enumerator.GetDeviceByUserID("CAM0");
                    CAM[Cam_Num].Dev = null;
                    CAM[Cam_Num].Dev = Enumerator.GetDeviceByUserID(CAM[Cam_Num].Cam_Name);
                }

                if (CAM[Cam_Num].Dev == null)
                {
                    return false;
                }

                // 打开设备
                // open device
                if (!CAM[Cam_Num].Dev.Open())
                {
                    return false;
                }

                CAM[Cam_Num].Connection = true;

                Update_Parameter(Cam_Num);

                // 注册连接事件
                // register event callback
                if (Cam_Num == 0)
                {
                    CAM[Cam_Num].Dev.CameraOpened += OnCameraOpen_0;
                    //CAM[Cam_Num].Dev.ConnectionLost += OnConnectLoss_0;
                    CAM[Cam_Num].Dev.CameraClosed += OnCameraClose_0;
                    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed += OnImageGrabbed_0;
                }
                if (Cam_Num == 1)
                {
                    CAM[Cam_Num].Dev.CameraOpened += OnCameraOpen_1;
                    //CAM[Cam_Num].Dev.ConnectionLost += OnConnectLoss_1;
                    CAM[Cam_Num].Dev.CameraClosed += OnCameraClose_1;
                    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed += OnImageGrabbed_1;
                }
                if (Cam_Num == 2)
                {
                    CAM[Cam_Num].Dev.CameraOpened += OnCameraOpen_2;
                    //CAM[Cam_Num].Dev.ConnectionLost += OnConnectLoss_2;
                    CAM[Cam_Num].Dev.CameraClosed += OnCameraClose_2;
                    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed += OnImageGrabbed_2;
                }
                if (Cam_Num == 3)
                {
                    CAM[Cam_Num].Dev.CameraOpened += OnCameraOpen_3;
                    //CAM[Cam_Num].Dev.ConnectionLost += OnConnectLoss_3;
                    CAM[Cam_Num].Dev.CameraClosed += OnCameraClose_3;
                    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed += OnImageGrabbed_3;
                }


                //// DeviceModelName
                //{
                //    using (IStringParameter p = CAM[Cam_Num].Dev.ParameterCollection[new StringName("DeviceModelName")])
                //    {
                //        //Trace.WriteLine(string.Format("DeviceModelName value: {0}", p.GetValue()));
                //        //textBox_Model.Text = p.GetValue();
                //    }
                //}

                //// AcquisitionFrameCount
                //{
                //    using (IIntegraParameter p = CAM[Cam_Num].Dev.ParameterCollection[new IntegerName("AcquisitionFrameCount")])
                //    {
                //        //Trace.WriteLine(string.Format("AcquisitionFrameCount value: {0}", p.GetValue()));
                //        //textBox_AcquisitionFrameCount.Text = p.GetValue().ToString();
                //    }
                //}

                //// ExposureTime
                //{
                //    using (IFloatParameter p = CAM[Cam_Num].Dev.ParameterCollection[new FloatName("ExposureTime")])
                //    {
                //        //Trace.WriteLine(string.Format("ExposureTime value: {0}", p.GetValue()));
                //        //textBox_ExposureTime.Text = p.GetValue().ToString("f2");
                //    }
                //}

                //// SharpnessAuto
                //{
                //    using (IEnumParameter p = CAM[Cam_Num].Dev.ParameterCollection[new EnumName("SharpnessEnabled")])
                //    {
                //        if (false == p.SetValue("On"))
                //        {
                //            //Trace.WriteLine("set SharpnessEnabled failed");
                //        }
                //    }

                //    using (IBooleanParameter p = CAM[Cam_Num].Dev.ParameterCollection[new BooleanName("SharpnessAuto")])
                //    {
                //        //string strText;
                //        //if (true == p.GetValue())
                //        //{
                //        //    strText = "True";
                //        //}
                //        //else
                //        //{
                //        //    strText = "False";
                //        //}
                //        //comboBox_SharpnessAuto.Text = strText;
                //    }
                //}
                return t_R;
            }
            catch (Exception exception)
            {
                CAM[Cam_Num].Connection = false;
                MessageBox.Show(exception.ToString());
                //Catcher.Show(exception);
                return false;
            }
        }

        public void Close(int Cam_Num)
        {
            try
            {
                //if (!CAM[Cam_Num].Connection)
                //{
                //    return;
                //}
                if (CAM[Cam_Num].Dev == null)
                {
                    return;
                    //throw new InvalidOperationException("Device is invalid");
                }

                if (CAM[Cam_Num].Connection && CAM[Cam_Num].Dev.IsGrabbing)
                {
                    CAM[Cam_Num].Dev.ShutdownGrab();
                }

                //if (Cam_Num == 0)
                //{
                //    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_0;
                //}
                //if (Cam_Num == 1)
                //{
                //    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_1;
                //}
                //if (Cam_Num == 2)
                //{
                //    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_2;
                //}
                //if (Cam_Num == 3)
                //{
                //    CAM[Cam_Num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_3;
                //}

                CAM[Cam_Num].Dev.Close();
                CAM[Cam_Num].Dev = null;
               CAM[Cam_Num].Connection = false;
            }
            catch (Exception exception)
            {
                CAM[Cam_Num].Connection = false;
                MessageBox.Show(exception.ToString());
                //Catcher.Show(exception);
            }
        }


        private void OnCameraOpen_0(object sender, EventArgs e)
        {
            int t_Cam_num = 0;
            CAM[t_Cam_num].Connection = true;
        }
        private void OnCameraClose_0(object sender, EventArgs e)
        {
            int t_Cam_num = 0;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_0;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_0;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_0;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_0;
            //Close(t_Cam_num);
        }
        private void OnConnectLoss_0(object sender, EventArgs e)
        {
            int t_Cam_num = 0;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_0;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_0;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_0;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_0;
            //Close(t_Cam_num);
            //Open(t_Cam_num);
        }
        private void OnImageGrabbed_0(Object sender, GrabbedEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 5)
            {
                LVApp.Instance().m_mainform.ctrCam1.m_bitmap = e.GrabResult.Clone().ToBitmap(false);
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[0] == 6)
            {
                LVApp.Instance().m_mainform.ctrCam1.m_bitmap = e.GrabResult.Clone().ToBitmap(true);
            }
            LVApp.Instance().m_mainform.ctrCam1_GrabComplete(sender, e);
        }

        private void OnCameraOpen_1(object sender, EventArgs e)
        {
            int t_Cam_num = 1;
            CAM[t_Cam_num].Connection = true;
        }
        private void OnCameraClose_1(object sender, EventArgs e)
        {
            int t_Cam_num = 1;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_1;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_1;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_1;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_1;
            //Close(t_Cam_num);
        }
        private void OnConnectLoss_1(object sender, EventArgs e)
        {
            int t_Cam_num = 1;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_1;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_1;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_1;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_1;
            //Close(t_Cam_num);
            //Open(t_Cam_num);
        }

        private void OnImageGrabbed_1(Object sender, GrabbedEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 5)
            {
                LVApp.Instance().m_mainform.ctrCam2.m_bitmap = e.GrabResult.Clone().ToBitmap(false);
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[1] == 6)
            {
                LVApp.Instance().m_mainform.ctrCam2.m_bitmap = e.GrabResult.Clone().ToBitmap(true);
            }
            LVApp.Instance().m_mainform.ctrCam2_GrabComplete(sender, e);
        }

        private void OnCameraOpen_2(object sender, EventArgs e)
        {
            int t_Cam_num = 2;
            CAM[t_Cam_num].Connection = true;
        }
        private void OnCameraClose_2(object sender, EventArgs e)
        {
            int t_Cam_num = 2;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_2;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_2;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_2;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_2;
            //Close(t_Cam_num);
        }
        private void OnConnectLoss_2(object sender, EventArgs e)
        {
            int t_Cam_num = 2;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_2;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_2;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_2;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_2;
            //Close(t_Cam_num);
            //Open(t_Cam_num);
        }
        private void OnImageGrabbed_2(Object sender, GrabbedEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 5)
            {
                LVApp.Instance().m_mainform.ctrCam3.m_bitmap = e.GrabResult.Clone().ToBitmap(false);
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[2] == 6)
            {
                LVApp.Instance().m_mainform.ctrCam3.m_bitmap = e.GrabResult.Clone().ToBitmap(true);
            }
            LVApp.Instance().m_mainform.ctrCam3_GrabComplete(sender, e);
        }

        private void OnCameraOpen_3(object sender, EventArgs e)
        {
            int t_Cam_num = 3;
            CAM[t_Cam_num].Connection = true;
        }
        private void OnCameraClose_3(object sender, EventArgs e)
        {
            int t_Cam_num = 3;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_3;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_3;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_3;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_3;
            //Close(t_Cam_num);
        }
        private void OnConnectLoss_3(object sender, EventArgs e)
        {
            int t_Cam_num = 3;
            CAM[t_Cam_num].Connection = false;
            CAM[t_Cam_num].Dev.CameraOpened -= OnCameraOpen_3;
            CAM[t_Cam_num].Dev.ConnectionLost -= OnConnectLoss_3;
            CAM[t_Cam_num].Dev.CameraClosed -= OnCameraClose_3;
            CAM[t_Cam_num].Dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed_3;
            //Close(t_Cam_num);
            //Open(t_Cam_num);
        }
        private void OnImageGrabbed_3(Object sender, GrabbedEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 5)
            {
                LVApp.Instance().m_mainform.ctrCam4.m_bitmap = e.GrabResult.Clone().ToBitmap(false);
            }
            else if (LVApp.Instance().m_Config.m_Cam_Kind[3] == 6)
            {
                LVApp.Instance().m_mainform.ctrCam4.m_bitmap = e.GrabResult.Clone().ToBitmap(true);
            }
            LVApp.Instance().m_mainform.ctrCam4_GrabComplete(sender, e);
        }
    }
}
