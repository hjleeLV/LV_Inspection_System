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
    public partial class Ctr_Popup : UserControl
    {
        public string t_Item = "";
        public Ctr_Popup()
        {
            InitializeComponent();
        }

        public void display_update()
        {
            //richTextBox1.Text = "HELP : ";
            richTextBox1.Text += t_Item;// +"\r\n";
            richTextBox1.Refresh();
            pictureBox1.Refresh();
        }
    }
}
