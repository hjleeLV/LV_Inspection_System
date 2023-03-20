#pragma once
#include "StdAfx.h"
#include "RANSAC_EllipseFittingAlgorithm.h" //Ÿ�� ������ ���� �߰�
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
	// �˰��� ���� ����(0:Head ī�޶�, 1:Side ī�޶�, 2:Tab ī�޶�)
	Mat Src_Img[4];				// ī�޶� �̹���
	Mat Gray_Img[4];			// ī�޶��̹��� ��� ��ȯ
	Mat Dst_Img[4];				// ��� �̹���
	Mat Template_Img[4];		// ī�޶� �� �̹���
	Mat Model_Img[4];			// ī�޶� �� �̹���
	CString Result_Info[4];		// ��� ����
	int License_Check();		// ���̼��� üũ
	bool License_START_Check();
	int m_License_Check;		// ���̼��� üũ �Ǿ�����? -1�̸� üũ �ȵ�.
	void Set_Image_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// ī�޶� �̹��� �Է�
	void Set_Image_1(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// ī�޶� �̹��� �Է�
	void Set_Image_2(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// ī�޶� �̹��� �Է�
	void Set_Image_3(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num);	// ī�޶� �̹��� �Է�
	void Reset_Dst_Image(int Cam_num);	// ����̹��� ����
	bool Result_Text_View;
	int m_Alg_Type;
	Mat element;
	Mat element_h;
	Mat element_v;
	int fontFace;
	double fontScale;
	int thickness;

	// �ʿ� �Լ� ����
	double f_angle(Point P1, Point P2);
	double f_angle360(Point P1, Point P2);
	void J_Fitting_Ellipse(Mat &Binary,double* Circle_Info);	// Ÿ�� ����
	void J_Fill_Hole(Mat &Binary);								// Hole filling
	int J_Delete_Small_Binary(Mat &Binary, int m_small);		// ���� �� ���ֱ�
	void J_Delete_Boundary(Mat &Img, int boundary_width);		// Boundary ����
	void J_Rotate(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num);	// ȸ����ȯ
	void J_Rotate_Black(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num);	// ȸ����ȯ
	void J_Rotate_PRE(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num);	// ȸ����ȯ
	Rect Check_ROI(Rect O_ROI, int W, int H);

	void drawArrow(Mat &image, CvPoint p, CvPoint q, CvScalar color, int arrowMagnitude, int thickness, int line_type, int shift);
	void J_FastMatchTemplate(Mat& srca,  // The reference image
							Mat& srcb,  // The template image
							Mat& dst    // Template matching result
							);	// Find tool�� �ʿ��� �͵�
	int number_of_iterations;
	double termination_eps;
	string warpType;
	Point2f draw_warped_roi(Mat& image, const int width, const int height, Mat& W, int m_left, int m_top, int Cam_num);
	Point2f J_Model_Find(int Cam_num);
	float Cal_Angle(Mat& Out_binary, int Cam_num);

	EImageBW8 E_Cam_Img[4];							// Euresys ī�޶��̹���
	EPatternFinder m_Cam_Find[4];					// Euresys ���ε���
	vector<EFoundPattern> m_Cam_FoundPattern[4];	// Euresys ���ε���
	EImageBW8 EBW8Image1[4];
	float C_X[4];
	float C_Y[4];
	float MatchRate[4];

	int m_Text_View[3];
	bool Result_Debugging;

	bool ROI_Mode;
	int ROI_Num;
	int ROI_CAM_Num;

	// CAM0 �˰��� ���� ���� �� �Լ�
	struct ALG_BOLT_ParameterData {
		// Input ����
		int nTableType;							// Table Type 0 : �ε���, 1 : ������
		int nCamPosition;						// ī�޶� ��ġ 0 : TOP, BOTTOM, 1 : SIDE

		int nUse[41];							// ������� 0:������, 1:�����.
		Rect nRect[41];							// ROI ����
		Rect nRect_Backup[41];					// ��� ROI ����

		//int nDisplay[41];						// �׸��� �ɼ�  -1 : �Ⱥ���, 0 : ������, 1 : �˻���

		float Offset[41];						// Offset
		float nResolution[2];					// ī�޶� Resolution [0]����, [1]����

		int nMethod_Thres[41];					// Threshold 0:����, 1:�̻�, 2:����, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 ��� ���
		float nThres_V1[41];					// �Ӱ谪 V1 %��
		float nThres_V2[41];					// �Ӱ谪 V2 %��
		int nMethod_Direc[41];					// ���� ���� 0:Horizontal Length, 1:Vertical Length, 2:���缱 ũ��, 3:���缱 ��ġ, 4:�Ӹ� ����
		int nMethod_Cal[41];					// Measurement 0:�ּ�, 1:�ִ�, 2:�ִ�-�ּ�, 3:���, 4:Total
		int nCalmin[41];						// ���� ���� Min %
		int nCalmax[41];						// ���� ���� Max %
		int nCircle1Radius[41];					// ��� ���� ��� C1 Radius
		int nCircle1Thickness[41];				// ��� ���� ��� C1 Thickness
		int nCircle2Radius[41];					// ��� ���� ��� C2 Radius
		int nCircle2Thickness[41];				// ��� ���� ��� C2 Thickness
		int nCirclePositionMethod[41];			// ���� ��ġ ��� ���
		float nCirclePositionThreshold[41];		// ���� ��ġ �Ӱ谪
		float nCircleStartAngle[41];			// ���� ���� ��
		float nCircleEndAngle[41];				// ���� ���� ��
		int nCircleOutputMethod[41];			// ��� ��� ���
		int nConvexBLOBOption[41];				// ���� BLOB ���� �ɼ�

		double nCalAngle[41];					// ���� ����(Deg.)
		double nCalMinDist[41];					// ���� �ּҰŸ�(mm)
		double nCalMaxDist[41];					// ���� �ִ�Ÿ�(mm)

		int nROI0_FilterSize[41];				// ROI0�� ������ ���� ���� ũ��
		double nROI0_BLOB_Min_Size[41];			// ROI0�� BLOB �ּ� ũ��
		double nROI0_BLOB_Max_Size[41];			// ROI0�� BLOB �ִ� ũ��
		int nROI0_MergeFilterSize[41];			// ROI0�� MERGE ���� ũ��

		double nDiameter_Min_Size[41];			// ���� �ּ� ����
		double nDiameter_Max_Size[41];			// ���� �ִ� ����

		double nThickness[41];					// ���� ���̵� ���� �β�
		int nHeightforShape[41];				// ���� ���� ã�� ����
		int nAngleHeightTop[41];				// ���� ȸ�� ���� ������
		int nAngleHeightHeight[41];				// ���� ȸ�� ���� ������
		int nAngleHeightFilterSize[41];			// ���缱 ���� ���� ũ��
		int nNASAPasteFilterSize[41];			// ���缱 ���� ���� ���� Ƚ��

		int nNumConnectedCircleBLOB[41];		// �ҷ� ���� Ƚ��
		int nROI0_Rotation_Method[41];			// ȸ�� ���� ��� ����
		int nROI0_FilterDirection[41];			// ROI0�� ������ ���� ���� ����

		int nROI12_Direction[41];				// ROI�� ���� ����
		Rect nROI1[41];							// ROI ����
		Rect nROI2[41];							// ROI ����

		int nCrossSizeMethod[41];				// ���� 6�� ũ�� ���� ���
		int nThreadSizeMethod[41];				// ����� ũ�� ���� ���
		int nCrossAngleNumber[41];				// �ٰ� ����
		int nCrossOutput[41];					// ��� ���(0:������, 1:�ݰ�)

		int nDirecFilterUsage[41];				// MP Filter �������
		int nDirecFilter[41];					// MP Filter ����
		int nDirecFilterCNT[41];				// MP Filter Ƚ��
		double nDirecFilterDarkThres[41];		// MP Filter ��ο� �Ӱ谪
		double nDirecFilterBrightThres[41];		// MP Filter ���� �Ӱ谪
		int nBlurFilterCNT[41];					// ��ó�� BLUR Filter Ƚ��

		int nColorMethod[41];					// ���� ó�� ���(0:���,1�÷�)
		int nColorMinThres[41];					// ���� �Ӱ� �ּڰ�
		int nColorMaxThres[41];					// ���� �Ӱ� �ִ�
		int nColorOutput[41];					// ���� ��¹��(0:���,1:�ȼ���)
		int nColorBlurFilterCNT[41];			// ��ó�� BLUR Filter Ƚ��

		Mat Gray_Obj_Img;						// �˻��� ROI Gray �̹���
		Mat Thres_Obj_Img;						// �˻��� ROI �Ӱ�ȭ �̹���
		Point Object_Postion;					// �˻��� ROI ������ ���� LT��ǥ -���:�߽���ǥ, ����:LT��ǥ
		Point Offset_Object_Postion;			// �˻��� ROI �˻�� ������� LT��ǥ ������ ��
		vector<vector<Point>> Object_contours;	// �˻��� Contour ����
		vector<Vec4i> Object_hierarchy;			// �˻��� Hierarchy ����
		int Object_1st_idx;						// �˻��� ����ū index
		int Object_2nd_idx;						// �˻��� �ι�° ū index

		float nThres_InnerCircle[41];			// ���� �Ӱ谪
		int nThresMethod_InnerCircle[41];		// ���� �Ӱ� ���(0:����,1:�̻�)
		int nOutput[41];						// ���ܰ� ���� ��� ���(0:mm,1:�ȼ�)

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
	// ����߸� �˰����� ���� ������
	int nSpecialCustomerN0;

	enum H0_INSPECTION_ITEM
	{
		INNER_RADIUS,			//����
		OUTER_RADIUS,			//�ܰ�
		SLIT_WIDTH_MIN,			//���� �� MIN
		SLIT_WIDTH_MAX,			//���� �� MAX
		SLIT_HEIGHT_MIN,		//���� ���� MIN
		SLIT_HEIGHT_MAX,		//���� ���� MAX
		DANJA_TO_DANJA_ANGLE,	//���� ���� �� ����
		DANJA_TO_SLIT_ANGLE,	//���� ���� �� ����
		SLIT_TO_SLIT_ANGLE,		//���� ���� �� ����
		DANJA_COUNT,			//���� ����
		DANJA_ROTATION_COUNT,	//���� �� ����
		SLIT_COUNT,				//���� ����
		SLIT_DEFECT_COUNT,		//���� �ҷ� ����
		DANJA_EACH_ANGLE,		//���� ������ ����
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

	// H0 �ܰ� �˰��� ���� ���� �� �Լ�
	struct ALG_SGYM_ParameterData {
		vector<Rect> ROI;					// ROI
		Rect nRect[1];						// ROI ����
		
		int m_HEAD_FIND_METHOD;				// ã�� ��� index
		int L_Threshold;					// �Ӱ谪 L 
		int H_Threshold;					// �Ӱ谪 H
		int ROI_Margin;						// ROI ����
		Mat Template_Img;					// ���ø� �̹���
		Mat Head_Image;						// ã�� �ص� �̹���
		Mat Head_Threshold_Image;			// ã�� �ص� �Ӱ�ȭ �̹���
		double Out_Circle_Info[5];			// �ص� �ܰ� Circle ����
		sEllipse c_ellipse;					// �ص� �ܰ� ������ ���� �ʿ�
		Rect m_Rect_Head;					// �������� Head�� ��ġ �ľ�
		double Result_Value[SGYM_INSPECTION_ITEM_END];            // 0. ���� 1. ���ڰ�, 2. ����Ʈ ��, 3. ����Ʈ ����, 4. ���ڰ� ����, 5. ����, 6. ����Ʈ�� ����, 7. ���ڼ�, 8. ���� �� ����, 9. ����Ʈ ����
		vector<int> Parameter_Calc_Method;		// 0. ���� ��� 
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