#include "StdAfx.h"
#include "ImPro_Library.h"
#include "math.h"
#define PI 3.14159265

CImPro_Library::CImPro_Library(void)
{
	m_License_Check = 1;
	Result_Text_View = true;
	for (int i=0;i<10;i++)
	{
		Src_Img[i]=NULL;				// 카메라 이미지
		Gray_Img[i]=NULL;				// 카메라이미지 흑백 변환
		Dst_Img[i]=NULL;				// 결과 이미지
		Result_Info[i]="";			// 결과 저장
	}
}


CImPro_Library::~CImPro_Library(void)
{
}

#pragma region 전역 함수
int CImPro_Library::License_Check()
{
	return 0;
	if (m_License_Check > -1)
	{
		m_License_Check++;
		if (m_License_Check > 9)
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

void CImPro_Library::Set_Image_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}
void CImPro_Library::Set_Image_1(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}
void CImPro_Library::Set_Image_2(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}
void CImPro_Library::Set_Image_3(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}
void CImPro_Library::Set_Image_4(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}
void CImPro_Library::Set_Image_5(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}
void CImPro_Library::Set_Image_6(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}

void CImPro_Library::Set_Image_7(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	Rect m_ROI;
	m_ROI.x = 0;m_ROI.y = 0;
	m_ROI.width = 4*((int)size_x/4);
	m_ROI.height = 4*((int)size_y/4);

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img[Cam_num] = src(m_ROI).clone();
		cvtColor(Src_Img[Cam_num],Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = Src_Img[Cam_num].clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_Img[Cam_num] = src(m_ROI).clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Src_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
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


/**
* Function to perform fast template matching with image pyramid
*/
void CImPro_Library::J_FastTemplateMatch(Mat& srca,  // The reference image
	Mat& srcb,  // The template image
	Mat& dst   // Template matching result
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

		// Only keep good matches
		threshold(res, res, 0.3, 1., CV_THRESH_TOZERO);
		results.push_back(res);
	}

	res.copyTo(dst);
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

	//		double cost = ransac_ellipse_fitting (data, no_data, ALG_H0_Param.c_ellipse, 50);

	//		Circle_Info[0] = ALG_H0_Param.c_ellipse.cx;
	//		Circle_Info[1] = ALG_H0_Param.c_ellipse.cy;

	//		Circle_Info[2] = ALG_H0_Param.c_ellipse.w*2;
	//		Circle_Info[3] = ALG_H0_Param.c_ellipse.h*2;
	//		Circle_Info[4] = ALG_H0_Param.c_ellipse.theta*180/M_PI;
	//	}
	//	delete [] data;
	//} else
	//{
	for( size_t k = 0; k < contours.size(); k++ ) 
	{	
		const int no_data = (int)contours[max_num].size();
		sPoint *data = new sPoint[no_data];

		if (no_data > 30)
		{
			for(int i=0; i<no_data; i++)
			{
				data[i].x =  (double)contours[max_num][i].x;
				data[i].y =  (double)contours[max_num][i].y;
			}
			sEllipse c_ellipse;
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
void CImPro_Library::J_Rotate(cv::Mat& src, double angle, cv::Mat& dst)
{
    int len = std::max(src.cols, src.rows);
    cv::Point2f pt(len/2., len/2.);
    cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);

    cv::warpAffine(src, dst, r, cv::Size(len, len));
}

void CImPro_Library::J_DrawArrow(Mat &image, CvPoint p, CvPoint q, CvScalar color, int arrowMagnitude, int thickness, int line_type, int shift) 
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

#pragma endregion

#pragma region H0 관련 함수

bool CImPro_Library::RUN_Algorithm_CAM0()
{
	License_Check();
	int Cam_num = 0;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		Mat Out_binary;
		Mat CP_Gray_Img = Gray_Img[Cam_num].clone();
		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1));
		Mat element_h = getStructuringElement(MORPH_RECT, Size(17, 1), Point(-1, -1) );
		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 15), Point(-1, -1) );
		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;
		CString msg;

		double t_thres = threshold(Gray_Img[Cam_num],Out_binary,ALG_C0_Param.nBGThreshold,255,CV_THRESH_BINARY_INV);

		if (t_thres > 220)
		{
			Result_Info[Cam_num].Format("C0:00_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		} else
		{
			int t_size = countNonZero(Out_binary);
			if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10)
			{
				Result_Info[Cam_num].Format("C0:00_-1");
				rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
				return true;
			}
		}
		//imwrite("00.bmp",Out_binary);

		// 검사대상만 가져오기
		Mat cp_Out_binary = Out_binary.clone();
		resize(Out_binary, Out_binary, Size(Out_binary.rows/8,Out_binary.cols/8));  // 시간을 줄이기위해
		erode(Out_binary,Out_binary,element_v,Point(-1,-1),5);
		dilate(Out_binary,Out_binary,element_v,Point(-1,-1),5);
		dilate(Out_binary,Out_binary,element_h,Point(-1,-1),1);
		erode(Out_binary,Out_binary,element_h,Point(-1,-1),1);
		resize(Out_binary, Out_binary, cp_Out_binary.size());

		//Mat sefse;
		//subtract(cp_Out_binary,Out_binary,sefse);
		
		threshold(Out_binary,Out_binary,50,255,CV_THRESH_BINARY);
		//imwrite("01.bmp",Out_binary);

		findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		int m_max_object_num = -1;int m_max_object_value = 0;
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

		//컨투어를 못 찾을 경우 에러 처리함.
		if(m_max_object_num == -1)
		{
			Result_Info[Cam_num].Format("C0:00_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		}

		Rect Rect_Contour = boundingRect(contours[m_max_object_num]);

		// MBR 이 영상의 하단과 상단에 붙을 경우 에러 처리함.
		if(Rect_Contour.x <= 1 || Rect_Contour.x + Rect_Contour.width >= Gray_Img[Cam_num].cols-1 || 
			Rect_Contour.y <= 1 || Rect_Contour.y + Rect_Contour.height >= Gray_Img[Cam_num].rows-1 )
		{
			Result_Info[Cam_num].Format("C0:00_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		}

		// 상단 폭측정

		int nLeft = -1;
		int nRight = -1;
		vector<int> nVec_Top_Left;
		vector<int> nVec_Top_Right;
		//폭 측정 
		for(int i=Rect_Contour.y+ALG_C0_Param.nTopMargin; i< Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight;i++)
		{
			if (i<0 || i >= Gray_Img[Cam_num].rows)
			{
				continue;
			}
			nLeft = -1;
			for(int j=Rect_Contour.x - 100 ; j<Rect_Contour.x + Rect_Contour.width/2 ; j++)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}
				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C0_Param.nEdgeThreshold)
				{
					nLeft = j;
					break;
				}
			}
			nRight = -1;
			for(int j=Rect_Contour.x + Rect_Contour.width+100; j>Rect_Contour.x + Rect_Contour.width/2 ; j--)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}

				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C0_Param.nEdgeThreshold)
				{
					nRight = j;
					break;
				}
			}
			nVec_Top_Left.push_back(nLeft);
			nVec_Top_Right.push_back(nRight);
		}
		Find_Center_Distance_Average(nVec_Top_Left,nVec_Top_Right,ALG_C0_Param.nCalMin,ALG_C0_Param.nCalMax);
		
		if (Result_Text_View)
		{
			line( Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,0), Point((int)ALG_C0_Param.oTopCenterX,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
			Rect TopROI;
			TopROI.x = Rect_Contour.x - 100;
			TopROI.y = Rect_Contour.y+ALG_C0_Param.nTopMargin;
			TopROI.width = Rect_Contour.width + 200;
			TopROI.height = ALG_C0_Param.nTopHeight;
			rectangle(Dst_Img[Cam_num],TopROI,CV_RGB(0,255,0),1);

			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oLeftX,TopROI.y+TopROI.height/2), Point((int)ALG_C0_Param.oRightX,TopROI.y+TopROI.height/2), CV_RGB(255,255,0), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oRightX,TopROI.y+TopROI.height/2), Point((int)ALG_C0_Param.oLeftX,TopROI.y+TopROI.height/2), CV_RGB(255,255,0), 10 , 2, CV_AA, 0);

			//circle(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2),2,CV_RGB(255,255,0),3);
			//circle(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2),1,CV_RGB(255,0,0),1);
			msg.Format("W(%1.3f)",ALG_C0_Param.oTopDistance*ALG_C0_Param.Res_X);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oTopCenterX-47,-10+Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
		}

		// 좌측 상단에서 꺽임 Edge까지 측정
		Mat Morph_Img;
		dilate(cp_Out_binary,Morph_Img,element_v,Point(-1,-1),3);
		erode(Morph_Img,Morph_Img,element_v,Point(-1,-1),10);
		dilate(Morph_Img,Morph_Img,element_v,Point(-1,-1),7);
		dilate(Morph_Img,Morph_Img,element_h,Point(-1,-1),3);
		erode(Morph_Img,Morph_Img,element_h,Point(-1,-1),3);
		Morph_Img.copyTo(cp_Out_binary);
		Mat element_v3 = getStructuringElement(MORPH_RECT, Size(1, 3), Point(-1, -1) );
		erode(Morph_Img,Morph_Img,element_v3,Point(-1,-1),1);
		subtract(cp_Out_binary,Morph_Img,Morph_Img);
		//imwrite("00.bmp",cp_Out_binary);
		//imwrite("01.bmp",Morph_Img);

		int nTop = -1;
		int nBottom = -1;

		int t_Max_L_Idx = -1;int t_Max_L_v = 1;
		int t_Max_R_Idx = -1;int t_Max_R_v = 1;
		//폭 측정 
		for(int i=Rect_Contour.y+50; i< Rect_Contour.y + ALG_C0_Param.nTopMargin;i++)
		{
			if (i<0 || i >= Gray_Img[Cam_num].rows)
			{
				continue;
			}
			nTop = 0;
			for(int j=Rect_Contour.x - 100 ; j<Rect_Contour.x + 50 ; j++)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}
				if(Morph_Img.at<uchar>(i,j)>=100)
				{
					nTop++;
				}
			}

			if (t_Max_L_v <= nTop)
			{
				t_Max_L_v = nTop;
				t_Max_L_Idx = i;
			}
			nBottom = 0;
			for(int j=Rect_Contour.x + Rect_Contour.width + 100 ; j>Rect_Contour.x + Rect_Contour.width - 50 ; j--)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}
				if(Morph_Img.at<uchar>(i,j)>=100)
				{
					nBottom++;
				}
			}

			if (t_Max_R_v <= nBottom)
			{
				t_Max_R_v = nBottom;
				t_Max_R_Idx = i;
			}
		}

		if (Result_Text_View)
		{
			if (t_Max_L_Idx != -1)
			{
				line( Dst_Img[Cam_num], Point((int)Rect_Contour.x - 100,t_Max_L_Idx), Point((int)Rect_Contour.x,t_Max_L_Idx), CV_RGB(255,0,255), 2, CV_AA);
			}
			if (t_Max_R_Idx != -1)
			{
				line( Dst_Img[Cam_num], Point((int)Rect_Contour.x + Rect_Contour.width,t_Max_R_Idx), Point((int)Rect_Contour.x+ Rect_Contour.width + 100,t_Max_R_Idx), CV_RGB(255,0,255), 2, CV_AA);
			}
		}

		// 좌측 상단, 하단 측정
		// 상단 폭측정

		vector<int> nVec_Left_Top;
		vector<int> nVec_Left_Bottom;
		//폭 측정 
		for(int j=Rect_Contour.x + ALG_C0_Param.nLeftMargin ; j<Rect_Contour.x  + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth ; j++)
		{
			if (j<0 || j >= Gray_Img[Cam_num].cols)
			{
				continue;
			}
			nTop = -1;
			for(int i=Rect_Contour.y-30; i< Rect_Contour.y+100;i++)
			{
				if (i<0 || i >= Gray_Img[Cam_num].rows)
				{
					continue;
				}
				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C0_Param.nEdgeThreshold)
				{
					nTop = i;
					break;
				}
			}
			nBottom = -1;
			for(int i=Rect_Contour.y + Rect_Contour.height+30; i> Rect_Contour.y + Rect_Contour.height-100;i--)
			{
				if (i<0 || i >= Gray_Img[Cam_num].rows)
				{
					continue;
				}

				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C0_Param.nEdgeThreshold)
				{
					nBottom = i;
					break;
				}
			}
			nVec_Left_Top.push_back(nTop);
			nVec_Left_Bottom.push_back(nBottom);
		}
		Find_Left_Right_Distance_Average(nVec_Left_Top,nVec_Left_Bottom,ALG_C0_Param.nCalMin,ALG_C0_Param.nCalMax,0);
		double v_dist_Left_Top_Edge = ((double)t_Max_L_Idx - ALG_C0_Param.oLeftTopY);
		double v_dist_Left_Edge_Bottom = (ALG_C0_Param.oLeftBottomY - (double)t_Max_L_Idx);
		if (Result_Text_View)
		{
			//line( Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,0), Point((int)ALG_C0_Param.oTopCenterX,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
			Rect TopROI;
			TopROI.x = Rect_Contour.x + ALG_C0_Param.nLeftMargin;
			TopROI.y = Rect_Contour.y-30;
			TopROI.width = ALG_C0_Param.nLeftWidth;
			TopROI.height = Rect_Contour.height+60;
			rectangle(Dst_Img[Cam_num],TopROI,CV_RGB(0,255,0),1);

			line( Dst_Img[Cam_num], Point((int)Rect_Contour.x - 100,(int)ALG_C0_Param.oLeftTopY), Point((int)Rect_Contour.x + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth,(int)ALG_C0_Param.oLeftTopY), CV_RGB(255,0,255), 2, CV_AA);
			line( Dst_Img[Cam_num], Point((int)Rect_Contour.x - 100,(int)ALG_C0_Param.oLeftBottomY), Point((int)Rect_Contour.x + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth,(int)ALG_C0_Param.oLeftBottomY), CV_RGB(255,0,255), 2, CV_AA);

			J_DrawArrow(Dst_Img[Cam_num], Point((int)Rect_Contour.x + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth/2,(int)ALG_C0_Param.oLeftTopY), Point((int)Rect_Contour.x + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth/2,(int)ALG_C0_Param.oLeftBottomY), CV_RGB(255,255,0), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)Rect_Contour.x + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth/2,(int)ALG_C0_Param.oLeftBottomY), Point((int)Rect_Contour.x + ALG_C0_Param.nLeftMargin + ALG_C0_Param.nLeftWidth/2,(int)ALG_C0_Param.oLeftTopY), CV_RGB(255,255,0), 10 , 2, CV_AA, 0);

			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oLeftX-25,(int)ALG_C0_Param.oLeftTopY), Point((int)ALG_C0_Param.oLeftX-25,t_Max_L_Idx), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oLeftX-25,t_Max_L_Idx), Point((int)ALG_C0_Param.oLeftX-25,(int)ALG_C0_Param.oLeftTopY), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oLeftX-25,(int)ALG_C0_Param.oLeftBottomY), Point((int)ALG_C0_Param.oLeftX-25,t_Max_L_Idx), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oLeftX-25,t_Max_L_Idx), Point((int)ALG_C0_Param.oLeftX-25,(int)ALG_C0_Param.oLeftBottomY), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);

			//circle(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2),2,CV_RGB(255,255,0),3);
			//circle(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2),1,CV_RGB(255,0,0),1);
			msg.Format("LH0(%1.3f)",ALG_C0_Param.oLeftDistance*ALG_C0_Param.Res_Y);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oLeftX-20,-10+Rect_Contour.y), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);

			msg.Format("LH1(%1.3f)",v_dist_Left_Top_Edge*ALG_C0_Param.Res_Y);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oLeftX-20,((int)ALG_C0_Param.oLeftTopY + t_Max_L_Idx)/2), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
			msg.Format("LH2(%1.3f)",v_dist_Left_Edge_Bottom*ALG_C0_Param.Res_Y);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oLeftX-20,((int)ALG_C0_Param.oLeftBottomY + t_Max_L_Idx)/2), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
		}


		// 우측 상단, 하단 측정
		// 상단 폭측정

		nTop = -1;
		nBottom = -1;
		vector<int> nVec_Right_Top;
		vector<int> nVec_Right_Bottom;
		//폭 측정 
		for(int j=Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin ; j>Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth ; j--)
		{
			if (j<0 || j >= Gray_Img[Cam_num].cols)
			{
				continue;
			}

			nTop = -1;
			for(int i=Rect_Contour.y-30; i< Rect_Contour.y+100;i++)
			{
				if (i<0 || i >= Gray_Img[Cam_num].rows)
				{
					continue;
				}
				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C0_Param.nEdgeThreshold)
				{
					nTop = i;
					break;
				}
			}
			nBottom = -1;
			for(int i=Rect_Contour.y + Rect_Contour.height+30; i> Rect_Contour.y + Rect_Contour.height-100;i--)
			{
				if (i<0 || i >= Gray_Img[Cam_num].rows)
				{
					continue;
				}

				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C0_Param.nEdgeThreshold)
				{
					nBottom = i;
					break;
				}
			}
			nVec_Right_Top.push_back(nTop);
			nVec_Right_Bottom.push_back(nBottom);
		}
		Find_Left_Right_Distance_Average(nVec_Right_Top,nVec_Right_Bottom,ALG_C0_Param.nCalMin,ALG_C0_Param.nCalMax,1);
		double v_dist_Right_Top_Edge = ((double)t_Max_R_Idx - ALG_C0_Param.oRightTopY);
		double v_dist_Right_Edge_Bottom = (ALG_C0_Param.oRightBottomY - (double)t_Max_R_Idx);

		if (Result_Text_View)
		{
			//line( Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,0), Point((int)ALG_C0_Param.oTopCenterX,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
			Rect TopROI;
			TopROI.x = Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth;
			TopROI.y = Rect_Contour.y-30;
			TopROI.width = ALG_C0_Param.nRightWidth;
			TopROI.height = Rect_Contour.height+60;
			rectangle(Dst_Img[Cam_num],TopROI,CV_RGB(0,255,0),1);

			line( Dst_Img[Cam_num], Point((int)Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth,(int)ALG_C0_Param.oRightBottomY), Point((int)Rect_Contour.x+ Rect_Contour.width + 100,(int)ALG_C0_Param.oRightBottomY), CV_RGB(255,0,255), 2, CV_AA);
			line( Dst_Img[Cam_num], Point((int)Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth,(int)ALG_C0_Param.oRightTopY), Point((int)Rect_Contour.x+ Rect_Contour.width + 100,(int)ALG_C0_Param.oRightTopY), CV_RGB(255,0,255), 2, CV_AA);

			J_DrawArrow(Dst_Img[Cam_num], Point((int)Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth/2,(int)ALG_C0_Param.oRightTopY), Point((int)Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth/2,(int)ALG_C0_Param.oRightBottomY), CV_RGB(255,255,0), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth/2,(int)ALG_C0_Param.oRightBottomY), Point((int)Rect_Contour.x + Rect_Contour.width - ALG_C0_Param.nRightMargin - ALG_C0_Param.nRightWidth/2,(int)ALG_C0_Param.oRightTopY), CV_RGB(255,255,0), 10 , 2, CV_AA, 0);

			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oRightX+25,(int)ALG_C0_Param.oRightTopY), Point((int)ALG_C0_Param.oRightX+25,t_Max_R_Idx), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oRightX+25,t_Max_R_Idx), Point((int)ALG_C0_Param.oRightX+25,(int)ALG_C0_Param.oRightTopY), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oRightX+25,(int)ALG_C0_Param.oRightBottomY), Point((int)ALG_C0_Param.oRightX+25,t_Max_R_Idx), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);
			J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oRightX+25,t_Max_R_Idx), Point((int)ALG_C0_Param.oRightX+25,(int)ALG_C0_Param.oRightBottomY), CV_RGB(50,0,255), 10 , 2, CV_AA, 0);

			//circle(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2),2,CV_RGB(255,255,0),3);
			//circle(Dst_Img[Cam_num], Point((int)ALG_C0_Param.oTopCenterX,Rect_Contour.y+ALG_C0_Param.nTopMargin+ALG_C0_Param.nTopHeight/2),1,CV_RGB(255,0,0),1);
			msg.Format("RH0(%1.3f)",ALG_C0_Param.oRightDistance*ALG_C0_Param.Res_Y);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oRightX-110,-10+Rect_Contour.y), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);

			msg.Format("RH1(%1.3f)",v_dist_Right_Top_Edge*ALG_C0_Param.Res_Y);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oRightX-110,((int)ALG_C0_Param.oRightTopY + t_Max_R_Idx)/2), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
			msg.Format("RH2(%1.3f)",v_dist_Right_Edge_Bottom*ALG_C0_Param.Res_Y);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C0_Param.oRightX-110,((int)ALG_C0_Param.oRightBottomY + t_Max_R_Idx)/2), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);

			msg.Format("H0(%1.3f)",ALG_C0_Param.Res_Y*(ALG_C0_Param.oLeftDistance+ALG_C0_Param.oRightDistance)/2.0);			
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)(ALG_C0_Param.oRightX+ALG_C0_Param.oLeftX)/2-65,-10+Rect_Contour.y), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
			msg.Format("H1(%1.3f)",ALG_C0_Param.Res_Y*(v_dist_Left_Top_Edge+v_dist_Right_Top_Edge)/2.0);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)(ALG_C0_Param.oRightX+ALG_C0_Param.oLeftX)/2-65,((int)ALG_C0_Param.oRightTopY + t_Max_R_Idx + (int)ALG_C0_Param.oLeftTopY + t_Max_L_Idx)/4), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
			msg.Format("H2(%1.3f)",ALG_C0_Param.Res_Y*(v_dist_Left_Edge_Bottom+v_dist_Right_Edge_Bottom)/2.0);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)(ALG_C0_Param.oRightX+ALG_C0_Param.oLeftX)/2-65,((int)ALG_C0_Param.oRightBottomY + t_Max_R_Idx + (int)ALG_C0_Param.oLeftBottomY + t_Max_L_Idx)/4), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
		}

		Result_Info[Cam_num].Format("C0:00_%1.3f_C0:01_%1.3f_C0:02_%1.3f_C0:03_%1.3f", ALG_C0_Param.oTopDistance,(v_dist_Left_Top_Edge+v_dist_Right_Top_Edge)/2.0,(v_dist_Left_Edge_Bottom+v_dist_Right_Edge_Bottom)/2.0,(ALG_C0_Param.oLeftDistance+ALG_C0_Param.oRightDistance)/2.0);
	} else
	{
		Result_Info[Cam_num].Format("C0:00_-1_C0:01_-1_C0:02_-1_C0:03_-1");
	}
	return true;
}

void CImPro_Library::Find_Center_Distance_Average(vector<int> Vec_Left, vector<int> Vec_Right, int nMeasureRange_Low,int nMeasureRange_High) 
{
	double nVectorSize = Vec_Left.size();
	if (nVectorSize == 0)
	{
		ALG_C0_Param.oTopCenterX = 0;
		ALG_C0_Param.oTopDistance = 0;
		return;
	}
	std::sort(Vec_Left.begin(),Vec_Left.end());
	std::sort(Vec_Right.begin(),Vec_Right.end());

	double nLCount = 0;
	double nLSum=0;
	double nRCount = 0;
	double nRSum=0;
	for (int i=(int)(nVectorSize * (double)nMeasureRange_Low / 100);i<(int)(nVectorSize * (double)nMeasureRange_High / 100);i++)
	{
		nLSum += (double)Vec_Left[i];
		nLCount++;
		nRSum += (double)Vec_Right[i];
		nRCount++;
	}
	if (nLCount > 0 && nRCount > 0)
	{
		nLSum/=nLCount;
		nRSum/=nRCount;
	}
	int Cam_num = 0;
	ALG_C0_Param.oTopDistance = abs(nRSum - nLSum);
	ALG_C0_Param.oTopCenterX = nLSum + abs(nRSum - nLSum)/2;
	ALG_C0_Param.oLeftX = nLSum;
	ALG_C0_Param.oRightX = nRSum;

	if (Result_Text_View)
	{
		line( Dst_Img[Cam_num], Point((int)nLSum,0), Point((int)nLSum,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
		line( Dst_Img[Cam_num], Point((int)nRSum,0), Point((int)nRSum,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
	}
}

void CImPro_Library::Find_Left_Right_Distance_Average(vector<int> Vec_Top, vector<int> Vec_Bottom, int nMeasureRange_Low,int nMeasureRange_High, int Option) // Option = 0이면 왼쪽, 1이면 오른쪽 
{
	double nVectorSize = Vec_Top.size();
	if (nVectorSize == 0)
	{
		if (Option == 0)
		{
			ALG_C0_Param.oLeftTopY = 0;
			ALG_C0_Param.oLeftBottomY = 0;
			ALG_C0_Param.oLeftDistance = 0;
		} else
		{
			ALG_C0_Param.oRightTopY = 0;
			ALG_C0_Param.oRightBottomY = 0;
			ALG_C0_Param.oRightDistance = 0;
		}

		return;
	}
	std::sort(Vec_Top.begin(),Vec_Top.end());
	std::sort(Vec_Bottom.begin(),Vec_Bottom.end());

	double nLCount = 0;
	double nLSum=0;
	double nRCount = 0;
	double nRSum=0;
	for (int i=(int)(nVectorSize * (double)nMeasureRange_Low / 100);i<(int)(nVectorSize * (double)nMeasureRange_High / 100);i++)
	{
		nLSum += (double)Vec_Top[i];
		nLCount++;
		nRSum += (double)Vec_Bottom[i];
		nRCount++;
	}
	if (nLCount > 0 && nRCount > 0)
	{
		nLSum/=nLCount;
		nRSum/=nRCount;
	}
	int Cam_num = 0;
	if (Option == 0)
	{
		ALG_C0_Param.oLeftTopY = nLSum;
		ALG_C0_Param.oLeftBottomY = nRSum;
		ALG_C0_Param.oLeftDistance = abs(nRSum - nLSum);
	} else
	{
		ALG_C0_Param.oRightTopY = nLSum;
		ALG_C0_Param.oRightBottomY = nRSum;
		ALG_C0_Param.oRightDistance = abs(nRSum - nLSum);
	}
	//if (Result_Text_View)
	//{
	//	line( Dst_Img[Cam_num], Point((int)nLSum,0), Point((int)nLSum,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
	//	line( Dst_Img[Cam_num], Point((int)nRSum,0), Point((int)nRSum,Dst_Img[Cam_num].rows), CV_RGB(255,0,0), 1, CV_AA);
	//}
}

bool CImPro_Library::RUN_Algorithm_CAM1()
{
	//License_Check();
	int Cam_num = 1;

	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		Mat Out_binary;
		Mat CP_Gray_Img = Gray_Img[Cam_num].clone();
		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1));
		Mat element_h = getStructuringElement(MORPH_RECT, Size(17, 1), Point(-1, -1) );
		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 17), Point(-1, -1) );
		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;

		//dilate(CP_Gray_Img,CP_Gray_Img,element,Point(-1,-1),3);
		//erode(CP_Gray_Img,CP_Gray_Img,element,Point(-1,-1),3);

		double t_thres = threshold(CP_Gray_Img,Out_binary,ALG_C1_Param.nBGThreshold,255,CV_THRESH_BINARY_INV);

		if (t_thres > 220)
		{
			Result_Info[Cam_num].Format("C1:00_-1_C1:01_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		} else
		{
			int t_size = countNonZero(Out_binary);
			if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10)
			{
				Result_Info[Cam_num].Format("C1:00_-1_C1:01_-1");
				rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
				return true;
			}
		}
		//imwrite("00.bmp",Out_binary);
		
		J_Delete_Boundary(Out_binary,10);
		Mat ttInner_Circle = Out_binary.clone();
		Mat Outter_Circle = Out_binary.clone();
		//imwrite("00.bmp",Outter_Circle);
		J_Fill_Hole(Outter_Circle);
		subtract(Outter_Circle,ttInner_Circle,ttInner_Circle);
		dilate(ttInner_Circle,ttInner_Circle,element,Point(-1,-1),10);
		erode(ttInner_Circle,ttInner_Circle,element,Point(-1,-1),20);
		dilate(ttInner_Circle,ttInner_Circle,element,Point(-1,-1),10);

		//imwrite("00.bmp",ttInner_Circle);
		erode(Outter_Circle,Outter_Circle,element,Point(-1,-1),20);
		dilate(Outter_Circle,Outter_Circle,element,Point(-1,-1),20);

		//imwrite("00.bmp",Outter_Circle);
		//Mat Inner_Circle = Out_binary.clone();
		//subtract(Outter_Circle,Inner_Circle,Inner_Circle);
		//Outter_Circle = Inner_Circle.clone();
		//dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),5);
		//J_Fill_Hole(Inner_Circle);
		//erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),5);

		////imwrite("00.bmp",Inner_Circle);
		//erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),10);
		//dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),10);
		////imwrite("01.bmp",Inner_Circle);

		//double circle_info[5];
		//J_Fitting_Ellipse(Inner_Circle,circle_info);
		//
		//Inner_Circle = Mat::zeros(Inner_Circle.size(), CV_8UC1);

		//ellipse(Inner_Circle,Point((int)circle_info[0],(int)circle_info[1]),Size(circle_info[2]/2,circle_info[3]/2),circle_info[4],0,360,Scalar(1,1,1),CV_FILLED,CV_AA,0);
		//
		//multiply(CP_Gray_Img,Inner_Circle,Out_binary);

		//threshold(Out_binary,Out_binary,ALG_C1_Param.nInnerThreshold,255,CV_THRESH_BINARY_INV);
		////imwrite("00.bmp",Out_binary);

		//erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),1);
		//multiply(Out_binary,Inner_Circle,Outter_Circle);
		//dilate(Outter_Circle,Outter_Circle,element,Point(-1,-1),3);
		//erode(Outter_Circle,Outter_Circle,element,Point(-1,-1),3);

		findContours( Outter_Circle.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		int m_max_object_num = -1;int m_max_object_value = 0;
		Rect boundRect;
		for( int k = 0; k < contours.size(); k++ )
		{  
			boundRect = boundingRect( Mat(contours[k]) );
			// MBR 이 영상의 하단과 상단에 붙을 경우 에러 처리함.
			if(boundRect.x > 1 || boundRect.x + boundRect.width < Gray_Img[Cam_num].cols-1 || 
				boundRect.y > 1 || boundRect.y + boundRect.height < Gray_Img[Cam_num].rows-1 )
			{
				if (m_max_object_value<= boundRect.width*boundRect.height 
					&& boundRect.width < 400 && boundRect.height < 400
					&& boundRect.width > 100 && boundRect.height > 100)
				{
					m_max_object_value = boundRect.width*boundRect.height;
					m_max_object_num = k;
				}
			}
		}

		//CString msg;
		//msg.Format("%d",contours.size());
		//AfxMessageBox(msg);
		//컨투어를 못 찾을 경우 에러 처리함.
		if(m_max_object_num == -1)
		{
			Result_Info[Cam_num].Format("C1:00_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		}

		Outter_Circle = Mat::zeros(Outter_Circle.size(), CV_8UC1);
		drawContours(Outter_Circle, contours, m_max_object_num, CV_RGB(1,1,1), CV_FILLED, 8, hierarchy);
		double circle_info[5];
		double out_circle_x = 0;
		double out_circle_y = 0;
		J_Fitting_Ellipse(Outter_Circle,circle_info);
		if (Result_Text_View)
		{
			//Mat t_draw = Outter_Circle*255;
			ellipse(Dst_Img[Cam_num],Point((int)circle_info[0],(int)circle_info[1]),Size(circle_info[2]/2,circle_info[3]/2),circle_info[4],0,360,CV_RGB(0,255,0),1,1,0);
		}

		J_Fitting_Ellipse(ttInner_Circle,circle_info);
		out_circle_x = circle_info[0];
		out_circle_y = circle_info[1];
		if (Result_Text_View)
		{
			//Mat t_draw = Outter_Circle*255;
			ellipse(Dst_Img[Cam_num],Point((int)circle_info[0],(int)circle_info[1]),Size(circle_info[2]/2,circle_info[3]/2),circle_info[4],0,360,CV_RGB(0,0,255),1,1,0);
			circle(Dst_Img[Cam_num],Point((int)circle_info[0],(int)circle_info[1]),1,CV_RGB(0,0,255),1);
		}
	
		erode(Outter_Circle,Outter_Circle,element,Point(-1,-1),ALG_C1_Param.nInnerNum);
		J_Fitting_Ellipse(Outter_Circle,circle_info);

		if (Result_Text_View)
		{
			ellipse(Dst_Img[Cam_num],Point((int)circle_info[0],(int)circle_info[1]),Size(circle_info[2]/2,circle_info[3]/2),circle_info[4],0,360,CV_RGB(0,255,0),1,1,0);
		}

		Mat t_Inner_Circle = Mat::zeros(Outter_Circle.size(), CV_8UC1);
		erode(Outter_Circle,t_Inner_Circle,element,Point(-1,-1),ALG_C1_Param.nInnerNum);
		t_Inner_Circle*=255;
		//imwrite("01.bmp",t_Inner_Circle);
		J_Fitting_Ellipse(t_Inner_Circle,circle_info);
		if (Result_Text_View)
		{
			ellipse(Dst_Img[Cam_num],Point((int)circle_info[0],(int)circle_info[1]),Size(circle_info[2]/2,circle_info[3]/2),circle_info[4],0,360,CV_RGB(0,255,0),1,1,0);
			//circle(Dst_Img[Cam_num],Point((int)circle_info[0],(int)circle_info[1]),1,CV_RGB(0,255,0),1);
		}

		Mat Inner_Circle = Mat::zeros(Outter_Circle.size(), CV_8UC1);
		
		multiply(CP_Gray_Img,Outter_Circle,Outter_Circle);

		//equalizeHist( Outter_Circle, Inner_Circle );
		//imwrite("01.bmp",Outter_Circle);
		Mat t_Circle;
		threshold(Outter_Circle,t_Circle,1,255,CV_THRESH_BINARY);


		threshold(Outter_Circle,Inner_Circle,ALG_C1_Param.nInnerThreshold,255,CV_THRESH_BINARY_INV);
		//Canny(Outter_Circle, Outter_Circle, 30, 30*3, 3);
		//imwrite("02.bmp",Outter_Circle);
		//add(Outter_Circle,Inner_Circle,Inner_Circle);
		subtract(Inner_Circle,t_Inner_Circle,Inner_Circle);
		add(Inner_Circle,Outter_Circle,Outter_Circle);
		threshold(Outter_Circle,Inner_Circle,ALG_C1_Param.nInnerThreshold,255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
		//imwrite("02.bmp",Inner_Circle);

		subtract(t_Circle,Inner_Circle,t_Circle);
		//imwrite("03.bmp",t_Circle);

		Mat Dist_Img, label, Tmp_Img;
		distanceTransform(t_Circle, Dist_Img,label, CV_DIST_L2,CV_DIST_MASK_3);
		double m_distance;Point m_distance_Center;
		minMaxLoc(Dist_Img,NULL,&m_distance,NULL,&m_distance_Center);
		
		Tmp_Img = Mat::zeros(t_Circle.size(), CV_8UC1);
		circle(Tmp_Img,m_distance_Center,m_distance,CV_RGB(255,255,255),6);
		bitwise_and(Tmp_Img,255-t_Circle,Tmp_Img);
		//imwrite("02.bmp",Tmp_Img);

		int m_area = 0;

		vector<int> px;
		vector<int> py;
		cv::Vec3f vec_ = cv::Vec3f(0, 0, 255);

		for (int i=(int)m_distance_Center.x - (int)m_distance - 10;i<(int)m_distance_Center.x + (int)m_distance + 10;i++)
		{
			for (int j=(int)m_distance_Center.y - (int)m_distance - 10;j<(int)m_distance_Center.y + (int)m_distance + 10;j++)
			{
				if (i<=0 || i>=Tmp_Img.cols || j<=0 || j>=Tmp_Img.rows)
				{
					continue;
				}
				if (Tmp_Img.at<uchar>(j,i) > 1)
				{
					m_area++;
					px.push_back(i);
					py.push_back(j);
					if (Result_Text_View)
					{
						Dst_Img[Cam_num].at<cv::Vec3b>(j,i) = vec_;
					}
				}
			}
		}
		if (px.size() >= 10)
		{
			const int no_data = px.size();
			sPoint *data = new sPoint[no_data];
			for(int i=0; i<no_data; i++)
			{
				data[i].x =  (double)px[i];
				data[i].y =  (double)py[i];
			}
			sEllipse c_ellipse;
			double cost = ransac_ellipse_fitting (data, no_data, c_ellipse, 50);

			m_distance_Center.x = c_ellipse.cx;
			m_distance_Center.y = c_ellipse.cy;

			m_distance = (c_ellipse.w+c_ellipse.h)/2;
			//AfxMessageBox("1");
			delete [] data;
		}

		if (Result_Text_View)
		{
			ellipse(Dst_Img[Cam_num],m_distance_Center,Size(m_distance,m_distance),0,0,360,CV_RGB(200,100,0),1,1,0);
			circle(Dst_Img[Cam_num],m_distance_Center,1,CV_RGB(200,100,0),1);
		}
		//imwrite("02.bmp",Tmp_Img);
		//erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),1);
		//dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),3);
		//erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),4);
		//imwrite("03.bmp",Inner_Circle);

		//findContours( Inner_Circle.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		//m_max_object_num = -1;m_max_object_value = 0;

		//for( int k = 0; k < contours.size(); k++ )
		//{  
		//	boundRect = boundingRect( Mat(contours[k]) );
		//	// MBR 이 영상의 하단과 상단에 붙을 경우 에러 처리함.
		//	if(boundRect.x > 1 || boundRect.x + boundRect.width < Gray_Img[Cam_num].cols-1 || 
		//		boundRect.y > 1 || boundRect.y + boundRect.height < Gray_Img[Cam_num].rows-1 )
		//	{
		//		if (m_max_object_value<= boundRect.width*boundRect.height)
		//		{
		//			m_max_object_value = boundRect.width*boundRect.height;
		//			m_max_object_num = k;
		//		}

		//		if (Result_Text_View)
		//		{
		//			drawContours(Dst_Img[Cam_num], contours, k, CV_RGB(0,100,255), 1, 1, hierarchy);
		//		}
		//		m_area += (int)(arcLength(contours[k],false));
		//	}
		//}

		double t_d_center = 0;

		t_d_center = sqrtf(ALG_C1_Param.Res_X*ALG_C1_Param.Res_X*((double)m_distance_Center.x - out_circle_x) * ((double)m_distance_Center.x - out_circle_x)
			+ ALG_C1_Param.Res_Y*ALG_C1_Param.Res_Y*((double)m_distance_Center.y - out_circle_y) * ((double)m_distance_Center.y - out_circle_y));

		CString msg;
		if (Result_Text_View)
		{
			msg.Format("L(%d)",m_area);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(out_circle_x-40,out_circle_y - 30), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
			msg.Format("dC(%1.3f)",t_d_center);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(out_circle_x-40,out_circle_y + 30), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
		}
		//Rect Rect_Contour = boundingRect(contours[m_max_object_num]);
		if (m_area != 0)
		{
			//m_area = (int)(contourArea(contours[m_max_object_num],false) + 0.5);
		} else
		{
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
		}

		t_d_center = sqrtf(((double)m_distance_Center.x - out_circle_x) * ((double)m_distance_Center.x - out_circle_x)
			+ ((double)m_distance_Center.y - out_circle_y) * ((double)m_distance_Center.y - out_circle_y));

		Result_Info[Cam_num].Format("C1:00_%d_C1:01_%1.3f",m_area,t_d_center);
	} else
	{
		Result_Info[Cam_num].Format("C1:00_-1_C1:01_-1");
	}
	return true;
}

bool CImPro_Library::RUN_Algorithm_CAM2()
{
	//License_Check();
	int Cam_num = 2;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		Mat Out_binary;
		Mat CP_Gray_Img = Gray_Img[Cam_num].clone();
		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1));
		Mat element_h = getStructuringElement(MORPH_RECT, Size(17, 1), Point(-1, -1) );
		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 17), Point(-1, -1) );
		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;

		//dilate(CP_Gray_Img,CP_Gray_Img,element,Point(-1,-1),2);
		//erode(CP_Gray_Img,CP_Gray_Img,element,Point(-1,-1),2);

		double t_thres = threshold(CP_Gray_Img,Out_binary,ALG_C2_Param.nBGThreshold,255,CV_THRESH_BINARY);

		if (t_thres > 255)
		{
			Result_Info[Cam_num].Format("C2:00_-1_C2:01_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		} else
		{
			int t_size = countNonZero(Out_binary);
			if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10)
			{
				Result_Info[Cam_num].Format("C2:00_-1_C2:01_-1");
				rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
				return true;
			}
		}

		//J_Delete_Boundary(Out_binary,10);
		Mat Inner_Circle = Out_binary.clone();

		erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),20);
		dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),20);
		//imwrite("00.bmp",Inner_Circle);


		//J_Fill_Hole(Outter_Circle);
		//imwrite("00.bmp",Outter_Circle);
		//subtract(Outter_Circle,Inner_Circle,Inner_Circle);

		//dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),3);
		//erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),3);
		//J_Fill_Hole(Inner_Circle);
		//Mat CP_Inner_Circle = Inner_Circle.clone();
		//subtract(Outter_Circle,Inner_Circle,Inner_Circle);
		//subtract(Outter_Circle,Inner_Circle,Inner_Circle);

		//dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),20);
		//bitwise_and(Inner_Circle,Out_binary,Outter_Circle);
		//subtract(Outter_Circle,CP_Inner_Circle,Outter_Circle);

		//imwrite("02.bmp",Outter_Circle);

		findContours( Inner_Circle.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		int m_max_object_num = -1;int m_max_object_value = 0;
		Rect boundRect;
		for( int k = 0; k < contours.size(); k++ )
		{  
			boundRect = boundingRect( Mat(contours[k]) );
			// MBR 이 영상의 하단과 상단에 붙을 경우 에러 처리함.
			if(boundRect.x > 1 || boundRect.x + boundRect.width < Gray_Img[Cam_num].cols-1 || 
				boundRect.y > 1 || boundRect.y + boundRect.height < Gray_Img[Cam_num].rows-1 )
			{
				if (m_max_object_value<= boundRect.width*boundRect.height 
					//&& boundRect.width < 400 && boundRect.height < 400
					&& boundRect.width > 200 && boundRect.height > 200
					&& boundRect.height/boundRect.width < 1.2 
					&& boundRect.height/boundRect.width > 0.8)
				{
					m_max_object_value = boundRect.width*boundRect.height;
					m_max_object_num = k;
				}
			}
		}

		//컨투어를 못 찾을 경우 에러 처리함.
		if(m_max_object_num == -1)
		{
			Result_Info[Cam_num].Format("C2:00_-1_C2:01_-1");
			rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
			return true;
		}

		Mat Outter_Circle = Out_binary.clone();
		Rect Rect_Contour = boundingRect(contours[m_max_object_num]);
		Inner_Circle = Mat::zeros(Outter_Circle.size(), CV_8UC1);
		drawContours( Inner_Circle, contours, m_max_object_num, Scalar(1,1,1), CV_FILLED, 8, hierarchy );
		//imwrite("01.bmp",255*Inner_Circle);

		dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),30);
		Outter_Circle = Mat::zeros(Outter_Circle.size(), CV_8UC1);
		multiply(Inner_Circle,Gray_Img[Cam_num],Outter_Circle);
		threshold(Outter_Circle,Inner_Circle,1,255,CV_THRESH_BINARY_INV);
		threshold(Outter_Circle,Outter_Circle,ALG_C2_Param.nBandThreshold,255,CV_THRESH_BINARY_INV);
		subtract(Outter_Circle,Inner_Circle,Inner_Circle);
		drawContours( Inner_Circle, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy );
		dilate(Inner_Circle,Inner_Circle,element,Point(-1,-1),2);
		erode(Inner_Circle,Inner_Circle,element,Point(-1,-1),2);
		J_Fill_Hole(Inner_Circle);
		//imwrite("02.bmp",Inner_Circle);

		double Circle_Info[5] = {0,}; // 내경 : 중심X,Y,가로,세로,각도
		J_Fitting_Ellipse(Inner_Circle.clone(), Circle_Info);
		double m_OutCircle = (Circle_Info[2]+Circle_Info[3])/2;
		if (Result_Text_View)
		{
			circle(Dst_Img[Cam_num],Point(Circle_Info[0],Circle_Info[1]),m_OutCircle/2,CV_RGB(255,50,255),2);
		}
		Mat Ref_Circle = Mat::zeros(Outter_Circle.size(), CV_8UC1);
		circle(Ref_Circle,Point(Circle_Info[0],Circle_Info[1]),ALG_C2_Param.nDistCentertoDolgi,Scalar(255,255,255),2); // Mask
		J_Fill_Hole(Ref_Circle);
		subtract(Ref_Circle,Inner_Circle,Ref_Circle);

		multiply(Ref_Circle/255,Gray_Img[Cam_num],Outter_Circle);

		threshold(Outter_Circle,Outter_Circle,ALG_C2_Param.nInnerThreshold,255,CV_THRESH_BINARY);
		dilate(Outter_Circle,Outter_Circle,element,Point(-1,-1),2);
		erode(Outter_Circle,Outter_Circle,element,Point(-1,-1),2);
		J_Fill_Hole(Outter_Circle);

		erode(Outter_Circle,Outter_Circle,element,Point(-1,-1),5);
		dilate(Outter_Circle,Outter_Circle,element,Point(-1,-1),12);
		erode(Outter_Circle,Outter_Circle,element,Point(-1,-1),4);
		J_Fill_Hole(Outter_Circle);

		//imwrite("00.bmp",Outter_Circle);

		Mat Dist_Img, label, Tmp_Img;
		distanceTransform(Outter_Circle, Dist_Img,label, CV_DIST_L2,CV_DIST_MASK_5);
		double m_distance;Point m_distance_Center;
		minMaxLoc(Dist_Img,NULL,&m_distance,NULL,&m_distance_Center);
		//imwrite("00.bmp",Outter_Circle);
		//int m_area = (int)(contourArea(contours[m_max_object_num],false) + 0.5);
		CString msg;
		if (Result_Text_View)
		{
			circle(Dst_Img[Cam_num],Point((Circle_Info[0]+m_distance_Center.x)/2,(Circle_Info[1]+m_distance_Center.y)/2),m_distance,CV_RGB(255,100,0),2);
			circle(Dst_Img[Cam_num],Point(Circle_Info[0],Circle_Info[1]),ALG_C2_Param.nDistCentertoDolgi,Scalar(0,255,0),1); // Mask
			circle(Dst_Img[Cam_num],Point(Circle_Info[0],Circle_Info[1]),3,CV_RGB(0,255,0),3);
			circle(Dst_Img[Cam_num],Point(Circle_Info[0],Circle_Info[1]),1,CV_RGB(255,0,0),1);

			msg.Format("D0(%1.3f)",0.5*(ALG_C2_Param.Res_X+ALG_C2_Param.Res_Y)*m_distance*2);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(Circle_Info[0]-60,Circle_Info[1] - (int)m_distance - 10), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,100,0), 2, 8);
			msg.Format("D1(%1.3f)",0.5*(ALG_C2_Param.Res_X+ALG_C2_Param.Res_Y)*m_OutCircle);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(Circle_Info[0]-60,Circle_Info[1] - (int)m_OutCircle/2 + 40), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,50,255), 2, 8);

			//drawContours(Dst_Img[Cam_num], contours, m_max_object_num, CV_RGB(100,255,0), 2, CV_AA, hierarchy);

			//findContours( Outter_Circle.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
			//drawContours(Dst_Img[Cam_num], contours, 0, CV_RGB(0,100,255), 1, CV_AA, hierarchy);
		}
		Result_Info[Cam_num].Format("C2:00_%1.3f_C2:01_%1.3f",m_distance*2,m_OutCircle);
	} else
	{
		Result_Info[Cam_num].Format("C2:00_-1_C2:01_-1");
	}
	return true;
}
bool CImPro_Library::RUN_Algorithm_CAM3()
{
	//License_Check();
	int Cam_num = 3;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		Mat Out_binary;
		Mat CP_Gray_Img = Gray_Img[Cam_num].clone();
		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1));
		Mat element_h = getStructuringElement(MORPH_RECT, Size(5, 1), Point(-1, -1) );
		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 5), Point(-1, -1) );
		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;

		//dilate(CP_Gray_Img,CP_Gray_Img,element,Point(-1,-1),3);
		//erode(CP_Gray_Img,CP_Gray_Img,element,Point(-1,-1),3);

		double t_thres = threshold(CP_Gray_Img,Out_binary,ALG_C3_Param.nInnerThreshold,255,CV_THRESH_BINARY_INV);

		//int t_size = countNonZero(Out_binary);
		//if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10)
		//{
		//	Result_Info[Cam_num].Format("C3:00_-1");
		//	rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
		//	return true;
		//}
		//imwrite("00.bmp",Out_binary(ALG_C3_Param.ROI));
		Mat ROI_Binary = Out_binary(ALG_C3_Param.ROI).clone();
		J_Delete_Boundary(ROI_Binary,1);
		J_Fill_Hole(ROI_Binary);
		//imwrite("00.bmp",ROI_Binary);
		dilate(ROI_Binary,ROI_Binary,element_v,Point(-1,-1),1);
		erode(ROI_Binary,ROI_Binary,element_v,Point(-1,-1),1);

		Mat Dist_Img, label, Tmp_Img;
		distanceTransform(ROI_Binary, Dist_Img,label, CV_DIST_L2,3);
		Dist_Img.convertTo(Tmp_Img,CV_8UC1);
		Out_binary = Mat::zeros(Out_binary.size(), CV_8UC1); 
		//CString msg;
		int t_max_dist = 0;int error_cnt = 0;
		for (int j=0;j<Dist_Img.rows;j++)
		{
			t_max_dist = 0;
			for (int i=0;i<Dist_Img.cols;i++)
			{
				if (Tmp_Img.at<uchar>(j,i) >= t_max_dist)
				{
					t_max_dist = (int)Tmp_Img.at<uchar>(j,i);
				}
			}
			//msg.Format("%d, %1.0f",t_max_dist,(double)ALG_C3_Param.nThickness/2);
			//AfxMessageBox(msg);
			if ((double)t_max_dist <= (double)ALG_C3_Param.nThickness/2)
			{
				error_cnt++;

				if (Result_Text_View)
				{
					for (int i=0;i<Dist_Img.cols;i++)
					{
						if (ROI_Binary.at<uchar>(j,i) > 1)
						{
							Out_binary.at<uchar>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = 255;
							Dst_Img[Cam_num].at<cv::Vec3b>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = Vec3b(0,100,255);
						}

						if (t_max_dist == 0)
						{
							Out_binary.at<uchar>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = 255;
							Dst_Img[Cam_num].at<cv::Vec3b>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = Vec3b(0,100,255);
						}
					}
				} else
				{
					for (int i=0;i<Dist_Img.cols;i++)
					{
						if (ROI_Binary.at<uchar>(j,i) > 1)
						{
							Out_binary.at<uchar>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = 255;
						}
						if (t_max_dist == 0)
						{
							Out_binary.at<uchar>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = 255;
							//Dst_Img[Cam_num].at<cv::Vec3b>(j+ALG_C3_Param.ROI.y,i+ALG_C3_Param.ROI.x) = Vec3b(0,100,255);
						}
					}
				}
			}
		}

		dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);
		erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);

		//imwrite("00.bmp",Out_binary);

		findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		int m_max_object_num = -1;int m_max_object_value = 0;
		Rect boundRect;
		double m_area = 0;
		for( int k = 0; k < contours.size(); k++ )
		{  
			//m_area = contourArea(contours[k],false);

			boundRect = boundingRect( Mat(contours[k]) );
			// MBR 이 영상의 하단과 상단에 붙을 경우 에러 처리함.
			//if(boundRect.x > 1 || boundRect.x + boundRect.width < Gray_Img[Cam_num].cols-1 || 
			//	boundRect.y > 1 || boundRect.y + boundRect.height < Gray_Img[Cam_num].rows-1 )
			if (m_max_object_value <= boundRect.width*boundRect.height)
			{
				m_max_object_value = boundRect.width*boundRect.height;
				//m_max_object_value = m_area;
				m_max_object_num = k;
			}
		}
		if (m_max_object_num >= 0 && Result_Text_View)
		{
			drawContours( Dst_Img[Cam_num], contours, m_max_object_num, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy );
		}
		if (Result_Text_View)
		{
			rectangle(Dst_Img[Cam_num],ALG_C3_Param.ROI,CV_RGB(0,255,0),1,8);
		}

		//Rect Rect_Contour = boundingRect(contours[m_max_object_num]);

		Result_Info[Cam_num].Format("C3:00_%d",m_max_object_value);
	} else
	{
		Result_Info[Cam_num].Format("C3:00_-1");
	}
	return true;
}

void CImPro_Library::Find_Top_Distance_Average(vector<int> Vec_Left, vector<int> Vec_Right, int nMeasureRange_Low,int nMeasureRange_High) 
{
	double nVectorSize = Vec_Left.size();
	if (nVectorSize == 0)
	{
		ALG_C4_Param.oTopCenterX = 0;
		ALG_C4_Param.oTopDistance = 0;
		return;
	}
	std::sort(Vec_Left.begin(),Vec_Left.end());
	std::sort(Vec_Right.begin(),Vec_Right.end());

	double nLCount = 0;
	double nLSum=0;
	double nRCount = 0;
	double nRSum=0;
	for (int i=(int)(nVectorSize * (double)nMeasureRange_Low / 100);i<(int)(nVectorSize * (double)nMeasureRange_High / 100);i++)
	{
		nLSum += (double)Vec_Left[i];
		nLCount++;
		nRSum += (double)Vec_Right[i];
		nRCount++;
	}
	if (nLCount > 0 && nRCount > 0)
	{
		nLSum/=nLCount;
		nRSum/=nRCount;
	}
	int Cam_num = 4;
	ALG_C4_Param.oTopDistance = abs(nRSum - nLSum);
	ALG_C4_Param.oTopCenterX = nLSum + abs(nRSum - nLSum)/2;
	ALG_C4_Param.oTopLeftX = nLSum;
	ALG_C4_Param.oTopRightX = nRSum;
	if (Result_Text_View)
	{
		line( Dst_Img[Cam_num], Point((int)nLSum,0), Point((int)nLSum,Dst_Img[Cam_num].rows), CV_RGB(255,100,0), 1, CV_AA);
		line( Dst_Img[Cam_num], Point((int)nRSum,0), Point((int)nRSum,Dst_Img[Cam_num].rows), CV_RGB(255,100,0), 1, CV_AA);
	}
}

void CImPro_Library::Find_Bottom_Distance_Average(vector<int> Vec_Left, vector<int> Vec_Right, int nMeasureRange_Low,int nMeasureRange_High) 
{
	double nVectorSize = Vec_Left.size();
	if (nVectorSize == 0)
	{
		ALG_C4_Param.oBottomCenterX = 0;
		ALG_C4_Param.oBottomDistance = 0;
		return;
	}
	std::sort(Vec_Left.begin(),Vec_Left.end());
	std::sort(Vec_Right.begin(),Vec_Right.end());

	double nLCount = 0;
	double nLSum=0;
	double nRCount = 0;
	double nRSum=0;
	for (int i=(int)(nVectorSize * (double)nMeasureRange_Low / 100);i<(int)(nVectorSize * (double)nMeasureRange_High / 100);i++)
	{
		nLSum += (double)Vec_Left[i];
		nLCount++;
		nRSum += (double)Vec_Right[i];
		nRCount++;
	}
	if (nLCount > 0 && nRCount > 0)
	{
		nLSum/=nLCount;
		nRSum/=nRCount;
	}
	int Cam_num = 4;
	ALG_C4_Param.oBottomDistance = abs(nRSum - nLSum);
	ALG_C4_Param.oBottomCenterX = nLSum + abs(nRSum - nLSum)/2;
	ALG_C4_Param.oBottomLeftX = nLSum;
	ALG_C4_Param.oBottomRightX = nRSum;

	if (Result_Text_View)
	{
		line( Dst_Img[Cam_num], Point((int)nLSum,0), Point((int)nLSum,Dst_Img[Cam_num].rows), CV_RGB(100,0,255), 1, CV_AA);
		line( Dst_Img[Cam_num], Point((int)nRSum,0), Point((int)nRSum,Dst_Img[Cam_num].rows), CV_RGB(100,0,255), 1, CV_AA);
	}
}


bool CImPro_Library::RUN_Algorithm_CAM4()
{
	//License_Check();
	int Cam_num = 4;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		Mat Out_binary;
		Mat CP_Gray_Img = Gray_Img[Cam_num].clone();
		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1));
		Mat element_h = getStructuringElement(MORPH_RECT, Size(17, 1), Point(-1, -1) );
		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 17), Point(-1, -1) );
		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;
		CString msg;
		////double t_thres = threshold(Gray_Img[Cam_num],Out_binary,ALG_C0_Param.nBGThreshold,255,CV_THRESH_BINARY_INV);

		////if (t_thres > 220)
		////{
		////	Result_Info[Cam_num].Format("C0:00_-1");
		////	rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
		////	return true;
		////} else
		////{
		////	int t_size = countNonZero(Out_binary);
		////	if (t_size >= 9*Out_binary.rows * Out_binary.cols / 10)
		////	{
		////		Result_Info[Cam_num].Format("C0:00_-1");
		////		rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
		////		return true;
		////	}
		////}

		//// 검사대상만 가져오기
		//Mat cp_Out_binary = Out_binary.clone();
		//resize(Out_binary, Out_binary, Size(Out_binary.rows/8,Out_binary.cols/8));  // 시간을 줄이기위해
		//erode(Out_binary,Out_binary,element_v,Point(-1,-1),6);
		//dilate(Out_binary,Out_binary,element_v,Point(-1,-1),6);
		//dilate(Out_binary,Out_binary,element_h,Point(-1,-1),1);
		//erode(Out_binary,Out_binary,element_h,Point(-1,-1),1);
		//resize(Out_binary, Out_binary, cp_Out_binary.size());

		////Mat sefse;
		////subtract(cp_Out_binary,Out_binary,sefse);

		//threshold(Out_binary,Out_binary,50,255,CV_THRESH_BINARY);
		////imwrite("00.bmp",Out_binary);

		//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
		//int m_max_object_num = -1;int m_max_object_value = 0;
		//Rect boundRect;
		//for( int k = 0; k < contours.size(); k++ )
		//{  
		//	boundRect = boundingRect( Mat(contours[k]) );
		//	if (m_max_object_value<= boundRect.width*boundRect.height)
		//	{
		//		m_max_object_value = boundRect.width*boundRect.height;
		//		m_max_object_num = k;
		//	}
		//}

		////컨투어를 못 찾을 경우 에러 처리함.
		//if(m_max_object_num == -1)
		//{
		//	Result_Info[Cam_num].Format("C0:00_-1");
		//	rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
		//	return true;
		//}

		//Rect Rect_Contour = boundingRect(contours[m_max_object_num]);

		//// MBR 이 영상의 하단과 상단에 붙을 경우 에러 처리함.
		//if(Rect_Contour.x <= 1 || Rect_Contour.x + Rect_Contour.width >= Gray_Img[Cam_num].cols-1 || 
		//	Rect_Contour.y <= 1 || Rect_Contour.y + Rect_Contour.height >= Gray_Img[Cam_num].rows-1 )
		//{
		//	Result_Info[Cam_num].Format("C0:00_-1");
		//	rectangle(Dst_Img[Cam_num],Rect(100,100,Out_binary.cols-200,Out_binary.rows-200),Scalar(0,0,255),2);
		//	return true;
		//}

		// 상단 폭측정

		int nLeft = -1;
		int nRight = -1;
		vector<int> nVec_Top_Left;
		vector<int> nVec_Top_Right;
		//폭 측정 
		for(int i=ALG_C4_Param.Top_ROI.y; i< ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height;i++)
		{
			if (i<0 || i >= Gray_Img[Cam_num].rows)
			{
				continue;
			}
			nLeft = -1;
			for(int j=ALG_C4_Param.Top_ROI.x; j<ALG_C4_Param.Top_ROI.x + ALG_C4_Param.Top_ROI.width/2 ; j++)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}
				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C4_Param.nConcentricEdgeThreshold)
				{
					nLeft = j;
					break;
				}
			}
			nRight = -1;
			for(int j=ALG_C4_Param.Top_ROI.x + ALG_C4_Param.Top_ROI.width; j>ALG_C4_Param.Top_ROI.x + ALG_C4_Param.Top_ROI.width/2 ; j--)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}

				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C4_Param.nConcentricEdgeThreshold)
				{
					nRight = j;
					break;
				}
			}
			nVec_Top_Left.push_back(nLeft);
			nVec_Top_Right.push_back(nRight);
		}
		Find_Top_Distance_Average(nVec_Top_Left,nVec_Top_Right,ALG_C4_Param.nCalMin,ALG_C4_Param.nCalMax);
		//AfxMessageBox("0");

		nLeft = -1;
		nRight = -1;
		vector<int> nVec_Bottom_Left;
		vector<int> nVec_Bottom_Right;
		//하단 폭 측정 
		for(int i=ALG_C4_Param.Bottom_ROI.y; i< ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height;i++)
		{
			if (i<0 || i >= Gray_Img[Cam_num].rows)
			{
				continue;
			}
			nLeft = -1;
			for(int j=ALG_C4_Param.Bottom_ROI.x; j<ALG_C4_Param.Bottom_ROI.x + ALG_C4_Param.Bottom_ROI.width/2 ; j++)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}
				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C4_Param.nConcentricEdgeThreshold)
				{
					nLeft = j;
					break;
				}
			}
			nRight = -1;
			for(int j=ALG_C4_Param.Bottom_ROI.x + ALG_C4_Param.Bottom_ROI.width; j>ALG_C4_Param.Bottom_ROI.x + ALG_C4_Param.Top_ROI.width/2 ; j--)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}

				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C4_Param.nConcentricEdgeThreshold)
				{
					nRight = j;
					break;
				}
			}
			nVec_Bottom_Left.push_back(nLeft);
			nVec_Bottom_Right.push_back(nRight);
		}
		Find_Bottom_Distance_Average(nVec_Bottom_Left,nVec_Bottom_Right,ALG_C4_Param.nCalMin,ALG_C4_Param.nCalMax);
		//AfxMessageBox("1");

		double t_center = 0;
		if (ALG_C4_Param.oTopDistance > 0 && ALG_C4_Param.oBottomDistance > 0)
		{
			t_center = ALG_C4_Param.oTopCenterX - ALG_C4_Param.oBottomCenterX;
		}
		if (Result_Text_View)
		{
			rectangle(Dst_Img[Cam_num],ALG_C4_Param.Top_ROI,CV_RGB(0,255,0),1);
			rectangle(Dst_Img[Cam_num],ALG_C4_Param.Bottom_ROI,CV_RGB(0,255,0),1);

			line( Dst_Img[Cam_num], Point((int)ALG_C4_Param.oTopCenterX,0), Point((int)ALG_C4_Param.oTopCenterX,Dst_Img[Cam_num].rows/2), CV_RGB(255,0,0), 1, CV_AA);
			line( Dst_Img[Cam_num], Point((int)ALG_C4_Param.oBottomCenterX,Dst_Img[Cam_num].rows/2), Point((int)ALG_C4_Param.oBottomCenterX,Dst_Img[Cam_num].rows), CV_RGB(0,0,255), 1, CV_AA);

			//J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oTopLeftX,ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2), Point((int)ALG_C4_Param.oTopRightX,ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
			//J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oTopRightX,ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2), Point((int)ALG_C4_Param.oTopLeftX,ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
			//J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oBottomLeftX,ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height/2), Point((int)ALG_C4_Param.oBottomRightX,ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height/2), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
			//J_DrawArrow(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oBottomRightX,ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height/2), Point((int)ALG_C4_Param.oBottomLeftX,ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height/2), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);

			circle(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oTopCenterX,ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2),2,CV_RGB(255,255,0),3);
			circle(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oTopCenterX,ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2),2,CV_RGB(255,0,0),1);
			circle(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oBottomCenterX,ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height/2),2,CV_RGB(255,255,0),3);
			circle(Dst_Img[Cam_num], Point((int)ALG_C4_Param.oBottomCenterX,ALG_C4_Param.Bottom_ROI.y+ALG_C4_Param.Bottom_ROI.height/2),2,CV_RGB(255,0,0),1);
			
			msg.Format("dC(%1.3f)",ALG_C4_Param.Res_X*t_center);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C4_Param.oTopCenterX-47,-10+ALG_C4_Param.Top_ROI.y+ALG_C4_Param.Top_ROI.height/2), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,255,0), 2, 8);
		}
		//AfxMessageBox("2");
		nLeft = -1;
		nRight = -1;
		vector<int> nVec_MinMax_Left;
		vector<int> nVec_MinMax_Right;
		vector<int> nVec_MinMaxDist_Right;
		//하단 폭 측정 
		for(int i=ALG_C4_Param.Top_y; i< ALG_C4_Param.Bottom_y;i++)
		{
			if (i<0 || i >= Gray_Img[Cam_num].rows)
			{
				continue;
			}

			nLeft = -1;
			for(int j=ALG_C4_Param.Bottom_ROI.x; j<ALG_C4_Param.Bottom_ROI.x + ALG_C4_Param.Bottom_ROI.width/2 ; j++)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}
				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C4_Param.nMinMaxEdgeThreshold)
				{
					nLeft = j;
					break;
				}
			}
			nRight = -1;
			for(int j=ALG_C4_Param.Bottom_ROI.x + ALG_C4_Param.Bottom_ROI.width; j>ALG_C4_Param.Bottom_ROI.x + ALG_C4_Param.Top_ROI.width/2 ; j--)
			{
				if (j<0 || j >= Gray_Img[Cam_num].cols)
				{
					continue;
				}

				if(CP_Gray_Img.at<uchar>(i,j)<=ALG_C4_Param.nMinMaxEdgeThreshold)
				{
					nRight = j;
					break;
				}
			}
			nVec_MinMax_Left.push_back(nLeft);
			nVec_MinMax_Right.push_back(nRight);
			nVec_MinMaxDist_Right.push_back(nRight-nLeft);
		}

		int t_min = 999999;int t_min_idx = -1;
		int t_max = 0;int t_max_idx = -1;

		for (int i=0;i<nVec_MinMaxDist_Right.size();i++)
		{
			if (nVec_MinMaxDist_Right[i] < t_min)
			{
				t_min = nVec_MinMaxDist_Right[i];
				t_min_idx = i;
			}
			if (nVec_MinMaxDist_Right[i] > t_max)
			{
				t_max = nVec_MinMaxDist_Right[i];
				t_max_idx = i;
			}
		}

		if (Result_Text_View)
		{
			line( Dst_Img[Cam_num], Point(0,ALG_C4_Param.Top_y), Point(Dst_Img[Cam_num].cols,ALG_C4_Param.Top_y), CV_RGB(0,255,0), 1, CV_AA);
			line( Dst_Img[Cam_num], Point(0,ALG_C4_Param.Bottom_y), Point(Dst_Img[Cam_num].cols,ALG_C4_Param.Bottom_y), CV_RGB(0,255,0), 1, CV_AA);
		
			if (t_min_idx != -1)
			{
				J_DrawArrow(Dst_Img[Cam_num], Point(nVec_MinMax_Left[t_min_idx],ALG_C4_Param.Top_y + t_min_idx), Point(nVec_MinMax_Right[t_min_idx],ALG_C4_Param.Top_y + t_min_idx), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
				J_DrawArrow(Dst_Img[Cam_num], Point(nVec_MinMax_Right[t_min_idx],ALG_C4_Param.Top_y + t_min_idx), Point(nVec_MinMax_Left[t_min_idx],ALG_C4_Param.Top_y + t_min_idx), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
				msg.Format("min(%1.3f)",ALG_C4_Param.Res_X*(double)t_min);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C4_Param.oBottomCenterX-47,-10+ALG_C4_Param.Top_y + t_min_idx), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,255,0), 2, 8);
			}
			if (t_max_idx != -1)
			{
				J_DrawArrow(Dst_Img[Cam_num], Point(nVec_MinMax_Left[t_max_idx],ALG_C4_Param.Top_y + t_max_idx), Point(nVec_MinMax_Right[t_max_idx],ALG_C4_Param.Top_y + t_max_idx), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
				J_DrawArrow(Dst_Img[Cam_num], Point(nVec_MinMax_Right[t_max_idx],ALG_C4_Param.Top_y + t_max_idx), Point(nVec_MinMax_Left[t_max_idx],ALG_C4_Param.Top_y + t_max_idx), CV_RGB(255,255,0), 10 , 1, CV_AA, 0);
				msg.Format("max(%1.3f)",ALG_C4_Param.Res_X*(double)t_max);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point((int)ALG_C4_Param.oBottomCenterX-47,-10+ALG_C4_Param.Top_y + t_max_idx), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(255,255,0), 2, 8);
			}
		}

		Result_Info[Cam_num].Format("C4:00_%1.3f_C4:01_%d_C4:02_%d", t_center,t_min,t_max);
	} else
	{
		Result_Info[Cam_num].Format("C4:00_-1_C4:01_-1_C4:02_-1");
	}
	return true;}
bool CImPro_Library::RUN_Algorithm_CAM5()
{
	License_Check();
	int Cam_num = 5;

	return true;
}
bool CImPro_Library::RUN_Algorithm_CAM6()
{
	License_Check();
	int Cam_num = 6;

	return true;
}
bool CImPro_Library::RUN_Algorithm_CAM7()
{
	License_Check();
	int Cam_num = 7;

	return true;
}
#pragma endregion

#pragma region 참고소스 주석
//
//void CImPro_Library::RUN_Algorithm_03()
//{
//	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
//	//QueryPerformanceFrequency(&frequency);
//	int Cam_num = 3;
//	if (!Gray_Img[Cam_num].empty())
//	{
//		CString msg;
//
//		//QueryPerformanceCounter(&tStart ); // 시간체크 시작
//
//		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
//		Mat element_h = getStructuringElement(MORPH_RECT, Size(3, 1), Point(-1, -1) );
//		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 3), Point(-1, -1) );
//
//
//		// 2. 임계화(반전)
//		//threshold(Gray_Img,Binary_Img,100,255,CV_THRESH_BINARY_INV);
//		blur(Gray_Img[Cam_num], Binary_Img[Cam_num], Size(3,3));
//		Mat Out_binary = Binary_Img[Cam_num].clone();
//		Mat In_binary = Binary_Img[Cam_num].clone();
//		threshold(Out_binary,Out_binary,100,255,CV_THRESH_BINARY_INV);
//		threshold(In_binary,In_binary,m_down_threshold,255,CV_THRESH_BINARY_INV);
//
//		//Mat temp_binary = Binary_Img[Cam_num].clone();
//
//		//Delete_Small_Binary(Binary_Img[Cam_num],5000);
//
//		//imshow("In_binary",In_binary);
//
//		//subtract(temp_binary,Binary_Img[Cam_num],Binary_Img[Cam_num]);
//		//subtract(temp_binary,Binary_Img[Cam_num],Binary_Img[Cam_num]);
//
//
//
//
//		//blur(Gray_Img, Binary_Img, Size(3,3));
//
//		//// Run the edge detector on grayscale
//		//Canny(Binary_Img, Binary_Img, 30, 30*3, 3);
//
//		//Mat T_Binary_Img = Binary_Img[Cam_num].clone();
//
//
//		//Mat element31 = getStructuringElement(MORPH_RECT, Size(31, 31), Point(-1, -1) );
//
//		//dilate(Binary_Img[Cam_num],Binary_Img[Cam_num],element31,Point(-1,-1),1);
//		//erode(Binary_Img[Cam_num],Binary_Img[Cam_num],element31,Point(-1,-1),1);
//		//dilate(Binary_Img,Binary_Img,element,Point(-1,-1),1);
//
//		//Delete_Small_Binary(Binary_Img[Cam_num],2000);
//		//imshow("Binary_Img",Binary_Img[Cam_num]);
//		Mat CP_Binary_Img = In_binary.clone();
//
//		vector<vector<Point> > contours1;
//		vector<Vec4i> hierarchy1;
//
//		findContours( CP_Binary_Img, contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//		vector<Rect> boundRect( contours1.size() );
//		int m_max_object_num = -1;int m_max_object_value = 0;
//		for( int k = 0; k < contours1.size(); k++ )
//		{  
//			boundRect[k] = boundingRect( Mat(contours1[k]) );
//			if (m_max_object_value<= boundRect[k].width*boundRect[k].height)
//			{
//				if (boundRect[k].x + boundRect[k].width/2 >= 1*Binary_Img[Cam_num].cols/4 && boundRect[k].x + boundRect[k].width/2 <= 3*Binary_Img[Cam_num].cols/4)
//				{
//					if (boundRect[k].y + boundRect[k].height/2 >= 1*Binary_Img[Cam_num].rows/4 && boundRect[k].y + boundRect[k].height/2 <= 3*Binary_Img[Cam_num].rows/4)
//					{
//						m_max_object_value = boundRect[k].width*boundRect[k].height;
//						m_max_object_num = k;
//					}
//				}
//			}
//		}
//
//		Circle_Info[0]="";
//		Circle_Ratio_Info[0] = "";
//		Blob_Info[Cam_num] = "";
//
//		if (m_max_object_num == -1)
//		{
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//			return;
//		}
//
//		int m_margin = 5;
//		if (m_max_object_num > -1)
//		{
//			boundRect[m_max_object_num].x -= m_margin;
//			boundRect[m_max_object_num].y -= m_margin;
//			boundRect[m_max_object_num].width += 2*m_margin;
//			boundRect[m_max_object_num].height += 2*m_margin;
//			boundRect[m_max_object_num].width = 4*(ceilf(boundRect[m_max_object_num].width/4));
//			boundRect[m_max_object_num].height = 4*(ceilf(boundRect[m_max_object_num].height/4));
//			if (boundRect[m_max_object_num].x < 0 || boundRect[m_max_object_num].y < 0 ||
//				boundRect[m_max_object_num].x + boundRect[m_max_object_num].width >= Gray_Img[Cam_num].cols ||
//				boundRect[m_max_object_num].y + boundRect[m_max_object_num].height >= Gray_Img[Cam_num].rows)
//			{
//				//AfxMessageBox("sef");
//				cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//				//QueryPerformanceCounter( &tEnd );
//				//msg.Format("T/T:%1.2f ms", 1000*(tEnd.QuadPart - tStart.QuadPart) / (double)frequency.QuadPart);
//				//int fontFace = FONT_HERSHEY_SIMPLEX;
//				//double fontScale = 0.9;
//				//int thickness = 2;
//
//				//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(35,35), fontFace, fontScale,
//				//	Scalar(255,0,0), thickness, 8);
//
//				//putText(Dst_Img[Cam_num], "N G", Point(Gray_Img[Cam_num].cols/4,Gray_Img[Cam_num].rows/2), FONT_HERSHEY_COMPLEX, 10,
//				//	Scalar(0,0,255), 10, 8);
//
//				return;
//			}
//
//			Mat ROI_Gray_Img = Gray_Img[Cam_num](boundRect[m_max_object_num]).clone();
//			Mat ROI_Out_Img = Out_binary(boundRect[m_max_object_num]).clone();
//			Mat ROI_Inner_Img = In_binary(boundRect[m_max_object_num]).clone();
//			//erode(ROI_Inner_Img,ROI_Inner_Img,element,Point(-1,-1),4);
//			//dilate(ROI_Inner_Img,ROI_Inner_Img,element,Point(-1,-1),4);
//
//			////threshold(ROI_Inner_Img,ROI_Inner_Img,150,1,CV_THRESH_BINARY);
//
//			////Mat ROI_Out_Img = ROI_Inner_Img.clone();
//			////ROI_Inner_Img.copyTo(ROI_Gray_Img);
//			////ROI_Out_Img*=255;
//			//Mat Tmp_Gray_Img = ROI_Inner_Img.clone();
//			//subtract(ROI_Inner_Img,ROI_Out_Img,Tmp_Gray_Img);
//
//			//imshow("Tmp_Gray_Img",Tmp_Gray_Img);
//
//
//			int m_border = 50;
//			Mat ROI_Out_Exp_Img;
//			copyMakeBorder( ROI_Out_Img, ROI_Out_Exp_Img, m_border, m_border, m_border, m_border, BORDER_CONSTANT, Scalar(0) );
//			//ROI_Inner_Img.copyTo(ROI_Out_Img);
//			//ROI_Out_Img*=255;
//
//			dilate(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element,Point(-1,-1),10);
//			erode(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element,Point(-1,-1),10);
//			Rect m_Exp_ROI;
//			m_Exp_ROI.x = m_border;m_Exp_ROI.y = m_border;m_Exp_ROI.width = ROI_Out_Img.cols;m_Exp_ROI.height = ROI_Out_Img.rows;
//			subtract(ROI_Out_Exp_Img(m_Exp_ROI),ROI_Out_Img,ROI_Out_Img);
//
//			m_border = 70;
//			copyMakeBorder( ROI_Out_Img, ROI_Out_Exp_Img, m_border, m_border, m_border, m_border, BORDER_CONSTANT, Scalar(0) );
//			dilate(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element,Point(-1,-1),15);
//			erode(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element,Point(-1,-1),16);
//			erode(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element_v,Point(-1,-1),3);
//			erode(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element_h,Point(-1,-1),2);
//			dilate(ROI_Out_Exp_Img,ROI_Out_Exp_Img,element_h,Point(-1,-1),2);
//
//			m_Exp_ROI.x = m_border;m_Exp_ROI.y = m_border;m_Exp_ROI.width = ROI_Out_Img.cols;m_Exp_ROI.height = ROI_Out_Img.rows;
//			subtract(ROI_Out_Exp_Img(m_Exp_ROI),ROI_Out_Img,ROI_Out_Img);
//
//			erode(ROI_Out_Img,ROI_Out_Img,element,Point(-1,-1),1);
//			dilate(ROI_Out_Img,ROI_Out_Img,element,Point(-1,-1),1);
//
//			//imshow("ROI_Out_Img",ROI_Out_Img);
//			//
//
//			//multiply(ROI_Gray_Img,ROI_Inner_Img,ROI_Inner_Img);
//
//			//threshold(ROI_Inner_Img,ROI_Inner_Img,m_up_threshold,255,CV_THRESH_BINARY);
//			////imshow("threshold",ROI_Inner_Img);
//
//			//// 알고리즘 수정(유수) 
//
//			//ROI_Inner_Img.copyTo(ROI_Gray_Img);
//
//			//dilate(ROI_Gray_Img,ROI_Gray_Img,element,Point(-1,-1),10);
//			//erode(ROI_Gray_Img,ROI_Gray_Img,element,Point(-1,-1),10);
//
//			//subtract(ROI_Gray_Img,ROI_Inner_Img,ROI_Gray_Img);
//
//			//erode(ROI_Gray_Img,ROI_Gray_Img,element,Point(-1,-1),1);
//			//dilate(ROI_Gray_Img,ROI_Gray_Img,element,Point(-1,-1),1);
//
//			//imshow("ROI_Gray_Img",ROI_Gray_Img);
//
//			//return;
//
//
//			//double Circle_Info[5] = {0,}; // 내경 : 중심X,Y,가로,세로,각도
//
//			////dilate(ROI_Inner_Img,ROI_Inner_Img,element,Point(-1,-1),10);
//			////erode(ROI_Inner_Img,ROI_Inner_Img,element,Point(-1,-1),10);
//			////dilate(ROI_Inner_Img,ROI_Inner_Img,element,Point(-1,-1),1);
//			//fitting_ellipse(ROI_Inner_Img, Circle_Info,Cam_num);
//			//ROI_Inner_Img = Mat::zeros(Size(ROI_Gray_Img.cols,ROI_Gray_Img.rows), CV_8U); 
//			//ellipse(ROI_Inner_Img,Point(Circle_Info[0],Circle_Info[1]),Size(Circle_Info[2],Circle_Info[3]),Circle_Info[4],0,360,Scalar(1),1);
//			//Mat Dist_Img, label, Tmp_Img;
//			////imshow("ROI_Inner_Img",ROI_Inner_Img*255);
//			////imshow("ROI_Inner_Img",ROI_Inner_Img*255);
//
//			//dilate(ROI_Gray_Img,ROI_Gray_Img,element,Point(-1,-1),1);
//
//			//distanceTransform(ROI_Gray_Img, Dist_Img,label, CV_DIST_L2,3);
//			//Dist_Img.convertTo(Tmp_Img,CV_8UC1);
//			//multiply(Tmp_Img,ROI_Inner_Img,Dist_Img);
//
//			//for (int i=0;i<Tmp_Img.rows;i++)
//			//{
//			//	for (int j=0;j<Tmp_Img.cols;j++)
//			//	{
//			//		if (Tmp_Img.at<uchar>(i,j)==0 && ROI_Inner_Img.at<uchar>(i,j)>=1)
//			//		{
//			//			Tmp_Img.at<uchar>(i,j) = 100;
//			//		}
//			//	}
//			//}
//
//			//threshold(Tmp_Img,Tmp_Img,50,255,CV_THRESH_BINARY);
//			//dilate(Tmp_Img,Tmp_Img,element,Point(-1,-1),1);
//
//			// 오링 바깥쪽 BURR 검출(Out_Burr)
//			//Delete_Small_Binary(Out_Burr,1);
//			Cal_Blob_Info("윗면불량", ROI_Out_Img,Cam_num);
//			//imshow("Out_Burr",Out_Burr);
//
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//
//			findContours( ROI_Out_Img, contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours1.size(); k++ )
//			{  
//				drawContours( Dst_Img[Cam_num](boundRect[m_max_object_num]), contours1, k, Scalar(0,0,255), 2, CV_AA, hierarchy1);
//			}
//		} else
//		{
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//		}
//	}
//}
//
//void CImPro_Library::RUN_Algorithm_02()
//{
//	//QueryPerformanceFrequency(&frequency);
//	int Cam_num = 2;
//	if (!Gray_Img[Cam_num].empty())
//	{
//		CString msg;
//
//		//QueryPerformanceCounter(&tStart ); // 시간체크 시작
//
//		threshold(Gray_Img[Cam_num],Binary_Img[Cam_num],m_baseline_height,255,CV_THRESH_BINARY);
//		Mat Cp_Binary_Img = Binary_Img[Cam_num].clone();
//		Delete_Small_Binary(Cp_Binary_Img,100);
//
//		vector<vector<Point> > contours;
//		vector<Vec4i> hierarchy;
//
//		findContours( Cp_Binary_Img, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//		vector<Rect> boundRect( contours.size() );
//		int m_max_object_num = -1;int m_max_object_value = 0;
//		for( int k = 0; k < contours.size(); k++ )
//		{  
//			boundRect[k] = boundingRect( Mat(contours[k]) );
//			if (m_max_object_value<= boundRect[k].width*boundRect[k].height)
//			{
//				m_max_object_value = boundRect[k].width*boundRect[k].height;
//				m_max_object_num = k;
//			}
//		}
//
//		if (m_max_object_num == -1)
//		{
//			Blob_Info[Cam_num] = "";
//			Blob_Info[Cam_num].Format("오링높이_%1.3f",0);
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//			return;
//		}
//
//		if (boundRect[m_max_object_num].height < 50)
//		{
//			Blob_Info[Cam_num] = "";
//			Blob_Info[Cam_num].Format("오링높이_%1.3f",0);
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//			return;
//		}
//		int min_x = 10000;
//		int max_x = 0;
//		for (int i=boundRect[m_max_object_num].x;i<boundRect[m_max_object_num].x+boundRect[m_max_object_num].width;i++)
//		{
//			for (int j=boundRect[m_max_object_num].y;j<boundRect[m_max_object_num].y+5;j++)
//			{
//				if (Binary_Img[Cam_num].at<uchar>(j,i) == 255)
//				{
//					if (min_x > i)
//					{
//						min_x = i;
//					}
//					if (max_x < i)
//					{
//						max_x = i;
//					}
//					break;
//				}
//			}
//		}
//
//		double m_height = 0;
//		double m_cnt = 0;
//
//		if (min_x != 10000 && max_x != 0)
//		{
//			for (int i=min_x;i<max_x;i++)
//			{
//				for (int j=boundRect[m_max_object_num].y;j<boundRect[m_max_object_num].y+5;j++)
//				{
//					if (Binary_Img[Cam_num].at<uchar>(j,i) == 255)
//					{
//						m_height += j;
//						m_cnt++;
//						break;
//					}
//				}
//			}
//			if (m_cnt == 0)
//			{
//				m_height = 0;
//			} else
//			{
//				m_height/=m_cnt;
//			}
//		}
//
//		double m_bottom = 0;
//		m_cnt = 0;
//
//		if (boundRect[m_max_object_num].height < 150)
//		{
//			if (min_x != 10000 && max_x != 0)
//			{
//				for (int i=boundRect[m_max_object_num].x;i<boundRect[m_max_object_num].x+10;i++)
//				{
//					for (int j=boundRect[m_max_object_num].y+boundRect[m_max_object_num].height;j>boundRect[m_max_object_num].y+boundRect[m_max_object_num].height-5;j--)
//					{
//						if (Binary_Img[Cam_num].at<uchar>(j,i) == 255)
//						{
//							m_bottom += j;
//							m_cnt++;
//							break;
//						}
//					}
//				}
//				if (m_cnt == 0)
//				{
//					m_bottom = m_baseline_height;
//				} else
//				{
//					m_bottom/=m_cnt;
//				}
//			}
//		} else
//		{
//			if (min_x != 10000 && max_x != 0)
//			{
//				for (int i=boundRect[m_max_object_num].x;i<boundRect[m_max_object_num].x+10;i++)
//				{
//					for (int j=boundRect[m_max_object_num].y;j<boundRect[m_max_object_num].y+boundRect[m_max_object_num].height;j++)
//					{
//						if (Binary_Img[Cam_num].at<uchar>(j,i) == 255)
//						{
//							m_bottom += j;
//							m_cnt++;
//							//break;
//						}
//					}
//				}
//				if (m_cnt == 0)
//				{
//					m_bottom = boundRect[m_max_object_num].y+boundRect[m_max_object_num].height;
//				} else
//				{
//					m_bottom/=m_cnt;
//				}
//			}
//		}
//
//
//		//msg.Format("%f",m_height);
//		//AfxMessageBox(msg);
//
//		Blob_Info[Cam_num] = "";
//		//Blob_Info[Cam_num].Format("오링높이_%d",(int)((double)Binary_Img[Cam_num].rows-m_height-m_baseline_height));
//		Blob_Info[Cam_num].Format("오링높이_%1.3f",m_bottom-m_height);
//
//		//Dst_Img[Cam_num] = Mat::zeros(Size(Gray_Img[Cam_num].cols,Gray_Img[Cam_num].rows), CV_8UC3); 
//		cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//
//		//imwrite("Dst_Img.bmp",Dst_Img[Cam_num]);
//
//
//		//QueryPerformanceCounter( &tEnd );
//		//msg.Format("T/T:%1.2f ms", 1000*(tEnd.QuadPart - tStart.QuadPart) / (double)frequency.QuadPart);
//		//int fontFace = FONT_HERSHEY_SIMPLEX;
//		//double fontScale = 0.3;
//		//int thickness = 1.5;
//
//		//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(25,15), fontFace, fontScale,
//		//	Scalar(255,0,0), thickness, 8);
//
//		if (min_x != 10000 && max_x != 0)
//		{
//			line(Dst_Img[Cam_num],cvPoint(min_x - 200, m_height),cvPoint(max_x + 200, m_height),CV_RGB(255, 128, 0), 2 );
//			line(Dst_Img[Cam_num],cvPoint(min_x - 200, m_bottom),cvPoint(max_x + 200, m_bottom),CV_RGB(255, 128, 0), 2 );
//
//			drawArrow(Dst_Img[Cam_num], cvPoint((max_x+min_x)/2, m_height), cvPoint((max_x+min_x)/2, m_bottom), CV_RGB(255, 0, 0), 7, 1);
//			drawArrow(Dst_Img[Cam_num], cvPoint((max_x+min_x)/2, m_bottom), cvPoint((max_x+min_x)/2, m_height), CV_RGB(255, 0, 0), 7, 1);
//		}
//	}
//}
//
//void CImPro_Library::drawArrow(Mat &image, CvPoint p, CvPoint q, CvScalar color, int arrowMagnitude, int thickness, int line_type, int shift) 
//{
//	//Draw the principle line
//	line(image,p, q, color, thickness, line_type, shift);
//	//cvLine(image, p, q, color, thickness, line_type, shift);
//	const double PI = 3.141592653;
//	//compute the angle alpha
//	double angle = atan2((double)p.y-q.y, (double)p.x-q.x);
//	//compute the coordinates of the first segment
//	p.x = (int) ( q.x +  arrowMagnitude * cos(angle + PI/4));
//	p.y = (int) ( q.y +  arrowMagnitude * sin(angle + PI/4));
//	//Draw the first segment
//	line(image, p, q, color, thickness, line_type, shift);
//	//compute the coordinates of the second segment
//	p.x = (int) ( q.x +  arrowMagnitude * cos(angle - PI/4));
//	p.y = (int) ( q.y +  arrowMagnitude * sin(angle - PI/4));
//	//Draw the second segment
//	line(image, p, q, color, thickness, line_type, shift);
//}
//
//void CImPro_Library::RUN_Algorithm_01()
//{
//	int Cam_num = 1;
//	if (!Gray_Img[Cam_num].empty())
//	{
//		CString msg;
//
//		//QueryPerformanceCounter(&tStart ); // 시간체크 시작
//
//		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
//		Mat element_h = getStructuringElement(MORPH_RECT, Size(3, 1), Point(-1, -1) );
//		Mat element_v = getStructuringElement(MORPH_RECT, Size(1, 3), Point(-1, -1) );
//
//
//		// 2. 임계화(반전)
//		//threshold(Gray_Img,Binary_Img,100,255,CV_THRESH_BINARY_INV);
//		blur(Gray_Img[Cam_num], Binary_Img[Cam_num], Size(3,3));
//		threshold(Binary_Img[Cam_num],Binary_Img[Cam_num],200,255,CV_THRESH_BINARY_INV);
//
//		Mat temp_binary = Binary_Img[Cam_num].clone();
//
//		Delete_Small_Binary(Binary_Img[Cam_num],5000);
//
//		subtract(temp_binary,Binary_Img[Cam_num],Binary_Img[Cam_num]);
//		subtract(temp_binary,Binary_Img[Cam_num],Binary_Img[Cam_num]);
//
//
//		//imshow("Binary_Img",Binary_Img[Cam_num]);
//
//		//blur(Gray_Img, Binary_Img, Size(3,3));
//
//		//// Run the edge detector on grayscale
//		//Canny(Binary_Img, Binary_Img, 30, 30*3, 3);
//
//		Mat T_Binary_Img = Binary_Img[Cam_num].clone();
//
//
//		dilate(Binary_Img[Cam_num],Binary_Img[Cam_num],element,Point(-1,-1),2);
//		erode(Binary_Img[Cam_num],Binary_Img[Cam_num],element,Point(-1,-1),2);
//		//dilate(Binary_Img,Binary_Img,element,Point(-1,-1),1);
//
//		//imshow("Binary_Img",Binary_Img);
//		Fill_Hole(Binary_Img[Cam_num]);
//
//		//Delete_Small_Binary(Binary_Img[Cam_num],2000);
//		//imshow("Binary_Img",Binary_Img[Cam_num]);
//		Mat CP_Binary_Img = Binary_Img[Cam_num].clone();
//
//		vector<vector<Point> > contours1;
//		vector<Vec4i> hierarchy1;
//
//		findContours( Binary_Img[Cam_num], contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//		vector<Rect> boundRect( contours1.size() );
//		int m_max_object_num = -1;int m_max_object_value = 0;
//		for( int k = 0; k < contours1.size(); k++ )
//		{  
//			boundRect[k] = boundingRect( Mat(contours1[k]) );
//			if (m_max_object_value<= boundRect[k].width*boundRect[k].height)
//			{
//				m_max_object_value = boundRect[k].width*boundRect[k].height;
//				m_max_object_num = k;
//			}
//		}
//
//		Circle_Info[0]="";
//		Circle_Ratio_Info[0] = "";
//		Blob_Info[Cam_num] = "";
//
//		if (m_max_object_num == -1)
//		{
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//			return;
//		}
//
//		int m_margin = 5;
//		if (m_max_object_num > -1)
//		{
//			boundRect[m_max_object_num].x -= m_margin;
//			boundRect[m_max_object_num].y -= m_margin;
//			boundRect[m_max_object_num].width += 2*m_margin;
//			boundRect[m_max_object_num].height += 2*m_margin;
//			boundRect[m_max_object_num].width = 4*(ceilf(boundRect[m_max_object_num].width/4));
//			boundRect[m_max_object_num].height = 4*(ceilf(boundRect[m_max_object_num].height/4));
//			if (boundRect[m_max_object_num].x < 0 || boundRect[m_max_object_num].y < 0 ||
//				boundRect[m_max_object_num].x + boundRect[m_max_object_num].width >= Gray_Img[Cam_num].cols ||
//				boundRect[m_max_object_num].y + boundRect[m_max_object_num].height >= Gray_Img[Cam_num].rows)
//			{
//				//AfxMessageBox("sef");
//				cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//				//QueryPerformanceCounter( &tEnd );
//				//msg.Format("T/T:%1.2f ms", 1000*(tEnd.QuadPart - tStart.QuadPart) / (double)frequency.QuadPart);
//				//int fontFace = FONT_HERSHEY_SIMPLEX;
//				//double fontScale = 0.9;
//				//int thickness = 2;
//
//				//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(35,35), fontFace, fontScale,
//				//	Scalar(255,0,0), thickness, 8);
//
//				//putText(Dst_Img[Cam_num], "N G", Point(Gray_Img[Cam_num].cols/4,Gray_Img[Cam_num].rows/2), FONT_HERSHEY_COMPLEX, 10,
//				//	Scalar(0,0,255), 10, 8);
//
//				return;
//			}
//
//			Mat ROI_Gray_Img = Gray_Img[Cam_num](boundRect[m_max_object_num]).clone();
//			Mat ROI_Outter_Img = CP_Binary_Img(boundRect[m_max_object_num]).clone();
//			subtract(CP_Binary_Img,T_Binary_Img,CP_Binary_Img);
//			Mat ROI_Inner_Img = CP_Binary_Img(boundRect[m_max_object_num]).clone();
//
//			//----------------------------------------------------------------- //
//			// Step 3. 검사 실시
//			//----------------------------------------------------------------- //
//
//			// 내경, 외경 계산
//			Mat Inner_Circle_Binary = ROI_Inner_Img.clone();
//			Mat Outter_Circle_Binary = ROI_Outter_Img.clone();
//
////			imshow("sefs",Inner_Circle_Binary);
////imshow("sefs1",Outter_Circle_Binary);
//
//			double Inner_Circle_Info[5] = {0,}; // 내경 : 중심X,Y,가로,세로,각도
//			double Outer_Circle_Info[5] = {0,}; // 외경 : 중심X,Y,가로,세로,각도
//			fitting_ellipse(Inner_Circle_Binary, Inner_Circle_Info,Cam_num);
//			fitting_ellipse(Outter_Circle_Binary, Outer_Circle_Info,Cam_num);
//
//
//
//			Circle_Info[0].Format("InCircle_%1.3f_%1.3f_%1.3f_OutSize_%1.3f_%1.3f_%1.3f_",(Inner_Circle_Info[2]+Inner_Circle_Info[3])/2,Inner_Circle_Info[0],Inner_Circle_Info[1],(Outer_Circle_Info[2]+Outer_Circle_Info[3])/2,Outer_Circle_Info[0],Outer_Circle_Info[1]);
//			//AfxMessageBox(Circle_Info[0]);
//
//			Circle_Ratio_Info[0].Format("_InCircleRatio_%1.3f_OutCircleRatio_%1.3f_",100*(min(Inner_Circle_Info[2],Inner_Circle_Info[3])/max(Inner_Circle_Info[2],Inner_Circle_Info[3])), 100*(min(Outer_Circle_Info[2],Outer_Circle_Info[3])/max(Outer_Circle_Info[2],Outer_Circle_Info[3])));
//			//
//			Mat In_Defect;Mat In_Burr;Mat In_Miss;
//			Mat Out_Burr;Mat Out_Miss;
//
//
//
//			// 오링 내부에 속한 이물 검출(In_Defect)
//			Mat CP_ROI_Inner_Img = ROI_Inner_Img.clone();
//			Delete_Small_Binary(CP_ROI_Inner_Img,min(Inner_Circle_Info[2],Inner_Circle_Info[3])*6);			
//			subtract(ROI_Inner_Img,CP_ROI_Inner_Img,In_Defect);
//			Delete_Small_Binary(In_Defect,1);
//			Cal_Blob_Info("내부이물", In_Defect,Cam_num);
//			//imshow("In_Defect",In_Defect);
//
//			// 오링 내부 레퍼런스 원
//			Scalar color( 255);
//			Inner_Circle_Binary = Mat::zeros(Inner_Circle_Binary.size(), CV_8UC1);
//			ellipse(Inner_Circle_Binary,Point(Inner_Circle_Info[0],Inner_Circle_Info[1]),Size(Inner_Circle_Info[2],Inner_Circle_Info[3]),Inner_Circle_Info[4],0,360,color,1);
//			Fill_Hole(Inner_Circle_Binary); 
//
//			// 오링 안쪽 BURR 검출(In_Burr)
//			subtract(Inner_Circle_Binary,CP_ROI_Inner_Img,In_Burr);
//			erode(In_Burr,In_Burr,element,Point(-1,-1),1);
//			dilate(In_Burr,In_Burr,element,Point(-1,-1),1);
//			Delete_Small_Binary(In_Burr,1);		
//			Cal_Blob_Info("안쪽BURR", In_Burr,Cam_num);
//			//imshow("In_Burr",In_Burr);
//
//			// 오링 안쪽 소실된 부분 검출(In_Miss)
//			dilate(Inner_Circle_Binary,Inner_Circle_Binary,element,Point(-1,-1),1);
//			subtract(CP_ROI_Inner_Img,Inner_Circle_Binary,In_Miss);
//			erode(In_Miss,In_Miss,element,Point(-1,-1),1);
//			dilate(In_Miss,In_Miss,element,Point(-1,-1),1);
//			Delete_Small_Binary(In_Miss,1);	
//			Cal_Blob_Info("안쪽소실", In_Miss,Cam_num);
//			//imshow("In_Miss",In_Miss);
//
//			// 오링 외부 레퍼런스 원
//			Outter_Circle_Binary = Mat::zeros(Outter_Circle_Binary.size(), CV_8UC1);
//			ellipse(Outter_Circle_Binary,Point(Outer_Circle_Info[0],Outer_Circle_Info[1]),Size(Outer_Circle_Info[2],Outer_Circle_Info[3]),Outer_Circle_Info[4],0,360,color,1);
//			Fill_Hole(Outter_Circle_Binary);
//
//			// 오링 바깥쪽 소실된 부분 검출(Out_Miss)
//			subtract(Outter_Circle_Binary,ROI_Outter_Img,Out_Miss);
//			erode(Out_Miss,Out_Miss,element,Point(-1,-1),1);
//			dilate(Out_Miss,Out_Miss,element,Point(-1,-1),1);
//			Delete_Small_Binary(Out_Miss,1);
//			Cal_Blob_Info("바깥쪽소실", Out_Miss,Cam_num);
//			//imshow("Out_Miss",Out_Miss);
//
//			// 오링 바깥쪽 BURR 검출(Out_Burr)
//			dilate(Outter_Circle_Binary,Outter_Circle_Binary,element,Point(-1,-1),1);
//			subtract(ROI_Outter_Img,Outter_Circle_Binary,Out_Burr);
//			erode(Out_Burr,Out_Burr,element,Point(-1,-1),1);
//			dilate(Out_Burr,Out_Burr,element,Point(-1,-1),1);
//			Delete_Small_Binary(Out_Burr,1);
//			Cal_Blob_Info("바깥쪽BURR", Out_Burr,Cam_num);
//			//imshow("Out_Burr",Out_Burr);
//
//
//			// 모든 불량 모은 이진영상
//			Mat Reseult_Binary;
//			add(In_Defect,In_Burr,Reseult_Binary);
//			add(Reseult_Binary,In_Miss,Reseult_Binary);
//			add(Reseult_Binary,Out_Burr,Reseult_Binary);
//			add(Reseult_Binary,Out_Miss,Reseult_Binary);
//
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//
//			findContours( Reseult_Binary, contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours1.size(); k++ )
//			{  
//				drawContours( Dst_Img[Cam_num](boundRect[m_max_object_num]), contours1, k, Scalar(0,0,255), 2, CV_AA, hierarchy1);
//			}
//		} else
//		{
//			cvtColor(Gray_Img[Cam_num],Dst_Img[Cam_num],CV_GRAY2BGR);
//		}
//
//		//QueryPerformanceCounter( &tEnd );
//		//msg.Format("T/T:%1.2f ms", 1000*(tEnd.QuadPart - tStart.QuadPart) / (double)frequency.QuadPart);
//		//int fontFace = FONT_HERSHEY_SIMPLEX;
//		//double fontScale = 0.9;
//		//int thickness = 2;
//
//		//putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(35,35), fontFace, fontScale,
//		//	Scalar(255,0,0), thickness, 8);
//	}
//}
//
//void CImPro_Library::RUN_Algorithm_00()
//{
//	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
//	RUN_Algorithm_H0();
//}
//
//
//void CImPro_Library::Cal_Blob_Info(CString name, Mat &Reseult_Binary, int Cam_num)
//{
//	Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
//	Mat CP_Reseult_Binary = Reseult_Binary.clone();
//	vector<vector<Point> > contours2;
//	vector<Vec4i> hierarchy2;
//
//	dilate(Reseult_Binary,Reseult_Binary,element,Point(-1,-1),1);
//
//	findContours( Reseult_Binary, contours2, hierarchy2, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//	Rect m_boundRect;
//	int blob_cnt = 0;double m_area = 0;
//
//	for( int k = 0; k < contours2.size(); k++ )
//	{  
//		m_area = contourArea(contours2[k],false);
//		m_boundRect = boundingRect(Mat(contours2[k]));
//
//		blob_cnt++;
//		Blob_Info[Cam_num].Format("%s%s_Area_%1.3f_Rect_%d_%d_%d_%d",Blob_Info[Cam_num],name,m_area,m_boundRect.x,m_boundRect.y,m_boundRect.width,m_boundRect.height);
//
//		Blob_Info[Cam_num] += "_";
//	}
//	CP_Reseult_Binary.copyTo(Reseult_Binary);
//}
////
////void CImPro_Library::CensusTransform()
////{
////	if (Gray_Img.empty())
////	{
////		return;
////	}
////	//If (CurrentPixelIntensity<CentrePixelIntensity) boolean bit=0 //else boolean bit=1 //450jx375i  
////	unsigned int census = 0;  
////	unsigned int bit = 0;  
////	int m = 3;  
////	int n = 3;//window size 
////	int i,j,x,y;  
////	int shiftCount = 0;  
////	Binary_Img = Mat::zeros(Size(Gray_Img.cols,Gray_Img.rows), CV_8U); 
////
////	for (x = m/2; x < Gray_Img.rows - m/2; x++)  
////	{    
////		for(y = n/2; y < Gray_Img.cols - n/2; y++)    
////		{      
////			census = 0;      
////			shiftCount = 0;      
////			for (i = x - m/2; i <= x + m/2; i++)      
////			{        
////				for (j = y - n/2; j <= y + n/2; j++)        
////				{            
////					if( shiftCount != m*n/2 )//skip the center pixel          
////					{          
////						census <<= 1;          
////						if( Gray_Img.at<uchar>(i,j) < Gray_Img.at<uchar>(x,y) )//compare pixel values in the neighborhood          
////						{bit = 1;}          
////						else         
////						{bit = 0;}          
////						census = census + bit;          //cout<<census<<" ";*/          
////					}          shiftCount ++;        
////				}      
////			}     //cout<<endl;     
////			//imgTemp.at<uchar>(x,y) = census;
////			Binary_Img.ptr<uchar>(x)[y] = census; 
////		}  
////	}    
////	imshow("CensusTransform", Binary_Img);
////}
////
////
////void CImPro_Library::ModifiedCensusTransform()
////{
////	if (Gray_Img.empty())
////	{
////		return;
////	}
////
////	Size imgSize = Gray_Img.size();  
////	Binary_Img = Mat::zeros(Size(Gray_Img.cols,Gray_Img.rows), CV_8U); 
////	//If (CurrentPixelIntensity<CentrePixelIntensity) boolean bit=0 //else boolean bit=1 //450jx375i  
////	int census = 0;  
////	unsigned int bit = 0;  
////	int m = 3;  
////	int n = 3;//window size int i,j,x,y;  
////	double m_mean = 0;
////	for (int x = m/2; x < imgSize.height - m/2; x++)  
////	{    
////		for(int y = n/2; y < imgSize.width - n/2; y++)    
////		{      
////			census = 0;       
////			m_mean = 0;
////			for (int i = x - m/2; i <= x + m/2; i++)      
////			{        
////				for (int j = y - n/2; j <= y + n/2; j++)        
////				{  
////					m_mean += (double)Gray_Img.at<uchar>(i,j) / 9;
////				}
////			}
////			for (int i = x - m/2; i <= x + m/2; i++)      
////			{        
////				for (int j = y - n/2; j <= y + n/2; j++)        
////				{                  
////					census <<= 1;          
////					if( (double)Gray_Img.at<uchar>(i,j) > m_mean )//compare pixel values in the neighborhood          
////					{bit = 1;}          
////					else         
////					{bit = 0;}          
////					census = census + bit;          //cout<<census<<" ";*/                  
////				}      
////			}     //cout<<endl;     
////			Binary_Img.ptr<uchar>(x)[y] = census/2;    
////		}  
////	}    
////	imshow("ModifiedCensusTransform", Binary_Img);
////}
//

//
//void CImPro_Library::fitting_ellipse(Mat &Binary,double* Circle_Info,int Cam_num)
//{
//	vector<vector<Point> > contours;
//	vector<Vec4i> hierarchy;
//	float m_length = 0;int idx = 0;
//	float m_area = 0;
//	//Mat Tmp_Img = Binary.clone();
//
//	findContours( Binary, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
//	//Binary = Mat::zeros(Binary.size(), CV_8UC1);
//	int max_num = 0; int m_max = 0;
//	if (contours.size() > 1)
//	{
//		vector<vector<Point> > total_contours(1);
//		for (int i=0;i<contours.size();i++)
//		{
//			for (int j=0;j<contours[i].size();j++)
//			{
//				total_contours[0].push_back(contours[i][j]);
//			}
//		}
//		//vector<Rect> boundRect( contours.size() );
//		//int m_max_object_num = -1;int m_max_object_value = 0;
//		//for( int k = 0; k < contours.size(); k++ )
//		//{  
//		//	boundRect[k] = boundingRect( Mat(contours[k]) );
//		//	if (m_max_object_value<= boundRect[k].width*boundRect[k].height)
//		//	{
//		//		m_max_object_value = boundRect[k].width*boundRect[k].height;
//		//		m_max_object_num = k;
//		//	}
//		//}
//		//if (m_max_object_num == -1)
//		//{
//		//	return;
//		//}
//		const int no_data = (int)total_contours[0].size();
//		sPoint *data = new sPoint[no_data];
//
//		if (no_data > 10)
//		{
//			for(int i=0; i<no_data; i++)
//			{
//				data[i].x =  (double)total_contours[0][i].x;
//				data[i].y =  (double)total_contours[0][i].y;
//			}
//
//			double cost = ransac_ellipse_fitting (data, no_data, c_ellipse[Cam_num], 50);
//
//			Circle_Info[0] = c_ellipse[Cam_num].cx;
//			Circle_Info[1] = c_ellipse[Cam_num].cy;
//
//			Circle_Info[2] = c_ellipse[Cam_num].w*2;
//			Circle_Info[3] = c_ellipse[Cam_num].h*2;
//			Circle_Info[4] = (c_ellipse[Cam_num].theta*180/M_PI);
//		}
//		delete [] data;
//	} else
//	{
//		for( size_t k = 0; k < contours.size(); k++ ) 
//		{	
//			const int no_data = (int)contours[max_num].size();
//			sPoint *data = new sPoint[no_data];
//
//			if (no_data > 30)
//			{
//				for(int i=0; i<no_data; i++)
//				{
//					data[i].x =  (double)contours[max_num][i].x;
//					data[i].y =  (double)contours[max_num][i].y;
//				}
//
//				double cost = ransac_ellipse_fitting (data, no_data, c_ellipse[Cam_num], 50);
//
//				Circle_Info[0] = c_ellipse[Cam_num].cx;
//				Circle_Info[1] = c_ellipse[Cam_num].cy;
//
//				Circle_Info[2] = c_ellipse[Cam_num].w*2;
//				Circle_Info[3] = c_ellipse[Cam_num].h*2;
//				Circle_Info[4] = (c_ellipse[Cam_num].theta*180/M_PI);
//			}
//			delete [] data;
//		}
//	}
//}
//
//
//void CImPro_Library::fitting_ellipse2(Mat &Binary,double* Circle_Info,int Cam_num)
//{
//	int n_d = 0;
//	for (int i=0;i<Binary.rows;i++)
//	{
//		for (int j=0;j<Binary.cols;j++)
//		{
//			if (Binary.at<uchar>(i,j) == 255)
//			{
//				n_d ++;
//			}
//		}
//	}
//	const int no_data = n_d;
//	sPoint *data = new sPoint[no_data];
//
//	for (int i=0;i<Binary.rows;i++)
//	{
//		for (int j=0;j<Binary.cols;j++)
//		{
//			if (Binary.at<uchar>(i,j) == 255)
//			{
//				data[i].x =  (double)i;
//				data[i].y =  (double)j;
//			}
//		}
//	}
//
//	double cost = ransac_ellipse_fitting (data, no_data, c_ellipse[Cam_num], 50);
//
//	Circle_Info[0] = c_ellipse[Cam_num].cx;
//	Circle_Info[1] = c_ellipse[Cam_num].cy;
//
//	Circle_Info[2] = c_ellipse[Cam_num].w*2;
//	Circle_Info[3] = c_ellipse[Cam_num].h*2;
//	Circle_Info[4] = (c_ellipse[Cam_num].theta*180/M_PI);
//	delete [] data;
//}
//
//
//
//
//void CImPro_Library::Set_Template_Image(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
//{
//	if (channel==3)
//	{
//		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
//		Src_Img[Cam_num] = src.clone();
//		cvtColor(Src_Img[Cam_num],Template_Img[Cam_num],CV_BGR2GRAY);
//		//Dst_Img[Cam_num] = Gray_Img[Cam_num].clone();
//	} else if (channel == 1)
//	{
//		Mat src(size_y, size_x, CV_8UC1, src_u8);
//		Template_Img[Cam_num] = src.clone();
//	}
//	//imshow("Gray_Img",Gray_Img);
//}
//
//

//
//void CImPro_Library::Set_Side_Image(unsigned char* src_u8, long size_x, long size_y, long channel)
//{
//	if (channel==3)
//	{
//		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
//		Side_Image = Mat(size_y, size_x, CV_8UC1);
//		cvtColor(src,Side_Image,CV_BGR2GRAY);
//		//Dst_Img[Cam_num] = Gray_Img[Cam_num].clone();
//	} else if (channel == 1)
//	{
//		Mat src(size_y, size_x, CV_8UC1, src_u8);
//		Side_Image = src.clone();
//	}
//	//imshow("Gray_Img",Gray_Img);
//}
//
//int CImPro_Library::Delete_Small_Binary(Mat &Binary, int m_small)
//{
//	vector<vector<Point> > contours;
//	vector<Vec4i> hierarchy;
//	float m_length = 0;int idx = 0;
//	float m_area = 0;
//
//	findContours( Binary, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//	//vector<vector<Point> > contours_poly( contours.size() );
//	//vector<Rect> boundRect( contours.size() );
//
//	Binary = Mat::zeros(Binary.size(), CV_8UC1);
//	Scalar color( 255);
//
//	for( size_t k = 0; k < contours.size(); k++ ) 
//	{
//		m_area = contourArea(contours[k],false);
//
//		if ( m_area <= m_small)
//		{
//			contours[k].clear();
//		} else
//		{
//			drawContours( Binary, contours, k, color, CV_FILLED, 8, hierarchy );
//		}
//	}
//	return contours.size();
//}
//
//
//
//int CImPro_Library::Labeling_Delete_Small_Large_Label(Mat &Binary, int m_small)
//{
//	vector<vector<Point> > contours;
//	vector<Vec4i> hierarchy;
//	float m_length = 0;int idx = 0;
//	float m_area = 0;
//
//	findContours( Binary, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//	vector<vector<Point> > contours_poly( contours.size() );
//	vector<Rect> boundRect( contours.size() );
//
//	Binary = Mat::zeros(Binary.size(), CV_8UC1);
//	Scalar color( 255);
//
//	for( size_t k = 0; k < contours.size(); k++ ) 
//	{
//		approxPolyDP( Mat(contours[k]), contours_poly[k], 3, true );
//		boundRect[k] = boundingRect( Mat(contours_poly[k]) );
//
//		int m_max_length = 0;float m_angle = 0;
//		if (boundRect[k].width >= boundRect[k].height)
//		{
//			m_max_length = boundRect[k].width;
//		} else
//		{
//			m_max_length = boundRect[k].height;
//		}
//
//		m_angle = atan2f(boundRect[k].height,boundRect[k].width)*180/CV_PI;
//		//m_length = contours[k].size(); 
//		//m_area = fabs(cv::contourArea( contours[k])); 
//		if ( m_max_length <= m_small)
//		{
//			contours[k].clear();
//		} else
//		{
//			if (m_angle >= 6)
//			{
//				contours[k].clear();
//			}
//			drawContours( Binary, contours, k, color, CV_FILLED, 8, hierarchy );
//		}
//	}
//
//	return contours.size();
//}
//
////
////
////int CImPro_Library::Connect_Line(Mat &Binary)
////{
////	vector<vector<Point> > contours;
////	vector<Vec4i> hierarchy;
////	float m_length = 0;int idx = 0;
////	float m_area = 0;
////
////	findContours( Binary, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
////
////	vector<vector<Point> > contours_poly( contours.size() );
////
////	Binary = Mat::zeros(Binary.size(), CV_8UC1);
////	Scalar color( 255);
////
////	for( size_t k = 0; k < contours.size(); k++ ) 
////	{
////		approxPolyDP( Mat(contours[k]), contours_poly[k], 3, true );
////		drawContours( Binary, contours, k, color, CV_FILLED, 8, hierarchy );
////	}
////
////	for( size_t k = 0; k < contours.size()-1; k++ ) 
////	{
////		int First_line_right_x = contours_poly[k+1][1].x;
////		int First_line_right_y = contours_poly[k+1][1].y;
////		int Second_line_left_x = contours_poly[k][0].x;
////		int Second_line_left_y = contours_poly[k][0].y;
////
////		float m_slope_angle = 0;
////		if (First_line_right_x != Second_line_left_x)
////		{
////			m_slope_angle = atan2((float)First_line_right_y - (float)Second_line_left_y,(float)First_line_right_x - (float)Second_line_left_x)*180/CV_PI;
////		}
////
////		if (First_line_right_x < Second_line_left_x)
////		{
////			if (abs(First_line_right_y-Second_line_left_y) < 20)
////			{
////				if (abs(m_slope_angle) < 20 || (abs(m_slope_angle)<=180 && abs(m_slope_angle)>=160))
////				{
////					line(Binary,contours_poly[k+1][1],contours_poly[k][0],color,1,8 );
////				}
////			}
////		}
////	}
////	return contours.size();
////}
////
////
////
/////**
//// * Code for thinning a binary image using Zhang-Suen algorithm.
//// */
/////**
//// * Perform one thinning iteration.
//// * Normally you wouldn't call this function directly from your code.
//// *
//// * @param  im    Binary image with range = 0-1
//// * @param  iter  0=even, 1=odd
//// */
////void CImPro_Library::thinningIteration(Mat& img, int iter)
////{
////	Mat marker = Mat::zeros(img.size(), CV_8UC1);
////
////
////	int nRows = img.rows;
////	int nCols = img.cols;
////
////
////	if (img.isContinuous()) {
////		nCols *= nRows;
////		nRows = 1;
////	}
////
////
////	int x, y;
////	uchar *pAbove;
////	uchar *pCurr;
////	uchar *pBelow;
////	uchar *nw, *no, *ne;    // north (pAbove)
////	uchar *we, *me, *ea;
////	uchar *sw, *so, *se;    // south (pBelow)
////
////
////	uchar *pDst;
////
////
////	// initialize row pointers
////	pAbove = NULL;
////	pCurr  = img.ptr<uchar>(0);
////	pBelow = img.ptr<uchar>(1);
////
////	int A;int B;int m1;int m2;
////	for (y = 1; y < img.rows-1; ++y) {
////		// shift the rows up by one
////		pAbove = pCurr;
////		pCurr  = pBelow;
////		pBelow = img.ptr<uchar>(y+1);
////
////
////		pDst = marker.ptr<uchar>(y);
////
////
////		// initialize col pointers
////		no = &(pAbove[0]);
////		ne = &(pAbove[1]);
////		me = &(pCurr[0]);
////		ea = &(pCurr[1]);
////		so = &(pBelow[0]);
////		se = &(pBelow[1]);
////
////
////		for (x = 1; x < img.cols-1; ++x) {
////			// shift col pointers left by one (scan left to right)
////			nw = no;
////			no = ne;
////			ne = &(pAbove[x+1]);
////			we = me;
////			me = ea;
////			ea = &(pCurr[x+1]);
////			sw = so;
////			so = se;
////			se = &(pBelow[x+1]);
////
////
////			A  = (*no == 0 && *ne == 1) + (*ne == 0 && *ea == 1) + 
////				(*ea == 0 && *se == 1) + (*se == 0 && *so == 1) + 
////				(*so == 0 && *sw == 1) + (*sw == 0 && *we == 1) +
////				(*we == 0 && *nw == 1) + (*nw == 0 && *no == 1);
////			B  = *no + *ne + *ea + *se + *so + *sw + *we + *nw;
////			m1 = iter == 0 ? (*no * *ea * *so) : (*no * *ea * *we);
////			m2 = iter == 0 ? (*ea * *so * *we) : (*no * *so * *we);
////
////
////			if (A == 1 && (B >= 2 && B <= 6) && m1 == 0 && m2 == 0)
////				pDst[x] = 1;
////		}
////	}
////
////	img &= ~marker;
////}
////
/////**
//// * Function for thinning the given binary image
//// *
//// * @param  im  Binary image with range = 0-255
//// */
////void CImPro_Library::thinning(Mat& im)
////{
////    im /= 255;
////
////    Mat prev = Mat::zeros(im.size(), CV_8UC1);
////    Mat diff;
////
////    do {
////        thinningIteration(im, 0);
////        thinningIteration(im, 1);
////        absdiff(im, prev, diff);
////        im.copyTo(prev);
////    } 
////    while (countNonZero(diff) > 0);
////	//im *= 255;
////}
////
////
////void CImPro_Library::Delete_Boundary(Mat &Img, int boundary_width)
////{
////	for (int ss=0;ss<boundary_width;ss++)
////	{
////		for (int kk=0;kk<Img.rows;kk++)
////		{
////			Img.at<uchar>(kk,ss) = 0;
////			Img.at<uchar>(kk,Img.cols-ss-1) = 0;
////		}
////	}
////	for (int ss=0;ss<Img.cols;ss++)
////	{
////		for (int kk=0;kk<boundary_width;kk++)
////		{
////			Img.at<uchar>(kk,ss) = 0;
////			Img.at<uchar>(Img.rows-kk-1,ss) = 0;
////		}
////	}
////}
////
////void CImPro_Library::Parallel_Thinning(Mat& binary)
////{
////	int divide_num = 8;
////	Rect Roi[8];
////	Mat Tmp_Img[8];
////	int Cut_Width = (int)binary.cols/divide_num;
////
////	omp_set_dynamic(1);
////	omp_set_num_threads(omp_get_max_threads());
////	for (int i=0;i<divide_num;i++)
////	{
////		if (i>0 && i<divide_num-1)
////		{
////			Roi[i].x=-6+i*Cut_Width;
////			Roi[i].y=0;
////			Roi[i].width=12+Cut_Width;
////			Roi[i].height=binary.rows;
////		} else if (i==0)
////		{
////			Roi[i].x=0;
////			Roi[i].y=0;
////			Roi[i].width=5+Cut_Width;
////			Roi[i].height=binary.rows;			
////		} else if (i==divide_num-1)
////		{
////			Roi[i].x=-5+i*Cut_Width;
////			Roi[i].y=0;
////			Roi[i].width=5+Cut_Width;
////			Roi[i].height=binary.rows;			
////		}
////	}
////
////#pragma omp parallel for shared(binary)// starts a new team
////	for (int i=0;i<divide_num;i++)
////	{
////		Tmp_Img[i] = binary(Roi[i]).clone();		
////		thinning(Tmp_Img[i]);
////	}
////
////	Rect T_Roi;
////	for (int i=0;i<divide_num;i++)
////	{
////		if (i>0 && i<divide_num-1)
////		{
////			T_Roi.x = 6;
////			T_Roi.y = 0;
////			T_Roi.width = Roi[i].width - 12;
////			T_Roi.height = Roi[i].height;
////			Roi[i].x=Roi[i].x+6;
////			Roi[i].width=Roi[i].width-12;
////			Tmp_Img[i](T_Roi).copyTo(binary(Roi[i]));
////		} else if (i==0)
////		{
////			T_Roi.x = 0;
////			T_Roi.y = 0;
////			T_Roi.width = Roi[i].width-5;
////			T_Roi.height = Roi[i].height;
////			Roi[i].x=0;
////			Roi[i].width=Cut_Width;
////			Tmp_Img[i](T_Roi).copyTo(binary(Roi[i]));
////		} else if (i==divide_num-1)
////		{
////			T_Roi.x = 5;
////			T_Roi.y = 0;
////			T_Roi.width = Roi[i].width-5;
////			T_Roi.height = Roi[i].height;
////			Roi[i].x=Roi[i].x+5;
////			Roi[i].width=Cut_Width;
////			Tmp_Img[i](T_Roi).copyTo(binary(Roi[i]));
////		}
////	}
////	Delete_Boundary(binary,1);
////	Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
////	dilate(binary,binary,element,Point(-1,-1),1);
////	erode(binary,binary,element,Point(-1,-1),1);
////}
////
////
////#define PIXEL_YX(img,y,x)	img->imageData[(y)*img->widthStep + (x)*img->nChannels*(0xFF&img->depth)/8]
////
////void CImPro_Library::ThinningVornoi(IplImage* inImg, IplImage* outImg)
////{
////	IplImage* dist   = cvCreateImage( cvGetSize(inImg), IPL_DEPTH_32F, 1 );
////	IplImage* edge   = cvCloneImage( inImg );
////	IplImage* labels = cvCreateImage( cvGetSize(inImg), IPL_DEPTH_32S, 1 );
////
////	//cvDistTransform의 labels 변수는 voronoi diagram을 구하기 위해 들어간다.
////	//cvDistTransform의 결과 이미지인 dist에는 voronoi가 아닌 물체간의 거리에 따른 거리 이미지가 들어간다.
////	//labels 변수는 이미지의 결과 값이 아니라 인덱스가 들어가있다.
////	cvDistTransform( edge, dist, CV_DIST_L2, CV_DIST_MASK_3, NULL, labels,CV_DIST_LABEL_CCOMP);
////	cvSaveImage("dist.bmp",dist);cvSaveImage("labels.bmp",labels);
////	cvSetZero (outImg);
////
////	//voronoi diagram을 cvDistTransform함수로 구하기 위해선 labels의 인덱스에 따라 이미지를 변환 해주어야 한다.
////	for(int y = 0; y < labels->height; y++ ) {
////		long prev_p = -1;
////		long *pp = (long *)&PIXEL_YX(labels, y, 0);
////
////		for(int x = 0; x < labels->width; x++, pp++) {
////			if (*pp != prev_p) {
////				prev_p = *pp; 		
////				PIXEL_YX(outImg, y, x) = 255;
////			}
////		}			
////	}
////
////	for(int x = 0; x < labels->width; x++ ) {
////		int prev_p = -1;
////		long *pp = (long *)&PIXEL_YX(labels, 0, x);
////
////		for(int y = 0; y < labels->height; y++, pp += labels->widthStep/4) {
////			if (*pp != prev_p) {
////				prev_p = *pp;
////				PIXEL_YX(outImg, y, x) = 255;
////			}
////		}			
////	}
////
////	cvReleaseImage( &edge );
////	cvReleaseImage( &dist );
////	cvReleaseImage( &labels );
////}
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
////
////
////bool CImPro_Library::J_Image_Transform(const char* filename, const char* m_target_name, int threshold_value, int threshold_method, int target_width, int target_height)
////{
////	Src_Img = imread(filename,0); // 이미지를 읽음.
////	//imshow("fse",Src_Img);
////	if (threshold_method == 0)
////	{
////		copyMakeBorder(Src_Img,Src_Img,60,60,60,60,BORDER_ISOLATED,0);
////	} else
////	{
////		copyMakeBorder(Src_Img,Src_Img,60,60,60,60,BORDER_ISOLATED,255);
////	}
////
////	if (!Src_Img.empty())
////	{
////		resize(Src_Img,Resize_Img,Size(4*((Src_Img.cols/10)/4),4*((Src_Img.rows/10)/4)),0,0,INTER_LINEAR);
////
////		//imwrite("Resize_Img.bmp",Resize_Img);
////		if (threshold_method == 0)
////		{
////			threshold(Resize_Img,Binary_Img,10,255,CV_THRESH_OTSU);	
////		} else
////		{
////			threshold(Resize_Img,Binary_Img,10,255,CV_THRESH_OTSU+CV_THRESH_BINARY_INV);	
////		}
////
////		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
////		dilate(Binary_Img,Binary_Img,element,Point(-1,-1),3);
////		erode(Binary_Img,Binary_Img,element,Point(-1,-1),3);
////
////		Mat holes=Binary_Img.clone();
////		floodFill(holes,Point2i(0,0),Scalar(1));
////		for(int i=0;i<Binary_Img.rows*Binary_Img.cols;i++)
////		{
////			if(holes.data[i]==0)
////				Binary_Img.data[i]=255;
////		}
////
////		Mat CP_Binary_Img = Binary_Img.clone();
////
////		vector<vector<Point> > contours1;
////		vector<Vec4i> hierarchy1;
////
////		findContours( Binary_Img, contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
////
////		vector<Rect> boundRect( contours1.size() );
////		/// Find the convex hull object for each contour
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
////
////		holes=Binary_Img.clone();
////		floodFill(holes,Point2i(0,0),Scalar(1));
////		for(int i=0;i<Binary_Img.rows*Binary_Img.cols;i++)
////		{
////			if(holes.data[i]==0)
////				Binary_Img.data[i]=255;
////		}
////
////		add(CP_Binary_Img,Binary_Img,Binary_Img);
////
////
////		int center_x = boundRect[m_max_object_num].x + boundRect[m_max_object_num].width/2;
////		int center_y = boundRect[m_max_object_num].y + boundRect[m_max_object_num].height/2;
////
////		Point2f ptop;
////		Point2f pbottom;
////		Point2f pleft;
////		Point2f pright;
////
////		ptop.x = center_x;ptop.y = 0;
////		for (int i=center_y;i>=0;i--)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(i,center_x) == 0)
////			{
////				ptop.x = center_x;
////				ptop.y = i;
////				break;
////			}
////		}
////		pbottom.x = center_x;pbottom.y = Resize_Img.rows;
////		for (int i=center_y;i< Resize_Img.rows;i++)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(i,center_x) == 0)
////			{
////				//AfxMessageBox("in");
////				pbottom.x = center_x;
////				pbottom.y = i;
////				break;
////			}
////		}
////
////		pleft.x = 0;pleft.y = center_y;
////		for (int i=center_x;i>= 0;i--)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(center_y,i) == 0)
////			{
////				pleft.x = i;
////				pleft.y = center_y;
////				break;
////			}
////		}
////		pright.x = Resize_Img.cols;pright.y = center_y;
////		for (int i=center_x;i< Resize_Img.cols;i++)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(center_y,i) == 0)
////			{
////				pright.x = i;
////				pright.y = center_y;
////				break;
////			}
////		}
////
////
////		Rect Top_ROI(ptop.x-boundRect[m_max_object_num].width/4, ptop.y - 30, 2*boundRect[m_max_object_num].width/4, 60);
////		if (Top_ROI.y < 0)
////		{
////			Top_ROI.y = 0;
////		}
////		Rect Bottom_ROI(pbottom.x-boundRect[m_max_object_num].width/5, pbottom.y - 30, 2*boundRect[m_max_object_num].width/5, 60);
////		if (Bottom_ROI.y + Bottom_ROI.height >= Resize_Img.rows)
////		{
////			Bottom_ROI.y = Resize_Img.rows-61;
////		}
////		Rect Left_ROI(pleft.x-30, pleft.y - boundRect[m_max_object_num].height/4, 60, 2*boundRect[m_max_object_num].height/4);
////		if (Left_ROI.x < 0)
////		{
////			Left_ROI.x = 0;
////		}
////		Rect Right_ROI(pright.x-30, pright.y - boundRect[m_max_object_num].height/4, 60, 2*boundRect[m_max_object_num].height/4);
////		if (Right_ROI.x + Right_ROI.width >= Resize_Img.cols)
////		{
////			Right_ROI.x = Resize_Img.cols-61;
////		}
////		//CString msg;
////		//msg.Format("%d,%d,  %d,%d,%d,%d      %d",center_x,center_y,Top_ROI.x,Top_ROI.y,Top_ROI.width,Top_ROI.height,Resize_Img.rows);
////		//AfxMessageBox(msg);
////		Mat Top_ROI_Img = Binary_Img(Top_ROI);
////		//imshow("Top",Top_ROI_Img);
////		//cvWaitKey(0);
////		Mat Top_ROI_Edge_Img;
////		erode(Top_ROI_Img,Top_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Top_ROI_Img,Top_ROI_Edge_Img,Top_ROI_Edge_Img);
////		Mat Bottom_ROI_Img = Binary_Img(Bottom_ROI);
////		//imshow("Top",Bottom_ROI_Img);
////		//cvWaitKey(0);
////		Mat Bottom_ROI_Edge_Img;
////		erode(Bottom_ROI_Img,Bottom_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Bottom_ROI_Img,Bottom_ROI_Edge_Img,Bottom_ROI_Edge_Img);
////		Mat Left_ROI_Img = Binary_Img(Left_ROI);
////		//imshow("Top",Left_ROI_Img);
////		//return false;
////		Mat Left_ROI_Edge_Img;
////		erode(Left_ROI_Img,Left_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Left_ROI_Img,Left_ROI_Edge_Img,Left_ROI_Edge_Img);
////		Mat Right_ROI_Img = Binary_Img(Right_ROI);
////		//imshow("Top",Right_ROI_Img);
////		//return false;
////		Mat Right_ROI_Edge_Img;
////		erode(Right_ROI_Img,Right_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Right_ROI_Img,Right_ROI_Edge_Img,Right_ROI_Edge_Img);
////
////		vector<Vec2f> lines;
////		HoughLines(Top_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Top_pt1, Top_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Top_pt1.x = cvRound(x0 + 1000*(-b))+Top_ROI.x;
////			Top_pt1.y = cvRound(y0 + 1000*(a))+Top_ROI.y;
////			Top_pt2.x = cvRound(x0 - 1000*(-b))+Top_ROI.x;
////			Top_pt2.y = cvRound(y0 - 1000*(a))+Top_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Top_pt1, Top_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		lines.clear();
////		HoughLines(Bottom_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Bottom_pt1, Bottom_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Bottom_pt1.x = cvRound(x0 + 1000*(-b))+Bottom_ROI.x;
////			Bottom_pt1.y = cvRound(y0 + 1000*(a))+Bottom_ROI.y;
////			Bottom_pt2.x = cvRound(x0 - 1000*(-b))+Bottom_ROI.x;
////			Bottom_pt2.y = cvRound(y0 - 1000*(a))+Bottom_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Bottom_pt1, Bottom_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		lines.clear();
////		HoughLines(Left_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Left_pt1, Left_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Left_pt1.x = cvRound(x0 + 1000*(-b))+Left_ROI.x;
////			Left_pt1.y = cvRound(y0 + 1000*(a))+Left_ROI.y;
////			Left_pt2.x = cvRound(x0 - 1000*(-b))+Left_ROI.x;
////			Left_pt2.y = cvRound(y0 - 1000*(a))+Left_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Left_pt1, Left_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		lines.clear();
////		HoughLines(Right_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Right_pt1, Right_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Right_pt1.x = cvRound(x0 + 1000*(-b))+Right_ROI.x;
////			Right_pt1.y = cvRound(y0 + 1000*(a))+Right_ROI.y;
////			Right_pt2.x = cvRound(x0 - 1000*(-b))+Right_ROI.x;
////			Right_pt2.y = cvRound(y0 - 1000*(a))+Right_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Right_pt1, Right_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		Point2f inputQuad[4];
////
////		//Bottom_pt1.x = Top_pt1.x;
////		//Bottom_pt1.y = Top_pt1.y + (pbottom.y-ptop.y);
////		//Bottom_pt2.x = Top_pt2.x;
////		//Bottom_pt2.y = Top_pt2.y + (pbottom.y-ptop.y);
////		GetIntersectPoint(Top_pt1, Top_pt2, Left_pt1, Left_pt2, &inputQuad[0]);
////		GetIntersectPoint(Top_pt1, Top_pt2, Right_pt1, Right_pt2, &inputQuad[1]);
////		GetIntersectPoint(Bottom_pt1, Bottom_pt2, Right_pt1, Right_pt2, &inputQuad[2]);
////		GetIntersectPoint(Bottom_pt1, Bottom_pt2, Left_pt1, Left_pt2, &inputQuad[3]);
////
////		if (m_max_object_num == -1)
////		{
////			return false;
////		}
////
////		int m_first_num = 0;
////		for (int i=0;i<4;i++)
////		{
////			inputQuad[i].x = 10*(inputQuad[i].x);
////			inputQuad[i].y = 10*(inputQuad[i].y);
////			if ((inputQuad[0].x+inputQuad[2].x)/2 < inputQuad[i].x && (inputQuad[0].y+inputQuad[2].y)/2 < inputQuad[i].y )
////			{
////				m_first_num = i;
////			}
////		}
////
////		Point2f outputQuad[4];
////		double SIZE_X = target_width-200;
////		double SIZE_Y = target_height-200;
////
////		if (m_first_num == 0)
////		{
////			outputQuad[2] = Point2f( 0,0 );
////			outputQuad[3] = Point2f( SIZE_X,0);
////			outputQuad[0] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[1] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 1)
////		{
////			outputQuad[1] = Point2f( 0,0 );
////			outputQuad[2] = Point2f( SIZE_X,0);
////			outputQuad[3] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[0] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 2)
////		{
////			outputQuad[0] = Point2f( 0,0 );
////			outputQuad[1] = Point2f( SIZE_X,0);
////			outputQuad[2] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[3] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 3)
////		{
////			outputQuad[1] = Point2f( 0,0 );
////			outputQuad[2] = Point2f( SIZE_X,0);
////			outputQuad[3] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[0] = Point2f( 0,SIZE_Y);
////		}
////
////		Transform_Img = Mat::zeros( SIZE_X, SIZE_Y, Src_Img.type() );
////
////		Mat lambda( 2, 4, CV_32FC1 );
////
////		lambda = getPerspectiveTransform( inputQuad, outputQuad );
////		warpPerspective(Src_Img,Transform_Img,lambda,Transform_Img.size() );
////
////		if (threshold_method == 0)
////		{
////			copyMakeBorder(Transform_Img,Transform_Img,100,100,100,100,BORDER_ISOLATED,0);
////		}
////		else if (threshold_method == 1)
////		{
////			copyMakeBorder(Transform_Img,Transform_Img,100,100,100,100,BORDER_ISOLATED,255);
////		}
////		imwrite(m_target_name,Transform_Img);
////
////		return true;
////	} else
////	{
////		return false;
////	}
////}
//
////bool CImPro_Library::J_Image_Transform(const char* filename, const char* m_target_name, int threshold_value, int threshold_method, int target_width, int target_height)
////{
////	Src_Img = imread(filename,0); // 이미지를 읽음.
////	//imshow("fse",Src_Img);
////
////	if (!Src_Img.empty())
////	{
////		resize(Src_Img,Resize_Img,Size(4*((Src_Img.cols/10)/4),4*((Src_Img.rows/10)/4)),0,0,INTER_LINEAR);
////
////		//imwrite("Resize_Img.bmp",Resize_Img);
////		if (threshold_method == 0)
////		{
////			threshold(Resize_Img,Binary_Img,threshold_value,255,CV_THRESH_BINARY);
////		} else
////		{
////			threshold(Resize_Img,Binary_Img,threshold_value,255,CV_THRESH_BINARY_INV);
////		}
////
////		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
////		dilate(Binary_Img,Binary_Img,element,Point(-1,-1),2);
////		erode(Binary_Img,Binary_Img,element,Point(-1,-1),2);
////
////		Mat holes=Binary_Img.clone();
////		floodFill(holes,Point2i(0,0),Scalar(1));
////		for(int i=0;i<Binary_Img.rows*Binary_Img.cols;i++)
////		{
////			if(holes.data[i]==0)
////				Binary_Img.data[i]=255;
////		}
////
////		vector<vector<Point> > contours;
////		vector<Vec4i> hierarchy;
////		float m_length = 0;int idx = 0;
////		float m_area = 0;
////
////		findContours( Binary_Img, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
////
////		//return false;
////
////		vector<vector<Point> > contours_poly( contours.size() );
////		vector<Rect> boundRect( contours.size() );
////		vector<RotatedRect> minRect( contours.size() );
////
////		//Scalar color( 0,255,255);
////		int m_max_object_num = -1;int m_max_object_value = 0;
////		for( size_t k = 0; k < contours.size(); k++ ) 
////		{
////			approxPolyDP( Mat(contours[k]), contours_poly[k], 3, true );
////			boundRect[k] = boundingRect( Mat(contours_poly[k]) );
////			minRect[k] = minAreaRect( Mat(contours_poly[k]) );
////			if (m_max_object_value <= boundRect[k].width && boundRect[k].width >= 100)
////			{
////				m_max_object_value = boundRect[k].width;
////				m_max_object_num = k;
////			}
////		}
////
////		if (m_max_object_num == -1)
////		{
////			return false;
////		}
////
////		Point2f inputQuad[4]; minRect[m_max_object_num].points( inputQuad );
////		int m_first_num = 0;
////		for (int i=0;i<4;i++)
////		{
////			inputQuad[i].x = 10*(inputQuad[i].x - boundRect[m_max_object_num].x);
////			inputQuad[i].y = 10*(inputQuad[i].y - boundRect[m_max_object_num].y);
////			if ((inputQuad[0].x+inputQuad[2].x)/2 < inputQuad[i].x && (inputQuad[0].y+inputQuad[2].y)/2 < inputQuad[i].y )
////			{
////				m_first_num = i;
////			}
////		}
////
////		Point2f outputQuad[4];
////		double SIZE_X = (double)target_width - 200;
////		double SIZE_Y = (double)target_height - 200;
////
////		if (m_first_num == 0)
////		{
////			outputQuad[2] = Point2f( 0,0 );
////			outputQuad[3] = Point2f( SIZE_X,0);
////			outputQuad[0] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[1] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 1)
////		{
////			outputQuad[1] = Point2f( 0,0 );
////			outputQuad[2] = Point2f( SIZE_X,0);
////			outputQuad[3] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[0] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 2)
////		{
////			outputQuad[0] = Point2f( 0,0 );
////			outputQuad[1] = Point2f( SIZE_X,0);
////			outputQuad[2] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[3] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 3)
////		{
////			outputQuad[1] = Point2f( 0,0 );
////			outputQuad[2] = Point2f( SIZE_X,0);
////			outputQuad[3] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[0] = Point2f( 0,SIZE_Y);
////		}
////
////		Rect ROI(10*boundRect[m_max_object_num].x,10*boundRect[m_max_object_num].y,10*boundRect[m_max_object_num].width,10*boundRect[m_max_object_num].height);
////		ROI_Image = Src_Img(ROI);
////
////		Transform_Img = Mat::zeros( SIZE_X, SIZE_Y, Src_Img.type() );
////
////		Mat lambda( 2, 4, CV_32FC1 );
////
////		lambda = getPerspectiveTransform( inputQuad, outputQuad );
////		warpPerspective(ROI_Image,Transform_Img,lambda,Transform_Img.size() );
////		if (threshold_method == 0)
////		{
////			copyMakeBorder(Transform_Img,Transform_Img,100,100,100,100,BORDER_ISOLATED,0);
////		}
////		else if (threshold_method == 1)
////		{
////			copyMakeBorder(Transform_Img,Transform_Img,100,100,100,100,BORDER_ISOLATED,255);
////		}
////		imwrite(m_target_name,Transform_Img);
////
////		return true;
////	} else
////	{
////		return false;
////	}
////}
////
////bool CImPro_Library::intersection(Point2f o1, Point2f p1, Point2f o2, Point2f p2,
////	Point2f &r)
////{
////	Point2f x = o2 - o1;
////	Point2f d1 = p1 - o1;
////	Point2f d2 = p2 - o2;
////
////	float cross = d1.x*d2.y - d1.y*d2.x;
////	if (abs(cross) < /*EPS*/1e-8)
////		return false;
////
////	double t1 = (x.x * d2.y - x.y * d2.x)/cross;
////	r = o1 + d1 * t1;
////	return true;
////}
////
////
////bool CImPro_Library::J_Image_Load(const char* filename)
////{
////	Src_Img = imread(filename,0); // 이미지를 읽음.
////	//imshow("sef",Src_Img);
////
////	copyMakeBorder(Src_Img,Src_Img,60,60,60,60,BORDER_ISOLATED,0);
////
////	resize(Src_Img,Resize_Img,Size(4*((Src_Img.cols/10)/4),4*((Src_Img.rows/10)/4)),0,0,INTER_LINEAR);
////
////	//cvtColor(Resize_Img,Dst_Img,CV_GRAY2BGR);
////	//imshow("sef", Binary_Img);
////
////	if (!Resize_Img.empty())
////	{
////		SizeX_Src = Resize_Img.cols;
////		SizeY_Src = Resize_Img.rows;
////		Ch_Src = Resize_Img.channels();
////		delete [] ImageBuf_Src;
////		ImageBuf_Src = NULL;
////		ImageBuf_Src = new unsigned char[SizeX_Src*SizeY_Src*Ch_Src];
////		memcpy(ImageBuf_Src,Resize_Img.data,sizeof(uchar)*SizeX_Src*SizeY_Src*Ch_Src);
////	} else
////	{
////		return false;
////	}
////
////	return true;
////}
////
////
////bool CImPro_Library::GetIntersectPoint(CvPoint AP1, CvPoint AP2, CvPoint BP1, CvPoint BP2, Point2f* IP) 
////{
////	double t;
////	double s; 
////	double under = (BP2.y-BP1.y)*(AP2.x-AP1.x)-(BP2.x-BP1.x)*(AP2.y-AP1.y);
////
////	if(under==0) return false;
////
////	double _t = (BP2.x-BP1.x)*(AP1.y-BP1.y) - (BP2.y-BP1.y)*(AP1.x-BP1.x);
////	double _s = (AP2.x-AP1.x)*(AP1.y-BP1.y) - (AP2.y-AP1.y)*(AP1.x-BP1.x); 
////
////	t = _t/under;
////	s = _s/under; 
////
////	if(t<0.0 || t>1.0 || s<0.0 || s>1.0) return false;
////
////	if(_t==0 && _s==0) return false; 
////
////	IP->x = AP1.x + t * (double)(AP2.x-AP1.x);
////	IP->y = AP1.y + t * (double)(AP2.y-AP1.y);
////
////	return true;
////}
////
////
////float CImPro_Library::HLineFitting(Mat Binary, double *v_slope, double *v_y)
////{
////	int i,j;
////	float sumx=0,sumy=0,sumxy=0,sumx2=0;
////	vector<float> x;
////	vector<float> y;
////	int ndata = 0;
////
////	for(i=0; i<Binary.cols; i++) 
////	{
////		for(j=0; j<Binary.rows; j++) 
////		{
////			if (Binary.at<uchar>(j,i) > 0)
////			{
////				x.push_back(i);
////				y.push_back(j);
////				ndata++;
////			}
////		}
////	}
////
////	for(i=0;i<ndata;i++)
////	{
////		sumx+=x[i];
////		sumy+=y[i];
////		sumxy+=x[i]*y[i];
////		sumx2+=x[i]*x[i];
////	}
////	*v_y=(sumy*sumx2-sumx*sumxy)/(ndata*sumx2-sumx*sumx);
////	*v_slope=(ndata*sumxy-sumx*sumy)/(ndata*sumx2-sumx*sumx);
////
////	return atanf(*v_slope)*180/CV_PI;
////}
////
////float CImPro_Library::VLineFitting(Mat Binary, double *v_slope, double *v_y)
////{
////	int i,j;
////	float sumx=0,sumy=0,sumxy=0,sumx2=0;
////	vector<float> x;
////	vector<float> y;
////	int ndata = 0;
////
////	for(i=0; i<Binary.cols; i++) 
////	{
////		for(j=0; j<Binary.rows; j++) 
////		{
////			if (Binary.at<uchar>(j,i) > 0)
////			{
////				x.push_back(j);
////				y.push_back(i);
////				ndata++;
////			}
////		}
////	}
////
////	for(i=0;i<ndata;i++)
////	{
////		sumx+=x[i];
////		sumy+=y[i];
////		sumxy+=x[i]*y[i];
////		sumx2+=x[i]*x[i];
////	}
////	*v_y=(sumy*sumx2-sumx*sumxy)/(ndata*sumx2-sumx*sumx);
////	*v_slope=(ndata*sumxy-sumx*sumy)/(ndata*sumx2-sumx*sumx);
////
////	return atanf(*v_slope)*180/CV_PI;
////}
////
////bool CImPro_Library::J_Alg_Test_Run()
////{
////	CString msg;
////	if (!Src_Img.empty())
////	{
////		QueryPerformanceCounter(&tStart ); // 시간체크 시작
////
////		threshold(Resize_Img,Binary_Img,10,255,CV_THRESH_OTSU);	
////
////		Mat element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
////		dilate(Binary_Img,Binary_Img,element,Point(-1,-1),3);
////		erode(Binary_Img,Binary_Img,element,Point(-1,-1),3);
////
////		Mat holes=Binary_Img.clone();
////		floodFill(holes,Point2i(0,0),Scalar(1));
////		for(int i=0;i<Binary_Img.rows*Binary_Img.cols;i++)
////		{
////			if(holes.data[i]==0)
////				Binary_Img.data[i]=255;
////		}
////
////		Mat CP_Binary_Img = Binary_Img.clone();
////
////		vector<vector<Point> > contours1;
////		vector<Vec4i> hierarchy1;
////
////		findContours( Binary_Img, contours1, hierarchy1, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
////
////		vector<Rect> boundRect( contours1.size() );
////		/// Find the convex hull object for each contour
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
////
////		holes=Binary_Img.clone();
////		floodFill(holes,Point2i(0,0),Scalar(1));
////		for(int i=0;i<Binary_Img.rows*Binary_Img.cols;i++)
////		{
////			if(holes.data[i]==0)
////				Binary_Img.data[i]=255;
////		}
////
////		add(CP_Binary_Img,Binary_Img,Binary_Img);
////
////		int center_x = boundRect[m_max_object_num].x + boundRect[m_max_object_num].width/2;
////		int center_y = boundRect[m_max_object_num].y + boundRect[m_max_object_num].height/2;
////
////		Point2f ptop;
////		Point2f pbottom;
////		Point2f pleft;
////		Point2f pright;
////
////		ptop.x = center_x;ptop.y = 0;
////		for (int i=center_y;i>=0;i--)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(i,center_x) == 0)
////			{
////				ptop.x = center_x;
////				ptop.y = i;
////				break;
////			}
////		}
////		pbottom.x = center_x;pbottom.y = Resize_Img.rows;
////		for (int i=center_y;i< Resize_Img.rows;i++)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(i,center_x) == 0)
////			{
////				//AfxMessageBox("in");
////				pbottom.x = center_x;
////				pbottom.y = i;
////				break;
////			}
////		}
////
////		pleft.x = 0;pleft.y = center_y;
////		for (int i=center_x;i>= 0;i--)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(center_y,i) == 0)
////			{
////				pleft.x = i;
////				pleft.y = center_y;
////				break;
////			}
////		}
////		pright.x = Resize_Img.cols;pright.y = center_y;
////		for (int i=center_x;i< Resize_Img.cols;i++)
////		{
////			//AfxMessageBox("sfe");
////			if (Binary_Img.at<uchar>(center_y,i) == 0)
////			{
////				pright.x = i;
////				pright.y = center_y;
////				break;
////			}
////		}
////
////
////		Rect Top_ROI(ptop.x-boundRect[m_max_object_num].width/4, ptop.y - 30, 2*boundRect[m_max_object_num].width/4, 60);
////		if (Top_ROI.y < 0)
////		{
////			Top_ROI.y = 0;
////		}
////		Rect Bottom_ROI(pbottom.x-boundRect[m_max_object_num].width/5, pbottom.y - 30, 2*boundRect[m_max_object_num].width/5, 60);
////		if (Bottom_ROI.y + Bottom_ROI.height >= Resize_Img.rows)
////		{
////			Bottom_ROI.y = Resize_Img.rows-61;
////		}
////		Rect Left_ROI(pleft.x-30, pleft.y - boundRect[m_max_object_num].height/4, 60, 2*boundRect[m_max_object_num].height/4);
////		if (Left_ROI.x < 0)
////		{
////			Left_ROI.x = 0;
////		}
////		Rect Right_ROI(pright.x-30, pright.y - boundRect[m_max_object_num].height/4, 60, 2*boundRect[m_max_object_num].height/4);
////		if (Right_ROI.x + Right_ROI.width >= Resize_Img.cols)
////		{
////			Right_ROI.x = Resize_Img.cols-61;
////		}
////		//CString msg;
////		//msg.Format("%d,%d,  %d,%d,%d,%d      %d",center_x,center_y,Top_ROI.x,Top_ROI.y,Top_ROI.width,Top_ROI.height,Resize_Img.rows);
////		//AfxMessageBox(msg);
////		Mat Top_ROI_Img = Binary_Img(Top_ROI);
////		//imshow("Top",Top_ROI_Img);
////		//return false;
////		Mat Top_ROI_Edge_Img;
////		erode(Top_ROI_Img,Top_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Top_ROI_Img,Top_ROI_Edge_Img,Top_ROI_Edge_Img);
////		Mat Bottom_ROI_Img = Binary_Img(Bottom_ROI);
////		//imshow("Top",Bottom_ROI_Img);
////		//return false;
////		Mat Bottom_ROI_Edge_Img;
////		erode(Bottom_ROI_Img,Bottom_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Bottom_ROI_Img,Bottom_ROI_Edge_Img,Bottom_ROI_Edge_Img);
////		Mat Left_ROI_Img = Binary_Img(Left_ROI);
////		//imshow("Top",Left_ROI_Img);
////		//return false;
////		Mat Left_ROI_Edge_Img;
////		erode(Left_ROI_Img,Left_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Left_ROI_Img,Left_ROI_Edge_Img,Left_ROI_Edge_Img);
////		Mat Right_ROI_Img = Binary_Img(Right_ROI);
////		//imshow("Top",Right_ROI_Img);
////		//return false;
////		Mat Right_ROI_Edge_Img;
////		erode(Right_ROI_Img,Right_ROI_Edge_Img,element,Point(-1,-1),1);
////		subtract(Right_ROI_Img,Right_ROI_Edge_Img,Right_ROI_Edge_Img);
////
////		vector<Vec2f> lines;
////		HoughLines(Top_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Top_pt1, Top_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Top_pt1.x = cvRound(x0 + 1000*(-b))+Top_ROI.x;
////			Top_pt1.y = cvRound(y0 + 1000*(a))+Top_ROI.y;
////			Top_pt2.x = cvRound(x0 - 1000*(-b))+Top_ROI.x;
////			Top_pt2.y = cvRound(y0 - 1000*(a))+Top_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Top_pt1, Top_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		lines.clear();
////		HoughLines(Bottom_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Bottom_pt1, Bottom_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Bottom_pt1.x = cvRound(x0 + 1000*(-b))+Bottom_ROI.x;
////			Bottom_pt1.y = cvRound(y0 + 1000*(a))+Bottom_ROI.y;
////			Bottom_pt2.x = cvRound(x0 - 1000*(-b))+Bottom_ROI.x;
////			Bottom_pt2.y = cvRound(y0 - 1000*(a))+Bottom_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Bottom_pt1, Bottom_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		lines.clear();
////		HoughLines(Left_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Left_pt1, Left_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Left_pt1.x = cvRound(x0 + 1000*(-b))+Left_ROI.x;
////			Left_pt1.y = cvRound(y0 + 1000*(a))+Left_ROI.y;
////			Left_pt2.x = cvRound(x0 - 1000*(-b))+Left_ROI.x;
////			Left_pt2.y = cvRound(y0 - 1000*(a))+Left_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Left_pt1, Left_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		lines.clear();
////		HoughLines(Right_ROI_Edge_Img, lines, 1, CV_PI/180, 50, 0, 0 );
////		Point Right_pt1, Right_pt2;
////		for( size_t i = 0; i < 1; i++ )
////		{
////			float rho = lines[i][0], theta = lines[i][1];
////			double a = cos(theta), b = sin(theta);
////			double x0 = a*rho, y0 = b*rho;
////			Right_pt1.x = cvRound(x0 + 1000*(-b))+Right_ROI.x;
////			Right_pt1.y = cvRound(y0 + 1000*(a))+Right_ROI.y;
////			Right_pt2.x = cvRound(x0 - 1000*(-b))+Right_ROI.x;
////			Right_pt2.y = cvRound(y0 - 1000*(a))+Right_ROI.y;
////			//line( Top_ROI_Edge_Img, pt1, pt2, Scalar(255), 1, CV_AA);
////			//line( Resize_Img, Right_pt1, Right_pt2, Scalar(255), 1, CV_AA);
////		}
////
////
////		Point2f inputQuad[4];
////		//Bottom_pt1.x = Top_pt1.x;
////		//Bottom_pt1.y = Top_pt1.y + (pbottom.y-ptop.y);
////		//Bottom_pt2.x = Top_pt2.x;
////		//Bottom_pt2.y = Top_pt2.y + (pbottom.y-ptop.y);
////		GetIntersectPoint(Top_pt1, Top_pt2, Left_pt1, Left_pt2, &inputQuad[0]);
////		GetIntersectPoint(Top_pt1, Top_pt2, Right_pt1, Right_pt2, &inputQuad[1]);
////		GetIntersectPoint(Bottom_pt1, Bottom_pt2, Right_pt1, Right_pt2, &inputQuad[2]);
////		GetIntersectPoint(Bottom_pt1, Bottom_pt2, Left_pt1, Left_pt2, &inputQuad[3]);
////
////		if (m_max_object_num == -1)
////		{
////			return false;
////		}
////
////		int m_first_num = 0;
////		for (int i=0;i<4;i++)
////		{
////			inputQuad[i].x = 10*(inputQuad[i].x);
////			inputQuad[i].y = 10*(inputQuad[i].y);
////			if ((inputQuad[0].x+inputQuad[2].x)/2 < inputQuad[i].x && (inputQuad[0].y+inputQuad[2].y)/2 < inputQuad[i].y )
////			{
////				m_first_num = i;
////			}
////		}
////
////		Point2f outputQuad[4];
////		double SIZE_X = 3000-200;
////		double SIZE_Y = 3000-200;
////
////		if (m_first_num == 0)
////		{
////			outputQuad[2] = Point2f( 0,0 );
////			outputQuad[3] = Point2f( SIZE_X,0);
////			outputQuad[0] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[1] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 1)
////		{
////			outputQuad[1] = Point2f( 0,0 );
////			outputQuad[2] = Point2f( SIZE_X,0);
////			outputQuad[3] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[0] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 2)
////		{
////			outputQuad[0] = Point2f( 0,0 );
////			outputQuad[1] = Point2f( SIZE_X,0);
////			outputQuad[2] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[3] = Point2f( 0,SIZE_Y);
////		} else if (m_first_num == 3)
////		{
////			outputQuad[1] = Point2f( 0,0 );
////			outputQuad[2] = Point2f( SIZE_X,0);
////			outputQuad[3] = Point2f( SIZE_X,SIZE_Y);
////			outputQuad[0] = Point2f( 0,SIZE_Y);
////		}
////
////		Transform_Img = Mat::zeros( SIZE_X, SIZE_Y, Src_Img.type() );
////
////		Mat lambda( 2, 4, CV_32FC1 );
////
////		lambda = getPerspectiveTransform( inputQuad, outputQuad );
////		warpPerspective(Src_Img,Transform_Img,lambda,Transform_Img.size() );
////
////		copyMakeBorder(Transform_Img,Transform_Img,100,100,100,100,BORDER_ISOLATED,0);
////
////		QueryPerformanceCounter( &tEnd );
////
////		cvtColor(Transform_Img,Dst_Img,CV_GRAY2BGR);
////
////		imwrite("Transformed_Image.jpg",Transform_Img);
////		////imwrite("Rotate_Image.bmp",Rotate_Image);
////
////		//cvtColor(Rotate_Image,Dst_Img,CV_GRAY2BGR);
////
////		//msg.Format("(%d x %d)", ROI_Image.rows, ROI_Image.cols);
////
////		Point center;
////		center.x = 200;
////		center.y = 200;
////		putText(Dst_Img, msg.GetBuffer(), center, CV_FONT_HERSHEY_SIMPLEX, 3.8, Scalar(0, 255, 0), 7, 1);
////
////		msg.Format("T/T:%1.2fms", 1000*(tEnd.QuadPart - tStart.QuadPart) / (double)frequency.QuadPart);
////		center.y = 350;
////		putText(Dst_Img, msg.GetBuffer(), center, CV_FONT_HERSHEY_SIMPLEX, 3.8, Scalar(0, 255, 0), 7, 1);
////
////
////
////
////		SizeX_Src = Dst_Img.cols;
////		SizeY_Src = Dst_Img.rows;
////		Ch_Src = Dst_Img.channels();
////		delete [] ImageBuf_Src;
////		ImageBuf_Src = NULL;
////		ImageBuf_Src = new unsigned char[SizeX_Src*SizeY_Src*Ch_Src];
////		memcpy(ImageBuf_Src,Dst_Img.data,sizeof(uchar)*SizeX_Src*SizeY_Src*Ch_Src);
////		return true;
////	} else
////	{
////		return false;
////	}
////}
////
//////void static Parallel_Threshold_CDJung(Mat Img, Mat& Binary_Img, int Slice_width, int Slice_height)
//////{
//////	// Img를 Slice_width*Slice_height 크기로 나누어서 임계화를 함. 
//////	// OMP를 사용하여 시간 단축.
//////	// col:x좌표, row:y좌표
//////
//////	int m_col = (int)(Img.cols/Slice_width);bool m_col_flag = false;
//////	int m_row = (int)(Img.rows/Slice_height);bool m_row_flag = false;
//////
//////	if (Img.cols - m_col*Slice_width > 0)
//////	{
//////		m_col++;
//////		m_col_flag = true;
//////	}
//////	if (Img.rows - m_row*Slice_height > 0 )
//////	{
//////		m_row++;
//////		m_row_flag = true;
//////	}
//////	int divide_num = m_col*m_row;
//////
//////	Rect *Roi = (Rect *)calloc(divide_num,sizeof(Rect));
//////	CString str;
//////	for (int yy=0;yy<m_row;yy++)
//////	{
//////		for (int xx=0;xx<m_col;xx++)
//////		{
//////			Roi[xx+m_row*yy].x = xx*Slice_width;
//////			Roi[xx+m_row*yy].y = yy*Slice_height;
//////			Roi[xx+m_row*yy].width = Slice_width;
//////			Roi[xx+m_row*yy].height = Slice_height;
//////			str.Format("%d,%d,%d,%d",Roi[xx+m_row*yy].x,Roi[xx+m_row*yy].y,Roi[xx+m_row*yy].width,Roi[xx+m_row*yy].height);
//////			//AfxMessageBox(str);
//////			if (m_col_flag && xx==m_col-1)
//////			{
//////				Roi[xx+m_row*yy].width = Img.cols - (m_col-1)*Slice_width;
//////			}
//////			if (m_row_flag && yy==m_row-1)
//////			{
//////				Roi[xx+m_row*yy].height = Img.rows - (m_row-1)*Slice_height;
//////			}
//////		}
//////	}
//////
//////	omp_set_dynamic(1);
//////	omp_set_num_threads(omp_get_max_threads());
//////	#pragma omp parallel for shared(Img,Binary_Img,Roi)// starts a new team
//////	for (int i=0;i<divide_num;i++)
//////	{
//////		threshold(Img(Roi[i]),Binary_Img(Roi[i]),30,255,CV_THRESH_BINARY);
//////		//int m_thres = Determine_Threshold_Value(Binary_Img(Roi[i]), Img(Roi[i]));
//////		//threshold(Img(Roi[i]),Binary_Img(Roi[i]),m_thres,255,CV_THRESH_BINARY_INV);
//////	}
//////}
////
//////void CImPro_Library::Display_Image(HWND UserWindowHandle)
//////{
//////   MIL_ID MilApplication,       /* Application identifier.  */
//////          MilSystem,            /* System identifier.       */
//////          MilDisplay,           /* Display identifier.      */
//////          MilImage,             /* Image buffer identifier. */
//////          MilLeftSubImage,      /* Sub-image buffer identifier for original image. */
//////          MilRightSubImage,     /* Sub-image buffer identifier for processed image.*/
//////          MilLumSubImage=0,     /* Sub-image buffer identifier for luminance.      */
//////          MilRedBandSubImage,   /* Sub-image buffer identifier for red component.  */
//////          MilGreenBandSubImage, /* Sub-image buffer identifier for green component.*/
//////          MilBlueBandSubImage;  /* Sub-image buffer identifier for blue component. */
//////
//////
//////   /* Allocate defaults. */
//////   MappAllocDefault(M_SETUP, &MilApplication, &MilSystem, &MilDisplay, M_NULL, M_NULL);
//////
//////   /* Allocate a color display buffer twice the size of the source image and display it. */
//////   MbufAllocColor(MilSystem,
//////                  MbufDiskInquire(IMAGE_FILE, M_SIZE_BAND, &SizeBand),
//////                  MbufDiskInquire(IMAGE_FILE, M_SIZE_X, &SizeX) * 2,
//////                  MbufDiskInquire(IMAGE_FILE, M_SIZE_Y, &SizeY),
//////                  MbufDiskInquire(IMAGE_FILE, M_TYPE,   &Type),
//////                  M_IMAGE+M_DISP+M_PROC, &MilImage);
//////   MbufClear(MilImage, 0L);
//////  // MdispSelect(MilDisplay, MilImage);
//////
//////   /* Select the MIL buffer to be displayed in the user-specified window. */
//////   //MdispSelectWindow(MilDisplay, MilImage, UserWindowHandle);
//////   /* Define 2 child buffers that maps to the left and right part of the display 
//////      buffer, to put the source and destination color images.
//////    */
//////   MbufChild2d(MilImage, 0L, 0L, SizeX, SizeY, &MilLeftSubImage);
//////   MbufChild2d(MilImage, SizeX, 0L, SizeX, SizeY, &MilRightSubImage);
//////
//////   /* Load the color source image on the left. */
//////   MbufLoad(IMAGE_FILE, MilLeftSubImage);
//////
//////   // 복사하기
//////   long iPitch, iHeigth, iWidth;
//////   MbufInquire(MilLeftSubImage, M_PITCH, &iPitch);
//////   MbufInquire(MilLeftSubImage, M_SIZE_Y, &iHeigth);
//////   MbufInquire(MilLeftSubImage, M_SIZE_X, &iWidth);
//////   m_ucImageBuf = new unsigned char[iWidth*iHeigth*iPitch];
//////
//////   if (SizeBand==3)
//////   {
//////	   MbufGetColor(MilLeftSubImage, M_PACKED+M_BGR24   , M_ALL_BANDS, m_ucImageBuf);
//////   } else
//////   {
//////	   MbufGetColor(MilLeftSubImage, M_PACKED+M_GRAYSCALE   , M_ALL_BANDS, m_ucImageBuf);
//////   }
//////
//////   
//////
//////   //m_ucImageBuf = (unsigned char*)GlobalAlloc(GMEM_ZEROINIT, iPitch*iHeigth);
//////   //MbufInquire(MilLeftSubImage, M_HOST_ADDRESS, &m_ucImageBuf);
//////   
//////   return;
//////   /* Define child buffers that map to the red, green and blue components
//////      of the source image.
//////    */
//////   MbufChildColor(MilLeftSubImage, M_RED,   &MilRedBandSubImage);
//////   MbufChildColor(MilLeftSubImage, M_GREEN, &MilGreenBandSubImage);
//////   MbufChildColor(MilLeftSubImage, M_BLUE,  &MilBlueBandSubImage);
//////
//////   /* Write color text annotations to show access in each individual band of the image.
//////   
//////      Note that this is typically simplified by using:
//////      MgraColor(M_DEFAULT, M_RGB(0xFF,0x90,0x00));
//////      MgraText(M_DEFAULT, MilLeftSubImage, ...);
//////    */
//////   MgraColor(M_DEFAULT, 0xFF);
//////   MgraText(M_DEFAULT, MilRedBandSubImage,   SizeX/16, SizeY/8, MIL_TEXT(" TOUCAN "));
//////   MgraColor(M_DEFAULT, 0x90);
//////   MgraText(M_DEFAULT, MilGreenBandSubImage, SizeX/16, SizeY/8, MIL_TEXT(" TOUCAN "));
//////   MgraColor(M_DEFAULT, 0x00);
//////   MgraText(M_DEFAULT, MilBlueBandSubImage,  SizeX/16, SizeY/8, MIL_TEXT(" TOUCAN "));
//////
//////   return;
//////   /*  If Image Processing module is available, augment the image luminance. */
//////   #if (!M_MIL_LITE)
//////      {
//////      /* Convert image to Hue, Luminance, Saturation color space (HLS). */
//////      MimConvert(MilLeftSubImage, MilRightSubImage, M_RGB_TO_HLS);
//////   
//////      /* Create a child buffer that maps to the luminance component. */
//////      MbufChildColor(MilRightSubImage, M_LUMINANCE, &MilLumSubImage);
//////        
//////      /* Add an offset to the luminance component. */
//////      MimArith(MilLumSubImage, IMAGE_LUMINANCE_OFFSET, MilLumSubImage, M_ADD_CONST+M_SATURATION);
//////   
//////      /* Convert image back to Red, Green, Blue color space (RGB) for display. */
//////      MimConvert(MilRightSubImage, MilRightSubImage, M_HLS_TO_RGB); 
//////
//////      /* Free the luminance band child buffer. */
//////     // MbufFree(MilLumSubImage);
//////
//////      /* Print a message. */
//////      //printf("Luminance was increased using color image processing.\n");
//////      }
//////   #else
//////      {
//////      /* Copy the color source image. */
//////      MbufCopy(MilLeftSubImage, MilRightSubImage);
//////
//////      /* Print a message. */
//////     // printf("The image was copied to the right.\n");
//////      }
//////   #endif
//////
//////	  //Sleep(5000);
//////		  
//////	  MbufFree(MilRedBandSubImage);
//////	  MbufFree(MilGreenBandSubImage);
//////	  MbufFree(MilBlueBandSubImage);
//////	  MbufFree(MilRightSubImage);
//////	  MbufFree(MilLeftSubImage);
//////	  MbufFree(MilImage);
//////
//////	  MappFreeDefault(MilApplication, MilSystem, MilDisplay, M_NULL, M_NULL);
//////}
#pragma endregion