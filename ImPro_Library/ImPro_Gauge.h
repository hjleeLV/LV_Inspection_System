#pragma once
#include "StdAfx.h"
class CImPro_Gauge
{
public:
	CImPro_Gauge(void);
	~CImPro_Gauge(void);

public:
	Mat Src_Img;
	Mat Gray_Img;
	Mat Binary_Img;
	Mat Edge_Img;
	Mat Resize_Img;
	Mat ROI_Image;
	Mat Dst_Img;

	CString Line_Gauge_Result;
	void Set_Image(unsigned char* src_u8, long size_x, long size_y, long channel);
	void Line_Gauge(int x,int y,int width,int height, double x_scale, double y_scale);
};

