// stdafx.h : ���� ��������� ���� ��������� �ʴ�
// ǥ�� �ý��� ���� ���� �� ������Ʈ ���� ���� ������
// ��� �ִ� ���� �����Դϴ�.
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // ���� ������ �ʴ� ������ Windows ������� �����մϴ�.
#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS      // �Ϻ� CString �����ڴ� ��������� ����˴ϴ�.

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            // ���� ������ �ʴ� ������ Windows ������� �����մϴ�.
#endif
//
#include <afx.h>
#include <afxwin.h>         // MFC �ٽ� �� ǥ�� ���� ����Դϴ�.
#include <atlstr.h>

// TODO: ���α׷��� �ʿ��� �߰� ����� ���⿡�� �����մϴ�.

#include <io.h>
#include <math.h>
#include <afxtempl.h>
#include <afxmt.h>
#include <mmsystem.h>
#include <vector>
#include <cv.h>
//#include <cxcore.h>
//#include <highgui.h>
#include <opencv2/core/utility.hpp>
#include "opencv2/imgproc.hpp"
#include <opencv2/imgcodecs.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/features2d.hpp>
#include <opencv2/video.hpp>
#include "opencv2/opencv.hpp"
#include "opencv2/dnn/dnn.hpp" // AI �߰�
//#include "lc.h"

#include <omp.h>
using namespace cv;
using namespace std;
using namespace cv::dnn; // AI �߰�

#if defined(_DEBUG)			// ����׸���϶�
#pragma comment (lib, "opencv_world3410d.lib")
//#pragma comment (lib, "lc1st.lib")
#else						// ���������϶�
#pragma comment (lib, "opencv_world3410.lib")
//#pragma comment (lib, "lc1st.lib")
#endif
