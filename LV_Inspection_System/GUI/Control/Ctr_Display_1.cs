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
    public partial class Ctr_Display_1 : UserControl
    {
        public Ctr_Display_1()
        {
            InitializeComponent();
            //digitalClockCtrl1.SetDigitalColor = SriClocks.DigitalColor.GreenColor;
        }

        private void Ctr_Display_1_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (LVApp.Instance().m_Config.m_Cam_Total_Num == 1)
                {
                    pictureBox_0.Visible = true;
                    pictureBox_1.Visible = false;
                    pictureBox_2.Visible = false;
                }
                else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 2)
                {
                    pictureBox_0.Visible = true;
                    pictureBox_1.Visible = true;
                    pictureBox_2.Visible = false;
                }
                else if (LVApp.Instance().m_Config.m_Cam_Total_Num == 3)
                {
                    pictureBox_0.Visible = true;
                    pictureBox_1.Visible = true;
                    pictureBox_2.Visible = true;
                }
                int t_picture_width = (int)((this.Width * 863f / 869f) / 5f);

                pictureBox_0.Width = t_picture_width;
                pictureBox_0.Height = (int)(80 * pictureBox_0.Width / 107f);
                pictureBox_1.Width = t_picture_width;
                pictureBox_1.Height = (int)(80 * pictureBox_0.Width / 107f);
                pictureBox_2.Width = t_picture_width;
                pictureBox_2.Height = (int)(80 * pictureBox_0.Width / 107f);
                pictureBox_3.Width = t_picture_width;
                pictureBox_3.Height = (int)(80 * pictureBox_0.Width / 107f);
                //pictureBox_4.Width = t_picture_width;
                //pictureBox_4.Height = (int)(80 * pictureBox_0.Width / 107f);
                //pictureBox_5.Width = t_picture_width;
                //pictureBox_5.Height = (int)(80 * pictureBox_0.Width / 107f);
                //pictureBox_6.Width = t_picture_width;
                //pictureBox_6.Height = (int)(80 * pictureBox_0.Width / 107f);
                //pictureBox_7.Width = t_picture_width;
                //pictureBox_7.Height = (int)(80 * pictureBox_0.Width / 107f);
                splitContainer1.SplitterDistance = pictureBox_0.Height + 1;
                //splitContainer1.SplitterDistance = 0;
                double t_picture_height = this.Width * 856 / 904;
            }
            catch
            { }
        }

        public int m_Selected_PictureBox = 0;
        Color Grid_Line_Color = Color.LightGoldenrodYellow;
        Color Selected_color = Color.LawnGreen;
        private void pictureBox_Main_Paint(object sender, PaintEventArgs e)
        {
            using (Font myFont = new Font("Arial", 11))
            {
                e.Graphics.DrawString("CAM " + m_Selected_PictureBox.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
                //e.Graphics.DrawString(Matching_Result_Str[0], myFont, Brushes.LightGoldenrodYellow, new Point(4, 24));
                //Pen pen1 = new Pen(Grid_Line_Color, 1);
                //pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                //Point VP1 = new Point(pictureBox_Main.Width / 2, 0);
                //Point VP2 = new Point(pictureBox_Main.Width / 2, pictureBox_Main.Height);
                //e.Graphics.DrawLine(pen1, VP1, VP2);
                //Point HP1 = new Point(0, pictureBox_Main.Height / 2);
                //Point HP2 = new Point(pictureBox_Main.Width, pictureBox_Main.Height / 2);
                //e.Graphics.DrawLine(pen1, HP1, HP2);
                //if (m_Selected_PictureBox == 2)
                //{
                //    Pen pen2 = new Pen(Color.HotPink, 1);
                //    HP1 = new Point(0, 17 * pictureBox4.Height / 18);
                //    HP2 = new Point(pictureBox4.Width, 17 * pictureBox4.Height / 18);
                //    e.Graphics.DrawLine(pen2, HP1, HP2);
                //    pen2.Dispose();
                //}
                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    e.Graphics.DrawString("검사중... ", myFont, Brushes.Orange, new Point(pictureBox_Main.Width - 100, 80));
                }

                //pen1.Dispose();
            }
            using (Pen pen = new Pen(Color.DimGray, 1))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_Main.Width - 1, pictureBox_Main.Height - 1);
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[m_Selected_PictureBox] == 1)
            {
                using (Font myFont = new Font("Arial", 25))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_Main.Width - 90, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[1], myFont, Brushes.OrangeRed, new Point(pictureBox_Main.Width - 90, 50));
                //}
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[m_Selected_PictureBox], myFont, Brushes.CornflowerBlue, new Point(pictureBox_Main.Width - 90, pictureBox_Main.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[m_Selected_PictureBox] == 0)
            {
                using (Font myFont = new Font("Arial", 25))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_Main.Width - 85, 10));
                }
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[m_Selected_PictureBox], myFont, Brushes.CornflowerBlue, new Point(pictureBox_Main.Width - 90, pictureBox_Main.Height - 25));
                }
            }
        }

        private void pictureBox_0_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 0;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("NONE", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_1_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 1;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 2)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("NONE", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_2_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 2;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 2)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("NONE", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_3_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 3;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 2)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("NONE", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_4_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 4;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 2)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("NONE", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_5_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 5;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_6_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 6;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_7_Paint(object sender, PaintEventArgs e)
        {
            int CamNum = 7;
            using (Font myFont = new Font("Arial", 10))
            {
                e.Graphics.DrawString("CAM " + CamNum.ToString(), myFont, Brushes.LightGoldenrodYellow, new Point(4, 4));
            }
            if (m_Selected_PictureBox == CamNum)
            {
                using (Pen pen = new Pen(Selected_color, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    e.Graphics.DrawRectangle(pen, 1, 1, pictureBox_0.Width - 2, pictureBox_0.Height - 2);
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawRectangle(pen, 0, 0, pictureBox_0.Width - 1, pictureBox_0.Height - 1);
                }
            }
            if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 1)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("N G", myFont, Brushes.Red, new Point(pictureBox_0.Width - 40, 10));
                }
                //using (Font myFont = new Font("Arial", 9))
                //{
                //    e.Graphics.DrawString(m_Result_Defect_List[0], myFont, Brushes.OrangeRed, new Point(pictureBox1.Width - 60, 50));
                //}
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
            else if (LVApp.Instance().m_Config.m_Error_Flag[CamNum] == 0)
            {
                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("O K", myFont, Brushes.SkyBlue, new Point(pictureBox_0.Width - 40, 10));
                }
                using (Font myFont = new Font("Arial", 9))
                {
                    e.Graphics.DrawString(LVApp.Instance().m_Config.m_TT[CamNum], myFont, Brushes.CornflowerBlue, new Point(pictureBox_0.Width - 70, pictureBox_0.Height - 25));
                }
            }
        }

        private void pictureBox_0_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 0)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_0.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 0;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM0.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 1)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 1;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM1.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 2)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 2;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM2.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 3)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 3;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM3.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 4)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 4;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM4.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_5_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 5)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 5;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM5.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 6)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 6;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM6.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        private void pictureBox_7_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                if (m_Selected_PictureBox == 7)
                {
                    //m_Selected_PictureBox = -1;
                    //pictureBox_1.ContextMenu = null;
                }
                else
                {
                    m_Selected_PictureBox = 7;
                    LVApp.Instance().m_mainform.ctr_Manual1.radioButton_CAM7.Checked = true;
                }
                Update_Main_Image(-1);
            }
        }

        public void Update_Main_Image(int Cam_num)
        {
            if (m_Selected_PictureBox == 0 && Cam_num == 0)
            {
                if (pictureBox_0.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_0.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 1 && Cam_num == 0)
            {
                if (pictureBox_1.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_1.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 2 && Cam_num == 0)
            {
                if (pictureBox_2.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_2.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 3 && Cam_num == 0)
            {
                if (pictureBox_3.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_3.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 4 && Cam_num == 0)
            {
                if (pictureBox_4.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_4.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 5 && Cam_num == 0)
            {
                if (pictureBox_5.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_5.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 6 && Cam_num == 0)
            {
                if (pictureBox_6.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_6.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }
            else if (m_Selected_PictureBox == 7 && Cam_num == 0)
            {
                if (pictureBox_7.Image != null)
                {
                    pictureBox_Main.Image = (System.Drawing.Image)pictureBox_7.Image.Clone();
                    pictureBox_Main.Refresh();
                }
            }

            if (Cam_num == -1)
            {
                if (m_Selected_PictureBox == 0)
                {
                    if (pictureBox_0.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_0.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 1)
                {
                    if (pictureBox_1.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_1.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 2)
                {
                    if (pictureBox_2.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_2.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 3)
                {
                    if (pictureBox_3.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_3.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 4)
                {
                    if (pictureBox_4.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_4.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 5)
                {
                    if (pictureBox_5.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_5.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 6)
                {
                    if (pictureBox_6.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_6.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                else if (m_Selected_PictureBox == 7)
                {
                    if (pictureBox_7.Image != null)
                    {
                        pictureBox_Main.Image = (System.Drawing.Image)pictureBox_7.Image.Clone();
                        pictureBox_Main.Refresh();
                    }
                }
                this.Refresh();
            }
        }

        private void PictureBoxPopupViewMain(object sender, EventArgs e)
        {
            Form_BigImage View_form = new Form_BigImage();
            View_form.imageControl1.Image = (Image)pictureBox_Main.Image.Clone();
            View_form.Show();
        }

        private void PictureBoxSaveMain(object sender, EventArgs e)
        {
            using (Image bmp = (Image)pictureBox_Main.Image.Clone())
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

        public void Image_SaveFileDialog(Image bmp)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = LVApp.Instance().excute_path;
            SaveFileDialog1.RestoreDirectory = true;
            SaveFileDialog1.Title = "이미지 저장";
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

        private void pictureBox_Main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                if (pictureBox_Main.Image != null)
                {
                    ContextMenu cm = new ContextMenu();
                    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeviewMain));
                    cm.MenuItems.Add("영상 처리 결과 보기", new EventHandler(PictureBoxResultviewMain));
                    cm.MenuItems.Add("이미지 화면 띄우기", new EventHandler(PictureBoxPopupViewMain));
                    cm.MenuItems.Add("이미지 저장하기", new EventHandler(PictureBoxSaveMain));


                    //cm.MenuItems.Add("Popup View", new EventHandler(PictureBoxPopupViewMain));
                    //cm.MenuItems.Add("Result View", new EventHandler(PictureBoxResultviewMain));
                    //cm.MenuItems.Add("Save", new EventHandler(PictureBoxSaveMain));
                    pictureBox_Main.ContextMenu = cm;
                    pictureBox_Main.ContextMenu.Show(pictureBox_Main, e.Location);
                    pictureBox_Main.ContextMenu = null;
                }
                else
                {
                    ContextMenu cm = new ContextMenu();
                    cm.MenuItems.Add("실시간 화면 보기", new EventHandler(PictureBoxRealtimeviewMain));
                    pictureBox_Main.ContextMenu = cm;
                    pictureBox_Main.ContextMenu.Show(pictureBox_Main, e.Location);
                    pictureBox_Main.ContextMenu = null;
                }
            }
        }

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
    }
}
