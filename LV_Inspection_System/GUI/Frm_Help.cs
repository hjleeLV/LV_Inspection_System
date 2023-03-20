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
    public partial class Frm_Help : Form
    {
        public bool m_hide_check = true;
        public Frm_Help()
        {
            InitializeComponent();
        }

        private void Frm_Help_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!LVApp.Instance().m_mainform.Force_close)
            {
                e.Cancel = true;
                this.Hide();
                m_hide_check = true;
            }
        }
    }
}
