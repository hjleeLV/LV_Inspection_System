#pragma once
#include "StdAfx.h"
#include "RANSAC_EllipseFittingAlgorithm.h" //타원 핏팅을 위해 추가

//Measure Mode Define
enum FIND_METHOD
{
	BINARY=0,					//0
	BINARY_INV,					//1
	BINARY_OTSU,				//2
	BINARY_INV_OTSU,			//3
	MATCHING_BINARY,			//4
	MATCHING_BINARY_INV,		//5
	INVARIANT_BINARY,			//6
	INVARIANT_BINARY_INV,		//7
};

enum CALCULATE_METHOD
{
	MIN=0,					//0
	AVERAGE,				//1
	MAX,					//2
	RANGE,					//3
	ALLDATA,				//4
};

class CImPro_Library
{
public:
	CImPro_Library(void);
	~CImPro_Library(void);

public:
	// 알고리즘 전역 변수(0:Head 카메라, 1:Side 카메라, 2:Tab 카메라)
	Mat Src_Img[10];				// 카메라 이미지
	Mat Gray_Img[10];				// 카메라이미지 흑백 변환
	Mat Dst_Img[10];				// 결과 이미지
	CString Result_Info[10];		// 결과 저장
	bool Result_Text_View;			// 결과 Text 볼건지 말건지
	int License_Check();		// 라이센스 체크
	int m_License_Check;		// 라이센스 체크 되었는지? -1이면 체크 안됨.
	void Set_Image_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_1(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_2(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_3(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_4(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_5(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_6(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_7(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Reset_Dst_Image(int Cam_num);	// 결과이미지 리셋

	// 필요 함수 구현
	void J_FastTemplateMatch(Mat& srca,  // The reference image
		Mat& srcb,  // The template image
		Mat& dst   // Template matching result
		);
	void J_Fitting_Ellipse(Mat &Binary,double* Circle_Info);	// 타원 핏팅
	void J_Fill_Hole(Mat &Binary);								// Hole filling
	int J_Delete_Small_Binary(Mat &Binary, int m_small);			// 작은 것 없애기
	void J_Delete_Boundary(Mat &Img, int boundary_width);		// Boundary 삭제
	void J_Rotate(cv::Mat& src, double angle, cv::Mat& dst);	// 회전변환

	// CAM0 외경 알고리즘 관련 변수 및 함수
	struct ALG_C0_ParameterData {
		// Input 변수
		int nBGThreshold;		// 배경 임계값
		int nTopMargin;			// 
		int nTopHeight;			// 
		int nBottomMargin;		// 
		int nBottomHeight;		//
		int nEdgeThreshold;		// 배경 임계값

		// Output 변수
		double oTopCenterX;
		double oTopDistance;
		double oBottomCenterX;
		double oBottomDistance;
		ALG_C0_ParameterData()
		{
			nBGThreshold = 100;
			nTopMargin = 153;
			nTopHeight = 70;
			nBottomMargin = 302;
			nBottomHeight = 100;
			nEdgeThreshold = 200;
		}
	};
	ALG_C0_ParameterData ALG_C0_Param;	// ALG_C0 Parameter Data
	bool RUN_Algorithm_CAM0();
	void Find_Center_Distance_Average(vector<int> Vec_Left, vector<int> Vec_Right, int nMeasureRange_Low,int nMeasureRange_High, int Option); 

	// CAM0 외경 알고리즘 관련 변수 및 함수
	struct ALG_C1_ParameterData {
		// Input 변수
		int nBGThreshold;		// 배경 임계값
		int nInnerThreshold;	// 단차 임계값

		// Output 변수

		ALG_C1_ParameterData()
		{
			nBGThreshold = 200;
			nInnerThreshold = 100;
		}
	};
	ALG_C1_ParameterData ALG_C1_Param;	// ALG_C0 Parameter Data

	bool RUN_Algorithm_CAM1();


	bool RUN_Algorithm_CAM2();
	bool RUN_Algorithm_CAM3();
	bool RUN_Algorithm_CAM4();
	bool RUN_Algorithm_CAM5();
	bool RUN_Algorithm_CAM6();
	bool RUN_Algorithm_CAM7();







	//struct ALG_H5_ParameterData {
	//	vector<Rect> ROI;					// ROI
	//	int nParam_Binary_Thres;			// 임계값
	//	ALG_H5_ParameterData()
	//	{
	//		nParam_Binary_Thres = 50;
	//	}
	//};
	//ALG_H5_ParameterData ALG_H5_Param;	// ALG_H5 Parameter Data
	//void RUN_Algorithm_H5();

	//
	//void Set_Template_Image(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);
	//void Set_Side_Image(unsigned char* src_u8, long size_x, long size_y, long channel);
	//void RUN_Algorithm_S0();



	//int m_HEAD_FIND_METHOD;
	//int H0_L_Threshold;
	//int H0_H_Threshold;
	//int H0_Margin;

	//int m_SIDE_FIND_METHOD;
	//int L0_L_Threshold;
	//int L0_H_Threshold;
	//int L0_Margin;

	//HEAD_FIND_METHOD Selected_SIDE_FIND_METHOD;













	//void RUN_Algorithm_00();
	//void RUN_Algorithm_01();
	//void RUN_Algorithm_02();
	//void RUN_Algorithm_03();
	//void Cal_Blob_Info(CString name, Mat &Reseult_Binary, int Cam_num);

	//void Fill_Hole(Mat &Binary);

	//int Delete_Small_Binary(Mat &Binary, int m_small);
	//int Connect_Line(Mat &Binary);

	//int Labeling_Delete_Small_Large_Label(Mat &Binary, int m_small);
	//void thinningIteration(Mat& img, int iter);
	//void thinning(Mat& im);
	//void Delete_Boundary(Mat &Img, int boundary_width);
	//void Add_Boundary(Mat &Img, int boundary_width);
	//void Parallel_Thinning(Mat& binary);
	//void ThinningVornoi(IplImage* inImg, IplImage* outImg);

	//void CensusTransform();
	//void ModifiedCensusTransform();

	//void drawArrow(Mat &image, CvPoint p, CvPoint q, CvScalar color, int arrowMagnitude = 9, int thickness=1, int line_type=8, int shift=0); 

	//int m_baseline_height;

	//int m_up_threshold;
	//int m_down_threshold;



	//// Mil 초기화 및 해제
	//void J_Initialize_Class();
	//void J_Dispose_Class();
	//
	//// Mil 이미지 불러오기
	//bool J_Image_Load(const char* filename);

	//bool J_Image_Transform(const char* filename, const char* m_target_name, int threshold_value, int threshold_method, int target_width, int target_height);
	//
	//
	//bool J_Alg_Test_Run();

	//bool intersection(Point2f o1, Point2f p1, Point2f o2, Point2f p2, Point2f &r);
	//bool GetIntersectPoint(CvPoint AP1, CvPoint AP2, CvPoint BP1, CvPoint BP2, Point2f* IP);
	//float HLineFitting(Mat Binary, double *v_slope, double *v_y);
	//float VLineFitting(Mat Binary, double *v_slope, double *v_y);

	//int SizeX_Src, SizeY_Src, Ch_Src;
	//unsigned char*		ImageBuf_Src;

	//// Time Check
	//LARGE_INTEGER frequency, tStart, tEnd;

};