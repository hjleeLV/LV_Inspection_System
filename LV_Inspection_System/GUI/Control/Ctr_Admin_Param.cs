using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_Admin_Param : UserControl
    {
        public Ctr_Admin_Param()
        {
            InitializeComponent();
        }

        //int m_SetLanguage = -1;
        public void Get_Item_From_ROI()
        {
            try
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    checkBox_ADMINMODE.Text = "고급 관리자 모드";

                    if (comboBox_TABLETYPE0 == null)
                    {
                        return;
                    }
                    int t_v = 0;
                    t_v = comboBox_TABLETYPE0.SelectedIndex;
                    comboBox_TABLETYPE0.Items.Clear();
                    comboBox_TABLETYPE0.Items.Add("#1 인덱스 타입");
                    comboBox_TABLETYPE0.Items.Add("#2 유리판 타입");
                    comboBox_TABLETYPE0.Items.Add("#3 라인스캔 벨트 타입");
                    comboBox_TABLETYPE0.Items.Add("#4 가이드 없음");
                    comboBox_TABLETYPE0.Items.Add("#5 ROI 기준 측정");
                    comboBox_TABLETYPE0.Items.Add("#6 고객사 전용 타입");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE0.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION0.SelectedIndex;
                    comboBox_CAMPOSITION0.Items.Clear();
                    comboBox_CAMPOSITION0.Items.Add("#1 상부/하부");
                    comboBox_CAMPOSITION0.Items.Add("#2 사이드");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION0.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE1.SelectedIndex;
                    comboBox_TABLETYPE1.Items.Clear();
                    comboBox_TABLETYPE1.Items.Add("#1 인덱스 타입");
                    comboBox_TABLETYPE1.Items.Add("#2 유리판 타입");
                    comboBox_TABLETYPE1.Items.Add("#3 라인스캔 벨트 타입");
                    comboBox_TABLETYPE1.Items.Add("#4 가이드 없음");
                    comboBox_TABLETYPE1.Items.Add("#5 ROI 기준 측정");
                    comboBox_TABLETYPE1.Items.Add("#6 고객사 전용 타입");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE1.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION1.SelectedIndex;
                    comboBox_CAMPOSITION1.Items.Clear();
                    comboBox_CAMPOSITION1.Items.Add("#1 상부/하부");
                    comboBox_CAMPOSITION1.Items.Add("#2 사이드");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION1.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE2.SelectedIndex;
                    comboBox_TABLETYPE2.Items.Clear();
                    comboBox_TABLETYPE2.Items.Add("#1 인덱스 타입");
                    comboBox_TABLETYPE2.Items.Add("#2 유리판 타입");
                    comboBox_TABLETYPE2.Items.Add("#3 라인스캔 벨트 타입");
                    comboBox_TABLETYPE2.Items.Add("#4 가이드 없음");
                    comboBox_TABLETYPE2.Items.Add("#5 ROI 기준 측정");
                    comboBox_TABLETYPE2.Items.Add("#6 고객사 전용 타입");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE2.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION2.SelectedIndex;
                    comboBox_CAMPOSITION2.Items.Clear();
                    comboBox_CAMPOSITION2.Items.Add("#1 상부/하부");
                    comboBox_CAMPOSITION2.Items.Add("#2 사이드");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION2.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE3.SelectedIndex;
                    comboBox_TABLETYPE3.Items.Clear();
                    comboBox_TABLETYPE3.Items.Add("#1 인덱스 타입");
                    comboBox_TABLETYPE3.Items.Add("#2 유리판 타입");
                    comboBox_TABLETYPE3.Items.Add("#3 라인스캔 벨트 타입");
                    comboBox_TABLETYPE3.Items.Add("#4 가이드 없음");
                    comboBox_TABLETYPE3.Items.Add("#5 ROI 기준 측정");
                    comboBox_TABLETYPE3.Items.Add("#6 고객사 전용 타입");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE3.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION3.SelectedIndex;
                    comboBox_CAMPOSITION3.Items.Clear();
                    comboBox_CAMPOSITION3.Items.Add("#1 상부/하부");
                    comboBox_CAMPOSITION3.Items.Add("#2 사이드");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION3.SelectedIndex = t_v;
                    }

                    //comboBox_TABLETYPE0.SelectedIndex = 0;
                    //comboBox_TABLETYPE1.SelectedIndex = 0;
                    //comboBox_TABLETYPE2.SelectedIndex = 0;
                    //comboBox_TABLETYPE3.SelectedIndex = 0;

                    //comboBox_CAMPOSITION0.SelectedIndex = 0;
                    //comboBox_CAMPOSITION1.SelectedIndex = 0;
                    //comboBox_CAMPOSITION2.SelectedIndex = 0;
                    //comboBox_CAMPOSITION3.SelectedIndex = 0;

                    button_LOAD.Text = "불러오기";
                    button_SAVE.Text = "저장";
                    button_Logout.Text = "로그아웃";
                }
                else if(LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    checkBox_ADMINMODE.Text = "High-end Admin Mode";
                    
                    int t_v = 0;
                    t_v = comboBox_TABLETYPE0.SelectedIndex;
                    //comboBox_TABLETYPE0.Text = "#1 Index Type";
                    comboBox_TABLETYPE0.Items.Clear();
                    comboBox_TABLETYPE0.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE0.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE0.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE0.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE0.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE0.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE0.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION0.SelectedIndex;
                    //comboBox_CAMPOSITION0.Text = "#1 Top/Bottom";
                    comboBox_CAMPOSITION0.Items.Clear();
                    comboBox_CAMPOSITION0.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION0.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION0.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE1.SelectedIndex;
                    comboBox_TABLETYPE1.Items.Clear();
                    comboBox_TABLETYPE1.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE1.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE1.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE1.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE1.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE1.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE1.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION1.SelectedIndex;
                    comboBox_CAMPOSITION1.Items.Clear();
                    comboBox_CAMPOSITION1.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION1.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION1.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE2.SelectedIndex;
                    comboBox_TABLETYPE2.Items.Clear();
                    comboBox_TABLETYPE2.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE2.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE2.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE2.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE2.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE2.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE2.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION2.SelectedIndex;
                    comboBox_CAMPOSITION2.Items.Clear();
                    comboBox_CAMPOSITION2.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION2.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION2.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE3.SelectedIndex;
                    comboBox_TABLETYPE3.Items.Clear();
                    comboBox_TABLETYPE3.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE3.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE3.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE3.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE3.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE3.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE3.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION3.SelectedIndex;
                    comboBox_CAMPOSITION3.Items.Clear();
                    comboBox_CAMPOSITION3.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION3.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION3.SelectedIndex = t_v;
                    }
                    //comboBox_TABLETYPE0.SelectedIndex = 0;
                    //comboBox_TABLETYPE1.SelectedIndex = 0;
                    //comboBox_TABLETYPE2.SelectedIndex = 0;
                    //comboBox_TABLETYPE3.SelectedIndex = 0;

                    //comboBox_CAMPOSITION0.SelectedIndex = 0;
                    //comboBox_CAMPOSITION1.SelectedIndex = 0;
                    //comboBox_CAMPOSITION2.SelectedIndex = 0;
                    //comboBox_CAMPOSITION3.SelectedIndex = 0;
                    button_LOAD.Text = "LOAD";
                    button_SAVE.Text = "SAVE";
                    button_Logout.Text = "LOGOUT";

                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    checkBox_ADMINMODE.Text = "High-end Admin Mode";

                    int t_v = 0;
                    t_v = comboBox_TABLETYPE0.SelectedIndex;
                    //comboBox_TABLETYPE0.Text = "#1 Index Type";
                    comboBox_TABLETYPE0.Items.Clear();
                    comboBox_TABLETYPE0.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE0.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE0.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE0.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE0.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE0.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE0.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION0.SelectedIndex;
                    //comboBox_CAMPOSITION0.Text = "#1 Top/Bottom";
                    comboBox_CAMPOSITION0.Items.Clear();
                    comboBox_CAMPOSITION0.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION0.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION0.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE1.SelectedIndex;
                    comboBox_TABLETYPE1.Items.Clear();
                    comboBox_TABLETYPE1.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE1.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE1.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE1.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE1.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE1.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE1.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION1.SelectedIndex;
                    comboBox_CAMPOSITION1.Items.Clear();
                    comboBox_CAMPOSITION1.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION1.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION1.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE2.SelectedIndex;
                    comboBox_TABLETYPE2.Items.Clear();
                    comboBox_TABLETYPE2.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE2.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE2.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE2.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE2.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE2.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE2.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION2.SelectedIndex;
                    comboBox_CAMPOSITION2.Items.Clear();
                    comboBox_CAMPOSITION2.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION2.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION2.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_TABLETYPE3.SelectedIndex;
                    comboBox_TABLETYPE3.Items.Clear();
                    comboBox_TABLETYPE3.Items.Add("#1 Index Type");
                    comboBox_TABLETYPE3.Items.Add("#2 Glass Type");
                    comboBox_TABLETYPE3.Items.Add("#3 Linescan Belt Type");
                    comboBox_TABLETYPE3.Items.Add("#4 No Guide");
                    comboBox_TABLETYPE3.Items.Add("#5 Measure by ROI");
                    comboBox_TABLETYPE3.Items.Add("#6 Special Customer Type");
                    if (t_v >= 0)
                    {
                        comboBox_TABLETYPE3.SelectedIndex = t_v;
                    }

                    t_v = 0;
                    t_v = comboBox_CAMPOSITION3.SelectedIndex;
                    comboBox_CAMPOSITION3.Items.Clear();
                    comboBox_CAMPOSITION3.Items.Add("#1 Top/Bottom");
                    comboBox_CAMPOSITION3.Items.Add("#2 Side");
                    if (t_v >= 0)
                    {
                        comboBox_CAMPOSITION3.SelectedIndex = t_v;
                    }
                    //comboBox_TABLETYPE0.SelectedIndex = 0;
                    //comboBox_TABLETYPE1.SelectedIndex = 0;
                    //comboBox_TABLETYPE2.SelectedIndex = 0;
                    //comboBox_TABLETYPE3.SelectedIndex = 0;

                    //comboBox_CAMPOSITION0.SelectedIndex = 0;
                    //comboBox_CAMPOSITION1.SelectedIndex = 0;
                    //comboBox_CAMPOSITION2.SelectedIndex = 0;
                    //comboBox_CAMPOSITION3.SelectedIndex = 0;
                    button_LOAD.Text = "负荷";
                    button_SAVE.Text = "救";
                    button_Logout.Text = "注销";
                }

                LVApp.Instance().m_Config.nTableType[0] = comboBox_TABLETYPE0.SelectedIndex;
                LVApp.Instance().m_Config.nTableType[1] = comboBox_TABLETYPE1.SelectedIndex;
                LVApp.Instance().m_Config.nTableType[2] = comboBox_TABLETYPE2.SelectedIndex;
                LVApp.Instance().m_Config.nTableType[3] = comboBox_TABLETYPE3.SelectedIndex;

                LVApp.Instance().m_Config.m_Camera_Position[0] = comboBox_CAMPOSITION0.SelectedIndex;
                LVApp.Instance().m_Config.m_Camera_Position[1] = comboBox_CAMPOSITION1.SelectedIndex;
                LVApp.Instance().m_Config.m_Camera_Position[2] = comboBox_CAMPOSITION2.SelectedIndex;
                LVApp.Instance().m_Config.m_Camera_Position[3] = comboBox_CAMPOSITION3.SelectedIndex;
                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[0])
                {
                    LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 1;
                    checkedListBox1.Items.Clear();
                    if (LVApp.Instance().m_Config.m_Camera_Position[0] == 0)
                    {   // 상부 또는 하부이면 

                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of circle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB in circle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "직경";
                        }
                        else
                        {
                            t_str = "Diameter";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "진원도(%)";
                        }
                        else
                        {
                            t_str = "Circularity(%)";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "십자 치수";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of cross";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 색상 BLOB";
                        }
                        else
                        {
                            t_str = "Color BLOB in circle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "내외경 중심 차이";
                        }
                        else
                        {
                            t_str = "Center difference between Inner and outter circle";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();


                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox1.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                            {
                                checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }
                    }
                    else
                    {   // 측면이면
                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 두께";
                        }
                        else
                        {
                            t_str = "Thickness of body(mm)";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 휨";
                        }
                        else
                        {
                            t_str = "Bending of body(mm)";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Pitch of thread";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of thread";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(0.5)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(0.5)";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(1)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(1)";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 V 각도";
                        }
                        else
                        {
                            t_str = "V Angle of bottom";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "머리 나사부 동심도";
                        }
                        else
                        {
                            t_str = "Concentricity";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 형상";
                        }
                        else
                        {
                            t_str = "Shape of bottom";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox1.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_0.Tables[2].Rows[0][1].ToString();

                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox1.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                            {
                                checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[1])
                {
                    LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = 1;
                    checkedListBox2.Items.Clear();
                    if (LVApp.Instance().m_Config.m_Camera_Position[1] == 0)
                    {   // 상부 또는 하부이면 

                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of circle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB in circle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "직경";
                        }
                        else
                        {
                            t_str = "Diameter";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "진원도(%)";
                        }
                        else
                        {
                            t_str = "Circularity(%)";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "십자 치수";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of cross";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 색상 BLOB";
                        }
                        else
                        {
                            t_str = "Color BLOB in circle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "내외경 중심 차이";
                        }
                        else
                        {
                            t_str = "Center difference between Inner and outter circle";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();


                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox2.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                            {
                                checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }
                    }
                    else
                    {   // 측면이면
                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 두께";
                        }
                        else
                        {
                            t_str = "Thickness of body(mm)";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 휨";
                        }
                        else
                        {
                            t_str = "Bending of body(mm)";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Pitch of thread";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of thread";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(0.5)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(0.5)";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(1)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(1)";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 V 각도";
                        }
                        else
                        {
                            t_str = "V Angle of bottom";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "머리 나사부 동심도";
                        }
                        else
                        {
                            t_str = "Concentricity";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 형상";
                        }
                        else
                        {
                            t_str = "Shape of bottom";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox2.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_1.Tables[2].Rows[0][1].ToString();

                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox2.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                            {
                                checkedListBox2.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox2.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[2])
                {
                    LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = 1;
                    checkedListBox3.Items.Clear();
                    if (LVApp.Instance().m_Config.m_Camera_Position[2] == 0)
                    {   // 상부 또는 하부이면 

                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of circle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB in circle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "직경";
                        }
                        else
                        {
                            t_str = "Diameter";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "진원도(%)";
                        }
                        else
                        {
                            t_str = "Circularity(%)";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "십자 치수";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of cross";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 색상 BLOB";
                        }
                        else
                        {
                            t_str = "Color BLOB in circle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "내외경 중심 차이";
                        }
                        else
                        {
                            t_str = "Center difference between Inner and outter circle";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();


                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox3.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                            {
                                checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }

                    }
                    else
                    {   // 측면이면
                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 두께";
                        }
                        else
                        {
                            t_str = "Thickness of body(mm)";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 휨";
                        }
                        else
                        {
                            t_str = "Bending of body(mm)";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Pitch of thread";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of thread";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(0.5)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(0.5)";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(1)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(1)";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 V 각도";
                        }
                        else
                        {
                            t_str = "V Angle of bottom";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "머리 나사부 동심도";
                        }
                        else
                        {
                            t_str = "Concentricity";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 형상";
                        }
                        else
                        {
                            t_str = "Shape of bottom";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox3.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_2.Tables[2].Rows[0][1].ToString();

                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox3.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                            {
                                checkedListBox3.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox3.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!LVApp.Instance().m_Config.ctr_Camera_Setting_Force_USE[3])
                {
                    LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = 1;
                    checkedListBox4.Items.Clear();
                    if (LVApp.Instance().m_Config.m_Camera_Position[3] == 0)
                    {   // 상부 또는 하부이면 

                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of circle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB in circle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "직경";
                        }
                        else
                        {
                            t_str = "Diameter";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "진원도(%)";
                        }
                        else
                        {
                            t_str = "Circularity(%)";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "십자 치수";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Dim. of cross";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of cross";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "원형 영역의 색상 BLOB";
                        }
                        else
                        {
                            t_str = "Color BLOB in circle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "내외경 중심 차이";
                        }
                        else
                        {
                            t_str = "Center difference between Inner and outter circle";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();


                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox4.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                            {
                                checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }

                    }
                    else
                    {   // 측면이면
                        int i = 0;
                        string t_str = "";
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "가로 길이";
                        }
                        else
                        {
                            t_str = "Hor. length";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "세로 길이";
                        }
                        else
                        {
                            t_str = "Ver. length";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 두께";
                        }
                        else
                        {
                            t_str = "Thickness of body(mm)";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "몸통 휨";
                        }
                        else
                        {
                            t_str = "Bending of body(mm)";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 피치";
                        }
                        else
                        {
                            t_str = "Pitch of thread";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "나사산 크기";
                        }
                        else
                        {
                            t_str = "Size of thread";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(0.5)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(0.5)";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "리드각(1)";
                        }
                        else
                        {
                            t_str = "Lead angle of thread(1)";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 밝기";
                        }
                        else
                        {
                            t_str = "Brightness of rectangle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "사각 영역의 BLOB";
                        }
                        else
                        {
                            t_str = "BLOB of rectangle ROI";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 V 각도";
                        }
                        else
                        {
                            t_str = "V Angle of bottom";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "머리 나사부 동심도";
                        }
                        else
                        {
                            t_str = "Concentricity";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "하부 형상";
                        }
                        else
                        {
                            t_str = "Shape of bottom";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }
                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "두 영역 중심간 거리";
                        }
                        else
                        {
                            t_str = "Distance between two area";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "면취 측정";
                        }
                        else
                        {
                            t_str = "Bevelling Measurement";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }


                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "AI 검사";
                        }
                        else
                        {
                            t_str = "AI Inspection";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                        {//한국어
                            t_str = "SSF";
                        }
                        else
                        {
                            t_str = "SSF";
                        }
                        checkedListBox4.Items.Add(t_str);
                        if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                        }

                        i++;
                        string main_str = "";
                        main_str = LVApp.Instance().m_Config.ds_DATA_3.Tables[2].Rows[0][1].ToString();

                        if (main_str.Contains("모델 사용") || main_str.Contains("Model find"))
                        {
                            if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                            {//한국어
                                t_str = "일치율(%)";
                            }
                            else
                            {
                                t_str = "Match rate(%)";
                            }
                            checkedListBox4.Items.Add(t_str);
                            if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                            {
                                checkedListBox4.SetItemCheckState(i, CheckState.Checked);
                            }
                            else
                            {
                                checkedListBox4.SetItemCheckState(i, CheckState.Unchecked);
                            }
                        }
                    }
                }
                LVApp.Instance().m_mainform.ctr_ROI1.listBox1.SelectedIndex = 0;

                LVApp.Instance().m_mainform.ctr_ROI2.listBox1.SelectedIndex = 0;

                LVApp.Instance().m_mainform.ctr_ROI3.listBox1.SelectedIndex = 0;

                LVApp.Instance().m_mainform.ctr_ROI4.listBox1.SelectedIndex = 0;

            }
            catch
            {

            }
        }

        private void Ctr_Admin_Param_SizeChanged(object sender, EventArgs e)
        {
            TableLayoutRowStyleCollection styles = tableLayoutPanel1.RowStyles;

            int t_row_idx = 0;
            foreach (RowStyle style in styles)
            {
                style.SizeType = SizeType.Percent;
                if (t_row_idx == 0)
                {
                    style.Height = 3;
                }
                else if (t_row_idx == 1)
                {
                    style.Height = 44;
                }
                else if (t_row_idx == 2)
                {
                    style.Height = 3;
                }
                else if (t_row_idx == 3)
                {
                    style.Height = 44;
                }
                else if (t_row_idx == 4)
                {
                    style.Height = 6;
                }
                t_row_idx++;
            }

            TableLayoutColumnStyleCollection styles1 = tableLayoutPanel1.ColumnStyles;

            t_row_idx = 0;
            foreach (ColumnStyle style in styles1)
            {
                style.SizeType = SizeType.Percent;
                if (t_row_idx == 0)
                {
                    style.Width = 40;
                }
                else if (t_row_idx == 1)
                {
                    style.Width = 10;
                }
                else if (t_row_idx == 2)
                {
                    style.Width = 40;
                }
                else if (t_row_idx == 3)
                {
                    style.Width = 5;
                }
                t_row_idx++;
            }

            button_LOAD.Width = button_SAVE.Width = -5 + panel1.Width / 2;
            button_SAVE.Left = button_LOAD.Left + button_SAVE.Width + 5;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0] = true;
                }
                else
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0] = false;
                }
            }

            if (!checkedListBox1.GetItemChecked(e.Index))
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 0] = true;
            }
            else
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 0] = false;
            }

            LVApp.Instance().m_mainform.ctr_ROI1.Referesh_Select_Menu(true);
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                if (checkedListBox2.GetItemCheckState(i) == CheckState.Checked)
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1] = true;
                }
                else
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1] = false;
                }
            }
            if (!checkedListBox2.GetItemChecked(e.Index))
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 1] = true;
            }
            else
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 1] = false;
            }

            LVApp.Instance().m_mainform.ctr_ROI2.Referesh_Select_Menu(true);
        }

        private void checkedListBox3_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox3.Items.Count; i++)
            {
                if (checkedListBox3.GetItemCheckState(i) == CheckState.Checked)
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2] = true;
                }
                else
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2] = false;
                }
            }
            if (!checkedListBox3.GetItemChecked(e.Index))
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 2] = true;
            }
            else
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 2] = false;
            }

            LVApp.Instance().m_mainform.ctr_ROI3.Referesh_Select_Menu(true);
        }

        private void checkedListBox4_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox4.Items.Count; i++)
            {
                if (checkedListBox4.GetItemCheckState(i) == CheckState.Checked)
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3] = true;
                }
                else
                {
                    LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3] = false;
                }
            }
            if (!checkedListBox4.GetItemChecked(e.Index))
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 3] = true;
            }
            else
            {
                LVApp.Instance().m_Config.m_ROI_ALG_Check[e.Index, 3] = false;
            }
            LVApp.Instance().m_mainform.ctr_ROI4.Referesh_Select_Menu(true);
        }

        public void button_LOAD_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\Alg_setting.txt";
                if (System.IO.File.Exists(filename))
                {
                    StreamReader sr = new StreamReader(filename);
                    String line = sr.ReadLine();
                    String[] t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0] = t_str[i] == "0" ? false : true;
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1] = t_str[i] == "0" ? false : true;
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2] = t_str[i] == "0" ? false : true;
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3] = t_str[i] == "0" ? false : true;
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        sr.Close();
                        Get_Item_From_ROI();
                        return;
                    }
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE0.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION0.SelectedIndex = int.Parse(t_str[1]);
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE1.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION1.SelectedIndex = int.Parse(t_str[1]);
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE2.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION2.SelectedIndex = int.Parse(t_str[1]);
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE3.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION3.SelectedIndex = int.Parse(t_str[1]);
                    }

                    sr.Close();
                    Get_Item_From_ROI();
                }
            }
            catch
            {

            }
        }


        public void Load_Combobox()
        {
            try
            {
                string filename = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\Alg_setting.txt";
                if (System.IO.File.Exists(filename))
                {
                    StreamReader sr = new StreamReader(filename);
                    String line = sr.ReadLine();
                    String[] t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0] = t_str[i] == "0" ? false : true;
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1] = t_str[i] == "0" ? false : true;
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2] = t_str[i] == "0" ? false : true;
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    for (int i = 0; i < 20; i++)
                    {
                        LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3] = t_str[i] == "0" ? false : true;
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        sr.Close();
                        return;
                    }
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE0.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION0.SelectedIndex = int.Parse(t_str[1]);
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE1.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION1.SelectedIndex = int.Parse(t_str[1]);
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE2.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION2.SelectedIndex = int.Parse(t_str[1]);
                    }
                    line = sr.ReadLine();
                    t_str = line.Split('_');
                    if (t_str.Length > 1)
                    {
                        comboBox_TABLETYPE3.SelectedIndex = int.Parse(t_str[0]);
                        comboBox_CAMPOSITION3.SelectedIndex = int.Parse(t_str[1]);
                    }

                    sr.Close();

                    LVApp.Instance().m_Config.nTableType[0] = comboBox_TABLETYPE0.SelectedIndex;
                    LVApp.Instance().m_Config.nTableType[1] = comboBox_TABLETYPE1.SelectedIndex;
                    LVApp.Instance().m_Config.nTableType[2] = comboBox_TABLETYPE2.SelectedIndex;
                    LVApp.Instance().m_Config.nTableType[3] = comboBox_TABLETYPE3.SelectedIndex;

                    LVApp.Instance().m_Config.m_Camera_Position[0] = comboBox_CAMPOSITION0.SelectedIndex;
                    LVApp.Instance().m_Config.m_Camera_Position[1] = comboBox_CAMPOSITION1.SelectedIndex;
                    LVApp.Instance().m_Config.m_Camera_Position[2] = comboBox_CAMPOSITION2.SelectedIndex;
                    LVApp.Instance().m_Config.m_Camera_Position[3] = comboBox_CAMPOSITION3.SelectedIndex;
                }
            }
            catch
            {

            }
        }
        public void button_SAVE_Click(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_Config.m_Model_Name == "")
            {
                if (LVApp.Instance().m_Config.m_SetLanguage == 0)
                {//한국어
                    LVApp.Instance().m_mainform.add_Log("모델을 등록후 사용하세요.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 1)
                {//영어
                    LVApp.Instance().m_mainform.add_Log("Use after registering a model.");
                }
                else if (LVApp.Instance().m_Config.m_SetLanguage == 2)
                {//중국어
                    LVApp.Instance().m_mainform.add_Log("注册模型后使用.");
                }
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name);
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }
            dir = new DirectoryInfo(LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param");
            // 폴더가 존재하지 않으면
            if (dir.Exists == false)
            {
                // 새로 생성합니다.
                dir.Create();
            }

            try
            {
                string t_file = LVApp.Instance().excute_path + "\\Models\\" + LVApp.Instance().m_Config.m_Model_Name + "\\ROI_Param\\Alg_setting.txt";
                string t_str = "";
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(t_file);
                for (int i = 0; i < 20; i++)
                {
                    if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 0])
                    {
                        t_str += "1_";
                    }
                    else
                    {
                        t_str += "0_";
                    }
                }
                //Write a line of text
                sw.WriteLine(t_str);

                t_str = "";
                for (int i = 0; i < 20; i++)
                {
                    if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 1])
                    {
                        t_str += "1_";
                    }
                    else
                    {
                        t_str += "0_";
                    }
                }
                //Write a line of text
                sw.WriteLine(t_str);
                t_str = "";
                for (int i = 0; i < 20; i++)
                {
                    if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 2])
                    {
                        t_str += "1_";
                    }
                    else
                    {
                        t_str += "0_";
                    }
                }
                //Write a line of text
                sw.WriteLine(t_str);
                t_str = "";
                for (int i = 0; i < 20; i++)
                {
                    if (LVApp.Instance().m_Config.m_ROI_ALG_Check[i, 3])
                    {
                        t_str += "1_";
                    }
                    else
                    {
                        t_str += "0_";
                    }
                }
                //Write a line of text
                sw.WriteLine(t_str);

                t_str = "";
                t_str += comboBox_TABLETYPE0.SelectedIndex.ToString() + "_" + comboBox_CAMPOSITION0.SelectedIndex.ToString();
                //Write a line of text
                sw.WriteLine(t_str);

                t_str = "";
                t_str += comboBox_TABLETYPE1.SelectedIndex.ToString() + "_" + comboBox_CAMPOSITION1.SelectedIndex.ToString();
                //Write a line of text
                sw.WriteLine(t_str);

                t_str = "";
                t_str += comboBox_TABLETYPE2.SelectedIndex.ToString() + "_" + comboBox_CAMPOSITION2.SelectedIndex.ToString();
                //Write a line of text
                sw.WriteLine(t_str);

                t_str = "";
                t_str += comboBox_TABLETYPE3.SelectedIndex.ToString() + "_" + comboBox_CAMPOSITION3.SelectedIndex.ToString();
                //Write a line of text
                sw.WriteLine(t_str);

                //Close the file
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                button_LOAD_Click(sender, e);
                Console.WriteLine("Executing finally block.");
            }

        }

        private void button_Logout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to logout?", " LOGOUT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LVApp.Instance().m_Config.m_Administrator_Super_Password_Flag = false;
                LVApp.Instance().m_mainform.neoTabWindow_INSP_SETTING.SelectedIndex = 1;
                checkBox_ADMINMODE.Checked = false;
            }
        }

        private void comboBox_TABLETYPE0_SelectedIndexChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_ROI1.comboBox_TABLETYPE.SelectedIndex = comboBox_TABLETYPE0.SelectedIndex;
        }

        private void comboBox_TABLETYPE1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_ROI2.comboBox_TABLETYPE.SelectedIndex = comboBox_TABLETYPE1.SelectedIndex;
        }

        private void comboBox_TABLETYPE2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_ROI3.comboBox_TABLETYPE.SelectedIndex = comboBox_TABLETYPE2.SelectedIndex;
        }

        private void comboBox_TABLETYPE3_SelectedIndexChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_ROI4.comboBox_TABLETYPE.SelectedIndex = comboBox_TABLETYPE3.SelectedIndex;
        }

        private void comboBox_CAMPOSITION0_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_mainform.ctr_ROI1.comboBox_CAMPOSITION.SelectedIndex != comboBox_CAMPOSITION0.SelectedIndex)
            {
                LVApp.Instance().m_mainform.ctr_ROI1.comboBox_CAMPOSITION.SelectedIndex = comboBox_CAMPOSITION0.SelectedIndex;
                Get_Item_From_ROI();
            }
        }

        private void comboBox_CAMPOSITION1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_mainform.ctr_ROI2.comboBox_CAMPOSITION.SelectedIndex != comboBox_CAMPOSITION1.SelectedIndex)
            {
                LVApp.Instance().m_mainform.ctr_ROI2.comboBox_CAMPOSITION.SelectedIndex = comboBox_CAMPOSITION1.SelectedIndex;
                Get_Item_From_ROI();
            }
        }

        private void comboBox_CAMPOSITION2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_mainform.ctr_ROI3.comboBox_CAMPOSITION.SelectedIndex != comboBox_CAMPOSITION2.SelectedIndex)
            {
                LVApp.Instance().m_mainform.ctr_ROI3.comboBox_CAMPOSITION.SelectedIndex = comboBox_CAMPOSITION2.SelectedIndex;
                Get_Item_From_ROI();
            }
        }

        private void comboBox_CAMPOSITION3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LVApp.Instance().m_mainform.ctr_ROI4.comboBox_CAMPOSITION.SelectedIndex != comboBox_CAMPOSITION3.SelectedIndex)
            {
                LVApp.Instance().m_mainform.ctr_ROI4.comboBox_CAMPOSITION.SelectedIndex = comboBox_CAMPOSITION3.SelectedIndex;
                Get_Item_From_ROI();
            }
        }

        private void checkBox_ADMINMODE_CheckedChanged(object sender, EventArgs e)
        {
            LVApp.Instance().m_mainform.ctr_ROI1.m_advenced_param_visible = !checkBox_ADMINMODE.Checked;
            LVApp.Instance().m_mainform.ctr_ROI2.m_advenced_param_visible = !checkBox_ADMINMODE.Checked;
            LVApp.Instance().m_mainform.ctr_ROI3.m_advenced_param_visible = !checkBox_ADMINMODE.Checked;
            LVApp.Instance().m_mainform.ctr_ROI4.m_advenced_param_visible = !checkBox_ADMINMODE.Checked;

            LVApp.Instance().m_mainform.ctr_ROI1.Advenced_Parameter(sender, e);
            LVApp.Instance().m_mainform.ctr_ROI2.Advenced_Parameter(sender, e);
            LVApp.Instance().m_mainform.ctr_ROI3.Advenced_Parameter(sender, e);
            LVApp.Instance().m_mainform.ctr_ROI4.Advenced_Parameter(sender, e);
        }
    }
}
