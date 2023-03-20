// ImPro_Library_Clr.h
#pragma once

#include "..\ImPro_Library\ImPro_Library.h"

using namespace System;

namespace ImPro_Library_Clr {

	public ref class ClassClr
	{
	public:
		ClassClr();
		virtual ~ClassClr();

	public:
		CImPro_Library* m_ImPro;

		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
		void Set_Image(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num);
		void Set_Image_0(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num);
		bool Get_Image_Gray([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num);
		bool Get_Image0([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num);
		void Set_Image_1(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num);
		bool Get_Image1([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num);
		void Set_Image_2(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num);
		bool Get_Image2([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num);
		void Set_Image_3(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num);
		bool Get_Image3([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num);
		void Set_Mask_Image(cli::array<System::Byte>^ Src, long size_x, long size_y, long channel, int Cam_num, int ROI_Idx);
		void Reset_Dst_Image(int Cam_num);

		System::String^ RUN_Algorithm(int Cam_num);
		System::String^ RUN_MissedAlgorithm(int Cam_num);
		void Set_Resolution(double Resolution_X, double Resolution_Y, int Cam_num);
		void Set_CAM_Offset(int Cam_Num, double P1,double P2,double P3,double P4,double P5,double P6,double P7,double P8,double P9, double P10
		,double P11,double P12,double P13,double P14,double P15,double P16,double P17,double P18,double P19,double P20
		,double P21,double P22,double P23,double P24,double P25,double P26,double P27,double P28,double P29,double P30
		,double P31,double P32,double P33,double P34,double P35,double P36,double P37,double P38,double P39,double P40);
		void Set_CAM_Parameters(int ROI_Num, int Cam_Num, int P_nTableType,int P_nCamPosition,double P5,double P6,double P7,double P8,double P9,double P10,double P11,double P12,double P13,double P14,double P15,double P16,double P17,double P18,double P19,double P20,double P21,double P22,double P23,double P24,double P25);
		void Set_Global_Parameters(bool P0, bool P1);
		void Set_ROI_Parameters(System::String^ str_ROI, int ROI_Num, int Cam_Num,System::String^ str_Ratio);
		void ROI_Object_Find([Runtime::InteropServices::Out] cli::array<Byte>^ %Dst, [Runtime::InteropServices::Out] int %Width, [Runtime::InteropServices::Out] int %Height, [Runtime::InteropServices::Out] int %Ch, int Cam_num);
		void Set_Display_Parameters(bool ROI_Mode, int ROI_Num, int ROI_CAM_Num);

		void Set_ModelName(System::String^ Model_Name, System::String^ Model_Save_Folder, bool AI_Image_Save);

		void Set_SSFSAVEFOLDER(System::String^ Save_Folder, bool SSF_Image_Save, int SSF_Save_Format, int Cam_num);
		System::String^ GET_Offset_Object_Location(int Cam_num);
		bool Get_AI_Model_Loaded(int Cam_num, int ROI_idx); // AI 검사
		void Set_AI_Model_Loaded(int Cam_num, int ROI_idx); // AI 검사
		void Set_AI_Model(int Cam_num, int ROI_idx, System::String^ Model_Name); // AI 검사
		bool EasyFind_Check();
		void Save_SSF_AI_Image(System::String^ Model_Name, bool Save_flag); // AI 검사
	};
}
