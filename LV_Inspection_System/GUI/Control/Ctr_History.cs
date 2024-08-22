﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LV_Inspection_System.GUI.Control
{
    public partial class Ctr_History : UserControl
    {
        public Ctr_History()
        {
            InitializeComponent();
        }

        public string SW_Version = "V3.8.8 AI (2024.07.09)";
        protected int m_Language = -1; // 언어 선택 0: 한국어 1:영어

        public int m_SetLanguage
        {
            get { return m_Language; }
            set
            {
                if (value == 0 && m_Language != value)
                {// 한국어
                    txtHistory.ResetText();
                    add("[Software Revision History]");
                    add(" ");
                    add(" 2019-06-11 V3.0.6");
                    add(" 영문 변경시 에러 수정, ROI 설정 변수창 기본 숨김, 숨김/보이기 버튼 추가, 수율화면 추가");
                    add(" ");
                    add(" 2019-06-04 V3.0.5");
                    add(" 검사시작시 카메라 이미지 리셋, 검사시작시 메뉴 작동 옵션, 직경 최대, 최소 측정 추가");
                    add(" ");
                    add(" 2019-05-29 V3.0.4");
                    add(" 합쳐보내기 및 보내는 index 유지 개선, 사각 BLOB 결과를 사각형에서 contour로 변경");
                    add(" ");
                    add(" 2019-05-27 V3.0.3");
                    add(" RESET 검사중 비활성");
                    add(" ");
                    add("* 2018-11-16 V2.8.6");
                    add(" 씨엠티 관련 로직 추가, Pylon5 버젼으로 업데이트");
                    add(" ");
                    add("* 2018-10-19 V2.8.2");
                    add(" ROI Check 시 안정성 강화, 모델 생성시 메인창 깨짐 수정");
                    add(" ");
                    add("* 2018-10-11 V2.8.1");
                    add(" ROI Check 시 안정성 강화, 저장,로드시 알림창 띄우기");
                    add(" ");
                    add("* 2018-10-04 V2.8.0");
                    add(" Index Side일때 회전 보정 및 Index판기준 회전보정 추가");
                    add(" ");
                    add("* 2018-10-03 V2.7.9");
                    add(" 가이드 없을때 이미지 그리기 버그 수정 및 회전 로직 변경");
                    add(" ");
                    add("* 2018-09-28 V2.7.8");
                    add(" 편의성 향상 수정");
                    add(" ");
                    add("* 2018-09-27 V2.7.7");
                    add(" 사각블랍에 7:좌우,상하 두께 차이 추가");
                    add(" ");
                    add("* 2018-09-13 V2.7.6");
                    add(" 비교 알고리즘 수정");
                    add(" ");
                    add("* 2018-09-10 V2.7.5");
                    add(" 요청사항 수정");
                    add(" ");
                    add("* 2018-08-30 V2.7.4");
                    add(" 임계화 방법 선택시 관련 사항만 표시되게 수정, 예비 변수 항목을 고급기능 사용시 보이게 수정");
                    add(" ");
                    add("* 2018-08-23 V2.7.3");
                    add(" NG Log check box, 십자 치수 알고리즘 수정");
                    add(" ");
                    add("* 2018-08-20 V2.7.2");
                    add(" ROI 기준에 상부 중심 추가");
                    add(" ");
                    add("* 2018-08-10 V2.7.1");
                    add(" 로그 항목에 NG Log 추가");
                    add(" ");
                    add("* 2018-08-07 V2.7.0");
                    add(" NG 영상만 display 할 수 있는 옵션 추가");
                    add(" ");
                    add("* 2018-07-20 V2.6.9");
                    add(" 원형 ROI에서 크기 필터링 오류 수정");
                    add(" ");
                    add("* 2018-07-13 V2.6.8");
                    add(" 딜레이 판단시 CLASS 적용문제 수정, 종료시 자동 저장 기능 삭제");
                    add(" ");
                    add("* 2018-06-14 V2.6.7");
                    add(" 몸통휨에 상부와 몸통 사이각도 출력 옵션 및 알고리즘 추가");
                    add(" ");
                    add("* 2018-06-08 V2.6.6");
                    add(" 몸통휨에 몸통 각도 출력 옵션 및 알고리즘 추가");
                    add(" ");
                    add("* 2018-05-28 V2.6.5");
                    add(" ROI 복사, 붙여넣기 기능 추가 / 카메라별 ROI 선택 독립적으로 작동 / 기타 버그 수정");
                    add(" ");
                    add("* 2018-05-03 V2.6.4");
                    add(" 모델 복원 기능 추가");
                    add(" ");
                    add("* 2018-04-27 V2.6.3");
                    add(" 알고리즘 버그 수정 및 실시간 DB 갱신");
                    add(" ");
                    add("* 2018-04-09 V2.6.2");
                    add(" 카메라 사용 유무에 따른 control enable/disable");
                    add(" ");
                    add("* 2018-04-06 V2.6.1");
                    add(" 사이드 회전 옵션 추가 / 모델 비교 회전 에러 수정 / 네트웍 TCP/IP 서버, 클라이언트 구현 / 기타 버그 수정");
                    add(" ");
                    add("* 2018-03-30 V2.6.0");
                    add(" MySQL DB 연결 구현 / 기타 버그 수정");
                    add(" ");
                    add("* 2018-03-15 V2.5.9");
                    add(" 직경, 진원도 로직 수정 / 기타 버그 수정");
                    add(" ");
                    add("* 2018-03-13 V2.5.8");
                    add(" 모델에 백업 버튼 추가, 기타 버그 수정");
                    add(" 로그에 수율 그래프 추가, 갱신 시간은 로그 셋팅에서 조정");
                    add(" ROI설정 table에서 오른쪽 마우스 클릭 시 검사 변수 초기화 메뉴 추가");
                    add(" ROI 검사 실시간 Thread 적용, 원형 ROI 시작각, 종료각 -360도 옵션 적용");
                    add(" ");
                    add("* 2018-03-02 V2.5.7");
                    add(" 원형 ROI에 원 2개 적용, 옵션 : 원#1(v1 이하), 원#2(v2 이상) 임계화 추가");
                    add(" 볼록 BLOB 차이 삭제 / 볼록 BLOB 차이는 원형, 사각 ROI BLOB의 옵션으로 합쳐짐");
                    add(" ");
                    add("* 2018-02-27 V2.5.6");
                    add(" 원형 ROI 알고리즘 최적화 / 버그 수정");
                    add(" ");
                    add("* 2018-02-23 V2.5.5");
                    add(" 원형 ROI 알고리즘 통합");
                    add(" ");
                    add("* 2018-02-20 V2.5.4");
                    add(" 변수 설정 및 퀵 메뉴 버그 수정");
                    add(" ");
                    add("* 2018-02-02 V2.5.3");
                    add(" 도움말 메뉴 추가");
                    add(" ");
                    add("* 2018-01-18 V2.5.2");
                    add(" 프로그램 수정사항 01-12.ppt 기준 수정");
                    add(" ");
                    add("* 2018-01-05 V2.5.1");
                    add(" 프로그램 수정사항 01-03.ppt 기준 수정");
                    add(" ");
                    add("* 2017-12-29 V2.5.0");
                    add(" 측면 머리 width 단위를 pixel에서 mm로 변경, ROI 설정에서 카메라 이동 조작 추가.");
                    add(" ");
                    add("* 2017-12-28 V2.4.9");
                    add(" 모델 회사 소개 수정, 기타 요청 사항 수정");
                    add(" ");
                    add("* 2017-12-08 V2.4.8");
                    add(" ROI 설정에서 카메라 gain, expo., 임계값을 조정 할 수 있도록 실시간 모드 추가, 0.5초에 한번 카메라 이미지 갱신");
                    add(" ");
                    add("* 2017-12-08 V2.4.7");
                    add(" 원형 ROI 위치옵션에 #2 원 삭제하고 계산 할 시작, 종료각 추가, 각도에 입력하면 3시기준 0도이며 +-입력값으로 계산(입력범위:-360~360)");
                    add(" ");
                    add("* 2017-12-07 V2.4.6");
                    add(" 원형 Clor ROI 위치옵션에 3:계산각도 추가. 각도에 입력하면 12시기준 +-입력값으로 계산(입력범위:-360~+360)");
                    add(" ");
                    add("* 2017-11-29 V2.4.5");
                    add(" 십자 및 다각형 치수 로직 업그레이드(반경측정 추가)");
                    add(" ");
                    add("* 2017-11-27 V2.4.4");
                    add(" 십자 치수 다각형 치수 mm로 출력");
                    add(" ");
                    add("* 2017-11-06 V2.4.3");
                    add(" 사각 ROI BLOB에 컬러 전처리 추가, 기준위치와 거리 계산 추가");
                    add(" ");
                    add("* 2017-11-01 V2.4.2");
                    add(" 프로그램 중복 실행 방지 ");
                    add(" 사이드 ROI 설정 사각 ROI BLOB 옵션 적용 ");
                    add(" ");
                    add("* 2017-10-26 V2.4.1");
                    add(" 사이드 검사 알고리즘에 나사선 크기에 3:볼록블랍 추가 ");
                    add(" 기본 모델 생성시 에러 수정 ");
                    add(" ");
                    add("* 2017-10-11 V2.4.0");
                    add(" 사각 ROI BLOB 크기/개수 로직에 계산방법 옵션추가(0:픽셀수,1:가로길이,2:세로길이,3:개수)");
                    add(" ");
                    add("* 2017-09-22 V2.3.9");
                    add(" 십자 치수(mm) 항목에 옵션으로 다각형 치수 측정으로 수정");
                    add(" ROI설정에 내외경 중심 차이 알고리즘 추가");
                    add(" ");
                    add("* 2017-09-19 V2.3.8");
                    add(" 십자 치수(mm) 항목에 옵션으로 6각 치수 측정 추가");
                    add(" ");
                    add("* 2017-09-14 V2.3.7");
                    add(" 스기야마 offset 버그 수정");
                    add(" ");
                    add("* 2017-09-05 V2.3.6");
                    add("  검사 정지, 리셋 버튼 클릭시 실행 여부 박스 띄움, 카메라 사용 안 할때 카메라 연동 해제");
                    add(" ");
                    add("* 2017-09-04 V2.3.5");
                    add("  사각 ROI의 BLOB 크기에 계산방법(0:픽셀수,1:가로길이,2:세로길이,3:개수) 추가");
                    add("  버그 수정");
                    add(" ");
                    add("* 2017-08-31 V2.3.4");
                    add("  임계화 V1이하, V2이상 추가");
                    add("  볼록 BLOB 검출 알고리즘 추가");
                    add(" ");
                    add("* 2017-08-25 V2.3.3");
                    add("  측면 검사 항목에 두 영역 중심간 거리 추가");
                    add("  상하부 검사 항목에 원형 ROI 색상 BLOB 추가");
                    add(" ");
                    add("* 2017-08-22,23 V2.3.2");
                    add("  ROI 설정 내 로딩시 항목 변경 에러 수정");
                    add(" ");
                    add("* 2017-08-18 V2.3.1");
                    add("  모델 저장/로드시 안정성 향상");
                    add(" ");
                    add("* 2017-08-16 V2.3.0");
                    add("  모델 저장시 ROI 일부 항목 바뀜 문제 수정");
                    add("  카메라 동작시 offset값 불러오기 가능하도록 수정");
                    add(" ");
                    add("* 2017-08-10 V2.2.9");
                    add("  스기야마 고객사 번호 팝업 오류 수정, ROI 검사영역설정 문구 영문으로 수정)");
                    add(" ");
                    add("* 2017-08-07 V2.2.8");
                    add("  사각 ROI 영상처리에 전처리 추가");
                    add("  카메라 연동 추가(카메라 설정에서 카메라 연동할 다른 카메라를 고르면 그 카메라의 이미지를 가지고 검사함)");
                    add(" ");
                    add("* 2017-07-24 V2.2.7");
                    add("  ROI 사용유무에 따라 항목뷰 변경 되도록 수정");
                    add("  카메라 이미지 저장시 24비트로 저장");
                    add("  카메라 카운트 최대치 double로 변경");
                    add(" ");
                    add("* 2017-07-17 V2.2.6");
                    add("  원형 ROI BLOB, count의 중복계산 오류 수정");
                    add(" ");
                    add("* 2017-07-13 V2.2.5");
                    add("  카메라 해상도 수정 적용시 카메라 꺼짐 방지");
                    add("  직경 알고리즘 수정");
                    add(" ");
                    add("* 2017-06-28 V2.2.4");
                    add("  ROI 설정 저장, 로드시 변수값 바뀜 수정");
                    add(" ");
                    add("* 2017-06-25 V2.2.3");
                    add("  카메라 설정에서 이미지 저장 버튼 추가");
                    add(" ");
                    add("* 2017-06-20 V2.2.2");
                    add("  원형 ROI BLOB크기, 개수에 기준 재설정 기능 추가");
                    add("  0:ROI#0기준을 따름, 1:P임계값 이하중 최대 큰것을 기준으로 중심을 잡음, 2:P임계값 이상중 최대 큰것을 기준으로 중심을 잡음");
                    add(" ");
                    add("* 2017-06-16 V2.2.1");
                    add("  고객사 전용 모드 추가(고객사 번호 입력시 작동)");
                    add(" ");
                    add("* 2017-06-08 V2.2.0");
                    add("  ROI 20개 추가");
                    add("  ROI설정에서 Table [ROI 기준 측정] 항목 추가(검사 영역내 ROI 따라다니는 기능 없이 설정된 ROI내 측정)");
                    add(" ");
                    add("* 2017-06-04 V2.1.9");
                    add("  PLC 설정에 검사 대상 없을때 내 보낼 클래스 설정하는 콤보 박스 추가");
                    add(" ");
                    add("* 2017-05-30 V2.1.8");
                    add("  불량 Class 적용");
                    add(" ");
                    add("* 2017-05-29 V2.1.7");
                    add("  사각 영역의 밝기에 컬러 처리 추가");
                    add("  카메라 설정에 컬러 카메라 추가");
                    add(" ");
                    add("* 2017-05-26 V2.1.6");
                    add("  사각 ROI BLOB 크기/개수 계산 필터 변수 추가 및 로직 수정");
                    add(" ");
                    add("* 2017-05-25 V2.1.5");
                    add("  사이드 검사 미스 버그 수정");
                    add(" ");
                    add("* 2017-05-16 V2.1.4");
                    add("  상/하부 검사시 나사선 크기 추가");
                    add(" ");
                    add("* 2017-05-15 V2.1.3");
                    add("  상/하부 검사시 회전 보정 기능 추가");
                    add("  상/하부 나사선 피치 계산 추가");
                    add("  상/하부 두 영역 거리 계산 추가");
                    add(" ");
                    add("* 2017-05-11 V2.1.2");
                    add("  언어 변경시 ROI 설정 에러 재수정.");
                    add(" ");
                    add("* 2017-05-10 V2.1.1");
                    add("  언어 변경시 ROI 설정 에러 수정.");
                    add(" ");
                    add("* 2017-05-08 V2.1.0");
                    add("  모델 설정에서 한국어, 영어 선택하여 자동 언어 변경 구현.");
                    add(" ");
                    add("* 2017-05-02 V2.0.9");
                    add("  PLC: Once Tx after inspection 모드 추가");
                    add("  (모드 사용시 교차 전송 사용 불가, 입력된 delay동안 추가 검사 없을 시 결과 전송, 하나라도 NG이면 NG로 전송됨.)");
                    add(" ");
                    add("* 2017-05-01 V2.0.8");
                    add("  Log folder 생성 오류 수정");
                    add(" ");
                    add("* 2017-04-29 V2.0.7");
                    add("  S/W 버그 종합 수정 from HM,Kim");
                    add(" ");
                    add("* 2017-04-25 V2.0.6");
                    add("  원형 영역의 BLOB 크기, 원형 영역의 BLOB 갯수에 경계 연결 수 변수 추가");
                    add("  (0이면 모두, 1이면 경계가 1개이상 걸릴 때, 2이면 경계가 2개 걸릴 때)");
                    add(" ");
                    add("* 2017-04-24 V2.0.5");
                    add("  휨 검사 합산 기능 추가(카메라별로 측정 아이템에 '휨', '합산' 명칭이 있는 것은 합산하여 판정 함.");
                    add(" ");
                    add("* 2017-04-21 V2.0.4");
                    add("  카메라 이미지에 따른 예외 처리 추가");
                    add(" ");
                    add("* 2017-04-20 V2.0.3");
                    add("  CAM3 통신, 저장관련 버그 수정");
                    add("  나사선 붙음 제거 변수 추가");
                    add(" ");
                    add("* 2017-04-18 V2.0.2");
                    add("  Camera 사용에 따른 화면분할 추가");
                    add("  벨트 타입, 가이드 없는 타입 ROI설정 및 알고리즘 추가");
                    add(" ");
                    add("* 2017-04-13 V2.0.1");
                    add("  운용 SW 버그 수정");
                    add("  사이드 휨검사 로직 수정(상단 3포인트 중심, 하단 3포인트 중심 연결 직선과 차이 계산)");
                    add(" ");
                    add("* 2017-04-11 V2.0.0");
                    add("  Start management of revision history.");
                    add("  Initial version is released.");
                    add(" ");
                    add("  Started S/W development by CDJung.");
                    add("  From Feb. 2017.");
                }
                else if (value == 1 && m_Language != value)
                {// 영어
                    txtHistory.ResetText();
                    add("[Software Revision History]");
                    add(" ");
                    add("* 2018-08-30 V2.7.4");
                    add(" Related item visible control in threshold, added advenced parameter visible control");
                    add(" ");
                    add("* 2018-08-23 V2.7.3");
                    add(" Added NG Log check box in LOG setting, modified cross size algorithm");
                    add(" ");
                    add("* 2018-08-20 V2.7.2");
                    add(" Added Top Center in ROI standard");
                    add(" ");
                    add("* 2018-08-10 V2.7.1");
                    add(" Added NG Log at LOG");
                    add(" ");
                    add("* 2018-08-07 V2.7.0");
                    add(" Added option : only display NG image");
                    add(" ");
                    add("* 2018-07-20 V2.6.9");
                    add(" Fixed bug filtering of size in Circle ROI ");
                    add(" ");
                    add("* 2018-07-13 V2.6.8");
                    add(" Fixed CLASS applying when judged by delay, remove auto saving when program exit");
                    add(" ");
                    add("* 2018-06-14 V2.6.7");
                    add(" Added output option(Angle between Top and Body) and algorithm in body bending");
                    add(" ");
                    add("* 2018-06-08 V2.6.6");
                    add(" Added output option(Body Angle) and algorithm in body bending");
                    add(" ");
                    add("* 2018-05-28 V2.6.5");
                    add(" Added new function : ROI copy, paste / ROI select independently as camera / bug fixed");
                    add(" ");
                    add("* 2018-05-03 V2.6.4");
                    add(" Added model restoring function");
                    add(" ");
                    add("* 2018-04-27 V2.6.3");
                    add(" Fixed bug / realtime DB update");
                    add(" ");
                    add("* 2018-04-09 V2.6.2");
                    add(" Control enable/disable according to use camera");
                    add(" ");
                    add("* 2018-04-06 V2.6.1");
                    add(" Added rotation option at side / fixed error on model find / implemented network TCP/IP server, client / bug fixed");
                    add(" ");
                    add("* 2018-03-30 V2.6.0");
                    add(" Implemented MySQL DB connection / Bug fixed");
                    add(" ");
                    add("* 2018-03-15 V2.5.9");
                    add(" Modified algorithm of Diameter, Circularity / Bug fixed");
                    add(" ");
                    add("* 2018-03-13 V2.5.8");
                    add(" Added backup button in model, etc bug fixed");
                    add(" Added graph of yield at log tap, adjustable updating time in log setting");
                    add(" Added parameters initialize menu on parameter table at ROI setting");
                    add(" Applied thread on inspection in ROI setting, insert -360 degree option at circle ROI");
                    add(" ");
                    add("* 2018-03-02 V2.5.7");
                    add(" Add circle ROI #2, add option : #1(less than v1), #2(more than v2) threshold");
                    add(" Removed convex BLOB difference / convex BLOB difference is merged option in circle, rectangle ROI BLOB");
                    add(" ");
                    add("* 2018-02-27 V2.5.6");
                    add(" Optimized algorithm in circle ROI / fixed bug");
                    add(" ");
                    add("* 2018-02-23 V2.5.5");
                    add(" Intergrated algorithm in circle ROI");
                    add(" ");
                    add("* 2018-02-20 V2.5.4");
                    add(" Fixed bug(ROI setting and quick menu)");
                    add(" ");
                    add("* 2018-02-02 V2.5.3");
                    add(" Added Help menu");
                    add(" ");
                    add("* 2018-01-18 V2.5.2");
                    add(" Modified based on requirement 01-12.ppt");
                    add(" ");
                    add("* 2018-01-05 V2.5.1");
                    add(" Modified based on requirement 01-03.ppt");
                    add(" ");
                    add("* 2017-12-29 V2.5.0");
                    add(" Change the unit of head width from pixel to mm in side algorithm, added camera operation buttons in ROI setting");
                    add(" ");
                    add("* 2017-12-28 V2.4.9");
                    add(" Modified introduction of company in model");
                    add(" ");
                    add("* 2017-12-08 V2.4.8");
                    add(" Added camera control, threshold changing trackbar in ROI settting, realtime on/off mode");
                    add(" ");
                    add("* 2017-12-08 V2.4.7");
                    add(" Removed #2 circle, added angle option at circle roi. standard 3hr direction, angle value(-360~360)");
                    add(" ");
                    add("* 2017-12-07 V2.4.6");
                    add(" Added angle option at circle color roi. standard 12hr direction, +- angle value(-360~+360)");
                    add(" ");
                    add("* 2017-11-29 V2.4.5");
                    add(" Upgraded algorithm of cross & polygon measurement(Added diameter option)");
                    add(" ");
                    add("* 2017-11-27 V2.4.4");
                    add(" Changed cross measurement unit from pixel to mm");
                    add(" ");
                    add("* 2017-11-06 V2.4.3");
                    add(" Added color preprocessing, distance options for side rectangle ROI BLOB");
                    add(" ");
                    add("* 2017-11-01 V2.4.2");
                    add(" Prevent running over 2 program");
                    add(" Apply options for side rectangle ROI BLOB");
                    add(" ");
                    add("* 2017-10-26 V2.4.1");
                    add(" Added logic for convex blob in side inspection");
                    add(" Error fixed when create a new basic model");
                    add(" ");
                    add("* 2017-10-11 V2.4.0");
                    add(" Added calculation method at Rect ROI BLOB Size/Count(0:Pixel,1:Width,2:Height,3:Count)");
                    add(" ");
                    add("* 2017-09-22 V2.3.9");
                    add(" Modified from distance of 6 angle to distance of poly-angle at Cross measurement");
                    add(" Added center difference between Inner and outter circle at ROI setting");
                    add(" ");
                    add("* 2017-09-19 V2.3.8");
                    add(" Added distance of 6 angle at Cross measurement");
                    add(" ");
                    add("* 2017-09-14 V2.3.7");
                    add(" Bug fixed offset error in Sugiyama");
                    add(" ");
                    add("* 2017-09-05 V2.3.6");
                    add("  Open the question box when STOP, RESET button click, release camera corelation when disable the camera");
                    add(" ");
                    add("* 2017-09-04 V2.3.5");
                    add("  Added output(0:pixel,1:width,2:height,3:count) in Rectangle ROI BLOB size");
                    add("  Some bugs fixed");
                    add(" ");
                    add("* 2017-08-31 V2.3.4");
                    add("  Added less V1, more V2 in threshold method");
                    add("  Added convex BLOB algorithm");
                    add(" ");
                    add("* 2017-08-25 V2.3.3");
                    add("  Added Distance between two area in side ROI");
                    add("  Added circle ROI color blob in top-bottom ROI");
                    add(" ");
                    add("* 2017-08-22,23 V2.3.2");
                    add("  Error fixed when ROI loading");
                    add(" ");
                    add("* 2017-08-18 V2.3.1");
                    add("  Improved safe save / load");
                    add(" ");
                    add("* 2017-08-16 V2.3.0");
                    add("  Fixed ROI data changing error when save model");
                    add("  Modified possible to load camera offset when camera operating");
                    add(" ");
                    add("* 2017-08-10 V2.2.9");
                    add("  Customer number popup error fixed, modified inspection area setting in English on ROI setting)");
                    add(" ");
                    add("* 2017-08-07 V2.2.8");
                    add("  Added preprocessing(blur) parameter in rectangle ROI");
                    add("  Interlocking camera function added at setting of camera");
                    add(" ");
                    add("* 2017-07-24 V2.2.7");
                    add("  Modified item view on the table as changing roi usage");
                    add("  Save image format is 24bitRGB at camera setting");
                    add("  Fixed double type for the count of camera");
                    add(" ");
                    add("* 2017-07-17 V2.2.6");
                    add("  Error fixed double counting at circle ROI BLOB, count");
                    add(" ");
                    add("* 2017-07-13 V2.2.5");
                    add("  Prevent turn off camera after changing resolution");
                    add("  Modified diameter algorithm");
                    add(" ");
                    add("* 2017-06-28 V2.2.4");
                    add("  Improved a bug of ROI setting save, load.");
                    add(" ");
                    add("* 2017-06-25 V2.2.3");
                    add("  Added image save button at camera setting");
                    add(" ");
                    add("* 2017-06-20 V2.2.2");
                    add("  Added parameters for recentering ROI");
                    add("  0:ROI#0, 1:P Threshold below, 2:P Threshold above");
                    add(" ");
                    add("* 2017-06-16 V2.2.1");
                    add("  Added customer mode(Type customer number)");
                    add(" ");
                    add("* 2017-06-08 V2.2.0");
                    add("  Added ROI 20ea");
                    add("  Added Table [Measure by ROI] item(No tracing, measurement in setting ROI)");
                    add(" ");
                    add("* 2017-06-04 V2.1.9");
                    add("  Added a combobox of Tx for classifying the no object.");
                    add(" ");
                    add("* 2017-05-30 V2.1.8");
                    add("  Applied error Class.");
                    add(" ");
                    add("* 2017-05-29 V2.1.7");
                    add("  Color processing added at Brightness of rectangle ROI");
                    add("  Color camera was included at camera setting.");
                    add(" ");
                    add("* 2017-05-26 V2.1.6");
                    add("  Added parameters and modified algorithm for rect ROI BLOB size/CNT");
                    add(" ");
                    add("* 2017-05-25 V2.1.5");
                    add("  Bug fixed at side algorithm");
                    add(" ");
                    add("* 2017-05-16 V2.1.4");
                    add("  Added size of thread at Top/Bottom inspection.");
                    add(" ");
                    add("* 2017-05-15 V2.1.3");
                    add("  Added function which calibrates rotation at Top/Bottom inspection.");
                    add("  Added pitch of thread at Top/Bottom inspection.");
                    add("  Added distance of two ROI at Top/Bottom inspection.");
                    add(" ");
                    add("* 2017-05-11 V2.1.2");
                    add("  Bug fixed adgain in changing language.");
                    add(" ");
                    add("* 2017-05-10 V2.1.1");
                    add("  Bug fixed in changing language.");
                    add(" ");
                    add("* 2017-05-08 V2.1.0");
                    add("  Implemented language selection: Korean, English");
                    add(" ");
                    add("* 2017-05-02 V2.0.9");
                    add("  PLC: Added Once Tx after inspection mode");
                    add("  (Can't use Tx in turn mode when enable this, After delay time, send Tx message if there is no more insplection, if there are only one NG, result will be NG.)");
                    add(" ");
                    add("* 2017-05-01 V2.0.8");
                    add("  Bug fixed at Log folder creation");
                    add(" ");
                    add("* 2017-04-29 V2.0.7");
                    add("  Bus fixed in S/W from HM,Kim");
                    add(" ");
                    add("* 2017-04-25 V2.0.6");
                    add("  Boundary condition added at BLOB size in circle ROI, BLOB count in circle ROI");
                    add("  (0: find all, 1: connected one side, 2: connected two side)");
                    add(" ");
                    add("* 2017-04-24 V2.0.5");
                    add("  Added bending inspection algorithm(Measurement name have to include 'bending', 'sum'.");
                    add(" ");
                    add("* 2017-04-21 V2.0.4");
                    add("  Error code added regarding camera image");
                    add(" ");
                    add("* 2017-04-20 V2.0.3");
                    add("  CAM3 PLC, logging bug fixed");
                    add("  Added preventing thread paste parameter");
                    add(" ");
                    add("* 2017-04-18 V2.0.2");
                    add("  Display slit mode added");
                    add("  Added belt type, no guide type in ROI");
                    add(" ");
                    add("* 2017-04-13 V2.0.1");
                    add("  Bug fixed in S/W");
                    add("  Modified rotation algorithm in side(Use top 3 points, botton 3 points)");
                    add(" ");
                    add("* 2017-04-11 V2.0.0");
                    add("  Start management of revision history.");
                    add("  Initial version is released.");
                    add(" ");
                    add("  Started S/W development by CDJung.");
                    add("  From Feb. 2017.");
                }
                m_Language = value;
            }
        }

        private void Ctr_History_Load(object sender, EventArgs e)
        {

        }

        private void add(string message)
        {
            txtHistory.Text += message + "\r\n";
            txtHistory.SelectionStart = txtHistory.Text.Length;
            txtHistory.ScrollToCaret();
            txtHistory.Refresh();
        }
    }
}
