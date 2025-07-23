using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_DataGrid : UserControl
    {
        DataTable table = new DataTable();

        public Ctr_DataGrid()
        {
            InitializeComponent();
        }

        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {

            }
            else
            {
                ContextMenu cm = new ContextMenu();
                cm.MenuItems.Add("초기화", new EventHandler(DataGrid1_Reset));
                dataGridView.ContextMenu = cm;
                dataGridView.ContextMenu.Show(dataGridView, e.Location);
                dataGridView.ContextMenu = null;
            }
        }

        private void DataGrid1_Reset(object sender, EventArgs e)
        {
            LVApp.Instance().m_Config.Initialize_Data_Log(0);
            LVApp.Instance().m_Config.Initialize_Data_Log(1);
            LVApp.Instance().m_Config.Initialize_Data_Log(2);
            LVApp.Instance().m_Config.Initialize_Data_Log(3);
            //LVApp.Instance().m_Config.Initialize_Data_Log(4);
            //LVApp.Instance().m_Config.ds_LOG.Clear();
            for (int i = 0; i < 4; i++)
            {
                Min_Max_Update(i);
            }
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        public void Min_Max_Update(int Cam_Num)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() {

                        Min_Max_Update(Cam_Num); 
                    
                    
                    }));
                }
                else
                {
                    //dataGridView_MINMAX.DataSource = null;
                    //DataTable t_tb = LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Clone();
                    if (LVApp.Instance().m_Config.ds_LOG == null)
                    {
                        return;
                    }
                    int t_Row_cnt = LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows.Count;
                    int t_Col_cnt = LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns.Count;

                    double[] minV = new double[t_Col_cnt];
                    double[] maxV = new double[t_Col_cnt];
                    double[] sumV = new double[t_Col_cnt];
                    double[] sumC = new double[t_Col_cnt];
                    for (int x = 0; x < t_Col_cnt; x++)
                    {
                        minV[x] = double.MaxValue;
                        maxV[x] = sumV[x] = sumC[x] = 0.0;
                    }
                    for (int y = 0; y < t_Row_cnt; y++)
                    {
                        double value = 0.0;

                        for (int x = 2; x < t_Col_cnt; x++)
                        {
                            if (double.TryParse(LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Rows[y][x].ToString(), out value))
                            {
                                if (value >= 0)
                                {
                                    if (value < minV[x]) minV[x] = value;
                                    if (value > maxV[x]) maxV[x] = value;
                                    sumV[x] += value;
                                    sumC[x] += 1.0;
                                }
                            }
                        }
                    }

                    table = null;
                    table = new DataTable();

                    for (int x = 0; x < t_Col_cnt; x++)
                    {
                        if (sumC[x] > 0)
                        {
                            sumV[x] /= sumC[x];
                        }
                        else
                        {
                            sumV[x] = 0.0; minV[x] = 0.0; maxV[x] = 0.0;
                        }
                        table.Columns.Add(LVApp.Instance().m_Config.ds_LOG.Tables[Cam_Num].Columns[x].ColumnName);
                    }
                    for (int y = 0; y < 4; y++)
                    {
                        table.Rows.Add();
                        if (y == 0)
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                table.Rows[y][0] = "최솟값(MIN)";
                            }
                            else
                            {
                                table.Rows[y][0] = "MIN. Value";
                            }
                            for (int x = 2; x < t_Col_cnt; x++)
                            {
                                table.Rows[y][x] = minV[x];
                            }
                        }
                        else if (y == 1)
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                table.Rows[y][0] = "최댓값(MAX)";
                            }
                            else
                            {
                                table.Rows[y][0] = "MAX. Value";
                            }
                            for (int x = 2; x < t_Col_cnt; x++)
                            {
                                table.Rows[y][x] = maxV[x];
                            }
                        }
                        else if (y == 2)
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                table.Rows[y][0] = "최댓값-최솟값(DIFF)";
                            }
                            else
                            {
                                table.Rows[y][0] = "MAX.-MIN. Value";
                            }
                            for (int x = 2; x < t_Col_cnt; x++)
                            {
                                table.Rows[y][x] = Math.Round(maxV[x] - minV[x], 3);
                            }
                        }
                        else if (y == 3)
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                table.Rows[y][0] = "평균값(AVG)";
                            }
                            else
                            {
                                table.Rows[y][0] = "Mean Value";
                            }
                            for (int x = 2; x < t_Col_cnt; x++)
                            {
                                table.Rows[y][x] = Math.Round(sumV[x], 3);
                            }
                        }
                    }
                    dataGridView_MINMAX.DataSource = table;
                    dataGridView_MINMAX.ClearSelection();
                    dataGridView_MINMAX.Refresh();
                }


            }
            catch
            { }
        }

        private void Ctr_DataGrid_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView_MINMAX.Rows)
                {
                    row.Height = dataGridView_MINMAX.Height / 3;
                }
                if ((dataGridView.ScrollBars & ScrollBars.Vertical) != ScrollBars.None) 
                {
                    splitContainer2.SplitterDistance = splitContainer2.Width - 17;
                }
                else
                {
                    splitContainer2.SplitterDistance = splitContainer2.Width;
                }
            }
            catch
            { }
        }
    }
}
