#pragma once
#include "StdAfx.h"
#include "RANSAC_EllipseFittingAlgorithm.h" //타원 핏팅을 위해 추가
#include "Open_eVision_1_2.h"
using namespace Euresys::Open_eVision_1_2;

//Measure Mode Define
enum THRES_METHOD
{
	BINARY_INV=0,				//0
	BINARY,						//1
	BINARY_BETWEEN,				//2
	BINARY_BETWEENOUT,			//3
	BINARY_INV_OTSU,			//4
	BINARY_OTSU,				//5
	EDGE,						//6
	FIRSTROI,					//7
	DIFFAVG,					//8
	COMPARE,					//9
};

enum POSITION_METHOD
{
	LEFT_END = 0,				//0
	RIGHT_END,					//1
	LEFT_TOP,					//2
	LEFT_BOTTOM,				//3
	RIGHT_TOP,					//4
	RIGHT_BOTTOM,				//5
	CENTER,						//6
};

enum GUIDE_METHOD
{
	INDEX_TYPE = 0,				//0
	CLASS_TYPE,					//1
	BELT_TYPE,					//2
	NONE_TYPE,					//3
	ROI_TYPE,					//4
	SPECIAL_TYPE,				//5
};

enum ROTATION_METHOD
{
	NONE = 0,				//0
	LEFT,					//1
	RIGHT,					//2
	TOP,					//3
	BOTTOM,					//4
	LRCENTER,				//5
	TBCENTER,				//6
};

enum FILTER_DIRECTION
{
	ALL = 0,				//0
	HORIZONTAL,				//1
	VERTICAL,				//2
};

enum ALGORITHM_TB
{
	W_LENGTH_TB=0,						//0:Horizontal Length, 
	H_LENGTH_TB,						//1:Vertical Length,
	SIZE_CROSS_TB,						//2:Cross Dimension, 
	DIAMETER_TB,						//3:Diameter, 
	BRIGHTNESS_AREA_TB,					//4:Brightness of Area
	BRIGHTNESSDIFF_AREA_TB,				//5:Difference of Brightness
	AREA_BLOB_TB,						//6:BLOB Size
	COUNT_BLOB_TB,						//7:BLOB Count
	CIRCLE_BLOB_SIZE_TB,				//8:Edge Crack
	CIRCLE_BLOB_COUNT_TB,				//9:BLOB COUNT
	CIRCULARITY_TB,						//10:Circularity
	PITCH_COIN_TB,						//11:Pitch of Screw Thread
	DIST_TWO_CENTER_TB,					//12:Distance between two area
	AREA_COIN_TB,						//13:Size of Screw Thread
	COLOR_BLOB_TB,						//14:Color blob in circle ROI
	CONVEX_BLOB_TB,						//15:Convex BLOB analysis
	DIFFINNEROUTTER_TB,					//16:Center difference between Inner and outter circle
	MATCH_RATE_TB,						//17:Match rate
};

enum ALGORITHM_S
{
	W_LENGTH_S=0,						//0:Horizontal Length, 
	H_LENGTH_S,							//1:Vertical Length,
	CENT_HT_S,							//2:Concentricity, 
	ANGLE_BT_S,							//3:Angle of Bottom, 
	PITCH_COIN_S,						//4:Pitch of Screw Thread
	AREA_COIN_S,						//5:Size of Screw Thread
	LEADANGLE_COIN_S,					//6:Lead Angle of Screw Thread
	LEADANGLE_HARF_COIN_S,				//7:Harf Cycle Lead Angle of Screw Thread
	BODY_WIDTH_S,						//8:Body Width
	BODY_BENDING_S,						//9:Body Bending
	BRIGHTNESS_AREA_S,					//10:Brightness of Area
	AREA_BLOB_S,						//11:AREA BLOB
	BOTTOM_SHAPE_S,						//12:Bottom Shape
	DIST_TWO_CENTER_S,					//13:Distance between two area
	CONVEX_BLOB_S,						//14:Convex BLOB analysis
	MATCH_RATE_S,						//15:Match rate
};

enum RESULT_METHOD
{
	MIN=0,					//0
	MAX,					//1
	RANGE,					//2
	AVERAGE,				//3
	SUMOFALL,				//4
};

struct Point4D { double IDX, CX, CY, AREA; Rect ROI; };
struct Data2D { int s, idx; };
struct Circle_Blob_Info
{
	Point2f centerPoint;
	float fAngle;
	float fAngleFromCenter;
	float fWidth;
	float fHeight;
	int fArea;
	float fDistanceFromCenter;
	RotatedRect minRect;
	vector<Point> contour;
	int fLabelNum;
	int fBoundaryHitNum;
};

class CImPro_Library
{
public:
	CImPro_Library(void);
	~CImPro_Library(void);

public:
	// 알고리즘 전역 변수(0:Head 카메라, 1:Side 카메라, 2:Tab 카메라)
	Mat Src_Img[4];				// 카메라 이미지
	Mat Gray_Img[4];			// 카메라이미지 흑백 변환
	Mat Dst_Img[4];				// 결과 이미지
	Mat Template_Img[4];		// 카메라별 모델 이미지
	Mat Model_Img[4];			// 카메라별 모델 이미지
	CString Result_Info[4];		// 결과 저장
	int License_Check();		// 라이센스 체크
	bool License_START_Check();
	int m_License_Check;		// 라이센스 체크 되었는지? -1이면 체크 안됨.
	void Set_Image_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_1(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_2(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Set_Image_3(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// 카메라 이미지 입력
	void Reset_Dst_Image(int Cam_num);	// 결과이미지 리셋
	bool Result_Text_View;
	int m_Alg_Type;
	Mat element;
	Mat element_h;
	Mat element_v;
	int fontFace;
	double fontScale;
	int thickness;

	// 필요 함수 구현
	double f_angle(Point P1, Point P2);
	double f_angle360(Point P1, Point P2);
	void J_Fitting_Ellipse(Mat &Binary,double* Circle_Info);	// 타원 핏팅
	void J_Fill_Hole(Mat &Binary);								// Hole filling
	int J_Delete_Small_Binary(Mat &Binary, int m_small);		// 작은 것 없애기
	void J_Delete_Boundary(Mat &Img, int boundary_width);		// Boundary 삭제
	void J_Rotate(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num);	// 회전변환
	void J_Rotate_Black(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num);	// 회전변환
	void J_Rotate_PRE(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num);	// 회전변환
	Rect Check_ROI(Rect O_ROI, int W, int H);

	void drawArrow(Mat &image, CvPoint p, CvPoint q, CvScalar color, int arrowMagnitude, int thickness, int line_type, int shift);
	void J_FastMatchTemplate(Mat& srca,  // The reference image
							Mat& srcb,  // The template image
							Mat& dst    // Template matching result
							);	// Find tool에 필요한 것들
	int number_of_iterations;
	double termination_eps;
	string warpType;
	Point2f draw_warped_roi(Mat& image, const int width, const int height, Mat& W, int m_left, int m_top, int Cam_num);
	Point2f J_Model_Find(int Cam_num);
	float Cal_Angle(Mat& Out_binary, int Cam_num);

	EImageBW8 E_Cam_Img[4];							// Euresys 카메라이미지
	EPatternFinder m_Cam_Find[4];					// Euresys 파인드툴
	vector<EFoundPattern> m_Cam_FoundPattern[4];	// Euresys 파인드결과
	EImageBW8 EBW8Image1[4];
	float C_X[4];
	float C_Y[4];
	float MatchRate[4];

	int m_Text_View[3];
	bool Result_Debugging;

	bool ROI_Mode;
	int ROI_Num;
	int ROI_CAM_Num;

	// CAM0 알고리즘 관련 변수 및 함수
	struct ALG_BOLT_ParameterData {
		// Input 변수
		int nTableType;							// Table Type 0 : 인덱스, 1 : 유리판
		int nCamPosition;						// 카메라 위치 0 : TOP, BOTTOM, 1 : SIDE

		int nUse[41];							// 계산유무 0:계산안함, 1:계산함.
		Rect nRect[41];							// ROI 정보
		Rect nRect_Backup[41];					// 백업 ROI 정보

		//int nDisplay[41];						// 그리기 옵션  -1 : 안보임, 0 : 설정때, 1 : 검사중

		float Offset[41];						// Offset
		float nResolution[2];					// 카메라 Resolution [0]가로, [1]세로

		int nMethod_Thres[41];					// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
		float nThres_V1[41];					// 임계값 V1 %값
		float nThres_V2[41];					// 임계값 V2 %값
		int nMethod_Direc[41];					// 측정 방향 0:Horizontal Length, 1:Vertical Length, 2:나사선 크기, 3:나사선 피치, 4:머리 높이
		int nMethod_Cal[41];					// Measurement 0:최소, 1:최대, 2:최대-최소, 3:평균, 4:Total
		int nCalmin[41];						// 계산시 범위 Min %
		int nCalmax[41];						// 계산시 범위 Max %
		int nCircle1Radius[41];					// 밝기 차이 계산 C1 Radius
		int nCircle1Thickness[41];				// 밝기 차이 계산 C1 Thickness
		int nCircle2Radius[41];					// 밝기 차이 계산 C2 Radius
		int nCircle2Thickness[41];				// 밝기 차이 계산 C2 Thickness
		int nCirclePositionMethod[41];			// 원형 위치 계산 방법
		float nCirclePositionThreshold[41];		// 원형 위치 임계값
		float nCircleStartAngle[41];			// 원형 시작 각
		float nCircleEndAngle[41];				// 원형 종료 각
		int nCircleOutputMethod[41];			// 출력 계산 방법
		int nConvexBLOBOption[41];				// 볼록 BLOB 차이 옵션

		double nCalAngle[41];					// 계산시 각도(Deg.)
		double nCalMinDist[41];					// 계산시 최소거리(mm)
		double nCalMaxDist[41];					// 계산시 최대거리(mm)

		int nROI0_FilterSize[41];				// ROI0의 노이즈 제거 필터 크기
		double nROI0_BLOB_Min_Size[41];			// ROI0의 BLOB 최소 크기
		double nROI0_BLOB_Max_Size[41];			// ROI0의 BLOB 최대 크기
		int nROI0_MergeFilterSize[41];			// ROI0의 MERGE 필터 크기

		double nDiameter_Min_Size[41];			// 직경 최소 길이
		double nDiameter_Max_Size[41];			// 직경 최대 길이

		double nThickness[41];					// 나사 사이드 볼때 두께
		int nHeightforShape[41];				// 나사 뽀족 찾을 높이
		int nAngleHeightTop[41];				// 나사 회전 높이 오프셋
		int nAngleHeightHeight[41];				// 나사 회전 높이 오프셋
		int nAngleHeightFilterSize[41];			// 나사선 제거 필터 크기
		int nNASAPasteFilterSize[41];			// 나사선 붙음 방지 필터 횟수

		int nNumConnectedCircleBLOB[41];		// 불량 붙음 횟수
		int nROI0_Rotation_Method[41];			// 회전 보정 방법 선택
		int nROI0_FilterDirection[41];			// ROI0의 노이즈 제거 필터 방향

		int nROI12_Direction[41];				// ROI의 측정 방향
		Rect nROI1[41];							// ROI 정보
		Rect nROI2[41];							// ROI 정보

		int nCrossSizeMethod[41];				// 십자 6각 크기 측정 방법
		int nThreadSizeMethod[41];				// 나사산 크기 측정 방법
		int nCrossAngleNumber[41];				// 다각 개수
		int nCrossOutput[41];					// 출력 방법(0:반지름, 1:반경)

		int nDirecFilterUsage[41];				// MP Filter 사용유무
		int nDirecFilter[41];					// MP Filter 방향
		int nDirecFilterCNT[41];				// MP Filter 횟수
		double nDirecFilterDarkThres[41];		// MP Filter 어두운 임계값
		double nDirecFilterBrightThres[41];		// MP Filter 밝은 임계값
		int nBlurFilterCNT[41];					// 전처리 BLUR Filter 횟수

		int nColorMethod[41];					// 색상 처리 방법(0:흑백,1컬러)
		int nColorMinThres[41];					// 색상 임계 최솟값
		int nColorMaxThres[41];					// 색상 임계 최댓값
		int nColorOutput[41];					// 색상 출력방법(0:밝기,1:픽셀수)
		int nColorBlurFilterCNT[41];			// 전처리 BLUR Filter 횟수

		Mat Gray_Obj_Img;						// 검사대상 ROI Gray 이미지
		Mat Thres_Obj_Img;						// 검사대상 ROI 임계화 이미지
		Point Object_Postion;					// 검사대상 ROI 설정시 기준 LT좌표 -상부:중심좌표, 측면:LT좌표
		Point Offset_Object_Postion;			// 검사대상 ROI 검사시 설정대비 LT좌표 움직인 량
		vector<vector<Point>> Object_contours;	// 검사대상 Contour 정보
		vector<Vec4i> Object_hierarchy;			// 검사대상 Hierarchy 정보
		int Object_1st_idx;						// 검사대상 가장큰 index
		int Object_2nd_idx;						// 검사대상 두번째 큰 index

		float nThres_InnerCircle[41];			// 내경 임계값
		int nThresMethod_InnerCircle[41];		// 내경 임계 방법(0:이하,1:이상)
		int nOutput[41];						// 내외경 차이 출력 방법(0:mm,1:픽셀)

		ALG_BOLT_ParameterData()
		{
			nTableType = 0;
			nCamPosition = 0;
			Object_Postion.x = Object_Postion.y = 0;
			Offset_Object_Postion.x = Offset_Object_Postion.y = 0;
			for (int i=0;i<21;i++)
			{
				nUse[i] = 1;
				//nDisplay[i] = 0;
				nThres_V1[i] = 30;
				nThres_V2[i] = 70;
				nMethod_Thres[i] = 0;
				nMethod_Cal[i] = 1;
				nMethod_Direc[i] = 0;
				nRect[i].x = nRect[i].y = nRect[i].width = nRect[i].height = 0;
				Offset[i] = 0;
				nCircle1Radius[i] = 100;
				nCircle1Thickness[i] = 10;
				nCircle2Radius[i] = 115;
				nCircle2Thickness[i] = 10;
				nThickness[i] = 50;
				nHeightforShape[i] = 40;
				nNASAPasteFilterSize[i] = 0;
				nNumConnectedCircleBLOB[i] = 2;
				nROI0_Rotation_Method[i] = 0;
				nROI0_FilterDirection[i] = 0;
				nROI12_Direction[i] = 0;
				nROI1[i].x = nROI1[i].y = nROI1[i].width = nROI1[i].height = 0;
				nROI2[i].x = nROI2[i].y = nROI2[i].width = nROI2[i].height = 0;
				nThreadSizeMethod[i] = 0;

				nDirecFilterUsage[i] = nDirecFilter[i] = nDirecFilterCNT[i] = nDirecFilterDarkThres[i] = nDirecFilterBrightThres[i] = nCrossSizeMethod[i] = 0;
				nColorMethod[i] = nColorMinThres[i] = nColorMaxThres[i] = nColorOutput[i] = nBlurFilterCNT[i] = nROI0_MergeFilterSize[i] = 0;
			}
			nResolution[0] = nResolution[1] = 1;
		}
	};
	ALG_BOLT_ParameterData BOLT_Param[4];	// ALG_AP Parameter Data

	bool RUN_Algorithm_CAM(int Cam_num);
	bool ROI_Object_Find(int Cam_num);
	void Rotation_Calibration(int Cam_num);

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 스기야마 알고리즘을 위한 변수들
	int nSpecialCustomerN0;

	enum H0_INSPECTION_ITEM
	{
		INNER_RADIUS,			//내경
		OUTER_RADIUS,			//외경
		SLIT_WIDTH_MIN,			//슬릿 폭 MIN
		SLIT_WIDTH_MAX,			//슬릿 폭 MAX
		SLIT_HEIGHT_MIN,		//슬릿 깊이 MIN
		SLIT_HEIGHT_MAX,		//슬릿 깊이 MAX
		DANJA_TO_DANJA_ANGLE,	//단자 단자 간 각도
		DANJA_TO_SLIT_ANGLE,	//단자 슬린 간 각도
		SLIT_TO_SLIT_ANGLE,		//슬릿 슬린 간 각도
		DANJA_COUNT,			//단자 갯수
		DANJA_ROTATION_COUNT,	//단자 휨 갯수
		SLIT_COUNT,				//슬릿 개수
		SLIT_DEFECT_COUNT,		//슬릿 불량 개수
		DANJA_EACH_ANGLE,		//단자 각각의 각도
		SGYM_INSPECTION_ITEM_END,
	};

	struct Slit_Info
	{
		Point2f centerPoint;
		float fAngle;
		float fAngleFromCenter;
		float fWidth;
		float fHeight;
		float fArea;
	};
	struct Danja_Info
	{
		Point2f centerPoint;
		float fAngle;
		float fAngleFromCenter;
		float fWidth;
		float fHeight;
		float fArea;
	};

	// H0 외경 알고리즘 관련 변수 및 함수
	struct ALG_SGYM_ParameterData {
		vector<Rect> ROI;					// ROI
		Rect nRect[1];						// ROI 정보
		
		int m_HEAD_FIND_METHOD;				// 찾을 방법 index
		int L_Threshold;					// 임계값 L 
		int H_Threshold;					// 임계값 H
		int ROI_Margin;						// ROI 마진
		Mat Template_Img;					// 탬플릿 이미지
		Mat Head_Image;						// 찾은 해드 이미지
		Mat Head_Threshold_Image;			// 찾은 해드 임계화 이미지
		double Out_Circle_Info[5];			// 해드 외경 Circle 정보
		sEllipse c_ellipse;					// 해드 외경 핏팅을 위해 필요
		Rect m_Rect_Head;					// 원본에서 Head의 위치 파악
		double Result_Value[SGYM_INSPECTION_ITEM_END];            // 0. 내경 1. 단자경, 2. 슬리트 폭, 3. 슬리트 깊이, 4. 단자간 각도, 5. 각도, 6. 슬리트간 각도, 7. 단자수, 8. 단자 휨 갯수, 9. 슬리트 개수
		vector<int> Parameter_Calc_Method;		// 0. 측정 방법 
		int Chip_Threshold;
		int DlDDM_Size_Threshold;

		double dDanjaMinArea;
		double dDanjaMinHeight;
		double dDanjaMaxHeight;
		double dDanjaRotationRage;
		double dSlitMinArea;
		double dSlitDefectMinArea;
		double dSlitDefectMaxArea;
		vector<Slit_Info> vec_Slit_Info;
		vector<Danja_Info> vec_Danja_Info;


		ALG_SGYM_ParameterData()
		{
			L_Threshold = 50;
			H_Threshold = 100;
			ROI_Margin = 5;
			dDanjaMinArea = 100;
			dDanjaMinHeight = 50;
			dDanjaMaxHeight = 70;
			dDanjaRotationRage = 1;
			dSlitMinArea = 100;
			dSlitDefectMinArea = 50;
			dSlitDefectMaxArea = 100;
			Chip_Threshold = 150;
			DlDDM_Size_Threshold = 5;
		}
	};
	int nSGYM_Measure_Count;
	double round( double value, int pos );
	ALG_SGYM_ParameterData ALG_SGYM_Param;	// ALG_H0 Parameter Data
	void RUN_Algorithm_SUGIYAMA();
	void AKAZE_Homography(Mat img_object, Mat img_scene, Mat warp_dst, Mat diff_img);
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
};