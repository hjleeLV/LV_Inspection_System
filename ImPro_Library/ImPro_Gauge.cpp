#include "StdAfx.h"
#include "ImPro_Gauge.h"


CImPro_Gauge::CImPro_Gauge(void)
{
}


CImPro_Gauge::~CImPro_Gauge(void)
{
}

void CImPro_Gauge::Line_Gauge(int x,int y,int width,int height, double x_scale, double y_scale)
{
	if (Gray_Img.empty())
	{
		return;
	}

	blur(Gray_Img, Binary_Img, Size(3,3));
	Canny(Binary_Img, Edge_Img, 30, 30*3, 3);
	Rect ROI;
	ROI.x = x/x_scale;ROI.y = y/y_scale;
	ROI.width = width/x_scale;ROI.height = height/y_scale;
	ROI_Image = Edge_Img(ROI).clone();

	vector<vector<Point> > contours;
	vector<Vec4i> hierarchy;

	findContours( Binary_Img, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );

	vector<Rect> boundRect( contours.size() );
	int m_first_object_num = -1;int m_first_object_value = 0;
	for( int k = 0; k < contours.size(); k++ )
	{  
		boundRect[k] = boundingRect( Mat(contours[k]) );
		if (m_first_object_value<= boundRect[k].width*boundRect[k].height)
		{
			m_first_object_value = boundRect[k].width*boundRect[k].height;
			m_first_object_num = k;
		}
	}
	int m_second_object_num = -1;int m_second_object_value = 0;
	for( int k = 0; k < contours.size(); k++ )
	{  
		if (m_second_object_value<= boundRect[k].width*boundRect[k].height && m_first_object_num != k)
		{
			m_second_object_value = boundRect[k].width*boundRect[k].height;
			m_second_object_num = k;
		}
	}

	struct contour_sorter // 'less' for contours
	{
		bool operator ()( const vector<Point>& a, const vector<Point> & b )
		{
			Rect ra(boundingRect(a));
			Rect rb(boundingRect(b));
			// scale factor for y should be larger than img.width
			return ( (ra.x + 1000*ra.y) < (rb.x + 1000*rb.y) );
		}
	};

	std::sort(contours.begin(), contours.end(), contour_sorter());

	Line_Gauge_Result = "";
	if (m_first_object_num > -1) // 하나만 있을경우
	{
		// 칸투어가 ROI중심에서 좌, 우로 분포하고 있을때
		//if (boundRect[m_first_object_num].x <= ROI.x+ROI.width/2 && 
		//	boundRect[m_first_object_num].x+boundRect[m_first_object_num].width >= ROI.x+ROI.width/2)
		//{
		//	std::sort(contours.begin(), contours.end(), contour_sorter());
		//}
		Line_Gauge_Result.Format(L"Left_%d_%d",contours[m_first_object_num][0].x,contours[m_first_object_num][0].y);
		Line_Gauge_Result.Format(L"%s_Right_%d_%d",Line_Gauge_Result,contours[m_first_object_num][contours[m_first_object_num].size()-1].x,contours[m_first_object_num][contours[m_first_object_num].size()-1].y);
		line( Dst_Img, contours[m_first_object_num][0], contours[m_first_object_num][contours[m_first_object_num].size()-1], Scalar(255,0,0), 1, CV_AA);
		AfxMessageBox(Line_Gauge_Result);
		imshow("Dst_Img",Dst_Img);
	}
}

void CImPro_Gauge::Set_Image(unsigned char* src_u8, long size_x, long size_y, long channel)
{
	if (channel==3)
	{
		Mat src = Mat(size_y, size_x, CV_8UC3, src_u8);
		Src_Img = src.clone();
		cvtColor(Src_Img,Gray_Img,CV_BGR2GRAY);
		Dst_Img = Gray_Img.clone();
	} else if (channel == 1)
	{
		Mat src(size_y, size_x, CV_8UC1, src_u8);
		Src_Img = src.clone();
		Src_Img.copyTo(Gray_Img);
		Dst_Img = Gray_Img.clone();
	}
}