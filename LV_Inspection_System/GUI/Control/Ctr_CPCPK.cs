using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_CPCPK : UserControl
    {
        //Thread객체 생성
        BackgroundWorker worker = new BackgroundWorker();
        private string t_file = "";
        private DataTable dt = new DataTable();
        private DataSet ds_CPCPK;

        public Ctr_CPCPK()
        {
            InitializeComponent();
            //Update_UI();

            // 이벤트 핸들러 지정
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerReportsProgress = true;
        }

        public void DB_Initialize(string [] header)
        {

            dataGridView.DataSource = null;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (ds_CPCPK != null)
            {
                ds_CPCPK = null;
            }
            ds_CPCPK = new DataSet();
            //==========================================================================================================//
            // 메인에 ds_YIELD 초기화
                DataTable table_cpcpk = new DataTable("CPCPK");
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    table_cpcpk.Columns.Add("구분");//0
                    int t_col_cnt = header.Length - 1;
                    if (t_col_cnt < 5)
                    {
                        t_col_cnt = 5;
                    }
                    string[] t_row = new string[t_col_cnt];
                    for (int i = 2; i < header.Length; i++)
                    {
                        table_cpcpk.Columns.Add(header[i].Substring(6,header[i].Length-6));//0
                        t_row[i - 1] = "";
                    }

                    t_row[0] = "상한규격";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "하한규격";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "최대치";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "최소치";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "평균";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "표준편차";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "Cp";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "Cpu";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "Cpl";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "Cpk";
                    table_cpcpk.Rows.Add(t_row);

                    t_row[0] = "";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "항목별 불량수";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "검사수량"; t_row[1] = "양품수량"; t_row[2] = "불량수량"; t_row[3] = "수율"; t_row[4] = "불량률";
                    table_cpcpk.Rows.Add(t_row);
                    t_row[0] = "0"; t_row[1] = "0"; t_row[2] = "0"; t_row[3] = "0"; t_row[4] = "0";
                    table_cpcpk.Rows.Add(t_row);

                }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                table_cpcpk.Columns.Add("ITEM");//0
                int t_col_cnt = header.Length - 1;
                if (t_col_cnt < 5)
                {
                    t_col_cnt = 5;
                }
                string[] t_row = new string[t_col_cnt];
                for (int i = 2; i < header.Length; i++)
                {
                    table_cpcpk.Columns.Add(header[i]);//0
                    t_row[i - 1] = "0";
                }

                t_row[0] = "USL";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "LSL";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "MAX";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "MIN";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "AVG";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "STD";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cp";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cpu";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cpl";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cpk";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "NG ITEM";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "TOTAL CNT"; t_row[1] = "OK CNT"; t_row[2] = "NG CNT"; t_row[3] = "YIELD"; t_row[4] = "ERROR RATE";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "0"; t_row[1] = "0"; t_row[2] = "0"; t_row[3] = "0"; t_row[4] = "0";
                table_cpcpk.Rows.Add(t_row);
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                table_cpcpk.Columns.Add("ITEM");//0
                int t_col_cnt = header.Length - 1;
                if (t_col_cnt < 5)
                {
                    t_col_cnt = 5;
                }
                string[] t_row = new string[t_col_cnt];
                for (int i = 2; i < header.Length; i++)
                {
                    table_cpcpk.Columns.Add(header[i]);//0
                    t_row[i - 1] = "0";
                }

                t_row[0] = "USL";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "LSL";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "MAX";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "MIN";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "AVG";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "STD";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cp";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cpu";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cpl";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "Cpk";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "NG ITEM";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "TOTAL CNT"; t_row[1] = "OK CNT"; t_row[2] = "NG CNT"; t_row[3] = "YIELD"; t_row[4] = "ERROR RATE";
                table_cpcpk.Rows.Add(t_row);
                t_row[0] = "0"; t_row[1] = "0"; t_row[2] = "0"; t_row[3] = "0"; t_row[4] = "0";
                table_cpcpk.Rows.Add(t_row);
            }
            ds_CPCPK.Tables.Add(table_cpcpk);
                dataGridView.DataSource = ds_CPCPK.Tables[0];
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.AllowUserToResizeColumns = false;
                dataGridView.AllowUserToResizeRows = false;
                dataGridView.RowHeadersWidth = 5;
                dataGridView.ColumnHeadersHeight = 26;
                dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                //dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
                dataGridView.RowTemplate.Height = 80;
                dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView.Font = new System.Drawing.Font("맑은 고딕", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                dataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                dataGridView.Columns[0].Width = 100;
                dataGridView.ReadOnly = true;
                dataGridView.Rows[10].DefaultCellStyle.BackColor = Color.DimGray;

                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                //for (int i = 0; i < dataGridView.RowCount; i++)
                //{
                //    dataGridView.Rows[i].Height = dataGridView.Height/15;
                //}

                dataGridView.ClearSelection();
        }

        // Worker Thread가 실제 하는 일
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var lineCount = (double)File.ReadLines(t_file).Count();

                if (lineCount <= 1)
                {
                    return;
                }

                using (StreamReader reader = new StreamReader(t_file))
                {
                    string line;
                    double t_curr_line = 0; int pct = 0;
                    double t_real_total = 0; double t_real_NG = 0;

                    string[] headers = reader.ReadLine().Split(',');
                    if (headers[0] != "Result")
                    {
                        MessageBox.Show("Please open Total.csv");
                        return;
                    }

                    if (this.dataGridView.InvokeRequired)
                    {
                        this.dataGridView.Invoke((MethodInvoker)delegate
                        {
                            DB_Initialize(headers);
                        });
                    }
                    else
                    {
                        DB_Initialize(headers);
                    }


                    List<double>[] data = new List<double>[headers.Length - 2];
                    for (int i = 0; i < headers.Length - 2; i++)
                    {
                        data[i] = new List<double>();
                    }

                    DataTable dt = new DataTable();
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }

                    while ((line = reader.ReadLine()) != null)
                    {
                        //Define pattern
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                        //Separating columns to array
                        string[] rows = CSVParser.Split(line);

                        /* Do something with X */

                        if (rows[0] != "Result")
                        {
                            t_real_total++;
                            if (rows[0].Contains("NG"))
                            {
                                t_real_NG++;
                            }
                            bool t_fill_check = true;

                            for (int i = 0; i < headers.Length; i++)
                            {
                                if (i > 1)
                                {
                                    if (rows[i].Length < 1)
                                    {
                                        t_fill_check = false;
                                    }
                                    else
                                    {
                                        if (double.Parse(rows[i]) < 0)
                                        {
                                            t_fill_check = false;
                                        }
                                    }

                                    if (!t_fill_check)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (t_fill_check)
                            {
                                //Random t_rand = new Random();
                                DataRow dr = dt.NewRow();
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    dr[i] = rows[i];
                                    if (i > 1)
                                    {
                                        data[i - 2].Add(double.Parse(rows[i]));// + (double)t_rand.Next(5));
                                    }
                                }
                                dt.Rows.Add(dr);
                            }
                        }

                        pct = (int)((++t_curr_line * 100) / lineCount);
                        worker.ReportProgress(pct);
                    }

                    string t_new_file = t_file.Substring(0, t_file.Length - 9) + "Processed_Total.csv";
                    if (File.Exists(t_new_file))
                    {
                        File.Delete(t_new_file);
                    }

                    dt.WriteToCsvFile(t_new_file);

                    t_new_file = t_file.Substring(0, t_file.Length - 9) + "Spec.csv";

                    if (!File.Exists(t_new_file))
                    {
                        LVApp.Instance().m_Config.Spec_Log_Save(t_new_file);
                    }

                    using (StreamReader spec_reader = new StreamReader(t_new_file))
                    {
                        line = spec_reader.ReadLine();
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] rows_USL = CSVParser.Split(spec_reader.ReadLine());
                        string[] rows_LSL = CSVParser.Split(spec_reader.ReadLine());

                        for (int i = 0; i < headers.Length - 2; i++)
                        {
                            double v_USL = double.Parse(rows_USL[i + 1]);
                            double v_LSL = double.Parse(rows_LSL[i + 1]);
                            double v_max = data[i].Max();
                            double v_min = data[i].Min();
                            double v_mean = data[i].Mean();
                            double v_std = data[i].StandardDeviation();
                            bool t_check_std_zero = false;
                            if (v_std <= 1.0/10000.0)
                            {
                                v_std = 1 / 100;
                                t_check_std_zero = true;
                            }
                            double o_cp = (v_USL - v_LSL) / (6 * v_std);
                            double o_cpu = Math.Abs((v_USL - v_mean) / (3 * v_std));
                            double o_cpl = Math.Abs((v_mean - v_LSL) / (3 * v_std));
                            //double o_cpk = Math.Max(o_cpu, o_cpl);
                            double v_k = (((v_USL + v_LSL) / 2) - v_mean) / ((v_USL - v_LSL) / 2);
                            double o_cpk = (1 - v_k) * o_cp;

                            ds_CPCPK.Tables[0].Rows[0][i + 1] = v_USL.ToString("0.000");
                            ds_CPCPK.Tables[0].Rows[1][i + 1] = v_LSL.ToString("0.000");
                            ds_CPCPK.Tables[0].Rows[2][i + 1] = v_max.ToString("0.000");
                            ds_CPCPK.Tables[0].Rows[3][i + 1] = v_min.ToString("0.000");
                            ds_CPCPK.Tables[0].Rows[4][i + 1] = v_mean.ToString("0.000");
                            ds_CPCPK.Tables[0].Rows[5][i + 1] = v_std.ToString("0.000");

                            if (!t_check_std_zero)
                            {
                                ds_CPCPK.Tables[0].Rows[6][i + 1] = o_cp.ToString("0.000");
                                ds_CPCPK.Tables[0].Rows[7][i + 1] = o_cpu.ToString("0.000");
                                ds_CPCPK.Tables[0].Rows[8][i + 1] = o_cpl.ToString("0.000");
                                ds_CPCPK.Tables[0].Rows[9][i + 1] = o_cpk.ToString("0.000");
                            }
                            else
                            {
                                ds_CPCPK.Tables[0].Rows[6][i + 1] = "None";
                                ds_CPCPK.Tables[0].Rows[7][i + 1] = "None";
                                ds_CPCPK.Tables[0].Rows[8][i + 1] = "None";
                                ds_CPCPK.Tables[0].Rows[9][i + 1] = "None";
                            }

                            int v_OK = 0;
                            int v_NG = 0;
                            //int[] v_OK = Enumerable.Repeat<int>(0, headers.Length - 2).ToArray<int>();
                            //int[] v_NG = Enumerable.Repeat<int>(0, headers.Length - 2).ToArray<int>();
                            for (int j = 0; j < data[i].Count; j++)
                            {
                                if (data[i][j] >= v_LSL && data[i][j] <= v_USL)
                                {
                                    v_OK++;
                                }
                                else
                                {
                                    v_NG++;
                                }
                            }
                            //v_NG = (int)t_real_total - v_OK;
                            ds_CPCPK.Tables[0].Rows[11][i + 1] = v_NG;
                        }
                    }

                    ds_CPCPK.Tables[0].Rows[13][0] = (int)t_real_total;
                    ds_CPCPK.Tables[0].Rows[13][1] = (int)(t_real_total - t_real_NG);
                    ds_CPCPK.Tables[0].Rows[13][2] = (int)t_real_NG;
                    ds_CPCPK.Tables[0].Rows[13][3] = (100 * (t_real_total - t_real_NG) / (t_real_total)).ToString("0.000");
                    ds_CPCPK.Tables[0].Rows[13][4] = (100 * (t_real_NG) / (t_real_total)).ToString("0.000");

                    t_new_file = t_file.Substring(0, t_file.Length - 9) + "Processed_Total.csv";
                    ds_CPCPK.Tables[0].WriteToCsvFile(t_new_file);

                    worker.ReportProgress(100);
                }
            }
            catch
            {
                worker.ReportProgress(0);
            }
        }

        // Progress 리포트 - UI Thread
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke((MethodInvoker)delegate
                {
                    this.progressBar1.Value = e.ProgressPercentage;
                });
            }
            else
            {
                this.progressBar1.Value = e.ProgressPercentage;
            }
        }
        
        // 작업 완료 - UI Thread
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //// 에러가 있는지 체크
            //if (e.Error != null)
            //{
            //    lblMsg.Text = e.Error.Message;
            //    MessageBox.Show(e.Error.Message, "Error");
            //    return;
            //}
            
            //lblMsg.Text = "성공적으로 완료되었습니다";
        }      

        private void Ctr_CPCPK_SizeChanged(object sender, EventArgs e)
        {
            //splitContainer1.SplitterDistance = splitContainer1.Width / 2;
            //splitContainer2.SplitterDistance = splitContainer1.Width / 2;
        }

        private void button_LOAD_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }
            
            t_file = "";
            if (LVApp.Instance().m_Config.m_Log_Save_Folder == "")
            {
                OpenFileDialog openPanel = new OpenFileDialog();
                openPanel.InitialDirectory = LVApp.Instance().excute_path + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name;
                openPanel.FileName = "Total*";
                openPanel.Filter = "All csv files|*.csv";
                if (openPanel.ShowDialog() == DialogResult.OK)
                {
                    t_file = openPanel.FileName;
                }
            }
            else
            {
                OpenFileDialog openPanel = new OpenFileDialog();
                openPanel.InitialDirectory = LVApp.Instance().m_Config.m_Log_Save_Folder + "\\Data\\" + LVApp.Instance().m_Config.m_Model_Name;
                openPanel.FileName = "Total*";
                openPanel.Filter = "All csv files|*.csv";
                if (openPanel.ShowDialog() == DialogResult.OK)
                {
                    t_file = openPanel.FileName;
                }
            }

            if (t_file.Length > 5)
            {
                this.progressBar1.Value = 0;
                worker.RunWorkerAsync();
            }
        }

        private void button_SAVE_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
            {
                return;
            }
            string t_new_file = t_file.Substring(0, t_file.Length - 9) + "Report.csv";
            ds_CPCPK.Tables[0].WriteToCsvFile(t_new_file);


            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = LVApp.Instance().excute_path;
            SaveFileDialog1.RestoreDirectory = true;
            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
            {//한국어
                SaveFileDialog1.Title = "Report 저장";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
            {//영어
                SaveFileDialog1.Title = "Report save";
            }
            else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
            {//중국어
                SaveFileDialog1.Title = "报告保存";
            }

            SaveFileDialog1.Filter = "All csv files|*.csv";
            SaveFileDialog1.FilterIndex = 2;
            SaveFileDialog1.FileName = "Report.csv";
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (SaveFileDialog1.FileName.Substring(Math.Max(0, SaveFileDialog1.FileName.Length - 3)).ToUpper() == "CSV")
                {
                    ds_CPCPK.Tables[0].WriteToCsvFile(SaveFileDialog1.FileName);
                }
            }
        }
    }

    public static class MyListExtensions
    {
        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static double StandardDeviation(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<double> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }
    }
}
