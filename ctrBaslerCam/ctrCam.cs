using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PylonC.NETSupportLibrary;
using PylonC.NET;
//using Euresys.MultiCam;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ctrBaslerCam
{
    public partial class ctrCam : UserControl
    {
        public ImageProvider m_imageProvider = new ImageProvider(); /* Create one image provider. */
        public Bitmap m_bitmap = null; /* The bitmap is used for displaying the image. */
        protected Bitmap Cam_bitmap = null;
        private int m_CamNum = -1;
        public double m_Grab_Num = 0;
        private bool m_Continuous = false;

        public string m_Model_Name = "IPSST";
        protected string m_Camera_Name = "CAM0";
        protected int m_Width = 640;
        protected int m_Height = 480;
        protected int m_X_Offset = 0;
        protected int m_Y_Offset = 0;
        protected int m_Gain = 3;
        protected int m_Exposure = 1000;
        protected bool m_Trigger_Mode = false;
        protected bool m_LineCam_mode = false;
        public int m_Camera_Interval = 30;
        private Stopwatch Cam_SW = new Stopwatch();

        private Color Grid_Line_Color = Color.LightGoldenrodYellow;

        public ctrCam()
        {
            InitializeComponent();
            if (m_Camera_Name == "CAM0")
            {
                Pylon.Initialize();
            }
            label1.Text = m_Camera_Name;
            Cam_SW.Start();
        }

        public bool Update_Camera_Config()
        {
            try
            {
                if (!m_LineCam_mode) // Area 카메라이면
                {
                    PylonC.NETSupportLibrary.SliderUserControl sliderWidth = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderHeight = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderOffsetX = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderOffsetY = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderGain = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderExposureTime = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.EnumerationComboBoxUserControl comboBoxPixelFormat = new PylonC.NETSupportLibrary.EnumerationComboBoxUserControl();

                    sliderGain.NodeName = "GainRaw";
                    sliderExposureTime.NodeName = "ExposureTimeRaw";
                    sliderWidth.NodeName = "Width";
                    sliderHeight.NodeName = "Height";
                    sliderOffsetX.NodeName = "OffsetX";
                    sliderOffsetY.NodeName = "OffsetY";
                    comboBoxPixelFormat.NodeName = "PixelFormat";

                    sliderWidth.MyImageProvider = m_imageProvider;
                    sliderHeight.MyImageProvider = m_imageProvider;
                    sliderOffsetX.MyImageProvider = m_imageProvider;
                    sliderOffsetY.MyImageProvider = m_imageProvider;
                    sliderGain.MyImageProvider = m_imageProvider;
                    sliderExposureTime.MyImageProvider = m_imageProvider;
                    comboBoxPixelFormat.MyImageProvider = m_imageProvider;

                    if (!m_imageProvider.IsOpen)
                    {
                        Open();
                    }

                    object sender = null; EventArgs e = null;
                    sliderOffsetX.slider.Value = 0;
                    sliderOffsetY.slider.Value = 0;
                    sliderOffsetX.slider_Scroll(sender, e);
                    sliderOffsetY.slider_Scroll(sender, e);


                    sliderWidth.slider.Value = m_Width;
                    sliderHeight.slider.Value = m_Height;
                    sliderWidth.slider_Scroll(sender, e);
                    sliderHeight.slider_Scroll(sender, e);

                    sliderOffsetX.slider.Value = m_X_Offset;
                    sliderOffsetY.slider.Value = m_Y_Offset;
                    sliderOffsetX.slider_Scroll(sender, e);
                    sliderOffsetY.slider_Scroll(sender, e);

                    sliderExposureTime.slider.Value = m_Exposure;
                    sliderExposureTime.slider_Scroll(sender, e);

                    if (m_Gain >= sliderGain.slider.Maximum)
                    {
                        m_Gain = sliderGain.slider.Maximum;
                    }
                    if (m_Gain <= sliderGain.slider.Minimum)
                    {
                        m_Gain = sliderGain.slider.Minimum;
                    }

                    sliderGain.slider.Value = m_Gain;
                    sliderGain.slider_Scroll(sender, e);
                    comboBoxPixelFormat.comboBox_SelectedIndexChanged(sender, e);
                }
                else // 라인카메라이면
                {
                    PylonC.NETSupportLibrary.SliderUserControl sliderWidth = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderHeight = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderOffsetX = new PylonC.NETSupportLibrary.SliderUserControl();
                    //PylonC.NETSupportLibrary.SliderUserControl sliderOffsetY = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderGain = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.SliderUserControl sliderExposureTime = new PylonC.NETSupportLibrary.SliderUserControl();
                    PylonC.NETSupportLibrary.EnumerationComboBoxUserControl comboBoxPixelFormat = new PylonC.NETSupportLibrary.EnumerationComboBoxUserControl();

                    sliderGain.NodeName = "GainRaw";
                    sliderExposureTime.NodeName = "ExposureTimeRaw";
                    sliderWidth.NodeName = "Width";
                    sliderHeight.NodeName = "Height";
                    sliderOffsetX.NodeName = "OffsetX";
                    //sliderOffsetY.NodeName = "OffsetY";
                    comboBoxPixelFormat.NodeName = "PixelFormat";

                    sliderWidth.MyImageProvider = m_imageProvider;
                    sliderHeight.MyImageProvider = m_imageProvider;
                    sliderOffsetX.MyImageProvider = m_imageProvider;
                    //sliderOffsetY.MyImageProvider = m_imageProvider;
                    sliderGain.MyImageProvider = m_imageProvider;
                    sliderExposureTime.MyImageProvider = m_imageProvider;
                    comboBoxPixelFormat.MyImageProvider = m_imageProvider;

                    if (!m_imageProvider.IsOpen)
                    {
                        Open();
                    }

                    object sender = null; EventArgs e = null;
                    sliderOffsetX.slider.Value = 0;
                    //sliderOffsetY.slider.Value = 0;
                    sliderOffsetX.slider_Scroll(sender, e);
                    //sliderOffsetY.slider_Scroll(sender, e);


                    sliderWidth.slider.Value = m_Width;
                    sliderHeight.slider.Value = m_Height;
                    sliderWidth.slider_Scroll(sender, e);
                    sliderHeight.slider_Scroll(sender, e);

                    sliderOffsetX.slider.Value = m_X_Offset;
                    //sliderOffsetY.slider.Value = m_Y_Offset;
                    sliderOffsetX.slider_Scroll(sender, e);
                    //sliderOffsetY.slider_Scroll(sender, e);

                    sliderExposureTime.slider.Value = m_Exposure;
                    sliderExposureTime.slider_Scroll(sender, e);

                    if (m_Gain >= sliderGain.slider.Maximum)
                    {
                        m_Gain = sliderGain.slider.Maximum;
                    }
                    if (m_Gain <= sliderGain.slider.Minimum)
                    {
                        m_Gain = sliderGain.slider.Minimum;
                    }
                    sliderGain.slider.Value = m_Gain;
                    sliderGain.slider_Scroll(sender, e);
                    comboBoxPixelFormat.comboBox_SelectedIndexChanged(sender, e);
                }


                return true;
            }
            catch
            {
                //MessageBox.Show("설정을 로드 할 수 없습니다. 설정값을 확인하세요!");
            }
            return true;
        }

        public bool Open()
        {
            if (m_imageProvider.IsOpen)
            {
                return true;
            }
            try
            {
                Init_Camera();
                UpdateDeviceList();
                if (m_CamNum == -1)
                {
                    Term_Camera();
                    return false;
                }
                //MessageBox.Show("카메라 번호 = " + m_CamNum.ToString());
                m_imageProvider.m_LineCam_mode = m_LineCam_mode;
                m_imageProvider.m_Trigger_mode = m_Trigger_Mode;
                m_imageProvider.Open((uint)m_CamNum);
                //MessageBox.Show("열림");
                Update_Camera_Config();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (!m_imageProvider.IsOpen)
            {
                return false;
            }
            return true;
        }

        public bool Close()
        {
            if (!m_imageProvider.IsOpen)
            {
                return true;
            }
            else
            {
                Stop();
            }
            try
            {
                m_imageProvider.Close();
                Term_Camera();
                m_Grab_Num = 0;
            }
            catch (System.Exception ex)
            {

            }
            return true;
        }

        /* Stops the image provider and handles exceptions. */
        public void Stop()
        {
            m_imageProvider.Stop();
            m_Continuous = false;
        }

        public void OneShot()
        {
            if (!m_imageProvider.IsOpen)
            {
                return;
            }
            if (m_Continuous)
            {
                Stop();
                m_Continuous = false;
            }
            m_imageProvider.OneShot(); /* Starts the grabbing of one image. */
        }

        public void ContinuousShot()
        {
            if (m_Continuous)
            {
                return;
            }

            if (!m_imageProvider.IsOpen)
            {
                return;
            }

            m_Continuous = true;
            m_imageProvider.ContinuousShot(); /* Starts the grabbing of one image. */
        }

        private void UpdateDeviceList()
        {
            try
            {
                /* Ask the device enumerator for a list of devices. */
                List<DeviceEnumerator.Device> list = DeviceEnumerator.EnumerateDevices();

                /* Add each new device to the list. */
                int t_cnt = 0;
                foreach (DeviceEnumerator.Device device in list)
                {
                    //MessageBox.Show(device.Name);
                    if (device.Name.Substring(0, m_Camera_Name.Length) == m_Camera_Name)
                    {
                        //MessageBox.Show("넘어옴");
                        m_CamNum = t_cnt;
                        break;
                    }
                    else
                    {
                        t_cnt++;
                    }
                }
                if (list.Count <= 0)
                {
                    m_CamNum = -1;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(m_imageProvider.GetLastErrorMessage());
            }

        }

        private void Init_Camera()
        {
            /* Register for the events of the image provider needed for proper operation. */
            m_imageProvider.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
            m_imageProvider.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
            m_imageProvider.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
            m_imageProvider.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
            m_imageProvider.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
            m_imageProvider.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
            m_imageProvider.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);
        }

        private void Term_Camera()
        {
            m_imageProvider.GrabErrorEvent -= new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
            m_imageProvider.DeviceRemovedEvent -= new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
            m_imageProvider.DeviceOpenedEvent -= new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
            m_imageProvider.DeviceClosedEvent -= new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
            m_imageProvider.GrabbingStartedEvent -= new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
            m_imageProvider.ImageReadyEvent -= new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
            m_imageProvider.GrabbingStoppedEvent -= new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);

        }
        /* Handles the event related to the occurrence of an error while grabbing proceeds. */
        private void OnGrabErrorEventCallback(Exception grabException, string additionalErrorMessage)
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback), grabException, additionalErrorMessage);
                return;
            }
        }
        /* Handles the event related to the removal of a currently open device. */
        private void OnDeviceRemovedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback));
                return;
            }
        }
        /* Handles the event related to a device being open. */
        private void OnDeviceOpenedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback));
                return;
            }
            /* The image provider is ready to grab. Enable the grab buttons. */
            //EnableButtons(true, false);
        }
        /* Handles the event related to a device being closed. */
        private void OnDeviceClosedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback));
                return;
            }
            /* The image provider is closed. Disable all buttons. */
            //EnableButtons(false, false);
        }
        /* Handles the event related to the image provider executing grabbing. */
        private void OnGrabbingStartedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback));
                return;
            }
        }
        /* Handles the event related to the image provider having stopped grabbing. */
        private void OnGrabbingStoppedEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback));
                return;
            }
        }

        public Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }

        private Bitmap ConvertTo24(ref Bitmap inputFileName)
        {
            //Bitmap bmpIn = inputFileName;

            Bitmap converted = new Bitmap(inputFileName.Width, inputFileName.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(converted))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.DrawImageUnscaled(inputFileName, 0, 0);
            }
            return converted;
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory")]
        static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        private Bitmap GetCopyOf(Bitmap bmp, bool CopyPalette = true)
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

        public bool t_check_grab = false;
        private EventArgs Cam_Grab_Event = new EventArgs();

        /* Handles the event related to an image having been taken and waiting for processing. */
        private void OnImageReadyEventCallback()
        {
            if (InvokeRequired)
            {
                /* If called from a different thread, we must use the Invoke method to marshal the call to the proper thread. */
                BeginInvoke(new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback));
                return;
            }
            //if (t_check_grab)
            //{
            //    return;
            //}
            //else
            //{
            //if (Cam_SW.ElapsedMilliseconds < m_Camera_Interval)
            //{
            //    return;
            //}
            //    Cam_SW.Reset();
            //    Cam_SW.Start();
            //t_check_grab = true;

            try
            {
                /* Acquire the image from the image provider. Only show the latest image. The camera may acquire images faster than images can be displayed*/
                ImageProvider.Image image = m_imageProvider.GetLatestImage();
                /* Check if the image has been removed in the meantime. */
                if (image != null)
                {
                    /* Check if the image is compatible with the currently used bitmap. */
                    if (BitmapFactory.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                    {
                        /* Update the bitmap with the image data. */
                        BitmapFactory.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    else /* A new bitmap is required. */
                    {
                        BitmapFactory.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                        BitmapFactory.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    }
                    m_imageProvider.ReleaseImage();

                    //m_Grab_Num++;
                    //if (Cam_bitmap.PixelFormat == PixelFormat.Format32bppRgb
                    //    || Cam_bitmap.PixelFormat == PixelFormat.Format32bppArgb
                    //    || Cam_bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
                    //{
                    //    m_bitmap = ConvertTo24(ref Cam_bitmap);
                    //}
                    //else
                    //{
                    //    //m_bitmap = GetCopyOf(Cam_bitmap);
                    //    //m_bitmap = Cam_bitmap.Clone() as Bitmap;
                    //}
                    /* The buffer can be used for the next image grabs. */
                    //Thread.Sleep(1);
                    if (GrabComplete != null)
                    {
                        //EventArgs Cam_Grab_Event = new EventArgs();
                        GrabComplete(this, Cam_Grab_Event);
                    }
                    //Delay(5);
                    //Thread.Sleep(3);
                    GC.Collect();
                }
                //t_check_grab = false;
            }
            catch (Exception e)
            {
                //MessageBox.Show("카메라 이미지 획득 실패 : " + e.ToString());
                m_bitmap = null;
                t_check_grab = false;
            }
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        [Description("Grab Complete"), Category("CatAction")]
        public event EventHandler GrabComplete;

        // get/set functions
        [Description("Camera Name"), Category("Data")]
        public string Camera_Name
        {
            get { return m_Camera_Name; }
            set { m_Camera_Name = value; label1.Text = m_Camera_Name; UpdateDeviceList(); }
        }

        [Description("Camera Gain"), Category("Data")]
        public int Camera_Gain
        {
            get { return m_Gain; }
            set { m_Gain = value; }
        }

        [Description("Camera Exposure"), Category("Data")]
        public int Camera_Exposure
        {
            get { return m_Exposure; }
            set { m_Exposure = value; }
        }


        [Description("Camera Width"), Category("Data")]
        public int Camera_Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        [Description("Camera Height"), Category("Data")]
        public int Camera_Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        [Description("Camera X_Offset"), Category("Data")]
        public int Camera_X_Offset
        {
            get { return m_X_Offset; }
            set { m_X_Offset = value; }
        }

        [Description("Camera Y_Offset"), Category("Data")]
        public int Camera_Y_Offset
        {
            get { return m_Y_Offset; }
            set { m_Y_Offset = value; }
        }

        [Description("Camera Trigger_Mode"), Category("Data")]
        public bool Camera_Trigger_Mode
        {
            get { return m_Trigger_Mode; }
            set { m_Trigger_Mode = value; }
        }

        [Description("Camera Line Camera"), Category("Data")]
        public bool Camera_Line_Mode
        {
            get { return m_LineCam_mode; }
            set { m_LineCam_mode = value; }
        }


    }
}
