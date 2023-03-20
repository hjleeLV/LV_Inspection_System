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
    public partial class Ctr_Yield_Log : UserControl
    {
        protected string cam_name = "CAM0";
        public Ctr_Yield_Log()
        {
            InitializeComponent();
        }

        public string m_cam_name
        {
            get { return cam_name; }
            set
            {
                cam_name = value;
                label_CAM.Text = cam_name;
            }
        }

        public void Update_Data(int Cam_num)
        {
            if (Cam_num == 0)
            {
            }
        }

        private void Ctr_Yield_Log_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
