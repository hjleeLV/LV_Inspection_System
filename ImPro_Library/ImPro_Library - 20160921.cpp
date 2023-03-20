#include "StdAfx.h"
#include "ImPro_Library.h"
#include "math.h"
#define PI 3.14159265

struct compare_Size {
	bool operator() (Point4D a, Point4D b) { return (a.CX  < b.CX);}
} Point_compare_Size;

struct compare_Increasing {
	bool operator() (double a, double b) { return (a  < b);}
} Point_compare_Increasing;

struct compare_Increasing_2d {
	bool operator() (Data2D a, Data2D b) { return (a.s  > b.s);}
} Point_compare_Increasing_2d;



CImPro_Library::CImPro_Library(void)
{
	m_Alg_Type = 0;
	m_License_Check = 1;
	element = getStructuringElement(MORPH_RECT, Size(3, 3), Point(-1, -1) );
	element_h = getStructuringElement(MORPH_RECT, Size(3, 1), Point(-1, -1) );
	element_v = getStructuringElement(MORPH_RECT, Size(1, 3), Point(-1, -1) );
	fontFace = FONT_HERSHEY_SIMPLEX;
	fontScale = 0.45;
	thickness = 1;
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
		return false;
	}
	// verify normal user password
	res1 = LC_passwd(handle, 1, (unsigned char *) "33577748");  
	if(res1) {
		LC_close(handle);
		AfxMessageBox("There is no dongle license key! Please buy the S/W license!");		
		return false;
	}
	LC_close(handle);
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

void CImPro_Library::Set_Image_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	//Rect m_ROI;
	//m_ROI.x = 0;m_ROI.y = 0;
	//m_ROI.width = (int)(4*(size_x/4));
	//m_ROI.height = (int)(4*(size_y/4));

	//ALG_BOLTPIN_Param.nRotationAngle = 10;

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		//Src_Img[Cam_num] = src.clone();
		//if (abs(ALG_BOLTPIN_Param.nRotationAngle) > 0)
		//{
		//	J_Rotate(src,ALG_BOLTPIN_Param.nRotationAngle,src);
		//}
		cvtColor(src,Gray_Img[Cam_num],CV_BGR2GRAY);
		Dst_Img[Cam_num] = src.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		//if (abs(ALG_BOLTPIN_Param.nRotationAngle) > 0)
		//{
		//	J_Rotate(src,ALG_BOLTPIN_Param.nRotationAngle,src);
		//}
		Gray_Img[Cam_num] = src.clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_Img[Cam_num] = Mat::zeros(Gray_Img[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_Img[Cam_num],CV_GRAY2BGR);
	}
	//imshow("Gray_Img",Gray_Img[Cam_num]);
}


void CImPro_Library::Set_MissedImage_0(unsigned char* src_u8, long size_x, long size_y, long channel, int Cam_num)
{
	//Rect m_ROI;
	//m_ROI.x = 0;m_ROI.y = 0;
	//m_ROI.width = (int)(4*(size_x/4));
	//m_ROI.height = (int)(4*(size_y/4));

	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		//Src_Img[Cam_num] = src.clone();
		cvtColor(src,Gray_MissedImg[Cam_num],CV_BGR2GRAY);
		Dst_MissedImg[Cam_num] = src.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Gray_MissedImg[Cam_num] = src.clone();
		//Src_Img[Cam_num].copyTo(Gray_Img[Cam_num]);
		Dst_MissedImg[Cam_num] = Mat::zeros(Gray_MissedImg[Cam_num].size(), CV_8UC3);
		cvtColor(src,Dst_MissedImg[Cam_num],CV_GRAY2BGR);
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
void CImPro_Library::J_Rotate(cv::Mat& src, double angle, cv::Mat& dst)
{
    //int len = std::max(src.cols, src.rows);
    //cv::Point2f pt(src.cols/2., src.rows/2.);
	cv::Point2f pt(ALG_BOLTPIN_Param.nRect[0].x, ALG_BOLTPIN_Param.nRect[0].y+ALG_BOLTPIN_Param.nRect[0].height);
    cv::Mat r = cv::getRotationMatrix2D(pt, angle, 1.0);
	if (src.channels() == 1)
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,0,CV_RGB(0,0,0));
	} else
	{
		cv::warpAffine(src, dst, r, cv::Size(src.cols, src.rows),1,0,CV_RGB(255,255,255));
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

#pragma endregion

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//bool CImPro_Library::RUN_Algorithm_CAM0()
//{
//	bool RTN = true;
//	if (m_Alg_Type == 0)
//	{
//		RTN = RUN_ALG_TYPE0_CAM0();
//	} 
//	else if (m_Alg_Type == 1)
//	{
//		RTN = RUN_ALG_TYPE0_CAM0();
//	}
//	else if (m_Alg_Type == 2)
//	{
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_Algorithm_CAM1()
//{
//	bool RTN = true;
//	if (m_Alg_Type == 0)
//	{
//		RTN = RUN_ALG_TYPE0_CAM1();
//	} 
//	else if (m_Alg_Type == 1)
//	{
//		RTN = RUN_ALG_TYPE1_CAM1();
//	}
//	else if (m_Alg_Type == 2)
//	{
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_Algorithm_CAM2()
//{
//	bool RTN = true;
//	if (m_Alg_Type == 0)
//	{
//		RTN = RUN_ALG_TYPE0_CAM2();
//	} 
//	else if (m_Alg_Type == 1)
//	{
//		RTN = RUN_Algorithm_Side(2);
//	}
//	else if (m_Alg_Type == 2)
//	{
//	}
//	return RTN;
//}
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//bool CImPro_Library::RUN_ALG_TYPE0_CAM0()
//{
//	int Cam_num = 0;
//
//	if (!Gray_Img[Cam_num].empty()) //이미지가 있으면
//	{
//		vector<vector<Point> > contours;
//		vector<Vec4i> hierarchy;
//		CString msg;
//		Result_Info[Cam_num] = "";
//		double Circle_Info[5] = {0,}; // 외경 : 중심X,Y,가로,세로,각도
//		double In_Circle_Info[5] = {0,}; // 내경 : 중심X,Y,가로,세로,각도
//		int Defect_size = 0;
//		for (int s=0;s<5;s++)
//		{
//			if (TYPE0_CAM0_Param.nRect_Use[s] == 0 || TYPE0_CAM0_Param.nRect[s].width <= 0 || TYPE0_CAM0_Param.nRect[s].height <= 0)
//			{
//				continue;
//			}
//
//			// ROI 이미지 잘라오기
//			Mat ROI_Gray_Img = Gray_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]).clone();	
//
//			if (Result_Debugging) // ROI 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),ROI_Gray_Img);		
//			}
//
//			if (m_Text_View[Cam_num]) // ROI 영역 표시
//			{
//				rectangle(Dst_Img[Cam_num],TYPE0_CAM0_Param.nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM0_Param.nRect[s].x+8,TYPE0_CAM0_Param.nRect[s].y + 23), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(0,0,255), 1, 8);
//			}
//
//			// 오링 임계화
//			Mat Thres_ROI_Gray_Img;	
//			threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM0_Param.nHeadThreshold,255,CV_THRESH_BINARY_INV);
//			if (Result_Debugging) // 오링 임계화 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Thres_ROI_Gray_Img);		
//			}
//
//			// 오링 채운 이미지
//			Mat Filled_Thres_ROI_Gray_Img = Thres_ROI_Gray_Img.clone();
//			dilate(Filled_Thres_ROI_Gray_Img,Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//			erode(Filled_Thres_ROI_Gray_Img,Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//
//			J_Delete_Boundary(Filled_Thres_ROI_Gray_Img,1);
//			J_Fill_Hole(Filled_Thres_ROI_Gray_Img);
//			if (Result_Debugging) // 오링 채운 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Filled_Thres_ROI_Gray_Img);		
//			}
//
//			// 오링만 찾기
//			findContours( Filled_Thres_ROI_Gray_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			if(contours.size() == 0) // 칸투어 갯수로 예외처리
//			{
//				Result_Info[Cam_num].Format("C%d:00_%d", Cam_num, -1);
//				rectangle(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Rect(10,10,TYPE0_CAM0_Param.nRect[s].width-20,TYPE0_CAM0_Param.nRect[s].height-20),Scalar(0,0,255),2);
//				msg.Format("Not Exist O-Ring!");
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM0_Param.nRect[s].x + TYPE0_CAM0_Param.nRect[s].width/2 - 150,TYPE0_CAM0_Param.nRect[s].y+TYPE0_CAM0_Param.nRect[s].height/2), FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0,0,255), 2, 8);
//				return true;
//			}
//
//			vector<Rect> boundRect( contours.size() );
//			int m_max_object_num = -1;int m_max_object_value = 0;
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				boundRect[k] = boundingRect( Mat(contours[k]) );
//				if (m_max_object_value<= boundRect[k].width*boundRect[k].height 
//					&& boundRect[k].x > 1 && boundRect[k].y > 1
//					&& boundRect[k].x + boundRect[k].width < Thres_ROI_Gray_Img.cols-1
//					&& boundRect[k].y + boundRect[k].height < Thres_ROI_Gray_Img.rows-1)
//				{
//					m_max_object_value = boundRect[k].width*boundRect[k].height;
//					m_max_object_num = k;
//				}
//			}
//
//			// 오링를 못 찾을 경우 에러 처리함.
//			if(m_max_object_num == -1)
//			{
//				Result_Info[Cam_num].Format("C%d:00_%d", Cam_num, -1);
//				rectangle(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Rect(10,10,TYPE0_CAM0_Param.nRect[s].width-20,TYPE0_CAM0_Param.nRect[s].height-20),Scalar(0,0,255),2);
//				msg.Format("Not Exist O-Ring!");
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM0_Param.nRect[s].x + TYPE0_CAM0_Param.nRect[s].width/2 - 150,TYPE0_CAM0_Param.nRect[s].y+TYPE0_CAM0_Param.nRect[s].height/2), FONT_HERSHEY_SIMPLEX, 1, CV_RGB(0,0,255), 2, 8);
//				return true;
//			}
//
//			Mat Oring_Filled_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			Mat Ellipse_Oring_Filled_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//
//
////#pragma omp parallel num_threads(2)
////			{
////#pragma omp sections // divides the team into sections
////				{ 
////					// everything herein is run only once.
////#pragma omp section
//					{ 
//						// 오링만 다시 그림
//						drawContours( Oring_Filled_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
//						if (Result_Debugging) // 오링 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),Oring_Filled_Thres_ROI_Gray_Img);		
//						}
//
//						// 오링 타원 핏팅
//						J_Fitting_Ellipse(Oring_Filled_Thres_ROI_Gray_Img.clone(), Circle_Info);
//
//						// 오링 타원 결과 그림
//						ellipse(Ellipse_Oring_Filled_Thres_ROI_Gray_Img,Size(Circle_Info[0],Circle_Info[1]),Size(Circle_Info[2]/2,Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//						if (Result_Debugging) // 오링 타원 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),Ellipse_Oring_Filled_Thres_ROI_Gray_Img);		
//						}
//						if (m_Text_View[Cam_num]) // 타원 표시
//						{
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(Circle_Info[0],Circle_Info[1]),Size(Circle_Info[2]/2,Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(0,255,0),1,CV_AA,0);
//						}
//					}
////#pragma omp section
//					{
//						// 오링 내경 찾기
//
//						Mat Inner_Oring_Filled_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//						drawContours( Inner_Oring_Filled_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
//						subtract(Inner_Oring_Filled_Thres_ROI_Gray_Img, Thres_ROI_Gray_Img, Inner_Oring_Filled_Thres_ROI_Gray_Img);
//						erode(Inner_Oring_Filled_Thres_ROI_Gray_Img,Inner_Oring_Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),20);
//						dilate(Inner_Oring_Filled_Thres_ROI_Gray_Img,Inner_Oring_Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),20);
//
//						if (Result_Debugging) // 오링 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_Inner_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),Inner_Oring_Filled_Thres_ROI_Gray_Img);		
//						}
//						// 오링 타원 핏팅
//						J_Fitting_Ellipse(Inner_Oring_Filled_Thres_ROI_Gray_Img.clone(), In_Circle_Info);
//					}
//			//	}
//			//}
//
//			Mat R1_Img = Ellipse_Oring_Filled_Thres_ROI_Gray_Img.clone();
//			Mat R2_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			Mat R3_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			Mat R4_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			Mat Polar_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			Mat Inv_Polar_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//
//#pragma omp parallel num_threads(3)
//			{
//#pragma omp sections // divides the team into sections
//				{ 
//#pragma omp section
//					{
//						// 1번 영역 따오기
//						threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM0_Param.nRegion1Threshold,255,CV_THRESH_BINARY_INV);
//						ellipse(R1_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion1Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion1Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//						bitwise_and(Thres_ROI_Gray_Img,R1_Img,R1_Img);
//						dilate(R1_Img,R1_Img,element,Point(-1,-1),2);
//						erode(R1_Img,R1_Img,element,Point(-1,-1),3);
//						dilate(R1_Img,R1_Img,element,Point(-1,-1),1);
//
//						if (Result_Debugging) // 오링 1번 영역 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_R1_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),R1_Img);		
//						}
//						if (m_Text_View[Cam_num]) // 타원 표시
//						{
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion1Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion1Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,100,0),1,CV_AA,0);
//						}
//						// 2번 영역 따오기
//						threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM0_Param.nRegion2Threshold,255,CV_THRESH_BINARY_INV);
//						ellipse(R2_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion1Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion1Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//						ellipse(R2_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion2Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion2Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//						bitwise_and(Thres_ROI_Gray_Img,R2_Img,R2_Img);
//						dilate(R2_Img,R2_Img,element,Point(-1,-1),2);
//						erode(R2_Img,R2_Img,element,Point(-1,-1),3);
//						dilate(R2_Img,R2_Img,element,Point(-1,-1),1);
//
//						if (Result_Debugging) // 오링 2번 영역 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_R2_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),R2_Img);		
//						}
//						if (m_Text_View[Cam_num]) // 타원 표시
//						{
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion2Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion2Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,100,0),1,CV_AA,0);
//						}
//						add(R1_Img,R2_Img,R2_Img);
//					}
//#pragma omp section
//					{
//
//						// 3번 영역 따오기
//						threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM0_Param.nRegion3Threshold,255,CV_THRESH_BINARY_INV);
//						ellipse(R3_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion2Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion2Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//						ellipse(R3_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion3Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion3Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//						bitwise_and(Thres_ROI_Gray_Img,R3_Img,R3_Img);
//						dilate(R3_Img,R3_Img,element,Point(-1,-1),2);
//						erode(R3_Img,R3_Img,element,Point(-1,-1),3);
//						dilate(R3_Img,R3_Img,element,Point(-1,-1),1);
//
//						if (Result_Debugging) // 오링 3번 영역 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_R3_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),R3_Img);		
//						}
//						if (m_Text_View[Cam_num]) // 타원 표시
//						{
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion3Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion3Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,100,0),1,CV_AA,0);
//						}
//						// 4번 영역 따오기
//						threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM0_Param.nRegion4Threshold,255,CV_THRESH_BINARY_INV);
//						ellipse(R4_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion3Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion3Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//						ellipse(R4_Img,Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion4Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion4Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//						bitwise_and(Thres_ROI_Gray_Img,R4_Img,R4_Img);
//						dilate(R4_Img,R4_Img,element,Point(-1,-1),2);
//						erode(R4_Img,R4_Img,element,Point(-1,-1),3);
//						dilate(R4_Img,R4_Img,element,Point(-1,-1),1);
//
//						if (Result_Debugging) // 오링 3번 영역 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_R4_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),R4_Img);		
//						}
//						if (m_Text_View[Cam_num]) // 타원 표시
//						{
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(Circle_Info[0],Circle_Info[1]),Size(-TYPE0_CAM0_Param.nRegion4Dist+Circle_Info[2]/2,-TYPE0_CAM0_Param.nRegion4Dist+Circle_Info[3]/2),Circle_Info[4],0,360,CV_RGB(255,100,0),1,CV_AA,0);
//						}
//						add(R3_Img,R4_Img,R4_Img);
//					}
//#pragma omp section
//					{
//						// 글자영역 찾아서 제거
//						linearPolar(ROI_Gray_Img,Polar_Thres_ROI_Gray_Img,Point2f(In_Circle_Info[0],In_Circle_Info[1]),Thres_ROI_Gray_Img.cols, INTER_LINEAR + WARP_FILL_OUTLIERS);
//						Rect t_right;
//						t_right.x = 10+(Circle_Info[2]+Circle_Info[3])/4;
//						t_right.y = 0;
//						t_right.width = Polar_Thres_ROI_Gray_Img.cols -t_right.x;
//						t_right.height = Polar_Thres_ROI_Gray_Img.rows;
//						Polar_Thres_ROI_Gray_Img(t_right) += 255;
//
//						Mat Morph_Polar_Thres_ROI_Gray_Img;
//						threshold(Polar_Thres_ROI_Gray_Img,Morph_Polar_Thres_ROI_Gray_Img,TYPE0_CAM0_Param.nRegionLetterThreshold,255,CV_THRESH_BINARY_INV);
//
//						if (Result_Debugging) // 못 머리 타원 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_Thres_Polar_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),Morph_Polar_Thres_ROI_Gray_Img);	
//						}
//
//						//TYPE0_CAM0_Param.nRegionLetterDist = TYPE0_CAM0_Param.nRegionLetterDistMargin + (In_Circle_Info[2]+In_Circle_Info[3])/4;
//						t_right.x = 0;
//						t_right.y = 0;
//						t_right.width = TYPE0_CAM0_Param.nRegionLetterDist;
//						t_right.height = Polar_Thres_ROI_Gray_Img.rows;
//						Morph_Polar_Thres_ROI_Gray_Img(t_right) = Mat::zeros(t_right.size(), CV_8UC1);
//
//						t_right.x = TYPE0_CAM0_Param.nRegionLetterDist + TYPE0_CAM0_Param.nRegionLetterWidth;
//						t_right.y = 0;
//						t_right.width = Polar_Thres_ROI_Gray_Img.cols - t_right.x;
//						t_right.height = Polar_Thres_ROI_Gray_Img.rows;
//						Morph_Polar_Thres_ROI_Gray_Img(t_right) = Mat::zeros(t_right.size(), CV_8UC1);
//
//						dilate(Morph_Polar_Thres_ROI_Gray_Img,Morph_Polar_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//						erode(Morph_Polar_Thres_ROI_Gray_Img,Morph_Polar_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//						erode(Morph_Polar_Thres_ROI_Gray_Img,Morph_Polar_Thres_ROI_Gray_Img,element_h,Point(-1,-1),TYPE0_CAM0_Param.nRegionLetterWidth/3);
//						dilate(Morph_Polar_Thres_ROI_Gray_Img,Morph_Polar_Thres_ROI_Gray_Img,element_h,Point(-1,-1),TYPE0_CAM0_Param.nRegionLetterWidth/3);
//						dilate(Morph_Polar_Thres_ROI_Gray_Img,Morph_Polar_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//
//						if (Result_Debugging) // 못 머리 타원 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_Morph_Polar_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),Morph_Polar_Thres_ROI_Gray_Img);		
//						}
//
//						Mat L_Morph_Polar_Thres_ROI_Gray_Img = Mat::zeros(Morph_Polar_Thres_ROI_Gray_Img.size(), CV_8UC1);
//						findContours( Morph_Polar_Thres_ROI_Gray_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//						Rect t_letter_bound;
//						vector<Data2D> t_vec_size;Data2D t_data;
//						for( int k = 0; k < contours.size(); k++ )
//						{  
//							t_letter_bound = boundingRect( Mat(contours[k]) );
//							if (t_letter_bound.x <= TYPE0_CAM0_Param.nRegionLetterDist+1 && t_letter_bound.width >= TYPE0_CAM0_Param.nRegionLetterWidth -1 && t_letter_bound.height >= TYPE0_CAM0_Param.nLetterMinSize && t_letter_bound.height <= TYPE0_CAM0_Param.nLetterMaxSize)
//							{
//								t_data.idx = k;t_data.s = t_letter_bound.height;
//								t_vec_size.push_back(t_data);
//							}
//						}
//
//						std::sort(t_vec_size.begin(), t_vec_size.end(), Point_compare_Increasing_2d);
//
//						if (t_vec_size.size() > 0)
//						{
//							for (int i = 0; i<TYPE0_CAM0_Param.nLetterCount; i++)
//							{
//								if (i < contours.size())
//								{
//									drawContours( L_Morph_Polar_Thres_ROI_Gray_Img, contours, t_vec_size[i].idx, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
//								}
//							}
//						}
//
//						linearPolar(L_Morph_Polar_Thres_ROI_Gray_Img,Inv_Polar_Thres_ROI_Gray_Img,Point2f(In_Circle_Info[0],In_Circle_Info[1]),Thres_ROI_Gray_Img.cols, INTER_LINEAR + WARP_INVERSE_MAP);
//
//						dilate(Inv_Polar_Thres_ROI_Gray_Img,Inv_Polar_Thres_ROI_Gray_Img,element,Point(-1,-1),3);
//						//erode(Inv_Polar_Thres_ROI_Gray_Img,Inv_Polar_Thres_ROI_Gray_Img,element,Point(-1,-1),1);
//
//						if (m_Text_View[Cam_num])
//						{				
//							findContours( Inv_Polar_Thres_ROI_Gray_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//							if (contours.size() > 0)
//							{
//								for( int k = 0; k < contours.size(); k++ )
//								{  
//									drawContours( Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]), contours, k, CV_RGB(0,255,0), 2, CV_AA, hierarchy);
//								}
//							}
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(In_Circle_Info[0],In_Circle_Info[1]),Size(TYPE0_CAM0_Param.nRegionLetterDist,TYPE0_CAM0_Param.nRegionLetterDist),In_Circle_Info[4],0,360,CV_RGB(0,255,100),1,CV_AA,0);
//							ellipse(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]),Size(In_Circle_Info[0],In_Circle_Info[1]),Size(TYPE0_CAM0_Param.nRegionLetterDist+TYPE0_CAM0_Param.nRegionLetterWidth,TYPE0_CAM0_Param.nRegionLetterDist+TYPE0_CAM0_Param.nRegionLetterWidth),In_Circle_Info[4],0,360,CV_RGB(0,255,100),1,CV_AA,0);
//						}
//
//
//
//						if (Result_Debugging) // 못 머리 타원 이미지 저장
//						{
//							msg.Format("Save\\Debugging\\Cam%d_%2d_Inv_Polar_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//							imwrite(msg.GetBuffer(),Inv_Polar_Thres_ROI_Gray_Img);		
//						}
//
//					}
//
//				} 
//			}
//
//			add(R2_Img,R4_Img,R4_Img);
//			subtract(R4_Img, Inv_Polar_Thres_ROI_Gray_Img, R4_Img);
//
//
//			// 불량만 찾기
//			findContours( R4_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			vector<double> vec_Defect_Size;
//			if (contours.size() > 0)
//			{
//				vector<Moments> mu(contours.size() );
//				Point2f rect_points[4];
//				for( int k = 0; k < contours.size(); k++ )
//				{  
//					mu[k] = moments( contours[k], false );
//					vec_Defect_Size.push_back(mu[k].m00);
//					if (m_Text_View[Cam_num]) // 내경 타원 표시
//					{				
//						drawContours( Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]), contours, k, CV_RGB(255,0,0), 2, CV_AA, hierarchy);
//						msg.Format("%1.0f",vec_Defect_Size[k]);
//						putText(Dst_Img[Cam_num](TYPE0_CAM0_Param.nRect[s]), msg.GetBuffer(), Point(mu[k].m10/mu[k].m00,mu[k].m01/mu[k].m00), fontFace, 0.6, CV_RGB(100,255,0), thickness, 8);
//					}
//				}
//			}
//
//			std::sort(vec_Defect_Size.begin(), vec_Defect_Size.end(), Point_compare_Increasing);
//			for( int k = 0; k < vec_Defect_Size.size(); k++ )
//			{  
//				Defect_size += (int)vec_Defect_Size[k];
//			}
//
//			if (Defect_size != 0)
//			{
//				Defect_size += TYPE0_CAM0_Param.Offset[0];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Defect Size(%d)",Defect_size);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,100), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Defect Size(%d)",Defect_size);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,100), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//
//			Result_Info[Cam_num].Format("C%d:00_%d", Cam_num, Defect_size);
//		}
//
//		//	Mat Polar_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//		//	Mat Inv_Polar_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//		//	linearPolar(ROI_Gray_Img,Polar_Thres_ROI_Gray_Img,Point2f(Circle_Info[0],Circle_Info[1]),Thres_ROI_Gray_Img.cols, INTER_LINEAR + WARP_FILL_OUTLIERS);
//		//	Rect t_right;
//		//	t_right.x = 10+(Circle_Info[2]+Circle_Info[3])/4;
//		//	t_right.y = 0;
//		//	t_right.width = Polar_Thres_ROI_Gray_Img.cols -t_right.x;
//		//	t_right.height = Polar_Thres_ROI_Gray_Img.rows;
//		//	Polar_Thres_ROI_Gray_Img(t_right) += 255;
//		//	linearPolar(Polar_Thres_ROI_Gray_Img,Inv_Polar_Thres_ROI_Gray_Img,Point2f(Circle_Info[0],Circle_Info[1]),Thres_ROI_Gray_Img.cols, INTER_LINEAR + WARP_INVERSE_MAP);
//
//		//	if (Result_Debugging) // 못 머리 타원 이미지 저장
//		//	{
//		//		msg.Format("Save\\Debugging\\Cam%d_%2d_Polar_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//		//		imwrite(msg.GetBuffer(),Polar_Thres_ROI_Gray_Img);		
//		//		msg.Format("Save\\Debugging\\Cam%d_%2d_Inv_Polar_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//		//		imwrite(msg.GetBuffer(),Inv_Polar_Thres_ROI_Gray_Img);		
//		//	}
//	}
//	else
//	{
//		Result_Info[Cam_num].Format("C%d:00_%d", Cam_num, -1);
//	}
//	return true;
//}
//
//
//bool CImPro_Library::RUN_ALG_TYPE0_CAM1()
//{
//	int Cam_num = 1;
//
//	if (!Gray_Img[Cam_num].empty()) //이미지가 있으면
//	{
//		vector<vector<Point> > contours;
//		vector<Vec4i> hierarchy;
//		CString msg;
//		Result_Info[Cam_num] = "";
//		double Max_Thickness = 0;
//		double Max_Height = 0;
//		Point Max_Thickness_Loc(10,100);
//		for (int s=0;s<5;s++)
//		{
//			if (TYPE0_CAM1_Param.nRect_Use[s] == 0 || TYPE0_CAM1_Param.nRect[s].width <= 0 || TYPE0_CAM1_Param.nRect[s].height <= 0)
//			{
//				continue;
//			}
//
//			// ROI 이미지 잘라오기
//			Mat ROI_Gray_Img = Gray_Img[Cam_num](TYPE0_CAM1_Param.nRect[s]).clone();	
//
//			if (m_Text_View[Cam_num]) // ROI 영역 표시
//			{
//				rectangle(Dst_Img[Cam_num],TYPE0_CAM1_Param.nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM1_Param.nRect[s].x+8,TYPE0_CAM1_Param.nRect[s].y + 23), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(0,0,255), 1, 8);
//			}
//
//			if (Result_Debugging) // ROI 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),ROI_Gray_Img);		
//			}
//
//			// 오링 사이드 임계화
//			Mat Thres_ROI_Gray_Img;	
//			threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM1_Param.nThreshold,255,CV_THRESH_BINARY_INV);
//			if (Result_Debugging) // 오링 사이드 임계화 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Thres_ROI_Gray_Img);		
//			}
//
//			// 오링 사이드 바닦 찾기
//			double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;
//			int t_Search_Range = 10;
//			for (int i = 0;i<t_Search_Range;i++)
//			{
//				for (int j = 0;j<Thres_ROI_Gray_Img.rows;j++)
//				{
//					if (Thres_ROI_Gray_Img.at<uchar>(j,i) > 0)
//					{
//						t_Left_x +=(double)i;
//						t_Left_y += (double)j;
//						t_cnt++;
//						break;
//					}
//				}
//			}
//			if (t_cnt > 0)
//			{
//				t_Left_x/=t_cnt;
//				t_Left_y/=t_cnt;
//			}
//
//			double t_Right_x = 0;double t_Right_y = 0;t_cnt = 0;
//			for (int i = Thres_ROI_Gray_Img.cols-1;i>=Thres_ROI_Gray_Img.cols - t_Search_Range-1;i--)
//			{
//				for (int j = 0;j<Thres_ROI_Gray_Img.rows;j++)
//				{
//					if (Thres_ROI_Gray_Img.at<uchar>(j,i) > 0)
//					{
//						t_Right_x +=(double)i;
//						t_Right_y += (double)j;
//						t_cnt++;
//						break;
//					}
//				}
//			}
//			if (t_cnt > 0)
//			{
//				t_Right_x/=t_cnt;
//				t_Right_y/=t_cnt;
//			}
//
//			if (t_Left_x > 0 && t_Left_y > 0 && t_Right_x > 0 && t_Right_y > 0)
//			{
//				line(Thres_ROI_Gray_Img,Point(t_Left_x,t_Left_y+1),Point(t_Right_x,t_Right_y+1),CV_RGB(0,0,0),2);
//			}
//			if (Result_Debugging) // 오링 사이드 바닦 라인 이미지
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Line_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Thres_ROI_Gray_Img);		
//			}
//			
//			erode(Thres_ROI_Gray_Img,Thres_ROI_Gray_Img,element_v,Point(-1,-1),1);
//			dilate(Thres_ROI_Gray_Img,Thres_ROI_Gray_Img,element_v,Point(-1,-1),1);
//
//			findContours( Thres_ROI_Gray_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			int m_max_object_num = -1;int m_max_object_value = 0;
//			Rect t_Side_rect;
//			RotatedRect t_Side_Rotatedrect;
//					
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				t_Side_rect = boundingRect( Mat(contours[k]) );
//				if (m_max_object_value <= t_Side_rect.width*t_Side_rect.height && t_Side_rect.x > 1)
//				{
//					m_max_object_value = t_Side_rect.width*t_Side_rect.height;
//					m_max_object_num = k;
//				}
//			}
//
//			Mat Target_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//
//			if (m_max_object_num > 0)
//			{
//				t_Side_rect = boundingRect( Mat(contours[m_max_object_num]) );
//				t_Side_Rotatedrect = minAreaRect( Mat(contours[m_max_object_num]) );
//				drawContours( Target_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
//			
//				Mat Dist_Img, label;//, //Tmp_Img;
//				distanceTransform(Target_Thres_ROI_Gray_Img, Dist_Img,label, CV_DIST_L2,3);
//				Point matchLoc0;
//				double minval0, maxval0;
//				cv::Point minLoc0, maxLoc0;
//				cv::minMaxLoc(Dist_Img, &minval0, &maxval0, &minLoc0, &maxLoc0);
//				matchLoc0 = maxLoc0;
//
//				for (int j = matchLoc0.y;j>=matchLoc0.y - 100;j--)
//				{
//					if (j>0 && j<Thres_ROI_Gray_Img.rows-1)
//					{
//						if (Thres_ROI_Gray_Img.at<uchar>(j,matchLoc0.x) == 0)
//						{
//							minLoc0.x =matchLoc0.x;
//							minLoc0.y = j;
//							break;
//						}
//					}
//				}
//				for (int j = matchLoc0.y;j<matchLoc0.y + 100;j++)
//				{
//					if (j>0 && j<Thres_ROI_Gray_Img.rows-1)
//					{
//						if (Thres_ROI_Gray_Img.at<uchar>(j,matchLoc0.x) == 0)
//						{
//							maxLoc0.x = matchLoc0.x;
//							maxLoc0.y = j;
//							break;
//						}
//					}
//				}
//
//				Max_Thickness = abs(maxLoc0.y - minLoc0.y);
//				Max_Thickness_Loc.x = TYPE0_CAM1_Param.nRect[s].x+minLoc0.x - 150;
//				Max_Thickness_Loc.y = TYPE0_CAM1_Param.nRect[s].y+minLoc0.y - 30;
//
//				if (m_Text_View[Cam_num]) // ROI 영역 표시
//				{
//					drawContours( Dst_Img[Cam_num](TYPE0_CAM1_Param.nRect[s]), contours, m_max_object_num, CV_RGB(0,255,0), 1, CV_AA, hierarchy);
//					drawArrow(Dst_Img[Cam_num](TYPE0_CAM1_Param.nRect[s]), minLoc0, maxLoc0, CV_RGB(255, 100, 0), 9, 2,1,0);
//					drawArrow(Dst_Img[Cam_num](TYPE0_CAM1_Param.nRect[s]), maxLoc0, minLoc0, CV_RGB(255, 100, 0), 9, 2,1,0);
//
//					drawArrow(Dst_Img[Cam_num](TYPE0_CAM1_Param.nRect[s]), Point(t_Side_rect.x+t_Side_rect.width+10, t_Side_rect.y), Point(t_Side_rect.x+t_Side_rect.width+10, t_Side_rect.y + t_Side_rect.height), CV_RGB(0, 100, 255), 9, 2,1,0);
//					drawArrow(Dst_Img[Cam_num](TYPE0_CAM1_Param.nRect[s]), Point(t_Side_rect.x+t_Side_rect.width+10, t_Side_rect.y + t_Side_rect.height), Point(t_Side_rect.x+t_Side_rect.width+10, t_Side_rect.y), CV_RGB(0, 100, 255), 9, 2,1,0);
//				}
//			}
//
//			if (Result_Debugging) // 오링 사이드 바닦 라인 이미지
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Target_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Target_Thres_ROI_Gray_Img);		
//			}
//
//
//			if (Max_Thickness != 0)
//			{
//				Max_Thickness *= TYPE0_CAM1_Param.Res_Y;
//				Max_Thickness += TYPE0_CAM1_Param.Offset[0];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Max Thickness(%1.3f)",Max_Thickness);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Max_Thickness_Loc, fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Max Thickness(%1.3f)",Max_Thickness);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Max_Thickness_Loc, fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//			
//			//Max_Height = t_Side_Rotatedrect.size.height;
//			Max_Height = t_Side_rect.height;
//			if (Max_Height != 0)
//			{
//				Max_Height *= TYPE0_CAM1_Param.Res_Y;
//				Max_Height += TYPE0_CAM1_Param.Offset[1];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Max Height(%1.3f)",Max_Height);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM1_Param.nRect[s].x+t_Side_rect.x+t_Side_rect.width-170, TYPE0_CAM1_Param.nRect[s].y+t_Side_rect.y-100), fontFace, fontScale, CV_RGB(0, 100, 255), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Max Height(%1.3f)",Max_Height);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM1_Param.nRect[s].x+t_Side_rect.x+t_Side_rect.width-170, TYPE0_CAM1_Param.nRect[s].y+t_Side_rect.y-100), fontFace, fontScale, CV_RGB(0, 100, 255), thickness, 8);
//				}
//			}
//		}
//		Result_Info[Cam_num].Format("C%d:00_%1.3f_C%d:01_%1.3f", Cam_num, Max_Thickness, Cam_num, Max_Height);
//	}
//	else
//	{
//		Result_Info[Cam_num].Format("C%d:00_%d_C%d:00_%d", Cam_num, -1, Cam_num, -1);
//	}
//	return true;
//}
//
//
//bool CImPro_Library::RUN_ALG_TYPE1_CAM1()
//{
//	int Cam_num = 1;
//	
//	if (!Gray_Img[Cam_num].empty()) //이미지가 있으면
//	{
//		vector<vector<Point> > contours;
//		vector<Vec4i> hierarchy;
//		CString msg;
//		Result_Info[Cam_num] = "";
//		double Out_Circle_Info[5] = {0,}; // 외경 : 중심X,Y,가로,세로,각도
//		double In_Circle_Info[5] = {0,};  // 내경 : 중심X,Y,가로,세로,각도
//		double Out_Diameter = 0;		  // 외경
//		double In_Diameter = 0;			  // 내경
//		double Hom_Size_Min = 0;		  // 홈 최소 크기
//		double Hom_Size_Max = 0;		  // 홈 최대 크기
//		double Burr_Size = 0;			  // 버 크기
//		double Cut_Size = 0;			  // 끈낌 크기
//		for (int s=0;s<5;s++)
//		{
//			if (TYPE1_CAM1_Param.nRect_Use[s] == 0 || TYPE1_CAM1_Param.nRect[s].width <= 0 || TYPE1_CAM1_Param.nRect[s].height <= 0)
//			{
//				continue;
//			}
//
//			// ROI 이미지 잘라오기
//			Mat ROI_Gray_Img = Gray_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]).clone();	
//
//			if (Result_Debugging) // ROI 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),ROI_Gray_Img);		
//			}
//			if (m_Text_View[Cam_num]) // ROI 영역 표시
//			{
//				rectangle(Dst_Img[Cam_num],TYPE1_CAM1_Param.nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE1_CAM1_Param.nRect[s].x+8,TYPE1_CAM1_Param.nRect[s].y + 23), FONT_HERSHEY_SIMPLEX, 0.6, CV_RGB(0,0,255), 1, 8);
//			}
//
//			// 오링 임계화
//			Mat Thres_ROI_Gray_Img;	
//			threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE1_CAM1_Param.nHeadThreshold,255,CV_THRESH_BINARY_INV);
//			if (Result_Debugging) // 임계화 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Thres_ROI_Gray_Img);		
//			}
//
//			// 오링 외경 채운 이미지
//			Mat Filled_Thres_ROI_Gray_Img = Thres_ROI_Gray_Img.clone();
//			dilate(Filled_Thres_ROI_Gray_Img,Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//			erode(Filled_Thres_ROI_Gray_Img,Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),5);
//
//			J_Delete_Boundary(Filled_Thres_ROI_Gray_Img,1);
//			J_Fill_Hole(Filled_Thres_ROI_Gray_Img);
//			if (Result_Debugging) // 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Filled_Thres_ROI_Gray_Img);		
//			}
//
//			// 오링만 찾기
//			findContours( Filled_Thres_ROI_Gray_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			if(contours.size() == 0) // 칸투어 갯수로 예외처리
//			{
//				Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d_C%d:03_%d", Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1);
//				rectangle(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]),Rect(10,10,TYPE1_CAM1_Param.nRect[s].width-20,TYPE1_CAM1_Param.nRect[s].height-20),Scalar(0,0,255),2);
//				return true;
//			}
//
//			vector<Rect> boundRect( contours.size() );
//			int m_max_object_num = -1;int m_max_object_value = 0;
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				boundRect[k] = boundingRect( Mat(contours[k]) );
//				if (m_max_object_value<= boundRect[k].width*boundRect[k].height 
//					&& boundRect[k].x > 1 && boundRect[k].y > 1
//					&& boundRect[k].x + boundRect[k].width < Thres_ROI_Gray_Img.cols-1
//					&& boundRect[k].y + boundRect[k].height < Thres_ROI_Gray_Img.rows-1)
//				{
//					m_max_object_value = boundRect[k].width*boundRect[k].height;
//					m_max_object_num = k;
//				}
//			}
//
//			// 오링를 못 찾을 경우 에러 처리함.
//			if(m_max_object_num == -1)
//			{
//				Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d_C%d:03_%d_C%d:04_%d", Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1);
//				rectangle(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]),Rect(10,10,TYPE1_CAM1_Param.nRect[s].width-20,TYPE1_CAM1_Param.nRect[s].height-20),Scalar(0,0,255),2);
//				return true;
//			}
//
//			// 오링만 다시 그림
//			Mat Oring_Filled_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			drawContours( Oring_Filled_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
//			if (Result_Debugging) // 못 머리 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Oring_Filled_Thres_ROI_Gray_Img);		
//			}
//
//			// 오링 타원 핏팅
//			J_Fitting_Ellipse(Oring_Filled_Thres_ROI_Gray_Img.clone(), Out_Circle_Info);
//
//			// 오링 타원 결과 그림
//			Mat Ellipse_Oring_Filled_Thres_ROI_Gray_Img = Mat::zeros(Thres_ROI_Gray_Img.size(), CV_8UC1);
//			ellipse(Ellipse_Oring_Filled_Thres_ROI_Gray_Img,Size(Out_Circle_Info[0],Out_Circle_Info[1]),Size(Out_Circle_Info[2]/2,Out_Circle_Info[3]/2),Out_Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//			if (Result_Debugging) // 오링 타원 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Ellipse_Oring_Filled_Thres_ROI_Gray_Img);		
//			}
//			if (m_Text_View[Cam_num]) // 타원 표시
//			{
//				ellipse(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]),Size(Out_Circle_Info[0],Out_Circle_Info[1]),Size(Out_Circle_Info[2]/2,Out_Circle_Info[3]/2),Out_Circle_Info[4],0,360,CV_RGB(0,255,0),1,CV_AA,0);
//			}
//
//			Out_Diameter = (Out_Circle_Info[2]+Out_Circle_Info[3])/2;
//
//			Mat In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img;
//
//			subtract(Ellipse_Oring_Filled_Thres_ROI_Gray_Img, Thres_ROI_Gray_Img,In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img);
//			erode(In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),1);
//			dilate(In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,element,Point(-1,-1),1);
//
//			if (Result_Debugging) // 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img);			
//			}
//
//			// 오링 내경 타원 핏팅
//			J_Fitting_Ellipse(In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.clone(), In_Circle_Info);
//	
//			if (m_Text_View[Cam_num]) // 내경 타원 표시
//			{
//				ellipse(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]),Size(In_Circle_Info[0],In_Circle_Info[1]),Size(In_Circle_Info[2]/2,In_Circle_Info[3]/2),In_Circle_Info[4],0,360,CV_RGB(0,255,0),1,CV_AA,0);
//			}
//
//			In_Diameter = (In_Circle_Info[2]+In_Circle_Info[3])/2;
//
//			Mat Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img = In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.clone();
//			ellipse(Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,Size(In_Circle_Info[0],In_Circle_Info[1]),Size(TYPE1_CAM1_Param.nHomMargin + In_Circle_Info[2]/2,TYPE1_CAM1_Param.nHomMargin + In_Circle_Info[3]/2),In_Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//			if (Result_Debugging) // 홈 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img);			
//			}
//			Mat Hom_Img = Mat::zeros(Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img.size(), CV_8UC1);
//			ellipse(Hom_Img,Size(In_Circle_Info[0],In_Circle_Info[1]),Size(2+TYPE1_CAM1_Param.nHomHeight+In_Circle_Info[2]/2,2+TYPE1_CAM1_Param.nHomHeight+In_Circle_Info[3]/2),In_Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//			bitwise_and(Hom_Img,Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,Hom_Img);
//			erode(Hom_Img,Hom_Img,element,Point(-1,-1),2);
//			dilate(Hom_Img,Hom_Img,element,Point(-1,-1),2);
//			if (Result_Debugging) // 홈 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Hom_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Hom_Img);			
//			}
//
//			// 홈만 찾기
//			findContours( Hom_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			vector<double> vec_Hom_Size;
//			if (contours.size() > 0)
//			{
//				vector<RotatedRect> HomboundRect( contours.size() );
//				vector<Moments> mu(contours.size() );
//				Point2f rect_points[4];
//				for( int k = 0; k < contours.size(); k++ )
//				{  
//					HomboundRect[k] = minAreaRect( Mat(contours[k]) );
//					mu[k] = moments( contours[k], false );
//					vec_Hom_Size.push_back(mu[k].m00);
//					if (m_Text_View[Cam_num]) // 내경 타원 표시
//					{				
//						HomboundRect[k].points( rect_points );
//						for( int j = 0; j < 4; j++ )
//						{
//							line( Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), rect_points[j], rect_points[(j+1)%4], CV_RGB(255,0,0), 1, 8 );
//						}
//						msg.Format("%1.0f",vec_Hom_Size[k]);
//						putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), rect_points[0], fontFace, 0.6, CV_RGB(255,100,0), thickness, 8);
//					}
//				}
//			}
//
//			if (vec_Hom_Size.size() > 0)
//			{
//				std::sort(vec_Hom_Size.begin(), vec_Hom_Size.end(), Point_compare_Increasing);
//				Hom_Size_Min = vec_Hom_Size[0];
//				Hom_Size_Max = vec_Hom_Size[vec_Hom_Size.size()-1];
//			}
//			if (vec_Hom_Size.size() < 4)
//			{
//				Hom_Size_Min = 0;
//			}
//			//msg.Format("Hom Min = %1.0f",Hom_Size_Min);
//			//putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), Point(10,100), fontFace, 0.6, CV_RGB(255,100,0), thickness, 8);
//			//msg.Format("Hom Max = %1.0f",Hom_Size_Max);
//			//putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), Point(10,130), fontFace, 0.6, CV_RGB(255,100,0), thickness, 8);
//
//
//			// Burr 측정
//			Mat Burr_Guide = Ellipse_Oring_Filled_Thres_ROI_Gray_Img.clone();
//			Mat Burr_Img;
//			//Thres_ROI_Gray_Img
//			//Ellipse_Oring_Filled_Thres_ROI_Gray_Img
//			ellipse(Burr_Guide,Size(Out_Circle_Info[0],Out_Circle_Info[1]),Size(TYPE1_CAM1_Param.nOutterGap+Out_Circle_Info[2]/2,TYPE1_CAM1_Param.nOutterGap+Out_Circle_Info[3]/2),Out_Circle_Info[4],0,360,CV_RGB(255,255,255),CV_FILLED,CV_AA,0);
//			ellipse(Burr_Guide,Size(In_Circle_Info[0],In_Circle_Info[1]),Size(-TYPE1_CAM1_Param.nInnerGap+In_Circle_Info[2]/2,-TYPE1_CAM1_Param.nInnerGap+In_Circle_Info[3]/2),In_Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//			subtract(Thres_ROI_Gray_Img, Burr_Guide, Burr_Img);
//			bitwise_and(Burr_Img,Oring_Filled_Thres_ROI_Gray_Img,Burr_Img);
//			erode(Burr_Img,Burr_Img,element,Point(-1,-1),2);
//			dilate(Burr_Img,Burr_Img,element,Point(-1,-1),2);
//			
//			if (Result_Debugging) // 홈 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Burr_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Burr_Img);			
//			}
//
//			
//			// Burr 찾기
//			findContours( Burr_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			vector<double> vec_Burr_Size;
//			if (contours.size() > 0)
//			{
//				vector<Moments> mu(contours.size() );
//				for( int k = 0; k < contours.size(); k++ )
//				{  
//					mu[k] = moments( contours[k], false );
//					if (TYPE1_CAM1_Param.nBurrDefectMinSize <= mu[k].m00)
//					{
//						vec_Burr_Size.push_back(mu[k].m00);
//						if (m_Text_View[Cam_num]) // 내경 타원 표시
//						{				
//							drawContours( Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), contours, k, CV_RGB(255,0,0), 2, CV_AA, hierarchy);
//							msg.Format("%1.0f",vec_Burr_Size[k]);
//							putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), Point(mu[k].m10/mu[k].m00,mu[k].m01/mu[k].m00), fontFace, 0.6, CV_RGB(255,100,0), thickness, 8);
//						}
//					}
//				}
//			}
//			if (vec_Burr_Size.size() > 0)
//			{
//				std::sort(vec_Burr_Size.begin(), vec_Burr_Size.end(), Point_compare_Increasing);
//				for( int k = 0; k < vec_Burr_Size.size(); k++ )
//				{ 
//					Burr_Size+=vec_Burr_Size[k];
//				}
//			}
//			//msg.Format("Burr Size = %1.0f",Burr_Size);
//			//putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), Point(10,160), fontFace, 0.6, CV_RGB(255,100,0), thickness, 8);
//
//
//			// 끈김 
//			Mat Cut_Img;
//			ellipse(Burr_Guide,Size(In_Circle_Info[0],In_Circle_Info[1]),Size(TYPE1_CAM1_Param.nHomHeight+In_Circle_Info[2]/2,TYPE1_CAM1_Param.nHomHeight+In_Circle_Info[3]/2),In_Circle_Info[4],0,360,CV_RGB(0,0,0),CV_FILLED,CV_AA,0);
//			bitwise_and(Burr_Guide,Hom_In_Ellipse_Oring_Filled_Thres_ROI_Gray_Img,Cut_Img);
//			erode(Cut_Img,Cut_Img,element,Point(-1,-1),2);
//			dilate(Cut_Img,Cut_Img,element,Point(-1,-1),2);
//			if (Result_Debugging) // 끈김 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Cut_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Cut_Img);			
//			}
//
//			findContours( Cut_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			vector<double> vec_Cut_Size;
//			if (contours.size() > 0)
//			{
//				vector<Moments> mu(contours.size() );
//				for( int k = 0; k < contours.size(); k++ )
//				{  
//					mu[k] = moments( contours[k], false );
//					vec_Cut_Size.push_back(mu[k].m00);
//					if (m_Text_View[Cam_num]) // 내경 타원 표시
//					{				
//						drawContours( Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), contours, k, CV_RGB(255,0,0), CV_FILLED, CV_AA, hierarchy);
//						msg.Format("%1.0f",vec_Cut_Size[k]);
//						putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), Point(mu[k].m10/mu[k].m00,mu[k].m01/mu[k].m00), fontFace, 0.6, CV_RGB(100,255,0), thickness, 8);
//					}
//				}
//			}
//
//			if (vec_Cut_Size.size() > 0)
//			{
//				std::sort(vec_Cut_Size.begin(), vec_Cut_Size.end(), Point_compare_Increasing);
//
//				for( int k = 0; k < vec_Cut_Size.size(); k++ )
//				{ 
//					Cut_Size+=vec_Cut_Size[k];
//				}
//			}
//			//msg.Format("Cut/Short Size = %1.0f",Cut_Size);
//			//putText(Dst_Img[Cam_num](TYPE1_CAM1_Param.nRect[s]), msg.GetBuffer(), Point(10,190), fontFace, 0.6, CV_RGB(255,100,0), thickness, 8);
//
//			if (Out_Diameter != 0)
//			{
//				Out_Diameter *= TYPE1_CAM1_Param.Res_X;
//				Out_Diameter += TYPE1_CAM1_Param.Offset[0];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Out Diameter(%1.3f)",Out_Diameter);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,100), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Out Diameter(%1.3f)",Out_Diameter);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,100), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//
//			if (In_Diameter != 0)
//			{
//				In_Diameter *= TYPE1_CAM1_Param.Res_X;
//				In_Diameter += TYPE1_CAM1_Param.Offset[1];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("In Diameter(%1.3f)",In_Diameter);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,130), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("In Diameter(%1.3f)",In_Diameter);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,130), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//
//			if (Hom_Size_Min != 0)
//			{
//				Hom_Size_Min += TYPE1_CAM1_Param.Offset[2];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Hom Min Size(%1.0f)",Hom_Size_Min);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,160), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Hom Min Size(%1.0f)",Hom_Size_Min);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,160), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//
//			if (Hom_Size_Max != 0)
//			{
//				Hom_Size_Max += TYPE1_CAM1_Param.Offset[3];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Hom Max Size(%1.0f)",Hom_Size_Max);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,190), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Hom Max Size(%1.0f)",Hom_Size_Max);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,190), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//
//			if (Burr_Size != 0)
//			{
//				Burr_Size += TYPE1_CAM1_Param.Offset[4];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Burr Size(%1.0f)",Burr_Size);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,220), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Burr Size(%1.0f)",Burr_Size);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,220), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//
//			if (Cut_Size != 0)
//			{
//				Cut_Size += TYPE1_CAM1_Param.Offset[5];
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Cut & Short Size(%1.0f)",Cut_Size);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,250), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			} else
//			{
//				if (m_Text_View[Cam_num])
//				{
//					msg.Format("Cut & Short Size(%1.0f)",Cut_Size);
//					putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,250), fontFace, 0.8, CV_RGB(255,100,0), thickness, 8);
//				}
//			}
//		}
//		Result_Info[Cam_num].Format("C%d:00_%1.3f_C%d:01_%1.3f_C%d:02_%d_C%d:03_%d_C%d:04_%d_C%d:05_%d", 
//			Cam_num, (float)Out_Diameter, 
//			Cam_num, (float)In_Diameter, 
//			Cam_num, (int)Hom_Size_Min, 
//			Cam_num, (int)Hom_Size_Max, 
//			Cam_num, (int)Burr_Size, 
//			Cam_num, (int)Cut_Size);
//	}
//	else
//	{
//		Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d_C%d:03_%d_C%d:04_%d_C%d:05_%d", Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1);
//	}
//	return true;
//}
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
//
//
//
//
//bool CImPro_Library::RUN_ALG_TYPE0_CAM2()
//{
//	int Cam_num = 2;
//	Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d_C%d:03_%d_C%d:04_%d", Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1);
//	return true;
//
//if (!Gray_Img[Cam_num].empty()) //이미지가 있으면
//	{
//		vector<vector<Point> > contours;
//		vector<Vec4i> hierarchy;
//		CString msg;
//		Result_Info[Cam_num] = "";
//		double Whole_Length = 0;
//		double Bending_Lenght = 0;
//		double Diff_Concentricity = 0;
//		double Shape_end_nail = 0;
//		double Head_Height = 0;
//		for (int s=0;s<5;s++)
//		{
//			if (TYPE0_CAM2_Param.nRect_Use[s] == 0 || TYPE0_CAM2_Param.nRect[s].width <= 0 || TYPE0_CAM2_Param.nRect[s].height <= 0)
//			{
//				continue;
//			}
//
//			// ROI 이미지 잘라오기
//			Mat ROI_Gray_Img = Gray_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]).clone();	
//
//			if (Result_Debugging) // ROI 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),ROI_Gray_Img);		
//			}
//
//			// 못 사이드 임계화
//			Mat Thres_ROI_Gray_Img;	
//			threshold(ROI_Gray_Img,Thres_ROI_Gray_Img,TYPE0_CAM2_Param.nThreshold,255,CV_THRESH_BINARY_INV);
//			if (Result_Debugging) // 못 사이드 임계화 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Thres_ROI_Gray_Img);		
//			}
//
//			// 못 회전량 계산
//			Rect t_Sub_ROI;
//			t_Sub_ROI.x = 0;t_Sub_ROI.y = ROI_Gray_Img.rows/2;t_Sub_ROI.width = ROI_Gray_Img.cols;t_Sub_ROI.height = ROI_Gray_Img.rows;
//			if (Result_Debugging) // 못 사이드 임계화 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Sub_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Thres_ROI_Gray_Img(t_Sub_ROI));		
//			}
//
//			findContours( Thres_ROI_Gray_Img(t_Sub_ROI).clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//			int m_max_object_num = -1;int m_max_object_value = 0;
//			RotatedRect minRect;
//			Point2f rect_points[4];
//			double t_angle=0;
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				minRect = minAreaRect(contours[k]);
//				if (m_max_object_value<= minRect.size.height)
//				{
//					m_max_object_value = minRect.size.height;
//					if(minRect.size.width<minRect.size.height){
//						t_angle = minRect.angle;
//					}else{
//						t_angle = -(270 - minRect.angle);
//					}
//					m_max_object_num = k;
//				}
//			}
//
//			if (m_max_object_num == -1)
//			{
//				Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d_C%d:03_%d_C%d:04_%d", Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1);
//				rectangle(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Rect(10,10,TYPE0_CAM2_Param.nRect[s].width-20,TYPE0_CAM2_Param.nRect[s].height-20),Scalar(0,0,255),2);
//				return true;
//			}
//
//			Mat Target_Thres_ROI_Gray_Img;
//			erode(Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),TYPE0_CAM2_Param.nThickness/2);
//			dilate(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_h,Point(-1,-1),TYPE0_CAM2_Param.nThickness);
//			subtract(Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img);
//			erode(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_v,Point(-1,-1),1);
//			dilate(Target_Thres_ROI_Gray_Img,Target_Thres_ROI_Gray_Img,element_v,Point(-1,-1),1);
//			if (Result_Debugging) // 못 사이드 임계화 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Target_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Target_Thres_ROI_Gray_Img);		
//			}
//
//			Mat Rotate_Target_Thres_ROI_Gray_Img;
//			J_Rotate(Target_Thres_ROI_Gray_Img,t_angle,Rotate_Target_Thres_ROI_Gray_Img);
//			if (m_Text_View[Cam_num]) // ROI 영역 표시
//			{
//				J_Rotate(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),t_angle,Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]));
//				rectangle(Dst_Img[Cam_num],TYPE0_CAM2_Param.nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(TYPE0_CAM2_Param.nRect[s].x-30,TYPE0_CAM2_Param.nRect[s].y + 6), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(0,0,255), 1, 8);
//			}
//
//			if (Result_Debugging) // 못 사이드 회전 이미지 저장
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_Rotate_Target_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
//				imwrite(msg.GetBuffer(),Rotate_Target_Thres_ROI_Gray_Img);		
//			}
//
//			// 해드와 사이드 합침
//			findContours( Rotate_Target_Thres_ROI_Gray_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			m_max_object_num = -1;int m_2nd_max_object_num = -1;
//			m_max_object_value = 0;
//			Rect tt_rect;Rect t_Side_rect;Rect t_Head_rect;
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				tt_rect = boundingRect( Mat(contours[k]) );
//				if (m_max_object_value<= tt_rect.width*tt_rect.height)
//				{
//					m_max_object_value = tt_rect.width*tt_rect.height;
//					m_max_object_num = k;
//				}
//			}
//			m_max_object_value = 0;
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				tt_rect = boundingRect( Mat(contours[k]) );
//				if (m_max_object_value<= tt_rect.width*tt_rect.height && m_max_object_num != k)
//				{
//					m_max_object_value = tt_rect.width*tt_rect.height;
//					m_2nd_max_object_num = k;
//				}
//			}
//
//			if (m_max_object_num >= 0 && m_2nd_max_object_num >= 0)
//			{
//				t_Side_rect = boundingRect( Mat(contours[m_max_object_num]) );
//				t_Head_rect = boundingRect( Mat(contours[m_2nd_max_object_num]) );
//				vector<vector<Point> > total_contours(1);
//				for (int j=0;j<contours[m_max_object_num].size();j++)
//				{
//					total_contours[0].push_back(contours[m_max_object_num][j]);
//				}
//				for (int j=0;j<contours[m_2nd_max_object_num].size();j++)
//				{
//					total_contours[0].push_back(contours[m_2nd_max_object_num][j]);
//				}
//				tt_rect = boundingRect( Mat(total_contours[0]) );
//
//				if (m_Text_View[Cam_num]) // ROI 영역 표시
//				{
//					Rect ttt_rect(TYPE0_CAM2_Param.nRect[s].x-1,TYPE0_CAM2_Param.nRect[s].y-1,TYPE0_CAM2_Param.nRect[s].width+2,TYPE0_CAM2_Param.nRect[s].height+2);
//					rectangle(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),ttt_rect,CV_RGB(0,255,0),1);
//					rectangle(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),t_Side_rect,CV_RGB(255,0,100),1);
//					rectangle(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),t_Head_rect,CV_RGB(255,0,100),1);
//				}
//				// 전장 계산
//				Whole_Length = tt_rect.height;
//
//				// Head Height 계산
//				Head_Height = t_Head_rect.height;
//
//				// 동심도 계산
//				double Top_c_x;double Top_c_y;double Top_c_x_cnt = 0;
//				double Center_c_x;double Center_c_y;double Center_c_x_cnt = 0;
//				double Bottom_c_x;double Bottom_c_y;double Bottom_c_x_cnt = 0;
//				double End_c_x_cnt[10] = {0,};
//				for (int j=0;j<10;j++)
//				{
//					for (int i=t_Side_rect.x;i<t_Side_rect.x+t_Side_rect.width;i++)
//					{
//						if (Rotate_Target_Thres_ROI_Gray_Img.at<uchar>(t_Side_rect.y+10+j,i) > 0)
//						{
//							Top_c_x+=(double)i;Top_c_y+=(double)(t_Side_rect.y+10+j);
//							Top_c_x_cnt+=1.0;
//						}
//						if (Rotate_Target_Thres_ROI_Gray_Img.at<uchar>(t_Side_rect.y+t_Side_rect.height/2-5+j,i) > 0)
//						{
//							Center_c_x+=(double)i;Center_c_y+=(double)(t_Side_rect.y+t_Side_rect.height/2-5+j);
//							Center_c_x_cnt+=1.0;
//						}
//						if (Rotate_Target_Thres_ROI_Gray_Img.at<uchar>(t_Side_rect.y+t_Side_rect.height-80-j,i) > 0)
//						{
//							Bottom_c_x+=(double)i;Bottom_c_y+=(double)(t_Side_rect.y+t_Side_rect.height-80-j);
//							Bottom_c_x_cnt+=1.0;
//						}
//						if (Rotate_Target_Thres_ROI_Gray_Img.at<uchar>(t_Side_rect.y+t_Side_rect.height-j*8,i) > 0)
//						{
//							End_c_x_cnt[j]+=1.0;
//						}
//					}
//					if (m_Text_View[Cam_num])
//					{
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(t_Side_rect.x,t_Side_rect.y+t_Side_rect.height-j*8),Point(t_Side_rect.x+t_Side_rect.width,t_Side_rect.y+t_Side_rect.height-j*8),CV_RGB(255,100,0),1);
//					}
//				}
//				if (Top_c_x_cnt > 0)
//				{
//					Top_c_x /= Top_c_x_cnt;Top_c_y /= Top_c_x_cnt;
//					if (m_Text_View[Cam_num])
//					{
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(Top_c_x-30,Top_c_y),Point(Top_c_x+30,Top_c_y),CV_RGB(255,255,0),1);
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(Top_c_x,Top_c_y-5),Point(Top_c_x,Top_c_y+5),CV_RGB(255,255,0),1);
//					}
//				}
//				if (Center_c_x_cnt > 0)
//				{
//					Center_c_x /= Center_c_x_cnt;Center_c_y /= Center_c_x_cnt;
//					if (m_Text_View[Cam_num])
//					{
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(Center_c_x-30,Center_c_y),Point(Center_c_x+30,Center_c_y),CV_RGB(255,255,0),1);
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(Center_c_x,Center_c_y-5),Point(Center_c_x,Center_c_y+5),CV_RGB(255,255,0),1);
//					}
//				}
//				if (Bottom_c_x_cnt > 0)
//				{
//					Bottom_c_x /= Bottom_c_x_cnt;Bottom_c_y /= Bottom_c_x_cnt;
//					if (m_Text_View[Cam_num])
//					{
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(Bottom_c_x-30,Bottom_c_y),Point(Bottom_c_x+30,Bottom_c_y),CV_RGB(255,255,0),1);
//						line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(Bottom_c_x,Bottom_c_y-5),Point(Bottom_c_x,Bottom_c_y+5),CV_RGB(255,255,0),1);
//					}
//				}
//				Diff_Concentricity = abs(((double)t_Head_rect.x + (double)t_Head_rect.width/2) - Top_c_x);
//
//				// 휨 각도 계산
//				Bending_Lenght = max(max(abs(Top_c_x - Center_c_x),abs(Top_c_x - Bottom_c_x)),abs(Bottom_c_x - Center_c_x));
//				if (m_Text_View[Cam_num])
//				{
//					line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(t_Head_rect.x + t_Head_rect.width/2,t_Head_rect.y),Point(t_Head_rect.x + t_Head_rect.width/2,t_Head_rect.y+t_Head_rect.height),CV_RGB(255,255,0),1);
//					line(Dst_Img[Cam_num](TYPE0_CAM2_Param.nRect[s]),Point(t_Head_rect.x + t_Head_rect.width/2 - 5,t_Head_rect.y+ t_Head_rect.height/2),Point(t_Head_rect.x + t_Head_rect.width/2 + 5,t_Head_rect.y+ t_Head_rect.height/2),CV_RGB(255,255,0),1);
//				}
//
//				// 못 끝 삼각형 Ratio
//				vector<double> End_diff;
//				for (int j=0;j<9;j++)
//				{
//					double t_v = (End_c_x_cnt[j+1]-End_c_x_cnt[j])/5;
//					End_diff.push_back(t_v);
//					if (t_v < 0)
//					{
//						Shape_end_nail = t_v;
//					}
//				}
//				if (Shape_end_nail >= 0)
//				{
//					Shape_end_nail = *max_element(End_diff.begin(), End_diff.end());
//				}
//			}
//		}
//		if (Whole_Length != 0)
//		{
//			Whole_Length *= TYPE0_CAM2_Param.Res_Y;
//			Whole_Length += TYPE0_CAM2_Param.Offset[0];
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("W.L.=%1.3f",Whole_Length);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,100), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		} else
//		{
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("W.L.=%1.3f",Whole_Length);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,100), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		}
//		if (Bending_Lenght != 0)
//		{
//			Bending_Lenght = Bending_Lenght*TYPE0_CAM1_Param.Res_X + TYPE0_CAM1_Param.Offset[1];
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("B.L.=%1.3f",Bending_Lenght);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,130), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		} else
//		{
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("B.L.=%1.3f",Bending_Lenght);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,130), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		}
//		if (Diff_Concentricity != 0)
//		{
//			Diff_Concentricity *= TYPE0_CAM2_Param.Res_X;
//			Diff_Concentricity += TYPE0_CAM2_Param.Offset[2];
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("D.C.=%1.3f",Diff_Concentricity);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,160), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		} else
//		{
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("D.C.=%1.3f",Diff_Concentricity);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,160), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		}
//		if (Shape_end_nail != 0)
//		{
//			Shape_end_nail += TYPE0_CAM2_Param.Offset[3];
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("S.N.=%1.3f",Shape_end_nail);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,190), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		} else
//		{
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("S.N.=%1.3f",Shape_end_nail);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,190), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		}
//		if (Head_Height != 0)
//		{
//			Head_Height *= TYPE0_CAM2_Param.Res_Y;
//			Head_Height += TYPE0_CAM2_Param.Offset[4];
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("H.H.=%1.3f",Head_Height);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,220), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		} else
//		{
//			if (m_Text_View[Cam_num])
//			{
//				msg.Format("H.H.=%1.3f",Head_Height);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(10,220), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//			}
//		}
//
//		//double Whole_Length = 0;
//		//double Bending_Lenght = 0;
//		//double Diff_Concentricity = 0;
//		//double Shape_end_nail = 0;
//		//double Head_Height = 0;
//
//
//		Result_Info[Cam_num].Format("C%d:00_%1.3f_C%d:01_%1.3f_C%d:02_%1.3f_C%d:03_%1.3f_C%d:04_%1.3f", Cam_num, Whole_Length, Cam_num, Bending_Lenght, Cam_num, Diff_Concentricity, Cam_num, Shape_end_nail, Cam_num, Head_Height);
//	}
//	else
//	{
//		Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d_C%d:03_%d_C%d:04_%d", Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1, Cam_num, -1);
//	}
//	return true;
//}
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//bool CImPro_Library::RUN_Algorithm_Side(int Cam_num)
//{
//	if (!Gray_Img[Cam_num].empty()) //이미지가 있으면
//	{
//		int m_dark_total = 0;
//		int m_bright_total = 0;
//		Result_Info[Cam_num] = "";
//		for (int s=0;s<5;s++)
//		{
//			if (ALG_Side_Param[Cam_num].nRect_Use[s] == 0 || ALG_Side_Param[Cam_num].nRect[s].width <= 0 || ALG_Side_Param[Cam_num].nRect[s].height <= 0)
//			{
//				continue;
//			}
//			Mat Defect_Dark_Img;
//			Mat Defect_Dark_Img1;
//			Mat Defect_Bright_Img;
//			Mat Defect_Bright_Img1;
//
//			Mat CP_Gray_Img = Gray_Img[Cam_num](ALG_Side_Param[Cam_num].nRect[s]).clone();	
//			//medianBlur(CP_Gray_Img,CP_Gray_Img,3);
//
//			vector<vector<Point> > contours;
//			vector<Vec4i> hierarchy;
//			CString msg;
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),CP_Gray_Img);		
//			}
//
//			// 어두운불량 구해오기
//			dilate(CP_Gray_Img,Defect_Dark_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			erode(Defect_Dark_Img,Defect_Dark_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			subtract(Defect_Dark_Img, CP_Gray_Img, Defect_Dark_Img);
//
//			dilate(CP_Gray_Img,Defect_Dark_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			erode(Defect_Dark_Img1,Defect_Dark_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			subtract(Defect_Dark_Img1, CP_Gray_Img, Defect_Dark_Img1);
//
//			add(Defect_Dark_Img,Defect_Dark_Img1,Defect_Dark_Img);
//
//			threshold(Defect_Dark_Img,Defect_Dark_Img,ALG_Side_Param[Cam_num].nDiffDarkThreshold[s],255,CV_THRESH_BINARY);
//			dilate(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),1);
//			erode(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),2);
//			dilate(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),1);
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_THDark_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),Defect_Dark_Img);		
//			}	
//
//			// 밝은불량 구해오기
//			erode(CP_Gray_Img,Defect_Bright_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			subtract(CP_Gray_Img, Defect_Bright_Img, Defect_Bright_Img);
//
//			erode(CP_Gray_Img,Defect_Bright_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			dilate(Defect_Bright_Img1,Defect_Bright_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			subtract(CP_Gray_Img, Defect_Bright_Img1, Defect_Bright_Img1);
//
//			add(Defect_Bright_Img,Defect_Bright_Img1,Defect_Bright_Img);
//
//			threshold(Defect_Bright_Img,Defect_Bright_Img,ALG_Side_Param[Cam_num].nDiffBrightThreshold[s],255,CV_THRESH_BINARY);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),1);
//			erode(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),2);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),1);
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_THBright_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),Defect_Bright_Img);		
//			}
//
//			//vector<int> Defect_Size;
//			Rect rect;
//			int m_area = 0;
//			// 칸투어 이용하여 검사영역 이미지 잘라옴.
//			findContours( Defect_Dark_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				rect = boundingRect(contours[k]);
//				m_area = rect.width*rect.height;
//				if (m_area > ALG_Side_Param[Cam_num].nDefectDarkMinSize[s])
//				{
//					m_dark_total+=m_area;
//					//Defect_Size.push_back(m_area);
//					if (m_Text_View[Cam_num])
//					{
//						drawContours( Dst_Img[Cam_num](ALG_Side_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,0,255), CV_FILLED, 8, hierarchy);
//						msg.Format("%d",m_area);
//						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x+rect.x,ALG_Side_Param[Cam_num].nRect[s].y+rect.y-5), fontFace, fontScale, CV_RGB(0,100,255), thickness, 8);
//					}
//				}
//			}
//			findContours( Defect_Bright_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				rect = boundingRect(contours[k]);
//				m_area = rect.width*rect.height;
//				if (m_area > ALG_Side_Param[Cam_num].nDefectBrightMinSize[s])
//				{
//					m_bright_total+=m_area;
//					//Defect_Size.push_back(m_area);
//					if (m_Text_View[Cam_num])
//					{
//						drawContours( Dst_Img[Cam_num](ALG_Side_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
//						msg.Format("%d",m_area);
//						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x+rect.x,ALG_Side_Param[Cam_num].nRect[s].y+rect.y-5), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//					}
//				}
//			}
//			if (m_Text_View[Cam_num])
//			{
//				rectangle(Dst_Img[Cam_num],ALG_Side_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x-30,ALG_Side_Param[Cam_num].nRect[s].y + 6), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(0,0,255), 1, 8);
//				msg.Format("(%d)", m_dark_total+m_bright_total);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x-30,ALG_Side_Param[Cam_num].nRect[s].y + 18), FONT_HERSHEY_SIMPLEX, 0.3, CV_RGB(220,30,30), 1, 8);
//			}
//		}
//		Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d", Cam_num, m_dark_total+m_bright_total, Cam_num, m_dark_total, Cam_num, m_bright_total);
//	}
//	return true;
//}
//
//
//bool CImPro_Library::RUN_Algorithm_CAM0()
//{
//	bool RTN = true;
//	if (m_Alg_Type <= 2)
//	{
//		RTN = RUN_Algorithm_Side(0);
//	} 
//	else if (m_Alg_Type == 3)
//	{
//		RTN = RUN_Algorithm_Top(0);
//	}
//	else if (m_Alg_Type == 4)
//	{
//		RTN = RUN_Algorithm_Bottom(0);
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_Algorithm_CAM1()
//{
//	bool RTN = true;
//	if (m_Alg_Type <= 2)
//	{
//		RTN = RUN_Algorithm_Side(1);
//	} 
//	else if (m_Alg_Type == 3)
//	{
//		RTN = RUN_Algorithm_Top(1);
//	}
//	else if (m_Alg_Type == 4)
//	{
//		RTN = RUN_Algorithm_Bottom(1);
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_Algorithm_CAM2()
//{
//	bool RTN = true;
//	if (m_Alg_Type <= 2)
//	{
//		RTN = RUN_Algorithm_Side(2);
//	} 
//	else if (m_Alg_Type == 3)
//	{
//		RTN = RUN_Algorithm_Top(2);
//	}
//	else if (m_Alg_Type == 4)
//	{
//		RTN = RUN_Algorithm_Bottom(2);
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_MissedAlgorithm_CAM0()
//{
//	bool RTN = true;
//	if (m_Alg_Type <= 2)
//	{
//		RTN = RUN_MissedAlgorithm_Side(0);
//	} 
//	else if (m_Alg_Type == 3)
//	{
//		RTN = RUN_MissedAlgorithm_Top(0);
//	}
//	else if (m_Alg_Type == 4)
//	{
//		RTN = RUN_MissedAlgorithm_Bottom(0);
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_MissedAlgorithm_CAM1()
//{
//	bool RTN = true;
//	if (m_Alg_Type <= 2)
//	{
//		RTN = RUN_MissedAlgorithm_Side(1);
//	} 
//	else if (m_Alg_Type == 3)
//	{
//		RTN = RUN_MissedAlgorithm_Top(1);
//	}
//	else if (m_Alg_Type == 4)
//	{
//		RTN = RUN_MissedAlgorithm_Bottom(1);
//	}
//	return RTN;
//}
//
//bool CImPro_Library::RUN_MissedAlgorithm_CAM2()
//{
//	bool RTN = true;
//	if (m_Alg_Type <= 2)
//	{
//		RTN = RUN_MissedAlgorithm_Side(2);
//	} 
//	else if (m_Alg_Type == 3)
//	{
//		RTN = RUN_MissedAlgorithm_Top(2);
//	}
//	else if (m_Alg_Type == 4)
//	{
//		RTN = RUN_MissedAlgorithm_Bottom(2);
//	}
//	return RTN;
//}
//
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//bool CImPro_Library::RUN_Algorithm_Side(int Cam_num)
//{
//	if (!Gray_Img[Cam_num].empty()) //이미지가 있으면
//	{
//		int m_dark_total = 0;
//		int m_bright_total = 0;
//		Result_Info[Cam_num] = "";
//		for (int s=0;s<10;s++)
//		{
//			if (ALG_Side_Param[Cam_num].nRect_Use[s] == 0 || ALG_Side_Param[Cam_num].nRect[s].width <= 0 || ALG_Side_Param[Cam_num].nRect[s].height <= 0)
//			{
//				continue;
//			}
//			Mat Defect_Dark_Img;
//			Mat Defect_Dark_Img1;
//			Mat Defect_Bright_Img;
//			Mat Defect_Bright_Img1;
//
//			Mat CP_Gray_Img = Gray_Img[Cam_num](ALG_Side_Param[Cam_num].nRect[s]).clone();	
//			//medianBlur(CP_Gray_Img,CP_Gray_Img,3);
//
//			vector<vector<Point> > contours;
//			vector<Vec4i> hierarchy;
//			CString msg;
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),CP_Gray_Img);		
//			}
//
//			// 어두운불량 구해오기
//			dilate(CP_Gray_Img,Defect_Dark_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			erode(Defect_Dark_Img,Defect_Dark_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			subtract(Defect_Dark_Img, CP_Gray_Img, Defect_Dark_Img);
//
//			dilate(CP_Gray_Img,Defect_Dark_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			erode(Defect_Dark_Img1,Defect_Dark_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			subtract(Defect_Dark_Img1, CP_Gray_Img, Defect_Dark_Img1);
//
//			add(Defect_Dark_Img,Defect_Dark_Img1,Defect_Dark_Img);
//
//			threshold(Defect_Dark_Img,Defect_Dark_Img,ALG_Side_Param[Cam_num].nDiffDarkThreshold[s],255,CV_THRESH_BINARY);
//			dilate(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),1);
//			erode(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),2);
//			dilate(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),1);
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_THDark_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),Defect_Dark_Img);		
//			}	
//
//			// 밝은불량 구해오기
//			erode(CP_Gray_Img,Defect_Bright_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			subtract(CP_Gray_Img, Defect_Bright_Img, Defect_Bright_Img);
//
//			erode(CP_Gray_Img,Defect_Bright_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			dilate(Defect_Bright_Img1,Defect_Bright_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			subtract(CP_Gray_Img, Defect_Bright_Img1, Defect_Bright_Img1);
//
//			add(Defect_Bright_Img,Defect_Bright_Img1,Defect_Bright_Img);
//
//			threshold(Defect_Bright_Img,Defect_Bright_Img,ALG_Side_Param[Cam_num].nDiffBrightThreshold[s],255,CV_THRESH_BINARY);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),1);
//			erode(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),2);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),1);
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_THBright_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),Defect_Bright_Img);		
//			}
//
//			//vector<int> Defect_Size;
//			Rect rect;
//			int m_area = 0;
//			// 칸투어 이용하여 검사영역 이미지 잘라옴.
//			findContours( Defect_Dark_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				rect = boundingRect(contours[k]);
//				m_area = rect.width*rect.height;
//				if (m_area > ALG_Side_Param[Cam_num].nDefectDarkMinSize[s])
//				{
//					m_dark_total+=m_area;
//					//Defect_Size.push_back(m_area);
//					if (m_Text_View[Cam_num])
//					{
//						drawContours( Dst_Img[Cam_num](ALG_Side_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,0,255), CV_FILLED, 8, hierarchy);
//						msg.Format("%d",m_area);
//						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x+rect.x,ALG_Side_Param[Cam_num].nRect[s].y+rect.y-5), fontFace, fontScale, CV_RGB(0,100,255), thickness, 8);
//					}
//				}
//			}
//			findContours( Defect_Bright_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				rect = boundingRect(contours[k]);
//				m_area = rect.width*rect.height;
//				if (m_area > ALG_Side_Param[Cam_num].nDefectBrightMinSize[s])
//				{
//					m_bright_total+=m_area;
//					//Defect_Size.push_back(m_area);
//					if (m_Text_View[Cam_num])
//					{
//						drawContours( Dst_Img[Cam_num](ALG_Side_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
//						msg.Format("%d",m_area);
//						putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x+rect.x,ALG_Side_Param[Cam_num].nRect[s].y+rect.y-5), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//					}
//				}
//			}
//			if (m_Text_View[Cam_num])
//			{
//				rectangle(Dst_Img[Cam_num],ALG_Side_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x-40,ALG_Side_Param[Cam_num].nRect[s].y + 6), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,0,255), 1, 8);
//				msg.Format("(%d)", m_dark_total+m_bright_total);
//				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x-40,ALG_Side_Param[Cam_num].nRect[s].y + 18), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(220,30,30), 1, 8);
//			}
//			Result_Info[Cam_num].Format("%s_C%d:0%d_%d", Result_Info[Cam_num], Cam_num, s, m_dark_total+m_bright_total);
//		}
//		//Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d", Cam_num, m_dark_total+m_bright_total, Cam_num, m_dark_total, Cam_num, m_bright_total);
//	}
//	return true;
//}
//
//bool CImPro_Library::RUN_Algorithm_Top(int Cam_num)
//{
//	Result_Info[Cam_num] = "";
//	for (int s=0;s<10;s++)
//	{
//			Result_Info[Cam_num].Format("%s_C%d:0%d_%d", Result_Info[Cam_num], Cam_num, s, -1);
//	}
//	return true;
//}
//
//bool CImPro_Library::RUN_Algorithm_Bottom(int Cam_num)
//{
//	Result_Info[Cam_num] = "";
//	for (int s=0;s<10;s++)
//	{
//			Result_Info[Cam_num].Format("%s_C%d:0%d_%d", Result_Info[Cam_num], Cam_num, s, -1);
//	}
//	return true;
//}
//
//
//
//bool CImPro_Library::RUN_MissedAlgorithm_Side(int Cam_num)
//{
//	if (!Gray_MissedImg[Cam_num].empty()) //이미지가 있으면
//	{
//		int m_dark_total = 0;
//		int m_bright_total = 0;
//		Result_Info[Cam_num] = "";
//		for (int s=0;s<10;s++)
//		{
//			if (ALG_Side_Param[Cam_num].nRect_Use[s] == 0 || ALG_Side_Param[Cam_num].nRect[s].width <= 0 || ALG_Side_Param[Cam_num].nRect[s].height <= 0)
//			{
//				continue;
//			}
//			Mat Defect_Dark_Img;
//			Mat Defect_Dark_Img1;
//			Mat Defect_Bright_Img;
//			Mat Defect_Bright_Img1;
//
//			Mat CP_Gray_Img = Gray_MissedImg[Cam_num](ALG_Side_Param[Cam_num].nRect[s]).clone();	
//			//medianBlur(CP_Gray_Img,CP_Gray_Img,3);
//
//			vector<vector<Point> > contours;
//			vector<Vec4i> hierarchy;
//			CString msg;
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),CP_Gray_Img);		
//			}
//
//			// 어두운불량 구해오기
//			dilate(CP_Gray_Img,Defect_Dark_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			erode(Defect_Dark_Img,Defect_Dark_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			subtract(Defect_Dark_Img, CP_Gray_Img, Defect_Dark_Img);
//
//			dilate(CP_Gray_Img,Defect_Dark_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			erode(Defect_Dark_Img1,Defect_Dark_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			subtract(Defect_Dark_Img1, CP_Gray_Img, Defect_Dark_Img1);
//
//			add(Defect_Dark_Img,Defect_Dark_Img1,Defect_Dark_Img);
//
//			threshold(Defect_Dark_Img,Defect_Dark_Img,ALG_Side_Param[Cam_num].nDiffDarkThreshold[s],255,CV_THRESH_BINARY);
//			dilate(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),1);
//			erode(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),2);
//			dilate(Defect_Dark_Img,Defect_Dark_Img,element,Point(-1,-1),1);
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_THDark_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),Defect_Dark_Img);		
//			}	
//
//			// 밝은불량 구해오기
//			erode(CP_Gray_Img,Defect_Bright_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element_h,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]);
//			subtract(CP_Gray_Img, Defect_Bright_Img, Defect_Bright_Img);
//
//			erode(CP_Gray_Img,Defect_Bright_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			dilate(Defect_Bright_Img1,Defect_Bright_Img1,element_v,Point(-1,-1),ALG_Side_Param[Cam_num].nMPfilterSize[s]/2);
//			subtract(CP_Gray_Img, Defect_Bright_Img1, Defect_Bright_Img1);
//
//			add(Defect_Bright_Img,Defect_Bright_Img1,Defect_Bright_Img);
//
//			threshold(Defect_Bright_Img,Defect_Bright_Img,ALG_Side_Param[Cam_num].nDiffBrightThreshold[s],255,CV_THRESH_BINARY);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),1);
//			erode(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),2);
//			dilate(Defect_Bright_Img,Defect_Bright_Img,element,Point(-1,-1),1);
//			if (Result_Debugging)
//			{
//				msg.Format("Save\\Debugging\\Cam%d_%2d_ROI_THBright_Img.bmp",Cam_num, s);
//				imwrite(msg.GetBuffer(),Defect_Bright_Img);		
//			}
//
//			//vector<int> Defect_Size;
//			Rect rect;
//			int m_area = 0;
//			// 칸투어 이용하여 검사영역 이미지 잘라옴.
//			findContours( Defect_Dark_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				rect = boundingRect(contours[k]);
//				m_area = rect.width*rect.height;
//				if (m_area > ALG_Side_Param[Cam_num].nDefectDarkMinSize[s])
//				{
//					m_dark_total+=m_area;
//					//Defect_Size.push_back(m_area);
//					if (m_Text_View[Cam_num])
//					{
//						drawContours( Dst_MissedImg[Cam_num](ALG_Side_Param[Cam_num].nRect[s]), contours, k, CV_RGB(0,0,255), CV_FILLED, 8, hierarchy);
//						msg.Format("%d",m_area);
//						putText(Dst_MissedImg[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x+rect.x,ALG_Side_Param[Cam_num].nRect[s].y+rect.y-5), fontFace, fontScale, CV_RGB(0,100,255), thickness, 8);
//					}
//				}
//			}
//			findContours( Defect_Bright_Img.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
//
//			for( int k = 0; k < contours.size(); k++ )
//			{  
//				rect = boundingRect(contours[k]);
//				m_area = rect.width*rect.height;
//				if (m_area > ALG_Side_Param[Cam_num].nDefectBrightMinSize[s])
//				{
//					m_bright_total+=m_area;
//					//Defect_Size.push_back(m_area);
//					if (m_Text_View[Cam_num])
//					{
//						drawContours( Dst_MissedImg[Cam_num](ALG_Side_Param[Cam_num].nRect[s]), contours, k, CV_RGB(255,0,0), CV_FILLED, 8, hierarchy);
//						msg.Format("%d",m_area);
//						putText(Dst_MissedImg[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x+rect.x,ALG_Side_Param[Cam_num].nRect[s].y+rect.y-5), fontFace, fontScale, CV_RGB(255,100,0), thickness, 8);
//					}
//				}
//			}
//			if (m_Text_View[Cam_num])
//			{
//				rectangle(Dst_MissedImg[Cam_num],ALG_Side_Param[Cam_num].nRect[s],CV_RGB(0,0,255),1);
//				msg.Format("ROI#%d", s + 1);
//				putText(Dst_MissedImg[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x-40,ALG_Side_Param[Cam_num].nRect[s].y + 6), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,0,255), 1, 8);
//				msg.Format("(%d)", m_dark_total+m_bright_total);
//				putText(Dst_MissedImg[Cam_num], msg.GetBuffer(), Point(ALG_Side_Param[Cam_num].nRect[s].x-40,ALG_Side_Param[Cam_num].nRect[s].y + 18), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(220,30,30), 1, 8);
//			}
//			Result_MissedInfo[Cam_num].Format("%s_C%d:0%d_%d", Result_MissedInfo[Cam_num], Cam_num, s, m_dark_total+m_bright_total);
//		}
//		//Result_Info[Cam_num].Format("C%d:00_%d_C%d:01_%d_C%d:02_%d", Cam_num, m_dark_total+m_bright_total, Cam_num, m_dark_total, Cam_num, m_bright_total);
//	}
//	return true;
//}
//
//bool CImPro_Library::RUN_MissedAlgorithm_Top(int Cam_num)
//{
//	Result_MissedInfo[Cam_num] = "";
//	for (int s=0;s<10;s++)
//	{
//			Result_MissedInfo[Cam_num].Format("%s_C%d:0%d_%d", Result_MissedInfo[Cam_num], Cam_num, s, -1);
//	}
//	return true;
//}
//
//bool CImPro_Library::RUN_MissedAlgorithm_Bottom(int Cam_num)
//{
//	Result_MissedInfo[Cam_num] = "";
//	for (int s=0;s<10;s++)
//	{
//			Result_MissedInfo[Cam_num].Format("%s_C%d:0%d_%d", Result_MissedInfo[Cam_num], Cam_num, s, -1);
//	}
//	return true;
//}

bool CImPro_Library::RUN_Algorithm_BOLTPIN()
{
	int Cam_num = 0;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		for (int s=0;s<12;s++)
		{
			if (ALG_BOLTPIN_Param.nUse[s] == 0 || ALG_BOLTPIN_Param.nRect[s].width <= 0 || ALG_BOLTPIN_Param.nRect[s].height <= 0)
			{
				//CString msg;
				//msg.Format("%d, w=%d,h=%d,use=%d",s, ALG_BOLTPIN_Param.nRect[s].width, ALG_BOLTPIN_Param.nRect[s].height, ALG_BOLTPIN_Param.nUse[s]);
				//AfxMessageBox(msg.GetBuffer());	
				if (s > 1)
				{
					Result_Info[Cam_num].Format("%sC0:0%d_-2_",Result_Info[Cam_num],s-2);
				}
				continue;
			}

			vector<vector<Point> > contours;
			vector<Vec4i> hierarchy;
			CString msg;

			// ROI 이미지 Crop
			Mat Out_binary;
			Mat Out_binary_Tmp;
			Mat CP_Gray_Img;
			Rect tROI = ALG_BOLTPIN_Param.nRect[s];
			int top_x_offset = 0;
			int top_y_offset = 0;
			if (s == 0)
			{
				CP_Gray_Img = Gray_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]).clone();
			} else
			{
				ALG_BOLTPIN_Param.nRect[s].x += ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.x;
				ALG_BOLTPIN_Param.nRect[s].y += ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.y;
				tROI = ALG_BOLTPIN_Param.nRect[s];
				tROI.x = tROI.x - ALG_BOLTPIN_Param.nRect[0].x;
				tROI.y = tROI.y - ALG_BOLTPIN_Param.nRect[0].y;

				if (tROI.x < 0)
				{
					top_x_offset = -tROI.x;
					tROI.x = 0;
				}
				if (tROI.y < 0)
				{
					top_y_offset = -tROI.y;
					tROI.y = 0;
				}
				if (tROI.x + tROI.width >= ALG_BOLTPIN_Param.Gray_ROI00_Img.cols)
				{
					tROI.width = ALG_BOLTPIN_Param.Gray_ROI00_Img.cols - tROI.x -1;
				}
				if (tROI.y + tROI.height >= ALG_BOLTPIN_Param.Gray_ROI00_Img.rows)
				{
					tROI.height = ALG_BOLTPIN_Param.Gray_ROI00_Img.rows - tROI.y -1;
				}
				ALG_BOLTPIN_Param.nRect[s].x += top_x_offset;
				ALG_BOLTPIN_Param.nRect[s].y += top_y_offset;
				//msg.Format("ROI:(%d,%d,%d,%d)",tROI.x,tROI.y,tROI.width,tROI.height);
				//AfxMessageBox(msg);
				CP_Gray_Img = ALG_BOLTPIN_Param.Gray_ROI00_Img(tROI).clone();
			}

			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\%2d_ROI_Gray_Img.bmp",s+1);
				imwrite(msg.GetBuffer(),CP_Gray_Img);		
			}

			if (m_Text_View[Cam_num] && s <= 1)
			{
				rectangle(Dst_Img[Cam_num],ALG_BOLTPIN_Param.nRect[s],CV_RGB(0,255,50),1);
				msg.Format("ROI#%d", s + 1);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_BOLTPIN_Param.nRect[s].x+1,ALG_BOLTPIN_Param.nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,255,50), 1, 8);
			}

			// 임계화
			// 임계화 방법 0:이하, 1:이상, 2:사이, 3:자동이하, 4:자동이상, 5:에지, 6:ROI#01 결과 사용
			if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::BINARY_INV) // V1이하
			{
				threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V1[s],255,CV_THRESH_BINARY_INV);
			} 
			else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::BINARY) // V2이상
			{
				threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY);
			} 
			else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::BINARY_BETWEEN) // V1~V2사이
			{
				threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V1[s],255,CV_THRESH_BINARY);
				threshold(CP_Gray_Img,Out_binary_Tmp,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY_INV);
				bitwise_and(Out_binary,Out_binary_Tmp,Out_binary);
			}
			else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::BINARY_INV_OTSU) // 자동이하
			{
				threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
			}
			else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::BINARY_OTSU) // 자동이상
			{
				threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
			}
			else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::EDGE) // 에지검출
			{
				blur(CP_Gray_Img, Out_binary, Size(3,3));
				// Run the edge detector on grayscale
				Canny(Out_binary, Out_binary, ALG_BOLTPIN_Param.nThres_V1[s], ALG_BOLTPIN_Param.nThres_V2[s], 3);

				//if (Result_Text_View)
				//{
				//	 findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
				//	 for(int k = 0;k < contours.size(); k++)
				//	 {
				//	 	drawContours(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
				//	 }
				//}
			}
			else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == THRES_METHOD::FIRSTROI) // ROI#01 결과 사용
			{
				Out_binary = ALG_BOLTPIN_Param.Thres_ROI00_Img(tROI).clone();
			}

			if (Result_Debugging)
			{
				msg.Format("Save\\Debugging\\%2d_Binary_Img.bmp",s+1);
				imwrite(msg.GetBuffer(),Out_binary);
			}

			if (s == 0)
			{
				if (ALG_BOLTPIN_Param.nMethod_Thres[s] != 5) // 에지검출
				{
					erode(Out_binary,Out_binary,element_v,Point(-1,-1),10);
					dilate(Out_binary,Out_binary,element_v,Point(-1,-1),10);
				}
				// 사이드 바닦 찾기
				double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;
				int t_Search_Range = 10;
				for (int i = 0;i<t_Search_Range;i++)
				{
					for (int j = Out_binary.rows/2;j<Out_binary.rows;j++)
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
					for (int j = Out_binary.rows/2;j<Out_binary.rows;j++)
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

				findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

				int m_max_object_num = -1;int m_max_object_value = 0;
				Rect t_Side_rect;

				for( int k = 0; k < contours.size(); k++ )
				{  
					t_Side_rect = boundingRect( Mat(contours[k]) );
					if (m_max_object_value <= t_Side_rect.width*t_Side_rect.height && t_Side_rect.x > 1)
					{
						m_max_object_value = t_Side_rect.width*t_Side_rect.height;
						m_max_object_num = k;
					}
				}

				if (m_max_object_num > 0)
				{
					t_Side_rect = boundingRect(Mat(contours[m_max_object_num]));

					if ( abs(ALG_BOLTPIN_Param.ROI01_Height - t_Side_rect.height) > ALG_BOLTPIN_Param.ROI01_Height/3)
					{
						Result_Info[Cam_num].Format("C0:00_-1_C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C8:00_-1");
						return true;
					}
					ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.x = t_Side_rect.x - ALG_BOLTPIN_Param.ROI01_Object_LEFTTOP.x;
					ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.y = t_Side_rect.y - ALG_BOLTPIN_Param.ROI01_Object_LEFTTOP.y;
					if (m_Text_View[Cam_num])
					{
						drawContours( Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), contours, m_max_object_num, CV_RGB(0,255,50), 1, CV_AA, hierarchy);
					}
				}

				for (int i=1;i<12;i++)
				{
					if (ALG_BOLTPIN_Param.nMethod_Thres[i] == 6)
					{
						Mat Target_Thres_ROI_Gray_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
						if (m_max_object_num > 0)
						{
							drawContours( Target_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
							ALG_BOLTPIN_Param.Thres_ROI00_Img = Out_binary.clone();
							ALG_BOLTPIN_Param.Gray_ROI00_Img = Gray_Img[Cam_num](ALG_BOLTPIN_Param.nRect[0]).clone();
							bitwise_and(ALG_BOLTPIN_Param.Gray_ROI00_Img,255-Target_Thres_ROI_Gray_Img,ALG_BOLTPIN_Param.Gray_ROI00_Img);
							bitwise_and(ALG_BOLTPIN_Param.Thres_ROI00_Img,Target_Thres_ROI_Gray_Img,ALG_BOLTPIN_Param.Thres_ROI00_Img);
						} else
						{
							ALG_BOLTPIN_Param.Thres_ROI00_Img = Out_binary.clone();
							ALG_BOLTPIN_Param.Gray_ROI00_Img = Gray_Img[Cam_num](ALG_BOLTPIN_Param.nRect[0]).clone();
						}

						if (Result_Debugging)
						{
							msg.Format("Save\\Debugging\\%2d_Gray_ROI00_Img.bmp",s+1);
							imwrite(msg.GetBuffer(),ALG_BOLTPIN_Param.Gray_ROI00_Img);
							msg.Format("Save\\Debugging\\%2d_Thres_ROI00_Img.bmp",s+1);
							imwrite(msg.GetBuffer(),ALG_BOLTPIN_Param.Thres_ROI00_Img);
						}
						break;
					}
				}
				continue;
			}

			//msg.Format("%d",s);
			//AfxMessageBox(msg);

			if (s == 1 && !ALG_BOLTPIN_Param.ROI02_Mirror_Check)
			{
				bool t_mirror_check = Object_Mirror_Find(Out_binary);
				ALG_BOLTPIN_Param.ROI02_Mirror_Check = true;
				if (t_mirror_check)
				{
					ALG_BOLTPIN_Param.nRect[s].x -= (ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.x + top_x_offset);
					ALG_BOLTPIN_Param.nRect[s].y -= (ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.y + top_y_offset);
					Reset_Dst_Image(0);
					return RUN_Algorithm_BOLTPIN();
				}
			}

			// 에지 찾기
			// 측정 방향 0:가로방향 길이, 1:세로방향 길이, 2:나사선 크기, 3:나사선 피치, 4:머리 높이
			int start_p = -1;
			int end_p = -1;
			vector<double> dist_vec;
			if (ALG_BOLTPIN_Param.nMethod_Direc[s] == ALGORITHM::W_LENGTH) // 가로방향 길이
			{
				for(int i=0;i<Out_binary.rows;i++)
				{
					for(int j=0;j<Out_binary.cols;j++)
					{
						if (Out_binary.at<uchar>(i,j) == 255 && start_p == -1)
						{
							start_p = j;
							break;
						}
					}
					for(int j=Out_binary.cols-1;j>=0;j--)
					{
						if (Out_binary.at<uchar>(i,j) == 255 && end_p == -1)
						{
							end_p = j;
							break;
						}
					}
					if (start_p != -1 && end_p != -1)
					{
						dist_vec.push_back((double)(end_p - start_p)*ALG_BOLTPIN_Param.nResolution[0]);
						if (Result_Text_View)
						{
							circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), Point(start_p,i),1,CV_RGB(255,0,0),1);
							circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), Point(end_p,i),1,CV_RGB(255,0,0),1);
						}
					}
					start_p = -1;end_p = -1;
				}
			} 
			else if (ALG_BOLTPIN_Param.nMethod_Direc[s] == ALGORITHM::H_LENGTH) // 세로방향 길이
			{
				for(int j=0;j<Out_binary.cols;j++)
				{
					for(int i=0;i<Out_binary.rows;i++)
					{
						if (Out_binary.at<uchar>(i,j) == 255 && start_p == -1)
						{
							start_p = i;
							break;
						}
					}
					for(int i=Out_binary.rows-1;i>=0;i--)
					{
						if (Out_binary.at<uchar>(i,j) == 255 && end_p == -1)
						{
							end_p = i;
							break;
						}
					}
					if (start_p != -1 && end_p != -1)
					{
						dist_vec.push_back((double)(end_p - start_p)*ALG_BOLTPIN_Param.nResolution[1]);
						if (Result_Text_View)
						{
							circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), Point(j,start_p),1,CV_RGB(255,0,0),1);
							circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), Point(j,end_p),1,CV_RGB(255,0,0),1);
						}
					}
					start_p = -1;end_p = -1;
				}
			} 
			else if (ALG_BOLTPIN_Param.nMethod_Direc[s] == ALGORITHM::SIZE_CONCHOID) // 나사선 크기
			{
				Mat Morph_Out_binary;
				erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),tROI.width/4);
				dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),tROI.width/4);
				subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
				erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),5);
				dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),5);

				findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

				if (contours.size() == 0)
				{
					dist_vec.push_back(0);
					continue;
				}

				vector<Moments> mu(contours.size());
				Rect boundRect;

				for( int k = 0; k < contours.size(); k++ )
				{  
					mu[k] = moments( contours[k], false ); 
					boundRect = boundingRect( Mat(contours[k]) );
					if (boundRect.x > 1 && boundRect.x + boundRect.width < Morph_Out_binary.cols-2)
					{
						if (Result_Text_View)
						{
							rectangle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), boundRect,CV_RGB(255,150,0),1);
							rectangle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), boundRect,CV_RGB(255,155,0),1);
						}
						dist_vec.push_back(mu[k].m00);
					}
				}
			} 
			else if (ALG_BOLTPIN_Param.nMethod_Direc[s] == ALGORITHM::PITCH_CONCHOID) // 나사선 피치
			{
				Mat Morph_Out_binary;
				erode(Out_binary,Morph_Out_binary,element_h,Point(-1,-1),tROI.width/4);
				dilate(Morph_Out_binary,Morph_Out_binary,element_h,Point(-1,-1),tROI.width/4);
				subtract(Out_binary,Morph_Out_binary,Morph_Out_binary);
				erode(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),5);
				dilate(Morph_Out_binary,Morph_Out_binary,element_v,Point(-1,-1),5);

				findContours( Morph_Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

				if (contours.size() == 0)
				{
					dist_vec.push_back(0);
					continue;
				}

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
						if (boundRect.y > 1)
						{
							t_Point4D.IDX = (double)k;
							t_Point4D.CX = mu[k].m10/mu[k].m00;
							t_Point4D.CY = mu[k].m01/mu[k].m00;
							t_Point4D.AREA = mu[k].m00;
							t_Point4D.ROI = boundRect;
							Pitch_Info.push_back(t_Point4D);
						}

						if (Result_Text_View)
						{
							rectangle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), boundRect,CV_RGB(255,150,0),1);
							rectangle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), boundRect,CV_RGB(255,150,0),1);
						}
						dist_vec.push_back(mu[k].m00);
					}
				}
				std::sort(Pitch_Info.begin(), Pitch_Info.end(), Point_compare_Size);
				if (Pitch_Info.size() > 0)
				{
					for (int i = 0;i < Pitch_Info.size()-1;i++)
					{
						dist_vec.push_back((Pitch_Info[i+1].CX - Pitch_Info[i].CX)*ALG_BOLTPIN_Param.nResolution[0]);
						if (m_Text_View[Cam_num])
						{
							//boundRect = Pitch_Info[i].ROI;
							//boundRect.x--;boundRect.y--;boundRect.width+=2;boundRect.height+=2;
							//rectangle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), boundRect,CV_RGB(0,0,255),1);
							circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]),Point(Pitch_Info[i].CX,Pitch_Info[i].CY),1,CV_RGB(255,100,0),1);
						}
					}
				}
			} 
			else if (ALG_BOLTPIN_Param.nMethod_Direc[s] == ALGORITHM::HEAD_HEIGHT) // 머리 높이
			{
				//dilate(Out_binary,Out_binary,element,Point(-1,-1),1);
				//erode(Out_binary,Out_binary,element,Point(-1,-1),1);
				////J_Fill_Hole(Out_binary);
				//double circle_info[5] = {0,};
				//findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
				//int m_max_object_num = -1;int m_max_object_value = 0;
				//Rect boundRect; double rx = 0; double ry = 0;
				//for( int k = 0; k < contours.size(); k++ )
				//{  
				//	boundRect = boundingRect( Mat(contours[k]) );
				//	rx = (double)boundRect.width/(double)boundRect.height;
				//	ry = (double)boundRect.height/(double)boundRect.width;
				//	if (m_max_object_value <= boundRect.width*boundRect.height
				//		//&& boundRect.x > 2 && boundRect.y > 2 && boundRect.x+boundRect.width < Out_binary.cols -3 && boundRect.y+boundRect.height < Out_binary.rows -3
				//		&& min(rx,ry) > 0.7)
				//	{
				//		m_max_object_value = boundRect.width*boundRect.height;
				//		m_max_object_num = k;
				//	}
				//}

				//if (m_max_object_num != -1)
				//{
				//	boundRect = boundingRect( Mat(contours[m_max_object_num]) );
				//	circle_info[0] = (double)boundRect.x;
				//	circle_info[1] = (double)boundRect.y;
				//	circle_info[2] = (double)boundRect.width;
				//	circle_info[3] = (double)boundRect.height;
				//	ALG_BOLTPIN_Param.Point_Cx = (double)ALG_BOLTPIN_Param.nRect[s].x + circle_info[0] + circle_info[2]/2;
				//	ALG_BOLTPIN_Param.Point_Cy = (double)ALG_BOLTPIN_Param.nRect[s].y + circle_info[1] + circle_info[3]/2;
				//	dist_vec.push_back(circle_info[3]*ALG_BOLTPIN_Param.nResolution[1]);
				//} else
				//{
				//	circle_info[0] = (double)ALG_BOLTPIN_Param.nRect[s].x;
				//	circle_info[1] = (double)ALG_BOLTPIN_Param.nRect[s].y;
				//	circle_info[2] = (double)ALG_BOLTPIN_Param.nRect[s].width;
				//	circle_info[3] = (double)ALG_BOLTPIN_Param.nRect[s].height;
				//	ALG_BOLTPIN_Param.Point_Cx = (double)ALG_BOLTPIN_Param.nRect[s].x + (double)ALG_BOLTPIN_Param.nRect[s].width/2;
				//	ALG_BOLTPIN_Param.Point_Cy = (double)ALG_BOLTPIN_Param.nRect[s].y + (double)ALG_BOLTPIN_Param.nRect[s].height/2;
				//	//dist_vec.push_back(0);
				//}

				//if (Result_Text_View)
				//{
				//	//ellipse(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]),Point((int)circle_info[0],(int)circle_info[1]),Size(circle_info[2]/2,circle_info[3]/2),circle_info[4],0,360,CV_RGB(0,255,0),1,1,0);
				//	line(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]),Point(boundRect.x+boundRect.width/2,boundRect.y),Point(boundRect.x+boundRect.width/2,-1+boundRect.y+boundRect.height),CV_RGB(255,0,0),1);
				//	circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]),Point((int)(circle_info[0]+circle_info[2]/2),(int)(circle_info[1]+circle_info[3]/2)),1,CV_RGB(0,255,0),1);
				//	drawContours(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), contours, m_max_object_num, CV_RGB(255,0,0),1, 8, hierarchy);
				//}
			}

			double v_result = 0;
			if (dist_vec.size() == 0)
			{
			}
			else
			{
				// 측정 방법 0:최소, 1:최대, 2:최대-최소, 3:평균, 4:총합
				if (ALG_BOLTPIN_Param.nMethod_Cal[s] == RESULT_METHOD::MIN) // 최소
				{
					double t_v = 99999;
					for (int i=0;i<dist_vec.size();i++)
					{
						if (t_v >= dist_vec[i])
						{
							t_v = dist_vec[i];
						}
					}
					v_result = t_v;
				} 
				else if (ALG_BOLTPIN_Param.nMethod_Cal[s] == RESULT_METHOD::MAX) // 최대
				{
					double t_v = 0;
					for (int i=0;i<dist_vec.size();i++)
					{
						if (t_v <= dist_vec[i])
						{
							t_v = dist_vec[i];
						}
					}
					v_result = t_v;
				}
				else if (ALG_BOLTPIN_Param.nMethod_Cal[s] == RESULT_METHOD::RANGE) // 최대-최소
				{
					double t_v_min = 99999;double t_v_max = 0;
					for (int i=0;i<dist_vec.size();i++)
					{
						if (t_v_min >= dist_vec[i])
						{
							t_v_min = dist_vec[i];
						}
						if (t_v_max <= dist_vec[i])
						{
							t_v_max = dist_vec[i];
						}
					}
					v_result = t_v_max - t_v_min;				
				}
				else if (ALG_BOLTPIN_Param.nMethod_Cal[s] == RESULT_METHOD::AVERAGE) // 평균
				{
					for (int i=0;i<dist_vec.size();i++)
					{
						v_result += dist_vec[i];
					}
					v_result/= (double)dist_vec.size();
				}			
				else if (ALG_BOLTPIN_Param.nMethod_Cal[s] == RESULT_METHOD::SUMOFALL) // 총합
				{
					for (int i=0;i<dist_vec.size();i++)
					{
						v_result += dist_vec[i];
					}
				}	
			}
			
			if (v_result != 0)
			{
				v_result += ALG_BOLTPIN_Param.Offset[s];
			}

			if (m_License_Check == -1)
			{
				Result_Info[Cam_num].Format("%sC0:0%d_-1_",Result_Info[Cam_num],s-2);
			} else
			{
				Result_Info[Cam_num].Format("%sC0:0%d_%1.3f_",Result_Info[Cam_num],s-2,v_result);
			}

			if (m_Text_View[Cam_num] && s > 1)
			{
				rectangle(Dst_Img[Cam_num],ALG_BOLTPIN_Param.nRect[s],CV_RGB(0,0,255),1);
				msg.Format("ROI#%d(%1.3f)", s + 1,v_result);
				putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_BOLTPIN_Param.nRect[s].x+1,ALG_BOLTPIN_Param.nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(255,100,0), 1, 8);
			}

			ALG_BOLTPIN_Param.nRect[s].x -= (ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.x + top_x_offset);
			ALG_BOLTPIN_Param.nRect[s].y -= (ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.y + top_y_offset);
		}
	} else
	{
		Result_Info[Cam_num].Format("C0:00_-1_C0:01_-1_C0:02_-1_C0:03_-1_C0:04_-1_C0:05_-1_C0:06_-1_C0:07_-1_C8:00_-1");
	}
	ALG_BOLTPIN_Param.ROI02_Mirror_Check = false;
	return true;
}

bool CImPro_Library::RUN_MissedAlgorithm_BOLTPIN()
{
	return true;
}

bool CImPro_Library::ROI_Object_Find()
{
	int Cam_num = 0;
	if (!Gray_Img[Cam_num].empty())
	{
		Result_Info[Cam_num] = "";

		int s=0;
		if (ALG_BOLTPIN_Param.nUse[s] == 0 || ALG_BOLTPIN_Param.nRect[s].width <= 0 || ALG_BOLTPIN_Param.nRect[s].height <= 0)
		{
			ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.x = 0;
			ALG_BOLTPIN_Param.ROI01_Offset_LEFTTOP.y = 0;
			return true;
		}

		vector<vector<Point> > contours;
		vector<Vec4i> hierarchy;
		CString msg;

		// ROI 이미지 Crop
		Mat Out_binary;
		Mat Out_binary_Tmp;
		Mat CP_Gray_Img;
		CP_Gray_Img = Gray_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]).clone();

		if (m_Text_View[Cam_num] && s <= 1)
		{
			rectangle(Dst_Img[Cam_num],ALG_BOLTPIN_Param.nRect[s],CV_RGB(0,0,255),1);
			msg.Format("ROI#%d", s + 1);
			putText(Dst_Img[Cam_num], msg.GetBuffer(), Point(ALG_BOLTPIN_Param.nRect[s].x+1,ALG_BOLTPIN_Param.nRect[s].y + 11), FONT_HERSHEY_SIMPLEX, 0.4, CV_RGB(0,0,255), 1, 8);
		}

		// 임계화
		// 임계화 방법 0:이하, 1:이상, 2:사이, 3:자동이하, 4:자동이상, 5:에지, 6:ROI#01 결과 사용
		if (ALG_BOLTPIN_Param.nMethod_Thres[s] == 0) // V1이하
		{
			threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V1[s],255,CV_THRESH_BINARY_INV);
		} 
		else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == 1) // V2이상
		{
			threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY);
		} 
		else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == 2) // V1~V2사이
		{
			threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V1[s],255,CV_THRESH_BINARY);
			threshold(CP_Gray_Img,Out_binary_Tmp,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY_INV);
			bitwise_and(Out_binary,Out_binary_Tmp,Out_binary);
		}
		else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == 3) // 자동이하
		{
			threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY_INV | CV_THRESH_OTSU);
		}
		else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == 4) // 자동이상
		{
			threshold(CP_Gray_Img,Out_binary,ALG_BOLTPIN_Param.nThres_V2[s],255,CV_THRESH_BINARY | CV_THRESH_OTSU);
		}
		else if (ALG_BOLTPIN_Param.nMethod_Thres[s] == 5) // 에지검출
		{
			blur(CP_Gray_Img, Out_binary, Size(3,3));
			// Run the edge detector on grayscale
			Canny(Out_binary, Out_binary, ALG_BOLTPIN_Param.nThres_V1[s], ALG_BOLTPIN_Param.nThres_V2[s], 3);

			//if (Result_Text_View)
			//{
			//	 findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
			//	 for(int k = 0;k < contours.size(); k++)
			//	 {
			//	 	drawContours(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), contours, k, CV_RGB(0,255,0), 1, 1, hierarchy);
			//	 }
			//}
		}

		if (ALG_BOLTPIN_Param.nMethod_Thres[s] != 5) // 에지검출
		{
			erode(Out_binary,Out_binary,element_v,Point(-1,-1),10);
			dilate(Out_binary,Out_binary,element_v,Point(-1,-1),10);
		}
		// 사이드 바닦 찾기
		double t_Left_x = 0;double t_Left_y = 0;double t_cnt = 0;
		int t_Search_Range = 10;
		for (int i = 0;i<t_Search_Range;i++)
		{
			for (int j = Out_binary.rows/2;j<Out_binary.rows;j++)
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
			for (int j = Out_binary.rows/2;j<Out_binary.rows;j++)
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
		//	msg.Format("Save\\Debugging\\Cam%d_%2d_Line_Thres_ROI_Gray_Img.bmp",Cam_num, s+1);
		//	imwrite(msg.GetBuffer(),Out_binary);		
		//}

		erode(Out_binary,Out_binary,element_v,Point(-1,-1),1);
		dilate(Out_binary,Out_binary,element_v,Point(-1,-1),1);

		findContours( Out_binary.clone(), contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

		int m_max_object_num = -1;int m_max_object_value = 0;
		Rect t_Side_rect;

		for( int k = 0; k < contours.size(); k++ )
		{  
			t_Side_rect = boundingRect( Mat(contours[k]) );
			if (m_max_object_value <= t_Side_rect.width*t_Side_rect.height && t_Side_rect.x > 1)
			{
				m_max_object_value = t_Side_rect.width*t_Side_rect.height;
				m_max_object_num = k;
			}
		}

		Mat Target_Thres_ROI_Gray_Img = Mat::zeros(Out_binary.size(), CV_8UC1);
		if (m_max_object_num > 0)
		{
			t_Side_rect = boundingRect(Mat(contours[m_max_object_num]));
			ALG_BOLTPIN_Param.ROI01_Object_LEFTTOP.x = t_Side_rect.x;
			ALG_BOLTPIN_Param.ROI01_Object_LEFTTOP.y = t_Side_rect.y;
			ALG_BOLTPIN_Param.ROI01_Height = t_Side_rect.height;
			drawContours( Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[s]), contours, m_max_object_num, Scalar(0,255,0), CV_FILLED, 8, hierarchy);
			drawContours( Target_Thres_ROI_Gray_Img, contours, m_max_object_num, Scalar(255,255,255), CV_FILLED, 8, hierarchy);
		}

		s=1;
		vector<int> vec_height;
		int t_start_x = 0;int t_end_x = 0;
		for (int i = ALG_BOLTPIN_Param.nRect[s].x-ALG_BOLTPIN_Param.nRect[0].x;i<=ALG_BOLTPIN_Param.nRect[s].x+ALG_BOLTPIN_Param.nRect[s].width-ALG_BOLTPIN_Param.nRect[0].x;i++)
		{
			t_start_x = -1;
			for (int j = ALG_BOLTPIN_Param.nRect[s].y-ALG_BOLTPIN_Param.nRect[0].y;j<ALG_BOLTPIN_Param.nRect[s].y+ALG_BOLTPIN_Param.nRect[s].height-ALG_BOLTPIN_Param.nRect[0].y;j++)
			{
				if (Target_Thres_ROI_Gray_Img.at<uchar>(j,i) > 0)
				{
					if (t_start_x == -1)
					{
						t_start_x = j;
						circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[0]),Point(i,j),1,CV_RGB(255,100,0),1);
						break;
					}
				}
			}
			t_end_x = -1;
			for (int j = ALG_BOLTPIN_Param.nRect[s].y+ALG_BOLTPIN_Param.nRect[s].height-ALG_BOLTPIN_Param.nRect[0].y -1;j>= ALG_BOLTPIN_Param.nRect[s].y-ALG_BOLTPIN_Param.nRect[0].y;j--)
			{
				if (Target_Thres_ROI_Gray_Img.at<uchar>(j,i) > 0)
				{
					if (t_end_x == -1)
					{
						t_end_x = j;
						circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[0]),Point(i,j),1,CV_RGB(255,100,0),1);
						break;
					}
				}
			}
			if (t_start_x > -1 && t_end_x > -1)
			{
				vec_height.push_back(t_end_x - t_start_x + 1);
			}
		}
		if (vec_height.size() > 0 )
		{
			std::sort(vec_height.begin(), vec_height.end(), Point_compare_Increasing);
			ALG_BOLTPIN_Param.ROI02_Min_Height = vec_height[0];
			ALG_BOLTPIN_Param.ROI02_Max_Height = vec_height[vec_height.size()-1];
			//msg.Format("Max=%d, Min=%d",ALG_BOLTPIN_Param.ROI02_Max_Height, ALG_BOLTPIN_Param.ROI02_Min_Height);
			//AfxMessageBox(msg);
		}
	}
	return true;
}


bool CImPro_Library::Object_Mirror_Find(Mat &Binary)
{
	CString msg;
	//if (Result_Debugging)
	//{
	//	msg.Format("Save\\Debugging\\Binary.bmp");
	//	imwrite(msg.GetBuffer(),Binary);
	//}

	int Cam_num = 0;
	if (!Gray_Img[Cam_num].empty())
	{
		vector<int> vec_height;
		int t_start_x = 0;int t_end_x = 0;
		for (int i = 0;i<=Binary.cols;i++)
		{
			t_start_x = -1;
			for (int j = 0;j<Binary.rows;j++)
			{
				if (Binary.at<uchar>(j,i) > 0)
				{
					if (t_start_x == -1)
					{
						t_start_x = j;
						//circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[1]),Point(i,j),1,CV_RGB(255,100,0),1);
						break;
					}
				}
			}
			t_end_x = -1;
			for (int j = Binary.rows -1;j>= 0;j--)
			{
				if (Binary.at<uchar>(j,i) > 0)
				{
					if (t_end_x == -1)
					{
						t_end_x = j;
						//circle(Dst_Img[Cam_num](ALG_BOLTPIN_Param.nRect[0]),Point(i,j),1,CV_RGB(255,100,0),1);
						break;
					}
				}
			}
			if (t_start_x > -1 && t_end_x > -1)
			{
				vec_height.push_back(t_end_x - t_start_x + 1);
			}
		}

		if (vec_height.size() > 0 )
		{
			std::sort(vec_height.begin(), vec_height.end(), Point_compare_Increasing);

			//if (//vec_height[0] >= ALG_BOLTPIN_Param.ROI02_Min_Height - ALG_BOLTPIN_Param.ROI02_MinMax_Margin
			//	//&& vec_height[0] <= ALG_BOLTPIN_Param.ROI02_Min_Height + ALG_BOLTPIN_Param.ROI02_MinMax_Margin
			//	vec_height[vec_height.size()-1] >= ALG_BOLTPIN_Param.ROI02_Max_Height - ALG_BOLTPIN_Param.ROI02_MinMax_Margin
			//	&& vec_height[vec_height.size()-1] <= ALG_BOLTPIN_Param.ROI02_Max_Height + ALG_BOLTPIN_Param.ROI02_MinMax_Margin
			if (//vec_height[0] >= ALG_BOLTPIN_Param.ROI02_Min_Height - ALG_BOLTPIN_Param.ROI02_MinMax_Margin
				//&& vec_height[0] <= ALG_BOLTPIN_Param.ROI02_Min_Height + ALG_BOLTPIN_Param.ROI02_MinMax_Margin
				0.5*(vec_height[0] + vec_height[vec_height.size()-1]) >= 0.5*(ALG_BOLTPIN_Param.ROI02_Min_Height+ALG_BOLTPIN_Param.ROI02_Max_Height) - ALG_BOLTPIN_Param.ROI02_MinMax_Margin
				&& 0.5*(vec_height[0] + vec_height[vec_height.size()-1]) <= 0.5*(ALG_BOLTPIN_Param.ROI02_Min_Height+ALG_BOLTPIN_Param.ROI02_Max_Height) + ALG_BOLTPIN_Param.ROI02_MinMax_Margin
				)
			{
				//msg.Format("Min=%d/%d, Max=%d/%d",vec_height[0],ALG_BOLTPIN_Param.ROI02_Min_Height, vec_height[vec_height.size()-1], ALG_BOLTPIN_Param.ROI02_Max_Height);
				//AfxMessageBox(msg);
				return false;
			}
			else
			{
				flip(Gray_Img[Cam_num],Gray_Img[Cam_num],1);
				//flip(Dst_Img[Cam_num],Dst_Img[Cam_num],1);
				//msg.Format("Min=%d/%d, Max=%d/%d",vec_height[0],ALG_BOLTPIN_Param.ROI02_Min_Height, vec_height[vec_height.size()-1],ALG_BOLTPIN_Param.ROI02_Max_Height);
				//AfxMessageBox(msg);
				return true;
			}
		}
	}
	return false;
}