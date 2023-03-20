#include "StdAfx.h"
#include "ImPro_Library.h"
#include "math.h"
#include <opencv2/features2d.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/calib3d.hpp>
#include <opencv2/opencv.hpp>
#include "fftm.hpp"
#include "mystuff.h"
#include "data.h"
#include "circle.h"
#include "Utilities.cpp"
#include "CircleFitByLevenbergMarquardtReduced.cpp"
#include "CircleFitByTaubin.cpp"

#define PI 3.14159265

struct compare_Size {
	bool operator() (Point4D a, Point4D b) { return (a.CY  < b.CY);}
} Point_compare_Size;

struct compare_Dist {
	bool operator() (Point3D a, Point3D b) { return (a.DIST  < b.DIST);}
} Point_compare_Dist;

struct compare_Size_X {
	bool operator() (Point4D a, Point4D b) { return (a.CX  < b.CX);}
} Point_compare_Size_X;

struct sort_descending_Circle_Blob_Info {
	bool operator() (Circle_Blob_Info a, Circle_Blob_Info b) { return (a.fAngleFromCenter  < b.fAngleFromCenter);}
} Descending_Circle_Blob_Info;


bool sort_descending(double A,double B)
{
	return A < B;
}

bool sort_ascending(double A,double B)
{
	return A < B;
}

struct compare_Increasing {
	bool operator() (double a, double b) { return (a  < b);}
} Point_compare_Increasing;

struct compare_Increasing_2d {
	bool operator() (Data2D a, Data2D b) { return (a.s  > b.s);}
} Point_compare_Increasing_2d;

#define HOMO_VECTOR(H, x, y)\
	H.at<float>(0,0) = (float)(x);\
	H.at<float>(1,0) = (float)(y);\
	H.at<float>(2,0) = 1.;

#define GET_HOMO_VALUES(X, x, y)\
	(x) = static_cast<float> (X.at<float>(0,0)/X.at<float>(2,0));\
	(y) = static_cast<float> (X.at<float>(1,0)/X.at<float>(2,0));

CImPro_Library::CImPro_Library(void)
{
	m_License_Check = 1;
	element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
	element_h = getStructuringElement(MORPH_RECT, Size(3, 1), Point(-1, -1) );
	element_v = getStructuringElement(MORPH_RECT, Size(1, 3), Point(-1, -1) );
	fontFace = FONT_HERSHEY_SIMPLEX;
	fontScale = 0.35;
	thickness = 1;
	number_of_iterations = 100;
	termination_eps = 0.0001;
	warpType = "affine";
	Alg_Run_Check[0] = false;
	Alg_Run_Check[1] = false;
	Alg_Run_Check[2] = false;
	Alg_Run_Check[3] = false;
	t_Alg_Run_Check_retun_cnt[0] = 0;
	t_Alg_Run_Check_retun_cnt[1] = 0;
	t_Alg_Run_Check_retun_cnt[2] = 0;
	t_Alg_Run_Check_retun_cnt[3] = 0;
}


CImPro_Library::~CImPro_Library(void)
{
}

//#pragma region 전역 함수
bool CImPro_Library::License_START_Check()
{
	int handle;
	int res1;

	// opening LC device
	res1 = LC_open(0x3f3f3f3f, 0, &handle);
	if(res1) {
		AfxMessageBox("There is no dongle license key! Please buy the S/W license!");
		m_License_Check = -1;
		return false;
	}
	// verify normal user password
	res1 = LC_passwd(handle, 1, (unsigned char *) "33577748");  
	if(res1) {
		LC_close(handle);
		AfxMessageBox("There is no dongle license key! Please buy the S/W license!");		
		m_License_Check = -1;
		return false;
	}

	unsigned char *buffer;
	LC_read(handle, 0, buffer);

	//CString myString;
	//myString.Format("%d",buffer);
	//AfxMessageBox(myString);

	LC_close(handle);
	m_License_Check = 1;


	return true;
}


int CImPro_Library::License_Check()
{
	if (m_License_Check > -1)
	{
		m_License_Check++;
		if (m_License_Check > 100)
		{
			m_License_Check = -1;
		}
		return 0;
	}

	//return 0;

	int handle;
	int res1;

	// opening LC device
	res1 = LC_open(0x3f3f3f3f, 0, &handle);
	if(res1) {
		AfxMessageBox("Please check the S/W license!");		
		return res1;
	}
	// verify normal user password
	res1 = LC_passwd(handle, 1, (unsigned char *) "33577748");  
	if(res1) {
		LC_close(handle);
		AfxMessageBox("Please check the S/W license!");		
		return res1;
	}
	LC_close(handle);
	m_License_Check++;
	return res1;
}

double CImPro_Library::f_angle(Point P1, Point P2) 
{
	double dy = P1.y - P2.y;
	double dx = P1.x - P2.x;
	double theta = atan2(dy, dx); // range (-PI, PI]
	theta *= 180 / PI; // rads to degs, range (-180, 180]
	return theta;
}

double CImPro_Library::f_angle360(Point P1, Point P2) 
{
	double theta = f_angle(P1, P2); // range (-180, 180]
	if (theta < 0) theta = 360 + theta; // range [0, 360)
	return theta;
}


void CImPro_Library::Set_Image_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	if (channel==3)
	{
		Src_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1);
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
	} else if (channel == 1)
	{
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1, src_u8);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}


void CImPro_Library::Set_Image_1(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	if (channel==3)
	{
		Src_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1);
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
		//AfxMessageBox("color");
	} else if (channel == 1)
	{
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1, src_u8);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
		//AfxMessageBox("gray");
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}



void CImPro_Library::Set_Image_2(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	if (channel==3)
	{
		Src_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1);
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
	} else if (channel == 1)
	{
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1, src_u8);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}


void CImPro_Library::Set_Image_3(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	if (channel==3)
	{
		Src_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1);
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3, src_u8);
	} else if (channel == 1)
	{
		Gray_Img[Cam_num] = Mat(size_y, size_x, CV_8UC1, src_u8);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Mat(size_y, size_x, CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}



void CImPro_Library::Reset_Dst_Image(int Cam_num)
{
	if (Alg_Run_Check[Cam_num])
	{
		return;
	}
	if (!Gray_Img[Cam_num].empty())
	{
		//Dst_Img[Cam_num] = NULL;
		Dst_Img[Cam_num] = Mat(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
}

void CImPro_Library::J_Fitting_Ellipse(Mat &Binary,double* Circle_Info, bool Method)
{
	// Method : true ellipse / false circle
	vector<vector<Point> > contours;
	vector<Vec4i> hierarchy;
	Mat Tmp_Img = Mat(Binary.size(), CV_8UC1);
	Binary.copyTo(Tmp_Img);

	findContours(Tmp_Img, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
	int m_max_object_num = -1;int m_max_object_value = 0;
	if (contours.size() > 0)
	{
		Rect boundRect;
		for( int k = 0; k < contours.size(); k++ )
		{  
			boundRect = boundingRect( Mat(contours[k]) );
			if (m_max_object_value<= boundRect.width*boundRect.height)
			{
				m_max_object_value = boundRect.width*boundRect.height;
				m_max_object_num = k;
			}
		}
	}
	if (m_max_object_num == -1)
	{
		return;
	}

	///////////////////////////////////////////////////////////////////////////
	// Contour Merge
	//if (contours.size() > 1)
	//{
	//	vector<vector<Point> > total_contours(1);
	//	for (int i=0;i<contours.size();i++)
	//	{
	//		for (int j=0;j<contours[i].size();j++)
	//		{
	//			total_contours[0].push_back(contours[i][j]);
	//		}
	//	}
	//}
	///////////////////////////////////////////////////////////////////////////

	if (!Method)
	{ // Circle
		reals LambdaIni=0.001;
		Circle circlef,circleIni;

		int n = contours[m_max_object_num].size();
		double* DataX = new double[n];
		double* DataY = new double[n];

		for(int s = 0; s < contours[m_max_object_num].size(); s++)
		{
			DataX[s] = contours[m_max_object_num][s].x;
			DataY[s] = contours[m_max_object_num][s].y;
		}
		Data dataXY(n,DataX,DataY);

		delete [] DataX;
		delete [] DataY;

		circleIni = CircleFitByTaubin (dataXY);
		int code = CircleFitByLevenbergMarquardtReduced (dataXY,circleIni,LambdaIni,circlef);
		if (circlef.a - circlef.r < 0 || circlef.a + circlef.r >=  Binary.cols || circlef.b - circlef.r < 0 || circlef.b + circlef.r >=  Binary.rows)
		{
			code = 1;
		}

		if (code == 0)
		{
			Circle_Info[0] = circlef.a;
			Circle_Info[1] = circlef.b;
			Circle_Info[2] = circlef.r*2;
			Circle_Info[3] = circlef.r*2;
			Circle_Info[4] = 0;
		}
	}
	else
	{ // Ellipse
		sEllipse c_ellipse;
		for( size_t k = 0; k < contours.size(); k++ ) 
		{	
			const int no_data = (int)contours[m_max_object_num].size();
			sPoint *data = new sPoint[no_data];

			if (no_data > 10)
			{
				for(int i=0; i<no_data; i++)
				{
					data[i].x =  (double)contours[m_max_object_num][i].x;
					data[i].y =  (double)contours[m_max_object_num][i].y;
				}

				double cost = ransac_ellipse_fitting (data, no_data, c_ellipse, 50);

				Circle_Info[0] = c_ellipse.cx;
				Circle_Info[1] = c_ellipse.cy;
				Circle_Info[2] = c_ellipse.w*2;
				Circle_Info[3] = c_ellipse.h*2;
				Circle_Info[4] = c_ellipse.theta*180/M_PI;
			}
			delete [] data;
		}
	}
	//}
}

void CImPro_Library::J_Fill_Hole(Mat &Binary)
{
	Mat holes = Mat(Binary.size(), CV_8UC1);
	Binary.copyTo(holes);
	floodFill(holes,Point2i(0,0),Scalar(1));
	for(int i=0;i<Binary.rows*Binary.cols;i++)
	{
		if(holes.data[i]==0)
			Binary.data[i]=255;
	}
}

int CImPro_Library::J_Delete_Small_Binary(Mat &Binary, int m_small)
{
	vector<vector<Point> > contours;
	vector<Vec4i> hierarchy;
	float m_length = 0;int idx = 0;
	float m_area = 0;

	findContours( Binary, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

	//vector<vector<Point> > contours_poly( contours.size() );
	//vector<Rect> boundRect( contours.size() );

	Binary = Mat::zeros(Binary.size(), CV_8UC1);
	Scalar color( 255);
	if (contours.size() > 0)
	{
		//#pragma omp parallel for
		for( int k = 0; k < contours.size(); k++ ) 
		{
			m_area = contourArea(contours[k],false);

			if ( m_area <= m_small)
			{
				contours[k].clear();
			} else
			{
				drawContours( Binary, contours, k, color, CV_FILLED, 8, hierarchy );
			}
		}
	}
	return contours.size();
}

// comparison function object
bool J_CompareContourAreas ( std::vector<cv::Point> contour1, std::vector<cv::Point> contour2 ) 
{
	double i = fabs( contourArea(cv::Mat(contour1)) );
	double j = fabs( contourArea(cv::Mat(contour2)) );
	return ( i < j );
}

void CImPro_Library::J_Delete_Boundary(Mat &Img, int boundary_width)
{
	for (int ss=0;ss<boundary_width;ss++)
	{
		for (int kk=0;kk<Img.rows;kk++)
		{
			Img.at<uchar>(kk,ss) = 0;
			Img.at<uchar>(kk,Img.cols-ss-1) = 0;
		}
	}
	for (int ss=0;ss<Img.cols;ss++)
	{
		for (int kk=0;kk<boundary_width;kk++)
		{
			Img.at<uchar>(kk,ss) = 0;
			Img.at<uchar>(Img.rows-kk-1,ss) = 0;
		}
	}
}

/**
* Rotate an image
*/
void CImPro_Library::J_Rotate(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num)
{
	if (angle == 0)
	{
		Mat dst = Mat(src.size(), src.type());
		src.copyTo(dst);
		//dst = src.clone();
	}
	//int len = std::max(src.cols, src.rows);
	cv::Point2f pt(src.cols/2, 0);
	//cv::Point2f pt(BOLT_Param[Cam_num].nRect[0].x, BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height);
	cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_WRAP,CV_RGB(255,255,255));
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_WRAP,CV_RGB(255,255,255));
	}
}

void CImPro_Library::J_Rotate_Black(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num)
{
	if (angle == 0)
	{
		Mat dst = Mat(src.size(), src.type());
		src.copyTo(dst);
		//dst = src.clone();
	}
	//int len = std::max(src.cols, src.rows);
	cv::Point2f pt(src.cols/2, 0);
	//cv::Point2f pt(BOLT_Param[Cam_num].nRect[0].x, BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height);
	cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_WRAP,CV_RGB(0,0,0));
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_WRAP,CV_RGB(0,0,0));
	}
}


void CImPro_Library::J_Rotate_PRE(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num)
{
	if (angle == 0)
	{
		Mat dst = Mat(src.size(), src.type());
		src.copyTo(dst);
		//dst = src.clone();
	}
	//int len = std::max(src.cols, src.rows);
	cv::Point2f pt(src.cols/2, src.rows/2);
	//cv::Point2f pt(BOLT_Param[Cam_num].nRect[0].x, BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height);
	cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_WRAP,CV_RGB(255,255,255));
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_WRAP,CV_RGB(255,255,255));
	}
}

void CImPro_Library::drawArrow(Mat &image, CvPoint p, CvPoint q, CvScalar color, int arrowMagnitude, int thickness, int line_type, int shift) 
{
	//Draw the principle line
	line(image,p, q, color, thickness, line_type, shift);
	//cvLine(image, p, q, color, thickness, line_type, shift);
	//compute the angle alpha
	double angle = atan2((double)p.y-q.y, (double)p.x-q.x);
	//compute the coordinates of the first segment
	p.x = (int) ( q.x +  arrowMagnitude * cos(angle + PI/4));
	p.y = (int) ( q.y +  arrowMagnitude * sin(angle + PI/4));
	//Draw the first segment
	line(image, p, q, color, thickness, line_type, shift);
	//compute the coordinates of the second segment
	p.x = (int) ( q.x +  arrowMagnitude * cos(angle - PI/4));
	p.y = (int) ( q.y +  arrowMagnitude * sin(angle - PI/4));
	//Draw the second segment
	line(image, p, q, color, thickness, line_type, shift);
}


void CImPro_Library::J_FastMatchTemplate(Mat& srca,  // The reference image
	Mat& srcb,  // The template image
	Mat& dst    // Template matching result
	)
{
	if (srca.rows < srcb.rows || srca.cols < srcb.cols)
	{
		return;
	}
	int maxlevel = 2;
	int match_method = CV_TM_CCOEFF_NORMED;
	vector<Mat> refs, tpls, results;

	// Build Gaussian pyramid
	buildPyramid(srca, refs, maxlevel);
	buildPyramid(srcb, tpls, maxlevel);

	Mat ref, tpl, res;

	// Process each level
	for (int level = maxlevel; level >= 0; level--)
	{
		ref = refs[level];
		tpl = tpls[level];
		res = Mat::zeros(ref.size() + cv::Size(1,1) - tpl.size(), CV_32FC1);

		if (level == maxlevel)
		{
			// On the smallest level, just perform regular template matching
			matchTemplate(ref, tpl, res, match_method);
		}
		else
		{
			// On the next layers, template matching is performed on pre-defined 
			// ROI areas.  We define the ROI using the template matching result 
			// from the previous layer.

			Mat mask;
			pyrUp(results.back(), mask);

			Mat mask8u;
			mask.convertTo(mask8u, CV_8U);

			// Find matches from previous layer
			vector<vector<Point> > contours;
			findContours(mask8u, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);

			// Use the contours to define region of interest and 
			// perform template matching on the areas
			if (contours.size() != 0)
			{
				for (int i = 0; i < contours.size(); i++)
				{
					Rect r = boundingRect(contours[i]);
					matchTemplate(
						ref(r + (tpl.size() - Size(1,1))), 
						tpl, 
						res(r), 
						match_method
						);
				}
			}
		}

		// Only keep good matches
		threshold(res, res, 0.5, 1., CV_THRESH_TOZERO);
		results.push_back(res);
	}

	res.copyTo(dst);
}

Point2f CImPro_Library::draw_warped_roi(Mat& image, const int width, const int height, Mat& W, int m_left, int m_top, int Cam_num)
{
	Point2f top_left, top_right, bottom_left, bottom_right;

	Mat  H = Mat (3, 1, CV_32F);
	Mat  U = Mat (3, 1, CV_32F);

	Mat warp_mat = Mat::eye (3, 3, CV_32F);

	for (int y = 0; y < W.rows; y++)
		for (int x = 0; x < W.cols; x++)
			warp_mat.at<float>(y,x) = W.at<float>(y,x);

	//warp the corners of rectangle

	// top-left
	HOMO_VECTOR(H, 0, 0);
	gemm(warp_mat, H, 1, 0, 0, U);
	GET_HOMO_VALUES(U, top_left.x, top_left.y);

	// top-right
	HOMO_VECTOR(H, width, 0);
	gemm(warp_mat, H, 1, 0, 0, U);
	GET_HOMO_VALUES(U, top_right.x, top_right.y);

	// bottom-left
	HOMO_VECTOR(H, 0, height);
	gemm(warp_mat, H, 1, 0, 0, U);
	GET_HOMO_VALUES(U, bottom_left.x, bottom_left.y);

	// bottom-right
	HOMO_VECTOR(H, width, height);
	gemm(warp_mat, H, 1, 0, 0, U);
	GET_HOMO_VALUES(U, bottom_right.x, bottom_right.y);

	top_left.x += m_left;top_left.y += m_top;
	top_right.x += m_left;top_right.y += m_top;
	bottom_right.x += m_left;bottom_right.y += m_top;
	bottom_left.x += m_left;bottom_left.y += m_top;
	//// draw the warped perimeter
	//line(image, top_left, top_right, Scalar(255,0,255));
	//line(image, top_right, bottom_right, Scalar(255,0,255));
	//line(image, bottom_right, bottom_left, Scalar(255,0,255));
	//line(image, bottom_left, top_left, Scalar(255,0,255));

	line(Dst_Img[Cam_num], top_left, top_right, Scalar(255,0,255));
	line(Dst_Img[Cam_num], top_right, bottom_right, Scalar(255,0,255));
	line(Dst_Img[Cam_num], bottom_right, bottom_left, Scalar(255,0,255));
	line(Dst_Img[Cam_num], bottom_left, top_left, Scalar(255,0,255));

	//if (Cam_num==0)
	//{

	//	//Cam0_Result_Data[Seq_num].Format("LT_%1.0f_%1.0f_LB_%1.0f_%1.0f_RT_%1.0f_%1.0f_RB_%1.0f_%1.0f",
	//	//	top_left.x,top_left.y,
	//	//	bottom_left.x,bottom_left.y,
	//	//	top_right.x,top_right.y,
	//	//	bottom_right.x,bottom_right.y
	//	//	);
	//}
	//else if (Cam_num==1)
	//{
	//	//Cam1_Result_Data[Seq_num].Format("LT_%1.0f_%1.0f_LB_%1.0f_%1.0f_RT_%1.0f_%1.0f_RB_%1.0f_%1.0f",
	//	//	top_left.x,top_left.y,
	//	//	bottom_left.x,bottom_left.y,
	//	//	top_right.x,top_right.y,
	//	//	bottom_right.x,bottom_right.y
	//	//	);
	//}
	//else if (Cam_num==2)
	//{
	//	//Cam2_Result_Data[Seq_num].Format("LT_%1.0f_%1.0f_LB_%1.0f_%1.0f_RT_%1.0f_%1.0f_RB_%1.0f_%1.0f",
	//	//	top_left.x,top_left.y,
	//	//	bottom_left.x,bottom_left.y,
	//	//	top_right.x,top_right.y,
	//	//	bottom_right.x,bottom_right.y
	//	//	);
	//}
	Point2f R(1+(top_left.x + top_right.x + bottom_right.x + bottom_left.x)/4,1+(top_left.y + top_right.y + bottom_right.y + bottom_left.y)/4);
	return R;
}

Vec3f rotationMatrixToEulerAngles(Mat &R)
{
    float sy = sqrt(R.at<double>(0,0) * R.at<double>(0,0) +  R.at<double>(1,0) * R.at<double>(1,0) );
 
    bool singular = sy < 1e-6; // If
 
    float x, y, z;
    if (!singular)
    {
        x = fmod(atan2(R.at<double>(2,1) , R.at<double>(2,2))/M_PI*180,360);
        y = fmod(atan2(-R.at<double>(2,0), (double)sy)/M_PI*180,360);
		z = fmod(atan2(R.at<double>(1,0), R.at<double>(0,0))/M_PI*180,360);
    }
    else
    {
        x = atan2(-R.at<double>(1,2), R.at<double>(1,1));
        y = atan2(-R.at<double>(2,0), (double)sy);
        z = 0;
    }
    return Vec3f(x, y, z);   
}

Point2f CImPro_Library::J_Model_Find(int Cam_num)
{
	bool t_license = Easy::CheckLicense(LicenseFeatures::EasyFind);
	if (t_license == false)
	{// 유레시스 없으면
		CString msg;
		int t_first_resize_rate = 8;
		int t_second_resize_rate = 1;

		float c_angle = 0;float c_scale = 1;float c_score = 0;
		Point2f R_Center(-1,-1);

		//Mat cp_Cam_Img = Mat::zeros(Size(Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).size().width/t_first_resize_rate,Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).size().height/t_first_resize_rate), CV_8UC1);
		//Mat cp_Tem_Img = Mat::zeros(Size(Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).size().width/t_first_resize_rate,Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).size().height/t_first_resize_rate), CV_8UC1);
		//resize(Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),cp_Cam_Img,cp_Cam_Img.size(),0,0,INTER_LINEAR);
		Mat cp_Cam_Img = Mat::zeros(Size(Gray_Img[Cam_num].size().width/t_first_resize_rate,Gray_Img[Cam_num].size().height/t_first_resize_rate), CV_8UC1);
		Mat cp_Tem_Img = Mat::zeros(Size(Gray_Img[Cam_num].size().width/t_first_resize_rate,Gray_Img[Cam_num].size().height/t_first_resize_rate), CV_8UC1);
		Mat cut_Tem_Img = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC1);
		Model_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).copyTo(cut_Tem_Img(BOLT_Param[Cam_num].nRect[0]));
		resize(Gray_Img[Cam_num],cp_Cam_Img,cp_Cam_Img.size(),0,0,INTER_LINEAR);
		resize(cut_Tem_Img,cp_Tem_Img,cp_Tem_Img.size(),0,0,INTER_LINEAR);
		//imwrite("01.bmp",cp_Cam_Img);
		//imwrite("00.bmp",cp_Tem_Img);
		Rect t_ROI(t_first_resize_rate/2+BOLT_Param[Cam_num].nRect[0].x/t_first_resize_rate,t_first_resize_rate/2+BOLT_Param[Cam_num].nRect[0].y/t_first_resize_rate,-t_first_resize_rate+BOLT_Param[Cam_num].nRect[0].width/t_first_resize_rate,-t_first_resize_rate+BOLT_Param[Cam_num].nRect[0].height/t_first_resize_rate);
		RotatedRect rr = LogPolarFFTTemplateMatch(cp_Cam_Img, cp_Tem_Img, t_ROI, 50, 150);

		Point2f rect_points[4];
		rr.points(rect_points);
		//R_Center.x = rr.center.x*t_first_resize_rate + BOLT_Param[Cam_num].nRect[0].x;
		//R_Center.y = rr.center.y*t_first_resize_rate + BOLT_Param[Cam_num].nRect[0].y;
		R_Center.x = rr.center.x*t_first_resize_rate;// + BOLT_Param[Cam_num].nRect[0].x;
		R_Center.y = rr.center.y*t_first_resize_rate;// + BOLT_Param[Cam_num].nRect[0].y;
		//for (int j = 0; j < 4; j++)
		//{
		//	rect_points[j].x*=t_first_resize_rate;
		//	rect_points[j].y*=t_first_resize_rate;
		//}
		//for (int j = 0; j < 4; j++)
		//{
		//	line(Dst_Img[Cam_num], rect_points[j], rect_points[(j + 1) % 4], Scalar(255, 0, 0), 2, CV_AA);
		//}

		//return R_Center;

		c_angle = rr.angle;
		Mat warp_mat = cv::getRotationMatrix2D(R_Center, c_angle, c_scale);
		Mat warp_img;
		Mat temp_img = Gray_Img[Cam_num].clone();
		warpAffine( temp_img, warp_img, warp_mat, Gray_Img[Cam_num].size(),INTER_CUBIC, BORDER_CONSTANT,CV_RGB(255,255,255));
		BOLT_Param[Cam_num].Gray_Obj_Img = Mat::zeros(temp_img.size(), CV_8UC1);
/*
		int offsetx =  BOLT_Param[Cam_num].nRect[0].x+BOLT_Param[Cam_num].nRect[0].width/2 - R_Center.x;
		int offsety = BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height/2 - R_Center.y;	*/	
		int offsetx =  Gray_Img[Cam_num].size().width/2 - R_Center.x;
		int offsety = Gray_Img[Cam_num].size().height/2 - R_Center.y;

		if (max(abs(offsetx), abs(offsety)) > 0)
		{
			Mat trans_mat = (Mat_<double>(2,3) << 1, 0, offsetx, 0, 1, offsety);
			warpAffine(warp_img,BOLT_Param[Cam_num].Gray_Obj_Img,trans_mat,BOLT_Param[Cam_num].Gray_Obj_Img.size(),INTER_CUBIC, BORDER_CONSTANT,CV_RGB(255,255,255));
			R_Center.x += offsetx;
			R_Center.y += offsety;
		}
		else
		{
			BOLT_Param[Cam_num].Gray_Obj_Img = warp_img.clone();
		}

		// 여기에서 이미지 Registration 한번 더

		int mode_temp;
		if (warpType == "translation")
			mode_temp = MOTION_TRANSLATION;
		else if (warpType == "euclidean")
			mode_temp = MOTION_EUCLIDEAN;
		else if (warpType == "affine")
			mode_temp = MOTION_AFFINE;
		else
			mode_temp = MOTION_HOMOGRAPHY;
		const int warp_mode = mode_temp;


		// initialize or load the warp matrix
		Mat warp_matrix;
		if (warpType == "homography")
			warp_matrix = Mat::eye(3, 3, CV_32F);
		else
			warp_matrix = Mat::eye(2, 3, CV_32F);

		if (warp_mode != MOTION_HOMOGRAPHY)
			warp_matrix.rows = 2;

		//Rect match_Roi(R_Center.x-Model_Img[Cam_num].cols/2,R_Center.y-Model_Img[Cam_num].rows/2,Model_Img[Cam_num].cols,Model_Img[Cam_num].rows);
		Rect match_Roi = BOLT_Param[Cam_num].nRect[0];
		Mat cp_Cam_Img2 = Mat::zeros(Size(match_Roi.size().width/t_second_resize_rate,match_Roi.size().height/t_second_resize_rate), CV_8UC1);
		Mat cp_Tem_Img2 = Mat::zeros(Size(match_Roi.size().width/t_second_resize_rate,match_Roi.size().height/t_second_resize_rate), CV_8UC1);
		resize(BOLT_Param[Cam_num].Gray_Obj_Img(match_Roi),cp_Cam_Img2,cp_Cam_Img2.size(),0,0,INTER_LINEAR);
		resize(Model_Img[Cam_num](match_Roi),cp_Tem_Img2,cp_Tem_Img2.size(),0,0,INTER_LINEAR);

		Canny(cp_Cam_Img2, cp_Cam_Img2, 90, 200); // you can change this
		//imwrite("00.bmp",cp_Cam_Img2);
		//cp_Cam_Img2.convertTo(cp_Cam_Img2, CV_32FC1, 1.0 / 255.0);
		Canny(cp_Tem_Img2, cp_Tem_Img2, 90, 200); // you can change this
		//imwrite("01.bmp",cp_Tem_Img2);
		//cp_Tem_Img2.convertTo(cp_Tem_Img2, CV_32FC1, 1.0 / 255.0);

		//Mat target_image = BOLT_Param[Cam_num].Gray_Obj_Img(match_Roi).clone();

		//Rect t_ROI2(0,0,cp_Tem_Img2.size().width,cp_Tem_Img2.size().height);
		//RotatedRect rr2 = LogPolarFFTTemplateMatch(cp_Cam_Img2, cp_Tem_Img2, t_ROI2, 100, 200);

		//rr2.points(rect_points);
		//R_Center.x = rr2.center.x*t_second_resize_rate + BOLT_Param[Cam_num].nRect[0].x;
		//R_Center.y = rr2.center.y*t_second_resize_rate + BOLT_Param[Cam_num].nRect[0].y;

		//c_angle = rr2.angle;
		//warp_mat = cv::getRotationMatrix2D(R_Center, c_angle, c_scale);
		//warpAffine( BOLT_Param[Cam_num].Gray_Obj_Img, BOLT_Param[Cam_num].Gray_Obj_Img, warp_mat, Gray_Img[Cam_num].size(),INTER_CUBIC, BORDER_CONSTANT,CV_RGB(255,255,255));
		//Dst_Img[Cam_num] = Mat::zeros(BOLT_Param[Cam_num].Gray_Obj_Img.size(), CV_8UC3);
		//cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

		//		//imwrite("02.bmp",BOLT_Param[Cam_num].Gray_Obj_Img(match_Roi));
		//circle(Dst_Img[Cam_num], R_Center,3,CV_RGB(0,0,255),1);
		//circle(Dst_Img[Cam_num], R_Center,1,CV_RGB(255,0,0),1);

		//msg.Format("No find tool");
		//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, 2 + Dst_Img[Cam_num].rows/50), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/1500, CV_RGB(0,200,255), (double)Dst_Img[Cam_num].rows/960, 8);
		//msg.Format("(Cx,Cy),Angle,Score=(%1.3f,%1.3f),%1.3f,%1.2f",R_Center.x, R_Center.y, c_angle, c_score*100);
		//MatchRate[Cam_num] = c_score*100;
		//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, Dst_Img[Cam_num].rows - Dst_Img[Cam_num].rows/50 +2), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/2000, CV_RGB(255,180,0), (double)Dst_Img[Cam_num].rows/960, 8);


		//return R_Center;

		double cc = findTransformECC (cp_Tem_Img2, cp_Cam_Img2, warp_matrix, warp_mode,
			TermCriteria (TermCriteria::COUNT+TermCriteria::EPS,
			number_of_iterations, termination_eps));

		if (cc == -1)
		{
			BOLT_Param[Cam_num].Object_Postion = Point(-1,-1);
			return Point2f(-1,-1);
		}

		c_score = cc;
		R_Center.x = match_Roi.x+match_Roi.width/2;
		R_Center.y = match_Roi.y+match_Roi.height/2;
		//R_Center = draw_warped_roi(BOLT_Param[Cam_num].Gray_Obj_Img, Model_Img[Cam_num].cols-2, Model_Img[Cam_num].rows-2, warp_matrix, R_Center.x-match_Roi.width/2, R_Center.y-match_Roi.height/2,Cam_num);

		warpAffine( BOLT_Param[Cam_num].Gray_Obj_Img, BOLT_Param[Cam_num].Gray_Obj_Img, warp_matrix, Gray_Img[Cam_num].size(),1, BORDER_WRAP,CV_RGB(255,255,255));
		Dst_Img[Cam_num] = Mat::zeros(BOLT_Param[Cam_num].Gray_Obj_Img.size(), CV_8UC3);
		cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

		//imwrite("02.bmp",BOLT_Param[Cam_num].Gray_Obj_Img(match_Roi));
		circle(Dst_Img[Cam_num], R_Center,3,CV_RGB(0,0,255),1);
		circle(Dst_Img[Cam_num], R_Center,1,CV_RGB(255,0,0),1);

		msg.Format("No find tool");
		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, 2 + Dst_Img[Cam_num].rows/50), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/1500, CV_RGB(0,200,255), (double)Dst_Img[Cam_num].rows/960, 8);
		msg.Format("(Cx,Cy),Angle,Score=(%1.3f,%1.3f),%1.3f,%1.2f",R_Center.x, R_Center.y, c_angle, c_score*100);
		MatchRate[Cam_num] = c_score*100;
		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, Dst_Img[Cam_num].rows - Dst_Img[Cam_num].rows/50 +2), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/2000, CV_RGB(255,180,0), (double)Dst_Img[Cam_num].rows/960, 8);

		return R_Center;

		//const double tic_init = (double) getTickCount();

		//CString msg;

		//int mode_temp;
		//if (warpType == "translation")
		//	mode_temp = MOTION_TRANSLATION;
		//else if (warpType == "euclidean")
		//	mode_temp = MOTION_EUCLIDEAN;
		//else if (warpType == "affine")
		//	mode_temp = MOTION_AFFINE;
		//else
		//	mode_temp = MOTION_HOMOGRAPHY;

		//Mat cp_Cam_Img = Gray_Img[Cam_num].clone();
		////Canny(Gray_Img[Cam_num],cp_Cam_Img,30,90);
		//// Similarity 계산
		//Mat ROI_Template_R_Img;
		////AfxMessageBox("0");
		//J_FastMatchTemplate(cp_Cam_Img, Model_Img[Cam_num], ROI_Template_R_Img);
		////AfxMessageBox("end");
		//Point matchLoc;
		//double minval, maxval;				// Matching된 Similarity 최소, 최대값
		//cv::Point minLoc, maxLoc;
		//cv::minMaxLoc(ROI_Template_R_Img, &minval, &maxval, &minLoc, &maxLoc);
		//matchLoc = maxLoc;					// Matching된 좌,상점

		//Rect match_Roi(matchLoc.x,matchLoc.y,Model_Img[Cam_num].cols,Model_Img[Cam_num].rows);
		//Mat target_image = cp_Cam_Img(match_Roi).clone();
		////matchLoc.x = cp_Cam_Img.cols/2 - Model_Img[Cam_num].cols/2;
		////matchLoc.y = cp_Cam_Img.rows/2 - Model_Img[Cam_num].rows/2;
		////Rect match_Roi(matchLoc.x,matchLoc.y,Model_Img[Cam_num].cols,Model_Img[Cam_num].rows);
		////Mat target_image = cp_Cam_Img(match_Roi).clone();
		//const int warp_mode = mode_temp;

		//// initialize or load the warp matrix
		//Mat warp_matrix;
		//if (warpType == "homography")
		//	warp_matrix = Mat::eye(3, 3, CV_32F);
		//else
		//	warp_matrix = Mat::eye(2, 3, CV_32F);

		//if (warp_mode != MOTION_HOMOGRAPHY)
		//	warp_matrix.rows = 2;

		//double cc = findTransformECC (Model_Img[Cam_num], target_image, warp_matrix, warp_mode,
		//	TermCriteria (TermCriteria::COUNT+TermCriteria::EPS,
		//	number_of_iterations, termination_eps));

		//if (cc == -1)
		//{
		//	BOLT_Param[Cam_num].Object_Postion = Point(-1,-1);
		//	return Point2f(-1,-1);
		//}
		//// draw boundaries of corresponding regions
		//Mat identity_matrix = Mat::eye(3,3,CV_32F);

		////AfxMessageBox("1");
		//Point2f R_Center = draw_warped_roi(cp_Cam_Img, Model_Img[Cam_num].cols-2, Model_Img[Cam_num].rows-2, warp_matrix, matchLoc.x, matchLoc.y,Cam_num);

		//Vec3f t_vec = rotationMatrixToEulerAngles(warp_matrix);
		//warpAffine( Gray_Img[Cam_num], BOLT_Param[Cam_num].Gray_Obj_Img, warp_matrix, Gray_Img[Cam_num].size(),1, BORDER_TRANSPARENT,CV_RGB(255,255,255));
		//Dst_Img[Cam_num] = Mat::zeros(BOLT_Param[Cam_num].Gray_Obj_Img.size(), CV_8UC3);
		//cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

		//msg.Format("No find tool");
		//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, 2 + Dst_Img[Cam_num].rows/50), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/1500, CV_RGB(0,100,255), (double)Dst_Img[Cam_num].rows/960, 8);

		////R_Center.x = (double)matchLoc.x + (double)BOLT_Param[Cam_num].nRect[0].x;
		////R_Center.y = (double)matchLoc.y + (double)BOLT_Param[Cam_num].nRect[0].y;

		//// end timing
		//const double toc_final  = (double) getTickCount ();
		//const double total_time = (toc_final-tic_init)/(getTickFrequency());
		//if (cc >= 0.05)
		//{
		//	msg.Format("(Cx,Cy),Angle,Score=(%1.3f,%1.3f),%1.3f,%1.2f",R_Center.x, R_Center.y, t_vec[0], cc*100);
		//	MatchRate[Cam_num] = cc*100;
		//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, Dst_Img[Cam_num].rows - Dst_Img[Cam_num].rows/50 +2), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/2000, CV_RGB(255,180,0), (double)Dst_Img[Cam_num].rows/960, 8);

		//	//msg.Format("Center_%1.5f_%1.5f_TT_%1.3f_Error_%1.3f",R_Center.x,R_Center.y,total_time,cc);
		//	circle(Dst_Img[Cam_num], R_Center,3,CV_RGB(0,0,255),1);
		//	circle(Dst_Img[Cam_num], R_Center,1,CV_RGB(255,0,0),1);
		//} else
		//{
		//	msg.Format("(Cx,Cy),Angle,Score=(%1.3f,%1.3f),%1.3f,%1.2f",R_Center.x, R_Center.y, t_vec[0], cc*100);
		//	MatchRate[Cam_num] = cc*100;
		//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, Dst_Img[Cam_num].rows - Dst_Img[Cam_num].rows/50 +2), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/2000, CV_RGB(255,180,0), (double)Dst_Img[Cam_num].rows/960, 8);
		//	//msg.Format("Center_-1_-1_TT_%1.3f_Error_%1.3f_%s",total_time,cc,"");
		//	//Cam0_Result_Data[Seq_num].Format("Center_%1.3f_%1.3f_TT_%1.3f_Error_%1.3f_%s",-1,-1,total_time,cc,Cam0_Result_Data[Seq_num]);
		//}
		//return R_Center;
	}
	else
	{// 유레시스 있으면
		float c_x = -1;float c_y = -1;float c_angle = 0;float c_scale = 0;float c_score = 0;
		//AfxMessageBox("1");
		if (m_Cam_Find[Cam_num].GetLearningDone())
		{	
			//AfxMessageBox("2");
			Mat temp_img = Gray_Img[Cam_num].clone();
			EBW8Image1[Cam_num].SetImagePtr(temp_img.cols,temp_img.rows,temp_img.data);
			m_Cam_FoundPattern[Cam_num].clear();
			m_Cam_FoundPattern[Cam_num] = m_Cam_Find[Cam_num].Find(&EBW8Image1[Cam_num]);
			MatchRate[Cam_num] = 0;
			if (m_Cam_FoundPattern[Cam_num].size() > 0)
			{
				//AfxMessageBox("3");
				c_x = m_Cam_FoundPattern[Cam_num][0].GetCenter().GetX();
				c_y = m_Cam_FoundPattern[Cam_num][0].GetCenter().GetY();
				c_angle = m_Cam_FoundPattern[Cam_num][0].GetAngle();
				c_scale = m_Cam_FoundPattern[Cam_num][0].GetScale();
				c_score = m_Cam_FoundPattern[Cam_num][0].GetScore();
				if (m_Text_View[Cam_num] && !ROI_Mode) // ROI 영역 표시
				{
					circle(Dst_Img[Cam_num],Point2f(c_x,c_y),2,CV_RGB(255,0,0),2);
					circle(Dst_Img[Cam_num],Point2f(c_x,c_y),1,CV_RGB(0,0,255),1);
				}
				//if (c_angle < -180)
				//{
				//	c_angle += 360;
				//}
				//if (c_angle > 180)
				//{
				//	c_angle -= 360;
				//}
				if (c_score > 0.999f)
				{
					BOLT_Param[Cam_num].Gray_Obj_Img = temp_img.clone();
				}
				else
				{
					Mat warp_mat = cv::getRotationMatrix2D(Point2f(c_x,c_y), c_angle, c_scale);
					Mat warp_img;
					warpAffine( temp_img, warp_img, warp_mat, Gray_Img[Cam_num].size(),INTER_CUBIC, BORDER_CONSTANT,CV_RGB(255,255,255));
					BOLT_Param[Cam_num].Gray_Obj_Img = Mat::zeros(temp_img.size(), CV_8UC1);

					int offsetx =  BOLT_Param[Cam_num].Object_Postion.x - c_x;
					int offsety = BOLT_Param[Cam_num].Object_Postion.y - c_y;

					if (max(abs(offsetx), abs(offsety)) > 10)
					{
						Mat trans_mat = (Mat_<double>(2,3) << 1, 0, offsetx, 0, 1, offsety);
						warpAffine(warp_img,BOLT_Param[Cam_num].Gray_Obj_Img,trans_mat,BOLT_Param[Cam_num].Gray_Obj_Img.size(),INTER_CUBIC, BORDER_CONSTANT,CV_RGB(255,255,255));
						c_x += offsetx;
						c_y += offsety;
					}
					else
					{
						BOLT_Param[Cam_num].Gray_Obj_Img = warp_img.clone();
					}
					//warp_mat.at<float>(0,2) -= -c_x + BOLT_Param[Cam_num].Object_Postion.x;
					//warp_mat.at<float>(1,2) -= -c_y + BOLT_Param[Cam_num].Object_Postion.y;
					//c_x -= warp_mat.at<float>(0,2);
					//c_y -= warp_mat.at<float>(1,2);
				}


				Dst_Img[Cam_num] = Mat::zeros(BOLT_Param[Cam_num].Gray_Obj_Img.size(), CV_8UC3);
				cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num],CV_GRAY2BGR);
				CString msg;
				msg.Format("Find tool");
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, 2 + Dst_Img[Cam_num].rows/50), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/1500, CV_RGB(0,200,255), (double)Dst_Img[Cam_num].rows/960, 8);
				msg.Format("(Cx,Cy),Angle,Score=(%1.3f,%1.3f),%1.3f,%1.2f",c_x, c_y, c_angle, c_score*100);
				MatchRate[Cam_num] = c_score*100;
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(5, Dst_Img[Cam_num].rows - Dst_Img[Cam_num].rows/50 +2), FONT_HERSHEY_SIMPLEX, (double)Dst_Img[Cam_num].rows/2000, CV_RGB(255,180,0), (double)Dst_Img[Cam_num].rows/960, 8);

				////warp_mat = cv::getRotationMatrix2D(Point2f(c_x,c_y), 10, 1);
				////Mat r_img = temp_img.clone();
				////warpAffine( temp_img, r_img, warp_mat, Gray_Img[Cam_num].size(),1, BORDER_TRANSPARENT,CV_RGB(255,255,255));
				//////imwrite("00.bmp",r_img);

				////msg.Format("00_%1.3f_%1.3f.bmp",c_angle,c_scale);
				////imwrite(msg.GetBuffer(),r_img);
				//CString msg;
				//msg.Format("%d_Cx=%1.3f,Cy=%1.3f,Ca=%1.3f,Cs=%1.3f",Cam_num,c_x,c_y,c_angle,c_scale);
				//AfxMessageBox(msg);
				//imwrite("01_0.bmp",temp_img);			
				//imwrite("01_1.bmp",Gray_Img[Cam_num]);			
				//imwrite("01_2.bmp",Template_Img[Cam_num]);			
				//E_Cam_Img[Cam_num].Save("00_1.bmp");
			}
			else
			{
				BOLT_Param[Cam_num].Gray_Obj_Img = temp_img.clone();
			}
		}
		return Point2f(c_x,c_y);
	}

	return Point2f(-1, -1);
}

//#pragma endregion

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
// [함수] CAM 검사 함수
// 개발자 : CDJung
// 마지막 수정 : 2017.04.10
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////
bool CImPro_Library::RUN_Algorithm_CAM(int Cam_num)
{
	//if (Alg_Run_Check[Cam_num])
	//{
	//	//AfxMessageBox("1");
	//	//t_Alg_Run_Check_retun_cnt[Cam_num]++;
	//	//if (t_Alg_Run_Check_retun_cnt[Cam_num] > 1)
	//	//{
	//		t_Alg_Run_Check_retun_cnt[Cam_num] = 0;
	//		Alg_Run_Check[Cam_num] = false;
	//	//}
	//	Result_Info[Cam_num] = "";
	//	for (int ss=1;ss<41;ss++)
	//	{
	//		Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num], Cam_num, ss);
	//	}
	//	return true;
	//}

	Alg_Run_Check[Cam_num] = true;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";
		CString msg;
		//if (Cam_num == 0 && BOLT_Param[Cam_num].nTableType == 5 && BOLT_Param[Cam_num].nROI0_FilterSize[0] == 7496)
		//{
		//	RUN_Algorithm_SUGIYAMA();
		//	Alg_Run_Check[Cam_num] = false;
		//	return true;
		//}
		//msg.Format("CAM%1d_Gray_Img.jpg",Cam_num);
		//imwrite(msg.GetBuffer(),Gray_Img[Cam_num]);
		// 변수 설정
		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;
		Mat Out_binary;
		Mat Out_binary_Tmp;
		Mat CP_Gray_Img;
		for (int s=0;s<41;s++)
		{
			if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num != s && s > 0)
			{
				continue;
			}
			contours.clear();hierarchy.clear();
			//if (BOLT_Param[Cam_num].nRect[s].width%4 == 1)
			//{
			//	BOLT_Param[Cam_num].nRect[s].width -= 1;
			//}
			//else if (BOLT_Param[Cam_num].nRect[s].width%4 == 2)
			//{
			//	BOLT_Param[Cam_num].nRect[s].x += 1;
			//	BOLT_Param[Cam_num].nRect[s].width -= 2;
			//}
			//else if (BOLT_Param[Cam_num].nRect[s].width%4 == 3)
			//{
			//	BOLT_Param[Cam_num].nRect[s].width += 1;
			//}
			//if (BOLT_Param[Cam_num].nRect[s].height%4 == 1)
			//{
			//	BOLT_Param[Cam_num].nRect[s].height -= 1;
			//}
			//else if (BOLT_Param[Cam_num].nRect[s].height%4 == 2)
			//{
			//	BOLT_Param[Cam_num].nRect[s].y += 1;
			//	BOLT_Param[Cam_num].nRect[s].height -= 2;
			//}
			//else if (BOLT_Param[Cam_num].nRect[s].height%4 == 3)
			//{
			//	BOLT_Param[Cam_num].nRect[s].height += 1;
			//}
			// 설정된 ROI Backup
			BOLT_Param[Cam_num].nRect_Backup[s] = BOLT_Param[Cam_num].nRect[s];

			// 설정된 ROI 유효성 검사
			if (BOLT_Param[Cam_num].nUse[s] == 0 || BOLT_Param[Cam_num].nRect[s].width <= 0 || BOLT_Param[Cam_num].nRect[s].height <= 0
				|| BOLT_Param[Cam_num].nRect[s].width > Gray_Img[Cam_num].cols || BOLT_Param[Cam_num].nRect[s].height > Gray_Img[Cam_num].rows || 
				Template_Img[Cam_num].rows != Gray_Img[Cam_num].rows || Template_Img[Cam_num].cols != Gray_Img[Cam_num].cols)
			{
				if (s > 0)
				{
					CString t_CString;
					t_CString.Format("C%d:%02d_-2_",Cam_num,s);
					Result_Info[Cam_num] += t_CString;
					//Result_Info[Cam_num].Format("%sC%d:%02d_-2_",Result_Info[Cam_num],Cam_num,s);
					continue;
				}
				//else if (s == 0)
				//{
				//	Result_Info[Cam_num] = "";
				//	for (int ss=1;ss<41;ss++)
				//	{
				//		CString t_CString;
				//		t_CString.Format("C%d:%02d_-2_",Cam_num,ss);
				//		Result_Info[Cam_num] += t_CString;
				//		//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num], Cam_num, ss);
				//	}
				//	return true;
				//}
			}

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [시작] ROI 설정 및 이미지 자르기
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			Rect tROI = BOLT_Param[Cam_num].nRect[s];
			if (s == 0)
			{
				bool t_ROI_Check = true;
				if (tROI.x < 0)
				{
					tROI.x = 0;
					t_ROI_Check = false;
				}
				if (tROI.y < 0)
				{
					tROI.y = 0;
					t_ROI_Check = false;
				}
				if (tROI.x + tROI.width > Gray_Img[Cam_num].cols)
				{
					tROI.width = Gray_Img[Cam_num].cols - tROI.x -1;
					t_ROI_Check = false;
				}
				if (tROI.y + tROI.height > Gray_Img[Cam_num].rows)
				{
					tROI.height = Gray_Img[Cam_num].rows - tROI.y -1;
					t_ROI_Check = false;
				}

				if (!t_ROI_Check)
				{
					rectangle(Dst_Img[Cam_num],tROI,CV_RGB(0,255,0),1);
					msg.Format("ROI#%d", s);
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(tROI.x+1,tROI.y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);

					rectangle(Dst_Img[Cam_num](tROI),Rect(10,10,tROI.width-20,tROI.height-20),Scalar(0,0,255),2);
					msg.Format("ROI Error!");
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(tROI.x+tROI.width/3,tROI.y + tROI.height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
					Result_Info[Cam_num] = "";
					for (int ss=1;ss<41;ss++)
					{
						CString t_CString;
						t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
						Result_Info[Cam_num] += t_CString;
						//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num], Cam_num, ss);
					}
					Alg_Run_Check[Cam_num] = false;
					return true;

				}

				//if (BOLT_Param[Cam_num].nRect[0].width%4 == 1)
				//{
				//	BOLT_Param[Cam_num].nRect[0].width -= 1;
				//}
				//else if (BOLT_Param[Cam_num].nRect[0].width%4 == 2)
				//{
				//	BOLT_Param[Cam_num].nRect[0].x += 1;
				//	BOLT_Param[Cam_num].nRect[0].width -= 2;
				//}
				//else if (BOLT_Param[Cam_num].nRect[0].width%4 == 3)
				//{
				//	BOLT_Param[Cam_num].nRect[0].width += 1;
				//}
				//if (BOLT_Param[Cam_num].nRect[0].height%4 == 1)
				//{
				//	BOLT_Param[Cam_num].nRect[0].height -= 1;
				//}
				//else if (BOLT_Param[Cam_num].nRect[0].height%4 == 2)
				//{
				//	BOLT_Param[Cam_num].nRect[0].y += 1;
				//	BOLT_Param[Cam_num].nRect[0].height -= 2;
				//}
				//else if (BOLT_Param[Cam_num].nRect[0].height%4 == 3)
				//{
				//	BOLT_Param[Cam_num].nRect[0].height += 1;
				//}

				if (BOLT_Param[Cam_num].nUse[0] == 0)
				{
					BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](tROI).clone();
					continue;
				}
				//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num != s && s == 0)
				//{
				//	BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](tROI).clone();
				//	continue;
				//}

				BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
				CP_Gray_Img = Gray_Img[Cam_num](tROI).clone();
				if (BOLT_Param[Cam_num].nMethod_Thres[0] != THRES_METHOD::FIRSTROI) // ROI#01 모델사용
				{
					if (m_Text_View[Cam_num] && !ROI_Mode && BOLT_Param[Cam_num].nCamPosition == 0)
					{
						//rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
						//msg.Format("Obj. ROI");
						//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,50), 1, 8);
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && BOLT_Param[Cam_num].nCamPosition == 0)
					{
						rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
						msg.Format("Obj. ROI");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,50), 1, 8);
					}
				}
			} else
			{
				// ROI를 Offset 이동후 유효영역 절대좌표로 변경
				tROI.x += BOLT_Param[Cam_num].Offset_Object_Postion.x;
				tROI.y += BOLT_Param[Cam_num].Offset_Object_Postion.y;
				if (BOLT_Param[Cam_num].nMethod_Thres[0] != THRES_METHOD::FIRSTROI) // ROI#01 모델사용
				{
					tROI.x -= BOLT_Param[Cam_num].nRect[0].x;
					tROI.y -= BOLT_Param[Cam_num].nRect[0].y;
				}
				tROI = Check_ROI(tROI, BOLT_Param[Cam_num].Gray_Obj_Img.cols, BOLT_Param[Cam_num].Gray_Obj_Img.rows);
				if (tROI.x < 0)
				{
					tROI.x = 1;
				}
				if (tROI.y < 0)
				{
					tROI.y = 1;
				}
				if (tROI.x + tROI.width >= BOLT_Param[Cam_num].Gray_Obj_Img.cols-1)
				{
					tROI.width = BOLT_Param[Cam_num].Gray_Obj_Img.cols - tROI.x -2;
				}
				if (tROI.y + tROI.height >= BOLT_Param[Cam_num].Gray_Obj_Img.rows-1)
				{
					tROI.height = BOLT_Param[Cam_num].Gray_Obj_Img.rows - tROI.y -2;
				}

				CP_Gray_Img = BOLT_Param[Cam_num].Gray_Obj_Img(tROI).clone();
				//imwrite("Gray_Obj_Img.bmp",BOLT_Param[Cam_num].Gray_Obj_Img);
				//imwrite("CP_Gray_Img.bmp",CP_Gray_Img);
				// 그리기위해 ROI 정보 갱신
				BOLT_Param[Cam_num].nRect[s] = tROI;
				if (BOLT_Param[Cam_num].nMethod_Thres[0] != THRES_METHOD::FIRSTROI) // ROI#01 모델사용
				{
					BOLT_Param[Cam_num].nRect[s].x += BOLT_Param[Cam_num].nRect[0].x;
					BOLT_Param[Cam_num].nRect[s].y += BOLT_Param[Cam_num].nRect[0].y;
				}
				BOLT_Param[Cam_num].nRect[s] = Check_ROI(BOLT_Param[Cam_num].nRect[s], Gray_Img[Cam_num].cols, Gray_Img[Cam_num].rows);
			}

			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Gray_Img.bmp",Cam_num,s);
				imwrite(msg.GetBuffer(),CP_Gray_Img);		
			}

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [끝] ROI 설정 및 이미지 자르기
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [시작] 임계화 ALG
			// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			if (BOLT_Param[Cam_num].nBlurFilterCNT[s] > 0)
			{
				blur(CP_Gray_Img,CP_Gray_Img, Size(BOLT_Param[Cam_num].nBlurFilterCNT[s]*2+1,BOLT_Param[Cam_num].nBlurFilterCNT[s]*2+1));
			}
			if (BOLT_Param[Cam_num].nMethod_Direc[s] == 0 || BOLT_Param[Cam_num].nMethod_Direc[s] == 1)
			{
				medianBlur(CP_Gray_Img,CP_Gray_Img,3);
			}
			if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
			} 
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
			} 
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
			{
				inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
			{
				Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
				inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
				subtract(White_Out_binary, Out_binary, Out_binary);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
			{
				blur(CP_Gray_Img, Out_binary, Size(3,3));
				// Run the edge detector on grayscale
				Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::FIRSTROI) // ROI#01 결과 사용
			{
				if (s > 0)
				{
					Out_binary = BOLT_Param[Cam_num].Thres_Obj_Img(tROI).clone();
				} 
				else
				{
					inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
				}
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::DIFFAVG) // 평균기준 차이
			{
				Scalar tempVal = mean( CP_Gray_Img );
				float myMAtMean = tempVal.val[0];
				Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
				inRange(CP_Gray_Img, Scalar(myMAtMean - BOLT_Param[Cam_num].nThres_V1[s]),Scalar(myMAtMean + BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
				subtract(White_Out_binary, Out_binary, Out_binary);
				if (BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_SIZE_TB && BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_COUNT_TB)
				{
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						msg.Format("Avg = %1.3f", myMAtMean);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						msg.Format("Avg = %1.3f", myMAtMean);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
					}
				}
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::COMPARE) // 비교 차이
			{
				bool t_license = Easy::CheckLicense(LicenseFeatures::EasyFind);
				if (t_license == false)
				{// 유레시스 없으면

					//작업중

					Rect comp_rect = BOLT_Param[Cam_num].nRect[0];
					comp_rect.x += BOLT_Param[Cam_num].Offset_Object_Postion.x;
					comp_rect.y += BOLT_Param[Cam_num].Offset_Object_Postion.y;

					Mat bin_dark_model_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bin_dark_camera_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bin_bright_model_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bin_bright_camera_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat dark_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bright_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					//imwrite("00-1.bmp",Model_Img[Cam_num]);
					//imwrite("00-2.bmp",BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect));

					if (Model_Img[Cam_num].size() == BOLT_Param[Cam_num].Gray_Obj_Img.size())
					{
						threshold(Model_Img[Cam_num](comp_rect),bin_dark_model_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(Model_Img[Cam_num](comp_rect),bin_bright_model_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_dark_camera_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_bright_camera_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
					}
					else
					{
						threshold(Model_Img[Cam_num],bin_dark_model_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(Model_Img[Cam_num],bin_bright_model_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_dark_camera_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_bright_camera_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
					}
					//imwrite("bin_dark_model_img.bmp",bin_dark_model_img);
					//imwrite("bin_bright_model_img.bmp",bin_bright_model_img);
					dilate(bin_dark_model_img,bin_dark_model_img,element,Point(-1,-1), 1);
					dilate(bin_bright_model_img,bin_bright_model_img,element,Point(-1,-1), 1);
					//imwrite("bin_dark_model_img_mp.bmp",bin_dark_model_img);
					//imwrite("bin_bright_model_img_mp.bmp",bin_bright_model_img);

					//imwrite("bin_dark_camera_img.bmp",bin_dark_camera_img);
					//imwrite("bin_bright_camera_img.bmp",bin_bright_camera_img);


					subtract(bin_dark_camera_img, bin_dark_model_img,dark_img);
					subtract(bin_bright_camera_img, bin_bright_model_img, bright_img);

					Mat Color_img;
					add(dark_img, bright_img, Color_img);
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num != s)
					{
						cvtColor(Color_img,Dst_Img[Cam_num](comp_rect),CV_GRAY2BGR);
					}

					Rect comp_rect2 = BOLT_Param[Cam_num].nRect[s];
					comp_rect2.x -= BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Offset_Object_Postion.x;
					comp_rect2.y -= BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Offset_Object_Postion.y;

					add(dark_img(comp_rect2),  bright_img(comp_rect2), Out_binary);
					#if 0
					if (Model_Img[Cam_num].size() == BOLT_Param[Cam_num].Gray_Obj_Img.size())
					{
						subtract(Model_Img[Cam_num](comp_rect), BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),dark_img);
						subtract(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),Model_Img[Cam_num](comp_rect), bright_img);
					}
					else
					{
						//AfxMessageBox("2");
						subtract(Model_Img[Cam_num], BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),dark_img);
						//AfxMessageBox("3");
						//imwrite("00.bmp",dark_img);
						subtract(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),Model_Img[Cam_num], bright_img);
						//imwrite("01.bmp",bright_img);
						//AfxMessageBox("4");
					}

					Mat Color_img;
					add(dark_img, bright_img, Color_img);
					cvtColor(Color_img,Dst_Img[Cam_num](comp_rect),CV_GRAY2BGR);

					threshold(dark_img,dark_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY);
					threshold(bright_img,bright_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);

					Rect comp_rect2 = BOLT_Param[Cam_num].nRect[s];
					comp_rect2.x -= BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Offset_Object_Postion.x;
					comp_rect2.y -= BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Offset_Object_Postion.y;

					add(dark_img(comp_rect2),  bright_img(comp_rect2), Out_binary);
					#endif
				}
				else
				{//유레시스 있으면
					//AfxMessageBox("1");
					Rect comp_rect = BOLT_Param[Cam_num].nRect[0];
					comp_rect.x += BOLT_Param[Cam_num].Offset_Object_Postion.x;
					comp_rect.y += BOLT_Param[Cam_num].Offset_Object_Postion.y;

					//CString msgs;
					//msgs.Format("%d,%d,%d,%d / %d,%d,%d,%d",BOLT_Param[Cam_num].nRect[0].x,BOLT_Param[Cam_num].nRect[0].y,BOLT_Param[Cam_num].nRect[0].width,BOLT_Param[Cam_num].nRect[0].height,
					//	comp_rect.x,comp_rect.y,comp_rect.width,comp_rect.height);
					//AfxMessageBox(msgs);
					Mat bin_dark_model_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bin_dark_camera_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bin_bright_model_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bin_bright_camera_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat dark_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					Mat bright_img = Mat::zeros(comp_rect.size(), CV_8UC1);
					//imwrite("00-1.bmp",Model_Img[Cam_num]);
					//imwrite("00-2.bmp",BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect));

					if (Model_Img[Cam_num].size() == BOLT_Param[Cam_num].Gray_Obj_Img.size())
					{
						threshold(Model_Img[Cam_num](comp_rect),bin_dark_model_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(Model_Img[Cam_num](comp_rect),bin_bright_model_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_dark_camera_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_bright_camera_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
					}
					else
					{
						threshold(Model_Img[Cam_num],bin_dark_model_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(Model_Img[Cam_num],bin_bright_model_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_dark_camera_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),bin_bright_camera_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
					}
					//imwrite("bin_dark_model_img.bmp",bin_dark_model_img);
					//imwrite("bin_bright_model_img.bmp",bin_bright_model_img);
					dilate(bin_dark_model_img,bin_dark_model_img,element,Point(-1,-1), 1);
					dilate(bin_bright_model_img,bin_bright_model_img,element,Point(-1,-1), 1);
					//imwrite("bin_dark_model_img_mp.bmp",bin_dark_model_img);
					//imwrite("bin_bright_model_img_mp.bmp",bin_bright_model_img);

					//imwrite("bin_dark_camera_img.bmp",bin_dark_camera_img);
					//imwrite("bin_bright_camera_img.bmp",bin_bright_camera_img);


					subtract(bin_dark_camera_img, bin_dark_model_img,dark_img);
					subtract(bin_bright_camera_img, bin_bright_model_img, bright_img);
					//erode(bin_bright_model_img,bin_bright_model_img,element,Point(-1,-1), 1);
					//imwrite("dark_img.bmp",dark_img);
					//imwrite("bright_img.bmp",bright_img);

					Mat Color_img;
					add(dark_img, bright_img, Color_img);
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num != s)
					{
						cvtColor(Color_img,Dst_Img[Cam_num](comp_rect),CV_GRAY2BGR);
					}

					Rect comp_rect2 = BOLT_Param[Cam_num].nRect[s];
					comp_rect2.x -= BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Offset_Object_Postion.x;
					comp_rect2.y -= BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Offset_Object_Postion.y;

					add(dark_img(comp_rect2),  bright_img(comp_rect2), Out_binary);

#if 0
					if (Model_Img[Cam_num].size() == BOLT_Param[Cam_num].Gray_Obj_Img.size())
					{
						subtract(Model_Img[Cam_num](comp_rect), BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),dark_img);
						subtract(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),Model_Img[Cam_num](comp_rect), bright_img);
					}
					else
					{
						//AfxMessageBox("2");
						subtract(Model_Img[Cam_num], BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),dark_img);
						//AfxMessageBox("3");
						//imwrite("00.bmp",dark_img);
						subtract(BOLT_Param[Cam_num].Gray_Obj_Img(comp_rect),Model_Img[Cam_num], bright_img);
						//imwrite("01.bmp",bright_img);
						//AfxMessageBox("4");
					}



					Mat Color_img;
					add(dark_img, bright_img, Color_img);
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num != s)
					{
						cvtColor(Color_img,Dst_Img[Cam_num](comp_rect),CV_GRAY2BGR);
					}

					threshold(dark_img,dark_img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY);
					threshold(bright_img,bright_img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);

					Rect comp_rect2 = BOLT_Param[Cam_num].nRect[s];
					comp_rect2.x -= BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Offset_Object_Postion.x;
					comp_rect2.y -= BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Offset_Object_Postion.y;

					add(dark_img(comp_rect2),  bright_img(comp_rect2), Out_binary);
#endif
				}
			}
			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Binary_Img.bmp",Cam_num,s);
				imwrite(msg.GetBuffer(),Out_binary);
			}
			J_Delete_Boundary(Out_binary,1);
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [끝] 임계화 ALG
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [시작] ROI Offset 계산
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			if (s == 0)
			{
				if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::ROI_TYPE) //ROI 기준 측정
				{
					BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
					BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
				}
				else
				{
					if (BOLT_Param[Cam_num].nMethod_Thres[0] == THRES_METHOD::FIRSTROI) // ROI#01 모델사용
					{
						//Model_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();
						Point2f R_Center = J_Model_Find(Cam_num);
						C_X[Cam_num] = R_Center.x;
						C_Y[Cam_num] = R_Center.y;
						// 머리를 못 찾을 경우 Error 처리함.
						if(R_Center.x == -1 || R_Center.y == -1)
						{
							rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
							msg.Format("No Object!");
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
							//return true;
						}
						else
						{
							//BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
							BOLT_Param[Cam_num].Offset_Object_Postion.x = R_Center.x - BOLT_Param[Cam_num].Object_Postion.x;
							BOLT_Param[Cam_num].Offset_Object_Postion.y = R_Center.y - BOLT_Param[Cam_num].Object_Postion.y;

							if (m_Text_View[Cam_num] && !ROI_Mode) // ROI 영역 표시
							{
								circle(Dst_Img[Cam_num],R_Center,2,CV_RGB(255,0,0),2);
								circle(Dst_Img[Cam_num],R_Center,1,CV_RGB(0,0,255),1);
							}
						}
						//AfxMessageBox("1");
					}
					else
					{
						int t_size = countNonZero(Out_binary);
						if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10 || t_size < 10)
						{
							rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
							msg.Format("ROI#%d", s);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);

							rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
							msg.Format("No Object!");
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
							Result_Info[Cam_num] = "";
							for (int ss=1;ss<41;ss++)
							{
								CString t_CString;
								t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
								Result_Info[Cam_num] += t_CString;
								//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num], Cam_num, ss);
							}
							BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
							Alg_Run_Check[Cam_num] = false;
							return true;
						}

						if (BOLT_Param[Cam_num].nCamPosition == 0)
						{// TOP, BOTTOM
							//imwrite("00.bmp",Out_binary);
							// 머리부 찾기
							if (BOLT_Param[Cam_num].nROI0_FilterSize[0] > 0)
							{
								if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::ALL)
								{
									erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
									dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
								}
								else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::VERTICAL)
								{
									erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
									dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
								}
								else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::HORIZONTAL)
								{
									erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
									dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
								}
								J_Delete_Boundary(Out_binary,1);
							}

							if (BOLT_Param[Cam_num].nROI0_MergeFilterSize[0] > 0)
							{
								if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::ALL)
								{
									dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
									erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
								}
								else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::VERTICAL)
								{
									dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
									erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
								}
								else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::HORIZONTAL)
								{
									dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
									erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
								}
								J_Delete_Boundary(Out_binary,1);
							}

							BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
							BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();

							//imwrite("01.bmp",Out_binary);
							int area, left, top, width, height = { 0, };
							int m_min_object_num = -1; int m_min_object_value = Out_binary.rows*Out_binary.cols;
							int m_max_object_num = -1; int m_max_object_value = 0;
							//imwrite("00.bmp",Out_binary);
							findContours(Out_binary, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_NONE);

							if (contours.size() > 0)
							{
								for (int i = 0; i < contours.size(); i++)
								{
									//if contour[i] is not a hole
									//if (hierarchy[i][2] != -1 && hierarchy[i][3] == -1)
									{
										Rect t_rect = boundingRect(contours[i]);
										left = t_rect.x + BOLT_Param[Cam_num].nRect[s].x;
										top = t_rect.y + BOLT_Param[Cam_num].nRect[s].y;
										width = t_rect.width;
										height = t_rect.height;
										area = (int)(contourArea(contours[i]) + 0.5);

										if ((double)area >= BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] && (double)area <= BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s])
										{
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												rectangle(Dst_Img[Cam_num], Point(left,top), Point(left+width,top+height),  
													CV_RGB(0,255,0),1 );  

												int x = left+width/2; //중심좌표
												int y = top+height/2;

												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(x-5,y-10), fontFace, fontScale, CV_RGB(0,100,255), 1, 8);
											}

											if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
											{
												if (left <= m_min_object_value)
												{
													m_min_object_value = left;
													m_min_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
											{
												if (left + width >= m_max_object_value)
												{
													m_max_object_value = left + width;
													m_max_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
											{
												double t_dist = sqrt((double)left*(double)left + (double)top*(double)top);
												if ((int)t_dist <= m_min_object_value)
												{
													m_min_object_value = (int)t_dist;
													m_min_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
											{
												double t_dist = sqrt((double)left*(double)left + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
												if ((int)t_dist <= m_min_object_value)
												{
													m_min_object_value = (int)t_dist;
													m_min_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
											{
												double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top)*(double)(top));
												if ((int)t_dist <= m_min_object_value)
												{
													m_min_object_value = (int)t_dist;
													m_min_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
											{
												double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
												if ((int)t_dist <= m_min_object_value)
												{
													m_min_object_value = (int)t_dist;
													m_min_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::CENTER)
											{
												if (area >= m_max_object_value)
												{
													m_max_object_value = area;
													m_max_object_num = i;
												}
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
											{
												if (area >= m_max_object_value)
												{
													m_max_object_value = area;
													m_max_object_num = i;
												}
											}	
										}
									} // end : if (hierarchy[i][3] == -1)
								} //  end : for (int i = 0; i < contours.size(); i++)

								if (m_min_object_num > -1)
								{
									Rect t_rect = boundingRect(contours[m_min_object_num]);
									left = t_rect.x + BOLT_Param[Cam_num].nRect[s].x;
									top = t_rect.y + BOLT_Param[Cam_num].nRect[s].y;
									width = t_rect.width;
									height = t_rect.height;

									int x = left + width / 2;
									int y = top + height / 2;

									if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
									{
										x = left; y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
									{
										x = left + width; y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
									{
										x = left; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
									{
										x = left; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
									{
										x = left + width; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
									{
										x = left + width; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
									{
										x = left + width/2; y = top;
									}
									BOLT_Param[Cam_num].Offset_Object_Postion = Point(x - BOLT_Param[Cam_num].Object_Postion.x, y - BOLT_Param[Cam_num].Object_Postion.y);

									if(abs(BOLT_Param[Cam_num].Offset_Object_Postion.x) > Out_binary.cols/3 || abs(BOLT_Param[Cam_num].Offset_Object_Postion.y) > Out_binary.rows/3)
									{
										Result_Info[Cam_num] = "";
										for (int ss=1;ss<41;ss++)
										{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;
											//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
										}
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
										msg.Format("Out of ROI!");
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
										BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
										Alg_Run_Check[Cam_num] = false;
										return true;
									}

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
											CV_RGB(255, 0, 0), 2);
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_min_object_num, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);

										circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 2, CV_RGB(255, 0, 0), 2);
										circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 1, CV_RGB(0, 0, 255), 1);
									}
								}

								if (m_max_object_num > -1)
								{
									Rect t_rect = boundingRect(contours[m_max_object_num]);
									left = t_rect.x + BOLT_Param[Cam_num].nRect[s].x;
									top = t_rect.y + BOLT_Param[Cam_num].nRect[s].y;
									width = t_rect.width;
									height = t_rect.height;

									int x = left + width / 2;
									int y = top + height / 2;

									if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
									{
										x = left;y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
									{
										x = left + width; y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
									{
										x = left; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
									{
										x = left; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
									{
										x = left + width; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
									{
										x = left + width; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
									{
										x = left + width/2; y = top;
									}

									BOLT_Param[Cam_num].Offset_Object_Postion = Point(x - BOLT_Param[Cam_num].Object_Postion.x, y - BOLT_Param[Cam_num].Object_Postion.y);

									if(abs(BOLT_Param[Cam_num].Offset_Object_Postion.x) > Out_binary.cols/3 || abs(BOLT_Param[Cam_num].Offset_Object_Postion.y) > Out_binary.rows/3)
									{
										Result_Info[Cam_num] = "";
										for (int ss=1;ss<41;ss++)
										{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;
											//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
										}
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
										msg.Format("Out of ROI!");
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
										BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
										Alg_Run_Check[Cam_num] = false;
										return true;
									}

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
											CV_RGB(255, 0, 0), 2);
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);

										circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 2, CV_RGB(255, 0, 0), 2);
										circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 1, CV_RGB(0, 0, 255), 1);
									}
								}
							} // end : if (contours.size() > 0)

							// 머리를 못 찾을 경우 Error 처리함.
							if(m_min_object_num == -1 && m_max_object_num == -1)
							{
								Result_Info[Cam_num] = "";
								for (int ss=1;ss<41;ss++)
								{
									CString t_CString;
									t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
									Result_Info[Cam_num] += t_CString;
									//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
								}
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								msg.Format("No Object!");
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
								BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
								Alg_Run_Check[Cam_num] = false;
								return true;
							}
						}
						else
						{// SIDE
							if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::CLASS_TYPE || BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE) //유리판일때
							{
								if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE) 
								{
									if (BOLT_Param[Cam_num].nThickness[0] > 0)
									{
										int t_filter_cnt = (int)(0.75*BOLT_Param[Cam_num].nThickness[0]/BOLT_Param[Cam_num].nResolution[0]);
										if (t_filter_cnt > BOLT_Param[Cam_num].nRect[s].width/1.5)
										{
											t_filter_cnt = BOLT_Param[Cam_num].nRect[s].width/1.5;
										}
										erode(Out_binary,Out_binary,element_v,Point(-1,-1), t_filter_cnt);
										dilate(Out_binary,Out_binary,element_v,Point(-1,-1), t_filter_cnt);
										J_Delete_Boundary(Out_binary,1);
									}
								}

								// 사이드 바닦 찾기
								double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;double t_angle = 0;
								int t_Search_Range = 10;
								for (int i = 0;i<t_Search_Range;i++)
								{
									for (int j = 0;j<Out_binary.rows;j++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_Left_x +=(double)i;
											t_Left_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_Left_x/=t_cnt;
									t_Left_y/=t_cnt;
								}
								else
								{
									t_Left_y = Out_binary.rows -1;
								}
/*
								else
								{
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}*/

								double t_Right_x = 0;double t_Right_y = 0;t_cnt = 0;
								for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
								{
									for (int j = 0;j<Out_binary.rows;j++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_Right_x +=(double)i;
											t_Right_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_Right_x/=t_cnt;
									t_Right_y/=t_cnt;
								}
								else
								{
									t_Right_x = Out_binary.cols -1;
									t_Right_y = Out_binary.rows -1;
								}
								//else
								//{
								//	Result_Info[Cam_num] = "";
								//	for (int ss=1;ss<41;ss++)
								//	{
								//		Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
								//	}
								//	rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								//	msg.Format("No Object!");
								//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
								//	BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
								//	return true;
								//}
								//msg.Format("%1f,%1f",t_Left_y,t_Right_y);
								//AfxMessageBox(msg);

								//if (abs(t_Left_y - t_Right_y) > (double)Out_binary.rows/5)
								//{
								//	Result_Info[Cam_num] = "";
								//	for (int ss=1;ss<41;ss++)
								//	{
								//		CString t_CString;
								//		t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
								//		Result_Info[Cam_num] += t_CString;

								//		//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
								//	}
								//	rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								//	msg.Format("No Object!");
								//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
								//	BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
								//	Alg_Run_Check[Cam_num] = false;
								//	return true;
								//}


								t_Left_x = 0;
								t_Right_x = Out_binary.cols-1;
								if (abs(t_Left_y - t_Right_y) > (double)50)
								{
									double t_LR_max = max(t_Left_y,t_Right_y);
									t_Left_y = t_LR_max;
									t_Right_y = t_LR_max;
								}
								BOLT_Param[Cam_num].nSideBottomLeft.x = t_Left_x;
								BOLT_Param[Cam_num].nSideBottomLeft.y = t_Left_y;
								BOLT_Param[Cam_num].nSideBottomRight.x = t_Right_x;
								BOLT_Param[Cam_num].nSideBottomRight.y = t_Right_y;


								if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 2)
								{// 유리판 기준 회전 보정
									t_angle = f_angle360(Point(t_Left_x,t_Left_y),Point(t_Right_x,t_Right_y));
									t_angle -= 180;

									Mat Rotate_Gray_Img;
									//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
									J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Rotate_Gray_Img,1);
									BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
									//cvtColor(Rotate_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);
									cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),CV_GRAY2BGR);
									// 임계화
									// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
									if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
									{
										threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
									}
									else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
									{
										threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
									}
									else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
									{
										inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
									}
									else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
									{
										Mat White_Out_binary = Mat::ones(BOLT_Param[Cam_num].Gray_Obj_Img.rows, BOLT_Param[Cam_num].Gray_Obj_Img.cols, CV_8U)*255;
										inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
										subtract(White_Out_binary, Out_binary, Out_binary);
									}
									else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
									{
										threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
									}
									else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
									{
										threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
									}
									else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
									{
										blur(BOLT_Param[Cam_num].Gray_Obj_Img, Out_binary, Size(3,3));
										// Run the edge detector on grayscale
										Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
									}


									if (BOLT_Param[Cam_num].nROI0_FilterSize[0] > 0)
									{
										erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
										dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
										J_Delete_Boundary(Out_binary,1);
									}

									t_Left_x = 0;t_Left_y = 0;t_cnt = 0;
									for (int i = 0;i<t_Search_Range;i++)
									{
										for (int j = 0;j<Out_binary.rows;j++)
										{
											if (Out_binary.at<uchar>(j,i) > 0)
											{
												t_Left_x +=(double)i;
												t_Left_y += (double)j;
												t_cnt++;
												break;
											}
										}
									}
									if (t_cnt > 0)
									{
										t_Left_x/=t_cnt;
										t_Left_y/=t_cnt;
									}
									else
									{
										t_Left_y = Out_binary.rows-1;
									}

									t_Right_x = 0;t_Right_y = 0;t_cnt = 0;
									for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
									{
										for (int j = 0;j<Out_binary.rows;j++)
										{
											if (Out_binary.at<uchar>(j,i) > 0)
											{
												t_Right_x +=(double)i;
												t_Right_y += (double)j;
												t_cnt++;
												break;
											}
										}
									}
									if (t_cnt > 0)
									{
										t_Right_x/=t_cnt;
										t_Right_y/=t_cnt;
									}
									else
									{
										t_Right_x = Out_binary.cols -1;
										t_Right_y = Out_binary.rows -1;
									}


									t_Left_x = 0;
									t_Right_x = Out_binary.cols-1;
									if (abs(t_Left_y - t_Right_y) > (double)50)
									{
										double t_LR_max = max(t_Left_y,t_Right_y);
										t_Left_y = t_LR_max;
										t_Right_y = t_LR_max;
									}

									if (t_Left_x >= 0 && t_Left_y > 0 && t_Right_x >= 0 && t_Right_y > 0)
									{
										line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
										Rect t_R_ROI(0,t_Left_y+2,Out_binary.cols,Out_binary.rows-(t_Left_y+2));
										Out_binary(t_R_ROI) = 0;

										if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
										{
											erode(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
											dilate(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
										}
									}
								}
								else 
								{
									if (t_Left_x >= 0 && t_Left_y > 0 && t_Right_x >= 0 && t_Right_y > 0)
									{
										line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
										Rect t_R_ROI(0,t_Left_y+2,Out_binary.cols,Out_binary.rows-(t_Left_y+2));
										Out_binary(t_R_ROI) = 0;
										if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
										{
											erode(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
											dilate(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
										}									}
								}

								//if (Result_Debugging) // 오링 사이드 바닦 라인 이미지
								//{
								//	msg.Format("Save\\Debugging\\Cam%d_%2d_Line_Thres_ROI_Gray_Img.bmp",Cam_num, s);
								//	imwrite(msg.GetBuffer(),Out_binary);		
								//}
								//imwrite("00.bmp",Out_binary);

								erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);
								J_Delete_Boundary(Out_binary,1);

								// 못 머리만 찾기
								findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								if(contours.size() == 0) // 칸투어 갯수로 예외처리
								{
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
										CString t_CString;
										t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
										Result_Info[Cam_num] += t_CString;

										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}
								
								vector<Rect> boundRect( contours.size() );
								int m_max_object_num = -1;int m_max_object_value = 0;
								for( int k = 0; k < contours.size(); k++ )
								{  
									boundRect[k] = boundingRect( Mat(contours[k]) );
									if (m_max_object_value <= boundRect[k].width*boundRect[k].height
										&& boundRect[k].x > 0 && boundRect[k].x + boundRect[k].width < Out_binary.cols-1
										)
										//	&& pointPolygonTest( contours[k], Point(tROI.width/2,tROI.height/2), false ) == 1)
									{
										m_max_object_value = boundRect[k].width*boundRect[k].height;
										m_max_object_num = k;
									}
								}

								// 머리를 못 찾을 경우 Error 처리함.
								if(m_max_object_num == -1)
								{
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
										CString t_CString;
										t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
										Result_Info[Cam_num] += t_CString;

										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}

								if (m_max_object_num >= 0)
								{
									int left = boundRect[m_max_object_num].x;
									int top = boundRect[m_max_object_num].y;
									int width = boundRect[m_max_object_num].width;
									int height = boundRect[m_max_object_num].height;

									if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 1)
									{// 나사 몸통 기준 회전 보정
										J_Fill_Hole(Out_binary);
										if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
										{
											erode(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
											dilate(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
											dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]/2);
											erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]/2);
											J_Delete_Boundary(Out_binary,1);
										}
										Point2f t_Top_Center(0,0);
										Point2f t_Bot_Center(0,0);

										double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;
										double t_B_Bottom_x = 0;double t_B_Bottom_y = 0;double t_Bottom_cnt = 0;
										int t_height = BOLT_Param[Cam_num].nAngleHeightHeight[0];
										if (t_height > height)
										{
											t_height = height;
										}

										Vec4f lines;
										vector<Point2f> vec_2center;

										for (int j = top+height/4;j<=top+t_height;j++)
										{
											t_B_Top_x = t_B_Top_y = t_Top_cnt = 0;
											for (int i = left;i<left+width;i++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_B_Top_x +=(double)i;
													t_B_Top_y += (double)j;
													t_Top_cnt++;
												}
											}
											if (t_Top_cnt > 0)
											{
												t_B_Top_x/=t_Top_cnt;
												t_B_Top_y/=t_Top_cnt;
												t_Top_Center.x = t_B_Top_x;
												t_Top_Center.y = t_B_Top_y;
												vec_2center.push_back(t_Top_Center);
											}
										}
										if (vec_2center.size() > 2 )
										{
											fitLine(vec_2center,lines, CV_DIST_L2,0,0.001,0.001);
											t_angle = (180.0/CV_PI)*atan(lines[1]/lines[0]);
											if (t_angle < 0)
											{
												t_angle += 90;
											}
											else if (t_angle > 0)
											{
												t_angle -= 90;
											}
											//msg.Format("%1.3f",t_angle-90);
											//AfxMessageBox(msg);
										}
/*
										for (int i = left;i<left+width;i++)
										{
											for (int j = top-t_height/20+t_height/3;j<=top+t_height/20+t_height/3;j++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_B_Top_x +=(double)i;
													t_B_Top_y += (double)j;
													t_Top_cnt++;
													break;
												}
											}
											for (int j = top-t_height/20+2*t_height/3;j<=top+t_height/20+2*t_height/3;j++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_B_Bottom_x +=(double)i;
													t_B_Bottom_y += (double)j;
													t_Bottom_cnt++;
													break;
												}
											}
										}
										if (t_Top_cnt > 0)
										{
											t_B_Top_x/=t_Top_cnt;
											t_B_Top_y/=t_Top_cnt;
											t_Top_Center.x = t_B_Top_x;
											t_Top_Center.y = t_B_Top_y;
										}
										if (t_Bottom_cnt > 0)
										{
											t_B_Bottom_x/=t_Bottom_cnt;
											t_B_Bottom_y/=t_Bottom_cnt;
											t_Bot_Center.x = t_B_Bottom_x;
											t_Bot_Center.y = t_B_Bottom_y;
										}

										double t_angle = f_angle360(t_Top_Center, t_Bot_Center);

										if (t_angle == 180 || t_angle == 0)
										{
											t_angle = 0;
										}
										else
										{
											t_angle += 90-360;
										}*/
										int m_max_object_num = -1;int m_max_object_value = 0;

										//if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 2)
										//{
										//	t_angle = f_angle360(Point(0,t_Left_y+1),Point(Out_binary.cols,t_Right_y+1));
										//	t_angle -= 180;
										//}

										Mat Rotate_Gray_Img;
										//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
										J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Rotate_Gray_Img,1);
										BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
										//cvtColor(Rotate_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

										// 임계화
										// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
										if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
										{
											threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
										}
										else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
										{
											threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
										}
										else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
										{
											inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
										}
										else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
										{
											Mat White_Out_binary = Mat::ones(BOLT_Param[Cam_num].Gray_Obj_Img.rows, BOLT_Param[Cam_num].Gray_Obj_Img.cols, CV_8U)*255;
											inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
											subtract(White_Out_binary, Out_binary, Out_binary);
										}
										else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
										{
											threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
										}
										else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
										{
											threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
										}
										else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
										{
											blur(BOLT_Param[Cam_num].Gray_Obj_Img, Out_binary, Size(3,3));
											// Run the edge detector on grayscale
											Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
										}

										t_Left_x = 0;t_Left_y = 0;t_cnt = 0;
										for (int i = 0;i<t_Search_Range;i++)
										{
											for (int j = 0;j<Out_binary.rows;j++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_Left_x +=(double)i;
													t_Left_y += (double)j;
													t_cnt++;
													break;
												}
											}
										}
										if (t_cnt > 0)
										{
											t_Left_x/=t_cnt;
											t_Left_y/=t_cnt;
										}
										else
										{
											t_Left_y = Out_binary.rows-1;
										}

										t_Right_x = 0;t_Right_y = 0;t_cnt = 0;
										for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
										{
											for (int j = 0;j<Out_binary.rows;j++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_Right_x +=(double)i;
													t_Right_y += (double)j;
													t_cnt++;
													break;
												}
											}
										}
										if (t_cnt > 0)
										{
											t_Right_x/=t_cnt;
											t_Right_y/=t_cnt;
										}
										else
										{
											t_Right_x = Out_binary.cols -1;
											t_Right_y = Out_binary.rows -1;
										}

										if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
										{
											line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
											Rect t_R_ROI(0,t_Left_y+2,Out_binary.cols,Out_binary.rows-(t_Left_y+2));
											Out_binary(t_R_ROI) = 0;
										}

										erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
										dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);
										J_Delete_Boundary(Out_binary,1);
										J_Fill_Hole(Out_binary);
										//imwrite("00.bmp",Out_binary);
										// 못 머리만 찾기
										findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										if(contours.size() == 0) // 칸투어 갯수로 예외처리
										{
											Result_Info[Cam_num] = "";
											for (int ss=1;ss<41;ss++)
											{
												CString t_CString;
												t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
												Result_Info[Cam_num] += t_CString;

												//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
											}
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
											msg.Format("No Object!");
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
											BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
											Alg_Run_Check[Cam_num] = false;
											return true;
										}

										vector<Rect> boundRect1( contours.size() );
										m_max_object_num = -1;m_max_object_value = 0;
										for( int k = 0; k < contours.size(); k++ )
										{  
											boundRect1[k] = boundingRect( Mat(contours[k]) );
											if (m_max_object_value <= boundRect1[k].width*boundRect1[k].height
												&& boundRect1[k].x > 0 && boundRect1[k].x + boundRect1[k].width < Out_binary.cols-1
												)
												//	&& pointPolygonTest( contours[k], Point(tROI.width/2,tROI.height/2), false ) == 1)
											{
												m_max_object_value = boundRect1[k].width*boundRect1[k].height;
												m_max_object_num = k;
											}
										}

										if (m_max_object_num >= 0)
										{
											Mat Target_Thres_ROI_Gray_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
											drawContours( Target_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8);
											BOLT_Param[Cam_num].Thres_Obj_Img = Target_Thres_ROI_Gray_Img.clone();
											//BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
											cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),CV_GRAY2BGR);

											left = boundRect1[m_max_object_num].x;
											top = boundRect1[m_max_object_num].y;
											width = boundRect1[m_max_object_num].width;
											height = boundRect1[m_max_object_num].height;
											//AfxMessageBox("1");
											int x = left + width / 2;
											int y = top + height / 2;

											if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
											{
												x = left;y = top + height / 2;
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
											{
												x = left + width; y = top + height / 2;
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
											{
												x = left; y = top;
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
											{
												x = left; y = top + height;
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
											{
												x = left + width; y = top;
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
											{
												x = left + width; y = top + height;
											}
											else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
											{
												x = left + width/2; y = top;
											}
											BOLT_Param[Cam_num].Offset_Object_Postion = Point(x - BOLT_Param[Cam_num].Object_Postion.x, y - BOLT_Param[Cam_num].Object_Postion.y);
										}
										//AfxMessageBox("1");

										//bitwise_and(BOLT_Param[Cam_num].Gray_Obj_Img,Target_Thres_ROI_Gray_Img,BOLT_Param[Cam_num].Gray_Obj_Img);
										//bitwise_and(BOLT_Param[Cam_num].Thres_Obj_Img,Target_Thres_ROI_Gray_Img,BOLT_Param[Cam_num].Thres_Obj_Img);
									}
									else
									{
										//AfxMessageBox("1");
										Mat Target_Thres_ROI_Gray_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
										drawContours( Target_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8);
										BOLT_Param[Cam_num].Thres_Obj_Img = Target_Thres_ROI_Gray_Img.clone();
										if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 0)
										{
											BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
										}
										cvtColor(BOLT_Param[Cam_num].Gray_Obj_Img,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),CV_GRAY2BGR);
										//bitwise_and(BOLT_Param[Cam_num].Gray_Obj_Img,Target_Thres_ROI_Gray_Img,BOLT_Param[Cam_num].Gray_Obj_Img);
										//bitwise_and(BOLT_Param[Cam_num].Thres_Obj_Img,Target_Thres_ROI_Gray_Img,BOLT_Param[Cam_num].Thres_Obj_Img);


										int x = left + width / 2;
										int y = top + height / 2;

										if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
										{
											x = left; y = top + height / 2;
										}
										else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
										{
											x = left + width; y = top + height / 2;
										}
										else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
										{
											x = left; y = top;
										}
										else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
										{
											x = left; y = top + height;
										}
										else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
										{
											x = left + width; y = top;
										}
										else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
										{
											x = left + width; y = top + height;
										}
										else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
										{
											x = left + width/2; y = top;
										}
										BOLT_Param[Cam_num].Offset_Object_Postion = Point(x - BOLT_Param[Cam_num].Object_Postion.x, y - BOLT_Param[Cam_num].Object_Postion.y);
									}

									//BOLT_Param[Cam_num].Offset_Object_Postion.x = boundRect[m_max_object_num].x - BOLT_Param[Cam_num].Object_Postion.x;
									//BOLT_Param[Cam_num].Offset_Object_Postion.y = boundRect[m_max_object_num].y - BOLT_Param[Cam_num].Object_Postion.y;
									if (Result_Debugging)
									{
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp",Cam_num,s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Thres_Img.bmp",Cam_num,s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
									}
									//msg.Format("%d,%d",BOLT_Param[Cam_num].Offset_Object_Postion.x,BOLT_Param[Cam_num].Offset_Object_Postion.y);
									//AfxMessageBox(msg);

									if (abs(BOLT_Param[Cam_num].Offset_Object_Postion.y) > BOLT_Param[Cam_num].nRect[0].height/3)
									{
										Result_Info[Cam_num] = "";
										for (int ss=1;ss<41;ss++)
										{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;

											//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
										}
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
										msg.Format("No Object!");
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
										BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
										Alg_Run_Check[Cam_num] = false;
										return true;
									}

								} else
								{
									BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
									BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
									if (Result_Debugging)
									{
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp",Cam_num,s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Thres_Img.bmp",Cam_num,s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
									}
								}
							}
							else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::INDEX_TYPE) //Index판일때
							{
								// 사이드 상부 바닦 찾기
								double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;
								int t_Search_Range = 10;
								for (int i = 0;i<t_Search_Range;i++)
								{
									for (int j = 0;j<Out_binary.rows;j++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_Left_x +=(double)i;
											t_Left_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_Left_x/=t_cnt;
									t_Left_y/=t_cnt;
								}

								double t_Right_x = 0;double t_Right_y = 0;t_cnt = 0;
								for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
								{
									for (int j = 0;j<Out_binary.rows;j++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_Right_x +=(double)i;
											t_Right_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_Right_x/=t_cnt;
									t_Right_y/=t_cnt;
								}

								t_Left_x = 0;
								t_Right_x = Out_binary.cols;
								if (abs(t_Left_y - t_Right_y) > 50)
								{
									double t_temp = max(t_Left_y,t_Right_y);
									t_Left_y = t_temp;
									t_Right_y = t_temp;
								}

								// 사이드 하부 바닦 찾기
								double t_B_Left_x = 0;double t_B_Left_y = 0;t_cnt = 0;
								for (int i = 0;i<t_Search_Range;i++)
								{
									for (int j = Out_binary.rows-1;j>0;j--)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_B_Left_x +=(double)i;
											t_B_Left_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_B_Left_x/=t_cnt;
									t_B_Left_y/=t_cnt;
								}

								double t_B_Right_x = 0;double t_B_Right_y = 0;t_cnt = 0;
								for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
								{
									for (int j = Out_binary.rows-1;j>0;j--)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_B_Right_x +=(double)i;
											t_B_Right_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_B_Right_x/=t_cnt;
									t_B_Right_y/=t_cnt;
								}

								if (abs(t_B_Left_y - t_B_Right_y) > 50)
								{
									double t_temp = min(t_B_Left_y,t_B_Right_y);
									if (t_temp == t_B_Left_y)
									{
										t_B_Left_y = t_temp;
										t_B_Right_y = t_B_Left_y + abs(t_Left_y - t_Right_y) + 1;
									}
									else
									{
										t_B_Right_y = t_temp;
										t_B_Left_y = t_B_Right_y + abs(t_Right_y - t_Left_y) + 1;
									}
								}

								Mat t_morph = Out_binary.clone();
								Rect tt_Sub_ROI;
								tt_Sub_ROI.x = 0;tt_Sub_ROI.y = max(t_Left_y,t_Right_y);tt_Sub_ROI.width = Out_binary.cols;tt_Sub_ROI.height = Out_binary.rows - max(t_Left_y,t_Right_y) - 1;
								if (tt_Sub_ROI.y < 0)
								{
									tt_Sub_ROI.y = 0;
								}
								if (tt_Sub_ROI.height <= 0)
								{
									tt_Sub_ROI.height = 1;
								}

								if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
								{
									erode(Out_binary(tt_Sub_ROI),t_morph(tt_Sub_ROI),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
									dilate(t_morph(tt_Sub_ROI),t_morph(tt_Sub_ROI),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
								}

								if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
								{
									line(t_morph,Point(0,t_Left_y+1),Point(Out_binary.cols,t_Right_y+1),CV_RGB(0,0,0),2);
								}
								if (t_B_Left_x > 0 && t_B_Left_y > 0 && t_B_Right_x > 0 && t_B_Right_y > 0)
								{
									line(t_morph,Point(0,t_B_Left_y+1),Point(Out_binary.cols,t_B_Right_y+1),CV_RGB(0,0,0),2);
								}

								//imwrite("00.bmp",t_morph);

								erode(t_morph,t_morph,element_v,Point(-1,-1), 1);
								dilate(t_morph,t_morph,element_v,Point(-1,-1), 1);

								Mat stats, centroids, label;  
								int numOfLables = connectedComponentsWithStats(t_morph, label, stats, centroids, 8,CV_32S);
								int m_Head_Idx = -1; int m_Head_value = 0;
								int m_Body_Idx = -1; int m_Body_value = 0;
								int area, left, top, width, height = { 0, };
								for (int j = 1; j < numOfLables; j++)
								{
									area = stats.at<int>(j, CC_STAT_AREA);
									left = stats.at<int>(j, CC_STAT_LEFT);
									top = stats.at<int>(j, CC_STAT_TOP);
									width = stats.at<int>(j, CC_STAT_WIDTH);
									height = stats.at<int>(j, CC_STAT_HEIGHT);

									if ((double)(top + height) <=  max(t_Left_y, t_Right_y) && area >= m_Head_value && left > 1 && left+width < t_morph.cols-1)
									{ // 머리부 찾기
										m_Head_value = area;
										m_Head_Idx = j;
									}
									if ((double)(top) >=  min(t_B_Left_y, t_B_Right_y) && area >= m_Body_value && left > 1 && left+width < t_morph.cols-1)
									{ // 바디부 찾기
										m_Body_value = area;
										m_Body_Idx = j;
									}
								}

								Point2f t_Head_Center(0,0);
								Point2f t_Body_Center(0,0);
								if (m_Head_Idx > -1 && m_Body_Idx > -1)
								{
									//t_Head_Center.x = centroids.at<double>(m_Head_Idx, 0); //중심좌표		
									//t_Head_Center.y = centroids.at<double>(m_Head_Idx, 1);
									t_Head_Center.x = (float)stats.at<int>(m_Head_Idx, CC_STAT_LEFT) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_WIDTH))/2; //중심좌표		
									t_Head_Center.y = (float)stats.at<int>(m_Head_Idx, CC_STAT_TOP) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_HEIGHT))/2;
									t_Body_Center.x = centroids.at<double>(m_Body_Idx, 0); //중심좌표		
									t_Body_Center.y = centroids.at<double>(m_Body_Idx, 1);
								}
								else
								{
									rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
									msg.Format("ROI#%d", s);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);

									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}

								if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 1 && m_Body_Idx > -1)
								{// 하부쪽 기준 회전
									left = stats.at<int>(m_Body_Idx, CC_STAT_LEFT);
									top = stats.at<int>(m_Body_Idx, CC_STAT_TOP);
									width = stats.at<int>(m_Body_Idx, CC_STAT_WIDTH);
									height = stats.at<int>(m_Body_Idx, CC_STAT_HEIGHT);

									double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;
									double t_B_Bottom_x = 0;double t_B_Bottom_y = 0;double t_Bottom_cnt = 0;
									for (int i = left;i<left+width;i++)
									{
										for (int j = top-2+height/4;j<=top+2+height/4;j++)
										{
											if (Out_binary.at<uchar>(j,i) > 0)
											{
												t_B_Top_x +=(double)i;
												t_B_Top_y += (double)j;
												t_Top_cnt++;
												break;
											}
										}
										for (int j = top-2+3*height/4;j<=top+2+3*height/4;j++)
										{
											if (Out_binary.at<uchar>(j,i) > 0)
											{
												t_B_Bottom_x +=(double)i;
												t_B_Bottom_y += (double)j;
												t_Bottom_cnt++;
												break;
											}
										}
									}
									if (t_Top_cnt > 0)
									{
										t_B_Top_x/=t_Top_cnt;
										t_B_Top_y/=t_Top_cnt;
										t_Head_Center.x = t_B_Top_x;
										t_Head_Center.y = t_B_Top_y;
									}
									if (t_Bottom_cnt > 0)
									{
										t_B_Bottom_x/=t_Bottom_cnt;
										t_B_Bottom_y/=t_Bottom_cnt;
										t_Body_Center.x = t_B_Bottom_x;
										t_Body_Center.y = t_B_Bottom_y;
									}
								}
								//imwrite("01.bmp",Dst_Img[Cam_num]);

								double t_angle = f_angle360(t_Head_Center, t_Body_Center);

								//if (t_angle == 180 || t_angle == 0)
								if ((t_angle >= 180 - 0.01 && t_angle <= 180 + 0.01) || (t_angle >= 0-0.1 && t_angle <= 0+0.1))
								{
									t_angle = 0;
								}
								else
								{
									t_angle += 90-360;
								}
								int m_max_object_num = -1;int m_max_object_value = 0;

								if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 2)
								{
									t_angle = f_angle360(Point(0,t_Left_y+1),Point(Out_binary.cols,t_Right_y+1));
									t_angle -= 180;
									//if (t_angle > 180)
									//{
									//	t_angle -= 180;
									//}
									//if (t_angle <= 180)
									//{
									//	t_angle -= 180;
									//}
									//msg.Format("%1.2f",t_angle);
									//AfxMessageBox(msg);
								}

								if (abs(t_angle) > 20)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
									}

									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,s);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}

								Mat Rotate_Gray_Img;
								//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
								J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Rotate_Gray_Img,1);
								BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
								cvtColor(Rotate_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

								// 임계화
								// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
								if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
								{
									threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
								}
								else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
								{
									threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
								}
								else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
								{
									inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
								}
								else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
								{
									Mat White_Out_binary = Mat::ones(BOLT_Param[Cam_num].Gray_Obj_Img.rows, BOLT_Param[Cam_num].Gray_Obj_Img.cols, CV_8U)*255;
									inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
									subtract(White_Out_binary, Out_binary, Out_binary);
								}
								else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
								{
									threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
								}
								else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
								{
									threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
								}
								else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
								{
									blur(BOLT_Param[Cam_num].Gray_Obj_Img, Out_binary, Size(3,3));
									// Run the edge detector on grayscale
									Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
								}
								//imwrite("02-0.bmp",BOLT_Param[Cam_num].Gray_Obj_Img);
								//imwrite("02.bmp",Out_binary);
								// 사이드 상부 바닦 찾기
								t_Left_x = 0;t_Left_y = 0;t_cnt = 0;
								t_Search_Range = 10;
								for (int i = 0;i<t_Search_Range;i++)
								{
									for (int j = 0;j<Out_binary.rows;j++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_Left_x +=(double)i;
											t_Left_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_Left_x/=t_cnt;
									t_Left_y/=t_cnt;
								}

								t_Right_x = 0;t_Right_y = 0;t_cnt = 0;
								for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
								{
									for (int j = 0;j<Out_binary.rows;j++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_Right_x +=(double)i;
											t_Right_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_Right_x/=t_cnt;
									t_Right_y/=t_cnt;
								}

								if (t_Left_y  < 10 || t_Right_y < 10)
								{
									double t_temp = max(t_Left_y,t_Right_y);
									t_Left_y = t_temp;
									t_Right_y = t_temp;
								}

								// 사이드 하부 바닦 찾기
								t_B_Left_x = 0;t_B_Left_y = 0;t_cnt = 0;
								for (int i = 0;i<t_Search_Range;i++)
								{
									for (int j = Out_binary.rows-1;j>0;j--)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_B_Left_x +=(double)i;
											t_B_Left_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_B_Left_x/=t_cnt;
									t_B_Left_y/=t_cnt;
								}

								t_B_Right_x = 0;t_B_Right_y = 0;t_cnt = 0;
								for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
								{
									for (int j = Out_binary.rows-1;j>0;j--)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_B_Right_x +=(double)i;
											t_B_Right_y += (double)j;
											t_cnt++;
											break;
										}
									}
								}
								if (t_cnt > 0)
								{
									t_B_Right_x/=t_cnt;
									t_B_Right_y/=t_cnt;
								}

								if (abs(t_B_Left_y - t_B_Right_y) > 50)
								{
									double t_temp = min(t_B_Left_y,t_B_Right_y);

									if (t_temp == t_B_Left_y)
									{
										t_B_Left_y = t_temp;
										t_B_Right_y = t_B_Left_y + abs(t_Left_y - t_Right_y) + 1;
									}
									else
									{
										t_B_Right_y = t_temp;
										t_B_Left_y = t_B_Right_y + abs(t_Right_y - t_Left_y) + 1;
									}
								}

								//imwrite("00.bmp",Out_binary);
								vector< vector<Point> >  co_ordinates;
								co_ordinates.push_back(vector<Point>());
								co_ordinates[0].push_back(Point(0,t_Left_y));
								co_ordinates[0].push_back(Point(Out_binary.cols-1,t_Right_y));
								co_ordinates[0].push_back(Point(Out_binary.cols-1,t_B_Right_y));
								co_ordinates[0].push_back(Point(0,t_B_Left_y));
								drawContours( Out_binary,co_ordinates,0, Scalar(0),CV_FILLED, 8 );
								//imwrite("01.bmp",Out_binary);
								tt_Sub_ROI.x = 0;tt_Sub_ROI.y = min(t_Left_y,t_Right_y)-1;tt_Sub_ROI.width = Out_binary.cols;tt_Sub_ROI.height = max((t_B_Left_y - t_Left_y),(t_B_Right_y - t_Right_y))+10;
								erode(Out_binary(tt_Sub_ROI),Out_binary(tt_Sub_ROI),element_v,Point(-1,-1), 1);
								dilate(Out_binary(tt_Sub_ROI),Out_binary(tt_Sub_ROI),element_v,Point(-1,-1), 1);
								//imwrite("02.bmp",Out_binary);

								numOfLables = connectedComponentsWithStats(Out_binary, label, stats, centroids, 8,CV_32S);
								m_Head_Idx = -1; m_Head_value = 0;
								m_Body_Idx = -1; m_Body_value = 0;
								for (int j = 1; j < numOfLables; j++)
								{
									area = stats.at<int>(j, CC_STAT_AREA);
									left = stats.at<int>(j, CC_STAT_LEFT);
									top = stats.at<int>(j, CC_STAT_TOP);
									width = stats.at<int>(j, CC_STAT_WIDTH);
									height = stats.at<int>(j, CC_STAT_HEIGHT);

									if ((double)(top + height) <=  max(t_Left_y, t_Right_y)+2 && area >= m_Head_value && left > 1 && left+width < Out_binary.cols-1)
									{ // 머리부 찾기
										m_Head_value = area;
										m_Head_Idx = j;
									}
									if ((double)(top) >=  min(t_B_Left_y, t_B_Right_y)-2 && area >= m_Body_value && left > 1 && left+width < Out_binary.cols-1)
									{ // 바디부 찾기
										m_Body_value = area;
										m_Body_Idx = j;
									}
									if (left < 2 || left+width > Out_binary.cols-3)
									{
										for (int y = 0; y < Out_binary.rows; ++y) {
											int *tlabel = label.ptr<int>(y);
											uchar* pixel = Out_binary.ptr<uchar>(y);
											uchar* pixel2 = BOLT_Param[Cam_num].Gray_Obj_Img.ptr<uchar>(y);
											for (int x = 0; x < Out_binary.cols; ++x) {
												if (tlabel[x] == j)
												{
													pixel[x] = 0;
													pixel2[x] = 255;
												}
											}
										}
									}
								}

								//imwrite("03.bmp",Out_binary);
								//imwrite("04.bmp",BOLT_Param[Cam_num].Gray_Obj_Img);
								if (m_Head_Idx > -1 && m_Body_Idx > -1)
								{
									for (int y = 0; y < Out_binary.rows; ++y) {
										int *tlabel = label.ptr<int>(y);
										uchar* pixel = Out_binary.ptr<uchar>(y);
										for (int x = 0; x < Out_binary.cols; ++x) {
											if (tlabel[x] != m_Head_Idx && tlabel[x] != m_Body_Idx)
											{
												pixel[x] = 0;
											}
										}
									}

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										for (int y = 0; y < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).rows; ++y) {
											int *tlabel = label.ptr<int>(y);
											Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(y);
											for (int x = 0; x < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).cols; ++x) {
												if (tlabel[x] == m_Head_Idx)
												{
													pixel[x][2] = 0;  
													pixel[x][1] = 255;
													pixel[x][0] = 0;
												}
												else if (tlabel[x] == m_Body_Idx)
												{
													pixel[x][2] = 0;  
													pixel[x][1] = 255;
													pixel[x][0] = 0;
												}
											}
										}
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										for (int y = 0; y < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).rows; ++y) {
											int *tlabel = label.ptr<int>(y);
											Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(y);
											for (int x = 0; x < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).cols; ++x) {
												if (tlabel[x] == m_Head_Idx)
												{
													pixel[x][2] = 0;  
													pixel[x][1] = 100;
													pixel[x][0] = 0;
												}
												else if (tlabel[x] == m_Body_Idx)
												{
													pixel[x][2] = 0;  
													pixel[x][1] = 100;
													pixel[x][0] = 0;
												}
											}
										}
									}
									//t_Head_Center.x = centroids.at<double>(m_Head_Idx, 0); //중심좌표		
									//t_Head_Center.y = centroids.at<double>(m_Head_Idx, 1);

									t_Head_Center.x = (float)stats.at<int>(m_Head_Idx, CC_STAT_LEFT) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_WIDTH))/2; //중심좌표		
									t_Head_Center.y = (float)stats.at<int>(m_Head_Idx, CC_STAT_TOP) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_HEIGHT))/2;
									t_Body_Center.x = centroids.at<double>(m_Body_Idx, 0); //중심좌표		
									t_Body_Center.y = centroids.at<double>(m_Body_Idx, 1);

									BOLT_Param[Cam_num].Offset_Object_Postion = Point(t_Head_Center.x - BOLT_Param[Cam_num].Object_Postion.x,
											t_Head_Center.y - BOLT_Param[Cam_num].Object_Postion.y);

									if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 1 && m_Body_Idx > -1)
									{// 하부쪽 기준 회전
										left = stats.at<int>(m_Body_Idx, CC_STAT_LEFT);
										top = stats.at<int>(m_Body_Idx, CC_STAT_TOP);
										width = stats.at<int>(m_Body_Idx, CC_STAT_WIDTH);
										height = stats.at<int>(m_Body_Idx, CC_STAT_HEIGHT);

										double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;
										double t_B_Bottom_x = 0;double t_B_Bottom_y = 0;double t_Bottom_cnt = 0;
										for (int i = left;i<left+width;i++)
										{
											for (int j = top-2+height/4;j<=top+2+height/4;j++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_B_Top_x +=(double)i;
													t_B_Top_y += (double)j;
													t_Top_cnt++;
													break;
												}
											}
											for (int j = top-2+3*height/4;j<=top+2+3*height/4;j++)
											{
												if (Out_binary.at<uchar>(j,i) > 0)
												{
													t_B_Bottom_x +=(double)i;
													t_B_Bottom_y += (double)j;
													t_Bottom_cnt++;
													break;
												}
											}
										}
										if (t_Top_cnt > 0)
										{
											t_B_Top_x/=t_Top_cnt;
											t_B_Top_y/=t_Top_cnt;
											t_Head_Center.x = t_B_Top_x;
											t_Head_Center.y = t_B_Top_y;
										}
										if (t_Bottom_cnt > 0)
										{
											t_B_Bottom_x/=t_Bottom_cnt;
											t_B_Bottom_y/=t_Bottom_cnt;
											t_Body_Center.x = t_B_Bottom_x;
											t_Body_Center.y = t_B_Bottom_y;
										}
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										//line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,t_Body_Center,CV_RGB(255,200,0),1,8);
										//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,2,CV_RGB(255,0,0),2);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,1,CV_RGB(255,255,0),1);
										//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Body_Center,2,CV_RGB(255,0,0),2);
										//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Body_Center,1,CV_RGB(0,0,255),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,t_Body_Center,CV_RGB(255,200,0),1,8);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,2,CV_RGB(255,0,0),2);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,1,CV_RGB(0,0,255),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Body_Center,2,CV_RGB(255,0,0),2);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Body_Center,1,CV_RGB(0,0,255),1);
									}
								}
								BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();

								findContours( Out_binary.clone(), BOLT_Param[Cam_num].Object_contours, BOLT_Param[Cam_num].Object_hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								if (BOLT_Param[Cam_num].Object_contours.size() == 0)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
									}

									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,s);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}

								m_max_object_num = -1;int m_2nd_max_object_num = -1;
								m_max_object_value = 0;
								Rect tt_rect;Rect t_Side_rect;Rect t_Head_rect;
								for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
								{  
									tt_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
									if (m_max_object_value<= tt_rect.width*tt_rect.height 
										&& tt_rect.x > 1 && tt_rect.y > 1 && tt_rect.x + tt_rect.width < Out_binary.cols-2 && tt_rect.y + tt_rect.height < Out_binary.rows-2)
									{
										m_max_object_value = tt_rect.width*tt_rect.height;
										m_max_object_num = k;
									}
								}
								m_max_object_value = 0;
								for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
								{  
									tt_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
									if (m_max_object_value<= tt_rect.width*tt_rect.height && m_max_object_num != k
										&& tt_rect.x > 1 && tt_rect.y > 1 && tt_rect.x + tt_rect.width < Out_binary.cols-2 && tt_rect.y + tt_rect.height < Out_binary.rows-2)
									{
										m_max_object_value = tt_rect.width*tt_rect.height;
										m_2nd_max_object_num = k;
									}
								}

								if (m_max_object_num == -1 || m_2nd_max_object_num == -1)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,s);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}
								BOLT_Param[Cam_num].Object_1st_idx = m_max_object_num;
								BOLT_Param[Cam_num].Object_2nd_idx = m_2nd_max_object_num;
								t_Side_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_max_object_num]) );
								t_Head_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num]) );
								if (t_Side_rect.y < t_Head_rect.y)
								{
									t_Side_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num]) );
									t_Head_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_max_object_num]) );
								}

								if (t_Side_rect.y < t_Head_rect.y)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,s);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}
								// 머리를 못 찾을 경우 Error 처리함.
								if(m_max_object_num == -1)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,s);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}

								//if (m_max_object_num >= 0 && m_2nd_max_object_num >= 0)
								//{
								//	//BOLT_Param[Cam_num].Offset_Object_Postion = Point(t_Head_rect.x - BOLT_Param[Cam_num].Object_Postion.x,
								//	//	t_Head_rect.y - BOLT_Param[Cam_num].Object_Postion.y);
								//	//BOLT_Param[Cam_num].Thres_Obj_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								//	//drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
								//	//drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_2nd_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

								//	if (m_Text_View[Cam_num] && !ROI_Mode)
								//	{
								//		vector<vector<Point> > total_contours(1);
								//		for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
								//		{
								//			total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
								//		}
								//		for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num].size();j++)
								//		{
								//			total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num][j]);
								//		}

								//		tt_rect = boundingRect( Mat(total_contours[0]) );
								//		Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
								//		rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
								//		//t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
								//		//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
								//		//t_Head_rect.x-=1;t_Head_rect.y-=1;t_Head_rect.width+=2;t_Head_rect.height+=2;
								//		//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
								//	}
								//	if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								//	{
								//		vector<vector<Point> > total_contours(1);
								//		for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
								//		{
								//			total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
								//		}
								//		for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num].size();j++)
								//		{
								//			total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num][j]);
								//		}
								//		BOLT_Param[Cam_num].Thres_Obj_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								//		drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
								//		drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_2nd_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

								//		tt_rect = boundingRect( Mat(total_contours[0]) );
								//		Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
								//		rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
								//		t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
								//		rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
								//		t_Head_rect.x-=1;t_Head_rect.y-=1;t_Head_rect.width+=2;t_Head_rect.height+=2;
								//		rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
								//	}
								//}
							}
							else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::NONE_TYPE) //가이드 없을 때
							{
								Mat t_morph = Out_binary.clone();
								if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
								{
									erode(Out_binary,t_morph,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
									dilate(t_morph,t_morph,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
									J_Delete_Boundary(t_morph,1);
								}
								//J_Delete_Boundary(Out_binary,1);
								// 나사 회전량 계산
								Rect t_Sub_ROI;
								t_Sub_ROI.x = 0;t_Sub_ROI.y = BOLT_Param[Cam_num].nAngleHeightTop[0];t_Sub_ROI.width = Out_binary.cols;t_Sub_ROI.height = BOLT_Param[Cam_num].nAngleHeightHeight[0];
								if (t_Sub_ROI.height == 0)
								{
									t_Sub_ROI.height = 1;
								}

								bool t_check_roi = true;
								if (t_Sub_ROI.x < 0)
								{
									t_check_roi = false;
								}
								if (t_Sub_ROI.y < 0)
								{
									t_check_roi = false;
								}
								if (t_Sub_ROI.x + t_Sub_ROI.width > Out_binary.cols)
								{
									t_check_roi = false;
								}
								if (t_Sub_ROI.y + t_Sub_ROI.height > Out_binary.rows)
								{
									t_check_roi = false;
								}

								if (!t_check_roi)
								{
									rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
									msg.Format("ROI#%d", s);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);

									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}
								double t_angle=Cal_Angle(t_morph(t_Sub_ROI), Cam_num);
								if (t_angle == 180)
								{
									t_angle = 0;
								}
								else
								{
									t_angle += 90-360;
								}

								if (BOLT_Param[Cam_num].nAngleHeightHeight[0] == 0)
								{
									t_angle = 0;
								}
								int m_max_object_num = -1;int m_max_object_value = 0;

								Mat Target_Thres_ROI_Gray_Img;

								if (t_angle == 0)
								{
									Target_Thres_ROI_Gray_Img = Gray_Img[Cam_num].clone();
								}
								else
								{
									J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Target_Thres_ROI_Gray_Img,1);
								}
								BOLT_Param[Cam_num].Gray_Obj_Img = Target_Thres_ROI_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
								cvtColor(Target_Thres_ROI_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

								if (t_angle == 0)
								{
									Target_Thres_ROI_Gray_Img = Out_binary.clone();
								}
								else
								{
									J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
								}
								
								//J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
								//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);

								Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();
								//J_Rotate_PRE(Out_binary,t_angle,Rotate_Target_Thres_ROI_Gray_Img,1);

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
									rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
									//msg.Format("Obj. ROI");
									//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,0,255), 1, 8);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
									rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
								}

								BOLT_Param[Cam_num].Thres_Obj_Img = Rotate_Target_Thres_ROI_Gray_Img.clone();

								// 해드와 사이드 합침
								findContours( Rotate_Target_Thres_ROI_Gray_Img.clone(), BOLT_Param[Cam_num].Object_contours, BOLT_Param[Cam_num].Object_hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								if (BOLT_Param[Cam_num].Object_contours.size() == 0)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
									}

									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}

								m_max_object_num = -1;
								m_max_object_value = 0;
								Rect tt_rect;Rect t_Side_rect;
								for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
								{
									tt_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
									if (m_max_object_value<= tt_rect.width*tt_rect.height 
										)//&& tt_rect.x > 1 && tt_rect.y >= 0 && tt_rect.x + tt_rect.width < Out_binary.cols-2 && tt_rect.y + tt_rect.height < Out_binary.rows-2)
									{
										m_max_object_value = tt_rect.width*tt_rect.height;
										m_max_object_num = k;
									}
								}

								if (m_max_object_num == -1)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
										msg.Format("ROI#%d", s);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									}
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									Result_Info[Cam_num] = "";
									for (int ss=1;ss<41;ss++)
									{
											CString t_CString;
											t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
											Result_Info[Cam_num] += t_CString;
										//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									Alg_Run_Check[Cam_num] = false;
									return true;
								}
								BOLT_Param[Cam_num].Object_1st_idx = m_max_object_num;
								t_Side_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_max_object_num]) );

								if (m_max_object_num >= 0)
								{
									int left = t_Side_rect.x;
									int top = t_Side_rect.y;
									int width = t_Side_rect.width;
									int height = t_Side_rect.height;

									int x = left + width / 2;
									int y = top + height / 2;

									if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
									{
										x = left; y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
									{
										x = left + width; y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
									{
										x = left; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
									{
										x = left; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
									{
										x = left + width; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
									{
										x = left + width; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
									{
										x = left + width/2; y = top;
									}
									BOLT_Param[Cam_num].Offset_Object_Postion = Point(x - BOLT_Param[Cam_num].Object_Postion.x, y - BOLT_Param[Cam_num].Object_Postion.y);

									//BOLT_Param[Cam_num].Offset_Object_Postion = Point(t_Side_rect.x - BOLT_Param[Cam_num].Object_Postion.x,
									//	t_Side_rect.y - BOLT_Param[Cam_num].Object_Postion.y);
									BOLT_Param[Cam_num].Thres_Obj_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										vector<vector<Point> > total_contours(1);
										for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
										{
											total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
										}
										tt_rect = boundingRect( Mat(total_contours[0]) );
										Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
										//t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
										//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
										//t_Head_rect.x-=1;t_Head_rect.y-=1;t_Head_rect.width+=2;t_Head_rect.height+=2;
										//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										vector<vector<Point> > total_contours(1);
										for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
										{
											total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
										}
										BOLT_Param[Cam_num].Thres_Obj_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
										drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

										tt_rect = boundingRect( Mat(total_contours[0]) );
										Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
										t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
									}
									//BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
									//BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Target_Thres_ROI_Gray_Img.clone();
									if (Result_Debugging)
									{
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp",s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Thres_Img.bmp",s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
									}
								}
								else
								{
									BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
									BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
									if (Result_Debugging)
									{
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp",s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Thres_Img.bmp",s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
									}
								}
							}
						}
					}
				}
				continue;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [끝] ROI Offset 계산
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [시작] 측정 알고리즘
			// 측정 방향 0:Horizontal Length, 
			//			 1:Vertical Length, 
			//           2:Cross Dimension, 
			//           3:Diameter, 
			//           4:Brightness of Area
			//           5:Difference of Brightness
			//           6:BLOB Size
			//			 7:Edge "C2
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			vector<double> dist_vec;
			double v_Dist = 0;
			if (BOLT_Param[Cam_num].nCamPosition == 0)
			{// TOP, BOTTOM
				if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::W_LENGTH_TB) // Horizontal Length
				{
					int start_p = -1;
					int end_p = -1;
					int ii = 0;
					bool check_angle_line_init = false;
					for(int i=0;i<Out_binary.rows;i++)
					{
						ii = i;
						if (BOLT_Param[Cam_num].nCalAngle[s] != 0)
						{
							ii += (int)((double)Out_binary.cols*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
						}

						if (start_p == -1)
						{
							for(int j=0;j<Out_binary.cols;j++)
							{
								if (Out_binary.at<uchar>(i,j) == 255)
								{
									start_p = j;
									break;
								}
							}
						}
						if (ii >= 0 && ii < Out_binary.rows)
						{
							if (!check_angle_line_init)
							{
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
								//	//AfxMessageBox(msg);
								//	line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),2);
								//}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),1);
								}
								check_angle_line_init = true;
							}
							if (start_p != -1 && end_p == -1)
							{
								for(int j=Out_binary.cols-1;j>=0;j--)
								{
									ii = i;
									if (BOLT_Param[Cam_num].nCalAngle[s] != 0)
									{
										ii += (int)((double)(Out_binary.cols-start_p-((Out_binary.cols-1)-j))*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
									}
									if (ii >= 0 && ii < Out_binary.rows)
									{
										if (Out_binary.at<uchar>(ii,j) == 255)
										{
											end_p = j;
											break;
										}
									}
								}
							}
						}
						if (start_p != -1 && end_p != -1)
						{
							v_Dist = sqrt((double)(start_p - end_p)*(double)(start_p - end_p)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]
							+ (double)(i - ii)*(double)(i - ii)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

							if (v_Dist >= BOLT_Param[Cam_num].nCalMinDist[s] && v_Dist <= BOLT_Param[Cam_num].nCalMaxDist[s])
							{
								dist_vec.push_back(v_Dist);
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(start_p,i),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(end_p,ii),1,CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(start_p,i),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(end_p,ii),1,CV_RGB(255,0,0),1);
								}
							}
						}
						start_p = -1;end_p = -1;
					}
				} 
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::H_LENGTH_TB) // Vertical Length
				{
					int start_p = -1;
					int end_p = -1;
					int jj = 0;
					bool check_angle_line_init = false;

					for(int j=0;j<Out_binary.cols;j++)
					{
						jj = j;
						if (BOLT_Param[Cam_num].nCalAngle[s] != 0)
						{
							jj += (int)((double)Out_binary.rows*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
						}

						if (start_p == -1)
						{
							for(int i=0;i<Out_binary.rows;i++)
							{
								if (Out_binary.at<uchar>(i,j) == 255)
								{
									start_p = i;
									break;
								}
							}
						}

						if (jj >= 0 && jj < Out_binary.cols)
						{
							if (!check_angle_line_init)
							{
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
								//	//AfxMessageBox(msg);
								//	line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),2);
								//}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),1);
								}
								check_angle_line_init = true;
							}
							if (start_p > -1 && end_p == -1)
							{
								for(int i=Out_binary.rows-1;i>=0;i--)
								{
									jj = j+(int)((double)(Out_binary.rows-start_p-((Out_binary.rows-1)-i))*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
									if (jj >= 0 && jj < Out_binary.cols)
									{
										if (Out_binary.at<uchar>(i,jj) == 255)
										{
											end_p = i;
											break;
										}
									}
								}
							}
						}
						if (start_p != -1 && end_p != -1)
						{
							v_Dist = sqrt((double)(start_p - end_p)*(double)(start_p - end_p)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]
							+ (double)(j - jj)*(double)(j - jj)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]);
							if (v_Dist >= BOLT_Param[Cam_num].nCalMinDist[s] && v_Dist <= BOLT_Param[Cam_num].nCalMaxDist[s])
							{
								dist_vec.push_back(v_Dist);
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,start_p),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(jj,end_p),1,CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,start_p),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(jj,end_p),1,CV_RGB(255,0,0),1);
								}
							}
						}
						start_p = -1;end_p = -1;
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::SIZE_CROSS_TB) // Cross Dimension
				{
					//J_Delete_Boundary(Out_binary,1);
					//J_Fill_Hole(Out_binary);

					if (BOLT_Param[Cam_num].nCrossOutput[s] == 0)
					{// 반지름으로 출력
						//if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 0)
						//{//십자 측정
						//	dilate(Out_binary,Out_binary,element,Point(-1,-1),3);
						//	erode(Out_binary,Out_binary,element,Point(-1,-1),3);
						//	J_Delete_Boundary(Out_binary,1);
						//	J_Fill_Hole(Out_binary);

						//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//	int m_max_object_num = -1;int m_max_object_value = 0;
						//	RotatedRect minRect;
						//	Point2f rect_points[4];
						//	double t_angle=0;Rect tt_rect;
						//	for( int k = 0; k < contours.size(); k++ )
						//	{  
						//		tt_rect = boundingRect( Mat(contours[k]) );
						//		minRect = minAreaRect(contours[k]);
						//		if (m_max_object_value <= minRect.size.width*minRect.size.height
						//			&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
						//			&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
						//		{
						//			m_max_object_value =  minRect.size.width*minRect.size.height;
						//			m_max_object_num = k;
						//		}
						//	}
						//	if (m_max_object_num >= 0)
						//	{
						//		minRect = minAreaRect(contours[m_max_object_num]);
						//		Point2f rect_points[4];
						//		minRect.points( rect_points );
						//		if (m_Text_View[Cam_num] && !ROI_Mode)
						//		{
						//			//for( int j = 0; j < 4; j++ )
						//			//{
						//			//	line( Dst_Img[Cam_num], Point2f(rect_points[j].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[j].y + BOLT_Param[Cam_num].nRect[s].y) , 
						//			//	Point2f(rect_points[(j+1)%4].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[(j+1)%4].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
						//			//}
						//		}
						//		if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//		{
						//			drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
						//			for( int j = 0; j < 4; j++ )
						//			{
						//				line( Dst_Img[Cam_num], Point2f(rect_points[j].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[j].y + BOLT_Param[Cam_num].nRect[s].y) , 
						//					Point2f(rect_points[(j+1)%4].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[(j+1)%4].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
						//			}
						//		}

						//		Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
						//		drawContours( Out_binary,  contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

						//		Mat Dist_Img, label;//, //Tmp_Img;
						//		distanceTransform(Out_binary, Dist_Img,label, CV_DIST_L2,3);
						//		double minval, maxval;
						//		cv::Point minLoc, CenterLoc;
						//		cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);

						//		erode(Out_binary,Dist_Img,element,Point(-1,-1),1);
						//		subtract(Out_binary,Dist_Img,Dist_Img);
						//		label = Mat::zeros(Out_binary.size(), CV_8UC1);
						//		for( int j = 0; j < 4; j++ )
						//		{
						//			line( label, Point2f(rect_points[j].x,rect_points[j].y) , 
						//				CenterLoc,CV_RGB(255,255,255), 1, 8 );
						//		}
						//		bitwise_and(label,Dist_Img,Dist_Img);
						//		Mat stats, centroids;  
						//		int numOfLables = connectedComponentsWithStats(Dist_Img, label,   
						//			stats, centroids, 8,CV_32S);
						//		for (int j = 1; j < numOfLables; j++) 
						//		{
						//			double x = centroids.at<double>(j, 0); //중심좌표
						//			double y = centroids.at<double>(j, 1);	
						//			dist_vec.push_back(0.5*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*sqrt(((double)CenterLoc.x-x)*((double)CenterLoc.x-x) + ((double)CenterLoc.y-y)*((double)CenterLoc.y-y)));
						//			if (m_Text_View[Cam_num] && !ROI_Mode)
						//			{
						//				//line( Dst_Img[Cam_num], Point2f(x + BOLT_Param[Cam_num].nRect[s].x,y + BOLT_Param[Cam_num].nRect[s].y) , 
						//				//	Point2f(CenterLoc.x + BOLT_Param[Cam_num].nRect[s].x,CenterLoc.y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(0,100,255), 1, 8 );
						//				//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(x,y),2,CV_RGB(255,0,0),1);
						//				//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
						//				//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.35, CV_RGB(255,100,0), 1, 8);
						//			}
						//			if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//			{
						//				line( Dst_Img[Cam_num], Point2f(x + BOLT_Param[Cam_num].nRect[s].x,y + BOLT_Param[Cam_num].nRect[s].y) , 
						//					Point2f(CenterLoc.x + BOLT_Param[Cam_num].nRect[s].x,CenterLoc.y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(0,100,255), 1, 8 );
						//				circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(x,y),2,CV_RGB(255,0,0),1);
						//				msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
						//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.35, CV_RGB(255,100,0), 1, 8);
						//			}
						//		}
						//		if (m_Text_View[Cam_num] && !ROI_Mode)
						//		{
						//			//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(255,100,0),1);
						//		}
						//		if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//		{
						//			circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(255,100,0),1);
						//		}
						//	}
						//}
						//else if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 1)
						//{// 다각 측정
						//	//imwrite("00.bmp",Out_binary);
						//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//	int m_max_object_num = -1;int m_max_object_value = 0;
						//	RotatedRect minRect;
						//	Rect tt_rect;
						//	for( int k = 0; k < contours.size(); k++ )
						//	{  
						//		tt_rect = boundingRect( Mat(contours[k]) );
						//		minRect = minAreaRect(contours[k]);
						//		if (m_max_object_value <= minRect.size.width*minRect.size.height
						//			&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
						//			&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
						//		{
						//			m_max_object_value =  minRect.size.width*minRect.size.height;
						//			m_max_object_num = k;
						//		}
						//	}
						//	if (m_max_object_num >= 0)
						//	{
						//		if (m_Text_View[Cam_num] && !ROI_Mode)
						//		{
						//			//for( int j = 0; j < 4; j++ )
						//			//{
						//			//	line( Dst_Img[Cam_num], Point2f(rect_points[j].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[j].y + BOLT_Param[Cam_num].nRect[s].y) , 
						//			//	Point2f(rect_points[(j+1)%4].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[(j+1)%4].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
						//			//}
						//		}
						//		if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//		{
						//			drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
						//		}

						//		Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
						//		drawContours( Out_binary,  contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
						//		//imwrite("01.bmp",Out_binary);
						//		RotatedRect Incircle_Info = fitEllipse(Mat(contours[m_max_object_num]));

						//		//Mat Dist_Img, label;//, //Tmp_Img;
						//		//distanceTransform(Out_binary, Dist_Img,label, CV_DIST_L2,3);
						//		double minval =0, maxval=0;
						//		cv::Point minLoc, CenterLoc;
						//		//cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);
						//		maxval = max(Incircle_Info.size.width,Incircle_Info.size.height)/2;
						//		CenterLoc = Incircle_Info.center;
						//		if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//		{
						//			circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
						//			circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
						//		}

						//		circle(Out_binary, CenterLoc,maxval,CV_RGB(0,0,0),CV_FILLED,8);

						//		dilate(Out_binary,Out_binary,element,Point(-1,-1),1);
						//		erode(Out_binary,Out_binary,element,Point(-1,-1),1);

						//		Mat Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);

						//		findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//		Point2f rect_points[4];
						//		dist_vec.clear();
						//		// 각도 계산
						//		vector<double> vCX;vector<double> vCY;
						//		vector<double> vCMag;vector<double> vCAngle;
						//		vector<double> vBX;vector<double> vBY;
						//		vector<double> vBMag;vector<double> vBAngle;

						//		for (int k = 0; k < contours.size(); k++)
						//		{  
						//			Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
						//			tt_rect = boundingRect( Mat(contours[k]) );
						//			drawContours( Line_Img,  contours, k, CV_RGB(255,255,255), 1, 1, hierarchy);
						//			minRect = minAreaRect(contours[k]);

						//			vCX.clear();vCY.clear();
						//			vCX.push_back(minRect.center.x - CenterLoc.x);
						//			vCY.push_back(minRect.center.y - CenterLoc.y);
						//			vCMag.clear();vCAngle.clear();

						//			if (vCX.size() > 0)
						//			{
						//				cartToPolar(vCX, vCY, vCMag, vCAngle, true); // mag에는 vector의 크기, angle에는 0~360도의 값이 들어감.  
						//			}

						//			vBX.clear();vBY.clear();
						//			vBMag.clear();vBAngle.clear();
						//			for (int x=tt_rect.x-1;x<tt_rect.x+tt_rect.width+2;x++)
						//			{
						//				for (int y=tt_rect.y-1;y<tt_rect.y+tt_rect.height+2;y++)
						//				{
						//					if (x>=0 && x<Line_Img.cols && y>=0 && y<Line_Img.rows)
						//					{
						//						if (Line_Img.at<uchar>(y,x) == 255)
						//						{
						//							vBX.push_back((double)x - CenterLoc.x);
						//							vBY.push_back((double)y - CenterLoc.y);
						//						}
						//					}
						//				}
						//			}

						//			if (vBX.size() > 0)
						//			{
						//				cartToPolar(vBX, vBY, vBMag, vBAngle, true); // mag에는 vector의 크기, angle에는 0~360도의 값이 들어감.  
						//			}

						//			double t_max = 0;double t_d = 0;
						//			for(int ii=0; ii<vBX.size(); ii++)
						//			{
						//				if (abs(vCAngle[0] - vBAngle[ii]) < 1.1 && vCMag[0] < vBMag[ii])
						//				{
						//					t_max = vBMag[ii]*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])/2;
						//					minLoc.x = (int)(vBX[ii] + CenterLoc.x);minLoc.y = (int)(vBY[ii] + CenterLoc.y);
						//				}
						//			}


						//			////minRect = minAreaRect(contours[k]);
						//			////minRect.points( rect_points );
						//			////for (int j = 0; j < 4; j++)
						//			////{
						//			////	line( Line_Img, Point2f(rect_points[j].x,rect_points[j].y) , 
						//			////	Point2f(rect_points[(j+1)%4].x,rect_points[(j+1)%4].y),CV_RGB(255,255,255), 3, 8 );
						//			////}
						//			//bitwise_and(Line_Img,Out_binary,Line_Img);
						//			//double t_max = 0;double t_d = 0;
						//			//for (int x=tt_rect.x-1;x<tt_rect.x+tt_rect.width+2;x++)
						//			//{
						//			//	for (int y=tt_rect.y-1;y<tt_rect.y+tt_rect.height+2;y++)
						//			//	{
						//			//		if (x>=0 && x<Line_Img.cols && y>=0 && y<Line_Img.rows)
						//			//		{
						//			//			if (Line_Img.at<uchar>(y,x) == 255)
						//			//			{
						//			//				t_d = sqrt(((double)x-(double)CenterLoc.x)*((double)x-(double)CenterLoc.x)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]+((double)y-(double)CenterLoc.y)*((double)y-(double)CenterLoc.y)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
						//			//				if (t_d > t_max)
						//			//				{
						//			//					t_max = t_d;
						//			//					minLoc.x = x;minLoc.y = y; 
						//			//				}
						//			//			}
						//			//		}
						//			//	}
						//			//}
						//			if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//			{
						//				line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc, minLoc,CV_RGB(255,100,0), 1, 1 );
						//				circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), minLoc,1,CV_RGB(0,0,255),1);
						//				msg.Format("%1.3f",t_max);
						//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + minLoc.x + 3,BOLT_Param[Cam_num].nRect[s].y + minLoc.y), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
						//			}
						//			if (m_Text_View[Cam_num] && !ROI_Mode)
						//			{
						//				line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc, minLoc,CV_RGB(255,100,0), 1, 1 );
						//				circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), minLoc,1,CV_RGB(0,0,255),1);
						//				msg.Format("%1.3f",t_max);
						//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + minLoc.x + 3,BOLT_Param[Cam_num].nRect[s].y + minLoc.y), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
						//			}
						//			dist_vec.push_back(t_max);
						//		}
						//		for (int ii=dist_vec.size()-1;ii>=0;ii--)
						//		{
						//			if (ii >= BOLT_Param[Cam_num].nCrossAngleNumber[s])
						//			{
						//				dist_vec.erase(dist_vec.begin()+ii);
						//			}
						//		}
						//		for (int ii=0;ii<BOLT_Param[Cam_num].nCrossAngleNumber[s];ii++)
						//		{
						//			if (ii >= dist_vec.size())
						//			{
						//				dist_vec.push_back(0);
						//			}
						//		}
						//		//imwrite("01.bmp",Out_binary);
						//		//imwrite("02.bmp",Line_Img);
						//	}
						//}

						if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 0)
						{//십자 측정
							BOLT_Param[Cam_num].nCrossAngleNumber[s] = 4;
						}
						if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 0 || BOLT_Param[Cam_num].nCrossSizeMethod[s] == 1)
						{// 다각 측정
							//imwrite("00.bmp",Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							int m_max_object_num = -1;int m_max_object_value = 0;
							RotatedRect minRect;
							Rect tt_rect;
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{  
									//if (hierarchy[k][3] == -1)
									{
										tt_rect = boundingRect( Mat(contours[k]) );
										minRect = minAreaRect(contours[k]);
										if (m_max_object_value <= minRect.size.width*minRect.size.height
											&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
											&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
										{
											m_max_object_value =  minRect.size.width*minRect.size.height;
											m_max_object_num = k;
										}
									}
								}
							}
							if (m_max_object_num >= 0)
							{
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
								}
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
								}
								//imwrite("00.bmp",Out_binary);
								Mat Edge_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
								vector<vector<Point>> hull1(1);
								convexHull( Mat(contours[m_max_object_num]), hull1[0], false );
								drawContours( Out_binary, hull1, 0, CV_RGB(255,255,255), 1, CV_AA, vector<Vec4i>(), 0, Point() );
								//imwrite("00.bmp",Out_binary);
								drawContours( Edge_Img, contours, m_max_object_num, CV_RGB(255,255,255), 1, CV_AA, vector<Vec4i>(), 0, Point() );
								//imwrite("01.bmp",Edge_Img);
								RotatedRect Incircle_Info = fitEllipse(Mat(hull1[0]));
								Rect m_max_rect = boundingRect( Mat(hull1[0]) );
								double minval =0, maxval=0;
								cv::Point minLoc, CenterLoc;
								maxval = max(Incircle_Info.size.width,Incircle_Info.size.height)/2;
								CenterLoc = Incircle_Info.center;
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
								}
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
								}
								cv::Point t_max_dist_point(0,0);
								double t_max = 0;double t_max_temp = 0;
								for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
								{
									for (int jj=m_max_rect.y;jj<m_max_rect.y+m_max_rect.height;jj++)
									{
										if (Out_binary.at<uchar>(jj,ii) > 0)
										{
											t_max_temp = sqrt(((Incircle_Info.center.x - (double)ii)*(Incircle_Info.center.x - (double)ii) + (Incircle_Info.center.y - (double)jj)*(Incircle_Info.center.y - (double)jj)));
											if (t_max_temp >= t_max)
											{
												t_max = t_max_temp;
												t_max_dist_point = cv::Point(ii,jj);
											}
										}
									}
								}

								// 최대 위치 보정이 필요
								Mat Our_Circle_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								circle(Our_Circle_Img, Incircle_Info.center,t_max,CV_RGB(255,255,255),3);
								bitwise_and(Our_Circle_Img, Out_binary, Our_Circle_Img);
								dilate(Our_Circle_Img,Our_Circle_Img,element,Point(-1,-1), 1);
								//imwrite("02.bmp",Our_Circle_Img);
								findContours( Our_Circle_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								double t_min = 99999;cv::Point2f t_max_dist_point2(0,0);
								if (contours.size() > 0)
								{
									for (int k = 0; k < contours.size(); k++)
									{  
										minRect = minAreaRect(contours[k]);
										t_max_temp =  sqrt(((minRect.center.x - (double)t_max_dist_point.x)*(minRect.center.x - (double)t_max_dist_point.x) + (minRect.center.y - (double)t_max_dist_point.y)*(minRect.center.y - (double)t_max_dist_point.y)));
										if (t_min >= t_max_temp)
										{
											t_min = t_max_temp;
											t_max_dist_point2 = cv::Point2f((1.1)*(minRect.center.x -Incircle_Info.center.x) + Incircle_Info.center.x, (1.1)*(minRect.center.y -Incircle_Info.center.y) + Incircle_Info.center.y);
										}
									}

									Mat Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									cv::Point2f t_max_dist_point_rotate(0,0);
									cv::Point2f t_max_dist_point_R(1.05f*(t_max_dist_point2.x-Incircle_Info.center.x),1.05f*(t_max_dist_point2.y-Incircle_Info.center.y));
									vector<cv::Point2f> t_vec_point;bool t_loop_check = false;

									for (int k=0;k<BOLT_Param[Cam_num].nCrossAngleNumber[s];k++)
									{
										Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
										t_max_dist_point_rotate.x = cos(((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s]))*t_max_dist_point_R.x
											+ sin((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;
										t_max_dist_point_rotate.y = -sin((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.x
											+ cos((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;

										Point2f P0(t_max_dist_point_rotate.x + Incircle_Info.center.x,t_max_dist_point_rotate.y + Incircle_Info.center.y);
										line( Line_Img, Incircle_Info.center, P0 ,CV_RGB(255,255,255), 1, 1 );
										bitwise_and(Line_Img, Edge_Img, Line_Img);
										for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
										{
											t_loop_check = false;
											for (int jj=m_max_rect.y;jj<m_max_rect.y+m_max_rect.height;jj++)
											{
												if (Line_Img.at<uchar>(jj,ii) > 0)
												{
													t_vec_point.push_back(cv::Point2f((float)ii,(float)jj));
													t_loop_check = true;
													break;
												}
											}
											if (t_loop_check)
											{
												break;
											}
										}
									}

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,1,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,2,CV_RGB(0,0,255),1);
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,1,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,2,CV_RGB(0,0,255),1);
									}
									dist_vec.clear();
									if (t_vec_point.size() > 0 && t_vec_point.size() % BOLT_Param[Cam_num].nCrossAngleNumber[s] == 0)
									{
										for (int k=0;k<BOLT_Param[Cam_num].nCrossAngleNumber[s];k++)
										{
											if (k < t_vec_point.size())
											{
												t_max = sqrt((BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]*(t_vec_point[k].x - Incircle_Info.center.x)*(t_vec_point[k].x - Incircle_Info.center.x) 
													+ BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]*(t_vec_point[k].y - Incircle_Info.center.y)*(t_vec_point[k].y - Incircle_Info.center.y)));
												dist_vec.push_back(t_max);
												if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
												{
													line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k], Incircle_Info.center,CV_RGB(255,100,0), 1, 1 );
													circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],1,CV_RGB(255,0,0),1);
													circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],2,CV_RGB(0,0,255),1);
													msg.Format("%1.3f",t_max);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + t_vec_point[k].x + 3,BOLT_Param[Cam_num].nRect[s].y + t_vec_point[k].y), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,150,255), 1, 8);
												}
												if (m_Text_View[Cam_num] && !ROI_Mode)
												{
													line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k], Incircle_Info.center,CV_RGB(255,100,0), 1, 1 );
													circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],1,CV_RGB(255,0,0),1);
													circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],2,CV_RGB(0,0,255),1);
													msg.Format("%1.3f",t_max);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + t_vec_point[k].x + 3,BOLT_Param[Cam_num].nRect[s].y + t_vec_point[k].y), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,150,255), 1, 8);
												}
											}
										}
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nCrossOutput[s] == 1)
					{// 직경으로 출력
						if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 0)
						{//십자 측정
							BOLT_Param[Cam_num].nCrossAngleNumber[s] = 4;
						}
						if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 0 || BOLT_Param[Cam_num].nCrossSizeMethod[s] == 1)
						{// 다각 측정
							//imwrite("00.bmp",Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							int m_max_object_num = -1;int m_max_object_value = 0;
							RotatedRect minRect;
							Rect tt_rect;
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{  
									tt_rect = boundingRect( Mat(contours[k]) );
									minRect = minAreaRect(contours[k]);
									if (m_max_object_value <= minRect.size.width*minRect.size.height
										&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
										&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
									{
										m_max_object_value =  minRect.size.width*minRect.size.height;
										m_max_object_num = k;
									}
								}
							}
							if (m_max_object_num >= 0)
							{
								//imwrite("00.bmp",Out_binary);
								Mat Edge_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
								vector<vector<Point>> hull1(1);
								convexHull( Mat(contours[m_max_object_num]), hull1[0], false );
								drawContours( Out_binary, hull1, 0, CV_RGB(255,255,255), 1, CV_AA, vector<Vec4i>(), 0, Point() );
								//imwrite("00.bmp",Out_binary);
								drawContours( Edge_Img, contours, m_max_object_num, CV_RGB(255,255,255), 1, CV_AA, vector<Vec4i>(), 0, Point() );
								//imwrite("01.bmp",Edge_Img);
								RotatedRect Incircle_Info = fitEllipse(Mat(hull1[0]));
								Rect m_max_rect = boundingRect( Mat(hull1[0]) );
								double minval =0, maxval=0;
								cv::Point minLoc, CenterLoc;
								maxval = max(Incircle_Info.size.width,Incircle_Info.size.height)/2;
								CenterLoc = Incircle_Info.center;
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
								}
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
								}
								cv::Point t_max_dist_point(0,0);
								double t_max = 0;double t_max_temp = 0;
								for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
								{
									for (int jj=m_max_rect.y;jj<m_max_rect.y+m_max_rect.height;jj++)
									{
										if (Out_binary.at<uchar>(jj,ii) > 0)
										{
											t_max_temp = sqrt(((Incircle_Info.center.x - (double)ii)*(Incircle_Info.center.x - (double)ii) + (Incircle_Info.center.y - (double)jj)*(Incircle_Info.center.y - (double)jj)));
											if (t_max_temp >= t_max)
											{
												t_max = t_max_temp;
												t_max_dist_point = cv::Point(ii,jj);
											}
										}
									}
								}

								// 최대 위치 보정이 필요
								Mat Our_Circle_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								circle(Our_Circle_Img, Incircle_Info.center,t_max,CV_RGB(255,255,255),3);
								bitwise_and(Our_Circle_Img, Out_binary, Our_Circle_Img);
								dilate(Our_Circle_Img,Our_Circle_Img,element,Point(-1,-1), 1);
								//imwrite("02.bmp",Our_Circle_Img);
								findContours( Our_Circle_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								double t_min = 99999;cv::Point2f t_max_dist_point2(0,0);
								if (contours.size() > 0)
								{
									for (int k = 0; k < contours.size(); k++)
									{  
										minRect = minAreaRect(contours[k]);
										t_max_temp =  sqrt(((minRect.center.x - (double)t_max_dist_point.x)*(minRect.center.x - (double)t_max_dist_point.x) + (minRect.center.y - (double)t_max_dist_point.y)*(minRect.center.y - (double)t_max_dist_point.y)));
										if (t_min >= t_max_temp)
										{
											t_min = t_max_temp;
											t_max_dist_point2 = cv::Point2f((1.1)*(minRect.center.x -Incircle_Info.center.x) + Incircle_Info.center.x, (1.1)*(minRect.center.y -Incircle_Info.center.y) + Incircle_Info.center.y);
										}
									}
								}

								Mat Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								cv::Point2f t_max_dist_point_rotate(0,0);
								cv::Point2f t_max_dist_point_R(1.05f*(t_max_dist_point2.x-Incircle_Info.center.x),1.05f*(t_max_dist_point2.y-Incircle_Info.center.y));
								vector<cv::Point2f> t_vec_point;bool t_loop_check = false;

								for (int k=0;k<BOLT_Param[Cam_num].nCrossAngleNumber[s];k++)
								{
									Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									t_max_dist_point_rotate.x = cos(((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s]))*t_max_dist_point_R.x
										                      + sin((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;
									t_max_dist_point_rotate.y = -sin((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.x
										+ cos((double)k*2.0*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;

									Point2f P0(t_max_dist_point_rotate.x + Incircle_Info.center.x,t_max_dist_point_rotate.y + Incircle_Info.center.y);
									line( Line_Img, Incircle_Info.center, P0 ,CV_RGB(255,255,255), 1, 1 );
									bitwise_and(Line_Img, Edge_Img, Line_Img);
									for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
									{
										t_loop_check = false;
										for (int jj=m_max_rect.y;jj<m_max_rect.y+m_max_rect.height;jj++)
										{
											if (Line_Img.at<uchar>(jj,ii) > 0)
											{
												t_vec_point.push_back(cv::Point2f((float)ii,(float)jj));
												t_loop_check = true;
												break;
											}
										}
										if (t_loop_check)
										{
											break;
										}
									}
								}

								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,2,CV_RGB(0,0,255),1);
								}
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Incircle_Info.center,2,CV_RGB(0,0,255),1);
								}

								dist_vec.clear();
								if (t_vec_point.size() > 0 && t_vec_point.size() % BOLT_Param[Cam_num].nCrossAngleNumber[s] == 0)
								{
									for (int k=0;k<BOLT_Param[Cam_num].nCrossAngleNumber[s]/2;k++)
									{
										if (k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2 < t_vec_point.size())
										{
											t_max = sqrt((BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]*(t_vec_point[k].x - t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2].x)*(t_vec_point[k].x - t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2].x) 
														+ BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]*(t_vec_point[k].y - t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2].y)*(t_vec_point[k].y - t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2].y)));
											dist_vec.push_back(t_max);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k], t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2],CV_RGB(255,100,0), 1, 1 );
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2],1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2],2,CV_RGB(0,0,255),1);
												msg.Format("%1.3f",t_max);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + t_vec_point[k].x + 3,BOLT_Param[Cam_num].nRect[s].y + t_vec_point[k].y), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,150,255), 1, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k], t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2],CV_RGB(255,100,0), 1, 1 );
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k],2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2],1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_vec_point[k+BOLT_Param[Cam_num].nCrossAngleNumber[s]/2],2,CV_RGB(0,0,255),1);
												msg.Format("%1.3f",t_max);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + t_vec_point[k].x + 3,BOLT_Param[Cam_num].nRect[s].y + t_vec_point[k].y), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,150,255), 1, 8);
											}
										}
									}
								}
							}
						}
					}

					if (BOLT_Param[Cam_num].nCrossSizeMethod[s] == 2)
					{// 내부 최대 Diameter 측정
						//imwrite("00.bmp",Out_binary);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						int m_max_object_num = -1;int m_max_object_value = 0;
						RotatedRect minRect;
						Rect tt_rect;
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{  
								tt_rect = boundingRect( Mat(contours[k]) );
								minRect = minAreaRect(contours[k]);
								if (m_max_object_value <= minRect.size.width*minRect.size.height
									&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
									&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
								{
									m_max_object_value =  minRect.size.width*minRect.size.height;
									m_max_object_num = k;
								}
							}
						}
						if (m_max_object_num >= 0)
						{
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								//for( int j = 0; j < 4; j++ )
								//{
								//	line( Dst_Img[Cam_num], Point2f(rect_points[j].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[j].y + BOLT_Param[Cam_num].nRect[s].y) , 
								//	Point2f(rect_points[(j+1)%4].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[(j+1)%4].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
								//}
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
							}
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
							}
							Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
							drawContours( Out_binary,  contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
							//imwrite("01.bmp",Out_binary);
							
							dilate(Out_binary,Out_binary,element,Point(-1,-1),5);
							erode(Out_binary,Out_binary,element,Point(-1,-1),5);
							J_Delete_Boundary(Out_binary,1);

							Mat Dist_Img, label;//, //Tmp_Img;
							distanceTransform(Out_binary, Dist_Img,label, CV_DIST_L2,3);
							double minval, maxval;
							cv::Point minLoc, CenterLoc;
							cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,maxval,CV_RGB(255,0,0),1);
								msg.Format("%1.3f",maxval*((BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])/2));
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + CenterLoc.x + 3,BOLT_Param[Cam_num].nRect[s].y + CenterLoc.y), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,150,255), 1, 8);
							}
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(255,0,0),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(0,0,255),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,maxval,CV_RGB(255,0,0),1);
								msg.Format("%1.3f",maxval*((BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])/2));
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + CenterLoc.x + 3,BOLT_Param[Cam_num].nRect[s].y + CenterLoc.y), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,150,255), 1, 8);
							}
							dist_vec.push_back(maxval*((BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])/2));
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::DIAMETER_TB) // DIAMETER
				{
					J_Delete_Boundary(Out_binary,1);

					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1),BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1),BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						J_Delete_Boundary(Out_binary,1);
					}
					//J_Fill_Hole(Out_binary);
					dist_vec.clear();
					double V_R = 0;
					if (BOLT_Param[Cam_num].nDiameter_Method[s] == 5)
					{
						Point2f P_Center;float R_radius;
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if (contours.size() > 0)
						{
							for(int k = 0; k < contours.size(); k++)
							{
								//if (hierarchy[k][3] == -1)
								{
									RotatedRect minRect = minAreaRect(contours[k]);
									Rect t_rect = boundingRect( Mat(contours[k]) );
									//double t_v = 100.0*min(minRect.size.width,minRect.size.height) / max(minRect.size.width,minRect.size.height);
									//msg.Format("%1.2f",t_v);
									R_radius = max(max((minRect.size.width+minRect.size.height)/4, (float)t_rect.width/2), (float)t_rect.height/2);
									P_Center = minRect.center;
									//minEnclosingCircle(contours[k],P_Center,R_radius);
									V_R = (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*(double)R_radius;
									if (V_R >= BOLT_Param[Cam_num].nDiameter_Min_Size[s] && V_R <= BOLT_Param[Cam_num].nDiameter_Max_Size[s])
									{
										dist_vec.push_back(V_R);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(55,200,0),1);
											//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
											//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(255,0,0),2);
											msg.Format("%1.3f",V_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
						}
					} // if (BOLT_Param[Cam_num].nDiameter_Method[s] == 0)
					if (BOLT_Param[Cam_num].nDiameter_Method[s] == 0)
					{
						Point2f P_Center;float R_radius;reals LambdaIni=0.001;
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if (contours.size() > 0)
						{
							Circle circlef,circleIni;
							for(int k = 0; k < contours.size(); k++)
							{
								//if (hierarchy[k][3] == -1)
								{
									//RotatedRect minRect = fitEllipse(Mat(contours[k]));

									int n = contours[k].size();
									double* DataX = new double[n];
									double* DataY = new double[n];

									for(int s = 0; s < contours[k].size(); s++)
									{
										DataX[s] = contours[k][s].x;
										DataY[s] = contours[k][s].y;
									}
									Data dataXY(n,DataX,DataY);

									delete [] DataX;
									delete [] DataY;

									circleIni = CircleFitByTaubin (dataXY);
									int code = CircleFitByLevenbergMarquardtReduced (dataXY,circleIni,LambdaIni,circlef);
									if (circlef.a - circlef.r < 0 || circlef.a + circlef.r >=  Out_binary.cols || circlef.b - circlef.r < 0 || circlef.b + circlef.r >=  Out_binary.rows)
									{
										code = 1;
									}

									if (code == 0)
									{
										//RotatedRect minRect = minAreaRect(contours[k]);
										//Rect t_rect = boundingRect( Mat(contours[k]) );
										//double t_v = 100.0*min(minRect.size.width,minRect.size.height) / max(minRect.size.width,minRect.size.height);
										//msg.Format("%1.2f",t_v);
										R_radius = (float)circlef.r;//max(max((minRect.size.width+minRect.size.height)/4, (float)t_rect.width/2), (float)t_rect.height/2);
										P_Center = Point2f((float)circlef.a,(float)circlef.b);

										//minEnclosingCircle(contours[k],P_Center,R_radius);
										V_R = (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*(double)R_radius;
										if (V_R >= BOLT_Param[Cam_num].nDiameter_Min_Size[s] && V_R <= BOLT_Param[Cam_num].nDiameter_Max_Size[s])
										{
											dist_vec.push_back(V_R);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(55,200,0),1);
												//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
												//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(255,0,0),2);
												msg.Format("%1.3f",V_R);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										}
									}
								}
							}
						}

						//Mat Dist_Img, label;//, //Tmp_Img;
						//J_Fill_Hole(Out_binary);
						//distanceTransform(Out_binary, Dist_Img, label, CV_DIST_L2, 5);
						//double minval = 0, maxval = 0;
						//cv::Point minLoc, CenterLoc;
						//cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);
						//maxval += 2;
						//V_R = (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*maxval;
						//if (V_R >= BOLT_Param[Cam_num].nDiameter_Min_Size[s] && V_R <= BOLT_Param[Cam_num].nDiameter_Max_Size[s])
						//{
						//	dist_vec.push_back(V_R);
						//	if (m_Text_View[Cam_num] && !ROI_Mode)
						//	{
						//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),maxval,CV_RGB(55,200,0),1);
						//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),1,CV_RGB(55,200,0),1);
						//		//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
						//		//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
						//	}
						//	if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//	{
						//		//drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
						//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),maxval,CV_RGB(55,200,0),1);
						//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),1,CV_RGB(55,200,0),1);
						//		msg.Format("%1.3f",V_R);
						//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+CenterLoc.x+5,BOLT_Param[Cam_num].nRect[s].y+CenterLoc.y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
						//	}
						//}

						//Point2f P_Center;float R_radius;
						//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//if (contours.size() > 0)
						//{
						//	sEllipse c_ellipse;double Circle_Info[5];
						//	for(int k = 0; k < contours.size(); k++)
						//	{
						//		//if (hierarchy[k][3] == -1)
						//		{
						//			const int no_data = (int)contours[k].size();
						//			sPoint *data = new sPoint[no_data];

						//			if (no_data > 10)
						//			{
						//				for(int i=0; i<no_data; i++)
						//				{
						//					data[i].x =  (double)contours[k][i].x;
						//					data[i].y =  (double)contours[k][i].y;
						//				}

						//				double cost = ransac_ellipse_fitting (data, no_data, c_ellipse, 50);

						//				Circle_Info[0] = c_ellipse.cx;
						//				Circle_Info[1] = c_ellipse.cy;
						//				Circle_Info[2] = c_ellipse.w*2;
						//				Circle_Info[3] = c_ellipse.h*2;
						//				Circle_Info[4] = c_ellipse.theta*180/M_PI;
						//			}
						//			delete [] data;

						//			//RotatedRect minRect = minAreaRect(contours[k]);
						//			Rect t_rect = boundingRect( Mat(contours[k]) );
						//			//double t_v = 100.0*min(minRect.size.width,minRect.size.height) / max(minRect.size.width,minRect.size.height);
						//			//msg.Format("%1.2f",t_v);
						//			R_radius = (Circle_Info[2] + Circle_Info[3])/4; //max(max((minRect.size.width+minRect.size.height)/4, (float)t_rect.width/2), (float)t_rect.height/2);
						//			P_Center = Point2f(Circle_Info[0],Circle_Info[1]);
						//			//minEnclosingCircle(contours[k],P_Center,R_radius);
						//			V_R = (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*(double)R_radius;
						//			if (V_R >= BOLT_Param[Cam_num].nDiameter_Min_Size[s] && V_R <= BOLT_Param[Cam_num].nDiameter_Max_Size[s])
						//			{
						//				dist_vec.push_back(V_R);
						//				if (m_Text_View[Cam_num] && !ROI_Mode)
						//				{
						//					circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(55,200,0),1);
						//					//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
						//					//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
						//				}
						//				if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						//				{
						//					drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
						//					circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(255,0,0),2);
						//					msg.Format("%1.3f",V_R);
						//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
						//				}
						//			}
						//		}
						//	}
						//}
					} // if (BOLT_Param[Cam_num].nDiameter_Method[s] == 5)
					else if (BOLT_Param[Cam_num].nDiameter_Method[s] == 4)
					{ // Distance Transform을 이용한 측정
						Mat Dist_Img, label;//, //Tmp_Img;
						//J_Fill_Hole(Out_binary);
						distanceTransform(Out_binary, Dist_Img, label, CV_DIST_L2, 5);
						double minval = 0, maxval = 0;
						cv::Point minLoc, CenterLoc;
						cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);
						maxval += 2;
						V_R = (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*maxval;

						dist_vec.push_back(V_R);
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),maxval,CV_RGB(55,200,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),1,CV_RGB(55,200,0),1);
							//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
							//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							//drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),maxval,CV_RGB(55,200,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(CenterLoc.x,CenterLoc.y),1,CV_RGB(55,200,0),1);
							msg.Format("%1.3f",V_R);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+CenterLoc.x+5,BOLT_Param[Cam_num].nRect[s].y+CenterLoc.y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
						}
					}
					else if (BOLT_Param[Cam_num].nDiameter_Method[s] >= 1 && BOLT_Param[Cam_num].nDiameter_Method[s] <= 3)
					{ // 1:중심에서 최소거리,2:중심에서 최대거리,3:중심에서 최대-최소거리 
						double top_min_dist = 0;
						double top_max_dist = 0;
						Mat Defect_Dark_Img;
						//AfxMessageBox("0");
						J_Delete_Boundary(Out_binary,1);
						//AfxMessageBox("1");
						J_Fill_Hole(Out_binary);
						//AfxMessageBox("2");
						//imwrite("1.bmp",Out_binary);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//AfxMessageBox("3");
						if (contours.size() > 0)
						{
							//AfxMessageBox("4");
							vector<Moments> mu(contours.size());
							Rect boundRect;
							int m_max_object_num = -1;int m_max_object_value = 0;
							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect(contours[k]);
								if (m_max_object_value <= boundRect.width*boundRect.height)
									//&& boundRect.x > 1 && boundRect.y > 1 && boundRect.x+boundRect.width < Out_binary.cols-1 && boundRect.y + boundRect.height < Out_binary.rows-1)
								{
									m_max_object_value =  boundRect.width*boundRect.height;
									m_max_object_num = k;
								}
							}
							if (m_max_object_num >= 0)
							{
								vector<Point3D> t_dist;
								Point2f t_Center = Point2f( mu[m_max_object_num].m10/mu[m_max_object_num].m00 , mu[m_max_object_num].m01/mu[m_max_object_num].m00 ); 
								Mat t_M_Out_binary;
								erode(Out_binary,t_M_Out_binary,element,Point(-1,-1),1);
								subtract(Out_binary,t_M_Out_binary,t_M_Out_binary);
								boundRect = boundingRect(contours[m_max_object_num]);
								Point3D t_Point3D;
								for (int i = boundRect.x;i<boundRect.x+boundRect.width;i++)
								{
									for (int j = boundRect.y;j<boundRect.y+boundRect.height;j++)
									{
										if (t_M_Out_binary.at<uchar>(j,i) > 0)
										{
											t_Point3D.DIST = sqrt((t_Center.x - (float)i)*(t_Center.x - (float)i)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (t_Center.y - (float)j)*(t_Center.y - (float)j)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											t_Point3D.CX = (float)i;
											t_Point3D.CY = (float)j;
											t_dist.push_back(t_Point3D);
										}
									}
								}
								std::sort(t_dist.begin(), t_dist.end(), Point_compare_Dist);
								top_min_dist = (double)t_dist[0].DIST;
								//dist_vec.push_back((double)t_dist[0].DIST);
								if (t_dist.size() > 1)
								{
									//dist_vec.push_back((double)t_dist[t_dist.size()-1].DIST);
									top_max_dist = (double)t_dist[t_dist.size()-1].DIST;
								}

								if (BOLT_Param[Cam_num].nDiameter_Method[s] == 1)
								{
									dist_vec.push_back(top_min_dist);

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
											Point2f(t_dist[0].CX,t_dist[0].CY),CV_RGB(0,100,255), 1, 8 );
										msg.Format("Min.Dist(%1.3f)",top_min_dist);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+30), fontFace, 0.4, CV_RGB(0,150,255), 1, 8);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),1,CV_RGB(255,255,0),1);

										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
											Point2f(t_dist[0].CX,t_dist[0].CY),CV_RGB(0,100,255), 1, 8 );
										msg.Format("Min.Dist(%1.3f)",top_min_dist);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+30), fontFace, 0.4, CV_RGB(0,150,255), 1, 8);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),1,CV_RGB(255,255,0),1);

										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
									}
								}
								else if (BOLT_Param[Cam_num].nDiameter_Method[s] == 2)
								{
									dist_vec.push_back(top_max_dist);

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										if (t_dist.size() > 1)
										{
											line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
												Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),CV_RGB(255,100,0), 1, 8 );
											msg.Format("Max.Dist(%1.3f)",top_max_dist);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+50), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
										}
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),2,CV_RGB(0,0,255),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),1,CV_RGB(255,255,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										if (t_dist.size() > 1)
										{
											line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
												Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),CV_RGB(255,100,0), 1, 8 );
											msg.Format("Max.Dist(%1.3f)",top_max_dist);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+50), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
										}
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),2,CV_RGB(0,0,255),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),1,CV_RGB(255,255,0),1);
									}
								}
								else if (BOLT_Param[Cam_num].nDiameter_Method[s] == 3)
								{
									dist_vec.push_back(top_max_dist - top_min_dist);

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
											Point2f(t_dist[0].CX,t_dist[0].CY),CV_RGB(0,100,255), 1, 8 );
										msg.Format("Min.Dist(%1.3f)",top_min_dist);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+30), fontFace, 0.4, CV_RGB(0,150,255), 1, 8);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),1,CV_RGB(255,255,0),1);

										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);

										if (t_dist.size() > 1)
										{
											line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
												Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),CV_RGB(255,100,0), 1, 8 );
											msg.Format("Max.Dist(%1.3f)",top_max_dist);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+50), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
										}
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),2,CV_RGB(0,0,255),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),1,CV_RGB(255,255,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
											Point2f(t_dist[0].CX,t_dist[0].CY),CV_RGB(0,100,255), 1, 8 );
										msg.Format("Min.Dist(%1.3f)",top_min_dist);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+30), fontFace, 0.4, CV_RGB(0,150,255), 1, 8);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[0].CX,t_dist[0].CY),1,CV_RGB(255,255,0),1);

										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
										if (t_dist.size() > 1)
										{
											line( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center, 
												Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),CV_RGB(255,100,0), 1, 8 );
											msg.Format("Max.Dist(%1.3f)",top_max_dist);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+5,BOLT_Param[Cam_num].nRect[s].y+50), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
										}
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,2,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Center,1,CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),2,CV_RGB(0,0,255),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_dist[t_dist.size()-1].CX,t_dist[t_dist.size()-1].CY),1,CV_RGB(255,255,0),1);
									}
								}
								//if (top_min_dist > 0)
								//{
								//	top_min_dist += BOLT_Param[Cam_num].Offset[0];
								//}
								//if (top_max_dist > 0)
								//{
								//	top_max_dist += BOLT_Param[Cam_num].Offset[1];
								//}
								}
							}
						} // else if (BOLT_Param[Cam_num].nDiameter_Method[s] >= 1 && BOLT_Param[Cam_num].nDiameter_Method[s] <= 3)
				} 
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::BRIGHTNESS_AREA_TB) // Brightness of Area
				{
					//J_Delete_Boundary(Out_binary,1);
					//J_Fill_Hole(Out_binary);

					if (BOLT_Param[Cam_num].nColorMethod[s] == 0)
					{//흑백 처리

						if (BOLT_Param[Cam_num].nThres_V1[s] != 0.0f || BOLT_Param[Cam_num].nThres_V2[s] != 255.0f)
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Out_binary.at<uchar>(i,j) > 0)
									{
										dist_vec.push_back((double)CP_Gray_Img.at<uchar>(i,j));
									}
								}	
							}
						}
						else
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									//if (Out_binary.at<uchar>(i,j) > 0)
									{
										dist_vec.push_back((double)CP_Gray_Img.at<uchar>(i,j));
									}
								}	
							}
						}
					}
					else if (BOLT_Param[Cam_num].nColorMethod[s] == 1)
					{//컬러 처리
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							Mat HSV_Img1;Mat HSV_channel1[3];
							cvtColor(Src_Img[Cam_num], HSV_Img1, CV_BGR2Lab);
							//blur(HSV_Img, HSV_Img, Size(5,5));
							split(HSV_Img1, HSV_channel1);
							cvtColor(HSV_channel1[1],Dst_Img[Cam_num],CV_GRAY2BGR);
						}

						Mat HSV_Img;Mat HSV_channel[3];
						cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2Lab);
						//cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2HSV);
						//blur(HSV_Img, HSV_Img, Size(5,5));
						split(HSV_Img, HSV_channel);
						inRange(HSV_channel[1], Scalar(BOLT_Param[Cam_num].nColorMinThres[s]),Scalar(BOLT_Param[Cam_num].nColorMaxThres[s]),Out_binary);

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							J_Delete_Boundary(Out_binary,1);
						}

						Mat stats, centroids, label;  
						int numOfLables = connectedComponentsWithStats(Out_binary, label, stats, centroids, 8,CV_32S);
						int area = 0;int left = 0;int top = 0;int right = 0;int bottom = 0;
						for (int j = 1; j < numOfLables; j++) 
						{
							area = stats.at<int>(j, CC_STAT_AREA);
							if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] > (double)area || BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] < (double)area)
							{
								left = stats.at<int>(j, CC_STAT_LEFT);
								top = stats.at<int>(j, CC_STAT_TOP);
								right = left + stats.at<int>(j, CC_STAT_WIDTH);
								bottom = top + stats.at<int>(j, CC_STAT_HEIGHT);

								for (int y = top; y < bottom; ++y) {

									int *tlabel = label.ptr<int>(y);

									for (int x = left; x < right; ++x) {
										if (tlabel[x] == j)
										{
											Out_binary.at<uchar>(y,x) = 0;
										}
									}
								}
							}
							else
							{
								if (BOLT_Param[Cam_num].nColorOutput[s] == 1)
								{
									dist_vec.push_back((double)area);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("%d",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}
							}

						}

						if (BOLT_Param[Cam_num].nColorOutput[s] == 0)
						{
							if (BOLT_Param[Cam_num].nThres_V1[s] != 0.0f || BOLT_Param[Cam_num].nThres_V2[s] != 255.0f)
							{
								for (int i=0;i<Out_binary.rows;i++)
								{
									for (int j=0;j<Out_binary.cols;j++)
									{
										if (Out_binary.at<uchar>(i,j) > 0)
										{
											dist_vec.push_back((double)HSV_channel[1].at<uchar>(i,j));
										}
									}	
								}
							}
							else
							{
								for (int i=0;i<Out_binary.rows;i++)
								{
									for (int j=0;j<Out_binary.cols;j++)
									{
										//if (Out_binary.at<uchar>(i,j) > 0)
										{
											dist_vec.push_back((double)HSV_channel[1].at<uchar>(i,j));
										}
									}	
								}
							}
						}
					}
					//if (m_Text_View[Cam_num] && !ROI_Mode)
					//{
					//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					//	//#pragma omp parallel for
					//	for( int k = 0; k < contours.size(); k++ )
					//	{
					//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), 1, 8, hierarchy);
					//		fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,100,255),8);
					//	}
					//}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						
						if (contours.size() > 0)
						{
							//#pragma omp parallel for
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,100,255),8);
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::BRIGHTNESSDIFF_AREA_TB) // Difference of Brightness
				{
					if (BOLT_Param[Cam_num].nColorMethod[s] == 1)
					{ // 두원의 밝기 차이
						//AfxMessageBox("1");
						Point2f P_Center;
						P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x - BOLT_Param[Cam_num].nRect[s].x;
						P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y - BOLT_Param[Cam_num].nRect[s].y;
						//P_Center.x = BOLT_Param[Cam_num].nRect[s].width/2;
						//P_Center.y = BOLT_Param[Cam_num].nRect[s].height/2;
						double t_Circle1_mindist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] - (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
						double t_Circle1_maxdist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] + (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;

						Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
						double R1_V = 0;double R1_CNT = 0;

						if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
						{
							//ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
							ellipse(Mask,P_Center,Size(t_Circle1_maxdist,t_Circle1_maxdist),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
							ellipse(Mask,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),0,0,360,Scalar(0,0,0),CV_FILLED,8,0);
						}
						else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
						{
							ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}
						if (BOLT_Param[Cam_num].nThres_V1[s] != 0.0f || BOLT_Param[Cam_num].nThres_V2[s] != 255.0f)
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Out_binary.at<uchar>(i,j) > 0 && Mask.at<uchar>(i,j) > 0)
									{
										R1_CNT++;
										R1_V += (double)CP_Gray_Img.at<uchar>(i,j);
									}
								}	
							}
							if (R1_CNT > 0)
							{
								R1_V /= R1_CNT;
							}
						}
						else
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Mask.at<uchar>(i,j) > 0)
									{
										R1_CNT++;
										R1_V += (double)CP_Gray_Img.at<uchar>(i,j);
									}
								}	
							}
							if (R1_CNT > 0)
							{
								R1_V /= R1_CNT;
							}
						}

						//AfxMessageBox("2");
						Mat Mask2 = Mat::zeros(Out_binary.size(), CV_8UC1);
						double R2_V = 0;double R2_CNT = 0;
						double t_Circle2_mindist = (double)BOLT_Param[Cam_num].nCircle2Radius[s] - (double)(BOLT_Param[Cam_num].nCircle2Thickness[s])/2;
						double t_Circle2_maxdist = (double)BOLT_Param[Cam_num].nCircle2Radius[s] + (double)(BOLT_Param[Cam_num].nCircle2Thickness[s])/2;

						if (BOLT_Param[Cam_num].nCircle2Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle2Radius[s] >= 0)
						{
							//ellipse(Mask2,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,0,360,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle2Thickness[s],0,0);
							ellipse(Mask2,P_Center,Size(t_Circle1_maxdist,t_Circle1_maxdist),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
							ellipse(Mask2,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),0,0,360,Scalar(0,0,0),CV_FILLED,8,0);
						}
						else if (BOLT_Param[Cam_num].nCircle2Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle2Radius[s] > 0)
						{
							ellipse(Mask2,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}

						if (BOLT_Param[Cam_num].nThres_V1[s] != 0.0f || BOLT_Param[Cam_num].nThres_V2[s] != 255.0f)
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Out_binary.at<uchar>(i,j) > 0 && Mask2.at<uchar>(i,j) > 0)
									{
										R2_CNT++;
										R2_V += (double)CP_Gray_Img.at<uchar>(i,j);
									}
								}	
							}
							if (R2_CNT > 0)
							{
								R2_V /= R2_CNT;
							}
						}
						else
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Mask2.at<uchar>(i,j) > 0)
									{
										R2_CNT++;
										R2_V += (double)CP_Gray_Img.at<uchar>(i,j);
									}
								}	
							}
							if (R2_CNT > 0)
							{
								R2_V /= R2_CNT;
							}
						}
						dist_vec.push_back(abs(R2_V-R1_V));

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(255,255,0),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(255,255,0),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
							//msg.Format("Circle1(%1.2f)",R1_V);
							//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
							//msg.Format("Circle2(%1.2f)",R2_V);
							//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 20), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
							add(Mask,Mask2,Mask);
							bitwise_and(Mask,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
								}
							}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(255,255,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(255,255,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),2);
							msg.Format("Circle1(%1.2f)",R1_V);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
							msg.Format("Circle2(%1.2f)",R2_V);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 20), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
							add(Mask,Mask2,Mask);
							bitwise_and(Mask,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nColorMethod[s] == 0)
					{ // 원의 밝기
						//AfxMessageBox("1");
						Point2f P_Center;
						P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x - BOLT_Param[Cam_num].nRect[s].x;
						P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y - BOLT_Param[Cam_num].nRect[s].y;
						double t_Circle1_mindist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] - (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
						double t_Circle1_maxdist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] + (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;

						//P_Center.x = BOLT_Param[Cam_num].nRect[s].width/2;
						//P_Center.y = BOLT_Param[Cam_num].nRect[s].height/2;
						Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
						double R1_V = 0;double R1_CNT = 0;

						if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
						{
							//ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
							ellipse(Mask,P_Center,Size(t_Circle1_maxdist,t_Circle1_maxdist),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
							ellipse(Mask,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),0,0,360,Scalar(0,0,0),CV_FILLED,8,0);
						}
						else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
						{
							ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}

						if (BOLT_Param[Cam_num].nThres_V1[s] != 0.0f || BOLT_Param[Cam_num].nThres_V2[s] != 255.0f)
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Out_binary.at<uchar>(i,j) > 0 && Mask.at<uchar>(i,j) > 0)
									{
										R1_CNT++;
										R1_V += (double)CP_Gray_Img.at<uchar>(i,j);
									}
								}	
							}
							if (R1_CNT > 0)
							{
								R1_V /= R1_CNT;
							}
						}
						else
						{
							for (int i=0;i<Out_binary.rows;i++)
							{
								for (int j=0;j<Out_binary.cols;j++)
								{
									if (Mask.at<uchar>(i,j) > 0)
									{
										R1_CNT++;
										R1_V += (double)CP_Gray_Img.at<uchar>(i,j);
									}
								}	
							}
							if (R1_CNT > 0)
							{
								R1_V /= R1_CNT;
							}
						}
						dist_vec.push_back(abs(R1_V));

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(255,255,0),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(255,255,0),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
							//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
							//msg.Format("Circle1(%1.2f)",R1_V);
							//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
							//msg.Format("Circle2(%1.2f)",R2_V);
							//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 20), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
							//add(Mask,Mask2,Mask);
							bitwise_and(Mask,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
								}
							}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(255,255,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(255,255,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
							msg.Format("Circle1(%1.2f)",R1_V);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
							//msg.Format("Circle2(%1.2f)",R2_V);
							//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 20), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
							//add(Mask,Mask2,Mask);
							bitwise_and(Mask,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
								}
							}
						}
					}

				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::AREA_BLOB_TB) // BLOB Size
				{
					if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 0 || BOLT_Param[Cam_num].nDirecFilterUsage[s] == 2)
					{
						if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 2)
						{
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								Mat HSV_Img1;Mat HSV_channel1[3];
								cvtColor(Src_Img[Cam_num], HSV_Img1, CV_BGR2Lab);
								//blur(HSV_Img, HSV_Img, Size(5,5));
								split(HSV_Img1, HSV_channel1);
								cvtColor(HSV_channel1[1],Dst_Img[Cam_num],CV_GRAY2BGR);
							}

							Mat HSV_Img;Mat HSV_channel[3];
							cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2Lab);
							//cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2HSV);
							//blur(HSV_Img, HSV_Img, Size(5,5));
							split(HSV_Img, HSV_channel);
							CP_Gray_Img = HSV_channel[1].clone();
							if (BOLT_Param[Cam_num].nMethod_Direc[s] == 0 || BOLT_Param[Cam_num].nMethod_Direc[s] == 1)
							{
								medianBlur(CP_Gray_Img,CP_Gray_Img,3);
							}
							if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
							} 
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
							} 
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
							{
								inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
							{
								Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
								inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
								subtract(White_Out_binary, Out_binary, Out_binary);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
							{
								blur(CP_Gray_Img, Out_binary, Size(3,3));
								// Run the edge detector on grayscale
								Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
							}
						}

						if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
						{
							Mat Convex_Img = Out_binary.clone();
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() > 0)
							{
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									convexHull( Mat(contours[k]), hull1[k], false );
									//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
									drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
								}
								subtract(Convex_Img,Out_binary,Out_binary);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
								erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
							}
						}

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							J_Delete_Boundary(Out_binary,1);
						}

						//Mat stats, centroids, label;  
						//int numOfLables = connectedComponentsWithStats(Out_binary, label,   
						//	stats, centroids, 8,CV_32S);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						int t_cnt = 0;double area = 0;int t_w = 0;int t_h = 0;int t_x = 0;int t_y = 0;
						//if (numOfLables > 200)
						//{
						//	numOfLables = 200;
						//}
						//else
						//{
							//if (m_Text_View[Cam_num] && !ROI_Mode)
							//{
							//	if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
							//	{
							//		//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//		////#pragma omp parallel for
							//		//for( int k = 0; k < contours.size(); k++ )
							//		//{
							//		//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
							//		//	fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							//		//}
							//	}
							//}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//#pragma omp parallel for
								if (contours.size() > 0)
								{
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
										fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
									}
								}
							}
						//}
							RotatedRect t_boundRect;
							if (contours.size() > 0)
							{
							for (int j = 0; j < contours.size(); j++) 
							{
								//area = stats.at<int>(j, CC_STAT_AREA);
								//t_x = stats.at<int>(j, CC_STAT_LEFT);
								//t_y = stats.at<int>(j, CC_STAT_TOP);
								//t_w = stats.at<int>(j, CC_STAT_WIDTH);
								//t_h = stats.at<int>(j, CC_STAT_HEIGHT);

								area = (contourArea(contours[j],false)+0.5);
								t_boundRect = minAreaRect( Mat(contours[j]) );
								t_x = t_boundRect.boundingRect().x;
								t_y = t_boundRect.boundingRect().y;
								t_w = t_boundRect.boundingRect().width;
								t_h = t_boundRect.boundingRect().height;

								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										dist_vec.push_back(area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {
												
											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//int *tlabel = label.ptr<int>(yy);
												//Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//	if (tlabel[xx] == j)
												//	{
												//		pixel[xx][2] = 255;  
												//		pixel[xx][1] = 0;
												//		pixel[xx][0] = 0;
												//	}
												//}
											//}
											msg.Format("BLOB(%f)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%f",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_w*BOLT_Param[Cam_num].nResolution[0] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_w*BOLT_Param[Cam_num].nResolution[0])
									{
										dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											/*for (int yy = t_y; yy < t_y + t_h; ++yy) {

												int *tlabel = label.ptr<int>(yy);
												Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												for (int xx = t_x; xx < t_x + t_w; ++xx) {
													if (tlabel[xx] == j)
													{
														pixel[xx][2] = 255;  
														pixel[xx][1] = 0;
														pixel[xx][0] = 0;
													}
												}
											}*/
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);
											//											for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
											msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_h*BOLT_Param[Cam_num].nResolution[1] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_h*BOLT_Param[Cam_num].nResolution[1])
									{
										dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[1]);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}

											msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								} 
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);


											msg.Format("No.(%d)",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 4)
								{//기준점에서 거리
									double Cx = t_boundRect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
									double Cy = t_boundRect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

									double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
									double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
									//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
									//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

									//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										//Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

										//int x = t_boundRect.center.x; //중심좌표
										//int y = t_boundRect.center.y;
										//drawContours( t_Mask, contours, j, CV_RGB(255,255,255), CV_FILLED, 8, hierarchy);

										////for (int yy = t_y; yy < t_y + t_h; ++yy) {

										////	int *tlabel = label.ptr<int>(yy);
										////	uchar* pixel = t_Mask.ptr<uchar>(yy);
										////	for (int xx = t_x; xx < t_x + t_w; ++xx) {
										////		if (tlabel[xx] == j)
										////		{
										////			pixel[xx] = 255;  
										////		}
										////	}
										////}
										//J_Fill_Hole(t_Mask);
										////imwrite("00.bmp",t_Mask);
										//findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										RotatedRect t_Rect =  minAreaRect(contours[j]);
										Cx = t_Rect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										Cy = t_Rect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

										double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
										dist_vec.push_back(t_D);

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

											msg.Format("Dist(%1.3f)",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 5)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
										&& t_x > 1 && t_y > 1 && t_x + t_w < BOLT_Param[Cam_num].nRect[s].size().width -1 && t_y + t_h < BOLT_Param[Cam_num].nRect[s].size().height -1)
									{
										dist_vec.push_back(area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("BLOB(%f)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%f",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 6)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
										&& (t_x <= 0 || t_y <= 0 || t_x + t_w >= BOLT_Param[Cam_num].nRect[s].size().width -1 || t_y + t_h >= BOLT_Param[Cam_num].nRect[s].size().height -1))
									{
										dist_vec.push_back(area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("BLOB(%f)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%f",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 7)
								{//기준점에서 양쪽 두께 차이
									//double Cx = centroids.at<double>(j, 0) + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
									//double Cy = centroids.at<double>(j, 1) + (double)BOLT_Param[Cam_num].nRect[s].y;

									//double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
									//double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
									//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
									//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

									//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
										drawContours( t_Mask, contours, j, CV_RGB(255,255,255), CV_FILLED, 8, hierarchy);
										//for (int yy = t_y; yy < t_y + t_h; ++yy) {

										//	int *tlabel = label.ptr<int>(yy);
										//	uchar* pixel = t_Mask.ptr<uchar>(yy);
										//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
										//		if (tlabel[xx] == j)
										//		{
										//			pixel[xx] = 255;  
										//		}
										//	}
										//}

										dilate(t_Mask,t_Mask,element,Point(-1,-1), 3);
										erode(t_Mask,t_Mask,element,Point(-1,-1), 3);

										Mat CP_t_Mask = t_Mask.clone();
										//imwrite("00.bmp",t_Mask);
										J_Fill_Hole(t_Mask);
										//imwrite("01.bmp",t_Mask);
										findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										if (contours.size() > 0)
										{

											RotatedRect t_Rot_Rect =  minAreaRect(contours[0]);
											Rect t_Rect = boundingRect( Mat(contours[0]));
											Point2f Rot_Rect_points[4];
											t_Rot_Rect.points( Rot_Rect_points );

											double Cx = t_Rot_Rect.center.x;// + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
											double Cy = t_Rot_Rect.center.y;// + (double)BOLT_Param[Cam_num].nRect[s].y;

											Mat Edge_t_Mask;
											erode(t_Mask,Edge_t_Mask,element,Point(-1,-1), 1);
											subtract(t_Mask,Edge_t_Mask,Edge_t_Mask);

											Mat Circle_Edge;
											subtract(t_Mask, CP_t_Mask,Circle_Edge);
											erode(Circle_Edge,CP_t_Mask,element,Point(-1,-1), 1);
											subtract(Circle_Edge,CP_t_Mask,Circle_Edge);

											// 1. Find LT, LB, RT, RB point
											Point LT_Point(-1,-1);
											Point LB_Point(-1,-1);
											Point RT_Point(-1,-1);
											Point RB_Point(-1,-1);

											Point2f Top_Mid_Point(-1,-1);
											Point2f Bottom_Mid_Point(-1,-1);
											Point2f Left_Mid_Point(-1,-1);
											Point2f Right_Mid_Point(-1,-1);
											float Mid_CNT = 0;

											Point2f Top_Circle_Point(-1,-1);
											Point2f Bottom_Circle_Point(-1,-1);
											Point2f Left_Circle_Point(-1,-1);
											Point2f Right_Circle_Point(-1,-1);


											double t_Dist = 9999; 
											for (int i = 0; i < 4;i++)
											{
												double t_Dist_temp =  sqrt(((float)t_Rect.x - Rot_Rect_points[i].x)*((float)t_Rect.x - Rot_Rect_points[i].x) + ((float)t_Rect.y - Rot_Rect_points[i].y)*((float)t_Rect.y - Rot_Rect_points[i].y));
												if (t_Dist >= t_Dist_temp)
												{
													t_Dist = t_Dist_temp;
													LT_Point = Point(cvRound(Rot_Rect_points[i].x),cvRound(Rot_Rect_points[i].y));
												}
											}
											t_Dist = 9999;
											for (int i = 0; i < 4;i++)
											{
												double t_Dist_temp =  sqrt(((float)(t_Rect.x + t_Rect.width) - Rot_Rect_points[i].x)*((float)(t_Rect.x + t_Rect.width) - Rot_Rect_points[i].x) + ((float)t_Rect.y - Rot_Rect_points[i].y)*((float)t_Rect.y - Rot_Rect_points[i].y));
												if (t_Dist >= t_Dist_temp)
												{
													t_Dist = t_Dist_temp;
													RT_Point = Point(cvRound(Rot_Rect_points[i].x),cvRound(Rot_Rect_points[i].y));
												}
											}
											t_Dist = 9999;
											for (int i = 0; i < 4;i++)
											{
												double t_Dist_temp =  sqrt(((float)(t_Rect.x) - Rot_Rect_points[i].x)*((float)(t_Rect.x) - Rot_Rect_points[i].x) + ((float)(t_Rect.y + t_Rect.height) - Rot_Rect_points[i].y)*((float)(t_Rect.y + t_Rect.height) - Rot_Rect_points[i].y));
												if (t_Dist >= t_Dist_temp)
												{
													t_Dist = t_Dist_temp;
													LB_Point = Point(cvRound(Rot_Rect_points[i].x),cvRound(Rot_Rect_points[i].y));
												}
											}
											t_Dist = 9999;
											for (int i = 0; i < 4;i++)
											{
												double t_Dist_temp =  sqrt(((float)(t_Rect.x + t_Rect.width) - Rot_Rect_points[i].x)*((float)(t_Rect.x + t_Rect.width) - Rot_Rect_points[i].x) + ((float)(t_Rect.y + t_Rect.height) - Rot_Rect_points[i].y)*((float)(t_Rect.y + t_Rect.height) - Rot_Rect_points[i].y));
												if (t_Dist >= t_Dist_temp)
												{
													t_Dist = t_Dist_temp;
													RB_Point = Point(cvRound(Rot_Rect_points[i].x),cvRound(Rot_Rect_points[i].y));
												}
											}

											//Vec4f lines_Top;
											//vector<Point2f> vec_Top_Point;
											int t_length = (RT_Point.x - LT_Point.x)/3;
											for (int ii = LT_Point.x + t_length;ii < RT_Point.x - t_length; ii++)
											{
												for (int jj = min(LT_Point.y,RT_Point.y) - 10;jj < max(LT_Point.y,RT_Point.y) + 10; jj++)
												{
													if (jj >= 0 && jj < Edge_t_Mask.rows)
													{
														if (Edge_t_Mask.at<uchar>(jj,ii) > 0)
														{
															//vec_Top_Point.push_back(Point2f(float(ii),float(jj)));
															Top_Mid_Point.x += float(ii);
															Top_Mid_Point.y += float(jj);
															Mid_CNT++;
															if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
															if (m_Text_View[Cam_num] && !ROI_Mode)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
														}
													}
												}
											}
											Top_Mid_Point.x /= Mid_CNT;
											Top_Mid_Point.y /= Mid_CNT;
											Mid_CNT = 0;
											//fitLine(vec_Top_Point,lines_Top, CV_DIST_L2,0,0.01,0.01);
											//double d = sqrt((double)lines_Top[0] * lines_Top[0] + (double)lines_Top[1] * lines_Top[1]); 
											//float t = (float)(Edge_t_Mask.cols + Edge_t_Mask.rows); 
											//Point pt1, pt2;

											//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											//{
											//	pt1.x = cvRound(lines_Top[2] - (lines_Top[0]/d) * t); 
											//	pt1.y = cvRound(lines_Top[3] - (lines_Top[1]/d) * t); 
											//	pt2.x = cvRound(lines_Top[2] + (lines_Top[0]/d) * t); 
											//	pt2.y = cvRound(lines_Top[3] + (lines_Top[1]/d) * t); 
											//	cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),pt1,pt2,CV_RGB(255,255,0),2);
											//}


											//Vec4f lines_Bottom;
											//vector<Point2f> vec_Bottom_Point;
											t_length = (RB_Point.x - LB_Point.x)/3;
											for (int ii = LB_Point.x + t_length;ii < RB_Point.x - t_length; ii++)
											{
												for (int jj = min(LB_Point.y,RB_Point.y) - 10;jj < max(LB_Point.y,RB_Point.y) + 10; jj++)
												{
													if (jj >= 0 && jj < Edge_t_Mask.rows)
													{
														if (Edge_t_Mask.at<uchar>(jj,ii) > 0)
														{
															//vec_Bottom_Point.push_back(Point2f(float(ii),float(jj)));
															Bottom_Mid_Point.x += float(ii);
															Bottom_Mid_Point.y += float(jj);
															Mid_CNT++;
															if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
															if (m_Text_View[Cam_num] && !ROI_Mode)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
														}
													}
												}
											}
											Bottom_Mid_Point.x /= Mid_CNT;
											Bottom_Mid_Point.y /= Mid_CNT;
											Mid_CNT = 0;
											//fitLine(vec_Bottom_Point,lines_Bottom, CV_DIST_L2,0,0.01,0.01);

											//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											//{
											//	d = sqrt((double)lines_Bottom[0] * lines_Bottom[0] + (double)lines_Bottom[1] * lines_Bottom[1]); 
											//	pt1.x = cvRound(lines_Bottom[2] - (lines_Bottom[0]/d) * t); 
											//	pt1.y = cvRound(lines_Bottom[3] - (lines_Bottom[1]/d) * t); 
											//	pt2.x = cvRound(lines_Bottom[2] + (lines_Bottom[0]/d) * t); 
											//	pt2.y = cvRound(lines_Bottom[3] + (lines_Bottom[1]/d) * t); 
											//	cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),pt1,pt2,CV_RGB(255,255,0),2);
											//}

											//Vec4f lines_Left;
											//vector<Point2f> vec_Left_Point;
											t_length = (LB_Point.y - LT_Point.y)/3;
											for (int ii = min(LT_Point.x,LB_Point.x) - 10;ii < max(LT_Point.x,LB_Point.x) + 10; ii++)
											{
												for (int jj = LT_Point.y + t_length;jj < LB_Point.y - t_length; jj++)
												{
													if (ii >= 0 && ii < Edge_t_Mask.cols)
													{
														if (Edge_t_Mask.at<uchar>(jj,ii) > 0)
														{
															//vec_Left_Point.push_back(Point2f(float(ii),float(jj)));
															Left_Mid_Point.x += float(ii);
															Left_Mid_Point.y += float(jj);
															Mid_CNT++;
															if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
															if (m_Text_View[Cam_num] && !ROI_Mode)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
														}
													}
												}
											}
											Left_Mid_Point.x /= Mid_CNT;
											Left_Mid_Point.y /= Mid_CNT;
											Mid_CNT = 0;
											//fitLine(vec_Left_Point,lines_Left, CV_DIST_L2,0,0.01,0.01);

											//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											//{
											//	d = sqrt((double)lines_Left[0] * lines_Left[0] + (double)lines_Left[1] * lines_Left[1]); 
											//	pt1.x = cvRound(lines_Left[2] - (lines_Left[0]/d) * t); 
											//	pt1.y = cvRound(lines_Left[3] - (lines_Left[1]/d) * t); 
											//	pt2.x = cvRound(lines_Left[2] + (lines_Left[0]/d) * t); 
											//	pt2.y = cvRound(lines_Left[3] + (lines_Left[1]/d) * t); 
											//	cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),pt1,pt2,CV_RGB(255,255,0),2);
											//}


											//Vec4f lines_Right;
											//vector<Point2f> vec_Right_Point;
											t_length = (RB_Point.y - RT_Point.y)/3;
											for (int ii = min(RT_Point.x,RB_Point.x) - 10;ii < max(RT_Point.x,RB_Point.x) + 10; ii++)
											{
												for (int jj = RT_Point.y + t_length;jj < RB_Point.y - t_length; jj++)
												{
													if (ii >= 0 && ii < Edge_t_Mask.cols)
													{
														if (Edge_t_Mask.at<uchar>(jj,ii) > 0)
														{
															//vec_Right_Point.push_back(Point2f(float(ii),float(jj)));
															Right_Mid_Point.x += float(ii);
															Right_Mid_Point.y += float(jj);
															Mid_CNT++;
															if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
															if (m_Text_View[Cam_num] && !ROI_Mode)
															{
																circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
															}
														}
													}
												}
											}
											Right_Mid_Point.x /= Mid_CNT;
											Right_Mid_Point.y /= Mid_CNT;
											Mid_CNT = 0;
											//fitLine(vec_Right_Point,lines_Right, CV_DIST_L2,0,0.01,0.01);


											//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											//{
											//	d = sqrt((double)lines_Right[0] * lines_Right[0] + (double)lines_Right[1] * lines_Right[1]); 
											//	pt1.x = cvRound(lines_Right[2] - (lines_Right[0]/d) * t); 
											//	pt1.y = cvRound(lines_Right[3] - (lines_Right[1]/d) * t); 
											//	pt2.x = cvRound(lines_Right[2] + (lines_Right[0]/d) * t); 
											//	pt2.y = cvRound(lines_Right[3] + (lines_Right[1]/d) * t); 
											//	cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),pt1,pt2,CV_RGB(255,255,0),2);
											//}


											Mat Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
											line(Line_Img,Top_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,255),3,8);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Top_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Top_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											bitwise_and(Line_Img,Circle_Edge,Line_Img);
											for (int ii = 0;ii < Line_Img.cols; ii++)
											{
												for (int jj = 0;jj < Line_Img.rows; jj++)
												{
													if (Line_Img.at<uchar>(jj,ii) > 0)
													{
														Top_Circle_Point.x += float(ii);
														Top_Circle_Point.y += float(jj);
														Mid_CNT++;
														if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
														if (m_Text_View[Cam_num] && !ROI_Mode)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
													}
												}
											}
											Top_Circle_Point.x /= Mid_CNT;
											Top_Circle_Point.y /= Mid_CNT;
											Mid_CNT = 0;


											//double v_Top_dist = abs(lines_Top[1]*Top_Circle_Point.x - lines_Top[0]*Top_Circle_Point.y + lines_Top[0]*lines_Top[3] - lines_Top[1]*lines_Top[2])/sqrt(lines_Top[0]*lines_Top[0] + lines_Top[1]*lines_Top[1])*BOLT_Param[Cam_num].nResolution[0];
											double v_Top_dist2 = sqrt((Top_Mid_Point.x - Top_Circle_Point.x)*(Top_Mid_Point.x - Top_Circle_Point.x)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (Top_Mid_Point.y - Top_Circle_Point.y)*(Top_Mid_Point.y - Top_Circle_Point.y)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												msg.Format("Top_Dist(%1.3f)",v_Top_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + (Top_Mid_Point.x + Top_Circle_Point.x)/2 - 100,BOLT_Param[Cam_num].nRect[s].y + (Top_Mid_Point.y + Top_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												msg.Format("%1.3f",v_Top_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + (Top_Mid_Point.x + Top_Circle_Point.x)/2 - 40,BOLT_Param[Cam_num].nRect[s].y + (Top_Mid_Point.y + Top_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}
											//imwrite("00.bmp",Line_Img);

											Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
											line(Line_Img,Bottom_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,255),3,8);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Bottom_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Bottom_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											bitwise_and(Line_Img,Circle_Edge,Line_Img);
											for (int ii = 0;ii < Line_Img.cols; ii++)
											{
												for (int jj = 0;jj < Line_Img.rows; jj++)
												{
													if (Line_Img.at<uchar>(jj,ii) > 0)
													{
														Bottom_Circle_Point.x += float(ii);
														Bottom_Circle_Point.y += float(jj);
														Mid_CNT++;
														if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
														if (m_Text_View[Cam_num] && !ROI_Mode)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
													}
												}
											}
											Bottom_Circle_Point.x /= Mid_CNT;
											Bottom_Circle_Point.y /= Mid_CNT;
											Mid_CNT = 0;


											//double v_Bottom_dist = abs(lines_Bottom[1]*Bottom_Circle_Point.x - lines_Bottom[0]*Bottom_Circle_Point.y + lines_Bottom[0]*lines_Bottom[3] - lines_Bottom[1]*lines_Bottom[2])/sqrt(lines_Bottom[0]*lines_Bottom[0] + lines_Bottom[1]*lines_Bottom[1])*BOLT_Param[Cam_num].nResolution[0];
											double v_Bottom_dist2 = sqrt((Bottom_Mid_Point.x - Bottom_Circle_Point.x)*(Bottom_Mid_Point.x - Bottom_Circle_Point.x)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (Bottom_Mid_Point.y - Bottom_Circle_Point.y)*(Bottom_Mid_Point.y - Bottom_Circle_Point.y)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												//t_cnt++;
												msg.Format("Bottom_Dist(%1.3f)",v_Bottom_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + (Bottom_Mid_Point.x + Bottom_Circle_Point.x)/2 - 100,BOLT_Param[Cam_num].nRect[s].y + (Bottom_Mid_Point.y + Bottom_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												msg.Format("%1.3f",v_Bottom_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + (Bottom_Mid_Point.x + Bottom_Circle_Point.x)/2 - 40,BOLT_Param[Cam_num].nRect[s].y + (Bottom_Mid_Point.y + Bottom_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}

											Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
											line(Line_Img,Left_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,255),3,8);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Left_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Left_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											bitwise_and(Line_Img,Circle_Edge,Line_Img);
											for (int ii = 0;ii < Line_Img.cols; ii++)
											{
												for (int jj = 0;jj < Line_Img.rows; jj++)
												{
													if (Line_Img.at<uchar>(jj,ii) > 0)
													{
														Left_Circle_Point.x += float(ii);
														Left_Circle_Point.y += float(jj);
														Mid_CNT++;
														if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
														if (m_Text_View[Cam_num] && !ROI_Mode)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
													}
												}
											}
											Left_Circle_Point.x /= Mid_CNT;
											Left_Circle_Point.y /= Mid_CNT;
											Mid_CNT = 0;


											//double v_Left_dist = abs(lines_Left[1]*Left_Circle_Point.x - lines_Left[0]*Left_Circle_Point.y + lines_Left[0]*lines_Left[3] - lines_Left[1]*lines_Left[2])/sqrt(lines_Left[0]*lines_Left[0] + lines_Left[1]*lines_Left[1])*BOLT_Param[Cam_num].nResolution[0];
											double v_Left_dist2 = sqrt((Left_Mid_Point.x - Left_Circle_Point.x)*(Left_Mid_Point.x - Left_Circle_Point.x)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (Left_Mid_Point.y - Left_Circle_Point.y)*(Left_Mid_Point.y - Left_Circle_Point.y)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												//t_cnt++;
												msg.Format("Left_Dist(%1.3f)",v_Left_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + Left_Mid_Point.x + 50,-15+BOLT_Param[Cam_num].nRect[s].y + (Left_Mid_Point.y + Left_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												msg.Format("%1.3f",v_Left_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + Left_Mid_Point.x + 100,-15+BOLT_Param[Cam_num].nRect[s].y + (Left_Mid_Point.y + Left_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}

											Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
											line(Line_Img,Right_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,255),3,8);
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Right_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												cv::line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Right_Mid_Point,Point2f(Cx,Cy),CV_RGB(255,255,0),3,8);
											}
											bitwise_and(Line_Img,Circle_Edge,Line_Img);
											for (int ii = 0;ii < Line_Img.cols; ii++)
											{
												for (int jj = 0;jj < Line_Img.rows; jj++)
												{
													if (Line_Img.at<uchar>(jj,ii) > 0)
													{
														Right_Circle_Point.x += float(ii);
														Right_Circle_Point.y += float(jj);
														Mid_CNT++;
														if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
														if (m_Text_View[Cam_num] && !ROI_Mode)
														{
															circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(float(ii),float(jj)),2,CV_RGB(255,100,0),1);
														}
													}
												}
											}
											Right_Circle_Point.x /= Mid_CNT;
											Right_Circle_Point.y /= Mid_CNT;
											Mid_CNT = 0;


											//double v_Right_dist = abs(lines_Right[1]*Right_Circle_Point.x - lines_Right[0]*Right_Circle_Point.y + lines_Right[0]*lines_Right[3] - lines_Right[1]*lines_Right[2])/sqrt(lines_Right[0]*lines_Right[0] + lines_Right[1]*lines_Right[1])*BOLT_Param[Cam_num].nResolution[0];
											double v_Right_dist2 = sqrt((Right_Mid_Point.x - Right_Circle_Point.x)*(Right_Mid_Point.x - Right_Circle_Point.x)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (Right_Mid_Point.y - Right_Circle_Point.y)*(Right_Mid_Point.y - Right_Circle_Point.y)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												//t_cnt++;
												msg.Format("Right_Dist(%1.3f)",v_Right_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + Right_Circle_Point.x + 50,-15+BOLT_Param[Cam_num].nRect[s].y + (Right_Mid_Point.y + Right_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												msg.Format("%1.3f",v_Right_dist2);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + Right_Circle_Point.x + 100,-15+BOLT_Param[Cam_num].nRect[s].y + (Right_Mid_Point.y + Right_Circle_Point.y)/2), FONT_HERSHEY_SIMPLEX, fontScale+0.5, CV_RGB(255,0,0), 2, 8);
											}
											dist_vec.push_back(abs(v_Right_dist2 - v_Left_dist2));
											dist_vec.push_back(abs(v_Top_dist2 - v_Bottom_dist2));
										}
										//if (m_Text_View[Cam_num] && !ROI_Mode)
										//{
										//	//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
										//	{
										//		for (int yy = t_y; yy < t_y + t_h; ++yy) {

										//			int *tlabel = label.ptr<int>(yy);
										//			Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
										//			for (int xx = t_x; xx < t_x + t_w; ++xx) {
										//				if (tlabel[xx] == j)
										//				{
										//					pixel[xx][2] = 255;  
										//					pixel[xx][1] = 0;
										//					pixel[xx][0] = 0;
										//				}
										//			}
										//		}
										//	}
										//}
										//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										//{
										//	int x = centroids.at<double>(j, 0); //중심좌표
										//	int y = centroids.at<double>(j, 1);

										//	for (int yy = t_y; yy < t_y + t_h; ++yy) {

										//		int *tlabel = label.ptr<int>(yy);
										//		Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
										//		for (int xx = t_x; xx < t_x + t_w; ++xx) {
										//			if (tlabel[xx] == j)
										//			{
										//				pixel[xx][2] = 255;  
										//				pixel[xx][1] = 0;
										//				pixel[xx][0] = 0;
										//			}
										//		}
										//	}

										//	line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
										//	circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
										//	circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
										//	circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
										//	circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

										//	msg.Format("Dist(%1.3f)",t_D);
										//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										//	msg.Format("%1.3f",t_D);
										//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										//}
										//if (m_Text_View[Cam_num] && !ROI_Mode)
										//{
										//	line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
										//	circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
										//	circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
										//	circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
										//	circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
										//}
										t_cnt++;
									}
								}
							}

							if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
							{
								dist_vec.push_back((double)t_cnt);
							}
						}
					}
					else if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 1 || BOLT_Param[Cam_num].nDirecFilterUsage[s] == 3)
					{
						if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 3)
						{
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								Mat HSV_Img1;Mat HSV_channel1[3];
								cvtColor(Src_Img[Cam_num], HSV_Img1, CV_BGR2Lab);
								//blur(HSV_Img, HSV_Img, Size(5,5));
								split(HSV_Img1, HSV_channel1);
								cvtColor(HSV_channel1[1],Dst_Img[Cam_num],CV_GRAY2BGR);
							}

							Mat HSV_Img;Mat HSV_channel[3];
							cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2Lab);
							//cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2HSV);
							//blur(HSV_Img, HSV_Img, Size(5,5));
							split(HSV_Img, HSV_channel);
							CP_Gray_Img = HSV_channel[1].clone();
						}

						// 어두운불량 구해오기
						Mat t_Blur, Defect_Dark_Img1, Defect_Bright_Img1;
						if (BOLT_Param[Cam_num].nDirecFilterCNT[s] <= 0)
						{
							BOLT_Param[Cam_num].nDirecFilterCNT[s] = 1;
						} 
						else if (BOLT_Param[Cam_num].nDirecFilterCNT[s] >= 100)
						{
							BOLT_Param[Cam_num].nDirecFilterCNT[s] = 100;
						} 
						if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
						{
							//medianBlur(CP_Gray_Img,t_Blur,BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(CP_Gray_Img,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
						{
							dilate(CP_Gray_Img,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
						{
							dilate(CP_Gray_Img,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}

						subtract(t_Blur, CP_Gray_Img, Defect_Dark_Img1);
						threshold(Defect_Dark_Img1,Out_binary,BOLT_Param[Cam_num].nDirecFilterDarkThres[s],255,CV_THRESH_BINARY);

						if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
						{
							Mat Convex_Img = Out_binary.clone();
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() > 0)
							{
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									convexHull( Mat(contours[k]), hull1[k], false );
									//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
									drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
								}
								subtract(Convex_Img,Out_binary,Out_binary);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
								erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
							}
						}

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
							{
								erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
						}

						if (BOLT_Param[Cam_num].nDirecFilterDarkThres[s] > 0)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//Mat stats, centroids, label;  
							//int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							//	stats, centroids, 8,CV_32S);
							//if (numOfLables > 200)
							//{
							//	numOfLables = 200;
							//}
							//else
							//{
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									////#pragma omp parallel for
									//for( int k = 0; k < contours.size(); k++ )
									//{
									//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
									//	fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
									//}
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									//#pragma omp parallel for
									if (contours.size() > 0)
									{									
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
											fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
										}
									}
								}
							//}
							int t_cnt = 0;double area = 0;int t_w = 0;int t_h = 0;int t_x = 0;int t_y = 0;
														RotatedRect t_boundRect;
							if (contours.size() > 0)
							{
							//if (numOfLables > 0)
							//{
								for (int j = 0; j < contours.size(); j++) 
								{
									area = (contourArea(contours[j],false)+0.5);
									t_boundRect = minAreaRect( Mat(contours[j]) );
									t_x = t_boundRect.boundingRect().x;
									t_y = t_boundRect.boundingRect().y;
									t_w = t_boundRect.boundingRect().width;
									t_h = t_boundRect.boundingRect().height;
									//area = stats.at<int>(j, CC_STAT_AREA);
									//t_x = stats.at<int>(j, CC_STAT_LEFT);
									//t_y = stats.at<int>(j, CC_STAT_TOP);
									//t_w = stats.at<int>(j, CC_STAT_WIDTH);
									//t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%f)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%f",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_w*BOLT_Param[Cam_num].nResolution[0] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_w*BOLT_Param[Cam_num].nResolution[0])
										{
											dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_h*BOLT_Param[Cam_num].nResolution[1] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_h*BOLT_Param[Cam_num].nResolution[1])
										{
											dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										t_cnt++;
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("No.(%d)",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 4)
									{//기준점에서 거리
										double Cx = t_boundRect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										double Cy = t_boundRect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

										double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
										double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
										//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
										//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											//Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
											//drawContours( t_Mask, contours, j, CV_RGB(255,255,255), CV_FILLED, 8, hierarchy);
											////for (int yy = t_y; yy < t_y + t_h; ++yy) {

											////	int *tlabel = label.ptr<int>(yy);
											////	uchar* pixel = t_Mask.ptr<uchar>(yy);
											////	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											////		if (tlabel[xx] == j)
											////		{
											////			pixel[xx] = 255;  
											////		}
											////	}
											////}
											//findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
											RotatedRect t_Rect =  minAreaRect(contours[j]);
											Cx = t_Rect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
											Cy = t_Rect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

											double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											dist_vec.push_back(t_D);

											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
												{
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
													//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
													//for (int yy = t_y; yy < t_y + t_h; ++yy) {

													//	int *tlabel = label.ptr<int>(yy);
													//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
													//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
													//		if (tlabel[xx] == j)
													//		{
													//			pixel[xx][2] = 255;  
													//			pixel[xx][1] = 0;
													//			pixel[xx][0] = 0;
													//		}
													//	}
													//}
												}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 5)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& t_x > 1 && t_y > 1 && t_x + t_w < BOLT_Param[Cam_num].nRect[s].size().width -1 && t_y + t_h < BOLT_Param[Cam_num].nRect[s].size().height -1)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%f)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%f",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 6)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& (t_x <= 0 || t_y <= 0 || t_x + t_w >= BOLT_Param[Cam_num].nRect[s].size().width -1 || t_y + t_h >= BOLT_Param[Cam_num].nRect[s].size().height -1))
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//	rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%f)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%f",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
								}

								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									dist_vec.push_back((double)t_cnt);
								}
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
								//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
						{
							//medianBlur(CP_Gray_Img,t_Blur,BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(CP_Gray_Img,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
						{
							erode(CP_Gray_Img,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
						{
							erode(CP_Gray_Img,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}

						subtract(CP_Gray_Img, t_Blur, Defect_Bright_Img1);
						threshold(Defect_Bright_Img1,Out_binary,BOLT_Param[Cam_num].nDirecFilterBrightThres[s],255,CV_THRESH_BINARY);

						if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
						{
							Mat Convex_Img = Out_binary.clone();
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() > 0)
							{
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									convexHull( Mat(contours[k]), hull1[k], false );
									//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
									drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
								}
								subtract(Convex_Img,Out_binary,Out_binary);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
								erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
							}
						}

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
							{
								erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterBrightThres[s] > 0)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//Mat stats, centroids, label;  
							//int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							//	stats, centroids, 8,CV_32S);
							//if (numOfLables > 200)
							//{
							//	numOfLables = 200;
							//}
							//else
							//{
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									////#pragma omp parallel for
									//for( int k = 0; k < contours.size(); k++ )
									//{
									//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
									//	fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
									//}
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									//#pragma omp parallel for
									if (contours.size() > 0)
									{
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
											fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
										}
									}
								}
							//}
							int t_cnt = 0;double area = 0;int t_w = 0;int t_h = 0;int t_x = 0;int t_y = 0;
							RotatedRect t_boundRect;
							if (contours.size() > 0)
							{
							/*if (numOfLables > 0)
							{*/
								for (int j = 0; j < contours.size(); j++) 
								{
									//area = stats.at<int>(j, CC_STAT_AREA);
									//t_x = stats.at<int>(j, CC_STAT_LEFT);
									//t_y = stats.at<int>(j, CC_STAT_TOP);
									//t_w = stats.at<int>(j, CC_STAT_WIDTH);
									//t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									area = (contourArea(contours[j],false)+0.5);
									t_boundRect = minAreaRect( Mat(contours[j]) );
									t_x = t_boundRect.boundingRect().x;
									t_y = t_boundRect.boundingRect().y;
									t_w = t_boundRect.boundingRect().width;
									t_h = t_boundRect.boundingRect().height;
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%f)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%f",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_w*BOLT_Param[Cam_num].nResolution[0] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_w*BOLT_Param[Cam_num].nResolution[0])
										{
											dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_h*BOLT_Param[Cam_num].nResolution[1] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_h*BOLT_Param[Cam_num].nResolution[1])
										{
											dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("No.(%d)",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 4)
									{//기준점에서 거리
										double Cx = t_boundRect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										double Cy = t_boundRect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

										double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
										double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
										//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
										//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											//Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	uchar* pixel = t_Mask.ptr<uchar>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx] = 255;  
											//		}
											//	}
											//}
											//findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
											RotatedRect t_Rect =  minAreaRect(contours[j]);
											Cx = t_Rect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
											Cy = t_Rect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

											double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											dist_vec.push_back(t_D);

											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
												{
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
													//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
													//for (int yy = t_y; yy < t_y + t_h; ++yy) {

													//	int *tlabel = label.ptr<int>(yy);
													//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
													//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
													//		if (tlabel[xx] == j)
													//		{
													//			pixel[xx][2] = 255;  
													//			pixel[xx][1] = 0;
													//			pixel[xx][0] = 0;
													//		}
													//	}
													//}
												}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 5)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& t_x > 1 && t_y > 1 && t_x + t_w < BOLT_Param[Cam_num].nRect[s].size().width -1 && t_y + t_h < BOLT_Param[Cam_num].nRect[s].size().height -1)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%f)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%f",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 6)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& (t_x <= 0 || t_y <= 0 || t_x + t_w >= BOLT_Param[Cam_num].nRect[s].size().width -1 || t_y + t_h >= BOLT_Param[Cam_num].nRect[s].size().height -1))
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%f)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%f",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
								}

								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									dist_vec.push_back((double)t_cnt);
								}
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
								//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::COUNT_BLOB_TB) // BLOB Count
				{
					if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 0)
					{
						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							J_Delete_Boundary(Out_binary,1);
						}

						Mat stats, centroids, label;  
						int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							stats, centroids, 8,CV_32S);
						int t_cnt = 0;
						if (numOfLables > 0)
						{
							for (int j = 1; j < numOfLables; j++) 
							{
								int area = stats.at<int>(j, CC_STAT_AREA);
								if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										//int x = centroids.at<double>(j, 0); //중심좌표
										//int y = centroids.at<double>(j, 1);

										//msg.Format("BLOB(%d)",area);
										//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
										//msg.Format("%d",area);
										//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("No(%d)",t_cnt+1);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										msg.Format("%d(%d)",t_cnt+1,area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
									t_cnt++;
								}
							}
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//#pragma omp parallel for
								if (contours.size() > 0)
								{
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, 8, hierarchy);
										fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,200,0),8);
									}
								}
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//#pragma omp parallel for
								if (contours.size() > 0)
								{
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, 8, hierarchy);
										fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,200,0),8);
									}
								}
							}
						}
						dist_vec.push_back((double)t_cnt);
					}
					else
					{
						// 어두운불량 구해오기
						Mat t_Blur, Defect_Dark_Img1, Defect_Bright_Img1;
						if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
						{
							//medianBlur(CP_Gray_Img,t_Blur,BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(CP_Gray_Img,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
						{
							dilate(CP_Gray_Img,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
						{
							dilate(CP_Gray_Img,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}

						subtract(t_Blur, CP_Gray_Img, Defect_Dark_Img1);
						threshold(Defect_Dark_Img1,Out_binary,BOLT_Param[Cam_num].nDirecFilterDarkThres[s],255,CV_THRESH_BINARY);
						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
							{
								erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterDarkThres[s] > 0)
						{
							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);
							int t_cnt = 0;
							if (numOfLables > 0)
							{
								for (int j = 1; j < numOfLables; j++) 
								{
									int area = stats.at<int>(j, CC_STAT_AREA);
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//msg.Format("BLOB(%d)",area);
											//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											//msg.Format("%d",area);
											//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("No(%d)",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d(%d)",t_cnt+1,area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									//#pragma omp parallel for
									if (contours.size() > 0)
									{
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, 8, hierarchy);
											fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,200,255),8);
										}
									}
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									//#pragma omp parallel for
									if (contours.size() > 0)
									{
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, 8, hierarchy);
											fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,200,255),8);
										}
									}
								}
							}
							if (t_cnt > 0)
							{
								dist_vec.push_back((double)t_cnt);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
						{
							//medianBlur(CP_Gray_Img,t_Blur,BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(CP_Gray_Img,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
						{
							erode(CP_Gray_Img,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
						{
							erode(CP_Gray_Img,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}

						subtract(CP_Gray_Img, t_Blur, Defect_Bright_Img1);
						threshold(Defect_Bright_Img1,Out_binary,BOLT_Param[Cam_num].nDirecFilterBrightThres[s],255,CV_THRESH_BINARY);
						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
							{
								erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterBrightThres[s] > 0)
						{
							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);
							int t_cnt = 0;
							if (numOfLables > 0)
							{
								for (int j = 1; j < numOfLables; j++) 
								{
									int area = stats.at<int>(j, CC_STAT_AREA);
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//msg.Format("BLOB(%d)",area);
											//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											//msg.Format("%d",area);
											//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("No(%d)",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d(%d)",t_cnt+1,area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									//#pragma omp parallel for
									if (contours.size() > 0)
									{
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, 8, hierarchy);
											fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,200,255),8);
										}
									}
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									//#pragma omp parallel for

									if (contours.size() > 0)
									{									
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, 8, hierarchy);
											fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,200,255),8);
										}
									}
								}
							}
							if (t_cnt > 0)
							{
								dist_vec.push_back((double)t_cnt);
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::CIRCLE_BLOB_SIZE_TB) // Edge Crack
				{
					if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] < 0)
					{
						BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] = 0;
					}
					else if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] > 2)
					{
						BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] = 2;
					}
					Point2f P_Center;
					P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x - BOLT_Param[Cam_num].nRect[s].x;
					P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y - BOLT_Param[Cam_num].nRect[s].y;

					if (BOLT_Param[Cam_num].nCirclePositionMethod[s] > 0 && BOLT_Param[Cam_num].nCirclePositionMethod[s] < 3)
					{
						Mat P_Out_binary;
						if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
						{// P값 이하로 찾음
							threshold(CP_Gray_Img,P_Out_binary,BOLT_Param[Cam_num].nCirclePositionThreshold[s],255,CV_THRESH_BINARY_INV);
						}
						else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
						{// P값 이상으로 찾음
							threshold(CP_Gray_Img,P_Out_binary,BOLT_Param[Cam_num].nCirclePositionThreshold[s],255,CV_THRESH_BINARY);
						}
						erode(P_Out_binary,P_Out_binary,element,Point(-1,-1), 1);
						dilate(P_Out_binary,P_Out_binary,element,Point(-1,-1), 1);
						findContours( P_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if(contours.size() > 0) // 칸투어 갯수로 예외처리
						{
							vector<Rect> boundRect( contours.size() );
							int m_max_object_num = -1;int m_max_object_value = 0;
							for( int k = 0; k < contours.size(); k++ )
							{  
								boundRect[k] = boundingRect( Mat(contours[k]) );
								if (m_max_object_value <= boundRect[k].width*boundRect[k].height)
									//	&& pointPolygonTest( contours[k], Point(tROI.width/2,tROI.height/2), false ) == 1)
								{
									m_max_object_value = boundRect[k].width*boundRect[k].height;
									m_max_object_num = k;
								}
							}
							P_Center.x = (float)boundRect[m_max_object_num].x + (float)boundRect[m_max_object_num].width/2.0f;
							P_Center.y = (float)boundRect[m_max_object_num].y + (float)boundRect[m_max_object_num].height/2.0f;
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								//#pragma omp parallel for
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 0, 1, hierarchy);
								}
							}
						}
					}
					
					double t_Circle1_mindist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] - (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
					double t_Circle1_maxdist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] + (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
					double t_Circle2_mindist = (double)BOLT_Param[Cam_num].nCircle2Radius[s] - (double)(BOLT_Param[Cam_num].nCircle2Thickness[s])/2;
					double t_Circle2_maxdist = (double)BOLT_Param[Cam_num].nCircle2Radius[s] + (double)(BOLT_Param[Cam_num].nCircle2Thickness[s])/2;
					int t_Circle1_connect_cnt = 0;
					int t_Circle2_connect_cnt = 0;

					Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
					Mat Mask2 = Mat::zeros(Out_binary.size(), CV_8UC1);

					//#pragma omp parallel num_threads(2)
					{
						//#pragma omp sections
						{
							//#pragma omp section
							{
								if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
								{
									ellipse(Mask,P_Center,Size(t_Circle1_maxdist,t_Circle1_maxdist),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
									ellipse(Mask,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),0,0,360,Scalar(0,0,0),CV_FILLED,8,0);
								}
								else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
								{
									ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
								}
							}

							//#pragma omp section
							{
								if (BOLT_Param[Cam_num].nCircle2Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle2Radius[s] >= 0)
								{
									ellipse(Mask2,P_Center,Size(t_Circle2_maxdist,t_Circle2_maxdist),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
									ellipse(Mask2,P_Center,Size(t_Circle2_mindist,t_Circle2_mindist),0,0,360,Scalar(0,0,0),CV_FILLED,8,0);
								}
								else if (BOLT_Param[Cam_num].nCircle2Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle2Radius[s] > 0)
								{
									ellipse(Mask2,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
								}
							}
						}
					}
	
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(0,0,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(0,0,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						findContours( Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,100,0), 3, 8, hierarchy);
								//fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							}
						}
					}

					if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 2)
					{//"옵션(0:미사용,1:볼록 BLOB 차이,2:영역1(v1이하) 영역2(v2이상) 임계화)";
						Mat P_Out_binary1, P_Out_binary2;
						threshold(CP_Gray_Img,P_Out_binary1,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						threshold(CP_Gray_Img,P_Out_binary2,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
						bitwise_and(Mask,P_Out_binary1,P_Out_binary1);
						bitwise_and(Mask2,P_Out_binary2,P_Out_binary2);
						add(P_Out_binary1,P_Out_binary2,Out_binary);
						add(Mask,Mask2,Mask);
					}
					else
					{
						add(Mask,Mask2,Mask);
						if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::DIFFAVG) // 평균기준 차이
						{
							Scalar tempVal = mean( CP_Gray_Img, Mask );
							float myMAtMean = tempVal.val[0];
							Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
							inRange(CP_Gray_Img, Scalar(myMAtMean - BOLT_Param[Cam_num].nThres_V1[s]),Scalar(myMAtMean + BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
							//imwrite("00.bmp",Out_binary);
							subtract(White_Out_binary, Out_binary, Out_binary);
							//imwrite("01.bmp",Out_binary);
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								msg.Format("Avg = %1.3f", myMAtMean);
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								msg.Format("Avg = %1.3f", myMAtMean);
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
							}
						}
						bitwise_and(Mask,Out_binary,Out_binary);
					}
					//imwrite("00.bmp",Out_binary);

					if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
					{//"옵션(0:미사용,1:볼록 BLOB 차이,2:영역1(v1이하) 영역2(v2이상) 임계화)";
						Mat Convex_Img = Out_binary.clone();
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if (contours.size() > 0)
						{
							vector<vector<Point>> hull1(contours.size());
							//#pragma omp parallel for
							for(int k=0; k<contours.size(); k++)
							{	
								convexHull( Mat(contours[k]), hull1[k], false );
								//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
								drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
							}
							bitwise_and(Mask,Convex_Img,Convex_Img);
							subtract(Convex_Img,Out_binary,Out_binary);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
							erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
						}
					}

					//imwrite("01.bmp",Out_binary);
					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						J_Delete_Boundary(Out_binary,1);
					}
					
					dist_vec.clear();
					findContours(Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_NONE);

					// 크기 미리 필터링 해야함.
					//if (s==2)
					//{
					//	imwrite("00.bmp",Out_binary);
					//}
					if (contours.size() > 0)
					{
						for (int i = 0; i < contours.size(); i++) // iterate through each contour.
						{
							//if (hierarchy[i][3] == -1)
							{
								double fArea = (int)(contourArea(contours[i]) + 0.5);//t_Circle_Blob_Info.Pixels.size();
								if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] > fArea || BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] < fArea)
								{
									drawContours( Out_binary, contours,i, CV_RGB(0,0,0), CV_FILLED, 8, hierarchy);
								}
							}
						}
					}
					//if (s==2)
					//{
					//	imwrite("01.bmp",Out_binary);
					//}

					findContours(Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_NONE);

					if (contours.size() > 0)
					{
						//Mat labels(Out_binary.size(), CV_16U);
						Mat labels = Mat::zeros(Out_binary.size(), CV_8U);
						vector<Circle_Blob_Info> vec_Circle_Blob_Info;
						int t_cnt = 1;//double t_dist = 0;
						// 각도 계산
						vector<double> vX;
						vector<double> vY;

						for (int i = 0; i < contours.size(); i++) // iterate through each contour.
						{
							if (t_cnt > 255)
							{
								break;
							}
							//if contour[i] is not a hole
							if (hierarchy[i][3] == -1)
							{
								Circle_Blob_Info t_Circle_Blob_Info;
								drawContours(labels, contours, i, Scalar(t_cnt),2, 8, hierarchy);
								Rect t_rect = boundingRect(contours[i]);          
								int left = t_rect.x;
								int top = t_rect.y;
								int width = t_rect.width;
								int height = t_rect.height;           
								int x_end = left + width;
								int y_end = top + height;
								bool t_circle1_min_check = false;
								bool t_circle1_max_check = false;
								bool t_circle2_min_check = false;
								bool t_circle2_max_check = false;
								t_Circle1_connect_cnt = t_Circle2_connect_cnt = 0;
								
								////#pragma omp parallel for
								for (int x = left; x < x_end; x++)
								{
									//if (x == left + 2)
									//{
									//	x += x_end - x - 4;
									//}
									////#pragma omp parallel for
									for (int y = top; y < y_end; y++)
									{
										//if (y > top + 2 && y < y_end - 3)
										//{
										//	continue;
										//}
										//AfxMessageBox("0");
										Point p(x, y);
										//msg.Format("%d,%d,%d,%d",x,y,t_cnt,labels.at<uchar>(p));
										//AfxMessageBox(msg);
										if (t_cnt == labels.at<uchar>(p))
										{
											//t_Circle_Blob_Info.Pixels.push_back(p);
											//AfxMessageBox("1");
											double t_dist = sqrt(((float)x-P_Center.x)*((float)x-P_Center.x)+((float)y-P_Center.y)*((float)y-P_Center.y));
											if (t_dist >= t_Circle1_mindist-2.0f && t_dist <= t_Circle1_maxdist+2.0f)
											{// 첫번째 원에 들어 있으면
												if (t_dist-2.0 <= t_Circle1_mindist && !t_circle1_min_check)
												{
													//AfxMessageBox("C1_In");
													t_Circle1_connect_cnt++;
													t_circle1_min_check = true;
												}
												if (t_dist+2.0 >= t_Circle1_maxdist && !t_circle1_max_check)
												{
													//AfxMessageBox("C1_Out");
													t_Circle1_connect_cnt++;
													t_circle1_max_check = true;
												}
											}
											if (t_dist >= t_Circle2_mindist-2.0f && t_dist <= t_Circle2_maxdist+2.0f)
											{// 첫번째 원에 들어 있으면
												if (t_dist-2.0 <= t_Circle2_mindist && !t_circle2_min_check)
												{
													t_Circle2_connect_cnt++;
													t_circle2_min_check = true;
												}
												if (t_dist+2.0 >= t_Circle2_maxdist && !t_circle2_max_check)
												{
													t_Circle2_connect_cnt++;
													t_circle2_max_check = true;
												}
											}

											if (max(t_Circle1_connect_cnt,t_Circle2_connect_cnt) >= BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s])
											{
												break;
											}
										}
										if (max(t_Circle1_connect_cnt,t_Circle2_connect_cnt) >= BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s])
										{
											break;
										}
									}
								}
							//msg.Format("%d/%d",t_Circle1_connect_cnt,t_Circle2_connect_cnt);
							//AfxMessageBox(msg);
								t_Circle_Blob_Info.fBoundaryHitNum = max(t_Circle1_connect_cnt,t_Circle2_connect_cnt);
								t_Circle_Blob_Info.fArea = (int)(contourArea(contours[i]) + 0.5);//t_Circle_Blob_Info.Pixels.size();
								//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_Circle_Blob_Info.fArea && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_Circle_Blob_Info.fArea)
								{
									RotatedRect t_minRect = minAreaRect(contours[i]);
									t_Circle_Blob_Info.centerPoint = t_minRect.center;
									t_Circle_Blob_Info.minRect = t_minRect;
									t_Circle_Blob_Info.fWidth = t_minRect.size.width*BOLT_Param[Cam_num].nResolution[0];
									t_Circle_Blob_Info.fHeight = t_minRect.size.height*BOLT_Param[Cam_num].nResolution[1];
									t_Circle_Blob_Info.fAngle = t_minRect.angle;
									vX.push_back(t_minRect.center.x - P_Center.x);
									vY.push_back(t_minRect.center.y - P_Center.y);
									t_Circle_Blob_Info.contour = contours[i];
									t_Circle_Blob_Info.contourNum = i;
									t_Circle_Blob_Info.fLabelNum = t_cnt;
									vec_Circle_Blob_Info.push_back(t_Circle_Blob_Info);
									t_cnt++;

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,i, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
										//fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
									}
								}
							}
						}
						vector<double> Circle_Blob_mag;
						vector<double> Circle_Blob_angle;

						cartToPolar(vX, vY, Circle_Blob_mag, Circle_Blob_angle, true); // mag에는 vector의 크기, angle에는 0~360도의 값이 들어감.  
						for(int i=0; i<vX.size(); i++)
						{					
							vec_Circle_Blob_Info[i].fDistanceFromCenter = Circle_Blob_mag[i];
							vec_Circle_Blob_Info[i].fAngleFromCenter = Circle_Blob_angle[i];
							if(((int)vec_Circle_Blob_Info[i].fAngleFromCenter > 90 && (int)vec_Circle_Blob_Info[i].fAngleFromCenter <= 180) ||
								((int)vec_Circle_Blob_Info[i].fAngleFromCenter > 270 && (int)vec_Circle_Blob_Info[i].fAngleFromCenter <= 360))
							{
								float fTemp = 0;
								fTemp = vec_Circle_Blob_Info[i].fWidth;
								vec_Circle_Blob_Info[i].fWidth = vec_Circle_Blob_Info[i].fHeight;
								vec_Circle_Blob_Info[i].fHeight = fTemp;
							}
							
							float fTemp1 = vec_Circle_Blob_Info[i].fHeight;
							float fTemp2 = vec_Circle_Blob_Info[i].fWidth;

							vec_Circle_Blob_Info[i].fHeight = max(fTemp1, fTemp2);
							vec_Circle_Blob_Info[i].fWidth = min(fTemp1, fTemp2);
							//msg.Format("%1.2f/i",vec_Circle_Blob_Info[i].fAngle,i);
							//AfxMessageBox(msg);
							float tv0 = min(abs(vec_Circle_Blob_Info[i].fAngle - vec_Circle_Blob_Info[i].fAngleFromCenter),abs(vec_Circle_Blob_Info[i].fAngle - vec_Circle_Blob_Info[i].fAngleFromCenter));
							float tv1 = min(abs(vec_Circle_Blob_Info[i].fAngle + 90.0f - vec_Circle_Blob_Info[i].fAngleFromCenter),abs(vec_Circle_Blob_Info[i].fAngle - 90.0f - vec_Circle_Blob_Info[i].fAngleFromCenter));
							float tv2 = min(abs(vec_Circle_Blob_Info[i].fAngle + 180.0f - vec_Circle_Blob_Info[i].fAngleFromCenter),abs(vec_Circle_Blob_Info[i].fAngle - 180.0f - vec_Circle_Blob_Info[i].fAngleFromCenter));
							float tv3 = min(abs(vec_Circle_Blob_Info[i].fAngle + 270.0f - vec_Circle_Blob_Info[i].fAngleFromCenter),abs(vec_Circle_Blob_Info[i].fAngle - 270.0f - vec_Circle_Blob_Info[i].fAngleFromCenter));
							float tv4 = min(abs(vec_Circle_Blob_Info[i].fAngle + 360.0f - vec_Circle_Blob_Info[i].fAngleFromCenter),abs(vec_Circle_Blob_Info[i].fAngle - 360.0f - vec_Circle_Blob_Info[i].fAngleFromCenter));
							float tvm1 = min(tv1,tv2);
							float tvm2 = min(tv3,tv4);
							vec_Circle_Blob_Info[i].fAngle = min(tv0,min(tvm1,tvm2));

							if (vec_Circle_Blob_Info[i].fAngle > 45)
							{
								vec_Circle_Blob_Info[i].fAngle = min(abs(vec_Circle_Blob_Info[i].fAngle-90), abs(vec_Circle_Blob_Info[i].fAngle -180));
							}
						}

						sort(vec_Circle_Blob_Info.begin(),vec_Circle_Blob_Info.end(),Descending_Circle_Blob_Info);
						t_cnt = 0;
						for(int i=0; i<vec_Circle_Blob_Info.size(); i++)
						{
							// 여기에 로직 추가
							if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)vec_Circle_Blob_Info[i].fArea && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)vec_Circle_Blob_Info[i].fArea)
							{
								if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] > 0)
								{// 바운더리 걸리는거 있을때
									if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] <= vec_Circle_Blob_Info[i].fBoundaryHitNum)
									{
										if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 0 || BOLT_Param[Cam_num].nCircleOutputMethod[s] == 1)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fArea);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//for (int pp = 0; pp < vec_Circle_Blob_Info[i].Pixels.size(); ++pp) 
												//{
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(vec_Circle_Blob_Info[i].Pixels[pp].y);
												//	pixel[vec_Circle_Blob_Info[i].Pixels[pp].x][2] = 255;
												//	pixel[vec_Circle_Blob_Info[i].Pixels[pp].x][1] = 0;
												//	pixel[vec_Circle_Blob_Info[i].Pixels[pp].x][0] = 0;
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;

												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

												//for (int pp = 0; pp < vec_Circle_Blob_Info[i].Pixels.size(); ++pp) 
												//{
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(vec_Circle_Blob_Info[i].Pixels[pp].y);
												//	pixel[vec_Circle_Blob_Info[i].Pixels[pp].x][2] = 255;
												//	pixel[vec_Circle_Blob_Info[i].Pixels[pp].x][1] = 0;
												//	pixel[vec_Circle_Blob_Info[i].Pixels[pp].x][0] = 0;
												//}
												msg.Format("%d(%d)",t_cnt+1,vec_Circle_Blob_Info[i].fArea);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d(%d)",t_cnt+1,vec_Circle_Blob_Info[i].fArea);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 2)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fHeight);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												msg.Format("H(%1.3f)",vec_Circle_Blob_Info[i].fHeight);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",vec_Circle_Blob_Info[i].fHeight);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 3)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fWidth);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												msg.Format("W(%1.3f)",vec_Circle_Blob_Info[i].fWidth);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",vec_Circle_Blob_Info[i].fWidth);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 4)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fAngle);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												msg.Format("A(%1.1f)",vec_Circle_Blob_Info[i].fAngle);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.1f",vec_Circle_Blob_Info[i].fAngle);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 5)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fAngleFromCenter);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												msg.Format("A(%1.1f)",vec_Circle_Blob_Info[i].fAngleFromCenter);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.1f",vec_Circle_Blob_Info[i].fAngleFromCenter);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 6)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											double t_temp = 0;
											if (i < vec_Circle_Blob_Info.size() - 1)
											{
												t_temp = abs(vec_Circle_Blob_Info[i].fAngleFromCenter-vec_Circle_Blob_Info[i+1].fAngleFromCenter);
												dist_vec.push_back(t_temp);
											}
											else
											{
												t_temp = abs(vec_Circle_Blob_Info[i].fAngleFromCenter-360.0-vec_Circle_Blob_Info[0].fAngleFromCenter);
												dist_vec.push_back(t_temp);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x1 = vec_Circle_Blob_Info[i].centerPoint.x;
												double y1 = vec_Circle_Blob_Info[i].centerPoint.y;
												double x2 = vec_Circle_Blob_Info[(i+1)%vec_Circle_Blob_Info.size()].centerPoint.x;
												double y2 = vec_Circle_Blob_Info[(i+1)%vec_Circle_Blob_Info.size()].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												msg.Format("IA(%1.1f)",t_temp);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.1f",t_temp);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+(x1+x2)/2,BOLT_Param[Cam_num].nRect[s].y+(y1+y2)/2), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 7)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											if (vec_Circle_Blob_Info[i].fBoundaryHitNum == 0)
											{
												dist_vec.push_back((double)vec_Circle_Blob_Info[i].fArea);
												if (m_Text_View[Cam_num] && !ROI_Mode)
												{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												}
												if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
												{
													double x = vec_Circle_Blob_Info[i].centerPoint.x;
													double y = vec_Circle_Blob_Info[i].centerPoint.y;
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
													msg.Format("BLOB(%d)",vec_Circle_Blob_Info[i].fArea);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
													msg.Format("%d",vec_Circle_Blob_Info[i].fArea);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
												}
												t_cnt++;
											}
										}
										else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 8)
										{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
											if (vec_Circle_Blob_Info[i].fBoundaryHitNum > 0)
											{
												dist_vec.push_back((double)vec_Circle_Blob_Info[i].fArea);
												if (m_Text_View[Cam_num] && !ROI_Mode)
												{
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												}
												if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
												{
													double x = vec_Circle_Blob_Info[i].centerPoint.x;
													double y = vec_Circle_Blob_Info[i].centerPoint.y;
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

													msg.Format("BLOB(%d)",vec_Circle_Blob_Info[i].fArea);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
													msg.Format("%d",vec_Circle_Blob_Info[i].fArea);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
												}
												t_cnt++;
											}
										}
									}
								}
								else
								{
									if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 0 || BOLT_Param[Cam_num].nCircleOutputMethod[s] == 1)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										dist_vec.push_back((double)vec_Circle_Blob_Info[i].fArea);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											double x = vec_Circle_Blob_Info[i].centerPoint.x;
											double y = vec_Circle_Blob_Info[i].centerPoint.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("%d(%d)",t_cnt+1,vec_Circle_Blob_Info[i].fArea);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d(%d)",t_cnt+1,vec_Circle_Blob_Info[i].fArea);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
										}
										t_cnt++;
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 2)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										dist_vec.push_back((double)vec_Circle_Blob_Info[i].fHeight);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											double x = vec_Circle_Blob_Info[i].centerPoint.x;
											double y = vec_Circle_Blob_Info[i].centerPoint.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("H(%1.3f)",vec_Circle_Blob_Info[i].fHeight);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",vec_Circle_Blob_Info[i].fHeight);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
										}
										t_cnt++;
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 3)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										dist_vec.push_back((double)vec_Circle_Blob_Info[i].fWidth);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											double x = vec_Circle_Blob_Info[i].centerPoint.x;
											double y = vec_Circle_Blob_Info[i].centerPoint.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("W(%1.3f)",vec_Circle_Blob_Info[i].fWidth);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",vec_Circle_Blob_Info[i].fWidth);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
										}
										t_cnt++;
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 4)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										dist_vec.push_back((double)vec_Circle_Blob_Info[i].fAngle);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											double x = vec_Circle_Blob_Info[i].centerPoint.x;
											double y = vec_Circle_Blob_Info[i].centerPoint.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("A(%1.1f)",vec_Circle_Blob_Info[i].fAngle);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.1f",vec_Circle_Blob_Info[i].fAngle);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
										}
										t_cnt++;
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 5)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										dist_vec.push_back((double)vec_Circle_Blob_Info[i].fAngleFromCenter);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											double x = vec_Circle_Blob_Info[i].centerPoint.x;
											double y = vec_Circle_Blob_Info[i].centerPoint.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("A(%1.1f)",vec_Circle_Blob_Info[i].fAngleFromCenter);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.1f",vec_Circle_Blob_Info[i].fAngleFromCenter);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
										}
										t_cnt++;
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 6)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										double t_temp = 0;
										if (i < vec_Circle_Blob_Info.size() - 1)
										{
											t_temp = abs(vec_Circle_Blob_Info[i].fAngleFromCenter-vec_Circle_Blob_Info[i+1].fAngleFromCenter);
											dist_vec.push_back(t_temp);
										}
										else
										{
											t_temp = abs(vec_Circle_Blob_Info[i].fAngleFromCenter-360.0-vec_Circle_Blob_Info[0].fAngleFromCenter);
											dist_vec.push_back(t_temp);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											double x1 = vec_Circle_Blob_Info[i].centerPoint.x;
											double y1 = vec_Circle_Blob_Info[i].centerPoint.y;
											double x2 = vec_Circle_Blob_Info[(i+1)%vec_Circle_Blob_Info.size()].centerPoint.x;
											double y2 = vec_Circle_Blob_Info[(i+1)%vec_Circle_Blob_Info.size()].centerPoint.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

											msg.Format("IA(%1.1f)",t_temp);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.1f",t_temp);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+(x1+x2)/2,BOLT_Param[Cam_num].nRect[s].y+(y1+y2)/2), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
										}
										t_cnt++;
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 7)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										if (vec_Circle_Blob_Info[i].fBoundaryHitNum == 0)
										{
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fArea);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

												msg.Format("BLOB(%d)",vec_Circle_Blob_Info[i].fArea);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",vec_Circle_Blob_Info[i].fArea);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 8)
									{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
										if (vec_Circle_Blob_Info[i].fBoundaryHitNum > 0)
										{
											dist_vec.push_back((double)vec_Circle_Blob_Info[i].fArea);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												double x = vec_Circle_Blob_Info[i].centerPoint.x;
												double y = vec_Circle_Blob_Info[i].centerPoint.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours,vec_Circle_Blob_Info[i].contourNum, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);

												msg.Format("BLOB(%d)",vec_Circle_Blob_Info[i].fArea);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",vec_Circle_Blob_Info[i].fArea);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(0,255,0), 1, 8);
											}
											t_cnt++;
										}
									}
								}
							}
						}

						if (BOLT_Param[Cam_num].nCircleOutputMethod[s] == 1)
						{//"계산방법(0:픽셀수,1:갯수,2:높이,3:넓이,4:축각도,5:각도(중심에서),6:사이각,7:픽셀수(가장자리 제외),8:픽셀수(가장자리))";
							dist_vec.clear();
							dist_vec.push_back((double)t_cnt);
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::CIRCLE_BLOB_COUNT_TB) // Edge Crack
				{
					if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] < 0)
					{
						BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] = 0;
					}
					else if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] > 2)
					{
						BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] = 2;
					}
					Point2f P_Center;
					P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x - BOLT_Param[Cam_num].nRect[s].x;
					P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y - BOLT_Param[Cam_num].nRect[s].y;
					//P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x + (BOLT_Param[Cam_num].nRect[0].x - BOLT_Param[Cam_num].nRect[s].x);
					//P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y + (BOLT_Param[Cam_num].nRect[0].y - BOLT_Param[Cam_num].nRect[s].y);
					//P_Center.x = BOLT_Param[Cam_num].nRect[s].width/2;
					//P_Center.y = BOLT_Param[Cam_num].nRect[s].height/2;
					if (BOLT_Param[Cam_num].nCirclePositionMethod[s] > 0 && BOLT_Param[Cam_num].nCirclePositionMethod[s] < 3)
					{
						Mat P_Out_binary;
						if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
						{// P값 이하로 찾음
							threshold(CP_Gray_Img,P_Out_binary,BOLT_Param[Cam_num].nCirclePositionThreshold[s],255,CV_THRESH_BINARY_INV);
						}
						else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
						{// P값 이상으로 찾음
							threshold(CP_Gray_Img,P_Out_binary,BOLT_Param[Cam_num].nCirclePositionThreshold[s],255,CV_THRESH_BINARY);
						}
						findContours( P_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if(contours.size() != 0) // 칸투어 갯수로 예외처리
						{
							vector<Rect> boundRect( contours.size() );
							int m_max_object_num = -1;int m_max_object_value = 0;
							for( int k = 0; k < contours.size(); k++ )
							{  
								boundRect[k] = boundingRect( Mat(contours[k]) );
								if (m_max_object_value <= boundRect[k].width*boundRect[k].height)
									//	&& pointPolygonTest( contours[k], Point(tROI.width/2,tROI.height/2), false ) == 1)
								{
									m_max_object_value = boundRect[k].width*boundRect[k].height;
									m_max_object_num = k;
								}
							}
							P_Center.x = (float)boundRect[m_max_object_num].x + (float)boundRect[m_max_object_num].width/2.0f;
							P_Center.y = (float)boundRect[m_max_object_num].y + (float)boundRect[m_max_object_num].height/2.0f;
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							//#pragma omp parallel for
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							}
						}
					}

					double t_Circle1_mindist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] - (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
					double t_Circle1_maxdist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] + (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
					//double t_Circle2_mindist = (double)BOLT_Param[Cam_num].nCircle2Radius[s] - (double)(BOLT_Param[Cam_num].nCircle2Thickness[s])/2;
					//double t_Circle2_maxdist = (double)BOLT_Param[Cam_num].nCircle2Radius[s] + (double)(BOLT_Param[Cam_num].nCircle2Thickness[s])/2;
					int t_Circle1_connect_cnt = 0;
					//int t_Circle2_connect_cnt = 0;

					Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

					//float t_angle = 360;
					//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
					//{// P값 이상으로 찾음
					//	t_angle = BOLT_Param[Cam_num].nCirclePositionThreshold[s];
					//}

					if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
					{
						//ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
						ellipse(Mask,P_Center,Size(t_Circle1_maxdist,t_Circle1_maxdist),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
						//ellipse(Mask,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(0,0,0),CV_FILLED,8,0);
						ellipse(Mask,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),0,0,360,Scalar(0,0,0),CV_FILLED,8,0);
					}
					else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
					{
						ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
					}

					//if (BOLT_Param[Cam_num].nCircle2Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle2Radius[s] >= 0)
					//{
					//	ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),BOLT_Param[Cam_num].nCircle2Thickness[s],0,0);
					//}
					//else if (BOLT_Param[Cam_num].nCircle2Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle2Radius[s] > 0)
					//{
					//	ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),CV_FILLED,8,0);
					//}

					if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::DIFFAVG) // 평균기준 차이
					{
						Scalar tempVal = mean( CP_Gray_Img, Mask );
						float myMAtMean = tempVal.val[0];
						Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
						inRange(CP_Gray_Img, Scalar(myMAtMean - BOLT_Param[Cam_num].nThres_V1[s]),Scalar(myMAtMean + BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
						//imwrite("00.bmp",Out_binary);
						subtract(White_Out_binary, Out_binary, Out_binary);
						//imwrite("01.bmp",Out_binary);
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							msg.Format("Avg = %1.3f", myMAtMean);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							msg.Format("Avg = %1.3f", myMAtMean);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
						}
					}

					bitwise_and(Mask,Out_binary,Out_binary);

					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(0,0,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(0,0,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							}
						}
					}

					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(0,0,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(0,0,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							}
						}
						findContours( Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,100,0), 3, 8, hierarchy);
								//fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							}
						}
					}

					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						J_Delete_Boundary(Out_binary,1);
					}

					Mat stats, centroids, label;  
					int numOfLables = connectedComponentsWithStats(Out_binary, label,   
						stats, centroids, 8,CV_32S);
					int t_cnt = 0;double t_dist = 0;
					int area = 0;int left = 0;int top = 0;int right = 0;int bottom = 0;
					bool t_circle1_min_check = false;
					bool t_circle1_max_check = false;
					//bool t_circle2_min_check = false;
					//bool t_circle2_max_check = false;
					if (numOfLables >= 1)
					{
						for (int j = 1; j < numOfLables; j++) 
						{
							area = stats.at<int>(j, CC_STAT_AREA);
							left = stats.at<int>(j, CC_STAT_LEFT);
							top = stats.at<int>(j, CC_STAT_TOP);
							right = left + stats.at<int>(j, CC_STAT_WIDTH);
							bottom = top + stats.at<int>(j, CC_STAT_HEIGHT);

							if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
							{
								t_Circle1_connect_cnt = 0;
								//t_Circle2_connect_cnt = 0;
								t_circle1_min_check = false;
								t_circle1_max_check = false;							
								//t_circle2_min_check = false;
								//t_circle2_max_check = false;
								for (int y = top; y < bottom; ++y) {

									int *tlabel = label.ptr<int>(y);
									for (int x = left; x < right; ++x) {
										if (tlabel[x] == j)
										{
											t_dist = sqrt((x-P_Center.x)*(x-P_Center.x)+(y-P_Center.y)*(y-P_Center.y));
											if (t_dist >= t_Circle1_mindist && t_dist <= t_Circle1_maxdist)
											{// 첫번째 원에 들어 있으면
												if (t_dist-1.0f < t_Circle1_mindist && !t_circle1_min_check)
												{
													t_Circle1_connect_cnt++;
													t_circle1_min_check = true;
												}
												else if (t_dist+1.0f > t_Circle1_maxdist && !t_circle1_max_check)
												{
													t_Circle1_connect_cnt++;
													t_circle1_max_check = true;
												}
											}
											//else if (t_dist >= t_Circle2_mindist && t_dist <= t_Circle2_maxdist)
											//{// 두번째 원에 들어 있으면
											//	if (t_dist-1.0f < t_Circle2_mindist && !t_circle2_min_check)
											//	{
											//		t_Circle2_connect_cnt++;
											//		t_circle2_min_check = true;
											//	}
											//	else if (t_dist+1.0f > t_Circle2_maxdist && !t_circle2_max_check)
											//	{
											//		t_Circle2_connect_cnt++;
											//		t_circle2_max_check = true;
											//	}
											//}
										}
									}
								}

								if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] > 0)
								{
									if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] <= t_Circle1_connect_cnt)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("%d",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("No(%d)",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
									//if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] <= t_Circle2_connect_cnt)
									//{
									//	if (m_Text_View[Cam_num] && !ROI_Mode)
									//	{
									//		int x = centroids.at<double>(j, 0); //중심좌표
									//		int y = centroids.at<double>(j, 1);

									//		msg.Format("%d",t_cnt+1);
									//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									//	}

									//	if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									//	{
									//		int x = centroids.at<double>(j, 0); //중심좌표
									//		int y = centroids.at<double>(j, 1);

									//		msg.Format("No(%d)",t_cnt+1);
									//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
									//		msg.Format("%d",t_cnt+1);
									//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									//	}
									//	t_cnt++;
									//}
								}
								else
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("%d",t_cnt+1);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("No(%d)",t_cnt+1);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										msg.Format("%d",t_cnt+1);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
									t_cnt++;
								}
							}
						}
					}
					dist_vec.push_back((double)t_cnt);
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::CIRCULARITY_TB)
				{
					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1),BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1),BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						J_Delete_Boundary(Out_binary,1);
					}
					else
					{
						J_Delete_Boundary(Out_binary,1);
					}

					// 큰 Object 찾기
					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if(contours.size() == 0) // 칸투어 갯수로 예외처리
					{
						msg.Format("No Object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
						dist_vec.push_back(0);
					}
					else
					{
						vector<RotatedRect> boundRect( contours.size() );
						double V_R = 0;float R_diameter = 0;
						int m_max_object_num = -1;double m_max_object_value = 0;
						for( int k = 0; k < contours.size(); k++ )
						{  
							if (hierarchy[k][3] == -1)
							{
								boundRect[k] = minAreaRect(contours[k]);
								R_diameter = max(boundRect[k].size.width,boundRect[k].size.height);
								V_R = 0.5*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*(double)R_diameter;
								if (V_R >= BOLT_Param[Cam_num].nDiameter_Min_Size[s] && V_R <= BOLT_Param[Cam_num].nDiameter_Max_Size[s])
								{
									if (m_max_object_value <= V_R)
									{
										m_max_object_value = V_R;
										m_max_object_num = k;
									}
								}
							}
						}

						if (m_max_object_num > -1)
						{
							//Circle circlef,circleIni;reals LambdaIni=0.001;
							//int n = contours[m_max_object_num].size();
							//double* DataX = new double[n];
							//double* DataY = new double[n];

							//for(int s = 0; s < contours[m_max_object_num].size(); s++)
							//{
							//	DataX[s] = contours[m_max_object_num][s].x;
							//	DataY[s] = contours[m_max_object_num][s].y;
							//}
							//Data dataXY(n,DataX,DataY);

							//delete [] DataX;
							//delete [] DataY;

							//circleIni = CircleFitByTaubin (dataXY);
							//int code = CircleFitByLevenbergMarquardtReduced (dataXY,circleIni,LambdaIni,circlef);
							//if (circlef.a - circlef.r < 0 || circlef.a + circlef.r >=  Out_binary.cols || circlef.b - circlef.r < 0 || circlef.b + circlef.r >=  Out_binary.rows)
							//{
							//	code = 1;
							//}

							//if (code == 0)
							{
								RotatedRect minRect = minAreaRect(contours[m_max_object_num]);
								double t_v = 100.0*min(minRect.size.width,minRect.size.height) / max(minRect.size.width,minRect.size.height);
								//t_v *= (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])/2;
								//double t_v = 100.0*min(circlef.a,circlef.b) / max(circlef.a,circlef.b);
								//msg.Format("%1.2f",t_v);
								//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, 1, CV_RGB(255,100,0), 1, 8);
								dist_vec.push_back(t_v);

								if (m_Text_View[Cam_num]&& !ROI_Mode)
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(255,0,0), 1, 1, hierarchy);
									//ellipse(Dst_Img[Cam_num],Point(minRect.center.x+BOLT_Param[Cam_num].nRect[s].x,minRect.center.y+BOLT_Param[Cam_num].nRect[s].y),Size(minRect.size.width/2,minRect.size.height/2),minRect.angle,0,360,CV_RGB(255,0,0),1,1,0);
									Point2f rect_points[4];
									minRect.points( rect_points );
									for( int j = 0; j < 4; j++ )
									{
										line( Dst_Img[Cam_num], Point2f(rect_points[j].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[j].y + BOLT_Param[Cam_num].nRect[s].y) , 
											Point2f(rect_points[(j+1)%4].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[(j+1)%4].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(200,15,15), 1, 8 );
									}
								}

								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(255,0,0), 1, 1, hierarchy);
									//ellipse(Dst_Img[Cam_num],Point(minRect.center.x+BOLT_Param[Cam_num].nRect[s].x,minRect.center.y+BOLT_Param[Cam_num].nRect[s].y),Size(minRect.size.width/2,minRect.size.height/2),minRect.angle,0,360,CV_RGB(255,0,0),1,1,0);
									Point2f rect_points[4];
									minRect.points( rect_points );
									for( int j = 0; j < 4; j++ )
									{
										line( Dst_Img[Cam_num], Point2f(rect_points[j].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[j].y + BOLT_Param[Cam_num].nRect[s].y) , 
											Point2f(rect_points[(j+1)%4].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[(j+1)%4].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(200,15,15), 1, 8 );
									}
									msg.Format("Min(%1.3f)",min(minRect.size.width,minRect.size.height)*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*0.5);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*1), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
									msg.Format("Max(%1.3f)",max(minRect.size.width,minRect.size.height)*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*0.5);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*2), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
									msg.Format("R(%1.3f)",t_v);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*3), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
									msg.Format("R=100*Min/Max");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*4), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::PITCH_COIN_TB) // Pitch of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{
							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
							dist_vec.clear();
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size()-1;i++)
								{
									if (Pitch_Info[i+1].CY - Pitch_Info[i].CY > 0)
									{
										double t_R = (Pitch_Info[i+1].CY - Pitch_Info[i].CY)*BOLT_Param[Cam_num].nResolution[1];
										dist_vec.push_back(t_R);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											if (i == Pitch_Info.size()-2)
											{
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),1,CV_RGB(255,100,0),1);
											}
											msg.Format("P%2d(%1.2f)",dist_vec.size(),t_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{

						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.x > 1 && boundRect.x + boundRect.width < Morph_Out_binary.cols-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size_X);
							dist_vec.clear();
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size()-1;i++)
								{
									if (Pitch_Info[i+1].CX - Pitch_Info[i].CX > 0)
									{
										double t_R = (Pitch_Info[i+1].CX - Pitch_Info[i].CX)*BOLT_Param[Cam_num].nResolution[0];
										dist_vec.push_back(t_R);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}

										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											if (i == Pitch_Info.size()-2)
											{
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),1,CV_RGB(255,100,0),1);
											}
											msg.Format("P%2d(%1.2f)",dist_vec.size(),t_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::DIST_TWO_CENTER_TB) // DIST_TWO_CENTER_TB
				{
					Mat ROI1_Img, ROI2_Img;
					bool ROI1_check = false;
					bool ROI2_check = false;

					if (BOLT_Param[Cam_num].nROI1[s].x + BOLT_Param[Cam_num].nROI1[s].width > Out_binary.cols-1)
					{
						BOLT_Param[Cam_num].nROI1[s].width = Out_binary.cols - BOLT_Param[Cam_num].nROI1[s].x -1;
					}
					if (BOLT_Param[Cam_num].nROI2[s].x + BOLT_Param[Cam_num].nROI2[s].width > Out_binary.cols-1)
					{
						BOLT_Param[Cam_num].nROI2[s].width = Out_binary.cols - BOLT_Param[Cam_num].nROI2[s].x -1;
					}
					if (BOLT_Param[Cam_num].nROI1[s].y + BOLT_Param[Cam_num].nROI1[s].height > Out_binary.rows-1)
					{
						BOLT_Param[Cam_num].nROI1[s].height = Out_binary.rows - BOLT_Param[Cam_num].nROI1[s].y -1;
					}
					if (BOLT_Param[Cam_num].nROI2[s].y + BOLT_Param[Cam_num].nROI2[s].height > Out_binary.rows-1)
					{
						BOLT_Param[Cam_num].nROI2[s].height = Out_binary.rows - BOLT_Param[Cam_num].nROI2[s].y -1;
					}

					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI1[s],CV_RGB(100,0,255),1);
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI2[s],CV_RGB(100,0,255),1);
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI1[s],CV_RGB(100,0,255),1);
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI2[s],CV_RGB(100,0,255),1);
					}
					if (BOLT_Param[Cam_num].nROI1[s].x >= 0 && BOLT_Param[Cam_num].nROI1[s].y >= 0 && 
						BOLT_Param[Cam_num].nROI1[s].x + BOLT_Param[Cam_num].nROI1[s].width < Out_binary.cols-1 &&
						BOLT_Param[Cam_num].nROI1[s].y + BOLT_Param[Cam_num].nROI1[s].height < Out_binary.rows-1)
					{
						ROI1_Img = Out_binary(BOLT_Param[Cam_num].nROI1[s]).clone();
						ROI1_check = true;
					}
					if (BOLT_Param[Cam_num].nROI2[s].x >= 0 && BOLT_Param[Cam_num].nROI2[s].y >= 0 && 
						BOLT_Param[Cam_num].nROI2[s].x + BOLT_Param[Cam_num].nROI2[s].width < Out_binary.cols-1 &&
						BOLT_Param[Cam_num].nROI2[s].y + BOLT_Param[Cam_num].nROI2[s].height < Out_binary.rows-1)
					{
						ROI2_Img = Out_binary(BOLT_Param[Cam_num].nROI2[s]).clone();
						ROI2_check = true;
					}

					if (!ROI1_check || !ROI2_check)
					{
						dist_vec.push_back(0);
					}
					else
					{
						Mat Mp_ROI1_Img, Mp_ROI2_Img;
						if (BOLT_Param[Cam_num].nROI12_Direction[s] == 0) // 가로방향
						{
							erode(ROI1_Img,Mp_ROI1_Img,element_h,Point(-1,-1), 1);
							subtract(ROI1_Img,Mp_ROI1_Img,Mp_ROI1_Img);
							erode(ROI2_Img,Mp_ROI2_Img,element_h,Point(-1,-1), 1);
							subtract(ROI2_Img,Mp_ROI2_Img,Mp_ROI2_Img);

							Point2f ROI1_Center;Point2f ROI2_Center;
							vector<Point2f> vec_ROI1_Point;vector<Point2f> vec_ROI2_Point;
							for (int j=1;j < Mp_ROI1_Img.rows-2;j++)
							{
								for (int i = 1; i < Mp_ROI1_Img.cols-2;i++)
								{
									if (Mp_ROI1_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI1_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI1[s].x,(float)j+BOLT_Param[Cam_num].nROI1[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}

							for (int j=1;j < Mp_ROI2_Img.rows-2;j++)
							{
								for (int i = 1; i < Mp_ROI2_Img.cols-2;i++)
								{
									if (Mp_ROI2_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI2_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI2[s].x,(float)j+BOLT_Param[Cam_num].nROI2[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}

							if (vec_ROI1_Point.size() == 0 || vec_ROI2_Point.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								float t_dist = 0;
								float t_dist_min = 999999999;float t_dist_max = 0;int t_ROI1_IDX = 0;int t_ROI2_IDX = 0;
								if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MIN) // 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_min*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MAX) // 최대
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_max*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::AVERAGE) // 평균
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									v_Dist = abs(ROI2_Center.x - ROI1_Center.x)*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::RANGE) // 최대 - 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = (t_dist_max-t_dist_min)*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}							
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::SUMOFALL) // 합계
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist += abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											t_ROI1_IDX++;
										}
									}
									t_dist /= (float)t_ROI1_IDX;
									v_Dist = t_dist*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI1_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI2_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI1_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI2_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
							}
						}
						else if (BOLT_Param[Cam_num].nROI12_Direction[s] == 1) // 세로방향
						{
							erode(ROI1_Img,Mp_ROI1_Img,element_v,Point(-1,-1), 1);
							subtract(ROI1_Img,Mp_ROI1_Img,Mp_ROI1_Img);
							erode(ROI2_Img,Mp_ROI2_Img,element_v,Point(-1,-1), 1);
							subtract(ROI2_Img,Mp_ROI2_Img,Mp_ROI2_Img);

							Point2f ROI1_Center;Point2f ROI2_Center;
							vector<Point2f> vec_ROI1_Point;vector<Point2f> vec_ROI2_Point;
							for (int i = 1; i < Mp_ROI1_Img.cols-2;i++)
							{
								for (int j=1;j < Mp_ROI1_Img.rows-2;j++)
								{
									if (Mp_ROI1_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI1_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI1[s].x,(float)j+BOLT_Param[Cam_num].nROI1[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}

							for (int i = 1; i < Mp_ROI2_Img.cols-2;i++)
							{
								for (int j=1;j < Mp_ROI2_Img.rows-2;j++)
								{
									if (Mp_ROI2_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI2_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI2[s].x,(float)j+BOLT_Param[Cam_num].nROI2[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}
							if (vec_ROI1_Point.size() == 0 || vec_ROI2_Point.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{

								float t_dist = 0;
								float t_dist_min = 999999999;float t_dist_max = 0;int t_ROI1_IDX = 0;int t_ROI2_IDX = 0;
								if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MIN) // 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_min*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MAX) // 최대
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_max*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::AVERAGE) // 평균
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									v_Dist = abs(ROI2_Center.y - ROI1_Center.y)*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::RANGE) // 최대 - 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = (t_dist_max-t_dist_min)*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}							
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::SUMOFALL) // 합계
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist += abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											t_ROI1_IDX++;
										}
									}
									t_dist /= (float)t_ROI1_IDX;
									v_Dist = t_dist*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI1_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI2_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI1_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI2_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::AREA_COIN_TB) // Size of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;
							dist_vec.clear();
							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-1 && mu[k].m00 > 0)
								{
									//if (boundRect.x > 1)
										double t_R = 0;
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
										{
											t_R = mu[k].m00;
											
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
										{
											t_R = (double)boundRect.width;
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
										{
											t_R = (double)boundRect.height;
										}
										dist_vec.push_back(t_R);
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										msg.Format("S%2d(%1.2f)",dist_vec.size(),t_R);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;
							dist_vec.clear();
							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.x > 1 && boundRect.x + boundRect.width < Morph_Out_binary.cols-1 && mu[k].m00 > 0)
								{
									double t_R = 0;
									//if (boundRect.x > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
										{
											t_R = mu[k].m00;
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
										{
											t_R = (double)boundRect.width;
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
										{
											t_R = (double)boundRect.height;
										}
										dist_vec.push_back(t_R);
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										msg.Format("S%2d(%1.2f)",dist_vec.size(),t_R);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::COLOR_BLOB_TB) // Edge Crack
				{
					//AfxMessageBox("1");
					Point2f P_Center;
					P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x - BOLT_Param[Cam_num].nRect[s].x;
					P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y - BOLT_Param[Cam_num].nRect[s].y;

					if (BOLT_Param[Cam_num].nCirclePositionMethod[s] > 0 && BOLT_Param[Cam_num].nCirclePositionMethod[s] < 3)
					{
						Mat P_Out_binary;
						if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
						{// P값 이하로 찾음
							threshold(CP_Gray_Img,P_Out_binary,BOLT_Param[Cam_num].nCirclePositionThreshold[s],255,CV_THRESH_BINARY_INV);
						}
						else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
						{// P값 이상으로 찾음
							threshold(CP_Gray_Img,P_Out_binary,BOLT_Param[Cam_num].nCirclePositionThreshold[s],255,CV_THRESH_BINARY);
						}
						findContours( P_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if(contours.size() != 0) // 칸투어 갯수로 예외처리
						{
							vector<Rect> boundRect( contours.size() );
							int m_max_object_num = -1;int m_max_object_value = 0;
							for( int k = 0; k < contours.size(); k++ )
							{  
								boundRect[k] = boundingRect( Mat(contours[k]) );
								if (m_max_object_value <= boundRect[k].width*boundRect[k].height)
									//	&& pointPolygonTest( contours[k], Point(tROI.width/2,tROI.height/2), false ) == 1)
								{
									m_max_object_value = boundRect[k].width*boundRect[k].height;
									m_max_object_num = k;
								}
							}
							P_Center.x = (float)boundRect[m_max_object_num].x + (float)boundRect[m_max_object_num].width/2.0f;
							P_Center.y = (float)boundRect[m_max_object_num].y + (float)boundRect[m_max_object_num].height/2.0f;
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							//#pragma omp parallel for
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 0, 1, hierarchy);
							}
						}
					}

					Mat CP_Src_Img = Src_Img[Cam_num].clone();
					if (BOLT_Param[Cam_num].nColorBlurFilterCNT[s] > 0)
					{
						blur(Src_Img[Cam_num],CP_Src_Img, Size(BOLT_Param[Cam_num].nColorBlurFilterCNT[s]*2+1,BOLT_Param[Cam_num].nColorBlurFilterCNT[s]*2+1));
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						Mat HSV_Img1;Mat HSV_channel1[3];
						cvtColor(CP_Src_Img, HSV_Img1, CV_BGR2Lab);
						//blur(HSV_Img, HSV_Img, Size(5,5));
						split(HSV_Img1, HSV_channel1);
						cvtColor(HSV_channel1[1],Dst_Img[Cam_num],CV_GRAY2BGR);
					}

					Mat HSV_Img;Mat HSV_channel[3];
					cvtColor(CP_Src_Img(BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2Lab);
					//cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2HSV);
					//blur(HSV_Img, HSV_Img, Size(5,5));
					split(HSV_Img, HSV_channel);
					inRange(HSV_channel[1], Scalar(BOLT_Param[Cam_num].nColorMinThres[s]),Scalar(BOLT_Param[Cam_num].nColorMaxThres[s]),Out_binary);

					double t_Circle1_mindist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] - (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
					double t_Circle1_maxdist = (double)BOLT_Param[Cam_num].nCircle1Radius[s] + (double)(BOLT_Param[Cam_num].nCircle1Thickness[s])/2;
					int t_Circle1_connect_cnt = 0;

					Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

					float t_angle = 360;
					if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
					{// P값 이상으로 찾음
						t_angle = BOLT_Param[Cam_num].nCirclePositionThreshold[s];
					}

					if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
					{
						//ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),-90,0,t_angle,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
						ellipse(Mask,P_Center,Size(t_Circle1_maxdist,t_Circle1_maxdist),-90,0,t_angle,Scalar(255,255,255),CV_FILLED,8,0);
						ellipse(Mask,P_Center,Size(t_Circle1_mindist,t_Circle1_mindist),-90,0,t_angle,Scalar(0,0,0),CV_FILLED,8,0);
					}
					else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
					{
						ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),-90,0,t_angle,Scalar(255,255,255),CV_FILLED,8,0);
					}

					bitwise_and(Mask,Out_binary,Out_binary);

					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(0,0,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
					}

					//if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					//{
					//	erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
					//	dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
					//}

					Mat stats, centroids, label;  
					int numOfLables = connectedComponentsWithStats(Out_binary, label, stats, centroids, 8,CV_32S);
					int area = 0;int left = 0;int top = 0;int right = 0;int bottom = 0;int cnt = 0;
					for (int j = 1; j < numOfLables; j++) 
					{
						area = stats.at<int>(j, CC_STAT_AREA);
						if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] > (double)area || BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] < (double)area)
						{
							left = stats.at<int>(j, CC_STAT_LEFT);
							top = stats.at<int>(j, CC_STAT_TOP);
							right = left + stats.at<int>(j, CC_STAT_WIDTH);
							bottom = top + stats.at<int>(j, CC_STAT_HEIGHT);

							for (int y = top; y < bottom; ++y) {

								int *tlabel = label.ptr<int>(y);

								for (int x = left; x < right; ++x) {
									if (tlabel[x] == j)
									{
										Out_binary.at<uchar>(y,x) = 0;
									}
								}
							}
						}
						else
						{
							if (BOLT_Param[Cam_num].nColorOutput[s] == 1)
							{
								dist_vec.push_back((double)area);
							}
							cnt++;
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								int x = centroids.at<double>(j, 0); //중심좌표
								int y = centroids.at<double>(j, 1);

								msg.Format("%d(%d)",cnt, area);
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
							}
						}

					}

					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,50,0), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,50,0),8);
							}
						}
					}
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,50,0), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,50,0),8);
							}
						}
					}
					if (BOLT_Param[Cam_num].nColorOutput[s] == 0)
					{
						for (int i=0;i<Out_binary.rows;i++)
						{
							for (int j=0;j<Out_binary.cols;j++)
							{
								if (Out_binary.at<uchar>(i,j) > 0)
								{
									dist_vec.push_back((double)HSV_channel[1].at<uchar>(i,j));
								}
							}	
						}
					}
					if (BOLT_Param[Cam_num].nColorOutput[s] == 2)
					{
						dist_vec.push_back((double)cnt);
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::CONVEX_BLOB_TB) // CONVEX BLOB ANALYSIS
				{
					vector<vector<Point>> contours1;
					vector<Vec4i> hierarchy1;

					Mat Convex_Img = Out_binary.clone();
					findContours( Out_binary.clone(), contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if (contours1.size() > 0)
					{
						vector<vector<Point>> hull1(contours1.size());
						for(size_t k=0; k<contours1.size(); k++)
						{	
							convexHull( Mat(contours1[k]), hull1[k], false );
							//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
							drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								//#pragma omp parallel for
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,50,0),8);
								}
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								//#pragma omp parallel for
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,50,0),8);
								}
							}
						}
					}
					subtract(Convex_Img,Out_binary,Convex_Img);
					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						dilate(Convex_Img,Convex_Img,element,Point(-1,-1), 1);
						erode(Convex_Img,Convex_Img,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]+1);
						dilate(Convex_Img,Convex_Img,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
					}

					Mat stats, centroids, label;  
					int numOfLables = connectedComponentsWithStats(Convex_Img, label,   
						stats, centroids, 8,CV_32S);
					int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
					if (numOfLables > 0)
					{
						for (int j = 1; j < numOfLables; j++) 
						{
							area = stats.at<int>(j, CC_STAT_AREA);
							t_w = stats.at<int>(j, CC_STAT_WIDTH);
							t_h = stats.at<int>(j, CC_STAT_HEIGHT);
							if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
							{
								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
								{
									dist_vec.push_back((double)area);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("BLOB(%d)",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										msg.Format("%d",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
								{
									dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
								{
									dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[0]);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("W(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}

								t_cnt++;
							}
						}
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,0,0),8);
								}
							}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,0,0),8);
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::DIFFINNEROUTTER_TB) // CONVEX BLOB ANALYSIS
				{
					// 내외경 중심 차이 알고리즘 추가
					//imwrite("00.bmp",Out_binary);
					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					int m_max_object_num = -1;int m_max_object_value = 0;
					RotatedRect minRect;
					Rect tt_rect;
					if (contours.size() > 0)
					{
						for( int k = 0; k < contours.size(); k++ )
						{  
							tt_rect = boundingRect( Mat(contours[k]) );
							minRect = minAreaRect(contours[k]);
							if (m_max_object_value <= minRect.size.width*minRect.size.height
								&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
								&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
							{
								m_max_object_value =  minRect.size.width*minRect.size.height;
								m_max_object_num = k;
							}
						}
					}
					if (m_max_object_num >= 0)
					{
						RotatedRect Outcircle_Info = minAreaRect(contours[m_max_object_num]);// = fitEllipse(Mat(contours[m_max_object_num]));

						Mat Outcircle_Img = Mat::zeros(CP_Gray_Img.size(), CV_8UC1);
						ellipse(Outcircle_Img, Outcircle_Info, CV_RGB(255,255,255), CV_FILLED, 8);
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s && BOLT_Param[Cam_num].nOutput[s] != 2)
						{						
							ellipse(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Outcircle_Info, CV_RGB(0,255,0), 1, 8);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Outcircle_Info.center,1,CV_RGB(0,255,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Outcircle_Info.center,2,CV_RGB(255,0,0),1);
						}

						if (m_Text_View[Cam_num] && !ROI_Mode && BOLT_Param[Cam_num].nOutput[s] != 2)
						{
							ellipse(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Outcircle_Info, CV_RGB(0,255,0), 1, 8);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Outcircle_Info.center,1,CV_RGB(0,255,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Outcircle_Info.center,2,CV_RGB(255,0,0),1);
						}

						if (BOLT_Param[Cam_num].nThresMethod_InnerCircle[s] == THRES_METHOD::BINARY_INV) // V1이하
						{
							threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_InnerCircle[s],255,CV_THRESH_BINARY_INV);
						} 
						else if (BOLT_Param[Cam_num].nThresMethod_InnerCircle[s] == THRES_METHOD::BINARY) // V2이상
						{
							threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_InnerCircle[s],255,CV_THRESH_BINARY);
						}
						Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
						if (BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
						{
							ellipse(Mask,Outcircle_Info.center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}
						bitwise_and(Mask,Out_binary,Out_binary);
						//imwrite("00.bmp",Out_binary);
						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						}
						if (BOLT_Param[Cam_num].nROI0_MergeFilterSize[s] > 0)
						{
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[s]);
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[s]);
							J_Delete_Boundary(Out_binary,1);
						}

						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						m_max_object_num = -1;m_max_object_value = 0;
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{  
								tt_rect = boundingRect( Mat(contours[k]) );
								minRect = minAreaRect(contours[k]);
								if (m_max_object_value <= minRect.size.width*minRect.size.height
									&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
									&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
								{
									m_max_object_value =  minRect.size.width*minRect.size.height;
									m_max_object_num = k;
								}
							}
						}
						if (m_max_object_num >= 0)
						{
							Out_binary = Mat::zeros(CP_Gray_Img.size(), CV_8UC1);
							drawContours( Out_binary, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8);
							//imwrite("01.bmp",Out_binary);
							RotatedRect Incircle_Info = minAreaRect(contours[m_max_object_num]);//fitEllipse(Mat(contours[m_max_object_num]));

							Outcircle_Img = Mat::zeros(CP_Gray_Img.size(), CV_8UC1);
							ellipse(Outcircle_Img, Incircle_Info, CV_RGB(255,255,255), CV_FILLED, 8);
							bitwise_and(Outcircle_Img,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							m_max_object_num = -1;m_max_object_value = 0;
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{  
									tt_rect = boundingRect( Mat(contours[k]) );
									minRect = minAreaRect(contours[k]);
									if (m_max_object_value <= minRect.size.width*minRect.size.height
										&& tt_rect.x > 1 && tt_rect.x+tt_rect.width < Out_binary.cols-2
										&& tt_rect.y > 1 && tt_rect.y+tt_rect.height < Out_binary.rows-2)
									{
										m_max_object_value =  minRect.size.width*minRect.size.height;
										m_max_object_num = k;
									}
								}
							}
							Incircle_Info = minAreaRect(contours[m_max_object_num]);//fitEllipse(Mat(contours[m_max_object_num]));
							//Mat Dist_Img, label;//, //Tmp_Img;
							//distanceTransform(Out_binary, Dist_Img,label, CV_DIST_L2,3);
							double minval = 0, maxval = 0;
							cv::Point minLoc, CenterLoc;
							//cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);
						
							maxval = (Incircle_Info.size.width+Incircle_Info.size.height)/4;
							CenterLoc = Incircle_Info.center;
							if (BOLT_Param[Cam_num].nOutput[s] == 0)
							{
								minval = sqrt(BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]*(Outcircle_Info.center.x - (float)CenterLoc.x)*(Outcircle_Info.center.x - (float)CenterLoc.x)
									+ BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]*(Outcircle_Info.center.y - (float)CenterLoc.y)*(Outcircle_Info.center.y - (float)CenterLoc.y));
							} 
							else if (BOLT_Param[Cam_num].nOutput[s] == 1)
							{
								minval = sqrt((Outcircle_Info.center.x - (float)CenterLoc.x)*(Outcircle_Info.center.x - (float)CenterLoc.x)
									+ (Outcircle_Info.center.y - (float)CenterLoc.y)*(Outcircle_Info.center.y - (float)CenterLoc.y));
							}
							else if (BOLT_Param[Cam_num].nOutput[s] == 2)
							{//내경
								minval = maxval*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1]);
							}
							else if (BOLT_Param[Cam_num].nOutput[s] == 3)
							{//외경
								minval = (Outcircle_Info.size.width + Outcircle_Info.size.height)*0.25*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1]);
							}
							else if (BOLT_Param[Cam_num].nOutput[s] == 4)
							{//외경-내경
								minval = (Outcircle_Info.size.width + Outcircle_Info.size.height)*0.25*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])
									- maxval*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1]);
							}
							dist_vec.push_back(minval);

							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								if (BOLT_Param[Cam_num].nOutput[s] != 3)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(0,0,255),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,maxval,CV_RGB(0,0,255),1);
								}
								msg.Format("%1.3f",minval);
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + CenterLoc.x - 5,BOLT_Param[Cam_num].nRect[s].y + CenterLoc.y - 20), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
							}
							if (m_Text_View[Cam_num] && !ROI_Mode && BOLT_Param[Cam_num].nOutput[s] != 3)
							{
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,1,CV_RGB(0,0,255),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,2,CV_RGB(255,0,0),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), CenterLoc,maxval,CV_RGB(0,0,255),1);
							}
						}
						else
						{
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s && BOLT_Param[Cam_num].nOutput[s] == 3)
							{
								msg.Format("%1.3f",(Outcircle_Info.size.width + Outcircle_Info.size.height)*0.25*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1]));
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + Outcircle_Info.center.x - 5,BOLT_Param[Cam_num].nRect[s].y + Outcircle_Info.center.y - 20), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s && BOLT_Param[Cam_num].nOutput[s] != 3)
							{
								msg.Format("No Inner Circle");
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
							}
						}
					}
					else
					{
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							msg.Format("No objects");
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
						}

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							msg.Format("No objects");
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::MATCH_RATE_TB) // MATCH_RATE_TB
				{
					// MATCH_RATE_TB
					dist_vec.push_back(MatchRate[Cam_num]);
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						msg.Format("%1.3f",MatchRate[Cam_num]);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(0,255,0), 1 + (double)Gray_Img[Cam_num].rows/960, 8);
					}

					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						msg.Format("%1.3f",MatchRate[Cam_num]);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(0,255,0), 1 + (double)Gray_Img[Cam_num].rows/960, 8);
					}
				}
			}
			else 
			{// SIDE /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::W_LENGTH_S) // Horizontal Length
				{
					int start_p = -1;
					int end_p = -1;
					int ii = 0;
					bool check_angle_line_init = false;
					for(int i=0;i<Out_binary.rows;i++)
					{
						ii = i;
						if (BOLT_Param[Cam_num].nCalAngle[s] != 0)
						{
							ii += (int)((double)Out_binary.cols*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
						}

						if (start_p == -1)
						{
							for(int j=0;j<Out_binary.cols;j++)
							{
								if (Out_binary.at<uchar>(i,j) == 255)
								{
									start_p = j;
									break;
								}
							}
						}
						if (ii >= 0 && ii < Out_binary.rows)
						{
							if (!check_angle_line_init)
							{
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
								//	//AfxMessageBox(msg);
								//	line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),2);
								//}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),1);
								}
								check_angle_line_init = true;
							}
							if (start_p != -1 && end_p == -1)
							{
								for(int j=Out_binary.cols-1;j>=0;j--)
								{
									ii = i;
									if (BOLT_Param[Cam_num].nCalAngle[s] != 0)
									{
										ii += (int)((double)(Out_binary.cols-start_p-((Out_binary.cols-1)-j))*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
									}
									if (ii >= 0 && ii < Out_binary.rows)
									{
										if (Out_binary.at<uchar>(ii,j) == 255)
										{
											end_p = j;
											break;
										}
									}
								}
							}
						}
						if (start_p != -1 && end_p != -1)
						{
							v_Dist = sqrt((double)(start_p - end_p)*(double)(start_p - end_p)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]
							+ (double)(i - ii)*(double)(i - ii)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

							if (v_Dist >= BOLT_Param[Cam_num].nCalMinDist[s] && v_Dist <= BOLT_Param[Cam_num].nCalMaxDist[s])
							{
								dist_vec.push_back(v_Dist);
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(start_p,i),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(end_p,ii),1,CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(start_p,i),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(end_p,ii),1,CV_RGB(255,0,0),1);
								}
							}
						}
						start_p = -1;end_p = -1;
					}
				} 
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::H_LENGTH_S) // Vertical Length
				{
					int start_p = -1;
					int end_p = -1;
					int jj = 0;
					bool check_angle_line_init = false;

					for(int j=0;j<Out_binary.cols;j++)
					{
						jj = j;
						if (BOLT_Param[Cam_num].nCalAngle[s] != 0)
						{
							jj += (int)((double)Out_binary.rows*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
						}

						if (start_p == -1)
						{
							for(int i=0;i<Out_binary.rows;i++)
							{
								if (Out_binary.at<uchar>(i,j) == 255)
								{
									start_p = i;
									break;
								}
							}
						}

						if (jj >= 0 && jj < Out_binary.cols)
						{
							if (!check_angle_line_init)
							{
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
								//	//AfxMessageBox(msg);
								//	line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),2);
								//}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),1);
								}
								check_angle_line_init = true;
							}
							if (start_p > -1 && end_p == -1)
							{
								for(int i=Out_binary.rows-1;i>=0;i--)
								{
									jj = j+(int)((double)(Out_binary.rows-start_p-((Out_binary.rows-1)-i))*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
									if (jj >= 0 && jj < Out_binary.cols)
									{
										if (Out_binary.at<uchar>(i,jj) == 255)
										{
											end_p = i;
											break;
										}
									}
								}
							}
						}
						if (start_p != -1 && end_p != -1)
						{
							v_Dist = sqrt((double)(start_p - end_p)*(double)(start_p - end_p)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]
							+ (double)(j - jj)*(double)(j - jj)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]);
							if (v_Dist >= BOLT_Param[Cam_num].nCalMinDist[s] && v_Dist <= BOLT_Param[Cam_num].nCalMaxDist[s])
							{
								dist_vec.push_back(v_Dist);
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,start_p),1,CV_RGB(255,0,0),1);
								//	circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(jj,end_p),1,CV_RGB(255,0,0),1);
								//}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,start_p),1,CV_RGB(255,0,0),1);
									circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(jj,end_p),1,CV_RGB(255,0,0),1);
								}
							}
						}
						start_p = -1;end_p = -1;
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::CENT_HT_S) // Concentricity
				{
					Mat t_binary = BOLT_Param[Cam_num].Thres_Obj_Img(tROI).clone();
					dilate(t_binary,t_binary,element,Point(-1,-1), 2);
					erode(t_binary,t_binary,element,Point(-1,-1), 2);
					//imwrite("00.bmp",t_binary);
					Mat stats, centroids, label;  
					Point2f rect_points[2];
					int numOfLables = connectedComponentsWithStats(t_binary, label,   
						stats, centroids, 8,CV_32S);
					if (numOfLables >= 2)
					{
						for (int j = 1; j < numOfLables; j++) 
						{
							//double x = centroids.at<double>(j, 0); //중심좌표
							//double y = centroids.at<double>(j, 1);

							double x = (double)stats.at<int>(j, CC_STAT_LEFT) + ((double)stats.at<int>(j, CC_STAT_WIDTH))/2; //중심좌표		
							double y = (double)stats.at<int>(j, CC_STAT_TOP) + ((double)stats.at<int>(j, CC_STAT_HEIGHT))/2;

							rect_points[j-1] = Point2f(x,y);
						}
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							line( Dst_Img[Cam_num], Point2f(rect_points[0].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[0].y + BOLT_Param[Cam_num].nRect[s].y) , 
								Point2f(rect_points[1].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[1].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[0].x,rect_points[0].y),2,CV_RGB(255,0,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[1].x,rect_points[1].y),2,CV_RGB(255,0,0),1);
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							line( Dst_Img[Cam_num], Point2f(rect_points[0].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[0].y + BOLT_Param[Cam_num].nRect[s].y) , 
								Point2f(rect_points[1].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[1].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[0].x,rect_points[0].y),2,CV_RGB(255,0,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[1].x,rect_points[1].y),2,CV_RGB(255,0,0),1);
						}
						dist_vec.push_back(abs((double)(rect_points[0].x - rect_points[1].x)*BOLT_Param[Cam_num].nResolution[0]));
					}
					//Mat t_binary = BOLT_Param[Cam_num].Thres_Obj_Img(tROI).clone();
					//Mat stats, centroids, label;  
					//int area, left, top, width, height = { 0, };
					//int m_min_object_num = -1; int m_min_object_value = 999999;
					//int m_max_object_num = -1; int m_max_object_value = 0;

					//Point2f rect_points[2];
					//int numOfLables = connectedComponentsWithStats(t_binary, label,   
					//	stats, centroids, 8,CV_32S);
					//if (numOfLables > 1)
					//{
					//	for (int j = 1; j < numOfLables; j++) 
					//	{
					//		top = stats.at<int>(j, CC_STAT_TOP);
					//		if (top <= m_min_object_value)
					//		{
					//			m_min_object_value = top;
					//			m_min_object_num = j;
					//		}
					//	}
					//	for (int j = 1; j < numOfLables; j++) 
					//	{
					//		area = stats.at<int>(j, CC_STAT_AREA);
					//		if (area >= m_max_object_value && m_min_object_num != j)
					//		{
					//			m_max_object_value = area;
					//			m_max_object_num = j;
					//		}
					//	}

					//	double x = centroids.at<double>(m_min_object_num, 0); //중심좌표
					//	double y = centroids.at<double>(m_min_object_num, 1);
					//	rect_points[0] = Point2f(x,y);

					//	x = centroids.at<double>(m_max_object_value, 0); //중심좌표
					//	y = centroids.at<double>(m_max_object_value, 1);
					//	rect_points[1] = Point2f(x,y);

					//	height = stats.at<int>(m_max_object_value, CC_STAT_HEIGHT);
					//	
					//	if (m_Text_View[Cam_num] && !ROI_Mode)
					//	{
					//		line( Dst_Img[Cam_num], Point2f(rect_points[0].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[0].y + BOLT_Param[Cam_num].nRect[s].y) , 
					//			Point2f(rect_points[1].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[1].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
					//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[0].x,rect_points[0].y),2,CV_RGB(255,0,0),1);
					//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[1].x,rect_points[1].y),2,CV_RGB(255,0,0),1);
					//	}
					//	if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					//	{
					//		line( Dst_Img[Cam_num], Point2f(rect_points[0].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[0].y + BOLT_Param[Cam_num].nRect[s].y) , 
					//			Point2f(rect_points[1].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[1].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
					//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[0].x,rect_points[0].y),2,CV_RGB(255,0,0),1);
					//		circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[1].x,rect_points[1].y),2,CV_RGB(255,0,0),1);
					//	}
					//	dist_vec.push_back((double)(rect_points[0].x - rect_points[1].x)*BOLT_Param[Cam_num].nResolution[0]);
					//}
				} 
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::ANGLE_BT_S) // Angle of Bottom
				{
					if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
						dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
					}

					Rect tt_TROI;int m_max_object_value = 0;int m_max_object_num = -1;
					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if (contours.size() > 0)
					{
						for( int k = 0; k < contours.size(); k++ )
						{  
							tt_TROI = boundingRect(contours[k]);
							if (m_max_object_value<= tt_TROI.height*tt_TROI.width)
							{
								m_max_object_value = tt_TROI.height*tt_TROI.width;
								m_max_object_num = k;
							}
						}
					}
					if (m_max_object_num >= 0)
					{
						tt_TROI = boundingRect(contours[m_max_object_num]);
						tt_TROI.height = tt_TROI.height - BOLT_Param[Cam_num].nHeightforShape[s];
						Out_binary(tt_TROI) -= 255;
					}

					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					m_max_object_num = -1;m_max_object_value = 0;
					if (contours.size() > 0)
					{
						for( int k = 0; k < contours.size(); k++ )
						{  
							tt_TROI = boundingRect(contours[k]);
							if (m_max_object_value<= tt_TROI.height*tt_TROI.width)
							{
								m_max_object_value = tt_TROI.height*tt_TROI.width;
								m_max_object_num = k;
							}
						}
					}
					Mat t_End_Nail = Mat::zeros(Out_binary.size(), CV_8UC1);
					if (m_max_object_num > -1)
					{
						tt_TROI = boundingRect(contours[m_max_object_num]);
						tt_TROI.x = tt_TROI.x;tt_TROI.width = tt_TROI.width;
						//tt_TROI.height--;
						t_End_Nail(tt_TROI) += 255;
						subtract(t_End_Nail, Out_binary,t_End_Nail);
						//t_End_Nail

					}

					dilate(t_End_Nail,t_End_Nail,element_v,Point(-1,-1),3);
					erode(t_End_Nail,t_End_Nail,element_v,Point(-1,-1),3);

					if (m_max_object_num > -1)
					{
						findContours( t_End_Nail.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						Point t_Left_Bottom(-1,-1);Point t_Left_Top(-1,-1);
						Point t_Right_Bottom(-1,-1);Point t_Right_Top(-1,-1);
						double Height_left_end_nail = 0;
						double Height_right_end_nail = 0;
						m_max_object_num = -1;m_max_object_value = 0;Rect tt_TROI;					
						int m_max_object_num2 = -1;int m_max_object_value2 = 0;					
						Rect tt_ROI0;
						Rect tt_ROI1;
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{  
								tt_TROI = boundingRect(contours[k]);
								if (m_max_object_value<= tt_TROI.height * tt_TROI.width)
								{
									m_max_object_value = tt_TROI.height * tt_TROI.width;
									m_max_object_num = k;
									tt_ROI0 = tt_TROI;
								}
							}
						}
						//msg.Format("%d,%d,%d,%d",tt_ROI0.x,tt_ROI0.y,tt_ROI0.width,tt_ROI0.height);
						//AfxMessageBox(msg);

						for( int k = 0; k < contours.size(); k++ )
						{  
							tt_TROI = boundingRect(contours[k]);
							if (m_max_object_value2<= tt_TROI.height * tt_TROI.width && k != m_max_object_num)
							{
								m_max_object_value2 = tt_TROI.height * tt_TROI.width;
								m_max_object_num2 = k;
								tt_ROI1 = tt_TROI;
							}
						}
						//msg.Format("%d,%d,%d,%d",tt_ROI1.x,tt_ROI1.y,tt_ROI1.width,tt_ROI1.height);
						//AfxMessageBox(msg);

						if (tt_ROI0.x <= tt_ROI1.x)
						{
							t_Left_Bottom.x = tt_ROI0.x;
							t_Left_Bottom.y = tt_ROI0.y;
							t_Left_Top.x = tt_ROI0.x+tt_ROI0.width;
							t_Left_Top.y = tt_ROI0.y+tt_ROI0.height;
							Height_left_end_nail = (double)tt_ROI0.height;
							t_Right_Bottom.x = tt_ROI1.x+tt_ROI1.width;
							t_Right_Bottom.y = tt_ROI1.y;
							t_Right_Top.x = tt_ROI1.x;
							t_Right_Top.y = tt_ROI1.y+tt_ROI1.height;
							Height_right_end_nail = (double)tt_ROI1.height;
						} else
						{
							t_Left_Bottom.x = tt_ROI1.x;
							t_Left_Bottom.y = tt_ROI1.y;
							t_Left_Top.x = tt_ROI1.x+tt_ROI1.width;
							t_Left_Top.y = tt_ROI1.y+tt_ROI1.height;
							Height_left_end_nail = (double)tt_ROI1.height;
							t_Right_Bottom.x = tt_ROI0.x+tt_ROI0.width;
							t_Right_Bottom.y = tt_ROI0.y;
							t_Right_Top.x = tt_ROI0.x;
							t_Right_Top.y = tt_ROI0.y+tt_ROI0.height;
							Height_right_end_nail = (double)tt_ROI0.height;
						}

						//Height_end_nail = max(Height_left_end_nail,Height_right_end_nail);
						if (t_Left_Bottom.x != -1 && t_Left_Top.x != -1)
						{
							dist_vec.push_back(abs(f_angle360(t_Left_Top,t_Left_Bottom)-90.0));
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Left_Bottom,t_Left_Top,CV_RGB(255,100,0),1);
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Left_Bottom,t_Left_Top,CV_RGB(255,100,0),1);
							}
						}
						if (t_Right_Bottom.x != -1 && t_Right_Top.x != -1)
						{
							dist_vec.push_back(abs(90.0 - f_angle360(t_Right_Top,t_Right_Bottom)));
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Right_Bottom,t_Right_Top,CV_RGB(255,100,0),1);
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Right_Bottom,t_Right_Top,CV_RGB(255,100,0),1);
							}
						}
					}
				} 
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::PITCH_COIN_S) // Pitch of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
							dist_vec.clear();
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size()-1;i++)
								{
									if (Pitch_Info[i+1].CY - Pitch_Info[i].CY > 0)
									{
										double t_R = (Pitch_Info[i+1].CY - Pitch_Info[i].CY)*BOLT_Param[Cam_num].nResolution[1];
										dist_vec.push_back(t_R);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											if (i == Pitch_Info.size()-2)
											{
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),1,CV_RGB(255,100,0),1);
											}
											msg.Format("P%2d(%1.2f)",dist_vec.size(),t_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{

						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.x > 1 && boundRect.x + boundRect.width < Morph_Out_binary.cols-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size_X);
							dist_vec.clear();
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size()-1;i++)
								{
									if (Pitch_Info[i+1].CX - Pitch_Info[i].CX > 0)
									{
										double t_R = (Pitch_Info[i+1].CX - Pitch_Info[i].CX)*BOLT_Param[Cam_num].nResolution[0];
										dist_vec.push_back(t_R);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											if (i == Pitch_Info.size()-2)
											{
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),1,CV_RGB(255,100,0),1);
											}
											msg.Format("P%2d(%1.2f)",dist_vec.size(),t_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::AREA_COIN_S) // Size of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						Rect boundRect;
						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{
							if (BOLT_Param[Cam_num].nThreadSizeMethod[s] < 3)
							{
								vector<Moments> mu(contours.size());

								vector<Point4D> Pitch_Info;
								Point4D t_Point4D;
								dist_vec.clear();
								for( int k = 0; k < contours.size(); k++ )
								{  
									mu[k] = moments( contours[k], false );
									boundRect = boundingRect( Mat(contours[k]) );

									if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-1 && mu[k].m00 > 0)
									{
										double t_R = 0;
										//if (boundRect.x > 1)
										{
											t_Point4D.IDX = (double)k;
											t_Point4D.CX = mu[k].m10/mu[k].m00;
											t_Point4D.CY = mu[k].m01/mu[k].m00;
											t_Point4D.AREA = mu[k].m00;
											if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
											{
												t_R = mu[k].m00;
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
											{
												t_R = (double)boundRect.width;
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
											{
												t_R = (double)boundRect.height;
											}
											dist_vec.push_back(t_R);
											t_Point4D.ROI = boundRect;
											Pitch_Info.push_back(t_Point4D);
										}

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
											msg.Format("S%2d(%1.2f)",dist_vec.size(),t_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
							else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 3)
							{//볼록BLOB 계산 옵션
								Mat Convex_Img = Out_binary.clone();
								vector<vector<Point>> hull1(contours.size());
								for(size_t k=0; k<contours.size(); k++)
								{	
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-1)
									{
										convexHull( Mat(contours[k]), hull1[k], false ); 
										//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
										drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
										}
									}
								}
								dilate(Out_binary,Out_binary,element,Point(-1,-1),1);
								subtract(Convex_Img,Out_binary,Convex_Img);
								//erode(Convex_Img,Convex_Img,element,Point(-1,-1),1);
								dilate(Convex_Img,Convex_Img,element,Point(-1,-1),1);
								Mat stats, centroids, label;  
								int numOfLables = connectedComponentsWithStats(Convex_Img, label,   
									stats, centroids, 8,CV_32S);
								int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
								if (numOfLables > 0)
								{
									for (int j = 1; j < numOfLables; j++) 
									{
										area = stats.at<int>(j, CC_STAT_AREA);
										t_w = stats.at<int>(j, CC_STAT_WIDTH);
										t_h = stats.at<int>(j, CC_STAT_HEIGHT);
										if (4 <= (double)area)
										{
											//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
											{
												dist_vec.push_back((double)area);
												if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
												{
													int x = centroids.at<double>(j, 0); //중심좌표
													int y = centroids.at<double>(j, 1);

													msg.Format("BLOB(%d)",area);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
													msg.Format("%d",area);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
												}
											}

											t_cnt++;
										}
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										//#pragma omp parallel for
										if (contours.size() > 0)
										{
											for( int k = 0; k < contours.size(); k++ )
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
												fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
											}
										}
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										//#pragma omp parallel for
										if (contours.size() > 0)
										{
											for( int k = 0; k < contours.size(); k++ )
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
												fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
											}
										}
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						Rect boundRect;
						if (contours.size() == 0)
						{
							dist_vec.push_back(0);
						}
						else
						{
							if (BOLT_Param[Cam_num].nThreadSizeMethod[s] < 3)
							{
								vector<Moments> mu(contours.size());

								vector<Point4D> Pitch_Info;
								Point4D t_Point4D;
								dist_vec.clear();
								for( int k = 0; k < contours.size(); k++ )
								{  
									mu[k] = moments( contours[k], false );
									boundRect = boundingRect( Mat(contours[k]) );

									if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-1 && mu[k].m00 > 0)
									{
										double t_R = 0;
										//if (boundRect.x > 1)
										{
											t_Point4D.IDX = (double)k;
											t_Point4D.CX = mu[k].m10/mu[k].m00;
											t_Point4D.CY = mu[k].m01/mu[k].m00;
											t_Point4D.AREA = mu[k].m00;
											if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
											{
												t_R = mu[k].m00;
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
											{
												t_R = (double)boundRect.width;
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
											{
												t_R = (double)boundRect.height;
											}
											dist_vec.push_back(t_R);
											t_Point4D.ROI = boundRect;
											Pitch_Info.push_back(t_Point4D);
										}

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
											msg.Format("S%2d(%1.2f)",dist_vec.size(),t_R);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
							else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 3)
							{//볼록BLOB 계산 옵션
								Mat Convex_Img = Out_binary.clone();
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-1)
									{
										convexHull( Mat(contours[k]), hull1[k], false ); 
										//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
										drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
										}
									}
								}
								dilate(Out_binary,Out_binary,element,Point(-1,-1),1);
								subtract(Convex_Img,Out_binary,Convex_Img);
								//erode(Convex_Img,Convex_Img,element,Point(-1,-1),1);
								dilate(Convex_Img,Convex_Img,element,Point(-1,-1),1);
								Mat stats, centroids, label;  
								int numOfLables = connectedComponentsWithStats(Convex_Img, label,   
									stats, centroids, 8,CV_32S);
								int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
								if (numOfLables > 0)
								{
									for (int j = 1; j < numOfLables; j++) 
									{
										area = stats.at<int>(j, CC_STAT_AREA);
										t_w = stats.at<int>(j, CC_STAT_WIDTH);
										t_h = stats.at<int>(j, CC_STAT_HEIGHT);
										if (4 <= (double)area)
										{
											//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
											{
												dist_vec.push_back((double)area);
												if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
												{
													int x = centroids.at<double>(j, 0); //중심좌표
													int y = centroids.at<double>(j, 1);

													msg.Format("BLOB(%d)",area);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
													msg.Format("%d",area);
													putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
												}
											}

											t_cnt++;
										}
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										//#pragma omp parallel for
										if (contours.size() > 0)
										{
											for( int k = 0; k < contours.size(); k++ )
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
												fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
											}
										}
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										//#pragma omp parallel for
										if (contours.size() > 0)
										{
											for( int k = 0; k < contours.size(); k++ )
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
												fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
											}
										}
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::LEADANGLE_COIN_S) // Lead Angle of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						//imwrite("00.bmp",Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() <= 1)
						{
							dist_vec.push_back(0);
						}
						else
						{
							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										if (Pitch_Info.size() > 0)
										{
											if (abs(Pitch_Info[Pitch_Info.size()-1].CX - t_Point4D.CX) > sqrt(max(Pitch_Info[Pitch_Info.size()-1].AREA,t_Point4D.AREA)))
											{
												Pitch_Info.push_back(t_Point4D);
											}
										}
										else
										{
											Pitch_Info.push_back(t_Point4D);
										}
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
							vector<double> vec_angle;
							//vector<int> vec_erase;
							if (Pitch_Info.size() > 0)
							{
								//for (int i = 0;i < Pitch_Info.size();i++)
								//{
								//	if (i < Pitch_Info.size()-1)
								//	{
								//		if (abs(Pitch_Info[i].CX - Pitch_Info[i+1].CX) <= sqrt(max(Pitch_Info[i].AREA,Pitch_Info[i+1].AREA)))
								//		{
								//			vec_erase.push_back(i+1);
								//		}
								//	}
								//}

								//if (vec_erase.size() > 0)
								//{
								//	for (int i = vec_erase.size()-1;i >= 0;i--)
								//	{
								//		Pitch_Info.erase(Pitch_Info.begin() + vec_erase[i]);
								//	}
								//}
								for (int i = 0;i < Pitch_Info.size()-1;i++)
								{
									if (i < Pitch_Info.size()-1 && Pitch_Info[i].CY > 0)
									{
										if (abs(Pitch_Info[i].CX - Pitch_Info[i+1].CX) > sqrt(max(Pitch_Info[i].AREA,Pitch_Info[i+1].AREA)))
										{
											float angle = (180.0 / CV_PI)*atan2(Pitch_Info[i+1].CY - Pitch_Info[i].CY, Pitch_Info[i+1].CX - Pitch_Info[i].CX);
											if (angle >= 90)
											{
												angle = 180 - angle;
											}
											vec_angle.push_back(angle);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
												//msg.Format("A%2d(%1.2f)",i+1,angle);
												//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*i), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											}
										}
									}
								}
							}
							if (vec_angle.size() > 0)
							{
								for (int i = 0;i < vec_angle.size()-1;i+=2)
								{
									double t_R = vec_angle[i]+vec_angle[i+1];
									dist_vec.push_back(t_R);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),t_R);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() <= 1)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.x > 1 && boundRect.x + boundRect.width < Morph_Out_binary.cols-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size_X);
							vector<double> vec_angle;
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size();i++)
								{
									if (i < Pitch_Info.size()-1 && Pitch_Info[i].CX > 0)
									{
										float angle = (180.0 / CV_PI)*atan2(Pitch_Info[i+1].CX - Pitch_Info[i].CX, Pitch_Info[i+1].CY - Pitch_Info[i].CY);
										if (angle >= 90)
										{
											angle = 180 - angle;
										}
										vec_angle.push_back(angle);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											//msg.Format("A%2d(%1.2f)",i+1,angle);
											//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*i), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
									}
								}
							}
							if (vec_angle.size() > 0)
							{
								for (int i = 0;i < vec_angle.size()-1;i+=2)
								{
									double t_R = vec_angle[i]+vec_angle[i+1];
									dist_vec.push_back(t_R);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),t_R);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::LEADANGLE_HARF_COIN_S) // Lead Angle of Screw Thread
				{
					//imwrite("00.bmp",Out_binary);
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() <= 1)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.y > 1 && boundRect.y + boundRect.height < Morph_Out_binary.rows-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							vector<double> vec_angle;
							if (Pitch_Info.size() > 0)
							{
								std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
								for (int i = 0;i < Pitch_Info.size();i++)
								{
									if (i < Pitch_Info.size()-1 && Pitch_Info[i].CY > 0)
									{
										if (abs(Pitch_Info[i].CY-Pitch_Info[i+1].CY) < (double)(Pitch_Info[i].ROI.height*5))
										{
											float angle = (180.0 / CV_PI)*atan2(Pitch_Info[i+1].CY - Pitch_Info[i].CY, Pitch_Info[i+1].CX - Pitch_Info[i].CX);
											if (angle >= 90)
											{
												angle = 180 - angle;
											}
											vec_angle.push_back(angle);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
												//msg.Format("A%2d(%1.2f)",i+1,angle);
												//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*i), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											}
										}
									}
								}
							}
							if (vec_angle.size() > 0)
							{
								for (int i = 0;i < vec_angle.size()-1;i++)
								{
									double t_R = vec_angle[i];
									dist_vec.push_back(t_R);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),t_R);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/8);
						subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
						if (BOLT_Param[Cam_num].nNASAPasteFilterSize[s] > 0)
						{
							erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
							dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nNASAPasteFilterSize[s]);
						}
						findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (contours.size() <= 1)
						{
							dist_vec.push_back(0);
						}
						else
						{

							vector<Moments> mu(contours.size());
							Rect boundRect;

							vector<Point4D> Pitch_Info;
							Point4D t_Point4D;

							for( int k = 0; k < contours.size(); k++ )
							{  
								mu[k] = moments( contours[k], false );
								boundRect = boundingRect( Mat(contours[k]) );

								if (boundRect.x > 1 && boundRect.x + boundRect.width < Morph_Out_binary.cols-2)
								{
									//if (boundRect.y > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							
							vector<double> vec_angle;
							if (Pitch_Info.size() > 0)
							{
								std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size_X);
								for (int i = 0;i < Pitch_Info.size();i++)
								{
									if (i < Pitch_Info.size()-1 && Pitch_Info[i].CX > 0)
									{
										if (abs(Pitch_Info[i].CX-Pitch_Info[i+1].CX) < (double)(Pitch_Info[i].ROI.width*5))
										{
											float angle = (180.0 / CV_PI)*atan2(Pitch_Info[i+1].CX - Pitch_Info[i].CX, Pitch_Info[i+1].CY - Pitch_Info[i].CY);
											if (angle >= 90)
											{
												angle = 180 - angle;
											}
											vec_angle.push_back(angle);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
												//msg.Format("A%2d(%1.2f)",i+1,angle);
												//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*i), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											}
										}
									}
								}
							}
							if (vec_angle.size() > 0)
							{
								for (int i = 0;i < vec_angle.size()-1;i++)
								{
									double t_R = vec_angle[i];
									dist_vec.push_back(t_R);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),t_R);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::BODY_WIDTH_S) // BODY_WIDTH
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						if (BOLT_Param[Cam_num].nBendingOutput[s] == 0)
						{// 몸통 두께
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 0)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							}

							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() != 0)
							{
								Rect boundRect;
								int t_max = 0; int t_max_idx = -1;
								for( int k = 0; k < contours.size(); k++ )
								{  
									boundRect = boundingRect( Mat(contours[k]) );
									if (t_max <= boundRect.height*boundRect.width
										&& boundRect.x > 1 && boundRect.x + boundRect.width < Out_binary.cols-2)
									{
										t_max = boundRect.height*boundRect.width;
										t_max_idx = k;
									}
								}

								if (t_max_idx >= 0)
								{
									boundRect = boundingRect( Mat(contours[t_max_idx]) );
									dist_vec.push_back((double)boundRect.width*BOLT_Param[Cam_num].nResolution[0]);
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										//msg.Format("W(%1.3f)",dist_vec[0]);
										//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										msg.Format("W(%1.3f)",dist_vec[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
						else if (BOLT_Param[Cam_num].nBendingOutput[s] == 1)
						{// 유효경
							J_Delete_Boundary(Out_binary,1);
							J_Fill_Hole(Out_binary);
							Mat Morph_T,Morph_B;
							erode(Out_binary,Morph_T,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_T,Morph_T,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							subtract(Out_binary,Morph_T,Morph_T);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							{
								erode(Morph_T,Morph_T,element_h,Point(-1,-1),5);
								dilate(Morph_T,Morph_T,element_h,Point(-1,-1),5);
							}
							else
							{
								erode(Morph_T,Morph_T,element_h,Point(-1,-1),2);
								dilate(Morph_T,Morph_T,element_h,Point(-1,-1),2);
							}


							//erode(Out_binary,Morph_B,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							//dilate(Morph_B,Morph_B,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							//subtract(Out_binary,Morph_B,Morph_B);
							//if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							//{
							//	erode(Morph_B,Morph_B,element_h,Point(-1,-1),5);
							//	dilate(Morph_B,Morph_B,element_h,Point(-1,-1),5);
							//}
							//else
							//{
							//	erode(Morph_B,Morph_B,element_h,Point(-1,-1),2);
							//	dilate(Morph_B,Morph_B,element_h,Point(-1,-1),2);
							//}

							dilate(Out_binary,Morph_B,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							erode(Morph_B,Morph_B,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							subtract(Morph_B, Out_binary, Morph_B);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							{
								erode(Morph_B,Morph_B,element_h,Point(-1,-1),5);
								dilate(Morph_B,Morph_B,element_h,Point(-1,-1),5);
							}
							else
							{
								erode(Morph_B,Morph_B,element_h,Point(-1,-1),2);
								dilate(Morph_B,Morph_B,element_h,Point(-1,-1),2);
							}

							Rect R_T(0,0,BOLT_Param[Cam_num].nRect[s].width/2,BOLT_Param[Cam_num].nRect[s].height);
							Rect R_B(BOLT_Param[Cam_num].nRect[s].width/2,0,BOLT_Param[Cam_num].nRect[s].width/2,BOLT_Param[Cam_num].nRect[s].height);

							Morph_T(R_B) -= 255;
							Morph_B(R_T) -= 255;

							//imwrite("00.bmp",Out_binary);
							//imwrite("01.bmp",Morph_T);
							//imwrite("02.bmp",Morph_B);

							Rect boundRect;
							vector<Point4D> T_Info, B_Info;
							Point4D t_Point4D;
							findContours( Morph_T.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//imwrite("123.bmp",Morph_Out_binary);
							if (contours.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.y > 2 && boundRect.y + boundRect.height < BOLT_Param[Cam_num].nRect[s].height-2 && boundRect.width >= 5 && boundRect.height >= BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4)
									{
										//boundRect.height /= 5;
										Mat stats, centroids, label;  
										int numOfLables = connectedComponentsWithStats(Morph_T(boundRect), label, stats, centroids, 8,CV_32S);
										if (numOfLables > 0)
										{
											int t_max = 0;int t_idx = -1;
											for (int j = 1; j < numOfLables; j++) 
											{
												int area = stats.at<int>(j, CC_STAT_AREA);
												if (t_max <= area)
												{
													t_max = area;t_idx = j;
												}
											}
											t_Point4D.AREA = (double)t_max;
											int x = centroids.at<double>(t_idx, 0); //중심좌표
											int y = centroids.at<double>(t_idx, 1);
											t_Point4D.CX = x + boundRect.x;
											t_Point4D.CY = y + boundRect.y;
											t_Point4D.ROI = boundRect;
											t_Point4D.IDX = k;
											T_Info.push_back(t_Point4D);
										}
									}	
									else
									{
										Morph_T(boundRect) -= 255;
									}
								}
							}

							findContours( Morph_B.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//imwrite("123.bmp",Morph_Out_binary);
							if (contours.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.y > 2 && boundRect.y + boundRect.height < BOLT_Param[Cam_num].nRect[s].height-2 && boundRect.width >= 5 && boundRect.height >= BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4)
									{
										//boundRect.height /= 5;
										Mat stats, centroids, label;  
										int numOfLables = connectedComponentsWithStats(Morph_B(boundRect), label, stats, centroids, 8,CV_32S);
										if (numOfLables > 0)
										{
											int t_max = 0;int t_idx = -1;
											for (int j = 1; j < numOfLables; j++) 
											{
												int area = stats.at<int>(j, CC_STAT_AREA);
												if (t_max <= area)
												{
													t_max = area;t_idx = j;
												}
											}
											t_Point4D.AREA = (double)t_max;
											int x = centroids.at<double>(t_idx, 0); //중심좌표
											int y = centroids.at<double>(t_idx, 1);
											t_Point4D.CX = x + boundRect.x;
											t_Point4D.CY = y + boundRect.y;
											t_Point4D.ROI = boundRect;
											t_Point4D.IDX = k;
											B_Info.push_back(t_Point4D);
										}
									}
									else
									{
										Morph_B(boundRect) -= 255;
									}
								}
							}

							std::sort(T_Info.begin(), T_Info.end(), Point_compare_Size);
							std::sort(B_Info.begin(), B_Info.end(), Point_compare_Size);

							if (T_Info.size() > 1 && B_Info.size() > 1)
							{
								if (abs(T_Info[0].CY-B_Info[0].CY) > T_Info[0].ROI.height/2)
								{
									if (T_Info[0].CY < B_Info[0].CY)
									{
										T_Info.erase(T_Info.begin() + 0);
									}
									else
									{
										B_Info.erase(B_Info.begin() + 0);
									}
								}

								int idx_min = min(T_Info.size(),B_Info.size());
								for (int i = 0;i < idx_min;i++)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(T_Info[i].CX,T_Info[i].CY),Point(B_Info[i].CX,B_Info[i].CY),CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(T_Info[i].CX,T_Info[i].CY),3,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(B_Info[i].CX,B_Info[i].CY),3,CV_RGB(255,0,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(T_Info[i].CX,T_Info[i].CY),Point(B_Info[i].CX,B_Info[i].CY),CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(T_Info[i].CX,T_Info[i].CY),3,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(B_Info[i].CX,B_Info[i].CY),3,CV_RGB(255,0,0),1);
									}
									double t_dist = sqrt(BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]*(T_Info[i].CX - B_Info[i].CX)*(T_Info[i].CX - B_Info[i].CX)
										+ BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]*(T_Info[i].CY - B_Info[i].CY)*(T_Info[i].CY - B_Info[i].CY));
									dist_vec.push_back(t_dist);
								}
								//imwrite("03.bmp",Dst_Img[Cam_num]);
							}

							// 우측기준으로
							dilate(Out_binary,Morph_T,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							erode(Morph_T,Morph_T,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							subtract(Morph_T, Out_binary, Morph_T);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							{
								erode(Morph_T,Morph_T,element_h,Point(-1,-1),5);
								dilate(Morph_T,Morph_T,element_h,Point(-1,-1),5);
							}
							else
							{
								erode(Morph_T,Morph_T,element_h,Point(-1,-1),2);
								dilate(Morph_T,Morph_T,element_h,Point(-1,-1),2);
							}

							erode(Out_binary,Morph_B,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_B,Morph_B,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							subtract(Out_binary, Morph_B, Morph_B);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							{
								erode(Morph_B,Morph_B,element_h,Point(-1,-1),5);
								dilate(Morph_B,Morph_B,element_h,Point(-1,-1),5);
							}
							else
							{
								erode(Morph_B,Morph_B,element_h,Point(-1,-1),2);
								dilate(Morph_B,Morph_B,element_h,Point(-1,-1),2);
							}

							Rect R_T1(0,0,BOLT_Param[Cam_num].nRect[s].width/2,BOLT_Param[Cam_num].nRect[s].height);
							Rect R_B1(BOLT_Param[Cam_num].nRect[s].width/2,0,BOLT_Param[Cam_num].nRect[s].width/2,BOLT_Param[Cam_num].nRect[s].height);

							Morph_T(R_B1) -= 255;
							Morph_B(R_T1) -= 255;

							//imwrite("00.bmp",Out_binary);
							//imwrite("01.bmp",Morph_T);
							//imwrite("02.bmp",Morph_B);

							T_Info.clear();B_Info.clear();
							findContours( Morph_T.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//imwrite("123.bmp",Morph_Out_binary);
							if (contours.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.y > 2 && boundRect.y + boundRect.height < BOLT_Param[Cam_num].nRect[s].height-2 && boundRect.width >= 5 && boundRect.height >= BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4)
									{
										//boundRect.height /= 5;
										Mat stats, centroids, label;  
										int numOfLables = connectedComponentsWithStats(Morph_T(boundRect), label, stats, centroids, 8,CV_32S);
										if (numOfLables > 0)
										{
											int t_max = 0;int t_idx = -1;
											for (int j = 1; j < numOfLables; j++) 
											{
												int area = stats.at<int>(j, CC_STAT_AREA);
												if (t_max <= area)
												{
													t_max = area;t_idx = j;
												}
											}
											t_Point4D.AREA = (double)t_max;
											int x = centroids.at<double>(t_idx, 0); //중심좌표
											int y = centroids.at<double>(t_idx, 1);
											t_Point4D.CX = x + boundRect.x;
											t_Point4D.CY = y + boundRect.y;
											t_Point4D.ROI = boundRect;
											t_Point4D.IDX = k;
											T_Info.push_back(t_Point4D);
										}
									}	
									else
									{
										Morph_T(boundRect) -= 255;
									}
								}
							}

							findContours( Morph_B.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//imwrite("123.bmp",Morph_Out_binary);
							if (contours.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.y > 2 && boundRect.y + boundRect.height < BOLT_Param[Cam_num].nRect[s].height-2 && boundRect.width >= 5 && boundRect.height >= BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4)
									{
										//boundRect.height /= 5;
										Mat stats, centroids, label;  
										int numOfLables = connectedComponentsWithStats(Morph_B(boundRect), label, stats, centroids, 8,CV_32S);
										if (numOfLables > 0)
										{
											int t_max = 0;int t_idx = -1;
											for (int j = 1; j < numOfLables; j++) 
											{
												int area = stats.at<int>(j, CC_STAT_AREA);
												if (t_max <= area)
												{
													t_max = area;t_idx = j;
												}
											}
											t_Point4D.AREA = (double)t_max;
											int x = centroids.at<double>(t_idx, 0); //중심좌표
											int y = centroids.at<double>(t_idx, 1);
											t_Point4D.CX = x + boundRect.x;
											t_Point4D.CY = y + boundRect.y;
											t_Point4D.ROI = boundRect;
											t_Point4D.IDX = k;
											B_Info.push_back(t_Point4D);
										}
									}
									else
									{
										Morph_B(boundRect) -= 255;
									}
								}
							}

							std::sort(T_Info.begin(), T_Info.end(), Point_compare_Size);
							std::sort(B_Info.begin(), B_Info.end(), Point_compare_Size);
							if (T_Info.size() > 1 && B_Info.size() > 1)
							{
								if (abs(T_Info[0].CY-B_Info[0].CY) > T_Info[0].ROI.height/2)
								{
									if (T_Info[0].CY < B_Info[0].CY)
									{
										T_Info.erase(T_Info.begin() + 0);
									}
									else
									{
										B_Info.erase(B_Info.begin() + 0);
									}
								}
								int idx_min = min(T_Info.size(),B_Info.size());
								for (int i = 0;i < idx_min;i++)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(T_Info[i].CX,T_Info[i].CY),Point(B_Info[i].CX,B_Info[i].CY),CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(T_Info[i].CX,T_Info[i].CY),3,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(B_Info[i].CX,B_Info[i].CY),3,CV_RGB(255,0,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(T_Info[i].CX,T_Info[i].CY),Point(B_Info[i].CX,B_Info[i].CY),CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(T_Info[i].CX,T_Info[i].CY),3,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(B_Info[i].CX,B_Info[i].CY),3,CV_RGB(255,0,0),1);
									}
									double t_dist = sqrt(BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]*(T_Info[i].CX - B_Info[i].CX)*(T_Info[i].CX - B_Info[i].CX)
										+ BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]*(T_Info[i].CY - B_Info[i].CY)*(T_Info[i].CY - B_Info[i].CY));
									dist_vec.push_back(t_dist);
								}
								//imwrite("03.bmp",Dst_Img[Cam_num]);
							}
						}
						else if (BOLT_Param[Cam_num].nBendingOutput[s] == 2)
						{// 평행도
							J_Delete_Boundary(Out_binary,1);
							J_Fill_Hole(Out_binary);
							Mat Morph_F_Img,Morph_R_Img;
							erode(Out_binary,Morph_F_Img,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_F_Img,Morph_F_Img,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_F_Img,Morph_R_Img,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4);
							erode(Morph_R_Img,Morph_R_Img,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4);

							subtract(Morph_R_Img,Morph_F_Img,Morph_F_Img);

							erode(Morph_F_Img,Morph_F_Img,element_h,Point(-1,-1),3);
							dilate(Morph_F_Img,Morph_F_Img,element_h,Point(-1,-1),3);
							J_Delete_Boundary(Morph_F_Img,1);
							findContours( Morph_F_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() != 0)
							{
								Rect boundRect;double t_v = 0;
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									//t_v = (contourArea(contours[k]) + 0.5);
									t_v = (double)boundRect.width*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(t_v);

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										boundRect.x -= 2;
										boundRect.y -= 2;
										boundRect.width += 4;
										boundRect.height += 4;

										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),boundRect,CV_RGB(255,0,0), 2, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
										msg.Format("%1.3f",t_v);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + boundRect.x,BOLT_Param[Cam_num].nRect[s].y + boundRect.y - 30), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,100,0), 2, 8);
									}
								}
							}
						}
						else if (BOLT_Param[Cam_num].nBendingOutput[s] == 3)
						{// 단높이
							J_Delete_Boundary(Out_binary,1);
							J_Fill_Hole(Out_binary);
							Mat Morph_Img;
							erode(Out_binary,Morph_Img,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_Img,Morph_Img,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							//imwrite("00.bmp",Morph_Img);
							Mat Edge_Img;
							erode(Morph_Img,Edge_Img,element_h,Point(-1,-1),1);
							subtract(Morph_Img,Edge_Img,Edge_Img);
							erode(Edge_Img,Edge_Img,element_v,Point(-1,-1),2);
							dilate(Edge_Img,Edge_Img,element,Point(-1,-1),2);
							//imwrite("00.bmp",Edge_Img);

							findContours( Edge_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() != 0)
							{
								Rect boundRect;
								int t_max = 0; int t_max_idx = -1;
								for( int k = 0; k < contours.size(); k++ )
								{  
									boundRect = boundingRect( Mat(contours[k]) );
									if (t_max <= boundRect.height*boundRect.width
										&& boundRect.y > 1 && boundRect.y + boundRect.height < Out_binary.rows-2)
									{
										t_max = boundRect.height*boundRect.width;
										t_max_idx = k;
									}
								}

								if (t_max_idx >= 0)
								{
									boundRect = boundingRect( Mat(contours[t_max_idx]) );
									dist_vec.push_back((double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),2);
										msg.Format("%1.3f",(double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + boundRect.x + boundRect.width+5,BOLT_Param[Cam_num].nRect[s].y + boundRect.y + boundRect.height/2), FONT_HERSHEY_SIMPLEX, 0.7, CV_RGB(255,100,0), 2, 8);
									}

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),2);
										msg.Format("%1.3f",(double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + boundRect.x + boundRect.width+5,BOLT_Param[Cam_num].nRect[s].y + boundRect.y + boundRect.height/2), FONT_HERSHEY_SIMPLEX, 0.7, CV_RGB(255,100,0), 2, 8);
									}
								}
								int t_max_idx2 = -1;t_max = 0;
								for( int k = 0; k < contours.size(); k++ )
								{  
									if (t_max_idx != k)
									{
										boundRect = boundingRect( Mat(contours[k]) );
										if (t_max <= boundRect.height*boundRect.width
											&& boundRect.y > 1 && boundRect.y + boundRect.height < Out_binary.rows-2)
										{
											t_max = boundRect.height*boundRect.width;
											t_max_idx2 = k;
										}
									}
								}
								if (t_max_idx2 >= 0)
								{
									boundRect = boundingRect( Mat(contours[t_max_idx2]) );
									dist_vec.push_back((double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),2);
										msg.Format("%1.3f",(double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + boundRect.x + boundRect.width+5,BOLT_Param[Cam_num].nRect[s].y + boundRect.y + boundRect.height/2), FONT_HERSHEY_SIMPLEX, 0.7, CV_RGB(255,100,0), 2, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),2);
										msg.Format("%1.3f",(double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + boundRect.x + boundRect.width+5,BOLT_Param[Cam_num].nRect[s].y + boundRect.y + boundRect.height/2), FONT_HERSHEY_SIMPLEX, 0.7, CV_RGB(255,100,0), 2, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						if (BOLT_Param[Cam_num].nBendingOutput[s] == 0)
						{// 몸통 두께
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 0)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							}

							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() != 0)
							{
								Rect boundRect;
								int t_max = 0; int t_max_idx = -1;
								for( int k = 0; k < contours.size(); k++ )
								{  
									boundRect = boundingRect( Mat(contours[k]) );
									if (t_max <= boundRect.height*boundRect.width
										&& boundRect.y > 1 && boundRect.y + boundRect.height < Out_binary.rows-2)
									{
										t_max = boundRect.height*boundRect.width;
										t_max_idx = k;
									}
								}

								if (t_max_idx >= 0)
								{
									boundRect = boundingRect( Mat(contours[t_max_idx]) );
									dist_vec.push_back((double)boundRect.height*BOLT_Param[Cam_num].nResolution[1]);
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										//msg.Format("W(%1.3f)",dist_vec[0]);
										//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										msg.Format("W(%1.3f)",dist_vec[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
						else if (BOLT_Param[Cam_num].nBendingOutput[s] == 1)
						{// 유효경
							J_Delete_Boundary(Out_binary,1);
							J_Fill_Hole(Out_binary);
							Mat Morph_T,Morph_B;
							erode(Out_binary,Morph_T,element_h,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_T,Morph_T,element_h,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							subtract(Out_binary,Morph_T,Morph_T);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							{
								erode(Morph_T,Morph_T,element_v,Point(-1,-1),5);
								dilate(Morph_T,Morph_T,element_v,Point(-1,-1),5);
							}
							else
							{
								erode(Morph_T,Morph_T,element_v,Point(-1,-1),2);
								dilate(Morph_T,Morph_T,element_v,Point(-1,-1),2);
							}


							erode(Out_binary,Morph_B,element_h,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Morph_B,Morph_B,element_h,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							subtract(Out_binary,Morph_B,Morph_B);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 20)
							{
								erode(Morph_B,Morph_B,element_v,Point(-1,-1),5);
								dilate(Morph_B,Morph_B,element_v,Point(-1,-1),5);
							}
							else
							{
								erode(Morph_B,Morph_B,element_v,Point(-1,-1),2);
								dilate(Morph_B,Morph_B,element_v,Point(-1,-1),2);
							}

							Rect R_T(0,0,BOLT_Param[Cam_num].nRect[s].width,BOLT_Param[Cam_num].nRect[s].height/2);
							Rect R_B(0,BOLT_Param[Cam_num].nRect[s].height/2,BOLT_Param[Cam_num].nRect[s].width,BOLT_Param[Cam_num].nRect[s].height/2);

							Morph_T(R_B) -= 255;
							Morph_B(R_T) -= 255;

							//imwrite("00.bmp",Out_binary);
							//imwrite("01.bmp",Morph_T);
							//imwrite("02.bmp",Morph_B);

							Rect boundRect;
							vector<Point4D> T_Info, B_Info;
							Point4D t_Point4D;
							findContours( Morph_T.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//imwrite("123.bmp",Morph_Out_binary);
							if (contours.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.x > 2 && boundRect.x + boundRect.width < BOLT_Param[Cam_num].nRect[s].width-2 && boundRect.width >= BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4 && boundRect.height >= 5)
									{
										//boundRect.height /= 5;
										Mat stats, centroids, label;  
										int numOfLables = connectedComponentsWithStats(Morph_T(boundRect), label, stats, centroids, 8,CV_32S);
										if (numOfLables > 0)
										{
											int t_max = 0;int t_idx = -1;
											for (int j = 1; j < numOfLables; j++) 
											{
												int area = stats.at<int>(j, CC_STAT_AREA);
												if (t_max <= area)
												{
													t_max = area;t_idx = j;
												}
											}
											t_Point4D.AREA = (double)t_max;
											int x = centroids.at<double>(t_idx, 0); //중심좌표
											int y = centroids.at<double>(t_idx, 1);
											t_Point4D.CX = x + boundRect.x;
											t_Point4D.CY = y + boundRect.y;
											t_Point4D.ROI = boundRect;
											t_Point4D.IDX = k;
											T_Info.push_back(t_Point4D);
										}
									}	
									else
									{
										Morph_T(boundRect) -= 255;
									}
								}
							}

							findContours( Morph_B.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//imwrite("123.bmp",Morph_Out_binary);
							if (contours.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									boundRect = boundingRect( Mat(contours[k]) );
									if (boundRect.x > 2 && boundRect.x + boundRect.width < BOLT_Param[Cam_num].nRect[s].width-2 && boundRect.width >= BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4 && boundRect.height >= 5)
									{
										//boundRect.height /= 5;
										Mat stats, centroids, label;  
										int numOfLables = connectedComponentsWithStats(Morph_B(boundRect), label, stats, centroids, 8,CV_32S);
										if (numOfLables > 0)
										{
											int t_max = 0;int t_idx = -1;
											for (int j = 1; j < numOfLables; j++) 
											{
												int area = stats.at<int>(j, CC_STAT_AREA);
												if (t_max <= area)
												{
													t_max = area;t_idx = j;
												}
											}
											t_Point4D.AREA = (double)t_max;
											int x = centroids.at<double>(t_idx, 0); //중심좌표
											int y = centroids.at<double>(t_idx, 1);
											t_Point4D.CX = x + boundRect.x;
											t_Point4D.CY = y + boundRect.y;
											t_Point4D.ROI = boundRect;
											t_Point4D.IDX = k;
											B_Info.push_back(t_Point4D);
										}
									}
									else
									{
										Morph_B(boundRect) -= 255;
									}
								}
							}

							std::sort(T_Info.begin(), T_Info.end(), Point_compare_Size_X);
							std::sort(B_Info.begin(), B_Info.end(), Point_compare_Size_X);
							if (T_Info.size() > 1 && B_Info.size() > 1)
							{
								int idx_min = min(T_Info.size(),B_Info.size());
								for (int i = 0;i < idx_min;i++)
								{
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(T_Info[i].CX,T_Info[i].CY),Point(B_Info[i].CX,B_Info[i].CY),CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(T_Info[i].CX,T_Info[i].CY),3,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(B_Info[i].CX,B_Info[i].CY),3,CV_RGB(255,0,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(T_Info[i].CX,T_Info[i].CY),Point(B_Info[i].CX,B_Info[i].CY),CV_RGB(255,255,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(T_Info[i].CX,T_Info[i].CY),3,CV_RGB(255,0,0),1);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(B_Info[i].CX,B_Info[i].CY),3,CV_RGB(255,0,0),1);
									}
									double t_dist = sqrt(BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0]*(T_Info[i].CX - B_Info[i].CX)*(T_Info[i].CX - B_Info[i].CX)
										+ BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]*(T_Info[i].CY - B_Info[i].CY)*(T_Info[i].CY - B_Info[i].CY));
									dist_vec.push_back(t_dist);
								}
								//imwrite("03.bmp",Dst_Img[Cam_num]);
							}
						}
						else if (BOLT_Param[Cam_num].nBendingOutput[s] == 2)
						{// 평행도


						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::BODY_BENDING_S) // BODY_BENDING
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						J_Delete_Boundary(Out_binary,1);
						J_Fill_Hole(Out_binary);

						double t_Left_x = 0;double t_Left_y = 0;
						double t_Right_x = 0;double t_Right_y = 0;

						if (BOLT_Param[Cam_num].nBendingOutput[s] >= 0 && BOLT_Param[Cam_num].nBendingOutput[s] <= 2)
						{
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 0)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							}
							//imwrite("00.bmp",Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() != 0)
							{
								Rect boundRect;
								int t_max = 0; int t_max_idx = -1;
								for( int k = 0; k < contours.size(); k++ )
								{  
									boundRect = boundingRect( Mat(contours[k]) );
									if (t_max <= boundRect.height*boundRect.width
										&& boundRect.x > 1 && boundRect.x + boundRect.width < Out_binary.cols-2)
									{
										t_max = boundRect.height*boundRect.width;
										t_max_idx = k;
									}
								}
								if (t_max_idx > -1)
								{
									Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
									Scalar color(255);
									drawContours( Out_binary, contours, t_max_idx, color, CV_FILLED, 8, hierarchy );
								}
								else
								{
									t_max_idx = 0;
								}
								boundRect = boundingRect( Mat(contours[t_max_idx]) );

								if (BOLT_Param[Cam_num].nBendingOutput[s] == 2)
								{
									// Top 좌,우 찾기
									double t_cnt = 0;
									int t_Search_Range = 10;
									for (int i = boundRect.x + boundRect.width/3;i<boundRect.x + boundRect.width/3 + t_Search_Range;i++)
									{
										for (int j = boundRect.y-1;j<Out_binary.rows;j++)
										{
											if (Out_binary.at<uchar>(j,i) > 0)
											{
												t_Left_x +=(double)i;
												t_Left_y += (double)j;
												t_cnt++;
												break;
											}
										}
									}
									if (t_cnt > 0)
									{
										t_Left_x/=t_cnt;
										t_Left_y/=t_cnt;
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_Left_x,t_Left_y),0,CV_RGB(255,255,0),1);
										}
									}
									else
									{
										t_Left_y = Out_binary.rows -1;
									}

									t_cnt = 0;
									for (int i = boundRect.x + 2*boundRect.width/3;i<boundRect.x + 2*boundRect.width/3 + t_Search_Range;i++)
									{
										for (int j = boundRect.y-1;j<Out_binary.rows;j++)
										{
											if (Out_binary.at<uchar>(j,i) > 0)
											{
												t_Right_x +=(double)i;
												t_Right_y += (double)j;
												t_cnt++;
												break;
											}
										}
									}
									if (t_cnt > 0)
									{
										t_Right_x/=t_cnt;
										t_Right_y/=t_cnt;
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(t_Right_x,t_Right_y),0,CV_RGB(255,255,0),1);
										}
									}
									else
									{
										t_Right_x = Out_binary.cols -1;
										t_Right_y = Out_binary.rows -1;
									}
								}// Top 좌,우 찾기 끝


								vector<Point2f> vec_center;
								double t_center = 0;double t_cnt = 0;
								for (int j=0;j < Out_binary.rows;j++)
								{
									if (BOLT_Param[Cam_num].nBendingOutput[s] == 1)
									{//Body 각도
										if (BOLT_Param[Cam_num].nRect[s].y + j >= BOLT_Param[Cam_num].nRect[0].y + min(BOLT_Param[Cam_num].nSideBottomLeft.y,BOLT_Param[Cam_num].nSideBottomRight.y) - 5)
										{
											break;
										}
									}

									if (BOLT_Param[Cam_num].nBendingOutput[s] == 2)
									{//Top & Body 각도
										if (BOLT_Param[Cam_num].nRect[s].y + j <= BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/4)
										{
											continue;
										}
									}

									t_center = 0;t_cnt = 0;
									for (int i = 0; i < Out_binary.cols;i++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_center += (double)i;
											t_cnt++;
										}
									}
									if (t_cnt > 0)
									{
										vec_center.push_back(Point2f(t_center/t_cnt,(double)j));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f((t_center/t_cnt),j),0,CV_RGB(255,255,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f((t_center/t_cnt),j),0,CV_RGB(255,255,0),1);
										}
									}
								}

								Vec4f lines;
								vector<Point2f> vec_2center; // 추가 2017.04.13
								if (vec_center.size() > 6)
								{
									float t_x0 = (vec_center[0].x + vec_center[1].x+vec_center[2].x)/3.0f;// 추가 2017.04.13
									float t_y0 = (vec_center[0].y + vec_center[1].y+vec_center[2].y)/3.0f;// 추가 2017.04.13
									float t_x1 = (vec_center[vec_center.size()-1].x + vec_center[vec_center.size()-2].x+vec_center[vec_center.size()-3].x)/3.0f;// 추가 2017.04.13
									float t_y1 = (vec_center[vec_center.size()-1].y + vec_center[vec_center.size()-2].y+vec_center[vec_center.size()-3].y)/3.0f;// 추가 2017.04.13
									vec_2center.push_back(Point2f(t_x0,t_y0));// 추가 2017.04.13
									vec_2center.push_back(Point2f(t_x1,t_y1));// 추가 2017.04.13
									//fitLine(vec_center,lines, CV_DIST_L2,0,0.01,0.01);
									fitLine(vec_2center,lines, CV_DIST_L2,0,0.01,0.01);
									//float lefty = (-lines[2]*lines[1]/lines[0])+lines[3];
									//float righty = ((Out_binary.cols-lines[2])*lines[1]/lines[0])+lines[3];
									//float lefty = t_y0;
									//float righty = t_y1;
									if (BOLT_Param[Cam_num].nBendingOutput[s] == 0)
									{//거리
										double v_distnace = 0;
										for (int i=0;i<vec_center.size();i++)
										{
											v_distnace = abs(lines[1]*vec_center[i].x - lines[0]*vec_center[i].y + lines[0]*lines[3] - lines[1]*lines[2])
												/sqrt(lines[0]*lines[0] + lines[1]*lines[1])*BOLT_Param[Cam_num].nResolution[0];
											dist_vec.push_back(v_distnace);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,0,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,0,0),1);
										}
									}
									else if (BOLT_Param[Cam_num].nBendingOutput[s] == 1)
									{//Body 각도
										Point2f P_A(t_x1 - t_x0,t_y1 - t_y0);
										Point2f P_B(BOLT_Param[Cam_num].nSideBottomRight.x - BOLT_Param[Cam_num].nSideBottomLeft.x,BOLT_Param[Cam_num].nSideBottomRight.y - BOLT_Param[Cam_num].nSideBottomLeft.y);
										float t_angle = acos((P_A.x*P_B.x + P_A.y*P_B.y)/(sqrt(P_A.x*P_A.x + P_A.y*P_A.y)*sqrt(P_B.x*P_B.x + P_B.y*P_B.y))) * 180.0 / PI;
										dist_vec.push_back(t_angle);

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,255,0),2);
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),BOLT_Param[Cam_num].nSideBottomLeft,BOLT_Param[Cam_num].nSideBottomRight,CV_RGB(255,255,0),2);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,255,0),2);
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),BOLT_Param[Cam_num].nSideBottomLeft,BOLT_Param[Cam_num].nSideBottomRight,CV_RGB(255,255,0),2);
										}
									}
									else if (BOLT_Param[Cam_num].nBendingOutput[s] == 2)
									{//Top, Body 각도
										Point2f P_A(t_x1 - t_x0,t_y1 - t_y0);
										Point2f P_B(t_Right_x - t_Left_x,t_Right_y - t_Left_y);
										float t_angle = acos((P_A.x*P_B.x + P_A.y*P_B.y)/(sqrt(P_A.x*P_A.x + P_A.y*P_A.y)*sqrt(P_B.x*P_B.x + P_B.y*P_B.y))) * 180.0 / PI;
										dist_vec.push_back(t_angle);

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,255,0),2);
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_Left_x,t_Left_y),Point2f(t_Right_x,t_Right_y),CV_RGB(255,255,0),2);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,255,0),2);
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_Left_x,t_Left_y),Point2f(t_Right_x,t_Right_y),CV_RGB(255,255,0),2);
										}
									}
								}
							}
						}
						else if (BOLT_Param[Cam_num].nBendingOutput[s] == 3)
						{// 흔들림
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							Mat Temp_Out_binary = Out_binary.clone();
							if (contours.size() != 0)
							{
								Rect boundRect;
								int t_max = 0; int t_max_idx = -1;
								for( int k = 0; k < contours.size(); k++ )
								{  
									boundRect = boundingRect( Mat(contours[k]) );
									if (t_max <= boundRect.height*boundRect.width
										&& boundRect.x > 1 && boundRect.x + boundRect.width < Out_binary.cols-2)
									{
										t_max = boundRect.height*boundRect.width;
										t_max_idx = k;
									}
								}
								if (t_max_idx > -1)
								{
									Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
									Scalar color(255);
									drawContours( Out_binary, contours, t_max_idx, color, CV_FILLED, 8, hierarchy );
								}
								else
								{
									t_max_idx = 0;
								}
								boundRect = boundingRect( Mat(contours[t_max_idx]) );

								int left = boundRect.x;
								int top = boundRect.y;
								int width = boundRect.width;
								int height = boundRect.height;
								Rect t_Rect(left,top,width, 3*height/4);

								//imwrite("00.bmp",Out_binary);
								if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 0)
								{
									erode(Out_binary(t_Rect),Out_binary(t_Rect),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
									dilate(Out_binary(t_Rect),Out_binary(t_Rect),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s] + BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4);
									erode(Out_binary(t_Rect),Out_binary(t_Rect),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]/4);
								}

								//imwrite("01.bmp",Out_binary);

								double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;double t_angle = 0;
								Vec4f lines;Point2f t_Top_Center(0,0);
								vector<Point2f> vec_2center;

								for (int j = top+height/4;j<=top+2*height/3;j++)
								{
									t_B_Top_x = t_B_Top_y = t_Top_cnt = 0;
									for (int i = left;i<left+width;i++)
									{
										if (Out_binary.at<uchar>(j,i) > 0)
										{
											t_B_Top_x +=(double)i;
											t_B_Top_y += (double)j;
											t_Top_cnt++;
										}
									}
									if (t_Top_cnt > 0)
									{
										t_B_Top_x/=t_Top_cnt;
										t_B_Top_y/=t_Top_cnt;
										t_Top_Center.x = t_B_Top_x;
										t_Top_Center.y = t_B_Top_y;
										vec_2center.push_back(t_Top_Center);
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Top_Center,0,CV_RGB(255,255,0),1);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), t_Top_Center,0,CV_RGB(255,255,0),1);
										}
									}
								}
								if (vec_2center.size() > 2 )
								{
									fitLine(vec_2center,lines, CV_DIST_L2,0,0.001,0.001);
									t_angle = (180.0/CV_PI)*atan(lines[1]/lines[0]);
									if (t_angle < 0)
									{
										t_angle += 90;
									}
									else if (t_angle > 0)
									{
										t_angle -= 90;
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("%1.3f",t_angle);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + vec_2center[vec_2center.size()-1].x - 40,BOLT_Param[Cam_num].nRect[s].y +  vec_2center[vec_2center.size()-1].y + 25), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,100,0), 2, 8);
									}

									//msg.Format("%1.3f",t_angle-90);
									//AfxMessageBox(msg);
								}

								int t_min_x = 9999;
								int t_max_x = 0;
								for (int j = top+height/4;j<=top+2*height/3;j++)
								{
									for (int i = left;i<left+width;i++)
									{
										if (Temp_Out_binary.at<uchar>(j,i) > 0)
										{
											if (t_min_x >i)
											{
												t_min_x = i;
											}
											break;
										}
									}
									for (int i = left+width -1;i>=left;i--)
									{
										if (Temp_Out_binary.at<uchar>(j,i) > 0)
										{
											if (t_max_x < i)
											{
												t_max_x = i;
											}
											break;
										}
									}
								}

								Vec4f Left_lines;Point2f Left_point(0,0);
								vector<Point2f> vec_left_center;double Left_angle = 0;
								Vec4f Right_lines;Point2f Right_point(0,0);
								vector<Point2f> vec_right_center;double Right_angle = 0;
								if (t_min_x != 9999 && t_max_x != 0)
								{
									//msg.Format("%d,%d",left+width,t_max_x);
									//AfxMessageBox(msg);
									for (int j = top+2*height/3;j<=top+height;j++)
									{
										for (int i = left+1;i<t_min_x-5;i++)
										{
											if (Temp_Out_binary.at<uchar>(j,i) > 0)
											{
												Left_point.x = i;Left_point.y = j;
												vec_left_center.push_back(Left_point);
												if (BOLT_Param[Cam_num].nOutput[s] == 0 || BOLT_Param[Cam_num].nOutput[s] == 1)
												{
													if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
													{
														circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Left_point,0,CV_RGB(255,255,0),2);
													}
													if (m_Text_View[Cam_num] && !ROI_Mode)
													{
														circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Left_point,0,CV_RGB(255,255,0),2);
													}
												}
												break;
											}
										}
										for (int i = left+width-2;i >= t_max_x + 5 ;i--)
										{
											if (Temp_Out_binary.at<uchar>(j,i) > 0)
											{
												Right_point.x = i;Right_point.y = j;
												vec_right_center.push_back(Right_point);
												if (BOLT_Param[Cam_num].nOutput[s] == 0 || BOLT_Param[Cam_num].nOutput[s] == 2)
												{
													if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
													{
														circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Right_point,0,CV_RGB(255,255,0),2);
													}	
													if (m_Text_View[Cam_num] && !ROI_Mode)
													{
														circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Right_point,0,CV_RGB(255,255,0),2);
													}
												}
												break;
											}
										}
									}
								}

								if (vec_left_center.size() > 2 )
								{
									fitLine(vec_left_center,Left_lines, CV_DIST_L2,0,0.001,0.001);
									Left_angle = (180.0/CV_PI)*atan(Left_lines[1]/Left_lines[0]) - t_angle;
									if (Left_angle < 0)
									{
										Left_angle += 90;
									}
									else if (Left_angle > 0)
									{
										Left_angle = 90 - Left_angle;
									}
									if (BOLT_Param[Cam_num].nOutput[s] == 0 || BOLT_Param[Cam_num].nOutput[s] == 1)
									{
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											msg.Format("%1.3f",Left_angle);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + vec_left_center[vec_left_center.size()/2].x + 10,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,100,0), 2, 8);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											msg.Format("%1.3f",Left_angle);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + vec_left_center[vec_left_center.size()/2].x + 10,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,100,0), 2, 8);
										}
									}
									//msg.Format("%1.3f",Left_angle);
									//AfxMessageBox(msg);
								}

								if (vec_right_center.size() > 2 )
								{
									fitLine(vec_right_center,Right_lines, CV_DIST_L2,0,0.001,0.001);
									Right_angle = (180.0/CV_PI)*atan(Right_lines[1]/Right_lines[0]) + t_angle;
									if (Right_angle < 0)
									{
										Right_angle += 90;
									}
									else if (Right_angle > 0)
									{
										Right_angle = 90 - Right_angle;
									}
									if (BOLT_Param[Cam_num].nOutput[s] == 0 || BOLT_Param[Cam_num].nOutput[s] == 2)
									{
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											msg.Format("%1.3f",Right_angle);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + vec_right_center[vec_right_center.size()/2].x - 100,BOLT_Param[Cam_num].nRect[s].y +  vec_right_center[vec_right_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,100,0), 2, 8);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											msg.Format("%1.3f",Right_angle);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + vec_right_center[vec_right_center.size()/2].x - 100,BOLT_Param[Cam_num].nRect[s].y +  vec_right_center[vec_right_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,100,0), 2, 8);
										}
									}
									//msg.Format("%1.3f",Right_angle);
									//AfxMessageBox(msg);
								}

								if (BOLT_Param[Cam_num].nOutput[s] == 0)
								{
									dist_vec.push_back(abs(Right_angle - Left_angle));
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("D:%1.3f",abs(Right_angle - Left_angle));
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x +left+width/2 - 50,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,255,0), 2, 8);
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										msg.Format("D:%1.3f",abs(Right_angle - Left_angle));
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x +left+width/2 - 50,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,255,0), 2, 8);
									}
								}
								else if (BOLT_Param[Cam_num].nOutput[s] == 1)
								{
									dist_vec.push_back(abs(Left_angle));
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("L:%1.3f",abs(Left_angle));
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x +left+width/2 - 50,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,255,0), 2, 8);
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										msg.Format("L:%1.3f",abs(Left_angle));
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x +left+width/2 - 50,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,255,0), 2, 8);
									}
								}
								else if (BOLT_Param[Cam_num].nOutput[s] == 2)
								{
									dist_vec.push_back(abs(Right_angle));									
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("R:%1.3f",abs(Right_angle));
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x +left+width/2 - 50,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,255,0), 2, 8);
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										msg.Format("R:%1.3f",abs(Right_angle));
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x +left+width/2 - 50,BOLT_Param[Cam_num].nRect[s].y +  vec_left_center[vec_left_center.size()/2].y), FONT_HERSHEY_SIMPLEX, 0.8, CV_RGB(255,255,0), 2, 8);
									}


								}


							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{

						if (BOLT_Param[Cam_num].nAngleHeightFilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
							dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[s]);
						}
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						if (contours.size() != 0)
						{
							Rect boundRect;
							int t_max = 0; int t_max_idx = -1;
							for( int k = 0; k < contours.size(); k++ )
							{  
								boundRect = boundingRect( Mat(contours[k]) );
								if (t_max <= boundRect.height*boundRect.width
									&& boundRect.y > 1 && boundRect.y + boundRect.height < Out_binary.rows-2)
								{
									t_max = boundRect.height*boundRect.width;
									t_max_idx = k;
								}
							}
							if (t_max_idx > -1)
							{
								Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
								Scalar color(255);
								drawContours( Out_binary, contours, t_max_idx, color, CV_FILLED, 8, hierarchy );
							}
							vector<Point2f> vec_center;
							double t_center = 0;double t_cnt = 0;
							for (int j=0;j < Out_binary.cols;j++)
							{
								t_center = 0;t_cnt = 0;
								for (int i = 0; i < Out_binary.rows;i++)
								{
									if (Out_binary.at<uchar>(i,j) > 0)
									{
										t_center += (double)i;
										t_cnt++;
									}
								}
								if (t_cnt > 0)
								{
									vec_center.push_back(Point2f((double)j,t_center/t_cnt));
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f((double)j,(t_center/t_cnt)),0,CV_RGB(255,255,0),1);
									}
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f((double)j,(t_center/t_cnt)),0,CV_RGB(255,255,0),1);
									}
								}
							}

							Vec4f lines;
							vector<Point2f> vec_2center; // 추가 2017.04.13
							if (vec_center.size() > 6)
							{
								float t_x0 = (vec_center[0].x + vec_center[1].x+vec_center[2].x)/3.0f;// 추가 2017.04.13
								float t_y0 = (vec_center[0].y + vec_center[1].y+vec_center[2].y)/3.0f;// 추가 2017.04.13
								float t_x1 = (vec_center[vec_center.size()-1].x + vec_center[vec_center.size()-2].x+vec_center[vec_center.size()-3].x)/3.0f;// 추가 2017.04.13
								float t_y1 = (vec_center[vec_center.size()-1].y + vec_center[vec_center.size()-2].y+vec_center[vec_center.size()-3].y)/3.0f;// 추가 2017.04.13
								vec_2center.push_back(Point2f(t_x0,t_y0));// 추가 2017.04.13
								vec_2center.push_back(Point2f(t_x1,t_y1));// 추가 2017.04.13
								//fitLine(vec_center,lines, CV_DIST_L2,0,0.01,0.01);
								fitLine(vec_2center,lines, CV_DIST_L2,0,0.01,0.01);
								//float lefty = (-lines[2]*lines[1]/lines[0])+lines[3];
								//float righty = ((Out_binary.cols-lines[2])*lines[1]/lines[0])+lines[3];
								//float lefty = t_y0;
								//float righty = t_y1;
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,0,0),1);
								}

								double v_distnace = 0;
								for (int i=0;i<vec_center.size();i++)
								{
									v_distnace = abs(lines[1]*vec_center[i].x - lines[0]*vec_center[i].y + lines[0]*lines[3] - lines[1]*lines[2])
										/sqrt(lines[0]*lines[0] + lines[1]*lines[1])*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_distnace);
								}
							}
						}
					}

				}	
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::BRIGHTNESS_AREA_S) // Brightness of Area
				{
					//J_Delete_Boundary(Out_binary,1);
					//J_Fill_Hole(Out_binary);

					if (BOLT_Param[Cam_num].nThres_V1[s] != 0.0f || BOLT_Param[Cam_num].nThres_V2[s] != 255.0f)
					{
						for (int i=0;i<Out_binary.rows;i++)
						{
							for (int j=0;j<Out_binary.cols;j++)
							{
								if (Out_binary.at<uchar>(i,j) > 0)
								{
									dist_vec.push_back((double)CP_Gray_Img.at<uchar>(i,j));
								}
							}	
						}
					}
					else
					{
						for (int i=0;i<Out_binary.rows;i++)
						{
							for (int j=0;j<Out_binary.cols;j++)
							{
								//if (Out_binary.at<uchar>(i,j) > 0)
								{
									dist_vec.push_back((double)CP_Gray_Img.at<uchar>(i,j));
								}
							}	
						}
					}
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//for( int k = 0; k < contours.size(); k++ )
						//{
						//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), CV_FILLED, 8, hierarchy);
						//}
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//#pragma omp parallel for
						if (contours.size() > 0)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), 1, 8, hierarchy);
								fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,100,255),8);
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::AREA_BLOB_S) // BLOB Size
				{
					if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 0 || BOLT_Param[Cam_num].nDirecFilterUsage[s] == 2)
					{
						if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 2)
						{
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								Mat HSV_Img1;Mat HSV_channel1[3];
								cvtColor(Src_Img[Cam_num], HSV_Img1, CV_BGR2Lab);
								//blur(HSV_Img, HSV_Img, Size(5,5));
								split(HSV_Img1, HSV_channel1);
								cvtColor(HSV_channel1[1],Dst_Img[Cam_num],CV_GRAY2BGR);
							}

							Mat HSV_Img;Mat HSV_channel[3];
							cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2Lab);
							//cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2HSV);
							//blur(HSV_Img, HSV_Img, Size(5,5));
							split(HSV_Img, HSV_channel);
							CP_Gray_Img = HSV_channel[1].clone();
							if (BOLT_Param[Cam_num].nMethod_Direc[s] == 0 || BOLT_Param[Cam_num].nMethod_Direc[s] == 1)
							{
								medianBlur(CP_Gray_Img,CP_Gray_Img,3);
							}
							if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
							} 
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
							} 
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
							{
								inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
							{
								Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
								inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
								subtract(White_Out_binary, Out_binary, Out_binary);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
							{
								threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
							{
								blur(CP_Gray_Img, Out_binary, Size(3,3));
								// Run the edge detector on grayscale
								Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
							}
						}

						if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
						{
							Mat Convex_Img = Out_binary.clone();
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() > 0)
							{
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									convexHull( Mat(contours[k]), hull1[k], false );
									//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
									drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
								}
								subtract(Convex_Img,Out_binary,Out_binary);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
								erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
							}
						}

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						}
						J_Delete_Boundary(Out_binary,1);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							////#pragma omp parallel for
							//for( int k = 0; k < contours.size(); k++ )
							//{
							//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
							//	fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							//}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
								}
							}
						}
						//Mat stats, centroids, label;  
						//int numOfLables = connectedComponentsWithStats(Out_binary, label,   
						//	stats, centroids, 8,CV_32S);
						int t_cnt = 0;double area = 0;int t_w = 0;int t_h = 0;int t_x = 0;int t_y = 0;
						//if (numOfLables > 0)
						RotatedRect t_boundRect;
						if (contours.size() > 0)
						{
							for (int j = 0; j < contours.size(); j++) 
							{
								//area = stats.at<int>(j, CC_STAT_AREA);
								//t_x = stats.at<int>(j, CC_STAT_LEFT);
								//t_y = stats.at<int>(j, CC_STAT_TOP);
								//t_w = stats.at<int>(j, CC_STAT_WIDTH);
								//t_h = stats.at<int>(j, CC_STAT_HEIGHT);
								area = (contourArea(contours[j],false)+0.5);
								t_boundRect = minAreaRect( Mat(contours[j]) );
								t_x = t_boundRect.boundingRect().x;
								t_y = t_boundRect.boundingRect().y;
								t_w = t_boundRect.boundingRect().width;
								t_h = t_boundRect.boundingRect().height;
								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										dist_vec.push_back(area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}

											msg.Format("BLOB(%f)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											msg.Format("%f",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_w*BOLT_Param[Cam_num].nResolution[0] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_w*BOLT_Param[Cam_num].nResolution[0])
									{
										dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}

											msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_h*BOLT_Param[Cam_num].nResolution[1] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_h*BOLT_Param[Cam_num].nResolution[1])
									{
										dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[1]);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}

											msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
									}
								} 
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
											msg.Format("No.(%d)",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d",t_cnt+1);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 4)
								{//기준점에서 거리
									double Cx = t_boundRect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
									double Cy = t_boundRect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

									double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
									double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
									//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
									//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

									//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
									{
										//Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

										//for (int yy = t_y; yy < t_y + t_h; ++yy) {

										//	int *tlabel = label.ptr<int>(yy);
										//	uchar* pixel = t_Mask.ptr<uchar>(yy);
										//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
										//		if (tlabel[xx] == j)
										//		{
										//			pixel[xx] = 255;  
										//		}
										//	}
										//}
										//findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										RotatedRect t_Rect =  minAreaRect(contours[j]);
										Cx = t_Rect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										Cy = t_Rect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

										double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
										dist_vec.push_back(t_D);

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}

											line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

											msg.Format("Dist(%1.3f)",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 5)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
										&& t_x > 1 && t_y > 1 && t_x + t_w < BOLT_Param[Cam_num].nRect[s].size().width -1 && t_y + t_h < BOLT_Param[Cam_num].nRect[s].size().height -1)
									{
										dist_vec.push_back(area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
											msg.Format("BLOB(%f)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%f",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 6)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
										&& (t_x <= 0 || t_y <= 0 || t_x + t_w >= BOLT_Param[Cam_num].nRect[s].size().width -1 || t_y + t_h >= BOLT_Param[Cam_num].nRect[s].size().height -1))
									{
										dist_vec.push_back(area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = t_boundRect.center.x; //중심좌표
											int y = t_boundRect.center.y;
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);											//int x = centroids.at<double>(j, 0); //중심좌표
											//int y = centroids.at<double>(j, 1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx][2] = 255;  
											//			pixel[xx][1] = 0;
											//			pixel[xx][0] = 0;
											//		}
											//	}
											//}

											msg.Format("BLOB(%f)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%f",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
								}
							}

							if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
							{
								dist_vec.push_back((double)t_cnt);
							}
						}
					}
					else if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 1 || BOLT_Param[Cam_num].nDirecFilterUsage[s] == 3)
					{
						if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 3)
						{
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								Mat HSV_Img1;Mat HSV_channel1[3];
								cvtColor(Src_Img[Cam_num], HSV_Img1, CV_BGR2Lab);
								//blur(HSV_Img, HSV_Img, Size(5,5));
								split(HSV_Img1, HSV_channel1);
								cvtColor(HSV_channel1[1],Dst_Img[Cam_num],CV_GRAY2BGR);
							}

							Mat HSV_Img;Mat HSV_channel[3];
							cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2Lab);
							//cvtColor(Src_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), HSV_Img, CV_BGR2HSV);
							//blur(HSV_Img, HSV_Img, Size(5,5));
							split(HSV_Img, HSV_channel);
							CP_Gray_Img = HSV_channel[1].clone();
						}
						// 어두운불량 구해오기
						Mat t_Blur, Defect_Dark_Img1, Defect_Bright_Img1;
						if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
						{
							//medianBlur(CP_Gray_Img,t_Blur,BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(CP_Gray_Img,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
						{
							dilate(CP_Gray_Img,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
						{
							dilate(CP_Gray_Img,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(t_Blur,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}

						subtract(t_Blur, CP_Gray_Img, Defect_Dark_Img1);
						threshold(Defect_Dark_Img1,Out_binary,BOLT_Param[Cam_num].nDirecFilterDarkThres[s],255,CV_THRESH_BINARY);

						if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
						{
							Mat Convex_Img = Out_binary.clone();
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() > 0)
							{
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									convexHull( Mat(contours[k]), hull1[k], false );
									//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
									drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
								}
								subtract(Convex_Img,Out_binary,Out_binary);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
								erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
							}
						}

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
							{
								erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
						}
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							////#pragma omp parallel for
							//for( int k = 0; k < contours.size(); k++ )
							//{
							//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
							//	fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							//}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
								}
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterDarkThres[s] > 0)
						{
							//Mat stats, centroids, label;  
							//int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							//	stats, centroids, 8,CV_32S);
							int t_cnt = 0;double area = 0;int t_w = 0;int t_h = 0;int t_x = 0;int t_y = 0;
							RotatedRect t_boundRect;
							if (contours.size() > 0)
								//if (numOfLables > 0)
							{
								for (int j = 0; j < contours.size(); j++) 
								{
									//area = stats.at<int>(j, CC_STAT_AREA);
									//t_x = stats.at<int>(j, CC_STAT_LEFT);
									//t_y = stats.at<int>(j, CC_STAT_TOP);
									//t_w = stats.at<int>(j, CC_STAT_WIDTH);
									//t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									area = (contourArea(contours[j],false)+0.5);
									t_boundRect = minAreaRect( Mat(contours[j]) );
									t_x = t_boundRect.boundingRect().x;
									t_y = t_boundRect.boundingRect().y;
									t_w = t_boundRect.boundingRect().width;
									t_h = t_boundRect.boundingRect().height;

									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);
												//												for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_w*BOLT_Param[Cam_num].nResolution[0] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_w*BOLT_Param[Cam_num].nResolution[0])
										{
											dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_h*BOLT_Param[Cam_num].nResolution[1] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_h*BOLT_Param[Cam_num].nResolution[1])
										{
											dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("No.(%d)",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 4)
									{//기준점에서 거리
										double Cx = t_boundRect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										double Cy = t_boundRect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

										double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
										double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
										//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
										//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											//Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	uchar* pixel = t_Mask.ptr<uchar>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx] = 255;  
											//		}
											//	}
											//}
											//findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
											RotatedRect t_Rect =  minAreaRect(contours[j]);
											Cx = t_Rect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
											Cy = t_Rect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

											double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											dist_vec.push_back(t_D);

											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
												{
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
													//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
													//for (int yy = t_y; yy < t_y + t_h; ++yy) {

													//	int *tlabel = label.ptr<int>(yy);
													//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
													//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
													//		if (tlabel[xx] == j)
													//		{
													//			pixel[xx][2] = 255;  
													//			pixel[xx][1] = 0;
													//			pixel[xx][0] = 0;
													//		}
													//	}
													//}
												}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 5)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& t_x > 1 && t_y > 1 && t_x + t_w < BOLT_Param[Cam_num].nRect[s].size().width -1 && t_y + t_h < BOLT_Param[Cam_num].nRect[s].size().height -1)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 6)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& (t_x <= 0 || t_y <= 0 || t_x + t_w >= BOLT_Param[Cam_num].nRect[s].size().width -1 || t_y + t_h >= BOLT_Param[Cam_num].nRect[s].size().height -1))
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
								}

								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									dist_vec.push_back((double)t_cnt);
								}
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
								//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
						{
							//medianBlur(CP_Gray_Img,t_Blur,BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							erode(CP_Gray_Img,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
						{
							erode(CP_Gray_Img,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element_h,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}
						else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
						{
							erode(CP_Gray_Img,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
							dilate(t_Blur,t_Blur,element_v,Point(-1,-1), BOLT_Param[Cam_num].nDirecFilterCNT[s]);
						}

						subtract(CP_Gray_Img, t_Blur, Defect_Bright_Img1);
						threshold(Defect_Bright_Img1,Out_binary,BOLT_Param[Cam_num].nDirecFilterBrightThres[s],255,CV_THRESH_BINARY);

						if (BOLT_Param[Cam_num].nConvexBLOBOption[s] == 1)
						{
							Mat Convex_Img = Out_binary.clone();
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							if (contours.size() > 0)
							{
								vector<vector<Point>> hull1(contours.size());
								//#pragma omp parallel for
								for(int k=0; k<contours.size(); k++)
								{	
									convexHull( Mat(contours[k]), hull1[k], false );
									//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
									drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
								}
								subtract(Convex_Img,Out_binary,Out_binary);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), 1);
								erode(Out_binary,Out_binary,element,Point(-1,-1), 1);
							}
						}

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::ALL)
							{
								erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::HORIZONTAL)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
							else if (BOLT_Param[Cam_num].nDirecFilter[s] == FILTER_DIRECTION::VERTICAL)
							{
								erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
								dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							}
						}
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							////#pragma omp parallel for
							//for( int k = 0; k < contours.size(); k++ )
							//{
							//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
							//	fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
							//}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(0,255,0),8);
								}
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterBrightThres[s] > 0)
						{
							//Mat stats, centroids, label;  
							//int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							//	stats, centroids, 8,CV_32S);

							int t_cnt = 0;double area = 0;int t_w = 0;int t_h = 0;int t_x = 0;int t_y = 0;
							RotatedRect t_boundRect;
							if (contours.size() > 0)
							//if (numOfLables > 0)
							{
								for (int j = 0; j < contours.size(); j++) 
								{
									//area = stats.at<int>(j, CC_STAT_AREA);
									//t_x = stats.at<int>(j, CC_STAT_LEFT);
									//t_y = stats.at<int>(j, CC_STAT_TOP);
									//t_w = stats.at<int>(j, CC_STAT_WIDTH);
									//t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									area = (contourArea(contours[j],false)+0.5);
									t_boundRect = minAreaRect( Mat(contours[j]) );
									t_x = t_boundRect.boundingRect().x;
									t_y = t_boundRect.boundingRect().y;
									t_w = t_boundRect.boundingRect().width;
									t_h = t_boundRect.boundingRect().height;
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_w*BOLT_Param[Cam_num].nResolution[0] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_w*BOLT_Param[Cam_num].nResolution[0])
										{
											dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)t_h*BOLT_Param[Cam_num].nResolution[1] && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)t_h*BOLT_Param[Cam_num].nResolution[1])
										{
											dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												//msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("No.(%d)",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",t_cnt+1);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 4)
									{//기준점에서 거리
										double Cx = t_boundRect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										double Cy = t_boundRect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

										double R0x = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x);
										double R0y = (double)(BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y);
										//double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
										//double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										//if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area)
										{
											//Mat t_Mask = Mat::zeros(Out_binary.size(), CV_8UC1);

											//for (int yy = t_y; yy < t_y + t_h; ++yy) {

											//	int *tlabel = label.ptr<int>(yy);
											//	uchar* pixel = t_Mask.ptr<uchar>(yy);
											//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
											//		if (tlabel[xx] == j)
											//		{
											//			pixel[xx] = 255;  
											//		}
											//	}
											//}
											//findContours( t_Mask.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
											RotatedRect t_Rect =  minAreaRect(contours[j]);
											Cx = t_Rect.center.x + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
											Cy = t_Rect.center.y + (double)BOLT_Param[Cam_num].nRect[s].y;

											double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);
											dist_vec.push_back(t_D);

											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												//if (BOLT_Param[Cam_num].nCirclePositionMethod[s] != 4)
												{
													drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
													//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
													//for (int yy = t_y; yy < t_y + t_h; ++yy) {

													//	int *tlabel = label.ptr<int>(yy);
													//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
													//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
													//		if (tlabel[xx] == j)
													//		{
													//			pixel[xx][2] = 255;  
													//			pixel[xx][1] = 0;
													//			pixel[xx][0] = 0;
													//		}
													//	}
													//}
												}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(0,0,255),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x+5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(255,100,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,0,255),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,0,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 5)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& t_x > 1 && t_y > 1 && t_x + t_w < BOLT_Param[Cam_num].nRect[s].size().width -1 && t_y + t_h < BOLT_Param[Cam_num].nRect[s].size().height -1)
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 6)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= area
											&& (t_x <= 0 || t_y <= 0 || t_x + t_w >= BOLT_Param[Cam_num].nRect[s].size().width -1 || t_y + t_h >= BOLT_Param[Cam_num].nRect[s].size().height -1))
										{
											dist_vec.push_back(area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
												//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(t_x,t_y,t_w,t_h),CV_RGB(255,0,0),1);
												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}
											}
											if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = t_boundRect.center.x; //중심좌표
												int y = t_boundRect.center.y;
												drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, j, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);												//int x = centroids.at<double>(j, 0); //중심좌표
												//int x = centroids.at<double>(j, 0); //중심좌표
												//int y = centroids.at<double>(j, 1);

												//for (int yy = t_y; yy < t_y + t_h; ++yy) {

												//	int *tlabel = label.ptr<int>(yy);
												//	Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(yy);
												//	for (int xx = t_x; xx < t_x + t_w; ++xx) {
												//		if (tlabel[xx] == j)
												//		{
												//			pixel[xx][2] = 255;  
												//			pixel[xx][1] = 0;
												//			pixel[xx][0] = 0;
												//		}
												//	}
												//}

												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
											t_cnt++;
										}
									}
								}

								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									dist_vec.push_back((double)t_cnt);
								}
								//if (m_Text_View[Cam_num] && !ROI_Mode)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
								//if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								//{
								//	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//	for( int k = 0; k < contours.size(); k++ )
								//	{
								//		drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
								//	}
								//}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::BOTTOM_SHAPE_S) // SHAPE of Bottom
				{
					if (BOLT_Param[Cam_num].nThickness[s] > 0)
					{
						int t_filter_cnt = (int)(1*BOLT_Param[Cam_num].nThickness[s]/BOLT_Param[Cam_num].nResolution[0]);
						erode(Out_binary,Out_binary,element_v,Point(-1,-1),t_filter_cnt);
						dilate(Out_binary,Out_binary,element_v,Point(-1,-1),t_filter_cnt);
					}

					Rect tt_TROI;int m_max_object_value = 0;int m_max_object_num = -1;
					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if (contours.size() > 0)
					{
						for( int k = 0; k < contours.size(); k++ )
						{  
							tt_TROI = boundingRect(contours[k]);
							if (m_max_object_value<= tt_TROI.height*tt_TROI.width)
							{
								m_max_object_value = tt_TROI.height*tt_TROI.width;
								m_max_object_num = k;
							}
						}
					}
					if (m_max_object_num >= 0)
					{
						tt_TROI = boundingRect(contours[m_max_object_num]);
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(tt_TROI.x+tt_TROI.width/2,tt_TROI.y),Point(tt_TROI.x+tt_TROI.width/2,tt_TROI.y+tt_TROI.height),CV_RGB(255,255,0),1);
						}
						//tt_TROI.height = tt_TROI.height - BOLT_Param[Cam_num].nHeightforShape[s];
						//Out_binary(tt_TROI) -= 255;
					}

					Mat t_End_Nail = Out_binary.clone();
					if (m_max_object_num > -1)
					{
						tt_TROI = boundingRect(contours[m_max_object_num]);
						//msg.Format("Save\\Debugging\\CAM0_00.bmp",s);
						//imwrite(msg.GetBuffer(),t_End_Nail);
						int t_h = tt_TROI.height - BOLT_Param[Cam_num].nHeightforShape[s]-1;
						if (t_h <= 0)
						{
							t_h = 1;
						}
						Rect t_mask(tt_TROI.x,tt_TROI.y
							,tt_TROI.width,t_h);
						t_End_Nail(t_mask) -= 255;
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(t_mask.x,t_mask.y),Point(t_mask.x+t_mask.width,t_mask.y),CV_RGB(255,255,0),1);
						}
						//msg.Format("Save\\Debugging\\CAM0_01.bmp",s);
						//imwrite(msg.GetBuffer(),t_End_Nail);

					}
					else
					{
						dist_vec.push_back(0);
					}



					//dilate(t_End_Nail,t_End_Nail,element_v,Point(-1,-1),1);
					//erode(t_End_Nail,t_End_Nail,element_v,Point(-1,-1),1);

					if (m_max_object_num > -1)
					{
						if (m_Text_View[Cam_num])
						{
							findContours( t_End_Nail.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							m_max_object_num = -1;m_max_object_value = 0;Rect tt_TROI;								
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{  
									tt_TROI = boundingRect(contours[k]);
									if (m_max_object_value<= tt_TROI.height * tt_TROI.width)
									{
										m_max_object_value = tt_TROI.height * tt_TROI.width;
										m_max_object_num = k;
									}
								}


								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,100,255), CV_FILLED, 8);
							}
						}

						Mat stats, centroids, label;  
						int numOfLables = connectedComponentsWithStats(t_End_Nail, label,   
							stats, centroids, 8,CV_32S);
						m_max_object_value = 0;m_max_object_num = -1;
						for (int j = 1; j < numOfLables; j++) 
						{
							int area = stats.at<int>(j, CC_STAT_AREA);

							if (area >= m_max_object_value)
							{
								m_max_object_value = area;
								m_max_object_num = j;
							}
						}
						if (m_max_object_num > -1)
						{
							int x = centroids.at<double>(m_max_object_num, 0); //중심좌표
							int y = centroids.at<double>(m_max_object_num, 1);

							dist_vec.push_back((double)m_max_object_value);
							//if (m_Text_View[Cam_num]) // 표시
							//{

							//	msg.Format("%d",m_max_object_value);
							//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-35,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.6, CV_RGB(255,100,0), 2, 8);
							//}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::DIST_TWO_CENTER_S) // DIST_TWO_CENTER_TB
				{
					Mat ROI1_Img, ROI2_Img;
					bool ROI1_check = false;
					bool ROI2_check = false;

					if (BOLT_Param[Cam_num].nROI1[s].x + BOLT_Param[Cam_num].nROI1[s].width > Out_binary.cols-1)
					{
						BOLT_Param[Cam_num].nROI1[s].width = Out_binary.cols - BOLT_Param[Cam_num].nROI1[s].x -1;
					}
					if (BOLT_Param[Cam_num].nROI2[s].x + BOLT_Param[Cam_num].nROI2[s].width > Out_binary.cols-1)
					{
						BOLT_Param[Cam_num].nROI2[s].width = Out_binary.cols - BOLT_Param[Cam_num].nROI2[s].x -1;
					}
					if (BOLT_Param[Cam_num].nROI1[s].y + BOLT_Param[Cam_num].nROI1[s].height > Out_binary.rows-1)
					{
						BOLT_Param[Cam_num].nROI1[s].height = Out_binary.rows - BOLT_Param[Cam_num].nROI1[s].y -1;
					}
					if (BOLT_Param[Cam_num].nROI2[s].y + BOLT_Param[Cam_num].nROI2[s].height > Out_binary.rows-1)
					{
						BOLT_Param[Cam_num].nROI2[s].height = Out_binary.rows - BOLT_Param[Cam_num].nROI2[s].y -1;
					}

					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI1[s],CV_RGB(100,0,255),1);
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI2[s],CV_RGB(100,0,255),1);
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI1[s],CV_RGB(100,0,255),1);
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].nROI2[s],CV_RGB(100,0,255),1);
					}
					if (BOLT_Param[Cam_num].nROI1[s].x >= 0 && BOLT_Param[Cam_num].nROI1[s].y >= 0 && 
						BOLT_Param[Cam_num].nROI1[s].x + BOLT_Param[Cam_num].nROI1[s].width < Out_binary.cols-1 &&
						BOLT_Param[Cam_num].nROI1[s].y + BOLT_Param[Cam_num].nROI1[s].height < Out_binary.rows-1)
					{
						ROI1_Img = Out_binary(BOLT_Param[Cam_num].nROI1[s]).clone();
						ROI1_check = true;
					}
					if (BOLT_Param[Cam_num].nROI2[s].x >= 0 && BOLT_Param[Cam_num].nROI2[s].y >= 0 && 
						BOLT_Param[Cam_num].nROI2[s].x + BOLT_Param[Cam_num].nROI2[s].width < Out_binary.cols-1 &&
						BOLT_Param[Cam_num].nROI2[s].y + BOLT_Param[Cam_num].nROI2[s].height < Out_binary.rows-1)
					{
						ROI2_Img = Out_binary(BOLT_Param[Cam_num].nROI2[s]).clone();
						ROI2_check = true;
					}

					if (!ROI1_check || !ROI2_check)
					{
						dist_vec.push_back(0);
					}
					else
					{
						Mat Mp_ROI1_Img, Mp_ROI2_Img;
						if (BOLT_Param[Cam_num].nROI12_Direction[s] == 0) // 가로방향
						{
							erode(ROI1_Img,Mp_ROI1_Img,element_h,Point(-1,-1), 1);
							subtract(ROI1_Img,Mp_ROI1_Img,Mp_ROI1_Img);
							erode(ROI2_Img,Mp_ROI2_Img,element_h,Point(-1,-1), 1);
							subtract(ROI2_Img,Mp_ROI2_Img,Mp_ROI2_Img);

							Point2f ROI1_Center;Point2f ROI2_Center;
							vector<Point2f> vec_ROI1_Point;vector<Point2f> vec_ROI2_Point;
							for (int j=1;j < Mp_ROI1_Img.rows-2;j++)
							{
								for (int i = 1; i < Mp_ROI1_Img.cols-2;i++)
								{
									if (Mp_ROI1_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI1_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI1[s].x,(float)j+BOLT_Param[Cam_num].nROI1[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}

							for (int j=1;j < Mp_ROI2_Img.rows-2;j++)
							{
								for (int i = 1; i < Mp_ROI2_Img.cols-2;i++)
								{
									if (Mp_ROI2_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI2_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI2[s].x,(float)j+BOLT_Param[Cam_num].nROI2[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}

							if (vec_ROI1_Point.size() == 0 || vec_ROI2_Point.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{
								float t_dist = 0;
								float t_dist_min = 999999999;float t_dist_max = 0;int t_ROI1_IDX = 0;int t_ROI2_IDX = 0;
								if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MIN) // 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_min*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MAX) // 최대
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_max*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::AVERAGE) // 평균
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									v_Dist = abs(ROI2_Center.x - ROI1_Center.x)*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::RANGE) // 최대 - 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = (t_dist_max-t_dist_min)*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}							
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::SUMOFALL) // 합계
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist += abs(vec_ROI2_Point[j].x - vec_ROI1_Point[i].x);
											t_ROI1_IDX++;
										}
									}
									t_dist /= (float)t_ROI1_IDX;
									v_Dist = t_dist*BOLT_Param[Cam_num].nResolution[0];
									dist_vec.push_back(v_Dist);
								}

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI1_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI2_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI1_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI2_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
							}
						}
						else if (BOLT_Param[Cam_num].nROI12_Direction[s] == 1) // 세로방향
						{
							erode(ROI1_Img,Mp_ROI1_Img,element_v,Point(-1,-1), 1);
							subtract(ROI1_Img,Mp_ROI1_Img,Mp_ROI1_Img);
							erode(ROI2_Img,Mp_ROI2_Img,element_v,Point(-1,-1), 1);
							subtract(ROI2_Img,Mp_ROI2_Img,Mp_ROI2_Img);

							Point2f ROI1_Center;Point2f ROI2_Center;
							vector<Point2f> vec_ROI1_Point;vector<Point2f> vec_ROI2_Point;
							for (int i = 1; i < Mp_ROI1_Img.cols-2;i++)
							{
								for (int j=1;j < Mp_ROI1_Img.rows-2;j++)
								{
									if (Mp_ROI1_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI1_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI1[s].x,(float)j+BOLT_Param[Cam_num].nROI1[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI1[s].x,j+BOLT_Param[Cam_num].nROI1[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}

							for (int i = 1; i < Mp_ROI2_Img.cols-2;i++)
							{
								for (int j=1;j < Mp_ROI2_Img.rows-2;j++)
								{
									if (Mp_ROI2_Img.at<uchar>(j,i) > 0)
									{
										vec_ROI2_Point.push_back(Point2f((float)i+BOLT_Param[Cam_num].nROI2[s].x,(float)j+BOLT_Param[Cam_num].nROI2[s].y));
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(i+BOLT_Param[Cam_num].nROI2[s].x,j+BOLT_Param[Cam_num].nROI2[s].y),0,CV_RGB(255,100,0),1);
										}
										break;
									}
								}
							}
							if (vec_ROI1_Point.size() == 0 || vec_ROI2_Point.size() == 0)
							{
								dist_vec.push_back(0);
							}
							else
							{

								float t_dist = 0;
								float t_dist_min = 999999999;float t_dist_max = 0;int t_ROI1_IDX = 0;int t_ROI2_IDX = 0;
								if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MIN) // 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_min*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MAX) // 최대
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = t_dist_max*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::AVERAGE) // 평균
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									v_Dist = abs(ROI2_Center.y - ROI1_Center.y)*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::RANGE) // 최대 - 최소
								{
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist <= t_dist_min)
											{
												t_dist_min = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist = abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											if (t_dist >= t_dist_max)
											{
												t_dist_max = t_dist;
												t_ROI1_IDX = i;t_ROI2_IDX = j;
											}
										}
									}
									ROI1_Center = vec_ROI1_Point[t_ROI1_IDX];
									ROI2_Center = vec_ROI2_Point[t_ROI2_IDX];
									v_Dist = (t_dist_max-t_dist_min)*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}							
								else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::SUMOFALL) // 합계
								{
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										ROI1_Center.x += vec_ROI1_Point[i].x;
										ROI1_Center.y += vec_ROI1_Point[i].y;
										t_ROI1_IDX++;
									}
									ROI1_Center.x /= (float)t_ROI1_IDX;
									ROI1_Center.y /= (float)t_ROI1_IDX;
									t_dist = 0;t_ROI2_IDX = 0;
									for (int i=0;i<vec_ROI2_Point.size();i++)
									{
										ROI2_Center.x += vec_ROI2_Point[i].x;
										ROI2_Center.y += vec_ROI2_Point[i].y;
										t_ROI2_IDX++;
									}
									ROI2_Center.x /= (float)t_ROI2_IDX;
									ROI2_Center.y /= (float)t_ROI2_IDX;
									t_dist = 0;t_ROI1_IDX = 0;
									for (int i=0;i<vec_ROI1_Point.size();i++)
									{
										for (int j=0;j<vec_ROI2_Point.size();j++)
										{
											t_dist += abs(vec_ROI2_Point[j].y - vec_ROI1_Point[i].y);
											t_ROI1_IDX++;
										}
									}
									t_dist /= (float)t_ROI1_IDX;
									v_Dist = t_dist*BOLT_Param[Cam_num].nResolution[1];
									dist_vec.push_back(v_Dist);
								}

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI1_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI2_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI1_Center.y),Point2f(ROI2_Center.x,ROI1_Center.y),CV_RGB(255,0,0),1);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(ROI1_Center.x,ROI2_Center.y),Point2f(ROI2_Center.x,ROI2_Center.y),CV_RGB(255,0,0),1);
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::CONVEX_BLOB_S) // CONVEX BLOB ANALYSIS
				{
					vector<vector<Point>> contours1;
					vector<Vec4i> hierarchy1;

					Mat Convex_Img = Out_binary.clone();
					findContours( Out_binary.clone(), contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if (contours1.size() > 0)
					{
						vector<vector<Point>> hull1(contours1.size());
						//#pragma omp parallel for
						for(int k=0; k<contours1.size(); k++)
						{	
							convexHull( Mat(contours1[k]), hull1[k], false ); 
							//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
							drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours1, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours1, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
							}
						}
					}
					subtract(Convex_Img,Out_binary,Convex_Img);
					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						dilate(Convex_Img,Convex_Img,element,Point(-1,-1), 1);
						erode(Convex_Img,Convex_Img,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]+1);
						dilate(Convex_Img,Convex_Img,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
					}

					Mat stats, centroids, label;  
					int numOfLables = connectedComponentsWithStats(Convex_Img, label,   
						stats, centroids, 8,CV_32S);
					int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
					if (numOfLables > 0)
					{
						for (int j = 1; j < numOfLables; j++) 
						{
							area = stats.at<int>(j, CC_STAT_AREA);
							t_w = stats.at<int>(j, CC_STAT_WIDTH);
							t_h = stats.at<int>(j, CC_STAT_HEIGHT);
							if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
							{
								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
								{
									dist_vec.push_back((double)area);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("BLOB(%d)",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
										msg.Format("%d",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 1)
								{
									dist_vec.push_back((double)t_w*BOLT_Param[Cam_num].nResolution[0]);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("W(%1.3f)",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
										msg.Format("%1.3f",(double)t_w*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									}
								}
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 2)
								{
									dist_vec.push_back((double)t_h*BOLT_Param[Cam_num].nResolution[0]);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
										msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[0]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									}
								}

								t_cnt++;
							}
						}
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,0,0),8);
								}
							}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//#pragma omp parallel for
							if (contours.size() > 0)
							{
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, 8, hierarchy);
									fillPoly(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, CV_RGB(255,0,0),8);
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::MATCH_RATE_S) // MATCH_RATE_S
				{
					// MATCH_RATE_S
					dist_vec.push_back(MatchRate[Cam_num]);
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						msg.Format("%1.3f",MatchRate[Cam_num]);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(0,255,0), 1 + (double)Gray_Img[Cam_num].rows/960, 8);
					}

					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						msg.Format("%1.3f",MatchRate[Cam_num]);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(0,255,0), 1 + (double)Gray_Img[Cam_num].rows/960, 8);
					}
				}
			}


			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [끝] 측정 알고리즘
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [시작] 측정된 값 각 방법에따라 계산
			// Measurement 0:최소, 1:최대, 2:최대-최소, 3:평균, 4:Total
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			double v_result = 0;
			if (dist_vec.size() <= 1)
			{
				if (dist_vec.size() == 1)
				{
					////msg.Format("%1.3f",dist_vec[0]);
					////AfxMessageBox(msg);
					v_result = dist_vec[0];
				}
			}
			else
			{
				std::sort(dist_vec.begin(), dist_vec.end(), Point_compare_Increasing);
				vector<double> t_dist_vec;
				for (int i=0;i<dist_vec.size();i++)
				{
					if ( i >= (int)((double)dist_vec.size()*(double)BOLT_Param[Cam_num].nCalmin[s]/100.0)
						&& i <= (int)((double)dist_vec.size()*(double)BOLT_Param[Cam_num].nCalmax[s]/100.0))
					{
						t_dist_vec.push_back(dist_vec[i]);
						//msg.Format("%1.3f",dist_vec[i]);
						//AfxMessageBox(msg);
					}
				}

				if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MIN) // 최소
				{
					double t_v = 99999;
					for (int i=0;i<t_dist_vec.size();i++)
					{
						if (t_v >= t_dist_vec[i])
						{
							t_v = t_dist_vec[i];
						}
					}
					v_result = t_v;
				} 
				else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::MAX) // 최대
				{
					double t_v = 0;
					for (int i=0;i<t_dist_vec.size();i++)
					{
						if (t_v <= t_dist_vec[i])
						{
							t_v = t_dist_vec[i];
						}
					}
					v_result = t_v;
				}
				else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::RANGE) // 최대-최소
				{
					double t_v_min = 99999;double t_v_max = 0;
					for (int i=0;i<t_dist_vec.size();i++)
					{
						if (t_v_min >= t_dist_vec[i])
						{
							t_v_min = t_dist_vec[i];
						}
						if (t_v_max <= t_dist_vec[i])
						{
							t_v_max = t_dist_vec[i];
						}
					}
					v_result = t_v_max - t_v_min;				
				}
				else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::AVERAGE) // 평균
				{
					for (int i=0;i<t_dist_vec.size();i++)
					{
						v_result += t_dist_vec[i];
					}
					v_result/= (double)t_dist_vec.size();
				}			
				else if (BOLT_Param[Cam_num].nMethod_Cal[s] == RESULT_METHOD::SUMOFALL) // Total
				{
					for (int i=0;i<t_dist_vec.size();i++)
					{
						v_result += t_dist_vec[i];
					}
				}	
			}

			// Offset값 더하기
			//if (v_result != 0)
			{
				v_result += BOLT_Param[Cam_num].Offset[s];
				if (v_result < 0)
				{
					v_result = 0;
				}
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [끝] 측정된 값 각 방법에따라 계산
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			if (m_License_Check == -1)
			{
				CString t_CString;
				t_CString.Format("C%d:%02d_-1_",Cam_num,s);
				Result_Info[Cam_num] += t_CString;
				//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
			} else
			{
				CString t_CString;
				t_CString.Format("C%d:%02d_%1.3f_",Cam_num,s,v_result);
				Result_Info[Cam_num] += t_CString;

				//Result_Info[Cam_num].Format("%sC%d:%02d_%1.3f_",Result_Info[Cam_num],Cam_num,s,v_result);
			}

			//if (m_Text_View[Cam_num] && s >= 1 && !ROI_Mode)
			//{
			//	rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
			//	msg.Format("ROI#%d(%1.3f)", s,v_result);
			//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+(2*Gray_Img[Cam_num].rows/480),BOLT_Param[Cam_num].nRect[s].y + 11 + 6*Gray_Img[Cam_num].rows/480), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1 + (double)Gray_Img[Cam_num].rows/960, 8);
			//}
			if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
			{
				//msg.Format("%d/%d", ROI_Num,s);
				//AfxMessageBox(msg);
				rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
				msg.Format("ROI#%d(%1.3f)", s,v_result);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+(2*Gray_Img[Cam_num].rows/480),BOLT_Param[Cam_num].nRect[s].y + 11+ 6*Gray_Img[Cam_num].rows/480), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1 + (double)Gray_Img[Cam_num].rows/960, 8);
			}

			BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
		}
	} else
	{
		for (int ss=1;ss<41;ss++)
		{
			CString t_CString;
			t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
			Result_Info[Cam_num] += t_CString;
			//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
		}
	}
	Alg_Run_Check[Cam_num] = false;
	return true;
}

bool CImPro_Library::ROI_Object_Find(int Cam_num)
{
	Template_Img[Cam_num] = Gray_Img[Cam_num].clone();
	if (Cam_num == 0 && BOLT_Param[Cam_num].nTableType == 5 && BOLT_Param[Cam_num].nROI0_FilterSize[0] == 7496)
	{
		RUN_Algorithm_SUGIYAMA();
		return true;
	}

	int s=0;
	CString msg;

	bool t_check_roi = true;
	if (BOLT_Param[Cam_num].nRect[s].x < 0)
	{
		t_check_roi = false;
	}
	if (BOLT_Param[Cam_num].nRect[s].y < 0)
	{
		t_check_roi = false;
	}
	if (BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width > Gray_Img[Cam_num].cols)
	{
		t_check_roi = false;
	}
	if (BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height > Gray_Img[Cam_num].rows)
	{
		t_check_roi = false;
	}

	if (!t_check_roi)
	{
		return false;
	}

	//if (BOLT_Param[Cam_num].nRect[0].width%4 == 1)
	//{
	//	BOLT_Param[Cam_num].nRect[0].width -= 1;
	//}
	//else if (BOLT_Param[Cam_num].nRect[0].width%4 == 2)
	//{
	//	BOLT_Param[Cam_num].nRect[0].x += 1;
	//	BOLT_Param[Cam_num].nRect[0].width -= 2;
	//}
	//else if (BOLT_Param[Cam_num].nRect[0].width%4 == 3)
	//{
	//	BOLT_Param[Cam_num].nRect[0].width += 1;
	//}
	//if (BOLT_Param[Cam_num].nRect[0].height%4 == 1)
	//{
	//	BOLT_Param[Cam_num].nRect[0].height -= 1;
	//}
	//else if (BOLT_Param[Cam_num].nRect[0].height%4 == 2)
	//{
	//	BOLT_Param[Cam_num].nRect[0].y += 1;
	//	BOLT_Param[Cam_num].nRect[0].height -= 2;
	//}
	//else if (BOLT_Param[Cam_num].nRect[0].height%4 == 3)
	//{
	//	BOLT_Param[Cam_num].nRect[0].height += 1;
	//}

	if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::ROI_TYPE || BOLT_Param[Cam_num].nUse[0] == 0) //ROI 기준 측정
	{
		BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
		BOLT_Param[Cam_num].Object_Postion = Point(BOLT_Param[Cam_num].nRect[s].x,BOLT_Param[Cam_num].nRect[s].y);
		Model_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();

		if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
		{
			rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
			msg.Format("Obj. ROI");
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,50), 1, 8);
		}
		return true;
	}

	if (!Gray_Img[Cam_num].empty())
	{
		if (BOLT_Param[Cam_num].nMethod_Thres[0] == THRES_METHOD::FIRSTROI) // ROI#01 모델사용
		{
			//AfxMessageBox("1");
			Point2f R_Center(-1,-1);
			if (!Easy::CheckLicense(LicenseFeatures::EasyFind))
			{ // 유레시스 없으면
				//AfxMessageBox("2");
				//Model_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
				Model_Img[Cam_num] = Gray_Img[Cam_num].clone();
				R_Center = J_Model_Find(Cam_num);
			}
			else
			{ // 유레시스 있으면
				//AfxMessageBox("3");

				Model_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
				E_Cam_Img[Cam_num].SetImagePtr(Model_Img[Cam_num].cols,Model_Img[Cam_num].rows,Model_Img[Cam_num].data);
				//E_Cam_Img[Cam_num].SaveJpeg("Template.jpg");
				//AfxMessageBox("4");
				m_Cam_Find[Cam_num].SetScaleBias(1.00f);
				m_Cam_Find[Cam_num].SetScaleTolerance(BOLT_Param[Cam_num].nFindScaleTolerance[0]);
				m_Cam_Find[Cam_num].SetAngleBias(0.00f);
				m_Cam_Find[Cam_num].SetAngleTolerance(BOLT_Param[Cam_num].nFindAngleTolerance[0]);
				m_Cam_Find[Cam_num].SetFindExtension(BOLT_Param[Cam_num].nFindFindExtension[0]);
				m_Cam_Find[Cam_num].SetPatternType(EPatternType_ConsistentEdges);
				//m_Cam_Find[Cam_num].SetContrastMode(EFindContrastMode_PointByPointNormal);
				m_Cam_Find[Cam_num].Learn(&E_Cam_Img[Cam_num]);
				//AfxMessageBox("5");
				R_Center = J_Model_Find(Cam_num);
				//R_Center.x = (double)BOLT_Param[Cam_num].nRect[s].x+(double)BOLT_Param[Cam_num].nRect[s].width/2;
				//R_Center.y = (double)BOLT_Param[Cam_num].nRect[s].y+(double)BOLT_Param[Cam_num].nRect[s].height/2;
			}
			// 머리를 못 찾을 경우 Error 처리함.
			if(R_Center.x == -1 || R_Center.y == -1)
			{
				BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
				BOLT_Param[Cam_num].Object_Postion = Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/2,BOLT_Param[Cam_num].nRect[s].y+BOLT_Param[Cam_num].nRect[s].height/2);
				rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
				msg.Format("No Object!");
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
				return true;
			}
			else
			{
				BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
				BOLT_Param[Cam_num].Object_Postion = R_Center;
				if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
				{
					circle(Dst_Img[Cam_num],BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
					circle(Dst_Img[Cam_num],BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
				}
			}
		}
		else
		{
			// ROI 이미지 Crop
			Mat Out_binary;
			Mat Out_binary_Tmp;
			Mat CP_Gray_Img;
			BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);

			if (BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width >= Gray_Img[Cam_num].cols)
			{
				BOLT_Param[Cam_num].nRect[s].width = Gray_Img[Cam_num].cols - BOLT_Param[Cam_num].nRect[s].x - 1;
			}
			if (BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height >= Gray_Img[Cam_num].rows)
			{
				BOLT_Param[Cam_num].nRect[s].height = Gray_Img[Cam_num].rows - BOLT_Param[Cam_num].nRect[s].y - 1;
			}
			//AfxMessageBox("1");
			CP_Gray_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();
			if (m_Text_View[Cam_num] && BOLT_Param[Cam_num].nCamPosition == 0)
			{
				rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
				msg.Format("Obj. ROI");
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,0,255), 1, 8);
			}
			//Result_Debugging = true;
			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_Base_Obj_Gray_Img.bmp",Cam_num);
				imwrite(msg.GetBuffer(),CP_Gray_Img);		
			}

			//AfxMessageBox("2");

			// 임계화
			// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
			if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
			{
				inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
			{
				Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
				inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
				subtract(White_Out_binary, Out_binary, Out_binary);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
			{
				blur(CP_Gray_Img, Out_binary, Size(3,3));
				// Run the edge detector on grayscale
				Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::FIRSTROI) // ROI#01 결과 사용
			{
				threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
			}
			else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::DIFFAVG) // 평균기준 차이
			{
				Scalar tempVal = mean( CP_Gray_Img );
				float myMAtMean = tempVal.val[0];
				Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
				inRange(CP_Gray_Img, Scalar(myMAtMean - BOLT_Param[Cam_num].nThres_V1[s]),Scalar(myMAtMean + BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
				//imwrite("00.bmp",Out_binary);
				subtract(White_Out_binary, Out_binary, Out_binary);
				//imwrite("01.bmp",Out_binary);

				if (BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_SIZE_TB && BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_COUNT_TB)
				{
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						msg.Format("Avg = %1.3f", myMAtMean);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						msg.Format("Avg = %1.3f", myMAtMean);
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
					}
				}
			}
			//AfxMessageBox("0");

			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_Base_Obj_Binary_Img.bmp",Cam_num);
				imwrite(msg.GetBuffer(),Out_binary);
			}
			J_Delete_Boundary(Out_binary,1);

			int t_size = countNonZero(Out_binary);
			if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10 || t_size < 10)
			{
				rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
				msg.Format("ROI#%d", s);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,0), 1, 8);

				rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
				msg.Format("No Object!");
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
				return true;
			}

			//AfxMessageBox("1");

			if (BOLT_Param[Cam_num].nCamPosition == 0)
			{// TOP, BOTTOM
				// 머리부 찾기
				if (BOLT_Param[Cam_num].nROI0_FilterSize[0] > 0)
				{
					if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::ALL)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
					}
					else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::VERTICAL)
					{
						erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
						dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
					}
					else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::HORIZONTAL)
					{
						erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
						dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
					}
				}

				if (BOLT_Param[Cam_num].nROI0_MergeFilterSize[0] > 0)
				{
					if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::ALL)
					{
						dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
						erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
					}
					else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::VERTICAL)
					{
						dilate(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
						erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
					}
					else if (BOLT_Param[Cam_num].nROI0_FilterDirection[0] == FILTER_DIRECTION::HORIZONTAL)
					{
						dilate(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
						erode(Out_binary,Out_binary,element_h,Point(-1,-1), BOLT_Param[Cam_num].nROI0_MergeFilterSize[0]);
					}
				}

				vector<vector<Point> > contours;
				vector<Vec4i> hierarchy;
				int area, left, top, width, height = { 0, };
				int m_min_object_num = -1; int m_min_object_value = Out_binary.rows*Out_binary.cols;
				int m_max_object_num = -1; int m_max_object_value = 0;
				//imwrite("00.bmp",Out_binary);
				J_Delete_Boundary(Out_binary,1);
				findContours(Out_binary, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_NONE);

				if (contours.size() > 0)
				{
					for (int i = 0; i < contours.size(); i++)
					{
						//if contour[i] is not a hole
						//if (hierarchy[i][2] != -1 && hierarchy[i][3] == -1)
						{
							Rect t_rect = boundingRect(contours[i]);
							left = t_rect.x + BOLT_Param[Cam_num].nRect[s].x;
							top = t_rect.y + BOLT_Param[Cam_num].nRect[s].y;
							width = t_rect.width;
							height = t_rect.height;
							area = (int)(contourArea(contours[i]) + 0.5);

							if ((double)area >= BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] && (double)area <= BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s])
							{
								if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									rectangle(Dst_Img[Cam_num], Point(left,top), Point(left+width,top+height),  
										CV_RGB(0,255,0),1 );  

									int x = left+width/2; //중심좌표
									int y = top+height/2;

									msg.Format("%d",area);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(x-5,y-10), fontFace, fontScale, CV_RGB(0,100,255), 1, 8);
								}

								if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
								{
									if (left <= m_min_object_value)
									{
										m_min_object_value = left;
										m_min_object_num = i;
									}
								}
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
								{
									if (left + width >= m_max_object_value)
									{
										m_max_object_value = left + width;
										m_max_object_num = i;
									}
								}
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
								{
									double t_dist = sqrt((double)left*(double)left + (double)top*(double)top);
									if ((int)t_dist <= m_min_object_value)
									{
										m_min_object_value = (int)t_dist;
										m_min_object_num = i;
									}
								}
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
								{
									double t_dist = sqrt((double)left*(double)left + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
									if ((int)t_dist <= m_min_object_value)
									{
										m_min_object_value = (int)t_dist;
										m_min_object_num = i;
									}
								}
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
								{
									double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top)*(double)(top));
									if ((int)t_dist <= m_min_object_value)
									{
										m_min_object_value = (int)t_dist;
										m_min_object_num = i;
									}
								}
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
								{
									double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
									if ((int)t_dist <= m_min_object_value)
									{
										m_min_object_value = (int)t_dist;
										m_min_object_num = i;
									}
								}
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::CENTER)
								{
									if (area >= m_max_object_value)
									{
										m_max_object_value = area;
										m_max_object_num = i;
									}
								}		
								else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
								{
									if (area >= m_max_object_value)
									{
										m_max_object_value = area;
										m_max_object_num = i;
									}
								}	
							}
						} // end : if (hierarchy[i][3] == -1)
					} //  end : for (int i = 0; i < contours.size(); i++)

					if (m_min_object_num > -1)
					{
						BOLT_Param[Cam_num].Offset_Object_Postion = Point(0, 0);

						Rect t_rect = boundingRect(contours[m_min_object_num]);
						left = t_rect.x + BOLT_Param[Cam_num].nRect[s].x;
						top = t_rect.y + BOLT_Param[Cam_num].nRect[s].y;
						width = t_rect.width;
						height = t_rect.height;

						int x = left + width / 2;
						int y = top + height / 2;

						if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
						{
							x = left; y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
						{
							x = left + width; y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
						{
							x = left; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
						{
							x = left; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
						{
							x = left + width; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
						{
							x = left + width; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
						{
							x = left + width/2; y = top;
						}

						BOLT_Param[Cam_num].Object_Postion = Point(x, y);
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
								CV_RGB(255, 0, 0), 2);
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_min_object_num, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);

							circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 2, CV_RGB(255, 0, 0), 2);
							circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 1, CV_RGB(0, 0, 255), 1);
						}
					}

					if (m_max_object_num > -1)
					{
						BOLT_Param[Cam_num].Offset_Object_Postion = Point(0, 0);//기준점 초기화
						Rect t_rect = boundingRect(contours[m_max_object_num]);
						left = t_rect.x + BOLT_Param[Cam_num].nRect[s].x;
						top = t_rect.y + BOLT_Param[Cam_num].nRect[s].y;
						width = t_rect.width;
						height = t_rect.height;

						int x = left + width / 2;
						int y = top + height / 2;

						if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
						{
							x = left;y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
						{
							x = left + width; y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
						{
							x = left; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
						{
							x = left; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
						{
							x = left + width; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
						{
							x = left + width; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
						{
							x = left + width/2; y = top;
						}

						BOLT_Param[Cam_num].Object_Postion = Point(x, y);//기준점 등록

						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
								CV_RGB(255, 0, 0), 2);
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);

							circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 2, CV_RGB(255, 0, 0), 2);
							circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 1, CV_RGB(0, 0, 255), 1);
						}
					}
				} // end : if (contours.size() > 0)

				// 머리를 못 찾을 경우 Error 처리함.
				if(m_min_object_num == -1 && m_max_object_num == -1)
				{
					Result_Info[Cam_num] = "";
					for (int ss=1;ss<41;ss++)
					{
						CString t_CString;
						t_CString.Format("C%d:%02d_-1_",Cam_num,ss);
						Result_Info[Cam_num] += t_CString;
						//Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,ss);
					}
					rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
					msg.Format("No Object!");
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
					BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
					return true;
				}
			}
			else
			{// SIDE ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::CLASS_TYPE || BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE) //유리판일때
				{
					if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE) 
					{
						if (BOLT_Param[Cam_num].nThickness[0] > 0)
						{
							int t_filter_cnt = (int)(0.75*BOLT_Param[Cam_num].nThickness[0]/BOLT_Param[Cam_num].nResolution[0]);
							erode(Out_binary,Out_binary,element_v,Point(-1,-1),t_filter_cnt);
							dilate(Out_binary,Out_binary,element_v,Point(-1,-1),t_filter_cnt);
						}
					}

					//if (BOLT_Param[Cam_num].nROI0_FilterSize[0] > 0)
					//{
					//	erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
					//	dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
					//}

					// 사이드 상부 바닦 찾기
					double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;double t_angle = 0;
					int t_Search_Range = 10;
					for (int i = 0;i<t_Search_Range;i++)
					{
						for (int j = 0;j<Out_binary.rows;j++)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_Left_x +=(double)i;
								t_Left_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_Left_x/=t_cnt;
						t_Left_y/=t_cnt;
					}
					else
					{
						t_Left_y = Out_binary.rows-1;
					}

					double t_Right_x = 0;double t_Right_y = 0;t_cnt = 0;
					for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
					{
						for (int j = 0;j<Out_binary.rows;j++)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_Right_x +=(double)i;
								t_Right_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_Right_x/=t_cnt;
						t_Right_y/=t_cnt;
					}
					else
					{
						t_Right_x = Out_binary.cols -1;
						t_Right_y = Out_binary.rows -1;
					}

					t_Left_x = 0;
					t_Right_x = Out_binary.cols-1;
					if (abs(t_Left_y - t_Right_y) > (double)50)
					{
						double t_LR_max = max(t_Left_y,t_Right_y);
						t_Left_y = t_LR_max;
						t_Right_y = t_LR_max;
					}

					if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 2)
					{// 유리판 기준 회전 보정
						t_angle = f_angle360(Point(t_Left_x,t_Left_y),Point(t_Right_x,t_Right_y));
						t_angle -= 180;

						Mat Rotate_Gray_Img;
						//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
						J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Rotate_Gray_Img,1);
						BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
						cvtColor(Rotate_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

						// 임계화
						// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
						if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
						{
							threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
						}
						else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
						{
							threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
						}
						else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
						{
							inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
						}
						else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
						{
							Mat White_Out_binary = Mat::ones(BOLT_Param[Cam_num].Gray_Obj_Img.rows, BOLT_Param[Cam_num].Gray_Obj_Img.cols, CV_8U)*255;
							inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
							subtract(White_Out_binary, Out_binary, Out_binary);
						}
						else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
						{
							threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
						}
						else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
						{
							threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
						}
						else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
						{
							blur(BOLT_Param[Cam_num].Gray_Obj_Img, Out_binary, Size(3,3));
							// Run the edge detector on grayscale
							Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
						}


						if (BOLT_Param[Cam_num].nROI0_FilterSize[0] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[0]);
						}

						t_Left_x = 0;t_Left_y = 0;t_cnt = 0;
						for (int i = 0;i<t_Search_Range;i++)
						{
							for (int j = 0;j<Out_binary.rows;j++)
							{
								if (Out_binary.at<uchar>(j,i) > 0)
								{
									t_Left_x +=(double)i;
									t_Left_y += (double)j;
									t_cnt++;
									break;
								}
							}
						}
						if (t_cnt > 0)
						{
							t_Left_x/=t_cnt;
							t_Left_y/=t_cnt;
						}
						else
						{
							t_Left_y = Out_binary.rows-1;
						}

						t_Right_x = 0;t_Right_y = 0;t_cnt = 0;
						for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
						{
							for (int j = 0;j<Out_binary.rows;j++)
							{
								if (Out_binary.at<uchar>(j,i) > 0)
								{
									t_Right_x +=(double)i;
									t_Right_y += (double)j;
									t_cnt++;
									break;
								}
							}
						}
						if (t_cnt > 0)
						{
							t_Right_x/=t_cnt;
							t_Right_y/=t_cnt;
						}
						else
						{
							t_Right_x = Out_binary.cols -1;
							t_Right_y = Out_binary.rows -1;
						}


						t_Left_x = 0;
						t_Right_x = Out_binary.cols-1;
						if (abs(t_Left_y - t_Right_y) > (double)50)
						{
							double t_LR_max = max(t_Left_y,t_Right_y);
							t_Left_y = t_LR_max;
							t_Right_y = t_LR_max;
						}

						if (t_Left_x >= 0 && t_Left_y > 0 && t_Right_x >= 0 && t_Right_y > 0)
						{
							line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
						}
					}
					else 
					{
						if (t_Left_x >= 0 && t_Left_y > 0 && t_Right_x >= 0 && t_Right_y > 0)
						{
							line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
						}
					}

					//if (Result_Debugging) // 오링 사이드 바닦 라인 이미지
					//{
					//	msg.Format("Save\\Debugging\\Cam%d_%2d_Line_Thres_ROI_Gray_Img.bmp",Cam_num, s);
					//	imwrite(msg.GetBuffer(),Out_binary);		
					//}


					erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
					dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);
					J_Delete_Boundary(Out_binary,1);

					if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
					{
						erode(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
						dilate(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
					}
					// 못 머리만 찾기
					findContours( Out_binary.clone(), BOLT_Param[Cam_num].Object_contours, BOLT_Param[Cam_num].Object_hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if(BOLT_Param[Cam_num].Object_contours.size() == 0) // 칸투어 갯수로 예외처리
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No Object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
						return true;
					}

					vector<Rect> boundRect( BOLT_Param[Cam_num].Object_contours.size() );
					int m_max_object_num = -1;int m_max_object_value = 0;
					for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
					{  
						boundRect[k] = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
						if (m_max_object_value <= boundRect[k].width*boundRect[k].height)
							//&& pointPolygonTest( BOLT_Param[Cam_num].Object_contours[k], Point(boundRect[k].x+boundRect[k].width/2,boundRect[k].y+boundRect[k].height/2), false ) == 1)
						{
							m_max_object_value = boundRect[k].width*boundRect[k].height;
							m_max_object_num = k;
						}
					}

					BOLT_Param[Cam_num].Object_1st_idx = m_max_object_num;

					// 머리를 못 찾을 경우 Error 처리함.
					if(m_max_object_num == -1)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No Object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
						return true;
					}

					if (m_max_object_num >= 0)
					{
						BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
						int left = boundRect[m_max_object_num].x;
						int top = boundRect[m_max_object_num].y;
						int width = boundRect[m_max_object_num].width;
						int height = boundRect[m_max_object_num].height;

						if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 1)
						{// 나사 몸통 기준 회전 보정
							J_Delete_Boundary(Out_binary,1);
							J_Fill_Hole(Out_binary);
							if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
							{
								erode(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1),BOLT_Param[Cam_num].nAngleHeightFilterSize[0] + BOLT_Param[Cam_num].nAngleHeightFilterSize[0]/4);
								erode(Out_binary,Out_binary,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]/4);
							}
							Point2f t_Top_Center(0,0);
							Point2f t_Bot_Center(0,0);

							double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;
							double t_B_Bottom_x = 0;double t_B_Bottom_y = 0;double t_Bottom_cnt = 0;
							int t_height = BOLT_Param[Cam_num].nAngleHeightHeight[0];
							if (t_height > height)
							{
								t_height = height;
							}

							Vec4f lines;
							vector<Point2f> vec_2center;

							for (int j = top+height/4;j<=top+t_height;j++)
							{
								t_B_Top_x = t_B_Top_y = t_Top_cnt = 0;
								for (int i = left;i<left+width;i++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_B_Top_x +=(double)i;
										t_B_Top_y += (double)j;
										t_Top_cnt++;
									}
								}
								if (t_Top_cnt > 0)
								{
									t_B_Top_x/=t_Top_cnt;
									t_B_Top_y/=t_Top_cnt;
									t_Top_Center.x = t_B_Top_x;
									t_Top_Center.y = t_B_Top_y;
									vec_2center.push_back(t_Top_Center);
								}
							}
							if (vec_2center.size() > 2 )
							{
								fitLine(vec_2center,lines, CV_DIST_L2,0,0.001,0.001);
								t_angle = (180.0/CV_PI)*atan(lines[1]/lines[0]);
								if (t_angle < 0)
								{
									t_angle += 90;
								}
								else if (t_angle > 0)
								{
									t_angle -= 90;
								}
								//msg.Format("%1.3f",t_angle-90);
								//AfxMessageBox(msg);
							}
/*
							for (int i = left;i<left+width;i++)
							{
								for (int j = top-t_height/20+t_height/3;j<=top+t_height/20+t_height/3;j++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_B_Top_x +=(double)i;
										t_B_Top_y += (double)j;
										t_Top_cnt++;
										break;
									}
								}
								for (int j = top-t_height/20+2*t_height/3;j<=top+t_height/20+2*t_height/3;j++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_B_Bottom_x +=(double)i;
										t_B_Bottom_y += (double)j;
										t_Bottom_cnt++;
										break;
									}
								}
							}
							if (t_Top_cnt > 0)
							{
								t_B_Top_x/=t_Top_cnt;
								t_B_Top_y/=t_Top_cnt;
								t_Top_Center.x = t_B_Top_x;
								t_Top_Center.y = t_B_Top_y;
							}
							if (t_Bottom_cnt > 0)
							{
								t_B_Bottom_x/=t_Bottom_cnt;
								t_B_Bottom_y/=t_Bottom_cnt;
								t_Bot_Center.x = t_B_Bottom_x;
								t_Bot_Center.y = t_B_Bottom_y;
							}

							double t_angle = f_angle360(t_Top_Center, t_Bot_Center);

							if (t_angle == 180 || t_angle == 0)
							{
								t_angle = 0;
							}
							else
							{
								t_angle += 90-360;
							}
							int m_max_object_num = -1;int m_max_object_value = 0;

							if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 2)
							{
								t_angle = f_angle360(Point(0,t_Left_y+1),Point(Out_binary.cols,t_Right_y+1));
								t_angle -= 180;
							}
*/
							Mat Rotate_Gray_Img;
							//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
							J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Rotate_Gray_Img,1);
							BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
							cvtColor(Rotate_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);


							// 임계화
							// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
							if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
							{
								threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
							{
								threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
							{
								inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
							{
								Mat White_Out_binary = Mat::ones(BOLT_Param[Cam_num].Gray_Obj_Img.rows, BOLT_Param[Cam_num].Gray_Obj_Img.cols, CV_8U)*255;
								inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
								subtract(White_Out_binary, Out_binary, Out_binary);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
							{
								threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
							{
								threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
							}
							else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
							{
								blur(BOLT_Param[Cam_num].Gray_Obj_Img, Out_binary, Size(3,3));
								// Run the edge detector on grayscale
								Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
							}

							t_Left_x = 0;t_Left_y = 0;t_cnt = 0;
							for (int i = 0;i<t_Search_Range;i++)
							{
								for (int j = 0;j<Out_binary.rows;j++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_Left_x +=(double)i;
										t_Left_y += (double)j;
										t_cnt++;
										break;
									}
								}
							}
							if (t_cnt > 0)
							{
								t_Left_x/=t_cnt;
								t_Left_y/=t_cnt;
							}
							else
							{
								t_Left_y = Out_binary.rows-1;
							}

							t_Right_x = 0;t_Right_y = 0;t_cnt = 0;
							for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
							{
								for (int j = 0;j<Out_binary.rows;j++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_Right_x +=(double)i;
										t_Right_y += (double)j;
										t_cnt++;
										break;
									}
								}
							}
							if (t_cnt > 0)
							{
								t_Right_x/=t_cnt;
								t_Right_y/=t_cnt;
							}
							else
							{
								t_Right_x = Out_binary.cols -1;
								t_Right_y = Out_binary.rows -1;
							}

							if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
							{
								line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
							}

							erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
							dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);
							J_Delete_Boundary(Out_binary,1);
							J_Fill_Hole(Out_binary);
							//imwrite("00.bmp",Out_binary);
							// 못 머리만 찾기
							BOLT_Param[Cam_num].Object_contours.clear();
							BOLT_Param[Cam_num].Object_hierarchy.clear();
							J_Delete_Boundary(Out_binary,1);
							findContours( Out_binary.clone(), BOLT_Param[Cam_num].Object_contours, BOLT_Param[Cam_num].Object_hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							//if(BOLT_Param[Cam_num].Object_contours.size() == 0) // 칸투어 갯수로 예외처리
							//{
							//	rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
							//	msg.Format("No Object!");
							//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
							//	return true;
							//}
								//AfxMessageBox("0");

							if (BOLT_Param[Cam_num].Object_contours.size() > 0)
							{
								vector<Rect> boundRect1( BOLT_Param[Cam_num].Object_contours.size() );
								m_max_object_num = -1;m_max_object_value = 0;
								for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
								{  
									boundRect1[k] = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
									if (m_max_object_value <= boundRect1[k].width*boundRect1[k].height && boundRect1[k].y + boundRect1[k].height <= Out_binary.rows-1)
										//&& pointPolygonTest( BOLT_Param[Cam_num].Object_contours[k], Point(boundRect[k].x+boundRect[k].width/2,boundRect[k].y+boundRect[k].height/2), false ) == 1)
									{
										m_max_object_value = boundRect1[k].width*boundRect1[k].height;
										m_max_object_num = k;
									}
								}

								BOLT_Param[Cam_num].Object_1st_idx = m_max_object_num;

								if (m_max_object_num >= 0)
								{
									left = boundRect1[m_max_object_num].x;
									top = boundRect1[m_max_object_num].y;
									width = boundRect1[m_max_object_num].width;
									height = boundRect1[m_max_object_num].height;
									//AfxMessageBox("1");
									int x = left + width / 2;
									int y = top + height / 2;

									if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
									{
										x = left;y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
									{
										x = left + width; y = top + height / 2;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
									{
										x = left; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
									{
										x = left; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
									{
										x = left + width; y = top;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
									{
										x = left + width; y = top + height;
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
									{
										x = left + width/2; y = top;
									}

									BOLT_Param[Cam_num].Object_Postion = Point(x, y);//기준점 등록

									//BOLT_Param[Cam_num].Object_Postion = Point(boundRect[m_max_object_num].x,boundRect[m_max_object_num].y);
									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
									{
										//msg.Format("%d",Cam_num);
										//AfxMessageBox(msg);
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
										//imwrite("01.bmp",Dst_Img[Cam_num]);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
									}
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
									}
									return true;
								}
							}
						}

						int x = left + width / 2;
						int y = top + height / 2;

						if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
						{
							x = left;y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
						{
							x = left + width; y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
						{
							x = left; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
						{
							x = left; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
						{
							x = left + width; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
						{
							x = left + width; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
						{
							x = left + width/2; y = top;
						}

						BOLT_Param[Cam_num].Object_Postion = Point(x, y);//기준점 등록

						//BOLT_Param[Cam_num].Object_Postion = Point(boundRect[m_max_object_num].x,boundRect[m_max_object_num].y);
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
						{
							//msg.Format("%d",Cam_num);
							//AfxMessageBox(msg);
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
							//imwrite("01.bmp",Dst_Img[Cam_num]);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
						}
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
						}
					}

					if (Result_Debugging)
					{
						msg.Format("Save\\Debugging\\CAM%d_Base_Find_Obj_Gray_Img.bmp",Cam_num);
						imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
						msg.Format("Save\\Debugging\\CAM%d_Base_Find_Obj_Thres_Img.bmp",Cam_num);
						imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
					}
				}
				else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::INDEX_TYPE) //INDEX판일때
				{
					// 사이드 상부 바닦 찾기
					double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;
					int t_Search_Range = 10;
					for (int i = 0;i<t_Search_Range;i++)
					{
						for (int j = 0;j<Out_binary.rows;j++)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_Left_x +=(double)i;
								t_Left_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_Left_x/=t_cnt;
						t_Left_y/=t_cnt;
					}

					double t_Right_x = 0;double t_Right_y = 0;t_cnt = 0;
					for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
					{
						for (int j = 0;j<Out_binary.rows;j++)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_Right_x +=(double)i;
								t_Right_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_Right_x/=t_cnt;
						t_Right_y/=t_cnt;
					}

					t_Left_x = 0;
					t_Right_x = Out_binary.cols;
					if (abs(t_Left_y - t_Right_y) > 50)
					{
						double t_temp = max(t_Left_y,t_Right_y);
						t_Left_y = t_temp;
						t_Right_y = t_temp;
					}

					// 사이드 하부 바닦 찾기
					double t_B_Left_x = 0;double t_B_Left_y = 0;t_cnt = 0;
					for (int i = 0;i<t_Search_Range;i++)
					{
						for (int j = Out_binary.rows-1;j>0;j--)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_B_Left_x +=(double)i;
								t_B_Left_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_B_Left_x/=t_cnt;
						t_B_Left_y/=t_cnt;
					}

					double t_B_Right_x = 0;double t_B_Right_y = 0;t_cnt = 0;
					for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
					{
						for (int j = Out_binary.rows-1;j>0;j--)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_B_Right_x +=(double)i;
								t_B_Right_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_B_Right_x/=t_cnt;
						t_B_Right_y/=t_cnt;
					}

					if (abs(t_B_Left_y - t_B_Right_y) > 50)
					{
						double t_temp = min(t_B_Left_y,t_B_Right_y);
						if (t_temp == t_B_Left_y)
						{
							t_B_Left_y = t_temp;
							t_B_Right_y = t_B_Left_y + abs(t_Left_y - t_Right_y) + 1;
						}
						else
						{
							t_B_Right_y = t_temp;
							t_B_Left_y = t_B_Right_y + abs(t_Right_y - t_Left_y) + 1;
						}
					}

					Mat t_morph = Out_binary.clone();
					Rect tt_Sub_ROI;
					tt_Sub_ROI.x = 0;tt_Sub_ROI.y = max(t_Left_y,t_Right_y);tt_Sub_ROI.width = Out_binary.cols;tt_Sub_ROI.height = Out_binary.rows - max(t_Left_y,t_Right_y) - 1;

					//if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
					//{
					//	erode(Out_binary(tt_Sub_ROI),t_morph(tt_Sub_ROI),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
					//	dilate(t_morph(tt_Sub_ROI),t_morph(tt_Sub_ROI),element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
					//}

					if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
					{
						line(t_morph,Point(0,t_Left_y+1),Point(Out_binary.cols,t_Right_y+1),CV_RGB(0,0,0),2);
					}
					if (t_B_Left_x > 0 && t_B_Left_y > 0 && t_B_Right_x > 0 && t_B_Right_y > 0)
					{
						line(t_morph,Point(0,t_B_Left_y+1),Point(Out_binary.cols,t_B_Right_y+1),CV_RGB(0,0,0),2);
					}

					//imwrite("00.bmp",t_morph);

					erode(t_morph,t_morph,element_v,Point(-1,-1), 1);
					dilate(t_morph,t_morph,element_v,Point(-1,-1), 1);

					Mat stats, centroids, label;  
					int numOfLables = connectedComponentsWithStats(t_morph, label, stats, centroids, 8,CV_32S);
					int m_Head_Idx = -1; int m_Head_value = 0;
					int m_Body_Idx = -1; int m_Body_value = 0;
					int area, left, top, width, height = { 0, };
					for (int j = 1; j < numOfLables; j++)
					{
						area = stats.at<int>(j, CC_STAT_AREA);
						left = stats.at<int>(j, CC_STAT_LEFT);
						top = stats.at<int>(j, CC_STAT_TOP);
						width = stats.at<int>(j, CC_STAT_WIDTH);
						height = stats.at<int>(j, CC_STAT_HEIGHT);

						if ((double)(top + height) <=  max(t_Left_y, t_Right_y) && area >= m_Head_value && left > 1 && left+width < t_morph.cols-1)
						{ // 머리부 찾기
							m_Head_value = area;
							m_Head_Idx = j;
						}
						if ((double)(top) >=  min(t_B_Left_y, t_B_Right_y) && area >= m_Body_value && left > 1 && left+width < t_morph.cols-1)
						{ // 바디부 찾기
							m_Body_value = area;
							m_Body_Idx = j;
						}
					}

					Point2f t_Head_Center(0,0);
					Point2f t_Body_Center(0,0);
					if (m_Head_Idx > -1 && m_Body_Idx > -1)
					{
						t_Head_Center.x = (float)stats.at<int>(m_Head_Idx, CC_STAT_LEFT) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_WIDTH))/2; //중심좌표		
						t_Head_Center.y = (float)stats.at<int>(m_Head_Idx, CC_STAT_TOP) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_HEIGHT))/2;
						//t_Head_Center.x = centroids.at<double>(m_Head_Idx, 0); //중심좌표		
						//t_Head_Center.y = centroids.at<double>(m_Head_Idx, 1);
						t_Body_Center.x = centroids.at<double>(m_Body_Idx, 0); //중심좌표		
						t_Body_Center.y = centroids.at<double>(m_Body_Idx, 1);
					}
					else
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No Object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
						return true;
					}

					//imwrite("01.bmp",Dst_Img[Cam_num]);

					if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 1 && m_Body_Idx > -1)
					{// 하부쪽 기준 회전
						left = stats.at<int>(m_Body_Idx, CC_STAT_LEFT);
						top = stats.at<int>(m_Body_Idx, CC_STAT_TOP);
						width = stats.at<int>(m_Body_Idx, CC_STAT_WIDTH);
						height = stats.at<int>(m_Body_Idx, CC_STAT_HEIGHT);

						double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;
						double t_B_Bottom_x = 0;double t_B_Bottom_y = 0;double t_Bottom_cnt = 0;
						for (int i = left;i<left+width;i++)
						{
							for (int j = top-2+height/4;j<=top+2+height/4;j++)
							{
								if (Out_binary.at<uchar>(j,i) > 0)
								{
									t_B_Top_x +=(double)i;
									t_B_Top_y += (double)j;
									t_Top_cnt++;
									break;
								}
							}
							for (int j = top-2+3*height/4;j<=top+2+3*height/4;j++)
							{
								if (Out_binary.at<uchar>(j,i) > 0)
								{
									t_B_Bottom_x +=(double)i;
									t_B_Bottom_y += (double)j;
									t_Bottom_cnt++;
									break;
								}
							}
						}
						if (t_Top_cnt > 0)
						{
							t_B_Top_x/=t_Top_cnt;
							t_B_Top_y/=t_Top_cnt;
							t_Head_Center.x = t_B_Top_x;
							t_Head_Center.y = t_B_Top_y;
						}
						if (t_Bottom_cnt > 0)
						{
							t_B_Bottom_x/=t_Bottom_cnt;
							t_B_Bottom_y/=t_Bottom_cnt;
							t_Body_Center.x = t_B_Bottom_x;
							t_Body_Center.y = t_B_Bottom_y;
						}
					}

					double t_angle = f_angle360(t_Head_Center, t_Body_Center);


					if (t_angle == 180 || t_angle == 0)
					{
						t_angle = 0;
					}
					else
					{
						t_angle += 90-360;
					}
					int m_max_object_num = -1;int m_max_object_value = 0;

					if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 2)
					{
						t_angle = f_angle360(Point(0,t_Left_y+1),Point(Out_binary.cols,t_Right_y+1));
						t_angle -= 180;
						//if (t_angle > 180)
						//{
						//	t_angle -= 180;
						//}
						//if (t_angle <= 180)
						//{
						//	t_angle -= 180;
						//}
						//msg.Format("%1.2f",t_angle);
						//AfxMessageBox(msg);
					}

					Mat Rotate_Gray_Img;
					//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
					J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Rotate_Gray_Img,1);
					BOLT_Param[Cam_num].Gray_Obj_Img = Rotate_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
					cvtColor(Rotate_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

					// 임계화
					// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
					if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
					{
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
					}
					else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
					{
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
					}
					else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
					{
						inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
					}
					else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
					{
						Mat White_Out_binary = Mat::ones(BOLT_Param[Cam_num].Gray_Obj_Img.rows, BOLT_Param[Cam_num].Gray_Obj_Img.cols, CV_8U)*255;
						inRange(BOLT_Param[Cam_num].Gray_Obj_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
						subtract(White_Out_binary, Out_binary, Out_binary);
					}
					else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
					{
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
					}
					else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
					{
						threshold(BOLT_Param[Cam_num].Gray_Obj_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
					}
					else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
					{
						blur(BOLT_Param[Cam_num].Gray_Obj_Img, Out_binary, Size(3,3));
						// Run the edge detector on grayscale
						Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
					}
					//imwrite("02-0.bmp",BOLT_Param[Cam_num].Gray_Obj_Img);
					//imwrite("02.bmp",Out_binary);
					// 사이드 상부 바닦 찾기
					t_Left_x = 0;t_Left_y = 0;t_cnt = 0;
					t_Search_Range = 10;
					for (int i = 0;i<t_Search_Range;i++)
					{
						for (int j = 0;j<Out_binary.rows;j++)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_Left_x +=(double)i;
								t_Left_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_Left_x/=t_cnt;
						t_Left_y/=t_cnt;
					}

					t_Right_x = 0;t_Right_y = 0;t_cnt = 0;
					for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
					{
						for (int j = 0;j<Out_binary.rows;j++)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_Right_x +=(double)i;
								t_Right_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_Right_x/=t_cnt;
						t_Right_y/=t_cnt;
					}
									
					if (t_Left_y  < 10 || t_Right_y < 10)
					{
						double t_temp = max(t_Left_y,t_Right_y);
						t_Left_y = t_temp;
						t_Right_y = t_temp;
					}

					// 사이드 하부 바닦 찾기
					t_B_Left_x = 0;t_B_Left_y = 0;t_cnt = 0;
					for (int i = 0;i<t_Search_Range;i++)
					{
						for (int j = Out_binary.rows-1;j>0;j--)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_B_Left_x +=(double)i;
								t_B_Left_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_B_Left_x/=t_cnt;
						t_B_Left_y/=t_cnt;
					}

					t_B_Right_x = 0;t_B_Right_y = 0;t_cnt = 0;
					for (int i = Out_binary.cols-1;i>=Out_binary.cols - t_Search_Range-1;i--)
					{
						for (int j = Out_binary.rows-1;j>0;j--)
						{
							if (Out_binary.at<uchar>(j,i) > 0)
							{
								t_B_Right_x +=(double)i;
								t_B_Right_y += (double)j;
								t_cnt++;
								break;
							}
						}
					}
					if (t_cnt > 0)
					{
						t_B_Right_x/=t_cnt;
						t_B_Right_y/=t_cnt;
					}
										
					if (abs(t_B_Left_y - t_B_Right_y) > 50)
					{
						double t_temp = min(t_B_Left_y,t_B_Right_y);

						if (t_temp == t_B_Left_y)
						{
							t_B_Left_y = t_temp;
							t_B_Right_y = t_B_Left_y + abs(t_Left_y - t_Right_y) + 1;
						}
						else
						{
							t_B_Right_y = t_temp;
							t_B_Left_y = t_B_Right_y + abs(t_Right_y - t_Left_y) + 1;
						}
					}

					//imwrite("00.bmp",Out_binary);
					vector< vector<Point> >  co_ordinates;
					co_ordinates.push_back(vector<Point>());
					co_ordinates[0].push_back(Point(0,t_Left_y));
					co_ordinates[0].push_back(Point(Out_binary.cols-1,t_Right_y));
					co_ordinates[0].push_back(Point(Out_binary.cols-1,t_B_Right_y));
					co_ordinates[0].push_back(Point(0,t_B_Left_y));
					drawContours( Out_binary,co_ordinates,0, Scalar(0),CV_FILLED, 8 );
					//imwrite("01.bmp",Out_binary);
					tt_Sub_ROI.x = 0;tt_Sub_ROI.y = min(t_Left_y,t_Right_y)-1;tt_Sub_ROI.width = Out_binary.cols;tt_Sub_ROI.height = max((t_B_Left_y - t_Left_y),(t_B_Right_y - t_Right_y))+10;
					erode(Out_binary(tt_Sub_ROI),Out_binary(tt_Sub_ROI),element_v,Point(-1,-1), 1);
					dilate(Out_binary(tt_Sub_ROI),Out_binary(tt_Sub_ROI),element_v,Point(-1,-1), 1);
					//imwrite("02.bmp",Out_binary);
					J_Delete_Boundary(Out_binary,1);
					numOfLables = connectedComponentsWithStats(Out_binary, label, stats, centroids, 8,CV_32S);
					m_Head_Idx = -1; m_Head_value = 0;
					m_Body_Idx = -1; m_Body_value = 0;
					for (int j = 1; j < numOfLables; j++)
					{
						area = stats.at<int>(j, CC_STAT_AREA);
						left = stats.at<int>(j, CC_STAT_LEFT);
						top = stats.at<int>(j, CC_STAT_TOP);
						width = stats.at<int>(j, CC_STAT_WIDTH);
						height = stats.at<int>(j, CC_STAT_HEIGHT);

						if ((double)(top + height) <=  max(t_Left_y, t_Right_y)+2 && area >= m_Head_value && left > 1 && left+width < Out_binary.cols-1)
						{ // 머리부 찾기
							m_Head_value = area;
							m_Head_Idx = j;
						}
						if ((double)(top) >=  min(t_B_Left_y, t_B_Right_y)-2 && area >= m_Body_value && left > 1 && left+width < Out_binary.cols-1)
						{ // 바디부 찾기
							m_Body_value = area;
							m_Body_Idx = j;
						}
						if (left < 2 || left+width > Out_binary.cols-3)
						{
							for (int y = 0; y < Out_binary.rows; ++y) {
								int *tlabel = label.ptr<int>(y);
								uchar* pixel = Out_binary.ptr<uchar>(y);
								uchar* pixel2 = BOLT_Param[Cam_num].Gray_Obj_Img.ptr<uchar>(y);
								for (int x = 0; x < Out_binary.cols; ++x) {
									if (tlabel[x] == j)
									{
										pixel[x] = 0;
										pixel2[x] = 255;
									}
								}
							}
						}
					}

					//imwrite("03.bmp",Out_binary);
					//imwrite("04.bmp",BOLT_Param[Cam_num].Gray_Obj_Img);
					if (m_Head_Idx > -1 && m_Body_Idx > -1)
					{
						for (int y = 0; y < Out_binary.rows; ++y) {
							int *tlabel = label.ptr<int>(y);
							uchar* pixel = Out_binary.ptr<uchar>(y);
							for (int x = 0; x < Out_binary.cols; ++x) {
								if (tlabel[x] != m_Head_Idx && tlabel[x] != m_Body_Idx)
								{
									pixel[x] = 0;
								}
							}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							for (int y = 0; y < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).rows; ++y) {
								int *tlabel = label.ptr<int>(y);
								Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(y);
								for (int x = 0; x < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).cols; ++x) {
									if (tlabel[x] == m_Head_Idx)
									{
										pixel[x][2] = 0;  
										pixel[x][1] = 255;
										pixel[x][0] = 0;
									}
									else if (tlabel[x] == m_Body_Idx)
									{
										pixel[x][2] = 0;  
										pixel[x][1] = 255;
										pixel[x][0] = 0;
									}
								}
							}
						}

						t_Head_Center.x = (float)stats.at<int>(m_Head_Idx, CC_STAT_LEFT) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_WIDTH))/2; //중심좌표		
						t_Head_Center.y = (float)stats.at<int>(m_Head_Idx, CC_STAT_TOP) + ((float)stats.at<int>(m_Head_Idx, CC_STAT_HEIGHT))/2;
						//t_Head_Center.x = centroids.at<double>(m_Head_Idx, 0); //중심좌표		
						//t_Head_Center.y = centroids.at<double>(m_Head_Idx, 1);
						t_Body_Center.x = centroids.at<double>(m_Body_Idx, 0); //중심좌표		
						t_Body_Center.y = centroids.at<double>(m_Body_Idx, 1);

						BOLT_Param[Cam_num].Object_Postion = t_Head_Center;

						if (BOLT_Param[Cam_num].nAngleCalMethod[s] == 1 && m_Body_Idx > -1)
						{// 하부쪽 기준 회전
							left = stats.at<int>(m_Body_Idx, CC_STAT_LEFT);
							top = stats.at<int>(m_Body_Idx, CC_STAT_TOP);
							width = stats.at<int>(m_Body_Idx, CC_STAT_WIDTH);
							height = stats.at<int>(m_Body_Idx, CC_STAT_HEIGHT);

							double t_B_Top_x = 0;double t_B_Top_y = 0;double t_Top_cnt = 0;
							double t_B_Bottom_x = 0;double t_B_Bottom_y = 0;double t_Bottom_cnt = 0;
							for (int i = left;i<left+width;i++)
							{
								for (int j = top-2+height/4;j<=top+2+height/4;j++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_B_Top_x +=(double)i;
										t_B_Top_y += (double)j;
										t_Top_cnt++;
										break;
									}
								}
								for (int j = top-2+3*height/4;j<=top+2+3*height/4;j++)
								{
									if (Out_binary.at<uchar>(j,i) > 0)
									{
										t_B_Bottom_x +=(double)i;
										t_B_Bottom_y += (double)j;
										t_Bottom_cnt++;
										break;
									}
								}
							}
							if (t_Top_cnt > 0)
							{
								t_B_Top_x/=t_Top_cnt;
								t_B_Top_y/=t_Top_cnt;
								t_Head_Center.x = t_B_Top_x;
								t_Head_Center.y = t_B_Top_y;
							}
							if (t_Bottom_cnt > 0)
							{
								t_B_Bottom_x/=t_Bottom_cnt;
								t_B_Bottom_y/=t_Bottom_cnt;
								t_Body_Center.x = t_B_Bottom_x;
								t_Body_Center.y = t_B_Bottom_y;
							}
						}
						if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,t_Body_Center,CV_RGB(255,200,0),1,8);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,2,CV_RGB(255,0,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_Center,1,CV_RGB(0,0,255),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Body_Center,2,CV_RGB(255,0,0),2);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Body_Center,1,CV_RGB(0,0,255),1);
						}
					}
					BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
					BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
				}
				else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::NONE_TYPE) //가이드 없을 때
				{
					Mat t_morph = Out_binary.clone();
					if (BOLT_Param[Cam_num].nAngleHeightFilterSize[0] > 0)
					{
						erode(Out_binary,t_morph,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
						dilate(t_morph,t_morph,element_v,Point(-1,-1), BOLT_Param[Cam_num].nAngleHeightFilterSize[0]);
					}
					//J_Delete_Boundary(Out_binary,1);

					// 나사 회전량 계산
					Rect t_Sub_ROI;
					t_Sub_ROI.x = 0;t_Sub_ROI.y = BOLT_Param[Cam_num].nAngleHeightTop[0];t_Sub_ROI.width = Out_binary.cols;t_Sub_ROI.height = BOLT_Param[Cam_num].nAngleHeightHeight[0];
					if (t_Sub_ROI.height == 0)
					{
						t_Sub_ROI.height = 1;
					}

					if (t_Sub_ROI.x < 0)
					{
						t_check_roi = false;
					}
					if (t_Sub_ROI.y < 0)
					{
						t_check_roi = false;
					}
					if (t_Sub_ROI.x + t_Sub_ROI.width > Out_binary.cols)
					{
						t_check_roi = false;
					}
					if (t_Sub_ROI.y + t_Sub_ROI.height > Out_binary.rows)
					{
						t_check_roi = false;
					}

					if (!t_check_roi)
					{
						AfxMessageBox("Exeed rotation height value!");
						return false;
					}
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,255,0),1);
					}
					//msg.Format("Save\\Debugging\\CAM%d_t_morph.bmp",Cam_num);
					//imwrite(msg.GetBuffer(),t_morph(t_Sub_ROI));
					double t_angle=Cal_Angle(t_morph(t_Sub_ROI), Cam_num);
					if (t_angle == 180)
					{
						t_angle = 0;
					}
					else
					{
						t_angle += 90-360;
					}

					if (BOLT_Param[Cam_num].nAngleHeightHeight[0] == 0)
					{
						t_angle = 0;
					}



					int m_max_object_num = -1;int m_max_object_value = 0;

					Mat Target_Thres_ROI_Gray_Img;
					//J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
					//J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
					//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);

					//Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();

					if (t_angle == 0)
					{
						Target_Thres_ROI_Gray_Img = Gray_Img[Cam_num].clone();
					}
					else
					{
						J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Target_Thres_ROI_Gray_Img,1);
					}
					BOLT_Param[Cam_num].Gray_Obj_Img = Target_Thres_ROI_Gray_Img(BOLT_Param[Cam_num].nRect[s]).clone();
					cvtColor(Target_Thres_ROI_Gray_Img,Dst_Img[Cam_num],CV_GRAY2BGR);

					if (t_angle == 0)
					{
						Target_Thres_ROI_Gray_Img = Out_binary.clone();
					}
					else
					{
						J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
					}

					//J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
					//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);

					Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();
					//J_Rotate_PRE(Out_binary,t_angle,Rotate_Target_Thres_ROI_Gray_Img,1);

					if (Result_Debugging) // 못 사이드 임계화 Image Save
					{
						msg.Format("Save\\Debugging\\CAM%d_Base_Obj_Target_Binary_Img.bmp",Cam_num);
						imwrite(msg.GetBuffer(),Target_Thres_ROI_Gray_Img);		
					}

					if (m_Text_View[Cam_num]) // ROI 영역 표시
					{
						//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
						rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
						msg.Format("Obj. ROI");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,0,255), 1, 8);
					}

					if (Result_Debugging) // 못 사이드 회전 Image Save
					{
						msg.Format("Save\\Debugging\\CAM%d_Base_Obj_Rotate_Binary_Img.bmp",Cam_num);
						imwrite(msg.GetBuffer(),Rotate_Target_Thres_ROI_Gray_Img);		
					}


					BOLT_Param[Cam_num].Thres_Obj_Img = Rotate_Target_Thres_ROI_Gray_Img.clone();

					// 해드와 사이드 합침
					findContours( Rotate_Target_Thres_ROI_Gray_Img.clone(), BOLT_Param[Cam_num].Object_contours, BOLT_Param[Cam_num].Object_hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					if (BOLT_Param[Cam_num].Object_contours.size() == 0)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No Object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);

						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,100,0),2);
						msg.Format("Bolt body must be in orange box!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,Dst_Img[Cam_num].rows-20), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 2, 8);
						return true;
					}
					//AfxMessageBox("1");
					m_max_object_num = -1;
					m_max_object_value = 0;
					Rect tt_rect;Rect t_Side_rect;
					for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
					{  
						tt_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
						if (m_max_object_value<= tt_rect.width*tt_rect.height 
							)//&& tt_rect.x >= 1 && tt_rect.y >= 0 && tt_rect.x + tt_rect.width < Out_binary.cols-2 && tt_rect.y + tt_rect.height < Out_binary.rows-2)
						{
							m_max_object_value = tt_rect.width*tt_rect.height;
							m_max_object_num = k;
						}
					}
					if (m_max_object_num == -1)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No head object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
						return true;
					}
					BOLT_Param[Cam_num].Object_1st_idx = m_max_object_num;
					t_Side_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_max_object_num]) );
					// 머리를 못 찾을 경우 Error 처리함.
					if(m_max_object_num == -1)
					{
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No Object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);

						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,100,0),2);
						msg.Format("Bolt body must be in orange box!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,Dst_Img[Cam_num].rows-20), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 2, 8);
						return true;
					}

					if (m_max_object_num >= 0)
					{
						BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
						int left = t_Side_rect.x;
						int top = t_Side_rect.y;
						int width = t_Side_rect.width;
						int height = t_Side_rect.height;

						int x = left + width / 2;
						int y = top + height / 2;

						if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
						{
							x = left;y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
						{
							x = left + width; y = top + height / 2;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
						{
							x = left; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
						{
							x = left; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
						{
							x = left + width; y = top;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
						{
							x = left + width; y = top + height;
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::TOP_CENTER)
						{
							x = left + width/2; y = top;
						}

						BOLT_Param[Cam_num].Object_Postion = Point(x, y);//기준점 등록

						//BOLT_Param[Cam_num].Object_Postion = Point(t_Side_rect.x,t_Side_rect.y);
						if (m_Text_View[Cam_num]) // ROI 영역 표시
						{
							rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,100,0),1);
							vector<vector<Point> > total_contours(1);
							for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
							{
								total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
							}
							if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

								tt_rect = boundingRect( Mat(total_contours[0]) );
								Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
								t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
							}
						}
					}

					if (Result_Debugging)
					{
						msg.Format("Save\\Debugging\\CAM%d_Base_Find_Obj_Gray_Img.bmp",Cam_num);
						imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
						msg.Format("Save\\Debugging\\CAM%d_Base_Find_Obj_Thres_Img.bmp",Cam_num);
						imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
					}
				}
			}
		}
	}
	return true;
}

float CImPro_Library::Cal_Angle(Mat& Out_binary, int Cam_num)
{
	vector<vector<Point>> contours;	// 검사대상 Contour 정보
	vector<Vec4i> hierarchy;			// 검사대상 Hierarchy 정보

	findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
	if (contours.size() != 0)
	{
		Rect boundRect;
		int t_max = 0; int t_max_idx = -1;
		for( int k = 0; k < contours.size(); k++ )
		{  
			boundRect = boundingRect( Mat(contours[k]) );
			if (t_max <= boundRect.height*boundRect.width
				&& boundRect.x > 1 && boundRect.x + boundRect.width < Out_binary.cols-2)
			{
				t_max = boundRect.height*boundRect.width;
				t_max_idx = k;
			}
		}
		if (t_max_idx > -1)
		{
			Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
			Scalar color(255);
			drawContours( Out_binary, contours, t_max_idx, color, CV_FILLED, 8, hierarchy );
		}
		vector<Point2f> vec_center;
		double t_center = 0;double t_cnt = 0;
		for (int j=0;j < Out_binary.rows;j++)
		{
			t_center = 0;t_cnt = 0;
			for (int i = 0; i < Out_binary.cols;i++)
			{
				if (Out_binary.at<uchar>(j,i) > 0)
				{
					t_center += (double)i;
					t_cnt++;
				}
			}
			if (t_cnt > 0)
			{
				vec_center.push_back(Point2f(t_center/t_cnt,(double)j));
				if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
				{
					Rect t_Sub_ROI;
					t_Sub_ROI.x = 0;t_Sub_ROI.y = BOLT_Param[Cam_num].nAngleHeightTop[0];t_Sub_ROI.width = Out_binary.cols;t_Sub_ROI.height = BOLT_Param[Cam_num].nAngleHeightHeight[0];

					circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[0])(t_Sub_ROI),vec_center[vec_center.size()-1],1,CV_RGB(255,255,0),1);
					//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
					//rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[0],CV_RGB(0,0,255),1);
				}
			}
		}

		Vec4f lines;
		vector<Point2f> vec_2center; // 추가 2017.04.13
		if (vec_center.size() > 6)
		{
			float t_x0 = (vec_center[0].x + vec_center[1].x+vec_center[2].x)/3.0f;// 추가 2017.04.13
			float t_y0 = (vec_center[0].y + vec_center[1].y+vec_center[2].y)/3.0f;// 추가 2017.04.13
			float t_x1 = (vec_center[vec_center.size()-1].x + vec_center[vec_center.size()-2].x+vec_center[vec_center.size()-3].x)/3.0f;// 추가 2017.04.13
			float t_y1 = (vec_center[vec_center.size()-1].y + vec_center[vec_center.size()-2].y+vec_center[vec_center.size()-3].y)/3.0f;// 추가 2017.04.13
			return f_angle360(Point2f(t_x0,t_y0), Point2f(t_x1,t_y1));
		}
	}
	return 180;
}

void CImPro_Library::Rotation_Calibration(int Cam_num)
{
	int s=0;
	CString msg;

	
	if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::NONE)
	{
		return;
	}

	bool t_check_roi = true;
	if (BOLT_Param[Cam_num].nRect[s].x < 0)
	{
		t_check_roi = false;
	}
	if (BOLT_Param[Cam_num].nRect[s].y < 0)
	{
		t_check_roi = false;
	}
	if (BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width >= Gray_Img[Cam_num].cols)
	{
		t_check_roi = false;
	}
	if (BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height >= Gray_Img[Cam_num].rows)
	{
		t_check_roi = false;
	}

	if (!t_check_roi)
	{
		return;
	}

	if (!Gray_Img[Cam_num].empty())
	{

		// ROI 이미지 Crop
		Mat Out_binary;
		Mat Out_binary_Tmp;
		Mat CP_Gray_Img;

		if (BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width >= Gray_Img[Cam_num].cols)
		{
			BOLT_Param[Cam_num].nRect[s].width = Gray_Img[Cam_num].cols - BOLT_Param[Cam_num].nRect[s].x - 1;
		}
		if (BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height >= Gray_Img[Cam_num].rows)
		{
			BOLT_Param[Cam_num].nRect[s].height = Gray_Img[Cam_num].rows - BOLT_Param[Cam_num].nRect[s].y - 1;
		}
		CP_Gray_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();

		// 임계화
		// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
		if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
		{
			threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
		{
			threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
		{
			inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
		{
			Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
			inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
			subtract(White_Out_binary, Out_binary, Out_binary);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
		{
			threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
		{
			threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
		{
			blur(CP_Gray_Img, Out_binary, Size(3,3));
			// Run the edge detector on grayscale
			Canny(Out_binary, Out_binary, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::FIRSTROI) // ROI#01 결과 사용
		{
			threshold(CP_Gray_Img,Out_binary,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::DIFFAVG) // 평균기준 차이
		{
			Scalar tempVal = mean( CP_Gray_Img );
			float myMAtMean = tempVal.val[0];
			Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
			inRange(CP_Gray_Img, Scalar(myMAtMean - BOLT_Param[Cam_num].nThres_V1[s]),Scalar(myMAtMean + BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
			subtract(White_Out_binary, Out_binary, Out_binary);
			if (BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_SIZE_TB && BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_COUNT_TB)
			{
				if (m_Text_View[Cam_num] && !ROI_Mode)
				{
					msg.Format("Avg = %1.3f", myMAtMean);
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
				}
				if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
				{
					msg.Format("Avg = %1.3f", myMAtMean);
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
				}
			}
		}

		vector<vector<Point>> contours;	// 검사대상 Contour 정보
		vector<Vec4i> hierarchy;			// 검사대상 Hierarchy 정보
		findContours(Out_binary, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
		Rect boundRect;
		int m_max_object_num = -1;int m_max_object_value = 0;
		if (contours.size() > 0)
		{
			for (int k = 0; k < contours.size(); k++)
			{
				boundRect = boundingRect(Mat(contours[k]));
				if (m_max_object_value <= boundRect.width*boundRect.height)
				{
					m_max_object_value = boundRect.width*boundRect.height;
					m_max_object_num = k;
				}
			}
		}
		if (m_max_object_num == -1)
		{
			return;
		}
		Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
		drawContours( Out_binary, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy );
		boundRect = boundingRect( Mat(contours[m_max_object_num]) );
		//imwrite("00.bmp",Out_binary);
		vector<Point2f> vec_Point;
		if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::LEFT)
		{
			double t_cnt = 0;
			for (int j=boundRect.y+boundRect.height/3;j < boundRect.y+2*boundRect.height/3;j++)
			{
				for (int i = 0; i < boundRect.x+boundRect.width;i++)
				{
					if (Out_binary.at<uchar>(j,i) > 0)
					{
						vec_Point.push_back(Point2f((double)i,(double)j));
						break;
					}
				}
			}

			Vec4f lines;
			vector<Point2f> vec_2center;
			if (vec_Point.size() > 6)
			{
				fitLine(vec_Point,lines, CV_DIST_L2,0,0.001,0.001);
				//float x0= lines[2]; // 선에 놓은 한 점
				//float y0= lines[3];
				//float x1= x0-200*lines[0]; // 200 길이를 갖는 벡터 추가
				//float y1= y0-200*lines[1]; // 단위 벡터 사용
				//float t_angle = atan((y1-y0)/(x1-x0));

				float t_angle = (180.0/CV_PI)*atan(lines[1]/lines[0])+90.0;
				//msg.Format("%1.3f, %1.3f",(180.0/CV_PI)*atan(lines[1]/lines[0]), t_angle);
				//AfxMessageBox(msg);

				//imwrite("01.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
				if (t_angle < 45 && t_angle > -45)
				{
					J_Rotate_PRE(Gray_Img[Cam_num], t_angle,Gray_Img[Cam_num],1);
				}
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
		else if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::RIGHT)
		{
			double t_cnt = 0;
			for (int j=boundRect.y+boundRect.height/3;j < boundRect.y+2*boundRect.height/3;j++)
			{
				for (int i = Out_binary.cols-1; i >= 0;i--)
				{
					if (Out_binary.at<uchar>(j,i) > 0)
					{
						vec_Point.push_back(Point2f((double)i,(double)j));
						break;
					}
				}
			}

			Vec4f lines;
			vector<Point2f> vec_2center;
			if (vec_Point.size() > 6)
			{
				fitLine(vec_Point,lines, CV_DIST_L2,0,0.001,0.001);
				//float x0= lines[2]; // 선에 놓은 한 점
				//float y0= lines[3];
				//float x1= x0-200*lines[0]; // 200 길이를 갖는 벡터 추가
				//float y1= y0-200*lines[1]; // 단위 벡터 사용
				//float t_angle = atan((y1-y0)/(x1-x0));

				float t_angle = (180.0/CV_PI)*atan(lines[1]/lines[0])-90.0;
				//imwrite("01.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));

				J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Gray_Img[Cam_num],1);
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
		else if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::TOP)
		{
			double t_cnt = 0;
			for (int i=boundRect.x;i < boundRect.x+boundRect.width;i++)
			{
				for (int j=boundRect.y;j < boundRect.y+boundRect.height;j++)
				{
					if (Out_binary.at<uchar>(j,i) > 0)
					{
						vec_Point.push_back(Point2f((double)i,(double)j));
						break;
					}
				}
			}

			Vec4f lines;
			vector<Point2f> vec_2center;
			if (vec_Point.size() > 6)
			{
				fitLine(vec_Point,lines, CV_DIST_L2,0,0.001,0.001);
				//float x0= lines[2]; // 선에 놓은 한 점
				//float y0= lines[3];
				//float x1= x0-200*lines[0]; // 200 길이를 갖는 벡터 추가
				//float y1= y0-200*lines[1]; // 단위 벡터 사용
				//float t_angle = atan((y1-y0)/(x1-x0));

				float t_angle = -(180.0/CV_PI)*atan(lines[1]/lines[0]);
				//msg.Format("%1.3f",t_angle);
				//AfxMessageBox(msg);
				//imwrite("01.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));

				J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Gray_Img[Cam_num],1);
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
		else if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::BOTTOM)
		{
			double t_cnt = 0;
			for (int i=boundRect.x+boundRect.width-1;i >= boundRect.x;i--)
			{
				for (int j=boundRect.y;j < boundRect.y+boundRect.height;j++)
				{
					if (Out_binary.at<uchar>(j,i) > 0)
					{
						vec_Point.push_back(Point2f((double)i,(double)j));
						break;
					}
				}
			}

			Vec4f lines;
			vector<Point2f> vec_2center;
			if (vec_Point.size() > 6)
			{
				fitLine(vec_Point,lines, CV_DIST_L2,0,0.001,0.001);
				//float x0= lines[2]; // 선에 놓은 한 점
				//float y0= lines[3];
				//float x1= x0-200*lines[0]; // 200 길이를 갖는 벡터 추가
				//float y1= y0-200*lines[1]; // 단위 벡터 사용
				//float t_angle = atan((y1-y0)/(x1-x0));

				float t_angle = -(180.0/CV_PI)*atan(lines[1]/lines[0]);
				//imwrite("01.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));

				J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Gray_Img[Cam_num],1);
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
		else if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::LRCENTER)
		{
			double t_v = 0;double t_cnt = 0;
			for (int j=boundRect.y;j < boundRect.y+boundRect.height;j++)
			{
				t_cnt = 0;t_v=0;
				for (int i = boundRect.x; i < boundRect.x+boundRect.width;i++)
				{
					if (Out_binary.at<uchar>(j,i) > 0)
					{
						t_v+=(double)i;
						t_cnt++;
					}
				}
				if (t_cnt > 0)
				{
					t_v /= t_cnt;
					vec_Point.push_back(Point2f(t_v,(double)j));
				}
			}

			Vec4f lines;
			vector<Point2f> vec_2center;
			if (vec_Point.size() > 6)
			{
				fitLine(vec_Point,lines, CV_DIST_L2,0,0.001,0.001);
				//float x0= lines[2]; // 선에 놓은 한 점
				//float y0= lines[3];
				//float x1= x0-200*lines[0]; // 200 길이를 갖는 벡터 추가
				//float y1= y0-200*lines[1]; // 단위 벡터 사용
				//float t_angle = atan((y1-y0)/(x1-x0));

				float t_angle = (180.0/CV_PI)*atan(lines[1]/lines[0])+90.0;
				//msg.Format("%1.3f, %1.3f",(180.0/CV_PI)*atan(lines[1]/lines[0]), t_angle);
				//AfxMessageBox(msg);

				//imwrite("01.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
				if (t_angle < 45 && t_angle > -45)
				{
					J_Rotate_PRE(Gray_Img[Cam_num], t_angle,Gray_Img[Cam_num],1);
				}
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
		else if (BOLT_Param[Cam_num].nROI0_Rotation_Method[s] == ROTATION_METHOD::TBCENTER)
		{
			double t_v = 0;double t_cnt = 0;
			for (int i=boundRect.x;i < boundRect.x+boundRect.width;i++)
			{
				t_cnt = 0;t_v=0;
				for (int j=boundRect.y;j < boundRect.y+boundRect.height;j++)
				{
					if (Out_binary.at<uchar>(j,i) > 0)
					{
						t_v+=(double)j;
						t_cnt++;
					}
				}
				if (t_cnt > 0)
				{
					t_v /= t_cnt;
					vec_Point.push_back(Point2f((double)i,t_v));
				}
			}

			Vec4f lines;
			vector<Point2f> vec_2center;
			if (vec_Point.size() > 6)
			{
				fitLine(vec_Point,lines, CV_DIST_L2,0,0.001,0.001);
				//float x0= lines[2]; // 선에 놓은 한 점
				//float y0= lines[3];
				//float x1= x0-200*lines[0]; // 200 길이를 갖는 벡터 추가
				//float y1= y0-200*lines[1]; // 단위 벡터 사용
				//float t_angle = atan((y1-y0)/(x1-x0));

				float t_angle = -(180.0/CV_PI)*atan(lines[1]/lines[0]);
				//msg.Format("%1.3f",t_angle);
				//AfxMessageBox(msg);
				//imwrite("01.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));

				J_Rotate_PRE(Gray_Img[Cam_num],t_angle,Gray_Img[Cam_num],1);
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
	}
}

double CImPro_Library::round( double value, int pos )
{
	double temp;
	temp = value * pow( (double)10, (double)pos );  // 원하는 소수점 자리수만큼 10의 누승을 함
	temp = floor( temp + 0.5 );          // 0.5를 더한후 버림하면 반올림이 됨
	temp *= pow( (double)10, (double)-pos );           // 다시 원래 소수점 자리수로
	return temp;
}  

void CImPro_Library::RUN_Algorithm_SUGIYAMA()
{
	int nDebug = false;
	int Cam_num = 0;int s = 0;
	Mat Original_Threshold_Img, CP_Gray_Img;
	Mat Out_Threshold_Img;
	Mat Inner_Threshold_Img;
	double Out_Circle_Info[5];			// 해드 외경 Circle 정보
	double Inner_Circle_Info[5];			// 해드 외경 Circle 정보
	vector<vector<Point>> contours;
	vector<Vec4i> hierarchy;
	CString strTemp,msg;
	vector<double> vec_Result_Value[SGYM_INSPECTION_ITEM_END];

	if (!Gray_Img[Cam_num].empty())
	{
		if(nDebug) imwrite("D:\\Log\\Algorithm\\Gray_Img[Cam_num].bmp",Gray_Img[Cam_num]);

		Rect tROI = BOLT_Param[Cam_num].nRect[s];
		if (tROI.x < 0)
		{
			tROI.x = 0;
		}
		if (tROI.y < 0)
		{
			tROI.y = 0;
		}
		if (tROI.x + tROI.width >= Gray_Img[Cam_num].cols)
		{
			tROI.width = Gray_Img[Cam_num].cols - tROI.x -2;
		}
		if (tROI.y + tROI.height >= Gray_Img[Cam_num].rows)
		{
			tROI.height = Gray_Img[Cam_num].rows - tROI.y -2;
		}
		tROI.x = 0;
		tROI.y = 0;
		tROI.width = Gray_Img[Cam_num].cols;
		tROI.height = Gray_Img[Cam_num].rows;

		CP_Gray_Img = Gray_Img[Cam_num](tROI).clone();
		//if (m_Text_View[Cam_num] && !ROI_Mode && BOLT_Param[Cam_num].nCamPosition == 0)
		//{
		//	rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
		//	msg.Format("Obj. ROI");
		//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,255,50), 1, 8);
		//}
		//if (ROI_Mode && ROI_CAM_Num == Cam_num && BOLT_Param[Cam_num].nCamPosition == 0)
		//{
		//	rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
		//	msg.Format("Obj. ROI");
		//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,255,50), 1, 8);
		//}

		if (Result_Debugging)
		{
			msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Gray_Img.bmp",Cam_num,s);
			imwrite(msg.GetBuffer(),CP_Gray_Img);		
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		// [끝] ROI 설정 및 이미지 자르기
		///////////////////////////////////////////////////////////////////////////////////////////////////////

		///////////////////////////////////////////////////////////////////////////////////////////////////////
		// [시작] 임계화 ALG
		// Threshold 0:이하, 1:이상, 2:사이, 3:Less than Auto, 4:More than Auto, 5:Edge, 6:ROI#01 결과 사용
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		if (BOLT_Param[Cam_num].nMethod_Direc[s] == 0 || BOLT_Param[Cam_num].nMethod_Direc[s] == 1)
		{
			medianBlur(CP_Gray_Img,CP_Gray_Img,3);
		}
		if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
		{
			threshold(CP_Gray_Img,Original_Threshold_Img,BOLT_Param[Cam_num].nThres_V1[s],255,CV_THRESH_BINARY_INV);
		} 
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
		{
			threshold(CP_Gray_Img,Original_Threshold_Img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY);
		} 
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
		{
			inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Original_Threshold_Img);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEENOUT) // V1이하V2이상
		{
			Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
			inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Original_Threshold_Img);
			subtract(White_Out_binary, Original_Threshold_Img, Original_Threshold_Img);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // Less than Auto
		{
			threshold(CP_Gray_Img,Original_Threshold_Img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // More than Auto
		{
			threshold(CP_Gray_Img,Original_Threshold_Img,BOLT_Param[Cam_num].nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::EDGE) // Edge검출
		{
			blur(CP_Gray_Img, Original_Threshold_Img, Size(3,3));
			// Run the edge detector on grayscale
			Canny(Original_Threshold_Img, Original_Threshold_Img, BOLT_Param[Cam_num].nThres_V1[s], BOLT_Param[Cam_num].nThres_V2[s], 3);
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::FIRSTROI) // ROI#01 결과 사용
		{
			if (s > 0)
			{
				Original_Threshold_Img = BOLT_Param[Cam_num].Thres_Obj_Img(tROI).clone();
			} 
			else
			{
				inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Original_Threshold_Img);
			}
		}
		else if (BOLT_Param[Cam_num].nMethod_Thres[s] == THRES_METHOD::DIFFAVG) // 평균기준 차이
		{
			Scalar tempVal = mean( CP_Gray_Img );
			float myMAtMean = tempVal.val[0];
			Mat White_Out_binary = Mat::ones(CP_Gray_Img.rows, CP_Gray_Img.cols, CV_8U)*255;
			inRange(CP_Gray_Img, Scalar(myMAtMean - BOLT_Param[Cam_num].nThres_V1[s]),Scalar(myMAtMean + BOLT_Param[Cam_num].nThres_V2[s]),Original_Threshold_Img);
			subtract(White_Out_binary, Original_Threshold_Img, Original_Threshold_Img);
			if (BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_SIZE_TB && BOLT_Param[Cam_num].nMethod_Direc[s] != ALGORITHM_TB::CIRCLE_BLOB_COUNT_TB)
			{
				if (m_Text_View[Cam_num] && !ROI_Mode)
				{
					msg.Format("Avg = %1.3f", myMAtMean);
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
				}
				if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
				{
					msg.Format("Avg = %1.3f", myMAtMean);
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height + 10), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1+ (double)Gray_Img[Cam_num].rows/960, 8);
				}
			}
		}

		Rect rect_original = Rect(10,10,Original_Threshold_Img.cols-20,Original_Threshold_Img.rows-20);
		int t_size = countNonZero(Original_Threshold_Img);
		if (t_size <= Original_Threshold_Img.rows * Original_Threshold_Img.cols / 16)
		{
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num],rect_original,Scalar(0,0,255),2);
			return;
		}

		if(nDebug) imwrite("D:\\Log\\Algorithm\\Original_Threshold_Img.bmp",Original_Threshold_Img);

		// 칸투어 이용하여 가장큰 RECT 찾아서 이미지 잘라옴.
		// 필요 없는 contour들은 0으로 채워 없앰.
		findContours( Original_Threshold_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

		if (contours.size() < 1)
		{
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),rect_original,Scalar(0,0,255),2);
			return;
		}

		int m_max_cnt = -1;
		double m_max = -1;
		double m_area = 0;
		for( int k = 0; k < contours.size(); k++ )
		{  
			m_area = contourArea(contours[k],false);

			if (m_area >= m_max)
			{
				m_max = m_area;
				m_max_cnt = k;
			}
		}
		Mat Original_Threshold_Img_Temp = Original_Threshold_Img.clone();
		if (m_max_cnt == -1)
		{
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),rect_original,Scalar(0,0,255),2);
			return;
		} else
		{
			Original_Threshold_Img = Mat::zeros(Original_Threshold_Img_Temp.size(), CV_8UC1);
			drawContours( Original_Threshold_Img, contours, m_max_cnt, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
			bitwise_and(Original_Threshold_Img_Temp,Original_Threshold_Img,Original_Threshold_Img);
		}

		//Original_Threshold_Img.copyTo(Original_Threshold_Img_Temp);

		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1));

		// 처리 시간 줄이기 위해서 ROI 크기만큼만 자른 이미지를 만든다.
		// ROI 이미지 만들고 좌우상하에 border를 만들어 준다.
		Rect m_boundRect = boundingRect(Mat(contours[m_max_cnt]));

		//CString strTemp;
		//strTemp.Format("x : %d, y : %d, width : %d, height : %d",m_boundRect.x,m_boundRect.y,m_boundRect.width,m_boundRect.height);
		//OutputDebugString(strTemp);

		int boundary_margin = 2;
		m_boundRect.x -= boundary_margin;
		if (m_boundRect.x < 0)
		{
			m_boundRect.x = 0;
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),Rect(100,100,Original_Threshold_Img.cols-200,Original_Threshold_Img.rows-200),Scalar(0,0,255),2);
			return;
		}
		m_boundRect.y -= boundary_margin;

		if (m_boundRect.y < 0)
		{
			m_boundRect.y = 0;
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),Rect(100,100,Original_Threshold_Img.cols-200,Original_Threshold_Img.rows-200),Scalar(0,0,255),2);
			return;
		}
		m_boundRect.width += 2*boundary_margin;
		if (m_boundRect.x + m_boundRect.width >= Original_Threshold_Img_Temp.cols)
		{
			m_boundRect.width = Original_Threshold_Img_Temp.cols-m_boundRect.x -1;
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),Rect(100,100,Original_Threshold_Img.cols-200,Original_Threshold_Img.rows-200),Scalar(0,0,255),2);
			return;
		}
		m_boundRect.height += 2*boundary_margin;
		if (m_boundRect.y + m_boundRect.height >= Original_Threshold_Img_Temp.rows)
		{
			m_boundRect.height = Original_Threshold_Img_Temp.rows-m_boundRect.y -1;
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),Rect(100,100,Original_Threshold_Img.cols-200,Original_Threshold_Img.rows-200),Scalar(0,0,255),2);
			return;
		}



		Mat ROI_Original_Threshold_Img = Original_Threshold_Img(m_boundRect);
		int ROI_Margin = 40;
		copyMakeBorder( ROI_Original_Threshold_Img, ROI_Original_Threshold_Img, ROI_Margin, ROI_Margin, ROI_Margin, ROI_Margin, BORDER_CONSTANT, Scalar(0));
		Rect CP_m_boundRect = m_boundRect;
		CP_m_boundRect.x -= ROI_Margin;
		CP_m_boundRect.y -= ROI_Margin;
		CP_m_boundRect.width += 2*ROI_Margin;
		CP_m_boundRect.height += 2*ROI_Margin;
		//imwrite("01.bmp",ROI_Original_Threshold_Img);
		// 외경계산
		dilate(ROI_Original_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),20); // 슬리트 채움
		erode(Out_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),20);
		Inner_Threshold_Img = Out_Threshold_Img.clone();
		Mat Slit_Img = Out_Threshold_Img.clone();
		J_Fill_Hole(Out_Threshold_Img);
		erode(Out_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),50); // fitting때 단자 간섭 제거위함.
		dilate(Out_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),50);
		J_Fitting_Ellipse(Out_Threshold_Img, Out_Circle_Info, true);

		// 내경계산
		subtract(Out_Threshold_Img,Inner_Threshold_Img,Inner_Threshold_Img);
		dilate(Inner_Threshold_Img,Inner_Threshold_Img,element,Point(-1,-1),5);
		erode(Inner_Threshold_Img,Inner_Threshold_Img,element,Point(-1,-1),5);
		J_Fitting_Ellipse(Inner_Threshold_Img, Inner_Circle_Info, true);
		vec_Result_Value[INNER_RADIUS].push_back(BOLT_Param[0].nResolution[0]*(Inner_Circle_Info[2]+Inner_Circle_Info[3])/2);
		circle(Dst_Img[0](tROI),Point(Inner_Circle_Info[0]+m_boundRect.x - ROI_Margin,Inner_Circle_Info[1]+m_boundRect.y - ROI_Margin),(Inner_Circle_Info[2]+Inner_Circle_Info[3])/4,Scalar(0,255,0),1,CV_AA);

		Out_Circle_Info[0] = round(Inner_Circle_Info[0],0);
		Out_Circle_Info[1] = round(Inner_Circle_Info[1],0);
		Out_Circle_Info[2] = round((Out_Circle_Info[2] + Out_Circle_Info[3])/2,0);
		Out_Circle_Info[3] = Out_Circle_Info[2];
		/*CString strTemp;
		strTemp.Format("Point X: %.3lf, Y: %.3lf, Radius : %.3lf ",Out_Circle_Info[0],Out_Circle_Info[1],(Out_Circle_Info[2] + Out_Circle_Info[3])/4);
		OutputDebugString(strTemp);

		Out_Circle_Info[0] = 422.384;
		Out_Circle_Info[1] = 424.895;
		Out_Circle_Info[2] = 308.692*2;
		Out_Circle_Info[3] = 308.692*2;*/

		circle(Dst_Img[0](tROI),Point(Out_Circle_Info[0]+m_boundRect.x - ROI_Margin,Out_Circle_Info[1]+m_boundRect.y - ROI_Margin),(Out_Circle_Info[2]+Out_Circle_Info[3])/4,Scalar(0,255,0),2,CV_AA);
		if(nDebug) imwrite("D:\\Log\\Algorithm\\Out_Threshold_Img.bmp",Out_Threshold_Img);

		// 단자경 계산
		Mat CP_Original_Threshold_Img = Original_Threshold_Img.clone();
		vector<vector<Point>> hull(1);
		convexHull( Mat(contours[m_max_cnt]), hull[0], false ); 
		Point2f R_Center(Out_Circle_Info[0]+m_boundRect.x - ROI_Margin,Out_Circle_Info[1]+m_boundRect.y - ROI_Margin);
		double t_max = 0;double t_min = 9999999;
		vector<double> out_diameter;
		for (int i = 0; i <hull[0].size(); i++)
		{
			out_diameter.push_back(sqrt((hull[0][i].x - R_Center.x)*(hull[0][i].x - R_Center.x) + (hull[0][i].y - R_Center.y)*(hull[0][i].y - R_Center.y)));
			if (t_min >= out_diameter[i])
			{
				t_min = out_diameter[i];
			}
			if (t_max <= out_diameter[i])
			{
				t_max = out_diameter[i];
			}
		}
		double tt_sum = 0;double tt_sum_cnt = 0;
		for (int i = 0; i <hull[0].size(); i++)
		{
			if (out_diameter[i] > 0.8*t_max && out_diameter[i] <= t_max)
			{
				vec_Result_Value[OUTER_RADIUS].push_back(BOLT_Param[0].nResolution[0]*out_diameter[i]*2);
				tt_sum+=out_diameter[i];
				tt_sum_cnt++;
			}
		}
		circle(Dst_Img[0](tROI),R_Center,tt_sum/tt_sum_cnt,Scalar(0,255,0),2,CV_AA);



		// 슬리트 찾기
		Mat Out_Circle_Img = Mat::zeros(ROI_Original_Threshold_Img.size(), CV_8UC1);
		ellipse(Out_Circle_Img,Point(Out_Circle_Info[0],Out_Circle_Info[1]),Size(Out_Circle_Info[2]/2,Out_Circle_Info[3]/2),Out_Circle_Info[4],0,360,Scalar(255,255,255),2);
		J_Fill_Hole(Out_Circle_Img);

		//imwrite("00.bmp",Slit_Img);
		subtract(Slit_Img,ROI_Original_Threshold_Img,Slit_Img);
		//imwrite("01.bmp",Slit_Img);
		dilate(Inner_Threshold_Img,Inner_Threshold_Img,element,Point(-1,-1),30);
		//imwrite("02.bmp",Inner_Threshold_Img);
		subtract(Slit_Img,Inner_Threshold_Img,Slit_Img);
		//imwrite("03.bmp",Slit_Img);
		subtract(Slit_Img,255-Out_Circle_Img,Slit_Img);
		//imwrite("04.bmp",Out_Circle_Img);
		//imwrite("05.bmp",Slit_Img);
		erode(Slit_Img,Slit_Img,element,Point(-1,-1),1);
		dilate(Slit_Img,Slit_Img,element,Point(-1,-1),10);
		erode(Slit_Img,Slit_Img,element,Point(-1,-1),9);
		if(nDebug) imwrite("D:\\Log\\Algorithm\\Slit_Img.bmp",Slit_Img);
		if(nDebug) imwrite("D:\\Log\\Algorithm\\Original_Threshold_Img_Temp.bmp",ROI_Original_Threshold_Img);
		if(nDebug) imwrite("D:\\Log\\Algorithm\\Out_Circle_Img.bmp",Out_Circle_Img);
		contours.clear();
		vector<vector<Point>> contours_temp;
		findContours( Slit_Img.clone(), contours_temp, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		if (contours_temp.size() > 0)
		{
			for(int i=0; i<contours_temp.size(); i++)
			{	
				if(contourArea(contours_temp[i]) > ALG_SGYM_Param.dSlitMinArea)
				{
					contours.push_back(contours_temp[i]);
				}
			}
		}

		if (contours.size() <= 0)
		{
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),Rect(100,100,Original_Threshold_Img.cols-200,Original_Threshold_Img.rows-200),Scalar(0,0,255),2);
			return;
		}

		vector<RotatedRect> minSlitRect(contours.size());
		Mat Slit_Blob_Img = Mat::zeros(ROI_Original_Threshold_Img.size(), CV_8UC1);
		Point2f rect_points[4];
		for(int i=0; i<contours.size(); i++)
		{
			minSlitRect[i] = minAreaRect(contours[i]);
			float fangle;
			//CString strTemp;
			if(minSlitRect[i].size.width < minSlitRect[i].size.height){
				if(minSlitRect[i].center.y- Out_Circle_Info[1] < 0)
				{
					fangle = (minSlitRect[i].angle + 90) + 180;
					//strTemp.Format("Slit Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minSlitRect[i].center.x- Out_Circle_Info[0],minSlitRect[i].center.y- Out_Circle_Info[1],minSlitRect[i].size.width,minSlitRect[i].size.height,(minSlitRect[i].angle + 90) + 180);
				}
				else
				{
					fangle = (minSlitRect[i].angle + 90);
					//strTemp.Format("Slit Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minSlitRect[i].center.x- Out_Circle_Info[0],minSlitRect[i].center.y- Out_Circle_Info[1],minSlitRect[i].size.width,minSlitRect[i].size.height,(minSlitRect[i].angle + 90));
				}
			}
			else
			{
				if(minSlitRect[i].center.y- Out_Circle_Info[1] < 0)
				{
					fangle = (minSlitRect[i].angle + 180) + 180;
					//strTemp.Format("Slit Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minSlitRect[i].center.x- Out_Circle_Info[0],minSlitRect[i].center.y- Out_Circle_Info[1],minSlitRect[i].size.width,minSlitRect[i].size.height,(minSlitRect[i].angle + 180) + 180);
				}
				else
				{
					fangle = (minSlitRect[i].angle + 180);
					//strTemp.Format("Slit Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minSlitRect[i].center.x- Out_Circle_Info[0],minSlitRect[i].center.y- Out_Circle_Info[1],minSlitRect[i].size.width,minSlitRect[i].size.height,(minSlitRect[i].angle + 180));
				}
			}
			Slit_Info struct_Slit_Info;
			struct_Slit_Info.fAngle = round(fangle,2);
			struct_Slit_Info.centerPoint.x = round(minSlitRect[i].center.x,1);
			struct_Slit_Info.centerPoint.y = round(minSlitRect[i].center.y,1);
			struct_Slit_Info.fArea = round(contourArea(contours[i]),1);
			if(fangle == 90 || fangle == 270)
			{
				struct_Slit_Info.fWidth = round(boundingRect(contours[i]).width,1);
				struct_Slit_Info.fHeight = round(boundingRect(contours[i]).height,1);
			}
			else if(fangle == 180 || fangle == 360 || fangle == 0)
			{
				struct_Slit_Info.fWidth = round(boundingRect(contours[i]).height,1);
				struct_Slit_Info.fHeight = round(boundingRect(contours[i]).width,1);
			}
			else
			{
				struct_Slit_Info.fWidth = round((float)minSlitRect[i].size.width,1);
				struct_Slit_Info.fHeight = round((float)minSlitRect[i].size.height,1);
			}

			ALG_SGYM_Param.vec_Slit_Info.push_back(struct_Slit_Info);

			minSlitRect[i].points( rect_points );
			for( int j = 0; j < 4; j++ )
			{
				line( Dst_Img[Cam_num](tROI), Point2f(rect_points[j].x +m_boundRect.x - ROI_Margin,rect_points[j].y +m_boundRect.y - ROI_Margin) , 
					Point2f(rect_points[(j+1)%4].x +m_boundRect.x - ROI_Margin,rect_points[(j+1)%4].y +m_boundRect.y - ROI_Margin), CV_RGB(0,0,255), 1, 8 );
				line( Slit_Blob_Img, Point2f(rect_points[j].x ,rect_points[j].y) , 
					Point2f(rect_points[(j+1)%4].x,rect_points[(j+1)%4].y), CV_RGB(255,255,255), 0, 8 );
			}
		}
		//strTemp.Format("SLIT_COUNT : %.lf",(double)contours.size());
		//OutputDebugString(strTemp);
		if (contours.size() > 0)
		{
			vec_Result_Value[SLIT_COUNT].push_back(contours.size());
		}

		///단자 찾기 
		Mat Danja_Img = Mat::zeros(ROI_Original_Threshold_Img.size(), CV_8UC1);
		subtract(ROI_Original_Threshold_Img,Out_Circle_Img,Danja_Img);
		subtract(Danja_Img,Inner_Threshold_Img,Danja_Img);
		//imwrite("00_\Danja_Img.bmp",Danja_Img);

		//imwrite("02.bmp",Danja_Img);

		////// 들뜸 측정 추가
		////Mat Danja_DlDDM_Img = Danja_Img.clone(); // 들뜸 측정을 위한 이미지
		////Mat Danja_DlDDM_Convex_Img = Mat::zeros(Danja_DlDDM_Img.size(), CV_8UC1);
		////erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),2);
		////dilate(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),3);
		////erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),1);
		////vector<vector<Point>> contours1;
		////vector<Vec4i> hierarchy1;

		////findContours( Danja_DlDDM_Img.clone(), contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		//imwrite("000.bmp",Danja_DlDDM_Img);
		int DlDDM_CNT = 0;
		////if (contours1.size() > 0)
		////{
		////	vector<vector<Point>> hull1(contours1.size());
		////	for(size_t k=0; k<contours1.size(); k++)
		////	{	
		////		convexHull( Mat(contours1[k]), hull1[k], false ); 
		////		//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
		////		drawContours( Danja_DlDDM_Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
		////	}
		////	//imwrite("001.bmp",Danja_DlDDM_Convex_Img);
		////	////imwrite("Danja_DlDDM_Convex_Img.bmp",Danja_DlDDM_Convex_Img);
		////	////imwrite("01_Danja_DlDDM_Img.bmp",Danja_DlDDM_Img);
		////	subtract(Danja_DlDDM_Convex_Img,Danja_DlDDM_Img,Danja_DlDDM_Img);
		////	erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),1);
		////	dilate(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),3);
		////	erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),1);

		////	Rect m_boundRect1(m_boundRect.x - ROI_Margin,m_boundRect.y - ROI_Margin,m_boundRect.width + 2*ROI_Margin,m_boundRect.height + 2*ROI_Margin);
		////	for(size_t k=0; k<contours1.size(); k++)
		////	{	
		////		Rect boundRect1 = boundingRect( Mat(contours1[k]) );
		////		int t_area1 = countNonZero(Danja_DlDDM_Img(boundRect1));
		////		if (t_area1 <= ALG_SGYM_Param.DlDDM_Size_Threshold)
		////		{
		////			DlDDM_CNT++;
		////			//drawContours( Dst_Img[Cam_num](tROI)(m_boundRect1), contours1, (int)k, Scalar(255,0,255),CV_FILLED, 8, hierarchy1);
		////		}
		////	}
		////}
		//imwrite("02_Danja_DlDDM_Img.bmp",Danja_DlDDM_Img);

		// Find the convex hull object for each contour
		////		vector<vector<Point> >hull( contours1.size() );
		////		int m_max_object_num = -1;int m_max_object_value = 0;
		////		for( int k = 0; k < contours1.size(); k++ )
		////		{  
		////			convexHull( Mat(contours1[k]), hull[k], false ); 
		////			boundRect[k] = boundingRect( Mat(contours1[k]) );
		////			if (m_max_object_value <= boundRect[k].width && boundRect[k].width >= 50)
		////			{
		////				m_max_object_value = boundRect[k].width;
		////				m_max_object_num = k;
		////				drawContours( Binary_Img, hull, k, Scalar(255), 1, 8, vector<Vec4i>(), 0, Point() );
		////			}
		////		}




		erode(Danja_Img,Danja_Img,element,Point(-1,-1),4);
		dilate(Danja_Img,Danja_Img,element,Point(-1,-1),10);
		erode(Danja_Img,Danja_Img,element,Point(-1,-1),6);
		if(nDebug) imwrite("D:\\Log\\Algorithm\\Danja_Img.bmp",Danja_Img);

		contours.clear();
		contours_temp.clear();
		///단자 에서 area기준으로 1차 Filtering 을 한다.

		//imwrite("03.bmp",Danja_Img);

		findContours( Danja_Img, contours_temp, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		if (contours_temp.size() <= 0)
		{
			Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_");
			rectangle(Dst_Img[Cam_num](tROI),Rect(100,100,Original_Threshold_Img.cols-200,Original_Threshold_Img.rows-200),Scalar(0,0,255),2);
			return;
		}

		for(int i=0; i<contours_temp.size(); i++)
		{	
			if(contourArea(contours_temp[i]) > ALG_SGYM_Param.dDanjaMinArea)
			{
				contours.push_back(contours_temp[i]);
			}
		}
		vector<RotatedRect> minRect(contours.size());

		for(int i=0; i<contours.size(); i++)
		{
			minRect[i] = minAreaRect(contours[i]);
			minRect[i].points( rect_points );
			CString strTemp;
			float fAngle = minRect[i].angle;
			//if(minRect[i].size.width < minRect[i].size.height){
			//	if(minRect[i].center.y - Out_Circle_Info[1] < 0)
			//	{
			//		fAngle = (minRect[i].angle + 90) + 180;
			//		//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 90) + 180);
			//	}
			//	else
			//	{
			//		fAngle = (minRect[i].angle + 90);
			//		//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 90));
			//	}
			//}
			//else
			//{
			//	if(minRect[i].center.y- Out_Circle_Info[1] < 0)
			//	{
			//		fAngle = (minRect[i].angle + 180) + 180;
			//		//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 180) + 180);
			//	}
			//	else
			//	{
			//		fAngle = (minRect[i].angle + 180);
			//		//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 180));
			//	}
			//}

			Danja_Info struct_Danja_Info;
			struct_Danja_Info.fAngle = round(fAngle,2);
			struct_Danja_Info.centerPoint.x = round(minRect[i].center.x,1);
			struct_Danja_Info.centerPoint.y = round(minRect[i].center.y,1);
			struct_Danja_Info.fArea = round(contourArea(contours[i]),1);
			if(fAngle == 90 || fAngle == 270)
			{
				struct_Danja_Info.fWidth = round(boundingRect(contours[i]).width,1);
				struct_Danja_Info.fHeight = round(boundingRect(contours[i]).height,1);
			}
			else if(fAngle == 180 || fAngle == 360 || fAngle == 0)
			{
				struct_Danja_Info.fWidth = round(boundingRect(contours[i]).height,1);
				struct_Danja_Info.fHeight = round(boundingRect(contours[i]).width,1);
			}
			else
			{
				struct_Danja_Info.fWidth = round((float)minRect[i].size.width,1);
				struct_Danja_Info.fHeight = round((float)minRect[i].size.height,1);
			}

			ALG_SGYM_Param.vec_Danja_Info.push_back(struct_Danja_Info);

			for( int j = 0; j < 4; j++ )
			{
				line( Dst_Img[Cam_num](tROI), Point2f(rect_points[j].x +m_boundRect.x - ROI_Margin,rect_points[j].y +m_boundRect.y - ROI_Margin) , 
					Point2f(rect_points[(j+1)%4].x +m_boundRect.x - ROI_Margin,rect_points[(j+1)%4].y +m_boundRect.y - ROI_Margin),CV_RGB(0,0,255), 1, 8 );
			}
		}

		if (contours.size() > 0)
		{
			vec_Result_Value[DANJA_COUNT].push_back(contours.size());
		}

		// 각도 계산
		vector<double> vX;
		vector<double> vY;

		for(int i=0; i<minSlitRect.size(); i++)
		{
			circle(Dst_Img[0](tROI),Point(minSlitRect[i].center.x+m_boundRect.x - ROI_Margin,minSlitRect[i].center.y+m_boundRect.y - ROI_Margin),2,Scalar(0,255,0),1,CV_AA);
			vX.push_back(minSlitRect[i].center.x - Out_Circle_Info[0]);
			vY.push_back(minSlitRect[i].center.y - Out_Circle_Info[1]);
		}

		vector<double> mag;
		vector<double> Slitangle;

		if (vX.size() > 0)
		{
			cartToPolar(vX, vY, mag, Slitangle, true); // mag에는 vector의 크기, angle에는 0~360도의 값이 들어감.  
		}

		for(int i=0; i<minSlitRect.size(); i++)
		{
			ALG_SGYM_Param.vec_Slit_Info[i].fAngleFromCenter = round(Slitangle[i],2);
			msg.Format("%1.1f", ALG_SGYM_Param.vec_Slit_Info[i].fAngleFromCenter);
			putText(Dst_Img[Cam_num], msg.GetBuffer(0), Point(ALG_SGYM_Param.vec_Slit_Info[i].centerPoint.x+CP_m_boundRect.x + 5,ALG_SGYM_Param.vec_Slit_Info[i].centerPoint.y+CP_m_boundRect.y), fontFace, 0.5, CV_RGB(0, 80, 255), 1, 8);
			if((int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 90 || (int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 0 ||
				(int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 180 || (int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 270 || (int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 360 ) continue;
			if(((int)Slitangle[i] >= 90 && (int)Slitangle[i] < 180) ||
				((int)Slitangle[i] >= 270 && (int)Slitangle[i] < 360))
			{
				float fTemp;
				fTemp = ALG_SGYM_Param.vec_Slit_Info[i].fWidth;
				ALG_SGYM_Param.vec_Slit_Info[i].fWidth = ALG_SGYM_Param.vec_Slit_Info[i].fHeight;
				ALG_SGYM_Param.vec_Slit_Info[i].fHeight = fTemp;
			}
		}
		vX.clear();
		vY.clear();

		for(int i=0; i<minRect.size(); i++)
		{
			circle(Dst_Img[0](tROI),Point(minRect[i].center.x+m_boundRect.x - ROI_Margin,minRect[i].center.y+m_boundRect.y - ROI_Margin),2,Scalar(0,255,0),1,CV_AA);

			vX.push_back(minRect[i].center.x - Out_Circle_Info[0]);
			vY.push_back(minRect[i].center.y - Out_Circle_Info[1]);
		}

		mag.clear();
		//vector<double> mag;
		vector<double> angle;

		cartToPolar(vX, vY, mag, angle, true); // mag에는 vector의 크기, angle에는 0~360도의 값이 들어감.  

		for(int i=0; i<minRect.size(); i++)
		{
			ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter = round(angle[i],1);

			if((int)ALG_SGYM_Param.vec_Danja_Info[i].fAngle == 90 || (int)ALG_SGYM_Param.vec_Danja_Info[i].fAngle == 0 || (int)ALG_SGYM_Param.vec_Danja_Info[i].fAngle == 180 
				|| (int)ALG_SGYM_Param.vec_Danja_Info[i].fAngle == 270 || (int)ALG_SGYM_Param.vec_Danja_Info[i].fAngle == 360 ) continue;;

			if(	((int)angle[i] >= 90 && (int)angle[i] < 180) ||
				((int)angle[i] >= 270 && (int)angle[i] < 360))
			{
				float fTemp;
				fTemp = ALG_SGYM_Param.vec_Danja_Info[i].fWidth;
				ALG_SGYM_Param.vec_Danja_Info[i].fWidth = ALG_SGYM_Param.vec_Danja_Info[i].fHeight;
				ALG_SGYM_Param.vec_Danja_Info[i].fHeight = fTemp;
			}
		}

		////////////////////////////
		////////////////////////////
		///중간 Vector 출력
		/*for(int i=0; i<ALG_SGYM_Param.vec_Danja_Info.size(); i++)
		{
		CString strTemp;
		strTemp.Format("Danja Center X : %.3lf, Y : %.3lf,Angle : %.3lf, Center Angle : %.3lf, Area : %.3lf, Width : %.3lf, Height : %.3lf",ALG_SGYM_Param.vec_Danja_Info[i].centerPoint.x - Out_Circle_Info[0],ALG_SGYM_Param.vec_Danja_Info[i].centerPoint.y - Out_Circle_Info[1],ALG_SGYM_Param.vec_Danja_Info[i].fAngle
		,ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,ALG_SGYM_Param.vec_Danja_Info[i].fArea,ALG_SGYM_Param.vec_Danja_Info[i].fWidth,ALG_SGYM_Param.vec_Danja_Info[i].fHeight);
		OutputDebugString(strTemp);
		}

		for(int i=0; i<ALG_SGYM_Param.vec_Slit_Info.size(); i++)
		{
		CString strTemp;
		strTemp.Format("Slit Center X : %.3lf, Y : %.3lf,Angle : %.3lf, Center Angle : %.3lf, Area : %.3lf, Width : %.3lf, Height : %.3lf",ALG_SGYM_Param.vec_Slit_Info[i].centerPoint.x - Out_Circle_Info[0],ALG_SGYM_Param.vec_Slit_Info[i].centerPoint.y - Out_Circle_Info[1],ALG_SGYM_Param.vec_Slit_Info[i].fAngle
		,ALG_SGYM_Param.vec_Slit_Info[i].fAngleFromCenter,ALG_SGYM_Param.vec_Slit_Info[i].fArea,ALG_SGYM_Param.vec_Slit_Info[i].fWidth,ALG_SGYM_Param.vec_Slit_Info[i].fHeight);
		OutputDebugString(strTemp);
		}*/
		//////////////////////////////
		//////////////////////////////


		// 단자 휨 계산
		vector<double> danja_length;tt_sum=0;tt_sum_cnt=0;
		for(int i=0; i<ALG_SGYM_Param.vec_Danja_Info.size(); i++)
		{
			///요기 Rotation 값을 확인해보자..
			if(ALG_SGYM_Param.vec_Danja_Info[i].fHeight < ALG_SGYM_Param.dDanjaMinHeight || 
				ALG_SGYM_Param.vec_Danja_Info[i].fHeight > ALG_SGYM_Param.dDanjaMaxHeight ||
				(abs( fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,180) - fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngle,180)) > ALG_SGYM_Param.dDanjaRotationRage &&
				abs( fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,180) - fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngle,180) - 180) > ALG_SGYM_Param.dDanjaRotationRage))
			{
				tt_sum_cnt++;
			}

			if(ALG_SGYM_Param.vec_Danja_Info[i].fHeight < ALG_SGYM_Param.dDanjaMinHeight || ALG_SGYM_Param.vec_Danja_Info[i].fHeight > ALG_SGYM_Param.dDanjaMaxHeight)
			{
				minRect[i].points( rect_points );
				for( int j = 0; j < 4; j++ )
				{
					line( Dst_Img[Cam_num](tROI), Point2f(rect_points[j].x +m_boundRect.x - ROI_Margin,rect_points[j].y +m_boundRect.y - ROI_Margin) , 
						Point2f(rect_points[(j+1)%4].x +m_boundRect.x - ROI_Margin,rect_points[(j+1)%4].y +m_boundRect.y - ROI_Margin), CV_RGB(0,0,255), 2, 8 );
				}
			}

			if (abs( fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,180) - fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngle,180)) > ALG_SGYM_Param.dDanjaRotationRage &&
				abs( fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,180) - fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngle,180) - 180) > ALG_SGYM_Param.dDanjaRotationRage)
			{
				minRect[i].points( rect_points );
				for( int j = 0; j < 4; j++ )
				{
					line( Dst_Img[Cam_num](tROI), Point2f(rect_points[j].x +m_boundRect.x - ROI_Margin,rect_points[j].y +m_boundRect.y - ROI_Margin) , 
						Point2f(rect_points[(j+1)%4].x +m_boundRect.x - ROI_Margin,rect_points[(j+1)%4].y +m_boundRect.y - ROI_Margin), CV_RGB(255,0,0), 2, 8 );
				}
			}

			if((ALG_SGYM_Param.vec_Danja_Info[i].fHeight < ALG_SGYM_Param.dDanjaMinHeight || 
				ALG_SGYM_Param.vec_Danja_Info[i].fHeight > ALG_SGYM_Param.dDanjaMaxHeight) &&
				(abs( fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,180) - fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngle,180)) > ALG_SGYM_Param.dDanjaRotationRage &&
				abs( fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter,180) - fmod(ALG_SGYM_Param.vec_Danja_Info[i].fAngle,180) - 180) > ALG_SGYM_Param.dDanjaRotationRage))
			{
				minRect[i].points( rect_points );
				for( int j = 0; j < 4; j++ )
				{
					line( Dst_Img[Cam_num](tROI), Point2f(rect_points[j].x +m_boundRect.x - ROI_Margin,rect_points[j].y +m_boundRect.y - ROI_Margin) , 
						Point2f(rect_points[(j+1)%4].x +m_boundRect.x - ROI_Margin,rect_points[(j+1)%4].y +m_boundRect.y - ROI_Margin), CV_RGB(255,0,255), 2, 8 );
				}
			}

			float tv1 = abs(ALG_SGYM_Param.vec_Danja_Info[i].fAngle + 90.0f - ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter);
			float tv2 = abs(ALG_SGYM_Param.vec_Danja_Info[i].fAngle + 180.0f - ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter);
			float tv3 = abs(ALG_SGYM_Param.vec_Danja_Info[i].fAngle + 270.0f - ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter);
			float tv4 = abs(ALG_SGYM_Param.vec_Danja_Info[i].fAngle + 360.0f - ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter);
			float tvm1 = min(tv1,tv2);
			float tvm2 = min(tv3,tv4);
			float tvm = min(tvm1,tvm2);
			//ALG_SGYM_Param.vec_Danja_Info[i].fAngle = tvm;
			vec_Result_Value[DANJA_EACH_ANGLE].push_back((double)tvm);
			msg.Format("%1.1f/%1.1f", tvm,ALG_SGYM_Param.vec_Danja_Info[i].fAngleFromCenter);
			putText(Dst_Img[Cam_num], msg.GetBuffer(0), Point(ALG_SGYM_Param.vec_Danja_Info[i].centerPoint.x+CP_m_boundRect.x + 5,ALG_SGYM_Param.vec_Danja_Info[i].centerPoint.y+CP_m_boundRect.y), fontFace, 0.5, CV_RGB(255, 100, 0), 1, 8);
		}

		vec_Result_Value[DANJA_ROTATION_COUNT].push_back(tt_sum_cnt);

		if (angle.size() > 0)
		{
			sort(angle.begin(),angle.end(),sort_descending);
		}
		if (Slitangle.size() > 0)
		{
			sort(Slitangle.begin(),Slitangle.end(),sort_descending);
		}

		for(int i=1; i<angle.size(); i++)
		{
			vec_Result_Value[DANJA_TO_DANJA_ANGLE].push_back(abs(angle[i] - angle[i-1]));
		}
		int tt_num = min(angle.size(),Slitangle.size());

		double t_dv = 999; 
		int t_dv_idx = 0;
		for(int i=0; i<tt_num; i++)
		{
			if (angle[i] <= t_dv)
			{
				t_dv = angle[i];
				t_dv_idx = i;
			}
		}
		double t_sv = 999; 
		int t_sv_idx = 0;
		for(int i=0; i<tt_num; i++)
		{
			if (Slitangle[i] <= t_sv)
			{
				t_sv = Slitangle[i];
				t_sv_idx = i;
			}
		}

		int t_add_idx = -1;
		if (t_dv > t_sv)
		{
			t_add_idx = 1;
		}
		
		for(int i=0; i<tt_num; i++)
		{
			vec_Result_Value[DANJA_TO_SLIT_ANGLE].push_back(abs(angle[(i+t_dv_idx)%tt_num] - Slitangle[(i+t_sv_idx)%tt_num]));
			if (angle[(i+t_dv_idx)%tt_num] >= 0 && angle[(i+t_dv_idx)%tt_num] < 30 && Slitangle[(t_add_idx+i+t_sv_idx)%tt_num] > 330)
			{
				vec_Result_Value[DANJA_TO_SLIT_ANGLE].push_back(abs(angle[(i+t_dv_idx)%tt_num] + (360 - Slitangle[(t_add_idx+i+t_sv_idx)%tt_num])));
			}
			else if (angle[(i+t_dv_idx)%tt_num] > 330 && Slitangle[(t_add_idx+i+t_sv_idx)%tt_num] >= 0 && Slitangle[(t_add_idx+i+t_sv_idx)%tt_num] < 30)
			{
				vec_Result_Value[DANJA_TO_SLIT_ANGLE].push_back((360 - abs(angle[(i+t_dv_idx)%tt_num]) + Slitangle[(t_add_idx+i+t_sv_idx)%tt_num]));
			}
			else
			{
				vec_Result_Value[DANJA_TO_SLIT_ANGLE].push_back(abs(angle[(i+t_dv_idx)%tt_num] - Slitangle[(t_add_idx+i+t_sv_idx)%tt_num]));
			}
		}

//		for(int i=1; i<Slitangle.size(); i++)
//		{
//			vec_Result_Value[SLIT_TO_SLIT_ANGLE].push_back(abs(Slitangle[i] - Slitangle[i-1]));
//		}

		for(int i=0; i<ALG_SGYM_Param.vec_Slit_Info.size(); i++)
		{
			vec_Result_Value[SLIT_WIDTH_MIN].push_back(BOLT_Param[0].nResolution[0]*(double)ALG_SGYM_Param.vec_Slit_Info[i].fWidth);
			vec_Result_Value[SLIT_WIDTH_MAX].push_back(BOLT_Param[0].nResolution[0]*(double)ALG_SGYM_Param.vec_Slit_Info[i].fWidth);
			vec_Result_Value[SLIT_HEIGHT_MIN].push_back(BOLT_Param[0].nResolution[0]*(double)ALG_SGYM_Param.vec_Slit_Info[i].fHeight);
			vec_Result_Value[SLIT_HEIGHT_MAX].push_back(BOLT_Param[0].nResolution[0]*(double)ALG_SGYM_Param.vec_Slit_Info[i].fHeight);
		}

		///Slit 이물 검사 // CDJung 수정
		J_Fill_Hole(Slit_Blob_Img);
		//imwrite("00_Slit_Blob_Img.bmp",Slit_Blob_Img);
		//imwrite("00_Original_Threshold_Img_Temp2.bmp",ROI_Original_Threshold_Img);
		Mat Slit_Defect_Img = Mat::zeros(ROI_Original_Threshold_Img.size(), CV_8UC1);
		erode(Slit_Blob_Img,Slit_Blob_Img,element,Point(-1,-1),2);

		Mat thres_Slit_Defect_Img = Mat::zeros(m_boundRect.size(), CV_8UC1);
		threshold(Gray_Img[Cam_num](m_boundRect),thres_Slit_Defect_Img,ALG_SGYM_Param.Chip_Threshold,255,CV_THRESH_BINARY_INV);
		copyMakeBorder( thres_Slit_Defect_Img, thres_Slit_Defect_Img, ROI_Margin, ROI_Margin, ROI_Margin, ROI_Margin, BORDER_CONSTANT, Scalar(0));

		bitwise_and(Slit_Blob_Img,thres_Slit_Defect_Img,Slit_Defect_Img);
		//imwrite("02_Slit_Blob_Img.bmp",Slit_Defect_Img);
		vector<vector<Point>> contours_Slit_Defect;
		vector<Vec4i> hierarchy_Slit_Defect;
		Rect rect_temp;
		rect_temp.x = ROI_Margin;
		rect_temp.y = ROI_Margin;
		rect_temp.width =  Slit_Defect_Img.cols - 2*ROI_Margin;
		rect_temp.height =  Slit_Defect_Img.rows - 2*ROI_Margin;

		findContours( Slit_Defect_Img(rect_temp).clone(), contours_Slit_Defect, hierarchy_Slit_Defect, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

		//double dSlitDefectArea = 0;
		//요기서 부터 작업하자..
		//Rect tt_rect(m_boundRect.x - ROI_Margin,m_boundRect.y - ROI_Margin,m_boundRect.width -2*ROI_Margin,m_boundRect.height -2*ROI_Margin);
		Rect tt_rect(m_boundRect.x,m_boundRect.y,m_boundRect.width,m_boundRect.height);
		if(tt_rect.x + tt_rect.width > Dst_Img[Cam_num](tROI).cols || tt_rect.x < 0 || tt_rect.y < 0 || tt_rect.y + tt_rect.height > Dst_Img[Cam_num](tROI).rows) 
		{
			CString strTemp;
			strTemp.Format("Contour 범위 Error!! x: %d, y : %d, width : %d, height : %d",tt_rect.x , tt_rect.y, tt_rect.width, tt_rect.height);
			OutputDebugString(strTemp);
			if(tt_rect.x < 0)
			{
				tt_rect.width += tt_rect.x;
				tt_rect.x = 0;
			}
			if(tt_rect.y < 0)
			{
				tt_rect.height += tt_rect.y;
				tt_rect.y = 0;
			}
			if(tt_rect.x + tt_rect.width >= Dst_Img[Cam_num](tROI).cols)
			{
				tt_rect.width = Dst_Img[Cam_num].cols - tt_rect.x;
			}
			if(tt_rect.y + tt_rect.height>= Dst_Img[Cam_num](tROI).rows)
			{
				tt_rect.height = Dst_Img[Cam_num](tROI).rows- tt_rect.y;
			}
		}
		//#pragma omp parallel for
		for(int i=0; i<contours_Slit_Defect.size(); i++)
		{
			//drawContours( Dst_Img[Cam_num](tt_rect), contours_Slit_Defect, i, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy_Slit_Defect);
			drawContours( Dst_Img[Cam_num](tROI)(tt_rect), contours_Slit_Defect, i, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy_Slit_Defect);
		}
		vec_Result_Value[SLIT_DEFECT_COUNT].push_back((double)countNonZero(Slit_Defect_Img));
		

		//OutputDebugString("Slit 이물 검사 End");
		//nSGYM_Measure_Count = SGYM_INSPECTION_ITEM_END;

		ALG_SGYM_Param.Parameter_Calc_Method.clear();
		///각 항목 別 측정 방법을 반영함.
		for(int i=0; i < SGYM_INSPECTION_ITEM_END; i++)
		{
			ALG_SGYM_Param.Parameter_Calc_Method.push_back(BOLT_Param[0].nMethod_Cal[i+1]);
			if(i == SLIT_HEIGHT_MIN || i == SLIT_WIDTH_MIN) {
				ALG_SGYM_Param.Parameter_Calc_Method[i] = 0;
			}

			if(i == SLIT_HEIGHT_MAX || i == SLIT_WIDTH_MAX) {
				ALG_SGYM_Param.Parameter_Calc_Method[i] = 1;
			}

			if(vec_Result_Value[i].size() > 0)
			{
				switch(ALG_SGYM_Param.Parameter_Calc_Method[i])
				{
				case 0:// Min
					{
						double dMinValue = 100000;
						for(int j=0; j<vec_Result_Value[i].size();j++)
						{
							if(dMinValue > vec_Result_Value[i][j])
							{
								dMinValue = vec_Result_Value[i][j];
							}
						}
						ALG_SGYM_Param.Result_Value[i] = dMinValue;
					}
					break;
				case 1://Max 
					{
						double dMaxValue = -1;
						for(int j=0; j<vec_Result_Value[i].size();j++)
						{
							if(dMaxValue < vec_Result_Value[i][j])
							{
								dMaxValue = vec_Result_Value[i][j];
							}
						}
						ALG_SGYM_Param.Result_Value[i] = dMaxValue;
					}
					break;
				case 3:// Average 
					{
						double dSumValue = 0;
						for(int j=0; j<vec_Result_Value[i].size();j++)
						{
							dSumValue += vec_Result_Value[i][j];
						}
						ALG_SGYM_Param.Result_Value[i] = dSumValue/vec_Result_Value[i].size();
					}
					break;
				case 2:// Range
					{
						double dMinValue = 100000;
						double dMaxValue = -1;
						for(int j=0; j<vec_Result_Value[i].size();j++)
						{
							if(dMinValue > vec_Result_Value[i][j])
							{
								dMinValue = vec_Result_Value[i][j];
							}
							if(dMaxValue < vec_Result_Value[i][j])
							{
								dMaxValue = vec_Result_Value[i][j];
							}
						}
						ALG_SGYM_Param.Result_Value[i] = dMaxValue-dMinValue;
					}
					break;
				case 4://Total 
					{
						double dMaxValue = 0;
						for(int j=0; j<vec_Result_Value[i].size();j++)
						{
							dMaxValue += vec_Result_Value[i][j];
						}
						ALG_SGYM_Param.Result_Value[i] = dMaxValue;
					}
					break;
				}
				vec_Result_Value[i].clear();
			}
			else if(vec_Result_Value[i].size() <= 0)
			{
				//AfxMessageBox("1");
				ALG_SGYM_Param.Result_Value[i] = 0;
			}

			if (ALG_SGYM_Param.Result_Value[i] > 0)
			{
				ALG_SGYM_Param.Result_Value[i] += BOLT_Param[0].Offset[i+1];
			}
		}


		Result_Info[Cam_num].Format("C0:01_%1.3f_C0:02_%1.3f_C0:03_%1.3f_C0:04_%1.3f_C0:05_%1.3f_C0:06_%1.3f_C0:07_%2.1f_C0:08_%2.1f_C0:09_%1.0f_C0:10_%1.0f_C0:11_%1.0f_C0:12_%1.0f_C0:13_%1.0f_C0:14_%1.1f",ALG_SGYM_Param.Result_Value[0],ALG_SGYM_Param.Result_Value[1],ALG_SGYM_Param.Result_Value[2],ALG_SGYM_Param.Result_Value[3],ALG_SGYM_Param.Result_Value[4],ALG_SGYM_Param.Result_Value[5],ALG_SGYM_Param.Result_Value[6],ALG_SGYM_Param.Result_Value[7],ALG_SGYM_Param.Result_Value[8],ALG_SGYM_Param.Result_Value[9],ALG_SGYM_Param.Result_Value[10],ALG_SGYM_Param.Result_Value[11],ALG_SGYM_Param.Result_Value[12],ALG_SGYM_Param.Result_Value[13]);
		//OutputDebugString(Result_Info[Cam_num]);
		//AfxMessageBox(Result_Info[Cam_num]);

		ALG_SGYM_Param.vec_Slit_Info.clear();
		ALG_SGYM_Param.vec_Danja_Info.clear();
	}
	else
	{
		Result_Info[Cam_num].Format("C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C0:08_-1_C0:09_-1_C0:10_-1_C0:11_-1_C0:12_-1_C0:13_-1_C0:14_-1");
	}
}

Rect CImPro_Library::Check_ROI(Rect O_ROI, int W, int H)
{
	Rect t_r = O_ROI;
	if (O_ROI.x < 0)
	{
		t_r.x = 0;
	}
	if (O_ROI.y < 0)
	{
		t_r.y = 0;
	}
	if (O_ROI.width <= 0)
	{
		t_r.width = 10;
	}
	if (O_ROI.height <= 0)
	{
		t_r.height = 10;
	}

	if (O_ROI.x + O_ROI.width >= W)
	{
		if (O_ROI.width >= W)
		{
			O_ROI.width = W - 10;
			t_r.width = W - 10;
		}
		t_r.x = W - O_ROI.width - 1;
	}
	if (O_ROI.y + O_ROI.height >= H)
	{
		if (O_ROI.height >= H)
		{
			O_ROI.height = H - 10;
			t_r.height = H - 10;
		}
		t_r.y = H - O_ROI.height - 1;
	}

	return t_r;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////
// [SURF_Homography] Surf와 perspective transform 을 이용한 차영상 제작
// Implemented by CDJung
// Last modification : 2012.10.08
//----------------------------------------------------------------------------------------------------
// Input :	Mat img_object : 카메라에서 grab된 잘려진 이미지 
//			Mat img_scene  : Template 이미지
// outout :	Mat warp_dst   : img_object가 img_scene에 맞게 변환된 이미지
//			Mat diff_img   : 차영상 = warp_dst - img_scene + 128(bias)
//////////////////////////////////////////////////////////////////////////////////////////////////////
void CImPro_Library::AKAZE_Homography(Mat img_object, Mat img_scene, Mat warp_dst, Mat diff_img)
{
	//imwrite("00.bmp",img_object);
	//imwrite("01.bmp",img_scene);
	if( !img_object.data || !img_scene.data )
	{ std::cout<< " --(!) Error reading images " << std::endl; return;}

	const float inlier_threshold = 2.5f; // Distance threshold to identify inliers
	const float nn_match_ratio = 0.8f;   // Nearest neighbor matching ratio

    Mat homography;
    FileStorage fs("H1to3p.xml", FileStorage::READ);
    fs.getFirstTopLevelNode() >> homography;

    vector<KeyPoint> kpts1, kpts2;
    Mat desc1, desc2;

	//AfxMessageBox("1");
	AKAZE akaze;
    //Ptr<Feature2D> akaze = AKAZE::create("0");
	akaze(img_object, noArray(), kpts1, desc1);
    akaze(img_scene, noArray(), kpts2, desc2);
	//akaze->detect(img_object, kpts1, desc1);
	//akaze->detect(img_scene, kpts2, desc2);

	//AfxMessageBox("2");
    BFMatcher matcher(NORM_HAMMING);
    vector< vector<DMatch> > nn_matches;
    matcher.knnMatch(desc1, desc2, nn_matches, 2);

    vector<KeyPoint> matched1, matched2, inliers1, inliers2;
    vector<DMatch> good_matches;
    for(size_t i = 0; i < nn_matches.size(); i++) {
        DMatch first = nn_matches[i][0];
        float dist1 = nn_matches[i][0].distance;
        float dist2 = nn_matches[i][1].distance;

        if(dist1 < nn_match_ratio * dist2) {
            matched1.push_back(kpts1[first.queryIdx]);
            matched2.push_back(kpts2[first.trainIdx]);
        }
    }

    for(unsigned i = 0; i < matched1.size(); i++) {
        Mat col = Mat::ones(3, 1, CV_64F);
        col.at<double>(0) = matched1[i].pt.x;
        col.at<double>(1) = matched1[i].pt.y;

        col = homography * col;
        col /= col.at<double>(2);
        double dist = sqrt( pow(col.at<double>(0) - matched2[i].pt.x, 2) +
                            pow(col.at<double>(1) - matched2[i].pt.y, 2));

        if(dist < inlier_threshold) {
            int new_i = static_cast<int>(inliers1.size());
            inliers1.push_back(matched1[i]);
            inliers2.push_back(matched2[i]);
            good_matches.push_back(DMatch(new_i, new_i, 0));
        }
    }
	//AfxMessageBox("2");
	//-- Localize the object from img_object in img_scene 
	std::vector<Point2f> obj;
	std::vector<Point2f> scene;

	for( int i = 0; i < good_matches.size(); i++ )
	{
		//-- Get the keypoints from the good matches
		obj.push_back( kpts1[ good_matches[i].queryIdx ].pt );
		scene.push_back( kpts2[ good_matches[i].trainIdx ].pt ); 
	}
	//AfxMessageBox("2");

	//-- Calculate Homography Matrix H
	Mat H = findHomography( obj, scene, 3 );


	//-- Get the corners from the img_object ( the object to be "detected" )
	std::vector<Point2f> obj_corners(4);
	obj_corners[0] = cvPoint(0,0); obj_corners[1] = cvPoint( img_object.cols, 0 );
	obj_corners[2] = cvPoint( img_object.cols, img_object.rows ); obj_corners[3] = cvPoint( 0, img_object.rows );
	std::vector<Point2f> scene_corners(4);

	perspectiveTransform( obj_corners, scene_corners, H);

	//-- Step 4: WarpPerspective
	warpPerspective(img_object, warp_dst, H, warp_dst.size(),INTER_CUBIC);

	//-- Step 5: Make Difference Image between warp_dst and img_scene.
	// diff_img = warp_dst - img_scene + 128(bias)
	AfxMessageBox("2");
	for (int i = 0; i < img_scene.rows; ++i)
	{
		for (int j = 0; j < img_scene.cols; ++j)
		{
			float temp = (float)warp_dst.at<uchar>(i,j) - (float)img_scene.at<uchar>(i,j) + 128;
			if (temp < 0)
			{
				diff_img.at<uchar>(i,j) = 0;
			} else if (temp > 255)
			{
				diff_img.at<uchar>(i,j) = 255;
			} else
			{
				diff_img.at<uchar>(i,j) = (uchar)temp;
			}
		}
	}
	imwrite("02.bmp",diff_img);
}


void CImPro_Library::J_TemplateMatch(Mat graySourceImage, Mat grayTemplateImage, int Cam_num){

    double minVal;
    Point minLoc;
    Point tempLoc;

    Mat binarySourceImage = Mat(graySourceImage.size(),CV_8UC1);
    Mat binaryTemplateImage = Mat(grayTemplateImage.size(),CV_8UC1);

	threshold(graySourceImage, binarySourceImage, 200, 255, CV_THRESH_OTSU );
    threshold(grayTemplateImage, binaryTemplateImage, 200, 255, CV_THRESH_OTSU);

    int templateHeight = grayTemplateImage.rows;
    int templateWidth = grayTemplateImage.cols;

    float templateScale = 0.5f;

    for(int i = 2; i <= 3; i++){

        int tempTemplateHeight = (int)(templateWidth * (i * templateScale));
        int tempTemplateWidth = (int)(templateHeight * (i * templateScale));

        Mat tempBinaryTemplateImage = Mat(Size(tempTemplateWidth,tempTemplateHeight),CV_8UC1);
        Mat result = Mat(Size(graySourceImage.cols - tempTemplateWidth + 1,graySourceImage.rows - tempTemplateHeight + 1),CV_32FC1);

        resize(binaryTemplateImage,tempBinaryTemplateImage,Size(tempBinaryTemplateImage.cols,tempBinaryTemplateImage.rows),0,0,INTER_LINEAR);

        float degree = 20.0f;

        for(int j = 0; j <= (int)(180/degree); j++){

            Mat rotateBinaryTemplateImage = Mat(Size(tempBinaryTemplateImage.cols, tempBinaryTemplateImage.rows), CV_8UC1);

            for(int y = 0; y < tempTemplateHeight; y++){
                for(int x = 0; x < tempTemplateWidth; x++){
                    rotateBinaryTemplateImage.data[y * tempTemplateWidth + x] = 255;
                }
            }


            for(int y = 0; y < tempTemplateHeight; y++){
                for(int x = 0; x < tempTemplateWidth; x++){

                    float radian = (float)j * degree * CV_PI / 180.0f;
                    int scale = y * tempTemplateWidth + x;

                    int rotateY = - sin(radian) * ((float)x - (float)tempTemplateWidth / 2.0f) + cos(radian) * ((float)y - (float)tempTemplateHeight / 2.0f) + tempTemplateHeight / 2;
                    int rotateX = cos(radian) * ((float)x - (float)tempTemplateWidth / 2.0f) + sin(radian) * ((float)y - (float)tempTemplateHeight / 2.0f) + tempTemplateWidth / 2;

                    if(rotateY < tempTemplateHeight && rotateX < tempTemplateWidth && rotateY >= 0 && rotateX  >= 0)
                        rotateBinaryTemplateImage.data[scale] = tempBinaryTemplateImage.data[rotateY * tempTemplateWidth + rotateX];
                }
            }

            matchTemplate(binarySourceImage, rotateBinaryTemplateImage, result, CV_TM_SQDIFF_NORMED);

            minMaxLoc(result, &minVal, 0, &minLoc, 0, Mat());

            cout<<(int)(i * 0.5 * 100)<<" , "<< j * 20<<" , "<< (1 - minVal) * 100<<endl;

            if(minVal < 0.2){ // 1 - 0.065 = 0.935 : 93.5%
                tempLoc.x = minLoc.x + tempTemplateWidth;
                tempLoc.y = minLoc.y + tempTemplateHeight;
                rectangle(Dst_Img[Cam_num], minLoc, tempLoc, CV_RGB(0, 255, 0), 1, 8, 0);
            }
        }
    }
    //return destinationImage;
}



