using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;   //호환되지 않은 Dll을 사용할때
using ZedGraph;
using System.Data;
//using lc_handle_t = System.UInt32;
using lc_handle_t = System.Int64;
using System.Diagnostics;

namespace LV_Inspection_System
{
    public class Utility
    {
        #region Basic Frame Counter


        //private static int frameCount = 0;
        //private static float nextUpdate = 0.0f;
        //private static float fps = 0.0f;
        //private static float updateRate = 1.0f;

        //public static float CalculateFrameRate0()
        //{
        //    frameCount++;
        //    if (DateTime.Now.Millisecond > nextUpdate)
        //    {
        //        nextUpdate = (float)DateTime.Now.Millisecond + 1.0f / updateRate;
        //        fps = (float)frameCount * updateRate;
        //        frameCount = 0;
        //    }
        //    return fps;
        //}


        private int[] lastTick = new int[8];
        private int[] lastFrameRate = new int[8];
        private int[] frameRate = new int[8];

        private int[,] frameRate_sum = new int[10,8];
        private int[] frameRate_idx = new int[8];
        public float[] m_FPS = new float[8]; 

        public void CalculateFrameRate(int t_num)
        {
            if (System.Environment.TickCount - lastTick[t_num] >= 1000)
            {
                lastFrameRate[t_num] = frameRate[t_num];
                frameRate[t_num] = 0;
                lastTick[t_num] = System.Environment.TickCount;
            }
            frameRate[t_num]++;

            frameRate_sum[frameRate_idx[t_num], t_num] = lastFrameRate[t_num];
            frameRate_idx[t_num]++; frameRate_idx[t_num] %= 10;

            float t_out = 0;
            for (int i = 0; i < 10; i++)
            {
                t_out += (float)frameRate_sum[i, t_num];
            }
            m_FPS[t_num] = t_out / 10f;
            //return m_FPS[t_num];
        }



        private static int lastTick0;
        private static int lastFrameRate0;
        private static int frameRate0;

        private static int[] frameRate0_sum = new int[10];
        private static int frameRate0_idx = 0;

        public static float CalculateFrameRate0()
        {
            if (System.Environment.TickCount - lastTick0 >= 1000)
            {
                lastFrameRate0 = frameRate0;
                frameRate0 = 0;
                lastTick0 = System.Environment.TickCount;
            }
            frameRate0++;

            frameRate0_sum[frameRate0_idx] = lastFrameRate0;
            frameRate0_idx++; frameRate0_idx %= 10;

            double t_out = 0;
            for (int i = 0; i < 10; i++)
            {
                t_out += (double)frameRate0_sum[i];
            }
            return (float)(t_out / 10.0);
        }

        private static int lastTick1;
        private static int lastFrameRate1;
        private static int frameRate1;

        public static int CalculateFrameRate1()
        {
            if (System.Environment.TickCount - lastTick1 >= 1000)
            {
                lastFrameRate1 = frameRate1;
                frameRate1 = 0;
                lastTick1 = System.Environment.TickCount;
            }
            frameRate1++;
            return lastFrameRate1;
        }

        private static int lastTick2;
        private static int lastframeRate2;
        private static int frameRate2;

        public static int CalculateFrameRate2()
        {
            if (System.Environment.TickCount - lastTick2 >= 1000)
            {
                lastframeRate2 = frameRate2;
                frameRate2 = 0;
                lastTick2 = System.Environment.TickCount;
            }
            frameRate2++;
            return lastframeRate2;
        }

        private static int lastTick3;
        private static int lastframeRate3;
        private static int frameRate3;

        public static int CalculateFrameRate3()
        {
            if (System.Environment.TickCount - lastTick3 >= 1000)
            {
                lastframeRate3 = frameRate3;
                frameRate3 = 0;
                lastTick3 = System.Environment.TickCount;
            }
            frameRate3++;
            return lastframeRate3;
        }

        private static int lastTick_C0;
        private static int lastFrameRate_C0;
        private static int frameRate_C0;

        public static int CalculateFrameRate_C0()
        {
            if (System.Environment.TickCount - lastTick_C0 >= 1000)
            {
                lastFrameRate_C0 = frameRate_C0;
                frameRate_C0 = 0;
                lastTick_C0 = System.Environment.TickCount;
            }
            frameRate_C0++;
            return lastFrameRate_C0;
        }

        private static int lastTick_C1;
        private static int lastFrameRate_C1;
        private static int frameRate_C1;

        public static int CalculateFrameRate_C1()
        {
            if (System.Environment.TickCount - lastTick_C1 >= 1000)
            {
                lastFrameRate_C1 = frameRate_C1;
                frameRate_C1 = 0;
                lastTick_C1 = System.Environment.TickCount;
            }
            frameRate_C1++;
            return lastFrameRate_C1;
        }

        private static int lastTick_C2;
        private static int lastFrameRate_C2;
        private static int frameRate_C2;

        public static int CalculateFrameRate_C2()
        {
            if (System.Environment.TickCount - lastTick_C2 >= 1000)
            {
                lastFrameRate_C2 = frameRate_C2;
                frameRate_C2 = 0;
                lastTick_C2 = System.Environment.TickCount;
            }
            frameRate_C2++;
            return lastFrameRate_C2;
        }

        private static int lastTick_C3;
        private static int lastFrameRate_C3;
        private static int frameRate_C3;

        public static int CalculateFrameRate_C3()
        {
            if (System.Environment.TickCount - lastTick_C3 >= 1000)
            {
                lastFrameRate_C3 = frameRate_C3;
                frameRate_C3 = 0;
                lastTick_C3 = System.Environment.TickCount;
            }
            frameRate_C3++;
            return lastFrameRate_C3;
        }

        public DateTime Delay(int MS)
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


        //private static double lastTick0;
        //private static double lastFrameRate0;
        //private static double frameRate0;

        //public static double CalculateFrameRate0()
        //{
        //    if ((double)System.Environment.TickCount - lastTick0 >= 1000d)
        //    {
        //        lastFrameRate0 = Math.Round((double)Stopwatch.Frequency * 0.0001d * frameRate0 / ((double)System.Environment.TickCount - lastTick0), 1);
        //        frameRate0 = 0;
        //        lastTick0 = (double)System.Environment.TickCount;
        //    }
        //    frameRate0++;
        //    return lastFrameRate0;
        //}

        //private static double lastTick1;
        //private static double lastFrameRate1;
        //private static double frameRate1;

        //public static double CalculateFrameRate1()
        //{
        //    if ((double)System.Environment.TickCount - lastTick1 >= 1000d)
        //    {
        //        lastFrameRate1 = Math.Round((double)Stopwatch.Frequency * 0.0001d * frameRate1 / ((double)System.Environment.TickCount - lastTick1), 1);
        //        frameRate1 = 0;
        //        lastTick1 = (double)System.Environment.TickCount;
        //    }
        //    frameRate1++;
        //    return lastFrameRate1;
        //}

        //private static double lastTick2;
        //private static double lastFrameRate2;
        //private static double frameRate2;

        //public static double CalculateFrameRate2()
        //{
        //    if ((double)System.Environment.TickCount - lastTick2 >= 1000d)
        //    {
        //        lastFrameRate2 = Math.Round((double)Stopwatch.Frequency * 0.0001d * frameRate2 / ((double)System.Environment.TickCount - lastTick2), 1);
        //        frameRate2 = 0;
        //        lastTick2 = (double)System.Environment.TickCount;
        //    }
        //    frameRate2++;
        //    return lastFrameRate2;
        //}

        //private static double lastTick3;
        //private static double lastFrameRate3;
        //private static double frameRate3;

        //public static double CalculateFrameRate3()
        //{
        //    if ((double)System.Environment.TickCount - lastTick3 >= 1000d)
        //    {
        //        lastFrameRate3 = Math.Round((double)Stopwatch.Frequency * 0.0001d * frameRate3 / ((double)System.Environment.TickCount - lastTick3), 1);
        //        frameRate3 = 0;
        //        lastTick3 = (double)System.Environment.TickCount;
        //    }
        //    frameRate3++;
        //    return lastFrameRate3;
        //}
        #endregion
    }

    public static class DataTableExtensions
    {
        public static void WriteToCsvFile(this DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {

                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            fileContent.Append(System.Environment.NewLine);
            //System.IO.File.WriteAllText(filePath, fileContent.ToString(), Encoding.UTF8);
            System.IO.File.AppendAllText(filePath, fileContent.ToString(), Encoding.UTF8);
        }


        public static void OpenCSVFile(this DataTable mycsvdt, string filepath)
        {
            string strpath = filepath; //csv file path
            try
            {
                DataRow mydr;
                string strline;
                string[] aryline;
                StreamReader mysr = new StreamReader(strpath, System.Text.Encoding.Default);
                bool t_check_file = false; int t_line_num = 0;
                while ((strline = mysr.ReadLine()) != null)
                {
                    aryline = strline.Split(new char[] { ',' });

                    if (aryline[0] == "")
                    {
                        continue;
                    }
                    //fill data into datatable
                    if (aryline[0] == "구분" || aryline[0] == "Item")
                    {
                        if (!t_check_file)
                        {
                            t_check_file = true;
                        }
                    }
                    else
                    {
                        if (t_line_num == 0)
                        {
                            if (!t_check_file)
                            {
                                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                                {//한국어
                                    MessageBox.Show("ROI 설정 파일이 불량입니다.");
                                }
                                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                                {//영어
                                    MessageBox.Show("No ROI setting file!");
                                }
                                return;
                            }
                        }
                        mydr = mycsvdt.NewRow();
                        for (int i = 0; i < 2; i++)
                        {
                            mydr[i] = aryline[i].Substring(1, aryline[i].Length - 2);
                        }
                        mycsvdt.Rows.Add(mydr);
                    }

                    //if (mydr[0].ToString() == "LOG")
                    //{
                    //    //LVApp.Instance().m_parameters.m_Log_CNT = Convert.ToInt32(mydr[1].ToString().Substring(0, mydr[1].ToString().Length-1));
                    //}
                    t_line_num++;
                }
                // return true;
            }
            catch
            {
                // throw (Stack.GetErrorStack(strpath + "Error" + e.Message, "OpenCSVFile("));
                // return false;
            }
        }
    }

    public static class Util
    {

        //=====================================================================
        //  시간 측정
        //=====================================================================
        public static double TimeInSeconds(DateTime stime)
        {
            TimeSpan dtime = DateTime.Now - stime;
            double dsec = (double)(dtime.Ticks / 10000000.0);
            return dsec;
        }

        //=====================================================================
        //  숫자인가? (정수, 실수 모두 true로 리턴)
        //=====================================================================
        public static bool IsNumber(string st)
        {
            double val;
            return double.TryParse(st, out val);
        }

        //=====================================================================
        //  정수인가?
        //=====================================================================
        public static bool IsInteger(string st)
        {
            int val;
            return int.TryParse(st, out val);
        }

        //=====================================================================
        //  실수의 반올림
        //=====================================================================
        public static double RoundK(double sv, int ndigit)
        {
            return Math.Round(sv, ndigit);

            // 위의 함수는 반올림 정의와 정확치 않은 경우가 있다.
            // 아래 형식을 활용하면 되긴 하는데 그럴 필요가 있는가??
            //string st = String.Format("{0:0.00}", 1.345);
        }

        //=====================================================================
        //  정수로 반올림
        //=====================================================================
        public static int Int(double sv)
        {
            return Convert.ToInt32(Util.RoundK(sv, 0));
        }

        public static int Int(float sv)
        {
            return Int(sv);
        }

        public static uint UInt(double sv)
        {
            return Convert.ToUInt32(Util.RoundK(sv, 0));
        }

        public static uint UInt(float sv)
        {
            return UInt(sv);
        }

        //=====================================================================
        //  시간 대기
        //=====================================================================
        public static void HoldForAWhileWithDoEvents(double sec)
        {
            double dsec;
            DateTime stime = DateTime.Now;
            while (true)
            {
                Thread.Sleep(1);

                Application.DoEvents();

                dsec = Util.TimeInSeconds(stime);
                if (dsec >= sec) break;
            }
        }

        public static void HoldForAWhileWithoutDoEvents(double sec)
        {
            double dsec;
            DateTime stime = DateTime.Now;
            while (true)
            {
                dsec = Util.TimeInSeconds(stime);
                if (dsec >= sec) break;
            }
        }

        //=====================================================================
        //  문자분리
        //=====================================================================
        public static string[] SplitString(string st, char c1)
        {
            return st.Split(c1);
        }
        public static string[] SplitString(string st, char c1, char c2)
        {
            //Char[] carr = { c1, c2 };
            //return st.Split(carr);
            return st.Split(new Char[] { c1, c2 });
        }
        public static string[] SplitString(string st, char[] carr)
        {
            return st.Split(carr);
        }

        //=====================================================================
        //  Text 뜯어내기 함수 : Substring() 함수는 에러가 발생할 수 있어
        //                       따로 자작함
        //=====================================================================
        public static string TextLeft(string st, int length)
        {
            string rtn = "";

            if (length <= 0)
                rtn = "";
            else if (st.Length <= length)
                rtn = st;
            else
                rtn = st.Substring(0, length);

            return rtn;
        }

        public static string TextRight(string st, int length)
        {
            string rtn = "";

            if (length <= 0)
                rtn = "";
            else if (st.Length <= length)
                rtn = st;
            else
                rtn = st.Substring(st.Length - length);

            return rtn;
        }

        public static string TextMid(string st, int startidx)
        {
            string rtn = "";

            if (startidx < 0) startidx = 0;
            if (startidx >= st.Length)
                rtn = "";
            else
                rtn = st.Substring(startidx);

            return rtn;
        }

        public static string TextMid(string st, int startidx, int length)
        {
            string rtn = "";

            if (startidx < 0) startidx = 0;
            if (length <= 0)
                rtn = "";
            else if (startidx >= st.Length)
                rtn = "";
            else
            {
                if (length >= st.Length - startidx)
                    rtn = st.Substring(startidx);
                else
                    rtn = st.Substring(startidx, length);
            }

            return rtn;
        }

        //=====================================================================
        //  16진수로 (기억이 잘 안나서...)
        //=====================================================================
        public static string Hex(uint ival)
        {
            return String.Format("{0:X}", ival);
        }
        public static string Hex(int ival)
        {
            return String.Format("{0:X}", ival);
        }

        //=====================================================================
        //  2진수로 
        //=====================================================================
        public static string Bin(uint ival)
        {
            uint pow = 1;
            string rtn = "";
            for (int i = 0; i < 32; i++)
            {
                rtn = ((ival & pow) != 0 ? "1" : "0") + rtn;
                pow *= 2;
            }
            return rtn;
        }
        public static string Bin(int ival)
        {
            return Bin((uint)ival);
        }

        //=====================================================================
        //  컨트롤의 모든 자식 컨트롤 얻기
        //  http://www.devpia.com/MAEUL/Contents/Detail.aspx?BoardID=17&MAEULNo=8&no=76292&ref=76285
        //=====================================================================
        public static Control[] GetAllControls(Control ctrl)
        {
            ArrayList allControls = new ArrayList();
            Queue queue = new Queue();

            queue.Enqueue(ctrl.Controls);

            while (queue.Count > 0)
            {
                Control.ControlCollection controls = (Control.ControlCollection)queue.Dequeue();

                if (controls == null || controls.Count == 0) continue;

                foreach (Control control in controls)
                {
                    allControls.Add(control);
                    queue.Enqueue(control.Controls);
                }
            }

            return (Control[])allControls.ToArray(typeof(Control));
        }

        //=====================================================================
        //  컨트롤의 모든 자식 컨트롤 중 주어진 타입만 얻기
        //  (!!) 타이머는 못 찾음
        //  (예)
        //      GetAllControlsOfType(frm, typeof(TextBox));
        //=====================================================================
        public static Control[] GetAllControlsOfType(Control ctrl, System.Type type)
        {
            // 일단 모두 찾기
            Control[] ctrlAll = GetAllControls(ctrl);

            // 추려내기
            ArrayList ctrlOfType = new ArrayList();
            foreach (Control control in ctrlAll)
            {
                if (control.GetType() == type)
                {
                    ctrlOfType.Add(control);
                }
            }

            return (Control[])ctrlOfType.ToArray(typeof(Control));
        }

        //=====================================================================
        //  컨트롤 안에서 주어진 이름의 자식컨트롤 찾기
        //  (예)
        //      GetControlOfName(frm, "btnEnd");
        //=====================================================================
        public static Control FindControlOfNameInCtrl(Control ctrl, string controlName)
        {
            Control rtn = null;

            ArrayList allControls = new ArrayList();
            Queue queue = new Queue();

            queue.Enqueue(ctrl.Controls);

            while (queue.Count > 0)
            {
                Control.ControlCollection controls = (Control.ControlCollection)queue.Dequeue();

                if (controls == null || controls.Count == 0) continue;

                foreach (Control ctl in controls)
                {
                    if (controlName.Equals(ctl.Name))
                    {
                        rtn = ctl;
                        goto EndOfFunc;
                    }
                    queue.Enqueue(ctl.Controls);
                }
            }
        EndOfFunc:
            return rtn;
        }
    }

    public static class RectangleDrawer
    {
        //사용은 아래와 같이 한다.
        // Rectangle rc = RectangleDrawer.Draw(this);
        private static Form mMask;
        private static Point mPos;
        public static Rectangle Draw(PictureBox parent)
        {
            // Record the start point
            mPos = parent.PointToClient(Control.MousePosition);
            // Create a transparent form on top of <frm>
            mMask = new Form();
            mMask.FormBorderStyle = FormBorderStyle.None;
            mMask.BackColor = Color.Magenta;
            mMask.TransparencyKey = mMask.BackColor;
            mMask.ShowInTaskbar = false;
            mMask.StartPosition = FormStartPosition.Manual;
            mMask.Size = parent.ClientSize;
            mMask.Location = parent.PointToScreen(Point.Empty);
            mMask.MouseDown += MouseDn;
            mMask.MouseMove += MouseMove;
            mMask.MouseUp += MouseUp;
            mMask.Paint += PaintRectangle;
            mMask.Load += DoCapture;
            // Display the overlay
            mMask.ShowDialog(parent);
            // Clean-up and calculate return value
            mMask.Dispose();
            mMask = null;
            Point pos = parent.PointToClient(Control.MousePosition);
            int x = Math.Min(mPos.X, pos.X);
            int y = Math.Min(mPos.Y, pos.Y);
            int w = Math.Abs(mPos.X - pos.X);
            int h = Math.Abs(mPos.Y - pos.Y);
            return new Rectangle(x, y, w, h);
        }
        private static void DoCapture(object sender, EventArgs e)
        {
            // Grab the mouse
            mMask.Capture = true;
        }
        private static void MouseDn(object sender, MouseEventArgs e)
        {
            mPos.X = e.X;
            mPos.Y = e.Y;
        }
        private static void MouseMove(object sender, MouseEventArgs e)
        {
            // Repaint the rectangle
            mMask.Invalidate();
        }
        private static void MouseUp(object sender, MouseEventArgs e)
        {
            // Done, close mask
            mMask.Close();
        }
        private static void PaintRectangle(object sender, PaintEventArgs e)
        {
            // Draw the current rectangle
            Point pos = mMask.PointToClient(Control.MousePosition);
            using (Pen pen = new Pen(Brushes.Yellow))
            {
                pen.DashStyle = DashStyle.Dot;
                e.Graphics.DrawLine(pen, mPos.X, mPos.Y, pos.X, mPos.Y);
                e.Graphics.DrawLine(pen, pos.X, mPos.Y, pos.X, pos.Y);
                e.Graphics.DrawLine(pen, pos.X, pos.Y, mPos.X, pos.Y);
                e.Graphics.DrawLine(pen, mPos.X, pos.Y, mPos.X, mPos.Y);
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    public class UserRect
    {
        private PictureBox mPictureBox;
        public Rectangle rect;
        public bool allowDeformingDuringMovement = true;
        private bool mIsClick = false;
        private bool mMove = false;
        private int oldX;
        private int oldY;
        private int sizeNodeRect = 5;
        private Bitmap mBmp = null;
        private PosSizableRect nodeSelected = PosSizableRect.None;
        public bool mSelected = true;
        public string m_ROI_Name = "";
        public bool mView = true;
        public bool mUse = true;
        public int m_Cam_Num = 0;
        public int t_idx = 0;
        public int m_radius_w = 0;
        public int m_radius_h = 0;
        private enum PosSizableRect
        {
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None

        };

        public UserRect(Rectangle r)
        {
            rect = r;
            mIsClick = false;
        }

        public void Draw(Graphics g)
        {
            if (LVApp.Instance().m_mainform.ctr_ROI1.dataGridView1.Rows[12].Cells[1].Value.ToString() == "7496" && LVApp.Instance().m_Config.ROI_Cam_Num == 0)
            {
                rect.X = 0;
                rect.Y = 0;
                rect.Width = LVApp.Instance().m_mainform.ctr_ROI1.pictureBox_Image.Width;
                rect.Height = LVApp.Instance().m_mainform.ctr_ROI1.pictureBox_Image.Height;
            }

            if (t_idx > 0)
            {
                int tt_CNT = LVApp.Instance().m_Config.m_AIParam[m_Cam_Num].Count;
                if (tt_CNT > 0)
                {
                    for (int i = 0; i < tt_CNT; i++)
                    {
                        UTIL.LV_Config.AIParam t_AIParam = LVApp.Instance().m_Config.m_AIParam[m_Cam_Num][i];
                        if (t_AIParam.USE && t_AIParam.ROI_IDX == t_idx - 1)
                        {
                            int t_v = Math.Max(rect.Height, rect.Width);
                            rect.Height = rect.Width = t_v;
                        }
                    }
                }
            }
            if (rect.Width <= 2)
            {
                rect.Width = 2;
                //return;
            }
            if (rect.Height <= 2)
            {
                rect.Height = 2;
                //return;
            }
            if (mSelected)
            {
                using (Pen pen = new Pen(Color.OrangeRed, 1))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(pen, rect);
                }

                Pen pen1 = new Pen(Color.HotPink, 1);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                int margin = 11;
                Point VP1 = new Point(rect.X + rect.Width / 2 - margin, rect.Y + rect.Height / 2);
                Point VP2 = new Point(rect.X + rect.Width / 2 + margin + 1, rect.Y + rect.Height / 2);
                g.DrawLine(pen1, VP1, VP2);
                Point HP1 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2 - margin);
                Point HP2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2 + margin +1);
                g.DrawLine(pen1, HP1, HP2);
                pen1.Dispose();

                using (Pen pen = new Pen(Color.LightBlue, 1))
                {
                    pen.DashStyle = DashStyle.Solid;
                    g.DrawEllipse(pen, new Rectangle((rect.X + rect.Width / 2) - m_radius_w, (rect.Y + rect.Height / 2) - m_radius_h, 2 * m_radius_w, 2 * m_radius_h));
                }
                
                foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
                {
                    g.DrawRectangle(new Pen(Color.Red, 1), GetRect(pos));
                }

                if (m_Cam_Num == 0)
                {
                    if (t_idx == LVApp.Instance().m_Config.ROI_Selected_IDX[m_Cam_Num])
                    {
                        if (t_idx > 0)
                        {
                            if (LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.X < LVApp.Instance().m_Config.Cam0_rect[0].rect.X)
                            {
                                LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.X = LVApp.Instance().m_Config.Cam0_rect[0].rect.X + 1;
                            }
                            if (LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y < LVApp.Instance().m_Config.Cam0_rect[0].rect.Y)
                            {
                                LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y = LVApp.Instance().m_Config.Cam0_rect[0].rect.Y - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.X + LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Width > LVApp.Instance().m_Config.Cam0_rect[0].rect.X + LVApp.Instance().m_Config.Cam0_rect[0].rect.Width)
                            {
                                LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Width = LVApp.Instance().m_Config.Cam0_rect[0].rect.X + LVApp.Instance().m_Config.Cam0_rect[0].rect.Width - LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.X - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y + LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Height > LVApp.Instance().m_Config.Cam0_rect[0].rect.Y + LVApp.Instance().m_Config.Cam0_rect[0].rect.Height)
                            {
                                LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Height = LVApp.Instance().m_Config.Cam0_rect[0].rect.Y + LVApp.Instance().m_Config.Cam0_rect[0].rect.Height - LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y - 1;
                            }
                        }

                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_mainform.ctr_ROI1.Change_initial_ROI();
                        }
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[1][1] = LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.X;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[2][1] = LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Y;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[3][1] = LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Width;
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[4][1] = LVApp.Instance().m_Config.Cam0_rect[t_idx].rect.Height;

                        string str = LVApp.Instance().m_Config.Cam0_rect[t_idx].mUse == false ? "X" : "O";
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[1][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[2][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[3][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[4][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[5][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[6][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[7][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[8][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[9][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[10][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[11][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[12][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[13][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[14][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[15][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[16][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[17][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[18][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[19][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[20][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[21][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[22][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[23][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[24][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_0.Tables[3].Rows[25][1].ToString();
                    }
                    else
                    {
                        string str = mUse == false ? "X" : "O";
                        string[] t_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1].ToString().Split('_');
                        LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            rect.X.ToString()
                            + "_" +
                            rect.Y.ToString()
                            + "_" +
                            rect.Width.ToString()
                            + "_" +
                            rect.Height.ToString()
                            + "_" +
                            t_str[5]
                            + "_" +
                            t_str[6]
                            + "_" +
                            t_str[7]
                            + "_" +
                            t_str[8]
                            + "_" +
                            t_str[9]
                            + "_" +
                            t_str[10]
                            + "_" +
                            t_str[11]
                            + "_" +
                            t_str[12]
                            + "_" +
                            t_str[13]
                            + "_" +
                            t_str[14]
                            + "_" +
                            t_str[15]
                            + "_" +
                            t_str[16]
                            + "_" +
                            t_str[17]
                            + "_" +
                            t_str[18]
                            + "_" +
                            t_str[19]
                            + "_" +
                            t_str[20]
                            + "_" +
                            t_str[21]
                            + "_" +
                            t_str[22]
                            + "_" +
                            t_str[23]
                            + "_" +
                            t_str[24]
                            + "_" +
                            t_str[25];

                    }
                }
                else if (m_Cam_Num == 1)
                {
                    if (t_idx == LVApp.Instance().m_Config.ROI_Selected_IDX[m_Cam_Num])
                    {
                        if (t_idx > 0)
                        {
                            if (LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.X < LVApp.Instance().m_Config.Cam1_rect[0].rect.X)
                            {
                                LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.X = LVApp.Instance().m_Config.Cam1_rect[0].rect.X + 1;
                            }
                            if (LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y < LVApp.Instance().m_Config.Cam1_rect[0].rect.Y)
                            {
                                LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y = LVApp.Instance().m_Config.Cam1_rect[0].rect.Y - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.X + LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Width > LVApp.Instance().m_Config.Cam1_rect[0].rect.X + LVApp.Instance().m_Config.Cam1_rect[0].rect.Width)
                            {
                                LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Width = LVApp.Instance().m_Config.Cam1_rect[0].rect.X + LVApp.Instance().m_Config.Cam1_rect[0].rect.Width - LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.X - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y + LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Height > LVApp.Instance().m_Config.Cam1_rect[0].rect.Y + LVApp.Instance().m_Config.Cam1_rect[0].rect.Height)
                            {
                                LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Height = LVApp.Instance().m_Config.Cam1_rect[0].rect.Y + LVApp.Instance().m_Config.Cam1_rect[0].rect.Height - LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y - 1;
                            }
                        }

                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_mainform.ctr_ROI2.Change_initial_ROI();
                        }

                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[1][1] = LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.X;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[2][1] = LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Y;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[3][1] = LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Width;
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[4][1] = LVApp.Instance().m_Config.Cam1_rect[t_idx].rect.Height;

                        string str = LVApp.Instance().m_Config.Cam1_rect[t_idx].mUse == false ? "X" : "O";
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[1][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[2][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[3][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[4][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[5][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[6][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[7][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[8][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[9][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[10][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[11][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[12][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[13][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[14][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[15][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[16][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[17][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[18][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[19][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[20][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[21][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[22][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[23][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[24][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_1.Tables[3].Rows[25][1].ToString();
                    }
                    else
                    {
                        string str = mUse == false ? "X" : "O";
                        string[] t_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1].ToString().Split('_');
                        LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            rect.X.ToString()
                            + "_" +
                            rect.Y.ToString()
                            + "_" +
                            rect.Width.ToString()
                            + "_" +
                            rect.Height.ToString()
                            + "_" +
                            t_str[5]
                            + "_" +
                            t_str[6]
                            + "_" +
                            t_str[7]
                            + "_" +
                            t_str[8]
                            + "_" +
                            t_str[9]
                            + "_" +
                            t_str[10]
                            + "_" +
                            t_str[11]
                            + "_" +
                            t_str[12]
                            + "_" +
                            t_str[13]
                            + "_" +
                            t_str[14]
                            + "_" +
                            t_str[15]
                            + "_" +
                            t_str[16]
                            + "_" +
                            t_str[17]
                            + "_" +
                            t_str[18]
                            + "_" +
                            t_str[19]
                            + "_" +
                            t_str[20]
                            + "_" +
                            t_str[21]
                            + "_" +
                            t_str[22]
                            + "_" +
                            t_str[23]
                            + "_" +
                            t_str[24]
                            + "_" +
                            t_str[25];
                    }
                }
                else if (m_Cam_Num == 2)
                {
                    if (t_idx == LVApp.Instance().m_Config.ROI_Selected_IDX[m_Cam_Num])
                    {
                        if (t_idx > 0)
                        {
                            if (LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.X < LVApp.Instance().m_Config.Cam2_rect[0].rect.X)
                            {
                                LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.X = LVApp.Instance().m_Config.Cam2_rect[0].rect.X + 1;
                            }
                            if (LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y < LVApp.Instance().m_Config.Cam2_rect[0].rect.Y)
                            {
                                LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y = LVApp.Instance().m_Config.Cam2_rect[0].rect.Y - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.X + LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Width > LVApp.Instance().m_Config.Cam2_rect[0].rect.X + LVApp.Instance().m_Config.Cam2_rect[0].rect.Width)
                            {
                                LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Width = LVApp.Instance().m_Config.Cam2_rect[0].rect.X + LVApp.Instance().m_Config.Cam2_rect[0].rect.Width - LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.X - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y + LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Height > LVApp.Instance().m_Config.Cam2_rect[0].rect.Y + LVApp.Instance().m_Config.Cam2_rect[0].rect.Height)
                            {
                                LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Height = LVApp.Instance().m_Config.Cam2_rect[0].rect.Y + LVApp.Instance().m_Config.Cam2_rect[0].rect.Height - LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y - 1;
                            }
                        }

                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_mainform.ctr_ROI3.Change_initial_ROI();
                        }

                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[1][1] = LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.X;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[2][1] = LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Y;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[3][1] = LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Width;
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[4][1] = LVApp.Instance().m_Config.Cam2_rect[t_idx].rect.Height;

                        string str = LVApp.Instance().m_Config.Cam2_rect[t_idx].mUse == false ? "X" : "O";
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[1][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[2][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[3][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[4][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[5][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[6][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[7][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[8][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[9][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[10][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[11][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[12][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[13][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[14][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[15][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[16][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[17][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[18][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[19][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[20][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[21][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[22][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[23][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[24][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_2.Tables[3].Rows[25][1].ToString();
                    }
                    else
                    {
                        string str = mUse == false ? "X" : "O";
                        string[] t_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1].ToString().Split('_');
                        LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            rect.X.ToString()
                            + "_" +
                            rect.Y.ToString()
                            + "_" +
                            rect.Width.ToString()
                            + "_" +
                            rect.Height.ToString()
                            + "_" +
                            t_str[5]
                            + "_" +
                            t_str[6]
                            + "_" +
                            t_str[7]
                            + "_" +
                            t_str[8]
                            + "_" +
                            t_str[9]
                            + "_" +
                            t_str[10]
                            + "_" +
                            t_str[11]
                            + "_" +
                            t_str[12]
                            + "_" +
                            t_str[13]
                            + "_" +
                            t_str[14]
                            + "_" +
                            t_str[15]
                            + "_" +
                            t_str[16]
                            + "_" +
                            t_str[17]
                            + "_" +
                            t_str[18]
                            + "_" +
                            t_str[19]
                            + "_" +
                            t_str[20]
                            + "_" +
                            t_str[21]
                            + "_" +
                            t_str[22]
                            + "_" +
                            t_str[23]
                            + "_" +
                            t_str[24]
                            + "_" +
                            t_str[25];
                    }
                }
                else if (m_Cam_Num == 3)
                {
                    if (t_idx == LVApp.Instance().m_Config.ROI_Selected_IDX[m_Cam_Num])
                    {
                        if (t_idx > 0)
                        {
                            if (LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.X < LVApp.Instance().m_Config.Cam3_rect[0].rect.X)
                            {
                                LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.X = LVApp.Instance().m_Config.Cam3_rect[0].rect.X + 1;
                            }
                            if (LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y < LVApp.Instance().m_Config.Cam3_rect[0].rect.Y)
                            {
                                LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y = LVApp.Instance().m_Config.Cam3_rect[0].rect.Y - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.X + LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Width > LVApp.Instance().m_Config.Cam3_rect[0].rect.X + LVApp.Instance().m_Config.Cam3_rect[0].rect.Width)
                            {
                                LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Width = LVApp.Instance().m_Config.Cam3_rect[0].rect.X + LVApp.Instance().m_Config.Cam3_rect[0].rect.Width - LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.X - 1;
                            }
                            if (LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y + LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Height > LVApp.Instance().m_Config.Cam3_rect[0].rect.Y + LVApp.Instance().m_Config.Cam3_rect[0].rect.Height)
                            {
                                LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Height = LVApp.Instance().m_Config.Cam3_rect[0].rect.Y + LVApp.Instance().m_Config.Cam3_rect[0].rect.Height - LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y - 1;
                            }
                        }

                        if (t_idx == 0)
                        {
                            LVApp.Instance().m_mainform.ctr_ROI4.Change_initial_ROI();
                        }

                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[1][1] = LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.X;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[2][1] = LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Y;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[3][1] = LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Width;
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[4][1] = LVApp.Instance().m_Config.Cam3_rect[t_idx].rect.Height;

                        string str = LVApp.Instance().m_Config.Cam3_rect[t_idx].mUse == false ? "X" : "O";
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[1][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[2][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[3][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[4][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[5][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[6][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[7][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[8][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[9][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[10][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[11][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[12][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[13][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[14][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[15][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[16][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[17][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[18][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[19][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[20][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[21][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[22][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[23][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[24][1].ToString()
                            + "_" +
                            LVApp.Instance().m_Config.ds_DATA_3.Tables[3].Rows[25][1].ToString();
                    }
                    else
                    {
                        string str = mUse == false ? "X" : "O";
                        string[] t_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1].ToString().Split('_');
                        LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[t_idx][1] =
                            str
                            + "_" +
                            rect.X.ToString()
                            + "_" +
                            rect.Y.ToString()
                            + "_" +
                            rect.Width.ToString()
                            + "_" +
                            rect.Height.ToString()
                            + "_" +
                            t_str[5]
                            + "_" +
                            t_str[6]
                            + "_" +
                            t_str[7]
                            + "_" +
                            t_str[8]
                            + "_" +
                            t_str[9]
                            + "_" +
                            t_str[10]
                            + "_" +
                            t_str[11]
                            + "_" +
                            t_str[12]
                            + "_" +
                            t_str[13]
                            + "_" +
                            t_str[14]
                            + "_" +
                            t_str[15]
                            + "_" +
                            t_str[16]
                            + "_" +
                            t_str[17]
                            + "_" +
                            t_str[18]
                            + "_" +
                            t_str[19]
                            + "_" +
                            t_str[20]
                            + "_" +
                            t_str[21]
                            + "_" +
                            t_str[22]
                            + "_" +
                            t_str[23]
                            + "_" +
                            t_str[24]
                            + "_" +
                            t_str[25];
                    }
                }
            } else
            {
                using (Pen pen = new Pen(Color.Blue, 1))
                {
                    pen.DashStyle = DashStyle.Solid;
                    g.DrawRectangle(pen, rect);
                }
                Pen pen1 = new Pen(Color.HotPink, 1);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                int margin = 11;
                Point VP1 = new Point(rect.X + rect.Width / 2 - margin, rect.Y + rect.Height / 2);
                Point VP2 = new Point(rect.X + rect.Width / 2 + margin + 1, rect.Y + rect.Height / 2);
                g.DrawLine(pen1, VP1, VP2);
                Point HP1 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2 - margin);
                Point HP2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2 + margin + 1);
                g.DrawLine(pen1, HP1, HP2);
                pen1.Dispose();
                using (Pen pen = new Pen(Color.LightBlue, 1))
                {
                    pen.DashStyle = DashStyle.Solid;
                    g.DrawEllipse(pen, new Rectangle((rect.X + rect.Width / 2) - m_radius_w, (rect.Y + rect.Height / 2) - m_radius_h, 2 * m_radius_w, 2 * m_radius_h));
                }

                using (var myBrush = new SolidBrush(Color.FromArgb(100, 0, 255, 255)))
                {
                    g.FillEllipse(myBrush, new Rectangle((rect.X + rect.Width / 2) - m_radius_w, (rect.Y + rect.Height / 2) - m_radius_h, 2 * m_radius_w, 2 * m_radius_h));
                }
            }
            using (Font myFont = new Font("Times New Roman", 10))
            using (var myBrush = new SolidBrush(Color.FromArgb(200, 255, 0, 255)))
            {
                g.DrawString(m_ROI_Name, myFont, myBrush, rect.X, rect.Y);
                //g.DrawString("Masking Area", myFont, myBrush, (rect.X + rect.Width / 2) - 50, (rect.Y + rect.Height / 2) - m_radius_h/2);
            }
        }

        public void SetBitmapFile(string filename)
        {
            this.mBmp = new Bitmap(filename);
        }

        public void SetBitmap(Bitmap bmp)
        {
            this.mBmp = bmp;
        }

        public void SetPictureBox(PictureBox p)
        {
            this.mPictureBox = p;
            mPictureBox.MouseDown += new MouseEventHandler(mPictureBox_MouseDown);
            mPictureBox.MouseUp += new MouseEventHandler(mPictureBox_MouseUp);
            mPictureBox.MouseClick += new MouseEventHandler(mPictureBox_MouseClick);
            mPictureBox.MouseMove += new MouseEventHandler(mPictureBox_MouseMove);
            mPictureBox.Paint += new PaintEventHandler(mPictureBox_Paint);
        }

        private void mPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            //if (rect.Width*rect.Height == 0)
            //{
            //    mView = false;
            //}
            //else
            //{
            //    mView = true;
            //}
            try
            {
                Draw(e.Graphics);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }

        }
        private void mPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            if (rect.Contains(new Point(e.X, e.Y)))
            {
                mSelected = true;
                //if (e.Button == MouseButtons.Right)
                //{
                //    ContextMenu cm = new ContextMenu();
                //    cm.MenuItems.Add("Remove", new EventHandler(RemoveROI));
                //    this.mPictureBox.ContextMenu = cm;
                //    this.mPictureBox.ContextMenu.Show(this.mPictureBox, e.Location);
                //    this.mPictureBox.ContextMenu = null;
                //}
            }
            else
            {
                mSelected = false;
            }
            oldX = e.X;
            oldY = e.Y;
            TestIfRectInsideArea();

            mPictureBox.Invalidate();
        }

        private void mPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            //MessageBox.Show(t_idx.ToString() + "_" + LVApp.Instance().m_Config.ROI_Selected_IDX[m_Cam_Num].ToString());
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode && t_idx == 0)
            {
                mSelected = false;
                //if (m_Cam_Num == 0)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log("검사중에는 ROI 0번 설정이 불가합니다.");
                //    }
                //    else
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log("Can't ROI#0 move when running!");
                //    }
                //}
                //else if (m_Cam_Num == 1)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log("검사중에는 ROI 0번 설정이 불가합니다.");
                //    }
                //    else
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log("Can't ROI#0 move when running!");
                //    }
                //}
                //else if (m_Cam_Num == 2)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log("검사중에는 ROI 0번 설정이 불가합니다.");
                //    }
                //    else
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log("Can't ROI#0 move when running!");
                //    }
                //}
                //else if (m_Cam_Num == 3)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log("검사중에는 ROI 0번 설정이 불가합니다.");
                //    }
                //    else
                //    {
                //        LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log("Can't ROI#0 move when running!");
                //    }
                //}
                return;
            }

            mIsClick = true;

            nodeSelected = PosSizableRect.None;
            nodeSelected = GetNodeSelectable(e.Location);

            if (rect.Contains(new Point(e.X, e.Y)))
            {
                mMove = true;
                mSelected = true;
            } else
            {
                mSelected = false;
            }
            oldX = e.X;
            oldY = e.Y;
        }

        private void mPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            mIsClick = false;
            mMove = false;
        }

        private void mPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            ChangeCursor(e.Location);
            if (mIsClick == false)
            {
                return;
            }

            Rectangle backupRect = rect;

            switch (nodeSelected)
            {
                case PosSizableRect.LeftUp:
                    rect.X += e.X - oldX;
                    rect.Width -= e.X - oldX;
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.LeftMiddle:
                    rect.X += e.X - oldX;
                    rect.Width -= e.X - oldX;
                    mSelected = true;
                    break;
                case PosSizableRect.LeftBottom:
                    rect.Width -= e.X - oldX;
                    rect.X += e.X - oldX;
                    rect.Height += e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.BottomMiddle:
                    rect.Height += e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.RightUp:
                    rect.Width += e.X - oldX;
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.RightBottom:
                    rect.Width += e.X - oldX;
                    rect.Height += e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.RightMiddle:
                    rect.Width += e.X - oldX;
                    mSelected = true;
                    break;

                case PosSizableRect.UpMiddle:
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    mSelected = true;
                    break;

                default:
                    if (mMove)
                    {
                        rect.X = rect.X + e.X - oldX;
                        rect.Y = rect.Y + e.Y - oldY;
                        mPictureBox.Cursor = Cursors.SizeAll;
                    }
                    break;
            }
            oldX = e.X;
            oldY = e.Y;

            if (rect.Width < 5 || rect.Height < 5)
            {
                rect = backupRect;
            }

            TestIfRectInsideArea();

            mPictureBox.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            // Test if rectangle still inside the area.
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (rect.Width <= 0) rect.Width = 1;
            if (rect.Height <= 0) rect.Height = 1;

            if (rect.X + rect.Width > mPictureBox.Width)
            {
                rect.Width = mPictureBox.Width - rect.X - 1; // -1 to be still show 
                if (allowDeformingDuringMovement == false)
                {
                    mIsClick = false;
                }
            }
            if (rect.Y + rect.Height > mPictureBox.Height)
            {
                rect.Height = mPictureBox.Height - rect.Y - 1;// -1 to be still show 
                if (allowDeformingDuringMovement == false)
                {
                    mIsClick = false;
                }
            }
        }

        private Rectangle CreateRectSizableNode(int x, int y)
        {
            return new Rectangle(x - sizeNodeRect / 2, y - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
        }

        private Rectangle GetRect(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return CreateRectSizableNode(rect.X, rect.Y);

                case PosSizableRect.LeftMiddle:
                    return CreateRectSizableNode(rect.X, rect.Y + +rect.Height / 2);

                case PosSizableRect.LeftBottom:
                    return CreateRectSizableNode(rect.X, rect.Y + rect.Height);

                case PosSizableRect.BottomMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y + rect.Height);

                case PosSizableRect.RightUp:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y);

                case PosSizableRect.RightBottom:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y + rect.Height);

                case PosSizableRect.RightMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y + rect.Height / 2);

                case PosSizableRect.UpMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y);
                default:
                    return new Rectangle();
            }
        }

        private PosSizableRect GetNodeSelectable(Point p)
        {
            foreach (PosSizableRect r in Enum.GetValues(typeof(PosSizableRect)))
            {
                if (GetRect(r).Contains(p))
                {
                    return r;
                }
            }
            return PosSizableRect.None;
        }

        private void ChangeCursor(Point p)
        {
            mPictureBox.Cursor = GetCursor(GetNodeSelectable(p));
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Cursor GetCursor(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return Cursors.SizeNWSE;

                case PosSizableRect.LeftMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.LeftBottom:
                    return Cursors.SizeNESW;

                case PosSizableRect.BottomMiddle:
                    return Cursors.SizeNS;

                case PosSizableRect.RightUp:
                    return Cursors.SizeNESW;

                case PosSizableRect.RightBottom:
                    return Cursors.SizeNWSE;

                case PosSizableRect.RightMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////
    public class UserRect2
    {
        private PictureBox mPictureBox;
        public Rectangle rect;
        public bool allowDeformingDuringMovement = true;
        private bool mIsClick = false;
        private bool mMove = false;
        private int oldX;
        private int oldY;
        private int sizeNodeRect = 5;
        private Bitmap mBmp = null;
        private PosSizableRect nodeSelected = PosSizableRect.None;
        public bool mSelected = true;
        public string m_ROI_Name = "";
        public bool mView = true;
        private enum PosSizableRect
        {
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None

        };

        public UserRect2(Rectangle r)
        {
            rect = r;
            mIsClick = false;
        }

        public void Draw(Graphics g)
        {
            if (rect.Width <= 5 || rect.Height <= 5)
            {
                return;
            }
            if (mSelected)
            {
                using (Pen pen = new Pen(Color.Purple, 1))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(pen, rect);
                }

                Pen pen1 = new Pen(Color.LightGoldenrodYellow, 1);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                int margin = 11;
                Point VP1 = new Point(rect.X + rect.Width / 2 - margin, rect.Y + rect.Height / 2);
                Point VP2 = new Point(rect.X + rect.Width / 2 + margin + 1, rect.Y + rect.Height / 2);
                g.DrawLine(pen1, VP1, VP2);
                Point HP1 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2 - margin);
                Point HP2 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2 + margin + 1);
                g.DrawLine(pen1, HP1, HP2);
                pen1.Dispose();

                foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
                {
                    g.DrawRectangle(new Pen(Color.Red, 1), GetRect(pos));
                }
            }
            else
            {
                using (Pen pen = new Pen(Color.Blue, 1))
                {
                    pen.DashStyle = DashStyle.Solid;
                    g.DrawRectangle(pen, rect);
                }
            }
            using (Font myFont = new Font("Times New Roman", 10))
            using (var myBrush = new SolidBrush(Color.FromArgb(200, 255, 50, 200)))
            {
                g.DrawString(m_ROI_Name, myFont, myBrush, rect.X, rect.Y);
            }
        }

        public void SetBitmapFile(string filename)
        {
            this.mBmp = new Bitmap(filename);
        }

        public void SetBitmap(Bitmap bmp)
        {
            this.mBmp = bmp;
        }

        public void SetPictureBox(PictureBox p)
        {
            this.mPictureBox = p;
            mPictureBox.MouseDown += new MouseEventHandler(mPictureBox_MouseDown);
            mPictureBox.MouseUp += new MouseEventHandler(mPictureBox_MouseUp);
            mPictureBox.MouseClick += new MouseEventHandler(mPictureBox_MouseClick);
            mPictureBox.MouseMove += new MouseEventHandler(mPictureBox_MouseMove);
            mPictureBox.Paint += new PaintEventHandler(mPictureBox_Paint);
        }

        private void mPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            //if (rect.Width*rect.Height == 0)
            //{
            //    mView = false;
            //}
            //else
            //{
            //    mView = true;
            //}
            try
            {
                Draw(e.Graphics);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }

        }
        private void mPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            if (rect.Contains(new Point(e.X, e.Y)))
            {
                mSelected = true;
                //if (e.Button == MouseButtons.Right)
                //{
                //    ContextMenu cm = new ContextMenu();
                //    cm.MenuItems.Add("Remove", new EventHandler(RemoveROI));
                //    this.mPictureBox.ContextMenu = cm;
                //    this.mPictureBox.ContextMenu.Show(this.mPictureBox, e.Location);
                //    this.mPictureBox.ContextMenu = null;
                //}
            }
            else
            {
                mSelected = false;
            }
            oldX = e.X;
            oldY = e.Y;
            TestIfRectInsideArea();

            mPictureBox.Invalidate();
        }

        private void mPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            mIsClick = true;

            nodeSelected = PosSizableRect.None;
            nodeSelected = GetNodeSelectable(e.Location);

            if (rect.Contains(new Point(e.X, e.Y)))
            {
                mMove = true;
                mSelected = true;
            }
            else
            {
                mSelected = false;
            }
            oldX = e.X;
            oldY = e.Y;
        }

        private void mPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            mIsClick = false;
            mMove = false;
        }

        private void mPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mView)
            {
                return;
            }
            ChangeCursor(e.Location);
            if (mIsClick == false)
            {
                return;
            }

            Rectangle backupRect = rect;

            switch (nodeSelected)
            {
                case PosSizableRect.LeftUp:
                    rect.X += e.X - oldX;
                    rect.Width -= e.X - oldX;
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.LeftMiddle:
                    rect.X += e.X - oldX;
                    rect.Width -= e.X - oldX;
                    mSelected = true;
                    break;
                case PosSizableRect.LeftBottom:
                    rect.Width -= e.X - oldX;
                    rect.X += e.X - oldX;
                    rect.Height += e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.BottomMiddle:
                    rect.Height += e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.RightUp:
                    rect.Width += e.X - oldX;
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.RightBottom:
                    rect.Width += e.X - oldX;
                    rect.Height += e.Y - oldY;
                    mSelected = true;
                    break;
                case PosSizableRect.RightMiddle:
                    rect.Width += e.X - oldX;
                    mSelected = true;
                    break;

                case PosSizableRect.UpMiddle:
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    mSelected = true;
                    break;

                default:
                    if (mMove)
                    {
                        rect.X = rect.X + e.X - oldX;
                        rect.Y = rect.Y + e.Y - oldY;
                        mPictureBox.Cursor = Cursors.SizeAll;
                    }
                    break;
            }
            oldX = e.X;
            oldY = e.Y;

            if (rect.Width < 5 || rect.Height < 5)
            {
                rect = backupRect;
            }

            TestIfRectInsideArea();

            mPictureBox.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            // Test if rectangle still inside the area.
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (rect.Width <= 0) rect.Width = 1;
            if (rect.Height <= 0) rect.Height = 1;

            if (rect.X + rect.Width > mPictureBox.Width)
            {
                rect.Width = mPictureBox.Width - rect.X - 1; // -1 to be still show 
                if (allowDeformingDuringMovement == false)
                {
                    mIsClick = false;
                }
            }
            if (rect.Y + rect.Height > mPictureBox.Height)
            {
                rect.Height = mPictureBox.Height - rect.Y - 1;// -1 to be still show 
                if (allowDeformingDuringMovement == false)
                {
                    mIsClick = false;
                }
            }
        }

        private Rectangle CreateRectSizableNode(int x, int y)
        {
            return new Rectangle(x - sizeNodeRect / 2, y - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
        }

        private Rectangle GetRect(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return CreateRectSizableNode(rect.X, rect.Y);

                case PosSizableRect.LeftMiddle:
                    return CreateRectSizableNode(rect.X, rect.Y + +rect.Height / 2);

                case PosSizableRect.LeftBottom:
                    return CreateRectSizableNode(rect.X, rect.Y + rect.Height);

                case PosSizableRect.BottomMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y + rect.Height);

                case PosSizableRect.RightUp:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y);

                case PosSizableRect.RightBottom:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y + rect.Height);

                case PosSizableRect.RightMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width, rect.Y + rect.Height / 2);

                case PosSizableRect.UpMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width / 2, rect.Y);
                default:
                    return new Rectangle();
            }
        }

        private PosSizableRect GetNodeSelectable(Point p)
        {
            foreach (PosSizableRect r in Enum.GetValues(typeof(PosSizableRect)))
            {
                if (GetRect(r).Contains(p))
                {
                    return r;
                }
            }
            return PosSizableRect.None;
        }

        private void ChangeCursor(Point p)
        {
            mPictureBox.Cursor = GetCursor(GetNodeSelectable(p));
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Cursor GetCursor(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return Cursors.SizeNWSE;

                case PosSizableRect.LeftMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.LeftBottom:
                    return Cursors.SizeNESW;

                case PosSizableRect.BottomMiddle:
                    return Cursors.SizeNS;

                case PosSizableRect.RightUp:
                    return Cursors.SizeNESW;

                case PosSizableRect.RightBottom:
                    return Cursors.SizeNWSE;

                case PosSizableRect.RightMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }
    }

    public static class ChartControl
    {
        public static double[] Input_NG = { 0, 0, 0 }; // NG
        public static double[] Input_OK = { 0, 0, 0 }; // OK
        // Call this method from the Form_Load method, passing your ZedGraphControl
        public static int m_Cam_Num = 3;
        public static void CreateChart1(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the title and axis labels
            myPane.Title.Text = "Stacked Bars with Value Labels Inside Each Bar";
            myPane.XAxis.Title.Text = "Position Number";
            myPane.YAxis.Title.Text = "Some Random Thing";

            // Create points for three BarItems
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();

            // Use random data values
            Random rand = new Random();
            for (int i = 1; i < 5; i++)
            {
                double y = (double)i;
                double x1 = 100.0 + rand.NextDouble() * 100.0;
                double x2 = 100.0 + rand.NextDouble() * 100.0;
                double x3 = 100.0 + rand.NextDouble() * 100.0;

                list1.Add(x1, y);
                list2.Add(x2, y);
                list3.Add(x3, y);
            }

            // Create the three BarItems, change the fill properties so the angle is at 90
            // degrees for horizontal bars
            BarItem bar1 = myPane.AddBar("Bar 1", list1, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red, 90);
            BarItem bar2 = myPane.AddBar("Bar 2", list2, Color.Blue);
            bar2.Bar.Fill = new Fill(Color.Blue, Color.White, Color.Blue, 90);
            BarItem bar3 = myPane.AddBar("Bar 3", list3, Color.Green);
            bar3.Bar.Fill = new Fill(Color.Green, Color.White, Color.Green, 90);

            // Set BarBase to the YAxis for horizontal bars
            myPane.BarSettings.Base = BarBase.Y;
            // Make the bars stack instead of cluster
            myPane.BarSettings.Type = BarType.Stack;

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.FromArgb(255, 255, 166), 45.0F);

            zgc.AxisChange();

            // Create TextObj's to provide labels for each bar
            BarItem.CreateBarLabels(myPane, true, "f0");
        }

        // Call this method from the Form_Load method, passing your ZedGraphControl
        public static void CreateChart2(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.IsFontsScaled = false;
            //myPane.YAxis.Title.FontSpec.Size = 14.0f;

            // Set the title and axis labels
            //myPane.Title.Text = "Vertical Bars with Value Labels Above Each Bar";
            //myPane.XAxis.Title.Text = "Camera Number";
            //myPane.YAxis.Title.Text = "Some Random Thing";
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.IsVisible = false;
            myPane.YAxis.Title.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.YAxis.IsVisible = false;

            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            //PointPairList list3 = new PointPairList();
            Random rand = new Random();


            // Generate random data for three curves
            for (int i = 0; i < 4; i++)
            {
                double x = (double)i;
                double y = rand.NextDouble() * 10;
                double y2 = rand.NextDouble() * 10;
                //double y3 = rand.NextDouble() * 1000;
                list.Add(x, y);
                list2.Add(x, y2);
                //list3.Add(x, y3);
            }

            // create the curves
            BarItem myCurve = myPane.AddBar("OK", list, Color.Blue);
            BarItem myCurve2 = myPane.AddBar("NG", list2, Color.Red);
            //BarItem myCurve3 = myPane.AddBar("curve 3", list3, Color.Green);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.FromArgb(200, 200, 200), 90.0F);

            zgc.AxisChange();

            // expand the range of the Y axis slightly to accommodate the labels
            myPane.YAxis.Scale.Max += myPane.YAxis.Scale.MajorStep;
            myPane.XAxis.Scale.Min = -0.5;
            myPane.XAxis.Scale.Max = 3.5;
            // Create TextObj's to provide labels for each bar
            BarItem.CreateBarLabels(myPane, false, "f0");
        }

        public static void Reset(ZedGraphControl zgc)
        {
            for (int i = 0; i < 3; i++)
            { Input_NG[i] = 0; Input_OK[i] = 0; }
            Refresh(zgc);
            //zgc.Invalidate();
        }

        public static void Refresh(ZedGraphControl zgc)
        {
            if (zgc.InvokeRequired)
            {
                zgc.Invoke((MethodInvoker)delegate
                {
                    zgc.GraphPane.CurveList.Clear();
                    zgc.GraphPane.GraphObjList.Clear();
                    zgc.Invalidate();
                    CreateChart3(zgc);
                });
            }
            else
            {
                zgc.GraphPane.CurveList.Clear();
                zgc.GraphPane.GraphObjList.Clear();
                zgc.Invalidate();
                CreateChart3(zgc);
            }                        
        }
        // Call this method from the Form_Load method, passing your ZedGraphControl
        public static void CreateChart3(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.IsFontsScaled = false;
            //myPane.YAxis.Title.FontSpec.Size = 14.0f;

            //myPane.BarSettings.ClusterScaleWidth = 7;
            //myPane.BarSettings.ClusterScaleWidthAuto = false;
            // Set the title and axis labels
            myPane.Title.Text = "Vertical Bars with Value Labels Above Each Bar";
            //myPane.XAxis.Title.Text = "Camera Number";
            //myPane.YAxis.Title.Text = "Some Random Thing";
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.IsVisible = false;
            myPane.YAxis.Title.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.YAxis.IsVisible = false;


            double[] tInput_OK = new double[m_Cam_Num];
            // Manually sum up the curves
            for (int i = 0; i < m_Cam_Num; i++)
                tInput_OK[i] = Input_OK[i] + Input_NG[i];


            // Create the three BarItems, change the fill properties so the angle is at 90
            // degrees for horizontal bars
            BarItem bar1 = myPane.AddBar("NG", null, Input_NG, Color.DarkRed);
            bar1.Bar.Fill = new Fill(Color.DarkRed, Color.White, Color.DarkRed, 0);
            BarItem bar2 = myPane.AddBar("OK", null, tInput_OK, Color.DarkBlue);
            bar2.Bar.Fill = new Fill(Color.DarkBlue, Color.White, Color.DarkBlue, 0);
 
            myPane.XAxis.MajorTic.IsBetweenLabels = true;

            if (m_Cam_Num == 1)
            {
                // Enter some data values
                string[] str = { "Cam0"};
                // Set the XAxis labels
                myPane.XAxis.Scale.TextLabels = str;
            }
            else if (m_Cam_Num == 2)
            {
                // Enter some data values
                string[] str = { "Cam0", "Cam1"};
                // Set the XAxis labels
                myPane.XAxis.Scale.TextLabels = str;
            }
            else
            {
                // Enter some data values
                string[] str = { "Cam0", "Cam1", "Cam2" };
                // Set the XAxis labels
                myPane.XAxis.Scale.TextLabels = str;
            }
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Set BarBase to the YAxis for horizontal bars
            myPane.BarSettings.Base = BarBase.X;
            // Make the bars stack instead of cluster
            myPane.BarSettings.Type = BarType.Stack;


            //// Generate a red bar with "Curve 1" in the legend
            //CurveItem myCurve = myPane.AddBar("OK", null, y, Color.Red);



            //// Generate a blue bar with "Curve 2" in the legend
            //myCurve = myPane.AddBar("NG", null, y2, Color.Blue);


            // Draw the X tics between the labels instead of at the labels
            //myPane.XAxis.MajorTic.IsBetweenLabels = false;

            // Set the XAxis to the ordinal type
            //myPane.XAxis.Type = AxisType.Ordinal;

            //Add Labels to the curves

            // Shift the text items up by 5 user scale units above the bars

            for (int i = 0; i < m_Cam_Num; i++)
            {
                if (tInput_OK[i] > 0)
                {
                    // format the label string to have 1 decimal place
                    string lab = tInput_OK[i].ToString("F0");
                    // create the text item (assumes the x axis is ordinal or text)
                    // for negative bars, the label appears just above the zero value
                    TextObj text = new TextObj(lab, (float)(i + 1), tInput_OK.Max() + tInput_OK.Max() / 2.7);
                    // tell Zedgraph to use user scale units for locating the TextItem
                    text.Location.CoordinateFrame = CoordType.AxisXYScale;
                    // AlignH the left-center of the text to the specified point
                    text.ZOrder = ZOrder.A_InFront;
                    //text.FontSpec.Border.IsVisible = false;
                    //text.FontSpec.Fill.IsVisible = false;
                    // rotate the text 90 degrees
                    text.FontSpec.Angle = 0;
                    // add the TextItem to the list
                    myPane.GraphObjList.Add(text);

                    var ttext = new TextObj(Input_NG[i].ToString(), i + 1, tInput_OK.Max() + tInput_OK.Max() / 10, CoordType.ChartFraction,
                                           AlignH.Center, AlignV.Center);
                    ttext.Location.CoordinateFrame = CoordType.AxisXYScale;
                    ttext.ZOrder = ZOrder.A_InFront;
                    ttext.FontSpec.FontColor = System.Drawing.Color.DarkRed;
                    ttext.FontSpec.Border.IsVisible = false;
                    ttext.FontSpec.Fill.IsVisible = false;
                    myPane.GraphObjList.Add(ttext);

                    var tttext = new TextObj(Input_OK[i].ToString(), i + 1, tInput_OK.Max() + tInput_OK.Max() / 4.6, CoordType.ChartFraction,
                                           AlignH.Center, AlignV.Center);
                    tttext.Location.CoordinateFrame = CoordType.AxisXYScale;
                    tttext.FontSpec.FontColor = System.Drawing.Color.DarkBlue;
                    tttext.FontSpec.Border.IsVisible = false;
                    tttext.FontSpec.Fill.IsVisible = false;
                    tttext.ZOrder = ZOrder.A_InFront;
                    myPane.GraphObjList.Add(tttext);
                }
            }

            // Indicate that the bars are overlay type, which are drawn on top of eachother
            myPane.BarSettings.Type = BarType.Overlay;

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.FromArgb(200, 200, 200), 45.0F);
            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            // Add one step to the max scale value to leave room for the labels
            myPane.YAxis.Scale.Max = tInput_OK.Max() * 1.5;
            //myPane.XAxis.Scale.Min = -0.5;
            //myPane.XAxis.Scale.Max = 3.5;
            // Create TextObj's to provide labels for each bar
            //BarItem.CreateBarLabels(myPane, true, "f0");

        }
    }


    public class DongleKey
    {
        // Error Code
        static public uint LC_SUCCESS = 0;  // Successful
        static public uint LC_OPEN_DEVICE_FAILED = 1;  // Open device failed
        static public uint LC_FIND_DEVICE_FAILED = 2;  // No matching device was found
        static public uint LC_INVALID_PARAMETER = 3;  // Parameter Error
        static public uint LC_INVALID_BLOCK_NUMBER = 4;  // Block Error
        static public uint LC_HARDWARE_COMMUNICATE_ERROR = 5;  // Communication error with hardware
        static public uint LC_INVALID_PASSWORD = 6;  // Invalid Password
        static public uint LC_ACCESS_DENIED = 7;  // No privileges
        static public uint LC_ALREADY_OPENED = 8;  // Device is open
        static public uint LC_ALLOCATE_MEMORY_FAILED = 9;  // Allocate memory failed
        static public uint LC_INVALID_UPDATE_PACKAGE = 10; // Invalid update package
        static public uint LC_SYN_ERROR = 11; // thread Synchronization error
        static public uint LC_OTHER_ERROR = 12;// Other unknown exceptions


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        // Hardware information structure
        public struct LC_hardware_info
        {
            public int developerNumber;             // Developer ID
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] serialNumber;             // Unique Device Serial Number
            public int setDate;                     // Manufacturing date
            public int reservation;                // Reserve
        }

        // Software information structure
        public struct LC_software_info
        {
            public int version;        // Software edition
            public int reservation;    // Reserve
        }

        // LC API function interface

        [DllImport(@"lc.dll")]
        private static extern int LC_open(int vendor, int index, ref lc_handle_t handle);
        [DllImport(@"lc.dll")]
        private static extern int LC_close(lc_handle_t handle);
        [DllImport(@"lc.dll")]
        private static extern int LC_passwd(lc_handle_t handle, int type, byte[] passwd);
        [DllImport(@"lc.dll")]
        private static extern int LC_read(lc_handle_t handle, int block, byte[] buffer);
        [DllImport(@"lc.dll")]
        private static extern int LC_write(lc_handle_t handle, int block, byte[] buffer);
        [DllImport(@"lc.dll")]
        private static extern int LC_encrypt(lc_handle_t handle, byte[] plaintext, byte[] ciphertext);
        [DllImport(@"lc.dll")]
        private static extern int LC_decrypt(lc_handle_t handle, byte[] ciphertext, byte[] plaintext);
        [DllImport(@"lc.dll")]
        private static extern int LC_set_passwd(lc_handle_t handle, int type, byte[] newpasswd, int retries);
        [DllImport(@"lc.dll")]
        private static extern int LC_change_passwd(lc_handle_t handle, int type, byte[] oldpasswd, byte[] newpasswd);
        [DllImport(@"lc.dll")]
        private static extern int LC_get_hardware_info(lc_handle_t handle, ref LC_hardware_info info);
        [DllImport(@"lc.dll")]
        private static extern int LC_get_software_info(ref LC_software_info info);
        [DllImport(@"lc.dll")]
        private static extern int LC_hmac(lc_handle_t handle, byte[] text, int textlen, byte[] digest);
        [DllImport(@"lc.dll")]
        private static extern int LC_hmac_software(byte[] text, int textlen, byte[] key, byte[] digest);
        [DllImport(@"lc.dll")]
        private static extern int LC_update(lc_handle_t handle, byte[] buffer);
        [DllImport(@"lc.dll")]
        private static extern int LC_set_key(lc_handle_t handle, int type, byte[] key);

        public bool Check_License()
        {
            try
            {
                lc_handle_t handle = 0;
                int res;

                // open device
                res = LC_open(0, 0, ref handle);
                if (res != LC_SUCCESS && res != LC_ALREADY_OPENED)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        MessageBox.Show("검사를 시작할 수 없습니다. 장비 제작사로 문의 바랍니다.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        MessageBox.Show("Can't inspection. Please contact to manufacture company.");
                    }
                    //LC_close(handle);
                    return false;
                }
                // verify user password
                byte[] passwd = { (byte)'L', (byte)'V', (byte)'2', (byte)'0', (byte)'2', (byte)'0', (byte)'8', (byte)'8' };
                res = LC_passwd(handle, 0, passwd);

                if (res != LC_SUCCESS)
                {
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        MessageBox.Show("검사를 시작할 수 없습니다. 장비 제작사로 문의 바랍니다.");
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        MessageBox.Show("Can't inspection. Please contact to manufacture company.");
                    }
                    LC_close(handle);
                    return false;
                }

                LC_close(handle);
                return true;

                // Read Block 0
                //byte[] outdata = new byte[512]; // at least 512 bytes

                //res = LC_read(handle, 0, outdata);
                //if (res != LC_SUCCESS)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {//한국어
                //        MessageBox.Show("검사를 시작할 수 없습니다. 장비 제작사로 문의 바랍니다.");
                //    }
                //    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                //    {//영어
                //        MessageBox.Show("Can't inspection. Please contact to manufacture company.");
                //    }
                //    LC_close(handle);
                //    return false;
                //}

                //if (outdata[16] == 0x31)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {//한국어
                //        MessageBox.Show("검사를 시작할 수 없습니다. 장비 제작사로 문의 바랍니다.");
                //    }
                //    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                //    {//영어
                //        MessageBox.Show("Can't inspection. Please contact to manufacture company.");
                //    }

                //    LC_close(handle);
                //    return false;
                //}

                //String str1 = System.Text.Encoding.Default.GetString(outdata);

                //DateTime dti01 = Convert.ToDateTime(str1.Substring(0, 10));

                //if (dti01.CompareTo(DateTime.Now) < 0)
                //{
                //    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                //    {//한국어
                //        MessageBox.Show("검사를 시작할 수 없습니다. 장비 제작사로 문의 바랍니다.");
                //    }
                //    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                //    {//영어
                //        MessageBox.Show("Can't inspection. Please contact to manufacture company.");
                //    }
                //    outdata[16] = 0x31;
                //    res = LC_write(handle, 0, outdata);
                //    if (res != LC_SUCCESS)
                //    {
                //        LC_close(handle);
                //        return false;
                //    }
                //    // 현재일 보다 빠르면  
                //}
                //else
                //{
                //    // 현재일 보다 느리면  
                //} 

                //System.IO.File.WriteAllText(@"0_block.txt", str1);

                //LVApp.Instance().m_mainform.add_Log(str1);

                //LC_close(handle);
                //return true;
            }
            catch
            {
                return false;
            }
        }
    }
    /// <summary>
    /// A class that aids the writing of a (hopefully) fully valid CSV file.
    /// </summary>
    public class CSVWriter
    {
        /// <summary>
        /// The destination stream to which we will be writing the csv.
        /// </summary>
        private StreamWriter destination;
        /// <summary>
        /// Whether the underlying filestream is open or closed. True if the stream is open, false if closed.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return destination.BaseStream != null;
            }
        }

        /// <summary>
        /// Whether the headers have been writteen yet or not.
        /// </summary>
        private bool headersWritten = false;
        private bool file_opened = false;
        /// <summary>
        /// Whether the headers have been written.
        /// Note that attempting to write the headers again when they have already been written results in an exception been thrown.
        /// </summary>
        public bool HeadersWritten
        {
            get
            {
                return headersWritten;
            }
            set
            {
                headersWritten = value;
            }
        }

        /// <summary>
        /// The number of fields that were specified when writing the headers.
        /// </summary>
        private int fieldCount;

        /// <summary>
        /// The number of fields declared when writing the headers. Records written to the CSV writer must have this number of fields, even if they are just empty strings.
        /// </summary>
        public int FieldCount
        {
            get
            {
                return fieldCount;
            }
        }

        /// <summary>
        /// The number of records that have been written to the output stream so far.
        /// </summary>
        private int recordsWritten = 0;
        
        /// <summary>
        /// The number of records written to the output stream so far. Note that this doesn't include the header line.
        /// </summary>
        public int RecordsWritten
        {
            get
            {
                return recordsWritten;
            }
        }

        /// <summary>
        /// Creates a new CSV writer that will write out to the given filename.
        /// </summary>
        /// <param name="filename">The filename to write the csv to.</param>
        public CSVWriter(string filename)
            : this(new StreamWriter(filename, true, System.Text.Encoding.UTF8, 128))
        {
        }

        /// <summary>
        /// Creates a new CSV writer that will write to the given StreamWriter.
        /// </summary>
        /// <param name="inDestination">The StreamWriter to write the csv to.</param>
        public CSVWriter(StreamWriter inDestination)
        {
            destination = inDestination;
        }

        /// <summary>
        /// Encapsulates a given string in quotes if it contains a comma.
        /// Rather useful when preparing cell data for writing.
        /// </summary>
        /// <param name="cellData">The string to check.</param>
        /// <returns>The properly escaped string.</returns>
        private string escapeCellData(string cellData)
        {
            if (cellData == null)
            {
                return cellData;
            }
            if (cellData.Contains(","))
                return String.Format("\"{0}\"", cellData);
            else
                return cellData;
        }

        /// <summary>
        /// Writes the header contained in the given array to the file.
        /// </summary>
        /// <param name="inHeaders">The headers to write to the csv file.</param>
        public void WriteHeader(string[] inHeaders)
        {
            try
            {
                file_opened = true;
                // Blow up if we try to write the headersmeor than once.
                if (headersWritten)
                {
                    fieldCount = inHeaders.Length;
                    destination.AutoFlush = true;
                    return;
                    //throw new InvalidOperationException("You cannot write the CSV headers more than once.");
                }

                // Save the number of headers for later
                fieldCount = inHeaders.Length;

                // Loop over each header and prepare it for writing. This encapsulates the header in quotes if it contians a comma.
                string[] preparedHeaders = new string[inHeaders.Length];
                for (int i = 0; i < inHeaders.Length; i++)
                {
                    preparedHeaders[i] = escapeCellData(inHeaders[i]);
                }
                destination.AutoFlush = true;
                destination.WriteLine(String.Join(",", preparedHeaders));

                headersWritten = true;
            }
            catch
            {
                headersWritten = false;
            }
        }

        /// <summary>
        /// Writes a new record to the csv file.
        /// Note that the number of fields in the data provided must the equal to that declared when writing the header.
        /// </summary>
        /// <param name="data">An array of records to write out.</param>
        public void WriteRecord(string[] data)
        {
            try
            {
                if (!IsOpen || !file_opened)
                {
                return;
            }
            //if (data.Length != FieldCount)
            //    throw new FormatException(String.Format("Data does not have the expected number of fields. Expected: {0}, Actual: {1}", data.Length, FieldCount));

            //string[] preparedData = new string[data.Length];
            //for (int i = 0; i < data.Length; i++)
            //{
            //    preparedData[i] = escapeCellData(data[i]);
            //}

            //destination.WriteLine(String.Join(",", preparedData));

            //recordsWritten++;

            if (FieldCount > 0)
            {
                string[] preparedData = new string[FieldCount];
                for (int i = 0; i < FieldCount; i++)
                {
                    if (i < data.Length && data[i] != null)
                    {
                        if (data[i].Length > 0)
                        {
                            preparedData[i] = escapeCellData(data[i]);
                        }
                        else
                        {
                            preparedData[i] = "-1";
                        }
                    }
                    else
                    {
                        preparedData[i] = "-1";
                    }
                }

                destination.WriteLine(String.Join(",", preparedData));

                recordsWritten++;

            }
        }
            catch
            { }
        }

        /// <summary>
        /// Closes the underlying StreamWriter.
        /// Don't attempt to write to the CSV writer after you've closed the file, as this will throw an exception!
        /// </summary>
        public void Close()
        {
            try
            {
                file_opened = false;
                destination.Close();
            }
            catch
            { }
        }
    }
}
