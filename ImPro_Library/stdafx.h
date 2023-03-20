// stdafx.h : 자주 사용하지만 자주 변경되지는 않는
// 표준 시스템 포함 파일 및 프로젝트 관련 포함 파일이
// 들어 있는 포함 파일입니다.
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // 거의 사용되지 않는 내용은 Windows 헤더에서 제외합니다.
#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS      // 일부 CString 생성자는 명시적으로 선언됩니다.

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            // 거의 사용되지 않는 내용은 Windows 헤더에서 제외합니다.
#endif
//
#include <afx.h>
#include <afxwin.h>         // MFC 핵심 및 표준 구성 요소입니다.
#include <atlstr.h>

// TODO: 프로그램에 필요한 추가 헤더는 여기에서 참조합니다.

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
#include "opencv2/dnn/dnn.hpp" // AI 추가
//#include "lc.h"

#include <omp.h>
using namespace cv;
using namespace std;
using namespace cv::dnn; // AI 추가

#if defined(_DEBUG)			// 디버그모드일때
#pragma comment (lib, "opencv_world3410d.lib")
//#pragma comment (lib, "lc1st.lib")
#else						// 릴리즈모드일때
#pragma comment (lib, "opencv_world3410.lib")
//#pragma comment (lib, "lc1st.lib")
#endif
