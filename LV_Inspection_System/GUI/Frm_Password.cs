using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LV_Inspection_System.GUI
{
    public partial class Frm_Password : Form
    {
        public Frm_Password()
        {
            InitializeComponent();

            this.Height = 100;
            m_SetLanguage = LVApp.Instance().m_Config.m_SetLanguage;
        }

        protected int m_Language = 0; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    this.Text = "관리자 모드";
                    label1.Text = "비밀번호 변경하기";
                    label2.Text = "기존 비밀번호";
                    label3.Text = "신규 비밀번호";
                    label4.Text = "신규 비밀번호 확인";
                    button2.Text = "변경하기";
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    this.Text = "Admin Mode";
                    label1.Text = "Change password";
                    label2.Text = "Current password";
                    label3.Text = "New password";
                    label4.Text = "Confirm password";
                    button2.Text = "Change";
                }
                else if (value == 2 && m_Language != value)
                {// 중국어
                    this.Text = "管理模式";
                    label1.Text = "改变 password";
                    label2.Text = "当前 password";
                    label3.Text = "新增 password";
                    label4.Text = "确认 password";
                    button2.Text = "更改";
                }
                m_Language = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "lv1234567890")
            {
                Properties.Settings.Default.PASSWD = "0000";
                Properties.Settings.Default.Save();
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    label5.Text = "비밀번호 초기화 완료!";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    label5.Text = "Password initialized!";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//영어
                    label5.Text = "Password 初始化!";
                }

                textBox1.Text = "0000";
                return;
            }
            if (textBox1.Text == Properties.Settings.Default.PASSWD)
            {
                LVApp.Instance().m_Config.m_Administrator_Password_Flag = true;
                LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = false;
                this.Close();
            }
            else if (textBox1.Text == "7748" || textBox1.Text == "9542")
            {
                LVApp.Instance().m_Config.m_Administrator_Password_Flag = true;
                LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = true;
                //Properties.Settings.Default.PASSWD = "0000";
                //Properties.Settings.Default.Save();
                this.Close();
            }
            else
            {
                LVApp.Instance().m_Config.m_Administrator_Password_Flag = false;
                LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = false;
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    AutoClosingMessageBox.Show("패스워드가 틀렸습니다. 다시 입력해주세요.", "Error", 750);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    AutoClosingMessageBox.Show("Wrong! Try again.", "Error", 750);
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//영어
                    AutoClosingMessageBox.Show("错！再试一次.", "Error", 750);
                }
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "lv1234567890")
                {
                    Properties.Settings.Default.PASSWD = "0000";
                    Properties.Settings.Default.Save();
                    textBox1.Text = "0000";
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        label5.Text = "비밀번호 초기화 완료!";
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        label5.Text = "Password initialized!";
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//영어
                        label5.Text = "Password 初始化!";
                    }
                    return;
                }
                if (textBox1.Text == Properties.Settings.Default.PASSWD)
                {
                    LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = false;
                    LVApp.Instance().m_Config.m_Administrator_Password_Flag = true;
                    this.Close();
                }
                else if (textBox1.Text == "7748" || textBox1.Text == "9542")
                {
                    LVApp.Instance().m_Config.m_Administrator_Password_Flag = true;
                    LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = true;
                    //Properties.Settings.Default.PASSWD = "0000";
                    //Properties.Settings.Default.Save();
                    this.Close();
                }
                else
                {
                    LVApp.Instance().m_Config.m_Administrator_Password_Flag = false;
                    LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = false;
                    if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                    {//한국어
                        AutoClosingMessageBox.Show("패스워드가 틀렸습니다. 다시 입력해주세요.", "Error", 750);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                    {//영어
                        AutoClosingMessageBox.Show("Wrong! Try again.", "Error", 750);
                    }
                    else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                    {//영어
                        AutoClosingMessageBox.Show("错！再试一次.", "Error", 750);
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Height = 251;
            if (Properties.Settings.Default.PASSWD == "0000")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    label5.Text = "초기 비밀번호 '0000'";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    label5.Text = "Initial password '0000'";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//영어
                    label5.Text = "早期 password '0000'";
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    label5.Text = "빈칸을 채워주세요.";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    label5.Text = "Type the password.";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    label5.Text = "输入 password";
                }
                return;
            }
            if (textBox2.Text != Properties.Settings.Default.PASSWD)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    label5.Text = "기존 비밀번호 틀림!";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    label5.Text = "Misstyped the password.";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    label5.Text = "输入错误 password.";
                }

                return;
            }
            if (textBox3.Text != textBox4.Text)
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    label5.Text = "비밀번호 변경 완료!";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    label5.Text = "Password changed.";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    label5.Text = "更改完成 password.";
                }
                return;
            }
            Properties.Settings.Default.PASSWD = textBox3.Text;
            Properties.Settings.Default.Save();
        }

        private void Form_Password_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.PASSWD == "0000")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    label5.Text = "초기 비밀번호 '0000'";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    label5.Text = "Initial password '0000'";
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//영어
                    label5.Text = "早期 password '0000'";
                }
            }
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            label1.ForeColor = Color.OrangeRed;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Black;
        }
    }
}

public class AutoClosingMessageBox
{
    static public bool _chekc_Open_Notice = false;
    System.Threading.Timer _timeoutTimer;
    string _caption;
    AutoClosingMessageBox(string text, string caption, int timeout)
    {
        _caption = caption;


        _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
            null, timeout, 2000);

        MessageBox.Show(text, caption,MessageBoxButtons.OK);

        //IntPtr mbWnd = FindWindow(null, caption);
        //Form.ClickOnPoint(handle, new Point(100, 100));
    }
    public static void Show(string text, string caption, int timeout)
    {
        if (_chekc_Open_Notice)
        {
            return;
        }
        _chekc_Open_Notice = true;
        new AutoClosingMessageBox(text, caption, timeout);
    }
    void OnTimerElapsed(object state)
    {
        IntPtr mbWnd = FindWindow(null, _caption);
        if (mbWnd != IntPtr.Zero)
            SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        _timeoutTimer.Dispose();
        _chekc_Open_Notice = false;
    }
    const int WM_CLOSE = 0x0010;
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
}
