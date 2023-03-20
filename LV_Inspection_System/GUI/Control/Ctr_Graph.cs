using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Threading;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Graph : UserControl
    {
        public Ctr_Graph()
        {
            InitializeComponent();
            zedGraphControl1.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);
            zedGraphControl2.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);
            zedGraphControl3.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);
            zedGraphControl4.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);
        }


        private string MyPointValueHandler(object sender, GraphPane pane, CurveItem curve, int iPt)
        {
            #region
            PointPair pt = curve[iPt];
            XDate the_date = new XDate(pt.X);
            string out_str = "";
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {
                out_str = curve.Label.Text + " : 수율 " + pt.Y.ToString("f2") + "% on " + the_date.DateTime;
            }
            else
            {
                out_str = curve.Label.Text + ": Yield " + pt.Y.ToString("f2") + "% on " + the_date.DateTime;
            }
            return out_str;
            #endregion
        }

        public void CreateGraphAll()
        {
            bool[] C_Cam = new bool[4];
            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
            {
                zedGraphControl1.Visible = false;
                C_Cam[0] = false;
            }
            else
            {
                zedGraphControl1.Visible = true;
                C_Cam[0] = true;
            }

            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
            {
                zedGraphControl2.Visible = false;
                C_Cam[1] = false;
            }
            else
            {
                zedGraphControl2.Visible = true;
                C_Cam[1] = true;
            }

            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
            {
                zedGraphControl3.Visible = false;
                C_Cam[2] = false;
            }
            else
            {
                zedGraphControl3.Visible = true;
                C_Cam[2] = true;
            }

            if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
            {
                zedGraphControl4.Visible = false;
                C_Cam[3] = false;
            }
            else
            {
                zedGraphControl4.Visible = true;
                C_Cam[3] = true;
            }

            splitContainer1.Panel1MinSize = 0;
            splitContainer1.Panel2MinSize = 0;
            splitContainer3.Panel1MinSize = 0;
            splitContainer3.Panel2MinSize = 0;
            splitContainer2.Panel1MinSize = 0;
            splitContainer2.Panel2MinSize = 0;
            splitContainer1.SplitterWidth = 1;
            splitContainer3.SplitterWidth = 1;
            splitContainer2.SplitterWidth = 1;
            splitContainer1.IsSplitterFixed = false;
            splitContainer3.IsSplitterFixed = false;
            splitContainer2.IsSplitterFixed = false;
            splitContainer2.Panel1.Controls.Add(zedGraphControl1);
            splitContainer2.Panel2.Controls.Add(zedGraphControl3);
            splitContainer3.Panel1.Controls.Add(zedGraphControl2);
            splitContainer3.Panel2.Controls.Add(zedGraphControl4);

            if (C_Cam[0] && !C_Cam[1] && !C_Cam[2] && !C_Cam[3])
            {//0번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = true;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && C_Cam[1] && !C_Cam[2] && !C_Cam[3])
            {//1번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                // 세로 패널
                splitContainer1.SplitterDistance = 0;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = true;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                //CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && !C_Cam[1] && C_Cam[2] && !C_Cam[3])
            {//2번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = 0;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = true;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                //CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && !C_Cam[1] && !C_Cam[2] && C_Cam[3])
            {//3번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                // 세로 패널
                splitContainer1.SplitterDistance = 0;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = 0;
                splitContainer1.IsSplitterFixed = true;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                //CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && C_Cam[1] && !C_Cam[2] && !C_Cam[3])
            {//0,1번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && !C_Cam[1] && C_Cam[2] && !C_Cam[3])
            {//0,2번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl1);
                splitContainer2.Panel2.Controls.Add(zedGraphControl2);
                splitContainer3.Panel1.Controls.Add(zedGraphControl3);
                splitContainer3.Panel2.Controls.Add(zedGraphControl4);
                CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && !C_Cam[1] && !C_Cam[2] && C_Cam[3])
            {//0,3번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl1);
                splitContainer2.Panel2.Controls.Add(zedGraphControl3);
                splitContainer3.Panel1.Controls.Add(zedGraphControl4);
                splitContainer3.Panel2.Controls.Add(zedGraphControl2);
                CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && C_Cam[1] && C_Cam[2] && !C_Cam[3])
            {//1,2번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl2);
                splitContainer2.Panel2.Controls.Add(zedGraphControl1);
                splitContainer3.Panel1.Controls.Add(zedGraphControl3);
                splitContainer3.Panel2.Controls.Add(zedGraphControl4);
                //CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && C_Cam[1] && !C_Cam[2] && C_Cam[3])
            {//1,3번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl2);
                splitContainer2.Panel2.Controls.Add(zedGraphControl1);
                splitContainer3.Panel1.Controls.Add(zedGraphControl4);
                splitContainer3.Panel2.Controls.Add(zedGraphControl3);
                //CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && !C_Cam[1] && C_Cam[2] && C_Cam[3])
            {//2,3번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = true;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl3);
                splitContainer2.Panel2.Controls.Add(zedGraphControl1);
                splitContainer3.Panel1.Controls.Add(zedGraphControl4);
                splitContainer3.Panel2.Controls.Add(zedGraphControl2);
                //CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && C_Cam[1] && C_Cam[2] && !C_Cam[3])
            {//0,1,2번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = false;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = false;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl1);
                splitContainer2.Panel2.Controls.Add(zedGraphControl4);
                splitContainer3.Panel1.Controls.Add(zedGraphControl2);
                splitContainer3.Panel2.Controls.Add(zedGraphControl3);
                CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                //CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && C_Cam[1] && !C_Cam[2] && C_Cam[3])
            {//0,1,3번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = false;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = false;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl1);
                splitContainer2.Panel2.Controls.Add(zedGraphControl3);
                splitContainer3.Panel1.Controls.Add(zedGraphControl2);
                splitContainer3.Panel2.Controls.Add(zedGraphControl4);
                CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                //CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && !C_Cam[1] && C_Cam[2] && C_Cam[3])
            {//0,2,3번만
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = false;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = false;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl1);
                splitContainer2.Panel2.Controls.Add(zedGraphControl2);
                splitContainer3.Panel1.Controls.Add(zedGraphControl3);
                splitContainer3.Panel2.Controls.Add(zedGraphControl4);
                CreateGraph(zedGraphControl1, 0);
                //CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (!C_Cam[0] && C_Cam[1] && C_Cam[2] && C_Cam[3])
            {//1,2,3번만
                zedGraphControl1.Visible = false;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = false;
                splitContainer2.IsSplitterFixed = true;
                splitContainer2.Panel1.Controls.Add(zedGraphControl2);
                splitContainer2.Panel2.Controls.Add(zedGraphControl1);
                splitContainer3.Panel1.Controls.Add(zedGraphControl3);
                splitContainer3.Panel2.Controls.Add(zedGraphControl4);
                //CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
            else if (C_Cam[0] && C_Cam[1] && C_Cam[2] && C_Cam[3])
            {//0,1,2,3번
                zedGraphControl1.Visible = true;
                zedGraphControl2.Visible = true;
                zedGraphControl3.Visible = true;
                zedGraphControl4.Visible = true;
                // 왼쪽 패널
                splitContainer2.SplitterDistance = splitContainer2.Height / 2;
                // 세로 패널
                splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                // 오른쪽 패널
                splitContainer3.SplitterDistance = splitContainer3.Height / 2;
                splitContainer1.IsSplitterFixed = false;
                splitContainer3.IsSplitterFixed = false;
                splitContainer2.IsSplitterFixed = false;
                splitContainer2.Panel1.Controls.Add(zedGraphControl1);
                splitContainer2.Panel2.Controls.Add(zedGraphControl3);
                splitContainer3.Panel1.Controls.Add(zedGraphControl2);
                splitContainer3.Panel2.Controls.Add(zedGraphControl4);
                CreateGraph(zedGraphControl1, 0);
                CreateGraph(zedGraphControl2, 1);
                CreateGraph(zedGraphControl3, 2);
                CreateGraph(zedGraphControl4, 3);
            }
        }

        LineItem[,] myCurve = new LineItem[4, 41];
        private void CreateGraph(ZedGraphControl zgc, int Cam_num)
        {
            zgc.GraphPane.CurveList.Clear();
            zgc.GraphPane.GraphObjList.Clear();
            GraphPane myPane = zgc.GraphPane;
            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.IsFontsScaled = false;
            myPane.Legend.FontSpec.Size = 12;
            myPane.Legend.Fill.Color = Color.Transparent;
            myPane.Legend.Border.IsVisible = false;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {
                myPane.XAxis.Title.Text = "시간";
                myPane.YAxis.Title.Text = "CAM" + Cam_num.ToString() + " 수율(%)";
            }
            else
            {
                myPane.XAxis.Title.Text = "Time";
                myPane.YAxis.Title.Text = "CAM" + Cam_num.ToString() + " Yield(%)";
            }
            //myPane.XAxis.Scale.Format = "f3";
            //myPane.XAxis.Type = AxisType.Linear;
            myPane.XAxis.Type = AxisType.Date;

            //myPane.XAxis.Scale.Format = "HH:mm:ss\nyy/MM/dd";
            //myPane.YAxis.Scale.Min = 0;            // We want to use time from now
            //myPane.YAxis.Scale.Max = 100;        // to 5 min per default
            //myPane.XAxis.Scale.MinorUnit = DateUnit.Second;              // set the minimum x unit to time/seconds
            //myPane.XAxis.Scale.MajorUnit = DateUnit.Minute;              // set the maximum x unit to time/minutes
            //myPane.XAxis.Scale.MinorStep = 1;                            // Setta os tracinhos da reta.
            //myPane.XAxis.Scale.MajorStep = 10;                            // Setta o intervalo do tempo na Scale X.
            ////myPane02m.XAxis.Scale.MinGrace = 1;
            ////myPane02m.XAxis.Scale.MaxGrace = 10;

            Random r = new Random();
            for (int i = 0; i < 41; i++)
            {
                Color randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                if (Cam_num == 0)
                {
                    if (LVApp.Instance().m_Config.m_GraphData0[i].use)
                    {
                        myCurve[Cam_num, i] = myPane.AddCurve(LVApp.Instance().m_Config.m_GraphData0[i].name, LVApp.Instance().m_Config.m_GraphData0[i].list, randomColor,
                                        SymbolType.None);
                        // Make up some data points from the Sine function
                        // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
                        //LineItem myCurve = myPane.AddCurve("My Curve", list, Color.Blue,
                        //                        SymbolType.Circle);
                        // Fill the area under the curve with a white-red gradient at 45 degrees
                        //myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
                        // Make the symbols opaque by filling them with white
                        //myCurve[Cam_num, i].Symbol.Fill = new Fill(Color.White);
                    }
                }
                else if (Cam_num == 1)
                {
                    if (LVApp.Instance().m_Config.m_GraphData1[i].use)
                    {
                        myCurve[Cam_num, i] = myPane.AddCurve(LVApp.Instance().m_Config.m_GraphData1[i].name, LVApp.Instance().m_Config.m_GraphData1[i].list, randomColor,
                                        SymbolType.None);
                        // Make up some data points from the Sine function
                        // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
                        //LineItem myCurve = myPane.AddCurve("My Curve", list, Color.Blue,
                        //                        SymbolType.Circle);
                        // Fill the area under the curve with a white-red gradient at 45 degrees
                        //myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
                        // Make the symbols opaque by filling them with white
                        //myCurve.Symbol.Fill = new Fill(Color.White);
                    }
                }
                else if (Cam_num == 2)
                {
                    if (LVApp.Instance().m_Config.m_GraphData2[i].use)
                    {
                        myCurve[Cam_num, i] = myPane.AddCurve(LVApp.Instance().m_Config.m_GraphData2[i].name, LVApp.Instance().m_Config.m_GraphData2[i].list, randomColor,
                                        SymbolType.None);
                        // Make up some data points from the Sine function
                        // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
                        //LineItem myCurve = myPane.AddCurve("My Curve", list, Color.Blue,
                        //                        SymbolType.Circle);
                        // Fill the area under the curve with a white-red gradient at 45 degrees
                        //myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
                        //// Make the symbols opaque by filling them with white
                        //myCurve.Symbol.Fill = new Fill(Color.White);
                    }
                }
                else if (Cam_num == 3)
                {
                    if (LVApp.Instance().m_Config.m_GraphData3[i].use)
                    {
                        myCurve[Cam_num, i] = myPane.AddCurve(LVApp.Instance().m_Config.m_GraphData3[i].name, LVApp.Instance().m_Config.m_GraphData3[i].list, randomColor,
                                        SymbolType.None);
                        // Make up some data points from the Sine function
                        // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
                        //LineItem myCurve = myPane.AddCurve("My Curve", list, Color.Blue,
                        //                        SymbolType.Circle);
                        // Fill the area under the curve with a white-red gradient at 45 degrees
                        //myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
                        //// Make the symbols opaque by filling them with white
                        //myCurve.Symbol.Fill = new Fill(Color.White);
                    }
                }
            }
            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.Gainsboro, 45F);

            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 200), 45F);
            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
            zgc.Refresh();
        }

        public bool t_update_check = false;
        private void timer_Update_Tick(object sender, EventArgs e)
        {
            if (t_update_check)
            {
                return;
            }
            //CreateGraphAll();
            //this.Refresh();
            if (zedGraphControl1.InvokeRequired)
            {
                zedGraphControl1.Invoke((MethodInvoker)delegate
                {
                    if (zedGraphControl1.Visible)
                    {
                        zedGraphControl1.GraphPane.XAxis.Scale.MaxAuto = true;
                        zedGraphControl1.AxisChange();
                        zedGraphControl1.Invalidate();
                        zedGraphControl1.Refresh();
                    }

                });
            }
            else
            {
                if (zedGraphControl1.Visible)
                {
                    zedGraphControl1.GraphPane.XAxis.Scale.MaxAuto = true;
                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                    zedGraphControl1.Refresh();
                }

            }

            Thread.Sleep(10);
            if (zedGraphControl2.InvokeRequired)
            {
                zedGraphControl2.Invoke((MethodInvoker)delegate
                {
                    if (zedGraphControl2.Visible)
                    {
                        zedGraphControl2.GraphPane.XAxis.Scale.MaxAuto = true;
                        zedGraphControl2.AxisChange();
                        zedGraphControl2.Invalidate();
                        zedGraphControl2.Refresh();
                    }

                });
            }
            else
            {
                if (zedGraphControl2.Visible)
                {
                    zedGraphControl2.GraphPane.XAxis.Scale.MaxAuto = true;
                    zedGraphControl2.AxisChange();
                    zedGraphControl2.Invalidate();
                    zedGraphControl2.Refresh();
                }

            }

            Thread.Sleep(10);
            if (zedGraphControl3.InvokeRequired)
            {
                zedGraphControl3.Invoke((MethodInvoker)delegate
                {
                    if (zedGraphControl3.Visible)
                    {
                        zedGraphControl3.GraphPane.XAxis.Scale.MaxAuto = true;
                        zedGraphControl3.AxisChange();
                        zedGraphControl3.Invalidate();
                        zedGraphControl3.Refresh();
                    }

                });
            }
            else
            {
                if (zedGraphControl3.Visible)
                {
                    zedGraphControl3.GraphPane.XAxis.Scale.MaxAuto = true;
                    zedGraphControl3.AxisChange();
                    zedGraphControl3.Invalidate();
                    zedGraphControl3.Refresh();
                }

            }

            Thread.Sleep(10);
            if (zedGraphControl4.InvokeRequired)
            {
                zedGraphControl4.Invoke((MethodInvoker)delegate
                {
                    if (zedGraphControl4.Visible)
                    {
                        zedGraphControl4.GraphPane.XAxis.Scale.MaxAuto = true;
                        zedGraphControl4.AxisChange();
                        zedGraphControl4.Invalidate();
                        zedGraphControl4.Refresh();
                    }

                });
            }
            else
            {
                if (zedGraphControl4.Visible)
                {
                    zedGraphControl4.GraphPane.XAxis.Scale.MaxAuto = true;
                    zedGraphControl4.AxisChange();
                    zedGraphControl4.Invalidate();
                    zedGraphControl4.Refresh();
                }

            }


            //this.Refresh();
        }
    }
}
