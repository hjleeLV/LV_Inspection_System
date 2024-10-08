// 기본 DLL 파일입니다.

#include "stdafx.h"
#include "ImPro_Library_Clr.h"

namespace ImPro_Library_Clr
{
	ClassClr::ClassClr()
	{
		m_ImPro = new CImPro_Library();
	}

	ClassClr::~ClassClr()
	{
	}

	bool ClassClr::EasyFind_Check()
	{
		return m_ImPro->EasyFind_Check();
	}

	void ClassClr::Set_SSFSAVEFOLDER(System::String^ Save_Folder, bool SSF_Image_Save, int SSF_Save_Format, int Cam_num)
	{
		m_ImPro->SSF_Save_Folder = Save_Folder;
		m_ImPro->SSF_Image_Save[Cam_num] = SSF_Image_Save;
		m_ImPro->SSF_Save_Format = SSF_Save_Format;
	}

	void ClassClr::Set_ModelName(System::String^ Model_Name, System::String^ Model_Save_Folder, bool AI_Image_Save)
	{
		m_ImPro->Model_Name = Model_Name;
		m_ImPro->Model_Save_Folder = Model_Save_Folder;
		m_ImPro->AI_Image_Save = AI_Image_Save;
	}


	void ClassClr::Set_Image(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num)
	{
		try
		{
			pin_ptr<System::Byte> p = &Src[0];
			unsigned char* pby = p;

			m_ImPro->Set_Image(pby,size_x,size_y,channel,Cam_num);
		}
		catch (CMemoryException* e)
		{
		}
		catch (CFileException* e)
		{
		}
		catch (CException* e)
		{
		}
	}

		bool ClassClr::Get_Image_Gray([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num)
	{
		if (m_ImPro->Gray_Img[Cam_num].empty())// || m_ImPro->Alg_Run_Check[Cam_num])
		{
			return false;
		}
		Width = (int)m_ImPro->Gray_Img[Cam_num].cols;
		Height = (int)m_ImPro->Gray_Img[Cam_num].rows;
		Ch = (int)m_ImPro->Gray_Img[Cam_num].channels();

		int length = Width*Height*Ch;
		cli::array<Byte>^ nums = gcnew cli::array<Byte>(length);
		
		System::Runtime::InteropServices::Marshal::Copy((IntPtr)m_ImPro->Gray_Img[Cam_num].data, nums, 0, length);

		//for(int i=0; i<length; i++)
		//{
		//	nums[i] = (Byte)m_ImPro->Dst_Img[Cam_num].data[i];
		//}

		Dst = nums;delete nums;

		return true;
	}

	void ClassClr::Set_Image_0(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num)
	{
		try
		{
			pin_ptr<System::Byte> p = &Src[0];
			unsigned char* pby = p;

			m_ImPro->Set_Image_0(pby,size_x,size_y,channel,Cam_num);
		}
		catch (CMemoryException* e)
		{
		}
		catch (CFileException* e)
		{
		}
		catch (CException* e)
		{
		}
	}


	bool ClassClr::Get_Image0([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num)
	{
		if (m_ImPro->Dst_Img[Cam_num].empty())// || m_ImPro->Alg_Run_Check[Cam_num])
		{
			return false;
		}
		Width = (int)m_ImPro->Dst_Img[Cam_num].cols;
		Height = (int)m_ImPro->Dst_Img[Cam_num].rows;
		Ch = (int)m_ImPro->Dst_Img[Cam_num].channels();

		int length = Width*Height*Ch;
		cli::array<Byte>^ nums = gcnew cli::array<Byte>(length);
		
		System::Runtime::InteropServices::Marshal::Copy((IntPtr)m_ImPro->Dst_Img[Cam_num].data, nums, 0, length);

		//for(int i=0; i<length; i++)
		//{
		//	nums[i] = (Byte)m_ImPro->Dst_Img[Cam_num].data[i];
		//}

		Dst = nums;delete nums;

		return true;
	}


void ClassClr::Set_Image_1(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num)
	{
		try
		{
			pin_ptr<System::Byte> p = &Src[0];
			unsigned char* pby = p;

			m_ImPro->Set_Image_1(pby,size_x,size_y,channel,Cam_num);
		}
		catch (CMemoryException* e)
		{
		}
		catch (CFileException* e)
		{
		}
		catch (CException* e)
		{
		}
	}


	bool ClassClr::Get_Image1([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num)
	{
		if (m_ImPro->Dst_Img[Cam_num].empty())// || m_ImPro->Alg_Run_Check[Cam_num])
		{
			return false;
		}
		Width = (int)m_ImPro->Dst_Img[Cam_num].cols;
		Height = (int)m_ImPro->Dst_Img[Cam_num].rows;
		Ch = (int)m_ImPro->Dst_Img[Cam_num].channels();

		int length = Width*Height*Ch;
		cli::array<Byte>^ nums = gcnew cli::array<Byte>(length);

		System::Runtime::InteropServices::Marshal::Copy((IntPtr)m_ImPro->Dst_Img[Cam_num].data, nums, 0, length);
		//for(int i=0; i<length; i++)
		//{
		//	nums[i] = (Byte)m_ImPro->Dst_Img[Cam_num].data[i];
		//}

		Dst = nums;delete nums;

		return true;
	}


	void ClassClr::Set_Image_2(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num)
	{
		try
		{
			pin_ptr<System::Byte> p = &Src[0];
			unsigned char* pby = p;

			m_ImPro->Set_Image_2(pby,size_x,size_y,channel,Cam_num);
		}
		catch (CMemoryException* e)
		{
		}
		catch (CFileException* e)
		{
		}
		catch (CException* e)
		{
		}
	}


	bool ClassClr::Get_Image2([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num)
	{
		if (m_ImPro->Dst_Img[Cam_num].empty())// || m_ImPro->Alg_Run_Check[Cam_num])
		{
			return false;
		}
		Width = (int)m_ImPro->Dst_Img[Cam_num].cols;
		Height = (int)m_ImPro->Dst_Img[Cam_num].rows;
		Ch = (int)m_ImPro->Dst_Img[Cam_num].channels();

		int length = Width*Height*Ch;
		cli::array<Byte>^ nums = gcnew cli::array<Byte>(length);

		System::Runtime::InteropServices::Marshal::Copy((IntPtr)m_ImPro->Dst_Img[Cam_num].data, nums, 0, length);
		//for(int i=0; i<length; i++)
		//{
		//	nums[i] = (Byte)m_ImPro->Dst_Img[Cam_num].data[i];
		//}

		Dst = nums;delete nums;

		return true;
	}


void ClassClr::Set_Image_3(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num)
	{
		try
		{
			pin_ptr<System::Byte> p = &Src[0];
			unsigned char* pby = p;

			m_ImPro->Set_Image_3(pby,size_x,size_y,channel,Cam_num);
		}
		catch (CMemoryException* e)
		{
		}
		catch (CFileException* e)
		{
		}
		catch (CException* e)
		{
		}
	}


	bool ClassClr::Get_Image3([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num)
	{
		if (m_ImPro->Dst_Img[Cam_num].empty())// || m_ImPro->Alg_Run_Check[Cam_num])
		{
			return false;
		}
		Width = (int)m_ImPro->Dst_Img[Cam_num].cols;
		Height = (int)m_ImPro->Dst_Img[Cam_num].rows;
		Ch = (int)m_ImPro->Dst_Img[Cam_num].channels();

		int length = Width*Height*Ch;
		cli::array<Byte>^ nums = gcnew cli::array<Byte>(length);
		System::Runtime::InteropServices::Marshal::Copy((IntPtr)m_ImPro->Dst_Img[Cam_num].data, nums, 0, length);
		//for(int i=0; i<length; i++)
		//{
		//	nums[i] = (Byte)m_ImPro->Dst_Img[Cam_num].data[i];
		//}

		Dst = nums;delete nums;

		return true;
	}

	void ClassClr::Set_Mask_Image(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num, int ROI_Idx)
	{
		try
		{
			if (size_x <= 0 || size_y <= 0)
			{
				unsigned char* t_pby;
				m_ImPro->Set_Mask_Image(t_pby, 0, 0, channel, Cam_num, ROI_Idx);
				return;
			}

			//if (size_x >= m_ImPro->BOLT_Param[Cam_num].nRect[ROI_Idx].width && size_y >= m_ImPro->BOLT_Param[Cam_num].nRect[ROI_Idx].height)
			{
				pin_ptr<System::Byte> p = &Src[0];
				unsigned char* pby = p;
				m_ImPro->Set_Mask_Image(pby, size_x, size_y, channel, Cam_num, ROI_Idx);
			}
			//else
			//{
			//	unsigned char* t_pby;
			//	m_ImPro->Set_Mask_Image(t_pby, 0, 0, channel, Cam_num, ROI_Idx);
			//	return;
			//}
		}
		catch (CMemoryException* e)
		{
		}
		catch (CFileException* e)
		{
		}
		catch (CException* e)
		{
		}
	}

	void ClassClr::Reset_Dst_Image(int Cam_num)
	{
		m_ImPro->Reset_Dst_Image(Cam_num);
	}

	void ClassClr::ROI_Object_Find([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num)
	{
		if (m_ImPro->Dst_Img[Cam_num].empty())
		{
			return;
		}
		m_ImPro->ROI_Object_Find(Cam_num);

		Width = (int)m_ImPro->Dst_Img[Cam_num].cols;
		Height = (int)m_ImPro->Dst_Img[Cam_num].rows;
		Ch = (int)m_ImPro->Dst_Img[Cam_num].channels();

		int length = Width*Height*Ch;
		cli::array<Byte>^ nums = gcnew cli::array<Byte>(length);

		System::Runtime::InteropServices::Marshal::Copy((IntPtr)m_ImPro->Dst_Img[Cam_num].data, nums, 0, length);
		//for(int i=0; i<length; i++)
		//{
		//	nums[i] = (Byte)m_ImPro->Dst_Img[Cam_num].data[i];
		//}

		Dst = nums;delete nums;
	}

	System::String^ ClassClr::RUN_Algorithm(int Cam_num)
	{
		try
		{
			if (m_ImPro->RUN_Algorithm_CAM(Cam_num))
			{
				System::String^ str_result = gcnew System::String(m_ImPro->Result_Info[Cam_num]);
				//m_ImPro->Alg_Run_Check[Cam_num] = false;
				return str_result;
			}
			//m_ImPro->Alg_Run_Check[Cam_num] = false;
			return "";
		}
		catch (CMemoryException* e)
		{
			return "";
		}
		catch (CFileException* e)
		{
			return "";
		}
		catch (CException* e)
		{
			return "";
		}
		//try
		//{
		return "";
	}


	System::String^ ClassClr::RUN_MissedAlgorithm(int Cam_num)
	{
		try
		{
			//if (Cam_num == 0)
			//{
			//	if (m_ImPro->RUN_MissedAlgorithm_BOLT(Cam_num))
			//	{
			//		System::String^ str_result = gcnew System::String(m_ImPro->Result_MissedInfo[Cam_num]);
			//		return str_result;
			//	}
			//} 
			return "";
		}
		catch (CMemoryException* e)
		{
			return "";
		}
		catch (CFileException* e)
		{
			return "";
		}
		catch (CException* e)
		{
			return "";
		}
		//try
		//{
		return "";
	}

	void ClassClr::Set_Resolution(double Resolution_X, double Resolution_Y, int Cam_num)
	{
		//if (m_ImPro->m_Alg_Type <= 2)
		{//SIDE
			m_ImPro->BOLT_Param[Cam_num].nResolution[0] = Resolution_X;
			m_ImPro->BOLT_Param[Cam_num].nResolution[1] = Resolution_Y;
		}
	}

	void ClassClr::Set_CAM_Offset(int Cam_Num, double P1,double P2,double P3,double P4,double P5,double P6,double P7,double P8,double P9, double P10
		,double P11,double P12,double P13,double P14,double P15,double P16,double P17,double P18,double P19,double P20
		,double P21,double P22,double P23,double P24,double P25,double P26,double P27,double P28,double P29,double P30
		,double P31,double P32,double P33,double P34,double P35,double P36,double P37,double P38,double P39,double P40)
	{
			m_ImPro->BOLT_Param[Cam_Num].Offset[1] = P1;
			m_ImPro->BOLT_Param[Cam_Num].Offset[2] = P2;
			m_ImPro->BOLT_Param[Cam_Num].Offset[3] = P3;
			m_ImPro->BOLT_Param[Cam_Num].Offset[4] = P4;
			m_ImPro->BOLT_Param[Cam_Num].Offset[5] = P5;
			m_ImPro->BOLT_Param[Cam_Num].Offset[6] = P6;
			m_ImPro->BOLT_Param[Cam_Num].Offset[7] = P7;
			m_ImPro->BOLT_Param[Cam_Num].Offset[8] = P8;
			m_ImPro->BOLT_Param[Cam_Num].Offset[9] = P9;
			m_ImPro->BOLT_Param[Cam_Num].Offset[10] = P10;
			m_ImPro->BOLT_Param[Cam_Num].Offset[11] = P11;
			m_ImPro->BOLT_Param[Cam_Num].Offset[12] = P12;
			m_ImPro->BOLT_Param[Cam_Num].Offset[13] = P13;
			m_ImPro->BOLT_Param[Cam_Num].Offset[14] = P14;
			m_ImPro->BOLT_Param[Cam_Num].Offset[15] = P15;
			m_ImPro->BOLT_Param[Cam_Num].Offset[16] = P16;
			m_ImPro->BOLT_Param[Cam_Num].Offset[17] = P17;
			m_ImPro->BOLT_Param[Cam_Num].Offset[18] = P18;
			m_ImPro->BOLT_Param[Cam_Num].Offset[19] = P19;
			m_ImPro->BOLT_Param[Cam_Num].Offset[20] = P20;
			m_ImPro->BOLT_Param[Cam_Num].Offset[21] = P21;
			m_ImPro->BOLT_Param[Cam_Num].Offset[22] = P22;
			m_ImPro->BOLT_Param[Cam_Num].Offset[23] = P23;
			m_ImPro->BOLT_Param[Cam_Num].Offset[24] = P24;
			m_ImPro->BOLT_Param[Cam_Num].Offset[25] = P25;
			m_ImPro->BOLT_Param[Cam_Num].Offset[26] = P26;
			m_ImPro->BOLT_Param[Cam_Num].Offset[27] = P27;
			m_ImPro->BOLT_Param[Cam_Num].Offset[28] = P28;
			m_ImPro->BOLT_Param[Cam_Num].Offset[29] = P29;
			m_ImPro->BOLT_Param[Cam_Num].Offset[30] = P30;
			m_ImPro->BOLT_Param[Cam_Num].Offset[31] = P31;
			m_ImPro->BOLT_Param[Cam_Num].Offset[32] = P32;
			m_ImPro->BOLT_Param[Cam_Num].Offset[33] = P33;
			m_ImPro->BOLT_Param[Cam_Num].Offset[34] = P34;
			m_ImPro->BOLT_Param[Cam_Num].Offset[35] = P35;
			m_ImPro->BOLT_Param[Cam_Num].Offset[36] = P36;
			m_ImPro->BOLT_Param[Cam_Num].Offset[37] = P37;
			m_ImPro->BOLT_Param[Cam_Num].Offset[38] = P38;
			m_ImPro->BOLT_Param[Cam_Num].Offset[39] = P39;
			m_ImPro->BOLT_Param[Cam_Num].Offset[40] = P40;
}

	void ClassClr::Set_CAM_Parameters(int ROI_Num, int Cam_Num, int P_nTableType,int P_nCamPosition,double P5,double P6,double P7,double P8,double P9,double P10,double P11,double P12,double P13,double P14,double P15,double P16,double P17,double P18,double P19,double P20,double P21,double P22,double P23,double P24,double P25)
	{
		// nTableType // Table Type 0 : 인덱스, 1 : 유리판
		// nCamPosition // 카메라 위치 0 : TOP, BOTTOM, 1 : SIDE
		m_ImPro->BOLT_Param[Cam_Num].nTableType = P_nTableType;
		m_ImPro->BOLT_Param[Cam_Num].nCamPosition = P_nCamPosition;

		if (m_ImPro->BOLT_Param[Cam_Num].nCamPosition == 0)
		{//TOP, BOTTOM
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Thres[ROI_Num] = (int)P5;
			m_ImPro->BOLT_Param[Cam_Num].nThres_V1[ROI_Num] = P6;
			m_ImPro->BOLT_Param[Cam_Num].nThres_V2[ROI_Num] = P7;
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Direc[ROI_Num] = (int)P8;
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Cal[ROI_Num] = (int)P9;
			m_ImPro->BOLT_Param[Cam_Num].nCalmin[ROI_Num] = P10;
			m_ImPro->BOLT_Param[Cam_Num].nCalmax[ROI_Num] = P11;

			if (ROI_Num == 0)
			{
				if (P5 == 7)
				{
					m_ImPro->BOLT_Param[Cam_Num].nFindAngleTolerance[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nFindScaleTolerance[ROI_Num] = P13 / 100.0f;
					m_ImPro->BOLT_Param[Cam_Num].nFindFindExtension[ROI_Num] = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nFindTopMargin[ROI_Num] = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nFindBottomMargin[ROI_Num] = (int)P14;
					float t_f = P15;
					if (t_f > 1)
					{
						t_f = 0;
					}
					else if (t_f < -1)
					{
						t_f = 0;
					}
					m_ImPro->BOLT_Param[Cam_Num].nFindLightBalance[ROI_Num] = t_f;
					m_ImPro->BOLT_Param[Cam_Num].nFindPostProcessing[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nFindScore[ROI_Num] = P16;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_Rotation_Method[ROI_Num] = (int)P17;
					//m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P18;
				}
				else
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterDirection[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nFindPostProcessing[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_Rotation_Method[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_MergeFilterSize[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOBSelect[ROI_Num] = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_ColorPreprocessing[ROI_Num] = (int)P20;
				}

			}
			else
			{
				if (P8 == 0 || P8 == 1)
				{// 가로 세로 길이
					m_ImPro->BOLT_Param[Cam_Num].nCalAngle[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nCalMinDist[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nCalMaxDist[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nCalPreprocessing[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nCalCalMethod[ROI_Num] = (int)P17;
				}
				if (P8 == 6 || P8 == 7)
				{ // 사각
					m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P12;
					//m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;

					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterUsage[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilter[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterCNT[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterDarkThres[ROI_Num] = P6;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterBrightThres[ROI_Num] = P7;
					m_ImPro->BOLT_Param[Cam_Num].nErodeFilterCNT[ROI_Num] = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nDilateFilterCNT[ROI_Num] = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nMPMethod[ROI_Num] = (int)P20;
					
					m_ImPro->BOLT_Param[Cam_Num].nConvexBLOBOption[ROI_Num] = (int)P21;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P22;
				}
				if (P8 == 2)//십자, 6각
				{
					m_ImPro->BOLT_Param[Cam_Num].nCrossSizeMethod[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nCrossAngleNumber[ROI_Num] = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nCrossOutput[ROI_Num] = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nCrossMethod[ROI_Num] = (int)P15;

				}
				if (P8 == 3)//직경
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nDiameter_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nDiameter_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nDiameter_Method[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nDiameter_Direction[ROI_Num] = (int)P16;
				}
				if (P8 == 4)//색상
				{
					m_ImPro->BOLT_Param[Cam_Num].nColorMethod[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nColorMinThres[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nColorMaxThres[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P16;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P17;
					m_ImPro->BOLT_Param[Cam_Num].nColorOutput[ROI_Num] = (int)P18;
					if (P12 == 0)
					{
						m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P13;
					}
					else if (P12 == 1)
					{
						m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P19;
					}
				}
				if (P8 == 5)//원형영역의 밝기
				{
					m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num] = (int)(0.5 * P12 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
					m_ImPro->BOLT_Param[Cam_Num].nCircle1Thickness[ROI_Num] = (int)(P13 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
					if (m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num] - m_ImPro->BOLT_Param[Cam_Num].nCircle1Thickness[ROI_Num] / 2 < 0)
					{
						m_ImPro->BOLT_Param[Cam_Num].nCircle1Thickness[ROI_Num] = 2 * m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num];
					}

					m_ImPro->BOLT_Param[Cam_Num].nCircle2Radius[ROI_Num] = (int)(0.5 * P14 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
					m_ImPro->BOLT_Param[Cam_Num].nCircle2Thickness[ROI_Num] = (int)(P15 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
					if (m_ImPro->BOLT_Param[Cam_Num].nCircle2Radius[ROI_Num] - m_ImPro->BOLT_Param[Cam_Num].nCircle2Thickness[ROI_Num] / 2 < 0)
					{
						m_ImPro->BOLT_Param[Cam_Num].nCircle2Thickness[ROI_Num] = 2 * m_ImPro->BOLT_Param[Cam_Num].nCircle2Radius[ROI_Num];
					}
					m_ImPro->BOLT_Param[Cam_Num].nColorMethod[ROI_Num] = (int)P16;
				}
				if (P8 == 8 || P8 == 9)
				{
					m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num] = (int)(0.5 * P12 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
					m_ImPro->BOLT_Param[Cam_Num].nCircle1Thickness[ROI_Num] = (int)(P13 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
					if (m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num] - m_ImPro->BOLT_Param[Cam_Num].nCircle1Thickness[ROI_Num] / 2 < 0)
					{
						m_ImPro->BOLT_Param[Cam_Num].nCircle1Thickness[ROI_Num] = 2 * m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num];
					}

					m_ImPro->BOLT_Param[Cam_Num].nCircle2Radius[ROI_Num] = 0;// 안씀
					m_ImPro->BOLT_Param[Cam_Num].nCircle2Thickness[ROI_Num] = 0;// 안씀

					m_ImPro->BOLT_Param[Cam_Num].nCircleStartAngle[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nCircleEndAngle[ROI_Num] = P15;

					m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nErodeFilterCNT[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nDilateFilterCNT[ROI_Num] = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nMPMethod[ROI_Num] = (int)P19;

					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P20;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P21;
					m_ImPro->BOLT_Param[Cam_Num].nNumConnectedCircleBLOB[ROI_Num] = 0;// 안씀
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P22;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionThreshold[ROI_Num] = P23;
					m_ImPro->BOLT_Param[Cam_Num].nCircleOutputMethod[ROI_Num] = (int)P24;
					m_ImPro->BOLT_Param[Cam_Num].nConvexBLOBOption[ROI_Num] = 0; // 안씀
				}
				if (P8 == 10)//진원도
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nDiameter_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nDiameter_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nOutput[ROI_Num] = (int)P15;
				}
				if (P8 == 11)
				{
					m_ImPro->BOLT_Param[Cam_Num].nNASAPasteFilterSize[ROI_Num] = (int)P12;
				}
				if (P8 == 12)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI12_Direction[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].x = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].y = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].width = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].height = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].x = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].y = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].width = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].height = (int)P20;
				}
				if (P8 == 13)
				{
					m_ImPro->BOLT_Param[Cam_Num].nAngleHeightFilterSize[ROI_Num] = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nThreadSizeMethod[ROI_Num] = (int)P14;
				}
				if (P8 == 14)
				{
					m_ImPro->BOLT_Param[Cam_Num].nColorBlurFilterCNT[ROI_Num] = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nColorMinThres[ROI_Num] = P15;
					m_ImPro->BOLT_Param[Cam_Num].nColorMaxThres[ROI_Num] = P16;
					//m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P18;
					m_ImPro->BOLT_Param[Cam_Num].nColorOutput[ROI_Num] = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P20;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionThreshold[ROI_Num] = P21;
				}
				if (P8 == 15)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P15;
				}
				if (P8 == 16)
				{//내외경 중심 차이
					m_ImPro->BOLT_Param[Cam_Num].nThres_InnerCircle[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nThresMethod_InnerCircle[ROI_Num] = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_MergeFilterSize[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nOutput[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nCircle1Radius[ROI_Num] = (int)(0.5 * P17 / m_ImPro->BOLT_Param[Cam_Num].nResolution[0]);
				}
				if (P8 == 18)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nOutput[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P17;
				}
				if (P8 == 20)
				{//SSF
					m_ImPro->BOLT_Param[Cam_Num].nSSFXLifting[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nSSFYLifting[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nSSFDefectFilterX[ROI_Num] = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nSSFDefectFilterY[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nSSFBaseFilterX[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nSSFBaseFilterY[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nSSFOpened[ROI_Num] = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nSSFClosed[ROI_Num] = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nSSFGDDark[ROI_Num] = P20;
					m_ImPro->BOLT_Param[Cam_Num].nSSFGDBright[ROI_Num] = P21;
					m_ImPro->BOLT_Param[Cam_Num].nSSFSizeDark[ROI_Num] = P22;
					m_ImPro->BOLT_Param[Cam_Num].nSSFSizeBright[ROI_Num] = P23;
					m_ImPro->BOLT_Param[Cam_Num].nSSFOutput[ROI_Num] = P24;
					m_ImPro->BOLT_Param[Cam_Num].nSSFAIClass[ROI_Num] = (int)P25;
				}
			}
		}
		else if (m_ImPro->BOLT_Param[Cam_Num].nCamPosition == 1)
		{//SIDE
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Thres[ROI_Num] = (int)P5;
			m_ImPro->BOLT_Param[Cam_Num].nThres_V1[ROI_Num] = P6;
			m_ImPro->BOLT_Param[Cam_Num].nThres_V2[ROI_Num] = P7;
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Direc[ROI_Num] = (int)P8;
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Cal[ROI_Num] = (int)P9;
			m_ImPro->BOLT_Param[Cam_Num].nCalmin[ROI_Num] = (int)P10;
			m_ImPro->BOLT_Param[Cam_Num].nCalmax[ROI_Num] = (int)P11;
			m_ImPro->BOLT_Param[Cam_Num].nHeightforShape[ROI_Num] = (int)(P12 / m_ImPro->BOLT_Param[Cam_Num].nResolution[1]);
			m_ImPro->BOLT_Param[Cam_Num].nCalAngle[ROI_Num] = P12;
			m_ImPro->BOLT_Param[Cam_Num].nNASAPasteFilterSize[ROI_Num] = (int)P12;
			m_ImPro->BOLT_Param[Cam_Num].nCalMinDist[ROI_Num] = P13;
			m_ImPro->BOLT_Param[Cam_Num].nCalMaxDist[ROI_Num] = P14;
			m_ImPro->BOLT_Param[Cam_Num].nROI0_Rotation_Method[ROI_Num] = 0;
			if (ROI_Num == 0)
			{
				m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
				m_ImPro->BOLT_Param[Cam_Num].nThickness[ROI_Num] = P12;
				m_ImPro->BOLT_Param[Cam_Num].nAngleHeightTop[ROI_Num] = (int)(P13 / m_ImPro->BOLT_Param[Cam_Num].nResolution[1]);
				m_ImPro->BOLT_Param[Cam_Num].nAngleHeightHeight[ROI_Num] = (int)(P14/ m_ImPro->BOLT_Param[Cam_Num].nResolution[1]);
				m_ImPro->BOLT_Param[Cam_Num].nAngleHeightFilterSize[ROI_Num] = (int)P15;
				m_ImPro->BOLT_Param[Cam_Num].nAngleCalMethod[ROI_Num] = (int)P16;
				m_ImPro->BOLT_Param[Cam_Num].nSRotationCalMethod[ROI_Num] = (int)P17;
				if (P17 > 0)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_Rotation_Method[ROI_Num] = 1;
					m_ImPro->BOLT_Param[Cam_Num].nSRotationTopHeight[ROI_Num] = (int)(P18/ m_ImPro->BOLT_Param[Cam_Num].nResolution[1]);
				}
				else
				{
					m_ImPro->BOLT_Param[Cam_Num].nSRotationTopHeight[ROI_Num] = 0;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_Rotation_Method[ROI_Num] = 0;
				}

				
				if (P_nTableType == 1)
				{// Glass Type
					m_ImPro->BOLT_Param[Cam_Num].nAngleHeightHeight[ROI_Num] = (int)(P15 / m_ImPro->BOLT_Param[Cam_Num].nResolution[1]);
					m_ImPro->BOLT_Param[Cam_Num].nAngleHeightFilterSize[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nAngleCalMethod[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_Rotation_Method[ROI_Num] = 0;
				}
				//m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = (int)P10;
				//m_ImPro->BOLT_Param[Cam_Num].nAngleHeightOffset[ROI_Num] = (int)P11;
			}
			else
			{
				m_ImPro->BOLT_Param[Cam_Num].nCalAngle[ROI_Num] = P12;
				m_ImPro->BOLT_Param[Cam_Num].nCalMinDist[ROI_Num] = P13;
				m_ImPro->BOLT_Param[Cam_Num].nCalMaxDist[ROI_Num] = P14;
								
				m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
				m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
				m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;

				if (P8 == 0 || P8 == 1)
				{// 가로 세로 길이
					m_ImPro->BOLT_Param[Cam_Num].nCalAngle[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nCalMinDist[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nCalMaxDist[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nCalPreprocessing[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nCalCalMethod[ROI_Num] = (int)P17;
				}

				if (P8 == 3)
				{
					m_ImPro->BOLT_Param[Cam_Num].nAngleHeightFilterSize[ROI_Num] = (int)P13;
				} 
				else if (P8 == 8 || P8 == 9)
				{
					m_ImPro->BOLT_Param[Cam_Num].nAngleHeightFilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nBendingOutput[ROI_Num] = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nOutput[ROI_Num] = (int)P14;
				} 
				if (P8 == 5)
				{
					m_ImPro->BOLT_Param[Cam_Num].nAngleHeightFilterSize[ROI_Num] = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nThreadSizeMethod[ROI_Num] = (int)P14;
				}
				if (P8 == 10)//색상
				{
					m_ImPro->BOLT_Param[Cam_Num].nColorMethod[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nColorMinThres[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nColorMaxThres[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P16;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P17;
					m_ImPro->BOLT_Param[Cam_Num].nColorOutput[ROI_Num] = (int)P18;
				}
				if (P8 == 11) // 사각 BLOB
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;

					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterUsage[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilter[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterCNT[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterDarkThres[ROI_Num] = P18;
					m_ImPro->BOLT_Param[Cam_Num].nDirecFilterBrightThres[ROI_Num] = P19;
					m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P20;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P21;
					m_ImPro->BOLT_Param[Cam_Num].nConvexBLOBOption[ROI_Num] = (int)P22;
				}
				if (P8 == 12)
				{
					m_ImPro->BOLT_Param[Cam_Num].nThickness[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nHeightforShape[ROI_Num] = (int)(P13 / m_ImPro->BOLT_Param[Cam_Num].nResolution[1]);
				}
				if (P8 == 13)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI12_Direction[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].x = (int)P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].y = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].width = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nROI1[ROI_Num].height = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].x = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].y = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].width = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nROI2[ROI_Num].height = (int)P20;
				}
				if (P8 == 14)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P15;
				}
				if (P8 == 16)
				{
					m_ImPro->BOLT_Param[Cam_Num].nROI0_FilterSize[ROI_Num] = (int)P12;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Min_Size[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nROI0_BLOB_Max_Size[ROI_Num] = P14;
					m_ImPro->BOLT_Param[Cam_Num].nBlurFilterCNT[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nOutput[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nCirclePositionMethod[ROI_Num] = (int)P17;
				}
				if (P8 == 18)
				{//SSF
					m_ImPro->BOLT_Param[Cam_Num].nSSFXLifting[ROI_Num] = P12;
					m_ImPro->BOLT_Param[Cam_Num].nSSFYLifting[ROI_Num] = P13;
					m_ImPro->BOLT_Param[Cam_Num].nSSFDefectFilterX[ROI_Num] = (int)P14;
					m_ImPro->BOLT_Param[Cam_Num].nSSFDefectFilterY[ROI_Num] = (int)P15;
					m_ImPro->BOLT_Param[Cam_Num].nSSFBaseFilterX[ROI_Num] = (int)P16;
					m_ImPro->BOLT_Param[Cam_Num].nSSFBaseFilterY[ROI_Num] = (int)P17;
					m_ImPro->BOLT_Param[Cam_Num].nSSFOpened[ROI_Num] = (int)P18;
					m_ImPro->BOLT_Param[Cam_Num].nSSFClosed[ROI_Num] = (int)P19;
					m_ImPro->BOLT_Param[Cam_Num].nSSFGDDark[ROI_Num] = P20;
					m_ImPro->BOLT_Param[Cam_Num].nSSFGDBright[ROI_Num] = P21;
					m_ImPro->BOLT_Param[Cam_Num].nSSFSizeDark[ROI_Num] = P22;
					m_ImPro->BOLT_Param[Cam_Num].nSSFSizeBright[ROI_Num] = P23;
					m_ImPro->BOLT_Param[Cam_Num].nSSFOutput[ROI_Num] = P24;
					m_ImPro->BOLT_Param[Cam_Num].nSSFAIClass[ROI_Num] = (int)P25;
				}
			}
		}

		if (P_nTableType == 5 && P12 == 7496 && Cam_Num == 0 && ROI_Num == 0)
		{// 스기야마 알고리즘
			m_ImPro->ALG_SGYM_Param.dDanjaMinArea = P13;
			m_ImPro->ALG_SGYM_Param.dSlitMinArea = P14;
			m_ImPro->ALG_SGYM_Param.dDanjaMinHeight = P15;
			m_ImPro->ALG_SGYM_Param.dDanjaMaxHeight = P16;
			m_ImPro->ALG_SGYM_Param.dDanjaRotationRage = P17;
			m_ImPro->ALG_SGYM_Param.Chip_Threshold = (int)P18;
			m_ImPro->ALG_SGYM_Param.DlDDM_Size_Threshold = (int)P19;
		}
		if (P_nTableType == 5 && Cam_Num == 0)
		{
			m_ImPro->BOLT_Param[Cam_Num].nMethod_Cal[ROI_Num] = (int)P9;
		}
	}

	void ClassClr::Set_Global_Parameters(bool P0, bool P1)
	{
		m_ImPro->Result_Text_View = P0;
		m_ImPro->Result_Debugging = P1;

		if (P0)
		{
			m_ImPro->m_Text_View[0] = 1;
			m_ImPro->m_Text_View[1] = 1;
			m_ImPro->m_Text_View[2] = 1;
			m_ImPro->m_Text_View[3] = 1;
		}
		else
		{
			m_ImPro->m_Text_View[0] = 0;
			m_ImPro->m_Text_View[1] = 0;
			m_ImPro->m_Text_View[2] = 0;
			m_ImPro->m_Text_View[3] = 0;
		}
	}

	void ClassClr::Set_Display_Parameters(bool ROI_Mode, int ROI_Num, int ROI_CAM_Num)
	{
		m_ImPro->ROI_Mode = ROI_Mode;
		m_ImPro->ROI_Num = ROI_Num;
		m_ImPro->ROI_CAM_Num = ROI_CAM_Num;
	}

	void ClassClr::Set_ROI_Parameters(System::String^ str_ROI, int ROI_Num, int Cam_Num, System::String^ str_Ratio)
	{
		//if (m_ImPro->m_Alg_Type <= 2)
		{//SIDE
			cli::array<System::String ^>^ str = str_ROI->Split('_');
			cli::array<System::String ^>^ str_ratio = str_Ratio->Split('_');
			if (str[0] == "O")
			{
				m_ImPro->BOLT_Param[Cam_Num].nUse[ROI_Num] = 1;
			} 
			else
			{
				m_ImPro->BOLT_Param[Cam_Num].nUse[ROI_Num] = 0;
			}
			m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].x = (int)(Convert::ToDouble(str[1])*Convert::ToDouble(str_ratio[0]));
			m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].y = (int)(Convert::ToDouble(str[2])*Convert::ToDouble(str_ratio[1]));
			m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width = (int)(Convert::ToDouble(str[3])*Convert::ToDouble(str_ratio[0]));
			m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height = (int)(Convert::ToDouble(str[4])*Convert::ToDouble(str_ratio[1]));


			if (m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width%4 == 1)
			{
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width -= 1;
			}
			else if (m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width%4 == 2)
			{
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].x += 1;
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width -= 2;
			}
			else if (m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width%4 == 3)
			{
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].x += 1;
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].width -= 3;
			}
			if (m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height%4 == 1)
			{
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height -= 1;
			}
			else if (m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height%4 == 2)
			{
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].y += 1;
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height -= 2;
			}
			else if (m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height%4 == 3)
			{
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].y += 1;
				m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].height -= 3;
			}
			//if (ROI_Num > 0)
			//{
			//	m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].x -= m_ImPro->BOLT_Param[Cam_Num].nRect[0].x;
			//	m_ImPro->BOLT_Param[Cam_Num].nRect[ROI_Num].y -= m_ImPro->BOLT_Param[Cam_Num].nRect[0].y;
			//}
		}
	}

	System::String^ ClassClr::GET_Offset_Object_Location(int Cam_num)
	{
		try
		{
			System::String^ str_result = gcnew System::String(m_ImPro->Offset_Object_Postion[Cam_num]);
			return str_result;
		}
		catch (CMemoryException* e)
		{
			return "0_0";
		}
		catch (CFileException* e)
		{
			return "0_0";
		}
		catch (CException* e)
		{
			return "0_0";
		}
		//try
		//{
		return "0_0";
	}

	bool ClassClr::Get_AI_Model_Loaded(int Cam_num, int ROI_idx)
	{
		return m_ImPro->AI_Model[Cam_num].model_loaded[ROI_idx];
	}

	void ClassClr::Set_AI_Model_Loaded(int Cam_num, int ROI_idx)
	{
		m_ImPro->AI_Model[Cam_num].model_loaded[ROI_idx] = false;
	}

	void ClassClr::Set_AI_Model(int Cam_num, int ROI_idx, System::String^ Model_Name)
	{
		USES_CONVERSION;
		pin_ptr<const wchar_t> wchStr = PtrToStringChars(Model_Name);
		//sprintf(W2A(wchStr));

		m_ImPro->BOLT_Param[Cam_num].AI_model[ROI_idx] = wchStr;
		m_ImPro->AI_Model_Load(Cam_num, ROI_idx);
	}
	void ClassClr::Save_SSF_AI_Image(System::String^ Model_Name, bool Save_flag) // AI 검사
	{
		USES_CONVERSION;
		pin_ptr<const wchar_t> wchStr = PtrToStringChars(Model_Name);
		//sprintf(W2A(wchStr));

		m_ImPro->model_save = wchStr;
		m_ImPro->model_save_flag = Save_flag;
	}
}