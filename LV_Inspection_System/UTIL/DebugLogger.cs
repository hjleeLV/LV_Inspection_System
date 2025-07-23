using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.IO;
using Python.Runtime;

namespace LV_Inspection_System
{
    public class DebugLogger : Logger
    {
        private static DebugLogger _instance = null;            //Make this class a singleton
        //private List<string> m_logs = new List<string>(); // keep the log around for a while
        private string Log_file_Name = "";
        public DebugLogger()
        {
            m_loggertype = eLoggerType.eDebugLogger;
            DateTime CurTime = DateTime.Now;
            DirectoryInfo dir = new DirectoryInfo($"{LVApp.Instance().excute_path}\\Logs\\{CurTime:yyyy}\\{CurTime:yyyy-MM}");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            Log_file_Name = CreateTimeStampFileName("");
            SetLogFile($"{LVApp.Instance().excute_path}\\Logs\\{CurTime:yyyy}\\{CurTime:yyyy-MM}\\{Log_file_Name}");

            logQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();
            flag_LogThread = true;
            logThread = new System.Threading.Thread(WriteLog);
            logThread.IsBackground = true;
            logThread.Start();
        }

        //public List<string> GetLog()
        //{
        //    return m_logs;
        //}

        /// <summary>
        /// Destructor:: Clear out the instance
        /// </summary>
        ~DebugLogger()
        {
            flag_LogThread = false;
            logThread.Join(100);
            if (logThread.IsAlive)
            {
                logThread.Interrupt();
            }
            while (logQueue.Count > 0)
            {
                logQueue.TryDequeue(out string str);
            }
            CloseLogFile();
            _instance = null;
        }

        /// <summary>
        /// Gets the instance to the File Logger, creates if necessary
        /// </summary>
        /// <returns>Instance of this object</returns>
        public static DebugLogger Instance()
        {
            if (_instance == null)
                _instance = new DebugLogger();
            return _instance;
        }

        /// <summary>
        /// Outputs a record to the log File
        /// </summary>
        /// <param name="OutStr">String to write</param>
        public override void LogRecord(string OutStr)
        {
            string MsgOut;
            try
            {
                DateTime CurTime = DateTime.Now;

                if (Log_file_Name != CreateTimeStampFileName(""))
                {
                    Log_file_Name = CreateTimeStampFileName("");
                    SetLogFile($"{LVApp.Instance().excute_path}\\Logs\\{CurTime:yyyy}\\{CurTime:yyyy-MM}\\{Log_file_Name}");
                }

                MsgOut = $"{CurTime:HH:mm:ss.fff}> {OutStr}";
                logQueue.Enqueue(MsgOut);
                //base.LogRecord(MsgOut);
                //m_logs.Add(MsgOut);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }
        public void LogInfo(string message)
        {
            LogRecord("Info:" + message);
        }
        public void LogWarning(string message)
        {
            LogRecord("Warn:" + message);
        }

        public void LogError(string message)
        {
            LogRecord("Err :" + message);
        }
        public void LogError(Exception ex)
        {
            //LVApp.Instance().m_mainform.ctr_Camera_Setting0.toolStripButtonDisconnect_Click(null, null);
            //LVApp.Instance().m_mainform.ctr_Camera_Setting1.toolStripButtonDisconnect_Click(null, null);
            //LVApp.Instance().m_mainform.ctr_Camera_Setting2.toolStripButtonDisconnect_Click(null, null);
            //LVApp.Instance().m_mainform.ctr_Camera_Setting3.toolStripButtonDisconnect_Click(null, null);

            LogRecord("Err :" + ex.Message);
            LogRecord("Err :" + ex.StackTrace);
        }

        #region 250228 LHJ - Log 개선 : 동일한 로그가 여러 줄에 쓰여지거나, 로그 일부가 쓰여지지 않는 현상 보완
        private System.Collections.Concurrent.ConcurrentQueue<string> logQueue;
        System.Threading.Thread logThread;
        bool flag_LogThread = false;
        private void WriteLog()
        {
            while (flag_LogThread)
            {
                if (logQueue.TryDequeue(out string msg))
                {
                    base.LogRecord(msg);
                }
                System.Threading.Thread.Sleep(5);
            }
        }
        #endregion
    }
}
