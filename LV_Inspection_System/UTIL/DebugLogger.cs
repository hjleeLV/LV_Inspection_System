using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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

            _cts_Log = new CancellationTokenSource();
            logQueue = new System.Collections.Concurrent.BlockingCollection<string>(new System.Collections.Concurrent.ConcurrentQueue<string>()/*최대 용량 지정 할 수 있음*/);            // 데이터를 더 추가하지 않으려면 CompleteAdding을 호출하면 됨.
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
            logQueue.CompleteAdding();
            //_cts_Log.Cancel();    // 즉시 종료하려면 주석 해제
            logThread.Join(100);
            if (logThread.IsAlive)
            {
                logThread.Interrupt();
            }
            while (logQueue.Count > 0)
            {
                logQueue.GetConsumingEnumerable();
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
                logQueue.Add(MsgOut);

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
        // ConcurrentQueue -> BlockingCollection 으로 변경
        //private System.Collections.Concurrent.ConcurrentQueue<string> logQueue;
        private System.Collections.Concurrent.BlockingCollection<string> logQueue;

        System.Threading.Thread logThread;
        private CancellationTokenSource _cts_Log;   // (참고)_logMessage.CompleteAdding()으로도 _logProcess를 종료 시킬 수는 있지만, 남은 메시지(로그)가 있다면 다 처리하고 종료 됨. _cts_Log는 즉시 종료 시킬 수 있음
        //bool flag_LogThread = false;
        private void WriteLog()
        {
            foreach (string log in logQueue.GetConsumingEnumerable(_cts_Log.Token))
            {
                base.LogRecord(log);
            }
        }
        #endregion
    }
}
