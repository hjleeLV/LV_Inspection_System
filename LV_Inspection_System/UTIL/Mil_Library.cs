using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LV_Inspection_System.UTIL
{
    /// <summary>
    /// MIL 라이브러리 c#용 개발
    /// 작성자 : CDJung
    /// 마지막 수정 : 2020.08.18
    /// </summary>
    public class Mil_Library
    {
        public MIL_ID MilApplication = MIL.M_NULL; // 어플리케이션
        public MIL_ID[] MilSystem = new MIL_ID[2]; // 프레임그래버
        public MIL_ID[] MilDigitizer = new MIL_ID[4]; // 카메라 4대
        public MIL_ID[] MilDisplay = new MIL_ID[4];
        public MIL_ID[] MilImageDisp = new MIL_ID[4];
        private const int BUFFERING_SIZE_MAX = 2;

        public MIL_ID[] CAM0_MilGrabBufferList = new MIL_ID[BUFFERING_SIZE_MAX];
        public MIL_ID[] CAM1_MilGrabBufferList = new MIL_ID[BUFFERING_SIZE_MAX];
        public MIL_ID[] CAM2_MilGrabBufferList = new MIL_ID[BUFFERING_SIZE_MAX];
        public MIL_ID[] CAM3_MilGrabBufferList = new MIL_ID[BUFFERING_SIZE_MAX];

        public Bitmap[] CAM0_MilGrabBMPList = new Bitmap[BUFFERING_SIZE_MAX];
        public Bitmap[] CAM1_MilGrabBMPList = new Bitmap[BUFFERING_SIZE_MAX];
        public Bitmap[] CAM2_MilGrabBMPList = new Bitmap[BUFFERING_SIZE_MAX];
        public Bitmap[] CAM3_MilGrabBMPList = new Bitmap[BUFFERING_SIZE_MAX];

        public bool CAM0_Initialized = false;
        public bool CAM1_Initialized = false;
        public bool CAM2_Initialized = false;
        public bool CAM3_Initialized = false;

        public int CAM0_MilGrabBufferIndex = 0;
        public int CAM1_MilGrabBufferIndex = 0;
        public int CAM2_MilGrabBufferIndex = 0;
        public int CAM3_MilGrabBufferIndex = 0;

        public int CAM0_MilGrabCount = 0;
        public int CAM1_MilGrabCount = 0;
        public int CAM2_MilGrabCount = 0;
        public int CAM3_MilGrabCount = 0;

        public bool CAM0_Grabbing = false;
        public bool CAM1_Grabbing = false;
        public bool CAM2_Grabbing = false;
        public bool CAM3_Grabbing = false;

        private int CAM0_Width = 0;
        private int CAM0_Height = 0;
        private int CAM1_Width = 0;
        private int CAM1_Height = 0;
        private int CAM2_Width = 0;
        private int CAM2_Height = 0;
        private int CAM3_Width = 0;
        private int CAM3_Height = 0;

        public string CAM0_dcfFilePath = "CAM0.dcf";
        public int CAM0_MIL_SystemNum = 1;
        public int CAM0_MIL_CH = 0;
        public string CAM1_dcfFilePath = "CAM1.dcf";
        public int CAM1_MIL_SystemNum = 1;
        public int CAM1_MIL_CH = 1;
        public string CAM2_dcfFilePath = "CAM2.dcf";
        public int CAM2_MIL_SystemNum = 0;
        public int CAM2_MIL_CH = 0;
        public string CAM3_dcfFilePath = "CAM3.dcf";
        public int CAM3_MIL_SystemNum = 0;
        public int CAM3_MIL_CH = 1;

        public int CAM0_MIL_GBOARD = 1;
        public int CAM1_MIL_GBOARD = 1;
        public int CAM2_MIL_GBOARD = 1;
        public int CAM3_MIL_GBOARD = 1;

        public bool[] m_Auto_Manual_Mode_Use = new bool[4];
        HookDataStruct CAM0_UserHookData = new HookDataStruct();
        HookDataStruct CAM1_UserHookData = new HookDataStruct();
        HookDataStruct CAM2_UserHookData = new HookDataStruct();
        HookDataStruct CAM3_UserHookData = new HookDataStruct();
        //GCHandle CAM0_hUserData;
        //GCHandle CAM1_hUserData;
        //GCHandle CAM2_hUserData;
        //GCHandle CAM3_hUserData;
        MIL_DIG_HOOK_FUNCTION_PTR CAM0_ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(CAM0_ProcessingFunction);
        MIL_DIG_HOOK_FUNCTION_PTR CAM1_ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(CAM1_ProcessingFunction);
        MIL_DIG_HOOK_FUNCTION_PTR CAM2_ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(CAM2_ProcessingFunction);
        MIL_DIG_HOOK_FUNCTION_PTR CAM3_ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(CAM3_ProcessingFunction);

        public event EventHandler CAM0_Grab_Completed;
        public event EventHandler CAM1_Grab_Completed;
        public event EventHandler CAM2_Grab_Completed;
        public event EventHandler CAM3_Grab_Completed;
        private EventArgs CAM0_Grab_Event = new EventArgs();
        private EventArgs CAM1_Grab_Event = new EventArgs();
        private EventArgs CAM2_Grab_Event = new EventArgs();
        private EventArgs CAM3_Grab_Event = new EventArgs();

        public class HookDataStruct
        {
            public MIL_ID MilDigitizer;
            public MIL_ID MilImageDisp;
            public int ProcessedImageCount;
        };

        public void Mil_Initialize(int Cam_Num)
        {
            try
            {
                //// 라이센스 체크
                //MIL_INT t_Licence_Check = MIL.MappInquire(MIL.M_LICENSE_MODULES);
                //if (t_Licence_Check == 0)
                //{
                //    CAM0_Initialized = false;
                //    CAM1_Initialized = false;
                //    CAM2_Initialized = false;
                //    CAM3_Initialized = false;
                //    return;
                //}

                //카메라 초기 설정  
                if (MilApplication == MIL.M_NULL)
                {
                    MIL.MappAlloc(MIL.M_DEFAULT, ref MilApplication);                                       // 어플리케이션 할당
                }
                else
                {
                    //if (Cam_Num == 0)
                    //{
                    //    CAM0_Initialized = true;
                    //}
                    //else if (Cam_Num == 1)
                    //{
                    //    CAM1_Initialized = true;
                    //}
                    //else if (Cam_Num == 2)
                    //{
                    //    CAM2_Initialized = true;
                    //}
                    //else if (Cam_Num == 3)
                    //{
                    //    CAM3_Initialized = true;
                    //}
                    //return;
                }
                
                if (Cam_Num == 0)
                {
                    if (CAM0_Initialized)
                    {
                        return;
                    }
                    if (CAM0_MIL_SystemNum == 0)
                    {
                        if (MilSystem[CAM0_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM0_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM0_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM0_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM0_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM0_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM0_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }
                    else if (CAM0_MIL_SystemNum == 1)
                    {
                        if (MilSystem[CAM0_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM0_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM0_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM0_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM0_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM0_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM0_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }

                    MIL.MdigAlloc(MilSystem[CAM0_MIL_SystemNum], CAM0_MIL_CH, CAM0_dcfFilePath, MIL.M_DEFAULT, ref MilDigitizer[Cam_Num]);  // 카메라 할당
                    MIL.MdispAlloc(MilSystem[CAM0_MIL_SystemNum], MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MilDisplay[Cam_Num]);   // 디스플레이 할당
                    CAM0_Width = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL);
                    CAM0_Height = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL);
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        // 버퍼 할당
                        MIL.MbufAlloc2d(MilSystem[CAM0_MIL_SystemNum],
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL),
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL),
                                        8 + MIL.M_UNSIGNED,
                                        MIL.M_IMAGE + MIL.M_GRAB,
                                        ref CAM0_MilGrabBufferList[i]);

                        if (CAM0_MilGrabBufferList[i] != MIL.M_NULL)
                        {
                            MIL.MbufClear(CAM0_MilGrabBufferList[i], 0xFF);
                        }
                        else
                        {
                            break;
                        }
                    }
                    MIL.MappControl(MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_ENABLE);

                    //CAM0_UserHookData.MilDigitizer = MilDigitizer[Cam_Num];
                    //CAM0_UserHookData.MilImageDisp = MilImageDisp[Cam_Num];
                    //CAM0_UserHookData.ProcessedImageCount = 0;
                    //CAM0_hUserData = GCHandle.Alloc(CAM0_UserHookData);
                    CAM0_Initialized = true;
                }
                else if (Cam_Num == 1)
                {
                    if (CAM1_Initialized)
                    {
                        return;
                    }
                    if (CAM1_MIL_SystemNum == 0)
                    {
                        if (MilSystem[CAM1_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM1_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM1_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM1_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM1_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM1_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM1_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }
                    else if (CAM1_MIL_SystemNum == 1)
                    {
                        if (MilSystem[CAM1_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM1_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM1_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM1_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM1_MIL_SystemNum]);          // 프레임 그래버 할당
                            }

                        }
                        //else
                        //{
                        //    MilSystem[CAM1_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM1_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }

                    MIL.MdigAlloc(MilSystem[CAM1_MIL_SystemNum], CAM1_MIL_CH, CAM1_dcfFilePath, MIL.M_DEFAULT, ref MilDigitizer[Cam_Num]);  // 카메라 할당
                    MIL.MdispAlloc(MilSystem[CAM1_MIL_SystemNum], MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MilDisplay[Cam_Num]);   // 디스플레이 할당
                    CAM1_Width = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL);
                    CAM1_Height = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL);
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        // 버퍼 할당
                        MIL.MbufAlloc2d(MilSystem[CAM1_MIL_SystemNum],
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL),
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL),
                                        8 + MIL.M_UNSIGNED,
                                        MIL.M_IMAGE + MIL.M_GRAB,
                                        ref CAM1_MilGrabBufferList[i]);

                        if (CAM1_MilGrabBufferList[i] != MIL.M_NULL)
                        {
                            MIL.MbufClear(CAM1_MilGrabBufferList[i], 0xFF);
                        }
                        else
                        {
                            break;
                        }
                    }
                    MIL.MappControl(MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_ENABLE);

                    //CAM1_UserHookData.MilDigitizer = MilDigitizer[Cam_Num];
                    //CAM1_UserHookData.MilImageDisp = MilImageDisp[Cam_Num];
                    //CAM1_UserHookData.ProcessedImageCount = 0;
                    //CAM1_hUserData = GCHandle.Alloc(CAM1_UserHookData);
                    CAM1_Initialized = true;
                }
                else if (Cam_Num == 2)
                {
                    if (CAM2_Initialized)
                    {
                        return;
                    }
                    if (CAM2_MIL_SystemNum == 0)
                    {
                        if (MilSystem[CAM2_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM2_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM2_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM2_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM2_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM2_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM2_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }
                    else if (CAM2_MIL_SystemNum == 1)
                    {
                        if (MilSystem[CAM2_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM2_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM2_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM2_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM2_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM2_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM2_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }

                    MIL.MdigAlloc(MilSystem[CAM2_MIL_SystemNum], CAM2_MIL_CH, CAM2_dcfFilePath, MIL.M_DEFAULT, ref MilDigitizer[Cam_Num]);  // 카메라 할당
                    MIL.MdispAlloc(MilSystem[CAM2_MIL_SystemNum], MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MilDisplay[Cam_Num]);   // 디스플레이 할당
                    CAM2_Width = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL);
                    CAM2_Height = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL);
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        // 버퍼 할당
                        MIL.MbufAlloc2d(MilSystem[CAM2_MIL_SystemNum],
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL),
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL),
                                        8 + MIL.M_UNSIGNED,
                                        MIL.M_IMAGE + MIL.M_GRAB,
                                        ref CAM2_MilGrabBufferList[i]);

                        if (CAM2_MilGrabBufferList[i] != MIL.M_NULL)
                        {
                            MIL.MbufClear(CAM2_MilGrabBufferList[i], 0xFF);
                        }
                        else
                        {
                            break;
                        }
                    }
                    MIL.MappControl(MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_ENABLE);

                    //CAM2_UserHookData.MilDigitizer = MilDigitizer[Cam_Num];
                    //CAM2_UserHookData.MilImageDisp = MilImageDisp[Cam_Num];
                    //CAM2_UserHookData.ProcessedImageCount = 0;
                    //CAM2_hUserData = GCHandle.Alloc(CAM2_UserHookData);
                    CAM2_Initialized = true;
                }
                else if (Cam_Num == 3)
                {
                    if (CAM3_Initialized)
                    {
                        return;
                    }
                    if (CAM3_MIL_SystemNum == 0)
                    {
                        if (MilSystem[CAM3_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM3_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM3_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM3_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM3_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM3_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0, MIL.M_COMPLETE, ref MilSystem[CAM3_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }
                    else if (CAM3_MIL_SystemNum == 1)
                    {
                        if (MilSystem[CAM3_MIL_SystemNum] == MIL.M_NULL)
                        {
                            if (CAM3_MIL_GBOARD == 0)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM3_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                            else if (CAM3_MIL_GBOARD == 1)
                            {
                                MIL.MsysAlloc(MIL.M_SYSTEM_RADIENTEVCL, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM3_MIL_SystemNum]);          // 프레임 그래버 할당
                            }
                        }
                        //else
                        //{
                        //    MilSystem[CAM3_MIL_SystemNum] = MIL.M_NULL;
                        //    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV1, MIL.M_COMPLETE, ref MilSystem[CAM3_MIL_SystemNum]);          // 프레임 그래버 할당
                        //}
                    }

                    MIL.MdigAlloc(MilSystem[CAM3_MIL_SystemNum], CAM3_MIL_CH, CAM3_dcfFilePath, MIL.M_DEFAULT, ref MilDigitizer[Cam_Num]);  // 카메라 할당
                    MIL.MdispAlloc(MilSystem[CAM3_MIL_SystemNum], MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MilDisplay[Cam_Num]);   // 디스플레이 할당
                    CAM3_Width = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL);
                    CAM3_Height = (int)MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL);
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        // 버퍼 할당
                        MIL.MbufAlloc2d(MilSystem[CAM3_MIL_SystemNum],
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_X, MIL.M_NULL),
                                        MIL.MdigInquire(MilDigitizer[Cam_Num], MIL.M_SIZE_Y, MIL.M_NULL),
                                        8 + MIL.M_UNSIGNED,
                                        MIL.M_IMAGE + MIL.M_GRAB,
                                        ref CAM3_MilGrabBufferList[i]);

                        if (CAM3_MilGrabBufferList[i] != MIL.M_NULL)
                        {
                            MIL.MbufClear(CAM3_MilGrabBufferList[i], 0xFF);
                        }
                        else
                        {
                            break;
                        }
                    }
                    MIL.MappControl(MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_ENABLE);

                    //CAM3_UserHookData.MilDigitizer = MilDigitizer[Cam_Num];
                    //CAM3_UserHookData.MilImageDisp = MilImageDisp[Cam_Num];
                    //CAM3_UserHookData.ProcessedImageCount = 0;
                    //CAM3_hUserData = GCHandle.Alloc(CAM3_UserHookData);
                    CAM3_Initialized = true;
                }
            }
            catch
            {

            }
        }

        public void CAM0_Mil_Grab()
        {
            try
            {
                if (!CAM0_Initialized || LVApp.Instance().m_mainform.Force_close || CAM0_Grabbing)
                {
                    return;
                }
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (!LVApp.Instance().m_mainform.t_cam_setting_view_mode)
                    {
                        return;
                    }
                }
                CAM0_Grabbing = true;
                int Cam_Num = 0;
                CAM0_MilGrabBufferIndex++;
                CAM0_MilGrabBufferIndex %= BUFFERING_SIZE_MAX;

                MIL.MdigGrab(MilDigitizer[Cam_Num], CAM0_MilGrabBufferList[CAM0_MilGrabBufferIndex]);
                CAM0_MilGrabCount++;
                // Stop the processing.
                //MIL.MdigProcess(MilDigitizer, MilGrabBufferList, 1, MIL.M_STOP, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));

                byte[] Imagearr = new byte[CAM0_Width * CAM0_Height];
                MIL.MbufGet(CAM0_MilGrabBufferList[CAM0_MilGrabBufferIndex], Imagearr);
                CAM0_MilGrabBMPList[CAM0_MilGrabBufferIndex] = ConvertBitmap(Imagearr, CAM0_Width, CAM0_Height, 1);
                if (CAM0_Grab_Completed != null)
                {
                    CAM0_Grab_Completed(this, CAM0_Grab_Event);
                }
                CAM0_Grabbing = false;
            }
            catch
            { }
        }


        public void CAM1_Mil_Grab()
        {
            try
            {
                if (!CAM1_Initialized || LVApp.Instance().m_mainform.Force_close || CAM1_Grabbing)
                {
                    return;
                }
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (!LVApp.Instance().m_mainform.t_cam_setting_view_mode)
                    {
                        return;
                    }
                }
                CAM1_Grabbing = true;
                int Cam_Num = 1;
                CAM1_MilGrabBufferIndex++;
                CAM1_MilGrabBufferIndex %= BUFFERING_SIZE_MAX;

                MIL.MdigGrab(MilDigitizer[Cam_Num], CAM1_MilGrabBufferList[CAM1_MilGrabBufferIndex]);
                CAM1_MilGrabCount++;

                // Stop the processing.
                //MIL.MdigProcess(MilDigitizer, MilGrabBufferList, 1, MIL.M_STOP, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));

                byte[] Imagearr = new byte[CAM1_Width * CAM1_Height];
                MIL.MbufGet(CAM1_MilGrabBufferList[CAM1_MilGrabBufferIndex], Imagearr);
                CAM1_MilGrabBMPList[CAM1_MilGrabBufferIndex] = ConvertBitmap(Imagearr, CAM1_Width, CAM1_Height, 1);
                if (CAM1_Grab_Completed != null)
                {
                    CAM1_Grab_Completed(this, CAM1_Grab_Event);
                }
                CAM1_Grabbing = false;
                //// 임시 이미지 저장 해보기
                //string FileName = MilGrabBufferIndex.ToString("00") + ".jpg";
                //MIL.MbufExport(FileName, MIL.M_JPEG_LOSSY, MilGrabBufferList[MilGrabBufferIndex]);
            }
            catch
            { }
        }

        public void CAM2_Mil_Grab()
        {
            try
            {
                if (!CAM2_Initialized || LVApp.Instance().m_mainform.Force_close || CAM2_Grabbing)
                {
                    return;
                }
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (!LVApp.Instance().m_mainform.t_cam_setting_view_mode)
                    {
                        return;
                    }
                }
                CAM2_Grabbing = true;
                int Cam_Num = 2;
                CAM2_MilGrabBufferIndex++;
                CAM2_MilGrabBufferIndex %= BUFFERING_SIZE_MAX;

                MIL.MdigGrab(MilDigitizer[Cam_Num], CAM2_MilGrabBufferList[CAM2_MilGrabBufferIndex]);
                CAM2_MilGrabCount++;

                // Stop the processing.
                //MIL.MdigProcess(MilDigitizer, MilGrabBufferList, 1, MIL.M_STOP, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));

                byte[] Imagearr = new byte[CAM2_Width * CAM2_Height];
                MIL.MbufGet(CAM2_MilGrabBufferList[CAM2_MilGrabBufferIndex], Imagearr);
                CAM2_MilGrabBMPList[CAM2_MilGrabBufferIndex] = ConvertBitmap(Imagearr, CAM2_Width, CAM2_Height, 1);
                if (CAM2_Grab_Completed != null)
                {
                    CAM2_Grab_Completed(this, CAM2_Grab_Event);
                }
                CAM2_Grabbing = false;
            }
            catch
            { }
        }

        public void CAM3_Mil_Grab()
        {
            try
            {
                if (!CAM3_Initialized || LVApp.Instance().m_mainform.Force_close || CAM3_Grabbing)
                {
                    return;
                }
                if (!LVApp.Instance().m_Config.m_Check_Inspection_Mode)
                {
                    if (!LVApp.Instance().m_mainform.t_cam_setting_view_mode)
                    {
                        return;
                    }
                }
                CAM3_Grabbing = true;
                int Cam_Num = 3;
                CAM3_MilGrabBufferIndex++;
                CAM3_MilGrabBufferIndex %= BUFFERING_SIZE_MAX;
                CAM3_MilGrabCount++;

                MIL.MdigGrab(MilDigitizer[Cam_Num], CAM3_MilGrabBufferList[CAM3_MilGrabBufferIndex]);

                // Stop the processing.
                //MIL.MdigProcess(MilDigitizer, MilGrabBufferList, 1, MIL.M_STOP, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));

                byte[] Imagearr = new byte[CAM3_Width * CAM3_Height];
                MIL.MbufGet(CAM3_MilGrabBufferList[CAM3_MilGrabBufferIndex], Imagearr);
                CAM3_MilGrabBMPList[CAM3_MilGrabBufferIndex] = ConvertBitmap(Imagearr, CAM3_Width, CAM3_Height, 1);
                if (CAM3_Grab_Completed != null)
                {
                    CAM3_Grab_Completed(this, CAM3_Grab_Event);
                }
                CAM3_Grabbing = false;
            }
            catch
            { }
        }

        static MIL_INT CAM0_ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
        {
            MIL_ID ModifiedBufferId = MIL.M_NULL;

            // this is how to check if the user data is null, the IntPtr class
            // contains a member, Zero, which exists solely for this purpose
            if (!IntPtr.Zero.Equals(HookDataPtr))
            {
                // get the handle to the DigHookUserData object back from the IntPtr
                GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);

                // get a reference to the DigHookUserData object
                HookDataStruct UserData = hUserData.Target as HookDataStruct;

                // Retrieve the MIL_ID of the grabbed buffer.
                MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);

                // Increment the frame counter.
                UserData.ProcessedImageCount++;

                // Print and draw the frame count (remove to reduce CPU usage).
                //Console.Write("Processing frame #{0}.\r", UserData.ProcessedImageCount);
                MIL.MgraText(MIL.M_DEFAULT, ModifiedBufferId, 20, 20, String.Format("{0}", UserData.ProcessedImageCount));

                // 임시 이미지 저장 해보기
                string FileName = UserData.ProcessedImageCount.ToString("00") + "_Processed.jpg";
                MIL.MbufExport(FileName, MIL.M_JPEG_LOSSY, ModifiedBufferId);

                // Execute the processing and update the display.
                //MIL.MimArith(ModifiedBufferId, MIL.M_NULL, UserData.MilImageDisp, MIL.M_NOT);
            }

            return 0;
        }

        static MIL_INT CAM1_ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
        {
            MIL_ID ModifiedBufferId = MIL.M_NULL;

            // this is how to check if the user data is null, the IntPtr class
            // contains a member, Zero, which exists solely for this purpose
            if (!IntPtr.Zero.Equals(HookDataPtr))
            {
                // get the handle to the DigHookUserData object back from the IntPtr
                GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);

                // get a reference to the DigHookUserData object
                HookDataStruct UserData = hUserData.Target as HookDataStruct;

                // Retrieve the MIL_ID of the grabbed buffer.
                MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);

                // Increment the frame counter.
                UserData.ProcessedImageCount++;

                // Print and draw the frame count (remove to reduce CPU usage).
                //Console.Write("Processing frame #{0}.\r", UserData.ProcessedImageCount);
                MIL.MgraText(MIL.M_DEFAULT, ModifiedBufferId, 20, 20, String.Format("{0}", UserData.ProcessedImageCount));

                // 임시 이미지 저장 해보기
                string FileName = UserData.ProcessedImageCount.ToString("00") + "_Processed.jpg";
                MIL.MbufExport(FileName, MIL.M_JPEG_LOSSY, ModifiedBufferId);

                // Execute the processing and update the display.
                //MIL.MimArith(ModifiedBufferId, MIL.M_NULL, UserData.MilImageDisp, MIL.M_NOT);
            }

            return 0;
        }

        static MIL_INT CAM2_ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
        {
            MIL_ID ModifiedBufferId = MIL.M_NULL;

            // this is how to check if the user data is null, the IntPtr class
            // contains a member, Zero, which exists solely for this purpose
            if (!IntPtr.Zero.Equals(HookDataPtr))
            {
                // get the handle to the DigHookUserData object back from the IntPtr
                GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);

                // get a reference to the DigHookUserData object
                HookDataStruct UserData = hUserData.Target as HookDataStruct;

                // Retrieve the MIL_ID of the grabbed buffer.
                MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);

                // Increment the frame counter.
                UserData.ProcessedImageCount++;

                // Print and draw the frame count (remove to reduce CPU usage).
                //Console.Write("Processing frame #{0}.\r", UserData.ProcessedImageCount);
                MIL.MgraText(MIL.M_DEFAULT, ModifiedBufferId, 20, 20, String.Format("{0}", UserData.ProcessedImageCount));

                // 임시 이미지 저장 해보기
                string FileName = UserData.ProcessedImageCount.ToString("00") + "_Processed.jpg";
                MIL.MbufExport(FileName, MIL.M_JPEG_LOSSY, ModifiedBufferId);

                // Execute the processing and update the display.
                //MIL.MimArith(ModifiedBufferId, MIL.M_NULL, UserData.MilImageDisp, MIL.M_NOT);
            }

            return 0;
        }

        static MIL_INT CAM3_ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
        {
            MIL_ID ModifiedBufferId = MIL.M_NULL;

            // this is how to check if the user data is null, the IntPtr class
            // contains a member, Zero, which exists solely for this purpose
            if (!IntPtr.Zero.Equals(HookDataPtr))
            {
                // get the handle to the DigHookUserData object back from the IntPtr
                GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);

                // get a reference to the DigHookUserData object
                HookDataStruct UserData = hUserData.Target as HookDataStruct;

                // Retrieve the MIL_ID of the grabbed buffer.
                MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);

                // Increment the frame counter.
                UserData.ProcessedImageCount++;

                // Print and draw the frame count (remove to reduce CPU usage).
                //Console.Write("Processing frame #{0}.\r", UserData.ProcessedImageCount);
                MIL.MgraText(MIL.M_DEFAULT, ModifiedBufferId, 20, 20, String.Format("{0}", UserData.ProcessedImageCount));

                // 임시 이미지 저장 해보기
                string FileName = UserData.ProcessedImageCount.ToString("00") + "_Processed.jpg";
                MIL.MbufExport(FileName, MIL.M_JPEG_LOSSY, ModifiedBufferId);

                // Execute the processing and update the display.
                //MIL.MimArith(ModifiedBufferId, MIL.M_NULL, UserData.MilImageDisp, MIL.M_NOT);
            }

            return 0;
        }

        public void MIL_Release(int Cam_Num)
        {
            try
            {
                //ALLOC 해제, 해제는 역순으로
                if (Cam_Num == 0 && CAM0_Initialized)
                {
                    CAM0_Initialized = false;
                    //CAM0_hUserData.Free();
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        MIL.MbufFree(CAM0_MilGrabBufferList[i]);
                        if (CAM0_MilGrabBMPList[i] != null)
                        {
                            CAM0_MilGrabBMPList[i].Dispose();
                        }
                    }
                }
                else if (Cam_Num == 1 && CAM1_Initialized)
                {
                    CAM1_Initialized = false;
                    //CAM1_hUserData.Free();
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        MIL.MbufFree(CAM1_MilGrabBufferList[i]);
                        if (CAM1_MilGrabBMPList[i] != null)
                        {
                            CAM1_MilGrabBMPList[i].Dispose();
                        }
                    }
                }
                else if (Cam_Num == 2 && CAM2_Initialized)
                {
                    CAM2_Initialized = false;
                    //CAM2_hUserData.Free();
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        MIL.MbufFree(CAM2_MilGrabBufferList[i]);
                        if (CAM2_MilGrabBMPList[i] != null)
                        {
                            CAM2_MilGrabBMPList[i].Dispose();
                        }
                    }
                }
                else if (Cam_Num == 3 && CAM3_Initialized)
                {
                    CAM3_Initialized = false;
                    //CAM3_hUserData.Free();
                    for (int i = 0; i < BUFFERING_SIZE_MAX; i++)
                    {
                        MIL.MbufFree(CAM3_MilGrabBufferList[i]);
                        if (CAM3_MilGrabBMPList[i] != null)
                        {
                            CAM3_MilGrabBMPList[i].Dispose();
                        }
                    }
                }
                if (MilDisplay[Cam_Num] != MIL.M_NULL)
                {
                    MIL.MdispFree(MilDisplay[Cam_Num]);
                    MilDisplay[Cam_Num] = MIL.M_NULL;
                }

                if (MilDigitizer[Cam_Num] != MIL.M_NULL)
                {
                    MIL.MdigFree(MilDigitizer[Cam_Num]);
                    MilDigitizer[Cam_Num] = MIL.M_NULL;
                }
            }
            catch
            { }
        }

        public void Closing_Release()
        {
            try
            {
                if (MilSystem[0] != MIL.M_NULL)
                {
                    MIL.MsysFree(MilSystem[0]);
                    MilSystem[0] = MIL.M_NULL;
                }
                if (MilSystem[1] != MIL.M_NULL)
                {
                    MIL.MsysFree(MilSystem[1]);
                    MilSystem[1] = MIL.M_NULL;
                }
                MIL.MappFree(MilApplication);
            }
            catch
            { }
        }

        public Bitmap ConvertBitmap(byte[] frame, int width, int height, int ch)
        {
            try
            {
                if (frame == null || frame.Length == 0)
                {
                    return null;
                }
                if (ch == 3)
                {
                    Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                    System.Runtime.InteropServices.Marshal.Copy(frame, 0, data.Scan0, frame.Length);

                    bmp.UnlockBits(data);

                    return bmp;
                }
                else
                {
                    Bitmap res = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                    BitmapData data
                   = res.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                    IntPtr ptr = data.Scan0;
                    Marshal.Copy(frame, 0, ptr, width * height);
                    res.UnlockBits(data);
                    ColorPalette cp = res.Palette;
                    for (int i = 0; i < 256; i++)
                    {
                        cp.Entries[i] = Color.FromArgb(i, i, i);
                    }

                    res.Palette = cp;
                    return res;
                }
            }
            catch
            {
            }
            return new Bitmap(640, 480);
        }
    }
}
