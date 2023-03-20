using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
//using System.Collections.Generic;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_NGLog : UserControl
    {
        public Bitmap[] t_Bitmap = new Bitmap[4];
        public string[] t_Time = new string[4];
        public string[] t_Value = new string[4];
        private string t_save_name = "";

        public struct NG_Data
        {
            public string _Time;
            public string _Value;
            public Bitmap _Image;
            public DataTable _Table;
        }

        protected List<NG_Data> CAM0_NG_List = new List<NG_Data>();
        protected List<NG_Data> CAM1_NG_List = new List<NG_Data>();
        protected List<NG_Data> CAM2_NG_List = new List<NG_Data>();
        protected List<NG_Data> CAM3_NG_List = new List<NG_Data>();

        public Ctr_NGLog()
        {
            InitializeComponent();
        }

        public void Insert_Data(int Cam_Num)
        {
            if (Cam_Num == 0)
            {
                if (listBox1.InvokeRequired)
                {
                    listBox1.Invoke((MethodInvoker)delegate
                    {
                        listBox1.ClearSelected();
                        DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                        DataRow t_Row;
                        string[] str_sub1 = t_Value[Cam_Num].Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Table.Columns.Add(str_sub2[0]);
                                }
                            }
                            t_Row = t_Table.NewRow();
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Row[i] = str_sub2[1];
                                }
                            }
                            t_Table.Rows.Add(t_Row);
                        }

                        NG_Data t_NG_Data = new NG_Data();
                        t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                        t_NG_Data._Value = t_Value[Cam_Num];
                        t_NG_Data._Image = t_Bitmap[Cam_Num];
                        t_NG_Data._Table = t_Table;
                        List_CAM0_NG = t_NG_Data;
                    });
                }
                else
                {
                    listBox1.ClearSelected();
                    DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                    DataRow t_Row;
                    string[] str_sub1 = t_Value[Cam_Num].Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Table.Columns.Add(str_sub2[0]);
                            }
                        }
                        t_Row = t_Table.NewRow();
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Row[i] = str_sub2[1];
                            }
                        }
                        t_Table.Rows.Add(t_Row);
                    }

                    NG_Data t_NG_Data = new NG_Data();
                    t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                    t_NG_Data._Value = t_Value[Cam_Num];
                    t_NG_Data._Image = t_Bitmap[Cam_Num];
                    t_NG_Data._Table = t_Table;
                    List_CAM0_NG = t_NG_Data;
                }
            }
            else if (Cam_Num == 1)
            {
                if (listBox2.InvokeRequired)
                {
                    listBox2.Invoke((MethodInvoker)delegate
                    {
                        listBox2.ClearSelected();
                        DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                        DataRow t_Row;
                        string[] str_sub1 = t_Value[Cam_Num].Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Table.Columns.Add(str_sub2[0]);
                                }
                            }
                            t_Row = t_Table.NewRow();
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Row[i] = str_sub2[1];
                                }
                            }
                            t_Table.Rows.Add(t_Row);
                        }

                        NG_Data t_NG_Data = new NG_Data();
                        t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                        t_NG_Data._Value = t_Value[Cam_Num];
                        t_NG_Data._Image = t_Bitmap[Cam_Num];
                        t_NG_Data._Table = t_Table;
                        List_CAM1_NG = t_NG_Data;
                    });
                }
                else
                {
                    listBox2.ClearSelected();
                    DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                    DataRow t_Row;
                    string[] str_sub1 = t_Value[Cam_Num].Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Table.Columns.Add(str_sub2[0]);
                            }
                        }
                        t_Row = t_Table.NewRow();
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Row[i] = str_sub2[1];
                            }
                        }
                        t_Table.Rows.Add(t_Row);
                    }

                    NG_Data t_NG_Data = new NG_Data();
                    t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                    t_NG_Data._Value = t_Value[Cam_Num];
                    t_NG_Data._Image = t_Bitmap[Cam_Num];
                    t_NG_Data._Table = t_Table;
                    List_CAM1_NG = t_NG_Data;
                }
                
            }
            else if (Cam_Num == 2)
            {
                if (listBox3.InvokeRequired)
                {
                    listBox3.Invoke((MethodInvoker)delegate
                    {
                        listBox3.ClearSelected();
                        DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                        DataRow t_Row;
                        string[] str_sub1 = t_Value[Cam_Num].Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Table.Columns.Add(str_sub2[0]);
                                }
                            }
                            t_Row = t_Table.NewRow();
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Row[i] = str_sub2[1];
                                }
                            }
                            t_Table.Rows.Add(t_Row);
                        }

                        NG_Data t_NG_Data = new NG_Data();
                        t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                        t_NG_Data._Value = t_Value[Cam_Num];
                        t_NG_Data._Image = t_Bitmap[Cam_Num];
                        t_NG_Data._Table = t_Table;
                        List_CAM2_NG = t_NG_Data;
                    });
                }
                else
                {
                    listBox3.ClearSelected();
                    DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                    DataRow t_Row;
                    string[] str_sub1 = t_Value[Cam_Num].Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Table.Columns.Add(str_sub2[0]);
                            }
                        }
                        t_Row = t_Table.NewRow();
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Row[i] = str_sub2[1];
                            }
                        }
                        t_Table.Rows.Add(t_Row);
                    }

                    NG_Data t_NG_Data = new NG_Data();
                    t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                    t_NG_Data._Value = t_Value[Cam_Num];
                    t_NG_Data._Image = t_Bitmap[Cam_Num];
                    t_NG_Data._Table = t_Table;
                    List_CAM2_NG = t_NG_Data;

                }
                
            }
            else if (Cam_Num == 3)
            {
                if (listBox4.InvokeRequired)
                {
                    listBox4.Invoke((MethodInvoker)delegate
                    {
                        listBox4.ClearSelected();
                        DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                        DataRow t_Row;
                        string[] str_sub1 = t_Value[Cam_Num].Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Table.Columns.Add(str_sub2[0]);
                                }
                            }
                            t_Row = t_Table.NewRow();
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    t_Row[i] = str_sub2[1];
                                }
                            }
                            t_Table.Rows.Add(t_Row);
                        }

                        NG_Data t_NG_Data = new NG_Data();
                        t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                        t_NG_Data._Value = t_Value[Cam_Num];
                        t_NG_Data._Image = t_Bitmap[Cam_Num];
                        t_NG_Data._Table = t_Table;
                        List_CAM3_NG = t_NG_Data;
                    });
                }
                else
                {
                    listBox4.ClearSelected();
                    DataTable t_Table = new DataTable(t_Bitmap[Cam_Num] + "_CAM" + Cam_Num.ToString());
                    DataRow t_Row;
                    string[] str_sub1 = t_Value[Cam_Num].Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Table.Columns.Add(str_sub2[0]);
                            }
                        }
                        t_Row = t_Table.NewRow();
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                t_Row[i] = str_sub2[1];
                            }
                        }
                        t_Table.Rows.Add(t_Row);
                    }

                    NG_Data t_NG_Data = new NG_Data();
                    t_NG_Data._Time = "CAM" + Cam_Num.ToString() + "_" + t_Time[Cam_Num];
                    t_NG_Data._Value = t_Value[Cam_Num];
                    t_NG_Data._Image = t_Bitmap[Cam_Num];
                    t_NG_Data._Table = t_Table;
                    List_CAM3_NG = t_NG_Data;
                }
                
            }
        }

        private NG_Data List_CAM0_NG
        {
            set
            {
                CAM0_NG_List.Insert(0, value);
                if (listBox1.InvokeRequired)
                {
                    listBox1.Invoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Insert(0, value._Time);
                        if (listBox1.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                        {
                            CAM0_NG_List.RemoveAt(listBox1.Items.Count - 1);
                            listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                        }
                    });
                }
                else
                {
                    listBox1.Items.Insert(0, value._Time);
                    if (listBox1.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                    {
                        CAM0_NG_List.RemoveAt(listBox1.Items.Count - 1);
                        listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                    }
                }
            }
        }

        public NG_Data List_CAM1_NG
        {
            set
            {
                CAM1_NG_List.Insert(0, value);
                if (listBox2.InvokeRequired)
                {
                    listBox2.Invoke((MethodInvoker)delegate
                    {
                        listBox2.Items.Insert(0, value._Time);
                        if (listBox2.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                        {
                            CAM1_NG_List.RemoveAt(listBox2.Items.Count - 1);
                            listBox2.Items.RemoveAt(listBox2.Items.Count - 1);
                        }
                    });
                }
                else
                {
                    listBox2.Items.Insert(0, value._Time);
                    if (listBox2.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                    {
                        CAM1_NG_List.RemoveAt(listBox2.Items.Count - 1);
                        listBox2.Items.RemoveAt(listBox2.Items.Count - 1);
                    }
                }
            }
        }

        public NG_Data List_CAM2_NG
        {
            set
            {
                CAM2_NG_List.Insert(0, value);
                if (listBox3.InvokeRequired)
                {
                    listBox3.Invoke((MethodInvoker)delegate
                    {
                        listBox3.Items.Insert(0, value._Time);
                        if (listBox3.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                        {
                            CAM2_NG_List.RemoveAt(listBox3.Items.Count - 1);
                            listBox3.Items.RemoveAt(listBox3.Items.Count - 1);
                        }
                    });
                }
                else
                {
                    listBox3.Items.Insert(0, value._Time);
                    if (listBox3.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                    {
                        CAM2_NG_List.RemoveAt(listBox3.Items.Count - 1);
                        listBox3.Items.RemoveAt(listBox3.Items.Count - 1);
                    }
                }
            }
        }

        public NG_Data List_CAM3_NG
        {
            set
            {
                CAM3_NG_List.Insert(0, value);
                if (listBox4.InvokeRequired)
                {
                    listBox4.Invoke((MethodInvoker)delegate
                    {
                        listBox4.Items.Insert(0, value._Time);
                        if (listBox4.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                        {
                            CAM3_NG_List.RemoveAt(listBox4.Items.Count - 1);
                            listBox4.Items.RemoveAt(listBox4.Items.Count - 1);
                        }
                    });
                }
                else
                {
                    listBox4.Items.Insert(0, value._Time);
                    if (listBox4.Items.Count > LVApp.Instance().m_Config.NG_Log_Max_CNT)
                    {
                        CAM3_NG_List.RemoveAt(listBox4.Items.Count - 1);
                        listBox4.Items.RemoveAt(listBox4.Items.Count - 1);
                    }
                }
            }
        }

        private void Ctr_NGLog_SizeChanged(object sender, EventArgs e)
        {
            UI_Update();
        }

        public void UI_Update()
        {
            try
            {
                TableLayoutRowStyleCollection styles = tableLayoutPanel1.RowStyles;

                int t_row_idx = 0;
                foreach (RowStyle style in styles)
                {
                    style.SizeType = SizeType.Percent;
                    if (t_row_idx == 0)
                    {
                        style.Height = 20;
                    }
                    else if (t_row_idx == 1)
                    {
                        style.Height = 5;
                    }
                    else if (t_row_idx == 2)
                    {
                        style.Height = 20;
                    }
                    else if (t_row_idx == 3)
                    {
                        style.Height = 5;
                    }
                    else if (t_row_idx == 4)
                    {
                        style.Height = 20;
                    }
                    else if (t_row_idx == 5)
                    {
                        style.Height = 5;
                    }
                    else if (t_row_idx == 6)
                    {
                        style.Height = 20;
                    }
                    else if (t_row_idx == 7)
                    {
                        style.Height = 5;
                    }
                    t_row_idx++;
                }


                bool[] C_Cam = new bool[4];
                if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                {
                    listBox1.Visible = button_RESET0.Visible = false;
                    C_Cam[0] = false;
                }
                else
                {
                    listBox1.Visible = button_RESET0.Visible = true;
                    C_Cam[0] = true;
                }

                if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                {
                    listBox2.Visible = button_RESET1.Visible = false;
                    C_Cam[1] = false;
                }
                else
                {
                    listBox2.Visible = button_RESET1.Visible = true;
                    C_Cam[1] = true;
                }

                if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                {
                    listBox3.Visible = button_RESET2.Visible = false;
                    C_Cam[2] = false;
                }
                else
                {
                    listBox3.Visible = button_RESET2.Visible = true;
                    C_Cam[2] = true;
                }

                if (LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                {
                    listBox4.Visible = button_RESET3.Visible = false;
                    C_Cam[3] = false;
                }
                else
                {
                    listBox4.Visible = button_RESET3.Visible = true;
                    C_Cam[3] = true;
                }
            }
            catch
            {
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke((MethodInvoker)delegate
                {
                    int t_idx = listBox1.SelectedIndex;
                    if (t_idx < 0)
                    {
                        return;
                    }
                    t_save_name = listBox1.Items[t_idx].ToString();
                    try
                    {
                        imageControl1.Image = CAM0_NG_List[t_idx]._Image;
                        imageControl1.Refresh();
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = CAM0_NG_List[t_idx]._Table;
                        dataGridView1.RowHeadersWidth = 24;
                        dataGridView1.ColumnHeadersHeight = 26;
                        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.ScrollBars = ScrollBars.Both;
                        dataGridView1.ClearSelection();
                        string[] str_sub1 = CAM0_NG_List[t_idx]._Value.Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    if (str_sub2[2].ToUpper().Contains("TRUE"))
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        dataGridView1.Refresh();
                        listBox2.ClearSelected();
                        listBox3.ClearSelected();
                        listBox4.ClearSelected();
                    }
                    catch
                    {

                    }
                });
            }
            else
            {
                int t_idx = listBox1.SelectedIndex;
                if (t_idx < 0)
                {
                    return;
                }

                t_save_name = listBox1.Items[t_idx].ToString();
                try
                {
                    imageControl1.Image = CAM0_NG_List[t_idx]._Image;
                    imageControl1.Refresh();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = CAM0_NG_List[t_idx]._Table;
                    dataGridView1.RowHeadersWidth = 24;
                    dataGridView1.ColumnHeadersHeight = 26;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ScrollBars = ScrollBars.Both;
                    dataGridView1.ClearSelection();
                    string[] str_sub1 = CAM0_NG_List[t_idx]._Value.Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                if (str_sub2[2].ToUpper().Contains("TRUE"))
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                }
                                else
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridView1.Refresh();
                    listBox2.ClearSelected();
                    listBox3.ClearSelected();
                    listBox4.ClearSelected();
                }
                catch
                {

                }
            }
            
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.InvokeRequired)
            {
                listBox2.Invoke((MethodInvoker)delegate
                {
                    int t_idx = listBox2.SelectedIndex;
                    if (t_idx < 0)
                    {
                        return;
                    }
                    t_save_name = listBox2.Items[t_idx].ToString();
                    try
                    {
                        imageControl1.Image = CAM1_NG_List[t_idx]._Image;
                        imageControl1.Refresh();
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = CAM1_NG_List[t_idx]._Table;
                        dataGridView1.RowHeadersWidth = 24;
                        dataGridView1.ColumnHeadersHeight = 26;
                        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.ScrollBars = ScrollBars.Both;
                        dataGridView1.ClearSelection();
                        string[] str_sub1 = CAM1_NG_List[t_idx]._Value.Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    if (str_sub2[2].ToUpper().Contains("TRUE"))
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        dataGridView1.Refresh();
                        listBox1.ClearSelected();
                        listBox3.ClearSelected();
                        listBox4.ClearSelected();
                    }
                    catch
                    {

                    }
                });
            }
            else
            {
                int t_idx = listBox2.SelectedIndex;
                if (t_idx < 0)
                {
                    return;
                }
                t_save_name = listBox2.Items[t_idx].ToString();
                try
                {
                    imageControl1.Image = CAM1_NG_List[t_idx]._Image;
                    imageControl1.Refresh();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = CAM1_NG_List[t_idx]._Table;
                    dataGridView1.RowHeadersWidth = 24;
                    dataGridView1.ColumnHeadersHeight = 26;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ScrollBars = ScrollBars.Both;
                    dataGridView1.ClearSelection();
                    string[] str_sub1 = CAM1_NG_List[t_idx]._Value.Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                if (str_sub2[2].ToUpper().Contains("TRUE"))
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                }
                                else
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridView1.Refresh();
                    listBox1.ClearSelected();
                    listBox3.ClearSelected();
                    listBox4.ClearSelected();
                }
                catch
                {

                }
            }
            
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.InvokeRequired)
            {
                listBox3.Invoke((MethodInvoker)delegate
                {
                    int t_idx = listBox3.SelectedIndex;
                    if (t_idx < 0)
                    {
                        return;
                    }
                    t_save_name = listBox3.Items[t_idx].ToString();
                    try
                    {
                        imageControl1.Image = CAM2_NG_List[t_idx]._Image;
                        imageControl1.Refresh();
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = CAM2_NG_List[t_idx]._Table;
                        dataGridView1.RowHeadersWidth = 24;
                        dataGridView1.ColumnHeadersHeight = 26;
                        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.ScrollBars = ScrollBars.Both;
                        dataGridView1.ClearSelection();
                        string[] str_sub1 = CAM2_NG_List[t_idx]._Value.Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    if (str_sub2[2].ToUpper().Contains("TRUE"))
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        dataGridView1.Refresh();
                        listBox1.ClearSelected();
                        listBox2.ClearSelected();
                        listBox4.ClearSelected();
                    }
                    catch
                    {

                    }

                });
            }
            else
            {
                int t_idx = listBox3.SelectedIndex;
                if (t_idx < 0)
                {
                    return;
                }
                t_save_name = listBox3.Items[t_idx].ToString();
                try
                {
                    imageControl1.Image = CAM2_NG_List[t_idx]._Image;
                    imageControl1.Refresh();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = CAM2_NG_List[t_idx]._Table;
                    dataGridView1.RowHeadersWidth = 24;
                    dataGridView1.ColumnHeadersHeight = 26;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ScrollBars = ScrollBars.Both;
                    dataGridView1.ClearSelection();
                    string[] str_sub1 = CAM2_NG_List[t_idx]._Value.Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                if (str_sub2[2].ToUpper().Contains("TRUE"))
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                }
                                else
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridView1.Refresh();
                    listBox1.ClearSelected();
                    listBox2.ClearSelected();
                    listBox4.ClearSelected();
                }
                catch
                {

                }
            }
            
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox4.InvokeRequired)
            {
                listBox4.Invoke((MethodInvoker)delegate
                {
                    int t_idx = listBox4.SelectedIndex;
                    if (t_idx < 0)
                    {
                        return;
                    }
                    t_save_name = listBox4.Items[t_idx].ToString();
                    try
                    {
                        imageControl1.Image = CAM3_NG_List[t_idx]._Image;
                        imageControl1.Refresh();
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = CAM3_NG_List[t_idx]._Table;
                        dataGridView1.RowHeadersWidth = 24;
                        dataGridView1.ColumnHeadersHeight = 26;
                        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.ScrollBars = ScrollBars.Both;
                        dataGridView1.ClearSelection();
                        string[] str_sub1 = CAM3_NG_List[t_idx]._Value.Split('!');
                        if (str_sub1.Length > 0)
                        {
                            for (int i = 0; i < str_sub1.Length; i++)
                            {
                                string[] str_sub2 = str_sub1[i].Split('^');
                                if (str_sub2.Length == 3)
                                {
                                    if (str_sub2[2].ToUpper().Contains("TRUE"))
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        dataGridView1.Refresh();
                        listBox1.ClearSelected();
                        listBox2.ClearSelected();
                        listBox3.ClearSelected();
                    }
                    catch
                    {

                    }
                });
            }
            else
            {
                int t_idx = listBox4.SelectedIndex;
                if (t_idx < 0)
                {
                    return;
                }
                t_save_name = listBox4.Items[t_idx].ToString();
                try
                {
                    imageControl1.Image = CAM3_NG_List[t_idx]._Image;
                    imageControl1.Refresh();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = CAM3_NG_List[t_idx]._Table;
                    dataGridView1.RowHeadersWidth = 24;
                    dataGridView1.ColumnHeadersHeight = 26;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ScrollBars = ScrollBars.Both;
                    dataGridView1.ClearSelection();
                    string[] str_sub1 = CAM3_NG_List[t_idx]._Value.Split('!');
                    if (str_sub1.Length > 0)
                    {
                        for (int i = 0; i < str_sub1.Length; i++)
                        {
                            string[] str_sub2 = str_sub1[i].Split('^');
                            if (str_sub2.Length == 3)
                            {
                                if (str_sub2[2].ToUpper().Contains("TRUE"))
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.White;
                                }
                                else
                                {
                                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.Red;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridView1.Refresh();
                    listBox1.ClearSelected();
                    listBox2.ClearSelected();
                    listBox3.ClearSelected();
                }
                catch
                {

                }
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (imageControl1.Image == null)
            {
                return;
            }

            using (System.Drawing.Image bmp = (System.Drawing.Image)imageControl1.Image.Clone())
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
                LVApp.Instance().m_mainform.add_Log("图像保存");
            }

            SaveFileDialog1.Filter = "All image files|*.jpg;*.bmp;*.png";
            SaveFileDialog1.FilterIndex = 2;
            SaveFileDialog1.FileName = t_save_name + ".bmp";
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

        private void button_ScreenSave_Click(object sender, EventArgs e)
        {
            if (imageControl1.Image == null)
            {
                return;
            }

            Rectangle bounds = splitContainer1.Bounds;
            Point location = splitContainer1.PointToScreen(Point.Empty);
            bounds.X += location.X;
            bounds.Y += location.Y;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }
                if (bitmap == null)
                {
                    return;
                }
                else
                {
                    Image_SaveFileDialog(bitmap);
                }
            }
        }

        public void Reset_NGLog()
        {
            CAM0_NG_List.Clear();
            listBox1.Items.Clear();

            CAM1_NG_List.Clear();
            listBox2.Items.Clear();

            CAM2_NG_List.Clear();
            listBox3.Items.Clear();

            CAM3_NG_List.Clear();
            listBox4.Items.Clear();
            
            imageControl1.Image = null;
            dataGridView1.DataSource = null;
        }

        private void button_RESET0_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "NG 로그를 초기화 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "Do you want to reset NG Log?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "是否要重置 NG 日志?";
            }
            if (MessageBox.Show(msg, " RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CAM0_NG_List.Clear();
                listBox1.Items.Clear();
                imageControl1.Image = null;
                dataGridView1.DataSource = null;
            }
        }

        private void button_RESET1_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "NG 로그를 초기화 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "Do you want to reset NG Log?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "是否要重置 NG 日志?";
            }
            if (MessageBox.Show(msg, " RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CAM1_NG_List.Clear();
                listBox2.Items.Clear();
                imageControl1.Image = null;
                dataGridView1.DataSource = null;
            }
        }

        private void button_RESET2_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "NG 로그를 초기화 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "Do you want to reset NG Log?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "是否要重置 NG 日志?";
            }
            if (MessageBox.Show(msg, " RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CAM2_NG_List.Clear();
                listBox3.Items.Clear();
                imageControl1.Image = null;
                dataGridView1.DataSource = null;
            }
        }

        private void button_RESET3_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                msg = "NG 로그를 초기화 하시겠습니까?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                msg = "Do you want to reset NG Log?";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                msg = "是否要重置 NG 日志?";
            }
            if (MessageBox.Show(msg, " RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CAM3_NG_List.Clear();
                listBox4.Items.Clear();
                imageControl1.Image = null;
                dataGridView1.DataSource = null;
            }
        }
    }
}
