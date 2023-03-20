using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Display_2 : UserControl
    {
        public Ctr_Display_2()
        {
            InitializeComponent();
            //digitalClockCtrl1.SetDigitalColor = SriClocks.DigitalColor.GreenColor;
        }

        private void Ctr_Display_2_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() 
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Total_Num == 1)
                        {
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = false;
                            splitContainer3.Panel2MinSize = 0;
                            splitContainer3.SplitterWidth = 1;
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            splitContainer1.SplitterWidth = 1;
                            splitContainer1.SplitterDistance = splitContainer1.Width;
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 2)
                        {
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = false;
                            splitContainer3.Panel2MinSize = 0;
                            splitContainer3.SplitterWidth = 1;
                            splitContainer3.SplitterDistance = splitContainer3.Height;

                            splitContainer2.Panel2MinSize = 0;
                            splitContainer2.SplitterWidth = 1;
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            //if (Properties.Settings.Default.Split_dist >= 0)
                            //{
                            //    splitContainer1.SplitterDistance = Properties.Settings.Default.Split_dist;
                            //}
                            //else
                            //{
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            //}

                            // LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1]_CheckedChanged(sender, e);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 3)
                        {
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = false;
                            splitContainer3.Panel2MinSize = 0;
                            splitContainer3.SplitterWidth = 1;
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            splitContainer1.SplitterWidth = 4;
                            splitContainer2.SplitterWidth = 4;

                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            //if (Properties.Settings.Default.Split_dist >= 0)
                            //{
                            //    splitContainer1.SplitterDistance = Properties.Settings.Default.Split_dist;
                            //}
                            //else
                            //{
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            //}
                            //LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2]_CheckedChanged(sender, e);
                        }
                        else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 4)
                        {
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = true;
                            splitContainer1.SplitterWidth = 2;
                            splitContainer2.SplitterWidth = 2;
                            splitContainer3.SplitterWidth = 2;
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                        }

                        Update_Display();
                    
                    }));
                }
                else
                {
                    if (LVApp.Instance().m_Config.m_Cam_Total_Num == 1)
                    {
                        pictureBox_0.Visible = true;
                        pictureBox_1.Visible = false;
                        pictureBox_2.Visible = false;
                        pictureBox_3.Visible = false;
                        splitContainer3.Panel2MinSize = 0;
                        splitContainer3.SplitterWidth = 1;
                        splitContainer3.SplitterDistance = splitContainer3.Height;
                        splitContainer1.SplitterWidth = 1;
                        splitContainer1.SplitterDistance = splitContainer1.Width;
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 2)
                    {
                        pictureBox_0.Visible = true;
                        pictureBox_1.Visible = true;
                        pictureBox_2.Visible = false;
                        pictureBox_3.Visible = false;
                        splitContainer3.Panel2MinSize = 0;
                        splitContainer3.SplitterWidth = 1;
                        splitContainer3.SplitterDistance = splitContainer3.Height;

                        splitContainer2.Panel2MinSize = 0;
                        splitContainer2.SplitterWidth = 1;
                        splitContainer2.SplitterDistance = splitContainer2.Height;
                        //if (Properties.Settings.Default.Split_dist >= 0)
                        //{
                        //    splitContainer1.SplitterDistance = Properties.Settings.Default.Split_dist;
                        //}
                        //else
                        //{
                        splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                        //}

                        // LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1]_CheckedChanged(sender, e);
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 3)
                    {
                        pictureBox_0.Visible = true;
                        pictureBox_1.Visible = true;
                        pictureBox_2.Visible = true;
                        pictureBox_3.Visible = false;
                        splitContainer3.Panel2MinSize = 0;
                        splitContainer3.SplitterWidth = 1;
                        splitContainer3.SplitterDistance = splitContainer3.Height;
                        splitContainer1.SplitterWidth = 4;
                        splitContainer2.SplitterWidth = 4;

                        splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                        //if (Properties.Settings.Default.Split_dist >= 0)
                        //{
                        //    splitContainer1.SplitterDistance = Properties.Settings.Default.Split_dist;
                        //}
                        //else
                        //{
                        splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                        //}
                        //LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2]_CheckedChanged(sender, e);
                    }
                    else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 4)
                    {
                        pictureBox_0.Visible = true;
                        pictureBox_1.Visible = true;
                        pictureBox_2.Visible = true;
                        pictureBox_3.Visible = true;
                        splitContainer1.SplitterWidth = 2;
                        splitContainer2.SplitterWidth = 2;
                        splitContainer3.SplitterWidth = 2;
                        splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                        splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                        splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                    }

                    Update_Display();
                }
            }
            catch
            {

            }
        }

        public int m_Selected_PictureBox = 0;
        Color Grid_Line_Color = Color.LightGoldenrodYellow;
        Color Selected_color = Color.LawnGreen;
        //private void pictureBox_Main_Paint(object sender, PaintEventArgs e)
        //{
        //    using (Font myFont = new Font("Arial", 11))
        //    {
        //        e.Graphics.DrawString("CAM " + m_Selected_PictureBox.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
        //        //e.Graphics.DrawString(Matching_Result_Str[0], myFont, Brushes.LightGoldenrodYellow, new Point(4, 24));
        //        //Pen pen1 = new Pen(Grid_Line_Color, 1);
        //        //pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //        //Point VP1 = new Point(pictureBox_Main.Width / 2, 0);
        //        //Point VP2 = new Point(pictureBox_Main.Width / 2, pictureBox_Main.Height);
        //        //e.Graphics.DrawLine(pen1, VP1, VP2);
        //        //Point HP1 = new Point(0, pictureBox_Main.Height / 2);
        //        //Point HP2 = new Point(pictureBox_Main.Width, pictureBox_Main.Height / 2);
        //        //e.Graphics.DrawLine(pen1, HP1, HP2);
        //        //if (m_Selected_PictureBox == 2)
        //        //{
        //        //    Pen pen2 = new Pen(Color.HotPink, 1);
        //        //    HP1 = new Point(0, 17 * pictureBox4.Height / 18);
        //        //    HP2 = new Point(pictureBox4.Width, 17 * pictureBox4.Height / 18);
        //        //    e.Graphics.DrawLine(pen2, HP1, HP2);
        //        //    pen2.Dispose();
        //        //}
        //        if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
        //        {
        //            e.Graphics.DrawString("검사중... ", myFont, Brushes.Orange, new Point(pictureBox_Main.Width - 100, 80));
        //        }

        //        //pen1.Dispose();
        //    }
        //    using (Pen pen = new Pen(Color.DimGray, 1))
        //    {
        //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //        e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_Main.Width - 1, pictureBox_Main.Height - 1);
        //    }
        //    if (LVApp.Instance().m_Config.m_Error_Flag[m_Selected_PictureBox] == 1)
        //    {
        //        using (Font myFont = new Font("Arial", 25))
        //        {
        //            e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_Main.Width - 90, 10));
        //        }
        //        //using (Font myFont = new Font("Arial", 9))
        //        //{
        //        //    e.Graphics.DrawString(m_Result_Defect_List[1], myFont, Brushes.OrangeRed, new Point(pictureBox_Main.Width - 90, 50));
        //        //}
        //        using (Font myFont = new Font("Arial", 12))
        //        {
        //            e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[m_Selected_PictureBox], myFont, Brushes.CornflowerBlue, new Point(pictureBox_Main.Width - 90, pictureBox_Main.Height - 25));
        //        }
        //    }
        //    else if (LVApp.Instance().m_Config.m_Error_Flag[m_Selected_PictureBox] == 0)
        //    {
        //        using (Font myFont = new Font("Arial", 25))
        //        {
        //            e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_Main.Width - 85, 10));
        //        }
        //        using (Font myFont = new Font("Arial", 12))
        //        {
        //            e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[m_Selected_PictureBox], myFont, Brushes.CornflowerBlue, new Point(pictureBox_Main.Width - 90, pictureBox_Main.Height - 25));
        //        }
        //    }
        //}

        private void pictureBox_0_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int CamNum = 0;
                using (Font myFont = new Font("Arial", 12))
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[CamNum])
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam1.Camera_Name, myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString() + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam1.Camera_Name + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                }

                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
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
                        t_Width *= 8;

                        using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                        {
                            e.Graphics.FillRectangle(myBrush, new Rectangle(3, pictureBox_0.Height - LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 - 5, t_Width, LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 + 8));
                        }

                        if (t_length > 0)
                        {
                            for (int i = 0; i < t_length; i++)
                            {
                                if (t_str[i].Contains("_OK"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.SkyBlue, new System.Drawing.Point(5, -2 + pictureBox_0.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                                else if (t_str[i].Contains("_NG"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.OrangeRed, new System.Drawing.Point(5, -2 + pictureBox_0.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                            }
                        }
                    }
                }

                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_Config.m_Cam_Kind[CamNum] != 3)
                {
                    using (Font myFont = new Font("Arial", 10))
                    {
                        e.Graphics.DrawString("[" + LVApp.Instance().m_mainform.ctr_Camera_Setting1.Grab_Num.ToString() + "]", myFont, Brushes.LightGoldenrodYellow, new Point(55, 4));
                    }
                }
				
                if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
                {//NG이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 160, pictureBox_0.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.Red, new Point(pictureBox_0.Width / 2 - 40, pictureBox_0.Height / 2));
                        }
                    }
                }
                else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
                {//OK이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 160, pictureBox_0.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width / 2 - 40, pictureBox_0.Height / 2));
                        }
                    }
                }

                
            }
            catch
            {

            }
        }

        private void pictureBox_1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int CamNum = 1;
                using (Font myFont = new Font("Arial", 12))
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[CamNum])
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam2.Camera_Name, myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString() + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam2.Camera_Name + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                }

                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_1.Width - 1, pictureBox_1.Height - 1);
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
                        t_Width *= 8;

                        using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                        {
                            e.Graphics.FillRectangle(myBrush, new Rectangle(3, pictureBox_1.Height - LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 - 5, t_Width, LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 + 8));
                        }

                        if (t_length > 0)
                        {
                            for (int i = 0; i < t_length; i++)
                            {
                                if (t_str[i].Contains("_OK"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.SkyBlue, new System.Drawing.Point(5, -2 + pictureBox_1.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                                else if (t_str[i].Contains("_NG"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.OrangeRed, new System.Drawing.Point(5, -2 + pictureBox_1.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                            }
                        }
                    }
                }

                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_Config.m_Cam_Kind[CamNum] != 3)
                {
                    using (Font myFont = new Font("Arial", 10))
                    {
                        e.Graphics.DrawString("[" + LVApp.Instance().m_mainform.ctr_Camera_Setting2.Grab_Num.ToString() + "]", myFont, Brushes.LightGoldenrodYellow, new Point(55, 4));
                    }
                }

                if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
                {//NG이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_1.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_1.Width - 160, pictureBox_1.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.Red, new Point(pictureBox_1.Width / 2 - 40, pictureBox_1.Height / 2));
                        }
                    }
                }
                else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
                {//OK이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_1.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_1.Width - 160, pictureBox_1.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.SkyBlue, new Point(pictureBox_1.Width / 2 - 40, pictureBox_1.Height / 2));
                        }
                    }
                }


            }
            catch
            {

            }
        }

        private void pictureBox_2_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int CamNum = 2;
                using (Font myFont = new Font("Arial", 12))
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[CamNum])
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam3.Camera_Name, myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString() + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam3.Camera_Name + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                }

                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_2.Width - 1, pictureBox_2.Height - 1);
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
                        t_Width *= 8;

                        using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                        {
                            e.Graphics.FillRectangle(myBrush, new Rectangle(3, pictureBox_2.Height - LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 - 5, t_Width, LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 + 8));
                        }

                        if (t_length > 0)
                        {
                            for (int i = 0; i < t_length; i++)
                            {
                                if (t_str[i].Contains("_OK"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.SkyBlue, new System.Drawing.Point(5, -2 + pictureBox_2.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                                else if (t_str[i].Contains("_NG"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.OrangeRed, new System.Drawing.Point(5, -2 + pictureBox_2.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                            }
                        }
                    }
                }

                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_Config.m_Cam_Kind[CamNum] != 3)
                {
                    using (Font myFont = new Font("Arial", 10))
                    {
                        e.Graphics.DrawString("[" + LVApp.Instance().m_mainform.ctr_Camera_Setting3.Grab_Num.ToString() + "]", myFont, Brushes.LightGoldenrodYellow, new Point(55, 4));
                    }
                }
				
                if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
                {//NG이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_2.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_2.Width - 160, pictureBox_2.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.Red, new Point(pictureBox_2.Width / 2 - 40, pictureBox_2.Height / 2));
                        }
                    }
                }
                else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
                {//OK이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_2.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_2.Width - 160, pictureBox_2.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.SkyBlue, new Point(pictureBox_2.Width / 2 - 40, pictureBox_2.Height / 2));
                        }
                    }
                }

                
            }
            catch
            {

            }
        }

        private void pictureBox_3_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int CamNum = 3;
                using (Font myFont = new Font("Arial", 12))
                {
                    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[CamNum])
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam4.Camera_Name, myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                    else
                    {
                        if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                        {
                            e.Graphics.DrawString("PROBE " + CamNum.ToString() + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                        else
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_mainform.ctrCam4.Camera_Name + " turn off", myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                        }
                    }
                }

                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_3.Width - 1, pictureBox_3.Height - 1);
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
                        t_Width *= 8;

                        using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                        {
                            e.Graphics.FillRectangle(myBrush, new Rectangle(3, pictureBox_3.Height - LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 - 5, t_Width, LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] * 15 + 8));
                        }

                        if (t_length > 0)
                        {
                            for (int i = 0; i < t_length; i++)
                            {
                                if (t_str[i].Contains("_OK"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.SkyBlue, new System.Drawing.Point(5, -2 + pictureBox_3.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                                else if (t_str[i].Contains("_NG"))
                                {
                                    using (Font myFont = new Font("Arial", 8))
                                    {
                                        e.Graphics.DrawString(t_str[i], myFont, Brushes.OrangeRed, new System.Drawing.Point(5, -2 + pictureBox_3.Height - (LVApp.Instance().m_Config.Disp_OKNG_List_CNT[CamNum] - i) * 15));
                                    }
                                }
                            }
                        }
                    }
                }

                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode && LVApp.Instance().m_Config.m_Cam_Kind[CamNum] != 3)
                {
                    using (Font myFont = new Font("Arial", 10))
                    {
                        e.Graphics.DrawString("[" + LVApp.Instance().m_mainform.ctr_Camera_Setting4.Grab_Num.ToString() + "]", myFont, Brushes.LightGoldenrodYellow, new Point(55, 4));
                    }
                }
				
                if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
                {//NG이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_3.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_3.Width - 160, pictureBox_3.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.Red, new Point(pictureBox_3.Width / 2 - (LVApp.Instance().m_Config.m_Probe[CamNum].Length*7), pictureBox_3.Height / 2));
                        }
                    }
                }
                else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
                {//OK이면
                    using (Font myFont = new Font("Arial", 30))
                    {
                        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_3.Width - 96, 15));
                    }
                    using (Font myFont = new Font("Arial", 9))
                    {
                        e.Graphics.DrawString(LVApp.Instance().m_Config.m_FPS[CamNum] + ", " + LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_3.Width - 160, pictureBox_3.Height - 25));
                    }
                    if (LVApp.Instance().m_Config.m_Cam_Kind[CamNum] == 3)
                    {
                        using (Font myFont = new Font("Arial", 20))
                        {
                            e.Graphics.DrawString(LVApp.Instance().m_Config.m_Probe[CamNum], myFont, Brushes.SkyBlue, new Point(pictureBox_3.Width / 2 -(LVApp.Instance().m_Config.m_Probe[CamNum].Length * 7), pictureBox_3.Height / 2));
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void pictureBox_4_Paint(object sender, PaintEventArgs e)
        {
            return;
            //int CamNum = 4;
            //using (Font myFont = new Font("Arial", 10))
            //{
            //    e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            //}
            //if (m_Selected_PictureBox == CamNum)
            //{
            //    using (Pen pen = new Pen(Selected_color, 2))
            //    {
            //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            //        e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
            //    }
            //}
            //else
            //{
            //    using (Pen pen = new Pen(Color.DimGray, 1))
            //    {
            //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            //        e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
            //    }
            //}
            //if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    //using (Font myFont = new Font("Arial", 9))
            //    //{
            //    //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
            //    //}
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
            //else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
        }

        private void pictureBox_5_Paint(object sender, PaintEventArgs e)
        {
            return;
            //int CamNum = 5;
            //using (Font myFont = new Font("Arial", 10))
            //{
            //    e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            //}
            //if (m_Selected_PictureBox == CamNum)
            //{
            //    using (Pen pen = new Pen(Selected_color, 2))
            //    {
            //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            //        e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
            //    }
            //}
            //else
            //{
            //    using (Pen pen = new Pen(Color.DimGray, 1))
            //    {
            //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            //        e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
            //    }
            //}
            //if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    //using (Font myFont = new Font("Arial", 9))
            //    //{
            //    //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
            //    //}
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
            //else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
        }

        private void pictureBox_6_Paint(object sender, PaintEventArgs e)
        {
            return;
            //int CamNum = 6;
            //using (Font myFont = new Font("Arial", 10))
            //{
            //    e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            //}
            //if (m_Selected_PictureBox == CamNum)
            //{
            //    using (Pen pen = new Pen(Selected_color, 2))
            //    {
            //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            //        e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
            //    }
            //}
            //else
            //{
            //    using (Pen pen = new Pen(Color.DimGray, 1))
            //    {
            //        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            //        e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
            //    }
            //}
            //if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    //using (Font myFont = new Font("Arial", 9))
            //    //{
            //    //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
            //    //}
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
            //else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
        }

        private void pictureBox_7_Paint(object sender, PaintEventArgs e)
        {
            return;
            //int CamNum = 7;
            //using (Font myFont = new Font("Arial", 10))
            //{
            //    e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            //}
            ////if (m_Selected_PictureBox == CamNum)
            ////{
            ////    using (Pen pen = new Pen(Selected_color, 2))
            ////    {
            ////        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            ////        e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
            ////    }
            ////}
            ////else
            ////{
            ////    using (Pen pen = new Pen(Color.DimGray, 1))
            ////    {
            ////        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            ////        e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
            ////    }
            ////}
            //if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    //using (Font myFont = new Font("Arial", 9))
            //    //{
            //    //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
            //    //}
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
            //else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            //{
            //    using (Font myFont = new Font("Arial", 12))
            //    {
            //        e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
            //    }
            //    using (Font myFont = new Font("Arial", 9))
            //    {
            //        e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
            //    }
            //}
        }

        private void pictureBox_0_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_0.Image != null)
                {
                    ContextMenu cm = new ContextMenu();

                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("결과 이미지 On/Off", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 화면 맞추기 On/Off", new EventHandler(PictureBoxviewMode0));
                        cm.MenuItems.Add("이미지 팝업창", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("이미지 저장", new EventHandler(PictureBoxSave0));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Fit screen On/Off", new EventHandler(PictureBoxviewMode0));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave0));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("适合屏幕 On/Off", new EventHandler(PictureBoxviewMode0));
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupView0));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave0));
                    }

                    //cm.MenuItems.Add("Popup View", new EventHandler(PictureBoxPopupViewMain));
                    //cm.MenuItems.Add("Result View", new EventHandler(PictureBoxResultviewMain));
                    //cm.MenuItems.Add("Save", new EventHandler(PictureBoxSaveMain));
                    pictureBox_0.ContextMenu = cm;
                    pictureBox_0.ContextMenu.Show(pictureBox_0, e.Location);
                    pictureBox_0.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //pictureBox_0.ContextMenu = cm;
                    //pictureBox_0.ContextMenu.Show(pictureBox_0, e.Location);
                    //pictureBox_0.ContextMenu = null;
                }
            }
        }

        private void PictureBoxviewMode0(object sender, EventArgs e)
        {
            if (pictureBox_0.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                pictureBox_0.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox_0.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void PictureBoxviewMode1(object sender, EventArgs e)
        {
            if (pictureBox_1.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                pictureBox_1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox_1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void PictureBoxviewMode2(object sender, EventArgs e)
        {
            if (pictureBox_2.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                pictureBox_2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox_2.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void PictureBoxviewMode3(object sender, EventArgs e)
        {
            if (pictureBox_3.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                pictureBox_3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox_3.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void PictureBoxPopupView0(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (Image)pictureBox_0.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSave0(object sender, EventArgs e)
        {
            m_Selected_PictureBox = 0;
            using (Image bmp = (Image)pictureBox_0.Image.Clone())
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

        private void PictureBoxPopupView1(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (Image)pictureBox_1.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSave1(object sender, EventArgs e)
        {
            m_Selected_PictureBox = 1;
            using (Image bmp = (Image)pictureBox_1.Image.Clone())
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

        private void PictureBoxPopupView2(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (Image)pictureBox_2.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSave2(object sender, EventArgs e)
        {
            m_Selected_PictureBox = 2;
            using (Image bmp = (Image)pictureBox_2.Image.Clone())
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

        private void PictureBoxPopupView3(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (Image)pictureBox_3.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSave3(object sender, EventArgs e)
        {
            m_Selected_PictureBox = 3;
            using (Image bmp = (Image)pictureBox_3.Image.Clone())
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

        private void pictureBox_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_1.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("결과 이미지 On/Off", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 화면 맞추기 On/Off", new EventHandler(PictureBoxviewMode1));
                        cm.MenuItems.Add("이미지 팝업창", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("이미지 저장", new EventHandler(PictureBoxSave1));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Fit screen On/Off", new EventHandler(PictureBoxviewMode1));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave1));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("适合屏幕 On/Off", new EventHandler(PictureBoxviewMode1));
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupView1));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave1));
                    }


                    //cm.MenuItems.Add("Popup View", new EventHandler(PictureBoxPopupViewMain));
                    //cm.MenuItems.Add("Result View", new EventHandler(PictureBoxResultviewMain));
                    //cm.MenuItems.Add("Save", new EventHandler(PictureBoxSaveMain));
                    pictureBox_1.ContextMenu = cm;
                    pictureBox_1.ContextMenu.Show(pictureBox_1, e.Location);
                    pictureBox_1.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //pictureBox_1.ContextMenu = cm;
                    //pictureBox_1.ContextMenu.Show(pictureBox_1, e.Location);
                    //pictureBox_1.ContextMenu = null;
                }
            }
        }

        private void pictureBox_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_2.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("결과 이미지 On/Off", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 화면 맞추기 On/Off", new EventHandler(PictureBoxviewMode2));
                        cm.MenuItems.Add("이미지 팝업창", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("이미지 저장", new EventHandler(PictureBoxSave2));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Fit screen On/Off", new EventHandler(PictureBoxviewMode2));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave2));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("适合屏幕 On/Off", new EventHandler(PictureBoxviewMode2));
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupView2));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave2));
                    }

                    //cm.MenuItems.Add("Popup View", new EventHandler(PictureBoxPopupViewMain));
                    //cm.MenuItems.Add("Result View", new EventHandler(PictureBoxResultviewMain));
                    //cm.MenuItems.Add("Save", new EventHandler(PictureBoxSaveMain));
                    pictureBox_2.ContextMenu = cm;
                    pictureBox_2.ContextMenu.Show(pictureBox_2, e.Location);
                    pictureBox_2.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //pictureBox_2.ContextMenu = cm;
                    //pictureBox_2.ContextMenu.Show(pictureBox_2, e.Location);
                    //pictureBox_2.ContextMenu = null;
                }
            }
        }

        //private void pictureBox_3_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button.ToString() == "Left")
        //    {
        //        if (m_Selected_PictureBox == 3)
        //        {
        //            //m_Selected_PictureBox = -1;
        //            //pictureBox_1.ContextMenu = null;
        //        }
        //        else
        //        {
        //            m_Selected_PictureBox = 3;
        //            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
        //        }
        //        Update_Main_Image(-1);
        //    }
        //}

        //private void pictureBox_4_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button.ToString() == "Left")
        //    {
        //        if (m_Selected_PictureBox == 4)
        //        {
        //            //m_Selected_PictureBox = -1;
        //            //pictureBox_1.ContextMenu = null;
        //        }
        //        else
        //        {
        //            m_Selected_PictureBox = 4;
        //            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM4.Checked = true;
        //        }
        //        Update_Main_Image(-1);
        //    }
        //}

        //private void pictureBox_5_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button.ToString() == "Left")
        //    {
        //        if (m_Selected_PictureBox == 5)
        //        {
        //            //m_Selected_PictureBox = -1;
        //            //pictureBox_1.ContextMenu = null;
        //        }
        //        else
        //        {
        //            m_Selected_PictureBox = 5;
        //            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM5.Checked = true;
        //        }
        //        Update_Main_Image(-1);
        //    }
        //}

        //private void pictureBox_6_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button.ToString() == "Left")
        //    {
        //        if (m_Selected_PictureBox == 6)
        //        {
        //            //m_Selected_PictureBox = -1;
        //            //pictureBox_1.ContextMenu = null;
        //        }
        //        else
        //        {
        //            m_Selected_PictureBox = 6;
        //            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM6.Checked = true;
        //        }
        //        Update_Main_Image(-1);
        //    }
        //}

        //private void pictureBox_7_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button.ToString() == "Left")
        //    {
        //        if (m_Selected_PictureBox == 7)
        //        {
        //            //m_Selected_PictureBox = -1;
        //            //pictureBox_1.ContextMenu = null;
        //        }
        //        else
        //        {
        //            m_Selected_PictureBox = 7;
        //            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM7.Checked = true;
        //        }
        //        Update_Main_Image(-1);
        //    }
        //}

        //public void Update_Main_Image(int Cam_num)
        //{
        //    if (m_Selected_PictureBox == 0 && Cam_num == 0)
        //    {
        //        if (pictureBox_0.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_0.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 1 && Cam_num == 0)
        //    {
        //        if (pictureBox_1.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_1.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 2 && Cam_num == 0)
        //    {
        //        if (pictureBox_2.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_2.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 3 && Cam_num == 0)
        //    {
        //        if (pictureBox_3.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_3.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 4 && Cam_num == 0)
        //    {
        //        if (pictureBox_4.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_4.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 5 && Cam_num == 0)
        //    {
        //        if (pictureBox_5.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_5.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 6 && Cam_num == 0)
        //    {
        //        if (pictureBox_6.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_6.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }
        //    else if (m_Selected_PictureBox == 7 && Cam_num == 0)
        //    {
        //        if (pictureBox_7.Image != null)
        //        {
        //            pictureBox_Main.Image = (System.Drawing.Image)pictureBox_7.Image.Clone();
        //            pictureBox_Main.Refresh();
        //        }
        //    }

        //    if (Cam_num == -1)
        //    {
        //        if (m_Selected_PictureBox == 0)
        //        {
        //            if (pictureBox_0.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_0.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 1)
        //        {
        //            if (pictureBox_1.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_1.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 2)
        //        {
        //            if (pictureBox_2.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_2.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 3)
        //        {
        //            if (pictureBox_3.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_3.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 4)
        //        {
        //            if (pictureBox_4.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_4.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 5)
        //        {
        //            if (pictureBox_5.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_5.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 6)
        //        {
        //            if (pictureBox_6.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_6.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        else if (m_Selected_PictureBox == 7)
        //        {
        //            if (pictureBox_7.Image != null)
        //            {
        //                pictureBox_Main.Image = (System.Drawing.Image)pictureBox_7.Image.Clone();
        //                pictureBox_Main.Refresh();
        //            }
        //        }
        //        this.Refresh();
        //    }
        //}

        //private void PictureBoxPopupViewMain(object sender, EventArgs e)
        //{
        //    Form_BigImage View_form = new Form_BigImage();
        //    View_form.imageControl1.Image = (Image)pictureBox_Main.Image.Clone();
        //    View_form.Show();
        //}

        //private void PictureBoxSaveMain(object sender, EventArgs e)
        //{
        //    using (Image bmp = (Image)pictureBox_Main.Image.Clone())
        //    {
        //        if (bmp == null)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            Image_SaveFileDialog(bmp);
        //        }
        //    }
        //}

        public void Image_SaveFileDialog(Image bmp)
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
            SaveFileDialog1.FileName = "Save_" + m_Selected_PictureBox.ToString() + ".bmp";
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

        //private void pictureBox_Main_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button.ToString() == "Left")
        //    {

        //    }
        //    else
        //    {
        //        if (pictureBox_Main.Image != null)
        //        {
        //            ContextMenu cm = new ContextMenu();
        //            cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
        //            cm.MenuItems.Add("결과 이미지 On/Off", new EventHandler(PictureBoxResultviewMain));
        //            cm.MenuItems.Add("이미지 팝업창", new EventHandler(PictureBoxPopupViewMain));
        //            cm.MenuItems.Add("이미지 저장", new EventHandler(PictureBoxSaveMain));


        //            //cm.MenuItems.Add("Popup View", new EventHandler(PictureBoxPopupViewMain));
        //            //cm.MenuItems.Add("Result View", new EventHandler(PictureBoxResultviewMain));
        //            //cm.MenuItems.Add("Save", new EventHandler(PictureBoxSaveMain));
        //            pictureBox_Main.ContextMenu = cm;
        //            pictureBox_Main.ContextMenu.Show(pictureBox_Main, e.Location);
        //            pictureBox_Main.ContextMenu = null;
        //        }
        //        else
        //        {
        //            ContextMenu cm = new ContextMenu();
        //            cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
        //            pictureBox_Main.ContextMenu = cm;
        //            pictureBox_Main.ContextMenu.Show(pictureBox_Main, e.Location);
        //            pictureBox_Main.ContextMenu = null;
        //        }
        //    }
        //}

        private void PictureBoxRealtimeviewMain(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.Realtime_View_Check)
            {
                LVApp.Instance().m_Config.Realtime_View_Check = false;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_Display.Checked = false;
            }
            else
            {
                LVApp.Instance().m_Config.Realtime_View_Check = true;
                LVApp.Instance().m_mainform.ctr_Log1.checkBox_Display.Checked = true;
            }
            LVApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);
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
            LVApp.Instance().m_mainform.ctr_Log1.button_LOGSAVE_Click(sender, e);
            LVApp.Instance().m_mainform.m_ImProClr_Class.Set_Global_Parameters(LVApp.Instance().m_Config.Alg_TextView, LVApp.Instance().m_Config.Alg_Debugging);
        }

        public void Update_Display()
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() 
                        {
                            bool[] C_Cam = new bool[4];
                            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                            {
                                //pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                                pictureBox_0.BackgroundImage = null;
                                pictureBox_0.Image = null;
                                C_Cam[0] = false;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[0].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[0].Enabled = false;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[0].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[0].Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM0.Enabled = false;
                            }
                            else
                            {
                                pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                                C_Cam[0] = true;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[0].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[0].Enabled = true;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[0].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[0].Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM0.Enabled = true;
                            }

                            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                            {
                                pictureBox_1.BackgroundImage = null;
                                pictureBox_1.Image = null;
                                C_Cam[1] = false;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[1].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[1].Enabled = false;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[1].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[1].Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM1.Enabled = false;
                            }
                            else
                            {
                                pictureBox_1.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                                C_Cam[1] = true;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[1].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[1].Enabled = true;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[1].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[1].Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM1.Enabled = true;
                            }

                            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                            {
                                pictureBox_2.BackgroundImage = null;
                                pictureBox_2.Image = null;
                                C_Cam[2] = false;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[2].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[2].Enabled = false;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[2].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[2].Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM2.Enabled = false;
                            }
                            else
                            {
                                pictureBox_2.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                                C_Cam[2] = true;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[2].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[2].Enabled = true;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[2].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[2].Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM2.Enabled = true;
                            }

                            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                            {
                                pictureBox_3.BackgroundImage = null;
                                pictureBox_3.Image = null;
                                C_Cam[3] = false;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[3].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[3].Enabled = false;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[3].Enabled = false;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[3].Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Enabled = false;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM3.Enabled = false;
                            }
                            else
                            {
                                pictureBox_3.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                                C_Cam[3] = true;
                                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[3].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[3].Enabled = true;
                                //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[3].Enabled = true;
                                LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[3].Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Enabled = true;
                                LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM3.Enabled = true;
                            }

                            splitContainer1.Panel1MinSize = 0;
                            splitContainer1.Panel2MinSize = 0;
                            splitContainer2.Panel1MinSize = 0;
                            splitContainer2.Panel2MinSize = 0;
                            splitContainer3.Panel1MinSize = 0;
                            splitContainer3.Panel2MinSize = 0;
                            splitContainer1.SplitterWidth = 1;
                            splitContainer2.SplitterWidth = 1;
                            splitContainer3.SplitterWidth = 1;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = false;
                            splitContainer3.IsSplitterFixed = false;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_2);
                            splitContainer2.Panel1.Controls.Add(pictureBox_1);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);

                            if (C_Cam[0] && !C_Cam[1] && !C_Cam[2] && !C_Cam[3])
                            {//0번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = true;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (!C_Cam[0] && C_Cam[1] && !C_Cam[2] && !C_Cam[3])
                            {//1번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                                // 세로 패널
                                splitContainer1.SplitterDistance = 0;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = true;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (!C_Cam[0] && !C_Cam[1] && C_Cam[2] && !C_Cam[3])
                            {//2번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = 0;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = true;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (!C_Cam[0] && !C_Cam[1] && !C_Cam[2] && C_Cam[3])
                            {//3번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                                // 세로 패널
                                splitContainer1.SplitterDistance = 0;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = 0;
                                splitContainer1.IsSplitterFixed = true;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (C_Cam[0] && C_Cam[1] && !C_Cam[2] && !C_Cam[3])
                            {//0,1번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (C_Cam[0] && !C_Cam[1] && C_Cam[2] && !C_Cam[3])
                            {//0,2번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_0);
                                splitContainer3.Panel2.Controls.Add(pictureBox_1);
                                splitContainer2.Panel1.Controls.Add(pictureBox_2);
                                splitContainer2.Panel2.Controls.Add(pictureBox_3);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (C_Cam[0] && !C_Cam[1] && !C_Cam[2] && C_Cam[3])
                            {//0,3번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_0);
                                splitContainer3.Panel2.Controls.Add(pictureBox_2);
                                splitContainer2.Panel1.Controls.Add(pictureBox_3);
                                splitContainer2.Panel2.Controls.Add(pictureBox_1);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (!C_Cam[0] && C_Cam[1] && C_Cam[2] && !C_Cam[3])
                            {//1,2번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_1);
                                splitContainer3.Panel2.Controls.Add(pictureBox_0);
                                splitContainer2.Panel1.Controls.Add(pictureBox_2);
                                splitContainer2.Panel2.Controls.Add(pictureBox_3);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (!C_Cam[0] && C_Cam[1] && !C_Cam[2] && C_Cam[3])
                            {//1,3번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_1);
                                splitContainer3.Panel2.Controls.Add(pictureBox_0);
                                splitContainer2.Panel1.Controls.Add(pictureBox_3);
                                splitContainer2.Panel2.Controls.Add(pictureBox_2);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (!C_Cam[0] && !C_Cam[1] && C_Cam[2] && C_Cam[3])
                            {//2,3번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = true;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_2);
                                splitContainer3.Panel2.Controls.Add(pictureBox_0);
                                splitContainer2.Panel1.Controls.Add(pictureBox_3);
                                splitContainer2.Panel2.Controls.Add(pictureBox_1);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (C_Cam[0] && C_Cam[1] && C_Cam[2] && !C_Cam[3])
                            {//0,1,2번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = false;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = false;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_0);
                                splitContainer3.Panel2.Controls.Add(pictureBox_3);
                                splitContainer2.Panel1.Controls.Add(pictureBox_1);
                                splitContainer2.Panel2.Controls.Add(pictureBox_2);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                            }
                            else if (C_Cam[0] && C_Cam[1] && !C_Cam[2] && C_Cam[3])
                            {//0,1,3번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = false;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = false;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_0);
                                splitContainer3.Panel2.Controls.Add(pictureBox_2);
                                splitContainer2.Panel1.Controls.Add(pictureBox_1);
                                splitContainer2.Panel2.Controls.Add(pictureBox_3);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (C_Cam[0] && !C_Cam[1] && C_Cam[2] && C_Cam[3])
                            {//0,2,3번만
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = false;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = false;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_0);
                                splitContainer3.Panel2.Controls.Add(pictureBox_1);
                                splitContainer2.Panel1.Controls.Add(pictureBox_2);
                                splitContainer2.Panel2.Controls.Add(pictureBox_3);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (!C_Cam[0] && C_Cam[1] && C_Cam[2] && C_Cam[3])
                            {//1,2,3번만
                                pictureBox_0.Visible = false;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = false;
                                splitContainer3.IsSplitterFixed = true;
                                splitContainer3.Panel1.Controls.Add(pictureBox_1);
                                splitContainer3.Panel2.Controls.Add(pictureBox_0);
                                splitContainer2.Panel1.Controls.Add(pictureBox_2);
                                splitContainer2.Panel2.Controls.Add(pictureBox_3);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                            else if (C_Cam[0] && C_Cam[1] && C_Cam[2] && C_Cam[3])
                            {//0,1,2,3번
                                pictureBox_0.Visible = true;
                                pictureBox_1.Visible = true;
                                pictureBox_2.Visible = true;
                                pictureBox_3.Visible = true;
                                // 왼쪽 패널
                                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                                // 세로 패널
                                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                                // 오른쪽 패널
                                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                                splitContainer1.IsSplitterFixed = false;
                                splitContainer2.IsSplitterFixed = false;
                                splitContainer3.IsSplitterFixed = false;
                                splitContainer3.Panel1.Controls.Add(pictureBox_0);
                                splitContainer3.Panel2.Controls.Add(pictureBox_2);
                                splitContainer2.Panel1.Controls.Add(pictureBox_1);
                                splitContainer2.Panel2.Controls.Add(pictureBox_3);
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                                LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                            }
                        }));
                }
                else
                {
                    if (LVApp.Instance().m_mainform == null)
                    {
                        return;
                    }
                    {
                        bool[] C_Cam = new bool[4];
                        if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                        {
                            //pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                            pictureBox_0.BackgroundImage = null;
                            pictureBox_0.Image = null;
                            C_Cam[0] = false;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[0].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[0].Enabled = false;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[0].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[0].Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM0.Enabled = false;
                        }
                        else
                        {
                            pictureBox_0.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                            C_Cam[0] = true;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[0].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[0].Enabled = true;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[0].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[0].Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM0.Enabled = true;
                        }

                        if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                        {
                            pictureBox_1.BackgroundImage = null;
                            pictureBox_1.Image = null;
                            C_Cam[1] = false;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[1].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[1].Enabled = false;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[1].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[1].Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM1.Enabled = false;
                        }
                        else
                        {
                            pictureBox_1.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                            C_Cam[1] = true;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[1].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[1].Enabled = true;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[1].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[1].Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM1.Enabled = true;
                        }

                        if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                        {
                            pictureBox_2.BackgroundImage = null;
                            pictureBox_2.Image = null;
                            C_Cam[2] = false;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[2].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[2].Enabled = false;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[2].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[2].Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM2.Enabled = false;
                        }
                        else
                        {
                            pictureBox_2.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                            C_Cam[2] = true;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[2].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[2].Enabled = true;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[2].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[2].Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM2.Enabled = true;
                        }

                        if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                        {
                            pictureBox_3.BackgroundImage = null;
                            pictureBox_3.Image = null;
                            C_Cam[3] = false;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[3].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[3].Enabled = false;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[3].Enabled = false;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[3].Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Enabled = false;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM3.Enabled = false;
                        }
                        else
                        {
                            pictureBox_3.BackgroundImage = global::LV_Inspection_System.Properties.Resources.Display;
                            C_Cam[3] = true;
                            LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING_CAM.TabPages[3].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow_ALG.TabPages[3].Enabled = true;
                            //LVApp.Instance().m_mainform.neoTabWindow1.TabPages[3].Enabled = true;
                            LVApp.Instance().m_mainform.neoTabWindow2_LOG.TabPages[3].Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Enabled = true;
                            LVApp.Instance().m_mainform.ctr_Log1.checkBox_CAM3.Enabled = true;
                        }

                        splitContainer1.Panel1MinSize = 0;
                        splitContainer1.Panel2MinSize = 0;
                        splitContainer2.Panel1MinSize = 0;
                        splitContainer2.Panel2MinSize = 0;
                        splitContainer3.Panel1MinSize = 0;
                        splitContainer3.Panel2MinSize = 0;
                        splitContainer1.SplitterWidth = 1;
                        splitContainer2.SplitterWidth = 1;
                        splitContainer3.SplitterWidth = 1;
                        splitContainer1.IsSplitterFixed = false;
                        splitContainer2.IsSplitterFixed = false;
                        splitContainer3.IsSplitterFixed = false;
                        splitContainer3.Panel1.Controls.Add(pictureBox_0);
                        splitContainer3.Panel2.Controls.Add(pictureBox_2);
                        splitContainer2.Panel1.Controls.Add(pictureBox_1);
                        splitContainer2.Panel2.Controls.Add(pictureBox_3);

                        if (C_Cam[0] && !C_Cam[1] && !C_Cam[2] && !C_Cam[3])
                        {//0번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = true;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (!C_Cam[0] && C_Cam[1] && !C_Cam[2] && !C_Cam[3])
                        {//1번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                            // 세로 패널
                            splitContainer1.SplitterDistance = 0;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = true;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (!C_Cam[0] && !C_Cam[1] && C_Cam[2] && !C_Cam[3])
                        {//2번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = 0;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = true;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (!C_Cam[0] && !C_Cam[1] && !C_Cam[2] && C_Cam[3])
                        {//3번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                            // 세로 패널
                            splitContainer1.SplitterDistance = 0;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = 0;
                            splitContainer1.IsSplitterFixed = true;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (C_Cam[0] && C_Cam[1] && !C_Cam[2] && !C_Cam[3])
                        {//0,1번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (C_Cam[0] && !C_Cam[1] && C_Cam[2] && !C_Cam[3])
                        {//0,2번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_1);
                            splitContainer2.Panel1.Controls.Add(pictureBox_2);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (C_Cam[0] && !C_Cam[1] && !C_Cam[2] && C_Cam[3])
                        {//0,3번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_2);
                            splitContainer2.Panel1.Controls.Add(pictureBox_3);
                            splitContainer2.Panel2.Controls.Add(pictureBox_1);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (!C_Cam[0] && C_Cam[1] && C_Cam[2] && !C_Cam[3])
                        {//1,2번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_1);
                            splitContainer3.Panel2.Controls.Add(pictureBox_0);
                            splitContainer2.Panel1.Controls.Add(pictureBox_2);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (!C_Cam[0] && C_Cam[1] && !C_Cam[2] && C_Cam[3])
                        {//1,3번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_1);
                            splitContainer3.Panel2.Controls.Add(pictureBox_0);
                            splitContainer2.Panel1.Controls.Add(pictureBox_3);
                            splitContainer2.Panel2.Controls.Add(pictureBox_2);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (!C_Cam[0] && !C_Cam[1] && C_Cam[2] && C_Cam[3])
                        {//2,3번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = true;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_2);
                            splitContainer3.Panel2.Controls.Add(pictureBox_0);
                            splitContainer2.Panel1.Controls.Add(pictureBox_3);
                            splitContainer2.Panel2.Controls.Add(pictureBox_1);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (C_Cam[0] && C_Cam[1] && C_Cam[2] && !C_Cam[3])
                        {//0,1,2번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = false;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = false;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_3);
                            splitContainer2.Panel1.Controls.Add(pictureBox_1);
                            splitContainer2.Panel2.Controls.Add(pictureBox_2);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = false;
                        }
                        else if (C_Cam[0] && C_Cam[1] && !C_Cam[2] && C_Cam[3])
                        {//0,1,3번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = false;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = false;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_2);
                            splitContainer2.Panel1.Controls.Add(pictureBox_1);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (C_Cam[0] && !C_Cam[1] && C_Cam[2] && C_Cam[3])
                        {//0,2,3번만
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = false;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = false;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_1);
                            splitContainer2.Panel1.Controls.Add(pictureBox_2);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (!C_Cam[0] && C_Cam[1] && C_Cam[2] && C_Cam[3])
                        {//1,2,3번만
                            pictureBox_0.Visible = false;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = false;
                            splitContainer3.IsSplitterFixed = true;
                            splitContainer3.Panel1.Controls.Add(pictureBox_1);
                            splitContainer3.Panel2.Controls.Add(pictureBox_0);
                            splitContainer2.Panel1.Controls.Add(pictureBox_2);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = false;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                        else if (C_Cam[0] && C_Cam[1] && C_Cam[2] && C_Cam[3])
                        {//0,1,2,3번
                            pictureBox_0.Visible = true;
                            pictureBox_1.Visible = true;
                            pictureBox_2.Visible = true;
                            pictureBox_3.Visible = true;
                            // 왼쪽 패널
                            splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                            // 세로 패널
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                            // 오른쪽 패널
                            splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                            splitContainer1.IsSplitterFixed = false;
                            splitContainer2.IsSplitterFixed = false;
                            splitContainer3.IsSplitterFixed = false;
                            splitContainer3.Panel1.Controls.Add(pictureBox_0);
                            splitContainer3.Panel2.Controls.Add(pictureBox_2);
                            splitContainer2.Panel1.Controls.Add(pictureBox_1);
                            splitContainer2.Panel2.Controls.Add(pictureBox_3);
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 1].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[0 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[1 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[2 * 3 + 2].Visible = true;
                            LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows[3 * 3 + 2].Visible = true;
                        }
                    }

                }

                int t_cnt = 0;
                foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows)
                {
                    if (row.Visible)
                    {
                        t_cnt++;
                    }
                }
                if (t_cnt > 0)
                {
                    foreach (DataGridViewRow row in LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Rows)
                    {
                        row.Height = (LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.Height - LVApp.Instance().m_mainform.dataGridView_AUTO_COUNT.ColumnHeadersHeight) / t_cnt;
                    }
                }
            }
            catch
            {
            }

            return;
            //try
            //{
            //    if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0]
            //        && LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1]
            //        && LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
            //    {
            //        pictureBox_0.Visible = true;
            //        pictureBox_1.Visible = false;
            //        pictureBox_2.Visible = false;
            //        splitContainer3.Panel2MinSize = 0;
            //        splitContainer3.SplitterWidth = 1;
            //        splitContainer3.SplitterDistance = splitContainer3.Height;
            //        splitContainer1.SplitterWidth = 1;
            //        splitContainer1.SplitterDistance = splitContainer1.Width;
            //    }
            //    else if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0]
            //        && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1]
            //        && LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
            //    {
            //        pictureBox_0.Visible = true;
            //        pictureBox_1.Visible = true;
            //        pictureBox_2.Visible = false;
            //        splitContainer3.Panel2MinSize = 0;
            //        splitContainer3.SplitterWidth = 1;
            //        splitContainer3.SplitterDistance = splitContainer3.Height;

            //        splitContainer2.Panel2MinSize = 0;
            //        splitContainer2.SplitterWidth = 1;
            //        splitContainer2.SplitterDistance = splitContainer2.Height;
            //        splitContainer1.SplitterDistance = splitContainer1.Width / 2;
            //    }
            //    else if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0]
            //        && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1]
            //        && !LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
            //    {
            //        pictureBox_0.Visible = true;
            //        pictureBox_1.Visible = true;
            //        pictureBox_2.Visible = true;
            //        splitContainer3.Panel2MinSize = 0;
            //        splitContainer3.SplitterWidth = 1;
            //        splitContainer3.SplitterDistance = splitContainer3.Height;
            //        splitContainer1.SplitterWidth = 4;
            //        splitContainer2.SplitterWidth = 4;

            //        splitContainer2.SplitterDistance = splitContainer2.Height / 2;
            //        splitContainer1.SplitterDistance = splitContainer1.Width / 2;
            //    }
            //}
            //catch
            //{
            //}
        }

        private void pictureBox_3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_3.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        //cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("결과 이미지 On/Off", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("이미지 화면 맞추기 On/Off", new EventHandler(PictureBoxviewMode3));
                        cm.MenuItems.Add("이미지 팝업창", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("이미지 저장", new EventHandler(PictureBoxSave3));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        //cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                        //cm.MenuItems.Add("Result view", new EventHandler(PictureBoxResultviewMain));
                        cm.MenuItems.Add("Fit screen On/Off", new EventHandler(PictureBoxviewMode3));
                        cm.MenuItems.Add("Popup image", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("Image save", new EventHandler(PictureBoxSave3));
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//중국어
                        cm.MenuItems.Add("适合屏幕 On/Off", new EventHandler(PictureBoxviewMode3));
                        cm.MenuItems.Add("弹出图像", new EventHandler(PictureBoxPopupView3));
                        cm.MenuItems.Add("图像保存", new EventHandler(PictureBoxSave3));
                    }

                    //cm.MenuItems.Add("Popup View", new EventHandler(PictureBoxPopupViewMain));
                    //cm.MenuItems.Add("Result View", new EventHandler(PictureBoxResultviewMain));
                    //cm.MenuItems.Add("Save", new EventHandler(PictureBoxSaveMain));
                    pictureBox_3.ContextMenu = cm;
                    pictureBox_3.ContextMenu.Show(pictureBox_3, e.Location);
                    pictureBox_3.ContextMenu = null;
                }
                else
                {
                    //ContextMenu cm = new ContextMenu();
                    //if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    //{//한국어
                    //    cm.MenuItems.Add("실시간 카메라 이미지 On/Off", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    //{//영어
                    //    cm.MenuItems.Add("Realtime view", new EventHandler(PictureBoxRealtimeviewMain));
                    //}
                    //pictureBox_3.ContextMenu = cm;
                    //pictureBox_3.ContextMenu.Show(pictureBox_3, e.Location);
                    //pictureBox_3.ContextMenu = null;
                }
            }
        }

        private void pictureBox_3_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
