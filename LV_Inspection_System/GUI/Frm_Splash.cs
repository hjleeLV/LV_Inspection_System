using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace LV_Inspection_System.GUI
{
    public partial class frmSplash : Form
    {
        System.Windows.Forms.Timer m_timer;
        private int m_total = 0;
        int max = 60;
        public frmSplash()
        {
            InitializeComponent();

            m_timer = new System.Windows.Forms.Timer();
            m_timer.Interval = 100;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();
            
            Opacity = 0.0;
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_total >= max) // check for closing
                {
                    m_timer.Stop();
                    Close();

                    return;
                }
                
                if (m_total > (max - 10)) // fade out
                {
                    this.Opacity -= .1;
                }

                if (m_total < 10) // fade in 
                {
                    this.Opacity += .1;
                }
                m_total++;
                if (m_total == 1)
                {

                    //Thread.Sleep(2000);
                    //LVApp.Instance().m_mainform.neoTabWindow_MAIN.Enabled = true;
                }    
            }
            catch (Exception ex) 
            {
                m_timer.Stop();
                DebugLogger.Instance().LogError(ex.Message);
                Close();
            }
        }
    }
}
