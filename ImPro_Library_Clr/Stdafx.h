// stdafx.h : 자주 사용하지만 자주 변경되지는 않는
// 표준 시스템 포함 파일 및 프로젝트 관련 포함 파일이
// 들어 있는 포함 파일입니다.

#pragma once
//

#if defined(_DEBUG)			// 디버그모드일때
#pragma comment (lib, "opencv_world3410d.lib")
#else						// 릴리즈모드일때
#pragma comment (lib, "opencv_world3410.lib")
#endif
