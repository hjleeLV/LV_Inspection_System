using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using AsyncClientServerLib.Server;
using AsyncClientServerLib.Client;
using AsyncClientServerLib.Message;
using SocketServerLib.SocketHandler;
using SocketServerLib.Server;
using System.Net.Sockets;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Mysql : UserControl
    {
        public MySqlConnection conn = null;
        private MySqlCommand cmd = null;
        public bool m_Connected_flag = false;
        public bool m_Server_Connected_flag = false;
        private int m_Server_port = 8100;

        public string m_Client_IP = "0.0.0.0";
        public int m_Client_port = 8100;
        public bool m_Client_Connected_flag = false;

        private BasicSocketServer server = null;
        private Guid serverGuid = Guid.Empty;
        private BasicSocketClient client = null;
        private Guid clientGuid = Guid.Empty;
        protected string m_SeverAct = "";

        public Ctr_Mysql()
        {
            InitializeComponent();
        }

        public string SeverAct
        {
            get { return m_SeverAct; }
            set
            {
                m_SeverAct = value;
            }
        }
        #region TCP/IP SERVER
        /// <summary>
        /// TCP/IP SERVER
        /// </summary>
        public void Sever_connect()
        {
            if (m_Server_Connected_flag)
            {
                return;
            }
            m_Server_Connected_flag = true;
            this.serverGuid = Guid.NewGuid();
            this.server = new BasicSocketServer();
            this.server.ReceiveMessageEvent += new SocketServerLib.SocketHandler.ReceiveMessageDelegate(server_ReceiveMessageEvent);
            this.server.ConnectionEvent += new SocketConnectionDelegate(server_ConnectionEvent);
            this.server.CloseConnectionEvent += new SocketConnectionDelegate(server_CloseConnectionEvent);
            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostAddresses(Dns.GetHostName())[0]);// Dns.GetHostByName(Dns.GetHostName());
            String myip = IPHost.AddressList[0].ToString();

            for (int i = 0; i < IPHost.AddressList.Length; i++)
            {
                if (IPHost.AddressList[i].AddressFamily == AddressFamily.InterNetwork &&
            !IPAddress.IsLoopback(IPHost.AddressList[i]) &&  // ignore loopback addresses
            !IPHost.AddressList[i].ToString().StartsWith("169.254."))
                {
                    myip = IPHost.AddressList[i].ToString();
                }
            }

            this.server.Init(new IPEndPoint(IPAddress.Parse(myip), m_Server_port));
            this.server.StartUp();
            LVApp.Instance().m_mainform.add_Log("IP[ " + myip + " ], Port[ " + m_Server_port.ToString()+" ]");
            LVApp.Instance().m_Config.ds_STATUS.Tables[2].Rows[1][1] = myip;
            LVApp.Instance().m_Config.ds_STATUS.Tables[2].Rows[2][1] = m_Server_port.ToString();
            LVApp.Instance().m_Ctr_Mysql.DB_Operating(LVApp.Instance().m_Config.ds_STATUS.Tables[2], "Information");
            timer_SeverAct.Start();
        }

        public void Sever_disconnect()
        {
            if (m_Server_Connected_flag)
            {
                this.server.Shutdown();
                this.server.Dispose();
                this.server = null;
                m_Server_Connected_flag = false;
                timer_SeverAct.Stop();
            }
        }

        void server_CloseConnectionEvent(AbstractTcpSocketClientHandler handler)
        {
            LVApp.Instance().m_mainform.add_Log("A client is disconnected.");
        }

        void server_ConnectionEvent(AbstractTcpSocketClientHandler handler)
        {
            LVApp.Instance().m_mainform.add_Log("A client is connected.");
        }

        void server_ReceiveMessageEvent(SocketServerLib.SocketHandler.AbstractTcpSocketClientHandler handler, SocketServerLib.Message.AbstractMessage message)
        {
            if (!m_Server_Connected_flag)
            {
                return;
            }
            BasicMessage receivedMessage = (BasicMessage)message;
            byte[] buffer = receivedMessage.GetBuffer();
            if (buffer.Length > 1000)
            {
                //MessageBox.Show(string.Format("Received a long message of {0} bytes", receivedMessage.MessageLength), "Socket Server",
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string s = System.Text.ASCIIEncoding.Unicode.GetString(buffer);
            SeverAct = s;
            LVApp.Instance().m_mainform.add_Log(s);
            server_SendMessage("OK");
        }

        void server_SendMessage(string str)
        {
            if (!m_Server_Connected_flag)
            {
                return;
            }
            ClientInfo[] clientList = this.server.GetClientList();
            if (clientList.Length == 0)
            {
                //MessageBox.Show("The client is not connected", "Socket Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AbstractTcpSocketClientHandler clientHandler = clientList[0].TcpSocketClientHandler;
            byte[] buffer = System.Text.ASCIIEncoding.Unicode.GetBytes(str);
            BasicMessage message = new BasicMessage(this.serverGuid, buffer);
            clientHandler.SendAsync(message);
        }
        #endregion

        #region TCP/IP CLIENT
        /// <summary>
        /// TCPIP CLIENT
        /// </summary>

        public void Client_connect()
        {
            if (m_Client_Connected_flag)
            {
                return;
            }
            try
            {
                bool isServerConnected;
                this.clientGuid = Guid.NewGuid();
                this.client = new BasicSocketClient();
                this.client.ReceiveMessageEvent += new SocketServerLib.SocketHandler.ReceiveMessageDelegate(client_ReceiveMessageEvent);
                //this.client.ConnectionEvent += new SocketServerLib.SocketHandler.SocketConnectionDelegate(client_ConnectionEvent);
                //this.client.CloseConnectionEvent += new SocketServerLib.SocketHandler.SocketConnectionDelegate(client_CloseConnectionEvent);

                this.client.Connect(new IPEndPoint(IPAddress.Parse(m_Client_IP), m_Client_port), out isServerConnected);

                m_Client_Connected_flag = true;
            }
            catch (Exception ex)
            {
                m_Client_Connected_flag = false;
                LVApp.Instance().m_mainform.add_Log(string.Format("Client failed to connect remote server.\n{0}", ex.Message));
            }
        }

        private void Client_disconnect(object sender, EventArgs e)
        {
            if (m_Client_Connected_flag)
            {
                this.client.Close();
                this.client.Dispose();
                this.client = null;
            } 
        }

        void client_ReceiveMessageEvent(SocketServerLib.SocketHandler.AbstractTcpSocketClientHandler handler, SocketServerLib.Message.AbstractMessage message)
        {
            if (!m_Client_Connected_flag)
            {
                return;
            }
            BasicMessage receivedMessage = (BasicMessage)message;
            byte[] buffer = receivedMessage.GetBuffer();
            string s = System.Text.ASCIIEncoding.Unicode.GetString(buffer);
            LVApp.Instance().m_mainform.add_Log(s);
            server_SendMessage("OK");
        }

        void Client_SendMessage(string str)
        {
            if (!m_Client_Connected_flag)
            {
                return;
            }
            byte[] buffer = System.Text.ASCIIEncoding.Unicode.GetBytes(str);
            BasicMessage message = new BasicMessage(this.clientGuid, buffer);
            this.client.SendAsync(message);
        }
        #endregion

        #region MySQL
        /// <summary>
        /// MySQL
        /// </summary>
        public void DB_connect()
        {
            //Sever_connect();
            if (m_Connected_flag)
            {
                return;
            }
            if (conn != null)
            {
                conn.Close();
            }

            string connStr = String.Format("server={0};user id={1}; password={2}; database=mysql; pooling=false",
                "localhost", "root", "1234567890");

            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                m_Connected_flag = true;
            }
            catch
            {
                m_Connected_flag = false;
                //MessageBox.Show("Error connecting to the server: " + ex.Message);
            }
        }

        public void DB_disconnect()
        {
            if (conn != null || m_Connected_flag)
            {
                Sever_disconnect();
                conn.Close(); conn = null;
                m_Connected_flag = false;
            }
        }

        public void DB_Create()
        {
            if (conn == null || !m_Connected_flag || LVApp.Instance().m_Config.m_Model_Name == "")
            {
                return;
            }

            string cmsStr = "CREATE DATABASE IF NOT EXISTS `"+LVApp.Instance().m_Config.m_Model_Name+"`;";
            cmd = new MySqlCommand(cmsStr, conn);
            cmd.ExecuteNonQuery();
            cmsStr = "CREATE DATABASE IF NOT EXISTS `" + "Operating" + "`;";
            cmd = new MySqlCommand(cmsStr, conn);
            cmd.ExecuteNonQuery();
            conn.ChangeDatabase(LVApp.Instance().m_Config.m_Model_Name);
        }

        public void DB_Drop()
        {
            if (conn == null || !m_Connected_flag || LVApp.Instance().m_Config.m_Model_Name == "")
            {
                return;
            }

            string cmsStr = "DROP DATABASE IF EXISTS `" + LVApp.Instance().m_Config.m_Model_Name + "`;";
            cmd = new MySqlCommand(cmsStr, conn);
            cmd.ExecuteNonQuery();
        }

        public void DB_Table2MySQL(DataTable table, String table_name)
        {
            try
            {
                if (conn == null || !m_Connected_flag)
                {
                    return;
                }

                conn.ChangeDatabase(LVApp.Instance().m_Config.m_Model_Name);

                StringBuilder queryBuilder = new StringBuilder();
                DateTime dt;

                // more than 1 column required and 1 or more rows
                if (table.Columns.Count > 1 && table.Rows.Count > 0)
                {
                    queryBuilder.AppendFormat("DROP TABLE IF EXISTS `" + LVApp.Instance().m_Config.m_Model_Name + "`.`" + table_name + "`; ");

                    queryBuilder.AppendFormat("CREATE TABLE `" + LVApp.Instance().m_Config.m_Model_Name + "`.`" + table_name + "` ( ");

                    if (table.Columns.Count > 1)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            if (i == table.Columns.Count - 1)
                            {
                                queryBuilder.AppendFormat("`{0}` VARCHAR(256) NOT NULL) ENGINE = InnoDB;", table.Columns[i].ColumnName);
                            }
                            else
                            {
                                queryBuilder.AppendFormat("`{0}` VARCHAR(256) NOT NULL , ", table.Columns[i].ColumnName);
                            }
                        }
                    }
                    queryBuilder.AppendFormat(" INSERT INTO `{0}` (", table_name);
                    // build all columns
                    queryBuilder.AppendFormat("`{0}`", table.Columns[0].ColumnName);

                    if (table.Columns.Count > 1)
                    {
                        for (int i = 1; i < table.Columns.Count; i++)
                        {
                            queryBuilder.AppendFormat(", `{0}` ", table.Columns[i].ColumnName);
                        }
                    }

                    queryBuilder.AppendFormat(") VALUES (", table_name);

                    // build all values for the first row
                    // escape String & Datetime values!
                    if (table.Columns[0].DataType == typeof(String))
                    {
                        queryBuilder.AppendFormat("'{0}'", MySqlHelper.EscapeString(table.Rows[0][table.Columns[0].ColumnName].ToString()));
                    }
                    else if (table.Columns[0].DataType == typeof(DateTime))
                    {
                        dt = (DateTime)table.Rows[0][table.Columns[0].ColumnName];
                        queryBuilder.AppendFormat("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else if (table.Columns[0].DataType == typeof(Int32))
                    {
                        queryBuilder.AppendFormat("{0}", table.Rows[0].Field<Int32?>(table.Columns[0].ColumnName) ?? 0);
                    }
                    else
                    {
                        if (table.Rows[0][table.Columns[0].ColumnName].ToString().ToUpper() == "TRUE")
                        {
                            queryBuilder.AppendFormat("1");
                        }
                        else if (table.Rows[0][table.Columns[0].ColumnName].ToString().ToUpper() == "FALSE")
                        {
                            queryBuilder.AppendFormat("0");
                        }
                        else
                        {
                            queryBuilder.AppendFormat(", {0}", table.Rows[0][table.Columns[0].ColumnName].ToString());
                        }
                    }

                    for (int i = 1; i < table.Columns.Count; i++)
                    {
                        // escape String & Datetime values!
                        if (table.Columns[i].DataType == typeof(String))
                        {
                            queryBuilder.AppendFormat(", '{0}'", MySqlHelper.EscapeString(table.Rows[0][table.Columns[i].ColumnName].ToString()));
                        }
                        else if (table.Columns[i].DataType == typeof(DateTime))
                        {
                            dt = (DateTime)table.Rows[0][table.Columns[i].ColumnName];
                            queryBuilder.AppendFormat(", '{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));

                        }
                        else if (table.Columns[i].DataType == typeof(Int32))
                        {
                            queryBuilder.AppendFormat(", {0}", table.Rows[0].Field<Int32?>(table.Columns[i].ColumnName) ?? 0);
                        }
                        else
                        {
                            if (table.Rows[0][table.Columns[i].ColumnName].ToString().ToUpper() == "TRUE")
                            {
                                queryBuilder.AppendFormat(", 1");
                            }
                            else if (table.Rows[0][table.Columns[i].ColumnName].ToString().ToUpper() == "FALSE")
                            {
                                queryBuilder.AppendFormat(", 0");
                            }
                            else
                            {
                                queryBuilder.AppendFormat(", {0}", table.Rows[0][table.Columns[i].ColumnName].ToString());
                            }
                        }
                    }

                    queryBuilder.Append(")");
                    queryBuilder.AppendLine();

                    // build all values all remaining rows
                    if (table.Rows.Count > 1)
                    {
                        // iterate over the rows
                        for (int row = 1; row < table.Rows.Count; row++)
                        {
                            // open value block
                            queryBuilder.Append(", (");

                            // escape String & Datetime values!
                            if (table.Columns[0].DataType == typeof(String))
                            {
                                queryBuilder.AppendFormat("'{0}'", MySqlHelper.EscapeString(table.Rows[row][table.Columns[0].ColumnName].ToString()));
                            }
                            else if (table.Columns[0].DataType == typeof(DateTime))
                            {
                                dt = (DateTime)table.Rows[row][table.Columns[0].ColumnName];
                                queryBuilder.AppendFormat("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else if (table.Columns[0].DataType == typeof(Int32))
                            {
                                queryBuilder.AppendFormat("{0}", table.Rows[row].Field<Int32?>(table.Columns[0].ColumnName) ?? 0);
                            }
                            else
                            {
                                if (table.Rows[row][table.Columns[0].ColumnName].ToString().ToUpper() == "TRUE")
                                {
                                    queryBuilder.AppendFormat("1");
                                }
                                else if (table.Rows[row][table.Columns[0].ColumnName].ToString().ToUpper() == "FALSE")
                                {
                                    queryBuilder.AppendFormat("0");
                                }
                                else
                                {
                                    queryBuilder.AppendFormat(", {0}", table.Rows[row][table.Columns[0].ColumnName].ToString());
                                }
                            }

                            for (int col = 1; col < table.Columns.Count; col++)
                            {
                                // escape String & Datetime values!
                                if (table.Columns[col].DataType == typeof(String))
                                {
                                    queryBuilder.AppendFormat(", '{0}'", MySqlHelper.EscapeString(table.Rows[row][table.Columns[col].ColumnName].ToString()));
                                }
                                else if (table.Columns[col].DataType == typeof(DateTime))
                                {
                                    dt = (DateTime)table.Rows[row][table.Columns[col].ColumnName];
                                    queryBuilder.AppendFormat(", '{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else if (table.Columns[col].DataType == typeof(Int32))
                                {
                                    queryBuilder.AppendFormat(", {0}", table.Rows[row].Field<Int32?>(table.Columns[col].ColumnName) ?? 0);
                                }
                                else
                                {
                                    queryBuilder.AppendFormat(", {0}", table.Rows[row][table.Columns[col].ColumnName].ToString());
                                }
                            } // end for (int i = 1; i < table.Columns.Count; i++)

                            // close value block
                            queryBuilder.Append(")");
                            queryBuilder.AppendLine();

                        } // end for (int r = 1; r < table.Rows.Count; r++)

                        // sql delimiter =)
                        queryBuilder.Append(";");

                    } // end if (table.Rows.Count > 1)

                    cmd = new MySqlCommand(queryBuilder.ToString(), conn);
                    cmd.ExecuteNonQuery();

                }
            }
            catch// (Exception ex)
            {
            }
        }

        public void DB_Operating(DataTable table, String table_name)
        {
            try
            {
                if (conn == null || !m_Connected_flag)
                {
                    return;
                }

                conn.ChangeDatabase("Operating");

                StringBuilder queryBuilder = new StringBuilder();
                DateTime dt;

                // more than 1 column required and 1 or more rows
                if (table.Columns.Count > 1 && table.Rows.Count > 0)
                {
                    queryBuilder.AppendFormat("DROP TABLE IF EXISTS `" + table_name + "`; ");

                    queryBuilder.AppendFormat("CREATE TABLE `" + table_name + "` ( ");

                    if (table.Columns.Count > 1)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            if (i == table.Columns.Count - 1)
                            {
                                queryBuilder.AppendFormat("`{0}` VARCHAR(256) NOT NULL) ENGINE = InnoDB;", table.Columns[i].ColumnName);
                            }
                            else
                            {
                                queryBuilder.AppendFormat("`{0}` VARCHAR(256) NOT NULL , ", table.Columns[i].ColumnName);
                            }
                        }
                    }
                    queryBuilder.AppendFormat(" INSERT INTO `{0}` (", table_name);
                    // build all columns
                    queryBuilder.AppendFormat("`{0}`", table.Columns[0].ColumnName);

                    if (table.Columns.Count > 1)
                    {
                        for (int i = 1; i < table.Columns.Count; i++)
                        {
                            queryBuilder.AppendFormat(", `{0}` ", table.Columns[i].ColumnName);
                        }
                    }

                    queryBuilder.AppendFormat(") VALUES (", table_name);

                    // build all values for the first row
                    // escape String & Datetime values!
                    if (table.Columns[0].DataType == typeof(String))
                    {
                        queryBuilder.AppendFormat("'{0}'", MySqlHelper.EscapeString(table.Rows[0][table.Columns[0].ColumnName].ToString()));
                    }
                    else if (table.Columns[0].DataType == typeof(DateTime))
                    {
                        dt = (DateTime)table.Rows[0][table.Columns[0].ColumnName];
                        queryBuilder.AppendFormat("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else if (table.Columns[0].DataType == typeof(Int32))
                    {
                        queryBuilder.AppendFormat("{0}", table.Rows[0].Field<Int32?>(table.Columns[0].ColumnName) ?? 0);
                    }
                    else
                    {
                        if (table.Rows[0][table.Columns[0].ColumnName].ToString().ToUpper() == "TRUE")
                        {
                            queryBuilder.AppendFormat("1");
                        }
                        else if (table.Rows[0][table.Columns[0].ColumnName].ToString().ToUpper() == "FALSE")
                        {
                            queryBuilder.AppendFormat("0");
                        }
                        else
                        {
                            queryBuilder.AppendFormat(", {0}", table.Rows[0][table.Columns[0].ColumnName].ToString());
                        }
                    }

                    for (int i = 1; i < table.Columns.Count; i++)
                    {
                        // escape String & Datetime values!
                        if (table.Columns[i].DataType == typeof(String))
                        {
                            queryBuilder.AppendFormat(", '{0}'", MySqlHelper.EscapeString(table.Rows[0][table.Columns[i].ColumnName].ToString()));
                        }
                        else if (table.Columns[i].DataType == typeof(DateTime))
                        {
                            dt = (DateTime)table.Rows[0][table.Columns[i].ColumnName];
                            queryBuilder.AppendFormat(", '{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));

                        }
                        else if (table.Columns[i].DataType == typeof(Int32))
                        {
                            queryBuilder.AppendFormat(", {0}", table.Rows[0].Field<Int32?>(table.Columns[i].ColumnName) ?? 0);
                        }
                        else
                        {
                            if (table.Rows[0][table.Columns[i].ColumnName].ToString().ToUpper() == "TRUE")
                            {
                                queryBuilder.AppendFormat(", 1");
                            }
                            else if (table.Rows[0][table.Columns[i].ColumnName].ToString().ToUpper() == "FALSE")
                            {
                                queryBuilder.AppendFormat(", 0");
                            }
                            else
                            {
                                queryBuilder.AppendFormat(", {0}", table.Rows[0][table.Columns[i].ColumnName].ToString());
                            }
                        }
                    }

                    queryBuilder.Append(")");
                    queryBuilder.AppendLine();

                    // build all values all remaining rows
                    if (table.Rows.Count > 1)
                    {
                        // iterate over the rows
                        for (int row = 1; row < table.Rows.Count; row++)
                        {
                            // open value block
                            queryBuilder.Append(", (");

                            // escape String & Datetime values!
                            if (table.Columns[0].DataType == typeof(String))
                            {
                                queryBuilder.AppendFormat("'{0}'", MySqlHelper.EscapeString(table.Rows[row][table.Columns[0].ColumnName].ToString()));
                            }
                            else if (table.Columns[0].DataType == typeof(DateTime))
                            {
                                dt = (DateTime)table.Rows[row][table.Columns[0].ColumnName];
                                queryBuilder.AppendFormat("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else if (table.Columns[0].DataType == typeof(Int32))
                            {
                                queryBuilder.AppendFormat("{0}", table.Rows[row].Field<Int32?>(table.Columns[0].ColumnName) ?? 0);
                            }
                            else
                            {
                                if (table.Rows[row][table.Columns[0].ColumnName].ToString().ToUpper() == "TRUE")
                                {
                                    queryBuilder.AppendFormat("1");
                                }
                                else if (table.Rows[row][table.Columns[0].ColumnName].ToString().ToUpper() == "FALSE")
                                {
                                    queryBuilder.AppendFormat("0");
                                }
                                else
                                {
                                    queryBuilder.AppendFormat(", {0}", table.Rows[row][table.Columns[0].ColumnName].ToString());
                                }
                            }

                            for (int col = 1; col < table.Columns.Count; col++)
                            {
                                // escape String & Datetime values!
                                if (table.Columns[col].DataType == typeof(String))
                                {
                                    queryBuilder.AppendFormat(", '{0}'", MySqlHelper.EscapeString(table.Rows[row][table.Columns[col].ColumnName].ToString()));
                                }
                                else if (table.Columns[col].DataType == typeof(DateTime))
                                {
                                    dt = (DateTime)table.Rows[row][table.Columns[col].ColumnName];
                                    queryBuilder.AppendFormat(", '{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else if (table.Columns[col].DataType == typeof(Int32))
                                {
                                    queryBuilder.AppendFormat(", {0}", table.Rows[row].Field<Int32?>(table.Columns[col].ColumnName) ?? 0);
                                }
                                else
                                {
                                    queryBuilder.AppendFormat(", {0}", table.Rows[row][table.Columns[col].ColumnName].ToString());
                                }
                            } // end for (int i = 1; i < table.Columns.Count; i++)

                            // close value block
                            queryBuilder.Append(")");
                            queryBuilder.AppendLine();

                        } // end for (int r = 1; r < table.Rows.Count; r++)

                        // sql delimiter =)
                        queryBuilder.Append(";");

                    } // end if (table.Rows.Count > 1)

                    cmd = new MySqlCommand(queryBuilder.ToString(), conn);
                    cmd.ExecuteNonQuery();

                }
            }
            catch// (Exception ex)
            {
            }
        }
        #endregion

        private void timer_SeverAct_Tick(object sender, EventArgs e)
        {
            if (SeverAct.ToUpper() == "START")
            {
                SeverAct = "";
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_mainform.button_INSPECTION_Click(null, null);
                }
            }
            else if (SeverAct.ToUpper() == "STOP")
            {
                SeverAct = "";
                if (LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    LVApp.Instance().m_Config.m_Check_Server_Operation = true;
                    LVApp.Instance().m_mainform.button_INSPECTION_Click(null, null);
                    LVApp.Instance().m_Config.m_Check_Server_Operation = false;
                }
            }
        }
    }
}
