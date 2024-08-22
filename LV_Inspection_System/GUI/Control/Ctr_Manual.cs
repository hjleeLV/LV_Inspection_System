using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;   //호환되지 않은 Dll을 사용할때
using System.Diagnostics;
using System.Threading;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
//using AForge.Imaging.IPPrototyper;
using AForge.Imaging.Formats;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Manual : UserControl
    {
        private Stopwatch[] Run_SW = new Stopwatch[8];
        public int m_Selected_Cam_Num = 0;

        public Ctr_Manual()
        {
            InitializeComponent();
            pictureBox_Manual.AllowDrop = true;
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    button_SnapShot.Text = "카메라 촬영";
                    button_Image_Load.Text = "이미지 열기";
                    button_Manual_Inspection.Text = "시험 검사";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    button_SnapShot.Text = "Snapshot";
                    button_Image_Load.Text = "Image Open";
                    button_Manual_Inspection.Text = "Test Inspection";
                }
                else if (value == 2 && m_Language != value)
                {//중국어
                    button_SnapShot.Text = "快照";
                    button_Image_Load.Text = "图像打开";
                    button_Manual_Inspection.Text = "测试检查";
                }
                m_Language = value;
            }
        }

        public Byte[] BmpToArray(Bitmap value)
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

        public Bitmap ConvertBitmap(byte[] frame, int width, int height, int ch)
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

                   = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

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

        private void radioButton_CAM0_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM0.Checked)
            {
                m_Selected_Cam_Num = 0;
            }
        }

        private void radioButton_CAM1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM1.Checked)
            {
                m_Selected_Cam_Num = 1;
            }
        }

        private void radioButton_CAM2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM2.Checked)
            {
                m_Selected_Cam_Num = 2;
            }
        }

        private void radioButton_CAM3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM3.Checked)
            {
                m_Selected_Cam_Num = 3;
            }
        }

        private void radioButton_CAM4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM4.Checked)
            {
                m_Selected_Cam_Num = 4;
            }
        }

        private void radioButton_CAM5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM5.Checked)
            {
                m_Selected_Cam_Num = 5;
            }
        }

        private void radioButton_CAM6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM6.Checked)
            {
                m_Selected_Cam_Num = 6;
            }
        }

        private void radioButton_CAM7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CAM7.Checked)
            {
                m_Selected_Cam_Num = 7;
            }
        }

        public Bitmap ConvertTo24(Bitmap inputFileName)
        {
            Bitmap bmpIn = inputFileName;

            Bitmap converted = new Bitmap(bmpIn.Width, bmpIn.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(converted))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.DrawImageUnscaled(bmpIn, 0, 0);
            }
            return converted;
        }


        private void button_SnapShot_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동 카메라 촬영를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't snapshot during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法快照!", "Notice", 2000);
                }
                return;
            }

            Bitmap t_Image = new Bitmap(640, 480);

            if (pictureBox_Manual.Image != null)
            {
                pictureBox_Manual.Image = null;
            }
            pictureBox_Manual.BackgroundImage = null;

            //if (LVApp.Instance().m_Config.ROI_Cam_Num == 0)
            //{
            //    LVApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
            //}
            //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 1)
            //{
            //    LVApp.Instance().m_mainform.ctr_ROI2.button_LOAD_Click(sender, e);
            //}
            //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 2)
            //{
            //    LVApp.Instance().m_mainform.ctr_ROI3.button_LOAD_Click(sender, e);
            //}

            
            switch (m_Selected_Cam_Num)
            {
                case 0:
                    LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                    if (LVApp.Instance().m_mainform.ctrCam1.m_bitmap == null)
                    {
                        t_Image.Dispose();
                        return;
                    }

                    t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam1.m_bitmap.Clone();
                    if (LVApp.Instance().m_mainform.ctrCam1.m_bitmap.PixelFormat == PixelFormat.Format32bppRgb)
                    {
                        t_Image = null;
                        t_Image = ConvertTo24((Bitmap)LVApp.Instance().m_mainform.ctrCam1.m_bitmap.Clone());
                    }

                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();
                    break;
                case 1:
                    LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                    if (LVApp.Instance().m_mainform.ctrCam2.m_bitmap == null)
                    {
                        t_Image.Dispose();
                        return;
                    }
                    t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam2.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();                    
                    break;
                case 2:
                    LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                    if (LVApp.Instance().m_mainform.ctrCam3.m_bitmap == null)
                    {
                        t_Image.Dispose();
                        return;
                    }
                    t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam3.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();                    break;
                case 3:
                    LVApp.Instance().m_mainform.ctr_Camera_Setting4.toolStripButtonOneShot_Click(sender, e);
                    Thread.Sleep(200);
                    if (LVApp.Instance().m_mainform.ctrCam4.m_bitmap == null)
                    {
                        t_Image.Dispose();
                        return;
                    }
                    t_Image = (Bitmap)LVApp.Instance().m_mainform.ctrCam4.m_bitmap.Clone();
                    // 영상처리 파트
                    if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == false)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == false && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipY);
                        }
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 0] == true && LVApp.Instance().m_Config.m_Cam_Filp[m_Selected_Cam_Num, 1] == true)
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            t_Image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipXY);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 0)
                        {
                            //t_Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 1)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 2)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Rot[m_Selected_Cam_Num] == 3)
                        {
                            t_Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                    }

                    pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();                    
                    break;
            }
            
            if (t_Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[m_Selected_Cam_Num] == 2)
                {
                    byte[] arr = BmpToArray(t_Image);

                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    if (m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                }
                else
                {
                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                    byte[] arr = BmpToArray(grayImage);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    if (m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                    }

                    grayImage.Dispose();
                }
            }
            else
            {
                byte[] arr = BmpToArray(t_Image);
                if (m_Selected_Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
                else if (m_Selected_Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
                else if (m_Selected_Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
                else if (m_Selected_Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
            }
            t_Image.Dispose();

            if (pictureBox_Manual.Image != null)
            {
                button_Manual_Inspection_Click(sender, e);
            }
        }

        public void button_Image_Load_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 이미지 열기를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't open an image during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法快照!", "Notice", 2000);
                }
                return;
            }

            if (t_M_I_Check)
            {
                return;
            }
            // 이미지를 불러와서 Opencv 클래스로 넣음.
            OpenFileDialog openPanel = new OpenFileDialog();
            openPanel.InitialDirectory = ".\\";
            openPanel.Filter = "All image files|*.jpg;*.bmp;*.png";
            if (openPanel.ShowDialog() == DialogResult.OK)
            {
                //if (LVApp.Instance().m_Config.ROI_Cam_Num == 0)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI1.button_LOAD_Click(sender, e);
                //}
                //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 1)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI2.button_LOAD_Click(sender, e);
                //}
                //else if (LVApp.Instance().m_Config.ROI_Cam_Num == 2)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI3.button_LOAD_Click(sender, e);
                //}

                if (pictureBox_Manual.Image != null)
                {
                    pictureBox_Manual.Image = null;
                }
                pictureBox_Manual.BackgroundImage = null;
                // Load bitmap
                ImageInfo imageInfo = null;
                Bitmap t_Image = ImageDecoder.DecodeFromFile(openPanel.FileName, out imageInfo);
                if (t_Image.PixelFormat == PixelFormat.Format32bppRgb)
                {
                    Bitmap tt_Image = ImageDecoder.DecodeFromFile(openPanel.FileName, out imageInfo);
                    t_Image = null;
                    t_Image = ConvertTo24((Bitmap)tt_Image.Clone());
                    tt_Image.Dispose();
                }

                pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();
                propertyGrid1.SelectedObject = imageInfo;
                propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24 || imageInfo.BitsPerPixel == 32)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = BmpToArray(t_Image);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        if (m_Selected_Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = BmpToArray(grayImage);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        if (m_Selected_Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                } else
                {
                    byte[] arr = BmpToArray(t_Image);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    if (m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
            }
            //return;
            if (pictureBox_Manual.Image != null)
            {
                button_Manual_Inspection_Click(sender,e);
            }
        }

        bool t_M_I_Check = false;
        public void button_Manual_Inspection_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("在线检查期间无法快照!", "Notice", 2000);
                }
                return;
            }

            if (t_M_I_Check)
            {
                return;
            }

            t_M_I_Check = true;

            if (Run_SW[m_Selected_Cam_Num] == null)
            {
                Run_SW[m_Selected_Cam_Num] = new Stopwatch();
            }

            //pictureBox_Manual.BackgroundImage = null;
            if (m_Selected_Cam_Num == 0)
            {
                LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_0.BackgroundImage = null;
                LVApp.Instance().m_mainform.pictureBox_Setting_0.BackgroundImage = null;
            }
            else if (m_Selected_Cam_Num == 1)
            {
                LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_1.BackgroundImage = null;
                LVApp.Instance().m_mainform.pictureBox_Setting_1.BackgroundImage = null;
            }
            else if (m_Selected_Cam_Num == 2)
            {
                LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_2.BackgroundImage = null;
                LVApp.Instance().m_mainform.pictureBox_Setting_2.BackgroundImage = null;
            }
            else if (m_Selected_Cam_Num == 3)
            {
                LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_3.BackgroundImage = null;
                LVApp.Instance().m_mainform.pictureBox_Setting_3.BackgroundImage = null;
            }

            LVApp.Instance().m_Config.Set_Parameters();


            //int t_CNT = LVApp.Instance().m_Config.m_AIParam[m_Selected_Cam_Num].Count;
            //if (t_CNT > 0)
            //{
            //    for (int i = 0; i < t_CNT; i++)
            //    {
            //        UTIL.IPSST_Config.AIParam t_AIParam = LVApp.Instance().m_Config.m_AIParam[m_Selected_Cam_Num][i];
            //        t_AIParam.Image = cropAtRect(pictureBox_Manual.Image.Clone() as Bitmap, t_AIParam.ROI);
            //        LVApp.Instance().m_Config.m_AIParam[m_Selected_Cam_Num][i] = t_AIParam;
            //    }
            //}

            //if (LVApp.Instance().m_mainform.m_Job_Mode[m_Selected_Cam_Num] == 0)
            //{
            //    LVApp.Instance().m_mainform.m_Job_Mode[m_Selected_Cam_Num] = 1;
            //}
            //if (m_Selected_Cam_Num == 4)
            //{
            //    LVApp.Instance().m_mainform.timer_Refresh_Amount.Start();
            //}
            //return;
            Run_SW[m_Selected_Cam_Num].Reset();
            Run_SW[m_Selected_Cam_Num].Start();
            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Display_Parameters(false, -1, -1);
            //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.m_Alg_Type, LVApp.Instance().m_Config.Alg_Debugging);
            if (pictureBox_Manual.Image == null)
            {
                return;
            }

            //string[] strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_Algorithm(m_Selected_Cam_Num).Split('_');
            //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(m_Selected_Cam_Num);
            //LVApp.Instance().m_mainform.m_ImProClr_Class.Reset_Dst_Image(m_Selected_Cam_Num);

            if (pictureBox_Manual.Image.Width == 578 && pictureBox_Manual.Image.Height == 433)
            {
                if (m_Selected_Cam_Num == 0)
                {
                    pictureBox_Manual.Image = LVApp.Instance().m_mainform.ctr_ROI1.pictureBox_Image.Image.Clone() as Bitmap;
                }
                if (m_Selected_Cam_Num == 1)
                {
                    pictureBox_Manual.Image = LVApp.Instance().m_mainform.ctr_ROI2.pictureBox_Image.Image.Clone() as Bitmap;
                }
                if (m_Selected_Cam_Num == 2)
                {
                    pictureBox_Manual.Image = LVApp.Instance().m_mainform.ctr_ROI3.pictureBox_Image.Image.Clone() as Bitmap;
                }
                if (m_Selected_Cam_Num == 3)
                {
                    pictureBox_Manual.Image = LVApp.Instance().m_mainform.ctr_ROI4.pictureBox_Image.Image.Clone() as Bitmap;
                }
            }
            if (pictureBox_Manual.Image == null)
            {
                return;
            }
            Bitmap t_Image = pictureBox_Manual.Image.Clone() as Bitmap;

            if (t_Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                if (LVApp.Instance().m_Config.m_Cam_Kind[m_Selected_Cam_Num] == 2 || LVApp.Instance().m_Config.m_Cam_Kind[m_Selected_Cam_Num] == 6)
                {
                    byte[] arr = BmpToArray(t_Image);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 3, Cam_Num);

                    if (m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                    }
                }
                else
                {
                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                    byte[] arr = BmpToArray(grayImage);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, grayImage.Width, grayImage.Height, 1, Cam_Num);
                    if (m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    grayImage.Dispose();
                }
            }
            else
            {
                byte[] arr = BmpToArray(t_Image);
                //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, Cam_Num);

                if (m_Selected_Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
                else if (m_Selected_Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
                else if (m_Selected_Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
                else if (m_Selected_Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                }
            }

            Run_Inspection(m_Selected_Cam_Num, ref t_Image);
            t_Image.Dispose();
            int Judge = LVApp.Instance().m_Config.Judge_DataSet(m_Selected_Cam_Num);
            //if (this.InvokeRequired)
            //{
            //    this.Invoke((MethodInvoker)delegate
            //    {
            //        LVApp.Instance().m_mainform.ctr_PLC1.send_Message[m_Selected_Cam_Num].Add("DW5" + LVApp.Instance().m_mainform.ctr_PLC1.m_SlaveID.Substring(1, 1) + "0" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //        //if (LVApp.Instance().m_Config.m_Alg_Type == 0)
            //        //{
            //        //    LVApp.Instance().m_mainform.ctr_PLC1.send_Message[m_Selected_Cam_Num].Add("DW5" + LVApp.Instance().m_mainform.ctr_PLC1.m_SlaveID.Substring(1, 1) + "0" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //        //}
            //        //else if (LVApp.Instance().m_Config.m_Alg_Type == 1)
            //        //{
            //        //    LVApp.Instance().m_mainform.ctr_PLC1.send_Message[m_Selected_Cam_Num].Add("DW520" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //        //}
            //    });
            //}
            //else
            //{
            //    LVApp.Instance().m_mainform.ctr_PLC1.send_Message[m_Selected_Cam_Num].Add("DW5" + LVApp.Instance().m_mainform.ctr_PLC1.m_SlaveID.Substring(1, 1) + "0" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //    //if (LVApp.Instance().m_Config.m_Alg_Type == 0)
            //    //{
            //    //    LVApp.Instance().m_mainform.ctr_PLC1.send_Message[m_Selected_Cam_Num].Add("DW5" + LVApp.Instance().m_mainform.ctr_PLC1.m_SlaveID.Substring(1, 1) + "0" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //    //}
            //    //else if (LVApp.Instance().m_Config.m_Alg_Type == 1)
            //    //{
            //    //    LVApp.Instance().m_mainform.ctr_PLC1.send_Message[m_Selected_Cam_Num].Add("DW520" + m_Selected_Cam_Num.ToString() + "_" + Judge.ToString());
            //    //}
            //}

            byte[] Dst_Img = null;
            int width = 0, height = 0, ch = 0;

            if (m_Selected_Cam_Num == 0)
            {
                LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image0(out Dst_Img, out width, out height, out ch, m_Selected_Cam_Num);
            }
            else if (m_Selected_Cam_Num == 1)
            {
                LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image1(out Dst_Img, out width, out height, out ch, m_Selected_Cam_Num);
            }
            else if (m_Selected_Cam_Num == 2)
            {
                LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image2(out Dst_Img, out width, out height, out ch, m_Selected_Cam_Num);
            }
            else if (m_Selected_Cam_Num == 3)
            {
                LVApp.Instance().m_mainform.m_ImProClr_Class.Get_Image3(out Dst_Img, out width, out height, out ch, m_Selected_Cam_Num);
            }
            //if (LVApp.Instance().m_mainform.m_ImProClr_Class0.Get_Image0(out Dst_Img, out width, out height, out ch, m_Selected_Cam_Num))
            //{
                //pictureBox_Manual.Image = null;
                //pictureBox_Manual.Image = ConvertBitmap(Dst_Img, width, height, 3);

                Bitmap newbmp = ConvertBitmap(Dst_Img, width, height, 3);

            if (width > 0 && height > 0)
            {
                LVApp.Instance().m_mainform.ctr_Display_1.m_Selected_PictureBox = m_Selected_Cam_Num;
                if (m_Selected_Cam_Num == 0)
                {
                    LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_0.Image = newbmp.Clone() as Bitmap;
                    LVApp.Instance().m_mainform.pictureBox_Setting_0.Image = newbmp.Clone() as Bitmap;
                }
                else if (m_Selected_Cam_Num == 1)
                {
                    LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_1.Image = newbmp.Clone() as Bitmap;
                    LVApp.Instance().m_mainform.pictureBox_Setting_1.Image = newbmp.Clone() as Bitmap;
                }
                else if (m_Selected_Cam_Num == 2)
                {
                    LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_2.Image = newbmp.Clone() as Bitmap;
                    LVApp.Instance().m_mainform.pictureBox_Setting_2.Image = newbmp.Clone() as Bitmap;
                }
                else if (m_Selected_Cam_Num == 3)
                {
                    LVApp.Instance().m_mainform.ctr_Display_1.pictureBox_3.Image = newbmp.Clone() as Bitmap;
                    LVApp.Instance().m_mainform.pictureBox_Setting_3.Image = newbmp.Clone() as Bitmap;
                }
            //}
            if (newbmp != null)
            {
                if (Judge == 40)
                {
                    LVApp.Instance().m_Config.m_Error_Flag[m_Selected_Cam_Num] = 0;
                    //LVApp.Instance().m_Config.m_OK_NG_Cnt[m_Selected_Cam_Num, 0]+=1; 
                    String filename = LVApp.Instance().m_Config.Result_Image_Save(m_Selected_Cam_Num, (Bitmap)newbmp.Clone(), 0);
                    LVApp.Instance().m_Config.Add_Log_Data(m_Selected_Cam_Num, filename);
                }
                else
                {
                    if (Judge == -1)
                    {
                            LVApp.Instance().m_Config.m_Error_Flag[m_Selected_Cam_Num] = 2;
                            //LVApp.Instance().m_Config.m_OK_NG_Cnt[m_Selected_Cam_Num, 1]+=1; 
                            String filename = LVApp.Instance().m_Config.Result_Image_Save(m_Selected_Cam_Num, (Bitmap)newbmp.Clone(), 2);
                            LVApp.Instance().m_Config.Add_Log_Data(m_Selected_Cam_Num, filename);
                            if (LVApp.Instance().m_Config.NG_Log_Use)
                            {
                                LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[m_Selected_Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                                LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[m_Selected_Cam_Num] = (Bitmap)newbmp.Clone();
                                LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(m_Selected_Cam_Num);
                            }
                    }
                    else
                    {
                        LVApp.Instance().m_Config.m_Error_Flag[m_Selected_Cam_Num] = 1;
                        //LVApp.Instance().m_Config.m_OK_NG_Cnt[m_Selected_Cam_Num, 1]+=1; 
                        String filename = LVApp.Instance().m_Config.Result_Image_Save(m_Selected_Cam_Num, (Bitmap)newbmp.Clone(), 1);
                        LVApp.Instance().m_Config.Add_Log_Data(m_Selected_Cam_Num, filename);
                        if (LVApp.Instance().m_Config.NG_Log_Use)
                        {
                            LVApp.Instance().m_mainform.ctr_NGLog1.t_Time[m_Selected_Cam_Num] = DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");
                            LVApp.Instance().m_mainform.ctr_NGLog1.t_Bitmap[m_Selected_Cam_Num] = (Bitmap)newbmp.Clone();
                            LVApp.Instance().m_mainform.ctr_NGLog1.Insert_Data(m_Selected_Cam_Num);
                        }
                    }
                }
                newbmp.Dispose();
                }
            }

            if (m_Selected_Cam_Num == 0)
            {
                LVApp.Instance().m_mainform.ctr_DataGrid1.Min_Max_Update(m_Selected_Cam_Num);
            }
            else if (m_Selected_Cam_Num == 1)
            {
                LVApp.Instance().m_mainform.ctr_DataGrid2.Min_Max_Update(m_Selected_Cam_Num);
            }
            else if (m_Selected_Cam_Num == 2)
            {
                LVApp.Instance().m_mainform.ctr_DataGrid3.Min_Max_Update(m_Selected_Cam_Num);
            }
            else if (m_Selected_Cam_Num == 3)
            {
                LVApp.Instance().m_mainform.ctr_DataGrid4.Min_Max_Update(m_Selected_Cam_Num);
            }

            Run_SW[m_Selected_Cam_Num].Stop();
            //mainform.m_TT[0] = "T/T : " + Run_sw0.ElapsedMilliseconds.ToString() + "ms";
            LVApp.Instance().m_mainform.add_Log("T/T : " + Run_SW[m_Selected_Cam_Num].ElapsedMilliseconds.ToString() + "ms");
            t_M_I_Check = false;
            GC.Collect();
        }

        private void AI_Image_Save(int Cam_num, int ROI_IDX, Bitmap t_bmp)
        {
            try
            {
                Bitmap m_bmp = t_bmp.Clone() as Bitmap;
                String m_AI_folder = LVApp.Instance().excute_path + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                if (LVApp.Instance().m_Config.m_Log_Save_Folder.Length > 1)
                {
                    m_AI_folder = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Images\\" + LVApp.Instance().m_Config.m_Model_Name + "\\" + DateTime.Now.ToString("yyyy_MM_dd");
                }
                if (LVApp.Instance().m_Config.AI_Image_Save)
                {
                    DirectoryInfo dir = new DirectoryInfo(m_AI_folder + "\\AI");
                    // 폴더가 존재하지 않으면
                    if (dir.Exists == false)
                    {
                        // 새로 생성합니다.
                        dir.Create();
                    }

                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting1.Force_USE.Checked && (Cam_num == 0 || Cam_num == 4))
                    {
                        int t_cam_num = 0;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam1.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + (t_cam_num).ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM1");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        m_bmp.Save(dir.FullName + "\\ROI" + ROI_IDX.ToString("00") + "_" + DateTime.Now.ToString("HHmmss_fff") + ".png", ImageFormat.Png);
                    }
                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting2.Force_USE.Checked && (Cam_num == 1 || Cam_num == 5))
                    {
                        int t_cam_num = 1;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam2.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + (t_cam_num).ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM2");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        m_bmp.Save(dir.FullName + "\\ROI" + ROI_IDX.ToString("00") + "_" + DateTime.Now.ToString("HHmmss_fff") + ".png", ImageFormat.Png);
                    }
                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting3.Force_USE.Checked && (Cam_num == 2 || Cam_num == 6))
                    {
                        int t_cam_num = 2;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam3.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + (t_cam_num).ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM3");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        m_bmp.Save(dir.FullName + "\\ROI" + ROI_IDX.ToString("00") + "_" + DateTime.Now.ToString("HHmmss_fff") + ".png", ImageFormat.Png);
                    }
                    if (!LVApp.Instance().m_mainform.ctr_Camera_Setting4.Force_USE.Checked && (Cam_num == 3 || Cam_num == 7))
                    {
                        int t_cam_num = 3;
                        if (int.TryParse(LVApp.Instance().m_mainform.ctrCam4.Camera_Name.Substring(3, 1), out t_cam_num))
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM" + (t_cam_num).ToString());
                        }
                        else
                        {
                            dir = new DirectoryInfo(m_AI_folder + "\\AI\\CAM4");
                        }

                        // 폴더가 존재하지 않으면
                        if (dir.Exists == false)
                        {
                            // 새로 생성합니다.
                            dir.Create();
                        }
                        m_bmp.Save(dir.FullName + "\\ROI" + ROI_IDX.ToString("00") + "_" + DateTime.Now.ToString("HHmmss_fff") + ".png", ImageFormat.Png);
                    }
                }
                m_bmp.Dispose();
            }
            catch
            { }
        }

        public void Run_Inspection(int Cam_num, ref Bitmap Camera_Img)
        {
            try
            {
                //LVApp.Instance().m_Config.m_Error_Flag[Cam_num] = -1;
                //lock (this)
                //{
                string[] strParameter = null;
                LVApp.Instance().m_Config.Set_ROI(Cam_num);
                string t_str = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_Algorithm(Cam_num);
                strParameter = t_str.Split('_');
                int t_result_CNT = strParameter.Length;

                if (t_result_CNT > 1)
                {
                    if (double.Parse(strParameter[1]) > -1)
                    {
                        int tt_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                        if (tt_CNT > 0)
                        {
                            string[] str_Offset = LVApp.Instance().m_mainform.m_ImProClr_Class.GET_Offset_Object_Location(Cam_num).Split('_');
                            System.Drawing.Point p_Offset = new System.Drawing.Point(0, 0);
                            int t_v = 0;
                            int.TryParse(str_Offset[0], out t_v); p_Offset.X = t_v; t_v = 0;
                            int.TryParse(str_Offset[1], out t_v); p_Offset.Y = t_v;

                            for (int i = 0; i < tt_CNT; i++)
                            {
                                UTIL.LV_Config.AIParam t_AIParam = LVApp.Instance().m_Config.m_AIParam[Cam_num][i];
                                if (t_AIParam.USE)
                                {
                                Rectangle t_ROI = new Rectangle(t_AIParam.ROI.X, t_AIParam.ROI.Y, t_AIParam.ROI.Width, t_AIParam.ROI.Height);

                                t_ROI.X += p_Offset.X;
                                t_ROI.Y += p_Offset.Y;
                                if (t_ROI.X < 0)
                                {
                                    t_ROI.X = 0;
                                }
                                if (t_ROI.Y < 0)
                                {
                                    t_ROI.Y = 0;
                                }

                                if (t_ROI.X + t_ROI.Width >= Camera_Img.Width)
                                {
                                    t_ROI.Width = Camera_Img.Width - t_ROI.X - 1;
                                }
                                if (t_ROI.Y + t_ROI.Height >= Camera_Img.Height)
                                {
                                    t_ROI.Height = Camera_Img.Height - t_ROI.Y - 1;
                                }

                                if (t_ROI.Width < 1)
                                {
                                    t_ROI.Width = 1;
                                }
                                if (t_ROI.Height < 1)
                                {
                                    t_ROI.Height = 1;
                                }
                                t_AIParam.Image = cropAtRect(Camera_Img.Clone() as Bitmap, t_ROI);
                                    if (t_AIParam.Mask != null)
                                    {
                                        t_AIParam.Cut_Mask = cropAtRect(t_AIParam.Mask.Clone() as Bitmap, t_ROI);
                                    }
                                    else
                                    {
                                        t_AIParam.Cut_Mask = null;
                                    }

                                    //t_AIParam.Image.Save("00.png", ImageFormat.Png);
                                    if (LVApp.Instance().m_Config.AI_Image_Save)
                                    {
                                        if (t_AIParam.Cut_Mask == null)
                                        {
                                            System.Drawing.Size resize = new System.Drawing.Size(224, 224);
                                            ;
                                            System.Threading.ThreadPool.QueueUserWorkItem(o =>
                                            {
                                                using (Bitmap m_bmp = new Bitmap(t_AIParam.Image.Clone() as Bitmap, resize))
                                                {
                                                    AI_Image_Save(Cam_num, t_AIParam.ROI_IDX + 1, m_bmp);
                                                }
                                            });
                                        }
                                        else
                                        {
                                            Mat t_Ori = BitmapConverter.ToMat(t_AIParam.Image.Clone() as Bitmap);
                                            Mat t_Mask = BitmapConverter.ToMat(t_AIParam.Cut_Mask.Clone() as Bitmap);

                                            Mat C_Ori = new Mat();
                                            if (t_Ori.Channels() == 1)
                                            {
                                                Cv2.CvtColor(t_Ori, C_Ori, ColorConversionCodes.GRAY2RGB);
                                            }
                                            else if (t_Ori.Channels() == 3)
                                            {
                                                t_Ori.CopyTo(C_Ori);
                                            }
                                            else if (t_Ori.Channels() == 4)
                                            {
                                                Cv2.CvtColor(t_Ori, C_Ori, ColorConversionCodes.RGBA2RGB);
                                            }

                                            Mat C_Mask = new Mat();
                                            if (t_Mask.Channels() == 1)
                                            {
                                                Cv2.CvtColor(t_Mask, C_Mask, ColorConversionCodes.GRAY2RGB);
                                            }
                                            else if (t_Mask.Channels() == 3)
                                            {
                                                t_Mask.CopyTo(C_Mask);
                                            }
                                            else if (t_Mask.Channels() == 4)
                                            {
                                                Cv2.CvtColor(t_Mask, C_Mask, ColorConversionCodes.RGBA2RGB);
                                            }
                                            Cv2.BitwiseAnd(C_Ori, C_Mask, C_Ori);
                                            Bitmap t_bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(C_Ori);

                                            System.Drawing.Size resize = new System.Drawing.Size(224, 224);
                                            Bitmap m_bmp = new Bitmap(t_bmp.Clone() as Bitmap, resize);
                                            t_AIParam.Image = null;
                                            t_AIParam.Image = m_bmp.Clone() as Bitmap;
                                            //t_AIParam.Image = m_bmp;// cropAtRect(t_Camera_Img.Clone() as Bitmap, t_ROI);
                                            System.Threading.ThreadPool.QueueUserWorkItem(o =>
                                            {
                                                using (Bitmap m_bmp1 = new Bitmap(m_bmp.Clone() as Bitmap, resize))
                                                {
                                                    AI_Image_Save(Cam_num, t_AIParam.ROI_IDX + 1, m_bmp1);
                                                }
                                            });

                                            //m_bmp.Dispose();
                                            t_bmp.Dispose();
                                        }

                                    }
                                    LVApp.Instance().m_Config.m_AIParam[Cam_num][i] = t_AIParam;
                                }
                            }
                        }
                        RUN_AI(Cam_num);
                    }
                }

                switch (Cam_num)
                {
                    case 0:
                        CurrencyManager currencyManager0 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.DataSource];
                        currencyManager0.SuspendBinding();

                        if (strParameter == null || t_result_CNT < 2)
                        {
                            for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows.Count; i++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[i][0].ToString() == "True")
                                {
                                    LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[i][3] = -1;
                                }
                            }
                        }
                        for (int i = 0; i < t_result_CNT; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[j][0].ToString() == "True")
                                    {
                                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);

                                        int t_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                                        if (t_CNT > 0)
                                        {
                                            for (int k = 0; k < t_CNT; k++)
                                            {
                                                if (LVApp.Instance().m_Config.m_AIParam[Cam_num][k].ROI_IDX == j)
                                                {
                                                    if (strParameter[i + 1] != "-1")
                                                    {
                                                        LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result_IDX;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        //LVApp.Instance().m_mainform.dataGridView_Setting_Value_0.ClearSelection();
                        currencyManager0.ResumeBinding();
                        break;
                    case 1:
                        CurrencyManager currencyManager1 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_1.DataSource];
                        currencyManager1.SuspendBinding();
                        if (strParameter == null || t_result_CNT < 2)
                        {
                            for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows.Count; i++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[i][0].ToString() == "True")
                                {
                                    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[i][3] = -1;
                                }

                            }
                        }
                        for (int i = 0; i < t_result_CNT; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[j][0].ToString().ToUpper() == "TRUE")
                                    {
                                        LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    }
                                    //if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][2]);
                                    //}
                                    int t_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                                    if (t_CNT > 0)
                                    {
                                        for (int k = 0; k < t_CNT; k++)
                                        {
                                            if (LVApp.Instance().m_Config.m_AIParam[Cam_num][k].ROI_IDX == j)
                                            {
                                                if (strParameter[i + 1] != "-1")
                                                {
                                                    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result_IDX;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        currencyManager1.ResumeBinding();
                        break;
                    case 2:
                        CurrencyManager currencyManager2 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_2.DataSource];
                        currencyManager2.SuspendBinding();
                        if (strParameter == null || t_result_CNT < 2)
                        {
                            for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows.Count; i++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[i][0].ToString() == "True")
                                {
                                    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[i][3] = -1;
                                }

                            }
                        }
                        for (int i = 0; i < t_result_CNT; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[j][0].ToString() == "True")
                                    {
                                        LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    }
                                    //if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][2]);
                                    //}
                                    int t_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                                    if (t_CNT > 0)
                                    {
                                        for (int k = 0; k < t_CNT; k++)
                                        {
                                            if (LVApp.Instance().m_Config.m_AIParam[Cam_num][k].ROI_IDX == j)
                                            {
                                                if (strParameter[i + 1] != "-1")
                                                {
                                                    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result_IDX;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        currencyManager2.ResumeBinding();
                        break;
                    case 3:
                        CurrencyManager currencyManager3 = (CurrencyManager)LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.BindingContext[LVApp.Instance().m_mainform.dataGridView_Setting_Value_3.DataSource];
                        currencyManager3.SuspendBinding();
                        if (strParameter == null || t_result_CNT < 2)
                        {
                            for (int i = 0; i < LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows.Count; i++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[i][0].ToString() == "True")
                                {
                                    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[i][3] = -1;
                                }

                            }
                        }
                        for (int i = 0; i < t_result_CNT; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[j][0].ToString() == "True")
                                    {
                                        LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    }
                                    //if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][2]);
                                    //}
                                    int t_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                                    if (t_CNT > 0)
                                    {
                                        for (int k = 0; k < t_CNT; k++)
                                        {
                                            if (LVApp.Instance().m_Config.m_AIParam[Cam_num][k].ROI_IDX == j)
                                            {
                                                if (strParameter[i + 1] != "-1")
                                                {
                                                    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = LVApp.Instance().m_Config.m_AIParam[Cam_num][k].Result_IDX;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        currencyManager3.ResumeBinding();
                        break;
                    }
                    //LVApp.Instance().m_Config.Add_Log_Data(Cam_num, "");
               // }
            }
            catch// (System.Exception ex)
            {
            }
        }

        public void RUN_AI(int Cam_num)
        {
            try
            {
                int t_CNT = LVApp.Instance().m_Config.m_AIParam[Cam_num].Count;
                if (t_CNT > 0)
                {
                    for (int i = 0; i < t_CNT; i++)
                    {
                        Stopwatch t_stw = new Stopwatch();
                        t_stw.Start();
                        UTIL.LV_Config.AIParam t_AIParam = LVApp.Instance().m_Config.m_AIParam[Cam_num][i];
                        if (t_AIParam.Image != null)
                        {
                            t_AIParam.Result = LVApp.Instance().m_AI_Pro.AI_Recognition_Image(Cam_num, t_AIParam.ROI_IDX, ref t_AIParam.Image);
                        }
                        t_stw.Stop();
                        t_AIParam.T_T = t_stw.ElapsedMilliseconds;

                        string[] t_R = t_AIParam.Result.Split(';');
                        double Max_MR = 0; int Max_IDX = -1; t_AIParam.Result_IDX = -1;
                        if (t_R.Length > 0)
                        {
                            for (int j = 0; j < t_R.Length; j++)
                            {
                                string[] t_RP = t_R[j].Split(':');
                                if (t_RP.Length == 2)
                                {
                                    double t_MR = 0;
                                    double.TryParse(t_RP[1], out t_MR);
                                    if (t_MR >= t_AIParam.Matching_Rate)
                                    {
                                        if (Max_MR <= t_MR)
                                        {
                                            Max_MR = t_MR;
                                            Max_IDX = j;

                                            t_AIParam.Result_MR = t_MR;
                                            t_AIParam.Result_Label = t_RP[0];
                                            t_AIParam.Result_IDX = j;
                                        }
                                    }
                                }
                            }
                        }

                        LVApp.Instance().m_Config.m_AIParam[Cam_num][i] = t_AIParam;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Bitmap cropAtRect(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, -section.X, -section.Y);
                return bitmap;
            }
        }

        public void Run_MissedInspection(int Cam_num)
        {
            //LVApp.Instance().m_Config.m_Error_Flag[Cam_num] = -1;

            try
            {
                //lock (this)
                //{
                string[] strParameter = null;
                if (Cam_num == 0)
                {
                    strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_MissedAlgorithm(Cam_num).Split('_');
                }
                else if (Cam_num == 1)
                {
                    strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_MissedAlgorithm(Cam_num).Split('_');
                }
                else if (Cam_num == 2)
                {
                    strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_MissedAlgorithm(Cam_num).Split('_');
                }
                else if (Cam_num == 3)
                {
                    strParameter = LVApp.Instance().m_mainform.m_ImProClr_Class.RUN_MissedAlgorithm(Cam_num).Split('_');
                }

                if (strParameter == null || strParameter.Length < 2)
                {
                    return;
                }

                switch (Cam_num)
                {
                    case 0:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_0.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 1:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_1.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_2.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 3:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_3.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 4:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_4.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_4.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 5:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_5.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_5.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 6:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_6.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_6.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                    case 7:
                        for (int i = 0; i < strParameter.Length; i++)
                        {
                            for (int j = 0; j < LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows.Count; j++)
                            {
                                if (LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows[j][0].ToString() == strParameter[i])
                                {
                                    LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]);
                                    //if (LVApp.Instance().m_Config.ds_DATA_7.Tables[0].Rows[j][2].ToString().Contains("(mm)"))
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) * LVApp.Instance().m_Config.m_Cam_Resolution[Cam_num, 0] + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows[j][2]);
                                    //}
                                    //else
                                    //{
                                    //    LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows[j][3] = Convert.ToDouble(strParameter[i + 1]) + Convert.ToDouble(LVApp.Instance().m_Config.ds_DATA_7.Tables[1].Rows[j][2]);
                                    //}
                                }
                            }
                        }
                        break;
                }
                //LVApp.Instance().m_Config.Add_Log_Data(Cam_num, "");
                // }
            }
            catch//(System.Exception ex)
            {

            }
        }

        private void pictureBox_Manual_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_Manual.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveMain));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupViewMain));
                        cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSaveMain));
                    }

                    pictureBox_Manual.ContextMenu = cm;
                    pictureBox_Manual.ContextMenu.Show(pictureBox_Manual, e.Location);
                    pictureBox_Manual.ContextMenu = null;
                }
            }
        }

        private void PictureBoxPopupViewMain(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (System.Drawing.Image)pictureBox_Manual.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSaveMain(object sender, EventArgs e)
        {
            using (System.Drawing.Image bmp = (System.Drawing.Image)pictureBox_Manual.Image.Clone())
            {
                if (bmp == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bmp);
                }
            }
        }

        public void Image_SaveFileDialog(System.Drawing.Image bmp)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = LVApp.Instance().excute_path;
            SaveFileDialog1.RestoreDirectory = true;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                SaveFileDialog1.Title = "이미지 저장";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                SaveFileDialog1.Title = "Image save";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                SaveFileDialog1.Title = "图像保存";
            }

            SaveFileDialog1.Filter = "All image files|*.jpg;*.bmp;*.png";
            SaveFileDialog1.FilterIndex = 2;
            SaveFileDialog1.FileName = "Save_" + m_Selected_Cam_Num.ToString() + ".bmp";
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

        private void PictureBoxResultviewMain(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.Alg_TextView)
            {
                LVApp.Instance().m_Config.Alg_TextView = false;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_TextView.Checked = false;
            }
            else
            {
                LVApp.Instance().m_Config.Alg_TextView = true;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_TextView.Checked = true;
            }
            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);
        }

        private void pictureBox_Manual_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    using (Font myFont = new Font("Arial", 11))
                    {
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            e.Graphics.DrawString("자동 검사중... 수동검사 불가!", myFont, Brushes.OrangeRed, new System.Drawing.Point(5, 5));
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                        {//영어
                            e.Graphics.DrawString("Can't test during inspection running!", myFont, Brushes.OrangeRed, new System.Drawing.Point(5, 5));
                        }
                        else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                        {//중국어
                            e.Graphics.DrawString("检查运行期间无法测试!", myFont, Brushes.OrangeRed, new System.Drawing.Point(5, 5));
                        }
                    }
                    return;
                }
                int CamNum = m_Selected_Cam_Num;
                using (Font myFont = new Font("Arial", 10))
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[CamNum])
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new System.Drawing.Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new System.Drawing.Point(4, 4));
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString() + " turn off", myFont, Brushes.LightGoldenrodYellow, new System.Drawing.Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString("CAM " + CamNum.ToString() + " turn off", myFont, Brushes.LightGoldenrodYellow, new System.Drawing.Point(4, 4));
                        }
                    }
                }

                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_Manual.Width - 1, pictureBox_Manual.Height - 1);
                }

                if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
                {
                    using (Font myFont = new Font("Arial", 12))
                    {
                        e.Graphics.DrawString("N G", myFont, Brushes.Red, new System.Drawing.Point(pictureBox_Manual.Width - 40, 10));
                    }

                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString("T/T " + Run_SW[m_Selected_Cam_Num].ElapsedMilliseconds.ToString() + "ms", myFont, Brushes.CornflowerBlue, new System.Drawing.Point(pictureBox_Manual.Width - 120, pictureBox_Manual.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            if (CamNum == 0)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[0][3].ToString(), myFont, Brushes.Red, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));

                            }
                            else if (CamNum == 1)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[0][3].ToString(), myFont, Brushes.Red, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));

                            }
                            else if (CamNum == 2)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[0][3].ToString(), myFont, Brushes.Red, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));

                            }
                            else if (CamNum == 3)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[0][3].ToString(), myFont, Brushes.Red, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));
                            }
                        }
                    }
                }
                else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
                {
                    using (Font myFont = new Font("Arial", 12))
                    {
                        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new System.Drawing.Point(pictureBox_Manual.Width - 40, 10));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString("T/T " + Run_SW[m_Selected_Cam_Num].ElapsedMilliseconds.ToString() + "ms", myFont, Brushes.CornflowerBlue, new System.Drawing.Point(pictureBox_Manual.Width - 120, pictureBox_Manual.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            if (CamNum == 0)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_0.Tables[1].Rows[0][3].ToString(), myFont, Brushes.SkyBlue, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));

                            }
                            else if (CamNum == 1)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_1.Tables[1].Rows[0][3].ToString(), myFont, Brushes.SkyBlue, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));

                            }
                            else if (CamNum == 2)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_2.Tables[1].Rows[0][3].ToString(), myFont, Brushes.SkyBlue, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));

                            }
                            else if (CamNum == 3)
                            {
                                e.Graphics.DrawString(LVApp.Instance().m_Config.ds_DATA_3.Tables[1].Rows[0][3].ToString(), myFont, Brushes.SkyBlue, new System.Drawing.Point(pictureBox_Manual.Width / 2 - 40, pictureBox_Manual.Height / 2));
                            }
                        }
                    }

                }


                if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0 || LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
                {
                    if (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] > 0)
                    {
                        int t_Width = 0;

                        string[] t_str = LVApp.Instance().m_Config.Disp_OKNG_List[CamNum].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        int t_length = t_str.Length;
                        if (t_length > 0)
                        {
                            for (int i = 0; i < t_length; i++)
                            {
                                if (t_str[i].Length > t_Width)
                                {
                                    t_Width = t_str[i].Length;
                                }
                            }
                        }
                        t_Width *= 9;

                        using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                        {
                            e.Graphics.FillRectangle(myBrush, new Rectangle(3, pictureBox_Manual.Height - LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 - 5, t_Width, LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 + 9));
                        }

                        if (t_length > 0)
                        {
                            for (int i = 0; i < t_length; i++)
                            {
                                if (t_str[i].Contains("_OK"))
                                {
                                    using (Font myFont = new Font("Arial", 9))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.SkyBlue, new System.Drawing.Point(5, -3 + pictureBox_Manual.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                                else if (t_str[i].Contains("_NG"))
                                {
                                    using (Font myFont = new Font("Arial", 9))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.OrangeRed, new System.Drawing.Point(5, -3 + pictureBox_Manual.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void pictureBox_Manual_DragEnter(object sender, DragEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //{//한국어
                //    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                //}
                //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                //{//영어
                //    AutoClosingMessageBox.Show("Can't do manual inspection during online inspection!", "Notice", 2000);
                //}
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void pictureBox_Manual_DragDrop(object sender, DragEventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_mainform.m_Start_Check && !LVApp.Instance().m_mainform.Force_close)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("[검사중...]에는 수동검사를 할 수 없습니다.", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Can't test during inspection running!", "Notice", 2000);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    AutoClosingMessageBox.Show("检查运行期间无法测试!", "Notice", 2000);
                }
                return;
            }
            foreach (string pic in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                if (pictureBox_Manual.Image != null)
                {
                    pictureBox_Manual.Image = null;
                }
                pictureBox_Manual.BackgroundImage = null;
                // Load bitmap
                ImageInfo imageInfo = null;
                Bitmap t_Image = ImageDecoder.DecodeFromFile(pic, out imageInfo);
                if (t_Image.PixelFormat == PixelFormat.Format32bppRgb)
                {
                    Bitmap tt_Image = ImageDecoder.DecodeFromFile(pic, out imageInfo);
                    t_Image = null;
                    t_Image = ConvertTo24((Bitmap)tt_Image.Clone());
                    tt_Image.Dispose();
                }

                pictureBox_Manual.Image = (System.Drawing.Image)t_Image.Clone();
                propertyGrid1.SelectedObject = imageInfo;
                propertyGrid1.ExpandAllGridItems();

                if (imageInfo.BitsPerPixel == 24 || imageInfo.BitsPerPixel == 32)
                {
                    if (LVApp.Instance().m_Config.m_Cam_Kind[m_Selected_Cam_Num] == 2)
                    {
                        byte[] arr = BmpToArray(t_Image);

                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);

                        if (m_Selected_Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 3, m_Selected_Cam_Num);
                        }
                    }
                    else
                    {
                        Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(t_Image);
                        byte[] arr = BmpToArray(grayImage);
                        //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        if (m_Selected_Cam_Num == 0)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 1)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 2)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }
                        else if (m_Selected_Cam_Num == 3)
                        {
                            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, grayImage.Width, grayImage.Height, 1, m_Selected_Cam_Num);
                        }

                        grayImage.Dispose();
                    }
                } else
                {
                    byte[] arr = BmpToArray(t_Image);
                    //LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    if (m_Selected_Cam_Num == 0)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_0(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 1)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_1(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 2)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_2(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                    else if (m_Selected_Cam_Num == 3)
                    {
                        LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Image_3(arr, t_Image.Width, t_Image.Height, 1, m_Selected_Cam_Num);
                    }
                }
                t_Image.Dispose();
                if (pictureBox_Manual.Image != null)
                {
                    button_Manual_Inspection_Click(sender, e);
                }
            }
        }
    }
}
