using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThridLibray;
using Matrox.MatroxImagingLibrary;
using OpenCvSharp;

namespace LV_Inspection_System.GUI.Control
{
    public partial class GeniCam_SliderUserControl : UserControl
    {
        private string name = "ValueName"; /* The name of the node. */
        public ThridLibray.IDevice Dev = null;
        public int Cam_Num = 0;
        public GeniCam_SliderUserControl()
        {
            InitializeComponent();
        }

        public string NodeName
        {
            get { return name; }
            set { name = value; labelName.Text = name + ":"; UpdateValues(); }
        }

        public void UpdateValues()
        {
            try
            {
                if (Dev == null)
                {
                    return;
                }
                int min = 0;
                int max = 0;
                int val = 0;
                int inc = 1;
                bool Enabled = true;
                if (Dev.DeviceInfo.Vendor.ToUpper() == "BASLER")
                {
                    if (name == "ExposureTime")
                    {
                        using (IIntegraParameter p = Dev.ParameterCollection[new IntegerName("ExposureTimeRaw")])
                        {
                            min = checked((int)p.GetMinimum());
                            max = checked((int)p.GetMaximum());
                            //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].ExposureTime;
                            val = checked((int)p.GetValue());
                            inc = 1;
                            if (val < min)
                            {
                                val = min;
                            }
                            if (val > max)
                            {
                                val = max;
                            }
                            inc = 1;
                            if (min == max)
                            {
                                Enabled = true;
                            }
                            else
                            {
                                Enabled = p.SetValue(val);
                            }
                        }
                    }
                }
                else
                {
                    if (name == "ExposureTime")
                    {
                        using (IFloatParameter p = Dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
                        {
                            min = checked((int)p.GetMinimum());
                            max = checked((int)p.GetMaximum());
                            //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].ExposureTime;
                            val = checked((int)p.GetValue());
                            inc = 1;
                            if (val < min)
                            {
                                val = min;
                            }
                            if (val > max)
                            {
                                val = max;
                            }
                            inc = 1;
                            if (min == max)
                            {
                                Enabled = true;
                            }
                            else
                            {
                                Enabled = p.SetValue((double)val);
                            }
                        }
                    }
                }
                if (Dev.DeviceInfo.Vendor.ToUpper() == "BASLER")
                {
                    if (name == "GainRaw")
                    {
                        using (IIntegraParameter p = Dev.ParameterCollection[new IntegerName("GainRaw")])
                        {
                            min = checked((int)p.GetMinimum());
                            max = checked((int)p.GetMaximum());
                            //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].GainRaw;//checked((int)p.GetValue());
                            val = checked((int)p.GetValue());
                            inc = 1;
                            if (val < min)
                            {
                                val = min;
                            }
                            if (val > max)
                            {
                                val = max;
                            }
                            inc = 1;
                            if (min == max)
                            {
                                Enabled = true;
                            }
                            else
                            {
                                Enabled = p.SetValue(val);
                            }
                        }
                    }
                }
                else
                {
                    if (name == "GainRaw")
                    {
                        using (IFloatParameter p = Dev.ParameterCollection[ParametrizeNameSet.GainRaw])
                        {
                            min = checked((int)p.GetMinimum());
                            max = checked((int)p.GetMaximum());
                            //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].GainRaw;//checked((int)p.GetValue());
                            val = checked((int)p.GetValue());
                            inc = 1;
                            if (val < min)
                            {
                                val = min;
                            }
                            if (val > max)
                            {
                                val = max;
                            }
                            inc = 1;
                            if (min == max)
                            {
                                Enabled = true;
                            }
                            else
                            {
                                Enabled = p.SetValue((double)val);
                            }
                        }
                    }
                }
                if (name == "Width")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageWidth])
                    {
                        min = checked((int)p.GetMinimum());
                        max = checked((int)p.GetMaximum());
                        //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].Width;//checked((int)p.GetValue());
                        val = checked((int)p.GetValue());
                        inc = 1;
                        if (val < min)
                        {
                            val = min;
                        }
                        if (val > max)
                        {
                            val = max;
                        }
                        inc = 1;
                        if (min == max)
                        {
                            Enabled = true;
                        }
                        else
                        {
                            Enabled = p.SetValue(val);
                        }
                    }
                }
                if (name == "Height")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageHeight])
                    {
                        min = checked((int)p.GetMinimum());
                        max = checked((int)p.GetMaximum());
                        //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].Height;//checked((int)p.GetValue());
                        val = checked((int)p.GetValue());
                        inc = 1;
                        if (val < min)
                        {
                            val = min;
                        }
                        if (val > max)
                        {
                            val = max;
                        }
                        inc = 1;
                        if (min == max)
                        {
                            Enabled = true;
                        }
                        else
                        {
                            Enabled = p.SetValue(val);
                        }
                    }
                }
                if (name == "OffsetX")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetX])
                    {
                        min = checked((int)p.GetMinimum());
                        max = checked((int)p.GetMaximum());
                        //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].X_Offset;//checked((int)p.GetValue());
                        val = checked((int)p.GetValue());
                        if (val < min)
                        {
                            val = min;
                        }
                        if (val > max)
                        {
                            val = max;
                        }
                        inc = 1;
                        if (min == max)
                        {
                            Enabled = true;
                        }
                        else
                        {
                            Enabled = p.SetValue(val);
                        }
                    }
                }
                if (name == "OffsetY")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetY])
                    {
                        min = checked((int)p.GetMinimum());
                        max = checked((int)p.GetMaximum());
                        //val = (int)IPSSTApp.Instance().m_GenICam.CAM[Cam_Num].Y_Offset;//checked((int)p.GetValue());
                        val = checked((int)p.GetValue());
                        inc = 1;
                        if (val < min)
                        {
                            val = min;
                        }
                        if (val > max)
                        {
                            val = max;
                        }
                        inc = 1;
                        if (min == max)
                        {
                            Enabled = true;
                        }
                        else
                        {
                            Enabled = p.SetValue(val);
                        }
                    }
                }

                //if (min == 0 && max == 0)
                //{
                //    return;
                //}
                if (slider.InvokeRequired)
                {
                    slider.Invoke((MethodInvoker)delegate
                    {
                        slider.Minimum = min;
                        slider.Maximum = max;
                        slider.Value = val;
                        slider.SmallChange = inc;
                        slider.TickFrequency = (max - min + 5) / 10;

                        /* Update the values. */
                        labelMin.Text = "" + min;
                        labelMax.Text = "" + max;
                        labelCurrentValue.Text = "" + val;

                        /* Update accessibility. */
                        slider.Enabled =
                        labelMin.Enabled =
                        labelMax.Enabled =
                        labelName.Enabled =
                        labelCurrentValue.Enabled = Enabled;
                    });
                }
                else
                {

                    slider.Minimum = min;
                    slider.Maximum = max;
                    slider.Value = val;
                    slider.SmallChange = inc;
                    slider.TickFrequency = (max - min + 5) / 10;

                    /* Update the values. */
                    labelMin.Text = "" + min;
                    labelMax.Text = "" + max;
                    labelCurrentValue.Text = "" + val;

                    /* Update accessibility. */
                    slider.Enabled =
                    labelMin.Enabled =
                    labelMax.Enabled =
                    labelName.Enabled =
                    labelCurrentValue.Enabled = Enabled;
                }

                return;
            }
            catch
            {
                Reset();
                /* If errors occurred disable the control. */
            }
        }

        private void Reset()
        {
            slider.Enabled = false;
            labelMin.Enabled = false;
            labelMax.Enabled = false;
            labelName.Enabled = false;
            labelCurrentValue.Enabled = false;
        }

        private void labelCurrentValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int t_v = 0;
                int.TryParse(labelCurrentValue.Text, out t_v);
                if (name == "Width" || name == "Height")
                {
                    t_v -= t_v % 4;
                    if (t_v < 0)
                    {
                        t_v = 0;
                    }
                }
                slider.Value = t_v;// Convert.ToInt32(labelCurrentValue.Text);
                slider_Scroll(sender, e);
            }
        }

        public void slider_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (Dev == null)
                {
                    return;
                }
                /* Correct the increment of the new value. */
                int value = slider.Value - ((slider.Value - slider.Minimum) % slider.SmallChange);
                if (name == "Width" || name == "Height")
                {
                    value -= value % 4;
                    if (value < 0)
                    {
                        value = 0;
                    }
                }
                /* Set the value. */
                if (Dev.DeviceInfo.Vendor.ToUpper() == "BASLER")
                {
                    if (name == "ExposureTime")
                    {
                        LVApp.Instance().m_GenICam.CAM[Cam_Num].ExposureTime = (double)value;
                        using (IIntegraParameter p = Dev.ParameterCollection[new IntegerName("ExposureTimeRaw")])
                        {
                            if (p.SetValue(value))
                            {
                                UpdateValues();
                            }
                        }
                    }
                    if (name == "GainRaw")
                    {
                        LVApp.Instance().m_GenICam.CAM[Cam_Num].GainRaw = (double)value;
                        using (IIntegraParameter p = Dev.ParameterCollection[new IntegerName("GainRaw")])
                        {
                            if (p.SetValue(value))
                            {
                                UpdateValues();
                            }
                        }
                    }
                }
                else
                {
                    if (name == "ExposureTime")
                    {
                        LVApp.Instance().m_GenICam.CAM[Cam_Num].ExposureTime = (double)value;
                        using (IFloatParameter p = Dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
                        {
                            if (p.SetValue((double)value))
                            {
                                UpdateValues();
                            }
                        }
                    }
                    if (name == "GainRaw")
                    {
                        LVApp.Instance().m_GenICam.CAM[Cam_Num].GainRaw = (double)value;
                        using (IFloatParameter p = Dev.ParameterCollection[ParametrizeNameSet.GainRaw])
                        {
                            if (p.SetValue((double)value))
                            {
                                UpdateValues();
                            }
                        }
                    }
                }
                if (name == "Width")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageWidth])
                    {
                        if (p.SetValue(value))
                        {
                            LVApp.Instance().m_GenICam.CAM[Cam_Num].Width = value;
                            UpdateValues();
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderOffsetX.UpdateValues();
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderOffsetX.UpdateValues();
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderOffsetX.UpdateValues();
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderOffsetX.UpdateValues();
                            }
                        }
                    }
                }
                if (name == "Height")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageHeight])
                    {
                        if (p.SetValue(value))
                        {
                            LVApp.Instance().m_GenICam.CAM[Cam_Num].Height = value;
                            UpdateValues();
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderOffsetY.UpdateValues();
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderOffsetY.UpdateValues();
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderOffsetY.UpdateValues();
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderOffsetY.UpdateValues();
                            }
                        }
                    }
                }
                if (name == "OffsetX")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetX])
                    {
                        if (p.SetValue(value))
                        {
                            LVApp.Instance().m_GenICam.CAM[Cam_Num].X_Offset = value;
                            UpdateValues();
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderWidth.UpdateValues();
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderWidth.UpdateValues();
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderWidth.UpdateValues();
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderWidth.UpdateValues();
                            }
                        }
                    }
                }
                if (name == "OffsetY")
                {
                    using (IIntegraParameter p = Dev.ParameterCollection[ParametrizeNameSet.ImageOffsetY])
                    {
                        if (p.SetValue(value))
                        {
                            LVApp.Instance().m_GenICam.CAM[Cam_Num].Y_Offset = value;
                            UpdateValues();
                            if (Cam_Num == 0)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting1.GeniCam_sliderHeight.UpdateValues();
                            }
                            else if (Cam_Num == 1)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting2.GeniCam_sliderHeight.UpdateValues();
                            }
                            else if (Cam_Num == 2)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting3.GeniCam_sliderHeight.UpdateValues();
                            }
                            else if (Cam_Num == 3)
                            {
                                LVApp.Instance().m_mainform.ctr_Camera_Setting4.GeniCam_sliderHeight.UpdateValues();
                            }
                        }
                    }
                }
            }
            catch
            {
                /* Ignore any errors here. */
            }
        }
    }
}
