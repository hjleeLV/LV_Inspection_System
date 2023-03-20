using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LV_Inspection_System.UTIL
{
    public class AI_Processing
    {
        private Net[,] net = new Net[4, 41];
        private Mat[,] blob = new Mat[4, 41];
        public bool[,] Flag_Model_Loaded = new bool[4, 41];
        public Mat[] m_Input_Image = new Mat[4];
        private List<string>[,] m_Label = new List<string>[4, 41];

        public void Model_Load(int Cam_Num, int ROI_Num, string Model_Path, string Label_Path)
        {
            try
            {
                if (m_Label[Cam_Num, ROI_Num] == null)
                {
                    m_Label[Cam_Num, ROI_Num] = new List<string>();
                }
                else
                {
                    m_Label[Cam_Num, ROI_Num].Clear();
                }
                Flag_Model_Loaded[Cam_Num, ROI_Num] = false;
                if (System.IO.File.Exists(Label_Path))
                {
                    string[] lines = System.IO.File.ReadAllLines(Label_Path);
                    foreach (string str in lines)
                    {
                        m_Label[Cam_Num, ROI_Num].Add(str);
                    }
                }
                else
                {
                    for (int i = 0;i < 40; i++)
                    {
                        m_Label[Cam_Num, ROI_Num].Add(i.ToString());
                    }
                }
                string Model_pb_Path = Path.GetDirectoryName(Model_Path) + "\\" + Path.GetFileNameWithoutExtension(Model_Path) + ".pb";
                if (System.IO.File.Exists(Model_pb_Path))
                {
                    net[Cam_Num, ROI_Num] = null;
                    net[Cam_Num, ROI_Num] = CvDnn.ReadNet(Model_pb_Path);
                    Flag_Model_Loaded[Cam_Num, ROI_Num] = true;
                    net[Cam_Num, ROI_Num].SetPreferableBackend(Backend.OPENCV);
                    net[Cam_Num, ROI_Num].SetPreferableTarget(Target.CPU);
                    //if (Cam_Num == 0)
                    //{LVApp
                    //    LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loaded.");
                    //}
                    //else if (Cam_Num == 1)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loaded.");
                    //}
                    //else if (Cam_Num == 2)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loaded.");
                    //}
                    //else if (Cam_Num == 3)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loaded.");
                    //}
                }
                else
                {
                    Flag_Model_Loaded[Cam_Num, ROI_Num] = false;
                    //if (Cam_Num == 0)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                    //}
                    //else if (Cam_Num == 1)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                    //}
                    //else if (Cam_Num == 2)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                    //}
                    //else if (Cam_Num == 3)
                    //{
                    //    LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                    //}
                }

            }
            catch (Exception ex)
            {
                Flag_Model_Loaded[Cam_Num, ROI_Num] = false;
                //if (Cam_Num == 0)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                //    LVApp.Instance().m_mainform.ctr_ROI1.ctr_ROI_Guide1.add_Log(ex.Message);
                //}
                //else if (Cam_Num == 1)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                //    LVApp.Instance().m_mainform.ctr_ROI2.ctr_ROI_Guide1.add_Log(ex.Message);
                //}
                //else if (Cam_Num == 2)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                //    LVApp.Instance().m_mainform.ctr_ROI3.ctr_ROI_Guide1.add_Log(ex.Message);
                //}
                //else if (Cam_Num == 3)
                //{
                //    LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log("CAM" + (Cam_Num + 1).ToString() + ":ROI" + ROI_Num.ToString() + " AI Model Loading Error!");
                //    LVApp.Instance().m_mainform.ctr_ROI4.ctr_ROI_Guide1.add_Log(ex.Message);
                //}
                Console.WriteLine(ex.Message);
            }
        }

        bool AI_Recognition_Image_Use_flag = false;
        public string AI_Recognition_Image(int Cam_Num, int ROI_Num, ref Bitmap t_BMP)
        {
            try
            {
                string Result_str = "";
                if (AI_Recognition_Image_Use_flag)
                {
                    return Result_str;
                }
                m_Input_Image[Cam_Num] = BitmapConverter.ToMat(t_BMP.Clone() as Bitmap);
                Mat t_Input_Image = new Mat();
                if (m_Input_Image[Cam_Num].Channels() == 3)
                {
                    Cv2.CvtColor(m_Input_Image[Cam_Num], t_Input_Image, ColorConversionCodes.BGR2RGB);
                }
                else if (m_Input_Image[Cam_Num].Channels() == 4)
                {
                    Cv2.CvtColor(m_Input_Image[Cam_Num], t_Input_Image, ColorConversionCodes.BGRA2RGB);
                }
                else
                {
                    Cv2.CvtColor(m_Input_Image[Cam_Num], t_Input_Image, ColorConversionCodes.GRAY2RGB);
                }
                if (!Flag_Model_Loaded[Cam_Num, ROI_Num])
                {
                    return "";
                }

                string[] input_layers = net[Cam_Num, ROI_Num].GetLayerNames();
                string[] output_layers = net[Cam_Num, ROI_Num].GetUnconnectedOutLayersNames();
                //blob = CvDnn.BlobFromImage(src, 1.0 / 127.5, new OpenCvSharp.Size(224, 224), new Scalar(127.5, 127.5, 127.5), true, false);
                blob[Cam_Num, ROI_Num] = CvDnn.BlobFromImage(t_Input_Image, 1.0 / 127, new OpenCvSharp.Size(224, 224), new Scalar(127, 127, 127), false, false);
                net[Cam_Num, ROI_Num].SetInput(blob[Cam_Num, ROI_Num]);
                int t_Softmax = 0;
                for (int i = 0; i < output_layers.Length; i++)
                {
                    if (output_layers[i].Contains("Softmax"))
                    {
                        t_Softmax = i;
                        break;
                    }
                }

                var prob = net[Cam_Num, ROI_Num].Forward(output_layers[t_Softmax]);
                var p = prob.Reshape(1, 1);
                for (int ss = 0; ss < p.Cols; ss++)
                {
                    if (m_Label[Cam_Num, ROI_Num].Count == p.Cols)
                    {
                        if (ss < p.Cols - 1)
                        {
                            Result_str += m_Label[Cam_Num, ROI_Num][ss] + ":" + (p.At<float>(0, ss)).ToString("0.00") + ";";
                        }
                        else
                        {
                            Result_str += m_Label[Cam_Num, ROI_Num][ss] + ":" + (p.At<float>(0, ss)).ToString("0.00");
                        }
                    }
                    else
                    {
                        if (ss < p.Cols - 1)
                        {
                            Result_str += ss.ToString() + ":" + (p.At<float>(0, ss)).ToString("0.00") + ";";
                        }
                        else
                        {
                            Result_str += ss.ToString() + ":" + (p.At<float>(0, ss)).ToString("0.00");
                        }
                    }
                }
                //GC.Collect();
                AI_Recognition_Image_Use_flag = false;
                return Result_str;
            }
            catch (Exception ex)
            {
                AI_Recognition_Image_Use_flag = false;
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }
}
