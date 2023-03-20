#include "StdAfx.h"
#include "ImPro_Library.h"
#include "math.h"
#define PI 3.14159265

struct compare_Size {
	bool operator() (Point4D a, Point4D b) { return (a.CY  < b.CY);}
} Point_compare_Size;

struct compare_Size_X {
	bool operator() (Point4D a, Point4D b) { return (a.CX  < b.CX);}
} Point_compare_Size_X;


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
	number_of_iterations = 50;
	termination_eps = 0.01;
	warpType = "euclidean";
}


CImPro_Library::~CImPro_Library(void)
{
}

#pragma region 전역 함수
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
	//Rect m_ROI;
	//m_ROI.x = 0;m_ROI.y = 0;
	//m_ROI.width = (int)(4*(size_x/4));
	//m_ROI.height = (int)(4*(size_y/4));

	//BOLT_Param[Cam_num].nRotationAngle = 10;

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src.clone();
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		cvtColor(src,Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
		//cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
		//Dst_Img[Cam_num] = src.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		Gray_Img[Cam_num] = src.clone();
		Rotation_Calibration(Cam_num);
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		//Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}


void CImPro_Library::Set_Image_1(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	//Rect m_ROI;
	//m_ROI.x = 0;m_ROI.y = 0;
	//m_ROI.width = (int)(4*(size_x/4));
	//m_ROI.height = (int)(4*(size_y/4));

	//BOLT_Param[Cam_num].nRotationAngle = 10;

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src.clone();
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		cvtColor(src,Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
		//cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
		//Dst_Img[Cam_num] = src.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		Gray_Img[Cam_num] = src.clone();
		Rotation_Calibration(Cam_num);
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		//Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}



void CImPro_Library::Set_Image_2(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	//Rect m_ROI;
	//m_ROI.x = 0;m_ROI.y = 0;
	//m_ROI.width = (int)(4*(size_x/4));
	//m_ROI.height = (int)(4*(size_y/4));

	//BOLT_Param[Cam_num].nRotationAngle = 10;

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src.clone();
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		cvtColor(src,Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
		//cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
		//Dst_Img[Cam_num] = src.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		Gray_Img[Cam_num] = src.clone();
		Rotation_Calibration(Cam_num);
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		//Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}


void CImPro_Library::Set_Image_3(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	//Rect m_ROI;
	//m_ROI.x = 0;m_ROI.y = 0;
	//m_ROI.width = (int)(4*(size_x/4));
	//m_ROI.height = (int)(4*(size_y/4));

	//BOLT_Param[Cam_num].nRotationAngle = 10;

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src.clone();
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		cvtColor(src,Gray_Img[Cam_num],CV_BGR2GRAY);
		Rotation_Calibration(Cam_num);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
		//cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
		//Dst_Img[Cam_num] = src.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		//if (abs(BOLT_Param[Cam_num].nRotationAngle) > 0)
		//{
		//	J_Rotate(src,BOLT_Param[Cam_num].nRotationAngle,src);
		//}
		Gray_Img[Cam_num] = src.clone();
		Rotation_Calibration(Cam_num);
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		//Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}



void CImPro_Library::Reset_Dst_Image(int Cam_num)
{
	if (!Gray_Img[Cam_num].empty())
	{
		Dst_Img[Cam_num] = NULL;
		Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
	}
}

void CImPro_Library::J_Fitting_Ellipse(Mat &Binary,double* Circle_Info)
{
	vector<vector<Point> > contours;
	vector<Vec4i> hierarchy;
	float m_length = 0;int idx = 0;
	float m_area = 0;
	Mat Tmp_Img = Binary.clone();

	findContours( Tmp_Img, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
	//Binary = Mat::zeros(Binary.size(), CV_8UC1);
	int max_num = 0; int m_max = 0;
	vector<Rect> boundRect( contours.size() );
	int m_max_object_num = -1;int m_max_object_value = 0;
	for( int k = 0; k < contours.size(); k++ )
	{  
		boundRect[k] = boundingRect( Mat(contours[k]) );
		if (m_max_object_value<= boundRect[k].width*boundRect[k].height)
		{
			m_max_object_value = boundRect[k].width*boundRect[k].height;
			m_max_object_num = k;
		}
	}
	if (m_max_object_num == -1)
	{
		return;
	}
	max_num = m_max_object_num;
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

	//	const int no_data = (int)total_contours[0].size();
	//	sPoint *data = new sPoint[no_data];

	//	if (no_data > 10)
	//	{
	//		for(int i=0; i<no_data; i++)
	//		{
	//			data[i].x =  (double)total_contours[0][i].x;
	//			data[i].y =  (double)total_contours[0][i].y;
	//		}

	//		double cost = ransac_ellipse_fitting (data, no_data, ALG_CAM0_Param.c_ellipse, 50);

	//		Circle_Info[0] = ALG_CAM0_Param.c_ellipse.cx;
	//		Circle_Info[1] = ALG_CAM0_Param.c_ellipse.cy;

	//		Circle_Info[2] = ALG_CAM0_Param.c_ellipse.w*2;
	//		Circle_Info[3] = ALG_CAM0_Param.c_ellipse.h*2;
	//		Circle_Info[4] = ALG_CAM0_Param.c_ellipse.theta*180/M_PI;
	//	}
	//	delete [] data;
	//} else
	//{
	sEllipse c_ellipse;
	for( size_t k = 0; k < contours.size(); k++ ) 
	{	
		const int no_data = (int)contours[max_num].size();
		sPoint *data = new sPoint[no_data];

		if (no_data > 10)
		{
			for(int i=0; i<no_data; i++)
			{
				data[i].x =  (double)contours[max_num][i].x;
				data[i].y =  (double)contours[max_num][i].y;
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
	//}
}

void CImPro_Library::J_Fill_Hole(Mat &Binary)
{
	Mat holes=Binary.clone();
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

	for( size_t k = 0; k < contours.size(); k++ ) 
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
		dst = src.clone();
	}
	//int len = std::max(src.cols, src.rows);
	cv::Point2f pt(src.cols/2, 0);
	//cv::Point2f pt(BOLT_Param[Cam_num].nRect[0].x, BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height);
	cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,0,CV_RGB(255,255,255));
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,0,CV_RGB(255,255,255));
	}
}

void CImPro_Library::J_Rotate_Black(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num)
{
	if (angle == 0)
	{
		dst = src.clone();
	}
	//int len = std::max(src.cols, src.rows);
	cv::Point2f pt(src.cols/2, 0);
	//cv::Point2f pt(BOLT_Param[Cam_num].nRect[0].x, BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height);
	cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,0,CV_RGB(0,0,0));
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,0,CV_RGB(0,0,0));
	}
}


void CImPro_Library::J_Rotate_PRE(cv::Mat& src, double angle, cv::Mat& dst, int Cam_num)
{
	if (angle == 0)
	{
		dst = src.clone();
	}
	//int len = std::max(src.cols, src.rows);
	cv::Point2f pt(src.cols/2, src.rows/2);
	//cv::Point2f pt(BOLT_Param[Cam_num].nRect[0].x, BOLT_Param[Cam_num].nRect[0].y+BOLT_Param[Cam_num].nRect[0].height);
	cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_TRANSPARENT);
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,BORDER_TRANSPARENT);
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
	int match_method = CV_TM_CCORR_NORMED;
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


Point2f CImPro_Library::J_Model_Find(int Cam_num)
{
	bool t_license = Easy::CheckLicense(LicenseFeatures::EasyFind);
	if (t_license == false)
	{// 유레시스 없으면
		// start timing
		const double tic_init = (double) getTickCount();

		CString msg;

		int mode_temp;
		if (warpType == "translation")
			mode_temp = MOTION_TRANSLATION;
		else if (warpType == "euclidean")
			mode_temp = MOTION_EUCLIDEAN;
		else if (warpType == "affine")
			mode_temp = MOTION_AFFINE;
		else
			mode_temp = MOTION_HOMOGRAPHY;

		Mat cp_Cam_Img = Gray_Img[Cam_num].clone();

		// Similarity 계산
		Mat ROI_Template_R_Img;
		//AfxMessageBox("0");
		J_FastMatchTemplate(cp_Cam_Img, Template_Img[Cam_num], ROI_Template_R_Img);
		//AfxMessageBox("end");
		Point matchLoc;
		double minval, maxval;				// Matching된 Similarity 최소, 최대값
		cv::Point minLoc, maxLoc;
		cv::minMaxLoc(ROI_Template_R_Img, &minval, &maxval, &minLoc, &maxLoc);
		matchLoc = maxLoc;					// Matching된 좌,상점

		Rect match_Roi(matchLoc.x,matchLoc.y,Template_Img[Cam_num].cols,Template_Img[Cam_num].rows);
		Mat target_image = cp_Cam_Img(match_Roi).clone();

		const int warp_mode = mode_temp;

		// initialize or load the warp matrix
		Mat warp_matrix;
		if (warpType == "homography")
			warp_matrix = Mat::eye(3, 3, CV_32F);
		else
			warp_matrix = Mat::eye(2, 3, CV_32F);

		if (warp_mode != MOTION_HOMOGRAPHY)
			warp_matrix.rows = 2;

		double cc = findTransformECC (Template_Img[Cam_num], target_image, warp_matrix, warp_mode,
			TermCriteria (TermCriteria::COUNT+TermCriteria::EPS,
			number_of_iterations, termination_eps));

		if (cc == -1)
		{
			BOLT_Param[Cam_num].Object_Postion = Point(-1,-1);
			return Point2f(-1,-1);
		}
		// draw boundaries of corresponding regions
		Mat identity_matrix = Mat::eye(3,3,CV_32F);

		//AfxMessageBox("1");
		Point2f R_Center = draw_warped_roi(cp_Cam_Img, Template_Img[Cam_num].cols-2, Template_Img[Cam_num].rows-2, warp_matrix, matchLoc.x, matchLoc.y,Cam_num);

		//R_Center.x = (double)matchLoc.x + (double)BOLT_Param[Cam_num].nRect[0].x;
		//R_Center.y = (double)matchLoc.y + (double)BOLT_Param[Cam_num].nRect[0].y;

		// end timing
		const double toc_final  = (double) getTickCount ();
		const double total_time = (toc_final-tic_init)/(getTickFrequency());
		if (cc >= 0.05)
		{
			msg.Format("Center_%1.5f_%1.5f_TT_%1.3f_Error_%1.3f",R_Center.x,R_Center.y,total_time,cc);
			circle(Dst_Img[Cam_num], R_Center,3,CV_RGB(0,0,255),1);
			circle(Dst_Img[Cam_num], R_Center,1,CV_RGB(255,0,0),1);
		} else
		{
			msg.Format("Center_-1_-1_TT_%1.3f_Error_%1.3f_%s",total_time,cc,"");
			//Cam0_Result_Data[Seq_num].Format("Center_%1.3f_%1.3f_TT_%1.3f_Error_%1.3f_%s",-1,-1,total_time,cc,Cam0_Result_Data[Seq_num]);
		}
		return R_Center;
	}
	else
	{// 유레시스 있으면
		float c_x = -1;float c_y = -1;float c_angle = 0;
		AfxMessageBox("1");
		if (m_Cam_Find[Cam_num].GetLearningDone())
		{	
		AfxMessageBox("2");
			Mat temp_img = Gray_Img[Cam_num].clone();
			EBW8Image1[Cam_num].SetImagePtr(temp_img.cols,temp_img.rows,temp_img.data);
			m_Cam_FoundPattern[Cam_num].clear();
			m_Cam_FoundPattern[Cam_num] = m_Cam_Find[Cam_num].Find(&EBW8Image1[Cam_num]);
			if (m_Cam_FoundPattern[Cam_num].size() > 0)
			{
		AfxMessageBox("3");
				c_x = m_Cam_FoundPattern[Cam_num][0].GetCenter().GetX();
				c_y = m_Cam_FoundPattern[Cam_num][0].GetCenter().GetY();
				c_angle = m_Cam_FoundPattern[Cam_num][0].GetAngle();
				Mat warp_mat = cv::getRotationMatrix2D(Point2f(c_x,c_y), c_angle, 1.0);
				warpAffine( temp_img, Gray_Img[Cam_num], warp_mat, Gray_Img[Cam_num].size(),1,0,CV_RGB(255,255,255));
				Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
				cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
				CString msg;
				msg.Format("%d_%1.3f,%1.3f,%1.3f",Cam_num,c_x,c_y,c_angle);
				AfxMessageBox(msg);
				imwrite("01_0.bmp",temp_img);			
				imwrite("01_1.bmp",Gray_Img[Cam_num]);			
				imwrite("01_2.bmp",Template_Img[Cam_num]);			
				E_Cam_Img[Cam_num].Save("00_1.bmp");
			}
		}
		return Point2f(c_x,c_y);
	}

	return Point2f(-1, -1);
}

#pragma endregion

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
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		if (Cam_num == 0 && BOLT_Param[Cam_num].nTableType == 5 && BOLT_Param[Cam_num].nROI0_FilterSize[0] == 7496)
		{
			RUN_Algorithm_SUGIYAMA();
			return true;
		}
		for (int s=0;s<41;s++)
		{
			// 설정된 ROI Backup
			BOLT_Param[Cam_num].nRect_Backup[s] = BOLT_Param[Cam_num].nRect[s];

			// 설정된 ROI 유효성 검사
			if (BOLT_Param[Cam_num].nUse[s] == 0 || BOLT_Param[Cam_num].nRect[s].width <= 0 || BOLT_Param[Cam_num].nRect[s].height <= 0
				|| BOLT_Param[Cam_num].nRect[s].width > Gray_Img[Cam_num].cols || BOLT_Param[Cam_num].nRect[s].height > Gray_Img[Cam_num].rows)
				//|| Template_Img[Cam_num].rows != Gray_Img[Cam_num].rows || Template_Img[Cam_num].cols != Gray_Img[Cam_num].cols)
			{
				if (s > 0)
				{
					Result_Info[Cam_num].Format("%sC%d:%02d_-2_",Result_Info[Cam_num],Cam_num,s);
				}
				continue;
			}

			// 변수 설정
			vector<vector<Point> > contours;
			vector<Vec4i> hierarchy;
			CString msg;
			Mat Out_binary;
			Mat Out_binary_Tmp;
			Mat CP_Gray_Img;

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
				if (tROI.x + tROI.width >= Gray_Img[Cam_num].cols)
				{
					tROI.width = Gray_Img[Cam_num].cols - tROI.x -2;
					t_ROI_Check = false;
				}
				if (tROI.y + tROI.height >= Gray_Img[Cam_num].rows)
				{
					tROI.height = Gray_Img[Cam_num].rows - tROI.y -2;
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
					for (int s=1;s<41;s++)
					{
						Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num], Cam_num, s);
					}
					return true;

				}
				BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
				CP_Gray_Img = Gray_Img[Cam_num](tROI).clone();
				if (m_Text_View[Cam_num] && !ROI_Mode && BOLT_Param[Cam_num].nCamPosition == 0)
				{
					rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
					msg.Format("Obj. ROI");
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,50), 1, 8);
				}
				if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && BOLT_Param[Cam_num].nCamPosition == 0)
				{
					rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,50),1);
					msg.Format("Obj. ROI");
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,255,50), 1, 8);
				}
			} else
			{
				// ROI를 Offset 이동후 유효영역 절대좌표로 변경
				tROI.x += BOLT_Param[Cam_num].Offset_Object_Postion.x;
				tROI.y += BOLT_Param[Cam_num].Offset_Object_Postion.y;
				tROI.x -= BOLT_Param[Cam_num].nRect[0].x;
				tROI.y -= BOLT_Param[Cam_num].nRect[0].y;
				tROI = Check_ROI(tROI, BOLT_Param[Cam_num].Gray_Obj_Img.cols, BOLT_Param[Cam_num].Gray_Obj_Img.rows);
				//if (tROI.x < 0)
				//{
				//	tROI.x = 0;
				//}
				//if (tROI.y < 0)
				//{
				//	tROI.y = 0;
				//}
				//if (tROI.x + tROI.width >= BOLT_Param[Cam_num].Gray_Obj_Img.cols-1)
				//{
				//	tROI.width = BOLT_Param[Cam_num].Gray_Obj_Img.cols - tROI.x -2;
				//}
				//if (tROI.y + tROI.height >= BOLT_Param[Cam_num].Gray_Obj_Img.rows-1)
				//{
				//	tROI.height = BOLT_Param[Cam_num].Gray_Obj_Img.rows - tROI.y -2;
				//}
				CP_Gray_Img = BOLT_Param[Cam_num].Gray_Obj_Img(tROI).clone();

				// 그리기위해 ROI 정보 갱신
				BOLT_Param[Cam_num].nRect[s] = tROI;
				BOLT_Param[Cam_num].nRect[s].x += BOLT_Param[Cam_num].nRect[0].x;
				BOLT_Param[Cam_num].nRect[s].y += BOLT_Param[Cam_num].nRect[0].y;
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
				//else
				//{
				//	inRange(CP_Gray_Img, Scalar(BOLT_Param[Cam_num].nThres_V1[s]),Scalar(BOLT_Param[Cam_num].nThres_V2[s]),Out_binary);
				//}
			}


			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Binary_Img.bmp",Cam_num,s);
				imwrite(msg.GetBuffer(),Out_binary);
			}
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
						for (int s=1;s<41;s++)
						{
							Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num], Cam_num, s);
						}
						BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
						return true;
					}

					AfxMessageBox("main_1");
					if (BOLT_Param[Cam_num].nMethod_Thres[0] == THRES_METHOD::FIRSTROI) // ROI#01 모델사용
					{
						AfxMessageBox("main_2");
						//Template_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();
						Point2f R_Center = J_Model_Find(Cam_num);
						// 머리를 못 찾을 경우 Error 처리함.
						AfxMessageBox("main_3");
						if(R_Center.x == -1 || R_Center.y == -1)
						{
							rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
							msg.Format("No Object!");
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
							//return true;
						}
						else
						{
							BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
							BOLT_Param[Cam_num].Offset_Object_Postion.x = R_Center.x - BOLT_Param[Cam_num].Object_Postion.x;
							BOLT_Param[Cam_num].Offset_Object_Postion.y = R_Center.y - BOLT_Param[Cam_num].Object_Postion.y;

							//if (m_Text_View[Cam_num]) // ROI 영역 표시
							//{
							//	circle(Dst_Img[Cam_num],BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
							//	circle(Dst_Img[Cam_num],BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
							//}
						}
						//AfxMessageBox("1");
					}
					else
					{
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


							//J_Delete_Boundary(Out_binary,1);

							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);
							int m_min_object_num = -1; int m_min_object_value = 999999;
							int m_max_object_num = -1; int m_max_object_value = 0;
							int area, left, top, width, height = { 0, };
							for (int j = 1; j < numOfLables; j++)
							{
								area = stats.at<int>(j, CC_STAT_AREA);
								if ((double)area >= BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] && (double)area <= BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s])
								{
									left = BOLT_Param[Cam_num].nRect[s].x + stats.at<int>(j, CC_STAT_LEFT);
									top = BOLT_Param[Cam_num].nRect[s].y + stats.at<int>(j, CC_STAT_TOP);
									width = stats.at<int>(j, CC_STAT_WIDTH);
									height = stats.at<int>(j, CC_STAT_HEIGHT);

									if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
									{
										if (left <= m_min_object_value)
										{
											m_min_object_value = left;
											m_min_object_num = j;
										}
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
									{
										if (left + width >= m_max_object_value)
										{
											m_max_object_value = left + width;
											m_max_object_num = j;
										}
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
									{
										double t_dist = sqrt((double)left*(double)left + (double)top*(double)top);
										if ((int)t_dist <= m_min_object_value)
										{
											m_min_object_value = (int)t_dist;
											m_min_object_num = j;
										}
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
									{
										double t_dist = sqrt((double)left*(double)left + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
										if ((int)t_dist <= m_min_object_value)
										{
											m_min_object_value = (int)t_dist;
											m_min_object_num = j;
										}
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
									{
										double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top)*(double)(top));
										if ((int)t_dist <= m_min_object_value)
										{
											m_min_object_value = (int)t_dist;
											m_min_object_num = j;
										}
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
									{
										double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
										if ((int)t_dist <= m_min_object_value)
										{
											m_min_object_value = (int)t_dist;
											m_min_object_num = j;
										}
									}
									else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::CENTER)
									{
										if (area >= m_max_object_value)
										{
											m_max_object_value = area;
											m_max_object_num = j;
										}
									}

									if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
											CV_RGB(0, 255, 0), 1);

										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("%d", area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(0), Point(BOLT_Param[Cam_num].nRect[s].x + x - 5, BOLT_Param[Cam_num].nRect[s].y + y), fontFace, fontScale, CV_RGB(0, 100, 255), 1, 8);
									}
								}
							}


							// 머리를 못 찾을 경우 Error 처리함.
							if(m_min_object_num == -1 && m_max_object_num == -1)
							{
								Result_Info[Cam_num] = "";
								for (int s=1;s<41;s++)
								{
									Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
								}
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								msg.Format("No Object!");
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
								BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
								return true;
							}

							BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();

							if (m_min_object_num >= 0)
							{
								left = BOLT_Param[Cam_num].nRect[s].x + stats.at<int>(m_min_object_num, CC_STAT_LEFT);
								top = BOLT_Param[Cam_num].nRect[s].y + stats.at<int>(m_min_object_num, CC_STAT_TOP);
								width = stats.at<int>(m_min_object_num, CC_STAT_WIDTH);
								height = stats.at<int>(m_min_object_num, CC_STAT_HEIGHT);
							}
							else if (m_max_object_num >= 0)
							{
								left = BOLT_Param[Cam_num].nRect[s].x + stats.at<int>(m_max_object_num, CC_STAT_LEFT);
								top = BOLT_Param[Cam_num].nRect[s].y + stats.at<int>(m_max_object_num, CC_STAT_TOP);
								width = stats.at<int>(m_max_object_num, CC_STAT_WIDTH);
								height = stats.at<int>(m_max_object_num, CC_STAT_HEIGHT);
							}
							else
							{
								BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
								BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
								if (Result_Debugging)
								{
									msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp",Cam_num, s);
									imwrite(msg.GetBuffer(), BOLT_Param[Cam_num].Gray_Obj_Img);
									msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Thres_Img.bmp",Cam_num, s);
									imwrite(msg.GetBuffer(), BOLT_Param[Cam_num].Thres_Obj_Img);
								}
								BOLT_Param[Cam_num].Offset_Object_Postion = Point(0, 0);
								continue;
							}
							int x = left + width / 2;
							int y = top + height / 2;

							if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
							{
								x = left; y = BOLT_Param[Cam_num].Object_Postion.y;
							}
							else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
							{
								x = left + width; y = BOLT_Param[Cam_num].Object_Postion.y;
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

							BOLT_Param[Cam_num].Offset_Object_Postion = Point(x - BOLT_Param[Cam_num].Object_Postion.x, y - BOLT_Param[Cam_num].Object_Postion.y);

							if(abs(BOLT_Param[Cam_num].Offset_Object_Postion.x) > Out_binary.cols/3 || abs(BOLT_Param[Cam_num].Offset_Object_Postion.y) > Out_binary.rows/3)
							{
								Result_Info[Cam_num] = "";
								for (int s=1;s<41;s++)
								{
									Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
								}
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								msg.Format("Out of ROI!");
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
								BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
								return true;
							}

							if (Result_Debugging)
							{
								msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp", Cam_num, s);
								imwrite(msg.GetBuffer(), BOLT_Param[Cam_num].Gray_Obj_Img);
							}

							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								rectangle(Dst_Img[Cam_num], Point(left,top), Point(left+width,top+height),  
									CV_RGB(255,100,0),1 );  
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
									}
								}

								// 사이드 바닦 찾기
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
								else
								{
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
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
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}

								if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
								{
									line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
									Rect t_R_ROI(0,t_Left_y+2,Out_binary.cols,Out_binary.rows-(t_Left_y+2));
									Out_binary(t_R_ROI) = 0;
								}
								//if (Result_Debugging) // 오링 사이드 바닦 라인 이미지
								//{
								//	msg.Format("Save\\Debugging\\Cam%d_%2d_Line_Thres_ROI_Gray_Img.bmp",Cam_num, s);
								//	imwrite(msg.GetBuffer(),Out_binary);		
								//}

								erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
								dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);

								// 못 머리만 찾기
								findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								if(contours.size() == 0) // 칸투어 갯수로 예외처리
								{
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}

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

								// 머리를 못 찾을 경우 Error 처리함.
								if(m_max_object_num == -1)
								{
									Result_Info[Cam_num] = "";
									for (int s=1;s<41;s++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
									msg.Format("No Object!");
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}

								if (m_max_object_num >= 0)
								{
									Mat Target_Thres_ROI_Gray_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									drawContours( Target_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8);
									BOLT_Param[Cam_num].Thres_Obj_Img = Out_binary.clone();
									BOLT_Param[Cam_num].Gray_Obj_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[0]).clone();
									//bitwise_and(BOLT_Param[Cam_num].Gray_Obj_Img,Target_Thres_ROI_Gray_Img,BOLT_Param[Cam_num].Gray_Obj_Img);
									//bitwise_and(BOLT_Param[Cam_num].Thres_Obj_Img,Target_Thres_ROI_Gray_Img,BOLT_Param[Cam_num].Thres_Obj_Img);
									BOLT_Param[Cam_num].Offset_Object_Postion.x = boundRect[m_max_object_num].x - BOLT_Param[Cam_num].Object_Postion.x;
									BOLT_Param[Cam_num].Offset_Object_Postion.y = boundRect[m_max_object_num].y - BOLT_Param[Cam_num].Object_Postion.y;
									if (Result_Debugging)
									{
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Gray_Img.bmp",Cam_num,s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Gray_Obj_Img);
										msg.Format("Save\\Debugging\\CAM%d_ROI%2d_Obj_Thres_Img.bmp",Cam_num,s);
										imwrite(msg.GetBuffer(),BOLT_Param[Cam_num].Thres_Obj_Img);
									}
									//msg.Format("%d,%d",BOLT_Param[Cam_num].Offset_Object_Postion.x,BOLT_Param[Cam_num].Offset_Object_Postion.y);
									//AfxMessageBox(msg);
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
									for (int s=1;s<41;s++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
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
								int m_max_object_num = -1;int m_max_object_value = 0;
								//RotatedRect minRect;
								//Point2f rect_points[4];
								//findContours( t_morph(t_Sub_ROI).clone(), BOLT_Param[Cam_num].Object_contours, BOLT_Param[Cam_num].Object_hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								//if (BOLT_Param[Cam_num].Object_contours.size() != 0)
								//{
								//	for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
								//	{  
								//		minRect = minAreaRect(BOLT_Param[Cam_num].Object_contours[k]);
								//		if (m_max_object_value<= minRect.size.height)
								//		{
								//			m_max_object_value = minRect.size.height;
								//			if(minRect.size.width<minRect.size.height){
								//				t_angle = minRect.angle;
								//			}else{
								//				t_angle = -(270 - minRect.angle);
								//			}
								//			if (abs(t_angle) > 350 && abs(t_angle) <= 360)
								//			{
								//				if (t_angle < 0)
								//				{
								//					t_angle = 360+t_angle;
								//				}
								//				else
								//				{
								//					t_angle = 360-t_angle;
								//				}
								//			}
								//			if (abs(t_angle) > 10)
								//			{
								//				t_angle = 0.0;
								//			}
								//			m_max_object_num = k;
								//		}
								//	}
								//}
								//else
								//{
								//	if (m_Text_View[Cam_num] && !ROI_Mode)
								//	{
								//		rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
								//		msg.Format("ROI#%d", s);
								//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(0,255,0), 1, 8);
								//	}
								//	if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								//	{
								//		rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,255,0),1);
								//		msg.Format("ROI#%d", s);
								//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(0,255,0), 1, 8);
								//	}

								//	rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								//	msg.Format("No Object!");
								//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, 1, CV_RGB(255,100,0), 1, 8);
								//	Result_Info[Cam_num] = "";
								//	for (int s=1;s<41;s++)
								//	{
								//		Result_Info[Cam_num].Format("%sC%d:%02d_0_",Result_Info[Cam_num],Cam_num,s);
								//	}
								//	BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
								//	return true;
								//}
								//msg.Format("%1.3f",t_angle);
								//AfxMessageBox(msg);
								//if (m_max_object_num == -1)
								//{
								//	rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
								//	msg.Format("No Object!");
								//	putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, 1, CV_RGB(255,100,0), 1, 8);
								//	Result_Info[Cam_num] = "";
								//	for (int s=1;s<41;s++)
								//	{
								//		Result_Info[Cam_num].Format("%sC%d:0%d_0_",Result_Info[Cam_num],Cam_num,s);
								//	}
								//	return true;
								//}

								Mat Target_Thres_ROI_Gray_Img;
								J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
								J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
								J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
								dilate(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),10);
								erode(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),10);

								Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();

								if (BOLT_Param[Cam_num].nThickness[0] > 0)
								{
									int t_filter_cnt = (int)(0.75*BOLT_Param[Cam_num].nThickness[0]/BOLT_Param[Cam_num].nResolution[0]);
									if (t_filter_cnt > BOLT_Param[Cam_num].nRect[s].width/1.5)
									{
										t_filter_cnt = BOLT_Param[Cam_num].nRect[s].width/1.5;
									}
									erode(Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1), t_filter_cnt);
									dilate(Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1), t_filter_cnt);
								}

								dilate(Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,element,Point(-1,-1),1);
								subtract(Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img);

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
									rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
									//msg.Format("Obj. ROI");
									//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,0,255), 1, 8);
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}

								if (m_max_object_num >= 0 && m_2nd_max_object_num >= 0)
								{
									BOLT_Param[Cam_num].Offset_Object_Postion = Point(t_Head_rect.x - BOLT_Param[Cam_num].Object_Postion.x,
										t_Head_rect.y - BOLT_Param[Cam_num].Object_Postion.y);
									BOLT_Param[Cam_num].Thres_Obj_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
									drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_2nd_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										vector<vector<Point> > total_contours(1);
										for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
										{
											total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
										}
										for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num].size();j++)
										{
											total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num][j]);
										}

										tt_rect = boundingRect( Mat(total_contours[0]) );
										Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
										//t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
										//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
										//t_Head_rect.x-=1;t_Head_rect.y-=1;t_Head_rect.width+=2;t_Head_rect.height+=2;
										//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										vector<vector<Point> > total_contours(1);
										for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
										{
											total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
										}
										for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num].size();j++)
										{
											total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num][j]);
										}
										BOLT_Param[Cam_num].Thres_Obj_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
										drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
										drawContours( BOLT_Param[Cam_num].Thres_Obj_Img,  BOLT_Param[Cam_num].Object_contours, m_2nd_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

										tt_rect = boundingRect( Mat(total_contours[0]) );
										Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
										t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
										t_Head_rect.x-=1;t_Head_rect.y-=1;t_Head_rect.width+=2;t_Head_rect.height+=2;
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
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
									for (int s=1;s<41;s++)
									{
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
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
								int m_max_object_num = -1;int m_max_object_value = 0;

								Mat Target_Thres_ROI_Gray_Img;
								J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
								J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
								J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);

								Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();

								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);
									rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
									//msg.Format("Obj. ROI");
									//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,0,255), 1, 8);
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}

								m_max_object_num = -1;
								m_max_object_value = 0;
								Rect tt_rect;Rect t_Side_rect;
								for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
								{
									tt_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
									if (m_max_object_value<= tt_rect.width*tt_rect.height 
										&& tt_rect.x > 1 && tt_rect.y >= 0 && tt_rect.x + tt_rect.width < Out_binary.cols-2 && tt_rect.y + tt_rect.height < Out_binary.rows-2)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
									}
									BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
									return true;
								}
								BOLT_Param[Cam_num].Object_1st_idx = m_max_object_num;
								t_Side_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[m_max_object_num]) );

								if (m_max_object_num >= 0)
								{
									BOLT_Param[Cam_num].Offset_Object_Postion = Point(t_Side_rect.x - BOLT_Param[Cam_num].Object_Postion.x,
										t_Side_rect.y - BOLT_Param[Cam_num].Object_Postion.y);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
			int start_p = -1;
			int end_p = -1;
			vector<double> dist_vec;
			bool check_angle_line_init = false;
			double v_Dist = 0;
			if (BOLT_Param[Cam_num].nCamPosition == 0)
			{// TOP, BOTTOM
				if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::W_LENGTH_TB) // Horizontal Length
				{
					for(int i=0;i<Out_binary.rows;i++)
					{
						int ii = i+(int)((double)Out_binary.cols*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
						for(int j=0;j<Out_binary.cols;j++)
						{
							if (Out_binary.at<uchar>(i,j) == 255 && start_p == -1)
							{
								start_p = j;
								break;
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),1);
								}
								check_angle_line_init = true;
							}
							for(int j=Out_binary.cols-1;j>=0;j--)
							{
								ii = i+(int)((double)(Out_binary.cols-start_p-((Out_binary.cols-1)-j))*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
								if (ii >= 0 && ii < Out_binary.rows)
								{
									if (Out_binary.at<uchar>(ii,j) == 255 && end_p == -1)
									{
										end_p = j;
										break;
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					for(int j=0;j<Out_binary.cols;j++)
					{
						int jj = j+(int)((double)Out_binary.rows*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));

						for(int i=0;i<Out_binary.rows;i++)
						{
							if (Out_binary.at<uchar>(i,j) == 255 && start_p == -1)
							{
								start_p = i;
								break;
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),1);
								}
								check_angle_line_init = true;
							}
							for(int i=Out_binary.rows-1;i>=0;i--)
							{
								jj = j+(int)((double)(Out_binary.rows-start_p-((Out_binary.rows-1)-i))*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
								if (jj >= 0 && jj < Out_binary.cols)
								{
									if (Out_binary.at<uchar>(i,jj) == 255 && end_p == -1)
									{
										end_p = i;
										break;
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						//		if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						//			if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						//		if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						//		if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						//		if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						//			if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							if (m_max_object_num >= 0)
							{
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
								}

								Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
								drawContours( Out_binary,  contours, m_max_object_num, CV_RGB(255,255,255), 1, 1, BOLT_Param[Cam_num].Object_hierarchy);
								//imwrite("01.bmp",Out_binary);
								RotatedRect Incircle_Info = fitEllipse(Mat(contours[m_max_object_num]));
								Rect m_max_rect = boundingRect( Mat(contours[m_max_object_num]) );
								double minval =0, maxval=0;
								cv::Point minLoc, CenterLoc;
								maxval = max(Incircle_Info.size.width,Incircle_Info.size.height)/2;
								CenterLoc = Incircle_Info.center;
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								circle(Our_Circle_Img, Incircle_Info.center,t_max,CV_RGB(255,255,255),5);
								bitwise_and(Our_Circle_Img, Out_binary, Our_Circle_Img);
								findContours( Our_Circle_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								double t_min = 99999;cv::Point2f t_max_dist_point2(0,0);
								for (int k = 0; k < contours.size(); k++)
								{  
									minRect = minAreaRect(contours[k]);
									t_max_temp =  sqrt(((minRect.center.x - (double)t_max_dist_point.x)*(minRect.center.x - (double)t_max_dist_point.x) + (minRect.center.y - (double)t_max_dist_point.y)*(minRect.center.y - (double)t_max_dist_point.y)));
									if (t_min >= t_max_temp)
									{
										t_min = t_max_temp;
										t_max_dist_point2 = cv::Point2f((t_max/t_max_temp)*(minRect.center.x -Incircle_Info.center.x) + Incircle_Info.center.x, (t_max/t_max_temp)*(minRect.center.y -Incircle_Info.center.y) + Incircle_Info.center.y);
									}
								}

								Mat Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								cv::Point2f t_max_dist_point_rotate(0,0);
								cv::Point2f t_max_dist_point_R(1.05f*(t_max_dist_point2.x-Incircle_Info.center.x),1.05f*(t_max_dist_point2.y-Incircle_Info.center.y));
								line( Line_Img, Incircle_Info.center, cv::Point2f(t_max_dist_point_R.x + Incircle_Info.center.x,t_max_dist_point_R.y + Incircle_Info.center.y) ,CV_RGB(255,255,355), 1, 1 );
								bitwise_and(Line_Img, Out_binary, Line_Img);
								vector<cv::Point2f> t_vec_point;bool t_loop_check = false;
								for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
								{
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
								
								for (int k=1;k<BOLT_Param[Cam_num].nCrossAngleNumber[s];k++)
								{
									Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									t_max_dist_point_rotate.x = cos((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.x
										                      + sin((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;
									t_max_dist_point_rotate.y = -sin((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.x
										+ cos((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;

									line( Line_Img, Incircle_Info.center, cv::Point2f(t_max_dist_point_rotate.x + Incircle_Info.center.x,t_max_dist_point_rotate.y + Incircle_Info.center.y) ,CV_RGB(255,255,355), 1, 1 );
									bitwise_and(Line_Img, Out_binary, Line_Img);
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

								if (m_Text_View[Cam_num])
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
											if (m_Text_View[Cam_num])
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
							if (m_max_object_num >= 0)
							{
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
								}

								Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
								drawContours( Out_binary,  contours, m_max_object_num, CV_RGB(255,255,255), 1, 1, BOLT_Param[Cam_num].Object_hierarchy);
								//imwrite("01.bmp",Out_binary);
								RotatedRect Incircle_Info = fitEllipse(Mat(contours[m_max_object_num]));
								Rect m_max_rect = boundingRect( Mat(contours[m_max_object_num]) );
								double minval =0, maxval=0;
								cv::Point minLoc, CenterLoc;
								maxval = max(Incircle_Info.size.width,Incircle_Info.size.height)/2;
								CenterLoc = Incircle_Info.center;
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								circle(Our_Circle_Img, Incircle_Info.center,t_max,CV_RGB(255,255,255),5);
								bitwise_and(Our_Circle_Img, Out_binary, Our_Circle_Img);
								findContours( Our_Circle_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								double t_min = 99999;cv::Point2f t_max_dist_point2(0,0);
								for (int k = 0; k < contours.size(); k++)
								{  
									minRect = minAreaRect(contours[k]);
									t_max_temp =  sqrt(((minRect.center.x - (double)t_max_dist_point.x)*(minRect.center.x - (double)t_max_dist_point.x) + (minRect.center.y - (double)t_max_dist_point.y)*(minRect.center.y - (double)t_max_dist_point.y)));
									if (t_min >= t_max_temp)
									{
										t_min = t_max_temp;
										t_max_dist_point2 = cv::Point2f((t_max/t_max_temp)*(minRect.center.x -Incircle_Info.center.x) + Incircle_Info.center.x, (t_max/t_max_temp)*(minRect.center.y -Incircle_Info.center.y) + Incircle_Info.center.y);
									}
								}

								Mat Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
								cv::Point2f t_max_dist_point_rotate(0,0);
								cv::Point2f t_max_dist_point_R(1.05f*(t_max_dist_point2.x-Incircle_Info.center.x),1.05f*(t_max_dist_point2.y-Incircle_Info.center.y));
								line( Line_Img, Incircle_Info.center, cv::Point2f(t_max_dist_point_R.x + Incircle_Info.center.x,t_max_dist_point_R.y + Incircle_Info.center.y) ,CV_RGB(255,255,355), 1, 1 );
								bitwise_and(Line_Img, Out_binary, Line_Img);
								vector<cv::Point2f> t_vec_point;bool t_loop_check = false;
								for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
								{
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
								
								for (int k=1;k<BOLT_Param[Cam_num].nCrossAngleNumber[s];k++)
								{
									Line_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
									t_max_dist_point_rotate.x = cos((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.x
										                      + sin((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;
									t_max_dist_point_rotate.y = -sin((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.x
										+ cos((double)k*2*CV_PI/(double)BOLT_Param[Cam_num].nCrossAngleNumber[s])*t_max_dist_point_R.y;

									line( Line_Img, Incircle_Info.center, cv::Point2f(t_max_dist_point_rotate.x + Incircle_Info.center.x,t_max_dist_point_rotate.y + Incircle_Info.center.y) ,CV_RGB(255,255,355), 1, 1 );
									bitwise_and(Line_Img, Out_binary, Line_Img);
									t_loop_check = false;
									for (int ii=m_max_rect.x;ii<m_max_rect.x+m_max_rect.width;ii++)
									{
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
											if (m_Text_View[Cam_num])
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, 1, hierarchy);
							}

							Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1);
							drawContours( Out_binary,  contours, m_max_object_num, CV_RGB(255,255,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
							//imwrite("01.bmp",Out_binary);
							
							dilate(Out_binary,Out_binary,element,Point(-1,-1),5);
							erode(Out_binary,Out_binary,element,Point(-1,-1),5);

							Mat Dist_Img, label;//, //Tmp_Img;
							distanceTransform(Out_binary, Dist_Img,label, CV_DIST_L2,3);
							double minval, maxval;
							cv::Point minLoc, CenterLoc;
							cv::minMaxLoc(Dist_Img, &minval, &maxval, &minLoc, &CenterLoc);
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					}
					//J_Fill_Hole(Out_binary);
					Point2f P_Center;float R_radius;double V_R = 0;
					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					for(int k = 0; k < contours.size(); k++)
					{
						RotatedRect minRect = minAreaRect(contours[k]);
						//double t_v = 100.0*min(minRect.size.width,minRect.size.height) / max(minRect.size.width,minRect.size.height);
						//msg.Format("%1.2f",t_v);
						R_radius = (minRect.size.width+minRect.size.height)/4;
						P_Center = minRect.center;
						//minEnclosingCircle(contours[k],P_Center,R_radius);
						V_R = (BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1])*(double)R_radius;
						if (V_R >= BOLT_Param[Cam_num].nDiameter_Min_Size[s] && V_R <= BOLT_Param[Cam_num].nDiameter_Max_Size[s])
						{
							dist_vec.push_back(V_R);
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								//circle(Dst_Img[Cam_num], Point2f(BOLT_Param[Cam_num].nRect[s].x+P_Center.x,BOLT_Param[Cam_num].nRect[s].y+P_Center.y),R_radius,CV_RGB(255,0,0),1);
								//msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
								//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, 0.4, CV_RGB(255,100,0), 1, 8);
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								drawContours(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
								circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f(P_Center.x,P_Center.y),R_radius,CV_RGB(255,0,0),1);
								msg.Format("%1.3f",dist_vec[dist_vec.size()-1]);
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+P_Center.x+5,BOLT_Param[Cam_num].nRect[s].y+P_Center.y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
							}
						}
					}
				} 
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::BRIGHTNESS_AREA_TB) // Brightness of Area
				{
					//J_Delete_Boundary(Out_binary,1);
					//J_Fill_Hole(Out_binary);

					if (BOLT_Param[Cam_num].nColorMethod[s] == 0)
					{//흑백 처리
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
					else if (BOLT_Param[Cam_num].nColorMethod[s] == 1)
					{//컬러 처리
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					}
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), 1, CV_AA, hierarchy);
						}
					}
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), 1, CV_AA, hierarchy);
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
						Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
						double R1_V = 0;double R1_CNT = 0;

						if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
						{
							ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
						}
						else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
						{
							ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}

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

						//AfxMessageBox("2");
						Mat Mask2 = Mat::zeros(Out_binary.size(), CV_8UC1);
						double R2_V = 0;double R2_CNT = 0;

						if (BOLT_Param[Cam_num].nCircle2Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle2Radius[s] >= 0)
						{
							ellipse(Mask2,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,0,360,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle2Thickness[s],0,0);
						}
						else if (BOLT_Param[Cam_num].nCircle2Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle2Radius[s] > 0)
						{
							ellipse(Mask2,P_Center,Size(BOLT_Param[Cam_num].nCircle2Radius[s],BOLT_Param[Cam_num].nCircle2Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}
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
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							msg.Format("Circle2(%1.2f)",R2_V);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 20), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
							add(Mask,Mask2,Mask);
							bitwise_and(Mask,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
							}
						}
					}
					else if (BOLT_Param[Cam_num].nColorMethod[s] == 0)
					{ // 원의 밝기
						//AfxMessageBox("1");
						Point2f P_Center;
						P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x - BOLT_Param[Cam_num].nRect[s].x;
						P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y - BOLT_Param[Cam_num].nRect[s].y;
						//P_Center.x = BOLT_Param[Cam_num].nRect[s].width/2;
						//P_Center.y = BOLT_Param[Cam_num].nRect[s].height/2;
						Mat Mask = Mat::zeros(Out_binary.size(), CV_8UC1);
						double R1_V = 0;double R1_CNT = 0;

						if (BOLT_Param[Cam_num].nCircle1Thickness[s] > 0 && BOLT_Param[Cam_num].nCircle1Radius[s] >= 0)
						{
							ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
						}
						else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
						{
							ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,0,360,Scalar(255,255,255),CV_FILLED,8,0);
						}

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
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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

						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						}

						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}

						Mat stats, centroids, label;  
						int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							stats, centroids, 8,CV_32S);
						int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
						if (numOfLables > 100)
						{
							numOfLables = 100;
						}
						if (numOfLables > 0)
						{
							for (int j = 1; j < numOfLables; j++) 
							{
								area = stats.at<int>(j, CC_STAT_AREA);
								t_w = stats.at<int>(j, CC_STAT_WIDTH);
								t_h = stats.at<int>(j, CC_STAT_HEIGHT);
								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
									{
										dist_vec.push_back((double)area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("BLOB(%d)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d",area);
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
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

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
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

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
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

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
									double Cx = centroids.at<double>(j, 0) + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
									double Cy = centroids.at<double>(j, 1) + (double)BOLT_Param[Cam_num].nRect[s].y;

									double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
									double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

									double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
									{
										dist_vec.push_back(t_D);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);

											msg.Format("Dist(%1.3f)",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",t_D);
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterDarkThres[s] > 0)
						{
							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);
							if (numOfLables > 100)
							{
								numOfLables = 100;
							}
							int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
							if (numOfLables > 0)
							{
								for (int j = 1; j < numOfLables; j++) 
								{
									area = stats.at<int>(j, CC_STAT_AREA);
									t_w = stats.at<int>(j, CC_STAT_WIDTH);
									t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											dist_vec.push_back((double)area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
										double Cx = centroids.at<double>(j, 0) + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										double Cy = centroids.at<double>(j, 1) + (double)BOLT_Param[Cam_num].nRect[s].y;

										double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
										double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										{
											dist_vec.push_back(t_D);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
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
								//if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterBrightThres[s] > 0)
						{
							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);
							if (numOfLables > 100)
							{
								numOfLables = 100;
							}
							int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
							if (numOfLables > 0)
							{
								for (int j = 1; j < numOfLables; j++) 
								{
									area = stats.at<int>(j, CC_STAT_AREA);
									t_w = stats.at<int>(j, CC_STAT_WIDTH);
									t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											dist_vec.push_back((double)area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												msg.Format("BLOB(%d)",area);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%d",area);
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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
											}
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
										double Cx = centroids.at<double>(j, 0) + (double)BOLT_Param[Cam_num].nRect[s].x; //중심좌표
										double Cy = centroids.at<double>(j, 1) + (double)BOLT_Param[Cam_num].nRect[s].y;

										double R0x = (double)(BOLT_Param[Cam_num].nRect[0].x + BOLT_Param[Cam_num].Object_Postion.x);
										double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										double t_D = sqrt((R0x - Cx)*(R0x - Cx)*BOLT_Param[Cam_num].nResolution[0]*BOLT_Param[Cam_num].nResolution[0] + (R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										{
											dist_vec.push_back(t_D);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
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
								//if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
								}
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
								for( int k = 0; k < contours.size(); k++ )
								{
									drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
									}
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,200,255), 1, CV_AA, hierarchy);
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
									}
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
									for( int k = 0; k < contours.size(); k++ )
									{
										drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
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
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 0, 1, hierarchy);
							}
						}
					}

					//P_Center.x = BOLT_Param[Cam_num].Offset_Object_Postion.x + BOLT_Param[Cam_num].Object_Postion.x + (BOLT_Param[Cam_num].nRect[0].x - BOLT_Param[Cam_num].nRect[s].x);
					//P_Center.y = BOLT_Param[Cam_num].Offset_Object_Postion.y + BOLT_Param[Cam_num].Object_Postion.y + (BOLT_Param[Cam_num].nRect[0].y - BOLT_Param[Cam_num].nRect[s].y);
					//P_Center.x = BOLT_Param[Cam_num].nRect[s].width/2;
					//P_Center.y = BOLT_Param[Cam_num].nRect[s].height/2;
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
						ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
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
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
						}
					}

					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(0,0,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(0,0,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
						}
					}

					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
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
										dist_vec.push_back((double)area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("%d",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("BLOB(%d)",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
											msg.Format("%d",area);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
										}
										t_cnt++;
									}
									//if (BOLT_Param[Cam_num].nNumConnectedCircleBLOB[s] <= t_Circle2_connect_cnt)
									//{
									//	dist_vec.push_back((double)area);
									//	if (m_Text_View[Cam_num] && !ROI_Mode)
									//	{
									//		int x = centroids.at<double>(j, 0); //중심좌표
									//		int y = centroids.at<double>(j, 1);

									//		msg.Format("%d",area);
									//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									//	}

									//	if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									//	{
									//		int x = centroids.at<double>(j, 0); //중심좌표
									//		int y = centroids.at<double>(j, 1);

									//		msg.Format("BLOB(%d)",area);
									//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
									//		msg.Format("%d",area);
									//		putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
									//	}
									//	t_cnt++;
									//}
								}
								else
								{
									dist_vec.push_back((double)area);
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("%d",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										int x = centroids.at<double>(j, 0); //중심좌표
										int y = centroids.at<double>(j, 1);

										msg.Format("BLOB(%d)",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
										msg.Format("%d",area);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
									}
									t_cnt++;
								}
							}
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
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), 0, 1, hierarchy);
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
						ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),0,BOLT_Param[Cam_num].nCircleStartAngle[s],BOLT_Param[Cam_num].nCircleEndAngle[s],Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
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
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
						}
					}

					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,2,CV_RGB(255,100,0),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s],CV_RGB(0,0,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]-BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle1Radius[s]+BOLT_Param[Cam_num].nCircle1Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s],CV_RGB(0,0,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]-BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						//circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), P_Center,BOLT_Param[Cam_num].nCircle2Radius[s]+BOLT_Param[Cam_num].nCircle2Thickness[s]/2,CV_RGB(0,255,255),1);
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
						}
					}

					if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
					{
						erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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

									//	if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						vector<Rect> boundRect( contours.size() );
						int m_max_object_num = -1;int m_max_object_value = 0;
						for( int k = 0; k < contours.size(); k++ )
						{  
							boundRect[k] = boundingRect(contours[k]);
							if (m_max_object_value <= boundRect[k].width*boundRect[k].height)
							{
								m_max_object_value = boundRect[k].width*boundRect[k].height;
								m_max_object_num = k;
							}
						}

						if (m_max_object_num > -1)
						{
							RotatedRect minRect = minAreaRect(contours[m_max_object_num]);
							double t_v = 100.0*min(minRect.size.width,minRect.size.height) / max(minRect.size.width,minRect.size.height);
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

							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_TB::PITCH_COIN_TB) // Pitch of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										dist_vec.push_back((Pitch_Info[i+1].CY - Pitch_Info[i].CY)*BOLT_Param[Cam_num].nResolution[1]);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											msg.Format("P%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
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
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										dist_vec.push_back((Pitch_Info[i+1].CX - Pitch_Info[i].CX)*BOLT_Param[Cam_num].nResolution[0]);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											msg.Format("P%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
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
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
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
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
										{
											dist_vec.push_back(mu[k].m00);
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
										{
											dist_vec.push_back((double)boundRect.width);
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
										{
											dist_vec.push_back((double)boundRect.height);
										}
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										msg.Format("S%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
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
									//if (boundRect.x > 1)
									{
										t_Point4D.IDX = (double)k;
										t_Point4D.CX = mu[k].m10/mu[k].m00;
										t_Point4D.CY = mu[k].m01/mu[k].m00;
										t_Point4D.AREA = mu[k].m00;
										if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
										{
											dist_vec.push_back(mu[k].m00);
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
										{
											dist_vec.push_back((double)boundRect.width);
										}
										else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
										{
											dist_vec.push_back((double)boundRect.height);
										}
										t_Point4D.ROI = boundRect;
										Pitch_Info.push_back(t_Point4D);
									}

									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										msg.Format("S%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
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
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
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
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),-90,0,t_angle,Scalar(255,255,255),BOLT_Param[Cam_num].nCircle1Thickness[s],0,0);
					}
					else if (BOLT_Param[Cam_num].nCircle1Thickness[s] == 0 && BOLT_Param[Cam_num].nCircle1Radius[s] > 0)
					{
						ellipse(Mask,P_Center,Size(BOLT_Param[Cam_num].nCircle1Radius[s],BOLT_Param[Cam_num].nCircle1Radius[s]),-90,0,t_angle,Scalar(255,255,255),CV_FILLED,8,0);
					}

					bitwise_and(Mask,Out_binary,Out_binary);

					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
							{
								int x = centroids.at<double>(j, 0); //중심좌표
								int y = centroids.at<double>(j, 1);

								msg.Format("%d(%d)",cnt, area);
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, fontScale, CV_RGB(255,100,0), 1, 8);
							}
						}

					}

					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,50,0), 0, 1, hierarchy);
						}
					}
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,50,0), 0, 1, hierarchy);
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
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours1, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
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
					if (m_max_object_num >= 0)
					{
						RotatedRect Outcircle_Info = fitEllipse(Mat(contours[m_max_object_num]));
						Mat Outcircle_Img = Mat::zeros(CP_Gray_Img.size(), CV_8UC1);
						ellipse(Outcircle_Img, Outcircle_Info, CV_RGB(255,255,255), CV_FILLED, 8);
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s && BOLT_Param[Cam_num].nOutput[s] != 2)
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
						bitwise_and(Outcircle_Img,Out_binary,Out_binary);
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
						}

						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						m_max_object_num = -1;m_max_object_value = 0;
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

						if (m_max_object_num >= 0)
						{
							Out_binary = Mat::zeros(CP_Gray_Img.size(), CV_8UC1);
							drawContours( Out_binary, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8);
							//imwrite("01.bmp",Out_binary);
							RotatedRect Incircle_Info = fitEllipse(Mat(contours[m_max_object_num]));
							Outcircle_Img = Mat::zeros(CP_Gray_Img.size(), CV_8UC1);
							ellipse(Outcircle_Img, Incircle_Info, CV_RGB(255,255,255), CV_FILLED, 8);
							bitwise_and(Outcircle_Img,Out_binary,Out_binary);
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							m_max_object_num = -1;m_max_object_value = 0;
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
							Incircle_Info = fitEllipse(Mat(contours[m_max_object_num]));
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

							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s && BOLT_Param[Cam_num].nOutput[s] == 3)
							{
								msg.Format("%1.3f",(Outcircle_Info.size.width + Outcircle_Info.size.height)*0.25*(BOLT_Param[Cam_num].nResolution[0]+BOLT_Param[Cam_num].nResolution[1]));
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + Outcircle_Info.center.x - 5,BOLT_Param[Cam_num].nRect[s].y + Outcircle_Info.center.y - 20), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s && BOLT_Param[Cam_num].nOutput[s] != 3)
							{
								msg.Format("No Inner Circle");
								putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width/2 - 5,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(0,150,255), 1, 8);
							}
						}
					}
					else
					{
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
			}
			else 
			{// SIDE /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::W_LENGTH_S) // Horizontal Length
				{
					for(int i=0;i<Out_binary.rows;i++)
					{
						int ii = i+(int)((double)Out_binary.cols*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
						for(int j=0;j<Out_binary.cols;j++)
						{
							if (Out_binary.at<uchar>(i,j) == 255 && start_p == -1)
							{
								start_p = j;
								break;
							}
						}
						if (ii >= 0 && ii < Out_binary.rows)
						{
							if (!check_angle_line_init)
							{
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
									//AfxMessageBox(msg);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),2);
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(0,i), Point(Out_binary.cols,ii),CV_RGB(255,255,0),2);
								}
								check_angle_line_init = true;
							}
							for(int j=Out_binary.cols-1;j>=0;j--)
							{
								ii = i+(int)((double)(Out_binary.cols-start_p-((Out_binary.cols-1)-j))*tan(BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
								if (ii >= 0 && ii < Out_binary.rows)
								{
									if (Out_binary.at<uchar>(ii,j) == 255 && end_p == -1)
									{
										end_p = j;
										break;
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					for(int j=0;j<Out_binary.cols;j++)
					{
						int jj = j+(int)((double)Out_binary.rows*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));

						for(int i=0;i<Out_binary.rows;i++)
						{
							if (Out_binary.at<uchar>(i,j) == 255 && start_p == -1)
							{
								start_p = i;
								break;
							}
						}
						if (jj >= 0 && jj < Out_binary.cols)
						{
							if (!check_angle_line_init)
							{
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
									//AfxMessageBox(msg);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),2);
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									//msg.Format("P(%d), A(%1.3f), T(%1.3f)",ii,ALG_BOLTPIN_Param.nCalAngle[s],tan(ALG_BOLTPIN_Param.nCalAngle[s]* PI / 180.0));
									//AfxMessageBox(msg);
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(j,0), Point(jj,Out_binary.rows),CV_RGB(255,255,0),2);
								}
								check_angle_line_init = true;
							}
							for(int i=Out_binary.rows-1;i>=0;i--)
							{
								jj = j+(int)((double)(Out_binary.rows-start_p-((Out_binary.rows-1)-i))*tan(-BOLT_Param[Cam_num].nCalAngle[s]* PI / 180.0));
								if (jj >= 0 && jj < Out_binary.cols)
								{
									if (Out_binary.at<uchar>(i,jj) == 255 && end_p == -1)
									{
										end_p = i;
										break;
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					Mat stats, centroids, label;  
					Point2f rect_points[2];
					int numOfLables = connectedComponentsWithStats(t_binary, label,   
						stats, centroids, 8,CV_32S);
					if (numOfLables > 1)
					{
						for (int j = 1; j < numOfLables; j++) 
						{
							double x = centroids.at<double>(j, 0); //중심좌표
							double y = centroids.at<double>(j, 1);
							rect_points[j-1] = Point2f(x,y);
						}
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							line( Dst_Img[Cam_num], Point2f(rect_points[0].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[0].y + BOLT_Param[Cam_num].nRect[s].y) , 
								Point2f(rect_points[1].x + BOLT_Param[Cam_num].nRect[s].x,rect_points[1].y + BOLT_Param[Cam_num].nRect[s].y),CV_RGB(255,255,0), 1, 8 );
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[0].x,rect_points[0].y),2,CV_RGB(255,0,0),1);
							circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point(rect_points[1].x,rect_points[1].y),2,CV_RGB(255,0,0),1);
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					//	if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					for( int k = 0; k < contours.size(); k++ )
					{  
						tt_TROI = boundingRect(contours[k]);
						if (m_max_object_value<= tt_TROI.height*tt_TROI.width)
						{
							m_max_object_value = tt_TROI.height*tt_TROI.width;
							m_max_object_num = k;
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
					for( int k = 0; k < contours.size(); k++ )
					{  
						tt_TROI = boundingRect(contours[k]);
						if (m_max_object_value<= tt_TROI.height*tt_TROI.width)
						{
							m_max_object_value = tt_TROI.height*tt_TROI.width;
							m_max_object_num = k;
						}
					}
					Mat t_End_Nail = Mat::zeros(Out_binary.size(), CV_8UC1);
					if (m_max_object_num > -1)
					{
						tt_TROI = boundingRect(contours[m_max_object_num]);
						tt_TROI.x = tt_TROI.x;tt_TROI.width = tt_TROI.width;
						t_End_Nail(tt_TROI) += 255;
						subtract(t_End_Nail, Out_binary,t_End_Nail);
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										dist_vec.push_back((Pitch_Info[i+1].CY - Pitch_Info[i].CY)*BOLT_Param[Cam_num].nResolution[1]);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											msg.Format("P%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
										}
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{

						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										dist_vec.push_back((Pitch_Info[i+1].CX - Pitch_Info[i].CX)*BOLT_Param[Cam_num].nResolution[0]);
										//msg.Format("%1.2f - %1.2f = %1.2f",Pitch_Info[i+1].CX, Pitch_Info[i].CX, dist_vec[dist_vec.size()-1]);
										//AfxMessageBox(msg);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											//boundRect = Pitch_Info[i].ROI;
											//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
											//rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(0,0,255),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
											msg.Format("P%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
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
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
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
										//if (boundRect.x > 1)
										{
											t_Point4D.IDX = (double)k;
											t_Point4D.CX = mu[k].m10/mu[k].m00;
											t_Point4D.CY = mu[k].m01/mu[k].m00;
											t_Point4D.AREA = mu[k].m00;
											if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
											{
												dist_vec.push_back(mu[k].m00);
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
											{
												dist_vec.push_back((double)boundRect.width);
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
											{
												dist_vec.push_back((double)boundRect.height);
											}
											t_Point4D.ROI = boundRect;
											Pitch_Info.push_back(t_Point4D);
										}

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
											msg.Format("S%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
												if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
										}
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
										}
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
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
										//if (boundRect.x > 1)
										{
											t_Point4D.IDX = (double)k;
											t_Point4D.CX = mu[k].m10/mu[k].m00;
											t_Point4D.CY = mu[k].m01/mu[k].m00;
											t_Point4D.AREA = mu[k].m00;
											if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 0)
											{
												dist_vec.push_back(mu[k].m00);
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 1)
											{
												dist_vec.push_back((double)boundRect.width);
											}
											else if (BOLT_Param[Cam_num].nThreadSizeMethod[s] == 2)
											{
												dist_vec.push_back((double)boundRect.height);
											}
											t_Point4D.ROI = boundRect;
											Pitch_Info.push_back(t_Point4D);
										}

										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
											msg.Format("S%2d(%1.2f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
												if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
										}
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
										for( int k = 0; k < contours.size(); k++ )
										{
											drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
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
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
							vector<double> vec_angle;
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size();i++)
								{
									if (i < Pitch_Info.size()-1 && Pitch_Info[i].CY > 0)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									dist_vec.push_back(vec_angle[i]+vec_angle[i+1]);
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									dist_vec.push_back(vec_angle[i]+vec_angle[i+1]);
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::LEADANGLE_HARF_COIN_S) // Lead Angle of Screw Thread
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),Out_binary.rows/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									}
								}
							}
							std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
							vector<double> vec_angle;
							if (Pitch_Info.size() > 0)
							{
								for (int i = 0;i < Pitch_Info.size();i++)
								{
									if (i < Pitch_Info.size()-1 && Pitch_Info[i].CY > 0)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
									}
								}
							}
							if (vec_angle.size() > 0)
							{
								for (int i = 0;i < vec_angle.size()-1;i++)
								{
									dist_vec.push_back(vec_angle[i]);
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
										putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10*(dist_vec.size()+1)), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
									}
								}
							}
						}
					}
					else if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::BELT_TYPE)
					{
						Mat Morph_Out_binary;
						erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
						dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),Out_binary.cols/4);
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),Point(Pitch_Info[i+1].CX,Pitch_Info[i+1].CY),CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
										}
									}
								}
							}
							if (vec_angle.size() > 0)
							{
								for (int i = 0;i < vec_angle.size()-1;i++)
								{
									dist_vec.push_back(vec_angle[i]);
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
									{
										msg.Format("A%2d(%1.3f)",dist_vec.size(),dist_vec[dist_vec.size()-1]);
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									msg.Format("W(%1.3f)",dist_vec[0]);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
								{
									rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), boundRect,CV_RGB(255,150,0),1);
									msg.Format("W(%1.3f)",dist_vec[0]);
									putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 10), FONT_HERSHEY_SIMPLEX, 0.28, CV_RGB(255,100,0), 1, 8);
								}
							}
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::BODY_BENDING_S) // BODY_BENDING
				{
					if (BOLT_Param[Cam_num].nTableType != GUIDE_METHOD::BELT_TYPE)
					{
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
									if (m_Text_View[Cam_num] && !ROI_Mode)
									{
										circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), Point2f((t_center/t_cnt),j),0,CV_RGB(255,255,0),1);
									}
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								if (m_Text_View[Cam_num] && !ROI_Mode)
								{
									line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point2f(t_x0,t_y0),Point2f(t_x1,t_y1),CV_RGB(255,0,0),1);
								}
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					if (m_Text_View[Cam_num] && !ROI_Mode)
					{
						//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						//for( int k = 0; k < contours.size(); k++ )
						//{
						//	drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), CV_FILLED, 8, hierarchy);
						//}
					}
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
						for( int k = 0; k < contours.size(); k++ )
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,100,255), CV_FILLED, 8, hierarchy);
						}
					}
				}
				else if (BOLT_Param[Cam_num].nMethod_Direc[s] == ALGORITHM_S::AREA_BLOB_S) // BLOB Size
				{
					if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 0 || BOLT_Param[Cam_num].nDirecFilterUsage[s] == 2)
					{
						if (BOLT_Param[Cam_num].nDirecFilterUsage[s] == 2)
						{
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						if (BOLT_Param[Cam_num].nROI0_FilterSize[s] > 0)
						{
							erode(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
							dilate(Out_binary,Out_binary,element,Point(-1,-1), BOLT_Param[Cam_num].nROI0_FilterSize[s]);
						}
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						Mat stats, centroids, label;  
						int numOfLables = connectedComponentsWithStats(Out_binary, label,   
							stats, centroids, 8,CV_32S);
						int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
						if (numOfLables > 0)
						{
							for (int j = 1; j < numOfLables; j++) 
							{
								area = stats.at<int>(j, CC_STAT_AREA);
								t_w = stats.at<int>(j, CC_STAT_WIDTH);
								t_h = stats.at<int>(j, CC_STAT_HEIGHT);
								if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
									{
										dist_vec.push_back((double)area);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

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
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

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
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
									}
								} 
								else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
								{
									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
									{
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

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
									double Cx = (double)(BOLT_Param[Cam_num].Object_Postion.x + BOLT_Param[Cam_num].nRect[0].width/2); //중심좌표
									double Cy = (double)BOLT_Param[Cam_num].nRect[s].y + (double)stats.at<int>(j, CC_STAT_TOP);//centroids.at<double>(j, 1);

									double R0x = (double)(BOLT_Param[Cam_num].Object_Postion.x + BOLT_Param[Cam_num].nRect[0].width/2);
									double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

									double t_D = sqrt((R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

									if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
									{
										dist_vec.push_back(t_D);
										if (m_Text_View[Cam_num] && !ROI_Mode)
										{
										}
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
										{
											int x = centroids.at<double>(j, 0); //중심좌표
											int y = centroids.at<double>(j, 1);

											line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
											circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,255,0),1);
											circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);

											msg.Format("Dist(%1.3f)",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
											msg.Format("%1.3f",t_D);
											putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
										}
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
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterDarkThres[s] > 0)
						{
							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);
							int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
							if (numOfLables > 0)
							{
								for (int j = 1; j < numOfLables; j++) 
								{
									area = stats.at<int>(j, CC_STAT_AREA);
									t_w = stats.at<int>(j, CC_STAT_WIDTH);
									t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											dist_vec.push_back((double)area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
										double Cx = (double)(BOLT_Param[Cam_num].Object_Postion.x + BOLT_Param[Cam_num].nRect[0].width/2); //중심좌표
										double Cy = (double)BOLT_Param[Cam_num].nRect[s].y +  (double)stats.at<int>(j, CC_STAT_TOP);//centroids.at<double>(j, 1);

										double R0x = (double)(BOLT_Param[Cam_num].Object_Postion.x + BOLT_Param[Cam_num].nRect[0].width/2);
										double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										double t_D = sqrt((R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										{
											dist_vec.push_back(t_D);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
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
								//if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						if (m_Text_View[Cam_num] && !ROI_Mode)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,255,0), CV_FILLED, 8, hierarchy);
							}
						}
						if (BOLT_Param[Cam_num].nDirecFilterBrightThres[s] > 0)
						{
							Mat stats, centroids, label;  
							int numOfLables = connectedComponentsWithStats(Out_binary, label,   
								stats, centroids, 8,CV_32S);

							int t_cnt = 0;int area = 0;int t_w = 0;int t_h = 0;
							if (numOfLables > 0)
							{
								for (int j = 1; j < numOfLables; j++) 
								{
									area = stats.at<int>(j, CC_STAT_AREA);
									t_w = stats.at<int>(j, CC_STAT_WIDTH);
									t_h = stats.at<int>(j, CC_STAT_HEIGHT);
									if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 0)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											dist_vec.push_back((double)area);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												msg.Format("H(%1.3f)",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",(double)t_h*BOLT_Param[Cam_num].nResolution[1]);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
										}
									} 
									else if (BOLT_Param[Cam_num].nCirclePositionMethod[s] == 3)
									{
										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= (double)area && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= (double)area)
										{
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

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
										double Cx = (double)(BOLT_Param[Cam_num].Object_Postion.x + BOLT_Param[Cam_num].nRect[0].width/2); //중심좌표
										double Cy = (double)BOLT_Param[Cam_num].nRect[s].y +  (double)stats.at<int>(j, CC_STAT_TOP);//centroids.at<double>(j, 1);

										double R0x = (double)(BOLT_Param[Cam_num].Object_Postion.x + BOLT_Param[Cam_num].nRect[0].width/2);
										double R0y = (double)(BOLT_Param[Cam_num].nRect[0].y + BOLT_Param[Cam_num].Object_Postion.y);

										double t_D = sqrt((R0y - Cy)*(R0y - Cy)*BOLT_Param[Cam_num].nResolution[1]*BOLT_Param[Cam_num].nResolution[1]);

										if (BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] <= t_D && BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s] >= t_D)
										{
											dist_vec.push_back(t_D);
											if (m_Text_View[Cam_num] && !ROI_Mode)
											{
											}
											if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
											{
												int x = centroids.at<double>(j, 0); //중심좌표
												int y = centroids.at<double>(j, 1);

												line(Dst_Img[Cam_num],Point(Cx,Cy),Point(R0x,R0y),CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),2,CV_RGB(0,255,0),1);
												circle(Dst_Img[Cam_num],Point(Cx,Cy),1,CV_RGB(255,255,0),1);
												circle(Dst_Img[Cam_num],Point(R0x,R0y),1,CV_RGB(255,255,0),1);

												msg.Format("Dist(%1.3f)",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x + BOLT_Param[Cam_num].nRect[s].width+3,BOLT_Param[Cam_num].nRect[s].y + 12*t_cnt), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(255,100,0), 1, 8);
												msg.Format("%1.3f",t_D);
												putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y), fontFace, 0.3, CV_RGB(255,100,0), 1, 8);
											}
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
								//if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
					if (BOLT_Param[Cam_num].nThickness[0] > 0)
					{
						int t_filter_cnt = (int)(1*BOLT_Param[Cam_num].nThickness[0]/BOLT_Param[Cam_num].nResolution[0]);
						erode(Out_binary,Out_binary,element_v,Point(-1,-1),t_filter_cnt);
						dilate(Out_binary,Out_binary,element_v,Point(-1,-1),t_filter_cnt);
					}

					Rect tt_TROI;int m_max_object_value = 0;int m_max_object_num = -1;
					findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
					for( int k = 0; k < contours.size(); k++ )
					{  
						tt_TROI = boundingRect(contours[k]);
						if (m_max_object_value<= tt_TROI.height*tt_TROI.width)
						{
							m_max_object_value = tt_TROI.height*tt_TROI.width;
							m_max_object_num = k;
						}
					}
					if (m_max_object_num >= 0)
					{
						tt_TROI = boundingRect(contours[m_max_object_num]);
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						Rect t_mask(tt_TROI.x,tt_TROI.y
							,tt_TROI.width,tt_TROI.height - BOLT_Param[Cam_num].nHeightforShape[s]-1);
						t_End_Nail(t_mask) -= 255;
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							line(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Point(t_mask.x,t_mask.y),Point(t_mask.x+t_mask.width,t_mask.y),CV_RGB(255,255,0),1);
						}
						//msg.Format("Save\\Debugging\\CAM0_01.bmp",s);
						//imwrite(msg.GetBuffer(),t_End_Nail);

					}



					//dilate(t_End_Nail,t_End_Nail,element_v,Point(-1,-1),1);
					//erode(t_End_Nail,t_End_Nail,element_v,Point(-1,-1),1);

					if (m_max_object_num > -1)
					{
						if (m_Text_View[Cam_num])
						{
							findContours( t_End_Nail.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							m_max_object_num = -1;m_max_object_value = 0;Rect tt_TROI;								

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
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
										if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
								if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
						for(size_t k=0; k<contours1.size(); k++)
						{	
							convexHull( Mat(contours1[k]), hull1[k], false ); 
							//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
							drawContours( Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
							if (m_Text_View[Cam_num] && !ROI_Mode)
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours1, k, CV_RGB(255,200,0), 1, CV_AA, hierarchy);
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
									if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
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
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
							}
						}
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							findContours( Convex_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
							for( int k = 0; k < contours.size(); k++ )
							{
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), 1, CV_AA, hierarchy);
							}
						}
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
			if (v_result != 0)
			{
				v_result += BOLT_Param[Cam_num].Offset[s];
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			// [끝] 측정된 값 각 방법에따라 계산
			///////////////////////////////////////////////////////////////////////////////////////////////////////

			if (m_License_Check == -1)
			{
				Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
			} else
			{
				Result_Info[Cam_num].Format("%sC%d:%02d_%1.3f_",Result_Info[Cam_num],Cam_num,s,v_result);
			}

			if (m_Text_View[Cam_num] && s >= 1 && !ROI_Mode)
			{
				rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
				msg.Format("ROI#%d(%1.3f)", s,v_result);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+(2*Gray_Img[Cam_num].rows/480),BOLT_Param[Cam_num].nRect[s].y + 11 + 6*Gray_Img[Cam_num].rows/480), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1.6, 8);
			}
			if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
			{
				rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
				msg.Format("ROI#%d(%1.3f)", s,v_result);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+(2*Gray_Img[Cam_num].rows/480),BOLT_Param[Cam_num].nRect[s].y + 11+ 6*Gray_Img[Cam_num].rows/480), FONT_HERSHEY_SIMPLEX, fontScale*((double)Gray_Img[Cam_num].rows/480), CV_RGB(255,100,0), 1.6, 8);
			}

			BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
		}
	} else
	{
		for (int s=1;s<41;s++)
		{
			Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
		}
	}
	return true;
}

bool CImPro_Library::ROI_Object_Find(int Cam_num)
{

	//Template_Img[Cam_num] = Gray_Img[Cam_num].clone();
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
		return false;
	}

	if (BOLT_Param[Cam_num].nTableType == GUIDE_METHOD::ROI_TYPE) //ROI 기준 측정
	{
		BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
		BOLT_Param[Cam_num].Object_Postion = Point(BOLT_Param[Cam_num].nRect[s].x,BOLT_Param[Cam_num].nRect[s].y);
		return true;
	}

	if (!Gray_Img[Cam_num].empty())
	{
		if (BOLT_Param[Cam_num].nMethod_Thres[0] == THRES_METHOD::FIRSTROI) // ROI#01 모델사용
		{
			bool t_license = Easy::CheckLicense(LicenseFeatures::EasyFind);
			if (t_license == false)
			{// 유레시스 없으면
				Template_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();
				Point2f R_Center = J_Model_Find(Cam_num);
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
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
					{
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
						circle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
					}
				}
			}
			else
			{// 유레시스 있으면
				AfxMessageBox("1");
				Template_Img[Cam_num] = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();
				E_Cam_Img[Cam_num].SetImagePtr(Template_Img[Cam_num].cols,Template_Img[Cam_num].rows,Template_Img[Cam_num].data);
				m_Cam_Find[Cam_num].SetMinScore(0.01);
				m_Cam_Find[Cam_num].SetInterpolate(true);
				m_Cam_Find[Cam_num].SetAngleTolerance(45.00f);
				m_Cam_Find[Cam_num].SetContrastMode(EFindContrastMode_Normal);
				m_Cam_Find[Cam_num].Learn(&E_Cam_Img[Cam_num]);
				Point2f R_Center((double)BOLT_Param[Cam_num].nRect[s].x+(double)BOLT_Param[Cam_num].nRect[s].width/2,(double)BOLT_Param[Cam_num].nRect[s].y+(double)BOLT_Param[Cam_num].nRect[s].height/2);
				// 머리를 못 찾을 경우 Error 처리함.
				if(R_Center.x == -1 || R_Center.y == -1)
				{
					BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
					BOLT_Param[Cam_num].Object_Postion = R_Center;
					rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
					msg.Format("No Object!");
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
					return true;
				}
				else
				{
					AfxMessageBox("2");
					BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
					BOLT_Param[Cam_num].Object_Postion = R_Center;
					if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
					{
						circle(Dst_Img[Cam_num],BOLT_Param[Cam_num].Object_Postion,2,CV_RGB(255,0,0),2);
						circle(Dst_Img[Cam_num],BOLT_Param[Cam_num].Object_Postion,1,CV_RGB(0,0,255),1);
					}
				}
			}
			//AfxMessageBox("1");
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
			CP_Gray_Img = Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).clone();
			if (m_Text_View[Cam_num] && BOLT_Param[Cam_num].nCamPosition == 0)
			{
				rectangle(Dst_Img[Cam_num],BOLT_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
				msg.Format("Obj. ROI");
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+1,BOLT_Param[Cam_num].nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(0,0,255), 1, 8);
			}

			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_Base_Obj_Gray_Img.bmp",Cam_num);
				imwrite(msg.GetBuffer(),CP_Gray_Img);		
			}


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

			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\CAM%d_Base_Obj_Binary_Img.bmp",Cam_num);
				imwrite(msg.GetBuffer(),Out_binary);
			}


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
				//J_Delete_Boundary(Out_binary,1);
				Mat stats, centroids, label;  
				int numOfLables = connectedComponentsWithStats(Out_binary, label,   
					stats, centroids, 8,CV_32S);
				int m_min_object_num = -1; int m_min_object_value = 999999;
				int m_max_object_num = -1; int m_max_object_value = 0;
				int area, left, top, width, height = { 0, };
				for (int j = 1; j < numOfLables; j++)
				{
					area = stats.at<int>(j, CC_STAT_AREA);

					if ((double)area >= BOLT_Param[Cam_num].nROI0_BLOB_Min_Size[s] && (double)area <= BOLT_Param[Cam_num].nROI0_BLOB_Max_Size[s])
					{
						left = BOLT_Param[Cam_num].nRect[s].x + stats.at<int>(j, CC_STAT_LEFT);
						top = BOLT_Param[Cam_num].nRect[s].y + stats.at<int>(j, CC_STAT_TOP);
						width = stats.at<int>(j, CC_STAT_WIDTH);
						height = stats.at<int>(j, CC_STAT_HEIGHT);
						if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
						{
							rectangle(Dst_Img[Cam_num], Point(left,top), Point(left+width,top+height),  
								CV_RGB(0,255,0),1 );  

							int x = centroids.at<double>(j, 0); //중심좌표
							int y = centroids.at<double>(j, 1);

							msg.Format("%d",area);
							putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+x-5,BOLT_Param[Cam_num].nRect[s].y+y-10), fontFace, fontScale, CV_RGB(0,100,255), 1, 8);
						}

						if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_END)
						{
							if (left <= m_min_object_value)
							{
								m_min_object_value = left;
								m_min_object_num = j;
							}
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_END)
						{
							if (left + width >= m_max_object_value)
							{
								m_max_object_value = left + width;
								m_max_object_num = j;
							}
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_TOP)
						{
							double t_dist = sqrt((double)left*(double)left + (double)top*(double)top);
							if ((int)t_dist <= m_min_object_value)
							{
								m_min_object_value = (int)t_dist;
								m_min_object_num = j;
							}
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::LEFT_BOTTOM)
						{
							double t_dist = sqrt((double)left*(double)left + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
							if ((int)t_dist <= m_min_object_value)
							{
								m_min_object_value = (int)t_dist;
								m_min_object_num = j;
							}
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_TOP)
						{
							double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top)*(double)(top));
							if ((int)t_dist <= m_min_object_value)
							{
								m_min_object_value = (int)t_dist;
								m_min_object_num = j;
							}
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::RIGHT_BOTTOM)
						{
							double t_dist = sqrt((double)(left + width - Out_binary.cols)*(double)(left + width - Out_binary.cols) + (double)(top + height - Out_binary.rows)*(double)(top + height - Out_binary.rows));
							if ((int)t_dist <= m_min_object_value)
							{
								m_min_object_value = (int)t_dist;
								m_min_object_num = j;
							}
						}
						else if (BOLT_Param[Cam_num].nMethod_Direc[s] == POSITION_METHOD::CENTER)
						{
							if (area >= m_max_object_value)
							{
								m_max_object_value = area;
								m_max_object_num = j;
							}
						}
					}
				}


				// 머리를 못 찾을 경우 Error 처리함.
				if(m_min_object_num == -1 && m_max_object_num == -1)
				{
					Result_Info[Cam_num] = "";
					for (int s=1;s<41;s++)
					{
						Result_Info[Cam_num].Format("%sC%d:%02d_-1_",Result_Info[Cam_num],Cam_num,s);
					}
					rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
					msg.Format("No Object!");
					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);
					BOLT_Param[Cam_num].nRect[s] = BOLT_Param[Cam_num].nRect_Backup[s];
					return true;
				}

				if (m_min_object_num > -1)
				{
					BOLT_Param[Cam_num].Offset_Object_Postion = Point(0, 0);
					left = BOLT_Param[Cam_num].nRect[s].x + stats.at<int>(m_min_object_num, CC_STAT_LEFT);
					top = BOLT_Param[Cam_num].nRect[s].y + stats.at<int>(m_min_object_num, CC_STAT_TOP);
					width = stats.at<int>(m_min_object_num, CC_STAT_WIDTH);
					height = stats.at<int>(m_min_object_num, CC_STAT_HEIGHT);

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

					BOLT_Param[Cam_num].Object_Postion = Point(x, y);
					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
							CV_RGB(255, 0, 0), 1);

						for (int y = 0; y < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).rows; ++y) {

							int *tlabel = label.ptr<int>(y);
							Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(y);
							for (int x = 0; x < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).cols; ++x) {
								if (tlabel[x] == m_max_object_num)
								{
									pixel[x][2] = 0;  
									pixel[x][1] = 255;
									pixel[x][0] = 0;
								}
								else if (tlabel[x] != m_max_object_num && tlabel[x] > 0)
								{
									//pixel[x][2] = 0;  
									//pixel[x][1] = 0;
									pixel[x][0] = 0;
								}
							}
						}

						circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 2, CV_RGB(255, 0, 0), 2);
						circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 1, CV_RGB(0, 0, 255), 1);
					}
				}

				if (m_max_object_num > -1)
				{
					BOLT_Param[Cam_num].Offset_Object_Postion = Point(0, 0);//기준점 초기화
					left = BOLT_Param[Cam_num].nRect[s].x + stats.at<int>(m_max_object_num, CC_STAT_LEFT);
					top = BOLT_Param[Cam_num].nRect[s].y + stats.at<int>(m_max_object_num, CC_STAT_TOP);
					width = stats.at<int>(m_max_object_num, CC_STAT_WIDTH);
					height = stats.at<int>(m_max_object_num, CC_STAT_HEIGHT);

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

					BOLT_Param[Cam_num].Object_Postion = Point(x, y);//기준점 등록

					if (ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == s)
					{
						rectangle(Dst_Img[Cam_num], Point(left, top), Point(left + width, top + height),
							CV_RGB(255, 0, 0), 1);

						for (int y = 0; y < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).rows; ++y) {

							int *tlabel = label.ptr<int>(y);
							Vec3b* pixel = Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).ptr<Vec3b>(y);
							for (int x = 0; x < Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]).cols; ++x) {
								if (tlabel[x] == m_max_object_num)
								{
									pixel[x][2] = 0;  
									pixel[x][1] = 255;
									pixel[x][0] = 0;
								}
								else if (tlabel[x] != m_max_object_num && tlabel[x] > 0)
								{
									//pixel[x][2] = 0;  
									//pixel[x][1] = 0;
									pixel[x][0] = 0;
								}
							}
						}

						circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 2, CV_RGB(255, 0, 0), 2);
						circle(Dst_Img[Cam_num], BOLT_Param[Cam_num].Object_Postion, 1, CV_RGB(0, 0, 255), 1);
					}
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
					// 사이드 바닦 찾기
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

					if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
					{
						line(Out_binary,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
					}
					//if (Result_Debugging) // 오링 사이드 바닦 라인 이미지
					//{
					//	msg.Format("Save\\Debugging\\Cam%d_%2d_Line_Thres_ROI_Gray_Img.bmp",Cam_num, s);
					//	imwrite(msg.GetBuffer(),Out_binary);		
					//}

					erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
					dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);
					// 머리부 찾기
					//erode(Out_binary,Out_binary,element,Point(-1,-1),5);
					//dilate(Out_binary,Out_binary,element,Point(-1,-1),5);
					//J_Delete_Boundary(Out_binary,1);

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
						BOLT_Param[Cam_num].Object_Postion = Point(boundRect[m_max_object_num].x,boundRect[m_max_object_num].y);
						if (m_Text_View[Cam_num]) // ROI 영역 표시
						{
							drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
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
					int m_max_object_num = -1;int m_max_object_value = 0;

					Mat Target_Thres_ROI_Gray_Img;
					J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
					J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
					J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);

					dilate(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),10);
					erode(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),10);

					Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();

					if (BOLT_Param[Cam_num].nThickness[0] > 0)
					{
						int t_filter_cnt = (int)(0.75*BOLT_Param[Cam_num].nThickness[0]/BOLT_Param[Cam_num].nResolution[0]);
						if (t_filter_cnt > BOLT_Param[Cam_num].nRect[s].width/1.5)
						{
							t_filter_cnt = BOLT_Param[Cam_num].nRect[s].width/1.5;
						}
						erode(Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),t_filter_cnt);
						dilate(Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),t_filter_cnt);
					}

					dilate(Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,element,Point(-1,-1),1);
					subtract(Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img,Rotate_Target_Thres_ROI_Gray_Img);
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
						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),Rect(10,10,BOLT_Param[Cam_num].nRect[s].width-20,BOLT_Param[Cam_num].nRect[s].height-20),Scalar(0,0,255),2);
						msg.Format("No head object!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(BOLT_Param[Cam_num].nRect[s].x+BOLT_Param[Cam_num].nRect[s].width/3,BOLT_Param[Cam_num].nRect[s].y + BOLT_Param[Cam_num].nRect[s].height/2 -11), FONT_HERSHEY_SIMPLEX, fontScale*2, CV_RGB(255,100,0), 1, 8);

						rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,100,0),2);
						msg.Format("Increase head width parameter!");
						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,Dst_Img[Cam_num].rows-20), FONT_HERSHEY_SIMPLEX, fontScale, CV_RGB(255,100,0), 2, 8);
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

					if (m_max_object_num >= 0 && m_2nd_max_object_num >= 0)
					{
						BOLT_Param[Cam_num].Offset_Object_Postion = Point(0,0);
						BOLT_Param[Cam_num].Object_Postion = Point(t_Head_rect.x,t_Head_rect.y);
						if (m_Text_View[Cam_num]) // ROI 영역 표시
						{
							rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,100,0),1);
							vector<vector<Point> > total_contours(1);
							for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
							{
								total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
							}
							for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num].size();j++)
							{
								total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_2nd_max_object_num][j]);
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
							{

								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),  BOLT_Param[Cam_num].Object_contours, m_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);
								drawContours( Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),  BOLT_Param[Cam_num].Object_contours, m_2nd_max_object_num, CV_RGB(0,0,255), CV_FILLED, 8, BOLT_Param[Cam_num].Object_hierarchy);

								tt_rect = boundingRect( Mat(total_contours[0]) );
								Rect ttt_rect(tt_rect.x-3,tt_rect.y-3,tt_rect.width+6,tt_rect.height+6);
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),ttt_rect,CV_RGB(0,255,100),1);
								t_Side_rect.x-=1;t_Side_rect.y-=1;t_Side_rect.width+=2;t_Side_rect.height+=2;
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
								t_Head_rect.x-=1;t_Head_rect.y-=1;t_Head_rect.width+=2;t_Head_rect.height+=2;
								rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
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
					int m_max_object_num = -1;int m_max_object_value = 0;


					Mat Target_Thres_ROI_Gray_Img;
					J_Rotate_PRE(Out_binary,t_angle,Target_Thres_ROI_Gray_Img,1);
					J_Rotate_PRE(CP_Gray_Img,t_angle,BOLT_Param[Cam_num].Gray_Obj_Img,1);
					J_Rotate_PRE(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_angle,Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),1);

					Mat Rotate_Target_Thres_ROI_Gray_Img = Target_Thres_ROI_Gray_Img.clone();

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

					m_max_object_num = -1;
					m_max_object_value = 0;
					Rect tt_rect;Rect t_Side_rect;
					for( int k = 0; k < BOLT_Param[Cam_num].Object_contours.size(); k++ )
					{  
						tt_rect = boundingRect( Mat(BOLT_Param[Cam_num].Object_contours[k]) );
						if (m_max_object_value<= tt_rect.width*tt_rect.height 
							&& tt_rect.x >= 1 && tt_rect.y >= 0 && tt_rect.x + tt_rect.width < Out_binary.cols-2 && tt_rect.y + tt_rect.height < Out_binary.rows-2)
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
						BOLT_Param[Cam_num].Object_Postion = Point(t_Side_rect.x,t_Side_rect.y);
						if (m_Text_View[Cam_num]) // ROI 영역 표시
						{
							rectangle(Dst_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]),t_Sub_ROI,CV_RGB(255,100,0),1);
							vector<vector<Point> > total_contours(1);
							for (int j=0;j<BOLT_Param[Cam_num].Object_contours[m_max_object_num].size();j++)
							{
								total_contours[0].push_back(BOLT_Param[Cam_num].Object_contours[m_max_object_num][j]);
							}
							if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && ROI_Num == 0)
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
		vector<vector<Point>> contours;	// 검사대상 Contour 정보
		vector<Vec4i> hierarchy;			// 검사대상 Hierarchy 정보
		findContours(Out_binary, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
		Rect boundRect;
		int m_max_object_num = -1;int m_max_object_value = 0;
		for( int k = 0; k < contours.size(); k++ )
		{  
			boundRect = boundingRect( Mat(contours[k]) );
			if (m_max_object_value<= boundRect.width*boundRect.height)
			{
				m_max_object_value = boundRect.width*boundRect.height;
				m_max_object_num = k;
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
			for (int j=boundRect.y;j < boundRect.y+boundRect.height;j++)
			{
				for (int i = boundRect.x; i < boundRect.x+boundRect.width;i++)
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
			for (int j=boundRect.y;j < boundRect.y+boundRect.height;j++)
			{
				for (int i = boundRect.x+boundRect.height-1; i >= boundRect.x;i--)
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

				J_Rotate_PRE(Gray_Img[Cam_num],-t_angle,Gray_Img[Cam_num],1);
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

				J_Rotate_PRE(Gray_Img[Cam_num],-t_angle,Gray_Img[Cam_num],1);
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

				J_Rotate_PRE(Gray_Img[Cam_num],-t_angle,Gray_Img[Cam_num],1);
				//imwrite("02.bmp",Gray_Img[Cam_num](BOLT_Param[Cam_num].nRect[s]));
			}
		}
	}
}


// 스기야마 0번 카메라 알고리즘
bool sort_descending(double A,double B)
{
	return A < B;
}

bool sort_ascending(double A,double B)
{
	return A < B;
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
		//if (m_Text_View[Cam_num] && ROI_Mode && ROI_CAM_Num == Cam_num && BOLT_Param[Cam_num].nCamPosition == 0)
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

		//imwrite("01.bmp",ROI_Original_Threshold_Img);
		// 외경계산
		dilate(ROI_Original_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),20); // 슬리트 채움
		erode(Out_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),20);
		Inner_Threshold_Img = Out_Threshold_Img.clone();
		Mat Slit_Img = Out_Threshold_Img.clone();
		J_Fill_Hole(Out_Threshold_Img);
		erode(Out_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),50); // fitting때 단자 간섭 제거위함.
		dilate(Out_Threshold_Img,Out_Threshold_Img,element,Point(-1,-1),50);
		J_Fitting_Ellipse(Out_Threshold_Img, Out_Circle_Info);

		// 내경계산
		subtract(Out_Threshold_Img,Inner_Threshold_Img,Inner_Threshold_Img);
		dilate(Inner_Threshold_Img,Inner_Threshold_Img,element,Point(-1,-1),5);
		erode(Inner_Threshold_Img,Inner_Threshold_Img,element,Point(-1,-1),5);
		J_Fitting_Ellipse(Inner_Threshold_Img, Inner_Circle_Info);
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
			struct_Slit_Info.fAngle = round(fangle,1);
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

		// 들뜸 측정 추가
		Mat Danja_DlDDM_Img = Danja_Img.clone(); // 들뜸 측정을 위한 이미지
		Mat Danja_DlDDM_Convex_Img = Mat::zeros(Danja_DlDDM_Img.size(), CV_8UC1);
		erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),2);
		dilate(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),3);
		erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),1);
		vector<vector<Point>> contours1;
		vector<Vec4i> hierarchy1;

		findContours( Danja_DlDDM_Img.clone(), contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		//imwrite("000.bmp",Danja_DlDDM_Img);
		int DlDDM_CNT = 0;
		if (contours1.size() > 0)
		{
			vector<vector<Point>> hull1(contours1.size());
			for(size_t k=0; k<contours1.size(); k++)
			{	
				convexHull( Mat(contours1[k]), hull1[k], false ); 
				//drawContours( Danja_DlDDM_Convex_Img, hull1, k, Scalar(255,255,255),CV_FILLED, 8, hierarchy1);
				drawContours( Danja_DlDDM_Convex_Img, hull1, (int)k, CV_RGB(255,255,255), CV_FILLED, 8, vector<Vec4i>(), 0, Point() );
			}
			//imwrite("001.bmp",Danja_DlDDM_Convex_Img);
			////imwrite("Danja_DlDDM_Convex_Img.bmp",Danja_DlDDM_Convex_Img);
			////imwrite("01_Danja_DlDDM_Img.bmp",Danja_DlDDM_Img);
			subtract(Danja_DlDDM_Convex_Img,Danja_DlDDM_Img,Danja_DlDDM_Img);
			erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),1);
			dilate(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),3);
			erode(Danja_DlDDM_Img,Danja_DlDDM_Img,element,Point(-1,-1),1);

			Rect m_boundRect1(m_boundRect.x - ROI_Margin,m_boundRect.y - ROI_Margin,m_boundRect.width + 2*ROI_Margin,m_boundRect.height + 2*ROI_Margin);
			for(size_t k=0; k<contours1.size(); k++)
			{	
				Rect boundRect1 = boundingRect( Mat(contours1[k]) );
				int t_area1 = countNonZero(Danja_DlDDM_Img(boundRect1));
				if (t_area1 <= ALG_SGYM_Param.DlDDM_Size_Threshold)
				{
					DlDDM_CNT++;
					//drawContours( Dst_Img[Cam_num](tROI)(m_boundRect1), contours1, (int)k, Scalar(255,0,255),CV_FILLED, 8, hierarchy1);
				}
			}
		}
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
			float fAngle;
			if(minRect[i].size.width < minRect[i].size.height){
				if(minRect[i].center.y- Out_Circle_Info[1] < 0)
				{
					fAngle = (minRect[i].angle + 90) + 180;
					//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 90) + 180);
				}
				else
				{
					fAngle = (minRect[i].angle + 90);
					//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 90));
				}
			}
			else
			{
				if(minRect[i].center.y- Out_Circle_Info[1] < 0)
				{
					fAngle = (minRect[i].angle + 180) + 180;
					//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 180) + 180);
				}
				else
				{
					fAngle = (minRect[i].angle + 180);
					//strTemp.Format("Danja Index : %d, X : %.3f, Y : %.3f,width : %.3f,height : %.3f, Angle : %.3f",i,minRect[i].center.x- Out_Circle_Info[0],minRect[i].center.y- Out_Circle_Info[1],minRect[i].size.width,minRect[i].size.height,(minRect[i].angle + 180));
				}
			}

			Danja_Info struct_Danja_Info;
			struct_Danja_Info.fAngle = round(fAngle,1);
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
			ALG_SGYM_Param.vec_Slit_Info[i].fAngleFromCenter = round(Slitangle[i],1);
			if((int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 90 || (int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 0 ||
				(int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 180 || (int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 270 || (int)ALG_SGYM_Param.vec_Slit_Info[i].fAngle == 360 ) continue;;
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
				minRect[i].points( rect_points );
				for( int j = 0; j < 4; j++ )
				{
					line( Dst_Img[Cam_num](tROI), Point2f(rect_points[j].x +m_boundRect.x - ROI_Margin,rect_points[j].y +m_boundRect.y - ROI_Margin) , 
						Point2f(rect_points[(j+1)%4].x +m_boundRect.x - ROI_Margin,rect_points[(j+1)%4].y +m_boundRect.y - ROI_Margin), CV_RGB(255,0,0), 2, 8 );
				}
			}
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


		Result_Info[Cam_num].Format("C0:01_%1.3f_C0:02_%1.3f_C0:03_%1.3f_C0:04_%1.3f_C0:05_%1.3f_C0:06_%1.3f_C0:07_%2.1f_C0:08_%2.1f_C0:09_%1.0f_C0:10_%1.0f_C0:11_%1.0f_C0:12_%1.0f_C0:13_%1.0f_C0:14_%d",ALG_SGYM_Param.Result_Value[0],ALG_SGYM_Param.Result_Value[1],ALG_SGYM_Param.Result_Value[2],ALG_SGYM_Param.Result_Value[3],ALG_SGYM_Param.Result_Value[4],ALG_SGYM_Param.Result_Value[5],ALG_SGYM_Param.Result_Value[6],ALG_SGYM_Param.Result_Value[7],ALG_SGYM_Param.Result_Value[8],ALG_SGYM_Param.Result_Value[9],ALG_SGYM_Param.Result_Value[10],ALG_SGYM_Param.Result_Value[11],ALG_SGYM_Param.Result_Value[12],DlDDM_CNT);
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
